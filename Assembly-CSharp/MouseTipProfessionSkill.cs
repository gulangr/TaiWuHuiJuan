using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CharacterDataMonitor;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.Taiwu.Profession.SkillsData;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020002C8 RID: 712
public class MouseTipProfessionSkill : MouseTipBase
{
	// Token: 0x170004B4 RID: 1204
	// (get) Token: 0x06002B05 RID: 11013 RVA: 0x0014BF04 File Offset: 0x0014A104
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002B06 RID: 11014 RVA: 0x0014BF08 File Offset: 0x0014A108
	protected override void Init(ArgumentBox argsBox)
	{
		bool isLocked;
		argsBox.Get("IsLocked", out isLocked);
		int skillId;
		argsBox.Get("ProfessionSkillId", out skillId);
		this._skillId = skillId;
		ProfessionData professionData;
		argsBox.Get<ProfessionData>("ProfessionData", out professionData);
		this._professionData = professionData;
		int skillIndex;
		argsBox.Get("SkillIndex", out skillIndex);
		int exp;
		argsBox.Get("Exp", out exp);
		ResourceInts resources;
		argsBox.Get<ResourceInts>("Resources", out resources);
		bool disableAdditionalRedText;
		argsBox.Get("DisableAdditionalRedText", out disableAdditionalRedText);
		bool coolDownIsMeet = true;
		ProfessionSkillItem skillConfig = ProfessionSkill.Instance[skillId];
		base.CGet<TextMeshProUGUI>("Title").text = skillConfig.Name;
		base.CGet<TextMeshProUGUI>("Desc").text = ProfessionSkill.Instance[skillId].Desc.ColorReplace();
		base.CGet<TextMeshProUGUI>("FuncDesc").text = ProfessionSkill.Instance[skillId].FunctionalDesc.ColorReplace();
		base.CGet<TextMeshProUGUI>("FuncDesc").transform.GetComponent<TMPTextSpriteHelper>().Parse();
		bool isNotPassive = skillConfig.Type != EProfessionSkillType.Passive;
		base.CGet<GameObject>("CoolDownLayout").SetActive(isNotPassive);
		base.CGet<GameObject>("CostLayout").SetActive(isNotPassive && !isLocked);
		TextMeshProUGUI otherText = base.CGet<TextMeshProUGUI>("Other");
		otherText.gameObject.SetActive(false);
		TextMeshProUGUI lackText = base.CGet<TextMeshProUGUI>("Lack");
		lackText.gameObject.SetActive(false);
		bool flag = isLocked;
		if (flag)
		{
			bool isNotPassive3 = isNotPassive;
			if (isNotPassive3)
			{
				base.CGet<TextMeshProUGUI>("CoolDown").text = LocalStringManager.GetFormat(LanguageKey.LK_ProfessionSkill_CoolDown, skillConfig.SkillCoolDown).ColorReplace();
			}
			base.CGet<GameObject>("SkillEffectLayout").SetActive(false);
			base.CGet<GameObject>("NoMeetLayout").SetActive(false);
			base.CGet<GameObject>("CurrentBonusLayout").SetActive(false);
		}
		else
		{
			bool isNotPassive2 = isNotPassive;
			if (isNotPassive2)
			{
				int coolDown = professionData.SkillOffCooldownDates[skillIndex];
				int currDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
				coolDownIsMeet = (currDate >= coolDown);
				base.CGet<TextMeshProUGUI>("CoolDown").text = LocalStringManager.GetFormat(LanguageKey.LK_ProfessionSkill_CoolDown, skillConfig.SkillCoolDown).ColorReplace();
				bool showExp = skillConfig.ExpCost > 0;
				base.CGet<GameObject>("Exp").SetActive(showExp);
				bool flag2 = showExp;
				if (flag2)
				{
					TextMeshProUGUI expCost = base.CGet<TextMeshProUGUI>("ExpCost");
					string expColor = (exp > skillConfig.ExpCost) ? "brightblue" : "brightred";
					expCost.text = LocalStringManager.GetFormat(LanguageKey.LK_ExpCost, CommonUtils.GetDisplayStringForNum(exp, 100000).SetColor(expColor), skillConfig.ExpCost).ColorReplace();
				}
				bool showTime = skillConfig.TimeCost > 0;
				base.CGet<GameObject>("Time").SetActive(showTime);
				bool flag3 = showTime;
				if (flag3)
				{
					int remainTime = SingletonObject.getInstance<TimeManager>().GetRemainingActionPointConvertToDays();
					string timeColor = (remainTime >= (int)skillConfig.TimeCost) ? "brightblue" : "brightred";
					base.CGet<TextMeshProUGUI>("TimeCost").text = LocalStringManager.GetFormat(LanguageKey.LK_TimeCost, remainTime.ToString().SetColor(timeColor), skillConfig.TimeCost).ColorReplace();
				}
				bool showResources = false;
				GameObject resourceHolder = base.CGet<GameObject>("ResourceHolder");
				int j;
				int i;
				for (i = 0; i < resourceHolder.transform.childCount; i = j + 1)
				{
					Transform child = resourceHolder.transform.GetChild(i);
					ResourceInfo resourceInfo = skillConfig.ResourcesCost.Find((ResourceInfo r) => (int)r.ResourceType == i && r.ResourceCount > 0);
					bool flag4 = (int)resourceInfo.ResourceType == i && resourceInfo.ResourceCount > 0;
					if (flag4)
					{
						showResources = true;
						ResourceTypeItem resourceConfig = Config.ResourceType.Instance[i];
						Refers refers = child.GetComponent<Refers>();
						refers.CGet<CImage>("Icon").SetSprite(resourceConfig.Icon, false, null);
						refers.CGet<TextMeshProUGUI>("Name").SetText(resourceConfig.Name, true);
						bool resourceIsMeet = resources.Get(i) >= resourceInfo.ResourceCount;
						bool flag5 = !resourceIsMeet;
						if (flag5)
						{
						}
						string resourceColor = resourceIsMeet ? "brightblue" : "brightred";
						refers.CGet<TextMeshProUGUI>("Value").text = LocalStringManager.GetFormat(LanguageKey.LK_Make_Resource_Require_Meet, CommonUtils.GetDisplayStringForNum(resources.Get(i), 100000).SetColor(resourceColor), resourceInfo.ResourceCount).ColorReplace();
						child.gameObject.SetActive(true);
					}
					else
					{
						child.gameObject.SetActive(false);
					}
					j = i;
				}
				base.CGet<GameObject>("ResourceHolder").SetActive(showResources);
				bool noneCost = !showTime && !showExp && !showResources;
				base.CGet<GameObject>("CostLayout").SetActive(!noneCost);
			}
			MapBlockData currentBlockData = SingletonObject.getInstance<WorldMapModel>().CurrentBlockData;
			bool flag6 = currentBlockData != null && WorldMapModel.IsSettlementBlock(currentBlockData.GetConfig());
			base.CGet<GameObject>("CurrentBonusLayout").SetActive(skillConfig.TemplateId == 2);
			ProfessionSkillStateHelper.AsyncGetSkillUseState(this, skillId, resources, exp, delegate(ProfessionSkillStateHelper.Result result)
			{
				bool disableAdditionalRedText = disableAdditionalRedText;
				if (disableAdditionalRedText)
				{
					result.ExtraLineStringBuilder.Clear();
				}
				WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
				int templateId = skillConfig.TemplateId;
				int num = templateId;
				if (num <= 23)
				{
					if (num == 2)
					{
						List<int> allStates = EasyPool.Get<List<int>>();
						allStates.Clear();
						foreach (MapBlockItem config in from x in MapBlock.Instance
						where x.CombatState >= 0
						select x)
						{
							bool flag7 = !allStates.Contains((int)config.CombatState);
							if (flag7)
							{
								allStates.Add((int)config.CombatState);
							}
						}
						allStates.Sort();
						MapBlockData playerAtBlock = mapModel.PlayerAtBlock;
						bool flag8 = playerAtBlock != null;
						if (flag8)
						{
							int step = 1;
							byte size = mapModel.GetAreaSize(playerAtBlock.AreaId);
							ByteCoordinate root = playerAtBlock.GetBlockPos();
							List<int> curStates = EasyPool.Get<List<int>>();
							byte x2 = (byte)Mathf.Max((int)root.X - step, 0);
							while ((int)x2 <= Mathf.Min((int)root.X + step, (int)size))
							{
								byte y = (byte)Mathf.Max((int)root.Y - step, 0);
								while ((int)y <= Mathf.Min((int)root.Y + step, (int)size))
								{
									short blockId = WorldMapModel.PositionToIndex(x2, y, size);
									MapBlockData block;
									bool flag9 = !mapModel.TryGetBlockData(new Location(playerAtBlock.AreaId, blockId), out block);
									if (!flag9)
									{
										bool flag10 = (int)block.GetManhattanDistanceToPos(root.X, root.Y) > step;
										if (!flag10)
										{
											short stateId = block.GetConfig().CombatState;
											bool flag11 = stateId >= 0 && !curStates.Contains((int)stateId);
											if (flag11)
											{
												curStates.Add((int)stateId);
											}
										}
									}
									y += 1;
								}
								x2 += 1;
							}
							curStates.Sort();
							GameObject currentBonusHolder = this.CGet<GameObject>("CurrentBonusHolder");
							for (int i = 0; i < allStates.Count; i++)
							{
								CombatStateItem stateConfig = CombatState.Instance[allStates[i]];
								string stateName = stateConfig.Name;
								GameObject stateContent = currentBonusHolder.transform.GetChild(i).GetComponent<Refers>().CGet<GameObject>("Other");
								bool flag12 = curStates.Contains(allStates[i]);
								if (flag12)
								{
									stateContent.GetComponent<TextMeshProUGUI>().text = LocalStringManager.GetFormat(LanguageKey.LK_Item_Operation_LifeSkill_Require_Meet, stateName.SetColor("orange"), stateConfig.Desc).SetColor("pinkyellow");
									stateContent.GetComponent<DisableStyleRoot>().SetStyleEffect(false, false);
								}
								else
								{
									stateContent.GetComponent<TextMeshProUGUI>().text = LocalStringManager.GetFormat(LanguageKey.LK_Item_Operation_LifeSkill_Require_Meet, stateName, stateConfig.Desc.RemoveColorTags()).SetColor("lightgrey");
									stateContent.GetComponent<DisableStyleRoot>().SetStyleEffect(true, false);
								}
							}
						}
						EasyPool.Free<List<int>>(allStates);
						base.<Init>g__RefreshNotMeetText|1(result);
						return;
					}
					if (num == 3)
					{
						MapBlockData blockData;
						bool flag13 = mapModel.TryGetBlockData(mapModel.CurrentLocation, out blockData);
						if (flag13)
						{
							bool flag14 = blockData.GetConfig().SubType == EMapBlockSubType.DLCLoong && SingletonObject.getInstance<DlcManager>().IsDlcInstalled(DlcManager.DlcIdFiveLoong);
							if (flag14)
							{
								result.ExtraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_SavageSkill3_Tip).ColorReplace());
							}
						}
						base.<Init>g__RefreshNotMeetText|1(result);
						return;
					}
					if (num == 23)
					{
						TaoistMonkSkillsData taoistMonkSkillsData = professionData.SkillsData as TaoistMonkSkillsData;
						int diff = (int)(4 - taoistMonkSkillsData.SurvivedTribulationCount);
						bool flag15 = diff == 0;
						if (flag15)
						{
							result.ExtraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_TaoistMonkSkill3_Finished).ColorReplace());
						}
						else
						{
							bool flag16 = diff == 1;
							if (flag16)
							{
								result.ExtraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_TaoistMonkSkill3_WillFinished).ColorReplace());
							}
						}
						base.<Init>g__RefreshNotMeetText|1(result);
						return;
					}
				}
				else if (num <= 33)
				{
					if (num == 25)
					{
						result.ExtraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_LegendaryInfected_BuddhistMonkSkill1_Tip).ColorReplace());
						base.<Init>g__RefreshNotMeetText|1(result);
						return;
					}
					if (num == 33)
					{
						result.ExtraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_LegendaryInfected_AristocratSkill1_Tip).ColorReplace());
						base.<Init>g__RefreshNotMeetText|1(result);
						return;
					}
				}
				else
				{
					if (num == 49)
					{
						result.ExtraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_LegendaryInfected_TravelingBuddhistMonkSkill1_Tip).ColorReplace());
						base.<Init>g__RefreshNotMeetText|1(result);
						return;
					}
					if (num == 50)
					{
						TravelingBuddhistMonkSkillsData travelingBuddhistMonkSkillsData = professionData.GetSkillsData<TravelingBuddhistMonkSkillsData>();
						bool flag17 = !disableAdditionalRedText;
						if (flag17)
						{
							int templeCount = travelingBuddhistMonkSkillsData.GetVisitedTempleCount();
							bool flag18 = templeCount >= 5;
							if (flag18)
							{
								result.ExtraLineStringBuilder.AppendLine(LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_TravelingBuddhistMonkSkill2_Finish).ColorReplace());
								base.<Init>g__RefreshNotMeetText|1(result);
							}
							else
							{
								StringBuilder nameSb = EasyPool.Get<StringBuilder>();
								nameSb.Clear();
								bool flag19 = !coolDownIsMeet;
								if (flag19)
								{
									result.ExtraLineStringBuilder.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_ProfessionSkill_TravelingBuddhistMonkSkill2_NotFinish, templeCount, 5, nameSb.ToString()).ColorReplace());
								}
								EasyPool.Free<StringBuilder>(nameSb);
								base.<Init>g__RefreshNotMeetText|1(result);
							}
						}
						return;
					}
				}
				base.<Init>g__RefreshNotMeetText|1(result);
			});
			this.UpdateSkillEffect();
		}
	}

	// Token: 0x06002B07 RID: 11015 RVA: 0x0014C53C File Offset: 0x0014A73C
	private void UpdateSkillEffect()
	{
		this._skillEffectCount = 0;
		bool isProcessSprite = false;
		List<ValueTuple<string, string, string>> effectList = this.GetProfessionSkillEffect(ref isProcessSprite);
		base.CGet<GameObject>("SkillEffectLayout").SetActive(effectList.Count != 0 || this._skillEffectCount > 0);
		bool flag = effectList.Count == 0;
		if (!flag)
		{
			this._skillEffectCount += effectList.Count;
			GameObject effectHolder = base.CGet<GameObject>("EffectHolder");
			for (int i = 0; i < this._skillEffectCount; i++)
			{
				Transform skillEffectItemClone = effectHolder.transform.GetChild(i);
				bool flag2 = i < effectList.Count;
				if (flag2)
				{
					skillEffectItemClone.gameObject.SetActive(true);
					skillEffectItemClone.GetComponent<Refers>().CGet<CImage>("Icon").transform.gameObject.SetActive(effectList[i].Item1 != "");
					skillEffectItemClone.GetComponent<Refers>().CGet<CImage>("Icon").SetSprite(effectList[i].Item1, false, null);
					skillEffectItemClone.GetComponent<Refers>().CGet<TextMeshProUGUI>("Name").SetText(effectList[i].Item2, true);
					skillEffectItemClone.GetComponent<Refers>().CGet<GameObject>("Colon").SetActive(effectList[i].Item3 != "");
					skillEffectItemClone.GetComponent<Refers>().CGet<TextMeshProUGUI>("Value").SetText(effectList[i].Item3, true);
					bool flag3 = isProcessSprite;
					if (flag3)
					{
						skillEffectItemClone.GetComponent<Refers>().CGet<TMPTextSpriteHelper>("SpriteHelper").Parse();
						isProcessSprite = false;
					}
				}
			}
			for (int j = this._skillEffectCount; j < effectHolder.transform.childCount; j++)
			{
				Transform skillEffectItemClone2 = effectHolder.transform.GetChild(j);
				skillEffectItemClone2.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002B08 RID: 11016 RVA: 0x0014C744 File Offset: 0x0014A944
	private void AddSkillEffectTip(ValueTuple<string, string, string> effectData, bool isProecssSprite = false)
	{
		base.CGet<GameObject>("SkillEffectLayout").SetActive(true);
		GameObject effectHolder = base.CGet<GameObject>("EffectHolder");
		Transform transform = effectHolder.transform;
		int skillEffectCount = this._skillEffectCount;
		this._skillEffectCount = skillEffectCount + 1;
		Transform skillEffectItemClone = transform.GetChild(skillEffectCount);
		skillEffectItemClone.gameObject.SetActive(true);
		skillEffectItemClone.GetComponent<Refers>().CGet<CImage>("Icon").transform.gameObject.SetActive(effectData.Item1 != "");
		skillEffectItemClone.GetComponent<Refers>().CGet<CImage>("Icon").SetSprite(effectData.Item1, false, null);
		skillEffectItemClone.GetComponent<Refers>().CGet<TextMeshProUGUI>("Name").SetText(effectData.Item2, true);
		skillEffectItemClone.GetComponent<Refers>().CGet<GameObject>("Colon").SetActive(effectData.Item3 != "");
		skillEffectItemClone.GetComponent<Refers>().CGet<TextMeshProUGUI>("Value").SetText(effectData.Item3, true);
		if (isProecssSprite)
		{
			skillEffectItemClone.GetComponent<Refers>().CGet<TMPTextSpriteHelper>("SpriteHelper").Parse();
		}
		for (int i = this._skillEffectCount; i < effectHolder.transform.childCount; i++)
		{
			skillEffectItemClone = effectHolder.transform.GetChild(i);
			skillEffectItemClone.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002B09 RID: 11017 RVA: 0x0014C8A8 File Offset: 0x0014AAA8
	public List<ValueTuple<string, string, string>> GetProfessionSkillEffect(ref bool isProecssSprite)
	{
		MouseTipProfessionSkill.<>c__DisplayClass9_0 CS$<>8__locals1 = new MouseTipProfessionSkill.<>c__DisplayClass9_0();
		CS$<>8__locals1.<>4__this = this;
		ProfessionSkillItem skillConfig = ProfessionSkill.Instance[this._skillId];
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		CS$<>8__locals1.professionSkillEffectList = new List<ValueTuple<string, string, string>>();
		MouseTipProfessionSkill.<>c__DisplayClass9_1 CS$<>8__locals2 = new MouseTipProfessionSkill.<>c__DisplayClass9_1();
		CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
		switch (skillConfig.TemplateId)
		{
		case 0:
			this.AddMainAttributeEffectToList(ref CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList, "mousetip_zhuyao_0", LanguageKey.LK_MouseTip_Strength_Regen, LanguageKey.LK_MouseTip_Strength_Recovery, 0);
			goto IL_1997;
		case 1:
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_resource", LocalStringManager.Get(LanguageKey.LK_MouseTip_ResourceRecovery), (this._professionData.GetSeniorityResourceRecoveryFactor().ToString() + "%").SetColor("pinkyellow")));
			goto IL_1997;
		case 4:
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_propertypromotion", LocalStringManager.Get(LanguageKey.LK_MouseTip_PropertyPromotion), ("+" + this._professionData.GetSeniorityHunterAnimalBonus().ToString() + "%").SetColor("brightblue")));
			goto IL_1997;
		case 5:
		{
			int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			CharacterDomainMethod.AsyncCall.GetAllEquipmentItems(this, taiwuId, delegate(int offset, RawDataPool dataPool)
			{
				List<ItemDisplayData> equipments = EasyPool.Get<List<ItemDisplayData>>();
				Serializer.Deserialize(dataPool, offset, ref equipments);
				ItemDisplayData beastItem = equipments[13];
				bool flag12 = beastItem.Key.IsValid() && Carrier.Instance[beastItem.Key.TemplateId].CharacterIdInCombat >= 0;
				if (flag12)
				{
					CS$<>8__locals2.CS$<>8__locals1.<>4__this.AddSkillEffectTip(new ValueTuple<string, string, string>("mousetip_beastgrade", LocalStringManager.Get(LanguageKey.LK_MouseTip_CurrentBeast), Carrier.Instance[beastItem.Key.TemplateId].Name.SetColor("pinkyellow")), false);
					string durabilityColor = (beastItem.Durability < 30) ? "brightred" : "brightblue";
					string durabilityText = LocalStringManager.GetFormat(LanguageKey.LK_Profession_Skill_Hunter_1_Durability, beastItem.Durability.ToString().SetColor(durabilityColor), "30".SetColor("pinkyellow"));
					CS$<>8__locals2.CS$<>8__locals1.<>4__this.AddSkillEffectTip(new ValueTuple<string, string, string>("mousetip_beastgrade", durabilityText.ColorReplace(), ""), false);
					bool flag13 = beastItem.Durability < 30;
					if (flag13)
					{
						CS$<>8__locals2.CS$<>8__locals1.<>4__this.AddSkillEffectTip(new ValueTuple<string, string, string>("", LocalStringManager.Get(LanguageKey.LK_Combat_Carrier_Attack_Tips_Durability).ColorReplace(), ""), false);
					}
				}
				else
				{
					CS$<>8__locals2.CS$<>8__locals1.<>4__this.AddSkillEffectTip(new ValueTuple<string, string, string>("mousetip_beastgrade", LocalStringManager.Get(LanguageKey.LK_MouseTip_CurrentBeast), LocalStringManager.Get(LanguageKey.LK_NewGame_Empty)), false);
				}
				EasyPool.Free<List<ItemDisplayData>>(equipments);
			});
			goto IL_1997;
		}
		case 6:
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_beastgrade", LocalStringManager.Get(LanguageKey.LK_MouseTip_BeastGrade), CommonUtils.GetProfessionAnimalGradeName(this._professionData)));
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_beastgrade", LocalStringManager.Get(LanguageKey.LK_MouseTip_ProfessionHunterFindAnimalCount), this._professionData.GetSeniorityAnimalCount().ToString().SetColor("brightblue")));
			goto IL_1997;
		case 7:
			goto IL_1997;
		case 8:
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_lifeskillcircular", LocalStringManager.Get(LanguageKey.LK_MouseTip_None_Tool_Attainment), ((100 + this._professionData.GetSeniorityEmptyToolAttainmentBonus()).ToString() + "%").SetColor("brightblue")));
			goto IL_1997;
		case 9:
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_lifeskillcircular", LocalStringManager.Get(LanguageKey.LK_MouseTip_Attainment_Bonus), ("+" + this._professionData.GetSeniorityAttainmentBonus().ToString() + "%").SetColor("brightblue")));
			goto IL_1997;
		case 10:
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_resource", LocalStringManager.Get(LanguageKey.LK_MouseTip_Resource_Reduce), (this._professionData.GetSeniorityChangeWeaponTrickCostResourceReduceRate().ToString() + "%").SetColor("brightblue")));
			goto IL_1997;
		case 11:
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_refine", LocalStringManager.Get(LanguageKey.LK_MouseTip_Effect_Ratio), (150.ToString() + "%").SetColor("brightblue")));
			goto IL_1997;
		case 12:
		{
			int loveEffect = (int)this._professionData.GetSeniorityFavorAddPercent();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_multilove", LocalStringManager.Get(LanguageKey.LK_MouseTip_Favorability_Promotion), ("+" + loveEffect.ToString() + "%").SetColor("brightblue")));
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_multilove", LocalStringManager.Get(LanguageKey.LK_InitialFavorabilitiesAdd), ("+" + GlobalConfig.Instance.ProfessionInitialFavorabilitiesImprovePercent.ToString() + "%").SetColor("brightblue")));
			int grade = (int)this._professionData.GetSeniorityGiftLevelReduce();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_gift", LocalStringManager.Get(LanguageKey.LK_MouseTip_Gift_Grade), ("+" + grade.ToString()).SetColor("brightblue")));
			goto IL_1997;
		}
		case 13:
		{
			int loopEffect = 50 + 50 * this._professionData.GetSeniorityPercent() / 100;
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_qibuff", LocalStringManager.Get(LanguageKey.LK_MouseTip_Loop_Bonus), ("+" + loopEffect.ToString() + "%").SetColor("brightblue")));
			int strategiesEffect = ProfessionData.SeniorityToExtraReadingLoopingStrategyCount(this._professionData.Seniority);
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_additionalstrategies", LocalStringManager.Get(LanguageKey.LK_MouseTip_Extra_Strategy), ("+" + strategiesEffect.ToString()).SetColor("brightblue")));
			goto IL_1997;
		}
		case 14:
		{
			int percent = this._professionData.GetSeniorityPercent();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_rewardlist", LocalStringManager.Get(LanguageKey.LK_MouseTip_Bounty_Reward), ("+" + percent.ToString() + "%").SetColor("brightblue")));
			goto IL_1997;
		}
		case 15:
			ExtraDomainMethod.AsyncCall.GetMartialArtistCreateGoodRandomEnemyAndBadRandomEnemyCount(this, delegate(int offset, RawDataPool dataPool)
			{
				IntPair intPair = default(IntPair);
				Serializer.Deserialize(dataPool, offset, ref intPair);
				CS$<>8__locals2.CS$<>8__locals1.<>4__this.AddSkillEffectTip(new ValueTuple<string, string, string>("mousetip_chivalrousperson", LocalStringManager.Get(LanguageKey.LK_MouseTip_GoodEnemy_Count).ColorReplace(), intPair.First.ToString().SetColor("pinkyellow")), false);
				CS$<>8__locals2.CS$<>8__locals1.<>4__this.AddSkillEffectTip(new ValueTuple<string, string, string>("mousetip_heterodoxy", LocalStringManager.Get(LanguageKey.LK_MouseTip_BadEnemy_Count).ColorReplace(), intPair.Second.ToString().SetColor("pinkyellow")), false);
			});
			goto IL_1997;
		case 16:
		{
			int grade = this._professionData.GetSeniorityOrgGrade();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_character", LocalStringManager.Get(LanguageKey.LK_MouseTip_CharacterGrade), LocalStringManager.Get(LanguageKey.LK_OrgGrade_0 + grade).SetGradeColor(grade)));
			goto IL_1997;
		}
		case 17:
		{
			int loopEffect = 30 + 30 * this._professionData.GetSeniorityPercent() / 100;
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_readingefficiency", LocalStringManager.Get(LanguageKey.LK_MouseTip_Read_Bonus), ("+" + loopEffect.ToString() + "%").SetColor("brightblue")));
			int strategiesEffect = ProfessionData.SeniorityToExtraReadingLoopingStrategyCount(this._professionData.Seniority);
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_additionalstrategies", LocalStringManager.Get(LanguageKey.LK_MouseTip_Extra_Strategy), ("+" + strategiesEffect.ToString()).SetColor("brightblue")));
			goto IL_1997;
		}
		case 18:
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_information", LocalStringManager.Get(LanguageKey.LK_MouseTip_SecretInformation_Effect), "+200%".SetColor("brightblue")));
			goto IL_1997;
		case 19:
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_information", LocalStringManager.Get(LanguageKey.LK_MouseTip_NormalInformation_Effect), "+50%".SetColor("brightblue")));
			goto IL_1997;
		case 20:
			this.AddMainAttributeEffectToList(ref CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList, "mousetip_zhuyao_4", LanguageKey.LK_MouseTip_Energy_Regen, LanguageKey.LK_MouseTip_Energy_Recovery, 4);
			goto IL_1997;
		case 21:
		{
			int grade = this._professionData.GetSeniorityOrgGrade();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_character", LocalStringManager.Get(LanguageKey.LK_MouseTip_CharacterGrade), LocalStringManager.Get(LanguageKey.LK_OrgGrade_0 + grade).SetGradeColor(grade)));
			goto IL_1997;
		}
		case 22:
			goto IL_1997;
		case 23:
		{
			isProecssSprite = true;
			CS$<>8__locals2.taoistMonkSkillsData = this._professionData.GetSkillsData<TaoistMonkSkillsData>();
			bool flag = !CS$<>8__locals2.taoistMonkSkillsData.HasSurvivedAllTribulation();
			if (flag)
			{
				CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_talisman", LocalStringManager.Get(LanguageKey.LK_MouseTip_Month_FuLu_Get), (((int)(CS$<>8__locals2.taoistMonkSkillsData.SurvivedTribulationCount * 3)).ToString() + "/").SetColor("pinkyellow") + TMPTextSpriteHelper.GetStringWithTextSpriteTag("mousetip_shijie")));
			}
			ExtraDomainMethod.AsyncCall.GetTianJieFuLuCount(this, delegate(int offset, RawDataPool dataPool)
			{
				int count = 0;
				Serializer.Deserialize(dataPool, offset, ref count);
				Color color = Colors.Instance["brightred"];
				bool flag12 = count >= 99;
				if (flag12)
				{
					color = Colors.Instance["brightblue"];
				}
				string countStr = count.ToString().SetColor(color);
				CS$<>8__locals2.CS$<>8__locals1.<>4__this.AddSkillEffectTip(new ValueTuple<string, string, string>("mousetip_talisman", LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_Tianjiefulu_Count, "").ColorReplace(), countStr), false);
				int nextKey = 223 + (int)CS$<>8__locals2.taoistMonkSkillsData.SurvivedTribulationCount;
				int diff = (int)(4 - CS$<>8__locals2.taoistMonkSkillsData.SurvivedTribulationCount);
				bool flag13 = diff > 1;
				if (flag13)
				{
					CharacterFeatureItem config = CharacterFeature.Instance[nextKey];
					CS$<>8__locals2.CS$<>8__locals1.<>4__this.AddSkillEffectTip(new ValueTuple<string, string, string>("", LocalStringManager.Get(LanguageKey.LK_MouseTip_Next_Tianjie), config.Name.SetGradeColor(8)), false);
				}
				else
				{
					bool flag14 = diff == 1;
					if (flag14)
					{
						CS$<>8__locals2.CS$<>8__locals1.<>4__this.AddSkillEffectTip(new ValueTuple<string, string, string>("", LocalStringManager.Get(LanguageKey.LK_MouseTip_Next_Tianjie), LocalStringManager.Get(LanguageKey.LK_ProfessionSkill_TaoistMonkSkill3_WillFinished).ColorReplace()), false);
					}
				}
			});
			goto IL_1997;
		}
		case 24:
			this.AddMainAttributeEffectToList(ref CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList, "mousetip_zhuyao_2", LanguageKey.LK_MouseTip_Concentration_Regen, LanguageKey.LK_MouseTip_Concentration_Recovery, 2);
			goto IL_1997;
		case 25:
		{
			int grade = this._professionData.GetSeniorityOrgGrade();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_character", LocalStringManager.Get(LanguageKey.LK_MouseTip_CharacterGrade), LocalStringManager.Get(LanguageKey.LK_OrgGrade_0 + grade).SetGradeColor(grade)));
			goto IL_1997;
		}
		case 28:
		{
			int grade = this._professionData.GetSeniorityOrgGrade();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_character", LocalStringManager.Get(LanguageKey.LK_MouseTip_CharacterGrade), LocalStringManager.Get(LanguageKey.LK_OrgGrade_0 + grade).SetGradeColor(grade)));
			goto IL_1997;
		}
		case 30:
		{
			CS$<>8__locals2.monitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EatingItemMonitor>(taiwuCharId, false);
			bool init = CS$<>8__locals2.monitor.Init;
			if (init)
			{
				CS$<>8__locals2.<GetProfessionSkillEffect>g__OnMonitored|2();
			}
			else
			{
				CS$<>8__locals2.monitor.AddEatingItemListener(new Action(CS$<>8__locals2.<GetProfessionSkillEffect>g__OnMonitored|2));
			}
			goto IL_1997;
		}
		case 32:
		{
			int grade = this._professionData.GetSeniorityOrgGrade();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_character", LocalStringManager.Get(LanguageKey.LK_MouseTip_CharacterGrade), LocalStringManager.Get(LanguageKey.LK_OrgGrade_0 + grade).SetGradeColor(grade)));
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_settlementinformation_0", LocalStringManager.Get(LanguageKey.LK_MouseTip_InfluencePower_Change), ("+" + this._professionData.GetInfluencePowerBonusFactor().ToString() + "%").SetColor("brightblue")));
			goto IL_1997;
		}
		case 33:
		{
			sbyte seniorityToGrowingGrade = this._professionData.GetSeniorityGrowingGrade();
			int lowest = (int)(ProfessionRelatedConstants.AristocratGradeRange[0] + seniorityToGrowingGrade);
			int highest = (int)(ProfessionRelatedConstants.AristocratGradeRange[1] + seniorityToGrowingGrade);
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("", LocalStringManager.Get(LanguageKey.LK_MouseTip_Lowest_Qualification), LocalStringManager.Get(LanguageKey.LK_OrgGrade_0 + lowest).SetGradeColor(lowest)));
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("", LocalStringManager.Get(LanguageKey.LK_MouseTip_Highest_Qualification), LocalStringManager.Get(LanguageKey.LK_OrgGrade_0 + highest).SetGradeColor(highest)));
			goto IL_1997;
		}
		case 36:
		{
			sbyte settlementType = this._professionData.GetSeniorityBeggarMaxSettlementType();
			StringBuilder settlementContent = EasyPool.Get<StringBuilder>();
			switch (settlementType)
			{
			case 0:
				settlementContent.Append(LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement_Village).SetGradeColor(0));
				settlementContent.Append(' ');
				break;
			case 1:
				settlementContent.Append(LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement_Village).SetGradeColor(0));
				settlementContent.Append(' ');
				settlementContent.Append(LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement_WalledTown).SetGradeColor(2));
				settlementContent.Append(' ');
				break;
			case 2:
				settlementContent.Append(LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement_Village).SetGradeColor(0));
				settlementContent.Append(' ');
				settlementContent.Append(LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement_WalledTown).SetGradeColor(2));
				settlementContent.Append(' ');
				settlementContent.Append(LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement_Town).SetGradeColor(4));
				break;
			case 3:
				settlementContent.Append(LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement_Village).SetGradeColor(0));
				settlementContent.Append(' ');
				settlementContent.Append(LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement_WalledTown).SetGradeColor(2));
				settlementContent.Append(' ');
				settlementContent.Append(LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement_Town).SetGradeColor(4));
				settlementContent.Append(' ');
				settlementContent.Append(LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement_City).SetGradeColor(5));
				settlementContent.Append(' ');
				settlementContent.Append(LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement_Sect).SetGradeColor(8));
				break;
			}
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_begging", LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement), settlementContent.ToString()));
			EasyPool.Free<StringBuilder>(settlementContent);
			goto IL_1997;
		}
		case 40:
		{
			int loveEffect = (int)this._professionData.GetSeniorityFavorAddPercent();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_multilove", LocalStringManager.Get(LanguageKey.LK_MouseTip_Favorability_Promotion), ("+" + loveEffect.ToString() + "%").SetColor("brightblue")));
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_multilove", LocalStringManager.Get(LanguageKey.LK_InitialFavorabilitiesAdd), ("+" + GlobalConfig.Instance.ProfessionInitialFavorabilitiesImprovePercent.ToString() + "%").SetColor("brightblue")));
			int grade = (int)this._professionData.GetSeniorityGiftLevelReduce();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_gift", LocalStringManager.Get(LanguageKey.LK_MouseTip_Gift_Grade), ("+" + grade.ToString()).SetColor("brightblue")));
			goto IL_1997;
		}
		case 41:
		{
			int removeCount = ProfessionData.GetSeniorityCivilianSeverHatredLimit(this._professionData.Seniority);
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_quenchhatred", LocalStringManager.Get(LanguageKey.LK_MouseTip_Rmove_Amount), removeCount.ToString().SetColor("pinkyellow")));
			goto IL_1997;
		}
		case 42:
		{
			int grade = this._professionData.GetSeniorityOrgGrade();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_character", LocalStringManager.Get(LanguageKey.LK_MouseTip_CharacterGrade), LocalStringManager.Get(LanguageKey.LK_OrgGrade_0 + grade).SetGradeColor(grade)));
			int becomeEnemyCount = ProfessionData.GetSeniorityCivilianAddHatredLimit(this._professionData.Seniority);
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_enmity", LocalStringManager.Get(LanguageKey.LK_MouseTip_Enmity_Amount), becomeEnemyCount.ToString().SetColor("pinkyellow")));
			goto IL_1997;
		}
		case 43:
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_propertypromotion", LocalStringManager.Get(LanguageKey.LK_MouseTip_PropertyPromotion), string.Format("+{0}%", 50).SetColor("brightblue")));
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_meili", LocalStringManager.Get(LanguageKey.LK_Charm), string.Format("+{0}%", 50).SetColor("brightblue")));
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_shouming", LocalStringManager.Get(LanguageKey.LK_Max_Heath), string.Format("+{0}%", 50).SetColor("brightblue")));
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_combatcircular", LocalStringManager.Get(LanguageKey.LK_MouseTip_CombatSkill_Qualification), string.Format("+{0}%", 50).SetColor("brightblue")));
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_lifeskillcircular", LocalStringManager.Get(LanguageKey.LK_MouseTip_LifeSkill_Qualification), string.Format("+{0}%", 50).SetColor("brightblue")));
			goto IL_1997;
		case 44:
			this.AddMainAttributeEffectToList(ref CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList, "mousetip_zhuyao_3", LanguageKey.LK_MouseTip_Vitality_Regen, LanguageKey.LK_MouseTip_Vitality_Recovery, 3);
			goto IL_1997;
		case 45:
		{
			int viewBonus = this._professionData.GetSeniorityVisionRangeBonus();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_view", LocalStringManager.Get(LanguageKey.LK_MouseTip_View_Bonus), ("+" + viewBonus.ToString()).SetColor("brightblue")));
			goto IL_1997;
		}
		case 46:
		{
			int teleportDistance = this._professionData.SeniorityToTeleportDistance();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_movingdistance", LocalStringManager.Get(LanguageKey.LK_MouseTip_Move_Distance), teleportDistance.ToString().SetColor("pinkyellow")));
			goto IL_1997;
		}
		case 47:
		{
			TravelerSkillsData travelerSkillsData = this._professionData.GetSkillsData<TravelerSkillsData>();
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			bool flag2 = travelerSkillsData != null;
			if (flag2)
			{
				for (int i = 0; i < 3; i++)
				{
					TravelerPalaceData palaceData = travelerSkillsData.TryGetPalaceData(i);
					bool flag3 = palaceData != null;
					if (flag3)
					{
						string palaceName = palaceData.CustomName ?? UI_ProfessionTravelerStation.GetPalaceDefaultName(mapModel, palaceData);
						CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_deliverylocation", LocalStringManager.Get(LanguageKey.LK_MouseTip_TravelerStation_Name), palaceName.SetColor("pinkyellow")));
					}
					else
					{
						CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_deliverylocation", LocalStringManager.Get(LanguageKey.LK_MouseTip_TravelerStation_Name), LocalStringManager.Get(LanguageKey.LK_ProfessionTravelerStation_Text5).SetColor("grey")));
					}
				}
			}
			goto IL_1997;
		}
		case 48:
			this.AddMainAttributeEffectToList(ref CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList, "mousetip_zhuyao_5", LanguageKey.LK_MouseTip_Intelligence_Regen, LanguageKey.LK_MouseTip_Intelligence_Recovery, 5);
			goto IL_1997;
		case 49:
		{
			int grade = this._professionData.GetSeniorityOrgGrade();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_character", LocalStringManager.Get(LanguageKey.LK_MouseTip_CharacterGrade), LocalStringManager.Get(LanguageKey.LK_OrgGrade_0 + grade).SetGradeColor(grade)));
			goto IL_1997;
		}
		case 50:
		{
			TravelingBuddhistMonkSkillsData travelingBuddhistMonkSkillsData = this._professionData.GetSkillsData<TravelingBuddhistMonkSkillsData>();
			WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
			for (sbyte stateId = 0; stateId <= 15; stateId += 1)
			{
				bool flag4 = !travelingBuddhistMonkSkillsData.StateHasTemple(stateId);
				if (!flag4)
				{
					Location location = travelingBuddhistMonkSkillsData.GetStateTempleLocation(stateId);
					MapAreaItem mapAreaItem = mapModel.Areas[(int)location.AreaId].GetConfig();
					sbyte lcoalStateId = mapAreaItem.StateID;
					string stateName = MapState.Instance[lcoalStateId].Name;
					string areaName = mapAreaItem.Name;
					string templeName = MapArea.Instance[mapAreaItem.TemplateId].TempleName;
					bool isVisited = travelingBuddhistMonkSkillsData.IsStateTempleVisited(stateId);
					string content = string.Concat(new string[]
					{
						stateName,
						" - ",
						areaName,
						" - ",
						templeName
					}).SetColor("pinkyellow");
					bool flag5 = isVisited;
					if (flag5)
					{
						content += ("(" + LocalStringManager.Get(LanguageKey.LK_MouseTip_Template_Visited) + ")").SetColor("brightblue");
					}
					else
					{
						content = content + "(" + LocalStringManager.Get(LanguageKey.LK_MouseTip_Template_NotVisited).SetColor("brightred") + ")";
					}
					CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("", content, ""));
				}
			}
			goto IL_1997;
		}
		case 51:
			goto IL_1997;
		case 52:
		{
			int grade = this._professionData.GetSeniorityOrgGrade();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_ziyuan_6", LocalStringManager.Get(LanguageKey.LK_MouseTip_Diagnosis_Money), ("+" + this._professionData.GetSeniorityDoctorMedicinePricePercent().ToString() + "%").SetColor("brightblue")));
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_multilove", LocalStringManager.Get(LanguageKey.LK_MouseTip_Diagnosis_Favorability), ("+" + this._professionData.GetSeniorityDoctorFavorAddPercent().ToString() + "%").SetColor("brightblue")));
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_character", LocalStringManager.Get(LanguageKey.LK_MouseTip_CharacterGrade), LocalStringManager.Get(LanguageKey.LK_OrgGrade_0 + grade).SetGradeColor(grade)));
			goto IL_1997;
		}
		case 53:
		{
			sbyte settlementType = this._professionData.GetSeniorityDoctorMaxSettlementType();
			StringBuilder settlementContent = EasyPool.Get<StringBuilder>();
			bool flag6 = settlementType >= 0;
			if (flag6)
			{
				settlementContent.Append(LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement_Village).SetGradeColor(0));
				settlementContent.Append(' ');
			}
			bool flag7 = settlementType >= 1;
			if (flag7)
			{
				settlementContent.Append(LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement_WalledTown).SetGradeColor(2));
				settlementContent.Append(' ');
			}
			bool flag8 = settlementType >= 2;
			if (flag8)
			{
				settlementContent.Append(LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement_Town).SetGradeColor(4));
				settlementContent.Append(' ');
			}
			bool flag9 = settlementType >= 3;
			if (flag9)
			{
				settlementContent.Append(LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement_Sect).SetGradeColor(5));
				settlementContent.Append(' ');
				settlementContent.Append(LocalStringManager.Get(LanguageKey.LK_MouseTip_Begging_Settlement_City).SetGradeColor(8));
				settlementContent.Append(' ');
			}
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_practisemedicine", LocalStringManager.Get(LanguageKey.LK_MouseTip_Doctor_Settlement), settlementContent.ToString()));
			EasyPool.Free<StringBuilder>(settlementContent);
			goto IL_1997;
		}
		case 56:
			this.AddMainAttributeEffectToList(ref CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList, "mousetip_zhuyao_1", LanguageKey.LK_MouseTip_Dexterity_Regen, LanguageKey.LK_MouseTip_Dexterity_Recovery, 1);
			goto IL_1997;
		case 57:
		{
			int grade = this._professionData.GetSeniorityOrgGrade();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_character", LocalStringManager.Get(LanguageKey.LK_MouseTip_CharacterGrade), LocalStringManager.Get(LanguageKey.LK_OrgGrade_0 + grade).SetGradeColor(grade)));
			goto IL_1997;
		}
		case 60:
		{
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_ziyuan_6", LocalStringManager.Get(LanguageKey.LK_MouseTip_Money_Cost), (500.ToString() + "%").SetColor("pinkyellow")));
			int grade = this._professionData.GetSeniorityOrgGrade();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_character", LocalStringManager.Get(LanguageKey.LK_MouseTip_CharacterGrade), LocalStringManager.Get(LanguageKey.LK_OrgGrade_0 + grade).SetGradeColor(grade)));
			goto IL_1997;
		}
		case 61:
		{
			ValueTuple<int, int> valueTuple = this._professionData.SeniorityToCaravanPrice();
			int sell = valueTuple.Item1;
			int buy = valueTuple.Item2;
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_ziyuan_6", LocalStringManager.Get(LanguageKey.LK_MouseTip_Buy_Price), ((100 + buy).ToString() + "%").SetColor("pinkyellow")));
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_ziyuan_6", LocalStringManager.Get(LanguageKey.LK_MouseTip_Sold_Price), ((100 + sell).ToString() + "%").SetColor("pinkyellow")));
			goto IL_1997;
		}
		case 64:
		{
			int grade = this._professionData.GetSeniorityOrgGrade();
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_character", LocalStringManager.Get(LanguageKey.LK_MouseTip_CharacterGrade), LocalStringManager.Get(LanguageKey.LK_OrgGrade_0 + grade).SetGradeColor(grade)));
			goto IL_1997;
		}
		case 66:
		{
			TeaTasterSkillsData teaTasterSkillsData = this._professionData.GetSkillsData<TeaTasterSkillsData>();
			string curActionPointGained = (teaTasterSkillsData.ActionPointGained / 10).ToString().SetColor((teaTasterSkillsData.ActionPointGained == GlobalConfig.Instance.ProfessionSkillRecoverActionPointLimit) ? "brightred" : "brightblue");
			CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList.Add(new ValueTuple<string, string, string>("mousetip_shijian", LocalStringManager.Get(LanguageKey.LK_MouseTip_Month_Recovery), string.Format("{0}/{1}", curActionPointGained, GlobalConfig.Instance.ProfessionSkillRecoverActionPointLimit / 10)));
			goto IL_1997;
		}
		case 69:
		{
			int grade = this._professionData.GetSeniorityOrgGrade();
			this.AddSkillEffectTip(new ValueTuple<string, string, string>("mousetip_character", LocalStringManager.Get(LanguageKey.LK_MouseTip_CharacterGrade), LocalStringManager.Get(LanguageKey.LK_OrgGrade_0 + grade).SetGradeColor(grade)), false);
			CS$<>8__locals2.duke = (DukeSkillsData)this._professionData.SkillsData;
			CS$<>8__locals2.titles = CS$<>8__locals2.duke.GetAllTitles().ToList<short>();
			List<int> charIdList = new List<int>();
			foreach (short templateId in CS$<>8__locals2.titles)
			{
				bool flag10 = CS$<>8__locals2.duke.TitleHasOwner(templateId);
				if (flag10)
				{
					charIdList.Add(CS$<>8__locals2.duke.GetOwnerOfTitle(templateId));
				}
			}
			bool flag11 = charIdList.Count == 0;
			if (flag11)
			{
				CS$<>8__locals2.CS$<>8__locals1.<GetProfessionSkillEffect>g__RefreshAsyncData_DukeSkill1|4(CS$<>8__locals2.titles, CS$<>8__locals2.duke, null);
			}
			else
			{
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataList(this, charIdList, delegate(int offset, RawDataPool dataPool)
				{
					List<CharacterDisplayData> charDataList = new List<CharacterDisplayData>();
					Serializer.Deserialize(dataPool, offset, ref charDataList);
					CS$<>8__locals2.CS$<>8__locals1.<GetProfessionSkillEffect>g__RefreshAsyncData_DukeSkill1|4(CS$<>8__locals2.titles, CS$<>8__locals2.duke, charDataList);
				});
			}
			goto IL_1997;
		}
		}
		return CS$<>8__locals2.CS$<>8__locals1.professionSkillEffectList;
		IL_1997:
		return CS$<>8__locals1.professionSkillEffectList;
	}

	// Token: 0x06002B0A RID: 11018 RVA: 0x0014E268 File Offset: 0x0014C468
	private void AddMainAttributeEffectToList(ref List<ValueTuple<string, string, string>> professionSkillEffectList, string iconName, LanguageKey regenLanguageKey, LanguageKey recoveryLanguageKey, sbyte mainAttributeType)
	{
		professionSkillEffectList.Add(new ValueTuple<string, string, string>(iconName, LocalStringManager.Get(regenLanguageKey), ("+" + this._professionData.GetSeniorityMainAttributeAdditional().ToString()).SetColor("brightblue")));
		professionSkillEffectList.Add(new ValueTuple<string, string, string>(iconName, LocalStringManager.Get(recoveryLanguageKey), ("+" + (this._professionData.GetMainAttributesRecoveryBonusAppliedRate(mainAttributeType, 100) - 100).ToString() + "%").SetColor("brightblue")));
	}

	// Token: 0x06002B0B RID: 11019 RVA: 0x0014E2F9 File Offset: 0x0014C4F9
	protected override void OnDisable()
	{
		base.OnDisable();
		AttributeMonitor attributeMonitor = this._attributeMonitor;
		if (attributeMonitor != null)
		{
			attributeMonitor.RemoveMainAttributeListener(new Action<sbyte>(this.OnMainAttributeChange));
		}
	}

	// Token: 0x06002B0C RID: 11020 RVA: 0x0014E324 File Offset: 0x0014C524
	private void OnMainAttributeChange(sbyte type = 0)
	{
		ProfessionSkillItem skillConfig = ProfessionSkill.Instance[this._skillId];
		short cur = this._attributeMonitor.CurMainAttribute[(int)skillConfig.CharacterProperty];
		short max = this._attributeMonitor.MaxMainAttribute[(int)skillConfig.CharacterProperty];
		string color = (cur >= max) ? "pinkyellow" : "grey";
		string content = cur.ToString().SetColor(color) + "/" + max.ToString();
		Refers propertyRefers = base.CGet<Refers>("Property");
		propertyRefers.CGet<TextMeshProUGUI>("Value").SetText(content, true);
	}

	// Token: 0x04001F18 RID: 7960
	private AttributeMonitor _attributeMonitor;

	// Token: 0x04001F19 RID: 7961
	private int _skillId;

	// Token: 0x04001F1A RID: 7962
	private ProfessionData _professionData;

	// Token: 0x04001F1B RID: 7963
	private int _skillEffectCount;
}
