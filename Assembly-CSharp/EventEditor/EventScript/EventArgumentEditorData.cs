using System;
using Config;

namespace EventEditor.EventScript
{
	// Token: 0x02000659 RID: 1625
	[Serializable]
	public class EventArgumentEditorData
	{
		// Token: 0x06004D6D RID: 19821 RVA: 0x00248778 File Offset: 0x00246978
		public EventArgumentEditorData()
		{
		}

		// Token: 0x06004D6E RID: 19822 RVA: 0x00248782 File Offset: 0x00246982
		public EventArgumentEditorData(EventArgumentItem config)
		{
			this.Value = config.DefaultValue;
			this.IsExpression = config.IsExpression;
		}

		// Token: 0x06004D6F RID: 19823 RVA: 0x002487A4 File Offset: 0x002469A4
		public EventArgumentEditorData(EventArgumentEditorData other)
		{
			this.Value = other.Value;
			this.IsExpression = other.IsExpression;
		}

		// Token: 0x06004D70 RID: 19824 RVA: 0x002487C6 File Offset: 0x002469C6
		public EventArgumentEditorData(string value, bool isExpression)
		{
			this.Value = value;
			this.IsExpression = isExpression;
		}

		// Token: 0x06004D71 RID: 19825 RVA: 0x002487E0 File Offset: 0x002469E0
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"{value=",
				this.Value,
				", isExpr=",
				this.IsExpression.ToString(),
				"}"
			});
		}

		// Token: 0x040035B8 RID: 13752
		public string Value;

		// Token: 0x040035B9 RID: 13753
		public bool IsExpression;
	}
}
