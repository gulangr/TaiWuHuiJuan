using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.ModSystem;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate.Mod;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200025C RID: 604
public class ModEditSettingPanel : MonoBehaviour
{
	// Token: 0x1700045C RID: 1116
	// (get) Token: 0x060027B1 RID: 10161 RVA: 0x001247D9 File Offset: 0x001229D9
	private CDropdown ToggleDropdown
	{
		get
		{
			return this.toggleSetting.toggleDropdown;
		}
	}

	// Token: 0x1700045D RID: 1117
	// (get) Token: 0x060027B2 RID: 10162 RVA: 0x001247E6 File Offset: 0x001229E6
	private TMP_InputField InputField
	{
		get
		{
			return this.inputFieldSetting.inputField;
		}
	}

	// Token: 0x1700045E RID: 1118
	// (get) Token: 0x060027B3 RID: 10163 RVA: 0x001247F3 File Offset: 0x001229F3
	private TMP_InputField SliderMinInputField
	{
		get
		{
			return this.sliderSetting.minInputField;
		}
	}

	// Token: 0x1700045F RID: 1119
	// (get) Token: 0x060027B4 RID: 10164 RVA: 0x00124800 File Offset: 0x00122A00
	private TMP_InputField SliderMaxInputField
	{
		get
		{
			return this.sliderSetting.maxInputField;
		}
	}

	// Token: 0x17000460 RID: 1120
	// (get) Token: 0x060027B5 RID: 10165 RVA: 0x0012480D File Offset: 0x00122A0D
	private TMP_InputField SliderDefaultInputField
	{
		get
		{
			return this.sliderSetting.defaultInputField;
		}
	}

	// Token: 0x17000461 RID: 1121
	// (get) Token: 0x060027B6 RID: 10166 RVA: 0x0012481A File Offset: 0x00122A1A
	private TMP_InputField SliderStepInputField
	{
		get
		{
			return this.sliderSetting.stepInputField;
		}
	}

	// Token: 0x17000462 RID: 1122
	// (get) Token: 0x060027B7 RID: 10167 RVA: 0x00124827 File Offset: 0x00122A27
	private RectTransform OptionLayout
	{
		get
		{
			return this.dropdownSetting.optionLayout;
		}
	}

