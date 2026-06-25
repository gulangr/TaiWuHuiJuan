using System;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x02000810 RID: 2064
	public class NewGameSubPageWorldDetailDifficultyLevelItem : MonoBehaviour
	{
		// Token: 0x06006565 RID: 25957 RVA: 0x002E5932 File Offset: 0x002E3B32
		public void SetLocked(bool isLocked)
		{
			this.lockedObj.gameObject.SetActive(isLocked);
			this.tip.enabled = isLocked;
		}

		// Token: 0x040046A4 RID: 18084
		[SerializeField]
		private GameObject lockedObj;

		// Token: 0x040046A5 RID: 18085
		[SerializeField]
		private TooltipInvoker tip;
	}
}
