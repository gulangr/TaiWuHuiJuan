using System;
using FrameWork;

// Token: 0x0200003A RID: 58
public abstract class BaseState : IState
{
	// Token: 0x060001FE RID: 510
	public abstract void OnEnter(ArgumentBox argsBox);

	// Token: 0x060001FF RID: 511
	public abstract void OnExit();

	// Token: 0x06000200 RID: 512
	public abstract void OnUpdate(float deltaTime);
}
