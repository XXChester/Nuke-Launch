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
			this.textureColourData = TextureUtils.TextureTo2DArray(this.terrain.Texture);
			this.Matrix = Matrix.Identity;
			Matrix test = MatrixUtils.getMatrix(this.terrain);
		}
		#endregion Constructor

		#region Support methods
		public void destroy(Vector2 position, float radius) {
			int y = (int)position.Y;
			int x = (int)position.X;
			/*if ((y < this.textureColourData.GetUpperBound(0) - 1) && (x < this.textureColourData.GetUpperBound(1) - 1)) {
				Color preChange = this.textureColourData[y, x];
				this.textureColourData[y, x] = Color.Transparent;

				/*for (int y = 0; y < this.textureColourData.GetUpperBound(0) - 1; y++) {
					for (int x = 0; x < this.textureColourData.GetUpperBound(1) - 1; x++) {

					}
				}*/
				Color[] textureColour = new Color[this.terrain.Texture.Height * this.terrain.Texture.Width];
				for (int cy = 0; cy < this.textureColourData.GetUpperBound(0) - 1; cy++) {
					for (int cx = 0; cx < this.textureColourData.GetUpperBound(1) - 1; cx++) {
						int TEMP = cy * this.textureColourData.GetUpperBound(1) - 1 + cx;
						textureColour[cy *  this.textureColourData.GetUpperBound(1)-1 + cx] = this.textureColourData[cy, cx];
					}
				}
				this.terrain.Texture.SetData<Color>(textureColour);
				this.textureColourData = TextureUtils.TextureTo2DArray(this.terrain.Texture);
			}*/
		}

		public void render(SpriteBatch spriteBatch) {
			this.terrain.render(spriteBatch);
		}
		#endregion Support methods
	}
}
