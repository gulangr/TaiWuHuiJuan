using System;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AF2 RID: 2802
	public class CombatResultWinLose : MonoBehaviour
	{
		// Token: 0x060089B0 RID: 35248 RVA: 0x003FC1B0 File Offset: 0x003FA3B0
		public void Set(bool isWin)
		{
			this.effectImage.texture = (isWin ? this.winEffect : this.loseEffect);
			this.winCorner.gameObject.SetActive(isWin);
			this.loseCorner.gameObject.SetActive(!isWin);
		}

		// Token: 0x04006997 RID: 27031
		[SerializeField]
		private CRawImage effectImage;

		// Token: 0x04006998 RID: 27032
		[SerializeField]
		private GameObject winCorner;

		// Token: 0x04006999 RID: 27033
		[SerializeField]
		private GameObject loseCorner;

		// Token: 0x0400699A RID: 27034
		[SerializeField]
		private Texture winEffect;

		// Token: 0x0400699B RID: 27035
		[SerializeField]
		private Texture loseEffect;
	}
}
