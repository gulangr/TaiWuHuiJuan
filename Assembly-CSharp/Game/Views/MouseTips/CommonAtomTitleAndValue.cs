using System;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000825 RID: 2085
	public class CommonAtomTitleAndValue : CommonAtomBase
	{
		// Token: 0x0600664D RID: 26189 RVA: 0x002EB554 File Offset: 0x002E9754
		public override void SetMarginLeft(int marginLeft)
		{
			bool flag = !this._cachedPosition;
			if (flag)
			{
				this._defaultAnchoredPosition = this.leftRoot.anchoredPosition;
				this._cachedPosition = true;
			}
			this.leftRoot.anchoredPosition = new Vector2(this._defaultAnchoredPosition.x + (float)marginLeft, this._defaultAnchoredPosition.y);
		}

		// Token: 0x0600664E RID: 26190 RVA: 0x002EB5B3 File Offset: 0x002E97B3
		public void SetTexts(string title, string value)
		{
			CommonAtomBase.SetLabelText(this.titleLabel, title);
			CommonAtomBase.SetLabelText(this.valueLabel, value);
		}

		// Token: 0x0400478F RID: 18319
		[SerializeField]
		private RectTransform leftRoot;

		// Token: 0x04004790 RID: 18320
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x04004791 RID: 18321
		[SerializeField]
		private TextMeshProUGUI valueLabel;

		// Token: 0x04004792 RID: 18322
		private bool _cachedPosition;

		// Token: 0x04004793 RID: 18323
		private Vector2 _defaultAnchoredPosition;
	}
}
