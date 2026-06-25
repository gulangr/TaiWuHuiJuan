using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using CharacterDataMonitor;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Character.LifeRecord;
using Game.Components.Common;
using Game.Views.MouseTips;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character;
using GameData.Domains.Character.Alertness;
using GameData.Domains.Character.Display;
using GameData.Domains.Character.Relation;
using GameData.Domains.Combat;
using GameData.Domains.Extra;
using GameData.Domains.Item.Display;
using GameData.Domains.Merchant;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using GameData.Domains.World;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.EventWindow
{
	// Token: 0x02000EFD RID: 3837
	public class EventWindowCharacter : MonoBehaviour
	{
		// Token: 0x17001409 RID: 5129
		// (get) Token: 0x0600B0B8 RID: 45240 RVA: 0x00509225 File Offset: 0x00507425
		private int TaiwuCharId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x1700140A RID: 5130
		// (get) Token: 0x0600B0B9 RID: 45241 RVA: 0x00509234 File Offset: 0x00507434
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

		// Token: 0x1700140B RID: 5131
		// (get) Token: 0x0600B0BA RID: 45242 RVA: 0x00509264 File Offset: 0x00507464
		private TaiwuEventDisplayData Data
		{
			get
			{
				return this.Model.DisplayingEventData;
			}
		}

		// Token: 0x1700140C RID: 5132
		// (get) Token: 0x0600B0BB RID: 45243 RVA: 0x00509271 File Offset: 0x00507471
		private string NpcAvatarTexturePath
		{
			get
			{
				return "NpcFace/BigFace";
			}
		}

		// Token: 0x1700140D RID: 5133
		// (get) Token: 0x0600B0BC RID: 45244 RVA: 0x00509278 File Offset: 0x00507478
		// (set) Token: 0x0600B0BD RID: 45245 RVA: 0x00509280 File Offset: 0x00507480
		public bool InitFlag { get; set; }

		// Token: 0x1700140E RID: 5134
		// (get) Token: 0x0600B0BE RID: 45246 RVA: 0x00509289 File Offset: 0x00507489
		// (set) Token: 0x0600B0BF RID: 45247 RVA: 0x00509291 File Offset: 0x00507491
		public ItemDisplayData SelectedMedicine { get; set; }

		// Token: 0x0600B0C0 RID: 45248 RVA: 0x0050929A File Offset: 0x0050749A
		private void Awake()
		{
			this.TryInit();
		}

		// Token: 0x0600B0C1 RID: 45249 RVA: 0x005092A4 File Offset: 0x005074A4
		private void OnEnable()
		{
			bool waitRefreshFavorFlag = this._waitRefreshFavorFlag;
			if (waitRefreshFavorFlag)
			{
				this.favorRoot.SetActive(false);
				this.relationLayout.gameObject.SetActive(false);
				base.StartCoroutine(this.UpdateFavorAndRelationShipShow());
			}
			bool flag = this._onEnableAction != null;
			if (flag)
			{
				this._onEnableAction();
				this._onEnableAction = null;
			}
		}

		// Token: 0x0600B0C2 RID: 45250 RVA: 0x0050930C File Offset: 0x0050750C
		private void OnDisable()
		{
			this.ResetFavorEffectState();
			this.ResetHappinessEffectState();
			bool flag = this._basicInfoMonitor != null;
			if (flag)
			{
				this._basicInfoMonitor.RemoveFavorabilityListener(new Action(this.FillFavorProgress));
			}
			this.UnbindHappinessMonitor();
			this.favorRoot.SetActive(false);
			this.relationLayout.gameObject.SetActive(false);
			this._consummateLevelHandler.CharacterId = -1;
			this._relationShipHandler.CharacterId = -1;
			this._favorabilityHandler.CharacterId = -1;
			this._behaviorHandler.CharacterId = -1;
			this._happinessHandler.CharacterId = -1;
			this._identityHandler.CharacterId = -1;
		}

		// Token: 0x0600B0C3 RID: 45251 RVA: 0x005093C4 File Offset: 0x005075C4
		public void Refresh()
		{
			bool skipRefreshForOnce = this._skipRefreshForOnce;
			if (skipRefreshForOnce)
			{
				this._skipRefreshForOnce = false;
			}
			else
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
					bool flag2 = characterDisplayData == null;
					if (flag2)
					{
						this.guardBack.SetActive(false);
					}
					bool needAnimation = !base.gameObject.activeSelf;
					EventActorData actorData = this.GetEventActorData();
					CaravanDisplayData caravanDisplayData = this.GetCaravanDisplayData();
					short templateIdOfCharacter = this.GetTemplateIdOfCharacter();
					ItemDisplayData jiaoMouseTipDisplayData = this.GetJiaoMouseTipDisplayData();
					sbyte merchantTemplateId = this.GetMerchantTemplateId(characterDisplayData, caravanDisplayData);
					ValueTuple<sbyte, sbyte> tuple = this.GetXiangShuAvatarId();
					this.UnbindHappinessMonitor();
					this.behaviorHolder.gameObject.SetActive(false);
					bool flag3 = actorData != null;
					if (flag3)
					{
						this.RefreshAsActor(actorData);
					}
					else
					{
						bool flag4 = 9 != tuple.Item1;
						if (flag4)
						{
							this.RefreshAsXiangShuAvatar(tuple.Item1, tuple.Item2);
						}
						else
						{
							bool flag5 = merchantTemplateId >= 0;
							if (flag5)
							{
								this.RefreshAsMerchant(merchantTemplateId);
							}
							else
							{
								bool flag6 = characterDisplayData != null;
								if (flag6)
								{
									this.RefreshAsNormalCharacter(characterDisplayData);
								}
								else
								{
									bool flag7 = -1 != templateIdOfCharacter;
									if (flag7)
									{
										this.RefreshAsTemplateCharacter(templateIdOfCharacter);
									}
								}
							}
						}
					}
					bool flag8 = jiaoMouseTipDisplayData != null;
					if (flag8)
					{
						this.SetJiaoMouseTip(jiaoMouseTipDisplayData);
					}
					else
					{
						this.showCharacterMenuTips.SetActive(false);
					}
					base.gameObject.SetActive(true);
					this.characterInfoHolder.SetActive(actorData == null);
					this.RefreshHappinessInfo();
					this.RefreshFavorView();
					this.RefreshAlertness();
					this.RefreshCommonBtn();
					this.UpdateInjuryInfoTipsVisible();
					this.RefreshAiActionToolTip();
					this.RefreshProfessionSkill();
					this.RefreshShowCharacterPosition();
					bool flag9 = needAnimation;
					if (flag9)
					{
						this.PlayAvatarAppearAnimation();
					}
				}
			}
		}

		// Token: 0x0600B0C4 RID: 45252 RVA: 0x0050959C File Offset: 0x0050779C
		private void RefreshShowCharacterPosition()
		{
			bool flag = this.showCharacterPositionRight == null || this.showCharacterPositionLeft == null;
			if (!flag)
			{
				this.showCharacterMenuBtn.transform.position = (this.professionInfo.gameObject.activeSelf ? this.showCharacterPositionRight.position : this.showCharacterPositionLeft.position);
			}
		}

		// Token: 0x0600B0C5 RID: 45253 RVA: 0x00509608 File Offset: 0x00507808
		private void RefreshProfessionSkill()
		{
			bool flag = this.professionInfo == null;
			if (!flag)
			{
				CharacterDisplayData data = this.GetCharacterDisplayData();
				this.professionInfo.gameObject.SetActive(false);
				bool flag2 = data == null || data.CreatingType != 1;
				if (!flag2)
				{
					ProfessionItem professionConfig = (data.CurrentProfession == null) ? null : Profession.Instance[data.CurrentProfession.TemplateId];
					string professionName = (professionConfig != null) ? professionConfig.Name : LanguageKey.LK_Tooltip_CharacterCurrentProfession_Empty.Tr();
					this.professionInfo.gameObject.SetActive(true);
					bool flag3 = this.txtProfessionName != null;
					if (flag3)
					{
						this.txtProfessionName.text = professionName;
					}
					this.professionInfoTooltip.Type = TipType.CharacterCurrentProfession;
					TooltipInvoker tooltipInvoker = this.professionInfoTooltip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
					}
					CharacterDomainMethod.AsyncCall.GetCharacterProfessionList(null, this._curCharacterId, delegate(int offset, RawDataPool pool)
					{
						ProfessionAllDisplayData data2 = null;
						Serializer.Deserialize(pool, offset, ref data2);
						this.professionInfoTooltip.RuntimeParam.SetObject("ProfessionData", data2);
					});
				}
			}
		}

		// Token: 0x0600B0C6 RID: 45254 RVA: 0x00509718 File Offset: 0x00507918
		private void RefreshAsNormalCharacter(CharacterDisplayData characterDisplayData)
		{
			this.behaviorHolder.gameObject.SetActive(true);
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
					this.avatar.RefreshAsSpine(spineName, spineSkinName);
				}
				else
				{
					string resPath = CharacterAvatar.GetNpcFaceResPath(CharacterAvatar.GetAvatarSizeFolder(this.avatar.Size), avatarAssetName);
					ResLoader.LoadModOrGameResource<Texture2D>(resPath, delegate(Texture2D tex)
					{
						this.avatar.Refresh(tex);
					}, delegate(string path)
					{
						this.avatar.Refresh(characterDisplayData, true);
					});
				}
			}
			else
			{
				this.avatar.Refresh(characterDisplayData, true);
			}
			string roleNameKey = this.GetRoleNameKey();
			bool useAlternativeName = this.GetUseAlternativeName();
			if (useAlternativeName)
			{
				CharacterItem config2 = Character.Instance.GetItem(characterDisplayData.TemplateId);
				bool flag6 = config2 != null && !string.IsNullOrEmpty(config2.AnonymousTitle);
				if (flag6)
				{
					this.nameLabel.text = config2.AnonymousTitle;
				}
			}
			else
			{
				bool flag7 = !string.IsNullOrEmpty(roleNameKey);
				if (flag7)
				{
					this.nameLabel.text = LocalStringManager.Get(roleNameKey);
				}
				else
				{
					this.nameLabel.text = NameCenter.GetCharMonasticTitleOrNameByDisplayData(characterDisplayData, characterDisplayData.CharacterId == this.TaiwuCharId, false);
				}
			}
			this.avatar.SetShadowStrength(this.GetRightCharacterShadow() ? 0.85f : 0f);
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
			this.BindHappinessMonitor(characterDisplayData.CharacterId);
			this._identityHandler.CharacterId = characterDisplayData.CharacterId;
			this._basicInfoMonitor = this._favorabilityHandler.GetMonitor<BasicInfoMonitor>();
			bool flag8 = this._basicInfoMonitor != null;
			if (flag8)
			{
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
			}
			CharacterRelationMonitor relationMonitor = this._relationShipHandler.GetMonitor<CharacterRelationMonitor>();
			bool needShowRelation = characterDisplayData.CharacterId != this.TaiwuCharId;
			bool flag9 = relationMonitor != null && relationMonitor.Init && relationMonitor.RelationBitFlag == ushort.MaxValue && relationMonitor.CharacterId == characterDisplayData.CharacterId;
			if (flag9)
			{
				needShowRelation = false;
			}
			bool flag10 = needShowRelation;
			if (flag10)
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
			bool flag11 = characterDisplayData.CharacterId != SingletonObject.getInstance<BasicGameData>().TaiwuCharId && characterDisplayData.CreatingType == 1;
			if (flag11)
			{
				CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForGuard(null, characterDisplayData.CharacterId, delegate(int offset, RawDataPool dataPool)
				{
					CharacterDisplayDataForGuard data = new CharacterDisplayDataForGuard();
					Serializer.Deserialize(dataPool, offset, ref data);
					this.RefreshGuard(data);
				});
			}
			else
			{
				this.guardBack.SetActive(false);
			}
		}

		// Token: 0x0600B0C7 RID: 45255 RVA: 0x00509BCC File Offset: 0x00507DCC
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
					this.avatar.RefreshAsSpine(actorConfig.SpineName, actorConfig.SpineSkinName);
				}
				else
				{
					ResLoader.LoadModOrGameResource<Texture2D>(this.NpcAvatarTexturePath + "/" + actorConfig.Texture, new Action<Texture2D>(this.avatar.Refresh), null);
				}
			}
			else
			{
				actorData.AvatarData.ClothDisplayId = actorData.ClothDisplayId;
				bool flag3 = this.isLeftCharacter;
				if (flag3)
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
				this.avatar.Refresh(actorData.AvatarData, (short)actorData.Age);
			}
			bool flag4 = !string.IsNullOrEmpty(roleNameKey);
			if (flag4)
			{
				this.nameLabel.text = LocalStringManager.Get(roleNameKey);
			}
			else
			{
				this.nameLabel.text = actorData.DisplayName;
			}
			this._showFavorabilityFlag = false;
		}

		// Token: 0x0600B0C8 RID: 45256 RVA: 0x00509DB4 File Offset: 0x00507FB4
		private void RefreshAsTemplateCharacter(short templateId)
		{
			this._relationShipHandler.CharacterId = -1;
			this._consummateLevelHandler.CharacterId = -1;
			CharacterItem config = Character.Instance.GetItem(templateId);
			bool flag = config == null;
			if (!flag)
			{
				this.SetTemplateCharacterConsummateLevelInfo(config);
				this.SetCharacterMenuBtn(false);
				this.nameLabel.text = config.Surname + config.GivenName;
				bool targetRoleUseAlternativeName = this.Data.ExtraData.TargetRoleUseAlternativeName;
				if (targetRoleUseAlternativeName)
				{
					bool flag2 = !string.IsNullOrEmpty(config.AnonymousTitle);
					if (flag2)
					{
						this.nameLabel.text = config.AnonymousTitle;
					}
				}
				bool flag3 = this.avatar.Size == AvatarSize.Big && !string.IsNullOrEmpty(config.FixedAvatarSpineName);
				if (flag3)
				{
					this.avatar.RefreshAsSpine(config.FixedAvatarSpineName, config.FixedAvatarSpineSkin);
				}
				else
				{
					bool flag4 = !string.IsNullOrEmpty(config.FixedAvatarName);
					if (flag4)
					{
						ResLoader.LoadModOrGameResource<Texture2D>(this.NpcAvatarTexturePath + "/" + config.FixedAvatarName, new Action<Texture2D>(this.avatar.Refresh), null);
					}
				}
			}
		}

		// Token: 0x0600B0C9 RID: 45257 RVA: 0x00509EE0 File Offset: 0x005080E0
		private void SetTemplateCharacterConsummateLevelInfo(CharacterItem config)
		{
			ValueTuple<string, string> consummateLevelShowData = CommonUtils.GetConsummateLevelShowData(config.ConsummateLevel);
			string iconName = consummateLevelShowData.Item1;
			string levelName = consummateLevelShowData.Item2;
			this.consummateLevelIcon.SetSprite(iconName, false, null);
			TooltipInvoker mouseTip = this.consummateLevelIcon.GetComponent<TooltipInvoker>();
			mouseTip.triggerByChildRaycast = true;
			mouseTip.Type = TipType.SingleDesc;
			mouseTip.RuntimeParam = new ArgumentBox();
			mouseTip.RuntimeParam.Set("arg0", LocalStringManager.GetFormat(LanguageKey.UI_EventWindow_Consummate_Level_Tips, levelName));
		}

		// Token: 0x0600B0CA RID: 45258 RVA: 0x00509F58 File Offset: 0x00508158
		private void RefreshAsMerchant(sbyte merchantTemplateId)
		{
			this._relationShipHandler.CharacterId = -1;
			this._consummateLevelHandler.CharacterId = -1;
			MerchantTypeItem merchantTypeConfig = Config.MerchantType.Instance[Merchant.Instance[merchantTemplateId].MerchantType];
			bool flag = !string.IsNullOrEmpty(merchantTypeConfig.CaravanSpineName);
			if (flag)
			{
				this.avatar.RefreshAsSpine(merchantTypeConfig.CaravanSpineName, null);
			}
			else
			{
				ResLoader.LoadModOrGameResource<Texture2D>(this.NpcAvatarTexturePath + "/" + merchantTypeConfig.CaravanAvatar, new Action<Texture2D>(this.avatar.Refresh), null);
			}
			this.nameLabel.text = merchantTypeConfig.Name;
			this.SetCharacterMenuBtn(false);
		}

		// Token: 0x0600B0CB RID: 45259 RVA: 0x0050A010 File Offset: 0x00508210
		private void RefreshAsXiangShuAvatar(sbyte xiangshuAvatarId, sbyte displayStatus)
		{
			this._relationShipHandler.CharacterId = -1;
			this._consummateLevelHandler.CharacterId = -1;
			this.SetCharacterMenuBtn(false);
			string spineName = EventWindowCharacter.GetXiangshuSpineName(xiangshuAvatarId, displayStatus);
			bool flag = !string.IsNullOrEmpty(spineName);
			if (flag)
			{
				this.avatar.RefreshAsSpine(spineName, null);
			}
			else
			{
				string xiangshuAvtarName = EventModel.XiangShuAvatarDisplayTextures[(int)xiangshuAvatarId][(int)displayStatus];
				ResLoader.LoadModOrGameResource<Texture2D>(this.NpcAvatarTexturePath + "/" + xiangshuAvtarName, new Action<Texture2D>(this.avatar.Refresh), null);
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
				this.nameLabel.text = config.Surname + config.GivenName;
			}
			else
			{
				this.nameLabel.text = string.Empty;
			}
			CharacterDisplayData characterDisplayData = this.GetCharacterDisplayData();
			bool flag4 = characterDisplayData != null;
			if (flag4)
			{
				this._showFavorabilityFlag = (displayStatus == 1 || displayStatus == 2);
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
				this.BindHappinessMonitor(characterDisplayData.CharacterId);
				this._basicInfoMonitor = this._favorabilityHandler.GetMonitor<BasicInfoMonitor>();
				bool flag5 = this._basicInfoMonitor != null;
				if (flag5)
				{
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
				this.UnbindHappinessMonitor();
			}
		}

		// Token: 0x0600B0CC RID: 45260 RVA: 0x0050A1F8 File Offset: 0x005083F8
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

		// Token: 0x0600B0CD RID: 45261 RVA: 0x0050A2BC File Offset: 0x005084BC
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

		// Token: 0x0600B0CE RID: 45262 RVA: 0x0050A300 File Offset: 0x00508500
		private void RefreshHappinessInfo()
		{
			CharacterDisplayData characterDisplayData = this.GetCharacterDisplayData();
			this.happinessRoot.gameObject.SetActive(characterDisplayData != null && characterDisplayData.CharacterId == this.TaiwuCharId);
			bool flag = characterDisplayData == null;
			if (!flag)
			{
				sbyte happinessType = HappinessType.GetHappinessType(characterDisplayData.Happiness);
				ValueTuple<sbyte, sbyte> valueTuple = HappinessType.Ranges[(int)happinessType];
				sbyte min = valueTuple.Item1;
				sbyte max = valueTuple.Item2;
				this.happinessProgressFill.fillAmount = ((float)characterDisplayData.Happiness - (float)min) / (float)(max - min);
				this.happinessProgressFill.SetColor(Colors.Instance.HappinessTypeColors[(int)happinessType]);
			}
		}

		// Token: 0x0600B0CF RID: 45263 RVA: 0x0050A3A4 File Offset: 0x005085A4
		private void OnHappinessChanged()
		{
			CharacterDisplayData characterDisplayData = this.GetCharacterDisplayData();
			bool showHappiness = characterDisplayData != null;
			bool flag = characterDisplayData == null;
			if (!flag)
			{
				DetailInfoMonitor happinessMonitor = this._happinessMonitor;
				sbyte happinessValue = (happinessMonitor != null) ? happinessMonitor.Happiness : characterDisplayData.Happiness;
				if (this._notedHappinessData == null)
				{
					this._notedHappinessData = new int[]
					{
						-1,
						int.MinValue
					};
				}
				bool flag2 = showHappiness && this._notedHappinessData[0] == characterDisplayData.CharacterId && this._notedHappinessData[1] != int.MinValue && this._notedHappinessData[1] != (int)happinessValue;
				if (flag2)
				{
					this.ResetHappinessEffectState();
					bool isDown = this._notedHappinessData[1] > (int)happinessValue;
					GameObject targetObj = isDown ? this.happinessLevelDownEffectObject : this.happinessLevelUpEffectObject;
					this.PlayFavorChangeSound(isDown ? "ui_favorite_down" : "ui_favorite_up");
					targetObj.SetActive(true);
				}
				this._notedHappinessData[0] = characterDisplayData.CharacterId;
				this._notedHappinessData[1] = (int)happinessValue;
			}
		}

		// Token: 0x0600B0D0 RID: 45264 RVA: 0x0050A4A0 File Offset: 0x005086A0
		private void RefreshCommonBtn()
		{
			CharacterDisplayData characterDisplayData = this.GetCharacterDisplayData();
			bool flag = characterDisplayData == null || characterDisplayData.CreatingType != 1;
			if (flag)
			{
				this.quickBtn.SetActive(false);
				this.RefreshCharacterMenuBtnState();
			}
			else
			{
				this.quickBtn.SetActive(true);
				this.itemBtn.gameObject.SetActive(characterDisplayData.CharacterId == this.TaiwuCharId);
				this.relationBtn.gameObject.SetActive(characterDisplayData.CharacterId == this.TaiwuCharId);
				this.lifeRecordBtn.gameObject.SetActive(characterDisplayData.CharacterId != this.TaiwuCharId);
				this.RefreshCharacterMenuBtnState();
				this.injuryBtn.interactable = !UIManager.Instance.IsElementActive(UIElement.CharacterMenu);
				int index = 0;
				for (int i = 0; i < this.quickBtn.transform.childCount - 1; i++)
				{
					this.quickBtn.transform.GetChild(index).GetChild(1).gameObject.SetActive(true);
					bool activeSelf = this.quickBtn.transform.GetChild(i).gameObject.activeSelf;
					if (activeSelf)
					{
						index = i;
					}
				}
				this.quickBtn.transform.GetChild(index).GetChild(1).gameObject.SetActive(false);
			}
		}

		// Token: 0x0600B0D1 RID: 45265 RVA: 0x0050A607 File Offset: 0x00508807
		private IEnumerator UpdateFavorAndRelationShipShow()
		{
			CharacterDisplayData characterDisplayData = this.GetCharacterDisplayData();
			this._waitRefreshFavorFlag = false;
			ConsummateLevelMonitor consummateLevelMonitor = this._consummateLevelHandler.GetMonitor<ConsummateLevelMonitor>();
			bool flag = consummateLevelMonitor == null || this._basicInfoMonitor == null;
			if (flag)
			{
				this.infoLine2Rect.gameObject.SetActive(false);
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
					this.infoLine2Rect.gameObject.SetActive(false);
					yield break;
				}
				charItemConfig = null;
			}
			this.favorRoot.SetActive(this._showFavorabilityFlag && characterDisplayData.CharacterId != this.TaiwuCharId);
			this.relationLayout.gameObject.SetActive(this._relationShipHandler.HasRelationShowed);
			bool needHide = true;
			int num;
			for (int i = 0; i < this.infoLine2Rect.childCount; i = num + 1)
			{
				Transform child = this.infoLine2Rect.GetChild(i);
				bool activeSelf = child.gameObject.activeSelf;
				if (activeSelf)
				{
					needHide = false;
				}
				child = null;
				num = i;
			}
			this.infoLine2Rect.gameObject.SetActive(!needHide);
			this.RefreshOtherRoot();
			yield break;
		}

		// Token: 0x0600B0D2 RID: 45266 RVA: 0x0050A616 File Offset: 0x00508816
		private void SetJiaoMouseTip(ItemDisplayData jiaoDisplayData)
		{
			ExtraDomainMethod.AsyncCall.GetJiaoLoongDisplayDataByItemKey(null, jiaoDisplayData.Key, delegate(int offset, RawDataPool dataPool)
			{
				JiaoLoongDisplayData displayData = new JiaoLoongDisplayData();
				Serializer.Deserialize(dataPool, offset, ref displayData);
				TooltipInvoker mouseTip = this.showCharacterMenuTips.GetComponent<TooltipInvoker>();
				TooltipInvoker tooltipInvoker = mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				mouseTip.RuntimeParam.Clear();
				mouseTip.RuntimeParam.SetObject("JiaoLoongData", displayData);
				mouseTip.RuntimeParam.Set("DisableCompare", true);
				this.showCharacterMenuTips.SetActive(true);
			});
		}

		// Token: 0x0600B0D3 RID: 45267 RVA: 0x0050A634 File Offset: 0x00508834
		private void RefreshGuard(CharacterDisplayDataForGuard data)
		{
			bool flag = data.Guards.Count <= 0;
			if (!flag)
			{
				this.guardBack.SetActive(true);
				this.guardIcon.SetSprite(string.Format("ui9_icon_guard_big_{0}", (!data.HasGuard) ? 1 : (data.IsMain ? 2 : 0)), false, null);
				TooltipInvoker tips = this.guardBack.GetComponent<TooltipInvoker>();
				StringBuilder sb = new StringBuilder();
				for (int index = 0; index < data.Guards.Count; index++)
				{
					NameRelatedData guardName = data.Guards[index];
					sb.Append(NameCenter.GetMonasticTitleOrDisplayName(ref guardName, false, false).SetGradeColor((int)guardName.OrgGrade));
					bool flag2 = index != data.Guards.Count - 1;
					if (flag2)
					{
						sb.Append(LanguageKey.LK_Split_Symbol.Tr());
					}
				}
				EventWindowCharacter.GuardTipRuntime runtime = new EventWindowCharacter.GuardTipRuntime(CommonTip.DefValue.Guard);
				runtime.Set("GuardNameStr", sb.ToString());
				runtime.Set("IsMain", data.IsMain ? LanguageKey.LK_Yes.Tr().SetColor("brightblue") : LanguageKey.LK_No.Tr().SetColor("brightred"));
				runtime.Set("GuardCanAffectInteract", data.GuardCanAffectInteract ? LanguageKey.LK_Yes.Tr().SetColor("brightblue") : LanguageKey.LK_No.Tr().SetColor("brightred"));
				runtime.SetDisableStringDisplay(!data.HasGuard);
				TooltipInvoker tooltipInvoker = tips;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				tips.RuntimeParam.SetObject("Runtime", runtime);
			}
		}

		// Token: 0x0600B0D4 RID: 45268 RVA: 0x0050A800 File Offset: 0x00508A00
		private void FillFavorProgress()
		{
			bool flag = this.GetCharacterDisplayData() == null || this._basicInfoMonitor == null || null == this;
			if (!flag)
			{
				CharacterFavorability favorabilityHandler = this._favorabilityHandler;
				if (favorabilityHandler != null)
				{
					favorabilityHandler.FillElement();
				}
				if (this._notedFavorData == null)
				{
					this._notedFavorData = new int[]
					{
						-1,
						26
					};
				}
				CharacterItem charConfig = Character.Instance[this._basicInfoMonitor.NameRelatedData.CharTemplateId];
				bool flag2 = this._notedFavorData[1] <= 30000 && (this._notedFavorData[1] >= -30000 || charConfig.CreatingType != 1) && this._notedFavorData[1] != (int)this._basicInfoMonitor.FavorabilityToTaiwu && this._notedFavorData[0] == this._basicInfoMonitor.CharacterId;
				if (flag2)
				{
					this.ResetFavorEffectState();
					bool isDown = this._notedFavorData[1] > (int)this._basicInfoMonitor.FavorabilityToTaiwu;
					GameObject targetObj = isDown ? this.favorLevelDownEffectObject : this.favorLevelUpEffectObject;
					string soundName = isDown ? "ui_favorite_down" : "ui_favorite_up";
					this.PlayFavorChangeSound(soundName);
					targetObj.SetActive(true);
					ValueTuple<short, short> favorabilityRange = FavorabilityType.GetFavorabilityRange(this._basicInfoMonitor.FavorabilityToTaiwu);
					short min = favorabilityRange.Item1;
					short max = favorabilityRange.Item2;
					float newProgress = (float)(this._basicInfoMonitor.FavorabilityToTaiwu - min) / (float)(max - min);
					this.favorProgressFill.DOFillAmount(newProgress, 0.3f);
				}
				else
				{
					ValueTuple<short, short> favorabilityRange2 = FavorabilityType.GetFavorabilityRange(this._basicInfoMonitor.FavorabilityToTaiwu);
					short min2 = favorabilityRange2.Item1;
					short max2 = favorabilityRange2.Item2;
					float progress = (float)(this._basicInfoMonitor.FavorabilityToTaiwu - min2) / (float)(max2 - min2);
					this.favorProgressFill.fillAmount = progress;
				}
				sbyte favorabilityType = FavorabilityType.GetFavorabilityType(this._basicInfoMonitor.FavorabilityToTaiwu);
				this.favorProgressFill.SetColor(Colors.Instance.FavorabilityTypeColors[(int)(favorabilityType + 7)]);
				this._notedFavorData[0] = this._basicInfoMonitor.CharacterId;
				this._notedFavorData[1] = (int)this._basicInfoMonitor.FavorabilityToTaiwu;
			}
		}

		// Token: 0x0600B0D5 RID: 45269 RVA: 0x0050AA18 File Offset: 0x00508C18
		public void UpdateInjuryInfoTipsVisible()
		{
			TooltipInvoker injuryTips = this.injuryBtn.GetComponent<TooltipInvoker>();
			injuryTips.Type = TipType.Injury;
			TooltipInvoker tooltipInvoker = injuryTips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			injuryTips.RuntimeParam.Set("characterId", this._curCharacterId);
			injuryTips.RuntimeParam.Set<ItemDisplayData>("selectedMedicine", this.SelectedMedicine);
		}

		// Token: 0x0600B0D6 RID: 45270 RVA: 0x0050AA80 File Offset: 0x00508C80
		private void RefreshAiActionToolTip()
		{
			bool flag = this.aiActionToolTip == null;
			if (!flag)
			{
				CharacterDisplayData characterDisplayData = this.GetCharacterDisplayData();
				bool isTaiwu = characterDisplayData != null && characterDisplayData.CharacterId == this.TaiwuCharId;
				this.aiActionToolTip.gameObject.SetActive(characterDisplayData != null && !isTaiwu && characterDisplayData.CreatingType == 1);
				bool flag2 = !this.aiActionToolTip.gameObject.activeSelf;
				if (!flag2)
				{
					this.aiActionToolTip.Type = TipType.AiAction;
					TooltipInvoker tooltipInvoker = this.aiActionToolTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
					}
					this.aiActionToolTip.RuntimeParam.Set("charId", (this._curCharacterId > -1) ? this._curCharacterId : characterDisplayData.CharacterId);
				}
			}
		}

		// Token: 0x0600B0D7 RID: 45271 RVA: 0x0050AB5C File Offset: 0x00508D5C
		private void PlayFavorChangeSound(string soundName)
		{
			bool flag = this._lastFavorUiSoundFrame + 20 > Time.frameCount;
			if (!flag)
			{
				this._lastFavorUiSoundFrame = Time.frameCount;
				AudioManager.Instance.PlaySoundNoRepeat(soundName, 100, false, false);
			}
		}

		// Token: 0x0600B0D8 RID: 45272 RVA: 0x0050AB9B File Offset: 0x00508D9B
		private void ResetFavorEffectState()
		{
			this.favorLevelUpEffectObject.SetActive(false);
			this.favorLevelDownEffectObject.SetActive(false);
		}

		// Token: 0x0600B0D9 RID: 45273 RVA: 0x0050ABB8 File Offset: 0x00508DB8
		private void ResetHappinessEffectState()
		{
			this.happinessLevelUpEffectObject.SetActive(false);
			this.happinessLevelDownEffectObject.SetActive(false);
		}

		// Token: 0x0600B0DA RID: 45274 RVA: 0x0050ABD8 File Offset: 0x00508DD8
		private void BindHappinessMonitor(int characterId)
		{
			this.UnbindHappinessMonitor();
			this._happinessHandler.CharacterId = characterId;
			this._happinessMonitor = this._happinessHandler.GetMonitor<DetailInfoMonitor>();
			bool flag = this._happinessMonitor != null;
			if (flag)
			{
				this._happinessMonitor.AddOnHappinessListener(new Action(this.OnHappinessChanged));
			}
		}

		// Token: 0x0600B0DB RID: 45275 RVA: 0x0050AC34 File Offset: 0x00508E34
		private void UnbindHappinessMonitor()
		{
			bool flag = this._happinessMonitor != null;
			if (flag)
			{
				this._happinessMonitor.RemoveOnHappinessListener(new Action(this.OnHappinessChanged));
				this._happinessMonitor = null;
			}
		}

		// Token: 0x0600B0DC RID: 45276 RVA: 0x0050AC70 File Offset: 0x00508E70
		private void RefreshAlertness()
		{
			EventWindowCharacter.<>c__DisplayClass108_0 CS$<>8__locals1;
			CS$<>8__locals1.characterDisplayData = this.GetCharacterDisplayData();
			bool show = CS$<>8__locals1.characterDisplayData != null && CS$<>8__locals1.characterDisplayData.CharacterId != this.TaiwuCharId && CS$<>8__locals1.characterDisplayData.CreatingType == 1;
			this.alertnessProperty.gameObject.SetActive(show);
			this.alertnessBtn.gameObject.SetActive(show);
			this.RefreshOtherRoot();
			bool flag = !show;
			if (!flag)
			{
				sbyte level = CharacterAlertnessData.GetLevel(CS$<>8__locals1.characterDisplayData.Alertness);
				string levelName = CommonUtils.GetAlertnessName((int)level);
				string levelIcon = CommonUtils.GetAlertnessIcon((int)level);
				string title = LanguageKey.LK_Alertness.Tr();
				this.alertnessProperty.Set(levelIcon, title, levelName, null, false);
				EventWindowCharacter.<RefreshAlertness>g__SetTip|108_0(this.alertnessProperty.Tip, ref CS$<>8__locals1);
				int cur = CS$<>8__locals1.characterDisplayData.Alertness;
				int min = 0;
				int minLevel = 0;
				for (int i = GlobalConfig.Instance.AlertnessLevelRange.Length - 2; i >= 0; i--)
				{
					int value = GlobalConfig.Instance.AlertnessLevelRange[i];
					bool flag2 = cur >= value;
					if (flag2)
					{
						min = value;
						minLevel = i;
						break;
					}
				}
				int maxLevel = minLevel + 1;
				int max = GlobalConfig.Instance.AlertnessLevelRange[maxLevel];
				this.alertnessProgress.fillAmount = (float)(cur - min) / (float)(max - min);
				level = CharacterAlertnessData.ValidateLevel((int)level);
				this.alertnessProgress.SetColor(Colors.Instance.AlertnessColors[(int)level]);
				this.alertnessBtn.ClearAndAddListener(new Action(this.OnClickAlertnessButton));
				TooltipInvoker buttonTip = this.alertnessBtn.gameObject.GetOrAddComponent<TooltipInvoker>();
				EventWindowCharacter.<RefreshAlertness>g__SetTip|108_0(buttonTip, ref CS$<>8__locals1);
			}
		}

		// Token: 0x0600B0DD RID: 45277 RVA: 0x0050AE38 File Offset: 0x00509038
		private void OnClickAlertnessButton()
		{
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("charId", this._curCharacterId);
			UIElement.Alertness.SetOnInitArgs(argumentBox);
			UIManager.Instance.ShowUI(UIElement.Alertness, true);
		}

		// Token: 0x0600B0DE RID: 45278 RVA: 0x0050AE79 File Offset: 0x00509079
		private void PlayAvatarAppearAnimation()
		{
		}

		// Token: 0x0600B0DF RID: 45279 RVA: 0x0050AE7C File Offset: 0x0050907C
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
						onViewCharacter(this._curCharacterId, pageIndex, this.isLeftCharacter);
					}
				}
			}
		}

		// Token: 0x0600B0E0 RID: 45280 RVA: 0x0050AED4 File Offset: 0x005090D4
		private void TryInit()
		{
			bool initFlag = this.InitFlag;
			if (!initFlag)
			{
				this._favorabilityHandler = new CharacterFavorability(this.favorabilityIcon, this.favorabilityLabel, null, null, null);
				this._relationShipHandler = new CharacterRelationShip(this.relationLayout.GetComponent<Refers>());
				this._consummateLevelHandler = new CharacterConsummateLevel(this.consummateLevelIcon, this.consummateLevelLabel, this.consummateLevelName);
				this._behaviorHandler = new CharacterBehavior(this.behaviorHolder, false)
				{
					UseNewIcon = true
				};
				this._happinessHandler = new CharacterHappiness(this.happinessRoot, false)
				{
					UseNewIcon = true
				};
				this._identityHandler = new CharacterOrganization(null, this.identityHolder);
				this.avatar.KeepSkeletonAnimationOnRefresh = true;
				bool flag = this.avatarBtn;
				if (flag)
				{
					this.avatarBtn.ClearAndAddListener(delegate
					{
						this.OnShowCharacterMenuClick(-1);
					});
				}
				this.showCharacterMenuBtn.ClearAndAddListener(delegate
				{
					this.OnShowCharacterMenuClick(-1);
				});
				this.injuryBtn.ClearAndAddListener(delegate
				{
					this.OnShowCharacterMenuClick(0);
				});
				this.itemBtn.ClearAndAddListener(delegate
				{
					this.OnShowCharacterMenuClick(3);
				});
				this.relationBtn.ClearAndAddListener(delegate
				{
					this.OnShowCharacterMenuClick(7);
				});
				this.lifeRecordBtn.ClearAndAddListener(delegate
				{
					this.OnShowCharacterMenuClick(8);
				});
				PointerTrigger lifeRecordPointerTrigger = this.lifeRecordBtn.GetComponent<PointerTrigger>();
				lifeRecordPointerTrigger.EnterEvent.ResetListener(new Action(this.PointerTriggerEnterShowLifeRecord));
				lifeRecordPointerTrigger.ExitEvent.ResetListener(new Action(this.PointerTriggerExitShowLifeRecord));
				this.characterLifeRecords.GetComponent<PointerTrigger>().ExitEvent.ResetListener(new Action(this.PointerTriggerExitShowLifeRecord));
				this.showCharacterMenuTips.SetActive(false);
				this.favorRoot.SetActive(false);
				this.relationLayout.gameObject.SetActive(false);
				this._jieqingPutOnMaskAdult = this.jieqingMaskHolder.CGet<ParticleSystem>("eff_jieqingtangram_mianju_chuan1");
				this._jieqingPutOnMaskChild = this.jieqingMaskHolder.CGet<ParticleSystem>("eff_jieqingtangram_mianju_chuan2");
				this._jieqingTakeOffMaskAdult = this.jieqingMaskHolder.CGet<ParticleSystem>("eff_jieqingtangram_mianju_tuo1");
				this._jieqingTakeOffMaskChild = this.jieqingMaskHolder.CGet<ParticleSystem>("eff_jieqingtangram_mianju_tuo2");
				this.InitFlag = true;
			}
		}

		// Token: 0x0600B0E1 RID: 45281 RVA: 0x0050B118 File Offset: 0x00509318
		private void RefreshOtherRoot()
		{
			bool isShowAll = this.favorRoot.activeSelf && this.alertnessProperty.gameObject.activeSelf;
			this.otherRootLine.SetActive(isShowAll);
			this.favorProgressFill.sprite = (isShowAll ? this.fillSprites[0] : this.fillSprites[1]);
			this.alertnessProgress.sprite = (isShowAll ? this.fillSprites[0] : this.fillSprites[1]);
		}

		// Token: 0x0600B0E2 RID: 45282 RVA: 0x0050B196 File Offset: 0x00509396
		private bool GetHasCharacter()
		{
			return this.isLeftCharacter ? this.HasLeftCharacter() : this.HasRightCharacter();
		}

		// Token: 0x0600B0E3 RID: 45283 RVA: 0x0050B1B0 File Offset: 0x005093B0
		private bool HasLeftCharacter()
		{
			bool flag = this.Data.MainCharacter != null;
			return flag || this.Data.ExtraData.LeftActorData != null;
		}

		// Token: 0x0600B0E4 RID: 45284 RVA: 0x0050B1EC File Offset: 0x005093EC
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

		// Token: 0x0600B0E5 RID: 45285 RVA: 0x0050B260 File Offset: 0x00509460
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
				result = (this.isLeftCharacter ? this.Data.MainCharacter : this.Data.TargetCharacter);
			}
			return result;
		}

		// Token: 0x0600B0E6 RID: 45286 RVA: 0x0050B2A4 File Offset: 0x005094A4
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
				result = (this.isLeftCharacter ? this.Data.ExtraData.LeftActorData : this.Data.ExtraData.ActorData);
			}
			return result;
		}

		// Token: 0x0600B0E7 RID: 45287 RVA: 0x0050B2F4 File Offset: 0x005094F4
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
				bool flag2 = !this.isLeftCharacter;
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

		// Token: 0x0600B0E8 RID: 45288 RVA: 0x0050B354 File Offset: 0x00509554
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
				result = (this.isLeftCharacter ? null : this.Data.ExtraData.CaravanData);
			}
			return result;
		}

		// Token: 0x0600B0E9 RID: 45289 RVA: 0x0050B394 File Offset: 0x00509594
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
				result = (this.isLeftCharacter ? null : this.Data.ExtraData.JiaoDisplayData);
			}
			return result;
		}

		// Token: 0x0600B0EA RID: 45290 RVA: 0x0050B3D4 File Offset: 0x005095D4
		private sbyte GetMerchantTemplateId(CharacterDisplayData characterDisplayData, CaravanDisplayData caravanDisplayData)
		{
			bool flag = this.Data == null || this.isLeftCharacter;
			sbyte result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				bool flag2 = characterDisplayData != null;
				if (flag2)
				{
					short templateId = characterDisplayData.TemplateId;
					if (!true)
					{
					}
					sbyte b;
					switch (templateId)
					{
					case 959:
						b = 0;
						break;
					case 960:
						b = 7;
						break;
					case 961:
						b = 14;
						break;
					case 962:
						b = 21;
						break;
					case 963:
						b = 28;
						break;
					case 964:
						b = 35;
						break;
					case 965:
						b = 42;
						break;
					default:
						b = -1;
						break;
					}
					if (!true)
					{
					}
					result = b;
				}
				else
				{
					bool flag3 = caravanDisplayData != null;
					if (flag3)
					{
						result = (sbyte)caravanDisplayData.MerchantTemplateId;
					}
					else
					{
						result = -1;
					}
				}
			}
			return result;
		}

		// Token: 0x0600B0EB RID: 45291 RVA: 0x0050B484 File Offset: 0x00509684
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
				result = (this.isLeftCharacter ? -1 : this.Data.ExtraData.HereticTemplateId);
			}
			return result;
		}

		// Token: 0x0600B0EC RID: 45292 RVA: 0x0050B4C4 File Offset: 0x005096C4
		private bool GetForbidViewCharacter()
		{
			bool flag = this.Data == null;
			return flag || (this.isLeftCharacter ? (this.Data.ExtraData.ForbidViewSelf || this.Data.MainCharacter == null || !Character.Instance[this.Data.MainCharacter.TemplateId].CanOpenCharacterMenu) : (this.Data.ExtraData.ForbidViewCharacter || this.Data.TargetCharacter == null || !Character.Instance[this.Data.TargetCharacter.TemplateId].CanOpenCharacterMenu));
		}

		// Token: 0x0600B0ED RID: 45293 RVA: 0x0050B57C File Offset: 0x0050977C
		private bool GetUseAlternativeName()
		{
			bool flag = this.Data == null;
			return !flag && (this.isLeftCharacter ? this.Data.ExtraData.MainRoleUseAlternativeName : this.Data.ExtraData.TargetRoleUseAlternativeName);
		}

		// Token: 0x0600B0EE RID: 45294 RVA: 0x0050B5CC File Offset: 0x005097CC
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
				result = (this.isLeftCharacter ? this.Model.LeftRoleNameKey : this.Model.RightRoleNameKey);
			}
			return result;
		}

		// Token: 0x0600B0EF RID: 45295 RVA: 0x0050B614 File Offset: 0x00509814
		private bool GetRightCharacterShadow()
		{
			bool flag = this.Data == null;
			return !flag && !this.isLeftCharacter && this.Data.ExtraData.RightCharacterShadow;
		}

		// Token: 0x0600B0F0 RID: 45296 RVA: 0x0050B654 File Offset: 0x00509854
		private bool GetRightForbiddenConsummateLevel()
		{
			bool flag = this.Data == null;
			return !flag && !this.isLeftCharacter && this.Data.ExtraData.RightForbiddenConsummateLevel;
		}

		// Token: 0x0600B0F1 RID: 45297 RVA: 0x0050B694 File Offset: 0x00509894
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
				bool flag2 = characterDisplayData != null && characterDisplayData.CharacterId == this.TaiwuCharId;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = this.GetEventActorData() != null;
					result = (!flag3 && (this.isLeftCharacter ? (!this.Data.ExtraData.HideLeftFavorability) : (!this.Data.ExtraData.HideRightFavorability)));
				}
			}
			return result;
		}

		// Token: 0x0600B0F2 RID: 45298 RVA: 0x0050B720 File Offset: 0x00509920
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
					bool flag3 = characterDisplayData.CharacterId == this.TaiwuCharId;
					if (flag3)
					{
						result = true;
					}
					else
					{
						bool flag4 = this.GetEventActorData() != null;
						result = (flag4 || (this.isLeftCharacter ? this.Data.ExtraData.LeftForbidShowFavorChangeEffect : this.Data.ExtraData.RightForbidShowFavorChangeEffect));
					}
				}
			}
			return result;
		}

		// Token: 0x0600B0F3 RID: 45299 RVA: 0x0050B7AC File Offset: 0x005099AC
		private bool IsCharacterMenuBtnInteractable()
		{
			return this.showCharacterMenuBtn.gameObject.activeSelf && SingletonObject.getInstance<TutorialChapterModel>().OpenCharacterMenuEnable && !UIManager.Instance.IsElementActive(UIElement.CharacterMenu);
		}

		// Token: 0x0600B0F4 RID: 45300 RVA: 0x0050B7F4 File Offset: 0x005099F4
		private void RefreshCharacterMenuBtnState()
		{
			bool interactable = this.IsCharacterMenuBtnInteractable();
			this.showCharacterMenuBtn.interactable = interactable;
			bool flag = this.avatarBtn;
			if (flag)
			{
				this.avatarBtn.interactable = interactable;
			}
		}

		// Token: 0x0600B0F5 RID: 45301 RVA: 0x0050B834 File Offset: 0x00509A34
		private void SetCharacterMenuBtn(bool show)
		{
			this.showCharacterMenuBtn.gameObject.SetActive(show);
			this.quickBtn.SetActive(show);
			bool flag = this.avatarBtn;
			if (flag)
			{
				this.avatarBtn.interactable = show;
			}
			if (show)
			{
				this.SetRelationLifeRecordBtn();
			}
		}

		// Token: 0x0600B0F6 RID: 45302 RVA: 0x0050B889 File Offset: 0x00509A89
		private void SetRelationLifeRecordBtn()
		{
			this.quickBtnMask.SetActive(true);
			CharacterDomainMethod.AsyncCall.GetViewCharacterMenuDisplayData(null, this._curCharacterId, null, delegate(int offset, RawDataPool pool)
			{
				ViewCharacterMenuDisplayData displayData = new ViewCharacterMenuDisplayData();
				Serializer.Deserialize(pool, offset, ref displayData);
				CharacterDisplayData characterDisplayData;
				this.relationBtn.interactable = (this.TryGetDisplayData(displayData, this._curCharacterId, out characterDisplayData) && characterDisplayData.CreatingType == 1);
				bool canViewLifeRecord = this.GetCanViewLifeRecord(displayData, this._curCharacterId);
				this.lifeRecordBtn.interactable = (canViewLifeRecord && this.relationBtn.interactable && !UIManager.Instance.IsElementActive(UIElement.CharacterMenu));
				this.lifeRecordBtn.GetComponent<PointerTrigger>().enabled = (canViewLifeRecord && this.relationBtn.interactable);
				this.quickBtnMask.SetActive(false);
			});
		}

		// Token: 0x0600B0F7 RID: 45303 RVA: 0x0050B8B4 File Offset: 0x00509AB4
		private bool GetCanViewLifeRecord(ViewCharacterMenuDisplayData viewCharacterMenuDisplayData, int curCharacterId)
		{
			return ((viewCharacterMenuDisplayData != null) ? viewCharacterMenuDisplayData.NoNameInfantCharIds : null) == null || !viewCharacterMenuDisplayData.NoNameInfantCharIds.Contains(curCharacterId);
		}

		// Token: 0x0600B0F8 RID: 45304 RVA: 0x0050B8E8 File Offset: 0x00509AE8
		private bool TryGetDisplayData(ViewCharacterMenuDisplayData viewCharacterMenuDisplayData, int charId, out CharacterDisplayData displayData)
		{
			foreach (CharacterDisplayData data in viewCharacterMenuDisplayData.CharacterDisplayDataList)
			{
				bool flag = data.CharacterId == charId;
				if (flag)
				{
					displayData = data;
					return true;
				}
			}
			displayData = null;
			return false;
		}

		// Token: 0x0600B0F9 RID: 45305 RVA: 0x0050B958 File Offset: 0x00509B58
		public EventWindowCharacter(Action<int, int, bool> onViewCharacter)
		{
			this.OnViewCharacter = onViewCharacter;
		}

		// Token: 0x0600B0FA RID: 45306 RVA: 0x0050B9B0 File Offset: 0x00509BB0
		public void JieqingMaskRefresh(bool needRefresh, List<int> jieqingMaskCharIdList)
		{
			RectTransform rectTransform = this.jieqingMaskHolder.GetComponent<RectTransform>();
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
				particle.Play();
				this._jieqingHasMask = hasMask;
			}
			if (needRefresh)
			{
				this.Refresh();
			}
			else
			{
				this._skipRefreshForOnce = true;
			}
		}

		// Token: 0x0600B0FB RID: 45307 RVA: 0x0050BAEB File Offset: 0x00509CEB
		public void ResetJieqingHasMaskState()
		{
			this._jieqingHasMask = false;
		}

		// Token: 0x0600B0FC RID: 45308 RVA: 0x0050BAF5 File Offset: 0x00509CF5
		public void PointerTriggerEnterShowLifeRecord()
		{
			this.characterLifeRecords.Set(null, this._curCharacterId, false);
			this.characterLifeRecords.gameObject.SetActive(true);
		}

		// Token: 0x0600B0FD RID: 45309 RVA: 0x0050BB1E File Offset: 0x00509D1E
		public void PointerTriggerExitShowLifeRecord()
		{
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.2f, delegate
			{
				bool flag = this.lifeRecordBtn == null || this.characterLifeRecords == null;
				if (!flag)
				{
					PointerTrigger lifeRecordBtnPointerTrigger = this.lifeRecordBtn.GetComponent<PointerTrigger>();
					PointerTrigger lifeRecordPointerTrigger = this.characterLifeRecords.GetComponent<PointerTrigger>();
					bool flag2 = !lifeRecordBtnPointerTrigger.AtEnter && !lifeRecordPointerTrigger.AtEnter;
					if (flag2)
					{
						this.characterLifeRecords.gameObject.SetActive(false);
					}
				}
			});
		}

		// Token: 0x0600B100 RID: 45312 RVA: 0x0050BBF4 File Offset: 0x00509DF4
		[CompilerGenerated]
		internal static void <RefreshAlertness>g__SetTip|108_0(TooltipInvoker tip, ref EventWindowCharacter.<>c__DisplayClass108_0 A_1)
		{
			tip.Type = TipType.Alertness;
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			tip.RuntimeParam.Clear();
			tip.RuntimeParam.Set("charId", A_1.characterDisplayData.CharacterId);
		}

		// Token: 0x040088CC RID: 35020
		public bool isLeftCharacter;

		// Token: 0x040088CD RID: 35021
		public Game.Components.Avatar.Avatar avatar;

		// Token: 0x040088CE RID: 35022
		public CButton showCharacterMenuBtn;

		// Token: 0x040088CF RID: 35023
		public CButton alertnessBtn;

		// Token: 0x040088D0 RID: 35024
		public CButton injuryBtn;

		// Token: 0x040088D1 RID: 35025
		public CButton itemBtn;

		// Token: 0x040088D2 RID: 35026
		public CButton relationBtn;

		// Token: 0x040088D3 RID: 35027
		public CButton lifeRecordBtn;

		// Token: 0x040088D4 RID: 35028
		public CButton avatarBtn;

		// Token: 0x040088D5 RID: 35029
		public CImage favorabilityIcon;

		// Token: 0x040088D6 RID: 35030
		public CImage favorProgressFill;

		// Token: 0x040088D7 RID: 35031
		public CImage consummateLevelIcon;

		// Token: 0x040088D8 RID: 35032
		public CImage happinessProgressFill;

		// Token: 0x040088D9 RID: 35033
		public GameObject favorRoot;

		// Token: 0x040088DA RID: 35034
		public GameObject showCharacterMenuTips;

		// Token: 0x040088DB RID: 35035
		public GameObject characterInfoHolder;

		// Token: 0x040088DC RID: 35036
		public GameObject quickBtnMask;

		// Token: 0x040088DD RID: 35037
		public GameObject quickBtn;

		// Token: 0x040088DE RID: 35038
		[Space]
		public GameObject favorLevelUpEffectObject;

		// Token: 0x040088DF RID: 35039
		public GameObject favorLevelDownEffectObject;

		// Token: 0x040088E0 RID: 35040
		public GameObject happinessLevelUpEffectObject;

		// Token: 0x040088E1 RID: 35041
		public GameObject happinessLevelDownEffectObject;

		// Token: 0x040088E2 RID: 35042
		[Space]
		public GridLayoutGroup relationLayout;

		// Token: 0x040088E3 RID: 35043
		public TextMeshProUGUI consummateLevelLabel;

		// Token: 0x040088E4 RID: 35044
		public TextMeshProUGUI consummateLevelName;

		// Token: 0x040088E5 RID: 35045
		public TextMeshProUGUI nameLabel;

		// Token: 0x040088E6 RID: 35046
		public TextMeshProUGUI favorabilityLabel;

		// Token: 0x040088E7 RID: 35047
		public Refers behaviorHolder;

		// Token: 0x040088E8 RID: 35048
		public Refers identityHolder;

		// Token: 0x040088E9 RID: 35049
		public Refers jieqingMaskHolder;

		// Token: 0x040088EA RID: 35050
		public Refers happinessRoot;

		// Token: 0x040088EB RID: 35051
		public Game.Components.Character.LifeRecord.LifeRecord characterLifeRecords;

		// Token: 0x040088EC RID: 35052
		public RectTransform infoLine2Rect;

		// Token: 0x040088ED RID: 35053
		public GameObject guardBack;

		// Token: 0x040088EE RID: 35054
		public CImage guardIcon;

		// Token: 0x040088EF RID: 35055
		private bool _skipRefreshForOnce = false;

		// Token: 0x040088F0 RID: 35056
		[SerializeField]
		private GameObject otherRootLine;

		// Token: 0x040088F1 RID: 35057
		[SerializeField]
		private Sprite[] fillSprites;

		// Token: 0x040088F2 RID: 35058
		[Header("戒心")]
		[SerializeField]
		private PropertyItem alertnessProperty;

		// Token: 0x040088F3 RID: 35059
		[SerializeField]
		private CImage alertnessProgress;

		// Token: 0x040088F4 RID: 35060
		[SerializeField]
		private TooltipInvoker aiActionToolTip;

		// Token: 0x040088F5 RID: 35061
		[Header("志向")]
		[SerializeField]
		private GameObject professionInfo;

		// Token: 0x040088F6 RID: 35062
		[SerializeField]
		private TooltipInvoker professionInfoTooltip;

		// Token: 0x040088F7 RID: 35063
		[SerializeField]
		private TextMeshProUGUI txtProfessionName;

		// Token: 0x040088F8 RID: 35064
		[Header("位置")]
		[SerializeField]
		private Transform showCharacterPositionRight;

		// Token: 0x040088F9 RID: 35065
		[SerializeField]
		private Transform showCharacterPositionLeft;

		// Token: 0x040088FA RID: 35066
		public Action<int, int, bool> OnViewCharacter;

		// Token: 0x040088FB RID: 35067
		private EventModel _model;

		// Token: 0x040088FC RID: 35068
		private int _curCharacterId;

		// Token: 0x040088FD RID: 35069
		private int[] _notedFavorData = new int[]
		{
			-1,
			26
		};

		// Token: 0x040088FE RID: 35070
		private BasicInfoMonitor _basicInfoMonitor;

		// Token: 0x040088FF RID: 35071
		private DetailInfoMonitor _happinessMonitor;

		// Token: 0x04008900 RID: 35072
		private CharacterFavorability _favorabilityHandler;

		// Token: 0x04008901 RID: 35073
		private int[] _notedHappinessData = new int[]
		{
			-1,
			int.MinValue
		};

		// Token: 0x04008902 RID: 35074
		private int _lastFavorUiSoundFrame = -1;

		// Token: 0x04008903 RID: 35075
		private CharacterRelationShip _relationShipHandler;

		// Token: 0x04008904 RID: 35076
		private CharacterConsummateLevel _consummateLevelHandler;

		// Token: 0x04008905 RID: 35077
		private CharacterBehavior _behaviorHandler;

		// Token: 0x04008906 RID: 35078
		private CharacterHappiness _happinessHandler;

		// Token: 0x04008907 RID: 35079
		private CharacterOrganization _identityHandler;

		// Token: 0x0400890A RID: 35082
		private bool _showFavorabilityFlag;

		// Token: 0x0400890B RID: 35083
		private bool _forbidShowFavorChangeEffectFlag;

		// Token: 0x0400890C RID: 35084
		private bool _waitRefreshFavorFlag;

		// Token: 0x0400890D RID: 35085
		private Action _onEnableAction;

		// Token: 0x0400890E RID: 35086
		private bool _jieqingHasMask;

		// Token: 0x0400890F RID: 35087
		private ParticleSystem _jieqingPutOnMaskAdult;

		// Token: 0x04008910 RID: 35088
		private ParticleSystem _jieqingPutOnMaskChild;

		// Token: 0x04008911 RID: 35089
		private ParticleSystem _jieqingTakeOffMaskAdult;

		// Token: 0x04008912 RID: 35090
		private ParticleSystem _jieqingTakeOffMaskChild;

		// Token: 0x02002563 RID: 9571
		public class GuardTipRuntime : CommonTipBaseRuntime
		{
			// Token: 0x06010B9D RID: 68509 RVA: 0x0066CAC7 File Offset: 0x0066ACC7
			public GuardTipRuntime(CommonTipItem configLine) : base(configLine)
			{
			}

			// Token: 0x06010B9E RID: 68510 RVA: 0x0066CAE4 File Offset: 0x0066ACE4
			public EventWindowCharacter.GuardTipRuntime Set(string key, string value)
			{
				bool flag = string.IsNullOrEmpty(key);
				EventWindowCharacter.GuardTipRuntime result;
				if (flag)
				{
					result = this;
				}
				else
				{
					bool flag2 = value == null;
					bool changed;
					if (flag2)
					{
						changed = this._arguments.Remove(key);
					}
					else
					{
						string oldValue;
						changed = (!this._arguments.TryGetValue(key, out oldValue) || oldValue != value);
						this._arguments[key] = value;
					}
					bool flag3 = changed;
					if (flag3)
					{
						base.RefreshOwner();
					}
					result = this;
				}
				return result;
			}

			// Token: 0x06010B9F RID: 68511 RVA: 0x0066CB58 File Offset: 0x0066AD58
			public override string GetArgument(string key)
			{
				string value;
				return this._arguments.TryGetValue(key, out value) ? value : null;
			}

			// Token: 0x06010BA0 RID: 68512 RVA: 0x0066CB80 File Offset: 0x0066AD80
			public override bool ShouldShowParagraph(string name)
			{
				return name != "Disable" || this._shouldShow;
			}

			// Token: 0x06010BA1 RID: 68513 RVA: 0x0066CBA8 File Offset: 0x0066ADA8
			public void SetDisableStringDisplay(bool value)
			{
				this._shouldShow = value;
			}

			// Token: 0x0400E7CC RID: 59340
			private readonly Dictionary<string, string> _arguments = new Dictionary<string, string>();

			// Token: 0x0400E7CD RID: 59341
			private bool _shouldShow = false;
		}
	}
}
