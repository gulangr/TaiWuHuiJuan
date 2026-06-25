using System;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Encyclopedia.Event;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Encyclopedia.Views
{
	// Token: 0x02000A6A RID: 2666
	internal class LabelItemFixed : MonoBehaviour
	{
		// Token: 0x17000E6D RID: 3693
		// (get) Token: 0x06008375 RID: 33653 RVA: 0x003D360A File Offset: 0x003D180A
		private EncyclopediaDataManager DataManager
		{
			get
			{
				return EncyclopediaDataManager.Instance;
			}
		}

		// Token: 0x06008376 RID: 33654 RVA: 0x003D3614 File Offset: 0x003D1814
		private void Awake()
		{
			this.labelButton.onClick.AddListener(new UnityAction(this.OnLabelButtonClick));
			this.infinityScroll.OnItemRender += this.OnItemRender;
			this.dropdownBtn.ClearAndAddListener(new Action(this.OnClickDropdownBtn));
		}

		// Token: 0x06008377 RID: 33655 RVA: 0x003D3670 File Offset: 0x003D1870
		private void OnClickDropdownBtn()
		{
			bool openFlag = !this.dropdownRect.gameObject.activeSelf;
			this.SetDropdownActive(openFlag);
		}

		// Token: 0x06008378 RID: 33656 RVA: 0x003D369C File Offset: 0x003D189C
		private void SetDropdownActive(bool openFlag)
		{
			this.dropdownRect.gameObject.SetActive(openFlag);
			this.expandedImg.transform.localRotation = Quaternion.Euler(Vector3.zero.SetZ((float)(openFlag ? 180 : 0)));
			if (openFlag)
			{
				this.infinityScroll.UpdateData(this.DataManager.PinnedNodeList.Count);
			}
		}

		// Token: 0x06008379 RID: 33657 RVA: 0x003D370C File Offset: 0x003D190C
		private void OnItemRender(int index, GameObject refers)
		{
			NodeData nodeData = this.DataManager.GetNodeData(this.DataManager.PinnedNodeList[index]);
			LabelItemFixedOption option = refers.GetComponent<LabelItemFixedOption>();
			bool isSelected = nodeData.Id == this.DataManager.CurrentPinnedDataId;
			option.SetData(nodeData, isSelected);
		}

		// Token: 0x0600837A RID: 33658 RVA: 0x003D375C File Offset: 0x003D195C
		private void OnClickFixedItem(int dataId)
		{
			OpenPinTitleEventArgs args = new OpenPinTitleEventArgs
			{
				DataId = dataId
			};
			IEventArgs arg = EventArgs<OpenPinTitleEventArgs>.CreateEventArgs(args);
			EventManager.Instance.Dispatch(6, arg);
		}

		// Token: 0x0600837B RID: 33659 RVA: 0x003D3790 File Offset: 0x003D1990
		private void OnLabelButtonClick()
		{
			this.OnClickFixedItem(this.DataManager.CurrentPinnedDataId);
		}

		// Token: 0x0600837C RID: 33660 RVA: 0x003D37A5 File Offset: 0x003D19A5
		public void UpdateSelectState()
		{
			this.selectedObj.SetActive(this.DataManager.IsHighlightPinned);
		}

		// Token: 0x0600837D RID: 33661 RVA: 0x003D37C0 File Offset: 0x003D19C0
		public void UpdateDisplay()
		{
			base.gameObject.SetActive(this.DataManager.PinnedNodeList.Count > 0);
			bool flag = this.DataManager.PinnedNodeList.Count == 0;
			if (!flag)
			{
				NodeData data = this.DataManager.GetNodeData(this.DataManager.CurrentPinnedDataId);
				this.labelName.text = data.Title;
				this.pinnedAmount.text = this.DataManager.PinnedNodeList.Count.ToString();
				bool activeSelf = this.dropdownRect.gameObject.activeSelf;
				if (activeSelf)
				{
					this.infinityScroll.UpdateData(this.DataManager.PinnedNodeList.Count);
				}
				this.UpdateSelectState();
			}
		}

		// Token: 0x0600837E RID: 33662 RVA: 0x003D3890 File Offset: 0x003D1A90
		private void Update()
		{
			bool flag = CommonCommandKit.LeftMouse.Check(UIElement.Encyclopedia, false, true, false, true, false);
			if (flag)
			{
				Camera uiCamera = UIManager.Instance.UiCamera;
				bool flag2 = this.dropdownRect.gameObject.activeSelf && !RectTransformUtility.RectangleContainsScreenPoint(this.dropdownRect, Input.mousePosition, uiCamera) && !RectTransformUtility.RectangleContainsScreenPoint(this.dropdownTitle, Input.mousePosition, uiCamera);
				if (flag2)
				{
					this.SetDropdownActive(false);
				}
			}
		}

		// Token: 0x040064A0 RID: 25760
		[SerializeField]
		private TextMeshProUGUI labelName;

		// Token: 0x040064A1 RID: 25761
		[SerializeField]
		private CButton dropdownBtn;

		// Token: 0x040064A2 RID: 25762
		[SerializeField]
		private TextMeshProUGUI pinnedAmount;

		// Token: 0x040064A3 RID: 25763
		[SerializeField]
		private GameObject expandedImg;

		// Token: 0x040064A4 RID: 25764
		[SerializeField]
		private GameObject selectedObj;

		// Token: 0x040064A5 RID: 25765
		[SerializeField]
		private InfinityScroll infinityScroll;

		// Token: 0x040064A6 RID: 25766
		[SerializeField]
		private RectTransform dropdownRect;

		// Token: 0x040064A7 RID: 25767
		[SerializeField]
		private RectTransform dropdownTitle;

		// Token: 0x040064A8 RID: 25768
		[SerializeField]
		private CButton labelButton;
	}
}
