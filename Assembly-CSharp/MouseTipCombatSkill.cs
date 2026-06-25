using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CharacterDataMonitor;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Creation;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000283 RID: 643
public class MouseTipCombatSkill : MouseTipBase
{
	// Token: 0x17000488 RID: 1160
	// (get) Token: 0x0600295B RID: 10587 RVA: 0x00134EA5 File Offset: 0x001330A5
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600295C RID: 10588 RVA: 0x00134EA8 File Offset: 0x001330A8
	public static string GetPartIcon(sbyte partId)
	{
		return MouseTipCombatSkill.CombatSkillPartIcon.CheckIndex((int)partId) ? MouseTipCombatSkill.CombatSkillPartIcon[(int)partId] : string.Empty;
	}

	// Token: 0x0600295D RID: 10589 RVA: 0x00134EC5 File Offset: 0x001330C5
	protected override void Init(ArgumentBox argsBox)
	{
		this.Refresh(argsBox);
	}

	// Token: 0x0600295E RID: 10590 RVA: 0x00134ED0 File Offset: 0x001330D0
	public override void Refresh(ArgumentBox argsBox)
	{
		argsBox.Get("CombatSkillId", out this._combatSkillTemplateId);
		argsBox.Get("CharId", out this._charId);
		argsBox.Get("ShowOnlyTemplateInfo", out this._showOnlyTemplateInfo);
		argsBox.Get("IsSimple", out this._isSimple);
		this._combatSkillDisplayData = null;
		this._configData = CombatSkill.Instance[this._combatSkillTemplateId];
		this._isTaiwuAttackSkill = (!this._showOnlyTemplateInfo && this._charId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId && this._configData.EquipType == 1);
		this._costTrickPrefab = base.CGet<Refers>("CostTrickPrefab");
		this._equipEffectMergeShowPropertyList.Clear();
		this._castEffectMergeShowPropertyList.Clear();
		this._commonTipRefers = base.CGet<CombatSkillCommonTipRefers>("CombatSkillCommonTipPrefab");
		this._commonTip2ValueRefers = base.CGet<CombatSkillCommonTip2ValueRefers>("CombatSkillInjuryTipPrefab");
		this._accessoryPoisonHolder = base.CGet<GameObject>("AccessoryPoisonHolder");
		this.NeedWaitData = !this._showOnlyTemplateInfo;
		string gradeNum = LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(9 - this._configData.Grade)));
		base.CGet<TextMeshProUGUI>("Name").text = this._configData.Name;
		base.CGet<TextMeshProUGUI>("GradeName").text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", this._configData.Grade));
		base.CGet<TextMeshProUGUI>("Grade").text = (gradeNum + LocalStringManager.Get(LanguageKey.LK_Grade)).SetColor(Colors.Instance.GradeColors[(int)this._configData.Grade]);
		base.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(this._configData.Grade), false, null);
		base.CGet<TextMeshProUGUI>("FiveElementsType").text = LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", this._configData.FiveElements));
		base.CGet<CImage>("FiveElementsIcon").SetSprite(string.Format("mousetip_shuxing_{0}", this._configData.FiveElements), false, null);
		MouseTip_Util.SetMultiLineAutoHeightText(base.CGet<TextMeshProUGUI>("Desc"), this._configData.Desc);
		CombatSkillTypeItem typeConfig = Config.CombatSkillType.Instance[this._configData.Type];
		base.CGet<TextMeshProUGUI>("Type").text = typeConfig.Name;
		base.CGet<CImage>("TypeIcon").SetSprite(typeConfig.TipsIcon, false, null);
		base.CGet<TextMeshProUGUI>("Sect").text = Organization.Instance[this._configData.SectId].Name;
		base.CGet<CImage>("SectIcon").SetSprite(string.Format("mousetip_menpai_{0}", this._configData.SectId), false, null);
		base.CGet<CImage>("SectType").SetSprite(CombatSkillView.EquipTypeImg[this._configData.EquipType], false, null);
		base.CGet<CImage>("SkillType").SetSprite(this._configData.Icon, false, null);
		base.CGet<CImage>("SkillType").SetColor(Colors.Instance.FiveElementsColors[(int)this._configData.FiveElements]);
		base.CGet<GameObject>("DescriptionHolder").SetActive(!this._isSimple);
		base.CGet<GameObject>("PowerAndRangeLayout").SetActive(!this._isSimple);
		base.CGet<GameObject>("RangeBack").SetActive(this._configData.EquipType == 1);
		base.CGet<GameObject>("NeigongProperty").SetActive(this._configData.EquipType == 0);
		base.CGet<GameObject>("Cost").SetActive(this._configData.EquipType != 0 && this._configData.EquipType != 4 && (this._configData.EquipType != 3 || this._configData.BreathStanceTotalCost != 0));
		base.CGet<GameObject>("AttackProperty").SetActive(this._configData.EquipType == 1);
		bool showOnlyTemplateInfo = this._showOnlyTemplateInfo;
		if (showOnlyTemplateInfo)
		{
			base.CGet<GameObject>("CastEffect").SetActive(this._configData.EquipType == 2 || this._configData.EquipType == 3);
		}
		base.CGet<GameObject>("EffectDurationRoot").SetActive(this._configData.EquipType == 3);
		base.CGet<GameObject>("BounceBackRoot").SetActive(false);
		base.CGet<GameObject>("FightBackBackRoot").SetActive(false);
		base.CGet<Transform>("ParticleLayer").gameObject.SetActive(false);
		base.CGet<CRawImage>("ParticleRenderTexture").gameObject.SetActive(false);
		bool flag = this._configData.Type == 0;
		if (flag)
		{
			this.UpdateGridCount(base.CGet<TextMeshProUGUI>("AttackGrid"), this._configData.SpecificGrids[0]);
			this.UpdateGridCount(base.CGet<TextMeshProUGUI>("AgileGrid"), this._configData.SpecificGrids[1]);
			this.UpdateGridCount(base.CGet<TextMeshProUGUI>("DefenceGrid"), this._configData.SpecificGrids[2]);
			this.UpdateGridCount(base.CGet<TextMeshProUGUI>("SpecialGrid"), this._configData.SpecificGrids[3]);
			sbyte destType = this._configData.DestTypeWhileLooping;
			base.CGet<GameObject>("FiveElementsTransfer").SetActive(destType >= 0);
			RectTransform srcBackHolder = base.CGet<RectTransform>("SrcBackHolder");
			RectTransform srcNameHolder = base.CGet<RectTransform>("SrcNameHolder");
			Transform srcEffectHolder = base.CGet<Transform>("SrcEffectHolder");
			RectTransform destBackHolder = base.CGet<RectTransform>("DestBackHolder");
			RectTransform destNameHolder = base.CGet<RectTransform>("DestNameHolder");
			Transform destEffectHolder = base.CGet<Transform>("DestEffectHolder");
			bool flag2 = destType >= 0;
			if (flag2)
			{
				sbyte transferType = this._configData.TransferTypeWhileLooping;
				if (!true)
				{
				}
				sbyte b;
				switch (transferType)
				{
				case 0:
					b = FiveElementsType.Countered[(int)destType];
					break;
				case 1:
					b = FiveElementsType.Countering[(int)destType];
					break;
				case 2:
					b = FiveElementsType.Produced[(int)destType];
					break;
				default:
					b = FiveElementsType.Producing[(int)destType];
					break;
				}
				if (!true)
				{
				}
				sbyte srcType = b;
				sbyte fiveElementsType = 0;
				while ((int)fiveElementsType < srcBackHolder.childCount)
				{
					bool isSrc = fiveElementsType == srcType;
					bool isDest = fiveElementsType == destType;
					srcBackHolder.GetChild((int)fiveElementsType).gameObject.SetActive(isSrc);
					srcNameHolder.GetChild((int)fiveElementsType).gameObject.SetActive(isSrc);
					srcEffectHolder.GetChild((int)fiveElementsType).gameObject.SetActive(isSrc);
					destBackHolder.GetChild((int)fiveElementsType).gameObject.SetActive(isDest);
					destNameHolder.GetChild((int)fiveElementsType).gameObject.SetActive(isDest);
					destEffectHolder.GetChild((int)fiveElementsType).gameObject.SetActive(isDest);
					fiveElementsType += 1;
				}
			}
			bool flag3 = this._configData.Type == 0;
			if (flag3)
			{
				bool flag4 = destType >= 0 && this != null;
				if (flag4)
				{
					Transform particleLayer = base.CGet<Transform>("ParticleLayer");
					CRawImage particleRenderTexture = base.CGet<CRawImage>("ParticleRenderTexture");
					bool flag5 = particleLayer != null && particleRenderTexture != null;
					if (flag5)
					{
						particleLayer.gameObject.SetActive(true);
						particleRenderTexture.gameObject.SetActive(true);
					}
				}
			}
		}
		this.UpdateAttackTypeEffect();
		bool flag6 = this._configData.EquipType == 2;
		if (flag6)
		{
			base.CGet<GameObject>("MoveCdBonusRoot").SetActive(this._configData.MoveCdBonus != 0);
			base.CGet<TextMeshProUGUI>("MoveCdBonus").text = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_MoveCdBonus, (int)(-(int)this._configData.MoveCdBonus));
			base.CGet<GameObject>("CostMobilityPerFrameRoot").SetActive(this._configData.MobilityReduceSpeed != 0);
			base.CGet<TextMeshProUGUI>("CostMobilityPerFrame").text = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_CostMobilityPerFrame, this.FormatCostMobility(this._configData.MobilityReduceSpeed));
			base.CGet<GameObject>("AddMobilityPerFrameRoot").SetActive(this._configData.MobilityAddSpeed != 0);
			base.CGet<TextMeshProUGUI>("AddMobilityPerFrame").text = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_AddMobilityPerFrame, this.FormatCostMobility(this._configData.MobilityAddSpeed));
			base.CGet<GameObject>("CostMobilityByMoveRoot").SetActive(this._configData.MoveCostMobility != 0);
			base.CGet<TextMeshProUGUI>("CostMobilityByMove").text = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_CostMobilityByMove, this.FormatCostMobility(this._configData.MoveCostMobility));
		}
		else
		{
			base.CGet<GameObject>("MoveCdBonusRoot").SetActive(false);
			base.CGet<GameObject>("JumpPrepareFrameRoot").SetActive(false);
			base.CGet<GameObject>("CostMobilityPerFrameRoot").SetActive(false);
			base.CGet<GameObject>("AddMobilityPerFrameRoot").SetActive(false);
			base.CGet<GameObject>("CostMobilityByMoveRoot").SetActive(false);
		}
		bool flag7 = this._configData.EquipType == 3 && this._configData.FightBackDamage > 0;
		if (flag7)
		{
			List<ECharacterPropertyReferencedType> addAvoidPropertyList = new List<ECharacterPropertyReferencedType>();
			bool flag8 = this._configData.AddAvoidOnCast[0] > 0;
			if (flag8)
			{
				addAvoidPropertyList.Add(ECharacterPropertyReferencedType.AvoidRateStrength);
			}
			bool flag9 = this._configData.AddAvoidOnCast[1] > 0;
			if (flag9)
			{
				addAvoidPropertyList.Add(ECharacterPropertyReferencedType.AvoidRateTechnique);
			}
			bool flag10 = this._configData.AddAvoidOnCast[2] > 0;
			if (flag10)
			{
				addAvoidPropertyList.Add(ECharacterPropertyReferencedType.AvoidRateSpeed);
			}
			this._strBuilder.Clear();
			for (int i = 0; i < addAvoidPropertyList.Count; i++)
			{
				bool flag11 = i > 0;
				if (flag11)
				{
					this._strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Split_Symbol));
				}
				this._strBuilder.Append(LocalStringManager.Get(CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[(int)addAvoidPropertyList[i]].DisplayType].Name));
			}
			this._defendSkillFightbackTypeStr = this._strBuilder.ToString();
		}
		this.InitiateCostNeed();
		this.InitiateBodyAndMentalStrong();
		bool flag12 = !this._showOnlyTemplateInfo;
		if (flag12)
		{
			this.Refresh();
		}
		else
		{
			this.HideAllSpecialBack();
			this.UpdateOnlyTemplateData();
		}
		this.RefreshAccessoryPoison();
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x0600295F RID: 10591 RVA: 0x0013598C File Offset: 0x00133B8C
	private void Awake()
	{
		RenderTexture particleRenderTexture = new RenderTexture(150, 50, 0);
		base.CGet<Camera>("ParticleCamera").targetTexture = particleRenderTexture;
		base.CGet<CRawImage>("ParticleRenderTexture").texture = particleRenderTexture;
	}

	// Token: 0x06002960 RID: 10592 RVA: 0x001359CC File Offset: 0x00133BCC
	protected override void OnDisable()
	{
		base.OnDisable();
		EquipmentMonitor equipmentMonitor = this._equipmentMonitor;
		if (equipmentMonitor != null)
		{
			equipmentMonitor.RemoveEquipmentChangeListener(new Action<sbyte>(this.OnEquipmentChanged));
		}
	}

	// Token: 0x06002961 RID: 10593 RVA: 0x001359F4 File Offset: 0x00133BF4
	public override void Refresh()
	{
		bool showOnlyTemplateInfo = this._showOnlyTemplateInfo;
		if (!showOnlyTemplateInfo)
		{
			CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayDataOnce(this, this._charId, this._combatSkillTemplateId, new AsyncMethodCallbackDelegate(this.HandlerMethodGetCombatSkillDisplayDataOnce));
		}
	}

	// Token: 0x06002962 RID: 10594 RVA: 0x00135A30 File Offset: 0x00133C30
	private void HandlerMethodGetCombatSkillDisplayDataOnce(int offset, RawDataPool pool)
	{
		bool flag = this == null || this._configData == null;
		if (!flag)
		{
			Serializer.Deserialize(pool, offset, ref this._combatSkillDisplayData);
			this.RefreshCombatSkillPanel();
		}
	}

	// Token: 0x06002963 RID: 10595 RVA: 0x00135A70 File Offset: 0x00133C70
	private void RefreshCombatSkillPanel()
	{
		bool broken = this._combatSkillDisplayData.EffectType != -1;
		short power = this.GetDisplayDataPower(this._combatSkillDisplayData);
		string powerStr = (power >= 0) ? string.Format("{0}%", power) : "-";
		this._strBuilder.Clear();
		this._strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_CombatSkill_Power));
		this._strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Colon_Symbol));
		this._strBuilder.Append(powerStr);
		base.CGet<TextMeshProUGUI>("Power").text = this._strBuilder.ToString();
		base.CGet<GameObject>("PowerSpecialBack").SetActive(this.GetPropertyIsInSpecialBreaks(0, false));
		base.CGet<GameObject>("Warning").SetActive(this._combatSkillDisplayData.Revoked);
		bool revoked = this._combatSkillDisplayData.Revoked;
		if (revoked)
		{
			base.CGet<GameObject>("Revoke").SetActive(this._combatSkillDisplayData.Revoked);
		}
		bool flag = this._configData.EquipType == 0;
		if (flag)
		{
			this.UpdateGridCount(base.CGet<TextMeshProUGUI>("AttackGrid"), this._combatSkillDisplayData.SpecificGrids[0]);
			this.UpdateGridCount(base.CGet<TextMeshProUGUI>("AgileGrid"), this._combatSkillDisplayData.SpecificGrids[1]);
			this.UpdateGridCount(base.CGet<TextMeshProUGUI>("DefenceGrid"), this._combatSkillDisplayData.SpecificGrids[2]);
			this.UpdateGridCount(base.CGet<TextMeshProUGUI>("SpecialGrid"), this._combatSkillDisplayData.SpecificGrids[3]);
			this.UpdateGridCount(base.CGet<TextMeshProUGUI>("GenericGrid"), this._combatSkillDisplayData.GenericGrid);
			this.UpdateGridCountBack(base.CGet<GameObject>("AttackSpecialBack"), 49);
			this.UpdateGridCountBack(base.CGet<GameObject>("AgileSpecialBack"), 50);
			this.UpdateGridCountBack(base.CGet<GameObject>("DefenceSpecialBack"), 51);
			this.UpdateGridCountBack(base.CGet<GameObject>("SpecialSpecialBack"), 52);
			this.UpdateGridCountBack(base.CGet<GameObject>("GenericSpecialBack"), 9);
		}
		this.UpdateAttackTypeEffect();
		this.UpdateEquipEffect();
		base.CGet<GameObject>("SpecialEffect").SetActive(true);
		GameObject directTitle = base.CGet<GameObject>("DirectEffectTitle");
		GameObject directDesc = base.CGet<GameObject>("DirectDesc");
		GameObject reverseTitle = base.CGet<GameObject>("ReverseEffectTitle");
		GameObject reverseDesc = base.CGet<GameObject>("ReverseDesc");
		bool flag2 = broken;
		if (flag2)
		{
			bool direct = this._combatSkillDisplayData.EffectType == 0;
			directDesc.SetActive(this._altDownWhenNotSticked || direct);
			directTitle.SetActive(this._altDownWhenNotSticked || direct);
			reverseTitle.SetActive(this._altDownWhenNotSticked || !direct);
			reverseDesc.SetActive(this._altDownWhenNotSticked || !direct);
			directTitle.GetComponent<DisableStyleRoot>().SetStyleEffect(!direct, false);
			directDesc.GetComponent<DisableStyleRoot>().SetStyleEffect(!direct, false);
			reverseTitle.GetComponent<DisableStyleRoot>().SetStyleEffect(direct, false);
			reverseDesc.GetComponent<DisableStyleRoot>().SetStyleEffect(direct, false);
			this.UpdateSpecialEffectText(base.CGet<TextMeshProUGUI>(direct ? "DirectEffectDesc" : "ReverseEffectDesc"), CommonUtils.GetSpecialEffectDesc(this._combatSkillDisplayData));
			this.UpdateSpecialEffectText(base.CGet<TextMeshProUGUI>(direct ? "ReverseEffectDesc" : "DirectEffectDesc"), CommonUtils.GetSpecialEffectDesc(direct ? this._configData.ReverseEffectID : this._configData.DirectEffectID));
		}
		else
		{
			directDesc.SetActive(this._altDownWhenNotSticked);
			directTitle.SetActive(this._altDownWhenNotSticked);
			reverseTitle.SetActive(this._altDownWhenNotSticked);
			reverseDesc.SetActive(this._altDownWhenNotSticked);
			this.UpdateSpecialEffectText(base.CGet<TextMeshProUGUI>("DirectEffectDesc"), CommonUtils.GetSpecialEffectDesc(this._configData.DirectEffectID));
			this.UpdateSpecialEffectText(base.CGet<TextMeshProUGUI>("ReverseEffectDesc"), CommonUtils.GetSpecialEffectDesc(this._configData.ReverseEffectID));
			directTitle.GetComponent<DisableStyleRoot>().SetStyleEffect(true, false);
			directDesc.GetComponent<DisableStyleRoot>().SetStyleEffect(true, false);
			reverseTitle.GetComponent<DisableStyleRoot>().SetStyleEffect(true, false);
			reverseDesc.GetComponent<DisableStyleRoot>().SetStyleEffect(true, false);
		}
		base.CGet<CombatSkillEffectContainer>("CombatEffect").Set(this._combatSkillDisplayData.EffectData);
		this.RefreshRequirement(this._combatSkillDisplayData);
		string maxPowerString = string.Format("{0}{1}%", LocalStringManager.Get(LanguageKey.LK_CombatSkill_Max_Power_Tips), this._combatSkillDisplayData.MaxPower);
		this._strBuilder.Clear();
		this._strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_CombatSkill_Curr_Power_Tips));
		this._strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Colon_Symbol));
		this._strBuilder.Append(this._combatSkillDisplayData.RequirementsPower);
		this._strBuilder.Append('%');
		base.CGet<TextMeshProUGUI>("CurrentPower").text = this._strBuilder.ToString();
		this._strBuilder.Clear();
		this._strBuilder.Append(LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Fix, maxPowerString));
		base.CGet<TextMeshProUGUI>("MaxPower").text = this._strBuilder.ToString();
		base.CGet<GameObject>("MaxPowerSpecialBack").SetActive(this.GetPropertyIsInSpecialBreaks(1, false));
		MultiHorizontalLayoutGroup castAddPropertyHolder = base.CGet<RectTransform>("CastAddPropertyHolder").GetComponent<MultiHorizontalLayoutGroup>();
		castAddPropertyHolder.ResetData();
		this._castEffectMergeShowPropertyList.Clear();
		int startIndex = 0;
		bool flag3 = this._configData.EquipType == 0;
		if (flag3)
		{
			bool gotMaxNeili = this._combatSkillDisplayData.ObtainedNeili == this._combatSkillDisplayData.MaxObtainableNeili;
			TextMeshProUGUI obtainedNeiliBlue = base.CGet<TextMeshProUGUI>("CurrObtainNeiliBlue");
			TextMeshProUGUI obtainedNeiliRed = base.CGet<TextMeshProUGUI>("CurrObtainNeiliRed");
			base.CGet<TextMeshProUGUI>("MaxObtainNeili").text = string.Format("/{0}", this._combatSkillDisplayData.MaxObtainableNeili);
			obtainedNeiliBlue.gameObject.SetActive(!gotMaxNeili);
			obtainedNeiliRed.gameObject.SetActive(gotMaxNeili);
			(gotMaxNeili ? obtainedNeiliRed : obtainedNeiliBlue).text = this._combatSkillDisplayData.ObtainedNeili.ToString();
			base.CGet<GameObject>("FinishedLayout").SetActive(gotMaxNeili);
		}
		else
		{
			bool flag4 = this._configData.EquipType == 1;
			if (flag4)
			{
				bool isMindHit = CombatSkillEquipType.IsMindHitSkill(this._combatSkillTemplateId);
				this._strBuilder.Clear();
				bool flag5 = this._combatSkillDisplayData.AddAttackDistanceBackward >= 0;
				if (flag5)
				{
					this._strBuilder.Append("+");
				}
				this._strBuilder.Append(((float)Math.Min(this._combatSkillDisplayData.AddAttackDistanceBackward, 120) / 10f).ToString("f1"));
				base.CGet<TextMeshProUGUI>("RangeFar").text = this._strBuilder.ToString();
				this._strBuilder.Clear();
				bool flag6 = this._combatSkillDisplayData.AddAttackDistanceForward >= 0;
				if (flag6)
				{
					this._strBuilder.Append("+");
				}
				this._strBuilder.Append(((float)Math.Min(this._combatSkillDisplayData.AddAttackDistanceForward, 120) / 10f).ToString("f1"));
				base.CGet<TextMeshProUGUI>("RangeNear").text = this._strBuilder.ToString();
				base.CGet<GameObject>("RangeBackSpecialBack").SetActive(this.GetPropertyIsInSpecialBreaks(34, false));
				bool showHitSpecialBack = this.GetPropertyIsInSpecialBreaks(30, false) || this.GetPropertyIsInSpecialBreaks(73, false);
				base.CGet<GameObject>("HitRateStrengthSpecialBack").SetActive(showHitSpecialBack);
				base.CGet<GameObject>("HitRateSpeedSpecialBack").SetActive(showHitSpecialBack);
				base.CGet<GameObject>("HitRateTechniqueSpecialBack").SetActive(showHitSpecialBack);
				base.CGet<GameObject>("HitRateTechniqueSpecialBack").SetActive(showHitSpecialBack);
				base.CGet<GameObject>("HitRateMindSpecialBack").SetActive(showHitSpecialBack);
				base.CGet<GameObject>("MindSpecialBack").SetActive(false);
				this.UpdateHitInfo(base.CGet<RectTransform>("SpeedPowerIconHolder"), base.CGet<TextMeshProUGUI>("HitRateSpeed"), isMindHit ? 0 : (this._combatSkillDisplayData.HitDistribution[2] / 10), string.Format("{0}%", 100 + this._combatSkillDisplayData.HitValueSpeed), base.CGet<GameObject>("Speed"));
				base.CGet<GameObject>("SpeedSpecialBack").SetActive(this.GetPropertyIsInSpecialBreaks(33, false));
				this.UpdateHitInfo(base.CGet<RectTransform>("TechniquePowerIconHolder"), base.CGet<TextMeshProUGUI>("HitRateTechnique"), isMindHit ? 0 : (this._combatSkillDisplayData.HitDistribution[1] / 10), string.Format("{0}%", 100 + this._combatSkillDisplayData.HitValueTechnique), base.CGet<GameObject>("Technique"));
				base.CGet<GameObject>("TechniqueSpecialBack").SetActive(this.GetPropertyIsInSpecialBreaks(32, false));
				this.UpdateHitInfo(base.CGet<RectTransform>("StrengthPowerIconHolder"), base.CGet<TextMeshProUGUI>("HitRateStrength"), isMindHit ? 0 : (this._combatSkillDisplayData.HitDistribution[0] / 10), string.Format("{0}%", 100 + this._combatSkillDisplayData.HitValueStrength), base.CGet<GameObject>("Strength"));
				base.CGet<GameObject>("StrengthSpecialBack").SetActive(this.GetPropertyIsInSpecialBreaks(31, false));
				this.UpdateHitInfo(base.CGet<RectTransform>("MindPowerIconHolder"), base.CGet<TextMeshProUGUI>("HitRateMind"), isMindHit ? 10 : 0, 100 + this._combatSkillDisplayData.HitValueMind, base.CGet<GameObject>("Mind"));
				base.CGet<TextMeshProUGUI>("OuterPenetrate").text = string.Format("{0}%", 100 + this._combatSkillDisplayData.PenetrateValueOuter);
				base.CGet<TextMeshProUGUI>("InnerPenetrate").text = string.Format("{0}%", 100 + this._combatSkillDisplayData.PenetrateValueInner);
				bool showPenetrateSpecialBack = this.GetPropertyIsInSpecialBreaks(29, false) || this.GetPropertyIsInSpecialBreaks(72, false);
				base.CGet<GameObject>("PenetrateSpecialBack").SetActive(showPenetrateSpecialBack);
			}
			else
			{
				bool flag7 = this._configData.EquipType == 2;
				if (flag7)
				{
					string jumpSpeedText = this.FormatJumpSpeed(this._configData.JumpPrepareFrame, this._combatSkillDisplayData.JumpSpeed);
					base.CGet<GameObject>("JumpPrepareFrameRoot").SetActive(!string.IsNullOrEmpty(jumpSpeedText));
					base.CGet<TextMeshProUGUI>("JumpPrepareFrame").text = jumpSpeedText;
					int index = 0;
					short[] castAddHit = this._configData.AddHitOnCast;
					List<short> comapreList = new List<short>
					{
						11,
						14,
						13,
						12,
						15
					};
					comapreList = (from propertyId in comapreList
					orderby this.GetPropertyIsInSpecialBreaks(propertyId, false)
					select propertyId).ToList<short>();
					foreach (short propertyId3 in comapreList)
					{
						bool isInBreakBouns = this.GetPropertyIsInSpecialBreaks(propertyId3, false);
						bool flag8 = isInBreakBouns;
						if (flag8)
						{
							this._castEffectMergeShowPropertyList.Add(propertyId3);
						}
						bool flag9 = propertyId3 == 11;
						if (flag9)
						{
							bool flag10 = this._configData.AddMoveSpeedOnCast > 0;
							if (flag10)
							{
								MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 20, (int)this._combatSkillDisplayData.AddMoveSpeed, false, false, true, true, isInBreakBouns);
								index++;
							}
							bool flag11 = this._configData.AddPercentMoveSpeedOnCast > 0;
							if (flag11)
							{
								MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 20, (int)this._combatSkillDisplayData.AddPercentMoveSpeed, false, true, true, true, false);
								index++;
							}
						}
						else
						{
							int hitType = (int)(propertyId3 - 12);
							bool flag12 = hitType == 2;
							if (flag12)
							{
								bool flag13 = castAddHit[hitType] > 0;
								if (flag13)
								{
									MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 8, this._combatSkillDisplayData.AddHitSpeed, false, false, true, true, isInBreakBouns);
									index++;
								}
							}
							else
							{
								bool flag14 = hitType == 1;
								if (flag14)
								{
									bool flag15 = castAddHit[hitType] > 0;
									if (flag15)
									{
										MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 7, this._combatSkillDisplayData.AddHitTechnique, false, false, true, true, isInBreakBouns);
										index++;
									}
								}
								else
								{
									bool flag16 = hitType == 0;
									if (flag16)
									{
										bool flag17 = castAddHit[hitType] > 0;
										if (flag17)
										{
											MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 6, this._combatSkillDisplayData.AddHitStrength, false, false, true, true, isInBreakBouns);
											index++;
										}
									}
									else
									{
										bool flag18 = hitType == 3;
										if (flag18)
										{
											bool flag19 = castAddHit[hitType] > 0;
											if (flag19)
											{
												MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 9, this._combatSkillDisplayData.AddHitMind, false, false, true, true, isInBreakBouns);
												index++;
											}
										}
									}
								}
							}
						}
					}
					startIndex = index;
				}
				else
				{
					bool flag20 = this._configData.EquipType == 3;
					if (flag20)
					{
						short[] castAddAvoid = this._configData.AddAvoidOnCast;
						int index2 = 0;
						List<short> comapreList2 = new List<short>
						{
							22,
							21,
							20,
							23,
							18,
							19
						};
						comapreList2 = (from propertyId in comapreList2
						orderby this.GetPropertyIsInSpecialBreaks(propertyId, false)
						select propertyId).ToList<short>();
						foreach (short propertyId2 in comapreList2)
						{
							bool isInBreakBouns2 = this.GetPropertyIsInSpecialBreaks(propertyId2, false);
							bool flag21 = isInBreakBouns2;
							if (flag21)
							{
								this._castEffectMergeShowPropertyList.Add(propertyId2);
							}
							bool flag22 = propertyId2 == 18 || propertyId2 == 19;
							if (flag22)
							{
								bool flag23 = propertyId2 == 18 && this._configData.AddOuterPenetrateResistOnCast > 0;
								if (flag23)
								{
									MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index2, 16, this._combatSkillDisplayData.AddOuterDef, false, false, true, true, isInBreakBouns2);
									index2++;
								}
								bool flag24 = propertyId2 == 19 && this._configData.AddInnerPenetrateResistOnCast > 0;
								if (flag24)
								{
									MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index2, 17, this._combatSkillDisplayData.AddInnerDef, false, false, true, true, isInBreakBouns2);
									index2++;
								}
							}
							else
							{
								int hitType2 = (int)(propertyId2 - 20);
								bool flag25 = hitType2 == 2;
								if (flag25)
								{
									bool flag26 = castAddAvoid[hitType2] > 0;
									if (flag26)
									{
										MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index2, 14, this._combatSkillDisplayData.AddAvoidSpeed, false, false, true, true, isInBreakBouns2);
										index2++;
									}
								}
								else
								{
									bool flag27 = hitType2 == 1;
									if (flag27)
									{
										bool flag28 = castAddAvoid[hitType2] > 0;
										if (flag28)
										{
											MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index2, 13, this._combatSkillDisplayData.AddAvoidTechnique, false, false, true, true, isInBreakBouns2);
											index2++;
										}
									}
									else
									{
										bool flag29 = hitType2 == 0;
										if (flag29)
										{
											bool flag30 = castAddAvoid[hitType2] > 0;
											if (flag30)
											{
												MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index2, 12, this._combatSkillDisplayData.AddAvoidStrength, false, false, true, true, isInBreakBouns2);
												index2++;
											}
										}
										else
										{
											bool flag31 = hitType2 == 3;
											if (flag31)
											{
												bool flag32 = castAddAvoid[hitType2] > 0;
												if (flag32)
												{
													MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index2, 15, this._combatSkillDisplayData.AddAvoidMind, false, false, true, true, isInBreakBouns2);
													index2++;
												}
											}
										}
									}
								}
							}
						}
						startIndex = index2;
						this.RefreshBounce(this._combatSkillDisplayData.BounceDistance, this._combatSkillDisplayData.BouncePowerOuter, this._combatSkillDisplayData.BouncePowerInner, true);
						this.RefreshFightBack(this._combatSkillDisplayData.FightbackPower, true);
						this.RefreshEffectDuration(this._combatSkillDisplayData.EffectDuration, true);
					}
				}
			}
		}
		bool flag33 = this._configData.EquipType == 1 || this._configData.EquipType == 2 || this._configData.EquipType == 3;
		if (flag33)
		{
			base.CGet<GameObject>("CostMobility").SetActive(this._combatSkillDisplayData.CostMobility > 0);
			bool flag34 = this._combatSkillDisplayData.CostMobility > 0;
			if (flag34)
			{
				this.UpdateCostText(base.CGet<TextMeshProUGUI>("MobilityCostNormal"), base.CGet<TextMeshProUGUI>("MobilityCostEnough"), base.CGet<TextMeshProUGUI>("MobilityCostNotEnough"), this._combatSkillDisplayData.CostMobilityFontType, string.Format("{0}%", this._combatSkillDisplayData.CostMobility));
			}
			base.CGet<GameObject>("CostMobilitySpecialBack").SetActive(this.GetPropertyIsInSpecialBreaks(10, false));
			List<NeedTrick> costTrickList = this._combatSkillDisplayData.CostTricks;
			RectTransform costTrickHolder = base.CGet<RectTransform>("CostTrickHolder");
			base.CGet<GameObject>("CostTrick").SetActive(costTrickList != null && costTrickList.Count > 0);
			bool showSpecialBack = false;
			bool flag35 = costTrickList != null && costTrickList.Count > 0;
			if (flag35)
			{
				for (int i = 0; i < costTrickList.Count; i++)
				{
					NeedTrick needTrick = costTrickList[i];
					bool flag36 = i >= costTrickHolder.childCount;
					if (flag36)
					{
						Object.Instantiate<GameObject>(this._costTrickPrefab.gameObject, costTrickHolder);
					}
					Refers trickRefers = costTrickHolder.GetChild(i).GetComponent<Refers>();
					TrickTypeItem trickConfig = Config.TrickType.Instance[needTrick.TrickType];
					trickRefers.CGet<TextMeshProUGUI>("TrickName").text = trickConfig.Name.SetColor(trickConfig.FontColor);
					string costText = string.Format("×{0}", needTrick.NeedCount);
					this.UpdateCostText(trickRefers.CGet<TextMeshProUGUI>("CountNormal"), trickRefers.CGet<TextMeshProUGUI>("CountEnough"), trickRefers.CGet<TextMeshProUGUI>("CountNotEnough"), this._combatSkillDisplayData.CostTricksFontType[i], costText);
					trickRefers.gameObject.SetActive(true);
					bool propertyIsInSpecialBreaks = this.GetPropertyIsInSpecialBreaks((short)(53 + needTrick.TrickType), false);
					if (propertyIsInSpecialBreaks)
					{
						showSpecialBack = true;
					}
				}
				for (int j = costTrickList.Count; j < costTrickHolder.childCount; j++)
				{
					costTrickHolder.GetChild(j).gameObject.SetActive(false);
				}
			}
			base.CGet<GameObject>("CostTrickSpecialBack").SetActive(showSpecialBack);
			GameObject costBreathStance = base.CGet<GameObject>("CostBreathStance");
			costBreathStance.SetActive(this._configData.EquipType != 2 || this._combatSkillDisplayData.CostBreath > 0 || this._combatSkillDisplayData.CostStance > 0);
			bool activeSelf = costBreathStance.activeSelf;
			if (activeSelf)
			{
				this.UpdateCostText(base.CGet<TextMeshProUGUI>("StanceNormal"), base.CGet<TextMeshProUGUI>("StanceEnough"), base.CGet<TextMeshProUGUI>("StanceNotEnough"), this._combatSkillDisplayData.CostStanceFontType, string.Format("{0}%", this._combatSkillDisplayData.CostStance));
				this.UpdateCostText(base.CGet<TextMeshProUGUI>("BreathNormal"), base.CGet<TextMeshProUGUI>("BreathEnough"), base.CGet<TextMeshProUGUI>("BreathNotEnough"), this._combatSkillDisplayData.CostBreathFontType, string.Format("{0}%", this._combatSkillDisplayData.CostBreath));
				base.CGet<GameObject>("CostBreathStanceSpecialBack").SetActive(this.GetPropertyIsInSpecialBreaks(3, false));
			}
			base.CGet<GameObject>("CostWug").SetActive(this._configData.WugCost > 0);
			this.UpdateCostText(base.CGet<TextMeshProUGUI>("WugCostNormal"), base.CGet<TextMeshProUGUI>("WugCostEnough"), base.CGet<TextMeshProUGUI>("WugCostNotEnough"), this._combatSkillDisplayData.CostWugFontType, this._configData.WugCost.ToString());
			base.CGet<GameObject>("CostWeaponDurability").SetActive(this._configData.WeaponDurableCost > 0);
			this.UpdateCostText(base.CGet<TextMeshProUGUI>("WeaponCostNormal"), base.CGet<TextMeshProUGUI>("WeaponCostEnough"), base.CGet<TextMeshProUGUI>("WeaponCostNotEnough"), this._combatSkillDisplayData.CostWeaponDurabilityFontType, this._configData.WeaponDurableCost.ToString());
			ValueTuple<sbyte, sbyte> costNeiliAllocation = this._combatSkillDisplayData.CostNeiliAllocation;
			base.CGet<GameObject>("CostNeiliAllocation").SetActive(costNeiliAllocation.Item1 >= 0);
			bool flag37 = costNeiliAllocation.Item1 >= 0;
			if (flag37)
			{
				base.CGet<CImage>("CostNeiliAllocationIcon").SetSprite(string.Format("mousetip_gongfalan_{0}", costNeiliAllocation.Item1), false, null);
				this.UpdateCostText(base.CGet<TextMeshProUGUI>("CostNeiliAllocationNormal"), base.CGet<TextMeshProUGUI>("CostNeiliAllocationEnough"), base.CGet<TextMeshProUGUI>("CostNeiliAllocationNotEnough"), this._combatSkillDisplayData.CostNeiliAllocationFontType, string.Format("x{0}", costNeiliAllocation.Item2));
			}
		}
		this.RefreshBodyAndMentalStrong(this._combatSkillDisplayData);
		this.RefreshHitParts();
		this.RefreshPageEffect(this._combatSkillDisplayData);
		bool isShowSpecial = this.InitSpecialBreakEffect(false, castAddPropertyHolder, startIndex);
		base.CGet<GameObject>("CastEffect").SetActive(this._configData.EquipType == 2 || this._configData.EquipType == 3 || isShowSpecial);
		UIElement element = this.Element;
		if (element != null)
		{
			element.ShowAfterRefresh();
		}
	}

	// Token: 0x06002964 RID: 10596 RVA: 0x001370C4 File Offset: 0x001352C4
	private short GetDisplayDataPower(CombatSkillDisplayData displayData)
	{
		return displayData.Power;
	}

	// Token: 0x06002965 RID: 10597 RVA: 0x001370CC File Offset: 0x001352CC
	private void UpdateOnlyTemplateData()
	{
		int power = 100;
		sbyte innerRatio = this._configData.BaseInnerRatio;
		string powerStr = string.Format("{0}{1}{2}%", LocalStringManager.Get(LanguageKey.LK_CombatSkill_Power), LocalStringManager.Get(LanguageKey.LK_Colon_Symbol), power);
		base.CGet<TextMeshProUGUI>("Power").text = powerStr;
		base.CGet<GameObject>("Warning").SetActive(false);
		base.CGet<GameObject>("Revoke").SetActive(false);
		base.CGet<CImage>("CostNeiliAllocationIcon").transform.parent.parent.gameObject.SetActive(false);
		bool flag = this._configData.Type == 0;
		if (flag)
		{
			this.UpdateGridCount(base.CGet<TextMeshProUGUI>("AttackGrid"), this._configData.SpecificGrids[0]);
			this.UpdateGridCount(base.CGet<TextMeshProUGUI>("AgileGrid"), this._configData.SpecificGrids[1]);
			this.UpdateGridCount(base.CGet<TextMeshProUGUI>("DefenceGrid"), this._configData.SpecificGrids[2]);
			this.UpdateGridCount(base.CGet<TextMeshProUGUI>("SpecialGrid"), this._configData.SpecificGrids[3]);
		}
		List<PropertyAndValue> equipAddPropertyList = new List<PropertyAndValue>(this._configData.PropertyAddList);
		int poisonNum = 0;
		short allPosionValue = 0;
		List<PropertyAndValue> poisons = new List<PropertyAndValue>();
		for (int i = 0; i < equipAddPropertyList.Count; i++)
		{
			PropertyAndValue addProperty = equipAddPropertyList[i];
			CharacterPropertyDisplayItem characterPropertyDisplayItem = CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[addProperty.PropertyId].DisplayType];
			ECharacterPropertyDisplayType type = characterPropertyDisplayItem.Type;
			bool flag2 = type == ECharacterPropertyDisplayType.ResistOfHotPoison || type == ECharacterPropertyDisplayType.ResistOfColdPoison || type == ECharacterPropertyDisplayType.ResistOfGloomyPoison || type == ECharacterPropertyDisplayType.ResistOfRedPoison || type == ECharacterPropertyDisplayType.ResistOfRottenPoison || type == ECharacterPropertyDisplayType.ResistOfIllusoryPoison;
			if (flag2)
			{
				poisonNum++;
				poisons.Add(addProperty);
				allPosionValue = addProperty.Value;
			}
		}
		bool allPoison = poisonNum >= 6;
		bool flag3 = allPoison;
		if (flag3)
		{
			for (int j = 0; j < poisons.Count; j++)
			{
				equipAddPropertyList.Remove(poisons[j]);
			}
			PropertyAndValue allPosionPropertyAndValue = new PropertyAndValue(10000, allPosionValue);
			equipAddPropertyList.Add(allPosionPropertyAndValue);
		}
		RectTransform equipAddPropertyHolder = base.CGet<RectTransform>("EquipAddPropertyHolder");
		bool anyEquipAddProperty = equipAddPropertyList.Count > 0;
		equipAddPropertyHolder.gameObject.SetActive(anyEquipAddProperty);
		bool flag4 = anyEquipAddProperty;
		if (flag4)
		{
			MultiHorizontalLayoutGroup equipEffectHolder = equipAddPropertyHolder.GetComponent<MultiHorizontalLayoutGroup>();
			equipEffectHolder.ResetData();
			for (int k = 0; k < equipAddPropertyList.Count; k++)
			{
				PropertyAndValue addProperty2 = equipAddPropertyList[k];
				bool flag5 = k < equipEffectHolder.childCount;
				TipsAddProperty addPropertyUi;
				if (flag5)
				{
					addPropertyUi = equipEffectHolder.GetChild(k).GetComponent<TipsAddProperty>();
				}
				else
				{
					addPropertyUi = Object.Instantiate<TipsAddProperty>(base.CGet<TipsAddProperty>("AddProperty"));
					addPropertyUi.transform.localScale = Vector3.one;
				}
				equipEffectHolder.AddChild(addPropertyUi.transform as RectTransform, false);
				int value = (int)(addProperty2.Value * 100 / 100) * power / 100;
				bool flag6 = addProperty2.PropertyId == 10000;
				if (flag6)
				{
					CharacterPropertyReferencedItem propertyReferencedItem = CharacterPropertyReferenced.Instance[28];
					CharacterPropertyDisplayItem propertyDisplayItem = CharacterPropertyDisplay.Instance[propertyReferencedItem.DisplayType];
					addPropertyUi.SetData("mousetip_duxing_big_all", LocalStringManager.Get(LanguageKey.LK_CombatSkill_AllPoison), value, false, propertyDisplayItem.IsPercent, false, true, false);
				}
				else
				{
					bool isPercent = CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[addProperty2.PropertyId].DisplayType].IsPercent;
					addPropertyUi.SetData(addProperty2.PropertyId, value, false, isPercent, false, !isPercent, true, false);
				}
				addPropertyUi.gameObject.SetActive(true);
			}
		}
		List<ValueTuple<short, int>> properties = this._configData.CalcDefaultNeiliAllocationBonus();
		this.UpdateNeiliAllocationAddProperty(properties);
		base.CGet<GameObject>("EquipEffect").SetActive((properties != null && properties.Count > 0) || anyEquipAddProperty);
		GameObject directTitle = base.CGet<GameObject>("DirectEffectTitle");
		GameObject directDesc = base.CGet<GameObject>("DirectDesc");
		GameObject reverseTitle = base.CGet<GameObject>("ReverseEffectTitle");
		GameObject reverseDesc = base.CGet<GameObject>("ReverseDesc");
		base.CGet<GameObject>("SpecialEffect").SetActive(true);
		directDesc.SetActive(true);
		directTitle.SetActive(true);
		reverseTitle.SetActive(true);
		reverseDesc.SetActive(true);
		directTitle.GetComponent<DisableStyleRoot>().SetStyleEffect(false, false);
		directDesc.GetComponent<DisableStyleRoot>().SetStyleEffect(false, false);
		reverseTitle.GetComponent<DisableStyleRoot>().SetStyleEffect(false, false);
		reverseDesc.GetComponent<DisableStyleRoot>().SetStyleEffect(false, false);
		this.UpdateSpecialEffectText(base.CGet<TextMeshProUGUI>("DirectEffectDesc"), CommonUtils.GetSpecialEffectDesc(this._configData.DirectEffectID));
		this.UpdateSpecialEffectText(base.CGet<TextMeshProUGUI>("ReverseEffectDesc"), CommonUtils.GetSpecialEffectDesc(this._configData.ReverseEffectID));
		base.CGet<CombatSkillEffectContainer>("CombatEffect").gameObject.SetActive(false);
		base.CGet<TipRequirement>("Requirement").CGet<GameObject>("SpecialBack").SetActive(false);
		RectTransform requirementHolder = base.CGet<TipRequirement>("Requirement").CGet<RectTransform>("RequirementHolder");
		bool flag7 = this._configData.UsingRequirement != null;
		if (flag7)
		{
			for (int l = 0; l < this._configData.UsingRequirement.Count; l++)
			{
				PropertyAndValue requirement = this._configData.UsingRequirement[l];
				bool flag8 = l < requirementHolder.childCount;
				TipsRequireProperty requireProperty;
				if (flag8)
				{
					requireProperty = requirementHolder.GetChild(l).GetComponent<TipsRequireProperty>();
				}
				else
				{
					requireProperty = Object.Instantiate<TipsRequireProperty>(base.CGet<TipsRequireProperty>("RequireProperty"), requirementHolder, true);
					requireProperty.transform.localScale = Vector3.one;
				}
				requireProperty.SetData(requirement.PropertyId, -1, (int)requirement.Value, "", false);
				requireProperty.gameObject.SetActive(true);
			}
		}
		List<PropertyAndValue> usingRequirement = this._configData.UsingRequirement;
		for (int m = (usingRequirement != null) ? usingRequirement.Count : 0; m < requirementHolder.childCount; m++)
		{
			requirementHolder.GetChild(m).gameObject.SetActive(false);
		}
		this._strBuilder.Clear();
		this._strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_CombatSkill_Curr_Power_Tips));
		this._strBuilder.Append(LocalStringManager.Get(LanguageKey.LK_Colon_Symbol));
		this._strBuilder.Append(power.ToString() + "%");
		base.CGet<TextMeshProUGUI>("CurrentPower").text = this._strBuilder.ToString();
		this._strBuilder.Clear();
		this._strBuilder.Append(LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Fix, LocalStringManager.Get(LanguageKey.LK_CombatSkill_Max_Power_Tips) + "100%"));
		base.CGet<TextMeshProUGUI>("MaxPower").text = this._strBuilder.ToString();
		bool flag9 = this._configData.EquipType == 0;
		if (flag9)
		{
			TextMeshProUGUI obtainedNeiliBlue = base.CGet<TextMeshProUGUI>("CurrObtainNeiliBlue");
			TextMeshProUGUI obtainedNeiliRed = base.CGet<TextMeshProUGUI>("CurrObtainNeiliRed");
			this.UpdateGridCount(base.CGet<TextMeshProUGUI>("GenericGrid"), this._configData.GenericGrid);
			base.CGet<TextMeshProUGUI>("MaxObtainNeili").text = string.Format("/{0}", this._configData.TotalObtainableNeili);
			obtainedNeiliBlue.gameObject.SetActive(true);
			obtainedNeiliRed.gameObject.SetActive(false);
			obtainedNeiliBlue.text = "-";
		}
		else
		{
			bool flag10 = this._configData.EquipType == 1;
			if (flag10)
			{
				bool isMindHit = CombatSkillEquipType.IsMindHitSkill(this._combatSkillTemplateId);
				this._strBuilder.Clear();
				this._strBuilder.Append("+");
				this._strBuilder.Append(((float)this._configData.DistanceAdditionWhenCast / 10f).ToString("f1"));
				base.CGet<TextMeshProUGUI>("RangeNear").text = this._strBuilder.ToString();
				this._strBuilder.Clear();
				this._strBuilder.Append("+");
				this._strBuilder.Append(((float)this._configData.DistanceAdditionWhenCast / 10f).ToString("f1"));
				base.CGet<TextMeshProUGUI>("RangeFar").text = this._strBuilder.ToString();
				this.UpdateHitInfo(base.CGet<RectTransform>("SpeedPowerIconHolder"), base.CGet<TextMeshProUGUI>("HitRateSpeed"), (int)(isMindHit ? 0 : (this._configData.PerHitDamageRateDistribution[2] / 10)), (int)(100 + this._configData.TotalHit * (short)this._configData.PerHitDamageRateDistribution[2] / 100), base.CGet<GameObject>("Speed"));
				this.UpdateHitInfo(base.CGet<RectTransform>("TechniquePowerIconHolder"), base.CGet<TextMeshProUGUI>("HitRateTechnique"), (int)(isMindHit ? 0 : (this._configData.PerHitDamageRateDistribution[1] / 10)), (int)(100 + this._configData.TotalHit * (short)this._configData.PerHitDamageRateDistribution[1] / 100), base.CGet<GameObject>("Technique"));
				this.UpdateHitInfo(base.CGet<RectTransform>("StrengthPowerIconHolder"), base.CGet<TextMeshProUGUI>("HitRateStrength"), (int)(isMindHit ? 0 : (this._configData.PerHitDamageRateDistribution[0] / 10)), (int)(100 + this._configData.TotalHit * (short)this._configData.PerHitDamageRateDistribution[0] / 100), base.CGet<GameObject>("Strength"));
				this.UpdateHitInfo(base.CGet<RectTransform>("MindPowerIconHolder"), base.CGet<TextMeshProUGUI>("HitRateMind"), isMindHit ? 10 : 0, (int)(100 + this._configData.TotalHit * (short)this._configData.PerHitDamageRateDistribution[3] / 100), base.CGet<GameObject>("Mind"));
				int penetrateInner = (int)(this._configData.Penetrate * (short)innerRatio / 100);
				base.CGet<TextMeshProUGUI>("OuterPenetrate").text = string.Format("{0}%", (int)(100 + this._configData.Penetrate) - penetrateInner);
				base.CGet<TextMeshProUGUI>("InnerPenetrate").text = string.Format("{0}%", 100 + penetrateInner);
			}
			else
			{
				bool flag11 = this._configData.EquipType == 2;
				if (flag11)
				{
					MultiHorizontalLayoutGroup castAddPropertyHolder = base.CGet<RectTransform>("CastAddPropertyHolder").GetComponent<MultiHorizontalLayoutGroup>();
					castAddPropertyHolder.ResetData();
					int index = 0;
					string jumpSpeedText = this.FormatJumpSpeed(this._configData.JumpPrepareFrame, CFormula.CalcJumpSpeed(0));
					base.CGet<GameObject>("JumpPrepareFrameRoot").SetActive(!string.IsNullOrEmpty(jumpSpeedText));
					base.CGet<TextMeshProUGUI>("JumpPrepareFrame").text = jumpSpeedText;
					bool flag12 = this._configData.AddMoveSpeedOnCast > 0;
					if (flag12)
					{
						MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 20, (int)(GlobalConfig.Instance.AgileSkillBaseAddSpeed * this._configData.AddMoveSpeedOnCast / 100), false, false, false, true, false);
						index++;
					}
					bool flag13 = this._configData.AddPercentMoveSpeedOnCast > 0;
					if (flag13)
					{
						MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 20, (int)this._configData.AddPercentMoveSpeedOnCast, false, true, false, true, false);
						index++;
					}
					bool flag14 = this._configData.AddHitOnCast[2] > 0;
					if (flag14)
					{
						MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 8, (int)(GlobalConfig.Instance.AgileSkillBaseAddHit * this._configData.AddHitOnCast[2] / 100), false, false, false, true, false);
						index++;
					}
					bool flag15 = this._configData.AddHitOnCast[1] > 0;
					if (flag15)
					{
						MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 7, (int)(GlobalConfig.Instance.AgileSkillBaseAddHit * this._configData.AddHitOnCast[1] / 100), false, false, false, true, false);
						index++;
					}
					bool flag16 = this._configData.AddHitOnCast[0] > 0;
					if (flag16)
					{
						MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 6, (int)(GlobalConfig.Instance.AgileSkillBaseAddHit * this._configData.AddHitOnCast[0] / 100), false, false, false, true, false);
						index++;
					}
					bool flag17 = this._configData.AddHitOnCast[3] > 0;
					if (flag17)
					{
						MouseTip_Util.AppendAddProperty(castAddPropertyHolder, base.CGet<TipsAddProperty>("AddProperty"), index, 9, (int)(GlobalConfig.Instance.AgileSkillBaseAddHit * this._configData.AddHitOnCast[3] / 100), false, false, false, true, false);
						index++;
					}
				}
				else
				{
					bool flag18 = this._configData.EquipType == 3;
					if (flag18)
					{
						MultiHorizontalLayoutGroup castAddPropertyHolder2 = base.CGet<RectTransform>("CastAddPropertyHolder").GetComponent<MultiHorizontalLayoutGroup>();
						castAddPropertyHolder2.ResetData();
						int index2 = 0;
						bool flag19 = this._configData.AddAvoidOnCast[2] > 0;
						if (flag19)
						{
							MouseTip_Util.AppendAddProperty(castAddPropertyHolder2, base.CGet<TipsAddProperty>("AddProperty"), index2, 14, (int)this._configData.AddAvoidOnCast[2], false, false, false, true, false);
							index2++;
						}
						bool flag20 = this._configData.AddAvoidOnCast[1] > 0;
						if (flag20)
						{
							MouseTip_Util.AppendAddProperty(castAddPropertyHolder2, base.CGet<TipsAddProperty>("AddProperty"), index2, 13, (int)this._configData.AddAvoidOnCast[1], false, false, false, true, false);
							index2++;
						}
						bool flag21 = this._configData.AddAvoidOnCast[0] > 0;
						if (flag21)
						{
							MouseTip_Util.AppendAddProperty(castAddPropertyHolder2, base.CGet<TipsAddProperty>("AddProperty"), index2, 12, (int)this._configData.AddAvoidOnCast[0], false, false, false, true, false);
							index2++;
						}
						bool flag22 = this._configData.AddAvoidOnCast[3] > 0;
						if (flag22)
						{
							MouseTip_Util.AppendAddProperty(castAddPropertyHolder2, base.CGet<TipsAddProperty>("AddProperty"), index2, 15, (int)this._configData.AddAvoidOnCast[3], false, false, false, true, false);
							index2++;
						}
						bool flag23 = this._configData.AddOuterPenetrateResistOnCast > 0;
						if (flag23)
						{
							MouseTip_Util.AppendAddProperty(castAddPropertyHolder2, base.CGet<TipsAddProperty>("AddProperty"), index2, 16, (int)this._configData.AddOuterPenetrateResistOnCast, false, false, false, true, false);
							index2++;
						}
						bool flag24 = this._configData.AddInnerPenetrateResistOnCast > 0;
						if (flag24)
						{
							MouseTip_Util.AppendAddProperty(castAddPropertyHolder2, base.CGet<TipsAddProperty>("AddProperty"), index2, 17, (int)this._configData.AddInnerPenetrateResistOnCast, false, false, false, true, false);
							index2++;
						}
						this.RefreshBounce(this._configData.BounceDistance, (int)(this._configData.BounceRateOfOuterInjury * GlobalConfig.Instance.DefendSkillBaseBouncePower / 100), (int)(this._configData.BounceRateOfInnerInjury * GlobalConfig.Instance.DefendSkillBaseBouncePower / 100), false);
						int fightbackPower = (int)(GlobalConfig.Instance.DefendSkillBaseFightBackPower * this._configData.FightBackDamage / 100);
						this.RefreshFightBack(fightbackPower, false);
						this.RefreshEffectDuration(this._configData.ContinuousFrames, false);
					}
				}
			}
		}
		bool flag25 = this._configData.EquipType == 1 || this._configData.EquipType == 2 || this._configData.EquipType == 3;
		if (flag25)
		{
			base.CGet<GameObject>("CostMobility").SetActive(this._configData.MobilityCost > 0);
			bool flag26 = this._configData.MobilityCost > 0;
			if (flag26)
			{
				this.UpdateCostText(base.CGet<TextMeshProUGUI>("MobilityCostNormal"), base.CGet<TextMeshProUGUI>("MobilityCostEnough"), base.CGet<TextMeshProUGUI>("MobilityCostNotEnough"), 0, string.Format("{0}%", this._configData.MobilityCost));
			}
			List<NeedTrick> costTrickList = this._configData.TrickCost;
			RectTransform costTrickHolder = base.CGet<RectTransform>("CostTrickHolder");
			base.CGet<GameObject>("CostTrick").SetActive(costTrickList != null && costTrickList.Count > 0);
			bool flag27 = costTrickList != null && costTrickList.Count > 0;
			if (flag27)
			{
				for (int n = 0; n < costTrickList.Count; n++)
				{
					NeedTrick needTrick = costTrickList[n];
					bool flag28 = n >= costTrickHolder.childCount;
					if (flag28)
					{
						Object.Instantiate<GameObject>(this._costTrickPrefab.gameObject, costTrickHolder);
					}
					Refers trickRefers = costTrickHolder.GetChild(n).GetComponent<Refers>();
					TrickTypeItem trickConfig = Config.TrickType.Instance[needTrick.TrickType];
					trickRefers.CGet<TextMeshProUGUI>("TrickName").text = trickConfig.Name.SetColor(trickConfig.FontColor);
					this.UpdateCostText(trickRefers.CGet<TextMeshProUGUI>("CountNormal"), trickRefers.CGet<TextMeshProUGUI>("CountEnough"), trickRefers.CGet<TextMeshProUGUI>("CountNotEnough"), 0, string.Format("×{0}", needTrick.NeedCount));
					trickRefers.gameObject.SetActive(true);
				}
				for (int i2 = costTrickList.Count; i2 < costTrickHolder.childCount; i2++)
				{
					costTrickHolder.GetChild(i2).gameObject.SetActive(false);
				}
			}
			GameObject costBreathStance = base.CGet<GameObject>("CostBreathStance");
			int costBreath = (int)(this._configData.BreathStanceTotalCost * innerRatio / 100);
			costBreathStance.SetActive(this._configData.BreathStanceTotalCost > 0);
			string stanceText = string.Format("{0}%", (int)this._configData.BreathStanceTotalCost - costBreath);
			this.UpdateCostText(base.CGet<TextMeshProUGUI>("StanceNormal"), base.CGet<TextMeshProUGUI>("StanceEnough"), base.CGet<TextMeshProUGUI>("StanceNotEnough"), 0, stanceText);
			this.UpdateCostText(base.CGet<TextMeshProUGUI>("BreathNormal"), base.CGet<TextMeshProUGUI>("BreathEnough"), base.CGet<TextMeshProUGUI>("BreathNotEnough"), 0, string.Format("{0}%", costBreath));
			base.CGet<GameObject>("CostWug").SetActive(this._configData.WugCost > 0);
			this.UpdateCostText(base.CGet<TextMeshProUGUI>("WugCostNormal"), base.CGet<TextMeshProUGUI>("WugCostEnough"), base.CGet<TextMeshProUGUI>("WugCostNotEnough"), 0, this._configData.WugCost.ToString());
			base.CGet<GameObject>("CostWeaponDurability").SetActive(this._configData.WeaponDurableCost > 0);
			this.UpdateCostText(base.CGet<TextMeshProUGUI>("WeaponCostNormal"), base.CGet<TextMeshProUGUI>("WeaponCostEnough"), base.CGet<TextMeshProUGUI>("WeaponCostNotEnough"), 0, this._configData.WeaponDurableCost.ToString());
		}
		this.InitiateHitParts();
		this.RefreshBodyAndMentalStrong(null);
		this.RefreshPageEffect(null);
	}

	// Token: 0x06002966 RID: 10598 RVA: 0x001383CC File Offset: 0x001365CC
	private void UpdateGridCount(TextMeshProUGUI gridText, sbyte gridCount)
	{
		this.UpdateGridCount(gridText, gridCount, gridCount.ToString());
	}

	// Token: 0x06002967 RID: 10599 RVA: 0x001383DF File Offset: 0x001365DF
	private void UpdateGridCount(TextMeshProUGUI gridText, sbyte gridCount, string gridCountString)
	{
		gridText.text = gridCountString;
		gridText.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2((float)((gridCount > 9 || gridCount < 0) ? 40 : 26), 26f);
	}

	// Token: 0x06002968 RID: 10600 RVA: 0x0013841C File Offset: 0x0013661C
	private void UpdateGridCountBack(GameObject gridSpecialBack, short propertyId)
	{
		bool inSpecial = this.GetPropertyIsInSpecialBreaks(propertyId, false);
		gridSpecialBack.SetActive(inSpecial);
	}

	// Token: 0x06002969 RID: 10601 RVA: 0x0013843C File Offset: 0x0013663C
	private unsafe void RefreshSelfPoisons(PoisonsAndLevels innatePoisons)
	{
		bool hasInnatePoison = innatePoisons.IsNonZero();
		Refers poisonLayout = base.CGet<Refers>("PoisonLayout");
		poisonLayout.gameObject.SetActive(hasInnatePoison);
		bool flag = hasInnatePoison;
		if (flag)
		{
			Refers poisons = poisonLayout.CGet<Refers>("SelfPoisons");
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
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(poisonHolder.parent.transform as RectTransform);
			});
		}
	}

	// Token: 0x0600296A RID: 10602 RVA: 0x001385AB File Offset: 0x001367AB
	private void UpdateHitInfo(RectTransform powerIconHolder, TextMeshProUGUI valueText, int power, int hitValue, GameObject powerRoot)
	{
		this.UpdateHitInfo(powerIconHolder, valueText, power, string.Format("{0}%", hitValue), powerRoot);
	}

	// Token: 0x0600296B RID: 10603 RVA: 0x001385CC File Offset: 0x001367CC
	private void UpdateHitInfo(RectTransform powerIconHolder, TextMeshProUGUI valueText, int power, string hitValueString, GameObject powerRoot)
	{
		powerRoot.SetActive(power > 0);
		bool flag = power == 0;
		if (!flag)
		{
			int lightNum = 0;
			CImage[] powerIcons = powerIconHolder.GetComponentsInChildren<CImage>(true);
			for (int i = 0; i < powerIcons.Length; i++)
			{
				CImage icon = powerIcons[i];
				bool show = i < power;
				icon.gameObject.SetActive(i < power);
				bool flag2 = i >= power - lightNum && show;
				if (flag2)
				{
					icon.SetSprite("mousetip_mingzongtiao_buff", false, null);
				}
				else
				{
					icon.SetSprite("mousetip_mingzongtiao", false, null);
				}
			}
			powerIconHolder.GetChild(5).gameObject.SetActive(power > 5);
			valueText.text = hitValueString;
		}
	}

	// Token: 0x0600296C RID: 10604 RVA: 0x00138684 File Offset: 0x00136884
	private void UpdateCostText(TextMeshProUGUI normalText, TextMeshProUGUI enoughText, TextMeshProUGUI notEnoughText, sbyte fontType, string text)
	{
		TextMeshProUGUI showingText = (fontType == 0) ? normalText : ((fontType == 1) ? enoughText : notEnoughText);
		normalText.gameObject.SetActive(fontType == 0);
		enoughText.gameObject.SetActive(fontType == 1);
		notEnoughText.gameObject.SetActive(fontType == 2);
		showingText.text = text;
	}

	// Token: 0x0600296D RID: 10605 RVA: 0x001386E0 File Offset: 0x001368E0
	private void UpdateNeiliAllocationAddProperty([TupleElementNames(new string[]
	{
		"propertyId",
		"bonus"
	})] List<ValueTuple<short, int>> data)
	{
		bool valid = data != null && data.Count > 0;
		RectTransform holder = base.CGet<RectTransform>("NeiliAllocationAddPropertyHolder");
		holder.gameObject.SetActive(valid);
		bool flag = !valid;
		if (!flag)
		{
			GameObject template = holder.GetChild(1).gameObject;
			for (int i = 0; i < data.Count; i++)
			{
				ValueTuple<short, int> valueTuple = data[i];
				short propertyId = valueTuple.Item1;
				int value = valueTuple.Item2;
				int propertyIndex = i + 1;
				bool flag2 = propertyIndex == holder.childCount;
				if (flag2)
				{
					Object.Instantiate<GameObject>(template, holder);
				}
				TipsAddProperty property = holder.GetChild(propertyIndex).GetComponent<TipsAddProperty>();
				property.Set(propertyId, value);
				property.gameObject.SetActive(true);
			}
			for (int j = data.Count + 1; j < holder.childCount; j++)
			{
				holder.GetChild(j).gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600296E RID: 10606 RVA: 0x001387E2 File Offset: 0x001369E2
	private void UpdateSpecialEffectText(TextMeshProUGUI effectText, string effectStr)
	{
		effectStr = "     " + effectStr;
		effectText.text = effectStr;
	}

	// Token: 0x0600296F RID: 10607 RVA: 0x001387FC File Offset: 0x001369FC
	private void UpdateAttackTypeEffect()
	{
		bool flag = this._configData.EquipType == 1;
		if (flag)
		{
			base.CGet<GameObject>("AtkTips").SetActive(this._configData.HasAtkAcupointEffect || this._configData.HasAtkFlawEffect);
			int power = Mathf.CeilToInt((float)CommonUtils.CalcSumMax2HitDistribution((int)this._configData.TemplateId) / 10f);
			TextMeshProUGUI atkAcupoint = base.CGet<TextMeshProUGUI>("AtkAcupoint");
			atkAcupoint.gameObject.SetActive(this._configData.HasAtkAcupointEffect);
			atkAcupoint.text = LocalStringManager.GetFormat(LanguageKey.LK_AtkAcupoint_Dynamic_Tips, LocalStringManager.Get(string.Format("LK_Num_{0}", power))).ColorReplace();
			TextMeshProUGUI atkFlaw = base.CGet<TextMeshProUGUI>("AtkFlaw");
			atkFlaw.gameObject.SetActive(this._configData.HasAtkFlawEffect);
			atkFlaw.text = LocalStringManager.GetFormat(LanguageKey.LK_AtkFlaw_Dynamic_Tips, LocalStringManager.Get(string.Format("LK_Num_{0}", power))).ColorReplace();
		}
		CombatSkillDisplayData combatSkillDisplayData = this._combatSkillDisplayData;
		PoisonsAndLevels poison = (combatSkillDisplayData != null) ? combatSkillDisplayData.Poisons : this._configData.Poisons;
		this.RefreshSelfPoisons(poison);
	}

	// Token: 0x06002970 RID: 10608 RVA: 0x00138930 File Offset: 0x00136B30
	public override void OnSticked()
	{
		bool activeSelf = base.CGet<Transform>("ParticleLayer").gameObject.activeSelf;
		if (activeSelf)
		{
			this._stickedIndex = MouseTipCombatSkill._stickedCount;
			MouseTipCombatSkill._stickedCount++;
		}
	}

	// Token: 0x06002971 RID: 10609 RVA: 0x00138970 File Offset: 0x00136B70
	private void Update()
	{
		bool flag = !this.HasStick;
		if (flag)
		{
			bool altDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
			bool altChanged = altDown != this._lastAltDownState;
			this._altDownWhenNotSticked = altDown;
			this._lastAltDownState = altDown;
			base.CGet<TipRequirement>("Requirement").gameObject.SetActive(altDown && !this._fixedCombatPower);
			base.CGet<GameObject>("CurrAndMaxPowerRoot").gameObject.SetActive(altDown && !this._fixedCombatPower);
			base.CGet<GameObject>("BodyAndMentalStrong").SetActive(altDown && this.CanBodyAndMentalStrongShow());
			MoreInfo2 moreInfo = base.CGet<MoreInfo2>("MoreInfo2");
			this.TickRefreshDirectActive(altDown);
			bool flag2 = altDown;
			if (flag2)
			{
				moreInfo.RefreshCancelDetail();
			}
			else
			{
				moreInfo.RefreshPressToDetail();
			}
			bool flag3 = altChanged;
			if (flag3)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
			}
		}
		else
		{
			bool flag4 = this._stickedIndex >= 0;
			if (flag4)
			{
				base.CGet<Transform>("ParticleLayer").position = Vector3.zero.SetY((float)(-3000 + -100 * this._stickedIndex));
			}
		}
	}

	// Token: 0x06002972 RID: 10610 RVA: 0x00138AB4 File Offset: 0x00136CB4
	private void TickRefreshDirectActive(bool altDown)
	{
		bool flag = this._combatSkillDisplayData != null;
		if (flag)
		{
			bool broken = this._combatSkillDisplayData.EffectType != -1;
			bool direct = this._combatSkillDisplayData.EffectType == 0;
			GameObject directTitle = base.CGet<GameObject>("DirectEffectTitle");
			GameObject directDesc = base.CGet<GameObject>("DirectDesc");
			GameObject reverseTitle = base.CGet<GameObject>("ReverseEffectTitle");
			GameObject reverseDesc = base.CGet<GameObject>("ReverseDesc");
			directTitle.SetActive((broken && direct) || altDown);
			directDesc.SetActive((broken && direct) || altDown);
			reverseTitle.SetActive((broken && !direct) || altDown);
			reverseDesc.SetActive((broken && !direct) || altDown);
		}
	}

	// Token: 0x06002973 RID: 10611 RVA: 0x00138B6C File Offset: 0x00136D6C
	private void InitiateCostNeed()
	{
		List<sbyte> needBodyPartTypes = this._configData.NeedBodyPartTypes;
		GameObject costNeedRoot = base.CGet<GameObject>("CostNeed");
		costNeedRoot.SetActive(true);
		bool flag = needBodyPartTypes.Count < 1;
		if (flag)
		{
			costNeedRoot.SetActive(false);
		}
		else
		{
			GameObject costNeedHolder = base.CGet<GameObject>("CostNeedHolder");
			for (int x = 0; x < costNeedHolder.transform.childCount; x++)
			{
				costNeedHolder.transform.GetChild(x).gameObject.SetActive(false);
			}
			Refers costNeedPrefab = base.CGet<Refers>("CostNeedPrefab");
			Refers currentCostNeed = null;
			int index = 0;
			for (int i = 0; i < needBodyPartTypes.Count; i++)
			{
				switch (needBodyPartTypes[i])
				{
				case 0:
					currentCostNeed = this.CreateCostNeedObject(costNeedPrefab, costNeedHolder.transform, index);
					currentCostNeed.CGet<CImage>("Icon").SetSprite(MouseTipCombatSkill.GetPartIcon(0), false, null);
					currentCostNeed.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.Get(MouseTipConstant.HitPartNames[0, 1]);
					index++;
					break;
				case 1:
					currentCostNeed = this.CreateCostNeedObject(costNeedPrefab, costNeedHolder.transform, index);
					currentCostNeed.CGet<CImage>("Icon").SetSprite(MouseTipCombatSkill.GetPartIcon(1), false, null);
					currentCostNeed.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.Get(MouseTipConstant.HitPartNames[1, 1]);
					index++;
					break;
				case 2:
					currentCostNeed = this.CreateCostNeedObject(costNeedPrefab, costNeedHolder.transform, index);
					currentCostNeed.CGet<CImage>("Icon").SetSprite(MouseTipCombatSkill.GetPartIcon(2), false, null);
					currentCostNeed.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.Get(MouseTipConstant.HitPartNames[2, 1]);
					index++;
					break;
				case 3:
					currentCostNeed = this.CreateCostNeedObject(costNeedPrefab, costNeedHolder.transform, index);
					currentCostNeed.CGet<CImage>("Icon").SetSprite(MouseTipCombatSkill.GetPartIcon(3), false, null);
					currentCostNeed.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.Get(MouseTipConstant.HitPartNames[3, 1]);
					index++;
					currentCostNeed = this.CreateCostNeedObject(costNeedPrefab, costNeedHolder.transform, index);
					currentCostNeed.CGet<CImage>("Icon").SetSprite(MouseTipCombatSkill.GetPartIcon(4), false, null);
					currentCostNeed.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.Get(MouseTipConstant.HitPartNames[4, 1]);
					index++;
					break;
				case 4:
					currentCostNeed = this.CreateCostNeedObject(costNeedPrefab, costNeedHolder.transform, index);
					currentCostNeed.CGet<CImage>("Icon").SetSprite(MouseTipCombatSkill.GetPartIcon(3), false, null);
					currentCostNeed.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.Get(MouseTipConstant.HitPartNames[3, 1]);
					index++;
					currentCostNeed.CGet<GameObject>("OrTemplate").gameObject.SetActive(true);
					currentCostNeed = this.CreateCostNeedObject(costNeedPrefab, costNeedHolder.transform, index);
					currentCostNeed.CGet<CImage>("Icon").SetSprite(MouseTipCombatSkill.GetPartIcon(4), false, null);
					currentCostNeed.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.Get(MouseTipConstant.HitPartNames[4, 1]);
					index++;
					break;
				case 5:
					currentCostNeed = this.CreateCostNeedObject(costNeedPrefab, costNeedHolder.transform, index);
					currentCostNeed.CGet<CImage>("Icon").SetSprite(MouseTipCombatSkill.GetPartIcon(5), false, null);
					currentCostNeed.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.Get(MouseTipConstant.HitPartNames[5, 1]);
					index++;
					currentCostNeed = this.CreateCostNeedObject(costNeedPrefab, costNeedHolder.transform, index);
					currentCostNeed.CGet<CImage>("Icon").SetSprite(MouseTipCombatSkill.GetPartIcon(6), false, null);
					currentCostNeed.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.Get(MouseTipConstant.HitPartNames[6, 1]);
					index++;
					break;
				case 6:
					currentCostNeed = this.CreateCostNeedObject(costNeedPrefab, costNeedHolder.transform, index);
					currentCostNeed.CGet<CImage>("Icon").SetSprite(MouseTipCombatSkill.GetPartIcon(5), false, null);
					currentCostNeed.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.Get(MouseTipConstant.HitPartNames[5, 1]);
					index++;
					currentCostNeed.CGet<GameObject>("OrTemplate").gameObject.SetActive(true);
					currentCostNeed = this.CreateCostNeedObject(costNeedPrefab, costNeedHolder.transform, index);
					currentCostNeed.CGet<CImage>("Icon").SetSprite(MouseTipCombatSkill.GetPartIcon(6), false, null);
					currentCostNeed.CGet<TextMeshProUGUI>("Name").text = LocalStringManager.Get(MouseTipConstant.HitPartNames[6, 1]);
					index++;
					break;
				}
				currentCostNeed.CGet<GameObject>("AndTemplate").SetActive(i < needBodyPartTypes.Count - 1);
			}
		}
	}

	// Token: 0x06002974 RID: 10612 RVA: 0x00139078 File Offset: 0x00137278
	private Refers CreateCostNeedObject(Refers refersPrefab, Transform parent, int index)
	{
		bool flag = index >= parent.transform.childCount;
		Refers costNeed;
		if (flag)
		{
			costNeed = Object.Instantiate<Refers>(refersPrefab, parent);
		}
		else
		{
			costNeed = parent.GetChild(index).GetComponent<Refers>();
		}
		costNeed.gameObject.SetActive(true);
		costNeed.CGet<GameObject>("AndTemplate").gameObject.SetActive(false);
		costNeed.CGet<GameObject>("OrTemplate").gameObject.SetActive(false);
		return costNeed;
	}

	// Token: 0x06002975 RID: 10613 RVA: 0x001390F8 File Offset: 0x001372F8
	private void InitiateHitParts()
	{
		int[] injuryPartAtkRates = new int[this._configData.InjuryPartAtkRateDistribution.Length];
		this._configData.InjuryPartAtkRateDistribution.CopyTo(injuryPartAtkRates, 0);
		CombatSkillHitParts combatSkillHitParts = base.CGet<CombatSkillHitParts>("HitParts");
		combatSkillHitParts.SetData(injuryPartAtkRates, null);
	}

	// Token: 0x06002976 RID: 10614 RVA: 0x00139144 File Offset: 0x00137344
	private void RefreshHitParts()
	{
		int[] injuryPartAtkRates = new int[7];
		List<int> bodyPartWeights = this._combatSkillDisplayData.BodyPartWeights;
		if (bodyPartWeights != null)
		{
			bodyPartWeights.CopyTo(injuryPartAtkRates, 0);
		}
		bool[] showBacks = this.CalInjuryPartAtkRatesBySpecialBreak();
		CombatSkillHitParts combatSkillHitParts = base.CGet<CombatSkillHitParts>("HitParts");
		combatSkillHitParts.SetData(injuryPartAtkRates, showBacks);
	}

	// Token: 0x06002977 RID: 10615 RVA: 0x00139190 File Offset: 0x00137390
	private bool[] CalInjuryPartAtkRatesBySpecialBreak()
	{
		bool[] showBacks = new bool[7];
		bool propertyIsInSpecialBreaks = this.GetPropertyIsInSpecialBreaks(35, false);
		if (propertyIsInSpecialBreaks)
		{
			showBacks[0] = true;
		}
		else
		{
			bool propertyIsInSpecialBreaks2 = this.GetPropertyIsInSpecialBreaks(36, false);
			if (propertyIsInSpecialBreaks2)
			{
				showBacks[1] = true;
			}
			else
			{
				bool propertyIsInSpecialBreaks3 = this.GetPropertyIsInSpecialBreaks(37, false);
				if (propertyIsInSpecialBreaks3)
				{
					showBacks[2] = true;
				}
				else
				{
					bool propertyIsInSpecialBreaks4 = this.GetPropertyIsInSpecialBreaks(38, false);
					if (propertyIsInSpecialBreaks4)
					{
						showBacks[3] = true;
						showBacks[4] = true;
					}
					else
					{
						bool propertyIsInSpecialBreaks5 = this.GetPropertyIsInSpecialBreaks(39, false);
						if (propertyIsInSpecialBreaks5)
						{
							showBacks[5] = true;
							showBacks[6] = true;
						}
					}
				}
			}
		}
		return showBacks;
	}

	// Token: 0x06002978 RID: 10616 RVA: 0x00139220 File Offset: 0x00137420
	private bool CanBodyAndMentalStrongShow()
	{
		GameObject bodyAndMentalStrongHolder = base.CGet<GameObject>("BodyAndMentalStrongHolder");
		bool showRoot = false;
		for (int i = 0; i < bodyAndMentalStrongHolder.transform.childCount; i++)
		{
			bool activeSelf = bodyAndMentalStrongHolder.transform.GetChild(i).gameObject.activeSelf;
			if (activeSelf)
			{
				showRoot = true;
				break;
			}
		}
		return showRoot;
	}

	// Token: 0x06002979 RID: 10617 RVA: 0x00139280 File Offset: 0x00137480
	private void InitiateBodyAndMentalStrong()
	{
		GameObject bodyAndMentalStrong = base.CGet<GameObject>("BodyAndMentalStrong");
		GameObject bodyAndMentalStrongHolder = base.CGet<GameObject>("BodyAndMentalStrongHolder");
		bodyAndMentalStrongHolder.SetActive(true);
		int[] outer = this._configData.OuterDamageSteps;
		int[] inner = this._configData.InnerDamageSteps;
		int total = outer.Length + inner.Length;
		bool flag = total <= 0;
		if (flag)
		{
			bodyAndMentalStrongHolder.SetActive(false);
		}
		else
		{
			for (int x = 0; x < bodyAndMentalStrongHolder.transform.childCount; x++)
			{
				bodyAndMentalStrongHolder.transform.GetChild(x).gameObject.SetActive(false);
			}
			for (int i = 0; i < outer.Length; i++)
			{
				int configIndex = (int)CommonUtils.ConvertShowIndexToConfigIndex((sbyte)i);
				bool flag2 = i >= bodyAndMentalStrongHolder.transform.childCount;
				CombatSkillCommonTip2ValueRefers tip;
				if (flag2)
				{
					tip = Object.Instantiate<CombatSkillCommonTip2ValueRefers>(this._commonTip2ValueRefers, bodyAndMentalStrongHolder.transform);
				}
				else
				{
					tip = bodyAndMentalStrongHolder.transform.GetChild(i).GetComponent<CombatSkillCommonTip2ValueRefers>();
				}
				tip.DefaultShow();
				this.CreateBodyAndMentalStrongNode(outer, MouseTipConstant.OuterInjuryInfos, tip, CombatSkillCommonTip2ValueRefers.ElementType.One, i, configIndex);
				this.CreateBodyAndMentalStrongNode(inner, MouseTipConstant.InnerInjuryInfos, tip, CombatSkillCommonTip2ValueRefers.ElementType.Two, i, configIndex);
				int injury = outer[configIndex] + inner[configIndex];
				bool flag3 = injury <= 0;
				if (flag3)
				{
					tip.gameObject.SetActive(false);
				}
			}
			int fatal = this._configData.FatalDamageStep;
			bool flag4 = outer.Length >= bodyAndMentalStrongHolder.transform.childCount;
			CombatSkillCommonTip2ValueRefers tipFatal;
			if (flag4)
			{
				tipFatal = Object.Instantiate<CombatSkillCommonTip2ValueRefers>(this._commonTip2ValueRefers, bodyAndMentalStrongHolder.transform);
			}
			else
			{
				tipFatal = bodyAndMentalStrongHolder.transform.GetChild(outer.Length).GetComponent<CombatSkillCommonTip2ValueRefers>();
			}
			tipFatal.gameObject.name = "Fatal";
			tipFatal.gameObject.SetActive(true);
			tipFatal.DefaultShow();
			tipFatal.ShowElement(CombatSkillCommonTip2ValueRefers.ElementType.One, "LK_CombatSkill_Injury_Fatal", "mousetip_zhongchuang_0", "");
			tipFatal.gameObject.SetActive(fatal > 0);
			int Mind = this._configData.MindDamageStep;
			bool flag5 = outer.Length + 1 >= bodyAndMentalStrongHolder.transform.childCount;
			CombatSkillCommonTip2ValueRefers tipMind;
			if (flag5)
			{
				tipMind = Object.Instantiate<CombatSkillCommonTip2ValueRefers>(this._commonTip2ValueRefers, bodyAndMentalStrongHolder.transform);
			}
			else
			{
				tipMind = bodyAndMentalStrongHolder.transform.GetChild(outer.Length + 1).GetComponent<CombatSkillCommonTip2ValueRefers>();
			}
			tipMind.gameObject.name = "Mind";
			tipMind.gameObject.SetActive(true);
			tipMind.DefaultShow();
			tipMind.ShowElement(CombatSkillCommonTip2ValueRefers.ElementType.One, "LK_CombatSkill_Injury_Mind", "mousetip_dongxin_0", "");
			tipMind.gameObject.SetActive(Mind > 0);
			bodyAndMentalStrong.SetActive(false);
		}
	}

	// Token: 0x0600297A RID: 10618 RVA: 0x00139548 File Offset: 0x00137748
	private void CreateBodyAndMentalStrongNode(int[] damageSteps, string[,] infos, CombatSkillCommonTip2ValueRefers tip, CombatSkillCommonTip2ValueRefers.ElementType elementType, int index, int configIndex)
	{
		tip.gameObject.name = infos[index, 0];
		tip.gameObject.SetActive(true);
		int damage = damageSteps[configIndex];
		bool flag = damage <= 0;
		if (!flag)
		{
			tip.ShowElement(elementType, infos[index, 1], infos[index, 2], "");
		}
	}

	// Token: 0x0600297B RID: 10619 RVA: 0x001395AC File Offset: 0x001377AC
	private void RefreshBodyAndMentalStrong(CombatSkillDisplayData combatSkillDisplayData = null)
	{
		GameObject bodyAndMentalStrongHolder = base.CGet<GameObject>("BodyAndMentalStrongHolder");
		int[] outer = this._configData.OuterDamageSteps;
		int[] inner = this._configData.InnerDamageSteps;
		CombatSkillDisplayData combatSkillDisplayData2 = this._combatSkillDisplayData;
		CombatSkillDamageStepBonusDisplayData? bonus = (combatSkillDisplayData2 != null) ? new CombatSkillDamageStepBonusDisplayData?(combatSkillDisplayData2.DamageStepBonus) : null;
		for (int i = 0; i < outer.Length; i++)
		{
			int configIndex = (int)CommonUtils.ConvertShowIndexToConfigIndex((sbyte)i);
			CombatSkillCommonTip2ValueRefers tip = bodyAndMentalStrongHolder.transform.GetChild(i).GetComponent<CombatSkillCommonTip2ValueRefers>();
			int outValue = outer[configIndex];
			int innerValue = inner[configIndex];
			bool flag = bonus != null;
			if (flag)
			{
				outValue *= bonus.Value.OuterInjuryStepBonus;
				innerValue *= bonus.Value.InnerInjuryStepBonus;
			}
			this.RefreshBodyAndMentalStrongNode(tip, outValue, MouseTipCombatSkill.InjuryStrongShowType.Outer);
			this.RefreshBodyAndMentalStrongNode(tip, innerValue, MouseTipCombatSkill.InjuryStrongShowType.Inner);
		}
		CombatSkillCommonTip2ValueRefers tipFatal = bodyAndMentalStrongHolder.transform.GetChild(outer.Length).GetComponent<CombatSkillCommonTip2ValueRefers>();
		CombatSkillCommonTip2ValueRefers tipMind = bodyAndMentalStrongHolder.transform.GetChild(outer.Length + 1).GetComponent<CombatSkillCommonTip2ValueRefers>();
		int fatalValue = this._configData.FatalDamageStep;
		int mindValue = this._configData.MindDamageStep;
		bool flag2 = bonus != null;
		if (flag2)
		{
			fatalValue *= bonus.Value.FatalStepBonus;
			mindValue *= bonus.Value.MindStepBonus;
		}
		this.RefreshBodyAndMentalStrongNode(tipFatal, fatalValue, MouseTipCombatSkill.InjuryStrongShowType.Fatal);
		this.RefreshBodyAndMentalStrongNode(tipMind, mindValue, MouseTipCombatSkill.InjuryStrongShowType.Mind);
	}

	// Token: 0x0600297C RID: 10620 RVA: 0x00139748 File Offset: 0x00137948
	private void RefreshPageEffect(CombatSkillDisplayData skillData = null)
	{
		GameObject pageEffectRoot = base.CGet<GameObject>("PageEffectRoot");
		pageEffectRoot.SetActive(false);
		bool flag = skillData == null;
		if (!flag)
		{
			byte directPageIndex = CombatSkillStateHelper.GetPageInternalIndex(-1, 0, 2);
			byte reversePageIndex = CombatSkillStateHelper.GetPageInternalIndex(-1, 1, 2);
			bool isDirectPageActive = CombatSkillStateHelper.IsPageActive(skillData.ActivationState, directPageIndex);
			bool isReversePageActive = CombatSkillStateHelper.IsPageActive(skillData.ActivationState, reversePageIndex);
			bool flag2 = this._configData.EquipType == 3 && (isDirectPageActive || isReversePageActive);
			if (flag2)
			{
				pageEffectRoot.SetActive(true);
				TextMeshProUGUI label = pageEffectRoot.transform.Find("PageEffectLabel").GetComponent<TextMeshProUGUI>();
				label.text = LocalStringManager.Get(isDirectPageActive ? LanguageKey.LK_CombatSkill_Tip_PageEffectDesc_Direct : LanguageKey.LK_CombatSkill_Tip_PageEffectDesc_Reverse);
			}
		}
	}

	// Token: 0x0600297D RID: 10621 RVA: 0x00139800 File Offset: 0x00137A00
	private void RefreshBodyAndMentalStrongNode(CombatSkillCommonTip2ValueRefers tip, int damage, MouseTipCombatSkill.InjuryStrongShowType type)
	{
		bool flag = damage <= 0;
		if (!flag)
		{
			string valueStr = string.Format("+{0}", damage);
			if (!true)
			{
			}
			ValueTuple<string, CombatSkillCommonTip2ValueRefers.ElementType> valueTuple;
			switch (type)
			{
			case MouseTipCombatSkill.InjuryStrongShowType.Outer:
				valueTuple = new ValueTuple<string, CombatSkillCommonTip2ValueRefers.ElementType>("outterinjury", CombatSkillCommonTip2ValueRefers.ElementType.One);
				break;
			case MouseTipCombatSkill.InjuryStrongShowType.Inner:
				valueTuple = new ValueTuple<string, CombatSkillCommonTip2ValueRefers.ElementType>("innerinjury", CombatSkillCommonTip2ValueRefers.ElementType.Two);
				break;
			case MouseTipCombatSkill.InjuryStrongShowType.Fatal:
				valueTuple = new ValueTuple<string, CombatSkillCommonTip2ValueRefers.ElementType>("fataldamage", CombatSkillCommonTip2ValueRefers.ElementType.One);
				break;
			case MouseTipCombatSkill.InjuryStrongShowType.Mind:
				valueTuple = new ValueTuple<string, CombatSkillCommonTip2ValueRefers.ElementType>("pinkyellow", CombatSkillCommonTip2ValueRefers.ElementType.One);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			ValueTuple<string, CombatSkillCommonTip2ValueRefers.ElementType> valueTuple2 = valueTuple;
			string colorName = valueTuple2.Item1;
			CombatSkillCommonTip2ValueRefers.ElementType elementType = valueTuple2.Item2;
			tip.SetValue(elementType, valueStr.SetColor(colorName));
		}
	}

	// Token: 0x0600297E RID: 10622 RVA: 0x001398B4 File Offset: 0x00137AB4
	private void RefreshRequirement(CombatSkillDisplayData displayData)
	{
		this._avatarInfoMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AvatarInfoMonitor>(displayData.CharId, false);
		CharacterItem config = Character.Instance[this._avatarInfoMonitor.TemplateId];
		this._fixedCombatPower = (config != null && config.FixCombatSkillPower != -1);
		bool flag = !this._fixedCombatPower;
		if (flag)
		{
			byte creatingType = this._avatarInfoMonitor.CreatingType;
			bool showSpecialBack = this.GetPropertyIsInSpecialBreaks(48, false);
			bool isNonEvolutionaryType = CreatingType.IsNonEvolutionaryType(creatingType);
			bool isGearMate = SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(this._avatarInfoMonitor.CharacterId);
			base.CGet<TipRequirement>("Requirement").RefreshRequirement(displayData.Requirements, !isGearMate && isNonEvolutionaryType, "", showSpecialBack, true);
		}
	}

	// Token: 0x0600297F RID: 10623 RVA: 0x00139974 File Offset: 0x00137B74
	private bool InitSpecialBreakEffect(bool isEquipEffect, MultiHorizontalLayoutGroup specialBreakEffectHolder, int startIndex)
	{
		bool showParent = false;
		bool flag = this._combatSkillDisplayData == null || this._combatSkillDisplayData.BreakAddProperty == null;
		bool result;
		if (flag)
		{
			result = showParent;
		}
		else
		{
			int index = startIndex;
			List<RectTransform> children = new List<RectTransform>();
			foreach (ValueTuple<short, short, bool> breakProperty in this._combatSkillDisplayData.BreakAddProperty)
			{
				bool flag2 = !breakProperty.Item3;
				if (!flag2)
				{
					bool flag3 = breakProperty.Item2 == 0;
					if (!flag3)
					{
						SpecialBreakProperty specialBreakProperty = default(SpecialBreakProperty);
						bool flag4 = (int)breakProperty.Item1 < CharacterPropertyReferenced.Instance.Count;
						if (flag4)
						{
							CharacterPropertyDisplayItem propertyConfig = CharacterPropertyDisplay.Instance[breakProperty.Item1];
							specialBreakProperty.isDisplaySpecially = propertyConfig.IsDisplaySpecially;
							specialBreakProperty.name = propertyConfig.Name;
							specialBreakProperty.isPercent = propertyConfig.IsPercent;
							specialBreakProperty.isInverse = propertyConfig.IsInverse;
							specialBreakProperty.displayFix = propertyConfig.DisplayFix;
							specialBreakProperty.isEquipEffect = true;
							specialBreakProperty.propertyId = breakProperty.Item1;
						}
						else
						{
							short propertyId = (short)((int)breakProperty.Item1 - CharacterPropertyReferenced.Instance.Count);
							CombatSkillPropertyItem propertyConfig2 = CombatSkillProperty.Instance[(int)propertyId];
							specialBreakProperty.isDisplaySpecially = propertyConfig2.IsDisplaySpecially;
							specialBreakProperty.name = propertyConfig2.Name;
							specialBreakProperty.isPercent = propertyConfig2.IsPercent;
							specialBreakProperty.isInverse = propertyConfig2.IsInverse;
							specialBreakProperty.displayFix = propertyConfig2.DisplayFix;
							specialBreakProperty.isEquipEffect = false;
							specialBreakProperty.propertyId = propertyId;
						}
						bool isDisplaySpecially = specialBreakProperty.isDisplaySpecially;
						if (!isDisplaySpecially)
						{
							bool flag5 = (isEquipEffect && (!specialBreakProperty.isEquipEffect || this._equipEffectMergeShowPropertyList.Contains(specialBreakProperty.propertyId))) || (!isEquipEffect && (specialBreakProperty.isEquipEffect || this._castEffectMergeShowPropertyList.Contains(specialBreakProperty.propertyId)));
							if (!flag5)
							{
								bool flag6 = index < specialBreakEffectHolder.childCount;
								TipsAddProperty addPropertyUi;
								if (flag6)
								{
									addPropertyUi = specialBreakEffectHolder.GetChild(index).GetComponent<TipsAddProperty>();
								}
								else
								{
									addPropertyUi = Object.Instantiate<TipsAddProperty>(base.CGet<TipsAddProperty>("AddProperty"));
								}
								children.Add(addPropertyUi.transform as RectTransform);
								if (isEquipEffect)
								{
									addPropertyUi.SetData(specialBreakProperty.propertyId, (int)breakProperty.Item2, false, specialBreakProperty.isPercent, false, true, false, false);
									showParent = true;
								}
								else
								{
									short propertyId2 = (short)((int)specialBreakProperty.propertyId + CharacterPropertyReferenced.Instance.Count);
									addPropertyUi.SetData(propertyId2, (int)breakProperty.Item2, false, specialBreakProperty.isPercent, specialBreakProperty.isInverse, true, false, false);
									showParent = true;
								}
								index++;
							}
						}
					}
				}
			}
			specialBreakEffectHolder.AddChildren(children, true);
			result = showParent;
		}
		return result;
	}

	// Token: 0x06002980 RID: 10624 RVA: 0x00139C74 File Offset: 0x00137E74
	private bool GetPropertyIsInSpecialBreaks(short propertyId, bool isEquipEffect)
	{
		bool flag = this._combatSkillDisplayData == null || this._combatSkillDisplayData.BreakAddProperty == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			for (int i = 0; i < this._combatSkillDisplayData.BreakAddProperty.Count; i++)
			{
				ValueTuple<short, short, bool> addProperty = this._combatSkillDisplayData.BreakAddProperty[i];
				bool flag2 = (int)addProperty.Item1 < CharacterPropertyReferenced.Instance.Count && isEquipEffect;
				if (flag2)
				{
					bool flag3 = addProperty.Item1 == propertyId && addProperty.Item3;
					if (flag3)
					{
						return true;
					}
				}
				else
				{
					bool flag4 = (int)addProperty.Item1 >= CharacterPropertyReferenced.Instance.Count && !isEquipEffect;
					if (flag4)
					{
						bool flag5 = (int)addProperty.Item1 - CharacterPropertyReferenced.Instance.Count == (int)propertyId && addProperty.Item3;
						if (flag5)
						{
							return true;
						}
					}
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x06002981 RID: 10625 RVA: 0x00139D68 File Offset: 0x00137F68
	private void UpdateEquipEffect()
	{
		List<PropertyAndValue> equipAddPropertyList = new List<PropertyAndValue>(this._configData.PropertyAddList);
		int poisonNum = 0;
		short allPosionValue = 0;
		List<PropertyAndValue> poisons = new List<PropertyAndValue>();
		for (int i = 0; i < equipAddPropertyList.Count; i++)
		{
			PropertyAndValue addProperty = equipAddPropertyList[i];
			CharacterPropertyDisplayItem characterPropertyDisplayItem = CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[addProperty.PropertyId].DisplayType];
			ECharacterPropertyDisplayType type = characterPropertyDisplayItem.Type;
			bool flag = type == ECharacterPropertyDisplayType.ResistOfHotPoison || type == ECharacterPropertyDisplayType.ResistOfColdPoison || type == ECharacterPropertyDisplayType.ResistOfGloomyPoison || type == ECharacterPropertyDisplayType.ResistOfRedPoison || type == ECharacterPropertyDisplayType.ResistOfRottenPoison || type == ECharacterPropertyDisplayType.ResistOfIllusoryPoison;
			if (flag)
			{
				poisonNum++;
				poisons.Add(addProperty);
				allPosionValue = addProperty.Value;
			}
		}
		bool allPoison = poisonNum >= 6;
		bool flag2 = allPoison;
		if (flag2)
		{
			for (int j = 0; j < poisons.Count; j++)
			{
				equipAddPropertyList.Remove(poisons[j]);
			}
			PropertyAndValue allPosionPropertyAndValue = new PropertyAndValue(10000, allPosionValue);
			equipAddPropertyList.Add(allPosionPropertyAndValue);
		}
		bool hasBreakPeoperty = this._combatSkillDisplayData.BreakAddProperty != null && this._combatSkillDisplayData.BreakAddProperty.Count > 0;
		Dictionary<short, ValueTuple<int, bool>> mergedProperties = this.MergeEquipProperties(equipAddPropertyList, hasBreakPeoperty);
		MultiHorizontalLayoutGroup equipEffectHolder = base.CGet<RectTransform>("EquipAddPropertyHolder").GetComponent<MultiHorizontalLayoutGroup>();
		equipEffectHolder.ResetData();
		int propertyCount = 0;
		bool flag3 = mergedProperties.Count > 0;
		if (flag3)
		{
			foreach (KeyValuePair<short, ValueTuple<int, bool>> kvp in mergedProperties)
			{
				short propertyId = kvp.Key;
				int totalValue = kvp.Value.Item1;
				bool hasSpecialBreak = kvp.Value.Item2;
				bool flag4 = propertyId == 10000;
				if (flag4)
				{
					bool flag5 = propertyCount < equipEffectHolder.childCount;
					TipsAddProperty addPropertyUi;
					if (flag5)
					{
						addPropertyUi = equipEffectHolder.GetChild(propertyCount).GetComponent<TipsAddProperty>();
					}
					else
					{
						addPropertyUi = Object.Instantiate<TipsAddProperty>(base.CGet<TipsAddProperty>("AddProperty"));
						addPropertyUi.transform.localScale = Vector3.one;
					}
					equipEffectHolder.AddChild(addPropertyUi.transform as RectTransform, hasSpecialBreak);
					CharacterPropertyReferencedItem propertyReferencedItem = CharacterPropertyReferenced.Instance[28];
					CharacterPropertyDisplayItem propertyDisplayItem = CharacterPropertyDisplay.Instance[propertyReferencedItem.DisplayType];
					addPropertyUi.SetData("mousetip_duxing_big_all", LocalStringManager.Get(LanguageKey.LK_CombatSkill_AllPoison), totalValue, false, propertyDisplayItem.IsPercent, false, true, false);
					addPropertyUi.gameObject.SetActive(true);
					propertyCount++;
				}
				else
				{
					bool isPercent = CharacterPropertyDisplay.Instance[CharacterPropertyReferenced.Instance[propertyId].DisplayType].IsPercent;
					MouseTip_Util.AppendAddProperty(equipEffectHolder, base.CGet<TipsAddProperty>("AddProperty"), propertyCount, propertyId, totalValue, false, isPercent, false, true, hasSpecialBreak);
					propertyCount++;
				}
			}
		}
		bool isSpecialShow = this.InitSpecialBreakEffect(true, equipEffectHolder, propertyCount);
		this.UpdateNeiliAllocationAddProperty(this._combatSkillDisplayData.NeiliAllocationAddProperty);
		GameObject gameObject = base.CGet<GameObject>("EquipEffect");
		List<ValueTuple<short, int>> neiliAllocationAddProperty = this._combatSkillDisplayData.NeiliAllocationAddProperty;
		gameObject.SetActive((neiliAllocationAddProperty != null && neiliAllocationAddProperty.Count > 0) || propertyCount > 0 || isSpecialShow);
	}

	// Token: 0x06002982 RID: 10626 RVA: 0x0013A0CC File Offset: 0x001382CC
	[return: TupleElementNames(new string[]
	{
		"totalValue",
		"hasSpecialBreak"
	})]
	private Dictionary<short, ValueTuple<int, bool>> MergeEquipProperties(List<PropertyAndValue> equipAddPropertyList, bool hasBreakProperty)
	{
		Dictionary<short, ValueTuple<int, bool>> mergedProperties = new Dictionary<short, ValueTuple<int, bool>>();
		foreach (PropertyAndValue property in equipAddPropertyList)
		{
			int value = (int)(property.Value * this.GetDisplayDataPower(this._combatSkillDisplayData) / 100);
			bool flag = mergedProperties.ContainsKey(property.PropertyId);
			if (flag)
			{
				mergedProperties[property.PropertyId] = new ValueTuple<int, bool>(mergedProperties[property.PropertyId].Item1 + value, mergedProperties[property.PropertyId].Item2);
			}
			else
			{
				mergedProperties[property.PropertyId] = new ValueTuple<int, bool>(value, false);
			}
		}
		this._equipEffectMergeShowPropertyList.Clear();
		if (hasBreakProperty)
		{
			foreach (ValueTuple<short, short, bool> addProperty in this._combatSkillDisplayData.BreakAddProperty)
			{
				bool flag2 = (int)addProperty.Item1 >= CharacterPropertyReferenced.Instance.Count || addProperty.Item2 == 0;
				if (!flag2)
				{
					bool isSpecialBreak = addProperty.Item3;
					bool flag3 = mergedProperties.ContainsKey(addProperty.Item1);
					if (flag3)
					{
						mergedProperties[addProperty.Item1] = new ValueTuple<int, bool>(mergedProperties[addProperty.Item1].Item1 + (int)addProperty.Item2, mergedProperties[addProperty.Item1].Item2 || isSpecialBreak);
					}
					else
					{
						mergedProperties[addProperty.Item1] = new ValueTuple<int, bool>((int)addProperty.Item2, isSpecialBreak);
					}
					bool flag4 = isSpecialBreak;
					if (flag4)
					{
						this._equipEffectMergeShowPropertyList.Add(addProperty.Item1);
					}
				}
			}
		}
		return mergedProperties;
	}

	// Token: 0x06002983 RID: 10627 RVA: 0x0013A2D0 File Offset: 0x001384D0
	private string FormatCostMobility(int costMobilityValue)
	{
		bool flag = costMobilityValue <= 0;
		string result;
		if (flag)
		{
			result = "0";
		}
		else
		{
			result = ((float)costMobilityValue * 100f / (float)MoveSpecialConstants.MaxMobility).ToString("F2");
		}
		return result;
	}

	// Token: 0x06002984 RID: 10628 RVA: 0x0013A314 File Offset: 0x00138514
	private string FormatJumpSpeed(int prepareFrame, int speed)
	{
		bool flag = prepareFrame <= 0 || speed <= 0;
		string result;
		if (flag)
		{
			result = string.Empty;
		}
		else
		{
			result = LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_JumpPrepareFrame, ((float)prepareFrame / (float)speed / 60f).ToString("F2"));
		}
		return result;
	}

	// Token: 0x06002985 RID: 10629 RVA: 0x0013A364 File Offset: 0x00138564
	private void RefreshBounce(short bounceDistance, int bouncePowerOuter, int bouncePowerInner, bool showBack)
	{
		bool showBounce = this._configData.BounceRateOfOuterInjury > 0 || this._configData.BounceRateOfInnerInjury > 0;
		bool flag = showBounce;
		if (flag)
		{
			this._strBuilder.Clear();
			this._strBuilder.Append(LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_Bounce_Range, string.Format("2.0-{0:f1}", (float)bounceDistance / 10f)));
			base.CGet<TextMeshProUGUI>("BounceRange").text = this._strBuilder.ToString().ColorReplace();
			base.CGet<GameObject>("BounceRangeSpecialBack").SetActive(this.GetPropertyIsInSpecialBreaks(27, false) && showBack);
			this._strBuilder.Clear();
			TextMeshProUGUI bounceOuter = base.CGet<TextMeshProUGUI>("BounceOuter");
			bounceOuter.transform.parent.gameObject.SetActive(this._configData.BounceRateOfOuterInjury > 0);
			bool flag2 = this._configData.BounceRateOfOuterInjury > 0;
			if (flag2)
			{
				this._strBuilder.Append(LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_Bounce_Outer, bouncePowerOuter.ToString()) ?? "");
				bounceOuter.text = this._strBuilder.ToString().ColorReplace();
				base.CGet<GameObject>("BounceOuterSpecialBack").SetActive(this.GetPropertyIsInSpecialBreaks(25, false) && showBack);
			}
			this._strBuilder.Clear();
			TextMeshProUGUI bounceInner = base.CGet<TextMeshProUGUI>("BounceInner");
			bounceInner.transform.parent.gameObject.SetActive(this._configData.BounceRateOfInnerInjury > 0);
			bool flag3 = this._configData.BounceRateOfInnerInjury > 0;
			if (flag3)
			{
				this._strBuilder.Append(LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_Bounce_Inner, bouncePowerInner.ToString()) ?? "");
				bounceInner.text = this._strBuilder.ToString().ColorReplace();
				base.CGet<GameObject>("BounceInnerSpecialBack").SetActive(this.GetPropertyIsInSpecialBreaks(26, false) && showBack);
			}
		}
		base.CGet<GameObject>("BounceBackRoot").SetActive(showBounce);
	}

	// Token: 0x06002986 RID: 10630 RVA: 0x0013A580 File Offset: 0x00138780
	private void RefreshFightBack(int fightbackPower, bool showBack)
	{
		TextMeshProUGUI fightBackText = base.CGet<TextMeshProUGUI>("Fightback");
		bool showFightBack = this._configData.FightBackDamage > 0;
		bool flag = showFightBack;
		if (flag)
		{
			this._strBuilder.Clear();
			this._strBuilder.Append(LocalStringManager.GetFormat(LanguageKey.LK_CombatSkill_FightBack_Desc, this._defendSkillFightbackTypeStr, fightbackPower) ?? "");
			fightBackText.text = this._strBuilder.ToString().ColorReplace();
			base.CGet<GameObject>("FightBackSpecialBack").SetActive(this.GetPropertyIsInSpecialBreaks(24, false) && showBack);
		}
		base.CGet<GameObject>("FightBackBackRoot").SetActive(showFightBack);
	}

	// Token: 0x06002987 RID: 10631 RVA: 0x0013A62C File Offset: 0x0013882C
	private void RefreshEffectDuration(short effectDuration, bool showBack)
	{
		base.CGet<TextMeshProUGUI>("EffectDuration").text = ((float)effectDuration / 60f).ToString("f1");
		base.CGet<GameObject>("EffectDurationSpecialBack").SetActive(this.GetPropertyIsInSpecialBreaks(28, false) && showBack);
	}

	// Token: 0x06002988 RID: 10632 RVA: 0x0013A67C File Offset: 0x0013887C
	private void HideAllSpecialBack()
	{
		base.CGet<GameObject>("AttackSpecialBack").SetActive(false);
		base.CGet<GameObject>("AgileSpecialBack").SetActive(false);
		base.CGet<GameObject>("DefenceSpecialBack").SetActive(false);
		base.CGet<GameObject>("SpecialSpecialBack").SetActive(false);
		base.CGet<GameObject>("GenericSpecialBack").SetActive(false);
		base.CGet<GameObject>("CostMobilitySpecialBack").SetActive(false);
		base.CGet<GameObject>("CostBreathStanceSpecialBack").SetActive(false);
		base.CGet<GameObject>("BounceOuterSpecialBack").SetActive(false);
		base.CGet<GameObject>("BounceInnerSpecialBack").SetActive(false);
		base.CGet<GameObject>("FightBackSpecialBack").SetActive(false);
		base.CGet<GameObject>("PenetrateSpecialBack").SetActive(false);
		base.CGet<GameObject>("MaxPowerSpecialBack").SetActive(false);
		base.CGet<GameObject>("RangeBackSpecialBack").SetActive(false);
		base.CGet<GameObject>("HitRateSpeedSpecialBack").SetActive(false);
		base.CGet<GameObject>("HitRateTechniqueSpecialBack").SetActive(false);
		base.CGet<GameObject>("EffectDurationSpecialBack").SetActive(false);
		base.CGet<GameObject>("CostTrickSpecialBack").SetActive(false);
		base.CGet<GameObject>("SpeedSpecialBack").SetActive(false);
		base.CGet<GameObject>("TechniqueSpecialBack").SetActive(false);
		base.CGet<GameObject>("StrengthSpecialBack").SetActive(false);
		base.CGet<GameObject>("MindSpecialBack").SetActive(false);
		base.CGet<GameObject>("HitRateMindSpecialBack").SetActive(false);
	}

	// Token: 0x06002989 RID: 10633 RVA: 0x0013A818 File Offset: 0x00138A18
	private void RefreshAccessoryPoison()
	{
		this._accessoryPoisonLayout = base.CGet<RectTransform>("AccessoryPoisonLayout");
		bool flag = this._configData.EquipType == 1 && UIManager.Instance.IsElementActive(UIElement.Combat);
		if (flag)
		{
			this._equipmentMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<EquipmentMonitor>(this._charId, false);
			this._equipmentMonitor.AddEquipmentChangeListener(new Action<sbyte>(this.OnEquipmentChanged));
			bool init = this._equipmentMonitor.Init;
			if (init)
			{
				this._equipmentMonitor.OnDataInit();
			}
		}
		else
		{
			this._accessoryPoisonHolder.SetActive(false);
		}
		this._accessoryViewRect = this._accessoryPoisonHolder.GetComponent<RectTransform>();
		this.SetAccessoryViewRight();
		this.TipAdditionSizeX = (this._accessoryViewRect.gameObject.activeSelf ? this._accessoryViewRect.sizeDelta.x : 0f);
		this.OnUpdatePos = delegate(bool showOnLeft)
		{
			bool flag2 = !this._accessoryViewRect.gameObject.activeSelf;
			if (!flag2)
			{
				if (showOnLeft)
				{
					this.SetAccessoryViewLeft();
				}
				else
				{
					this.SetAccessoryViewRight();
				}
			}
		};
	}

	// Token: 0x0600298A RID: 10634 RVA: 0x0013A914 File Offset: 0x00138B14
	private void OnEquipmentChanged(sbyte equipmentSlot = 0)
	{
		List<ItemKey> keyList = new List<ItemKey>();
		List<ItemDisplayData> dataList = new List<ItemDisplayData>();
		for (int index = 0; index < this._equipmentMonitor.Equipment.Length; index++)
		{
			bool flag = index == 8 || index == 9 || index == 10;
			if (flag)
			{
				ItemKey itemKey = this._equipmentMonitor.Equipment[index];
				bool flag2 = itemKey.IsValid();
				if (flag2)
				{
					keyList.Add(itemKey);
				}
			}
		}
		ItemDomainMethod.AsyncCall.GetItemDisplayDataList(this, keyList, delegate(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref dataList);
			this.RefreshAccessoryPoisonContent(dataList);
		});
	}

	// Token: 0x0600298B RID: 10635 RVA: 0x0013A9B4 File Offset: 0x00138BB4
	private unsafe void RefreshAccessoryPoisonContent(List<ItemDisplayData> dataList)
	{
		bool flag = !this._accessoryPoisonLayout || !this._accessoryPoisonHolder || this._isSimple;
		if (!flag)
		{
			bool showAccessoryPoison = false;
			for (int i = 0; i < dataList.Count; i++)
			{
				ItemDisplayData itemData = dataList[i];
				Refers accessoryRefers = this._accessoryPoisonLayout.GetChild(i).GetComponent<Refers>();
				accessoryRefers.gameObject.SetActive(itemData.HasAnyPoison);
				bool hasAnyPoison = itemData.HasAnyPoison;
				if (hasAnyPoison)
				{
					showAccessoryPoison = true;
				}
				sbyte grade = ItemTemplateHelper.GetGrade(itemData.Key.ItemType, itemData.Key.TemplateId);
				accessoryRefers.CGet<CImage>("GradeBack").SetSprite(ItemView.GetGradeIcon(grade), false, null);
				accessoryRefers.CGet<TextMeshProUGUI>("Grade").text = ItemView.GetGradeText(grade);
				string text = ItemTemplateHelper.GetName(itemData.Key.ItemType, itemData.Key.TemplateId).SetColor(Colors.Instance.GradeColors[(int)grade]);
				accessoryRefers.CGet<TextMeshProUGUI>("Name").text = text;
				Refers attachedPoisons = accessoryRefers.CGet<Refers>("AttachedPoisons");
				RectTransform poisonHolder = attachedPoisons.CGet<RectTransform>("PoisonHolder");
				FullPoisonEffects poisonEffects = itemData.PoisonEffects;
				PoisonsAndLevels innatePoisons = (poisonEffects != null) ? poisonEffects.GetAllPoisonsAndLevels() : default(PoisonsAndLevels);
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
			}
			this._accessoryPoisonHolder.SetActive(showAccessoryPoison);
			for (int j = dataList.Count; j < this._accessoryPoisonLayout.childCount; j++)
			{
				this._accessoryPoisonLayout.GetChild(j).gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600298C RID: 10636 RVA: 0x0013AC5C File Offset: 0x00138E5C
	private void SetAccessoryViewRight()
	{
		this._accessoryViewRect.pivot = Vector2.up;
		this._accessoryViewRect.anchorMin = Vector2.one;
		this._accessoryViewRect.anchorMax = Vector2.one;
		this._accessoryViewRect.anchoredPosition = new Vector2(4f, -50f);
	}

	// Token: 0x0600298D RID: 10637 RVA: 0x0013ACB8 File Offset: 0x00138EB8
	private void SetAccessoryViewLeft()
	{
		this._accessoryViewRect.pivot = Vector2.one;
		this._accessoryViewRect.anchorMin = Vector2.up;
		this._accessoryViewRect.anchorMax = Vector2.up;
		this._accessoryViewRect.anchoredPosition = new Vector2(-4f, -50f);
	}

	// Token: 0x04001E06 RID: 7686
	public const int AttackSkillPowerGridGroupCount = 5;

	// Token: 0x04001E07 RID: 7687
	public const int FirstStickParticalPos = -3000;

	// Token: 0x04001E08 RID: 7688
	public const int StickParticalPosInterval = -100;

	// Token: 0x04001E09 RID: 7689
	private static int _stickedCount = 0;

	// Token: 0x04001E0A RID: 7690
	private int _stickedIndex = -1;

	// Token: 0x04001E0B RID: 7691
	private short _combatSkillTemplateId;

	// Token: 0x04001E0C RID: 7692
	private int _charId;

	// Token: 0x04001E0D RID: 7693
	private CombatSkillItem _configData;

	// Token: 0x04001E0E RID: 7694
	private bool _showOnlyTemplateInfo;

	// Token: 0x04001E0F RID: 7695
	private bool _isTaiwuAttackSkill;

	// Token: 0x04001E10 RID: 7696
	private string _defendSkillFightbackTypeStr;

	// Token: 0x04001E11 RID: 7697
	private readonly StringBuilder _strBuilder = new StringBuilder();

	// Token: 0x04001E12 RID: 7698
	private Refers _costTrickPrefab;

	// Token: 0x04001E13 RID: 7699
	private CombatSkillCommonTipRefers _commonTipRefers;

	// Token: 0x04001E14 RID: 7700
	private CombatSkillCommonTip2ValueRefers _commonTip2ValueRefers;

	// Token: 0x04001E15 RID: 7701
	private AvatarInfoMonitor _avatarInfoMonitor;

	// Token: 0x04001E16 RID: 7702
	private CombatSkillDisplayData _combatSkillDisplayData;

	// Token: 0x04001E17 RID: 7703
	private List<short> _equipEffectMergeShowPropertyList = new List<short>();

	// Token: 0x04001E18 RID: 7704
	private List<short> _castEffectMergeShowPropertyList = new List<short>();

	// Token: 0x04001E19 RID: 7705
	private bool _fixedCombatPower = false;

	// Token: 0x04001E1A RID: 7706
	private static readonly string[] CombatSkillPartIcon = new string[]
	{
		"mousetip_buwei_0",
		"mousetip_buwei_1",
		"mousetip_buwei_2",
		"mousetip_buwei_3",
		"mousetip_buwei_4",
		"mousetip_buwei_5",
		"mousetip_buwei_6"
	};

	// Token: 0x04001E1B RID: 7707
	private EquipmentMonitor _equipmentMonitor;

	// Token: 0x04001E1C RID: 7708
	private RectTransform _accessoryViewRect;

	// Token: 0x04001E1D RID: 7709
	private GameObject _accessoryPoisonHolder;

	// Token: 0x04001E1E RID: 7710
	private RectTransform _accessoryPoisonLayout;

	// Token: 0x04001E1F RID: 7711
	private bool _isSimple;

	// Token: 0x04001E20 RID: 7712
	private bool _altDownWhenNotSticked;

	// Token: 0x04001E21 RID: 7713
	private bool _lastAltDownState;

	// Token: 0x020015F6 RID: 5622
	public enum InjuryStrongShowType
	{
		// Token: 0x0400A68B RID: 42635
		Outer,
		// Token: 0x0400A68C RID: 42636
		Inner,
		// Token: 0x0400A68D RID: 42637
		Fatal,
		// Token: 0x0400A68E RID: 42638
		Mind
	}
}
