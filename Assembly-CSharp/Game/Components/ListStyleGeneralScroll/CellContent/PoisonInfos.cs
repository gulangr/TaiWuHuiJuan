using System;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000ED2 RID: 3794
	public class PoisonInfos : MonoBehaviour
	{
		// Token: 0x0600AF14 RID: 44820 RVA: 0x004FC4F0 File Offset: 0x004FA6F0
		public void Refresh(ItemDisplayData data)
		{
			bool flag = data == null;
			if (flag)
			{
				foreach (PoisonInfo poisonInfo in this.poisonInfos)
				{
					poisonInfo.gameObject.SetActive(false);
				}
			}
			else
			{
				bool flag2 = !data.PoisonIsIdentified || !data.HasAnyPoison;
				if (flag2)
				{
					foreach (PoisonInfo poisonInfo2 in this.poisonInfos)
					{
						poisonInfo2.gameObject.SetActive(false);
					}
				}
				else
				{
					int poisonSlotCount = data.PoisonEffects.PoisonSlotList.Count;
					for (int i = 0; i < this.poisonInfos.Length; i++)
					{
						PoisonInfo poisonInfo3 = this.poisonInfos[i];
						bool flag3 = i < poisonSlotCount;
						if (flag3)
						{
							poisonInfo3.gameObject.SetActive(true);
							poisonInfo3.Refresh(data.PoisonEffects.PoisonSlotList[i], false);
						}
						else
						{
							poisonInfo3.gameObject.SetActive(false);
						}
					}
				}
			}
		}

		// Token: 0x0600AF15 RID: 44821 RVA: 0x004FC60C File Offset: 0x004FA80C
		public void Refresh(FullPoisonEffects poisonEffects, bool needPoisonBack)
		{
			bool flag = poisonEffects == null;
			if (flag)
			{
				foreach (PoisonInfo poisonInfo in this.poisonInfos)
				{
					poisonInfo.gameObject.SetActive(false);
				}
			}
			else
			{
				int index = 0;
				for (int i = 0; i < this.poisonInfos.Length; i++)
				{
					PoisonInfo poisonInfo2 = this.poisonInfos[i];
					bool flag2 = poisonEffects.PoisonSlotList.CheckIndex(i) && poisonEffects.PoisonSlotList[index].IsValid;
					if (flag2)
					{
						poisonInfo2.gameObject.SetActive(true);
						poisonInfo2.Refresh(poisonEffects.PoisonSlotList[index], needPoisonBack && poisonEffects.PoisonSlotList[index].CurrentMedicineCount == FullPoisonEffects.MaxSlotCount);
						index++;
					}
					else
					{
						poisonInfo2.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x0600AF16 RID: 44822 RVA: 0x004FC70C File Offset: 0x004FA90C
		public void SetStyleEffect(int index, bool isGray = true)
		{
			for (int i = 0; i < index; i++)
			{
				PoisonInfo poisonInfo = this.poisonInfos[i];
				poisonInfo.SetStyleEffect(isGray);
			}
			while (index < this.poisonInfos.Length)
			{
				PoisonInfo poisonInfo2 = this.poisonInfos[index];
				poisonInfo2.SetStyleEffect(true);
				index++;
			}
		}

		// Token: 0x04008798 RID: 34712
		[SerializeField]
		private PoisonInfo[] poisonInfos = new PoisonInfo[FullPoisonEffects.MaxSlotCount];
	}
}
