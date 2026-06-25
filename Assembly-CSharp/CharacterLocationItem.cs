using System;
using Config;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x0200031E RID: 798
public class CharacterLocationItem : Refers
{
	// Token: 0x06002EB6 RID: 11958 RVA: 0x00170981 File Offset: 0x0016EB81
	public void Init(UIBase parent)
	{
		this._parent = parent;
		this.InitRefers();
	}

	// Token: 0x06002EB7 RID: 11959 RVA: 0x00170992 File Offset: 0x0016EB92
	public void Refresh(int characterId)
	{
		CharacterDomainMethod.AsyncCall.GetCharacterLocationDisplayData(this._parent, characterId, new AsyncMethodCallbackDelegate(this.OnCharacterLocationDisplayData));
	}

	// Token: 0x06002EB8 RID: 11960 RVA: 0x001709B0 File Offset: 0x0016EBB0
	private void OnCharacterLocationDisplayData(int offset, RawDataPool pool)
	{
		CharacterLocationDisplayData locationDisplayData = null;
		Serializer.Deserialize(pool, offset, ref locationDisplayData);
		bool flag = locationDisplayData.BlockData != null || locationDisplayData.RootBlockData != null;
		if (flag)
		{
			this._unknown.SetActive(false);
			this._mapBlockView.gameObject.SetActive(true);
			this._mapBlockView.Refresh(locationDisplayData.BlockData, locationDisplayData.RootBlockData);
		}
		else
		{
			this._unknown.SetActive(true);
			this._mapBlockView.gameObject.SetActive(false);
		}
		FullBlockName fullBlockName = locationDisplayData.FullBlockName;
		MapStateItem stateConfig = MapState.Instance[fullBlockName.stateTemplateId];
		MapAreaItem areaConfig = MapArea.Instance[fullBlockName.areaTemplateId];
		bool flag2 = stateConfig == null && areaConfig == null;
		string locationName;
		if (flag2)
		{
			locationName = LocalStringManager.Get(LanguageKey.LK_Character_Location_Format_Invalid_2);
		}
		else
		{
			bool isCapturedInStoneRoom = locationDisplayData.IsCapturedInStoneRoom;
			if (isCapturedInStoneRoom)
			{
				locationName = LocalStringManager.GetFormat(LanguageKey.LK_Character_Location_Format_StoneHouse_2, Organization.Instance[16].Name);
			}
			else
			{
				MapBlockData blockData = fullBlockName.BlockData;
				WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
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
		}
		int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		string kidnapper = "";
		bool flag3 = locationDisplayData.Kidnapper != null;
		if (flag3)
		{
			kidnapper = NameCenter.GetCharMonasticTitleOrNameByDisplayData(locationDisplayData.Kidnapper, locationDisplayData.Kidnapper.CharacterId == taiwuId, false);
		}
		switch (locationDisplayData.DisplayType)
		{
		case 0:
			this._characterInfoLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_LocationItem_Normal, locationName).ColorReplace();
			break;
		case 1:
			this._characterInfoLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_LocationItem_Kidnapped, locationName, kidnapper).ColorReplace();
			break;
		case 3:
			this._characterInfoLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_LocationItem_Dead, locationName).ColorReplace();
			break;
		}
		this._characterInfoLabel.GetComponent<TMPTextSpriteHelper>().Parse();
	}

	// Token: 0x06002EB9 RID: 11961 RVA: 0x00170C0E File Offset: 0x0016EE0E
	private void InitRefers()
	{
		this._mapBlockView = base.CGet<MapBlockView>("MapBlockView");
		this._characterInfoLabel = base.CGet<TextMeshProUGUI>("CharacterInfoLabel");
		this._unknown = base.CGet<GameObject>("Unknown");
	}

	// Token: 0x040021E6 RID: 8678
	private UIBase _parent;

	// Token: 0x040021E7 RID: 8679
	private MapBlockView _mapBlockView;

	// Token: 0x040021E8 RID: 8680
	private TextMeshProUGUI _characterInfoLabel;

	// Token: 0x040021E9 RID: 8681
	private GameObject _unknown;
}
