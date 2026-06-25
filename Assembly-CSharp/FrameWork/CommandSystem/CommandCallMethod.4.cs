using System;
using GameData.GameDataBridge;

namespace FrameWork.CommandSystem
{
	// Token: 0x02001064 RID: 4196
	public class CommandCallMethod<T, T1, T2> : CommandCallMethod<T, T1>
	{
		// Token: 0x0600BEF5 RID: 48885 RVA: 0x00566D4B File Offset: 0x00564F4B
		protected override void AnalysisArgument(ArgumentBox argBox)
		{
			base.AnalysisArgument(argBox);
			argBox.Get<T2>("arg2", out this.Arg2);
		}

		// Token: 0x0600BEF6 RID: 48886 RVA: 0x00566D68 File Offset: 0x00564F68
		protected override void ReleaseArguments()
		{
			base.ReleaseArguments();
			this.Arg2 = default(T2);
		}

		// Token: 0x0600BEF7 RID: 48887 RVA: 0x00566D7E File Offset: 0x00564F7E
		protected override void DoCallMethod()
		{
			GameDataBridge.AddMethodCall<T, T1, T2>(this.ListenerId, this.DomainId, this.MethodId, this.Arg0, this.Arg1, this.Arg2);
		}

		// Token: 0x04009265 RID: 37477
		protected T2 Arg2;
	}
}
