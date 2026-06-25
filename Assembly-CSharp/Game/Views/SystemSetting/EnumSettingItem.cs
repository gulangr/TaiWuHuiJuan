using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.UI.LanguageRule;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.SystemSetting
{
	// Token: 0x0200075E RID: 1886
	public class EnumSettingItem : SettingItemBase
	{
		// Token: 0x06005B41 RID: 23361 RVA: 0x002A5841 File Offset: 0x002A3A41
		private void Awake()
		{
			this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnDropdownChanged));
		}

		// Token: 0x06005B42 RID: 23362 RVA: 0x002A5864 File Offset: 0x002A3A64
		public override void Initialize(ISettingItemInfo info)
		{
			base.Initialize(info);
			this._isStringMode = (info.PropertyType == typeof(string));
			EnumSettingAttribute attr = info.Attribute as EnumSettingAttribute;
			bool isStringMode = this._isStringMode;
			if (isStringMode)
			{
				SettingItemInfo<string> typedInfo3 = (SettingItemInfo<string>)info;
				this._getValueAction = null;
				this._setValueAction = null;
				this.dropdown.ClearOptions();
				bool flag = ((attr != null) ? attr.Options : null) != null && attr.Options.Length != 0;
				if (flag)
				{
					List<string> optionTexts = new List<string>();
					foreach (LanguageKey key in attr.Options)
					{
						optionTexts.Add(key.Tr());
					}
					this.dropdown.AddOptions(optionTexts);
				}
				else
				{
					bool flag2 = attr.Key == ESettingKey.Language;
					if (flag2)
					{
						string[] languages = LocalStringManager.GetAvailableLanguages().ToArray<string>();
						this.dropdown.AddOptions(languages.Select(new Func<string, string>(LocalStringManager.GetLanguageName)).ToList<string>());
					}
				}
				string currentLang = typedInfo3.GetValue();
				int index = this.dropdown.options.FindIndex((CDropdown.OptionData o) => o.text == currentLang);
				bool flag3 = attr.Key == ESettingKey.Language;
				if (flag3)
				{
					List<string> languages2 = LocalStringManager.GetAvailableLanguages().ToList<string>();
					index = languages2.FindIndex((string v) => v == currentLang);
				}
				bool flag4 = index >= 0;
				if (flag4)
				{
					this.dropdown.value = index;
				}
			}
			else
			{
				bool isEnum = info.PropertyType.IsEnum;
				if (isEnum)
				{
					SettingItemInfo<int> typedInfo = (SettingItemInfo<int>)info;
					this._getValueAction = (() => typedInfo.GetValue());
					this._setValueAction = delegate(int v)
					{
						typedInfo.SetValue(v);
					};
					this.dropdown.ClearOptions();
					string[] enumNames = Enum.GetNames(info.PropertyType);
					this.dropdown.AddOptions(new List<string>(enumNames));
					this._value = this._getValueAction();
					this.dropdown.value = this._value;
				}
				else
				{
					bool flag5 = info.PropertyType == typeof(sbyte);
					if (flag5)
					{
						SettingItemInfo<sbyte> typedInfo = (SettingItemInfo<sbyte>)info;
						this._getValueAction = (() => (int)typedInfo.GetValue());
						this._setValueAction = delegate(int v)
						{
							typedInfo.SetValue((sbyte)v);
						};
						this.dropdown.ClearOptions();
						bool flag6 = ((attr != null) ? attr.Options : null) != null && attr.Options.Length != 0;
						if (flag6)
						{
							List<string> optionTexts2 = new List<string>();
							foreach (LanguageKey key2 in attr.Options)
							{
								optionTexts2.Add(key2.Tr());
							}
							this.dropdown.AddOptions(optionTexts2);
						}
						else
						{
							for (sbyte i = 0; i < 10; i += 1)
							{
								this.dropdown.options.Add(new CDropdown.OptionData(CommonUtils.GetPreGradeText(i)));
							}
						}
						this._value = this._getValueAction();
						bool flag7 = this._value < 0;
						if (flag7)
						{
							this._value = 0;
						}
						this.dropdown.value = this._value;
					}
					else
					{
						bool flag8 = info.PropertyType == typeof(int);
						if (flag8)
						{
							SettingItemInfo<int> typedInfo = (SettingItemInfo<int>)info;
							this._getValueAction = (() => typedInfo.GetValue());
							this._setValueAction = delegate(int v)
							{
								typedInfo.SetValue(v);
							};
							this.dropdown.ClearOptions();
							bool flag9 = ((attr != null) ? attr.Options : null) != null && attr.Options.Length != 0;
							if (flag9)
							{
								List<string> optionTexts3 = new List<string>();
								foreach (LanguageKey key3 in attr.Options)
								{
									optionTexts3.Add(key3.Tr());
								}
								this.dropdown.AddOptions(optionTexts3);
							}
							this._value = this._getValueAction();
							this.dropdown.value = this._value;
						}
						else
						{
							bool flag10 = info.PropertyType == typeof(Vector2Int);
							if (flag10)
							{
								this._isResolutionMode = true;
								SettingItemInfo<Vector2Int> typedInfo2 = (SettingItemInfo<Vector2Int>)info;
								Vector2Int maxResolution = GlobalSettings.GetMaxResolution();
								this._resolutionList.Clear();
								for (int j = 0; j < EnumSettingItem.PresetResolutions.Length; j++)
								{
									Vector2Int resolution = EnumSettingItem.PresetResolutions[j];
									bool flag11 = resolution.x <= maxResolution.x && resolution.y <= maxResolution.y;
									if (flag11)
									{
										bool flag12 = !this._resolutionList.Contains(resolution);
										if (flag12)
										{
											this._resolutionList.Add(resolution);
										}
									}
								}
								this._resolutionList.Sort((Vector2Int left, Vector2Int right) => (left.x != right.x) ? (right.x - left.x) : (right.y - left.y));
								this.dropdown.ClearOptions();
								List<string> optionTexts4 = new List<string>();
								for (int k = 0; k < this._resolutionList.Count; k++)
								{
									optionTexts4.Add(string.Format("{0} x {1}", this._resolutionList[k].x, this._resolutionList[k].y));
								}
								this.dropdown.AddOptions(optionTexts4);
								Vector2Int currentValue = typedInfo2.GetValue();
								this._value = this._resolutionList.IndexOf(currentValue);
								bool flag13 = this._value < 0;
								if (flag13)
								{
									this._value = 0;
								}
								this.dropdown.value = this._value;
							}
						}
					}
				}
			}
		}

		// Token: 0x06005B43 RID: 23363 RVA: 0x002A5EAC File Offset: 0x002A40AC
		private void OnDropdownChanged(int index)
		{
			int oldIndex = this._value;
			this._value = index;
			bool isStringMode = this._isStringMode;
			if (isStringMode)
			{
				EnumSettingAttribute attr = base.Info.Attribute as EnumSettingAttribute;
				bool flag = attr.Key == ESettingKey.Language;
				if (flag)
				{
					this.OnLanguageChange(index, oldIndex);
				}
				else
				{
					string selectedText = this.dropdown.options[index].text;
					base.Info.SetValueBoxed(selectedText);
				}
			}
			else
			{
				bool isResolutionMode = this._isResolutionMode;
				if (isResolutionMode)
				{
					bool flag2 = index >= 0 && index < this._resolutionList.Count;
					if (flag2)
					{
						SettingItemInfo<Vector2Int> typedInfo = (SettingItemInfo<Vector2Int>)base.Info;
						typedInfo.SetValue(this._resolutionList[index]);
					}
				}
				else
				{
					this._setValueAction(this._value);
				}
			}
			base.NotifyChanged();
		}

		// Token: 0x06005B44 RID: 23364 RVA: 0x002A5F94 File Offset: 0x002A4194
		public override object GetValue()
		{
			bool isStringMode = this._isStringMode;
			object result;
			if (isStringMode)
			{
				result = this.dropdown.options[this._value].text;
			}
			else
			{
				bool isResolutionMode = this._isResolutionMode;
				if (isResolutionMode)
				{
					result = this._resolutionList[this._value];
				}
				else
				{
					result = this._value;
				}
			}
			return result;
		}

		// Token: 0x06005B45 RID: 23365 RVA: 0x002A5FFC File Offset: 0x002A41FC
		public override void SetValue(object value)
		{
			bool isStringMode = this._isStringMode;
			if (isStringMode)
			{
				string strValue = (string)value;
				int index = this.dropdown.options.FindIndex((CDropdown.OptionData o) => o.text == strValue);
				bool flag = index >= 0;
				if (flag)
				{
					this._value = index;
					this.dropdown.value = index;
				}
			}
			else
			{
				bool isResolutionMode = this._isResolutionMode;
				if (isResolutionMode)
				{
					Vector2Int vec2;
					bool flag2;
					if (value is Vector2Int)
					{
						vec2 = (Vector2Int)value;
						flag2 = true;
					}
					else
					{
						flag2 = false;
					}
					bool flag3 = flag2;
					if (flag3)
					{
						this._value = this._resolutionList.IndexOf(vec2);
						bool flag4 = this._value < 0;
						if (flag4)
						{
							this._value = 0;
						}
						this.dropdown.value = this._value;
					}
				}
				else
				{
					this._value = Convert.ToInt32(value);
					bool flag5 = this._value < 0;
					if (flag5)
					{
						this._value = 0;
					}
					this.dropdown.value = this._value;
				}
			}
		}

		// Token: 0x06005B46 RID: 23366 RVA: 0x002A6108 File Offset: 0x002A4308
		public void SetTypedValue(int value)
		{
			this._value = value;
			bool flag = this._value < 0;
			if (flag)
			{
				this._value = 0;
			}
			bool flag2 = this.dropdown != null;
			if (flag2)
			{
				this.dropdown.value = this._value;
			}
		}

		// Token: 0x06005B47 RID: 23367 RVA: 0x002A6154 File Offset: 0x002A4354
		public override void SetInteractable(bool interactable)
		{
			bool flag = this.dropdown != null;
			if (flag)
			{
				this.dropdown.interactable = interactable;
			}
		}

		// Token: 0x06005B48 RID: 23368 RVA: 0x002A6180 File Offset: 0x002A4380
		private void OnDestroy()
		{
			bool flag = this.dropdown != null;
			if (flag)
			{
				this.dropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnDropdownChanged));
			}
		}

		// Token: 0x06005B49 RID: 23369 RVA: 0x002A61BC File Offset: 0x002A43BC
		private void OnLanguageChange(int newIndex, int oldIndex)
		{
			EnumSettingItem.<>c__DisplayClass16_0 CS$<>8__locals1 = new EnumSettingItem.<>c__DisplayClass16_0();
			CS$<>8__locals1.<>4__this = this;
			string[] languages = LocalStringManager.GetAvailableLanguages().ToArray<string>();
			CS$<>8__locals1.global = SingletonObject.getInstance<GlobalSettings>();
			CS$<>8__locals1.original = oldIndex;
			CS$<>8__locals1.target = languages[newIndex];
			UIElement mainMenuElement = UIElement.MainMenu;
			bool isShowing = mainMenuElement.IsShowing;
			if (isShowing)
			{
				string globalLanguage = CS$<>8__locals1.global.Language;
				CS$<>8__locals1.<OnLanguageChange>g__ConfirmLanguageChange|1(CS$<>8__locals1.target);
				bool flag = globalLanguage != CS$<>8__locals1.target;
				if (flag)
				{
					UIElement.SystemSetting.UiBaseAs<ViewSystemSetting>().Refresh();
					Array.ForEach<LanguageRuleTips>(this.dropdown.transform.GetComponentsInChildren<LanguageRuleTips>(), delegate(LanguageRuleTips v)
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
						Content = EnumSettingItem.<OnLanguageChange>g__MultiText|16_0(LanguageKey.LK_SystemSetting_LocalizationSetting_Language_Reset_Text, CS$<>8__locals1.target, true),
						GroupYesText = EnumSettingItem.<OnLanguageChange>g__MultiText|16_0(LanguageKey.LK_Confirm, CS$<>8__locals1.target, false),
						GroupNoText = EnumSettingItem.<OnLanguageChange>g__MultiText|16_0(LanguageKey.LK_Cancel, CS$<>8__locals1.target, false),
						DialogType = EDialogType.None,
						Yes = delegate()
						{
							ArgumentBox box = null;
							Action onShowAction = null;
							Action onHideAction;
							if ((onHideAction = CS$<>8__locals1.<>9__5) == null)
							{
								onHideAction = (CS$<>8__locals1.<>9__5 = delegate()
								{
									base.<OnLanguageChange>g__ConfirmLanguageChange|1(CS$<>8__locals1.target);
								});
							}
							GameApp.ReturnToMainMenu(box, onShowAction, onHideAction);
						},
						No = delegate()
						{
							CS$<>8__locals1.<>4__this.dropdown.value = CS$<>8__locals1.original;
						}
					};
					CommonUtils.ShowDialog(cmd);
				}
			}
		}

		// Token: 0x06005B4C RID: 23372 RVA: 0x002A63EC File Offset: 0x002A45EC
		[CompilerGenerated]
		internal static string <OnLanguageChange>g__MultiText|16_0(LanguageKey key, string language, bool styleNewLine)
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

		// Token: 0x04003EF0 RID: 16112
		[SerializeField]
		private CDropdown dropdown;

		// Token: 0x04003EF1 RID: 16113
		private int _value;

		// Token: 0x04003EF2 RID: 16114
		private bool _isStringMode;

		// Token: 0x04003EF3 RID: 16115
		private bool _isResolutionMode;

		// Token: 0x04003EF4 RID: 16116
		private List<Vector2Int> _resolutionList = new List<Vector2Int>();

		// Token: 0x04003EF5 RID: 16117
		private Action<int> _setValueAction;

		// Token: 0x04003EF6 RID: 16118
		private Func<int> _getValueAction;

		// Token: 0x04003EF7 RID: 16119
		private static readonly Vector2Int[] PresetResolutions = new Vector2Int[]
		{
			new Vector2Int(3840, 2160),
			new Vector2Int(2560, 1440),
			new Vector2Int(1920, 1080),
			new Vector2Int(1600, 900),
			new Vector2Int(1360, 768),
			new Vector2Int(1280, 720)
		};
	}
}
