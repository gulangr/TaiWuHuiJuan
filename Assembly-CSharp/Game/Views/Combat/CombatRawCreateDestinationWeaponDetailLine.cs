using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item;
using GameData.Domains.Item.Display;

namespace Game.Views.Combat
{
	// Token: 0x02000B0F RID: 2831
	public sealed class CombatRawCreateDestinationWeaponDetailLine : WeaponDetailedFilterLine
	{
		// Token: 0x17000F58 RID: 3928
		// (get) Token: 0x06008B20 RID: 35616 RVA: 0x00405DA0 File Offset: 0x00403FA0
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000F59 RID: 3929
		// (get) Token: 0x06008B21 RID: 35617 RVA: 0x00405DA3 File Offset: 0x00403FA3
		protected override bool IndividualLine
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06008B22 RID: 35618 RVA: 0x00405DA6 File Offset: 0x00403FA6
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}

		// Token: 0x06008B23 RID: 35619 RVA: 0x00405DA9 File Offset: 0x00403FA9
		protected override IEnumerable<DetailedFilterMenuLogic<ITradeableContent>> GenerateMenus()
		{
			yield return new WeaponMaterialMenu();
			yield return new WeaponFabricHardnessMenu();
			yield return new WeaponWoodHardnessMenu();
			yield return new WeaponJadeHardnessMenu();
			yield return new WeaponMetalHardnessMenu();
			yield return new WeaponHerbMakeTypeMenu();
			yield break;
		}
	}
}
