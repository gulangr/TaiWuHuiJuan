using System;
using FrameWork.ModSystem;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate.Mod;
using TMPro;
using UnityEngine;

namespace Game.Views.Mod.Upload
{
	// Token: 0x020008D5 RID: 2261
	public class ModSettingEntry : MonoBehaviour
	{
		// Token: 0x06006C4F RID: 27727 RVA: 0x0031EC98 File Offset: 0x0031CE98
		public void Refresh(SettingEntry settingEntry, Action<SettingEntry> onClick, Action<SettingEntry> onDelete)
		{
			this._settingEntry = settingEntry;
			this.toggleContainer.gameObject.SetActive(this._settingEntry.Type == SettingEntry.SettingType.Toggle);
			this.inputFieldContainer.gameObject.SetActive(this._settingEntry.Type == SettingEntry.SettingType.InputField);
			this.sliderContainer.gameObject.SetActive(this._settingEntry.Type == SettingEntry.SettingType.Slider);
			this.dropdownContainer.gameObject.SetActive(this._settingEntry.Type == SettingEntry.SettingType.Dropdown);
			TextMeshProUGUI label;
			switch (this._settingEntry.Type)
			{
			case SettingEntry.SettingType.Toggle:
			{
				ToggleSetting toggleSettingEntry = settingEntry as ToggleSetting;
				CToggle toggle = this.toggleContainer.toggle;
				toggle.onValueChanged.RemoveAllListeners();
				toggle.isOn = toggleSettingEntry.Value;
				toggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					toggleSettingEntry.Value = isOn;
				});
				toggle.interactable = true;
				this.toggleContainer.label.SetText(settingEntry.DisplayName, true);
				label = this.toggleContainer.label;
				break;
			}
			case SettingEntry.SettingType.InputField:
			{
				InputFieldSetting inputFieldSettingEntry = settingEntry as InputFieldSetting;
				TMP_InputField inputField = this.inputFieldContainer.inputField;
				inputField.onEndEdit.RemoveAllListeners();
				inputField.text = inputFieldSettingEntry.Value;
				((TextMeshProUGUI)inputField.placeholder).text = LocalStringManager.Get(LanguageKey.LK_EventInput_StringHolderTips);
				inputField.onEndEdit.AddListener(delegate(string val)
				{
					inputFieldSettingEntry.Value = val;
				});
				inputField.interactable = true;
				label = this.inputFieldContainer.label;
				break;
			}
			case SettingEntry.SettingType.Slider:
			{
				SliderSetting sliderSettingEntry = settingEntry as SliderSetting;
				CSlider slider = this.sliderContainer.slider;
				slider.onValueChanged.RemoveAllListeners();
				slider.wholeNumbers = true;
				slider.maxValue = (float)sliderSettingEntry.MaxValue;
				slider.minValue = (float)sliderSettingEntry.MinValue;
				slider.value = (float)sliderSettingEntry.Value;
				TextMeshProUGUI curValue = this.sliderContainer.curValue;
				curValue.text = sliderSettingEntry.Value.ToString();
				slider.onValueChanged.AddListener(delegate(float val)
				{
					int intVal = (int)val;
					sliderSettingEntry.Value = intVal;
					curValue.text = ((int)val).ToString();
				});
				slider.interactable = true;
				label = this.sliderContainer.label;
				break;
			}
			case SettingEntry.SettingType.Dropdown:
			{
				DropdownSetting dropdownSettingEntry = settingEntry as DropdownSetting;
				CDropdown dropdown = this.dropdownContainer.dropdown;
				dropdown.onValueChanged.RemoveAllListeners();
				dropdown.ClearOptions();
				dropdown.AddOptions(dropdownSettingEntry.Options);
				dropdown.value = dropdownSettingEntry.Value;
				dropdown.onValueChanged.AddListener(delegate(int val)
				{
					dropdownSettingEntry.Value = val;
				});
				dropdown.interactable = true;
				label = this.dropdownContainer.label;
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
			label.SetText(settingEntry.DisplayName, true);
			TooltipInvoker tipsDisplayer = label.GetComponent<TooltipInvoker>();
			tipsDisplayer.PresetParam[0] = settingEntry.Description;
			tipsDisplayer.Refresh(false, -1);
			Action <>9__6;
			this.buttonDelete.ClearAndAddListener(delegate
			{
				string title = LanguageKey.LK_Mod_EditSetting_RemoveEntry.Tr();
				string content = LanguageKey.LK_Mod_EditSetting_RemoveEntry_Confirm.Tr();
				string title2 = title;
				string text = content;
				Action onConfirm;
				if ((onConfirm = <>9__6) == null)
				{
					onConfirm = (<>9__6 = delegate()
					{
						onDelete(settingEntry);
					});
				}
				CommonUtils.ShowConfirmDialog(title2, text, onConfirm, null, EDialogType.None);
			});
			this.button.ClearAndAddListener(delegate
			{
				onClick(settingEntry);
			});
		}

		// Token: 0x04004E8A RID: 20106
		[SerializeField]
		private CButton button;

		// Token: 0x04004E8B RID: 20107
		[SerializeField]
		private CButton buttonDelete;

		// Token: 0x04004E8C RID: 20108
		[SerializeField]
		private ModSettingWidgetsToggleContainer toggleContainer;

		// Token: 0x04004E8D RID: 20109
		[SerializeField]
		private ModSettingWidgetsInputFieldContainer inputFieldContainer;

		// Token: 0x04004E8E RID: 20110
		[SerializeField]
		private ModSettingWidgetsSliderContainer sliderContainer;

		// Token: 0x04004E8F RID: 20111
		[SerializeField]
		private ModSettingWidgetsDropdownContainer dropdownContainer;

		// Token: 0x04004E90 RID: 20112
		private SettingEntry _settingEntry;
	}
}
