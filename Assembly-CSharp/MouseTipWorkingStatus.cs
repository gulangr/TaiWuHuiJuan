using System;
using Config;
using FrameWork;
using Game.Views.Building;
using GameData.Domains.Building;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.Taiwu.Display.VillagerRoleArrangement;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;

// Token: 0x020002E3 RID: 739
public class MouseTipWorkingStatus : MouseTipBase
{
	// Token: 0x170004D5 RID: 1237
	// (get) Token: 0x06002BBD RID: 11197 RVA: 0x00155D7F File Offset: 0x00153F7F
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170004D6 RID: 1238
	// (get) Token: 0x06002BBE RID: 11198 RVA: 0x00155D82 File Offset: 0x00153F82
	private BuildingModel _buildingModel
	{
		get
		{
			return SingletonObject.getInstance<BuildingModel>();
		}
	}

	// Token: 0x06002BBF RID: 11199 RVA: 0x00155D8C File Offset: 0x00153F8C
	protected override void Init(ArgumentBox argsBox)
	{
		this._titleText = base.CGet<TextMeshProUGUI>("Title");
		base.CGet<Refers>("LineType_Location").gameObject.SetActive(false);
		base.CGet<Refers>("LineType_Building").gameObject.SetActive(false);
		argsBox.Get("charId", out this._charId);
		argsBox.Get("arrangementTemplateId", out this._arrangementTemplateId);
		argsBox.Get<VillagerRoleCharacterDisplayData>("displayData", out this._displayData);
		this.Refresh(argsBox);
	}

