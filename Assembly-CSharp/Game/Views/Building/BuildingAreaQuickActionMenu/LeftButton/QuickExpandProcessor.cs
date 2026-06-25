using System;
using System.Collections.Generic;
using Config;
using Game.Components.Common;
using GameData.Domains.Building;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views.Building.BuildingAreaQuickActionMenu.LeftButton
{
	// Token: 0x02000C28 RID: 3112
	public class QuickExpandProcessor : LeftButtonProcessor
	{
		// Token: 0x06009E39 RID: 40505 RVA: 0x004A0914 File Offset: 0x0049EB14
		public QuickExpandProcessor(ViewBuildingQuickActionMenu menu, GameObject buttonObject, LeftButtonType type) : base(menu, buttonObject, type)
		{
			EventTrigger trigger = buttonObject.GetOrAddComponent<EventTrigger>();
			bool flag = trigger.triggers == null;
			if (flag)
			{
				trigger.triggers = new List<EventTrigger.Entry>();
			}
			EventTrigger.Entry downEntry = new EventTrigger.Entry
			{
				eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown
			};
			downEntry.callback.AddListener(delegate(BaseEventData data)
			{
				this._isHolding = true;
			});
			trigger.triggers.Add(downEntry);
			EventTrigger.Entry upEntry = new EventTrigger.Entry
			{
				eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp
			};
			upEntry.callback.AddListener(delegate(BaseEventData data)
			{
				this._isHolding = false;
			});
			trigger.triggers.Add(upEntry);
			buttonObject.GetOrAddComponent<Game.Components.Common.UIHoldButton>();
		}

		// Token: 0x170010BA RID: 4282
		// (get) Token: 0x06009E3A RID: 40506 RVA: 0x004A09B7 File Offset: 0x0049EBB7
		// (set) Token: 0x06009E3B RID: 40507 RVA: 0x004A09BF File Offset: 0x0049EBBF
		public int BuildingCoreItemCount { get; set; }

		// Token: 0x06009E3C RID: 40508 RVA: 0x004A09C8 File Offset: 0x0049EBC8
		public override void PrepareData()
		{
			short buildingCoreItemId = this._menu.ConfigData.BuildingCoreItem;
			bool flag = buildingCoreItemId < 0;
			if (flag)
			{
				this.OnCoreItemCountReady(0);
			}
			else
			{
				this._menu.FetchItemCount(buildingCoreItemId, delegate(int count)
				{
					this.OnCoreItemCountReady(count);
				});
			}
		}

		// Token: 0x06009E3D RID: 40509 RVA: 0x004A0A13 File Offset: 0x0049EC13
		private void OnCoreItemCountReady(int count)
		{
			this.BuildingCoreItemCount = count;
			base.UpdateVisibility();
			base.UpdateInteractivity();
			this.UpdateTipContent();
			this._tip.Refresh(false, -1);
		}

		// Token: 0x06009E3E RID: 40510 RVA: 0x004A0A44 File Offset: 0x0049EC44
		private void UpdateTipContent()
		{
			short buildingCoreItemId = this._menu.ConfigData.BuildingCoreItem;
			bool flag = buildingCoreItemId < 0;
			if (!flag)
			{
				MiscItem buildingCoreItemConfig = Misc.Instance[buildingCoreItemId];
				string color = (this.BuildingCoreItemCount >= 1) ? "brightblue" : "brightred";
				string haveCount = this.BuildingCoreItemCount.ToString().SetColor(color);
				string content = LanguageKey.LK_Building_QuickAction_Expand_Content.TrFormat(buildingCoreItemConfig.Name, haveCount, "1");
				string title = LanguageKey.LK_Building_QuickAction_Expand_Title.Tr();
				this._tip.PresetParam = new string[]
				{
					title,
					content
				};
			}
		}

		// Token: 0x06009E3F RID: 40511 RVA: 0x004A0AE8 File Offset: 0x0049ECE8
		public override bool IsVisible()
		{
			BuildingBlockItem configData = this._menu.ConfigData;
			BuildingBlockData blockData = this._menu.BlockData;
			int currentLevel = this._menu.BuildingModel.GetTaiwuSpecialBuildingLevel(this._menu.BlockKey);
			return (configData.Type == EBuildingBlockType.NormalResource || configData.Type == EBuildingBlockType.SpecialResource) && currentLevel < (int)configData.MaxLevel && blockData.OperationType == -1;
		}

		// Token: 0x06009E40 RID: 40512 RVA: 0x004A0B54 File Offset: 0x0049ED54
		public override bool CanInteract()
		{
			return this.IsVisible() && this.BuildingCoreItemCount >= 1;
		}

		// Token: 0x06009E41 RID: 40513 RVA: 0x004A0B80 File Offset: 0x0049ED80
		public override void OnClick()
		{
			bool flag = !this.CanInteract() || this._isCultivating;
			if (!flag)
			{
				this._isCultivating = true;
				BuildingDomainMethod.AsyncCall.UpgradeResourceBuilding(this._menu, this._menu.BlockKey, 1, new AsyncMethodCallbackDelegate(this.OnUpgradeComplete));
			}
		}

		// Token: 0x06009E42 RID: 40514 RVA: 0x004A0BD0 File Offset: 0x0049EDD0
		private void OnUpgradeComplete(int offset, RawDataPool pool)
		{
			bool success = false;
			Serializer.Deserialize(pool, offset, ref success);
			this._isCultivating = false;
			bool flag = success;
			if (flag)
			{
				this.PrepareData();
				bool flag2 = this._isHolding && this.CanInteract();
				if (flag2)
				{
					this.OnClick();
				}
			}
		}

		// Token: 0x06009E43 RID: 40515 RVA: 0x004A0C1D File Offset: 0x0049EE1D
		public override void OnHoverEnter()
		{
			this.UpdateTipContent();
		}

		// Token: 0x04007A74 RID: 31348
		private bool _isCultivating;

		// Token: 0x04007A75 RID: 31349
		private bool _isHolding;
	}
}
