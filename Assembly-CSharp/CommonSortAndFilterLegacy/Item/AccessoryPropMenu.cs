using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004A7 RID: 1191
	public class AccessoryPropMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x0600417C RID: 16764 RVA: 0x002019BE File Offset: 0x001FFBBE
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x0600417D RID: 16765 RVA: 0x002019C1 File Offset: 0x001FFBC1
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600417E RID: 16766 RVA: 0x002019C4 File Offset: 0x001FFBC4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Effect);
		}

		// Token: 0x0600417F RID: 16767 RVA: 0x002019E8 File Offset: 0x001FFBE8
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			foreach (object propType in Enum.GetValues(typeof(EAccessoryPropType)))
			{
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Item_Third_Accessory_{0}", (int)propType))
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x06004180 RID: 16768 RVA: 0x00201A90 File Offset: 0x001FFC90
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			AccessoryItem accessoryConfig = Accessory.Instance[data.Key.TemplateId];
			using (IEnumerator<int> enumerator = selectedIndices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case 0:
					{
						bool flag = AccessoryMainPropMenu.CheckAnyOptionValidStatic(accessoryConfig);
						if (flag)
						{
							return true;
						}
						break;
					}
					case 1:
					{
						bool flag2 = AccessorySecondaryPropMenu.CheckAnyOptionValidStatic(accessoryConfig);
						if (flag2)
						{
							return true;
						}
						break;
					}
					case 2:
					{
						bool flag3 = AccessoryAttackPropMenu.CheckAnyOptionValidStatic(accessoryConfig);
						if (flag3)
						{
							return true;
						}
						break;
					}
					case 3:
					{
						bool flag4 = AccessoryDefPropMenu.CheckAnyOptionValidStatic(accessoryConfig);
						if (flag4)
						{
							return true;
						}
						break;
					}
					case 4:
					{
						bool flag5 = AccessoryPosionPropMenu.CheckAnyOptionValidStatic(accessoryConfig);
						if (flag5)
						{
							return true;
						}
						break;
					}
					case 5:
					{
						bool flag6 = accessoryConfig.BonusCombatSkillSect > 0;
						if (flag6)
						{
							return true;
						}
						break;
					}
					case 6:
					{
						bool flag7 = accessoryConfig.CombatSkillAddMaxPower > 0;
						if (flag7)
						{
							return true;
						}
						break;
					}
					case 7:
					{
						bool flag8 = accessoryConfig.MaxInventoryLoadBonus > 0;
						if (flag8)
						{
							return true;
						}
						break;
					}
					case 8:
					{
						bool flag9 = accessoryConfig.DropRateBonus > 0;
						if (flag9)
						{
							return true;
						}
						break;
					}
					case 9:
					{
						bool flag10 = accessoryConfig.BaseCaptureRateBonus > 0;
						if (flag10)
						{
							return true;
						}
						break;
					}
					}
				}
			}
			return false;
		}
	}
}
