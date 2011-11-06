using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NukeLaunch.Models;
using GWNorthEngine.Utils;
namespace NukeLaunch.Logic {
	public class CollisionHelper {
		public static bool collision(Launcher launcher, Terrain terrain) {
			Vector2 collisionPoint;
			return collision(launcher, terrain, out collisionPoint);
		}
		
		public static bool collision(Launcher launcher, Terrain terrain, out Vector2 collisionPoint) {
			bool collision = false;
			if (CollisionUtils.doPixelsIntersect(launcher.TextureColourData, launcher.Matrix, terrain.TextureColourData, terrain.Matrix, out collisionPoint)) {
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
			if (CollisionUtils.doPixelsIntersect(nuke.TextureColourData, nuke.Matrix, terrain.TextureColourData, terrain.Matrix, out collisionPoint)) {
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
			if (CollisionUtils.doPixelsIntersect(nuke.TextureColourData, nuke.Matrix, launcher.TextureColourData, launcher.Matrix, out collisionPoint)) {
				collision = true;
			}
			return collision;
		}
	}
}
