using System;
using UnityEngine;

namespace Game.Views.Combat.Migrate
{
	// Token: 0x02000B52 RID: 2898
	public class CombatInfoTop : MonoBehaviour
	{
		// Token: 0x04006DBB RID: 28091
		public CombatNeiliAllocation neiliAllocationHolder;

		// Token: 0x04006DBC RID: 28092
		public TooltipInvoker special;

		// Token: 0x04006DBD RID: 28093
		public TooltipInvoker buff;

		// Token: 0x04006DBE RID: 28094
		public TooltipInvoker debuff;

		// Token: 0x04006DBF RID: 28095
		public TooltipInvoker healInjury;

		// Token: 0x04006DC0 RID: 28096
		public TooltipInvoker healPoison;

		// Token: 0x04006DC1 RID: 28097
		public RectTransform wug;

		// Token: 0x04006DC2 RID: 28098
		public TooltipInvoker xiangShuEffect;

		// Token: 0x04006DC3 RID: 28099
		public RectTransform skillEffectHolder;

		// Token: 0x04006DC4 RID: 28100
		public RectTransform defeatMarkHolder;

		// Token: 0x04006DC5 RID: 28101
		public CombatDamageValueHolder damageValueHolder;

		// Token: 0x04006DC6 RID: 28102
		public RectTransform teammateHolder;

		// Token: 0x04006DC7 RID: 28103
		public RectTransform costNeiliAllocation;

		// Token: 0x04006DC8 RID: 28104
		public CombatNeiliType neiliType;

		// Token: 0x04006DC9 RID: 28105
		public CombatConsummateLevel consummate;

		// Token: 0x04006DCA RID: 28106
		public GameObject mixPoison;

		// Token: 0x04006DCB RID: 28107
		public GameObject banned;

		// Token: 0x04006DCC RID: 28108
		public CImage defeatMarkBarBack;

		// Token: 0x04006DCD RID: 28109
		public CombatDefeatMarkCount defeatMarkCount;

		// Token: 0x04006DCE RID: 28110
		public CombatItemDefeatMarkSeparator defeatMarkSeparator;

		// Token: 0x04006DCF RID: 28111
		public CombatDefeatMarkGroup defeatMarkGroup;

		// Token: 0x04006DD0 RID: 28112
		public RectTransform teammateBubbles;

		// Token: 0x04006DD1 RID: 28113
		public GameObject unlockItem;

		// Token: 0x04006DD2 RID: 28114
		public CombatGangqi combatGangqi;
	}
}
