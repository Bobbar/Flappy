using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using unvell.D2DLib;

namespace Flappy
{
	public static class Extentions
	{
		public static D2DPoint Add(this D2DPoint point, D2DPoint other)
		{
			return new D2DPoint(point.x + other.x, point.y + other.y);
		}
	}
}
