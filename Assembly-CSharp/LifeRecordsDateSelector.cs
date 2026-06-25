using System;
using System.Collections.Generic;
using System.Linq;
using CommonSortAndFilterLegacy;
using Config;
using GameData.Domains.World;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001D4 RID: 468
public class LifeRecordsDateSelector : Refers
{
	// Token: 0x1700030A RID: 778
	// (get) Token: 0x06001E50 RID: 7760 RVA: 0x000DA40D File Offset: 0x000D860D
	public LifeRecordsController.LifeRecordMonthData SelectedMonthData
	{
		get
		{
			return this._selectedMonthData;
		}
	}

	// Token: 0x06001E51 RID: 7761 RVA: 0x000DA418 File Offset: 0x000D8618
	public void Init(Action<int> onSelect)
	{
		this._menuBar = base.CGet<FilterMenuBar>("MenuBar");
		this._monthPanel = base.CGet<GameObject>("MonthPanel");
		this._slider = base.CGet<CSliderLegacy>("Slider");
		this._buttonLayout = base.CGet<RectTransform>("ButtonLayout");
		this._buttonMin = base.CGet<CButtonObsolete>("ButtonMin");
		this._buttonLess = base.CGet<CButtonObsolete>("ButtonLess");
		this._buttonMore = base.CGet<CButtonObsolete>("ButtonMore");
		this._buttonMax = base.CGet<CButtonObsolete>("ButtonMax");
		this._monthButtonList.Clear();
		this._monthButtonList.AddRange(this._buttonLayout.GetComponentsInChildren<CButtonObsolete>(true));
		this._yearsRecordList.Clear();
		this._selectedYearData = null;
		this._selectedMonthData = null;
		this._onSelect = onSelect;
		this._menuBar.SetupSwapToggle(delegate(bool isOn)
		{
			this._monthPanel.SetActive(isOn);
			this._menuBar.SetSelected(isOn);
			this._menuBar.SetStatusIcon(isOn);
		});
		this.HidePanel();
		this._buttonMin.ClearAndAddListener(new Action(this.OnClickButtonMin));
		this._buttonLess.ClearAndAddListener(new Action(this.OnClickButtonLess));
		this._buttonMore.ClearAndAddListener(new Action(this.OnClickButtonMore));
		this._buttonMax.ClearAndAddListener(new Action(this.OnClickButtonMax));
		this._lastClickTimeMin = 0f;
		this._lastClickTimeLess = 0f;
		this._lastClickTimeMore = 0f;
		this._lastClickTimeMax = 0f;
		this._slider.onValueChanged.RemoveAllListeners();
		this._slider.onValueChanged.AddListener(delegate(float value)
		{
			LifeRecordsController.LifeRecordYearData curYearData = this._yearsRecordList.GetOrDefault((int)value);
			bool flag = curYearData == this._selectedYearData && curYearData != null;
			if (!flag)
			{
				this.OnSelectedYearChange(curYearData, false);
			}
		});
	}

	// Token: 0x06001E52 RID: 7762 RVA: 0x000DA5CC File Offset: 0x000D87CC
	public void HidePanel()
	{
		FilterMenuBar menuBar = this._menuBar;
		if (menuBar != null)
		{
			menuBar.SetSwapToggle(false, true);
		}
	}

	// Token: 0x06001E53 RID: 7763 RVA: 0x000DA5E4 File Offset: 0x000D87E4
	private void RefreshTitle(int date)
	{
		int year = date / 12;
		int month = date % 12;
		string title = LocalStringManager.GetFormat(LanguageKey.LK_Game_Time, new object[]
		{
			year + 1,
			month + 1,
			LocalStringManager.Get(string.Format("LK_Season_{0}", TimeKit.GetSeason(date))),
			Month.Instance[month].Name
		});
		this._menuBar.SetLabelText(title);
	}

