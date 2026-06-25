using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Following
{
	// Token: 0x02000A23 RID: 2595
	public class DeadCharacterDisplay : MonoBehaviour
	{
		// Token: 0x06007F42 RID: 32578 RVA: 0x003B47C8 File Offset: 0x003B29C8
		public void Set(CharacterDisplayData data)
		{
			TMP_Text tmp_Text = this.title;
			LanguageKey languageKey = LanguageKey.LK_Following_Title;
			List<short> ids = data.TitleIds;
			object arg;
			if (ids == null || ids.Count <= 0)
			{
				arg = LanguageKey.LK_Common_None.Tr();
			}
			else
			{
				arg = string.Join(LanguageKey.LK_Comma_Symbol.Tr(), from id in ids
				select CharacterTitle.Instance[id].Name);
			}
			tmp_Text.text = languageKey.TrFormat(arg);
			this.settlement.text = LanguageKey.LK_Following_Settlement.TrFormat(SingletonObject.getInstance<WorldMapModel>().GetSettlementName(data.OrgInfo));
			this.grade.text = LanguageKey.LK_Following_Grade.TrFormat(CommonUtils.GetIdentityString(data.OrgInfo, data.Gender, data.PhysiologicalAge, false));
			this.fame.text = LanguageKey.LK_Following_Fame.TrFormat(CommonUtils.GetFameString(data.FameType));
			this.duration.text = LanguageKey.LK_Following_Duration.TrFormat(data.GraveDuration.ToString());
			this.lifeDuration.text = LanguageKey.LK_Following_Lifespan.TrFormat(TimeManager.GetYearDisplayString(data.BirthDate), TimeManager.GetYearDisplayString(data.DeathDate));
			this.gradeImg.sprite = this.grades[(int)data.OrgInfo.Grade];
			this.fameImg.sprite = (this.fames.CheckIndex((int)data.FameType) ? this.fames[(int)data.FameType] : this.fames[3]);
		}

		// Token: 0x0400615D RID: 24925
		[SerializeField]
		private TMP_Text lifeDuration;

		// Token: 0x0400615E RID: 24926
		[SerializeField]
		private TMP_Text title;

		// Token: 0x0400615F RID: 24927
		[SerializeField]
		private TMP_Text settlement;

		// Token: 0x04006160 RID: 24928
		[SerializeField]
		private TMP_Text grade;

		// Token: 0x04006161 RID: 24929
		[SerializeField]
		private TMP_Text fame;

		// Token: 0x04006162 RID: 24930
		[SerializeField]
		private TMP_Text duration;

		// Token: 0x04006163 RID: 24931
		[SerializeField]
		private Sprite[] grades;

		// Token: 0x04006164 RID: 24932
		[SerializeField]
		private Sprite[] fames;

		// Token: 0x04006165 RID: 24933
		[SerializeField]
		private CImage gradeImg;

		// Token: 0x04006166 RID: 24934
		[SerializeField]
		private CImage fameImg;
	}
}
