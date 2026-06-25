using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using TMPro;
using UICommon.Character.Elements;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003A7 RID: 935
public class UI_SelectThreeVitalsTarget : UIBase
{
	// Token: 0x170005C3 RID: 1475
	// (get) Token: 0x06003836 RID: 14390 RVA: 0x001C5767 File Offset: 0x001C3967
	private InfinityScrollLegacy CharacterScroll
	{
		get
		{
			return base.CGet<InfinityScrollLegacy>("CharacterScroll");
		}
	}

	// Token: 0x170005C4 RID: 1476
	// (get) Token: 0x06003837 RID: 14391 RVA: 0x001C5774 File Offset: 0x001C3974
	private InfectSortAndFilter InfectSortAndFilter
	{
		get
		{
			return base.CGet<InfectSortAndFilter>("InfectSortAndFilter");
		}
	}

	// Token: 0x170005C5 RID: 1477
	// (get) Token: 0x06003838 RID: 14392 RVA: 0x001C5781 File Offset: 0x001C3981
	private TMP_InputField Search
	{
		get
		{
			return base.CGet<TMP_InputField>("Search");
		}
	}

	// Token: 0x06003839 RID: 14393 RVA: 0x001C5790 File Offset: 0x001C3990
	public override void OnInit(ArgumentBox argsBox)
	{
		this._selectedTargetCharData = null;
		this._tempCharDataList.Clear();
		this._targetTransferInfectionDict.Clear();
		this.RefreshConfirmButton();
		argsBox.Get("isGoodEnd", out this._isGoodEnd);
		argsBox.Get<List<CharacterDisplayDataForInfect>>("targetCharDataList", out this._targetCharDataList);
		argsBox.Get<SectStoryThreeVitalsCharacter>("vitalData", out this._vitalData);
		argsBox.Get<CharacterDisplayData>("vitalCharData", out this._vitalCharData);
		argsBox.Get<Action<Dictionary<int, int>>>("onConfirm", out this._onConfirm);
		foreach (CharacterDisplayDataForInfect data in this._targetCharDataList)
		{
			data.TempInfection = (int)data.Infection;
		}
		this.Search.SetTextWithoutNotify(string.Empty);
		this.Search.onValueChanged.RemoveAllListeners();
		this.Search.onValueChanged.AddListener(new UnityAction<string>(this.OnSearch));
		CToggleGroupObsolete toggleGroup = this.CharacterScroll.GetComponent<CToggleGroupObsolete>();
		toggleGroup.Clear();
		toggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnActiveToggleChange);
		this.CharacterScroll.SetTogGroup(toggleGroup, false, false);
		this.CharacterScroll.OnItemRender = new Action<int, Refers>(this.OnItemRender);
		this.InfectSortAndFilter.Init();
		this.InfectSortAndFilter.SetList(ref this._targetCharDataList, this._isGoodEnd, true, "UI_SelectThreeVitalsTarget", delegate
		{
			this.OnSearch(this.Search.text);
		});
		this.RefreshVitalInfo();
		this.RefreshVitalInfection();
		this.SetMouseTip();
	}

	// Token: 0x0600383A RID: 14394 RVA: 0x001C5940 File Offset: 0x001C3B40
	private void OnDisable()
	{
		this.InfectSortAndFilter.SaveSortFilterSetting();
	}

	// Token: 0x0600383B RID: 14395 RVA: 0x001C5950 File Offset: 0x001C3B50
	private void SetMouseTip()
	{
		TooltipInvoker confirmMouseTip = base.CGet<TooltipInvoker>("ConfirmMouseTip");
		TooltipInvoker tooltipInvoker = confirmMouseTip;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		confirmMouseTip.RuntimeParam.Set("arg0", LocalStringManager.Get(this._isGoodEnd ? LanguageKey.LK_ThreeVitals_Cannot_Transfer_Good : LanguageKey.LK_ThreeVitals_Cannot_Transfer_Bad));
		TooltipInvoker buttonMoreMouseTip = base.CGet<TooltipInvoker>("ButtonMoreMouseTip");
		tooltipInvoker = buttonMoreMouseTip;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		buttonMoreMouseTip.RuntimeParam.Set("arg0", LocalStringManager.Get(this._isGoodEnd ? LanguageKey.LK_ThreeVitals_Not_Transfer_Good : LanguageKey.LK_ThreeVitals_Not_Transfer_Bad));
	}

	// Token: 0x0600383C RID: 14396 RVA: 0x001C59FC File Offset: 0x001C3BFC
	private void OnActiveToggleChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		bool flag = !newTog;
		if (!flag)
		{
			CharacterDisplayDataForInfect charData = this._tempCharDataList[newTog.Key];
			this._selectedTargetCharData = charData;
			Refers setSelectCount = base.CGet<Refers>("SetSelectCount");
			int tempVitalInfection = this.GetTempVitalInfection();
			bool flag2;
			bool flag3;
			int tempInfection = this.GetTempCharInfection(charData, out flag2, out flag3);
			int minValue = 0;
			int maxValue = this._isGoodEnd ? Math.Min(tempVitalInfection, 200 - tempInfection) : Math.Min(GlobalConfig.Instance.ThreeVitalsMaxInfection - tempVitalInfection, tempInfection);
			int transferValue;
			this._targetTransferInfectionDict.TryGetValue(charData.DataForTooltip.Id, out transferValue);
			maxValue = Mathf.Max(maxValue, transferValue);
			setSelectCount.CGet<TextMeshProUGUI>("Max").text = string.Format("/{0}", maxValue);
			LanguageKey titleKey = this._isGoodEnd ? LanguageKey.LK_ThreeVitals_Transfer_Good : LanguageKey.LK_ThreeVitals_Transfer_Bad;
			setSelectCount.CGet<TextMeshProUGUI>("Title").text = LocalStringManager.Get(titleKey);
			CSliderLegacy slider = setSelectCount.CGet<CSliderLegacy>("Slider");
			TMP_InputField inputField = setSelectCount.CGet<TMP_InputField>("InputField");
			inputField.SetTextWithoutNotify(transferValue.ToString());
			inputField.onValueChanged.RemoveAllListeners();
			inputField.onValueChanged.AddListener(delegate(string value)
			{
				int delta;
				int.TryParse(value, out delta);
				slider.value = (float)delta;
				inputField.SetTextWithoutNotify(slider.value.ToString());
				bool flag4 = (int)slider.value == (int)slider.minValue;
				if (flag4)
				{
					inputField.MoveTextEnd(false);
				}
			});
			slider.onValueChanged.RemoveAllListeners();
			slider.onValueChanged.AddListener(delegate(float value)
			{
				int delta = (int)value;
				this._targetTransferInfectionDict[charData.DataForTooltip.Id] = delta;
				charData.TempInfection = this.CalcTempCharInfection((int)charData.Infection, delta);
				inputField.SetTextWithoutNotify(delta.ToString());
				this.RefreshVitalInfection();
				this.CharacterScroll.RefreshCell(newTog.Key);
				this.UpdateSliderButtonInteractable();
				this.RefreshConfirmButton();
			});
			slider.minValue = (float)minValue;
			slider.maxValue = (float)maxValue;
			slider.value = (float)transferValue;
			this.UpdateSliderButtonInteractable();
			CButtonObsolete buttonMore = setSelectCount.CGet<CButtonObsolete>("ButtonMore");
			buttonMore.ClearAndAddListener(delegate
			{
				CSliderLegacy slider = slider;
				float value = slider.value;
				slider.value = value + 1f;
			});
			CButtonObsolete buttonLess = setSelectCount.CGet<CButtonObsolete>("ButtonLess");
			buttonLess.ClearAndAddListener(delegate
			{
				CSliderLegacy slider = slider;
				float value = slider.value;
				slider.value = value - 1f;
			});
		}
	}

	// Token: 0x0600383D RID: 14397 RVA: 0x001C5C34 File Offset: 0x001C3E34
	private void UpdateSliderButtonInteractable()
	{
		Refers setSelectCount = base.CGet<Refers>("SetSelectCount");
		CSliderLegacy slider = setSelectCount.CGet<CSliderLegacy>("Slider");
		CButtonObsolete buttonMore = setSelectCount.CGet<CButtonObsolete>("ButtonMore");
		buttonMore.interactable = (slider.value < slider.maxValue);
		TooltipInvoker buttonMoreTip = buttonMore.GetComponent<TooltipInvoker>();
		buttonMoreTip.enabled = !buttonMore.interactable;
		CButtonObsolete buttonLess = setSelectCount.CGet<CButtonObsolete>("ButtonLess");
		buttonLess.interactable = (slider.value > 0f);
	}

	// Token: 0x0600383E RID: 14398 RVA: 0x001C5CB4 File Offset: 0x001C3EB4
	private void RefreshVitalInfo()
	{
		SelectableCharacter vital = base.CGet<SelectableCharacter>("Vital");
		vital.SetData(this._vitalCharData);
		vital.GetComponent<CImage>().SetSprite(this._isGoodEnd ? "sp_03_mh_touxiang_3" : "sp_03_mh_touxiang_4", false, null);
		CImage fill = base.CGet<CImage>("Fill");
		fill.SetSprite(this._isGoodEnd ? "popup_SelectRemainOfTeacher_progressbarmini_1" : "popup_SelectRemainOfTeacher_progressbarmini_2", false, null);
	}

	// Token: 0x0600383F RID: 14399 RVA: 0x001C5D28 File Offset: 0x001C3F28
	private void RefreshVitalInfection()
	{
		int tempVitalInfection = this.GetTempVitalInfection();
		CImage fill = base.CGet<CImage>("Fill");
		fill.fillAmount = (float)tempVitalInfection / (float)GlobalConfig.Instance.ThreeVitalsMaxInfection;
		int helpPercent = this._isGoodEnd ? GlobalConfig.Instance.ThreeVitalsThresholdHigh : GlobalConfig.Instance.ThreeVitalsThresholdLow;
		float helpRate = (float)helpPercent / 100f;
		float markPos = Mathf.Lerp(this.progressMarkPosMin, this.progressMarkPosMax, helpRate);
		base.CGet<RectTransform>("MarkRoot").anchoredPosition = Vector2.zero.SetX(markPos);
		bool isHelp = UI_ThreeVitals.IsVitalHelping(this._isGoodEnd, tempVitalInfection);
		string color = isHelp ? "brightblue" : "brightred";
		LanguageKey nameKey = isHelp ? LanguageKey.LK_ThreeVitals_Help : LanguageKey.LK_ThreeVitals_Danger;
		base.CGet<TextMeshProUGUI>("InfectionName").text = LocalStringManager.Get(nameKey).SetColor(color);
		base.CGet<TextMeshProUGUI>("InfectionContent").text = string.Format("{0}/{1}", tempVitalInfection.ToString().SetColor(color), GlobalConfig.Instance.ThreeVitalsMaxInfection);
		base.CGet<CImage>("Frame").SetSprite(isHelp ? "popup_SelectRemainOfTeacher_progressbarmini_3" : "popup_SelectRemainOfTeacher_progressbarmini_4", false, null);
	}

	// Token: 0x06003840 RID: 14400 RVA: 0x001C5E64 File Offset: 0x001C4064
	private int GetTempVitalInfection()
	{
		int tempVitalInfection = this._vitalData.Infection;
		foreach (KeyValuePair<int, int> keyValuePair in this._targetTransferInfectionDict)
		{
			int num;
			int num2;
			keyValuePair.Deconstruct(out num, out num2);
			int value = num2;
			bool isGoodEnd = this._isGoodEnd;
			if (isGoodEnd)
			{
				tempVitalInfection -= value;
			}
			else
			{
				tempVitalInfection += value;
			}
		}
		return tempVitalInfection;
	}

	// Token: 0x06003841 RID: 14401 RVA: 0x001C5EF4 File Offset: 0x001C40F4
	private int GetTempCharInfection(CharacterDisplayDataForInfect data, out bool isAdd, out bool isReduce)
	{
		int changeValue;
		this._targetTransferInfectionDict.TryGetValue(data.DataForTooltip.Id, out changeValue);
		int tempInfection = this.CalcTempCharInfection((int)data.Infection, changeValue);
		isAdd = (this._isGoodEnd && changeValue > 0);
		isReduce = (!this._isGoodEnd && changeValue > 0);
		return tempInfection;
	}

	// Token: 0x06003842 RID: 14402 RVA: 0x001C5F50 File Offset: 0x001C4150
	private int CalcTempCharInfection(int infection, int changeValue)
	{
		return this._isGoodEnd ? (infection + changeValue) : (infection - changeValue);
	}

	// Token: 0x06003843 RID: 14403 RVA: 0x001C5F74 File Offset: 0x001C4174
	private void OnSearch(string value)
	{
		this._tempCharDataList.Clear();
		bool flag = value.IsNullOrEmpty();
		if (flag)
		{
			this._tempCharDataList.AddRange(this.InfectSortAndFilter.OutputList);
		}
		else
		{
			foreach (CharacterDisplayDataForInfect data in this.InfectSortAndFilter.OutputList)
			{
				NameRelatedData nameRelatedData = data.DataForTooltip.GetNameRelatedData();
				string charName = NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, data.IsTaiwu, false);
				bool flag2 = charName.Contains(value);
				if (flag2)
				{
					this._tempCharDataList.Add(data);
				}
			}
		}
		this.CharacterScroll.OnRenderEnd = delegate()
		{
			int index = (this._selectedTargetCharData == null) ? 0 : this._tempCharDataList.IndexOf(this._selectedTargetCharData);
			bool flag3 = index < 0;
			if (flag3)
			{
				CToggleObsolete active = this.CharacterScroll.TogGroup.GetActive();
				bool flag4 = active;
				if (flag4)
				{
					this.CharacterScroll.TogGroup.Set(active, false);
				}
			}
			else
			{
				bool flag5 = this._tempCharDataList.Count > 0;
				if (flag5)
				{
					this.CharacterScroll.TogGroup.Set(index, true, false);
				}
			}
			this.CharacterScroll.OnRenderEnd = null;
		};
		this.CharacterScroll.UpdateData(this._tempCharDataList.Count);
	}

	// Token: 0x06003844 RID: 14404 RVA: 0x001C6064 File Offset: 0x001C4264
	private void OnItemRender(int index, Refers refers)
	{
		CharacterDisplayDataForInfect data = this._tempCharDataList[index];
		refers.GetComponent<SelectableCharacter>().SetData(data.DataForTooltip);
		bool add;
		bool reduce;
		int tempInfection = this.GetTempCharInfection(data, out add, out reduce);
		string color = add ? "brightred" : (reduce ? "brightblue" : "pinkyellow");
		refers.CGet<TextMeshProUGUI>("InfectionText").text = tempInfection.ToString().SetColor(color);
		refers.CGet<GameObject>("Kidnapped").SetActive(data.IsKidnapped);
		bool interactable = UI_ThreeVitals.IsTargetInteractable(this._isGoodEnd, (int)data.Infection);
		refers.CGet<CToggleObsolete>("Toggle").interactable = interactable;
		refers.CGet<DisableStyleRoot>("DisableStyleRoot").SetStyleEffect(!interactable, false);
		TooltipInvoker tip = refers.CGet<TooltipInvoker>("Tip");
		tip.enabled = !interactable;
		bool enabled = tip.enabled;
		if (enabled)
		{
			tip.Type = TipType.SingleDesc;
			string[] presetParam = tip.PresetParam;
			bool flag = presetParam == null || presetParam.Length != 1;
			if (flag)
			{
				tip.PresetParam = new string[1];
			}
			LanguageKey tipKey = this._isGoodEnd ? LanguageKey.LK_ThreeVitals_Target_NotMeet_Good : LanguageKey.LK_ThreeVitals_Target_NotMeet_Bad;
			tip.PresetParam[0] = LocalStringManager.Get(tipKey);
		}
	}

	// Token: 0x06003845 RID: 14405 RVA: 0x001C61B0 File Offset: 0x001C43B0
	protected override void OnClick(Transform btn)
	{
		string name = btn.name;
		string a = name;
		if (!(a == "Confirm"))
		{
			if (a == "Cancel")
			{
				this.QuickHide();
			}
		}
		else
		{
			this.OnClickConfirm();
		}
	}

	// Token: 0x06003846 RID: 14406 RVA: 0x001C61F5 File Offset: 0x001C43F5
	private void OnClickConfirm()
	{
		this.QuickHide();
		Action<Dictionary<int, int>> onConfirm = this._onConfirm;
		if (onConfirm != null)
		{
			onConfirm(this._targetTransferInfectionDict);
		}
	}

	// Token: 0x06003847 RID: 14407 RVA: 0x001C6218 File Offset: 0x001C4418
	private void RefreshConfirmButton()
	{
		int change = this._targetTransferInfectionDict.Sum((KeyValuePair<int, int> p) => p.Value);
		CButtonObsolete buttonConfirm = base.CGet<CButtonObsolete>("Confirm");
		buttonConfirm.interactable = (change > 0);
		buttonConfirm.GetComponent<DisableStyleRoot>().SetStyleEffect(!buttonConfirm.interactable, false);
		TooltipInvoker tip = buttonConfirm.GetComponentInChildren<TooltipInvoker>();
		tip.enabled = !buttonConfirm.interactable;
	}

	// Token: 0x040028BF RID: 10431
	[SerializeField]
	private float progressMarkPosMin;

	// Token: 0x040028C0 RID: 10432
	[SerializeField]
	private float progressMarkPosMax;

	// Token: 0x040028C1 RID: 10433
	private List<CharacterDisplayDataForInfect> _targetCharDataList;

	// Token: 0x040028C2 RID: 10434
	private CharacterDisplayDataForInfect _selectedTargetCharData;

	// Token: 0x040028C3 RID: 10435
	private CharacterDisplayData _vitalCharData;

	// Token: 0x040028C4 RID: 10436
	private bool _isGoodEnd;

	// Token: 0x040028C5 RID: 10437
	private SectStoryThreeVitalsCharacter _vitalData;

	// Token: 0x040028C6 RID: 10438
	private Action<Dictionary<int, int>> _onConfirm;

	// Token: 0x040028C7 RID: 10439
	private readonly List<CharacterDisplayDataForInfect> _tempCharDataList = new List<CharacterDisplayDataForInfect>();

	// Token: 0x040028C8 RID: 10440
	private readonly Dictionary<int, int> _targetTransferInfectionDict = new Dictionary<int, int>();
}
