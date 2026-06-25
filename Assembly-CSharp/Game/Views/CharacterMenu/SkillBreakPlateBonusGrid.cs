using System;
using System.Collections.Generic;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B8E RID: 2958
	public class SkillBreakPlateBonusGrid : MonoBehaviour
	{
		// Token: 0x060091ED RID: 37357 RVA: 0x0043F21B File Offset: 0x0043D41B
		public void SetTitle(SkillBreakPlateBonus skillBreakPlateBonus)
		{
			this.RefreshTitle(skillBreakPlateBonus);
		}

		// Token: 0x060091EE RID: 37358 RVA: 0x0043F226 File Offset: 0x0043D426
		public void SetTitleText(string titleText, int grade = 0)
		{
			this.title.text = titleText.SetGradeColor(grade);
		}

		// Token: 0x060091EF RID: 37359 RVA: 0x0043F23C File Offset: 0x0043D43C
		public void SetContent(List<SkillBreakBonusEffectDisplay> effectList)
		{
			CommonUtils.PrepareEnoughChildren(this.lineContent, this.bonusEffectTemplate.gameObject, effectList.Count, null);
			for (int i = 0; i < effectList.Count; i++)
			{
				SkillBreakBonusEffect item = this.lineContent.GetChild(i).GetComponent<SkillBreakBonusEffect>();
				item.Refresh(effectList[i], SkillBreakBonusEffect.EBonusIconSize.Big);
				bool flag = i % 2 == 0;
				if (flag)
				{
					item.imageBack.enabled = true;
					item.imageBack.sprite = this.evenSprite;
				}
				else
				{
					item.imageBack.enabled = false;
				}
			}
		}

		// Token: 0x060091F0 RID: 37360 RVA: 0x0043F2E8 File Offset: 0x0043D4E8
		private void RefreshTitle(SkillBreakPlateBonus bonus)
		{
			SkillBreakPlateUtils.AsyncGetBonusName(null, bonus, delegate(string bonusName)
			{
				this.title.text = bonusName.SetGradeColor((int)bonus.Grade);
			});
		}

		// Token: 0x04007073 RID: 28787
		[SerializeField]
		private Sprite evenSprite;

		// Token: 0x04007074 RID: 28788
		[SerializeField]
		private SkillBreakBonusEffect bonusEffectTemplate;

		// Token: 0x04007075 RID: 28789
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x04007076 RID: 28790
		[SerializeField]
		private RectTransform lineContent;
	}
}
