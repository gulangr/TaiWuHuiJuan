using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using FrameWork;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000080 RID: 128
public class TooltipManager : ISingletonInit, IDisposable
{
	// Token: 0x14000006 RID: 6
	// (add) Token: 0x060004AC RID: 1196 RVA: 0x0001F8F4 File Offset: 0x0001DAF4
	// (remove) Token: 0x060004AD RID: 1197 RVA: 0x0001F92C File Offset: 0x0001DB2C
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<TipType, int> OnShowingTips;

	// Token: 0x14000007 RID: 7
	// (add) Token: 0x060004AE RID: 1198 RVA: 0x0001F964 File Offset: 0x0001DB64
	// (remove) Token: 0x060004AF RID: 1199 RVA: 0x0001F99C File Offset: 0x0001DB9C
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<TipType> OnHidingTips;

	// Token: 0x060004B0 RID: 1200 RVA: 0x0001F9D4 File Offset: 0x0001DBD4
	public void Init()
	{
		this._uiElementDict[TipType.SingleDesc] = UIElement.MouseTipSingleDesc;
		this._uiElementDict[TipType.Simple] = UIElement.MouseTipSimple;
		this._uiElementDict[TipType.CombatSkill] = UIElement.MouseTipCombatSkill;
		this._uiElementDict[TipType.Weapon] = UIElement.MouseTipWeapon;
		this._uiElementDict[TipType.SkillBook] = UIElement.MouseTipBook;
		this._uiElementDict[TipType.ReadingBook] = UIElement.MouseTipReading;
		this._uiElementDict[TipType.CraftTool] = UIElement.MouseTipCraftTool;
		this._uiElementDict[TipType.Material] = UIElement.MouseTipMaterial;
		this._uiElementDict[TipType.Cricket] = UIElement.MouseTipCricket;
		this._uiElementDict[TipType.Armor] = UIElement.MouseTipArmor;
		this._uiElementDict[TipType.Carrier] = UIElement.MouseTipCarrier;
		this._uiElementDict[TipType.Clothing] = UIElement.MouseTipClothing;
		this._uiElementDict[TipType.Food] = UIElement.MouseTipFood;
		this._uiElementDict[TipType.Medicine] = UIElement.MouseTipMedicine;
		this._uiElementDict[TipType.Misc] = UIElement.MouseTipMisc;
		this._uiElementDict[TipType.TeaWine] = UIElement.MouseTipTeaWine;
		this._uiElementDict[TipType.Accessory] = UIElement.MouseTipAccessory;
		this._uiElementDict[TipType.LifeRecords] = UIElement.MouseTipLifeRecords;
		this._uiElementDict[TipType.Character] = UIElement.MouseTipCharacter;
		this._uiElementDict[TipType.Resource] = UIElement.MouseTipResource;
		this._uiElementDict[TipType.ResourceHolder] = UIElement.MouseTipResourceHolder;
		this._uiElementDict[TipType.EatingItems] = UIElement.MouseTipEatingItems;
		this._uiElementDict[TipType.MapBlock] = UIElement.MouseTipMapBlock;
		this._uiElementDict[TipType.Feature] = UIElement.MouseTipFeature;
		this._uiElementDict[TipType.MartialArtTournament] = UIElement.MouseTipMartialArtTournament;
		this._uiElementDict[TipType.SimpleWide] = UIElement.MouseTipSimpleWide;
		this._uiElementDict[TipType.MakeItem] = UIElement.MouseTipMakeItem;
		this._uiElementDict[TipType.InnateFiveElements] = UIElement.MouseTipInnateFiveElements;
		this._uiElementDict[TipType.DisassembleItem] = UIElement.MouseTipDisassembleItem;
		this._uiElementDict[TipType.RepairItem] = UIElement.MouseTipRepairItem;
		this._uiElementDict[TipType.SecretInformation] = UIElement.MouseTipSecretInformation;
		this._uiElementDict[TipType.LifeCombatSkillValue] = UIElement.MouseTipLifeCombatSkillValue;
		this._uiElementDict[TipType.BuildingShowItem] = UIElement.MouseTipBuildingShowItem;
		this._uiElementDict[TipType.BuildingShowRecruitPeople] = UIElement.MouseTipBuildingShowRecruitPeople;
		this._uiElementDict[TipType.SecretInformationBroadcastNotify] = UIElement.MouseTipSecretInformationBroadcastNotify;
		this._uiElementDict[TipType.LegendaryBookBonus] = UIElement.MouseTipLegendaryBookBonus;
		this._uiElementDict[TipType.ProfessionSkill] = UIElement.MouseTipProfessionSkill;
		this._uiElementDict[TipType.Profession] = UIElement.MouseTipProfession;
		this._uiElementDict[TipType.AdventureNode] = UIElement.MouseTipAdventureNode;
		this._uiElementDict[TipType.Injury] = UIElement.MouseTipInjury;
		this._uiElementDict[TipType.MapArea] = UIElement.MouseTipMapArea;
		this._uiElementDict[TipType.AttachedPoison] = UIElement.MouseTipAttachedPoison;
		this._uiElementDict[TipType.MixPoison] = UIElement.MouseTipMixPoison;
		this._uiElementDict[TipType.Adventure] = UIElement.MouseTipAdventure;
		this._uiElementDict[TipType.CharacterPoison] = UIElement.MouseTipCharacterPoison;
		this._uiElementDict[TipType.LifeSkillValue] = UIElement.MouseTipLifeSkillValue;
		this._uiElementDict[TipType.CombatSkillValue] = UIElement.MouseTipCombatSkillValue;
		this._uiElementDict[TipType.CombatSkillPractice] = UIElement.MouseTipCombatSkillPractice;
		this._uiElementDict[TipType.CombatSkillBanReason] = UIElement.MouseTipCombatSkillBanReason;
		this._uiElementDict[TipType.BodyPart] = UIElement.MouseTipBodyPart;
		this._uiElementDict[TipType.Fold] = UIElement.MouseTipFold;
		this._uiElementDict[TipType.MonthNotify] = UIElement.MouseTipMonthNotify;
		this._uiElementDict[TipType.CombatSkillBreakout] = UIElement.CombatSkillBreakout;
		this._uiElementDict[TipType.Flaw] = UIElement.MouseTipFlaw;
		this._uiElementDict[TipType.Encyclopedia] = UIElement.MouseTipEncyclopedia;
		this._uiElementDict[TipType.CharacterComplete] = UIElement.MouseTipCharacterComplete;
		this._uiElementDict[TipType.CombatChangeTrick] = UIElement.MouseTipCombatChangeTrick;
		this._uiElementDict[TipType.Advance] = UIElement.MouseTipAdvance;
		this._uiElementDict[TipType.TrickType] = UIElement.MouseTipTrickType;
		this._uiElementDict[TipType.UpgradeTeammateCommand] = UIElement.MouseTipUpgradeTeammateCommand;
		this._uiElementDict[TipType.FiveElements] = UIElement.MouseTipFiveElements;
		this._uiElementDict[TipType.NeiliAllocation] = UIElement.MouseTipNeiliAllocation;
		this._uiElementDict[TipType.Music] = UIElement.MouseTipMusic;
		this._uiElementDict[TipType.ReadingEvent] = UIElement.MouseTipReadingEvent;
		this._uiElementDict[TipType.Legacy] = UIElement.MouseTipLegacy;
		this._uiElementDict[TipType.Fuyu] = UIElement.MouseTipFuyu;
		this._uiElementDict[TipType.DynamicCondition] = UIElement.MouseTipDynamicCondition;
		this._uiElementDict[TipType.Jiao] = UIElement.MouseTipJiao;
		this._uiElementDict[TipType.JiaoEgg] = UIElement.MouseTipJiaoEgg;
		this._uiElementDict[TipType.loongDebuff] = UIElement.MouseTipLoongDebuff;
		this._uiElementDict[TipType.JiaoNurturance] = UIElement.MouseTipJiaoNurturance;
		this._uiElementDict[TipType.CombatSkillBuff] = UIElement.MouseTipCombatSkillBuff;
		this._uiElementDict[TipType.GeneralLines] = UIElement.MouseTipGeneralLines;
		this._uiElementDict[TipType.MixPoisonEffectSimple] = UIElement.MouseTipMixPoisonEffectSimple;
		this._uiElementDict[TipType.MixPoisonEffectDetailed] = UIElement.MouseTipMixPoisonEffectDetailed;
		this._uiElementDict[TipType.MixPoisonEffectOutCombat] = UIElement.MouseTipMixPoisonEffectOutCombat;
		this._uiElementDict[TipType.DisorderOfQi] = UIElement.MouseTipDisorderOfQi;
		this._uiElementDict[TipType.MakeWugKing] = UIElement.MouseTipMakeWugKing;
		this._uiElementDict[TipType.EmptyContainer] = UIElement.MouseTipEmptyContainer;
		this._uiElementDict[TipType.BuildingProduce] = UIElement.MouseTipBuildingProduce;
		this._uiElementDict[TipType.BuildingProduceCollectResource] = UIElement.MouseTipBuildingProduceCollectResource;
		this._uiElementDict[TipType.EatingWug] = UIElement.MouseTipEatingWug;
		this._uiElementDict[TipType.BuildingRequireCultureSafety] = UIElement.MouseTipBuildingRequireCultureSafety;
		this._uiElementDict[TipType.ChangeTrick] = UIElement.MouseTipChangeTrick;
		this._uiElementDict[TipType.CombatBlockAttack] = UIElement.MouseTipCombatBlockAttack;
		this._uiElementDict[TipType.CombatBannedList] = UIElement.MouseTipCombatBannedList;
		this._uiElementDict[TipType.EquipLoad] = UIElement.MouseTipEquipLoad;
		this._uiElementDict[TipType.DefeatMark] = UIElement.MouseTipDefeatMark;
		this._uiElementDict[TipType.DamageValue] = UIElement.MouseTipDamageValue;
		this._uiElementDict[TipType.Destiny] = UIElement.MouseTipDestiny;
		this._uiElementDict[TipType.SettlementTreasury] = UIElement.MouseTipSettlementTreasury;
		this._uiElementDict[TipType.LoopingEvent] = UIElement.MouseTipLoopingEvent;
		this._uiElementDict[TipType.BuildingLevel] = UIElement.MouseTipBuildingLevel;
		this._uiElementDict[TipType.LegendaryBookGiveUp] = UIElement.LegendaryBookGiveUp;
		this._uiElementDict[TipType.LifeLinkNeiliType] = UIElement.MouseTipLifeLinkNeiliType;
		this._uiElementDict[TipType.ActiveRead] = UIElement.MouseTipActiveRead;
		this._uiElementDict[TipType.ActiveLoop] = UIElement.MouseTipActiveLoop;
		this._uiElementDict[TipType.ReadProgress] = UIElement.MouseTipReadProgress;
		this._uiElementDict[TipType.LoopProgress] = UIElement.MouseTipLoopProgress;
		this._uiElementDict[TipType.CharacterOnMapBlock] = UIElement.MouseTipCharacterOnMapBlock;
		this._uiElementDict[TipType.TeammateCommand] = UIElement.MouseTipTeammateCommand;
		this._uiElementDict[TipType.FeatureMedalLegacy] = UIElement.MouseTipFeatureMedalLegacy;
		this._uiElementDict[TipType.SimpleWithHotkeyDisplay] = UIElement.MouseTipSimpleWithHotkeyDisplay;
		this._uiElementDict[TipType.FulongFlame] = UIElement.MouseTipFulongFlame;
		this._uiElementDict[TipType.VillagerRoleAvailableCount] = UIElement.MouseTipVillagerRoleAvailableCount;
		this._uiElementDict[TipType.VillagerRoleEffect] = UIElement.MouseTipVillagerRoleEffect;
		this._uiElementDict[TipType.CombatRawCreate] = UIElement.MouseTipCombatRawCreate;
		this._uiElementDict[TipType.CombatUnlockProgress] = UIElement.MouseTipCombatUnlockProgress;
		this._uiElementDict[TipType.CombatWeaponUnlock] = UIElement.MouseTipCombatWeaponUnlock;
		this._uiElementDict[TipType.MouseTipGearMateUpgradeAttribute] = UIElement.MouseTipGearMateUpgradeAttribute;
		this._uiElementDict[TipType.MouseTipGearMateUpgradeFeature] = UIElement.MouseTipGearMateUpgradeFeature;
		this._uiElementDict[TipType.CaravanOperation] = UIElement.MouseTipCaravanOperation;
		this._uiElementDict[TipType.TaiwuWanted] = UIElement.MouseTipTaiwuWanted;
		this._uiElementDict[TipType.CaravanPath] = UIElement.MouseTipCaravanPath;
		this._uiElementDict[TipType.ExtraProfessionSkill] = UIElement.MouseTipExtraProfessionSkill;
		this._uiElementDict[TipType.Organization] = UIElement.MouseTipOrganization;
		this._uiElementDict[TipType.VillagerNeedItem] = UIElement.MouseTipVillagerNeedItem;
		this._uiElementDict[TipType.InteractCheckResultPhase] = UIElement.MouseTipInteractCheckResult;
		this._uiElementDict[TipType.NormalInformationType] = UIElement.MouseTipNormalInformationType;
		this._uiElementDict[TipType.ExpCheck] = UIElement.MouseTipExpCheck;
		this._uiElementDict[TipType.CombatSkillBreakInfo] = UIElement.MouseTipCombatSkillBreakInfo;
		this._uiElementDict[TipType.CombatSkillBonus] = UIElement.MouseTipCombatSkillBonus;
		this._uiElementDict[TipType.CombatSkillOneBonus] = UIElement.MouseTipCombatSkillOneBonus;
		this._uiElementDict[TipType.DemonSlayer] = UIElement.MouseTipDemonSlayer;
		this._uiElementDict[TipType.LegacyLevel] = UIElement.MouseTipLegacyLevel;
		this._uiElementDict[TipType.SkillBreakBonus] = UIElement.MouseTipSkillBreakBonus;
		this._uiElementDict[TipType.SkillBreakNormalCell] = UIElement.MouseTipSkillBreakNormalCell;
		this._uiElementDict[TipType.CombatInjuryChange] = UIElement.MouseTipCombatInjuryChange;
		this._uiElementDict[TipType.SectStory] = UIElement.MouseTipSectStory;
		this._uiElementDict[TipType.RecordIncompatible] = UIElement.MouseTipRecordIncompatible;
		this._uiElementDict[TipType.TeammateCount] = UIElement.MouseTipTeammateCount;
		this._uiElementDict[TipType.LifeSkillDetailReadProgress] = UIElement.MouseTipLifeSkillDetailReadProgress;
		this._uiElementDict[TipType.LifeSkillDetailUnlockBuilding] = UIElement.MouseTipLifeSkillDetailUnlockBuilding;
		this._uiElementDict[TipType.LifeSkillDetailUnlockInformation] = UIElement.MouseTipLifeSkillDetailUnlockInformation;
		this._uiElementDict[TipType.LifeSkillDetailUnlockStrategy] = UIElement.MouseTipLifeSkillDetailUnlockStrategy;
		this._uiElementDict[TipType.LifeSkillCombatCardType] = UIElement.MouseTipLifeSkillCombatCardType;
		this._uiElementDict[TipType.LifeSkillCombatUnit] = UIElement.MouseTipLifeSkillCombatUnit;
		this._uiElementDict[TipType.LifeSkillCombatStrategy] = UIElement.MouseTipLifeSkillCombatStrategy;
		this._uiElementDict[TipType.LifeSkillCombatStress] = UIElement.MouseTipLifeSkillCombatStress;
		this._uiElementDict[TipType.SimpleList] = UIElement.MouseTipSimpleList;
		this._uiElementDict[TipType.LifeSkillCombatBlock] = UIElement.MouseTipLifeSkillCombatBlock;
		this._uiElementDict[TipType.MatchVillagerRole] = UIElement.MouseTipMatchVillagerRole;
		this._uiElementDict[TipType.BuildingTeachBook] = UIElement.BuildingTeachBook;
		this._uiElementDict[TipType.TaiwuVillageStele] = UIElement.MouseTipTaiwuVillageStele;
		this._uiElementDict[TipType.WorkingStatus] = UIElement.WorkingStatus;
		this._uiElementDict[TipType.LifeSkillCombatFirstMove] = UIElement.MouseTipLifeSkillCombatFirstMove;
		this._uiElementDict[TipType.LifeSkillCombatLastMove] = UIElement.MouseTipLifeSkillCombatLastMove;
		this._uiElementDict[TipType.LifeSkillCombatAudience] = UIElement.MouseTipLifeSkillCombatAudience;
		this._uiElementDict[TipType.ThreeVitals] = UIElement.MouseTipThreeVitals;
		this._uiElementDict[TipType.PrisonerResistance] = UIElement.MouseTipPrisonerResistance;
		this._uiElementDict[TipType.SettlementTreasuryOrPrisonLayer] = UIElement.MouseTipSettlementTreasuryOrPrisonLayer;
		this._uiElementDict[TipType.CaravanPathDetail] = UIElement.MouseTipCaravanPathDetail;
		this._uiElementDict[TipType.BuildingFeast] = UIElement.MouseTipBuildingFeast;
		this._uiElementDict[TipType.SpecialBuild] = UIElement.MouseTipSpecialBuild;
		this._uiElementDict[TipType.DeadCharacterComplete] = UIElement.MouseTipDeadCharacterComplete;
		this._uiElementDict[TipType.JieqingInteractCharTips] = UIElement.MouseTipTargetStarFortune;
		this._uiElementDict[TipType.CricketEncyclopedia] = UIElement.MouseTipCricketEncyclopedia;
		this._uiElementDict[TipType.ProfessionEncyclopedia] = UIElement.MouseTipProfessionEncyclopedia;
		this._uiElementDict[TipType.ProfessionSkillEncyclopedia] = UIElement.MouseTipProfessionSkillEncyclopedia;
		this._uiElementDict[TipType.FeatureMedal] = UIElement.MouseTipFeatureMedal;
		this._uiElementDict[TipType.Alertness] = UIElement.MouseTipAlertness;
		this._uiElementDict[TipType.BuildingBlock] = UIElement.MouseTipBuildingBlock;
		this._uiElementDict[TipType.MakeTargetMaterial] = UIElement.MouseTipMakeTargetMaterial;
		this._uiElementDict[TipType.CommonTip] = UIElement.ToolTipCommon;
		this._uiElementDict[TipType.LegendaryBook] = UIElement.MouseTipLegendaryBook;
		this._uiElementDict[TipType.LegendaryBookPageItem] = UIElement.MouseTipLegendaryBookPageItem;
		this._uiElementDict[TipType.Fame] = UIElement.MouseTipFame;
		this._uiElementDict[TipType.HealthInfo] = UIElement.MouseTipHealthInfo;
		this._uiElementDict[TipType.PracticeNotice] = UIElement.MouseTipPracticeNotice;
		this._uiElementDict[TipType.ActiveLoopCost] = UIElement.MouseTipActiveLoopCost;
		this._uiElementDict[TipType.ActiveReadCost] = UIElement.MouseTipActiveReadCost;
		this._uiElementDict[TipType.AiAction] = UIElement.MouseTipAiAction;
		this._uiElementDict[TipType.CharacterCurrentProfession] = UIElement.MouseTipCharacterCurrentProfession;
		this._uiElementDict[TipType.KongSangDing] = UIElement.MouseTipKongSangDing;
		this._uiElementDict[TipType.SoulPiece] = UIElement.ToolTipSoulPiece;
		this._uiElementDict[TipType.Chicken] = UIElement.TooltipChicken;
		this._uiElementDict[TipType.VillagerAssign] = UIElement.MouseTipVillagerAssign;
		this._updateMouseOverObjCoroutine = this.UpdateMouseOverObj();
		SingletonObject.getInstance<YieldHelper>().StartYield(this._updateMouseOverObjCoroutine);
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x000207E0 File Offset: 0x0001E9E0
	private IEnumerator UpdateMouseOverObj()
	{
		for (;;)
		{
			this.Tick(false);
			yield return null;
		}
		yield break;
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x000207F0 File Offset: 0x0001E9F0
	public void Tick(bool isForceFromSpecialType = false)
	{
		Vector2 currentMousePos = Input.mousePosition;
		bool flag = !Application.isFocused;
		if (flag)
		{
			bool wasApplicationFocused = this._wasApplicationFocused;
			if (wasApplicationFocused)
			{
				this.ResetTransientState();
				this._wasApplicationFocused = false;
			}
			this._lastMousePosition = currentMousePos;
		}
		else
		{
			bool flag2 = !this._wasApplicationFocused;
			if (flag2)
			{
				this.ResetTransientState();
				this._wasApplicationFocused = true;
				this._lastMousePosition = currentMousePos;
			}
			float deltaTime = Time.unscaledDeltaTime;
			bool flag3 = deltaTime > 0f;
			if (flag3)
			{
				this._currentMouseSpeed = (currentMousePos - this._lastMousePosition).magnitude / deltaTime;
			}
			this._lastMousePosition = currentMousePos;
			GameObject mouseHitObj = this.GetHitObject();
			bool flag4 = isForceFromSpecialType || mouseHitObj != this._currMouseOverObj || this._lastHittingShowingTips || this._isTriggerWaiting;
			if (flag4)
			{
				this.TickInternal(mouseHitObj, false, isForceFromSpecialType);
			}
			this._currMouseOverObj = mouseHitObj;
			this._lastHittingShowingTips = false;
		}
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x000208E8 File Offset: 0x0001EAE8
	private void TickInternal(GameObject mouseHitObj, bool isHittingShowingTips, bool isForceFromSpecialType)
	{
		TooltipInvoker currentDisplayer = (mouseHitObj != null) ? mouseHitObj.GetComponent<TooltipInvoker>() : null;
		bool isCurrentDisplayerValid = currentDisplayer != null && currentDisplayer.enabled;
		GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
		float tipsTriggerTime = settings.TipsTriggerTime + 0.01f;
		float tipsTriggerSpeed = (float)settings.TipsTriggerSpeed;
		bool tipsContinuousTrigger = settings.TipsContinuousTrigger;
		bool isMouseSpeedAcceptable = this._currentMouseSpeed <= tipsTriggerSpeed * 100f;
		bool flag = !isHittingShowingTips;
		if (flag)
		{
			bool flag2 = !isCurrentDisplayerValid;
			if (flag2)
			{
				this.CancelTriggerWait();
				bool flag3 = this._pendingTipsType != TipType.DefeatMark;
				if (flag3)
				{
					this.HideTips(TipType.Count, true);
				}
			}
			else
			{
				bool flag4 = !isMouseSpeedAcceptable;
				if (flag4)
				{
					this._triggerWaitStartTime = Time.unscaledTime;
				}
			}
			bool flag5 = this._lastHittingShowingTips && this.NeedShowNewTipsOnMouseExitTips != null;
			if (flag5)
			{
				foreach (Delegate func in this.NeedShowNewTipsOnMouseExitTips.GetInvocationList())
				{
					bool flag6 = Convert.ToBoolean(func.DynamicInvoke(Array.Empty<object>()));
					if (flag6)
					{
						isForceFromSpecialType = true;
						break;
					}
				}
			}
		}
		bool flag7 = !isHittingShowingTips && isCurrentDisplayerValid && currentDisplayer != this._lastDisplayer;
		if (flag7)
		{
			this.CancelTriggerWait();
			bool flag8 = this.ShouldSkipTriggerWait(tipsContinuousTrigger);
			if (flag8)
			{
				this.StartTriggerWait(currentDisplayer, 0f);
			}
			else
			{
				bool flag9 = isMouseSpeedAcceptable;
				if (flag9)
				{
					this.StartTriggerWait(currentDisplayer, tipsTriggerTime);
				}
				else
				{
					this.StartTriggerWait(currentDisplayer, tipsTriggerTime);
				}
			}
		}
		else
		{
			bool flag10 = isForceFromSpecialType && isCurrentDisplayerValid;
			if (flag10)
			{
				this.CancelTriggerWait();
				this.HideTips(TipType.Count, true);
				currentDisplayer.ShowTips();
			}
		}
		bool flag11 = this._isTriggerWaiting && isCurrentDisplayerValid && currentDisplayer == this._pendingTriggerDisplayer;
		if (flag11)
		{
			bool flag12 = isMouseSpeedAcceptable;
			if (flag12)
			{
				float elapsed = Time.unscaledTime - this._triggerWaitStartTime;
				bool flag13 = elapsed >= tipsTriggerTime;
				if (flag13)
				{
					this._isTriggerWaiting = false;
					this._pendingTriggerDisplayer = null;
					currentDisplayer.ShowTips();
					this.TriggerAutoFixed();
				}
			}
			else
			{
				this._triggerWaitStartTime = Time.unscaledTime;
			}
		}
		else
		{
			bool flag14 = this._isTriggerWaiting && isCurrentDisplayerValid && currentDisplayer != this._pendingTriggerDisplayer;
			if (flag14)
			{
				this.CancelTriggerWait();
				bool flag15 = isMouseSpeedAcceptable;
				if (flag15)
				{
					bool flag16 = this.ShouldSkipTriggerWait(tipsContinuousTrigger);
					if (flag16)
					{
						this.StartTriggerWait(currentDisplayer, 0f);
					}
					else
					{
						this.StartTriggerWait(currentDisplayer, tipsTriggerTime);
					}
				}
			}
			else
			{
				bool flag17 = !this._isTriggerWaiting && isCurrentDisplayerValid && !isHittingShowingTips && currentDisplayer == this._lastDisplayer && !this.IsTipsVisible(currentDisplayer.Type);
				if (flag17)
				{
					bool flag18 = isMouseSpeedAcceptable;
					if (flag18)
					{
						bool flag19 = this.ShouldSkipTriggerWait(tipsContinuousTrigger);
						if (flag19)
						{
							this.StartTriggerWait(currentDisplayer, 0f);
						}
						else
						{
							this.StartTriggerWait(currentDisplayer, tipsTriggerTime);
						}
					}
				}
			}
		}
		bool flag20 = !isHittingShowingTips;
		if (flag20)
		{
			this._lastDisplayer = currentDisplayer;
		}
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x00020BF0 File Offset: 0x0001EDF0
	private bool ShouldSkipTriggerWait(bool tipsContinuousTrigger)
	{
		bool flag = !tipsContinuousTrigger;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = this._showingTips != null && this._showingTips.UiBase != null && this._showingTips.UiBase.gameObject.activeInHierarchy;
			result = (flag2 || Time.unscaledTime - this._lastTipsHideTime <= 0.1f);
		}
		return result;
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x00020C60 File Offset: 0x0001EE60
	private void StartTriggerWait(TooltipInvoker displayer, float tipsTriggerTime)
	{
		bool flag = tipsTriggerTime <= 0f;
		if (flag)
		{
			displayer.ShowTips();
			this.TriggerAutoFixed();
		}
		else
		{
			this.HideTips(TipType.Count, true);
			this._isTriggerWaiting = true;
			this._pendingTriggerDisplayer = displayer;
			this._triggerWaitStartTime = Time.unscaledTime;
		}
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x00020CB6 File Offset: 0x0001EEB6
	private void TriggerAutoFixed()
	{
		UIElement showingTips = this._showingTips;
		MouseTipBase mouseTipBase = ((showingTips != null) ? showingTips.UiBase : null) as MouseTipBase;
		if (mouseTipBase != null)
		{
			mouseTipBase.StartAutoFixedTimer();
		}
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x00020CDC File Offset: 0x0001EEDC
	private void CancelTriggerWait()
	{
		this._isTriggerWaiting = false;
		this._pendingTriggerDisplayer = null;
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x00020CF0 File Offset: 0x0001EEF0
	private void ResetTransientState()
	{
		this.CancelTriggerWait();
		this._currMouseOverObj = null;
		this._lastDisplayer = null;
		this._lastHittingShowingTips = false;
		this._pendingTipsType = TipType.Count;
		this._currentMouseSpeed = 0f;
		bool flag = this._waitingDisplayTween != null;
		if (flag)
		{
			this._waitingDisplayTween.Kill(false);
			this._waitingDisplayTween = null;
		}
		this.HideTips(TipType.Count, true);
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x00020D60 File Offset: 0x0001EF60
	private GameObject GetHitObject()
	{
		Vector2 screenMousePos = UIManager.Instance.UiCamera.ScreenToViewportPoint(Input.mousePosition);
		GameObject hitObj = null;
		float num = screenMousePos.x;
		bool flag;
		if (num >= 0f && num <= 1f)
		{
			num = screenMousePos.y;
			flag = (num >= 0f && num <= 1f);
		}
		else
		{
			flag = false;
		}
		bool flag2 = flag;
		if (flag2)
		{
			RaycastAllManager raycastManger = SingletonObject.getInstance<RaycastAllManager>();
			bool flag3 = raycastManger == null;
			if (flag3)
			{
				return null;
			}
			List<RaycastResult> _raycastResults = raycastManger.GetCurrentFrameResults();
			bool flag4 = _raycastResults.Count > 0;
			if (flag4)
			{
				hitObj = _raycastResults[0].gameObject;
				TooltipInvoker displayer;
				for (;;)
				{
					displayer = hitObj.GetComponent<TooltipInvoker>();
					bool flag5 = displayer != null;
					if (flag5)
					{
						break;
					}
					Transform parent = hitObj.transform.parent;
					bool flag6 = parent == null;
					if (flag6)
					{
						goto Block_10;
					}
					hitObj = parent.gameObject;
				}
				hitObj = ((TooltipManager.OneDisplayerMultiTipsSet.Contains(displayer.Type) || displayer.triggerByChildRaycast) ? displayer.gameObject : _raycastResults[0].gameObject);
				goto IL_140;
				Block_10:
				hitObj = _raycastResults[0].gameObject;
				IL_140:;
			}
		}
		return hitObj;
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x00020EB8 File Offset: 0x0001F0B8
	public void DelayUpdateShowingTipsPosition()
	{
		bool flag = this._waitingDisplayTween != null;
		if (flag)
		{
			this._waitingDisplayTween.Kill(false);
		}
		this._waitingDisplayTween = DOVirtual.DelayedCall(TooltipManager.ShowTipsDelayTime, delegate
		{
			bool flag2 = this._showingTips == null;
			if (!flag2)
			{
				bool flag3 = this._showingTips.UiBase == null || this._showingTips.UiBase as MouseTipBase == null;
				if (flag3)
				{
					Debug.LogWarning(string.Format("update a tip which is null or not MouseTipBase. _showingTips: {0}", this._showingTips));
				}
				UIElement showingTips = this._showingTips;
				bool flag4 = ((showingTips != null) ? showingTips.UiBase : null) != null;
				if (flag4)
				{
					UIElement showingTips2 = this._showingTips;
					MouseTipBase tipBase = ((showingTips2 != null) ? showingTips2.UiBase : null) as MouseTipBase;
					if (tipBase != null)
					{
						tipBase.UpdatePos();
					}
					if (tipBase != null)
					{
						tipBase.EnableSelfRefreshPosition();
					}
					this._pendingTipsType = TipType.Count;
				}
			}
		}, true);
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x00020F00 File Offset: 0x0001F100
	public bool IsTipsVisible(TipType tipType)
	{
		return this._showingTips != null && this._showingTips.UiBase != null && this._showingTips.UiBase.gameObject.activeInHierarchy;
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x00020F48 File Offset: 0x0001F148
	public bool IsShowingUnfixedTip()
	{
		bool flag = this._showingTips == null || this._showingTips.UiBase == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = !this._showingTips.UiBase.gameObject.activeInHierarchy;
			if (flag2)
			{
				result = false;
			}
			else
			{
				MouseTipBase mouseTipBase = this._showingTips.UiBase as MouseTipBase;
				result = (mouseTipBase == null || !mouseTipBase.HasStick);
			}
		}
		return result;
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x00020FC0 File Offset: 0x0001F1C0
	public UIElement GetShowingTipElement()
	{
		return this._showingTips;
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x00020FD8 File Offset: 0x0001F1D8
	public bool IsHittingShowingTips()
	{
		bool flag = this._showingTips == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = null == this._showingTips.UiBase;
			if (flag2)
			{
				result = false;
			}
			else
			{
				RectTransform showingTipRect = this._showingTips.UiBase.GetComponent<RectTransform>();
				Vector3[] corners = new Vector3[4];
				showingTipRect.GetWorldCorners(corners);
				Vector3 mouseWorldPos = UIManager.Instance.UiCamera.ScreenToWorldPoint(Input.mousePosition);
				result = (mouseWorldPos.x >= corners[0].x && mouseWorldPos.x <= corners[2].x && mouseWorldPos.y >= corners[0].y && mouseWorldPos.y <= corners[2].y);
			}
		}
		return result;
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x000210AB File Offset: 0x0001F2AB
	public void Dispose()
	{
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x000210B0 File Offset: 0x0001F2B0
	public static int GetEncyclopediaLinkId(ArgumentBox argsBox)
	{
		int t;
		bool flag = argsBox.Get("EncyclopediaLink", out t);
		int linkType;
		if (flag)
		{
			linkType = t;
		}
		else
		{
			linkType = -1;
		}
		return linkType;
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x000210DC File Offset: 0x0001F2DC
	public void ShowTips(TipType type, ArgumentBox argsBox, bool needRefresh = false, bool showOnLeft = false, bool showOnTop = false, Vector2[] offsetPosArray = null)
	{
		TooltipManager.<>c__DisplayClass44_0 CS$<>8__locals1 = new TooltipManager.<>c__DisplayClass44_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.showOnLeft = showOnLeft;
		CS$<>8__locals1.showOnTop = showOnTop;
		CS$<>8__locals1.offsetPosArray = offsetPosArray;
		CS$<>8__locals1.type = type;
		CS$<>8__locals1.argsBox = argsBox;
		bool isPendingTips = CS$<>8__locals1.type == this._pendingTipsType;
		CS$<>8__locals1.isShowPengingTipsAgain = (this._showingTips != null && isPendingTips);
		CS$<>8__locals1.isShowShowingTipsAgain = this.IsTipsVisible(CS$<>8__locals1.type);
		CS$<>8__locals1.tipToShow = this._uiElementDict[CS$<>8__locals1.type];
		MouseTipBase tipBase = CS$<>8__locals1.tipToShow.UiBase as MouseTipBase;
		bool flag = tipBase != null && !tipBase.CanShowWithArgumentBox(CS$<>8__locals1.argsBox);
		if (flag)
		{
			this._lastTipsHideTime = Time.unscaledTime;
		}
		else
		{
			this.HideTips(TipType.Count, !CS$<>8__locals1.isShowPengingTipsAgain);
			this._showingTips = CS$<>8__locals1.tipToShow;
			bool shouldCallShow = !CS$<>8__locals1.tipToShow.IsWaitShowing;
			CS$<>8__locals1.argsBox.Set("NeedRefresh", needRefresh);
			CS$<>8__locals1.tipToShow.SetOnInitArgs(CS$<>8__locals1.argsBox);
			UIElement tipToShow = CS$<>8__locals1.tipToShow;
			tipToShow.OnListenerIdReady = (Action)Delegate.Combine(tipToShow.OnListenerIdReady, new Action(delegate()
			{
				bool flag6 = CS$<>8__locals1.<>4__this._showingTips == CS$<>8__locals1.tipToShow;
				if (flag6)
				{
					MouseTipBase mouseTipBase = (MouseTipBase)CS$<>8__locals1.tipToShow.UiBase;
					mouseTipBase.ShowOnLeft = CS$<>8__locals1.showOnLeft;
					mouseTipBase.ShowOnTop = CS$<>8__locals1.showOnTop;
					bool flag7 = CS$<>8__locals1.offsetPosArray != null;
					if (flag7)
					{
						mouseTipBase.RightDownOffsetPos = CS$<>8__locals1.offsetPosArray[0];
						mouseTipBase.RightUpOffsetPos = CS$<>8__locals1.offsetPosArray[1];
						mouseTipBase.LeftUpOffsetPos = CS$<>8__locals1.offsetPosArray[2];
						mouseTipBase.LeftDownOffsetPos = CS$<>8__locals1.offsetPosArray[3];
					}
					mouseTipBase.UpdateOffsetPos();
				}
				else
				{
					CS$<>8__locals1.tipToShow.Hide(false);
				}
			}));
			bool flag2 = shouldCallShow;
			if (flag2)
			{
				bool flag3 = CS$<>8__locals1.tipToShow.UiBase == null;
				if (flag3)
				{
					CS$<>8__locals1.tipToShow.PrepareRes(false, delegate(GameObject _)
					{
						base.<ShowTips>g__AfterPrepare|1();
					}, false);
				}
				else
				{
					CS$<>8__locals1.<ShowTips>g__AfterPrepare|1();
				}
			}
			else
			{
				bool isShowPengingTipsAgain = CS$<>8__locals1.isShowPengingTipsAgain;
				if (isShowPengingTipsAgain)
				{
					MouseTipBase baseTip = CS$<>8__locals1.tipToShow.UiBase as MouseTipBase;
					bool flag4 = baseTip != null;
					if (flag4)
					{
						baseTip.Refresh(CS$<>8__locals1.argsBox);
					}
				}
			}
			int linkType = TooltipManager.GetEncyclopediaLinkId(CS$<>8__locals1.argsBox);
			Action<TipType, int> onShowingTips = this.OnShowingTips;
			if (onShowingTips != null)
			{
				onShowingTips(CS$<>8__locals1.type, linkType);
			}
			bool flag5 = !CS$<>8__locals1.isShowShowingTipsAgain && !CS$<>8__locals1.isShowPengingTipsAgain;
			if (flag5)
			{
				this.DelayUpdateShowingTipsPosition();
			}
		}
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x000212F0 File Offset: 0x0001F4F0
	public void HideTips(TipType type = TipType.Count, bool clearShowingTips = true)
	{
		Action<TipType> onHidingTips = this.OnHidingTips;
		if (onHidingTips != null)
		{
			onHidingTips(type);
		}
		bool flag = clearShowingTips && type == TipType.Count && this._showingTips != null;
		if (flag)
		{
			MouseTipBase mouseTipBase = this._showingTips.UiBase as MouseTipBase;
			if (mouseTipBase != null)
			{
				mouseTipBase.StopAutoFixedTimer();
			}
			this._showingTips.Hide(false);
			this._showingTips = null;
			this._lastTipsHideTime = Time.unscaledTime;
			this._pendingTipsType = TipType.Count;
			Tween waitingDisplayTween = this._waitingDisplayTween;
			if (waitingDisplayTween != null)
			{
				waitingDisplayTween.Kill(false);
			}
			this._waitingDisplayTween = null;
		}
		else
		{
			UIElement tipToHide;
			bool flag2 = this._uiElementDict.TryGetValue(type, out tipToHide);
			if (flag2)
			{
				MouseTipBase mouseTipBase2 = tipToHide.UiBase as MouseTipBase;
				if (mouseTipBase2 != null)
				{
					mouseTipBase2.StopAutoFixedTimer();
				}
				tipToHide.Hide(false);
				this._lastTipsHideTime = Time.unscaledTime;
			}
		}
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x000213D0 File Offset: 0x0001F5D0
	public bool IsCurrMouseOverObj(GameObject obj)
	{
		return obj == this._currMouseOverObj;
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x000213F0 File Offset: 0x0001F5F0
	public MouseTipBase GetTipsUi(TipType type)
	{
		return this._uiElementDict[type].UiBaseAs<MouseTipBase>();
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x00021444 File Offset: 0x0001F644
	// Note: this type is marked as 'beforefieldinit'.
	static TooltipManager()
	{
		Dictionary<sbyte, TipType> dictionary = new Dictionary<sbyte, TipType>();
		dictionary[6] = TipType.CraftTool;
		dictionary[5] = TipType.Material;
		dictionary[10] = TipType.SkillBook;
		dictionary[11] = TipType.Cricket;
		dictionary[1] = TipType.Armor;
		dictionary[4] = TipType.Carrier;
		dictionary[3] = TipType.Clothing;
		dictionary[7] = TipType.Food;
		dictionary[8] = TipType.Medicine;
		dictionary[12] = TipType.Misc;
		dictionary[9] = TipType.TeaWine;
		dictionary[2] = TipType.Accessory;
		dictionary[0] = TipType.Weapon;
		TooltipManager.ItemTypeToTipType = dictionary;
		TooltipManager.OneDisplayerMultiTipsSet = new HashSet<TipType>
		{
			TipType.MapBlock
		};
		TooltipManager.ShowTipsDelayTime = GlobalConfig.Instance.MouseTipDelayTime;
	}

	// Token: 0x040003BA RID: 954
	private const float ContinuousTriggerWindowTime = 0.1f;

	// Token: 0x040003BB RID: 955
	public static Dictionary<sbyte, TipType> ItemTypeToTipType;

	// Token: 0x040003BC RID: 956
	private static readonly HashSet<TipType> OneDisplayerMultiTipsSet;

	// Token: 0x040003BD RID: 957
	public TooltipManager.NeedShowNewTipsOnMouseExitTipsDelegate NeedShowNewTipsOnMouseExitTips;

	// Token: 0x040003BE RID: 958
	private static readonly float ShowTipsDelayTime;

	// Token: 0x040003BF RID: 959
	private readonly Dictionary<TipType, UIElement> _uiElementDict = new Dictionary<TipType, UIElement>();

	// Token: 0x040003C0 RID: 960
	private UIElement _showingTips;

	// Token: 0x040003C1 RID: 961
	private IEnumerator _updateMouseOverObjCoroutine;

	// Token: 0x040003C2 RID: 962
	private GameObject _currMouseOverObj;

	// Token: 0x040003C3 RID: 963
	private TooltipInvoker _lastDisplayer;

	// Token: 0x040003C4 RID: 964
	private Tween _waitingDisplayTween;

	// Token: 0x040003C5 RID: 965
	private bool _lastHittingShowingTips;

	// Token: 0x040003C6 RID: 966
	private TipType _pendingTipsType = TipType.Count;

	// Token: 0x040003C7 RID: 967
	private Vector2 _lastMousePosition;

	// Token: 0x040003C8 RID: 968
	private float _currentMouseSpeed;

	// Token: 0x040003C9 RID: 969
	private bool _isTriggerWaiting;

	// Token: 0x040003CA RID: 970
	private float _triggerWaitStartTime;

	// Token: 0x040003CB RID: 971
	private TooltipInvoker _pendingTriggerDisplayer;

	// Token: 0x040003CC RID: 972
	private float _lastTipsHideTime = float.NegativeInfinity;

	// Token: 0x040003CD RID: 973
	private bool _wasApplicationFocused = true;

	// Token: 0x020010FC RID: 4348
	// (Invoke) Token: 0x0600C106 RID: 49414
	public delegate bool NeedShowNewTipsOnMouseExitTipsDelegate();
}
