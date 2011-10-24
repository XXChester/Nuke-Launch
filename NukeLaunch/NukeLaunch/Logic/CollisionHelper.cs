using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NukeLaunch.Models;
namespace NukeLaunch.Logic {
	public class CollisionHelper {
		private static bool texturesCollide(Color[,] tex1, Matrix mat1, Color[,] tex2, Matrix mat2, out Vector2 collisionPoint, Color ignoreColour) {
			bool collision = false;
			collisionPoint = new Vector2(-1f);
			int width1 = tex1.GetLength(0);
			int height1 = tex1.GetLength(1);
			int width2 = tex2.GetLength(0);
			int height2 = tex2.GetLength(1);
			Vector2 position1;
			Vector2 position2;
			Vector2 screenCoord;
			for (int y = 0; y < height1; y++) {
				for (int x = 0; x < width1; x++) {
					position1 = new Vector2(x, y);
					position2 = Vector2.Transform(position1, mat2);
					int x2 = (int)position2.X;
					int y2 = (int)position2.Y;
					if (x2 >= 0 && x2 <= width2 && y2 >= 0 && y2 < height2) {
						if (/*(tex1[x, y].A != 0 && tex2[x2, y2].A != 0) &&*/ (tex1[y, x] != ignoreColour && tex2[y2, x2] != ignoreColour)) {
							screenCoord = Vector2.Transform(position1, mat1);
							collision = true;
							break;
						}
					}
				}
				// if we found a collision there is no need to continue
				if (collision) {
					break;
				}
			}
			return collision;
		}

		/*private static bool TexturesCollide(Color[,] tex1, Matrix mat1, Color[,] tex2, Matrix mat2, out Vector2 collisionPoint) {
			bool collision = false;
			collisionPoint = new Vector2(-1f);
			Matrix mat1to2 = mat1 * Matrix.Invert(mat2);
			int width1 = tex1.GetLength(0);
			int height1 = tex1.GetLength(1);
			int width2 = tex2.GetLength(0);
			int height2 = tex2.GetLength(1);

			Vector2 pos1;
			Vector2 pos2;
			int x2;
			int y2;
			for (int x1 = 0; x1 < width1; x1++) {
				for (int y1 = 0; y1 < height1; y1++) {
					pos1 = new Vector2(x1, y1);
					pos2 = Vector2.Transform(pos1, mat1to2);

					x2 = (int)pos2.X;
					y2 = (int)pos2.Y;
					if ((x2 >= 0) && (x2 < width2)) {
						if ((y2 >= 0) && (y2 < height2)) {
								if (tex1[x1, y1].A > 0 && tex2[x2, y2].A > 0) {
								collisionPoint = Vector2.Transform(pos1, mat1);
								collision = true;
								break;
							}
						}
					}
				}

				if (collision) {
					break;
				}
			}

			return collision;
		}*/

		private static bool TexturesCollide(Color[,] tex1, Matrix mat1, Color[,] tex2, Matrix mat2, out Vector2 collisionPoint) {
			Matrix mat1to2 = mat1 * Matrix.Invert(mat2);//This is the problem...all comes up NAN
			int width1 = tex1.GetLength(0);
			int height1 = tex1.GetLength(1);
			int width2 = tex2.GetLength(0);
			int height2 = tex2.GetLength(1);

			for (int x1 = 0; x1 < width1; x1++) {
				for (int y1 = 0; y1 < height1; y1++) {
					Vector2 pos1 = new Vector2(x1, y1);
					Vector2 pos2 = Vector2.Transform(pos1, mat1to2);

					int x2 = (int)pos2.X;
					int y2 = (int)pos2.Y;
					if ((x2 >= 0) && (x2 < width2)) {
						if ((y2 >= 0) && (y2 < height2)) {
							if (tex1[x1, y1].A > 0) {
								if (tex2[x2, y2].A > 0) {
									collisionPoint = Vector2.Transform(pos1, mat1);
									return true;
								}
							}
						}
					}
				}
			}
			collisionPoint = new Vector2(-1, -1);
			return false;
		}



		public static bool collision(Launcher launcher, Terrain terrain) {
			Vector2 collisionPoint;
			return collision(launcher, terrain, out collisionPoint);
		}
		
		public static bool collision(Launcher launcher, Terrain terrain, out Vector2 collisionPoint) {
			bool collision = false;
			if (TexturesCollide(launcher.TextureColourData, launcher.Matrix, terrain.TextureColourData, terrain.Matrix, out collisionPoint)) {
				collision = true;
			}
			return collision;
		}

		public static bool collision(Nuke nuke, Terrain terrain) {
			Vector2 collisionPoint;
			return collision(nuke, terrain, out collisionPoint);
		}

		public static bool collision(Nuke nuke, Terrain terrain, out Vector2 collisionPoint) {
			bool collision = false;
			if (TexturesCollide(nuke.TextureColourData, nuke.Matrix, terrain.TextureColourData, terrain.Matrix, out collisionPoint)) {
				collision = true;
			}
			return collision;
		}

		public static bool collision(Nuke nuke, Launcher launcher) {
			Vector2 collisionPoint;
			return collision(nuke, launcher, out collisionPoint);
		}

		public static bool collision(Nuke nuke, Launcher launcher, out Vector2 collisionPoint) {
			bool collision = false;
			collisionPoint = new Vector2(-1f);
			// first check BBox collision in order to check if we need to do pixel perfect collision
			if (nuke.BBox.Intersects(launcher.BBox) && 
				TexturesCollide(nuke.TextureColourData, nuke.Matrix, launcher.TextureColourData, launcher.Matrix, out collisionPoint)) {
				collision = true;
			}
			return collision;
		}
	}
}
