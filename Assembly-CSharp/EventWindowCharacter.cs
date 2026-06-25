using System;
using System.Collections;
using System.Collections.Generic;
using CharacterDataMonitor;
using Config;
using DG.Tweening;
using FrameWork;
using Game.Components.Avatar;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Combat;
using GameData.Domains.Extra;
using GameData.Domains.Item.Display;
using GameData.Domains.Merchant;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UICommon.Character;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200033A RID: 826
public class EventWindowCharacter : Refers
{
	// Token: 0x17000550 RID: 1360
	// (get) Token: 0x06003024 RID: 12324 RVA: 0x0017829C File Offset: 0x0017649C
	private EventModel Model
	{
		get
		{
			bool flag = this._model == null;
			if (flag)
			{
				this._model = SingletonObject.getInstance<EventModel>();
			}
			return this._model;
		}
	}

	// Token: 0x17000551 RID: 1361
	// (get) Token: 0x06003025 RID: 12325 RVA: 0x001782CC File Offset: 0x001764CC
	private TaiwuEventDisplayData Data
	{
		get
		{
			return this.Model.DisplayingEventData;
		}
	}

	// Token: 0x17000552 RID: 1362
	// (get) Token: 0x06003026 RID: 12326 RVA: 0x001782D9 File Offset: 0x001764D9
	// (set) Token: 0x06003027 RID: 12327 RVA: 0x001782E1 File Offset: 0x001764E1
	public bool InitFlag { get; set; }

	// Token: 0x06003028 RID: 12328 RVA: 0x001782EA File Offset: 0x001764EA
	private void Awake()
	{
		this.TryInit();
	}

	// Token: 0x06003029 RID: 12329 RVA: 0x001782F4 File Offset: 0x001764F4
	private void OnEnable()
	{
		bool waitRefreshFavorFlag = this._waitRefreshFavorFlag;
		if (waitRefreshFavorFlag)
		{
			this.FavorInfoRootObj.SetActive(false);
			this.RelationLayout.gameObject.SetActive(false);
			base.StartCoroutine(this.UpdateFavorAndRelationShipShow());
		}
		bool flag = this._onEnableAction != null;
		if (flag)
		{
			this._onEnableAction();
			this._onEnableAction = null;
		}
	}

	// Token: 0x0600302A RID: 12330 RVA: 0x0017835C File Offset: 0x0017655C
	private void OnDisable()
	{
		this.ResetFavorChange();
		bool flag = this._basicInfoMonitor != null;
		if (flag)
		{
			this._basicInfoMonitor.RemoveDebtsOfTaiwuListener(new Action(this.UpdateDebtEntranceVisible));
			this._basicInfoMonitor.RemoveFavorabilityListener(new Action(this.FillFavorProgress));
		}
		this.FavorInfoRootObj.SetActive(false);
		this.RelationLayout.gameObject.SetActive(false);
		this._consummateLevelHandler.CharacterId = -1;
		this._relationShipHandler.CharacterId = -1;
		this._favorabilityHandler.CharacterId = -1;
		this._behaviorHandler.CharacterId = -1;
		this._fameHandler.CharacterId = -1;
		this._identityHandler.CharacterId = -1;
	}

