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
		#endregion Class variables

		#region Class propeties
		public Color[,] TextureColourData { get { return this.textureColourData; } }
		public StaticDrawable2D TerrainImage { get; set; }
		public Matrix Matrix { get; set; }
		#endregion Class properties

		#region Constructor
		public Terrain(ContentManager content) {
			StaticDrawable2DParams parms = new StaticDrawable2DParams();
			parms.Texture = LoadingUtils.loadTexture2D(content, "DefaultTerrain");
			this.TerrainImage = new StaticDrawable2D(parms);

			//this.textureColourData = TextureUtils.getColourData2D(this.TerrainImage.Texture);
			//this.textureColourData = TextureUtils.textureTo2DArray(this.TerrainImage.Texture);
			this.textureColourData = TextureUtils.TextureTo2DArray(this.TerrainImage.Texture);
			this.Matrix = Matrix.Identity;
			Matrix test = MatrixUtils.getMatrix(this.TerrainImage);
		}
		#endregion Constructor

		#region Support methods
		public void destroy(Vector2 position, float radius) {

		}

		public void render(SpriteBatch spriteBatch) {
			this.TerrainImage.render(spriteBatch);
		}
		#endregion Support methods
	}
}
