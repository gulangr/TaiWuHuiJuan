using System;
using TMPro;
using UnityEngine;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F4E RID: 3918
	public class LifeRecordSummaryYearComplex : MonoBehaviour
	{
		// Token: 0x17001460 RID: 5216
		// (get) Token: 0x0600B3BF RID: 46015 RVA: 0x0051CE8C File Offset: 0x0051B08C
		public RectTransform Complex
		{
			get
			{
				return this.yearLabel.transform as RectTransform;
			}
		}

		// Token: 0x0600B3C0 RID: 46016 RVA: 0x0051CEA0 File Offset: 0x0051B0A0
		public void Set(int year, int baseScore, int minScore, int maxScore, string label)
		{
			base.gameObject.SetActive(true);
			TMP_Text tmp_Text = this.yearSummary;
			CImage cimage = this.complexBg;
			if (!true)
			{
			}
			ValueTuple<string, Sprite> valueTuple;
			if (baseScore < 50)
			{
				if (baseScore >= 0)
				{
					valueTuple = new ValueTuple<string, Sprite>(LanguageKey.UI_LifeRecord_Year_Bad.Tr(), this.icons[0]);
					goto IL_9F;
				}
			}
			else if (baseScore <= 100)
			{
				if (baseScore != 50)
				{
					valueTuple = new ValueTuple<string, Sprite>(LanguageKey.UI_LifeRecord_Year_Good.Tr(), this.icons[2]);
					goto IL_9F;
				}
				valueTuple = new ValueTuple<string, Sprite>(LanguageKey.UI_LifeRecord_Year_Normal.Tr(), this.icons[1]);
				goto IL_9F;
			}
			valueTuple = new ValueTuple<string, Sprite>(LanguageKey.LK_LifeRecord_None.Tr(), this.icons[1]);
			IL_9F:
			if (!true)
			{
			}
			ValueTuple<string, Sprite> valueTuple2 = valueTuple;
			tmp_Text.text = valueTuple2.Item1;
			cimage.sprite = valueTuple2.Item2;
			this.yearLabel.text = label;
			bool flag = baseScore == -1;
			if (flag)
			{
				baseScore = 50;
			}
			float complexPercent = ((float)(baseScore - minScore) + 0.5f) / (float)(maxScore - minScore + 1);
			this.complex.anchorMin = this.complex.anchorMin.SetY(complexPercent);
			this.complex.anchorMax = this.complex.anchorMax.SetY(complexPercent);
			this.complex.pivot = this.complex.pivot.SetY(complexPercent);
			this.yearText.text = TimeManager.GetYearDisplayString(year * 12 - 1);
			this.complexBg.rectTransform.sizeDelta = this.complexBg.rectTransform.sizeDelta.SetX((LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? this.cnWidth : this.enWidth);
			this.yearText.rectTransform.sizeDelta = this.yearText.rectTransform.sizeDelta.SetX((LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? this.cnWidth : this.enWidth);
			this.yearSummary.rectTransform.sizeDelta = this.yearSummary.rectTransform.sizeDelta.SetX((LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? this.cnWidth : this.enWidth);
		}

		// Token: 0x04008BC2 RID: 35778
		[SerializeField]
		private Sprite[] icons;

		// Token: 0x04008BC3 RID: 35779
		[SerializeField]
		private RectTransform complex;

		// Token: 0x04008BC4 RID: 35780
		[SerializeField]
		private RectTransform self;

		// Token: 0x04008BC5 RID: 35781
		[SerializeField]
		private CImage complexBg;

		// Token: 0x04008BC6 RID: 35782
		[SerializeField]
		private TMP_Text yearSummary;

		// Token: 0x04008BC7 RID: 35783
		[SerializeField]
		private TMP_Text yearLabel;

		// Token: 0x04008BC8 RID: 35784
		[SerializeField]
		private TMP_Text yearText;

		// Token: 0x04008BC9 RID: 35785
		[SerializeField]
		private float cnWidth = 100f;

		// Token: 0x04008BCA RID: 35786
		[SerializeField]
		private float enWidth = 192f;
	}
}
