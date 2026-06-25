using System;

// Token: 0x020000BA RID: 186
public abstract class UIElementStateBase : State
{
	// Token: 0x0600068A RID: 1674 RVA: 0x0002E141 File Offset: 0x0002C341
	protected UIElementStateBase(Enum state, UIElementStateMachine machine) : base(state)
	{
		this.StateMachine = machine;
	}

	// Token: 0x04000736 RID: 1846
	protected UIElementStateMachine StateMachine;
}
