using System;
using FrameWork;

// Token: 0x020000C0 RID: 192
public class UIElementStateReady : UIElementStateBase
{
	// Token: 0x06000696 RID: 1686 RVA: 0x0002E328 File Offset: 0x0002C528
	public UIElementStateReady(UIElementStateMachine machine) : base(EUiElementState.Ready, machine)
	{
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x0002E339 File Offset: 0x0002C539
	public override void OnEnter(ArgumentBox argsBox)
	{
		Action onShowed = this.StateMachine.Element.OnShowed;
		if (onShowed != null)
		{
			onShowed();
		}
		this.StateMachine.Element.OnShowed = null;
	}
}
