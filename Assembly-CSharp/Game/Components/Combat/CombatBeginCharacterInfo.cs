using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using TMPro;
using UnityEngine;

namespace Game.Components.Combat
{
	// Token: 0x02000F01 RID: 3841
	public class CombatBeginCharacterInfo : MonoBehaviour
	{
		// Token: 0x0600B129 RID: 45353 RVA: 0x0050C320 File Offset: 0x0050A520
		private void Awake()
		{
			for (int i = 0; i < 3; i++)
			{
				Transform child = this.teammateHolder.GetChild(i);
				CombatBeginTeammateSelf self = child.GetComponent<CombatBeginTeammateSelf>();
				CombatBeginTeammateEnemy enemy = child.GetComponent<CombatBeginTeammateEnemy>();
				bool flag = self != null;
				if (flag)
				{
					self.commandBubble.GetComponent<RectTransform>().SetParent(this.commandBubbleHolder);
				}
				bool flag2 = enemy != null;
				if (flag2)
				{
					enemy.commandBubble.GetComponent<RectTransform>().SetParent(this.commandBubbleHolder);
				}
			}
		}

		// Token: 0x0400892B RID: 35115
		[SerializeField]
		public Game.Components.Avatar.Avatar avatar;

		// Token: 0x0400892C RID: 35116
		[SerializeField]
		public CButton openCharMenu;

		// Token: 0x0400892D RID: 35117
		[SerializeField]
		public CImage neiliTypeBack;

		// Token: 0x0400892E RID: 35118
		[SerializeField]
		public CImage consummateIcon;

		// Token: 0x0400892F RID: 35119
		[SerializeField]
		public CImage wisdomIcon;

		// Token: 0x04008930 RID: 35120
		[SerializeField]
		public TextMeshProUGUI neiliType;

		// Token: 0x04008931 RID: 35121
		[SerializeField]
		public TextMeshProUGUI consummateLevel;

		// Token: 0x04008932 RID: 35122
		[SerializeField]
		public TextMeshProUGUI charName;

		// Token: 0x04008933 RID: 35123
		[SerializeField]
		public GameObject openCharMenuTips;

		// Token: 0x04008934 RID: 35124
		[SerializeField]
		public RectTransform neiliAllocationHolder;

		// Token: 0x04008935 RID: 35125
		[SerializeField]
		public RectTransform teammateHolder;

		// Token: 0x04008936 RID: 35126
		[SerializeField]
		public TooltipInvoker injuryMouseTip;

		// Token: 0x04008937 RID: 35127
		[SerializeField]
		public CImage neiliProgress;

		// Token: 0x04008938 RID: 35128
		[SerializeField]
		public TextMeshProUGUI txtNeiGong;

		// Token: 0x04008939 RID: 35129
		[SerializeField]
		public TextMeshProUGUI txtJueJi;

		// Token: 0x0400893A RID: 35130
		[SerializeField]
		public TextMeshProUGUI txtFreeWeaponTitle;

		// Token: 0x0400893B RID: 35131
		[SerializeField]
		public RectTransform commandBubbleHolder;
	}
}
