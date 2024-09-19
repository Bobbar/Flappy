using unvell.D2DLib;

namespace Flappy.Renderables
{
    public class Impact : Renderable
    {
        private D2DRect _clipRect;
        private D2DBitmap _sprite;
        private D2DRect _srcRect;
        private float _angle = 0;

        public Impact(D2DPoint position, D2DRect clipRect, D2DBitmap sprite) : base(position)
        {
            _sprite = sprite;
            _clipRect = clipRect;

            const int deflatePad = 2;
            _clipRect.left += deflatePad;
            _clipRect.right -= deflatePad;
            _clipRect.top += deflatePad;
            _clipRect.bottom -= deflatePad;

            _srcRect = new D2DRect(0, 0, _sprite.Size.width, _sprite.Size.height);
            _angle = new Random().Next(0, 360);
        }

        public override void Render(D2DGraphics gfx)
        {
            gfx.PushTransform();
            gfx.PushClip(_clipRect);

            gfx.RotateTransform(_angle, Position);

            gfx.DrawBitmap(_sprite, new D2DRect(Position.X - (_sprite.Width * 0.5f), Position.Y - (_sprite.Height * 0.5f), _srcRect.Width, _srcRect.Height), _srcRect, 0.6f, D2DBitmapInterpolationMode.NearestNeighbor);

            gfx.PopClip();
            gfx.PopTransform();
        }
    }
}
