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
using GWNorthEngine.Model.Effects;
using GWNorthEngine.Model.Effects.Params;
using GWNorthEngine.Utils;
namespace NukeLaunch.Models {
	public class SmokeParticleEmitter : BaseParticle2DEmitter {
		#region Class variables
		private const float TIME_TO_LIVE = 750f;
		public const float SPAWN_DELAY = 100f;
		#endregion Class variables

		#region Constructor
		public SmokeParticleEmitter(ContentManager content, BaseParticle2DEmitterParams parms)
			: base(parms) {
			BaseParticle2DParams particleParms = new BaseParticle2DParams();
			particleParms.Origin = new Vector2(16f);
			particleParms.Texture = parms.ParticleTexture;
			particleParms.TimeToLive = TIME_TO_LIVE;
			base.particleParams = particleParms;
		}
		#endregion Constructor

		#region Support methods
		public void createParticle(Vector2 position) {
			base.particleParams.Position = position;
			SmokeParticle particle = new SmokeParticle(base.particleParams);
			ScaleOverTimeEffectParams effectParms = new ScaleOverTimeEffectParams {
				Reference = particle,
				ScaleBy = new Vector2(1f)
			};
			particle.addEffect(new ScaleOverTimeEffect(effectParms));

			FadeEffectParams fadeEffectParms = new FadeEffectParams {
				Reference = particle,
				State = FadeEffect.FadeState.Out,
				TotalTransitionTime = TIME_TO_LIVE,
				OriginalColour = Color.White
			};
			particle.addEffect(new FadeEffect(fadeEffectParms));

			base.particles.Add(particle);
			base.createParticle();
		}

		public void update(float elapsed, Vector2 rocketPosition) {
			base.update(elapsed);
			if (this.elapsedSpawnTime >= base.spawnDelay) {
				createParticle(rocketPosition);
			}
		}
		#endregion Support methods
	}
}
