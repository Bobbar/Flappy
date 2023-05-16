using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.D2DLib;

namespace Flappy
{
	public class Pipe : Renderable
	{
		private const float GAP = 120f;//100f;
		private const float WIDTH = 50f;

		public bool Entered = false;
		public bool Passed = false;

		private D2DBitmap _sprite;
		private D2DRect _srcRect;

		private D2DRect _capRect = new D2DRect(0, 0, 51, 23);
		private D2DRect _bodyRect = new D2DRect(0, 24, 51, 297);


		public Pipe(D2DPoint position, D2DBitmap sprite) : base(position)
		{
			_sprite = sprite;
			_srcRect = new D2DRect(0,0, _sprite.Width, _sprite.Height);
		}

		public Pipe(D2DPoint position) : base(position)
		{
		}

		public Pipe(D2DPoint position, float rotation) : base(position, rotation)
		{
		}

		public override void Render(D2DGraphics gfx)
		{
			gfx.PushTransform();

			var topRect = GetTopRect();
			var botRect = GetBotRect();

			var botCapRect = new D2DRect(botRect.left, botRect.top, WIDTH, _capRect.Height);
			var botBodyRect = new D2DRect(botRect.left, botRect.top, WIDTH, botRect.Height);
			gfx.DrawBitmap(_sprite, botBodyRect, _bodyRect, 1f, D2DBitmapInterpolationMode.NearestNeighbor);
			gfx.DrawBitmap(_sprite, botCapRect, _capRect, 1f, D2DBitmapInterpolationMode.NearestNeighbor);

			var topBodyRect = new D2DRect(topRect.left, topRect.top, WIDTH, topRect.Height);
			var topBodyCenter = new D2DPoint(topBodyRect.X + (topBodyRect.Width * 0.5f), topBodyRect.Y + (topBodyRect.Height * 0.5f));
			gfx.RotateTransform(180, topBodyCenter);
			gfx.DrawBitmap(_sprite, topBodyRect, _bodyRect, 1f, D2DBitmapInterpolationMode.NearestNeighbor);

			gfx.PopTransform();
			gfx.PushTransform();

			var topCapRect = new D2DRect(topRect.left, topRect.top + (topRect.Height - _capRect.Height), WIDTH, _capRect.Height);
			var topCenter = new D2DPoint(topCapRect.X + (topCapRect.Width * 0.5f), topCapRect.Y + (topCapRect.Height * 0.5f));
			gfx.RotateTransform(180, topCenter);
			gfx.DrawBitmap(_sprite, topCapRect, _capRect, 1f, D2DBitmapInterpolationMode.NearestNeighbor);

			gfx.PopTransform();
		}

		private D2DRect GetTopRect()
		{
			return new D2DRect(Position.x - (WIDTH * 0.5f), 0, WIDTH, Position.y - (GAP * 0.5f));
		}

		private D2DRect GetBotRect()
		{
			return new D2DRect(Position.x - (WIDTH * 0.5f), Position.y + (GAP * 0.5f), WIDTH, 3000f); // TODO: How to know bounds of viewport?
		}

		public D2DRect[] GetRects()
		{
			var rects = new D2DRect[2];
			rects[0] = GetTopRect();
			rects[1] = GetBotRect();
			return rects;
		}

		public D2DRect GetGapRect()
		{
			return new D2DRect(Position.x - (WIDTH * 0.5f), Position.y - (GAP * 0.5f), WIDTH, GAP);
		}
	}
}
