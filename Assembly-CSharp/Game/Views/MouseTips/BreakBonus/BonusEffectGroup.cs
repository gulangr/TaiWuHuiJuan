using System;
using System.Collections.Generic;
using Config;
using Game.Views.CharacterMenu;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips.BreakBonus
{
	// Token: 0x020008BA RID: 2234
	public class BonusEffectGroup : MonoBehaviour
	{
		// Token: 0x06006A9F RID: 27295 RVA: 0x003139E8 File Offset: 0x00311BE8
		public void Set(IAsyncMethodRequestHandler handler, SkillBreakPlateBonus bonus, List<SkillBreakBonusEffectDisplay> bonusEffectList, sbyte luohanId)
		{
			bool flag = luohanId >= 0;
			if (flag)
			{
				this.nameLabel.text = Luohan.Instance[luohanId].Name.SetGradeColor((int)bonus.Grade);
			}
			else
			{
				SkillBreakPlateUtils.AsyncGetBonusName(handler, bonus, delegate(string bonusName)
				{
					this.nameLabel.text = bonusName.SetGradeColor((int)bonus.Grade);
				});
			}
			for (int i = 0; i < bonusEffectList.Count; i++)
			{
				bool flag2 = i >= this.properties.childCount;
				if (flag2)
				{
					Object.Instantiate<Transform>(this.properties.GetChild(0), this.properties);
				}
				Game.Views.CharacterMenu.SkillBreakBonusEffect item = this.properties.GetChild(i).GetComponent<Game.Views.CharacterMenu.SkillBreakBonusEffect>();
				item.Refresh(bonusEffectList[i], Game.Views.CharacterMenu.SkillBreakBonusEffect.EBonusIconSize.Small);
				item.gameObject.SetActive(true);
			}
			for (int j = bonusEffectList.Count; j < this.properties.childCount; j++)
			{
				this.properties.GetChild(j).gameObject.SetActive(false);
			}
		}

		// Token: 0x04004D09 RID: 19721
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x04004D0A RID: 19722
		[SerializeField]
		private Transform properties;
	}
}
