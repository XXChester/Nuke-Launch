using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;
using GWNorthEngine.Audio;

using NukeLaunch.Logic;
using NukeLaunch.Managers;
namespace NukeLaunch.Models {
	public abstract class Launcher {
		protected enum Direction {
			Right,
			Left,
		}
		protected enum RotationDirection {
			Up,
			Down
		}
		#region Class variables
		protected Direction direction;
		protected float angle;
		protected float power;
		protected StaticDrawable2D truck;
		protected StaticDrawable2D leftBarrel;
		protected StaticDrawable2D rightBarrel;
		protected StaticDrawable2D activeBarrel;

		private float previousAngle;
		private SFXEngine sfxEngine;
		private SoundEffect launchSFX;
		private SoundEffect aimingSFX;
		private Color[,] textureColourData;
		private ContentManager content;
		private NukeDelegate nukeDelegate;
		private NextTurnDelegate turnDelegate;
		protected const float BARREL_OFF_SET = 15f;
		protected const float MAX_POWER = 100f;
		protected const float MIN_POWER = 20f;
		protected const float POWER_INCREASE = 1f;
		private const float TRUCK_CAB_CLEARANCE = 70f;
		private const float MAX_ANGLE = 5f;
		private const float SHIFT_PER_SECOND = 750f / 1000f;
		#endregion Class variables

		#region Class propeties
		public Color[,] TextureColourData { get { return this.textureColourData; } }
		public Vector2 Position { get { return this.truck.Position; } }
		public Matrix Matrix { get; set; }
		public int ID { get; set; }
		#endregion Class properties

		#region Constructor
		public Launcher(ContentManager content, SFXEngine sfxEngine, Vector2 position, NukeDelegate nukeDelegate, NextTurnDelegate turnDelegate, int ID) {
			this.content = content;
			this.angle = 90f;
			this.direction = Direction.Right;
			this.nukeDelegate = nukeDelegate;
			this.turnDelegate = turnDelegate;
			this.power = 100f;
			this.ID = ID;

			StaticDrawable2DParams staticParms = new StaticDrawable2DParams();
			staticParms.Position = position;
			staticParms.Origin = new Vector2(32f);
			staticParms.Texture = LoadingUtils.loadTexture2D(content, "LauncherTruck");
			this.truck = new StaticDrawable2D(staticParms);

			staticParms.Position = new Vector2(this.truck.Position.X - this.truck.Origin.X, this.truck.Position.Y + BARREL_OFF_SET);
			staticParms.Texture = LoadingUtils.loadTexture2D(content, "LauncherBarrelRight");
			staticParms.Origin = new Vector2(43f, 62f);
			staticParms.Scale = new Vector2(.75f);
			staticParms.Rotation = MathHelper.ToRadians(this.angle);
			this.rightBarrel = new StaticDrawable2D(staticParms);

			staticParms.Position = new Vector2(this.truck.Position.X + this.truck.Origin.X, this.truck.Position.Y + BARREL_OFF_SET);
			staticParms.Texture = LoadingUtils.loadTexture2D(content, "LauncherBarrelLeft");
			staticParms.Origin = new Vector2(21f, 65f);
			this.leftBarrel = new StaticDrawable2D(staticParms);

			this.activeBarrel = this.rightBarrel;
			this.textureColourData = TextureUtils.getColourData2D(this.truck.Texture);
			setMatrix();

			// sfx
			this.sfxEngine = sfxEngine;
			this.launchSFX = LoadingUtils.loadSoundEffect(content, "Launch");
			this.aimingSFX = LoadingUtils.loadSoundEffect(content, "Aiming");
		}
		#endregion Constructor

		#region Support methods
		private void setMatrix() {
			// we want the negative origin because the axis are mirrored
			Matrix origin = Matrix.CreateTranslation(new Vector3(-this.truck.Origin, 0f));
			Matrix position = Matrix.CreateTranslation(new Vector3(this.truck.Position, 0f));
			this.Matrix = origin * position;

			//*****IMPORTANT***** Standard calculation
			//this.Matrix = origin * rotation * scale * position;
			//But we only use the origin and position because you only factor in the pieces you are using
			
		}

