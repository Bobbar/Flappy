using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;
using unvell.D2DLib;
using unvell.D2DLib.WinForm;


namespace Flappy
{
	public partial class Form1 : Form
	{
		private D2DDevice _device;
		private D2DGraphics _gfx;
		private D2DBitmap _birbSprite;
		private WaitableTimer _loopTimer = new WaitableTimer();

		private bool _gameOver = false;
		private bool _paused = true;
		private const long TARGET_FPS = 60;
		private long _targetWaitTime = TimeSpan.TicksPerSecond / TARGET_FPS;

		private const float GRAVITY = 9.8f;
		private const float DT = 0.2f; //0.3f;
		private const float PIPE_VELO = -10f;//-8f;
		private const int PIPE_FREQ_MS = 3000;
		private const int PIPE_FREQ_VARIATION = 1000;
		private const int FALL_ANIM_DELAY = 650;
		private const float FLAP_VELO = -30f;

		private long _lastFlapTime = 0;
		private long _lastPipeTime = 0;
		private long _nextPipeVariation = 0;
		private int _score = 0;
		private Font _scoreFont = new Font("Consolas", 30f);
		private Birb _birb;
		private D2DPoint _birbVelo = D2DPoint.Zero;
		private Animation<float> _birbFallingAnim;
		private FlapAnimation _birbFlapAnim;
		private List<Pipe> _pipes = new List<Pipe>();
		private Task _renderThread;
		private Random _rnd = new Random(1234);

		public Form1()
		{
			InitializeComponent();

			this.DoubleBuffered = false;
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			InitGfx();
			InitBirb();

			_renderThread = new Task(RenderLoop, TaskCreationOptions.LongRunning);
			_renderThread.Start();
		}

		private float EaseQuinticOut(float k)
		{
			return 1f + ((k -= 1f) * (float)Math.Pow(k, 4));
		}

		private void InitGfx()
		{
			this.Refresh();
			this.Invalidate();

			_device?.Dispose();
			_device = D2DDevice.FromHwnd(this.Handle);
			_gfx = new D2DGraphics(_device);

			_birbSprite?.Dispose();
			_birbSprite = _device.CreateBitmapFromFile($@".\birb_sprite.png");

			_device.Resize();

			_gfx.ResetTransform();

			this.Refresh();
			this.Invalidate();
		}

		private void InitBirb()
		{
			_birb = new Birb(new D2DPoint(this.Width * 0.5f, this.Height * 0.5f), _birbSprite);
			_birbFlapAnim = new FlapAnimation(_birb, 0, -45, 300 * 10000, EaseQuinticOut);
			_birbFallingAnim = new FallingAnimation(_birb, 0, 90, 500 * 10000, EaseQuinticOut);
		}

		private void RenderLoop()
		{
			while (!this.Disposing)
			{
				_gfx.BeginRender(_gameOver ? D2DColor.Red : D2DColor.White);

				if (!_gameOver && !_paused)
				{
					var elap = (DateTime.Now.Ticks - _lastFlapTime) / TimeSpan.TicksPerMillisecond;
					if (elap > FALL_ANIM_DELAY)
					{
						_birbFallingAnim.Step();
					}
					else
					{
						_birbFlapAnim.Step();
					}

					_birbVelo.y += DT * GRAVITY;
					_birb.Position = _birb.Position.Add(new D2DPoint(0, DT * _birbVelo.y));
					_pipes.ForEach(p => p.Position = p.Position.Add(new D2DPoint(DT * PIPE_VELO, 0)));

					DoCollisions();
					SpawnPipes();
				}

				_birb.Render(_gfx);
				_pipes.ForEach(p => p.Render(_gfx));

				_gfx.DrawText($"{_score}", D2DColor.Black, _scoreFont, this.Width * 0.5f, 20f);

				_gfx.EndRender();

				_loopTimer.Wait(_targetWaitTime, false);
			}
		}

		private void DoCollisions()
		{
			if (_birb.Position.y >= this.Height)
			{
				_birb.Position = new D2DPoint(_birb.Position.x, this.Height);
				_birbVelo = D2DPoint.Zero;
				_gameOver = true;
			}

			var bounds = new D2DRect(0, 0, this.Width, this.Height);
			for (int i = 0; i < _pipes.Count; i++)
			{
				var pipe = _pipes[i];
				if (!bounds.Contains(pipe.Position))
				{
					_pipes.RemoveAt(i);
				}
				else
				{
					foreach (var rect in pipe.GetRects())
					{
						if (_birb.IsColliding(rect))
							_gameOver = true;
					}

					if (!pipe.Entered && !pipe.Passed && _birb.IsColliding(pipe.GetGapRect()))
					{
						pipe.Entered = true;
					}

					if (pipe.Entered && !pipe.Passed && !_birb.IsColliding(pipe.GetGapRect()))
					{
						pipe.Passed = true;
						_score++;
					}
				}
			}
		}

		private void SpawnPipes()
		{
			var elap = DateTime.Now.Ticks - _lastPipeTime;

			if (elap / 10000 > PIPE_FREQ_MS + _nextPipeVariation || _lastPipeTime == 0)
			{
				_lastPipeTime = DateTime.Now.Ticks;

				_pipes.Add(new Pipe(new D2DPoint(this.Width, _rnd.Next(100, this.Height - 100))));

				_nextPipeVariation = _rnd.Next(-PIPE_FREQ_VARIATION, PIPE_FREQ_VARIATION);
			}

		}

		private void DoFlap()
		{
			_paused = false;
			//_birbAnim.Reverse();
			_birbFlapAnim.Flap();
			_birbVelo.y = FLAP_VELO;
			_lastFlapTime = DateTime.Now.Ticks;

		}

		private void Reset()
		{
			_pipes.Clear();
			_gameOver = false;
			_paused = true;
			_birb.Position = new D2DPoint(this.Width * 0.5f, this.Height * 0.5f);
			_lastPipeTime = 0;
			_score = 0;

			InitBirb();


			_device?.Resize();
		}

		private void Form1_Click(object sender, EventArgs e)
		{
			if (_renderThread.Status != TaskStatus.Running)
				_renderThread.Start();
		}

		private void Form1_MouseDown(object sender, MouseEventArgs e)
		{
			DoFlap();

			if (e.Button == MouseButtons.Right)
				Reset();
		}

		private void Form1_ResizeEnd(object sender, EventArgs e)
		{
			_device?.Resize();
		}

		private void Form1_Resize(object sender, EventArgs e)
		{
			_device?.Resize();
		}
	}
}