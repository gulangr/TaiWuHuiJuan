using System;
using System.Collections.Generic;

namespace SubEditor
{
	// Token: 0x0200069A RID: 1690
	public interface ISubEditorReceiver<T>
	{
		// Token: 0x06004F51 RID: 20305
		void SetEditableList(List<T> list);
	}
}
