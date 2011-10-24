using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
namespace NukeLaunch.Logic {
	public class PositionGenerator {
		private static PositionGenerator instance = new PositionGenerator();
		#region Class variables
		private Random random;
		private List<int> usedPositions;
		private const int MAX_WIDTH = 1000;
		private const float Y = 100f;
		private const float MIN_DISTANCE_REQUIRED = 100f;
		#endregion Class variables

		#region Class propeties

		#endregion Class properties

		#region Constructor
		public PositionGenerator() {
			reset();
		}
		#endregion Constructor

		#region Support methods
		public static PositionGenerator getInstance() {
			return instance;
		}

		public void reset() {
			this.random = new Random();
			this.usedPositions = new List<int>();
		}

		public Vector2 generate() {
			Vector2 position = new Vector2();
			int amount = 10;
			int x;
			bool accepted = false;
			do {
				x = 0;
				for (int i = 0; i < amount; i++) {
					x += this.random.Next(MAX_WIDTH);
				}
				// get the average
				x /= amount;

				accepted = true;
				// Verify it is over the min distance between players
				int space;
				for (int i = 0; i < this.usedPositions.Count; i++) {
					space = (int)(MathHelper.Max(this.usedPositions[i], x) - MathHelper.Min(this.usedPositions[i], x));
					if (space < MIN_DISTANCE_REQUIRED) {
						accepted = false;
						break;
					}
				}
			} while (!accepted);
			position = new Vector2(x, Y);
			usedPositions.Add(x);
#if (DEBUG)
			Console.WriteLine(position);
#endif
			return position;
		}
		#endregion Support methods

		#region Destructor

		#endregion Destructor
	}
}
