using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x0200064B RID: 1611
	public class EventEditorSelectJumpEvent : EventEditorSubPageBase
	{
		// Token: 0x1700096C RID: 2412
		// (get) Token: 0x06004CC5 RID: 19653 RVA: 0x00244689 File Offset: 0x00242889
		// (set) Token: 0x06004CC6 RID: 19654 RVA: 0x00244690 File Offset: 0x00242890
		public static EventEditorSelectJumpEvent Instance { get; private set; }

		// Token: 0x06004CC7 RID: 19655 RVA: 0x00244698 File Offset: 0x00242898
		public static void Init(EventEditorSelectJumpEvent instance)
		{
			EventEditorSelectJumpEvent.Instance = instance;
			EventEditorSelectJumpEvent.Instance.InternalInit();
		}

		// Token: 0x06004CC8 RID: 19656 RVA: 0x002446AD File Offset: 0x002428AD
		protected override void InternalInit()
		{
			this.btnHide.ClearAndAddListener(new Action(this.OnHideClick));
			this._windowRoot = this.windowRoot;
			PoolManager.SetSrcObject("EventEditorSelectJumpEvent_JumpEventPrefab", this.toEventPrefab.gameObject);
		}

		// Token: 0x06004CC9 RID: 19657 RVA: 0x002446EA File Offset: 0x002428EA
		public override void Show()
		{
		}

		// Token: 0x06004CCA RID: 19658 RVA: 0x002446ED File Offset: 0x002428ED
		public override void Hide()
		{
		}

		// Token: 0x06004CCB RID: 19659 RVA: 0x002446F0 File Offset: 0x002428F0
		private void OnHideClick()
		{
			EventEditorSelectJumpEventToEventPrefabInfo[] toEventPrefabInfos = this._windowRoot.GetComponentsInTopChildren(false);
			foreach (EventEditorSelectJumpEventToEventPrefabInfo info in toEventPrefabInfos)
			{
				PoolManager.Destroy("EventEditorSelectJumpEvent_JumpEventPrefab", info.gameObject);
			}
			base.gameObject.SetActive(false);
		}

		// Token: 0x06004CCC RID: 19660 RVA: 0x00244740 File Offset: 0x00242940
		public void Show(IReadOnlyList<string> eventList)
		{
			bool flag = eventList == null || eventList.Count < 2;
			if (!flag)
			{
				EventEditorModel eventEditorModel = SingletonObject.getInstance<EventEditorModel>();
				using (IEnumerator<string> enumerator = eventList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string eventGuid = enumerator.Current;
						EventEditorData eventData = eventEditorModel.GetEvent(eventGuid);
						bool flag2 = eventData == null;
						if (!flag2)
						{
							EventEditorSelectJumpEventToEventPrefabInfo toEventInfo = PoolManager.GetObject<EventEditorSelectJumpEventToEventPrefabInfo>("EventEditorSelectJumpEvent_JumpEventPrefab");
							toEventInfo.transform.SetParent(this._windowRoot, false);
							toEventInfo.transform.SetAsLastSibling();
							toEventInfo.txtMeshEventName.text = eventData.EventName;
							toEventInfo.btn.ClearAndAddListener(delegate
							{
								this.OnHideClick();
								Action<string> onSelected = this.OnSelected;
								if (onSelected != null)
								{
									onSelected(eventGuid);
								}
							});
						}
					}
				}
				this._windowRoot.localPosition = Vector3.zero;
				base.gameObject.SetActive(true);
			}
		}

		// Token: 0x04003535 RID: 13621
		private const string JumpEventPrefab = "EventEditorSelectJumpEvent_JumpEventPrefab";

		// Token: 0x04003537 RID: 13623
		[SerializeField]
		private CButton btnHide;

		// Token: 0x04003538 RID: 13624
		[SerializeField]
		private RectTransform windowRoot;

		// Token: 0x04003539 RID: 13625
		[SerializeField]
		private RectTransform toEventPrefab;

		// Token: 0x0400353A RID: 13626
		private RectTransform _windowRoot;

		// Token: 0x0400353B RID: 13627
		[NonSerialized]
		public Action<string> OnSelected;
	}
}
