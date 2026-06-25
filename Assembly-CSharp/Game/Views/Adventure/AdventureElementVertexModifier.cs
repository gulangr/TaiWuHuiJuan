using System;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C65 RID: 3173
	public class AdventureElementVertexModifier : AdventureVertexModifier
	{
		// Token: 0x0600A195 RID: 41365 RVA: 0x004B8490 File Offset: 0x004B6690
		public void SetOutline(bool enabled)
		{
			bool flag = this._outlineEnabled == enabled;
			if (!flag)
			{
				this._outlineEnabled = enabled;
				bool flag2 = base.graphic != null;
				if (flag2)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}

		// Token: 0x0600A196 RID: 41366 RVA: 0x004B84D0 File Offset: 0x004B66D0
		protected override void ModifyVertex(ref UIVertex vertex, Rect rect, int index)
		{
			vertex.uv3 = new Vector4(0f, 0f, this._outlineEnabled ? 1f : 0f, 0f);
		}

		// Token: 0x04007D70 RID: 32112
		private bool _outlineEnabled;
	}
}
