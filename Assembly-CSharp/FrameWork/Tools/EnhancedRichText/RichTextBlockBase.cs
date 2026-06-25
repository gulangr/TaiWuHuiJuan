using System;
using UnityEngine;

namespace FrameWork.Tools.EnhancedRichText
{
	// Token: 0x0200103D RID: 4157
	public abstract class RichTextBlockBase
	{
		// Token: 0x0600BDC8 RID: 48584 RVA: 0x00562D28 File Offset: 0x00560F28
		protected RichTextBlockBase(int width, int height)
		{
			this.Width = width;
			this.Height = height;
		}

		// Token: 0x17001563 RID: 5475
		// (get) Token: 0x0600BDCA RID: 48586 RVA: 0x00562D49 File Offset: 0x00560F49
		// (set) Token: 0x0600BDC9 RID: 48585 RVA: 0x00562D40 File Offset: 0x00560F40
		public IRootBlock Parent { get; protected internal set; }

		// Token: 0x0600BDCB RID: 48587
		public abstract void Draw(Transform parent, Func<string, ArgumentBox, TipType> linkTipActivate);

		// Token: 0x04009203 RID: 37379
		public int Width;

		// Token: 0x04009204 RID: 37380
		public int Height;
	}
}
