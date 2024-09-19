using unvell.D2DLib;

namespace Flappy.Renderables
{
    public class Skyline : Renderable
    {
        private D2DBitmap _sprite;
        private D2DSize _bounds;
        private D2DRect _srcRect;
        private D2DRect[] _destRects;

        public Skyline(D2DBitmap sprite, D2DSize bounds) : base()
        {
            _sprite = sprite;
            _bounds = bounds;
            _srcRect = new D2DRect(0, 0, _sprite.Size.width, _sprite.Size.height);

            var numRects = (int)Math.Ceiling(_bounds.width / _sprite.Width) + 1;
            _destRects = new D2DRect[numRects];

            for (int i = 0; i < numRects; i++)
            {
                _destRects[i] = new D2DRect(Position.X + (_sprite.Width * i), 0, _sprite.Width, _bounds.height);
            }
        }

        public override void Render(D2DGraphics gfx)
        {
            gfx.PushTransform();

            gfx.TranslateTransform(Position.X, Position.Y);

            foreach (var rect in _destRects)
            {
                gfx.DrawBitmap(_sprite, rect, _srcRect, 1f, D2DBitmapInterpolationMode.NearestNeighbor);
            }

            gfx.PopTransform();

            if (Position.X <= -_sprite.Width)
            {
                var diff = _sprite.Width - Math.Abs(Position.X);
                Position = new D2DPoint(diff, 0);
            }
        }
    }
}
