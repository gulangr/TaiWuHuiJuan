using System;
using GameData.Domains.Combat;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B29 RID: 2857
	public class CombatDefeatMark : MonoBehaviour
	{
		// Token: 0x06008C18 RID: 35864 RVA: 0x0040BD30 File Offset: 0x00409F30
		public void PlayParticle(DefeatMarkKey markKey, bool isAlly)
		{
			bool combatPureOpen = CombatUtils.CombatPureOpen;
			if (!combatPureOpen)
			{
				CombatUtils.PlayAndHideParticle((markKey.Type == EMarkType.Fatal) ? this.fatalDamageParticle : (isAlly ? this.selfParticle : this.enemyParticle), 2f);
			}
		}

		// Token: 0x04006B37 RID: 27447
		public int UserInt;

		// Token: 0x04006B38 RID: 27448
		public CImage countdown;

		// Token: 0x04006B39 RID: 27449
		[SerializeField]
		private ParticleSystem selfParticle;

		// Token: 0x04006B3A RID: 27450
		[SerializeField]
		private ParticleSystem fatalDamageParticle;

		// Token: 0x04006B3B RID: 27451
		[SerializeField]
		private ParticleSystem enemyParticle;
	}
}
