using System;
using System.Collections.Generic;
using Config;
using DisplayConfig;
using GameData.Adventure;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.LifeRecord;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.Domains.Map;
using GameData.Domains.Organization.Display;
using GameData.Domains.World;
using GameData.Utilities;
using UnityEngine;

// Token: 0x0200012D RID: 301
public static class GameMessageUtils
{
	// Token: 0x06000CC6 RID: 3270 RVA: 0x000549D8 File Offset: 0x00052BD8
	public static void RenderFixedArguments(ArgumentCollection argCollection, RenderedArgumentCollection renderedArgCollection, bool insertSprite = false)
	{
		GameMessageUtils.RenderItemNames(argCollection, renderedArgCollection, insertSprite);
		GameMessageUtils.RenderResourceNames(argCollection, renderedArgCollection, insertSprite);
		GameMessageUtils.RenderCombatSkillNames(argCollection, renderedArgCollection, insertSprite);
		GameMessageUtils.RenderOrgGradeNames(argCollection, renderedArgCollection);
		GameMessageUtils.RenderBuildingNames(argCollection, renderedArgCollection);
		GameMessageUtils.RenderSwordTombNames(argCollection, renderedArgCollection);
		GameMessageUtils.RenderJuniorXiangshuNames(argCollection, renderedArgCollection);
		GameMessageUtils.RenderIntegers(argCollection, renderedArgCollection);
		GameMessageUtils.RenderAdventureNames(argCollection, renderedArgCollection, insertSprite);
		GameMessageUtils.RenderBehaviorTypeNames(argCollection, renderedArgCollection);
		GameMessageUtils.RenderFavorabilityTypeNames(argCollection, renderedArgCollection);
		GameMessageUtils.RenderCricketNames(argCollection, renderedArgCollection);
		GameMessageUtils.RenderItemSubTypeNames(argCollection, renderedArgCollection);
		GameMessageUtils.RenderChickenNames(argCollection, renderedArgCollection);
		GameMessageUtils.RenderCharacterPropertyDisplayNames(argCollection, renderedArgCollection);
		GameMessageUtils.RenderBodyPartTypes(argCollection, renderedArgCollection);
		GameMessageUtils.RenderInjuryTypes(argCollection, renderedArgCollection);
		GameMessageUtils.RenderPoisonTypes(argCollection, renderedArgCollection);
		GameMessageUtils.RenderCharacterTemplates(argCollection, renderedArgCollection);
		GameMessageUtils.RenderFeatures(argCollection, renderedArgCollection);
		GameMessageUtils.RenderLifeSkills(argCollection, renderedArgCollection, insertSprite);
		GameMessageUtils.RenderMerchantTypes(argCollection, renderedArgCollection);
		GameMessageUtils.RenderItemKeys(argCollection, renderedArgCollection, insertSprite);
		GameMessageUtils.RenderCombatTypes(argCollection, renderedArgCollection);
		GameMessageUtils.RenderLifeSkillTypes(argCollection, renderedArgCollection);
		GameMessageUtils.RenderCombatSkillTypes(argCollection, renderedArgCollection);
		GameMessageUtils.RenderInformations(argCollection, renderedArgCollection);
		GameMessageUtils.RenderSecretInformationTemplates(argCollection, renderedArgCollection);
		GameMessageUtils.RenderPunishmentTypes(argCollection, renderedArgCollection);
		GameMessageUtils.RenderCharacterTitles(argCollection, renderedArgCollection);
		GameMessageUtils.RenderFloats(argCollection, renderedArgCollection);
		GameMessageUtils.RenderMonths(argCollection, renderedArgCollection);
		GameMessageUtils.RenderProfessionNames(argCollection, renderedArgCollection);
		GameMessageUtils.RenderProfessionSkillNames(argCollection, renderedArgCollection);
		GameMessageUtils.RenderItemGradeNames(argCollection, renderedArgCollection);
		GameMessageUtils.RenderMusicNames(argCollection, renderedArgCollection);
		GameMessageUtils.RenderMapStateNames(argCollection, renderedArgCollection);
		GameMessageUtils.RenderTexts(argCollection, renderedArgCollection);
		GameMessageUtils.RenderJiaoProperties(argCollection, renderedArgCollection);
		GameMessageUtils.RenderDestinyTypes(argCollection, renderedArgCollection);
		GameMessageUtils.RenderSecretInformations(argCollection, renderedArgCollection);
		GameMessageUtils.RenderMerchants(argCollection, renderedArgCollection);
		GameMessageUtils.RenderLegacys(argCollection, renderedArgCollection);
		GameMessageUtils.RenderCharGrades(argCollection, renderedArgCollection);
		GameMessageUtils.RenderFeasts(argCollection, renderedArgCollection);
		GameMessageUtils.RenderAdventureElementNames(argCollection, renderedArgCollection);
		GameMessageUtils.RenderPersonalityTypeNames(argCollection, renderedArgCollection);
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x00054B64 File Offset: 0x00052D64
	public static void RenderDynamicArguments(ArgumentCollectionRenderArguments arguments, ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection, bool insertSprite = false, bool addLink = false)
	{
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		List<NameAndLifeRelatedData> charNameAndLifeDataList = arguments.CharNameAndLifeDataList;
		bool flag = charNameAndLifeDataList != null && charNameAndLifeDataList.Count > 0;
		if (flag)
		{
			GameMessageUtils.RenderCharacterNames(taiwuCharId, arguments.CharNameAndLifeDataList, argumentCollection, renderedArgCollection, addLink);
		}
		List<LocationNameRelatedData> locationNames = arguments.LocationNames;
		bool flag2 = locationNames != null && locationNames.Count > 0;
		if (flag2)
		{
			GameMessageUtils.RenderLocationNames(arguments.LocationNames, renderedArgCollection);
		}
		List<SettlementNameRelatedData> settlementNames = arguments.SettlementNames;
		bool flag3 = settlementNames != null && settlementNames.Count > 0;
		if (flag3)
		{
			GameMessageUtils.RenderSettlementNames(arguments.SettlementNames, renderedArgCollection);
		}
		List<JiaoLoongNameRelatedData> jiaoLoongNames = arguments.JiaoLoongNames;
		bool flag4 = jiaoLoongNames != null && jiaoLoongNames.Count > 0;
		if (flag4)
		{
			GameMessageUtils.RenderJiaoLoongNames(arguments.JiaoLoongNames, renderedArgCollection);
		}
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x00054C1C File Offset: 0x00052E1C
	public static void RenderCharacterNames(int taiwuCharId, List<NameAndLifeRelatedData> nameAndLifeRelatedDataList, ArgumentCollection argCollection, RenderedArgumentCollection renderedArgCollection, bool addLink = false)
	{
		bool flag = nameAndLifeRelatedDataList.Count != argCollection.Characters.Count + argCollection.CharacterRealNames.Count;
		if (flag)
		{
			GLog.TagLog("GameMessageUtils", "RenderCharacterNames get unexpected data!", Array.Empty<object>());
		}
		for (int i = 0; i < argCollection.Characters.Count; i++)
		{
			NameRelatedData nameRelatedData = nameAndLifeRelatedDataList[i].NameRelatedData;
			bool isAlive = nameAndLifeRelatedDataList[i].LifeState == 0;
			bool isSpecial = nameRelatedData.CharTemplateId == 779;
			bool flag2 = nameRelatedData.CharTemplateId == -1;
			string nameStr;
			if (flag2)
			{
				bool flag3 = addLink && !isSpecial;
				if (flag3)
				{
					nameStr = "<link=\"ForgottenCharacter\">???</link>".SetColor("lightgrey");
				}
				else
				{
					nameStr = "???".SetColor("lightgrey");
				}
			}
			else
			{
				int charId = argCollection.Characters[i];
				bool isTaiwu = charId == taiwuCharId;
				string charName = NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, isTaiwu, false);
				bool flag4 = addLink && !isSpecial;
				if (flag4)
				{
					nameStr = string.Format("  <link=\"character_{0}\">{1}</link>  ", charId, charName).SetColor(isAlive ? "darkbrown" : "red");
				}
				else
				{
					nameStr = charName.SetColor(isAlive ? "darkbrown" : "red");
				}
			}
			renderedArgCollection.Characters.Add(nameStr);
		}
		int realNameBeginIndex = argCollection.Characters.Count;
		for (int j = realNameBeginIndex; j < nameAndLifeRelatedDataList.Count; j++)
		{
			NameRelatedData nameRelatedData2 = nameAndLifeRelatedDataList[j].NameRelatedData;
			bool isAlive2 = nameAndLifeRelatedDataList[j].LifeState == 0;
			bool isSpecial2 = nameRelatedData2.CharTemplateId == 779;
			bool flag5 = nameRelatedData2.CharTemplateId == -1;
			string nameStr2;
			if (flag5)
			{
				bool flag6 = addLink && !isSpecial2;
				if (flag6)
				{
					nameStr2 = "<link=\"ForgottenCharacter\">???</link>".SetColor("lightgrey");
				}
				else
				{
					nameStr2 = "???".SetColor("lightgrey");
				}
			}
			else
			{
				int charId2 = argCollection.CharacterRealNames[j - realNameBeginIndex];
				bool isTaiwu2 = charId2 == taiwuCharId;
				string charName2 = NameCenter.GetRealName(ref nameRelatedData2);
				bool flag7 = addLink && !isSpecial2;
				if (flag7)
				{
					nameStr2 = string.Format("  <link=\"character_{0}\">{1}</link>  ", charId2, charName2).SetColor(isAlive2 ? "darkbrown" : "red");
				}
				else
				{
					nameStr2 = charName2.SetColor(isAlive2 ? "darkbrown" : "red");
				}
			}
			renderedArgCollection.CharacterRealNames.Add(nameStr2);
		}
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x00054ED0 File Offset: 0x000530D0
	[Obsolete]
	public static void RenderCharacterRealNames(int taiwuCharId, List<NameAndLifeRelatedData> nameAndLifeRelatedDataList, ArgumentCollection argCollection, RenderedArgumentCollection renderedArgCollection, bool addLink = false)
	{
		for (int i = 0; i < nameAndLifeRelatedDataList.Count; i++)
		{
			NameRelatedData nameRelatedData = nameAndLifeRelatedDataList[i].NameRelatedData;
			bool isAlive = nameAndLifeRelatedDataList[i].LifeState == 0;
			bool flag = nameRelatedData.CharTemplateId == -1;
			string nameStr;
			if (flag)
			{
				if (addLink)
				{
					nameStr = "<link=\"ForgottenCharacter\">???</link>".SetColor("lightgrey");
				}
				else
				{
					nameStr = "???".SetColor("lightgrey");
				}
			}
			else
			{
				int charId = argCollection.Characters[i];
				string charName = NameCenter.GetRealName(ref nameRelatedData);
				if (addLink)
				{
					nameStr = string.Format("  <link=\"character_{0}\">{1}</link>  ", charId, charName).SetColor(isAlive ? "darkbrown" : "red");
				}
				else
				{
					nameStr = charName.SetColor(isAlive ? "darkbrown" : "red");
				}
			}
			renderedArgCollection.Characters.Add(nameStr);
		}
	}

	// Token: 0x06000CCA RID: 3274 RVA: 0x00054FCC File Offset: 0x000531CC
	public static void RenderLocationNames(List<LocationNameRelatedData> locationNames, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < locationNames.Count; i++)
		{
			renderedArgCollection.Locations.Add(CommonUtils.GetRelativeLocationName(locationNames[i]).SetColor("pinkyellow"));
		}
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x00055014 File Offset: 0x00053214
	public static void RenderLocationStateAreaNames(List<LocationNameRelatedData> locationNames, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < locationNames.Count; i++)
		{
			renderedArgCollection.Locations.Add(CommonUtils.GetLocationStateAreaName(locationNames[i]).SetColor("pinkyellow"));
		}
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x0005505C File Offset: 0x0005325C
	public static void RenderSettlementNames(List<SettlementNameRelatedData> settlementNames, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < settlementNames.Count; i++)
		{
			SettlementNameRelatedData settlementName = settlementNames[i];
			renderedArgCollection.Settlements.Add(CommonUtils.GetSettlementString(settlementName.RandomNameId, settlementName.MapBlockTemplateId).SetColor("pinkyellow"));
		}
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x000550B0 File Offset: 0x000532B0
	public static void RenderItemNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection, bool insertIcon)
	{
		for (int i = 0; i < argumentCollection.Items.Count; i++)
		{
			sbyte itemType = argumentCollection.Items[i].Item1;
			short templateId = argumentCollection.Items[i].Item2;
			short itemSubType = ItemTemplateHelper.GetItemSubType(itemType, templateId);
			bool flag = itemType == 10 || itemSubType == 1202;
			string itemName;
			if (flag)
			{
				itemName = ItemTemplateHelper.GetName(itemType, templateId).Replace('\n', ' ').SetColor(Colors.Instance.GradeColors[(int)ItemTemplateHelper.GetGrade(itemType, templateId)]);
			}
			else
			{
				itemName = LocalStringManager.GetFormat(LanguageKey.LK_Quotation_Marks_Fix, ItemTemplateHelper.GetName(itemType, templateId).Replace('\n', ' ')).SetColor(Colors.Instance.GradeColors[(int)ItemTemplateHelper.GetGrade(itemType, templateId)]);
			}
			string icon = "charactermenu3_26_icon_daoju";
			bool flag2 = itemSubType == 802;
			if (flag2)
			{
				icon = "charactermenu3_26_icon_guchong";
			}
			else
			{
				bool flag3 = itemSubType == 801;
				if (flag3)
				{
					icon = "charactermenu3_26_icon_duyao";
				}
				else
				{
					bool flag4 = ItemType.IsEquipmentItemType(itemType);
					if (flag4)
					{
						icon = "charactermenu3_26_icon_zhuangbei";
					}
				}
			}
			renderedArgCollection.Items.Add(insertIcon ? ("<SpName=" + icon + ">" + itemName) : itemName);
		}
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x000551F8 File Offset: 0x000533F8
	public static void RenderItemKeys(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection, bool insertIcon)
	{
		for (int i = 0; i < argumentCollection.ItemKeys.Count; i++)
		{
			ItemKey itemKey = (ItemKey)argumentCollection.ItemKeys[i];
			sbyte itemType = itemKey.ItemType;
			short templateId = itemKey.TemplateId;
			short itemSubType = ItemTemplateHelper.GetItemSubType(itemType, templateId);
			bool flag = itemType == 10 || itemSubType == 1202;
			string itemName;
			if (flag)
			{
				itemName = ItemTemplateHelper.GetName(itemType, templateId).Replace('\n', ' ').SetColor(Colors.Instance.GradeColors[(int)ItemTemplateHelper.GetGrade(itemType, templateId)]);
			}
			else
			{
				itemName = LocalStringManager.GetFormat(LanguageKey.LK_Quotation_Marks_Fix, ItemTemplateHelper.GetName(itemType, templateId).Replace('\n', ' ')).SetColor(Colors.Instance.GradeColors[(int)ItemTemplateHelper.GetGrade(itemType, templateId)]);
			}
			string icon = "charactermenu3_26_icon_daoju";
			bool flag2 = itemSubType == 802;
			if (flag2)
			{
				icon = "charactermenu3_26_icon_guchong";
			}
			else
			{
				bool flag3 = itemSubType == 801 || itemSubType == 506;
				if (flag3)
				{
					icon = "charactermenu3_26_icon_duyao";
				}
				else
				{
					bool flag4 = ItemType.IsEquipmentItemType(itemType);
					if (flag4)
					{
						icon = "charactermenu3_26_icon_zhuangbei";
					}
				}
			}
			renderedArgCollection.ItemKeys.Add(insertIcon ? ("<SpName=" + icon + ">" + itemName) : itemName);
		}
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x0005534C File Offset: 0x0005354C
	public static void RenderResourceNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection, bool insertIcon)
	{
		for (int i = 0; i < argumentCollection.Resources.Count; i++)
		{
			sbyte resourceType = argumentCollection.Resources[i];
			ResourceTypeItem config = Config.ResourceType.Instance[resourceType];
			string resourceName = LocalStringManager.GetFormat(LanguageKey.LK_Quotation_Marks_Fix, config.Name).SetColor("orange");
			string resourceIcon = string.Format("mousetip_ziyuan_{0}", resourceType);
			renderedArgCollection.Resources.Add(insertIcon ? ("<SpName=" + resourceIcon + ">" + resourceName) : resourceName);
		}
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x000553E8 File Offset: 0x000535E8
	public static void RenderCombatSkillNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection, bool insertIcon = false)
	{
		for (int i = 0; i < argumentCollection.CombatSkills.Count; i++)
		{
			CombatSkillItem combatSkill = CombatSkill.Instance[argumentCollection.CombatSkills[i]];
			string combatSkillName = LocalStringManager.GetFormat(LanguageKey.LK_Quotation_Marks_Fix, combatSkill.Name).SetColor(Colors.Instance.GradeColors[(int)combatSkill.Grade]);
			renderedArgCollection.CombatSkills.Add(insertIcon ? ("<SpName=" + CombatSkillType.Instance[combatSkill.Type].DisplayIcon + ">" + combatSkillName) : combatSkillName);
		}
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x00055490 File Offset: 0x00053690
	public static void RenderOrgGradeNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.OrgGrades.Count; i++)
		{
			ValueTuple<sbyte, sbyte, bool, sbyte> orgGrade = argumentCollection.OrgGrades[i];
			OrganizationInfo orgInfo = new OrganizationInfo(orgGrade.Item1, orgGrade.Item2, orgGrade.Item3, -1);
			renderedArgCollection.OrgGrades.Add(CommonUtils.GetCharacterGradeString(orgInfo, orgGrade.Item4, -1));
		}
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x000554FC File Offset: 0x000536FC
	public static void RenderBuildingNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.Buildings.Count; i++)
		{
			short building = argumentCollection.Buildings[i];
			renderedArgCollection.Buildings.Add(BuildingBlock.Instance[building].Name.SetColor("yellow"));
		}
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x0005555C File Offset: 0x0005375C
	public static void RenderSwordTombNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.SwordTombs.Count; i++)
		{
			sbyte swordTomb = argumentCollection.SwordTombs[i];
			renderedArgCollection.SwordTombs.Add(LocalStringManager.Get("LK_SwordTomb_" + swordTomb.ToString()).SetColor("darkbrown"));
		}
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x000555C0 File Offset: 0x000537C0
	public static void RenderJuniorXiangshuNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.JuniorXiangshuList.Count; i++)
		{
			sbyte xiangshuAvatarId = argumentCollection.JuniorXiangshuList[i];
			CharacterItem configData = Character.Instance[201 + (int)xiangshuAvatarId];
			renderedArgCollection.JuniorXiangshuList.Add((configData.Surname + configData.GivenName).SetColor("darkbrown"));
		}
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x00055630 File Offset: 0x00053830
	public static void RenderIntegers(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.Integers.Count; i++)
		{
			renderedArgCollection.Integers.Add(argumentCollection.Integers[i].ToString().SetColor("pinkyellow"));
		}
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x00055684 File Offset: 0x00053884
	public static void RenderAdventureNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection, bool insertIcon = false)
	{
		for (int i = 0; i < argumentCollection.Adventures.Count; i++)
		{
			int coreId = argumentCollection.Adventures[i];
			IAdventureData adventureCore = AdventureRemakeModel.Core.GetAdventureAny(coreId);
			string adventureName = (((adventureCore != null) ? adventureCore.Name : null) ?? string.Format("Unknown({0})", coreId)).SetColor("normaladventure");
			renderedArgCollection.Adventures.Add(insertIcon ? ("<SpName=charactermenu3_26_icon_qiyu>" + adventureName) : adventureName);
		}
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x00055714 File Offset: 0x00053914
	public static void RenderAdventureElementNames(ArgumentCollection argCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argCollection.AdventureElements.Count; i++)
		{
			int coreId = argCollection.AdventureElements[i];
			AdventureElementData elementData;
			bool flag = AdventureRemakeModel.Core.TryGetAdventureElementData(coreId, out elementData);
			if (flag)
			{
				renderedArgCollection.AdventureElements.Add(elementData.Name);
			}
			else
			{
				renderedArgCollection.AdventureElements.Add(string.Empty);
			}
		}
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x00055784 File Offset: 0x00053984
	private static void RenderPersonalityTypeNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.PersonalityTypes.Count; i++)
		{
			renderedArgCollection.PersonalityTypes.Add(Personality.Instance[(int)argumentCollection.PersonalityTypes[i]].Name);
		}
	}

	// Token: 0x06000CD9 RID: 3289 RVA: 0x000557D8 File Offset: 0x000539D8
	public static void RenderBehaviorTypeNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.BehaviorTypes.Count; i++)
		{
			renderedArgCollection.BehaviorTypes.Add(CommonUtils.GetBehaviorString(argumentCollection.BehaviorTypes[i]));
		}
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x00055820 File Offset: 0x00053A20
	public static void RenderFavorabilityTypeNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.FavorabilityTypes.Count; i++)
		{
			renderedArgCollection.FavorabilityTypes.Add(CommonUtils.GetFavorStringByLevel(argumentCollection.FavorabilityTypes[i]));
		}
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x00055868 File Offset: 0x00053A68
	public static void RenderCricketNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.Crickets.Count; i++)
		{
			ValueTuple<short, short, int> valueTuple = argumentCollection.Crickets[i];
			short colorId = valueTuple.Item1;
			short partId = valueTuple.Item2;
			int nameId = valueTuple.Item3;
			string cricketName = CricketCombineHelper.CalcCricketName(colorId, partId, nameId);
			sbyte cricketGrade = new ValueTuple<short, short>(colorId, partId).CalcCricketGrade();
			renderedArgCollection.Crickets.Add(cricketName.SetGradeColor((int)cricketGrade));
		}
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x000558E4 File Offset: 0x00053AE4
	public static void RenderItemSubTypeNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.ItemSubTypes.Count; i++)
		{
			string subTypeName = LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", argumentCollection.ItemSubTypes[i]));
			renderedArgCollection.ItemSubTypes.Add(subTypeName);
		}
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x0005593C File Offset: 0x00053B3C
	public static void RenderChickenNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.Chickens.Count; i++)
		{
			ChickenItem chickenConfig = Chicken.Instance[argumentCollection.Chickens[i]];
			renderedArgCollection.Chickens.Add(chickenConfig.Name.SetColor(Colors.Instance.GradeColors[(int)chickenConfig.Grade]));
		}
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x000559AC File Offset: 0x00053BAC
	public static void RenderCharacterPropertyDisplayNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.CharacterPropertyReferencedTypes.Count; i++)
		{
			short propertyReferenceType = argumentCollection.CharacterPropertyReferencedTypes[i];
			short propertyDisplayType = CharacterPropertyReferenced.Instance[propertyReferenceType].DisplayType;
			CharacterPropertyDisplayItem propertyDisplayCfg = CharacterPropertyDisplay.Instance[propertyDisplayType];
			renderedArgCollection.CharacterPropertyReferencedTypes.Add(propertyDisplayCfg.Name);
		}
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x00055A14 File Offset: 0x00053C14
	public static void RenderBodyPartTypes(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.BodyPartTypes.Count; i++)
		{
			sbyte partId = argumentCollection.BodyPartTypes[i];
			bool flag = partId >= 0;
			if (flag)
			{
				BodyPartItem bodyPartCfg = BodyPart.Instance[partId];
				renderedArgCollection.BodyPartTypes.Add(bodyPartCfg.Name);
			}
			else
			{
				renderedArgCollection.BodyPartTypes.Add(LocalStringManager.Get(LanguageKey.LK_Unknown));
			}
		}
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x00055A94 File Offset: 0x00053C94
	public static void RenderInjuryTypes(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.InjuryTypes.Count; i++)
		{
			sbyte injuryType = argumentCollection.InjuryTypes[i];
			string injuryTypeName = (injuryType == 1) ? LocalStringManager.Get(LanguageKey.LK_Inner_Injury) : LocalStringManager.Get(LanguageKey.LK_Out_Injury);
			renderedArgCollection.InjuryTypes.Add(injuryTypeName);
		}
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x00055AF4 File Offset: 0x00053CF4
	public static void RenderPoisonTypes(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.PoisonTypes.Count; i++)
		{
			sbyte poisonType = argumentCollection.PoisonTypes[i];
			PoisonItem poisonCfg = Poison.Instance[poisonType];
			renderedArgCollection.PoisonTypes.Add(poisonCfg.Name.SetColor(poisonCfg.FontColor));
		}
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x00055B54 File Offset: 0x00053D54
	public static void RenderCharacterTemplates(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		bool smallVillageXiangshuProgress = GameData.Domains.World.SharedMethods.SmallVillageXiangshuProgress();
		for (int i = 0; i < argumentCollection.CharacterTemplates.Count; i++)
		{
			short templateId = argumentCollection.CharacterTemplates[i];
			CharacterItem charCfg = Character.Instance[templateId];
			bool flag = templateId == 366 && smallVillageXiangshuProgress;
			if (flag)
			{
				renderedArgCollection.CharacterTemplates.Add((charCfg.AnonymousTitle ?? "").SetColor("darkbrown"));
			}
			else
			{
				renderedArgCollection.CharacterTemplates.Add((charCfg.Surname + charCfg.GivenName).SetColor("darkbrown"));
			}
		}
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x00055C00 File Offset: 0x00053E00
	public static void RenderFeatures(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.Features.Count; i++)
		{
			short templateId = argumentCollection.Features[i];
			CharacterFeatureItem featureCfg = CharacterFeature.Instance[templateId];
			renderedArgCollection.Features.Add(featureCfg.Name);
		}
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x00055C58 File Offset: 0x00053E58
	public static void RenderLifeSkills(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection, bool insertIcon = false)
	{
		for (int i = 0; i < argumentCollection.LifeSkills.Count; i++)
		{
			short templateId = argumentCollection.LifeSkills[i];
			Config.LifeSkillItem lifeSkillCfg = LifeSkill.Instance[templateId];
			string lifeSkillName = LocalStringManager.GetFormat(LanguageKey.LK_Quotation_Marks_Fix, lifeSkillCfg.Name).SetColor(Colors.Instance.GradeColors[(int)lifeSkillCfg.Grade]);
			renderedArgCollection.LifeSkills.Add(insertIcon ? ("<SpName=" + Config.LifeSkillType.Instance[lifeSkillCfg.Type].DisplayIcon + ">" + lifeSkillName) : lifeSkillName);
		}
	}

	// Token: 0x06000CE5 RID: 3301 RVA: 0x00055D08 File Offset: 0x00053F08
	public static void RenderMerchantTypes(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.MerchantTypes.Count; i++)
		{
			sbyte merchantType = argumentCollection.MerchantTypes[i];
			MerchantTypeItem merchantTypeCfg = MerchantType.Instance[merchantType];
			renderedArgCollection.MerchantTypes.Add(merchantTypeCfg.Name);
		}
	}

	// Token: 0x06000CE6 RID: 3302 RVA: 0x00055D60 File Offset: 0x00053F60
	public static void RenderCombatTypes(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.CombatTypes.Count; i++)
		{
			sbyte combatType = argumentCollection.CombatTypes[i];
			string combatTypeStr = LocalStringManager.Get(string.Format("LK_Combat_Type_{0}", combatType));
			renderedArgCollection.CombatTypes.Add(combatTypeStr);
		}
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x00055DBC File Offset: 0x00053FBC
	public static void RenderLifeSkillTypes(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.LifeSkillTypes.Count; i++)
		{
			sbyte lifeSkillType = argumentCollection.LifeSkillTypes[i];
			LifeSkillTypeItem config = Config.LifeSkillType.Instance[lifeSkillType];
			renderedArgCollection.LifeSkillTypes.Add(config.Name);
		}
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x00055E14 File Offset: 0x00054014
	public static void RenderCombatSkillTypes(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.CombatSkillTypes.Count; i++)
		{
			sbyte combatSkillType = argumentCollection.CombatSkillTypes[i];
			CombatSkillTypeItem config = CombatSkillType.Instance[combatSkillType];
			renderedArgCollection.CombatSkillTypes.Add(config.Name);
		}
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x00055E6C File Offset: 0x0005406C
	public static void RenderInformations(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.Informations.Count; i++)
		{
			short templateId = argumentCollection.Informations[i];
			InformationInfoItem config = InformationInfo.Instance[templateId];
			renderedArgCollection.Informations.Add(config.Name);
		}
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x00055EC4 File Offset: 0x000540C4
	public static void RenderSecretInformationTemplates(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.SecretInformationTemplates.Count; i++)
		{
			short templateId = argumentCollection.SecretInformationTemplates[i];
			SecretInformationItem config = SecretInformation.Instance[templateId];
			renderedArgCollection.SecretInformationTemplates.Add(config.Name);
		}
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x00055F1C File Offset: 0x0005411C
	public static void RenderPunishmentTypes(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.PunishmentTypes.Count; i++)
		{
			short templateId = argumentCollection.PunishmentTypes[i];
			PunishmentTypeItem config = PunishmentType.Instance[templateId];
			renderedArgCollection.PunishmentTypes.Add(config.Name);
		}
	}

	// Token: 0x06000CEC RID: 3308 RVA: 0x00055F74 File Offset: 0x00054174
	public static void RenderCharacterTitles(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.CharacterTitles.Count; i++)
		{
			short templateId = argumentCollection.CharacterTitles[i];
			CharacterTitleItem config = CharacterTitle.Instance[templateId];
			renderedArgCollection.CharacterTitles.Add(config.Name);
		}
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x00055FCC File Offset: 0x000541CC
	public static void RenderFloats(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int i = 0; i < argumentCollection.FloatValues.Count; i++)
		{
			float floatVal = argumentCollection.FloatValues[i];
			renderedArgCollection.FloatValues.Add(floatVal.ToString("F1").SetColor("pinkyellow"));
		}
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x00056028 File Offset: 0x00054228
	public static void RenderMonths(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgumentCollection)
	{
		for (int i = 0; i < argumentCollection.Months.Count; i++)
		{
			sbyte month = argumentCollection.Months[i];
			renderedArgumentCollection.Months.Add(Month.Instance[month].Name);
		}
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x0005607C File Offset: 0x0005427C
	public static void RenderProfessionNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		foreach (int id in argumentCollection.Professions)
		{
			string name = Profession.Instance[id].Name.SetColor("pinkyellow");
			renderedArgCollection.Professions.Add(name);
		}
	}

	// Token: 0x06000CF0 RID: 3312 RVA: 0x000560F8 File Offset: 0x000542F8
	public static void RenderProfessionSkillNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		for (int index = 0; index < argumentCollection.ProfessionSkills.Count; index++)
		{
			int id = argumentCollection.ProfessionSkills[index];
			ProfessionSkillItem professionSkillItem = ProfessionSkill.Instance[id];
			string name = professionSkillItem.Name;
			switch (professionSkillItem.Level)
			{
			case 0:
				name = name.SetColor("pinkyellow");
				break;
			case 1:
				name = name.SetColor(Colors.Instance.GradeColors[3]);
				break;
			case 2:
				name = name.SetColor(Colors.Instance.GradeColors[5]);
				break;
			case 4:
				name = name.SetColor(Colors.Instance.GradeColors[7]);
				break;
			}
			renderedArgCollection.ProfessionSkills.Add(name);
		}
	}

	// Token: 0x06000CF1 RID: 3313 RVA: 0x000561DC File Offset: 0x000543DC
	public static void RenderItemGradeNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		foreach (sbyte grade in argumentCollection.ItemGrades)
		{
			string name = CommonUtils.GetShortGradeText((int)grade, true);
			renderedArgCollection.ItemGrades.Add(name);
		}
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x00056244 File Offset: 0x00054444
	public static void RenderMusicNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		foreach (short id in argumentCollection.Musics)
		{
			MusicItem music = Music.Instance[id];
			renderedArgCollection.Musics.Add(music.Name.SetColor("pinkyellow"));
		}
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x000562C0 File Offset: 0x000544C0
	public static void RenderMapStateNames(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		foreach (sbyte id in argumentCollection.MapStates)
		{
			MapStateItem state = MapState.Instance[id];
			renderedArgCollection.MapStates.Add(state.Name.SetColor("pinkyellow"));
		}
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x0005633C File Offset: 0x0005453C
	public static void RenderTexts(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		renderedArgCollection.Texts.AddRange(argumentCollection.Texts);
	}

	// Token: 0x06000CF5 RID: 3317 RVA: 0x00056354 File Offset: 0x00054554
	public static void RenderJiaoLoongNames(List<JiaoLoongNameRelatedData> nameRelatedDataList, RenderedArgumentCollection renderedArgCollection)
	{
		foreach (JiaoLoongNameRelatedData nameRelatedData in nameRelatedDataList)
		{
			renderedArgCollection.JiaoLoongs.Add(nameRelatedData.GetName());
		}
	}

	// Token: 0x06000CF6 RID: 3318 RVA: 0x000563B4 File Offset: 0x000545B4
	public static void RenderJiaoProperties(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		foreach (short propertyId in argumentCollection.JiaoProperties)
		{
			JiaoPropertyItem property = Config.JiaoProperty.Instance[propertyId];
			renderedArgCollection.JiaoProperties.Add(property.Name);
		}
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x00056424 File Offset: 0x00054624
	public static void RenderDestinyTypes(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		foreach (sbyte id in argumentCollection.DestinyTypes)
		{
			DestinyTypeItem config = DestinyType.Instance[id];
			renderedArgCollection.Destinys.Add(config.Name.SetColor(config.RecordColor));
		}
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x000564A0 File Offset: 0x000546A0
	public static void RenderSecretInformations(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		foreach (ValueTuple<short, int> valueTuple in argumentCollection.SecretInformations)
		{
			short templateId = valueTuple.Item1;
			int id = valueTuple.Item2;
			SecretInformationItem config = SecretInformation.Instance[templateId];
			renderedArgCollection.SecretInformations.Add(config.Name);
		}
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x0005651C File Offset: 0x0005471C
	public static void RenderMerchants(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		foreach (sbyte id in argumentCollection.Merchants)
		{
			MerchantItem config = Merchant.Instance[id];
			Color color = CommonUtils.GetMerchantLevelColor(Convert.ToSByte((int)(config.Level + 1)));
			renderedArgCollection.Merchants.Add(config.UiName.SetColor(color));
		}
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x000565A4 File Offset: 0x000547A4
	public static void RenderLegacys(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		foreach (short id in argumentCollection.Legacys)
		{
			LegacyItem config = Legacy.Instance[id];
			renderedArgCollection.Legacys.Add(config.Name.SetColor(Colors.Instance.GradeColors[(int)config.Grade]));
		}
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x00056630 File Offset: 0x00054830
	public static void RenderCharGrades(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		foreach (sbyte grade in argumentCollection.CharGrades)
		{
			renderedArgCollection.CharGrades.Add(LocalStringManager.Get(LanguageKey.LK_OrgGrade_0 + (int)grade).SetGradeColor((int)grade));
		}
	}

	// Token: 0x06000CFC RID: 3324 RVA: 0x000566A0 File Offset: 0x000548A0
	public static void RenderFeasts(ArgumentCollection argumentCollection, RenderedArgumentCollection renderedArgCollection)
	{
		foreach (short feastId in argumentCollection.Feasts)
		{
			renderedArgCollection.Feasts.Add(Feast.Instance[feastId].Name);
		}
	}

	// Token: 0x06000CFD RID: 3325 RVA: 0x0005670C File Offset: 0x0005490C
	public static string GetText(this RenderInfo renderInfo, RenderedArgumentCollection renderedArgCollection)
	{
		return GameMessageUtils.ParseRenderInfoToText(renderInfo.Text, renderInfo, renderedArgCollection);
	}

	// Token: 0x06000CFE RID: 3326 RVA: 0x0005671C File Offset: 0x0005491C
	public static string ParseRenderInfoToText(string messageFormat, RenderInfo renderInfo, RenderedArgumentCollection renderedArgCollection)
	{
		bool flag = renderInfo.Arguments.Count <= 0;
		string result;
		if (flag)
		{
			result = messageFormat;
		}
		else
		{
			object[] recordArgs = renderInfo.ParseArguments(renderedArgCollection);
			result = string.Format(messageFormat, recordArgs);
		}
		return result;
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x00056758 File Offset: 0x00054958
	public static object[] ParseArguments(this RenderInfo renderInfo, RenderedArgumentCollection renderedArgCollection)
	{
		object[] recordArgs = new object[renderInfo.Arguments.Count];
		for (int i = 0; i < recordArgs.Length; i++)
		{
			string paramStr;
			bool flag = !renderedArgCollection.TryGet(renderInfo.Arguments[i].Item1, renderInfo.Arguments[i].Item2, out paramStr);
			if (flag)
			{
				PredefinedLog.DefValue.GameMessageRenderError.Log(new object[]
				{
					renderInfo.Text,
					renderInfo.RecordType,
					i,
					renderInfo.Arguments[i].Item1
				});
			}
			recordArgs[i] = paramStr;
		}
		return recordArgs;
	}

	// Token: 0x06000D00 RID: 3328 RVA: 0x0005681C File Offset: 0x00054A1C
	public static string RenderCharacterName(int index, TransferableRecordDataBase data, bool addLink = false)
	{
		NameAndLifeRelatedData nameAndLifeRelatedData;
		bool flag = !data.CharNames.TryGetValue(index, out nameAndLifeRelatedData);
		string result;
		if (flag)
		{
			GLog.TagLog("GameMessageUtils", "RenderCharacterNames get unexpected data!", Array.Empty<object>());
			result = "";
		}
		else
		{
			NameRelatedData nameRelatedData = nameAndLifeRelatedData.NameRelatedData;
			bool isAlive = nameAndLifeRelatedData.LifeState == 0;
			bool isSpecial = nameRelatedData.CharTemplateId == 779;
			bool flag2 = nameRelatedData.CharTemplateId == -1;
			string nameStr;
			if (flag2)
			{
				bool flag3 = addLink && !isSpecial;
				if (flag3)
				{
					nameStr = "  <link=\"ForgottenCharacter\">???</link>  ".SetColor("lightgrey");
				}
				else
				{
					nameStr = "???".SetColor("lightgrey");
				}
			}
			else
			{
				bool isTaiwu = index == data.TaiwuCharId;
				string charName = NameCenter.GetMonasticTitleOrDisplayName(ref nameRelatedData, isTaiwu, false);
				bool flag4 = addLink && !isSpecial;
				if (flag4)
				{
					nameStr = string.Format("  <link=\"character_{0}\">{1}</link>  ", index, charName).SetColor(isAlive ? "darkbrown" : "red");
				}
				else
				{
					nameStr = charName.SetColor(isAlive ? "darkbrown" : "red");
				}
			}
			result = nameStr;
		}
		return result;
	}

	// Token: 0x06000D01 RID: 3329 RVA: 0x00056948 File Offset: 0x00054B48
	public static string RenderCharacterRealName(int index, TransferableRecordDataBase data, bool addLink = false)
	{
		NameAndLifeRelatedData nameAndLifeRelatedData;
		bool flag = !data.CharNames.TryGetValue(index, out nameAndLifeRelatedData);
		string result;
		if (flag)
		{
			GLog.TagLog("GameMessageUtils", "RenderCharacterRealName get unexpected data!", Array.Empty<object>());
			result = "";
		}
		else
		{
			NameRelatedData nameRelatedData = nameAndLifeRelatedData.NameRelatedData;
			bool isAlive = nameAndLifeRelatedData.LifeState == 0;
			bool flag2 = nameRelatedData.CharTemplateId == -1;
			string nameStr;
			if (flag2)
			{
				if (addLink)
				{
					nameStr = "  <link=\"ForgottenCharacter\">???</link>  ".SetColor("lightgrey");
				}
				else
				{
					nameStr = "???".SetColor("lightgrey");
				}
			}
			else
			{
				string charName = NameCenter.GetRealName(ref nameRelatedData);
				if (addLink)
				{
					nameStr = string.Format("  <link=\"character_{0}\">{1}</link>  ", index, charName).SetColor(isAlive ? "darkbrown" : "red");
				}
				else
				{
					nameStr = charName.SetColor(isAlive ? "darkbrown" : "red");
				}
			}
			result = nameStr;
		}
		return result;
	}

	// Token: 0x06000D02 RID: 3330 RVA: 0x00056A44 File Offset: 0x00054C44
	public static string RenderLocationName(int index, TransferableRecordDataBase data)
	{
		Location location = data.ArgumentCollection.Locations[index];
		LocationNameRelatedData locationName;
		bool flag = !data.LocationNames.TryGetValue(location, out locationName);
		string result;
		if (flag)
		{
			GLog.TagLog("GameMessageUtils", "RenderLocationNames get unexpected data!", Array.Empty<object>());
			result = "";
		}
		else
		{
			result = CommonUtils.GetRelativeLocationName(locationName).SetColor("pinkyellow");
		}
		return result;
	}

	// Token: 0x06000D03 RID: 3331 RVA: 0x00056AB0 File Offset: 0x00054CB0
	public static string RenderLocationStateAreaName(int index, TransferableRecordDataBase data)
	{
		Location location = data.ArgumentCollection.Locations[index];
		LocationNameRelatedData locationName;
		bool flag = !data.LocationNames.TryGetValue(location, out locationName);
		string result;
		if (flag)
		{
			GLog.TagLog("GameMessageUtils", "RenderLocationStateAreaNames get unexpected data!", Array.Empty<object>());
			result = "";
		}
		else
		{
			result = CommonUtils.GetLocationStateAreaName(locationName).SetColor("pinkyellow");
		}
		return result;
	}

	// Token: 0x06000D04 RID: 3332 RVA: 0x00056B1C File Offset: 0x00054D1C
	public static string RenderSettlementName(int index, TransferableRecordDataBase data)
	{
		short settlementId = (short)index;
		SettlementNameRelatedData settlementName;
		bool flag = !data.SettlementNames.TryGetValue(settlementId, out settlementName);
		string result;
		if (flag)
		{
			GLog.TagLog("GameMessageUtils", "RenderCharacterRealName get unexpected data!", Array.Empty<object>());
			result = "";
		}
		else
		{
			result = CommonUtils.GetSettlementString(settlementName.RandomNameId, settlementName.MapBlockTemplateId).SetColor("pinkyellow");
		}
		return result;
	}

	// Token: 0x06000D05 RID: 3333 RVA: 0x00056B80 File Offset: 0x00054D80
	private static string RenderItem(sbyte itemType, short templateId, bool insertIcon)
	{
		short itemSubType = ItemTemplateHelper.GetItemSubType(itemType, templateId);
		bool flag = itemType == 10 || itemSubType == 1202;
		string itemName;
		if (flag)
		{
			itemName = ItemTemplateHelper.GetName(itemType, templateId).Replace('\n', ' ').SetColor(Colors.Instance.GradeColors[(int)ItemTemplateHelper.GetGrade(itemType, templateId)]);
		}
		else
		{
			itemName = LocalStringManager.GetFormat(LanguageKey.LK_Quotation_Marks_Fix, ItemTemplateHelper.GetName(itemType, templateId).Replace('\n', ' ')).SetColor(Colors.Instance.GradeColors[(int)ItemTemplateHelper.GetGrade(itemType, templateId)]);
		}
		string icon = "charactermenu3_26_icon_daoju";
		bool flag2 = itemSubType == 802;
		if (flag2)
		{
			icon = "charactermenu3_26_icon_guchong";
		}
		else
		{
			bool flag3 = itemSubType == 801;
			if (flag3)
			{
				icon = "charactermenu3_26_icon_duyao";
			}
			else
			{
				bool flag4 = ItemType.IsEquipmentItemType(itemType);
				if (flag4)
				{
					icon = "charactermenu3_26_icon_zhuangbei";
				}
			}
		}
		return insertIcon ? ("<SpName=" + icon + ">" + itemName) : itemName;
	}

	// Token: 0x06000D06 RID: 3334 RVA: 0x00056C70 File Offset: 0x00054E70
	public static string RenderItemName(int index, TransferableRecordDataBase data, bool insertIcon)
	{
		ValueTuple<sbyte, short> item = data.ArgumentCollection.Items[index];
		return GameMessageUtils.RenderItem(item.Item1, item.Item2, insertIcon);
	}

	// Token: 0x06000D07 RID: 3335 RVA: 0x00056CA8 File Offset: 0x00054EA8
	public static string RenderItemKey(int index, TransferableRecordDataBase data, bool insertIcon)
	{
		ItemKey item = (ItemKey)data.ArgumentCollection.ItemKeys[index];
		return GameMessageUtils.RenderItem(item.ItemType, item.TemplateId, insertIcon);
	}

	// Token: 0x06000D08 RID: 3336 RVA: 0x00056CE8 File Offset: 0x00054EE8
	public static string RenderResourceName(int index, TransferableRecordDataBase data, bool insertIcon)
	{
		sbyte resourceType = (sbyte)index;
		ResourceTypeItem config = Config.ResourceType.Instance[resourceType];
		string resourceName = LocalStringManager.GetFormat(LanguageKey.LK_Quotation_Marks_Fix, config.Name).SetColor("orange");
		string resourceIcon = string.Format("mousetip_ziyuan_{0}", resourceType);
		return insertIcon ? ("<SpName=" + resourceIcon + ">" + resourceName) : resourceName;
	}

	// Token: 0x06000D09 RID: 3337 RVA: 0x00056D50 File Offset: 0x00054F50
	public static string RenderCombatSkillName(int index, TransferableRecordDataBase data, bool insertIcon = false)
	{
		CombatSkillItem combatSkill = CombatSkill.Instance[index];
		string combatSkillName = LocalStringManager.GetFormat(LanguageKey.LK_Quotation_Marks_Fix, combatSkill.Name).SetColor(Colors.Instance.GradeColors[(int)combatSkill.Grade]);
		return insertIcon ? ("<SpName=" + CombatSkillType.Instance[combatSkill.Type].DisplayIcon + ">" + combatSkillName) : combatSkillName;
	}

	// Token: 0x06000D0A RID: 3338 RVA: 0x00056DC8 File Offset: 0x00054FC8
	public static string RenderOrgGradeName(int index, TransferableRecordDataBase data)
	{
		ValueTuple<sbyte, sbyte, bool, sbyte> orgGrade = data.ArgumentCollection.OrgGrades[index];
		OrganizationInfo orgInfo = new OrganizationInfo(orgGrade.Item1, orgGrade.Item2, orgGrade.Item3, -1);
		return CommonUtils.GetCharacterGradeString(orgInfo, orgGrade.Item4, -1);
	}

	// Token: 0x06000D0B RID: 3339 RVA: 0x00056E18 File Offset: 0x00055018
	public static string RenderBuildingName(int index, TransferableRecordDataBase data)
	{
		return BuildingBlock.Instance[index].Name.SetColor("yellow");
	}

	// Token: 0x06000D0C RID: 3340 RVA: 0x00056E48 File Offset: 0x00055048
	public static string RenderSwordTombName(int index, TransferableRecordDataBase data)
	{
		int id = index;
		return LocalStringManager.Get("LK_SwordTomb_" + id.ToString()).SetColor("darkbrown");
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x00056E7C File Offset: 0x0005507C
	public static string RenderJuniorXiangshuName(int index, TransferableRecordDataBase data)
	{
		CharacterItem configData = Character.Instance[201 + index];
		return (configData.Surname + configData.GivenName).SetColor("darkbrown");
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x00056EC0 File Offset: 0x000550C0
	public static string RenderInteger(int index, TransferableRecordDataBase data)
	{
		int integer = index;
		return integer.ToString().SetColor("pinkyellow");
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x00056EE8 File Offset: 0x000550E8
	public static string RenderAdventureName(int index, TransferableRecordDataBase data, bool insertIcon = false)
	{
		AdventureItem advConfig = Adventure.Instance[index];
		AdventureTypeItem advTypeCfg = AdventureType.Instance[advConfig.Type];
		string adventureName = advConfig.Name.SetColor(((advTypeCfg != null) ? advTypeCfg.ColorName : null) ?? "normaladventure");
		return insertIcon ? ("<SpName=charactermenu3_26_icon_qiyu>" + adventureName) : adventureName;
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x00056F50 File Offset: 0x00055150
	public static string RenderBehaviorTypeName(int index, TransferableRecordDataBase data)
	{
		sbyte name = (sbyte)index;
		return CommonUtils.GetBehaviorString(name);
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x00056F6C File Offset: 0x0005516C
	public static string RenderFavorabilityTypeName(int index, TransferableRecordDataBase data)
	{
		sbyte level = (sbyte)index;
		return CommonUtils.GetFavorStringByLevel(level);
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x00056F88 File Offset: 0x00055188
	public static string RenderCricketName(int index, TransferableRecordDataBase data)
	{
		ValueTuple<short, short, int> valueTuple = data.ArgumentCollection.Crickets[index];
		short colorId = valueTuple.Item1;
		short partId = valueTuple.Item2;
		int nameId = valueTuple.Item3;
		string cricketName = CricketCombineHelper.CalcCricketName(colorId, partId, nameId);
		sbyte cricketGrade = new ValueTuple<short, short>(colorId, partId).CalcCricketGrade();
		return cricketName.SetColor(Colors.Instance.GradeColors[(int)cricketGrade]);
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x00056FF4 File Offset: 0x000551F4
	public static string RenderItemSubTypeName(int index, TransferableRecordDataBase data)
	{
		return LocalStringManager.Get(string.Format("LK_ItemSubType_{0}", index));
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x00057020 File Offset: 0x00055220
	public static string RenderChickenName(int index, TransferableRecordDataBase data)
	{
		ChickenItem chickenConfig = Chicken.Instance[index];
		return chickenConfig.Name.SetColor(Colors.Instance.GradeColors[(int)chickenConfig.Grade]);
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x00057060 File Offset: 0x00055260
	public static string RenderCharacterPropertyDisplayName(int index, TransferableRecordDataBase data)
	{
		short propertyDisplayType = CharacterPropertyReferenced.Instance[index].DisplayType;
		CharacterPropertyDisplayItem propertyDisplayCfg = CharacterPropertyDisplay.Instance[propertyDisplayType];
		return propertyDisplayCfg.Name;
	}

	// Token: 0x06000D16 RID: 3350 RVA: 0x00057098 File Offset: 0x00055298
	public static string RenderBodyPartType(int index, TransferableRecordDataBase data)
	{
		return (index >= 0) ? BodyPart.Instance[index].Name : LanguageKey.LK_Unknown.Tr();
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x000570CC File Offset: 0x000552CC
	public static string RenderInjuryType(int index, TransferableRecordDataBase data)
	{
		return (index == 1) ? LanguageKey.LK_Inner_Injury.Tr() : LanguageKey.LK_Out_Injury.Tr();
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x000570FC File Offset: 0x000552FC
	public static string RenderPoisonType(int index, TransferableRecordDataBase data)
	{
		PoisonItem poisonCfg = Poison.Instance[index];
		return poisonCfg.Name.SetColor(poisonCfg.FontColor);
	}

	// Token: 0x06000D19 RID: 3353 RVA: 0x00057130 File Offset: 0x00055330
	public static string RenderCharacterTemplate(int index, TransferableRecordDataBase data)
	{
		short templateId = (short)index;
		bool smallVillageXiangshuProgress = GameData.Domains.World.SharedMethods.SmallVillageXiangshuProgress();
		CharacterItem charCfg = Character.Instance[templateId];
		return (templateId == 366 && smallVillageXiangshuProgress) ? (charCfg.AnonymousTitle ?? "").SetColor("darkbrown") : (charCfg.Surname + charCfg.GivenName).SetColor("darkbrown");
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x0005719C File Offset: 0x0005539C
	public static string RenderFeature(int index, TransferableRecordDataBase data)
	{
		short templateId = (short)index;
		CharacterFeatureItem featureCfg = CharacterFeature.Instance[templateId];
		return featureCfg.Name;
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x000571C4 File Offset: 0x000553C4
	public static string RenderLifeSkill(int index, TransferableRecordDataBase data, bool insertIcon = false)
	{
		short templateId = (short)index;
		Config.LifeSkillItem lifeSkillCfg = LifeSkill.Instance[templateId];
		string lifeSkillName = LanguageKey.LK_Quotation_Marks_Fix.TrFormat(lifeSkillCfg.Name).SetColor(Colors.Instance.GradeColors[(int)lifeSkillCfg.Grade]);
		return insertIcon ? ("<SpName=" + Config.LifeSkillType.Instance[lifeSkillCfg.Type].DisplayIcon + ">" + lifeSkillName) : lifeSkillName;
	}

	// Token: 0x06000D1C RID: 3356 RVA: 0x0005723C File Offset: 0x0005543C
	public static string RenderMerchantType(int index, TransferableRecordDataBase data)
	{
		sbyte merchantType = (sbyte)index;
		MerchantTypeItem merchantTypeCfg = MerchantType.Instance[merchantType];
		return merchantTypeCfg.Name;
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x00057264 File Offset: 0x00055464
	public static string RenderCombatType(int index, TransferableRecordDataBase data)
	{
		sbyte combatType = (sbyte)index;
		return LocalStringManager.Get(string.Format("LK_Combat_Type_{0}", combatType));
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x00057290 File Offset: 0x00055490
	public static string RenderLifeSkillType(int index, TransferableRecordDataBase data)
	{
		sbyte lifeSkillType = (sbyte)index;
		LifeSkillTypeItem config = Config.LifeSkillType.Instance[lifeSkillType];
		return config.Name;
	}

	// Token: 0x06000D1F RID: 3359 RVA: 0x000572B8 File Offset: 0x000554B8
	public static string RenderCombatSkillType(int index, TransferableRecordDataBase data)
	{
		sbyte combatSkillType = (sbyte)index;
		CombatSkillTypeItem config = CombatSkillType.Instance[combatSkillType];
		return config.Name;
	}

	// Token: 0x06000D20 RID: 3360 RVA: 0x000572E0 File Offset: 0x000554E0
	public static string RenderInformation(int index, TransferableRecordDataBase data)
	{
		short templateId = (short)index;
		InformationInfoItem config = InformationInfo.Instance[templateId];
		return config.Name;
	}

	// Token: 0x06000D21 RID: 3361 RVA: 0x00057308 File Offset: 0x00055508
	public static string RenderSecretInformationTemplate(int index, TransferableRecordDataBase data)
	{
		short templateId = (short)index;
		SecretInformationItem config = SecretInformation.Instance[templateId];
		return ((config != null) ? config.Name : null) ?? "";
	}

	// Token: 0x06000D22 RID: 3362 RVA: 0x00057340 File Offset: 0x00055540
	public static string RenderPunishmentType(int index, TransferableRecordDataBase data)
	{
		short templateId = (short)index;
		PunishmentTypeItem config = PunishmentType.Instance[templateId];
		return config.Name;
	}

	// Token: 0x06000D23 RID: 3363 RVA: 0x00057368 File Offset: 0x00055568
	public static string RenderCharacterTitle(int index, TransferableRecordDataBase data)
	{
		short templateId = (short)index;
		CharacterTitleItem config = CharacterTitle.Instance[templateId];
		return config.Name;
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x00057390 File Offset: 0x00055590
	public static string RenderFloat(int index, TransferableRecordDataBase data)
	{
		return data.ArgumentCollection.Floats[index].ToString("F1").SetColor("pinkyellow");
	}

	// Token: 0x06000D25 RID: 3365 RVA: 0x000573CC File Offset: 0x000555CC
	public static string RenderMonth(int index, TransferableRecordDataBase data)
	{
		sbyte month = (sbyte)index;
		return Month.Instance[month].Name;
	}

	// Token: 0x06000D26 RID: 3366 RVA: 0x000573F4 File Offset: 0x000555F4
	public static string RenderProfessionName(int index, TransferableRecordDataBase data)
	{
		return Profession.Instance[index].Name.SetColor("pinkyellow");
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x00057424 File Offset: 0x00055624
	public static string RenderProfessionSkillName(int index, TransferableRecordDataBase data)
	{
		ProfessionSkillItem professionSkillItem = ProfessionSkill.Instance[index];
		string name = professionSkillItem.Name;
		switch (professionSkillItem.Level)
		{
		case 0:
			name = name.SetColor("pinkyellow");
			break;
		case 1:
			name = name.SetColor(Colors.Instance.GradeColors[3]);
			break;
		case 2:
			name = name.SetColor(Colors.Instance.GradeColors[5]);
			break;
		case 4:
			name = name.SetColor(Colors.Instance.GradeColors[7]);
			break;
		}
		return name;
	}

	// Token: 0x06000D28 RID: 3368 RVA: 0x000574D0 File Offset: 0x000556D0
	public static string RenderItemGradeName(int index, TransferableRecordDataBase data)
	{
		sbyte grade = (sbyte)index;
		return CommonUtils.GetShortGradeText((int)grade, true);
	}

	// Token: 0x06000D29 RID: 3369 RVA: 0x000574EC File Offset: 0x000556EC
	public static string RenderMusicName(int index, TransferableRecordDataBase data)
	{
		short id = (short)index;
		return Music.Instance[id].Name.SetColor("pinkyellow");
	}

	// Token: 0x06000D2A RID: 3370 RVA: 0x0005751C File Offset: 0x0005571C
	public static string RenderMapStateName(int index, TransferableRecordDataBase data)
	{
		sbyte id = (sbyte)index;
		MapStateItem state = MapState.Instance[id];
		return state.Name.SetColor("pinkyellow");
	}

	// Token: 0x06000D2B RID: 3371 RVA: 0x00057550 File Offset: 0x00055750
	public static string RenderText(int index, TransferableRecordDataBase data)
	{
		return data.ArgumentCollection.Texts[index];
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x00057574 File Offset: 0x00055774
	public static string RenderJiaoLoongName(int index, TransferableRecordDataBase data)
	{
		JiaoLoongNameRelatedData nameRelatedData;
		bool flag = data.JiaoLoongNames.TryGetValue(index, out nameRelatedData);
		string result;
		if (flag)
		{
			result = nameRelatedData.GetName();
		}
		else
		{
			AdaptableLog.Warning(string.Format("cannot got JiaoLoongName with id {0}", index), false);
			result = "";
		}
		return result;
	}

	// Token: 0x06000D2D RID: 3373 RVA: 0x000575C4 File Offset: 0x000557C4
	public static string RenderJiaoProperty(int index, TransferableRecordDataBase data)
	{
		short propertyId = (short)index;
		JiaoPropertyItem property = Config.JiaoProperty.Instance[propertyId];
		return property.Name;
	}

	// Token: 0x06000D2E RID: 3374 RVA: 0x000575EC File Offset: 0x000557EC
	public static string RenderDestinyType(int index, TransferableRecordDataBase data)
	{
		sbyte id = (sbyte)index;
		DestinyTypeItem config = DestinyType.Instance[id];
		return config.Name.SetColor(config.RecordColor);
	}

	// Token: 0x06000D2F RID: 3375 RVA: 0x00057620 File Offset: 0x00055820
	public static string RenderSecretInformation(int index, TransferableRecordDataBase data)
	{
		ValueTuple<short, int> valueTuple = data.ArgumentCollection.SecretInformations[index];
		short templateId = valueTuple.Item1;
		int id = valueTuple.Item2;
		SecretInformationItem config = SecretInformation.Instance[templateId];
		return config.Name;
	}

	// Token: 0x06000D30 RID: 3376 RVA: 0x00057664 File Offset: 0x00055864
	public static string RenderMerchant(int index, TransferableRecordDataBase data)
	{
		sbyte id = (sbyte)index;
		MerchantItem config = Merchant.Instance[id];
		Color color = CommonUtils.GetMerchantLevelColor(Convert.ToSByte((int)(config.Level + 1)));
		return config.UiName.SetColor(color);
	}

	// Token: 0x06000D31 RID: 3377 RVA: 0x000576A4 File Offset: 0x000558A4
	public static string RenderLegacy(int index, TransferableRecordDataBase data)
	{
		short id = (short)index;
		LegacyItem config = Legacy.Instance[id];
		return config.Name.SetColor(Colors.Instance.GradeColors[(int)config.Grade]);
	}

	// Token: 0x06000D32 RID: 3378 RVA: 0x000576E8 File Offset: 0x000558E8
	public static string RenderCharGrade(int index, TransferableRecordDataBase data)
	{
		sbyte grade = (sbyte)index;
		return LocalStringManager.Get(LanguageKey.LK_OrgGrade_0 + (int)grade).SetGradeColor((int)grade);
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x00057710 File Offset: 0x00055910
	public static string RenderFeast(int index, TransferableRecordDataBase data)
	{
		short feastId = (short)index;
		return Feast.Instance[feastId].Name;
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x00057738 File Offset: 0x00055938
	public static string RenderDefault(ValueTuple<sbyte, int> arg, TransferableRecordDataBase data)
	{
		AdaptableLog.Warning(string.Format("Render error with type {0} and data {1}", arg.Item1, arg.Item2), false);
		return "";
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x00057778 File Offset: 0x00055978
	public static string ReadArguments(sbyte type, int index, TransferableRecordDataBase data)
	{
		if (!true)
		{
		}
		string result;
		switch (type)
		{
		case 0:
			result = GameMessageUtils.RenderCharacterName(index, data, true);
			break;
		case 1:
			result = GameMessageUtils.RenderLocationName(index, data);
			break;
		case 2:
			result = GameMessageUtils.RenderItemName(index, data, true);
			break;
		case 3:
			result = GameMessageUtils.RenderCombatSkillName(index, data, true);
			break;
		case 4:
			result = GameMessageUtils.RenderResourceName(index, data, true);
			break;
		case 5:
			result = GameMessageUtils.RenderSettlementName(index, data);
			break;
		case 6:
			result = GameMessageUtils.RenderOrgGradeName(index, data);
			break;
		case 7:
			result = GameMessageUtils.RenderBuildingName(index, data);
			break;
		case 8:
			result = GameMessageUtils.RenderSwordTombName(index, data);
			break;
		case 9:
			result = GameMessageUtils.RenderJuniorXiangshuName(index, data);
			break;
		case 10:
			result = GameMessageUtils.RenderAdventureName(index, data, true);
			break;
		case 11:
			result = GameMessageUtils.RenderBehaviorTypeName(index, data);
			break;
		case 12:
			result = GameMessageUtils.RenderFavorabilityTypeName(index, data);
			break;
		case 13:
			result = GameMessageUtils.RenderCricketName(index, data);
			break;
		case 14:
			result = GameMessageUtils.RenderItemSubTypeName(index, data);
			break;
		case 15:
			result = GameMessageUtils.RenderChickenName(index, data);
			break;
		case 16:
			result = GameMessageUtils.RenderCharacterPropertyDisplayName(index, data);
			break;
		case 17:
			result = GameMessageUtils.RenderBodyPartType(index, data);
			break;
		case 18:
			result = GameMessageUtils.RenderInjuryType(index, data);
			break;
		case 19:
			result = GameMessageUtils.RenderPoisonType(index, data);
			break;
		case 20:
			result = GameMessageUtils.RenderCharacterTemplate(index, data);
			break;
		case 21:
			result = GameMessageUtils.RenderFeature(index, data);
			break;
		case 22:
			result = GameMessageUtils.RenderInteger(index, data);
			break;
		case 23:
			result = GameMessageUtils.RenderLifeSkill(index, data, true);
			break;
		case 24:
			result = GameMessageUtils.RenderMerchantType(index, data);
			break;
		case 25:
			result = GameMessageUtils.RenderItemKey(index, data, true);
			break;
		case 26:
			result = GameMessageUtils.RenderCombatType(index, data);
			break;
		case 27:
			result = GameMessageUtils.RenderLifeSkillType(index, data);
			break;
		case 28:
			result = GameMessageUtils.RenderCombatSkillType(index, data);
			break;
		case 29:
			result = GameMessageUtils.RenderInformation(index, data);
			break;
		case 30:
			result = GameMessageUtils.RenderSecretInformationTemplate(index, data);
			break;
		case 31:
			result = GameMessageUtils.RenderPunishmentType(index, data);
			break;
		case 32:
			result = GameMessageUtils.RenderCharacterTitle(index, data);
			break;
		case 33:
			result = GameMessageUtils.RenderFloat(index, data);
			break;
		case 34:
			result = GameMessageUtils.RenderCharacterRealName(index, data, true);
			break;
		case 35:
			result = GameMessageUtils.RenderMonth(index, data);
			break;
		case 36:
			result = GameMessageUtils.RenderProfessionName(index, data);
			break;
		case 37:
			result = GameMessageUtils.RenderProfessionSkillName(index, data);
			break;
		case 38:
			result = GameMessageUtils.RenderItemGradeName(index, data);
			break;
		case 39:
			result = GameMessageUtils.RenderText(index, data);
			break;
		case 40:
			result = GameMessageUtils.RenderMusicName(index, data);
			break;
		case 41:
			result = GameMessageUtils.RenderMapStateName(index, data);
			break;
		case 42:
			result = GameMessageUtils.RenderJiaoLoongName(index, data);
			break;
		case 43:
			result = GameMessageUtils.RenderJiaoProperty(index, data);
			break;
		case 44:
			result = GameMessageUtils.RenderDestinyType(index, data);
			break;
		case 45:
			result = GameMessageUtils.RenderSecretInformation(index, data);
			break;
		case 46:
			result = GameMessageUtils.RenderMerchant(index, data);
			break;
		case 47:
			result = GameMessageUtils.RenderLegacy(index, data);
			break;
		case 48:
			result = GameMessageUtils.RenderCharGrade(index, data);
			break;
		case 49:
			result = GameMessageUtils.RenderFeast(index, data);
			break;
		default:
			result = GameMessageUtils.RenderDefault(new ValueTuple<sbyte, int>(type, index), data);
			break;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x04000DDC RID: 3548
	private const string ItemIcon = "charactermenu3_26_icon_daoju";

	// Token: 0x04000DDD RID: 3549
	private const string AdventureIcon = "charactermenu3_26_icon_qiyu";

	// Token: 0x04000DDE RID: 3550
	private const string PoisonIcon = "charactermenu3_26_icon_duyao";

	// Token: 0x04000DDF RID: 3551
	private const string WugIcon = "charactermenu3_26_icon_guchong";

	// Token: 0x04000DE0 RID: 3552
	private const string EquipmentIcon = "charactermenu3_26_icon_zhuangbei";

	// Token: 0x04000DE1 RID: 3553
	public const int DynamicArgumentTypeCount = 4;
}
