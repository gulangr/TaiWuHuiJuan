using System;
using FrameWork;

// Token: 0x0200036F RID: 879
public class UI_BlockInteract : UIBase
{
	// Token: 0x060032FB RID: 13051 RVA: 0x00191B70 File Offset: 0x0018FD70
	public override void OnInit(ArgumentBox argsBox)
	{
		CommandKitBase.SetDisable(true);
		UIElement element = this.Element;
		element.OnDeActive = (Action)Delegate.Combine(element.OnDeActive, new Action(delegate()
		{
			CommandKitBase.SetDisable(false);
		}));
	}
}
