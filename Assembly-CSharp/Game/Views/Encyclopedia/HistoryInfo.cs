using System;
using Game.Views.Encyclopedia.SyntaxTree;

namespace Game.Views.Encyclopedia
{
	// Token: 0x02000A4C RID: 2636
	[Serializable]
	public class HistoryInfo : IEquatable<HistoryInfo>
	{
		// Token: 0x06008264 RID: 33380 RVA: 0x003CC95C File Offset: 0x003CAB5C
		public override string ToString()
		{
			return this.LinkId;
		}

		// Token: 0x06008265 RID: 33381 RVA: 0x003CC974 File Offset: 0x003CAB74
		public bool Equals(HistoryInfo other)
		{
			return this.LinkId == other.LinkId && this.TitleElementType == other.TitleElementType;
		}

		// Token: 0x040063AB RID: 25515
		public NodeLayerType TitleElementType;

		// Token: 0x040063AC RID: 25516
		public string LinkId;

		// Token: 0x040063AD RID: 25517
		public string Title;
	}
}
