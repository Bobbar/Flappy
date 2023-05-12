using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unvell.D2DLib;

namespace Flappy
{
	//internal interface Renderable
	//{
	//	D2DPoint Position { get; set; }
	//	float Rotation { get; set; }

	//	void Render(D2DGraphics gfx);
	//}

	public abstract class Renderable
	{
		public D2DPoint Position { get; set; }

		public float Rotation { get; set; }

		public abstract void Render(D2DGraphics gfx);

	}
}
