using System;
using Config;
using FrameWork;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;

// Token: 0x020001F0 RID: 496
public class SmallCombatSkillView : MonoBehaviour
{
	// Token: 0x06002075 RID: 8309 RVA: 0x000EC618 File Offset: 0x000EA818
	public void SetData(CombatSkillDisplayData skillData)
	{
		short templateId = skillData.TemplateId;
		this.SetByCombatSkillTemplateId(templateId);
		this.UpdateMouseTip(skillData);
	}

	// Token: 0x06002076 RID: 8310 RVA: 0x000EC640 File Offset: 0x000EA840
	public void SetByCombatSkillTemplateId(short templateId)
	{
		CombatSkillItem configData = CombatSkill.Instance[templateId];
		this.gradeBack.SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
		this.grade.text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
		this.sectType.SetSprite(CombatSkillView.EquipTypeImg[configData.EquipType], true, null);
		this.skillType.SetSprite(configData.Icon, true, null);
		this.skillType.SetColor(Colors.Instance.FiveElementsColors[(int)configData.FiveElements]);
	}

	// Token: 0x06002077 RID: 8311 RVA: 0x000EC6EC File Offset: 0x000EA8EC
	private void UpdateMouseTip(CombatSkillDisplayData skillData)
	{
		TooltipInvoker mouseTip = base.GetComponent<TooltipInvoker>();
		bool flag = !this.EnableTips;
		if (flag)
		{
			mouseTip.enabled = false;
		}
		else
		{
			mouseTip.Type = TipType.CombatSkill;
			mouseTip.enabled = (skillData != null);
			bool flag2 = skillData == null;
			if (!flag2)
			{
				mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CombatSkillId", skillData.TemplateId).Set("CharId", skillData.CharId);
				bool showing = mouseTip.Showing;
				if (showing)
				{
					mouseTip.Refresh(false, -1);
				}
			}
		}
	}

	// Token: 0x04001882 RID: 6274
	[SerializeField]
	private CImage gradeBack;

	// Token: 0x04001883 RID: 6275
	[SerializeField]
	private CImage sectType;

	// Token: 0x04001884 RID: 6276
	[SerializeField]
	private CImage skillType;

	// Token: 0x04001885 RID: 6277
	[SerializeField]
	private TextMeshProUGUI grade;

	// Token: 0x04001886 RID: 6278
	public bool EnableTips = true;
}
