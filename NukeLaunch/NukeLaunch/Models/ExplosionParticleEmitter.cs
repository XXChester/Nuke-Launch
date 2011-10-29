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
	public class ExplosionParticleEmitter : BaseParticle2DEmitter {
		#region Class variables
		private Vector2 position;
		private const float TIME_TO_LIVE = 750f;
		private const int MIN_PARTICLE_SPEED = 1;
		private const int MAX_PARTICLE_SPEED = 3;
		private const int MAX_ROTATION_SPEED = 1;
		private const int MAX_RANGE_FROM_EMITTER = 16;
		private readonly Texture2D[] PARTICLE_TEXTURES;
		#endregion Class variables

		#region Constructor
		public ExplosionParticleEmitter(ContentManager content, BaseParticle2DEmitterParams parms)
				: base(parms) {
			this.PARTICLE_TEXTURES = new Texture2D[5];
			for (int i = 1; i <= this.PARTICLE_TEXTURES.Length; i++) {
				this.PARTICLE_TEXTURES[i - 1] = LoadingUtils.loadTexture2D(content, "ExplosionParticle" + i);
			}
			BaseParticle2DParams particleParams = new BaseParticle2DParams();
			particleParams.Scale = new Vector2(.5f);
			particleParams.Origin = new Vector2(16f);
			particleParams.LightColour = Color.White;
			particleParams.TimeToLive = TIME_TO_LIVE;
			base.particleParams = particleParams;
		}
		#endregion Constructor

		#region Support methods
		public override void createParticle() {
			int positionX = base.RANDOM.Next(MAX_RANGE_FROM_EMITTER);
			int positionY = base.RANDOM.Next(MAX_RANGE_FROM_EMITTER);
			int positionDirectionX = base.RANDOM.Next(2);
			int positionDirectionY = base.RANDOM.Next(2);
			float directionX = base.RANDOM.Next(MIN_PARTICLE_SPEED, MAX_PARTICLE_SPEED);
			float directionY = base.RANDOM.Next(MIN_PARTICLE_SPEED, MAX_PARTICLE_SPEED);
			float x, y;
			if (positionDirectionX == 0) {
				x = this.position.X + positionX;
			} else {
				x = this.position.X - positionX;
				directionX *= -1;
			}

			if (positionDirectionY == 0) {
				y = this.position.Y + positionY;
			} else {
				y = this.position.Y - positionY;
				directionY *= -1;
			}
			
			// setup the particle rotation
			float rotationSpeed = (float)base.RANDOM.Next(MAX_ROTATION_SPEED);
			rotationSpeed /= 1000f;//per second
			int rotationDirection = base.RANDOM.Next(2);
			if (rotationDirection == 0) {
				rotationSpeed *= -1;
			}
			

			base.particleParams.Position = new Vector2(x, y);
			base.particleParams.Direction = new Vector2(directionX, directionY);
			base.particleParams.Texture = this.PARTICLE_TEXTURES[base.RANDOM.Next(this.PARTICLE_TEXTURES.Length)];
			base.particles.Add(new ExplosionParticle(base.particleParams, rotationSpeed));
			base.createParticle();
		}

		public void reset(Vector2 position) {
			this.position = position;
			for (int i = 0; i < 30; i++) {
				createParticle();
			}
		}
		#endregion Support methods
	}
}
