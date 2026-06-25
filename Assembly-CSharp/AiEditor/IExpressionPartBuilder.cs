using System;
using System.Text.RegularExpressions;

namespace AiEditor
{
	// Token: 0x02000688 RID: 1672
	public interface IExpressionPartBuilder
	{
		// Token: 0x1700099F RID: 2463
		// (get) Token: 0x06004EC7 RID: 20167
		Regex Regex { get; }

		// Token: 0x06004EC8 RID: 20168 RVA: 0x0024F910 File Offset: 0x0024DB10
		bool CanBuild(string input, int index)
		{
			return this.Regex.IsMatch(input.Substring(index, input.Length - index));
		}

		// Token: 0x06004EC9 RID: 20169
		void Build(ExpressionBuilder builder, Match match);
	}
}
