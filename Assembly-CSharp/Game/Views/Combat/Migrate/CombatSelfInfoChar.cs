using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat.Migrate
{
	// Token: 0x02000B57 RID: 2903
	public class CombatSelfInfoChar : CombatInfoChar
	{
		// Token: 0x04006DE3 RID: 28131
		public GameObject moveBackwardTips;

		// Token: 0x04006DE4 RID: 28132
		public GameObject moveForwardTips;

		// Token: 0x04006DE5 RID: 28133
		public GameObject attackTips;

		// Token: 0x04006DE6 RID: 28134
		public TextMeshProUGUI attackTipsKey;

		// Token: 0x04006DE7 RID: 28135
		public GameObject weaponDurabilityNotEnoughTips;

		// Token: 0x04006DE8 RID: 28136
		public CImage costMobility;

		// Token: 0x04006DE9 RID: 28137
		public RectTransform costMobilityBorder;

		// Token: 0x04006DEA RID: 28138
		public RectTransform costMobilityLine;

		// Token: 0x04006DEB RID: 28139
		public CButton attackRecovery;

		// Token: 0x04006DEC RID: 28140
		public GameObject reserveNormalAttack;
	}
}
