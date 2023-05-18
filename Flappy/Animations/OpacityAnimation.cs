using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flappy.Renderables;

namespace Flappy.Animations
{
    public class OpacityAnimation : Animation<float>
    {
        public OpacityAnimation(Renderable target, float start, float end, float duration, Func<float, float> easeFunc) : base(target, start, end, duration, easeFunc) { }

        public override void DoStep(float factor)
        {
            var newRot = _start + (_end - _start) * factor;
            _target.Opacity = newRot;
        }
    }
}
