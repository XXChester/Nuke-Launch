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
using GWNorthEngine.Input;
using GWNorthEngine.Utils;
using GWNorthEngine.Audio;

using NukeLaunch.Logic;
using NukeLaunch.Managers;
namespace NukeLaunch.Models {
	public class Player : Launcher{
		#region Class variables
		private Text2D powerText;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public Player(ContentManager content, SFXEngine sfxEngine, Vector2 position, NukeDelegate nukeDelegate, int ownerID)
			: base(content, sfxEngine, position, nukeDelegate, ownerID) {
			Text2DParams textParms = new Text2DParams();
			textParms.Font = LoadingUtils.load<SpriteFont>(content, "SpriteFont1");
			textParms.Position = new Vector2(10f);
			textParms.LightColour = Color.Black;
			this.powerText = new Text2D(textParms);
		}
		#endregion Constructor

		#region Support methods
		public override void update(float elapsed) {
			if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Active
				&& (int)StateManager.getInstance().WhosTurn == base.ID) {
				// handle input
				if (InputManager.getInstance().wasKeyPressed(Keys.Space)) {
					base.fire();
				}

				// Adjust barel angle
				if (InputManager.getInstance().isKeyDown(Keys.Left)) {
					base.rotate(RotationDirection.Down);
				} else if (InputManager.getInstance().isKeyDown(Keys.Right)) {
					base.rotate(RotationDirection.Up);
				}

				// adjust power
				if (InputManager.getInstance().isKeyDown(Keys.Up)) {
					if (base.power + POWER_INCREASE <= MAX_POWER) {
						base.power += POWER_INCREASE;
					}
				} else if (InputManager.getInstance().isKeyDown(Keys.Down)) {
					if (base.power - POWER_INCREASE >= MIN_POWER) {
						base.power -= POWER_INCREASE;
					}
				} else if (InputManager.getInstance().wasKeyPressed(Keys.A)) {
					if (base.direction != Direction.Left) {
						base.direction = Direction.Left;
						base.updateDirection();
					}
				} else if (InputManager.getInstance().wasKeyPressed(Keys.D)) {
					if (base.direction != Direction.Right) {
						base.direction = Direction.Right;
						base.updateDirection();
					}
				}
				this.powerText.WrittenText = "Power: " + base.power + "%";
			}
			base.update(elapsed);
		}

		public override void render(SpriteBatch spriteBatch) {
			base.render(spriteBatch);
			if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Active
				&& (int)StateManager.getInstance().WhosTurn == base.ID) {
				this.powerText.render(spriteBatch);
			}
		}
		#endregion Support methods
	}
}
