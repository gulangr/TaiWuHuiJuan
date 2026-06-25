using System;
using System.Collections.Generic;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Item;
using GameData.Domains.Item.Display;

namespace Game.Views.Combat
{
	// Token: 0x02000B0E RID: 2830
	public sealed class CombatRawCreateDestinationItemSortAndFilterController : ItemSortAndFilterController
	{
		// Token: 0x17000F57 RID: 3927
		// (get) Token: 0x06008B1C RID: 35612 RVA: 0x00405CDC File Offset: 0x00403EDC
		public EEquipSubFilterKeys EquipSubFilterKey
		{
			get
			{
				return this._equipSubFilterKey;
			}
		}

		// Token: 0x06008B1D RID: 35613 RVA: 0x00405CE4 File Offset: 0x00403EE4
		public CombatRawCreateDestinationItemSortAndFilterController(ISortAndFilterView sortAndFilter, EEquipSubFilterKeys equipSubFilterKey, LanguageKey filterPanelTitleKey = LanguageKey.EventEditor_Error_DuplicateGroupKey) : base(sortAndFilter, filterPanelTitleKey)
		{
			this._equipSubFilterKey = equipSubFilterKey;
			this.SortController = new ItemSortController
			{
				EnablePriorityCompare = false
			};
		}

		// Token: 0x06008B1E RID: 35614 RVA: 0x00405D18 File Offset: 0x00403F18
		public static bool TryMapEquipmentSlotToEquipSubFilter(sbyte equipmentSlot, out EEquipSubFilterKeys key)
		{
			switch (equipmentSlot)
			{
			case 0:
			case 1:
			case 2:
				key = EEquipSubFilterKeys.Weapon;
				return true;
			case 3:
				key = EEquipSubFilterKeys.Helm;
				return true;
			case 5:
				key = EEquipSubFilterKeys.Torso;
				return true;
			case 6:
				key = EEquipSubFilterKeys.Bracers;
				return true;
			case 7:
				key = EEquipSubFilterKeys.Boots;
				return true;
			case 8:
			case 9:
			case 10:
				key = EEquipSubFilterKeys.Accessory;
				return true;
			}
			key = EEquipSubFilterKeys.Weapon;
			return false;
		}

		// Token: 0x06008B1F RID: 35615 RVA: 0x00405D90 File Offset: 0x00403F90
		protected override IEnumerable<FilterLineBase<ITradeableContent>> GenerateFilterLines()
		{
			switch (this._equipSubFilterKey)
			{
			case EEquipSubFilterKeys.Weapon:
				yield return new CombatRawCreateDestinationWeaponDetailLine();
				break;
			case EEquipSubFilterKeys.Helm:
				yield return new CombatRawCreateDestinationHelmDetailLine();
				break;
			case EEquipSubFilterKeys.Torso:
				yield return new CombatRawCreateDestinationTorsoDetailLine();
				break;
			case EEquipSubFilterKeys.Bracers:
				yield return new CombatRawCreateDestinationBracersDetailLine();
				break;
			case EEquipSubFilterKeys.Boots:
				yield return new CombatRawCreateDestinationBootsDetailLine();
				break;
			case EEquipSubFilterKeys.Accessory:
				yield return new CombatRawCreateDestinationAccessoryDetailLine();
				break;
			}
			yield break;
		}

		// Token: 0x04006AB9 RID: 27321
		private readonly EEquipSubFilterKeys _equipSubFilterKey;
	}
}
