using System;
using Config;
using Game.Views.Cricket.Combat;
using GameData.Combat.Cricket;
using GameData.Domains.Item.Display;

namespace Game.Views.Cricket
{
	// Token: 0x02000AB8 RID: 2744
	public static class CricketCoreUtils
	{
		// Token: 0x060086A7 RID: 34471 RVA: 0x003E9E04 File Offset: 0x003E8004
		public static CricketCore BuildCricketBasePropertyForTip(ItemDisplayData displayData)
		{
			bool flag = CricketCombatKit.Board.Status == ECricketCombatStatus.Combating;
			if (flag)
			{
				CricketCombatBlackBoard board = CricketCombatKit.Board;
				bool flag2 = board.SelfCricket != null && board.GetCricketItem(true).Key == displayData.Key;
				if (flag2)
				{
					return board.SelfCricket.Data.ModifiedCore;
				}
				bool flag3 = board.EnemyCricket != null && board.GetCricketItem(false).Key == displayData.Key;
				if (flag3)
				{
					return board.EnemyCricket.Data.ModifiedCore;
				}
			}
			return CricketCoreUtils.BuildCricketBaseProperty(displayData);
		}

		// Token: 0x060086A8 RID: 34472 RVA: 0x003E9EAC File Offset: 0x003E80AC
		public static CricketCore BuildCricketBaseProperty(ItemDisplayData displayData)
		{
			short colorId = displayData.CricketColorId;
			CricketCore core = CricketParts.Instance[colorId];
			short partId = displayData.CricketPartId;
			bool flag = partId > 0;
			if (flag)
			{
				core += CricketParts.Instance[partId];
			}
			return core;
		}

		// Token: 0x060086A9 RID: 34473 RVA: 0x003E9F00 File Offset: 0x003E8100
		public static CricketCore BuildCricketFinalProperty(ItemDisplayData displayData, CricketInjury injury)
		{
			CricketCore core = CricketCoreUtils.BuildCricketBaseProperty(displayData);
			core += displayData.CricketData.SpiritAddProperties;
			core.SetProperty(ECricketCombatPropertyType.Hp, core.GetProperty(ECricketCombatPropertyType.Hp) - injury.Hp);
			core.SetProperty(ECricketCombatPropertyType.Sp, core.GetProperty(ECricketCombatPropertyType.Sp) - injury.Sp);
			core.SetProperty(ECricketCombatPropertyType.Strength, core.GetProperty(ECricketCombatPropertyType.Strength) - injury.Strength);
			core.SetProperty(ECricketCombatPropertyType.Vigor, core.GetProperty(ECricketCombatPropertyType.Vigor) - injury.Vigor);
			core.SetProperty(ECricketCombatPropertyType.Bite, core.GetProperty(ECricketCombatPropertyType.Bite) - injury.Bite);
			return core;
		}
	}
}
