using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x020004C8 RID: 1224
	public class CarrierSubTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000756 RID: 1878
		// (get) Token: 0x06004211 RID: 16913 RVA: 0x002031EE File Offset: 0x002013EE
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004212 RID: 16914 RVA: 0x002031F4 File Offset: 0x002013F4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x06004213 RID: 16915 RVA: 0x00203218 File Offset: 0x00201418
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> dropdownConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (int i = 0; i < CarrierSubTypeMenu.SubTypeList.Count; i++)
			{
				dropdownConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = "",
					Text = StringKey.CreateKey(string.Format("LK_Mousetip_Carrier_{0}", i))
				});
			}
			return dropdownConfigs;
		}

		// Token: 0x06004214 RID: 16916 RVA: 0x00203284 File Offset: 0x00201484
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			CarrierItem carrierConfig = Carrier.Instance[data.Key.TemplateId];
			foreach (int item in selectedIndices)
			{
				int num = item;
				int num2 = num;
				if (num2 != 0)
				{
					if (num2 != 1)
					{
						bool flag = carrierConfig.ItemSubType >= 402 && carrierConfig.ItemSubType <= 404;
						if (flag)
						{
							return true;
						}
					}
					else
					{
						bool flag2 = carrierConfig.ItemSubType == 401;
						if (flag2)
						{
							return true;
						}
					}
				}
				else
				{
					bool flag3 = carrierConfig.ItemSubType == 400;
					if (flag3)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x17000757 RID: 1879
		// (get) Token: 0x06004215 RID: 16917 RVA: 0x0020335C File Offset: 0x0020155C
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x04002EC8 RID: 11976
		private static readonly List<short> SubTypeList = new List<short>
		{
			400,
			401,
			402
		};
	}
}
