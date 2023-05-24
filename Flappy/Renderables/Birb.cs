using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.D2DLib;
using unvell.D2DLib.WinForm;

namespace Flappy.Renderables
{
    internal class Birb : Renderable
    {
		private D2DRect _destRect;
        private D2DRect _srcRect = new D2DRect(0, 0, 34, 24);
        private const int MS_PER_LOOP = 550;
        private long _lastFrameTicks = 0;
		private const float COL_RADIUS = 20f;
        private D2DBitmap[] _sprites;

        public Birb(D2DPoint position, D2DBitmap[] sprites, Size renderSize) : base(position)
        {
            _sprites = sprites;
			_destRect = new D2DRect(D2DPoint.Zero, new D2DSize(renderSize.Width, renderSize.Height));
		}

		public override void Render(D2DGraphics gfx)
        {
            gfx.PushTransform();

            gfx.RotateTransform(Rotation, Position);

            if (_lastFrameTicks == 0)
                _lastFrameTicks = DateTime.Now.Ticks;

            var elap = (DateTime.Now.Ticks - _lastFrameTicks) / TimeSpan.TicksPerMillisecond;
            int frame = (int)Math.Floor(elap / (float)(MS_PER_LOOP / _sprites.Length));

            frame = Math.Min(frame, _sprites.Length - 1);

            if (elap >= MS_PER_LOOP)
                _lastFrameTicks = DateTime.Now.Ticks;

            _destRect.Location = Position.Subtract(new D2DPoint(_destRect.Width * 0.5f, _destRect.Height * 0.5f));
			gfx.DrawBitmap(_sprites[frame], _destRect, _srcRect, 1f, D2DBitmapInterpolationMode.NearestNeighbor);

			gfx.PopTransform();
        }

		public bool IsColliding(D2DRect rect)
		{
			var distX = Math.Abs(Position.x - (rect.X + (rect.Width * 0.5f)));
			var distY = Math.Abs(Position.y - (rect.Y + (rect.Height * 0.5f)));

			if (distX > (rect.Width * 0.5f + COL_RADIUS))
				return false;

			if (distY > (rect.Height * 0.5f + COL_RADIUS))
				return false;

			if (distX <= (rect.Width * 0.5f))
				return true;

			if (distY <= (rect.Height * 0.5f))
				return true;

			var cornerDisSq = Math.Pow(distX - rect.Width * 0.5f, 2f) + Math.Pow(distY - rect.Height * 0.5f, 2f);

			return cornerDisSq <= Math.Pow(COL_RADIUS, 2f);
		}

	}
}
