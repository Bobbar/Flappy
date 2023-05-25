using Flappy.Animations;
using Flappy.Renderables;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Numerics;
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
		private D2DBitmap _groundSprite;
		private D2DBitmap _impactSprite;

		private bool _gameOver = false;
		private bool _paused = true;

		private const long TARGET_FPS = 60;
		private long _targetWaitTime = TimeSpan.TicksPerSecond / TARGET_FPS;

		private const float GRAVITY = 9.8f;
		private const float DT = 0.2f;
		private const float BIRB_SPEED = -15f;
		private readonly Size BIRB_RENDER_SIZE = new Size(40, 28);
		private const float SKYLINE_PARALLAX_FACT = 0.3f;
		private const int PIPE_SPACING = 400;
		private const int PIPE_SPACING_VARIATION = 50;
		private const int PIPE_GAP_POS_PADDING = 100;
		private const int PIPE_GAP_SIZE = 100;
		private const int PIPE_WIDTH = 60;
		private const int GROUND_HEIGHT = 112;
		private const int FALL_ANIM_DELAY = 500;
		private const float FLAP_VELO = -30f;
		private const int RND_SEED = 4321;
		private const bool FIXED_SEED = false;

		private long _lastFlapTime = 0;
		private long _lastPipeTime = 0;
		private long _nextPipeVariation = 0;
		private int _score = 0;
		private long _distance = 0;
		private Font _scoreFont = new Font("Consolas", 30f);
		private Birb _birb;
		private Birb _deadBirb;
		private Vector2[] _birbCollisionMask;
		private D2DSize _birbCollisionMaskSize;
		private Skyline _skyline;
		private Ground _ground;
		private GifRenderer _impactGif;
		private List<Impact> _impacts = new List<Impact>();
		private GameOverOverlay _gameOverOverlay;
		private D2DPoint _birbVelo = D2DPoint.Zero;
		private Animation<float> _birbFallingAnim;
		private FlapAnimation _birbFlapAnim;
		private Animation<float> _gameOverAnimation;
		private List<Pipe> _pipes = new List<Pipe>();

		private Task _renderThread;
		private WaitableTimer _loopTimer = new WaitableTimer();
		private Stopwatch _renderTimer = new Stopwatch();
		private Random _rnd = new Random();

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
			InitGameObjects();

			Reset();

			_renderThread = new Task(RenderLoop, TaskCreationOptions.LongRunning);
			_renderThread.Start();
		}

		private void InitGfx()
		{
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

			_impactSprite?.Dispose();
			_impactSprite = _device.CreateBitmapFromFile($@".\Sprites\bloodsplats_0004.png");

			_groundSprite?.Dispose();
			_groundSprite = _device.CreateBitmapFromFile($@".\Sprites\base.png");

			_pipeSprite?.Dispose();
			_pipeSprite = _device.CreateBitmapFromFile($@".\Sprites\pipe-green.png");

			_device.Resize();
			_gfx.Antialias = false;

			CreateCollisionMask(Bitmap.FromFile($@".\Sprites\yellowbird-midflap.png"));
		}

		private void InitGameObjects()
		{
			_birb = new Birb(new D2DPoint(this.Width * 0.5f, this.Height * 0.5f), _birbSprites, BIRB_RENDER_SIZE);
			_deadBirb = new Birb(new D2DPoint(this.Width * 0.5f, this.Height * 0.5f), _birbSprites, BIRB_RENDER_SIZE);
			_deadBirb.Opacity = 0.5f;
			_deadBirb.Animate = false;

			_birbFlapAnim = new FlapAnimation(_birb, 0, -25, 300, EasingFunctions.EaseQuinticOut);
			_birbFallingAnim = new FallingAnimation(_birb, 0, 90, 300, EasingFunctions.EaseQuinticIn);

			_skyline = new Skyline(_skylineSprite, new D2DSize(this.Width, this.Height - 20));

			_ground = new Ground(_groundSprite, new D2DSize(this.Width, this.Height), GROUND_HEIGHT);

			_impactGif = new GifRenderer(D2DPoint.Zero, Image.FromFile($@".\Sprites\splash.gif"), _gfx);

			_gameOverOverlay = new GameOverOverlay(new D2DSize(this.Width, this.Height));
			_gameOverAnimation = new OpacityAnimation(_gameOverOverlay, 0.7f, 0.4f, 1000, EasingFunctions.EaseOutElastic);
		}

		private void RenderLoop()
		{
			while (!this.Disposing)
			{
				_renderTimer.Restart();

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

					_pipes.ForEach(p => p.Position = p.Position.Add(new D2DPoint(DT * BIRB_SPEED, 0)));
					_skyline.Position = _skyline.Position.Add(new D2DPoint(DT * (BIRB_SPEED * SKYLINE_PARALLAX_FACT), 0));
					_ground.Position = _ground.Position.Add(new D2DPoint(DT * BIRB_SPEED, 0));
					_distance += -(long)(DT * BIRB_SPEED);

					SpawnPipes();
				}

				if (!_paused)
				{
					_birbVelo.y += DT * GRAVITY;
					_birb.Position = _birb.Position.Add(new D2DPoint(0, DT * _birbVelo.y));

					if (!_gameOver)
						DoCollisions();
				}

				_skyline.Render(_gfx);
				_pipes.ForEach(p => p.Render(_gfx));
				_ground.Render(_gfx);
				_impacts.ForEach(i => i.Render(_gfx));
				_birb.Render(_gfx);
				_impactGif.Render(_gfx);

				if (_gameOver)
				{
					_deadBirb.Render(_gfx);
					_birbFallingAnim.Step();
					_gameOverAnimation.Step();
					_gameOverOverlay.Render(_gfx);
				}

				_gfx.DrawText($"{_score}", D2DColor.White, _scoreFont, this.Width * 0.5f, 20f);

				//DrawCollisionMask();

				_gfx.EndRender();

				_renderTimer.Stop();

				_loopTimer.Wait(_targetWaitTime - _renderTimer.ElapsedTicks, false);
			}
		}

		private unsafe void CreateCollisionMask(Image src)
		{
			var points = new List<Vector2>();
			_birbCollisionMaskSize = new D2DSize(src.Width, src.Height);

			using (var mask = new Bitmap((Image)src.Clone()))
			{
				var data = mask.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadWrite, src.PixelFormat);
				byte* pixels = (byte*)data.Scan0;

				for (int x = 0; x < src.Width; x++)
				{
					for (int y = 0; y < src.Height; y++)
					{
						int pidx = (y * src.Width + x) * 4;

						if (pixels[pidx + 3] > 0)
						{
							points.Add(new Vector2(x, y));
						}
					}
				}

				mask.UnlockBits(data);
			}

			_birbCollisionMask = points.ToArray();
		}

		private unsafe bool PerPixelCollision(D2DRect rect)
		{
			bool isColliding = false;
			var renderSzHalf = new Vector2(BIRB_RENDER_SIZE.Width * 0.5f, BIRB_RENDER_SIZE.Height * 0.5f);
			var mat = Matrix3x2.CreateScale(new Vector2(BIRB_RENDER_SIZE.Width / _birbCollisionMaskSize.width, BIRB_RENDER_SIZE.Height / _birbCollisionMaskSize.height));
			mat *= Matrix3x2.CreateRotation(_birb.Rotation * (float)(Math.PI / 180f), renderSzHalf);
			mat *= Matrix3x2.CreateTranslation(new Vector2(_birb.Position.x - renderSzHalf.X, _birb.Position.y - renderSzHalf.Y));

			foreach (var pnt in _birbCollisionMask)
			{
				var transPnt = Vector2.Transform(pnt, mat);
				if (rect.Contains(new D2DPoint(transPnt.X, transPnt.Y)))
				{
					isColliding = true;
					break;
				}
			}

			return isColliding;
		}

		private void DrawCollisionMask()
		{
			var renderSzHalf = new Vector2(BIRB_RENDER_SIZE.Width * 0.5f, BIRB_RENDER_SIZE.Height * 0.5f);
			var mat = Matrix3x2.CreateScale(new Vector2(BIRB_RENDER_SIZE.Width / _birbCollisionMaskSize.width, BIRB_RENDER_SIZE.Height / _birbCollisionMaskSize.height));
			mat *= Matrix3x2.CreateRotation(_birb.Rotation * (float)(Math.PI / 180f), renderSzHalf);
			mat *= Matrix3x2.CreateTranslation(new Vector2(_birb.Position.x - renderSzHalf.X, _birb.Position.y - renderSzHalf.Y));

			foreach (var pnt in _birbCollisionMask)
			{
				var transPnt = Vector2.Transform(pnt, mat);
				_gfx.FillRectangle(new D2DPoint(transPnt.X, transPnt.Y), new D2DSize(1f, 1f), D2DColor.Red);
			}
		}

		private void BirbImpact(D2DRect clipRect)
		{
			_impacts.Add(new Impact(_birb.Position, clipRect, _impactSprite));
			_impactGif.Restart(_birb.Position);
			_birbVelo = D2DPoint.Zero;
			_deadBirb.Position = _birb.Position;
			_deadBirb.Rotation = _birb.Rotation;
			_deadBirb.Frame = _birb.Frame;
			_gameOverAnimation.Reset();
			_gameOver = true;
		}

		private void DoCollisions()
		{
			if (_birb.Position.y >= this.Height - GROUND_HEIGHT - 20)
			{
				var colRect = new D2DRect(0, this.Height - GROUND_HEIGHT, this.Width, this.Height);

				if (_birb.IsColliding(colRect))
				{
					if (PerPixelCollision(colRect))
					{
						BirbImpact(colRect);
						return;
					}
				}
			}

			var bounds = new D2DRect(0, 0, this.Width, this.Height);
			for (int i = 0; i < _pipes.Count; i++)
			{
				var pipe = _pipes[i];
				if (!bounds.Contains(pipe.Position.Add(new D2DPoint(50f, 0))) && pipe.Position.x < (bounds.Width * 0.5f))
				{
					_pipes.RemoveAt(i);
				}
				else
				{
					foreach (var rect in pipe.GetRects())
					{
						if (_birb.IsColliding(rect))
						{
							if (PerPixelCollision(rect))
							{
								BirbImpact(rect);
								return;
							}
						}
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
			var dist = _distance - _lastPipeTime;
			if (dist >= PIPE_SPACING + _nextPipeVariation || _lastPipeTime == 0)
			{
				_lastPipeTime = _distance;
				_pipes.Add(new Pipe(new D2DPoint(this.Width, _rnd.Next(PIPE_GAP_POS_PADDING, this.Height - (PIPE_GAP_POS_PADDING + GROUND_HEIGHT))), _pipeSprite, PIPE_GAP_SIZE, PIPE_WIDTH));
				_nextPipeVariation = _rnd.Next(-PIPE_SPACING_VARIATION, 0);
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
			_birb.Rotation = 0;
			_lastPipeTime = 0;
			_score = 0;
			_birbVelo.y = 0f;
			_impacts.Clear();
			_rnd = FIXED_SEED ? new Random(RND_SEED) : new Random();
			_distance = 0;

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