using System;
using System.Text.RegularExpressions;

namespace AiEditor
{
	// Token: 0x0200068C RID: 1676
	public class ExpressionPartBuilderConsummateLevel : IExpressionPartBuilder
	{
		// Token: 0x170009A3 RID: 2467
		// (get) Token: 0x06004ED4 RID: 20180 RVA: 0x0024FA54 File Offset: 0x0024DC54
		public Regex Regex { get; } = new Regex("^\\$CL", RegexOptions.Compiled);

		// Token: 0x06004ED5 RID: 20181 RVA: 0x0024FA5C File Offset: 0x0024DC5C
		public void Build(ExpressionBuilder builder, Match match)
		{
			builder.BuildConsummateLevel();
		}
	}
}
