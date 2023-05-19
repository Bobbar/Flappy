using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.D2DLib;

namespace Flappy.Renderables
{
	public class GifRenderer : Renderable
	{

		private Image _srcGif;
		private D2DBitmap[] _frames;
		private int _currentFrame = 0;
		private bool _done = true;

		public GifRenderer(D2DPoint position, Image src, D2DGraphics gfx) : base(position)
		{
			_srcGif = src;

			LoadFrames(src, gfx);
		}

		private void LoadFrames(Image src, D2DGraphics gfx)
		{
			var dim = new FrameDimension(src.FrameDimensionsList[0]);
			var nFrames = src.GetFrameCount(dim);

			_frames = new D2DBitmap[nFrames];

			for (int i = 0; i < nFrames; i++)
			{
				src.SelectActiveFrame(dim, i);

				using (var bmp = new Bitmap(src))
				{
					_frames[i] = gfx.Device.CreateBitmapFromGDIBitmap(bmp);
				}
			}
		}

		public override void Render(D2DGraphics gfx)
		{
			if (_done)
				return;

			gfx.PushTransform();

			gfx.DrawBitmap(_frames[_currentFrame], new D2DRect(Position, new D2DSize(_srcGif.Width, _srcGif.Height)));

			_currentFrame = (_currentFrame + 1) % _frames.Length;

			if (_currentFrame == 0)
				_done = true;

			gfx.PopTransform();
		}

		public void Restart()
		{
			_done = false;
			_currentFrame = 0;
		}

		public void Restart(D2DPoint position)
		{
			this.Position = position;
			_done = false;
			_currentFrame = 0;
		}
	}
}
