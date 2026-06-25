using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x0200100E RID: 4110
	[RequireComponent(typeof(CToggle))]
	public class ToggleStyle : MonoBehaviour
	{
		// Token: 0x1700152F RID: 5423
		// (get) Token: 0x0600BBDD RID: 48093 RVA: 0x005574F9 File Offset: 0x005556F9
		public TextMeshProUGUI Label
		{
			get
			{
				return this.label;
			}
		}

		// Token: 0x17001530 RID: 5424
		// (get) Token: 0x0600BBDE RID: 48094 RVA: 0x00557501 File Offset: 0x00555701
		public CImage HoverOverlay
		{
			get
			{
				return this.hoverOverlay;
			}
		}

		// Token: 0x17001531 RID: 5425
		// (get) Token: 0x0600BBDF RID: 48095 RVA: 0x00557509 File Offset: 0x00555709
		public CToggle Toggle
		{
			get
			{
				return this._toggle;
			}
		}

		// Token: 0x0600BBE0 RID: 48096 RVA: 0x00557511 File Offset: 0x00555711
		private void Update()
		{
			this.RefreshRuntimeDisplayState();
		}

		// Token: 0x0600BBE1 RID: 48097 RVA: 0x0055751C File Offset: 0x0055571C
		private void RefreshRuntimeDisplayState()
		{
			this.CacheDependencies();
			bool flag = this._toggle == null;
			if (!flag)
			{
				bool isInteractable = this._toggle.IsInteractable();
				bool? lastInteractable = this._lastInteractable;
				bool flag2 = isInteractable;
				bool flag3 = !(lastInteractable.GetValueOrDefault() == flag2 & lastInteractable != null);
				if (flag3)
				{
					this.RefreshInteractableRelatedDisplay();
					this.RefreshHoverVisual();
					this._lastInteractable = new bool?(isInteractable);
				}
				bool isPointerOver = this.IsPointerOverToggle();
				bool flag4 = this._isPointerOver != isPointerOver;
				if (flag4)
				{
					this._isPointerOver = isPointerOver;
					this.RefreshHoverVisual();
				}
				bool isOn = this._toggle.isOn;
				bool flag5 = this._lastIsOn != isOn;
				if (flag5)
				{
					this._lastIsOn = isOn;
					this.RefreshHoverVisual();
				}
			}
		}

		// Token: 0x0600BBE2 RID: 48098 RVA: 0x005575EC File Offset: 0x005557EC
		private bool IsPointerOverToggle()
		{
			RaycastAllManager manager = SingletonObject.getInstance<RaycastAllManager>();
			bool flag = manager == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				List<RaycastResult> _raycastResults = manager.GetCurrentFrameResults();
				bool flag2 = _raycastResults.Count == 0;
				if (flag2)
				{
					result = false;
				}
				else
				{
					GameObject firstHit = _raycastResults[0].gameObject;
					bool flag3 = firstHit == null;
					result = (!flag3 && (firstHit == base.gameObject || firstHit.transform.IsChildOf(base.transform)));
				}
			}
			return result;
		}

		// Token: 0x0600BBE3 RID: 48099 RVA: 0x00557678 File Offset: 0x00555878
		public void SetLabelText(string text)
		{
			bool flag = this.label == null;
			if (!flag)
			{
				this.label.text = text;
			}
		}

		// Token: 0x0600BBE4 RID: 48100 RVA: 0x005576A8 File Offset: 0x005558A8
		public void RefreshInteractableRelatedDisplay()
		{
			this.CacheDependencies();
			bool flag = this._toggle == null;
			if (!flag)
			{
				bool isInteractable = this._toggle.IsInteractable();
				bool flag2 = this.label != null;
				if (flag2)
				{
					this.label.alpha = (isInteractable ? 1f : 0.5f);
				}
				this.RefreshDisabledAlphaGraphics(isInteractable);
			}
		}

		// Token: 0x0600BBE5 RID: 48101 RVA: 0x00557710 File Offset: 0x00555910
		private void RefreshDisabledAlphaGraphics(bool isInteractable)
		{
			bool flag = this.disabledAlphaGraphics == null || this.disabledAlphaGraphics.Length == 0;
			if (!flag)
			{
				float targetAlpha = isInteractable ? 1f : 0.5f;
				for (int i = 0; i < this.disabledAlphaGraphics.Length; i++)
				{
					Graphic graphic = this.disabledAlphaGraphics[i];
					bool flag2 = graphic == null;
					if (!flag2)
					{
						Color c = graphic.color;
						c.a = targetAlpha;
						graphic.color = c;
					}
				}
			}
		}

		// Token: 0x0600BBE6 RID: 48102 RVA: 0x00557795 File Offset: 0x00555995
		public void SetPointerState(bool isPointerOver)
		{
			this._isPointerOver = isPointerOver;
			this.RefreshHoverVisual();
		}

		// Token: 0x0600BBE7 RID: 48103 RVA: 0x005577A8 File Offset: 0x005559A8
		public void SetHoverSprite(Sprite sprite)
		{
			bool flag = this.hoverOverlay == null;
			if (!flag)
			{
				this.hoverOverlay.sprite = sprite;
				bool flag2 = sprite != null;
				if (flag2)
				{
					this.hoverOverlay.SetNativeSize();
				}
				this.hoverOverlay.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600BBE8 RID: 48104 RVA: 0x00557800 File Offset: 0x00555A00
		private void RefreshHoverVisual()
		{
			this.CacheDependencies();
			bool flag = this.hoverOverlay == null;
			if (!flag)
			{
				bool shouldShow = this._isPointerOver && this._toggle != null && this._toggle.IsInteractable() && this.hoverOverlay.sprite != null;
				bool isOn = this._toggle.isOn;
				if (isOn)
				{
					shouldShow = (shouldShow && !this.isBlockHoverWhenToggleIsOn);
				}
				bool flag2 = this.hoverOverlay.gameObject.activeSelf != shouldShow;
				if (flag2)
				{
					this.hoverOverlay.gameObject.SetActive(shouldShow);
				}
			}
		}

		// Token: 0x0600BBE9 RID: 48105 RVA: 0x005578B0 File Offset: 0x00555AB0
		private void CacheDependencies()
		{
			if (this._toggle == null)
			{
				this._toggle = base.GetComponent<CToggle>();
			}
			bool flag = this._toggle == null;
			if (flag)
			{
			}
		}

		// Token: 0x040090BA RID: 37050
		private const float DisabledAlpha = 0.5f;

		// Token: 0x040090BB RID: 37051
		[Header("组件引用")]
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x040090BC RID: 37052
		[SerializeField]
		private CImage hoverOverlay;

		// Token: 0x040090BD RID: 37053
		[SerializeField]
		private Graphic[] disabledAlphaGraphics;

		// Token: 0x040090BE RID: 37054
		[SerializeField]
		private bool isBlockHoverWhenToggleIsOn;

		// Token: 0x040090BF RID: 37055
		private CToggle _toggle;

		// Token: 0x040090C0 RID: 37056
		private bool _isPointerOver;

		// Token: 0x040090C1 RID: 37057
		private bool? _lastInteractable;

		// Token: 0x040090C2 RID: 37058
		private bool _lastIsOn;
	}
}
