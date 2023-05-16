﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flappy
{
	public class FlapAnimation : Animation<float>
	{
		private bool _flapUp = false;
		private float _curTarget = 0;


		public FlapAnimation(Renderable target, float start, float end, float duration, Func<float, float> easeFunc) : base(target, start, end, duration, easeFunc) 
		{
			_curTarget = base._end;
		}

		public override void DoStep(float factor)
		{
			var start = _target.Rotation;
			var newRot = start + (_curTarget - start) * factor;
			_target.Rotation = newRot;
		}

		public override void Done()
		{
			_flapUp = false;
			_curTarget = base._start;
			Reset();
		}

		public void Flap()
		{
			_flapUp = true;
			_curTarget = base._end;
			Reset();
		}
	}
}
