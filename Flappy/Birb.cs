using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.D2DLib;

namespace Flappy
{
	internal class Birb : Renderable
	{
		private D2DRect rect = new D2DRect(D2DPoint.Zero, new D2DSize(40, 40));
		private D2DRect srcRect = new D2DRect(new D2DPoint(592, 592), new D2DSize(634, 634));

		private D2DBitmap _birbSprite;

		public Birb() : base()
		{
		}

		public Birb(D2DBitmap sprite) : base()
		{
			_birbSprite = sprite;
		}

		public Birb(D2DPoint position) : base(position)
		{
		}

		public Birb(D2DPoint position, D2DBitmap sprite) : base(position)
		{
			_birbSprite = sprite;
		}

		public Birb(D2DPoint position, float rotation) : base(position, rotation)
		{
		}

		public override void Render(D2DGraphics gfx)
		{
			gfx.PushTransform();

			gfx.RotateTransform(Rotation, Position);

			rect.Location = Position.Subtract(new D2DPoint(rect.Width * 0.5f, rect.Height * 0.5f));
			gfx.DrawBitmap(_birbSprite, rect, srcRect, 1f, D2DBitmapInterpolationMode.NearestNeighbor);

			//gfx.FillRectangle(rect, D2DColor.Blue);

			gfx.PopTransform();
		}

		public bool IsColliding(D2DRect rect)
		{
			bool isColliding = false;

			var r = new D2DRect(this.Position, new D2DSize(40, 40));

			if (rect.Contains(new D2DPoint(r.left, r.top)))
				isColliding = true;

			if (rect.Contains(new D2DPoint(r.right, r.top)))
				isColliding = true;

			if (rect.Contains(new D2DPoint(r.left, r.bottom)))
				isColliding = true;

			if (rect.Contains(new D2DPoint(r.right, r.bottom)))
				isColliding = true;

			return isColliding;
		}
		
	}
}
