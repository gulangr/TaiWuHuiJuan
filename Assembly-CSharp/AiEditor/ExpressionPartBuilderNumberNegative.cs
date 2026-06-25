using System;
using System.Text.RegularExpressions;

namespace AiEditor
{
	// Token: 0x0200068A RID: 1674
	public class ExpressionPartBuilderNumberNegative : IExpressionPartBuilder
	{
		// Token: 0x170009A1 RID: 2465
		// (get) Token: 0x06004ECD RID: 20173 RVA: 0x0024F97C File Offset: 0x0024DB7C
		public Regex Regex { get; } = new Regex("^-[0-9]+", RegexOptions.Compiled);

		// Token: 0x06004ECE RID: 20174 RVA: 0x0024F984 File Offset: 0x0024DB84
		public void Build(ExpressionBuilder builder, Match match)
		{
			builder.BuildNumber(int.Parse(match.Groups[0].ToString()));
		}

		// Token: 0x06004ECF RID: 20175 RVA: 0x0024F9A4 File Offset: 0x0024DBA4
		bool IExpressionPartBuilder.CanBuild(string input, int index)
		{
			if (index != 0)
			{
				char c = input[index - 1];
				if (c != '+' && c != '-' && c != '×' && c != '÷')
				{
					return false;
				}
			}
			return this.Regex.IsMatch(input.Substring(index, input.Length - index));
		}
	}
}
