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
		public Player(ContentManager content, Vector2 position, NukeDelegate nukeDelegate, NextTurnDelegate turnDelegate, int ownerID)
			: base(content, position, nukeDelegate, turnDelegate, ownerID) {
			Text2DParams textParms = new Text2DParams();
			textParms.Font = LoadingUtils.loadSpriteFont(content, "SpriteFont1");
			textParms.Position = new Vector2(10f);
			textParms.LightColour = Color.Black;
			this.powerText = new Text2D(textParms);
		}
		#endregion Constructor

		#region Support methods
		public override void update(float elapsed) {
			if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Active
				&& StateManager.getInstance().WhosTurn == StateManager.Player.One) {
				// handle input
				if (InputManager.getInstance().wasKeyPressed(Keys.Space)) {
					base.fire();
				}

				// Adjust barel angle
				if (InputManager.getInstance().isKeyDown(Keys.Left)) {
					base.angle -= 1f;
				} else if (InputManager.getInstance().isKeyDown(Keys.Right)) {
					base.angle += 1f;
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
						base.truck.SpriteEffect = SpriteEffects.FlipHorizontally;
						base.activeBarrel = base.leftBarrel;
						float preAngle = base.angle;
						base.angle = (base.angle - base.angle) - preAngle;
					}
				} else if (InputManager.getInstance().wasKeyPressed(Keys.D)) {
					if (base.direction != Direction.Right) {
						base.direction = Direction.Right;
						base.truck.SpriteEffect = SpriteEffects.None;
						base.activeBarrel = base.rightBarrel;
						float preAngle = base.angle;
						base.angle = (base.angle - base.angle) - preAngle;
					}
				}
				this.powerText.WrittenText = "Power: " + base.power + "%";
			}
			base.update(elapsed);
		}

		public override void render(SpriteBatch spriteBatch) {
			base.render(spriteBatch);
			this.powerText.render(spriteBatch);
		}
		#endregion Support methods
	}
}
