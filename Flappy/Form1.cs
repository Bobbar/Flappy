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
		private WaitableTimer _loopTimer = new WaitableTimer();

		private const long _targetFPS = 60;
		private long _targetWaitTime = TimeSpan.TicksPerSecond / _targetFPS;


		private Birb testBirb = new Birb();
		private Animation<float> testAnim; //= new RotationAnimation(testBirb, 0, 180, 3000 * 10000, EaseQuinticOut);

		private Task _renderThread;

		private bool _mouseClick = false;

		public Form1()
		{
			InitializeComponent();

			//InitGfx();

		


			testBirb.Position = new D2DPoint(100, 100);
			testAnim = new RotationAnimation(testBirb, 0, 90, 1000 * 10000, EaseQuinticOut);
			//testAnim.Loop = true;
			//testAnim.ReverseOnLoop = true;


			_renderThread = new Task(RenderLoop);
			_renderThread.Start();



			//var anim = new Animation<Birb,float>(t => t.Rotation, 0f, 100f, 100f);


			//var f = new Func<Birb, float>((b) => b.Rotation);
			////f(test) += 100f;

			//var n = f(test);
			//n += 100f;
			//f(test) = n;

		}

		//protected override void CreateHandle()
		//{
		//	base.CreateHandle();

		//	this.DoubleBuffered = false;

		//	InitGfx();

		//	_renderThread = new Task(RenderLoop);
		//	_renderThread.Start();
		//}

		private float EaseQuinticOut(float k)
		{
			return 1f + ((k -= 1f) * (float)Math.Pow(k, 4));
		}

		private void InitGfx()
		{
			_device?.Dispose();
			_device = D2DDevice.FromHwnd(this.Handle);
			_device.Resize();
			_gfx = new D2DGraphics(_device);
		}

		private void RenderLoop()
		{
			while (!this.Disposing)
			{
				_gfx.BeginRender(D2DColor.White);

				
				if (_mouseClick)
				{
					testAnim.Reverse();
					_mouseClick = false;
				}

				testAnim.Step();

				testBirb.Position = testBirb.Position.Add(new D2DPoint(0.5f, 0f));

				testBirb.Render(_gfx);


				_gfx.EndRender();

				_loopTimer.Wait(_targetWaitTime, false);
			}
		}

		private void Form1_Click(object sender, EventArgs e)
		{
			//if (_renderThread.Status != TaskStatus.Running)
			//	_renderThread.Start();

			//_device.Resize();

			//testAnim.Reverse();
			Debug.WriteLine("Click!!!");
			_mouseClick = true;
		}
	}
}