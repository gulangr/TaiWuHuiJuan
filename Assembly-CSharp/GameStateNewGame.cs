using System;
using FrameWork;

// Token: 0x020000EB RID: 235
public class GameStateNewGame : GameStateBase
{
	// Token: 0x0600083F RID: 2111 RVA: 0x00037FF9 File Offset: 0x000361F9
	public GameStateNewGame(Enum state) : base(state)
	{
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x00038004 File Offset: 0x00036204
	public override void OnEnter(ArgumentBox argsBox)
	{
		base.OnEnter(argsBox);
		UIManager.Instance.ChangeToUI(UIElement.NewGame);
	}
}
