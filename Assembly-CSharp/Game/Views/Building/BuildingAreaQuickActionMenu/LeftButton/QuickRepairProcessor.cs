using System;
using Config;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Building.BuildingAreaQuickActionMenu.LeftButton
{
	// Token: 0x02000C2B RID: 3115
	public class QuickRepairProcessor : LeftButtonProcessor
	{
		// Token: 0x06009E56 RID: 40534 RVA: 0x004A0F21 File Offset: 0x0049F121
		public QuickRepairProcessor(ViewBuildingQuickActionMenu menu, GameObject buttonObject, LeftButtonType type) : base(menu, buttonObject, type)
		{
		}

		// Token: 0x06009E57 RID: 40535 RVA: 0x004A0F2E File Offset: 0x0049F12E
		public override void PrepareData()
		{
			base.UpdateVisibility();
			base.UpdateInteractivity();
		}

		// Token: 0x06009E58 RID: 40536 RVA: 0x004A0F40 File Offset: 0x0049F140
		public override bool IsVisible()
		{
			BuildingBlockItem configData = this._menu.ConfigData;
			BuildingBlockData blockData = this._menu.BlockData;
			return (configData.Type == EBuildingBlockType.Building || configData.Type == EBuildingBlockType.MainBuilding) && blockData.NeedMaintenanceCost() && this._menu.IsTaiwuVillageBuilding && blockData.OperationType == -1 && blockData.Durability < configData.MaxDurability;
		}

		// Token: 0x06009E59 RID: 40537 RVA: 0x004A0FAC File Offset: 0x0049F1AC
		public override bool CanInteract()
		{
			bool flag = !this.IsVisible();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = !SingletonObject.getInstance<FunctionLockManager>().IsFunctionUnlock(10);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = this._menu.BlockData.Durability >= this._menu.ConfigData.MaxDurability;
					result = (!flag3 && this.HasEnoughResources());
				}
			}
			return result;
		}

		// Token: 0x06009E5A RID: 40538 RVA: 0x004A101C File Offset: 0x0049F21C
		public override void OnClick()
		{
			bool flag = !this.CanInteract();
			if (!flag)
			{
				BuildingDomainMethod.AsyncCall.Repair(this._menu, this._menu.BlockKey, delegate(int offset, RawDataPool pool)
				{
					ValueTuple<short, BuildingBlockData> retValue = new ValueTuple<short, BuildingBlockData>(0, new BuildingBlockData());
					Serializer.Deserialize(pool, offset, ref retValue);
					GEvent.OnEvent(UiEvents.RepairBuilding, EasyPool.Get<ArgumentBox>().Set("BuildingBlockIndex", retValue.Item2.BlockIndex));
				});
				BuildingDomainMethod.Call.GetBuildingBlockList(this._menu.Element.GameDataListenerId, new Location(this._menu.AreaId, this._menu.BlockId));
			}
		}

		// Token: 0x06009E5B RID: 40539 RVA: 0x004A10A0 File Offset: 0x0049F2A0
		public override void OnHoverEnter()
		{
			BuildingBlockItem configData = this._menu.ConfigData;
			BuildingBlockData blockData = this._menu.BlockData;
			BuildingModel buildingModel = this._menu.BuildingModel;
			int repairCostCount = GameData.Domains.Building.SharedMethods.CalcRepairBuildingCost(blockData, configData);
			int moneyOwned = buildingModel.GetResourceCount(6);
			string countStr = CommonUtils.GetColoredStringByCompare(moneyOwned, repairCostCount, repairCostCount.CompareTo(moneyOwned), false);
			ResourceTypeItem moneyConfig = ResourceType.Instance[6];
			string content = LanguageKey.LK_Building_QuickAction_Repair_Content.TrFormat(moneyConfig.Icon, countStr);
			string title = LanguageKey.LK_Building_QuickAction_Repair_Title.Tr();
			this._tip.PresetParam = new string[]
			{
				title,
				content
			};
		}

		// Token: 0x06009E5C RID: 40540 RVA: 0x004A1140 File Offset: 0x0049F340
		private bool HasEnoughResources()
		{
			BuildingBlockData blockData = this._menu.BlockData;
			BuildingBlockItem configData = this._menu.ConfigData;
			BuildingModel buildingModel = this._menu.BuildingModel;
			int repairCostCount = GameData.Domains.Building.SharedMethods.CalcRepairBuildingCost(blockData, configData);
			int moneyOwned = buildingModel.GetResourceCount(6);
			bool flag = moneyOwned < repairCostCount;
			return !flag;
		}
	}
}
