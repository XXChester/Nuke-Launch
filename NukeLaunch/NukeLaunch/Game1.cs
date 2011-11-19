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
using GWNorthEngine.Audio;
using GWNorthEngine.Audio.Params;

using NukeLaunch.Managers;
using NukeLaunch.Models;
using NukeLaunch.Logic;
namespace NukeLaunch {
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : BaseRenderer {
		private struct Tracker : IComparable<Tracker> {
			public int ID;
			public float distance;

			public Tracker(int ID, float distance) {
				this.ID = ID;
				this.distance = distance;
			}

			public int CompareTo(Tracker compareTo) {
				int result = 0;
				if (this.distance < compareTo.distance) {
					result = -1;
				} else if (this.distance > compareTo.distance) {
					result = 1;
				}
				return result;
			}
		}

		private Terrain terrain;
		private Launcher player;
		private Launcher[] enemies;
		private Nuke nuke;
		private ExplosionParticleEmitter explosionEmitter;
		private SFXEngine sfxEngine;
		private SoundEffect explosionSFX;
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
			StateManager.getInstance().WhosTurn = StateManager.Player.One;
			SFXEngineParams sfxParms = new SFXEngineParams();
			sfxParms.Muted = false;
			this.sfxEngine = new SFXEngine(sfxParms);
			this.terrain = new Terrain(Content);
			this.enemies = new Enemy[3];
			this.nuke = new Nuke(Content, this.sfxEngine);
			
			NukeDelegate nukeDelegate = null;
			NextTurnDelegate turnDelegate = null;
			ClosestTargetDelegate targetDelegate = null;
			loadDeletagtes(out nukeDelegate, out turnDelegate, out targetDelegate);

			PositionGenerator.getInstance().reset();
			this.player = new Player(Content, this.sfxEngine, PositionGenerator.getInstance().generate(), nukeDelegate, turnDelegate, 
				0);
			for (int i = 0; i < this.enemies.Length; i++) {
				this.enemies[i] = new Enemy(Content, this.sfxEngine, PositionGenerator.getInstance().generate(), nukeDelegate, 
					turnDelegate, i + 1, targetDelegate);
			}

			// now that all parties are setup, find initial targets
			foreach (Enemy enemy in this.enemies) {
				enemy.findTarget();
			}
			this.explosionEmitter = new ExplosionParticleEmitter(Content, new BaseParticle2DEmitterParams());

			// sfx
			this.explosionSFX = LoadingUtils.loadSoundEffect(Content, "Explosion");
		}

