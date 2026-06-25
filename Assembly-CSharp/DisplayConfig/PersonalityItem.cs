using System;

namespace DisplayConfig
{
	// Token: 0x020006DF RID: 1759
	public class PersonalityItem
	{
		// Token: 0x17000A53 RID: 2643
		// (get) Token: 0x06005398 RID: 21400 RVA: 0x0026C5FA File Offset: 0x0026A7FA
		public string Name
		{
			get
			{
				return this._nameKey.Tr();
			}
		}

		// Token: 0x17000A54 RID: 2644
		// (get) Token: 0x06005399 RID: 21401 RVA: 0x0026C607 File Offset: 0x0026A807
		public string Desc
		{
			get
			{
				return this._descKey.Tr();
			}
		}

		// Token: 0x0600539A RID: 21402 RVA: 0x0026C614 File Offset: 0x0026A814
		public PersonalityItem(sbyte templateId, LanguageKey nameKey, LanguageKey descKey, string icon)
		{
			this.TemplateId = templateId;
			this._nameKey = nameKey;
			this._descKey = descKey;
			this.Icon = icon;
		}

		// Token: 0x04003886 RID: 14470
		public readonly sbyte TemplateId;

		// Token: 0x04003887 RID: 14471
		public readonly string Icon;

		// Token: 0x04003888 RID: 14472
		private readonly LanguageKey _nameKey;

		// Token: 0x04003889 RID: 14473
		private readonly LanguageKey _descKey;
	}
}
