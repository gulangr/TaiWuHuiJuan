using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x02001000 RID: 4096
	[RequireComponent(typeof(UIInteractionBehaviour))]
	public class CButton : Button
	{
		// Token: 0x0600BAD1 RID: 47825 RVA: 0x00551278 File Offset: 0x0054F478
		protected override void Awake()
		{
			base.Awake();
			bool flag = !Application.isPlaying;
			if (!flag)
			{
				this._interactionHandler = base.GetComponent<UIInteractionBehaviour>();
			}
		}

		// Token: 0x0600BAD2 RID: 47826 RVA: 0x005512A7 File Offset: 0x0054F4A7
		public void ClearAndAddListener(Action action)
		{
			base.onClick.RemoveAllListeners();
			base.onClick.AddListener(new UnityAction(action.Invoke));
		}

		// Token: 0x0600BAD3 RID: 47827 RVA: 0x005512CE File Offset: 0x0054F4CE
		public void RemoveAllListeners()
		{
			base.onClick.RemoveAllListeners();
		}

		// Token: 0x0600BAD4 RID: 47828 RVA: 0x005512E0 File Offset: 0x0054F4E0
		public override void OnPointerClick(PointerEventData eventData)
		{
			bool flag = eventData.button != PointerEventData.InputButton.Left && eventData.button != PointerEventData.InputButton.Right;
			if (flag)
			{
				base.OnPointerClick(eventData);
			}
			else
			{
				bool canInteract = this.IsActive() && this.IsInteractable();
				bool enableClickAudio = this.EnableClickAudio;
				if (enableClickAudio)
				{
					UIInteractionBehaviour uiinteractionBehaviour;
					if ((uiinteractionBehaviour = this._interactionHandler) == null)
					{
						uiinteractionBehaviour = (this._interactionHandler = base.GetComponent<UIInteractionBehaviour>());
					}
					UIInteractionBehaviour uiinteractionBehaviour2 = uiinteractionBehaviour;
					if (uiinteractionBehaviour2 != null)
					{
						uiinteractionBehaviour2.Play(eventData.button, canInteract);
					}
				}
				bool flag2 = !canInteract || eventData.button > PointerEventData.InputButton.Left;
				if (!flag2)
				{
					base.OnPointerClick(eventData);
				}
			}
		}

		// Token: 0x0400904F RID: 36943
		[Tooltip("是否在 UIBase 自动为按钮注册点击事件")]
		public bool autoListen;

		// Token: 0x04009050 RID: 36944
		[NonSerialized]
		public bool EnableClickAudio = true;

		// Token: 0x04009051 RID: 36945
		private UIInteractionBehaviour _interactionHandler;
	}
}
