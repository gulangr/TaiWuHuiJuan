using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.LifeSkillCombat
{
	// Token: 0x02000985 RID: 2437
	public class LifeSkillCombatBeginSkillStrategyItem : MonoBehaviour
	{
		// Token: 0x17000D34 RID: 3380
		// (get) Token: 0x0600751A RID: 29978 RVA: 0x00368FCB File Offset: 0x003671CB
		public CToggle Toggle
		{
			get
			{
				return this.toggle;
			}
		}

		// Token: 0x0600751B RID: 29979 RVA: 0x00368FD4 File Offset: 0x003671D4
		public void Set(sbyte skillId, int bit)
		{
			this.skillName.text = LifeSkillType.Instance[skillId].Name;
			this.skillIcon.SetSprite("ui9_icon_life_skill_combat_select_skill_icon_0_" + skillId.ToString(), false, null);
			this.strategyCount.text = this.GetCount(bit).ToString();
			bool flag = this.tips != null;
			if (flag)
			{
				TooltipInvoker tooltipInvoker = this.tips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("LifeSkillType", skillId).Set("UnlockedState", bit);
				}
			}
		}

		// Token: 0x0600751C RID: 29980 RVA: 0x0036907C File Offset: 0x0036727C
		private int GetCount(int bit)
		{
			int res = 0;
			for (int i = 0; i <= 2; i++)
			{
				bool flag = (bit & 1 << i) != 0;
				if (flag)
				{
					res++;
				}
			}
			return res;
		}

		// Token: 0x040057D7 RID: 22487
		[SerializeField]
		private CToggle toggle;

		// Token: 0x040057D8 RID: 22488
		[SerializeField]
		private TextMeshProUGUI skillName;

		// Token: 0x040057D9 RID: 22489
		[SerializeField]
		private CImage skillIcon;

		// Token: 0x040057DA RID: 22490
		[SerializeField]
		private TextMeshProUGUI strategyCount;

		// Token: 0x040057DB RID: 22491
		[SerializeField]
		private TooltipInvoker tips;
	}
}
