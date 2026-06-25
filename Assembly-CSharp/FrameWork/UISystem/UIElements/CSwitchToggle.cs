using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x02001009 RID: 4105
	public class CSwitchToggle : CToggle, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x0600BB91 RID: 48017 RVA: 0x00555AD1 File Offset: 0x00553CD1
		protected override void Awake()
		{
			base.Awake();
			this.RefreshDisplay();
			this._lastIsOn = new bool?(base.isOn);
			this._lastInteractable = new bool?(base.interactable);
		}

		// Token: 0x0600BB92 RID: 48018 RVA: 0x00555B04 File Offset: 0x00553D04
		protected override void OnEnable()
		{
			base.OnEnable();
			this.RefreshDisplay();
		}

		// Token: 0x0600BB93 RID: 48019 RVA: 0x00555B15 File Offset: 0x00553D15
		protected override void Start()
		{
			base.Start();
			this.RefreshDisplay();
		}

		// Token: 0x0600BB94 RID: 48020 RVA: 0x00555B28 File Offset: 0x00553D28
		private void Update()
		{
			bool? flag = this._lastInteractable;
			bool flag2 = base.interactable;
			bool flag3;
			if (flag.GetValueOrDefault() == flag2 & flag != null)
			{
				flag = this._lastIsOn;
				flag2 = base.isOn;
				flag3 = !(flag.GetValueOrDefault() == flag2 & flag != null);
			}
			else
			{
				flag3 = true;
			}
			bool flag4 = flag3;
			if (flag4)
			{
				this.RefreshDisplay();
			}
			this._lastIsOn = new bool?(base.isOn);
			this._lastInteractable = new bool?(base.interactable);
		}

		// Token: 0x0600BB95 RID: 48021 RVA: 0x00555BAC File Offset: 0x00553DAC
		public new void OnPointerEnter(PointerEventData eventData)
		{
			bool flag = !this.IsActive() || !this.IsInteractable();
			if (!flag)
			{
				this._isHovering = true;
				this.RefreshDisplay();
			}
		}

		// Token: 0x0600BB96 RID: 48022 RVA: 0x00555BE2 File Offset: 0x00553DE2
		public new void OnPointerExit(PointerEventData eventData)
		{
			this._isHovering = false;
			this.RefreshDisplay();
		}

		// Token: 0x0600BB97 RID: 48023 RVA: 0x00555BF3 File Offset: 0x00553DF3
		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			this.RefreshDisplay();
		}

		// Token: 0x0600BB98 RID: 48024 RVA: 0x00555C08 File Offset: 0x00553E08
		private void RefreshDisplay()
		{
			bool flag = this.backgroundImage != null;
			if (flag)
			{
				Sprite bgSprite = this.GetBackgroundSprite();
				bool flag2 = bgSprite != null;
				if (flag2)
				{
					this.backgroundImage.sprite = bgSprite;
				}
			}
			bool flag3 = this.switchImage != null;
			if (flag3)
			{
				Sprite targetSprite = this.GetSwitchSprite();
				bool flag4 = targetSprite != null;
				if (flag4)
				{
					this.switchImage.sprite = targetSprite;
				}
				this.UpdateSwitchPosition();
			}
		}

		// Token: 0x0600BB99 RID: 48025 RVA: 0x00555C88 File Offset: 0x00553E88
		private Sprite GetBackgroundSprite()
		{
			bool flag = !this.IsInteractable();
			Sprite result;
			if (flag)
			{
				result = this.bgDisabledSprite;
			}
			else
			{
				result = (base.isOn ? this.bgOnSprite : this.bgOffSprite);
			}
			return result;
		}

		// Token: 0x0600BB9A RID: 48026 RVA: 0x00555CC8 File Offset: 0x00553EC8
		private Sprite GetSwitchSprite()
		{
			bool flag = !this.IsInteractable();
			Sprite result;
			if (flag)
			{
				result = (base.isOn ? this.onDisabledSprite : this.offDisabledSprite);
			}
			else
			{
				bool isOn = base.isOn;
				if (isOn)
				{
					result = (this._isHovering ? this.onHoverSprite : this.onNormalSprite);
				}
				else
				{
					result = (this._isHovering ? this.offHoverSprite : this.offNormalSprite);
				}
			}
			return result;
		}

		// Token: 0x0600BB9B RID: 48027 RVA: 0x00555D3C File Offset: 0x00553F3C
		private void UpdateSwitchPosition()
		{
			bool flag = this.switchImage == null || this.switchImage.rectTransform == null;
			if (!flag)
			{
				float totalWidth = base.GetComponent<RectTransform>().rect.width;
				bool flag2 = totalWidth <= 0f;
				if (flag2)
				{
					Canvas.ForceUpdateCanvases();
					totalWidth = base.GetComponent<RectTransform>().rect.width;
				}
				Vector2 anchoredPosition = this.switchImage.rectTransform.anchoredPosition;
				anchoredPosition.x = (base.isOn ? 0f : (totalWidth / 2f));
				this.switchImage.rectTransform.anchoredPosition = anchoredPosition;
			}
		}

		// Token: 0x0400909C RID: 37020
		[Header("图片组件")]
		[SerializeField]
		private CImage backgroundImage;

		// Token: 0x0400909D RID: 37021
		[SerializeField]
		private CImage switchImage;

		// Token: 0x0400909E RID: 37022
		[Header("背景Sprite素材")]
		[SerializeField]
		private Sprite bgOffSprite;

		// Token: 0x0400909F RID: 37023
		[SerializeField]
		private Sprite bgOnSprite;

		// Token: 0x040090A0 RID: 37024
		[SerializeField]
		private Sprite bgDisabledSprite;

		// Token: 0x040090A1 RID: 37025
		[Header("滑块Sprite素材")]
		[SerializeField]
		private Sprite offNormalSprite;

		// Token: 0x040090A2 RID: 37026
		[SerializeField]
		private Sprite offHoverSprite;

		// Token: 0x040090A3 RID: 37027
		[SerializeField]
		private Sprite offDisabledSprite;

		// Token: 0x040090A4 RID: 37028
		[SerializeField]
		private Sprite onNormalSprite;

		// Token: 0x040090A5 RID: 37029
		[SerializeField]
		private Sprite onHoverSprite;

		// Token: 0x040090A6 RID: 37030
		[SerializeField]
		private Sprite onDisabledSprite;

		// Token: 0x040090A7 RID: 37031
		private bool _isHovering;

		// Token: 0x040090A8 RID: 37032
		private bool? _lastIsOn;

		// Token: 0x040090A9 RID: 37033
		private bool? _lastInteractable;
	}
}
