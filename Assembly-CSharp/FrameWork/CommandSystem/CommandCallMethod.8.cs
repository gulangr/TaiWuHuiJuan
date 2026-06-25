using System;
using GameData.GameDataBridge;

namespace FrameWork.CommandSystem
{
	// Token: 0x02001068 RID: 4200
	public class CommandCallMethod<T, T1, T2, T3, T4, T5, T6> : CommandCallMethod<T, T1, T2, T3, T4, T5>
	{
		// Token: 0x0600BF05 RID: 48901 RVA: 0x00566F1F File Offset: 0x0056511F
		protected override void AnalysisArgument(ArgumentBox argBox)
		{
			base.AnalysisArgument(argBox);
			argBox.Get<T6>("arg6", out this.Arg6);
		}

		// Token: 0x0600BF06 RID: 48902 RVA: 0x00566F3C File Offset: 0x0056513C
		protected override void ReleaseArguments()
		{
			base.ReleaseArguments();
			this.Arg6 = default(T6);
		}

		// Token: 0x0600BF07 RID: 48903 RVA: 0x00566F54 File Offset: 0x00565154
		protected override void DoCallMethod()
		{
			GameDataBridge.AddMethodCall<T, T1, T2, T3, T4, T5, T6>(this.ListenerId, this.DomainId, this.MethodId, this.Arg0, this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5, this.Arg6);
		}

		// Token: 0x04009269 RID: 37481
		protected T6 Arg6;
	}
}
