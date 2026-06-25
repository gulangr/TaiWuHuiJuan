using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork.UISystem.UIElements;
using TMPro;
using UILogic.DisplayDataStructure;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Bottom
{
	// Token: 0x02000C40 RID: 3136
	public class NotificationDisplayItem : MonoBehaviour
	{
		// Token: 0x06009F48 RID: 40776 RVA: 0x004A6EB5 File Offset: 0x004A50B5
		private void Awake()
		{
			this.button.onClick.ResetListener(delegate()
			{
				UIManager.Instance.MaskUI(UIElement.InstantNotification);
			});
		}

		// Token: 0x06009F49 RID: 40777 RVA: 0x004A6EE8 File Offset: 0x004A50E8
		public void Set(NotificationItem item, RectTransform parent, List<NotificationDisplayItem> showingNotificationList, float notificationSpacing, TweenCallback onComplete = null)
		{
			this.ShowingTimer = 0f;
			InstantNotificationItem config = InstantNotification.Instance.GetItem(item.RenderInfoList[0].RecordType);
			string simpleDesc = item.GetSimpleDesc();
			bool flag = string.IsNullOrEmpty(simpleDesc);
			if (flag)
			{
				simpleDesc = item.ToString();
			}
			this.canvasGroup.alpha = 0f;
			showingNotificationList.Add(this);
			base.transform.SetParent(parent, false);
			base.transform.SetAsLastSibling();
			bool flag2 = string.IsNullOrEmpty(simpleDesc);
			if (flag2)
			{
				simpleDesc = item.ToString();
			}
			this.content.text = simpleDesc;
			this.icon.SetSprite(this.GetIcon(config.Type), false, null);
			this.titleBg.sprite = this.bgSprites[(int)config.Type];
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.itemRectTrans);
			float newNotificationHeight = this.Height;
			Sequence sequence = DOTween.Sequence();
			bool flag3 = showingNotificationList.Count > 1;
			if (flag3)
			{
				float totalMoveDistance = newNotificationHeight + notificationSpacing;
				for (int i = 0; i < parent.childCount - 1; i++)
				{
					RectTransform existingNotification = parent.GetChild(i).GetComponent<RectTransform>();
					LayoutRebuilder.ForceRebuildLayoutImmediate(existingNotification);
					float currentY = existingNotification.anchoredPosition.y;
					float targetY = currentY + totalMoveDistance;
					sequence.Join(existingNotification.DOAnchorPosY(targetY, 0.2f, false).SetEase(Ease.OutQuad));
				}
			}
			float originalX = 0f;
			float entryY = -parent.rect.height + newNotificationHeight;
			this.itemRectTrans.anchoredPosition = new Vector2(originalX + this.itemRectTrans.rect.width, entryY);
			sequence.Join(this.canvasGroup.DOFade(1f, 0.2f));
			sequence.Join(this.itemRectTrans.DOAnchorPosX(originalX, 0.2f, false).SetEase(Ease.OutQuad));
			bool flag4 = onComplete != null;
			if (flag4)
			{
				sequence.OnComplete(onComplete);
			}
		}

		// Token: 0x170010CF RID: 4303
		// (get) Token: 0x06009F4A RID: 40778 RVA: 0x004A70EC File Offset: 0x004A52EC
		public float Height
		{
			get
			{
				return this.itemRectTrans.rect.height;
			}
		}

		// Token: 0x06009F4B RID: 40779 RVA: 0x004A710C File Offset: 0x004A530C
		private string GetIcon(EInstantNotificationType type)
		{
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case EInstantNotificationType.TaiwuVillage:
				result = "notification_icon_5";
				break;
			case EInstantNotificationType.Team:
				result = "notification_icon_4";
				break;
			case EInstantNotificationType.Property:
				result = "notification_icon_3";
				break;
			case EInstantNotificationType.Society:
				result = "notification_icon_2";
				break;
			case EInstantNotificationType.DuringMonth:
				result = "notification_icon_1";
				break;
			case EInstantNotificationType.Item:
				result = "notification_icon_0";
				break;
			case EInstantNotificationType.LegendaryBook:
				result = "notification_icon_6";
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x04007B29 RID: 31529
		[SerializeField]
		private CImage icon;

		// Token: 0x04007B2A RID: 31530
		[SerializeField]
		private CImage titleBg;

		// Token: 0x04007B2B RID: 31531
		[SerializeField]
		private TMP_Text content;

		// Token: 0x04007B2C RID: 31532
		[SerializeField]
		private CButton button;

		// Token: 0x04007B2D RID: 31533
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04007B2E RID: 31534
		[SerializeField]
		private RectTransform itemRectTrans;

		// Token: 0x04007B2F RID: 31535
		[SerializeField]
		private Sprite[] bgSprites;

		// Token: 0x04007B30 RID: 31536
		public float ShowingTimer;
	}
}
