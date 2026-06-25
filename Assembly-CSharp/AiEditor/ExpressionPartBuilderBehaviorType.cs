using System;
using System.Text.RegularExpressions;

namespace AiEditor
{
	// Token: 0x0200068D RID: 1677
	public class ExpressionPartBuilderBehaviorType : IExpressionPartBuilder
	{
		// Token: 0x170009A4 RID: 2468
		// (get) Token: 0x06004ED7 RID: 20183 RVA: 0x0024FA7F File Offset: 0x0024DC7F
		public Regex Regex { get; } = new Regex("^\\$BT", RegexOptions.Compiled);

		// Token: 0x06004ED8 RID: 20184 RVA: 0x0024FA87 File Offset: 0x0024DC87
		public void Build(ExpressionBuilder builder, Match match)
		{
			builder.BuildBehaviorType();
		}
	}
}
