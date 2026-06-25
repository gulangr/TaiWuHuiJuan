using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Item
{
	// Token: 0x02000A11 RID: 2577
	public class ItemAutoOperationItem : MonoBehaviour
	{
		// Token: 0x06007DF7 RID: 32247 RVA: 0x003A732C File Offset: 0x003A552C
		public void Init(ItemAutoOperationSettingItem data)
		{
			this.buttonReset.ClearAndAddListener(new Action(this.OnClickButtonReset));
			this.dropdownGrade.ClearOptions();
			List<string> optionList = new List<string>();
			for (int i = 0; i < 9; i++)
			{
				LanguageKey key = LanguageKey.LK_Grade_0 + i;
				optionList.Add(key.Tr().SetGradeColor(i));
			}
			this.dropdownGrade.AddOptions(optionList);
			this.dropdownGrade.onValueChanged.RemoveAllListeners();
			this.dropdownGrade.onValueChanged.AddListener(new UnityAction<int>(this.DropdownGradeOnValueChanged));
			this._itemData = data;
			this.textTitle.text = ItemAutoOperationItem.GetTypeName(data.TargetType);
			this.dropdownGrade.SetValueWithoutNotify((int)data.Grade);
			this.InitToggleGroupFilter();
		}

		// Token: 0x06007DF8 RID: 32248 RVA: 0x003A7404 File Offset: 0x003A5604
		private void OnClickButtonReset()
		{
			this._itemData.Reset();
			this.Init(this._itemData);
		}

		// Token: 0x06007DF9 RID: 32249 RVA: 0x003A7420 File Offset: 0x003A5620
		private void DropdownGradeOnValueChanged(int value)
		{
			this._itemData.Grade = (sbyte)value;
		}

		// Token: 0x06007DFA RID: 32250 RVA: 0x003A7430 File Offset: 0x003A5630
		private void InitToggleGroupFilter()
		{
			this.toggleGroupFilter.Clear();
			int typeCount = ItemAutoOperationSettingItem.GetSubTypeCount(this._itemData.TargetType);
			int count = typeCount + 1;
			GameObject template = this.toggleGroupFilter.transform.GetChild(0).gameObject;
			sbyte index = 0;
			while ((int)index < count)
			{
				Transform child = ((int)index < this.toggleGroupFilter.transform.childCount) ? this.toggleGroupFilter.transform.GetChild((int)index) : Object.Instantiate<GameObject>(template, this.toggleGroupFilter.transform).transform;
				child.gameObject.SetActive(true);
				CToggle item = child.GetComponent<CToggle>();
				bool isAll = index == 0;
				sbyte typeIndex = Convert.ToSByte((int)(index - 1));
				string subTypeName = isAll ? LanguageKey.LK_Common_All.Tr() : ItemAutoOperationItem.GetSubTypeName(this._itemData.TargetType, (int)typeIndex);
				TextMeshProUGUI componentInChildren = item.GetComponentInChildren<TextMeshProUGUI>();
				if (componentInChildren != null)
				{
					componentInChildren.SetText(subTypeName, true);
				}
				bool isOn = isAll ? (this._itemData.SubtypeList.Count == typeCount) : this._itemData.SubtypeList.Contains(typeIndex);
				item.SetIsOnWithoutNotify(isOn);
				this.toggleGroupFilter.Add(item);
				index += 1;
			}
			for (int i = count; i < this.toggleGroupFilter.transform.childCount; i++)
			{
				this.toggleGroupFilter.transform.GetChild(i).gameObject.SetActive(false);
			}
			this.toggleGroupFilter.Init();
			this.toggleGroupFilter.OnActiveIndexChange -= this.ToggleGroupFilterOnActiveIndexChange;
			this.toggleGroupFilter.OnActiveIndexChange += this.ToggleGroupFilterOnActiveIndexChange;
		}

		// Token: 0x06007DFB RID: 32251 RVA: 0x003A75F4 File Offset: 0x003A57F4
		private void ToggleGroupFilterOnActiveIndexChange(int newIndex, int oldIndex)
		{
			bool flag = newIndex == 0;
			if (flag)
			{
				this.toggleGroupFilter.SelectAll(false);
			}
			else
			{
				bool flag2 = oldIndex == 0;
				if (flag2)
				{
					this.toggleGroupFilter.DeSelectAll(false);
				}
				else
				{
					sbyte newType = Convert.ToSByte(newIndex - 1);
					sbyte oldType = Convert.ToSByte(oldIndex - 1);
					bool flag3 = !this._itemData.SubtypeList.Contains(newType);
					if (flag3)
					{
						this._itemData.SubtypeList.Add(newType);
					}
					else
					{
						bool flag4 = this._itemData.SubtypeList.Contains(oldType);
						if (flag4)
						{
							this._itemData.SubtypeList.Remove(oldType);
						}
					}
					int typeCount = ItemAutoOperationSettingItem.GetSubTypeCount(this._itemData.TargetType);
					this.toggleGroupFilter.Get(0).SetIsOnWithoutNotify(this._itemData.SubtypeList.Count == typeCount);
				}
			}
		}

		// Token: 0x06007DFC RID: 32252 RVA: 0x003A76DC File Offset: 0x003A58DC
		private static string GetTypeName(EItemAutoOperationTargetType type)
		{
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case EItemAutoOperationTargetType.Food:
				result = LanguageKey.LK_ItemType_7.Tr();
				break;
			case EItemAutoOperationTargetType.Medicine:
				result = LanguageKey.LK_ItemType_8.Tr();
				break;
			case EItemAutoOperationTargetType.Weapon:
				result = LanguageKey.LK_ItemType_0.Tr();
				break;
			case EItemAutoOperationTargetType.OtherEquipment:
				result = LanguageKey.LK_CharacterMenu_Tog_Equip.Tr();
				break;
			case EItemAutoOperationTargetType.Book:
				result = LanguageKey.LK_ItemType_10.Tr();
				break;
			case EItemAutoOperationTargetType.Tool:
				result = LanguageKey.LK_ItemType_6.Tr();
				break;
			case EItemAutoOperationTargetType.Material:
				result = LanguageKey.LK_ItemType_5.Tr();
				break;
			case EItemAutoOperationTargetType.RefineMaterial:
				result = LanguageKey.LK_Refine_Material.Tr();
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007DFD RID: 32253 RVA: 0x003A7794 File Offset: 0x003A5994
		private static string GetSubTypeName(EItemAutoOperationTargetType type, int index)
		{
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case EItemAutoOperationTargetType.Food:
				result = ItemAutoOperationItem.GetFoodSubTypeName((EItemAutoOperationFoodSubtype)index);
				break;
			case EItemAutoOperationTargetType.Medicine:
				result = ItemAutoOperationItem.GetMedicineSubTypeName((EItemAutoOperationMedicineSubtype)index);
				break;
			case EItemAutoOperationTargetType.Weapon:
				result = ItemAutoOperationItem.GetWeaponSubTypeName((EItemAutoOperationWeaponSubtype)index);
				break;
			case EItemAutoOperationTargetType.OtherEquipment:
				result = ItemAutoOperationItem.GetOtherEquipmentSubTypeName((EItemAutoOperationOtherEquipmentSubtype)index);
				break;
			case EItemAutoOperationTargetType.Book:
				result = ItemAutoOperationItem.GetBookSubTypeName((EItemAutoOperationBookSubtype)index);
				break;
			case EItemAutoOperationTargetType.Tool:
				result = ItemAutoOperationItem.GetToolSubTypeName((EItemAutoOperationToolSubtype)index);
				break;
			case EItemAutoOperationTargetType.Material:
				result = ItemAutoOperationItem.GetMaterialSubTypeName((EItemAutoOperationMaterialSubtype)index);
				break;
			case EItemAutoOperationTargetType.RefineMaterial:
				result = ItemAutoOperationItem.GetRefineMaterialSubTypeName((EItemAutoOperationRefineMaterialSubtype)index);
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007DFE RID: 32254 RVA: 0x003A782C File Offset: 0x003A5A2C
		private static string GetFoodSubTypeName(EItemAutoOperationFoodSubtype subtype)
		{
			if (!true)
			{
			}
			string result;
			switch (subtype)
			{
			case EItemAutoOperationFoodSubtype.VegetarianFood:
				result = LanguageKey.LK_ItemSubType_700.Tr();
				break;
			case EItemAutoOperationFoodSubtype.MeatFood:
				result = LanguageKey.LK_ItemSubType_701.Tr();
				break;
			case EItemAutoOperationFoodSubtype.Tea:
				result = LanguageKey.LK_ItemSubType_900.Tr();
				break;
			case EItemAutoOperationFoodSubtype.Wine:
				result = LanguageKey.LK_ItemSubType_901.Tr();
				break;
			default:
				throw new ArgumentOutOfRangeException("subtype", subtype, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007DFF RID: 32255 RVA: 0x003A78A0 File Offset: 0x003A5AA0
		private static string GetMedicineSubTypeName(EItemAutoOperationMedicineSubtype subtype)
		{
			if (!true)
			{
			}
			string result;
			switch (subtype)
			{
			case EItemAutoOperationMedicineSubtype.Outer:
				result = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_0.Tr();
				break;
			case EItemAutoOperationMedicineSubtype.Inner:
				result = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_1.Tr();
				break;
			case EItemAutoOperationMedicineSubtype.Detox:
				result = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_2.Tr();
				break;
			case EItemAutoOperationMedicineSubtype.Poison:
				result = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_3.Tr();
				break;
			case EItemAutoOperationMedicineSubtype.Disorder:
				result = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_4.Tr();
				break;
			case EItemAutoOperationMedicineSubtype.Health:
				result = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_5.Tr();
				break;
			case EItemAutoOperationMedicineSubtype.Buff:
				result = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_6.Tr();
				break;
			case EItemAutoOperationMedicineSubtype.Other:
				result = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Medicine_7.Tr();
				break;
			default:
				throw new ArgumentOutOfRangeException("subtype", subtype, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007E00 RID: 32256 RVA: 0x003A7958 File Offset: 0x003A5B58
		private static string GetWeaponSubTypeName(EItemAutoOperationWeaponSubtype subtype)
		{
			if (!true)
			{
			}
			string result;
			switch (subtype)
			{
			case EItemAutoOperationWeaponSubtype.NeedleBox:
				result = LanguageKey.LK_ItemSubType_0.Tr();
				break;
			case EItemAutoOperationWeaponSubtype.DoubleDaggers:
				result = LanguageKey.LK_ItemSubType_1.Tr();
				break;
			case EItemAutoOperationWeaponSubtype.Hidden:
				result = LanguageKey.LK_ItemSubType_2.Tr();
				break;
			case EItemAutoOperationWeaponSubtype.Flute:
				result = LanguageKey.LK_ItemSubType_3.Tr();
				break;
			case EItemAutoOperationWeaponSubtype.Gloves:
				result = LanguageKey.LK_ItemSubType_4.Tr();
				break;
			case EItemAutoOperationWeaponSubtype.Pestle:
				result = LanguageKey.LK_ItemSubType_5.Tr();
				break;
			case EItemAutoOperationWeaponSubtype.Whisk:
				result = LanguageKey.LK_ItemSubType_6.Tr();
				break;
			case EItemAutoOperationWeaponSubtype.Whip:
				result = LanguageKey.LK_ItemSubType_7.Tr();
				break;
			case EItemAutoOperationWeaponSubtype.Sword:
				result = LanguageKey.LK_ItemSubType_8.Tr();
				break;
			case EItemAutoOperationWeaponSubtype.Blade:
				result = LanguageKey.LK_ItemSubType_9.Tr();
				break;
			case EItemAutoOperationWeaponSubtype.Polearm:
				result = LanguageKey.LK_ItemSubType_10.Tr();
				break;
			case EItemAutoOperationWeaponSubtype.Zither:
				result = LanguageKey.LK_ItemSubType_11.Tr();
				break;
			case EItemAutoOperationWeaponSubtype.MechanicWeapon:
				result = LanguageKey.LK_ItemSubType_12.Tr();
				break;
			case EItemAutoOperationWeaponSubtype.MagicSymbol:
				result = LanguageKey.LK_ItemSubType_13.Tr();
				break;
			case EItemAutoOperationWeaponSubtype.PoisonWeaponCream:
				result = LanguageKey.LK_ItemSubType_14.Tr();
				break;
			case EItemAutoOperationWeaponSubtype.PoisonWeaponSand:
				result = LanguageKey.LK_ItemSubType_15.Tr();
				break;
			default:
				throw new ArgumentOutOfRangeException("subtype", subtype, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007E01 RID: 32257 RVA: 0x003A7AB0 File Offset: 0x003A5CB0
		private static string GetOtherEquipmentSubTypeName(EItemAutoOperationOtherEquipmentSubtype subtype)
		{
			if (!true)
			{
			}
			string result;
			switch (subtype)
			{
			case EItemAutoOperationOtherEquipmentSubtype.Helm:
				result = LanguageKey.LK_ItemSubType_100.Tr();
				break;
			case EItemAutoOperationOtherEquipmentSubtype.TorsoArmor:
				result = LanguageKey.LK_ItemSubType_101.Tr();
				break;
			case EItemAutoOperationOtherEquipmentSubtype.Bracers:
				result = LanguageKey.LK_ItemSubType_102.Tr();
				break;
			case EItemAutoOperationOtherEquipmentSubtype.Boots:
				result = LanguageKey.LK_ItemSubType_103.Tr();
				break;
			case EItemAutoOperationOtherEquipmentSubtype.AnimalArmor:
				result = LanguageKey.LK_ItemSubType_104.Tr();
				break;
			case EItemAutoOperationOtherEquipmentSubtype.Accessory:
				result = LanguageKey.LK_ItemSubType_200.Tr();
				break;
			case EItemAutoOperationOtherEquipmentSubtype.Pocket:
				result = LanguageKey.LK_ItemSubType_201.Tr();
				break;
			case EItemAutoOperationOtherEquipmentSubtype.Clothing:
				result = LanguageKey.LK_ItemType_3.Tr();
				break;
			case EItemAutoOperationOtherEquipmentSubtype.Carrier:
				result = LanguageKey.LK_ItemType_4.Tr();
				break;
			default:
				throw new ArgumentOutOfRangeException("subtype", subtype, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007E02 RID: 32258 RVA: 0x003A7B7C File Offset: 0x003A5D7C
		private static string GetBookSubTypeName(EItemAutoOperationBookSubtype subtype)
		{
			if (!true)
			{
			}
			string result;
			switch (subtype)
			{
			case EItemAutoOperationBookSubtype.ReadFinished:
				result = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_BookReasState_0.Tr();
				break;
			case EItemAutoOperationBookSubtype.Reading:
				result = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_BookReasState_1.Tr();
				break;
			case EItemAutoOperationBookSubtype.NotReading:
				result = LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_BookReasState_2.Tr();
				break;
			default:
				throw new ArgumentOutOfRangeException("subtype", subtype, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007E03 RID: 32259 RVA: 0x003A7BE0 File Offset: 0x003A5DE0
		private static string GetToolSubTypeName(EItemAutoOperationToolSubtype subtype)
		{
			if (!true)
			{
			}
			string result;
			switch (subtype)
			{
			case EItemAutoOperationToolSubtype.Forging:
				result = LanguageKey.LK_LifeSkillType_6.Tr();
				break;
			case EItemAutoOperationToolSubtype.Woodworking:
				result = LanguageKey.LK_LifeSkillType_7.Tr();
				break;
			case EItemAutoOperationToolSubtype.Medicine:
				result = LanguageKey.LK_LifeSkillType_8.Tr();
				break;
			case EItemAutoOperationToolSubtype.Toxicology:
				result = LanguageKey.LK_LifeSkillType_9.Tr();
				break;
			case EItemAutoOperationToolSubtype.Weaving:
				result = LanguageKey.LK_LifeSkillType_10.Tr();
				break;
			case EItemAutoOperationToolSubtype.Jade:
				result = LanguageKey.LK_LifeSkillType_11.Tr();
				break;
			case EItemAutoOperationToolSubtype.Cooking:
				result = LanguageKey.LK_LifeSkillType_14.Tr();
				break;
			default:
				throw new ArgumentOutOfRangeException("subtype", subtype, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007E04 RID: 32260 RVA: 0x003A7C88 File Offset: 0x003A5E88
		private static string GetMaterialSubTypeName(EItemAutoOperationMaterialSubtype subtype)
		{
			if (!true)
			{
			}
			string result;
			switch (subtype)
			{
			case EItemAutoOperationMaterialSubtype.Food:
				result = LanguageKey.LK_ItemSubType_500.Tr();
				break;
			case EItemAutoOperationMaterialSubtype.Wood:
				result = LanguageKey.LK_ItemSubType_501.Tr();
				break;
			case EItemAutoOperationMaterialSubtype.Metal:
				result = LanguageKey.LK_ItemSubType_502.Tr();
				break;
			case EItemAutoOperationMaterialSubtype.Jade:
				result = LanguageKey.LK_ItemSubType_503.Tr();
				break;
			case EItemAutoOperationMaterialSubtype.Fabric:
				result = LanguageKey.LK_ItemSubType_504.Tr();
				break;
			case EItemAutoOperationMaterialSubtype.Herb:
				result = LanguageKey.LK_ItemSubType_505.Tr();
				break;
			case EItemAutoOperationMaterialSubtype.Poison:
				result = LanguageKey.LK_ItemSubType_506.Tr();
				break;
			default:
				throw new ArgumentOutOfRangeException("subtype", subtype, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007E05 RID: 32261 RVA: 0x003A7D30 File Offset: 0x003A5F30
		private static string GetRefineMaterialSubTypeName(EItemAutoOperationRefineMaterialSubtype subtype)
		{
			if (!true)
			{
			}
			string result;
			switch (subtype)
			{
			case EItemAutoOperationRefineMaterialSubtype.Wood:
				result = LanguageKey.LK_ItemSubType_501.Tr();
				break;
			case EItemAutoOperationRefineMaterialSubtype.Metal:
				result = LanguageKey.LK_ItemSubType_502.Tr();
				break;
			case EItemAutoOperationRefineMaterialSubtype.Jade:
				result = LanguageKey.LK_ItemSubType_503.Tr();
				break;
			case EItemAutoOperationRefineMaterialSubtype.Fabric:
				result = LanguageKey.LK_ItemSubType_504.Tr();
				break;
			default:
				throw new ArgumentOutOfRangeException("subtype", subtype, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0400601C RID: 24604
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x0400601D RID: 24605
		[SerializeField]
		private CButton buttonReset;

		// Token: 0x0400601E RID: 24606
		[SerializeField]
		private CDropdown dropdownGrade;

		// Token: 0x0400601F RID: 24607
		[SerializeField]
		private CToggleGroupMultiSelect toggleGroupFilter;

		// Token: 0x04006020 RID: 24608
		private ItemAutoOperationSettingItem _itemData;
	}
}
