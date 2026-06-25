using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item
{
	// Token: 0x02000DD4 RID: 3540
	public class MiscKeyItemTypeMenu : DetailedFilterMenuLogic<ITradeableContent>
	{
		// Token: 0x17001297 RID: 4759
		// (get) Token: 0x0600A9F7 RID: 43511 RVA: 0x004E6EF8 File Offset: 0x004E50F8
		public override EFilterLogic LogicType
		{
			get
			{
				return EFilterLogic.Or;
			}
		}

		// Token: 0x17001298 RID: 4760
		// (get) Token: 0x0600A9F8 RID: 43512 RVA: 0x004E6EFB File Offset: 0x004E50FB
		public override int Id
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x0600A9F9 RID: 43513 RVA: 0x004E6F00 File Offset: 0x004E5100
		public override StringKey GetMenuBarLabel()
		{
			return LanguageKey.LK_CommonSortAndFilter_Filter_Item_Third_Title_Category;
		}

		// Token: 0x0600A9FA RID: 43514 RVA: 0x004E6F1C File Offset: 0x004E511C
		public override List<FilterDropdownItemConfig> GetMenuItemConfigs()
		{
			List<FilterDropdownItemConfig> configs = new List<FilterDropdownItemConfig>();
			for (MiscKeyItemTypeMenu.EMenuKeys t = MiscKeyItemTypeMenu.EMenuKeys.Resource; t < MiscKeyItemTypeMenu.EMenuKeys.Count; t++)
			{
				configs.Add(new FilterDropdownItemConfig(StringKey.CreateKey(string.Format("LK_CommonSortAndFilter_Filter_Item_Third_MiscKeyItem_{0}", (int)t))));
			}
			return configs;
		}

		// Token: 0x0600A9FB RID: 43515 RVA: 0x004E6F6C File Offset: 0x004E516C
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

		// Token: 0x0600A9FC RID: 43516 RVA: 0x004E7210 File Offset: 0x004E5410
		private static bool IsResource(MiscItem miscItem)
		{
			short groupId = miscItem.GroupId;
			return groupId == 100 || groupId == 110;
		}

		// Token: 0x0600A9FD RID: 43517 RVA: 0x004E723C File Offset: 0x004E543C
		private static bool IsCollectResource(MiscItem miscItem)
		{
			return miscItem.GroupId == 120;
		}

		// Token: 0x0600A9FE RID: 43518 RVA: 0x004E7258 File Offset: 0x004E5458
		private static bool IsTaiwuVillage(MiscItem miscItem)
		{
			short templateId = miscItem.TemplateId;
			return templateId == 225 || templateId == 226;
		}

		// Token: 0x0600A9FF RID: 43519 RVA: 0x004E728A File Offset: 0x004E548A
		private static bool IsCombatSkill(MiscItem miscItem)
		{
			return miscItem.GroupId == 130;
		}

		// Token: 0x0600AA00 RID: 43520 RVA: 0x004E7299 File Offset: 0x004E5499
		private static bool IsMusic(MiscItem miscItem)
		{
			return miscItem.GroupId == 146;
		}

		// Token: 0x0600AA01 RID: 43521 RVA: 0x004E72A8 File Offset: 0x004E54A8
		private static bool IsChess(MiscItem miscItem)
		{
			return miscItem.GroupId == 150;
		}

		// Token: 0x0600AA02 RID: 43522 RVA: 0x004E72B7 File Offset: 0x004E54B7
		private static bool IsPoem(MiscItem miscItem)
		{
			return miscItem.GroupId == 154;
		}

		// Token: 0x0600AA03 RID: 43523 RVA: 0x004E72C6 File Offset: 0x004E54C6
		private static bool IsPainting(MiscItem miscItem)
		{
			return miscItem.GroupId == 158;
		}

		// Token: 0x0600AA04 RID: 43524 RVA: 0x004E72D5 File Offset: 0x004E54D5
		private static bool IsMath(MiscItem miscItem)
		{
			return miscItem.GroupId == 162;
		}

		// Token: 0x0600AA05 RID: 43525 RVA: 0x004E72E4 File Offset: 0x004E54E4
		private static bool IsAppraisal(MiscItem miscItem)
		{
			return miscItem.GroupId == 166;
		}

		// Token: 0x0600AA06 RID: 43526 RVA: 0x004E72F3 File Offset: 0x004E54F3
		private static bool IsForging(MiscItem miscItem)
		{
			return miscItem.GroupId == 171;
		}

		// Token: 0x0600AA07 RID: 43527 RVA: 0x004E7302 File Offset: 0x004E5502
		private static bool IsWoodworking(MiscItem miscItem)
		{
			return miscItem.GroupId == 177;
		}

		// Token: 0x0600AA08 RID: 43528 RVA: 0x004E7311 File Offset: 0x004E5511
		private static bool IsMedicine(MiscItem miscItem)
		{
			return miscItem.GroupId == 183;
		}

		// Token: 0x0600AA09 RID: 43529 RVA: 0x004E7320 File Offset: 0x004E5520
		private static bool IsToxicology(MiscItem miscItem)
		{
			return miscItem.GroupId == 189;
		}

		// Token: 0x0600AA0A RID: 43530 RVA: 0x004E732F File Offset: 0x004E552F
		private static bool IsWeaving(MiscItem miscItem)
		{
			return miscItem.GroupId == 195;
		}

		// Token: 0x0600AA0B RID: 43531 RVA: 0x004E733E File Offset: 0x004E553E
		private static bool IsJade(MiscItem miscItem)
		{
			return miscItem.GroupId == 201;
		}

		// Token: 0x0600AA0C RID: 43532 RVA: 0x004E734D File Offset: 0x004E554D
		private static bool IsTaoism(MiscItem miscItem)
		{
			return miscItem.GroupId == 207;
		}

		// Token: 0x0600AA0D RID: 43533 RVA: 0x004E735C File Offset: 0x004E555C
		private static bool IsBuddhism(MiscItem miscItem)
		{
			return miscItem.GroupId == 211;
		}

		// Token: 0x0600AA0E RID: 43534 RVA: 0x004E736B File Offset: 0x004E556B
		private static bool IsCooking(MiscItem miscItem)
		{
			return miscItem.GroupId == 215;
		}

		// Token: 0x0600AA0F RID: 43535 RVA: 0x004E737A File Offset: 0x004E557A
		private static bool IsEclectic(MiscItem miscItem)
		{
			return miscItem.GroupId == 221;
		}

		// Token: 0x0200249C RID: 9372
		private enum EMenuKeys
		{
			// Token: 0x0400E4F1 RID: 58609
			Resource,
			// Token: 0x0400E4F2 RID: 58610
			CollectResource,
			// Token: 0x0400E4F3 RID: 58611
			TaiwuVillage,
			// Token: 0x0400E4F4 RID: 58612
			CombatSkill,
			// Token: 0x0400E4F5 RID: 58613
			Music,
			// Token: 0x0400E4F6 RID: 58614
			Chess,
			// Token: 0x0400E4F7 RID: 58615
			Poem,
			// Token: 0x0400E4F8 RID: 58616
			Painting,
			// Token: 0x0400E4F9 RID: 58617
			Math,
			// Token: 0x0400E4FA RID: 58618
			Appraisal,
			// Token: 0x0400E4FB RID: 58619
			Forging,
			// Token: 0x0400E4FC RID: 58620
			Woodworking,
			// Token: 0x0400E4FD RID: 58621
			Medicine,
			// Token: 0x0400E4FE RID: 58622
			Toxicology,
			// Token: 0x0400E4FF RID: 58623
			Weaving,
			// Token: 0x0400E500 RID: 58624
			Jade,
			// Token: 0x0400E501 RID: 58625
			Taoism,
			// Token: 0x0400E502 RID: 58626
			Buddhism,
			// Token: 0x0400E503 RID: 58627
			Cooking,
			// Token: 0x0400E504 RID: 58628
			Eclectic,
			// Token: 0x0400E505 RID: 58629
			Count
		}
	}
}
