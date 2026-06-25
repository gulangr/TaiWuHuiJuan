using System;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Components.SortAndFilter
{
	// Token: 0x02000C9B RID: 3227
	public class FilterMenuBar : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x0600A434 RID: 42036 RVA: 0x004CB442 File Offset: 0x004C9642
		public void OnPointerEnter(PointerEventData eventData)
		{
			Action onPointerEnterEvent = this.OnPointerEnterEvent;
			if (onPointerEnterEvent != null)
			{
				onPointerEnterEvent();
			}
		}

		// Token: 0x0600A435 RID: 42037 RVA: 0x004CB457 File Offset: 0x004C9657
		public void OnPointerExit(PointerEventData eventData)
		{
			Action onPointerExitEvent = this.OnPointerExitEvent;
			if (onPointerExitEvent != null)
			{
				onPointerExitEvent();
			}
		}

		// Token: 0x0600A436 RID: 42038 RVA: 0x004CB46C File Offset: 0x004C966C
		public void SetLabelText(string text)
		{
			bool flag = this.label != null;
			if (flag)
			{
				this.label.text = text;
			}
		}

		// Token: 0x0600A437 RID: 42039 RVA: 0x004CB497 File Offset: 0x004C9697
		public void SetStatusImage(bool isMenuShow)
		{
			this.bg.sprite = (isMenuShow ? this.selectedSprite : this.normalSprite);
		}

		// Token: 0x0600A438 RID: 42040 RVA: 0x004CB4B8 File Offset: 0x004C96B8
		public void SetupSwapToggle(Action<bool> onValueChanged)
		{
			bool flag = this.swapToggle == null;
			if (!flag)
			{
				this.swapToggle.onValueChanged.RemoveAllListeners();
				this.swapToggle.isOn = false;
				this.swapToggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					Action<bool> onValueChanged2 = onValueChanged;
					if (onValueChanged2 != null)
					{
						onValueChanged2(isOn);
					}
				});
			}
		}

		// Token: 0x0600A439 RID: 42041 RVA: 0x004CB520 File Offset: 0x004C9720
		public void SetSwapToggle(bool isOn, bool force = false)
		{
			bool flag = this.swapToggle == null;
			if (!flag)
			{
				bool flag2 = this.swapToggle.isOn != isOn;
				if (flag2)
				{
					this.swapToggle.isOn = isOn;
				}
				else if (force)
				{
					Toggle.ToggleEvent onValueChanged = this.swapToggle.onValueChanged;
					if (onValueChanged != null)
					{
						onValueChanged.Invoke(isOn);
					}
				}
			}
		}

		// Token: 0x04008201 RID: 33281
		[NonSerialized]
		public Action OnPointerEnterEvent;

		// Token: 0x04008202 RID: 33282
		[NonSerialized]
		public Action OnPointerExitEvent;

		// Token: 0x04008203 RID: 33283
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x04008204 RID: 33284
		[SerializeField]
		private CToggle swapToggle;

		// Token: 0x04008205 RID: 33285
		[SerializeField]
		private CImage bg;

		// Token: 0x04008206 RID: 33286
		[SerializeField]
		private Sprite normalSprite;

		// Token: 0x04008207 RID: 33287
		[SerializeField]
		private Sprite selectedSprite;
	}
}
