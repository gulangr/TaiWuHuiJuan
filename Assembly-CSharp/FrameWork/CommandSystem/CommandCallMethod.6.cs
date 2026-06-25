using System;
using GameData.GameDataBridge;

namespace FrameWork.CommandSystem
{
	// Token: 0x02001066 RID: 4198
	public class CommandCallMethod<T, T1, T2, T3, T4> : CommandCallMethod<T, T1, T2, T3>
	{
		// Token: 0x0600BEFD RID: 48893 RVA: 0x00566E23 File Offset: 0x00565023
		protected override void AnalysisArgument(ArgumentBox argBox)
		{
			base.AnalysisArgument(argBox);
			argBox.Get<T4>("arg4", out this.Arg4);
		}

		// Token: 0x0600BEFE RID: 48894 RVA: 0x00566E40 File Offset: 0x00565040
		protected override void ReleaseArguments()
		{
			base.ReleaseArguments();
			this.Arg4 = default(T4);
		}

		// Token: 0x0600BEFF RID: 48895 RVA: 0x00566E56 File Offset: 0x00565056
		protected override void DoCallMethod()
		{
			GameDataBridge.AddMethodCall<T, T1, T2, T3, T4>(this.ListenerId, this.DomainId, this.MethodId, this.Arg0, this.Arg1, this.Arg2, this.Arg3, this.Arg4);
		}

		// Token: 0x04009267 RID: 37479
		protected T4 Arg4;
	}
}
