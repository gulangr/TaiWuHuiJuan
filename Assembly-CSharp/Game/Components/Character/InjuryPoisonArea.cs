using System;
using Config;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F33 RID: 3891
	public class InjuryPoisonArea : MonoBehaviour
	{
		// Token: 0x0600B306 RID: 45830 RVA: 0x00517EF4 File Offset: 0x005160F4
		public void Set(CharacterInjuryDisplayData displayData)
		{
			this._characterInjuryDisplayData = displayData;
			sbyte type = 0;
			while ((int)type < this.injuryPoisonItems.Length)
			{
				int value = displayData.Poisons.Get((int)type);
				int resist = displayData.PoisonResists.Get((int)type);
				bool isImmune = displayData.IsImmune[(int)type];
				bool isBornImmune = displayData.IsBornImmune[(int)type];
				InjuryPoisonItem item = this.injuryPoisonItems[(int)type];
				item.Set(type, value, resist, isImmune, isBornImmune);
				this.injuryPoisonHeartParts[(int)type].SetActive(value > 0);
				type += 1;
			}
			CommonUtils.SetMixPoisonBorder(this.mixPoisonBorder, displayData.CharacterId);
		}

		// Token: 0x0600B307 RID: 45831 RVA: 0x00517F8E File Offset: 0x0051618E
		public InjuryPoisonItem GetInjuryPoisonItem(sbyte type)
		{
			return this.injuryPoisonItems[(int)type];
		}

		// Token: 0x0600B308 RID: 45832 RVA: 0x00517F98 File Offset: 0x00516198
		public unsafe void ShowInfectNotice(MedicineItem medicineItem, PoisonInts preViewPoisons)
		{
			EMedicineEffectType type = medicineItem.EffectType;
			for (sbyte poisonType = 0; poisonType < 6; poisonType += 1)
			{
				bool immuneFlag = this._characterInjuryDisplayData.IsImmune[(int)poisonType];
				InjuryPoisonItem poisonRefers = this.GetInjuryPoisonItem(poisonType);
				int delta = 0;
				bool flag = poisonType == medicineItem.PoisonType;
				if (flag)
				{
					bool flag2 = type == EMedicineEffectType.DetoxPoison;
					if (flag2)
					{
						bool flag3 = !immuneFlag;
						if (flag3)
						{
							delta = -medicineItem.GetEffectValue(100);
						}
					}
					else
					{
						bool flag4 = type == EMedicineEffectType.ApplyPoison;
						if (flag4)
						{
							bool flag5 = !immuneFlag;
							if (flag5)
							{
								delta = medicineItem.GetEffectValue(100);
							}
						}
					}
				}
				poisonRefers.Recovery.SetActive(delta < 0);
				poisonRefers.Aggravate.SetActive(delta > 0);
				int finalPoison = *preViewPoisons[(int)poisonType];
				int lastPoison = *this._characterInjuryDisplayData.Poisons[(int)poisonType];
				bool poisonChanged = lastPoison != finalPoison;
				bool flag6 = delta != 0;
				if (flag6)
				{
					string color = (delta < 0) ? "brightblue" : "brightred";
					poisonRefers.TextValue.text = (poisonChanged ? finalPoison.ToString().SetColor(color) : finalPoison.ToString());
				}
			}
		}

		// Token: 0x04008AF3 RID: 35571
		[SerializeField]
		private InjuryPoisonItem[] injuryPoisonItems;

		// Token: 0x04008AF4 RID: 35572
		[SerializeField]
		private GameObject[] injuryPoisonHeartParts;

		// Token: 0x04008AF5 RID: 35573
		[SerializeField]
		private GameObject mixPoisonBorder;

		// Token: 0x04008AF6 RID: 35574
		private CharacterInjuryDisplayData _characterInjuryDisplayData;
	}
}
