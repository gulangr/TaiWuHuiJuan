using System;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;

namespace Game.Views.Main.Inscription
{
	// Token: 0x02000979 RID: 2425
	public class CheckInscriptionSkillItem : MonoBehaviour
	{
		// Token: 0x06007450 RID: 29776 RVA: 0x00362D98 File Offset: 0x00360F98
		public void SetLifeSkill(int index, int value)
		{
			this.icon.SetSprite(string.Format("{0}{1}", "ui9_back_attainments_life_3_", index), false, null);
			this.title.text = LocalStringManager.Get(string.Format("LK_LifeSkillType_{0}", index));
			this.valueText.text = value.ToString();
			LifeSkillTypeItem config = LifeSkillType.Instance[index];
			bool flag = config != null && this.tipDisplayer != null;
			if (flag)
			{
				this.tipDisplayer.enabled = true;
				this.tipDisplayer.Type = TipType.Simple;
				TooltipInvoker tooltipInvoker = this.tipDisplayer;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.tipDisplayer.RuntimeParam.Set("arg0", config.Name).Set("arg1", config.Desc);
			}
		}

		// Token: 0x06007451 RID: 29777 RVA: 0x00362E84 File Offset: 0x00361084
		public void SetCombatSkill(int index, int value)
		{
			this.icon.SetSprite(string.Format("{0}{1}", "ui9_back_attainments_combat_3_", index), false, null);
			this.title.text = LocalStringManager.Get(string.Format("LK_CombatSkillType_{0}", index));
			this.valueText.text = value.ToString();
			CombatSkillTypeItem config = CombatSkillType.Instance[index];
			bool flag = config != null && this.tipDisplayer != null;
			if (flag)
			{
				this.tipDisplayer.enabled = true;
				this.tipDisplayer.Type = TipType.Simple;
				TooltipInvoker tooltipInvoker = this.tipDisplayer;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.tipDisplayer.RuntimeParam.Set("arg0", config.Name).Set("arg1", config.Desc);
			}
		}

		// Token: 0x040056DF RID: 22239
		[SerializeField]
		private CImage icon;

		// Token: 0x040056E0 RID: 22240
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x040056E1 RID: 22241
		[SerializeField]
		private TextMeshProUGUI valueText;

		// Token: 0x040056E2 RID: 22242
		[SerializeField]
		private TooltipInvoker tipDisplayer;
	}
}
