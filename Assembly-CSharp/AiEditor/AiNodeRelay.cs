using System;

namespace AiEditor
{
	// Token: 0x0200067E RID: 1662
	public class AiNodeRelay : Refers
	{
		// Token: 0x17000995 RID: 2453
		// (get) Token: 0x06004E86 RID: 20102 RVA: 0x0024DCD3 File Offset: 0x0024BED3
		public AiNodeRelate Next
		{
			get
			{
				return base.CGet<AiNodeRelate>("Next");
			}
		}

		// Token: 0x17000996 RID: 2454
		// (get) Token: 0x06004E87 RID: 20103 RVA: 0x0024DCE0 File Offset: 0x0024BEE0
		public AiNodeRelate Relay
		{
			get
			{
				return base.CGet<AiNodeRelate>("Relay");
			}
		}

		// Token: 0x06004E88 RID: 20104 RVA: 0x0024DCED File Offset: 0x0024BEED
		public void Bind(IAiNodeRelateHandler handler)
		{
			this.Next.Bind(handler);
			this.Relay.Bind(handler);
		}

		// Token: 0x06004E89 RID: 20105 RVA: 0x0024DD0A File Offset: 0x0024BF0A
		public void Set()
		{
			this.Next.Set(-1);
			this.Relay.Set(-1);
		}
	}
}
