using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.Component
{
	// Token: 0x02000FEB RID: 4075
	public class LineRenderer2DMulti : LineRenderer2D
	{
		// Token: 0x0600BA11 RID: 47633 RVA: 0x0054BDF4 File Offset: 0x00549FF4
		public override void ModifyMesh(VertexHelper verts)
		{
			base.ModifyMesh(verts);
			bool flag = this.ExtraVertices != null;
			if (flag)
			{
				foreach (IList<Vector2> lineVertices in this.ExtraVertices)
				{
					bool flag2 = lineVertices != null;
					if (flag2)
					{
						base.GenerateLineMesh(lineVertices, verts);
					}
				}
			}
		}

		// Token: 0x04008FDB RID: 36827
		public IList<IList<Vector2>> ExtraVertices;
	}
}
