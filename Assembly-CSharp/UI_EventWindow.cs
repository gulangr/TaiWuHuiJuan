using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CharacterDataMonitor;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.CommandSystem;
using Game.Components.Avatar;
using Game.Views.CharacterMenu;
using Game.Views.EventWindow;
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
using GameData.Domains.TaiwuEvent.EventOption;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using Item;
using Spine.Unity;
using TMPro;
using UICommon.Character.Elements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200037D RID: 893
public class UI_EventWindow : UIBase
{
	// Token: 0x17000598 RID: 1432
	// (get) Token: 0x0600341D RID: 13341 RVA: 0x0019D1C3 File Offset: 0x0019B3C3
	private bool CanSelect
	{
		get
		{
			return this._waitSelect && !this._animating && !this._layoutDirty && !this._isDisplayingLog;
		}
	}

	// Token: 0x17000599 RID: 1433
	// (get) Token: 0x0600341E RID: 13342 RVA: 0x0019D1EC File Offset: 0x0019B3EC
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

	// Token: 0x1700059A RID: 1434
	// (get) Token: 0x0600341F RID: 13343 RVA: 0x0019D21C File Offset: 0x0019B41C
	private EventTextureManager EventTextureManager
	{
		get
		{
			return SingletonObject.getInstance<EventTextureManager>();
		}
	}

	// Token: 0x1700059B RID: 1435
	// (get) Token: 0x06003420 RID: 13344 RVA: 0x0019D223 File Offset: 0x0019B423
	private TaiwuEventDisplayData Data
	{
		get
		{
			return this.Model.DisplayingEventData;
		}
	}

	// Token: 0x06003421 RID: 13345 RVA: 0x0019D230 File Offset: 0x0019B430
	private void Awake()
	{
		this._optionPool = new PoolItem("UI_EventWindow_OptionItem", base.CGet<Refers>("OptionItem").gameObject);
		base.CGet<EventWindowCharacter>("LeftCharacter").OnViewCharacter = new Action<int, int, bool>(this.ShowCharacterMenu);
		base.CGet<EventWindowCharacter>("RightCharacter").OnViewCharacter = new Action<int, int, bool>(this.ShowCharacterMenu);
		CSliderLegacy slider = base.CGet<CSliderLegacy>("ContentTextSizeSlider");
		slider.onValueChanged.RemoveAllListeners();
		slider.onValueChanged.AddListener(delegate(float value)
		{
			base.CGet<TextMeshProUGUI>("EventContent").fontSize = value;
			this.MarkLayoutDirty();
		});
	}

	// Token: 0x06003422 RID: 13346 RVA: 0x0019D2C8 File Offset: 0x0019B4C8
	private void OnEnable()
	{
		GEvent.OnEvent(UiEvents.OnEventWindowStart, null);
		SingletonObject.getInstance<WorldMapModel>().ChangeTaiwuMoveState(WorldMapModel.MoveState.Idle);
		base.CGet<RectTransform>("AnimationRoot").localPosition = Vector3.up * 3000f;
		this._posInViewPort = false;
		foreach (UIElement conflictElement in UI_EventWindow.ConflictElements)
		{
			UIManager.Instance.HideUI(conflictElement);
		}
		GEvent.Add(UiEvents.EventWindowAutoChoose, new GEvent.Callback(this.OnAutoChooseMessage));
		this.Refresh();
		GEvent.OnEvent(UiEvents.OnNewEventComingToShow, null);
		GEvent.OnEvent(UiEvents.OnNeedCombatPause, null);
		GEvent.Add(UiEvents.OnEventWindowDisplayDataChanged, new GEvent.Callback(this.OnDisplayDataChanged));
		GEvent.Add(UiEvents.OnMarriageCharacterListChanged, new GEvent.Callback(this.OnMarriageCharacterAvatarChanged));
		GEvent.Add(UiEvents.OnJieqingMaskCharacterListChanged, new GEvent.Callback(this.OnJieqingMaskCharacterListChanged));
		GEvent.Add(UiEvents.MouseTipBaseOnEnable, new GEvent.Callback(this.MouseTipBaseOnEnable));
		GEvent.Add(UiEvents.MouseTipBaseOnDisable, new GEvent.Callback(this.MouseTipBaseOnDisable));
	}

