using System;
using FrameWork.UISystem.UIElements;
using Game.Views.Encyclopedia.Event;
using TMPro;
using UnityEngine;

namespace Game.Views.Encyclopedia.Views
{
	// Token: 0x02000A6B RID: 2667
	public class LabelItemFixedOption : MonoBehaviour
	{
		// Token: 0x06008380 RID: 33664 RVA: 0x003D3920 File Offset: 0x003D1B20
		public void SetData(NodeData nodeData, bool isSelected)
		{
			this.button.ClearAndAddListener(delegate
			{
				this.OnClickFixedItem(nodeData.Id);
			});
			this.buttonDelete.ClearAndAddListener(delegate
			{
				this.OnDeleteFixedItem(nodeData.Id);
			});
			this.selected.SetActive(isSelected);
			this.title.text = nodeData.Title;
		}

		// Token: 0x06008381 RID: 33665 RVA: 0x003D3998 File Offset: 0x003D1B98
		private void OnDeleteFixedItem(int dataId)
		{
			PinTitleEventArgs args = new PinTitleEventArgs
			{
				DataId = dataId,
				ToPin = false
			};
			IEventArgs arg = EventArgs<PinTitleEventArgs>.CreateEventArgs(args);
			EventManager.Instance.Dispatch(5, arg);
		}

		// Token: 0x06008382 RID: 33666 RVA: 0x003D39D4 File Offset: 0x003D1BD4
		private void OnClickFixedItem(int dataId)
		{
			OpenPinTitleEventArgs args = new OpenPinTitleEventArgs
			{
				DataId = dataId
			};
			IEventArgs arg = EventArgs<OpenPinTitleEventArgs>.CreateEventArgs(args);
			EventManager.Instance.Dispatch(6, arg);
		}

		// Token: 0x040064A9 RID: 25769
		[SerializeField]
		private CButton button;

		// Token: 0x040064AA RID: 25770
		[SerializeField]
		private CButton buttonDelete;

		// Token: 0x040064AB RID: 25771
		[SerializeField]
		private GameObject selected;

		// Token: 0x040064AC RID: 25772
		[SerializeField]
		private TextMeshProUGUI title;
	}
}
