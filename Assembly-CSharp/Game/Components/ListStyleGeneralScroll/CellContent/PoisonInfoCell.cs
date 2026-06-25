using System;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000ED1 RID: 3793
	public class PoisonInfoCell : MonoBehaviour, ICellContent<ItemDisplayData>, ICellContent
	{
		// Token: 0x0600AF12 RID: 44818 RVA: 0x004FC3D8 File Offset: 0x004FA5D8
		public void SetData(ItemDisplayData data)
		{
			bool flag = !data.HasAnyPoison;
			if (flag)
			{
				foreach (PoisonLevel poisonLevel in this.poisonLevels)
				{
					poisonLevel.RefreshEmpty();
				}
			}
			else
			{
				bool flag2 = data.HasAnyPoison && !data.PoisonIsIdentified;
				if (flag2)
				{
					foreach (PoisonLevel poisonLevel2 in this.poisonLevels)
					{
						poisonLevel2.RefreshNotIdentified();
					}
				}
				else
				{
					int poisonSlotCount = data.PoisonEffects.PoisonSlotList.Count;
					for (int i = 0; i < this.poisonLevels.Length; i++)
					{
						PoisonLevel poison = this.poisonLevels[i];
						bool flag3 = i < poisonSlotCount;
						if (flag3)
						{
							poison.Refresh(data.PoisonEffects.PoisonSlotList[i]);
						}
						else
						{
							poison.RefreshEmpty();
						}
					}
				}
			}
		}

		// Token: 0x04008797 RID: 34711
		[SerializeField]
		private PoisonLevel[] poisonLevels = new PoisonLevel[FullPoisonEffects.MaxSlotCount];
	}
}
