using System;

namespace FrameWork.CommandSystem
{
	// Token: 0x0200106A RID: 4202
	public class CommandShowUI : BaseCommand, ICollectable<UIElement>, ICollectable
	{
		// Token: 0x0600BF0D RID: 48909 RVA: 0x00567040 File Offset: 0x00565240
		public override bool Execute()
		{
			bool flag = this.Element == null || this.Element.Exist;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				UIElement element = this.Element;
				element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(delegate()
				{
					this._isOpen = true;
				}));
				this.DoShow();
				result = true;
			}
			return result;
		}

		// Token: 0x1700157D RID: 5501
		// (get) Token: 0x0600BF0E RID: 48910 RVA: 0x0056709F File Offset: 0x0056529F
		public override bool Done
		{
			get
			{
				return this._isOpen;
			}
		}

		// Token: 0x0600BF0F RID: 48911 RVA: 0x005670A7 File Offset: 0x005652A7
		public override void Reset()
		{
			this.Reset(null);
		}

		// Token: 0x0600BF10 RID: 48912 RVA: 0x005670B2 File Offset: 0x005652B2
		public void Reset(UIElement param)
		{
			this._isOpen = false;
			this.Element = param;
		}

		// Token: 0x0600BF11 RID: 48913 RVA: 0x005670C3 File Offset: 0x005652C3
		public override void Collect()
		{
			this.Element = null;
		}

		// Token: 0x0600BF12 RID: 48914 RVA: 0x005670CD File Offset: 0x005652CD
		protected virtual void DoShow()
		{
			UIManager.Instance.ShowUI(this.Element, true);
		}

		// Token: 0x0400926B RID: 37483
		protected UIElement Element;

		// Token: 0x0400926C RID: 37484
		private bool _isOpen;
	}
}
