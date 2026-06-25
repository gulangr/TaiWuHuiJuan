using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using GameData.Domains.Item.Display;

namespace CommonSortAndFilterLegacy.Item
{
	// Token: 0x0200054E RID: 1358
	internal class MiscKeyItemTypeMenu : StaticDetailedFilterMenuBase<ItemDisplayData>
	{
		// Token: 0x17000803 RID: 2051
		// (get) Token: 0x060043D4 RID: 17364 RVA: 0x0020845B File Offset: 0x0020665B
		public override FilterLogic LogicType
		{
			get
			{
				return FilterLogic.Or;
			}
		}

		// Token: 0x060043D5 RID: 17365 RVA: 0x00208460 File Offset: 0x00206660
		public override DetailFilterMultiSelectDropdownMenuBarConfig GetMenuBarConfig()
		{
			return new DetailFilterMultiSelectDropdownMenuBarConfig(LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category);
		}

		// Token: 0x060043D6 RID: 17366 RVA: 0x00208484 File Offset: 0x00206684
		public override List<DetailFilterMultiSelectDropdownItemConfig> GetMenuConfigs()
		{
			List<DetailFilterMultiSelectDropdownItemConfig> menuConfigs = new List<DetailFilterMultiSelectDropdownItemConfig>();
			for (MiscKeyItemTypeMenu.EMenuKeys t = MiscKeyItemTypeMenu.EMenuKeys.Resource; t < MiscKeyItemTypeMenu.EMenuKeys.Count; t++)
			{
				StringKey key = StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Item_Third_MiscKeyItem_{0}", (int)t));
				menuConfigs.Add(new DetailFilterMultiSelectDropdownItemConfig
				{
					IconPath = null,
					Text = key
				});
			}
			return menuConfigs;
		}

