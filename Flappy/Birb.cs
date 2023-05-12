using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.D2DLib;

namespace Flappy
{
	internal class Birb : Renderable
	{
		private D2DRect rect = new D2DRect(D2DPoint.Zero, new D2DSize(40, 40));

		public override void Render(D2DGraphics gfx)
		{
			gfx.PushTransform();
			gfx.RotateTransform(Rotation);

			gfx.PushTransform();

			gfx.TranslateTransform(Position.x, Position.y);

			gfx.FillRectangle(rect, D2DColor.Blue);

			gfx.PopTransform();
			gfx.PopTransform();


		}
	}
}
