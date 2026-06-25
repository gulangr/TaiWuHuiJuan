using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Common;
using GameData.Domains.LifeRecord;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.Domains.World.TravelingEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using UILogic.DisplayDataStructure;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.Bottom
{
	// Token: 0x02000C56 RID: 3158
	public class ViewInstantNotificationEvent : UIBase
	{
		// Token: 0x170010E9 RID: 4329
		// (get) Token: 0x0600A0C4 RID: 41156 RVA: 0x004B149C File Offset: 0x004AF69C
		private float HideTime
		{
			get
			{
				return (this._showingNotificationList.Count >= this.notificationMaxCount) ? this.hideDelayWhenFull : this.hideDelay;
			}
		}

		// Token: 0x0600A0C5 RID: 41157 RVA: 0x004B14BF File Offset: 0x004AF6BF
		public static void SetBlockOpenWindow(bool block)
		{
			ViewInstantNotificationEvent._blockOpenWindow = block;
		}

		// Token: 0x0600A0C6 RID: 41158 RVA: 0x004B14C8 File Offset: 0x004AF6C8
		public override void OnInit(ArgumentBox argsBox)
		{
			this.Element.ShowAfterRefresh();
			UIElement mainMenu = UIElement.MainMenu;
			mainMenu.OnShowed = (Action)Delegate.Combine(mainMenu.OnShowed, new Action(this.OnReturnToMainMenu));
			this._isMoving = false;
			this.uiAnim = base.GetComponent<UIAnim>();
			this.uiAnim.Init(Vector3.zero, Vector3.zero.SetX(500f));
			this.RefreshAnimState();
		}

		// Token: 0x0600A0C7 RID: 41159 RVA: 0x004B1542 File Offset: 0x004AF742
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 12, ulong.MaxValue, null));
		}

		// Token: 0x0600A0C8 RID: 41160 RVA: 0x004B1560 File Offset: 0x004AF760
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b != 0)
				{
					if (b == 1)
					{
						bool flag = notification.DomainId == 13 && notification.MethodId == 5;
						if (flag)
						{
							ArgumentCollectionRenderArguments collectionRenderArguments = null;
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref collectionRenderArguments);
							string frameKey = collectionRenderArguments.Key;
							string renderedArgCollectionKey = frameKey + "_RenderedArgumentCollection";
							string argumentCollectionKey = frameKey + "_ArgumentCollection";
							ArgumentCollection argumentCollection;
							this._renderNotificationHelperBox.Get<ArgumentCollection>(argumentCollectionKey, out argumentCollection);
							RenderedArgumentCollection renderedArgumentCollection;
							this._renderNotificationHelperBox.Get<RenderedArgumentCollection>(renderedArgCollectionKey, out renderedArgumentCollection);
							bool flag2 = argumentCollection != null && renderedArgumentCollection != null;
							if (flag2)
							{
								GameMessageUtils.RenderDynamicArguments(collectionRenderArguments, argumentCollection, renderedArgumentCollection, false, false);
								this.RenderTravelEventNotification(frameKey);
							}
						}
					}
				}
				else
				{
					DataUid uid = notification.Uid;
					bool flag3 = uid.DomainId == 19 && uid.DataId == 12;
					if (flag3)
					{
						List<ValueTuple<int, int>> newDataInfos = EasyPool.Get<List<ValueTuple<int, int>>>();
						Serializer.DeserializeModifications(wrapper.DataPool, notification.ValueOffset, this._srcCollection, newDataInfos);
						this.Handle_NewInstantNotifications(newDataInfos);
					}
				}
			}
		}

		// Token: 0x0600A0C9 RID: 41161 RVA: 0x004B16E4 File Offset: 0x004AF8E4
		private void Handle_NewInstantNotifications([TupleElementNames(new string[]
		{
			"offset",
			"size"
		})] List<ValueTuple<int, int>> newDataInfos)
		{
			bool flag = newDataInfos == null || newDataInfos.Count <= 0;
			if (!flag)
			{
				List<TravelingEventRenderInfo> cacheList = new List<TravelingEventRenderInfo>();
				ArgumentCollection argumentCollection = new ArgumentCollection();
				int i = 0;
				int count = newDataInfos.Count;
				while (i < count)
				{
					ValueTuple<int, int> valueTuple = newDataInfos[i];
					int offset = valueTuple.Item1;
					int size = valueTuple.Item2;
					int currOffset = offset;
					while (currOffset >= 0 && currOffset < offset + size)
					{
						TravelingEventRenderInfo renderInfo = this._srcCollection.GetRenderInfo(currOffset, argumentCollection);
						cacheList.Add(renderInfo);
						currOffset = this._srcCollection.Next(currOffset);
					}
					i++;
				}
				bool flag2 = SingletonObject.getInstance<DisplayTriggerModel>().RenderedNotificationList == null;
				if (flag2)
				{
					SingletonObject.getInstance<DisplayTriggerModel>().RenderedNotificationList = new List<NotificationItem>();
				}
				string frameKey = string.Format("Frame_{0}", Time.frameCount);
				string renderedArgCollectionKey = frameKey + "_RenderedArgumentCollection";
				string cacheDataListKey = frameKey + "_DataList";
				string argumentCollectionKey = frameKey + "_ArgumentCollection";
				int index = 0;
				RenderedArgumentCollection renderedArgumentCollection2;
				List<TravelingEventRenderInfo> list;
				ArgumentCollection argumentCollection2;
				while (this._renderNotificationHelperBox.Get<RenderedArgumentCollection>(renderedArgCollectionKey, out renderedArgumentCollection2) || this._renderNotificationHelperBox.Get<List<TravelingEventRenderInfo>>(cacheDataListKey, out list) || this._renderNotificationHelperBox.Get<ArgumentCollection>(cacheDataListKey, out argumentCollection2))
				{
					frameKey = string.Format("Frame_{0}_{1}", Time.frameCount, index++);
					renderedArgCollectionKey = frameKey + "_RenderedArgumentCollection";
					cacheDataListKey = frameKey + "_DataList";
					argumentCollectionKey = frameKey + "_ArgumentCollection";
				}
				RenderedArgumentCollection renderedArgumentCollection = new RenderedArgumentCollection();
				GameMessageUtils.RenderFixedArguments(argumentCollection, renderedArgumentCollection, false);
				this._renderNotificationHelperBox.SetObject(renderedArgCollectionKey, renderedArgumentCollection);
				this._renderNotificationHelperBox.SetObject(cacheDataListKey, cacheList);
				this._renderNotificationHelperBox.SetObject(argumentCollectionKey, argumentCollection);
				bool flag3 = argumentCollection.Characters.Count <= 0 && argumentCollection.Locations.Count <= 0 && argumentCollection.Settlements.Count <= 0;
				if (flag3)
				{
					this.RenderTravelEventNotification(frameKey);
				}
				else
				{
					RecordArgumentsRequest argRequest = new RecordArgumentsRequest(argumentCollection);
					LifeRecordDomainMethod.Call.GetRecordRenderInfoArguments(this.Element.GameDataListenerId, frameKey, argRequest);
				}
			}
		}

		// Token: 0x0600A0CA RID: 41162 RVA: 0x004B1920 File Offset: 0x004AFB20
		private void RenderTravelEventNotification(string frameKey)
		{
			string renderedArgCollectionKey = frameKey + "_RenderedArgumentCollection";
			string argumentCollectionKey = frameKey + "_ArgumentCollection";
			string cacheDataListKey = frameKey + "_DataList";
			List<TravelingEventRenderInfo> cacheList;
			this._renderNotificationHelperBox.Get<List<TravelingEventRenderInfo>>(cacheDataListKey, out cacheList);
			RenderedArgumentCollection renderedArgumentCollection;
			this._renderNotificationHelperBox.Get<RenderedArgumentCollection>(renderedArgCollectionKey, out renderedArgumentCollection);
			bool flag = cacheList == null || renderedArgumentCollection == null;
			if (flag)
			{
				GLog.TagError("TravelNotificationError", frameKey + " instant notification render failure ... ", Array.Empty<object>());
			}
			else
			{
				int currDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
				int newMessageIndex = SingletonObject.getInstance<DisplayTriggerModel>().RenderedNotificationList.Count;
				foreach (TravelingEventRenderInfo renderInfo in cacheList)
				{
					NotificationItem newItem = new NotificationItem((int)renderInfo.RecordType, renderInfo, (short id) => null);
					newItem.Date = currDate;
					newItem.RenderedArgumentCollection = renderedArgumentCollection;
					string _ = newItem.ToString();
					SingletonObject.getInstance<DisplayTriggerModel>().RenderedNotificationList.Add(newItem);
				}
				this._renderNotificationHelperBox.SetObject(renderedArgCollectionKey, null);
				this._renderNotificationHelperBox.SetObject(argumentCollectionKey, null);
				this._renderNotificationHelperBox.SetObject(cacheDataListKey, null);
				GEvent.OnEvent(UiEvents.OnNewTravelNotification, EasyPool.Get<ArgumentBox>().Set("NewMessageIndex", newMessageIndex));
			}
		}

		// Token: 0x0600A0CB RID: 41163 RVA: 0x004B1AA4 File Offset: 0x004AFCA4
		private void Awake()
		{
			PoolManager.SetSrcObject("InstantNotificationItemKey", this.notificationItem.gameObject);
			GEvent.Add(UiEvents.OnNewInstantNotification, new GEvent.Callback(this.OnNewInstantNotification));
			GEvent.Add(UiEvents.OnNewTravelNotification, new GEvent.Callback(this.OnNewInstantNotification));
		}

		// Token: 0x0600A0CC RID: 41164 RVA: 0x004B1AFC File Offset: 0x004AFCFC
		private void OnEnable()
		{
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChange));
			GEvent.Add(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
			GEvent.Add(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
			GEvent.Add(UiEvents.OnInstantNotificationEventStopReceive, new GEvent.Callback(this.OnInstantNotificationEventStopReceive));
			GEvent.Add(UiEvents.OnInstantNotificationEventStartReceive, new GEvent.Callback(this.OnInstantNotificationEventStartReceive));
			GEvent.Add(UiEvents.OnBottomShowAllLogClicked, new GEvent.Callback(this.OnShowLogButtonClick));
		}

		// Token: 0x0600A0CD RID: 41165 RVA: 0x004B1BB0 File Offset: 0x004AFDB0
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChange));
			GEvent.Remove(UiEvents.PlayAnimToHideMainUI, new GEvent.Callback(this.PlayAnimToHideMainUI));
			GEvent.Remove(UiEvents.PlayAnimToShowMainUI, new GEvent.Callback(this.PlayAnimToShowMainUI));
			GEvent.Remove(UiEvents.OnInstantNotificationEventStopReceive, new GEvent.Callback(this.OnInstantNotificationEventStopReceive));
			GEvent.Remove(UiEvents.OnInstantNotificationEventStartReceive, new GEvent.Callback(this.OnInstantNotificationEventStartReceive));
			GEvent.Remove(UiEvents.OnBottomShowAllLogClicked, new GEvent.Callback(this.OnShowLogButtonClick));
		}

		// Token: 0x0600A0CE RID: 41166 RVA: 0x004B1C64 File Offset: 0x004AFE64
		private void Update()
		{
			bool flag = this._ticking && this._canReceive && null == this._hoveredNotification;
			if (flag)
			{
				this.HideTick();
				this.ShowTick();
			}
		}

		// Token: 0x0600A0CF RID: 41167 RVA: 0x004B1CA5 File Offset: 0x004AFEA5
		private void OnDestroy()
		{
			PoolManager.RemoveData("InstantNotificationItemKey");
			GEvent.Remove(UiEvents.OnNewInstantNotification, new GEvent.Callback(this.OnNewInstantNotification));
			GEvent.Remove(UiEvents.OnNewTravelNotification, new GEvent.Callback(this.OnNewInstantNotification));
		}

		// Token: 0x0600A0D0 RID: 41168 RVA: 0x004B1CE8 File Offset: 0x004AFEE8
		private void OnShowLogButtonClick(ArgumentBox argbox)
		{
			bool blockOpenWindow = ViewInstantNotificationEvent._blockOpenWindow;
			if (!blockOpenWindow)
			{
				UIManager.Instance.MaskUI(UIElement.InstantNotification);
			}
		}

		// Token: 0x0600A0D1 RID: 41169 RVA: 0x004B1D14 File Offset: 0x004AFF14
		private void OnNewInstantNotification(ArgumentBox argBox)
		{
			int newMessageIndex;
			bool flag = argBox.Get("NewMessageIndex", out newMessageIndex);
			if (flag)
			{
				List<NotificationItem> list = SingletonObject.getInstance<DisplayTriggerModel>().RenderedNotificationList;
				for (int i = newMessageIndex; i < list.Count; i++)
				{
					this._waitingNotificationList.Add(list[i]);
				}
			}
		}

		// Token: 0x0600A0D2 RID: 41170 RVA: 0x004B1D6A File Offset: 0x004AFF6A
		private void OnTopUiChange(ArgumentBox box)
		{
			this.RefreshAnimState();
		}

		// Token: 0x0600A0D3 RID: 41171 RVA: 0x004B1D74 File Offset: 0x004AFF74
		private void RefreshAnimState()
		{
			int childCount = this.content.childCount;
			bool flag = !UIManager.Instance.IsFocusElement(UIElement.InstantNotificationEvent) && !UIManager.Instance.IsFocusElement(UIElement.PartWorld);
			if (flag)
			{
				for (int i = 0; i < childCount; i++)
				{
					Transform child = this.content.GetChild(i);
					child.DOPause();
				}
				this._ticking = false;
			}
			else
			{
				this._ticking = true;
				for (int j = 0; j < childCount; j++)
				{
					Transform child2 = this.content.GetChild(j);
					child2.DOPlay();
				}
			}
		}

		// Token: 0x0600A0D4 RID: 41172 RVA: 0x004B1E20 File Offset: 0x004B0020
		private void OnReturnToMainMenu()
		{
			this._waitingNotificationList.Clear();
			this._showingNotificationList.Clear();
		}

		// Token: 0x0600A0D5 RID: 41173 RVA: 0x004B1E3C File Offset: 0x004B003C
		private void RemoveOldNotification(int index)
		{
			this._isMoving = true;
			NotificationDisplayItem oldNotification = this._showingNotificationList[index];
			bool flag = this._hoveredNotification == oldNotification;
			if (flag)
			{
				this._hoveredNotification = null;
			}
			RectTransform itemRectTrans = oldNotification.GetComponent<RectTransform>();
			LayoutRebuilder.ForceRebuildLayoutImmediate(itemRectTrans);
			float removedHeight = itemRectTrans.rect.height + this.notificationSpacing;
			float removedY = itemRectTrans.anchoredPosition.y;
			Sequence sequence = DOTween.Sequence();
			foreach (NotificationDisplayItem notification in this._showingNotificationList)
			{
				bool flag2 = notification == oldNotification;
				if (!flag2)
				{
					RectTransform rt = notification.GetComponent<RectTransform>();
					bool flag3 = rt.anchoredPosition.y > removedY;
					if (flag3)
					{
						sequence.Join(rt.DOAnchorPosY(rt.anchoredPosition.y - removedHeight, 0.2f, false).SetEase(Ease.OutQuad));
					}
				}
			}
			this._showingNotificationList.RemoveAt(index);
			CanvasGroup canvasGroup = oldNotification.GetComponent<CanvasGroup>();
			float currentX = itemRectTrans.anchoredPosition.x;
			float targetX = currentX + itemRectTrans.rect.width;
			sequence.Join(canvasGroup.DOFade(0f, 0.2f));
			sequence.Join(itemRectTrans.DOAnchorPosX(targetX, 0.2f, false).SetEase(Ease.InQuad));
			sequence.OnComplete(delegate
			{
				PointerTrigger pointTrigger = oldNotification.GetComponent<PointerTrigger>();
				pointTrigger.EnterEvent = (pointTrigger.ExitEvent = null);
				PoolManager.RemoveObject("InstantNotificationItemKey", oldNotification.gameObject);
				this._isMoving = false;
			});
		}

		// Token: 0x0600A0D6 RID: 41174 RVA: 0x004B1FF4 File Offset: 0x004B01F4
		private IEnumerator AddNewNotification(NotificationItem item)
		{
			bool isMoving = this._isMoving;
			if (isMoving)
			{
				yield break;
			}
			this._isMoving = true;
			yield return new WaitForSeconds(0.2f);
			GEvent.OnEvent(UiEvents.OnBottomShowNewNotification, null);
			NotificationDisplayItem notificationDisplayItem = PoolManager.GetObject<NotificationDisplayItem>("InstantNotificationItemKey");
			notificationDisplayItem.Set(item, this.content, this._showingNotificationList, this.notificationSpacing, delegate
			{
				PointerTrigger pointerTrigger = notificationDisplayItem.GetComponent<PointerTrigger>();
				pointerTrigger.EnterEvent = new UnityEvent();
				pointerTrigger.EnterEvent.AddListener(new UnityAction(base.<AddNewNotification>g__OnPointerEnter|1));
				pointerTrigger.ExitEvent = new UnityEvent();
				pointerTrigger.ExitEvent.AddListener(new UnityAction(base.<AddNewNotification>g__OnPointerExit|2));
				this._isMoving = false;
			});
			yield return null;
			yield break;
		}

		// Token: 0x0600A0D7 RID: 41175 RVA: 0x004B200C File Offset: 0x004B020C
		private void HideTick()
		{
			bool isMoving = this._isMoving;
			if (!isMoving)
			{
				float time = Time.deltaTime;
				for (int i = 0; i < this._showingNotificationList.Count; i++)
				{
					bool flag = (this._showingNotificationList[i].ShowingTimer += time) >= this.HideTime;
					if (flag)
					{
						this.RemoveOldNotification(i--);
					}
				}
			}
		}

		// Token: 0x0600A0D8 RID: 41176 RVA: 0x004B2084 File Offset: 0x004B0284
		private void ShowTick()
		{
			bool isMoving = this._isMoving;
			if (!isMoving)
			{
				bool show = this._showTimer > this.showInterval && this._waitingNotificationList.Count > 0 && this._showingNotificationList.Count <= this.notificationMaxCount && UIManager.Instance.IsFocusElement(UIElement.InstantNotificationEvent);
				bool flag = show;
				if (flag)
				{
					this._showTimer = 0f;
					NotificationItem item = this._waitingNotificationList.First<NotificationItem>();
					this._waitingNotificationList.RemoveAt(0);
					base.StopCoroutine("AddNewNotification");
					base.StartCoroutine(this.AddNewNotification(item));
				}
				this._showTimer += Time.deltaTime;
			}
		}

		// Token: 0x0600A0D9 RID: 41177 RVA: 0x004B213A File Offset: 0x004B033A
		private void OnInstantNotificationEventStopReceive(ArgumentBox argumentBox)
		{
			this._canReceive = false;
			this.uiAnim.PlayHideAnimation(null, false);
		}

		// Token: 0x0600A0DA RID: 41178 RVA: 0x004B2152 File Offset: 0x004B0352
		private void OnInstantNotificationEventStartReceive(ArgumentBox argumentBox)
		{
			this._canReceive = true;
			this.uiAnim.PlayShowAnimation(null, false);
		}

		// Token: 0x0600A0DB RID: 41179 RVA: 0x004B216A File Offset: 0x004B036A
		private void PlayAnimToHideMainUI(ArgumentBox argumentBox)
		{
			this.uiAnim.PlayHideAnimation(null, false);
		}

		// Token: 0x0600A0DC RID: 41180 RVA: 0x004B217A File Offset: 0x004B037A
		private void PlayAnimToShowMainUI(ArgumentBox argumentBox)
		{
			this.uiAnim.PlayShowAnimation(null, false);
		}

		// Token: 0x04007CA8 RID: 31912
		private const string NotificationItemKey = "InstantNotificationItemKey";

		// Token: 0x04007CA9 RID: 31913
		private static bool _blockOpenWindow;

		// Token: 0x04007CAA RID: 31914
		private List<NotificationItem> _waitingNotificationList = new List<NotificationItem>();

		// Token: 0x04007CAB RID: 31915
		private List<NotificationDisplayItem> _showingNotificationList = new List<NotificationDisplayItem>();

		// Token: 0x04007CAC RID: 31916
		private bool _ticking = true;

		// Token: 0x04007CAD RID: 31917
		private bool _canReceive = true;

		// Token: 0x04007CAE RID: 31918
		public float hideDelay = 8f;

		// Token: 0x04007CAF RID: 31919
		public float hideDelayWhenFull = 2f;

		// Token: 0x04007CB0 RID: 31920
		[SerializeField]
		private int notificationMaxCount = 8;

		// Token: 0x04007CB1 RID: 31921
		private bool _isMoving;

		// Token: 0x04007CB2 RID: 31922
		private float _showTimer;

		// Token: 0x04007CB3 RID: 31923
		[SerializeField]
		private float showInterval = 0.05f;

		// Token: 0x04007CB4 RID: 31924
		[SerializeField]
		private float notificationSpacing = 1f;

		// Token: 0x04007CB5 RID: 31925
		private NotificationDisplayItem _hoveredNotification;

		// Token: 0x04007CB6 RID: 31926
		[SerializeField]
		private UIAnim uiAnim;

		// Token: 0x04007CB7 RID: 31927
		private readonly TravelingEventCollection _srcCollection = new TravelingEventCollection();

		// Token: 0x04007CB8 RID: 31928
		private readonly ArgumentBox _renderNotificationHelperBox = new ArgumentBox();

		// Token: 0x04007CB9 RID: 31929
		[SerializeField]
		private NotificationDisplayItem notificationItem;

		// Token: 0x04007CBA RID: 31930
		[SerializeField]
		private RectTransform content;
	}
}
