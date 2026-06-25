using System;
using GameData.GameDataBridge;

namespace FrameWork.CommandSystem
{
	// Token: 0x02001062 RID: 4194
	public class CommandCallMethod<T> : CommandCallMethod
	{
		// Token: 0x0600BEED RID: 48877 RVA: 0x00566C9A File Offset: 0x00564E9A
		protected override void AnalysisArgument(ArgumentBox argBox)
		{
			argBox.Get<T>("arg0", out this.Arg0);
		}

		// Token: 0x0600BEEE RID: 48878 RVA: 0x00566CAF File Offset: 0x00564EAF
		protected override void ReleaseArguments()
		{
			this.Arg0 = default(T);
		}

		// Token: 0x0600BEEF RID: 48879 RVA: 0x00566CBE File Offset: 0x00564EBE
		protected override void DoCallMethod()
		{
			GameDataBridge.AddMethodCall<T>(this.ListenerId, this.DomainId, this.MethodId, this.Arg0);
		}

		// Token: 0x04009263 RID: 37475
		protected T Arg0;
	}
}
