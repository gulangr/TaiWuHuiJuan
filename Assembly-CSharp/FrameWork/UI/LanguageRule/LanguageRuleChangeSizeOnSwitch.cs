using System;
using UnityEngine;

namespace FrameWork.UI.LanguageRule
{
	// Token: 0x02000FEE RID: 4078
	public class LanguageRuleChangeSizeOnSwitch : MonoBehaviour, ILanguage
	{
		// Token: 0x0600BA1A RID: 47642 RVA: 0x0054C1CC File Offset: 0x0054A3CC
		private void OnEnable()
		{
			bool flag = this.autoGetCnSizeOnEnable;
			if (flag)
			{
				this.cnSize = this.targetRectTransform.rect.size;
			}
		}

		// Token: 0x0600BA1B RID: 47643 RVA: 0x0054C200 File Offset: 0x0054A400
		public void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			switch (languageType)
			{
			case LocalStringManager.LanguageType.CN:
				this.targetRectTransform.SetSize(this.cnSize);
				break;
			case LocalStringManager.LanguageType.EN:
				this.targetRectTransform.SetSize(this.enSize);
				break;
			case LocalStringManager.LanguageType.KO:
				this.targetRectTransform.SetSize(this.koSize);
				break;
			case LocalStringManager.LanguageType.CNH:
				this.targetRectTransform.SetSize(this.cnhSize);
				break;
			}
		}

		// Token: 0x04008FE6 RID: 36838
		[Header("即将被改变尺寸的节点")]
		[SerializeField]
		private RectTransform targetRectTransform;

		// Token: 0x04008FE7 RID: 36839
		[Header("中文尺寸")]
		[SerializeField]
		private Vector2 cnSize;

		// Token: 0x04008FE8 RID: 36840
		[Header("是否在Enable时自动获取中文尺寸")]
		[SerializeField]
		private bool autoGetCnSizeOnEnable;

		// Token: 0x04008FE9 RID: 36841
		[Header("英文尺寸")]
		[SerializeField]
		private Vector2 enSize;

		// Token: 0x04008FEA RID: 36842
		[Header("韩文尺寸")]
		[SerializeField]
		private Vector2 koSize;

		// Token: 0x04008FEB RID: 36843
		[Header("繁中尺寸")]
		[SerializeField]
		private Vector2 cnhSize;
	}
}
