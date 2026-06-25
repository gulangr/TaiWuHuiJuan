using System;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat.Migrate
{
	// Token: 0x02000B5B RID: 2907
	public class CombatWeaponPrefab : MonoBehaviour
	{
		// Token: 0x04006E07 RID: 28167
		public int UserInt;

		// Token: 0x04006E08 RID: 28168
		public CImage highLight;

		// Token: 0x04006E09 RID: 28169
		public GameObject usingGo;

		// Token: 0x04006E0A RID: 28170
		public CImage icon;

		// Token: 0x04006E0B RID: 28171
		public CImage cdProgress;

		// Token: 0x04006E0C RID: 28172
		public CImage lockProgress;

		// Token: 0x04006E0D RID: 28173
		public GameObject outAttackRange;

		// Token: 0x04006E0E RID: 28174
		public RectTransform outAttackRangeLine;

		// Token: 0x04006E0F RID: 28175
		public TextMeshProUGUI countDownText;

		// Token: 0x04006E10 RID: 28176
		public CImage iconBack;

		// Token: 0x04006E11 RID: 28177
		public TextMeshProUGUI currDurability;

		// Token: 0x04006E12 RID: 28178
		public TextMeshProUGUI maxDurability;

		// Token: 0x04006E13 RID: 28179
		public CombatWeaponUnlockHolderPrefab unlockHolder;
	}
}
