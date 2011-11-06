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
namespace NukeLaunch.Models {
	public class Terrain {
		#region Class variables
		private Color[,] textureColourData;
		private StaticDrawable2D terrain;
		private StaticDrawable2D crater;
		#endregion Class variables

		#region Class propeties
		public Color[,] TextureColourData { get { return this.textureColourData; } }
		public Matrix Matrix { get; set; }
		#endregion Class properties

		#region Constructor
		public Terrain(ContentManager content) {
			StaticDrawable2DParams parms = new StaticDrawable2DParams();
			parms.Texture = LoadingUtils.loadTexture2D(content, "DefaultTerrain");
			this.terrain = new StaticDrawable2D(parms);
			parms.Texture = LoadingUtils.loadTexture2D(content, "Crater");
			parms.Origin = new Vector2(32f);
			parms.LightColour = Color.White;
			this.crater = new StaticDrawable2D(parms);
			this.textureColourData = TextureUtils.getColourData2D(this.terrain.Texture);
			this.Matrix = Matrix.Identity;
		}
		#endregion Constructor

		#region Support methods
		public void destroy(GraphicsDevice device, Vector2 position) {
			// Render to texture the crater
			this.crater.Position = position;
			RenderTarget2D renderTarget = new RenderTarget2D(device, this.terrain.Texture.Width, this.terrain.Texture.Height);
			device.SetRenderTarget(renderTarget);
			device.Clear(Color.Transparent);

			// first pass set the collision point
			SpriteBatch spriteBatch = new SpriteBatch(device);
			//spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.Default,
			//	RasterizerState.CullNone);

			spriteBatch.Begin();
			this.terrain.render(spriteBatch);
			this.crater.render(spriteBatch);
			spriteBatch.End();

			// second pass make the collision point transparent


			this.terrain.Texture = (Texture2D)renderTarget;
			device.SetRenderTarget(null);
			this.textureColourData = TextureUtils.getColourData2D(this.terrain.Texture);


			// we need to make the white pixesl transparent
			/*for (int y = 0; y < this.terrain.Texture.Height; y++) {
				for (int x = 0; x < this.terrain.Texture.Width; x++) {
					if (this.textureColourData[x, y] == Color.Red) {
						this.textureColourData[x, y] = Color.Transparent;
					}
				}
			}*/

			using (System.IO.FileStream stream = new System.IO.FileStream("test.png", System.IO.FileMode.Create)) {
				this.terrain.Texture.SaveAsPng(stream, this.terrain.Texture.Width, this.terrain.Texture.Height);
			}

			/*if ((y < this.textureColourData.GetUpperBound(0) - 1) && (x < this.textureColourData.GetUpperBound(1) - 1)) {
				// flatten the array to 1D
				Color[] colourData = new Color[this.terrain.Texture.Width * this.terrain.Texture.Height];
				this.terrain.Texture.GetData<Color>(colourData);

				// Create the crater
				int startIndex = x + (y * this.terrain.Texture.Width);

				colourData[x + (y * this.terrain.Texture.Width)] = Color.Transparent;
				this.terrain.Texture.SetData<Color>(colourData);
			}*/
		}

		public void render(SpriteBatch spriteBatch) {
			this.terrain.render(spriteBatch);
		}
		#endregion Support methods
	}
}
