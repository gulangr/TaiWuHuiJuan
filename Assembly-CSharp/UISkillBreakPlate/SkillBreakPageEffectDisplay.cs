using System;

namespace UISkillBreakPlate
{
	// Token: 0x0200041A RID: 1050
	public readonly struct SkillBreakPageEffectDisplay
	{
		// Token: 0x06003E6E RID: 15982 RVA: 0x001F52C4 File Offset: 0x001F34C4
		public SkillBreakPageEffectDisplay(string effectTitle, string effectDesc, string icon)
		{
			this._effectTitle = effectTitle;
			this._effectDesc = effectDesc;
			this._icon = icon;
		}

		// Token: 0x06003E6F RID: 15983 RVA: 0x001F52DC File Offset: 0x001F34DC
		public string ToIconEmbedString()
		{
			string colon = LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
			string fullDesc = this._effectTitle + colon + this._effectDesc;
			return (this._icon == null) ? fullDesc : ("<SpName=" + this._icon + ">" + fullDesc);
		}

		// Token: 0x1700065E RID: 1630
		// (get) Token: 0x06003E70 RID: 15984 RVA: 0x001F532D File Offset: 0x001F352D
		public string EffectTitle
		{
			get
			{
				return this._effectTitle;
			}
		}

		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x06003E71 RID: 15985 RVA: 0x001F5335 File Offset: 0x001F3535
		public string EffectDesc
		{
			get
			{
				return this._effectDesc;
			}
		}

		// Token: 0x17000660 RID: 1632
		// (get) Token: 0x06003E72 RID: 15986 RVA: 0x001F533D File Offset: 0x001F353D
		public string Icon
		{
			get
			{
				return this._icon;
			}
		}

		// Token: 0x04002D01 RID: 11521
		private readonly string _effectTitle;

		// Token: 0x04002D02 RID: 11522
		private readonly string _effectDesc;

		// Token: 0x04002D03 RID: 11523
		private readonly string _icon;
	}
}
