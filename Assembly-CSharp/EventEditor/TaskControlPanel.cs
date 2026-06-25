using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x02000655 RID: 1621
	public class TaskControlPanel : EventEditorSubPageBase
	{
		// Token: 0x06004D37 RID: 19767 RVA: 0x002473D6 File Offset: 0x002455D6
		public static void Init(TaskControlPanel instance)
		{
			TaskControlPanel.Instance = instance;
			TaskControlPanel.Instance.InternalInit();
		}

		// Token: 0x06004D38 RID: 19768 RVA: 0x002473EC File Offset: 0x002455EC
		protected override void InternalInit()
		{
			this._normalColor = "847876".HexStringToColor();
			this._highLightColor = "DBD058".HexStringToColor();
			this._eventDetailTrans = this.eventEditorWindow;
			this._eventDetailTrans.position = this.btnEventEditor.transform.position;
			this._eventDetailTrans.localScale = Vector3.zero;
			this.btnEventEditor.targetGraphic.color = this._normalColor;
			this.btnEventEditor.ClearAndAddListener(new Action(this.OnEventEditorClick));
			this.miniEventEditor.ClearAndAddListener(new Action(this.OnEventEditorClick));
			this.btnShowLogTrigger.ClearAndAddListener(new Action(this.OnShowLogClick));
			this.logTrigger.TextObject.text = string.Empty;
			this.rightTabButtons.OnActiveIndexChange += this.OnRightTabToggleGroupChanged;
			this.rightTabButtons.Init(-1);
			EventEditorEventDetail.Init(this.tabEventDetail);
			EventEditorSimulateEnvironment.Init(this.tabSimulateEnv);
			EventEditorTagCenter.Init(this.tabTagCenter);
			EventEditorNotes.Instance.OnNewNote = new Action<string>(this.OnNewNote);
		}

		// Token: 0x06004D39 RID: 19769 RVA: 0x00247527 File Offset: 0x00245727
		public override void Show()
		{
		}

		// Token: 0x06004D3A RID: 19770 RVA: 0x0024752A File Offset: 0x0024572A
		public override void Hide()
		{
		}

		// Token: 0x06004D3B RID: 19771 RVA: 0x00247530 File Offset: 0x00245730
		public void OnEventEditorClick()
		{
			this.isEventEditorShow = !this.isEventEditorShow;
			bool flag = this.isEventEditorShow;
			if (flag)
			{
				this.ShowEventEditorWindow();
			}
			else
			{
				this.HideEventEditorWindow();
			}
			this.btnEventEditor.targetGraphic.color = (this.isEventEditorShow ? this._highLightColor : this._normalColor);
		}

		// Token: 0x06004D3C RID: 19772 RVA: 0x00247590 File Offset: 0x00245790
		public void ShowTips(string tips, bool animate = true)
		{
			bool flag = string.IsNullOrEmpty(tips);
			if (!flag)
			{
				this.logTrigger.TextObject.DOKill(false);
				this.logTrigger.TextObject.color = Color.white;
				this.logTrigger.StartTypeWriter(tips);
				bool flag2 = !animate;
				if (flag2)
				{
					this.logTrigger.StopTypeWriter();
				}
				this._shouldClearTip = true;
			}
		}

		// Token: 0x06004D3D RID: 19773 RVA: 0x002475FA File Offset: 0x002457FA
		private void OnShowLogClick()
		{
			EventEditorNotes.Instance.Show();
		}

		// Token: 0x06004D3E RID: 19774 RVA: 0x00247608 File Offset: 0x00245808
		private void ShowEventEditorWindow()
		{
			this.eventEditorWindowReady = false;
			EventEditorEventList.Instance.Show();
			this._eventDetailTrans.DOKill(false);
			this._eventDetailTrans.position = this.btnEventEditor.transform.position;
			this._eventDetailTrans.localScale = Vector3.zero;
			this._eventDetailTrans.DOScale(1f, 0.3f);
			this._eventDetailTrans.DOLocalMove(Vector3.up * 40f, 0.3f, false).OnComplete(delegate
			{
				this.eventEditorWindowReady = true;
				EventEditorEventDetail.Instance.Show();
				EventEditorEventPreview.Instance.Show();
			});
		}

		// Token: 0x06004D3F RID: 19775 RVA: 0x002476AC File Offset: 0x002458AC
		private void HideEventEditorWindow()
		{
			this.eventEditorWindowReady = false;
			this._eventDetailTrans.DOKill(false);
			this._eventDetailTrans.DOScale(0f, 0.3f);
			this._eventDetailTrans.DOMove(this.btnEventEditor.transform.position, 0.3f, false);
			EventGroupTreeView.Instance.Show();
		}

		// Token: 0x06004D40 RID: 19776 RVA: 0x00247714 File Offset: 0x00245914
		private void OnRightTabToggleGroupChanged(int newIndex, int preIndex)
		{
			this.tabEventDetail.gameObject.SetActive(newIndex == 0);
			this.tabSimulateEnv.gameObject.SetActive(newIndex == 1);
			this.tabTagCenter.gameObject.SetActive(newIndex == 2);
		}

		// Token: 0x06004D41 RID: 19777 RVA: 0x00247764 File Offset: 0x00245964
		private void OnNewNote(string note)
		{
			bool flag = string.IsNullOrEmpty(note);
			if (!flag)
			{
				this.logTrigger.StartTypeWriter(note);
				this._shouldClearTip = false;
			}
		}

		// Token: 0x06004D42 RID: 19778 RVA: 0x00247794 File Offset: 0x00245994
		private void Update()
		{
			bool flag = this.eventEditorWindowReady && !EventEditorScript.Instance.isActiveAndEnabled;
			if (flag)
			{
				bool flag2 = EventEditorCommandKit.SaveCommand.Check(UIElement.EventEditor, false, false, false, true, false);
				if (flag2)
				{
					EventEditorEventDetail.Instance.OnSaveEvent();
				}
				bool flag3 = EventEditorCommandKit.UndoCommand.Check(UIElement.EventEditor, false, false, false, true, false);
				if (flag3)
				{
					EventEditorEventDetail.Instance.Undo();
				}
				bool flag4 = EventEditorCommandKit.RedoCommand.Check(UIElement.EventEditor, false, false, false, true, false);
				if (flag4)
				{
					EventEditorEventDetail.Instance.Redo();
				}
			}
			bool shouldClearTip = this._shouldClearTip;
			if (shouldClearTip)
			{
				bool flag5 = !this.logTrigger.Active;
				if (flag5)
				{
					this._shouldClearTip = false;
					TextMeshProUGUI text = this.logTrigger.TextObject;
					text.DOFade(0f, 3f).SetDelay(3f).OnComplete(delegate
					{
						text.text = string.Empty;
						text.color = Color.white;
					});
				}
			}
		}

		// Token: 0x0400358B RID: 13707
		public static TaskControlPanel Instance;

		// Token: 0x0400358C RID: 13708
		private Color _normalColor;

		// Token: 0x0400358D RID: 13709
		private Color _highLightColor;

		// Token: 0x0400358E RID: 13710
		public bool isEventEditorShow;

		// Token: 0x0400358F RID: 13711
		private RectTransform _eventDetailTrans;

		// Token: 0x04003590 RID: 13712
		private const float TweenTime = 0.3f;

		// Token: 0x04003591 RID: 13713
		public bool eventEditorWindowReady;

		// Token: 0x04003592 RID: 13714
		private bool _shouldClearTip;

		// Token: 0x04003593 RID: 13715
		[SerializeField]
		private CButton btnEventEditor;

		// Token: 0x04003594 RID: 13716
		[SerializeField]
		private RectTransform eventEditorWindow;

		// Token: 0x04003595 RID: 13717
		[SerializeField]
		private UITypeWriterEffect logTrigger;

		// Token: 0x04003596 RID: 13718
		[SerializeField]
		private CButton miniEventEditor;

		// Token: 0x04003597 RID: 13719
		[SerializeField]
		private CToggleGroup rightTabButtons;

		// Token: 0x04003598 RID: 13720
		[SerializeField]
		private EventEditorEventDetail tabEventDetail;

		// Token: 0x04003599 RID: 13721
		[SerializeField]
		private EventEditorSimulateEnvironment tabSimulateEnv;

		// Token: 0x0400359A RID: 13722
		[SerializeField]
		private CButton btnShowLogTrigger;

		// Token: 0x0400359B RID: 13723
		[SerializeField]
		private EventEditorTagCenter tabTagCenter;

		// Token: 0x02001A90 RID: 6800
		private enum EToggle
		{
			// Token: 0x0400B67C RID: 46716
			EventDetail,
			// Token: 0x0400B67D RID: 46717
			SimulateEnv,
			// Token: 0x0400B67E RID: 46718
			TagCenter
		}
	}
}
