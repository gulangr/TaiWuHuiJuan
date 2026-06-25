using System;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000880 RID: 2176
	public class TooltipCombatSkillBodyPartGroup : MonoBehaviour
	{
		// Token: 0x17000C80 RID: 3200
		// (get) Token: 0x060068D8 RID: 26840 RVA: 0x003030DB File Offset: 0x003012DB
		public Transform ItemContainer
		{
			get
			{
				return this.itemContainer;
			}
		}

		// Token: 0x17000C81 RID: 3201
		// (get) Token: 0x060068D9 RID: 26841 RVA: 0x003030E3 File Offset: 0x003012E3
		public TooltipCombatSkillBodyPartItem ItemTemplate
		{
			get
			{
				return this.itemTemplate;
			}
		}

		// Token: 0x17000C82 RID: 3202
		// (get) Token: 0x060068DA RID: 26842 RVA: 0x003030EB File Offset: 0x003012EB
		public GameObject PipeDivider
		{
			get
			{
				return this.pipeDivider;
			}
		}

		// Token: 0x060068DB RID: 26843 RVA: 0x003030F3 File Offset: 0x003012F3
		public void SetPipeDividerVisible(bool visible)
		{
			this.pipeDivider.SetActive(visible);
		}

		// Token: 0x04004AEA RID: 19178
		[SerializeField]
		private Transform itemContainer;

		// Token: 0x04004AEB RID: 19179
		[SerializeField]
		private TooltipCombatSkillBodyPartItem itemTemplate;

		// Token: 0x04004AEC RID: 19180
		[SerializeField]
		private GameObject pipeDivider;
	}
}
