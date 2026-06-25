using System;
using GameData.GameDataBridge;

namespace FrameWork.CommandSystem
{
	// Token: 0x02001069 RID: 4201
	public class CommandCallMethod<T, T1, T2, T3, T4, T5, T6, T7> : CommandCallMethod<T, T1, T2, T3, T4, T5, T6>
	{
		// Token: 0x0600BF09 RID: 48905 RVA: 0x00566FAD File Offset: 0x005651AD
		protected override void AnalysisArgument(ArgumentBox argBox)
		{
			base.AnalysisArgument(argBox);
			argBox.Get<T7>("arg7", out this.Arg7);
		}

		// Token: 0x0600BF0A RID: 48906 RVA: 0x00566FCA File Offset: 0x005651CA
		protected override void ReleaseArguments()
		{
			base.ReleaseArguments();
			this.Arg7 = default(T7);
		}

		// Token: 0x0600BF0B RID: 48907 RVA: 0x00566FE0 File Offset: 0x005651E0
		protected override void DoCallMethod()
		{
			GameDataBridge.AddMethodCall<T, T1, T2, T3, T4, T5, T6, T7>(this.ListenerId, this.DomainId, this.MethodId, this.Arg0, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6, this.Arg7);
		}

		// Token: 0x0400926A RID: 37482
		protected T7 Arg7;
	}
}