		// Token: 0x060043D7 RID: 17367 RVA: 0x002084EC File Offset: 0x002066EC
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
						bool flag = MiscKeyItemTypeMenu.IsResource(miscItem);
						if (flag)
						{
							return true;
						}
						break;
					}
					case 1:
					{
						bool flag2 = MiscKeyItemTypeMenu.IsCollectResource(miscItem);
						if (flag2)
						{
							return true;
						}
						break;
					}
					case 2:
					{
						bool flag3 = MiscKeyItemTypeMenu.IsTaiwuVillage(miscItem);
						if (flag3)
						{
							return true;
						}
						break;
					}
					case 3:
					{
						bool flag4 = MiscKeyItemTypeMenu.IsCombatSkill(miscItem);
						if (flag4)
						{
							return true;
						}
						break;
					}
					case 4:
					{
						bool flag5 = MiscKeyItemTypeMenu.IsMusic(miscItem);
						if (flag5)
						{
							return true;
						}
						break;
					}
					case 5:
					{
						bool flag6 = MiscKeyItemTypeMenu.IsChess(miscItem);
						if (flag6)
						{
							return true;
						}
						break;
					}
					case 6:
					{
						bool flag7 = MiscKeyItemTypeMenu.IsPoem(miscItem);
						if (flag7)
						{
							return true;
						}
						break;
					}
					case 7:
					{
						bool flag8 = MiscKeyItemTypeMenu.IsPainting(miscItem);
						if (flag8)
						{
							return true;
						}
						break;
					}
					case 8:
					{
						bool flag9 = MiscKeyItemTypeMenu.IsMath(miscItem);
						if (flag9)
						{
							return true;
						}
						break;
					}
					case 9:
					{
						bool flag10 = MiscKeyItemTypeMenu.IsAppraisal(miscItem);
						if (flag10)
						{
							return true;
						}
						break;
					}
					case 10:
					{
						bool flag11 = MiscKeyItemTypeMenu.IsForging(miscItem);
						if (flag11)
						{
							return true;
						}
						break;
					}
					case 11:
					{
						bool flag12 = MiscKeyItemTypeMenu.IsWoodworking(miscItem);
						if (flag12)
						{
							return true;
						}
						break;
					}
					case 12:
					{
						bool flag13 = MiscKeyItemTypeMenu.IsMedicine(miscItem);
						if (flag13)
						{
							return true;
						}
						break;
					}
					case 13:
					{
						bool flag14 = MiscKeyItemTypeMenu.IsToxicology(miscItem);
						if (flag14)
						{
							return true;
						}
						break;
					}
					case 14:
					{
						bool flag15 = MiscKeyItemTypeMenu.IsWeaving(miscItem);
						if (flag15)
						{
							return true;
						}
						break;
					}
					case 15:
					{
						bool flag16 = MiscKeyItemTypeMenu.IsJade(miscItem);
						if (flag16)
						{
							return true;
						}
						break;
					}
					case 16:
					{
						bool flag17 = MiscKeyItemTypeMenu.IsTaoism(miscItem);
						if (flag17)
						{
							return true;
						}
						break;
					}
					case 17:
					{
						bool flag18 = MiscKeyItemTypeMenu.IsBuddhism(miscItem);
						if (flag18)
						{
							return true;
						}
						break;
					}
					case 18:
					{
						bool flag19 = MiscKeyItemTypeMenu.IsCooking(miscItem);
						if (flag19)
						{
							return true;
						}
						break;
					}
					case 19:
					{
						bool flag20 = MiscKeyItemTypeMenu.IsEclectic(miscItem);
						if (flag20)
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

		// Token: 0x17000804 RID: 2052
		// (get) Token: 0x060043D8 RID: 17368 RVA: 0x00208790 File Offset: 0x00206990
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x060043D9 RID: 17369 RVA: 0x00208794 File Offset: 0x00206994
		private static bool IsResource(MiscItem miscItem)
		{
			short groupId = miscItem.GroupId;
			return groupId == 100 || groupId == 110;
		}

		// Token: 0x060043DA RID: 17370 RVA: 0x002087C0 File Offset: 0x002069C0
		private static bool IsCollectResource(MiscItem miscItem)
		{
			return miscItem.GroupId == 120;
		}

		// Token: 0x060043DB RID: 17371 RVA: 0x002087DC File Offset: 0x002069DC
		private static bool IsTaiwuVillage(MiscItem miscItem)
		{
			short templateId = miscItem.TemplateId;
			return templateId == 225 || templateId == 226;
		}

		// Token: 0x060043DC RID: 17372 RVA: 0x0020880E File Offset: 0x00206A0E
		private static bool IsCombatSkill(MiscItem miscItem)
		{
			return miscItem.GroupId == 130;
		}

		// Token: 0x060043DD RID: 17373 RVA: 0x0020881D File Offset: 0x00206A1D
		private static bool IsMusic(MiscItem miscItem)
		{
			return miscItem.GroupId == 146;
		}

		// Token: 0x060043DE RID: 17374 RVA: 0x0020882C File Offset: 0x00206A2C
		private static bool IsChess(MiscItem miscItem)
		{
			return miscItem.GroupId == 150;
		}

		// Token: 0x060043DF RID: 17375 RVA: 0x0020883B File Offset: 0x00206A3B
		private static bool IsPoem(MiscItem miscItem)
		{
			return miscItem.GroupId == 154;
		}

		// Token: 0x060043E0 RID: 17376 RVA: 0x0020884A File Offset: 0x00206A4A
		private static bool IsPainting(MiscItem miscItem)
		{
			return miscItem.GroupId == 158;
		}

		// Token: 0x060043E1 RID: 17377 RVA: 0x00208859 File Offset: 0x00206A59
		private static bool IsMath(MiscItem miscItem)
		{
			return miscItem.GroupId == 162;
		}

		// Token: 0x060043E2 RID: 17378 RVA: 0x00208868 File Offset: 0x00206A68
		private static bool IsAppraisal(MiscItem miscItem)
		{
			return miscItem.GroupId == 166;
		}

		// Token: 0x060043E3 RID: 17379 RVA: 0x00208877 File Offset: 0x00206A77
		private static bool IsForging(MiscItem miscItem)
		{
			return miscItem.GroupId == 171;
		}

		// Token: 0x060043E4 RID: 17380 RVA: 0x00208886 File Offset: 0x00206A86
		private static bool IsWoodworking(MiscItem miscItem)
		{
			return miscItem.GroupId == 177;
		}

		// Token: 0x060043E5 RID: 17381 RVA: 0x00208895 File Offset: 0x00206A95
		private static bool IsMedicine(MiscItem miscItem)
		{
			return miscItem.GroupId == 183;
		}

		// Token: 0x060043E6 RID: 17382 RVA: 0x002088A4 File Offset: 0x00206AA4
		private static bool IsToxicology(MiscItem miscItem)
		{
			return miscItem.GroupId == 189;
		}

		// Token: 0x060043E7 RID: 17383 RVA: 0x002088B3 File Offset: 0x00206AB3
		private static bool IsWeaving(MiscItem miscItem)
		{
			return miscItem.GroupId == 195;
		}

		// Token: 0x060043E8 RID: 17384 RVA: 0x002088C2 File Offset: 0x00206AC2
		private static bool IsJade(MiscItem miscItem)
		{
			return miscItem.GroupId == 201;
		}

		// Token: 0x060043E9 RID: 17385 RVA: 0x002088D1 File Offset: 0x00206AD1
		private static bool IsTaoism(MiscItem miscItem)
		{
			return miscItem.GroupId == 207;
		}

		// Token: 0x060043EA RID: 17386 RVA: 0x002088E0 File Offset: 0x00206AE0
		private static bool IsBuddhism(MiscItem miscItem)
		{
			return miscItem.GroupId == 211;
		}

		// Token: 0x060043EB RID: 17387 RVA: 0x002088EF File Offset: 0x00206AEF
		private static bool IsCooking(MiscItem miscItem)
		{
			return miscItem.GroupId == 215;
		}

		// Token: 0x060043EC RID: 17388 RVA: 0x002088FE File Offset: 0x00206AFE
		private static bool IsEclectic(MiscItem miscItem)
		{
			return miscItem.GroupId == 221;
		}

		// Token: 0x02001942 RID: 6466
		private enum EMenuKeys
		{
			// Token: 0x0400B1A6 RID: 45478
			Resource,
			// Token: 0x0400B1A7 RID: 45479
			CollectResource,
			// Token: 0x0400B1A8 RID: 45480
			TaiwuVillage,
			// Token: 0x0400B1A9 RID: 45481
			CombatSkill,
			// Token: 0x0400B1AA RID: 45482
			Music,
			// Token: 0x0400B1AB RID: 45483
			Chess,
			// Token: 0x0400B1AC RID: 45484
			Poem,
			// Token: 0x0400B1AD RID: 45485
			Painting,
			// Token: 0x0400B1AE RID: 45486
			Math,
			// Token: 0x0400B1AF RID: 45487
			Appraisal,
			// Token: 0x0400B1B0 RID: 45488
			Forging,
			// Token: 0x0400B1B1 RID: 45489
			Woodworking,
			// Token: 0x0400B1B2 RID: 45490
			Medicine,
			// Token: 0x0400B1B3 RID: 45491
			Toxicology,
			// Token: 0x0400B1B4 RID: 45492
			Weaving,
			// Token: 0x0400B1B5 RID: 45493
			Jade,
			// Token: 0x0400B1B6 RID: 45494
			Taoism,
			// Token: 0x0400B1B7 RID: 45495
			Buddhism,
			// Token: 0x0400B1B8 RID: 45496
			Cooking,
			// Token: 0x0400B1B9 RID: 45497
			Eclectic,
			// Token: 0x0400B1BA RID: 45498
			Count
		}
	}
}
