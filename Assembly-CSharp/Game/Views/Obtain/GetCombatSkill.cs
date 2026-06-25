using System;
using Game.Views.CharacterMenu;
using GameData.Domains.CombatSkill;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Obtain
{
	// Token: 0x020007D5 RID: 2005
	public class GetCombatSkill : MonoBehaviour
	{
		// Token: 0x17000BDD RID: 3037
		// (get) Token: 0x060061D6 RID: 25046 RVA: 0x002CE341 File Offset: 0x002CC541
		private RectTransform SkillRectTransform
		{
			get
			{
				return this.combatSkill.GetComponent<RectTransform>();
			}
		}

		// Token: 0x060061D7 RID: 25047 RVA: 0x002CE350 File Offset: 0x002CC550
		public void Set(CombatSkillDisplayData data)
		{
			this.combatSkill.Set(data);
			this.SkillRectTransform.anchoredPosition = this.SkillRectTransform.anchoredPosition.SetY(-555f);
			this.fail.SetActive(false);
			this.proficiency.SetActive(false);
			this.power.SetActive(false);
		}

		// Token: 0x060061D8 RID: 25048 RVA: 0x002CE3B4 File Offset: 0x002CC5B4
		public void Set(CombatSkillDisplayData data, IntPair combatSkillProficiency)
		{
			this.combatSkill.Set(data);
			this.prevProficiency.text = string.Format("{0}/{1}", combatSkillProficiency.First, 300);
			this.currProficiency.text = string.Format("{0}/{1}", combatSkillProficiency.Second, 300);
			this.SkillRectTransform.anchoredPosition = this.SkillRectTransform.anchoredPosition.SetY(-555f);
			this.fail.SetActive(false);
			this.proficiency.SetActive(true);
			this.power.SetActive(false);
		}

		// Token: 0x060061D9 RID: 25049 RVA: 0x002CE470 File Offset: 0x002CC670
		public void Set(CombatSkillDisplayData data, int powerValue)
		{
			this.combatSkill.Set(data);
			this.powerText.text = CommonUtils.GetBreakoutMaxPowerName(powerValue, data.TemplateId)[0];
			this.SkillRectTransform.anchoredPosition = this.SkillRectTransform.anchoredPosition.SetY(-555f);
			this.fail.SetActive(false);
			this.proficiency.SetActive(false);
			this.power.SetActive(true);
		}

		// Token: 0x060061DA RID: 25050 RVA: 0x002CE4F0 File Offset: 0x002CC6F0
		public void Set(CombatSkillDisplayData data, bool _)
		{
			this.combatSkill.Set(data);
			this.SkillRectTransform.anchoredPosition = this.SkillRectTransform.anchoredPosition.SetY(-623f);
			this.fail.SetActive(true);
			this.proficiency.SetActive(false);
			this.power.SetActive(false);
		}

		// Token: 0x040043EF RID: 17391
		[SerializeField]
		private CharacterMenuCombatSkillItem combatSkill;

		// Token: 0x040043F0 RID: 17392
		[SerializeField]
		private GameObject fail;

		// Token: 0x040043F1 RID: 17393
		[SerializeField]
		private GameObject proficiency;

		// Token: 0x040043F2 RID: 17394
		[SerializeField]
		private GameObject power;

		// Token: 0x040043F3 RID: 17395
		[SerializeField]
		private TextMeshProUGUI prevProficiency;

		// Token: 0x040043F4 RID: 17396
		[SerializeField]
		private TextMeshProUGUI currProficiency;

		// Token: 0x040043F5 RID: 17397
		[SerializeField]
		private TextMeshProUGUI powerText;

		// Token: 0x040043F6 RID: 17398
		private const float FailPosition = -623f;

		// Token: 0x040043F7 RID: 17399
		private const float SuccessPosition = -555f;
	}
}
