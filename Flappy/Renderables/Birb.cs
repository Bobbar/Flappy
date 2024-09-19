using unvell.D2DLib;

namespace Flappy.Renderables
{
    internal class Birb : Renderable
    {
        private D2DRect _destRect;
        private D2DRect _srcRect = new D2DRect(0, 0, 34, 24);
        private const int MS_PER_LOOP = 550;
        private long _lastFrameTicks = 0;
        private const float COL_RADIUS = 40f;
        private D2DBitmap[] _sprites;
        private int _currentFrame = 0;

        public bool Animate = true;
        public int Frame
        {
            get { return _currentFrame; }

            set
            {
                if (value >= 0 && value < _sprites.Length)
                {
                    _currentFrame = value;
                }
            }
        }

        public Birb(D2DPoint position, D2DBitmap[] sprites, Size renderSize) : base(position)
        {
            _sprites = sprites;
            _destRect = new D2DRect(D2DPoint.Zero, new D2DSize(renderSize.Width, renderSize.Height));
        }

        public override void Render(D2DGraphics gfx)
        {
            gfx.PushTransform();

            gfx.RotateTransform(Rotation, Position);

            if (Animate)
            {
                if (_lastFrameTicks == 0)
                    _lastFrameTicks = DateTime.Now.Ticks;

                var elap = (DateTime.Now.Ticks - _lastFrameTicks) / TimeSpan.TicksPerMillisecond;
                _currentFrame = (int)Math.Floor(elap / (float)(MS_PER_LOOP / _sprites.Length));
                _currentFrame = Math.Min(_currentFrame, _sprites.Length - 1);

                if (elap >= MS_PER_LOOP)
                    _lastFrameTicks = DateTime.Now.Ticks;
            }

            _destRect.Location = Position.Subtract(new D2DPoint(_destRect.Width * 0.5f, _destRect.Height * 0.5f));
            gfx.DrawBitmap(_sprites[_currentFrame], _destRect, _srcRect, Opacity, D2DBitmapInterpolationMode.NearestNeighbor);

            gfx.PopTransform();
        }

        public bool IsColliding(D2DRect rect)
        {
            var distX = Math.Abs(Position.X - (rect.X + (rect.Width * 0.5f)));
            var distY = Math.Abs(Position.Y - (rect.Y + (rect.Height * 0.5f)));

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
