using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NukeLaunch.Managers {
	public class StateManager {
		//singleton instance variable
		private static StateManager instance = new StateManager();

		public enum GameState {
			Loading,
			Waiting,
			Active,
			GameOver
		}

		public enum Player {
			One,
			Two,
			Three,
			Four
		}

		#region Class properties
		public GameState CurrentGameState { get; set; }
		public Player WhosTurn { get; set; }
		#endregion Class properties

		public static StateManager getInstance() {
			return instance;
		}

		#region Constructor
		public StateManager() {
			this.CurrentGameState = GameState.Loading;
			this.WhosTurn = Player.One;
			//this.CurrentGameState = GameState.Active;
		}
		#endregion Constructor
	}
}
