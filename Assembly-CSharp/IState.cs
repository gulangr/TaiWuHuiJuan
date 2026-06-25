using System;
using FrameWork;

// Token: 0x02000039 RID: 57
public interface IState
{
	// Token: 0x060001FB RID: 507
	void OnEnter(ArgumentBox argsBox);

	// Token: 0x060001FC RID: 508
	void OnExit();

	// Token: 0x060001FD RID: 509
	void OnUpdate(float deltaTime);
}
