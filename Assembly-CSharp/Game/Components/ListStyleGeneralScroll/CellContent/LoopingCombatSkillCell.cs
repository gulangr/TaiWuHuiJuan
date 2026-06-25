using System;
using Config;
using FrameWork;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EC4 RID: 3780
	public class LoopingCombatSkillCell : MonoBehaviour, ICellContent<CombatSkillDisplayDataCharacterMenuListItem>, ICellContent
	{
		// Token: 0x0600AEFD RID: 44797 RVA: 0x004FBAC8 File Offset: 0x004F9CC8
		public void SetData(CombatSkillDisplayDataCharacterMenuListItem data)
		{
			CombatSkillItem config = CombatSkill.Instance[data.TemplateId];
			Color gradeColor = Colors.Instance.GradeColors[(int)config.Grade];
			this.skillName.text = config.Name.SetColor(gradeColor);
			this.skillIcon.SetSprite(config.Icon, false, null);
			this.skillIcon.SetColor(CommonUtils.GetFiveElementColor((int)config.FiveElements));
			this.skillFrame.SetSprite(LoopingCombatSkillCell.FramePaths[(int)config.EquipType] + config.Grade.ToString(), false, null);
			int remainingNeili = (int)(data.MaxObtainableNeili - data.ObtainedNeili);
			this.status.gameObject.SetActive(remainingNeili <= 0);
			bool flag = this.tipDisplayer;
			if (flag)
			{
				this.tipDisplayer.Type = TipType.CombatSkill;
				this.tipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CombatSkillId", data.TemplateId).Set("CharId", SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
			}
			this.fiveElementsFrame.gameObject.SetActive(false);
		}

		// Token: 0x04008767 RID: 34663
		[SerializeField]
		private CImage skillFrame;

		// Token: 0x04008768 RID: 34664
		[SerializeField]
		private CImage skillIcon;

		// Token: 0x04008769 RID: 34665
		[SerializeField]
		private TextMeshProUGUI skillName;

		// Token: 0x0400876A RID: 34666
		[SerializeField]
		private CImage status;

		// Token: 0x0400876B RID: 34667
		[SerializeField]
		private TooltipInvoker tipDisplayer;

		// Token: 0x0400876C RID: 34668
		[SerializeField]
		private CImage fiveElementsFrame;

		// Token: 0x0400876D RID: 34669
		private static readonly string[] FramePaths = new string[]
		{
			"ui9_icon_combat_skill_type_neigong_",
			"ui9_icon_combat_skill_type_attack_",
			"ui9_icon_combat_skill_type_agile_",
			"ui9_icon_combat_skill_type_defense_",
			"ui9_icon_combat_skill_type_assist_"
		};
	}
}
