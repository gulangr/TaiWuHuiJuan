using System;
using FrameWork;

// Token: 0x020000C2 RID: 194
public class UIElementStateHiding : UIElementStateBase
{
	// Token: 0x0600069B RID: 1691 RVA: 0x0002E3F7 File Offset: 0x0002C5F7
	public UIElementStateHiding(UIElementStateMachine machine) : base(EUiElementState.Hiding, machine)
	{
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x0002E408 File Offset: 0x0002C608
	public override void OnEnter(ArgumentBox argsBox)
	{
		this.StateMachine.Element.SetElementReady(false);
		Action onHide = this.StateMachine.Element.OnHide;
		if (onHide != null)
		{
			onHide();
		}
		this.StateMachine.Element.OnHide = null;
		this.StateMachine.Element.ServeGroup = null;
		this.StateMachine.Element.UnMonitorData();
		this.StateMachine.Element.NotifyElementHide();
		this.StateMachine.Element.SetUIVisible(false);
	}
}
