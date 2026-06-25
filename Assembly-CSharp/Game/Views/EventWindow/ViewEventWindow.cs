using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using CharacterDataMonitor;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Common;
using Game.Components.EventWindow;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Views.CharacterMenu;
using Game.Views.Cricket;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UICommon.Character.Elements;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.EventWindow
{
	// Token: 0x02000A46 RID: 2630
	public class ViewEventWindow : UIBase
	{
		// Token: 0x17000E3C RID: 3644
		// (get) Token: 0x060081D6 RID: 33238 RVA: 0x003C73B5 File Offset: 0x003C55B5
		// (set) Token: 0x060081D7 RID: 33239 RVA: 0x003C73BD File Offset: 0x003C55BD
		private bool WaitSelect { get; set; }

		// Token: 0x17000E3D RID: 3645
		// (get) Token: 0x060081D8 RID: 33240 RVA: 0x003C73C6 File Offset: 0x003C55C6
		private bool CanSelect
		{
			get
			{
				return this.WaitSelect && !this._animating && !this._isDisplayingLog;
			}
		}

		// Token: 0x060081D9 RID: 33241 RVA: 0x003C73E4 File Offset: 0x003C55E4
		private void ApplyContentFontSize()
		{
			this._contentFontSize = ViewEventWindow.s_savedContentFontSize;
			this.eventContent.fontSize = this._contentFontSize;
			this.contentTextSizeSlider.SetValueWithoutNotify(this._contentFontSize);
		}

		// Token: 0x17000E3E RID: 3646
		// (get) Token: 0x060081DA RID: 33242 RVA: 0x003C7418 File Offset: 0x003C5618
		private EventModel Model
		{
			get
			{
				bool flag = this._model == null;
				if (flag)
				{
					this._model = SingletonObject.getInstance<EventModel>();
				}
				return this._model;
			}
		}

		// Token: 0x17000E3F RID: 3647
		// (get) Token: 0x060081DB RID: 33243 RVA: 0x003C7448 File Offset: 0x003C5648
		private EventTextureManager EventTextureManager
		{
			get
			{
				return SingletonObject.getInstance<EventTextureManager>();
			}
		}

		// Token: 0x17000E40 RID: 3648
		// (get) Token: 0x060081DC RID: 33244 RVA: 0x003C744F File Offset: 0x003C564F
		private TaiwuEventDisplayData Data
		{
			get
			{
				return this.Model.DisplayingEventData;
			}
		}

		// Token: 0x17000E41 RID: 3649
		// (get) Token: 0x060081DD RID: 33245 RVA: 0x003C745C File Offset: 0x003C565C
		private int NormalOptionHotKeyCount
		{
			get
			{
				return this._optionHotKeyCommands.Length - 1;
			}
		}

		// Token: 0x060081DE RID: 33246 RVA: 0x003C7468 File Offset: 0x003C5668
		private void Awake()
		{
			this.mouseTipExit.enabled = false;
			this.mouseTipExit.Type = TipType.SingleDesc;
			TooltipInvoker tooltipInvoker = this.mouseTipExit;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.mouseTipExit.RuntimeParam.Set("arg0", LocalStringManager.GetFormat(LanguageKey.LK_Mousetip_EventWindow_Farewell, EventWindowCommandKit.OptionExit.ToString()));
			this._optionPool = new PoolItem("UI_EventWindow_OptionItem", this.optionItem.gameObject);
			this.leftCharacter.OnViewCharacter = new Action<int, int, bool>(this.ShowCharacterMenu);
			this.leftCharacter.isLeftCharacter = true;
			this.rightCharacter.OnViewCharacter = new Action<int, int, bool>(this.ShowCharacterMenu);
			this.contentTextSizeSlider.minValue = 18f;
			this.contentTextSizeSlider.maxValue = 36f;
			this.contentTextSizeSlider.onValueChanged.RemoveAllListeners();
			this.contentTextSizeSlider.onValueChanged.AddListener(delegate(float value)
			{
				this.eventContent.fontSize = value;
				this._contentFontSize = value;
				ViewEventWindow.s_savedContentFontSize = value;
			});
			this.ApplyContentFontSize();
			this.eventContentArea.EnterEvent.ResetListener(delegate()
			{
				this.contentTextSizeBtn.gameObject.SetActive(true);
			});
			this.eventContentArea.ExitEvent.ResetListener(delegate()
			{
				this.contentTextSizeSlider.gameObject.SetActive(false);
				this.contentTextSizeBtn.gameObject.SetActive(false);
			});
			this.characterScrollView.OnItemRender += this.OnCharacterScrollItemRender;
			this.characterScrollView.OnItemHide += this.OnCharacterScrollItemHide;
			this.interactionTypeTogGroup.OnActiveIndexChange += this.OnActiveToggleChange;
			this._toggleGroupAnim = this.interactionTypeTogGroup.GetComponent<CommonSecondToggleContentRefreshAnim>();
			this._toggleGroupAnim.SetWaitCallParam(new List<RectTransform>
			{
				this.optionContainer
			}, null);
		}

		// Token: 0x060081DF RID: 33247 RVA: 0x003C7634 File Offset: 0x003C5834
		private void OnEnable()
		{
			GEvent.OnEvent(UiEvents.OnEventWindowStart, null);
			SingletonObject.getInstance<WorldMapModel>().ChangeTaiwuMoveState(WorldMapModel.MoveState.Idle);
			this.contentTextSizeSlider.gameObject.SetActive(false);
			this.contentTextSizeBtn.gameObject.SetActive(false);
			this.animationRoot.localPosition = Vector3.up * 3000f;
			this._posInViewPort = false;
			foreach (UIElement conflictElement in ViewEventWindow._conflictElements)
			{
				UIManager.Instance.HideUI(conflictElement);
			}
			GEvent.Add(UiEvents.EventWindowAutoChoose, new GEvent.Callback(this.OnAutoChooseMessage));
			bool flag = string.IsNullOrEmpty(this.Model.EventCgTextureData.Item2) && this.cGTexture.texture != null;
			if (flag)
			{
				this.HideCgTexture();
			}
			bool flag2 = string.IsNullOrEmpty(this.Model.EventCgTextureData.Item2) && this.cGTexture.texture == null;
			if (flag2)
			{
				this.HideCgTexture();
				this.Model.SetCgDataHandled();
				this.blackMask.SetAlpha(0f);
				this._animationMaskComplete = true;
			}
			this.Refresh();
			this.ApplyContentFontSize();
			GEvent.OnEvent(UiEvents.OnNewEventComingToShow, null);
			GEvent.OnEvent(UiEvents.OnNeedCombatPause, null);
			GEvent.Add(UiEvents.OnEventWindowDisplayDataChanged, new GEvent.Callback(this.OnDisplayDataChanged));
			GEvent.Add(UiEvents.OnMarriageCharacterListChanged, new GEvent.Callback(this.OnMarriageCharacterAvatarChanged));
			GEvent.Add(UiEvents.OnJieqingMaskCharacterListChanged, new GEvent.Callback(this.OnJieqingMaskCharacterListChanged));
			GEvent.Add(UiEvents.MouseTipBaseOnEnable, new GEvent.Callback(this.MouseTipBaseOnEnable));
			GEvent.Add(UiEvents.MouseTipBaseOnDisable, new GEvent.Callback(this.MouseTipBaseOnDisable));
			GEvent.Add(UiEvents.OnChangeCharacterClothing, new GEvent.Callback(this.OnChangeCharacterClothing));
			GEvent.Add(UiEvents.OnEventWindowSelectItems, new GEvent.Callback(this.OnEventWindowSelectItems));
			GEvent.Add(UiEvents.OnTaiwuOriginalSurnameSettingChanged, new GEvent.Callback(this.OnTaiwuOriginalSurnameSettingChanged));
		}

		// Token: 0x060081E0 RID: 33248 RVA: 0x003C78A4 File Offset: 0x003C5AA4
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUIChanged));
			GEvent.Remove(UiEvents.OnTaiwuOriginalSurnameSettingChanged, new GEvent.Callback(this.OnTaiwuOriginalSurnameSettingChanged));
			GEvent.Remove(UiEvents.OnMarriageCharacterListChanged, new GEvent.Callback(this.OnMarriageCharacterAvatarChanged));
			GEvent.Remove(UiEvents.OnJieqingMaskCharacterListChanged, new GEvent.Callback(this.OnJieqingMaskCharacterListChanged));
			GEvent.Remove(UiEvents.OnEventWindowDisplayDataChanged, new GEvent.Callback(this.OnDisplayDataChanged));
			GEvent.Remove(UiEvents.EventWindowAutoChoose, new GEvent.Callback(this.OnAutoChooseMessage));
			this.leftCharacter.gameObject.SetActive(false);
			this.rightCharacter.gameObject.SetActive(false);
			this.StopAllCricketSing();
			this.SetSelecting(false);
			GEvent.OnEvent(UiEvents.OnNeedCombatResume, null);
			GEvent.OnEvent(UiEvents.OnEventWindowEnded, null);
			GEvent.Remove(UiEvents.MouseTipBaseOnEnable, new GEvent.Callback(this.MouseTipBaseOnEnable));
			GEvent.Remove(UiEvents.MouseTipBaseOnDisable, new GEvent.Callback(this.MouseTipBaseOnDisable));
			GEvent.Remove(UiEvents.OnChangeCharacterClothing, new GEvent.Callback(this.OnChangeCharacterClothing));
			GEvent.Remove(UiEvents.OnEventWindowSelectItems, new GEvent.Callback(this.OnEventWindowSelectItems));
			this.ClearCommonOptionPointerHandlers();
			this._isCommonOptionInit = false;
			this._lastUserSelectedCommonOptionIndex = -1;
			this._selectedOptionInfo = default(EventOptionInfo);
		}

		// Token: 0x060081E1 RID: 33249 RVA: 0x003C7A30 File Offset: 0x003C5C30
		private void OnDestroy()
		{
			this._optionPool.Destroy();
		}

		// Token: 0x060081E2 RID: 33250 RVA: 0x003C7A40 File Offset: 0x003C5C40
		private void Update()
		{
			bool flag = this.Data == null;
			if (!flag)
			{
				bool flag2 = !this._animationMaskComplete;
				if (!flag2)
				{
					for (int i = 0; i < this._optionHotKeyCommands.Length; i++)
					{
						bool flag3 = i == (int)this.Data.EscOptionIndex;
						if (!flag3)
						{
							bool flag4 = this._optionHotKeyCommands[i].Check(this.Element, false, false, true, true, false);
							if (flag4)
							{
								this.SelectOptionByIndex(i, false);
								return;
							}
							bool flag5 = i >= this.Data.EventOptionInfos.Count;
							if (flag5)
							{
								break;
							}
						}
					}
					short j = 0;
					while ((int)j < this._commonHotKeyCommands.Length)
					{
						bool flag6 = j == 6;
						if (flag6)
						{
							break;
						}
						bool flag7 = this._commonHotKeyCommands[(int)j].Check(this.Element, false, false, true, true, false);
						if (flag7)
						{
							bool activeSelf = this.commonOption.gameObject.activeSelf;
							if (activeSelf)
							{
								CToggle toggle = this.interactionTypeTogGroup.Get((int)j);
								bool flag8 = toggle.interactable && toggle.isActiveAndEnabled;
								if (flag8)
								{
									this.interactionTypeTogGroup.Set((int)j, false);
								}
							}
							return;
						}
						j += 1;
					}
					bool flag9 = TabSwitchCommandKit.PrevTabLevel1.Check(this.Element, false, false, true, true, false);
					if (flag9)
					{
						bool flag10 = !this.commonOption.gameObject.activeSelf;
						if (flag10)
						{
							return;
						}
						this.interactionTypeTogGroup.SelectNext(-1, false);
					}
					bool flag11 = TabSwitchCommandKit.NextTabLevel1.Check(this.Element, false, false, true, true, false);
					if (flag11)
					{
						bool flag12 = !this.commonOption.gameObject.activeSelf;
						if (flag12)
						{
							return;
						}
						this.interactionTypeTogGroup.SelectNext(1, false);
					}
					bool escHandled = UIManager.Instance.EscHandled;
					if (!escHandled)
					{
						bool flag13 = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && this.ConfirmActive();
						if (flag13)
						{
							this.OnClick(this.confirm.transform);
						}
						else
						{
							bool flag14 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) && this.CancelActive();
							if (flag14)
							{
								this.OnClick(this.cancel.transform);
							}
							else
							{
								bool flag15 = this.Data.EscOptionIndex < 0 && this._optionHotKeyCommands.Last<HotKeyCommand>().Check(this.Element, false, false, true, true, false);
								if (flag15)
								{
									CommandManager.AddCommand<CommandMaskUI, UIElement>(EPriority.OpenUISystemOption, UIElement.SystemOption);
								}
								bool flag16 = this._optionHotKeyCommands.Last<HotKeyCommand>().Check(this.Element, false, false, true, true, false) && this.Data.EscOptionIndex >= 0;
								if (flag16)
								{
									this.SelectEscOption();
								}
								bool flag17 = UIManager.Instance.IsFocusElement(this.Element) && this.commonOption.gameObject.activeSelf && this.Data.EscOptionIndex < 0 && Input.GetKey(this._optionHotKeyCommands.Last<HotKeyCommand>().KeyGroup.Key);
								if (flag17)
								{
									TaiwuEventDomainMethod.Call.EventCommonOptionSelect(6);
									UIManager.Instance.SetEscHandler(delegate
									{
									});
								}
								bool flag18 = CommonCommandKit.RightMouse.Check(this.Element, false, false, true, true, false);
								if (flag18)
								{
									bool flag19 = this.Data.ExtraData.InputRequestData != null && this.cancel.interactable && this.cancel.gameObject.activeInHierarchy;
									if (flag19)
									{
										this.OnClick(this.cancel.transform);
									}
									else
									{
										bool flag20 = this.Data.EscOptionIndex >= 0;
										if (flag20)
										{
											this.SelectEscOption();
										}
										else
										{
											bool activeSelf2 = this.commonOption.gameObject.activeSelf;
											if (activeSelf2)
											{
												TaiwuEventDomainMethod.Call.EventCommonOptionSelect(6);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060081E3 RID: 33251 RVA: 0x003C7E6C File Offset: 0x003C606C
		private bool ConfirmActive()
		{
			return this.confirm.interactable && this.confirm.gameObject.activeInHierarchy && this.Data.ExtraData.InputRequestData == null;
		}

		// Token: 0x060081E4 RID: 33252 RVA: 0x003C7EB4 File Offset: 0x003C60B4
		private bool CancelActive()
		{
			return this.cancel.interactable && this.cancel.gameObject.activeInHierarchy;
		}

		// Token: 0x060081E5 RID: 33253 RVA: 0x003C7EE8 File Offset: 0x003C60E8
		public override void OnInit(ArgumentBox argsBox)
		{
			this.operateArea.gameObject.SetActive(false);
			this.buttons.SetActive(false);
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUIChanged));
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this._taiwuDetailInfoMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DetailInfoMonitor>(taiwuCharId, false);
			RectTransform rectTrans = this.selectItemsPanel.GetComponent<RectTransform>();
			this._itemScrollViewInitPos = rectTrans.anchoredPosition;
			this._itemScrollViewInitDeltaSize = rectTrans.sizeDelta;
		}

		// Token: 0x060081E6 RID: 33254 RVA: 0x003C7F70 File Offset: 0x003C6170
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = "Confirm" == btnName;
			if (flag)
			{
				ViewEventWindow.<>c__DisplayClass111_0 CS$<>8__locals1 = new ViewEventWindow.<>c__DisplayClass111_0();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.optionIndex = 0;
				bool flag2 = this.Data.ExtraData.InputRequestData != null;
				if (flag2)
				{
					bool flag3 = this.Data.ExtraData.InputRequestData.InputDataType == 5;
					if (flag3)
					{
						string inputResultSur = this.panelInputChangeName.InputFieldSurName.text;
						string inputResultGiven = this.panelInputChangeName.InputFieldGivenName.text;
						bool flag4 = string.IsNullOrEmpty(inputResultSur) || string.IsNullOrEmpty(inputResultGiven);
						if (flag4)
						{
							return;
						}
						SingletonObject.getInstance<EventModel>().SetInputNameResult(inputResultSur, inputResultGiven);
					}
					else
					{
						string inputResult = this.panelInput.InputField.text;
						bool flag5 = string.IsNullOrEmpty(inputResult);
						if (flag5)
						{
							return;
						}
						SingletonObject.getInstance<EventModel>().SetInputResult(inputResult);
					}
				}
				bool flag6 = this.Data.ExtraData.SelectItemData != null && this.selectItemsPanel.SelectedKeyList.Count > 0;
				if (flag6)
				{
					SingletonObject.getInstance<EventModel>().SetSelectItemResult(this.selectItemsPanel.SelectedKeyList, this.selectItemsPanel.GetSelectedItemDict());
					this._selectedItemList.Clear();
					this._selectedItemDict.Clear();
					this.SetSelecting(false);
				}
				bool flag7 = this.Data.ExtraData.SelectCharacterData != null && this.directSelectCharacterPanel.SelectedCharIdList.Count > 0;
				if (flag7)
				{
					bool flag8 = this.Data.ExtraData.SelectCharacterData.SelectApprovedTaiwu != null && this.<OnClick>g__SelectedCharacterHasDukeTitle|111_2();
					if (flag8)
					{
						DialogCmd cmd = new DialogCmd
						{
							Title = LocalStringManager.Get(LanguageKey.LK_Bottom_MapInfo_GiveTitle),
							Content = LocalStringManager.Get(LanguageKey.UI_Profession_SelectDukeTitleTips),
							Type = 1,
							Yes = delegate()
							{
								CS$<>8__locals1.<>4__this.<OnClick>g__SelectCharacterAction|111_1(CS$<>8__locals1.<>4__this.directSelectCharacterPanel.SelectedCharIdList);
								base.<OnClick>g__ConfirmAction|0();
							}
						};
						UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
						UIManager.Instance.MaskUI(UIElement.Dialog);
						return;
					}
					this.<OnClick>g__SelectCharacterAction|111_1(this.directSelectCharacterPanel.SelectedCharIdList);
				}
				bool flag9 = this.Data.ExtraData.SelectCharacterData != null && this.characterListPanel.SelectedCharIdList.Count > 0;
				if (flag9)
				{
					this.<OnClick>g__SelectCharacterAction|111_1(this.characterListPanel.SelectedCharIdList);
				}
				bool flag10 = this.Data.ExtraData.SelectReadingBookCountData != null;
				if (flag10)
				{
					SingletonObject.getInstance<EventModel>().SetSelectBookCountResult(this.panelSelectBookCount.CurAmount, this.panelSelectBookCount.TargetBookKey);
				}
				else
				{
					bool flag11 = this.Data.ExtraData.SelectNeigongLoopingCountData != null;
					if (flag11)
					{
						SingletonObject.getInstance<EventModel>().SetSelectCombatSkillCountResult(this.panelSelectNeigongLoopingCount.CurAmount, this.panelSelectNeigongLoopingCount.selectedCombatSkill.TemplateId);
					}
					else
					{
						bool flag12 = this.Data.ExtraData.SelectFuyuFaithCountData != null;
						if (flag12)
						{
							SingletonObject.getInstance<EventModel>().SetSelectCountResult(this.fuyuFaith.Value);
							this.SetSelecting(false);
						}
					}
				}
				bool flag13 = this.Data.ExtraData.SelectOneAvatarRelatedDataList != null && -1 != this._selectedAvatarIndex;
				if (flag13)
				{
					CS$<>8__locals1.optionIndex = this._selectedAvatarIndex;
					this.SetSelecting(false);
					this._selectedAvatarIndex = -1;
				}
				bool flag14 = this.Data.ExtraData.SelectFameData != null && this.fameActionPanel.selectedFameActionIdList.Count > 0;
				if (flag14)
				{
					SingletonObject.getInstance<EventModel>().SetSelectFameActionResult(this.fameActionPanel.selectedFameActionIdList);
				}
				CS$<>8__locals1.<OnClick>g__ConfirmAction|0();
			}
			else
			{
				bool flag15 = "Cancel" == btnName;
				if (flag15)
				{
					int selectOptionIndex = 1;
					bool flag16 = this.Data.ExtraData.SelectOneAvatarRelatedDataList != null && this.Data.ExtraData.SelectOneAvatarRelatedDataList.Count > 0;
					if (flag16)
					{
						selectOptionIndex = (int)this.Data.EscOptionIndex;
					}
					this.SelectOption(this.Data.EventOptionInfos[selectOptionIndex]);
					this.ResetData();
				}
				else
				{
					bool flag17 = "BtnEventLog" == btnName;
					if (flag17)
					{
						bool flag18 = !this.WaitSelect;
						if (!flag18)
						{
							this.WaitSelect = false;
							UIElement eventLog = UIElement.EventLog;
							eventLog.OnShowed = (Action)Delegate.Combine(eventLog.OnShowed, new Action(delegate()
							{
								this.WaitSelect = true;
							}));
							UIElement eventLog2 = UIElement.EventLog;
							eventLog2.OnHide = (Action)Delegate.Combine(eventLog2.OnHide, new Action(delegate()
							{
								this.SwitchIsDisplayingLog(false);
							}));
							UIManager.Instance.MaskUI(UIElement.EventLog);
							this.SwitchIsDisplayingLog(true);
						}
					}
					else
					{
						bool flag19 = "ContentTextSizeBtn" == btnName;
						if (flag19)
						{
							this.contentTextSizeSlider.gameObject.SetActive(!this.contentTextSizeSlider.gameObject.activeInHierarchy);
							bool activeInHierarchy = this.contentTextSizeSlider.gameObject.activeInHierarchy;
							if (activeInHierarchy)
							{
								this.ApplyContentFontSize();
							}
						}
						else
						{
							bool flag20 = "System" == btnName;
							if (flag20)
							{
								CommandManager.AddCommand<CommandMaskUI, UIElement>(EPriority.OpenUISystemOption, UIElement.SystemOption);
							}
						}
					}
				}
			}
		}

		// Token: 0x060081E7 RID: 33255 RVA: 0x003C84DD File Offset: 0x003C66DD
		private void ResetData()
		{
			this._selectedItemList.Clear();
			this._selectedItemDict.Clear();
			this.characterListPanel.Clear();
			this.SetSelecting(false);
		}

		// Token: 0x060081E8 RID: 33256 RVA: 0x003C850C File Offset: 0x003C670C
		private void OnTopUIChanged(ArgumentBox argBox)
		{
			bool flag = UIManager.Instance.IsFocusElement(this.Element);
			if (flag)
			{
				base.transform.localPosition = Vector3.zero;
				bool flag2 = this._needSelectItem && this.NeedShowSelectingInItemSelectMode;
				if (flag2)
				{
					this.SetSelecting(true);
				}
			}
			else
			{
				this.SetSelecting(false);
			}
		}

		// Token: 0x060081E9 RID: 33257 RVA: 0x003C856C File Offset: 0x003C676C
		private void OnTaiwuOriginalSurnameSettingChanged(ArgumentBox argBox)
		{
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			bool flag = this.Data.MainCharacter != null && this.Data.MainCharacter.CharacterId == taiwuCharId;
			if (flag)
			{
				this.leftCharacter.Refresh();
			}
			bool flag2 = this.Data.TargetCharacter != null && this.Data.TargetCharacter.CharacterId == taiwuCharId;
			if (flag2)
			{
				this.rightCharacter.Refresh();
			}
		}

		// Token: 0x060081EA RID: 33258 RVA: 0x003C85EB File Offset: 0x003C67EB
		private void OnDisplayDataChanged(ArgumentBox box)
		{
			this.Refresh();
		}

		// Token: 0x060081EB RID: 33259 RVA: 0x003C85F8 File Offset: 0x003C67F8
		private void OnMarriageCharacterAvatarChanged(ArgumentBox box)
		{
			bool flag = this.Data == null;
			if (!flag)
			{
				this.leftCharacter.Refresh();
				this.rightCharacter.Refresh();
			}
		}

		// Token: 0x060081EC RID: 33260 RVA: 0x003C8630 File Offset: 0x003C6830
		private void OnJieqingMaskCharacterListChanged(ArgumentBox box)
		{
			Game.Components.EventWindow.EventWindowCharacter left = this.leftCharacter;
			Game.Components.EventWindow.EventWindowCharacter right = this.rightCharacter;
			bool flag = this.Data == null;
			if (flag)
			{
				bool initFlag = left.InitFlag;
				if (initFlag)
				{
					left.ResetJieqingHasMaskState();
				}
				bool initFlag2 = right.InitFlag;
				if (initFlag2)
				{
					right.ResetJieqingHasMaskState();
				}
			}
			else
			{
				bool needRefresh = box == null;
				List<int> jieqingMaskCharIdList = new List<int>();
				if (box != null)
				{
					box.Get<List<int>>("JieqingMaskCharIdList", out jieqingMaskCharIdList);
				}
				bool initFlag3 = left.InitFlag;
				if (initFlag3)
				{
					left.JieqingMaskRefresh(needRefresh, jieqingMaskCharIdList);
				}
				bool initFlag4 = right.InitFlag;
				if (initFlag4)
				{
					right.JieqingMaskRefresh(needRefresh, jieqingMaskCharIdList);
				}
			}
		}

		// Token: 0x060081ED RID: 33261 RVA: 0x003C86CC File Offset: 0x003C68CC
		private void OnEventWindowSelectItems(ArgumentBox _)
		{
			this.RefreshProfessionDoctorSkill0MedicinePreview();
		}

		// Token: 0x060081EE RID: 33262 RVA: 0x003C86D8 File Offset: 0x003C68D8
		private void RefreshProfessionDoctorSkill0MedicinePreview()
		{
			this.rightCharacter.SelectedMedicine = ((this.selectItemsPanel.gameObject.activeSelf && this.Data.ExtraData.SelectItemData.ItemOperationType == 16) ? (this.selectItemsPanel.DisplayDataDic.Keys.FirstOrDefault<ITradeableContent>() as ItemDisplayData) : null);
			this.rightCharacter.UpdateInjuryInfoTipsVisible();
		}

		// Token: 0x060081EF RID: 33263 RVA: 0x003C8748 File Offset: 0x003C6948
		private void MouseTipBaseOnEnable(ArgumentBox box)
		{
			bool flag = !UIManager.Instance.IsFocusElement(this.Element);
			if (!flag)
			{
				this.SetSelecting(false);
				this.SetSelectingApprovedTaiwu(false);
			}
		}

		// Token: 0x060081F0 RID: 33264 RVA: 0x003C8780 File Offset: 0x003C6980
		private void MouseTipBaseOnDisable(ArgumentBox box)
		{
			bool flag = !UIManager.Instance.IsFocusElement(this.Element);
			if (!flag)
			{
				bool flag2 = this._needSelectItem && this.NeedShowSelectingInItemSelectMode;
				if (flag2)
				{
					this.SetSelecting(true);
				}
			}
		}

		// Token: 0x060081F1 RID: 33265 RVA: 0x003C87C4 File Offset: 0x003C69C4
		private void OnAutoChooseMessage(ArgumentBox box)
		{
			ViewEventWindow.<>c__DisplayClass122_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.box = box;
			string guid;
			bool flag = !CS$<>8__locals1.box.Get("Guid", out guid) || guid != this.Data.EventGuid;
			if (flag)
			{
				throw new Exception("Event AutoChooseMessage Guid Error:" + guid);
			}
			string optionKey;
			bool flag2 = !CS$<>8__locals1.box.Get("Option", out optionKey) || !this.<OnAutoChooseMessage>g__CanSelectOption|122_0(optionKey, ref CS$<>8__locals1);
			if (flag2)
			{
				bool flag3 = this.RandomSelection(CS$<>8__locals1.box);
				if (flag3)
				{
					return;
				}
				bool firstOption;
				bool flag4 = CS$<>8__locals1.box.Get("AutoFirstOption", out firstOption) && firstOption;
				if (flag4)
				{
					RectTransform optionRoot = this.optionContainer;
					bool last;
					bool flag5 = CS$<>8__locals1.box.Get("Last", out last) && last;
					if (flag5)
					{
						int i = this.Data.EventOptionInfos.Count - 1;
						int max = 0;
						while (i > max)
						{
							Transform child = optionRoot.Find(this.Data.EventOptionInfos[i].OptionKey);
							Refers refers = child.GetComponent<Refers>();
							bool flag6 = null == refers;
							if (!flag6)
							{
								CButton button = refers.CGet<CButton>("Button");
								bool flag7 = null == button || !button.interactable;
								if (!flag7)
								{
									this.SelectOptionByIndex(i, false);
									break;
								}
							}
							i--;
						}
					}
					else
					{
						int j = 0;
						int max2 = this.Data.EventOptionInfos.Count;
						while (j < max2)
						{
							Transform child2 = optionRoot.Find(this.Data.EventOptionInfos[j].OptionKey);
							Refers refers2 = child2.GetComponent<Refers>();
							bool flag8 = null == refers2;
							if (!flag8)
							{
								CButton button2 = refers2.CGet<CButton>("Button");
								bool flag9 = null == button2 || !button2.interactable;
								if (!flag9)
								{
									this.SelectOptionByIndex(j, false);
									break;
								}
							}
							j++;
						}
					}
					return;
				}
				bool flag10 = !this.<OnAutoChooseMessage>g__CanSelectOption|122_0(optionKey, ref CS$<>8__locals1);
				if (flag10)
				{
					RectTransform optionRoot2 = this.optionContainer;
					int k = 0;
					int max3 = this.Data.EventOptionInfos.Count;
					while (k < max3)
					{
						Transform child3 = optionRoot2.Find(this.Data.EventOptionInfos[k].OptionKey);
						Refers refers3 = child3.GetComponent<Refers>();
						bool flag11 = null == refers3;
						if (!flag11)
						{
							CButton button3 = refers3.CGet<CButton>("Button");
							bool flag12 = null == button3 || !button3.interactable;
							if (!flag12)
							{
								this.SelectOptionByIndex(k, false);
								break;
							}
						}
						k++;
					}
					return;
				}
			}
			short combatSkillId;
			bool flag13 = CS$<>8__locals1.box.Get("CombatSkillId", out combatSkillId) && this._model.SelectCombatSkillData != null && optionKey == this._model.SelectCombatSkillData.OptionKey;
			if (flag13)
			{
				this.Model.SelectCombatSkillData.SelectResultIndex = this.Model.SelectCombatSkillData.CanSelectCombatSkillIdList.IndexOf(combatSkillId);
				TaiwuEventDomainMethod.Call.SetCombatSkillSelectResult(combatSkillId);
				this.SelectOptionByOptionKey(optionKey);
			}
			else
			{
				string inputValue;
				bool flag14 = CS$<>8__locals1.box.Get("InputValue", out inputValue) && this.Data.ExtraData.InputRequestData != null;
				if (flag14)
				{
					this._model.SetInputResult(inputValue);
					this.SelectOptionByOptionKey(optionKey);
				}
				else
				{
					bool selectItem;
					bool flag15 = CS$<>8__locals1.box.Get("SelectItem", out selectItem) && selectItem && this.Data.ExtraData.SelectItemData != null;
					if (flag15)
					{
						this.RandomSelection(CS$<>8__locals1.box);
					}
					else
					{
						List<int> charIdList;
						bool flag16 = CS$<>8__locals1.box.Get<List<int>>("CharacterResult", out charIdList) && charIdList.Count > 0 && this.Data.ExtraData.SelectCharacterData != null;
						if (flag16)
						{
							this.characterListPanel.SelectedCharIdList.AddRange(charIdList);
							this._model.SetSelectCharacterResult(charIdList);
							this.SelectOptionByOptionKey(optionKey);
						}
						else
						{
							short templateId;
							sbyte level;
							bool flag17 = CS$<>8__locals1.box.Get("NormalInformationTemplateId", out templateId) && CS$<>8__locals1.box.Get("NormalInformationLevel", out level);
							if (flag17)
							{
								this._model.SetNormalInformationSelectResult(new NormalInformation(templateId, level));
								this.SelectOptionByOptionKey(optionKey);
							}
							else
							{
								this.SelectOptionByOptionKey(optionKey);
							}
						}
					}
				}
			}
		}

		// Token: 0x060081F2 RID: 33266 RVA: 0x003C8C88 File Offset: 0x003C6E88
		private bool RandomSelection(ArgumentBox box)
		{
			bool flag = this.CheckIsSpecialEvent(box, "SelectItem");
			bool result;
			if (flag)
			{
				bool flag2 = this.IsAutoChooseItem();
				if (flag2)
				{
					List<ItemKey> itemKey = this.RandomSelectionItem();
					foreach (ItemKey item in itemKey)
					{
						this.SetItemSelectCount(item, 1);
					}
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(4U, new Action(this.AutoOnClickConfirm));
				}
				else
				{
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(4U, new Action(this.AutoOnClickCancel));
				}
				result = true;
			}
			else
			{
				bool flag3 = this.CheckIsSpecialEvent(box, "SelectCharacter");
				if (flag3)
				{
					int characterID = this.RandomSelectionCharacter();
					this.characterListPanel.SelectedCharIdList.Add(characterID);
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(4U, new Action(this.AutoOnClickConfirm));
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x060081F3 RID: 33267 RVA: 0x003C8D94 File Offset: 0x003C6F94
		private void ShowCharacterMenu(int charId, int pageIndex, bool isLeftCharacter)
		{
			bool flag = isLeftCharacter && !SingletonObject.getInstance<TutorialChapterModel>().OpenCharacterMenuEnable;
			if (!flag)
			{
				bool flag2 = !this.WaitSelect;
				if (!flag2)
				{
					bool isDisplayingLog = this._isDisplayingLog;
					if (!isDisplayingLog)
					{
						bool isStateMainWorld = UIManager.Instance.IsElementActive(UIElement.StateMainWorld);
						bool isNotAdvancing = SingletonObject.getInstance<BasicGameData>().AdvancingMonthState == 0;
						bool isOnNormalInteractEvent = SingletonObject.getInstance<EventModel>().IsOnNormalInteractEvent;
						bool canOperate = isStateMainWorld && isNotAdvancing && isOnNormalInteractEvent;
						ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
						argBox.Set("CharacterId", charId);
						argBox.Set("CanOperate", canOperate);
						argBox.Set("PreviousView", 5);
						AdventureRemakeModel model = SingletonObject.getInstance<AdventureRemakeModel>();
						bool inAdventure = model.AdventureMajorEventTaiwu.InAdventure;
						if (inAdventure)
						{
							argBox.Set("PreviousView", 2);
						}
						bool flag3 = pageIndex >= 0;
						if (flag3)
						{
							ECharacterSubToggleBase targetPage;
							if (pageIndex != 3)
							{
								if (pageIndex != 7)
								{
									if (pageIndex != 8)
									{
										targetPage = ECharacterSubToggleBase.CharacterBase;
									}
									else
									{
										targetPage = ECharacterSubToggleBase.StoryBase;
									}
								}
								else
								{
									targetPage = ECharacterSubToggleBase.RelationshipBase;
								}
							}
							else
							{
								targetPage = ECharacterSubToggleBase.ItemBase;
							}
							argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(targetPage, ECharacterSubPage.None));
							bool flag4 = pageIndex == 0;
							if (flag4)
							{
								argBox.Set("BaseAttributeIndex", 1);
							}
						}
						UIElement.CharacterMenu.SetOnInitArgs(argBox);
						UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
					}
				}
			}
		}

		// Token: 0x060081F4 RID: 33268 RVA: 0x003C8EFE File Offset: 0x003C70FE
		public void SwitchIsDisplayingLog(bool value)
		{
			this._isDisplayingLog = value;
		}

		// Token: 0x060081F5 RID: 33269 RVA: 0x003C8F07 File Offset: 0x003C7107
		private void AutoOnClickConfirm()
		{
			this.confirm.onClick.Invoke();
		}

		// Token: 0x060081F6 RID: 33270 RVA: 0x003C8F1B File Offset: 0x003C711B
		private void AutoOnClickCancel()
		{
			this.cancel.onClick.Invoke();
		}

		// Token: 0x060081F7 RID: 33271 RVA: 0x003C8F30 File Offset: 0x003C7130
		private List<ItemKey> RandomSelectionItem()
		{
			List<ItemKey> itemKeyList = new List<ItemKey>();
			int randomIndex = Random.Range(0, this.Data.ExtraData.SelectItemData.CanSelectItemList.Count - 1);
			itemKeyList.Clear();
			itemKeyList.Add(this.Data.ExtraData.SelectItemData.CanSelectItemList[randomIndex].Key);
			return itemKeyList;
		}

		// Token: 0x060081F8 RID: 33272 RVA: 0x003C8F9C File Offset: 0x003C719C
		private bool IsAutoChooseItem()
		{
			return this.Data.ExtraData.SelectItemData.CanSelectItemList.Count > 0;
		}

		// Token: 0x060081F9 RID: 33273 RVA: 0x003C8FCC File Offset: 0x003C71CC
		private int RandomSelectionCharacter()
		{
			int randomIndex = Random.Range(0, this.characterListPanel.CanSelectCharIdList.Count - 1);
			return this.characterListPanel.CanSelectCharIdList[randomIndex];
		}

		// Token: 0x060081FA RID: 33274 RVA: 0x003C9008 File Offset: 0x003C7208
		private bool CheckIsSpecialEvent(ArgumentBox box, string eventType)
		{
			bool result;
			box.Get(eventType, out result);
			return result;
		}

		// Token: 0x060081FB RID: 33275 RVA: 0x003C9028 File Offset: 0x003C7228
		public void Refresh()
		{
			bool flag = this.Data == null;
			if (!flag)
			{
				this._animationMaskCompleteAction = null;
				this._animationWindowCompleteAction = null;
				this._animationCgTextureCompleteAction = null;
				this.fadeRoot.alpha = 1f;
				bool needShowCgTexture = !this.Model.EventCgTextureData.Item1 && !string.IsNullOrEmpty(this.Model.EventCgTextureData.Item2);
				sbyte maskControlCode = this.Data.MaskControlCode;
				bool flag2 = maskControlCode == 1 || maskControlCode == 3 || maskControlCode == 4 || maskControlCode == 5;
				if (flag2)
				{
					this._animationWindowCompleteAction = delegate()
					{
						this.UpdateWindow();
						bool flag6 = this.Data.MaskControlCode == 4;
						if (flag6)
						{
							this.AnimMaskToBlackToAlpha();
						}
						else
						{
							bool flag7 = this.Data.MaskControlCode == 5;
							if (flag7)
							{
								this.AnimMaskToAlpha();
							}
							else
							{
								this.AnimMaskToBlack();
							}
						}
					};
					bool flag3 = needShowCgTexture;
					if (flag3)
					{
						this._animationMaskCompleteAction = new Action(this.AnimCgTextureIn);
						this._animationCgTextureCompleteAction = new Action(this.AnimEventWindowIn);
					}
					else
					{
						this._animationMaskCompleteAction = new Action(this.AnimEventWindowIn);
					}
					bool posInViewPort = this._posInViewPort;
					if (posInViewPort)
					{
						this.AnimEventWindowOut();
					}
					else
					{
						this._animationWindowCompleteAction();
					}
				}
				else
				{
					bool flag4 = needShowCgTexture;
					if (flag4)
					{
						this._animationWindowCompleteAction = new Action(this.AnimCgTextureIn);
						this._animationCgTextureCompleteAction = new Action(this.AnimEventWindowIn);
						this._animationCgTextureCompleteAction = (Action)Delegate.Combine(this._animationCgTextureCompleteAction, new Action(this.UpdateWindow));
						bool posInViewPort2 = this._posInViewPort;
						if (posInViewPort2)
						{
							this.AnimEventWindowOut();
						}
						else
						{
							this._animationWindowCompleteAction();
						}
					}
					else
					{
						this.UpdateWindow();
						bool flag5 = !this._posInViewPort;
						if (flag5)
						{
							this.AnimEventWindowIn();
						}
					}
				}
			}
		}

		// Token: 0x060081FC RID: 33276 RVA: 0x003C91D4 File Offset: 0x003C73D4
		public void HideSelf()
		{
			bool flag = this._animationMaskCompleteAction != null;
			if (flag)
			{
				this._animationMaskCompleteAction = (Action)Delegate.Combine(this._animationMaskCompleteAction, new Action(this.QuickHide));
			}
			else
			{
				bool flag2 = this._animationCgTextureCompleteAction != null;
				if (flag2)
				{
					this._animationCgTextureCompleteAction = (Action)Delegate.Combine(this._animationCgTextureCompleteAction, new Action(this.QuickHide));
				}
				else
				{
					bool posInViewPort = this._posInViewPort;
					if (posInViewPort)
					{
						this._animationWindowCompleteAction = new Action(this.QuickHide);
						this.AnimEventWindowOut();
					}
					else
					{
						UIManager.Instance.HideUI(this.Element);
					}
				}
			}
		}

		// Token: 0x060081FD RID: 33277 RVA: 0x003C9288 File Offset: 0x003C7488
		private void StopAllCricketSing()
		{
			CricketView[] allCrickets = base.GetComponentsInChildren<CricketView>(true);
			Array.ForEach<CricketView>(allCrickets, delegate(CricketView e)
			{
				e.StopLoopSing();
			});
		}

		// Token: 0x060081FE RID: 33278 RVA: 0x003C92C4 File Offset: 0x003C74C4
		private IEnumerator DelayNotifyEventWindowRefresh(ArgumentBox argBox)
		{
			while (!base.gameObject.activeInHierarchy)
			{
				yield return new WaitForSeconds(0.1f);
			}
			yield return new WaitForSeconds(0.5f);
			GEvent.OnEvent(UiEvents.EventWindowRefreshComplete, argBox);
			yield break;
		}

		// Token: 0x060081FF RID: 33279 RVA: 0x003C92DC File Offset: 0x003C74DC
		private void UpdateWindow()
		{
			ViewEventWindow.<>c__DisplayClass136_0 CS$<>8__locals1 = new ViewEventWindow.<>c__DisplayClass136_0();
			CS$<>8__locals1.<>4__this = this;
			TaiwuEventDisplayData data = this.Data;
			bool flag = ((data != null) ? data.ExtraData : null) == null;
			if (!flag)
			{
				CS$<>8__locals1.showInput = (this.Data.ExtraData.InputRequestData != null);
				CS$<>8__locals1.showNameInput = (CS$<>8__locals1.showInput && this.Data.ExtraData.InputRequestData.InputDataType == 5);
				CS$<>8__locals1.showSelectItem = (this.Data.ExtraData.SelectItemData != null);
				CS$<>8__locals1.showSelectReadingBookCount = (this.Data.ExtraData.SelectReadingBookCountData != null);
				CS$<>8__locals1.showSelectNeigongLoopingCount = (this.Data.ExtraData.SelectNeigongLoopingCountData != null);
				CS$<>8__locals1.showSelectFuyuFaithCount = (this.Data.ExtraData.SelectFuyuFaithCountData != null);
				CS$<>8__locals1.showSelectFameAction = (this.Data.ExtraData.SelectFameData != null);
				CS$<>8__locals1.showSelectCharacter = (this.Data.ExtraData.SelectCharacterData != null);
				CS$<>8__locals1.showDirectSelectCharacter = (CS$<>8__locals1.showSelectCharacter && this.Data.ExtraData.SelectCharacterData.SelectApprovedTaiwu != null);
				CS$<>8__locals1.showSelectAvatar = (this.Data.ExtraData.SelectOneAvatarRelatedDataList != null);
				CS$<>8__locals1.showOperateArea = (CS$<>8__locals1.showSelectItem | CS$<>8__locals1.showSelectCharacter | CS$<>8__locals1.showInput | CS$<>8__locals1.showSelectAvatar | CS$<>8__locals1.showSelectReadingBookCount | CS$<>8__locals1.showSelectNeigongLoopingCount | CS$<>8__locals1.showSelectFameAction | CS$<>8__locals1.showSelectFuyuFaithCount);
				bool flag2 = CS$<>8__locals1.showSelectCharacter && !CS$<>8__locals1.showDirectSelectCharacter && this.Data.ExtraData.SelectCharacterData.SelectApprovedTaiwu == null;
				if (flag2)
				{
					bool autoSelect = true;
					bool flag3 = autoSelect;
					if (flag3)
					{
						this.characterListPanel.RefreshSelectCharacter(this.Data.ExtraData.SelectCharacterData, this.confirm, true, new Action(CS$<>8__locals1.<UpdateWindow>g__ProcessDisplay|0));
					}
					else
					{
						CS$<>8__locals1.<UpdateWindow>g__ProcessDisplay|0();
					}
				}
				else
				{
					CS$<>8__locals1.<UpdateWindow>g__ProcessDisplay|0();
				}
			}
		}

		// Token: 0x06008200 RID: 33280 RVA: 0x003C94F0 File Offset: 0x003C76F0
		private void HandlerInteractArea()
		{
			bool showSelectItem = this.Data.ExtraData.SelectItemData != null;
			bool showInteractArea = showSelectItem && (this.Data.ExtraData.SelectItemData.ItemOperationType == ItemOperationType.EItemOperationType.ExchangeTools.ToSbyte() || this.Data.ExtraData.SelectItemData.ItemOperationType == ItemOperationType.EItemOperationType.FixItem.ToSbyte());
			this.interactAreaBottomBG.gameObject.SetActive(showInteractArea);
			RectTransform itemScrollViewRectTrans = this.selectItemsPanel.GetComponent<RectTransform>();
			RectTransform upRectTrans = this.itemScrollViewUpPos;
			itemScrollViewRectTrans.anchoredPosition = (showInteractArea ? upRectTrans.anchoredPosition : this._itemScrollViewInitPos);
			itemScrollViewRectTrans.sizeDelta = (showInteractArea ? upRectTrans.sizeDelta : this._itemScrollViewInitDeltaSize);
		}

		// Token: 0x06008201 RID: 33281 RVA: 0x003C95B8 File Offset: 0x003C77B8
		private void HandlerResourceOrSpiritualDebtCost()
		{
			bool showSelectItem = this.Data.ExtraData.SelectItemData != null;
			bool flag = !showSelectItem;
			if (!flag)
			{
				bool showCost = this.Data.ExtraData.SelectItemData.ItemOperationType == ItemOperationType.EItemOperationType.ExchangeTools.ToSbyte() || this.Data.ExtraData.SelectItemData.ItemOperationType == ItemOperationType.EItemOperationType.FixItem.ToSbyte();
				Refers costRefers = this.interactAreaBottomBG.CGet<Refers>("CostRefer");
				costRefers.gameObject.SetActive(showCost);
				bool flag2 = this.Data.ExtraData.SelectItemData.ItemOperationType == ItemOperationType.EItemOperationType.ExchangeTools.ToSbyte();
				if (flag2)
				{
					costRefers.CGet<CImage>("Icon").SetSprite("ui9_back_eventwindow_spiritual", false, null);
					costRefers.CGet<TextMeshProUGUI>("Tips").SetText(LocalStringManager.Get(LanguageKey.LK_MouseTipMedicine_CostProperty).GetFormat(LocalStringManager.Get(LanguageKey.LK_Combat_Result_Area_Debt_Tips)), true);
					this._haveSpiritualDebt = SingletonObject.getInstance<WorldMapModel>().GetCurrAreaSpiritualDebt();
					costRefers.CGet<TextMeshProUGUI>("Have").SetText(WorldMapModel.GetFormatSpiritualDebt(this._haveSpiritualDebt, 0), true);
					this.ExchangeToolSetCostNeed(0);
				}
				else
				{
					bool flag3 = this.Data.ExtraData.SelectItemData.ItemOperationType == ItemOperationType.EItemOperationType.FixItem.ToSbyte();
					if (flag3)
					{
						costRefers.CGet<CImage>("Icon").SetSprite("ui9_back_eventwindow_money", false, null);
						costRefers.CGet<TextMeshProUGUI>("Tips").SetText(LocalStringManager.Get(LanguageKey.LK_MouseTipMedicine_CostProperty).GetFormat(CommonUtils.GetResOrExpName(6)), true);
						this._haveMoney = SingletonObject.getInstance<BuildingModel>().GetResourceCount(ItemSourceType.Resources, 6);
						costRefers.CGet<TextMeshProUGUI>("Have").SetText(CommonUtils.GetDisplayStringForNum(this._haveMoney, 100000), true);
						this.FixItemSetCostNeed(0);
					}
				}
			}
		}

		// Token: 0x06008202 RID: 33282 RVA: 0x003C97A4 File Offset: 0x003C79A4
		public void ExchangeToolSetCostNeed(int value)
		{
			Refers costRefers = this.interactAreaBottomBG.CGet<Refers>("CostRefer");
			TextMeshProUGUI cost = costRefers.CGet<TextMeshProUGUI>("Cost");
			cost.text = CommonUtils.GetDisplayStringForNum(value, 10000);
			costRefers.CGet<TextMeshProUGUI>("Have").text.SetColor((this._haveSpiritualDebt >= value) ? "brightblue" : "brightred");
		}

		// Token: 0x06008203 RID: 33283 RVA: 0x003C980C File Offset: 0x003C7A0C
		public void FixItemSetCostNeed(int value)
		{
			Refers costRefers = this.interactAreaBottomBG.CGet<Refers>("CostRefer");
			costRefers.CGet<TextMeshProUGUI>("Cost").SetText(CommonUtils.GetDisplayStringForNum(value, 100000), true);
			costRefers.CGet<TextMeshProUGUI>("Have").text.SetColor((this._haveMoney >= value) ? "brightblue" : "brightred");
		}

		// Token: 0x06008204 RID: 33284 RVA: 0x003C9874 File Offset: 0x003C7A74
		public void FixItemUpdateConfirm(int value)
		{
			this.confirm.interactable = (this._haveMoney >= value && this.selectItemsPanel.SelectedKeyList.Count >= this.Data.ExtraData.SelectItemData.FilterList.Count);
		}

		// Token: 0x06008205 RID: 33285 RVA: 0x003C98CC File Offset: 0x003C7ACC
		public void ExchangeToolUpdateConfirm(int value)
		{
			this.confirm.interactable = (this._haveSpiritualDebt >= value && this.selectItemsPanel.SelectedKeyList.Count >= this.Data.ExtraData.SelectItemData.FilterList.Count);
		}

		// Token: 0x06008206 RID: 33286 RVA: 0x003C9924 File Offset: 0x003C7B24
		private void UpdateCost()
		{
			bool isFixItem = this._isFixItem;
			if (isFixItem)
			{
				this.UpdateFixItemCost();
			}
			else
			{
				bool isExchangeTools = this._isExchangeTools;
				if (isExchangeTools)
				{
					this.UpdateExchangeToolCost();
				}
			}
		}

		// Token: 0x06008207 RID: 33287 RVA: 0x003C995C File Offset: 0x003C7B5C
		public void UpdateFixItemCost()
		{
			bool isFixItem = this._isFixItem;
			if (isFixItem)
			{
				ItemKey itemKey = (this.selectItemsPanel.SelectedKeyList == null || this.selectItemsPanel.SelectedKeyList.Count == 0) ? ItemKey.Invalid : this.selectItemsPanel.SelectedKeyList[0];
				bool flag = itemKey.IsValid();
				if (flag)
				{
					ItemDomainMethod.AsyncCall.GetRepairItemNeedResourceCount(this, itemKey, delegate(int offset, RawDataPool dataPool)
					{
						int cost = 0;
						Serializer.Deserialize(dataPool, offset, ref cost);
						this.FixItemSetCostNeed(cost);
						this.FixItemUpdateConfirm(cost);
					});
				}
			}
		}

		// Token: 0x06008208 RID: 33288 RVA: 0x003C99D0 File Offset: 0x003C7BD0
		public void UpdateExchangeToolCost()
		{
			bool isExchangeTools = this._isExchangeTools;
			if (isExchangeTools)
			{
				ItemKey itemKey = (this.selectItemsPanel.SelectedKeyList == null || this.selectItemsPanel.SelectedKeyList.Count == 0) ? ItemKey.Invalid : this.selectItemsPanel.SelectedKeyList[0];
				int cost = 0;
				bool flag = itemKey.IsValid();
				if (flag)
				{
					sbyte grade = ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId);
					cost = GameData.Domains.Extra.SharedMethods.GetExchangeToolSpiritualDebtCost(grade);
				}
				this.ExchangeToolSetCostNeed(cost);
				this.ExchangeToolUpdateConfirm(cost);
			}
		}

		// Token: 0x06008209 RID: 33289 RVA: 0x003C9A5C File Offset: 0x003C7C5C
		private void AnimEventWindowIn()
		{
			AudioManager.Instance.PlaySound("ui_default_small_whoosh", false, false);
			this._animating = true;
			this._posInViewPort = true;
			RectTransform windowRoot = this.animationRoot;
			windowRoot.localPosition = Vector3.zero;
			windowRoot.DOKill(false);
			Tween inOutAnimTween = this._inOutAnimTween;
			if (inOutAnimTween != null)
			{
				inOutAnimTween.Kill(false);
			}
			RectTransform animGroupPanelTrans = this.animGroupPanel.transform as RectTransform;
			this.animGroupBg.alpha = 0f;
			this.animGroupPanel.alpha = 0f;
			this.animGroupLeftChara.localPosition = Vector3.left * 800f;
			this.animGroupRightChara.localPosition = Vector3.right * 800f;
			animGroupPanelTrans.localPosition = Vector3.up * 240f;
			Vector2 backPosX = this.eventCharacterBack.LeftRightHolderOriginPosX;
			Vector3 tempPos = this.eventCharacterBack.LeftHolder.localPosition;
			this.eventCharacterBack.LeftHolder.localPosition = new Vector3(backPosX.x - 800f, tempPos.y, tempPos.z);
			tempPos = this.eventCharacterBack.RightHolder.localPosition;
			this.eventCharacterBack.RightHolder.localPosition = new Vector3(backPosX.y + 800f, tempPos.y, tempPos.z);
			this.blackMask_ForDialog.SetAlpha(0f);
			float animDurRatio = this.windowAnimDuration / 0.5f;
			this._inOutAnimTween = DOTween.Sequence().Join(this.animGroupBg.DOFade(1f, this.windowAnimDuration)).Join(this.animGroupPanel.DOFade(1f, this.windowAnimDuration)).Join(this.animGroupLeftChara.DOLocalMove(Vector3.zero, 0.33f * animDurRatio, false)).Join(this.animGroupRightChara.DOLocalMove(Vector3.zero, 0.33f * animDurRatio, false)).Join(this.eventCharacterBack.LeftHolder.DOLocalMoveX(backPosX.x, 0.33f * animDurRatio, false)).Join(this.eventCharacterBack.RightHolder.DOLocalMoveX(backPosX.y, 0.33f * animDurRatio, false)).Join(animGroupPanelTrans.DOLocalMove(Vector3.zero, this.windowAnimDuration, false).SetEase(Ease.OutQuad)).Join(this.blackMask_ForDialog.DOFade(0.7f, 0.16f)).SetUpdate(true).SetAutoKill(true).OnComplete(delegate
			{
				this._animating = false;
				Action animationWindowCompleteAction = this._animationWindowCompleteAction;
				if (animationWindowCompleteAction != null)
				{
					animationWindowCompleteAction();
				}
				this._animationWindowCompleteAction = null;
			}).OnKill(delegate
			{
				this._inOutAnimTween = null;
			});
			CRawImage cgRawImage = this.cGTexture;
			bool enabled = cgRawImage.enabled;
			if (enabled)
			{
				this.maskForCg.SetAlpha(0f);
				this.maskForCg.gameObject.SetActive(true);
				DOVirtual.Float(0f, 0.7f, this.windowAnimDuration, new TweenCallback<float>(this.maskForCg.SetAlpha)).SetAutoKill(true);
			}
			this.RefreshCharacterBack();
		}

		// Token: 0x0600820A RID: 33290 RVA: 0x003C9D78 File Offset: 0x003C7F78
		private void AnimEventWindowOut()
		{
			AudioManager.Instance.PlaySound("ui_default_small_back", false, false);
			this._animating = true;
			this._posInViewPort = false;
			RectTransform windowRoot = this.animationRoot;
			windowRoot.DOKill(false);
			Tween inOutAnimTween = this._inOutAnimTween;
			if (inOutAnimTween != null)
			{
				inOutAnimTween.Kill(false);
			}
			RectTransform animGroupPanelTrans = this.animGroupPanel.transform as RectTransform;
			Vector2 backPosX = this.eventCharacterBack.LeftRightHolderOriginPosX;
			float duration = this.windowAnimDuration * 0.8f;
			float animDurRatio = this.windowAnimDuration / 0.5f;
			this._inOutAnimTween = DOTween.Sequence().Join(this.animGroupBg.DOFade(0f, duration)).Join(this.animGroupLeftChara.DOLocalMove(Vector3.left * 800f, 0.33f * animDurRatio, false)).Join(this.animGroupRightChara.DOLocalMove(Vector3.right * 800f, 0.33f * animDurRatio, false)).Join(this.eventCharacterBack.LeftHolder.DOLocalMoveX(backPosX.x - 800f, 0.33f * animDurRatio, false)).Join(this.eventCharacterBack.RightHolder.DOLocalMoveX(backPosX.y + 800f, 0.33f * animDurRatio, false)).Join(animGroupPanelTrans.DOLocalMove(Vector3.up * 240f, duration, false).SetEase(Ease.OutQuad)).Join(this.blackMask_ForDialog.DOFade(0f, 0.16f)).Join(this.animGroupPanel.DOFade(0f, 0.7f * duration).SetDelay(0.3f * duration)).SetUpdate(true).SetAutoKill(true).OnComplete(delegate
			{
				this._animating = false;
				Action animationWindowCompleteAction = this._animationWindowCompleteAction;
				if (animationWindowCompleteAction != null)
				{
					animationWindowCompleteAction();
				}
				this._animationWindowCompleteAction = null;
			}).OnKill(delegate
			{
				this._inOutAnimTween = null;
			});
			bool activeSelf = this.maskForCg.gameObject.activeSelf;
			if (activeSelf)
			{
				DOVirtual.Float(0.7f, 0f, this.windowAnimDuration, new TweenCallback<float>(this.maskForCg.SetAlpha)).SetUpdate(true).OnComplete(delegate
				{
					this.maskForCg.gameObject.SetActive(false);
				}).SetAutoKill(true);
			}
		}

		// Token: 0x0600820B RID: 33291 RVA: 0x003C9FB4 File Offset: 0x003C81B4
		private void AnimMaskToBlack()
		{
			bool flag = this.Data == null;
			if (!flag)
			{
				this.blackMask.SetAlpha(0f);
				this._animationMaskComplete = false;
				DOVirtual.Float(0f, 1f, (float)this.Data.MaskTweenTime / 100f, new TweenCallback<float>(this.blackMask.SetAlpha)).OnComplete(delegate
				{
					Action animationMaskCompleteAction = this._animationMaskCompleteAction;
					if (animationMaskCompleteAction != null)
					{
						animationMaskCompleteAction();
					}
					this._animationMaskCompleteAction = null;
					this._animationMaskComplete = true;
				}).SetUpdate(true).SetAutoKill(true);
			}
		}

		// Token: 0x0600820C RID: 33292 RVA: 0x003CA03C File Offset: 0x003C823C
		private void AnimMaskToAlpha()
		{
			bool flag = this.Data == null;
			if (!flag)
			{
				this._animationMaskComplete = false;
				DOVirtual.Float(1f, 0f, (float)this.Data.MaskTweenTime / 100f, new TweenCallback<float>(this.blackMask.SetAlpha)).OnComplete(delegate
				{
					Action animationMaskCompleteAction = this._animationMaskCompleteAction;
					if (animationMaskCompleteAction != null)
					{
						animationMaskCompleteAction();
					}
					this._animationMaskCompleteAction = null;
					this._animationMaskComplete = true;
				}).SetUpdate(true).SetAutoKill(true);
			}
		}

		// Token: 0x0600820D RID: 33293 RVA: 0x003CA0B0 File Offset: 0x003C82B0
		private void AnimMaskToBlackToAlpha()
		{
			bool flag = this.Data == null;
			if (!flag)
			{
				this._animationMaskComplete = false;
				this.blackMask.SetAlpha(0f);
				DOVirtual.Float(0f, 1f, (float)this.Data.MaskTweenTime / 100f, new TweenCallback<float>(this.blackMask.SetAlpha)).OnComplete(delegate
				{
					DOVirtual.Float(1f, 0f, (float)this.Data.MaskTweenTime / 100f, new TweenCallback<float>(this.blackMask.SetAlpha)).OnComplete(delegate
					{
						Action animationMaskCompleteAction = this._animationMaskCompleteAction;
						if (animationMaskCompleteAction != null)
						{
							animationMaskCompleteAction();
						}
						this._animationMaskCompleteAction = null;
						this._animationMaskComplete = true;
					}).SetUpdate(true).SetAutoKill(true);
				}).SetUpdate(true).SetAutoKill(true);
			}
		}

		// Token: 0x0600820E RID: 33294 RVA: 0x003CA138 File Offset: 0x003C8338
		private void AnimCgTextureIn()
		{
			bool flag = this.Model.EventCgTextureData.Item1 || string.IsNullOrEmpty(this._model.EventCgTextureData.Item2);
			if (flag)
			{
				Action animationCgTextureCompleteAction = this._animationCgTextureCompleteAction;
				if (animationCgTextureCompleteAction != null)
				{
					animationCgTextureCompleteAction();
				}
				this._animationCgTextureCompleteAction = null;
			}
			else
			{
				string texturePath = "CGTextures/" + this.Model.EventCgTextureData.Item2;
				ResLoader.LoadModOrGameResource<Texture2D>(texturePath, delegate(Texture2D cgTexture)
				{
					this.blackMask.SetAlpha(1f);
					Color c = Color.white;
					c.a = 0f;
					this.cGTexture.texture = cgTexture;
					this.cGTexture.SetNativeSize();
					this.cGTexture.color = c;
					this.cGTexture.enabled = true;
					Sequence sequence = DOTween.Sequence();
					sequence.Append(DOVirtual.Float(0f, 1f, this._model.EventCgTextureData.Item3, delegate(float stepValue)
					{
						c.a = stepValue;
						this.cGTexture.color = c;
					}).SetAutoKill(true));
					sequence.AppendInterval(this.cgShowTime);
					sequence.AppendCallback(delegate
					{
						Action animationCgTextureCompleteAction2 = this._animationCgTextureCompleteAction;
						if (animationCgTextureCompleteAction2 != null)
						{
							animationCgTextureCompleteAction2();
						}
						this._animationCgTextureCompleteAction = null;
					});
					sequence.SetUpdate(true).SetAutoKill(true);
					sequence.Play<Sequence>();
				}, null);
				this.Model.SetCgDataHandled();
			}
		}

		// Token: 0x0600820F RID: 33295 RVA: 0x003CA1CC File Offset: 0x003C83CC
		private void AnimCgTextureOut()
		{
			CRawImage cgRawImage = this.cGTexture;
			bool flag = this.Model.EventCgTextureData.Item1 || !string.IsNullOrEmpty(this._model.EventCgTextureData.Item2) || null == cgRawImage.texture;
			if (flag)
			{
				cgRawImage.enabled = false;
				Action animationCgTextureCompleteAction = this._animationCgTextureCompleteAction;
				if (animationCgTextureCompleteAction != null)
				{
					animationCgTextureCompleteAction();
				}
				this._animationCgTextureCompleteAction = null;
			}
			else
			{
				this.Model.SetCgDataHandled();
				Color c = Color.white;
				c.a = 1f;
				cgRawImage.color = c;
				cgRawImage.enabled = true;
				DOVirtual.Float(1f, 0f, this._model.EventCgTextureData.Item3, delegate(float stepValue)
				{
					c.a = stepValue;
					cgRawImage.color = c;
				}).OnComplete(new TweenCallback(this.HideCgTexture)).SetUpdate(true).SetAutoKill(true);
			}
		}

		// Token: 0x06008210 RID: 33296 RVA: 0x003CA2E8 File Offset: 0x003C84E8
		private void HideCgTexture()
		{
			Action animationCgTextureCompleteAction = this._animationCgTextureCompleteAction;
			if (animationCgTextureCompleteAction != null)
			{
				animationCgTextureCompleteAction();
			}
			this._animationCgTextureCompleteAction = null;
			this.cGTexture.enabled = false;
			this.cGTexture.texture = null;
		}

		// Token: 0x06008211 RID: 33297 RVA: 0x003CA320 File Offset: 0x003C8520
		private void AdjustContentScrollView()
		{
			this.contentScrollView.ScrollBar.value = 0f;
			this.contentScrollView.Content.anchoredPosition = new Vector2(0f, -this.contentScrollView.Content.sizeDelta.y);
		}

		// Token: 0x06008212 RID: 33298 RVA: 0x003CA378 File Offset: 0x003C8578
		public static void UpdateTexture(string textureName, CRawImage eventRawImage, EventTextureManager eventTextureManager, string textureDirectory, string errorInfo = "")
		{
			bool flag = string.IsNullOrEmpty(textureName);
			if (flag)
			{
				WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
				MapBlockData blockData = worldMapModel.GetBlockData(worldMapModel.CurrentBlockId);
				textureName = blockData.GetConfig().EventBack;
			}
			else
			{
				bool flag2 = SingletonObject.getInstance<DlcManager>().SetEventBackTexture(eventRawImage, textureName);
				if (flag2)
				{
					return;
				}
			}
			bool flag3 = string.IsNullOrEmpty(textureName);
			if (flag3)
			{
				GLog.TagError("UpdateTexture", "failed to set event " + errorInfo + " 's texture,event texture name is empty!", Array.Empty<object>());
			}
			else
			{
				string texturePath;
				bool eventBackPath = eventTextureManager.GetEventBackPath(textureName, out texturePath);
				if (eventBackPath)
				{
					ResLoader.LoadModOrGameResource<Texture2D>(texturePath, delegate(Texture2D texture)
					{
						eventRawImage.texture = texture;
						eventRawImage.enabled = true;
					}, null);
				}
				else
				{
					bool flag4 = File.Exists(textureName);
					if (flag4)
					{
						using (FileStream stream = new FileStream(textureName, FileMode.Open, FileAccess.Read))
						{
							byte[] buffer = new byte[stream.Length];
							int read = stream.Read(buffer, 0, buffer.Length);
							Texture2D texture2D = new Texture2D(0, 0);
							texture2D.LoadImage(buffer);
							texture2D.name = textureName;
							eventRawImage.texture = texture2D;
							eventRawImage.enabled = true;
						}
					}
					else
					{
						textureName = Path.GetFileNameWithoutExtension(textureName);
						bool eventBackPath2 = eventTextureManager.GetEventBackPath(textureName, out texturePath);
						if (eventBackPath2)
						{
							texturePath = Path.Combine(textureDirectory, texturePath);
							ResLoader.Load<Texture2D>(texturePath, delegate(Texture2D texture)
							{
								eventRawImage.texture = texture;
								eventRawImage.enabled = true;
							}, null, false);
						}
					}
				}
			}
		}

		// Token: 0x06008213 RID: 33299 RVA: 0x003CA50C File Offset: 0x003C870C
		private void UpdateContent()
		{
			string content = this.Data.EventContent;
			List<string> extraFormatLanguageKeys = this.Data.ExtraFormatLanguageKeys;
			bool flag = extraFormatLanguageKeys != null && extraFormatLanguageKeys.Count > 0;
			if (flag)
			{
				object[] array = this.Data.ExtraFormatLanguageKeys.ConvertAll<string>(new Converter<string, string>(LocalStringManager.Get)).ToArray();
				object[] formatArgs = array;
				content = content.GetFormat(formatArgs);
			}
			this.eventContent.text = content.ColorReplace();
			this.eventContent.SetLayoutDirty();
			this.ApplyContentFontSize();
			this.noContent.SetActive(string.IsNullOrEmpty(content));
			base.DelayFrameCall(delegate
			{
				this.AdjustContentScrollView();
				this.ApplyContentFontSize();
			}, 2U);
		}

		// Token: 0x06008214 RID: 33300 RVA: 0x003CA5C0 File Offset: 0x003C87C0
		public static sbyte OptionBehaviorToCharacterBehavior(sbyte optionBehavior)
		{
			return optionBehavior - 1;
		}

		// Token: 0x06008215 RID: 33301 RVA: 0x003CA5C6 File Offset: 0x003C87C6
		public static bool CheckBehaviorCondition(sbyte charBehaviorType, sbyte optionBehaviorType)
		{
			return GameData.Domains.Character.BehaviorType.IsCloseOrSame(charBehaviorType, optionBehaviorType);
		}

		// Token: 0x06008216 RID: 33302 RVA: 0x003CA5D0 File Offset: 0x003C87D0
		private void RefreshOptionScrollByCurrentState()
		{
			bool flag = !base.isActiveAndEnabled || this.Data == null || this.optionContainer == null;
			if (!flag)
			{
				List<EventOptionInfo> optionInfos = this.Data.EventOptionInfos ?? new List<EventOptionInfo>();
				this.UpdateOptionScroll(optionInfos, (int)this.Data.EscOptionIndex, this._needPlayToggleGroupAnim);
				this._needPlayToggleGroupAnim = false;
			}
		}

		// Token: 0x06008217 RID: 33303 RVA: 0x003CA638 File Offset: 0x003C8838
		private void UpdateOptionScroll(IReadOnlyList<EventOptionInfo> optionInfos, int escOptionIndex, bool playToggleAnim)
		{
			this.WaitSelect = true;
			RectTransform optionRoot = this.optionContainer;
			EventWindowOption[] prevRefers = optionRoot.GetComponentsInTopChildren(true);
			for (int i = 0; i < prevRefers.Length; i++)
			{
				this._optionPool.DestroyObject(prevRefers[i].gameObject);
			}
			sbyte taiwuBehaviorType = GameData.Domains.Character.BehaviorType.GetBehaviorType(this._taiwuDetailInfoMonitor.Behavior);
			int strMaxCount = this.GetMaxLabelTextCount(optionInfos.Count, escOptionIndex);
			for (int index = 0; index < optionInfos.Count; index++)
			{
				EventOptionInfo optionInfo = optionInfos[index];
				EventWindowOption refer = this._optionPool.GetObject().GetComponent<EventWindowOption>();
				refer.transform.SetParent(optionRoot, false);
				refer.gameObject.SetActive(true);
				refer.transform.SetAsLastSibling();
				refer.name = optionInfo.OptionKey;
				string commandKey = (index >= this.NormalOptionHotKeyCount) ? string.Empty : this._optionHotKeyCommands[index].ToString();
				bool flag = index == escOptionIndex;
				if (flag)
				{
					commandKey = this._optionHotKeyCommands.Last<HotKeyCommand>().ToString();
				}
				refer.Setup(optionInfo, commandKey, strMaxCount, taiwuBehaviorType, this._taiwuDetailInfoMonitor.Behavior, new Action<EventOptionInfo>(this.SelectOption));
			}
			if (playToggleAnim)
			{
				this._toggleGroupAnim.CallAnim();
			}
		}

		// Token: 0x06008218 RID: 33304 RVA: 0x003CA79C File Offset: 0x003C899C
		private int GetMaxLabelTextCount(int optionCount, int escOptionIndex)
		{
			int strMaxCount = 0;
			int count = Math.Min(optionCount, this.NormalOptionHotKeyCount);
			for (int index = 0; index < count; index++)
			{
				string commandKey = this._optionHotKeyCommands[index].ToString();
				bool flag = index == escOptionIndex;
				if (flag)
				{
					commandKey = this._optionHotKeyCommands.Last<HotKeyCommand>().ToString();
				}
				bool flag2 = commandKey.Length > strMaxCount;
				if (flag2)
				{
					strMaxCount = commandKey.Length;
				}
			}
			return strMaxCount;
		}

		// Token: 0x06008219 RID: 33305 RVA: 0x003CA814 File Offset: 0x003C8A14
		private void SelectEscOption()
		{
			RectTransform operateAreaTrans = this.operateArea;
			bool flag = this.Data.EscOptionIndex < 0;
			if (!flag)
			{
				bool flag2 = !this._animating && this.SelectOptionByIndex((int)this.Data.EscOptionIndex, true) && operateAreaTrans.gameObject.activeSelf;
				if (flag2)
				{
					this.ResetData();
				}
			}
		}

		// Token: 0x0600821A RID: 33306 RVA: 0x003CA874 File Offset: 0x003C8A74
		private void SelectOptionByOptionKey(string optionKey)
		{
			bool flag = this.Data == null;
			if (!flag)
			{
				int i = 0;
				int max = this.Data.EventOptionInfos.Count;
				while (i < max)
				{
					bool flag2 = this.Data.EventOptionInfos[i].OptionKey == optionKey;
					if (flag2)
					{
						this.SelectOption(this.Data.EventOptionInfos[i]);
						break;
					}
					i++;
				}
			}
		}

		// Token: 0x0600821B RID: 33307 RVA: 0x003CA8F4 File Offset: 0x003C8AF4
		private bool SelectOptionByIndex(int index, bool isEsc = false)
		{
			bool flag = this.Data == null || !this.Data.EventOptionInfos.CheckIndex(index) || (!isEsc && (this.Data.ExtraData.InputRequestData != null || this.Data.ExtraData.SelectItemData != null || this.Data.ExtraData.SelectCharacterData != null || this.Data.ExtraData.SelectNeigongLoopingCountData != null || this.Data.ExtraData.SelectReadingBookCountData != null || this.Data.ExtraData.SelectFuyuFaithCountData != null));
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.SelectOption(this.Data.EventOptionInfos[index]);
				result = true;
			}
			return result;
		}

		// Token: 0x0600821C RID: 33308 RVA: 0x003CA9C0 File Offset: 0x003C8BC0
		private void SelectOption(EventOptionInfo optionInfo)
		{
			ViewEventWindow.<>c__DisplayClass168_0 CS$<>8__locals1 = new ViewEventWindow.<>c__DisplayClass168_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.optionInfo = optionInfo;
			bool flag = !this.CanSelect || CS$<>8__locals1.optionInfo.OptionState == -1;
			if (!flag)
			{
				this._selectedOptionInfo = CS$<>8__locals1.optionInfo;
				this.buttons.SetActive(false);
				bool flag2 = CS$<>8__locals1.optionInfo.Behavior != 0;
				if (flag2)
				{
					bool flag3 = !EventModel.IgnoreEventBehavior && SingletonObject.getInstance<BasicGameData>().RestrictOptionsBehaviorType;
					if (flag3)
					{
						sbyte taiwuBehavior = GameData.Domains.Character.BehaviorType.GetBehaviorType(this._taiwuDetailInfoMonitor.Behavior);
						bool flag4 = Mathf.Abs((int)(ViewEventWindow.OptionBehaviorToCharacterBehavior(CS$<>8__locals1.optionInfo.Behavior) - taiwuBehavior)) > 1;
						if (flag4)
						{
							return;
						}
					}
				}
				bool flag5 = this.Model.SelectCombatSkillData != null;
				if (flag5)
				{
					bool flag6 = CS$<>8__locals1.optionInfo.OptionKey == this.Model.SelectCombatSkillData.OptionKey;
					if (flag6)
					{
						bool flag7 = this.Model.SelectCombatSkillData.SelectResultIndex < 0;
						if (flag7)
						{
							ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
							argBox.Set("CharId", this.Model.SelectCombatSkillData.CharId);
							argBox.Set("ShowCombatSkill", true);
							argBox.Set("ShowNone", false);
							argBox.SetObject("UnselectableCombatSkillList", new List<short>());
							argBox.SetObject("CombatSkillIdList", this.Model.SelectCombatSkillData.CanSelectCombatSkillIdList);
							argBox.SetObject("Callback", new Action<sbyte, short>(delegate(sbyte _, short skillId)
							{
								CS$<>8__locals1.<>4__this.Model.SelectCombatSkillData.SelectResultIndex = CS$<>8__locals1.<>4__this.Model.SelectCombatSkillData.CanSelectCombatSkillIdList.IndexOf(skillId);
								TaiwuEventDomainMethod.Call.SetCombatSkillSelectResult(skillId);
								ArgumentBox box = EasyPool.Get<ArgumentBox>();
								box.Set("CombatSkillId", skillId);
								GEvent.OnEvent(UiEvents.EventWindowAppearResult, box);
								CS$<>8__locals1.<>4__this.SelectOptionByOptionKey(CS$<>8__locals1.<>4__this.Model.SelectCombatSkillData.OptionKey);
							}));
							argBox.Set("IsShowNeiLiFinish", false);
							UIElement.SelectSkill.SetOnInitArgs(argBox);
							UIManager.Instance.MaskUI(UIElement.SelectSkill);
							return;
						}
					}
					else
					{
						TaiwuEventDomainMethod.Call.SetCombatSkillSelectResult(-1);
					}
				}
				bool flag8 = this.Model.SelectLifeSkillData != null;
				if (flag8)
				{
					bool flag9 = CS$<>8__locals1.optionInfo.OptionKey == this.Model.SelectLifeSkillData.OptionKey;
					if (flag9)
					{
						bool flag10 = this.Model.SelectLifeSkillData.SelectResultIndex < 0;
						if (flag10)
						{
							ArgumentBox argBox2 = EasyPool.Get<ArgumentBox>();
							argBox2.Set("CharId", this.Model.SelectLifeSkillData.CharId);
							argBox2.Set("ShowLifeSkill", true);
							argBox2.Set("ShowNone", false);
							argBox2.SetObject("UnselectableLifeSkillList", new List<short>());
							argBox2.SetObject("LifeSkillIdList", this.Model.SelectLifeSkillData.CanSelectLifeSkillIdList);
							argBox2.SetObject("Callback", new Action<sbyte, short>(delegate(sbyte _, short skillId)
							{
								CS$<>8__locals1.<>4__this.Model.SelectLifeSkillData.SelectResultIndex = CS$<>8__locals1.<>4__this.Model.SelectLifeSkillData.CanSelectLifeSkillIdList.IndexOf(skillId);
								TaiwuEventDomainMethod.Call.SetLifeSkillSelectResult(skillId);
								ArgumentBox box = EasyPool.Get<ArgumentBox>();
								box.Set("LifeSkillId", skillId);
								GEvent.OnEvent(UiEvents.EventWindowAppearResult, box);
								CS$<>8__locals1.<>4__this.SelectOptionByOptionKey(CS$<>8__locals1.<>4__this.Model.SelectLifeSkillData.OptionKey);
							}));
							argBox2.Set("IsShowNeiLiFinish", false);
							UIElement.SelectSkill.SetOnInitArgs(argBox2);
							UIManager.Instance.MaskUI(UIElement.SelectSkill);
							return;
						}
					}
					else
					{
						TaiwuEventDomainMethod.Call.SetLifeSkillSelectResult(-1);
					}
				}
				bool needFadeOutCg = !this.Model.EventCgTextureData.Item1 && string.IsNullOrEmpty(this.Model.EventCgTextureData.Item2);
				sbyte maskControlCode = this.Data.MaskControlCode;
				bool flag11 = maskControlCode == 2 || maskControlCode == 3 || maskControlCode == 6;
				if (flag11)
				{
					bool flag12 = needFadeOutCg;
					if (flag12)
					{
						this._animationWindowCompleteAction = new Action(this.AnimCgTextureOut);
						this._animationCgTextureCompleteAction = ((this.Data.MaskControlCode == 6) ? new Action(this.AnimMaskToBlack) : new Action(this.AnimMaskToAlpha));
						this._animationMaskCompleteAction = new Action(CS$<>8__locals1.<SelectOption>g__OptionSelect|0);
					}
					else
					{
						this._animationWindowCompleteAction = ((this.Data.MaskControlCode == 6) ? new Action(this.AnimMaskToBlack) : new Action(this.AnimMaskToAlpha));
						this._animationMaskCompleteAction = new Action(CS$<>8__locals1.<SelectOption>g__OptionSelect|0);
					}
					this.AnimEventWindowOut();
				}
				else
				{
					bool flag13 = needFadeOutCg;
					if (flag13)
					{
						this._animationWindowCompleteAction = new Action(this.AnimCgTextureOut);
						this._animationCgTextureCompleteAction = new Action(CS$<>8__locals1.<SelectOption>g__OptionSelect|0);
						this.AnimEventWindowOut();
					}
					else
					{
						CS$<>8__locals1.<SelectOption>g__OptionSelect|0();
					}
				}
				this.WaitSelect = false;
			}
		}

		// Token: 0x0600821D RID: 33309 RVA: 0x003CAE14 File Offset: 0x003C9014
		private void SetSelecting(bool selecting)
		{
			ConchShipCursor.Instance.SetSelectCountActive(false);
		}

		// Token: 0x0600821E RID: 33310 RVA: 0x003CAE22 File Offset: 0x003C9022
		private void SetSelectedCount(int selectedCount)
		{
			ConchShipCursor.Instance.SetSelectCountCur(selectedCount, null);
		}

		// Token: 0x0600821F RID: 33311 RVA: 0x003CAE31 File Offset: 0x003C9031
		private void SetSelectTargetCount(int targetCount)
		{
			ConchShipCursor.Instance.SetSelectCountMax(targetCount, null);
		}

		// Token: 0x06008220 RID: 33312 RVA: 0x003CAE40 File Offset: 0x003C9040
		private void SetSelectingApprovedTaiwu(bool selecting)
		{
			ConchShipCursor.Instance.SetSelectApprovedTaiwuActive(selecting);
		}

		// Token: 0x06008221 RID: 33313 RVA: 0x003CAE4E File Offset: 0x003C904E
		private void SetSelectedApprovedTaiwu(string selectedCount)
		{
			ConchShipCursor.Instance.SetSelectApprovedTaiwuCur(selectedCount);
		}

		// Token: 0x06008222 RID: 33314 RVA: 0x003CAE5C File Offset: 0x003C905C
		private void SetSelectTargetApprovedTaiwu(string targetCount)
		{
			ConchShipCursor.Instance.SetSelectApprovedTaiwuMax(targetCount);
		}

		// Token: 0x17000E42 RID: 3650
		// (get) Token: 0x06008223 RID: 33315 RVA: 0x003CAE6C File Offset: 0x003C906C
		private bool NeedShowSelectingInItemSelectMode
		{
			get
			{
				bool result;
				if (!this._isMultiItemSelect)
				{
					TaiwuEventDisplayData data = this.Data;
					bool flag;
					if (data == null)
					{
						flag = (null != null);
					}
					else
					{
						TaiwuEventDisplayExtraData extraData = data.ExtraData;
						flag = (((extraData != null) ? extraData.SelectItemData : null) != null);
					}
					result = (!flag || this.Data.ExtraData.SelectItemData.FilterList.Count != 1);
				}
				else
				{
					result = false;
				}
				return result;
			}
		}

		// Token: 0x06008224 RID: 33316 RVA: 0x003CAEC8 File Offset: 0x003C90C8
		private void RefreshItemSelectScroll()
		{
			this._isExchangeTools = (this.Data.ExtraData.SelectItemData.ItemOperationType == ItemOperationType.EItemOperationType.ExchangeTools.ToSbyte());
			this._isFixItem = (this.Data.ExtraData.SelectItemData.ItemOperationType == ItemOperationType.EItemOperationType.FixItem.ToSbyte());
			bool autoSelect = true;
			this.selectItemsPanel.Refresh(this.Data.ExtraData.SelectItemData, this.Data.ExtraData.ShowProfessionReview, this.confirm, new Action(this.RefreshButtonAndCost), autoSelect);
			this.cancel.gameObject.SetActive(this.Data.EventOptionInfos.Count == 2);
			this.confirm.interactable = false;
			this.RefreshConfirmButtonTips();
			this.WaitSelect = true;
			this._selectedItemList.Clear();
			this._selectedItemDict.Clear();
			this.SetSelectedCount(0);
			this.SetSelectTargetCount(this.Data.ExtraData.SelectItemData.FilterList.Count);
			this.SetSelecting(this.NeedShowSelectingInItemSelectMode);
			this._needSelectItem = true;
			this.buttons.SetActive(true);
		}

		// Token: 0x06008225 RID: 33317 RVA: 0x003CB010 File Offset: 0x003C9210
		private void CheckItemSelectAvailable()
		{
			this.confirm.interactable = (this._selectedItemList.Count > 0 && this.Data.ExtraData.SelectItemData.IsAvailableSelectResult(this._selectedItemList));
			this.SetSelectedCount(this._selectedItemList.Count);
			this.RefreshConfirmButtonTips();
		}

		// Token: 0x06008226 RID: 33318 RVA: 0x003CB06F File Offset: 0x003C926F
		private void RefreshButtonAndCost()
		{
			this.UpdateCost();
			this.RefreshConfirmButtonTips();
		}

		// Token: 0x06008227 RID: 33319 RVA: 0x003CB080 File Offset: 0x003C9280
		private void RefreshConfirmButtonTips()
		{
			string targetKey = null;
			bool flag = this.Data.ExtraData.SelectItemData != null;
			if (flag)
			{
				targetKey = this.Data.ExtraData.SelectItemData.ConfirmDisableTips;
			}
			else
			{
				bool flag2 = this.Data.ExtraData.InputRequestData != null;
				if (flag2)
				{
					targetKey = this.Data.ExtraData.InputRequestData.ConfirmDisableTips;
				}
			}
			bool flag3 = targetKey.IsNullOrEmpty();
			if (!flag3)
			{
				TooltipInvoker tipDisplayer = this.confirm.GetComponent<TooltipInvoker>();
				tipDisplayer.enabled = !this.confirm.interactable;
				tipDisplayer.Type = TipType.SingleDesc;
				TooltipInvoker tooltipInvoker = tipDisplayer;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(targetKey));
			}
		}

		// Token: 0x06008228 RID: 33320 RVA: 0x003CB15C File Offset: 0x003C935C
		private void SetItemSelectCount(ItemKey itemKey, int count)
		{
			bool flag = this.Data.ExtraData.SelectItemData.FilterList.Count == 1;
			if (flag)
			{
				this._selectedItemDict.Clear();
				this._selectedItemList.Clear();
				bool flag2 = count > 0;
				if (flag2)
				{
					this._selectedItemList.Add(itemKey);
				}
			}
			else
			{
				bool isMiscResource = ItemTemplateHelper.IsMiscResource(itemKey.ItemType, itemKey.TemplateId);
				bool flag3 = isMiscResource;
				if (flag3)
				{
					bool flag4 = this._selectedItemList.Contains(itemKey);
					if (flag4)
					{
						bool flag5 = count == 0;
						if (flag5)
						{
							this._selectedItemList.Remove(itemKey);
						}
					}
					else
					{
						this._selectedItemList.Add(itemKey);
					}
				}
				else
				{
					int selectedCount = 0;
					for (int i = this._selectedItemList.Count - 1; i >= 0; i--)
					{
						bool flag6 = this._selectedItemList[i].Equals(itemKey);
						if (flag6)
						{
							bool flag7 = selectedCount >= count;
							if (flag7)
							{
								this._selectedItemList.RemoveAt(i);
							}
							else
							{
								selectedCount++;
							}
						}
					}
					while (selectedCount < count)
					{
						bool flag8 = this._selectedItemList.Count >= this.Data.ExtraData.SelectItemData.FilterList.Count;
						if (flag8)
						{
							break;
						}
						this._selectedItemList.Add(itemKey);
						selectedCount++;
					}
				}
			}
			bool flag9 = count > 0;
			if (flag9)
			{
				this._selectedItemDict[itemKey] = count;
			}
			else
			{
				this._selectedItemDict.Remove(itemKey);
			}
			this.CheckItemSelectAvailable();
		}

		// Token: 0x06008229 RID: 33321 RVA: 0x003CB30C File Offset: 0x003C950C
		[return: TupleElementNames(new string[]
		{
			"love",
			"hate"
		})]
		private ValueTuple<bool, bool> GetItemLoveAndHateFlag(ItemDisplayData itemDisplayData)
		{
			short subType = ItemTemplateHelper.GetItemSubType(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId);
			CharacterLoveAndHateItemInfo info = this.Data.ExtraData.SelectItemData.LoveAndHateItemInfo;
			return new ValueTuple<bool, bool>(info != null && info.LovingItemRevealed && info.LovingItemSubType == subType, info != null && info.HatingItemRevealed && info.HatingItemSubType == subType);
		}

		// Token: 0x0600822A RID: 33322 RVA: 0x003CB384 File Offset: 0x003C9584
		private void UpdateItemList(EventCharacterItemList itemList, ItemDisplayData[] itemDisplayDataArray, bool showForCricketBattleGuess, sbyte gradeOfCharacter)
		{
			int elemCount = 0;
			int visibleIndex = -1;
			string jarGradeIcon = string.Empty;
			if (showForCricketBattleGuess)
			{
				visibleIndex = Random.Range(0, 3);
				jarGradeIcon = Misc.Instance.GetItem((short)(91 + gradeOfCharacter)).Icon;
			}
			itemList.gameObject.SetActive(true);
			for (int i = 0; i < 3; i++)
			{
				bool needShowElement = itemDisplayDataArray[i] != null;
				elemCount += (needShowElement ? 1 : 0);
				GameObject elemObj = itemList.items[i];
				elemObj.SetActive(needShowElement);
				bool flag = !needShowElement;
				if (!flag)
				{
					CImage jarImg = itemList.cricketJars[i];
					CardItem itemView = itemList.cardItems[i];
					RowItemMain rowItemMain = itemView.GetComponentInChildren<RowItemMain>();
					rowItemMain.SetData(itemDisplayDataArray[i]);
					itemView.Set(rowItemMain, true);
					rowItemMain.ItemBack.SetCountVisible(false);
					bool flag2 = this.Model.CoverCricketJarGradeList != null && i != visibleIndex;
					if (flag2)
					{
						jarGradeIcon = Misc.Instance.GetItem((short)(91 + this.Model.CoverCricketJarGradeList[i])).Icon;
					}
					jarImg.SetSprite((i == visibleIndex) ? string.Empty : jarGradeIcon, false, null);
					bool flag3 = showForCricketBattleGuess || (itemDisplayDataArray[i].Key.ItemType == 11 && itemDisplayDataArray[i].Durability > 0);
					if (flag3)
					{
						CricketViewNew cricketView = itemView.CricketView;
						SkeletonGraphic graphic = cricketView.skeletonGraphic;
						CImage singImage = cricketView.GetSingImage();
						Canvas canvas = singImage.gameObject.GetOrAddComponent<Canvas>();
						canvas.overrideSorting = true;
						canvas.sortingLayerName = "UI";
						canvas.sortingOrder = 699;
						graphic.color = graphic.color.SetAlpha((float)((i == visibleIndex) ? 1 : 0));
						bool isVisibleCricket = i == visibleIndex;
						Action<float> <>9__1;
						SingletonObject.getInstance<YieldHelper>().DelaySecondsDo((float)Random.Range(1, cricketView.Level + 1), delegate
						{
							CricketViewNew cricketView = cricketView;
							bool loop = true;
							bool playSound = true;
							bool isVisibleCricket = isVisibleCricket;
							float singSize = 1f + (float)cricketView.Level * 0.15f;
							Action<float> onSingStart;
							if ((onSingStart = <>9__1) == null)
							{
								onSingStart = (<>9__1 = delegate(float _)
								{
									bool flag5 = !isVisibleCricket;
									if (flag5)
									{
										jarImg.rectTransform.DOPunchRotation(Vector3.forward * (float)(2 + cricketView.Level), 1.2f, 5, 0.2f);
									}
								});
							}
							cricketView.Sing(loop, playSound, isVisibleCricket, singSize, onSingStart, 0f);
						});
					}
					bool shouldEnableTips = string.IsNullOrEmpty(jarGradeIcon);
					if (showForCricketBattleGuess)
					{
						shouldEnableTips = (i == visibleIndex);
					}
					itemView.GetComponent<TooltipInvoker>().enabled = shouldEnableTips;
				}
			}
			itemList.gameObject.SetActive(elemCount > 0);
			bool flag4 = elemCount > 0;
			if (flag4)
			{
				float size = (float)(elemCount * 114 + (elemCount - 1) * 10);
				RectTransform rectTransform = itemList.transform as RectTransform;
				if (rectTransform != null)
				{
					rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
				}
			}
		}

		// Token: 0x0600822B RID: 33323 RVA: 0x003CB634 File Offset: 0x003C9834
		private void RefreshReadingBookCountPanel()
		{
			this.buttons.SetActive(true);
			this.WaitSelect = true;
			int maxCount = this.Data.ExtraData.SelectReadingBookCountData.MaxReadingBookCount;
			this.panelSelectBookCount.Refresh(maxCount, 1);
		}

		// Token: 0x0600822C RID: 33324 RVA: 0x003CB67C File Offset: 0x003C987C
		private void RefreshNeigongLoopingCountPanel()
		{
			this.buttons.SetActive(true);
			this.WaitSelect = true;
			this.panelSelectNeigongLoopingCount.Refresh(this.Data.ExtraData.SelectNeigongLoopingCountData.SelectedCombatSkill, this.Data.ExtraData.SelectNeigongLoopingCountData.MaxLoopingCount, 1);
		}

		// Token: 0x0600822D RID: 33325 RVA: 0x003CB6D8 File Offset: 0x003C98D8
		private void RefreshSelectFameActionPanel()
		{
			this.buttons.SetActive(true);
			this.confirm.interactable = false;
			this.fameActionPanel.Refresh(this.Data.ExtraData.SelectFameData.fameActionRecords, new Action(this.RefreshConfirmButtonTips), this.confirm);
			this.WaitSelect = true;
		}

		// Token: 0x0600822E RID: 33326 RVA: 0x003CB73B File Offset: 0x003C993B
		private void RefreshCharacterSelectScroll()
		{
			this.RefreshConfirmButtonTips();
			this._needSelectItem = true;
			this.WaitSelect = true;
			this.buttons.SetActive(true);
		}

		// Token: 0x0600822F RID: 33327 RVA: 0x003CB761 File Offset: 0x003C9961
		private void OnCharacterScrollItemRender(int index, GameObject go)
		{
			this.OnAvatarItemRender(index, go);
		}

		// Token: 0x06008230 RID: 33328 RVA: 0x003CB76D File Offset: 0x003C996D
		private void OnCharacterScrollItemHide(GameObject go)
		{
			this.OnAvatarItemHide(go);
		}

		// Token: 0x06008231 RID: 33329 RVA: 0x003CB778 File Offset: 0x003C9978
		private void RefreshCharacterSelectApprovedTaiwuScroll()
		{
			this.directSelectCharacterPanel.RefreshShowApprove(this.Data.ExtraData.SelectCharacterData.SelectApprovedTaiwu, this.confirm);
			this.RefreshConfirmButtonTips();
			this._needSelectApprovedTaiwu = true;
			this.WaitSelect = true;
			this.buttons.SetActive(true);
		}

		// Token: 0x06008232 RID: 33330 RVA: 0x003CB7D0 File Offset: 0x003C99D0
		private bool IsCharacterHaveDukeTitle(int charId)
		{
			return this.Data.ExtraData.SelectCharacterData.SelectApprovedTaiwu.DukeTitleCharIdList != null && this.Data.ExtraData.SelectCharacterData.SelectApprovedTaiwu.DukeTitleCharIdList.Contains(charId);
		}

		// Token: 0x06008233 RID: 33331 RVA: 0x003CB82B File Offset: 0x003C9A2B
		private void RefreshInputPanel()
		{
			this.panelInput.Refresh(this.confirm, this.cancel, this.buttons, delegate(bool flag)
			{
				this.WaitSelect = flag;
			}, new Action(this.RefreshConfirmButtonTips));
			this.RefreshConfirmButtonTips();
		}

		// Token: 0x06008234 RID: 33332 RVA: 0x003CB86B File Offset: 0x003C9A6B
		private void RefreshInputNamePanel()
		{
			this.panelInputChangeName.Refresh(this.confirm, this.cancel, this.buttons, delegate(bool flag)
			{
				this.WaitSelect = flag;
			}, new Action(this.RefreshConfirmButtonTips));
			this.RefreshConfirmButtonTips();
		}

		// Token: 0x06008235 RID: 33333 RVA: 0x003CB8AC File Offset: 0x003C9AAC
		private void RefreshSelectAvatarScroll()
		{
			this.directSelectCharacterPanel.RefreshShowAvatar(this.Data.ExtraData.SelectOneAvatarRelatedDataList, this.confirm, delegate(int index)
			{
				bool flag = index != 1;
				if (flag)
				{
					int prevIndex = this._selectedAvatarIndex;
					this._selectedAvatarIndex = index;
					this.confirm.interactable = true;
					this.RefreshConfirmButtonTips();
				}
				else
				{
					this.confirm.interactable = false;
					this.RefreshConfirmButtonTips();
					this._selectedAvatarIndex = index;
				}
			});
			this.RefreshConfirmButtonTips();
			this._needSelectItem = true;
			this.WaitSelect = true;
			this.buttons.SetActive(true);
		}

		// Token: 0x06008236 RID: 33334 RVA: 0x003CB90C File Offset: 0x003C9B0C
		private void OnAvatarItemRender(int index, GameObject go)
		{
			Refers refersHolder = go.GetComponent<Refers>();
			Refers refers = refersHolder.CGet<Refers>("SelectableCharacter");
			refers.CGet<GameObject>("Selected").SetActive(index == this._selectedAvatarIndex);
			Refers approvingRateRefers = refers.CGet<Refers>("ApprovingRateHolder");
			approvingRateRefers.gameObject.SetActive(false);
			AvatarRelatedData data = this.Data.ExtraData.SelectOneAvatarRelatedDataList[index];
			refers.GetComponent<SelectableCharacter>().SetIsDeadCharacter(false);
			Game.Components.Avatar.Avatar avatar = refers.CGet<Game.Components.Avatar.Avatar>("Avatar");
			CToggleObsolete toggle = refers.CGet<CToggleObsolete>("Toggle");
			toggle.onValueChanged.RemoveAllListeners();
			toggle.isOn = (index == this._selectedAvatarIndex);
			TextMeshProUGUI nameLabel = refers.CGet<TextMeshProUGUI>("Name");
			nameLabel.text = "? ? ?";
			bool flag = data == null;
			if (flag)
			{
				avatar.ResetToBlank(false);
				toggle.interactable = false;
			}
			else
			{
				data.AvatarData.ClothDisplayId = data.ClothingDisplayId;
				avatar.Refresh(data.AvatarData, data.DisplayAge);
				toggle.interactable = true;
				toggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					if (isOn)
					{
						int prevIndex = this._selectedAvatarIndex;
						this._selectedAvatarIndex = index;
						bool flag2 = -1 != prevIndex;
						if (flag2)
						{
							this.characterScrollView.RefreshCell(prevIndex);
						}
						refers.CGet<GameObject>("Selected").SetActive(true);
						this.confirm.interactable = true;
						this.RefreshConfirmButtonTips();
						this.SetSelectedCount(1);
					}
					else
					{
						bool flag3 = this._selectedAvatarIndex == index;
						if (flag3)
						{
							this.SetSelectedCount(0);
							this.confirm.interactable = false;
							this.RefreshConfirmButtonTips();
						}
					}
				});
				TooltipInvoker mouseTipDisplayer = refers.CGet<TooltipInvoker>("MouseTip");
				mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().SetObject("avatar", data);
			}
		}

		// Token: 0x06008237 RID: 33335 RVA: 0x003CBAAC File Offset: 0x003C9CAC
		private void OnAvatarItemHide(GameObject go)
		{
			Refers refers = go.GetComponent<Refers>();
			refers.CGet<Game.Components.Avatar.Avatar>("Avatar").ResetToBlank(false);
			refers.CGet<GameObject>("Selected").SetActive(false);
		}

		// Token: 0x06008238 RID: 33336 RVA: 0x003CBAE8 File Offset: 0x003C9CE8
		private void ClearCommonOptionPointerHandlers()
		{
			foreach (KeyValuePair<short, UnityAction> pair in this._commonOptionToggleEnterHandlers)
			{
				CToggle toggle = this.interactionTypeTogGroup.Get((int)pair.Key);
				PointerTrigger pointerTrigger = (toggle != null) ? toggle.GetComponent<PointerTrigger>() : null;
				if (pointerTrigger != null)
				{
					UnityEvent enterEvent = pointerTrigger.EnterEvent;
					if (enterEvent != null)
					{
						enterEvent.RemoveListener(pair.Value);
					}
				}
			}
			this._commonOptionToggleEnterHandlers.Clear();
		}

		// Token: 0x06008239 RID: 33337 RVA: 0x003CBB84 File Offset: 0x003C9D84
		private void SetCommonOption()
		{
			CharacterDisplayData targetCharacter = this.Data.TargetCharacter;
			bool flag = targetCharacter == null || targetCharacter.CreatingType != 1;
			if (flag)
			{
				this.commonOption.gameObject.SetActive(false);
				this.interactionTypeTogGroup.gameObject.SetActive(false);
			}
			else
			{
				this.commonOption.gameObject.SetActive(this.Data.ExtraData.ShowCommonOptionIndex >= 0);
				this.interactionTypeTogGroup.gameObject.SetActive(this.Data.ExtraData.ShowCommonOptionIndex >= 0);
				bool flag2 = this.Data.ExtraData.ShowCommonOptionIndex >= 0;
				if (flag2)
				{
					bool flag3 = this._isCommonOptionInit && this._lastUserSelectedCommonOptionIndex >= 0 && this._lastUserSelectedCommonOptionIndex != (short)this.Data.ExtraData.ShowCommonOptionIndex && this.Data.ExtraData.SelectItemData == null;
					if (flag3)
					{
						TaiwuEventDomainMethod.Call.EventCommonOptionSelect(this._lastUserSelectedCommonOptionIndex);
					}
					else
					{
						this.SetCommonOptionInfo();
					}
				}
			}
		}

		// Token: 0x0600823A RID: 33338 RVA: 0x003CBCA4 File Offset: 0x003C9EA4
		private void SetCommonOptionInfo()
		{
			bool flag = !this._isCommonOptionInit;
			if (flag)
			{
				List<Refers> shortcutHintList = this.commonOption.CGetList<Refers>("ShortcutHint");
				this._isCommonOptionInit = true;
				this.interactionTypeTogGroup.Init(0);
				this._lastUserSelectedCommonOptionIndex = (short)this.Data.ExtraData.ShowCommonOptionIndex;
				short i = 0;
				while ((int)i < EventCommonOption.Instance.Count)
				{
					EventCommonOptionItem config = EventCommonOption.Instance[i];
					Refers hint = shortcutHintList[(int)i];
					bool flag2 = i != 6;
					TextMeshProUGUI label;
					if (flag2)
					{
						CToggle toggle = this.interactionTypeTogGroup.Get((int)i);
						bool display = this.Model.IsEventCommonOptionUnlocked(config);
						toggle.gameObject.SetActive(display);
						hint.gameObject.SetActive(display);
						TaiwuEventDomainMethod.AsyncCall.EventCommonOptionHaveAvailableOption(this, i, delegate(int offset, RawDataPool dataPool)
						{
							bool haveAvailableOption = false;
							Serializer.Deserialize(dataPool, offset, ref haveAvailableOption);
							toggle.interactable = haveAvailableOption;
						});
						PointerTrigger pointerTrigger = toggle.GetComponent<PointerTrigger>();
						bool flag3 = pointerTrigger != null;
						if (flag3)
						{
							PointerTrigger pointerTrigger2 = pointerTrigger;
							if (pointerTrigger2.EnterEvent == null)
							{
								pointerTrigger2.EnterEvent = new UnityEvent();
							}
							UnityAction prevEnterHandler;
							bool flag4 = this._commonOptionToggleEnterHandlers.TryGetValue(i, out prevEnterHandler);
							if (flag4)
							{
								pointerTrigger.EnterEvent.RemoveListener(prevEnterHandler);
							}
							short templateId = i;
							UnityAction enterHandler = delegate()
							{
								this.OnCommonOptionTogglePointerEnter(templateId);
							};
							this._commonOptionToggleEnterHandlers[i] = enterHandler;
							pointerTrigger.EnterEvent.AddListener(enterHandler);
						}
						Refers tempRefers = toggle.GetComponent<Refers>();
						label = tempRefers.CGet<TextMeshProUGUI>("Label");
					}
					else
					{
						label = this.commonButtonFinish.GetComponent<Refers>().CGet<TextMeshProUGUI>("OptionName");
						short templateId = i;
						this.commonButtonFinish.ClearAndAddListener(delegate
						{
							TaiwuEventDomainMethod.Call.EventCommonOptionSelect(templateId);
						});
					}
					hint.CGet<TextMeshProUGUI>("ShortcutKey").text = this._commonHotKeyCommands[(int)i].ToString();
					hint.CGet<TextMeshProUGUI>("OptionName").text = config.OptionTitle;
					label.text = config.OptionTitle;
					i += 1;
				}
			}
			this.SyncCommonOptionToggleVisuals();
		}

		// Token: 0x0600823B RID: 33339 RVA: 0x003CBF00 File Offset: 0x003CA100
		private void SyncCommonOptionToggleVisuals()
		{
			bool flag = this.Data.ExtraData.ShowCommonOptionIndex < 0;
			if (!flag)
			{
				int activeIndex = (int)this.Data.ExtraData.ShowCommonOptionIndex;
				bool flag2 = this.interactionTypeTogGroup.GetActiveIndex() != activeIndex;
				if (flag2)
				{
					this.interactionTypeTogGroup.SetWithoutNotify(activeIndex);
				}
				short i = 0;
				while ((int)i < EventCommonOption.Instance.Count)
				{
					bool flag3 = i == 6;
					if (!flag3)
					{
						EventCommonOptionItem config = EventCommonOption.Instance[i];
						CToggle toggle = this.interactionTypeTogGroup.Get((int)i);
						bool display = this.Model.IsEventCommonOptionUnlocked(config);
						Refers tempRefers = toggle.GetComponent<Refers>();
						tempRefers.CGet<CImage>("Icon").SetSprite(this.GetIconName(i, display, activeIndex == (int)i), false, null);
					}
					i += 1;
				}
			}
		}

		// Token: 0x0600823C RID: 33340 RVA: 0x003CBFDC File Offset: 0x003CA1DC
		private string GetIconName(short i, bool display, bool selected)
		{
			int targetIndex = (!display) ? 3 : (selected ? 1 : 0);
			return string.Format("ui9_btn_eventwindow_tab_{0}_{1}", i, targetIndex);
		}

		// Token: 0x0600823D RID: 33341 RVA: 0x003CC014 File Offset: 0x003CA214
		private void OnCommonOptionTogglePointerEnter(short templateId)
		{
			bool flag = !this.commonOption.gameObject.activeSelf;
			if (!flag)
			{
				bool flag2 = !base.isActiveAndEnabled || this.Data == null;
				if (!flag2)
				{
					bool flag3 = this.interactionTypeTogGroup.GetActiveIndex() == (int)templateId;
					if (!flag3)
					{
						CToggle toggle = this.interactionTypeTogGroup.Get((int)templateId);
						bool flag4 = toggle == null || !toggle.isActiveAndEnabled || !toggle.interactable;
						if (!flag4)
						{
							this.interactionTypeTogGroup.Set((int)templateId, false);
						}
					}
				}
			}
		}

		// Token: 0x0600823E RID: 33342 RVA: 0x003CC0AC File Offset: 0x003CA2AC
		private void OnActiveToggleChange(int newToggleIndex, int oldToggleIndex)
		{
			bool flag = !base.isActiveAndEnabled || this.Data == null;
			if (!flag)
			{
				this._lastUserSelectedCommonOptionIndex = (short)newToggleIndex;
				TaiwuEventDomainMethod.Call.EventCommonOptionSelect((short)newToggleIndex);
				short i = 0;
				while ((int)i < EventCommonOption.Instance.Count)
				{
					EventCommonOptionItem config = EventCommonOption.Instance[i];
					bool flag2 = i != 6;
					if (flag2)
					{
						CToggle toggle = this.interactionTypeTogGroup.Get((int)i);
						bool display = this.Model.IsEventCommonOptionUnlocked(config);
						Refers tempRefers = toggle.GetComponent<Refers>();
						tempRefers.CGet<CImage>("Icon").SetSprite(this.GetIconName(i, display, newToggleIndex == (int)i), false, null);
					}
					else
					{
						short templateId = i;
						this.commonButtonFinish.ClearAndAddListener(delegate
						{
							TaiwuEventDomainMethod.Call.EventCommonOptionSelect(templateId);
						});
					}
					i += 1;
				}
				this._needPlayToggleGroupAnim = true;
			}
		}

		// Token: 0x0600823F RID: 33343 RVA: 0x003CC19D File Offset: 0x003CA39D
		private void OnChangeCharacterClothing(ArgumentBox argBox)
		{
			TaiwuEventDomainMethod.Call.UpdateShowingEventTaiwuCharacterDisplayData();
		}

		// Token: 0x06008240 RID: 33344 RVA: 0x003CC1A8 File Offset: 0x003CA3A8
		private void RefreshCharacterBack()
		{
			TaiwuEventDisplayData data = this.Data;
			bool flag;
			if (data != null)
			{
				TaiwuEventDisplayExtraData extraData = data.ExtraData;
				if (extraData != null)
				{
					flag = extraData.ShowBlockCharacterBack;
					goto IL_1E;
				}
			}
			flag = false;
			IL_1E:
			bool show = flag;
			this.eventCharacterBack.Refresh(show);
		}

		// Token: 0x06008246 RID: 33350 RVA: 0x003CC3CA File Offset: 0x003CA5CA
		[CompilerGenerated]
		private void <OnClick>g__SelectCharacterAction|111_1(List<int> selectedCharIdList)
		{
			SingletonObject.getInstance<EventModel>().SetSelectCharacterResult(selectedCharIdList);
			this.SetSelecting(false);
			selectedCharIdList.Clear();
		}

		// Token: 0x06008247 RID: 33351 RVA: 0x003CC3E8 File Offset: 0x003CA5E8
		[CompilerGenerated]
		private bool <OnClick>g__SelectedCharacterHasDukeTitle|111_2()
		{
			for (int i = 0; i < this.directSelectCharacterPanel.SelectedCharIdList.Count; i++)
			{
				int charId = this.directSelectCharacterPanel.SelectedCharIdList[i];
				bool flag = this.IsCharacterHaveDukeTitle(charId);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600824A RID: 33354 RVA: 0x003CC454 File Offset: 0x003CA654
		[CompilerGenerated]
		private bool <OnAutoChooseMessage>g__CanSelectOption|122_0(string toCheckOptionKey, ref ViewEventWindow.<>c__DisplayClass122_0 A_2)
		{
			bool flag = string.IsNullOrEmpty(toCheckOptionKey);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int i = 0;
				int max = this.Data.EventOptionInfos.Count;
				while (i < max)
				{
					bool flag2 = this.Data.EventOptionInfos[i].OptionKey == toCheckOptionKey;
					if (flag2)
					{
						return this.Data.EventOptionInfos[i].OptionState != -1;
					}
					bool flag3 = i == max - 1;
					if (flag3)
					{
						A_2.box.Set("AutoFirstOption", true);
					}
					i++;
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0400633F RID: 25407
		[SerializeField]
		private CButton btnConfiscateAll;

		// Token: 0x04006340 RID: 25408
		[SerializeField]
		private CButton cancel;

		// Token: 0x04006341 RID: 25409
		[SerializeField]
		private CButton confirm;

		// Token: 0x04006342 RID: 25410
		[SerializeField]
		private CButton contentTextSizeBtn;

		// Token: 0x04006343 RID: 25411
		[SerializeField]
		private CButton commonButtonFinish;

		// Token: 0x04006344 RID: 25412
		[SerializeField]
		private CButton systemBtn;

		// Token: 0x04006345 RID: 25413
		[SerializeField]
		private CImage blackMask;

		// Token: 0x04006346 RID: 25414
		[SerializeField]
		private CImage blackMask_ForDialog;

		// Token: 0x04006347 RID: 25415
		[SerializeField]
		private CImage maskForCg;

		// Token: 0x04006348 RID: 25416
		[SerializeField]
		private CRawImage cGTexture;

		// Token: 0x04006349 RID: 25417
		[SerializeField]
		private CRawImage eventTexture;

		// Token: 0x0400634A RID: 25418
		[SerializeField]
		private CScrollRect contentScrollView;

		// Token: 0x0400634B RID: 25419
		[SerializeField]
		private CScrollRect optionScroll;

		// Token: 0x0400634C RID: 25420
		[SerializeField]
		private CSlider contentTextSizeSlider;

		// Token: 0x0400634D RID: 25421
		[SerializeField]
		private CanvasGroup fadeRoot;

		// Token: 0x0400634E RID: 25422
		[SerializeField]
		private Game.Components.EventWindow.EventWindowCharacter leftCharacter;

		// Token: 0x0400634F RID: 25423
		[SerializeField]
		private Game.Components.EventWindow.EventWindowCharacter rightCharacter;

		// Token: 0x04006350 RID: 25424
		[SerializeField]
		private GameObject buttons;

		// Token: 0x04006351 RID: 25425
		[SerializeField]
		private GameObject inputPanel;

		// Token: 0x04006352 RID: 25426
		[SerializeField]
		private GameObject noContent;

		// Token: 0x04006353 RID: 25427
		[SerializeField]
		private PointerTrigger eventContentArea;

		// Token: 0x04006354 RID: 25428
		[SerializeField]
		private RectTransform interactArea;

		// Token: 0x04006355 RID: 25429
		[SerializeField]
		private RectTransform itemScrollViewUpPos;

		// Token: 0x04006356 RID: 25430
		[SerializeField]
		private RectTransform mainWindowRoot;

		// Token: 0x04006357 RID: 25431
		[SerializeField]
		private RectTransform operateArea;

		// Token: 0x04006358 RID: 25432
		[SerializeField]
		private RectTransform optionContainer;

		// Token: 0x04006359 RID: 25433
		[SerializeField]
		private RectTransform selectDataObj;

		// Token: 0x0400635A RID: 25434
		[SerializeField]
		private Refers commonOption;

		// Token: 0x0400635B RID: 25435
		[SerializeField]
		private Refers interactAreaBottomBG;

		// Token: 0x0400635C RID: 25436
		[SerializeField]
		private EventCharacterItemList itemListOfLeftCharacter;

		// Token: 0x0400635D RID: 25437
		[SerializeField]
		private EventCharacterItemList itemListOfRightCharacter;

		// Token: 0x0400635E RID: 25438
		[SerializeField]
		private EventWindowOption optionItem;

		// Token: 0x0400635F RID: 25439
		[SerializeField]
		private TextMeshProUGUI curSelectCount;

		// Token: 0x04006360 RID: 25440
		[SerializeField]
		private TextMeshProUGUI eventContent;

		// Token: 0x04006361 RID: 25441
		[SerializeField]
		private TextMeshProUGUI needCount;

		// Token: 0x04006362 RID: 25442
		[Header("Panels")]
		[SerializeField]
		private EventWindowInputPanel panelInput;

		// Token: 0x04006363 RID: 25443
		[SerializeField]
		private EventWindowChangeNameInputPanel panelInputChangeName;

		// Token: 0x04006364 RID: 25444
		[SerializeField]
		private EventWindowSelectNeigongLoopingCount panelSelectNeigongLoopingCount;

		// Token: 0x04006365 RID: 25445
		[SerializeField]
		private EventWindowSelectBookCount panelSelectBookCount;

		// Token: 0x04006366 RID: 25446
		[SerializeField]
		private EventWindowSelectFuyuFaith fuyuFaith;

		// Token: 0x04006367 RID: 25447
		[SerializeField]
		private EventWindowFameActionPanel fameActionPanel;

		// Token: 0x04006368 RID: 25448
		[SerializeField]
		private InfinityScroll characterScrollView;

		// Token: 0x04006369 RID: 25449
		[SerializeField]
		private EventWindowCharacterListPanel characterListPanel;

		// Token: 0x0400636A RID: 25450
		[SerializeField]
		private EventWindowDirectSelectCharacterListPanel directSelectCharacterPanel;

		// Token: 0x0400636B RID: 25451
		[SerializeField]
		private EventWindowSelctItems selectItemsPanel;

		// Token: 0x0400636C RID: 25452
		[Header("互动Tab")]
		[SerializeField]
		private CToggleGroup interactionTypeTogGroup;

		// Token: 0x0400636D RID: 25453
		[SerializeField]
		private TooltipInvoker mouseTipExit;

		// Token: 0x0400636E RID: 25454
		[Header("角色背景")]
		[SerializeField]
		private EventCharacterBack eventCharacterBack;

		// Token: 0x0400636F RID: 25455
		[Header("动画控制节点")]
		[SerializeField]
		private RectTransform animationRoot;

		// Token: 0x04006370 RID: 25456
		[SerializeField]
		private CanvasGroup animGroupBg;

		// Token: 0x04006371 RID: 25457
		[SerializeField]
		private CanvasGroup animGroupPanel;

		// Token: 0x04006372 RID: 25458
		[SerializeField]
		private RectTransform animGroupLeftChara;

		// Token: 0x04006373 RID: 25459
		[SerializeField]
		private RectTransform animGroupRightChara;

		// Token: 0x04006374 RID: 25460
		private static readonly List<UIElement> _conflictElements = new List<UIElement>
		{
			UIElement.AdventurePrepareRemake
		};

		// Token: 0x04006375 RID: 25461
		private const string OptionItem = "UI_EventWindow_OptionItem";

		// Token: 0x04006376 RID: 25462
		private PoolItem _optionPool;

		// Token: 0x04006377 RID: 25463
		private EventOptionInfo _selectedOptionInfo;

		// Token: 0x04006378 RID: 25464
		public float windowAnimDuration = 0.5f;

		// Token: 0x04006379 RID: 25465
		public float cgShowTime = 3f;

		// Token: 0x0400637A RID: 25466
		public static string TextureDirectory = "RemakeResources/Textures/EventBack";

		// Token: 0x0400637C RID: 25468
		private bool _needSelectItem;

		// Token: 0x0400637D RID: 25469
		private bool _needSelectApprovedTaiwu;

		// Token: 0x0400637E RID: 25470
		private bool _animating;

		// Token: 0x0400637F RID: 25471
		private bool _posInViewPort;

		// Token: 0x04006380 RID: 25472
		private bool _isDisplayingLog;

		// Token: 0x04006381 RID: 25473
		private DetailInfoMonitor _taiwuDetailInfoMonitor;

		// Token: 0x04006382 RID: 25474
		private bool _maskAnimHandled;

		// Token: 0x04006383 RID: 25475
		private sbyte _maskControlCode;

		// Token: 0x04006384 RID: 25476
		private float _maskAnimTweenTime;

		// Token: 0x04006385 RID: 25477
		private float _contentFontSize;

		// Token: 0x04006386 RID: 25478
		private static float s_savedContentFontSize = 24f;

		// Token: 0x04006387 RID: 25479
		private const float ContentFontSizeMin = 18f;

		// Token: 0x04006388 RID: 25480
		private const float ContentFontSizeMax = 36f;

		// Token: 0x04006389 RID: 25481
		private Tweener _adjustLayoutTween;

		// Token: 0x0400638A RID: 25482
		private EventModel _model;

		// Token: 0x0400638B RID: 25483
		private Action _animationWindowCompleteAction;

		// Token: 0x0400638C RID: 25484
		private Action _animationMaskCompleteAction;

		// Token: 0x0400638D RID: 25485
		private bool _animationMaskComplete = true;

		// Token: 0x0400638E RID: 25486
		private Action _animationCgTextureCompleteAction;

		// Token: 0x0400638F RID: 25487
		public static readonly List<LanguageKey> BehaviorNameKeyList = new List<LanguageKey>
		{
			LanguageKey.LK_Goodness_0,
			LanguageKey.LK_Goodness_1,
			LanguageKey.LK_Goodness_2,
			LanguageKey.LK_Goodness_3,
			LanguageKey.LK_Goodness_4
		};

		// Token: 0x04006390 RID: 25488
		private readonly HotKeyCommand[] _optionHotKeyCommands = new HotKeyCommand[]
		{
			EventWindowCommandKit.Option1,
			EventWindowCommandKit.Option2,
			EventWindowCommandKit.Option3,
			EventWindowCommandKit.Option4,
			EventWindowCommandKit.Option5,
			EventWindowCommandKit.Option6,
			EventWindowCommandKit.Option7,
			EventWindowCommandKit.Option8,
			EventWindowCommandKit.Option9,
			EventWindowCommandKit.Option10,
			EventWindowCommandKit.OptionExit
		};

		// Token: 0x04006391 RID: 25489
		private readonly HotKeyCommand[] _commonHotKeyCommands = new HotKeyCommand[]
		{
			EventWindowCommandKit.TopicChat,
			EventWindowCommandKit.TopicCompare,
			EventWindowCommandKit.TopicStudy,
			EventWindowCommandKit.TopicIntimacy,
			EventWindowCommandKit.TopicHostile,
			EventWindowCommandKit.TopicInteract,
			EventWindowCommandKit.OptionExit
		};

		// Token: 0x04006392 RID: 25490
		private Vector2 _itemScrollViewInitPos;

		// Token: 0x04006393 RID: 25491
		private Vector2 _itemScrollViewInitDeltaSize;

		// Token: 0x04006394 RID: 25492
		private CommonSecondToggleContentRefreshAnim _toggleGroupAnim;

		// Token: 0x04006395 RID: 25493
		private bool _needPlayToggleGroupAnim;

		// Token: 0x04006396 RID: 25494
		private readonly Dictionary<short, UnityAction> _commonOptionToggleEnterHandlers = new Dictionary<short, UnityAction>();

		// Token: 0x04006397 RID: 25495
		private Tween _inOutAnimTween;

		// Token: 0x04006398 RID: 25496
		private int _haveSpiritualDebt;

		// Token: 0x04006399 RID: 25497
		private int _haveMoney;

		// Token: 0x0400639A RID: 25498
		private Dictionary<ItemKey, bool> _fixItemAttainmentMeetDict = new Dictionary<ItemKey, bool>();

		// Token: 0x0400639B RID: 25499
		private readonly List<ItemKey> _selectedItemList = new List<ItemKey>();

		// Token: 0x0400639C RID: 25500
		private readonly Dictionary<ItemKey, int> _selectedItemDict = new Dictionary<ItemKey, int>();

		// Token: 0x0400639D RID: 25501
		private bool _isMultiItemSelect;

		// Token: 0x0400639E RID: 25502
		private bool _isExchangeTools;

		// Token: 0x0400639F RID: 25503
		private bool _isFixItem;

		// Token: 0x040063A0 RID: 25504
		private int _selectedAvatarIndex;

		// Token: 0x040063A1 RID: 25505
		private bool _isCommonOptionInit;

		// Token: 0x040063A2 RID: 25506
		private short _lastUserSelectedCommonOptionIndex = -1;
	}
}
