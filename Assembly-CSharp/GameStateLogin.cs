using System;
using FrameWork;
using FrameWork.UISystem.UI;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using UnityEngine;

// Token: 0x020000EA RID: 234
public class GameStateLogin : GameStateBase
{
	// Token: 0x0600083C RID: 2108 RVA: 0x00037E9C File Offset: 0x0003609C
	public GameStateLogin(Enum stateName) : base(stateName)
	{
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x00037EA8 File Offset: 0x000360A8
	public override void OnEnter(ArgumentBox argsBox)
	{
		base.OnEnter(argsBox);
		bool flag = argsBox != null;
		if (flag)
		{
			GlobalOperations.LeaveWorld();
			GlobalOperations.OnLeaveWorld();
			bool isBackToMainMenu;
			argsBox.Get("IsBack", out isBackToMainMenu);
			bool flag2 = isBackToMainMenu;
			if (flag2)
			{
				GameApp.AdvancingMonth = false;
				GEvent.ClearEvent(EEvents.OnActionPointChange);
				bool isTutorial;
				bool flag3 = argsBox.Get("Tutorial", out isTutorial) && isTutorial;
				if (flag3)
				{
					UIElement mainMenu = UIElement.MainMenu;
					mainMenu.OnShowed = (Action)Delegate.Combine(mainMenu.OnShowed, new Action(delegate()
					{
						UIManager.Instance.ShowUI(UIElement.TutorialChaptersMenu, true);
					}));
				}
			}
			bool flag4 = UI_GMWindow.Instance != null;
			if (flag4)
			{
				UI_GMWindow.Instance.OnLeaveWorld();
				Object.Destroy(UI_GMWindow.Instance.gameObject);
				UI_GMWindow.Instance = null;
			}
		}
		else
		{
			UIElement.PermanentTips.Show();
			TaiwuDomainMethod.Call.SetAutoAllocateNeiliToMax(SingletonObject.getInstance<GlobalSettings>().AutoAllocateNeiliToMax);
		}
		GameApp.ResetGameSubPageState();
		GMFunc.Reset();
		UIMaskManager instance = SingletonObject.getInstance<UIMaskManager>();
		if (instance != null)
		{
			instance.CleanupSharedMaskInstance();
		}
		UIManager.Instance.ChangeToUI(UIElement.MainMenu);
		SingletonObject.getInstance<TooltipManager>();
	}

	// Token: 0x0600083E RID: 2110 RVA: 0x00037FD3 File Offset: 0x000361D3
	public override void OnExit()
	{
		base.OnExit();
		UIElement.MainMenu.Destroy();
		UIElement.RecordSelect.Destroy();
		PoolManager.CleanPool();
	}
}
