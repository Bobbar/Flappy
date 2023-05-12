using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flappy
{
	public class RotationAnimation : Animation<float>
	{
		public RotationAnimation(Renderable target, float start, float end, float duration, Func<float, float> easeFunc) : base(target, start, end, duration, easeFunc) { }

		public override void DoStep(float factor)
		{
			
			var newRot = _start + (_end - _start) * factor;
			_target.Rotation = newRot;


			//Debug.WriteLine($"Pos: {_position}  Ang: {newRot}  Rev: {_isReversed}");

		}
	}
}
