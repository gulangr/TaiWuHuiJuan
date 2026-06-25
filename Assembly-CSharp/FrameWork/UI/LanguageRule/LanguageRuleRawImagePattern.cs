using System;
using UnityEngine;

namespace FrameWork.UI.LanguageRule
{
	// Token: 0x02000FF2 RID: 4082
	public class LanguageRuleRawImagePattern : MonoBehaviour, ILanguage
	{
		// Token: 0x0600BA3C RID: 47676 RVA: 0x0054D24F File Offset: 0x0054B44F
		private void Reset()
		{
			this.targetImage = base.GetComponent<CRawImage>();
		}

		// Token: 0x0600BA3D RID: 47677 RVA: 0x0054D25E File Offset: 0x0054B45E
		private void OnEnable()
		{
			this.RefreshImage();
		}

		// Token: 0x0600BA3E RID: 47678 RVA: 0x0054D268 File Offset: 0x0054B468
		public void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			this.RefreshImage();
		}

		// Token: 0x0600BA3F RID: 47679 RVA: 0x0054D274 File Offset: 0x0054B474
		private void RefreshImage()
		{
			string language = SingletonObject.getInstance<GlobalSettings>().Language.ToLower();
			string textureName = string.Format(this.imagePattern, language);
			this.targetImage.SetTexture(textureName);
		}

		// Token: 0x04009000 RID: 36864
		[SerializeField]
		private CRawImage targetImage;

		// Token: 0x04009001 RID: 36865
		[SerializeField]
		private string imagePattern;
	}
}
