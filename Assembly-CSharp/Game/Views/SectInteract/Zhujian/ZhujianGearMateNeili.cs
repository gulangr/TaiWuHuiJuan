using System;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009CA RID: 2506
	public class ZhujianGearMateNeili : MonoBehaviour
	{
		// Token: 0x060079AC RID: 31148 RVA: 0x0038895C File Offset: 0x00386B5C
		public void Set(float currPercent, float previewPercent, int currVal, int previewVal, int maxVal)
		{
			currVal = Math.Min(maxVal, currVal);
			previewVal = Math.Clamp(previewVal, 0, maxVal - currVal);
			this.progressText.text = ((previewPercent == 0f) ? (((int)(currPercent * 100f)).ToString() + "%").SetColor("PersonalityType_Calm") : ((((int)(currPercent * 100f)).ToString() + "%").SetColor("PersonalityType_Calm") + " + " + (((int)(previewPercent * 100f)).ToString() + "%").SetColor("FiveElementType_Xuanyin")));
			this.progressBar.fillAmount = currPercent + previewPercent;
			this.valueLabel.text = ((previewVal == 0) ? (currVal.ToString() ?? "").SetColor("PersonalityType_Calm") : (currVal.ToString().SetColor("PersonalityType_Calm") + "+" + previewVal.ToString().SetColor("FiveElementType_Xuanyin")));
			this.upgradePreview.gameObject.SetActive(previewPercent > 0f);
		}

		// Token: 0x04005C37 RID: 23607
		[SerializeField]
		private TextMeshProUGUI valueLabel;

		// Token: 0x04005C38 RID: 23608
		[SerializeField]
		private TextMeshProUGUI progressText;

		// Token: 0x04005C39 RID: 23609
		[SerializeField]
		private CImage progressBar;

		// Token: 0x04005C3A RID: 23610
		[SerializeField]
		private CImage upgradePreview;
	}
}
