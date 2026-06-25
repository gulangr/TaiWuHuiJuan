using System;
using GameData.GameDataBridge;

namespace FrameWork.CommandSystem
{
	// Token: 0x02001063 RID: 4195
	public class CommandCallMethod<T, T1> : CommandCallMethod<T>
	{
		// Token: 0x0600BEF1 RID: 48881 RVA: 0x00566CE8 File Offset: 0x00564EE8
		protected override void AnalysisArgument(ArgumentBox argBox)
		{
			base.AnalysisArgument(argBox);
			argBox.Get<T1>("arg1", out this.Arg1);
		}

		// Token: 0x0600BEF2 RID: 48882 RVA: 0x00566D05 File Offset: 0x00564F05
		protected override void ReleaseArguments()
		{
			base.ReleaseArguments();
			this.Arg1 = default(T1);
		}

		// Token: 0x0600BEF3 RID: 48883 RVA: 0x00566D1B File Offset: 0x00564F1B
		protected override void DoCallMethod()
		{
			GameDataBridge.AddMethodCall<T, T1>(this.ListenerId, this.DomainId, this.MethodId, this.Arg0, this.Arg1);
		}

		// Token: 0x04009264 RID: 37476
		protected T1 Arg1;
	}
}
