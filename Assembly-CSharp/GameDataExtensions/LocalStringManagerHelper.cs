using System;

namespace GameDataExtensions
{
	// Token: 0x020006D8 RID: 1752
	public static class LocalStringManagerHelper
	{
		// Token: 0x17000A4D RID: 2637
		// (get) Token: 0x06005370 RID: 21360 RVA: 0x0026B894 File Offset: 0x00269A94
		public static string CurLanguageKey
		{
			get
			{
				return SingletonObject.getInstance<GlobalSettings>().Language;
			}
		}

		// Token: 0x17000A4E RID: 2638
		// (get) Token: 0x06005371 RID: 21361 RVA: 0x0026B8A0 File Offset: 0x00269AA0
		public static LocalStringManager.LanguageType CurLanguageType
		{
			get
			{
				LocalStringManager.LanguageType languageType;
				return Enum.TryParse<LocalStringManager.LanguageType>(LocalStringManagerHelper.CurLanguageKey, out languageType) ? languageType : LocalStringManager.LanguageType.CN;
			}
		}
	}
}
