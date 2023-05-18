using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.D2DLib;

namespace Flappy.Renderables
{
    public abstract class Renderable
    {
        public D2DPoint Position { get; set; }

        public float Rotation { get; set; }

        public float Opacity { get; set; } = 1.0f;

        public abstract void Render(D2DGraphics gfx);

        public Renderable() { }

        public Renderable(D2DPoint position, float rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public Renderable(D2DPoint position)
        {
            Position = position;
        }
    }
}
