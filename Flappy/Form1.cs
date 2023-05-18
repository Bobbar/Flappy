using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;
using Flappy.Animations;
using Flappy.Renderables;
using unvell.D2DLib;
using unvell.D2DLib.WinForm;


namespace Flappy
{
    public partial class Form1 : Form
	{
		private D2DDevice _device;
		private D2DGraphics _gfx;
		private D2DBitmap[] _birbSprites = new D2DBitmap[3];
		private D2DBitmap _skylineSprite;
		private D2DBitmap _pipeSprite;

		private WaitableTimer _loopTimer = new WaitableTimer();

		private bool _gameOver = false;
		private bool _paused = true;
		private const long TARGET_FPS = 60;
		private long _targetWaitTime = TimeSpan.TicksPerSecond / TARGET_FPS;

		private const float GRAVITY = 9.8f;
		private const float DT = 0.2f;
		private const float PIPE_VELO = -10f;//-8f;
		private const float SKYLINE_PARALLAX_FACT = 0.5f;
		private const int PIPE_FREQ_MS = 2500;
		private const int PIPE_FREQ_VARIATION = 500;
		private const int PIPE_GAP_POS_PADDING = 100;
		private const int PIPE_GAP_SIZE = 120;
		private const int PIPE_WIDTH = 60;
		private const int FALL_ANIM_DELAY = 650;
		private const float FLAP_VELO = -30f;

		private long _lastFlapTime = 0;
		private long _lastPipeTime = 0;
		private long _nextPipeVariation = 0;
		private int _score = 0;
		private Font _scoreFont = new Font("Consolas", 30f);
		private Birb _birb;
		private Skyline _skyline;
		private GameOverOverlay _gameOverOverlay;
		private D2DPoint _birbVelo = D2DPoint.Zero;
		private Animation<float> _birbFallingAnim;
		private FlapAnimation _birbFlapAnim;
		private Animation<float> _gameOverAnimation;

		private List<Pipe> _pipes = new List<Pipe>();
		private Task _renderThread;
		private Random _rnd = new Random(1234);

		public Form1()
		{
			InitializeComponent();

			this.MouseWheel += Form1_MouseWheel;

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

		private float EaseOutElastic(float k)
		{
			const float c4 = (2f * (float)Math.PI) / 3f;

			return k == 0f ? 0f : k == 1f ? 1f : (float)Math.Pow(2f, -10f * k) * (float)Math.Sin((k * 10f - 0.75f) * c4) + 1f;
		}

		private void InitGfx()
		{
			this.Refresh();
			this.Invalidate();

			_device?.Dispose();
			_device = D2DDevice.FromHwnd(this.Handle);
			_gfx = new D2DGraphics(_device);

			for (int i = 0; i < _birbSprites.Length; i++)
			{
				_birbSprites[i]?.Dispose();
			}

			_birbSprites[0] = _device.CreateBitmapFromFile($@".\Sprites\yellowbird-downflap.png");
			_birbSprites[1] = _device.CreateBitmapFromFile($@".\Sprites\yellowbird-midflap.png");
			_birbSprites[2] = _device.CreateBitmapFromFile($@".\Sprites\yellowbird-upflap.png");

			_skylineSprite?.Dispose();
			_skylineSprite = _device.CreateBitmapFromFile($@".\Sprites\skyline.png");

			_pipeSprite?.Dispose();
			_pipeSprite = _device.CreateBitmapFromFile($@".\Sprites\pipe-green.png");

			_device.Resize();

			_gfx.ResetTransform();

			this.Refresh();
			this.Invalidate();
		}

		private void InitBirb()
		{
			_birb = new Birb(new D2DPoint(this.Width * 0.5f, this.Height * 0.5f), _birbSprites);

			_birbFlapAnim = new FlapAnimation(_birb, 0, -45, 400, EaseQuinticOut);
			_birbFallingAnim = new FallingAnimation(_birb, 0, 90, 1000, EaseQuinticOut);

			_skyline = new Skyline(_skylineSprite, new D2DSize(this.Width, this.Height - 20));
			_skyline.Position = new D2DPoint(this.Width, 0);

			_gameOverOverlay = new GameOverOverlay(new D2DSize(this.Width, this.Height));
			_gameOverAnimation = new OpacityAnimation(_gameOverOverlay, 0f, 0.7f, 500, EaseOutElastic);
		}

		private void RenderLoop()
		{
			while (!this.Disposing)
			{
				_gfx.BeginRender(D2DColor.White);

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

					//_birbVelo.y += DT * GRAVITY;
					//_birb.Position = _birb.Position.Add(new D2DPoint(0, DT * _birbVelo.y));

					_pipes.ForEach(p => p.Position = p.Position.Add(new D2DPoint(DT * PIPE_VELO, 0)));
					_skyline.Position = _skyline.Position.Add(new D2DPoint(DT * (PIPE_VELO * SKYLINE_PARALLAX_FACT), 0));

					if (_skyline.Position.x <= 0f)
						_skyline.Position = new D2DPoint(this.Width, 0);

					DoCollisions();
					SpawnPipes();
				}

				if (!_paused)
				{
					_birbVelo.y += DT * GRAVITY;
					_birb.Position = _birb.Position.Add(new D2DPoint(0, DT * _birbVelo.y));
				}

				_skyline.Render(_gfx);
				_pipes.ForEach(p => p.Render(_gfx));
				_birb.Render(_gfx);

				if (_gameOver)
				{
					_birbFallingAnim.Step();
					_gameOverAnimation.Step();
					_gameOverOverlay.Render(_gfx);
				}

				_gfx.DrawText($"{_score}", D2DColor.White, _scoreFont, this.Width * 0.5f, 20f);

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
				if (!bounds.Contains(pipe.Position.Add(new D2DPoint(50f,0))) && pipe.Position.x < (bounds.Width * 0.5f))
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
				_pipes.Add(new Pipe(new D2DPoint(this.Width, _rnd.Next(PIPE_GAP_POS_PADDING, this.Height - (PIPE_GAP_POS_PADDING + 20))), _pipeSprite, PIPE_GAP_SIZE, PIPE_WIDTH));

				_nextPipeVariation = _rnd.Next(-PIPE_FREQ_VARIATION, PIPE_FREQ_VARIATION);
			}
		}

		private void DoFlap()
		{
			if (_gameOver)
				return;

			_paused = false;
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
			_birbVelo.y = 0f;

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

		private void Form1_MouseWheel(object? sender, MouseEventArgs e)
		{
			DoFlap();
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