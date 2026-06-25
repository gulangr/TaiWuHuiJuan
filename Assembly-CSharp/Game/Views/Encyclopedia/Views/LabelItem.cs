using System;
using System.Diagnostics;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Encyclopedia.Views
{
	// Token: 0x02000A69 RID: 2665
	internal class LabelItem : MonoBehaviour
	{
		// Token: 0x14000082 RID: 130
		// (add) Token: 0x0600836B RID: 33643 RVA: 0x003D3448 File Offset: 0x003D1648
		// (remove) Token: 0x0600836C RID: 33644 RVA: 0x003D3480 File Offset: 0x003D1680
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int> OnClickLabelItem;

		// Token: 0x14000083 RID: 131
		// (add) Token: 0x0600836D RID: 33645 RVA: 0x003D34B8 File Offset: 0x003D16B8
		// (remove) Token: 0x0600836E RID: 33646 RVA: 0x003D34F0 File Offset: 0x003D16F0
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<int> OnClickDelete;

		// Token: 0x0600836F RID: 33647 RVA: 0x003D3525 File Offset: 0x003D1725
		private void Awake()
		{
			this.labelButton.onClick.AddListener(new UnityAction(this.OnLabelButtonClick));
			this.deleteButton.onClick.AddListener(new UnityAction(this.OnDeleteButtonClick));
		}

		// Token: 0x06008370 RID: 33648 RVA: 0x003D3562 File Offset: 0x003D1762
		private void OnLabelButtonClick()
		{
			Action<int> onClickLabelItem = this.OnClickLabelItem;
			if (onClickLabelItem != null)
			{
				onClickLabelItem(this._nodeData.Id);
			}
		}

		// Token: 0x06008371 RID: 33649 RVA: 0x003D3582 File Offset: 0x003D1782
		private void OnDeleteButtonClick()
		{
			Action<int> onClickDelete = this.OnClickDelete;
			if (onClickDelete != null)
			{
				onClickDelete(this._nodeData.Id);
			}
		}

		// Token: 0x06008372 RID: 33650 RVA: 0x003D35A4 File Offset: 0x003D17A4
		public void Init(int dataId, bool selected, Action<int> onClickLabelItem, Action<int> onClickDeleteAction)
		{
			this._nodeData = EncyclopediaDataManager.Instance.GetNodeData(dataId);
			this.labelName.text = this._nodeData.Title;
			this.OnClickLabelItem = onClickLabelItem;
			this.OnClickDelete = onClickDeleteAction;
			this.UpdateSelectState(selected);
		}

		// Token: 0x06008373 RID: 33651 RVA: 0x003D35F1 File Offset: 0x003D17F1
		private void UpdateSelectState(bool selected)
		{
			this.selectedObj.SetActive(selected);
		}

		// Token: 0x04006499 RID: 25753
		[SerializeField]
		private TextMeshProUGUI labelName;

		// Token: 0x0400649A RID: 25754
		[SerializeField]
		private CButton deleteButton;

		// Token: 0x0400649B RID: 25755
		[SerializeField]
		private GameObject selectedObj;

		// Token: 0x0400649C RID: 25756
		[SerializeField]
		private CButton labelButton;

		// Token: 0x0400649D RID: 25757
		private NodeData _nodeData;
	}
}
