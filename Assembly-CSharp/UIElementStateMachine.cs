using System;

// Token: 0x020000B9 RID: 185
public class UIElementStateMachine : StateMachine
{
	// Token: 0x06000687 RID: 1671 RVA: 0x0002E080 File Offset: 0x0002C280
	protected override void Init()
	{
		base.Init();
		base.RegisterState(new UIElementStateSleep(this));
		base.RegisterState(new UIElementStateResourcePrepare(this));
		base.RegisterState(new UIElementStateReset(this));
		base.RegisterState(new UIElementStateDataPrepare(this));
		base.RegisterState(new UIElementStateAnimateIn(this));
		base.RegisterState(new UIElementStateReady(this));
		base.RegisterState(new UIElementStateAnimateOut(this));
		base.RegisterState(new UIElementStateHiding(this));
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x0002E100 File Offset: 0x0002C300
	public bool IsInState(EUiElementState checkState)
	{
		UIElementStateBase state = base.GetCurrentState() as UIElementStateBase;
		bool flag = state == null;
		return !flag && (EUiElementState)state.stateName == checkState;
	}

	// Token: 0x04000735 RID: 1845
	public UIElement Element;
}
