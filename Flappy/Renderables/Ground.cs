using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.D2DLib;

namespace Flappy.Renderables
{
	public class Ground : Renderable
	{
		private D2DBitmap _sprite;
		private D2DSize _bounds;
		private D2DRect _srcRect;
		private int _height;
		private D2DRect[] _destRects;

		public Ground(D2DBitmap sprite, D2DSize bounds, int height) : base()
		{
			_sprite = sprite;
			_bounds = bounds;
			_height = height;
			_srcRect = new D2DRect(0, 0, _sprite.Size.width, _sprite.Height);

			var numRects = (int)Math.Ceiling(_bounds.width / _sprite.Width) + 1;
			_destRects = new D2DRect[numRects];

			for (int i = 0; i < numRects; i++)
			{
				_destRects[i] = new D2DRect(Position.x + (_sprite.Width * i), _bounds.height - _height, _sprite.Width, _sprite.Height);
			}
		}

		public override void Render(D2DGraphics gfx)
		{
			gfx.PushTransform();

			gfx.TranslateTransform(Position.x, Position.y);

			foreach (var rect in _destRects)
			{
				gfx.DrawBitmap(_sprite, rect, _srcRect, 1f, D2DBitmapInterpolationMode.NearestNeighbor);
			}

			gfx.PopTransform();

			if (Position.x <= -_sprite.Width)
				Position = new D2DPoint(0, 0);
		}
	}
}
