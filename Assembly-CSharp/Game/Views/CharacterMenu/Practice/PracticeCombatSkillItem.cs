using System;
using Game.Components.Common;
using GameData.Domains.CombatSkill;
using UnityEngine;

namespace Game.Views.CharacterMenu.Practice
{
	// Token: 0x02000BB6 RID: 2998
	public class PracticeCombatSkillItem : CombatSkillSelectable
	{
		// Token: 0x060096E6 RID: 38630 RVA: 0x00465EF4 File Offset: 0x004640F4
		public override void Set(CombatSkillDisplayDataForList data, Action<short> onSelect, bool init)
		{
			base.Set(data, onSelect, init);
			bool canBreak = !CombatSkillStateHelper.IsBrokenOut(data.ActivationState) && CombatSkillStateHelper.HasReadOutlinePages(data.ReadingState) && CombatSkillStateHelper.IsReadNormalPagesMeetConditionOfBreakout(data.ReadingState) && !data.Revoked;
			this.breakableMark.SetActive(canBreak);
		}

		// Token: 0x040073C7 RID: 29639
		[SerializeField]
		private GameObject breakableMark;
	}
}
