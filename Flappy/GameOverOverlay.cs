using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.D2DLib;

namespace Flappy
{
	public class GameOverOverlay : Renderable
	{
		private D2DSize _bounds;

		public GameOverOverlay(D2DSize bounds) : base()
		{
			_bounds = bounds;
		}

		public override void Render(D2DGraphics gfx)
		{
			gfx.PushTransform();

			gfx.FillRectangle(0, 0, _bounds.width, _bounds.height, new D2DColor(this.Opacity, D2DColor.Red));

			gfx.PopTransform();
		}
	}
}
