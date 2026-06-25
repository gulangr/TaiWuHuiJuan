using System;
using FrameWork;

// Token: 0x020000BF RID: 191
public class UIElementStateAnimateIn : UIElementStateBase
{
	// Token: 0x06000693 RID: 1683 RVA: 0x0002E28B File Offset: 0x0002C48B
	public UIElementStateAnimateIn(UIElementStateMachine machine) : base(EUiElementState.AnimateIn, machine)
	{
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x0002E29C File Offset: 0x0002C49C
	public override void OnEnter(ArgumentBox argsBox)
	{
		this.StateMachine.Element.NotifyElementShow();
		this.StateMachine.Element.PlayShowAnimation(new Action(this.OnAnimationComplete), true);
		this.StateMachine.Element.UiBase.PlayAudioIn();
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x0002E2EF File Offset: 0x0002C4EF
	private void OnAnimationComplete()
	{
		this.StateMachine.Element.SetElementReady(true);
		this.StateMachine.Element.NotifyElementShowFinish();
		this.StateMachine.TranslateState(EUiElementState.Ready, null);
	}
}
