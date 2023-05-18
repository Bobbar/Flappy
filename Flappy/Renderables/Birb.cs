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
        private const int _sizeX = 40;
        private const int _sizeY = 30;
        private D2DSize _size = new D2DSize(_sizeX, _sizeY);
        private D2DRect _destRect = new D2DRect(D2DPoint.Zero, new D2DSize(_sizeX, _sizeY));
        private D2DRect _srcRect = new D2DRect(0, 0, 34, 24);

        private const int MS_PER_LOOP = 550;
        private long _lastFrameTicks = 0;

        private D2DBitmap[] _sprites;

        public Birb(D2DPoint position, D2DBitmap[] sprites) : base(position)
        {
            _sprites = sprites;
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

            //gfx.DrawRectangle(_destRect, D2DColor.Blue);
            //gfx.FillEllipse(Position.Subtract(new D2DPoint(5f, 5f)), 10f, D2DColor.Red);

            gfx.PopTransform();
        }

        public bool IsColliding(D2DRect rect)
        {
            bool isColliding = false;

            var r = new D2DRect(Position, _size);

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
