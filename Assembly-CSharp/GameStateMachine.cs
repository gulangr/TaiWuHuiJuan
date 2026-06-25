using System;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;

// Token: 0x020000E9 RID: 233
public class GameStateMachine : StateMachine
{
	// Token: 0x06000839 RID: 2105 RVA: 0x00037DF4 File Offset: 0x00035FF4
	protected override void Init()
	{
		base.Init();
		base.RegisterState(new GameStateLogin(EGameState.Login));
		base.RegisterState(new GameStateLoading(EGameState.Loading));
		base.RegisterState(new GameStateInGame(EGameState.InGame));
		base.RegisterState(new GameStateNewGame(EGameState.NewGame));
		SharedConstValue.InitializeCharId2AnimalIdCache();
		PlayerCastBossSkills.Initialize();
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x00037E60 File Offset: 0x00036060
	protected override void OnStateChanged(BaseState lastState, BaseState currentState)
	{
		base.OnStateChanged(lastState, currentState);
		bool flag = lastState is GameStateLogin;
		if (!flag)
		{
			GEvent.OnEvent(UiEvents.UnloadPackers, null);
		}
	}
}
