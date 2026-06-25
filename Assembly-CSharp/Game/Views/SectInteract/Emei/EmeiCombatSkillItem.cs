using System;
using Game.Components.Common;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Emei
{
	// Token: 0x020009EF RID: 2543
	public class EmeiCombatSkillItem : CombatSkillSelectable
	{
		// Token: 0x06007D18 RID: 32024 RVA: 0x003A20EC File Offset: 0x003A02EC
		public override void Set(CombatSkillDisplayDataForList data, Action<short> onSelect, bool init)
		{
			base.Set(data, onSelect, init);
			this.bonus.SetActive(data.EmeiBonus1 >= 0);
			this.bonus2.SetActive(data.EmeiBonus2 >= 0);
			this.bonusText.text = (((data.EmeiBonus1 >= 0) ? 1 : 0) + ((data.EmeiBonus2 >= 0) ? 1 : 0)).ToString();
		}

		// Token: 0x04005F27 RID: 24359
		public GameObject bonus;

		// Token: 0x04005F28 RID: 24360
		public GameObject bonus2;

		// Token: 0x04005F29 RID: 24361
		public TextMeshProUGUI bonusText;
	}
}
