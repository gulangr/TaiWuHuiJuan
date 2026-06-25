using System;
using FrameWork;

// Token: 0x020000BD RID: 189
public class UIElementStateReset : UIElementStateBase
{
	// Token: 0x0600068F RID: 1679 RVA: 0x0002E1A8 File Offset: 0x0002C3A8
	public UIElementStateReset(UIElementStateMachine machine) : base(EUiElementState.Reset, machine)
	{
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x0002E1B9 File Offset: 0x0002C3B9
	public override void OnEnter(ArgumentBox argsBox)
	{
		this.StateMachine.Element.ResetElement();
		this.StateMachine.Element.SetElementReady(false);
		this.StateMachine.TranslateState(EUiElementState.DataPrepare, null);
	}
}
