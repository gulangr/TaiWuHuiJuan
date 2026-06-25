using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using Game.Views.Building.BuildingManage;
using Game.Views.Make;
using Game.Views.VillagerRoleView;
using GameData.Domains.Building;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Building.BuildingAreaQuickActionMenu
{
	// Token: 0x02000C1F RID: 3103
	public static class BuildingActionUtils
	{
		// Token: 0x06009D9C RID: 40348 RVA: 0x0049D645 File Offset: 0x0049B845
		public static void OpenBuildingManage(BuildingBlockKey blockKey, Game.Views.Building.BuildingManage.BuildingManageTogKey tabKey = Game.Views.Building.BuildingManage.BuildingManageTogKey.Invalid)
		{
			GEvent.OnEvent(UiEvents.RequestOpenBuildingManage, EasyPool.Get<ArgumentBox>().SetObject("buildingKey", blockKey).Set("tabKey", tabKey));
		}

		// Token: 0x06009D9D RID: 40349 RVA: 0x0049D67D File Offset: 0x0049B87D
		public static void ShowCricketCollection()
		{
			UIManager.Instance.ShowUI(UIElement.CricketCollection, true);
		}

		// Token: 0x06009D9E RID: 40350 RVA: 0x0049D691 File Offset: 0x0049B891
		public static void ShowStoneHouse(sbyte level)
		{
			UIElement.TaiwuVillageStoneRoom.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("villageLevel", level));
			UIManager.Instance.ShowUI(UIElement.TaiwuVillageStoneRoom, true);
		}

		// Token: 0x06009D9F RID: 40351 RVA: 0x0049D6C0 File Offset: 0x0049B8C0
		public static void ShowStoneHouse(BuildingBlockKey blockKey)
		{
			BuildingActionUtils.ShowStoneHouse(SingletonObject.getInstance<BuildingModel>().GetTaiwuBuildingData(blockKey).CalcUnlockedLevelCount());
		}

		// Token: 0x06009DA0 RID: 40352 RVA: 0x0049D6D8 File Offset: 0x0049B8D8
		public static void ShowStoneHouse()
		{
			BuildingBlockData blockDataEx;
			bool building = SingletonObject.getInstance<BuildingModel>().GetBuilding(44, out blockDataEx);
			if (building)
			{
				BuildingActionUtils.ShowStoneHouse(blockDataEx.CalcUnlockedLevelCount());
			}
			else
			{
				Debug.LogError("cannot find BuildingBlock.DefKey.TaiwuVillage");
			}
		}

		// Token: 0x06009DA1 RID: 40353 RVA: 0x0049D711 File Offset: 0x0049B911
		public static void ShowJiaoPool(sbyte landFormType)
		{
			UIElement.BuildingJiaoPool.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("landFormType", landFormType));
			UIManager.Instance.ShowUI(UIElement.BuildingJiaoPool, true);
		}

		// Token: 0x06009DA2 RID: 40354 RVA: 0x0049D740 File Offset: 0x0049B940
		public static void ShowJiaoPool(BuildingAreaData areaData)
		{
			BuildingActionUtils.ShowJiaoPool(areaData.LandFormType);
		}

		// Token: 0x06009DA3 RID: 40355 RVA: 0x0049D74E File Offset: 0x0049B94E
		public static void ShowJiaoPool(IAsyncMethodRequestHandler parent = null)
		{
			BuildingDomainMethod.AsyncCall.GetTaiwuVillageBuildingAreaData(parent, delegate(int offset, RawDataPool pool)
			{
				BuildingAreaData areaData = new BuildingAreaData();
				Serializer.Deserialize(pool, offset, ref areaData);
				BuildingActionUtils.ShowJiaoPool(areaData.LandFormType);
			});
		}

		// Token: 0x06009DA4 RID: 40356 RVA: 0x0049D776 File Offset: 0x0049B976
		public static void ShowKungfuPracticeRoom()
		{
			UIManager.Instance.ShowUI(UIElement.KungfuPracticeRoomPuppet, true);
		}

		// Token: 0x06009DA5 RID: 40357 RVA: 0x0049D78A File Offset: 0x0049B98A
		public static void ShowSamsaraPlatform(BuildingBlockKey blockKey)
		{
			SingletonObject.getInstance<CharacterMonitorModel>().RefreshAllMonitorCharacterAliveState();
			UIElement.SamsaraPlatform.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("BuildingKey", blockKey));
			UIManager.Instance.ShowUI(UIElement.SamsaraPlatform, true);
		}

		// Token: 0x06009DA6 RID: 40358 RVA: 0x0049D7CC File Offset: 0x0049B9CC
		public static void ShowSamsaraPlatform()
		{
			BuildingBlockKey key;
			BuildingBlockData buildingBlockData;
			bool building = SingletonObject.getInstance<BuildingModel>().GetBuilding(50, out key, out buildingBlockData);
			if (building)
			{
				BuildingActionUtils.ShowSamsaraPlatform(key);
			}
			else
			{
				Debug.LogError("cannot find BuildingBlock.DefKey.SamsaraPlatform");
			}
		}

		// Token: 0x06009DA7 RID: 40359 RVA: 0x0049D802 File Offset: 0x0049BA02
		public static void ShowSwapSoul()
		{
			UIManager.Instance.ShowUI(UIElement.SwapSoul, true);
		}

		// Token: 0x06009DA8 RID: 40360 RVA: 0x0049D816 File Offset: 0x0049BA16
		public static void ShowMonkSoul()
		{
			BuildingDomainMethod.Call.SectMainStoryJingangClickMonkSoulBtn();
		}

		// Token: 0x06009DA9 RID: 40361 RVA: 0x0049D820 File Offset: 0x0049BA20
		public static void ShowChickenCoop(Location location)
		{
			BuildingAreaData areaData = new BuildingAreaData();
			BuildingDomainMethod.AsyncCall.GetBuildingAreaData(null, location, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref areaData);
			});
			BuildingBlockData buildingBlockData = null;
			BuildingDomainMethod.AsyncCall.GetBuildingBlockList(null, location, delegate(int offset, RawDataPool dataPool)
			{
				List<BuildingBlockData> list = new List<BuildingBlockData>();
				Serializer.Deserialize(dataPool, offset, ref list);
				buildingBlockData = list.FirstOrDefault((BuildingBlockData x) => x.TemplateId == 49);
			});
			MapDomainMethod.AsyncCall.GetBlockData(null, location.AreaId, location.BlockId, delegate(int offset, RawDataPool dataPool)
			{
				MapBlockData block = new MapBlockData();
				Serializer.Deserialize(dataPool, offset, ref block);
				ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
				argsBox.SetObject("MapBlockData", block);
				argsBox.SetObject("BuildingBlockData", buildingBlockData);
				argsBox.Set<BuildingAreaData>("BuildingAreaData", areaData);
				argsBox.Set("tabKey", Game.Views.Building.BuildingManage.BuildingManageTogKey.Chicken);
				UIElement.BuildingManage.SetOnInitArgs(argsBox);
				UIManager.Instance.ShowUI(UIElement.BuildingManage, true);
			});
		}

		// Token: 0x06009DAA RID: 40362 RVA: 0x0049D88D File Offset: 0x0049BA8D
		public static void ShowChickenCoop(BuildingBlockKey blockKey)
		{
			BuildingActionUtils.ShowChickenCoop(new Location(blockKey.AreaId, blockKey.BlockId));
		}

		// Token: 0x06009DAB RID: 40363 RVA: 0x0049D8A6 File Offset: 0x0049BAA6
		public static void ShowChickenCoop()
		{
			BuildingActionUtils.ShowChickenCoop(SingletonObject.getInstance<WorldMapModel>().GetTaiwuVillageBlock());
		}

		// Token: 0x06009DAC RID: 40364 RVA: 0x0049D8B8 File Offset: 0x0049BAB8
		public static void ShowTrough()
		{
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			argumentBox.Clear();
			argumentBox.SetObject("WarehouseItemSourceType", ItemSourceType.Trough);
			UIElement.Warehouse.SetOnInitArgs(argumentBox);
			UIManager.Instance.ShowUI(UIElement.Warehouse, true);
		}

		// Token: 0x06009DAD RID: 40365 RVA: 0x0049D904 File Offset: 0x0049BB04
		public static void ShowVillageRole()
		{
			ArgumentBox argbox = EasyPool.Get<ArgumentBox>();
			argbox.Set("EnterType", ViewVillagerRole.EnterType.Normal);
			argbox.Set("EnterPage", ViewVillagerRole.EVillagerRolePage.RoleDescription);
			UIElement.VillagerRole.SetOnInitArgs(argbox);
			UIManager.Instance.ShowUI(UIElement.VillagerRole, true);
		}

		// Token: 0x06009DAE RID: 40366 RVA: 0x0049D95C File Offset: 0x0049BB5C
		public static void ShowVillageRoleChickenAssign()
		{
			ArgumentBox argbox = EasyPool.Get<ArgumentBox>();
			argbox.Set("EnterType", ViewVillagerRole.EnterType.Normal);
			argbox.Set("EnterPage", ViewVillagerRole.EVillagerRolePage.ChickenAssign);
			UIElement.VillagerRole.SetOnInitArgs(argbox);
			UIManager.Instance.ShowUI(UIElement.VillagerRole, true);
		}

		// Token: 0x06009DAF RID: 40367 RVA: 0x0049D9B4 File Offset: 0x0049BBB4
		public static void ShowTaiwuLifeSummary()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			UIElement.TaiwuLifeSummary.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.TaiwuLifeSummary, true);
		}

		// Token: 0x06009DB0 RID: 40368 RVA: 0x0049D9E5 File Offset: 0x0049BBE5
		public static void ShowTeaHorseCaravan(BuildingBlockData blockData, BuildingBlockKey blockKey)
		{
			UIElement.TeaHorseCaravan.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("BuildingBlockData", blockData).SetObject("BuildingBlockKey", blockKey));
			UIManager.Instance.ShowUI(UIElement.TeaHorseCaravan, true);
		}

		// Token: 0x06009DB1 RID: 40369 RVA: 0x0049DA24 File Offset: 0x0049BC24
		public static void ShowTeaHorseCaravan()
		{
			BuildingBlockKey key;
			BuildingBlockData data;
			bool building = SingletonObject.getInstance<BuildingModel>().GetBuilding(51, out key, out data);
			if (building)
			{
				BuildingActionUtils.ShowTeaHorseCaravan(data, key);
			}
			else
			{
				Debug.LogError("cannot find BuildingBlock.DefKey.SamsaraPlatform");
			}
		}

		// Token: 0x06009DB2 RID: 40370 RVA: 0x0049DA5C File Offset: 0x0049BC5C
		public static void ShowWarehouse(ItemSourceType itemSourceType)
		{
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			argumentBox.Clear();
			argumentBox.SetObject("WarehouseItemSourceType", itemSourceType);
			UIElement.Warehouse.SetOnInitArgs(argumentBox);
			UIManager.Instance.ShowUI(UIElement.Warehouse, true);
		}

		// Token: 0x06009DB3 RID: 40371 RVA: 0x0049DAA8 File Offset: 0x0049BCA8
		public static void ShowMerchant(BuildingBlockItem configData, short areaId)
		{
			short areaTemplateId = SingletonObject.getInstance<WorldMapModel>().Areas[(int)areaId].GetTemplateId();
			MerchantTypeItem merchantTypeConfig = Config.MerchantType.Instance[configData.MerchantId];
			bool isHeadBuildingMerchant = merchantTypeConfig.HeadArea == areaTemplateId;
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			argumentBox.Clear();
			OpenShopEventArguments.EMerchantSourceType merchantSourceType = isHeadBuildingMerchant ? OpenShopEventArguments.EMerchantSourceType.MerchantHeadBuilding : OpenShopEventArguments.EMerchantSourceType.MerchantBranchBuilding;
			OpenShopEventArguments openShopEventArguments = new OpenShopEventArguments
			{
				BuildingMerchantType = configData.MerchantId,
				MerchantSourceType = (sbyte)merchantSourceType
			};
			argumentBox.SetObject("OpenShopEventArguments", openShopEventArguments);
			UIElement.NewShop.SetOnInitArgs(argumentBox);
			UIManager.Instance.ShowUI(UIElement.NewShop, true);
		}

		// Token: 0x06009DB4 RID: 40372 RVA: 0x0049DB40 File Offset: 0x0049BD40
		public static void ShowSpecialShop(BuildingBlockItem configData)
		{
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
			argumentBox.Clear();
			OpenShopEventArguments openShopEventArguments = new OpenShopEventArguments
			{
				BuildingMerchantType = configData.MerchantId,
				MerchantSourceType = OpenShopEventArguments.EMerchantSourceType.SpecialBuilding.ToSbyte()
			};
			argumentBox.SetObject("OpenShopEventArguments", openShopEventArguments);
			UIElement.NewShop.SetOnInitArgs(argumentBox);
			UIManager.Instance.ShowUI(UIElement.NewShop, true);
		}

		// Token: 0x06009DB5 RID: 40373 RVA: 0x0049DBA8 File Offset: 0x0049BDA8
		public static void ShowTreasuryShop(BuildingBlockData blockData)
		{
			TaiwuEventDomainMethod.Call.SettlementTreasuryBuildingClicked(blockData.TemplateId, 0, 0);
		}

		// Token: 0x06009DB6 RID: 40374 RVA: 0x0049DBB8 File Offset: 0x0049BDB8
		public static void ShowMake(BuildingBlockData blockData, BuildingBlockKey blockKey, UI_Make.UIMakeTab tab)
		{
			bool key = Input.GetKey(KeyCode.LeftControl);
			if (key)
			{
				ArgumentBox argumentBox = UI_Make.GetMakeBuildingInfo(blockData, blockKey);
				argumentBox.SetObject("Tab", tab);
				UIElement.MakeOld.SetOnInitArgs(argumentBox);
				UIManager.Instance.ShowUI(UIElement.MakeOld, true);
			}
			else
			{
				ArgumentBox argumentBox2 = ViewMake.GetMakeBuildingInfo(blockData, blockKey, MakeTogKey.Invalid);
				argumentBox2.SetObject("Tab", (MakeTogKey)(tab - 1));
				UIElement.Make.SetOnInitArgs(argumentBox2);
				UIManager.Instance.ShowUI(UIElement.Make, true);
			}
		}

		// Token: 0x06009DB7 RID: 40375 RVA: 0x0049DC4C File Offset: 0x0049BE4C
		public static void ShowCraftMan(BuildingBlockData blockData, BuildingBlockKey blockKey)
		{
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().SetObject("blockKey", blockKey).SetObject("blockData", blockData);
			UIElement.Craftsman.SetOnInitArgs(argumentBox);
			UIManager.Instance.ShowUI(UIElement.Craftsman, true);
		}

		// Token: 0x06009DB8 RID: 40376 RVA: 0x0049DC98 File Offset: 0x0049BE98
		public static void ShowPrison()
		{
			TaiwuEventDomainMethod.Call.OnClickedPrisonBtn(-1);
		}

		// Token: 0x06009DB9 RID: 40377 RVA: 0x0049DCA4 File Offset: 0x0049BEA4
		public static void ShowBounty(BuildingBlockKey blockKey)
		{
			SettlementInfo settlementInfo = SingletonObject.getInstance<WorldMapModel>().GetLocationOrganizationInfo(blockKey.GetLocation());
			UIElement.SettlementBounty.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("SettlementId", settlementInfo.SettlementId));
			UIManager.Instance.MaskUI(UIElement.SettlementBounty);
		}

		// Token: 0x06009DBA RID: 40378 RVA: 0x0049DCF4 File Offset: 0x0049BEF4
		public static void ShowLaw(BuildingBlockKey blockKey)
		{
			sbyte stateID = MapArea.Instance[blockKey.AreaId].StateID;
			ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("IsSect", true).Set("StateTemplateId", stateID);
			UIElement.SectLaw.SetOnInitArgs(args);
			UIManager.Instance.ShowUI(UIElement.SectLaw, true);
		}

		// Token: 0x06009DBB RID: 40379 RVA: 0x0049DD54 File Offset: 0x0049BF54
		internal static List<float> CalculateGroupAngles(float centerAngle, int buttonCount, float spacingAngle)
		{
			List<float> angles = new List<float>();
			bool flag = buttonCount == 0;
			List<float> result;
			if (flag)
			{
				result = angles;
			}
			else
			{
				bool flag2 = buttonCount == 1;
				if (flag2)
				{
					angles.Add(centerAngle);
					result = angles;
				}
				else
				{
					int halfCount = buttonCount / 2;
					bool hasCenterButton = buttonCount % 2 == 1;
					bool flag3 = hasCenterButton;
					if (flag3)
					{
						angles.Add(centerAngle);
						for (int i = 1; i <= halfCount; i++)
						{
							angles.Insert(0, centerAngle - (float)i * spacingAngle);
							angles.Add(centerAngle + (float)i * spacingAngle);
						}
					}
					else
					{
						for (int j = 0; j < halfCount; j++)
						{
							angles.Insert(0, centerAngle - ((float)j + 0.5f) * spacingAngle);
							angles.Add(centerAngle + ((float)j + 0.5f) * spacingAngle);
						}
					}
					result = angles;
				}
			}
			return result;
		}

		// Token: 0x06009DBC RID: 40380 RVA: 0x0049DE34 File Offset: 0x0049C034
		public static Vector2 PolarToCartesian(float angle, float radius)
		{
			float radian = angle * 0.017453292f;
			float x = Mathf.Sin(radian) * radius;
			float y = Mathf.Cos(radian) * radius;
			return new Vector2(x, y);
		}
	}
}
