using System;
using System.Collections.Generic;

// Token: 0x02000268 RID: 616
public class GeneralLineData
{
	// Token: 0x060028D2 RID: 10450 RVA: 0x0012E1D1 File Offset: 0x0012C3D1
	public GeneralLineData(sbyte type, List<string> args, List<object> extraArgs = null)
	{
		this.Type = type;
		this.Args = args;
		this.ExtraArgs = extraArgs;
	}

	// Token: 0x060028D3 RID: 10451 RVA: 0x0012E1FB File Offset: 0x0012C3FB
	public GeneralLineData()
	{
	}

	// Token: 0x04001DC1 RID: 7617
	public sbyte Type;

	// Token: 0x04001DC2 RID: 7618
	public List<string> Args;

	// Token: 0x04001DC3 RID: 7619
	public float PreferredHeight = 30f;

	// Token: 0x04001DC4 RID: 7620
	public List<object> ExtraArgs;
}
