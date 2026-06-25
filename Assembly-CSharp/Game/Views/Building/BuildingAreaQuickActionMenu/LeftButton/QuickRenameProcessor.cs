using System;
using Config;
using GameData.Domains.Building;
using UnityEngine;

namespace Game.Views.Building.BuildingAreaQuickActionMenu.LeftButton
{
	// Token: 0x02000C2A RID: 3114
	public class QuickRenameProcessor : LeftButtonProcessor
	{
		// Token: 0x06009E50 RID: 40528 RVA: 0x004A0DF6 File Offset: 0x0049EFF6
		public QuickRenameProcessor(ViewBuildingQuickActionMenu menu, GameObject buttonObject, LeftButtonType type) : base(menu, buttonObject, type)
		{
		}

		// Token: 0x06009E51 RID: 40529 RVA: 0x004A0E03 File Offset: 0x0049F003
		public override void PrepareData()
		{
			base.UpdateVisibility();
			base.UpdateInteractivity();
		}

		// Token: 0x06009E52 RID: 40530 RVA: 0x004A0E14 File Offset: 0x0049F014
		public override bool IsVisible()
		{
			BuildingBlockItem configData = this._menu.ConfigData;
			return this._menu.IsTaiwuVillageBuilding && BuildingBlockData.IsBuilding(configData.Type);
		}

		// Token: 0x06009E53 RID: 40531 RVA: 0x004A0E50 File Offset: 0x0049F050
		public override bool CanInteract()
		{
			return this.IsVisible();
		}

		// Token: 0x06009E54 RID: 40532 RVA: 0x004A0E68 File Offset: 0x0049F068
		public override void OnClick()
		{
			bool flag = !this.CanInteract();
			if (!flag)
			{
				BuildingModel buildingModel = this._menu.BuildingModel;
				string currentName = buildingModel.CustomBuildingName.ContainsKey(this._menu.BlockKey) ? SingletonObject.getInstance<BasicGameData>().CustomTexts[buildingModel.CustomBuildingName[this._menu.BlockKey]] : "";
				this._menu.ShowRenameDialog(currentName);
			}
		}

		// Token: 0x06009E55 RID: 40533 RVA: 0x004A0EE4 File Offset: 0x0049F0E4
		public override void OnHoverEnter()
		{
			string content = LanguageKey.LK_Building_QuickAction_Rename_Content.Tr();
			string title = LanguageKey.LK_Building_QuickAction_Rename_Title.Tr();
			this._tip.PresetParam = new string[]
			{
				title,
				content
			};
		}
	}
}
