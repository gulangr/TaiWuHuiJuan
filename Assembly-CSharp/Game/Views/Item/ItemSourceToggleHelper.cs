using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;

namespace Game.Views.Item
{
	// Token: 0x02000A13 RID: 2579
	public static class ItemSourceToggleHelper
	{
		// Token: 0x06007E38 RID: 32312 RVA: 0x003AA1B8 File Offset: 0x003A83B8
		public static void RefreshInteractableForInteract(CToggleGroup toggleGroup, bool isInSettlement, bool forceShowStock = false)
		{
			bool canShowTreasury = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
			int inventoryIndex = EItemSourceToggleKey.Inventory.ToInt();
			int warehouseIndex = EItemSourceToggleKey.Warehouse.ToInt();
			List<CToggle> all = toggleGroup.GetAll();
			for (int index = 0; index < all.Count; index++)
			{
				CToggle toggle = all[index];
				toggle.interactable = (isInSettlement || index == inventoryIndex);
				bool isShow = canShowTreasury || index <= warehouseIndex;
				bool flag = index == EItemSourceToggleKey.Stock.ToInt() && !forceShowStock;
				if (flag)
				{
					isShow = false;
				}
				toggle.gameObject.SetActive(isShow);
				TooltipInvoker tip = toggle.GetComponentInChildren<TooltipInvoker>();
				bool flag2 = tip;
				if (flag2)
				{
					tip.enabled = !toggle.interactable;
				}
			}
			bool flag3 = !isInSettlement && toggleGroup.GetActiveIndex() > inventoryIndex;
			if (flag3)
			{
				toggleGroup.Set(inventoryIndex, false);
			}
		}

		// Token: 0x06007E39 RID: 32313 RVA: 0x003AA2B8 File Offset: 0x003A84B8
		public static void RefreshInteractableForBuilding(CToggleGroup toggleGroup, bool isInSettlement)
		{
			bool canShowTreasury = SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
			int inventoryIndex = EItemSourceToggleKey.Inventory.ToInt();
			int warehouseIndex = EItemSourceToggleKey.Warehouse.ToInt();
			List<CToggle> all = toggleGroup.GetAll();
			for (int index = 0; index < all.Count; index++)
			{
				CToggle toggle = all[index];
				toggle.interactable = (isInSettlement || index > inventoryIndex);
				bool isShow = canShowTreasury || index <= warehouseIndex;
				bool flag = index == EItemSourceToggleKey.Stock.ToInt();
				if (flag)
				{
					isShow = false;
				}
				toggle.gameObject.SetActive(isShow);
				TooltipInvoker tip = toggle.GetComponentInChildren<TooltipInvoker>();
				bool flag2 = tip;
				if (flag2)
				{
					tip.enabled = !toggle.interactable;
				}
			}
			bool flag3 = !isInSettlement && toggleGroup.GetActiveIndex() == inventoryIndex;
			if (flag3)
			{
				toggleGroup.Set(EItemSourceToggleKey.Warehouse.ToInt(), false);
			}
		}
	}
}
