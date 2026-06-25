using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.Global;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x02000803 RID: 2051
	public class NewGameSubPageCustomPresetAttribute : NewGameSubPageCustomPresetItemBase
	{
		// Token: 0x17000C0F RID: 3087
		// (get) Token: 0x06006445 RID: 25669 RVA: 0x002DEB70 File Offset: 0x002DCD70
		public override DialogCmd StartGameCheck
		{
			get
			{
				DialogCmd result;
				if (this.RemainingPoints <= 0)
				{
					result = null;
				}
				else
				{
					DialogCmd dialogCmd = new DialogCmd();
					dialogCmd.Title = LanguageKey.LK_NewGame_StartGameCheck_NewGameSubPageCustomPreset_Title.Tr();
					dialogCmd.Content = LanguageKey.LK_NewGame_StartGameCheck_NewGameSubPageCustomPreset_Desc.Tr();
					dialogCmd.Yes = delegate()
					{
						this.ParentCustomPresetPage.StartGameChecked = true;
					};
					result = dialogCmd;
					dialogCmd.No = delegate()
					{
						this.ParentCustomPresetPage.SwitchToSubPage(NewGameSubPageCustomPreset.ESubPageType.Attribute, true, true);
					};
				}
				return result;
			}
		}

		// Token: 0x17000C10 RID: 3088
		// (get) Token: 0x06006446 RID: 25670 RVA: 0x002DEBD2 File Offset: 0x002DCDD2
		// (set) Token: 0x06006447 RID: 25671 RVA: 0x002DEBDA File Offset: 0x002DCDDA
		public override bool StartGameChecked { get; set; }

		// Token: 0x17000C11 RID: 3089
		// (get) Token: 0x06006448 RID: 25672 RVA: 0x002DEBE3 File Offset: 0x002DCDE3
		public override int SpentPoints
		{
			get
			{
				return this._spentPoints;
			}
		}

		// Token: 0x17000C12 RID: 3090
		// (get) Token: 0x06006449 RID: 25673 RVA: 0x002DEBEB File Offset: 0x002DCDEB
		public override int RemainingPoints
		{
			get
			{
				return (int)GlobalConfig.Instance.CustomProtagonistMainAttributeTotalPoint - this._spentPoints;
			}
		}

		// Token: 0x0600644A RID: 25674 RVA: 0x002DEBFE File Offset: 0x002DCDFE
		private void Awake()
		{
			this.EnsureUiInitialized();
		}

		// Token: 0x0600644B RID: 25675 RVA: 0x002DEC08 File Offset: 0x002DCE08
		public override void RefreshUI()
		{
			this.EnsureUiInitialized();
			this.RefreshAttributeUI();
		}

		// Token: 0x0600644C RID: 25676 RVA: 0x002DEC19 File Offset: 0x002DCE19
		public override void ApplyToPreset(CustomProtagonistPresetItem presetItem)
		{
			presetItem.MainAttributes = this._mainAttributes;
		}

		// Token: 0x0600644D RID: 25677 RVA: 0x002DEC28 File Offset: 0x002DCE28
		public override void ApplyFromPreset(CustomProtagonistPresetItem presetItem)
		{
			this._mainAttributes = presetItem.MainAttributes;
			this._spentPoints = this._mainAttributes.GetSum();
			this.RefreshAttributeResetButton();
		}

		// Token: 0x0600644E RID: 25678 RVA: 0x002DEC50 File Offset: 0x002DCE50
		public unsafe void ResetMainAttributes()
		{
			short defaultValue = GlobalConfig.Instance.CustomProtagonistMainAttributeDefaultPoint;
			bool hasChanged = false;
			for (int i = 0; i < 6; i++)
			{
				sbyte type = (sbyte)i;
				bool flag = *this._mainAttributes[(int)type] == defaultValue;
				if (!flag)
				{
					*this._mainAttributes[(int)type] = defaultValue;
					hasChanged = true;
				}
			}
			bool flag2 = !hasChanged;
			if (flag2)
			{
				this.RefreshAttributeResetButton();
			}
			else
			{
				int oldSpent = this._spentPoints;
				this._spentPoints = this._mainAttributes.GetSum();
				bool flag3 = oldSpent != this._spentPoints;
				if (flag3)
				{
					this.ParentCustomPresetPage.OnSubPagePointsChanged();
				}
				base.NotifyDataModified();
				this.RefreshUI();
				this.RefreshAttributeResetButton();
			}
		}

		// Token: 0x0600644F RID: 25679 RVA: 0x002DED0C File Offset: 0x002DCF0C
		public unsafe void AddMainAttribute(sbyte type, int delta)
		{
			bool flag = this.ParentCustomPresetPage == null;
			if (!flag)
			{
				GlobalConfig global = GlobalConfig.Instance;
				int maxPerAttribute = (int)global.CustomProtagonistMainAttributeMaxPoint;
				int defaultValue = (int)global.CustomProtagonistMainAttributeDefaultPoint;
				int totalPoint = (int)global.CustomProtagonistMainAttributeTotalPoint;
				int currentValue = (int)(*this._mainAttributes[(int)type]);
				int newValue = Mathf.Clamp(currentValue + delta, 0, maxPerAttribute);
				int actualDelta = newValue - currentValue;
				bool flag2 = actualDelta == 0;
				if (!flag2)
				{
					int currentTotal = this._mainAttributes.GetSum();
					int newTotal = currentTotal + actualDelta;
					bool flag3 = newTotal > totalPoint;
					if (flag3)
					{
						actualDelta = totalPoint - currentTotal;
						newValue = currentValue + actualDelta;
					}
					bool flag4 = actualDelta == 0;
					if (!flag4)
					{
						int oldSpent = this._spentPoints;
						*this._mainAttributes[(int)type] = (short)newValue;
						this._spentPoints = this._mainAttributes.GetSum();
						int pointsDelta = this._spentPoints - oldSpent;
						bool flag5 = pointsDelta != 0;
						if (flag5)
						{
							this.ParentCustomPresetPage.OnSubPagePointsChanged();
						}
						base.NotifyDataModified();
						this.RefreshUI();
						this.RefreshAttributeResetButton();
					}
				}
			}
		}

		// Token: 0x06006450 RID: 25680 RVA: 0x002DEE1C File Offset: 0x002DD01C
		private unsafe void RefreshAttributeResetButton()
		{
			short defaultValue = GlobalConfig.Instance.CustomProtagonistMainAttributeDefaultPoint;
			for (int i = 0; i < 6; i++)
			{
				bool flag = *this._mainAttributes[(int)((sbyte)i)] != defaultValue;
				if (flag)
				{
					this.attributeResetButton.interactable = true;
					return;
				}
			}
			this.attributeResetButton.interactable = false;
		}

		// Token: 0x06006451 RID: 25681 RVA: 0x002DEE7C File Offset: 0x002DD07C
		private void EnsureUiInitialized()
		{
			bool uiInitialized = this._uiInitialized;
			if (!uiInitialized)
			{
				this.attributeResetButton.onClick.ResetListener(new Action(this.ResetMainAttributes));
				this.InitializeAttributeItems();
				this._uiInitialized = true;
			}
		}

		// Token: 0x06006452 RID: 25682 RVA: 0x002DEEC4 File Offset: 0x002DD0C4
		private void InitializeAttributeItems()
		{
			CommonUtils.PrepareEnoughChildren(this.attributeGrid, this.attributeItemTemplate.gameObject, 6, null);
			this._attributeItems.Clear();
			for (int i = 0; i < 6; i++)
			{
				NewGameCustomPresetPointItem item = this.attributeGrid.GetChild(i).GetComponent<NewGameCustomPresetPointItem>();
				item.Initialize(NewGameSubPageCustomPresetAttribute.MainAttributeDisplayOrder[i], new Action<sbyte, int>(this.AddMainAttribute));
				this._attributeItems.Add(item);
			}
		}

		// Token: 0x06006453 RID: 25683 RVA: 0x002DEF4C File Offset: 0x002DD14C
		private unsafe void RefreshAttributeUI()
		{
			bool flag = this._attributeItems.Count != 6;
			if (flag)
			{
				this.InitializeAttributeItems();
			}
			GlobalConfig global = GlobalConfig.Instance;
			short totalPoint = global.CustomProtagonistMainAttributeTotalPoint;
			int currentTotal = this._mainAttributes.GetSum();
			short maxPoint = global.CustomProtagonistMainAttributeMaxPoint;
			short defaultPoint = global.CustomProtagonistMainAttributeDefaultPoint;
			string remainingStr = ((int)totalPoint - this._spentPoints).ToString().SetColor("brightblue");
			this.attributePointText.text = string.Format("{0}/{1}", remainingStr, totalPoint);
			for (int i = 0; i < 6; i++)
			{
				sbyte type = NewGameSubPageCustomPresetAttribute.MainAttributeDisplayOrder[i];
				short value = *this._mainAttributes[(int)type];
				short spentPoints = value;
				bool canAdd = value < maxPoint && currentTotal < (int)totalPoint;
				string addBtnTip = (!canAdd) ? ((value >= maxPoint) ? LanguageKey.LK_NewGame_CustomPreset_AddButton_Disable_Max.Tr() : LanguageKey.LK_NewGame_CustomPreset_AddButton_Disable_NoPoint.Tr()) : null;
				this._attributeItems[i].RefreshItem(NewGameSubPageCustomPresetAttribute.GetMainAttributeIcon(type), NewGameSubPageCustomPresetAttribute.GetMainAttributeName(type), (int)value, (int)spentPoints, canAdd, value > 0, addBtnTip);
			}
		}

		// Token: 0x06006454 RID: 25684 RVA: 0x002DF078 File Offset: 0x002DD278
		private static string GetMainAttributeIcon(sbyte type)
		{
			return "ui9_icon_attribute_major_big_" + type.ToString();
		}

		// Token: 0x06006455 RID: 25685 RVA: 0x002DF09C File Offset: 0x002DD29C
		private static string GetMainAttributeName(sbyte type)
		{
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case 0:
				result = LanguageKey.LK_Main_Attribute_Strength.Tr();
				break;
			case 1:
				result = LanguageKey.LK_Main_Attribute_Dexterity.Tr();
				break;
			case 2:
				result = LanguageKey.LK_Main_Attribute_Concentration.Tr();
				break;
			case 3:
				result = LanguageKey.LK_Main_Attribute_Vitality.Tr();
				break;
			case 4:
				result = LanguageKey.LK_Main_Attribute_Energy.Tr();
				break;
			case 5:
				result = LanguageKey.LK_Main_Attribute_Intelligence.Tr();
				break;
			default:
				result = string.Empty;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x040045FC RID: 17916
		private MainAttributes _mainAttributes;

		// Token: 0x040045FD RID: 17917
		[Header("主属性UI")]
		[SerializeField]
		private TextMeshProUGUI attributePointText;

		// Token: 0x040045FE RID: 17918
		[SerializeField]
		private CButton attributeResetButton;

		// Token: 0x040045FF RID: 17919
		[SerializeField]
		private Transform attributeGrid;

		// Token: 0x04004600 RID: 17920
		[SerializeField]
		private NewGameCustomPresetPointItem attributeItemTemplate;

		// Token: 0x04004601 RID: 17921
		private readonly List<NewGameCustomPresetPointItem> _attributeItems = new List<NewGameCustomPresetPointItem>();

		// Token: 0x04004602 RID: 17922
		private bool _uiInitialized;

		// Token: 0x04004603 RID: 17923
		private static readonly sbyte[] MainAttributeDisplayOrder = new sbyte[]
		{
			0,
			3,
			1,
			4,
			2,
			5
		};

		// Token: 0x04004604 RID: 17924
		private int _spentPoints;
	}
}
