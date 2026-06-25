using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x0200100A RID: 4106
	[RequireComponent(typeof(UIInteractionBehaviour))]
	public class CToggle : Toggle
	{
		// Token: 0x0600BB9D RID: 48029 RVA: 0x00555DFF File Offset: 0x00553FFF
		protected override void Awake()
		{
			base.Awake();
			this._interactionHandler = base.GetComponent<UIInteractionBehaviour>();
		}

		// Token: 0x0600BB9E RID: 48030 RVA: 0x00555E15 File Offset: 0x00554015
		public void Register(ICToggleGroup tg)
		{
			this._toggleGroup = tg;
		}

		// Token: 0x0600BB9F RID: 48031 RVA: 0x00555E1F File Offset: 0x0055401F
		public void UnRegister()
		{
			this._toggleGroup = null;
		}

		// Token: 0x0600BBA0 RID: 48032 RVA: 0x00555E2C File Offset: 0x0055402C
		public override void OnPointerClick(PointerEventData eventData)
		{
			bool flag = eventData.button != PointerEventData.InputButton.Left && eventData.button != PointerEventData.InputButton.Right;
			if (!flag)
			{
				bool flag2 = eventData.button == PointerEventData.InputButton.Right;
				if (flag2)
				{
					bool canInteractRight = this.IsActive() && this.IsInteractable();
					UIInteractionBehaviour interactionHandler = this._interactionHandler;
					if (interactionHandler != null)
					{
						interactionHandler.Play(eventData.button, canInteractRight);
					}
				}
				else
				{
					bool canInteract = this.IsActive() && this.IsInteractable();
					bool flag3 = !canInteract;
					if (flag3)
					{
						UIInteractionBehaviour interactionHandler2 = this._interactionHandler;
						if (interactionHandler2 != null)
						{
							interactionHandler2.Play(eventData.button, false);
						}
					}
					else
					{
						bool flag4 = this._toggleGroup != null;
						if (flag4)
						{
							bool flag5 = !this._toggleGroup.ValidateStateChange(this, base.isOn);
							if (!flag5)
							{
								base.OnPointerClick(eventData);
								this._toggleGroup.NotifyToggle(this, base.isOn, true);
								UIInteractionBehaviour interactionHandler3 = this._interactionHandler;
								if (interactionHandler3 != null)
								{
									interactionHandler3.Play(eventData.button, true);
								}
							}
						}
						else
						{
							base.OnPointerClick(eventData);
							UIInteractionBehaviour interactionHandler4 = this._interactionHandler;
							if (interactionHandler4 != null)
							{
								interactionHandler4.Play(eventData.button, true);
							}
						}
					}
				}
			}
		}

		// Token: 0x040090AA RID: 37034
		private ICToggleGroup _toggleGroup;

		// Token: 0x040090AB RID: 37035
		private UIInteractionBehaviour _interactionHandler;
	}
}
