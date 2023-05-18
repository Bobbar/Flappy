using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.D2DLib;

namespace Flappy.Renderables
{
    public class Pipe : Renderable
    {
        public bool Entered = false;
        public bool Passed = false;

        private int _gap;
        private int _width;
        private D2DBitmap _sprite;
        private D2DRect _capRect = new D2DRect(0, 0, 52, 24);
        private D2DRect _bodyRect = new D2DRect(0, 24, 52, 297);

        public Pipe(D2DPoint position, D2DBitmap sprite, int gapSize, int width) : base(position)
        {
            _sprite = sprite;
            _gap = gapSize;
            _width = width;
        }

        public override void Render(D2DGraphics gfx)
        {
            gfx.PushTransform();

            var topRect = GetTopRect();
            var botRect = GetBotRect();

            var botCapRect = new D2DRect(botRect.left, botRect.top, _width, _capRect.Height);
            var botBodyRect = new D2DRect(botRect.left, botRect.top, _width, botRect.Height);
            gfx.DrawBitmap(_sprite, botBodyRect, _bodyRect, 1f, D2DBitmapInterpolationMode.NearestNeighbor);
            gfx.DrawBitmap(_sprite, botCapRect, _capRect, 1f, D2DBitmapInterpolationMode.NearestNeighbor);

            var topBodyRect = new D2DRect(topRect.left, topRect.top, _width, topRect.Height);
            var topBodyCenter = new D2DPoint(topBodyRect.X + topBodyRect.Width * 0.5f, topBodyRect.Y + topBodyRect.Height * 0.5f);
            gfx.RotateTransform(180, topBodyCenter);
            gfx.DrawBitmap(_sprite, topBodyRect, _bodyRect, 1f, D2DBitmapInterpolationMode.NearestNeighbor);

            gfx.PopTransform();
            gfx.PushTransform();

            var topCapRect = new D2DRect(topRect.left, topRect.top + (topRect.Height - _capRect.Height), _width, _capRect.Height);
            var topCenter = new D2DPoint(topCapRect.X + topCapRect.Width * 0.5f, topCapRect.Y + topCapRect.Height * 0.5f);
            gfx.RotateTransform(180, topCenter);
            gfx.DrawBitmap(_sprite, topCapRect, _capRect, 1f, D2DBitmapInterpolationMode.NearestNeighbor);

            gfx.PopTransform();
        }

        private D2DRect GetTopRect()
        {
            return new D2DRect(Position.x - _width * 0.5f, 0, _width, Position.y - _gap * 0.5f);
        }

        private D2DRect GetBotRect()
        {
            return new D2DRect(Position.x - _width * 0.5f, Position.y + _gap * 0.5f, _width, 3000f); // TODO: How to know bounds of viewport?
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
            return new D2DRect(Position.x - _width * 0.5f, Position.y - _gap * 0.5f, _width, _gap);
        }
    }
}
