using System;

namespace FrameWork.CommandSystem
{
	// Token: 0x0200106B RID: 4203
	public class CommandShowUIWithArgs : CommandShowUI, ICollectable<UIElement, ArgumentBox>, ICollectable
	{
		// Token: 0x0600BF15 RID: 48917 RVA: 0x005670F4 File Offset: 0x005652F4
		public void Reset(UIElement arg1, ArgumentBox arg2)
		{
			base.Reset(arg1);
			this.ArgBox = arg2;
		}

		// Token: 0x0600BF16 RID: 48918 RVA: 0x00567106 File Offset: 0x00565306
		public override void Reset()
		{
			this.Reset(null, null);
		}

		// Token: 0x0600BF17 RID: 48919 RVA: 0x00567114 File Offset: 0x00565314
		public override void Collect()
		{
			base.Collect();
			bool flag = this.ArgBox != null;
			if (flag)
			{
				EasyPool.Free<ArgumentBox>(this.ArgBox);
			}
			this.ArgBox = null;
		}

		// Token: 0x0600BF18 RID: 48920 RVA: 0x00567149 File Offset: 0x00565349
		protected override void DoShow()
		{
			this.Element.SetOnInitArgs(this.ArgBox);
			this.ArgBox = null;
			base.DoShow();
		}

		// Token: 0x0400926D RID: 37485
		protected ArgumentBox ArgBox;
	}
}
