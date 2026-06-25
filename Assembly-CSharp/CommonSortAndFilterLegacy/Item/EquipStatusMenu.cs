using System;
using System.Collections.Generic;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004EA RID: 1258
	public class EquipStatusMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x06004298 RID: 17048 RVA: 0x00204863 File Offset: 0x00202A63
		public EquipStatusMenu(int id, List<EEquipStatusMenuId> filterTypes)
		{
			this._id = id;
			this._filterTypes = filterTypes;
		}

		// Token: 0x17000792 RID: 1938
		// (get) Token: 0x06004299 RID: 17049 RVA: 0x0020487B File Offset: 0x00202A7B
		public override int Id
		{
			get
			{
				return this._id;
			}
		}

		// Token: 0x0600429A RID: 17050 RVA: 0x00204884 File Offset: 0x00202A84
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Status);
		}

		// Token: 0x0600429B RID: 17051 RVA: 0x002048A8 File Offset: 0x00202AA8
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			foreach (EEquipStatusMenuId item in this._filterTypes)
			{
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = LocalStringManager.Get(string.Format("LK_CommonSortAndFilter_Filter_Item_Third_Title_Status_{0}", (int)item))
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x0600429C RID: 17052 RVA: 0x00204944 File Offset: 0x00202B44
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			bool flag = selectedIndices == null || selectedIndices.Count == 0;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				foreach (int index in selectedIndices)
				{
					switch (this._filterTypes[index])
					{
					case EEquipStatusMenuId.Equiped:
					{
						bool flag2 = data.UsingType == ItemDisplayData.ItemUsingType.Equiped;
						if (flag2)
						{
							return true;
						}
						break;
					}
					case EEquipStatusMenuId.Preset:
					{
						bool flag3 = data.UsingType == ItemDisplayData.ItemUsingType.EquipmentPlaned;
						if (flag3)
						{
							return true;
						}
						break;
					}
					case EEquipStatusMenuId.NotEquiped:
					{
						bool flag4 = data.UsingType != ItemDisplayData.ItemUsingType.Equiped;
						if (flag4)
						{
							return true;
						}
						break;
					}
					case EEquipStatusMenuId.Refine:
					{
						bool isRefined = data.RefiningEffects.IsRefined;
						if (isRefined)
						{
							return true;
						}
						break;
					}
					case EEquipStatusMenuId.Position:
					{
						bool flag5 = data.PoisonEffects != null && data.PoisonEffects.IsValid;
						if (flag5)
						{
							return true;
						}
						break;
					}
					case EEquipStatusMenuId.HasSpecialEffect:
					{
						bool flag6 = data.EquipmentEffectIds != null && data.EquipmentEffectIds.Count > 0;
						if (flag6)
						{
							return true;
						}
						break;
					}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x17000793 RID: 1939
		// (get) Token: 0x0600429D RID: 17053 RVA: 0x00204A94 File Offset: 0x00202C94
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x04002F07 RID: 12039
		private int _id;

		// Token: 0x04002F08 RID: 12040
		private List<EEquipStatusMenuId> _filterTypes;
	}
}
