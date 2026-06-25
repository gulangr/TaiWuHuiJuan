using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

namespace Game.Views.Building.BuildingManage.Production
{
	// Token: 0x02000C16 RID: 3094
	public static class ProductionHandlerExtensions
	{
		// Token: 0x170010A7 RID: 4263
		// (get) Token: 0x06009D4B RID: 40267 RVA: 0x0049AD5D File Offset: 0x00498F5D
		public static BuildingModel Model
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x06009D4C RID: 40268 RVA: 0x0049AD64 File Offset: 0x00498F64
		public static bool IsAuthorityEnoughToRecruit(this IProductionHandler _, int count = 1)
		{
			return ProductionHandlerExtensions.Model.GetResourceCount(7) >= _.AuthorityToRecruit(count);
		}

		// Token: 0x06009D4D RID: 40269 RVA: 0x0049AD7D File Offset: 0x00498F7D
		public static int AuthorityToRecruit(this IProductionHandler _, int count = 1)
		{
			return (int)GlobalConfig.Instance.RecruitPeopleCost * count;
		}

		// Token: 0x06009D4E RID: 40270 RVA: 0x0049AD8B File Offset: 0x00498F8B
		public static bool IsMoneyEnoughToBuy(this IProductionHandler _, ITradeableContent content)
		{
			return ProductionHandlerExtensions.Model.GetResourceCount(6) >= _.PawnShopCostMoney(content);
		}

		// Token: 0x06009D4F RID: 40271 RVA: 0x0049ADA4 File Offset: 0x00498FA4
		public static int PawnShopCostMoney(this IProductionHandler _, ITradeableContent content)
		{
			return ItemTemplateHelper.GetBaseValue(content.Key.ItemType, content.Key.TemplateId);
		}

		// Token: 0x06009D50 RID: 40272 RVA: 0x0049ADC1 File Offset: 0x00498FC1
		public static int RecruitRemainMonth(this IProductionHandler handler, int timePassed)
		{
			return SharedMethods.GetBuildingEarnPreserveTime(handler.TemplateId) - timePassed;
		}

		// Token: 0x06009D51 RID: 40273 RVA: 0x0049ADD0 File Offset: 0x00498FD0
		public static int PawnShopRemainMonth(this IProductionHandler handler, int timePassed)
		{
			return SharedMethods.GetBuildingEarnPreserveTime(handler.TemplateId) - timePassed;
		}

		// Token: 0x06009D52 RID: 40274 RVA: 0x0049ADE0 File Offset: 0x00498FE0
		public static void ShowGetPeopleView(this IProductionHandler _, List<int> charIdList)
		{
			bool flag = charIdList == null;
			if (!flag)
			{
				UIElement.GetItem.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("CharIdList", charIdList));
				UIManager.Instance.MaskUI(UIElement.GetItem);
			}
		}

		// Token: 0x06009D53 RID: 40275 RVA: 0x0049AE24 File Offset: 0x00499024
		public static void ShowGetItemView(this IProductionHandler _, List<ItemDisplayData> itemList)
		{
			bool flag = itemList == null;
			if (!flag)
			{
				UIElement.GetItem.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("ItemList", itemList));
				UIManager.Instance.MaskUI(UIElement.GetItem);
			}
		}

		// Token: 0x06009D54 RID: 40276 RVA: 0x0049AE68 File Offset: 0x00499068
		public static void ShowGetResourceView(this IProductionHandler _, List<ItemDisplayData> itemList)
		{
			bool flag = itemList == null;
			if (!flag)
			{
				UIElement.GetItem.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("ItemList", itemList));
				UIManager.Instance.MaskUI(UIElement.GetItem);
			}
		}
	}
}