	// Token: 0x06001E54 RID: 7764 RVA: 0x000DA660 File Offset: 0x000D8860
	public void Refresh(List<LifeRecordsController.LifeRecordYearData> yearsRecordList, bool init)
	{
		this._yearsRecordList.Clear();
		foreach (LifeRecordsController.LifeRecordYearData yearData in yearsRecordList)
		{
			bool flag = yearData.MonthRecordDataList.Any(delegate(LifeRecordsController.LifeRecordMonthData m)
			{
				List<ValueTuple<int, string, string, short>> recordList = m.RecordList;
				return recordList != null && recordList.Count > 0;
			});
			if (flag)
			{
				this._yearsRecordList.Add(yearData);
			}
		}
		bool flag2 = this._yearsRecordList.Count > 0;
		if (flag2)
		{
			this._slider.interactable = true;
			this.RefreshSlider(init, true);
		}
		else
		{
			this._menuBar.SetLabelText(string.Empty);
			this._slider.interactable = false;
			this._selectedYearData = null;
			this._selectedMonthData = null;
			this.RefreshMonthButtons();
			this.RefreshJumpButton();
		}
	}

	// Token: 0x06001E55 RID: 7765 RVA: 0x000DA75C File Offset: 0x000D895C
	private void RefreshSlider(bool init, bool reset)
	{
		this._slider.wholeNumbers = true;
		this._slider.minValue = 0f;
		this._slider.maxValue = (float)(this._yearsRecordList.Count - 1);
		this._slider.SetValueWithoutNotify(this._slider.maxValue);
		bool flag = reset || init;
		if (flag)
		{
			LifeRecordsController.LifeRecordYearData curYearData = this._yearsRecordList.GetOrDefault((int)this._slider.maxValue);
			this.OnSelectedYearChange(curYearData, init);
		}
	}

	// Token: 0x06001E56 RID: 7766 RVA: 0x000DA7E4 File Offset: 0x000D89E4
	private void OnSelectedYearChange(LifeRecordsController.LifeRecordYearData yearData, bool init)
	{
		this._selectedYearData = yearData;
		LifeRecordsController.LifeRecordMonthData selectedMonthData;
		if (!init)
		{
			LifeRecordsController.LifeRecordYearData selectedYearData = this._selectedYearData;
			if (selectedYearData == null)
			{
				selectedMonthData = null;
			}
			else
			{
				selectedMonthData = selectedYearData.MonthRecordDataList.First(delegate(LifeRecordsController.LifeRecordMonthData m)
				{
					List<ValueTuple<int, string, string, short>> recordList = m.RecordList;
					return recordList != null && recordList.Count > 0;
				});
			}
		}
		else
		{
			LifeRecordsController.LifeRecordYearData selectedYearData2 = this._selectedYearData;
			if (selectedYearData2 == null)
			{
				selectedMonthData = null;
			}
			else
			{
				selectedMonthData = selectedYearData2.MonthRecordDataList.Last(delegate(LifeRecordsController.LifeRecordMonthData m)
				{
					List<ValueTuple<int, string, string, short>> recordList = m.RecordList;
					return recordList != null && recordList.Count > 0;
				});
			}
		}
		this._selectedMonthData = selectedMonthData;
		LifeRecordsController.LifeRecordMonthData selectedMonthData2 = this._selectedMonthData;
		int data = (selectedMonthData2 != null) ? selectedMonthData2.Date : SingletonObject.getInstance<BasicGameData>().CurrDate;
		this.RefreshTitle(data);
		this.RefreshMonthButtons();
		this.RefreshJumpButton();
		this._onSelect(data);
	}

