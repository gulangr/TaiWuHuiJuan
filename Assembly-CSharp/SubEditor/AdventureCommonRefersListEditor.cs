using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace SubEditor
{
	// Token: 0x02000696 RID: 1686
	public class AdventureCommonRefersListEditor<T> : AdventureAbstractListEditor<T, MonoBehaviour>, ISubEditorReceiver<T>
	{
		// Token: 0x06004F2B RID: 20267 RVA: 0x00250E5E File Offset: 0x0024F05E
		public void SetEditableList(List<T> list)
		{
			this.List = list;
			CButton closeButton = this._closeButton;
			if (closeButton != null)
			{
				closeButton.onClick.ResetListener(delegate()
				{
					base.gameObject.SetActive(false);
				});
			}
		}

		// Token: 0x06004F2C RID: 20268 RVA: 0x00250E8B File Offset: 0x0024F08B
		protected override T ItemCreator(IList<T> list)
		{
			return this.Creator(list);
		}

		// Token: 0x06004F2D RID: 20269 RVA: 0x00250E9C File Offset: 0x0024F09C
		protected override void RefreshItem(IList<T> list, MonoBehaviour editorItem, int index)
		{
			RectTransform item = editorItem.GetComponent<RectTransform>();
			DisableStyleRoot disableRoot = editorItem.gameObject.GetOrAddComponent<DisableStyleRoot>();
			disableRoot.SetStyleEffect(list[index] == null, false);
			this.FixColumns(item);
			this.RefreshAction(list, editorItem, index, delegate
			{
				this.Refresh(list);
			});
		}

		// Token: 0x06004F2E RID: 20270 RVA: 0x00250F18 File Offset: 0x0024F118
		protected override void RefreshItem(IList<T> list, MonoBehaviour editorItem, int index, bool setDisableStyle)
		{
			RectTransform item = editorItem.GetComponent<RectTransform>();
			DisableStyleRoot disableRoot = editorItem.gameObject.GetOrAddComponent<DisableStyleRoot>();
			disableRoot.SetStyleEffect(list[index] == null && setDisableStyle, false);
			this.FixColumns(item);
			this.RefreshAction(list, editorItem, index, delegate
			{
				this.Refresh(list);
			});
		}

		// Token: 0x06004F2F RID: 20271 RVA: 0x00250F94 File Offset: 0x0024F194
		protected void FixColumns(RectTransform item)
		{
			bool flag = this.columnsHeader != null;
			if (flag)
			{
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
				{
					AdventureAbstractListEditor<T, MonoBehaviour>.FixByColumn(this.columnsHeader, item);
				});
			}
		}

		// Token: 0x06004F30 RID: 20272 RVA: 0x00250FE0 File Offset: 0x0024F1E0
		internal void ForceRefresh(IList<T> list)
		{
			this.Refresh(list);
		}

		// Token: 0x04003671 RID: 13937
		[SerializeField]
		private CButton _closeButton;

		// Token: 0x04003672 RID: 13938
		[NonSerialized]
		public Func<IList<T>, T> Creator;

		// Token: 0x04003673 RID: 13939
		[NonSerialized]
		public Action<IList<T>, MonoBehaviour, int, Action> RefreshAction;
	}
}
