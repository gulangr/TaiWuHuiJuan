using System;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using UnityEngine;

namespace Game.Views.Profession
{
	// Token: 0x020007C6 RID: 1990
	public class ExchangeFeatureItem : MonoBehaviour
	{
		// Token: 0x0600611E RID: 24862 RVA: 0x002C8124 File Offset: 0x002C6324
		private void Awake()
		{
			this.pointerTrigger.EnterEvent.ResetListener(delegate()
			{
				this.hover.gameObject.SetActive(true);
				Action<int> onPointerEnter = this._onPointerEnter;
				if (onPointerEnter != null)
				{
					onPointerEnter((int)this._featureId);
				}
			});
			this.pointerTrigger.ExitEvent.ResetListener(delegate()
			{
				this.hover.gameObject.SetActive(false);
				Action<int> onPointerExit = this._onPointerExit;
				if (onPointerExit != null)
				{
					onPointerExit((int)this._featureId);
				}
			});
			this.button.ClearAndAddListener(delegate
			{
				Action onClick = this._onClick;
				if (onClick != null)
				{
					onClick();
				}
			});
		}

		// Token: 0x0600611F RID: 24863 RVA: 0x002C8184 File Offset: 0x002C6384
		public void Set(short templateId, int characterId, bool isTaiwu, Action action)
		{
			this._featureId = templateId;
			this.feature.Set(templateId, characterId, isTaiwu, -1);
			this._onClick = action;
			this.hover.gameObject.SetActive(false);
		}

		// Token: 0x06006120 RID: 24864 RVA: 0x002C81B8 File Offset: 0x002C63B8
		public void SetPointerEventCallbacks(Action<int> onPointerEnter, Action<int> onPointerExit)
		{
			this._onPointerEnter = onPointerEnter;
			this._onPointerExit = onPointerExit;
		}

		// Token: 0x06006121 RID: 24865 RVA: 0x002C81CC File Offset: 0x002C63CC
		public void SetHighlight(bool isHighlight)
		{
			bool flag = this.hover != null;
			if (flag)
			{
				this.hover.gameObject.SetActive(isHighlight);
			}
		}

		// Token: 0x04004361 RID: 17249
		[SerializeField]
		private CButton button;

		// Token: 0x04004362 RID: 17250
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04004363 RID: 17251
		[SerializeField]
		private CImage hover;

		// Token: 0x04004364 RID: 17252
		[SerializeField]
		private Feature feature;

		// Token: 0x04004365 RID: 17253
		private Action _onClick;

		// Token: 0x04004366 RID: 17254
		private Action<int> _onPointerEnter;

		// Token: 0x04004367 RID: 17255
		private Action<int> _onPointerExit;

		// Token: 0x04004368 RID: 17256
		private short _featureId;
	}
}
