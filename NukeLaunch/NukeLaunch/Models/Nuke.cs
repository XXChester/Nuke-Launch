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
	public class Nuke {
		#region Class variables
		private SmokeParticleEmitter emitter;
		private Animated2DSprite bomb;
		private Vector2 direction;
		private SpriteState state;
		private readonly Vector2 GRAVITY = new Vector2(0f, 10f / 1000f);
		private List<Color[,]> textureColourDatas;
		#endregion Class variables

		#region Class propeties
		public SpriteState State {
			get { return this.state; }
			set {
				this.state = value;
				if (this.state == SpriteState.InActive) {
					this.bomb.Position = new Vector2(-200f);
				}
			}
		}
		public Color[,] TextureColourData { get { return this.textureColourDatas[this.bomb.AnimationManager.CurrentFrame]; } }
		public Vector2 Position { get { return this.bomb.Position; } }
		public Matrix Matrix { get; set; }
		public BoundingBox BBox { get; set; }
		#endregion Class properties

		#region Constructor
		public Nuke(ContentManager content) {
			int frames = 6;
			BaseAnimationManagerParams animationParms = new BaseAnimationManagerParams();
			animationParms.AnimationState = AnimationManager.AnimationState.PlayForward;
			animationParms.FrameRate = 100f;
			animationParms.TotalFrameCount = frames;
			Animated2DSpriteParams spriteParms = new Animated2DSpriteParams();
			spriteParms.Position = new Vector2(-200f);
			spriteParms.Rotation = 0f;
			spriteParms.Origin = new Vector2(32f);
			spriteParms.Scale = new Vector2(.75f);
			spriteParms.Texture2D = LoadingUtils.loadTexture2D(content, "NukeSprite");
			spriteParms.LoadingType = Animated2DSprite.LoadingType.WholeSheetReadFramesFromFile;
			spriteParms.AnimationParams = animationParms;

			this.bomb = new Animated2DSprite(spriteParms);
			this.state = SpriteState.InActive;
			this.textureColourDatas = new List<Color[,]>();
			Rectangle frame;
			for (int i = 0; i < frames; i++) {
				frame = this.bomb.Frames[i];
				this.textureColourDatas.Add(
					TextureUtils.textureTo2DArray(this.bomb.Texture, frame.X, frame.Y, frame.X + frame.Width, frame.Height));
			}
		}
		#endregion Constructor

		#region Support methods
		public void reset(Vector2 position, float direction, float power) {
			Vector2 up = new Vector2(0f, -1f);
			Matrix rotationMatrix = Matrix.CreateRotationZ(direction);
			this.bomb.Position = position;
			this.bomb.Rotation = direction;
			this.direction = Vector2.Transform(up, rotationMatrix);
			this.direction *= power;
			this.state = SpriteState.Active;
			this.BBox = new BoundingBox(new Vector3(Vector2.Subtract(position, this.bomb.Origin), 0f), new Vector3(
				Vector2.Add(position, this.bomb.Origin), 0f));
		}

		public void update(float elapsed) {
			if (this.state == SpriteState.Active) {
				this.bomb.update(elapsed);
				this.direction += GRAVITY * elapsed;
				this.bomb.Position += this.direction;
				this.bomb.Rotation = (float)Math.Atan2(this.direction.X, -this.direction.Y);
				this.BBox = new BoundingBox(new Vector3(Vector2.Subtract(this.bomb.Position, this.bomb.Origin), 0f), new Vector3(
					Vector2.Add(this.bomb.Position, this.bomb.Origin), 0f));

				Matrix origin = Matrix.CreateTranslation(new Vector3(-this.bomb.Origin, 0f));
				Matrix scale = Matrix.CreateScale(new Vector3(this.bomb.Scale, 0f));
				Matrix rotation = Matrix.CreateRotationZ(this.bomb.Rotation);
				Matrix position = Matrix.CreateTranslation(new Vector3(this.bomb.Position, 0f));

				this.Matrix = origin * rotation * scale * position;
			} else {
				this.bomb.reset();
				//Destroy the bounding box
				this.BBox = new BoundingBox(new Vector3(-1000f), new Vector3(-1000f));
			}
		}

		public void render(SpriteBatch spriteBatch) {
			if (this.State == SpriteState.Active) {
				this.bomb.render(spriteBatch);
			}
		}
		#endregion Support methods
	}
}
