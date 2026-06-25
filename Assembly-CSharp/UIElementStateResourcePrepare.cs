using System;
using FrameWork;

// Token: 0x020000BC RID: 188
public class UIElementStateResourcePrepare : UIElementStateBase
{
	// Token: 0x0600068D RID: 1677 RVA: 0x0002E180 File Offset: 0x0002C380
	public UIElementStateResourcePrepare(UIElementStateMachine machine) : base(EUiElementState.ResourcePrepare, machine)
	{
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x0002E191 File Offset: 0x0002C391
	public override void OnEnter(ArgumentBox argsBox)
	{
		this.StateMachine.Element.PrepareRes(true, null, false);
	}
}
