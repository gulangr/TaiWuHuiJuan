using System;
using System.Collections.Generic;

namespace Game.Views.MouseTips
{
	// Token: 0x02000829 RID: 2089
	[Serializable]
	internal sealed class CommonTipConfig
	{
		// Token: 0x040047A5 RID: 18341
		public string Title;

		// Token: 0x040047A6 RID: 18342
		public List<CommonTipParagraphConfig> Paragraphs;

		// Token: 0x040047A7 RID: 18343
		public string EncyclopediaTipLink;
	}
}
