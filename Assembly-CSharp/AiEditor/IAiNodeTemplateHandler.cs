using System;

namespace AiEditor
{
	// Token: 0x02000680 RID: 1664
	public interface IAiNodeTemplateHandler
	{
		// Token: 0x06004EAA RID: 20138
		void NodeRelate(int runtimeId);

		// Token: 0x06004EAB RID: 20139
		void NodeAction(int runtimeId, Action action);
	}
}
