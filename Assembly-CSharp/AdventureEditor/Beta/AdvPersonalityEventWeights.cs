using System;
using System.Collections.Generic;

namespace AdventureEditor.Beta
{
	// Token: 0x020006A9 RID: 1705
	public class AdvPersonalityEventWeights
	{
		// Token: 0x06004FA2 RID: 20386 RVA: 0x00253BC0 File Offset: 0x00251DC0
		public bool IsConfigured()
		{
			return this.EmptyBlockWeight > 0 || this.ResRewardWeights.Count > 0 || this.ItemRewardWeights.Count > 0 || this.EventWeights.Count > 0 || this.BonusWeights.Count > 0;
		}

		// Token: 0x040036CE RID: 14030
		public short EmptyBlockWeight;

		// Token: 0x040036CF RID: 14031
		public List<ValueTuple<string, short>> EventWeights = new List<ValueTuple<string, short>>();

		// Token: 0x040036D0 RID: 14032
		public List<ValueTuple<byte, short, short>> ResRewardWeights = new List<ValueTuple<byte, short, short>>();

		// Token: 0x040036D1 RID: 14033
		public List<ValueTuple<byte, short, short, short>> ItemRewardWeights = new List<ValueTuple<byte, short, short, short>>();

		// Token: 0x040036D2 RID: 14034
		public List<ValueTuple<string, short>> BonusWeights = new List<ValueTuple<string, short>>();
	}
}