	// Token: 0x06001E57 RID: 7767 RVA: 0x000DA8B0 File Offset: 0x000D8AB0
	private void RefreshMonthButtons()
	{
		LifeRecordsDateSelector.<>c__DisplayClass25_0 CS$<>8__locals1 = new LifeRecordsDateSelector.<>c__DisplayClass25_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.i = 0;
		while (CS$<>8__locals1.i < this._monthButtonList.Count)
		{
			LifeRecordsDateSelector.<>c__DisplayClass25_1 CS$<>8__locals2 = new LifeRecordsDateSelector.<>c__DisplayClass25_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			LifeRecordsDateSelector.<>c__DisplayClass25_1 CS$<>8__locals3 = CS$<>8__locals2;
			LifeRecordsController.LifeRecordYearData selectedYearData = this._selectedYearData;
			LifeRecordsController.LifeRecordMonthData monthData;
			if (selectedYearData == null)
			{
				monthData = null;
			}
			else
			{
				List<LifeRecordsController.LifeRecordMonthData> monthRecordDataList = selectedYearData.MonthRecordDataList;
				monthData = ((monthRecordDataList != null) ? monthRecordDataList.Find((LifeRecordsController.LifeRecordMonthData m) => m.Month == CS$<>8__locals2.CS$<>8__locals1.i) : null);
			}
			CS$<>8__locals3.monthData = monthData;
			LifeRecordsController.LifeRecordMonthData monthData2 = CS$<>8__locals2.monthData;
			bool flag;
			int num2;
			if (monthData2 == null)
			{
				flag = false;
			}
			else
			{
				List<ValueTuple<int, string, string, short>> recordList = monthData2.RecordList;
				int? num = (recordList != null) ? new int?(recordList.Count) : null;
				num2 = 0;
				flag = (num.GetValueOrDefault() > num2 & num != null);
			}
			bool valid = flag;
			bool isSelected = valid && CS$<>8__locals2.monthData == this._selectedMonthData;
			CButtonObsolete button = this._monthButtonList[CS$<>8__locals2.CS$<>8__locals1.i];
			Transform transform = button.transform.Find("Selected");
			if (transform != null)
			{
				transform.gameObject.SetActive(isSelected);
			}
			button.interactable = valid;
			button.GetComponent<HSVStyleRoot>().SetInteractable(button.interactable);
			TextMeshProUGUI[] texts = button.GetComponentsInChildren<TextMeshProUGUI>(true);
			foreach (TextMeshProUGUI text in texts)
			{
				text.text = string.Format("{0}{1}", CS$<>8__locals2.CS$<>8__locals1.i + 1, LanguageKey.LK_Month.Tr());
			}
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(delegate()
			{
				CS$<>8__locals2.CS$<>8__locals1.<>4__this.Select(CS$<>8__locals2.monthData, true);
			});
			num2 = CS$<>8__locals1.i;
			CS$<>8__locals1.i = num2 + 1;
		}
	}

	// Token: 0x06001E58 RID: 7768 RVA: 0x000DAA78 File Offset: 0x000D8C78
	public void Select(LifeRecordsController.LifeRecordMonthData monthData, bool callBack)
	{
		this._selectedYearData = this._yearsRecordList.Find(delegate(LifeRecordsController.LifeRecordYearData y)
		{
			List<LifeRecordsController.LifeRecordMonthData> monthRecordDataList = y.MonthRecordDataList;
			return monthRecordDataList != null && monthRecordDataList.Contains(monthData);
		});
		this._selectedMonthData = monthData;
		this.RefreshTitle(this._selectedMonthData.Date);
		this.RefreshMonthButtons();
		this.RefreshJumpButton();
		this.HidePanel();
		if (callBack)
		{
			this._onSelect(this._selectedMonthData.Date);
		}
	}

	// Token: 0x06001E59 RID: 7769 RVA: 0x000DAB04 File Offset: 0x000D8D04
	private void Jump(int offset)
	{
		int curIndex = this._yearsRecordList.IndexOf(this._selectedYearData);
		int index = Mathf.Clamp(curIndex + offset, 0, this._yearsRecordList.Count - 1);
		bool flag = Mathf.Approximately((float)index, this._slider.value);
		if (flag)
		{
			Slider.SliderEvent onValueChanged = this._slider.onValueChanged;
			if (onValueChanged != null)
			{
				onValueChanged.Invoke((float)index);
			}
		}
		else
		{
			this._slider.value = (float)index;
		}
		this.RefreshJumpButton();
	}

	// Token: 0x06001E5A RID: 7770 RVA: 0x000DAB88 File Offset: 0x000D8D88
	private void OnClickButtonMin()
	{
		bool flag = Time.time - this._lastClickTimeMin < 0.1f;
		if (!flag)
		{
			this._lastClickTimeMin = Time.time;
			this.Jump(-10);
		}
	}

