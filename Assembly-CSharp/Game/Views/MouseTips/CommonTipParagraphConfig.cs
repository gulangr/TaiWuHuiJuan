using System;
using System.Collections.Generic;

namespace Game.Views.MouseTips
{
	// Token: 0x0200082A RID: 2090
	[Serializable]
	internal sealed class CommonTipParagraphConfig
	{
		// Token: 0x040047A8 RID: 18344
		public string Name;

		// Token: 0x040047A9 RID: 18345
		public string Type;

		// Token: 0x040047AA RID: 18346
		public ParagraphBackgroundType Background;

		// Token: 0x040047AB RID: 18347
		public List<CommonTipAtomConfig> Atoms;
	}
}
