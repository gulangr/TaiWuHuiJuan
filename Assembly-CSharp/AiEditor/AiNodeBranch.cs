using System;

namespace AiEditor
{
	// Token: 0x0200067A RID: 1658
	public class AiNodeBranch : Refers
	{
		// Token: 0x1700098D RID: 2445
		// (get) Token: 0x06004E65 RID: 20069 RVA: 0x0024D6ED File Offset: 0x0024B8ED
		public AiNodeHyperlink Condition
		{
			get
			{
				return base.CGet<AiNodeHyperlink>("Condition");
			}
		}

		// Token: 0x1700098E RID: 2446
		// (get) Token: 0x06004E66 RID: 20070 RVA: 0x0024D6FA File Offset: 0x0024B8FA
		public AiNodeRelate True
		{
			get
			{
				return base.CGet<AiNodeRelate>("True");
			}
		}

		// Token: 0x1700098F RID: 2447
		// (get) Token: 0x06004E67 RID: 20071 RVA: 0x0024D707 File Offset: 0x0024B907
		public AiNodeRelate False
		{
			get
			{
				return base.CGet<AiNodeRelate>("False");
			}
		}

		// Token: 0x06004E68 RID: 20072 RVA: 0x0024D714 File Offset: 0x0024B914
		public void Bind(IAiNodeRelateHandler handler, IAiNodeHyperlinkHandler hyperlinkHandler)
		{
			this.True.Bind(handler);
			this.False.Bind(handler);
			this.Condition.Bind(hyperlinkHandler);
		}

		// Token: 0x06004E69 RID: 20073 RVA: 0x0024D73E File Offset: 0x0024B93E
		public void Set(int conditionId)
		{
			this.Condition.SetCondition(conditionId);
			this.True.Set(-1);
			this.False.Set(-1);
		}
	}
}
