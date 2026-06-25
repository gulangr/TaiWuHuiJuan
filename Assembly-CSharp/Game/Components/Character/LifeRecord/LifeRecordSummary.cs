using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F4D RID: 3917
	public class LifeRecordSummary : MonoBehaviour
	{
		// Token: 0x1700145F RID: 5215
		// (get) Token: 0x0600B3B6 RID: 46006 RVA: 0x0051C9F3 File Offset: 0x0051ABF3
		// (set) Token: 0x0600B3B7 RID: 46007 RVA: 0x0051C9FC File Offset: 0x0051ABFC
		public bool IsSimple
		{
			get
			{
				return this.isSimple;
			}
			set
			{
				bool flag = this.isSimple == value;
				if (!flag)
				{
					this.isSimple = value;
					this.SetYear(this._currYear);
				}
			}
		}

		// Token: 0x0600B3B8 RID: 46008 RVA: 0x0051CA2D File Offset: 0x0051AC2D
		private void Awake()
		{
			this.modeSwitcher.onClick.ResetListener(delegate()
			{
				this.IsSimple = !this.IsSimple;
			});
		}

		// Token: 0x0600B3B9 RID: 46009 RVA: 0x0051CA50 File Offset: 0x0051AC50
		public void Set(IList<int> years, IList<int> scores, IList<string> importantThings)
		{
			this.simpleLayout.spacing = (this.complexLayout.spacing = (float)((LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? this.cnSpace : this.enSpace));
			this._years = years.ToArray<int>();
			this._scores = scores.ToArray<int>();
			this._importantThings = importantThings.ToArray<string>();
			this.SetYear(this._years[0]);
		}

		// Token: 0x0600B3BA RID: 46010 RVA: 0x0051CAC2 File Offset: 0x0051ACC2
		public void SetDate(int date)
		{
			this.SetYear(date / 12);
		}

		// Token: 0x0600B3BB RID: 46011 RVA: 0x0051CAD0 File Offset: 0x0051ACD0
		public void SetYear(int year)
		{
			this._currYear = year;
			int index = Array.BinarySearch<int>(this._years, year + 1, LifeRecordSummary.Cmp);
			bool flag = index < 0;
			if (flag)
			{
				index = ~index;
			}
			int len = (LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? this.cnComponents : this.enComponents;
			int yearStartIndex = Math.Max(0, index + 1 - len);
			int yearEndIndex = Math.Min(this._years.Length, yearStartIndex + len);
			int[] scores = RuntimeHelpers.GetSubArray<int>(this._scores, new Range(yearStartIndex, yearEndIndex));
			int[] years = RuntimeHelpers.GetSubArray<int>(this._years, new Range(yearStartIndex, yearEndIndex));
			string[] importantThings = RuntimeHelpers.GetSubArray<string>(this._importantThings, new Range(yearStartIndex, yearEndIndex));
			this.simple.gameObject.SetActive(this.isSimple);
			this.complex.gameObject.SetActive(!this.isSimple);
			this.btnMore.transform.localScale = new Vector3((float)(this.isSimple ? -1 : 1), 1f, 1f);
			this.btnMoreText.text = LocalStringManager.Get(this.isSimple ? LanguageKey.Lk_LifeRecords_Unfold : LanguageKey.Lk_LifeRecords_Fold);
			for (index = 0; index < scores.Length; index++)
			{
				this.simples[index].Set(years[years.Length - 1 - index], scores[scores.Length - 1 - index]);
			}
			while (index < this.simples.Length)
			{
				this.simples[index].gameObject.SetActive(false);
				index++;
			}
			index = 0;
			int min = (from x in scores
			select (x == -1) ? 50 : x).Append(50).Min();
			int max = scores.Append(50).Max();
			int end = Math.Min(scores.Length, importantThings.Length) - 1;
			while (index <= end)
			{
				this.complexes[index].Set(years[end - index], scores[end - index], min, max, importantThings[end - index]);
				bool flag2 = scores[end - index] == -1;
				if (flag2)
				{
					scores[end - index] = 50;
				}
				index++;
			}
			bool flag3 = !this.isSimple;
			if (flag3)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.complex);
			}
			for (index = 0; index <= end; index++)
			{
				bool flag4 = index > 0;
				if (flag4)
				{
					this.lines[index - 1].Set(this.complexes[index - 1].Complex, this.complexes[index].Complex, (scores[end - (index - 1)] < scores[end - index]) ? 2 : ((scores[end - (index - 1)] > scores[end - index]) ? 0 : 1));
				}
			}
			while (index < this.complexes.Length)
			{
				this.complexes[index].gameObject.SetActive(false);
				this.lines[index - 1].gameObject.SetActive(false);
				index++;
			}
		}

		// Token: 0x04008BAE RID: 35758
		[SerializeField]
		private bool isSimple;

		// Token: 0x04008BAF RID: 35759
		[SerializeField]
		private RectTransform simple;

		// Token: 0x04008BB0 RID: 35760
		[SerializeField]
		private RectTransform complex;

		// Token: 0x04008BB1 RID: 35761
		[SerializeField]
		private LifeRecordSummaryYearSimple[] simples;

		// Token: 0x04008BB2 RID: 35762
		[SerializeField]
		private LifeRecordSummaryYearComplex[] complexes;

		// Token: 0x04008BB3 RID: 35763
		[SerializeField]
		private LifeRecordSummaryYearComplexLine[] lines;

		// Token: 0x04008BB4 RID: 35764
		[SerializeField]
		private CButton modeSwitcher;

		// Token: 0x04008BB5 RID: 35765
		[SerializeField]
		private GameObject btnMore;

		// Token: 0x04008BB6 RID: 35766
		[SerializeField]
		private TextMeshProUGUI btnMoreText;

		// Token: 0x04008BB7 RID: 35767
		[SerializeField]
		private int cnComponents = 9;

		// Token: 0x04008BB8 RID: 35768
		[SerializeField]
		private int enComponents = 8;

		// Token: 0x04008BB9 RID: 35769
		[SerializeField]
		private int cnSpace = 110;

		// Token: 0x04008BBA RID: 35770
		[SerializeField]
		private int enSpace = 140;

		// Token: 0x04008BBB RID: 35771
		[SerializeField]
		private HorizontalLayoutGroup simpleLayout;

		// Token: 0x04008BBC RID: 35772
		[SerializeField]
		private HorizontalLayoutGroup complexLayout;

		// Token: 0x04008BBD RID: 35773
		private int[] _years = new int[0];

		// Token: 0x04008BBE RID: 35774
		private int[] _scores = new int[0];

		// Token: 0x04008BBF RID: 35775
		private string[] _importantThings = new string[0];

		// Token: 0x04008BC0 RID: 35776
		private int _currYear;

		// Token: 0x04008BC1 RID: 35777
		private static readonly IComparer<int> Cmp = Comparer<int>.Create((int x, int y) => y.CompareTo(x));
	}
}
