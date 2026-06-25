using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Tools.EnhancedRichText
{
	// Token: 0x0200103B RID: 4155
	public class ExtraRootTag<T> : RichTextBlockBase, IRootBlock where T : RichTextBlockBase
	{
		// Token: 0x0600BDC4 RID: 48580 RVA: 0x00562C4F File Offset: 0x00560E4F
		public ExtraRootTag(int width, int height) : base(width, height)
		{
		}

		// Token: 0x0600BDC5 RID: 48581 RVA: 0x00562C68 File Offset: 0x00560E68
		public override void Draw(Transform parent, Func<string, ArgumentBox, TipType> linkTipActivate)
		{
			foreach (T block in this.ChildBlocks)
			{
				block.Draw(parent, linkTipActivate);
			}
		}

		// Token: 0x0600BDC6 RID: 48582 RVA: 0x00562CC8 File Offset: 0x00560EC8
		public void AddChildBlock(RichTextBlockBase blockBase)
		{
			T element = blockBase as T;
			bool flag = element == null;
			if (flag)
			{
				throw new FormatException(string.Format("ExtraRootTag: Trying to add a {0} block in a {1}", blockBase.GetType(), base.GetType()));
			}
			this.ChildBlocks.Add(element);
			element.Parent = this;
		}

		// Token: 0x04009202 RID: 37378
		public readonly List<T> ChildBlocks = new List<T>();
	}
}
