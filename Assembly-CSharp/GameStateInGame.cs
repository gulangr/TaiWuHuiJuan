using System;
using FrameWork;
using Game.Views;

// Token: 0x020000ED RID: 237
public class GameStateInGame : GameStateBase
{
	// Token: 0x06000843 RID: 2115 RVA: 0x0003809F File Offset: 0x0003629F
	public GameStateInGame(Enum state) : base(state)
	{
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x000380AC File Offset: 0x000362AC
	public override void OnEnter(ArgumentBox argsBox)
	{
		base.OnEnter(argsBox);
		UIElement.Advance.Show();
		SingletonObject.getInstance<EventModel>();
		GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
		settings.SetHideTaiwuOriginalSurname(settings.HideTaiwuOriginalSurname);
		settings.AllowExecute = settings.AllowExecute;
		settings.SetArchiveFilesBackupInterval(settings.ArchiveFilesBackupInterval);
		settings.SetArchiveFilesBackupCount(settings.ArchiveFilesBackupCount);
		settings.SetImproveSaveSpeed(settings.ImproveSaveSpeed);
		settings.SaveSettings();
		SingletonObject.getInstance<CharacterMonitorModel>();
		SingletonObject.getInstance<PunishmentModel>();
		UI_GMWindow.EnsureInstanceExist(GameApp.Instance.EnableGMPanel);
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x0003813D File Offset: 0x0003633D
	private void ResetData()
	{
		UI_ProfessionSkillUnlocked.ClearProfessionSkillUnlockedQueue();
		ViewNewFunctionUnlock.ClearQueue();
	}

	// Token: 0x06000846 RID: 2118 RVA: 0x0003814C File Offset: 0x0003634C
	public override void OnExit()
	{
		base.OnExit();
		UIElement.Advance.Hide(false);
		this.ResetData();
	}
}
