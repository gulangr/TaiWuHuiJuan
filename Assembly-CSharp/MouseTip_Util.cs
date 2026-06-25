using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020002E4 RID: 740
public class MouseTip_Util
{
	// Token: 0x06002BC7 RID: 11207 RVA: 0x0015631C File Offset: 0x0015451C
	public static void SetMultiLineAutoHeightText(TextMeshProUGUI textComponent, string textStr)
	{
		textComponent.text = textStr;
		SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
		{
			bool flag = textComponent.textInfo == null;
			if (!flag)
			{
				int lineCount = textComponent.textInfo.lineCount;
				bool flag2;
				if (!(textComponent == null) && !(textComponent.rectTransform == null))
				{
					Vector2 sizeDelta = textComponent.rectTransform.sizeDelta;
					flag2 = false;
				}
				else
				{
					flag2 = true;
				}
				bool flag3 = flag2;
				if (!flag3)
				{
					textComponent.rectTransform.sizeDelta = new Vector2(textComponent.rectTransform.sizeDelta.x, 32f * (float)lineCount);
				}
			}
		});
	}

	// Token: 0x06002BC8 RID: 11208 RVA: 0x0015635C File Offset: 0x0015455C
	public static int AppendAddTeaWineProperty(short subType, RectTransform propertyHolder, TipsAddProperty addPropertyPrefab, int index, short propertyType, int propertyValue, bool templateOnly = false)
	{
		ProfessionModel professionModel = SingletonObject.getInstance<ProfessionModel>();
		bool wineMeet = subType == 901 && !templateOnly && professionModel.IsProfessionalSkillUnlockedAndEquipped(29);
		bool teaMeet = subType == 900 && !templateOnly && professionModel.IsProfessionalSkillUnlockedAndEquipped(65);
		bool flag = wineMeet || teaMeet;
		if (flag)
		{
			bool flag2 = propertyValue < 0;
			if (flag2)
			{
				propertyValue = 0;
			}
		}
		return MouseTip_Util.AppendAddProperty(propertyHolder, addPropertyPrefab, index, propertyType, propertyValue, false, false, false, true, true, false);
	}

	// Token: 0x06002BC9 RID: 11209 RVA: 0x001563D8 File Offset: 0x001545D8
	public static int AppendAddProperty(RectTransform propertyHolder, TipsAddProperty addPropertyPrefab, int index, short propertyType, int propertyValue, bool isRecover = false, bool percent = false, bool showZero = false, bool showAddMark = true, bool isBigIcon = false, bool basedOnPercent = false)
	{
		bool flag = propertyValue == 0 && !showZero;
		int result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			bool flag2 = index < propertyHolder.childCount;
			TipsAddProperty addPropertyUi;
			if (flag2)
			{
				addPropertyUi = propertyHolder.GetChild(index).GetComponent<TipsAddProperty>();
			}
			else
			{
				addPropertyUi = Object.Instantiate<TipsAddProperty>(addPropertyPrefab);
				addPropertyUi.transform.SetParent(propertyHolder);
				addPropertyUi.transform.localScale = Vector3.one;
			}
			addPropertyUi.SetData(propertyType, propertyValue, isRecover, percent, false, showAddMark, isBigIcon, basedOnPercent);
			addPropertyUi.gameObject.SetActive(true);
			result = 1;
		}
		return result;
	}

	// Token: 0x06002BCA RID: 11210 RVA: 0x00156468 File Offset: 0x00154668
	public static int AppendAddPropertyWithConfig(RectTransform propertyHolder, TipsAddProperty addPropertyPrefab, int index, short propertyType, int propertyValue, bool isRecover = false, bool showZero = false, bool showAddMark = true, bool isBigIcon = false)
	{
		bool flag = propertyValue == 0 && !showZero;
		int result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			bool flag2 = index < propertyHolder.childCount;
			TipsAddProperty addPropertyUi;
			if (flag2)
			{
				addPropertyUi = propertyHolder.GetChild(index).GetComponent<TipsAddProperty>();
			}
			else
			{
				addPropertyUi = Object.Instantiate<TipsAddProperty>(addPropertyPrefab);
				addPropertyUi.transform.SetParent(propertyHolder);
				addPropertyUi.transform.localScale = Vector3.one;
			}
			CharacterPropertyReferencedItem propertyReferencedItem = CharacterPropertyReferenced.Instance[propertyType];
			CharacterPropertyDisplayItem propertyDisplayItem = CharacterPropertyDisplay.Instance[propertyReferencedItem.DisplayType];
			addPropertyUi.SetData(propertyType, propertyValue, isRecover, propertyDisplayItem.IsPercent, false, showAddMark, isBigIcon, false);
			addPropertyUi.gameObject.SetActive(true);
			result = 1;
		}
		return result;
	}

	// Token: 0x06002BCB RID: 11211 RVA: 0x00156520 File Offset: 0x00154720
	public static int AppendAddProperty(MultiHorizontalLayoutGroup propertyHolder, TipsAddProperty addPropertyPrefab, int index, short propertyType, int propertyValue, bool isRecover = false, bool percent = false, bool showZero = false, bool showAddMark = true, bool showBackground = false)
	{
		propertyHolder.gameObject.SetActive(true);
		bool flag = propertyValue == 0 && !showZero;
		int result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			bool flag2 = index < propertyHolder.childCount;
			TipsAddProperty addPropertyUi;
			if (flag2)
			{
				addPropertyUi = propertyHolder.GetChild(index).GetComponent<TipsAddProperty>();
			}
			else
			{
				addPropertyUi = Object.Instantiate<TipsAddProperty>(addPropertyPrefab);
				addPropertyUi.transform.localScale = Vector3.one;
			}
			propertyHolder.AddChild(addPropertyUi.transform as RectTransform, showBackground);
			addPropertyUi.SetData(propertyType, propertyValue, isRecover, percent, false, showAddMark, false, false);
			addPropertyUi.gameObject.SetActive(true);
			result = 1;
		}
		return result;
	}

	// Token: 0x06002BCC RID: 11212 RVA: 0x001565C0 File Offset: 0x001547C0
	public static int AppendAddProperty(RectTransform propertyHolder, TipsAddProperty addPropertyPrefab, int index, string icon, string propertyName, int propertyValue, bool isRecover = false, bool percent = false, bool showZero = false, bool showAddMark = true, bool hideIconIfEmpty = false)
	{
		bool flag = propertyValue == 0 && !showZero;
		int result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			bool flag2 = index < propertyHolder.childCount;
			TipsAddProperty addPropertyUi;
			if (flag2)
			{
				addPropertyUi = propertyHolder.GetChild(index).GetComponent<TipsAddProperty>();
			}
			else
			{
				addPropertyUi = Object.Instantiate<TipsAddProperty>(addPropertyPrefab);
				addPropertyUi.transform.SetParent(propertyHolder);
				addPropertyUi.transform.localScale = Vector3.one;
			}
			addPropertyUi.SetData(icon, propertyName, propertyValue, isRecover, percent, false, showAddMark, hideIconIfEmpty);
			addPropertyUi.gameObject.SetActive(true);
			result = 1;
		}
		return result;
	}

	// Token: 0x06002BCD RID: 11213 RVA: 0x00156650 File Offset: 0x00154850
	public static void UpdatePoisonResistRefers(Refers poisonRefers, ECharacterPropertyDisplayType displayType, int value)
	{
		poisonRefers.gameObject.SetActive(value != 0);
		bool flag = value == 0;
		if (!flag)
		{
			CharacterPropertyDisplayItem config = CharacterPropertyDisplay.Instance[(short)displayType];
			string symbol = config.IsPercent ? "%" : string.Empty;
			poisonRefers.CGet<TextMeshProUGUI>("AddValue").text = ((value > 0) ? string.Format("+{0}{1}", value, symbol) : "");
			poisonRefers.CGet<TextMeshProUGUI>("ReduceValue").text = ((value > 0) ? "" : string.Format("{0}{1}", value, symbol));
		}
	}

	// Token: 0x06002BCE RID: 11214 RVA: 0x001566F4 File Offset: 0x001548F4
	public static string GetCharPropertyFontColor(short displayPropertyType, string defaultColor)
	{
		string result;
		switch (displayPropertyType)
		{
		case 28:
			result = "PoisonType_Hot";
			break;
		case 29:
			result = "PoisonType_Gloomy";
			break;
		case 30:
			result = "PoisonType_Cold";
			break;
		case 31:
			result = "PoisonType_Red";
			break;
		case 32:
			result = "PoisonType_Rotten";
			break;
		case 33:
			result = "PoisonType_Illusory";
			break;
		default:
			result = defaultColor;
			break;
		}
		return result;
	}

	// Token: 0x06002BCF RID: 11215 RVA: 0x00156760 File Offset: 0x00154960
	public static string GetCombatSkillPropertyFontColor(short skillPropertyType, string defaultColor)
	{
		string result;
		switch (skillPropertyType)
		{
		case 42:
			result = "PoisonType_Hot";
			break;
		case 43:
			result = "PoisonType_Gloomy";
			break;
		case 44:
			result = "PoisonType_Cold";
			break;
		case 45:
			result = "PoisonType_Red";
			break;
		case 46:
			result = "PoisonType_Rotten";
			break;
		case 47:
			result = "PoisonType_Illusory";
			break;
		default:
			result = defaultColor;
			break;
		}
		return result;
	}

	// Token: 0x06002BD0 RID: 11216 RVA: 0x001567CC File Offset: 0x001549CC
	public unsafe static string GetNeiliAllocationTips(sbyte neiliType, byte neiliAllocationType, short allocatedValue, short effectValue)
	{
		NeiliTypeItem typeConfig = NeiliType.Instance[neiliType];
		NeiliAllocationEffectItem configData = NeiliAllocationEffect.Instance[(int)neiliAllocationType];
		StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
		strBuilder.Clear();
		bool flag = allocatedValue > 0 || effectValue > 0;
		if (flag)
		{
			for (sbyte hitType = 0; hitType < 4; hitType += 1)
			{
				MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)(*(ref configData.HitValues.Items.FixedElementField + (IntPtr)hitType * 2)), (int)(*(ref typeConfig.HitValues.Items.FixedElementField + (IntPtr)hitType * 2)), (int)(6 + hitType));
				MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)(*(ref configData.AvoidValues.Items.FixedElementField + (IntPtr)hitType * 2)), (int)(*(ref typeConfig.AvoidValues.Items.FixedElementField + (IntPtr)hitType * 2)), (int)(12 + hitType));
			}
			MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)configData.Penetrations.Outer, (int)typeConfig.Penetrations.Outer, 10);
			MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)configData.Penetrations.Inner, (int)typeConfig.Penetrations.Inner, 11);
			MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)configData.PenetrationResists.Outer, (int)typeConfig.PenetrationResists.Outer, 16);
			MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)configData.PenetrationResists.Inner, (int)typeConfig.PenetrationResists.Inner, 17);
			MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)configData.RecoveryOfStanceAndBreath.Outer, (int)typeConfig.RecoveryOfStanceAndBreath.Outer, 18);
			MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)configData.RecoveryOfStanceAndBreath.Inner, (int)typeConfig.RecoveryOfStanceAndBreath.Inner, 19);
			MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)configData.MoveSpeed, (int)typeConfig.MoveSpeed, 20);
			MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)configData.RecoveryOfFlaw, (int)typeConfig.RecoveryOfFlaw, 21);
			MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)configData.CastSpeed, (int)typeConfig.CastSpeed, 22);
			MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)configData.RecoveryOfBlockedAcupoint, (int)typeConfig.RecoveryOfBlockedAcupoint, 23);
			MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)configData.WeaponSwitchSpeed, (int)typeConfig.WeaponSwitchSpeed, 24);
			MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)configData.AttackSpeed, (int)typeConfig.AttackSpeed, 25);
			MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)configData.InnerRatio, (int)typeConfig.InnerRatio, 26);
			MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)configData.RecoveryOfQiDisorder, (int)typeConfig.RecoveryOfQiDisorder, 27);
			for (sbyte order = 0; order < 6; order += 1)
			{
				sbyte type = PoisonType.GetTypeBySortingOrder(order);
				MouseTip_Util.AppendNeiliAllocationAddPropertyTips(strBuilder, allocatedValue, effectValue, (int)(*(ref configData.PoisonResists.Items.FixedElementField + (IntPtr)type * 2)), (int)(*(ref typeConfig.PoisonResists.Items.FixedElementField + (IntPtr)type * 2)), (int)(28 + type));
			}
		}
		string tipsContent = strBuilder.ToString();
		EasyPool.Free<StringBuilder>(strBuilder);
		return tipsContent;
	}

	// Token: 0x06002BD1 RID: 11217 RVA: 0x00156A9C File Offset: 0x00154C9C
	private static void AppendNeiliAllocationAddPropertyTips(StringBuilder strBuilder, short neiliAllocationValue, short allocationEffectValue, int stepValue, int addUnit, int propertyType)
	{
		bool flag = stepValue <= 0;
		if (!flag)
		{
			int addValue = ((int)neiliAllocationValue / stepValue + (int)allocationEffectValue / stepValue) * addUnit;
			bool flag2 = addValue > 0;
			if (flag2)
			{
				strBuilder.Append(string.Format("<SpName={0}>{1}<color=#brightblue>+{2}</color>\n", CharacterPropertyDisplay.Instance[propertyType].TipsIcon, CharacterPropertyDisplay.Instance[propertyType].Name, addValue));
			}
		}
	}

	// Token: 0x06002BD2 RID: 11218 RVA: 0x00156B04 File Offset: 0x00154D04
	public static void UpdateMixPoisonEffectText(TextMeshProUGUI effectText, string effectStr)
	{
		effectStr = "                  " + effectStr.ColorReplace();
		float textWidth = effectText.rectTransform.sizeDelta.x;
		Vector2 preferredSize = effectText.GetPreferredValues(effectStr, textWidth, float.PositiveInfinity);
		effectText.rectTransform.sizeDelta = preferredSize.SetX(textWidth);
		effectText.text = effectStr;
		TMPTextSpriteHelper component = effectText.GetComponent<TMPTextSpriteHelper>();
		if (component != null)
		{
			component.Parse();
		}
	}

	// Token: 0x06002BD3 RID: 11219 RVA: 0x00156B70 File Offset: 0x00154D70
	public static bool HasEquipForCompare(ItemDisplayData selectedItem, List<ItemKey> equipItems)
	{
		bool flag = selectedItem == null || equipItems == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			switch (ItemTemplateHelper.GetEquipmentType(selectedItem.Key.ItemType, selectedItem.Key.TemplateId))
			{
			case 0:
				for (sbyte index = 0; index <= 2; index += 1)
				{
					bool flag2 = (int)index < equipItems.Count && equipItems[(int)index].IsValid();
					if (flag2)
					{
						return true;
					}
				}
				break;
			case 1:
				return 3 < equipItems.Count && equipItems[3].IsValid();
			case 2:
				return 4 < equipItems.Count && equipItems[4].IsValid();
			case 3:
				return 5 < equipItems.Count && equipItems[5].IsValid();
			case 4:
				return 6 < equipItems.Count && equipItems[6].IsValid();
			case 5:
				return 7 < equipItems.Count && equipItems[7].IsValid();
			case 6:
				for (sbyte index2 = 8; index2 <= 10; index2 += 1)
				{
					bool flag3 = (int)index2 < equipItems.Count && equipItems[(int)index2].IsValid();
					if (flag3)
					{
						return true;
					}
				}
				break;
			case 7:
				return 11 < equipItems.Count && equipItems[11].IsValid();
			}
			result = false;
		}
		return result;
	}

	// Token: 0x06002BD4 RID: 11220 RVA: 0x00156D40 File Offset: 0x00154F40
	public static void ShowEquipCompare(UIElement element, ItemDisplayData itemData, int charId, MouseTipBase tipsObj, int equipmentOwnerId = -1)
	{
		bool flag = tipsObj == null || element == null || !element.Ready || tipsObj.transform == null;
		if (!flag)
		{
			bool flag2 = equipmentOwnerId < 0;
			if (flag2)
			{
				equipmentOwnerId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
			CharacterDomainMethod.AsyncCall.GetAllEquipmentItems(null, equipmentOwnerId, delegate(int offset, RawDataPool dataPool)
			{
				List<ItemDisplayData> equipments = null;
				Serializer.Deserialize(dataPool, offset, ref equipments);
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("NewData", itemData);
				argBox.SetObject("NewTips", tipsObj);
				argBox.Set("CharId", charId);
				argBox.SetObject("Equipments", equipments);
				ItemKey originCarrier = equipments[11].Key;
				bool flag3 = itemData.Key.ItemType == 4;
				if (flag3)
				{
					bool flag4 = ItemTemplateHelper.IsJiaoLoong(originCarrier.ItemType, originCarrier.TemplateId);
					if (flag4)
					{
						AsyncMethodCallbackDelegate <>9__3;
						ExtraDomainMethod.AsyncCall.GetJiaoLoongDisplayDataByItemKey(null, originCarrier, delegate(int offset2, RawDataPool dataPool2)
						{
							JiaoLoongDisplayData displayData = new JiaoLoongDisplayData();
							Serializer.Deserialize(dataPool2, offset2, ref displayData);
							argBox.SetObject("OriginJiaoLoongData", displayData);
							bool flag6 = ItemTemplateHelper.IsJiaoLoong(itemData.Key.ItemType, itemData.Key.TemplateId);
							if (flag6)
							{
								IAsyncMethodRequestHandler requestHandler = null;
								ItemKey key = itemData.Key;
								AsyncMethodCallbackDelegate callback;
								if ((callback = <>9__3) == null)
								{
									callback = (<>9__3 = delegate(int offset3, RawDataPool dataPool3)
									{
										JiaoLoongDisplayData newDisplayData = new JiaoLoongDisplayData();
										Serializer.Deserialize(dataPool3, offset3, ref newDisplayData);
										argBox.SetObject("NewJiaoLoongData", newDisplayData);
										UIElement.EquipCompareTips.SetOnInitArgs(argBox);
										UIManager.Instance.ShowUI(UIElement.EquipCompareTips, true);
									});
								}
								ExtraDomainMethod.AsyncCall.GetJiaoLoongDisplayDataByItemKey(requestHandler, key, callback);
							}
							else
							{
								UIElement.EquipCompareTips.SetOnInitArgs(argBox);
								UIManager.Instance.ShowUI(UIElement.EquipCompareTips, true);
							}
						});
					}
					else
					{
						bool flag5 = ItemTemplateHelper.IsJiaoLoong(itemData.Key.ItemType, itemData.Key.TemplateId);
						if (flag5)
						{
							ExtraDomainMethod.AsyncCall.GetJiaoLoongDisplayDataByItemKey(null, itemData.Key, delegate(int offset3, RawDataPool dataPool3)
							{
								JiaoLoongDisplayData newDisplayData = new JiaoLoongDisplayData();
								Serializer.Deserialize(dataPool3, offset3, ref newDisplayData);
								argBox.SetObject("NewJiaoLoongData", newDisplayData);
								UIElement.EquipCompareTips.SetOnInitArgs(argBox);
								UIManager.Instance.ShowUI(UIElement.EquipCompareTips, true);
							});
						}
						else
						{
							UIElement.EquipCompareTips.SetOnInitArgs(argBox);
							UIManager.Instance.ShowUI(UIElement.EquipCompareTips, true);
						}
					}
				}
				else
				{
					UIElement.EquipCompareTips.SetOnInitArgs(argBox);
					UIManager.Instance.ShowUI(UIElement.EquipCompareTips, true);
				}
			});
		}
	}

	// Token: 0x06002BD5 RID: 11221 RVA: 0x00156DC8 File Offset: 0x00154FC8
	public static void SetInventoryOverLoadTips(TooltipInvoker tipDisplayer, List<IntPair> overweightSanctionPercent, int moveTimeCostPercent)
	{
		bool flag = overweightSanctionPercent == null;
		if (!flag)
		{
			int finalSpeed = moveTimeCostPercent - 100;
			int sanction = 0;
			foreach (IntPair pair in overweightSanctionPercent)
			{
				sanction = Math.Max(pair.Second, sanction);
			}
			WorldStateItem worldStateItem = WorldState.Instance[11];
			TooltipInvoker tipDisplayer2 = tipDisplayer;
			if (tipDisplayer2.RuntimeParam == null)
			{
				tipDisplayer2.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			tipDisplayer.RuntimeParam.Clear();
			tipDisplayer.RuntimeParam.Set("Title", worldStateItem.Name);
			tipDisplayer.RuntimeParam.Set("Desc", worldStateItem.Desc);
			tipDisplayer.RuntimeParam.Set("SubTitle", LocalStringManager.Get(LanguageKey.UI_WorldState_InventoryOverload_MoveTimeCost));
			List<MouseTipDynamicCondition.ConditionData> conditionList = new List<MouseTipDynamicCondition.ConditionData>();
			bool flag2 = sanction != 0;
			if (flag2)
			{
				MouseTipDynamicCondition.ConditionData burdenPunishmentCondition = default(MouseTipDynamicCondition.ConditionData);
				burdenPunishmentCondition.Name = LocalStringManager.Get(LanguageKey.UI_WorldState_InventoryOverload_BurdenPunishment);
				bool flag3 = sanction > 0;
				if (flag3)
				{
					burdenPunishmentCondition.ReduceValueString = string.Format("{0}%", sanction);
				}
				else
				{
					burdenPunishmentCondition.AddValueString = string.Format("{0}%", sanction);
				}
				conditionList.Add(burdenPunishmentCondition);
			}
			bool flag4 = finalSpeed != 0;
			if (flag4)
			{
				MouseTipDynamicCondition.ConditionData moveTimeCostPunishmentCondition = default(MouseTipDynamicCondition.ConditionData);
				moveTimeCostPunishmentCondition.Name = LocalStringManager.Get(LanguageKey.UI_WorldState_InventoryOverload_MoveTimeCostPunishment);
				bool flag5 = finalSpeed > 0;
				if (flag5)
				{
					moveTimeCostPunishmentCondition.ReduceValueString = string.Format("+{0}%", finalSpeed);
				}
				else
				{
					moveTimeCostPunishmentCondition.AddValueString = string.Format("{0}%", finalSpeed);
				}
				conditionList.Add(moveTimeCostPunishmentCondition);
			}
			bool flag6 = conditionList.Count > 0;
			if (flag6)
			{
				tipDisplayer.RuntimeParam.SetObject("Conditions", conditionList);
			}
			tipDisplayer.Type = TipType.DynamicCondition;
			TaiwuDomainMethod.AsyncCall.GetInventoryOverloadedGroupCharNames(null, delegate(int offset, RawDataPool dataPool)
			{
				List<CharacterDisplayData> charDataList = null;
				Serializer.Deserialize(dataPool, offset, ref charDataList);
				int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				bool flag7 = charDataList != null && tipDisplayer.RuntimeParam != null;
				if (flag7)
				{
					string extraDesc = string.Empty;
					foreach (CharacterDisplayData data in charDataList)
					{
						bool isTaiwu = taiwuCharId == data.CharacterId;
						string name = NameCenter.GetMonasticTitleOrDisplayName(data, isTaiwu);
						extraDesc = extraDesc + "\n" + LocalStringManager.GetFormat(LanguageKey.LK_Char_Inventory_Overflow, name).ColorReplace();
					}
					tipDisplayer.RuntimeParam.Set("ExtraDesc", extraDesc);
				}
			});
		}
	}

	// Token: 0x06002BD6 RID: 11222 RVA: 0x00157000 File Offset: 0x00155200
	public static string GetItemDurabilityColorString(int currentDurability, int maxDurability)
	{
		bool isHalf = currentDurability > maxDurability / 2;
		return currentDurability.ToString().SetColor(isHalf ? "pinkyellow" : "brightred") + "/" + maxDurability.ToString().SetColor("pinkyellow");
	}
}