	// Token: 0x06001E5B RID: 7771 RVA: 0x000DABC4 File Offset: 0x000D8DC4
	private void OnClickButtonLess()
	{
		bool flag = Time.time - this._lastClickTimeLess < 0.1f;
		if (!flag)
		{
			this._lastClickTimeLess = Time.time;
			this.Jump(-1);
		}
	}

	// Token: 0x06001E5C RID: 7772 RVA: 0x000DAC00 File Offset: 0x000D8E00
	private void OnClickButtonMore()
	{
		bool flag = Time.time - this._lastClickTimeMore < 0.1f;
		if (!flag)
		{
			this._lastClickTimeMore = Time.time;
			this.Jump(1);
		}
	}

	// Token: 0x06001E5D RID: 7773 RVA: 0x000DAC3C File Offset: 0x000D8E3C
	private void OnClickButtonMax()
	{
		bool flag = Time.time - this._lastClickTimeMax < 0.1f;
		if (!flag)
		{
			this._lastClickTimeMax = Time.time;
			this.Jump(10);
		}
	}

	// Token: 0x06001E5E RID: 7774 RVA: 0x000DAC78 File Offset: 0x000D8E78
	private void RefreshJumpButton()
	{
		int index = (this._yearsRecordList.Count > 0) ? this._yearsRecordList.IndexOf(this._selectedYearData) : -1;
		this._buttonMin.interactable = (this._buttonLess.interactable = (index > 0));
		this._buttonMax.interactable = (this._buttonMore.interactable = (index < this._yearsRecordList.Count - 1));
	}

	// Token: 0x06001E5F RID: 7775 RVA: 0x000DACF4 File Offset: 0x000D8EF4
	public void Refresh(LifeRecordsDateSelector other)
	{
		this._yearsRecordList.Clear();
		this._yearsRecordList.AddRange(other._yearsRecordList);
		this._selectedYearData = other._selectedYearData;
		this._selectedMonthData = other._selectedMonthData;
		this.RefreshSlider(false, false);
		this._slider.SetValueWithoutNotify(other._slider.value);
		this.RefreshTitle(this._selectedMonthData.Date);
		this.RefreshMonthButtons();
		this.RefreshJumpButton();
	}

	// Token: 0x04001700 RID: 5888
	private FilterMenuBar _menuBar;

	// Token: 0x04001701 RID: 5889
	private GameObject _monthPanel;

	// Token: 0x04001702 RID: 5890
	private CSliderLegacy _slider;

	// Token: 0x04001703 RID: 5891
	private RectTransform _buttonLayout;

	// Token: 0x04001704 RID: 5892
	private CButtonObsolete _buttonMin;

	// Token: 0x04001705 RID: 5893
	private CButtonObsolete _buttonLess;

	// Token: 0x04001706 RID: 5894
	private CButtonObsolete _buttonMore;

	// Token: 0x04001707 RID: 5895
	private CButtonObsolete _buttonMax;

	// Token: 0x04001708 RID: 5896
	private float _lastClickTimeMin;

	// Token: 0x04001709 RID: 5897
	private float _lastClickTimeLess;

	// Token: 0x0400170A RID: 5898
	private float _lastClickTimeMore;

	// Token: 0x0400170B RID: 5899
	private float _lastClickTimeMax;

	// Token: 0x0400170C RID: 5900
	private readonly List<CButtonObsolete> _monthButtonList = new List<CButtonObsolete>();

	// Token: 0x0400170D RID: 5901
	private readonly List<LifeRecordsController.LifeRecordYearData> _yearsRecordList = new List<LifeRecordsController.LifeRecordYearData>();

	// Token: 0x0400170E RID: 5902
	private LifeRecordsController.LifeRecordYearData _selectedYearData;

	// Token: 0x0400170F RID: 5903
	private LifeRecordsController.LifeRecordMonthData _selectedMonthData;

	// Token: 0x04001710 RID: 5904
	private Action<int> _onSelect;
}
