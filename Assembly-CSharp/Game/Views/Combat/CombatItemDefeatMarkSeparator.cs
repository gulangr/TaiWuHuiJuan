using System;
using GameData.Combat.Math;
using GameData.Domains.Combat;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B2F RID: 2863
	public class CombatItemDefeatMarkSeparator : MonoBehaviour
	{
		// Token: 0x06008C3E RID: 35902 RVA: 0x0040CA2C File Offset: 0x0040AC2C
		public void Set(CombatType combatType, DefeatMarkCollection markCollection)
		{
			byte requireCount = GlobalConfig.NeedDefeatMarkCount[(int)combatType];
			int markCount = markCollection.GetTotalCount();
			bool effectOn = markCount >= (int)(requireCount / 2);
			this.effect.SetActive(effectOn);
			this.separator.SetActive(!effectOn);
			bool flag = effectOn;
			if (flag)
			{
				this.effectAnimation.SetFrameSpeed(this.CalcFrameSpeed(markCount, (int)requireCount));
			}
		}

		// Token: 0x06008C3F RID: 35903 RVA: 0x0040CA8C File Offset: 0x0040AC8C
		private sbyte CalcFrameSpeed(int markCount, int requireCount)
		{
			int half = requireCount / 2;
			CValuePercent percent = CValuePercent.Parse(markCount - half, requireCount - half);
			return (sbyte)Mathf.Clamp((int)this.minSpeed + (int)(this.maxSpeed - this.minSpeed) * percent, (int)this.minSpeed, (int)this.maxSpeed);
		}

		// Token: 0x04006B52 RID: 27474
		[SerializeField]
		private sbyte minSpeed = 20;

		// Token: 0x04006B53 RID: 27475
		[SerializeField]
		private sbyte maxSpeed = 36;

		// Token: 0x04006B54 RID: 27476
		[SerializeField]
		private GameObject separator;

		// Token: 0x04006B55 RID: 27477
		[SerializeField]
		private GameObject effect;

		// Token: 0x04006B56 RID: 27478
		[SerializeField]
		private FrameAnimation effectAnimation;
	}
}
