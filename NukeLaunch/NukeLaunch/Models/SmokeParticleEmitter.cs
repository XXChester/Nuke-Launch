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
namespace NukeLaunch.Models {
	public class SmokeParticleEmitter : BaseParticle2DEmitter {
		#region Class variables

		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public SmokeParticleEmitter(BaseParticle2DEmitterParams parms)
			: base(parms) {

		}
		#endregion Constructor

		#region Support methods
		public override void createParticle() {
			
			base.createParticle();
		}

		public override void update(float elapsed) {
			// create particle logic
			base.update(elapsed);
		}
		#endregion Support methods
	}
}
