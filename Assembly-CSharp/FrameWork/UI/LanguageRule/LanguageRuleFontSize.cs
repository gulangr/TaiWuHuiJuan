using System;
using TMPro;
using UnityEngine;

namespace FrameWork.UI.LanguageRule
{
	// Token: 0x02000FF0 RID: 4080
	public class LanguageRuleFontSize : MonoBehaviour, ILanguage
	{
		// Token: 0x0600BA33 RID: 47667 RVA: 0x0054D0B6 File Offset: 0x0054B2B6
		private void OnEnable()
		{
			if (this.targetText == null)
			{
				this.targetText = base.GetComponent<TextMeshProUGUI>();
			}
			this.OnLanguageChange(LocalStringManager.CurLanguageType);
		}

		// Token: 0x0600BA34 RID: 47668 RVA: 0x0054D0DC File Offset: 0x0054B2DC
		public void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			bool flag = this.targetText == null;
			if (!flag)
			{
				switch (languageType)
				{
				case LocalStringManager.LanguageType.CN:
					this.targetText.fontSize = this.cnFontSize;
					break;
				case LocalStringManager.LanguageType.EN:
					this.targetText.fontSize = this.enFontSize;
					break;
				case LocalStringManager.LanguageType.KO:
					this.targetText.fontSize = this.koFontSize;
					break;
				case LocalStringManager.LanguageType.CNH:
					this.targetText.fontSize = this.cnhFontSize;
					break;
				}
			}
		}

		// Token: 0x04008FF8 RID: 36856
		[Header("目标文本组件")]
		[SerializeField]
		private TextMeshProUGUI targetText;

		// Token: 0x04008FF9 RID: 36857
		[Header("中文字体大小")]
		[SerializeField]
		private float cnFontSize = 24f;

		// Token: 0x04008FFA RID: 36858
		[Header("英文字体大小")]
		[SerializeField]
		private float enFontSize = 22f;

		// Token: 0x04008FFB RID: 36859
		[Header("韩文字体大小")]
		[SerializeField]
		private float koFontSize = 22f;

		// Token: 0x04008FFC RID: 36860
		[Header("繁中字体大小")]
		[SerializeField]
		private float cnhFontSize = 24f;
	}
}
