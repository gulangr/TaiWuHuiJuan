using System;
using System.Text.RegularExpressions;

namespace AiEditor
{
	// Token: 0x0200068B RID: 1675
	public class ExpressionPartBuilderPersonality : IExpressionPartBuilder
	{
		// Token: 0x170009A2 RID: 2466
		// (get) Token: 0x06004ED1 RID: 20177 RVA: 0x0024FA13 File Offset: 0x0024DC13
		public Regex Regex { get; } = new Regex("^\\$P([0-9]+)", RegexOptions.Compiled);

		// Token: 0x06004ED2 RID: 20178 RVA: 0x0024FA1B File Offset: 0x0024DC1B
		public void Build(ExpressionBuilder builder, Match match)
		{
			builder.BuildPersonality(sbyte.Parse(match.Groups[1].ToString()));
		}
	}
}
