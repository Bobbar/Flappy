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

			gfx.FillRectangle(topRect, D2DColor.DarkGreen);
			gfx.FillRectangle(botRect, D2DColor.DarkGreen);

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
