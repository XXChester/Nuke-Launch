using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GWNorthEngine.Engine;
using GWNorthEngine.Engine.Params;
using GWNorthEngine.Input;
using GWNorthEngine.Model;
using GWNorthEngine.Model.Params;
using GWNorthEngine.Utils;

using NukeLaunch.Managers;
using NukeLaunch.Models;
using NukeLaunch.Logic;
namespace NukeLaunch {
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : BaseRenderer {
		private Terrain terrain;
		private Launcher player;
		private Launcher[] enemies;
		private Nuke nuke;
		private ExplosionParticleEmitter explosionEmitter;
		private const string TITLE = "Nuke Launch";

		public Game1() {
			BaseRendererParams parms = new BaseRendererParams();
			parms.MouseVisible = false;
			parms.RunningMode = RunningMode.Release;
			parms.ScreenHeight = 768;
			parms.ScreenWidth = 1024;
			parms.WindowsText = TITLE;
#if DEBUG
			parms.MouseVisible = true;
			parms.RunningMode = RunningMode.Debug;
#endif
			base.initialize(parms);
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent() {
			reset(false);
		}

		public void reset(bool setState=true) {
			if (setState) {
				StateManager.getInstance().CurrentGameState = StateManager.GameState.Loading;
			}
			this.terrain = new Terrain(Content);
			this.enemies = new Enemy[3];
			this.nuke = new Nuke(Content);
			NukeDelegate nukeDelegate = delegate(Vector2 position, Vector2 origin, float direction, float power, int ownerID) {
				this.nuke.reset(position, origin, direction, power, ownerID);
			};
			NextTurnDelegate turnDelegate = delegate(int firedID) {
				bool foundNextTurn = false;
				int nextID = firedID;
				do {
					nextID = (nextID + 1) % sizeof(StateManager.Player);
					// are we a player or enemy
					if (nextID == 0) {
						foundNextTurn = true;
					} else {
						if (this.enemies[nextID-1] != null) {// we -1 becuase the player is not part of this array
							foundNextTurn = true;
						}
					}
				} while (!foundNextTurn);
				if (StateManager.getInstance().CurrentGameState != StateManager.GameState.GameOver) {
					StateManager.getInstance().WhosTurn = EnumUtils.numberToEnum<StateManager.Player>(nextID);
				}
			};
			PositionGenerator.getInstance().reset();
			this.player = new Player(Content, PositionGenerator.getInstance().generate(), nukeDelegate, turnDelegate, 0);
			for (int i = 0; i < this.enemies.Length; i++) {
				this.enemies[i] = new Enemy(Content, PositionGenerator.getInstance().generate(), nukeDelegate, turnDelegate, i + 1);
			}
			this.explosionEmitter = new ExplosionParticleEmitter(Content, new BaseParticle2DEmitterParams());
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent() {
			base.UnloadContent();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime) {
			// Allows the game to exit
			if (InputManager.getInstance().wasKeyPressed(Keys.Escape)) {
				this.Exit();
			}
			float elapsed = gameTime.ElapsedGameTime.Milliseconds;

			this.player.update(elapsed);
			this.nuke.update(elapsed);
			this.explosionEmitter.update(elapsed);
			foreach (Launcher launcher in this.enemies) {
				if (launcher != null) {
					launcher.update(elapsed);
				}
			}

			if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Loading) {
				bool playerLanded = true;
				if (!CollisionHelper.collision(this.player, this.terrain)) {
					this.player.shiftDown(elapsed);
					playerLanded = false;
				}

				bool enemiesLanded = true;
				foreach (Launcher launcher in this.enemies) {
					if (!CollisionHelper.collision(launcher, this.terrain)) {
						launcher.shiftDown(elapsed);
						enemiesLanded = false;
					}
				}

				if (playerLanded && enemiesLanded) {
					StateManager.getInstance().CurrentGameState = StateManager.GameState.Active;
				}
			} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Active) {
				if (this.nuke.State == SpriteState.Active) {
					// check for collision with enemies
					for (int i = 0; i < this.enemies.Length; i++) {
						// Your own nuke cannot kill you
						if (this.enemies[i] != null && this.enemies[i].ID != this.nuke.OwnerID) {
							if (CollisionHelper.collision(this.nuke, this.enemies[i])) {
								this.enemies[i] = null;
								this.explosionEmitter.reset(this.nuke.Position);
								this.nuke.State = SpriteState.InActive;
								break;
							}
						}
					}

					if (this.nuke.OwnerID != this.player.ID && CollisionHelper.collision(this.nuke, this.player)) {
						//this.player = null;
						this.explosionEmitter.reset(this.nuke.Position);
						StateManager.getInstance().CurrentGameState = StateManager.GameState.GameOver;
					} else if (CollisionHelper.collision(this.nuke, this.terrain)) {
						this.explosionEmitter.reset(this.nuke.Position);
						this.terrain.destroy(GraphicsDevice, this.nuke.Position);
						this.nuke.State = SpriteState.InActive;
					}
				}
			} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.GameOver) {
				if (InputManager.getInstance().wasButtonPressed(MouseButton.Left)) {
					reset();
				}
			}

#if DEBUG
			if (InputManager.getInstance().isButtonDown(MouseButton.Left)) {
				this.terrain.destroy(GraphicsDevice, InputManager.getInstance().MousePosition);
			}
			if (InputManager.getInstance().wasKeyPressed(Keys.R)) {
				reset();
			} else if (InputManager.getInstance().wasKeyPressed(Keys.C)) {
				Console.Clear();
			}
			this.Window.Title = TITLE + " " + FrameRate.getInstance().calculateFrameRate(gameTime);
#endif
			base.Update(gameTime);
		}



		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.White);

			base.spriteBatch.Begin();
			this.terrain.render(base.spriteBatch);
			this.player.render(base.spriteBatch);
			foreach (Launcher launcher in this.enemies) {
				if (launcher != null) {
					launcher.render(base.spriteBatch);
				}
			}
			this.nuke.render(base.spriteBatch);
			this.explosionEmitter.render(base.spriteBatch);
			base.spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
