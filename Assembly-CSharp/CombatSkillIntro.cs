using System;
using Config;
using GameData.Domains.CombatSkill;
using TMPro;

// Token: 0x0200024F RID: 591
public class CombatSkillIntro : Refers
{
	// Token: 0x17000447 RID: 1095
	// (get) Token: 0x06002734 RID: 10036 RVA: 0x00121431 File Offset: 0x0011F631
	private TextMeshProUGUI _nameLabel
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("Name");
		}
	}

	// Token: 0x17000448 RID: 1096
	// (get) Token: 0x06002735 RID: 10037 RVA: 0x0012143E File Offset: 0x0011F63E
	private TextMeshProUGUI _descriptionLabel
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("Description");
		}
	}

	// Token: 0x17000449 RID: 1097
	// (get) Token: 0x06002736 RID: 10038 RVA: 0x0012144B File Offset: 0x0011F64B
	private CombatSkillView _combatSkill
	{
		get
		{
			return base.CGet<CombatSkillView>("CombatSkillView");
		}
	}

	// Token: 0x06002737 RID: 10039 RVA: 0x00121458 File Offset: 0x0011F658
	public void Refresh(CombatSkillDisplayData displayData)
	{
		bool flag = displayData == null;
		if (flag)
		{
			this._combatSkill.SetData(null, true, false, true, false);
			this._nameLabel.text = "";
			this._nameLabel.transform.parent.gameObject.SetActive(false);
			this._descriptionLabel.text = "";
		}
		else
		{
			this._nameLabel.transform.parent.gameObject.SetActive(true);
			CombatSkillItem config = CombatSkill.Instance[displayData.TemplateId];
			this._nameLabel.text = config.Name;
			this._descriptionLabel.text = config.Desc;
			this._combatSkill.SetData(displayData, true, false, true, true);
			this._combatSkill.CGet<PointerTrigger>("CombatSkillView_PointerTrigger").enabled = false;
		}
	}
}
