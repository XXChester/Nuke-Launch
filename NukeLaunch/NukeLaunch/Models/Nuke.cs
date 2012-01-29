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
namespace NukeLaunch.Models {
	public class Nuke {
		#region Class variables
		private SmokeParticleEmitter emitter;
		private Animated2DSprite bomb;
		private Vector2 direction;
		private List<Color[,]> textureColourDatas;
		private SFXEngine sfxEngine;
		private readonly Vector2 GRAVITY = new Vector2(0f, 10f / 1000f);
		#endregion Class variables

		#region Class propeties
		public Color[,] TextureColourData { get { return this.textureColourDatas[this.bomb.AnimationManager.CurrentFrame]; } }
		public Vector2 Position { get { return this.bomb.Position; } }
		public Matrix Matrix { get; set; }
		public SpriteState State { get; set; }
		public int OwnerID { get; set; }
		#endregion Class properties

		#region Constructor
		public Nuke(ContentManager content, SFXEngine sfxEngine) {
			int frames = 6;
			BaseAnimationManagerParams animationParms = new BaseAnimationManagerParams();
			animationParms.AnimationState = AnimationManager.AnimationState.PlayForward;
			animationParms.FrameRate = 100f;
			animationParms.TotalFrameCount = frames;
			BaseAnimated2DSpriteParams spriteParms = new Animated2DSpriteLoadSingleRowBasedOnTexture();
			spriteParms.Position = new Vector2(-200f);
			spriteParms.Rotation = 0f;
			spriteParms.Origin = new Vector2(32f);
			spriteParms.Scale = new Vector2(.75f);
			spriteParms.Texture2D = LoadingUtils.load<Texture2D>(content, "NukeSprite");
			spriteParms.AnimationParams = animationParms;

			this.bomb = new Animated2DSprite(spriteParms);
			this.State = SpriteState.InActive;
			this.textureColourDatas = new List<Color[,]>();
			Rectangle frame;
			for (int i = 0; i < frames; i++) {
				frame = this.bomb.Frames[i];
				this.textureColourDatas.Add(
					TextureUtils.getColourData2D(this.bomb.Texture, startX: frame.X, width: frame.X + frame.Width));
			}

			// Smoke emitter
			BaseParticle2DEmitterParams particleEmitterParms = new BaseParticle2DEmitterParams();
			particleEmitterParms.ParticleTexture = LoadingUtils.load<Texture2D>(content, "Smoke");
			particleEmitterParms.SpawnDelay = SmokeParticleEmitter.SPAWN_DELAY;
			this.emitter = new SmokeParticleEmitter(content, particleEmitterParms);

			// sfx
			this.sfxEngine = sfxEngine;
		}
		#endregion Constructor

		#region Support methods
		public void reset(Vector2 position, Vector2 origin, float direction, float power, int ownerID) {
			Vector2 up = new Vector2(0f, -1f);
			Matrix rotationMatrix = Matrix.CreateRotationZ(direction);
			this.bomb.Position = position;
			this.bomb.Rotation = direction;
			this.bomb.Origin = origin;
			this.direction = Vector2.Transform(up, rotationMatrix);
			this.direction *= power;
			this.State = SpriteState.Active;
			this.OwnerID = ownerID;
		}

		public void update(float elapsed) {
			if (this.State == SpriteState.Active) {
				this.bomb.update(elapsed);
				this.direction += GRAVITY * elapsed;
				this.bomb.Position += this.direction;
				this.bomb.Rotation = (float)Math.Atan2(this.direction.X, -this.direction.Y);
				this.emitter.update(elapsed, this.bomb.Position);

				Matrix origin = Matrix.CreateTranslation(new Vector3(-this.bomb.Origin, 0f));
				Matrix scale = Matrix.CreateScale(new Vector3(this.bomb.Scale, 0f));
				Matrix rotation = Matrix.CreateRotationZ(this.bomb.Rotation);
				Matrix position = Matrix.CreateTranslation(new Vector3(this.bomb.Position, 0f));

				this.Matrix = origin * rotation * scale * position;
			} else {
				this.bomb.reset();
				this.emitter.update(elapsed);
			}
		}

		public void render(SpriteBatch spriteBatch) {
			this.emitter.render(spriteBatch);
			if (this.State == SpriteState.Active) {
				this.bomb.render(spriteBatch);
			}
		}
		#endregion Support methods
	}
}
