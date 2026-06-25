using System;
using System.Collections.Generic;
using FrameWork;
using Game.Views.Select;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C08 RID: 3080
	public class ChickenCommonHelper
	{
		// Token: 0x06009C7C RID: 40060 RVA: 0x0049487C File Offset: 0x00492A7C
		public static void SelectItemAndFeedChicken(int listenerId, IAsyncMethodRequestHandler requestHandler, int chichekId, Action<ItemKey> customCallback = null)
		{
			List<ItemDisplayData> feedItems = new List<ItemDisplayData>();
			SelectItemsCallback <>9__1;
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
				SelectItemRules selectItemRules = new SelectItemRules();
				selectItemRules.OnlyFromInventory = true;
				SelectItemsCallback callback;
				if ((callback = <>9__1) == null)
				{
					callback = (<>9__1 = delegate(List<SelectedItemData> itemSelected)
					{
						bool flag2 = itemSelected != null && itemSelected.Count > 0;
						if (flag2)
						{
							SelectedItemData selectedItem = itemSelected[0];
							bool flag3 = !selectedItem.IsCancelled && selectedItem.ItemData != null;
							if (flag3)
							{
								ItemKey curItemKey = selectedItem.ItemData.Key;
								BuildingDomainMethod.Call.FeedChicken(listenerId, chichekId, curItemKey);
								Action<ItemKey> customCallback2 = customCallback;
								if (customCallback2 != null)
								{
									customCallback2(curItemKey);
								}
							}
						}
					});
				}
				SelectItemConfig config = SelectItemConfig.CreateMultipleSelectConfig(selectItemRules, callback, "", 0, -1, null);
				config.OperationMode = ESelectItemOperationMode.NoPreSelect;
				config.MaxSelectCount = 1;
				config.ExternalItems = feedItems;
				config.InitialSelectedItems = new List<SelectedItemData>();
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("DisplayBg", true);
				argBox.SetObject("SelectItemConfig", config);
				UIElement.SelectItem.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.SelectItem, true);
			});
		}
	}
}
