using System;
using FrameWork;

// Token: 0x020000BB RID: 187
public class UIElementStateSleep : UIElementStateBase
{
	// Token: 0x0600068B RID: 1675 RVA: 0x0002E153 File Offset: 0x0002C353
	public UIElementStateSleep(UIElementStateMachine machine) : base(EUiElementState.Sleep, machine)
	{
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x0002E164 File Offset: 0x0002C364
	public override void OnEnter(ArgumentBox argsBox)
	{
		base.OnEnter(argsBox);
		this.StateMachine.Element.DestroyUIBase();
	}
}
