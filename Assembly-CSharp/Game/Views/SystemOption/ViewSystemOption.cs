using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.GameDataBridge;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

namespace Game.Views.SystemOption
{
	// Token: 0x020009F4 RID: 2548
	public class ViewSystemOption : UIBase
	{
		// Token: 0x06007D72 RID: 32114 RVA: 0x003A47EC File Offset: 0x003A29EC
		private void Awake()
		{
			base.GetComponent<CanvasGroup>().alpha = 0f;
			this.btnReturnToGame.ClearAndAddListener(new Action(this.OnReturnToGame));
			this.btnSystemSetting.ClearAndAddListener(new Action(this.OnSystemSetting));
			this.btnManageMod.ClearAndAddListener(new Action(this.OnManageMod));
			this.btnBugFeedback.ClearAndAddListener(new Action(this.OnBugFeedback));
			this.btnGuidingChapter.ClearAndAddListener(new Action(this.OnGuidingChapter));
			this.btnReturnToMainMenu.ClearAndAddListener(new Action(this.OnReturnToMainMenu));
			this.btnExitGame.ClearAndAddListener(new Action(this.OnExitGame));
			this.btnManageDLC.ClearAndAddListener(new Action(this.OnManageDLC));
			this.btnUpdateLog.ClearAndAddListener(new Action(this.OnUpdatelog));
			this.btnDLCIntroduce.ClearAndAddListener(new Action(this.OnDLCIntroduce));
			this.btnEncyclopedia.ClearAndAddListener(new Action(this.OnEncyclopedia));
			this.btnAchievement.ClearAndAddListener(new Action(this.OnAchievement));
		}

		// Token: 0x06007D73 RID: 32115 RVA: 0x003A492C File Offset: 0x003A2B2C
		public override void OnInit(ArgumentBox argsBox)
		{
			this._audioLock = false;
			List<string> notices = null;
			if (argsBox != null)
			{
				argsBox.Get<List<string>>("Notices", out notices);
			}
			this.ShowNotices(notices);
			this.UpdateHotKeyDisplay();
		}

		// Token: 0x06007D74 RID: 32116 RVA: 0x003A4968 File Offset: 0x003A2B68
		private void OnSystemSetting()
		{
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.SetObject("Parent", this.Element);
			UIElement.SystemSetting.SetOnInitArgs(box);
			UIManager.Instance.ShowUI(UIElement.SystemSetting, true);
			this._audioLock = true;
		}

		// Token: 0x06007D75 RID: 32117 RVA: 0x003A49B2 File Offset: 0x003A2BB2
		private void OnManageDLC()
		{
			UIManager.Instance.MaskUI(UIElement.UpdateLog);
		}

		// Token: 0x06007D76 RID: 32118 RVA: 0x003A49C5 File Offset: 0x003A2BC5
		private void OnExitGame()
		{
			GameApp.GameQuitConfirm();
		}

