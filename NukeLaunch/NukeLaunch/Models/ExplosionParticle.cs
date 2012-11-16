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
	public class ExplosionParticle : BaseParticle2D {
		#region Class variables
		#endregion Class variables

		#region Constructor
		public ExplosionParticle(BaseParticle2DParams parms)
			:base(parms) {

		}
		#endregion Constructor

		#region Support methods
		public override void update(float elapsed) {
			base.update(elapsed);
			base.position += Vector2.Multiply(base.direction, elapsed);
		}
		#endregion Support methods
	}
}
