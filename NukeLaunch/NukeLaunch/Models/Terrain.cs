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
		private List<Vector2> distancesFromMiddle;
		#endregion Class variables

		#region Class propeties
		public Color[,] TextureColourData { get { return this.textureColourData; } }
		public Matrix Matrix { get; set; }
		#endregion Class properties

		#region Constructor
		public Terrain(ContentManager content) {
			StaticDrawable2DParams parms = new StaticDrawable2DParams();
			parms.Texture = LoadingUtils.load<Texture2D>(content, "DefaultTerrain");
			this.terrain = new StaticDrawable2D(parms);
			this.textureColourData = TextureUtils.getColourData2D(this.terrain.Texture);
			this.Matrix = Matrix.Identity;

			this.distancesFromMiddle = new List<Vector2>();
			Texture2D crater = LoadingUtils.load<Texture2D>(content, "Crater");
			Color[,] colours = TextureUtils.getColourData2D(crater);
			for (int y = 0; y < crater.Height; y++) {
				for (int x = 0; x < crater.Width; x++) {
					if (colours[x, y] == Color.White) {
						this.distancesFromMiddle.Add(new Vector2(x - 32f, y - 32f));// half of the textures size so we render at the origin
					}
				}
			}
		}
		#endregion Constructor

		#region Support methods
		public void destroy(Vector2 position) {
			int tcdW = this.textureColourData.GetUpperBound(0) + 1;
			int tcdH = this.textureColourData.GetUpperBound(1) + 1;
			Vector2 pixelPosition;
			for (int i = 0; i < this.distancesFromMiddle.Count; i++) {
				pixelPosition = Vector2.Add(position, this.distancesFromMiddle[i]);
				if (pixelPosition.X >= 0 && pixelPosition.X < tcdW && pixelPosition.Y >= 0 && pixelPosition.Y < tcdH) {
					this.textureColourData[(int)pixelPosition.X, (int)pixelPosition.Y] = Color.Transparent;
				}
			}
			// flatten the array
			Color[] colours = new Color[tcdH * tcdW];
			for (int x = 0; x < tcdW; x++) {
				for (int y = 0; y < tcdH; y++) {
					colours[x + y * tcdW] = this.textureColourData[x, y];
				}
			}
			this.terrain.Texture.SetData<Color>(colours);
		}

		public void render(SpriteBatch spriteBatch) {
			this.terrain.render(spriteBatch);
		}
		#endregion Support methods
	}
}
