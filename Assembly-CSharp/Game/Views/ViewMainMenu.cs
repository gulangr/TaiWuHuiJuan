using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using EventEditor;
using FrameWork;
using FrameWork.UI.LanguageRule;
using FrameWork.UISystem.UIElements;
using Game.Views.DLCIntroduce;
using GameData.Domains.Global;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace Game.Views
{
	// Token: 0x02000708 RID: 1800
	public class ViewMainMenu : UIBase
	{
		// Token: 0x060054FB RID: 21755 RVA: 0x00276A54 File Offset: 0x00274C54
		public override void OnInit(ArgumentBox argsBox)
		{
			this._settingData = SingletonObject.getInstance<GlobalSettings>();
			this.buttonEventEdit.interactable = UI_EventEditor.IsDev;
			this.buttonAvatarPreset.interactable = UI_EventEditor.IsDev;
			this.version.text = GameApp.Instance.GameVersion;
			this.OnLanguageChanged(null);
			this.dLCIntroduceHelper.Set(-1, true);
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				GlobalDomainMethod.Call.GetGlobalFlag(this.Element.GameDataListenerId, 0);
			}));
		}

		// Token: 0x060054FC RID: 21756 RVA: 0x00276AE9 File Offset: 0x00274CE9
		private void Start()
		{
			GEvent.Add(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChanged));
		}

		// Token: 0x060054FD RID: 21757 RVA: 0x00276B08 File Offset: 0x00274D08
		private void FixedUpdate()
		{
			bool flag = this.videoPlayer && this.videoPlayer.frame >= (long)(this.videoPlayer.frameCount - 1UL);
			if (flag)
			{
				this.ResetBackGround();
			}
		}

		// Token: 0x060054FE RID: 21758 RVA: 0x00276B54 File Offset: 0x00274D54
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.OnLanguageChanged));
			bool flag = this._videoTexture;
			if (flag)
			{
				Object.Destroy(this._videoTexture);
			}
		}

		// Token: 0x060054FF RID: 21759 RVA: 0x00276B99 File Offset: 0x00274D99
		private void OnEnable()
		{
			this.UpdateRoleListButtonInteractable();
			this.ResetBackGround();
		}

		// Token: 0x06005500 RID: 21760 RVA: 0x00276BAC File Offset: 0x00274DAC
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				bool flag = notification.Type == 1;
				if (flag)
				{
					Notification notification2 = notification;
					Notification notification3 = notification2;
					ushort domainId = notification3.DomainId;
					if (domainId == 0)
					{
						ushort methodId = notification3.MethodId;
						if (methodId == 12)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._pastEnding);
							this.PlayMusic();
							this.Element.ShowAfterRefresh();
						}
					}
				}
			}
		}

		// Token: 0x06005501 RID: 21761 RVA: 0x00276C60 File Offset: 0x00274E60
		private void OnLanguageChanged(ArgumentBox args = null)
		{
			this.RefreshLanguageDropdown();
			foreach (LanguageRuleImagePattern rulePattern in base.GetComponentsInChildren<LanguageRuleImagePattern>(true))
			{
				rulePattern.OnLanguageChange(LocalStringManager.CurLanguageType);
			}
			this.dLCIntroduceHelper.Set(-1, true);
			ViewMainMenu.<OnLanguageChanged>g__SetupButton|28_13(this.buttonRoleList, new string[]
			{
				LanguageKey.UI_MainMenu_Button_RoleList_2.Tr(),
				LanguageKey.UI_MainMenu_Button_RoleList.Tr()
			}, delegate
			{
				UIManager.Instance.ShowUI(UIElement.FullScreenMask, true);
				UIElement recordSelect = UIElement.RecordSelect;
				recordSelect.OnShowed = (Action)Delegate.Combine(recordSelect.OnShowed, new Action(delegate()
				{
					UIManager.Instance.HideUI(UIElement.FullScreenMask);
				}));
				UIManager.Instance.ShowUI(UIElement.RecordSelect, true);
			});
			ViewMainMenu.<OnLanguageChanged>g__SetupButton|28_13(this.buttonTeaching, new string[]
			{
				LanguageKey.UI_MainMenu_Button_Tutorial_2.Tr(),
				LanguageKey.UI_MainMenu_Button_Tutorial.Tr()
			}, delegate
			{
				GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
				bool flag = settings.HaveDoneSave || settings.HaveShowFirstTime;
				if (flag)
				{
					UIManager.Instance.ShowUI(UIElement.TutorialChaptersMenu, true);
				}
				else
				{
					UIManager.Instance.ShowUI(UIElement.FirstTime, true);
				}
			});
			ViewMainMenu.<OnLanguageChanged>g__SetupButton|28_13(this.buttonSettings, new string[]
			{
				LanguageKey.LK_SystemSetting.Tr()
			}, delegate
			{
				UIElement.SystemSetting.SetOnInitArgs(null);
				UIManager.Instance.ShowUI(UIElement.SystemSetting, true);
			});
			ViewMainMenu.<OnLanguageChanged>g__SetupButton|28_13(this.buttonEncyclopedia, new string[]
			{
				LanguageKey.UI_MainMenu_Button_Encyclopedia.Tr()
			}, delegate
			{
				UIManager.Instance.ShowUI(UIElement.Encyclopedia, true);
			});
			ViewMainMenu.<OnLanguageChanged>g__SetupButton|28_13(this.buttonModManage, new string[]
			{
				LanguageKey.UI_MainMenu_Button_ModManage.Tr()
			}, delegate
			{
				bool key = Input.GetKey(KeyCode.LeftControl);
				if (key)
				{
					UIManager.Instance.ShowUI(UIElement.ModPanelOld, true);
				}
				else
				{
					UIManager.Instance.ShowUI(UIElement.Mod, true);
				}
			});
			ViewMainMenu.<OnLanguageChanged>g__SetupButton|28_13(this.buttonEventEdit, new string[]
			{
				LanguageKey.UI_MainMenu_Button_EventEdit.Tr()
			}, delegate
			{
				UIManager.Instance.ChangeToUI(UIElement.EventEditor);
			});
			this.buttonEventEdit.interactable = UI_EventEditor.IsDev;
			ViewMainMenu.<OnLanguageChanged>g__SetupButton|28_13(this.buttonExit, new string[]
			{
				LanguageKey.UI_MainMenu_Button_Exit.Tr()
			}, delegate
			{
				GameApp.GameQuitConfirm();
			});
			ViewMainMenu.<OnLanguageChanged>g__SetupButton|28_13(this.buttonAvatarPreset, new string[]
			{
				LanguageKey.UI_MainMenu_Button_AvatarPreset.Tr()
			}, delegate
			{
				UIManager.Instance.ShowUI(UIElement.AvatarPreset, true);
			});
			this.buttonAvatarPreset.interactable = false;
			ViewMainMenu.<OnLanguageChanged>g__SetupButton|28_13(this.buttonUpdateLog, new string[]
			{
				LanguageKey.UI_MainMenu_Button_VersionList.Tr()
			}, delegate
			{
				UIManager.Instance.MaskUI(UIElement.UpdateLog);
			});
			ViewMainMenu.<OnLanguageChanged>g__SetupButton|28_13(this.buttonVisitConchship, new string[]
			{
				LanguageKey.UI_MainMenu_Button_VisitConchShip.Tr()
			}, delegate
			{
				Application.OpenURL("https://www.conchship.com.cn/");
			});
			ViewMainMenu.<OnLanguageChanged>g__SetupButton|28_13(this.buttonAchievement, new string[]
			{
				LanguageKey.UI_MainMenu_Button_TaiwuAchievement.Tr()
			}, delegate
			{
				UIManager.Instance.MaskUI(UIElement.Achievement);
			});
			ViewMainMenu.<OnLanguageChanged>g__SetupButton|28_13(this.buttonThankYouList, new string[]
			{
				LanguageKey.UI_MainMenu_Button_ThankYouList.Tr()
			}, delegate
			{
			});
			this.buttonThankYouList.interactable = false;
			ViewMainMenu.<OnLanguageChanged>g__SetupButton|28_13(this.buttonCreatorInfo, new string[]
			{
				LanguageKey.UI_MainMenu_Button_CreatorInfo.Tr()
			}, delegate
			{
				UIManager.Instance.ShowUI(UIElement.DevelopmentTeam, true);
			});
			this.buttonCreatorInfo.interactable = false;
		}

		// Token: 0x06005502 RID: 21762 RVA: 0x00277028 File Offset: 0x00275228
		private void ResetBackGround()
		{
			VideoClip clip = this.videoPlayer.clip;
			bool flag = !this._videoTexture;
			if (flag)
			{
				this._videoTexture = new RenderTexture((int)clip.width, (int)clip.height, 24);
			}
			bool flag2 = this._videoPlayerBackBuffer == null;
			if (flag2)
			{
				GameObject videoObject = Object.Instantiate<GameObject>(this.videoPlayer.gameObject, this.videoPlayer.transform.parent);
				this._videoPlayerBackBuffer = videoObject.GetComponent<VideoPlayer>();
			}
			else
			{
				VideoPlayer videoPlayerBackBuffer = this._videoPlayerBackBuffer;
				VideoPlayer videoPlayerBackBuffer2 = this.videoPlayer;
				this.videoPlayer = videoPlayerBackBuffer;
				this._videoPlayerBackBuffer = videoPlayerBackBuffer2;
				this._videoPlayerBackBuffer.Stop();
			}
			this.videoPlayer.isLooping = false;
			this.videoPlayer.Stop();
			this.videoPlayer.frame = 0L;
			this.videoPlayer.started -= this.<ResetBackGround>g__OnVideoPlayStart|29_1;
			this.videoPlayer.started += this.<ResetBackGround>g__OnVideoPlayStart|29_1;
			this.videoPlayer.errorReceived -= this.<ResetBackGround>g__OnVideoPlayError|29_0;
			this.videoPlayer.errorReceived += this.<ResetBackGround>g__OnVideoPlayError|29_0;
			this.videoPlayer.targetTexture = this._videoTexture;
			this.videoPlayer.Play();
			this.videoPlayerTarget.enabled = false;
			this.videoPlayerTarget.texture = this._videoTexture;
		}

		// Token: 0x06005503 RID: 21763 RVA: 0x002771A4 File Offset: 0x002753A4
		public void UpdateRoleListButtonInteractable()
		{
			this.buttonRoleList.interactable = (this._settingData.SkipTutorialChapters || this._settingData.CompletedChapters > 7);
			Array.ForEach<MonoJoint>(this.buttonRoleList.GetComponentsInChildren<MonoJoint>(true), delegate(MonoJoint e)
			{
				e.JointSync();
			});
		}

		// Token: 0x06005504 RID: 21764 RVA: 0x00277210 File Offset: 0x00275410
		private void PlayMusic()
		{
			AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 1f, 100);
			string audioName = this._pastEnding ? "Title_TianWeiZhiXia" : AudioManager.MainMenuBgmRandomPool.GetRandom<string>();
			AudioManager.Instance.PlayMusic(audioName, 1f, 100, null);
		}

		// Token: 0x06005505 RID: 21765 RVA: 0x00277264 File Offset: 0x00275464
		private void RefreshLanguageDropdown()
		{
			this.languageDropdown.ClearOptions();
			List<string> languages = LocalStringManager.GetAvailableLanguages().ToList<string>();
			this.languageDropdown.AddOptions(languages.Select(new Func<string, string>(LocalStringManager.GetLanguageName)).ToList<string>());
			int index = languages.FindIndex((string v) => v == LocalStringManager.CurLanguageKey);
			bool flag = index < 0;
			if (flag)
			{
				index = languages.FindIndex((string v) => v == "EN");
			}
			bool flag2 = index < 0;
			if (flag2)
			{
				index = 0;
			}
			this.languageDropdown.SetValueWithoutNotify(index);
			this.languageDropdown.onValueChanged.RemoveAllListeners();
			this.languageDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnLanguageDropdownValueChanged));
		}

		// Token: 0x06005506 RID: 21766 RVA: 0x00277345 File Offset: 0x00275545
		private void OnLanguageDropdownValueChanged(int value)
		{
			this.OnLanguageChange(value, this.languageDropdown.value);
		}

		// Token: 0x06005507 RID: 21767 RVA: 0x0027735C File Offset: 0x0027555C
		private void OnLanguageChange(int newIndex, int oldIndex)
		{
			ViewMainMenu.<>c__DisplayClass34_0 CS$<>8__locals1 = new ViewMainMenu.<>c__DisplayClass34_0();
			CS$<>8__locals1.<>4__this = this;
			string[] languages = LocalStringManager.GetAvailableLanguages().ToArray<string>();
			CS$<>8__locals1.global = SingletonObject.getInstance<GlobalSettings>();
			CS$<>8__locals1.original = oldIndex;
			CS$<>8__locals1.target = languages[newIndex];
			CS$<>8__locals1.mainMenuElement = UIElement.MainMenu;
			bool isShowing = CS$<>8__locals1.mainMenuElement.IsShowing;
			if (isShowing)
			{
				string globalLanguage = CS$<>8__locals1.global.Language;
				CS$<>8__locals1.<OnLanguageChange>g__ConfirmLanguageChange|1(CS$<>8__locals1.target);
				bool flag = globalLanguage != CS$<>8__locals1.target;
				if (flag)
				{
					Array.ForEach<LanguageRuleTips>(this.languageDropdown.transform.GetComponentsInChildren<LanguageRuleTips>(), delegate(LanguageRuleTips v)
					{
						v.OnLanguageChange(LocalStringManager.CurLanguageType);
					});
				}
			}
			else
			{
				bool flag2 = CS$<>8__locals1.global.Language != CS$<>8__locals1.target;
				if (flag2)
				{
					DialogCmd cmd = new DialogCmd
					{
						Title = LanguageKey.LK_SystemSetting_LocalizationSetting_Language_Reset_Title.Tr(),
						Content = ViewMainMenu.<OnLanguageChange>g__MultiText|34_0(LanguageKey.LK_SystemSetting_LocalizationSetting_Language_Reset_Text, CS$<>8__locals1.target, true),
						GroupYesText = ViewMainMenu.<OnLanguageChange>g__MultiText|34_0(LanguageKey.LK_Confirm, CS$<>8__locals1.target, false),
						GroupNoText = ViewMainMenu.<OnLanguageChange>g__MultiText|34_0(LanguageKey.LK_Cancel, CS$<>8__locals1.target, false),
						DialogType = EDialogType.None,
						Yes = delegate()
						{
							UIElement mainMenuElement = CS$<>8__locals1.mainMenuElement;
							Delegate onShowed = mainMenuElement.OnShowed;
							Action b;
							if ((b = CS$<>8__locals1.<>9__5) == null)
							{
								b = (CS$<>8__locals1.<>9__5 = delegate()
								{
									base.<OnLanguageChange>g__ConfirmLanguageChange|1(CS$<>8__locals1.target);
								});
							}
							mainMenuElement.OnShowed = (Action)Delegate.Combine(onShowed, b);
							GameApp.ReturnToMainMenu(null, null, null);
						},
						No = delegate()
						{
							CS$<>8__locals1.<>4__this.languageDropdown.value = CS$<>8__locals1.original;
						}
					};
					CommonUtils.ShowDialog(cmd);
				}
			}
		}

		// Token: 0x0600550A RID: 21770 RVA: 0x002774F0 File Offset: 0x002756F0
		[CompilerGenerated]
		internal static void <OnLanguageChanged>g__SetupButton|28_13(CButton button, IReadOnlyList<string> texts, Action action)
		{
			button.onClick.ResetListener(action);
			TextMeshProUGUI[] labels = button.GetComponentsInChildren<TextMeshProUGUI>();
			for (int i = 0; i < labels.Length; i++)
			{
				labels[i].text = texts[i];
			}
		}

		// Token: 0x0600550B RID: 21771 RVA: 0x00277536 File Offset: 0x00275736
		[CompilerGenerated]
		private void <ResetBackGround>g__OnVideoPlayError|29_0(VideoPlayer source, string message)
		{
			this.videoPlayerTarget.enabled = false;
		}

		// Token: 0x0600550C RID: 21772 RVA: 0x00277546 File Offset: 0x00275746
		[CompilerGenerated]
		private void <ResetBackGround>g__OnVideoPlayStart|29_1(VideoPlayer source)
		{
			this.videoPlayerTarget.enabled = true;
		}

		// Token: 0x0600550D RID: 21773 RVA: 0x00277558 File Offset: 0x00275758
		[CompilerGenerated]
		internal static string <OnLanguageChange>g__MultiText|34_0(LanguageKey key, string language, bool styleNewLine)
		{
			string text = key.Tr();
			string ext = LocalStringManager.GetCrossLanguage(key, language);
			if (styleNewLine)
			{
				text = text + "\n" + ext;
			}
			else
			{
				text = text + "(" + ext + ")";
			}
			return text;
		}

		// Token: 0x040039DD RID: 14813
		[SerializeField]
		private CButton buttonRoleList;

		// Token: 0x040039DE RID: 14814
		[SerializeField]
		private CButton buttonTeaching;

		// Token: 0x040039DF RID: 14815
		[SerializeField]
		private CButton buttonEventEdit;

		// Token: 0x040039E0 RID: 14816
		[SerializeField]
		private CButton buttonAvatarPreset;

		// Token: 0x040039E1 RID: 14817
		[SerializeField]
		private CButton buttonVisitConchship;

		// Token: 0x040039E2 RID: 14818
		[SerializeField]
		private CButton buttonSettings;

		// Token: 0x040039E3 RID: 14819
		[SerializeField]
		private CButton buttonExit;

		// Token: 0x040039E4 RID: 14820
		[SerializeField]
		private CButton buttonModManage;

		// Token: 0x040039E5 RID: 14821
		[SerializeField]
		private CButton buttonEncyclopedia;

		// Token: 0x040039E6 RID: 14822
		[SerializeField]
		private CButton buttonUpdateLog;

		// Token: 0x040039E7 RID: 14823
		[SerializeField]
		private CButton buttonAchievement;

		// Token: 0x040039E8 RID: 14824
		[SerializeField]
		private CButton buttonThankYouList;

		// Token: 0x040039E9 RID: 14825
		[SerializeField]
		private CButton buttonCreatorInfo;

		// Token: 0x040039EA RID: 14826
		[SerializeField]
		private CDropdown languageDropdown;

		// Token: 0x040039EB RID: 14827
		[SerializeField]
		private VideoPlayer videoPlayer;

		// Token: 0x040039EC RID: 14828
		[SerializeField]
		private CRawImage videoPlayerTarget;

		// Token: 0x040039ED RID: 14829
		[SerializeField]
		private TextMeshProUGUI version;

		// Token: 0x040039EE RID: 14830
		[SerializeField]
		private DLCIntroduceHelper dLCIntroduceHelper;

		// Token: 0x040039EF RID: 14831
		private VideoPlayer _videoPlayerBackBuffer;

		// Token: 0x040039F0 RID: 14832
		private RenderTexture _videoTexture;

		// Token: 0x040039F1 RID: 14833
		private GlobalSettings _settingData;

		// Token: 0x040039F2 RID: 14834
		private bool _pastEnding;
	}
}
