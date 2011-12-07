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
using GWNorthEngine.Audio;

using NukeLaunch.Logic;
using NukeLaunch.Managers;

namespace NukeLaunch.Models {
	public class Enemy : Launcher {
		#region Class variables
		private Vector2 target;
		private ClosestTargetDelegate closestTarget;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public Enemy(ContentManager content, SFXEngine sfxEngine, Vector2 position, NukeDelegate nukeDelegate,
			int ownerID, ClosestTargetDelegate closestTarget)
			: base(content, sfxEngine, position, nukeDelegate, ownerID) {
			this.closestTarget = closestTarget;
		}
		#endregion Constructor

		#region Support methods
		public void findTarget() {
			this.target = this.closestTarget.Invoke(base.truck.Position, base.ID);
			Vector2 direction = Vector2.Subtract(base.truck.Position, target);
			if (direction.X < 0) {
				if (base.direction != Direction.Right) {
					base.direction = Direction.Right;
					base.updateDirection();
				}
			} else {
				if (base.direction != Direction.Left) {
					base.direction = Direction.Left;
					base.updateDirection();
				}
			}
		}

		int TEMP_COUNT = 0;
		public override void update(float elapsed) {
			if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Active && 
				((int)StateManager.getInstance().WhosTurn) == this.ID) {

				// figure out a good angle/power to fire at
				Vector2 max = new Vector2(base.truck.Position.X, target.X);
				Vector2 min = new Vector2(base.truck.Position.Y, target.Y);
				Vector2 distance = Vector2.Subtract(max, min);
				if (distance.X > 1000f) {
					if (base.direction == Direction.Left) {
						
					} else if (base.direction == Direction.Right) {

					}
				} else if (distance.X > 800f) {
					if (base.direction == Direction.Left) {

					} else if (base.direction == Direction.Right) {

					}
				} else if (distance.X > 600f) {
					if (base.direction == Direction.Left) {

					} else if (base.direction == Direction.Right) {

					}
				} else if (distance.X > 400f) {
					if (base.direction == Direction.Left) {

					} else if (base.direction == Direction.Right) {

					}
				} else if (distance.X > 200f) {
					if (base.direction == Direction.Left) {

					} else if (base.direction == Direction.Right) {

					}
				} else {
					if (base.direction == Direction.Left) {

					} else if (base.direction == Direction.Right) {

					}
				}

				//fire
				if (TEMP_COUNT == 250) {
					base.fire();
					TEMP_COUNT = 0;
				} else {
					TEMP_COUNT++;
				}
			}

			base.update(elapsed);
		}
		#endregion Support methods
	}
}
