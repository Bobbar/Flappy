using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flappy
{
	public class FallingAnimation : Animation<float>
	{

		public FallingAnimation(Renderable target, float start, float end, float duration, Func<float, float> easeFunc) : base(target, start, end, duration, easeFunc) { }

		public override void DoStep(float factor)
		{
			var start = _target.Rotation;
			var newRot = start + (_end - start) * factor;
			_target.Rotation = newRot;
		}

		public override void Done()
		{
			base.Done();

			Reset();
		}
	}
}
