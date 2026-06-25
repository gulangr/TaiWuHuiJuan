using System;
using System.Collections.Generic;
using UnityEngine;

namespace AiEditor
{
	// Token: 0x02000678 RID: 1656
	public interface ISelectAndDragHandler
	{
		// Token: 0x1700098A RID: 2442
		// (get) Token: 0x06004E53 RID: 20051
		int SelectedComponentCount { get; }

		// Token: 0x06004E54 RID: 20052
		bool IsSelectedComponent(ISelectAndDragComponent component);

		// Token: 0x1700098B RID: 2443
		// (get) Token: 0x06004E55 RID: 20053
		IEnumerable<ISelectAndDragComponent> SelectedComponents { get; }

		// Token: 0x06004E56 RID: 20054
		void Select(ISelectAndDragComponent component);

		// Token: 0x06004E57 RID: 20055
		void Unselect(ISelectAndDragComponent component);

		// Token: 0x06004E58 RID: 20056
		void SelectEmpty();

		// Token: 0x06004E59 RID: 20057
		void BeginMultiSelect(Vector2 startPos);

		// Token: 0x06004E5A RID: 20058
		void OnMultiSelect(Vector2 startPos, Vector2 curPos);

		// Token: 0x06004E5B RID: 20059
		void EndMultiSelect();

		// Token: 0x06004E5C RID: 20060
		void BeginDrag();

		// Token: 0x06004E5D RID: 20061
		void EndDrag(ISelectAndDragComponent component, Vector2 startPos, Vector2 endPos);

		// Token: 0x06004E5E RID: 20062
		void ActionContext();

		// Token: 0x06004E5F RID: 20063
		void ActionComponents();
	}
}
