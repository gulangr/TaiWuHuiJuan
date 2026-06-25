using System;
using GameData.Domains.Character;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B07 RID: 2823
	public class CombatDistanceRange : MonoBehaviour
	{
		// Token: 0x06008AE5 RID: 35557 RVA: 0x00404530 File Offset: 0x00402730
		public void Refresh(OuterAndInnerShorts range, float scale)
		{
			int correctionFactor = this.reverse ? -1 : 1;
			float rangePos = (float)(range.Outer * 5) * scale * (float)correctionFactor;
			float rangeWidth = (float)((range.Inner - range.Outer) * 5) * scale;
			RectTransform rangeTransform = (RectTransform)base.transform;
			rangeTransform.anchoredPosition = rangeTransform.anchoredPosition.SetX(rangePos);
			rangeTransform.SetWidth(rangeWidth);
			this.minRange.text = ((float)range.Outer / 10f).ToString("F1");
			this.maxRange.text = ((float)range.Inner / 10f).ToString("F1");
		}

		// Token: 0x04006A83 RID: 27267
		[SerializeField]
		private bool reverse = true;

		// Token: 0x04006A84 RID: 27268
		[SerializeField]
		private TextMeshProUGUI minRange;

		// Token: 0x04006A85 RID: 27269
		[SerializeField]
		private TextMeshProUGUI maxRange;
	}
}
