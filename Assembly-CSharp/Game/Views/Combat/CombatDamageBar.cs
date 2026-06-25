using System;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B1F RID: 2847
	public class CombatDamageBar : MonoBehaviour
	{
		// Token: 0x06008BB5 RID: 35765 RVA: 0x004083F6 File Offset: 0x004065F6
		public void SetState(CombatDamageBar.EState state)
		{
			this.hitNotConducted.SetActive(state == CombatDamageBar.EState.NotConducted);
			this.hitSuccessBg.SetActive(state == CombatDamageBar.EState.Hit);
			this.hitMissBg.SetActive(state == CombatDamageBar.EState.Miss);
		}

		// Token: 0x06008BB6 RID: 35766 RVA: 0x0040842C File Offset: 0x0040662C
		public void SetHitType(sbyte hitType)
		{
			string iconName = "ui9_icon_attribute_hit_big_" + hitType.ToString();
			for (int i = 0; i < this.attributeIcons.Length; i++)
			{
				this.attributeIcons[i].SetSprite(iconName, false, null);
			}
		}

		// Token: 0x04006AF5 RID: 27381
		[SerializeField]
		private GameObject hitNotConducted;

		// Token: 0x04006AF6 RID: 27382
		[SerializeField]
		private GameObject hitSuccessBg;

		// Token: 0x04006AF7 RID: 27383
		[SerializeField]
		private GameObject hitMissBg;

		// Token: 0x04006AF8 RID: 27384
		[SerializeField]
		private CImage[] attributeIcons;

		// Token: 0x020020E0 RID: 8416
		public enum EState
		{
			// Token: 0x0400D2D4 RID: 53972
			NotConducted,
			// Token: 0x0400D2D5 RID: 53973
			Hit,
			// Token: 0x0400D2D6 RID: 53974
			Miss
		}
	}
}
