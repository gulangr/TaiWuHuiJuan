using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x020001AB RID: 427
public class ChickenCommonHelper
{
	// Token: 0x06001835 RID: 6197 RVA: 0x000949B4 File Offset: 0x00092BB4
	public static void SelectItemAndFeedChicken(int listenerId, IAsyncMethodRequestHandler requestHandler, int chichekId, Action<ItemKey> customCallback = null)
	{
		ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
		argumentBox.Set("showNone", true);
		argumentBox.SetObject("filterType", ItemSortAndFilter.ItemFilterType.Invalid);
		List<ItemDisplayData> feedItems = new List<ItemDisplayData>();
		Action<ItemKey> <>9__1;
		CharacterDomainMethod.AsyncCall.GetAllInventoryItems(requestHandler, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
		{
			List<ItemDisplayData> itemList = new List<ItemDisplayData>();
			Serializer.Deserialize(dataPool, offset, ref itemList);
			for (int i = 0; i < itemList.Count; i++)
			{
				bool flag = itemList[i].Key.ItemType == 11 || itemList[i].Key.ItemType == 5 || ItemTemplateHelper.GetItemSubType(itemList[i].Key.ItemType, itemList[i].Key.TemplateId) == 1204;
				if (flag)
				{
					feedItems.Add(itemList[i]);
				}
			}
			ArgumentBox argumentBox;
			argumentBox.SetObject("DisplayData", feedItems);
			argumentBox = argumentBox;
			string key = "callback";
			Action<ItemKey> arg;
			if ((arg = <>9__1) == null)
			{
				arg = (<>9__1 = delegate(ItemKey curItemKey)
				{
					bool flag2 = curItemKey.ItemType != -1;
					if (flag2)
					{
						BuildingDomainMethod.Call.FeedChicken(listenerId, chichekId, curItemKey);
					}
					Action<ItemKey> customCallback2 = customCallback;
					if (customCallback2 != null)
					{
						customCallback2(curItemKey);
					}
				});
			}
			argumentBox.SetObject(key, arg);
			UIElement.SelectItemLegacy.SetOnInitArgs(argumentBox);
			UIManager.Instance.ShowUI(UIElement.SelectItemLegacy, true);
		});
	}
}
