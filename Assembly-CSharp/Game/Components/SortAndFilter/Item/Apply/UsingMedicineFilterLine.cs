using System;
using System.Collections.Generic;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

namespace Game.Components.SortAndFilter.Item.Apply
{
	// Token: 0x02000DF6 RID: 3574
	public class UsingMedicineFilterLine : MedicineFilterLine
	{
		// Token: 0x170012C6 RID: 4806
		// (get) Token: 0x0600AAB0 RID: 43696 RVA: 0x004E92AE File Offset: 0x004E74AE
		protected override int Level
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x170012C7 RID: 4807
		// (get) Token: 0x0600AAB1 RID: 43697 RVA: 0x004E92B1 File Offset: 0x004E74B1
		public override int Id
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x0600AAB2 RID: 43698 RVA: 0x004E92B4 File Offset: 0x004E74B4
		public override bool IsDataMatch(ITradeableContent data, LineState medicineLineState)
		{
			ToggleKey medicineToggleState = medicineLineState.ToggleGroupState;
			bool isAll = medicineToggleState.IsAll;
			bool result;
			if (isAll)
			{
				result = true;
			}
			else
			{
				bool flag = data.Key.ItemType == 8;
				if (flag)
				{
					result = base.IsDataMatch(data, medicineLineState);
				}
				else
				{
					bool flag2 = medicineToggleState.Index == (int)EMedicineSubFilterKeys.Other.ToSbyte();
					result = (flag2 && (ItemTemplateHelper.IsTianJieFuLu(data.Key.ItemType, data.Key.TemplateId) || CommonUtils.CanMaterialEat(data.Key.ItemType, data.Key.TemplateId, SingletonObject.getInstance<BasicGameData>().TaiwuCharId)));
				}
			}
			return result;
		}

		// Token: 0x0600AAB3 RID: 43699 RVA: 0x004E9360 File Offset: 0x004E7560
		protected override List<ToggleIdIndex> GetActiveDependencies()
		{
			return null;
		}
	}
}
