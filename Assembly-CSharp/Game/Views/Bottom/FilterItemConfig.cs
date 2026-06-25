using System;
using GameData.Domains.Map;
using GameData.Utilities;

namespace Game.Views.Bottom
{
	// Token: 0x02000C53 RID: 3155
	public class FilterItemConfig
	{
		// Token: 0x0600A09C RID: 41116 RVA: 0x004AFB41 File Offset: 0x004ADD41
		public FilterItemConfig(EFilterItemKey key, string displayName, EFilterElementType type)
		{
			this.Key = key;
			this.DisplayName = displayName;
			this.Type = type;
			this.Options = null;
			this.SliderRange = new IntPair(0, 0);
		}

		// Token: 0x04007C8C RID: 31884
		public EFilterItemKey Key;

		// Token: 0x04007C8D RID: 31885
		public string DisplayName;

		// Token: 0x04007C8E RID: 31886
		public EFilterElementType Type;

		// Token: 0x04007C8F RID: 31887
		public string[] Options;

		// Token: 0x04007C90 RID: 31888
		public IntPair SliderRange;
	}
}
