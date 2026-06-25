using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x02000647 RID: 1607
	public class EventEditorNotify : EventEditorSubPageBase
	{
		// Token: 0x06004C50 RID: 19536 RVA: 0x00240297 File Offset: 0x0023E497
		public static void Init(EventEditorNotify instance)
		{
			EventEditorNotify.Instance = instance;
		}

		// Token: 0x06004C51 RID: 19537 RVA: 0x002402A0 File Offset: 0x0023E4A0
		protected override void InternalInit()
		{
		}

		// Token: 0x06004C52 RID: 19538 RVA: 0x002402A3 File Offset: 0x0023E4A3
		public override void Show()
		{
			base.gameObject.SetActive(true);
			this.windowRoot.DOAnchorPosY(0f, 0.3f, false);
		}

		// Token: 0x06004C53 RID: 19539 RVA: 0x002402CA File Offset: 0x0023E4CA
		public override void Hide()
		{
			this.windowRoot.DOAnchorPosY(3000f, 0.3f, false).OnComplete(delegate
			{
				base.gameObject.SetActive(false);
			});
			this.txtMeshNotifyContent.GetComponent<TMPLinkClickBridge>().OnLinkClickEvent = null;
		}

		// Token: 0x06004C54 RID: 19540 RVA: 0x00240306 File Offset: 0x0023E506
		public void SetNotifyAndShow(string message)
		{
			this.txtMeshNotifyContent.text = message.ColorReplace();
			this.Show();
		}

		// Token: 0x06004C55 RID: 19541 RVA: 0x00240324 File Offset: 0x0023E524
		public void SetLinkDelegate(Action<string> linkDelegate)
		{
			TMPLinkClickBridge bridge = this.txtMeshNotifyContent.GetComponent<TMPLinkClickBridge>();
			TMPLinkClickBridge tmplinkClickBridge = bridge;
			tmplinkClickBridge.OnLinkClickEvent = (Action<string>)Delegate.Remove(tmplinkClickBridge.OnLinkClickEvent, linkDelegate);
			TMPLinkClickBridge tmplinkClickBridge2 = bridge;
			tmplinkClickBridge2.OnLinkClickEvent = (Action<string>)Delegate.Combine(tmplinkClickBridge2.OnLinkClickEvent, linkDelegate);
		}

		// Token: 0x06004C56 RID: 19542 RVA: 0x0024036C File Offset: 0x0023E56C
		private void Update()
		{
			bool flag = CommonCommandKit.Esc.Check(UIElement.EventEditor, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(UIElement.EventEditor, false, false, false, true, false);
			if (flag)
			{
				this.Hide();
			}
		}

		// Token: 0x040034F6 RID: 13558
		public static EventEditorNotify Instance;

		// Token: 0x040034F7 RID: 13559
		[SerializeField]
		private RectTransform windowRoot;

		// Token: 0x040034F8 RID: 13560
		[SerializeField]
		private TextMeshProUGUI txtMeshNotifyContent;
	}
}
