using System;
using TMPro;
using UnityEngine;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F50 RID: 3920
	public class LifeRecordSummaryYearSimple : MonoBehaviour
	{
		// Token: 0x0600B3C4 RID: 46020 RVA: 0x0051D1D4 File Offset: 0x0051B3D4
		public void Set(int year, int baseScore)
		{
			base.gameObject.SetActive(true);
			TMP_Text tmp_Text = this.yearSummary;
			if (!true)
			{
			}
			string text;
			if (baseScore < 50)
			{
				if (baseScore >= 0)
				{
					text = LanguageKey.UI_LifeRecord_Year_Bad.Tr();
					goto IL_64;
				}
			}
			else if (baseScore <= 100)
			{
				if (baseScore != 50)
				{
					text = LanguageKey.UI_LifeRecord_Year_Good.Tr();
					goto IL_64;
				}
				text = LanguageKey.UI_LifeRecord_Year_Normal.Tr();
				goto IL_64;
			}
			text = LanguageKey.LK_LifeRecord_None.Tr();
			IL_64:
			if (!true)
			{
			}
			tmp_Text.text = text;
			this.yearName.text = TimeManager.GetYearDisplayString(year * 12 - 1);
		}

		// Token: 0x04008BCE RID: 35790
		[SerializeField]
		private RectTransform simple;

		// Token: 0x04008BCF RID: 35791
		[SerializeField]
		private RectTransform self;

		// Token: 0x04008BD0 RID: 35792
		[SerializeField]
		private CImage simpleBg;

		// Token: 0x04008BD1 RID: 35793
		[SerializeField]
		private TMP_Text yearSummary;

		// Token: 0x04008BD2 RID: 35794
		[SerializeField]
		private TMP_Text yearName;
	}
}