	// Token: 0x06002BC0 RID: 11200 RVA: 0x00155E18 File Offset: 0x00154018
	public override void Refresh(ArgumentBox argBox)
	{
		base.Refresh();
		bool disableRaycastTarget = this._disableRaycastTarget;
		if (disableRaycastTarget)
		{
			base.gameObject.GetComponent<CImage>().raycastTarget = false;
		}
		string statusName = string.Empty;
		BuildingModel _buildingModel = SingletonObject.getInstance<BuildingModel>();
		VillagerWorkData workingData;
		bool flag = _buildingModel.VillagerWork.TryGetValue(this._charId, out workingData) && (workingData.WorkType == 0 || workingData.WorkType == 1 || workingData.WorkType >= 10);
		if (flag)
		{
			bool flag2 = workingData.WorkType == 1 || workingData.WorkType == 0;
			if (flag2)
			{
				BuildingBlockKey blockKey = new BuildingBlockKey(workingData.AreaId, workingData.BlockId, workingData.BuildingBlockIndex);
				BuildingBlockData buildingBlockData = new BuildingBlockData();
				BuildingDomainMethod.AsyncCall.GetBuildingBlockData(null, blockKey, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref buildingBlockData);
					bool flag12 = workingData.WorkType == 0;
					if (flag12)
					{
						bool flag13 = buildingBlockData.OperationType == 1;
						if (flag13)
						{
							statusName = LocalStringManager.Get(LanguageKey.LK_VillagerSelection_Removing);
						}
						else
						{
							statusName = LocalStringManager.Get(LanguageKey.LK_VillagerSelection_Building);
						}
					}
					else
					{
						bool flag14 = workingData.WorkType == 1;
						if (flag14)
						{
							statusName = LocalStringManager.Get(LanguageKey.LK_VillagerSelection_Building);
							BuildingBlockItem buildingBlockConfig = BuildingBlock.Instance[buildingBlockData.TemplateId];
							bool artisanOrderAvailable = buildingBlockConfig.ArtisanOrderAvailable;
							if (artisanOrderAvailable)
							{
								statusName = LocalStringManager.Get(LanguageKey.LK_VillagerSelection_Crafting);
							}
							else
							{
								statusName = LocalStringManager.Get(LanguageKey.LK_VillagerSelection_Managing);
							}
						}
					}
					this.SetStatusName(statusName);
					this.SetBlockNameBuilding(buildingBlockData, blockKey);
				});
			}
			else
			{
				bool flag3 = workingData.WorkType == 11;
				if (flag3)
				{
					statusName = LocalStringManager.Get(LanguageKey.LK_VillagerSelection_Supervising);
					this.SetBlockNameAdventure(this._charId);
				}
				else
				{
					bool flag4 = workingData.WorkType == 10;
					if (flag4)
					{
						statusName = LocalStringManager.Get(LanguageKey.LK_VillagerSelection_Collecting);
						this.SetBlockNameNormal(this._charId);
					}
					else
					{
						bool flag5 = workingData.WorkType == 14;
						if (flag5)
						{
							statusName = LocalStringManager.Get(LanguageKey.LK_VillagerSelection_Migrating);
							this.SetBlockNameNormal(this._charId);
						}
						else
						{
							bool flag6 = workingData.WorkType == 12;
							if (flag6)
							{
								statusName = LocalStringManager.Get(LanguageKey.LK_VillagerSelection_GraveKeeping);
								this.SetBlockNameNormal(this._charId);
							}
						}
					}
				}
				this.SetStatusName(statusName);
			}
		}
		else
		{
			bool flag7 = this._arrangementTemplateId == 6;
			if (flag7)
			{
				statusName = LocalStringManager.Get(LanguageKey.LK_VillagerSelection_Treating);
				this.SetBlockNameNormal(this._charId);
			}
			else
			{
				bool flag8 = this._arrangementTemplateId == 8;
				if (flag8)
				{
					statusName = LocalStringManager.Get(LanguageKey.LK_VillagerSelection_Merchant);
					this.SetBlockNameNormal(this._charId);
				}
				else
				{
					bool flag9 = this._arrangementTemplateId == 11;
					if (flag9)
					{
						statusName = LocalStringManager.Get(LanguageKey.LK_VillagerSelection_Tripping);
						this.SetBlockNameNormal(this._charId);
					}
					else
					{
						bool flag10 = this._arrangementTemplateId == 13;
						if (flag10)
						{
							statusName = LocalStringManager.Get(LanguageKey.LK_VillagerSelection_Guarding);
							this.SetBlockNameSwordTomb(this._charId);
						}
						else
						{
							bool flag11 = this._arrangementTemplateId == 15;
							if (flag11)
							{
								statusName = LocalStringManager.Get(LanguageKey.LK_VillagerSelection_Visiting);
								this.SetBlockNameNormal(this._charId);
							}
						}
					}
				}
			}
			this.SetStatusName(statusName);
		}
	}

	// Token: 0x06002BC1 RID: 11201 RVA: 0x00156150 File Offset: 0x00154350
	private void SetBlockNameBuilding(BuildingBlockData buildingBlockData, BuildingBlockKey blockKey)
	{
		BuildingBlockItem config = BuildingBlock.Instance[buildingBlockData.TemplateId];
		Refers lineObj = base.CGet<Refers>("LineType_Building");
		lineObj.gameObject.SetActive(true);
		TextMeshProUGUI txtContent = lineObj.CGet<TextMeshProUGUI>("Text");
		ViewBuildingArea.SetBuildingName(txtContent, config, blockKey, 0, false);
		lineObj.CGet<TextMeshProUGUI>("levelText").SetText(this._buildingModel.GetBuildingLevel(blockKey, buildingBlockData).ToString(), true);
		bool flag = config.BuildingAreaLevelInfoBackendPattern != null && config.BuildingAreaLevelInfoBackendPattern.Length != 0;
		string spriteIcon;
		if (flag)
		{
			spriteIcon = config.BuildingAreaLevelInfoBackendPattern[0];
		}
		else
		{
			spriteIcon = "mousetip_buildinglevel";
		}
		lineObj.CGet<CImage>("Image").SetSprite(spriteIcon, false, null);
		txtContent.GetComponent<TMPTextSpriteHelper>().Parse();
	}

	// Token: 0x06002BC2 RID: 11202 RVA: 0x00156218 File Offset: 0x00154418
	private void SetBlockNameNormal(int characterId)
	{
		Refers lineObj = base.CGet<Refers>("LineType_Location");
		lineObj.gameObject.SetActive(true);
		CharacterDomainMethod.AsyncCall.GetCharacterLocationDisplayData(null, characterId, delegate(int offset, RawDataPool pool)
		{
			CharacterLocationDisplayData locationDisplayData = new CharacterLocationDisplayData();
			Serializer.Deserialize(pool, offset, ref locationDisplayData);
			FullBlockName fullBlockName = locationDisplayData.FullBlockName;
			MapStateItem stateConfig = MapState.Instance[fullBlockName.stateTemplateId];
			MapAreaItem areaConfig = MapArea.Instance[fullBlockName.areaTemplateId];
			WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			MapBlockData blockData = fullBlockName.BlockData;
			bool flag = blockData == null;
			string locationName;
			if (flag)
			{
				locationName = LocalStringManager.Get(LanguageKey.LK_Character_Location_Format_Invalid_2);
			}
			else
			{
				int nameIndex = worldMapModel.GetBlockNameIndex(blockData, worldMapModel.GetAreaSize(blockData.AreaId));
				string blockName = worldMapModel.GetBlockName(blockData.AreaId, blockData.BlockId, blockData.TemplateId, nameIndex);
				locationName = string.Concat(new string[]
				{
					stateConfig.Name,
					"-",
					areaConfig.Name,
					"-",
					blockName
				});
			}
			lineObj.CGet<TextMeshProUGUI>("Text").SetText(LocalStringManager.GetFormat(LanguageKey.LK_LocationItem_MouseTipNormal, locationName).ColorReplace(), true);
			lineObj.CGet<TextMeshProUGUI>("Text").GetComponent<TMPTextSpriteHelper>().Parse();
		});
	}

	// Token: 0x06002BC3 RID: 11203 RVA: 0x00156264 File Offset: 0x00154464
	private void SetBlockNameAdventure(int characterId)
	{
		Refers lineObj = base.CGet<Refers>("LineType_Location");
		lineObj.gameObject.SetActive(true);
		CharacterDomainMethod.AsyncCall.GetCharacterLocationDisplayData(null, characterId, delegate(int offset, RawDataPool pool)
		{
			CharacterLocationDisplayData locationDisplayData = new CharacterLocationDisplayData();
			Serializer.Deserialize(pool, offset, ref locationDisplayData);
			FullBlockName fullBlockName = locationDisplayData.FullBlockName;
			MapStateItem stateConfig = MapState.Instance[fullBlockName.stateTemplateId];
			MapAreaItem areaConfig = MapArea.Instance[fullBlockName.areaTemplateId];
			WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			MapBlockData blockData = fullBlockName.BlockData;
			bool flag = blockData == null;
			MouseTipWorkingStatus.<>c__DisplayClass13_1 CS$<>8__locals2;
			if (flag)
			{
				CS$<>8__locals2.locationName = LocalStringManager.Get(LanguageKey.LK_Character_Location_Format_Invalid_2);
			}
			else
			{
				int nameIndex = worldMapModel.GetBlockNameIndex(blockData, worldMapModel.GetAreaSize(blockData.AreaId));
				string blockName = worldMapModel.GetBlockName(blockData.AreaId, blockData.BlockId, blockData.TemplateId, nameIndex);
				CS$<>8__locals2.locationName = stateConfig.Name + "-" + areaConfig.Name;
			}
			base.<SetBlockNameAdventure>g__SetAdventureName|1(locationDisplayData.AdventureCoreId, ref CS$<>8__locals2);
		});
	}

	// Token: 0x06002BC4 RID: 11204 RVA: 0x001562B0 File Offset: 0x001544B0
	private void SetBlockNameSwordTomb(int characterId)
	{
		Refers lineObj = base.CGet<Refers>("LineType_Location");
		lineObj.gameObject.SetActive(true);
		CharacterDomainMethod.AsyncCall.GetCharacterLocationDisplayData(null, characterId, delegate(int offset, RawDataPool pool)
		{
			CharacterLocationDisplayData locationDisplayData = new CharacterLocationDisplayData();
			Serializer.Deserialize(pool, offset, ref locationDisplayData);
			FullBlockName fullBlockName = locationDisplayData.FullBlockName;
			MapStateItem stateConfig = MapState.Instance[fullBlockName.stateTemplateId];
			MapAreaItem areaConfig = MapArea.Instance[fullBlockName.areaTemplateId];
			WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
			MapBlockData blockData = fullBlockName.BlockData;
			bool flag = blockData == null;
			string locationName;
			if (flag)
			{
				locationName = LocalStringManager.Get(LanguageKey.LK_Character_Location_Format_Invalid_2);
			}
			else
			{
				int nameIndex = worldMapModel.GetBlockNameIndex(blockData, worldMapModel.GetAreaSize(blockData.AreaId));
				string blockName = worldMapModel.GetBlockName(blockData.AreaId, blockData.BlockId, blockData.TemplateId, nameIndex);
				locationName = stateConfig.Name + "-" + areaConfig.Name;
			}
			string adventureName = "";
			GuardingSwordTombDisplayData displayDataGuardTomb = this._displayData.ArrangementDisplayData.ArrangementData as GuardingSwordTombDisplayData;
			bool flag2 = displayDataGuardTomb != null;
			if (flag2)
			{
				adventureName = LocalStringManager.Get(string.Format("LK_SwordTomb_{0}", displayDataGuardTomb.SwordTombId));
			}
			lineObj.CGet<TextMeshProUGUI>("Text").SetText(LocalStringManager.GetFormat(LanguageKey.LK_LocationItem_MouseTipSwordTomb, locationName, adventureName).ColorReplace(), true);
			lineObj.CGet<TextMeshProUGUI>("Text").GetComponent<TMPTextSpriteHelper>().Parse();
		});
	}

	// Token: 0x06002BC5 RID: 11205 RVA: 0x00156302 File Offset: 0x00154502
	private void SetStatusName(string statusName)
	{
		this._titleText.SetText(statusName, true);
	}

	// Token: 0x04001FDB RID: 8155
	private int _charId;

	// Token: 0x04001FDC RID: 8156
	private short _arrangementTemplateId;

	// Token: 0x04001FDD RID: 8157
	private bool _disableRaycastTarget;

	// Token: 0x04001FDE RID: 8158
	private VillagerRoleCharacterDisplayData _displayData;

	// Token: 0x04001FDF RID: 8159
	private TextMeshProUGUI _titleText;
}
