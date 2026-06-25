using System;

// Token: 0x02000122 RID: 290
public interface IAsyncMethodRequestHandler
{
	// Token: 0x06000B99 RID: 2969
	void RegisterAsyncMethodCall(int requestId);

	// Token: 0x06000B9A RID: 2970
	void ClearAsyncMethodCalls();
}
