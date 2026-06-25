using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;

namespace Game.Views.CombatSkillTree
{
	// Token: 0x02000AE6 RID: 2790
	public class CombatSkillTreeSkillLine : MonoBehaviour
	{
		// Token: 0x17000F21 RID: 3873
		// (get) Token: 0x06008915 RID: 35093 RVA: 0x003F7301 File Offset: 0x003F5501
		private ViewCombatSkillTree ViewCombatSkillTree
		{
			get
			{
				return UIElement.CombatSkillTree.UiBaseAs<ViewCombatSkillTree>();
			}
		}

		// Token: 0x06008916 RID: 35094 RVA: 0x003F7310 File Offset: 0x003F5510
		public void Set(sbyte visibleLevel, sbyte skillType, CombatSkillTypeItem config, List<CombatSkillItem> combatSkillItemList, List<CombatSkillDisplayData> LearnedSkills)
		{
			CombatSkillTreeSkillLine.<>c__DisplayClass7_0 CS$<>8__locals1 = new CombatSkillTreeSkillLine.<>c__DisplayClass7_0();
			CS$<>8__locals1.combatSkillItemList = combatSkillItemList;
			this.skillTypeIcon.SetSprite("ui9_back_attainments_combat_0_" + skillType.ToString(), false, null);
			this.skillTypeName.text = config.Name;
			CommonUtils.PrepareEnoughChildren(this.skillRoot, this.templateSkillItem.gameObject, CS$<>8__locals1.combatSkillItemList.Count, null);
			int i;
			int j;
			for (i = 0; i < CS$<>8__locals1.combatSkillItemList.Count; i = j + 1)
			{
				bool flag = this._skillItems[i] == null;
				if (flag)
				{
					this._skillItems[i] = this.skillRoot.GetChild(i).GetComponent<CombatSkillTreeSkillItem>();
				}
				CombatSkillDisplayData combatSkillDisplayData = (LearnedSkills != null) ? LearnedSkills.Find((CombatSkillDisplayData v) => v.TemplateId == CS$<>8__locals1.combatSkillItemList[i].TemplateId) : null;
				bool isVisible = !CS$<>8__locals1.combatSkillItemList[i].IsNonPublic || (int)visibleLevel >= i || combatSkillDisplayData != null;
				this._skillItems[i].Set(isVisible, CS$<>8__locals1.combatSkillItemList[i], combatSkillDisplayData);
				j = i;
			}
		}

		// Token: 0x0400690C RID: 26892
		[SerializeField]
		private CombatSkillTreeSkillItem templateSkillItem;

		// Token: 0x0400690D RID: 26893
		[SerializeField]
		private CImage skillTypeIcon;

		// Token: 0x0400690E RID: 26894
		[SerializeField]
		private TextMeshProUGUI skillTypeName;

		// Token: 0x0400690F RID: 26895
		[SerializeField]
		private RectTransform skillRoot;

		// Token: 0x04006910 RID: 26896
		private readonly CombatSkillTreeSkillItem[] _skillItems = new CombatSkillTreeSkillItem[9];
	}
}
