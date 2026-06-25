using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x0200100F RID: 4111
	[DisallowMultipleComponent]
	public class RangeSlider : UIBehaviour
	{
		// Token: 0x17001532 RID: 5426
		// (get) Token: 0x0600BBEB RID: 48107 RVA: 0x005578ED File Offset: 0x00555AED
		private int _halfThumbWidth
		{
			get
			{
				return this.thumbWidth / 2;
			}
		}

		// Token: 0x17001533 RID: 5427
		// (get) Token: 0x0600BBEC RID: 48108 RVA: 0x005578F8 File Offset: 0x00555AF8
		private float _totalSliderLength
		{
			get
			{
				return this.background.rect.width - (float)this.thumbWidth;
			}
		}

		// Token: 0x17001534 RID: 5428
		// (get) Token: 0x0600BBED RID: 48109 RVA: 0x00557920 File Offset: 0x00555B20
		public float MinValue
		{
			get
			{
				return this.minValue;
			}
		}

		// Token: 0x17001535 RID: 5429
		// (get) Token: 0x0600BBEE RID: 48110 RVA: 0x00557928 File Offset: 0x00555B28
		public float MaxValue
		{
			get
			{
				return this.maxValue;
			}
		}

		// Token: 0x17001536 RID: 5430
		// (get) Token: 0x0600BBEF RID: 48111 RVA: 0x00557930 File Offset: 0x00555B30
		public float LowerValue
		{
			get
			{
				return this.lowerValue;
			}
		}

		// Token: 0x17001537 RID: 5431
		// (get) Token: 0x0600BBF0 RID: 48112 RVA: 0x00557938 File Offset: 0x00555B38
		public float UpperValue
		{
			get
			{
				return this.upperValue;
			}
		}

		// Token: 0x0600BBF1 RID: 48113 RVA: 0x00557940 File Offset: 0x00555B40
		protected override void Start()
		{
			base.Start();
			this.InitializeThumbs();
		}

		// Token: 0x0600BBF2 RID: 48114 RVA: 0x00557954 File Offset: 0x00555B54
		private void InitializeThumbs()
		{
			bool flag = this.leftThumb == null || this.rightThumb == null || this.background == null;
			if (!flag)
			{
				this.SetupThumb(this.leftThumb.gameObject, RangeSlider.ThumbType.Left);
				this.SetupThumb(this.rightThumb.gameObject, RangeSlider.ThumbType.Right);
				bool flag2 = this.middleHandle != null;
				if (flag2)
				{
					this.SetupThumb(this.middleHandle.gameObject, RangeSlider.ThumbType.Middle);
				}
				this.UpdateVisuals();
			}
		}

		// Token: 0x0600BBF3 RID: 48115 RVA: 0x005579E4 File Offset: 0x00555BE4
		private void SetupThumb(GameObject go, RangeSlider.ThumbType type)
		{
			bool flag = go == null;
			if (!flag)
			{
				bool flag2 = go.GetComponent<UIInteractionBehaviour>() == null;
				if (flag2)
				{
					go.AddComponent<UIInteractionBehaviour>();
				}
				EventTrigger trigger = go.GetComponent<EventTrigger>() ?? go.AddComponent<EventTrigger>();
				this.AddTrigger(trigger, EventTriggerType.BeginDrag, delegate(BaseEventData data)
				{
					this.OnBeginDrag(data, type);
				});
				this.AddTrigger(trigger, EventTriggerType.Drag, delegate(BaseEventData data)
				{
					this.OnDrag(data, type);
				});
				this.AddTrigger(trigger, EventTriggerType.EndDrag, delegate(BaseEventData data)
				{
					this.OnEndDrag(type);
				});
				this.AddTrigger(trigger, EventTriggerType.PointerEnter, delegate(BaseEventData data)
				{
					this.OnPointerEnter(type);
				});
				this.AddTrigger(trigger, EventTriggerType.PointerExit, delegate(BaseEventData data)
				{
					this.OnPointerExit(type);
				});
			}
		}

		// Token: 0x0600BBF4 RID: 48116 RVA: 0x00557AAC File Offset: 0x00555CAC
		private void AddTrigger(EventTrigger trigger, EventTriggerType eventType, Action<BaseEventData> callback)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry
			{
				eventID = eventType
			};
			entry.callback.AddListener(delegate(BaseEventData data)
			{
				callback(data);
			});
			trigger.triggers.Add(entry);
		}

		// Token: 0x0600BBF5 RID: 48117 RVA: 0x00557AF9 File Offset: 0x00555CF9
		public void SetRange(float min, float max, Action<float, float> callback)
		{
			this.minValue = min;
			this.maxValue = max;
			this.onValueChanged = callback;
			this.ValidateValues();
			this.UpdateVisuals();
		}

		// Token: 0x0600BBF6 RID: 48118 RVA: 0x00557B20 File Offset: 0x00555D20
		public void SetValue(float lower, float upper, bool invokeEvent = true)
		{
			this.lowerValue = lower;
			this.upperValue = upper;
			this.ValidateValues();
			this.UpdateVisuals();
			if (invokeEvent)
			{
				Action<float, float> action = this.onValueChanged;
				if (action != null)
				{
					action(this.lowerValue, this.upperValue);
				}
			}
		}

		// Token: 0x0600BBF7 RID: 48119 RVA: 0x00557B70 File Offset: 0x00555D70
		private void OnBeginDrag(BaseEventData data, RangeSlider.ThumbType type)
		{
			this.PlayInteractionAudio();
			this.dragStartLower = this.lowerValue;
			this.dragStartUpper = this.upperValue;
			this.GetLocalPosition((PointerEventData)data, out this.dragStartMousePos);
			switch (type)
			{
			case RangeSlider.ThumbType.Left:
				this.isDraggingLeft = true;
				break;
			case RangeSlider.ThumbType.Right:
				this.isDraggingRight = true;
				break;
			case RangeSlider.ThumbType.Middle:
				this.isDraggingMiddle = true;
				break;
			}
		}

		// Token: 0x0600BBF8 RID: 48120 RVA: 0x00557BE4 File Offset: 0x00555DE4
		private void OnDrag(BaseEventData data, RangeSlider.ThumbType type)
		{
			bool flag = !this.IsDragging(type);
			if (!flag)
			{
				PointerEventData pointerData = (PointerEventData)data;
				Vector2 localPos;
				this.GetLocalPosition(pointerData, out localPos);
				float x = (type != RangeSlider.ThumbType.Middle) ? ((type == RangeSlider.ThumbType.Left) ? (localPos.x + (float)this._halfThumbWidth) : (localPos.x - (float)this._halfThumbWidth)) : localPos.x;
				float t = Mathf.InverseLerp(this.background.rect.xMin + (float)this._halfThumbWidth, this.background.rect.xMax - (float)this._halfThumbWidth, x);
				float newValue = Mathf.Lerp(this.minValue, this.maxValue, t);
				bool flag2 = this.wholeNumbers;
				if (flag2)
				{
					newValue = Mathf.Round(newValue);
				}
				switch (type)
				{
				case RangeSlider.ThumbType.Left:
					this.lowerValue = Mathf.Clamp(newValue, this.minValue, this.upperValue);
					break;
				case RangeSlider.ThumbType.Right:
					this.upperValue = Mathf.Clamp(newValue, this.lowerValue, this.maxValue);
					break;
				case RangeSlider.ThumbType.Middle:
					this.MoveBothThumbs(newValue);
					break;
				}
				this.UpdateVisuals();
				Action<float, float> action = this.onValueChanged;
				if (action != null)
				{
					action(this.lowerValue, this.upperValue);
				}
			}
		}

		// Token: 0x0600BBF9 RID: 48121 RVA: 0x00557D30 File Offset: 0x00555F30
		private void OnEndDrag(RangeSlider.ThumbType type)
		{
			switch (type)
			{
			case RangeSlider.ThumbType.Left:
				this.isDraggingLeft = false;
				break;
			case RangeSlider.ThumbType.Right:
				this.isDraggingRight = false;
				break;
			case RangeSlider.ThumbType.Middle:
				this.isDraggingMiddle = false;
				break;
			}
		}

		// Token: 0x0600BBFA RID: 48122 RVA: 0x00557D74 File Offset: 0x00555F74
		private bool IsDragging(RangeSlider.ThumbType type)
		{
			if (!true)
			{
			}
			bool result;
			switch (type)
			{
			case RangeSlider.ThumbType.Left:
				result = this.isDraggingLeft;
				break;
			case RangeSlider.ThumbType.Right:
				result = this.isDraggingRight;
				break;
			case RangeSlider.ThumbType.Middle:
				result = this.isDraggingMiddle;
				break;
			default:
				result = false;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600BBFB RID: 48123 RVA: 0x00557DC0 File Offset: 0x00555FC0
		private void MoveBothThumbs(float newCenter)
		{
			float delta = newCenter - (this.dragStartLower + this.dragStartUpper) * 0.5f;
			float range = this.dragStartUpper - this.dragStartLower;
			float newLower = this.dragStartLower + delta;
			float newUpper = this.dragStartUpper + delta;
			bool flag = newLower < this.minValue;
			if (flag)
			{
				newLower = this.minValue;
				newUpper = this.minValue + range;
			}
			else
			{
				bool flag2 = newUpper > this.maxValue;
				if (flag2)
				{
					newUpper = this.maxValue;
					newLower = this.maxValue - range;
				}
			}
			this.lowerValue = newLower;
			this.upperValue = newUpper;
		}

		// Token: 0x0600BBFC RID: 48124 RVA: 0x00557E56 File Offset: 0x00556056
		private void OnPointerEnter(RangeSlider.ThumbType type)
		{
			this.OnThumbHoverStateChanged();
		}

		// Token: 0x0600BBFD RID: 48125 RVA: 0x00557E5F File Offset: 0x0055605F
		private void OnPointerExit(RangeSlider.ThumbType type)
		{
			this.OnThumbHoverStateChanged();
		}

		// Token: 0x0600BBFE RID: 48126 RVA: 0x00557E68 File Offset: 0x00556068
		protected virtual void OnThumbHoverStateChanged()
		{
		}

		// Token: 0x0600BBFF RID: 48127 RVA: 0x00557E6C File Offset: 0x0055606C
		private void UpdateVisuals()
		{
			bool flag = this.background == null || this.leftThumb == null || this.rightThumb == null;
			if (!flag)
			{
				float tLower = Mathf.InverseLerp(this.minValue, this.maxValue, this.lowerValue);
				float tUpper = Mathf.InverseLerp(this.minValue, this.maxValue, this.upperValue);
				float halfWidth = this.background.rect.width * 0.5f;
				float rX = tUpper * this._totalSliderLength + (float)this.thumbWidth - halfWidth;
				float lX = tLower * this._totalSliderLength - halfWidth;
				this.leftThumb.anchoredPosition = new Vector2(lX, this.leftThumb.anchoredPosition.y);
				this.rightThumb.anchoredPosition = new Vector2(rX, this.rightThumb.anchoredPosition.y);
				this.UpdateMiddleHandle();
			}
		}

		// Token: 0x0600BC00 RID: 48128 RVA: 0x00557F68 File Offset: 0x00556168
		private void UpdateMiddleHandle()
		{
			bool flag = this.middleHandle == null;
			if (!flag)
			{
				float tLower = Mathf.InverseLerp(this.minValue, this.maxValue, this.lowerValue);
				float tUpper = Mathf.InverseLerp(this.minValue, this.maxValue, this.upperValue);
				float halfWidth = this.background.rect.width * 0.5f;
				this.middleHandle.anchoredPosition = new Vector2(tLower * this._totalSliderLength, this.middleHandle.anchoredPosition.y);
				this.middleHandle.sizeDelta = new Vector2((tUpper - tLower) * this._totalSliderLength + (float)this.thumbWidth, this.middleHandle.sizeDelta.y);
			}
		}

		// Token: 0x0600BC01 RID: 48129 RVA: 0x00558032 File Offset: 0x00556232
		private void GetLocalPosition(PointerEventData data, out Vector2 localPos)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.background, data.position, data.pressEventCamera, out localPos);
		}

		// Token: 0x0600BC02 RID: 48130 RVA: 0x00558050 File Offset: 0x00556250
		private void ValidateValues()
		{
			bool flag = this.minValue > this.maxValue;
			if (flag)
			{
				float num = this.maxValue;
				float num2 = this.minValue;
				this.minValue = num;
				this.maxValue = num2;
			}
			this.lowerValue = Mathf.Clamp(this.lowerValue, this.minValue, this.maxValue);
			this.upperValue = Mathf.Clamp(this.upperValue, this.lowerValue, this.maxValue);
		}

		// Token: 0x0600BC03 RID: 48131 RVA: 0x005580C6 File Offset: 0x005562C6
		private void PlayInteractionAudio()
		{
			if (this.interactionBehaviour == null)
			{
				this.interactionBehaviour = base.GetComponent<UIInteractionBehaviour>();
			}
			UIInteractionBehaviour uiinteractionBehaviour = this.interactionBehaviour;
			if (uiinteractionBehaviour != null)
			{
				uiinteractionBehaviour.Play(true);
			}
		}

		// Token: 0x040090C3 RID: 37059
		[Header("RectTransforms")]
		[SerializeField]
		private RectTransform background;

		// Token: 0x040090C4 RID: 37060
		[SerializeField]
		private RectTransform leftThumb;

		// Token: 0x040090C5 RID: 37061
		[SerializeField]
		private RectTransform rightThumb;

		// Token: 0x040090C6 RID: 37062
		[SerializeField]
		private RectTransform middleHandle;

		// Token: 0x040090C7 RID: 37063
		[Header("数值范围")]
		[SerializeField]
		private float minValue = 0f;

		// Token: 0x040090C8 RID: 37064
		[SerializeField]
		private float maxValue = 100f;

		// Token: 0x040090C9 RID: 37065
		[SerializeField]
		private float lowerValue = 20f;

		// Token: 0x040090CA RID: 37066
		[SerializeField]
		private float upperValue = 80f;

		// Token: 0x040090CB RID: 37067
		[Header("是否为整数")]
		[SerializeField]
		private bool wholeNumbers = false;

		// Token: 0x040090CC RID: 37068
		[SerializeField]
		private int thumbWidth = 0;

		// Token: 0x040090CD RID: 37069
		private bool isDraggingLeft;

		// Token: 0x040090CE RID: 37070
		private bool isDraggingRight;

		// Token: 0x040090CF RID: 37071
		private bool isDraggingMiddle;

		// Token: 0x040090D0 RID: 37072
		private float dragStartLower;

		// Token: 0x040090D1 RID: 37073
		private float dragStartUpper;

		// Token: 0x040090D2 RID: 37074
		private Vector2 dragStartMousePos;

		// Token: 0x040090D3 RID: 37075
		private UIInteractionBehaviour interactionBehaviour;

		// Token: 0x040090D4 RID: 37076
		private Action<float, float> onValueChanged;

		// Token: 0x02002649 RID: 9801
		private enum ThumbType
		{
			// Token: 0x0400EA2A RID: 59946
			Left,
			// Token: 0x0400EA2B RID: 59947
			Right,
			// Token: 0x0400EA2C RID: 59948
			Middle
		}
	}
}
