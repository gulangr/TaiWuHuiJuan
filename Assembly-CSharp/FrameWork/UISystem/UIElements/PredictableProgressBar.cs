using System;
using UnityEngine;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x02000FFD RID: 4093
	public class PredictableProgressBar : Refers
	{
		// Token: 0x0600BAC6 RID: 47814 RVA: 0x00551119 File Offset: 0x0054F319
		public void SetPredictProgress(float percent)
		{
			this._predictImage.fillAmount = percent;
		}

		// Token: 0x0600BAC7 RID: 47815 RVA: 0x00551129 File Offset: 0x0054F329
		public void SetProgressValue(float percent)
		{
			this._fillImage.fillAmount = percent;
		}

		// Token: 0x04009049 RID: 36937
		[SerializeField]
		private CImage _fillImage;

		// Token: 0x0400904A RID: 36938
		[SerializeField]
		private CImage _predictImage;
	}
}
