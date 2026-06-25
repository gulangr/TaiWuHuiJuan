using System;

namespace Game.Views.TipsBuilders
{
	// Token: 0x0200074C RID: 1868
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	public class CommonTipBuilderAttribute : Attribute
	{
		// Token: 0x06005AB8 RID: 23224 RVA: 0x002A1014 File Offset: 0x0029F214
		public CommonTipBuilderAttribute(int templateId)
		{
			this.TemplateId = templateId;
			this.replaceTarget = null;
		}

		// Token: 0x06005AB9 RID: 23225 RVA: 0x002A1031 File Offset: 0x0029F231
		public CommonTipBuilderAttribute(int templateId, TipType replaceTarget)
		{
			this.TemplateId = templateId;
			this.replaceTarget = new TipType?(replaceTarget);
		}

		// Token: 0x04003E83 RID: 16003
		public readonly int TemplateId;

		// Token: 0x04003E84 RID: 16004
		public readonly TipType? replaceTarget;
	}
}
