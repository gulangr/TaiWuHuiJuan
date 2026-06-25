using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DD7 RID: 3543
	public class MiscWesternPresentTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x1700129B RID: 4763
		// (get) Token: 0x0600AA16 RID: 43542 RVA: 0x004E73E4 File Offset: 0x004E55E4
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x1700129C RID: 4764
		// (get) Token: 0x0600AA17 RID: 43543 RVA: 0x004E73E7 File Offset: 0x004E55E7
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600AA18 RID: 43544 RVA: 0x004E73EC File Offset: 0x004E55EC
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600AA19 RID: 43545 RVA: 0x004E7408 File Offset: 0x004E5608
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (MiscWesternPresentTypeMenu.EMenuKeys t = MiscWesternPresentTypeMenu.EMenuKeys.Monv; t < MiscWesternPresentTypeMenu.EMenuKeys.Count; t++)
			{
				configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Item_Third_MiscWesternPresent_{0}", (int)t))));
			}
			return configs;
		}

		// Token: 0x0600AA1A RID: 43546 RVA: 0x004E7458 File Offset: 0x004E5658
		public override bool IsDataMatch(ITradeableContent data, IReadOnlyCollection<int> selectedIndices)
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

		// Token: 0x0600AA1B RID: 43547 RVA: 0x004E75BC File Offset: 0x004E57BC
		private static bool IsMonv(MiscItem miscItem)
		{
			return miscItem.GroupId == 37;
		}

		// Token: 0x0600AA1C RID: 43548 RVA: 0x004E75C8 File Offset: 0x004E57C8
		private static bool IsDayueYaochang(MiscItem miscItem)
		{
			return miscItem.GroupId == 42;
		}

		// Token: 0x0600AA1D RID: 43549 RVA: 0x004E75D4 File Offset: 0x004E57D4
		private static bool IsJiuhan(MiscItem miscItem)
		{
			return miscItem.GroupId == 47;
		}

		// Token: 0x0600AA1E RID: 43550 RVA: 0x004E75E0 File Offset: 0x004E57E0
		private static bool IsJinHuanger(MiscItem miscItem)
		{
			return miscItem.GroupId == 52;
		}

		// Token: 0x0600AA1F RID: 43551 RVA: 0x004E75EC File Offset: 0x004E57EC
		private static bool IsYiYihou(MiscItem miscItem)
		{
			return miscItem.GroupId == 57;
		}

		// Token: 0x0600AA20 RID: 43552 RVA: 0x004E75F8 File Offset: 0x004E57F8
		private static bool IsWeiQi(MiscItem miscItem)
		{
			return miscItem.GroupId == 62;
		}

		// Token: 0x0600AA21 RID: 43553 RVA: 0x004E7604 File Offset: 0x004E5804
		private static bool IsYixiang(MiscItem miscItem)
		{
			return miscItem.GroupId == 67;
		}

		// Token: 0x0600AA22 RID: 43554 RVA: 0x004E7610 File Offset: 0x004E5810
		private static bool IsXuefeng(MiscItem miscItem)
		{
			return miscItem.GroupId == 72;
		}

		// Token: 0x0600AA23 RID: 43555 RVA: 0x004E761C File Offset: 0x004E581C
		private static bool IsShuFang(MiscItem miscItem)
		{
			return miscItem.GroupId == 77;
		}

		// Token: 0x0200249E RID: 9374
		private enum EMenuKeys
		{
			// Token: 0x0400E50B RID: 58635
			Monv,
			// Token: 0x0400E50C RID: 58636
			DayueYaochang,
			// Token: 0x0400E50D RID: 58637
			Jiuhan,
			// Token: 0x0400E50E RID: 58638
			JinHuanger,
			// Token: 0x0400E50F RID: 58639
			YiYihou,
			// Token: 0x0400E510 RID: 58640
			WeiQi,
			// Token: 0x0400E511 RID: 58641
			Yixiang,
			// Token: 0x0400E512 RID: 58642
			Xuefeng,
			// Token: 0x0400E513 RID: 58643
			ShuFang,
			// Token: 0x0400E514 RID: 58644
			Count
		}
	}
}
