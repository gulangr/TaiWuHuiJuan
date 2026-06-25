using System;
using System.Text.RegularExpressions;

namespace AiEditor
{
	// Token: 0x02000689 RID: 1673
	public class ExpressionPartBuilderNumber : IExpressionPartBuilder
	{
		// Token: 0x170009A0 RID: 2464
		// (get) Token: 0x06004ECA RID: 20170 RVA: 0x0024F93B File Offset: 0x0024DB3B
		public Regex Regex { get; } = new Regex("^[0-9]+", RegexOptions.Compiled);

		// Token: 0x06004ECB RID: 20171 RVA: 0x0024F943 File Offset: 0x0024DB43
		public void Build(ExpressionBuilder builder, Match match)
		{
			builder.BuildNumber(int.Parse(match.Groups[0].ToString()));
		}
	}
}
