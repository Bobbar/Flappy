using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.D2DLib;

namespace Flappy
{
	public class Skyline : Renderable
	{
		private D2DBitmap _sprite;
		private D2DSize _bounds;
		private D2DRect _srcRect;

		public Skyline(D2DBitmap sprite, D2DSize bounds) : base()
		{
			_sprite = sprite;
			_bounds = bounds;
			_srcRect = new D2DRect(0, 0, _sprite.Size.width, _sprite.Size.height);
		}

		public override void Render(D2DGraphics gfx)
		{
			gfx.PushTransform();

			var destRect1 = new D2DRect(Position.x - _bounds.width, 0, _bounds.width, _bounds.height);
			gfx.DrawBitmap(_sprite, destRect1, _srcRect, 1f, D2DBitmapInterpolationMode.NearestNeighbor);

			var destRect2 = new D2DRect(Position.x - 1f, 0, _bounds.width, _bounds.height);
			gfx.DrawBitmap(_sprite, destRect2, _srcRect, 1f, D2DBitmapInterpolationMode.NearestNeighbor);

			gfx.PopTransform();
		}
	}
}
