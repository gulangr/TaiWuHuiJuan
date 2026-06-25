using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using CharacterDataMonitor;
using Config;
using Config.ConfigCells;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Switch;
using Game.Views.Combat.Item;
using Game.Views.Combat.Migrate;
using Game.Views.MouseTips;
using GameData.Combat.Animation;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.Combat.MixPoison;
using GameData.Domains.CombatSkill;
using GameData.Domains.Extra;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Profession;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.Combat
{
	// Token: 0x02000B35 RID: 2869
	public class ViewCombat : UIBase, IPreloadElement
	{
		// Token: 0x06008C8D RID: 35981 RVA: 0x0040F318 File Offset: 0x0040D518
		private static float GetDefeatMarkSeparatorX(int index)
		{
			byte count = GlobalConfig.NeedDefeatMarkCount[index];
			return 208f + 20f * (float)count + 6f * (float)(count - 1) + 3f;
		}

		// Token: 0x17000F79 RID: 3961
		// (get) Token: 0x06008C8E RID: 35982 RVA: 0x0040F351 File Offset: 0x0040D551
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x17000F7A RID: 3962
		// (get) Token: 0x06008C8F RID: 35983 RVA: 0x0040F358 File Offset: 0x0040D558
		private bool IsPausing
		{
			get
			{
				return this.Model.IsPausing;
			}
		}

		// Token: 0x17000F7B RID: 3963
		// (get) Token: 0x06008C90 RID: 35984 RVA: 0x0040F365 File Offset: 0x0040D565
		private sbyte _combatType
		{
			get
			{
				return this.Model.Config.CombatType;
			}
		}

		// Token: 0x17000F7C RID: 3964
		// (get) Token: 0x06008C91 RID: 35985 RVA: 0x0040F377 File Offset: 0x0040D577
		private short _distance
		{
			get
			{
				return this.Model.CurrentDistance;
			}
		}

		// Token: 0x17000F7D RID: 3965
		// (get) Token: 0x06008C92 RID: 35986 RVA: 0x0040F384 File Offset: 0x0040D584
		private float _realTimeScale
		{
			get
			{
				return this.Model.TimeScale;
			}
		}

		// Token: 0x17000F7E RID: 3966
		// (get) Token: 0x06008C93 RID: 35987 RVA: 0x0040F391 File Offset: 0x0040D591
		private IReadOnlyDictionary<int, CharacterDisplayData> _charDisplayDataDict
		{
			get
			{
				return this.Model.DisplayDataCache;
			}
		}

		// Token: 0x06008C94 RID: 35988 RVA: 0x0040F39E File Offset: 0x0040D59E
		public void SetClickAttackValue(bool value)
		{
			this._clickAttackButton = value;
		}

		// Token: 0x17000F7F RID: 3967
		// (get) Token: 0x06008C95 RID: 35989 RVA: 0x0040F3A8 File Offset: 0x0040D5A8
		private IReadOnlyList<int> _selfTeam
		{
			get
			{
				return this.Model.SelfTeam;
			}
		}

		// Token: 0x17000F80 RID: 3968
		// (get) Token: 0x06008C96 RID: 35990 RVA: 0x0040F3B5 File Offset: 0x0040D5B5
		private ref int _selfCurrCharId
		{
			get
			{
				return ref this.Model.SelfCharId;
			}
		}

		// Token: 0x17000F81 RID: 3969
		// (get) Token: 0x06008C97 RID: 35991 RVA: 0x0040F3C2 File Offset: 0x0040D5C2
		public short SelfAffectingMoveSkillId
		{
			get
			{
				return this._selfAffectingMoveSkillId;
			}
		}

		// Token: 0x17000F82 RID: 3970
		// (get) Token: 0x06008C98 RID: 35992 RVA: 0x0040F3CA File Offset: 0x0040D5CA
		public short SelfAffectingDefendSkillId
		{
			get
			{
				return this._selfAffectingDefendSkillId;
			}
		}

		// Token: 0x17000F83 RID: 3971
		// (get) Token: 0x06008C99 RID: 35993 RVA: 0x0040F3D2 File Offset: 0x0040D5D2
		private ItemKey[] _selfWeaponList
		{
			get
			{
				CombatSubProcessorCharacter selfCharacter = this.Model.SelfCharacter;
				return (selfCharacter != null) ? selfCharacter.Weapons : null;
			}
		}

		// Token: 0x17000F84 RID: 3972
		// (get) Token: 0x06008C9A RID: 35994 RVA: 0x0040F3EB File Offset: 0x0040D5EB
		private ItemKey[] _enemyWeaponList
		{
			get
			{
				CombatSubProcessorCharacter enemyCharacter = this.Model.EnemyCharacter;
				return (enemyCharacter != null) ? enemyCharacter.Weapons : null;
			}
		}

		// Token: 0x17000F85 RID: 3973
		// (get) Token: 0x06008C9B RID: 35995 RVA: 0x0040F404 File Offset: 0x0040D604
		private IReadOnlyList<int> _enemyTeam
		{
			get
			{
				return this.Model.EnemyTeam;
			}
		}

		// Token: 0x17000F86 RID: 3974
		// (get) Token: 0x06008C9C RID: 35996 RVA: 0x0040F411 File Offset: 0x0040D611
		private ref int _enemyCurrCharId
		{
			get
			{
				return ref this.Model.EnemyCharId;
			}
		}

		// Token: 0x17000F87 RID: 3975
		// (get) Token: 0x06008C9D RID: 35997 RVA: 0x0040F41E File Offset: 0x0040D61E
		private unsafe SkeletonAnimation SelfCurrCharSkeleton
		{
			get
			{
				return this.GetSkeleton(*this._selfCurrCharId);
			}
		}

		// Token: 0x17000F88 RID: 3976
		// (get) Token: 0x06008C9E RID: 35998 RVA: 0x0040F42D File Offset: 0x0040D62D
		private unsafe SkeletonAnimation EnemyCurrCharSkeleton
		{
			get
			{
				return this.GetSkeleton(*this._enemyCurrCharId);
			}
		}

		// Token: 0x17000F89 RID: 3977
		// (get) Token: 0x06008C9F RID: 35999 RVA: 0x0040F43C File Offset: 0x0040D63C
		private GlobalSettings SettingData
		{
			get
			{
				return SingletonObject.getInstance<GlobalSettings>();
			}
		}

		// Token: 0x17000F8A RID: 3978
		// (get) Token: 0x06008CA0 RID: 36000 RVA: 0x0040F443 File Offset: 0x0040D643
		private AiOptions AiOptions
		{
			get
			{
				return this.aiOptionMask.GetAiOptions();
			}
		}

		// Token: 0x17000F8B RID: 3979
		// (get) Token: 0x06008CA1 RID: 36001 RVA: 0x0040F450 File Offset: 0x0040D650
		private bool SettingAiOptions
		{
			get
			{
				return this.aiOptionMask.gameObject.activeSelf;
			}
		}

		// Token: 0x06008CA2 RID: 36002 RVA: 0x0040F464 File Offset: 0x0040D664
		public override void OnInit(ArgumentBox argsBox)
		{
			this.Awake();
			this._showingSystemOption = false;
			this._keepPauseUntilCastSkill = false;
			this._skillWheelPauseHolding = false;
			this._skillSortPauseHolding = false;
			this._guidingChapterOpen = false;
			this._maxDefeatMarkCount = (int)GlobalConfig.NeedDefeatMarkCount[(int)this._combatType];
			this._inBulletTime = false;
			this._flawCountDict.Clear();
			this._flawTimeDict.Clear();
			this._acupointCountDict.Clear();
			this._acupointTimeDict.Clear();
			this._loopAniDict.Clear();
			this._displayPosDict.Clear();
			this._skeletonLoaded.Clear();
			this._autoCombat = false;
			this._isInSelfAttackRange = true;
			this._attackSkillInitialized = false;
			this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this._selfPos = int.MinValue;
			this._selfDisplayPos = int.MinValue;
			this._selfTargetDistance = -1;
			this._selfAffectingMoveSkillId = -1;
			this._selfAffectingDefendSkillId = -1;
			this._selfDefeatMarkInitialized = false;
			this._selfDefeatMarkObjs.Clear();
			this.ReleaseMarkPrefab("ui_Combat_DefeatMarkPrefab", this._selfDefeatMarkAddQueue);
			this.ReleaseMarkPrefab("ui_Combat_DefeatMarkSeparatorPrefab", this._selfDefeatMarkSeparatorAddQueue);
			this._selfDefeatMarkKeyList.Clear();
			this._selfPoisons.Initialize();
			this._selfPoisonResists.Initialize();
			this._selfInjuryAutoHealCollection = null;
			this._selfOldInjuryAutoHealCollection = null;
			this._selfUnlockPrepareValue.Clear();
			this._selfCanUnlockAttack.Clear();
			this._selfUnlockTriggered = new bool[3];
			this._selfUnlockEffectTriggered = new bool[3];
			this._selfRawCreateEffects.Clear();
			this._enemyPos = int.MinValue;
			this._enemyDisplayPos = int.MinValue;
			this._enemyTargetDistance = -1;
			this._enemyAffectingMoveSkillId = -1;
			this._enemyAffectingDefendSkillId = -1;
			this._enemyDefeatMarkInitialized = false;
			this._enemyDefeatMarkObjs.Clear();
			this.ReleaseMarkPrefab("ui_Combat_DefeatMarkPrefab", this._enemyDefeatMarkAddQueue);
			this.ReleaseMarkPrefab("ui_Combat_DefeatMarkSeparatorPrefab", this._enemyDefeatMarkSeparatorAddQueue);
			this._enemyDefeatMarkKeyList.Clear();
			this._enemyPoisonResists.Initialize();
			this._enemyInjuryAutoHealCollection = null;
			this._enemyOldInjuryAutoHealCollection = null;
			this._enemyUnlockPrepareValue.Clear();
			this._enemyCanUnlockAttack.Clear();
			this._enemyUnlockTriggered = new bool[3];
			this._moveKeyDownList.Clear();
			this._moveTweenerDict.Clear();
			this._selectingUseItem = false;
			this._xiangshuSceneInitialized = false;
			this._needLoadAssetSkillList.Clear();
			this._loadedAssetSkillList.Clear();
			this._skillAniDict.Clear();
			foreach (string particleName in this._skillAndSpecialParticleNameList)
			{
				CombatPoolAdaptor.RemoveData(particleName);
			}
			this._skillAndSpecialParticleNameList.Clear();
			this._skillAndSpecialSoundDict.Clear();
			for (int i = 0; i < this._defendBounceRangeList.Length; i++)
			{
				this._defendBounceRangeList[i] = -1;
			}
			for (int j = 0; j < this._defendBounceCharList.Length; j++)
			{
				this._defendBounceCharList[j] = -1;
			}
			this._mouseOverSkill = -1;
			this.combatTimeScaleToggle.SetPause(false);
			this.combatConfigTipsBack.gameObject.SetActive(false);
			this.bulletTimeMask.gameObject.SetActive(false);
			this.ChangeTrickMaskSetActive(false);
			this.clickMask.SetActive(false);
			ViewCombat.SetInnerRatioVisible(this.selfInnerRatio, false);
			ViewCombat.SetInnerRatioVisible(this.enemyInnerRatio, false);
			this.UpdateHotkeyText();
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
			UIElement element2 = this.Element;
			element2.OnShowed = (Action)Delegate.Combine(element2.OnShowed, new Action(this.OnShowed));
			this._virtualCamera.Reset();
			this._virtualCamera.SetScaleFactor(this.Model.Config.ScaleFactor);
		}

		// Token: 0x06008CA3 RID: 36003 RVA: 0x0040F880 File Offset: 0x0040DA80
		private void OnListenerIdReady()
		{
			this.selfInfoTop.defeatMarkGroup.Set(this._maxDefeatMarkCount);
			this.InitDefeatMarkBack(this.selfInfoTop, true);
			this.selfInfoChar.outOfAttackRangeTips.gameObject.SetActive(false);
			this.selfInfoChar.weaponDurabilityNotEnoughTips.SetActive(false);
			this.selfInfoChar.prepareProgress.gameObject.SetActive(false);
			this.selfTeammateAffectingDefSkill.gameObject.SetActive(false);
			this._selfOtherActionHolder.otherActionTypeList[2].GetComponent<TooltipInvoker>().PresetParam[1] = (this.Model.Config.SelfCanFlee ? LocalStringManager.Get(LanguageKey.LK_Combat_EscapeInfo) : LocalStringManager.Get(LanguageKey.LK_Combat_CannotEscapeInfo));
			this.HideAttackTips();
			this._previewRangeSkill = -1;
			this._previewRangeSkillCharId = -1;
			this._previewRangeWeapon = ItemKey.Invalid;
			this._previewRangeWeaponCharId = -1;
			this._previewRangeItem = new OuterAndInnerShorts(-1, -1);
			this.UpdateAttackRangePreview();
			this.Model.ClearPreviewCostSkillData();
			this.UpdateSkillCostPreview();
			this.enemyInfoTop.defeatMarkGroup.Set(this._maxDefeatMarkCount);
			this.InitDefeatMarkBack(this.enemyInfoTop, false);
			this.enemyInfoChar.outOfAttackRangeTips.gameObject.SetActive(false);
			this.enemyInfoChar.prepareProgress.gameObject.SetActive(false);
			this.enemyTeammateAffectingDefSkill.gameObject.SetActive(false);
			this._tipsList.Clear();
			this.combatTimeScaleToggle.SetPause(false);
			this.SetDisplayTimeScale(this.SettingData.CombatSpeed, true);
			this.UpdateTimeScale(1f);
			this.UpdateTargetDistanceInteract();
			this.selfCarrierAnimalSkeleton.gameObject.SetActive(false);
			this.selfSpecialShowSkeleton.gameObject.SetActive(false);
			CharacterDomainMethod.Call.GetCurrMaxEatingSlotsCount(this.Element.GameDataListenerId, this._taiwuCharId);
			CharacterDomainMethod.Call.IsCarrierDurabilityRunningOut(this.Element.GameDataListenerId, this._taiwuCharId);
			ExtraDomainMethod.Call.IsProfessionalSkillUnlocked(this.Element.GameDataListenerId, 1, 1);
			this.OnDataReady();
			this.pureMode.OpenPure(CombatUtils.CombatPureOpen);
		}

		// Token: 0x06008CA4 RID: 36004 RVA: 0x0040FABC File Offset: 0x0040DCBC
		private void OnShowed()
		{
			GlobalDomainMethod.Call.InvokeGuidingTrigger(63);
			this._virtualCamera.UnlockPosition();
			this._virtualCamera.UnlockScale();
			this.UpdateAutoFightMark(this.SettingData.AutoCombat);
			string[] bgm2 = this.Model.Config.Bgm;
			string bgm = (bgm2 != null) ? bgm2[0] : null;
			bool flag = bgm.IsNullOrEmpty();
			if (flag)
			{
				bgm = ViewCombat.CommonBgmList[Random.Range(0, ViewCombat.CommonBgmList.Length)];
			}
			AudioManager.Instance.PlayMusic(bgm, 1f, 100, null);
			AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 1f, 100);
		}

		// Token: 0x06008CA5 RID: 36005 RVA: 0x0040FB60 File Offset: 0x0040DD60
		private void OnCombatBeginReady()
		{
			CombatDomainMethod.Call.StartCombat(this.Element.GameDataListenerId);
			bool autoCombat = this.SettingData.AutoCombat;
			if (autoCombat)
			{
				this.SetAutoCombat(true);
			}
		}

		// Token: 0x06008CA6 RID: 36006 RVA: 0x0040FB96 File Offset: 0x0040DD96
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(8, 34, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(8, 35, ulong.MaxValue, null));
		}

		// Token: 0x06008CA7 RID: 36007 RVA: 0x0040FBC8 File Offset: 0x0040DDC8
		public void Preload()
		{
			bool flag = ViewCombat.CharId2BossId.Count == 0;
			if (flag)
			{
				sbyte bossId = 0;
				while ((int)bossId < Boss.Instance.Count)
				{
					short[] charIdList = Boss.Instance[bossId].CharacterIdList;
					foreach (short charId in charIdList)
					{
						ViewCombat.CharId2BossId[charId] = bossId;
					}
					bossId += 1;
				}
			}
			bool flag2 = ViewCombat.MoveSkillAniSet.Count == 0;
			if (flag2)
			{
				short i = 0;
				while ((int)i < CombatSkill.Instance.Count)
				{
					CombatSkillItem skillConfig = CombatSkill.Instance[i];
					bool flag3 = skillConfig.JumpAni != null;
					if (flag3)
					{
						ViewCombat.MoveSkillAniSet.Add(skillConfig.JumpAni[0]);
						ViewCombat.MoveSkillAniSet.Add(skillConfig.JumpAni[1]);
					}
					i += 1;
				}
			}
			CombatPoolAdaptor.Preload("ui_Combat_DefeatMarkPrefab", this.defeatMarkPrefab, 108);
			ResLoader.Load<SkeletonDataAsset>("RemakeResources/SpineAnimations/Character/skeleton_skin_SkeletonData", delegate(SkeletonDataAsset asset)
			{
				this._actorSkeleton = asset;
				this._actorSkeleton.GetSkeletonData(false);
				foreach (SkeletonAnimation skeleton in this.selfSkeletonList)
				{
					skeleton.skeletonDataAsset = this._actorSkeleton;
					skeleton.Initialize(true, false);
				}
				foreach (SkeletonAnimation skeleton2 in this.enemySkeletonList)
				{
					skeleton2.skeletonDataAsset = this._actorSkeleton;
					skeleton2.Initialize(true, false);
				}
			}, null, false);
			ResLoader.Load<AssetBundle>("combatbase.uab", delegate(AssetBundle bundle)
			{
				RawAnimationAsset[] animations = bundle.LoadAllAssets<RawAnimationAsset>();
				GameObject[] particles = bundle.LoadAllAssets<GameObject>();
				AudioClip[] sounds = bundle.LoadAllAssets<AudioClip>();
				RectTransform particleHolder = this.particlePrefabHolder;
				foreach (RawAnimationAsset rawAnimation in animations)
				{
					this._commonAniDict.Add(rawAnimation.animName, rawAnimation);
				}
				foreach (GameObject particle in particles)
				{
					GameObject poolObj = Object.Instantiate<GameObject>(particle, particleHolder);
					this._commonParticleNameList.Add(particle.name);
					CombatPoolAdaptor.SetSrcObject(particle.name, poolObj, 0);
				}
				foreach (AudioClip sound in sounds)
				{
					this._commonSoundDict.Add(sound.name, sound);
				}
			}, null, false);
		}

		// Token: 0x06008CA8 RID: 36008 RVA: 0x0040FD08 File Offset: 0x0040DF08
		private void Awake()
		{
			bool awaked = this._awaked;
			if (!awaked)
			{
				this._awaked = true;
				this.combatTimeScaleToggle.Setup(this.combatConfigTipsBack);
				this._components = base.transform.GetComponentsInChildren<ICombatComponent>(true);
				this.combatWheel.Init();
				this.combatWheel.externalMaskTarget = this.combatSharedMaskTarget;
				this.combatWheel.OnOpen = new Action(this.WheelOnOpen);
				this.combatWheel.OnClose = new Action(this.WheelOnClose);
				this.combatWheel.OnOpenChangeTrick = new Action(this.WheelOpenChangeTrick);
				this.combatWheel.OnToggleUseItem = new Action(this.WheelToggleUseItem);
				this.combatWheel.OnChangeWeapon = new Action<int>(this.WheelChangeWeapon);
				this.combatWheel.OnUnlockWeapon = new Action<int>(this.WheelUnlockWeapon);
				this.combatWheel.OnRequestOtherAction = new Action<sbyte>(this.WheelRequestOtherAction);
				this.combatWheel.OnCastSkill = new Action<short>(this.WheelCastSkillByTemplateId);
				this.combatWheel.OnBreakAffectingSkill = new Action<short>(this.WheelBreakAffectingSkill);
				this.combatWheel.OnUpdatePreviewRangeWeapon = new Action<int>(this.UpdatePreviewRangeWeapon);
				this.combatWheel.OnClearPreviewRangeWeapon = new Action(this.ClearPreviewRangeWeapon);
				this.combatSkillWheel.Init();
				this.combatSkillWheel.OnSkillWheelOpen = new Action(this.SkillWheelOnOpen);
				this.combatSkillWheel.OnSkillWheelClose = new Action(this.SkillWheelOnClose);
				this.combatSkillWheel.OnCastSkill = new Action<short>(this.SkillWheelCastSkill);
				this.combatSkillWheel.OnBreakAffectingSkill = new Action<short>(this.SkillWheelBreakAffectingSkill);
				this.combatSkillSortWidget.Init();
				this.combatSkillSortWidget.OnOpen = new Action(this.SkillSortOnOpen);
				this.combatSkillSortWidget.OnClose = new Action(this.SkillSortOnClose);
				CombatPoolAdaptor.SetSrcObject("ui_Combat_DefeatMarkSeparatorPrefab", this.defeatMarkSeparatorPrefab, 0);
				CombatPoolAdaptor.SetSrcObject("ui_Combat_TextTipsPrefab", this.textTipsPrefab, 0);
				CombatPoolAdaptor.SetSrcObject("ui_Combat_IconTipsPrefab", this.iconTipsPrefab, 0);
				CombatPoolAdaptor.SetSrcObject("ui_Combat_EffectTipsPrefab", this.effectTipsPrefab, 0);
				CombatPoolAdaptor.SetSrcObject("ui_Combat_CommandBubblePrefab", this.commandBubblePrefab, 0);
				CombatPoolAdaptor.SetSrcObject("ui_Combat_TrickPrefab", this.trickPrefab, 0);
				CombatPoolAdaptor.SetSrcObject("ui_Combat_Damage_Num", this.damageNumPrefab, 0);
				CombatPoolAdaptor.SetSrcObject("ui_Combat_Fatal_Damage_Num", this.fatalDamageNumPrefab, 0);
				this.combatDropsPrefab.SetPoolItem();
				this._selfPetSkeleton = this.selfSkeletonList[0].GetComponent<CombatSpineSkeleton>().pet;
				RectTransform costMobilityLine = this.selfInfoChar.costMobilityLine;
				Vector2 costMobilityLineSize = costMobilityLine.sizeDelta;
				costMobilityLine.DOSizeDelta(costMobilityLineSize.SetY(costMobilityLineSize.y + 6f), 0.5f, false).SetLoops(-1, LoopType.Yoyo);
				this._selfWeaponHolder = this.selfInfoBottom.weaponHolder;
				this._selfOtherActionHolder = this.combatQuickUseItemPanel.otherActionHolder;
				for (int i = 0; i < this._selfWeaponHolder.childCount; i++)
				{
					int index = i;
					CombatWeaponPrefab weaponRefers = this._selfWeaponHolder.GetChild(i).GetComponent<CombatWeaponPrefab>();
					PointerTrigger weaponPointerTrigger = weaponRefers.GetComponent<PointerTrigger>();
					CImage highLightImg = weaponRefers.highLight;
					RectTransform outRangeLine = weaponRefers.outAttackRangeLine;
					Vector2 outRangeLineSize = outRangeLine.sizeDelta;
					outRangeLine.DOSizeDelta(outRangeLineSize.SetX(outRangeLineSize.x + 6f), 0.5f, false).SetLoops(-1, LoopType.Yoyo);
					weaponPointerTrigger.EnterEvent.AddListener(delegate()
					{
						bool flag4 = index >= 3 || weaponRefers.UserInt > 0;
						if (flag4)
						{
							highLightImg.gameObject.SetActive(true);
						}
						bool flag5 = this.Model.SelfCharacter == null;
						if (!flag5)
						{
							bool flag6 = index != this.Model.SelfCharacter.UsingWeaponIndex;
							if (flag6)
							{
								this._previewRangeWeapon = this._selfWeaponList[index];
								this._previewRangeWeaponCharId = this.Model.SelfCharId;
								this.UpdateAttackRangePreview();
							}
						}
					});
					weaponPointerTrigger.ExitEvent.AddListener(delegate()
					{
						highLightImg.gameObject.SetActive(false);
						bool flag4 = this.Model.SelfCharacter == null;
						if (!flag4)
						{
							bool flag5 = index != this.Model.SelfCharacter.UsingWeaponIndex;
							if (flag5)
							{
								this._previewRangeWeapon = ItemKey.Invalid;
								this._previewRangeWeaponCharId = -1;
								this.UpdateAttackRangePreview();
							}
						}
					});
					weaponRefers.GetComponent<CButton>().ClearAndAddListener(delegate
					{
						this._previewRangeWeapon = ItemKey.Invalid;
						this._previewRangeWeaponCharId = -1;
						this.UpdateAttackRangePreview();
						this.DoRequestChangeWeapon(index);
					});
					bool flag = index < 3;
					if (flag)
					{
						CombatWeaponUnlockHolderPrefab unlockHolder = weaponRefers.unlockHolder;
						CButton unlockBtn = unlockHolder.unlockBtn;
						unlockBtn.ClearAndAddListener(delegate
						{
							this.DoRequestUnlockWeapon(index);
						});
					}
				}
				for (int j = 0; j <= 2; j++)
				{
					int type = j;
					CombatOtherActionType actionRefers = this._selfOtherActionHolder.otherActionTypeList[j].GetComponent<CombatOtherActionType>();
					PointerTrigger actionPointerTrigger = actionRefers.GetComponent<PointerTrigger>();
					actionPointerTrigger.EnterEvent.AddListener(delegate()
					{
						bool flag4 = type < 2;
						if (flag4)
						{
							this.UpdateHealInjuryPoisonBtnTips(actionRefers.GetComponent<TooltipInvoker>(), type == 0);
						}
					});
				}
				CToggleGroup changeTrickTypeTogGroup = this.changeTrickTrickHolder;
				CToggleGroup changeTrickPartTogGroup = this.changeTrickPartHolder;
				changeTrickTypeTogGroup.Init(-1);
				changeTrickPartTogGroup.Init(-1);
				changeTrickPartTogGroup.OnActiveIndexChange += delegate(int _, int _)
				{
					this.SetSelfAttackTogGroup();
				};
				this.flawChangeTrickBtn.ClearAndAddListener(delegate
				{
					this.OnClickFlawOrAcupointChangeTrick(EFlawOrAcupointType.Flaw);
				});
				this.acupointChangeTrickBtn.ClearAndAddListener(delegate
				{
					this.OnClickFlawOrAcupointChangeTrick(EFlawOrAcupointType.Acupoint);
				});
				ViewCombat.SetupChangeTrickBtnHover(this.flawChangeTrickBtn);
				ViewCombat.SetupChangeTrickBtnHover(this.acupointChangeTrickBtn);
				ViewCombat.SetupChangeTrickBtnHover(this.confirmChangeTrick);
				bool flag2 = this.autoFight != null;
				if (flag2)
				{
					this.autoFight.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnAutoFightValueChanged));
					this.autoFight.onValueChanged.AddListener(new UnityAction<bool>(this.OnAutoFightValueChanged));
					this.autoFight.navigation = new Navigation
					{
						mode = Navigation.Mode.None
					};
				}
				this._enemyPetSkeleton = this.enemySkeletonList[0].GetComponent<CombatSpineSkeleton>().pet;
				this._enemyWeaponHolder = this.enemyInfoBottom.weaponHolder;
				for (int k = 0; k < this._enemyWeaponHolder.childCount; k++)
				{
					CombatWeaponPrefab weaponRefers2 = this._enemyWeaponHolder.GetChild(k).GetComponent<CombatWeaponPrefab>();
					RectTransform outRangeLine2 = weaponRefers2.outAttackRangeLine;
					Vector2 outRangeLineSize2 = outRangeLine2.sizeDelta;
					outRangeLine2.DOSizeDelta(outRangeLineSize2.SetX(outRangeLineSize2.x + 6f), 0.5f, false).SetLoops(-1, LoopType.Yoyo);
					bool flag3 = k < 3;
					if (flag3)
					{
						weaponRefers2.unlockHolder.unlockBtn.transition = Selectable.Transition.None;
					}
				}
				short[] freeWeapons = new short[]
				{
					0,
					1,
					2,
					884
				};
				for (int l = 3; l < this._selfWeaponHolder.childCount; l++)
				{
					WeaponItem weaponConfig = Weapon.Instance[freeWeapons[l - 3]];
					CombatWeaponPrefab selfWeapon = this._selfWeaponHolder.GetChild(l).GetComponent<CombatWeaponPrefab>();
					CombatWeaponPrefab enemyWeapon = this._enemyWeaponHolder.GetChild(l).GetComponent<CombatWeaponPrefab>();
					selfWeapon.icon.SetSprite(weaponConfig.Icon, false, null);
					enemyWeapon.icon.SetSprite(weaponConfig.Icon, false, null);
				}
				this.InitHotKey();
				this.targetDistanceBar.BarScaleProvider = (() => this._virtualCamera.GetScale());
				this.combatSkillScroll.InitCallbacks(delegate(CombatProactiveSkillView _, CombatSkillDisplayData skillData)
				{
					this.CastProactiveSkill(this.Model.OrderedProactiveSkillList[this.Model.SelfCharId].IndexOf(skillData.TemplateId));
				}, new CombatProactiveSkillScroll.ProactiveCombatSkillViewEvent(this.ShowJumpSetting), new CombatProactiveSkillScroll.ProactiveCombatSkillViewEvent(this.OnEnterProactiveSkillView), new CombatProactiveSkillScroll.ProactiveCombatSkillViewEvent(this.OnExitProactiveSkillView));
				this.combatParticle.GetSkeletonCallback = new Func<int, SkeletonAnimation>(this.GetSkeleton);
				this.combatParticle.ParticleLocalizeCallback = new Func<string, string>(ViewCombat.LocalizeAvoidParticle);
				GEvent.Add(EEvents.OnGameStateChange, new GEvent.Callback(this.OnGameStateChange));
				this.rawCreatePage.InitSet();
				RectTransform selfCurrentTrickHolder = this.selfInfoBottom.weaponTrick.GetCurrTrickHolder();
				this.selfInfoBottom.trickQueueAndMark.SetSpawnPointFollowTarget(selfCurrentTrickHolder);
				RectTransform enemyCurrentTrickHolder = this.enemyInfoBottom.weaponTrick.GetCurrTrickHolder();
				this.enemyInfoBottom.trickQueueAndMark.SetSpawnPointFollowTarget(enemyCurrentTrickHolder);
			}
		}

		// Token: 0x06008CA9 RID: 36009 RVA: 0x00410534 File Offset: 0x0040E734
		private void OnEnable()
		{
			this.SetupEvents();
			this.combatParticle.SetCameraEnabled(true);
		}

		// Token: 0x06008CAA RID: 36010 RVA: 0x0041054B File Offset: 0x0040E74B
		private void OnDisable()
		{
			this.CloseEvents();
			Tweener shakeTweener = this._shakeTweener;
			if (shakeTweener != null)
			{
				shakeTweener.Kill(true);
			}
			this.SettingData.SaveSettings();
			this.SetTimeScale(1f);
		}

		// Token: 0x06008CAB RID: 36011 RVA: 0x00410580 File Offset: 0x0040E780
		private void OnDestroy()
		{
			CombatPoolAdaptor.RemoveData("ui_Combat_DefeatMarkSeparatorPrefab");
			CombatPoolAdaptor.RemoveData("ui_Combat_TextTipsPrefab");
			CombatPoolAdaptor.RemoveData("ui_Combat_IconTipsPrefab");
			CombatPoolAdaptor.RemoveData("ui_Combat_EffectTipsPrefab");
			CombatPoolAdaptor.RemoveData("ui_Combat_CommandBubblePrefab");
			CombatPoolAdaptor.RemoveData("ui_Combat_TrickPrefab");
			CombatPoolAdaptor.RemoveData("ui_Combat_Damage_Num");
			CombatPoolAdaptor.RemoveData("ui_Combat_Fatal_Damage_Num");
			this.combatDropsPrefab.ClearPoolItem();
			foreach (string particleName in this._commonParticleNameList)
			{
				CombatPoolAdaptor.RemoveData(particleName);
			}
			GEvent.Remove(EEvents.OnGameStateChange, new GEvent.Callback(this.OnGameStateChange));
			bool flag = this.autoFight != null;
			if (flag)
			{
				this.autoFight.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnAutoFightValueChanged));
			}
		}

		// Token: 0x06008CAC RID: 36012 RVA: 0x00410680 File Offset: 0x0040E880
		private void OnApplicationFocus(bool focus)
		{
			bool flag = !focus;
			if (flag)
			{
				this._moveKeyDownList.Clear();
			}
		}

		// Token: 0x06008CAD RID: 36013 RVA: 0x004106A4 File Offset: 0x0040E8A4
		private bool CanOpenWheel()
		{
			return this.Model.CombatStatus == 1 && this.Model.CanOperateSelfCharacter;
		}

		// Token: 0x06008CAE RID: 36014 RVA: 0x004106D4 File Offset: 0x0040E8D4
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b != 0)
				{
					if (b == 1)
					{
						ushort domainId = notification.DomainId;
						ushort num = domainId;
						if (num != 4)
						{
							if (num != 8)
							{
								if (num == 19)
								{
									this.HandlerMethodExtraDomain(notification, wrapper);
								}
							}
							else
							{
								this.HandlerMethodCombatDomain(notification, wrapper);
							}
						}
						else
						{
							this.HandlerMethodCharacterDomain(notification, wrapper);
						}
					}
				}
				else
				{
					DataUid uid = notification.Uid;
					ushort domainId2 = uid.DomainId;
					ushort num2 = domainId2;
					if (num2 == 8)
					{
						this.HandlerDataCombatDomain(notification, wrapper);
					}
				}
			}
		}

		// Token: 0x06008CAF RID: 36015 RVA: 0x004107B4 File Offset: 0x0040E9B4
		public void Update()
		{
			bool exist = UIElement.Encyclopedia.Exist;
			if (exist)
			{
				this.SwitchPauseState(true, ViewCombat.EPauseReason.Encyclopedia);
			}
			else
			{
				bool exist2 = UIElement.TutorialGuidingChapter.Exist;
				if (exist2)
				{
					bool flag = !this.IsPausing;
					if (flag)
					{
						this.SwitchPauseState(true, ViewCombat.EPauseReason.GuidingChapter);
					}
					this._guidingChapterOpen = true;
				}
				else
				{
					bool guidingChapterOpen = this._guidingChapterOpen;
					if (guidingChapterOpen)
					{
						this._guidingChapterOpen = false;
						bool flag2 = this._pauseReason == ViewCombat.EPauseReason.GuidingChapter;
						if (flag2)
						{
							this.SwitchPauseState(false, ViewCombat.EPauseReason.None);
						}
					}
					this.UpdateInnerRatioVisible();
					bool flag3 = this.autoFight != null && this.autoFight.isOn;
					if (flag3)
					{
						this.autoFight.transform.Rotate(0f, 0f, -90f * Time.deltaTime);
					}
					bool clickMaskShowing = this.clickMask.activeSelf;
					bool flag4 = CombatCommandKit.Pause.Check(this.Element, false, false, false, false, false) && !this.rawCreatePage.gameObject.activeSelf && this.combatTimeScaleToggle.PauseInteractable;
					if (flag4)
					{
						bool flag5 = !clickMaskShowing && !this.Model.SelfChangingTrick && !this.SettingAiOptions && !this._settingJumpThreshold && !this._selectingUseItem;
						if (flag5)
						{
							this.SwitchPauseState();
						}
						bool settingJumpThreshold = this._settingJumpThreshold;
						if (settingJumpThreshold)
						{
							this.jumpThresholdSetting.OnClickConfirm();
						}
						bool selfChangingTrick = this.Model.SelfChangingTrick;
						if (selfChangingTrick)
						{
							bool isPausing = this.IsPausing;
							if (isPausing)
							{
								this.SwitchPauseState(false, ViewCombat.EPauseReason.None);
							}
							this.OnClickConfirmChangeTrick();
						}
						bool flag6 = this.combatWheel.IsOpened && !this.IsPausing;
						if (flag6)
						{
							CombatDomainMethod.Call.SetTimeScale(Mathf.Min(this.combatTimeScaleToggle.DisplayTimeScale, 0.5f));
						}
					}
					bool flag7 = CombatCommandKit.AutoFight.Check(this.Element, false, false, false, false, false);
					if (flag7)
					{
						this.OnClickAutoFight();
					}
					bool flag8 = !this.IsPausing && this._selfAffectingDefendSkillId >= 0 && this._selfReserveData.NeedUseSkillId >= 0 && this.SettingData.AutoClearDefendInBlockAttackSkill;
					if (flag8)
					{
						CombatDomainMethod.Call.ClearDefendInBlockAttackSkill(this.Element.GameDataListenerId);
					}
					bool flag9 = UIManager.Instance.IsFocusElement(this.Element) && Input.GetKeyDown(KeyCode.Mouse1) && RectTransformUtility.RectangleContainsScreenPoint(this.moveClickArea, Input.mousePosition, UIManager.Instance.UiCamera) && (!(this.combatWheel != null) || !this.combatWheel.IsOpened) && (!(this.combatSkillWheel != null) || !this.combatSkillWheel.IsOpened);
					if (flag9)
					{
						this._moveKeyDownList.Add(0);
					}
					bool flag10 = this._moveKeyDownList.Contains(0) && (!UIManager.Instance.IsFocusElement(this.Element) || !Input.GetKey(KeyCode.Mouse1));
					if (flag10)
					{
						this._moveKeyDownList.Remove(0);
					}
					bool flag11 = CombatCommandKit.MoveForward.Check(this.Element, false, true, false, false, false);
					if (flag11)
					{
						this._moveKeyDownList.Add(1);
					}
					bool flag12 = !CombatCommandKit.MoveForward.Check(this.Element, true, false, false, false, false) && this._moveKeyDownList.Contains(1);
					if (flag12)
					{
						this._moveKeyDownList.Remove(1);
					}
					bool flag13 = CombatCommandKit.MoveBackward.Check(this.Element, false, true, false, false, false);
					if (flag13)
					{
						this._moveKeyDownList.Add(2);
					}
					bool flag14 = !CombatCommandKit.MoveBackward.Check(this.Element, true, false, false, false, false) && this._moveKeyDownList.Contains(2);
					if (flag14)
					{
						this._moveKeyDownList.Remove(2);
					}
					bool flag15 = this._moveKeyDownList.Count == 0 && CombatCommandKit.MoveForward.Check(this.Element, true, false, false, false, false);
					if (flag15)
					{
						this._moveKeyDownList.Add(1);
					}
					bool flag16 = this._moveKeyDownList.Count == 0 && CombatCommandKit.MoveBackward.Check(this.Element, true, false, false, false, false);
					if (flag16)
					{
						this._moveKeyDownList.Add(2);
					}
					bool flag17 = this.Model.CombatStatus != 1;
					if (!flag17)
					{
						bool showingMercyIsAlly = this.Model.ShowingMercyIsAlly;
						if (!showingMercyIsAlly)
						{
							bool flag18 = CombatCommandKit.SpeedUp.Check(this.Element, false, false, false, false, false);
							if (flag18)
							{
								this.combatTimeScaleToggle.IncreaseSpeed();
							}
							bool flag19 = CombatCommandKit.SpeedDown.Check(this.Element, false, false, false, false, false);
							if (flag19)
							{
								this.combatTimeScaleToggle.DecreaseSpeed();
							}
							bool enableBulletTime = CombatCommandKit.BulletTime.Check(this.Element, true, false, false, true, false) && !this.rawCreatePage.gameObject.activeSelf;
							this.EnableBulletTime(enableBulletTime);
							bool flag20 = this.CanOpenWheel() && !clickMaskShowing;
							if (flag20)
							{
								bool flag21 = CombatCommandKit.OpenQuickWheel.Check(this.Element, false, false, false, true, false);
								if (flag21)
								{
									bool isOpened = this.combatSkillSortWidget.IsOpened;
									if (isOpened)
									{
										this.combatSkillSortWidget.Close();
									}
									bool isOpened2 = this.combatSkillWheel.IsOpened;
									if (isOpened2)
									{
										this.combatSkillWheel.Close();
									}
									bool isOpened3 = this.combatQuickUseItemPanel.IsOpened;
									if (isOpened3)
									{
										this.combatQuickUseItemPanel.Hide(true, true);
									}
									else
									{
										bool flag22 = !this.Model.CanOperateSelfCharacter;
										if (flag22)
										{
											return;
										}
										this.Model.RaiseEvent(ECombatEvents.OnClickCirclePanelButton);
										this.ShowQuickUseItemPanel();
									}
								}
							}
							bool flag23 = CombatCommandKit.OpenUseItemPanel.Check(this.Element, false, false, false, true, false);
							if (flag23)
							{
								bool activeSelf = this.combatUseItemPanel.gameObject.activeSelf;
								if (activeSelf)
								{
									this.combatUseItemPanel.Hide(true);
								}
								else
								{
									bool flag24 = !this.Model.CanOperateSelfCharacter;
									if (flag24)
									{
										return;
									}
									this.ShowSelectItemInCombat();
								}
							}
							bool flag25 = !this.Model.CanOperateSelfCharacter;
							if (!flag25)
							{
								bool flag26 = this._realTimeScale > 0f && this.Model.SelfCharacter != null;
								if (flag26)
								{
									MoveState moveState = MoveState.Stay;
									bool flag27 = this._moveKeyDownList.Count > 0;
									if (flag27)
									{
										List<sbyte> moveKeyDownList = this._moveKeyDownList;
										sbyte lastDownKeyType = moveKeyDownList[moveKeyDownList.Count - 1];
										bool flag28 = lastDownKeyType == 0;
										if (flag28)
										{
											float mousePosX = Input.mousePosition.x;
											float charPosX = RectTransformUtility.WorldToScreenPoint(UIManager.Instance.UiCamera, this.SelfCurrCharSkeleton.transform.position).x;
											bool flag29 = Mathf.Abs(mousePosX - charPosX) > 5f;
											if (flag29)
											{
												moveState = ((mousePosX > charPosX) ? MoveState.Forward : MoveState.Backward);
											}
										}
										else
										{
											moveState = ((lastDownKeyType == 1) ? MoveState.Forward : MoveState.Backward);
										}
									}
									CombatSubProcessorCharacter processor = this.Model.SelfCharacter;
									bool flag30 = processor.MoveState != moveState && (moveState != MoveState.Stay || processor.PlayerControllingMove);
									if (flag30)
									{
										CombatDomainMethod.Call.SetMoveState(moveState, true, true);
									}
								}
								bool flag31 = clickMaskShowing;
								if (!flag31)
								{
									bool flag32 = this.UpdateCheckTargetDistance();
									if (!flag32)
									{
										bool flag33 = CombatCommandKit.ShowHideTips.Check(this.Element, false, false, false, false, false);
										if (flag33)
										{
											GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
											settings.GlobalTipsHide = !settings.GlobalTipsHide;
											settings.SaveSettings();
										}
										bool flag34 = CombatCommandKit.ClearAgile.Check(this.Element, false, false, false, false, false);
										if (flag34)
										{
											this.moveSkillBreaker.DoBreak();
										}
										else
										{
											bool flag35 = CombatCommandKit.ClearDefend.Check(this.Element, false, false, false, false, false);
											if (flag35)
											{
												this.defendSkillBreakerExtra.DoBreak();
											}
											else
											{
												bool flag36 = CombatCommandKit.ClearAttack.Check(this.Element, false, false, false, false, false);
												if (flag36)
												{
													GEvent.OnEvent(UiEvents.OnTryInterruptSkillCasting, null);
												}
												bool flag37 = CombatCommandKit.ChangeTrickAttack.Check(this.Element, false, false, false, false, false);
												if (flag37)
												{
													bool flag38 = !this.Model.SelfChangingTrick;
													if (flag38)
													{
														this.OnClickChangeTrick();
													}
													else
													{
														this.OnClickConfirmChangeTrick();
													}
												}
												bool flag39 = this.Model.SelfChangingTrick && !UIElement.CharacterMenu.Exist && !this.rawCreatePage.gameObject.activeSelf && (CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false));
												if (flag39)
												{
													this.OnClickCancelChangeTrick();
												}
												bool selfChangingTrick2 = this.Model.SelfChangingTrick;
												if (!selfChangingTrick2)
												{
													bool flag40 = this.SettingAiOptions && !this.rawCreatePage.gameObject.activeSelf && (CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false));
													if (flag40)
													{
														this.aiOptionMask.Hide();
													}
													else
													{
														bool flag41 = this._settingJumpThreshold && !this.rawCreatePage.gameObject.activeSelf && (CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false));
														if (flag41)
														{
															this.HideJumpSetting();
														}
														else
														{
															foreach (Action checkSeries in this._hotKey2Action.CheckSeries(this.Element))
															{
																checkSeries();
															}
															bool normalAttackReserve = SingletonObject.getInstance<GlobalSettings>().NormalAttackReserve;
															if (normalAttackReserve)
															{
																bool flag42 = (CommonCommandKit.LeftMouse.Check(this.Element, true, false, false, false, false) && this._clickAttackButton) || CombatCommandKit.NormalAttack.Check(this.Element, true, false, false, false, false);
																if (flag42)
																{
																	bool flag43 = this._startContinuousNormalAttackTime == 0f;
																	if (flag43)
																	{
																		this._startContinuousNormalAttackTime = Time.unscaledTime;
																	}
																	bool flag44 = Time.unscaledTime - this._startContinuousNormalAttackTime > this._continuousNormalAttackStartUpTime;
																	if (flag44)
																	{
																		CombatSubProcessorCharacter processor2;
																		bool flag45 = this.Model.ProcessorCharacters.TryGetValue(this.Model.SelfCharId, out processor2) && !processor2.ReserveNormalAttack;
																		if (flag45)
																		{
																			this.DoRequestNormalAttackOrCancelReserve(false);
																		}
																	}
																}
																else
																{
																	this._startContinuousNormalAttackTime = 0f;
																}
															}
															GameObject weaponDurabilityNotEnoughTips = this.selfInfoChar.weaponDurabilityNotEnoughTips;
															bool flag46 = this.Model.CanOperateSelfCharacter && this.InAttackRange(true);
															if (flag46)
															{
																CombatSubProcessorCharacter selfProcessor = this.Model.SelfCharacter;
																bool flag47 = this._selfWeaponList != null && selfProcessor != null && selfProcessor.UsingWeaponIndex >= 0;
																if (flag47)
																{
																	ItemKey weaponKey = this._selfWeaponList[selfProcessor.UsingWeaponIndex];
																	CombatSubProcessorWeapon weapon;
																	weaponDurabilityNotEnoughTips.SetActive(selfProcessor.UsingWeaponIndex < 3 && this.Model.ProcessorWeapons.TryGetValue(weaponKey, out weapon) && weapon.Durability < 4);
																}
																bool flag48 = this._realTimeScale > 0f && !weaponDurabilityNotEnoughTips.activeSelf && UIManager.Instance.IsFocusElement(this.Element) && selfProcessor != null && selfProcessor.PreparingSkillId < 0 && selfProcessor.PreparingOtherAction < 0 && !selfProcessor.PreparingItem.IsValid() && !this.Model.SelfChangingTrick && !this.IsBoss(false);
																if (flag48)
																{
																	int weaponTemplateId = this.GetUsingWeaponTemplateId(true);
																	bool flag49 = weaponTemplateId >= 0;
																	if (flag49)
																	{
																		this._showAttackTipsTimer += Time.deltaTime;
																		bool flag50 = this._showAttackTipsTimer > 8f;
																		if (flag50)
																		{
																			this.selfInfoChar.attackTips.SetActive(this.SettingData.ShowCombatTutorial);
																		}
																	}
																}
															}
															else
															{
																weaponDurabilityNotEnoughTips.SetActive(false);
																this.HideAttackTips();
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06008CB0 RID: 36016 RVA: 0x00411424 File Offset: 0x0040F624
		private void LateUpdate()
		{
			this.UpdateCameraAndSkeletons();
		}

		// Token: 0x06008CB1 RID: 36017 RVA: 0x0041142E File Offset: 0x0040F62E
		private void UpdateCameraAndSkeletons()
		{
			this._virtualCamera.OnUpdate(Time.deltaTime);
			this.UpdateSkeletonsByVirtualCamera();
			this.UpdateParticleCameraOrthoSize(Time.deltaTime);
		}

		// Token: 0x06008CB2 RID: 36018 RVA: 0x00411458 File Offset: 0x0040F658
		private bool UpdateCheckTargetDistance()
		{
			int targetDistanceDelta = 0;
			bool isFirst = false;
			bool flag = CombatCommandKit.ToggleMoveTarget.Check(this.Element, false, false, false, false, false);
			bool result;
			if (flag)
			{
				this.ToggleMoveTarget();
				result = true;
			}
			else
			{
				bool flag2 = CombatCommandKit.TargetForwardRegion.Check(this.Element, false, true, false, false, false);
				if (flag2)
				{
					targetDistanceDelta = -10;
					isFirst = true;
				}
				else
				{
					bool flag3 = CombatCommandKit.TargetForwardRegion.Check(this.Element, true, false, false, false, false);
					if (flag3)
					{
						return false;
					}
					bool flag4 = CombatCommandKit.TargetForward.Check(this.Element, false, true, false, false, false);
					if (flag4)
					{
						targetDistanceDelta = -1;
						isFirst = true;
					}
					else
					{
						bool flag5 = CombatCommandKit.TargetForward.Check(this.Element, true, false, false, false, false) && Time.unscaledTimeAsDouble - this._lastSetTargetDistanceTime > this._lastSetTargetDistanceDelta;
						if (flag5)
						{
							targetDistanceDelta = -1;
						}
					}
				}
				bool flag6 = CombatCommandKit.TargetBackwardRegion.Check(this.Element, false, true, false, false, false);
				if (flag6)
				{
					targetDistanceDelta = 10;
					isFirst = true;
				}
				else
				{
					bool flag7 = CombatCommandKit.TargetBackwardRegion.Check(this.Element, true, false, false, false, false);
					if (flag7)
					{
						return false;
					}
					bool flag8 = CombatCommandKit.TargetBackward.Check(this.Element, false, true, false, false, false);
					if (flag8)
					{
						targetDistanceDelta = 1;
						isFirst = true;
					}
					else
					{
						bool flag9 = CombatCommandKit.TargetBackward.Check(this.Element, true, false, false, false, false) && Time.unscaledTimeAsDouble - this._lastSetTargetDistanceTime > this._lastSetTargetDistanceDelta;
						if (flag9)
						{
							targetDistanceDelta = 1;
						}
					}
				}
				bool flag10 = targetDistanceDelta != 0;
				if (flag10)
				{
					this.ChangeTargetDistance(targetDistanceDelta * (1 + this._setTargetDistanceTimes / this.setTargetDistanceDeltaPerTimes), isFirst);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06008CB3 RID: 36019 RVA: 0x004115F8 File Offset: 0x0040F7F8
		protected override void OnClick(Transform btn)
		{
			bool showingMercy = this.Model.ShowingMercy;
			if (!showingMercy)
			{
				string btnName = btn.name;
				bool flag = btnName == "AiOptionBtn";
				if (flag)
				{
					this.Model.RaiseEvent(ECombatEvents.OnClickAiOptionButton);
				}
				else
				{
					bool flag2 = btnName == "BtnCirclePanel";
					if (flag2)
					{
						bool flag3 = !this.Model.CanOperateSelfCharacter;
						if (!flag3)
						{
							this.Model.RaiseEvent(ECombatEvents.OnClickCirclePanelButton);
							this.ShowQuickUseItemPanel();
						}
					}
					else
					{
						bool flag4 = btnName == "AttackButton";
						if (flag4)
						{
							this.OnCheckNormalAttack();
						}
						else
						{
							bool flag5 = btnName == "UseItem";
							if (flag5)
							{
								this.DoRequestSelectItem();
							}
							else
							{
								bool flag6 = btnName == "SkillWheelButton";
								if (flag6)
								{
									this.combatSkillWheel.OpenAtPosition(Input.mousePosition, this._selfAffectingMoveSkillId, this._selfAffectingDefendSkillId);
								}
								else
								{
									bool flag7 = btnName == "ChangeCombatSkillSortButton";
									if (flag7)
									{
										this.combatSkillSortWidget.Open();
									}
									else
									{
										bool flag8 = btnName == "ChangeTrick";
										if (flag8)
										{
											this.OnClickChangeTrick();
										}
										else
										{
											bool flag9 = btnName == "ConfirmChangeTrick";
											if (flag9)
											{
												this.OnClickConfirmChangeTrick();
											}
											else
											{
												bool flag10 = btnName == "CancelChangeTrick";
												if (flag10)
												{
													this.OnClickCancelChangeTrick();
												}
												else
												{
													bool flag11 = btnName == "SpeedDown";
													if (flag11)
													{
														this.combatTimeScaleToggle.DecreaseSpeed();
													}
													else
													{
														bool flag12 = btnName == "SpeedUp";
														if (flag12)
														{
															this.combatTimeScaleToggle.IncreaseSpeed();
														}
														else
														{
															bool flag13 = btnName == "HealInjury";
															if (flag13)
															{
																bool flag14 = this.DoRequestOtherAction(0);
																if (flag14)
																{
																	this.HideAttackTips();
																	this.UpdateHealInjuryPoisonBtnTips(btn.GetComponent<TooltipInvoker>(), true);
																	this.combatQuickUseItemPanel.Hide(true, true);
																}
															}
															else
															{
																bool flag15 = btnName == "HealPoison";
																if (flag15)
																{
																	bool flag16 = this.DoRequestOtherAction(1);
																	if (flag16)
																	{
																		this.HideAttackTips();
																		this.UpdateHealInjuryPoisonBtnTips(btn.GetComponent<TooltipInvoker>(), false);
																		this.combatQuickUseItemPanel.Hide(true, true);
																	}
																}
																else
																{
																	bool flag17 = btnName == "Flee";
																	if (flag17)
																	{
																		bool flag18 = this.DoRequestOtherAction(2);
																		if (flag18)
																		{
																			this.HideAttackTips();
																			this.combatQuickUseItemPanel.Hide(true, true);
																		}
																	}
																	else
																	{
																		bool flag19 = btnName == "Surrender";
																		if (flag19)
																		{
																			bool flag20 = this.DoRequestOtherAction(4);
																			if (flag20)
																			{
																				this.HideAttackTips();
																				this.combatQuickUseItemPanel.Hide(true, true);
																			}
																		}
																		else
																		{
																			bool flag21 = btnName == "CarrierAttack";
																			if (flag21)
																			{
																				bool flag22 = this.DoRequestOtherAction(3);
																				if (flag22)
																				{
																					this.HideAttackTips();
																					this.combatQuickUseItemPanel.Hide(true, true);
																				}
																			}
																			else
																			{
																				bool flag23 = btnName == "UseGoldenWire";
																				if (flag23)
																				{
																					this.DoRequestUseSpecialItem(12, 275);
																				}
																				else
																				{
																					bool flag24 = btnName == "Details";
																					if (flag24)
																					{
																						bool flag25 = this.Model.SkillDamageData != null;
																						if (flag25)
																						{
																							UIManager.Instance.MaskUI(UIElement.CombatDamageDetail);
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06008CB4 RID: 36020 RVA: 0x00411968 File Offset: 0x0040FB68
		private void SetupEvents()
		{
			foreach (ICombatComponent component in this._components)
			{
				component.Setup();
			}
			this.Model.AddEvent(ECombatEvents.OnTimeScaleChanged, new OnCombatEvent(this.OnTimeScaleChanged));
			this.Model.AddEvent(ECombatEvents.OnCurrentDistanceChanged, new OnCombatEvent(this.OnCurrentDistanceChanged));
			this.Model.AddEvent(ECombatEvents.OnIsPuppetCombatChanged, new OnCombatEvent(this.OnIsPuppetCombatChanged));
			this.Model.AddEvent(ECombatEvents.OnTeamWisdomTypeChanged, new OnCombatEvent(this.OnTeamWisdomTypeChanged));
			this.Model.AddEvent(ECombatEvents.OnTeamWisdomCountChanged, new OnCombatEvent(this.RefreshUsingItemButtonWisdomCount));
			this.Model.AddEvent(ECombatEvents.CarrierAnimalCombatCharIdReady, new OnCombatEvent(this.OnCarrierAnimalCombatCharIdReady));
			this.Model.AddEvent(ECombatEvents.SpecialShowCombatCharIdReady, new OnCombatEvent(this.OnSpecialShowCombatCharIdReady));
			this.Model.AddEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
			this.Model.AddEvent(ECombatEvents.WaitingDelaySettlement, new OnCombatEvent(this.OnWaitingDelaySettlement));
			this.Model.AddEvent(ECombatEvents.CombatEnd, new OnCombatEvent(this.OnCombatEnd));
			this.Model.AddEvent(ECombatEvents.SetSelfLoopAnimationIdle, new OnCombatEvent(this.SetSelfLoopAnimationIdle));
			this.Model.AddEvent(ECombatEvents.SetSelfLoopAnimationHolding, new OnCombatEvent(this.SetSelfLoopAnimationHolding));
			this.Model.AddEvent(ECombatEvents.SetEnemyLoopAnimationHolding, new OnCombatEvent(this.SetEnemyLoopAnimationHolding));
			this.Model.AddEvent(ECombatEvents.DisableInteractByMercy, new OnCombatEvent(this.DisableInteractByMercy));
			this.Model.AddEvent(ECombatEvents.SwitchPauseState, new OnCombatEvent(this.SwitchPauseState));
			this.Model.AddEvent(ECombatEvents.OnCharacterMenuShowed, new OnCombatEvent(this.OnCharacterMenuShowed));
			this.Model.AddEvent(ECombatEvents.OnCharacterMenuHide, new OnCombatEvent(this.OnCharacterMenuHide));
			this.Model.AddEvent(ECombatEvents.BeforeCharacterMenuShowed, new OnCombatEvent(this.BeforeCharacterMenuShowed));
			this.Model.AddEvent(ECombatEvents.OnAiOptionPanelShow, new OnCombatEvent(this.OnAiOptionShow));
			this.Model.AddEvent(ECombatEvents.OnAiOptionPanelHide, new OnCombatEvent(this.OnAiOptionsHide));
			this.Model.AddEvent(ECombatEvents.CombatPrepared, new OnCombatEvent(this.UpdateTargetDistanceInteract));
			this.Model.AddEvent(ECombatEvents.CombatBeginReady, new OnCombatEvent(this.OnCombatBeginReady));
			this.Model.AddEvent(ECombatEvents.OnAiOptionsChanged, new OnCombatEvent(this.UpdateTargetDistanceInteract));
			this.Model.AddEvent(ECombatEvents.OnCirclePanelShow, new OnCombatEvent(this.OnCirclePanelShow));
			this.Model.AddEvent(ECombatEvents.OnCirclePanelHide, new OnCombatEvent(this.OnCirclePanelHide));
			this.Model.AddEvent(ECombatEvents.OnDamageDetailShow, new OnCombatEvent(this.OnDamageDetailShow));
			this.Model.AddEvent(ECombatEvents.OnDamageDetailHide, new OnCombatEvent(this.OnDamageDetailHide));
			this.Model.OnShowUseGoldenWireChanged += this.OnShowUseGoldenWireChanged;
			CombatModel model = this.Model;
			model.OnNeiliTypeChanged = (OnDataChangedEvent)Delegate.Combine(model.OnNeiliTypeChanged, new OnDataChangedEvent(this.UpdateNeiliType));
			CombatModel model2 = this.Model;
			model2.OnConsummateLevelChanged = (OnDataChangedEvent)Delegate.Combine(model2.OnConsummateLevelChanged, new OnDataChangedEvent(this.UpdateConsummateLevel));
			CombatModel model3 = this.Model;
			model3.OnMoveSpeedChanged = (OnDataChangedEvent)Delegate.Combine(model3.OnMoveSpeedChanged, new OnDataChangedEvent(this.UpdateMobilityTips));
			CombatModel model4 = this.Model;
			model4.OnWeaponsChanged = (OnDataChangedEvent)Delegate.Combine(model4.OnWeaponsChanged, new OnDataChangedEvent(this.OnWeaponsChanged));
			CombatModel model5 = this.Model;
			model5.OnUsingWeaponIndexChanged = (OnDataChangedEvent)Delegate.Combine(model5.OnUsingWeaponIndexChanged, new OnDataChangedEvent(this.OnUsingWeaponIndexChanged));
			CombatModel model6 = this.Model;
			model6.OnAttackSkillListChanged = (OnDataChangedEvent)Delegate.Combine(model6.OnAttackSkillListChanged, new OnDataChangedEvent(this.OnAttackSkillListChanged));
			CombatModel model7 = this.Model;
			model7.OnWeaponDurabilityChanged = (OnWeaponDataChangedEvent)Delegate.Combine(model7.OnWeaponDurabilityChanged, new OnWeaponDataChangedEvent(this.OnWeaponDurabilityChanged));
			CombatModel model8 = this.Model;
			model8.OnWeaponCanChangeToChanged = (OnWeaponDataChangedEvent)Delegate.Combine(model8.OnWeaponCanChangeToChanged, new OnWeaponDataChangedEvent(this.OnWeaponCanChangeToChanged));
			CombatModel model9 = this.Model;
			model9.OnWeaponCdFrameChanged = (OnWeaponDataChangedEvent)Delegate.Combine(model9.OnWeaponCdFrameChanged, new OnWeaponDataChangedEvent(this.OnWeaponCdFrameRelatedDataChanged));
			CombatModel model10 = this.Model;
			model10.OnWeaponFixedCdLeftFrameChanged = (OnWeaponDataChangedEvent)Delegate.Combine(model10.OnWeaponFixedCdLeftFrameChanged, new OnWeaponDataChangedEvent(this.OnWeaponCdFrameRelatedDataChanged));
			CombatModel model11 = this.Model;
			model11.OnWeaponFixedCdTotalFrameChanged = (OnWeaponDataChangedEvent)Delegate.Combine(model11.OnWeaponFixedCdTotalFrameChanged, new OnWeaponDataChangedEvent(this.OnWeaponCdFrameRelatedDataChanged));
			CombatModel model12 = this.Model;
			model12.OnCombatSkillCanUseChanged = (OnCombatSkillDataChangedEvent)Delegate.Combine(model12.OnCombatSkillCanUseChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillCanUseChanged));
			this.Model.OnGetProactiveSkillList += this.OnGetProactiveSkillList;
			CombatModel model13 = this.Model;
			model13.OnCharacterTemplateIdChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model13.OnCharacterTemplateIdChanged, new OnCharacterDataChangedEvent(this.OnCharacterTemplateIdChanged));
			CombatModel model14 = this.Model;
			model14.OnCharacterVisibleChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model14.OnCharacterVisibleChanged, new OnCharacterDataChangedEvent(this.OnCharacterVisibleChanged));
			CombatModel model15 = this.Model;
			model15.OnDisorderOfQiChanged = (OnDataChangedEvent)Delegate.Combine(model15.OnDisorderOfQiChanged, new OnDataChangedEvent(this.OnDisorderOfQiChanged));
			CombatModel model16 = this.Model;
			model16.OnAttackRangeChanged = (OnDataChangedEvent)Delegate.Combine(model16.OnAttackRangeChanged, new OnDataChangedEvent(this.OnAttackRangeChanged));
			CombatModel model17 = this.Model;
			model17.OnChangingTrickChanged = (OnDataChangedEvent)Delegate.Combine(model17.OnChangingTrickChanged, new OnDataChangedEvent(this.OnChangingTrickChanged));
			CombatModel model18 = this.Model;
			model18.OnChangeTrickAttackChanged = (OnDataChangedEvent)Delegate.Combine(model18.OnChangeTrickAttackChanged, new OnDataChangedEvent(this.OnChangeTrickAttackChanged));
			CombatModel model19 = this.Model;
			model19.OnSkillEffectCollectionChanged = (OnDataChangedEvent)Delegate.Combine(model19.OnSkillEffectCollectionChanged, new OnDataChangedEvent(this.OnSkillEffectCollectionChanged));
			CombatModel model20 = this.Model;
			model20.OnPreparingOtherActionChanged = (OnDataChangedEvent)Delegate.Combine(model20.OnPreparingOtherActionChanged, new OnDataChangedEvent(this.UpdateOtherActionPrepare));
			CombatModel model21 = this.Model;
			model21.OnOtherActionPreparePercentChanged = (OnDataChangedEvent)Delegate.Combine(model21.OnOtherActionPreparePercentChanged, new OnDataChangedEvent(this.UpdateOtherActionPrepare));
			CombatModel model22 = this.Model;
			model22.OnOtherActionCanUseChanged = (OnDataChangedEvent)Delegate.Combine(model22.OnOtherActionCanUseChanged, new OnDataChangedEvent(this.OnOtherActionCanUseChanged));
			CombatModel model23 = this.Model;
			model23.OnHealInjuryCountChanged = (OnDataChangedEvent)Delegate.Combine(model23.OnHealInjuryCountChanged, new OnDataChangedEvent(this.OnHealInjuryCountChanged));
			CombatModel model24 = this.Model;
			model24.OnHealPoisonCountChanged = (OnDataChangedEvent)Delegate.Combine(model24.OnHealPoisonCountChanged, new OnDataChangedEvent(this.OnHealPoisonCountChanged));
			CombatModel model25 = this.Model;
			model25.OnCanUseItemChanged = (OnDataChangedEvent)Delegate.Combine(model25.OnCanUseItemChanged, new OnDataChangedEvent(this.OnCanUseItemChanged));
			CombatModel model26 = this.Model;
			model26.OnPreparingItemChanged = (OnDataChangedEvent)Delegate.Combine(model26.OnPreparingItemChanged, new OnDataChangedEvent(this.OnPreparingItemChanged));
			CombatModel model27 = this.Model;
			model27.OnUseItemPreparePercentChanged = (OnDataChangedEvent)Delegate.Combine(model27.OnUseItemPreparePercentChanged, new OnDataChangedEvent(this.UpdateUseItemPrepare));
			CombatModel model28 = this.Model;
			model28.OnDefeatMarkCollectionChanged = (OnCharacterDataChangedEvent<DefeatMarkCollection>)Delegate.Combine(model28.OnDefeatMarkCollectionChanged, new OnCharacterDataChangedEvent<DefeatMarkCollection>(this.OnDefeatMarkCollectionChanged));
			CombatModel model29 = this.Model;
			model29.OnExecutingTeammateCommandChanged = (OnCharacterDataChangedEvent<sbyte>)Delegate.Combine(model29.OnExecutingTeammateCommandChanged, new OnCharacterDataChangedEvent<sbyte>(this.OnExecutingTeammateCommandChanged));
			CombatModel model30 = this.Model;
			model30.OnTeammateCommandPreparePercentChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model30.OnTeammateCommandPreparePercentChanged, new OnCharacterDataChangedEvent(this.OnTeammateCommandPreparePercentChanged));
			CombatModel model31 = this.Model;
			model31.OnTeammateCommandTimePercentChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model31.OnTeammateCommandTimePercentChanged, new OnCharacterDataChangedEvent(this.OnTeammateCommandTimePercentChanged));
			CombatModel model32 = this.Model;
			model32.OnPerformingSkillIdChanged = (OnDataChangedEvent)Delegate.Combine(model32.OnPerformingSkillIdChanged, new OnDataChangedEvent(this.OnPerformingSkillIdChanged));
			CombatModel model33 = this.Model;
			model33.OnHaveLeftArmChanged = (OnDataChangedEvent)Delegate.Combine(model33.OnHaveLeftArmChanged, new OnDataChangedEvent(this.OnBodyPartExistsChanged));
			CombatModel model34 = this.Model;
			model34.OnHaveRightArmChanged = (OnDataChangedEvent)Delegate.Combine(model34.OnHaveRightArmChanged, new OnDataChangedEvent(this.OnBodyPartExistsChanged));
			CombatModel model35 = this.Model;
			model35.OnHaveLeftLegChanged = (OnDataChangedEvent)Delegate.Combine(model35.OnHaveLeftLegChanged, new OnDataChangedEvent(this.OnBodyPartExistsChanged));
			CombatModel model36 = this.Model;
			model36.OnHaveRightLegChanged = (OnDataChangedEvent)Delegate.Combine(model36.OnHaveRightLegChanged, new OnDataChangedEvent(this.OnBodyPartExistsChanged));
			CombatModel model37 = this.Model;
			model37.OnOuterDamageValueChanged = (OnDataChangedEvent)Delegate.Combine(model37.OnOuterDamageValueChanged, new OnDataChangedEvent(this.OnOuterDamageValueChanged));
			CombatModel model38 = this.Model;
			model38.OnInnerDamageValueChanged = (OnDataChangedEvent)Delegate.Combine(model38.OnInnerDamageValueChanged, new OnDataChangedEvent(this.OnInnerDamageValueChanged));
			CombatModel model39 = this.Model;
			model39.OnOldInjuriesChanged = (OnDataChangedEvent)Delegate.Combine(model39.OnOldInjuriesChanged, new OnDataChangedEvent(this.OnOldInjuriesChanged));
			CombatModel model40 = this.Model;
			model40.OnOldPoisonChanged = (OnDataChangedEvent)Delegate.Combine(model40.OnOldPoisonChanged, new OnDataChangedEvent(this.OnOldPoisonChanged));
			CombatModel model41 = this.Model;
			model41.OnOldDisorderOfQiChanged = (OnDataChangedEvent)Delegate.Combine(model41.OnOldDisorderOfQiChanged, new OnDataChangedEvent(this.OnOldDisorderOfQiChanged));
			CombatModel model42 = this.Model;
			model42.OnCombatReserveDataChanged = (OnCharacterDataChangedEvent<CombatReserveData>)Delegate.Combine(model42.OnCombatReserveDataChanged, new OnCharacterDataChangedEvent<CombatReserveData>(this.OnCombatReserveDataChanged));
			CombatModel model43 = this.Model;
			model43.OnReserveNormalAttack = (OnDataChangedEvent)Delegate.Combine(model43.OnReserveNormalAttack, new OnDataChangedEvent(this.OnReserveNormalAttack));
			CombatModel model44 = this.Model;
			model44.OnMindRhythmChanged = (OnDataChangedEvent)Delegate.Combine(model44.OnMindRhythmChanged, new OnDataChangedEvent(this.OnMindRhythmChanged));
			CombatModel model45 = this.Model;
			model45.OnMindUpheavalTimeChanged = (OnDataChangedEvent)Delegate.Combine(model45.OnMindUpheavalTimeChanged, new OnDataChangedEvent(this.OnMindUpheavalTimeChanged));
			CombatModel model46 = this.Model;
			model46.OnMindUpheavalChanged = (OnDataChangedEvent)Delegate.Combine(model46.OnMindUpheavalChanged, new OnDataChangedEvent(this.OnMindUpheavalChanged));
			CombatModel model47 = this.Model;
			model47.OnBossPhaseChanged = (OnDataChangedEvent)Delegate.Combine(model47.OnBossPhaseChanged, new OnDataChangedEvent(this.OnBossPhaseChanged));
			CombatModel model48 = this.Model;
			model48.OnAnimationToPlayOnceChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model48.OnAnimationToPlayOnceChanged, new OnCharacterDataChangedEvent(this.OnAnimationToPlayOnceChanged));
			CombatModel model49 = this.Model;
			model49.OnSoundToLoopChanged = (OnCharacterDataChangedEvent)Delegate.Combine(model49.OnSoundToLoopChanged, new OnCharacterDataChangedEvent(this.OnSoundToLoopChanged));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
			GEvent.Add(EEvents.OnConfirmQuitGameState, new GEvent.Callback(this.OnConfirmQuitGameState));
			GEvent.Add(UiEvents.OnNeedCombatPause, new GEvent.Callback(this.OnNeedCombatPause));
			GEvent.Add(UiEvents.OnNeedCombatResume, new GEvent.Callback(this.OnNeedCombatResume));
			GEvent.Add(UiEvents.PlayCombatSoundOnce, new GEvent.Callback(this.PlayCombatSoundOnce));
			GEvent.Add(UiEvents.ShowCombatTeammateCommand, new GEvent.Callback(this.ShowCombatTeammateCommand));
			GEvent.Add(UiEvents.ShowCombatSpecialEffectTips, new GEvent.Callback(this.ShowCombatSpecialEffectTips));
			GEvent.Add(UiEvents.ShowCombatSurrenderMark, new GEvent.Callback(this.ShowCombatSurrenderMark));
			GEvent.Add(UiEvents.ShowFleeAnimation, new GEvent.Callback(this.ShowFleeAnimation));
			GEvent.Add(UiEvents.ShowCombatTianSuiBaoLu, new GEvent.Callback(this.ShowCombatTianSuiBaoLu));
			GEvent.Add(UiEvents.ShowAbsorbNeiliAllocation, new GEvent.Callback(this.ShowAbsorbNeiliAllocation));
			GEvent.Add(UiEvents.CombatTutorialSettingChanged, new GEvent.Callback(this.CombatTutorialSettingChanged));
		}

		// Token: 0x06008CB5 RID: 36021 RVA: 0x00412564 File Offset: 0x00410764
		private void CloseEvents()
		{
			foreach (ICombatComponent component in this._components)
			{
				component.Close();
			}
			this.Model.RemoveEvent(ECombatEvents.OnTimeScaleChanged, new OnCombatEvent(this.OnTimeScaleChanged));
			this.Model.RemoveEvent(ECombatEvents.OnCurrentDistanceChanged, new OnCombatEvent(this.OnCurrentDistanceChanged));
			this.Model.RemoveEvent(ECombatEvents.OnIsPuppetCombatChanged, new OnCombatEvent(this.OnIsPuppetCombatChanged));
			this.Model.RemoveEvent(ECombatEvents.OnTeamWisdomTypeChanged, new OnCombatEvent(this.OnTeamWisdomTypeChanged));
			this.Model.RemoveEvent(ECombatEvents.OnTeamWisdomCountChanged, new OnCombatEvent(this.RefreshUsingItemButtonWisdomCount));
			this.Model.RemoveEvent(ECombatEvents.CarrierAnimalCombatCharIdReady, new OnCombatEvent(this.OnCarrierAnimalCombatCharIdReady));
			this.Model.RemoveEvent(ECombatEvents.SpecialShowCombatCharIdReady, new OnCombatEvent(this.OnSpecialShowCombatCharIdReady));
			this.Model.RemoveEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
			this.Model.RemoveEvent(ECombatEvents.WaitingDelaySettlement, new OnCombatEvent(this.OnWaitingDelaySettlement));
			this.Model.RemoveEvent(ECombatEvents.CombatEnd, new OnCombatEvent(this.OnCombatEnd));
			this.Model.RemoveEvent(ECombatEvents.SetSelfLoopAnimationIdle, new OnCombatEvent(this.SetSelfLoopAnimationIdle));
			this.Model.RemoveEvent(ECombatEvents.SetSelfLoopAnimationHolding, new OnCombatEvent(this.SetSelfLoopAnimationHolding));
			this.Model.RemoveEvent(ECombatEvents.SetEnemyLoopAnimationHolding, new OnCombatEvent(this.SetEnemyLoopAnimationHolding));
			this.Model.RemoveEvent(ECombatEvents.DisableInteractByMercy, new OnCombatEvent(this.DisableInteractByMercy));
			this.Model.RemoveEvent(ECombatEvents.SwitchPauseState, new OnCombatEvent(this.SwitchPauseState));
			this.Model.RemoveEvent(ECombatEvents.OnCharacterMenuShowed, new OnCombatEvent(this.OnCharacterMenuShowed));
			this.Model.RemoveEvent(ECombatEvents.OnCharacterMenuHide, new OnCombatEvent(this.OnCharacterMenuHide));
			this.Model.RemoveEvent(ECombatEvents.BeforeCharacterMenuShowed, new OnCombatEvent(this.BeforeCharacterMenuShowed));
			this.Model.RemoveEvent(ECombatEvents.OnAiOptionPanelShow, new OnCombatEvent(this.OnAiOptionShow));
			this.Model.RemoveEvent(ECombatEvents.OnAiOptionPanelHide, new OnCombatEvent(this.OnAiOptionsHide));
			this.Model.RemoveEvent(ECombatEvents.CombatPrepared, new OnCombatEvent(this.UpdateTargetDistanceInteract));
			this.Model.RemoveEvent(ECombatEvents.CombatBeginReady, new OnCombatEvent(this.OnCombatBeginReady));
			this.Model.RemoveEvent(ECombatEvents.OnAiOptionsChanged, new OnCombatEvent(this.UpdateTargetDistanceInteract));
			this.Model.RemoveEvent(ECombatEvents.OnCirclePanelShow, new OnCombatEvent(this.OnCirclePanelShow));
			this.Model.RemoveEvent(ECombatEvents.OnCirclePanelHide, new OnCombatEvent(this.OnCirclePanelHide));
			this.Model.RemoveEvent(ECombatEvents.OnDamageDetailShow, new OnCombatEvent(this.OnDamageDetailShow));
			this.Model.RemoveEvent(ECombatEvents.OnDamageDetailHide, new OnCombatEvent(this.OnDamageDetailHide));
			this.Model.OnShowUseGoldenWireChanged -= this.OnShowUseGoldenWireChanged;
			CombatModel model = this.Model;
			model.OnNeiliTypeChanged = (OnDataChangedEvent)Delegate.Remove(model.OnNeiliTypeChanged, new OnDataChangedEvent(this.UpdateNeiliType));
			CombatModel model2 = this.Model;
			model2.OnConsummateLevelChanged = (OnDataChangedEvent)Delegate.Remove(model2.OnConsummateLevelChanged, new OnDataChangedEvent(this.UpdateConsummateLevel));
			CombatModel model3 = this.Model;
			model3.OnMoveSpeedChanged = (OnDataChangedEvent)Delegate.Remove(model3.OnMoveSpeedChanged, new OnDataChangedEvent(this.UpdateMobilityTips));
			CombatModel model4 = this.Model;
			model4.OnWeaponsChanged = (OnDataChangedEvent)Delegate.Remove(model4.OnWeaponsChanged, new OnDataChangedEvent(this.OnWeaponsChanged));
			CombatModel model5 = this.Model;
			model5.OnUsingWeaponIndexChanged = (OnDataChangedEvent)Delegate.Remove(model5.OnUsingWeaponIndexChanged, new OnDataChangedEvent(this.OnUsingWeaponIndexChanged));
			CombatModel model6 = this.Model;
			model6.OnAttackSkillListChanged = (OnDataChangedEvent)Delegate.Remove(model6.OnAttackSkillListChanged, new OnDataChangedEvent(this.OnAttackSkillListChanged));
			CombatModel model7 = this.Model;
			model7.OnWeaponDurabilityChanged = (OnWeaponDataChangedEvent)Delegate.Remove(model7.OnWeaponDurabilityChanged, new OnWeaponDataChangedEvent(this.OnWeaponDurabilityChanged));
			CombatModel model8 = this.Model;
			model8.OnWeaponCanChangeToChanged = (OnWeaponDataChangedEvent)Delegate.Remove(model8.OnWeaponCanChangeToChanged, new OnWeaponDataChangedEvent(this.OnWeaponCanChangeToChanged));
			CombatModel model9 = this.Model;
			model9.OnWeaponCdFrameChanged = (OnWeaponDataChangedEvent)Delegate.Remove(model9.OnWeaponCdFrameChanged, new OnWeaponDataChangedEvent(this.OnWeaponCdFrameRelatedDataChanged));
			CombatModel model10 = this.Model;
			model10.OnWeaponFixedCdLeftFrameChanged = (OnWeaponDataChangedEvent)Delegate.Remove(model10.OnWeaponFixedCdLeftFrameChanged, new OnWeaponDataChangedEvent(this.OnWeaponCdFrameRelatedDataChanged));
			CombatModel model11 = this.Model;
			model11.OnWeaponFixedCdTotalFrameChanged = (OnWeaponDataChangedEvent)Delegate.Remove(model11.OnWeaponFixedCdTotalFrameChanged, new OnWeaponDataChangedEvent(this.OnWeaponCdFrameRelatedDataChanged));
			CombatModel model12 = this.Model;
			model12.OnCombatSkillCanUseChanged = (OnCombatSkillDataChangedEvent)Delegate.Remove(model12.OnCombatSkillCanUseChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillCanUseChanged));
			this.Model.OnGetProactiveSkillList -= this.OnGetProactiveSkillList;
			CombatModel model13 = this.Model;
			model13.OnCharacterTemplateIdChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model13.OnCharacterTemplateIdChanged, new OnCharacterDataChangedEvent(this.OnCharacterTemplateIdChanged));
			CombatModel model14 = this.Model;
			model14.OnCharacterVisibleChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model14.OnCharacterVisibleChanged, new OnCharacterDataChangedEvent(this.OnCharacterVisibleChanged));
			CombatModel model15 = this.Model;
			model15.OnDisorderOfQiChanged = (OnDataChangedEvent)Delegate.Remove(model15.OnDisorderOfQiChanged, new OnDataChangedEvent(this.OnDisorderOfQiChanged));
			CombatModel model16 = this.Model;
			model16.OnAttackRangeChanged = (OnDataChangedEvent)Delegate.Remove(model16.OnAttackRangeChanged, new OnDataChangedEvent(this.OnAttackRangeChanged));
			CombatModel model17 = this.Model;
			model17.OnChangingTrickChanged = (OnDataChangedEvent)Delegate.Remove(model17.OnChangingTrickChanged, new OnDataChangedEvent(this.OnChangingTrickChanged));
			CombatModel model18 = this.Model;
			model18.OnChangeTrickAttackChanged = (OnDataChangedEvent)Delegate.Remove(model18.OnChangeTrickAttackChanged, new OnDataChangedEvent(this.OnChangeTrickAttackChanged));
			CombatModel model19 = this.Model;
			model19.OnSkillEffectCollectionChanged = (OnDataChangedEvent)Delegate.Remove(model19.OnSkillEffectCollectionChanged, new OnDataChangedEvent(this.OnSkillEffectCollectionChanged));
			CombatModel model20 = this.Model;
			model20.OnPreparingOtherActionChanged = (OnDataChangedEvent)Delegate.Remove(model20.OnPreparingOtherActionChanged, new OnDataChangedEvent(this.UpdateOtherActionPrepare));
			CombatModel model21 = this.Model;
			model21.OnOtherActionPreparePercentChanged = (OnDataChangedEvent)Delegate.Remove(model21.OnOtherActionPreparePercentChanged, new OnDataChangedEvent(this.UpdateOtherActionPrepare));
			CombatModel model22 = this.Model;
			model22.OnOtherActionCanUseChanged = (OnDataChangedEvent)Delegate.Remove(model22.OnOtherActionCanUseChanged, new OnDataChangedEvent(this.OnOtherActionCanUseChanged));
			CombatModel model23 = this.Model;
			model23.OnHealInjuryCountChanged = (OnDataChangedEvent)Delegate.Remove(model23.OnHealInjuryCountChanged, new OnDataChangedEvent(this.OnHealInjuryCountChanged));
			CombatModel model24 = this.Model;
			model24.OnHealPoisonCountChanged = (OnDataChangedEvent)Delegate.Remove(model24.OnHealPoisonCountChanged, new OnDataChangedEvent(this.OnHealPoisonCountChanged));
			CombatModel model25 = this.Model;
			model25.OnCanUseItemChanged = (OnDataChangedEvent)Delegate.Remove(model25.OnCanUseItemChanged, new OnDataChangedEvent(this.OnCanUseItemChanged));
			CombatModel model26 = this.Model;
			model26.OnPreparingItemChanged = (OnDataChangedEvent)Delegate.Remove(model26.OnPreparingItemChanged, new OnDataChangedEvent(this.OnPreparingItemChanged));
			CombatModel model27 = this.Model;
			model27.OnUseItemPreparePercentChanged = (OnDataChangedEvent)Delegate.Remove(model27.OnUseItemPreparePercentChanged, new OnDataChangedEvent(this.UpdateUseItemPrepare));
			CombatModel model28 = this.Model;
			model28.OnDefeatMarkCollectionChanged = (OnCharacterDataChangedEvent<DefeatMarkCollection>)Delegate.Remove(model28.OnDefeatMarkCollectionChanged, new OnCharacterDataChangedEvent<DefeatMarkCollection>(this.OnDefeatMarkCollectionChanged));
			CombatModel model29 = this.Model;
			model29.OnExecutingTeammateCommandChanged = (OnCharacterDataChangedEvent<sbyte>)Delegate.Remove(model29.OnExecutingTeammateCommandChanged, new OnCharacterDataChangedEvent<sbyte>(this.OnExecutingTeammateCommandChanged));
			CombatModel model30 = this.Model;
			model30.OnTeammateCommandPreparePercentChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model30.OnTeammateCommandPreparePercentChanged, new OnCharacterDataChangedEvent(this.OnTeammateCommandPreparePercentChanged));
			CombatModel model31 = this.Model;
			model31.OnTeammateCommandTimePercentChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model31.OnTeammateCommandTimePercentChanged, new OnCharacterDataChangedEvent(this.OnTeammateCommandTimePercentChanged));
			CombatModel model32 = this.Model;
			model32.OnPerformingSkillIdChanged = (OnDataChangedEvent)Delegate.Remove(model32.OnPerformingSkillIdChanged, new OnDataChangedEvent(this.OnPerformingSkillIdChanged));
			CombatModel model33 = this.Model;
			model33.OnHaveLeftArmChanged = (OnDataChangedEvent)Delegate.Remove(model33.OnHaveLeftArmChanged, new OnDataChangedEvent(this.OnBodyPartExistsChanged));
			CombatModel model34 = this.Model;
			model34.OnHaveRightArmChanged = (OnDataChangedEvent)Delegate.Remove(model34.OnHaveRightArmChanged, new OnDataChangedEvent(this.OnBodyPartExistsChanged));
			CombatModel model35 = this.Model;
			model35.OnHaveLeftLegChanged = (OnDataChangedEvent)Delegate.Remove(model35.OnHaveLeftLegChanged, new OnDataChangedEvent(this.OnBodyPartExistsChanged));
			CombatModel model36 = this.Model;
			model36.OnHaveRightLegChanged = (OnDataChangedEvent)Delegate.Remove(model36.OnHaveRightLegChanged, new OnDataChangedEvent(this.OnBodyPartExistsChanged));
			CombatModel model37 = this.Model;
			model37.OnOuterDamageValueChanged = (OnDataChangedEvent)Delegate.Remove(model37.OnOuterDamageValueChanged, new OnDataChangedEvent(this.OnOuterDamageValueChanged));
			CombatModel model38 = this.Model;
			model38.OnInnerDamageValueChanged = (OnDataChangedEvent)Delegate.Remove(model38.OnInnerDamageValueChanged, new OnDataChangedEvent(this.OnInnerDamageValueChanged));
			CombatModel model39 = this.Model;
			model39.OnOldInjuriesChanged = (OnDataChangedEvent)Delegate.Remove(model39.OnOldInjuriesChanged, new OnDataChangedEvent(this.OnOldInjuriesChanged));
			CombatModel model40 = this.Model;
			model40.OnOldPoisonChanged = (OnDataChangedEvent)Delegate.Remove(model40.OnOldPoisonChanged, new OnDataChangedEvent(this.OnOldPoisonChanged));
			CombatModel model41 = this.Model;
			model41.OnOldDisorderOfQiChanged = (OnDataChangedEvent)Delegate.Remove(model41.OnOldDisorderOfQiChanged, new OnDataChangedEvent(this.OnOldDisorderOfQiChanged));
			CombatModel model42 = this.Model;
			model42.OnCombatReserveDataChanged = (OnCharacterDataChangedEvent<CombatReserveData>)Delegate.Remove(model42.OnCombatReserveDataChanged, new OnCharacterDataChangedEvent<CombatReserveData>(this.OnCombatReserveDataChanged));
			CombatModel model43 = this.Model;
			model43.OnReserveNormalAttack = (OnDataChangedEvent)Delegate.Remove(model43.OnReserveNormalAttack, new OnDataChangedEvent(this.OnReserveNormalAttack));
			CombatModel model44 = this.Model;
			model44.OnMindRhythmChanged = (OnDataChangedEvent)Delegate.Remove(model44.OnMindRhythmChanged, new OnDataChangedEvent(this.OnMindRhythmChanged));
			CombatModel model45 = this.Model;
			model45.OnMindUpheavalTimeChanged = (OnDataChangedEvent)Delegate.Remove(model45.OnMindUpheavalTimeChanged, new OnDataChangedEvent(this.OnMindUpheavalTimeChanged));
			CombatModel model46 = this.Model;
			model46.OnMindUpheavalChanged = (OnDataChangedEvent)Delegate.Remove(model46.OnMindUpheavalChanged, new OnDataChangedEvent(this.OnMindUpheavalChanged));
			CombatModel model47 = this.Model;
			model47.OnBossPhaseChanged = (OnDataChangedEvent)Delegate.Remove(model47.OnBossPhaseChanged, new OnDataChangedEvent(this.OnBossPhaseChanged));
			CombatModel model48 = this.Model;
			model48.OnAnimationToPlayOnceChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model48.OnAnimationToPlayOnceChanged, new OnCharacterDataChangedEvent(this.OnAnimationToPlayOnceChanged));
			CombatModel model49 = this.Model;
			model49.OnSoundToLoopChanged = (OnCharacterDataChangedEvent)Delegate.Remove(model49.OnSoundToLoopChanged, new OnCharacterDataChangedEvent(this.OnSoundToLoopChanged));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUiChanged));
			GEvent.Remove(EEvents.OnConfirmQuitGameState, new GEvent.Callback(this.OnConfirmQuitGameState));
			GEvent.Remove(UiEvents.OnNeedCombatPause, new GEvent.Callback(this.OnNeedCombatPause));
			GEvent.Remove(UiEvents.OnNeedCombatResume, new GEvent.Callback(this.OnNeedCombatResume));
			GEvent.Remove(UiEvents.PlayCombatSoundOnce, new GEvent.Callback(this.PlayCombatSoundOnce));
			GEvent.Remove(UiEvents.ShowCombatTeammateCommand, new GEvent.Callback(this.ShowCombatTeammateCommand));
			GEvent.Remove(UiEvents.ShowCombatSpecialEffectTips, new GEvent.Callback(this.ShowCombatSpecialEffectTips));
			GEvent.Remove(UiEvents.ShowCombatSurrenderMark, new GEvent.Callback(this.ShowCombatSurrenderMark));
			GEvent.Remove(UiEvents.ShowFleeAnimation, new GEvent.Callback(this.ShowFleeAnimation));
			GEvent.Remove(UiEvents.ShowCombatTianSuiBaoLu, new GEvent.Callback(this.ShowCombatTianSuiBaoLu));
			GEvent.Remove(UiEvents.ShowAbsorbNeiliAllocation, new GEvent.Callback(this.ShowAbsorbNeiliAllocation));
			GEvent.Remove(UiEvents.CombatTutorialSettingChanged, new GEvent.Callback(this.CombatTutorialSettingChanged));
		}

		// Token: 0x06008CB6 RID: 36022 RVA: 0x00413160 File Offset: 0x00411360
		private unsafe void OnDataReady()
		{
			*this._selfCurrCharId = this._selfTeam[0];
			*this._enemyCurrCharId = this._enemyTeam[0];
			List<int> allCharList = EasyPool.Get<List<int>>();
			allCharList.Clear();
			allCharList.AddRange(this._selfTeam);
			allCharList.AddRange(this._enemyTeam);
			CombatDomainMethod.Call.RequestSwordFragmentSkillIds(this.Element.GameDataListenerId);
			this.combatQuickUseItemPanel.Setup(*this._selfCurrCharId);
			foreach (int charId in allCharList)
			{
				base.AppendMonitorFieldId(new UIBase.MonitorDataField(8, 10, (ulong)charId, this._combatCharSubIds));
				this.Model.RequestProactiveSkillList(charId);
			}
			EasyPool.Free<List<int>>(allCharList);
			this.InitTeam(true);
			this.InitTeam(false);
			this.Model.RaiseEvent(ECombatEvents.OnDataReady);
		}

		// Token: 0x06008CB7 RID: 36023 RVA: 0x00413268 File Offset: 0x00411468
		private void OnChangeChar()
		{
			base.StartCoroutine(this.ChangeCombatChar(this.Model.ChangingFromCharId, this.Model.ChangingToCharId));
			GameObject reserveNormalAttack = this.selfInfoChar.reserveNormalAttack;
			reserveNormalAttack.gameObject.SetActive(false);
		}

		// Token: 0x06008CB8 RID: 36024 RVA: 0x004132B2 File Offset: 0x004114B2
		private void OnWaitingDelaySettlement()
		{
			this.clickMask.SetActive(true);
		}

		// Token: 0x06008CB9 RID: 36025 RVA: 0x004132C4 File Offset: 0x004114C4
		private void OnCombatEnd()
		{
			this.combatWheel.ForceClose();
			CombatSkillWheel combatSkillWheel = this.combatSkillWheel;
			if (combatSkillWheel != null)
			{
				combatSkillWheel.ForceClose();
			}
			this.combatSkillSortWidget.ForceClose();
			bool activeSelf = this.rawCreatePage.gameObject.activeSelf;
			if (activeSelf)
			{
				this.rawCreatePage.gameObject.SetActive(false);
			}
			this.ChangeTrickMaskSetActive(false);
			this.clickMask.SetActive(true);
			base.StartCoroutine(this.CombatOver());
		}

		// Token: 0x06008CBA RID: 36026 RVA: 0x00413344 File Offset: 0x00411544
		private void OnTimeScaleChanged()
		{
			bool flag = !this.SettingAiOptions && !this.Model.SelfChangingTrick;
			if (flag)
			{
				this.UpdateTimeScale(this._realTimeScale);
			}
		}

		// Token: 0x06008CBB RID: 36027 RVA: 0x0041337C File Offset: 0x0041157C
		private void OnCurrentDistanceChanged()
		{
			this.distance.text = (this.Model.Config.HideDistance ? LocalStringManager.Get(LanguageKey.LK_Combat_HiddenDistance) : ((float)this._distance / 10f).ToString("f1"));
			this.UpdateAttackRange(true);
			this.UpdateAttackRange(false);
			this.PlayEnemyEnterAttackRangeSound();
			this.UpdateDefendSkillRangeText();
			this.UpdateDefendSkillRangeText();
			bool activeSelf = this.selfTeammateAffectingDefSkill.gameObject.activeSelf;
			if (activeSelf)
			{
				this.UpdateDefendSkillRangeText();
			}
			bool activeSelf2 = this.enemyTeammateAffectingDefSkill.gameObject.activeSelf;
			if (activeSelf2)
			{
				this.UpdateDefendSkillRangeText();
			}
		}

		// Token: 0x06008CBC RID: 36028 RVA: 0x0041342C File Offset: 0x0041162C
		private void OnIsPuppetCombatChanged()
		{
			TooltipInvoker mouseTip = this._selfOtherActionHolder.surrender.GetComponent<TooltipInvoker>();
			mouseTip.PresetParam[0] = (this.Model.IsPuppetCombat ? LocalStringManager.Get(LanguageKey.LK_Combat_Puppet_SurrenderTitle) : LocalStringManager.Get(LanguageKey.LK_Combat_SurrenderTitle));
			mouseTip.PresetParam[1] = (this.Model.IsPuppetCombat ? LocalStringManager.Get(LanguageKey.LK_Combat_Puppet_SurrenderInfo) : LocalStringManager.Get(LanguageKey.LK_Combat_SurrenderInfo));
			mouseTip.encyclopediaConfigTypeId = 118;
		}

		// Token: 0x06008CBD RID: 36029 RVA: 0x004134A9 File Offset: 0x004116A9
		private void OnTeamWisdomTypeChanged()
		{
		}

		// Token: 0x06008CBE RID: 36030 RVA: 0x004134AC File Offset: 0x004116AC
		private void OnCarrierAnimalCombatCharIdReady()
		{
			int carrierAnimalCharId = this.Model.CarrierAnimalCombatCharId;
			bool flag = carrierAnimalCharId < 0;
			if (!flag)
			{
				base.AppendMonitorFieldId(new UIBase.MonitorDataField(8, 10, (ulong)carrierAnimalCharId, this._carrierAnimalSubIds));
				CharacterDisplayData displayData = this.Model.DisplayDataCache[carrierAnimalCharId];
				AnimalItem animalConfig;
				bool flag2 = this.TryGetAnimalConfig(carrierAnimalCharId, out animalConfig);
				if (flag2)
				{
					string assetPath = "RemakeResources/Combat/CombatAnimals/" + animalConfig.AssetFileName + "/" + animalConfig.AssetFileName;
					this.LoadSpecialCharAsset(carrierAnimalCharId, assetPath, this.selfCarrierAnimalSkeleton, null);
				}
				else
				{
					PredefinedLog.Show(8, string.Format("Unexcepted animalDisplayData {0}", displayData.TemplateId));
				}
			}
		}

		// Token: 0x06008CBF RID: 36031 RVA: 0x0041355C File Offset: 0x0041175C
		private void OnSpecialShowCombatCharIdReady()
		{
			int specialShowCharId = this.Model.SpecialShowCombatCharId;
			bool flag = specialShowCharId < 0;
			if (!flag)
			{
				base.AppendMonitorFieldId(new UIBase.MonitorDataField(8, 10, (ulong)specialShowCharId, this._specialShowCharSubIds));
				this.UpdateSkeleton(specialShowCharId, this.selfSpecialShowSkeleton);
			}
		}

		// Token: 0x06008CC0 RID: 36032 RVA: 0x004135A5 File Offset: 0x004117A5
		private void SetSelfLoopAnimationIdle()
		{
			this.SetLoopAni(this.Model.SelfCharId, "C_000");
		}

		// Token: 0x06008CC1 RID: 36033 RVA: 0x004135BE File Offset: 0x004117BE
		private void SetSelfLoopAnimationHolding()
		{
			this.SetLoopAni(this.Model.SelfCharId, "C_007");
		}

		// Token: 0x06008CC2 RID: 36034 RVA: 0x004135D7 File Offset: 0x004117D7
		private void SetEnemyLoopAnimationHolding()
		{
			this.SetLoopAni(this.Model.EnemyCharId, "C_007");
		}

		// Token: 0x06008CC3 RID: 36035 RVA: 0x004135F0 File Offset: 0x004117F0
		private void DisableInteractByMercy()
		{
			this.EnableBulletTime(false);
			this.ChangeTrickMaskSetActive(false);
			bool flag = this.Model.ShowingMercyIsAlly ? this.Model.IsEnemyMainChar : this.Model.IsAllyMainChar;
			if (flag)
			{
				this.combatTimeScaleToggle.PauseInteractable = false;
			}
		}

		// Token: 0x06008CC4 RID: 36036 RVA: 0x00413644 File Offset: 0x00411844
		private void OnShowUseGoldenWireChanged(SpecialMiscData showUseGoldenWire)
		{
			this.enemyInfoChar.useGoldenWireBack.SetActive(showUseGoldenWire.CanUse);
			this.enemyInfoChar.useGoldenWireChance.text = this.GetCaptureChanceText(showUseGoldenWire.Chance);
		}

		// Token: 0x06008CC5 RID: 36037 RVA: 0x0041367C File Offset: 0x0041187C
		private void OnGetProactiveSkillList(int charId)
		{
			IReadOnlyList<CombatSkillDisplayData> displayDataList = this.Model.ProactiveSkillData[charId];
			int skillCount = displayDataList.Count;
			BossItem bossConfig;
			bool flag = this.TryGetBossConfig(this._selfTeam.Contains(charId) ? this.Model.SelfCharId : this.Model.EnemyCharId, out bossConfig);
			if (flag)
			{
				foreach (short playerCastSkill in bossConfig.PlayerCastSkills)
				{
					this.LoadCombatSkillAsset(playerCastSkill);
				}
				bool flag2 = bossConfig.FailPlayerAssetSkill >= 0;
				if (flag2)
				{
					this.LoadCombatSkillAsset(bossConfig.FailPlayerAssetSkill);
				}
			}
			else
			{
				for (int i = 0; i < skillCount; i++)
				{
					short skillId = displayDataList[i].TemplateId;
					this.LoadCombatSkillAsset(skillId);
				}
			}
			this.TryFinishLoad();
		}

		// Token: 0x06008CC6 RID: 36038 RVA: 0x00413784 File Offset: 0x00411984
		private void OnCharacterTemplateIdChanged(int charId)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				short charTemplateId = processor.TemplateId;
				bool flag2 = charId == this.Model.SelfCharId;
				if (flag2)
				{
					bool tutorialCombat = this.Model.IsTutorialCombat;
					this.autoFight.gameObject.SetActive(!tutorialCombat);
					this.aiOptionBtn.gameObject.SetActive(!tutorialCombat);
				}
				bool isXiangshu = this.EnemyIsBoss(9) || this.EnemyIsBoss(14);
				bool flag3 = isXiangshu && charId == this.Model.EnemyCharId && !this._xiangshuSceneInitialized;
				if (flag3)
				{
					base.StartCoroutine(this.InitXiangshuScene(charTemplateId));
					this._xiangshuSceneInitialized = true;
				}
			}
		}

		// Token: 0x06008CC7 RID: 36039 RVA: 0x00413858 File Offset: 0x00411A58
		private void OnCharacterVisibleChanged(int charId)
		{
			CombatSubProcessorCharacterDisplay processorDisplay;
			bool flag = !this.Model.TryGetCharacterDisplayProcessor(charId, out processorDisplay);
			if (!flag)
			{
				bool flag2 = charId == this.Model.CarrierAnimalCombatCharId;
				if (flag2)
				{
					this.selfCarrierAnimalSkeleton.gameObject.SetActive(processorDisplay.Visible);
				}
				else
				{
					bool flag3 = charId == this.Model.SpecialShowCombatCharId;
					if (flag3)
					{
						this.selfSpecialShowSkeleton.gameObject.SetActive(processorDisplay.Visible);
					}
					else
					{
						this.OnNormalCharacterVisibleChanged(charId, processorDisplay);
					}
				}
			}
		}

		// Token: 0x06008CC8 RID: 36040 RVA: 0x004138DC File Offset: 0x00411ADC
		private void OnNormalCharacterVisibleChanged(int charId, CombatSubProcessorCharacterDisplay processorDisplay)
		{
			bool isAlly = this.Model.CharIsAlly(charId);
			IReadOnlyList<int> team = isAlly ? this._selfTeam : this._enemyTeam;
			int teammateIndex = team.IndexOf(charId) - 1;
			bool flag = teammateIndex >= 0;
			if (flag)
			{
				SkeletonAnimation teammateSkeleton = this.GetSkeleton(charId);
				bool visible = processorDisplay.Visible;
				if (visible)
				{
					RectTransform mainCharTransform = (isAlly ? this.SelfCurrCharSkeleton : this.EnemyCurrCharSkeleton).GetComponent<RectTransform>();
					float screenPos = mainCharTransform.anchoredPosition.x + (float)(isAlly ? -1500 : 1500);
					teammateSkeleton.GetComponent<RectTransform>().anchoredPosition = new Vector2(screenPos, mainCharTransform.anchoredPosition.y);
					teammateSkeleton.gameObject.SetActive(true);
				}
				else
				{
					teammateSkeleton.GetComponent<CombatSpineSkeleton>().commandPrepare.gameObject.SetActive(false);
					DOVirtual.DelayedCall(0.1f, delegate
					{
						teammateSkeleton.gameObject.SetActive(processorDisplay.Visible);
					}, true);
				}
				this.UpdateDefendBounceRange();
			}
		}

		// Token: 0x06008CC9 RID: 36041 RVA: 0x00413A18 File Offset: 0x00411C18
		private void OnDisorderOfQiChanged(bool isAlly)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				CombatInfoTop topRefers = isAlly ? this.selfInfoTop : this.enemyInfoTop;
				CombatDamageValueHolder holder = topRefers.damageValueHolder;
				holder.SetQiDisorder(charId, processor.DisorderOfQi);
			}
		}

		// Token: 0x06008CCA RID: 36042 RVA: 0x00413A88 File Offset: 0x00411C88
		private void OnAttackRangeChanged(bool isAlly)
		{
			this.UpdateAttackRange(isAlly);
			this.UpdateAttackRangeBar();
			if (isAlly)
			{
				this.PlayEnemyEnterAttackRangeSound();
			}
		}

		// Token: 0x06008CCB RID: 36043 RVA: 0x00413AB4 File Offset: 0x00411CB4
		private void OnChangingTrickChanged(bool isAlly)
		{
			bool flag = !isAlly;
			if (!flag)
			{
				CombatSubProcessorCharacter processor;
				bool flag2 = !this.Model.ProcessorCharacters.TryGetValue(this.Model.SelfCharId, out processor);
				if (!flag2)
				{
					bool changingTrick = processor.ChangingTrick;
					if (changingTrick)
					{
						this.ShowChangeTrick();
					}
					else
					{
						this.ChangeTrickMaskSetActive(false);
					}
				}
			}
		}

		// Token: 0x06008CCC RID: 36044 RVA: 0x00413B10 File Offset: 0x00411D10
		private void OnChangeTrickAttackChanged(bool isAlly)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				bool changeTrickAttack = processor.ChangeTrickAttack;
				bool flag2 = !changeTrickAttack;
				if (!flag2)
				{
					SkeletonAnimation charSkeleton = isAlly ? this.SelfCurrCharSkeleton : this.EnemyCurrCharSkeleton;
					ParticleSystem changeTrickParticle = charSkeleton.GetComponent<CombatSpineSkeleton>().changeTrickParticle;
					CombatUtils.PlayAndHideParticle(changeTrickParticle, changeTrickParticle.main.duration);
				}
			}
		}

		// Token: 0x06008CCD RID: 36045 RVA: 0x00413BA4 File Offset: 0x00411DA4
		private void OnSkillEffectCollectionChanged(bool isAlly)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				SkillEffectCollection effectCollection = processor.SkillEffectCollection;
				RectTransform effectHolder = (isAlly ? this.selfInfoTop : this.enemyInfoTop).skillEffectHolder;
				int effectIconIndex = 1;
				bool flag2 = effectCollection.EffectDict != null;
				if (flag2)
				{
					foreach (KeyValuePair<SkillEffectKey, short> effect in effectCollection.EffectDict)
					{
						TooltipInvoker effectTips = effectHolder.GetChild(effectIconIndex).GetComponent<TooltipInvoker>();
						CombatSkillItem skillConfig = CombatSkill.Instance[effect.Key.SkillId];
						effectTips.PresetParam[0] = skillConfig.Name;
						CombatSkillEffectDescriptionDisplayData effectDescription;
						effectTips.PresetParam[1] = (effectCollection.EffectDescriptionDict.TryGetValue(effect.Key, out effectDescription) ? CommonUtils.GetSpecialEffectDesc(effectDescription, isAlly) : CommonUtils.GetSpecialEffectDesc((int)effect.Key.SkillId, effect.Key.IsDirect, isAlly));
						effectTips.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = effect.Value.ToString();
						effectTips.gameObject.SetActive(true);
						effectIconIndex++;
					}
				}
				this.SetChangeTrickBodyType(effectCollection, isAlly);
				this.RefreshUsingItemButtonEffect(effectCollection, isAlly);
				for (int i = effectIconIndex; i < effectHolder.childCount; i++)
				{
					effectHolder.GetChild(i).gameObject.SetActive(false);
				}
				bool flag3 = effectIconIndex > 10;
				if (flag3)
				{
					effectHolder.GetComponent<HorizontalLayoutGroup>().spacing = -24f;
				}
				else
				{
					effectHolder.GetComponent<HorizontalLayoutGroup>().spacing = -11f;
				}
				LayoutRebuilder.ForceRebuildLayoutImmediate(effectHolder);
				CombatUtils.UpdateIconHolderVisible(effectHolder);
			}
		}

		// Token: 0x06008CCE RID: 36046 RVA: 0x00413DAC File Offset: 0x00411FAC
		private void OnOtherActionCanUseChanged(bool isAlly)
		{
			bool flag = !isAlly;
			if (!flag)
			{
				CombatSubProcessorCharacter processor;
				bool flag2 = !this.Model.ProcessorCharacters.TryGetValue(this.Model.SelfCharId, out processor);
				if (!flag2)
				{
					bool[] canUseList = processor.OtherActionCanUse;
					for (int i = 0; i < canUseList.Length; i++)
					{
						bool flag3 = i == 4;
						if (!flag3)
						{
							bool canUse = canUseList[i];
							CButton actionBtn = this._selfOtherActionHolder.otherActionTypeList[i];
							actionBtn.interactable = (this.Model.CanOperateSelfCharacter && canUse);
							bool flag4 = i < 2;
							if (flag4)
							{
								this.UpdateHealInjuryPoisonBtnTips(actionBtn.GetComponent<TooltipInvoker>(), i == 0);
							}
						}
					}
				}
			}
		}

		// Token: 0x06008CCF RID: 36047 RVA: 0x00413E6C File Offset: 0x0041206C
		private void OnHealInjuryCountChanged(bool isAlly)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				byte healCount = processor.HealInjuryCount;
				CombatInfoTop infoTop = isAlly ? this.selfInfoTop : this.enemyInfoTop;
				infoTop.healInjury.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = healCount.ToString();
				bool flag2 = !isAlly;
				if (!flag2)
				{
					sbyte actionType = 0;
					this._selfOtherActionHolder.otherActionTypeList[(int)actionType].GetComponent<CombatOtherActionType>().count.text = healCount.ToString();
					this.combatWheel.SetOtherActionCount(actionType, healCount);
					string tipText = this.BuildHealTipText(true, healCount);
					this.combatWheel.SetOtherActionTip(actionType, tipText);
					CButton holderBtn = this._selfOtherActionHolder.otherActionTypeList[(int)actionType];
					TooltipInvoker holderTip = holderBtn.GetComponent<TooltipInvoker>();
					holderTip.PresetParam[1] = tipText;
					bool showing = holderTip.Showing;
					if (showing)
					{
						holderTip.Refresh(false, 118);
					}
				}
			}
		}

		// Token: 0x06008CD0 RID: 36048 RVA: 0x00413F98 File Offset: 0x00412198
		private void OnHealPoisonCountChanged(bool isAlly)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				byte healCount = processor.HealPoisonCount;
				CombatInfoTop infoTop = isAlly ? this.selfInfoTop : this.enemyInfoTop;
				infoTop.healPoison.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = healCount.ToString();
				bool flag2 = !isAlly;
				if (!flag2)
				{
					sbyte actionType = 1;
					this._selfOtherActionHolder.otherActionTypeList[(int)actionType].GetComponent<CombatOtherActionType>().count.text = healCount.ToString();
					this.combatWheel.SetOtherActionCount(actionType, healCount);
					string tipText = this.BuildHealTipText(false, healCount);
					this.combatWheel.SetOtherActionTip(actionType, tipText);
					CButton holderBtn = this._selfOtherActionHolder.otherActionTypeList[(int)actionType];
					TooltipInvoker holderTip = holderBtn.GetComponent<TooltipInvoker>();
					holderTip.PresetParam[1] = tipText;
					bool showing = holderTip.Showing;
					if (showing)
					{
						holderTip.Refresh(false, 118);
					}
				}
			}
		}

		// Token: 0x06008CD1 RID: 36049 RVA: 0x004140C4 File Offset: 0x004122C4
		private string BuildHealTipText(bool isHealInjury, byte healCount)
		{
			string text = LocalStringManager.Get(isHealInjury ? LanguageKey.LK_Combat_HealInfo : LanguageKey.LK_Combat_DispelInfo);
			bool flag = healCount == 0;
			if (flag)
			{
				text = text + "\n" + LocalStringManager.Get(isHealInjury ? LanguageKey.LK_Heal_Injury_Disable_Tips_NoCount : LanguageKey.LK_Heal_Poison_Disable_Tips_NoCount).SetColor("brightred");
			}
			return text;
		}

		// Token: 0x06008CD2 RID: 36050 RVA: 0x00414120 File Offset: 0x00412320
		private string BuildHealTipText(bool isHealInjury)
		{
			CombatSubProcessorCharacter processor = this.Model.SelfCharacter;
			bool flag = processor == null;
			string result;
			if (flag)
			{
				result = LocalStringManager.Get(isHealInjury ? LanguageKey.LK_Combat_HealInfo : LanguageKey.LK_Combat_DispelInfo);
			}
			else
			{
				result = this.BuildHealTipText(isHealInjury, isHealInjury ? processor.HealInjuryCount : processor.HealPoisonCount);
			}
			return result;
		}

		// Token: 0x06008CD3 RID: 36051 RVA: 0x00414178 File Offset: 0x00412378
		private void OnCanUseItemChanged(bool isAlly)
		{
			bool flag = !isAlly;
			if (!flag)
			{
				int charId = this.Model.SelfCharId;
				CombatSubProcessorCharacter processor;
				bool flag2 = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
				if (!flag2)
				{
					CButton useItemBtn = this._selfOtherActionHolder.useItem;
					bool canUseItem = processor.CanUseItem;
					canUseItem = (canUseItem && this.Model.CanOperateSelfCharacter);
					useItemBtn.interactable = canUseItem;
				}
			}
		}

		// Token: 0x06008CD4 RID: 36052 RVA: 0x004141EC File Offset: 0x004123EC
		private void OnPreparingItemChanged(bool isAlly)
		{
			this.UpdateUseItemPrepare(isAlly);
			this.UpdateAttackRange(isAlly);
			if (isAlly)
			{
				this.UpdateAttackRangeBar();
			}
		}

		// Token: 0x06008CD5 RID: 36053 RVA: 0x00414218 File Offset: 0x00412418
		private void UpdateUseItemPrepare(bool isAlly)
		{
			CombatSubProcessorCharacter processor = isAlly ? this.Model.SelfCharacter : this.Model.EnemyCharacter;
			bool flag = processor == null;
			if (!flag)
			{
				byte progress = processor.UseItemPreparePercent;
				ItemKey itemKey = processor.PreparingItem;
				CombatInfoChar infoChar = isAlly ? this.selfInfoChar : this.enemyInfoChar;
				CombatPrepareProgress progressRefers = infoChar.prepareProgress;
				bool flag2 = itemKey.IsValid();
				if (flag2)
				{
					this.ShowCharPrepareProgress(progressRefers, ItemTemplateHelper.GetName(itemKey.ItemType, itemKey.TemplateId), (float)progress / 100f, itemKey, "", "");
				}
				else
				{
					progressRefers.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06008CD6 RID: 36054 RVA: 0x004142C0 File Offset: 0x004124C0
		private void OnDefeatMarkCollectionChanged(int charId, DefeatMarkCollection oldValue)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				DefeatMarkCollection marks = processor.DefeatMarkCollection;
				bool isAlly = this.Model.CharIsAlly(charId);
				bool isCurrChar = charId == (isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId);
				bool flag2 = isCurrChar;
				if (flag2)
				{
					this.UpdateDefeatMarks(isAlly, marks, oldValue);
				}
			}
		}

		// Token: 0x06008CD7 RID: 36055 RVA: 0x00414330 File Offset: 0x00412530
		private void BeforeCharacterMenuShowed()
		{
			this.SetSkeletonAndVfxTimePause(true);
			this.SetTimeScale(1f);
			foreach (RectTransform tips in this._tipsList)
			{
				bool flag = tips != null;
				if (flag)
				{
					CanvasGroup canvasGroup = tips.GetComponent<CanvasGroup>();
					bool flag2 = canvasGroup != null;
					if (flag2)
					{
						DOTween.Pause(canvasGroup);
					}
				}
			}
		}

		// Token: 0x06008CD8 RID: 36056 RVA: 0x004143C0 File Offset: 0x004125C0
		private void OnCharacterMenuShowed()
		{
			this.combatParticle.SetCameraEnabled(false);
		}

		// Token: 0x06008CD9 RID: 36057 RVA: 0x004143D0 File Offset: 0x004125D0
		private void OnCharacterMenuHide()
		{
			this.SetTimeScale(0f);
			this.SetSkeletonAndVfxTimePause(false);
			foreach (RectTransform tips in this._tipsList)
			{
				bool flag = tips != null;
				if (flag)
				{
					CanvasGroup canvasGroup = tips.GetComponent<CanvasGroup>();
					bool flag2 = canvasGroup != null;
					if (flag2)
					{
						DOTween.Play(canvasGroup);
					}
				}
			}
			this.combatParticle.SetCameraEnabled(true);
			this.UpdateSkillBreaker(this._selfAffectingMoveSkillId, this.moveSkillBreaker.gameObject);
			this.UpdateSkillBreaker(this._selfAffectingDefendSkillId, this.defendSkillBreakerExtra.gameObject);
		}

		// Token: 0x06008CDA RID: 36058 RVA: 0x0041449C File Offset: 0x0041269C
		private void OnExecutingTeammateCommandChanged(int charId, sbyte oldExecutingTeammateCommand)
		{
			bool isAlly = this.Model.CharIsAlly(charId);
			IReadOnlyList<int> team = isAlly ? this.Model.SelfTeam : this.Model.EnemyTeam;
			int teammateIndex = team.IndexOf(charId) - 1;
			bool flag = teammateIndex < 0;
			if (!flag)
			{
				CombatSubProcessorCharacter processor;
				bool flag2 = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
				if (!flag2)
				{
					this.UpdateExecutingTeammateCmd(isAlly, charId, processor.ExecutingTeammateCommand, oldExecutingTeammateCommand);
					bool flag3 = isAlly && charId == this.Model.SelfCharId;
					if (flag3)
					{
						bool autoFightMarkState = this._autoCombat || !this.Model.CanOperateCharacter(charId);
						this.UpdateAutoFightMark(autoFightMarkState);
						CButton healInjuryBtn = this._selfOtherActionHolder.otherActionTypeList[0];
						CButton healPoisonBtn = this._selfOtherActionHolder.otherActionTypeList[1];
						for (int i = 0; i < this._selfOtherActionHolder.transform.childCount; i++)
						{
							CButton actionBtn;
							bool hasButton = this._selfOtherActionHolder.transform.GetChild(i).TryGetComponent<CButton>(out actionBtn);
							bool flag4 = hasButton;
							if (flag4)
							{
								actionBtn.interactable = (this.Model.CanOperateCharacter(charId) && actionBtn != healInjuryBtn && actionBtn != healPoisonBtn);
							}
						}
					}
				}
			}
		}

		// Token: 0x06008CDB RID: 36059 RVA: 0x00414600 File Offset: 0x00412800
		private void OnTeammateCommandPreparePercentChanged(int charId)
		{
			bool isAlly = this.Model.CharIsAlly(charId);
			IReadOnlyList<int> team = isAlly ? this.Model.SelfTeam : this.Model.EnemyTeam;
			int teammateIndex = team.IndexOf(charId) - 1;
			bool flag = teammateIndex < 0;
			if (!flag)
			{
				CombatSubProcessorCharacter processor;
				bool flag2 = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
				if (!flag2)
				{
					sbyte executingTeammateCommand = processor.ExecutingTeammateCommand;
					bool flag3 = executingTeammateCommand < 0;
					if (!flag3)
					{
						byte preparePercent = processor.TeammateCommandPreparePercent;
						SkeletonAnimation skeleton = (executingTeammateCommand != 13) ? this.GetSkeleton(charId) : (isAlly ? this.SelfCurrCharSkeleton : this.EnemyCurrCharSkeleton);
						CombatSpineSkeleton skeletonRefers = skeleton.GetComponent<CombatSpineSkeleton>();
						GameObject cmdPrepareObj = skeletonRefers.commandPrepare.gameObject;
						bool showPrepareBar = preparePercent > 0;
						bool flag4 = showPrepareBar && !cmdPrepareObj.activeSelf;
						if (flag4)
						{
							skeletonRefers.commandName.text = TeammateCommand.Instance[executingTeammateCommand].Name;
						}
						cmdPrepareObj.SetActive(showPrepareBar);
						skeletonRefers.commandPrepareBar.fillAmount = (float)preparePercent / 100f;
					}
				}
			}
		}

		// Token: 0x06008CDC RID: 36060 RVA: 0x00414728 File Offset: 0x00412928
		private void OnTeammateCommandTimePercentChanged(int charId)
		{
			bool isAlly = this.Model.CharIsAlly(charId);
			IReadOnlyList<int> team = isAlly ? this.Model.SelfTeam : this.Model.EnemyTeam;
			int teammateIndex = team.IndexOf(charId) - 1;
			bool flag = teammateIndex < 0;
			if (!flag)
			{
				CombatSubProcessorCharacter processor;
				bool flag2 = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
				if (!flag2)
				{
					byte percent = processor.TeammateCommandTimePercent;
					sbyte executingTeammateCommand = processor.ExecutingTeammateCommand;
					bool flag3 = executingTeammateCommand < 0;
					if (!flag3)
					{
						ETeammateCommandImplement implement = TeammateCommand.Instance[executingTeammateCommand].Implement;
						bool flag4 = !implement.IsDefend();
						if (!flag4)
						{
							CombatAffectingDefendSkill teammateDefendSkillRefers = isAlly ? this.selfTeammateAffectingDefSkill : this.enemyTeammateAffectingDefSkill;
							teammateDefendSkillRefers.progressBar.sizeDelta = new Vector2(52f + (float)(192 * percent) / 100f, 36f);
						}
					}
				}
			}
		}

		// Token: 0x06008CDD RID: 36061 RVA: 0x0041481A File Offset: 0x00412A1A
		private void OnBodyPartExistsChanged(bool isAlly)
		{
			this.UpdateDefeatMarkBar(isAlly);
			this.OnOuterDamageValueChanged(isAlly);
			this.OnInnerDamageValueChanged(isAlly);
		}

		// Token: 0x06008CDE RID: 36062 RVA: 0x00414838 File Offset: 0x00412A38
		private void OnOuterDamageValueChanged(bool isAlly)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				CombatDamageValueHolder damageValueHolder = (isAlly ? this.selfInfoTop : this.enemyInfoTop).damageValueHolder;
				int[] outerDamageValue = processor.OuterDamageValue;
				bool flag2 = outerDamageValue != null;
				if (flag2)
				{
					damageValueHolder.SetOuter(outerDamageValue, processor);
				}
			}
		}

		// Token: 0x06008CDF RID: 36063 RVA: 0x004148B4 File Offset: 0x00412AB4
		private void OnInnerDamageValueChanged(bool isAlly)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				CombatDamageValueHolder damageValueHolder = (isAlly ? this.selfInfoTop : this.enemyInfoTop).damageValueHolder;
				int[] innerDamageValue = processor.InnerDamageValue;
				bool flag2 = innerDamageValue != null;
				if (flag2)
				{
					damageValueHolder.SetInner(innerDamageValue, processor);
				}
			}
		}

		// Token: 0x06008CE0 RID: 36064 RVA: 0x0041492D File Offset: 0x00412B2D
		private void OnOldInjuriesChanged(bool isAlly)
		{
			this.UpdateDefeatMarkBar(isAlly);
		}

		// Token: 0x06008CE1 RID: 36065 RVA: 0x00414938 File Offset: 0x00412B38
		private void OnOldPoisonChanged(bool isAlly)
		{
			this.UpdateDefeatMarkBar(isAlly);
		}

		// Token: 0x06008CE2 RID: 36066 RVA: 0x00414943 File Offset: 0x00412B43
		private void OnOldDisorderOfQiChanged(bool isAlly)
		{
			this.UpdateDefeatMarkBar(isAlly);
		}

		// Token: 0x06008CE3 RID: 36067 RVA: 0x00414950 File Offset: 0x00412B50
		private void OnCombatReserveDataChanged(int charId, CombatReserveData combatReserveData)
		{
			bool flag = charId == this.Model.SelfCharId;
			if (flag)
			{
				this._selfReserveData = combatReserveData;
				this.UpdateReserveStatus();
				this.UpdateCombatCircleReserveStatus();
			}
		}

		// Token: 0x06008CE4 RID: 36068 RVA: 0x00414987 File Offset: 0x00412B87
		private void OnReserveNormalAttack(bool isAlly)
		{
			this.UpdateSelfReserveNormalAttack(isAlly);
		}

		// Token: 0x06008CE5 RID: 36069 RVA: 0x00414994 File Offset: 0x00412B94
		private void OnMindRhythmChanged(bool isAlly)
		{
			CombatInfoChar infoChar = isAlly ? this.selfInfoChar : this.enemyInfoChar;
			CombatCharacterMind mind = infoChar.combatCharacterMind;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId, out processor);
			if (flag)
			{
				mind.mindRhythm.gameObject.SetActive(false);
			}
			else
			{
				CountdownData mindRhythm = processor.MindRhythm;
				mind.mindRhythm.gameObject.SetActive(mindRhythm.On);
				mind.mindRhythmValue.SetText(mindRhythm.Left.ToString(), true);
			}
		}

		// Token: 0x06008CE6 RID: 36070 RVA: 0x00414A44 File Offset: 0x00412C44
		private void OnMindUpheavalTimeChanged(bool isAlly)
		{
			CombatInfoChar infoChar = isAlly ? this.selfInfoChar : this.enemyInfoChar;
			CombatCharacterMind mind = infoChar.combatCharacterMind;
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (flag)
			{
				mind.mindRhythm.gameObject.SetActive(false);
			}
			else
			{
				CountdownData mindUpheavalTime = processor.MindUpheavalTime;
				mind.mindUpheavalTime.gameObject.SetActive(mindUpheavalTime.Progress > 0f);
				mind.mindUpheavalTimeValue.fillAmount = mindUpheavalTime.Progress;
			}
		}

		// Token: 0x06008CE7 RID: 36071 RVA: 0x00414AF4 File Offset: 0x00412CF4
		private void OnMindUpheavalChanged(bool isAlly)
		{
			CombatSelfInfoChar combatSelfInfoChar = isAlly ? this.selfInfoChar : this.enemyInfoChar;
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			this.combatParticle.PlayVfx(isAlly, this.GetSkeleton(charId), "Particle_Effect_MindUpheaval");
		}

		// Token: 0x06008CE8 RID: 36072 RVA: 0x00414B4C File Offset: 0x00412D4C
		private void OnBossPhaseChanged(bool isAlly)
		{
			if (!isAlly)
			{
				CombatSubProcessorCharacter enemyCharacter = this.Model.EnemyCharacter;
				sbyte enemyBossPhase = (enemyCharacter != null) ? enemyCharacter.BossPhase : 0;
				bool flag = enemyBossPhase <= 0;
				if (!flag)
				{
					bool flag2 = this.EnemyIsBoss(9);
					if (flag2)
					{
						this.XiangshuSceneChangePhase();
					}
					else
					{
						bool flag3 = this.EnemyIsBoss(10);
						if (flag3)
						{
							ViewCombat.<>c__DisplayClass362_0 CS$<>8__locals1 = new ViewCombat.<>c__DisplayClass362_0();
							CS$<>8__locals1.<>4__this = this;
							CS$<>8__locals1.sceneTransform = UI_CombatBackground.Instance.Scene.transform;
							bool flag4 = enemyBossPhase == 3;
							if (flag4)
							{
								DOVirtual.DelayedCall(1.3f, delegate
								{
									UI_CombatBackground.Instance.Scene.StartTransition(0);
								}, true).SetUpdate(false);
								DOVirtual.DelayedCall(2.14f, delegate
								{
									CS$<>8__locals1.sceneTransform.Find("1_1").gameObject.SetActive(false);
									CS$<>8__locals1.sceneTransform.Find("1_2").gameObject.SetActive(false);
									CS$<>8__locals1.sceneTransform.Find("1_3").gameObject.SetActive(false);
									CS$<>8__locals1.sceneTransform.Find("1_4").gameObject.SetActive(false);
									CS$<>8__locals1.sceneTransform.Find("1_5").gameObject.SetActive(false);
									CS$<>8__locals1.sceneTransform.Find("1_6").gameObject.SetActive(false);
									CS$<>8__locals1.sceneTransform.Find("1_7").gameObject.SetActive(false);
									CS$<>8__locals1.sceneTransform.Find("3_1").gameObject.SetActive(true);
									CS$<>8__locals1.sceneTransform.Find("3_2").gameObject.SetActive(true);
									CS$<>8__locals1.sceneTransform.Find("3_3").gameObject.SetActive(true);
									CS$<>8__locals1.sceneTransform.Find("3_4").gameObject.SetActive(true);
									CS$<>8__locals1.sceneTransform.Find("3_5").gameObject.SetActive(true);
									CS$<>8__locals1.sceneTransform.Find("4_7").gameObject.SetActive(true);
								}, true).SetUpdate(false);
							}
							else
							{
								bool flag5 = enemyBossPhase == 5;
								if (flag5)
								{
									DOVirtual.DelayedCall(26.7f, delegate
									{
										ParticleSystem fogParticle = CS$<>8__locals1.sceneTransform.Find("4_7/4c_eff").GetComponent<ParticleSystem>();
										fogParticle.gameObject.SetActive(true);
										fogParticle.Play(true);
										DOVirtual.DelayedCall(fogParticle.main.duration, delegate
										{
											fogParticle.transform.parent.gameObject.SetActive(false);
										}, true).SetUpdate(false);
									}, true).SetUpdate(false);
									DOVirtual.DelayedCall(26.7f, delegate
									{
										AudioManager.Instance.PlaySound(this._commonSoundDict["ui_battle_sword"], this._realTimeScale, false, 100);
									}, true).SetUpdate(false);
									DOVirtual.DelayedCall(31.7f, delegate
									{
										CS$<>8__locals1.sceneTransform.Find("3_1").gameObject.SetActive(false);
										CS$<>8__locals1.sceneTransform.Find("3_2").gameObject.SetActive(false);
										CS$<>8__locals1.sceneTransform.Find("3_3").gameObject.SetActive(false);
										CS$<>8__locals1.sceneTransform.Find("3_4").gameObject.SetActive(false);
										CS$<>8__locals1.sceneTransform.Find("3_5").gameObject.SetActive(false);
										CS$<>8__locals1.sceneTransform.Find("4_1").gameObject.SetActive(true);
										CS$<>8__locals1.sceneTransform.Find("4_2").gameObject.SetActive(true);
										CS$<>8__locals1.sceneTransform.Find("4_3").gameObject.SetActive(true);
										CS$<>8__locals1.sceneTransform.Find("4_4").gameObject.SetActive(true);
										CS$<>8__locals1.sceneTransform.Find("4_5").gameObject.SetActive(true);
										CS$<>8__locals1.sceneTransform.Find("4_6").gameObject.SetActive(true);
										for (int j = 1; j < 9; j++)
										{
											CS$<>8__locals1.sceneTransform.Find(string.Format("4_6/SG_j{0}", j)).gameObject.SetActive(false);
										}
									}, true).SetUpdate(false);
									for (int i = 1; i < 9; i++)
									{
										CS$<>8__locals1.<OnBossPhaseChanged>g__PlaySwordAni|5(31.7f + ((i == 5) ? 1.8f : ((i == 6) ? 2.2f : ((i == 7) ? 2.6f : 3f))), string.Format("4_6/SG_j{0}", i));
									}
									float selfPos = this.SelfCurrCharSkeleton.GetComponent<RectTransform>().anchoredPosition.x;
									float enemyPos = this.EnemyCurrCharSkeleton.GetComponent<RectTransform>().anchoredPosition.x;
									float screenDistanceBackup = Mathf.Abs(selfPos - enemyPos);
									short cameraActingDistance = 600;
									DOVirtual.DelayedCall(1f, delegate
									{
										CS$<>8__locals1.<>4__this._virtualCamera.SetTargetScaleManuallyByScreenDistance((float)cameraActingDistance);
										CS$<>8__locals1.<>4__this.combatParticle.DoCameraOrthoSize(3.85f, 1f);
									}, true).SetUpdate(false);
									Tweener tween = null;
									DOVirtual.DelayedCall(28f, delegate
									{
										tween = DOVirtual.Float((float)cameraActingDistance, screenDistanceBackup, 1f, delegate(float value)
										{
											CS$<>8__locals1.<>4__this._virtualCamera.SetTargetScaleManuallyByScreenDistance(value);
										});
									}, true).SetUpdate(false);
									DOVirtual.DelayedCall(29.1f, delegate
									{
										bool flag9 = tween != null;
										if (flag9)
										{
											tween.Kill(false);
										}
										CS$<>8__locals1.<>4__this._virtualCamera.UnlockTargetScale();
									}, true).SetUpdate(false);
								}
							}
						}
						else
						{
							bool flag6 = this.EnemyIsBoss(14);
							if (flag6)
							{
								this.XiangshuSceneChangePhase();
								int bossCharId = this.Model.EnemyCharId;
								this._transitioningCharId = bossCharId;
								SkeletonAnimation skeleton = this.EnemyCurrCharSkeleton;
								Spine.Animation failAnim = this.GetAnimation(skeleton, "boss18_C_005");
								bool flag7 = failAnim != null;
								if (flag7)
								{
									TrackEntry failTrack = skeleton.AnimationState.SetAnimation(0, failAnim, false);
									Spine.AnimationState.TrackEntryDelegate <>9__15;
									Action<SkeletonDataAsset> <>9__14;
									failTrack.Complete += delegate(TrackEntry entry)
									{
										this._transitioningCharId = -1;
										string assetPath = "RemakeResources/Combat/CombatBoss/boss18/boss18_1_SkeletonData";
										string assetPath2 = assetPath;
										Action<SkeletonDataAsset> onLoad;
										if ((onLoad = <>9__14) == null)
										{
											onLoad = (<>9__14 = delegate(SkeletonDataAsset skeletonData)
											{
												skeleton.skeletonDataAsset = skeletonData;
												skeleton.initialSkinName = "default";
												skeleton.ClearState();
												skeleton.Initialize(true, false);
												this.StartCoroutine(this.RegisterAniEvent(skeleton));
												TrackEntry entryTrack = this.PlayAni(bossCharId, "C_into", false);
												bool flag9 = entryTrack != null;
												if (flag9)
												{
													TrackEntry trackEntry = entryTrack;
													Spine.AnimationState.TrackEntryDelegate value;
													if ((value = <>9__15) == null)
													{
														value = (<>9__15 = delegate(TrackEntry entry2)
														{
															string loopAnim;
															bool flag10 = this._loopAniDict.TryGetValue(bossCharId, out loopAnim) && !string.IsNullOrEmpty(loopAnim) && skeleton.AnimationState != null;
															if (flag10)
															{
																this.PlayAni(bossCharId, loopAnim, true);
															}
														});
													}
													trackEntry.Complete += value;
												}
												this.combatParticle.PlayVfx(false, skeleton, "Particle_boss18_1_C_into");
											});
										}
										ResLoader.Load<SkeletonDataAsset>(assetPath2, onLoad, null, false);
									};
								}
							}
							else
							{
								bool flag8 = UI_CombatBackground.Instance.Scene.TransitionStageList.Count >= (int)enemyBossPhase;
								if (flag8)
								{
									UI_CombatBackground.Instance.Scene.StartTransition((int)(enemyBossPhase - 1));
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06008CE9 RID: 36073 RVA: 0x00414EB8 File Offset: 0x004130B8
		private void XiangshuSceneChangePhase()
		{
			Transform sceneTransform = UI_CombatBackground.Instance.Scene.transform;
			ParticleSystem fogParticle = sceneTransform.Find("2_7/2c_wu").GetComponent<ParticleSystem>();
			List<global::CombatScene.TransitionItem> itemsToShow = UI_CombatBackground.Instance.Scene.TransitionStageList[0].TransitionEndItems;
			fogParticle.gameObject.SetActive(true);
			fogParticle.Play(true);
			DOVirtual.DelayedCall(fogParticle.main.duration, delegate
			{
				fogParticle.transform.parent.gameObject.SetActive(false);
			}, true).SetUpdate(false);
			DOVirtual.DelayedCall(7f, delegate
			{
				sceneTransform.Find("2_0").gameObject.SetActive(false);
				sceneTransform.Find("2_1").gameObject.SetActive(false);
				sceneTransform.Find("2_2").gameObject.SetActive(false);
				sceneTransform.Find("2_3").gameObject.SetActive(false);
				sceneTransform.Find("2_4").gameObject.SetActive(false);
				sceneTransform.Find("2_5").gameObject.SetActive(false);
				sceneTransform.Find("2_6").gameObject.SetActive(false);
				for (int i = 0; i < itemsToShow.Count; i++)
				{
					itemsToShow[i].Target.SetActive(true);
				}
				this.UpdateVirtualCameraTargetData(this.GetCurrCharScreenPos(true), this.GetCurrCharScreenPos(false));
			}, true).SetUpdate(false);
		}

		// Token: 0x06008CEA RID: 36074 RVA: 0x00414F8C File Offset: 0x0041318C
		private void OnAnimationToPlayOnceChanged(int charId)
		{
			bool flag = this._transitioningCharId >= 0 && charId == this._transitioningCharId;
			if (!flag)
			{
				CombatSubProcessorCharacterDisplay processor;
				bool flag2 = !this.Model.TryGetCharacterDisplayProcessor(charId, out processor);
				if (!flag2)
				{
					string onceAni = processor.AnimationToPlayOnce;
					this.SetAniToPlayOnce(charId, onceAni);
				}
			}
		}

		// Token: 0x06008CEB RID: 36075 RVA: 0x00414FDC File Offset: 0x004131DC
		private void OnSoundToLoopChanged(int charId)
		{
			CombatSubProcessorCharacterDisplay processor;
			bool flag = !this.Model.TryGetCharacterDisplayProcessor(charId, out processor);
			if (!flag)
			{
				string soundToLoop = processor.SoundToLoop;
				this.SetLoopSound(charId, soundToLoop);
			}
		}

		// Token: 0x06008CEC RID: 36076 RVA: 0x00415014 File Offset: 0x00413214
		private void HandlerDataCombatDomain(Notification notification, NotificationWrapper wrapper)
		{
			DataUid uid = notification.Uid;
			bool flag = uid.DataId == 10;
			if (flag)
			{
				this.HandlerDataCombatChar(uid, notification.ValueOffset, wrapper.DataPool);
			}
			else
			{
				bool flag2 = uid.DataId == 34;
				if (flag2)
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._changeTrickIndex);
				}
				else
				{
					bool flag3 = uid.DataId == 35;
					if (flag3)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._changeTrickBodyPart);
					}
				}
			}
		}

		// Token: 0x06008CED RID: 36077 RVA: 0x0041509C File Offset: 0x0041329C
		private void HandlerMethodCharacterDomain(Notification notification, NotificationWrapper wrapper)
		{
			bool flag = notification.MethodId == 33;
			if (flag)
			{
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._selfMaxEatingSlotCount);
			}
			else
			{
				bool flag2 = notification.MethodId == 181;
				if (flag2)
				{
					bool isCarrierDurabilityRunningOut = false;
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref isCarrierDurabilityRunningOut);
					CButton button = this._selfOtherActionHolder.otherActionTypeList[3];
					button.interactable = !isCarrierDurabilityRunningOut;
					this.UpdateCarrierAttackTip();
				}
			}
		}

		// Token: 0x06008CEE RID: 36078 RVA: 0x00415124 File Offset: 0x00413324
		private void UpdateCarrierAttackTip()
		{
			CButton button = this._selfOtherActionHolder.otherActionTypeList[3];
			TooltipInvoker tip = button.GetComponent<TooltipInvoker>();
			tip.Type = TipType.ProfessionSkill;
			tip.PresetParam = null;
			ProfessionModel professionModel = SingletonObject.getInstance<ProfessionModel>();
			int skillId = 5;
			ValueTuple<ProfessionData, int> valueTuple = professionModel.FindProfessionDataBySkillId(skillId);
			ProfessionData professionData = valueTuple.Item1;
			int skillIndex = valueTuple.Item2;
			ResourceMonitor taiwuResourceMonitor = SingletonObject.getInstance<WorldMapModel>().TaiwuResources;
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("IsLocked", professionData == null || !professionData.IsSkillUnlocked(skillIndex));
			argBox.Set("ProfessionSkillId", skillId);
			argBox.SetObject("ProfessionData", professionData);
			argBox.Set("SkillIndex", skillIndex);
			argBox.Set("Exp", (taiwuResourceMonitor != null) ? taiwuResourceMonitor.Exp : 0);
			argBox.Set("DisableAdditionalRedText", true);
			argBox.SetObject("Resources", new ResourceInts(((taiwuResourceMonitor != null) ? taiwuResourceMonitor.Resources : null) ?? new int[8]));
			tip.RuntimeParam = argBox;
			bool showing = tip.Showing;
			if (showing)
			{
				tip.Refresh(false, -1);
			}
		}

		// Token: 0x06008CEF RID: 36079 RVA: 0x00415250 File Offset: 0x00413450
		private void HandlerMethodCombatDomain(Notification notification, NotificationWrapper wrapper)
		{
			bool flag = notification.MethodId == 37;
			if (flag)
			{
				bool success = false;
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref success);
				bool flag2 = !success;
				if (!flag2)
				{
					this.combatTimeScaleToggle.PauseInteractable = true;
					this.SetDisplayTimeScale(this.SettingData.CombatSpeed, false);
					this.ResetMobilityAvoid();
					bool flag3 = UIManager.Instance.IsFocusElement(UIElement.SystemOption);
					if (flag3)
					{
						this._showingSystemOption = true;
						bool flag4 = !this.IsPausing;
						if (flag4)
						{
							this._pausedBySystemMenu = true;
							this.OnShowConfirmDialog(true);
						}
						else
						{
							this._pausedBySystemMenu = false;
						}
					}
				}
			}
			else
			{
				bool flag5 = notification.MethodId == 99;
				if (flag5)
				{
					List<short> skillIds = new List<short>();
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref skillIds);
					foreach (short skillId in skillIds)
					{
						this.LoadCombatSkillAsset(skillId);
					}
				}
			}
		}

		// Token: 0x06008CF0 RID: 36080 RVA: 0x0041537C File Offset: 0x0041357C
		private void HandlerMethodExtraDomain(Notification notification, NotificationWrapper wrapper)
		{
			bool flag = notification.MethodId == 19;
			if (flag)
			{
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._hunterSkillUnlocked);
			}
		}

		// Token: 0x06008CF1 RID: 36081 RVA: 0x004153B4 File Offset: 0x004135B4
		private unsafe void HandlerDataCombatChar(DataUid uid, int valueOffset, RawDataPool dataPool)
		{
			int charId = (int)uid.SubId0;
			bool isAlly = this.Model.CharIsAlly(charId);
			bool isCurrChar = charId == (isAlly ? (*this._selfCurrCharId) : (*this._enemyCurrCharId));
			IReadOnlyList<int> readOnlyList = isAlly ? this._selfTeam : this._enemyTeam;
			DamageStepCollection damageStepCollection = isAlly ? this._selfDamageStepCollection : this._enemyDamageStepCollection;
			CombatDamageValueHolder damageValueHolder = (isAlly ? this.selfInfoTop : this.enemyInfoTop).damageValueHolder;
			bool flag = isCurrChar;
			if (flag)
			{
				uint subId = uid.SubId1;
				uint num = subId;
				if (num <= 80U)
				{
					if (num <= 36U)
					{
						switch (num)
						{
						case 9U:
							break;
						case 10U:
							goto IL_1095;
						case 11U:
						case 14U:
							goto IL_212;
						case 12U:
						{
							sbyte b = isAlly ? this._selfJumpPrepareProgress : this._enemyJumpPrepareProgress;
							bool flag2 = isAlly;
							if (flag2)
							{
								Serializer.Deserialize(dataPool, valueOffset, ref this._selfJumpPrepareProgress);
							}
							else
							{
								Serializer.Deserialize(dataPool, valueOffset, ref this._enemyJumpPrepareProgress);
							}
							this.UpdateJumpPrepare(isAlly);
							goto IL_1095;
						}
						case 13U:
						{
							short skillId = isAlly ? this._selfAffectingMoveSkillId : this._enemyAffectingMoveSkillId;
							sbyte curDistance = isAlly ? this._selfJumpPreparedDistance : this._enemyJumpPreparedDistance;
							bool flag3 = isAlly;
							if (flag3)
							{
								Serializer.Deserialize(dataPool, valueOffset, ref this._selfJumpPreparedDistance);
							}
							else
							{
								Serializer.Deserialize(dataPool, valueOffset, ref this._enemyJumpPreparedDistance);
							}
							bool flag4 = skillId == 126 && curDistance == 0 && (isAlly ? this._selfJumpPreparedDistance : this._enemyJumpPreparedDistance) >= 10;
							if (flag4)
							{
								bool flag5 = this.Model.TimeScale > 0f;
								if (flag5)
								{
									DOVirtual.DelayedCall(0.6f, delegate
									{
										AudioManager.Instance.PlaySound("battle_JindingMove2", false, false);
									}, true);
								}
								else
								{
									AudioManager.Instance.PlaySound("battle_JindingMove2", false, false);
								}
							}
							this.UpdateJumpPrepare(isAlly);
							goto IL_1095;
						}
						default:
							switch (num)
							{
							case 31U:
							{
								bool flag6 = isAlly;
								if (flag6)
								{
									Serializer.Deserialize(dataPool, valueOffset, ref this._selfInjuryAutoHealCollection);
								}
								else
								{
									Serializer.Deserialize(dataPool, valueOffset, ref this._enemyInjuryAutoHealCollection);
								}
								this.UpdateInjuryAutoHealProgress(isAlly);
								goto IL_1095;
							}
							case 32U:
								Serializer.Deserialize(dataPool, valueOffset, ref damageStepCollection);
								damageValueHolder.SetStep(damageStepCollection);
								goto IL_1095;
							case 33U:
							case 34U:
								goto IL_1095;
							case 35U:
							{
								int mindDamageValue = 0;
								Serializer.Deserialize(dataPool, valueOffset, ref mindDamageValue);
								damageValueHolder.SetMind(mindDamageValue);
								goto IL_1095;
							}
							case 36U:
							{
								int fatalDamageValue = 0;
								Serializer.Deserialize(dataPool, valueOffset, ref fatalDamageValue);
								damageValueHolder.SetFatal(fatalDamageValue);
								goto IL_1095;
							}
							default:
								goto IL_1095;
							}
							break;
						}
					}
					else
					{
						switch (num)
						{
						case 45U:
						{
							MindMarkList mindMarkList = null;
							bool flag7 = isAlly;
							if (flag7)
							{
								Serializer.Deserialize(dataPool, valueOffset, ref mindMarkList);
							}
							else
							{
								Serializer.Deserialize(dataPool, valueOffset, ref mindMarkList);
							}
							this.UpdateMindMarkTime(isAlly, mindMarkList);
							goto IL_1095;
						}
						case 46U:
						{
							PoisonInts poison = default(PoisonInts);
							Serializer.Deserialize(dataPool, valueOffset, ref poison);
							bool flag8 = isAlly;
							if (flag8)
							{
								this._selfPoisons = poison;
							}
							damageValueHolder.SetPoison(poison);
							goto IL_1095;
						}
						case 47U:
							goto IL_1095;
						case 48U:
						{
							bool flag9 = isAlly;
							if (flag9)
							{
								Serializer.Deserialize(dataPool, valueOffset, ref this._selfPoisonResists);
							}
							else
							{
								Serializer.Deserialize(dataPool, valueOffset, ref this._enemyPoisonResists);
							}
							this.UpdateDefeatMarkBar(isAlly);
							goto IL_1095;
						}
						case 49U:
						{
							bool flag10 = isAlly;
							PoisonsAndLevels poisons;
							if (flag10)
							{
								Serializer.Deserialize(dataPool, valueOffset, ref poisons);
							}
							else
							{
								Serializer.Deserialize(dataPool, valueOffset, ref poisons);
							}
							for (sbyte order = 0; order < 6; order += 1)
							{
								sbyte type = PoisonType.GetTypeBySortingOrder(order);
								bool flag11 = *(ref poisons.Values.FixedElementField + (IntPtr)type * 2) > 0;
								if (flag11)
								{
									this.ShowIconTips(charId, Poison.Instance[type].Icon, (ref poisons.Values.FixedElementField + (IntPtr)type * 2).ToString());
									this.SummaryDamageNum(charId, new DefeatMarkKey(EMarkType.Poison, (int)type, 0), (int)(*(ref poisons.Values.FixedElementField + (IntPtr)type * 2)));
								}
							}
							goto IL_1095;
						}
						default:
							switch (num)
							{
							case 60U:
							{
								byte attackIndex = 0;
								Serializer.Deserialize(dataPool, valueOffset, ref attackIndex);
								CombatSubProcessorCharacter processor = isAlly ? this.Model.SelfCharacter : this.Model.EnemyCharacter;
								short? num2 = (processor != null) ? new short?(processor.PerformingSkillId) : null;
								int? performingSkillId = (num2 != null) ? new int?((int)num2.GetValueOrDefault()) : null;
								int? num3 = performingSkillId;
								int num4 = 0;
								bool flag12 = num3.GetValueOrDefault() < num4 & num3 != null;
								if (flag12)
								{
									SkeletonAnimation petSkeleton = isAlly ? this._selfPetSkeleton : this._enemyPetSkeleton;
									BossItem bossConfig;
									bool flag13 = petSkeleton.gameObject.activeSelf && (!this.TryGetBossConfig(isAlly ? (*this._selfCurrCharId) : (*this._enemyCurrCharId), out bossConfig) || bossConfig.PetAniPrefix == null);
									if (flag13)
									{
										petSkeleton.gameObject.SetActive(false);
									}
								}
								goto IL_1095;
							}
							case 61U:
							{
								byte power = 0;
								Serializer.Deserialize(dataPool, valueOffset, ref power);
								this.damageCompare.Set(power);
								goto IL_1095;
							}
							case 62U:
							{
								short moveSkillId = 0;
								Serializer.Deserialize(dataPool, valueOffset, ref moveSkillId);
								bool flag14 = isAlly;
								if (flag14)
								{
									this._selfAffectingMoveSkillId = moveSkillId;
								}
								else
								{
									this._enemyAffectingMoveSkillId = moveSkillId;
								}
								bool flag15 = moveSkillId >= 0;
								if (flag15)
								{
									CombatSkillItem skillConfig = CombatSkill.Instance[moveSkillId];
									bool flag16 = skillConfig.EquipType == 2;
									if (flag16)
									{
										AudioManager.Instance.PlaySound("battle_MovementStart", false, false);
									}
								}
								CombatAffectingMoveSkill moveSkillRefers = (isAlly ? this.selfInfoChar : this.enemyInfoChar).affectingMoveSkill;
								TooltipInvoker mouseTipMoveSkill = moveSkillRefers.GetComponent<TooltipInvoker>();
								CImage sectType = moveSkillRefers.sectType;
								CImage greyType = moveSkillRefers.greyType;
								greyType.gameObject.SetActive(moveSkillId < 0);
								sectType.gameObject.SetActive(moveSkillId >= 0);
								moveSkillRefers.GetComponent<CEmptyGraphic>().enabled = (moveSkillId >= 0);
								bool flag17 = moveSkillId >= 0;
								if (flag17)
								{
									moveSkillRefers.UserInt = charId;
									CombatUtils.SetCombatSkillTips(mouseTipMoveSkill, charId, moveSkillId);
								}
								this.UpdateMobilityBar(isAlly);
								this.UpdateMobilityTips(isAlly);
								bool flag18 = isAlly;
								if (flag18)
								{
									this.UpdateSkillBreaker(this._selfAffectingMoveSkillId, this.moveSkillBreaker.gameObject);
									this.combatSkillWheel.UpdateAffectingSkills(this._selfAffectingMoveSkillId, this._selfAffectingDefendSkillId);
									this.combatWheel.UpdateAffectingSkills(this._selfAffectingMoveSkillId, this._selfAffectingDefendSkillId);
								}
								goto IL_1095;
							}
							case 63U:
							{
								short defendSkillId = 0;
								Serializer.Deserialize(dataPool, valueOffset, ref defendSkillId);
								bool flag19 = isAlly;
								if (flag19)
								{
									this._selfAffectingDefendSkillId = defendSkillId;
								}
								else
								{
									this._enemyAffectingDefendSkillId = defendSkillId;
								}
								CombatAffectingDefendSkill defendSkillRefers = (isAlly ? this.selfInfoChar : this.enemyInfoChar).affectingDefendSkill;
								defendSkillRefers.gameObject.SetActive(defendSkillId >= 0);
								bool flag20 = defendSkillId >= 0;
								if (flag20)
								{
									this.UpdateAffectingDefendSkill(defendSkillRefers, charId, isAlly, defendSkillId);
								}
								else
								{
									this.DestroyDefendSkillParticleAndSound(charId);
								}
								bool flag21 = isAlly;
								if (flag21)
								{
									this.UpdateSkillBreaker(this._selfAffectingDefendSkillId, this.defendSkillBreakerExtra.gameObject);
									this.combatSkillWheel.UpdateAffectingSkills(this._selfAffectingMoveSkillId, this._selfAffectingDefendSkillId);
									this.combatWheel.UpdateAffectingSkills(this._selfAffectingMoveSkillId, this._selfAffectingDefendSkillId);
								}
								goto IL_1095;
							}
							case 64U:
							{
								byte defendSkillTimePercent = 0;
								Serializer.Deserialize(dataPool, valueOffset, ref defendSkillTimePercent);
								CombatAffectingDefendSkill defendSkillRefers2 = (isAlly ? this.selfInfoChar : this.enemyInfoChar).affectingDefendSkill;
								defendSkillRefers2.progressBar.sizeDelta = new Vector2(52f + (float)(192 * defendSkillTimePercent) / 100f, 36f);
								goto IL_1095;
							}
							case 65U:
							{
								short wugCount = 0;
								Serializer.Deserialize(dataPool, valueOffset, ref wugCount);
								RectTransform wugTips = (isAlly ? this.selfInfoTop : this.enemyInfoTop).wug;
								wugTips.gameObject.SetActive(wugCount > 0);
								bool flag22 = wugCount > 0;
								if (flag22)
								{
									wugTips.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = wugCount.ToString();
								}
								CombatUtils.UpdateIconHolderVisible(wugTips.parent);
								goto IL_1095;
							}
							case 66U:
							case 67U:
							case 68U:
							case 69U:
							case 70U:
							case 72U:
							case 73U:
							case 74U:
							case 75U:
							case 79U:
								goto IL_1095;
							case 71U:
							{
								bool flag23 = !isAlly;
								if (flag23)
								{
									goto IL_1095;
								}
								bool canSurrender = false;
								Serializer.Deserialize(dataPool, valueOffset, ref canSurrender);
								canSurrender = (canSurrender && this.Model.CanOperateSelfCharacter);
								CButton surrenderBtn = this._selfOtherActionHolder.surrender;
								surrenderBtn.interactable = canSurrender;
								goto IL_1095;
							}
							case 76U:
							case 77U:
							case 78U:
							{
								CombatStateCollection stateCollection = null;
								Serializer.Deserialize(dataPool, valueOffset, ref stateCollection);
								string refersName = (uid.SubId1 == 76U) ? "Buff" : ((uid.SubId1 == 77U) ? "Debuff" : "Special");
								CombatInfoTop infoTop = isAlly ? this.selfInfoTop : this.enemyInfoTop;
								TooltipInvoker buffTips = (uid.SubId1 == 76U) ? infoTop.buff : ((uid.SubId1 == 77U) ? infoTop.debuff : infoTop.special);
								buffTips.gameObject.SetActive(stateCollection.StateDict.Count > 0);
								bool flag24 = stateCollection.StateDict.Count > 0;
								if (flag24)
								{
									buffTips.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = stateCollection.StateDict.Count.ToString();
									buffTips.PresetParam[0] = LocalStringManager.Get("LK_BuffGroup_" + refersName);
									buffTips.PresetParam[1] = this.GetCombatStateDesc(stateCollection.StateDict);
								}
								CombatUtils.UpdateIconHolderVisible(buffTips.transform.parent);
								goto IL_1095;
							}
							case 80U:
							{
								short xiangshuEffectId = 0;
								Serializer.Deserialize(dataPool, valueOffset, ref xiangshuEffectId);
								TooltipInvoker xiangshuEffectTip = (isAlly ? this.selfInfoTop : this.enemyInfoTop).xiangShuEffect;
								xiangshuEffectTip.gameObject.SetActive(xiangshuEffectId >= 0);
								bool flag25 = xiangshuEffectId >= 0;
								if (flag25)
								{
									SpecialEffectItem configData = SpecialEffect.Instance[xiangshuEffectId];
									xiangshuEffectTip.PresetParam[0] = configData.Name;
									xiangshuEffectTip.PresetParam[1] = CommonUtils.GetSpecialEffectDesc((int)xiangshuEffectId);
								}
								CombatUtils.UpdateIconHolderVisible((isAlly ? this.selfInfoTop : this.enemyInfoTop).skillEffectHolder);
								goto IL_1095;
							}
							default:
								goto IL_1095;
							}
							break;
						}
					}
				}
				else if (num <= 122U)
				{
					switch (num)
					{
					case 87U:
					{
						string aniName = null;
						Serializer.Deserialize(dataPool, valueOffset, ref aniName);
						SkeletonAnimation skillPetSkeleton = isAlly ? this._selfPetSkeleton : this._enemyPetSkeleton;
						BossItem bossConfig2;
						this.TryGetBossConfig(isAlly ? (*this._selfCurrCharId) : (*this._enemyCurrCharId), out bossConfig2);
						skillPetSkeleton.gameObject.SetActive(!aniName.IsNullOrEmpty() || (bossConfig2 != null && bossConfig2.PetAniPrefix != null));
						bool flag26 = !aniName.IsNullOrEmpty();
						if (flag26)
						{
							skillPetSkeleton.AnimationState.SetAnimation(0, aniName, false);
						}
						goto IL_1095;
					}
					case 88U:
					{
						string particleName = null;
						Serializer.Deserialize(dataPool, valueOffset, ref particleName);
						bool flag27 = !particleName.IsNullOrEmpty();
						if (flag27)
						{
							this.combatParticle.PlayVfx(isAlly, isAlly ? this._selfPetSkeleton : this._enemyPetSkeleton, particleName);
						}
						goto IL_1095;
					}
					case 89U:
						goto IL_1095;
					case 90U:
					{
						bool attackMissed = false;
						Serializer.Deserialize(dataPool, valueOffset, ref attackMissed);
						goto IL_1095;
					}
					default:
						switch (num)
						{
						case 113U:
						{
							bool flag28 = isAlly;
							if (flag28)
							{
								Serializer.Deserialize(dataPool, valueOffset, ref this._selfTargetDistance);
							}
							else
							{
								Serializer.Deserialize(dataPool, valueOffset, ref this._enemyTargetDistance);
							}
							this.UpdateTargetDistanceBar();
							goto IL_1095;
						}
						case 114U:
						{
							bool flag29 = isAlly;
							if (flag29)
							{
								Serializer.Deserialize(dataPool, valueOffset, ref this._selfOldInjuryAutoHealCollection);
							}
							else
							{
								Serializer.Deserialize(dataPool, valueOffset, ref this._enemyOldInjuryAutoHealCollection);
							}
							this.UpdateInjuryAutoHealProgress(isAlly);
							goto IL_1095;
						}
						case 115U:
						{
							bool flag30 = isAlly;
							if (flag30)
							{
								Serializer.Deserialize(dataPool, valueOffset, ref this._selfMixPoisonAffectedCount);
							}
							else
							{
								Serializer.Deserialize(dataPool, valueOffset, ref this._enemyMixPoisonAffectedCount);
							}
							goto IL_1095;
						}
						case 116U:
						case 117U:
						case 118U:
						case 119U:
							goto IL_1095;
						case 120U:
						{
							bool flag31 = isAlly;
							if (flag31)
							{
								this._oldSelfUnlockPrepareValue.Clear();
								this._oldSelfUnlockPrepareValue.AddRange(this._selfUnlockPrepareValue);
							}
							else
							{
								this._oldEnemyUnlockPrepareValue.Clear();
								this._oldEnemyUnlockPrepareValue.AddRange(this._enemyUnlockPrepareValue);
							}
							bool flag32 = isAlly;
							if (flag32)
							{
								Serializer.Deserialize(dataPool, valueOffset, ref this._selfUnlockPrepareValue);
							}
							else
							{
								Serializer.Deserialize(dataPool, valueOffset, ref this._enemyUnlockPrepareValue);
							}
							this.UpdateWeaponUnlockState(isAlly, true, false);
							goto IL_1095;
						}
						case 121U:
						{
							bool flag33 = !isAlly;
							if (flag33)
							{
								return;
							}
							Serializer.Deserialize(dataPool, valueOffset, ref this._selfRawCreateEffects);
							bool flag34 = this._selfRawCreateEffects.Count == 0 && this.rawCreatePage.gameObject.activeSelf;
							if (flag34)
							{
								this.rawCreatePage.gameObject.SetActive(false);
								this.SetTimeScale(this.combatTimeScaleToggle.DisplayTimeScale);
								this.SetRaycastBlocked(false);
								UIManager.Instance.SetEscHandler(null);
								return;
							}
							bool flag35 = this._selfRawCreateEffects.Count > 0;
							if (flag35)
							{
								GlobalDomainMethod.Call.InvokeGuidingTrigger(321);
								string characterName = CombatUtils.GetNameString(this._charDisplayDataDict[*this._selfCurrCharId], true);
								this.rawCreatePage.Refresh(this._selfRawCreateEffects, *this._selfCurrCharId, characterName, this._selfRawCreateData);
								bool flag36 = !this.rawCreatePage.gameObject.activeSelf;
								if (flag36)
								{
									this.rawCreatePage.gameObject.SetActive(true);
									this.SetTimeScale(0f);
									this.SetRaycastBlocked(true);
									RectTransform pageRect = this.rawCreatePage.GetComponent<RectTransform>();
									CanvasGroup pageCg = pageRect.gameObject.GetOrAddComponent<CanvasGroup>();
									Vector2 originAnchorPos = pageRect.anchoredPosition;
									Vector2 startAnchorPos = originAnchorPos + Vector2.up * 1500f;
									pageRect.anchoredPosition = startAnchorPos;
									pageCg.alpha = 0f;
									DG.Tweening.Sequence seq = DOTween.Sequence();
									seq.Append(pageRect.DOAnchorPos(originAnchorPos, 0.5f, false));
									seq.Insert(0f, pageCg.DOFade(1f, 1.6f));
									seq.SetUpdate(UpdateType.Late, true);
									seq.OnComplete(delegate
									{
										pageRect.anchoredPosition = originAnchorPos;
										pageCg.alpha = 1f;
										UIManager.Instance.SetEscHandler(delegate
										{
										});
									});
									seq.Restart(true, -1f);
								}
							}
							goto IL_1095;
						}
						case 122U:
						{
							RawCreateCollection rawCreateCollection = null;
							Serializer.Deserialize(dataPool, valueOffset, ref rawCreateCollection);
							bool flag37 = isAlly;
							if (flag37)
							{
								this._selfRawCreateData = rawCreateCollection;
							}
							GameObject refers = isAlly ? this.selfInfoTop.unlockItem : this.enemyInfoTop.unlockItem;
							refers.GetComponent<CombatCount>().count.text = rawCreateCollection.Effects.Count.ToString();
							TooltipInvoker mouseTipRawCreate = refers.GetComponent<TooltipInvoker>();
							TooltipInvoker tooltipInvoker = mouseTipRawCreate;
							if (tooltipInvoker.RuntimeParam == null)
							{
								tooltipInvoker.RuntimeParam = new ArgumentBox();
							}
							mouseTipRawCreate.RuntimeParam.Clear();
							mouseTipRawCreate.RuntimeParam.SetObject("rawCreateCollection", rawCreateCollection);
							mouseTipRawCreate.HideTips();
							mouseTipRawCreate.Refresh(false, -1);
							refers.gameObject.SetActive(rawCreateCollection.Effects.Count > 0);
							goto IL_1095;
						}
						default:
							goto IL_1095;
						}
						break;
					}
				}
				else
				{
					if (num == 129U)
					{
						List<CountdownData> scarMarkTime = null;
						Serializer.Deserialize(dataPool, valueOffset, ref scarMarkTime);
						this.UpdateScarMarkTime(isAlly, scarMarkTime);
						goto IL_1095;
					}
					switch (num)
					{
					case 135U:
					case 143U:
					case 144U:
						goto IL_212;
					case 136U:
					case 138U:
					case 139U:
					case 140U:
						goto IL_1095;
					case 137U:
						break;
					case 141U:
					{
						int combatStateTotalBuffPower = 0;
						Serializer.Deserialize(dataPool, valueOffset, ref combatStateTotalBuffPower);
						damageValueHolder.SetState(combatStateTotalBuffPower);
						goto IL_1095;
					}
					case 142U:
					{
						HeavyOrBreakInjuryData heavyOrBreakInjuryData = default(HeavyOrBreakInjuryData);
						Serializer.Deserialize(dataPool, valueOffset, ref heavyOrBreakInjuryData);
						this.UpdateHeavyOrBreakInjuryData(isAlly, heavyOrBreakInjuryData);
						goto IL_1095;
					}
					case 145U:
					{
						bool flag38 = isAlly;
						if (flag38)
						{
							Serializer.Deserialize(dataPool, valueOffset, ref this._selfCanUnlockAttack);
						}
						else
						{
							Serializer.Deserialize(dataPool, valueOffset, ref this._enemyCanUnlockAttack);
						}
						this.UpdateWeaponUnlockState(isAlly, false, true);
						goto IL_1095;
					}
					default:
						goto IL_1095;
					}
				}
				this.HandlerDataCombatCharPosition(uid, valueOffset, dataPool);
				goto IL_1095;
				IL_212:
				this.HandlerDataCombatCharMobility(uid, valueOffset, dataPool);
				IL_1095:;
			}
			uint subId2 = uid.SubId1;
			uint num5 = subId2;
			if (num5 <= 44U)
			{
				if (num5 != 8U)
				{
					if (num5 != 10U)
					{
						switch (num5)
						{
						case 37U:
						{
							IntPair[] outerDamageToShow = null;
							Serializer.Deserialize(dataPool, valueOffset, ref outerDamageToShow);
							for (int i = 0; i < outerDamageToShow.Length; i++)
							{
								bool flag39 = outerDamageToShow[i].First >= 0;
								if (flag39)
								{
									this.ShowDamageNumTips(charId, outerDamageToShow[i].First, outerDamageToShow[i].Second > 100, new DefeatMarkKey(EMarkType.Outer, i, 0));
								}
							}
							break;
						}
						case 38U:
						{
							IntPair[] innerDamageToShow = null;
							Serializer.Deserialize(dataPool, valueOffset, ref innerDamageToShow);
							for (int j = 0; j < innerDamageToShow.Length; j++)
							{
								bool flag40 = innerDamageToShow[j].First >= 0;
								if (flag40)
								{
									this.ShowDamageNumTips(charId, innerDamageToShow[j].First, innerDamageToShow[j].Second > 100, new DefeatMarkKey(EMarkType.Inner, j, 0));
								}
							}
							break;
						}
						case 39U:
						{
							int mindDamageToShow = 0;
							Serializer.Deserialize(dataPool, valueOffset, ref mindDamageToShow);
							bool flag41 = mindDamageToShow >= 0;
							if (flag41)
							{
								this.ShowDamageNumTips(charId, mindDamageToShow, false, EMarkType.Mind);
							}
							break;
						}
						case 40U:
						{
							int fatalDamageToShow = 0;
							Serializer.Deserialize(dataPool, valueOffset, ref fatalDamageToShow);
							bool flag42 = fatalDamageToShow >= 0;
							if (flag42)
							{
								this.ShowDamageNumTips(charId, fatalDamageToShow, false, EMarkType.Fatal);
							}
							break;
						}
						case 41U:
						{
							bool flag43 = !this._flawCountDict.ContainsKey(charId);
							if (flag43)
							{
								this._flawCountDict.Add(charId, new byte[7]);
							}
							byte[] flawCount = this._flawCountDict[charId];
							List<byte> lastFlawCount = EasyPool.Get<List<byte>>();
							lastFlawCount.Clear();
							lastFlawCount.AddRange(flawCount);
							Serializer.Deserialize(dataPool, valueOffset, ref flawCount);
							this.UpdateFlawCount(charId, lastFlawCount);
							EasyPool.Free<List<byte>>(lastFlawCount);
							break;
						}
						case 42U:
						{
							bool flag44 = !this._flawTimeDict.ContainsKey(charId);
							if (flag44)
							{
								this._flawTimeDict.Add(charId, new FlawOrAcupointCollection());
							}
							FlawOrAcupointCollection flawTime = this._flawTimeDict[charId];
							Serializer.Deserialize(dataPool, valueOffset, ref flawTime);
							bool flag45 = isCurrChar;
							if (flag45)
							{
								this.UpdateFlawTime(charId, isAlly);
							}
							break;
						}
						case 43U:
						{
							bool flag46 = !this._acupointCountDict.ContainsKey(charId);
							if (flag46)
							{
								this._acupointCountDict.Add(charId, new byte[7]);
							}
							byte[] acupointCount = this._acupointCountDict[charId];
							List<byte> lastAcupointCount = EasyPool.Get<List<byte>>();
							lastAcupointCount.Clear();
							lastAcupointCount.AddRange(acupointCount);
							Serializer.Deserialize(dataPool, valueOffset, ref acupointCount);
							this.UpdateAcupointCount(charId, lastAcupointCount);
							EasyPool.Free<List<byte>>(lastAcupointCount);
							break;
						}
						case 44U:
						{
							bool flag47 = !this._acupointTimeDict.ContainsKey(charId);
							if (flag47)
							{
								this._acupointTimeDict.Add(charId, new FlawOrAcupointCollection());
							}
							FlawOrAcupointCollection acupointTime = this._acupointTimeDict[charId];
							Serializer.Deserialize(dataPool, valueOffset, ref acupointTime);
							bool flag48 = isCurrChar;
							if (flag48)
							{
								this.UpdateAcupointTime(charId, isAlly);
							}
							break;
						}
						}
					}
					else
					{
						int displayPos = 0;
						Serializer.Deserialize(dataPool, valueOffset, ref displayPos);
						this.SetDisplayPosition(charId, isAlly, isCurrChar, displayPos);
					}
				}
				else
				{
					ShowAvoidData avoidData = default(ShowAvoidData);
					Serializer.Deserialize(dataPool, valueOffset, ref avoidData);
					bool flag49 = avoidData.HitType >= 0;
					if (flag49)
					{
						this.ShowTextTips(charId, LocalStringManager.Get(string.Format("LK_AvoidType_{0}", avoidData.HitType)));
					}
				}
			}
			else
			{
				switch (num5)
				{
				case 82U:
				{
					ShowSpecialEffectCollection effectList = null;
					Serializer.Deserialize(dataPool, valueOffset, ref effectList);
					foreach (ShowSpecialEffectDisplayData effect in effectList.ShowEffectList)
					{
						this.ShowSpecialEffectTip(isAlly, effect);
					}
					bool flag50;
					if (isAlly)
					{
						CombatSubProcessorCharacter selfCharacter = this.Model.SelfCharacter;
						short? num2 = (selfCharacter != null) ? new short?(selfCharacter.PreparingSkillId) : null;
						int? num3 = (num2 != null) ? new int?((int)num2.GetValueOrDefault()) : null;
						int num4 = 0;
						flag50 = (num3.GetValueOrDefault() >= num4 & num3 != null);
					}
					else
					{
						flag50 = false;
					}
					bool flag51 = flag50;
					if (flag51)
					{
						this.Model.DoRequestGetAllCostNeiliEffectData();
					}
					break;
				}
				case 83U:
				{
					string loopAni = null;
					Serializer.Deserialize(dataPool, valueOffset, ref loopAni);
					this.SetLoopAni(charId, loopAni);
					break;
				}
				case 84U:
				case 85U:
				case 86U:
				case 87U:
				case 88U:
				case 90U:
					break;
				case 89U:
				{
					float aniTimeScale = 0f;
					Serializer.Deserialize(dataPool, valueOffset, ref aniTimeScale);
					SkeletonAnimation skeleton = this.GetSkeleton(charId);
					bool flag52 = skeleton.timeScale > 0f;
					if (flag52)
					{
						skeleton.timeScale = aniTimeScale;
					}
					else
					{
						this._skeletonTimeScaleBeforePause[skeleton] = aniTimeScale;
					}
					break;
				}
				case 91U:
				case 92U:
				case 93U:
				case 94U:
				case 95U:
				case 96U:
				case 97U:
				case 98U:
				{
					string audioName = null;
					Serializer.Deserialize(dataPool, valueOffset, ref audioName);
					this.SetSoundOnce(audioName, 100);
					break;
				}
				default:
					if (num5 != 119U)
					{
						if (num5 == 140U)
						{
							bool flag53 = isCurrChar;
							if (flag53)
							{
								bool flag54 = isAlly;
								GameObject obj;
								int count;
								if (flag54)
								{
									Serializer.Deserialize(dataPool, valueOffset, ref this._allySilenceData);
									obj = this.selfInfoTop.banned;
									count = ((this._allySilenceData == null) ? 0 : (this._allySilenceData.CombatSkill.Count + this._allySilenceData.WeaponKeys.Count));
								}
								else
								{
									Serializer.Deserialize(dataPool, valueOffset, ref this._enemySilenceData);
									obj = this.enemyInfoTop.banned;
									count = ((this._enemySilenceData == null) ? 0 : (this._enemySilenceData.CombatSkill.Count + this._enemySilenceData.WeaponKeys.Count));
								}
								obj.GetComponent<CombatCount>().count.text = count.ToString();
								obj.SetActive(count > 0);
								bool flag55 = count > 0;
								if (flag55)
								{
									GlobalDomainMethod.Call.InvokeGuidingTrigger(174);
								}
								TooltipInvoker mouseTip = obj.GetComponent<TooltipInvoker>();
								TooltipInvoker tooltipInvoker = mouseTip;
								if (tooltipInvoker.RuntimeParam == null)
								{
									tooltipInvoker.RuntimeParam = new ArgumentBox();
								}
								mouseTip.RuntimeParam.Clear();
								mouseTip.RuntimeParam.Set("CharId", charId);
								mouseTip.RuntimeParam.Set("IsAlly", isAlly);
								bool flag56 = isAlly;
								if (flag56)
								{
									mouseTip.RuntimeParam.SetObject("AllySilenceData", this._allySilenceData);
								}
								else
								{
									mouseTip.RuntimeParam.SetObject("EnemySilenceData", this._enemySilenceData);
								}
								CombatUtils.UpdateIconHolderVisible(obj.transform.parent);
							}
						}
					}
					else
					{
						List<TeammateCommandDisplayData> showCommands = null;
						Serializer.Deserialize(dataPool, valueOffset, ref showCommands);
						bool flag57 = showCommands == null;
						if (!flag57)
						{
							foreach (TeammateCommandDisplayData data in showCommands)
							{
								this.ShowTeammateCmdBubble(data);
							}
						}
					}
					break;
				}
			}
		}

		// Token: 0x06008CF2 RID: 36082 RVA: 0x00416BB0 File Offset: 0x00414DB0
		private void HandlerDataCombatCharPosition(DataUid uid, int valueOffset, RawDataPool dataPool)
		{
			int charId = (int)uid.SubId0;
			bool isAlly = this.Model.CharIsAlly(charId);
			uint subId = uid.SubId1;
			uint num = subId;
			if (num != 9U)
			{
				if (num == 137U)
				{
					bool flag = isAlly;
					if (flag)
					{
						Serializer.Deserialize(dataPool, valueOffset, ref this._selfChangeDistanceDuration);
					}
					else
					{
						Serializer.Deserialize(dataPool, valueOffset, ref this._enemyChangeDistanceDuration);
					}
				}
			}
			else
			{
				int pos = 0;
				Serializer.Deserialize(dataPool, valueOffset, ref pos);
				this.SetPosition(isAlly, pos);
			}
		}

		// Token: 0x06008CF3 RID: 36083 RVA: 0x00416C30 File Offset: 0x00414E30
		private void HandlerDataCombatCharMobility(DataUid uid, int valueOffset, RawDataPool dataPool)
		{
			int charId = (int)uid.SubId0;
			bool isAlly = this.Model.CharIsAlly(charId);
			uint subId = uid.SubId1;
			uint num = subId;
			if (num <= 14U)
			{
				if (num != 11U)
				{
					if (num == 14U)
					{
						short mobilityLockEffectCount = 0;
						Serializer.Deserialize(dataPool, valueOffset, ref mobilityLockEffectCount);
						bool flag = isAlly;
						if (flag)
						{
							this._selfMobilityLockEffectCount = mobilityLockEffectCount;
						}
						else
						{
							this._enemyMobilityLockEffectCount = mobilityLockEffectCount;
						}
						this.UpdateMobilityLock(isAlly);
					}
				}
				else
				{
					int mobility = 0;
					Serializer.Deserialize(dataPool, valueOffset, ref mobility);
					bool flag2 = isAlly;
					if (flag2)
					{
						this._selfMobility = mobility;
					}
					else
					{
						this._enemyMobility = mobility;
					}
					CImage mobilityBar = (isAlly ? this.selfInfoChar : this.enemyInfoChar).mobilityBar;
					bool flag3 = !isAlly || !this.Model.ShowSkillCostPreview;
					if (flag3)
					{
						mobilityBar.fillAmount = (float)mobility / (float)MoveSpecialConstants.MaxMobility;
					}
					else
					{
						this.UpdateSkillCostMobilityPreview();
					}
					this.UpdateMobilityTips(isAlly);
				}
			}
			else if (num != 135U)
			{
				if (num != 143U)
				{
					if (num == 144U)
					{
						int recoverSpeed = 0;
						Serializer.Deserialize(dataPool, valueOffset, ref recoverSpeed);
						bool flag4 = isAlly;
						if (flag4)
						{
							this._selfMobilityRecoverSpeed = recoverSpeed;
						}
						else
						{
							this._enemyMobilityRecoverSpeed = recoverSpeed;
						}
						this.UpdateMobilityTips(isAlly);
					}
				}
				else
				{
					short moveCd = 0;
					Serializer.Deserialize(dataPool, valueOffset, ref moveCd);
					bool flag5 = isAlly;
					if (flag5)
					{
						this._selfMoveCd = moveCd;
					}
					else
					{
						this._enemyMoveCd = moveCd;
					}
					this.UpdateMobilityTips(isAlly);
				}
			}
			else
			{
				byte mobilityLevel = 0;
				Serializer.Deserialize(dataPool, valueOffset, ref mobilityLevel);
				bool flag6 = isAlly;
				if (flag6)
				{
					this._selfMobilityLevel = mobilityLevel;
				}
				else
				{
					this._enemyMobilityLevel = mobilityLevel;
				}
				this.UpdateMobilityBar(isAlly);
				this.UpdateMobilityTips(isAlly);
			}
		}

		// Token: 0x06008CF4 RID: 36084 RVA: 0x00416DEC File Offset: 0x00414FEC
		private void InitHotKey()
		{
			bool flag = this._hotKey2Action != null;
			if (!flag)
			{
				this._hotKey2Action = new Dictionary<HotKeyCommand, Action>
				{
					{
						CombatCommandKit.NormalAttack,
						new Action(this.OnCheckNormalAttack)
					},
					{
						CombatCommandKit.CancelReserveNormalAttack,
						new Action(this.CancelReserveNormalAttack)
					},
					{
						CombatCommandKit.ChangeToWeapon0,
						delegate()
						{
							this.DoRequestChangeWeapon(0);
						}
					},
					{
						CombatCommandKit.ChangeToWeapon1,
						delegate()
						{
							this.DoRequestChangeWeapon(1);
						}
					},
					{
						CombatCommandKit.ChangeToWeapon2,
						delegate()
						{
							this.DoRequestChangeWeapon(2);
						}
					},
					{
						CombatCommandKit.ChangeToWeapon3,
						delegate()
						{
							this.DoRequestChangeWeapon(3);
						}
					},
					{
						CombatCommandKit.ChangeToWeapon4,
						delegate()
						{
							this.DoRequestChangeWeapon(4);
						}
					},
					{
						CombatCommandKit.ChangeToWeapon5,
						delegate()
						{
							this.DoRequestChangeWeapon(5);
						}
					},
					{
						CombatCommandKit.ChangeToWeapon6,
						delegate()
						{
							this.DoRequestChangeWeapon(6);
						}
					},
					{
						CombatCommandKit.UnlockWeapon0,
						delegate()
						{
							this.DoRequestUnlockWeapon(0);
						}
					},
					{
						CombatCommandKit.UnlockWeapon1,
						delegate()
						{
							this.DoRequestUnlockWeapon(1);
						}
					},
					{
						CombatCommandKit.UnlockWeapon2,
						delegate()
						{
							this.DoRequestUnlockWeapon(2);
						}
					},
					{
						CombatCommandKit.UseSkill0,
						delegate()
						{
							this.OnCheckUseSkill(0);
						}
					},
					{
						CombatCommandKit.UseSkill1,
						delegate()
						{
							this.OnCheckUseSkill(1);
						}
					},
					{
						CombatCommandKit.UseSkill2,
						delegate()
						{
							this.OnCheckUseSkill(2);
						}
					},
					{
						CombatCommandKit.UseSkill3,
						delegate()
						{
							this.OnCheckUseSkill(3);
						}
					},
					{
						CombatCommandKit.UseSkill4,
						delegate()
						{
							this.OnCheckUseSkill(4);
						}
					},
					{
						CombatCommandKit.UseSkill5,
						delegate()
						{
							this.OnCheckUseSkill(5);
						}
					},
					{
						CombatCommandKit.UseSkill6,
						delegate()
						{
							this.OnCheckUseSkill(6);
						}
					},
					{
						CombatCommandKit.UseSkill7,
						delegate()
						{
							this.OnCheckUseSkill(7);
						}
					},
					{
						CombatCommandKit.UseSkill8,
						delegate()
						{
							this.OnCheckUseSkill(8);
						}
					},
					{
						CombatCommandKit.UseSkill9,
						delegate()
						{
							this.OnCheckUseSkill(9);
						}
					},
					{
						CombatCommandKit.UseSkill10,
						delegate()
						{
							this.OnCheckUseSkill(10);
						}
					},
					{
						CombatCommandKit.UseSkill11,
						delegate()
						{
							this.OnCheckUseSkill(11);
						}
					},
					{
						CombatCommandKit.UseSkill12,
						delegate()
						{
							this.OnCheckUseSkill(12);
						}
					},
					{
						CombatCommandKit.UseSkill13,
						delegate()
						{
							this.OnCheckUseSkill(13);
						}
					},
					{
						CombatCommandKit.UseSkill14,
						delegate()
						{
							this.OnCheckUseSkill(14);
						}
					},
					{
						CombatCommandKit.UseSkill15,
						delegate()
						{
							this.OnCheckUseSkill(15);
						}
					},
					{
						CombatCommandKit.UseSkill16,
						delegate()
						{
							this.OnCheckUseSkill(16);
						}
					},
					{
						CombatCommandKit.UseSkill17,
						delegate()
						{
							this.OnCheckUseSkill(17);
						}
					},
					{
						CombatCommandKit.UseSkill18,
						delegate()
						{
							this.OnCheckUseSkill(18);
						}
					},
					{
						CombatCommandKit.UseSkill19,
						delegate()
						{
							this.OnCheckUseSkill(19);
						}
					},
					{
						CombatCommandKit.UseSkill20,
						delegate()
						{
							this.OnCheckUseSkill(20);
						}
					},
					{
						CombatCommandKit.UseSkill21,
						delegate()
						{
							this.OnCheckUseSkill(21);
						}
					},
					{
						CombatCommandKit.UseSkill22,
						delegate()
						{
							this.OnCheckUseSkill(22);
						}
					},
					{
						CombatCommandKit.UseSkill23,
						delegate()
						{
							this.OnCheckUseSkill(23);
						}
					},
					{
						CombatCommandKit.UseSkill24,
						delegate()
						{
							this.OnCheckUseSkill(24);
						}
					},
					{
						CombatCommandKit.UseSkill25,
						delegate()
						{
							this.OnCheckUseSkill(25);
						}
					},
					{
						CombatCommandKit.UseSkill26,
						delegate()
						{
							this.OnCheckUseSkill(26);
						}
					},
					{
						CombatBehaviorCommandKit.HealInjury,
						delegate()
						{
							this.DoRequestOtherAction(0);
						}
					},
					{
						CombatBehaviorCommandKit.HealPoison,
						delegate()
						{
							this.DoRequestOtherAction(1);
						}
					},
					{
						CombatBehaviorCommandKit.Flee,
						delegate()
						{
							this.DoRequestOtherAction(2);
						}
					},
					{
						CombatBehaviorCommandKit.AnimalAttack,
						delegate()
						{
							this.DoRequestOtherAction(3);
						}
					},
					{
						CombatBehaviorCommandKit.Surrender,
						delegate()
						{
							this.DoRequestOtherAction(4);
						}
					},
					{
						CombatCommandKit.UseTeammateCommand0,
						delegate()
						{
							this.DoCheckUseTeammateCommand(0, 0);
						}
					},
					{
						CombatCommandKit.UseTeammateCommand1,
						delegate()
						{
							this.DoCheckUseTeammateCommand(0, 1);
						}
					},
					{
						CombatCommandKit.UseTeammateCommand2,
						delegate()
						{
							this.DoCheckUseTeammateCommand(0, 2);
						}
					},
					{
						CombatCommandKit.UseTeammateCommand3,
						delegate()
						{
							this.DoCheckUseTeammateCommand(1, 0);
						}
					},
					{
						CombatCommandKit.UseTeammateCommand4,
						delegate()
						{
							this.DoCheckUseTeammateCommand(1, 1);
						}
					},
					{
						CombatCommandKit.UseTeammateCommand5,
						delegate()
						{
							this.DoCheckUseTeammateCommand(1, 2);
						}
					},
					{
						CombatCommandKit.UseTeammateCommand6,
						delegate()
						{
							this.DoCheckUseTeammateCommand(2, 0);
						}
					},
					{
						CombatCommandKit.UseTeammateCommand7,
						delegate()
						{
							this.DoCheckUseTeammateCommand(2, 1);
						}
					},
					{
						CombatCommandKit.UseTeammateCommand8,
						delegate()
						{
							this.DoCheckUseTeammateCommand(2, 2);
						}
					},
					{
						CombatBehaviorCommandKit.UseQuickItem1,
						delegate()
						{
							this.combatQuickUseItemPanel.SelectItem(0);
						}
					},
					{
						CombatBehaviorCommandKit.UseQuickItem2,
						delegate()
						{
							this.combatQuickUseItemPanel.SelectItem(1);
						}
					},
					{
						CombatBehaviorCommandKit.UseQuickItem3,
						delegate()
						{
							this.combatQuickUseItemPanel.SelectItem(2);
						}
					},
					{
						CombatBehaviorCommandKit.UseQuickItem4,
						delegate()
						{
							this.combatQuickUseItemPanel.SelectItem(3);
						}
					},
					{
						CombatBehaviorCommandKit.UseQuickItem5,
						delegate()
						{
							this.combatQuickUseItemPanel.SelectItem(4);
						}
					},
					{
						CombatBehaviorCommandKit.UseQuickItem6,
						delegate()
						{
							this.combatQuickUseItemPanel.SelectItem(5);
						}
					},
					{
						CombatBehaviorCommandKit.UseQuickItem7,
						delegate()
						{
							this.combatQuickUseItemPanel.SelectItem(6);
						}
					},
					{
						CombatBehaviorCommandKit.UseQuickItem8,
						delegate()
						{
							this.combatQuickUseItemPanel.SelectItem(7);
						}
					},
					{
						CombatBehaviorCommandKit.UseQuickItem9,
						delegate()
						{
							this.combatQuickUseItemPanel.SelectItem(8);
						}
					},
					{
						CombatCommandKit.PureMode,
						delegate()
						{
							this.SettingData.CombatPure = !this.SettingData.CombatPure;
						}
					},
					{
						CombatCommandKit.AmplifyCast,
						new Action(this.OnAmplifyCast)
					}
				};
			}
		}

		// Token: 0x06008CF5 RID: 36085 RVA: 0x00417417 File Offset: 0x00415617
		private void OnCheckNormalAttack()
		{
			this.DoRequestNormalAttackOrCancelReserve(true);
		}

		// Token: 0x06008CF6 RID: 36086 RVA: 0x00417424 File Offset: 0x00415624
		private void DoRequestNormalAttackOrCancelReserve(bool cancelOnReserve)
		{
			bool keepPauseUntilCastSkill = this._keepPauseUntilCastSkill;
			if (!keepPauseUntilCastSkill)
			{
				CombatSubProcessorCharacter processor;
				bool flag = !this.Model.ProcessorCharacters.TryGetValue(this.Model.SelfCharId, out processor);
				if (!flag)
				{
					bool flag2 = !this.Model.CanOperateSelfCharacter;
					if (!flag2)
					{
						bool flag3 = cancelOnReserve && processor.ReserveNormalAttack;
						if (flag3)
						{
							CombatDomainMethod.Call.ClearReserveNormalAttack();
						}
						else
						{
							bool normalAttackReserve = SingletonObject.getInstance<GlobalSettings>().NormalAttackReserve;
							if (normalAttackReserve)
							{
								CombatDomainMethod.Call.NormalAttack();
							}
							else
							{
								CombatDomainMethod.Call.NormalAttackImmediate();
							}
						}
						this.HideAttackTips();
					}
				}
			}
		}

		// Token: 0x06008CF7 RID: 36087 RVA: 0x004174B8 File Offset: 0x004156B8
		private void CancelReserveNormalAttack()
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(this.Model.SelfCharId, out processor);
			if (!flag)
			{
				bool reserveNormalAttack = processor.ReserveNormalAttack;
				if (reserveNormalAttack)
				{
					CombatDomainMethod.Call.ClearReserveNormalAttack();
				}
			}
		}

		// Token: 0x06008CF8 RID: 36088 RVA: 0x004174FC File Offset: 0x004156FC
		private void OnAmplifyCast()
		{
			CombatCostNeiliAllocation costAlloc = base.GetComponentInChildren<CombatCostNeiliAllocation>(true);
			bool flag = costAlloc == null || !costAlloc.gameObject.activeInHierarchy;
			if (!flag)
			{
				for (int i = 0; i < costAlloc.transform.childCount; i++)
				{
					Transform child = costAlloc.transform.GetChild(i);
					bool flag2 = !child.gameObject.activeSelf;
					if (!flag2)
					{
						CombatSpecialEffectCostNeiliAllocation effectBtn = child.GetComponent<CombatSpecialEffectCostNeiliAllocation>();
						bool flag3 = effectBtn != null && child.GetComponent<CButton>().interactable;
						if (flag3)
						{
							effectBtn.TryRequestCost();
							break;
						}
					}
				}
			}
		}

		// Token: 0x06008CF9 RID: 36089 RVA: 0x004175A4 File Offset: 0x004157A4
		private void OnCheckUseSkill(int index)
		{
			bool flag = this.Model.OrderedProactiveSkillList[this.Model.SelfCharId].Count > index;
			if (flag)
			{
				this.CastProactiveSkill(index);
			}
		}

		// Token: 0x06008CFA RID: 36090 RVA: 0x004175E4 File Offset: 0x004157E4
		private void DoCheckUseTeammateCommand(int teammateIndex, int commandIndex)
		{
			int index = teammateIndex + 1;
			bool flag = index >= this.Model.SelfTeam.Count;
			if (!flag)
			{
				int teammateId = this.Model.SelfTeam[index];
				CombatSubProcessorCharacter processor = this.Model.ProcessorCharacters[teammateId];
				List<sbyte> currCmdList = processor.CurrTeammateCommands;
				CombatSubProcessorCharacter orDefault = this.Model.ProcessorCharacters.GetOrDefault(this.Model.SelfTeam[0]);
				bool showTransferInjuryCommand = orDefault != null && orDefault.ShowTransferInjuryCommand;
				sbyte cmdType = showTransferInjuryCommand ? 13 : currCmdList[commandIndex];
				bool canUse = cmdType >= 0 && (!showTransferInjuryCommand || commandIndex == 0) && processor.GetTeammateCmdCanUse(commandIndex);
				bool flag2 = !canUse;
				if (!flag2)
				{
					int mainSelf = this.Model.SelfTeam[0];
					CombatSubProcessorCharacter mainSelfProcessor = this.Model.ProcessorCharacters[mainSelf];
					bool flag3 = mainSelfProcessor.CombatReserveData.TeammateCharId == teammateId && mainSelfProcessor.CombatReserveData.TeammateCmdIndex == commandIndex;
					if (flag3)
					{
						CombatDomainMethod.Call.ClearAllReserveAction();
					}
					else
					{
						this.Model.DoRequestUseTeammateCommand(commandIndex, teammateId);
					}
				}
			}
		}

		// Token: 0x06008CFB RID: 36091 RVA: 0x00417710 File Offset: 0x00415910
		public void WheelOnOpen()
		{
			bool flag = !this.combatTimeScaleToggle.IsPaused;
			if (flag)
			{
				CombatDomainMethod.Call.SetTimeScale(Mathf.Min(this.combatTimeScaleToggle.DisplayTimeScale, 0.5f));
			}
		}

		// Token: 0x06008CFC RID: 36092 RVA: 0x0041774C File Offset: 0x0041594C
		public void WheelOnClose()
		{
			bool flag = !this.combatTimeScaleToggle.IsPaused && this.Model.CombatStatus == 1;
			if (flag)
			{
				CombatDomainMethod.Call.SetTimeScale(this.combatTimeScaleToggle.DisplayTimeScale);
			}
		}

		// Token: 0x06008CFD RID: 36093 RVA: 0x0041778D File Offset: 0x0041598D
		public void WheelOpenChangeTrick()
		{
			this.OnClickChangeTrick();
			this.HideAttackTips();
		}

		// Token: 0x06008CFE RID: 36094 RVA: 0x0041779E File Offset: 0x0041599E
		public void WheelToggleUseItem()
		{
			this.DoRequestSelectItem();
			this.HideAttackTips();
		}

		// Token: 0x06008CFF RID: 36095 RVA: 0x004177AF File Offset: 0x004159AF
		public void WheelChangeWeapon(int index)
		{
			this.DoRequestChangeWeapon(index);
			this.HideAttackTips();
		}

		// Token: 0x06008D00 RID: 36096 RVA: 0x004177C1 File Offset: 0x004159C1
		public void WheelUnlockWeapon(int index)
		{
			this.DoRequestUnlockWeapon(index);
		}

		// Token: 0x06008D01 RID: 36097 RVA: 0x004177CC File Offset: 0x004159CC
		public void WheelRequestOtherAction(sbyte actionType)
		{
			this.DoRequestOtherAction(actionType);
			this.HideAttackTips();
		}

		// Token: 0x06008D02 RID: 36098 RVA: 0x004177E0 File Offset: 0x004159E0
		public void WheelCastSkillByTemplateId(short skillTemplateId)
		{
			List<short> skillList;
			bool flag = !this.Model.OrderedProactiveSkillList.TryGetValue(this.Model.SelfCharId, out skillList);
			if (!flag)
			{
				int index = skillList.IndexOf(skillTemplateId);
				bool flag2 = index < 0;
				if (!flag2)
				{
					this.CastProactiveSkill(index);
				}
			}
		}

		// Token: 0x06008D03 RID: 36099 RVA: 0x00417830 File Offset: 0x00415A30
		private void WheelBreakAffectingSkill(short skillId)
		{
			bool flag = skillId == this._selfAffectingMoveSkillId;
			if (flag)
			{
				this.moveSkillBreaker.DoBreak();
			}
			else
			{
				bool flag2 = skillId == this._selfAffectingDefendSkillId;
				if (flag2)
				{
					this.defendSkillBreakerExtra.DoBreak();
				}
			}
		}

		// Token: 0x06008D04 RID: 36100 RVA: 0x00417874 File Offset: 0x00415A74
		private void SkillWheelCastSkill(short templateId)
		{
			List<short> skillList;
			bool flag = !this.Model.OrderedProactiveSkillList.TryGetValue(this.Model.SelfCharId, out skillList);
			if (!flag)
			{
				int index = skillList.IndexOf(templateId);
				bool flag2 = index < 0;
				if (!flag2)
				{
					this.CastProactiveSkill(index);
				}
			}
		}

		// Token: 0x06008D05 RID: 36101 RVA: 0x004178C4 File Offset: 0x00415AC4
		private void SkillWheelBreakAffectingSkill(short skillId)
		{
			bool flag = skillId == this._selfAffectingMoveSkillId;
			if (flag)
			{
				this.moveSkillBreaker.DoBreak();
			}
			else
			{
				bool flag2 = skillId == this._selfAffectingDefendSkillId;
				if (flag2)
				{
					this.defendSkillBreakerExtra.DoBreak();
				}
			}
		}

		// Token: 0x06008D06 RID: 36102 RVA: 0x00417908 File Offset: 0x00415B08
		private void SkillWheelOnOpen()
		{
			bool flag = this.Model.CombatStatus != 1;
			if (!flag)
			{
				this._skillWheelPauseHolding = false;
				bool flag2 = !this.IsPausing;
				if (flag2)
				{
					this.SwitchPauseState(true, ViewCombat.EPauseReason.SkillWheel);
					this._skillWheelPauseHolding = (this.combatTimeScaleToggle.IsPaused && this._pauseReason == ViewCombat.EPauseReason.SkillWheel);
				}
			}
		}

		// Token: 0x06008D07 RID: 36103 RVA: 0x0041796C File Offset: 0x00415B6C
		private void SkillWheelOnClose()
		{
			bool flag = !this._skillWheelPauseHolding;
			if (!flag)
			{
				this._skillWheelPauseHolding = false;
				bool flag2 = this.Model.CombatStatus != 1;
				if (!flag2)
				{
					bool flag3 = this.combatTimeScaleToggle.IsPaused && this._pauseReason == ViewCombat.EPauseReason.SkillWheel;
					if (flag3)
					{
						this.SwitchPauseState(false, ViewCombat.EPauseReason.SkillWheel);
					}
				}
			}
		}

		// Token: 0x06008D08 RID: 36104 RVA: 0x004179D0 File Offset: 0x00415BD0
		private void SkillSortOnOpen()
		{
			bool flag = this.Model.CombatStatus != 1;
			if (!flag)
			{
				this._skillSortPauseHolding = false;
				bool flag2 = !this.IsPausing;
				if (flag2)
				{
					this.SwitchPauseState(true, ViewCombat.EPauseReason.SkillSort);
					this._skillSortPauseHolding = (this.combatTimeScaleToggle.IsPaused && this._pauseReason == ViewCombat.EPauseReason.SkillSort);
				}
			}
		}

		// Token: 0x06008D09 RID: 36105 RVA: 0x00417A34 File Offset: 0x00415C34
		private void SkillSortOnClose()
		{
			bool flag = !this._skillSortPauseHolding;
			if (!flag)
			{
				this._skillSortPauseHolding = false;
				bool flag2 = this.Model.CombatStatus != 1;
				if (!flag2)
				{
					bool flag3 = this.combatTimeScaleToggle.IsPaused && this._pauseReason == ViewCombat.EPauseReason.SkillSort;
					if (flag3)
					{
						this.SwitchPauseState(false, ViewCombat.EPauseReason.SkillSort);
					}
				}
			}
		}

		// Token: 0x06008D0A RID: 36106 RVA: 0x00417A98 File Offset: 0x00415C98
		public bool IsOtherActionButtonVisible(sbyte actionType)
		{
			CombatOtherActionHolder selfOtherActionHolder = this._selfOtherActionHolder;
			bool flag = ((selfOtherActionHolder != null) ? selfOtherActionHolder.otherActionTypeList : null) == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = actionType < 0 || (int)actionType >= this._selfOtherActionHolder.otherActionTypeList.Count;
				result = (!flag2 && this._selfOtherActionHolder.otherActionTypeList[(int)actionType].gameObject.activeSelf);
			}
			return result;
		}

		// Token: 0x06008D0B RID: 36107 RVA: 0x00417B08 File Offset: 0x00415D08
		private void DoRequestChangeTrick()
		{
			bool needShowChangeTrick = this._selfReserveData.NeedShowChangeTrick;
			if (needShowChangeTrick)
			{
				CombatDomainMethod.Call.ClearAllReserveAction();
			}
			else
			{
				CombatDomainMethod.Call.StartChangeTrick();
			}
		}

		// Token: 0x06008D0C RID: 36108 RVA: 0x00417B34 File Offset: 0x00415D34
		public void DoRequestChangeWeapon(int index)
		{
			bool flag = this._selfReserveData.NeedChangeWeaponIndex == index;
			if (flag)
			{
				CombatDomainMethod.Call.ClearAllReserveAction();
			}
			else
			{
				CombatDomainMethod.Call.ChangeWeapon(index);
			}
		}

		// Token: 0x06008D0D RID: 36109 RVA: 0x00417B64 File Offset: 0x00415D64
		public void DoRequestUnlockWeapon(int index)
		{
			bool flag = this._selfReserveData.NeedUnlockWeaponIndex == index;
			if (flag)
			{
				CombatDomainMethod.Call.ClearAllReserveAction();
			}
			else
			{
				int i = index;
				CombatWeaponPrefab weaponRefers = this._selfWeaponHolder.GetChild(i).GetComponent<CombatWeaponPrefab>();
				CombatWeaponUnlockHolderPrefab unlockHolder = weaponRefers.unlockHolder;
				CButton unlockBtn = unlockHolder.unlockBtn;
				bool flag2 = !unlockBtn.interactable;
				if (!flag2)
				{
					base.DelayCall(delegate
					{
						this._selfUnlockTriggered[i] = false;
					}, 0.2f);
					AudioManager.Instance.PlaySound("ui_combat_rawcreate", false, false);
					this._selfUnlockEffectTriggered[index] = false;
					CombatDomainMethod.Call.UnlockAttack(index);
				}
			}
		}

		// Token: 0x06008D0E RID: 36110 RVA: 0x00417C18 File Offset: 0x00415E18
		private bool DoRequestCastSkill(short skillId)
		{
			bool flag = this._selfAffectingMoveSkillId == skillId;
			if (flag)
			{
				this.moveSkillBreaker.DoBreak();
			}
			else
			{
				bool flag2 = this._selfAffectingDefendSkillId == skillId;
				if (flag2)
				{
					this.defendSkillBreakerExtra.DoBreak();
				}
				else
				{
					bool flag3 = this._selfReserveData.NeedUseSkillId == skillId;
					if (flag3)
					{
						CombatDomainMethod.Call.ClearAllReserveAction();
					}
					else
					{
						CombatDomainMethod.Call.StartPrepareSkill(skillId);
					}
				}
			}
			return true;
		}

		// Token: 0x06008D0F RID: 36111 RVA: 0x00417C84 File Offset: 0x00415E84
		private void DoRequestSelectItem()
		{
			bool flag = this._selfReserveData.NeedUseItem.IsValid();
			if (flag)
			{
				CombatDomainMethod.Call.ClearAllReserveAction();
			}
			else
			{
				this.ShowSelectItemInCombat();
			}
		}

		// Token: 0x06008D10 RID: 36112 RVA: 0x00417CB8 File Offset: 0x00415EB8
		private void DoRequestUseItem(ItemKey itemKey, sbyte useType = -1, List<sbyte> targetBodyParts = null)
		{
			bool flag = this._selfReserveData.NeedUseItem.IsValid() && this._selfReserveData.NeedUseItem.TemplateEquals(itemKey);
			if (flag)
			{
				CombatDomainMethod.Call.ClearAllReserveAction();
			}
			else
			{
				CombatDomainMethod.Call.UseItem(itemKey, useType, true, targetBodyParts);
			}
		}

		// Token: 0x06008D11 RID: 36113 RVA: 0x00417D08 File Offset: 0x00415F08
		private void DoRequestRepairItem(ItemKey toolKey, ItemKey targetKey)
		{
			bool flag = this._selfReserveData.NeedUseItem.IsValid();
			if (flag)
			{
				CombatDomainMethod.Call.ClearAllReserveAction();
			}
			else
			{
				CombatDomainMethod.Call.RepairItem(toolKey, targetKey);
			}
		}

		// Token: 0x06008D12 RID: 36114 RVA: 0x00417D40 File Offset: 0x00415F40
		private void DoRequestUseSpecialItem(sbyte itemType, short templateId)
		{
			bool flag = this._selfReserveData.NeedUseItem.IsValid() && this._selfReserveData.NeedUseItem.TemplateEquals(itemType, templateId);
			if (flag)
			{
				CombatDomainMethod.Call.ClearAllReserveAction();
			}
			else
			{
				CombatDomainMethod.Call.UseSpecialItem(itemType, templateId);
			}
		}

		// Token: 0x06008D13 RID: 36115 RVA: 0x00417D90 File Offset: 0x00415F90
		private bool DoRequestOtherAction(sbyte actionType)
		{
			bool doAction = this._selfReserveData.NeedUseOtherAction != actionType;
			bool flag = doAction;
			if (flag)
			{
				CombatDomainMethod.Call.StartPrepareOtherAction(actionType);
			}
			else
			{
				CombatDomainMethod.Call.ClearAllReserveAction();
			}
			return doAction;
		}

		// Token: 0x06008D14 RID: 36116 RVA: 0x00417DCC File Offset: 0x00415FCC
		private void InitTeam(bool isAlly)
		{
			IReadOnlyList<int> readOnlyList = isAlly ? this._selfTeam : this._enemyTeam;
			CombatInfoChar infoChar = isAlly ? this.selfInfoChar : this.enemyInfoChar;
			PositionFollower infoCharConstraint = infoChar.GetComponent<PositionFollower>();
			List<SkeletonAnimation> skeletonList = isAlly ? this.selfSkeletonList : this.enemySkeletonList;
			SkeletonAnimation mainSkeleton = skeletonList[0];
			this.InitSkeletonList(isAlly);
			bool flag = infoCharConstraint.Target != mainSkeleton.GetComponent<RectTransform>();
			if (flag)
			{
				infoCharConstraint.Target = mainSkeleton.GetComponent<RectTransform>();
			}
		}

		// Token: 0x06008D15 RID: 36117 RVA: 0x00417E50 File Offset: 0x00416050
		private void InitSkeletonList(bool isAlly)
		{
			IReadOnlyList<int> charList = isAlly ? this._selfTeam : this._enemyTeam;
			List<SkeletonAnimation> skeletonList = isAlly ? this.selfSkeletonList : this.enemySkeletonList;
			for (int i = 0; i < skeletonList.Count; i++)
			{
				SkeletonAnimation skeleton = skeletonList[i];
				skeleton.gameObject.SetActive(i < charList.Count);
				bool flag = i < charList.Count;
				if (flag)
				{
					this.InitSkeleton(skeleton, isAlly, charList[i], i == 0);
				}
			}
		}

		// Token: 0x06008D16 RID: 36118 RVA: 0x00417EDC File Offset: 0x004160DC
		private void InitSkeleton(SkeletonAnimation skeleton, bool isAlly, int charId, bool isMainCharacter)
		{
			short charTemplateId = this._charDisplayDataDict[charId].TemplateId;
			bool isBoss = ViewCombat.CharId2BossId.ContainsKey(charTemplateId);
			bool isAnimal = GameData.Domains.Combat.SharedConstValue.CharId2AnimalId.ContainsKey(charTemplateId);
			bool isActor = !isBoss && !isAnimal;
			skeleton.GetComponent<CombatSpineSkeleton>().UserInt = charId;
			SkeletonUtility skeletonUtility = skeleton.GetComponent<SkeletonUtility>();
			skeletonUtility.enabled = false;
			SkeletonAnimation petSkeleton = isAlly ? this._selfPetSkeleton : this._enemyPetSkeleton;
			bool flag = isActor;
			if (flag)
			{
				petSkeleton.gameObject.SetActive(false);
				skeleton.skeletonDataAsset = this._actorSkeleton;
				skeleton.initialSkinName = this.GetInitialSkinName(charId);
				skeleton.ClearState();
				skeleton.Initialize(true, false);
				skeleton.gameObject.SetActive(isMainCharacter);
				skeletonUtility.enabled = true;
				skeletonUtility.HandleRendererReset(skeleton);
				base.StartCoroutine(this.RegisterAniEvent(skeleton));
				this._skeletonLoaded[charId] = true;
				this.Model.RequestGetAllEquipmentItems(charId, delegate
				{
					this.UpdateSkeleton(charId, skeleton);
				});
			}
			else
			{
				BossItem boss;
				bool flag2 = this.TryGetBossConfig(charId, out boss);
				if (flag2)
				{
					petSkeleton.gameObject.SetActive(boss.PetAniPrefix != null);
					string assetPath = "RemakeResources/Combat/CombatBoss/" + boss.AssetFileName + "/" + boss.AssetFileName;
					this.LoadSpecialCharAsset(charId, assetPath, skeleton, petSkeleton);
					this.Model.RequestGetAllEquipmentItems(charId, null);
				}
				else
				{
					AnimalItem animal;
					bool flag3 = this.TryGetAnimalConfig(charId, out animal);
					if (flag3)
					{
						petSkeleton.gameObject.SetActive(false);
						string assetPath2 = "RemakeResources/Combat/CombatAnimals/" + animal.AssetFileName + "/" + animal.AssetFileName;
						this.LoadSpecialCharAsset(charId, assetPath2, skeleton, null);
						this.Model.RequestGetAllEquipmentItems(charId, null);
					}
					else
					{
						skeleton.gameObject.SetActive(false);
						PredefinedLog.Show(8, string.Format("unexpected char {0}", charId));
					}
				}
			}
		}

		// Token: 0x06008D17 RID: 36119 RVA: 0x00418168 File Offset: 0x00416368
		private IEnumerator RegisterAniEvent(SkeletonAnimation skeleton)
		{
			while (skeleton.AnimationState == null)
			{
				yield return null;
			}
			skeleton.AnimationState.Event += this.HandleAnimationEvent;
			yield break;
		}

		// Token: 0x06008D18 RID: 36120 RVA: 0x00418180 File Offset: 0x00416380
		private void LoadSpecialCharAsset(int charId, string assetPath, SkeletonAnimation skeleton, SkeletonAnimation petSkeleton = null)
		{
			ResLoader.Load<SpecialCombatCharAsset>(assetPath, delegate(SpecialCombatCharAsset charAsset)
			{
				skeleton.skeletonDataAsset = charAsset.SkeletonData;
				skeleton.initialSkinName = this.GetInitialSkinName(charId);
				skeleton.ClearState();
				skeleton.Initialize(true, false);
				this.StartCoroutine(this.RegisterAniEvent(skeleton));
				bool flag = petSkeleton != null && charAsset.PetSkeletonData != null;
				if (flag)
				{
					petSkeleton.skeletonDataAsset = charAsset.PetSkeletonData;
					petSkeleton.Initialize(true, false);
				}
				RectTransform particleHolder = this.particlePrefabHolder;
				foreach (KeyValuePair<string, GameObject> entry in charAsset.ParticleDict)
				{
					GameObject poolObj = Object.Instantiate<GameObject>(entry.Value, particleHolder);
					this._skillAndSpecialParticleNameList.Add(entry.Key);
					CombatPoolAdaptor.SetSrcObject(entry.Key, poolObj, 0);
					poolObj.SetActive(false);
				}
				foreach (KeyValuePair<string, AudioClip> entry2 in charAsset.AudioDict)
				{
					bool flag2 = !this._skillAndSpecialSoundDict.ContainsKey(entry2.Key);
					if (flag2)
					{
						this._skillAndSpecialSoundDict.Add(entry2.Key, entry2.Value);
					}
				}
				bool flag3 = charId != this.Model.CarrierAnimalCombatCharId;
				if (flag3)
				{
					this._skeletonLoaded[charId] = true;
				}
				CombatSkeletonItem skeletonConfig;
				bool flag4 = this.TryGetSpecialCharConfig(charId, out skeletonConfig);
				if (flag4)
				{
					CombatAnimationUtils.UpdateSkeletonSpecial(skeleton, skeletonConfig);
				}
				string loopAnim;
				bool flag5 = this._loopAniDict.TryGetValue(charId, out loopAnim);
				if (flag5)
				{
					this.PlayAni(charId, loopAnim, true);
				}
			}, null, false);
		}

		// Token: 0x06008D19 RID: 36121 RVA: 0x004181C8 File Offset: 0x004163C8
		private void LoadCombatSkillAsset(short skillId)
		{
			bool flag = this._needLoadAssetSkillList.Contains(skillId) || this._loadedAssetSkillList.Contains(skillId);
			if (!flag)
			{
				this._needLoadAssetSkillList.Add(skillId);
				CombatSkillItem skillConfig = CombatSkill.Instance[skillId];
				bool flag2 = !string.IsNullOrEmpty(skillConfig.AssetFileName);
				if (flag2)
				{
					string assetPath = "RemakeResources/Combat/CombatSkills/" + skillConfig.AssetFileName + "/" + skillConfig.AssetFileName;
					ResLoader.Load<CombatSkillAsset>(assetPath, delegate(CombatSkillAsset skillAsset)
					{
						this.ApplyCombatSkillResource(skillAsset, skillConfig);
					}, null, false);
				}
				else
				{
					this._needLoadAssetSkillList.Remove(skillId);
				}
			}
		}

		// Token: 0x06008D1A RID: 36122 RVA: 0x0041828C File Offset: 0x0041648C
		private void ApplyCombatSkillResource(CombatSkillAsset skillAsset, CombatSkillItem skillConfig)
		{
			foreach (RawAnimationAsset rawAnimation in skillAsset.animations)
			{
				this._skillAniDict[rawAnimation.animName] = rawAnimation;
			}
			foreach (GameObject prefabObj in skillAsset.particles)
			{
				RectTransform particleHolder = this.particlePrefabHolder;
				GameObject poolObj = Object.Instantiate<GameObject>(prefabObj, particleHolder);
				this._skillAndSpecialParticleNameList.Add(prefabObj.name);
				CombatPoolAdaptor.SetSrcObject(prefabObj.name, poolObj, 0);
				poolObj.SetActive(false);
			}
			foreach (AudioClip clip in skillAsset.audios)
			{
				this._skillAndSpecialSoundDict[clip.name] = clip;
			}
			this._loadedAssetSkillList.Add(skillConfig.TemplateId);
			this.TryFinishLoad();
		}

		// Token: 0x06008D1B RID: 36123 RVA: 0x0041837C File Offset: 0x0041657C
		private void TryFinishLoad()
		{
			bool flag = this._loadedAssetSkillList.Count != this._needLoadAssetSkillList.Count;
			if (!flag)
			{
				this.Element.ShowAfterRefresh();
			}
		}

		// Token: 0x06008D1C RID: 36124 RVA: 0x004183B7 File Offset: 0x004165B7
		private IEnumerator InitXiangshuScene(short charTemplateId)
		{
			bool canDefeat = charTemplateId == 904 || charTemplateId == 1174;
			while (UI_CombatBackground.Instance.Scene == null)
			{
				yield return null;
			}
			Transform sceneTransform = UI_CombatBackground.Instance.Scene.transform;
			GameObject cloud = sceneTransform.Find("1_10").gameObject;
			GameObject cloud2 = sceneTransform.Find("1_12").gameObject;
			SkeletonGraphic ani1 = sceneTransform.Find("1_11/DiaoXiang/SG_gujia1").GetComponent<SkeletonGraphic>();
			SkeletonGraphic ani2 = sceneTransform.Find("1_11/DiaoXiang/SG_gujia2").GetComponent<SkeletonGraphic>();
			SkeletonGraphic ani3 = sceneTransform.Find("1_11/DiaoXiang/SG_gujia3").GetComponent<SkeletonGraphic>();
			ParticleSystem particle1 = sceneTransform.Find("1_9/Imagey/you_eff").GetComponent<ParticleSystem>();
			ParticleSystem particle2 = sceneTransform.Find("1_9/Imagez/zuo_eff").GetComponent<ParticleSystem>();
			sceneTransform.Find("1_11").gameObject.SetActive(true);
			cloud.SetActive(!canDefeat);
			cloud2.SetActive(!canDefeat);
			particle1.gameObject.SetActive(canDefeat);
			particle2.gameObject.SetActive(canDefeat);
			bool flag = canDefeat;
			if (flag)
			{
				ViewCombat.<>c__DisplayClass414_1 CS$<>8__locals2 = new ViewCombat.<>c__DisplayClass414_1();
				CS$<>8__locals2.dissolveImg = sceneTransform.Find("1_8/RawImage_rongjie").GetComponent<RawImage>();
				CS$<>8__locals2.dissolveImg.material = new UnityEngine.Material(CS$<>8__locals2.dissolveImg.material);
				CS$<>8__locals2.dissolveMat = CS$<>8__locals2.dissolveImg.material;
				ani1.AnimationState.SetAnimation(0, "animation", false);
				ani2.AnimationState.SetAnimation(0, "animation", false);
				ani3.AnimationState.SetAnimation(0, "animation", false);
				CS$<>8__locals2.dissolveMat.SetFloat("_EffectRange", 0f);
				CS$<>8__locals2.dissolveMat.SetFloat("_GradientStart", 0f);
				CS$<>8__locals2.dissolveMat.SetFloat("_GradientWidth", 0.2f);
				CS$<>8__locals2.dissolveImg.SetMaterialDirty();
				DOVirtual.Float(0f, 1f, 5f, delegate(float stepValue)
				{
					CS$<>8__locals2.dissolveMat.SetFloat("_EffectRange", stepValue);
					CS$<>8__locals2.dissolveImg.SetMaterialDirty();
				}).SetDelay(2.5f).SetAutoKill(true);
				DOVirtual.Float(0f, 1f, 5f, delegate(float stepValue)
				{
					CS$<>8__locals2.dissolveMat.SetFloat("_GradientStart", stepValue);
					CS$<>8__locals2.dissolveImg.SetMaterialDirty();
				}).SetDelay(4f).SetAutoKill(true);
				DOVirtual.Float(0.2f, 1f, 5f, delegate(float stepValue)
				{
					CS$<>8__locals2.dissolveMat.SetFloat("_GradientWidth", stepValue);
					CS$<>8__locals2.dissolveImg.SetMaterialDirty();
				}).SetDelay(5f).SetAutoKill(true);
				particle1.Play(true);
				particle2.Play(true);
				DOVirtual.DelayedCall(10f, delegate
				{
					ani1.gameObject.SetActive(false);
					ani2.gameObject.SetActive(false);
					ani3.gameObject.SetActive(false);
					particle1.gameObject.SetActive(false);
					particle2.gameObject.SetActive(false);
				}, true);
				CS$<>8__locals2 = null;
			}
			else
			{
				ani1.AnimationState.SetAnimation(0, "idle", true);
				ani2.AnimationState.SetAnimation(0, "idle", true);
				ani3.AnimationState.SetAnimation(0, "idle", true);
			}
			yield break;
		}

		// Token: 0x06008D1D RID: 36125 RVA: 0x004183D0 File Offset: 0x004165D0
		private void InitDefeatMarkBack(CombatInfoTop infoTopRefers, bool isAlly)
		{
			infoTopRefers.defeatMarkBarBack.fillAmount = ViewCombat.DefeatMarkBarBackFillAmounts[(int)this._combatType];
			((RectTransform)infoTopRefers.defeatMarkSeparator.transform).anchoredPosition = new Vector2(ViewCombat.GetDefeatMarkSeparatorX((int)this._combatType) * (float)(isAlly ? 1 : -1), 1f);
		}

		// Token: 0x06008D1E RID: 36126 RVA: 0x0041842A File Offset: 0x0041662A
		private void SetRaycastBlocked(bool blocked)
		{
			this.selfInfoTop.defeatMarkGroup.RaycastBlocked = blocked;
			this.enemyInfoTop.defeatMarkGroup.RaycastBlocked = blocked;
		}

		// Token: 0x06008D1F RID: 36127 RVA: 0x00418454 File Offset: 0x00416654
		private void UpdateDefeatMarks(bool isAlly, DefeatMarkCollection currMarks, DefeatMarkCollection oldMarks)
		{
			bool inited = isAlly ? this._selfDefeatMarkInitialized : this._enemyDefeatMarkInitialized;
			this.UpdateDefeatMarkBar(isAlly);
			bool flag = inited;
			if (flag)
			{
				int newMarkCount = this.ShowNewMarkDropIcon(isAlly, oldMarks, currMarks);
				RectTransform rectTransform = base.transform as RectTransform;
				bool flag2 = this.SettingData.CombatShake && newMarkCount > 0 && !DOTween.IsTweening(rectTransform, true);
				if (flag2)
				{
					int strength = 10 + Math.Min(newMarkCount * 6, 36);
					float duration = 0.2f + Math.Min((float)newMarkCount * 0.2f, 1.2f);
					this._shakeTweener = rectTransform.DOShakeAnchorPos(duration, (float)strength, 40, 45f, false, true, ShakeRandomnessMode.Full).SetDelay(0.01f).OnComplete(delegate
					{
						((RectTransform)base.transform).anchoredPosition = Vector3.zero;
						this._shakeTweener = null;
					});
				}
			}
			else if (isAlly)
			{
				this._selfDefeatMarkInitialized = true;
			}
			else
			{
				this._enemyDefeatMarkInitialized = true;
			}
			this.UpdateMobilityTips(isAlly);
			this.UpdateMixPoisonAffectedCount(isAlly, currMarks);
		}

		// Token: 0x06008D20 RID: 36128 RVA: 0x00418550 File Offset: 0x00416750
		private unsafe void UpdateMixPoisonAffectedCount(bool isAlly, DefeatMarkCollection currMarks)
		{
			CombatSubProcessorCharacter processor = isAlly ? this.Model.SelfCharacter : this.Model.EnemyCharacter;
			MixPoisonAffectedCountCollection canAffectCollection = processor.MixPoisonCanAffectCount;
			int efficientPoisonCount = 0;
			bool showObj = false;
			foreach (MixPoisonEffectItem mixPoisonEffectItem in ((IEnumerable<MixPoisonEffectItem>)MixPoisonEffect.Instance))
			{
				int canAffectCount = (canAffectCollection != null) ? canAffectCollection.GetAffectedCount(mixPoisonEffectItem.TemplateId) : 0;
				bool flag = canAffectCount == 0;
				if (!flag)
				{
					MixPoisonAffectedCountCollection mixPoisonAffectedCount = isAlly ? this._selfMixPoisonAffectedCount : this._enemyMixPoisonAffectedCount;
					bool flag2 = mixPoisonAffectedCount == null;
					if (flag2)
					{
						return;
					}
					int affectedCount = mixPoisonAffectedCount.GetAffectedCount(mixPoisonEffectItem.TemplateId);
					int leftCount = canAffectCount - affectedCount;
					bool flag3 = canAffectCount > 0;
					if (flag3)
					{
						showObj = true;
					}
					bool flag4 = leftCount > 0;
					if (flag4)
					{
						efficientPoisonCount++;
					}
				}
			}
			GameObject obj = isAlly ? this.selfInfoTop.mixPoison : this.enemyInfoTop.mixPoison;
			obj.GetComponent<CombatCount>().count.text = efficientPoisonCount.ToString();
			obj.SetActive(showObj);
			CombatUtils.UpdateIconHolderVisible(obj.transform.parent);
			TooltipInvoker mouseTip = obj.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = mouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			mouseTip.RuntimeParam.Clear();
			mouseTip.RuntimeParam.Set("CharacterId", isAlly ? (*this._selfCurrCharId) : (*this._enemyCurrCharId));
			mouseTip.RuntimeParam.SetObject("PoisonMarkList", currMarks.PoisonMarkList);
			mouseTip.RuntimeParam.SetObject("MixPoisonAffectedCount", isAlly ? this._selfMixPoisonAffectedCount : this._enemyMixPoisonAffectedCount);
			mouseTip.Refresh(false, -1);
		}

		// Token: 0x06008D21 RID: 36129 RVA: 0x00418734 File Offset: 0x00416934
		private int ShowNewMarkDropIcon(bool isAlly, DefeatMarkCollection lastCollection, DefeatMarkCollection currCollection)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			int newMarkTotalCount = 0;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			int result;
			if (flag)
			{
				result = newMarkTotalCount;
			}
			else
			{
				for (sbyte part = 0; part < 7; part += 1)
				{
					List<byte> flawOrAcupointList = EasyPool.Get<List<byte>>();
					int newMarkCount = (int)(currCollection.OuterInjuryMarkList[(int)part] - lastCollection.OuterInjuryMarkList[(int)part]);
					newMarkTotalCount += newMarkCount;
					newMarkCount = (int)(currCollection.InnerInjuryMarkList[(int)part] - lastCollection.InnerInjuryMarkList[(int)part]);
					newMarkTotalCount += newMarkCount;
					flawOrAcupointList.Clear();
					flawOrAcupointList.AddRange(currCollection.FlawMarkList[(int)part]);
					foreach (byte level in lastCollection.FlawMarkList[(int)part])
					{
						bool flag2 = flawOrAcupointList.Contains(level);
						if (flag2)
						{
							flawOrAcupointList.Remove(level);
						}
					}
					newMarkTotalCount += flawOrAcupointList.Count;
					DefeatMarkKey flawMarkKey = new DefeatMarkKey(EMarkType.Flaw, (int)part, 0);
					this.SummaryDamageNum(charId, flawMarkKey, flawOrAcupointList.Count);
					for (int i = 0; i < flawOrAcupointList.Count; i++)
					{
						this.ShowIconTips(charId, this.GetDropMarkIcon(flawMarkKey), "");
					}
					flawOrAcupointList.Clear();
					flawOrAcupointList.AddRange(currCollection.AcupointMarkList[(int)part]);
					foreach (byte level2 in lastCollection.AcupointMarkList[(int)part])
					{
						bool flag3 = flawOrAcupointList.Contains(level2);
						if (flag3)
						{
							flawOrAcupointList.Remove(level2);
						}
					}
					newMarkTotalCount += flawOrAcupointList.Count;
					DefeatMarkKey acupointMarkKey = new DefeatMarkKey(EMarkType.Acupoint, (int)part, 0);
					this.SummaryDamageNum(charId, acupointMarkKey, flawOrAcupointList.Count);
					for (int j = 0; j < flawOrAcupointList.Count; j++)
					{
						this.ShowIconTips(charId, this.GetDropMarkIcon(acupointMarkKey), "");
					}
					EasyPool.Free<List<byte>>(flawOrAcupointList);
				}
				for (sbyte order = 0; order < 6; order += 1)
				{
					sbyte type = PoisonType.GetTypeBySortingOrder(order);
					int newMarkCount = (int)(currCollection.PoisonMarkList[(int)type] - lastCollection.PoisonMarkList[(int)type]);
					newMarkTotalCount += newMarkCount;
					for (int k = 0; k < newMarkCount; k++)
					{
						this.ShowIconTips(charId, this.GetDropMarkIcon(new DefeatMarkKey(EMarkType.Poison, (int)type, 0)), "");
					}
				}
				newMarkTotalCount += currCollection.MindMarkList.Count - lastCollection.MindMarkList.Count;
				int newFatalDamageCount = currCollection.FatalDamageMarkCount - lastCollection.FatalDamageMarkCount;
				bool flag4 = newFatalDamageCount > 0;
				if (flag4)
				{
					ItemKey[] weaponList = processor.Weapons;
					int weaponIndex = processor.UsingWeaponIndex;
					string fatalParticle = Weapon.Instance[weaponList[weaponIndex].TemplateId].FatalParticle;
					bool flag5 = !string.IsNullOrEmpty(fatalParticle);
					if (flag5)
					{
						this.combatParticle.PlayVfx(isAlly, this.GetSkeleton(charId), fatalParticle);
					}
				}
				List<CombatSkillKey> diedMarkList = EasyPool.Get<List<CombatSkillKey>>();
				diedMarkList.Clear();
				diedMarkList.AddRange(currCollection.DieMarkList);
				foreach (CombatSkillKey skillKey in lastCollection.DieMarkList)
				{
					bool flag6 = diedMarkList.Contains(skillKey);
					if (flag6)
					{
						diedMarkList.Remove(skillKey);
					}
				}
				newMarkTotalCount += diedMarkList.Count;
				DefeatMarkKey dieMarkKey = EMarkType.Die;
				this.SummaryDamageNum(charId, dieMarkKey, diedMarkList.Count);
				for (int l = 0; l < diedMarkList.Count; l++)
				{
					this.ShowIconTips(charId, this.GetDropMarkIcon(dieMarkKey), "");
				}
				EasyPool.Free<List<CombatSkillKey>>(diedMarkList);
				result = newMarkTotalCount;
			}
			return result;
		}

		// Token: 0x06008D22 RID: 36130 RVA: 0x00418B50 File Offset: 0x00416D50
		private void UpdateDefeatMarkBar(bool isAlly)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				DefeatMarkCollection currCollection = processor.DefeatMarkCollection;
				bool flag2 = currCollection == null;
				if (!flag2)
				{
					CombatInfoTop topRefers = isAlly ? this.selfInfoTop : this.enemyInfoTop;
					SortedDictionary<DefeatMarkKey, List<RectTransform>> markDict = isAlly ? this._selfDefeatMarkObjs : this._enemyDefeatMarkObjs;
					List<RectTransform> markQueue = isAlly ? this._selfDefeatMarkAddQueue : this._enemyDefeatMarkAddQueue;
					List<RectTransform> markSeparatorQueue = isAlly ? this._selfDefeatMarkSeparatorAddQueue : this._enemyDefeatMarkSeparatorAddQueue;
					List<DefeatMarkKey> markKeyList = isAlly ? this._selfDefeatMarkKeyList : this._enemyDefeatMarkKeyList;
					RectTransform markHolder = topRefers.defeatMarkHolder;
					List<DefeatMarkKey> lastMarkKeyList = EasyPool.Get<List<DefeatMarkKey>>();
					lastMarkKeyList.Clear();
					foreach (RectTransform mark in markQueue)
					{
						lastMarkKeyList.Add((DefeatMarkKey)mark.GetComponent<CombatDefeatMark>().UserInt);
					}
					markKeyList.Clear();
					short oldDisorderOfQi = processor.OldDisorderOfQi;
					PoisonInts oldPoisons = processor.OldPoison;
					Injuries oldInjuries = processor.OldInjuries;
					markKeyList.AddRange(currCollection.GetAllKeys(oldDisorderOfQi, oldPoisons, oldInjuries));
					int showMarkCount = Mathf.Min(markKeyList.Count, 108);
					int showSeparatorCount = (showMarkCount > this._maxDefeatMarkCount) ? 1 : 0;
					int showedSeparatorCount = 0;
					int i = 0;
					while (i < showMarkCount)
					{
						DefeatMarkKey markKey = markKeyList[i];
						bool flag3 = i == this._maxDefeatMarkCount;
						if (flag3)
						{
							bool flag4 = markSeparatorQueue.Count == showedSeparatorCount;
							if (flag4)
							{
								markSeparatorQueue.Add(CombatPoolAdaptor.GetObject<RectTransform>("ui_Combat_DefeatMarkSeparatorPrefab", true));
							}
							RectTransform parentMark = markQueue[i - 1];
							RectTransform separator = markSeparatorQueue[showedSeparatorCount];
							separator.GetComponent<CombatItemDefeatMarkSeparatorNew>().Set(parentMark);
							showedSeparatorCount++;
						}
						bool flag5 = markQueue.Count > i;
						if (!flag5)
						{
							goto IL_23D;
						}
						RectTransform mark2 = markQueue[i];
						CombatDefeatMark markRefers = mark2.GetComponent<CombatDefeatMark>();
						bool flag6 = markKey == markRefers.UserInt;
						if (!flag6)
						{
							goto IL_23D;
						}
						CImage countDownImg = markRefers.countdown;
						countDownImg.SetSprite(this.GetBarMarkSprite(markKey), false, null);
						IL_315:
						i++;
						continue;
						IL_23D:
						RectTransform markTransform = CombatPoolAdaptor.GetObject<RectTransform>("ui_Combat_DefeatMarkPrefab", true);
						markRefers = markTransform.GetComponent<CombatDefeatMark>();
						countDownImg = markRefers.countdown;
						markRefers.UserInt = markKey;
						countDownImg.SetSprite(this.GetBarMarkSprite(markKey), false, null);
						countDownImg.fillAmount = 1f;
						countDownImg.gameObject.SetActive(true);
						markTransform.GetComponent<CImage>().SetSprite(this.GetBarMarkBack(markKey), false, null);
						markTransform.SetParent(markHolder, false);
						markTransform.SetSiblingIndex(i);
						markTransform.gameObject.SetActive(true);
						markDict.GetOrNew(markKey).Add(markTransform);
						markQueue.Insert(i, markTransform);
						bool flag7 = lastMarkKeyList.Contains(markKey);
						if (flag7)
						{
							lastMarkKeyList.Remove(markKey);
						}
						else
						{
							markRefers.PlayParticle(markKey, isAlly);
						}
						goto IL_315;
					}
					for (int j = markQueue.Count - 1; j >= showMarkCount; j--)
					{
						RectTransform markTransform2 = markQueue[j];
						markDict[(DefeatMarkKey)markTransform2.GetComponent<CombatDefeatMark>().UserInt].Remove(markTransform2);
						markQueue.RemoveAt(j);
						CombatPoolAdaptor.Destroy("ui_Combat_DefeatMarkPrefab", markTransform2.gameObject, true);
						markTransform2.SetParent(base.transform);
					}
					for (int k = markSeparatorQueue.Count - 1; k >= showSeparatorCount; k--)
					{
						RectTransform separatorTransform = markSeparatorQueue[k];
						markSeparatorQueue.RemoveAt(k);
						CombatPoolAdaptor.Destroy("ui_Combat_DefeatMarkSeparatorPrefab", separatorTransform.gameObject, true);
						separatorTransform.SetParent(base.transform);
					}
					EasyPool.Free<List<DefeatMarkKey>>(lastMarkKeyList);
					HorizontalLayoutGroup scroll = markHolder.GetComponent<HorizontalLayoutGroup>();
					float width = 0f;
					for (int l = 0; l < markHolder.childCount; l++)
					{
						width += ((RectTransform)markHolder.GetChild(l).transform).rect.width;
					}
					scroll.spacing = Mathf.Min((markHolder.rect.width - width) / (float)(markHolder.childCount - 1), 6f);
					CombatDefeatMarkGroup group = topRefers.defeatMarkGroup;
					group.Set(markHolder, markQueue, markKeyList, charId);
					CombatDefeatMarkCount markCount = topRefers.defeatMarkCount;
					markCount.Set((CombatType)this._combatType, currCollection);
					string markCountText = ("<color=#lowwarning>" + currCollection.GetTotalCount().ToString() + "</color>/" + GlobalConfig.NeedDefeatMarkCount[(int)this._combatType].ToString()).ColorReplace();
					if (isAlly)
					{
						this.combatQuickUseItemPanel.selfInjuryCount.text = markCountText;
					}
					else
					{
						this.combatQuickUseItemPanel.enemyInjuryCount.text = markCountText;
					}
					CombatItemDefeatMarkSeparator staticSeparator = topRefers.defeatMarkSeparator;
					staticSeparator.Set((CombatType)this._combatType, currCollection);
					staticSeparator.gameObject.SetActive(showSeparatorCount == 0);
					CombatDamageValueHolder damageValueHolder = topRefers.damageValueHolder;
					damageValueHolder.SetMark(currCollection, processor);
					bool flag8 = this._flawTimeDict.ContainsKey(charId);
					if (flag8)
					{
						this.UpdateFlawTime(charId, isAlly);
					}
					bool flag9 = this._acupointTimeDict.ContainsKey(charId);
					if (flag9)
					{
						this.UpdateAcupointTime(charId, isAlly);
					}
				}
			}
		}

		// Token: 0x06008D23 RID: 36131 RVA: 0x00419100 File Offset: 0x00417300
		private void ReleaseMarkPrefab(string prefabKey, List<RectTransform> instances)
		{
			foreach (RectTransform instance in instances)
			{
				CombatPoolAdaptor.Destroy(prefabKey, instance.gameObject, true);
				instance.SetParent(base.transform);
			}
			instances.Clear();
		}

		// Token: 0x06008D24 RID: 36132 RVA: 0x00419170 File Offset: 0x00417370
		private DefeatMarkKey GetDefeatMarkKey(EMarkType type, int subType = 0, int subType2 = 0)
		{
			return new DefeatMarkKey(type, subType, subType2);
		}

		// Token: 0x06008D25 RID: 36133 RVA: 0x0041918C File Offset: 0x0041738C
		private DefeatMarkKey GetDefeatMarkKey(int type, int subType, int subType2)
		{
			return new DefeatMarkKey((EMarkType)type, subType, subType2);
		}

		// Token: 0x06008D26 RID: 36134 RVA: 0x004191A8 File Offset: 0x004173A8
		private string GetBarMarkBack(DefeatMarkKey markKey)
		{
			EMarkType type = markKey.Type;
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case EMarkType.Outer:
			case EMarkType.Inner:
				result = "combat_top_xuetiaoicon_waishang_2";
				break;
			case EMarkType.Flaw:
			case EMarkType.Acupoint:
			{
				int level = markKey.Level;
				if (!true)
				{
				}
				string text;
				switch (level)
				{
				case 0:
					text = "combat_top_xuetiaoicon_dianxue_4";
					break;
				case 1:
					text = "combat_top_xuetiaoicon_dianxue_5";
					break;
				case 2:
					text = "combat_top_xuetiaoicon_dianxue_6";
					break;
				default:
					text = "combat_top_xuetiaoicon_dianxue_7";
					break;
				}
				if (!true)
				{
				}
				result = text;
				break;
			}
			case EMarkType.Poison:
				result = "combat_top_xuetiaoicon_zhongdu_12";
				break;
			case EMarkType.Mind:
				result = "combat_top_xuetiaoicon_shishen_1";
				break;
			default:
				if (type != EMarkType.Scar)
				{
					result = this.GetBarMarkSprite(markKey);
				}
				else
				{
					result = "combat_top_scar_1";
				}
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008D27 RID: 36135 RVA: 0x00419268 File Offset: 0x00417468
		private string GetBarMarkSprite(DefeatMarkKey markKey)
		{
			EMarkType type = markKey.Type;
			if (!true)
			{
			}
			string result;
			switch (type)
			{
			case EMarkType.Outer:
				result = (markKey.Old ? "combat_top_xuetiaoicon_waishang_0" : "combat_top_xuetiaoicon_waishang_1");
				goto IL_2E7;
			case EMarkType.Inner:
				result = (markKey.Old ? "combat_top_xuetiaoicon_neishang_0" : "combat_top_xuetiaoicon_neishang_1");
				goto IL_2E7;
			case EMarkType.Flaw:
			{
				int level = markKey.Level;
				if (!true)
				{
				}
				string text;
				switch (level)
				{
				case 0:
					text = (markKey.Old ? "combat_top_xuetiaoicon_pozhan_4" : "combat_top_xuetiaoicon_pozhan_0");
					break;
				case 1:
					text = (markKey.Old ? "combat_top_xuetiaoicon_pozhan_5" : "combat_top_xuetiaoicon_pozhan_1");
					break;
				case 2:
					text = (markKey.Old ? "combat_top_xuetiaoicon_pozhan_6" : "combat_top_xuetiaoicon_pozhan_2");
					break;
				default:
					text = (markKey.Old ? "combat_top_xuetiaoicon_pozhan_7" : "combat_top_xuetiaoicon_pozhan_3");
					break;
				}
				if (!true)
				{
				}
				result = text;
				goto IL_2E7;
			}
			case EMarkType.Acupoint:
			{
				int level2 = markKey.Level;
				if (!true)
				{
				}
				string text;
				switch (level2)
				{
				case 0:
					text = (markKey.Old ? "combat_top_xuetiaoicon_dianxue_4" : "combat_top_xuetiaoicon_dianxue_0");
					break;
				case 1:
					text = (markKey.Old ? "combat_top_xuetiaoicon_dianxue_5" : "combat_top_xuetiaoicon_dianxue_1");
					break;
				case 2:
					text = (markKey.Old ? "combat_top_xuetiaoicon_dianxue_6" : "combat_top_xuetiaoicon_dianxue_2");
					break;
				default:
					text = (markKey.Old ? "combat_top_xuetiaoicon_dianxue_7" : "combat_top_xuetiaoicon_dianxue_3");
					break;
				}
				if (!true)
				{
				}
				result = text;
				goto IL_2E7;
			}
			case EMarkType.Poison:
			{
				sbyte poisonType = markKey.PoisonType;
				if (!true)
				{
				}
				string text;
				switch (poisonType)
				{
				case 0:
					text = (markKey.Old ? "combat_top_xuetiaoicon_zhongdu_2" : "combat_top_xuetiaoicon_zhongdu_8");
					break;
				case 1:
					text = (markKey.Old ? "combat_top_xuetiaoicon_zhongdu_5" : "combat_top_xuetiaoicon_zhongdu_11");
					break;
				case 2:
					text = (markKey.Old ? "combat_top_xuetiaoicon_zhongdu_1" : "combat_top_xuetiaoicon_zhongdu_7");
					break;
				case 3:
					text = (markKey.Old ? "combat_top_xuetiaoicon_zhongdu_3" : "combat_top_xuetiaoicon_zhongdu_9");
					break;
				case 4:
					text = (markKey.Old ? "combat_top_xuetiaoicon_zhongdu_0" : "combat_top_xuetiaoicon_zhongdu_6");
					break;
				default:
					text = (markKey.Old ? "combat_top_xuetiaoicon_zhongdu_4" : "combat_top_xuetiaoicon_zhongdu_10");
					break;
				}
				if (!true)
				{
				}
				result = text;
				goto IL_2E7;
			}
			case EMarkType.Mind:
				result = (markKey.Old ? "combat_top_xuetiaoicon_shishen_2" : "combat_top_xuetiaoicon_shishen_0");
				goto IL_2E7;
			case EMarkType.Fatal:
				result = "combat_top_xuetiaoicon_zhongchuang_0";
				goto IL_2E7;
			case EMarkType.Die:
				result = "combat_top_xuetiaoicon_pozhan_bisi";
				goto IL_2E7;
			case EMarkType.Wug:
				result = "combat_top_wug_0";
				goto IL_2E7;
			case EMarkType.QiDisorder:
				result = (markKey.Old ? "combat_top_neixi_1" : "combat_top_neixi_0");
				goto IL_2E7;
			case EMarkType.State:
				result = "combat_top_state_0";
				goto IL_2E7;
			case EMarkType.NeiliAllocation:
				result = (markKey.Scatter ? "combat_top_qi_1" : "combat_top_qi_0");
				goto IL_2E7;
			case EMarkType.Scar:
				result = "combat_top_scar_0";
				goto IL_2E7;
			case EMarkType.Tired:
				result = "combat_top_tired";
				goto IL_2E7;
			}
			result = "combat_top_health_0";
			IL_2E7:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008D28 RID: 36136 RVA: 0x00419568 File Offset: 0x00417768
		private string GetDropMarkIcon(DefeatMarkKey markKey)
		{
			EMarkType type = markKey.Type;
			bool flag = type == EMarkType.Outer || type == EMarkType.Inner;
			string result;
			if (flag)
			{
				BodyPartItem bodyPartConfig = BodyPart.Instance[markKey.BodyPart];
				result = ((markKey.Type == EMarkType.Outer) ? bodyPartConfig.OuterInjuryIcon : bodyPartConfig.InnerInjuryIcon);
			}
			else
			{
				EMarkType type2 = markKey.Type;
				if (!true)
				{
				}
				string text;
				switch (type2)
				{
				case EMarkType.Flaw:
					text = string.Format("sp_combat_icon_pozhan_{0}", markKey.BodyPart);
					break;
				case EMarkType.Acupoint:
					text = string.Format("sp_combat_icon_dianxue_{0}", markKey.BodyPart);
					break;
				case EMarkType.Poison:
					text = Poison.Instance[markKey.PoisonType].Icon;
					break;
				case EMarkType.Mind:
					text = "sp_combat_icon_shishen";
					break;
				case EMarkType.Fatal:
					text = "sp_combat_icon_zhongchuang_0";
					break;
				default:
					text = "sp_combat_icon_bisi";
					break;
				}
				if (!true)
				{
				}
				result = text;
			}
			return result;
		}

		// Token: 0x06008D29 RID: 36137 RVA: 0x00419664 File Offset: 0x00417864
		private void UpdateFlawTime(int charId, bool isAlly)
		{
			FlawOrAcupointCollection flawCollection = this._flawTimeDict[charId];
			SortedDictionary<DefeatMarkKey, List<RectTransform>> markDict = isAlly ? this._selfDefeatMarkObjs : this._enemyDefeatMarkObjs;
			for (sbyte bodyPart = 0; bodyPart < 7; bodyPart += 1)
			{
				List<FlawOrAcupointEntry> dataList = flawCollection.BodyPartDict[bodyPart];
				List<int> markIndexList = EasyPool.Get<List<int>>();
				markIndexList.Clear();
				for (int i = 0; i < 4; i++)
				{
					markIndexList.Add(0);
				}
				foreach (FlawOrAcupointEntry data in dataList)
				{
					int index = markIndexList[(int)data.Level];
					DefeatMarkKey key = this.GetDefeatMarkKey(EMarkType.Flaw, (int)bodyPart, (int)data.Level);
					List<RectTransform> flawMarkList;
					bool flag = markDict.TryGetValue(key, out flawMarkList) && flawMarkList.Count > index;
					if (flag)
					{
						flawMarkList[index].GetComponent<CombatDefeatMark>().countdown.fillAmount = (float)data.LeftFrame / (float)data.TotalFrame;
					}
					List<int> list = markIndexList;
					int level = (int)data.Level;
					int num = list[level];
					list[level] = num + 1;
				}
				EasyPool.Free<List<int>>(markIndexList);
			}
		}

		// Token: 0x06008D2A RID: 36138 RVA: 0x004197C4 File Offset: 0x004179C4
		private void UpdateAcupointTime(int charId, bool isAlly)
		{
			FlawOrAcupointCollection acupointCollection = this._acupointTimeDict[charId];
			SortedDictionary<DefeatMarkKey, List<RectTransform>> markDict = isAlly ? this._selfDefeatMarkObjs : this._enemyDefeatMarkObjs;
			for (sbyte bodyPart = 0; bodyPart < 7; bodyPart += 1)
			{
				List<FlawOrAcupointEntry> dataList = acupointCollection.BodyPartDict[bodyPart];
				List<int> markIndexList = EasyPool.Get<List<int>>();
				markIndexList.Clear();
				for (int i = 0; i < 4; i++)
				{
					markIndexList.Add(0);
				}
				foreach (FlawOrAcupointEntry data in dataList)
				{
					int index = markIndexList[(int)data.Level];
					DefeatMarkKey key = this.GetDefeatMarkKey(EMarkType.Acupoint, (int)bodyPart, (int)data.Level);
					List<RectTransform> acupointMarkList;
					bool flag = markDict.TryGetValue(key, out acupointMarkList) && acupointMarkList.Count > index;
					if (flag)
					{
						acupointMarkList[index].GetComponent<CombatDefeatMark>().countdown.fillAmount = (float)data.LeftFrame / (float)data.TotalFrame;
					}
					List<int> list = markIndexList;
					int level = (int)data.Level;
					int num = list[level];
					list[level] = num + 1;
				}
				EasyPool.Free<List<int>>(markIndexList);
			}
		}

		// Token: 0x06008D2B RID: 36139 RVA: 0x00419924 File Offset: 0x00417B24
		private void UpdateMindMarkTime(bool isAlly, MindMarkList markList)
		{
			SortedDictionary<DefeatMarkKey, List<RectTransform>> markDict = isAlly ? this._selfDefeatMarkObjs : this._enemyDefeatMarkObjs;
			List<RectTransform> mindMarkList;
			bool flag = markList.MarkList == null || !markDict.TryGetValue(this.GetDefeatMarkKey(EMarkType.Mind, 0, 0), out mindMarkList);
			if (!flag)
			{
				List<CountdownData> notInfinityMarks = EasyPool.Get<List<CountdownData>>();
				notInfinityMarks.Clear();
				notInfinityMarks.AddRange(from x in markList.MarkList
				where !x.Infinite
				select x);
				for (int i = 0; i < mindMarkList.Count; i++)
				{
					bool flag2 = i >= notInfinityMarks.Count;
					if (flag2)
					{
						break;
					}
					mindMarkList[i].GetComponent<CombatDefeatMark>().countdown.fillAmount = notInfinityMarks[i].Progress;
				}
				EasyPool.Free<List<CountdownData>>(notInfinityMarks);
			}
		}

		// Token: 0x06008D2C RID: 36140 RVA: 0x00419A0C File Offset: 0x00417C0C
		private void UpdateScarMarkTime(bool isAlly, List<CountdownData> markList)
		{
			SortedDictionary<DefeatMarkKey, List<RectTransform>> markDict = isAlly ? this._selfDefeatMarkObjs : this._enemyDefeatMarkObjs;
			List<RectTransform> marks;
			bool flag = markList == null || markList.Count <= 0 || !markDict.TryGetValue(this.GetDefeatMarkKey(EMarkType.Scar, 0, 0), out marks);
			if (!flag)
			{
				for (int i = 0; i < markList.Count; i++)
				{
					bool flag2 = i >= marks.Count;
					if (flag2)
					{
						break;
					}
					marks[i].GetComponent<CombatDefeatMark>().countdown.fillAmount = markList[i].Progress;
				}
			}
		}

		// Token: 0x06008D2D RID: 36141 RVA: 0x00419AAA File Offset: 0x00417CAA
		private void UpdateInjuryAutoHealProgress(bool isAlly)
		{
			this.UpdateInjuryAutoHealProgress(isAlly, true, true);
			this.UpdateInjuryAutoHealProgress(isAlly, false, true);
			this.UpdateInjuryAutoHealProgress(isAlly, true, false);
			this.UpdateInjuryAutoHealProgress(isAlly, false, false);
		}

		// Token: 0x06008D2E RID: 36142 RVA: 0x00419AD8 File Offset: 0x00417CD8
		private void UpdateInjuryAutoHealProgress(bool isAlly, bool isInner, bool isOld)
		{
			InjuryAutoHealCollection healProgress;
			if (isAlly)
			{
				healProgress = (isOld ? this._selfOldInjuryAutoHealCollection : this._selfInjuryAutoHealCollection);
			}
			else
			{
				healProgress = (isOld ? this._enemyOldInjuryAutoHealCollection : this._enemyInjuryAutoHealCollection);
			}
			bool flag = healProgress == null;
			if (!flag)
			{
				SortedDictionary<DefeatMarkKey, List<RectTransform>> markDict = isAlly ? this._selfDefeatMarkObjs : this._enemyDefeatMarkObjs;
				for (sbyte bodyPart = 0; bodyPart < 7; bodyPart += 1)
				{
					List<short> progressList = (isInner ? healProgress.InnerBodyPartList : healProgress.OuterBodyPartList)[(int)bodyPart];
					DefeatMarkKey markKey = this.GetDefeatMarkKey(isInner ? 1 : 0, (int)bodyPart, isOld ? 1 : 0);
					List<RectTransform> markList;
					bool flag2 = !markDict.TryGetValue(markKey, out markList);
					if (!flag2)
					{
						for (int i = 0; i < markList.Count; i++)
						{
							int progress = (int)((progressList.Count > i) ? progressList[i] : 0);
							CImage countdown = markList[i].GetComponent<CombatDefeatMark>().countdown;
							countdown.fillAmount = 1f - (float)progress / 900f;
						}
					}
				}
			}
		}

		// Token: 0x06008D2F RID: 36143 RVA: 0x00419BF8 File Offset: 0x00417DF8
		private void UpdateHeavyOrBreakInjuryData(bool isAlly, HeavyOrBreakInjuryData data)
		{
			CombatInfoTop topRefers = isAlly ? this.selfInfoTop : this.enemyInfoTop;
			CombatDefeatMarkGroup markGroup = topRefers.defeatMarkGroup;
			markGroup.Set(data);
			CombatDamageValueHolder damageValueHolder = topRefers.damageValueHolder;
			damageValueHolder.SetHeavyOrBreak(data);
		}

		// Token: 0x06008D30 RID: 36144 RVA: 0x00419C38 File Offset: 0x00417E38
		private void OnEnterProactiveSkillView(CombatProactiveSkillView skillView, CombatSkillDisplayData skillData)
		{
			this._mouseOverSkill = skillData.TemplateId;
			CombatSkillItem configData = CombatSkill.Instance[skillData.TemplateId];
			this.ShowSkillPreview(configData, skillView.GetHolderComponent<CButton>().interactable);
			skillView.OnMouseEnter();
		}

		// Token: 0x06008D31 RID: 36145 RVA: 0x00419C80 File Offset: 0x00417E80
		private void OnExitProactiveSkillView(CombatProactiveSkillView skillView, CombatSkillDisplayData skillData)
		{
			this._mouseOverSkill = -1;
			bool flag = skillData.TemplateId != this._selfReserveData.NeedUseSkillId;
			if (flag)
			{
				this.HideSkillPreview();
			}
			bool flag2 = this._selfReserveData.NeedUseSkillId >= 0;
			if (flag2)
			{
				this.ShowSkillPreview(CombatSkill.Instance[this._selfReserveData.NeedUseSkillId], true);
			}
			skillView.OnMouseExit();
		}

		// Token: 0x06008D32 RID: 36146 RVA: 0x00419CF0 File Offset: 0x00417EF0
		private void ShowSkillPreview(CombatSkillItem skillConfig, bool canUse)
		{
			this._previewRangeSkill = ((skillConfig.EquipType == 1) ? skillConfig.TemplateId : -1);
			this._previewRangeSkillCharId = this.Model.SelfCharId;
			this.Model.RequestGetPreviewCostSkillDisplayData(skillConfig.TemplateId, canUse, delegate
			{
				bool flag = this.Model.PreviewCostSkillData.TemplateId == this.Model.PreviewCostSkill;
				if (flag)
				{
					this.UpdateSkillCostPreview();
				}
				this.UpdateAttackRangePreview();
			});
		}

		// Token: 0x06008D33 RID: 36147 RVA: 0x00419D48 File Offset: 0x00417F48
		private void HideSkillPreview()
		{
			bool flag = this._previewRangeSkill >= 0;
			if (flag)
			{
				this._previewRangeSkill = -1;
				this._previewRangeSkillCharId = -1;
				this.UpdateAttackRangePreview();
			}
			this.Model.ClearPreviewCostSkillData();
			this.UpdateSkillCostPreview();
		}

		// Token: 0x06008D34 RID: 36148 RVA: 0x00419D90 File Offset: 0x00417F90
		private void CastProactiveSkill(int index)
		{
			short skillTemplateId = this.Model.OrderedProactiveSkillList[this.Model.SelfCharId][index];
			CombatSkillKey combatSkillKey = new CombatSkillKey(this.Model.SelfCharId, skillTemplateId);
			CombatSubProcessorSkill processorSkill;
			bool hasCanUseData = this.Model.ProcessorSkills.TryGetValue(combatSkillKey, out processorSkill);
			bool flag = !hasCanUseData || !processorSkill.CanUse;
			if (flag)
			{
				bool flag2 = skillTemplateId == this._selfAffectingMoveSkillId;
				if (flag2)
				{
					this.moveSkillBreaker.DoBreak();
				}
				else
				{
					bool flag3 = skillTemplateId == this._selfAffectingDefendSkillId;
					if (flag3)
					{
						this.defendSkillBreakerExtra.DoBreak();
					}
				}
			}
			else
			{
				bool flag4 = !this.DoRequestCastSkill(skillTemplateId);
				if (!flag4)
				{
					this.combatSkillScroll.OnRequestCastSkill(index);
					this.HideAttackTips();
					bool keepPauseUntilCastSkill = this._keepPauseUntilCastSkill;
					if (keepPauseUntilCastSkill)
					{
						this._keepPauseUntilCastSkill = false;
						this.combatTimeScaleToggle.PauseInteractable = true;
						this.Resume();
					}
				}
			}
		}

		// Token: 0x06008D35 RID: 36149 RVA: 0x00419E88 File Offset: 0x00418088
		private void OnPerformingSkillIdChanged(bool isAlly)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				short skillId = processor.PerformingSkillId;
				bool flag2 = skillId < 0;
				if (flag2)
				{
					this.damageCompare.gameObject.SetActive(false);
					GlobalDomainMethod.Call.InvokeGuidingTrigger(172);
				}
			}
		}

		// Token: 0x06008D36 RID: 36150 RVA: 0x00419EFE File Offset: 0x004180FE
		private void UpdateSkillCostPreview()
		{
			this.UpdateSkillCostMobilityPreview();
			this.Model.RaiseEvent(ECombatEvents.UpdateSkillCostPreview);
		}

		// Token: 0x06008D37 RID: 36151 RVA: 0x00419F18 File Offset: 0x00418118
		private void UpdateSkillCostMobilityPreview()
		{
			CImage mobilityBar = this.selfInfoChar.mobilityBar;
			CImage costMobilityBar = this.selfInfoChar.costMobility;
			RectTransform costMobilityBorder = this.selfInfoChar.costMobilityBorder;
			RectTransform costMobilityLine = this.selfInfoChar.costMobilityLine;
			bool flag = this.Model.ShowSkillCostPreview && this.Model.PreviewCostSkillData.CostMobility > 0;
			if (flag)
			{
				int costMobility = MoveSpecialConstants.MaxMobility * (int)this.Model.PreviewCostSkillData.CostMobility / 100;
				costMobilityBar.fillAmount = (float)this._selfMobility / (float)MoveSpecialConstants.MaxMobility;
				costMobilityBorder.gameObject.SetActive(true);
				costMobilityLine.gameObject.SetActive(true);
				float mobilityBarWidth = mobilityBar.rectTransform.sizeDelta.x;
				mobilityBar.fillAmount = (float)(this._selfMobility - costMobility) / (float)MoveSpecialConstants.MaxMobility;
				costMobilityBorder.anchoredPosition = costMobilityBorder.anchoredPosition.SetX(mobilityBarWidth * mobilityBar.fillAmount - 1f);
				costMobilityBorder.sizeDelta = costMobilityBorder.sizeDelta.SetX(mobilityBarWidth * (float)this.Model.PreviewCostSkillData.CostMobility / 100f + 13f);
				costMobilityLine.anchoredPosition = costMobilityLine.anchoredPosition.SetX(mobilityBarWidth * mobilityBar.fillAmount + 2f);
			}
			else
			{
				mobilityBar.fillAmount = (float)this._selfMobility / (float)MoveSpecialConstants.MaxMobility;
				costMobilityBar.fillAmount = 0f;
				costMobilityBorder.gameObject.SetActive(false);
				costMobilityLine.gameObject.SetActive(false);
			}
		}

		// Token: 0x06008D38 RID: 36152 RVA: 0x0041A0AC File Offset: 0x004182AC
		private void UpdateSkeleton(int charId, SkeletonAnimation skeleton)
		{
			bool flag = charId == this.Model.CarrierAnimalCombatCharId;
			if (!flag)
			{
				List<ItemDisplayData> equipments = this.Model.EquipmentDataCache[charId];
				CombatSkeletonItem skeletonConfig;
				bool flag2 = this.TryGetSpecialCharConfig(charId, out skeletonConfig);
				if (flag2)
				{
					CombatAnimationUtils.UpdateSkeletonSpecial(skeleton, skeletonConfig);
				}
				else
				{
					CharacterDisplayData displayData;
					bool flag3 = this._charDisplayDataDict.TryGetValue(charId, out displayData);
					if (flag3)
					{
						CombatAnimationUtils.UpdateSkeleton(skeleton, displayData, equipments, (charId == this._selfTeam[0]) ? this.Model.HideSkeletonEquipSlots.GetOrDefault(this.Model.CurrEquipmentPlanId).Items : null);
					}
					else
					{
						PredefinedLog.Show(8, string.Format("UpdateSkeleton with missing displayData {0}", charId));
					}
				}
			}
		}

		// Token: 0x06008D39 RID: 36153 RVA: 0x0041A164 File Offset: 0x00418364
		private void UpdateSkeletonsByVirtualCamera()
		{
			bool flag = !this.SelfCurrCharSkeleton || !this.EnemyCurrCharSkeleton;
			if (!flag)
			{
				float selfScreenPos = this.GetCurrCharScreenPos(true);
				float enemyScreenPos = this.GetCurrCharScreenPos(false);
				float distance = CombatVirtualCamera.Distance(selfScreenPos, enemyScreenPos);
				this.spineLayer.anchoredPosition = new Vector2(0f, this.spineLayer.anchoredPosition.y);
				float spineLayerScale = this._virtualCamera.GetSpineLayerScale();
				bool flag2 = float.IsNaN(spineLayerScale) || float.IsInfinity(spineLayerScale) || spineLayerScale <= 0f;
				if (flag2)
				{
					spineLayerScale = 1f;
				}
				this.spineLayer.localScale = new Vector3(spineLayerScale, spineLayerScale, spineLayerScale);
				foreach (KeyValuePair<SkeletonAnimation, ViewCombat.SkeletonMoveInfo> keyValuePair in this._skeletonMoveInfoDict)
				{
					SkeletonAnimation skeletonAnimation;
					ViewCombat.SkeletonMoveInfo skeletonMoveInfo;
					keyValuePair.Deconstruct(out skeletonAnimation, out skeletonMoveInfo);
					SkeletonAnimation skeleton = skeletonAnimation;
					ViewCombat.SkeletonMoveInfo moveInfo = skeletonMoveInfo;
					float posX = this._virtualCamera.CalculateSpineLayerPositionX(moveInfo.Pos);
					this.SetSkeletonPos(skeleton, posX, moveInfo.UpdateParticlePos);
				}
				UI_CombatBackground.Instance.SetOffset(this._virtualCamera.GetBackgroundOffset());
				float backgroundScale = this._virtualCamera.GetBackgroundScale(distance);
				global::CombatScene scene = UI_CombatBackground.Instance.Scene;
				if (scene != null)
				{
					scene.UpdateSceneScale(backgroundScale, Time.deltaTime);
				}
				float scale = this._virtualCamera.GetScale();
				bool flag3 = float.IsNaN(scale) || float.IsInfinity(scale) || scale <= 0.0001f;
				if (flag3)
				{
					scale = 1f;
				}
				for (int i = 0; i < this.selfSkeletonList.Count; i++)
				{
					this.selfSkeletonList[i].GetComponent<CombatSpineSkeleton>().currPosMark.rectTransform.localScale = Vector3.one / scale;
					this.enemySkeletonList[i].GetComponent<CombatSpineSkeleton>().currPosMark.rectTransform.localScale = Vector3.one / scale;
				}
			}
		}

		// Token: 0x06008D3A RID: 36154 RVA: 0x0041A39C File Offset: 0x0041859C
		private void SetSkeletonPos(SkeletonAnimation skeleton, float posX, bool updateParticlePos)
		{
			skeleton.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, skeleton.GetComponent<RectTransform>().anchoredPosition.y);
			this.UpdateOtherBySkeletonPos(updateParticlePos);
		}

		// Token: 0x06008D3B RID: 36155 RVA: 0x0041A3CC File Offset: 0x004185CC
		private void UpdateOtherBySkeletonPos(bool updateParticlePos)
		{
			this.UpdateTargetDistanceBar();
			this.UpdateAttackRangeBar();
			this.UpdateAttackRangePreview();
			this.UpdateDefendBounceRange();
			if (updateParticlePos)
			{
				this.combatParticle.UpdateAllParticlePos();
			}
		}

		// Token: 0x06008D3C RID: 36156 RVA: 0x0041A408 File Offset: 0x00418608
		private void SetPosition(bool isAlly, int position)
		{
			int currPos = isAlly ? this._selfPos : this._enemyPos;
			bool hasDisplayPos = (isAlly ? this._selfDisplayPos : this._enemyDisplayPos) != int.MinValue;
			bool doAni = currPos != int.MinValue;
			if (isAlly)
			{
				this._selfPos = position;
			}
			else
			{
				this._enemyPos = position;
			}
			bool flag = hasDisplayPos;
			if (!flag)
			{
				float screenPos = this.GetCurrCharScreenPos(isAlly);
				float otherScreenPos = this.GetCurrCharScreenPos(!isAlly);
				if (isAlly)
				{
					this.UpdateVirtualCameraTargetData(screenPos, otherScreenPos);
				}
				else
				{
					this.UpdateVirtualCameraTargetData(otherScreenPos, screenPos);
				}
				bool flag2 = doAni;
				if (flag2)
				{
					float moveTime = isAlly ? this._selfChangeDistanceDuration : this._enemyChangeDistanceDuration;
					this.SetSkeletonPos(isAlly ? this.SelfCurrCharSkeleton : this.EnemyCurrCharSkeleton, screenPos, moveTime, false, false);
				}
				else
				{
					SkeletonAnimation skeleton = isAlly ? this.SelfCurrCharSkeleton : this.EnemyCurrCharSkeleton;
					this.SetSkeletonPos(skeleton, screenPos, 0f, false, false);
				}
			}
		}

		// Token: 0x06008D3D RID: 36157 RVA: 0x0041A508 File Offset: 0x00418708
		private void SetDisplayPosition(int charId, bool isAlly, bool isCurrChar, int position)
		{
			if (isCurrChar)
			{
				if (isAlly)
				{
					this._selfDisplayPos = position;
				}
				else
				{
					this._enemyDisplayPos = position;
				}
				float moveTime = 0.1f;
				float screenPos = this.GetCurrCharScreenPos(isAlly);
				SkeletonAnimation skeleton = isAlly ? this.SelfCurrCharSkeleton : this.EnemyCurrCharSkeleton;
				this.SetSkeletonPos(skeleton, screenPos, moveTime, false, false);
				this.UpdateVirtualCameraTargetData(screenPos, this.GetCurrCharScreenPos(!isAlly));
				this.attackRangeBar.gameObject.SetActive(position == int.MinValue);
				this.targetDistanceBar.gameObject.SetActive(position == int.MinValue);
				bool flag = position != int.MinValue && !this._isDisplayDistanceChanging;
				if (flag)
				{
					this._isDisplayDistanceChanging = true;
					this._isAllyDisplayDistanceChanging = isAlly;
					bool flag2 = this._displayDistanceChangeCoroutine != null;
					if (flag2)
					{
						base.StopCoroutine(this._displayDistanceChangeCoroutine);
					}
					this._displayDistanceChangeCoroutine = base.StartCoroutine(this.UpdateCharBottomInfoPos());
				}
			}
			else
			{
				SkeletonAnimation mainSkeleton = isAlly ? this.SelfCurrCharSkeleton : this.EnemyCurrCharSkeleton;
				bool enter = !this._displayPosDict.ContainsKey(charId) || this._displayPosDict[charId] == int.MinValue;
				bool exit = position == int.MinValue;
				float moveTime2 = enter ? 0.56666666f : (exit ? ((float)((charId != this.Model.CarrierAnimalCombatCharId) ? 48 : 24) / 60f) : 0.1f);
				float mainSkeletonPosx = this._skeletonMoveInfoDict.GetOrDefault(mainSkeleton).Pos;
				float screenPos2 = (!exit) ? ((float)(position * 10)) : (mainSkeletonPosx + (float)(-1500 * (isAlly ? 1 : -1)));
				this._displayPosDict[charId] = position;
				this.SetSkeletonPos(this.GetSkeleton(charId), screenPos2, moveTime2, true, false);
			}
		}

		// Token: 0x06008D3E RID: 36158 RVA: 0x0041A6D8 File Offset: 0x004188D8
		private void ResetMobilityAvoid()
		{
			bool flag = this._displayDistanceChangeCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._displayDistanceChangeCoroutine);
			}
			bool isDisplayDistanceChanging = this._isDisplayDistanceChanging;
			if (isDisplayDistanceChanging)
			{
				this._isDisplayDistanceChanging = false;
			}
		}

		// Token: 0x06008D3F RID: 36159 RVA: 0x0041A715 File Offset: 0x00418915
		private IEnumerator UpdateCharBottomInfoPos()
		{
			CombatInfoChar infoRefers = this._isAllyDisplayDistanceChanging ? this.selfInfoChar : this.enemyInfoChar;
			RectTransform infoTransform = infoRefers.GetComponent<RectTransform>();
			RectTransform mobilityTransform = infoRefers.mobility;
			RectTransform changingSkeletonTransform = (this._isAllyDisplayDistanceChanging ? this.SelfCurrCharSkeleton : this.EnemyCurrCharSkeleton).GetComponent<RectTransform>();
			RectTransform otherSkeletonTransform = (this._isAllyDisplayDistanceChanging ? this.EnemyCurrCharSkeleton : this.SelfCurrCharSkeleton).GetComponent<RectTransform>();
			float charInfoWidth = infoTransform.sizeDelta.x;
			int iterations = 0;
			bool hasLoggedMaxIterations = false;
			float initialMobilityPos = mobilityTransform.anchoredPosition.x;
			for (;;)
			{
				int num = iterations;
				iterations = num + 1;
				bool flag = iterations > 4096 && !hasLoggedMaxIterations;
				if (flag)
				{
					hasLoggedMaxIterations = true;
				}
				float localScale = this._virtualCamera.GetScale();
				Vector2 anchoredPosition = this.spineLayer.anchoredPosition;
				float selfInfoPos = changingSkeletonTransform.anchoredPosition.x * localScale + anchoredPosition.x;
				float enemyInfoPos = otherSkeletonTransform.anchoredPosition.x * localScale + anchoredPosition.x;
				float num2 = this._isAllyDisplayDistanceChanging ? (enemyInfoPos - charInfoWidth) : (enemyInfoPos + charInfoWidth);
				int delta = this._isAllyDisplayDistanceChanging ? 100 : -100;
				int currentDisplayPos = this._isAllyDisplayDistanceChanging ? this._selfDisplayPos : this._enemyDisplayPos;
				bool flag2 = currentDisplayPos == int.MinValue && Mathf.Approximately(mobilityTransform.anchoredPosition.x, (float)delta);
				if (flag2)
				{
					break;
				}
				yield return null;
				bool flag3 = this._realTimeScale <= 0f;
				if (flag3)
				{
					num = iterations;
					iterations = num - 1;
				}
				anchoredPosition = default(Vector2);
			}
			this._isDisplayDistanceChanging = false;
			yield break;
		}

		// Token: 0x06008D40 RID: 36160 RVA: 0x0041A724 File Offset: 0x00418924
		private void UpdateVirtualCameraTargetData(float selfPos, float enemyPos)
		{
			this._virtualCamera.UpdateTargetData(selfPos, enemyPos);
		}

		// Token: 0x06008D41 RID: 36161 RVA: 0x0041A738 File Offset: 0x00418938
		private void UpdateParticleCameraOrthoSize(float duration)
		{
			float scaleFactor = this._virtualCamera.GetEquivalentScaleFactor();
			float orthoSize = Mathf.Lerp(2.5f, 3.7f, 1f - scaleFactor);
			this.combatParticle.DoCameraOrthoSize(orthoSize, duration);
		}

		// Token: 0x06008D42 RID: 36162 RVA: 0x0041A778 File Offset: 0x00418978
		private float GetCurrCharScreenPos(bool isAlly)
		{
			int position = isAlly ? this._selfDisplayPos : this._enemyDisplayPos;
			bool flag = position != int.MinValue;
			float result;
			if (flag)
			{
				result = (float)(position * 10);
			}
			else
			{
				position = (isAlly ? this._selfPos : this._enemyPos);
				result = (float)(position * 10);
			}
			return result;
		}

		// Token: 0x06008D43 RID: 36163 RVA: 0x0041A7CC File Offset: 0x004189CC
		private void SetSkeletonPos(SkeletonAnimation skeleton, float screenPos, float moveDuration, bool updateParticlePos = false, bool noTween = false)
		{
			ViewCombat.SkeletonMoveInfo moveInfo;
			bool flag = noTween || !this._skeletonMoveInfoDict.TryGetValue(skeleton, out moveInfo);
			if (flag)
			{
				Tweener tweener;
				bool flag2 = this._moveTweenerDict.TryGetValue(skeleton, out tweener);
				if (flag2)
				{
					CommonUtils.TryKillTween(tweener, false);
				}
				this._skeletonMoveInfoDict[skeleton] = new ViewCombat.SkeletonMoveInfo
				{
					Pos = screenPos,
					UpdateParticlePos = updateParticlePos
				};
			}
			else
			{
				float startPos = moveInfo.Pos;
				Tweener tweener2;
				bool flag3 = this._moveTweenerDict.TryGetValue(skeleton, out tweener2);
				if (flag3)
				{
					CommonUtils.TryKillTween(tweener2, false);
				}
				this._moveTweenerDict[skeleton] = DOVirtual.Float(startPos, screenPos, moveDuration, delegate(float x)
				{
					this._skeletonMoveInfoDict[skeleton] = new ViewCombat.SkeletonMoveInfo
					{
						Pos = x,
						UpdateParticlePos = moveInfo.UpdateParticlePos
					};
				}).SetEase(Ease.Linear).OnComplete(delegate
				{
					this._moveTweenerDict.Remove(skeleton);
				});
			}
		}

		// Token: 0x06008D44 RID: 36164 RVA: 0x0041A8D8 File Offset: 0x00418AD8
		private SkeletonAnimation GetSkeleton(int charId)
		{
			bool flag = charId == this.Model.CarrierAnimalCombatCharId;
			SkeletonAnimation result;
			if (flag)
			{
				result = this.selfCarrierAnimalSkeleton;
			}
			else
			{
				bool flag2 = charId == this.Model.SpecialShowCombatCharId;
				if (flag2)
				{
					result = this.selfSpecialShowSkeleton;
				}
				else
				{
					IReadOnlyList<int> teamList = this._selfTeam.Contains(charId) ? this._selfTeam : this._enemyTeam;
					int skeletonIndex = teamList.IndexOf(charId);
					bool flag3 = skeletonIndex < 0;
					if (flag3)
					{
						result = null;
					}
					else
					{
						List<SkeletonAnimation> teammateSkeletonList = this._selfTeam.Contains(charId) ? this.selfSkeletonList : this.enemySkeletonList;
						result = teammateSkeletonList[skeletonIndex];
					}
				}
			}
			return result;
		}

		// Token: 0x06008D45 RID: 36165 RVA: 0x0041A980 File Offset: 0x00418B80
		private string GetInitialSkinName(int charId)
		{
			CombatSkeletonItem skeletonConfig;
			bool flag = this.TryGetSpecialCharConfig(charId, out skeletonConfig);
			string result;
			if (flag)
			{
				result = skeletonConfig.SkinName;
			}
			else
			{
				BossItem bossItem;
				AnimalItem animalItem;
				bool flag2 = this.TryGetBossConfig(charId, out bossItem) || this.TryGetAnimalConfig(charId, out animalItem);
				if (flag2)
				{
					result = "default";
				}
				else
				{
					CharacterDisplayData charData;
					bool flag3 = !this._charDisplayDataDict.TryGetValue(charId, out charData);
					if (flag3)
					{
						PredefinedLog.Show(8, string.Format("GetInitialSkinName without displayData {0}", charId));
						result = "default";
					}
					else
					{
						result = ((charData.AvatarRelatedData.DisplayAge < 16) ? "kid" : ((charData.AvatarRelatedData.AvatarData.Gender == 0) ? "female" : "doll"));
					}
				}
			}
			return result;
		}

		// Token: 0x06008D46 RID: 36166 RVA: 0x0041AA44 File Offset: 0x00418C44
		private bool TryGetSpecialCharConfig(int charId, out CombatSkeletonItem skeletonConfig)
		{
			skeletonConfig = null;
			CharacterDisplayData displayData;
			bool flag = !this._charDisplayDataDict.TryGetValue(charId, out displayData);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				CharacterItem charConfig = Character.Instance[displayData.TemplateId];
				bool flag2 = charConfig == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					skeletonConfig = CombatSkeleton.Instance[charConfig.SpecialCombatSkeleton];
					result = (skeletonConfig != null);
				}
			}
			return result;
		}

		// Token: 0x06008D47 RID: 36167 RVA: 0x0041AAA8 File Offset: 0x00418CA8
		private bool TryGetBossConfig(int charId, out BossItem bossConfig)
		{
			bossConfig = null;
			CharacterDisplayData displayData;
			bool flag = !this.Model.DisplayDataCache.TryGetValue(charId, out displayData);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				sbyte bossId;
				bool flag2 = !ViewCombat.CharId2BossId.TryGetValue(displayData.TemplateId, out bossId);
				if (flag2)
				{
					result = false;
				}
				else
				{
					bossConfig = Boss.Instance[bossId];
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06008D48 RID: 36168 RVA: 0x0041AB08 File Offset: 0x00418D08
		private bool TryGetAnimalConfig(int charId, out AnimalItem animalConfig)
		{
			animalConfig = null;
			CharacterDisplayData displayData;
			bool flag = !this._charDisplayDataDict.TryGetValue(charId, out displayData);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				sbyte animalId;
				bool flag2 = !GameData.Domains.Combat.SharedConstValue.CharId2AnimalId.TryGetValue(displayData.TemplateId, out animalId);
				if (flag2)
				{
					result = false;
				}
				else
				{
					animalConfig = Config.Animal.Instance[animalId];
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06008D49 RID: 36169 RVA: 0x0041AB64 File Offset: 0x00418D64
		private void SetLoopAni(int charId, string aniName)
		{
			string currAni = this._loopAniDict.GetOrDefault(charId);
			bool flag = currAni == aniName;
			if (!flag)
			{
				this._loopAniDict[charId] = aniName;
				bool flag2 = !this._skeletonLoaded.ContainsKey(charId);
				if (!flag2)
				{
					SkeletonAnimation skeleton = this.GetSkeleton(charId);
					bool flag3 = skeleton.AnimationState != null && !string.IsNullOrEmpty(aniName) && (skeleton.AnimationState.GetCurrent(0) == null || skeleton.AnimationState.GetCurrent(0).IsComplete || skeleton.AnimationState.GetCurrent(0).Loop);
					if (flag3)
					{
						this.PlayAni(charId, aniName, true);
					}
				}
			}
		}

		// Token: 0x06008D4A RID: 36170 RVA: 0x0041AC14 File Offset: 0x00418E14
		private void SetAniToPlayOnce(int charId, string aniName)
		{
			bool flag = string.IsNullOrEmpty(aniName);
			if (!flag)
			{
				SkeletonAnimation skeleton = this.GetSkeleton(charId);
				bool flag2 = skeleton.AnimationState != null;
				if (flag2)
				{
					TrackEntry track = this.PlayAni(charId, aniName, false);
					bool flag3 = track != null;
					if (flag3)
					{
						track.Complete += delegate(TrackEntry a)
						{
							string loopAni = this._loopAniDict.GetOrDefault(charId);
							bool flag4 = skeleton.AnimationState.GetCurrent(0).IsComplete && !string.IsNullOrEmpty(loopAni);
							if (flag4)
							{
								this.PlayAni(charId, loopAni, true);
							}
						};
					}
				}
			}
		}

		// Token: 0x06008D4B RID: 36171 RVA: 0x0041AC94 File Offset: 0x00418E94
		private TrackEntry PlayAni(int charId, string aniName, bool loop)
		{
			bool isAlly = this._selfTeam.Contains(charId);
			SkeletonAnimation skeleton = this.GetSkeleton(charId);
			string finalAniName = aniName;
			BossItem bossConfig;
			bool flag = this.TryGetBossConfig(charId, out bossConfig);
			if (flag)
			{
				SkeletonAnimation petSkeleton = isAlly ? this._selfPetSkeleton : this._enemyPetSkeleton;
				bool flag2 = petSkeleton.gameObject.activeSelf && bossConfig.PetAniPrefix != null;
				if (flag2)
				{
					List<string> petAniPrefix = bossConfig.PetAniPrefix;
					int index;
					if (!isAlly)
					{
						CombatSubProcessorCharacter enemyCharacter = this.Model.EnemyCharacter;
						index = (int)((enemyCharacter != null) ? enemyCharacter.BossPhase : 0);
					}
					else
					{
						CombatSubProcessorCharacter selfCharacter = this.Model.SelfCharacter;
						index = (int)((selfCharacter != null) ? selfCharacter.BossPhase : 0);
					}
					string petAniName = petAniPrefix[index] + aniName;
					petSkeleton.AnimationState.SetAnimation(0, petAniName, loop);
				}
				int num;
				if (!isAlly)
				{
					CombatSubProcessorCharacter enemyCharacter2 = this.Model.EnemyCharacter;
					num = (int)((enemyCharacter2 != null) ? enemyCharacter2.BossPhase : 0);
				}
				else
				{
					CombatSubProcessorCharacter selfCharacter2 = this.Model.SelfCharacter;
					num = (int)((selfCharacter2 != null) ? selfCharacter2.BossPhase : 0);
				}
				int bossPhase = num;
				bool flag3 = this.Model.Config.Scene < 0 && aniName == bossConfig.FailAnimation;
				if (flag3)
				{
					bossPhase = 1;
				}
				finalAniName = bossConfig.AniPrefix[bossPhase] + aniName;
			}
			else
			{
				AnimalItem animalConfig;
				bool flag4 = this.TryGetAnimalConfig(charId, out animalConfig);
				if (flag4)
				{
					finalAniName = animalConfig.AniPrefix + aniName;
				}
			}
			Spine.Animation anim = this.GetAnimation(skeleton, finalAniName);
			bool flag5 = anim == null;
			if (flag5)
			{
				throw new Exception("Animation " + finalAniName + " not exist in " + skeleton.SkeletonDataAsset.name);
			}
			TrackEntry trackEntry = skeleton.AnimationState.SetAnimation(0, anim, loop);
			bool needMix = (finalAniName.Contains("M_") || finalAniName.Contains("MR_")) && !ViewCombat.MoveSkillAniSet.Contains(finalAniName);
			trackEntry.MixDuration = (needMix ? 0.2f : 0f);
			return trackEntry;
		}

		// Token: 0x06008D4C RID: 36172 RVA: 0x0041AE84 File Offset: 0x00419084
		private Spine.Animation GetAnimation(SkeletonRenderer skeleton, string aniName)
		{
			Spine.Animation internalAnimation = skeleton.Skeleton.Data.FindAnimation(aniName);
			bool flag = internalAnimation != null;
			Spine.Animation result;
			if (flag)
			{
				result = internalAnimation;
			}
			else
			{
				RawAnimationAsset rawAnimation;
				bool flag2 = this._commonAniDict.TryGetValue(aniName, out rawAnimation) || this._skillAniDict.TryGetValue(aniName, out rawAnimation);
				if (flag2)
				{
					result = rawAnimation.GetAnimation(skeleton.skeletonDataAsset);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x06008D4D RID: 36173 RVA: 0x0041AEEC File Offset: 0x004190EC
		public Spine.Animation GetCommonAnim(IHasSkeletonDataAsset skeleton, string aniName)
		{
			RawAnimationAsset rawAnimation;
			bool flag = this._commonAniDict.TryGetValue(aniName, out rawAnimation);
			Spine.Animation result;
			if (flag)
			{
				result = rawAnimation.GetAnimation(skeleton.SkeletonDataAsset);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06008D4E RID: 36174 RVA: 0x0041AF20 File Offset: 0x00419120
		private unsafe void UpdateUsingWeaponSlot(bool isAlly)
		{
			bool flag = this.IsBoss(isAlly) || this.IsAnimal(isAlly) || (isAlly && this.Model.IsTutorialCombat);
			if (!flag)
			{
				int templateId = this.GetUsingWeaponTemplateId(isAlly);
				bool flag2 = templateId < 0;
				if (!flag2)
				{
					SkeletonAnimation skeleton = this.GetSkeleton(isAlly ? (*this._selfCurrCharId) : (*this._enemyCurrCharId));
					CombatAnimationUtils.UpdateSkeletonWeapon(skeleton, templateId);
				}
			}
		}

		// Token: 0x06008D4F RID: 36175 RVA: 0x0041AF90 File Offset: 0x00419190
		private unsafe void ClearWeaponSlot(bool isAlly)
		{
			bool flag = this.IsBoss(isAlly) || this.IsAnimal(isAlly) || (isAlly && this.Model.IsTutorialCombat);
			if (!flag)
			{
				SkeletonAnimation skeleton = this.GetSkeleton(isAlly ? (*this._selfCurrCharId) : (*this._enemyCurrCharId));
				CombatAnimationUtils.ClearSkeletonWeaponSlot(skeleton);
			}
		}

		// Token: 0x06008D50 RID: 36176 RVA: 0x0041AFEC File Offset: 0x004191EC
		private unsafe void HandleAnimationEvent(TrackEntry trackEntry, Spine.Event e)
		{
			SkeletonAnimation selfSkeleton = this.SelfCurrCharSkeleton;
			SkeletonAnimation enemySkeleton = this.EnemyCurrCharSkeleton;
			bool flag = selfSkeleton == null || enemySkeleton == null;
			if (flag)
			{
				PredefinedLog.Show(8, string.Format("HandleAnimationEvent {0} {1} {2} {3}, {4} [{5}] [{6}]", new object[]
				{
					selfSkeleton,
					enemySkeleton,
					e.Data.Name,
					*this._selfCurrCharId,
					*this._enemyCurrCharId,
					string.Join<int>(",", this._selfTeam),
					string.Join<int>(",", this._enemyTeam)
				}));
			}
			else
			{
				bool isAlly = selfSkeleton.AnimationState.GetCurrent(0) == trackEntry;
				SkeletonAnimation skeleton = isAlly ? selfSkeleton : enemySkeleton;
				Transform flawAndAcupointHolder = skeleton.transform.GetChild(0);
				bool flag2 = e.Data.Name == "step";
				if (flag2)
				{
					CombatDomainMethod.Call.PlayMoveStepSound(isAlly);
				}
				else
				{
					bool flag3 = e.Data.Name == "hide";
					if (flag3)
					{
						flawAndAcupointHolder.gameObject.SetActive(false);
					}
					else
					{
						bool flag4 = e.Data.Name == "unhide";
						if (flag4)
						{
							flawAndAcupointHolder.gameObject.SetActive(true);
						}
						else
						{
							bool flag5 = e.Data.Name == "weapon_on";
							if (flag5)
							{
								this.UpdateUsingWeaponSlot(isAlly);
							}
							else
							{
								bool flag6 = e.Data.Name == "weapon_off";
								if (flag6)
								{
									this.ClearWeaponSlot(isAlly);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06008D51 RID: 36177 RVA: 0x0041B183 File Offset: 0x00419383
		private void PlayUiParticle(ParticleSystem particle)
		{
			particle.Play(true);
			particle.gameObject.SetActive(true);
			base.StartCoroutine(this.HideParticle(particle));
		}

		// Token: 0x06008D52 RID: 36178 RVA: 0x0041B1A9 File Offset: 0x004193A9
		private IEnumerator DestroyParticle(string vfxName, ParticleSystem particle)
		{
			float timeAccumulator = 0f;
			while (timeAccumulator < particle.main.duration)
			{
				bool flag = !particle.isPaused;
				if (flag)
				{
					timeAccumulator += Time.deltaTime;
				}
				yield return null;
			}
			CombatPoolAdaptor.Destroy(vfxName, particle.gameObject, true);
			yield break;
		}

		// Token: 0x06008D53 RID: 36179 RVA: 0x0041B1C6 File Offset: 0x004193C6
		private IEnumerator HideParticle(ParticleSystem particle)
		{
			float timeAccumulator = 0f;
			while (timeAccumulator < particle.main.duration)
			{
				bool flag = !particle.isPaused;
				if (flag)
				{
					timeAccumulator += Time.deltaTime;
				}
				yield return null;
			}
			particle.Stop(true);
			particle.gameObject.SetActive(false);
			yield break;
		}

		// Token: 0x06008D54 RID: 36180 RVA: 0x0041B1DC File Offset: 0x004193DC
		private void SetSoundOnce(string audioName, int volume = 100)
		{
			bool flag = !audioName.IsNullOrEmpty();
			if (flag)
			{
				AudioClip skillAndSpecialSound;
				bool flag2 = this._skillAndSpecialSoundDict.TryGetValue(audioName, out skillAndSpecialSound);
				if (flag2)
				{
					AudioManager.Instance.PlaySound(skillAndSpecialSound, this._realTimeScale, false, volume);
				}
				else
				{
					AudioClip commonSound;
					bool flag3 = this._commonSoundDict.TryGetValue(audioName, out commonSound);
					if (flag3)
					{
						AudioManager.Instance.PlaySound(commonSound, this._realTimeScale, false, volume);
					}
					else
					{
						GLog.LogWithTag("Audio " + audioName + " not found", Array.Empty<object>());
					}
				}
			}
		}

		// Token: 0x06008D55 RID: 36181 RVA: 0x0041B268 File Offset: 0x00419468
		private void PlayEnemyEnterAttackRangeSound()
		{
			bool isInAttackRange = this.InAttackRange(true);
			bool flag = isInAttackRange;
			if (flag)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(150);
			}
			bool flag2 = !this._isInSelfAttackRange && isInAttackRange;
			if (flag2)
			{
				AudioManager.Instance.PlaySound("combat_enter_attack_range", false, false);
			}
			this._isInSelfAttackRange = isInAttackRange;
		}

		// Token: 0x06008D56 RID: 36182 RVA: 0x0041B2B8 File Offset: 0x004194B8
		private void SetLoopSound(int charId, string soundName)
		{
			bool flag = this._loopSoundDict.ContainsKey(charId);
			if (flag)
			{
				AudioManager.Instance.StopSound(this._loopSoundDict[charId]);
				this._loopSoundDict.Remove(charId);
			}
			bool flag2 = soundName.IsNullOrEmpty();
			if (!flag2)
			{
				AudioClip commonClip;
				AudioClip clip = this._commonSoundDict.TryGetValue(soundName, out commonClip) ? commonClip : this._skillAndSpecialSoundDict.GetValueOrDefault(soundName);
				bool flag3 = clip == null;
				if (!flag3)
				{
					AudioManager.Instance.PlaySound(clip, this._realTimeScale, true, 100);
					this._loopSoundDict.Add(charId, clip.name);
				}
			}
		}

		// Token: 0x06008D57 RID: 36183 RVA: 0x0041B360 File Offset: 0x00419560
		private void StopLoopSoundForCharacter(int oldCharId)
		{
			string soundName;
			bool flag = this._loopSoundDict.Remove(oldCharId, out soundName);
			if (flag)
			{
				AudioManager.Instance.StopSound(soundName);
			}
		}

		// Token: 0x06008D58 RID: 36184 RVA: 0x0041B390 File Offset: 0x00419590
		private void SetSkeletonAndVfxTimePause(bool pause)
		{
			List<SkeletonAnimation> allSkeletonList = EasyPool.Get<List<SkeletonAnimation>>();
			allSkeletonList.Clear();
			allSkeletonList.AddRange(this.selfSkeletonList);
			allSkeletonList.AddRange(this.enemySkeletonList);
			bool activeSelf = this._selfPetSkeleton.gameObject.activeSelf;
			if (activeSelf)
			{
				allSkeletonList.Add(this._selfPetSkeleton);
			}
			bool activeSelf2 = this._enemyPetSkeleton.gameObject.activeSelf;
			if (activeSelf2)
			{
				allSkeletonList.Add(this._enemyPetSkeleton);
			}
			foreach (SkeletonAnimation skeleton in allSkeletonList)
			{
				if (pause)
				{
					this._skeletonTimeScaleBeforePause[skeleton] = skeleton.timeScale;
				}
				skeleton.timeScale = (pause ? 0f : this._skeletonTimeScaleBeforePause.GetValueOrDefault(skeleton, 1f));
			}
			this.combatParticle.SetParticleTimePause(pause);
			EasyPool.Free<List<SkeletonAnimation>>(allSkeletonList);
			UI_CombatBackground.Instance.SetPause(pause);
		}

		// Token: 0x06008D59 RID: 36185 RVA: 0x0041B4A8 File Offset: 0x004196A8
		private void UpdateFlawCount(int charId, List<byte> lastCount)
		{
			byte[] flawCount = this._flawCountDict[charId];
			Transform flawAndAcupointHolder = this.GetSkeleton(charId).transform.GetChild(0);
			for (sbyte bodyPart = 0; bodyPart < 7; bodyPart += 1)
			{
				byte count = flawCount[(int)bodyPart];
				GameObject flawObj = flawAndAcupointHolder.GetChild((int)bodyPart).GetChild(0).gameObject;
				flawObj.SetActive(count > 0);
				bool flag = count > 0;
				if (flag)
				{
					TooltipInvoker mouseTip = flawObj.GetComponent<TooltipInvoker>();
					ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>();
					argumentBox.Set("Title", LocalStringManager.GetFormat(LanguageKey.LK_Combat_FlawTitle, BodyPart.Instance[bodyPart].Name));
					argumentBox.Set("Desc", LocalStringManager.GetFormat(LanguageKey.LK_Combat_FlawInfo, count.ToString()).ColorReplace());
					argumentBox.Set("PartIcon", MouseTipConstant.HitPartNamesByConfig[(int)bodyPart, 2]);
					argumentBox.Set("PartName", LocalStringManager.Get(MouseTipConstant.HitPartNamesByConfig[(int)bodyPart, 1]));
					argumentBox.Set("PartDesc", LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_CombatFlaw_Desc, string.Format("{0}%", GlobalConfig.Instance.FlawAddDamagePercent)).ColorReplace());
					mouseTip.RuntimeParam = argumentBox;
				}
				bool flag2 = count < lastCount[(int)bodyPart];
				if (flag2)
				{
					CombatUtils.PlayAndHideParticle(flawAndAcupointHolder.GetChild((int)bodyPart).GetChild(2).GetComponent<ParticleSystem>(), 2f);
				}
			}
		}

		// Token: 0x06008D5A RID: 36186 RVA: 0x0041B624 File Offset: 0x00419824
		private void UpdateAcupointCount(int charId, List<byte> lastCount)
		{
			byte[] acupointCount = this._acupointCountDict[charId];
			Transform flawAndAcupointHolder = this.GetSkeleton(charId).transform.GetChild(0);
			for (sbyte bodyPart = 0; bodyPart < 7; bodyPart += 1)
			{
				byte count = acupointCount[(int)bodyPart];
				GameObject acupointObj = flawAndAcupointHolder.GetChild((int)bodyPart).GetChild(1).gameObject;
				acupointObj.SetActive(count > 0);
				bool flag = count > 0;
				if (flag)
				{
					TooltipInvoker mouseTip = acupointObj.GetComponent<TooltipInvoker>();
					BodyPartItem partConfig = BodyPart.Instance[bodyPart];
					mouseTip.PresetParam[0] = LocalStringManager.GetFormat("LK_Combat_AcupointTitle", partConfig.Name);
					mouseTip.PresetParam[1] = LocalStringManager.GetFormat("LK_Combat_AcupointInfo", count.ToString()).ColorReplace() + "\n" + partConfig.AcupointDesc;
				}
				bool flag2 = count < lastCount[(int)bodyPart];
				if (flag2)
				{
					CombatUtils.PlayAndHideParticle(flawAndAcupointHolder.GetChild((int)bodyPart).GetChild(3).GetComponent<ParticleSystem>(), 2f);
				}
			}
		}

		// Token: 0x06008D5B RID: 36187 RVA: 0x0041B72C File Offset: 0x0041992C
		private void UpdateAttackRange(bool isAlly)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId, out processor);
			if (!flag)
			{
				OuterAndInnerShorts attackRange = processor.AttackRange;
				ItemKey preparingItem = processor.PreparingItem;
				bool flag2 = preparingItem.IsValid();
				if (flag2)
				{
					short useDistance = (short)((preparingItem.ItemType == 8) ? Medicine.Instance[preparingItem.TemplateId].MaxUseDistance : Misc.Instance[preparingItem.TemplateId].MaxUseDistance);
					bool flag3 = useDistance > 0;
					if (flag3)
					{
						attackRange = new OuterAndInnerShorts((short)this.Model.Config.MinDistance, useDistance);
					}
				}
				bool outRange = attackRange.Outer > this._distance || attackRange.Inner < this._distance;
				if (isAlly)
				{
					this.UpdateMoveTips();
				}
				SkeletonAnimation skeleton = isAlly ? this.EnemyCurrCharSkeleton : this.SelfCurrCharSkeleton;
				bool flag4 = skeleton != null;
				if (flag4)
				{
					skeleton.GetComponent<CombatSpineSkeleton>().currPosMark.SetSprite(this._currPosSprite[outRange ? 2 : (isAlly ? 1 : 0)], false, null);
				}
				int weaponIndex = processor.UsingWeaponIndex;
				bool flag5 = weaponIndex < 0;
				if (!flag5)
				{
					RectTransform weaponHolder = isAlly ? this._selfWeaponHolder : this._enemyWeaponHolder;
					CombatWeaponPrefab weaponRefers = weaponHolder.GetChild(weaponIndex).GetComponent<CombatWeaponPrefab>();
					GameObject outRangeTips = weaponRefers.outAttackRange;
					bool flag6 = outRangeTips.activeSelf != outRange;
					if (flag6)
					{
						outRangeTips.SetActive(outRange);
					}
				}
			}
		}

		// Token: 0x06008D5C RID: 36188 RVA: 0x0041B8C4 File Offset: 0x00419AC4
		private void ChangeTargetDistance(int delta, bool isFirst)
		{
			short newTargetDistance = (short)Mathf.Clamp((int)((this._selfTargetDistance < 0) ? this._distance : this._selfTargetDistance) + delta, 20, 120);
			bool flag = newTargetDistance == this._selfTargetDistance;
			if (!flag)
			{
				if (isFirst)
				{
					this._setTargetDistanceTimes = 0;
					this._lastSetTargetDistanceDelta = this.setTargetDistanceFixedDelta;
					this._lastSetTargetDistanceTime = Time.unscaledTimeAsDouble + this.setTargetDistanceFirstDelta;
				}
				else
				{
					this._setTargetDistanceTimes++;
					this._lastSetTargetDistanceDelta = Math.Max(this._lastSetTargetDistanceDelta * this.setTargetDistanceDamping, this.setTargetDistanceFixedDeltaMin);
					this._lastSetTargetDistanceTime = Time.unscaledTimeAsDouble;
				}
				bool flag2 = this._setTargetDistanceTimes % this.setTargetDistanceDeltaPerTimes == 0;
				if (flag2)
				{
					this._lastSetTargetDistanceDelta = this.setTargetDistanceFixedDelta;
				}
				CombatDomainMethod.Call.SetTargetDistance(newTargetDistance);
			}
		}

		// Token: 0x06008D5D RID: 36189 RVA: 0x0041B996 File Offset: 0x00419B96
		private void RequestClearTargetDistance()
		{
			GameDataBridge.AddMethodCall(-1, 8, 79);
		}

		// Token: 0x06008D5E RID: 36190 RVA: 0x0041B9A3 File Offset: 0x00419BA3
		private void UpdateTargetDistanceBar()
		{
			this.targetDistanceBar.RefreshMark();
			this.targetDistanceBar.RefreshTarget(this._selfTargetDistance, this._enemyTargetDistance);
		}

		// Token: 0x06008D5F RID: 36191 RVA: 0x0041B9CC File Offset: 0x00419BCC
		private void UpdateTargetDistanceInteract()
		{
			bool inGuiding = SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
			if (inGuiding)
			{
				this.targetDistanceBar.SetInteract(false);
			}
			else
			{
				bool canInteract = !this._autoCombat || !this.AiOptions.AutoMove;
				canInteract = (canInteract && this.Model.CanOperateSelfCharacter);
				this.targetDistanceBar.SetInteract(canInteract);
			}
		}

		// Token: 0x06008D60 RID: 36192 RVA: 0x0041BA34 File Offset: 0x00419C34
		private void UpdateAttackRangeBar()
		{
			bool flag = this.SelfCurrCharSkeleton == null || this.EnemyCurrCharSkeleton == null;
			if (!flag)
			{
				bool flag2 = this.Model.SelfCharacter == null;
				if (!flag2)
				{
					bool flag3 = this.Model.EnemyCharacter == null;
					if (!flag3)
					{
						OuterAndInnerShorts selfAttackRange = this.Model.SelfCharacter.AttackRange;
						OuterAndInnerShorts enemyAttackRange = this.Model.EnemyCharacter.AttackRange;
						ItemKey selfPreparingItem = this.Model.SelfCharacter.PreparingItem;
						ItemKey enemyPreparingItem = this.Model.EnemyCharacter.PreparingItem;
						bool flag4 = selfPreparingItem.IsValid();
						if (flag4)
						{
							short useDistance = (short)((selfPreparingItem.ItemType == 8) ? Medicine.Instance[selfPreparingItem.TemplateId].MaxUseDistance : Misc.Instance[selfPreparingItem.TemplateId].MaxUseDistance);
							bool flag5 = useDistance > 0;
							if (flag5)
							{
								selfAttackRange = new OuterAndInnerShorts((short)this.Model.Config.MinDistance, useDistance);
							}
						}
						bool flag6 = enemyPreparingItem.IsValid();
						if (flag6)
						{
							short useDistance2 = (short)((enemyPreparingItem.ItemType == 8) ? Medicine.Instance[enemyPreparingItem.TemplateId].MaxUseDistance : Misc.Instance[enemyPreparingItem.TemplateId].MaxUseDistance);
							bool flag7 = useDistance2 > 0;
							if (flag7)
							{
								enemyAttackRange = new OuterAndInnerShorts((short)this.Model.Config.MinDistance, useDistance2);
							}
						}
						this.UpdateRange(selfAttackRange, CombatRangeText.EType.Self);
						this.UpdateRange(enemyAttackRange, CombatRangeText.EType.Enemy);
					}
				}
			}
		}

		// Token: 0x06008D61 RID: 36193 RVA: 0x0041BBC8 File Offset: 0x00419DC8
		private void UpdateAttackRangePreview()
		{
			bool flag = this._previewRangeSkill >= 0 && this._previewRangeSkillCharId == this.Model.SelfCharId;
			if (flag)
			{
				List<short> skillList;
				bool flag2 = !this.Model.OrderedProactiveSkillList.TryGetValue(this.Model.SelfCharId, out skillList) || !skillList.Contains(this._previewRangeSkill);
				if (!flag2)
				{
					bool flag3 = this._previewRangeSkillCharId != this.Model.SelfCharId;
					if (!flag3)
					{
						CombatDomainMethod.AsyncCall.GetPreviewAttackRange(this, this._previewRangeSkillCharId, this._previewRangeSkill, new AsyncMethodCallbackDelegate(this.HandlerSkillRangePreview));
					}
				}
			}
			else
			{
				bool flag4 = this._previewRangeWeapon.IsValid();
				if (flag4)
				{
					int index;
					bool flag6;
					bool flag5 = !this.TryGetWeaponIndexAndIsAllyByItemKey(this._previewRangeWeapon, out index, out flag6);
					if (!flag5)
					{
						bool flag7 = this._previewRangeWeaponCharId != this.Model.SelfCharId;
						if (!flag7)
						{
							CombatDomainMethod.AsyncCall.GetPreviewAttackRange(this, this._previewRangeWeaponCharId, -1, index, new AsyncMethodCallbackDelegate(this.HandlerWeaponRangePreview));
						}
					}
				}
				else
				{
					bool flag8 = this._previewRangeItem.Inner >= 0;
					if (flag8)
					{
						this.UpdateAttackRangePreview(this._previewRangeItem);
					}
					else
					{
						this.UpdateAttackRangePreview(new OuterAndInnerShorts(-1, -1));
					}
				}
			}
		}

		// Token: 0x06008D62 RID: 36194 RVA: 0x0041BD16 File Offset: 0x00419F16
		private void UpdatePreviewRangeItem(OuterAndInnerShorts range)
		{
			this._previewRangeItem = range;
			this.UpdateAttackRangePreview(range);
		}

		// Token: 0x06008D63 RID: 36195 RVA: 0x0041BD28 File Offset: 0x00419F28
		public void UpdatePreviewRangeWeapon(int index)
		{
			this._previewRangeWeapon = this._selfWeaponList[index];
			this._previewRangeWeaponCharId = this.Model.SelfCharId;
			this.UpdateAttackRangePreview();
		}

		// Token: 0x06008D64 RID: 36196 RVA: 0x0041BD55 File Offset: 0x00419F55
		public void ClearPreviewRangeWeapon()
		{
			this._previewRangeWeapon = ItemKey.Invalid;
			this._previewRangeWeaponCharId = -1;
			this.UpdateAttackRangePreview();
		}

		// Token: 0x06008D65 RID: 36197 RVA: 0x0041BD74 File Offset: 0x00419F74
		private void HandlerSkillRangePreview(int offset, RawDataPool pool)
		{
			OuterAndInnerShorts range = default(OuterAndInnerShorts);
			Serializer.Deserialize(pool, offset, ref range);
			this.UpdateAttackRangePreview((this._previewRangeSkill < 0) ? new OuterAndInnerShorts(-1, -1) : range);
		}

		// Token: 0x06008D66 RID: 36198 RVA: 0x0041BDB0 File Offset: 0x00419FB0
		private void HandlerWeaponRangePreview(int offset, RawDataPool pool)
		{
			OuterAndInnerShorts attackRange = new OuterAndInnerShorts(-1, -1);
			Serializer.Deserialize(pool, offset, ref attackRange);
			this.UpdateAttackRangePreview(attackRange);
		}

		// Token: 0x06008D67 RID: 36199 RVA: 0x0041BDDC File Offset: 0x00419FDC
		private void UpdateAttackRangePreview(OuterAndInnerShorts attackRange)
		{
			RectTransform previewRange = this.attackRangeBar.previewRange;
			previewRange.gameObject.SetActive(attackRange.Inner >= 0);
			bool flag = attackRange.Inner >= 0;
			if (flag)
			{
				this.UpdateRange(attackRange, CombatRangeText.EType.Preview);
			}
		}

		// Token: 0x06008D68 RID: 36200 RVA: 0x0041BE28 File Offset: 0x0041A028
		private void UpdateRange(OuterAndInnerShorts range, CombatRangeText.EType rangeType)
		{
			range = CombatRangeText.AttackRangeClamp(range);
			float scale = this._virtualCamera.GetScale();
			int correctionFactor = (rangeType == CombatRangeText.EType.Enemy) ? -1 : 1;
			float rangePos = ((float)(range.Outer * 5) * scale - 65f) * (float)correctionFactor;
			float rangeWidth = (float)((range.Inner - range.Outer) * 5) * scale + 65f;
			bool inRange = range.Outer <= this._distance && this._distance <= range.Inner;
			if (!true)
			{
			}
			RectTransform rectTransform;
			switch (rangeType)
			{
			case CombatRangeText.EType.Self:
				rectTransform = this.attackRangeBar.selfRange;
				break;
			case CombatRangeText.EType.Enemy:
				rectTransform = this.attackRangeBar.enemyRange;
				break;
			case CombatRangeText.EType.Preview:
				rectTransform = this.attackRangeBar.previewRange;
				break;
			default:
				throw new ArgumentOutOfRangeException("rangeType", rangeType, null);
			}
			if (!true)
			{
			}
			RectTransform rangeTransform = rectTransform;
			rangeTransform.anchoredPosition = rangeTransform.anchoredPosition.SetX(rangePos);
			rangeTransform.SetWidth(rangeWidth);
			CImage component = rangeTransform.GetComponent<CImage>();
			if (!true)
			{
			}
			string spriteName;
			switch (rangeType)
			{
			case CombatRangeText.EType.Self:
				spriteName = (inRange ? "combat_jiaodi_chibiao_0_1" : "combat_jiaodi_chibiao_0_4");
				break;
			case CombatRangeText.EType.Enemy:
				spriteName = (inRange ? "combat_jiaodi_chibiao_0_0" : "combat_jiaodi_chibiao_0_3");
				break;
			case CombatRangeText.EType.Preview:
				spriteName = (inRange ? "combat_jiaodi_chibiao_0_2" : "combat_jiaodi_chibiao_0_5");
				break;
			default:
				throw new ArgumentOutOfRangeException("rangeType", rangeType, null);
			}
			if (!true)
			{
			}
			component.SetSprite(spriteName, false, null);
			if (!true)
			{
			}
			CombatRangeText combatRangeText;
			switch (rangeType)
			{
			case CombatRangeText.EType.Self:
				combatRangeText = this.attackRangeBar.selfRangeCanvas;
				break;
			case CombatRangeText.EType.Enemy:
				combatRangeText = this.attackRangeBar.enemyRangeCanvas;
				break;
			case CombatRangeText.EType.Preview:
				combatRangeText = this.attackRangeBar.previewRangeCanvas;
				break;
			default:
				throw new ArgumentOutOfRangeException("rangeType", rangeType, null);
			}
			if (!true)
			{
			}
			CombatRangeText rangeText = combatRangeText;
			rangeText.Refresh(rangeType, range, inRange);
			this.targetDistanceBar.RefreshRange(rangeType, range);
		}

		// Token: 0x06008D69 RID: 36201 RVA: 0x0041C01C File Offset: 0x0041A21C
		private void UpdateDefendBounceRange()
		{
			for (int i = 0; i < this._defendBounceRangeList.Length; i++)
			{
				short bounceDistance = this._defendBounceRangeList[i];
				int bounceChar = this._defendBounceCharList.GetOrDefault(i);
				CombatSubProcessorCharacterDisplay characterProcessor;
				bool bounceCharVisible = !this.Model.TryGetCharacterDisplayProcessor(bounceChar, out characterProcessor) || characterProcessor.Visible;
				bool hasDefendSkill = bounceDistance > 0 && bounceCharVisible;
				CRawImage rangeImg = this.attackRangeBar.GetSkillRangeRawImage(i);
				bool flag = rangeImg.gameObject.activeSelf != hasDefendSkill;
				if (flag)
				{
					rangeImg.DOKill(false);
					bool flag2 = hasDefendSkill;
					if (flag2)
					{
						rangeImg.color = Color.white;
						rangeImg.gameObject.SetActive(true);
					}
					else
					{
						rangeImg.DOFade(0f, 0.2f).OnComplete(delegate
						{
							rangeImg.gameObject.SetActive(false);
						});
					}
				}
				bool flag3 = hasDefendSkill;
				if (flag3)
				{
					bool isAlly = i < 2;
					bool inRange = this._distance <= bounceDistance;
					RectTransform imgTransform = rangeImg.rectTransform;
					float barWidth = this.attackRangeBar.GetComponent<RectTransform>().rect.width;
					float scale = this._virtualCamera.GetScale();
					float imgPos = isAlly ? ((float)(100 + (bounceDistance - 20) * 5) * scale + barWidth / 2f) : ((float)(-100 - (bounceDistance - 20) * 5) * scale + barWidth / 2f);
					rangeImg.texture = (inRange ? this.attackRangeBar.defendInRange : this.attackRangeBar.defendOutRange);
					imgTransform.anchoredPosition = imgTransform.anchoredPosition.SetX(imgPos + (float)(26 * (isAlly ? 1 : -1)));
				}
			}
		}

		// Token: 0x06008D6A RID: 36202 RVA: 0x0041C1F8 File Offset: 0x0041A3F8
		private void UpdateAffectingDefendSkill(CombatAffectingDefendSkill defendSkillRefers, int charId, bool isAlly, short skillId)
		{
			CombatSkillItem skillConfig = CombatSkill.Instance[skillId];
			BossItem bossConfig;
			this.TryGetBossConfig(charId, out bossConfig);
			sbyte b;
			if (!isAlly)
			{
				CombatSubProcessorCharacter enemyCharacter = this.Model.EnemyCharacter;
				b = ((enemyCharacter != null) ? enemyCharacter.BossPhase : 0);
			}
			else
			{
				CombatSubProcessorCharacter selfCharacter = this.Model.SelfCharacter;
				b = ((selfCharacter != null) ? selfCharacter.BossPhase : 0);
			}
			sbyte bossPhase = b;
			TooltipInvoker mouseTip = defendSkillRefers.tipDisplayer;
			defendSkillRefers.UserInt = charId;
			CombatUtils.SetCombatSkillTips(mouseTip, charId, skillId);
			defendSkillRefers.skillType.SetSprite(skillConfig.Icon, false, null);
			defendSkillRefers.skillType.SetColor(Colors.Instance.FiveElementsColors[(int)skillConfig.FiveElements]);
			defendSkillRefers.progressBar.sizeDelta = new Vector2(244f, 36f);
			this.UpdateDefendSkillRangeText();
			this.DestroyDefendSkillParticleAndSound(charId);
			string particleName = skillConfig.DefendParticle;
			bool flag = !particleName.IsNullOrEmpty();
			if (flag)
			{
				bool flag2 = bossConfig != null;
				if (flag2)
				{
					particleName = bossConfig.DefendSkillParticlePrefix[(int)bossPhase] + particleName;
				}
				ParticleSystem particle = this.combatParticle.PlayDefendVfx(charId, this._selfTeam.Contains(charId), particleName);
				bool flag3 = particle != null;
				if (flag3)
				{
					this._defendParticleDict.Add(charId, particle);
				}
			}
			string soundName = skillConfig.DefendSound;
			bool flag4 = !soundName.IsNullOrEmpty();
			if (flag4)
			{
				bool flag5 = bossConfig != null;
				if (flag5)
				{
					soundName = bossConfig.DefendSkillSoundPrefix[(int)bossPhase] + soundName;
				}
				AudioClip clip;
				bool flag6 = this._skillAndSpecialSoundDict.TryGetValue(soundName, out clip);
				if (flag6)
				{
					AudioManager.Instance.PlaySound(clip, this._realTimeScale, true, 100);
					this._defendSoundDict.Add(charId, clip.name);
				}
			}
			defendSkillRefers.gameObject.SetActive(true);
			int rangeIndex = isAlly ? ((charId == this._selfTeam[0]) ? 0 : 1) : ((charId == this._enemyTeam[0]) ? 2 : 3);
			this._defendBounceRangeList[rangeIndex] = skillConfig.BounceDistance;
			this._defendBounceCharList[rangeIndex] = charId;
			this.UpdateDefendBounceRange();
		}

		// Token: 0x06008D6B RID: 36203 RVA: 0x0041C40E File Offset: 0x0041A60E
		private void UpdateDefendSkillRangeText()
		{
		}

		// Token: 0x06008D6C RID: 36204 RVA: 0x0041C414 File Offset: 0x0041A614
		private void DestroyDefendSkillParticleAndSound(int charId)
		{
			bool flag = this._defendParticleDict.ContainsKey(charId);
			if (flag)
			{
				Object.Destroy(this._defendParticleDict[charId].gameObject);
				this._defendParticleDict.Remove(charId);
			}
			bool flag2 = this._defendSoundDict.ContainsKey(charId);
			if (flag2)
			{
				AudioManager.Instance.StopSound(this._defendSoundDict[charId]);
				this._defendSoundDict.Remove(charId);
			}
			int defendRangeIndex = this._selfTeam.Contains(charId) ? ((charId == this._selfTeam[0]) ? 0 : 1) : ((charId == this._enemyTeam[0]) ? 2 : 3);
			this._defendBounceRangeList[defendRangeIndex] = -1;
			this._defendBounceCharList[defendRangeIndex] = -1;
			this.UpdateDefendBounceRange();
		}

		// Token: 0x06008D6D RID: 36205 RVA: 0x0041C4E0 File Offset: 0x0041A6E0
		private void UpdateMobilityLock(bool isAlly)
		{
			short lockEffectCount = isAlly ? this._selfMobilityLockEffectCount : this._enemyMobilityLockEffectCount;
			CombatInfoChar infoChar = isAlly ? this.selfInfoChar : this.enemyInfoChar;
			infoChar.mobilityLock.SetActive(lockEffectCount > 0);
		}

		// Token: 0x06008D6E RID: 36206 RVA: 0x0041C524 File Offset: 0x0041A724
		private void UpdateMobilityBar(bool isAlly)
		{
			short affectingMoveSkill = isAlly ? this._selfAffectingMoveSkillId : this._enemyAffectingMoveSkillId;
			byte mobilityLevel = isAlly ? this._selfMobilityLevel : this._enemyMobilityLevel;
			CombatInfoChar infoChar = isAlly ? this.selfInfoChar : this.enemyInfoChar;
			CImage mobilityBar = infoChar.mobilityBar;
			mobilityBar.SetSprite((affectingMoveSkill >= 0) ? "combat_jiaodi_tiao_8" : this._mobilityLevelSprite[(int)mobilityLevel], false, null);
		}

		// Token: 0x06008D6F RID: 36207 RVA: 0x0041C58C File Offset: 0x0041A78C
		private unsafe void UpdateMobilityTips(bool isAlly)
		{
			int mobility = isAlly ? this._selfMobility : this._enemyMobility;
			int charId = isAlly ? (*this._selfCurrCharId) : (*this._enemyCurrCharId);
			string moveCdStr = this.GetMoveCdText(isAlly);
			string recoverStr = this.GetMobilityRecoverText(isAlly);
			CombatSubProcessorCharacter orDefault = this.Model.ProcessorCharacters.GetOrDefault(charId);
			short moveSpeed = (orDefault != null) ? orDefault.MoveSpeed : 0;
			CombatInfoChar infoChar = isAlly ? this.selfInfoChar : this.enemyInfoChar;
			TooltipInvoker mobilityMouseTip = infoChar.mobilityBar.GetComponent<TooltipInvoker>();
			string mobilityStr = string.Format("{0}%", mobility * 100 / MoveSpecialConstants.MaxMobility).SetColor("pinkyellow");
			mobilityMouseTip.PresetParam[1] = mobilityStr + "\n" + LocalStringManager.GetFormat(LanguageKey.LK_Combat_Mobility_Tips, moveSpeed, moveCdStr) + recoverStr;
			mobilityMouseTip.Refresh(false, 109);
		}

		// Token: 0x06008D70 RID: 36208 RVA: 0x0041C668 File Offset: 0x0041A868
		private string GetMoveCdText(bool isAlly)
		{
			short moveCd = isAlly ? this._selfMoveCd : this._enemyMoveCd;
			int maxDigit = 100;
			float moveCdFixedFloat = (float)((int)moveCd * maxDigit) / 60f;
			int moveCdFixed = Mathf.RoundToInt(moveCdFixedFloat);
			bool flag = moveCdFixed % 100 == 0;
			string result;
			if (flag)
			{
				result = (moveCdFixed / 100).ToString();
			}
			else
			{
				result = ((float)moveCdFixed / 100f).ToString((moveCdFixed % 10 == 0) ? "F1" : "F2");
			}
			return result;
		}

		// Token: 0x06008D71 RID: 36209 RVA: 0x0041C6E4 File Offset: 0x0041A8E4
		private string GetMobilityRecoverText(bool isAlly)
		{
			int mobility = isAlly ? this._selfMobility : this._enemyMobility;
			bool flag = mobility >= MoveSpecialConstants.MaxMobility;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				short skillId = isAlly ? this._selfAffectingMoveSkillId : this._enemyAffectingMoveSkillId;
				bool flag2 = skillId >= 0;
				if (flag2)
				{
					result = string.Empty;
				}
				else
				{
					int recoverSpeed = isAlly ? this._selfMobilityRecoverSpeed : this._enemyMobilityRecoverSpeed;
					bool flag3 = recoverSpeed <= 0;
					if (flag3)
					{
						result = string.Empty;
					}
					else
					{
						float remain = (float)(MoveSpecialConstants.MaxMobility - mobility) / (float)recoverSpeed / 60f;
						string cdText = CombatUtils.StyleCountDownText(remain, false);
						result = LocalStringManager.GetFormat(LanguageKey.LK_Combat_Mobility_Tips_Locking, cdText);
					}
				}
			}
			return result;
		}

		// Token: 0x06008D72 RID: 36210 RVA: 0x0041C79C File Offset: 0x0041A99C
		private unsafe void UpdateSkillBreaker(short skillId, GameObject combatSkillBreaker)
		{
			CombatProactiveSkillView moveSkillView = (skillId < 0) ? null : this.combatSkillScroll.GetProactiveSkillView(skillId);
			bool flag = moveSkillView != null;
			if (flag)
			{
				combatSkillBreaker.transform.SetParent(moveSkillView.transform);
				combatSkillBreaker.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
				combatSkillBreaker.gameObject.SetActive(true);
				combatSkillBreaker.GetComponent<PointerTrigger>().enabled = this.Model.CanOperateSelfCharacter;
				CombatUtils.SetCombatSkillTips(combatSkillBreaker.GetComponent<TooltipInvoker>(), *this._selfCurrCharId, skillId);
			}
			else
			{
				combatSkillBreaker.transform.SetParent(base.transform);
				combatSkillBreaker.gameObject.SetActive(false);
			}
		}

		// Token: 0x06008D73 RID: 36211 RVA: 0x0041C84C File Offset: 0x0041AA4C
		private void UpdateJumpPrepare(bool isAlly)
		{
			sbyte progress = isAlly ? this._selfJumpPrepareProgress : this._enemyJumpPrepareProgress;
			short distance = (short)(isAlly ? this._selfJumpPreparedDistance : this._enemyJumpPreparedDistance);
			short skillId = isAlly ? this._selfAffectingMoveSkillId : this._enemyAffectingMoveSkillId;
			CombatInfoChar infoChar = isAlly ? this.selfInfoChar : this.enemyInfoChar;
			CombatMovePrepareProgress progressRefers = infoChar.movePrepareProgress;
			bool flag = progress > 0 || distance > 0;
			if (flag)
			{
				CombatSkillItem skillConfig = (skillId < 0) ? null : CombatSkill.Instance[skillId];
				string sprite = (skillConfig == null) ? "combat_xuanfu_icon_move_0" : skillConfig.Icon;
				Color color = (skillConfig == null) ? Color.white : Colors.Instance.FiveElementsColors[(int)skillConfig.FiveElements];
				progressRefers.skillType.SetSprite(sprite, false, null);
				progressRefers.skillType.SetColor(color);
				progressRefers.tips.text = LocalStringManager.Get(LanguageKey.LK_Combat_Jump_Move_Tips);
				progressRefers.progressBar.fillAmount = (float)progress / 100f;
				progressRefers.actionNum.text = ((int)(distance / 10)).ToString();
				progressRefers.gameObject.SetActive(true);
			}
			else
			{
				progressRefers.gameObject.SetActive(false);
			}
		}

		// Token: 0x06008D74 RID: 36212 RVA: 0x0041C990 File Offset: 0x0041AB90
		private void UpdateOtherActionPrepare(bool isAlly)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				byte progress = processor.OtherActionPreparePercent;
				sbyte actionType = processor.PreparingOtherAction;
				CombatInfoChar infoChar = isAlly ? this.selfInfoChar : this.enemyInfoChar;
				CombatPrepareProgress progressRefers = infoChar.prepareProgress;
				bool flag2 = actionType >= 0 && actionType != 4;
				if (flag2)
				{
					string tips = LocalStringManager.Get((actionType == 0) ? LanguageKey.LK_HotKeyGroup_Combat_Heal_Injury : ((actionType == 1) ? LanguageKey.LK_HotKeyGroup_Combat_Heal_Poison : ((actionType == 2) ? LanguageKey.LK_HotKeyGroup_Combat_Flee : LanguageKey.LK_Combat_Carrier_Attack)));
					string sprite = (actionType == 0) ? "combat_xuanfu_icon_0" : ((actionType == 1) ? "combat_xuanfu_icon_1" : ((actionType == 2) ? "combat_xuanfu_icon_2" : "combat_bottom_icon_zhaohuanyeshou_0"));
					progressRefers.actionIcon.rectTransform.localScale = Vector3.one * ((actionType == 3) ? 0.9f : 1f);
					this.ShowCharPrepareProgress(progressRefers, tips, (float)progress / 100f, ItemKey.Invalid, sprite, "");
				}
				else
				{
					bool flag3 = actionType == 4;
					if (flag3)
					{
						CombatDomainMethod.Call.InterruptOtherActionManual();
						SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
						{
							DialogCmd cmd = new DialogCmd
							{
								Title = (this.Model.IsPuppetCombat ? LocalStringManager.Get(LanguageKey.LK_Combat_Puppet_SurrenderTitle) : LocalStringManager.Get(LanguageKey.LK_Combat_SurrenderTitle)),
								Content = (this.Model.IsPuppetCombat ? LocalStringManager.Get(LanguageKey.LK_Combat_Puppet_SurrenderInfo) : LocalStringManager.Get(LanguageKey.LK_Combat_SurrenderConfirm).ColorReplace()),
								Yes = delegate()
								{
									CombatDomainMethod.Call.Surrender();
									this.OnShowConfirmDialog(false);
								},
								No = delegate()
								{
									this.OnShowConfirmDialog(false);
								}
							};
							UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
							UIManager.Instance.MaskUI(UIElement.Dialog);
							this.OnShowConfirmDialog(true);
						});
					}
					else
					{
						progressRefers.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x06008D75 RID: 36213 RVA: 0x0041CAFC File Offset: 0x0041ACFC
		private void UpdateCombatCircleReserveStatus()
		{
			this.combatQuickUseItemPanel.circlePanelReserve.gameObject.SetActive(false);
			bool flag = this._selfReserveData.NeedUseOtherAction != -1;
			if (flag)
			{
				CombatOtherActionHolder holder = this.combatQuickUseItemPanel.otherActionHolder;
				sbyte needUseOtherAction = this._selfReserveData.NeedUseOtherAction;
				if (!true)
				{
				}
				RectTransform component;
				switch (needUseOtherAction)
				{
				case 0:
					component = holder.otherActionTypeList[0].GetComponent<RectTransform>();
					break;
				case 1:
					component = holder.otherActionTypeList[1].GetComponent<RectTransform>();
					break;
				case 2:
					component = holder.otherActionTypeList[2].GetComponent<RectTransform>();
					break;
				case 3:
					component = holder.otherActionTypeList[3].GetComponent<RectTransform>();
					break;
				case 4:
					component = holder.surrender.GetComponent<RectTransform>();
					break;
				default:
					throw new Exception(string.Format("UICombat.UpdateReserveStatus() other action type out of definition range {0}", this._selfReserveData.NeedUseOtherAction));
				}
				if (!true)
				{
				}
				RectTransform reserveRoot = component;
				this.combatQuickUseItemPanel.circlePanelReserve.Refresh(CombatCirclePanelReserve.EReserveType.OtherAction, reserveRoot);
				this.combatQuickUseItemPanel.circlePanelReserve.gameObject.SetActive(true);
			}
			else
			{
				bool flag2 = this._selfReserveData.NeedUseItem.IsValid();
				if (flag2)
				{
					CombatQuickUseItemSlot slot = this.combatQuickUseItemPanel.GetQuickUseItemSlot(this._selfReserveData.NeedUseItem);
					bool flag3 = slot != null;
					if (flag3)
					{
						this.combatQuickUseItemPanel.circlePanelReserve.Refresh(CombatCirclePanelReserve.EReserveType.UseItem, slot.GetComponent<RectTransform>());
					}
					else
					{
						CombatOtherActionHolder holder2 = this.combatQuickUseItemPanel.otherActionHolder;
						this.combatQuickUseItemPanel.circlePanelReserve.Refresh(CombatCirclePanelReserve.EReserveType.OtherAction, holder2.useItem.GetComponent<RectTransform>());
					}
					this.combatQuickUseItemPanel.circlePanelReserve.gameObject.SetActive(true);
				}
				else
				{
					this.combatQuickUseItemPanel.circlePanelReserve.Cancel();
					this.combatQuickUseItemPanel.circlePanelReserve.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06008D76 RID: 36214 RVA: 0x0041CCFC File Offset: 0x0041AEFC
		private void UpdateReserveStatus()
		{
			CombatSubProcessorSkill skill;
			bool hasCanUseData = this.Model.ProcessorSkills.TryGetValue(new ValueTuple<int, short>(this.Model.SelfCharId, this._mouseOverSkill), out skill);
			bool flag = this._mouseOverSkill >= 0 && this.Model.CanOperateSelfCharacter && hasCanUseData;
			if (flag)
			{
				this.ShowSkillPreview(CombatSkill.Instance[this._mouseOverSkill], skill.CanUse);
			}
			else
			{
				bool flag2 = this._selfReserveData.NeedUseSkillId >= 0;
				if (flag2)
				{
					this.ShowSkillPreview(CombatSkill.Instance[this._selfReserveData.NeedUseSkillId], true);
				}
				else
				{
					this.HideSkillPreview();
				}
			}
			this.reserveTips.gameObject.SetActive(this._selfReserveData.AnyReserve);
			bool flag3 = !this.reserveTips.gameObject.activeSelf;
			if (!flag3)
			{
				bool flag4 = this._selfReserveData.NeedUseOtherAction != -1;
				if (flag4)
				{
					this.reserveTips.Refresh(CombatReserveController.EReserveType.OtherAction, this.otherActionReserveRoot);
				}
				else
				{
					bool flag5 = this._selfReserveData.NeedUseItem.IsValid();
					if (flag5)
					{
						this.<UpdateReserveStatus>g__ShowUseItem|504_0();
					}
					else
					{
						bool flag6 = this._selfReserveData.NeedChangeWeaponIndex >= 0;
						if (flag6)
						{
							RectTransform holder = this.selfInfoBottom.weaponHolder;
							RectTransform reserveRoot = holder.GetChild(this._selfReserveData.NeedChangeWeaponIndex).GetComponent<RectTransform>();
							CombatReserveController.EReserveType type = (this._selfReserveData.NeedChangeWeaponIndex < 3) ? CombatReserveController.EReserveType.ChangeWeapon : CombatReserveController.EReserveType.ChangeWeaponDefault;
							this.reserveTips.Refresh(type, reserveRoot);
						}
						else
						{
							bool needShowChangeTrick = this._selfReserveData.NeedShowChangeTrick;
							if (needShowChangeTrick)
							{
								RectTransform reserveRoot2 = this.selfInfoBottom.weaponTrick.GetChangeTrick();
								this.reserveTips.Refresh(CombatReserveController.EReserveType.ShowChangeTrick, reserveRoot2);
							}
							else
							{
								bool flag7 = this._selfReserveData.NeedUseSkillId >= 0;
								if (flag7)
								{
									Tester.Assert(CombatSkill.Instance[this._selfReserveData.NeedUseSkillId].EquipType != 4, "");
									CombatProactiveSkillView refers = this.combatSkillScroll.GetProactiveSkillView(this._selfReserveData.NeedUseSkillId);
									bool flag8 = refers != null;
									if (flag8)
									{
										this.reserveTips.Refresh(CombatReserveController.EReserveType.UseSkill, refers.GetComponent<RectTransform>());
									}
									else
									{
										this.<UpdateReserveStatus>g__ShowUseItem|504_0();
									}
								}
								else
								{
									bool flag9 = this._selfReserveData.NeedUnlockWeaponIndex >= 0;
									if (flag9)
									{
										RectTransform holder2 = this.selfInfoBottom.weaponHolder;
										CombatWeaponPrefab weaponRefers = holder2.GetChild(this._selfReserveData.NeedUnlockWeaponIndex).GetComponent<CombatWeaponPrefab>();
										CombatWeaponUnlockHolderPrefab unlockHolder = weaponRefers.unlockHolder;
										RectTransform unlockBtnRectTransform = unlockHolder.unlockBtn.GetComponent<RectTransform>();
										this.reserveTips.Refresh(CombatReserveController.EReserveType.UnlockWeapon, unlockBtnRectTransform);
									}
									else
									{
										bool flag10 = this._selfReserveData.Type == ECombatReserveType.TeammateCommand;
										if (flag10)
										{
											int teammateIdIndex = -1;
											for (int i = 0; i < this.Model.SelfTeam.Count; i++)
											{
												bool flag11 = this._selfReserveData.TeammateCharId == this.Model.SelfTeam[i];
												if (flag11)
												{
													teammateIdIndex = i;
													break;
												}
											}
											bool flag12 = teammateIdIndex == -1;
											if (flag12)
											{
												AdaptableLog.Warning(string.Format("Reserve TeammateCommand, teammateCharId cannot find,teammateCharId:{0} ", this._selfReserveData.TeammateCharId), false);
											}
											else
											{
												teammateIdIndex--;
												RectTransform selfTeammateHolder = this.selfInfoTop.teammateHolder;
												Transform selfTeammate = selfTeammateHolder.GetChild(teammateIdIndex);
												CombatTeammate combatTeammate = selfTeammate.GetComponent<CombatTeammate>();
												RectTransform commandHolder = combatTeammate.GetCommandHolder();
												RectTransform command = commandHolder.GetChild(this._selfReserveData.TeammateCmdIndex).GetComponent<RectTransform>();
												this.reserveTips.Refresh(CombatReserveController.EReserveType.TeammateCommand, command);
											}
										}
										else
										{
											AdaptableLog.Warning("Analysis failure. Reserve controller will be set active false", false);
											this.reserveTips.gameObject.SetActive(false);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06008D77 RID: 36215 RVA: 0x0041D0F4 File Offset: 0x0041B2F4
		private void UpdateSelfReserveNormalAttack(bool isAlly)
		{
			if (isAlly)
			{
				CombatSubProcessorCharacter processor;
				bool flag = !this.Model.ProcessorCharacters.TryGetValue(this.Model.SelfCharId, out processor);
				if (!flag)
				{
					GameObject reserveNormalAttack = this.selfInfoChar.reserveNormalAttack;
					reserveNormalAttack.gameObject.SetActive(processor.ReserveNormalAttack);
				}
			}
		}

		// Token: 0x06008D78 RID: 36216 RVA: 0x0041D150 File Offset: 0x0041B350
		private void ShowCharPrepareProgress(CombatPrepareProgress progressRefers, string tips, float progress, ItemKey itemKey, string actionSprite = "", string num = "")
		{
			CImage actionIcon = progressRefers.actionIcon;
			CImage itemBack = progressRefers.item;
			progressRefers.tips.text = tips;
			progressRefers.progressBar.fillAmount = progress;
			actionIcon.gameObject.SetActive(!itemKey.IsValid());
			itemBack.gameObject.SetActive(itemKey.IsValid());
			bool flag = !itemKey.IsValid();
			if (flag)
			{
				actionIcon.SetSprite(actionSprite, false, null);
				progressRefers.actionNum.text = num;
			}
			else
			{
				sbyte grade = ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId);
				itemBack.SetSprite(ItemView.GetGradeBack(grade), false, null);
				itemBack.transform.GetChild(0).GetComponent<CImage>().SetSprite(ItemTemplateHelper.GetIcon(itemKey.ItemType, itemKey.TemplateId), false, null);
			}
			progressRefers.gameObject.SetActive(true);
		}

		// Token: 0x06008D79 RID: 36217 RVA: 0x0041D238 File Offset: 0x0041B438
		private void ShowCharCommonTips(bool isAlly, string tips, ItemKey itemKey, sbyte trickType = -1)
		{
			CombatInfoChar infoChar = isAlly ? this.selfInfoChar : this.enemyInfoChar;
			CombatCommonTips commonTipsRefers = infoChar.commonTips;
			CanvasGroup tipsCanvasGroup = commonTipsRefers.GetComponent<CanvasGroup>();
			CImage itemBack = commonTipsRefers.item;
			CImage trickBack = commonTipsRefers.trick;
			commonTipsRefers.tips.text = tips;
			itemBack.gameObject.SetActive(itemKey.IsValid());
			bool flag = itemKey.IsValid();
			if (flag)
			{
				sbyte grade = ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId);
				itemBack.SetSprite(ItemView.GetGradeBack(grade), false, null);
				itemBack.transform.GetChild(0).GetComponent<CImage>().SetSprite(ItemTemplateHelper.GetIcon(itemKey.ItemType, itemKey.TemplateId), false, null);
			}
			trickBack.gameObject.SetActive(trickType >= 0);
			bool flag2 = trickType >= 0;
			if (flag2)
			{
				CombatWeaponTrick.UpdateTrickIcon(trickBack.GetComponent<CombatTrickPrefab>(), trickType, false, false, false, null);
			}
			tipsCanvasGroup.DOKill(false);
			tipsCanvasGroup.alpha = 1f;
			tipsCanvasGroup.DOFade(0f, 0.5f).SetDelay(2f).OnComplete(delegate
			{
				commonTipsRefers.gameObject.SetActive(false);
			});
			commonTipsRefers.gameObject.SetActive(true);
		}

		// Token: 0x06008D7A RID: 36218 RVA: 0x0041D3A4 File Offset: 0x0041B5A4
		private string GetCaptureChanceText(int chance)
		{
			string chanceColor = (chance >= 100) ? "lightblue" : "pinkyellow";
			string chanceNumber = Math.Min(chance, 100).ToString();
			string chanceText = LocalStringManager.GetFormat(LanguageKey.LK_Combat_Capture_Tips, chanceNumber, chanceColor);
			return chanceText.ColorReplace();
		}

		// Token: 0x06008D7B RID: 36219 RVA: 0x0041D3F0 File Offset: 0x0041B5F0
		private void ShowTextTips(int charId, string tips)
		{
			RectTransform tipsTransform = CombatPoolAdaptor.GetObject<RectTransform>("ui_Combat_TextTipsPrefab", true);
			tipsTransform.GetComponent<TextMeshProUGUI>().text = tips;
			this.DoDropTipsAni(charId, tipsTransform, "ui_Combat_TextTipsPrefab");
		}

		// Token: 0x06008D7C RID: 36220 RVA: 0x0041D428 File Offset: 0x0041B628
		private void ShowIconTips(int charId, string iconSprite, string tips = "")
		{
			RectTransform tipsTransform = CombatPoolAdaptor.GetObject<RectTransform>("ui_Combat_IconTipsPrefab", true);
			this._tipsList.Add(tipsTransform);
			tipsTransform.GetComponent<CImage>().SetSprite(iconSprite, true, null);
			tipsTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = tips;
			this.DoDropTipsAni(charId, tipsTransform, "ui_Combat_IconTipsPrefab");
		}

		// Token: 0x06008D7D RID: 36221 RVA: 0x0041D480 File Offset: 0x0041B680
		private void ShowDamageNumTips(int charId, int damageValue, bool critical, DefeatMarkKey markKey)
		{
			this.SummaryDamageNum(charId, markKey, damageValue);
			bool flag = !this.SettingData.ShowDamageNumber;
			if (!flag)
			{
				bool isAlly = this._selfTeam.Contains(charId);
				string prefabKey = critical ? "ui_Combat_Fatal_Damage_Num" : "ui_Combat_Damage_Num";
				string spritePrefix = critical ? "combat_number_1_" : "combat_number_0_";
				RectTransform tipsTransform = CombatPoolAdaptor.GetObject<RectTransform>(prefabKey, true);
				this._tipsList.Add(tipsTransform);
				CombatDamageNumPrefab tipsRefers = tipsTransform.GetComponent<CombatDamageNumPrefab>();
				CImage selfIcon = tipsRefers.selfIcon;
				CImage enemyIcon = tipsRefers.enemyIcon;
				int numStartChildIndex = critical ? 4 : 2;
				GameObject numPrefab = tipsTransform.GetChild(numStartChildIndex).gameObject;
				List<int> digits = EasyPool.Get<List<int>>();
				int oppositeCharId = isAlly ? this.Model.EnemyCharId : this.Model.SelfCharId;
				CombatSubProcessorCharacter oppositeProcessor;
				bool flag2 = !this.Model.ProcessorCharacters.TryGetValue(oppositeCharId, out oppositeProcessor);
				if (!flag2)
				{
					short enemyPerformingSkill = oppositeProcessor.PreparingSkillId;
					bool isSkillDamage = enemyPerformingSkill >= 0 && CombatSkill.Instance[enemyPerformingSkill].EquipType == 1;
					digits.Clear();
					bool flag3 = damageValue > 0;
					if (flag3)
					{
						while (damageValue > 0)
						{
							digits.Add(damageValue % 10);
							damageValue /= 10;
						}
						digits.Reverse();
					}
					else
					{
						digits.Add(0);
					}
					selfIcon.gameObject.SetActive(isAlly);
					enemyIcon.gameObject.SetActive(!isAlly);
					(isAlly ? selfIcon : enemyIcon).SetSprite(this.GetDropMarkIcon(markKey), false, null);
					tipsTransform.localScale = Vector3.one * (isSkillDamage ? 1f : 0.8f);
					tipsTransform.GetComponent<BoxCollider2D>().size = new Vector2((float)(43 * digits.Count), (float)(critical ? 115 : 55));
					for (int i = 0; i < digits.Count; i++)
					{
						bool flag4 = numStartChildIndex + i >= tipsTransform.childCount;
						if (flag4)
						{
							Object.Instantiate<GameObject>(numPrefab, tipsTransform);
						}
						CImage numImg = tipsTransform.GetChild(numStartChildIndex + i).GetComponent<CImage>();
						numImg.SetSprite((digits.Count > 1 || digits[i] != 0) ? string.Format("{0}{1}", spritePrefix, digits[i]) : "combat_number_2_0", true, null);
						numImg.gameObject.SetActive(true);
					}
					for (int j = numStartChildIndex + digits.Count; j < tipsTransform.childCount; j++)
					{
						tipsTransform.GetChild(j).gameObject.SetActive(false);
					}
					EasyPool.Free<List<int>>(digits);
					if (critical)
					{
						tipsRefers.selfFatalMark.SetActive(isAlly);
						tipsRefers.enemyFatalMark.SetActive(!isAlly);
					}
					this.DoDropTipsAni(charId, tipsTransform, prefabKey);
				}
			}
		}

		// Token: 0x06008D7E RID: 36222 RVA: 0x0041D770 File Offset: 0x0041B970
		private void UpdateInnerRatioVisible()
		{
			Camera uiCamera = UIManager.Instance.UiCamera;
			ViewCombat.UpdateInnerRatioVisible(this.selfInnerRatioCheckArea, this.selfInnerRatio, uiCamera);
			ViewCombat.UpdateInnerRatioVisible(this.enemyInnerRatioCheckArea, this.enemyInnerRatio, uiCamera);
		}

		// Token: 0x06008D7F RID: 36223 RVA: 0x0041D7B0 File Offset: 0x0041B9B0
		private static bool IsMouseInRectTransform(RectTransform rectTransform, Camera uiCamera)
		{
			return rectTransform != null && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, uiCamera);
		}

		// Token: 0x06008D80 RID: 36224 RVA: 0x0041D7E0 File Offset: 0x0041B9E0
		private static void UpdateInnerRatioVisible(RectTransform checkArea, GameObject target, Camera uiCamera)
		{
			bool flag = target == null;
			if (!flag)
			{
				bool isMouseInCheckArea = ViewCombat.IsMouseInRectTransform(checkArea, uiCamera);
				RectTransform targetRectTransform = target.transform as RectTransform;
				bool isMouseInTarget = ViewCombat.IsMouseInRectTransform(targetRectTransform, uiCamera);
				bool visible = target.activeSelf ? (isMouseInCheckArea || isMouseInTarget) : isMouseInCheckArea;
				ViewCombat.SetInnerRatioVisible(target, visible);
			}
		}

		// Token: 0x06008D81 RID: 36225 RVA: 0x0041D830 File Offset: 0x0041BA30
		private static void SetInnerRatioVisible(GameObject target, bool visible)
		{
			bool flag = target != null && target.activeSelf != visible;
			if (flag)
			{
				target.SetActive(visible);
			}
		}

		// Token: 0x06008D82 RID: 36226 RVA: 0x0041D864 File Offset: 0x0041BA64
		private void DoDropTipsAni(int charId, RectTransform tipsTransform, string prefabKey)
		{
			bool isAlly = this._selfTeam.Contains(charId);
			PositionConstraint constraint = tipsTransform.GetComponent<PositionConstraint>();
			Vector2 force = new Vector2(Random.Range(60f, 200f) * (float)(isAlly ? -1 : 1), Random.Range(50f, 200f));
			constraint.constraintActive = false;
			constraint.locked = false;
			while (constraint.sourceCount > 0)
			{
				constraint.RemoveSource(0);
			}
			DOVirtual.DelayedCall(0.5f, delegate
			{
				ConstraintSource constraintSource = default(ConstraintSource);
				CombatInfoChar infoChar = isAlly ? this.selfInfoChar : this.enemyInfoChar;
				RectTransform srcTransform = infoChar.GetComponent<RectTransform>();
				constraintSource.sourceTransform = srcTransform;
				constraintSource.weight = 1f;
				constraint.AddSource(constraintSource);
				constraint.translationOffset = new Vector3(tipsTransform.anchoredPosition.x - srcTransform.anchoredPosition.x, 0f, 0f);
				constraint.locked = true;
				constraint.constraintActive = true;
			}, true);
			tipsTransform.gameObject.SetActive(true);
			tipsTransform.SetParent(this.injuryTextLayer, false);
			tipsTransform.position = this.GetSkeleton(charId).transform.TransformPoint(Vector3.zero.SetY(30f));
			tipsTransform.GetComponent<Rigidbody2D>().AddForce(force);
			CanvasGroup canvasGroup = tipsTransform.GetComponent<CanvasGroup>();
			canvasGroup.alpha = 1f;
			canvasGroup.DOFade(0.3f, 1.5f).SetDelay(1.5f).OnComplete(delegate
			{
				CombatPoolAdaptor.Destroy(prefabKey, tipsTransform.gameObject, true);
			});
		}

		// Token: 0x06008D83 RID: 36227 RVA: 0x0041D9E0 File Offset: 0x0041BBE0
		private void ShowSpecialEffectTip(bool isAlly, ShowSpecialEffectDisplayData effectInfo)
		{
			bool combatPureOpen = CombatUtils.CombatPureOpen;
			if (!combatPureOpen)
			{
				SpecialEffectItem configData = SpecialEffect.Instance[effectInfo.EffectId];
				int effectIndex = effectInfo.Index;
				RectTransform effectTransform = CombatPoolAdaptor.GetObject<RectTransform>("ui_Combat_EffectTipsPrefab", true);
				CanvasGroup canvasGroup = effectTransform.GetComponent<CanvasGroup>();
				TooltipInvoker mouseTip = effectTransform.GetComponent<TooltipInvoker>();
				PointerTrigger pointerTrigger = effectTransform.GetComponent<PointerTrigger>();
				GameObject highLight = effectTransform.GetChild(0).gameObject;
				string itemDesc = (effectInfo.ItemData.TemplateId >= 0) ? ItemTemplateHelper.GetDesc(effectInfo.ItemData.ItemType, effectInfo.ItemData.TemplateId) : null;
				CombatInfoChar infoChar = isAlly ? this.selfInfoChar : this.enemyInfoChar;
				CombatPrepareProgress siblingRef = infoChar.prepareProgress;
				effectTransform.SetParent(infoChar.follow.transform);
				effectTransform.SetSiblingIndex(siblingRef.transform.GetSiblingIndex() - 1);
				effectTransform.anchoredPosition = new Vector2((float)(Random.Range(-190, 11) * (isAlly ? 1 : -1)), (float)Random.Range(150, 311));
				effectTransform.GetComponent<CImage>().SetSprite(isAlly ? "combat_xuanfu_gongfa_0" : "combat_xuanfu_gongfa_1", false, null);
				effectTransform.GetChild(1).GetComponent<TextMeshProUGUI>().text = configData.Name;
				effectTransform.GetChild(2).GetComponent<TextMeshProUGUI>().text = configData.ShortDesc[effectIndex];
				string desc = itemDesc ?? ((effectInfo.EffectDescription.EffectId < 0) ? CommonUtils.GetSpecialEffectDesc(effectInfo.EffectId) : CommonUtils.GetSpecialEffectDesc(effectInfo.EffectDescription, isAlly));
				desc = (desc.IsNullOrEmpty() ? configData.Desc[0] : desc);
				mouseTip.PresetParam[0] = configData.Name;
				mouseTip.PresetParam[1] = desc;
				effectTransform.gameObject.SetActive(true);
				highLight.SetActive(false);
				pointerTrigger.EnterEvent.RemoveAllListeners();
				pointerTrigger.EnterEvent.AddListener(delegate()
				{
					highLight.SetActive(true);
				});
				pointerTrigger.ExitEvent.RemoveAllListeners();
				pointerTrigger.ExitEvent.AddListener(delegate()
				{
					highLight.SetActive(false);
				});
				canvasGroup.alpha = 0f;
				effectTransform.localScale = Vector3.zero;
				canvasGroup.DOFade(1f, 0.3f);
				effectTransform.DOScale(2f, 0.1f).SetEase(Ease.OutBack);
				effectTransform.DOScale(1f, 0.2f).SetDelay(0.1f).SetEase(Ease.OutBack);
				canvasGroup.DOFade(0f, 0.6f).SetDelay(3f);
				effectTransform.DOScale(2f, 0.6f).SetEase(Ease.InBack).SetDelay(3f).OnComplete(delegate
				{
					CombatPoolAdaptor.Destroy("ui_Combat_EffectTipsPrefab", effectTransform.gameObject, true);
				});
				int effectId = effectInfo.EffectId;
				bool flag = effectId == 1462 || effectId == 1487;
				if (flag)
				{
					GlobalDomainMethod.Call.InvokeGuidingTrigger(149);
				}
			}
		}

		// Token: 0x06008D84 RID: 36228 RVA: 0x0041DD3E File Offset: 0x0041BF3E
		private void UpdateWeaponSlot(bool isAlly, int index, ItemKey itemKey)
		{
			this.UpdateWeaponSlotInner(isAlly ? this._selfWeaponHolder : this._enemyWeaponHolder, isAlly, index, itemKey);
		}

		// Token: 0x06008D85 RID: 36229 RVA: 0x0041DD5C File Offset: 0x0041BF5C
		private void UpdateWeaponSlotInner(RectTransform weaponHolder, bool isAlly, int index, ItemKey itemKey)
		{
			CombatWeaponPrefab weaponRefers = weaponHolder.GetChild(index).GetComponent<CombatWeaponPrefab>();
			bool flag = index < 3;
			if (flag)
			{
				sbyte grade = ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId);
				weaponRefers.icon.SetSprite(ItemTemplateHelper.GetIcon(itemKey.ItemType, itemKey.TemplateId), false, null);
				weaponRefers.iconBack.color = Colors.Instance.GradeColors[(int)grade];
			}
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			CombatSubProcessorCharacter processor;
			bool flag2 = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag2)
			{
				bool flag3 = index == processor.UsingWeaponIndex && weaponHolder == this._selfWeaponHolder;
				if (flag3)
				{
					this.UpdateUsingWeapon(isAlly);
				}
				bool flag4 = isAlly && index < 3;
				if (flag4)
				{
					this.UpdateWeaponDurabilityInner(weaponHolder, isAlly, index);
				}
			}
		}

		// Token: 0x06008D86 RID: 36230 RVA: 0x0041DE4E File Offset: 0x0041C04E
		private void UpdateWeaponDurability(bool isAlly, int index, ItemKey itemKey)
		{
			this.UpdateWeaponDurabilityInner(isAlly ? this._selfWeaponHolder : this._enemyWeaponHolder, isAlly, index);
		}

		// Token: 0x06008D87 RID: 36231 RVA: 0x0041DE6C File Offset: 0x0041C06C
		private void UpdateWeaponDurabilityInner(RectTransform weaponHolder, bool isAlly, int index)
		{
			CombatWeaponPrefab weaponRefers = weaponHolder.GetChild(index).GetComponent<CombatWeaponPrefab>();
			CombatSubProcessorCharacter character = isAlly ? this.Model.SelfCharacter : this.Model.EnemyCharacter;
			bool flag = character == null;
			if (!flag)
			{
				bool flag2 = character.Weapons.Length <= index;
				if (!flag2)
				{
					CombatSubProcessorWeapon weapon = this.Model.ProcessorWeapons.GetValueOrDefault(character.Weapons[index]);
					bool flag3 = weapon == null;
					if (!flag3)
					{
						short durability = weapon.Durability;
						short maxDurability = weapon.MaxDurability;
						weaponRefers.UserInt = (int)durability;
						bool flag4 = durability <= 0;
						if (flag4)
						{
							weaponRefers.highLight.gameObject.SetActive(false);
						}
						weaponRefers.currDurability.text = ((durability > maxDurability / 2) ? durability.ToString() : durability.ToString().SetColor("brightred"));
						weaponRefers.maxDurability.text = string.Format("/{0}", maxDurability);
					}
				}
			}
		}

		// Token: 0x06008D88 RID: 36232 RVA: 0x0041DF73 File Offset: 0x0041C173
		private void UpdateWeaponCdImage(bool isAlly, int index, ItemKey itemKey)
		{
			this.UpdateWeaponCdImageInner(isAlly ? this._selfWeaponHolder : this._enemyWeaponHolder, isAlly, index, itemKey);
		}

		// Token: 0x06008D89 RID: 36233 RVA: 0x0041DF94 File Offset: 0x0041C194
		private void UpdateWeaponCdImageInner(RectTransform weaponHolder, bool isAlly, int index, ItemKey itemKey)
		{
			ViewCombat.<>c__DisplayClass523_0 CS$<>8__locals1;
			CS$<>8__locals1.isAlly = isAlly;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.index = index;
			CS$<>8__locals1.weaponRefers = weaponHolder.GetChild(CS$<>8__locals1.index).GetComponent<CombatWeaponPrefab>();
			CombatSubProcessorWeapon weaponProcessor = this.Model.ProcessorWeapons.GetValueOrDefault(itemKey);
			CS$<>8__locals1.noDurability = (CS$<>8__locals1.index < 3 && weaponProcessor != null && weaponProcessor.Durability <= 0);
			CS$<>8__locals1.cdFrame = ((weaponProcessor != null) ? weaponProcessor.CdFrame : 0);
			short fixedLeftCdFrame = (weaponProcessor != null) ? weaponProcessor.FixedCdLeftFrame : 0;
			short fixedTotalCdFrame = (weaponProcessor != null) ? weaponProcessor.FixedCdTotalFrame : 0;
			bool isLock = fixedLeftCdFrame > 0 | CS$<>8__locals1.noDurability;
			bool isCd = CS$<>8__locals1.cdFrame > 0 && (CS$<>8__locals1.index > 3 || !isLock);
			CS$<>8__locals1.cdProgress = CS$<>8__locals1.weaponRefers.cdProgress;
			CImage lockProgress = CS$<>8__locals1.weaponRefers.lockProgress;
			bool flag = isLock;
			if (flag)
			{
				float fillAmount = Mathf.Min(new float[]
				{
					(float)fixedLeftCdFrame / (float)Mathf.Max((int)fixedTotalCdFrame, 1)
				});
				bool noDurability = CS$<>8__locals1.noDurability;
				if (noDurability)
				{
					fillAmount = 1f;
				}
				bool flag2 = !lockProgress.gameObject.activeSelf;
				if (flag2)
				{
					lockProgress.gameObject.SetActive(true);
				}
				bool activeSelf = CS$<>8__locals1.cdProgress.gameObject.activeSelf;
				if (activeSelf)
				{
					CS$<>8__locals1.cdProgress.gameObject.SetActive(false);
				}
				lockProgress.fillAmount = fillAmount;
				string countDownText = CombatUtils.StyleCountDownText((float)fixedLeftCdFrame / 60f, CS$<>8__locals1.noDurability);
				CS$<>8__locals1.weaponRefers.countDownText.SetText(countDownText.SetColor("red"), true);
				this.<UpdateWeaponCdImageInner>g__SetCountDownTextActive|523_0(true, ref CS$<>8__locals1);
			}
			else
			{
				bool flag3 = isCd;
				if (flag3)
				{
					bool activeSelf2 = lockProgress.gameObject.activeSelf;
					if (activeSelf2)
					{
						lockProgress.gameObject.SetActive(false);
					}
					this.<UpdateWeaponCdImageInner>g__HandleCd|523_1(ref CS$<>8__locals1);
				}
				else
				{
					CS$<>8__locals1.cdProgress.fillAmount = 0f;
					lockProgress.fillAmount = 0f;
					this.<UpdateWeaponCdImageInner>g__SetCountDownTextActive|523_0(false, ref CS$<>8__locals1);
				}
			}
		}

		// Token: 0x06008D8A RID: 36234 RVA: 0x0041E1AF File Offset: 0x0041C3AF
		private void UpdateWeaponUnlockState(bool isAlly, bool updateUnlockPrepareValue, bool updateCanUnlock)
		{
			this.UpdateWeaponUnlockStateInner(isAlly ? this._selfWeaponHolder : this._enemyWeaponHolder, isAlly, updateUnlockPrepareValue, updateCanUnlock);
		}

		// Token: 0x06008D8B RID: 36235 RVA: 0x0041E1D0 File Offset: 0x0041C3D0
		private unsafe void UpdateWeaponUnlockStateInner(RectTransform weaponHolder, bool isAlly, bool updateUnlockPrepareValue, bool updateCanUnlock)
		{
			bool[] unlockTriggered = isAlly ? this._selfUnlockTriggered : this._enemyUnlockTriggered;
			List<int> unlockPrepareValueList = isAlly ? this._selfUnlockPrepareValue : this._enemyUnlockPrepareValue;
			List<int> oldUnlockPrepareValueList = isAlly ? this._oldSelfUnlockPrepareValue : this._oldEnemyUnlockPrepareValue;
			ItemKey[] weaponList = isAlly ? this._selfWeaponList : this._enemyWeaponList;
			List<bool> canUnlockList = isAlly ? this._selfCanUnlockAttack : this._enemyCanUnlockAttack;
			int currCharId = isAlly ? (*this._selfCurrCharId) : (*this._enemyCurrCharId);
			int i = 0;
			while (i < 3)
			{
				int index = i;
				CombatWeaponPrefab weaponRefers = weaponHolder.GetChild(index).GetComponent<CombatWeaponPrefab>();
				CombatWeaponUnlockHolderPrefab unlockRefers = weaponRefers.unlockHolder;
				GameObject unlockLoopEffectHolder = unlockRefers.unlockXunhuanEffectHolder;
				if (updateUnlockPrepareValue)
				{
					int unlockPrepareValue = unlockPrepareValueList[index];
					unlockRefers.gameObject.SetActive(unlockPrepareValue > 0);
					bool flag = unlockPrepareValue > 0;
					if (flag)
					{
						CImage progressHolder = unlockRefers.unlockProgressHolder;
						CButton unlockBtn = unlockRefers.unlockBtn;
						bool unlock = unlockPrepareValue >= GlobalConfig.Instance.UnlockAttackUnit;
						bool lastUnlock = oldUnlockPrepareValueList[index] >= GlobalConfig.Instance.UnlockAttackUnit;
						progressHolder.gameObject.SetActive(!unlock);
						unlockBtn.gameObject.SetActive(unlock);
						bool flag2 = unlock;
						if (flag2)
						{
							ParticleSystem unlockEffectParticle = unlockRefers.unlockEffectParticle;
							bool flag3 = isAlly && !this._selfUnlockEffectTriggered[index];
							if (flag3)
							{
								unlockEffectParticle.Play();
								this._selfUnlockEffectTriggered[index] = true;
							}
							bool flag4 = !isAlly && !lastUnlock;
							if (flag4)
							{
								unlockEffectParticle.Play();
							}
						}
						bool flag5 = unlock;
						if (flag5)
						{
							bool flag6 = !unlockTriggered[index];
							if (flag6)
							{
								AudioManager.Instance.PlaySound("ui_combat_unlock_ready", false, false);
								unlockTriggered[index] = true;
							}
						}
						else
						{
							CImage progress = unlockRefers.unlockProgress;
							progress.fillAmount = (float)unlockPrepareValue / (float)GlobalConfig.Instance.UnlockAttackUnit;
							bool flag7 = weaponList == null;
							if (flag7)
							{
								goto IL_428;
							}
							bool flag8 = !weaponList.CheckIndex(index);
							if (flag8)
							{
								goto IL_428;
							}
							ItemKey weapon = weaponList[index];
							bool flag9 = !weapon.IsValid();
							if (flag9)
							{
								goto IL_428;
							}
							short itemSubType = ItemTemplateHelper.GetItemSubType(weapon.ItemType, weapon.TemplateId);
							TooltipInvoker mouseTip = progressHolder.GetComponent<TooltipInvoker>();
							TooltipInvoker tooltipInvoker = mouseTip;
							if (tooltipInvoker.RuntimeParam == null)
							{
								tooltipInvoker.RuntimeParam = new ArgumentBox();
							}
							mouseTip.RuntimeParam.Clear();
							mouseTip.RuntimeParam.Set("UnlockPrepareValue", unlockPrepareValue);
							mouseTip.RuntimeParam.Set("ItemSubType", itemSubType);
							mouseTip.RuntimeParam.Set("CharacterId", currCharId);
							mouseTip.Refresh(false, -1);
						}
					}
					goto IL_2B7;
				}
				goto IL_2B7;
				IL_428:
				i++;
				continue;
				IL_2B7:
				if (updateCanUnlock)
				{
					bool canUnlock = canUnlockList[index];
					CButton unlockBtn2 = unlockRefers.unlockBtn;
					unlockBtn2.GetComponent<DisableStyleRoot>().SetStyleEffect(!canUnlock, false);
					unlockBtn2.interactable = canUnlock;
					unlockLoopEffectHolder.gameObject.SetActive(canUnlock);
					TooltipInvoker mouseTip2 = unlockBtn2.GetComponent<TooltipInvoker>();
					TooltipInvoker tooltipInvoker = mouseTip2;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					mouseTip2.RuntimeParam.Clear();
					bool forceUseHideAndShow = false;
					bool flag10 = canUnlock;
					if (flag10)
					{
						bool flag11 = weaponList == null;
						if (flag11)
						{
							break;
						}
						bool flag12 = !weaponList.CheckIndex(index);
						if (flag12)
						{
							break;
						}
						ItemKey weapon2 = weaponList[index];
						bool flag13 = mouseTip2.Type != TipType.CombatWeaponUnlock;
						if (flag13)
						{
							mouseTip2.Type = TipType.CombatWeaponUnlock;
							forceUseHideAndShow = true;
						}
						mouseTip2.RuntimeParam.Set("WeaponIndex", index);
						mouseTip2.RuntimeParam.Set("IsAlly", isAlly);
						mouseTip2.RuntimeParam.Set("WeaponTemplateId", weapon2.TemplateId);
					}
					else
					{
						bool flag14 = mouseTip2.Type > TipType.SingleDesc;
						if (flag14)
						{
							mouseTip2.Type = TipType.SingleDesc;
							forceUseHideAndShow = true;
						}
						mouseTip2.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Combat_Curr_Weapon_Out_Range_Tips2));
					}
					bool flag15 = forceUseHideAndShow;
					if (flag15)
					{
						mouseTip2.Refresh(true, -1);
					}
				}
				goto IL_428;
			}
		}

		// Token: 0x06008D8C RID: 36236 RVA: 0x0041E61C File Offset: 0x0041C81C
		private void UpdateUsingWeapon(bool isAlly)
		{
			ItemKey usingWeaponKey = this.GetUsingWeaponKey(isAlly);
			bool flag = !usingWeaponKey.IsValid();
			if (!flag)
			{
				WeaponItem weaponConfig = Weapon.Instance[usingWeaponKey.TemplateId];
				this.UpdateUsingWeaponSlot(isAlly);
				this.UpdateAttackRange(isAlly);
				this.ShowCharCommonTips(isAlly, weaponConfig.Name, usingWeaponKey, -1);
			}
		}

		// Token: 0x06008D8D RID: 36237 RVA: 0x0041E674 File Offset: 0x0041C874
		private void SetUsingWeaponIndex(bool isAlly, int usingWeaponIndex)
		{
			RectTransform weaponHolder = isAlly ? this._selfWeaponHolder : this._enemyWeaponHolder;
			for (int i = 0; i < weaponHolder.childCount; i++)
			{
				CombatWeaponPrefab weaponRefers = weaponHolder.GetChild(i).GetComponent<CombatWeaponPrefab>();
				weaponRefers.usingGo.SetActive(usingWeaponIndex == i);
				weaponRefers.outAttackRange.SetActive(false);
			}
			bool flag = (isAlly ? this._selfWeaponList : this._enemyWeaponList) != null;
			if (flag)
			{
				this.UpdateUsingWeapon(isAlly);
			}
		}

		// Token: 0x06008D8E RID: 36238 RVA: 0x0041E6F8 File Offset: 0x0041C8F8
		private unsafe void OnWeaponsChanged(bool isAlly)
		{
			int charId = isAlly ? (*this._selfCurrCharId) : (*this._enemyCurrCharId);
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				ItemKey[] weaponList = processor.Weapons;
				RectTransform weaponHolder = isAlly ? this._selfWeaponHolder : this._enemyWeaponHolder;
				for (int weaponIndex = 0; weaponIndex < weaponList.Length; weaponIndex++)
				{
					ItemKey weaponKey = weaponList[weaponIndex];
					int index = weaponIndex;
					weaponHolder.GetChild(weaponIndex).gameObject.SetActive(weaponKey.IsValid());
					bool flag2 = weaponKey.IsValid();
					if (flag2)
					{
						this.UpdateWeaponSlot(isAlly, weaponIndex, weaponKey);
						ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
						argBox.Set<ItemKey>("ItemKey", weaponKey);
						argBox.Set("CharId", charId);
						argBox.Set("GetNewItemDisplayData", true);
						weaponHolder.GetChild(index).GetComponent<TooltipInvoker>().RuntimeParam = argBox;
					}
				}
			}
		}

		// Token: 0x06008D8F RID: 36239 RVA: 0x0041E800 File Offset: 0x0041CA00
		private unsafe void OnUsingWeaponIndexChanged(bool isAlly)
		{
			int charId = isAlly ? (*this._selfCurrCharId) : (*this._enemyCurrCharId);
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				int usingWeaponIndex = processor.UsingWeaponIndex;
				this.SetUsingWeaponIndex(isAlly, usingWeaponIndex);
			}
		}

		// Token: 0x06008D90 RID: 36240 RVA: 0x0041E850 File Offset: 0x0041CA50
		private unsafe void OnAttackSkillListChanged(bool isAlly)
		{
			int charId = isAlly ? (*this._selfCurrCharId) : (*this._enemyCurrCharId);
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				List<short> attackSkillList = processor.AttackSkillList;
				if (isAlly)
				{
					bool flag2 = !this._attackSkillInitialized;
					if (flag2)
					{
						this._attackSkillInitialized = true;
					}
					else
					{
						bool isTutorialCombat = this.Model.IsTutorialCombat;
						if (isTutorialCombat)
						{
							int appendedSkillIndex = 0;
							short appendedSkillId = attackSkillList[0];
							for (int i = 1; i < attackSkillList.Count; i++)
							{
								int appendedSkillIndex2 = appendedSkillIndex;
								appendedSkillIndex = appendedSkillIndex2 + 1;
								appendedSkillId = attackSkillList[1];
							}
							CombatSkillItem appendedSkillConfig = CombatSkill.Instance[appendedSkillId];
							string assetPath = "RemakeResources/Combat/CombatSkills/" + appendedSkillConfig.AssetFileName + "/" + appendedSkillConfig.AssetFileName;
							ResLoader.Load<CombatSkillAsset>(assetPath, delegate(CombatSkillAsset skillAsset)
							{
								this.ApplyCombatSkillResource(skillAsset, appendedSkillConfig);
							}, null, false);
							CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayData(this, *this._selfCurrCharId, new List<short>
							{
								appendedSkillId
							}, delegate(int offset, RawDataPool skillDataPool)
							{
								List<CombatSkillDisplayData> dataList = EasyPool.Get<List<CombatSkillDisplayData>>();
								Serializer.Deserialize(skillDataPool, offset, ref dataList);
								CombatSkillDisplayData skillData = dataList[0];
								EasyPool.Free<List<CombatSkillDisplayData>>(dataList);
								this.Model.OrderedProactiveSkillList[this.Model.SelfCharId][appendedSkillIndex] = skillData.TemplateId;
								this.combatSkillScroll.SetSkillViewByIndex(appendedSkillIndex, skillData);
							});
							bool flag3 = this.Model.Config.TemplateId != 125 || appendedSkillIndex > 0;
							if (flag3)
							{
								this._keepPauseUntilCastSkill = true;
								this.combatTimeScaleToggle.PauseInteractable = false;
							}
						}
					}
				}
			}
		}

		// Token: 0x06008D91 RID: 36241 RVA: 0x0041E9EC File Offset: 0x0041CBEC
		private bool TryGetWeaponIndexAndIsAllyByItemKey(ItemKey itemKey, out int index, out bool isAlly)
		{
			CombatSubProcessorCharacter selfCharacter = this.Model.SelfCharacter;
			index = ((selfCharacter != null) ? selfCharacter.Weapons.IndexOf(itemKey) : -1);
			isAlly = (index >= 0);
			bool flag = index < 0;
			if (flag)
			{
				CombatSubProcessorCharacter enemyCharacter = this.Model.EnemyCharacter;
				index = ((enemyCharacter != null) ? enemyCharacter.Weapons.IndexOf(itemKey) : -1);
			}
			return index >= 0;
		}

		// Token: 0x06008D92 RID: 36242 RVA: 0x0041EA58 File Offset: 0x0041CC58
		private void OnWeaponDurabilityChanged(ItemKey itemKey)
		{
			int index;
			bool isAlly;
			bool flag = !this.TryGetWeaponIndexAndIsAllyByItemKey(itemKey, out index, out isAlly);
			if (!flag)
			{
				this.UpdateWeaponCdImage(isAlly, index, itemKey);
				bool flag2 = index < 3;
				if (flag2)
				{
					this.UpdateWeaponDurability(isAlly, index, itemKey);
				}
			}
		}

		// Token: 0x06008D93 RID: 36243 RVA: 0x0041EA98 File Offset: 0x0041CC98
		private void OnWeaponCanChangeToChanged(ItemKey itemKey)
		{
			int index;
			bool isAlly;
			bool flag = !this.TryGetWeaponIndexAndIsAllyByItemKey(itemKey, out index, out isAlly);
			if (!flag)
			{
				bool canChangeTo = this.Model.ProcessorWeapons[itemKey].CanChangeTo;
				bool flag2 = isAlly;
				if (flag2)
				{
					RectTransform weaponHolder = this._selfWeaponHolder;
					CombatWeaponPrefab weaponRefers = weaponHolder.GetChild(index).GetComponent<CombatWeaponPrefab>();
					weaponRefers.GetComponent<CButton>().interactable = (canChangeTo && this.Model.CanOperateSelfCharacter);
				}
			}
		}

		// Token: 0x06008D94 RID: 36244 RVA: 0x0041EB10 File Offset: 0x0041CD10
		private void OnWeaponCdFrameRelatedDataChanged(ItemKey itemKey)
		{
			int index;
			bool isAlly;
			bool flag = !this.TryGetWeaponIndexAndIsAllyByItemKey(itemKey, out index, out isAlly);
			if (!flag)
			{
				this.UpdateWeaponCdImage(isAlly, index, itemKey);
			}
		}

		// Token: 0x06008D95 RID: 36245 RVA: 0x0041EB3C File Offset: 0x0041CD3C
		private void OnCombatSkillCanUseChanged(CombatSkillKey combatSkillKey)
		{
			bool flag = CombatSkillEquipType.IsAssist(combatSkillKey.SkillTemplateId);
			if (!flag)
			{
				CombatSubProcessorSkill processorSkill;
				bool flag2 = !this.Model.ProcessorSkills.TryGetValue(combatSkillKey, out processorSkill);
				if (!flag2)
				{
					bool canUse = processorSkill.CanUse;
					short skillId = combatSkillKey.SkillTemplateId;
					bool interactable = canUse && this.Model.CanOperateSelfCharacter;
					bool flag3 = skillId == this.Model.PreviewCostSkill && this.Model.PreviewCostSkillCanUse != interactable;
					if (flag3)
					{
						this.Model.ModifyPreviewCostSkillCanUse(interactable);
						this.UpdateSkillCostPreview();
					}
				}
			}
		}

		// Token: 0x06008D96 RID: 36246 RVA: 0x0041EBDC File Offset: 0x0041CDDC
		private ItemKey GetUsingWeaponKey(bool isAlly)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			ItemKey result;
			if (flag)
			{
				result = ItemKey.Invalid;
			}
			else
			{
				bool flag2 = processor.UsingWeaponIndex >= 0 && processor.Weapons != null;
				if (flag2)
				{
					result = processor.Weapons[processor.UsingWeaponIndex];
				}
				else
				{
					result = ItemKey.Invalid;
				}
			}
			return result;
		}

		// Token: 0x06008D97 RID: 36247 RVA: 0x0041EC60 File Offset: 0x0041CE60
		private int GetUsingWeaponTemplateId(bool isAlly)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				bool flag2 = processor.UsingWeaponIndex >= 0 && processor.Weapons != null;
				if (flag2)
				{
					result = (int)processor.Weapons[processor.UsingWeaponIndex].TemplateId;
				}
				else
				{
					result = -1;
				}
			}
			return result;
		}

		// Token: 0x06008D98 RID: 36248 RVA: 0x0041ECE4 File Offset: 0x0041CEE4
		private void OnClickChangeTrick()
		{
			bool canOperateSelfCharacter = this.Model.CanOperateSelfCharacter;
			if (canOperateSelfCharacter)
			{
				this.DoRequestChangeTrick();
			}
		}

		// Token: 0x06008D99 RID: 36249 RVA: 0x0041ED08 File Offset: 0x0041CF08
		private void ChangeTrickMaskSetActive(bool active)
		{
			if (active)
			{
				this.changeTrickMask.SetActive(true);
			}
			else
			{
				this.changeTrickMask.SetActive(false);
			}
		}

		// Token: 0x06008D9A RID: 36250 RVA: 0x0041ED3C File Offset: 0x0041CF3C
		private unsafe void ShowChangeTrick()
		{
			CombatSubProcessorCharacter selfCharacter = this.Model.SelfCharacter;
			bool flag = selfCharacter == null || !selfCharacter.CanChangeTrick;
			if (!flag)
			{
				this.SetTimeScale(0f);
				this.HideAttackTips();
				CToggleGroup changeTrickTypeTogGroup = this.changeTrickTrickHolder;
				CToggleGroup changeTrickPartTogGroup = this.changeTrickPartHolder;
				CharacterItem charConfig = Character.Instance[this._charDisplayDataDict[*this._enemyCurrCharId].TemplateId];
				CommonTipItem trickTipItem = CommonTip.DefValue.CombatChangeTrickTrick;
				for (int i = 0; i < this.Model.SelfCharacter.WeaponTricks.Length; i++)
				{
					sbyte trickType = this.Model.SelfCharacter.WeaponTricks[i];
					TrickTypeItem trickConfig = Config.TrickType.Instance[trickType];
					changeTrickTypeTogGroup.transform.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = trickConfig.Name;
					GameObject toggle = changeTrickTypeTogGroup.transform.GetChild(i).gameObject;
					TooltipInvoker trickMouseTip = toggle.GetComponent<TooltipInvoker>();
					bool flag2 = trickMouseTip != null;
					if (flag2)
					{
						int avoidType = (int)trickConfig.AvoidType;
						CommonTipSimpleRuntime runtime = trickTipItem.BuildSimple();
						runtime.Set("trickName", trickConfig.ChineseName);
						runtime.Set("hitTypeIcon", "<SpName=" + TipsRefiningEffect.RefiningIconName[0][avoidType] + ">");
						runtime.Set("hitTypeName", LocalStringManager.Get(TipsRefiningEffect.RefiningPropertyNameKey[0][avoidType]));
						trickMouseTip.Type = TipType.CommonTip;
						TooltipInvoker tooltipInvoker = trickMouseTip;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = new ArgumentBox();
						}
						trickMouseTip.RuntimeParam.SetObject("Runtime", runtime);
					}
				}
				changeTrickPartTogGroup.SetInteractable(charConfig.HaveLeftArm, 3);
				changeTrickPartTogGroup.SetInteractable(charConfig.HaveRightArm, 4);
				changeTrickPartTogGroup.SetInteractable(charConfig.HaveLeftLeg, 5);
				changeTrickPartTogGroup.SetInteractable(charConfig.HaveRightLeg, 6);
				bool flag3 = !changeTrickPartTogGroup.Get((int)this._changeTrickBodyPart).interactable;
				if (flag3)
				{
					this._changeTrickBodyPart = 0;
				}
				for (int j = 0; j < changeTrickPartTogGroup.Count(); j++)
				{
					CToggle partTog = changeTrickPartTogGroup.Get(j);
					partTog.GetComponent<DisableStyleRoot>().SetStyleEffect(!partTog.interactable, false);
				}
				changeTrickTypeTogGroup.Set((int)this._changeTrickIndex, false);
				changeTrickPartTogGroup.Set((int)this._changeTrickBodyPart, false);
				this.SetSelfAttackTogGroup();
				this.ChangeTrickMaskSetActive(true);
			}
		}

		// Token: 0x06008D9B RID: 36251 RVA: 0x0041EFC0 File Offset: 0x0041D1C0
		private void SetSelfAttackTogGroup()
		{
			CButton flawBtn = this.flawChangeTrickBtn;
			CButton acupointBtn = this.acupointChangeTrickBtn;
			int weaponTemplateId = this.GetUsingWeaponTemplateId(true);
			int attackNeedCount = (int)(Weapon.Instance[weaponTemplateId].AttackPreparePointCost + 1);
			int flawNeedChangeTrickCount = GlobalConfig.Instance.ChangeTrickMultiplierFlaw * attackNeedCount;
			int acupointNeedChangeTrickCount = GlobalConfig.Instance.ChangeTrickMultiplierAcupoint * attackNeedCount;
			CombatSubProcessorCharacter selfCharacter = this.Model.SelfCharacter;
			short selfChangeTrickCount = (selfCharacter != null) ? selfCharacter.ChangeTrickCount : 0;
			flawBtn.interactable = ((int)selfChangeTrickCount >= flawNeedChangeTrickCount);
			acupointBtn.interactable = ((int)selfChangeTrickCount >= acupointNeedChangeTrickCount);
			TooltipInvoker flawMouseTip = flawBtn.GetComponent<TooltipInvoker>();
			flawMouseTip.triggerByChildRaycast = true;
			TooltipInvoker acupointMouseTip = acupointBtn.GetComponent<TooltipInvoker>();
			acupointMouseTip.triggerByChildRaycast = true;
			flawMouseTip.Type = TipType.GeneralLines;
			acupointMouseTip.Type = TipType.GeneralLines;
			TooltipInvoker tooltipInvoker = flawMouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tooltipInvoker = acupointMouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			int lineCount = 0;
			flawMouseTip.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Combat_Flaw));
			flawMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 4,
				PreferredHeight = 15f
			});
			flawMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(3, new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_Combat_ChangeTrick_FlawTips).SetColor("lightgrey")
			}, null));
			flawMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 4,
				PreferredHeight = 25f
			});
			flawMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(5, new List<string>
			{
				LocalStringManager.GetFormat(LanguageKey.LK_Combat_ChangeTrick_Cost, ("×" + flawNeedChangeTrickCount.ToString()).SetColor(flawBtn.interactable ? "brightblue" : "brightred"))
			}, null));
			flawMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 4,
				PreferredHeight = 30f
			});
			flawMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(8, new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_MouseTip_CombatFlaw_Effect).SetColor("lightgrey") + "：",
				MouseTipConstant.HitPartNamesByConfig[this.changeTrickPartHolder.GetActiveIndex(), 2],
				LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_CombatFlaw_Desc, string.Format("{0}%", GlobalConfig.Instance.FlawAddDamagePercent)).SetColor("lightgrey")
			}, null));
			flawMouseTip.RuntimeParam.Set("LineCount", lineCount);
			flawMouseTip.RuntimeParam.Set("EncyclopediaLink", 116);
			lineCount = 0;
			acupointMouseTip.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Combat_Acupoint));
			acupointMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 4,
				PreferredHeight = 15f
			});
			acupointMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(3, new List<string>
			{
				LocalStringManager.Get(LanguageKey.LK_Combat_ChangeTrick_AcupointTips).SetColor("lightgrey")
			}, null));
			acupointMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 4,
				PreferredHeight = 25f
			});
			acupointMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(5, new List<string>
			{
				LocalStringManager.GetFormat(LanguageKey.LK_Combat_ChangeTrick_Cost, ("×" + acupointNeedChangeTrickCount.ToString()).SetColor(acupointBtn.interactable ? "brightblue" : "brightred"))
			}, null));
			acupointMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
			{
				Type = 4,
				PreferredHeight = 30f
			});
			acupointMouseTip.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(3, new List<string>
			{
				BodyPart.Instance[this.changeTrickPartHolder.GetActiveIndex()].AcupointDesc.SetColor("lightgrey")
			}, null));
			acupointMouseTip.RuntimeParam.Set("LineCount", lineCount);
			acupointMouseTip.RuntimeParam.Set("EncyclopediaLink", 116);
			this.SetSelfAttackTogGroupCostText();
			TooltipInvoker confirmMouseTip = this.confirmChangeTrick.transform.Find("Interact").GetComponent<TooltipInvoker>();
			bool flag = confirmMouseTip != null;
			if (flag)
			{
				int bodyPartIndex = this.changeTrickPartHolder.GetActiveIndex();
				string bodyPartIcon = BodyPart.Instance[bodyPartIndex].MouseTipIcon;
				string bodyPartName = BodyPart.Instance[bodyPartIndex].Name;
				int pointCost = attackNeedCount - 1;
				int addHitRate = (int)(100 + GlobalConfig.Instance.AttackChangeTrickHitValueAddPercent[pointCost]);
				CommonTipItem confirmTipItem = CommonTip.DefValue.CombatChangeTrickConfirm;
				CommonTipSimpleRuntime runtime = confirmTipItem.BuildSimple();
				runtime.Set("changeTrickCount", attackNeedCount.ToString());
				runtime.Set("bodyPartIcon", "<SpName=" + bodyPartIcon + ">");
				runtime.Set("bodyPartName", bodyPartName);
				runtime.Set("hitRatePercent", addHitRate.ToString());
				confirmMouseTip.Type = TipType.CommonTip;
				tooltipInvoker = confirmMouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				confirmMouseTip.RuntimeParam.SetObject("Runtime", runtime);
			}
		}

		// Token: 0x06008D9C RID: 36252 RVA: 0x0041F62C File Offset: 0x0041D82C
		private void SetSelfAttackTogGroupCostText()
		{
			int weaponTemplateId = this.GetUsingWeaponTemplateId(true);
			int attackNeedCount = (int)(Weapon.Instance[weaponTemplateId].AttackPreparePointCost + 1);
			int flawNeedChangeTrickCount = GlobalConfig.Instance.ChangeTrickMultiplierFlaw * attackNeedCount;
			int acupointNeedChangeTrickCount = GlobalConfig.Instance.ChangeTrickMultiplierAcupoint * attackNeedCount;
			CombatSubProcessorCharacter selfCharacter = this.Model.SelfCharacter;
			short selfChangeTrickCount = (selfCharacter != null) ? selfCharacter.ChangeTrickCount : 0;
			this.flawChangeTrickCostText.SetText(string.Format("{0}/{1}", selfChangeTrickCount, flawNeedChangeTrickCount), true);
			this.acupointChangeTrickCostText.SetText(string.Format("{0}/{1}", selfChangeTrickCount, acupointNeedChangeTrickCount), true);
			this.confirmChangeTrickCostText.SetText(string.Format("{0}/{1}", selfChangeTrickCount, attackNeedCount), true);
		}

		// Token: 0x06008D9D RID: 36253 RVA: 0x0041F6F4 File Offset: 0x0041D8F4
		private void SetChangeTrickBodyType(SkillEffectCollection effectCollection, bool isAlly)
		{
			int count;
			CombatSkillItem existCombatSkillItem;
			SpecialEffectItem existSpecialEffectItem;
			bool exist = CombatUtils.FindTransferProportionEffectFromCollection(effectCollection, out count, out existCombatSkillItem, out existSpecialEffectItem);
			bool flag = !isAlly;
			if (!flag)
			{
				CToggleGroup changeTrickTypeTogGroup = this.changeTrickPartHolder;
				for (sbyte bodyPart = 0; bodyPart < 7; bodyPart += 1)
				{
					CToggle partToggle = changeTrickTypeTogGroup.Get((int)bodyPart);
					CombatBodyPartPrefab partRefers = partToggle.GetComponent<CombatBodyPartPrefab>();
					TextMeshProUGUI skillEffectCount = partRefers.skillEffectCount;
					skillEffectCount.gameObject.SetActive(exist);
					skillEffectCount.SetText(count.ToString(), true);
					partRefers.back.SetSprite(exist ? "combat_bottom_anniu_3_0" : "combat_bottom_anniu_2_0", false, null);
					partRefers.highlight.SetSprite(exist ? "combat_bottom_anniu_3_1" : "combat_bottom_anniu_2_1", false, null);
					partRefers.checkMark.SetSprite(exist ? "combat_bottom_anniu_3_2" : "combat_bottom_anniu_2_2", false, null);
					TooltipInvoker mouseTip = partRefers.mouseTipDisplayer;
					mouseTip.enabled = true;
					bool flag2 = exist;
					if (flag2)
					{
						sbyte fiveElementsType = BodyPartType.TransferToFiveElementsType(bodyPart);
						string desc = LocalStringManager.Get(LanguageKey.LK_LegendaryBook_Affect_Skill_Tips) + "：" + existCombatSkillItem.Name.SetGradeColor((int)existCombatSkillItem.Grade);
						string desc2 = string.Format("{0}：{1}", LocalStringManager.Get(LanguageKey.LK_LegendaryBook_Affect_Skill_Tips2), count);
						string desc3 = LocalStringManager.GetFormat(LanguageKey.LK_LegendaryBook_Affect_Skill_Tips3, existSpecialEffectItem.TransferProportion, (LocalStringManager.Get(string.Format("LK_FiveElements_Type_{0}", fiveElementsType)) + LocalStringManager.Get(LanguageKey.LK_Neili)).SetColor(Colors.Instance.FiveElementsColors[(int)fiveElementsType])).ColorReplace();
						mouseTip.Type = TipType.ChangeTrick;
						TooltipInvoker tooltipInvoker = mouseTip;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = new ArgumentBox();
						}
						mouseTip.RuntimeParam.Set("arg0", BodyPart.Instance[bodyPart].Name);
						mouseTip.RuntimeParam.Set("arg1", desc);
						mouseTip.RuntimeParam.Set("arg2", desc2);
						mouseTip.RuntimeParam.Set("arg3", desc3);
					}
					else
					{
						mouseTip.Type = TipType.Simple;
						TooltipInvoker tooltipInvoker = mouseTip;
						if (tooltipInvoker.RuntimeParam == null)
						{
							tooltipInvoker.RuntimeParam = new ArgumentBox();
						}
						mouseTip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Combat_ChangeTrick_AttackPartTitle));
						mouseTip.RuntimeParam.Set("arg1", LocalStringManager.GetFormat(LanguageKey.LK_Combat_ChangeTrick_AttackPartDesc, BodyPart.Instance[bodyPart].Name));
					}
				}
			}
		}

		// Token: 0x06008D9E RID: 36254 RVA: 0x0041F994 File Offset: 0x0041DB94
		private void OnClickConfirmChangeTrick()
		{
			bool flag = !this.Model.SelfChangingTrick;
			if (!flag)
			{
				CToggleGroup changeTrickTypeTogGroup = this.changeTrickTrickHolder;
				CToggleGroup changeTrickPartTogGroup = this.changeTrickPartHolder;
				this.SetTimeScale(this._realTimeScale);
				this.ChangeTrickMaskSetActive(false);
				CombatSubProcessorCharacter selfProcessor = this.Model.SelfCharacter;
				bool flag2 = selfProcessor == null;
				if (!flag2)
				{
					CombatDomainMethod.Call.SelectChangeTrick(selfProcessor.WeaponTricks[changeTrickTypeTogGroup.GetActiveIndex()], (sbyte)changeTrickPartTogGroup.GetActiveIndex(), 0);
					GameDataBridge.AddDataModification<sbyte>(8, 34, ulong.MaxValue, uint.MaxValue, (sbyte)changeTrickTypeTogGroup.GetActiveIndex());
					GameDataBridge.AddDataModification<sbyte>(8, 35, ulong.MaxValue, uint.MaxValue, (sbyte)changeTrickPartTogGroup.GetActiveIndex());
				}
			}
		}

		// Token: 0x06008D9F RID: 36255 RVA: 0x0041FA34 File Offset: 0x0041DC34
		private static void SetupChangeTrickBtnHover(CButton button)
		{
			PointerTrigger pointerTrigger = button.GetComponent<PointerTrigger>();
			Transform hover = button.transform.Find("Hover");
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				bool interactable = button.interactable;
				if (interactable)
				{
					hover.gameObject.SetActive(true);
				}
			});
			pointerTrigger.ExitEvent.AddListener(delegate()
			{
				hover.gameObject.SetActive(false);
			});
		}

		// Token: 0x06008DA0 RID: 36256 RVA: 0x0041FAA8 File Offset: 0x0041DCA8
		private void OnClickFlawOrAcupointChangeTrick(EFlawOrAcupointType type)
		{
			bool flag = !this.Model.SelfChangingTrick;
			if (!flag)
			{
				CToggleGroup changeTrickTypeTogGroup = this.changeTrickTrickHolder;
				CToggleGroup changeTrickPartTogGroup = this.changeTrickPartHolder;
				this.SetTimeScale(this._realTimeScale);
				this.ChangeTrickMaskSetActive(false);
				CombatSubProcessorCharacter selfProcessor = this.Model.SelfCharacter;
				bool flag2 = selfProcessor == null;
				if (!flag2)
				{
					CombatDomainMethod.Call.SelectChangeTrick(selfProcessor.WeaponTricks[changeTrickTypeTogGroup.GetActiveIndex()], (sbyte)changeTrickPartTogGroup.GetActiveIndex(), (int)type);
					GameDataBridge.AddDataModification<sbyte>(8, 34, ulong.MaxValue, uint.MaxValue, (sbyte)changeTrickTypeTogGroup.GetActiveIndex());
					GameDataBridge.AddDataModification<sbyte>(8, 35, ulong.MaxValue, uint.MaxValue, (sbyte)changeTrickPartTogGroup.GetActiveIndex());
				}
			}
		}

		// Token: 0x06008DA1 RID: 36257 RVA: 0x0041FB48 File Offset: 0x0041DD48
		private void OnClickCancelChangeTrick()
		{
			bool flag = !this.Model.SelfChangingTrick;
			if (!flag)
			{
				CombatSubProcessorCharacter selfProcessor = this.Model.SelfCharacter;
				bool flag2 = selfProcessor != null;
				if (flag2)
				{
					selfProcessor.ChangingTrick = false;
				}
				bool flag3 = !this.IsPausing;
				if (flag3)
				{
					this.SetTimeScale(this._realTimeScale);
				}
				CombatDomainMethod.Call.CancelChangeTrick();
				this.ChangeTrickMaskSetActive(false);
				GameDataBridge.AddDataModification<sbyte>(8, 34, ulong.MaxValue, uint.MaxValue, (sbyte)this.changeTrickTrickHolder.GetActiveIndex());
				GameDataBridge.AddDataModification<sbyte>(8, 35, ulong.MaxValue, uint.MaxValue, (sbyte)this.changeTrickPartHolder.GetActiveIndex());
			}
		}

		// Token: 0x06008DA2 RID: 36258 RVA: 0x0041FBE0 File Offset: 0x0041DDE0
		private void ShowJumpSetting(CombatProactiveSkillView skillView, CombatSkillDisplayData displayData)
		{
			bool flag = displayData.JumpThreshold < 0;
			if (!flag)
			{
				this._settingJumpThreshold = true;
				bool flag2 = !this.IsPausing;
				if (flag2)
				{
					this.SwitchPauseState(true, ViewCombat.EPauseReason.JumpSetting);
				}
				this.combatSkillScroll.enabled = false;
				this.jumpThresholdMask.gameObject.SetActive(true);
				this.jumpThresholdSetting.OnConfirm = new Action(this.OnExitJumpSetting);
				this.jumpThresholdSetting.OnCancel = new Action(this.OnExitJumpSetting);
				this.jumpThresholdSetting.Refresh(skillView.GetHolderComponent<Canvas>(), displayData);
				CButton jumpSettingButton = skillView.jumpSetting;
				RectTransform jumpThresholdSettingRect = (RectTransform)this.jumpThresholdSetting.transform;
				jumpThresholdSettingRect.SetParent(jumpSettingButton.transform);
				jumpThresholdSettingRect.anchoredPosition = Vector2.zero.SetY(20f);
				jumpThresholdSettingRect.SetParent(this.jumpThresholdMask.transform, true);
			}
		}

		// Token: 0x06008DA3 RID: 36259 RVA: 0x0041FCD0 File Offset: 0x0041DED0
		private void HideJumpSetting()
		{
			bool flag = !this._settingJumpThreshold;
			if (!flag)
			{
				this.jumpThresholdSetting.ForceCancel();
			}
		}

		// Token: 0x06008DA4 RID: 36260 RVA: 0x0041FCFC File Offset: 0x0041DEFC
		private void OnExitJumpSetting()
		{
			this._settingJumpThreshold = false;
			bool flag = this._pauseReason == ViewCombat.EPauseReason.JumpSetting;
			if (flag)
			{
				this.SwitchPauseState(false, ViewCombat.EPauseReason.JumpSetting);
			}
			this.combatSkillScroll.enabled = true;
			this.jumpThresholdMask.gameObject.SetActive(false);
		}

		// Token: 0x06008DA5 RID: 36261 RVA: 0x0041FD48 File Offset: 0x0041DF48
		private void UpdateExecutingTeammateCmd(bool isAlly, int teammateId, sbyte cmdType, sbyte oldExecutingTeammateCommand)
		{
			CombatAffectingDefendSkill defendSkillRefers = isAlly ? this.selfTeammateAffectingDefSkill : this.enemyTeammateAffectingDefSkill;
			bool flag = oldExecutingTeammateCommand == cmdType;
			if (!flag)
			{
				ETeammateCommandImplement originImplement = (oldExecutingTeammateCommand < 0) ? ETeammateCommandImplement.Invalid : TeammateCommand.Instance[oldExecutingTeammateCommand].Implement;
				ETeammateCommandImplement cmdImplement = (cmdType < 0) ? ETeammateCommandImplement.Invalid : TeammateCommand.Instance[cmdType].Implement;
				bool flag2 = cmdType >= 0;
				if (flag2)
				{
					SkeletonAnimation skeleton = this.GetSkeleton(teammateId);
					AnimalItem animalItem;
					bool flag3 = this.TryGetAnimalConfig(teammateId, out animalItem);
					if (!flag3)
					{
						bool flag4 = cmdImplement.IsAttack();
						if (flag4)
						{
							ItemKey weaponKey = this.Model.ProcessorCharacters[teammateId].AttackCommandWeaponKey;
							CombatAnimationUtils.UpdateSkeletonWeapon(skeleton, (int)weaponKey.TemplateId);
						}
						else
						{
							CombatAnimationUtils.ClearSkeletonWeaponSlot(skeleton);
						}
					}
				}
				bool flag5 = oldExecutingTeammateCommand >= 0;
				if (flag5)
				{
					bool flag6 = originImplement.IsDefend();
					if (flag6)
					{
						this.DestroyDefendSkillParticleAndSound(teammateId);
					}
					else
					{
						bool flag7 = oldExecutingTeammateCommand == 13;
						if (flag7)
						{
							SkeletonAnimation skeleton2 = isAlly ? this.SelfCurrCharSkeleton : this.EnemyCurrCharSkeleton;
							skeleton2.GetComponent<CombatSpineSkeleton>().commandPrepare.gameObject.SetActive(false);
						}
					}
				}
				bool flag8 = cmdImplement.IsDefend();
				if (flag8)
				{
					short defendCommandSkillId = this.Model.ProcessorCharacters[teammateId].DefendCommandSkillId;
					this.UpdateAffectingDefendSkill(defendSkillRefers, teammateId, isAlly, defendCommandSkillId);
					RectTransform constraintSource = this.GetSkeleton(teammateId).GetComponent<RectTransform>();
					defendSkillRefers.GetComponent<PositionFollower>().Target = constraintSource;
				}
				else
				{
					bool flag9 = originImplement.IsDefend();
					if (flag9)
					{
						defendSkillRefers.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x06008DA6 RID: 36262 RVA: 0x0041FEE0 File Offset: 0x0041E0E0
		private void ShowTeammateCmdBubble(TeammateCommandDisplayData data)
		{
			RectTransform root = (data.IsAlly ? this.selfInfoTop : this.enemyInfoTop).teammateBubbles;
			RectTransform mountPoint = (RectTransform)root.GetChild((int)(data.ValidIndexCharacter * 3 + data.IndexCommand));
			Bubble bubble = CombatPoolAdaptor.GetObject<Bubble>("ui_Combat_CommandBubblePrefab", true);
			RectTransform bubbleRect = (RectTransform)bubble.transform;
			bubbleRect.SetParent(root);
			bubbleRect.SetAsLastSibling();
			bubbleRect.anchorMin = mountPoint.anchorMin;
			bubbleRect.anchorMax = mountPoint.anchorMax;
			bubbleRect.pivot = mountPoint.pivot;
			bubbleRect.anchoredPosition = mountPoint.anchoredPosition;
			bubble.GetComponent<UISwitcher>().Switch(data.IsAlly);
			bubble.gameObject.SetActive(true);
			bubble.SetText(this.GetTeammateCommandBubbleText(data), true);
			TeammateCommandItem config = TeammateCommand.Instance[data.CmdType];
			string sprite = (config.Type == ETeammateCommandType.Negative) ? "sp_bubblebase_1" : "sp_bubblebase_0";
			bubble.GetComponent<CombatBubblePrefab>().back.SetSprite(sprite, false, null);
			CanvasGroup canvas = bubble.GetComponent<CanvasGroup>();
			canvas.alpha = 1f;
			canvas.DOFade(0f, 0.6f).SetDelay(3f).OnComplete(delegate
			{
				CombatPoolAdaptor.Destroy("ui_Combat_CommandBubblePrefab", bubble.gameObject, true);
			});
		}

		// Token: 0x06008DA7 RID: 36263 RVA: 0x00420058 File Offset: 0x0041E258
		private string GetTeammateCommandBubbleText(TeammateCommandDisplayData data)
		{
			string configText = TeammateCommand.Instance[data.CmdType].BubbleText;
			IReadOnlyList<int> team = data.IsAlly ? this._selfTeam : this._enemyTeam;
			int charId = ((int)(data.IndexCharacter + 1) < team.Count) ? team[(int)(data.IndexCharacter + 1)] : -1;
			bool flag = charId < 0;
			string result;
			if (flag)
			{
				result = configText;
			}
			else
			{
				CharacterDisplayData displayData;
				bool flag2 = this._charDisplayDataDict.TryGetValue(charId, out displayData) && displayData.CanNotSpeak;
				if (flag2)
				{
					result = LocalStringManager.Get(LanguageKey.LK_TeammateCommand_Mute);
				}
				else
				{
					result = configText;
				}
			}
			return result;
		}

		// Token: 0x06008DA8 RID: 36264 RVA: 0x004200F8 File Offset: 0x0041E2F8
		private void SwitchPauseState()
		{
			this.SwitchPauseState(!this.IsPausing, ViewCombat.EPauseReason.None);
		}

		// Token: 0x06008DA9 RID: 36265 RVA: 0x0042010C File Offset: 0x0041E30C
		private void SwitchPauseState(bool isPause, ViewCombat.EPauseReason pauseReason = ViewCombat.EPauseReason.None)
		{
			bool flag = !isPause && (this._skillWheelPauseHolding || this._skillSortPauseHolding) && pauseReason != ViewCombat.EPauseReason.SkillWheel && pauseReason != ViewCombat.EPauseReason.SkillSort;
			if (!flag)
			{
				bool flag2 = isPause && this.Model.SelfChangingTrick;
				if (flag2)
				{
					this.OnClickCancelChangeTrick();
				}
				bool flag3 = this.combatTimeScaleToggle.PauseInteractable && (!this._keepPauseUntilCastSkill || !this.combatTimeScaleToggle.IsPaused);
				if (flag3)
				{
					this.combatTimeScaleToggle.SetPause(isPause);
					this._pauseReason = (this.IsPausing ? pauseReason : ViewCombat.EPauseReason.None);
				}
			}
		}

		// Token: 0x06008DAA RID: 36266 RVA: 0x004201AB File Offset: 0x0041E3AB
		private void SetDisplayTimeScale(float timeScale, bool init = false)
		{
			this.combatTimeScaleToggle.SetDisplayTimeScale(timeScale, init);
		}

		// Token: 0x06008DAB RID: 36267 RVA: 0x004201BC File Offset: 0x0041E3BC
		private void UpdateTimeScale(float timeScale)
		{
			this.SetTimeScale(timeScale);
			AudioManager.Instance.SetSoundTimeScale(timeScale);
		}

		// Token: 0x06008DAC RID: 36268 RVA: 0x004201D3 File Offset: 0x0041E3D3
		private void SetTimeScale(float timeScale)
		{
			Time.timeScale = timeScale;
		}

		// Token: 0x06008DAD RID: 36269 RVA: 0x004201E0 File Offset: 0x0041E3E0
		private void EnableBulletTime(bool enable)
		{
			bool flag = this._inBulletTime == enable;
			if (!flag)
			{
				this._inBulletTime = enable;
				CombatDomainMethod.Call.EnableBulletTime(this._inBulletTime);
				this.bulletTimeMask.gameObject.SetActive(this._inBulletTime);
				bool inBulletTime = this._inBulletTime;
				if (inBulletTime)
				{
					this.bulletTimeMask.DOKill(false);
					this.bulletTimeMask.color = Color.white.SetAlpha(0.2f);
					this.bulletTimeMask.DOFade(1f, 10f);
				}
			}
		}

		// Token: 0x06008DAE RID: 36270 RVA: 0x00420274 File Offset: 0x0041E474
		private string GetCombatStateDesc([TupleElementNames(new string[]
		{
			"power",
			"reverse",
			"srcCharId"
		})] Dictionary<short, ValueTuple<short, bool, int>> combatStateDict)
		{
			StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
			strBuilder.Clear();
			foreach (KeyValuePair<short, ValueTuple<short, bool, int>> buff in combatStateDict)
			{
				int id = (int)buff.Key;
				short power = buff.Value.Item1;
				bool reverse = buff.Value.Item2;
				int srcCharId = buff.Value.Item3;
				CombatStateItem configData = CombatState.Instance[id];
				string powerStr = string.Format("({0}%)", power);
				strBuilder.Append(configData.Name + " " + powerStr + " :\n");
				bool flag = configData.PropertyList.Count > 0;
				if (flag)
				{
					foreach (CombatStateProperty propertyChange in configData.PropertyList)
					{
						SpecialEffectDataFieldItem config = SpecialEffectDataField.Instance[propertyChange.SpecialEffectDataId];
						string fieldName = config.Name;
						int value = (int)(propertyChange.Value * power / 100 * (reverse ? -1 : 1));
						string valueStr = ((float)Mathf.Abs(value) / (float)Mathf.Max(config.DisplayDivisor, 1)).ToString(config.DisplayFormat);
						valueStr = ((value < 0) ? "-" : "+") + valueStr + ((propertyChange.ModifyType > 0) ? "%" : "");
						strBuilder.Append(" ·" + fieldName + valueStr + "\n");
					}
				}
				else
				{
					bool flag2 = configData.TipsDesc.Contains("{0}") && srcCharId >= 0;
					if (flag2)
					{
						strBuilder.Append(configData.TipsDesc.GetFormat(NameCenter.GetNameByDisplayData(this._charDisplayDataDict[srcCharId], srcCharId == this._taiwuCharId, false)));
						strBuilder.Append("\n");
					}
					else
					{
						strBuilder.Append(configData.TipsDesc);
						strBuilder.Append("\n");
					}
				}
			}
			string desc = strBuilder.ToString();
			EasyPool.Free<StringBuilder>(strBuilder);
			return desc;
		}

		// Token: 0x06008DAF RID: 36271 RVA: 0x00420504 File Offset: 0x0041E704
		private void UpdateNeiliType(bool isAlly)
		{
			CombatSubProcessorCharacter processor = isAlly ? this.Model.SelfCharacter : this.Model.EnemyCharacter;
			bool flag = processor == null;
			if (!flag)
			{
				CombatInfoTop infoRefers = isAlly ? this.selfInfoTop : this.enemyInfoTop;
				CombatNeiliType neiliTypeComponent = infoRefers.neiliType;
				neiliTypeComponent.Set(processor.NeiliType);
			}
		}

		// Token: 0x06008DB0 RID: 36272 RVA: 0x00420560 File Offset: 0x0041E760
		private void UpdateConsummateLevel(bool isAlly)
		{
			CombatSubProcessorCharacter selfProcessor = this.Model.SelfCharacter;
			CombatSubProcessorCharacter enemyProcessor = this.Model.EnemyCharacter;
			CombatSubProcessorCharacter processor = isAlly ? selfProcessor : enemyProcessor;
			bool flag = processor == null;
			if (!flag)
			{
				CombatInfoTop infoRefers = isAlly ? this.selfInfoTop : this.enemyInfoTop;
				CombatConsummateLevel consummateLevelComponent = infoRefers.consummate;
				consummateLevelComponent.Set(processor.ConsummateLevel);
				bool flag2 = selfProcessor == null || enemyProcessor == null;
				if (!flag2)
				{
					this.combatConfigTipsBack.SetConSummateLevel(selfProcessor.ConsummateLevel, enemyProcessor.ConsummateLevel);
				}
			}
		}

		// Token: 0x06008DB1 RID: 36273 RVA: 0x004205EC File Offset: 0x0041E7EC
		private void OnAutoFightValueChanged(bool isOn)
		{
			bool flag = !this.Model.CanOperateSelfCharacter;
			if (flag)
			{
				bool flag2 = this.autoFight != null && this.autoFight.isOn != this._autoCombat;
				if (flag2)
				{
					this.autoFight.SetWithoutNotify(this._autoCombat);
					GameObject gameObject = this.autoFightEffect;
					if (gameObject != null)
					{
						gameObject.SetActive(this._autoCombat);
					}
				}
			}
			else
			{
				this.SetAutoCombat(isOn);
			}
		}

		// Token: 0x06008DB2 RID: 36274 RVA: 0x00420670 File Offset: 0x0041E870
		private void OnClickAutoFight()
		{
			bool flag = !this.Model.CanOperateSelfCharacter;
			if (!flag)
			{
				this.SetAutoCombat(!this._autoCombat);
			}
		}

		// Token: 0x06008DB3 RID: 36275 RVA: 0x004206A4 File Offset: 0x0041E8A4
		private void SetAutoCombat(bool autoCombat)
		{
			bool flag = this._autoCombat == autoCombat;
			if (flag)
			{
				this.UpdateAutoFightMark(this._autoCombat);
			}
			else
			{
				this._autoCombat = autoCombat;
				this.UpdateAutoFightMark(this._autoCombat);
				this.UpdateTargetDistanceInteract();
				CombatDomainMethod.Call.SetPlayerAutoCombat(this._autoCombat);
				bool autoSaveAutoCombat = this.SettingData.AutoSaveAutoCombat;
				if (autoSaveAutoCombat)
				{
					this.SettingData.AutoCombat = this._autoCombat;
				}
			}
		}

		// Token: 0x06008DB4 RID: 36276 RVA: 0x00420718 File Offset: 0x0041E918
		private void UpdateAutoFightMark(bool autoCombat)
		{
			bool flag = this.autoFight != null && this.autoFight.isOn != autoCombat;
			if (flag)
			{
				this.autoFight.SetWithoutNotify(autoCombat);
			}
			GameObject gameObject = this.autoFightEffect;
			if (gameObject != null)
			{
				gameObject.SetActive(autoCombat);
			}
		}

		// Token: 0x06008DB5 RID: 36277 RVA: 0x0042076C File Offset: 0x0041E96C
		private bool InAttackRange(bool isAlly)
		{
			CombatSubProcessorCharacter processor = isAlly ? this.Model.SelfCharacter : this.Model.EnemyCharacter;
			OuterAndInnerShorts attackRange = (processor != null) ? processor.AttackRange : default(OuterAndInnerShorts);
			int minDist = Mathf.Max((int)this.Model.Config.MinDistance, (int)attackRange.Outer);
			int maxDist = Mathf.Min((int)this.Model.Config.MaxDistance, (int)attackRange.Inner);
			return minDist <= (int)this._distance && (int)this._distance <= maxDist;
		}

		// Token: 0x06008DB6 RID: 36278 RVA: 0x00420804 File Offset: 0x0041EA04
		private void ToggleMoveTarget()
		{
			bool flag = this._selfTargetDistance == -1;
			if (flag)
			{
				CombatDomainMethod.Call.SetTargetDistance(this._distance);
			}
			else
			{
				CombatDomainMethod.Call.ClearTargetDistance();
			}
		}

		// Token: 0x06008DB7 RID: 36279 RVA: 0x00420838 File Offset: 0x0041EA38
		private void UpdateMoveTips()
		{
			bool showTutorial = this.SettingData.ShowCombatTutorial;
			CombatSubProcessorCharacter selfCharacter = this.Model.SelfCharacter;
			OuterAndInnerShorts attackRange = (selfCharacter != null) ? selfCharacter.AttackRange : default(OuterAndInnerShorts);
			bool canOperateSelfCharacter = this.Model.CanOperateSelfCharacter;
			this.selfInfoChar.moveBackwardTips.SetActive(canOperateSelfCharacter && this._distance < attackRange.Outer && showTutorial);
			this.selfInfoChar.moveForwardTips.SetActive(canOperateSelfCharacter && this._distance > attackRange.Inner && showTutorial);
		}

		// Token: 0x06008DB8 RID: 36280 RVA: 0x004208CC File Offset: 0x0041EACC
		private void HideAttackTips()
		{
			this.selfInfoChar.attackTips.SetActive(false);
			this._showAttackTipsTimer = 0f;
		}

		// Token: 0x06008DB9 RID: 36281 RVA: 0x004208EC File Offset: 0x0041EAEC
		private unsafe bool EnemyIsBoss(sbyte bossTemplateId)
		{
			BossItem bossConfig;
			return this.TryGetBossConfig(*this._enemyCurrCharId, out bossConfig) && bossConfig.TemplateId == bossTemplateId;
		}

		// Token: 0x06008DBA RID: 36282 RVA: 0x0042091C File Offset: 0x0041EB1C
		private unsafe bool IsBoss(bool isAlly)
		{
			BossItem bossItem;
			return this.TryGetBossConfig(isAlly ? (*this._selfCurrCharId) : (*this._enemyCurrCharId), out bossItem);
		}

		// Token: 0x06008DBB RID: 36283 RVA: 0x0042094C File Offset: 0x0041EB4C
		private unsafe bool IsAnimal(bool isAlly)
		{
			AnimalItem animalItem;
			return this.TryGetAnimalConfig(isAlly ? (*this._selfCurrCharId) : (*this._enemyCurrCharId), out animalItem);
		}

		// Token: 0x06008DBC RID: 36284 RVA: 0x0042097C File Offset: 0x0041EB7C
		public bool IsCharInCombat(int charId)
		{
			return this._selfTeam.Contains(charId) || this._enemyTeam.Contains(charId);
		}

		// Token: 0x06008DBD RID: 36285 RVA: 0x004209AC File Offset: 0x0041EBAC
		private void UpdateHotkeyText()
		{
			CommandKitBase cmdKit = CommandKitBase.CombatCommandKit;
			StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
			this.GetHotkeyString(CombatCommandKit.MoveBackward, strBuilder);
			this.backwardTipsKey.text = strBuilder.ToString();
			this.GetHotkeyString(CombatCommandKit.MoveForward, strBuilder);
			this.forwardTipsKey.text = strBuilder.ToString();
			this.GetHotkeyString(CombatCommandKit.NormalAttack, strBuilder);
			this.attackTipsKey.text = strBuilder.ToString();
			EasyPool.Free<StringBuilder>(strBuilder);
		}

		// Token: 0x06008DBE RID: 36286 RVA: 0x00420A2C File Offset: 0x0041EC2C
		private void GetHotkeyString(HotKeyCommand cmd, StringBuilder strBuilder)
		{
			KeyCode[] keyCodes = cmd.GetKeyCode(false);
			strBuilder.Clear();
			foreach (KeyCode keyCode in keyCodes)
			{
				bool flag = keyCode > KeyCode.None;
				if (flag)
				{
					bool flag2 = strBuilder.Length > 0;
					if (flag2)
					{
						strBuilder.Append("+");
					}
					strBuilder.Append(cmd.GetKeyCodeString(keyCode));
				}
			}
		}

		// Token: 0x06008DBF RID: 36287 RVA: 0x00420A98 File Offset: 0x0041EC98
		private unsafe void UpdateHealInjuryPoisonBtnTips(TooltipInvoker mouseTip, bool isHealInjury)
		{
			CButton btn = mouseTip.GetComponent<CButton>();
			string tipText = this.BuildHealTipText(isHealInjury);
			mouseTip.PresetParam[1] = tipText;
			mouseTip.Refresh(false, 118);
			int doctorId = *this._selfCurrCharId;
			int patientId = *this._selfCurrCharId;
			if (isHealInjury)
			{
				CombatDomainMethod.AsyncCall.GetHealInjuryBanReason(this, doctorId, patientId, delegate(int offset, RawDataPool pool)
				{
					this.HandlerMethodGetHealBanReason(offset, pool, mouseTip, true, btn.enabled);
				});
			}
			else
			{
				CombatDomainMethod.AsyncCall.GetHealPoisonBanReason(this, doctorId, patientId, delegate(int offset, RawDataPool pool)
				{
					this.HandlerMethodGetHealBanReason(offset, pool, mouseTip, false, btn.enabled);
				});
			}
		}

		// Token: 0x06008DC0 RID: 36288 RVA: 0x00420B34 File Offset: 0x0041ED34
		private unsafe void HandlerMethodGetHealBanReason(int offset, RawDataPool pool, TooltipInvoker mouseTip, bool isHealInjury, bool btnEnabled = false)
		{
			bool flag = !RectTransformUtility.RectangleContainsScreenPoint(mouseTip.GetComponent<RectTransform>(), Input.mousePosition, UIManager.Instance.UiCamera);
			if (!flag)
			{
				uint cannotHealReason = 0U;
				Serializer.Deserialize(pool, offset, ref cannotHealReason);
				CombatSubProcessorCharacter processor = this.Model.ProcessorCharacters.GetValueOrDefault(*this._selfCurrCharId);
				byte healCount = (processor == null) ? 0 : (isHealInjury ? processor.HealInjuryCount : processor.HealPoisonCount);
				StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
				strBuilder.Clear();
				strBuilder.Append(LocalStringManager.Get(isHealInjury ? LanguageKey.LK_Combat_HealInfo : LanguageKey.LK_Combat_DispelInfo));
				bool flag2 = healCount == 0;
				if (flag2)
				{
					strBuilder.Append("\n");
					strBuilder.Append(LocalStringManager.Get(isHealInjury ? LanguageKey.LK_Heal_Injury_Disable_Tips_NoCount : LanguageKey.LK_Heal_Poison_Disable_Tips_NoCount).SetColor("brightred"));
				}
				foreach (ECombatHealBanReason reason in CombatHealBanReasonHelper.ParseReasons(cannotHealReason))
				{
					bool flag3 = reason == ECombatHealBanReason.NonTarget && btnEnabled;
					if (!flag3)
					{
						bool flag4 = reason == ECombatHealBanReason.CountLack;
						if (!flag4)
						{
							strBuilder.Append("\n");
							strBuilder.Append(ViewCombat.<HandlerMethodGetHealBanReason>g__GetReasonStr|578_0(reason, isHealInjury));
						}
					}
				}
				mouseTip.PresetParam[1] = strBuilder.ToString();
				mouseTip.Refresh(false, 118);
				EasyPool.Free<StringBuilder>(strBuilder);
			}
		}

		// Token: 0x06008DC1 RID: 36289 RVA: 0x00420CB4 File Offset: 0x0041EEB4
		private void OnAiOptionShow()
		{
			this.SetSkeletonAndVfxTimePause(true);
			this.combatParticle.SetCameraEnabled(false);
			bool flag = !this.IsPausing;
			if (flag)
			{
				this.SwitchPauseState(true, ViewCombat.EPauseReason.AiOptions);
			}
		}

		// Token: 0x06008DC2 RID: 36290 RVA: 0x00420CF0 File Offset: 0x0041EEF0
		private void OnAiOptionsHide()
		{
			this.SetSkeletonAndVfxTimePause(false);
			this.combatParticle.SetCameraEnabled(true);
			bool flag = this._pauseReason == ViewCombat.EPauseReason.AiOptions;
			if (flag)
			{
				this.SwitchPauseState(false, ViewCombat.EPauseReason.None);
			}
		}

		// Token: 0x06008DC3 RID: 36291 RVA: 0x00420D2C File Offset: 0x0041EF2C
		private void OnCirclePanelShow()
		{
			this.SetSkeletonAndVfxTimePause(true);
			this.combatParticle.SetCameraEnabled(false);
			bool flag = !this.IsPausing;
			if (flag)
			{
				this.SwitchPauseState(true, ViewCombat.EPauseReason.CirclePanel);
			}
		}

		// Token: 0x06008DC4 RID: 36292 RVA: 0x00420D68 File Offset: 0x0041EF68
		private void OnCirclePanelHide()
		{
			this.SetSkeletonAndVfxTimePause(false);
			this.combatParticle.SetCameraEnabled(true);
			bool flag = this._pauseReason == ViewCombat.EPauseReason.CirclePanel;
			if (flag)
			{
				this.SwitchPauseState(false, ViewCombat.EPauseReason.None);
			}
		}

		// Token: 0x06008DC5 RID: 36293 RVA: 0x00420DA4 File Offset: 0x0041EFA4
		private void OnDamageDetailShow()
		{
			bool flag = !this.IsPausing;
			if (flag)
			{
				this.SwitchPauseState(true, ViewCombat.EPauseReason.DamageDetail);
			}
		}

		// Token: 0x06008DC6 RID: 36294 RVA: 0x00420DC8 File Offset: 0x0041EFC8
		private void OnDamageDetailHide()
		{
			bool flag = this._pauseReason == ViewCombat.EPauseReason.DamageDetail;
			if (flag)
			{
				this.SwitchPauseState(false, ViewCombat.EPauseReason.None);
			}
		}

		// Token: 0x06008DC7 RID: 36295 RVA: 0x00420DEC File Offset: 0x0041EFEC
		private unsafe IEnumerator ChangeCombatChar(int fromCharId, int toCharId)
		{
			bool isAlly = this._selfTeam.Contains(fromCharId);
			bool changeToMainChar = toCharId == (isAlly ? this._selfTeam : this._enemyTeam)[0];
			SkeletonAnimation skeletonOld = this.GetSkeleton(fromCharId);
			SkeletonAnimation skeletonNew = this.GetSkeleton(toCharId);
			float skeletonX = this._skeletonMoveInfoDict.GetOrDefault(skeletonOld).Pos;
			float aniTime = CombatUtils.ChangeCharacterWaitTime();
			bool autoFightMark = changeToMainChar ? this._autoCombat : (!this.Model.CanOperateCharacter(toCharId) || this._autoCombat);
			bool flag = isAlly;
			if (flag)
			{
				this.UpdateAutoFightMark(autoFightMark);
			}
			this._virtualCamera.LockPosition();
			this._virtualCamera.LockScale();
			int toCharLastDisplayPos;
			bool flag2 = this._displayPosDict.TryGetValue(toCharId, out toCharLastDisplayPos) && toCharLastDisplayPos == int.MinValue;
			if (flag2)
			{
				this.SetSkeletonPos(skeletonNew, skeletonX + (float)(-1500 * (isAlly ? 1 : -1)), 0f, false, true);
				yield return null;
			}
			this.SetSkeletonPos(skeletonOld, skeletonX + (float)(-1500 * (isAlly ? 1 : -1)), aniTime, false, false);
			skeletonNew.Skeleton.SetBonesToSetupPose();
			skeletonNew.gameObject.SetActive(true);
			bool flag3 = isAlly;
			if (flag3)
			{
				int num;
				for (int i = 0; i < this._selfWeaponHolder.childCount; i = num + 1)
				{
					this._selfWeaponHolder.GetChild(i).GetComponent<CButton>().interactable = false;
					num = i;
				}
				CButton healInjuryBtn = this._selfOtherActionHolder.otherActionTypeList[0];
				CButton healPoisonBtn = this._selfOtherActionHolder.otherActionTypeList[1];
				for (int j = 0; j < this._selfOtherActionHolder.transform.childCount; j = num + 1)
				{
					CButton actionBtn;
					bool hasButton = this._selfOtherActionHolder.transform.GetChild(j).TryGetComponent<CButton>(out actionBtn);
					bool flag4 = hasButton;
					if (flag4)
					{
						actionBtn.interactable = ((changeToMainChar || this.Model.CanOperateCharacter(toCharId)) && actionBtn != healInjuryBtn && actionBtn != healPoisonBtn);
						bool flag5 = actionBtn == healInjuryBtn || actionBtn == healPoisonBtn;
						if (flag5)
						{
							this.UpdateHealInjuryPoisonBtnTips(actionBtn.GetComponent<TooltipInvoker>(), actionBtn == healInjuryBtn);
						}
					}
					actionBtn = null;
					num = j;
				}
				this.HideAttackTips();
				healInjuryBtn = null;
				healPoisonBtn = null;
			}
			base.RemoveMonitorFieldId(8, 10, (ulong)toCharId);
			yield return new WaitForSeconds(aniTime + 0.5f);
			base.AppendMonitorFieldId(new UIBase.MonitorDataField(8, 10, (ulong)toCharId, this._combatCharSubIds));
			this.Model.RequestProactiveSkillList(toCharId);
			RectTransform constraintSource = skeletonNew.GetComponent<RectTransform>();
			bool flag6 = isAlly;
			if (flag6)
			{
				*this._selfCurrCharId = toCharId;
				this._selfDefeatMarkInitialized = false;
				this.selfInfoChar.GetComponent<PositionFollower>().Target = constraintSource;
				this.combatQuickUseItemPanel.Setup(*this._selfCurrCharId);
			}
			else
			{
				*this._enemyCurrCharId = toCharId;
				this._enemyDefeatMarkInitialized = false;
				this.enemyInfoChar.GetComponent<PositionFollower>().Target = constraintSource;
			}
			this.UpdateNeiliType(isAlly);
			this.UpdateConsummateLevel(isAlly);
			this.OnWeaponsChanged(isAlly);
			this.OnSkillEffectCollectionChanged(isAlly);
			CombatSubProcessorCharacter orDefault = this.Model.ProcessorCharacters.GetOrDefault(fromCharId);
			this.OnDefeatMarkCollectionChanged(toCharId, (orDefault != null) ? orDefault.DefeatMarkCollection : null);
			this.OnOtherActionCanUseChanged(isAlly);
			this.OnUsingWeaponIndexChanged(isAlly);
			this.OnPreparingItemChanged(isAlly);
			this.StopLoopSoundForCharacter(fromCharId);
			this.DestroyDefendSkillParticleAndSound(fromCharId);
			this.OnCombatReserveDataChanged(toCharId, this.Model.ProcessorCharacters.GetOrDefault(toCharId).CombatReserveData);
			ItemKey[] array;
			if (!isAlly)
			{
				CombatSubProcessorCharacter enemyCharacter = this.Model.EnemyCharacter;
				array = ((enemyCharacter != null) ? enemyCharacter.Weapons : null);
			}
			else
			{
				CombatSubProcessorCharacter selfCharacter = this.Model.SelfCharacter;
				array = ((selfCharacter != null) ? selfCharacter.Weapons : null);
			}
			ItemKey[] weaponList = array;
			bool flag7 = weaponList != null;
			if (flag7)
			{
				foreach (ItemKey itemKey in weaponList)
				{
					bool flag8 = !itemKey.IsValid();
					if (!flag8)
					{
						this.OnWeaponCanChangeToChanged(itemKey);
						this.OnWeaponCdFrameRelatedDataChanged(itemKey);
						this.OnWeaponDurabilityChanged(itemKey);
						itemKey = default(ItemKey);
					}
				}
				ItemKey[] array2 = null;
			}
			skeletonOld.GetComponent<CombatSpineSkeleton>().currPosMark.SetSprite(this._currPosSprite[isAlly ? 0 : 1], false, null);
			skeletonOld.gameObject.SetActive(false);
			this.SetSkeletonPos(skeletonNew, skeletonX, aniTime, false, false);
			DOVirtual.DelayedCall(aniTime, delegate
			{
				this.SetPosition(true, this._selfPos);
				this._virtualCamera.UnlockPosition();
				this._virtualCamera.UnlockScale();
			}, true);
			yield break;
		}

		// Token: 0x06008DC8 RID: 36296 RVA: 0x00420E09 File Offset: 0x0041F009
		private IEnumerator CombatOver()
		{
			sbyte statusType = this.Model.CombatStatus;
			AudioManager.Instance.PlayMusic(AudioManager.DummyAudioName, 1f, 100, null);
			this.combatTimeScaleToggle.PauseInteractable = false;
			this.UpdateTimeScale(1f);
			this._virtualCamera.LockPosition();
			this._virtualCamera.LockScale();
			bool flag = statusType == 4;
			if (flag)
			{
				this.SelfCurrCharSkeleton.transform.DOLocalMoveX(this.SelfCurrCharSkeleton.transform.localPosition.x + -1500f, 1.5f, false).SetDelay(0.1f);
			}
			else
			{
				bool flag2 = statusType == 5;
				if (flag2)
				{
					this.EnemyCurrCharSkeleton.transform.DOLocalMoveX(this.EnemyCurrCharSkeleton.transform.localPosition.x - -1500f, 1.5f, false).SetDelay(0.1f);
				}
			}
			CombatResultDisplayData result = null;
			bool responded = false;
			CombatDomainMethod.AsyncCall.GetCombatResultDisplayData(this, delegate(int offset, RawDataPool pool)
			{
				result = new CombatResultDisplayData();
				Serializer.Deserialize(pool, offset, ref result);
				responded = true;
				bool isWin = result.IsWin;
				if (isWin)
				{
					foreach (RectTransform tips in this._tipsList)
					{
						bool flag3 = tips != null && tips.gameObject != null && tips.gameObject.activeSelf;
						if (flag3)
						{
							tips.gameObject.SetActive(false);
						}
					}
					this.combatDropsPrefab.PlayDropAniCoroutine(result, this.enemyInfoChar, this.EnemyCurrCharSkeleton);
				}
			});
			yield return new WaitUntil(() => responded);
			Tester.Assert(result != null, "");
			yield return new WaitForSeconds(2f);
			ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
			sbyte combatResult = ViewCombat.GetCombatResultTypeByCombatResultType(statusType, this.Model.NotExecuted);
			this.ClearAllCharParticleAndSound();
			argsBox.Set("CombatResult", combatResult);
			argsBox.Set("CombatType", this._combatType);
			argsBox.Set("MainEnemyId", this._enemyTeam[0]);
			argsBox.Set("MainEnemyTemplateId", this._charDisplayDataDict[this._enemyTeam[0]].TemplateId);
			argsBox.SetObject("DisplayData", result);
			UIElement.CombatResult.SetOnInitArgs(argsBox);
			UIManager.Instance.MaskUI(UIElement.CombatResult);
			this.ResetMobilityAvoid();
			yield break;
		}

		// Token: 0x06008DC9 RID: 36297 RVA: 0x00420E18 File Offset: 0x0041F018
		public static sbyte GetCombatResultTypeByCombatResultType(sbyte statusType, bool notExecuted = true)
		{
			bool flag = statusType == 3;
			sbyte combatResult;
			if (flag)
			{
				combatResult = (notExecuted ? 0 : 5);
			}
			else
			{
				bool flag2 = statusType == 2;
				if (flag2)
				{
					combatResult = (notExecuted ? 1 : 4);
				}
				else
				{
					combatResult = ((statusType == 4) ? 2 : 3);
				}
			}
			return combatResult;
		}

		// Token: 0x06008DCA RID: 36298 RVA: 0x00420E58 File Offset: 0x0041F058
		private void ClearAllCharParticleAndSound()
		{
			foreach (ParticleSystem particle in this._defendParticleDict.Values)
			{
				Object.Destroy(particle.gameObject);
			}
			this._defendParticleDict.Clear();
			foreach (string soundName in this._defendSoundDict.Values)
			{
				AudioManager.Instance.StopSound(soundName);
			}
			this._defendSoundDict.Clear();
			this.combatParticle.CleanupAllParticles();
			foreach (string soundName2 in this._loopSoundDict.Values)
			{
				AudioManager.Instance.StopSound(soundName2);
			}
			this._loopSoundDict.Clear();
		}

		// Token: 0x06008DCB RID: 36299 RVA: 0x00420F88 File Offset: 0x0041F188
		private void OnTopUiChanged(ArgumentBox argsBox = null)
		{
			bool flag = !this._showingSystemOption && UIManager.Instance.IsFocusElement(UIElement.SystemOption);
			if (flag)
			{
				this._showingSystemOption = true;
				bool flag2 = !this.IsPausing;
				if (flag2)
				{
					this._pausedBySystemMenu = true;
					this.OnShowConfirmDialog(true);
				}
				else
				{
					this._pausedBySystemMenu = false;
				}
			}
			else
			{
				bool flag3 = this._showingSystemOption && UIManager.Instance.IsFocusElement(UIElement.StateCombat);
				if (flag3)
				{
					this._showingSystemOption = false;
					this.UpdateHotkeyText();
					bool pausedBySystemMenu = this._pausedBySystemMenu;
					if (pausedBySystemMenu)
					{
						this.OnShowConfirmDialog(false);
					}
					this._pausedBySystemMenu = false;
				}
			}
		}

		// Token: 0x06008DCC RID: 36300 RVA: 0x00421034 File Offset: 0x0041F234
		private void OnGameStateChange(ArgumentBox argBox)
		{
			Enum state;
			argBox.Get("newState", out state);
			bool flag = (EGameState)state == EGameState.Loading;
			if (flag)
			{
				Time.timeScale = 1f;
				CombatDomainMethod.Call.ClearAllReserveAction();
				this.ClearAllCharParticleAndSound();
			}
		}

		// Token: 0x06008DCD RID: 36301 RVA: 0x00421078 File Offset: 0x0041F278
		private void OnConfirmQuitGameState(ArgumentBox argBox)
		{
			bool showingSystemOption = this._showingSystemOption;
			if (!showingSystemOption)
			{
				bool show;
				argBox.Get("ShowState", out show);
				this.OnShowConfirmDialog(show);
			}
		}

		// Token: 0x06008DCE RID: 36302 RVA: 0x004210A8 File Offset: 0x0041F2A8
		private void OnNeedCombatPause(ArgumentBox argBox)
		{
			this.Pause();
		}

		// Token: 0x06008DCF RID: 36303 RVA: 0x004210B2 File Offset: 0x0041F2B2
		private void OnNeedCombatResume(ArgumentBox argBox)
		{
			this.Resume();
		}

		// Token: 0x06008DD0 RID: 36304 RVA: 0x004210BC File Offset: 0x0041F2BC
		private void PlayCombatSoundOnce(ArgumentBox argBox)
		{
			string soundName;
			argBox.Get("soundName", out soundName);
			this.SetSoundOnce(soundName, 100);
		}

		// Token: 0x06008DD1 RID: 36305 RVA: 0x004210E4 File Offset: 0x0041F2E4
		private void ShowCombatTeammateCommand(ArgumentBox argBox)
		{
			TeammateCommandDisplayData data;
			argBox.Get<TeammateCommandDisplayData>("data", out data);
			this.ShowTeammateCmdBubble(data);
		}

		// Token: 0x06008DD2 RID: 36306 RVA: 0x00421108 File Offset: 0x0041F308
		private void ShowCombatSpecialEffectTips(ArgumentBox argBox)
		{
			bool isAlly;
			argBox.Get("isAlly", out isAlly);
			ShowSpecialEffectDisplayData data;
			argBox.Get<ShowSpecialEffectDisplayData>("data", out data);
			this.ShowSpecialEffectTip(isAlly, data);
		}

		// Token: 0x06008DD3 RID: 36307 RVA: 0x0042113C File Offset: 0x0041F33C
		private unsafe void ShowCombatSurrenderMark(ArgumentBox argBox)
		{
			List<int> showData;
			argBox.Get<List<int>>("showData", out showData);
			foreach (int data in showData)
			{
				DefeatMarkKey markKey = (DefeatMarkKey)data;
				this.ShowIconTips(*this._selfCurrCharId, this.GetDropMarkIcon(markKey), "");
			}
		}

		// Token: 0x06008DD4 RID: 36308 RVA: 0x004211B8 File Offset: 0x0041F3B8
		private void ShowFleeAnimation(ArgumentBox argBox)
		{
			int failCharId;
			argBox.Get("failCharId", out failCharId);
			string failAnimation;
			argBox.Get("failAnimation", out failAnimation);
			this.PlayAni(failCharId, failAnimation, false);
			this._virtualCamera.LockPosition();
			this._virtualCamera.LockScale();
			bool ally = failCharId == this.Model.SelfCharId;
			SkeletonAnimation skeleton = this.GetSkeleton(failCharId);
			float skeletonX = this._skeletonMoveInfoDict.GetOrDefault(skeleton).Pos;
			float aniTime = CombatUtils.ChangeCharacterWaitTime();
			this.SetSkeletonPos(skeleton, skeletonX + (float)(-1500 * (ally ? 1 : -1)), aniTime, false, false);
		}

		// Token: 0x06008DD5 RID: 36309 RVA: 0x00421254 File Offset: 0x0041F454
		private unsafe void ShowCombatTianSuiBaoLu(ArgumentBox argBox)
		{
			bool isDirect;
			argBox.Get("isDirect", out isDirect);
			int charId;
			argBox.Get("charId", out charId);
			bool isAlly = this.Model.CharIsAlly(charId);
			bool isCurrent = charId == (isAlly ? (*this._selfCurrCharId) : (*this._enemyCurrCharId));
			bool flag = !isCurrent;
			if (!flag)
			{
				RectTransform skeleton = this.GetSkeleton(charId).GetComponent<RectTransform>();
				this.combatUIParticle.PlayTianSuiBaoLuParticle(isAlly, skeleton, this._selfOtherActionHolder.useItem.GetComponent<RectTransform>(), isDirect);
			}
		}

		// Token: 0x06008DD6 RID: 36310 RVA: 0x004212DC File Offset: 0x0041F4DC
		private void ShowAbsorbNeiliAllocation(ArgumentBox argBox)
		{
			int targetId;
			argBox.Get("targetId", out targetId);
			int absorbCharId;
			argBox.Get("absorbCharId", out absorbCharId);
			byte neiliAllocationType;
			argBox.Get("neiliAllocationType", out neiliAllocationType);
			RectTransform fromSkeleton = this.GetSkeleton(targetId).GetComponent<RectTransform>();
			RectTransform toSkeleton = this.GetSkeleton(absorbCharId).GetComponent<RectTransform>();
			this.combatUIParticle.PlayAbsorbNeiliAllocation(fromSkeleton, toSkeleton, neiliAllocationType);
		}

		// Token: 0x06008DD7 RID: 36311 RVA: 0x0042133F File Offset: 0x0041F53F
		private void CombatTutorialSettingChanged(ArgumentBox argBox)
		{
			this.UpdateMoveTips();
		}

		// Token: 0x06008DD8 RID: 36312 RVA: 0x00421348 File Offset: 0x0041F548
		private void OnShowConfirmDialog(bool show)
		{
			if (show)
			{
				this.SwitchPauseState(true, ViewCombat.EPauseReason.Dialog);
			}
			else
			{
				bool flag = this._pauseReason == ViewCombat.EPauseReason.Dialog;
				if (flag)
				{
					this.SwitchPauseState(false, ViewCombat.EPauseReason.None);
				}
			}
			this.combatParticle.SetCameraEnabled(!show);
		}

		// Token: 0x06008DD9 RID: 36313 RVA: 0x00421390 File Offset: 0x0041F590
		public void Pause()
		{
			this.combatTimeScaleToggle.SetPause(true);
		}

		// Token: 0x06008DDA RID: 36314 RVA: 0x004213A0 File Offset: 0x0041F5A0
		public void Resume()
		{
			bool flag = this._keepPauseUntilCastSkill || this._skillWheelPauseHolding;
			if (!flag)
			{
				this.combatTimeScaleToggle.SetPause(false);
			}
		}

		// Token: 0x06008DDB RID: 36315 RVA: 0x004213D4 File Offset: 0x0041F5D4
		private void RefreshUsingItemButtonEffect(SkillEffectCollection effectCollection, bool isAlly)
		{
			bool flag = !isAlly;
			if (!flag)
			{
				this._selfSpecialWisdomCount = 0;
				bool flag2 = effectCollection.EffectDict != null;
				if (flag2)
				{
					foreach (KeyValuePair<SkillEffectKey, short> pair in effectCollection.EffectDict)
					{
						CombatSkillItem combatSkillItem = CombatSkill.Instance[pair.Key.SkillId];
						int effectId = pair.Key.IsDirect ? combatSkillItem.DirectEffectID : combatSkillItem.ReverseEffectID;
						SpecialEffectItem specialEffectItem = SpecialEffect.Instance[effectId];
						bool showUsingItemButtonEffect = specialEffectItem.ShowUsingItemButtonEffect;
						if (showUsingItemButtonEffect)
						{
							this._selfSpecialWisdomCount = pair.Value;
							break;
						}
					}
				}
				this.combatQuickUseItemPanel.useItemEffect.SetActive(this._selfSpecialWisdomCount > 0);
				this.RefreshUsingItemButtonWisdomCount();
			}
		}

		// Token: 0x06008DDC RID: 36316 RVA: 0x004214D0 File Offset: 0x0041F6D0
		private void RefreshUsingItemButtonWisdomCount()
		{
			this.combatWheel.SetUseItemWisdomCount(this.Model.SelfTeamWisdomCount, this._selfSpecialWisdomCount);
		}

		// Token: 0x06008DDD RID: 36317 RVA: 0x004214F0 File Offset: 0x0041F6F0
		private void ShowSelectItemInCombat()
		{
			this.combatQuickUseItemPanel.Hide(false, true);
			ArgumentBox argBox = this.GetUseItemPanelArgs();
			this._selectingUseItem = true;
			bool flag = !this.IsPausing;
			if (flag)
			{
				this.SwitchPauseState();
				this.combatUseItemPanel.Show(argBox, delegate
				{
					this._selectingUseItem = false;
					this.SwitchPauseState();
				});
			}
			else
			{
				this.combatUseItemPanel.Show(argBox, delegate
				{
					this._selectingUseItem = false;
				});
			}
		}

		// Token: 0x06008DDE RID: 36318 RVA: 0x00421568 File Offset: 0x0041F768
		private void OnUseItem(ItemKey itemOrToolKey, sbyte useType, ItemKey toolTargetKey, List<sbyte> targetBodyParts = null)
		{
			bool flag = itemOrToolKey.ItemType == 6;
			if (flag)
			{
				this.DoRequestRepairItem(itemOrToolKey, toolTargetKey);
			}
			else
			{
				this.DoRequestUseItem(itemOrToolKey, useType, targetBodyParts);
			}
			this.HideAttackTips();
			this.ClearPreviewRangeItem();
		}

		// Token: 0x06008DDF RID: 36319 RVA: 0x004215A7 File Offset: 0x0041F7A7
		private void ClearPreviewRangeItem()
		{
			this.UpdatePreviewRangeItem(new OuterAndInnerShorts(-1, -1));
		}

		// Token: 0x06008DE0 RID: 36320 RVA: 0x004215B8 File Offset: 0x0041F7B8
		private unsafe ArgumentBox GetUseItemPanelArgs()
		{
			CombatSubProcessorTaiwu processorTaiwu = this.Model.ProcessorTaiwu;
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("WisdomType", this.Model.SelfTeamWisdomType);
			argBox.Set("WisdomCount", this.Model.SelfTeamWisdomCount);
			argBox.Set("SpecialWisdomCount", this._selfSpecialWisdomCount);
			argBox.Set("CanEatMore", processorTaiwu.EatingItems.GetAvailableEatingSlot(this._selfMaxEatingSlotCount) >= 0);
			argBox.Set("CanUseSwordFragment", processorTaiwu.XiangshuInfection < 200);
			argBox.SetObject("MainAttributes", processorTaiwu.CurrMainAttributes);
			ArgumentBox argumentBox = argBox;
			string key = "WeaponTricks";
			CombatSubProcessorCharacter selfCharacter = this.Model.SelfCharacter;
			argumentBox.SetObject(key, (selfCharacter != null) ? selfCharacter.WeaponTricks : null);
			argBox.SetObject("CallBack", new Action<ItemKey, sbyte, ItemKey, List<sbyte>>(this.OnUseItem));
			argBox.SetObject("UpdateAttackRangePreview", new Action<OuterAndInnerShorts>(this.UpdatePreviewRangeItem));
			argBox.Set("CharId", *this._selfCurrCharId);
			return argBox;
		}

		// Token: 0x06008DE1 RID: 36321 RVA: 0x004216D8 File Offset: 0x0041F8D8
		private void ShowQuickUseItemPanel()
		{
			ArgumentBox argBox = this.GetUseItemPanelArgs();
			this._selectingUseItem = true;
			this.combatQuickUseItemPanel.Show(argBox, delegate
			{
				this._selectingUseItem = false;
				this.ClearPreviewRangeItem();
			});
			CharacterDomainMethod.AsyncCall.GetAllEquipmentItems(null, this._taiwuCharId, delegate(int offset, RawDataPool pool)
			{
				List<ItemDisplayData> equipments = null;
				Serializer.Deserialize(pool, offset, ref equipments);
				bool flag = equipments == null;
				if (!flag)
				{
					ItemDisplayData beastItem = equipments[13];
					CButton button = this._selfOtherActionHolder.otherActionTypeList[3];
					bool showButton = this._hunterSkillUnlocked && beastItem.Key.IsValid();
					button.gameObject.SetActive(showButton);
					this.combatWheel.SetOtherActionButtonVisible(3, showButton);
					this.combatWheel.SyncOtherActionButtons();
					bool flag2 = !showButton;
					if (!flag2)
					{
						button.GetComponent<CombatOtherActionType>().count.text = beastItem.Durability.ToString();
						button.interactable = (beastItem.Durability >= 30);
						TooltipInvoker tip = button.GetComponent<TooltipInvoker>();
						bool showing = tip.Showing;
						if (showing)
						{
							tip.Refresh(false, -1);
						}
					}
				}
			});
		}

		// Token: 0x06008DE2 RID: 36322 RVA: 0x00421728 File Offset: 0x0041F928
		private static string LocalizeAvoidParticle(string particleName)
		{
			bool flag = !CombatAnimationConstants.SelfAvoidParticle.Contains(particleName) && !CombatAnimationConstants.EnemyAvoidParticle.Contains(particleName);
			string result;
			if (flag)
			{
				result = particleName;
			}
			else
			{
				string suffix = LocalStringManager.CurLanguageType.ToString().ToLower();
				string localized = particleName + "_" + suffix;
				bool flag2 = CombatPoolAdaptor.HasObject(localized);
				if (flag2)
				{
					result = localized;
				}
				else
				{
					bool flag3 = suffix != "cn";
					if (flag3)
					{
						string cnFallback = particleName + "_cn";
						bool flag4 = CombatPoolAdaptor.HasObject(cnFallback);
						if (flag4)
						{
							return cnFallback;
						}
					}
					result = particleName;
				}
			}
			return result;
		}

		// Token: 0x06008DE3 RID: 36323 RVA: 0x004217CA File Offset: 0x0041F9CA
		IEnumerator IPreloadElement.Preload()
		{
			bool flag = ViewCombat.CharId2BossId.Count == 0;
			if (flag)
			{
				sbyte bossId = 0;
				while ((int)bossId < Boss.Instance.Count)
				{
					short[] charIdList = Boss.Instance[bossId].CharacterIdList;
					foreach (short charId in charIdList)
					{
						ViewCombat.CharId2BossId[charId] = bossId;
					}
					short[] array = null;
					charIdList = null;
					sbyte b = bossId;
					bossId = b + 1;
				}
			}
			bool flag2 = ViewCombat.MoveSkillAniSet.Count == 0;
			if (flag2)
			{
				short i = 0;
				while ((int)i < CombatSkill.Instance.Count)
				{
					CombatSkillItem skillConfig = CombatSkill.Instance[i];
					bool flag3 = skillConfig.JumpAni != null;
					if (flag3)
					{
						ViewCombat.MoveSkillAniSet.Add(skillConfig.JumpAni[0]);
						ViewCombat.MoveSkillAniSet.Add(skillConfig.JumpAni[1]);
					}
					skillConfig = null;
					short num = i;
					i = num + 1;
				}
			}
			CombatPoolAdaptor.Preload("ui_Combat_DefeatMarkPrefab", this.defeatMarkPrefab, 108);
			ViewCombat.<>c__DisplayClass613_0 CS$<>8__locals1 = new ViewCombat.<>c__DisplayClass613_0();
			ResLoader.Load<SkeletonDataAsset>("RemakeResources/SpineAnimations/Character/skeleton_skin_SkeletonData", delegate(SkeletonDataAsset asset)
			{
				this._actorSkeleton = asset;
				this._actorSkeleton.GetSkeletonData(false);
			}, null, true);
			CS$<>8__locals1.animations = null;
			CS$<>8__locals1.particles = null;
			CS$<>8__locals1.sounds = null;
			ResLoader.Load<AssetBundle>("combatbase.uab", delegate(AssetBundle bundle)
			{
				AssetBundleRequest reqAnim = bundle.LoadAllAssetsAsync<RawAnimationAsset>();
				reqAnim.completed += delegate(AsyncOperation _)
				{
					CS$<>8__locals1.animations = reqAnim.allAssets.Cast<RawAnimationAsset>().ToArray<RawAnimationAsset>();
				};
				AssetBundleRequest reqParticles = bundle.LoadAllAssetsAsync<GameObject>();
				reqParticles.completed += delegate(AsyncOperation _)
				{
					CS$<>8__locals1.particles = reqParticles.allAssets.Cast<GameObject>().ToArray<GameObject>();
				};
				AssetBundleRequest reqSounds = bundle.LoadAllAssetsAsync<AudioClip>();
				reqSounds.completed += delegate(AsyncOperation _)
				{
					CS$<>8__locals1.sounds = reqSounds.allAssets.Cast<AudioClip>().ToArray<AudioClip>();
				};
			}, null, true);
			WaitUntil waitSkeleton = new WaitUntil(() => null != this._actorSkeleton);
			yield return waitSkeleton;
			TimeSpan maxExecuteDuration = TimeSpan.FromMilliseconds(32.0);
			DateTime startTime = DateTime.UtcNow;
			foreach (SkeletonAnimation skeleton in this.selfSkeletonList)
			{
				skeleton.skeletonDataAsset = this._actorSkeleton;
				skeleton.Initialize(true, false);
				bool flag4 = DateTime.UtcNow - startTime > maxExecuteDuration;
				if (flag4)
				{
					yield return null;
					startTime = DateTime.UtcNow;
				}
				skeleton = null;
			}
			List<SkeletonAnimation>.Enumerator enumerator = default(List<SkeletonAnimation>.Enumerator);
			startTime = DateTime.UtcNow;
			foreach (SkeletonAnimation skeleton2 in this.enemySkeletonList)
			{
				skeleton2.skeletonDataAsset = this._actorSkeleton;
				skeleton2.Initialize(true, false);
				bool flag5 = DateTime.UtcNow - startTime > maxExecuteDuration;
				if (flag5)
				{
					yield return null;
					startTime = DateTime.UtcNow;
				}
				skeleton2 = null;
			}
			List<SkeletonAnimation>.Enumerator enumerator2 = default(List<SkeletonAnimation>.Enumerator);
			WaitUntil waitBaseRess = new WaitUntil(() => CS$<>8__locals1.animations != null && CS$<>8__locals1.particles != null && CS$<>8__locals1.sounds != null);
			yield return waitBaseRess;
			RectTransform particleHolder = this.particlePrefabHolder;
			foreach (RawAnimationAsset rawAnimation in CS$<>8__locals1.animations)
			{
				this._commonAniDict.Add(rawAnimation.animName, rawAnimation);
				rawAnimation = null;
			}
			RawAnimationAsset[] array2 = null;
			startTime = DateTime.UtcNow;
			foreach (GameObject particle in CS$<>8__locals1.particles)
			{
				GameObject poolObj = Object.Instantiate<GameObject>(particle, particleHolder);
				this._commonParticleNameList.Add(particle.name);
				CombatPoolAdaptor.SetSrcObject(particle.name, poolObj, 0);
				bool flag6 = DateTime.UtcNow - startTime > maxExecuteDuration;
				if (flag6)
				{
					yield return null;
					startTime = DateTime.UtcNow;
				}
				poolObj = null;
				particle = null;
			}
			GameObject[] array3 = null;
			foreach (AudioClip sound in CS$<>8__locals1.sounds)
			{
				this._commonSoundDict.Add(sound.name, sound);
				sound = null;
			}
			AudioClip[] array4 = null;
			CS$<>8__locals1 = null;
			waitSkeleton = null;
			maxExecuteDuration = default(TimeSpan);
			waitBaseRess = null;
			particleHolder = null;
			yield break;
			yield break;
		}

		// Token: 0x06008DE4 RID: 36324 RVA: 0x004217DC File Offset: 0x0041F9DC
		private void SummaryDamageNum(int charId, DefeatMarkKey markKey, int damageValue = 1)
		{
			bool ally = this.Model.CharIsAlly(charId);
			EMarkType type = markKey.Type;
			if (!true)
			{
			}
			int num;
			switch (type)
			{
			case EMarkType.Outer:
				num = (ally ? 25 : 15);
				break;
			case EMarkType.Inner:
				num = (ally ? 26 : 16);
				break;
			case EMarkType.Flaw:
				num = (ally ? 30 : 20);
				break;
			case EMarkType.Acupoint:
				num = (ally ? 31 : 21);
				break;
			case EMarkType.Poison:
				num = (ally ? 32 : 22);
				break;
			case EMarkType.Mind:
				num = (ally ? 29 : 19);
				break;
			case EMarkType.Fatal:
				num = (ally ? 27 : 17);
				break;
			case EMarkType.Die:
				num = (ally ? 33 : 23);
				break;
			default:
				num = -1;
				break;
			}
			if (!true)
			{
			}
			int summaryType = num;
			bool flag = summaryType < 0;
			if (!flag)
			{
				TaiwuDomainMethod.Call.RecordLifeSummary(summaryType, damageValue);
			}
		}

		// Token: 0x06008E32 RID: 36402 RVA: 0x0042229A File Offset: 0x0042049A
		[CompilerGenerated]
		private void <UpdateReserveStatus>g__ShowUseItem|504_0()
		{
			this.reserveTips.Refresh(CombatReserveController.EReserveType.OtherAction, this.otherActionReserveRoot);
		}

		// Token: 0x06008E33 RID: 36403 RVA: 0x004222B0 File Offset: 0x004204B0
		[CompilerGenerated]
		private void <UpdateWeaponCdImageInner>g__SetCountDownTextActive|523_0(bool active, ref ViewCombat.<>c__DisplayClass523_0 A_2)
		{
			TextMeshProUGUI countDownText = A_2.weaponRefers.countDownText;
			if (active)
			{
				bool flag = !countDownText.gameObject.activeSelf;
				if (flag)
				{
					countDownText.gameObject.SetActive(true);
				}
			}
			else
			{
				bool activeSelf = countDownText.gameObject.activeSelf;
				if (activeSelf)
				{
					countDownText.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06008E34 RID: 36404 RVA: 0x00422310 File Offset: 0x00420510
		[CompilerGenerated]
		private void <UpdateWeaponCdImageInner>g__HandleCd|523_1(ref ViewCombat.<>c__DisplayClass523_0 A_1)
		{
			CombatSubProcessorCharacter character = A_1.isAlly ? this.Model.SelfCharacter : this.Model.EnemyCharacter;
			bool flag = character == null;
			if (!flag)
			{
				bool flag2 = character.Weapons.Length <= A_1.index;
				if (!flag2)
				{
					CombatSubProcessorWeapon weapon = this.Model.ProcessorWeapons.GetValueOrDefault(character.Weapons[A_1.index]);
					bool flag3 = weapon == null;
					if (!flag3)
					{
						this.<UpdateWeaponCdImageInner>g__SetCountDownTextActive|523_0(true, ref A_1);
						bool flag4 = !A_1.cdProgress.gameObject.activeSelf;
						if (flag4)
						{
							A_1.cdProgress.gameObject.SetActive(true);
						}
						int cdFrameSpeed = CFormula.CalcWeaponCdFrameSpeed((int)this.Model.SelfCharacter.WeaponSwitchSpeed, weapon.Weight);
						string countDownText = CombatUtils.StyleCountDownText((float)A_1.cdFrame / (float)cdFrameSpeed / 60f, A_1.noDurability);
						float fillAmount = Mathf.Min((float)A_1.cdFrame / 30000f, 1f);
						A_1.cdProgress.fillAmount = fillAmount;
						A_1.weaponRefers.countDownText.SetText(countDownText.SetColor("goldyellow"), true);
					}
				}
			}
		}

		// Token: 0x06008E35 RID: 36405 RVA: 0x00422450 File Offset: 0x00420650
		[CompilerGenerated]
		internal static string <HandlerMethodGetHealBanReason>g__GetReasonStr|578_0(ECombatHealBanReason reason, bool isInjury)
		{
			string result;
			switch (reason)
			{
			case ECombatHealBanReason.NonTarget:
				result = LocalStringManager.Get(isInjury ? LanguageKey.LK_Heal_Injury_No_Need_Tips : LanguageKey.LK_Heal_Poison_No_Need_Tips).SetColor("brightred");
				break;
			case ECombatHealBanReason.CountLack:
				result = LocalStringManager.Get(isInjury ? LanguageKey.LK_Heal_Injury_Disable_Tips_NoCount : LanguageKey.LK_Heal_Poison_Disable_Tips_NoCount).SetColor("brightred");
				break;
			case ECombatHealBanReason.HerbLack:
				result = LocalStringManager.Get(LanguageKey.LK_Heal_In_Combat_No_Effect_Tips_Herb).SetColor("brightred");
				break;
			case ECombatHealBanReason.AttainmentLack:
				result = LocalStringManager.Get(isInjury ? LanguageKey.LK_Heal_Injury_In_Combat_No_Effect_Tips : LanguageKey.LK_Heal_Poison_In_Combat_No_Effect_Tips).SetColor("brightred");
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		// Token: 0x04006B72 RID: 27506
		private static readonly string[] CommonBgmList = new string[]
		{
			"combat_01",
			"combat_02",
			"combat_03",
			"combat_04",
			"combat_05"
		};

		// Token: 0x04006B73 RID: 27507
		public const string ActorAssetPath = "RemakeResources/SpineAnimations/Character/skeleton_skin_SkeletonData";

		// Token: 0x04006B74 RID: 27508
		public static readonly Regex RegexSkill = new Regex("^[S|D]_(?<idString>([0-9]+))(_hit)?");

		// Token: 0x04006B75 RID: 27509
		private const float DefeatMarkSeparatorY = 1f;

		// Token: 0x04006B76 RID: 27510
		private const float BaseX = 208f;

		// Token: 0x04006B77 RID: 27511
		private const float BaseWidth = 20f;

		// Token: 0x04006B78 RID: 27512
		private const float BaseSpace = 6f;

		// Token: 0x04006B79 RID: 27513
		private const int DefeatMarkMaxShowCount = 108;

		// Token: 0x04006B7A RID: 27514
		private const float DefeatMarkMaxSpacing = 6f;

		// Token: 0x04006B7B RID: 27515
		private static readonly float[] DefeatMarkBarBackFillAmounts = new float[]
		{
			0.34f,
			0.66f,
			1f,
			0.66f
		};

		// Token: 0x04006B7C RID: 27516
		private const short MinScaleScreenDistance = 400;

		// Token: 0x04006B7D RID: 27517
		private const short MaxScaleScreenDistance = 600;

		// Token: 0x04006B7E RID: 27518
		private const float MinSpineScale = 1.3f;

		// Token: 0x04006B7F RID: 27519
		private const float MaxSpineScale = 2f;

		// Token: 0x04006B80 RID: 27520
		private readonly string[] _mobilityLevelSprite = new string[]
		{
			"combat_jiaodi_tiao_4",
			"combat_jiaodi_tiao_3",
			"combat_jiaodi_tiao_0"
		};

		// Token: 0x04006B81 RID: 27521
		private readonly string[] _currPosSprite = new string[]
		{
			"combat_jiaodi_chibiaoweizhi",
			"combat_jiaodi_chibiaoweizhidifang",
			"combat_jiaodi_chibiaoweizhihui"
		};

		// Token: 0x04006B82 RID: 27522
		private const int RangeImgAddWidth = 65;

		// Token: 0x04006B83 RID: 27523
		public const float FrameToSecond = 60f;

		// Token: 0x04006B84 RID: 27524
		public const int NormalWeaponCount = 3;

		// Token: 0x04006B85 RID: 27525
		private const float ShowAttackTipsTime = 8f;

		// Token: 0x04006B86 RID: 27526
		private static readonly HashSet<string> MoveSkillAniSet = new HashSet<string>();

		// Token: 0x04006B87 RID: 27527
		public static readonly Dictionary<short, sbyte> CharId2BossId = new Dictionary<short, sbyte>();

		// Token: 0x04006B88 RID: 27528
		private readonly uint[] _combatCharSubIds = new uint[]
		{
			64U,
			83U,
			89U,
			143U,
			11U,
			135U,
			144U,
			14U,
			137U,
			12U,
			13U,
			115U,
			31U,
			114U,
			142U,
			46U,
			48U,
			49U,
			32U,
			35U,
			36U,
			37U,
			38U,
			39U,
			40U,
			120U,
			145U,
			121U,
			122U,
			60U,
			8U,
			71U,
			91U,
			93U,
			95U,
			96U,
			97U,
			94U,
			92U,
			98U,
			26U,
			41U,
			42U,
			43U,
			44U,
			45U,
			129U,
			61U,
			76U,
			77U,
			78U,
			141U,
			80U,
			65U,
			62U,
			63U,
			82U,
			119U,
			90U,
			4U,
			9U,
			10U,
			87U,
			88U,
			113U,
			140U
		};

		// Token: 0x04006B89 RID: 27529
		private readonly uint[] _carrierAnimalSubIds = new uint[]
		{
			83U,
			89U,
			91U,
			93U,
			95U,
			96U,
			97U,
			94U,
			92U,
			98U,
			9U,
			10U
		};

		// Token: 0x04006B8A RID: 27530
		private readonly uint[] _specialShowCharSubIds = new uint[]
		{
			83U,
			89U,
			91U,
			93U,
			95U,
			96U,
			97U,
			94U,
			92U,
			98U,
			9U,
			10U
		};

		// Token: 0x04006B8B RID: 27531
		[SerializeField]
		private double setTargetDistanceFirstDelta = 0.08;

		// Token: 0x04006B8C RID: 27532
		[SerializeField]
		private double setTargetDistanceFixedDelta = 0.04;

		// Token: 0x04006B8D RID: 27533
		[SerializeField]
		private double setTargetDistanceFixedDeltaMin = 0.02;

		// Token: 0x04006B8E RID: 27534
		[SerializeField]
		private double setTargetDistanceDamping = 1.0;

		// Token: 0x04006B8F RID: 27535
		[SerializeField]
		private int setTargetDistanceDeltaPerTimes = 20;

		// Token: 0x04006B90 RID: 27536
		[SerializeField]
		private CombatDamageCompare damageCompare;

		// Token: 0x04006B91 RID: 27537
		[SerializeField]
		private GameObject effectTipsPrefab;

		// Token: 0x04006B92 RID: 27538
		[SerializeField]
		private GameObject defeatMarkPrefab;

		// Token: 0x04006B93 RID: 27539
		[SerializeField]
		private GameObject textTipsPrefab;

		// Token: 0x04006B94 RID: 27540
		[SerializeField]
		private GameObject iconTipsPrefab;

		// Token: 0x04006B95 RID: 27541
		[SerializeField]
		private RectTransform particlePrefabHolder;

		// Token: 0x04006B96 RID: 27542
		[SerializeField]
		private CButton attackButton;

		// Token: 0x04006B97 RID: 27543
		[SerializeField]
		private RectTransform moveClickArea;

		// Token: 0x04006B98 RID: 27544
		[SerializeField]
		private CombatInfoTop selfInfoTop;

		// Token: 0x04006B99 RID: 27545
		[SerializeField]
		private CombatInfoTop enemyInfoTop;

		// Token: 0x04006B9A RID: 27546
		[SerializeField]
		private List<SkeletonAnimation> selfSkeletonList;

		// Token: 0x04006B9B RID: 27547
		[SerializeField]
		private List<SkeletonAnimation> enemySkeletonList;

		// Token: 0x04006B9C RID: 27548
		[SerializeField]
		private RectTransform injuryTextLayer;

		// Token: 0x04006B9D RID: 27549
		[SerializeField]
		private CombatSelfInfoChar selfInfoChar;

		// Token: 0x04006B9E RID: 27550
		[SerializeField]
		private CombatAffectingDefendSkill selfTeammateAffectingDefSkill;

		// Token: 0x04006B9F RID: 27551
		[SerializeField]
		private CombatEnemyInfoChar enemyInfoChar;

		// Token: 0x04006BA0 RID: 27552
		[SerializeField]
		private CombatAffectingDefendSkill enemyTeammateAffectingDefSkill;

		// Token: 0x04006BA1 RID: 27553
		[SerializeField]
		private TextMeshProUGUI distance;

		// Token: 0x04006BA2 RID: 27554
		[SerializeField]
		private CombatInfoBottom selfInfoBottom;

		// Token: 0x04006BA3 RID: 27555
		[SerializeField]
		private CombatInfoBottom enemyInfoBottom;

		// Token: 0x04006BA4 RID: 27556
		[SerializeField]
		private CombatProactiveSkillScroll combatSkillScroll;

		// Token: 0x04006BA5 RID: 27557
		[SerializeField]
		private GameObject trickPrefab;

		// Token: 0x04006BA6 RID: 27558
		[SerializeField]
		private GameObject bottom;

		// Token: 0x04006BA7 RID: 27559
		[SerializeField]
		private RectTransform spineLayer;

		// Token: 0x04006BA8 RID: 27560
		[SerializeField]
		private TextMeshProUGUI backwardTipsKey;

		// Token: 0x04006BA9 RID: 27561
		[SerializeField]
		private TextMeshProUGUI forwardTipsKey;

		// Token: 0x04006BAA RID: 27562
		[SerializeField]
		private CRawImage particleRenderTexture2;

		// Token: 0x04006BAB RID: 27563
		[SerializeField]
		private TextMeshProUGUI attackTipsKey;

		// Token: 0x04006BAC RID: 27564
		[SerializeField]
		private SwitchToggleSmall autoFight;

		// Token: 0x04006BAD RID: 27565
		[SerializeField]
		private GameObject autoFightEffect;

		// Token: 0x04006BAE RID: 27566
		[SerializeField]
		private CombatAttackRangeBar attackRangeBar;

		// Token: 0x04006BAF RID: 27567
		[SerializeField]
		private CRawImage bulletTimeMask;

		// Token: 0x04006BB0 RID: 27568
		[SerializeField]
		private CombatConfigTips combatConfigTipsBack;

		// Token: 0x04006BB1 RID: 27569
		[SerializeField]
		private SkeletonAnimation selfCarrierAnimalSkeleton;

		// Token: 0x04006BB2 RID: 27570
		[SerializeField]
		private SkeletonAnimation selfSpecialShowSkeleton;

		// Token: 0x04006BB3 RID: 27571
		[SerializeField]
		private RectTransform combatSharedMaskTarget;

		// Token: 0x04006BB4 RID: 27572
		[SerializeField]
		private GameObject changeTrickMask;

		// Token: 0x04006BB5 RID: 27573
		[SerializeField]
		private CToggleGroup changeTrickTrickHolder;

		// Token: 0x04006BB6 RID: 27574
		[SerializeField]
		private CToggleGroup changeTrickPartHolder;

		// Token: 0x04006BB7 RID: 27575
		[SerializeField]
		private TextMeshProUGUI flawChangeTrickCostText;

		// Token: 0x04006BB8 RID: 27576
		[SerializeField]
		private TextMeshProUGUI acupointChangeTrickCostText;

		// Token: 0x04006BB9 RID: 27577
		[SerializeField]
		private TextMeshProUGUI confirmChangeTrickCostText;

		// Token: 0x04006BBA RID: 27578
		[SerializeField]
		private CButton confirmChangeTrick;

		// Token: 0x04006BBB RID: 27579
		[SerializeField]
		private GameObject clickMask;

		// Token: 0x04006BBC RID: 27580
		[SerializeField]
		private GameObject damageNumPrefab;

		// Token: 0x04006BBD RID: 27581
		[SerializeField]
		private GameObject fatalDamageNumPrefab;

		// Token: 0x04006BBE RID: 27582
		[SerializeField]
		private CombatAiOptionPanel aiOptionMask;

		// Token: 0x04006BBF RID: 27583
		[SerializeField]
		private GameObject aiOptionBtn;

		// Token: 0x04006BC0 RID: 27584
		[SerializeField]
		private CombatReserveController reserveTips;

		// Token: 0x04006BC1 RID: 27585
		[SerializeField]
		private DisableStyleRoot syncOptions;

		// Token: 0x04006BC2 RID: 27586
		[SerializeField]
		private GameObject saveTips;

		// Token: 0x04006BC3 RID: 27587
		[SerializeField]
		private CImage backgroundMask;

		// Token: 0x04006BC4 RID: 27588
		[SerializeField]
		private CombatDistanceBar targetDistanceBar;

		// Token: 0x04006BC5 RID: 27589
		[SerializeField]
		private GameObject jumpThresholdMask;

		// Token: 0x04006BC6 RID: 27590
		[SerializeField]
		private CombatJumpThreshold jumpThresholdSetting;

		// Token: 0x04006BC7 RID: 27591
		[SerializeField]
		private CombatSkillBreakerDefend defendSkillBreaker;

		// Token: 0x04006BC8 RID: 27592
		[SerializeField]
		private CombatDrops combatDropsPrefab;

		// Token: 0x04006BC9 RID: 27593
		[SerializeField]
		private CButton flawChangeTrickBtn;

		// Token: 0x04006BCA RID: 27594
		[SerializeField]
		private CButton acupointChangeTrickBtn;

		// Token: 0x04006BCB RID: 27595
		[SerializeField]
		private GameObject defeatMarkSeparatorPrefab;

		// Token: 0x04006BCC RID: 27596
		[SerializeField]
		private GameObject commandBubblePrefab;

		// Token: 0x04006BCD RID: 27597
		[SerializeField]
		private CombatTimeScaleToggle combatTimeScaleToggle;

		// Token: 0x04006BCE RID: 27598
		[SerializeField]
		private CombatSkillBreakerMove moveSkillBreaker;

		// Token: 0x04006BCF RID: 27599
		[SerializeField]
		private CombatRawCreatePage rawCreatePage;

		// Token: 0x04006BD0 RID: 27600
		[SerializeField]
		private CombatSkillBreakerDefendExtra defendSkillBreakerExtra;

		// Token: 0x04006BD1 RID: 27601
		[SerializeField]
		private CombatPure pureMode;

		// Token: 0x04006BD2 RID: 27602
		[SerializeField]
		private CombatParticle combatParticle;

		// Token: 0x04006BD3 RID: 27603
		[SerializeField]
		private CRawImage particleRenderTexture1;

		// Token: 0x04006BD4 RID: 27604
		[SerializeField]
		private CombatUIParticle combatUIParticle;

		// Token: 0x04006BD5 RID: 27605
		[SerializeField]
		private CombatUseItemPanel combatUseItemPanel;

		// Token: 0x04006BD6 RID: 27606
		[SerializeField]
		private CombatWheel combatWheel;

		// Token: 0x04006BD7 RID: 27607
		[SerializeField]
		private CombatQuickUseItemPanel combatQuickUseItemPanel;

		// Token: 0x04006BD8 RID: 27608
		[SerializeField]
		private CButton buttonUseItem;

		// Token: 0x04006BD9 RID: 27609
		[SerializeField]
		private CombatSkillWheel combatSkillWheel;

		// Token: 0x04006BDA RID: 27610
		[SerializeField]
		private CombatSkillSortWidget combatSkillSortWidget;

		// Token: 0x04006BDB RID: 27611
		[SerializeField]
		private RectTransform selfInnerRatioCheckArea;

		// Token: 0x04006BDC RID: 27612
		[SerializeField]
		private GameObject selfInnerRatio;

		// Token: 0x04006BDD RID: 27613
		[SerializeField]
		private RectTransform enemyInnerRatioCheckArea;

		// Token: 0x04006BDE RID: 27614
		[SerializeField]
		private GameObject enemyInnerRatio;

		// Token: 0x04006BDF RID: 27615
		[SerializeField]
		private RectTransform otherActionReserveRoot;

		// Token: 0x04006BE0 RID: 27616
		private ViewCombat.EPauseReason _pauseReason;

		// Token: 0x04006BE1 RID: 27617
		private Dictionary<HotKeyCommand, Action> _hotKey2Action;

		// Token: 0x04006BE2 RID: 27618
		private int _maxDefeatMarkCount;

		// Token: 0x04006BE3 RID: 27619
		private short _normalAttackDistance;

		// Token: 0x04006BE4 RID: 27620
		private bool _inBulletTime;

		// Token: 0x04006BE5 RID: 27621
		private readonly Dictionary<int, byte[]> _flawCountDict = new Dictionary<int, byte[]>();

		// Token: 0x04006BE6 RID: 27622
		private readonly Dictionary<int, FlawOrAcupointCollection> _flawTimeDict = new Dictionary<int, FlawOrAcupointCollection>();

		// Token: 0x04006BE7 RID: 27623
		private readonly Dictionary<int, byte[]> _acupointCountDict = new Dictionary<int, byte[]>();

		// Token: 0x04006BE8 RID: 27624
		private readonly Dictionary<int, FlawOrAcupointCollection> _acupointTimeDict = new Dictionary<int, FlawOrAcupointCollection>();

		// Token: 0x04006BE9 RID: 27625
		private readonly Dictionary<int, string> _loopAniDict = new Dictionary<int, string>();

		// Token: 0x04006BEA RID: 27626
		private int _transitioningCharId = -1;

		// Token: 0x04006BEB RID: 27627
		private readonly Dictionary<int, int> _displayPosDict = new Dictionary<int, int>();

		// Token: 0x04006BEC RID: 27628
		private readonly Dictionary<SkeletonAnimation, float> _skeletonTimeScaleBeforePause = new Dictionary<SkeletonAnimation, float>();

		// Token: 0x04006BED RID: 27629
		private readonly Dictionary<int, bool> _skeletonLoaded = new Dictionary<int, bool>();

		// Token: 0x04006BEE RID: 27630
		private readonly List<RectTransform> _tipsList = new List<RectTransform>();

		// Token: 0x04006BEF RID: 27631
		private SilenceData _enemySilenceData = new SilenceData();

		// Token: 0x04006BF0 RID: 27632
		private SilenceData _allySilenceData = new SilenceData();

		// Token: 0x04006BF1 RID: 27633
		private bool _autoCombat;

		// Token: 0x04006BF2 RID: 27634
		private byte _moveState;

		// Token: 0x04006BF3 RID: 27635
		private short _selfSpecialWisdomCount;

		// Token: 0x04006BF4 RID: 27636
		private sbyte _selfMaxEatingSlotCount;

		// Token: 0x04006BF5 RID: 27637
		private bool _attackSkillInitialized;

		// Token: 0x04006BF6 RID: 27638
		private bool _isInSelfAttackRange;

		// Token: 0x04006BF7 RID: 27639
		private sbyte _changeTrickIndex;

		// Token: 0x04006BF8 RID: 27640
		private sbyte _changeTrickBodyPart;

		// Token: 0x04006BF9 RID: 27641
		private int _taiwuCharId;

		// Token: 0x04006BFA RID: 27642
		private bool _hunterSkillUnlocked;

		// Token: 0x04006BFB RID: 27643
		private float _showAttackTipsTimer;

		// Token: 0x04006BFC RID: 27644
		private short _mouseOverSkill;

		// Token: 0x04006BFD RID: 27645
		private ItemKey _previewRangeWeapon;

		// Token: 0x04006BFE RID: 27646
		private int _previewRangeWeaponCharId;

		// Token: 0x04006BFF RID: 27647
		private short _previewRangeSkill;

		// Token: 0x04006C00 RID: 27648
		private int _previewRangeSkillCharId;

		// Token: 0x04006C01 RID: 27649
		private OuterAndInnerShorts _previewRangeItem;

		// Token: 0x04006C02 RID: 27650
		private double _lastSetTargetDistanceDelta;

		// Token: 0x04006C03 RID: 27651
		private double _lastSetTargetDistanceTime;

		// Token: 0x04006C04 RID: 27652
		private int _setTargetDistanceTimes;

		// Token: 0x04006C05 RID: 27653
		private RawCreateCollection _selfRawCreateData = new RawCreateCollection();

		// Token: 0x04006C06 RID: 27654
		private float _startContinuousNormalAttackTime;

		// Token: 0x04006C07 RID: 27655
		private float _continuousNormalAttackStartUpTime = 0.3f;

		// Token: 0x04006C08 RID: 27656
		private bool _clickAttackButton;

		// Token: 0x04006C09 RID: 27657
		private readonly CombatVirtualCamera _virtualCamera = new CombatVirtualCamera(new CombatVirtualCamera.Config(600f, 400f, 2f, 1.3f));

		// Token: 0x04006C0A RID: 27658
		private int _selfPos;

		// Token: 0x04006C0B RID: 27659
		private int _selfDisplayPos;

		// Token: 0x04006C0C RID: 27660
		private short _selfMoveCd;

		// Token: 0x04006C0D RID: 27661
		private int _selfMobility;

		// Token: 0x04006C0E RID: 27662
		private byte _selfMobilityLevel;

		// Token: 0x04006C0F RID: 27663
		private int _selfMobilityRecoverSpeed;

		// Token: 0x04006C10 RID: 27664
		private sbyte _selfJumpPrepareProgress;

		// Token: 0x04006C11 RID: 27665
		private sbyte _selfJumpPreparedDistance;

		// Token: 0x04006C12 RID: 27666
		private short _selfMobilityLockEffectCount;

		// Token: 0x04006C13 RID: 27667
		private float _selfChangeDistanceDuration;

		// Token: 0x04006C14 RID: 27668
		private CombatReserveData _selfReserveData;

		// Token: 0x04006C15 RID: 27669
		private short _selfAffectingMoveSkillId;

		// Token: 0x04006C16 RID: 27670
		private short _selfAffectingDefendSkillId;

		// Token: 0x04006C17 RID: 27671
		private bool _selfDefeatMarkInitialized;

		// Token: 0x04006C18 RID: 27672
		private readonly SortedDictionary<DefeatMarkKey, List<RectTransform>> _selfDefeatMarkObjs = new SortedDictionary<DefeatMarkKey, List<RectTransform>>();

		// Token: 0x04006C19 RID: 27673
		private readonly List<RectTransform> _selfDefeatMarkAddQueue = new List<RectTransform>();

		// Token: 0x04006C1A RID: 27674
		private readonly List<RectTransform> _selfDefeatMarkSeparatorAddQueue = new List<RectTransform>();

		// Token: 0x04006C1B RID: 27675
		private readonly List<DefeatMarkKey> _selfDefeatMarkKeyList = new List<DefeatMarkKey>();

		// Token: 0x04006C1C RID: 27676
		private PoisonInts _selfPoisons;

		// Token: 0x04006C1D RID: 27677
		private PoisonInts _selfPoisonResists;

		// Token: 0x04006C1E RID: 27678
		private MixPoisonAffectedCountCollection _selfMixPoisonAffectedCount;

		// Token: 0x04006C1F RID: 27679
		private InjuryAutoHealCollection _selfInjuryAutoHealCollection;

		// Token: 0x04006C20 RID: 27680
		private InjuryAutoHealCollection _selfOldInjuryAutoHealCollection;

		// Token: 0x04006C21 RID: 27681
		private short _selfTargetDistance;

		// Token: 0x04006C22 RID: 27682
		private readonly DamageStepCollection _selfDamageStepCollection = new DamageStepCollection();

		// Token: 0x04006C23 RID: 27683
		private List<int> _selfUnlockPrepareValue = new List<int>();

		// Token: 0x04006C24 RID: 27684
		private List<int> _oldSelfUnlockPrepareValue = new List<int>();

		// Token: 0x04006C25 RID: 27685
		private List<bool> _selfCanUnlockAttack = new List<bool>();

		// Token: 0x04006C26 RID: 27686
		private bool[] _selfUnlockTriggered = new bool[3];

		// Token: 0x04006C27 RID: 27687
		private bool[] _selfUnlockEffectTriggered = new bool[3];

		// Token: 0x04006C28 RID: 27688
		private List<int> _selfRawCreateEffects = new List<int>();

		// Token: 0x04006C29 RID: 27689
		private int _enemyPos;

		// Token: 0x04006C2A RID: 27690
		private int _enemyDisplayPos;

		// Token: 0x04006C2B RID: 27691
		private short _enemyMoveCd;

		// Token: 0x04006C2C RID: 27692
		private int _enemyMobility;

		// Token: 0x04006C2D RID: 27693
		private byte _enemyMobilityLevel;

		// Token: 0x04006C2E RID: 27694
		private int _enemyMobilityRecoverSpeed;

		// Token: 0x04006C2F RID: 27695
		private sbyte _enemyJumpPrepareProgress;

		// Token: 0x04006C30 RID: 27696
		private sbyte _enemyJumpPreparedDistance;

		// Token: 0x04006C31 RID: 27697
		private short _enemyMobilityLockEffectCount;

		// Token: 0x04006C32 RID: 27698
		private float _enemyChangeDistanceDuration;

		// Token: 0x04006C33 RID: 27699
		private short _enemyAffectingMoveSkillId;

		// Token: 0x04006C34 RID: 27700
		private short _enemyAffectingDefendSkillId;

		// Token: 0x04006C35 RID: 27701
		private bool _enemyDoingChangeMaxTrickAni;

		// Token: 0x04006C36 RID: 27702
		private bool _enemyDefeatMarkInitialized;

		// Token: 0x04006C37 RID: 27703
		private readonly SortedDictionary<DefeatMarkKey, List<RectTransform>> _enemyDefeatMarkObjs = new SortedDictionary<DefeatMarkKey, List<RectTransform>>();

		// Token: 0x04006C38 RID: 27704
		private readonly List<RectTransform> _enemyDefeatMarkAddQueue = new List<RectTransform>();

		// Token: 0x04006C39 RID: 27705
		private readonly List<RectTransform> _enemyDefeatMarkSeparatorAddQueue = new List<RectTransform>();

		// Token: 0x04006C3A RID: 27706
		private readonly List<DefeatMarkKey> _enemyDefeatMarkKeyList = new List<DefeatMarkKey>();

		// Token: 0x04006C3B RID: 27707
		private PoisonInts _enemyPoisonResists;

		// Token: 0x04006C3C RID: 27708
		private MixPoisonAffectedCountCollection _enemyMixPoisonAffectedCount;

		// Token: 0x04006C3D RID: 27709
		private InjuryAutoHealCollection _enemyInjuryAutoHealCollection;

		// Token: 0x04006C3E RID: 27710
		private InjuryAutoHealCollection _enemyOldInjuryAutoHealCollection;

		// Token: 0x04006C3F RID: 27711
		private short _enemyTargetDistance;

		// Token: 0x04006C40 RID: 27712
		private readonly DamageStepCollection _enemyDamageStepCollection = new DamageStepCollection();

		// Token: 0x04006C41 RID: 27713
		private List<int> _enemyUnlockPrepareValue = new List<int>();

		// Token: 0x04006C42 RID: 27714
		private List<int> _oldEnemyUnlockPrepareValue = new List<int>();

		// Token: 0x04006C43 RID: 27715
		private List<bool> _enemyCanUnlockAttack = new List<bool>();

		// Token: 0x04006C44 RID: 27716
		private bool[] _enemyUnlockTriggered = new bool[3];

		// Token: 0x04006C45 RID: 27717
		private SkeletonAnimation _selfPetSkeleton;

		// Token: 0x04006C46 RID: 27718
		private RectTransform _selfWeaponHolder;

		// Token: 0x04006C47 RID: 27719
		private CombatOtherActionHolder _selfOtherActionHolder;

		// Token: 0x04006C48 RID: 27720
		private SkeletonAnimation _enemyPetSkeleton;

		// Token: 0x04006C49 RID: 27721
		private RectTransform _enemyWeaponHolder;

		// Token: 0x04006C4A RID: 27722
		private IReadOnlyList<ICombatComponent> _components;

		// Token: 0x04006C4B RID: 27723
		private DisableStyleRoot _syncOptionsBtnStyle;

		// Token: 0x04006C4C RID: 27724
		private RectTransform _moveClickRect;

		// Token: 0x04006C4D RID: 27725
		private bool _selectingUseItem;

		// Token: 0x04006C4E RID: 27726
		private bool _settingJumpThreshold;

		// Token: 0x04006C4F RID: 27727
		private bool _xiangshuSceneInitialized;

		// Token: 0x04006C50 RID: 27728
		private bool _isDisplayDistanceChanging;

		// Token: 0x04006C51 RID: 27729
		private Coroutine _displayDistanceChangeCoroutine;

		// Token: 0x04006C52 RID: 27730
		private bool _isAllyDisplayDistanceChanging;

		// Token: 0x04006C53 RID: 27731
		private Tweener _shakeTweener;

		// Token: 0x04006C54 RID: 27732
		private readonly List<sbyte> _moveKeyDownList = new List<sbyte>();

		// Token: 0x04006C55 RID: 27733
		private readonly Dictionary<SkeletonAnimation, Tweener> _moveTweenerDict = new Dictionary<SkeletonAnimation, Tweener>();

		// Token: 0x04006C56 RID: 27734
		private readonly Dictionary<SkeletonAnimation, ViewCombat.SkeletonMoveInfo> _skeletonMoveInfoDict = new Dictionary<SkeletonAnimation, ViewCombat.SkeletonMoveInfo>();

		// Token: 0x04006C57 RID: 27735
		private SkeletonDataAsset _actorSkeleton;

		// Token: 0x04006C58 RID: 27736
		private readonly Dictionary<string, RawAnimationAsset> _commonAniDict = new Dictionary<string, RawAnimationAsset>();

		// Token: 0x04006C59 RID: 27737
		private readonly List<string> _commonParticleNameList = new List<string>();

		// Token: 0x04006C5A RID: 27738
		private readonly Dictionary<string, AudioClip> _commonSoundDict = new Dictionary<string, AudioClip>();

		// Token: 0x04006C5B RID: 27739
		private readonly List<short> _needLoadAssetSkillList = new List<short>();

		// Token: 0x04006C5C RID: 27740
		private readonly List<short> _loadedAssetSkillList = new List<short>();

		// Token: 0x04006C5D RID: 27741
		private readonly Dictionary<string, RawAnimationAsset> _skillAniDict = new Dictionary<string, RawAnimationAsset>();

		// Token: 0x04006C5E RID: 27742
		private readonly List<string> _skillAndSpecialParticleNameList = new List<string>();

		// Token: 0x04006C5F RID: 27743
		private readonly Dictionary<string, AudioClip> _skillAndSpecialSoundDict = new Dictionary<string, AudioClip>();

		// Token: 0x04006C60 RID: 27744
		private readonly Dictionary<int, ParticleSystem> _defendParticleDict = new Dictionary<int, ParticleSystem>();

		// Token: 0x04006C61 RID: 27745
		private readonly Dictionary<int, string> _defendSoundDict = new Dictionary<int, string>();

		// Token: 0x04006C62 RID: 27746
		private readonly short[] _defendBounceRangeList = new short[4];

		// Token: 0x04006C63 RID: 27747
		private readonly int[] _defendBounceCharList = new int[4];

		// Token: 0x04006C64 RID: 27748
		private readonly Dictionary<int, string> _loopSoundDict = new Dictionary<int, string>();

		// Token: 0x04006C65 RID: 27749
		private bool _showingSystemOption;

		// Token: 0x04006C66 RID: 27750
		private bool _guidingChapterOpen;

		// Token: 0x04006C67 RID: 27751
		private bool _keepPauseUntilCastSkill;

		// Token: 0x04006C68 RID: 27752
		private bool _skillWheelPauseHolding;

		// Token: 0x04006C69 RID: 27753
		private bool _skillSortPauseHolding;

		// Token: 0x04006C6A RID: 27754
		private bool _pausedBySystemMenu;

		// Token: 0x04006C6B RID: 27755
		private bool _awaked;

		// Token: 0x020020F5 RID: 8437
		private enum EPauseReason
		{
			// Token: 0x0400D311 RID: 54033
			None,
			// Token: 0x0400D312 RID: 54034
			AiOptions,
			// Token: 0x0400D313 RID: 54035
			JumpSetting,
			// Token: 0x0400D314 RID: 54036
			Dialog,
			// Token: 0x0400D315 RID: 54037
			SkillWheel,
			// Token: 0x0400D316 RID: 54038
			SkillSort,
			// Token: 0x0400D317 RID: 54039
			CirclePanel,
			// Token: 0x0400D318 RID: 54040
			DamageDetail,
			// Token: 0x0400D319 RID: 54041
			Encyclopedia,
			// Token: 0x0400D31A RID: 54042
			GuidingChapter
		}

		// Token: 0x020020F6 RID: 8438
		private struct SkeletonMoveInfo
		{
			// Token: 0x0400D31B RID: 54043
			public float Pos;

			// Token: 0x0400D31C RID: 54044
			public bool UpdateParticlePos;
		}
	}
}