	// Token: 0x0600302B RID: 12331 RVA: 0x00178420 File Offset: 0x00176620
	public void Refresh()
	{
		this._curCharacterId = -1;
		bool flag = !this.GetHasCharacter();
		if (flag)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			this.TryInit();
			CharacterDisplayData characterDisplayData = this.GetCharacterDisplayData();
			bool needAnimation = !base.gameObject.activeSelf;
			EventActorData actorData = this.GetEventActorData();
			CaravanDisplayData caravanDisplayData = this.GetCaravanDisplayData();
			short templateIdOfCharacter = this.GetTemplateIdOfCharacter();
			ItemDisplayData jiaoMouseTipDisplayData = this.GetJiaoMouseTipDisplayData();
			ValueTuple<sbyte, sbyte> tuple = this.GetXiangShuAvatarId();
			this.CharacterOnMapBlockTips.enabled = false;
			base.CGet<GameObject>("CharacterInfoHolder").SetActive(actorData == null);
			bool flag2 = actorData != null;
			if (flag2)
			{
				this.RefreshAsActor(actorData);
			}
			else
			{
				bool flag3 = 9 != tuple.Item1;
				if (flag3)
				{
					this.RefreshAsXiangShuAvatar(tuple.Item1, tuple.Item2);
				}
				else
				{
					bool flag4 = characterDisplayData != null;
					if (flag4)
					{
						this.RefreshAsNormalCharacter(characterDisplayData);
					}
					else
					{
						bool flag5 = -1 != templateIdOfCharacter;
						if (flag5)
						{
							this.RefreshAsTemplateCharacter(templateIdOfCharacter);
						}
						else
						{
							bool flag6 = caravanDisplayData != null;
							if (flag6)
							{
								this.RefreshAsMerchant(caravanDisplayData);
							}
						}
					}
				}
			}
			bool flag7 = jiaoMouseTipDisplayData != null;
			if (flag7)
			{
				this.SetJiaoMouseTip(jiaoMouseTipDisplayData);
			}
			else
			{
				this.ShowCharacterMenuTips.SetActive(false);
			}
			base.gameObject.SetActive(true);
			this.RefreshFavorView();
			this.UpdateInjuryInfoTipsVisible();
			bool flag8 = needAnimation;
			if (flag8)
			{
				this.PlayAvatarAppearAnimation();
			}
		}
	}

	// Token: 0x0600302C RID: 12332 RVA: 0x00178588 File Offset: 0x00176788
	private void RefreshAsNormalCharacter(CharacterDisplayData characterDisplayData)
	{
		bool flag = SingletonObject.getInstance<TutorialChapterModel>().InGuiding && characterDisplayData.TemplateId == 908;
		if (flag)
		{
			string avatarAssetName = "NpcFace_huanxin";
			bool huanxinSurprised = SingletonObject.getInstance<TutorialChapterModel>().HuanxinSurprised;
			if (huanxinSurprised)
			{
				avatarAssetName = "NpcFace_jingyahuanxin";
			}
			bool huanxinDying = SingletonObject.getInstance<TutorialChapterModel>().HuanxinDying;
			if (huanxinDying)
			{
				avatarAssetName = "NpcFace_binsihuanxin";
			}
			string spineName = null;
			string spineSkinName = null;
			CharacterItem config = Character.Instance[characterDisplayData.TemplateId];
			bool flag2 = avatarAssetName == "NpcFace_huanxin";
			if (flag2)
			{
				spineName = config.FixedAvatarSpineName;
				spineSkinName = config.FixedAvatarSpineSkin;
			}
			else
			{
				bool flag3 = avatarAssetName == "NpcFace_jingyahuanxin";
				if (flag3)
				{
					spineName = "NpcFace/huanxin_jingya";
				}
				else
				{
					bool flag4 = avatarAssetName == "NpcFace_binsihuanxin";
					if (flag4)
					{
						spineName = "NpcFace/huanxin_binsi";
					}
				}
			}
			bool flag5 = !string.IsNullOrEmpty(spineName);
			if (flag5)
			{
				this.Avatar.RefreshAsSpine(spineName, spineSkinName);
			}
			else
			{
				string resPath = CharacterAvatar.GetNpcFaceResPath(CharacterAvatar.GetAvatarSizeFolder(this.Avatar.Size), avatarAssetName);
				ResLoader.LoadModOrGameResource<Texture2D>(resPath, delegate(Texture2D tex)
				{
					this.Avatar.Refresh(tex);
				}, delegate(string path)
				{
					this.Avatar.Refresh(characterDisplayData, true);
				});
			}
		}
		else
		{
			this.Avatar.Refresh(characterDisplayData, true);
		}
		string roleNameKey = this.GetRoleNameKey();
		int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		bool useAlternativeName = this.GetUseAlternativeName();
		if (useAlternativeName)
		{
			CharacterItem config2 = Character.Instance.GetItem(characterDisplayData.TemplateId);
			bool flag6 = config2 != null && !string.IsNullOrEmpty(config2.AnonymousTitle);
			if (flag6)
			{
				this.NameLabel.text = config2.AnonymousTitle;
			}
		}
		else
		{
			bool flag7 = !string.IsNullOrEmpty(roleNameKey);
			if (flag7)
			{
				this.NameLabel.text = LocalStringManager.Get(roleNameKey);
			}
			else
			{
				this.NameLabel.text = NameCenter.GetCharMonasticTitleOrNameByDisplayData(characterDisplayData, characterDisplayData.CharacterId == taiwuCharId, false);
			}
		}
		bool rightCharacterShadow = this.GetRightCharacterShadow();
		if (rightCharacterShadow)
		{
			this.Avatar.SetShadowStrength(0.85f);
		}
		bool flag8 = this._basicInfoMonitor != null;
		if (flag8)
		{
			this._basicInfoMonitor.RemoveDebtsOfTaiwuListener(new Action(this.UpdateDebtEntranceVisible));
		}
		this._favorabilityHandler.CharacterId = characterDisplayData.CharacterId;
		bool rightForbiddenConsummateLevel = this.GetRightForbiddenConsummateLevel();
		if (rightForbiddenConsummateLevel)
		{
			this._consummateLevelHandler.CharacterId = -1;
		}
		else
		{
			this._consummateLevelHandler.CharacterId = characterDisplayData.CharacterId;
		}
		this._behaviorHandler.CharacterId = characterDisplayData.CharacterId;
		this._fameHandler.CharacterId = characterDisplayData.CharacterId;
		this._identityHandler.CharacterId = characterDisplayData.CharacterId;
		this._basicInfoMonitor = this._favorabilityHandler.GetMonitor<BasicInfoMonitor>();
		bool flag9 = this._basicInfoMonitor != null;
		if (flag9)
		{
			this._basicInfoMonitor.AddDebtsOfTaiwuListener(new Action(this.UpdateDebtEntranceVisible));
			this._basicInfoMonitor.AddFavorabilityListener(new Action(this.FillFavorProgress));
			this._forbidShowFavorChangeEffectFlag = this.GetForbidShowFavorChangeEffectFlag();
			bool forbidShowFavorChangeEffectFlag = this._forbidShowFavorChangeEffectFlag;
			if (forbidShowFavorChangeEffectFlag)
			{
				this._basicInfoMonitor.RemoveFavorabilityListener(new Action(this.FillFavorProgress));
			}
			bool init = this._basicInfoMonitor.Init;
			if (init)
			{
				this._basicInfoMonitor.Refresh();
			}
			bool flag10 = null != this.DebtEntranceButton;
			if (flag10)
			{
				this.DebtEntranceButton.ClearAndAddListener(delegate
				{
				});
			}
		}
		CharacterRelationMonitor relationMonitor = this._relationShipHandler.GetMonitor<CharacterRelationMonitor>();
		bool needShowRelation = characterDisplayData.CharacterId != SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		bool flag11 = relationMonitor != null && relationMonitor.Init && relationMonitor.RelationBitFlag == ushort.MaxValue && relationMonitor.CharacterId == characterDisplayData.CharacterId;
		if (flag11)
		{
			needShowRelation = false;
		}
		bool flag12 = needShowRelation;
		if (flag12)
		{
			this._relationShipHandler.CharacterId = characterDisplayData.CharacterId;
		}
		else
		{
			this._relationShipHandler.CharacterId = -1;
		}
		this._showFavorabilityFlag = this.GetShowFavoriteFlag();
		bool isAnimal = GameData.Domains.Combat.SharedConstValue.CharId2AnimalId.ContainsKey(characterDisplayData.TemplateId);
		bool isAdoptiveFather = characterDisplayData.TemplateId == 880;
		bool canViewCharacter = !this.GetForbidViewCharacter() && !isAdoptiveFather;
		this._curCharacterId = characterDisplayData.CharacterId;
		this.SetCharacterMenuBtn(!isAnimal && canViewCharacter);
		bool flag13 = this.CharacterOnMapBlockTips.enabled = (characterDisplayData.CreatingType == 1);
		if (flag13)
		{
			this.CharacterOnMapBlockTips.Type = TipType.CharacterOnMapBlock;
			this.CharacterOnMapBlockTips.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CharId", this._curCharacterId).Set("IsEvent", this.IsLeftCharacter);
		}
	}

	// Token: 0x0600302D RID: 12333 RVA: 0x00178AC0 File Offset: 0x00176CC0
	private void RefreshAsActor(EventActorData actorData)
	{
		this._relationShipHandler.CharacterId = -1;
		this._consummateLevelHandler.CharacterId = -1;
		this.SetCharacterMenuBtn(false);
		EventActorsItem actorConfig = EventActors.Instance.GetItem(actorData.TemplateId);
		string roleNameKey = this.GetRoleNameKey();
		bool flag = !string.IsNullOrEmpty(actorConfig.Texture);
		if (flag)
		{
			bool flag2 = !string.IsNullOrEmpty(actorConfig.SpineName);
			if (flag2)
			{
				this.Avatar.RefreshAsSpine(actorConfig.SpineName, actorConfig.SpineSkinName);
			}
			else
			{
				ResLoader.LoadModOrGameResource<Texture2D>(this._npcAvatarTexturePath + "/" + actorConfig.Texture, new Action<Texture2D>(this.Avatar.Refresh), null);
			}
		}
		else
		{
			actorData.AvatarData.ClothDisplayId = actorData.ClothDisplayId;
			bool isLeftCharacter = this.IsLeftCharacter;
			if (isLeftCharacter)
			{
				bool leftActorShowMarriageLook = this.Data.ExtraData.LeftActorShowMarriageLook1;
				if (leftActorShowMarriageLook)
				{
					actorData.AvatarData.ChangeToMarriageStyle1();
				}
				else
				{
					bool leftActorShowMarriageLook2 = this.Data.ExtraData.LeftActorShowMarriageLook2;
					if (leftActorShowMarriageLook2)
					{
						actorData.AvatarData.ChangeToMarriageStyle2();
					}
				}
				actorData.ClothDisplayId = actorData.AvatarData.ClothDisplayId;
			}
			else
			{
				bool rightActorShowMarriageLook = this.Data.ExtraData.RightActorShowMarriageLook1;
				if (rightActorShowMarriageLook)
				{
					actorData.AvatarData.ChangeToMarriageStyle1();
				}
				else
				{
					bool rightActorShowMarriageLook2 = this.Data.ExtraData.RightActorShowMarriageLook2;
					if (rightActorShowMarriageLook2)
					{
						actorData.AvatarData.ChangeToMarriageStyle2();
					}
				}
				actorData.ClothDisplayId = actorData.AvatarData.ClothDisplayId;
			}
			this.Avatar.Refresh(actorData.AvatarData, (short)actorData.Age);
		}
		bool flag3 = !string.IsNullOrEmpty(roleNameKey);
		if (flag3)
		{
			this.NameLabel.text = LocalStringManager.Get(roleNameKey);
		}
		else
		{
			this.NameLabel.text = actorData.DisplayName;
		}
		this._showFavorabilityFlag = false;
	}

	// Token: 0x0600302E RID: 12334 RVA: 0x00178CA8 File Offset: 0x00176EA8
	private void RefreshAsTemplateCharacter(short templateId)
	{
		this._relationShipHandler.CharacterId = -1;
		this._consummateLevelHandler.CharacterId = -1;
		this.SetCharacterMenuBtn(false);
		CharacterItem config = Character.Instance.GetItem(templateId);
		bool flag = config != null;
		if (flag)
		{
			this.NameLabel.text = config.Surname + config.GivenName;
			bool targetRoleUseAlternativeName = this.Data.ExtraData.TargetRoleUseAlternativeName;
			if (targetRoleUseAlternativeName)
			{
				bool flag2 = !string.IsNullOrEmpty(config.AnonymousTitle);
				if (flag2)
				{
					this.NameLabel.text = config.AnonymousTitle;
				}
			}
			bool flag3 = this.Avatar.Size == AvatarSize.Big && !string.IsNullOrEmpty(config.FixedAvatarSpineName);
			if (flag3)
			{
				this.Avatar.RefreshAsSpine(config.FixedAvatarSpineName, config.FixedAvatarSpineSkin);
			}
			else
			{
				bool flag4 = !string.IsNullOrEmpty(config.FixedAvatarName);
				if (flag4)
				{
					ResLoader.LoadModOrGameResource<Texture2D>(this._npcAvatarTexturePath + "/" + config.FixedAvatarName, new Action<Texture2D>(this.Avatar.Refresh), null);
				}
			}
		}
	}

	// Token: 0x0600302F RID: 12335 RVA: 0x00178DCC File Offset: 0x00176FCC
	private void RefreshAsMerchant(CaravanDisplayData caravanDisplayData)
	{
		this._relationShipHandler.CharacterId = -1;
		this._consummateLevelHandler.CharacterId = -1;
		MerchantTypeItem merchantTypeConfig = Config.MerchantType.Instance[Merchant.Instance[(int)caravanDisplayData.MerchantTemplateId].MerchantType];
		bool flag = !string.IsNullOrEmpty(merchantTypeConfig.CaravanSpineName);
		if (flag)
		{
			this.Avatar.RefreshAsSpine(merchantTypeConfig.CaravanSpineName, null);
		}
		else
		{
			ResLoader.LoadModOrGameResource<Texture2D>(this._npcAvatarTexturePath + "/" + merchantTypeConfig.CaravanAvatar, new Action<Texture2D>(this.Avatar.Refresh), null);
		}
		this.NameLabel.text = merchantTypeConfig.Name;
		this.SetCharacterMenuBtn(false);
	}

	// Token: 0x06003030 RID: 12336 RVA: 0x00178E88 File Offset: 0x00177088
	private void RefreshAsXiangShuAvatar(sbyte xiangshuAvatarId, sbyte displayStatus)
	{
		this._relationShipHandler.CharacterId = -1;
		this._consummateLevelHandler.CharacterId = -1;
		this.SetCharacterMenuBtn(false);
		string spineName = EventWindowCharacter.GetXiangshuSpineName(xiangshuAvatarId, displayStatus);
		bool flag = !string.IsNullOrEmpty(spineName);
		if (flag)
		{
			this.Avatar.RefreshAsSpine(spineName, null);
		}
		else
		{
			string xiangshuAvtarName = EventModel.XiangShuAvatarDisplayTextures[(int)xiangshuAvatarId][(int)displayStatus];
			ResLoader.LoadModOrGameResource<Texture2D>(this._npcAvatarTexturePath + "/" + xiangshuAvtarName, new Action<Texture2D>(this.Avatar.Refresh), null);
		}
		bool flag2 = displayStatus < 3;
		short templateId;
		if (flag2)
		{
			templateId = XiangshuAvatarIds.JuniorXiangshuTemplateIds[(int)this.Model.RightRoleXiangShuAvatarId];
		}
		else
		{
			templateId = XiangshuAvatarIds.XiangshuBossBeginIds[(int)this.Model.RightRoleXiangShuAvatarId];
		}
		bool flag3 = templateId > -1;
		if (flag3)
		{
			CharacterItem config = Character.Instance[templateId];
			this.NameLabel.text = config.Surname + config.GivenName;
		}
		else
		{
			this.NameLabel.text = string.Empty;
		}
		CharacterDisplayData characterDisplayData = this.GetCharacterDisplayData();
		bool flag4 = characterDisplayData != null;
		if (flag4)
		{
			this._showFavorabilityFlag = (displayStatus == 1 || displayStatus == 2);
			bool flag5 = this._basicInfoMonitor != null;
			if (flag5)
			{
				this._basicInfoMonitor.RemoveDebtsOfTaiwuListener(new Action(this.UpdateDebtEntranceVisible));
			}
			this._favorabilityHandler.CharacterId = characterDisplayData.CharacterId;
			bool rightForbiddenConsummateLevel = this.GetRightForbiddenConsummateLevel();
			if (rightForbiddenConsummateLevel)
			{
				this._consummateLevelHandler.CharacterId = -1;
			}
			else
			{
				this._consummateLevelHandler.CharacterId = characterDisplayData.CharacterId;
			}
			this._basicInfoMonitor = this._favorabilityHandler.GetMonitor<BasicInfoMonitor>();
			bool flag6 = this._basicInfoMonitor != null;
			if (flag6)
			{
				this._basicInfoMonitor.AddDebtsOfTaiwuListener(new Action(this.UpdateDebtEntranceVisible));
				this._basicInfoMonitor.AddFavorabilityListener(new Action(this.FillFavorProgress));
				bool init = this._basicInfoMonitor.Init;
				if (init)
				{
					this._basicInfoMonitor.Refresh();
				}
			}
		}
		else
		{
			this._showFavorabilityFlag = false;
		}
	}

	// Token: 0x06003031 RID: 12337 RVA: 0x0017909C File Offset: 0x0017729C
	private static string GetXiangshuSpineName(sbyte xiangshuAvatarId, sbyte displayStatus)
	{
		int bossNumber = (int)(xiangshuAvatarId + 1);
		if (!true)
		{
		}
		string text;
		switch (displayStatus)
		{
		case 0:
			text = string.Format("character_boss{0}", bossNumber);
			break;
		case 1:
			text = string.Format("character_bosskid{0}_joy", bossNumber);
			break;
		case 2:
			text = string.Format("character_bosskid{0}_sad", bossNumber);
			break;
		case 3:
			text = string.Format("character_boss{0}_joy", bossNumber);
			break;
		case 4:
			text = string.Format("character_boss{0}_sad", bossNumber);
			break;
		default:
			text = null;
			break;
		}
		if (!true)
		{
		}
		string folderAndFileName = text;
		bool flag = string.IsNullOrEmpty(folderAndFileName);
		string result;
		if (flag)
		{
			result = null;
		}
		else
		{
			result = "DynamicIllustration/" + folderAndFileName + "/" + folderAndFileName;
		}
		return result;
	}

	// Token: 0x06003032 RID: 12338 RVA: 0x00179160 File Offset: 0x00177360
	private void RefreshFavorView()
	{
		bool activeInHierarchy = base.gameObject.activeInHierarchy;
		if (activeInHierarchy)
		{
			base.StopCoroutine(this.UpdateFavorAndRelationShipShow());
			base.StartCoroutine(this.UpdateFavorAndRelationShipShow());
		}
		else
		{
			this._waitRefreshFavorFlag = true;
		}
	}

	// Token: 0x06003033 RID: 12339 RVA: 0x001791A2 File Offset: 0x001773A2
	private IEnumerator UpdateFavorAndRelationShipShow()
	{
		this._waitRefreshFavorFlag = false;
		ConsummateLevelMonitor consummateLevelMonitor = this._consummateLevelHandler.GetMonitor<ConsummateLevelMonitor>();
		bool flag = consummateLevelMonitor == null || this._basicInfoMonitor == null;
		if (flag)
		{
			this.FavorInfoRootObj.SetActive(false);
			this.RelationLayout.gameObject.SetActive(false);
			yield break;
		}
		yield return new WaitUntil(() => consummateLevelMonitor.Init && this._basicInfoMonitor.Init);
		bool flag2 = consummateLevelMonitor.CreatingType != 1;
		if (flag2)
		{
			CharacterItem charItemConfig = Character.Instance.GetItem(this._basicInfoMonitor.NameRelatedData.CharTemplateId);
			bool flag3 = !charItemConfig.IsFavorabilityDisplay;
			if (flag3)
			{
				this.FavorInfoRootObj.SetActive(false);
				this.RelationLayout.gameObject.SetActive(false);
				yield break;
			}
			charItemConfig = null;
		}
		this.FavorInfoRootObj.SetActive(this._showFavorabilityFlag);
		this.RelationLayout.gameObject.SetActive(this._relationShipHandler.HasRelationShowed);
		RectTransform relationRect = this.RelationLayout.GetComponent<RectTransform>();
		float height = 0f;
		bool flag4 = this._relationShipHandler.RelationShowedCount % 4 == 0;
		if (flag4)
		{
			height = (float)(this._relationShipHandler.RelationShowedCount / this.RelationLayout.constraintCount * 60);
		}
		else
		{
			height = (float)((this._relationShipHandler.RelationShowedCount / this.RelationLayout.constraintCount + 1) * 60);
		}
		relationRect.sizeDelta = new Vector2(relationRect.sizeDelta.x, height);
		yield break;
	}

	// Token: 0x06003034 RID: 12340 RVA: 0x001791B1 File Offset: 0x001773B1
	private void SetJiaoMouseTip(ItemDisplayData jiaoDisplayData)
	{
		ExtraDomainMethod.AsyncCall.GetJiaoLoongDisplayDataByItemKey(null, jiaoDisplayData.Key, delegate(int offset, RawDataPool dataPool)
		{
			JiaoLoongDisplayData displayData = new JiaoLoongDisplayData();
			Serializer.Deserialize(dataPool, offset, ref displayData);
			TooltipInvoker mouseTip = this.ShowCharacterMenuTips.GetComponent<TooltipInvoker>();
			TooltipInvoker tooltipInvoker = mouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			mouseTip.RuntimeParam.Clear();
			mouseTip.NeedRefresh = true;
			mouseTip.RuntimeParam.SetObject("JiaoLoongData", displayData);
			mouseTip.RuntimeParam.Set("DisableCompare", true);
			this.ShowCharacterMenuTips.SetActive(true);
		});
	}

	// Token: 0x06003035 RID: 12341 RVA: 0x001791D0 File Offset: 0x001773D0
	private string GetFavorFillSpriteNameByFavorType(sbyte favorType)
	{
		if (!true)
		{
		}
		string result;
		switch (favorType)
		{
		case -6:
			result = "taiwuevent_01_progress_8";
			break;
		case -5:
			result = "taiwuevent_01_progress_8";
			break;
		case -4:
			result = "taiwuevent_01_progress_8";
			break;
		case -3:
			result = "taiwuevent_01_progress_7";
			break;
		case -2:
			result = "taiwuevent_01_progress_7";
			break;
		case -1:
			result = "taiwuevent_01_progress_7";
			break;
		case 0:
			result = "taiwuevent_01_progress_6";
			break;
		case 1:
			result = "taiwuevent_01_progress_5";
			break;
		case 2:
			result = "taiwuevent_01_progress_4";
			break;
		case 3:
			result = "taiwuevent_01_progress_3";
			break;
		case 4:
			result = "taiwuevent_01_progress_2";
			break;
		case 5:
			result = "taiwuevent_01_progress_1";
			break;
		case 6:
			result = "taiwuevent_01_progress_0";
			break;
		default:
			result = "taiwuevent_01_progress_6";
			break;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06003036 RID: 12342 RVA: 0x0017929C File Offset: 0x0017749C
	private void FillFavorProgress()
	{
		bool flag = this.GetCharacterDisplayData() == null || this._basicInfoMonitor == null || null == this;
		if (!flag)
		{
			CharacterItem charConfig = Character.Instance[this._basicInfoMonitor.NameRelatedData.CharTemplateId];
			bool flag2 = this._notedFavorData[1] < 30000 && (this._notedFavorData[1] > -30000 || charConfig.CreatingType != 1) && this._notedFavorData[1] != (int)this._basicInfoMonitor.FavorabilityToTaiwu && this._notedFavorData[0] == this._basicInfoMonitor.CharacterId;
			if (flag2)
			{
				base.CancelInvoke();
				this.ResetFavorChange();
				bool flag3 = !base.gameObject.activeInHierarchy;
				if (flag3)
				{
					this._onEnableAction = delegate()
					{
						this.CanvasChanger.overrideSorting = true;
					};
				}
				else
				{
					this.CanvasChanger.overrideSorting = true;
				}
				bool isDown = this._notedFavorData[1] > (int)this._basicInfoMonitor.FavorabilityToTaiwu;
				GameObject targetObj = isDown ? this.FavorLevelDownEffectObject : this.FavorLevelUpEffectObject;
				string soundName = isDown ? "ui_favorite_down" : "ui_favorite_up";
				AudioManager.Instance.PlaySound(soundName, false, false);
				targetObj.SetActive(true);
				foreach (object obj in targetObj.transform)
				{
					Transform child = (Transform)obj;
					ParticleSystem particle = child.GetComponent<ParticleSystem>();
					bool flag4 = null != particle;
					if (flag4)
					{
						particle.Play(true);
					}
					SkeletonGraphic spineAnimation = child.GetComponent<SkeletonGraphic>();
					bool flag5 = null != spineAnimation;
					if (flag5)
					{
						spineAnimation.AnimationState.Tracks.Items[0].TrackTime = 0f;
						spineAnimation.AnimationState.SetAnimation(0, "animation", false);
						spineAnimation.AnimationState.Apply(spineAnimation.Skeleton);
					}
					child.gameObject.SetActive(true);
				}
				base.Invoke("ResetFavorChange", 3f);
				this.FavorProgressFill.SetSpriteOnly(this.GetFavorFillSpriteNameByFavorType(FavorabilityType.GetFavorabilityType(this._basicInfoMonitor.FavorabilityToTaiwu)), false, null);
				ValueTuple<short, short> favorabilityRange = FavorabilityType.GetFavorabilityRange(this._basicInfoMonitor.FavorabilityToTaiwu);
				short min = favorabilityRange.Item1;
				short max = favorabilityRange.Item2;
				float newProgress = (float)(this._basicInfoMonitor.FavorabilityToTaiwu - min) / (float)(max - min);
				this.FavorProgressFill.DOFillAmount(newProgress, 0.3f);
			}
			else
			{
				this.FavorProgressFill.SetSpriteOnly(this.GetFavorFillSpriteNameByFavorType(FavorabilityType.GetFavorabilityType(this._basicInfoMonitor.FavorabilityToTaiwu)), false, null);
				ValueTuple<short, short> favorabilityRange2 = FavorabilityType.GetFavorabilityRange(this._basicInfoMonitor.FavorabilityToTaiwu);
				short min2 = favorabilityRange2.Item1;
				short max2 = favorabilityRange2.Item2;
				float progress = (float)(this._basicInfoMonitor.FavorabilityToTaiwu - min2) / (float)(max2 - min2);
				this.FavorProgressFill.fillAmount = progress;
			}
			this._notedFavorData[0] = this._basicInfoMonitor.CharacterId;
			this._notedFavorData[1] = (int)this._basicInfoMonitor.FavorabilityToTaiwu;
		}
	}

	// Token: 0x06003037 RID: 12343 RVA: 0x001795D4 File Offset: 0x001777D4
	private void UpdateInjuryInfoTipsVisible()
	{
		this._injuryBtn = base.CGet<CButtonObsolete>("InjuryBtn");
		TooltipInvoker injuryTips = this._injuryBtn.GetComponent<TooltipInvoker>();
		TooltipInvoker tooltipInvoker = injuryTips;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
		}
		injuryTips.RuntimeParam.Set("characterId", this._curCharacterId);
	}

	// Token: 0x06003038 RID: 12344 RVA: 0x00179630 File Offset: 0x00177830
	private void UpdateDebtEntranceVisible()
	{
		bool flag = null == this.DebtEntranceButton;
		if (!flag)
		{
			bool hasDebt = this._favorabilityHandler.GetMonitor<BasicInfoMonitor>().HasAlertness;
			this.DebtEntranceButton.gameObject.SetActive(hasDebt);
			this.RefreshFavorView();
			bool flag2 = hasDebt;
			if (flag2)
			{
				CharacterDomainMethod.AsyncCall.CalcMaxFavorabilityToTaiwuById(null, this._favorabilityHandler.CharacterId, delegate(int offset, RawDataPool dataPool)
				{
					short maxFavor = 0;
					Serializer.Deserialize(dataPool, offset, ref maxFavor);
					TooltipInvoker mouseTip = this.DebtEntranceButton.GetComponent<TooltipInvoker>();
					bool flag3 = mouseTip != null;
					if (flag3)
					{
						mouseTip.Type = TipType.Simple;
						mouseTip.RuntimeParam = new ArgumentBox();
						mouseTip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_DebtOverview_Title));
						string tipsContent = LocalStringManager.Get(LanguageKey.LK_DebtOverview_FavorTopLimit);
						tipsContent = string.Concat(new string[]
						{
							tipsContent,
							"<SpName=",
							CommonUtils.GetFavorIconLegacy(maxFavor),
							"> ",
							CommonUtils.GetFavorString(maxFavor)
						});
						mouseTip.RuntimeParam.Set("arg1", tipsContent);
					}
				});
			}
		}
	}

	// Token: 0x06003039 RID: 12345 RVA: 0x0017969F File Offset: 0x0017789F
	private void ResetFavorChange()
	{
		this.CanvasChanger.overrideSorting = false;
		this.FavorLevelUpEffectObject.SetActive(false);
		this.FavorLevelDownEffectObject.SetActive(false);
	}

	// Token: 0x0600303A RID: 12346 RVA: 0x001796C9 File Offset: 0x001778C9
	private void PlayAvatarAppearAnimation()
	{
	}

	// Token: 0x0600303B RID: 12347 RVA: 0x001796CC File Offset: 0x001778CC
	private void OnShowCharacterMenuClick(int pageIndex)
	{
		bool flag = -1 == this._curCharacterId;
		if (flag)
		{
			GLog.Warn("Can not show CharacterMenu because _curCharacterId is -1");
		}
		else
		{
			bool flag2 = this.Data == null;
			if (!flag2)
			{
				Action<int, int, bool> onViewCharacter = this.OnViewCharacter;
				if (onViewCharacter != null)
				{
					onViewCharacter(this._curCharacterId, pageIndex, this.IsLeftCharacter);
				}
			}
		}
	}

	// Token: 0x0600303C RID: 12348 RVA: 0x00179724 File Offset: 0x00177924
	private void TryInit()
	{
		bool initFlag = this.InitFlag;
		if (!initFlag)
		{
			this._favorabilityHandler = new CharacterFavorability(this.FavorabilityIcon, this.FavorabilityLabel, null, null, null);
			this._relationShipHandler = new CharacterRelationShip(this.RelationLayout.GetComponent<Refers>());
			this._consummateLevelHandler = new CharacterConsummateLevel(this.ConsummateLevelIcon, this.ConsummateLevelLabel, this.ConsummateLevelName);
			this._behaviorHandler = new CharacterBehavior(base.CGet<Refers>("BehaviorHolder"), false);
			this._fameHandler = new CharacterFame(base.CGet<Refers>("FameHolder"), false);
			this._identityHandler = new CharacterOrganization(null, base.CGet<Refers>("IdentityHolder"));
			this.ShowCharacterMenuBtn.ClearAndAddListener(delegate
			{
				this.OnShowCharacterMenuClick(-1);
			});
			this._injuryBtn = base.CGet<CButtonObsolete>("InjuryBtn");
			this._injuryBtn.ClearAndAddListener(delegate
			{
				this.OnShowCharacterMenuClick(0);
			});
			this._itemBtn = base.CGet<CButtonObsolete>("ItemBtn");
			this._itemBtn.ClearAndAddListener(delegate
			{
				this.OnShowCharacterMenuClick(3);
			});
			this._relationBtn = base.CGet<CButtonObsolete>("RelationBtn");
			this._relationBtn.ClearAndAddListener(delegate
			{
				this.OnShowCharacterMenuClick(7);
			});
			this._lifeRecordBtn = base.CGet<CButtonObsolete>("LifeRecordBtn");
			this._lifeRecordBtn.ClearAndAddListener(delegate
			{
				this.OnShowCharacterMenuClick(8);
			});
			this.ShowCharacterMenuTips.SetActive(false);
			this.FavorInfoRootObj.SetActive(false);
			this.RelationLayout.gameObject.SetActive(false);
			this.CharacterOnMapBlockTips.enabled = false;
			this._maskHolder = base.CGet<Refers>("JieqingMaskHolder");
			this._jieqingPutOnMaskAdult = this._maskHolder.CGet<ParticleSystem>("eff_jieqingtangram_mianju_chuan1");
			this._jieqingPutOnMaskChild = this._maskHolder.CGet<ParticleSystem>("eff_jieqingtangram_mianju_chuan2");
			this._jieqingTakeOffMaskAdult = this._maskHolder.CGet<ParticleSystem>("eff_jieqingtangram_mianju_tuo1");
			this._jieqingTakeOffMaskChild = this._maskHolder.CGet<ParticleSystem>("eff_jieqingtangram_mianju_tuo2");
			this.InitFlag = true;
		}
	}

	// Token: 0x0600303D RID: 12349 RVA: 0x00179939 File Offset: 0x00177B39
	private bool GetHasCharacter()
	{
		return this.IsLeftCharacter ? this.HasLeftCharacter() : this.HasRightCharacter();
	}

	// Token: 0x0600303E RID: 12350 RVA: 0x00179954 File Offset: 0x00177B54
	private bool HasLeftCharacter()
	{
		bool flag = this.Data.MainCharacter != null;
		return flag || this.Data.ExtraData.LeftActorData != null;
	}

	// Token: 0x0600303F RID: 12351 RVA: 0x00179990 File Offset: 0x00177B90
	private bool HasRightCharacter()
	{
		bool flag = this.Data.TargetCharacter != null;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			bool flag2 = this.Data.ExtraData.CaravanData != null;
			if (flag2)
			{
				result = true;
			}
			else
			{
				bool flag3 = -1 != this.Data.ExtraData.HereticTemplateId;
				result = (flag3 || this.Data.ExtraData.ActorData != null);
			}
		}
		return result;
	}

	// Token: 0x06003040 RID: 12352 RVA: 0x00179A04 File Offset: 0x00177C04
	private CharacterDisplayData GetCharacterDisplayData()
	{
		bool flag = this.Data == null;
		CharacterDisplayData result;
		if (flag)
		{
			result = null;
		}
		else
		{
			result = (this.IsLeftCharacter ? this.Data.MainCharacter : this.Data.TargetCharacter);
		}
		return result;
	}

	// Token: 0x06003041 RID: 12353 RVA: 0x00179A48 File Offset: 0x00177C48
	private EventActorData GetEventActorData()
	{
		bool flag = this.Data == null;
		EventActorData result;
		if (flag)
		{
			result = null;
		}
		else
		{
			result = (this.IsLeftCharacter ? this.Data.ExtraData.LeftActorData : this.Data.ExtraData.ActorData);
		}
		return result;
	}

	// Token: 0x06003042 RID: 12354 RVA: 0x00179A98 File Offset: 0x00177C98
	private ValueTuple<sbyte, sbyte> GetXiangShuAvatarId()
	{
		bool flag = this.Data == null;
		ValueTuple<sbyte, sbyte> result;
		if (flag)
		{
			result = new ValueTuple<sbyte, sbyte>(9, -1);
		}
		else
		{
			bool flag2 = !this.IsLeftCharacter;
			if (flag2)
			{
				result = new ValueTuple<sbyte, sbyte>(this.Model.RightRoleXiangShuAvatarId, this.Model.RightRoleXiangShuDisplayStatus);
			}
			else
			{
				result = new ValueTuple<sbyte, sbyte>(9, -1);
			}
		}
		return result;
	}

	// Token: 0x06003043 RID: 12355 RVA: 0x00179AF8 File Offset: 0x00177CF8
	private CaravanDisplayData GetCaravanDisplayData()
	{
		bool flag = this.Data == null;
		CaravanDisplayData result;
		if (flag)
		{
			result = null;
		}
		else
		{
			result = (this.IsLeftCharacter ? null : this.Data.ExtraData.CaravanData);
		}
		return result;
	}

	// Token: 0x06003044 RID: 12356 RVA: 0x00179B38 File Offset: 0x00177D38
	private ItemDisplayData GetJiaoMouseTipDisplayData()
	{
		bool flag = this.Data == null;
		ItemDisplayData result;
		if (flag)
		{
			result = null;
		}
		else
		{
			result = (this.IsLeftCharacter ? null : this.Data.ExtraData.JiaoDisplayData);
		}
		return result;
	}

	// Token: 0x06003045 RID: 12357 RVA: 0x00179B78 File Offset: 0x00177D78
	private short GetTemplateIdOfCharacter()
	{
		bool flag = this.Data == null;
		short result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			result = (this.IsLeftCharacter ? -1 : this.Data.ExtraData.HereticTemplateId);
		}
		return result;
	}

	// Token: 0x06003046 RID: 12358 RVA: 0x00179BB8 File Offset: 0x00177DB8
	private bool GetForbidViewCharacter()
	{
		bool flag = this.Data == null;
		return flag || (this.IsLeftCharacter ? (this.Data.ExtraData.ForbidViewSelf || this.Data.MainCharacter == null || !Character.Instance[this.Data.MainCharacter.TemplateId].CanOpenCharacterMenu) : (this.Data.ExtraData.ForbidViewCharacter || this.Data.TargetCharacter == null || !Character.Instance[this.Data.TargetCharacter.TemplateId].CanOpenCharacterMenu));
	}

	// Token: 0x06003047 RID: 12359 RVA: 0x00179C70 File Offset: 0x00177E70
	private bool GetUseAlternativeName()
	{
		bool flag = this.Data == null;
		return !flag && (this.IsLeftCharacter ? this.Data.ExtraData.MainRoleUseAlternativeName : this.Data.ExtraData.TargetRoleUseAlternativeName);
	}

	// Token: 0x06003048 RID: 12360 RVA: 0x00179CC0 File Offset: 0x00177EC0
	private string GetRoleNameKey()
	{
		bool flag = this.Data == null;
		string result;
		if (flag)
		{
			result = string.Empty;
		}
		else
		{
			result = (this.IsLeftCharacter ? this.Model.LeftRoleNameKey : this.Model.RightRoleNameKey);
		}
		return result;
	}

	// Token: 0x06003049 RID: 12361 RVA: 0x00179D08 File Offset: 0x00177F08
	private bool GetRightCharacterShadow()
	{
		bool flag = this.Data == null;
		return !flag && !this.IsLeftCharacter && this.Data.ExtraData.RightCharacterShadow;
	}

	// Token: 0x0600304A RID: 12362 RVA: 0x00179D48 File Offset: 0x00177F48
	private bool GetRightForbiddenConsummateLevel()
	{
		bool flag = this.Data == null;
		return !flag && !this.IsLeftCharacter && this.Data.ExtraData.RightForbiddenConsummateLevel;
	}

	// Token: 0x0600304B RID: 12363 RVA: 0x00179D88 File Offset: 0x00177F88
	private bool GetShowFavoriteFlag()
	{
		bool flag = this.Data == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			CharacterDisplayData characterDisplayData = this.GetCharacterDisplayData();
			bool flag2 = characterDisplayData != null && characterDisplayData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = this.GetEventActorData() != null;
				result = (!flag3 && (this.IsLeftCharacter ? (!this.Data.ExtraData.HideLeftFavorability) : (!this.Data.ExtraData.HideRightFavorability)));
			}
		}
		return result;
	}

	// Token: 0x0600304C RID: 12364 RVA: 0x00179E18 File Offset: 0x00178018
	private bool GetForbidShowFavorChangeEffectFlag()
	{
		bool flag = this.Data == null;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			CharacterDisplayData characterDisplayData = this.GetCharacterDisplayData();
			bool flag2 = characterDisplayData == null;
			if (flag2)
			{
				result = true;
			}
			else
			{
				bool flag3 = characterDisplayData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				if (flag3)
				{
					result = true;
				}
				else
				{
					bool flag4 = this.GetEventActorData() != null;
					result = (flag4 || (this.IsLeftCharacter ? this.Data.ExtraData.LeftForbidShowFavorChangeEffect : this.Data.ExtraData.RightForbidShowFavorChangeEffect));
				}
			}
		}
		return result;
	}

	// Token: 0x0600304D RID: 12365 RVA: 0x00179EA8 File Offset: 0x001780A8
	private void SetCharacterMenuBtn(bool show)
	{
		this.ShowCharacterMenuBtn.gameObject.SetActive(show);
		base.CGet<GameObject>("QuickBtn").SetActive(show);
		if (show)
		{
			this.SetRelationLifeRecordBtn();
		}
	}

	// Token: 0x0600304E RID: 12366 RVA: 0x00179EE8 File Offset: 0x001780E8
	private void SetRelationLifeRecordBtn()
	{
		EventWindowCharacter.<>c__DisplayClass84_0 CS$<>8__locals1 = new EventWindowCharacter.<>c__DisplayClass84_0();
		CS$<>8__locals1.<>4__this = this;
		base.CGet<GameObject>("QuickBtnMask").SetActive(true);
		CS$<>8__locals1.avatarInfoMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AvatarInfoMonitor>(this._curCharacterId, false);
		bool flag = !CS$<>8__locals1.avatarInfoMonitor.Init;
		if (flag)
		{
			CS$<>8__locals1.avatarInfoMonitor.AddOnAvatarDataChangeEventListener(new Action(CS$<>8__locals1.<SetRelationLifeRecordBtn>g__ValidateStory|0));
		}
		else
		{
			base.CGet<GameObject>("QuickBtnMask").SetActive(false);
		}
	}

	// Token: 0x0600304F RID: 12367 RVA: 0x00179F70 File Offset: 0x00178170
	private bool GetCanViewLifeRecord(int charId)
	{
		BasicInfoMonitor basicInfoMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<BasicInfoMonitor>(charId, false);
		bool flag = (basicInfoMonitor.NameRelatedData.FullName.Type & 16) != 0;
		return !flag;
	}

	// Token: 0x06003050 RID: 12368 RVA: 0x00179FB0 File Offset: 0x001781B0
	public void JieqingMaskRefresh(bool needRefresh, List<int> jieqingMaskCharIdList)
	{
		RectTransform rectTransform = this._maskHolder.GetComponent<RectTransform>();
		PositionFollower positionFollower = rectTransform.GetComponent<PositionFollower>();
		RectTransform targetRectTransform = positionFollower.Target.GetComponent<RectTransform>();
		rectTransform.SetWidth(targetRectTransform.sizeDelta.x);
		CharacterDisplayData characterDisplayData = this.GetCharacterDisplayData();
		bool flag = characterDisplayData != null && !needRefresh;
		if (flag)
		{
			bool hasMask = jieqingMaskCharIdList != null && jieqingMaskCharIdList.Contains(characterDisplayData.CharacterId);
			sbyte ageGroup = AgeGroup.GetAgeGroup(characterDisplayData.PhysiologicalAge);
			bool flag2 = (!this._jieqingHasMask && !hasMask) || (this._jieqingHasMask && hasMask);
			if (flag2)
			{
				return;
			}
			bool flag3 = ageGroup >= 2 && !this._jieqingHasMask && hasMask;
			ParticleSystem particle;
			if (flag3)
			{
				particle = this._jieqingPutOnMaskAdult;
			}
			else
			{
				bool flag4 = ageGroup >= 2 && this._jieqingHasMask && !hasMask;
				if (flag4)
				{
					particle = this._jieqingTakeOffMaskAdult;
				}
				else
				{
					bool flag5 = ageGroup < 2 && !this._jieqingHasMask && hasMask;
					if (flag5)
					{
						particle = this._jieqingPutOnMaskChild;
					}
					else
					{
						particle = this._jieqingTakeOffMaskChild;
					}
				}
			}
			this._maskHolder.transform.SetSiblingIndex(this._maskHolder.transform.parent.Find("FrontHair").GetSiblingIndex() - 1);
			particle.Play();
			this._jieqingHasMask = hasMask;
		}
		if (needRefresh)
		{
			this.Refresh();
		}
	}

	// Token: 0x06003051 RID: 12369 RVA: 0x0017A114 File Offset: 0x00178314
	public void ResetJieqingHasMaskState()
	{
		this._jieqingHasMask = false;
	}

	// Token: 0x06003052 RID: 12370 RVA: 0x0017A120 File Offset: 0x00178320
	public void PointerTriggerEnterShowLifeRecord()
	{
		CharacterLifeRecords characterLifeRecords = base.CGet<CharacterLifeRecords>("CharacterLifeRecords");
		characterLifeRecords.CharacterId = this._curCharacterId;
		characterLifeRecords.gameObject.SetActive(true);
	}

	// Token: 0x06003053 RID: 12371 RVA: 0x0017A154 File Offset: 0x00178354
	public void PointerTriggerExitShowLifeRecord()
	{
		base.DelayCall(delegate
		{
			CharacterLifeRecords characterLifeRecords = base.CGet<CharacterLifeRecords>("CharacterLifeRecords");
			PointerTrigger lifeRecordBtnPointerTrigger = this._lifeRecordBtn.GetComponent<PointerTrigger>();
			PointerTrigger lifeRecordPointerTrigger = characterLifeRecords.GetComponent<PointerTrigger>();
			bool flag = !lifeRecordBtnPointerTrigger.AtEnter && !lifeRecordPointerTrigger.AtEnter;
			if (flag)
			{
				characterLifeRecords.gameObject.SetActive(false);
			}
		}, 0.1f);
	}

	// Token: 0x040022E9 RID: 8937
	public bool IsLeftCharacter;

	// Token: 0x040022EA RID: 8938
	public Canvas CanvasChanger;

	// Token: 0x040022EB RID: 8939
	public TextMeshProUGUI NameLabel;

	// Token: 0x040022EC RID: 8940
	public Game.Components.Avatar.Avatar Avatar;

	// Token: 0x040022ED RID: 8941
	public CButtonObsolete ShowCharacterMenuBtn;

	// Token: 0x040022EE RID: 8942
	public GameObject ShowCharacterMenuTips;

	// Token: 0x040022EF RID: 8943
	public CImage FavorabilityIcon;

	// Token: 0x040022F0 RID: 8944
	public TextMeshProUGUI FavorabilityLabel;

	// Token: 0x040022F1 RID: 8945
	public CImage FavorProgressFill;

	// Token: 0x040022F2 RID: 8946
	public GameObject FavorInfoRootObj;

	// Token: 0x040022F3 RID: 8947
	public GameObject FavorLevelUpEffectObject;

	// Token: 0x040022F4 RID: 8948
	public GameObject FavorLevelDownEffectObject;

	// Token: 0x040022F5 RID: 8949
	public CButtonObsolete DebtEntranceButton;

	// Token: 0x040022F6 RID: 8950
	public GridLayoutGroup RelationLayout;

	// Token: 0x040022F7 RID: 8951
	public CImage ConsummateLevelIcon;

	// Token: 0x040022F8 RID: 8952
	public TextMeshProUGUI ConsummateLevelLabel;

	// Token: 0x040022F9 RID: 8953
	public TextMeshProUGUI ConsummateLevelName;

	// Token: 0x040022FA RID: 8954
	public TooltipInvoker CharacterOnMapBlockTips;

	// Token: 0x040022FB RID: 8955
	public Action<int, int, bool> OnViewCharacter;

	// Token: 0x040022FC RID: 8956
	private EventModel _model;

	// Token: 0x040022FD RID: 8957
	private readonly string _npcAvatarTexturePath = "NpcFace/BigFace";

	// Token: 0x040022FE RID: 8958
	private CButtonObsolete _injuryBtn;

	// Token: 0x040022FF RID: 8959
	private CButtonObsolete _itemBtn;

	// Token: 0x04002300 RID: 8960
	private CButtonObsolete _relationBtn;

	// Token: 0x04002301 RID: 8961
	private CButtonObsolete _lifeRecordBtn;

	// Token: 0x04002302 RID: 8962
	private int _curCharacterId;

	// Token: 0x04002303 RID: 8963
	private int[] _notedFavorData = new int[]
	{
		-1,
		26
	};

	// Token: 0x04002304 RID: 8964
	private BasicInfoMonitor _basicInfoMonitor;

	// Token: 0x04002305 RID: 8965
	private CharacterFavorability _favorabilityHandler;

	// Token: 0x04002306 RID: 8966
	private CharacterRelationShip _relationShipHandler;

	// Token: 0x04002307 RID: 8967
	private CharacterConsummateLevel _consummateLevelHandler;

	// Token: 0x04002308 RID: 8968
	private CharacterBehavior _behaviorHandler;

	// Token: 0x04002309 RID: 8969
	private CharacterFame _fameHandler;

	// Token: 0x0400230A RID: 8970
	private CharacterOrganization _identityHandler;

	// Token: 0x0400230C RID: 8972
	private bool _showFavorabilityFlag;

	// Token: 0x0400230D RID: 8973
	private bool _forbidShowFavorChangeEffectFlag;

	// Token: 0x0400230E RID: 8974
	private bool _waitRefreshFavorFlag;

	// Token: 0x0400230F RID: 8975
	private Action _onEnableAction;

	// Token: 0x04002310 RID: 8976
	private bool _jieqingHasMask;

	// Token: 0x04002311 RID: 8977
	private Refers _maskHolder;

	// Token: 0x04002312 RID: 8978
	private ParticleSystem _jieqingPutOnMaskAdult;

	// Token: 0x04002313 RID: 8979
	private ParticleSystem _jieqingPutOnMaskChild;

	// Token: 0x04002314 RID: 8980
	private ParticleSystem _jieqingTakeOffMaskAdult;

	// Token: 0x04002315 RID: 8981
	private ParticleSystem _jieqingTakeOffMaskChild;
}
