using System;
using System.Collections.Generic;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000D7D RID: 3453
	public class EquipStatusMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x0600A8A8 RID: 43176 RVA: 0x004E1377 File Offset: 0x004DF577
		public EquipStatusMenu(int id, List<EquipStatusMenu.EEquipStatusMenuId> filterTypes)
		{
			this._id = id;
			this._filterTypes = filterTypes;
		}

		// Token: 0x17001233 RID: 4659
		// (get) Token: 0x0600A8A9 RID: 43177 RVA: 0x004E138F File Offset: 0x004DF58F
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001234 RID: 4660
		// (get) Token: 0x0600A8AA RID: 43178 RVA: 0x004E1392 File Offset: 0x004DF592
		public override int Id
		{
			get
			{
				return this._id;
			}
		}

		// Token: 0x0600A8AB RID: 43179 RVA: 0x004E139C File Offset: 0x004DF59C
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Status;
		}

		// Token: 0x0600A8AC RID: 43180 RVA: 0x004E13B8 File Offset: 0x004DF5B8
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			foreach (EquipStatusMenu.EEquipStatusMenuId item in this._filterTypes)
			{
				configs.Add(new FilterDropdownItemConfig
				{
					Text = LocalStringManager.Get(string.Format("LK_CommonSortAndFilter_Filter_Item_Third_Title_Status_{0}", (int)item))
				});
			}
			return configs;
		}

		// Token: 0x0600A8AD RID: 43181 RVA: 0x004E1448 File Offset: 0x004DF648
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
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
					case EquipStatusMenu.EEquipStatusMenuId.Equiped:
					{
						bool flag2 = data.UsingType == ItemDisplayData.ItemUsingType.Equiped;
						if (flag2)
						{
							return true;
						}
						break;
					}
					case EquipStatusMenu.EEquipStatusMenuId.Preset:
					{
						bool flag3 = data.UsingType == ItemDisplayData.ItemUsingType.EquipmentPlaned;
						if (flag3)
						{
							return true;
						}
						break;
					}
					case EquipStatusMenu.EEquipStatusMenuId.NotEquiped:
					{
						bool flag4 = data.UsingType != ItemDisplayData.ItemUsingType.Equiped;
						if (flag4)
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

		// Token: 0x040083EE RID: 33774
		private int _id;

		// Token: 0x040083EF RID: 33775
		private List<EquipStatusMenu.EEquipStatusMenuId> _filterTypes;

		// Token: 0x02002471 RID: 9329
		public enum EEquipStatusMenuId
		{
			// Token: 0x0400E437 RID: 58423
			Equiped,
			// Token: 0x0400E438 RID: 58424
			Preset,
			// Token: 0x0400E439 RID: 58425
			NotEquiped,
			// Token: 0x0400E43A RID: 58426
			Refine,
			// Token: 0x0400E43B RID: 58427
			Position,
			// Token: 0x0400E43C RID: 58428
			HasSpecialEffect
		}
	}
}
