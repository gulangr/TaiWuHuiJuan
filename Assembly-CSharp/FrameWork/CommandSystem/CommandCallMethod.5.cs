using System;
using GameData.GameDataBridge;

namespace FrameWork.CommandSystem
{
	// Token: 0x02001065 RID: 4197
	public class CommandCallMethod<T, T1, T2, T3> : CommandCallMethod<T, T1, T2>
	{
		// Token: 0x0600BEF9 RID: 48889 RVA: 0x00566DB4 File Offset: 0x00564FB4
		protected override void AnalysisArgument(ArgumentBox argBox)
		{
			base.AnalysisArgument(argBox);
			argBox.Get<T3>("arg3", out this.Arg3);
		}

		// Token: 0x0600BEFA RID: 48890 RVA: 0x00566DD1 File Offset: 0x00564FD1
		protected override void ReleaseArguments()
		{
			base.ReleaseArguments();
			this.Arg3 = default(T3);
		}

		// Token: 0x0600BEFB RID: 48891 RVA: 0x00566DE7 File Offset: 0x00564FE7
		protected override void DoCallMethod()
		{
			GameDataBridge.AddMethodCall<T, T1, T2, T3>(this.ListenerId, this.DomainId, this.MethodId, this.Arg0, this.Arg1, this.Arg2, this.Arg3);
		}

		// Token: 0x04009266 RID: 37478
		protected T3 Arg3;
	}
}
