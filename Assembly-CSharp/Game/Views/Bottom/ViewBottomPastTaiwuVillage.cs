using System;
using FrameWork;

namespace Game.Views.Bottom
{
	// Token: 0x02000C4D RID: 3149
	public class ViewBottomPastTaiwuVillage : UIBase, IMainMenuButtonParent, IAsyncMethodRequestHandler
	{
		// Token: 0x0600A06B RID: 41067 RVA: 0x004AE3DF File Offset: 0x004AC5DF
		public override void OnInit(ArgumentBox argsBox)
		{
		}

		// Token: 0x0600A06C RID: 41068 RVA: 0x004AE3E4 File Offset: 0x004AC5E4
		private void Update()
		{
			bool flag = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				GEvent.OnEvent(EEvents.RequestAdvanceMonth, null);
			}
		}
	}
}
