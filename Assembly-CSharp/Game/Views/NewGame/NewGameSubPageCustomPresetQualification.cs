using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.Global;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x02000806 RID: 2054
	public class NewGameSubPageCustomPresetQualification : NewGameSubPageCustomPresetItemBase
	{
		// Token: 0x17000C1B RID: 3099
		// (get) Token: 0x060064A7 RID: 25767 RVA: 0x002E08A0 File Offset: 0x002DEAA0
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
						this.ParentCustomPresetPage.SwitchToSubPage(NewGameSubPageCustomPreset.ESubPageType.Qualification, true, true);
					};
				}
				return result;
			}
		}

		// Token: 0x17000C1C RID: 3100
		// (get) Token: 0x060064A8 RID: 25768 RVA: 0x002E0902 File Offset: 0x002DEB02
		// (set) Token: 0x060064A9 RID: 25769 RVA: 0x002E090A File Offset: 0x002DEB0A
		public override bool StartGameChecked { get; set; }

		// Token: 0x17000C1D RID: 3101
		// (get) Token: 0x060064AA RID: 25770 RVA: 0x002E0913 File Offset: 0x002DEB13
		public override int SpentPoints
		{
			get
			{
				return this._lifeSkillSpentPoints + this._combatSkillSpentPoints;
			}
		}

		// Token: 0x17000C1E RID: 3102
		// (get) Token: 0x060064AB RID: 25771 RVA: 0x002E0922 File Offset: 0x002DEB22
		public override int RemainingPoints
		{
			get
			{
				return (int)(GlobalConfig.Instance.CustomProtagonistLifeSkillQualificationTotalPoint + GlobalConfig.Instance.CustomProtagonistCombatSkillQualificationTotalPoint) - this.SpentPoints;
			}
		}

		// Token: 0x060064AC RID: 25772 RVA: 0x002E0940 File Offset: 0x002DEB40
		private void Awake()
		{
			this.EnsureUiInitialized();
		}

		// Token: 0x060064AD RID: 25773 RVA: 0x002E094A File Offset: 0x002DEB4A
		public override void RefreshUI()
		{
			this.EnsureUiInitialized();
			this.RefreshLifeSkillUI();
			this.RefreshCombatSkillUI();
		}

		// Token: 0x060064AE RID: 25774 RVA: 0x002E0962 File Offset: 0x002DEB62
		public override void ApplyToPreset(CustomProtagonistPresetItem presetItem)
		{
			presetItem.LifeSkillQualifications = this._lifeSkillQualifications;
			presetItem.LifeSkillQualificationGrowthType = this._lifeSkillGrowthType;
			presetItem.CombatSkillQualifications = this._combatSkillQualifications;
			presetItem.CombatSkillQualificationGrowthType = this._combatSkillGrowthType;
		}

		// Token: 0x060064AF RID: 25775 RVA: 0x002E0998 File Offset: 0x002DEB98
		public override void ApplyFromPreset(CustomProtagonistPresetItem presetItem)
		{
			this._lifeSkillQualifications = presetItem.LifeSkillQualifications;
			this._lifeSkillGrowthType = presetItem.LifeSkillQualificationGrowthType;
			this._combatSkillQualifications = presetItem.CombatSkillQualifications;
			this._combatSkillGrowthType = presetItem.CombatSkillQualificationGrowthType;
			this._lifeSkillSpentPoints = this._lifeSkillQualifications.GetSum();
			this._combatSkillSpentPoints = this._combatSkillQualifications.GetSum();
			this.RefreshLifeSkillResetButton();
			this.RefreshCombatSkillResetButton();
		}

		// Token: 0x060064B0 RID: 25776 RVA: 0x002E0A08 File Offset: 0x002DEC08
		public unsafe void ResetLifeSkillQualifications()
		{
			short defaultValue = GlobalConfig.Instance.CustomProtagonistLifeSkillQualificationDefaultPoint;
			bool hasChanged = false;
			for (int i = 0; i < 16; i++)
			{
				sbyte type = (sbyte)i;
				bool flag = *this._lifeSkillQualifications[(int)type] == defaultValue;
				if (!flag)
				{
					*this._lifeSkillQualifications[(int)type] = defaultValue;
					hasChanged = true;
				}
			}
			bool flag2 = !hasChanged;
			if (flag2)
			{
				this.RefreshLifeSkillResetButton();
			}
			else
			{
				int oldSpent = this._lifeSkillSpentPoints;
				this._lifeSkillSpentPoints = this._lifeSkillQualifications.GetSum();
				bool flag3 = oldSpent != this._lifeSkillSpentPoints;
				if (flag3)
				{
					this.ParentCustomPresetPage.OnSubPagePointsChanged();
				}
				base.NotifyDataModified();
				this.RefreshUI();
				this.RefreshLifeSkillResetButton();
			}
		}

		// Token: 0x060064B1 RID: 25777 RVA: 0x002E0AC8 File Offset: 0x002DECC8
		public unsafe void ResetCombatSkillQualifications()
		{
			short defaultValue = GlobalConfig.Instance.CustomProtagonistCombatSkillQualificationDefaultPoint;
			bool hasChanged = false;
			for (int i = 0; i < 14; i++)
			{
				sbyte type = (sbyte)i;
				bool flag = *this._combatSkillQualifications[(int)type] == defaultValue;
				if (!flag)
				{
					*this._combatSkillQualifications[(int)type] = defaultValue;
					hasChanged = true;
				}
			}
			bool flag2 = !hasChanged;
			if (flag2)
			{
				this.RefreshCombatSkillResetButton();
			}
			else
			{
				int oldSpent = this._combatSkillSpentPoints;
				this._combatSkillSpentPoints = this._combatSkillQualifications.GetSum();
				bool flag3 = oldSpent != this._combatSkillSpentPoints;
				if (flag3)
				{
					this.ParentCustomPresetPage.OnSubPagePointsChanged();
				}
				base.NotifyDataModified();
				this.RefreshUI();
				this.RefreshCombatSkillResetButton();
			}
		}

		// Token: 0x060064B2 RID: 25778 RVA: 0x002E0B85 File Offset: 0x002DED85
		public void AddLifeSkillQualification(sbyte type, int delta)
		{
			this.AddLifeSkillQualificationInternal(type, delta, ref this._lifeSkillQualifications, ref this._lifeSkillSpentPoints);
		}

		// Token: 0x060064B3 RID: 25779 RVA: 0x002E0B9D File Offset: 0x002DED9D
		public void AddCombatSkillQualification(sbyte type, int delta)
		{
			this.AddCombatSkillQualificationInternal(type, delta, ref this._combatSkillQualifications, ref this._combatSkillSpentPoints);
		}

		// Token: 0x060064B4 RID: 25780 RVA: 0x002E0BB8 File Offset: 0x002DEDB8
		private void AddLifeSkillQualificationInternal(sbyte type, int delta, ref LifeSkillShorts qualifications, ref int spentPoints)
		{
			bool flag = this.ParentCustomPresetPage == null;
			if (!flag)
			{
				GlobalConfig global = GlobalConfig.Instance;
				int maxPerQualification = (int)global.CustomProtagonistLifeSkillQualificationMaxPoint;
				int totalPoint = (int)global.CustomProtagonistLifeSkillQualificationTotalPoint;
				int currentValue = (int)qualifications.Get((int)type);
				int newValue = Mathf.Clamp(currentValue + delta, 0, maxPerQualification);
				int actualDelta = newValue - currentValue;
				bool flag2 = actualDelta == 0;
				if (!flag2)
				{
					int currentTotal = qualifications.GetSum();
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
						int oldSpent = spentPoints;
						qualifications.Set((int)type, (short)newValue);
						spentPoints = qualifications.GetSum();
						int pointsDelta = spentPoints - oldSpent;
						bool flag5 = pointsDelta != 0;
						if (flag5)
						{
							this.ParentCustomPresetPage.OnSubPagePointsChanged();
						}
						base.NotifyDataModified();
						this.RefreshUI();
						this.RefreshLifeSkillResetButton();
						this.RefreshCombatSkillResetButton();
					}
				}
			}
		}

		// Token: 0x060064B5 RID: 25781 RVA: 0x002E0CA4 File Offset: 0x002DEEA4
		private unsafe void AddCombatSkillQualificationInternal(sbyte type, int delta, ref CombatSkillShorts qualifications, ref int spentPoints)
		{
			bool flag = this.ParentCustomPresetPage == null;
			if (!flag)
			{
				GlobalConfig global = GlobalConfig.Instance;
				int maxPerQualification = (int)global.CustomProtagonistCombatSkillQualificationMaxPoint;
				int totalPoint = (int)global.CustomProtagonistCombatSkillQualificationTotalPoint;
				int currentValue = (int)(*qualifications[(int)type]);
				int newValue = Mathf.Clamp(currentValue + delta, 0, maxPerQualification);
				int actualDelta = newValue - currentValue;
				bool flag2 = actualDelta == 0;
				if (!flag2)
				{
					int currentTotal = qualifications.GetSum();
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
						int oldSpent = spentPoints;
						*qualifications[(int)type] = (short)newValue;
						spentPoints = qualifications.GetSum();
						int pointsDelta = spentPoints - oldSpent;
						bool flag5 = pointsDelta != 0;
						if (flag5)
						{
							this.ParentCustomPresetPage.OnSubPagePointsChanged();
						}
						base.NotifyDataModified();
						this.RefreshUI();
						this.RefreshLifeSkillResetButton();
						this.RefreshCombatSkillResetButton();
					}
				}
			}
		}

		// Token: 0x060064B6 RID: 25782 RVA: 0x002E0D94 File Offset: 0x002DEF94
		public void SetLifeSkillGrowthType(sbyte type)
		{
			bool flag = this._lifeSkillGrowthType == type;
			if (!flag)
			{
				this._lifeSkillGrowthType = type;
				base.NotifyDataModified();
				this.RefreshUI();
				this.RefreshLifeSkillResetButton();
			}
		}

		// Token: 0x060064B7 RID: 25783 RVA: 0x002E0DD0 File Offset: 0x002DEFD0
		public void SetCombatSkillGrowthType(sbyte type)
		{
			bool flag = this._combatSkillGrowthType == type;
			if (!flag)
			{
				this._combatSkillGrowthType = type;
				base.NotifyDataModified();
				this.RefreshUI();
				this.RefreshCombatSkillResetButton();
			}
		}

		// Token: 0x060064B8 RID: 25784 RVA: 0x002E0E0C File Offset: 0x002DF00C
		private static void InitializeGrowthDropdown(CDropdown dropdown, Action<int> onValueChanged)
		{
			dropdown.ClearOptions();
			dropdown.AddOptions(new List<string>
			{
				LanguageKey.LK_Qualification_Growth_Average.Tr(),
				LanguageKey.LK_Qualification_Growth_Precocious.Tr(),
				LanguageKey.LK_Qualification_Growth_LateBlooming.Tr()
			});
			dropdown.onValueChanged.ResetListener(onValueChanged);
		}

		// Token: 0x060064B9 RID: 25785 RVA: 0x002E0E70 File Offset: 0x002DF070
		private void InitializeLifeSkillItems()
		{
			CommonUtils.PrepareEnoughChildren(this.lifeSkillGrid, this.lifeSkillItemTemplate.gameObject, 16, null);
			this._lifeSkillItems.Clear();
			for (int i = 0; i < 16; i++)
			{
				NewGameCustomPresetPointItem item = this.lifeSkillGrid.GetChild(i).GetComponent<NewGameCustomPresetPointItem>();
				item.Initialize((sbyte)i, new Action<sbyte, int>(this.AddLifeSkillQualification));
				this._lifeSkillItems.Add(item);
			}
		}

		// Token: 0x060064BA RID: 25786 RVA: 0x002E0EF4 File Offset: 0x002DF0F4
		private void InitializeCombatSkillItems()
		{
			CommonUtils.PrepareEnoughChildren(this.combatSkillGrid, this.combatSkillItemTemplate.gameObject, 14, null);
			this._combatSkillItems.Clear();
			for (int i = 0; i < 14; i++)
			{
				NewGameCustomPresetPointItem item = this.combatSkillGrid.GetChild(i).GetComponent<NewGameCustomPresetPointItem>();
				item.Initialize((sbyte)i, new Action<sbyte, int>(this.AddCombatSkillQualification));
				this._combatSkillItems.Add(item);
			}
		}

		// Token: 0x060064BB RID: 25787 RVA: 0x002E0F78 File Offset: 0x002DF178
		private unsafe void RefreshLifeSkillResetButton()
		{
			short defaultValue = GlobalConfig.Instance.CustomProtagonistLifeSkillQualificationDefaultPoint;
			for (int i = 0; i < 16; i++)
			{
				bool flag = *this._lifeSkillQualifications[(int)((sbyte)i)] != defaultValue;
				if (flag)
				{
					this.lifeSkillResetButton.interactable = true;
					return;
				}
			}
			this.lifeSkillResetButton.interactable = false;
		}

		// Token: 0x060064BC RID: 25788 RVA: 0x002E0FDC File Offset: 0x002DF1DC
		private unsafe void RefreshCombatSkillResetButton()
		{
			short defaultValue = GlobalConfig.Instance.CustomProtagonistCombatSkillQualificationDefaultPoint;
			for (int i = 0; i < 14; i++)
			{
				bool flag = *this._combatSkillQualifications[(int)((sbyte)i)] != defaultValue;
				if (flag)
				{
					this.combatSkillResetButton.interactable = true;
					return;
				}
			}
			this.combatSkillResetButton.interactable = false;
		}

		// Token: 0x060064BD RID: 25789 RVA: 0x002E1040 File Offset: 0x002DF240
		private void EnsureUiInitialized()
		{
			bool uiInitialized = this._uiInitialized;
			if (!uiInitialized)
			{
				NewGameSubPageCustomPresetQualification.InitializeGrowthDropdown(this.lifeSkillGrowthDropdown, new Action<int>(this.OnLifeSkillGrowthDropdownChanged));
				NewGameSubPageCustomPresetQualification.InitializeGrowthDropdown(this.combatSkillGrowthDropdown, new Action<int>(this.OnCombatSkillGrowthDropdownChanged));
				this.lifeSkillResetButton.onClick.ResetListener(new Action(this.ResetLifeSkillQualifications));
				this.combatSkillResetButton.onClick.ResetListener(new Action(this.ResetCombatSkillQualifications));
				this.InitializeLifeSkillItems();
				this.InitializeCombatSkillItems();
				this._uiInitialized = true;
			}
		}

		// Token: 0x060064BE RID: 25790 RVA: 0x002E10DC File Offset: 0x002DF2DC
		private unsafe void RefreshLifeSkillUI()
		{
			bool flag = this._lifeSkillItems.Count != 16;
			if (flag)
			{
				this.InitializeLifeSkillItems();
			}
			GlobalConfig global = GlobalConfig.Instance;
			short totalPoint = global.CustomProtagonistLifeSkillQualificationTotalPoint;
			int currentTotal = this._lifeSkillQualifications.GetSum();
			short maxPoint = global.CustomProtagonistLifeSkillQualificationMaxPoint;
			string usedStr = ((int)totalPoint - this._lifeSkillSpentPoints).ToString().SetColor("brightblue");
			this.lifeSkillPointText.text = string.Format("{0}/{1}", usedStr, totalPoint);
			this.lifeSkillGrowthDropdown.SetValueWithoutNotify(NewGameSubPageCustomPresetQualification.GetDropdownIndexByGrowthType(this._lifeSkillGrowthType));
			for (int i = 0; i < 16; i++)
			{
				sbyte type = (sbyte)i;
				short value = *this._lifeSkillQualifications[(int)type];
				short spentPoints = value;
				LifeSkillTypeItem config = Config.LifeSkillType.Instance[type];
				bool canAdd = value < maxPoint && currentTotal < (int)totalPoint;
				string addBtnTip = (!canAdd) ? ((value >= maxPoint) ? LanguageKey.LK_NewGame_CustomPreset_AddButton_Disable_Max.Tr() : LanguageKey.LK_NewGame_CustomPreset_AddButton_Disable_NoPoint.Tr()) : null;
				this._lifeSkillItems[i].RefreshItem("ui9_back_lifeskill_small_1_" + config.TemplateId.ToString(), config.Name, (int)value, (int)spentPoints, canAdd, value > 0, addBtnTip);
			}
		}

		// Token: 0x060064BF RID: 25791 RVA: 0x002E1230 File Offset: 0x002DF430
		private unsafe void RefreshCombatSkillUI()
		{
			bool flag = this._combatSkillItems.Count != 14;
			if (flag)
			{
				this.InitializeCombatSkillItems();
			}
			GlobalConfig global = GlobalConfig.Instance;
			short totalPoint = global.CustomProtagonistCombatSkillQualificationTotalPoint;
			int currentTotal = this._combatSkillQualifications.GetSum();
			short maxPoint = global.CustomProtagonistCombatSkillQualificationMaxPoint;
			string usedStr = ((int)totalPoint - this._combatSkillSpentPoints).ToString().SetColor("brightblue");
			this.combatSkillPointText.text = string.Format("{0}/{1}", usedStr, totalPoint);
			this.combatSkillGrowthDropdown.SetValueWithoutNotify(NewGameSubPageCustomPresetQualification.GetDropdownIndexByGrowthType(this._combatSkillGrowthType));
			for (int i = 0; i < 14; i++)
			{
				sbyte type = (sbyte)i;
				short value = *this._combatSkillQualifications[(int)type];
				short spentPoints = value;
				CombatSkillTypeItem config = CombatSkillType.Instance[type];
				bool canAdd = value < maxPoint && currentTotal < (int)totalPoint;
				string addBtnTip = (!canAdd) ? ((value >= maxPoint) ? LanguageKey.LK_NewGame_CustomPreset_AddButton_Disable_Max.Tr() : LanguageKey.LK_NewGame_CustomPreset_AddButton_Disable_NoPoint.Tr()) : null;
				this._combatSkillItems[i].RefreshItem("ui9_back_combatskill_small_1_" + config.TemplateId.ToString(), config.Name, (int)value, (int)spentPoints, canAdd, value > 0, addBtnTip);
			}
		}

		// Token: 0x060064C0 RID: 25792 RVA: 0x002E1384 File Offset: 0x002DF584
		private void OnLifeSkillGrowthDropdownChanged(int index)
		{
			this.SetLifeSkillGrowthType(NewGameSubPageCustomPresetQualification.GetGrowthTypeByDropdownIndex(index));
		}

		// Token: 0x060064C1 RID: 25793 RVA: 0x002E1394 File Offset: 0x002DF594
		private void OnCombatSkillGrowthDropdownChanged(int index)
		{
			this.SetCombatSkillGrowthType(NewGameSubPageCustomPresetQualification.GetGrowthTypeByDropdownIndex(index));
		}

		// Token: 0x060064C2 RID: 25794 RVA: 0x002E13A4 File Offset: 0x002DF5A4
		private static int GetDropdownIndexByGrowthType(sbyte growthType)
		{
			for (int i = 0; i < NewGameSubPageCustomPresetQualification.GrowthTypes.Length; i++)
			{
				bool flag = NewGameSubPageCustomPresetQualification.GrowthTypes[i] == growthType;
				if (flag)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x060064C3 RID: 25795 RVA: 0x002E13E4 File Offset: 0x002DF5E4
		private static sbyte GetGrowthTypeByDropdownIndex(int index)
		{
			bool flag = index >= 0 && index < NewGameSubPageCustomPresetQualification.GrowthTypes.Length;
			sbyte result;
			if (flag)
			{
				result = NewGameSubPageCustomPresetQualification.GrowthTypes[index];
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x04004620 RID: 17952
		private LifeSkillShorts _lifeSkillQualifications;

		// Token: 0x04004621 RID: 17953
		private sbyte _lifeSkillGrowthType = 0;

		// Token: 0x04004622 RID: 17954
		private CombatSkillShorts _combatSkillQualifications;

		// Token: 0x04004623 RID: 17955
		private sbyte _combatSkillGrowthType = 0;

		// Token: 0x04004624 RID: 17956
		[Header("技艺资质UI")]
		[SerializeField]
		private CDropdown lifeSkillGrowthDropdown;

		// Token: 0x04004625 RID: 17957
		[SerializeField]
		private TextMeshProUGUI lifeSkillPointText;

		// Token: 0x04004626 RID: 17958
		[SerializeField]
		private CButton lifeSkillResetButton;

		// Token: 0x04004627 RID: 17959
		[SerializeField]
		private Transform lifeSkillGrid;

		// Token: 0x04004628 RID: 17960
		[SerializeField]
		private NewGameCustomPresetPointItem lifeSkillItemTemplate;

		// Token: 0x04004629 RID: 17961
		[Header("功法资质UI")]
		[SerializeField]
		private CDropdown combatSkillGrowthDropdown;

		// Token: 0x0400462A RID: 17962
		[SerializeField]
		private TextMeshProUGUI combatSkillPointText;

		// Token: 0x0400462B RID: 17963
		[SerializeField]
		private CButton combatSkillResetButton;

		// Token: 0x0400462C RID: 17964
		[SerializeField]
		private Transform combatSkillGrid;

		// Token: 0x0400462D RID: 17965
		[SerializeField]
		private NewGameCustomPresetPointItem combatSkillItemTemplate;

		// Token: 0x0400462E RID: 17966
		private int _lifeSkillSpentPoints;

		// Token: 0x0400462F RID: 17967
		private int _combatSkillSpentPoints;

		// Token: 0x04004630 RID: 17968
		private readonly List<NewGameCustomPresetPointItem> _lifeSkillItems = new List<NewGameCustomPresetPointItem>();

		// Token: 0x04004631 RID: 17969
		private readonly List<NewGameCustomPresetPointItem> _combatSkillItems = new List<NewGameCustomPresetPointItem>();

		// Token: 0x04004632 RID: 17970
		private bool _uiInitialized;

		// Token: 0x04004633 RID: 17971
		private static readonly sbyte[] GrowthTypes = new sbyte[]
		{
			0,
			1,
			2
		};
	}
}
