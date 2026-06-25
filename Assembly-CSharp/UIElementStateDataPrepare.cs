using System;
using FrameWork;

// Token: 0x020000BE RID: 190
public class UIElementStateDataPrepare : UIElementStateBase
{
	// Token: 0x06000691 RID: 1681 RVA: 0x0002E1F2 File Offset: 0x0002C3F2
	public UIElementStateDataPrepare(UIElementStateMachine machine) : base(EUiElementState.DataPrepare, machine)
	{
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x0002E204 File Offset: 0x0002C404
	public override void OnEnter(ArgumentBox argsBox)
	{
		this.StateMachine.Element.InitElement();
		this.StateMachine.Element.MonitorData();
		Action onListenerIdReady = this.StateMachine.Element.OnListenerIdReady;
		if (onListenerIdReady != null)
		{
			onListenerIdReady();
		}
		this.StateMachine.Element.OnListenerIdReady = null;
		bool flag = !this.StateMachine.Element.NeedWaitData();
		if (flag)
		{
			this.StateMachine.TranslateState(EUiElementState.AnimateIn, null);
		}
	}
}
