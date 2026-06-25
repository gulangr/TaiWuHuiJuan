using System;
using System.Collections.Generic;
using System.Linq;
using CharacterDataMonitor;
using FrameWork;
using Game.Views;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

// Token: 0x02000128 RID: 296
public static class EatingUtils
{
	// Token: 0x06000C6C RID: 3180 RVA: 0x000515A4 File Offset: 0x0004F7A4
	public static void TryRequestAddEatingItems(int charId, ITradeableContent itemData, int count, Action onRequest, bool preview = false, List<sbyte> targetBodyParts = null)
	{
		EatingUtils.<>c__DisplayClass0_0 CS$<>8__locals1 = new EatingUtils.<>c__DisplayClass0_0();
		CS$<>8__locals1.preview = preview;
		CS$<>8__locals1.onRequest = onRequest;
		CS$<>8__locals1.itemData = itemData;
		CS$<>8__locals1.charId = charId;
		CS$<>8__locals1.count = count;
		CS$<>8__locals1.targetBodyParts = targetBodyParts;
		EatingItemMonitor monitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EatingItemMonitor>(CS$<>8__locals1.charId, false);
		bool eatingWugKing = monitor.EatingItemList.Any((ValueTuple<ItemKey, short> x) => EatingItems.IsWugKing(x.Item1));
		bool flag = EatingItems.IsWugKing(CS$<>8__locals1.itemData.Key) && (eatingWugKing || CS$<>8__locals1.count > 1);
		if (flag)
		{
			DialogCmd cmd = new DialogCmd
			{
				Title = LocalStringManager.Get(LanguageKey.LK_Eating_Wug_King_Replace_Title),
				Content = LocalStringManager.Get(LanguageKey.LK_Eating_Wug_King_Replace_Content),
				Yes = new Action(CS$<>8__locals1.<TryRequestAddEatingItems>g__Execute|1)
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}
		else
		{
			CS$<>8__locals1.<TryRequestAddEatingItems>g__Execute|1();
		}
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x000516C0 File Offset: 0x0004F8C0
	private static void DoRequestAddEatingItems(int charId, ITradeableContent itemData, int count, List<sbyte> targetBodyParts, Action onRequest)
	{
		Inventory inventory = itemData.GetOperationInventoryFromPool(count, false);
		itemData.ChangeAmount(inventory, false);
		foreach (KeyValuePair<ItemKey, int> keyValuePair in inventory.Items)
		{
			ItemKey itemKey;
			int num;
			keyValuePair.Deconstruct(out itemKey, out num);
			ItemKey key = itemKey;
			int amount = num;
			for (int i = 0; i < amount; i++)
			{
				CharacterDomainMethod.Call.AddEatingItem(charId, key, targetBodyParts);
			}
		}
		ItemDisplayData.ReturnInventoryToPool(inventory);
		if (onRequest != null)
		{
			onRequest();
		}
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x00051768 File Offset: 0x0004F968
	public static void TryRequestEatTianJieFuLu(int charId, ITradeableContent itemData, int count, Action onRequest, bool preview = false)
	{
		bool flag = !preview;
		if (flag)
		{
			ExtraDomainMethod.Call.EatTianJieFuLu(charId, itemData.Key, count);
		}
		ViewPopupMenu popupMenu = UIElement.PopupMenu.UiBaseAs<ViewPopupMenu>();
		bool flag2 = popupMenu != null;
		if (flag2)
		{
			popupMenu.ConfirmSound = "ui_charactermenu_eat";
		}
		ViewSetSelectCount setSelectCount = UIElement.SetSelectCount.UiBaseAs<ViewSetSelectCount>();
		bool flag3 = setSelectCount != null;
		if (flag3)
		{
			setSelectCount.ConfirmSound = "ui_charactermenu_eat";
		}
		if (onRequest != null)
		{
			onRequest();
		}
	}
}