		// Token: 0x06007D77 RID: 32119 RVA: 0x003A49D0 File Offset: 0x003A2BD0
		private void OnReturnToMainMenu()
		{
			Action confirmAction = delegate()
			{
				this._audioLock = true;
				this.QuickHide();
				GameApp.ReturnToMainMenu(null, null, null);
			};
			DialogCmd cmd = new DialogCmd
			{
				Title = LocalStringManager.Get(LanguageKey.LK_Return_To_Main_Menu),
				Content = LocalStringManager.Get(LanguageKey.LK_Return_To_Main_Menu_Confirm),
				Type = 1,
				Yes = confirmAction
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06007D78 RID: 32120 RVA: 0x003A4A4A File Offset: 0x003A2C4A
		private void OnBugFeedback()
		{
			Application.OpenURL("https://help.conchship.com.cn/");
		}

		// Token: 0x06007D79 RID: 32121 RVA: 0x003A4A58 File Offset: 0x003A2C58
		private void OnManageMod()
		{
			this.QuickHide();
			UIManager.Instance.ShowUI(UIElement.Mod, true);
		}

		// Token: 0x06007D7A RID: 32122 RVA: 0x003A4A73 File Offset: 0x003A2C73
		private void OnReturnToGame()
		{
			this._audioLock = true;
			this.QuickHide();
		}

		// Token: 0x06007D7B RID: 32123 RVA: 0x003A4A84 File Offset: 0x003A2C84
		private void OnUpdatelog()
		{
			UIManager.Instance.MaskUI(UIElement.UpdateLog);
		}

		// Token: 0x06007D7C RID: 32124 RVA: 0x003A4A97 File Offset: 0x003A2C97
		private void OnDLCIntroduce()
		{
			UIElement.DLCIntroduce.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("SelectedIndex", 0));
			UIManager.Instance.MaskUI(UIElement.DLCIntroduce);
		}

		// Token: 0x06007D7D RID: 32125 RVA: 0x003A4AC5 File Offset: 0x003A2CC5
		private void OnGuidingChapter()
		{
			this.QuickHide();
			UIManager.Instance.MaskUI(UIElement.TutorialGuidingChapter);
		}

		// Token: 0x06007D7E RID: 32126 RVA: 0x003A4ADF File Offset: 0x003A2CDF
		private void OnEncyclopedia()
		{
			this.QuickHide();
			UIManager.Instance.ShowUI(UIElement.Encyclopedia, true);
		}

		// Token: 0x06007D7F RID: 32127 RVA: 0x003A4AFA File Offset: 0x003A2CFA
		private void OnAchievement()
		{
			this.QuickHide();
			UIManager.Instance.MaskUI(UIElement.Achievement);
		}

		// Token: 0x06007D80 RID: 32128 RVA: 0x003A4B14 File Offset: 0x003A2D14
		private void ShowNotices(List<string> notices)
		{
			bool isShow = notices != null && notices.Count > 0;
			Refers commonHintMiddleLevelur = base.CGet<Refers>("CommonHintMiddleLevel");
			while (isShow && commonHintMiddleLevelur.transform.parent.childCount < notices.Count)
			{
				Object.Instantiate<Refers>(commonHintMiddleLevelur, commonHintMiddleLevelur.transform.parent);
			}
			for (int i = 0; i < commonHintMiddleLevelur.transform.parent.childCount; i++)
			{
				Refers notice = commonHintMiddleLevelur.transform.parent.GetChild(i).GetComponent<Refers>();
				notice.transform.gameObject.SetActive(isShow && i < notices.Count);
				bool flag = isShow && i < notices.Count;
				if (flag)
				{
					notice.CGet<TextMeshProUGUI>("Label").text = notices[i];
				}
			}
			base.CGet<CScrollbarStateHelper>("VerticalScrollbar").SetStatus((notices != null && notices.Count > 3) ? CScrollbarStateHelper.ShowStatus.ShowBar : CScrollbarStateHelper.ShowStatus.Hide);
		}

		// Token: 0x06007D81 RID: 32129 RVA: 0x003A4C24 File Offset: 0x003A2E24
		public override void QuickHide()
		{
			bool flag = !this._audioLock;
			if (flag)
			{
				AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
			}
			base.QuickHide();
		}

		// Token: 0x06007D82 RID: 32130 RVA: 0x003A4C5C File Offset: 0x003A2E5C
		private void OnEnable()
		{
			AudioManager.Instance.EnableMusicVolumeRate(0.2f);
			AudioManager.Instance.SetMusicVolumeWithFade(1f, 0f);
			bool flag = GlobalOperations.CurrGameWorldType == 1;
			if (flag)
			{
				BasicGameData basicGameData = SingletonObject.getInstance<BasicGameData>();
				this.btnGuidingChapter.gameObject.SetActive(basicGameData.HaveAnyTriggeredGuidingChapter());
			}
			else
			{
				this.btnGuidingChapter.gameObject.SetActive(false);
			}
		}

		// Token: 0x06007D83 RID: 32131 RVA: 0x003A4CD0 File Offset: 0x003A2ED0
		private void OnDisable()
		{
			AudioManager.Instance.DisableMusicVolumeRate();
			AudioManager.Instance.SetMusicVolumeWithFade(1f, 0f);
		}

		// Token: 0x06007D84 RID: 32132 RVA: 0x003A4CF4 File Offset: 0x003A2EF4
		private void Update()
		{
			bool needCheckPackers = this._needCheckPackers;
			if (needCheckPackers)
			{
				bool loadFinish = true;
				foreach (SpriteAtlas item in this.RelativeAtlases)
				{
					bool flag = AtlasInfo.Instance.GetLoadedPacker(item.name) == null;
					if (flag)
					{
						loadFinish = false;
					}
				}
				bool flag2 = loadFinish;
				if (flag2)
				{
					this._needCheckPackers = false;
					base.GetComponent<CanvasGroup>().alpha = 1f;
				}
			}
		}

		// Token: 0x06007D85 RID: 32133 RVA: 0x003A4D6F File Offset: 0x003A2F6F
		public void UpdateHotKeyDisplay()
		{
			this.globalHideTipsDisplay.Refresh((!SingletonObject.getInstance<GlobalSettings>().GlobalTipsHide) ? EHotKeyDisplayType.SystemOptionGlobalTipsHide : EHotKeyDisplayType.SystemOptionGlobalTipsShow);
		}

		// Token: 0x04005F78 RID: 24440
		private bool _audioLock;

		// Token: 0x04005F79 RID: 24441
		private bool _needCheckPackers = true;

		// Token: 0x04005F7A RID: 24442
		[SerializeField]
		private CanvasGroup updateLogCanvasGroup;

		// Token: 0x04005F7B RID: 24443
		[SerializeField]
		private CButton btnReturnToGame;

		// Token: 0x04005F7C RID: 24444
		[SerializeField]
		private CButton btnSystemSetting;

		// Token: 0x04005F7D RID: 24445
		[SerializeField]
		private CButton btnManageMod;

		// Token: 0x04005F7E RID: 24446
		[SerializeField]
		private CButton btnBugFeedback;

		// Token: 0x04005F7F RID: 24447
		[SerializeField]
		private CButton btnGuidingChapter;

		// Token: 0x04005F80 RID: 24448
		[SerializeField]
		private CButton btnReturnToMainMenu;

		// Token: 0x04005F81 RID: 24449
		[SerializeField]
		private CButton btnExitGame;

		// Token: 0x04005F82 RID: 24450
		[SerializeField]
		private CButton btnManageDLC;

		// Token: 0x04005F83 RID: 24451
		[SerializeField]
		private CButton btnUpdateLog;

		// Token: 0x04005F84 RID: 24452
		[SerializeField]
		private CButton btnDLCIntroduce;

		// Token: 0x04005F85 RID: 24453
		[SerializeField]
		private CButton btnEncyclopedia;

		// Token: 0x04005F86 RID: 24454
		[SerializeField]
		private CButton btnAchievement;

		// Token: 0x04005F87 RID: 24455
		[SerializeField]
		private Game.Components.Common.HotkeyDisplay globalHideTipsDisplay;
	}
}
