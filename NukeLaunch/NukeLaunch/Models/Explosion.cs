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
	public class Explosion {
		#region Class variables
		private Animated2DSprite sprite;
		private SpriteState state;
		#endregion Class variables

		#region Class propeties
		
		#endregion Class properties

		#region Constructor
		public Explosion(ContentManager content) {
			BaseAnimationManagerParams animationParms = new BaseAnimationManagerParams();
			animationParms.AnimationState = AnimationManager.AnimationState.Paused;
			animationParms.FrameRate = 25f;
			//animationParms.TotalFrameCount = 16;
			//animationParms.TotalFrameCount = 11;
			animationParms.TotalFrameCount = 6;
			Animated2DSpriteParams spriteParms = new Animated2DSpriteParams();
			spriteParms.Position = new Vector2(-1000f);
			/*spriteParms.Texture2D = LoadingUtils.loadTexture2D(content, "Explosion1");
			spriteParms.LoadingType = Animated2DSprite.LoadingType.CustomizedSheetDefineFrames;*/
			spriteParms.Origin = new Vector2(32f);
			spriteParms.Texture2D = LoadingUtils.loadTexture2D(content, "Explosion2");
			spriteParms.LoadingType = Animated2DSprite.LoadingType.WholeSheetReadFramesFromFile;
			spriteParms.AnimationParams = animationParms;
			this.sprite = new Animated2DSprite(spriteParms);
			this.state = SpriteState.InActive;

			/*List<Rectangle> frames = new List<Rectangle>();
			int size = 64;
			for (int y = 0; y < 4; y++) {
				for (int x = 0; x < 4; x++) {
					frames.Add(new Rectangle(x * size, y * size, size, size));
				}
			}
			this.sprite.Frames = frames.ToArray();*/
		}
		#endregion Constructor

		#region Support methods
		public void reset(Vector2 position) {
			this.sprite.Position = position;
			this.sprite.AnimationManager.State = AnimationManager.AnimationState.PlayForwardOnce;
			this.state = SpriteState.Active;
		}

		public void update(float elapsed) {
			if (this.state == SpriteState.Active) {
				this.sprite.update(elapsed);
				// once the sprite is finished its animation inactivate it
				if (this.sprite.AnimationManager.State == AnimationManager.AnimationState.Paused) {
					this.state = SpriteState.InActive;
					this.sprite.reset();
				}
			}
		}

		public void render(SpriteBatch spriteBatch) {
			if (this.state == SpriteState.Active) {
				this.sprite.render(spriteBatch);
			}
		}
		#endregion Support methods
	}
}
