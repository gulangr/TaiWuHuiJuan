using System;
using GameData.GameDataBridge;

namespace FrameWork.CommandSystem
{
	// Token: 0x02001067 RID: 4199
	public class CommandCallMethod<T, T1, T2, T3, T4, T5> : CommandCallMethod<T, T1, T2, T3, T4>
	{
		// Token: 0x0600BF01 RID: 48897 RVA: 0x00566E98 File Offset: 0x00565098
		protected override void AnalysisArgument(ArgumentBox argBox)
		{
			base.AnalysisArgument(argBox);
			argBox.Get<T5>("arg5", out this.Arg5);
		}

		// Token: 0x0600BF02 RID: 48898 RVA: 0x00566EB5 File Offset: 0x005650B5
		protected override void ReleaseArguments()
		{
			base.ReleaseArguments();
			this.Arg5 = default(T5);
		}

		// Token: 0x0600BF03 RID: 48899 RVA: 0x00566ECC File Offset: 0x005650CC
		protected override void DoCallMethod()
		{
			GameDataBridge.AddMethodCall<T, T1, T2, T3, T4, T5>(this.ListenerId, this.DomainId, this.MethodId, this.Arg0, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5);
		}

		// Token: 0x04009268 RID: 37480
		protected T5 Arg5;
	}
}
