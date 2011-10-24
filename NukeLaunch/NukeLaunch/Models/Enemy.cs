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
using NukeLaunch.Logic;
namespace NukeLaunch.Models {
	public class Enemy : Launcher {
		#region Class variables

		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public Enemy(ContentManager content, Vector2 position, NukeDelegate nukeDelegate)
			: base(content, position, nukeDelegate) {

		}
		#endregion Constructor

		#region Support methods
		public override void update(float elapsed) {
			//AI
			base.update(elapsed);
		}
		#endregion Support methods
	}
}
