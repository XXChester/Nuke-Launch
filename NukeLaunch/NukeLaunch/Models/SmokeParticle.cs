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
namespace NukeLaunch.Models {
	public class SmokeParticle : BaseParticle2D {
		#region Class variables
		private readonly Vector2 SCALE_BY = new Vector2(.0005f);
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public SmokeParticle(BaseParticle2DParams parms)
			:base(parms) {

		}
		#endregion Constructor

		#region Support methods
		public override void update(float elapsed) {
			base.update(elapsed);
			base.fadeOutAsLifeProgresses();
			base.scaleAsLifeProgresses(SCALE_BY * elapsed);
		}
		#endregion Support methods
	}
}