	// Token: 0x06003423 RID: 13347 RVA: 0x0019D42C File Offset: 0x0019B62C
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUIChanged));
		GEvent.Remove(UiEvents.OnMarriageCharacterListChanged, new GEvent.Callback(this.OnMarriageCharacterAvatarChanged));
		GEvent.Remove(UiEvents.OnJieqingMaskCharacterListChanged, new GEvent.Callback(this.OnJieqingMaskCharacterListChanged));
		GEvent.Remove(UiEvents.OnEventWindowDisplayDataChanged, new GEvent.Callback(this.OnDisplayDataChanged));
		GEvent.Remove(UiEvents.EventWindowAutoChoose, new GEvent.Callback(this.OnAutoChooseMessage));
		base.CGet<EventWindowCharacter>("LeftCharacter").gameObject.SetActive(false);
		base.CGet<EventWindowCharacter>("RightCharacter").gameObject.SetActive(false);
		this._canSelectCharIdList.Clear();
		this._canSelectCharacterDisplayDataList.Clear();
		this.StopAllCricketSing();
		this.SetSelecting(false);
		GEvent.OnEvent(UiEvents.OnNeedCombatResume, null);
		GEvent.OnEvent(UiEvents.OnEventWindowEnded, null);
		GEvent.Remove(UiEvents.MouseTipBaseOnEnable, new GEvent.Callback(this.MouseTipBaseOnEnable));
		GEvent.Remove(UiEvents.MouseTipBaseOnDisable, new GEvent.Callback(this.MouseTipBaseOnDisable));
		this._isCommonOptionInit = false;
	}

	// Token: 0x06003424 RID: 13348 RVA: 0x0019D56C File Offset: 0x0019B76C
	private void OnDestroy()
	{
		this._optionPool.Destroy();
	}

	// Token: 0x06003425 RID: 13349 RVA: 0x0019D57C File Offset: 0x0019B77C
	private void Update()
	{
		bool flag = this.Data == null;
		if (!flag)
		{
			bool layoutDirty = this._layoutDirty;
			if (layoutDirty)
			{
				this._layoutDirtyCount += 1;
				bool flag2 = this._layoutDirtyCount >= this.LayoutDirtyDelayFrame;
				if (flag2)
				{
					this._layoutDirty = false;
					this._layoutDirtyCount = 0;
					this.LayoutAndAnimAdjustSize();
				}
			}
			bool flag3 = !this._animationMaskComplete;
			if (!flag3)
			{
				for (int i = 0; i < this._optionHotKeyCommands.Length; i++)
				{
					bool flag4 = i == (int)this.Data.EscOptionIndex;
					if (!flag4)
					{
						bool flag5 = this._optionHotKeyCommands[i].Check(this.Element, false, false, true, true, false);
						if (flag5)
						{
							this.SelectOptionByIndex(i, false);
							return;
						}
						bool flag6 = i >= this.Data.EventOptionInfos.Count;
						if (flag6)
						{
							break;
						}
					}
				}
				bool escHandled = UIManager.Instance.EscHandled;
				if (!escHandled)
				{
					bool flag7 = CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) && this.ConfirmActive();
					if (flag7)
					{
						this.OnClick(base.CGet<CButtonObsolete>("Confirm").transform);
					}
					else
					{
						bool flag8 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) && this.CancelActive();
						if (flag8)
						{
							this.OnClick(base.CGet<CButtonObsolete>("Cancel").transform);
						}
						else
						{
							bool flag9 = CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) && UIManager.Instance.IsFocusElement(this.Element);
							if (flag9)
							{
								CommandManager.AddCommand<CommandShowUI, UIElement>(EPriority.OpenUISystemOption, UIElement.SystemOption);
							}
							bool flag10 = this._optionHotKeyCommands.Last<HotKeyCommand>().Check(this.Element, false, false, true, true, false);
							if (flag10)
							{
								bool flag11 = this._currentPressDuration >= 0.6f;
								if (flag11)
								{
									bool activeSelf = base.CGet<Refers>("CommonOption").gameObject.activeSelf;
									if (activeSelf)
									{
										TaiwuEventDomainMethod.Call.EventCommonOptionSelect(6);
									}
								}
								else
								{
									bool flag12 = this.Data.EscOptionIndex >= 0;
									if (flag12)
									{
										this.SelectEscOption();
									}
								}
							}
							bool key = Input.GetKey(this._optionHotKeyCommands.Last<HotKeyCommand>().KeyGroup.Key);
							if (key)
							{
								this._currentPressDuration += Time.deltaTime;
							}
							else
							{
								this._currentPressDuration = 0f;
							}
							bool flag13 = CommonCommandKit.RightMouse.Check(this.Element, false, false, true, true, false);
							if (flag13)
							{
								bool flag14 = this.Data.EscOptionIndex >= 0;
								if (flag14)
								{
									this.SelectEscOption();
								}
								else
								{
									bool activeSelf2 = base.CGet<Refers>("CommonOption").gameObject.activeSelf;
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

	// Token: 0x06003426 RID: 13350 RVA: 0x0019D874 File Offset: 0x0019BA74
	private bool ConfirmActive()
	{
		return base.CGet<CButtonObsolete>("Confirm").interactable && base.CGet<CButtonObsolete>("Confirm").gameObject.activeInHierarchy && this.Data.ExtraData.InputRequestData == null;
	}

	// Token: 0x06003427 RID: 13351 RVA: 0x0019D8C8 File Offset: 0x0019BAC8
	private bool CancelActive()
	{
		return base.CGet<CButtonObsolete>("Cancel").interactable && base.CGet<CButtonObsolete>("Cancel").gameObject.activeInHierarchy && this.Data.ExtraData.InputRequestData == null;
	}

	// Token: 0x06003428 RID: 13352 RVA: 0x0019D91C File Offset: 0x0019BB1C
	public override void OnInit(ArgumentBox argsBox)
	{
		base.CGet<RectTransform>("OperateArea").gameObject.SetActive(false);
		base.CGet<GameObject>("Buttons").SetActive(false);
		GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUIChanged));
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		this._taiwuDetailInfoMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DetailInfoMonitor>(taiwuCharId, false);
		RectTransform rectTrans = base.CGet<ItemScrollView>("ItemScrollView").GetComponent<RectTransform>();
		this._itemScrollViewInitPos = rectTrans.anchoredPosition;
		this._itemScrollViewInitDeltaSize = rectTrans.sizeDelta;
	}

	// Token: 0x06003429 RID: 13353 RVA: 0x0019D9B4 File Offset: 0x0019BBB4
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = "Confirm" == btnName;
		if (flag)
		{
			UI_EventWindow.<>c__DisplayClass50_0 CS$<>8__locals1 = new UI_EventWindow.<>c__DisplayClass50_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.optionIndex = 0;
			bool flag2 = this.Data.ExtraData.InputRequestData != null;
			if (flag2)
			{
				string inputResult = base.CGet<TMP_InputField>("InputField").text;
				bool flag3 = string.IsNullOrEmpty(inputResult);
				if (flag3)
				{
					return;
				}
				SingletonObject.getInstance<EventModel>().SetInputResult(inputResult);
			}
			bool flag4 = this.Data.ExtraData.SelectItemData != null && this._selectedItemList.Count > 0;
			if (flag4)
			{
				SingletonObject.getInstance<EventModel>().SetSelectItemResult(this._selectedItemList, this._selectedItemDict);
				this._selectedItemList.Clear();
				this._selectedItemDict.Clear();
				this.SetSelecting(false);
			}
			bool flag5 = this.Data.ExtraData.SelectCharacterData != null && this._selectedCharIdList.Count > 0;
			if (flag5)
			{
				bool flag6 = this.Data.ExtraData.SelectCharacterData.SelectApprovedTaiwu != null && this.<OnClick>g__SelectedCharacterHasDukeTitle|50_2();
				if (flag6)
				{
					DialogCmd cmd = new DialogCmd
					{
						Title = LocalStringManager.Get(LanguageKey.LK_Bottom_MapInfo_GiveTitle),
						Content = LocalStringManager.Get(LanguageKey.UI_Profession_SelectDukeTitleTips),
						Type = 1,
						Yes = delegate()
						{
							CS$<>8__locals1.<>4__this.<OnClick>g__SelectCharacterAction|50_1();
							base.<OnClick>g__ConfirmAction|0();
						}
					};
					UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
					UIManager.Instance.MaskUI(UIElement.Dialog);
					return;
				}
				this.<OnClick>g__SelectCharacterAction|50_1();
			}
			bool flag7 = this.Data.ExtraData.SelectReadingBookCountData != null || this.Data.ExtraData.SelectNeigongLoopingCountData != null;
			if (flag7)
			{
				SingletonObject.getInstance<EventModel>().SetSelectCountResult(this._curCount);
			}
			else
			{
				bool flag8 = this.Data.ExtraData.SelectFuyuFaithCountData != null;
				if (flag8)
				{
					SingletonObject.getInstance<EventModel>().SetSelectCountResult(this.fuyuFaith.Value);
					this.SetSelecting(false);
				}
			}
			bool flag9 = this.Data.ExtraData.SelectOneAvatarRelatedDataList != null && -1 != this._selectedAvatarIndex;
			if (flag9)
			{
				CS$<>8__locals1.optionIndex = this._selectedAvatarIndex;
				this.SetSelecting(false);
				this._selectedAvatarIndex = -1;
			}
			bool flag10 = this.Data.ExtraData.SelectFameData != null && this._selectedFameActionIdList.Count > 0;
			if (flag10)
			{
				SingletonObject.getInstance<EventModel>().SetSelectFameActionResult(this._selectedFameActionIdList);
			}
			CS$<>8__locals1.<OnClick>g__ConfirmAction|0();
		}
		else
		{
			bool flag11 = "Cancel" == btnName;
			if (flag11)
			{
				int selectOptionIndex = 1;
				bool flag12 = this.Data.ExtraData.SelectOneAvatarRelatedDataList != null && this.Data.ExtraData.SelectOneAvatarRelatedDataList.Count > 0;
				if (flag12)
				{
					selectOptionIndex = (int)this.Data.EscOptionIndex;
				}
				this.SelectOption(this.Data.EventOptionInfos[selectOptionIndex]);
				this.ResetData();
				this.AnimOperateAreaOut();
			}
			else
			{
				bool flag13 = "BtnEventLog" == btnName;
				if (flag13)
				{
					bool flag14 = !this._waitSelect;
					if (!flag14)
					{
						this._waitSelect = false;
						UIElement eventLog = UIElement.EventLog;
						eventLog.OnShowed = (Action)Delegate.Combine(eventLog.OnShowed, new Action(delegate()
						{
							this._waitSelect = true;
						}));
						UIManager.Instance.ShowUI(UIElement.EventLog, true);
						this.SwitchIsDisplayingLog(true);
					}
				}
				else
				{
					bool flag15 = "ContentTextSizeBtn" == btnName;
					if (flag15)
					{
						CSliderLegacy slider = base.CGet<CSliderLegacy>("ContentTextSizeSlider");
						Transform sliderHolder = slider.gameObject.transform.parent;
						sliderHolder.gameObject.SetActive(!sliderHolder.gameObject.activeInHierarchy);
						bool activeInHierarchy = sliderHolder.gameObject.activeInHierarchy;
						if (activeInHierarchy)
						{
							slider.value = base.CGet<TextMeshProUGUI>("EventContent").fontSize;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600342A RID: 13354 RVA: 0x0019DDE0 File Offset: 0x0019BFE0
	private void ResetData()
	{
		this._selectedItemList.Clear();
		this._selectedItemDict.Clear();
		this._selectedCharIdList.Clear();
		this.SetSelecting(false);
		bool flag = this._multiplyItemScrollView;
		if (flag)
		{
			this._multiplyItemScrollView.ExitMultiplyMode();
		}
	}

	// Token: 0x0600342B RID: 13355 RVA: 0x0019DE38 File Offset: 0x0019C038
	private void OnViewLostFocus(UIElement element = null)
	{
		bool flag = element == null;
		if (flag)
		{
			base.transform.localPosition = Vector3.up * 3000f;
		}
		else
		{
			element.OnActive = (Action)Delegate.Combine(element.OnActive, new Action(delegate()
			{
				base.transform.localPosition = Vector3.up * 3000f;
			}));
		}
	}

	// Token: 0x0600342C RID: 13356 RVA: 0x0019DE90 File Offset: 0x0019C090
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

	// Token: 0x0600342D RID: 13357 RVA: 0x0019DEEE File Offset: 0x0019C0EE
	private void OnDisplayDataChanged(ArgumentBox box)
	{
		this.Refresh();
	}

	// Token: 0x0600342E RID: 13358 RVA: 0x0019DEF8 File Offset: 0x0019C0F8
	private void OnMarriageCharacterAvatarChanged(ArgumentBox box)
	{
		bool flag = this.Data == null;
		if (!flag)
		{
			base.CGet<EventWindowCharacter>("LeftCharacter").Refresh();
			base.CGet<EventWindowCharacter>("RightCharacter").Refresh();
		}
	}

	// Token: 0x0600342F RID: 13359 RVA: 0x0019DF38 File Offset: 0x0019C138
	private void OnJieqingMaskCharacterListChanged(ArgumentBox box)
	{
		EventWindowCharacter left = base.CGet<EventWindowCharacter>("LeftCharacter");
		EventWindowCharacter right = base.CGet<EventWindowCharacter>("RightCharacter");
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

	// Token: 0x06003430 RID: 13360 RVA: 0x0019DFE0 File Offset: 0x0019C1E0
	private void MouseTipBaseOnEnable(ArgumentBox box)
	{
		bool flag = !UIManager.Instance.IsFocusElement(this.Element);
		if (!flag)
		{
			this.SetSelecting(false);
			this.SetSelectingApprovedTaiwu(false);
		}
	}

	// Token: 0x06003431 RID: 13361 RVA: 0x0019E018 File Offset: 0x0019C218
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
			bool needSelectApprovedTaiwu = this._needSelectApprovedTaiwu;
			if (needSelectApprovedTaiwu)
			{
				this.SetSelectingApprovedTaiwu(true);
			}
		}
	}

	// Token: 0x06003432 RID: 13362 RVA: 0x0019E070 File Offset: 0x0019C270
	private void OnAutoChooseMessage(ArgumentBox box)
	{
		UI_EventWindow.<>c__DisplayClass59_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.box = box;
		string guid;
		bool flag = !CS$<>8__locals1.box.Get("Guid", out guid) || guid != this.Data.EventGuid;
		if (flag)
		{
			throw new Exception("Event AutoChooseMessage Guid Error:" + guid);
		}
		string optionKey;
		bool flag2 = !CS$<>8__locals1.box.Get("Option", out optionKey) || !this.<OnAutoChooseMessage>g__CanSelectOption|59_0(optionKey, ref CS$<>8__locals1);
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
				RectTransform optionRoot = base.CGet<RectTransform>("OptionContainer");
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
							CButtonObsolete button = refers.CGet<CButtonObsolete>("Button");
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
							CButtonObsolete button2 = refers2.CGet<CButtonObsolete>("Button");
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
			bool flag10 = !this.<OnAutoChooseMessage>g__CanSelectOption|59_0(optionKey, ref CS$<>8__locals1);
			if (flag10)
			{
				RectTransform optionRoot2 = base.CGet<RectTransform>("OptionContainer");
				int k = 0;
				int max3 = this.Data.EventOptionInfos.Count;
				while (k < max3)
				{
					Transform child3 = optionRoot2.Find(this.Data.EventOptionInfos[k].OptionKey);
					Refers refers3 = child3.GetComponent<Refers>();
					bool flag11 = null == refers3;
					if (!flag11)
					{
						CButtonObsolete button3 = refers3.CGet<CButtonObsolete>("Button");
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
						this._selectedCharIdList.AddRange(charIdList);
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

	// Token: 0x06003433 RID: 13363 RVA: 0x0019E538 File Offset: 0x0019C738
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
				this._selectedCharIdList.Add(characterID);
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

	// Token: 0x06003434 RID: 13364 RVA: 0x0019E63C File Offset: 0x0019C83C
	private void ShowCharacterMenu(int charId, int pageIndex, bool isLeftCharacter)
	{
		bool flag = isLeftCharacter && !SingletonObject.getInstance<TutorialChapterModel>().OpenCharacterMenuEnable;
		if (!flag)
		{
			bool flag2 = !this._waitSelect;
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
					this.OnViewLostFocus(UIElement.CharacterMenu);
					UIElement.CharacterMenu.SetOnInitArgs(argBox);
					UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
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
						ArgumentBox args = new ArgumentBox();
						args.SetObject("TargetPageIndex", targetPage);
						bool flag4 = pageIndex == 0;
						if (flag4)
						{
							args.Set("BaseAttributeIndex", 1);
						}
						GEvent.OnEvent(UiEvents.OnNeedOpenCharacterMenuSubPage, args);
					}
				}
			}
		}
	}

	// Token: 0x06003435 RID: 13365 RVA: 0x0019E790 File Offset: 0x0019C990
	public void SwitchIsDisplayingLog(bool value)
	{
		this._isDisplayingLog = value;
	}

	// Token: 0x06003436 RID: 13366 RVA: 0x0019E799 File Offset: 0x0019C999
	private void AutoOnClickConfirm()
	{
		base.CGet<CButtonObsolete>("Confirm").onClick.Invoke();
	}

	// Token: 0x06003437 RID: 13367 RVA: 0x0019E7B2 File Offset: 0x0019C9B2
	private void AutoOnClickCancel()
	{
		base.CGet<CButtonObsolete>("Cancel").onClick.Invoke();
	}

	// Token: 0x06003438 RID: 13368 RVA: 0x0019E7CC File Offset: 0x0019C9CC
	private List<ItemKey> RandomSelectionItem()
	{
		List<ItemKey> itemKeyList = new List<ItemKey>();
		int randomIndex = Random.Range(0, this.Data.ExtraData.SelectItemData.CanSelectItemList.Count - 1);
		itemKeyList.Clear();
		itemKeyList.Add(this.Data.ExtraData.SelectItemData.CanSelectItemList[randomIndex].Key);
		return itemKeyList;
	}

	// Token: 0x06003439 RID: 13369 RVA: 0x0019E838 File Offset: 0x0019CA38
	private bool IsAutoChooseItem()
	{
		return this.Data.ExtraData.SelectItemData.CanSelectItemList.Count > 0;
	}

	// Token: 0x0600343A RID: 13370 RVA: 0x0019E868 File Offset: 0x0019CA68
	private int RandomSelectionCharacter()
	{
		int randomIndex = Random.Range(0, this._canSelectCharIdList.Count - 1);
		return this._canSelectCharIdList[randomIndex];
	}

	// Token: 0x0600343B RID: 13371 RVA: 0x0019E89C File Offset: 0x0019CA9C
	private bool CheckIsSpecialEvent(ArgumentBox box, string eventType)
	{
		bool result;
		box.Get(eventType, out result);
		return result;
	}

	// Token: 0x0600343C RID: 13372 RVA: 0x0019E8BC File Offset: 0x0019CABC
	public void Refresh()
	{
		bool flag = this.Data == null;
		if (!flag)
		{
			this._animationMaskCompleteAction = null;
			this._animationWindowCompleteAction = null;
			this._animationCgTextureCompleteAction = null;
			base.CGet<CanvasGroup>("FadeRoot").alpha = 1f;
			bool needShowCgTexture = !this.Model.EventCgTextureData.Item1 && !string.IsNullOrEmpty(this.Model.EventCgTextureData.Item2);
			sbyte maskControlCode = this.Data.MaskControlCode;
			bool flag2 = maskControlCode == 1 || maskControlCode == 3 || maskControlCode == 4 || maskControlCode == 5;
			if (flag2)
			{
				this._layoutDirty = false;
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
				this._layoutDirty = true;
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

	// Token: 0x0600343D RID: 13373 RVA: 0x0019EA80 File Offset: 0x0019CC80
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

	// Token: 0x0600343E RID: 13374 RVA: 0x0019EB34 File Offset: 0x0019CD34
	private void StopAllCricketSing()
	{
		CricketView[] allCrickets = base.GetComponentsInChildren<CricketView>(true);
		Array.ForEach<CricketView>(allCrickets, delegate(CricketView e)
		{
			e.StopLoopSing();
		});
	}

	// Token: 0x0600343F RID: 13375 RVA: 0x0019EB70 File Offset: 0x0019CD70
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

	// Token: 0x06003440 RID: 13376 RVA: 0x0019EB88 File Offset: 0x0019CD88
	private void UpdateWindow()
	{
		bool flag = this.Data == null;
		if (!flag)
		{
			this.SetCommonOption();
			this.UpdateTexture();
			this.UpdateContent();
			base.CGet<EventWindowCharacter>("LeftCharacter").Refresh();
			base.CGet<EventWindowCharacter>("RightCharacter").Refresh();
			sbyte grade = (this.Data.MainCharacter != null) ? this.Data.MainCharacter.OrgInfo.Grade : 0;
			this.UpdateItemList(base.CGet<Refers>("ItemListOfLeftCharacter"), this.Model.LeftItemList, this.Model.ShowItemWithCricketBattleGuess, grade);
			grade = ((this.Data.TargetCharacter != null) ? this.Data.TargetCharacter.OrgInfo.Grade : 0);
			this.UpdateItemList(base.CGet<Refers>("ItemListOfRightCharacter"), this.Model.RightItemList, this.Model.ShowItemWithCricketBattleGuess, grade);
			bool showInput = this.Data.ExtraData.InputRequestData != null;
			bool showSelectItem = this.Data.ExtraData.SelectItemData != null;
			bool showSelectReadingBookCount = this.Data.ExtraData.SelectReadingBookCountData != null;
			bool showSelectNeigongLoopingCount = this.Data.ExtraData.SelectNeigongLoopingCountData != null;
			bool showSelectFuyuFaithCount = this.Data.ExtraData.SelectFuyuFaithCountData != null;
			bool showSelectFameAction = this.Data.ExtraData.SelectFameData != null;
			bool showSelectCharacter = this.Data.ExtraData.SelectCharacterData != null;
			bool showSelectAvatar = this.Data.ExtraData.SelectOneAvatarRelatedDataList != null;
			bool showOperateArea = showSelectItem || showSelectCharacter || showInput || showSelectAvatar || showSelectReadingBookCount || showSelectNeigongLoopingCount || showSelectFameAction || showSelectFuyuFaithCount;
			base.CGet<RectTransform>("OperateArea").gameObject.SetActive(showOperateArea);
			base.CGet<CScrollRectLegacy>("OptionScroll").gameObject.SetActive(!showOperateArea);
			base.CGet<Refers>("SelectReadingBookCount").gameObject.SetActive(showSelectReadingBookCount);
			base.CGet<Refers>("SelectNeigongLoopingCount").gameObject.SetActive(showSelectNeigongLoopingCount);
			base.CGet<Refers>("SetSelectCount").gameObject.SetActive(showSelectReadingBookCount || showSelectNeigongLoopingCount);
			base.CGet<InfinityScrollLegacy>("FameActionScrollView").gameObject.SetActive(showSelectFameAction);
			base.CGet<ItemScrollView>("ItemScrollView").gameObject.SetActive(showSelectItem);
			base.CGet<InfinityScrollLegacy>("CharacterScrollView").gameObject.SetActive(showSelectCharacter || showSelectAvatar);
			base.CGet<GameObject>("InputPanel").SetActive(showInput);
			base.CGet<InfinityScrollLegacy>("CharacterScrollView").OnItemRender = null;
			base.CGet<InfinityScrollLegacy>("CharacterScrollView").OnItemHide = null;
			this._needSelectItem = false;
			this._needSelectApprovedTaiwu = false;
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("Guid", this.Data.EventGuid);
			bool flag2 = this.Model.SelectCombatSkillData != null && this.Model.SelectCombatSkillData.ResultSaveKey != null;
			if (flag2)
			{
				argBox.Set("WillSelectCombatSkill", true);
			}
			bool flag3 = this.Model.SelectLifeSkillData != null && this.Model.SelectLifeSkillData.ResultSaveKey != null;
			if (flag3)
			{
				argBox.Set("WillSelectLifeSKill", true);
			}
			this.fuyuFaith.gameObject.SetActive(showSelectFuyuFaithCount);
			this.HandlerInteractArea();
			this.HandlerConfiscateItem();
			this.HandlerResourceOrSpiritualDebtCost();
			bool flag4 = showSelectItem;
			if (flag4)
			{
				this.RefreshItemSelectScroll();
				argBox.Set("SelectItem", true);
			}
			else
			{
				bool flag5 = showSelectReadingBookCount;
				if (flag5)
				{
					this.RefreshReadingBookCountPanel();
					argBox.Set("SelectReadingBookCount", true);
				}
				else
				{
					bool flag6 = showSelectNeigongLoopingCount;
					if (flag6)
					{
						this.RefreshNeigongLoopingCountPanel();
						argBox.Set("SelectNeigongLoopingCount", true);
					}
					else
					{
						bool flag7 = showSelectFuyuFaithCount;
						if (flag7)
						{
							this._waitSelect = true;
							base.CGet<GameObject>("Buttons").SetActive(true);
							base.CGet<CButtonObsolete>("Confirm").interactable = true;
							this.fuyuFaith.SetData(this.Data.ExtraData.SelectFuyuFaithCountData);
						}
						else
						{
							bool flag8 = showSelectFameAction;
							if (flag8)
							{
								this.RefreshSelectFameActionPanel();
								argBox.Set("SelectFameAction", true);
							}
							else
							{
								bool flag9 = showSelectCharacter;
								if (flag9)
								{
									bool flag10 = this.Data.ExtraData.SelectCharacterData.SelectApprovedTaiwu != null;
									if (flag10)
									{
										this.RefreshCharacterSelectApprovedTaiwuScroll();
									}
									else
									{
										this.RefreshCharacterSelectScroll();
									}
									argBox.Set("SelectCharacter", true);
								}
								else
								{
									bool flag11 = showInput;
									if (flag11)
									{
										this.RefreshInputPanel();
										argBox.Set("WaitInput", true);
									}
									else
									{
										bool flag12 = showSelectAvatar;
										if (flag12)
										{
											this.RefreshSelectAvatarScroll();
											argBox.Set("SelectAvatar", true);
										}
										else
										{
											this.SetSelecting(false);
											this.UpdateOptionScroll();
										}
									}
								}
							}
						}
					}
				}
			}
			base.StartCoroutine(this.DelayNotifyEventWindowRefresh(argBox));
		}
	}

	// Token: 0x06003441 RID: 13377 RVA: 0x0019F084 File Offset: 0x0019D284
	private void HandlerInteractArea()
	{
		bool showSelectItem = this.Data.ExtraData.SelectItemData != null;
		bool showInteractArea = showSelectItem && (this.Data.ExtraData.SelectItemData.ItemOperationType == ItemOperationType.EItemOperationType.Confiscate.ToSbyte() || this.Data.ExtraData.SelectItemData.ItemOperationType == ItemOperationType.EItemOperationType.ExchangeTools.ToSbyte() || this.Data.ExtraData.SelectItemData.ItemOperationType == ItemOperationType.EItemOperationType.FixItem.ToSbyte());
		base.CGet<Refers>("InteractAreaBottomBG").gameObject.SetActive(showInteractArea);
		string interactAreaBgName = showInteractArea ? "taiwuevent_01_xuanze_8" : "taiwuevent_01_xuanze_7";
		CImage interactAreaImage = base.CGet<RectTransform>("OperateArea").GetComponent<CImage>();
		interactAreaImage.SetSprite(interactAreaBgName, false, null);
		ItemScrollView itemScrollView = base.CGet<ItemScrollView>("ItemScrollView");
		RectTransform itemScrollViewRectTrans = itemScrollView.GetComponent<RectTransform>();
		RectTransform upRectTrans = base.CGet<RectTransform>("ItemScrollViewUpPos");
		itemScrollViewRectTrans.anchoredPosition = (showInteractArea ? upRectTrans.anchoredPosition : this._itemScrollViewInitPos);
		itemScrollViewRectTrans.sizeDelta = (showInteractArea ? upRectTrans.sizeDelta : this._itemScrollViewInitDeltaSize);
	}

	// Token: 0x06003442 RID: 13378 RVA: 0x0019F1B4 File Offset: 0x0019D3B4
	private void HandlerConfiscateItem()
	{
		bool showSelectItem = this.Data.ExtraData.SelectItemData != null;
		bool isConfiscateItem = showSelectItem && this.Data.ExtraData.SelectItemData.ItemOperationType == ItemOperationType.EItemOperationType.Confiscate.ToSbyte();
		CButtonObsolete btnConfiscateAll = base.CGet<Refers>("InteractAreaBottomBG").CGet<CButtonObsolete>("BtnConfiscateAll");
		btnConfiscateAll.gameObject.SetActive(isConfiscateItem);
		EventSelectItemData selectItemData = this.Data.ExtraData.SelectItemData;
		int? num;
		if (selectItemData == null)
		{
			num = null;
		}
		else
		{
			List<ITradeableContent> canSelectItemList = selectItemData.CanSelectItemList;
			num = ((canSelectItemList != null) ? new int?(canSelectItemList.Count) : null);
		}
		int? num2 = num;
		int count = num2.GetValueOrDefault();
		btnConfiscateAll.interactable = (isConfiscateItem && count > 0);
		bool interactable = btnConfiscateAll.interactable;
		if (interactable)
		{
			btnConfiscateAll.ClearAndAddListener(new Action(this.OnClickConfiscateAll));
		}
	}

	// Token: 0x06003443 RID: 13379 RVA: 0x0019F29C File Offset: 0x0019D49C
	private void HandlerResourceOrSpiritualDebtCost()
	{
		bool showSelectItem = this.Data.ExtraData.SelectItemData != null;
		bool flag = !showSelectItem;
		if (!flag)
		{
			bool showCost = this.Data.ExtraData.SelectItemData.ItemOperationType == ItemOperationType.EItemOperationType.ExchangeTools.ToSbyte() || this.Data.ExtraData.SelectItemData.ItemOperationType == ItemOperationType.EItemOperationType.FixItem.ToSbyte();
			Refers costRefers = base.CGet<Refers>("InteractAreaBottomBG").CGet<Refers>("CostRefer");
			costRefers.gameObject.SetActive(showCost);
			bool flag2 = this.Data.ExtraData.SelectItemData.ItemOperationType == ItemOperationType.EItemOperationType.ExchangeTools.ToSbyte();
			if (flag2)
			{
				costRefers.CGet<CImage>("Icon").SetSprite("sp_icon_resource_gratitude", false, null);
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
					costRefers.CGet<CImage>("Icon").SetSprite(CommonUtils.GetResOrExpIconLegacy(6), false, null);
					costRefers.CGet<TextMeshProUGUI>("Tips").SetText(LocalStringManager.Get(LanguageKey.LK_MouseTipMedicine_CostProperty).GetFormat(CommonUtils.GetResOrExpName(6)), true);
					this._haveMoney = SingletonObject.getInstance<BuildingModel>().GetResourceCount(ItemSourceType.Resources, 6);
					costRefers.CGet<TextMeshProUGUI>("Have").SetText(CommonUtils.GetDisplayStringForNum(this._haveMoney, 100000), true);
					this.FixItemSetCostNeed(0);
				}
			}
		}
	}

	// Token: 0x06003444 RID: 13380 RVA: 0x0019F48C File Offset: 0x0019D68C
	public void ExchangeToolSetCostNeed(int value)
	{
		Refers costRefers = base.CGet<Refers>("InteractAreaBottomBG").CGet<Refers>("CostRefer");
		TextMeshProUGUI cost = costRefers.CGet<TextMeshProUGUI>("Cost");
		cost.text = CommonUtils.GetDisplayStringForNum(value, 10000).SetColor((this._haveSpiritualDebt >= value) ? "brightblue" : "brightred");
	}

	// Token: 0x06003445 RID: 13381 RVA: 0x0019F4E8 File Offset: 0x0019D6E8
	public void FixItemSetCostNeed(int value)
	{
		Refers costRefers = base.CGet<Refers>("InteractAreaBottomBG").CGet<Refers>("CostRefer");
		costRefers.CGet<TextMeshProUGUI>("Cost").SetText(CommonUtils.GetDisplayStringForNum(value, 100000).SetColor((this._haveMoney >= value) ? "brightblue" : "brightred"), true);
	}

	// Token: 0x06003446 RID: 13382 RVA: 0x0019F544 File Offset: 0x0019D744
	public void ExchangeToolUpdateConfirm(int value)
	{
		base.CGet<CButtonObsolete>("Confirm").interactable = (this._haveSpiritualDebt >= value && this._multiplyItemScrollView.TotalSelectedCount >= this.Data.ExtraData.SelectItemData.FilterList.Count);
	}

	// Token: 0x06003447 RID: 13383 RVA: 0x0019F59C File Offset: 0x0019D79C
	public void FixItemUpdateConfirm(int value)
	{
		base.CGet<CButtonObsolete>("Confirm").interactable = (this._haveMoney >= value && this._selectedItemDict.Keys.Count >= this.Data.ExtraData.SelectItemData.FilterList.Count);
	}

	// Token: 0x06003448 RID: 13384 RVA: 0x0019F5F8 File Offset: 0x0019D7F8
	public void UpdateExchangeToolCost()
	{
		bool isExchangeTools = this._isExchangeTools;
		if (isExchangeTools)
		{
			ItemKey itemKey = this._multiplyItemScrollView.GetAlreadySelectItem();
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

	// Token: 0x06003449 RID: 13385 RVA: 0x0019F658 File Offset: 0x0019D858
	public void UpdateFixItemCost(ItemDisplayData itemDisplayData)
	{
		bool isFixItem = this._isFixItem;
		if (isFixItem)
		{
			ItemKey itemKey = itemDisplayData.Key;
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

	// Token: 0x0600344A RID: 13386 RVA: 0x0019F69C File Offset: 0x0019D89C
	private void AnimEventWindowIn()
	{
		this._animating = true;
		this._posInViewPort = true;
		RectTransform windowRoot = base.CGet<RectTransform>("AnimationRoot");
		windowRoot.localPosition = Vector3.up * 3000f;
		windowRoot.DOKill(false);
		windowRoot.DOLocalMove(Vector3.zero, this.WindowAnimDuration, false).SetEase(Ease.OutBack).OnComplete(delegate
		{
			this._animating = false;
			Action animationWindowCompleteAction = this._animationWindowCompleteAction;
			if (animationWindowCompleteAction != null)
			{
				animationWindowCompleteAction();
			}
			this._animationWindowCompleteAction = null;
		}).SetUpdate(true);
		CRawImage cgRawImage = base.CGet<CRawImage>("CGTexture");
		bool enabled = cgRawImage.enabled;
		if (enabled)
		{
			CImage maskForCg = base.CGet<CImage>("MaskForCg");
			maskForCg.SetAlpha(0f);
			maskForCg.gameObject.SetActive(true);
			DOVirtual.Float(0f, 0.7f, this.WindowAnimDuration, new TweenCallback<float>(maskForCg.SetAlpha)).SetAutoKill(true);
		}
	}

	// Token: 0x0600344B RID: 13387 RVA: 0x0019F778 File Offset: 0x0019D978
	private void AnimEventWindowOut()
	{
		this._animating = true;
		this._posInViewPort = false;
		RectTransform windowRoot = base.CGet<RectTransform>("AnimationRoot");
		windowRoot.DOKill(false);
		windowRoot.DOLocalMove(Vector3.up * 3000f, this.WindowAnimDuration, false).SetEase(Ease.InBack).OnComplete(delegate
		{
			this._animating = false;
			Action animationWindowCompleteAction = this._animationWindowCompleteAction;
			if (animationWindowCompleteAction != null)
			{
				animationWindowCompleteAction();
			}
			this._animationWindowCompleteAction = null;
		}).SetUpdate(true);
		CImage maskForCg = base.CGet<CImage>("MaskForCg");
		bool activeSelf = maskForCg.gameObject.activeSelf;
		if (activeSelf)
		{
			DOVirtual.Float(0.7f, 0f, this.WindowAnimDuration, new TweenCallback<float>(maskForCg.SetAlpha)).SetUpdate(true).OnComplete(delegate
			{
				maskForCg.gameObject.SetActive(false);
			}).SetAutoKill(true);
		}
	}

	// Token: 0x0600344C RID: 13388 RVA: 0x0019F85C File Offset: 0x0019DA5C
	private void AnimMaskToBlack()
	{
		CImage maskImage = base.CGet<CImage>("BlackMask");
		maskImage.SetAlpha(0f);
		DOVirtual.Float(0f, 1f, (float)this.Data.MaskTweenTime / 100f, new TweenCallback<float>(maskImage.SetAlpha)).OnComplete(delegate
		{
			Action animationMaskCompleteAction = this._animationMaskCompleteAction;
			if (animationMaskCompleteAction != null)
			{
				animationMaskCompleteAction();
			}
			this._animationMaskCompleteAction = null;
		}).SetUpdate(true).SetAutoKill(true);
	}

	// Token: 0x0600344D RID: 13389 RVA: 0x0019F8D0 File Offset: 0x0019DAD0
	private void AnimMaskToAlpha()
	{
		CImage maskImage = base.CGet<CImage>("BlackMask");
		DOVirtual.Float(1f, 0f, (float)this.Data.MaskTweenTime / 100f, new TweenCallback<float>(maskImage.SetAlpha)).OnComplete(delegate
		{
			Action animationMaskCompleteAction = this._animationMaskCompleteAction;
			if (animationMaskCompleteAction != null)
			{
				animationMaskCompleteAction();
			}
			this._animationMaskCompleteAction = null;
		}).SetUpdate(true).SetAutoKill(true);
	}

	// Token: 0x0600344E RID: 13390 RVA: 0x0019F938 File Offset: 0x0019DB38
	private void AnimMaskToBlackToAlpha()
	{
		this._animationMaskComplete = false;
		CImage maskImage = base.CGet<CImage>("BlackMask");
		maskImage.SetAlpha(0f);
		TweenCallback <>9__1;
		DOVirtual.Float(0f, 1f, (float)this.Data.MaskTweenTime / 100f, new TweenCallback<float>(maskImage.SetAlpha)).OnComplete(delegate
		{
			Tweener t = DOVirtual.Float(1f, 0f, (float)this.Data.MaskTweenTime / 100f, new TweenCallback<float>(maskImage.SetAlpha));
			TweenCallback action;
			if ((action = <>9__1) == null)
			{
				action = (<>9__1 = delegate()
				{
					Action animationMaskCompleteAction = this._animationMaskCompleteAction;
					if (animationMaskCompleteAction != null)
					{
						animationMaskCompleteAction();
					}
					this._animationMaskCompleteAction = null;
					this._animationMaskComplete = true;
				});
			}
			t.OnComplete(action).SetUpdate(true).SetAutoKill(true);
		}).SetUpdate(true).SetAutoKill(true);
	}

	// Token: 0x0600344F RID: 13391 RVA: 0x0019F9CC File Offset: 0x0019DBCC
	private void AnimOperateAreaOut()
	{
	}

	// Token: 0x06003450 RID: 13392 RVA: 0x0019F9EC File Offset: 0x0019DBEC
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
				base.CGet<CImage>("BlackMask").SetAlpha(1f);
				Color c = Color.white;
				c.a = 0f;
				CRawImage cgRawImage = base.CGet<CRawImage>("CGTexture");
				cgRawImage.texture = cgTexture;
				cgRawImage.SetNativeSize();
				cgRawImage.color = c;
				cgRawImage.enabled = true;
				Sequence sequence = DOTween.Sequence();
				sequence.Append(DOVirtual.Float(0f, 1f, this._model.EventCgTextureData.Item3, delegate(float stepValue)
				{
					c.a = stepValue;
					cgRawImage.color = c;
				}).SetAutoKill(true));
				sequence.AppendInterval(this.CgShowTime);
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

	// Token: 0x06003451 RID: 13393 RVA: 0x0019FA80 File Offset: 0x0019DC80
	private void AnimCgTextureOut()
	{
		CRawImage cgRawImage = base.CGet<CRawImage>("CGTexture");
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
			}).OnComplete(delegate
			{
				Action animationCgTextureCompleteAction2 = this._animationCgTextureCompleteAction;
				if (animationCgTextureCompleteAction2 != null)
				{
					animationCgTextureCompleteAction2();
				}
				this._animationCgTextureCompleteAction = null;
				cgRawImage.enabled = false;
				cgRawImage.texture = null;
			}).SetUpdate(true).SetAutoKill(true);
		}
	}

	// Token: 0x06003452 RID: 13394 RVA: 0x0019FBA8 File Offset: 0x0019DDA8
	public void AnimFadeToHide()
	{
		base.CGet<EventWindowCharacter>("LeftCharacter").GetComponent<RectTransform>().DOAnchorPosX(-670f, 0.6f, false).OnComplete(delegate
		{
			Action animationWindowCompleteAction = this._animationWindowCompleteAction;
			if (animationWindowCompleteAction != null)
			{
				animationWindowCompleteAction();
			}
		}).SetUpdate(true);
		base.CGet<EventWindowCharacter>("RightCharacter").GetComponent<RectTransform>().DOAnchorPosX(670f, 0.6f, false).SetUpdate(true);
		base.CGet<CanvasGroup>("FadeRoot").DOFade(0f, 0.2f).SetUpdate(true);
	}

	// Token: 0x06003453 RID: 13395 RVA: 0x0019FC38 File Offset: 0x0019DE38
	private void ResetAnimFadeToHide()
	{
		base.CGet<EventWindowCharacter>("LeftCharacter").GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		base.CGet<EventWindowCharacter>("RightCharacter").GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		base.CGet<CanvasGroup>("FadeRoot").alpha = 1f;
	}

	// Token: 0x06003454 RID: 13396 RVA: 0x0019FC94 File Offset: 0x0019DE94
	private void LayoutAndAnimAdjustSize()
	{
		RectTransform optionRoot = base.CGet<RectTransform>("OptionContainer");
		bool activeInHierarchy = optionRoot.gameObject.activeInHierarchy;
		if (activeInHierarchy)
		{
			base.CGet<CScrollRectLegacy>("OptionScroll").ScrollBar.value = 0f;
		}
		bool activeInHierarchy2 = base.CGet<RectTransform>("OperateArea").gameObject.activeInHierarchy;
		if (activeInHierarchy2)
		{
			base.CGet<InfinityScrollLegacy>("CharacterScrollView").GetComponent<CScrollRectLegacy>().ScrollBar.value = 0f;
		}
		this.AdjustContentScrollView();
	}

	// Token: 0x06003455 RID: 13397 RVA: 0x0019FD1C File Offset: 0x0019DF1C
	private void AdjustContentScrollView()
	{
		CScrollRectLegacy contentScrollView = base.CGet<CScrollRectLegacy>("ContentScrollView");
		contentScrollView.ScrollBar.value = 0f;
		contentScrollView.Content.anchoredPosition = new Vector2(0f, -contentScrollView.Content.sizeDelta.y);
	}

	// Token: 0x06003456 RID: 13398 RVA: 0x0019FD6E File Offset: 0x0019DF6E
	private void MarkLayoutDirty()
	{
		this._layoutDirty = true;
		this._layoutDirtyCount = 0;
	}

	// Token: 0x06003457 RID: 13399 RVA: 0x0019FD80 File Offset: 0x0019DF80
	private void UpdateTexture()
	{
		string textureName = this.Data.EventTexture;
		CRawImage eventRawImage = base.CGet<CRawImage>("EventTexture");
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
			GLog.TagError(this.Element.Name, "failed to set event " + this.Data.EventGuid + " 's texture,event texture name is empty!", Array.Empty<object>());
		}
		else
		{
			string texturePath;
			bool eventBackPath = this.EventTextureManager.GetEventBackPath(textureName, out texturePath);
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
					bool eventBackPath2 = this.EventTextureManager.GetEventBackPath(textureName, out texturePath);
					if (eventBackPath2)
					{
						texturePath = Path.Combine(this.TextureDirectory, texturePath);
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

	// Token: 0x06003458 RID: 13400 RVA: 0x0019FF48 File Offset: 0x0019E148
	private void UpdateContent()
	{
		string content = this.Data.EventContent;
		bool flag = this.Data.ExtraFormatLanguageKeys != null && this.Data.ExtraFormatLanguageKeys.Count > 0;
		if (flag)
		{
			object[] array = this.Data.ExtraFormatLanguageKeys.ConvertAll<string>(new Converter<string, string>(LocalStringManager.Get)).ToArray();
			object[] formatArgs = array;
			content = content.GetFormat(formatArgs);
		}
		base.CGet<TextMeshProUGUI>("EventContent").text = content.ColorReplace();
		base.CGet<TextMeshProUGUI>("EventContent").SetLayoutDirty();
		base.CGet<GameObject>("NoContent").SetActive(string.IsNullOrEmpty(content));
	}

	// Token: 0x06003459 RID: 13401 RVA: 0x0019FFF8 File Offset: 0x0019E1F8
	private void UpdateItemList(Refers refers, ItemDisplayData[] itemDisplayDataArray, bool showForCricketBattleGuess, sbyte gradeOfCharacter)
	{
		int elemCount = 0;
		int visibleIndex = -1;
		string jarGradeIcon = string.Empty;
		if (showForCricketBattleGuess)
		{
			visibleIndex = Random.Range(0, 3);
			jarGradeIcon = Misc.Instance.GetItem((short)(91 + gradeOfCharacter)).Icon;
		}
		refers.gameObject.SetActive(true);
		for (int i = 0; i < 3; i++)
		{
			bool needShowElement = itemDisplayDataArray[i] != null;
			elemCount += (needShowElement ? 1 : 0);
			GameObject elemObj = refers.CGet<GameObject>(string.Format("Item_{0}", i));
			elemObj.SetActive(needShowElement);
			bool flag = !needShowElement;
			if (!flag)
			{
				CImage jarImg = refers.CGet<CImage>(string.Format("Jar_{0}", i));
				ItemView itemView = refers.CGet<ItemView>(string.Format("ItemView_{0}", i));
				itemView.SetData(itemDisplayDataArray[i], false, -1, false, false, null, false, true);
				bool flag2 = this.Model.CoverCricketJarGradeList != null && i != visibleIndex;
				if (flag2)
				{
					jarGradeIcon = Misc.Instance.GetItem((short)(91 + this.Model.CoverCricketJarGradeList[i])).Icon;
				}
				jarImg.SetSprite((i == visibleIndex) ? string.Empty : jarGradeIcon, false, null);
				bool flag3 = showForCricketBattleGuess || (itemDisplayDataArray[i].Key.ItemType == 11 && itemDisplayDataArray[i].Durability > 0);
				if (flag3)
				{
					CricketView cricketView = itemView.CricketView;
					SkeletonGraphic graphic = cricketView.CGet<SkeletonGraphic>("SkeletonGraphic");
					graphic.color = graphic.color.SetAlpha((float)((i == visibleIndex) ? 1 : 0));
					bool isVisibleCricket = i == visibleIndex;
					Action<float> <>9__1;
					SingletonObject.getInstance<YieldHelper>().DelaySecondsDo((float)Random.Range(1, cricketView.Level + 1), delegate
					{
						CricketView cricketView = cricketView;
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
				itemView.CGet<CImage>("GradeBack").gameObject.SetActive(shouldEnableTips);
			}
		}
		refers.gameObject.SetActive(elemCount > 0);
		bool flag4 = elemCount > 0;
		if (flag4)
		{
			float size = (float)(elemCount * 114 + (elemCount - 1) * 10);
			RectTransform rectTransform = refers.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
			}
		}
	}

	// Token: 0x0600345A RID: 13402 RVA: 0x001A0296 File Offset: 0x0019E496
	public static sbyte OptionBehaviorToCharacterBehavior(sbyte optionBehavior)
	{
		return optionBehavior - 1;
	}

	// Token: 0x0600345B RID: 13403 RVA: 0x001A029C File Offset: 0x0019E49C
	public static string GetCharacterBehaviorConditionStr(sbyte formatBehavior, bool hasDot = true)
	{
		StringBuilder sb = EasyPool.Get<StringBuilder>();
		if (hasDot)
		{
			sb.Append(LocalStringManager.Get(LanguageKey.LK_Dot_Symbol));
		}
		sb.Append(LocalStringManager.Get(LanguageKey.LK_OptionNeedBehavior));
		int startIndex = Mathf.Max((int)(formatBehavior - 1), 0);
		int endIndex = Mathf.Min((int)(formatBehavior + 1), 4);
		for (int index = startIndex; index <= endIndex; index++)
		{
			string behaviorName = LocalStringManager.Get(UI_EventWindow.BehaviorNameKeyList[index]).ColorReplace();
			sb.Append(behaviorName);
			bool flag = index < endIndex;
			if (flag)
			{
				sb.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
			}
		}
		string forbidReason = sb.ToString();
		EasyPool.Free<StringBuilder>(sb);
		return forbidReason;
	}

	// Token: 0x0600345C RID: 13404 RVA: 0x001A035A File Offset: 0x0019E55A
	public static bool CheckBehaviorCondition(sbyte charBehaviorType, sbyte optionBehaviorType)
	{
		return GameData.Domains.Character.BehaviorType.IsCloseOrSame(charBehaviorType, optionBehaviorType);
	}

	// Token: 0x0600345D RID: 13405 RVA: 0x001A0364 File Offset: 0x0019E564
	private void ProcessSpecialEventOption(EventOptionInfo info, Refers refers)
	{
		CharacterDisplayData displayData = this.Data.TargetCharacter;
		CImage icon = refers.CGet<CImage>("SpecialOptionIcon");
		TextMeshProUGUI label = refers.CGet<TextMeshProUGUI>("SpecialOptionText");
		TooltipInvoker mouseTipDisplayer = icon.GetComponent<TooltipInvoker>();
		bool flag = info.OptionType == 4;
		if (flag)
		{
			icon.gameObject.SetActive(true);
			bool flag2 = displayData != null && displayData.AvatarRelatedData.HasNewGoods;
			if (flag2)
			{
				icon.SetSprite("blockchar_icon_shanghui_2", false, null);
				label.text = LocalStringManager.Get(LanguageKey.LK_Word_New);
				mouseTipDisplayer.enabled = true;
				mouseTipDisplayer.PresetParam = new string[]
				{
					LocalStringManager.Get(LanguageKey.LK_NewGoods_TipTitle),
					LocalStringManager.Get(LanguageKey.LK_NewGoods_TipContent)
				};
			}
			else
			{
				icon.SetSprite("sp_icon_shanghui", false, null);
				label.text = string.Empty;
				mouseTipDisplayer.enabled = false;
			}
		}
		else
		{
			icon.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600345E RID: 13406 RVA: 0x001A0458 File Offset: 0x0019E658
	private void UpdateOptionScroll()
	{
		this._waitSelect = true;
		RectTransform optionRoot = base.CGet<RectTransform>("OptionContainer");
		Refers[] prevRefers = optionRoot.GetComponentsInTopChildren(true);
		for (int i = 0; i < prevRefers.Length; i++)
		{
			this._optionPool.DestroyObject(prevRefers[i].gameObject);
		}
		sbyte taiwuBehaviorType = GameData.Domains.Character.BehaviorType.GetBehaviorType(this._taiwuDetailInfoMonitor.Behavior);
		int strMaxCount = this.GetMaxLabelTextCount();
		for (int index = 0; index < this.Data.EventOptionInfos.Count; index++)
		{
			EventOptionInfo optionInfo = this.Data.EventOptionInfos[index];
			Refers refer = this._optionPool.GetObject().GetComponent<Refers>();
			refer.transform.SetParent(optionRoot, false);
			refer.gameObject.SetActive(true);
			refer.transform.SetAsLastSibling();
			refer.name = optionInfo.OptionKey;
			string commandKey = this._optionHotKeyCommands[index].ToString();
			bool flag = index == (int)this.Data.EscOptionIndex;
			if (flag)
			{
				commandKey = this._optionHotKeyCommands.Last<HotKeyCommand>().ToString();
			}
			TextMeshProUGUI hotKeyLabel = refer.CGet<TextMeshProUGUI>("OptionIndex");
			hotKeyLabel.text = "[".SetColor("brightblue") + (commandKey.ToLower() ?? "").SetColor("hotkeyyellow") + "]".SetColor("brightblue");
			LayoutElement layoutElement = refer.CGet<LayoutElement>("HotKeyContainer");
			layoutElement.minWidth = (float)(strMaxCount * 10 + 50);
			sbyte formatBehavior = UI_EventWindow.OptionBehaviorToCharacterBehavior(optionInfo.Behavior);
			bool optionBehaviorForbid = optionInfo.Behavior != 0 && !GameData.Domains.Character.BehaviorType.IsCloseOrSame(formatBehavior, taiwuBehaviorType);
			List<OptionAvailableInfo> optionAvailableConditions = optionInfo.OptionAvailableConditions;
			bool flag2;
			if (optionAvailableConditions == null || optionAvailableConditions.Count <= 0)
			{
				List<OptionAvailableConditionInfo> optionAvailableConditionInfos = optionInfo.OptionAvailableConditionInfos;
				flag2 = (optionAvailableConditionInfos != null && optionAvailableConditionInfos.Count > 0);
			}
			else
			{
				flag2 = true;
			}
			bool showHelp = flag2;
			bool ignoreOptionBehavior = EventModel.IgnoreEventBehavior || !SingletonObject.getInstance<BasicGameData>().RestrictOptionsBehaviorType;
			bool flag3 = !ignoreOptionBehavior && optionBehaviorForbid;
			if (flag3)
			{
				showHelp = true;
			}
			refer.CGet<TooltipInvoker>("OptionHelp").gameObject.SetActive(showHelp);
			bool flag4 = showHelp;
			if (flag4)
			{
				bool hasHelpTips = false;
				string tipsTitle = LocalStringManager.Get(LanguageKey.LK_Event_Need);
				string orWord = LocalStringManager.Get(LanguageKey.LK_Event_Or);
				List<string> allAvailableInfoContents = EasyPool.Get<List<string>>();
				int j = 0;
				for (;;)
				{
					int num = j;
					List<OptionAvailableInfo> optionAvailableConditions2 = optionInfo.OptionAvailableConditions;
					int? num2 = (optionAvailableConditions2 != null) ? new int?(optionAvailableConditions2.Count) : null;
					if (!(num < num2.GetValueOrDefault() & num2 != null))
					{
						break;
					}
					OptionAvailableInfo info = optionInfo.OptionAvailableConditions[j];
					string start = LocalStringManager.Get(LanguageKey.LK_Dot_Symbol);
					List<string> cacheList = EasyPool.Get<List<string>>();
					for (int k = 0; k < info.Data.Length; k++)
					{
						OptionAvailableInfoMinimumElement element = info.Data[k];
						string conditionCellContent = this.Model.GetOptionConditionContent(element.ConditionId);
						string str = conditionCellContent;
						string[] formatArgs2 = element.FormatArgs;
						object[] array;
						if (formatArgs2 == null)
						{
							array = null;
						}
						else
						{
							array = formatArgs2.ChangeArrType((string e) => e);
						}
						conditionCellContent = str.GetFormat(array ?? Array.Empty<object>());
						bool flag5 = !element.Pass;
						if (flag5)
						{
							conditionCellContent = "<color=#lightgrey>" + conditionCellContent + "</color>";
						}
						bool flag6 = element.ConditionId != 4 || !element.Pass;
						if (flag6)
						{
							cacheList.Add(conditionCellContent);
						}
						hasHelpTips = true;
					}
					bool flag7 = cacheList.Count > 0;
					if (flag7)
					{
						allAvailableInfoContents.Add(start + string.Join(orWord, cacheList));
					}
					EasyPool.Free<List<string>>(cacheList);
					j++;
				}
				int l = 0;
				for (;;)
				{
					int num3 = l;
					List<OptionAvailableConditionInfo> optionAvailableConditionInfos2 = optionInfo.OptionAvailableConditionInfos;
					int? num2 = (optionAvailableConditionInfos2 != null) ? new int?(optionAvailableConditionInfos2.Count) : null;
					if (!(num3 < num2.GetValueOrDefault() & num2 != null))
					{
						break;
					}
					OptionAvailableConditionInfo info2 = optionInfo.OptionAvailableConditionInfos[l];
					string start2 = LocalStringManager.Get(LanguageKey.LK_Dot_Symbol);
					string inGameHint = EventFunction.Instance[info2.EventFunctionId].InGameHint;
					string[] args = info2.Args;
					string text2;
					if (args == null || args.Length <= 0)
					{
						text2 = inGameHint;
					}
					else
					{
						string str2 = inGameHint;
						object[] array2 = info2.Args;
						text2 = str2.GetFormat(array2);
					}
					string text = text2;
					bool pass = info2.Pass;
					if (pass)
					{
						allAvailableInfoContents.Add(start2 + text);
					}
					else
					{
						allAvailableInfoContents.Add(start2 + "<color=#lightgrey>" + text + "</color>");
					}
					hasHelpTips = true;
					l++;
				}
				bool flag8 = optionBehaviorForbid;
				if (flag8)
				{
					string forbidReason = UI_EventWindow.GetCharacterBehaviorConditionStr(formatBehavior, true);
					allAvailableInfoContents.Add(forbidReason);
					hasHelpTips = true;
				}
				bool flag9 = hasHelpTips;
				if (flag9)
				{
					string finalTipsDesc = string.Join("\n", allAvailableInfoContents);
					refer.CGet<TooltipInvoker>("OptionHelp").PresetParam = new string[]
					{
						tipsTitle,
						finalTipsDesc.SetColor("pinkyellow").ColorReplace()
					};
				}
				else
				{
					refer.CGet<TooltipInvoker>("OptionHelp").gameObject.SetActive(false);
				}
				EasyPool.Free<List<string>>(allAvailableInfoContents);
			}
			this.ProcessSpecialEventOption(optionInfo, refer);
			bool canSelect = optionInfo.OptionState != -1 && (!optionBehaviorForbid || ignoreOptionBehavior);
			string optionDesc = optionInfo.OptionContent;
			bool flag10 = optionInfo.ExtraFormatLanguageKeys != null && optionInfo.ExtraFormatLanguageKeys.Count > 0;
			if (flag10)
			{
				object[] array2 = optionInfo.ExtraFormatLanguageKeys.ConvertAll<string>(new Converter<string, string>(LocalStringManager.Get)).ToArray();
				object[] formatArgs = array2;
				optionDesc = optionDesc.GetFormat(formatArgs);
			}
			bool hasBehavior = optionInfo.Behavior != 0;
			refer.CGet<GameObject>("BehaviorIconContainer").SetActive(hasBehavior);
			bool flag11 = hasBehavior;
			if (flag11)
			{
				BehaviorTypeItem behaviorConfig = Config.BehaviorType.Instance.GetItem((short)formatBehavior);
				refer.CGet<CImage>("OptionBehaviorIcon").SetSprite(behaviorConfig.Icon, false, null);
				string[] behaviorTipsData = new string[]
				{
					behaviorConfig.Name + LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Behavior),
					LocalStringManager.Get(LanguageKey.UI_EventWindow_OptionBehavior_DiffTips)
				};
				bool flag12 = canSelect;
				if (flag12)
				{
					bool flag13 = optionBehaviorForbid;
					if (flag13)
					{
						optionDesc = optionDesc.SetColor(Colors.Instance["brightred"]);
					}
					else
					{
						bool flag14 = (short)formatBehavior == this._taiwuDetailInfoMonitor.Behavior;
						if (flag14)
						{
							optionDesc = optionDesc.SetColor(Colors.Instance["brightblue"]);
							behaviorTipsData[1] = LocalStringManager.Get(LanguageKey.UI_EventWindow_OptionBehavior_SameTips);
						}
						else
						{
							optionDesc = optionDesc.SetColor(Colors.Instance["supportyellow"]);
							behaviorTipsData[1] = LocalStringManager.Get(LanguageKey.UI_EventWindow_OptionBehavior_NearTips);
						}
					}
				}
				refer.CGet<TooltipInvoker>("BehaviorIconTips").PresetParam = behaviorTipsData;
				optionDesc = "「" + LocalStringManager.Get(UI_EventWindow.BehaviorNameKeyList[(int)formatBehavior]) + "」" + optionDesc;
			}
			TextMeshProUGUI consumeInfo = refer.CGet<TextMeshProUGUI>("ConsumeInfo");
			List<OptionConsumeInfo> optionConsumeInfos = optionInfo.OptionConsumeInfos;
			bool hasOptionConsumeInfos = optionConsumeInfos != null && optionConsumeInfos.Count > 0;
			consumeInfo.transform.parent.gameObject.SetActive(hasOptionConsumeInfos);
			bool flag15 = hasOptionConsumeInfos;
			if (flag15)
			{
				List<string> consumeInfoStringList = new List<string>();
				for (int m = 0; m < optionInfo.OptionConsumeInfos.Count; m++)
				{
					consumeInfoStringList.Add(this.Model.GetOptionConsumeInfoTextMeshProSpriteString(optionInfo.OptionConsumeInfos[m]));
				}
				string content = string.Join(" ", consumeInfoStringList).ColorReplace();
				consumeInfo.SetText(content, true);
				consumeInfo.GetComponent<TMPTextSpriteHelper>().Parse();
			}
			refer.CGet<GameObject>("ReadStateContainer").SetActive(optionInfo.OptionState == 1 || optionInfo.OptionState == 2);
			refer.CGet<GameObject>("Read").SetActive(optionInfo.OptionState == 2);
			refer.CGet<GameObject>("UnRead").SetActive(optionInfo.OptionState == 1);
			refer.CGet<CButtonObsolete>("Button").interactable = canSelect;
			refer.GetComponent<CEmptyGraphic>().enabled = canSelect;
			TextMeshProUGUI contentLabel = refer.CGet<TextMeshProUGUI>("OptionDesc");
			CImage image = refer.CGet<CImage>("Back");
			Color color = image.color;
			bool flag16 = !canSelect;
			if (flag16)
			{
				string info3 = "<color=#grey>" + optionDesc + "</color>";
				contentLabel.text = info3.ColorReplace();
				color.a = 0.3f;
			}
			else
			{
				contentLabel.text = optionDesc.ColorReplace();
				color.a = 1f;
			}
			contentLabel.GetComponent<TMPTextSpriteHelper>().Parse();
			image.color = color;
			refer.GetComponent<PointerTrigger>().enabled = canSelect;
			refer.CGet<CButtonObsolete>("Button").ClearAndAddListener(delegate
			{
				this.SelectOption(optionInfo);
			});
			refer.GetComponent<PointerTrigger>().OnPointerExit(null);
			RectTransform rectTrans = refer.GetComponent<RectTransform>();
			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTrans);
			float itemPreferredHeight = Mathf.Max(contentLabel.preferredHeight + 15f, 60f);
			rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemPreferredHeight);
		}
		this.MarkLayoutDirty();
	}

	// Token: 0x0600345F RID: 13407 RVA: 0x001A0E70 File Offset: 0x0019F070
	private int GetMaxLabelTextCount()
	{
		int strMaxCount = 0;
		for (int index = 0; index < this.Data.EventOptionInfos.Count; index++)
		{
			string commandKey = this._optionHotKeyCommands[index].ToString();
			bool flag = index == (int)this.Data.EscOptionIndex;
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

	// Token: 0x06003460 RID: 13408 RVA: 0x001A0EF4 File Offset: 0x0019F0F4
	private void SelectEscOption()
	{
		RectTransform operateAreaTrans = base.CGet<RectTransform>("OperateArea");
		bool flag = !this._animating && this.SelectOptionByIndex((int)this.Data.EscOptionIndex, true) && operateAreaTrans.gameObject.activeSelf;
		if (flag)
		{
			this.ResetData();
			this.AnimOperateAreaOut();
		}
	}

	// Token: 0x06003461 RID: 13409 RVA: 0x001A0F4C File Offset: 0x0019F14C
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

	// Token: 0x06003462 RID: 13410 RVA: 0x001A0FCC File Offset: 0x0019F1CC
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

	// Token: 0x06003463 RID: 13411 RVA: 0x001A1098 File Offset: 0x0019F298
	private void SelectOption(EventOptionInfo optionInfo)
	{
		bool flag = !this.CanSelect || optionInfo.OptionState == -1;
		if (!flag)
		{
			base.CGet<GameObject>("Buttons").SetActive(false);
			bool flag2 = optionInfo.Behavior != 0;
			if (flag2)
			{
				bool flag3 = !EventModel.IgnoreEventBehavior && SingletonObject.getInstance<BasicGameData>().RestrictOptionsBehaviorType;
				if (flag3)
				{
					sbyte taiwuBehavior = GameData.Domains.Character.BehaviorType.GetBehaviorType(this._taiwuDetailInfoMonitor.Behavior);
					bool flag4 = Mathf.Abs((int)(UI_EventWindow.OptionBehaviorToCharacterBehavior(optionInfo.Behavior) - taiwuBehavior)) > 1;
					if (flag4)
					{
						return;
					}
				}
			}
			bool flag5 = this.Model.SelectCombatSkillData != null;
			if (flag5)
			{
				bool flag6 = optionInfo.OptionKey == this.Model.SelectCombatSkillData.OptionKey;
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
							this.Model.SelectCombatSkillData.SelectResultIndex = this.Model.SelectCombatSkillData.CanSelectCombatSkillIdList.IndexOf(skillId);
							TaiwuEventDomainMethod.Call.SetCombatSkillSelectResult(skillId);
							ArgumentBox box = EasyPool.Get<ArgumentBox>();
							box.Set("CombatSkillId", skillId);
							GEvent.OnEvent(UiEvents.EventWindowAppearResult, box);
							this.SelectOptionByOptionKey(this.Model.SelectCombatSkillData.OptionKey);
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
				bool flag9 = optionInfo.OptionKey == this.Model.SelectLifeSkillData.OptionKey;
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
							this.Model.SelectLifeSkillData.SelectResultIndex = this.Model.SelectLifeSkillData.CanSelectLifeSkillIdList.IndexOf(skillId);
							TaiwuEventDomainMethod.Call.SetLifeSkillSelectResult(skillId);
							ArgumentBox box = EasyPool.Get<ArgumentBox>();
							box.Set("LifeSkillId", skillId);
							GEvent.OnEvent(UiEvents.EventWindowAppearResult, box);
							this.SelectOptionByOptionKey(this.Model.SelectLifeSkillData.OptionKey);
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
			bool flag11 = optionInfo.OptionType == 4;
			if (flag11)
			{
				this._animationWindowCompleteAction = delegate()
				{
					UIElement element = this.Element;
					element.OnDeActive = (Action)Delegate.Combine(element.OnDeActive, new Action(this.ResetAnimFadeToHide));
					this.Model.Select(optionInfo.OptionKey, null);
				};
				this.AnimFadeToHide();
			}
			else
			{
				sbyte maskControlCode = this.Data.MaskControlCode;
				bool flag12 = maskControlCode == 2 || maskControlCode == 3 || maskControlCode == 6;
				if (flag12)
				{
					this._layoutDirty = false;
					bool flag13 = needFadeOutCg;
					if (flag13)
					{
						this._animationWindowCompleteAction = new Action(this.AnimCgTextureOut);
						this._animationCgTextureCompleteAction = ((this.Data.MaskControlCode == 6) ? new Action(this.AnimMaskToBlack) : new Action(this.AnimMaskToAlpha));
						this._animationMaskCompleteAction = delegate()
						{
							this.Model.Select(optionInfo.OptionKey, null);
						};
					}
					else
					{
						this._animationWindowCompleteAction = ((this.Data.MaskControlCode == 6) ? new Action(this.AnimMaskToBlack) : new Action(this.AnimMaskToAlpha));
						this._animationMaskCompleteAction = delegate()
						{
							this.Model.Select(optionInfo.OptionKey, null);
						};
					}
					this.AnimEventWindowOut();
				}
				else
				{
					bool flag14 = needFadeOutCg;
					if (flag14)
					{
						this._animationWindowCompleteAction = new Action(this.AnimCgTextureOut);
						this._animationCgTextureCompleteAction = delegate()
						{
							this.Model.Select(optionInfo.OptionKey, null);
						};
						this.AnimEventWindowOut();
					}
					else
					{
						this.Model.Select(optionInfo.OptionKey, null);
					}
				}
			}
			this._waitSelect = false;
		}
	}

	// Token: 0x06003464 RID: 13412 RVA: 0x001A152F File Offset: 0x0019F72F
	private void SetSelecting(bool selecting)
	{
		ConchShipCursor.Instance.SetSelectCountActive(selecting);
	}

	// Token: 0x06003465 RID: 13413 RVA: 0x001A153D File Offset: 0x0019F73D
	private void SetSelectedCount(int selectedCount)
	{
		ConchShipCursor.Instance.SetSelectCountCur(selectedCount, null);
	}

	// Token: 0x06003466 RID: 13414 RVA: 0x001A154C File Offset: 0x0019F74C
	private void SetSelectTargetCount(int targetCount)
	{
		ConchShipCursor.Instance.SetSelectCountMax(targetCount, null);
	}

	// Token: 0x06003467 RID: 13415 RVA: 0x001A155B File Offset: 0x0019F75B
	private void SetSelectingApprovedTaiwu(bool selecting)
	{
		ConchShipCursor.Instance.SetSelectApprovedTaiwuActive(selecting);
	}

	// Token: 0x06003468 RID: 13416 RVA: 0x001A1569 File Offset: 0x0019F769
	private void SetSelectedApprovedTaiwu(string selectedCount)
	{
		ConchShipCursor.Instance.SetSelectApprovedTaiwuCur(selectedCount);
	}

	// Token: 0x06003469 RID: 13417 RVA: 0x001A1577 File Offset: 0x0019F777
	private void SetSelectTargetApprovedTaiwu(string targetCount)
	{
		ConchShipCursor.Instance.SetSelectApprovedTaiwuMax(targetCount);
	}

	// Token: 0x1700059C RID: 1436
	// (get) Token: 0x0600346A RID: 13418 RVA: 0x001A1588 File Offset: 0x0019F788
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

	// Token: 0x0600346B RID: 13419 RVA: 0x001A15E3 File Offset: 0x0019F7E3
	private void RefreshItemSelectScroll()
	{
	}

	// Token: 0x0600346C RID: 13420 RVA: 0x001A15E8 File Offset: 0x0019F7E8
	private void CheckItemSelectAvailable()
	{
		bool flag = this._selectedItemList.Count > 0;
		if (flag)
		{
			base.CGet<CButtonObsolete>("Confirm").interactable = this.Data.ExtraData.SelectItemData.IsAvailableSelectResult(this._selectedItemList);
		}
		else
		{
			base.CGet<CButtonObsolete>("Confirm").interactable = false;
		}
		this.SetSelectedCount(this._selectedItemList.Count);
		this.RefreshConfirmButtonTips();
	}

	// Token: 0x0600346D RID: 13421 RVA: 0x001A1664 File Offset: 0x0019F864
	private void RefreshConfirmButtonTips()
	{
		CButtonObsolete button = base.CGet<CButtonObsolete>("Confirm");
		Transform entry = button.transform.Find("ClickRect");
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
			TooltipInvoker tipDisplayer = entry.GetComponent<TooltipInvoker>();
			tipDisplayer.enabled = !button.interactable;
			bool flag4 = tipDisplayer == null;
			if (flag4)
			{
				tipDisplayer = entry.gameObject.AddComponent<TooltipInvoker>();
			}
			tipDisplayer.Type = TipType.SingleDesc;
			TooltipInvoker tooltipInvoker = tipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tipDisplayer.RuntimeParam.Set("arg0", LocalStringManager.Get(targetKey));
		}
	}

	// Token: 0x0600346E RID: 13422 RVA: 0x001A1770 File Offset: 0x0019F970
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
		base.CGet<ItemScrollView>("ItemScrollView").ReRender();
	}

	// Token: 0x0600346F RID: 13423 RVA: 0x001A1934 File Offset: 0x0019FB34
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

	// Token: 0x06003470 RID: 13424 RVA: 0x001A19AC File Offset: 0x0019FBAC
	private void OnItemRender(ItemDisplayData itemData, ItemView view)
	{
		bool isExchangeTools = this._isExchangeTools;
		if (isExchangeTools)
		{
			this._multiplyItemScrollView.OnRenderItemExchangeTools(itemData, view);
		}
		else
		{
			bool isMultiItemSelect = this._isMultiItemSelect;
			if (isMultiItemSelect)
			{
				this._multiplyItemScrollView.OnRenderItemMultiply(itemData, view);
			}
			else
			{
				this.OnItemRenderSingle(itemData, view);
			}
		}
	}

	// Token: 0x06003471 RID: 13425 RVA: 0x001A19F8 File Offset: 0x0019FBF8
	private void OnItemRenderSingle(ItemDisplayData itemData, ItemView view)
	{
		UI_EventWindow.<>c__DisplayClass132_0 CS$<>8__locals1 = new UI_EventWindow.<>c__DisplayClass132_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.view = view;
		CS$<>8__locals1.itemData = itemData;
		CS$<>8__locals1.scrollView = base.CGet<ItemScrollView>("ItemScrollView");
		bool needHighlight = CS$<>8__locals1.scrollView.UserObject != null && CS$<>8__locals1.scrollView.UserObject.Equals(CS$<>8__locals1.itemData);
		CS$<>8__locals1.view.SetSelectedOrder(false, 0);
		CS$<>8__locals1.selectedCount = 0;
		CS$<>8__locals1.view.SetLocked(CS$<>8__locals1.itemData.UnavailableType > ItemDisplayData.ItemUnavailableType.Valid);
		bool flag = CS$<>8__locals1.itemData.UnavailableType == ItemDisplayData.ItemUnavailableType.NoAlcohol;
		if (flag)
		{
			CS$<>8__locals1.view.SetInteractionStateLockText(LocalStringManager.Get(LanguageKey.LK_ItemUnavailable_NoAlcohol));
		}
		else
		{
			bool flag2 = CS$<>8__locals1.itemData.UnavailableType == ItemDisplayData.ItemUnavailableType.NoMeat;
			if (flag2)
			{
				CS$<>8__locals1.view.SetInteractionStateLockText(LocalStringManager.Get(LanguageKey.LK_ItemUnavailable_NoMeat));
			}
			else
			{
				bool flag3 = CS$<>8__locals1.itemData.UnavailableType == ItemDisplayData.ItemUnavailableType.HighGrade;
				if (flag3)
				{
					CS$<>8__locals1.view.SetInteractionStateLockText(LocalStringManager.Get(LanguageKey.LK_ItemUnavailable_HighGrade));
				}
			}
		}
		bool isMiscResource = ItemTemplateHelper.IsMiscResource(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
		bool flag4 = isMiscResource;
		if (flag4)
		{
			this._selectedItemDict.TryGetValue(CS$<>8__locals1.itemData.Key, out CS$<>8__locals1.selectedCount);
		}
		else
		{
			foreach (ItemKey itemKey2 in this._selectedItemList)
			{
				bool flag5 = CS$<>8__locals1.itemData.ContainsItemKey(itemKey2);
				if (flag5)
				{
					int selectedCount = CS$<>8__locals1.selectedCount;
					CS$<>8__locals1.selectedCount = selectedCount + 1;
				}
			}
		}
		bool isFixItem = this._isFixItem;
		if (isFixItem)
		{
			CS$<>8__locals1.view.ShowDurability();
			sbyte grade = ItemTemplateHelper.GetGrade(CS$<>8__locals1.itemData.Key.ItemType, CS$<>8__locals1.itemData.Key.TemplateId);
			int needAttainment = GameData.Domains.Extra.SharedMethods.GetFixItemAttainmentNeed(grade);
			sbyte needAttainmentType = GameData.Domains.Extra.SharedMethods.GetItemRequireLifeSkillType(CS$<>8__locals1.itemData.Key);
			ItemKey itemKey = CS$<>8__locals1.itemData.Key;
			bool flag6 = !this._fixItemAttainmentMeetDict.ContainsKey(itemKey);
			if (flag6)
			{
				CharacterDomainMethod.AsyncCall.GetLifeSkillAttainment(this, this.Data.TargetCharacter.CharacterId, needAttainmentType, delegate(int offset, RawDataPool dataPool)
				{
					ValueTuple<int, int> skillValue = new ValueTuple<int, int>(-1, -1);
					Serializer.Deserialize(dataPool, offset, ref skillValue);
					bool canSelect2 = skillValue.Item2 >= needAttainment;
					CS$<>8__locals1.<>4__this._fixItemAttainmentMeetDict.Add(itemKey, canSelect2);
					CS$<>8__locals1.<OnItemRenderSingle>g__SetStateInfo|2(canSelect2);
				});
			}
			else
			{
				bool canSelect = this._fixItemAttainmentMeetDict[itemKey];
				CS$<>8__locals1.<OnItemRenderSingle>g__SetStateInfo|2(canSelect);
			}
		}
		else
		{
			bool flag7 = CS$<>8__locals1.selectedCount > 0;
			if (flag7)
			{
				bool isSelectingItemReward = this.Data.ExtraData.SelectItemData.IsSelectingItemReward;
				if (isSelectingItemReward)
				{
					CS$<>8__locals1.view.SetSelectedCount((CS$<>8__locals1.selectedCount > 0) ? CS$<>8__locals1.itemData.Amount : 0);
				}
				else
				{
					CS$<>8__locals1.view.SetSelectedCount(CS$<>8__locals1.selectedCount);
				}
			}
		}
		CS$<>8__locals1.view.SetHighLight(needHighlight && CS$<>8__locals1.selectedCount > 0);
		CS$<>8__locals1.view.SetSelectState(CS$<>8__locals1.selectedCount > 0 && this._isMultiItemSelect);
		ItemLoveHateHandler loveHateHandler = CS$<>8__locals1.view.GetComponent<ItemLoveHateHandler>();
		bool flag8 = null != loveHateHandler && CS$<>8__locals1.itemData.Key.IsValid();
		if (flag8)
		{
			ValueTuple<bool, bool> itemLoveAndHateFlag = this.GetItemLoveAndHateFlag(CS$<>8__locals1.itemData);
			bool love = itemLoveAndHateFlag.Item1;
			bool hate = itemLoveAndHateFlag.Item2;
			loveHateHandler.Refresh(love, hate);
		}
		CS$<>8__locals1.taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		CS$<>8__locals1.view.SetClickEvent(delegate
		{
			bool flag9 = CS$<>8__locals1.itemData.UsingType != ItemDisplayData.ItemUsingType.Invalid && CS$<>8__locals1.itemData.OwnerCharId == CS$<>8__locals1.taiwuCharId;
			if (flag9)
			{
				DialogCmd dialogCmd = new DialogCmd();
				dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
				dialogCmd.Content = CS$<>8__locals1.itemData.GetUsingOperationConfirmTip(ItemDisplayData.ItemUsingOperationType.Default);
				dialogCmd.Type = 1;
				Action yes;
				if ((yes = CS$<>8__locals1.<>9__4) == null)
				{
					yes = (CS$<>8__locals1.<>9__4 = delegate()
					{
						CS$<>8__locals1.scrollView.HandleClickItem(CS$<>8__locals1.itemData, CS$<>8__locals1.view, new Action<ItemView>(base.<OnItemRenderSingle>g__Action|3));
					});
				}
				dialogCmd.Yes = yes;
				DialogCmd cmd = dialogCmd;
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				CS$<>8__locals1.scrollView.HandleClickItem(CS$<>8__locals1.itemData, CS$<>8__locals1.view, new Action<ItemView>(base.<OnItemRenderSingle>g__Action|3));
			}
		});
	}

	// Token: 0x06003472 RID: 13426 RVA: 0x001A1DF8 File Offset: 0x0019FFF8
	private void OnClickConfiscateAll()
	{
		this._selectedItemList.Clear();
		this._selectedItemDict.Clear();
		int count = this._multiplyItemScrollView.MaxSelectCount;
		List<ItemDisplayData> itemList = this._multiplyItemScrollView.CurMultiplyScrollView.SortAndFilter.OutputItemList;
		List<ValueTuple<ItemDisplayData, int>> selectionList = EasyPool.Get<List<ValueTuple<ItemDisplayData, int>>>();
		for (int i = 0; i < itemList.Count; i++)
		{
			ItemDisplayData itemData = itemList[i];
			bool isMiscResource = ItemTemplateHelper.IsMiscResource(itemData.Key.ItemType, itemData.Key.TemplateId);
			int operationCount = Mathf.Min(count, isMiscResource ? 1 : itemData.Amount);
			bool flag = operationCount <= 0;
			if (flag)
			{
				break;
			}
			int selectedCount = isMiscResource ? itemData.Amount : operationCount;
			selectionList.Add(new ValueTuple<ItemDisplayData, int>(itemData, selectedCount));
			count -= operationCount;
		}
		this._multiplyItemScrollView.SelectItem(selectionList);
		EasyPool.Free<List<ValueTuple<ItemDisplayData, int>>>(selectionList);
	}

	// Token: 0x06003473 RID: 13427 RVA: 0x001A1EEC File Offset: 0x001A00EC
	private void RefreshButtonConfiscateAllInteractable()
	{
		CButtonObsolete btnConfiscateAll = base.CGet<Refers>("InteractAreaBottomBG").CGet<CButtonObsolete>("BtnConfiscateAll");
		btnConfiscateAll.interactable = (btnConfiscateAll.gameObject.activeSelf && this.Data.ExtraData.SelectItemData.ItemOperationType == 7 && this._multiplyItemScrollView.CurMultiplyScrollView.SortAndFilter.OutputItemList.Exists((ItemDisplayData data) => !this._multiplyItemScrollView.SelectedMultiplyItemDict.ContainsKey(data)));
	}

	// Token: 0x1700059D RID: 1437
	// (get) Token: 0x06003474 RID: 13428 RVA: 0x001A1F65 File Offset: 0x001A0165
	// (set) Token: 0x06003475 RID: 13429 RVA: 0x001A1F6D File Offset: 0x001A016D
	private int CurCount
	{
		get
		{
			return this._curCount;
		}
		set
		{
			this._curCount = value;
			this.OnCountChange();
		}
	}

	// Token: 0x06003476 RID: 13430 RVA: 0x001A1F80 File Offset: 0x001A0180
	private void RefreshSelectCount()
	{
		Refers setSelectCountRefer = base.CGet<Refers>("SetSelectCount");
		this._buttonMore = setSelectCountRefer.CGet<CButtonObsolete>("ButtonMore");
		this._buttonLess = setSelectCountRefer.CGet<CButtonObsolete>("ButtonLess");
		this._countSlider = setSelectCountRefer.CGet<CSliderLegacy>("Slider");
		this._inputCountField = setSelectCountRefer.CGet<TMP_InputField>("InputField");
		TextMeshProUGUI max = setSelectCountRefer.CGet<TextMeshProUGUI>("Max");
		max.SetText(string.Format("/{0}", this._maxCount), true);
		this._inputCountField.onValueChanged.RemoveAllListeners();
		this._inputCountField.onValueChanged.AddListener(delegate(string value)
		{
			int result;
			int.TryParse(value, out result);
			bool flag = result > this._maxCount;
			if (flag)
			{
				this.CurCount = Math.Clamp(result, this._minCount, this._maxCount);
			}
		});
		this._inputCountField.onEndEdit.RemoveAllListeners();
		this._inputCountField.onEndEdit.AddListener(new UnityAction<string>(this.OnEndCountEdit));
		this._buttonMore.ClearAndAddListener(new Action(this.More));
		this._buttonLess.ClearAndAddListener(new Action(this.Less));
		this._countSlider.onValueChanged.RemoveAllListeners();
		this._countSlider.onValueChanged.AddListener(delegate(float value)
		{
			int result = Mathf.CeilToInt(value * (float)this._maxCount);
			this.CurCount = Math.Clamp(result, this._minCount, this._maxCount);
		});
	}

	// Token: 0x06003477 RID: 13431 RVA: 0x001A20C1 File Offset: 0x001A02C1
	private void More()
	{
		this.CurCount = Math.Clamp(this.CurCount + this._changeValue, this._minCount, this._maxCount);
	}

	// Token: 0x06003478 RID: 13432 RVA: 0x001A20E9 File Offset: 0x001A02E9
	private void Less()
	{
		this.CurCount = Math.Clamp(this.CurCount - this._changeValue, this._minCount, this._maxCount);
	}

	// Token: 0x06003479 RID: 13433 RVA: 0x001A2114 File Offset: 0x001A0314
	private void OnEndCountEdit(string value)
	{
		int result;
		int.TryParse(value, out result);
		this.CurCount = Math.Clamp(result, this._minCount, this._maxCount);
	}

	// Token: 0x0600347A RID: 13434 RVA: 0x001A2144 File Offset: 0x001A0344
	private void OnCountChange()
	{
		this.UpdateButtonState();
		this.UpdateCount();
	}

	// Token: 0x0600347B RID: 13435 RVA: 0x001A2158 File Offset: 0x001A0358
	private void UpdateCount()
	{
		this._inputCountField.SetTextWithoutNotify(this.CurCount.ToString());
		float rate = (float)this.CurCount / (float)this._maxCount;
		this._countSlider.SetValueWithoutNotify(rate);
	}

	// Token: 0x0600347C RID: 13436 RVA: 0x001A21A0 File Offset: 0x001A03A0
	private void UpdateButtonState()
	{
		bool flag = this._buttonMore;
		if (flag)
		{
			this._buttonMore.interactable = (this.CurCount < this._maxCount);
		}
		bool flag2 = this._buttonLess;
		if (flag2)
		{
			this._buttonLess.interactable = (this.CurCount > this._minCount);
		}
	}

	// Token: 0x0600347D RID: 13437 RVA: 0x001A2204 File Offset: 0x001A0404
	private void RefreshReadingBookCountPanel()
	{
		base.CGet<GameObject>("Buttons").SetActive(true);
		this._waitSelect = true;
		Refers refers = base.CGet<Refers>("SelectReadingBookCount");
		ItemView itemView = refers.CGet<ItemView>("ItemView");
		TextMeshProUGUI itemViewName = refers.CGet<TextMeshProUGUI>("ReadingBookName");
		TextMeshProUGUI durabilityValue = refers.CGet<TextMeshProUGUI>("DurabilityValue");
		ItemDisplayData bookData = this.Data.ExtraData.SelectReadingBookCountData.SelectedBookData;
		this._maxCount = this.Data.ExtraData.SelectReadingBookCountData.MaxReadingBookCount;
		itemView.SetData(bookData, false, -1, false, true, null, false, true);
		sbyte grade = ItemTemplateHelper.GetGrade(bookData.Key.ItemType, bookData.Key.TemplateId);
		itemViewName.SetText(ItemTemplateHelper.GetName(bookData.Key.ItemType, bookData.Key.TemplateId).SetColor(Colors.Instance.GradeColors[(int)grade]), true);
		durabilityValue.SetText(MouseTip_Util.GetItemDurabilityColorString((int)bookData.Durability, (int)bookData.MaxDurability), true);
		this.RefreshSelectCount();
		this.CurCount = 1;
	}

	// Token: 0x0600347E RID: 13438 RVA: 0x001A2324 File Offset: 0x001A0524
	private void RefreshNeigongLoopingCountPanel()
	{
		base.CGet<GameObject>("Buttons").SetActive(true);
		this._waitSelect = true;
		Refers refers = base.CGet<Refers>("SelectNeigongLoopingCount");
		CombatSkillView combatSkillView = refers.CGet<CombatSkillView>("CombatSkillView");
		combatSkillView.SetData(this.Data.ExtraData.SelectNeigongLoopingCountData.SelectedCombatSkill, true, false, true, false);
		this._maxCount = this.Data.ExtraData.SelectNeigongLoopingCountData.MaxLoopingCount;
		this.RefreshSelectCount();
		this.CurCount = 1;
	}

	// Token: 0x0600347F RID: 13439 RVA: 0x001A23B0 File Offset: 0x001A05B0
	private void RefreshSelectFameActionPanel()
	{
		base.CGet<GameObject>("Buttons").SetActive(true);
		base.CGet<CButtonObsolete>("Confirm").interactable = false;
		this._fameActionSortIndex = 0;
		this._fameExistTimeSortIndex = 0;
		this._waitSelect = true;
		this._selectedFameActionIdList.Clear();
		this._canSelectedFameActionIdList.Clear();
		this._fameActionIdToIndex.Clear();
		bool flag = this.Data.ExtraData.SelectFameData.fameActionRecords != null;
		if (flag)
		{
			foreach (FameActionRecord item in this.Data.ExtraData.SelectFameData.fameActionRecords)
			{
				this._canSelectedFameActionIdList.Add(item.Id);
				this._fameActionIdToIndex.Add(item.Id, this._canSelectedFameActionIdList.Count - 1);
			}
		}
		InfinityScrollLegacy fameActionScrollView = base.CGet<InfinityScrollLegacy>("FameActionScrollView");
		fameActionScrollView.OnItemRender = new Action<int, Refers>(this.OnFameActionItemRender);
		fameActionScrollView.SetDataCount(this._canSelectedFameActionIdList.Count);
		Refers refers = fameActionScrollView.transform.GetComponent<Refers>();
		this.SetAuthorityCostAndButtons(refers);
		CButtonObsolete allSelectButton = refers.CGet<CButtonObsolete>("AllSelectButton");
		this._fameActionButton = refers.CGet<Refers>("FameAction");
		this._existTimeButton = refers.CGet<Refers>("ExistTime");
		this.UpdateFameActionSort(false);
		allSelectButton.ClearAndAddListener(delegate
		{
			bool flag2 = this._selectedFameActionIdList.Count != this._canSelectedFameActionIdList.Count;
			if (flag2)
			{
				this._selectedFameActionIdList.Clear();
				this._selectedFameActionIdList.AddRange(this._canSelectedFameActionIdList);
			}
			else
			{
				this._selectedFameActionIdList.Clear();
			}
			fameActionScrollView.ReRender();
			this.SetAuthorityCostAndButtons(refers);
		});
		this._fameActionButton.GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
		{
			this._fameActionSortIndex = (this._fameActionSortIndex + 1) % 3;
			this._fameExistTimeSortIndex = 0;
			this.UpdateFameActionSort(true);
		});
		this._existTimeButton.GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
		{
			this._fameExistTimeSortIndex = (this._fameExistTimeSortIndex + 1) % 3;
			this._fameActionSortIndex = 0;
			this.UpdateFameActionSort(false);
		});
	}

	// Token: 0x06003480 RID: 13440 RVA: 0x001A25C0 File Offset: 0x001A07C0
	private void UpdateFameActionSort(bool isFameAction = false)
	{
		UI_EventWindow.<>c__DisplayClass163_0 CS$<>8__locals1 = new UI_EventWindow.<>c__DisplayClass163_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.isFameAction = isFameAction;
		UI_EventWindow.<UpdateFameActionSort>g__SetCheckMark|163_2(this._fameActionButton, this._fameActionSortIndex);
		UI_EventWindow.<UpdateFameActionSort>g__SetCheckMark|163_2(this._existTimeButton, this._fameExistTimeSortIndex);
		CS$<>8__locals1.currDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
		List<FameActionRecord> fameActionRecords = this.Data.ExtraData.SelectFameData.fameActionRecords;
		bool flag = fameActionRecords == null || fameActionRecords.Count<FameActionRecord>() == 0;
		if (!flag)
		{
			bool flag2 = this._fameActionSortIndex != 0 || this._fameExistTimeSortIndex != 0;
			if (flag2)
			{
				bool flag3 = this._fameActionSortIndex > 1 || this._fameExistTimeSortIndex > 1;
				if (flag3)
				{
					fameActionRecords.Sort(new Comparison<FameActionRecord>(CS$<>8__locals1.<UpdateFameActionSort>g__comparison|3));
				}
				else
				{
					fameActionRecords.Sort((FameActionRecord a, FameActionRecord b) => -base.<UpdateFameActionSort>g__comparison|3(a, b));
				}
			}
			else
			{
				fameActionRecords.Sort((FameActionRecord a, FameActionRecord b) => CS$<>8__locals1.<>4__this._fameActionIdToIndex[a.Id].CompareTo(CS$<>8__locals1.<>4__this._fameActionIdToIndex[b.Id]));
			}
			InfinityScrollLegacy fameActionScrollView = base.CGet<InfinityScrollLegacy>("FameActionScrollView");
			fameActionScrollView.ReRender();
		}
	}

	// Token: 0x06003481 RID: 13441 RVA: 0x001A26D4 File Offset: 0x001A08D4
	private int GetAuthorityCost(int index)
	{
		FameActionRecord fameAction = this.Data.ExtraData.SelectFameData.fameActionRecords[index];
		FameActionItem config = FameAction.Instance[fameAction.Id];
		return (int)(Math.Abs(fameAction.Value) * config.ReductionTime * 20);
	}

	// Token: 0x06003482 RID: 13442 RVA: 0x001A2728 File Offset: 0x001A0928
	private void SetAuthorityCostAndButtons(Refers refers)
	{
		int authorityCost = 0;
		for (int i = 0; i < this._canSelectedFameActionIdList.Count; i++)
		{
			bool flag = this._selectedFameActionIdList.Contains(this._canSelectedFameActionIdList[i]);
			if (flag)
			{
				authorityCost += this.GetAuthorityCost(i);
			}
		}
		refers.CGet<TextMeshProUGUI>("CostText").text = authorityCost.ToString().SetColor("pinkyellow");
		TextMeshProUGUI haveText = refers.CGet<TextMeshProUGUI>("HaveText");
		int haveValue = SingletonObject.getInstance<BuildingModel>().GetResourceCount(ItemSourceType.Resources, 7);
		haveText.text = CommonUtils.GetDisplayStringForNum(haveValue, 100000).SetColor((haveValue < authorityCost) ? "brightred" : "brightblue");
		base.CGet<CButtonObsolete>("Confirm").interactable = (this._selectedFameActionIdList.Count > 0 && authorityCost < haveValue);
		refers.CGet<CToggleObsolete>("SelectBtn").isOn = (this._selectedFameActionIdList.Count == this._canSelectedFameActionIdList.Count);
	}

	// Token: 0x06003483 RID: 13443 RVA: 0x001A2830 File Offset: 0x001A0A30
	private void OnFameActionItemRender(int index, Refers refers)
	{
		FameActionRecord fameAction = this.Data.ExtraData.SelectFameData.fameActionRecords[index];
		FameActionItem config = FameAction.Instance[fameAction.Id];
		string color = (config.Fame > 0) ? "brightblue" : "brightred";
		int currDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
		refers.CGet<TextMeshProUGUI>("FameNameText").text = config.Name.SetColor(color);
		string fameActionText = (fameAction.Value > 0) ? string.Format("+{0}", fameAction.Value) : fameAction.Value.ToString();
		refers.CGet<TextMeshProUGUI>("FameActionText").text = fameActionText.SetColor(color);
		refers.CGet<TextMeshProUGUI>("CurrentText").text = (fameAction.EndDate - currDate).ToString().SetColor("pinkyellow");
		refers.CGet<TextMeshProUGUI>("PreviewText").text = Math.Max(fameAction.EndDate - currDate - (int)config.ReductionTime, 0).ToString().SetColor("orange");
		short fameActionId = fameAction.Id;
		CToggleObsolete toggle = refers.CGet<CToggleObsolete>("SelectBtn");
		toggle.onValueChanged.RemoveAllListeners();
		toggle.isOn = this._selectedFameActionIdList.Contains(fameActionId);
		toggle.onValueChanged.AddListener(delegate(bool isOn)
		{
			bool flag = !isOn;
			if (flag)
			{
				this._selectedFameActionIdList.Remove(fameActionId);
			}
			else
			{
				this._selectedFameActionIdList.Add(fameActionId);
			}
			this.RefreshConfirmButtonTips();
			InfinityScrollLegacy fameActionScrollView = this.CGet<InfinityScrollLegacy>("FameActionScrollView");
			Refers refers2 = fameActionScrollView.transform.GetComponent<Refers>();
			this.SetAuthorityCostAndButtons(refers2);
		});
		CButtonObsolete mark = refers.CGet<CButtonObsolete>("Select");
		mark.ClearAndAddListener(delegate
		{
			toggle.isOn = !toggle.isOn;
		});
	}

	// Token: 0x06003484 RID: 13444 RVA: 0x001A29F0 File Offset: 0x001A0BF0
	private void RefreshCharacterSelectScroll()
	{
		this._canSelectCharIdList.Clear();
		this._canSelectCharacterDisplayDataList.Clear();
		foreach (CharacterSelectFilter filter in this.Data.ExtraData.SelectCharacterData.FilterList)
		{
			bool flag = filter.AvailableCharactersDisplayDataList == null;
			if (!flag)
			{
				foreach (CharacterDisplayData characterDisplayData in filter.AvailableCharactersDisplayDataList)
				{
					bool flag2 = !this._canSelectCharacterDisplayDataList.Contains(characterDisplayData);
					if (flag2)
					{
						this._canSelectCharacterDisplayDataList.Add(characterDisplayData);
						this._canSelectCharIdList.Add(characterDisplayData.CharacterId);
					}
				}
			}
		}
		InfinityScrollLegacy characterScrollView = base.CGet<InfinityScrollLegacy>("CharacterScrollView");
		characterScrollView.OnItemRender = new Action<int, Refers>(this.OnCharacterItemRender);
		characterScrollView.OnItemHide = new Action<Refers>(this.OnCharacterItemHide);
		characterScrollView.SetDataCount(this._canSelectCharacterDisplayDataList.Count);
		base.CGet<CButtonObsolete>("Confirm").interactable = false;
		this.RefreshConfirmButtonTips();
		this.SetSelectedCount(0);
		this.SetSelectTargetCount(this.Data.ExtraData.SelectCharacterData.FilterList.Count);
		this.SetSelecting(true);
		this._needSelectItem = true;
		this._waitSelect = true;
		this.MarkLayoutDirty();
		base.CGet<GameObject>("Buttons").SetActive(true);
	}

	// Token: 0x06003485 RID: 13445 RVA: 0x001A2BAC File Offset: 0x001A0DAC
	private void OnCharacterItemRender(int index, Refers refersHolder)
	{
		Refers refers = refersHolder.CGet<Refers>("SelectableCharacter");
		CharacterDisplayData characterDisplayData = this._canSelectCharacterDisplayDataList[index];
		int charId = characterDisplayData.CharacterId;
		refers.GetComponent<SelectableCharacter>().SetData(characterDisplayData);
		refers.CGet<GameObject>("Selected").SetActive(this._selectedCharIdList.Contains(charId));
		bool flag = characterDisplayData.BountyOrgTemplate == characterDisplayData.CurrOrgTemplate;
		if (flag)
		{
			CImage bountyPunishmentSeverity = refers.CGet<CImage>("BountyPunishmentSeverity");
			bountyPunishmentSeverity.gameObject.SetActive(true);
			bountyPunishmentSeverity.SetSprite(string.Format("prison_icon_seize_{0}", characterDisplayData.BountyPunishmentSeverity), false, null);
		}
		else
		{
			refers.CGet<CImage>("BountyPunishmentSeverity").gameObject.SetActive(false);
		}
		CToggleObsolete toggle = refers.CGet<CToggleObsolete>("Toggle");
		Refers approvingRateRefers = refers.CGet<Refers>("ApprovingRateHolder");
		approvingRateRefers.gameObject.SetActive(false);
		CharacterItem config = Character.Instance[characterDisplayData.TemplateId];
		toggle.interactable = this.CanSelectCharacter(config);
		DisableStyleRoot disableStyleRoot = refers.CGet<DisableStyleRoot>("DisableStyleRoot");
		disableStyleRoot.SetStyleEffect(!toggle.interactable, false);
		toggle.onValueChanged.RemoveAllListeners();
		toggle.isOn = this._selectedCharIdList.Contains(charId);
		toggle.onValueChanged.AddListener(delegate(bool isOn)
		{
			bool flag5 = !isOn;
			if (flag5)
			{
				this._selectedCharIdList.Remove(charId);
			}
			else
			{
				bool flag6 = this.Data.ExtraData.SelectCharacterData.FilterList.Count == 1;
				if (flag6)
				{
					this._selectedCharIdList.Clear();
				}
				this._selectedCharIdList.Add(charId);
			}
			this.CGet<CButtonObsolete>("Confirm").interactable = this.Data.ExtraData.SelectCharacterData.IsAvailableSelectResult(this._selectedCharIdList);
			this.RefreshConfirmButtonTips();
			this.CGet<InfinityScrollLegacy>("CharacterScrollView").ReRender();
			this.SetSelectedCount(this._selectedCharIdList.Count);
		});
		TooltipInvoker mouseTipDisplayer = refers.CGet<TooltipInvoker>("MouseTip");
		bool flag2 = this.CanSelectCharacter(config);
		if (flag2)
		{
			mouseTipDisplayer.Type = TipType.Character;
			mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("charId", charId);
		}
		else
		{
			mouseTipDisplayer.Type = TipType.SingleDesc;
			bool flag3 = config.SpecialTemmateType == ECharacterSpecialTemmateType.BeastCarrier;
			if (flag3)
			{
				mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("arg0", LocalStringManager.Get(LanguageKey.LK_EventWindow_SpecialTeammate_BeastCarrier_Tip));
			}
			else
			{
				bool flag4 = config.SpecialTemmateType == ECharacterSpecialTemmateType.GearMate;
				if (flag4)
				{
					mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("arg0", LocalStringManager.Get(LanguageKey.LK_EventWindow_SpecialTeammate_GearMate_Tip));
				}
			}
		}
	}

	// Token: 0x06003486 RID: 13446 RVA: 0x001A2DE0 File Offset: 0x001A0FE0
	private bool CanSelectCharacter(CharacterItem config)
	{
		return config.SpecialTemmateType == ECharacterSpecialTemmateType.Invalid;
	}

	// Token: 0x06003487 RID: 13447 RVA: 0x001A2DFC File Offset: 0x001A0FFC
	private void OnCharacterItemHide(Refers refersHolder)
	{
		Refers refers = refersHolder.CGet<Refers>("SelectableCharacter");
		refers.CGet<GameObject>("Selected").SetActive(false);
	}

	// Token: 0x06003488 RID: 13448 RVA: 0x001A2E28 File Offset: 0x001A1028
	private void RefreshCharacterSelectApprovedTaiwuScroll()
	{
		this._canSelectCharIdList.Clear();
		this._isEnough = false;
		foreach (KeyValuePair<int, short> pair in this.Data.ExtraData.SelectCharacterData.SelectApprovedTaiwu.CharacterApprovingRate)
		{
			bool flag = !this._canSelectCharIdList.Contains(pair.Key);
			if (flag)
			{
				this._canSelectCharIdList.Add(pair.Key);
			}
		}
		InfinityScrollLegacy characterScrollView = base.CGet<InfinityScrollLegacy>("CharacterScrollView");
		characterScrollView.OnItemRender = new Action<int, Refers>(this.OnCharacterApprovedTaiwuItemRender);
		characterScrollView.OnItemHide = new Action<Refers>(this.OnCharacterApprovedTaiwuItemHide);
		characterScrollView.SetDataCount(this._canSelectCharIdList.Count);
		base.CGet<CButtonObsolete>("Confirm").interactable = false;
		this.RefreshConfirmButtonTips();
		this.SetSelectedApprovedTaiwu(GameData.Domains.World.SharedMethods.GetApproveTaiwuDisplayDataString(0).SetColor("brightred"));
		this.SetSelectTargetApprovedTaiwu(GameData.Domains.World.SharedMethods.GetApproveTaiwuDisplayDataString(this.Data.ExtraData.SelectCharacterData.SelectApprovedTaiwu.TargetApprovingRate).SetColor("pinkyellow"));
		this.SetSelectingApprovedTaiwu(true);
		this._needSelectApprovedTaiwu = true;
		this._waitSelect = true;
		this.MarkLayoutDirty();
		base.CGet<GameObject>("Buttons").SetActive(true);
	}

	// Token: 0x06003489 RID: 13449 RVA: 0x001A2FA0 File Offset: 0x001A11A0
	private void OnCharacterApprovedTaiwuItemRender(int index, Refers refersHolder)
	{
		UI_EventWindow.<>c__DisplayClass176_0 CS$<>8__locals1 = new UI_EventWindow.<>c__DisplayClass176_0();
		CS$<>8__locals1.<>4__this = this;
		Refers refers = refersHolder.CGet<Refers>("SelectableCharacter");
		refers.CGet<CImage>("BountyPunishmentSeverity").gameObject.SetActive(false);
		CS$<>8__locals1.charId = this._canSelectCharIdList[index];
		refers.GetComponent<SelectableCharacter>().CharacterId = CS$<>8__locals1.charId;
		GameObject selectedGo = refers.CGet<GameObject>("Selected");
		selectedGo.SetActive(this._selectedCharIdList.Contains(CS$<>8__locals1.charId));
		Refers approvingRateRefers = refers.CGet<Refers>("ApprovingRateHolder");
		approvingRateRefers.gameObject.SetActive(true);
		short approvingRate = this.GetApprovingRate(CS$<>8__locals1.charId);
		double displayValue = GameData.Domains.World.SharedMethods.GetApproveTaiwuDisplayData(approvingRate);
		approvingRateRefers.CGet<TextMeshProUGUI>("ApprovingRateText").text = LocalStringManager.GetFormat(LanguageKey.LK_MapBlockCharList_Approve, displayValue);
		CImage approvingIcon = approvingRateRefers.CGet<CImage>("ApprovingRateIcon");
		approvingIcon.SetSprite(this.IsCharacterHaveDukeTitle(CS$<>8__locals1.charId) ? "blockchar_icon_wanggongzhichi" : "blockchar_icon_zhichi", false, null);
		approvingIcon.SetNativeSize();
		DisableStyleRoot disableStyleRoot = refers.CGet<DisableStyleRoot>("DisableStyleRoot");
		CToggleObsolete toggle = refers.CGet<CToggleObsolete>("Toggle");
		toggle.onValueChanged.RemoveAllListeners();
		toggle.isOn = this._selectedCharIdList.Contains(CS$<>8__locals1.charId);
		bool isEnough = this._isEnough;
		if (isEnough)
		{
			bool flag = this._selectedCharIdList.Contains(CS$<>8__locals1.charId);
			if (flag)
			{
				toggle.onValueChanged.AddListener(new UnityAction<bool>(CS$<>8__locals1.<OnCharacterApprovedTaiwuItemRender>g__Action|0));
				toggle.interactable = true;
				disableStyleRoot.SetStyleEffect(false, false);
			}
			else
			{
				toggle.interactable = false;
				disableStyleRoot.SetStyleEffect(true, false);
			}
		}
		else
		{
			toggle.onValueChanged.AddListener(new UnityAction<bool>(CS$<>8__locals1.<OnCharacterApprovedTaiwuItemRender>g__Action|0));
			toggle.interactable = true;
			disableStyleRoot.SetStyleEffect(false, false);
		}
		TooltipInvoker mouseTipDisplayer = refers.CGet<TooltipInvoker>("MouseTip");
		mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("charId", CS$<>8__locals1.charId);
	}

	// Token: 0x0600348A RID: 13450 RVA: 0x001A31B8 File Offset: 0x001A13B8
	private void OnCharacterApprovedTaiwuItemHide(Refers refersHolder)
	{
		Refers refers = refersHolder.CGet<Refers>("SelectableCharacter");
		refers.GetComponent<SelectableCharacter>().ResetToEmpty();
		refers.CGet<GameObject>("Selected").SetActive(false);
	}

	// Token: 0x0600348B RID: 13451 RVA: 0x001A31F0 File Offset: 0x001A13F0
	private bool SelectApprovedTaiwuResult(List<int> selectedCharIdList)
	{
		return this.GetTotalApprovingRate(selectedCharIdList) >= this.Data.ExtraData.SelectCharacterData.SelectApprovedTaiwu.TargetApprovingRate;
	}

	// Token: 0x0600348C RID: 13452 RVA: 0x001A3228 File Offset: 0x001A1428
	private short GetTotalApprovingRate(List<int> selectedCharIdList)
	{
		short totalApprovingRate = 0;
		for (int i = 0; i < selectedCharIdList.Count; i++)
		{
			int charId = selectedCharIdList[i];
			short approvingRate = this.GetApprovingRate(charId);
			totalApprovingRate += approvingRate;
		}
		return totalApprovingRate;
	}

	// Token: 0x0600348D RID: 13453 RVA: 0x001A3270 File Offset: 0x001A1470
	private short GetApprovingRate(int charId)
	{
		short approvingRate;
		this.Data.ExtraData.SelectCharacterData.SelectApprovedTaiwu.CharacterApprovingRate.TryGetValue(charId, out approvingRate);
		return approvingRate;
	}

	// Token: 0x0600348E RID: 13454 RVA: 0x001A32A8 File Offset: 0x001A14A8
	private bool IsCharacterHaveDukeTitle(int charId)
	{
		return this.Data.ExtraData.SelectCharacterData.SelectApprovedTaiwu.DukeTitleCharIdList != null && this.Data.ExtraData.SelectCharacterData.SelectApprovedTaiwu.DukeTitleCharIdList.Contains(charId);
	}

	// Token: 0x0600348F RID: 13455 RVA: 0x001A3304 File Offset: 0x001A1504
	private void RefreshInputPanel()
	{
		TMP_InputField inputField = base.CGet<TMP_InputField>("InputField");
		CanvasGroup canvasGroup = base.CGet<CanvasGroup>("SensitiveWarningTip");
		canvasGroup.alpha = 0f;
		inputField.text = string.Empty;
		TextMeshProUGUI placeHolder = (TextMeshProUGUI)inputField.placeholder;
		int[] range = this.Data.ExtraData.InputRequestData.NumberRange;
		base.CGet<RandomNameButton>("BtnRandomName").Refresh(inputField, this.Data.ExtraData.InputRequestData.InputDataType);
		switch (this.Data.ExtraData.InputRequestData.InputDataType)
		{
		case 0:
			placeHolder.text = LocalStringManager.Get(LanguageKey.LK_EventInput_StringHolderTips);
			inputField.contentType = TMP_InputField.ContentType.Standard;
			break;
		case 1:
		{
			inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
			bool flag = range == null;
			if (flag)
			{
				placeHolder.text = LocalStringManager.Get(LanguageKey.LK_EventInput_StringHolderTips);
			}
			else
			{
				placeHolder.text = LocalStringManager.GetFormat(LanguageKey.LK_EventInput_IntegerTips, range[0], range[1]);
			}
			break;
		}
		case 2:
			placeHolder.text = LocalStringManager.Get(LanguageKey.LK_EventInput_NameHolderTips);
			inputField.contentType = TMP_InputField.ContentType.Name;
			inputField.characterLimit = range[1];
			break;
		case 3:
			placeHolder.text = LocalStringManager.Get(LanguageKey.LK_EventInput_GivenNameHolderTips);
			inputField.contentType = TMP_InputField.ContentType.Name;
			inputField.characterLimit = range[1];
			break;
		case 4:
			placeHolder.text = LocalStringManager.Get(LanguageKey.LK_EventInput_SurNameHolderTips);
			inputField.contentType = TMP_InputField.ContentType.Name;
			inputField.characterLimit = range[1];
			break;
		default:
			inputField.contentType = TMP_InputField.ContentType.Standard;
			break;
		}
		inputField.onValueChanged.RemoveAllListeners();
		inputField.onValueChanged.AddListener(new UnityAction<string>(this.OnInputValueChanged));
		inputField.onEndEdit.RemoveAllListeners();
		inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEndEditCheckSensitiveWord));
		base.CGet<TextMeshProUGUI>("InputTips").text = string.Empty;
		base.CGet<TextMeshProUGUI>("InputTips").text = string.Empty;
		this._waitSelect = true;
		this.MarkLayoutDirty();
		base.CGet<CButtonObsolete>("Cancel").gameObject.SetActive(this.Data.EventOptionInfos.Count == 2);
		base.CGet<GameObject>("Buttons").SetActive(true);
		bool canConfirm = !inputField.text.IsNullOrEmpty();
		base.CGet<CButtonObsolete>("Confirm").interactable = canConfirm;
		this.RefreshConfirmButtonTips();
	}

	// Token: 0x06003490 RID: 13456 RVA: 0x001A358C File Offset: 0x001A178C
	private void OnInputValueChanged(string inputStr)
	{
		bool canConfirm = false;
		bool flag = this.Data.ExtraData.InputRequestData.InputDataType == 1;
		if (flag)
		{
			int intValue;
			bool flag2 = int.TryParse(inputStr, out intValue);
			if (flag2)
			{
				bool flag3 = this.Data.ExtraData.InputRequestData.NumberRange != null;
				if (flag3)
				{
					int[] range = this.Data.ExtraData.InputRequestData.NumberRange;
					bool flag4 = intValue >= range[0] && intValue <= range[1];
					if (flag4)
					{
						canConfirm = true;
						base.CGet<TextMeshProUGUI>("InputTips").text = string.Empty;
					}
					else
					{
						base.CGet<TextMeshProUGUI>("InputTips").text = LocalStringManager.GetFormat(LanguageKey.LK_EventInput_IntegerTips, range[0], range[1]);
					}
				}
				else
				{
					canConfirm = true;
					base.CGet<TextMeshProUGUI>("InputTips").text = string.Empty;
				}
			}
		}
		else
		{
			bool flag5 = this.Data.ExtraData.InputRequestData.InputDataType == 2 || this.Data.ExtraData.InputRequestData.InputDataType == 3;
			if (flag5)
			{
				int[] lengthRange = this.Data.ExtraData.InputRequestData.NumberRange;
				TMP_InputField inputField = base.CGet<TMP_InputField>("InputField");
				bool flag6 = CommonUtils.FixToShowAbleString(ref inputStr, inputField.textComponent.font);
				if (flag6)
				{
					inputField.SetTextWithoutNotify(inputStr);
				}
				bool flag7 = inputField.text.IsNullOrEmpty();
				if (flag7)
				{
					this.Data.ExtraData.InputRequestData.ConfirmDisableTips = "LK_Event_InputMode_ConfirmButtonTips_NoInput";
				}
				canConfirm = (!inputStr.IsNullOrEmpty() && inputStr.Length <= lengthRange[1] && inputStr.Length >= lengthRange[0]);
				canConfirm = (canConfirm && !NameCenter.HasInvalidCharForName(inputStr));
			}
			else
			{
				canConfirm = (inputStr.Length > 0);
			}
		}
		base.CGet<CButtonObsolete>("Confirm").interactable = canConfirm;
		this.RefreshConfirmButtonTips();
	}

	// Token: 0x06003491 RID: 13457 RVA: 0x001A379C File Offset: 0x001A199C
	private void OnEndEditCheckSensitiveWord(string inputStr)
	{
		bool flag = inputStr.IsNullOrEmpty();
		if (!flag)
		{
			TMP_InputField inputField = base.CGet<TMP_InputField>("InputField");
			bool hasSensitiveWord = false;
			TaiwuEventDisplayData data = this.Data;
			sbyte? b;
			if (data == null)
			{
				b = null;
			}
			else
			{
				TaiwuEventDisplayExtraData extraData = data.ExtraData;
				if (extraData == null)
				{
					b = null;
				}
				else
				{
					EventInputRequestData inputRequestData = extraData.InputRequestData;
					b = ((inputRequestData != null) ? new sbyte?(inputRequestData.InputDataType) : null);
				}
			}
			sbyte? b2 = b;
			int? num = (b2 != null) ? new int?((int)b2.GetValueOrDefault()) : null;
			int num2 = 1;
			bool flag2 = !(num.GetValueOrDefault() == num2 & num != null);
			if (flag2)
			{
				hasSensitiveWord = inputField.SensitiveWordHandle(ref inputStr);
			}
			bool flag3 = hasSensitiveWord;
			if (flag3)
			{
				CanvasGroup canvasGroup = base.CGet<CanvasGroup>("SensitiveWarningTip");
				canvasGroup.alpha = 1f;
				bool flag4 = this._sensitiveWordTipCoroutine != null;
				if (flag4)
				{
					base.StopCoroutine(this._sensitiveWordTipCoroutine);
				}
				Tween sensitiveWordTipTween = this._sensitiveWordTipTween;
				if (sensitiveWordTipTween != null)
				{
					sensitiveWordTipTween.Kill(false);
				}
				this._sensitiveWordTipCoroutine = base.DelayCallReturnCoroutine(delegate
				{
					bool activeInHierarchy = canvasGroup.gameObject.activeInHierarchy;
					if (activeInHierarchy)
					{
						this._sensitiveWordTipTween = canvasGroup.DOFade(0f, SensitiveWordsSystem.SensitiveWordAnimationFadeTime);
					}
				}, SensitiveWordsSystem.SensitiveWordAnimationStayTime);
				inputField.SetTextWithoutNotify(string.Empty);
				this.Data.ExtraData.InputRequestData.ConfirmDisableTips = "LK_Event_InputMode_ConfirmButtonTips_NoInput";
				base.CGet<CButtonObsolete>("Confirm").interactable = false;
				this.RefreshConfirmButtonTips();
			}
		}
	}

	// Token: 0x06003492 RID: 13458 RVA: 0x001A3928 File Offset: 0x001A1B28
	private void RefreshSelectAvatarScroll()
	{
		this._selectedAvatarIndex = -1;
		base.CGet<InfinityScrollLegacy>("CharacterScrollView").OnItemRender = new Action<int, Refers>(this.OnAvatarItemRender);
		base.CGet<InfinityScrollLegacy>("CharacterScrollView").OnItemHide = new Action<Refers>(this.OnAvatarItemHide);
		base.CGet<InfinityScrollLegacy>("CharacterScrollView").SetDataCount(this.Data.ExtraData.SelectOneAvatarRelatedDataList.Count - 1);
		base.CGet<CButtonObsolete>("Confirm").interactable = false;
		this.RefreshConfirmButtonTips();
		this.SetSelectedCount(0);
		this.SetSelectTargetCount(1);
		this.SetSelecting(true);
		this._needSelectItem = true;
		this._waitSelect = true;
		this.MarkLayoutDirty();
		base.CGet<GameObject>("Buttons").SetActive(true);
	}

	// Token: 0x06003493 RID: 13459 RVA: 0x001A39F8 File Offset: 0x001A1BF8
	private void OnAvatarItemRender(int index, Refers refersHolder)
	{
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
						this.CGet<InfinityScrollLegacy>("CharacterScrollView").RefreshCell(prevIndex);
					}
					refers.CGet<GameObject>("Selected").SetActive(true);
					this.CGet<CButtonObsolete>("Confirm").interactable = true;
					this.RefreshConfirmButtonTips();
					this.SetSelectedCount(1);
				}
				else
				{
					bool flag3 = this._selectedAvatarIndex == index;
					if (flag3)
					{
						this.SetSelectedCount(0);
						this.CGet<CButtonObsolete>("Confirm").interactable = false;
						this.RefreshConfirmButtonTips();
					}
				}
			});
			TooltipInvoker mouseTipDisplayer = refers.CGet<TooltipInvoker>("MouseTip");
			mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().SetObject("avatar", data);
		}
	}

	// Token: 0x06003494 RID: 13460 RVA: 0x001A3B8E File Offset: 0x001A1D8E
	private void OnAvatarItemHide(Refers refers)
	{
		refers.CGet<Game.Components.Avatar.Avatar>("Avatar").ResetToBlank(false);
		refers.CGet<GameObject>("Selected").SetActive(false);
	}

	// Token: 0x06003495 RID: 13461 RVA: 0x001A3BB8 File Offset: 0x001A1DB8
	private void SetCommonOption()
	{
		Refers commonOption = base.CGet<Refers>("CommonOption");
		CharacterDisplayData targetCharacter = this.Data.TargetCharacter;
		bool flag = targetCharacter == null || targetCharacter.CreatingType != 1;
		if (flag)
		{
			commonOption.gameObject.SetActive(false);
		}
		else
		{
			commonOption.gameObject.SetActive(this.Data.ExtraData.ShowCommonOptionIndex >= 0);
			bool flag2 = this.Data.ExtraData.ShowCommonOptionIndex >= 0;
			if (flag2)
			{
				this.SetCommonOptionInfo();
			}
		}
	}

	// Token: 0x06003496 RID: 13462 RVA: 0x001A3C4C File Offset: 0x001A1E4C
	private void SetCommonOptionInfo()
	{
		bool isCommonOptionInit = this._isCommonOptionInit;
		if (!isCommonOptionInit)
		{
			this._isCommonOptionInit = true;
			Refers commonOption = base.CGet<Refers>("CommonOption");
			CToggleGroupObsolete toggleGroup = commonOption.CGet<CToggleGroupObsolete>("CommonOptionToggleGroup");
			toggleGroup.InitPreOnToggle(0);
			toggleGroup.OnActiveToggleChange = null;
			toggleGroup.Set((int)this.Data.ExtraData.ShowCommonOptionIndex, true, false);
			toggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnActiveToggleChange);
			short i = 0;
			while ((int)i < EventCommonOption.Instance.Count)
			{
				EventCommonOptionItem config = EventCommonOption.Instance[i];
				bool flag = i != 6;
				if (flag)
				{
					CToggleObsolete toggle = toggleGroup.Get((int)i);
					toggle.gameObject.SetActive(this.Model.IsEventCommonOptionUnlocked(config));
					TextMeshProUGUI[] labels = toggle.GetComponentsInChildren<TextMeshProUGUI>(true);
				}
				else
				{
					CButtonObsolete finishBtn = commonOption.CGet<CButtonObsolete>("CommonButtonFinish");
					TextMeshProUGUI[] labels = finishBtn.GetComponentsInChildren<TextMeshProUGUI>(true);
					short templateId = i;
					finishBtn.ClearAndAddListener(delegate
					{
						TaiwuEventDomainMethod.Call.EventCommonOptionSelect(templateId);
					});
				}
				i += 1;
			}
		}
	}

	// Token: 0x06003497 RID: 13463 RVA: 0x001A3D6E File Offset: 0x001A1F6E
	private void OnActiveToggleChange(CToggleObsolete newToggle, CToggleObsolete preToggle)
	{
		TaiwuEventDomainMethod.Call.EventCommonOptionSelect((short)newToggle.Key);
	}

	// Token: 0x0600349B RID: 13467 RVA: 0x001A3F21 File Offset: 0x001A2121
	[CompilerGenerated]
	private void <OnClick>g__SelectCharacterAction|50_1()
	{
		SingletonObject.getInstance<EventModel>().SetSelectCharacterResult(this._selectedCharIdList);
		this.SetSelecting(false);
		this._selectedCharIdList.Clear();
	}

	// Token: 0x0600349C RID: 13468 RVA: 0x001A3F4C File Offset: 0x001A214C
	[CompilerGenerated]
	private bool <OnClick>g__SelectedCharacterHasDukeTitle|50_2()
	{
		for (int i = 0; i < this._selectedCharIdList.Count; i++)
		{
			int charId = this._selectedCharIdList[i];
			bool flag = this.IsCharacterHaveDukeTitle(charId);
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600349F RID: 13471 RVA: 0x001A3FC0 File Offset: 0x001A21C0
	[CompilerGenerated]
	private bool <OnAutoChooseMessage>g__CanSelectOption|59_0(string toCheckOptionKey, ref UI_EventWindow.<>c__DisplayClass59_0 A_2)
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

	// Token: 0x060034AB RID: 13483 RVA: 0x001A4304 File Offset: 0x001A2504
	[CompilerGenerated]
	internal static void <UpdateFameActionSort>g__SetCheckMark|163_2(Refers refers, sbyte index)
	{
		refers.CGet<CImage>("CheckMark").SetAlpha((float)((index > 0) ? 1 : 0));
		RectTransform arrow = refers.CGet<RectTransform>("Arrow");
		arrow.gameObject.SetActive(index > 0);
		arrow.localRotation = SortFilter.GetArrowRotation(index < 2);
		arrow.anchoredPosition = SortFilter.GetArrowAnchoredPos(index < 2);
	}

	// Token: 0x040025FE RID: 9726
	[SerializeField]
	private EventWindowSelectFuyuFaith fuyuFaith;

	// Token: 0x040025FF RID: 9727
	private static List<UIElement> ConflictElements = new List<UIElement>
	{
		UIElement.AdventurePrepareRemake
	};

	// Token: 0x04002600 RID: 9728
	private const string OptionItem = "UI_EventWindow_OptionItem";

	// Token: 0x04002601 RID: 9729
	private PoolItem _optionPool;

	// Token: 0x04002602 RID: 9730
	public float WindowAnimDuration;

	// Token: 0x04002603 RID: 9731
	public float CgShowTime = 3f;

	// Token: 0x04002604 RID: 9732
	public string TextureDirectory;

	// Token: 0x04002605 RID: 9733
	public sbyte LayoutDirtyDelayFrame = 2;

	// Token: 0x04002606 RID: 9734
	private bool _layoutDirty;

	// Token: 0x04002607 RID: 9735
	private sbyte _layoutDirtyCount;

	// Token: 0x04002608 RID: 9736
	private bool _waitSelect;

	// Token: 0x04002609 RID: 9737
	private bool _needSelectItem;

	// Token: 0x0400260A RID: 9738
	private bool _needSelectApprovedTaiwu;

	// Token: 0x0400260B RID: 9739
	private bool _animating;

	// Token: 0x0400260C RID: 9740
	private bool _posInViewPort;

	// Token: 0x0400260D RID: 9741
	private bool _isDisplayingLog;

	// Token: 0x0400260E RID: 9742
	private DetailInfoMonitor _taiwuDetailInfoMonitor;

	// Token: 0x0400260F RID: 9743
	private bool _maskAnimHandled;

	// Token: 0x04002610 RID: 9744
	private sbyte _maskControlCode;

	// Token: 0x04002611 RID: 9745
	private float _maskAnimTweenTime;

	// Token: 0x04002612 RID: 9746
	private const float _longPressDuration = 0.6f;

	// Token: 0x04002613 RID: 9747
	private float _currentPressDuration;

	// Token: 0x04002614 RID: 9748
	private Tweener _adjustLayoutTween;

	// Token: 0x04002615 RID: 9749
	private EventModel _model;

	// Token: 0x04002616 RID: 9750
	private Action _animationWindowCompleteAction;

	// Token: 0x04002617 RID: 9751
	private Action _animationMaskCompleteAction;

	// Token: 0x04002618 RID: 9752
	private bool _animationMaskComplete = true;

	// Token: 0x04002619 RID: 9753
	private Action _animationCgTextureCompleteAction;

	// Token: 0x0400261A RID: 9754
	public static readonly List<LanguageKey> BehaviorNameKeyList = new List<LanguageKey>
	{
		LanguageKey.LK_Goodness_0,
		LanguageKey.LK_Goodness_1,
		LanguageKey.LK_Goodness_2,
		LanguageKey.LK_Goodness_3,
		LanguageKey.LK_Goodness_4
	};

	// Token: 0x0400261B RID: 9755
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

	// Token: 0x0400261C RID: 9756
	private Vector2 _itemScrollViewInitPos;

	// Token: 0x0400261D RID: 9757
	private Vector2 _itemScrollViewInitDeltaSize;

	// Token: 0x0400261E RID: 9758
	private Coroutine _sensitiveWordTipCoroutine;

	// Token: 0x0400261F RID: 9759
	private Tween _sensitiveWordTipTween;

	// Token: 0x04002620 RID: 9760
	private int _haveSpiritualDebt;

	// Token: 0x04002621 RID: 9761
	private int _haveMoney;

	// Token: 0x04002622 RID: 9762
	private Dictionary<ItemKey, bool> _fixItemAttainmentMeetDict = new Dictionary<ItemKey, bool>();

	// Token: 0x04002623 RID: 9763
	private readonly List<ItemKey> _selectedItemList = new List<ItemKey>();

	// Token: 0x04002624 RID: 9764
	private readonly Dictionary<ItemKey, int> _selectedItemDict = new Dictionary<ItemKey, int>();

	// Token: 0x04002625 RID: 9765
	private bool _isMultiItemSelect;

	// Token: 0x04002626 RID: 9766
	private bool _isExchangeTools;

	// Token: 0x04002627 RID: 9767
	private bool _isFixItem;

	// Token: 0x04002628 RID: 9768
	private MultiplyItemScrollView _multiplyItemScrollView;

	// Token: 0x04002629 RID: 9769
	private int _curCount;

	// Token: 0x0400262A RID: 9770
	private int _minCount = 1;

	// Token: 0x0400262B RID: 9771
	private int _maxCount;

	// Token: 0x0400262C RID: 9772
	private int _changeValue = 1;

	// Token: 0x0400262D RID: 9773
	private CSliderLegacy _countSlider;

	// Token: 0x0400262E RID: 9774
	private TMP_InputField _inputCountField;

	// Token: 0x0400262F RID: 9775
	private CButtonObsolete _buttonMore;

	// Token: 0x04002630 RID: 9776
	private CButtonObsolete _buttonLess;

	// Token: 0x04002631 RID: 9777
	private sbyte _fameActionSortIndex = 0;

	// Token: 0x04002632 RID: 9778
	private sbyte _fameExistTimeSortIndex = 0;

	// Token: 0x04002633 RID: 9779
	private Refers _fameActionButton;

	// Token: 0x04002634 RID: 9780
	private Refers _existTimeButton;

	// Token: 0x04002635 RID: 9781
	private readonly List<short> _selectedFameActionIdList = new List<short>();

	// Token: 0x04002636 RID: 9782
	private readonly List<short> _canSelectedFameActionIdList = new List<short>();

	// Token: 0x04002637 RID: 9783
	private readonly Dictionary<short, int> _fameActionIdToIndex = new Dictionary<short, int>();

	// Token: 0x04002638 RID: 9784
	private List<int> _selectedCharIdList = new List<int>();

	// Token: 0x04002639 RID: 9785
	private List<int> _canSelectCharIdList = new List<int>();

	// Token: 0x0400263A RID: 9786
	private List<CharacterDisplayData> _canSelectCharacterDisplayDataList = new List<CharacterDisplayData>();

	// Token: 0x0400263B RID: 9787
	private bool _isEnough = false;

	// Token: 0x0400263C RID: 9788
	private int _selectedAvatarIndex;

	// Token: 0x0400263D RID: 9789
	private bool _isCommonOptionInit;
}
