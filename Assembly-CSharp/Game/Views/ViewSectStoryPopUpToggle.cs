using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.UIElements;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Video;

namespace Game.Views
{
	// Token: 0x0200070E RID: 1806
	public class ViewSectStoryPopUpToggle : UIBase
	{
		// Token: 0x17000A69 RID: 2665
		// (get) Token: 0x06005581 RID: 21889 RVA: 0x0027A428 File Offset: 0x00278628
		// (set) Token: 0x06005582 RID: 21890 RVA: 0x0027A430 File Offset: 0x00278630
		public bool IsSkipEnabled { get; private set; } = false;

		// Token: 0x17000A6A RID: 2666
		// (get) Token: 0x06005583 RID: 21891 RVA: 0x0027A439 File Offset: 0x00278639
		private int CurrentDate
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().CurrDate;
			}
		}

		// Token: 0x06005584 RID: 21892 RVA: 0x0027A445 File Offset: 0x00278645
		private void Awake()
		{
			this.SetupEventTriggers();
			this.sectStoryToggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				this._isPaused = !isOn;
				this.confirmButton.interactable = isOn;
				bool inited = this._inited;
				if (inited)
				{
					SectMainSettings.SetSectMainStoryIsActive(this._sectTemplateId, this._isPaused);
				}
				this.cancelButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ((!this._isShowConfirmButton) ? LanguageKey.LK_ViewSectStoryPopUpToggle_Close.Tr() : (isOn ? LanguageKey.LK_ViewSectStoryPopUpToggle_SkipOnce.Tr() : LanguageKey.LK_ViewSectStoryPopUpToggle_Close.Tr()));
				TooltipInvoker mouseTip = this.sectStoryToggle.GetComponent<TooltipInvoker>();
				mouseTip.PresetParam[0] = (isOn ? "LK_SystemSetting_BaseSettings_BtnTips_Pause_Desc" : "LK_SystemSetting_BaseSettings_BtnTips_Start_Desc");
				mouseTip.Refresh(false, -1);
				this.status.text = LanguageKey.LK_Dot_Symbol.Tr() + ((!this._isPaused) ? LanguageKey.LK_ViewSectStoryPopUpToggle_TipsStart.Tr() : LanguageKey.LK_ViewSectStoryPopUpToggle_TipsPause.Tr());
			});
		}

		// Token: 0x06005585 RID: 21893 RVA: 0x0027A46C File Offset: 0x0027866C
		public bool IsTriggeredThisMonth(sbyte sectTemplateId)
		{
			bool flag = this._triggeredDate != this.CurrentDate;
			if (flag)
			{
				this.SectTriggeredSet.Clear();
				this._triggeredDate = this.CurrentDate;
			}
			return this.SectTriggeredSet.Contains((int)sectTemplateId);
		}

		// Token: 0x06005586 RID: 21894 RVA: 0x0027A4BC File Offset: 0x002786BC
		public override void OnInit(ArgumentBox argsBox)
		{
			ViewSectStoryPopUpToggle.<>c__DisplayClass37_0 CS$<>8__locals1 = new ViewSectStoryPopUpToggle.<>c__DisplayClass37_0();
			CS$<>8__locals1.<>4__this = this;
			argsBox.Get("TemplateId", out this._templateId);
			argsBox.Get("ConditionStatus", out CS$<>8__locals1.conditionStatus);
			argsBox.Get("IsShowConfirmButton", out this._isShowConfirmButton);
			CS$<>8__locals1.config = WorldState.Instance[this._templateId];
			this._sectTemplateId = CS$<>8__locals1.config.Sect;
			this.title.text = CS$<>8__locals1.config.Name;
			this.desc.text = CS$<>8__locals1.config.Desc;
			this.sectImage.texture = this.sectTexs[(int)(this._sectTemplateId - 1)];
			bool flag = CS$<>8__locals1.config.SectStoryCondition == null;
			if (!flag)
			{
				CImage cimage = this.buttonIcon;
				string str = "ui9_icon_mainpopup_sectswitch_icon_play_";
				object obj = this.playVideoButton.interactable ? '0' : "2";
				cimage.SetSprite(str + ((obj != null) ? obj.ToString() : null), false, null);
				this.videoStaticBack.texture = this.videoBackImgs[0];
				this.confirmButton.gameObject.SetActive(this._isShowConfirmButton);
				this._inited = false;
				SectMainSettings.GetSectMainStoryIsActive(this._sectTemplateId, new Action<int>(CS$<>8__locals1.<OnInit>g__RefreshActiveStatus|0), UIElement.SectStoryPopUpToggle.UiBaseAs<ViewSectStoryPopUpToggle>());
			}
		}

		// Token: 0x06005587 RID: 21895 RVA: 0x0027A628 File Offset: 0x00278828
		private void Update()
		{
			bool flag = CommonCommandKit.LeftMouse.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				bool waitingCgPlayer = this._waitingCgPlayer;
				if (waitingCgPlayer)
				{
					return;
				}
				this.SkipAnim();
			}
			bool flag2 = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
			if (flag2)
			{
				bool flag3 = this.confirmButton.gameObject.activeSelf && this.confirmButton.interactable;
				if (flag3)
				{
					this.confirmButton.onClick.Invoke();
				}
			}
		}

		// Token: 0x06005588 RID: 21896 RVA: 0x0027A6B8 File Offset: 0x002788B8
		private void LateUpdate()
		{
			bool flag = !this.videoPlayer.isPlaying;
			if (flag)
			{
				bool playStarted = this._playStarted;
				if (playStarted)
				{
					this.videoHolder.gameObject.SetActive(false);
				}
			}
			else
			{
				this._playStarted = true;
			}
		}

		// Token: 0x06005589 RID: 21897 RVA: 0x0027A701 File Offset: 0x00278901
		private void OnDisable()
		{
			this._playStarted = false;
		}

		// Token: 0x0600558A RID: 21898 RVA: 0x0027A70C File Offset: 0x0027890C
		private void UpdateStatus(WorldStateItem config, int conditionStatus)
		{
			CommonUtils.PrepareEnoughChildren(this.conditionHolder, this.conditionHolder.GetChild(0).gameObject, config.SectStoryCondition.Length, null);
			for (int i = 0; i < config.SectStoryCondition.Length; i++)
			{
				TextMeshProUGUI obj = this.conditionHolder.GetChild(i).GetComponent<TextMeshProUGUI>();
				bool isEnable = (conditionStatus & 1 << i) != 0;
				obj.text = config.SectStoryCondition[i].SetColor(isEnable ? "brightblue" : "grey");
			}
		}

		// Token: 0x0600558B RID: 21899 RVA: 0x0027A7A4 File Offset: 0x002789A4
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			string text = btnName;
			string a = text;
			if (!(a == "PlayVideoButton"))
			{
				if (!(a == "ConfirmButton"))
				{
					if (a == "CancelButton")
					{
						bool flag = this._isShowConfirmButton && !this.SectTriggeredSet.Contains((int)this._sectTemplateId);
						if (flag)
						{
							this.SectTriggeredSet.Add((int)this._sectTemplateId);
						}
						this.QuickHide();
					}
				}
				else
				{
					TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("SectStoryPopUpToggle", "SectStoryPopUpToggleIsConfirm", true);
					this.QuickHide();
				}
			}
			else
			{
				UIElement.BlockInteract.Show();
				CommandManager.AddCommandAuto(EPriority.ShowUINormal, new CmdExecuteDelegate(this.PlayCg), () => !this._waitingCgPlayer);
			}
		}

		// Token: 0x0600558C RID: 21900 RVA: 0x0027A870 File Offset: 0x00278A70
		public override void QuickHide()
		{
			TaiwuEventDomainMethod.Call.TriggerListener("SectStoryPopUpToggle", true);
			base.QuickHide();
		}

		// Token: 0x0600558D RID: 21901 RVA: 0x0027A888 File Offset: 0x00278A88
		private bool PlayCg()
		{
			this._waitingCgPlayer = true;
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("CGName", string.Format("SectStoryPV/SectStoryPV_{0}", this._sectTemplateId));
			box.Set("RenderMode", 2);
			box.Set("Localized", true);
			box.Set("JumpOpen", true);
			box.SetObject("OnVideoPlayStart", new Action(ViewSectStoryPopUpToggle.<PlayCg>g__OnVideoPlayStart|44_0));
			UIElement cgPlayer = UIElement.CgPlayer;
			cgPlayer.OnDeActive = (Action)Delegate.Combine(cgPlayer.OnDeActive, new Action(delegate()
			{
				UIElement.BlockInteract.Hide(false);
				this._waitingCgPlayer = false;
				AudioManager.Instance.ExitVideoMode();
			}));
			UIElement.CgPlayer.SetOnInitArgs(box);
			UIElement.CgPlayer.Show();
			return true;
		}

		// Token: 0x0600558E RID: 21902 RVA: 0x0027A948 File Offset: 0x00278B48
		private void SetupEventTriggers()
		{
			EventTrigger trigger = this.playVideoButton.gameObject.GetComponent<EventTrigger>();
			ViewSectStoryPopUpToggle.<SetupEventTriggers>g__AddEventTriggerListener|45_6(trigger, UnityEngine.EventSystems.EventTriggerType.PointerEnter, delegate(BaseEventData data)
			{
				this.<SetupEventTriggers>g__OnButtonHighlighted|45_8();
			});
			ViewSectStoryPopUpToggle.<SetupEventTriggers>g__AddEventTriggerListener|45_6(trigger, UnityEngine.EventSystems.EventTriggerType.PointerExit, delegate(BaseEventData data)
			{
				this.<SetupEventTriggers>g__OnButtonNormal|45_7();
			});
			ViewSectStoryPopUpToggle.<SetupEventTriggers>g__AddEventTriggerListener|45_6(trigger, UnityEngine.EventSystems.EventTriggerType.PointerDown, delegate(BaseEventData data)
			{
				this.<SetupEventTriggers>g__OnButtonPressed|45_9();
			});
			ViewSectStoryPopUpToggle.<SetupEventTriggers>g__AddEventTriggerListener|45_6(trigger, UnityEngine.EventSystems.EventTriggerType.PointerUp, delegate(BaseEventData data)
			{
				this.<SetupEventTriggers>g__OnButtonNormal|45_7();
			});
			ViewSectStoryPopUpToggle.<SetupEventTriggers>g__AddEventTriggerListener|45_6(trigger, UnityEngine.EventSystems.EventTriggerType.Select, delegate(BaseEventData data)
			{
				this.<SetupEventTriggers>g__OnButtonSelected|45_10();
			});
			ViewSectStoryPopUpToggle.<SetupEventTriggers>g__AddEventTriggerListener|45_6(trigger, UnityEngine.EventSystems.EventTriggerType.Deselect, delegate(BaseEventData data)
			{
				this.<SetupEventTriggers>g__OnButtonNormal|45_7();
			});
		}

		// Token: 0x0600558F RID: 21903 RVA: 0x0027A9E8 File Offset: 0x00278BE8
		private void SkipAnim()
		{
			bool flag = !this.videoHolder || !this.videoHolder.gameObject.activeSelf || this.skipNotice == null;
			if (!flag)
			{
				bool activeSelf = this.skipNotice.gameObject.activeSelf;
				if (activeSelf)
				{
					bool flag2 = !this.IsSkipEnabled;
					if (!flag2)
					{
						this.skipNotice.gameObject.SetActive(false);
						this.videoHolder.gameObject.SetActive(false);
						this.playVideoButton.interactable = true;
					}
				}
				else
				{
					this.skipNotice.gameObject.SetActive(true);
					base.DelayCall(delegate
					{
						this.skipNotice.gameObject.SetActive(false);
					}, 3f);
					UIManager.Instance.SetEscHandler(new Action(this.SkipAnim));
				}
			}
		}

		// Token: 0x06005594 RID: 21908 RVA: 0x0027AC13 File Offset: 0x00278E13
		[CompilerGenerated]
		internal static void <PlayCg>g__OnVideoPlayStart|44_0()
		{
			AudioManager.Instance.EnterVideoMode();
		}

		// Token: 0x0600559B RID: 21915 RVA: 0x0027AC58 File Offset: 0x00278E58
		[CompilerGenerated]
		internal static void <SetupEventTriggers>g__AddEventTriggerListener|45_6(EventTrigger trigger, UnityEngine.EventSystems.EventTriggerType eventType, UnityAction<BaseEventData> callback)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry
			{
				eventID = eventType
			};
			entry.callback.AddListener(callback);
			trigger.triggers.Add(entry);
		}

		// Token: 0x0600559C RID: 21916 RVA: 0x0027AC90 File Offset: 0x00278E90
		[CompilerGenerated]
		private void <SetupEventTriggers>g__OnButtonNormal|45_7()
		{
			CImage cimage = this.buttonIcon;
			string str = "ui9_icon_mainpopup_sectswitch_icon_play_";
			object obj = this.playVideoButton.interactable ? '0' : "2";
			cimage.SetSprite(str + ((obj != null) ? obj.ToString() : null), false, null);
		}

		// Token: 0x0600559D RID: 21917 RVA: 0x0027ACE0 File Offset: 0x00278EE0
		[CompilerGenerated]
		private void <SetupEventTriggers>g__OnButtonHighlighted|45_8()
		{
			CImage cimage = this.buttonIcon;
			string str = "ui9_icon_mainpopup_sectswitch_icon_play_";
			object obj = this.playVideoButton.interactable ? '1' : "2";
			cimage.SetSprite(str + ((obj != null) ? obj.ToString() : null), false, null);
		}

		// Token: 0x0600559E RID: 21918 RVA: 0x0027AD30 File Offset: 0x00278F30
		[CompilerGenerated]
		private void <SetupEventTriggers>g__OnButtonPressed|45_9()
		{
			CImage cimage = this.buttonIcon;
			string str = "ui9_icon_mainpopup_sectswitch_icon_play_";
			object obj = this.playVideoButton.interactable ? '1' : "2";
			cimage.SetSprite(str + ((obj != null) ? obj.ToString() : null), false, null);
		}

		// Token: 0x0600559F RID: 21919 RVA: 0x0027AD80 File Offset: 0x00278F80
		[CompilerGenerated]
		private void <SetupEventTriggers>g__OnButtonSelected|45_10()
		{
			CImage cimage = this.buttonIcon;
			string str = "ui9_icon_mainpopup_sectswitch_icon_play_";
			object obj = this.playVideoButton.interactable ? '1' : "2";
			cimage.SetSprite(str + ((obj != null) ? obj.ToString() : null), false, null);
		}

		// Token: 0x04003A4F RID: 14927
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x04003A50 RID: 14928
		[SerializeField]
		private CRawImage sectImage;

		// Token: 0x04003A51 RID: 14929
		[SerializeField]
		private Texture2D[] sectTexs;

		// Token: 0x04003A52 RID: 14930
		[SerializeField]
		private CButton playVideoButton;

		// Token: 0x04003A53 RID: 14931
		[SerializeField]
		private CImage buttonIcon;

		// Token: 0x04003A54 RID: 14932
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x04003A55 RID: 14933
		[SerializeField]
		private RectTransform conditionHolder;

		// Token: 0x04003A56 RID: 14934
		[SerializeField]
		private TextMeshProUGUI status;

		// Token: 0x04003A57 RID: 14935
		[SerializeField]
		private CToggle sectStoryToggle;

		// Token: 0x04003A58 RID: 14936
		[SerializeField]
		private CButton confirmButton;

		// Token: 0x04003A59 RID: 14937
		[SerializeField]
		private CButton cancelButton;

		// Token: 0x04003A5A RID: 14938
		[SerializeField]
		private RectTransform videoHolder;

		// Token: 0x04003A5B RID: 14939
		[SerializeField]
		private VideoPlayer videoPlayer;

		// Token: 0x04003A5C RID: 14940
		[SerializeField]
		private CRawImage videoPlayerTarget;

		// Token: 0x04003A5D RID: 14941
		[SerializeField]
		private VideoClip[] videoClips;

		// Token: 0x04003A5E RID: 14942
		[SerializeField]
		private CRawImage videoStaticBack;

		// Token: 0x04003A5F RID: 14943
		[SerializeField]
		private Texture[] videoBackImgs;

		// Token: 0x04003A60 RID: 14944
		[SerializeField]
		private RectTransform skipNotice;

		// Token: 0x04003A61 RID: 14945
		private bool _playStarted;

		// Token: 0x04003A62 RID: 14946
		private sbyte _templateId;

		// Token: 0x04003A63 RID: 14947
		private sbyte _sectTemplateId;

		// Token: 0x04003A64 RID: 14948
		private bool _isPaused;

		// Token: 0x04003A65 RID: 14949
		private bool _isShowConfirmButton;

		// Token: 0x04003A66 RID: 14950
		private bool _waitingCgPlayer;

		// Token: 0x04003A67 RID: 14951
		private const float DisableSkipTime = 0.3f;

		// Token: 0x04003A68 RID: 14952
		private readonly WaitForSeconds _disableSkipWait = new WaitForSeconds(0.3f);

		// Token: 0x04003A6A RID: 14954
		public readonly HashSet<int> SectTriggeredSet = new HashSet<int>();

		// Token: 0x04003A6B RID: 14955
		private int _triggeredDate;

		// Token: 0x04003A6C RID: 14956
		private bool _inited;
	}
}
