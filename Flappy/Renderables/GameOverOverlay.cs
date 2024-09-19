using unvell.D2DLib;

namespace Flappy.Renderables
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

            gfx.FillRectangle(0, 0, _bounds.width, _bounds.height, new D2DColor(Opacity, D2DColor.Red));

            gfx.PopTransform();
        }
    }
}
