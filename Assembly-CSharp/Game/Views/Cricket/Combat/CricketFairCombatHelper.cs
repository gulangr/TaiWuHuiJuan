using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AD9 RID: 2777
	public static class CricketFairCombatHelper
	{
		// Token: 0x17000F16 RID: 3862
		// (get) Token: 0x060088AB RID: 34987 RVA: 0x003F5267 File Offset: 0x003F3467
		public static bool IsEnabled
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().ChallengeModeData.IsEnabled(EChallengeModeImplement.CricketFairCombat);
			}
		}

		// Token: 0x060088AC RID: 34988 RVA: 0x003F527C File Offset: 0x003F347C
		public static int GetSelectedCost(IEnumerable<ItemDisplayData> crickets)
		{
			int totalCost = 0;
			foreach (ItemDisplayData cricket in crickets)
			{
				bool flag = cricket == null;
				if (!flag)
				{
					totalCost += CricketFairCombatHelper.GetCricketCost(cricket);
				}
			}
			return totalCost;
		}

		// Token: 0x060088AD RID: 34989 RVA: 0x003F52E0 File Offset: 0x003F34E0
		public static int GetCricketCost(ItemDisplayData cricket)
		{
			bool flag = cricket == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = cricket.CricketColorId == 0;
				if (flag2)
				{
					result = 0;
				}
				else
				{
					int grade = CricketFairCombatHelper.GetCricketGrade(cricket);
					int[] costByGrade = GlobalConfig.Instance.ChallengeCricketFairCombatCostByCricketGrade;
					bool flag3 = costByGrade == null || costByGrade.Length == 0;
					if (flag3)
					{
						result = 0;
					}
					else
					{
						grade = Mathf.Clamp(grade, 0, costByGrade.Length - 1);
						result = costByGrade[grade];
					}
				}
			}
			return result;
		}

		// Token: 0x060088AE RID: 34990 RVA: 0x003F5348 File Offset: 0x003F3548
		public static int GetMaxPointByWager(Wager wager)
		{
			int[] maxPointByGrade = GlobalConfig.Instance.ChallengeCricketFairCombatMaxPointByWagerGrade;
			bool flag = maxPointByGrade == null || maxPointByGrade.Length == 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				sbyte grade = wager.Grade;
				bool flag2 = grade < 0;
				if (flag2)
				{
					grade = 0;
				}
				grade = (sbyte)Mathf.Clamp((int)grade, 0, maxPointByGrade.Length - 1);
				result = maxPointByGrade[(int)grade];
			}
			return result;
		}

		// Token: 0x060088AF RID: 34991 RVA: 0x003F53A0 File Offset: 0x003F35A0
		public static bool HasEnoughCrickets(IReadOnlyList<ItemDisplayData> crickets)
		{
			int validCount = 0;
			foreach (ItemDisplayData cricket in crickets)
			{
				bool flag = cricket != null && cricket.Key.IsValid();
				if (flag)
				{
					validCount++;
				}
			}
			return validCount >= 3;
		}

		// Token: 0x060088B0 RID: 34992 RVA: 0x003F5414 File Offset: 0x003F3614
		public static bool IsPointExceeded(IReadOnlyList<ItemDisplayData> crickets, Wager wager)
		{
			bool flag = !CricketFairCombatHelper.IsEnabled;
			return !flag && CricketFairCombatHelper.GetSelectedCost(crickets) > CricketFairCombatHelper.GetMaxPointByWager(wager);
		}

		// Token: 0x060088B1 RID: 34993 RVA: 0x003F5444 File Offset: 0x003F3644
		public static bool CanStartCombat(IReadOnlyList<ItemDisplayData> crickets, Wager wager)
		{
			return CricketFairCombatHelper.HasEnoughCrickets(crickets) && !CricketFairCombatHelper.IsPointExceeded(crickets, wager);
		}

		// Token: 0x060088B2 RID: 34994 RVA: 0x003F546C File Offset: 0x003F366C
		public static int GetCricketGrade(ItemDisplayData cricket)
		{
			CricketPartsItem color = CricketParts.Instance[cricket.CricketColorId];
			CricketPartsItem part = CricketParts.Instance[cricket.CricketPartId];
			return Mathf.Max((int)color.Level, (int)((part != null) ? part.Level : 0));
		}
	}
}
