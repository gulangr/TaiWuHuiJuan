using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using FrameWork;
using GameData.Common;
using GameData.Domains.LifeRecord;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.Domains.World.TravelingEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UILogic.DisplayDataStructure;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003B3 RID: 947
public class UI_TravelNotification : UIBase
{
	// Token: 0x060038DF RID: 14559 RVA: 0x001CBAA3 File Offset: 0x001C9CA3
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(19, 12, ulong.MaxValue, null));
	}

	// Token: 0x060038E0 RID: 14560 RVA: 0x001CBAC0 File Offset: 0x001C9CC0
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
					bool initFlag = this._srcCollection != null;
					bool flag4 = !initFlag;
					if (flag4)
					{
						this._srcCollection = new TravelingEventCollection();
					}
					List<ValueTuple<int, int>> newDataInfos = EasyPool.Get<List<ValueTuple<int, int>>>();
					Serializer.DeserializeModifications(wrapper.DataPool, notification.ValueOffset, this._srcCollection, newDataInfos);
					bool flag5 = initFlag;
					if (flag5)
					{
						this.Handle_NewInstantNotifications(newDataInfos);
					}
				}
			}
		}
	}

	// Token: 0x060038E1 RID: 14561 RVA: 0x001CBC70 File Offset: 0x001C9E70
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
			bool flag2 = this.RenderedNotificationList == null;
			if (flag2)
			{
				this.RenderedNotificationList = new List<NotificationItem>();
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

	// Token: 0x060038E2 RID: 14562 RVA: 0x001CBEA4 File Offset: 0x001CA0A4
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
			int newMessageIndex = this.RenderedNotificationList.Count;
			foreach (TravelingEventRenderInfo renderInfo in cacheList)
			{
				NotificationItem newItem = new NotificationItem((int)renderInfo.RecordType, renderInfo, (short id) => null);
				newItem.RenderedArgumentCollection = renderedArgumentCollection;
				string _ = newItem.ToString();
				this.RenderedNotificationList.Add(newItem);
			}
			this._renderNotificationHelperBox.SetObject(renderedArgCollectionKey, null);
			this._renderNotificationHelperBox.SetObject(argumentCollectionKey, null);
			this._renderNotificationHelperBox.SetObject(cacheDataListKey, null);
			GEvent.OnEvent(UiEvents.OnNewTravelNotification, EasyPool.Get<ArgumentBox>().Set("NewMessageIndex", newMessageIndex));
		}
	}

	// Token: 0x170005CD RID: 1485
	// (get) Token: 0x060038E3 RID: 14563 RVA: 0x001CC00C File Offset: 0x001CA20C
	private List<NotificationItem> DataList
	{
		get
		{
			return this.RenderedNotificationList;
		}
	}

	// Token: 0x060038E4 RID: 14564 RVA: 0x001CC014 File Offset: 0x001CA214
	private void Awake()
	{
		this._itemPool = new PoolItem("TravelNotificationItemPool", base.CGet<GameObject>("NotificationItem"));
		this._contentRoot = base.CGet<RectTransform>("Content");
		this._scrollRect = base.CGet<CScrollRectLegacy>("Scroll");
	}

	// Token: 0x060038E5 RID: 14565 RVA: 0x001CC054 File Offset: 0x001CA254
	public override void OnInit(ArgumentBox argsBox)
	{
		UIElement element = this.Element;
		element.OnActive = (Action)Delegate.Combine(element.OnActive, new Action(this.Element.ShowAfterRefresh));
		bool flag = !WorldMapModel.Traveling;
		if (flag)
		{
			List<NotificationItem> dataList = this.DataList;
			if (dataList != null)
			{
				dataList.Clear();
			}
		}
	}

	// Token: 0x060038E6 RID: 14566 RVA: 0x001CC0B0 File Offset: 0x001CA2B0
	private void OnEnable()
	{
		this.RefreshList();
		GEvent.Add(UiEvents.OnNewTravelNotification, new GEvent.Callback(this.OnNewTravelNotification));
		GEvent.Add(UiEvents.OnTravelStart, new GEvent.Callback(this.OnTravelStateChange));
		GEvent.Add(UiEvents.OnTravelFinish, new GEvent.Callback(this.OnTravelStateChange));
	}

	// Token: 0x060038E7 RID: 14567 RVA: 0x001CC110 File Offset: 0x001CA310
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.OnNewTravelNotification, new GEvent.Callback(this.OnNewTravelNotification));
		GEvent.Remove(UiEvents.OnTravelStart, new GEvent.Callback(this.OnTravelStateChange));
		GEvent.Remove(UiEvents.OnTravelFinish, new GEvent.Callback(this.OnTravelStateChange));
	}

	// Token: 0x060038E8 RID: 14568 RVA: 0x001CC169 File Offset: 0x001CA369
	private void OnDestroy()
	{
		this._itemPool.Destroy();
		this._itemPool = null;
	}

	// Token: 0x060038E9 RID: 14569 RVA: 0x001CC17F File Offset: 0x001CA37F
	private void OnNewTravelNotification(ArgumentBox argBox)
	{
		this.RefreshList();
	}

	// Token: 0x060038EA RID: 14570 RVA: 0x001CC18C File Offset: 0x001CA38C
	private void OnTravelStateChange(ArgumentBox argBox)
	{
		Refers[] cachedRefers = this._contentRoot.GetComponentsInTopChildren(false);
		Array.ForEach<Refers>(cachedRefers, delegate(Refers e)
		{
			e.name = string.Format("CachePoolItem_{0}", e.GetInstanceID());
			this._itemPool.DestroyObject(e.gameObject);
		});
		List<NotificationItem> dataList = this.DataList;
		if (dataList != null)
		{
			dataList.Clear();
		}
	}

	// Token: 0x060038EB RID: 14571 RVA: 0x001CC1CC File Offset: 0x001CA3CC
	private void RefreshList()
	{
		bool flag = this.DataList == null || this.DataList.Count <= 0;
		if (flag)
		{
			this._contentRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
		}
		else
		{
			int i = 0;
			int max = this.DataList.Count;
			while (i < max)
			{
				Transform itemTransform = this._contentRoot.Find(i.ToString());
				bool flag2 = null != itemTransform && itemTransform.gameObject.activeSelf;
				if (!flag2)
				{
					Refers cellRefers = this._itemPool.GetObject().GetComponent<Refers>();
					cellRefers.UserInt = i;
					cellRefers.name = i.ToString();
					RectTransform cellRectTrans = (RectTransform)cellRefers.transform;
					cellRectTrans.SetParent(this._contentRoot, false);
					cellRefers.CGet<TextMeshProUGUI>("Content").text = this.DataList[i].ToString();
					cellRectTrans.SetAsLastSibling();
				}
				i++;
			}
			LayoutRebuilder.MarkLayoutForRebuild(this._contentRoot);
			bool flag3 = this._contentRoot.rect.height > this._scrollRect.Viewport.rect.height;
			if (flag3)
			{
				float val = this._scrollRect.ScrollBar.value;
				DOVirtual.Float(val, 1f, 0.3f, delegate(float stepValue)
				{
					this._scrollRect.ScrollBar.value = stepValue;
				}).OnComplete(delegate
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(this._contentRoot);
					this._scrollRect.ScrollBar.value = 1f;
				}).SetAutoKill(true);
			}
		}
	}

	// Token: 0x04002930 RID: 10544
	private TravelingEventCollection _srcCollection;

	// Token: 0x04002931 RID: 10545
	[NonSerialized]
	public List<NotificationItem> RenderedNotificationList;

	// Token: 0x04002932 RID: 10546
	private ArgumentBox _renderNotificationHelperBox = new ArgumentBox();

	// Token: 0x04002933 RID: 10547
	private RectTransform _contentRoot;

	// Token: 0x04002934 RID: 10548
	private PoolItem _itemPool;

	// Token: 0x04002935 RID: 10549
	private List<ValueTuple<Refers, float>> _newAddItemList;

	// Token: 0x04002936 RID: 10550
	private CScrollRectLegacy _scrollRect;
}
