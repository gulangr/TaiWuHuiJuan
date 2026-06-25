using System;
using UnityEngine;

namespace Game.Views.Combat.Migrate
{
	// Token: 0x02000B51 RID: 2897
	public class CombatInfoChar : MonoBehaviour
	{
		// Token: 0x04006DB0 RID: 28080
		public CImage mobilityBar;

		// Token: 0x04006DB1 RID: 28081
		public CombatAffectingDefendSkill affectingDefendSkill;

		// Token: 0x04006DB2 RID: 28082
		public RectTransform outOfAttackRangeTips;

		// Token: 0x04006DB3 RID: 28083
		public CombatPrepareProgress prepareProgress;

		// Token: 0x04006DB4 RID: 28084
		public CombatCommonTips commonTips;

		// Token: 0x04006DB5 RID: 28085
		public GameObject mobilityLock;

		// Token: 0x04006DB6 RID: 28086
		public CombatAffectingMoveSkill affectingMoveSkill;

		// Token: 0x04006DB7 RID: 28087
		public CombatMovePrepareProgress movePrepareProgress;

		// Token: 0x04006DB8 RID: 28088
		public RectTransform mobility;

		// Token: 0x04006DB9 RID: 28089
		public RectTransform follow;

		// Token: 0x04006DBA RID: 28090
		public CombatCharacterMind combatCharacterMind;
	}
}
