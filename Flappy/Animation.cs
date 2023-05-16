using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flappy
{



	public abstract class Animation<T>
	{
		public Renderable _target;

		public T _start;
		public T _end;
		public float _duration;
		public float _position;
		public long _startTicks;
		public Func<float, float> _easeFunc;

		public bool IsPlaying = false;
		public bool Loop = false;
		public bool ReverseOnLoop = false;

		protected bool _isReversed = false;

		protected T _ogStart;
		protected T _ogEnd;

		protected T _tempEnd;

		protected Animation() { }

		protected Animation(Renderable target, T start, T end, float duration, Func<float, float> easeFunc)
		{
			_target = target;
			_start = start;
			_end = end;
			_duration = duration;
			_easeFunc = easeFunc;

			_ogStart = start;
			_ogEnd = end;
		}

		public void Step()
		{
			if (!IsPlaying)
			{
				IsPlaying = true;
				_startTicks = DateTime.Now.Ticks;
			}

			var elap = DateTime.Now.Ticks - _startTicks;
			_position = elap / _duration;

			if (elap < _duration)
			{
				var factor = _easeFunc(_position);
				DoStep(factor);
			}
			else
			{
				if (Loop)
				{
					_startTicks = DateTime.Now.Ticks;
					_position = 0f;

					if (ReverseOnLoop)
						_isReversed = !_isReversed;
				}
				else
				{
					//IsPlaying = false;
					Done();
				}
			}
		}

		public void Reset()
		{
			_startTicks = DateTime.Now.Ticks;
		}

		public virtual void Reverse()
		{
		
		}

		public virtual void DoStep(float factor)
		{

		}

		public virtual void Done()
		{

		}

		//public abstract void Reverse();

		//public abstract void DoStep(float factor);

		//public abstract void Done();
	}
}
