using System;
using System.Collections.Generic;
using System.Text;
using CharacterDataMonitor;
using Config;
using FrameWork;
using Game.Components.Common;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200007D RID: 125
public class MouseTipItem : MouseTipBase
{
	// Token: 0x0600048A RID: 1162 RVA: 0x0001DE68 File Offset: 0x0001C068
	protected override void Init(ArgumentBox argsBox)
	{
		bool isNew;
		this._isNew = (argsBox.Get("IsNew", out isNew) && isNew);
		this.DisableCompare = (argsBox.Get("DisableCompare", out this.DisableCompare) && this.DisableCompare);
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x0001DEB0 File Offset: 0x0001C0B0
	protected void PostInit()
	{
		bool flag = !this.DisableCompare;
		if (flag)
		{
			CharacterDomainMethod.AsyncCall.GetEquipmentKeys(null, this._charId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._currentEquipments);
			});
		}
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x0001DEE8 File Offset: 0x0001C0E8
	protected void RefreshHoldCount()
	{
		bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.Adventure || GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
		if (flag)
		{
			TaiwuDomainMethod.AsyncCall.GetItemCount(this, this._itemData.Key.ItemType, this._itemData.Key.TemplateId, delegate(int offset, RawDataPool dataPool)
			{
				TextMeshProUGUI text2;
				bool flag3 = this._destroyed || !this.CTryGet<TextMeshProUGUI>("HoldCountText", out text2) || !text2;
				if (!flag3)
				{
					int count = 0;
					Serializer.Deserialize(dataPool, offset, ref count);
					bool isNew = this._isNew;
					if (isNew)
					{
						count -= this._itemData.Amount;
					}
					count = Mathf.Max(0, count);
					text2.transform.parent.gameObject.SetActive(true);
					text2.text = LocalStringManager.GetFormat(LanguageKey.LK_Item_Mousetip_TotalCount, count.ToString().SetColor("pinkyellow"));
				}
			});
		}
		else
		{
			TextMeshProUGUI text;
			bool flag2 = this.CTryGet<TextMeshProUGUI>("HoldCountText", out text) && text;
			if (flag2)
			{
				text.transform.parent.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x0001DF80 File Offset: 0x0001C180
	protected override void OnDisable()
	{
		base.OnDisable();
		foreach (GameObject go in this._equipmentEffectList)
		{
			go.SetActive(false);
		}
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x0001DFE0 File Offset: 0x0001C1E0
	protected void OnDestroy()
	{
		this._destroyed = true;
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x0001DFEC File Offset: 0x0001C1EC
	protected virtual void InitItemDisableFunctionList(ItemDisplayData itemDisplayData)
	{
		this._disableFunctionList.Clear();
		ItemKey key = itemDisplayData.Key;
		bool dissemblable = ItemTemplateHelper.GetCanDisassemble(key.ItemType, key.TemplateId);
		bool flag = !dissemblable;
		if (flag)
		{
			this._disableFunctionList.Add(MouseTipItem.ItemFunction.Disassemble);
		}
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x0001E034 File Offset: 0x0001C234
	protected void RefreshDisableFunction()
	{
		bool showDisableFunc = this._disableFunctionList.Count > 0;
		GameObject disableFunctionLayout = base.CGet<GameObject>("DisableFunctionLayout");
		disableFunctionLayout.gameObject.SetActive(showDisableFunc);
		bool flag = showDisableFunc;
		if (flag)
		{
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			for (int i = 0; i < this._disableFunctionList.Count; i++)
			{
				MouseTipItem.ItemFunction function = this._disableFunctionList[i];
				if (!true)
				{
				}
				string text2;
				switch (function)
				{
				case MouseTipItem.ItemFunction.Repairable:
					text2 = LocalStringManager.Get(LanguageKey.LK_Repair_Item).SetColor("repair");
					break;
				case MouseTipItem.ItemFunction.Transferable:
					text2 = LocalStringManager.Get(LanguageKey.LK_Transfer_Item).SetColor("gift");
					break;
				case MouseTipItem.ItemFunction.Poisonable:
					text2 = LocalStringManager.Get(LanguageKey.LK_Poison_Item).SetColor("darkpurple");
					break;
				case MouseTipItem.ItemFunction.Refinable:
					text2 = LocalStringManager.Get(LanguageKey.LK_Strengthen_Item).SetColor("lightblue");
					break;
				case MouseTipItem.ItemFunction.Disassemble:
					text2 = LocalStringManager.Get(LanguageKey.LK_Disassemble_Item).SetColor("pinkyellow");
					break;
				case MouseTipItem.ItemFunction.CanChangeTrick:
					text2 = LocalStringManager.Get(LanguageKey.LK_WeaponChangeTrickPercent).SetColor("brightred");
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				if (!true)
				{
				}
				string functionKey = text2;
				sb.Append(functionKey);
				bool flag2 = i < this._disableFunctionList.Count - 1;
				if (flag2)
				{
					sb.Append(LocalStringManager.Get(LanguageKey.LK_ItemTips_DisableFunctionSeparator));
				}
			}
			string text = LocalStringManager.GetFormat(LanguageKey.LK_ItemTips_Function, sb.ToString());
			disableFunctionLayout.GetComponentInChildren<TextMeshProUGUI>().text = text;
			EasyPool.Free<StringBuilder>(sb);
		}
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x0001E1D0 File Offset: 0x0001C3D0
	protected unsafe bool RefreshSelfPoisons(PoisonsAndLevels innatePoisons)
	{
		bool hasInnatePoison = innatePoisons.IsNonZero();
		Refers poisonLayout = base.CGet<Refers>("PoisonLayout");
		Refers poisons = poisonLayout.CGet<Refers>("SelfPoisons");
		poisons.gameObject.SetActive(hasInnatePoison);
		bool flag = hasInnatePoison;
		if (flag)
		{
			RectTransform poisonHolder = poisons.CGet<RectTransform>("PoisonHolder");
			for (sbyte order = 0; order < 6; order += 1)
			{
				sbyte type = PoisonType.GetTypeBySortingOrder(order);
				PoisonItem poisonTypeConfig = Poison.Instance[type];
				Refers poisonRefers = poisonHolder.GetChild((int)type).GetComponent<Refers>();
				short innatePoisonValue = *(ref innatePoisons.Values.FixedElementField + (IntPtr)type * 2);
				sbyte innatePoisonLevel = *(ref innatePoisons.Levels.FixedElementField + type);
				bool show = innatePoisonValue > 0;
				poisonRefers.gameObject.SetActive(show);
				bool flag2 = show;
				if (flag2)
				{
					poisonRefers.CGet<TextMeshProUGUI>("Name").text = poisonTypeConfig.Name;
					poisonRefers.CGet<TextMeshProUGUI>("Value").text = innatePoisonValue.ToString();
					poisonRefers.CGet<CImage>("Icon").SetSprite(MouseTipBase.GetPoisonBigIcon(type), false, null);
					poisonRefers.CGet<CImage>("LevelIcon").SetSprite(MouseTipBase.GetPoisonLevelIcon(innatePoisonLevel), false, null);
				}
			}
			this.ForceRebuildLayout(1U, poisonHolder.parent.transform as RectTransform);
		}
		return hasInnatePoison;
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x0001E338 File Offset: 0x0001C538
	protected unsafe bool RefreshAttachedPoisons(ItemDisplayData itemData)
	{
		FullPoisonEffects poisonEffects = itemData.PoisonEffects ?? new FullPoisonEffects();
		bool hasPoisonEffect = poisonEffects.IsValid && poisonEffects.IsIdentified;
		Refers poisonLayout = base.CGet<Refers>("PoisonLayout");
		Refers poisons = poisonLayout.CGet<Refers>("AttachedPoisons");
		poisons.gameObject.SetActive(hasPoisonEffect);
		RectTransform poisonHolder = poisons.CGet<RectTransform>("PoisonHolder");
		for (sbyte order = 0; order < 6; order += 1)
		{
			sbyte type = PoisonType.GetTypeBySortingOrder(order);
			PoisonItem poisonTypeConfig = Poison.Instance[type];
			Refers poisonRefers = poisonHolder.GetChild((int)type).GetComponent<Refers>();
			PoisonsAndLevels attachedPoison = poisonEffects.GetAllPoisonsAndLevels();
			short attachedPoisonValue = *(ref attachedPoison.Values.FixedElementField + (IntPtr)type * 2);
			sbyte attachedPoisonLevel = *(ref attachedPoison.Levels.FixedElementField + type);
			bool show = attachedPoisonValue > 0;
			poisonRefers.gameObject.SetActive(show);
			bool flag = show;
			if (flag)
			{
				poisonRefers.CGet<TextMeshProUGUI>("Name").text = poisonTypeConfig.Name;
				poisonRefers.CGet<TextMeshProUGUI>("Value").text = attachedPoisonValue.ToString();
				poisonRefers.CGet<CImage>("Icon").SetSprite(MouseTipBase.GetPoisonBigIcon(type), false, null);
				poisonRefers.CGet<CImage>("LevelIcon").SetSprite(MouseTipBase.GetPoisonLevelIcon(attachedPoisonLevel), false, null);
				bool isCondensed = poisonEffects.IsCondensed && poisonEffects.PoisonSlotList.Exists((PoisonSlot s) => s.IsCondensed && s.MedicineConfig.PoisonType == type);
				poisonRefers.CGet<GameObject>("Condense").SetActive(isCondensed);
			}
		}
		this.ForceRebuildLayout(1U, poisonHolder.parent.transform as RectTransform);
		Refers mixPoisonEffect = poisonLayout.CGet<Refers>("MixPoisonEffect");
		mixPoisonEffect.gameObject.SetActive(hasPoisonEffect && poisonEffects.IsThreePoisonsMix());
		bool activeSelf = mixPoisonEffect.gameObject.activeSelf;
		if (activeSelf)
		{
			bool showInCombat = ItemType.IsEquipmentItemType(itemData.Key.ItemType) || ItemType.IsEatable(itemData.Key.ItemType);
			bool showOutCombat = 10 == itemData.Key.ItemType || ItemType.IsEatable(itemData.Key.ItemType);
			MedicineItem medicineConfig = Medicine.Instance[poisonEffects.GetMedicineTemplateId()];
			GameObject inCombatTitle = mixPoisonEffect.CGet<GameObject>("MixPoisonEffectInCombatTitle");
			GameObject outCombatTitle = mixPoisonEffect.CGet<GameObject>("MixPoisonEffectOutCombatTitle");
			inCombatTitle.SetActive(showInCombat);
			outCombatTitle.SetActive(showOutCombat);
			TextMeshProUGUI inCombatDesc = mixPoisonEffect.CGet<TextMeshProUGUI>("MixPoisonEffectInCombatDesc");
			TextMeshProUGUI outCombatDesc = mixPoisonEffect.CGet<TextMeshProUGUI>("MixPoisonEffectOutCombatDesc");
			inCombatDesc.gameObject.SetActive(showInCombat);
			outCombatDesc.gameObject.SetActive(showOutCombat);
			bool flag2 = showInCombat;
			if (flag2)
			{
				MouseTip_Util.UpdateMixPoisonEffectText(inCombatDesc, medicineConfig.SpecialEffectDesc);
				TextMeshProUGUI inCombatName = mixPoisonEffect.CGet<TextMeshProUGUI>("MixPoisonEffectInCombatName");
				inCombatName.text = medicineConfig.Name + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
			}
			bool flag3 = showOutCombat;
			if (flag3)
			{
				MouseTip_Util.UpdateMixPoisonEffectText(outCombatDesc, medicineConfig.Desc);
				TextMeshProUGUI outCombatName = mixPoisonEffect.CGet<TextMeshProUGUI>("MixPoisonEffectOutCombatName");
				outCombatName.text = medicineConfig.Name + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
			}
		}
		bool showPoisonIdentifyTips = itemData.PoisonIsIdentified && !itemData.HasAnyPoison;
		GameObject poisonIdentifyLayout = poisonLayout.CGet<GameObject>("PoisonIdentifyLayout");
		poisonIdentifyLayout.SetActive(showPoisonIdentifyTips);
		poisonIdentifyLayout.GetComponentInChildren<TextMeshProUGUI>().text = LocalStringManager.Get(LanguageKey.LK_ItemTips_No_Poison).SetColor("brightblue");
		return hasPoisonEffect;
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x0001E6F4 File Offset: 0x0001C8F4
	protected void RefreshPoisons(PoisonsAndLevels innatePoisons, ItemDisplayData itemData)
	{
		Refers poisonLayout = base.CGet<Refers>("PoisonLayout");
		bool hasInnatePoison = this.RefreshSelfPoisons(innatePoisons);
		bool hasPoisonEffect = this.RefreshAttachedPoisons(itemData);
		bool showPoisonTitle = hasInnatePoison || hasPoisonEffect;
		poisonLayout.CGet<GameObject>("PoisonTitle").SetActive(showPoisonTitle);
		bool isEatable = ItemType.IsEatable(itemData.Key.ItemType);
		poisonLayout.CGet<GameObject>("EatingPoisonTimeTips").SetActive(isEatable && showPoisonTitle);
		TextMeshProUGUI poisonTip = poisonLayout.CGet<TextMeshProUGUI>("PoisonTips");
		sbyte itemType = itemData.Key.ItemType;
		if (!true)
		{
		}
		LanguageKey languageKey;
		switch (itemType)
		{
		case 0:
			languageKey = LanguageKey.LK_Weapon_Poison_Tips;
			break;
		case 1:
			languageKey = LanguageKey.LK_Armor_Poison_Tips;
			break;
		case 2:
			languageKey = LanguageKey.LK_Accessory_Poison_Tips;
			break;
		default:
			if (itemType != 10)
			{
				languageKey = LanguageKey.Invalid;
			}
			else
			{
				languageKey = LanguageKey.LK_Book_Poison_Tips;
			}
			break;
		}
		if (!true)
		{
		}
		LanguageKey tipKey = languageKey;
		bool showPoisonTip = tipKey != LanguageKey.Invalid && showPoisonTitle;
		poisonTip.text = LocalStringManager.Get(tipKey);
		poisonTip.transform.parent.gameObject.SetActive(showPoisonTip);
		bool showPoisonLayout = false;
		for (int i = 0; i < poisonLayout.transform.childCount; i++)
		{
			bool activeSelf = poisonLayout.transform.GetChild(i).gameObject.activeSelf;
			if (activeSelf)
			{
				showPoisonLayout = true;
				break;
			}
		}
		poisonLayout.gameObject.SetActive(showPoisonLayout);
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x0001E854 File Offset: 0x0001CA54
	protected void RefreshRefiningEffect(ItemDisplayData itemData, int charId)
	{
		bool hasRefiningEffect = itemData.RefiningEffects.IsRefined;
		base.CGet<GameObject>("RefiningEffects").SetActive(hasRefiningEffect);
		bool flag = hasRefiningEffect;
		if (flag)
		{
			RectTransform refiningEffectHolder = base.CGet<RectTransform>("RefiningEffectHolder");
			TipsRefiningEffect effectPrefab = refiningEffectHolder.GetChild(0).GetComponent<TipsRefiningEffect>();
			int matIndex = 0;
			for (int i = 0; i < 5; i++)
			{
				short materialId = itemData.RefiningEffects.GetMaterialTemplateIdAt(i);
				bool flag2 = materialId < 0;
				if (!flag2)
				{
					bool flag3 = matIndex < refiningEffectHolder.childCount;
					TipsRefiningEffect effectUi;
					if (flag3)
					{
						effectUi = refiningEffectHolder.GetChild(matIndex).GetComponent<TipsRefiningEffect>();
					}
					else
					{
						effectUi = Object.Instantiate<TipsRefiningEffect>(effectPrefab, refiningEffectHolder, true);
						effectUi.transform.localScale = Vector3.one;
					}
					effectUi.gameObject.SetActive(true);
					MaterialItem materialConfig = Config.Material.Instance[materialId];
					RefiningEffectItem refineConfig = RefiningEffect.Instance[materialConfig.RefiningEffect];
					sbyte itemType = itemData.Key.ItemType;
					if (!true)
					{
					}
					sbyte b;
					switch (itemType)
					{
					case 0:
						b = (sbyte)refineConfig.WeaponType;
						break;
					case 1:
						b = (sbyte)refineConfig.ArmorType;
						break;
					case 2:
						b = (sbyte)refineConfig.AccessoryType;
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
					if (!true)
					{
					}
					sbyte propertyType = b;
					sbyte itemType2 = itemData.Key.ItemType;
					if (!true)
					{
					}
					sbyte[] array;
					switch (itemType2)
					{
					case 0:
						array = refineConfig.WeaponBonusValues;
						break;
					case 1:
						array = refineConfig.ArmorBonusValues;
						break;
					case 2:
						array = refineConfig.AccessoryBonusValues;
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
					if (!true)
					{
					}
					sbyte[] values = array;
					bool isPercent = itemData.Key.ItemType != 2;
					effectUi.SetData(charId, itemData.Key.ItemType, propertyType, (int)values[(int)materialConfig.Grade], isPercent, false);
					matIndex++;
				}
			}
			for (int j = matIndex; j < refiningEffectHolder.childCount; j++)
			{
				refiningEffectHolder.GetChild(j).gameObject.SetActive(false);
			}
			this.ForceRebuildLayout(1U, refiningEffectHolder.parent.transform as RectTransform);
		}
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x0001EA84 File Offset: 0x0001CC84
	protected void RefreshEquipmentEffect(ItemDisplayData itemData)
	{
		GameObject layout = base.CGet<GameObject>("AttachedEffectLayout");
		GameObject templateEquipmentEffect = base.CGet<TextMeshProUGUI>("EquipmentEffect").gameObject;
		this._equipmentEffectList.Clear();
		for (int index = templateEquipmentEffect.transform.GetSiblingIndex(); index < layout.transform.childCount; index++)
		{
			Transform equipmentEffect = layout.transform.GetChild(index);
			this._equipmentEffectList.Add(equipmentEffect.gameObject);
			equipmentEffect.gameObject.SetActive(false);
		}
		List<ValueTuple<string, string>> effects = new List<ValueTuple<string, string>>();
		IItemConfig config = itemData.Key.GetConfig();
		bool flag = config.EquipmentMasteryId >= 0;
		if (flag)
		{
			SpecialEffectItem effectConfig = SpecialEffect.Instance[config.EquipmentMasteryId];
			effects.Add(new ValueTuple<string, string>(effectConfig.Name, effectConfig.Desc[0]));
		}
		List<short> equipmentEffectIds = itemData.EquipmentEffectIds;
		bool flag2 = equipmentEffectIds != null && equipmentEffectIds.Count > 0;
		if (flag2)
		{
			foreach (short effectId in itemData.EquipmentEffectIds)
			{
				EquipmentEffectItem effectConfig2 = EquipmentEffect.Instance[effectId];
				effects.Add(new ValueTuple<string, string>(effectConfig2.Name, effectConfig2.Desc));
			}
		}
		bool flag3 = effects != null && effects.Count > 0;
		if (flag3)
		{
			for (int i = 0; i < effects.Count; i++)
			{
				bool flag4 = i >= this._equipmentEffectList.Count;
				if (flag4)
				{
					GameObject instantiateEquipmentEffect = Object.Instantiate<GameObject>(templateEquipmentEffect, templateEquipmentEffect.transform.parent, true);
					this._equipmentEffectList.Add(instantiateEquipmentEffect);
				}
				GameObject equipmentEffect2 = this._equipmentEffectList[i];
				ValueTuple<string, string> valueTuple = effects[i];
				string effectName = valueTuple.Item1;
				string effectDesc = valueTuple.Item2;
				for (int j = 0; j < equipmentEffect2.transform.childCount; j++)
				{
					TextMeshProUGUI effectTitle = equipmentEffect2.transform.GetChild(j).GetComponent<TextMeshProUGUI>();
					bool flag5 = effectTitle == null;
					if (!flag5)
					{
						effectTitle.text = effectName + LocalStringManager.Get(LanguageKey.LK_Colon_Symbol);
						effectTitle.ForceMeshUpdate(false, false);
						float titleX = effectTitle.rectTransform.anchoredPosition.x;
						float titleWidth = effectTitle.GetPreferredValues().x;
						string descStr = string.Format("<pos={0}>{1}", titleX + titleWidth + 4f, effectDesc);
						MouseTip_Util.SetMultiLineAutoHeightText(equipmentEffect2.GetComponent<TextMeshProUGUI>(), descStr);
						equipmentEffect2.SetActive(true);
					}
				}
			}
		}
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x0001ED60 File Offset: 0x0001CF60
	protected void RefreshAttachedEffectLayout()
	{
		GameObject layout = base.CGet<GameObject>("AttachedEffectLayout");
		bool show = false;
		for (int i = 0; i < layout.transform.childCount; i++)
		{
			bool activeSelf = layout.transform.GetChild(i).gameObject.activeSelf;
			if (activeSelf)
			{
				show = true;
				break;
			}
		}
		layout.gameObject.SetActive(show);
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x0001EDC6 File Offset: 0x0001CFC6
	protected void RefreshAttachedEffect()
	{
		this.RefreshRefiningEffect(this._itemData, this._charId);
		this.RefreshEquipmentEffect(this._itemData);
		this.RefreshAttachedEffectLayout();
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x0001EDF0 File Offset: 0x0001CFF0
	protected void RefreshRequirement(List<ValueTuple<int, int, int>> requirements)
	{
		this._avatarInfoMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AvatarInfoMonitor>(this._charId, false);
		CharacterItem config = Character.Instance[this._avatarInfoMonitor.TemplateId];
		this._fixedPower = (config != null && ((this._itemData.Key.ItemType == 0) ? config.FixWeaponPower : config.FixArmorPower) != -1);
		bool fixedPower = this._fixedPower;
		if (!fixedPower)
		{
			byte creatingType = this._avatarInfoMonitor.CreatingType;
			bool isConvertValue = CreatingType.IsNonEvolutionaryType(creatingType);
			TipRequirement requirement = base.CGet<TipRequirement>("Requirement");
			bool showRequirement = requirements != null && requirements.Count > 0;
			requirement.gameObject.SetActive(showRequirement);
			bool flag = showRequirement;
			if (flag)
			{
				requirement.RefreshRequirement(requirements, isConvertValue, "", false, false);
			}
		}
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x0001EEC4 File Offset: 0x0001D0C4
	protected void UpdateCompare()
	{
		bool flag = this.HasStick && !this._isInCompareUI;
		if (!flag)
		{
			bool altDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
			bool ctrlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
			this.UpdateMorInfoAltCtrl(altDown, ctrlDown, this._fixedPower);
			base.CGet<Refers>("Requirement").gameObject.SetActive(altDown && !this._fixedPower);
			base.CGet<TextMeshProUGUI>("CurrAndMaxPower").gameObject.SetActive(altDown && !this._fixedPower);
			base.GetComponent<ContentSizeFitter>().SetLayoutVertical();
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
			bool disableCompare = this.DisableCompare;
			if (!disableCompare)
			{
				this.UpdateCompareCommonPart();
			}
		}
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x0001EFA8 File Offset: 0x0001D1A8
	protected void UpdateCompareCommonPart()
	{
		bool flag = this.DisableCompare || (this.HasStick && !this._isInCompareUI);
		if (!flag)
		{
			bool ctrlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
			bool flag2 = !this._lastCtrlDown && ctrlDown;
			if (flag2)
			{
				this._ctrlDownTime = Time.realtimeSinceStartup;
			}
			bool flag3 = this.NeedShowCompare();
			if (flag3)
			{
				MouseTip_Util.ShowEquipCompare(this.Element, this._itemData, this._charId, this, -1);
			}
			this._lastCtrlDown = ctrlDown;
		}
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x0001F040 File Offset: 0x0001D240
	protected bool NeedShowCompare()
	{
		bool ctrlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		return ctrlDown && Time.realtimeSinceStartup - this._ctrlDownTime >= 0.25f && MouseTip_Util.HasEquipForCompare(this._itemData, this._currentEquipments);
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x0001F098 File Offset: 0x0001D298
	protected void UpdateMorInfoAltCtrl(bool altDown, bool ctrlDown, bool altDisabled = false)
	{
		MoreInfo2 moreInfo = base.CGet<MoreInfo2>("MoreInfo2");
		moreInfo.gameObject.SetActive(true);
		bool flag = (!this.DisableCompare && MouseTip_Util.HasEquipForCompare(this._itemData, this._currentEquipments)) || this._isInCompareUI;
		if (flag)
		{
			if (altDisabled)
			{
				if (ctrlDown)
				{
					moreInfo.RefreshCancelCompare();
				}
				else
				{
					moreInfo.RefreshPressToCompare();
				}
			}
			else
			{
				bool flag2 = altDown && ctrlDown;
				if (flag2)
				{
					moreInfo.RefreshCancelCompare();
				}
				else if (ctrlDown)
				{
					moreInfo.RefreshPressToDetail();
				}
				else if (altDown)
				{
					moreInfo.RefreshCancelDetail();
				}
				else
				{
					moreInfo.RefreshPressToDetailAndCompare();
				}
			}
		}
		else if (altDisabled)
		{
			moreInfo.gameObject.SetActive(false);
		}
		else if (altDown)
		{
			moreInfo.RefreshCancelDetail();
		}
		else
		{
			moreInfo.RefreshPressToDetail();
		}
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x0001F184 File Offset: 0x0001D384
	protected void UpdateMoreInfoCtrl()
	{
		MoreInfo2 moreInfo = base.CGet<MoreInfo2>("MoreInfo2");
		bool flag = this.DisableCompare || !MouseTip_Util.HasEquipForCompare(this._itemData, this._currentEquipments);
		if (flag)
		{
			moreInfo.gameObject.SetActive(false);
		}
		else
		{
			moreInfo.gameObject.SetActive(true);
			bool isInCompareUI = this._isInCompareUI;
			if (isInCompareUI)
			{
				moreInfo.RefreshCancelCompare();
			}
			else
			{
				moreInfo.RefreshPressToCompare();
			}
		}
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x0001F1FC File Offset: 0x0001D3FC
	protected void ForceRebuildLayout(uint delayFrame = 2U, RectTransform rectTransform = null)
	{
		bool destroyed = this._destroyed;
		if (!destroyed)
		{
			bool flag = rectTransform == null;
			if (flag)
			{
				rectTransform = base.GetComponent<RectTransform>();
			}
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(delayFrame, delegate
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
			});
		}
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x0001F254 File Offset: 0x0001D454
	protected void RefreshHotkeyDisplayLockItem()
	{
		bool flag = !this.hotkeyDisplayLockItem;
		if (!flag)
		{
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			bool isShow = this._itemData != null && this._itemData.OwnerCharId == taiwuCharId && this._itemData.ItemSourceTypeEnum == ItemSourceType.Inventory && ViewCharacterMenuItems.CurrItemOperation == ItemOperationType.EItemOperationType.Invalid && ItemTemplateHelper.IsTransferable(this._itemData.RealKey.ItemType, this._itemData.RealKey.TemplateId) && UIManager.Instance.IsFocusElement(UIElement.CharacterMenu) && ViewCharacterMenu.CurSubToggleIndex == ECharacterSubToggleBase.ItemBase && !UIElement.CharacterMenu.UiBaseAs<ViewCharacterMenu>().StackView.isActiveAndEnabled;
			this.hotkeyDisplayLockItem.gameObject.SetActive(isShow);
			bool flag2 = isShow;
			if (flag2)
			{
				this.hotkeyDisplayLockItem.Refresh(EHotKeyDisplayType.LockItem);
			}
		}
	}

	// Token: 0x040002E4 RID: 740
	protected bool _isInCompareUI = false;

	// Token: 0x040002E5 RID: 741
	protected ItemKey _itemKey;

	// Token: 0x040002E6 RID: 742
	protected ItemDisplayData _itemData;

	// Token: 0x040002E7 RID: 743
	protected int _charId;

	// Token: 0x040002E8 RID: 744
	private bool _destroyed;

	// Token: 0x040002E9 RID: 745
	private List<GameObject> _equipmentEffectList = new List<GameObject>();

	// Token: 0x040002EA RID: 746
	private List<ItemKey> _currentEquipments = null;

	// Token: 0x040002EB RID: 747
	protected bool DisableCompare = false;

	// Token: 0x040002EC RID: 748
	private bool _isNew;

	// Token: 0x040002ED RID: 749
	protected readonly List<MouseTipItem.ItemFunction> _disableFunctionList = new List<MouseTipItem.ItemFunction>();

	// Token: 0x040002EE RID: 750
	private AvatarInfoMonitor _avatarInfoMonitor;

	// Token: 0x040002EF RID: 751
	private bool _fixedPower;

	// Token: 0x040002F0 RID: 752
	private bool _lastCtrlDown = false;

	// Token: 0x040002F1 RID: 753
	private float _ctrlDownTime = 0f;

	// Token: 0x040002F2 RID: 754
	[SerializeField]
	private CommonHotkeyDisplayNew hotkeyDisplayLockItem;

	// Token: 0x020010F9 RID: 4345
	protected enum ItemFunction
	{
		// Token: 0x04009506 RID: 38150
		Repairable,
		// Token: 0x04009507 RID: 38151
		Transferable,
		// Token: 0x04009508 RID: 38152
		Poisonable,
		// Token: 0x04009509 RID: 38153
		Refinable,
		// Token: 0x0400950A RID: 38154
		Disassemble,
		// Token: 0x0400950B RID: 38155
		CanChangeTrick
	}
}
