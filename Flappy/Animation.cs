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

		protected Animation() { }

		protected Animation(Renderable target, T start, T end, float duration, Func<float, float> easeFunc)
		{
			_target = target;
			_start = start;
			_end = end;
			_duration = duration;
			_easeFunc = easeFunc;
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

			//var factor = _easeFunc(_position);

			//Debug.WriteLine($"Pos: {_position}  Fac: {factor}  Rev: {_isReversed}");

			//if (_isReversed)
			//	factor = -factor;
			

			if (elap < _duration)
			{
				var factor = _easeFunc(_position);

				Debug.WriteLine($"Pos: {_position}  Fac: {factor}  Rev: {_isReversed}");

				if (_isReversed)
					factor = -factor;

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
			}
		}

		public void Reverse()
		{
			_isReversed = !_isReversed;
			_startTicks = DateTime.Now.Ticks;
			_position = 0f;

			Debug.WriteLine($"[Reverse]  Rev: {_isReversed}");
		}

		public abstract void DoStep(float factor);

	}



	//public class Animation<PType, RType>
	//{
	//	private Func<PType, RType> _property;
	//	private RType _start;
	//	private RType _end;
	//	private float _duration;

	//	public Animation(Func<PType, RType> property, RType start, RType end, float duration)
	//	{
	//		_property = property;
	//		_start = start;
	//		_end = end;
	//		_duration = duration;
	//	}

	//	public void Step(float dt)
	//	{
	//		//_property.Invoke(dt);
	//	}
	//}
}
