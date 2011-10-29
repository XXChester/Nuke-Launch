using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NukeLaunch.Models;
namespace NukeLaunch.Logic {
	public delegate void NukeDelegate(Vector2 position, Vector2 origin, float direction, float power, int ownerID);
	public delegate void NextTurnDelegate(int firedID);
}