		protected void updateDirection() {
			if (this.direction == Direction.Left) {
				this.truck.SpriteEffect = SpriteEffects.FlipHorizontally;
				this.activeBarrel = this.leftBarrel;
			} else if (this.direction == Direction.Right) {
				this.truck.SpriteEffect = SpriteEffects.None;
				this.activeBarrel = this.rightBarrel;
			}
			float preAngle = this.angle;
			this.angle = (this.angle - this.angle) - preAngle;


			/*if (InputManager.getInstance().wasKeyPressed(Keys.A)) {
				if (base.direction != Direction.Left) {
					base.direction = Direction.Left;
					base.truck.SpriteEffect = SpriteEffects.FlipHorizontally;
					base.activeBarrel = base.leftBarrel;
					float preAngle = base.angle;
					base.angle = (base.angle - base.angle) - preAngle;
				}
			} else if (InputManager.getInstance().wasKeyPressed(Keys.D)) {
				if (base.direction != Direction.Right) {
					base.direction = Direction.Right;
					base.truck.SpriteEffect = SpriteEffects.None;
					base.activeBarrel = base.rightBarrel;
					float preAngle = base.angle;
					base.angle = (base.angle - base.angle) - preAngle;
				}
			}*/
		}

		protected void rotate(RotationDirection rotationDirection) {
			if (rotationDirection == RotationDirection.Up) {
				this.angle += 1f;
			} else if (rotationDirection == RotationDirection.Down) {
				this.angle -= 1f;
			}

			if (this.direction == Direction.Right) {
				this.angle = MathHelper.Clamp(this.angle, MAX_ANGLE, TRUCK_CAB_CLEARANCE);
			} else if (this.direction == Direction.Left) {
				this.angle = MathHelper.Clamp(this.angle, -TRUCK_CAB_CLEARANCE, -MAX_ANGLE);
			}

			if (this.angle != this.previousAngle) {
				if (!this.sfxEngine.isPlaying(this.aimingSFX.Name)) {
					this.sfxEngine.playSoundEffect(this.aimingSFX);
				}
			}
		}

		public void shiftDown(float elapsed) {
			float change = (SHIFT_PER_SECOND * elapsed);
			this.truck.Position = new Vector2(this.truck.Position.X, this.truck.Position.Y + change);
			this.leftBarrel.Position = new Vector2(this.leftBarrel.Position.X, this.leftBarrel.Position.Y + change);
			this.rightBarrel.Position = new Vector2(this.rightBarrel.Position.X, this.rightBarrel.Position.Y + change);
			setMatrix();
		}

		public void fire() {
			this.sfxEngine.playSoundEffect(this.launchSFX);
			this.nukeDelegate.Invoke(
				this.activeBarrel.Position, this.activeBarrel.Origin, this.activeBarrel.Rotation, this.power / 7f, this.ID);
			this.turnDelegate.Invoke(this.ID);
		}

		public virtual void update(float elapsed) {
			if (this.direction == Direction.Right) {
				this.angle = MathHelper.Clamp(this.angle, MAX_ANGLE, TRUCK_CAB_CLEARANCE);
			} else if (this.direction == Direction.Left) {
				this.angle = MathHelper.Clamp(this.angle, -TRUCK_CAB_CLEARANCE, -MAX_ANGLE);
			}
			
			this.activeBarrel.Rotation = MathHelper.ToRadians(angle);
			this.previousAngle = this.angle;
		}

		public virtual  void render(SpriteBatch spriteBatch) {
			if ((int)StateManager.getInstance().WhosTurn == this.ID) {
				this.activeBarrel.render(spriteBatch);
			}
			this.truck.render(spriteBatch);
		}
		#endregion Support methods
	}
}
