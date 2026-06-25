using System;
using FrameWork;

// Token: 0x020000C1 RID: 193
public class UIElementStateAnimateOut : UIElementStateBase
{
	// Token: 0x06000698 RID: 1688 RVA: 0x0002E369 File Offset: 0x0002C569
	public UIElementStateAnimateOut(UIElementStateMachine machine) : base(EUiElementState.AnimateOut, machine)
	{
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x0002E37C File Offset: 0x0002C57C
	public override void OnEnter(ArgumentBox argsBox)
	{
		this.StateMachine.Element.SetElementReady(false);
		this.StateMachine.Element.NotifyElementHideStart();
		this.StateMachine.Element.PlayHideAnimation(new Action(this.OnHideAnimationComplete), true);
		this.StateMachine.Element.UiBase.PlayAudioOut();
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x0002E3E1 File Offset: 0x0002C5E1
	private void OnHideAnimationComplete()
	{
		this.StateMachine.TranslateState(EUiElementState.Hiding, null);
	}
}
