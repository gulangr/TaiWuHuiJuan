using System;
using Config;
using TMPro;

// Token: 0x020001EC RID: 492
public static class CombatSkillViewExtension
{
	// Token: 0x06002049 RID: 8265 RVA: 0x000EB4E4 File Offset: 0x000E96E4
	public static void SetData(this CombatSkillView view, short templateId)
	{
		CombatSkillItem configData = CombatSkill.Instance[templateId];
		view.CGet<TextMeshProUGUI>("Name").text = configData.Name;
		view.CGet<CImage>("FiveElementsType").enabled = false;
		view.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(configData.Grade), false, null);
		view.CGet<TextMeshProUGUI>("Grade").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", configData.Grade));
		view.CGet<CImage>("SectType").SetSprite(CombatSkillView.EquipTypeImg[configData.EquipType], true, null);
		view.CGet<CImage>("SkillType").SetSprite(configData.Icon, true, null);
		view.CGet<CImage>("SkillType").SetColor(Colors.Instance.FiveElementsColors[(int)configData.FiveElements]);
	}
}
