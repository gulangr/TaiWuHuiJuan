using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x02000559 RID: 1369
	internal class MiscWesternPresentTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000814 RID: 2068
		// (get) Token: 0x06004417 RID: 17431 RVA: 0x00208E9F File Offset: 0x0020709F
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x06004418 RID: 17432 RVA: 0x00208EA4 File Offset: 0x002070A4
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x06004419 RID: 17433 RVA: 0x00208EC8 File Offset: 0x002070C8
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> menuConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (MiscWesternPresentTypeMenu.EMenuKeys t = MiscWesternPresentTypeMenu.EMenuKeys.Monv; t < MiscWesternPresentTypeMenu.EMenuKeys.Count; t++)
			{
				StringKey key = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Item_Third_MiscWesternPresent_{0}", (int)t));
				menuConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = key
				});
			}
			return menuConfigs;
		}

		// Token: 0x0600441A RID: 17434 RVA: 0x00208F30 File Offset: 0x00207130
		public override bool IsDataMatch(ItemDisplayData data, IReadOnlyCollection<int> selectedIndices)
		{
			MiscItem miscItem = Misc.Instance[data.Key.TemplateId];
			using (IEnumerator<int> enumerator = selectedIndices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case 0:
					{
						bool flag = MiscWesternPresentTypeMenu.IsMonv(miscItem);
						if (flag)
						{
							return true;
						}
						break;
					}
					case 1:
					{
						bool flag2 = MiscWesternPresentTypeMenu.IsDayueYaochang(miscItem);
						if (flag2)
						{
							return true;
						}
						break;
					}
					case 2:
					{
						bool flag3 = MiscWesternPresentTypeMenu.IsJiuhan(miscItem);
						if (flag3)
						{
							return true;
						}
						break;
					}
					case 3:
					{
						bool flag4 = MiscWesternPresentTypeMenu.IsJinHuanger(miscItem);
						if (flag4)
						{
							return true;
						}
						break;
					}
					case 4:
					{
						bool flag5 = MiscWesternPresentTypeMenu.IsYiYihou(miscItem);
						if (flag5)
						{
							return true;
						}
						break;
					}
					case 5:
					{
						bool flag6 = MiscWesternPresentTypeMenu.IsWeiQi(miscItem);
						if (flag6)
						{
							return true;
						}
						break;
					}
					case 6:
					{
						bool flag7 = MiscWesternPresentTypeMenu.IsYixiang(miscItem);
						if (flag7)
						{
							return true;
						}
						break;
					}
					case 7:
					{
						bool flag8 = MiscWesternPresentTypeMenu.IsXuefeng(miscItem);
						if (flag8)
						{
							return true;
						}
						break;
					}
					case 8:
					{
						bool flag9 = MiscWesternPresentTypeMenu.IsShuFang(miscItem);
						if (flag9)
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

		// Token: 0x17000815 RID: 2069
		// (get) Token: 0x0600441B RID: 17435 RVA: 0x00209094 File Offset: 0x00207294
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600441C RID: 17436 RVA: 0x00209097 File Offset: 0x00207297
		private static bool IsMonv(MiscItem miscItem)
		{
			return miscItem.GroupId == 37;
		}

		// Token: 0x0600441D RID: 17437 RVA: 0x002090A3 File Offset: 0x002072A3
		private static bool IsDayueYaochang(MiscItem miscItem)
		{
			return miscItem.GroupId == 42;
		}

		// Token: 0x0600441E RID: 17438 RVA: 0x002090AF File Offset: 0x002072AF
		private static bool IsJiuhan(MiscItem miscItem)
		{
			return miscItem.GroupId == 47;
		}

		// Token: 0x0600441F RID: 17439 RVA: 0x002090BB File Offset: 0x002072BB
		private static bool IsJinHuanger(MiscItem miscItem)
		{
			return miscItem.GroupId == 52;
		}

		// Token: 0x06004420 RID: 17440 RVA: 0x002090C7 File Offset: 0x002072C7
		private static bool IsYiYihou(MiscItem miscItem)
		{
			return miscItem.GroupId == 57;
		}

		// Token: 0x06004421 RID: 17441 RVA: 0x002090D3 File Offset: 0x002072D3
		private static bool IsWeiQi(MiscItem miscItem)
		{
			return miscItem.GroupId == 62;
		}

		// Token: 0x06004422 RID: 17442 RVA: 0x002090DF File Offset: 0x002072DF
		private static bool IsYixiang(MiscItem miscItem)
		{
			return miscItem.GroupId == 67;
		}

		// Token: 0x06004423 RID: 17443 RVA: 0x002090EB File Offset: 0x002072EB
		private static bool IsXuefeng(MiscItem miscItem)
		{
			return miscItem.GroupId == 72;
		}

		// Token: 0x06004424 RID: 17444 RVA: 0x002090F7 File Offset: 0x002072F7
		private static bool IsShuFang(MiscItem miscItem)
		{
			return miscItem.GroupId == 77;
		}

		// Token: 0x02001949 RID: 6473
		private enum EMenuKeys
		{
			// Token: 0x0400B1D1 RID: 45521
			Monv,
			// Token: 0x0400B1D2 RID: 45522
			DayueYaochang,
			// Token: 0x0400B1D3 RID: 45523
			Jiuhan,
			// Token: 0x0400B1D4 RID: 45524
			JinHuanger,
			// Token: 0x0400B1D5 RID: 45525
			YiYihou,
			// Token: 0x0400B1D6 RID: 45526
			WeiQi,
			// Token: 0x0400B1D7 RID: 45527
			Yixiang,
			// Token: 0x0400B1D8 RID: 45528
			Xuefeng,
			// Token: 0x0400B1D9 RID: 45529
			ShuFang,
			// Token: 0x0400B1DA RID: 45530
			Count
		}
	}
}