	// Token: 0x060027B8 RID: 10168 RVA: 0x00124834 File Offset: 0x00122A34
	public void Init()
	{
		this.typeDropdown.onValueChanged.RemoveAllListeners();
		this.typeDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnTypeDropdownValueChanged));
		this.typeDropdown.ClearOptions();
		List<string> typeOptions = ModEditSettingPanel.TypeNameKeyList.Select(new Func<LanguageKey, string>(LocalStringManager.Get)).ToList<string>();
		this.typeDropdown.AddOptions(typeOptions);
		this.typeDropdown.SetValueWithoutNotify(0);
		this.OnTypeDropdownValueChanged(0);
		this.btnYes.ClearAndAddListener(new Action(this.OnClickConfirm));
		this.btnNo.ClearAndAddListener(new Action(this.OnClickCancel));
		this.ToggleDropdown.ClearOptions();
		List<string> switchOptions = ModEditSettingPanel.SwitchKeyNameList.Select(new Func<LanguageKey, string>(LocalStringManager.Get)).ToList<string>();
		this.ToggleDropdown.AddOptions(switchOptions);
		this.ToggleDropdown.value = 0;
		this.SliderMinInputField.onValueChanged.RemoveAllListeners();
		this.SliderMinInputField.onValueChanged.AddListener(new UnityAction<string>(this.OnSliderRangeValueChanged));
		this.SliderMaxInputField.onValueChanged.RemoveAllListeners();
		this.SliderMaxInputField.onValueChanged.AddListener(new UnityAction<string>(this.OnSliderRangeValueChanged));
		this.SliderStepInputField.onValueChanged.RemoveAllListeners();
		this.SliderDefaultInputField.onValueChanged.RemoveAllListeners();
		this.SliderDefaultInputField.onValueChanged.AddListener(new UnityAction<string>(this.OnSliderDefaultValueChanged));
		this.SliderStepInputField.onValueChanged.AddListener(new UnityAction<string>(this.OnSliderStepValueChanged));
		this.dropdownSetting.buttonMore.ClearAndAddListener(new Action(this.OnClickButtonMore));
	}

	// Token: 0x060027B9 RID: 10169 RVA: 0x00124A01 File Offset: 0x00122C01
	private void OnClickButtonMore()
	{
		this._dropdownOptionList.Add(null);
		this.RefreshDropdownItemList();
	}

	// Token: 0x060027BA RID: 10170 RVA: 0x00124A18 File Offset: 0x00122C18
	private void RefreshDropdownItemList()
	{
		for (int i = 0; i < this._dropdownOptionList.Count; i++)
		{
			int index = i;
			ModEditDropdownSettingOption item = (i > this.OptionLayout.childCount - 1) ? Object.Instantiate<ModEditDropdownSettingOption>(this.OptionLayout.GetChild(0).GetComponent<ModEditDropdownSettingOption>(), this.OptionLayout) : this.OptionLayout.GetChild(i).GetComponent<ModEditDropdownSettingOption>();
			bool flag = !item.gameObject.activeSelf;
			if (flag)
			{
				item.gameObject.SetActive(true);
			}
			CButton buttonLess = item.buttonLess;
			buttonLess.gameObject.SetActive(index > 0);
			buttonLess.ClearAndAddListener(delegate
			{
				this._dropdownOptionList.RemoveAt(index);
				this.RefreshDropdownItemList();
			});
			TMP_InputField inputField = item.contentInputField;
			inputField.onValueChanged.RemoveAllListeners();
			inputField.onValueChanged.AddListener(delegate(string value)
			{
				this._dropdownOptionList[index] = value;
			});
			inputField.SetTextWithoutNotify(this._dropdownOptionList[index] ?? string.Empty);
		}
		for (int j = this._dropdownOptionList.Count; j < this.OptionLayout.childCount; j++)
		{
			this.OptionLayout.GetChild(j).gameObject.SetActive(false);
		}
	}

	// Token: 0x060027BB RID: 10171 RVA: 0x00124B81 File Offset: 0x00122D81
	private void ShowMakeDropdownMask(bool show)
	{
	}

	// Token: 0x060027BC RID: 10172 RVA: 0x00124B84 File Offset: 0x00122D84
	private void OnGUI()
	{
		ModDropdownUtils.HandleDropdown(this.typeDropdown, null);
		ModDropdownUtils.HandleDropdown(this.ToggleDropdown, null);
	}

	// Token: 0x060027BD RID: 10173 RVA: 0x00124BA4 File Offset: 0x00122DA4
	private void OnSliderStepValueChanged(string value)
	{
		int result = this.GetSliderClampedValue(value);
		this.SliderStepInputField.SetTextWithoutNotify(result.ToString());
	}

	// Token: 0x060027BE RID: 10174 RVA: 0x00124BD0 File Offset: 0x00122DD0
	private void OnSliderDefaultValueChanged(string value)
	{
		int result = this.GetSliderClampedValue(value);
		this.SliderDefaultInputField.SetTextWithoutNotify(result.ToString());
	}

	// Token: 0x060027BF RID: 10175 RVA: 0x00124BFC File Offset: 0x00122DFC
	private void OnSliderRangeValueChanged(string _)
	{
		TMP_InputField.OnChangeEvent onValueChanged = this.SliderDefaultInputField.onValueChanged;
		if (onValueChanged != null)
		{
			onValueChanged.Invoke(this.SliderDefaultInputField.text);
		}
		TMP_InputField.OnChangeEvent onValueChanged2 = this.SliderStepInputField.onValueChanged;
		if (onValueChanged2 != null)
		{
			onValueChanged2.Invoke(this.SliderStepInputField.text);
		}
	}

	// Token: 0x060027C0 RID: 10176 RVA: 0x00124C50 File Offset: 0x00122E50
	private int GetSliderClampedValue(string value)
	{
		int min;
		int.TryParse(this.SliderMinInputField.text, out min);
		int max;
		int.TryParse(this.SliderMaxInputField.text, out max);
		int cur;
		int.TryParse(value, out cur);
		return Mathf.Clamp(cur, min, max);
	}

	// Token: 0x060027C1 RID: 10177 RVA: 0x00124C9C File Offset: 0x00122E9C
	private void OnTypeDropdownValueChanged(int value)
	{
		bool flag = value != 0;
		if (flag)
		{
			this.ToggleDropdown.value = 0;
		}
		bool flag2 = value != 1;
		if (flag2)
		{
			this.InputField.text = string.Empty;
		}
		bool flag3 = value != 2;
		if (flag3)
		{
			this.SliderMinInputField.text = string.Empty;
			this.SliderMaxInputField.text = string.Empty;
			this.SliderDefaultInputField.text = string.Empty;
			this.SliderStepInputField.text = string.Empty;
		}
		bool flag4 = value != 3;
		if (flag4)
		{
			this._dropdownOptionList.Clear();
		}
		this.toggleSetting.gameObject.SetActive(false);
		this.inputFieldSetting.gameObject.SetActive(false);
		this.sliderSetting.gameObject.SetActive(false);
		this.dropdownSetting.gameObject.SetActive(false);
		switch (value)
		{
		case 0:
			this.toggleSetting.gameObject.SetActive(true);
			break;
		case 1:
			this.inputFieldSetting.gameObject.SetActive(true);
			break;
		case 2:
			this.sliderSetting.gameObject.SetActive(true);
			break;
		case 3:
		{
			bool flag5 = this._dropdownOptionList.Count == 0;
			if (flag5)
			{
				this._dropdownOptionList.Add(null);
			}
			this.RefreshDropdownItemList();
			this.dropdownSetting.gameObject.SetActive(true);
			break;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060027C2 RID: 10178 RVA: 0x00124E34 File Offset: 0x00123034
	private void OnClickConfirm()
	{
		switch (this.typeDropdown.value)
		{
		case 0:
			this._curSettingEntry = new ToggleSetting(this._group, this.titleKeyInputField.text, this.titleInputField.text, this.descriptionInputField.text, this.ToggleDropdown.value != 0);
			break;
		case 1:
			this._curSettingEntry = new InputFieldSetting(this._group, this.titleKeyInputField.text, this.titleInputField.text, this.descriptionInputField.text, this.InputField.text);
			break;
		case 2:
			this._curSettingEntry = new SliderSetting(this._group, this.titleKeyInputField.text, this.titleInputField.text, this.descriptionInputField.text, int.Parse(this.SliderDefaultInputField.text), int.Parse(this.SliderMinInputField.text), int.Parse(this.SliderMaxInputField.text), int.Parse(this.SliderStepInputField.text));
			break;
		case 3:
			this._dropdownOptionList.RemoveAll((string o) => o.IsNullOrEmpty());
			this._curSettingEntry = new DropdownSetting(this._group, this.titleKeyInputField.text, this.titleInputField.text, this.descriptionInputField.text, 0, this._dropdownOptionList);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		bool flag = this._index == -1;
		if (flag)
		{
			this._curModSettingEntry.Add(this._curSettingEntry);
		}
		else
		{
			this._curModSettingEntry[this._index] = this._curSettingEntry;
		}
		this.Hide();
		GEvent.OnEvent(UiEvents.ModEditSettings, null);
	}

	// Token: 0x060027C3 RID: 10179 RVA: 0x00125023 File Offset: 0x00123223
	private void OnClickCancel()
	{
		this.Hide();
	}

	// Token: 0x060027C4 RID: 10180 RVA: 0x0012502C File Offset: 0x0012322C
	public void Show(List<SettingEntry> settingEntries, SettingEntry entry, string group, List<string> groupList)
	{
		this.btnYes.interactable = false;
		this._curModSettingEntry = settingEntries;
		this._group = group;
		this._index = this._curModSettingEntry.IndexOf(entry);
		this._curSettingEntry = entry;
		this.groupDropdown.ClearOptions();
		this.groupDropdown.interactable = (groupList.Count > 0);
		bool interactable = this.groupDropdown.interactable;
		if (interactable)
		{
			this.groupDropdown.AddOptions(groupList);
			int groupIndex = groupList.IndexOf(group);
			bool flag = groupIndex >= 0;
			if (flag)
			{
				this.groupDropdown.SetValueWithoutNotify(groupIndex);
			}
			this.groupDropdown.onValueChanged.RemoveAllListeners();
			this.groupDropdown.onValueChanged.AddListener(delegate(int value)
			{
				this._group = groupList[value];
			});
		}
		SettingEntry curSettingEntry = this._curSettingEntry;
		SettingEntry settingEntry = curSettingEntry;
		ToggleSetting toggleSettingEntry = settingEntry as ToggleSetting;
		if (toggleSettingEntry == null)
		{
			InputFieldSetting inputFieldSettingEntry = settingEntry as InputFieldSetting;
			if (inputFieldSettingEntry == null)
			{
				SliderSetting sliderSettingEntry = settingEntry as SliderSetting;
				if (sliderSettingEntry == null)
				{
					DropdownSetting dropdownSettingEntry = settingEntry as DropdownSetting;
					if (dropdownSettingEntry == null)
					{
						this.typeDropdown.value = SettingEntry.SettingType.Toggle.ToInt();
					}
					else
					{
						this._dropdownOptionList.Clear();
						this._dropdownOptionList.AddRange(dropdownSettingEntry.Options);
						bool flag2 = this._dropdownOptionList.Count == 0;
						if (flag2)
						{
							this._dropdownOptionList.Add(null);
						}
						this.typeDropdown.value = SettingEntry.SettingType.Dropdown.ToInt();
					}
				}
				else
				{
					this.SliderMinInputField.text = sliderSettingEntry.MinValue.ToString();
					this.SliderMaxInputField.text = sliderSettingEntry.MaxValue.ToString();
					this.SliderDefaultInputField.text = sliderSettingEntry.Value.ToString();
					this.SliderStepInputField.text = sliderSettingEntry.StepSize.ToString();
					this.typeDropdown.value = SettingEntry.SettingType.Slider.ToInt();
				}
			}
			else
			{
				this.InputField.text = inputFieldSettingEntry.Value;
				this.typeDropdown.value = SettingEntry.SettingType.InputField.ToInt();
			}
		}
		else
		{
			this.ToggleDropdown.value = (toggleSettingEntry.Value ? 1 : 0);
			this.typeDropdown.value = SettingEntry.SettingType.Toggle.ToInt();
		}
		TMP_InputField tmp_InputField = this.titleInputField;
		SettingEntry curSettingEntry2 = this._curSettingEntry;
		tmp_InputField.text = (((curSettingEntry2 != null) ? curSettingEntry2.DisplayName : null) ?? string.Empty);
		TMP_InputField tmp_InputField2 = this.titleKeyInputField;
		SettingEntry curSettingEntry3 = this._curSettingEntry;
		tmp_InputField2.text = (((curSettingEntry3 != null) ? curSettingEntry3.Key : null) ?? string.Empty);
		TMP_InputField tmp_InputField3 = this.descriptionInputField;
		SettingEntry curSettingEntry4 = this._curSettingEntry;
		tmp_InputField3.text = (((curSettingEntry4 != null) ? curSettingEntry4.Description : null) ?? string.Empty);
		base.gameObject.SetActive(true);
	}

	// Token: 0x060027C5 RID: 10181 RVA: 0x00125350 File Offset: 0x00123550
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x060027C6 RID: 10182 RVA: 0x00125360 File Offset: 0x00123560
	private bool CheckCondition()
	{
		bool flag = this.titleInputField.text.IsNullOrEmpty();
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = this.titleKeyInputField.text.IsNullOrEmpty();
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = this.descriptionInputField.text.IsNullOrEmpty();
				if (flag3)
				{
					result = false;
				}
				else
				{
					SettingEntry.SettingType settingType = (SettingEntry.SettingType)this.typeDropdown.value;
					bool flag4 = settingType == SettingEntry.SettingType.Slider;
					if (flag4)
					{
						bool flag5 = this.SliderMinInputField.text.IsNullOrEmpty();
						if (flag5)
						{
							return false;
						}
						bool flag6 = this.SliderMaxInputField.text.IsNullOrEmpty();
						if (flag6)
						{
							return false;
						}
						bool flag7 = this.SliderDefaultInputField.text.IsNullOrEmpty();
						if (flag7)
						{
							return false;
						}
						bool flag8 = this.SliderStepInputField.text.IsNullOrEmpty();
						if (flag8)
						{
							return false;
						}
					}
					else
					{
						bool flag9 = settingType == SettingEntry.SettingType.Dropdown;
						if (flag9)
						{
							bool flag10 = this._dropdownOptionList.Exists((string o) => o.IsNullOrEmpty());
							if (flag10)
							{
								return false;
							}
						}
					}
					result = true;
				}
			}
		}
		return result;
	}

	// Token: 0x060027C7 RID: 10183 RVA: 0x0012548B File Offset: 0x0012368B
	private void RefreshConfirmButton()
	{
		this.btnYes.interactable = this.CheckCondition();
	}

	// Token: 0x060027C8 RID: 10184 RVA: 0x001254A0 File Offset: 0x001236A0
	private void Update()
	{
		this.RefreshConfirmButton();
	}

	// Token: 0x04001CFD RID: 7421
	[SerializeField]
	private CButton btnYes;

	// Token: 0x04001CFE RID: 7422
	[SerializeField]
	private CButton btnNo;

	// Token: 0x04001CFF RID: 7423
	[SerializeField]
	private CDropdown typeDropdown;

	// Token: 0x04001D00 RID: 7424
	[SerializeField]
	private CDropdown groupDropdown;

	// Token: 0x04001D01 RID: 7425
	[SerializeField]
	private TMP_InputField titleInputField;

	// Token: 0x04001D02 RID: 7426
	[SerializeField]
	private TMP_InputField titleKeyInputField;

	// Token: 0x04001D03 RID: 7427
	[SerializeField]
	private TMP_InputField descriptionInputField;

	// Token: 0x04001D04 RID: 7428
	[SerializeField]
	private ModEditToggleSetting toggleSetting;

	// Token: 0x04001D05 RID: 7429
	[SerializeField]
	private ModEditInputFieldSetting inputFieldSetting;

	// Token: 0x04001D06 RID: 7430
	[SerializeField]
	private ModEditSliderSetting sliderSetting;

	// Token: 0x04001D07 RID: 7431
	[SerializeField]
	private ModEditDropdownSetting dropdownSetting;

	// Token: 0x04001D08 RID: 7432
	private List<SettingEntry> _curModSettingEntry;

	// Token: 0x04001D09 RID: 7433
	private SettingEntry _curSettingEntry;

	// Token: 0x04001D0A RID: 7434
	private int _index;

	// Token: 0x04001D0B RID: 7435
	private string _group;

	// Token: 0x04001D0C RID: 7436
	private static readonly List<LanguageKey> TypeNameKeyList = new List<LanguageKey>
	{
		LanguageKey.LK_Mod_SettingType_Toggle,
		LanguageKey.LK_Mod_SettingType_InputField,
		LanguageKey.LK_Mod_SettingType_Slider,
		LanguageKey.LK_Mod_SettingType_Dropdown
	};

	// Token: 0x04001D0D RID: 7437
	private static readonly List<LanguageKey> SwitchKeyNameList = new List<LanguageKey>
	{
		LanguageKey.LK_Off,
		LanguageKey.LK_On
	};

	// Token: 0x04001D0E RID: 7438
	private readonly List<string> _dropdownOptionList = new List<string>();
}
