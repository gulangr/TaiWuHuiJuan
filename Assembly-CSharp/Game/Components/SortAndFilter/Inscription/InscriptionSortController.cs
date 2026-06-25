using System;
using System.Collections.Generic;
using Game.Views.Main.Inscription;

namespace Game.Components.SortAndFilter.Inscription
{
	// Token: 0x02000DFF RID: 3583
	public class InscriptionSortController : SortController<CheckInscriptionCharData>
	{
		// Token: 0x170012D1 RID: 4817
		// (get) Token: 0x0600AAD5 RID: 43733 RVA: 0x004E973A File Offset: 0x004E793A
		// (set) Token: 0x0600AAD6 RID: 43734 RVA: 0x004E9742 File Offset: 0x004E7942
		public bool IgnorePinOrder { get; set; }

		// Token: 0x0600AAD7 RID: 43735 RVA: 0x004E974C File Offset: 0x004E794C
		public override Comparison<CheckInscriptionCharData> GenerateComparer(SortStateData sortData)
		{
			return (CheckInscriptionCharData x, CheckInscriptionCharData y) => this.CompareData(x, y, sortData);
		}

		// Token: 0x0600AAD8 RID: 43736 RVA: 0x004E9780 File Offset: 0x004E7980
		private int CompareData(CheckInscriptionCharData x, CheckInscriptionCharData y, SortStateData sortData)
		{
			bool flag = x == null && y == null;
			int result2;
			if (flag)
			{
				result2 = 0;
			}
			else
			{
				bool flag2 = x == null;
				if (flag2)
				{
					result2 = 1;
				}
				else
				{
					bool flag3 = y == null;
					if (flag3)
					{
						result2 = -1;
					}
					else
					{
						bool flag4 = !this.IgnorePinOrder;
						if (flag4)
						{
							bool xPinned = x.PinOrder >= 0;
							bool yPinned = y.PinOrder >= 0;
							bool flag5 = xPinned != yPinned;
							if (flag5)
							{
								return xPinned ? -1 : 1;
							}
							bool flag6 = xPinned && x.PinOrder != y.PinOrder;
							if (flag6)
							{
								return x.PinOrder.CompareTo(y.PinOrder);
							}
						}
						bool flag7 = ((sortData != null) ? sortData.ItemStates : null) != null;
						if (flag7)
						{
							foreach (SortItemState itemState in sortData.ItemStates)
							{
								int result = InscriptionSortController.CompareBySortId(x, y, itemState.SortId);
								bool flag8 = result != 0;
								if (flag8)
								{
									return (itemState.SortDirection == ESortDirection.Ascending) ? result : (-result);
								}
							}
						}
						int worldCompare = x.Key.WorldId.CompareTo(y.Key.WorldId);
						bool flag9 = worldCompare != 0;
						if (flag9)
						{
							result2 = worldCompare;
						}
						else
						{
							result2 = x.Key.CharId.CompareTo(y.Key.CharId);
						}
					}
				}
			}
			return result2;
		}

		// Token: 0x0600AAD9 RID: 43737 RVA: 0x004E9918 File Offset: 0x004E7B18
		private static int CompareBySortId(CheckInscriptionCharData x, CheckInscriptionCharData y, short sortId)
		{
			int result;
			if (sortId != 8)
			{
				if (sortId != 9)
				{
					switch (sortId)
					{
					case 140:
						result = x.CombatSkillSum.CompareTo(y.CombatSkillSum);
						break;
					case 141:
						result = x.LifeSkillSum.CompareTo(y.LifeSkillSum);
						break;
					case 142:
						result = x.MainAttributeSum.CompareTo(y.MainAttributeSum);
						break;
					default:
						result = 0;
						break;
					}
				}
				else
				{
					result = x.Charm.CompareTo(y.Charm);
				}
			}
			else
			{
				result = x.Character.CurrAge.CompareTo(y.Character.CurrAge);
			}
			return result;
		}

		// Token: 0x0600AADA RID: 43738 RVA: 0x004E99C4 File Offset: 0x004E7BC4
		public override SortUiConfig GenerateConfig()
		{
			List<short> sortIds = new List<short>
			{
				8,
				9,
				142,
				141,
				140
			};
			return new SortUiConfig
			{
				SortIds = sortIds,
				SortNameIndexList = new List<int>(new int[sortIds.Count]),
				DefaultSortIds = sortIds
			};
		}

		// Token: 0x020024BA RID: 9402
		public static class SortIds
		{
			// Token: 0x0400E57C RID: 58748
			public const short Age = 8;

			// Token: 0x0400E57D RID: 58749
			public const short Charm = 9;

			// Token: 0x0400E57E RID: 58750
			public const short MainAttributeSum = 142;

			// Token: 0x0400E57F RID: 58751
			public const short LifeSkillSum = 141;

			// Token: 0x0400E580 RID: 58752
			public const short CombatSkillSum = 140;
		}
	}
}
