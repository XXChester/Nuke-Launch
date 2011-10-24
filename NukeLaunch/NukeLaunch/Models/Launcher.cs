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
using NukeLaunch.Logic;
namespace NukeLaunch.Models {
	public abstract class Launcher {
		protected enum Direction {
			Right,
			Left,
		}
		#region Class variables
		protected Vector2 position;
		protected Direction direction;
		protected float angle;
		protected float power;
		protected StaticDrawable2D truck;
		protected StaticDrawable2D leftBarrel;
		protected StaticDrawable2D rightBarrel;
		protected StaticDrawable2D activeBarrel;
		private Color[,] textureColourData;
		private ContentManager content;
		private NukeDelegate nukeDelegate;
		protected const float BARREL_OFF_SET = 15f;
		protected const float MAX_POWER = 100f;
		protected const float MIN_POWER = 20f;
		protected const float POWER_INCREASE = 1f;
		private const float TRUCK_CAB_CLEARANCE = 70f;
		private const float MAX_ANGLE = 5f;
		private const float SHIFT_PER_SECOND = 1f;//500f / 1000f;
		#endregion Class variables

		#region Class propeties
		public Color[,] TextureColourData { get { return this.textureColourData; } }
		public BoundingBox BBox { get; set; }
		public Matrix Matrix { get; set; }
		#endregion Class properties

		#region Constructor
		public Launcher(ContentManager content, Vector2 position, NukeDelegate nukeDelegate) {
			this.content = content;
			this.angle = 90f;
			this.direction = Direction.Right;
			this.nukeDelegate = nukeDelegate;
			this.power = 100f;

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
			this.BBox = new BoundingBox(new Vector3(Vector2.Subtract(this.truck.Position, new Vector2(this.truck.Origin.X,
				this.truck.Origin.Y / 2f)), 0f), new Vector3(Vector2.Add(this.truck.Position, this.truck.Origin), 0f));
			this.textureColourData = TextureUtils.TextureTo2DArray(this.truck.Texture);
			setMatrix();
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

		public void shiftDown(float elapsed) {
			float change = (SHIFT_PER_SECOND * elapsed);
			this.truck.Position = new Vector2(this.truck.Position.X, this.truck.Position.Y + change);
			this.leftBarrel.Position = new Vector2(this.leftBarrel.Position.X, this.leftBarrel.Position.Y + change);
			this.rightBarrel.Position = new Vector2(this.rightBarrel.Position.X, this.rightBarrel.Position.Y + change);
			this.BBox = new BoundingBox(new Vector3(Vector2.Subtract(this.truck.Position, new Vector2(this.truck.Origin.X,
				this.truck.Origin.Y / 2f)), 0f), new Vector3(Vector2.Add(this.truck.Position, this.truck.Origin), 0f));
			setMatrix();
		}

		public void fire() {
			this.nukeDelegate.Invoke(
				Vector2.Subtract(this.activeBarrel.Position, this.activeBarrel.Origin), this.activeBarrel.Rotation, this.power / 7f);
		}

		public virtual void update(float elapsed) {
			if (this.direction == Direction.Right) {
				this.angle = MathHelper.Clamp(this.angle, MAX_ANGLE, TRUCK_CAB_CLEARANCE);
			} else if (this.direction == Direction.Left) {
				this.angle = MathHelper.Clamp(this.angle, -TRUCK_CAB_CLEARANCE, -MAX_ANGLE);
			}
			
			this.activeBarrel.Rotation = MathHelper.ToRadians(angle);
		}

		public virtual  void render(SpriteBatch spriteBatch) {
			this.activeBarrel.render(spriteBatch);
			this.truck.render(spriteBatch);
		}
		#endregion Support methods
	}
}
