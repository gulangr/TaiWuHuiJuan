using System;

namespace DisplayConfig
{
	// Token: 0x020006DD RID: 1757
	public class FiveElementItem
	{
		// Token: 0x17000A51 RID: 2641
		// (get) Token: 0x06005393 RID: 21395 RVA: 0x0026C524 File Offset: 0x0026A724
		public string Name
		{
			get
			{
				return this._nameKey.Tr();
			}
		}

		// Token: 0x06005394 RID: 21396 RVA: 0x0026C531 File Offset: 0x0026A731
		public FiveElementItem(sbyte templateId, LanguageKey nameKey, string icon)
		{
			this.TemplateId = templateId;
			this._nameKey = nameKey;
			this.Icon = icon;
		}

		// Token: 0x04003881 RID: 14465
		public readonly sbyte TemplateId;

		// Token: 0x04003882 RID: 14466
		public readonly string Icon;

		// Token: 0x04003883 RID: 14467
		private readonly LanguageKey _nameKey;
	}
}