		private void loadDeletagtes(out NukeDelegate nukeDelegate, out NextTurnDelegate turnDelegate, out 
			ClosestTargetDelegate closestTargetDelegate) {
			nukeDelegate = delegate(Vector2 position, Vector2 origin, float direction, float power, int ownerID) {
				this.nuke.reset(position, origin, direction, power, ownerID);
			};


			turnDelegate = delegate(int firedID) {
				bool foundNextTurn = false;
				int nextID = firedID;
				do {
					nextID = (nextID + 1) % sizeof(StateManager.Player);
					// are we a player or enemy
					if (nextID == 0) {
						foundNextTurn = true;
					} else {
						if (this.enemies[nextID - 1] != null) {// we -1 becuase the player is not part of this array
							foundNextTurn = true;
						}
					}
				} while (!foundNextTurn);
				if (StateManager.getInstance().CurrentGameState != StateManager.GameState.GameOver) {
					StateManager.getInstance().WhosTurn = EnumUtils.numberToEnum<StateManager.Player>(nextID);
				}
			};


			closestTargetDelegate = delegate(Vector2 myPosition, int ID) {
				List<Tracker> distances = new List<Tracker>();
				Vector2 max;
				Vector2 min;
				Vector2 distance;

				// get players distance
				if (this.player != null) {
					max = new Vector2(MathHelper.Max(myPosition.X, this.player.Position.X),
						MathHelper.Max(myPosition.Y, this.player.Position.Y));
					min = new Vector2(MathHelper.Min(myPosition.X, this.player.Position.X),
						MathHelper.Min(myPosition.Y, this.player.Position.Y));

					distance = Vector2.Subtract(max, min);
					distances.Add(new Tracker(this.player.ID, distance.X + distance.Y));
				}

				foreach (Launcher launcher in this.enemies) {
					if (launcher != null && ID != launcher.ID) {
						max = new Vector2(MathHelper.Max(myPosition.X, launcher.Position.X),
							MathHelper.Max(myPosition.Y, launcher.Position.Y));
						min = new Vector2(MathHelper.Min(myPosition.X, launcher.Position.X),
							MathHelper.Min(myPosition.Y, launcher.Position.Y));

						distance = Vector2.Subtract(max, min);
						distances.Add(new Tracker(launcher.ID, distance.X + distance.Y));
					}
				}

				distances.Sort();
				Vector2 target;
				if (distances[0].ID == 0) {
					target = this.player.Position;
				} else {
					target = this.enemies[distances[0].ID - 1].Position;
				}
				return target;
			};
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent() {
			base.UnloadContent();
		}

		private void explode(Vector2 collisionPoint) {
			this.sfxEngine.playSoundEffect(this.explosionSFX);
			this.explosionEmitter.reset(collisionPoint);
			this.nuke.State = SpriteState.InActive;
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

			bool playerLanded = true;
			if (!CollisionHelper.collision(this.player, this.terrain)) {
				this.player.shiftDown(elapsed);
				playerLanded = false;
			}

			bool enemiesLanded = true;
			foreach (Launcher launcher in this.enemies) {
				if (launcher != null && !CollisionHelper.collision(launcher, this.terrain)) {
					launcher.shiftDown(elapsed);
					enemiesLanded = false;
				}
			}

			if (playerLanded && enemiesLanded) {
				StateManager.getInstance().CurrentGameState = StateManager.GameState.Active;
			}
			if (StateManager.getInstance().CurrentGameState == StateManager.GameState.Active) {
				if (this.nuke.State == SpriteState.Active) {
					Vector2 collisionPoint;
					// check for collision with enemies
					for (int i = 0; i < this.enemies.Length; i++) {
						if (this.enemies[i] != null && this.enemies[i].ID != this.nuke.OwnerID) {
							if (CollisionHelper.collision(this.nuke, this.enemies[i], out collisionPoint)) {
								this.enemies[i] = null;
								explode(collisionPoint);
								break;
							}
						}
					}

					// Your own nuke cannot kill you
					if (this.nuke.OwnerID != this.player.ID && CollisionHelper.collision(this.nuke, this.player, out collisionPoint)) {
						//this.player = null;

						explode(collisionPoint);
						StateManager.getInstance().CurrentGameState = StateManager.GameState.GameOver;
					} else if (CollisionHelper.collision(this.nuke, this.terrain, out collisionPoint)) {
						explode(collisionPoint);
						this.terrain.destroy(collisionPoint);
					}

					// check if it went out of bounds
					Vector2 nukePos = this.nuke.Position;
					if (nukePos.X > base.graphics.PreferredBackBufferWidth * (1.5f) ||
						nukePos.X < -.5f * base.graphics.PreferredBackBufferWidth ||
						nukePos.Y > base.graphics.PreferredBackBufferHeight * (1.5)) {
							explode(nukePos);
					}
				}
			} else if (StateManager.getInstance().CurrentGameState == StateManager.GameState.GameOver) {
				if (InputManager.getInstance().wasButtonPressed(MouseButton.Left)) {
					reset();
				}
			}
			this.sfxEngine.update();

#if DEBUG
			if (InputManager.getInstance().isButtonDown(MouseButton.Left)) {
				this.terrain.destroy(InputManager.getInstance().MousePosition);
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
