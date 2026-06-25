using System;
using UnityEngine;

namespace FrameWork.UI.LanguageRule
{
	// Token: 0x02000FF1 RID: 4081
	public class LanguageRuleImagePattern : MonoBehaviour, ILanguage
	{
		// Token: 0x0600BA36 RID: 47670 RVA: 0x0054D19D File Offset: 0x0054B39D
		private void Reset()
		{
			this.targetImage = base.GetComponent<CImage>();
		}

		// Token: 0x0600BA37 RID: 47671 RVA: 0x0054D1AC File Offset: 0x0054B3AC
		private void OnEnable()
		{
			this.RefreshImage();
		}

		// Token: 0x0600BA38 RID: 47672 RVA: 0x0054D1B6 File Offset: 0x0054B3B6
		public void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			this.RefreshImage();
		}

		// Token: 0x0600BA39 RID: 47673 RVA: 0x0054D1C0 File Offset: 0x0054B3C0
		private void RefreshImage()
		{
			string language = SingletonObject.getInstance<GlobalSettings>().Language.ToLower();
			bool flag = !this.TrySetSprite(language) && language != "cn";
			if (flag)
			{
				this.TrySetSprite("cn");
			}
		}

		// Token: 0x0600BA3A RID: 47674 RVA: 0x0054D208 File Offset: 0x0054B408
		private bool TrySetSprite(string language)
		{
			string spriteName = string.Format(this.imagePattern, language);
			this.targetImage.SetSprite(spriteName, this.nativeSize, null);
			return this.targetImage.enabled;
		}

		// Token: 0x04008FFD RID: 36861
		[SerializeField]
		private CImage targetImage;

		// Token: 0x04008FFE RID: 36862
		[SerializeField]
		private string imagePattern;

		// Token: 0x04008FFF RID: 36863
		[SerializeField]
		private bool nativeSize;
	}
}
