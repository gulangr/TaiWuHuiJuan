using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using UICommon.Character;
using UnityEngine;

namespace Game.Components.Avatar
{
	// Token: 0x02000F6E RID: 3950
	public class Avatar : MonoBehaviour
	{
		// Token: 0x17001481 RID: 5249
		// (get) Token: 0x0600B4A9 RID: 46249 RVA: 0x005232F1 File Offset: 0x005214F1
		public bool PreferDynamicAvatar
		{
			get
			{
				return this.preferDynamicAvatar && SingletonObject.getInstance<GlobalSettings>().ShowDynamicAvatarIfPossible;
			}
		}

		// Token: 0x17001482 RID: 5250
		// (get) Token: 0x0600B4AA RID: 46250 RVA: 0x00523308 File Offset: 0x00521508
		public AvatarSize Size
		{
			get
			{
				return this.size;
			}
		}

		// Token: 0x17001483 RID: 5251
		// (get) Token: 0x0600B4AB RID: 46251 RVA: 0x00523310 File Offset: 0x00521510
		// (set) Token: 0x0600B4AC RID: 46252 RVA: 0x00523318 File Offset: 0x00521518
		public AvatarData Data { get; private set; }

		// Token: 0x17001484 RID: 5252
		// (get) Token: 0x0600B4AD RID: 46253 RVA: 0x00523321 File Offset: 0x00521521
		// (set) Token: 0x0600B4AE RID: 46254 RVA: 0x00523329 File Offset: 0x00521529
		public short AvatarAge { get; set; }

		// Token: 0x17001485 RID: 5253
		// (get) Token: 0x0600B4AF RID: 46255 RVA: 0x00523332 File Offset: 0x00521532
		// (set) Token: 0x0600B4B0 RID: 46256 RVA: 0x0052333A File Offset: 0x0052153A
		public bool IgnoreAgeControlOfWhiteHair { get; set; }

		// Token: 0x17001486 RID: 5254
		// (get) Token: 0x0600B4B1 RID: 46257 RVA: 0x00523343 File Offset: 0x00521543
		// (set) Token: 0x0600B4B2 RID: 46258 RVA: 0x0052334B File Offset: 0x0052154B
		public bool LogDetail { get; set; }

		// Token: 0x17001487 RID: 5255
		// (get) Token: 0x0600B4B3 RID: 46259 RVA: 0x00523354 File Offset: 0x00521554
		// (set) Token: 0x0600B4B4 RID: 46260 RVA: 0x0052335C File Offset: 0x0052155C
		public object UserObject { get; set; }

		// Token: 0x17001488 RID: 5256
		// (get) Token: 0x0600B4B5 RID: 46261 RVA: 0x00523365 File Offset: 0x00521565
		// (set) Token: 0x0600B4B6 RID: 46262 RVA: 0x0052336D File Offset: 0x0052156D
		public bool KeepSkeletonAnimationOnRefresh { get; set; }

		// Token: 0x17001489 RID: 5257
		// (get) Token: 0x0600B4B7 RID: 46263 RVA: 0x00523376 File Offset: 0x00521576
		// (set) Token: 0x0600B4B8 RID: 46264 RVA: 0x0052337E File Offset: 0x0052157E
		public EFacePartVisibility FacePartVisibility { get; set; } = EFacePartVisibility.All;

		// Token: 0x1700148A RID: 5258
		// (get) Token: 0x0600B4B9 RID: 46265 RVA: 0x00523387 File Offset: 0x00521587
		private static Color WhiteHairColor
		{
			get
			{
				return Colors.Instance["whitehair"];
			}
		}

		// Token: 0x1700148B RID: 5259
		// (get) Token: 0x0600B4BA RID: 46266 RVA: 0x00523398 File Offset: 0x00521598
		private byte AvatarId
		{
			get
			{
				bool flag = AgeGroup.GetAgeGroup(this.AvatarAge) == 0;
				byte result;
				if (flag)
				{
					result = byte.MaxValue;
				}
				else
				{
					bool flag2 = this.AvatarAge < 16;
					if (flag2)
					{
						result = SingletonObject.getInstance<AvatarManager>().GetChildAvatarIdByAvatarId(this.Data.AvatarId);
					}
					else
					{
						result = this.Data.AvatarId;
					}
				}
				return result;
			}
		}

		// Token: 0x1700148C RID: 5260
		// (get) Token: 0x0600B4BB RID: 46267 RVA: 0x005233F8 File Offset: 0x005215F8
		public bool IsSkeleton
		{
			get
			{
				AvatarData data = this.Data;
				return data != null && data.HeadId >= byte.MaxValue;
			}
		}

		// Token: 0x1700148D RID: 5261
		// (get) Token: 0x0600B4BC RID: 46268 RVA: 0x00523422 File Offset: 0x00521622
		// (set) Token: 0x0600B4BD RID: 46269 RVA: 0x00523448 File Offset: 0x00521648
		private bool CommonAvatarPreferSkeleton
		{
			get
			{
				return this.PreferDynamicAvatar && this.avatarSkeleton && this.avatarSkeleton.enabled;
			}
			set
			{
				bool flag = this.avatarSkeleton;
				if (flag)
				{
					bool flag2 = this.avatarSkeleton.enabled != value;
					if (flag2)
					{
						this.avatarSkeleton.enabled = value;
						this.Refresh();
					}
				}
			}
		}

		// Token: 0x1700148E RID: 5262
		// (get) Token: 0x0600B4BE RID: 46270 RVA: 0x00523492 File Offset: 0x00521692
		private bool CommonAvatarUsingSkeleton
		{
			get
			{
				return this.PreferDynamicAvatar && this.avatarSkeleton && this.avatarSkeleton.isActiveAndEnabled;
			}
		}

		// Token: 0x0600B4BF RID: 46271 RVA: 0x005234B7 File Offset: 0x005216B7
		private void Awake()
		{
			this.extraPartTemplate.gameObject.SetActive(false);
		}

		// Token: 0x0600B4C0 RID: 46272 RVA: 0x005234CC File Offset: 0x005216CC
		private void OnEnable()
		{
			bool flag = this.preferDynamicAvatar;
			if (flag)
			{
				this._usingDynamicAvatar = SingletonObject.getInstance<GlobalSettings>().ShowDynamicAvatarIfPossible;
				GEvent.Add(UiEvents.RefreshBookList, new GEvent.Callback(this.RefreshCall));
			}
		}

		// Token: 0x0600B4C1 RID: 46273 RVA: 0x00523510 File Offset: 0x00521710
		private void OnDisable()
		{
			bool flag = this.preferDynamicAvatar;
			if (flag)
			{
				GEvent.Remove(UiEvents.RefreshBookList, new GEvent.Callback(this.RefreshCall));
			}
			CharacterAvatar characterAvatar = this.UserObject as CharacterAvatar;
			bool flag2 = characterAvatar != null;
			if (flag2)
			{
				characterAvatar.CharacterId = -1;
			}
			this.FreeClothParticle();
		}

		// Token: 0x0600B4C2 RID: 46274 RVA: 0x00523568 File Offset: 0x00521768
		private void RefreshCall(ArgumentBox _)
		{
			bool flag = this._usingDynamicAvatar == SingletonObject.getInstance<GlobalSettings>().ShowDynamicAvatarIfPossible;
			if (!flag)
			{
				this._usingDynamicAvatar = !this._usingDynamicAvatar;
				this.Refresh();
			}
		}

		// Token: 0x0600B4C3 RID: 46275 RVA: 0x005235A4 File Offset: 0x005217A4
		private void ApplyAvatarOffset(Vector2? additionalOffset = null)
		{
			bool flag = AvatarSetting.Instance == null;
			if (!flag)
			{
				bool flag2 = null != this.FaceRectIfNpc;
				if (flag2)
				{
					this.FixedAvatarOffsetSetIndex = 1;
					this.NormalAvatarOffsetSetIndex = 1;
				}
				Vector2 finalOffset = Vector2.zero;
				float finalScale = 1f;
				string assetName = (this.cloth.sprite != null) ? this.cloth.sprite.name : this._currentSpineName;
				bool appliedFixedOffset = false;
				bool flag3 = this.FixedAvatarOffsetSetIndex > 0 && !string.IsNullOrEmpty(assetName);
				if (flag3)
				{
					Vector2 fixedOffset;
					float fixedScale;
					bool flag4 = AvatarSetting.Instance.TryGetFixedAvatarOffset(this.FixedAvatarOffsetSetIndex, assetName, this.size, out fixedOffset, out fixedScale);
					if (flag4)
					{
						finalOffset += fixedOffset;
						finalScale *= fixedScale;
						appliedFixedOffset = true;
					}
				}
				bool flag5 = this.NormalAvatarOffsetSetIndex > 0 && !appliedFixedOffset;
				if (flag5)
				{
					Vector2 normalOffset = AvatarSetting.Instance.GetNormalAvatarOffset(this.NormalAvatarOffsetSetIndex, this.AvatarId, this.size);
					finalOffset += normalOffset;
				}
				bool flag6 = additionalOffset != null;
				if (flag6)
				{
					finalOffset = additionalOffset.Value;
				}
				bool flag7 = this.avatarContainer != null;
				if (flag7)
				{
					RectTransform rectTransform = this.avatarContainer.GetComponent<RectTransform>();
					bool flag8 = rectTransform != null;
					if (flag8)
					{
						rectTransform.anchoredPosition = finalOffset;
						rectTransform.localScale = Vector3.one * finalScale;
					}
				}
				bool flag9 = this.avatarSkeleton != null;
				if (flag9)
				{
					RectTransform skeletonTransform = this.avatarSkeleton.GetComponent<RectTransform>();
					bool flag10 = skeletonTransform != null;
					if (flag10)
					{
						skeletonTransform.anchoredPosition = finalOffset;
					}
				}
			}
		}

		// Token: 0x0600B4C4 RID: 46276 RVA: 0x00523750 File Offset: 0x00521950
		public void RefreshAsGrave()
		{
			bool flag = null == Avatar._graveSprite;
			if (flag)
			{
				ResLoader.LoadModOrGameResource<Sprite>(CharacterAvatar.GraveResPath, delegate(Sprite sprite)
				{
					Avatar._graveSprite = sprite;
					this.RefreshGraveSprite(sprite);
				}, delegate(string _)
				{
					this.ResetToBlank(false);
				});
			}
			else
			{
				this.RefreshGraveSprite(Avatar._graveSprite);
			}
		}

		// Token: 0x0600B4C5 RID: 46277 RVA: 0x005237A0 File Offset: 0x005219A0
		private void RefreshGraveSprite(Sprite sprite)
		{
			this.avatarContainer.SetActive(false);
			AvatarSkeleton avatarSkeleton = this.avatarSkeleton;
			if (avatarSkeleton != null)
			{
				avatarSkeleton.gameObject.SetActive(false);
			}
			this.gravestoneContainer.SetActive(true);
			this.gravestoneImage.sprite = sprite;
			this.gravestoneImage.SetNativeSize();
			this.gravestoneImage.enabled = true;
			AvatarSize avatarSize = this.size;
			if (!true)
			{
			}
			float num;
			switch (avatarSize)
			{
			case AvatarSize.Big:
				num = 1f;
				break;
			case AvatarSize.Normal:
				num = 0.5f;
				break;
			case AvatarSize.Small:
				num = 0.25f;
				break;
			default:
				num = 1f;
				break;
			}
			if (!true)
			{
			}
			float scale = num;
			this.gravestoneImage.rectTransform.localScale = Vector3.one * scale;
		}

		// Token: 0x0600B4C6 RID: 46278 RVA: 0x00523867 File Offset: 0x00521A67
		private void ShowNormalState()
		{
			this.avatarContainer.SetActive(true);
			this.gravestoneContainer.SetActive(false);
		}

		// Token: 0x0600B4C7 RID: 46279 RVA: 0x00523884 File Offset: 0x00521A84
		public void Refresh(CharacterDisplayData displayData, bool isShowGrave = true)
		{
			bool flag = displayData.AliveState == 1 && isShowGrave;
			if (flag)
			{
				this.RefreshAsGrave();
			}
			else
			{
				this.ShowNormalState();
				EventModel eventModel = SingletonObject.getInstance<EventModel>();
				bool flag2 = eventModel.NeedShowAsMarriageLook1(displayData.CharacterId);
				if (flag2)
				{
					displayData.AvatarRelatedData.AvatarData.ChangeToMarriageStyle1();
					displayData.AvatarRelatedData.ClothingDisplayId = displayData.AvatarRelatedData.AvatarData.ClothDisplayId;
				}
				bool flag3 = eventModel.NeedShowAsMarriageLook2(displayData.CharacterId);
				if (flag3)
				{
					displayData.AvatarRelatedData.AvatarData.ChangeToMarriageStyle2();
					displayData.AvatarRelatedData.ClothingDisplayId = displayData.AvatarRelatedData.AvatarData.ClothDisplayId;
				}
				bool flag4 = eventModel.NeedShowShixiangBarbarianMasterCloth(displayData.CharacterId);
				if (flag4)
				{
					displayData.AvatarRelatedData.AvatarData.ChangeToShixiangBarbarianMaster();
					displayData.AvatarRelatedData.ClothingDisplayId = displayData.AvatarRelatedData.AvatarData.ClothDisplayId;
				}
				short taiwuClothDisplayId;
				bool flag5 = eventModel.TryGetTaiwuClothingDisplayId(displayData.CharacterId, out taiwuClothDisplayId);
				if (flag5)
				{
					displayData.AvatarRelatedData.AvatarData.ClothDisplayId = taiwuClothDisplayId;
					displayData.AvatarRelatedData.ClothingDisplayId = taiwuClothDisplayId;
				}
				displayData.AvatarRelatedData.AvatarData.ShowJieqingMask = eventModel.NeedShowJieqingMask(displayData.CharacterId);
				displayData.AvatarRelatedData.AvatarData.ShowBlush = eventModel.NeedShowBlush(displayData.CharacterId);
				eventModel.CheckAvatarClothDisplayIdForEvent(displayData.CharacterId, displayData.AvatarRelatedData.AvatarData, displayData.AvatarRelatedData);
				displayData.AvatarRelatedData.AvatarData.DarkAshStyle = CommonUtils.GetDarkAshStyle(displayData);
				sbyte infectionStyle = CommonUtils.GetXiangshuInfectionStyle(displayData);
				displayData.AvatarRelatedData.AvatarData.XiangshuInfectionStyle = infectionStyle;
				this.Refresh(displayData.AvatarRelatedData, displayData.TemplateId);
				sbyte huanxinStyle = CommonUtils.GetHuanxinFaceStyle(displayData);
				displayData.AvatarRelatedData.AvatarData.HuanxinFaceStyle = huanxinStyle;
				bool flag6 = this.Data != null;
				if (flag6)
				{
					this.Data.HuanxinFaceStyle = huanxinStyle;
					this.TryAddHuanxinFaceSprite();
				}
			}
		}

		// Token: 0x0600B4C8 RID: 46280 RVA: 0x00523A88 File Offset: 0x00521C88
		public void Refresh(AvatarRelatedData relatedData)
		{
			this.ShowNormalState();
			relatedData.AvatarData.ClothDisplayId = relatedData.ClothingDisplayId;
			this.AvatarAge = relatedData.DisplayAge;
			this.Data = relatedData.AvatarData;
			this.Data.HuanxinFaceStyle = -1;
			this.Refresh();
		}

		// Token: 0x0600B4C9 RID: 46281 RVA: 0x00523ADC File Offset: 0x00521CDC
		public void Refresh(AvatarRelatedData relatedData, short characterTemplateId)
		{
			CharacterItem config = Character.Instance[characterTemplateId];
			bool flag = CreatingType.IsFixedPresetType(config.CreatingType);
			if (flag)
			{
				string spineName = config.FixedAvatarSpineName;
				string skinName = config.FixedAvatarSpineSkin;
				bool flag2 = !string.IsNullOrEmpty(spineName) && this.npcSkeleton != null && this.PreferDynamicAvatar;
				if (flag2)
				{
					this.RefreshAsSpine(spineName, skinName);
					return;
				}
				bool flag3 = !string.IsNullOrEmpty(config.FixedAvatarName);
				if (flag3)
				{
					AvatarSize avatarSize = this.size;
					if (!true)
					{
					}
					string text;
					if (avatarSize != AvatarSize.Normal)
					{
						if (avatarSize != AvatarSize.Small)
						{
							text = "BigFace";
						}
						else
						{
							text = "SmallFace";
						}
					}
					else
					{
						text = "NormalFace";
					}
					if (!true)
					{
					}
					string sizeFolder = text;
					string resPath = "NpcFace/" + sizeFolder + "/" + config.FixedAvatarName;
					ResLoader.LoadModOrGameResource<Texture2D>(resPath, new Action<Texture2D>(this.Refresh), delegate(string _)
					{
						this.Refresh(relatedData);
					});
					return;
				}
			}
			this.Refresh(relatedData);
		}

		// Token: 0x0600B4CA RID: 46282 RVA: 0x00523BFC File Offset: 0x00521DFC
		public void RefreshCustom(AvatarRelatedData relatedData, string customPicture)
		{
			bool flag = !string.IsNullOrWhiteSpace(customPicture);
			if (flag)
			{
				AvatarSize avatarSize = this.size;
				if (!true)
				{
				}
				string text;
				if (avatarSize != AvatarSize.Normal)
				{
					if (avatarSize != AvatarSize.Small)
					{
						text = "BigFace";
					}
					else
					{
						text = "SmallFace";
					}
				}
				else
				{
					text = "NormalFace";
				}
				if (!true)
				{
				}
				string sizeFolder = text;
				string resPath = "RemakeResources/Textures/CustomNpcFace/" + sizeFolder + "/" + customPicture;
				ResLoader.LoadModOrGameResource<Texture2D>(resPath, new Action<Texture2D>(this.Refresh), delegate(string _)
				{
					this.Refresh(relatedData);
				});
			}
			else
			{
				this.Refresh(relatedData);
			}
		}

		// Token: 0x0600B4CB RID: 46283 RVA: 0x00523CA8 File Offset: 0x00521EA8
		public void Refresh(AvatarData avatarData, short displayAge)
		{
			this.ShowNormalState();
			this.Data = avatarData;
			this.Data.HuanxinFaceStyle = -1;
			this.AvatarAge = displayAge;
			this.Refresh();
		}

		// Token: 0x0600B4CC RID: 46284 RVA: 0x00523CD8 File Offset: 0x00521ED8
		public void Refresh(Sprite pictureSprite)
		{
			this.Refresh(pictureSprite, null);
		}

		// Token: 0x0600B4CD RID: 46285 RVA: 0x00523CF8 File Offset: 0x00521EF8
		public void Refresh(Sprite pictureSprite, Vector2? additionalOffset)
		{
			AvatarSkeleton avatarSkeleton = this.avatarSkeleton;
			if (avatarSkeleton != null)
			{
				avatarSkeleton.gameObject.SetActive(false);
			}
			this.ResetToBlank(true);
			this.cloth.sprite = pictureSprite;
			this.cloth.SetNativeSize();
			this.ApplyAvatarOffset(additionalOffset);
			this.cloth.enabled = true;
		}

		// Token: 0x0600B4CE RID: 46286 RVA: 0x00523D54 File Offset: 0x00521F54
		public void Refresh(Texture2D texture)
		{
			Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, (float)texture.width, (float)texture.height), 0.5f * Vector2.one);
			sprite.name = texture.name;
			this.Refresh(sprite);
		}

		// Token: 0x0600B4CF RID: 46287 RVA: 0x00523DAC File Offset: 0x00521FAC
		public void RefreshAsSpine(string spineName, string skinName)
		{
			Avatar.<>c__DisplayClass124_0 CS$<>8__locals1 = new Avatar.<>c__DisplayClass124_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.skinName = skinName;
			CS$<>8__locals1.spineName = spineName;
			bool flag = this.npcSkeleton == null;
			if (flag)
			{
				Debug.LogError("npcSkeleton is not assigned in Avatar component");
			}
			else
			{
				AvatarSize avatarSize = this.size;
				if (!true)
				{
				}
				float spineScale;
				if (avatarSize != AvatarSize.Big)
				{
					if (avatarSize != AvatarSize.Normal)
					{
						spineScale = 1f;
					}
					else
					{
						spineScale = 0.5f;
					}
				}
				else
				{
					spineScale = 1f;
				}
				if (!true)
				{
				}
				CS$<>8__locals1.spineScale = spineScale;
				AvatarSkeleton avatarSkeleton = this.avatarSkeleton;
				if (avatarSkeleton != null)
				{
					avatarSkeleton.gameObject.SetActive(false);
				}
				this.avatarContainer.gameObject.SetActive(true);
				bool isSameSpine = this.IsCurrentSpine(CS$<>8__locals1.spineName, CS$<>8__locals1.skinName);
				bool flag2 = isSameSpine && this.npcSkeleton.gameObject.activeSelf;
				if (flag2)
				{
					this.npcSkeleton.transform.localScale = Vector3.one * CS$<>8__locals1.spineScale;
					this.ApplyAvatarOffset(null);
				}
				else
				{
					bool flag3 = !isSameSpine;
					if (flag3)
					{
						this.ResetToBlank(false);
					}
					bool flag4 = isSameSpine;
					if (flag4)
					{
						this.npcSkeleton.transform.localScale = Vector3.one * CS$<>8__locals1.spineScale;
						this.npcSkeleton.gameObject.SetActive(true);
						this.ApplyAvatarOffset(null);
					}
					else
					{
						string spineDataPath = "RemakeResources/SpineAnimations/" + CS$<>8__locals1.spineName + "_SkeletonData";
						Avatar.<>c__DisplayClass124_0 CS$<>8__locals2 = CS$<>8__locals1;
						uint num = this._spineRequestVersion + 1U;
						this._spineRequestVersion = num;
						CS$<>8__locals2.requestVersion = num;
						ResLoader.Load<SkeletonDataAsset>(spineDataPath, delegate(SkeletonDataAsset skeletonData)
						{
							bool flag5 = CS$<>8__locals1.<>4__this.npcSkeleton == null;
							if (!flag5)
							{
								bool flag6 = CS$<>8__locals1.<>4__this._spineRequestVersion != CS$<>8__locals1.requestVersion;
								if (!flag6)
								{
									CS$<>8__locals1.<>4__this.npcSkeleton.skeletonDataAsset = skeletonData;
									CS$<>8__locals1.<>4__this.npcSkeleton.initialSkinName = CS$<>8__locals1.skinName;
									CS$<>8__locals1.<>4__this.npcSkeleton.Initialize(true);
									CS$<>8__locals1.<>4__this.npcSkeleton.UnscaledTime = true;
									Spine.Animation[] animations = skeletonData.GetSkeletonData(true).Animations.Items;
									bool flag7 = animations.Length != 0;
									if (flag7)
									{
										CS$<>8__locals1.<>4__this.npcSkeleton.startingAnimation = animations[0].Name;
									}
									CS$<>8__locals1.<>4__this.npcSkeleton.transform.localScale = Vector3.one * CS$<>8__locals1.spineScale;
									CS$<>8__locals1.<>4__this.npcSkeleton.gameObject.SetActive(true);
									Spine.AnimationState animationState = CS$<>8__locals1.<>4__this.npcSkeleton.AnimationState;
									if (animationState != null)
									{
										animationState.SetAnimation(0, CS$<>8__locals1.<>4__this.npcSkeleton.startingAnimation, true);
									}
									CS$<>8__locals1.<>4__this._currentSpineName = CS$<>8__locals1.spineName;
									CS$<>8__locals1.<>4__this._currentSpineSkin = (CS$<>8__locals1.skinName ?? string.Empty);
									CS$<>8__locals1.<>4__this.ApplyAvatarOffset(null);
								}
							}
						}, null, false);
					}
				}
			}
		}

		// Token: 0x0600B4D0 RID: 46288 RVA: 0x00523F70 File Offset: 0x00522170
		public void Refresh()
		{
			this.avatarContainer.gameObject.SetActive(true);
			bool refreshFull = this.InternalRefresh(delegate
			{
				bool commonAvatarPreferSkeleton2 = this.CommonAvatarPreferSkeleton;
				if (commonAvatarPreferSkeleton2)
				{
					this.avatarSkeleton.gameObject.SetActive(true);
				}
			});
			bool commonAvatarPreferSkeleton = this.CommonAvatarPreferSkeleton;
			if (commonAvatarPreferSkeleton)
			{
				bool shouldShowWhiteHair = this.ShouldShowWhiteHair();
				bool hairShow = this.HairShow;
				bool flag = refreshFull;
				if (flag)
				{
					this.avatarSkeleton.HairShown[0].ShouldWhite = shouldShowWhiteHair;
					this.avatarSkeleton.HairShown[0].Bare = !hairShow;
					this.avatarSkeleton.HairShown[1].ShouldWhite = shouldShowWhiteHair;
					this.avatarSkeleton.HairShown[1].Bare = (!hairShow || !this.CalcCanShowBackHair());
				}
				this.avatarSkeleton.KeepAnimationOnRefresh = this.KeepSkeletonAnimationOnRefresh;
				bool canUseSkeleton = refreshFull && this.avatarSkeleton.Refresh(this.Data, new byte?(this.AvatarId));
				this.avatarSkeleton.gameObject.SetActive(canUseSkeleton);
				this.avatarContainer.gameObject.SetActive(!canUseSkeleton);
				bool flag2 = !canUseSkeleton;
				if (flag2)
				{
					this.InternalRefresh(null);
				}
			}
		}

		// Token: 0x0600B4D1 RID: 46289 RVA: 0x005240A8 File Offset: 0x005222A8
		private bool InternalRefresh(Action onStart = null)
		{
			bool flag = this.Data == null || this.Data.AvatarId == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this._manager = SingletonObject.getInstance<AvatarManager>();
				this.ResetToBlank(true);
				if (onStart != null)
				{
					onStart();
				}
				short veilTemplateId = AvatarManagerUtils.GetAvatarVeilTemplateIdByAvatarId(this.AvatarId);
				bool showVeil = this.Data.ShowVeil;
				if (showVeil)
				{
					this.AddExtraPartItem(EAvatarExtraPartsType.Veil, veilTemplateId);
				}
				else
				{
					this.RemoveExtraPartItem(EAvatarExtraPartsType.Veil);
				}
				bool flag2 = !this.Data.ShowBlush;
				if (flag2)
				{
					this.RemoveExtraPartItem(EAvatarExtraPartsType.Blush);
				}
				bool flag3 = AgeGroup.GetAgeGroup(this.AvatarAge) == 0;
				if (flag3)
				{
					this.RefreshAsBaby();
					this.ApplyAvatarOffset(null);
					result = false;
				}
				else
				{
					bool flag4 = this.AvatarAge < 16;
					if (flag4)
					{
						this.RefreshAsChild();
						this.ApplyAvatarOffset(null);
						result = true;
					}
					else
					{
						bool isSkeleton = this.IsSkeleton;
						if (isSkeleton)
						{
							this.UpdateCloth();
							bool flag5 = this.PreferDynamicAvatar && this.avatarSkeleton;
							if (flag5)
							{
								this.avatarSkeleton.RefreshAsSkeleton(this.Data, this.AvatarId);
								this.avatarSkeleton.gameObject.SetActive(true);
								this.avatarContainer.gameObject.SetActive(false);
								this.UpdateHead();
								this.UpdateBackHair();
								this.UpdateFrontHair();
							}
							else
							{
								this.UpdateHead();
								this.UpdateBackHair();
								this.UpdateFrontHair();
							}
							this.TryAddXiangshuInfectionSprite();
							this.TryAddHuanxinFaceSprite();
							this.ApplyAvatarOffset(null);
							result = true;
						}
						else
						{
							this.UpdateCloth();
							this.UpdateHead();
							this.UpdateBackHair();
							this.UpdateFrontHair();
							this.UpdateEyes();
							this.UpdateEyebrows();
							this.UpdateMouth();
							this.UpdateBeard();
							this.UpdateNose();
							this.UpdateFeature();
							this.UpdateWrinkle();
							this.TryAddXiangshuInfectionSprite();
							this.TryAddHuanxinFaceSprite();
							this.ApplyAvatarOffset(null);
							PositionFollower backHairFollower = this.backHair.GetComponent<PositionFollower>();
							bool flag6 = backHairFollower != null;
							if (flag6)
							{
								backHairFollower.Excute();
							}
							this.SyncAllExtraPartPositions();
							this.ApplyFacePartVisibility();
							result = true;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600B4D2 RID: 46290 RVA: 0x00524310 File Offset: 0x00522510
		public void RefreshAsClothTree(AvatarData avatarData, short displayAge, Sprite[] headSprite, string skinColor = "")
		{
			this.AvatarAge = displayAge;
			this.Data = avatarData;
			this.ResetToBlank(true);
			this._manager = SingletonObject.getInstance<AvatarManager>();
			short veilTemplateId = AvatarManagerUtils.GetAvatarVeilTemplateIdByAvatarId(this.AvatarId);
			bool showVeil = this.Data.ShowVeil;
			if (showVeil)
			{
				this.AddExtraPartItem(EAvatarExtraPartsType.Veil, veilTemplateId);
			}
			else
			{
				this.RemoveExtraPartItem(EAvatarExtraPartsType.Veil);
			}
			bool flag = !this.Data.ShowBlush;
			if (flag)
			{
				this.RemoveExtraPartItem(EAvatarExtraPartsType.Blush);
			}
			this.UpdateCloth();
			this.UpdateHead();
			this.headImage.sprite = headSprite[(int)(avatarData.AvatarId - 1)];
			this.headImage.SetNativeSize();
			this.headImage.color = skinColor.HexStringToColor();
			this.clothSkin.color = skinColor.HexStringToColor();
			this.TryAddXiangshuInfectionSprite();
			this.TryAddHuanxinFaceSprite();
		}

		// Token: 0x0600B4D3 RID: 46291 RVA: 0x005243F0 File Offset: 0x005225F0
		public void ResetToBlank(bool clearSpineState = false)
		{
			this.ResetSize();
			AvatarSkeleton avatarSkeleton = this.avatarSkeleton;
			if (avatarSkeleton != null)
			{
				avatarSkeleton.gameObject.SetActive(false);
			}
			if (clearSpineState)
			{
				this._currentSpineName = null;
				this._currentSpineSkin = null;
			}
			this._spineRequestVersion += 1U;
			this.gravestoneContainer.SetActive(false);
			this.gravestoneImage.SetSprite("", false, null);
			this.gravestoneImage.color = Color.white;
			this.avatarContainer.SetActive(true);
			bool flag = this.cloth != null;
			if (flag)
			{
				this.cloth.SetSprite("", false, null);
				this.cloth.color = Color.white;
			}
			bool flag2 = this.clothColor != null;
			if (flag2)
			{
				this.clothColor.SetSprite("", false, null);
				this.clothColor.color = Color.white;
			}
			bool flag3 = this.clothSkin != null;
			if (flag3)
			{
				this.clothSkin.SetSprite("", false, null);
				this.clothSkin.color = Color.white;
			}
			bool flag4 = this.bodyCover != null;
			if (flag4)
			{
				this.bodyCover.localScale = Vector3.one;
				this.bodyCover.anchoredPosition = Vector2.zero;
			}
			bool flag5 = this.clothCover != null;
			if (flag5)
			{
				this.clothCover.SetSprite("", false, null);
				this.clothCover.color = Color.white;
			}
			bool flag6 = this.clothColorCover != null;
			if (flag6)
			{
				this.clothColorCover.SetSprite("", false, null);
				this.clothColorCover.color = Color.white;
			}
			bool flag7 = this.clothSkinCover != null;
			if (flag7)
			{
				this.clothSkinCover.SetSprite("", false, null);
				this.clothSkinCover.color = Color.white;
			}
			bool flag8 = this.headImage != null;
			if (flag8)
			{
				this.headImage.SetSprite("", false, null);
				this.headImage.color = Color.white;
			}
			bool flag9 = this.backHair != null;
			if (flag9)
			{
				this.backHair.SetSprite("", false, null);
				this.backHair.color = Color.white;
			}
			bool flag10 = this.backHairPart != null;
			if (flag10)
			{
				this.backHairPart.SetSprite("", false, null);
				this.backHairPart.color = Color.white;
			}
			bool flag11 = this.frontHair != null;
			if (flag11)
			{
				this.frontHair.SetSprite("", false, null);
				this.frontHair.color = Color.white;
			}
			bool flag12 = this.frontHairPart != null;
			if (flag12)
			{
				this.frontHairPart.SetSprite("", false, null);
				this.frontHairPart.color = Color.white;
			}
			bool flag13 = this.mouth != null;
			if (flag13)
			{
				this.mouth.SetSprite("", false, null);
				this.mouth.color = Color.white;
			}
			bool flag14 = this.mouthPart != null;
			if (flag14)
			{
				this.mouthPart.SetSprite("", false, null);
				this.mouthPart.color = Color.white;
			}
			bool flag15 = this.upperBeard != null;
			if (flag15)
			{
				this.upperBeard.SetSprite("", false, null);
				this.upperBeard.color = Color.white;
			}
			bool flag16 = this.lowerBeard != null;
			if (flag16)
			{
				this.lowerBeard.SetSprite("", false, null);
				this.lowerBeard.color = Color.white;
			}
			bool flag17 = this.nose != null;
			if (flag17)
			{
				this.nose.SetSprite("", false, null);
				this.nose.color = Color.white;
			}
			bool flag18 = this.leftEyebrow != null;
			if (flag18)
			{
				this.leftEyebrow.SetSprite("", false, null);
				this.leftEyebrow.color = Color.white;
			}
			bool flag19 = this.rightEyebrow != null;
			if (flag19)
			{
				this.rightEyebrow.SetSprite("", false, null);
				this.rightEyebrow.color = Color.white;
			}
			bool flag20 = this.leftEye != null;
			if (flag20)
			{
				this.leftEye.SetSprite("", false, null);
				this.leftEye.color = Color.white;
			}
			bool flag21 = this.rightEye != null;
			if (flag21)
			{
				this.rightEye.SetSprite("", false, null);
				this.rightEye.color = Color.white;
			}
			bool flag22 = this.leftEyeball != null;
			if (flag22)
			{
				this.leftEyeball.SetSprite("", false, null);
				this.leftEyeball.color = Color.white;
			}
			bool flag23 = this.rightEyeball != null;
			if (flag23)
			{
				this.rightEyeball.SetSprite("", false, null);
				this.rightEyeball.color = Color.white;
			}
			bool flag24 = this.feature1CenterOrLeft != null;
			if (flag24)
			{
				this.feature1CenterOrLeft.SetSprite("", false, null);
				this.feature1CenterOrLeft.color = Color.white;
			}
			bool flag25 = this.feature1Right != null;
			if (flag25)
			{
				this.feature1Right.SetSprite("", false, null);
				this.feature1Right.color = Color.white;
			}
			bool flag26 = this.feature1LeftEye != null;
			if (flag26)
			{
				this.feature1LeftEye.SetSprite("", false, null);
				this.feature1LeftEye.color = Color.white;
			}
			bool flag27 = this.feature1RightEye != null;
			if (flag27)
			{
				this.feature1RightEye.SetSprite("", false, null);
				this.feature1RightEye.color = Color.white;
			}
			bool flag28 = this.feature2CenterOrLeft != null;
			if (flag28)
			{
				this.feature2CenterOrLeft.SetSprite("", false, null);
				this.feature2CenterOrLeft.color = Color.white;
			}
			bool flag29 = this.feature2Right != null;
			if (flag29)
			{
				this.feature2Right.SetSprite("", false, null);
				this.feature2Right.color = Color.white;
			}
			bool flag30 = this.feature2LeftEye != null;
			if (flag30)
			{
				this.feature2LeftEye.SetSprite("", false, null);
				this.feature2LeftEye.color = Color.white;
			}
			bool flag31 = this.feature2RightEye != null;
			if (flag31)
			{
				this.feature2RightEye.SetSprite("", false, null);
				this.feature2RightEye.color = Color.white;
			}
			bool flag32 = this.wrinkle1 != null;
			if (flag32)
			{
				this.wrinkle1.SetSprite("", false, null);
				this.wrinkle1.color = Color.white;
			}
			bool flag33 = this.wrinkle2 != null;
			if (flag33)
			{
				this.wrinkle2.SetSprite("", false, null);
				this.wrinkle2.color = Color.white;
			}
			bool flag34 = this.wrinkle3Left != null;
			if (flag34)
			{
				this.wrinkle3Left.SetSprite("", false, null);
				this.wrinkle3Left.color = Color.white;
			}
			bool flag35 = this.wrinkle3Right != null;
			if (flag35)
			{
				this.wrinkle3Right.SetSprite("", false, null);
				this.wrinkle3Right.color = Color.white;
			}
			bool flag36 = this.frontHairHighLight != null;
			if (flag36)
			{
				this.frontHairHighLight.SetSprite("", false, null);
				this.frontHairHighLight.color = Color.white;
			}
			bool flag37 = this.backHairHighLight != null;
			if (flag37)
			{
				this.backHairHighLight.SetSprite("", false, null);
				this.backHairHighLight.color = Color.white;
			}
			bool flag38 = this.xiangshuInfectionImage != null;
			if (flag38)
			{
				this.xiangshuInfectionImage.sprite = null;
				this.xiangshuInfectionImage.enabled = false;
			}
			bool flag39 = this.huanxinFaceImage != null;
			if (flag39)
			{
				this.huanxinFaceImage.sprite = null;
				this.huanxinFaceImage.enabled = false;
			}
			AvatarSkeleton avatarSkeleton2 = this.avatarSkeleton;
			if (avatarSkeleton2 != null)
			{
				avatarSkeleton2.TrySetXiangshuInfection(null);
			}
			AvatarSkeleton avatarSkeleton3 = this.avatarSkeleton;
			if (avatarSkeleton3 != null)
			{
				avatarSkeleton3.TrySetHuanxinFace(null);
			}
			this.ResetAllExtraParts();
			AvatarSkeleton avatarSkeleton4 = this.avatarSkeleton;
			if (avatarSkeleton4 != null)
			{
				avatarSkeleton4.LegacyResetAllExtraParts();
			}
			this.FreeClothParticle();
			bool flag40 = this.npcSkeleton != null;
			if (flag40)
			{
				this.npcSkeleton.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600B4D4 RID: 46292 RVA: 0x00524D58 File Offset: 0x00522F58
		public void SetShadowStrength(float strengthValue)
		{
			CImage[] cImages = base.gameObject.GetComponentsInChildren<CImage>();
			foreach (CImage cImage in cImages)
			{
				bool enabled = cImage.enabled;
				if (enabled)
				{
					Avatar.SetImageShadowStrength(cImage, strengthValue);
				}
			}
			bool flag = this.avatarSkeleton != null;
			if (flag)
			{
				this.avatarSkeleton.SetShadowStrength(strengthValue);
			}
		}

		// Token: 0x0600B4D5 RID: 46293 RVA: 0x00524DBC File Offset: 0x00522FBC
		public void AddExtraPartItem(EAvatarExtraPartsType extraItemType, short extraItemTemplateId)
		{
			this.HandleExtraPart(extraItemTemplateId, true, EAvatarElementsType.Count, extraItemType, true);
		}

		// Token: 0x0600B4D6 RID: 46294 RVA: 0x00524DCC File Offset: 0x00522FCC
		public void AddJieqingMaskPartItem(short extraItemTemplateId)
		{
			this.HandleExtraPart(extraItemTemplateId, false, EAvatarElementsType.Cloth, EAvatarExtraPartsType.Mask, true);
		}

		// Token: 0x0600B4D7 RID: 46295 RVA: 0x00524DDC File Offset: 0x00522FDC
		public void TryAddDashAshExtraPartItem()
		{
			sbyte style = this.Data.DarkAshStyle;
			bool flag = style < 0;
			if (!flag)
			{
				short startTemplateId = 46;
				short offsetInStyle = AvatarManagerUtils.GetAvatarDarkAshTemplateIdOffset(this.AvatarId);
				int templateId = (int)(startTemplateId + (short)(style * 10) + offsetInStyle);
				this.AddExtraPartItem(EAvatarExtraPartsType.DashAsh, (short)templateId);
			}
		}

		// Token: 0x0600B4D8 RID: 46296 RVA: 0x00524E24 File Offset: 0x00523024
		public void TryAddXiangshuInfectionSprite()
		{
			bool flag = this.Data.XiangshuInfectionStyle < 0;
			if (flag)
			{
				bool flag2 = this.xiangshuInfectionImage != null;
				if (flag2)
				{
					this.xiangshuInfectionImage.enabled = false;
					this.xiangshuInfectionImage.gameObject.SetActive(false);
				}
				AvatarSkeleton avatarSkeleton = this.avatarSkeleton;
				if (avatarSkeleton != null)
				{
					avatarSkeleton.TrySetXiangshuInfection(null);
				}
			}
			else
			{
				bool flag3 = this.PreferDynamicAvatar && this.avatarSkeleton != null;
				if (flag3)
				{
					this.avatarSkeleton.TrySetXiangshuInfection(this.Data);
					bool flag4 = this.xiangshuInfectionImage != null;
					if (flag4)
					{
						this.xiangshuInfectionImage.enabled = false;
						this.xiangshuInfectionImage.gameObject.SetActive(false);
					}
				}
				else
				{
					byte avatarId = this.Data.AvatarId;
					bool flag5 = avatarId < 1 || avatarId > 6;
					if (flag5)
					{
						bool flag6 = this.xiangshuInfectionImage != null;
						if (flag6)
						{
							this.xiangshuInfectionImage.enabled = false;
							this.xiangshuInfectionImage.gameObject.SetActive(false);
						}
					}
					else
					{
						AvatarSize avatarSize = this.size;
						if (!true)
						{
						}
						string text;
						switch (avatarSize)
						{
						case AvatarSize.Big:
							text = "big";
							break;
						case AvatarSize.Normal:
							text = "normal";
							break;
						case AvatarSize.Small:
							text = "small";
							break;
						default:
							text = "big";
							break;
						}
						if (!true)
						{
						}
						string sizeStr = text;
						string spriteName = string.Format("{0}{1}_{2}_partly", "ui9_back_avatar_infection_", avatarId, sizeStr);
						bool flag7 = this.xiangshuInfectionImage != null;
						if (flag7)
						{
							this.xiangshuInfectionImage.SetSprite(spriteName, false, null);
							this.xiangshuInfectionImage.gameObject.SetActive(this.xiangshuInfectionImage.sprite != null);
						}
					}
				}
			}
		}

		// Token: 0x0600B4D9 RID: 46297 RVA: 0x00524FF8 File Offset: 0x005231F8
		public void TryAddHuanxinFaceSprite()
		{
			bool flag = this.disableXinNian || this.Data.HuanxinFaceStyle < 0 || this.size == AvatarSize.Small;
			if (flag)
			{
				bool flag2 = this.huanxinFaceImage != null;
				if (flag2)
				{
					this.huanxinFaceImage.enabled = false;
				}
				AvatarSkeleton avatarSkeleton = this.avatarSkeleton;
				if (avatarSkeleton != null)
				{
					avatarSkeleton.TrySetHuanxinFace(null);
				}
			}
			else
			{
				sbyte style = this.Data.HuanxinFaceStyle;
				string spriteName = (style >= 0 && (int)style < Avatar.HuanxinFaceStaticSpriteNames.Length) ? Avatar.HuanxinFaceStaticSpriteNames[(int)style] : null;
				bool flag3 = this.PreferDynamicAvatar && this.avatarSkeleton != null;
				if (flag3)
				{
					this.avatarSkeleton.TrySetHuanxinFace(this.Data);
					bool flag4 = spriteName != null;
					if (flag4)
					{
						this.avatarSkeleton.SetHuanxinFaceOffset(Vector2.zero);
					}
					bool flag5 = this.huanxinFaceImage != null;
					if (flag5)
					{
						this.huanxinFaceImage.enabled = false;
					}
				}
				else
				{
					bool flag6 = spriteName == null;
					if (flag6)
					{
						bool flag7 = this.huanxinFaceImage != null;
						if (flag7)
						{
							this.huanxinFaceImage.enabled = false;
						}
					}
					else
					{
						bool flag8 = this.huanxinFaceImage != null;
						if (flag8)
						{
							this.huanxinFaceImage.enabled = false;
							AvatarSize avatarSize = this.size;
							if (!true)
							{
							}
							string text;
							if (avatarSize != AvatarSize.Normal)
							{
								text = "BigFace";
							}
							else
							{
								text = "NormalFace";
							}
							if (!true)
							{
							}
							string sizeFolder = text;
							string resPath = "NpcFace/" + sizeFolder + "/" + spriteName;
							ResLoader.LoadModOrGameResource<Texture2D>(resPath, delegate(Texture2D tex)
							{
								bool flag9 = this.huanxinFaceImage == null;
								if (!flag9)
								{
									bool flag10 = tex != null;
									if (flag10)
									{
										Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, (float)tex.width, (float)tex.height), 0.5f * Vector2.one);
										sprite.name = tex.name;
										this.huanxinFaceImage.sprite = sprite;
										this.huanxinFaceImage.SetNativeSize();
										this.huanxinFaceImage.rectTransform.anchoredPosition = Vector2.zero;
										this.huanxinFaceImage.enabled = true;
									}
								}
							}, null);
						}
					}
				}
			}
		}

		// Token: 0x0600B4DA RID: 46298 RVA: 0x005251A4 File Offset: 0x005233A4
		public void RemoveExtraPartItem(EAvatarExtraPartsType extraItemType)
		{
			bool flag = this._outComeExtraPartMap == null || this._outComeExtraPartMap.Count <= 0;
			if (!flag)
			{
				PositionFollower follower;
				bool flag2 = this._outComeExtraPartMap.TryGetValue(extraItemType, out follower);
				if (flag2)
				{
					follower.gameObject.SetActive(false);
				}
				bool flag3 = this.CommonAvatarPreferSkeleton && this.avatarSkeleton != null;
				if (flag3)
				{
					this.avatarSkeleton.LegacyRemoveExtraPart(extraItemType);
				}
			}
		}

		// Token: 0x0600B4DB RID: 46299 RVA: 0x00525220 File Offset: 0x00523420
		public void ClearExtraParts()
		{
			this.ResetAllExtraParts();
			bool flag = this.CommonAvatarPreferSkeleton && this.avatarSkeleton != null;
			if (flag)
			{
				this.avatarSkeleton.LegacyResetAllExtraParts();
			}
		}

		// Token: 0x0600B4DC RID: 46300 RVA: 0x0052525C File Offset: 0x0052345C
		private void ResetSize()
		{
			this.body.localScale = Vector3.one;
			this.body.anchoredPosition = Vector2.zero;
			bool flag = this.bodyCover != null;
			if (flag)
			{
				this.bodyCover.localScale = Vector3.one;
				this.bodyCover.anchoredPosition = Vector2.zero;
			}
			bool flag2 = this.avatarContainer != null;
			if (flag2)
			{
				RectTransform rectTransform = this.avatarContainer.GetComponent<RectTransform>();
				bool flag3 = rectTransform != null;
				if (flag3)
				{
					rectTransform.anchoredPosition = Vector2.zero;
					rectTransform.localScale = Vector3.one;
				}
			}
			bool flag4 = this.avatarSkeleton;
			if (flag4)
			{
				this.avatarSkeleton.LegacyPassBodyAnchoredPosition(this.body.anchoredPosition);
			}
		}

		// Token: 0x0600B4DD RID: 46301 RVA: 0x0052532C File Offset: 0x0052352C
		private void ResetAllExtraParts()
		{
			bool flag = this._outComeExtraPartMap != null;
			if (flag)
			{
				foreach (KeyValuePair<EAvatarExtraPartsType, PositionFollower> keyValuePair in this._outComeExtraPartMap)
				{
					EAvatarExtraPartsType eavatarExtraPartsType;
					PositionFollower positionFollower;
					keyValuePair.Deconstruct(out eavatarExtraPartsType, out positionFollower);
					PositionFollower follower = positionFollower;
					CImage image = follower.GetComponent<CImage>();
					image.enabled = false;
					image.sprite = null;
					image.color = Color.white;
					follower.enabled = false;
				}
			}
			bool flag2 = this._extraPartMap != null;
			if (flag2)
			{
				foreach (KeyValuePair<EAvatarElementsType, PositionFollower> keyValuePair2 in this._extraPartMap)
				{
					PositionFollower positionFollower;
					EAvatarElementsType eavatarElementsType;
					keyValuePair2.Deconstruct(out eavatarElementsType, out positionFollower);
					PositionFollower follower2 = positionFollower;
					CImage image2 = follower2.GetComponent<CImage>();
					image2.enabled = false;
					image2.sprite = null;
					image2.color = Color.white;
					follower2.enabled = false;
				}
			}
		}

		// Token: 0x0600B4DE RID: 46302 RVA: 0x00525460 File Offset: 0x00523660
		private void SyncAllExtraPartPositions()
		{
			bool flag = this._outComeExtraPartMap != null;
			if (flag)
			{
				foreach (KeyValuePair<EAvatarExtraPartsType, PositionFollower> keyValuePair in this._outComeExtraPartMap)
				{
					EAvatarExtraPartsType eavatarExtraPartsType;
					PositionFollower positionFollower;
					keyValuePair.Deconstruct(out eavatarExtraPartsType, out positionFollower);
					PositionFollower follower = positionFollower;
					bool flag2 = follower.enabled && follower.gameObject.activeSelf;
					if (flag2)
					{
						follower.Excute();
					}
				}
			}
			bool flag3 = this._extraPartMap != null;
			if (flag3)
			{
				foreach (KeyValuePair<EAvatarElementsType, PositionFollower> keyValuePair2 in this._extraPartMap)
				{
					PositionFollower positionFollower;
					EAvatarElementsType eavatarElementsType;
					keyValuePair2.Deconstruct(out eavatarElementsType, out positionFollower);
					PositionFollower follower2 = positionFollower;
					bool flag4 = follower2.enabled && follower2.gameObject.activeSelf;
					if (flag4)
					{
						follower2.Excute();
					}
				}
			}
		}

		// Token: 0x0600B4DF RID: 46303 RVA: 0x0052557C File Offset: 0x0052377C
		private static void SetImageShadowStrength(CImage image, float strengthValue)
		{
			bool flag = null == image || null == image.sprite;
			if (!flag)
			{
				Color c = image.color;
				c.r = Mathf.Lerp(c.r, 0f, strengthValue);
				c.g = Mathf.Lerp(c.g, 0f, strengthValue);
				c.b = Mathf.Lerp(c.b, 0f, strengthValue);
				image.color = c;
			}
		}

		// Token: 0x0600B4E0 RID: 46304 RVA: 0x00525600 File Offset: 0x00523800
		private void HandleExtraPart(short templateId, bool isOutcome, EAvatarElementsType elementType, EAvatarExtraPartsType extraPartType, bool needSyncToSkeleton = false)
		{
			AvatarExtraPartsItem extraPartItem = AvatarExtraParts.Instance.GetItem(templateId);
			bool flag = extraPartItem != null && extraPartItem.Type == EAvatarExtraPartsType.DuckHead;
			if (flag)
			{
				PositionFollower outcomeFollower;
				bool flag2 = isOutcome && this._outComeExtraPartMap != null && this._outComeExtraPartMap.TryGetValue(EAvatarExtraPartsType.DuckHead, out outcomeFollower);
				if (flag2)
				{
					outcomeFollower.gameObject.SetActive(false);
				}
				else
				{
					PositionFollower elementFollower;
					bool flag3 = !isOutcome && this._extraPartMap != null && this._extraPartMap.TryGetValue(elementType, out elementFollower);
					if (flag3)
					{
						elementFollower.gameObject.SetActive(false);
					}
				}
			}
			else
			{
				if (isOutcome)
				{
					if (this._outComeExtraPartMap == null)
					{
						this._outComeExtraPartMap = new Dictionary<EAvatarExtraPartsType, PositionFollower>();
					}
				}
				else if (this._extraPartMap == null)
				{
					this._extraPartMap = new Dictionary<EAvatarElementsType, PositionFollower>();
				}
				PositionFollower follower;
				bool existFlag = isOutcome ? this._outComeExtraPartMap.TryGetValue(extraPartType, out follower) : this._extraPartMap.TryGetValue(elementType, out follower);
				bool flag4 = extraPartItem == null;
				if (flag4)
				{
					bool flag5 = !existFlag;
					if (!flag5)
					{
						follower.gameObject.SetActive(false);
						follower.Target = null;
					}
				}
				else
				{
					bool flag6 = !existFlag;
					if (flag6)
					{
						GameObject followerObject = Object.Instantiate<GameObject>(this.extraPartTemplate.gameObject, this.extraPartTemplate.parent, false);
						follower = followerObject.GetComponent<PositionFollower>();
						if (isOutcome)
						{
							this._outComeExtraPartMap.Add(extraPartType, follower);
						}
						else
						{
							this._extraPartMap.Add(elementType, follower);
						}
					}
					CImage followerImage = follower.GetComponent<CImage>();
					followerImage.sprite = AvatarAtlasAssets.Instance.GetSprite(this.AvatarId, extraPartItem.Name, (sbyte)this.size);
					followerImage.SetNativeSize();
					followerImage.enabled = true;
					Transform followerTrans = follower.transform;
					Transform layerFollow = this.GetPartByName(extraPartItem.LayerFollow);
					bool flag7 = layerFollow != null;
					if (flag7)
					{
						bool wasInTargetParent = followerTrans.parent == layerFollow.parent;
						int followerIdx = followerTrans.GetSiblingIndex();
						int layerIdx = layerFollow.GetSiblingIndex();
						followerTrans.SetParent(layerFollow.parent, false);
						int targetIdx = layerIdx + (int)extraPartItem.LayerOffset;
						bool flag8 = wasInTargetParent && followerIdx < layerIdx;
						if (flag8)
						{
							targetIdx--;
						}
						bool flag9 = extraPartType == EAvatarExtraPartsType.Mask;
						if (flag9)
						{
							Transform jieqingMaskParticle = layerFollow.parent.Find("JieqingMaskHolder");
							bool flag10 = jieqingMaskParticle != null;
							if (flag10)
							{
								followerTrans.SetSiblingIndex(jieqingMaskParticle.transform.GetSiblingIndex());
							}
							else
							{
								followerTrans.SetSiblingIndex(targetIdx);
							}
						}
						else
						{
							followerTrans.SetSiblingIndex(targetIdx);
						}
					}
					followerTrans.localScale = Vector3.one;
					bool flag11 = null == followerImage.sprite;
					if (flag11)
					{
						follower.gameObject.SetActive(false);
					}
					else
					{
						bool flag12 = !string.IsNullOrEmpty(extraPartItem.PositionFollow);
						if (flag12)
						{
							follower.Target = Avatar.EnsureCenterFollowTransform(this.GetPartByName(extraPartItem.PositionFollow));
							float x = AvatarManagerUtils.FloatScaleBySize(extraPartItem.PositionOffset[0], this.size);
							float y = AvatarManagerUtils.FloatScaleBySize(extraPartItem.PositionOffset[1], this.size);
							follower.Offset = new Vector3(x, y, 0f);
							follower.Excute();
						}
						bool flag13 = !string.IsNullOrEmpty(extraPartItem.ColorFollow);
						if (flag13)
						{
							Transform colorFollowTrans = this.GetPartByName(extraPartItem.ColorFollow);
							bool flag14 = null != colorFollowTrans;
							if (flag14)
							{
								CImage colorFollowImage = colorFollowTrans.GetComponent<CImage>();
								bool flag15 = null != colorFollowImage;
								if (flag15)
								{
									follower.GetComponent<CImage>().color = colorFollowImage.color;
								}
							}
						}
						bool flag16 = !string.IsNullOrEmpty(extraPartItem.ScaleFollow);
						if (flag16)
						{
							Transform scaleFollowTrans = this.GetPartByName(extraPartItem.ScaleFollow);
							bool flag17 = null != scaleFollowTrans;
							if (flag17)
							{
								follower.transform.localScale = scaleFollowTrans.localScale;
							}
						}
						follower.enabled = true;
						follower.gameObject.SetActive(true);
						bool flag18 = needSyncToSkeleton && this.CommonAvatarPreferSkeleton && this.avatarSkeleton != null;
						if (flag18)
						{
							this.avatarSkeleton.LegacyAddExtraPart(extraPartType, templateId, this.AvatarId);
						}
					}
				}
			}
		}

		// Token: 0x0600B4E1 RID: 46305 RVA: 0x00525A28 File Offset: 0x00523C28
		private static Transform EnsureCenterFollowTransform(Transform parentTrans)
		{
			bool flag = !parentTrans;
			Transform result;
			if (flag)
			{
				result = parentTrans;
			}
			else
			{
				Transform centerTrans = parentTrans.Find("Center");
				bool flag2 = centerTrans;
				if (flag2)
				{
					result = centerTrans;
				}
				else
				{
					centerTrans = new GameObject("Center").transform;
					centerTrans.SetParent(parentTrans, false);
					RectTransform centerRectTrans = centerTrans.GetComponent<RectTransform>();
					bool flag3 = null == centerRectTrans;
					if (flag3)
					{
						centerRectTrans = centerTrans.gameObject.AddComponent<RectTransform>();
					}
					centerRectTrans.anchorMin = (centerRectTrans.anchorMax = Vector2.one * 0.5f);
					centerRectTrans.pivot = Vector2.one * 0.5f;
					centerRectTrans.anchoredPosition = Vector2.zero;
					centerTrans = centerRectTrans;
					result = centerTrans;
				}
			}
			return result;
		}

		// Token: 0x0600B4E2 RID: 46306 RVA: 0x00525AF0 File Offset: 0x00523CF0
		private Transform GetPartByName(string partName)
		{
			bool flag = string.IsNullOrEmpty(partName);
			Transform result;
			if (flag)
			{
				result = null;
			}
			else
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(partName);
				if (num <= 1588814372U)
				{
					if (num != 278137476U)
					{
						if (num != 1116344469U)
						{
							if (num == 1588814372U)
							{
								if (partName == "FrontHair")
								{
									return this.frontHair.transform;
								}
							}
						}
						else if (partName == "Body")
						{
							return this.body;
						}
					}
					else if (partName == "Beard_2")
					{
						return this.lowerBeard.transform;
					}
				}
				else if (num <= 1713534085U)
				{
					if (num != 1652762006U)
					{
						if (num == 1713534085U)
						{
							if (partName == "BackHairPosition")
							{
								return this.backHairPosition;
							}
						}
					}
					else if (partName == "EyesArea")
					{
						return this.eyesCenterPosition;
					}
				}
				else if (num != 2110600634U)
				{
					if (num == 2996251363U)
					{
						if (partName == "Head")
						{
							return this.head;
						}
					}
				}
				else if (partName == "BackHair")
				{
					return this.backHair.transform;
				}
				Debug.LogError("Unknown part name: " + partName);
				result = null;
			}
			return result;
		}

		// Token: 0x0600B4E3 RID: 46307 RVA: 0x00525C5C File Offset: 0x00523E5C
		private void RefreshAsBaby()
		{
			AvatarGroup babyAvatarGroup = this._manager.GetAvatarGroup(255);
			short clothId = this.Data.ClothDisplayId;
			Sprite clothSprite = this._manager.GetSprite((int)babyAvatarGroup.Id, EAvatarElementsType.Cloth, (sbyte)this.size, new short[]
			{
				clothId
			});
			bool flag = null != clothSprite;
			if (flag)
			{
				this.cloth.sprite = clothSprite;
				this.cloth.SetNativeSize();
				this.cloth.enabled = true;
			}
			else
			{
				this.cloth.enabled = false;
			}
			Sprite clothSkinSprite = this._manager.GetSprite((int)babyAvatarGroup.Id, EAvatarElementsType.ClothSkin, (sbyte)this.size, new short[]
			{
				clothId
			});
			AvatarAsset clothSkinAsset = babyAvatarGroup.Get(EAvatarElementsType.ClothSkin, new short[]
			{
				clothId
			});
			bool flag2 = null != clothSkinSprite && clothSkinAsset != null;
			if (flag2)
			{
				this.clothSkin.sprite = clothSkinSprite;
				this.clothSkin.SetNativeSize();
				this.clothSkin.enabled = true;
				this.clothSkin.color = AvatarSkinColors.Instance[this.Data.ColorSkinId].ColorHex.HexStringToColor();
				Vector2 headBodyOffset = new Vector2(AvatarManagerUtils.FloatScaleBySize(clothSkinAsset.Config.Offset[0], this.size), AvatarManagerUtils.FloatScaleBySize(clothSkinAsset.Config.Offset[1], this.size));
				this.head.anchoredPosition = headBodyOffset;
				this.body.anchoredPosition = Vector2.zero;
			}
			else
			{
				this.clothSkin.enabled = false;
			}
			AvatarAsset clothColorAsset = babyAvatarGroup.Get(EAvatarElementsType.ClothColor, new short[]
			{
				clothId
			});
			Sprite clothColorSprite = this._manager.GetSprite((int)babyAvatarGroup.Id, EAvatarElementsType.ClothColor, (sbyte)this.size, new short[]
			{
				clothId
			});
			bool flag3 = clothColorAsset != null && null != clothColorSprite;
			if (flag3)
			{
				this.clothColor.sprite = clothColorSprite;
				this.clothColor.SetNativeSize();
				this.clothColor.enabled = true;
				this.clothColor.color = AvatarClothColors.Instance[this.Data.ColorClothId].ColorHex.HexStringToColor();
			}
			else
			{
				this.clothColor.enabled = false;
			}
			AvatarAsset babyClothAsset = babyAvatarGroup.Get(EAvatarElementsType.Cloth, new short[]
			{
				clothId
			});
			bool hasCover = babyClothAsset != null && babyClothAsset.Config.HasCover;
			this.bodyCover.gameObject.SetActive(hasCover);
			bool flag4 = hasCover;
			if (flag4)
			{
				this.bodyCover.anchoredPosition = Vector2.zero;
				this.clothCover.sprite = this._manager.GetCoverSprite((int)babyAvatarGroup.Id, EAvatarElementsType.Cloth, (sbyte)this.size, new short[]
				{
					clothId
				});
				this.clothCover.SetNativeSize();
				Sprite clothColorCoverSprite = this._manager.GetCoverSprite((int)babyAvatarGroup.Id, EAvatarElementsType.ClothColor, (sbyte)this.size, new short[]
				{
					clothId
				});
				bool flag5 = null != clothColorCoverSprite;
				if (flag5)
				{
					this.clothColorCover.sprite = clothColorCoverSprite;
					this.clothColorCover.SetNativeSize();
					this.clothColorCover.color = AvatarClothColors.Instance[this.Data.ColorClothId].ColorHex.HexStringToColor();
				}
				Sprite clothSkinCoverSprite = this._manager.GetCoverSprite((int)babyAvatarGroup.Id, EAvatarElementsType.ClothSkin, (sbyte)this.size, new short[]
				{
					clothId
				});
				bool flag6 = null != clothSkinCoverSprite;
				if (flag6)
				{
					this.clothSkinCover.sprite = clothSkinCoverSprite;
					this.clothSkinCover.SetNativeSize();
					this.clothSkinCover.color = AvatarSkinColors.Instance[this.Data.ColorSkinId].ColorHex.HexStringToColor();
				}
			}
		}

		// Token: 0x0600B4E4 RID: 46308 RVA: 0x00526038 File Offset: 0x00524238
		private void RefreshAsChild()
		{
			AvatarGroup childAvatarGroup = this._manager.GetAvatarGroup((int)this.AvatarId);
			this.InternalUpdateCloth(this.Data.ClothDisplayId);
			this.UpdateHead();
			List<HairRes> hair2Res = childAvatarGroup.Hair2Res;
			bool flag = hair2Res != null && hair2Res.Count > 0;
			if (flag)
			{
				List<short> backHairIdList = childAvatarGroup.Hair2Res.ConvertAll<short>((HairRes e) => e.Id);
				short backHairId = backHairIdList[(int)this.Data.BackHairId % backHairIdList.Count];
				this.InternalUpdateBackHair(backHairId);
			}
			List<HairRes> hair1Res = childAvatarGroup.Hair1Res;
			bool flag2 = hair1Res != null && hair1Res.Count > 0;
			if (flag2)
			{
				List<short> frontHairIdList = childAvatarGroup.Hair1Res.ConvertAll<short>((HairRes e) => e.Id);
				short frontHairId = frontHairIdList[(int)this.Data.FrontHairId % frontHairIdList.Count];
				this.InternalUpdateFrontHair(frontHairId);
			}
			List<AvatarAsset> eyesRes = childAvatarGroup.EyesRes;
			bool flag3 = eyesRes != null && eyesRes.Count > 0;
			if (flag3)
			{
				int eyesResCount = childAvatarGroup.EyesGroup.Count;
				EyeRes eyeRes = childAvatarGroup.EyesGroup[(int)this.Data.EyesMainId % eyesResCount];
				this.InternalUpdateEyes(eyeRes.Id, eyeRes.LeftEye.SubId, eyeRes.RightEye.SubId);
			}
			List<AvatarAsset> eyeBrowRes = childAvatarGroup.EyeBrowRes;
			bool flag4 = eyeBrowRes != null && eyeBrowRes.Count > 0;
			if (flag4)
			{
				List<short> eyebrowIdList = childAvatarGroup.EyeBrowRes.ConvertAll<short>((AvatarAsset e) => e.Id);
				short eyebrowId = eyebrowIdList[(int)this.Data.EyebrowId % eyebrowIdList.Count];
				this.InternalUpdateEyebrows(eyebrowId);
			}
			List<MouthRes> mouthRes = childAvatarGroup.MouthRes;
			bool flag5 = mouthRes != null && mouthRes.Count > 0;
			if (flag5)
			{
				List<short> mouthIdList = childAvatarGroup.MouthRes.ConvertAll<short>((MouthRes e) => e.Id);
				short mouthId = mouthIdList[(int)this.Data.MouthId % mouthIdList.Count];
				this.InternalUpdateMouth(mouthId);
			}
			List<AvatarAsset> noseRes = childAvatarGroup.NoseRes;
			bool flag6 = noseRes != null && noseRes.Count > 0;
			if (flag6)
			{
				List<short> noseIdList = childAvatarGroup.NoseRes.ConvertAll<short>((AvatarAsset e) => e.Id);
				short noseId = noseIdList[(int)this.Data.NoseId % noseIdList.Count];
				this.InternalUpdateNose(noseId);
			}
			bool flag7 = this.avatarSkeleton != null;
			if (flag7)
			{
				this.avatarSkeleton.LegacyUpdateFeature1(this.feature1CenterOrLeft, this.feature1Right, this.feature1LeftEye, this.feature1RightEye);
				this.avatarSkeleton.LegacyUpdateFeature2(this.feature2CenterOrLeft, this.feature2Right, this.feature2LeftEye, this.feature2RightEye);
				this.avatarSkeleton.LegacyUpdateWrinkle(this.wrinkle1, this.wrinkle2, this.wrinkle3Left, this.wrinkle3Right);
				this.avatarSkeleton.BeardShown = new AvatarSkeleton.BeardShownConfiguration[]
				{
					new AvatarSkeleton.BeardShownConfiguration
					{
						Enable = false
					},
					new AvatarSkeleton.BeardShownConfiguration
					{
						Enable = false
					}
				};
				this.avatarSkeleton.SetNeedRefresh();
			}
			this.TryAddXiangshuInfectionSprite();
			this.TryAddHuanxinFaceSprite();
		}

		// Token: 0x0600B4E5 RID: 46309 RVA: 0x005263DB File Offset: 0x005245DB
		public void UpdateCloth()
		{
			this.InternalUpdateCloth(this.Data.ClothDisplayId);
		}

		// Token: 0x0600B4E6 RID: 46310 RVA: 0x005263F0 File Offset: 0x005245F0
		private void InternalUpdateCloth(short clothId)
		{
			AvatarElementPositionItem positionConfig = this.GetPositionConfig(true);
			Vector2 headBodyOffset = this.GetElementPosition(positionConfig.HeadBodyOffset);
			this.head.anchoredPosition = headBodyOffset;
			this.body.anchoredPosition = Vector2.zero;
			this.cloth.SetColor(Color.white);
			this.SetImage(this.cloth, EAvatarElementsType.Cloth, true, new short[]
			{
				clothId
			});
			bool flag = null == this.cloth.sprite;
			if (flag)
			{
				this.SetImage(this.cloth, EAvatarElementsType.Cloth, true, new short[1]);
			}
			this.SetClothEffect(clothId);
			this.clothColor.SetColor(Color.white);
			this.clothColor.color = AvatarClothColors.Instance[this.Data.ColorClothId].ColorHex.HexStringToColor();
			this.SetImage(this.clothColor, EAvatarElementsType.ClothColor, true, new short[]
			{
				clothId
			});
			bool flag2 = null == this.clothColor.sprite;
			if (flag2)
			{
				this.SetImage(this.clothColor, EAvatarElementsType.ClothColor, true, new short[1]);
			}
			this.clothSkin.SetColor(Color.white);
			this.clothSkin.color = AvatarSkinColors.Instance[this.Data.ColorSkinId].ColorHex.HexStringToColor();
			this.SetImage(this.clothSkin, EAvatarElementsType.ClothSkin, true, new short[]
			{
				clothId
			});
			bool flag3 = null == this.clothSkin.sprite;
			if (flag3)
			{
				this.SetImage(this.clothSkin, EAvatarElementsType.ClothSkin, true, new short[1]);
			}
			AvatarAsset clothAsset = this._manager.GetAsset((int)this.AvatarId, EAvatarElementsType.Cloth, new short[]
			{
				clothId
			});
			bool hasCover = clothAsset != null && clothAsset.Config.HasCover;
			this.bodyCover.gameObject.SetActive(hasCover);
			bool flag4 = hasCover;
			if (flag4)
			{
				this.bodyCover.anchoredPosition = Vector2.zero;
				this.clothCover.SetColor(Color.white);
				this.SetCoverImage(this.clothCover, EAvatarElementsType.Cloth, clothId);
				this.clothColorCover.SetColor(Color.white);
				this.clothColorCover.color = AvatarClothColors.Instance[this.Data.ColorClothId].ColorHex.HexStringToColor();
				this.SetCoverImage(this.clothColorCover, EAvatarElementsType.ClothColor, clothId);
				this.clothSkinCover.SetColor(Color.white);
				this.clothSkinCover.color = AvatarSkinColors.Instance[this.Data.ColorSkinId].ColorHex.HexStringToColor();
				this.SetCoverImage(this.clothSkinCover, EAvatarElementsType.ClothSkin, clothId);
			}
			bool flag5 = this.avatarSkeleton;
			if (flag5)
			{
				this.avatarSkeleton.LegacyPassBodyAnchoredPosition(this.body.anchoredPosition);
			}
		}

		// Token: 0x0600B4E7 RID: 46311 RVA: 0x005266D0 File Offset: 0x005248D0
		private void SetClothEffect(short clothId)
		{
			bool flag = this.size > AvatarSize.Big;
			if (flag)
			{
				this.FreeClothParticle();
			}
			else
			{
				AvatarAsset asset = this._manager.GetAsset((int)this.AvatarId, EAvatarElementsType.Cloth, new short[]
				{
					clothId
				});
				bool flag2 = asset == null;
				if (flag2)
				{
					this.FreeClothParticle();
				}
				else
				{
					AvatarElementsItem config = asset.Config;
					string clothEffect = config.ClothEffect;
					bool flag3 = clothEffect.IsNullOrEmpty();
					if (flag3)
					{
						this.FreeClothParticle();
					}
					else
					{
						AvatarEffectHelper helper = SingletonObject.getInstance<AvatarEffectHelper>();
						helper.GetParticle(base.gameObject, clothEffect, this.body);
					}
				}
			}
		}

		// Token: 0x0600B4E8 RID: 46312 RVA: 0x00526768 File Offset: 0x00524968
		public void UpdateHead()
		{
			this.headImage.SetColor(Color.white);
			byte headId = this.Data.HeadId;
			bool flag = this.AvatarId >= 251 && this.AvatarId <= 254;
			if (flag)
			{
				headId = 1;
			}
			this.SetImage(this.headImage, EAvatarElementsType.Head, true, new short[]
			{
				(short)headId
			});
			bool flag2 = !this.IsSkeleton;
			if (flag2)
			{
				this.headImage.color = AvatarSkinColors.Instance[this.Data.ColorSkinId].ColorHex.HexStringToColor();
				this.nose.color = this.headImage.color;
			}
			bool showBlush = this.Data.ShowBlush;
			if (showBlush)
			{
				this.AddExtraPartItem(EAvatarExtraPartsType.Blush, AvatarManagerUtils.GetAvatarBlushTemplateIdByAvatarId(this.AvatarId));
			}
			bool showJieqingMask = this.Data.ShowJieqingMask;
			if (showJieqingMask)
			{
				this.AddJieqingMaskPartItem(AvatarManagerUtils.GetAvatarJieqingMaskTemplateIdByAvatarId(this.AvatarId));
			}
			bool flag3 = this.Data.DarkAshStyle >= 0;
			if (flag3)
			{
				this.TryAddDashAshExtraPartItem();
			}
		}

		// Token: 0x0600B4E9 RID: 46313 RVA: 0x00526888 File Offset: 0x00524A88
		private bool CalcCanShowBackHair()
		{
			bool flag = this.AvatarId >= 251 && this.AvatarId <= 254;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				AvatarAsset frontHairAsset = this._manager.GetAsset((int)this.AvatarId, EAvatarElementsType.Hair1, new short[]
				{
					this.Data.FrontHairId
				});
				AvatarAsset backHairAsset = this._manager.GetAsset((int)this.AvatarId, EAvatarElementsType.Hair2, new short[]
				{
					this.Data.BackHairId
				});
				result = ((frontHairAsset.Config.BanElements == null || !Array.Exists<uint>(frontHairAsset.Config.BanElements, (uint e) => e == backHairAsset.Config.TemplateId)) && !frontHairAsset.Config.DisableRelativeType);
			}
			return result;
		}

		// Token: 0x1700148F RID: 5263
		// (get) Token: 0x0600B4EA RID: 46314 RVA: 0x0052695C File Offset: 0x00524B5C
		private bool HairShow
		{
			get
			{
				return this.Data.GetGrowableElementShowingState(0) && this.Data.GetGrowableElementShowingAbility(0);
			}
		}

		// Token: 0x0600B4EB RID: 46315 RVA: 0x0052697B File Offset: 0x00524B7B
		public void UpdateBackHair()
		{
			this.InternalUpdateBackHair(this.Data.BackHairId);
		}

		// Token: 0x0600B4EC RID: 46316 RVA: 0x00526990 File Offset: 0x00524B90
		private void InternalUpdateBackHair(short backHairId)
		{
			AvatarElementPositionItem positionConfig = this.GetPositionConfig(true);
			this.SetElementPosition(this.backHairPosition, positionConfig.BackHair);
			AvatarSkeleton avatarSkeleton = this.avatarSkeleton;
			if (avatarSkeleton != null)
			{
				avatarSkeleton.LegacyUpdateBackHairPosition(positionConfig.BackHair, this.size);
			}
			this.backHair.SetColor(Color.white);
			bool flag = this.backHairHighLight != null;
			if (flag)
			{
				this.backHairHighLight.SetColor(Color.white);
			}
			bool flag2 = backHairId != 1;
			if (flag2)
			{
				bool hairShow = this.CalcCanShowBackHair() && this.HairShow;
				bool flag3 = !hairShow;
				if (flag3)
				{
					backHairId = 1;
				}
			}
			this.backHair.color = (this.ShouldShowWhiteHair() ? Avatar.WhiteHairColor : AvatarHairColors.Instance[this.Data.ColorBackHairId].ColorHex.HexStringToColor());
			this.SetImage(this.backHair, EAvatarElementsType.Hair2, true, new short[]
			{
				backHairId
			});
			bool flag4 = this.backHairHighLight != null;
			if (flag4)
			{
				this.SetHairHighLightImage(this.backHairHighLight, EAvatarElementsType.Hair2, new short[]
				{
					backHairId
				});
			}
			bool flag5 = this.SetClothHatBackPart(this.backHair, new short[]
			{
				this.Data.ClothDisplayId
			});
			if (flag5)
			{
				this.backHairPart.sprite = null;
				this.backHairPart.enabled = false;
				this.backHairHighLight.sprite = null;
				this.backHairHighLight.enabled = false;
			}
			else
			{
				AvatarAsset backHairPartAsset = this._manager.GetAsset((int)this.AvatarId, EAvatarElementsType.Hair2Part, new short[]
				{
					backHairId
				});
				this.backHairPart.SetColor(Color.white);
				bool flag6 = backHairPartAsset == null;
				if (flag6)
				{
					this.backHairPart.sprite = null;
					this.backHairPart.enabled = false;
				}
				else
				{
					bool flag7 = backHairPartAsset.Config.ColorGroup > 0;
					if (flag7)
					{
						this.backHairPart.color = AvatarClothColors.Instance[this.Data.ColorClothId].ColorHex.HexStringToColor();
					}
					this.SetImage(this.backHairPart, EAvatarElementsType.Hair2Part, true, new short[]
					{
						backHairId
					});
					bool flag8 = this.avatarSkeleton;
					if (flag8)
					{
						this.avatarSkeleton.SetNeedRefresh();
					}
				}
			}
		}

		// Token: 0x0600B4ED RID: 46317 RVA: 0x00526BE6 File Offset: 0x00524DE6
		public void UpdateFrontHair()
		{
			this.InternalUpdateFrontHair(this.Data.FrontHairId);
		}

		// Token: 0x0600B4EE RID: 46318 RVA: 0x00526BFC File Offset: 0x00524DFC
		private void InternalUpdateFrontHair(short frontHairId)
		{
			AvatarElementPositionItem positionConfig = this.GetPositionConfig(true);
			this.SetElementPosition(this.frontHair.rectTransform, positionConfig.FrontHair);
			AvatarSkeleton avatarSkeleton = this.avatarSkeleton;
			if (avatarSkeleton != null)
			{
				avatarSkeleton.LegacyUpdateFrontHairPosition(positionConfig.FrontHair, this.size);
			}
			this.frontHair.SetColor(Color.white);
			bool flag = this.frontHairHighLight != null;
			if (flag)
			{
				this.frontHairHighLight.SetColor(Color.white);
			}
			bool hairShow = this.HairShow;
			bool flag2 = !hairShow;
			if (flag2)
			{
				frontHairId = 1;
			}
			this.frontHair.color = (this.ShouldShowWhiteHair() ? Avatar.WhiteHairColor : AvatarHairColors.Instance[this.Data.ColorFrontHairId].ColorHex.HexStringToColor());
			this.SetImage(this.frontHair, EAvatarElementsType.Hair1, true, new short[]
			{
				frontHairId
			});
			bool flag3 = this.frontHairHighLight != null;
			if (flag3)
			{
				this.SetHairHighLightImage(this.frontHairHighLight, EAvatarElementsType.Hair1, new short[]
				{
					frontHairId
				});
			}
			this.frontHairPart.SetColor(Color.white);
			AvatarAsset frontHairPartAsset = this._manager.GetAsset((int)this.AvatarId, EAvatarElementsType.Hair1Part, new short[]
			{
				frontHairId
			});
			bool flag4 = frontHairPartAsset == null;
			if (flag4)
			{
				this.frontHairPart.sprite = null;
				this.frontHairPart.enabled = false;
			}
			else
			{
				bool flag5 = frontHairPartAsset.Config.ColorGroup > 0;
				if (flag5)
				{
					this.frontHairPart.color = AvatarClothColors.Instance[this.Data.ColorClothId].ColorHex.HexStringToColor();
				}
				this.SetImage(this.frontHairPart, EAvatarElementsType.Hair1Part, true, new short[]
				{
					frontHairId
				});
				bool flag6 = this.avatarSkeleton;
				if (flag6)
				{
					this.avatarSkeleton.SetNeedRefresh();
				}
			}
		}

		// Token: 0x0600B4EF RID: 46319 RVA: 0x00526DE0 File Offset: 0x00524FE0
		private bool ShouldShowWhiteHair()
		{
			return !this.IgnoreAgeControlOfWhiteHair && this.AvatarAge >= GlobalConfig.Instance.AgeShowWhiteHair;
		}

		// Token: 0x0600B4F0 RID: 46320 RVA: 0x00526E14 File Offset: 0x00525014
		private void InternalCommonUpdate(Action process, Action migration)
		{
			bool commonAvatarPreferSkeleton = this.CommonAvatarPreferSkeleton;
			if (commonAvatarPreferSkeleton)
			{
				AvatarSize realSize = this.size;
				this.size = AvatarSize.Big;
				process();
				migration();
				this.size = realSize;
			}
			process();
		}

		// Token: 0x0600B4F1 RID: 46321 RVA: 0x00526E58 File Offset: 0x00525058
		public void UpdateEyes()
		{
			bool isSkeleton = this.IsSkeleton;
			if (!isSkeleton)
			{
				this.InternalUpdateEyes(this.Data.EyesMainId, this.Data.EyesLeftId, this.Data.EyesRightId);
			}
		}

		// Token: 0x0600B4F2 RID: 46322 RVA: 0x00526E9C File Offset: 0x0052509C
		private void InternalUpdateEyes(short eyesMainId, short eyesLeftId, short eyesRightId)
		{
			this.InternalCommonUpdate(delegate
			{
				AvatarElementPositionItem positionConfig = this.GetPositionConfig(true);
				Vector2 leftEyePos = this.GetElementPosition(positionConfig.LeftEye);
				Vector2 rightEyePos = this.GetElementPosition(positionConfig.RightEye);
				float yOffset = AvatarManagerUtils.FloatScaleBySize((float)this.Data.EyesHeight * 0.01f, this.size);
				float xOffset = AvatarManagerUtils.FloatScaleBySize((float)this.Data.EyesDistance * 0.01f, this.size);
				leftEyePos.y += yOffset;
				rightEyePos.y += yOffset;
				leftEyePos.x -= xOffset;
				rightEyePos.x += xOffset;
				this.leftEye.rectTransform.anchoredPosition = leftEyePos;
				this.rightEye.rectTransform.anchoredPosition = rightEyePos;
				this.eyesCenterPosition.anchoredPosition = (this.leftEye.rectTransform.anchoredPosition + this.rightEye.rectTransform.anchoredPosition) * 0.5f;
				this.leftEye.SetColor(Color.white);
				this.SetImage(this.leftEye, EAvatarElementsType.Eye, true, new short[]
				{
					eyesMainId
				});
				this.leftEyeball.SetColor(Color.white);
				this.leftEyeball.color = AvatarEyeballColors.Instance[this.Data.ColorEyeballId].ColorHex.HexStringToColor();
				this.SetImage(this.leftEyeball, EAvatarElementsType.EyeBall, true, new short[]
				{
					eyesMainId
				});
				this.rightEye.SetColor(Color.white);
				this.SetImage(this.rightEye, EAvatarElementsType.Eye, false, new short[]
				{
					eyesMainId
				});
				this.rightEyeball.SetColor(Color.white);
				this.rightEyeball.color = AvatarEyeballColors.Instance[this.Data.ColorEyeballId].ColorHex.HexStringToColor();
				this.SetImage(this.rightEyeball, EAvatarElementsType.EyeBall, false, new short[]
				{
					eyesMainId
				});
				Avatar.SetSizeWithScaleArg(this.leftEye, (float)this.Data.EyesScale * 0.01f);
				Avatar.SetSizeWithScaleArg(this.rightEye, (float)this.Data.EyesScale * 0.01f);
				Avatar.SetSizeWithScaleArg(this.leftEyeball, (float)this.Data.EyesScale * 0.01f);
				Avatar.SetSizeWithScaleArg(this.rightEyeball, (float)this.Data.EyesScale * 0.01f);
				this.leftEye.rectTransform.localEulerAngles = Vector3.back * ((float)this.Data.EyesAngle * 0.01f);
				this.rightEye.rectTransform.localEulerAngles = Vector3.forward * ((float)this.Data.EyesAngle * 0.01f);
			}, delegate
			{
				this.avatarSkeleton.LegacyUpdateEyes(this.leftEye, this.rightEye, this.leftEyeball, this.rightEyeball);
			});
		}

		// Token: 0x0600B4F3 RID: 46323 RVA: 0x00526EE0 File Offset: 0x005250E0
		public void UpdateEyebrows()
		{
			bool isSkeleton = this.IsSkeleton;
			if (!isSkeleton)
			{
				this.InternalUpdateEyebrows(this.Data.EyebrowId);
			}
		}

		// Token: 0x0600B4F4 RID: 46324 RVA: 0x00526F0C File Offset: 0x0052510C
		private void InternalUpdateEyebrows(short eyebrowId)
		{
			this.InternalCommonUpdate(delegate
			{
				AvatarElementPositionItem positionConfig = this.GetPositionConfig(true);
				Vector2 leftEyebrowPos = this.GetElementPosition(positionConfig.LeftBrow);
				Vector2 rightEyebrowPos = this.GetElementPosition(positionConfig.RightBrow);
				float yOffset = AvatarManagerUtils.FloatScaleBySize((float)this.Data.EyebrowHeight * 0.01f, this.size);
				float xOffset = AvatarManagerUtils.FloatScaleBySize((float)this.Data.EyebrowDistance * 0.01f, this.size);
				leftEyebrowPos.y += yOffset;
				rightEyebrowPos.y += yOffset;
				leftEyebrowPos.x -= xOffset;
				rightEyebrowPos.x += xOffset;
				this.leftEyebrow.rectTransform.anchoredPosition = leftEyebrowPos;
				this.rightEyebrow.rectTransform.anchoredPosition = rightEyebrowPos;
				bool eyebrowShow = (this.Data.GetGrowableElementShowingState(6) && this.Data.GetGrowableElementShowingAbility(6)) || this.AvatarAge < 16;
				bool flag = !eyebrowShow;
				if (flag)
				{
					eyebrowId = 0;
				}
				this.leftEyebrow.SetColor(Color.white);
				this.leftEyebrow.color = (this.ShouldShowWhiteHair() ? Avatar.WhiteHairColor : AvatarHairColors.Instance[this.Data.ColorEyebrowId].ColorHex.HexStringToColor());
				this.SetImage(this.leftEyebrow, EAvatarElementsType.EyeBrow, true, new short[]
				{
					eyebrowId
				});
				this.rightEyebrow.SetColor(Color.white);
				this.rightEyebrow.color = (this.ShouldShowWhiteHair() ? Avatar.WhiteHairColor : AvatarHairColors.Instance[this.Data.ColorEyebrowId].ColorHex.HexStringToColor());
				this.SetImage(this.rightEyebrow, EAvatarElementsType.EyeBrow, false, new short[]
				{
					eyebrowId
				});
				Avatar.SetSizeWithScaleArg(this.leftEyebrow, (float)this.Data.EyebrowScale * 0.01f);
				Avatar.SetSizeWithScaleArg(this.rightEyebrow, (float)this.Data.EyebrowScale * 0.01f);
				this.leftEyebrow.rectTransform.localEulerAngles = Vector3.back * ((float)this.Data.EyebrowAngle * 0.01f);
				this.rightEyebrow.rectTransform.localEulerAngles = Vector3.forward * ((float)this.Data.EyebrowAngle * 0.01f);
			}, delegate
			{
				this.avatarSkeleton.LegacyUpdateEyebrows(this.leftEyebrow, this.rightEyebrow);
			});
		}

		// Token: 0x0600B4F5 RID: 46325 RVA: 0x00526F50 File Offset: 0x00525150
		public void UpdateMouth()
		{
			bool isSkeleton = this.IsSkeleton;
			if (!isSkeleton)
			{
				this.InternalUpdateMouth(this.Data.MouthId);
			}
		}

		// Token: 0x0600B4F6 RID: 46326 RVA: 0x00526F7C File Offset: 0x0052517C
		private void InternalUpdateMouth(short mouthId)
		{
			this.InternalCommonUpdate(delegate
			{
				AvatarElementPositionItem positionConfig = this.GetPositionConfig(true);
				Vector2 mouthPos = this.GetElementPosition(positionConfig.Mouth);
				float yOffset = AvatarManagerUtils.FloatScaleBySize((float)this.Data.MouthHeight * 0.01f, this.size);
				mouthPos.y += yOffset;
				this.mouth.rectTransform.anchoredPosition = mouthPos;
				this.mouth.SetColor(Color.white);
				this.mouth.color = AvatarLipColors.Instance[this.Data.ColorMouthId].ColorHex.HexStringToColor();
				this.SetImage(this.mouth, EAvatarElementsType.Mouth, true, new short[]
				{
					mouthId
				});
				Avatar.SetSizeWithScaleArg(this.mouth, (float)this.Data.MouthScale * 0.01f);
				this.mouthPart.SetColor(Color.white);
				this.SetImage(this.mouthPart, EAvatarElementsType.MouthPart, true, new short[]
				{
					mouthId
				});
				Avatar.SetSizeWithScaleArg(this.mouthPart, (float)this.Data.MouthScale * 0.01f);
			}, delegate
			{
				this.avatarSkeleton.LegacyUpdateMouth(this.mouth, this.mouthPart);
			});
		}

		// Token: 0x0600B4F7 RID: 46327 RVA: 0x00526FC0 File Offset: 0x005251C0
		public void UpdateNose()
		{
			bool isSkeleton = this.IsSkeleton;
			if (!isSkeleton)
			{
				this.InternalUpdateNose(this.Data.NoseId);
			}
		}

		// Token: 0x0600B4F8 RID: 46328 RVA: 0x00526FEC File Offset: 0x005251EC
		private void InternalUpdateNose(short noseId)
		{
			this.InternalCommonUpdate(delegate
			{
				AvatarElementPositionItem positionConfig = this.GetPositionConfig(true);
				Vector2 nosePos = this.GetElementPosition(positionConfig.Nose);
				float yOffset = AvatarManagerUtils.FloatScaleBySize((float)this.Data.NoseHeight * 0.01f, this.size);
				nosePos.y += yOffset;
				this.nose.rectTransform.anchoredPosition = nosePos;
				this.nose.SetColor(Color.white);
				this.nose.color = AvatarSkinColors.Instance[this.Data.ColorSkinId].ColorHex.HexStringToColor();
				this.SetImage(this.nose, EAvatarElementsType.Nose, true, new short[]
				{
					noseId
				});
				Avatar.SetSizeWithScaleArg(this.nose, (float)this.Data.NoseScale * 0.01f);
			}, delegate
			{
				this.avatarSkeleton.LegacyUpdateNose(this.nose);
			});
		}

		// Token: 0x0600B4F9 RID: 46329 RVA: 0x00527030 File Offset: 0x00525230
		public void UpdateBeard()
		{
			bool beard1Show = this.Data.GetGrowableElementShowingAbility(1) && this.Data.GetGrowableElementShowingState(1);
			bool beard2Show = this.Data.GetGrowableElementShowingAbility(2) && this.Data.GetGrowableElementShowingState(2);
			bool shouldWhite = this.ShouldShowWhiteHair();
			float mouthBasedScale = (float)this.Data.MouthScale * 0.01f;
			this.InternalCommonUpdate(delegate
			{
				AvatarElementPositionItem positionConfig = this.GetPositionConfig(false);
				this.SetElementPosition(this.upperBeard.rectTransform, positionConfig.UpperBeard);
				this.SetElementPosition(this.lowerBeard.rectTransform, positionConfig.LowerBeard);
				this.upperBeard.SetColor(Color.white);
				bool flag = !beard1Show;
				if (flag)
				{
					this.upperBeard.sprite = null;
					this.upperBeard.enabled = false;
				}
				else
				{
					this.upperBeard.color = (shouldWhite ? Avatar.WhiteHairColor : AvatarHairColors.Instance[this.Data.ColorBeard1Id].ColorHex.HexStringToColor());
					this.SetImage(this.upperBeard, EAvatarElementsType.Beard1, true, new short[]
					{
						this.Data.Beard1Id
					});
					Avatar.SetSizeWithScaleArg(this.upperBeard, mouthBasedScale);
				}
				this.lowerBeard.SetColor(Color.white);
				bool flag2 = !beard2Show;
				if (flag2)
				{
					this.lowerBeard.sprite = null;
					this.lowerBeard.enabled = false;
				}
				else
				{
					this.lowerBeard.color = (shouldWhite ? Avatar.WhiteHairColor : AvatarHairColors.Instance[this.Data.ColorBeard2Id].ColorHex.HexStringToColor());
					this.SetImage(this.lowerBeard, EAvatarElementsType.Beard2, true, new short[]
					{
						this.Data.Beard2Id
					});
				}
			}, delegate
			{
				bool flag = this.avatarSkeleton;
				if (flag)
				{
					this.avatarSkeleton.BeardShown = new AvatarSkeleton.BeardShownConfiguration[]
					{
						new AvatarSkeleton.BeardShownConfiguration
						{
							Enable = beard1Show,
							AnchoredPosition = this.upperBeard.rectTransform.anchoredPosition,
							ShouldWhite = shouldWhite,
							Scale = mouthBasedScale
						},
						new AvatarSkeleton.BeardShownConfiguration
						{
							Enable = beard2Show,
							AnchoredPosition = this.lowerBeard.rectTransform.anchoredPosition,
							ShouldWhite = shouldWhite,
							Scale = 1f
						}
					};
					this.avatarSkeleton.SetNeedRefresh();
				}
			});
		}

		// Token: 0x0600B4FA RID: 46330 RVA: 0x005270D4 File Offset: 0x005252D4
		public void UpdateFeature()
		{
			bool isSkeleton = this.IsSkeleton;
			if (!isSkeleton)
			{
				this.InternalCommonUpdate(delegate
				{
					AvatarElementPositionItem positionConfig = this.GetPositionConfig(true);
					this.SetElementPosition(this.feature1CenterOrLeft.rectTransform, positionConfig.PositiveFeature);
					this.SetElementPosition(this.feature2CenterOrLeft.rectTransform, positionConfig.NegativeFeature);
					this.HandleUpdateFeature(this.Data.Feature1Id, this.Data.ColorFeature1Id, EAvatarElementsType.Feature1, this.feature1CenterOrLeft, this.feature1Right, this.feature1LeftEye, this.feature1RightEye);
					this.HandleUpdateFeature(this.Data.Feature2Id, this.Data.ColorFeature2Id, EAvatarElementsType.Feature2, this.feature2CenterOrLeft, this.feature2Right, this.feature2LeftEye, this.feature2RightEye);
				}, delegate
				{
					AvatarSkeleton avatarSkeleton = this.avatarSkeleton;
					if (avatarSkeleton != null)
					{
						avatarSkeleton.LegacyUpdateFeature1(this.feature1CenterOrLeft, this.feature1Right, this.feature1LeftEye, this.feature1RightEye);
					}
					AvatarSkeleton avatarSkeleton2 = this.avatarSkeleton;
					if (avatarSkeleton2 != null)
					{
						avatarSkeleton2.LegacyUpdateFeature2(this.feature2CenterOrLeft, this.feature2Right, this.feature2LeftEye, this.feature2RightEye);
					}
				});
			}
		}

		// Token: 0x0600B4FB RID: 46331 RVA: 0x00527110 File Offset: 0x00525310
		private void HandleUpdateFeature(short featureId, byte colorId, EAvatarElementsType elementsType, CImage featureCenterOrLeft, CImage featureRight, CImage featureLeftEye, CImage featureRightEye)
		{
			AvatarAsset featureAsset = this._manager.GetAsset((int)this.Data.AvatarId, elementsType, new short[]
			{
				featureId
			});
			bool flag = featureAsset == null;
			if (flag)
			{
				featureCenterOrLeft.sprite = null;
				featureCenterOrLeft.enabled = false;
				featureRight.sprite = null;
				featureRight.enabled = false;
				featureLeftEye.sprite = null;
				featureLeftEye.enabled = false;
				featureRightEye.sprite = null;
				featureRightEye.enabled = false;
			}
			else
			{
				Color featureColor = Color.white;
				bool flag2 = featureAsset.Config.ColorGroup > 0;
				if (flag2)
				{
					featureColor = AvatarFeatureColors.Instance[colorId].ColorHex.HexStringToColor();
				}
				bool shouldMirrorEyes = featureAsset.Config.ShouldMirrorEyes;
				if (shouldMirrorEyes)
				{
					featureCenterOrLeft.sprite = null;
					featureCenterOrLeft.enabled = false;
					featureRight.sprite = null;
					featureRight.enabled = false;
					featureLeftEye.color = featureColor;
					this.SetFeatureImage(featureLeftEye, elementsType, true, true, new short[]
					{
						featureId
					});
					Avatar.SetSizeWithScaleArg(featureLeftEye, (float)this.Data.EyesScale * 0.01f);
					featureRightEye.color = featureColor;
					this.SetFeatureImage(featureRightEye, elementsType, true, false, new short[]
					{
						featureId
					});
					Avatar.SetSizeWithScaleArg(featureRightEye, (float)this.Data.EyesScale * 0.01f);
					sbyte mirrorType = (elementsType == EAvatarElementsType.Feature1) ? this.Data.Feature1MirrorType : this.Data.Feature2MirrorType;
					featureLeftEye.gameObject.SetActive(mirrorType == 0 || mirrorType == 2);
					featureRightEye.gameObject.SetActive(mirrorType == 1 || mirrorType == 2);
				}
				else
				{
					bool canMirror = featureAsset.Config.CanMirror;
					if (canMirror)
					{
						featureLeftEye.sprite = null;
						featureLeftEye.enabled = false;
						featureRightEye.sprite = null;
						featureRightEye.enabled = false;
						featureCenterOrLeft.color = featureColor;
						this.SetFeatureImage(featureCenterOrLeft, elementsType, true, true, new short[]
						{
							featureId
						});
						featureRight.color = featureColor;
						this.SetFeatureImage(featureRight, elementsType, true, false, new short[]
						{
							featureId
						});
						sbyte mirrorType2 = (elementsType == EAvatarElementsType.Feature1) ? this.Data.Feature1MirrorType : this.Data.Feature2MirrorType;
						featureCenterOrLeft.gameObject.SetActive(mirrorType2 == 0 || mirrorType2 == 2);
						featureRight.gameObject.SetActive(mirrorType2 == 1 || mirrorType2 == 2);
					}
					else
					{
						featureRight.gameObject.SetActive(false);
						featureLeftEye.sprite = null;
						featureLeftEye.enabled = false;
						featureRightEye.sprite = null;
						featureRightEye.enabled = false;
						featureCenterOrLeft.gameObject.SetActive(true);
						featureCenterOrLeft.color = featureColor;
						this.SetImage(featureCenterOrLeft, elementsType, true, new short[]
						{
							featureId
						});
					}
				}
			}
		}

		// Token: 0x0600B4FC RID: 46332 RVA: 0x005273F8 File Offset: 0x005255F8
		public void UpdateFeatureMirrorType(EAvatarElementsType elementsType, sbyte mirrorType)
		{
			bool flag = elementsType == EAvatarElementsType.Feature1;
			if (flag)
			{
				this.Data.Feature1MirrorType = mirrorType;
			}
			bool flag2 = elementsType == EAvatarElementsType.Feature2;
			if (flag2)
			{
				this.Data.Feature2MirrorType = mirrorType;
			}
		}

		// Token: 0x0600B4FD RID: 46333 RVA: 0x00527430 File Offset: 0x00525630
		private static int GetWrinkle3ConfigIndex(byte avatarId)
		{
			bool flag = avatarId >= 1 && avatarId <= 6;
			int result;
			if (flag)
			{
				result = (int)(avatarId - 1);
			}
			else
			{
				bool flag2 = avatarId >= 251 && avatarId <= 254;
				if (flag2)
				{
					result = (int)(avatarId - 245);
				}
				else
				{
					result = -1;
				}
			}
			return result;
		}

		// Token: 0x0600B4FE RID: 46334 RVA: 0x00527480 File Offset: 0x00525680
		public void UpdateWrinkle()
		{
			bool isSkeleton = this.IsSkeleton;
			if (!isSkeleton)
			{
				this.InternalCommonUpdate(delegate
				{
					this.wrinkle1.SetColor(Color.white);
					bool showWrinkle = this.Data.GetGrowableElementShowingState(3) && this.Data.GetGrowableElementShowingAbility(3);
					bool flag = !showWrinkle;
					if (flag)
					{
						this.wrinkle1.sprite = null;
						this.wrinkle1.enabled = false;
					}
					else
					{
						this.SetImage(this.wrinkle1, EAvatarElementsType.Wrinkle1, true, new short[]
						{
							this.Data.Wrinkle1Id
						});
					}
					this.wrinkle2.SetColor(Color.white);
					bool showWrinkle2 = this.Data.GetGrowableElementShowingState(4) && this.Data.GetGrowableElementShowingAbility(4);
					bool flag2 = !showWrinkle2;
					if (flag2)
					{
						this.wrinkle2.sprite = null;
						this.wrinkle2.enabled = false;
					}
					else
					{
						this.SetImage(this.wrinkle2, EAvatarElementsType.Wrinkle2, true, new short[]
						{
							this.Data.Wrinkle2Id
						});
					}
					this.wrinkle3Left.SetColor(Color.white);
					this.wrinkle3Right.SetColor(Color.white);
					bool showWrinkle3 = this.Data.GetGrowableElementShowingState(5) && this.Data.GetGrowableElementShowingAbility(5);
					bool flag3 = !showWrinkle3;
					if (flag3)
					{
						this.wrinkle3Left.sprite = null;
						this.wrinkle3Left.enabled = false;
						this.wrinkle3Right.sprite = null;
						this.wrinkle3Right.enabled = false;
					}
					else
					{
						this.SetImage(this.wrinkle3Left, EAvatarElementsType.Wrinkle3, true, new short[]
						{
							this.Data.Wrinkle3Id
						});
						this.SetImage(this.wrinkle3Right, EAvatarElementsType.Wrinkle3, false, new short[]
						{
							this.Data.Wrinkle3Id
						});
						Avatar.SetSizeWithScaleArg(this.wrinkle3Left, (float)this.Data.EyesScale * 0.01f);
						Avatar.SetSizeWithScaleArg(this.wrinkle3Right, (float)this.Data.EyesScale * 0.01f);
						int configIndex = Avatar.GetWrinkle3ConfigIndex(this.Data.AvatarId);
						bool flag4 = configIndex >= 0;
						if (flag4)
						{
							float yOffset = Avatar.Wrinkle3YOffsets[configIndex, (int)this.size];
							this.wrinkle3Left.rectTransform.anchoredPosition = new Vector2(this.wrinkle3Left.rectTransform.anchoredPosition.x, yOffset);
							this.wrinkle3Right.rectTransform.anchoredPosition = new Vector2(this.wrinkle3Right.rectTransform.anchoredPosition.x, yOffset);
						}
					}
				}, delegate
				{
					this.avatarSkeleton.LegacyUpdateWrinkle(this.wrinkle1, this.wrinkle2, this.wrinkle3Left, this.wrinkle3Right);
				});
			}
		}

		// Token: 0x0600B4FF RID: 46335 RVA: 0x005274BC File Offset: 0x005256BC
		private void FreeClothParticle()
		{
			bool flag = this.size > AvatarSize.Big;
			if (!flag)
			{
				AvatarEffectHelper helper = SingletonObject.getInstance<AvatarEffectHelper>();
				for (int i = 0; i < this.body.childCount; i++)
				{
					Transform child = this.body.GetChild(i);
					bool flag2 = child.GetComponent<UIParticle>() != null && helper.IsAvatarEffect(child.name);
					if (flag2)
					{
						helper.ReturnParticle(base.gameObject, child.name);
					}
				}
			}
		}

		// Token: 0x0600B500 RID: 46336 RVA: 0x00527544 File Offset: 0x00525744
		private AvatarElementPositionItem GetPositionConfig(bool considerChild = false)
		{
			int num2;
			if (considerChild)
			{
				byte avatarId = this.AvatarId;
				if (!true)
				{
				}
				int num;
				switch (avatarId)
				{
				case 251:
					num = 6;
					break;
				case 252:
					num = 7;
					break;
				case 253:
					num = 8;
					break;
				case 254:
					num = 9;
					break;
				default:
					num = (int)(this.Data.AvatarId - 1);
					break;
				}
				if (!true)
				{
				}
				num2 = num;
			}
			else
			{
				num2 = (int)(this.Data.AvatarId - 1);
			}
			int index = num2;
			return AvatarElementPosition.Instance[index];
		}

		// Token: 0x0600B501 RID: 46337 RVA: 0x005275C8 File Offset: 0x005257C8
		private bool IsCurrentSpine(string spineName, string skinName)
		{
			return string.Equals(this._currentSpineName, spineName, StringComparison.Ordinal) && string.Equals(this._currentSpineSkin, skinName ?? string.Empty, StringComparison.Ordinal);
		}

		// Token: 0x0600B502 RID: 46338 RVA: 0x00527604 File Offset: 0x00525804
		private void SetCoverImage(CImage image, EAvatarElementsType elemType, short clothId)
		{
			image.sprite = this._manager.GetCoverSprite((int)this.AvatarId, elemType, (sbyte)this.size, new short[]
			{
				clothId
			});
			image.SetNativeSize();
			image.enabled = image.sprite;
		}

		// Token: 0x0600B503 RID: 46339 RVA: 0x00527658 File Offset: 0x00525858
		private void SetImage(CImage image, EAvatarElementsType elemType, bool checkExtraPart, params short[] ids)
		{
			image.sprite = this._manager.GetSprite((int)this.AvatarId, elemType, (sbyte)this.size, ids);
			image.SetNativeSize();
			image.enabled = image.sprite;
			bool flag = !checkExtraPart;
			if (!flag)
			{
				AvatarAsset asset = this._manager.GetAsset((int)this.AvatarId, elemType, ids);
				bool flag2 = asset == null;
				if (!flag2)
				{
					short id = (elemType == EAvatarElementsType.Head) ? asset.HeadConfig.RelativeExtraPart : asset.Config.RelativeExtraPart;
					this.HandleExtraPart(id, false, elemType, EAvatarExtraPartsType.Count, true);
				}
			}
		}

		// Token: 0x0600B504 RID: 46340 RVA: 0x005276F4 File Offset: 0x005258F4
		private void SetHairHighLightImage(CImage image, EAvatarElementsType hairElementType, params short[] ids)
		{
			Sprite sprite = this._manager.GetHighLightForHair((int)this.AvatarId, hairElementType, (sbyte)this.size, ids);
			image.enabled = sprite;
			bool flag = !sprite;
			if (!flag)
			{
				image.sprite = sprite;
				image.SetNativeSize();
			}
		}

		// Token: 0x0600B505 RID: 46341 RVA: 0x00527748 File Offset: 0x00525948
		private bool SetClothHatBackPart(CImage image, params short[] ids)
		{
			AvatarAsset clothAsset = this._manager.GetAsset((int)this.AvatarId, EAvatarElementsType.Cloth, new short[]
			{
				this.Data.ClothDisplayId
			});
			bool flag = !string.IsNullOrEmpty(clothAsset.Config.HatBack);
			if (flag)
			{
				Sprite sprite = this._manager.GetClothHatBackPart((int)this.AvatarId, (sbyte)this.size, ids);
				bool flag2 = sprite != null;
				if (flag2)
				{
					image.enabled = true;
					image.sprite = sprite;
					image.SetNativeSize();
					image.SetColor(Color.white);
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600B506 RID: 46342 RVA: 0x005277EC File Offset: 0x005259EC
		private static void SetSizeWithScaleArg(CImage img, float scale)
		{
			bool flag = !img.sprite;
			if (!flag)
			{
				float baseWidth = img.sprite.rect.width;
				float baseHeight = img.sprite.rect.height;
				RectTransform rectImg = img.rectTransform;
				rectImg.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, baseWidth * scale);
				rectImg.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, baseHeight * scale);
			}
		}

		// Token: 0x0600B507 RID: 46343 RVA: 0x00527858 File Offset: 0x00525A58
		private void SetFeatureImage(CImage image, EAvatarElementsType elemType, bool checkExtraPart, bool isLeft, params short[] ids)
		{
			Tester.Assert(elemType == EAvatarElementsType.Feature1 || elemType == EAvatarElementsType.Feature2, "");
			image.sprite = this._manager.GetFeatureSprite((int)this.AvatarId, elemType, (sbyte)this.size, isLeft, ids);
			image.SetNativeSize();
			image.enabled = image.sprite;
			bool flag = !checkExtraPart;
			if (!flag)
			{
				AvatarAsset asset = this._manager.GetAsset((int)this.AvatarId, elemType, ids);
				bool flag2 = asset == null;
				if (!flag2)
				{
					short id = (elemType == EAvatarElementsType.Head) ? asset.HeadConfig.RelativeExtraPart : asset.Config.RelativeExtraPart;
					this.HandleExtraPart(id, false, elemType, EAvatarExtraPartsType.Count, true);
				}
			}
		}

		// Token: 0x0600B508 RID: 46344 RVA: 0x00527910 File Offset: 0x00525B10
		private Vector2 GetElementPosition(float[] positionConfig)
		{
			bool flag = positionConfig == null || positionConfig.Length < 2;
			Vector2 result;
			if (flag)
			{
				result = Vector2.zero;
			}
			else
			{
				float x = AvatarManagerUtils.FloatScaleBySize(positionConfig[0], this.size);
				float y = AvatarManagerUtils.FloatScaleBySize(positionConfig[1], this.size);
				result = new Vector2(x, y);
			}
			return result;
		}

		// Token: 0x0600B509 RID: 46345 RVA: 0x0052795F File Offset: 0x00525B5F
		private void SetElementPosition(RectTransform rectTransform, float[] positionConfig)
		{
			rectTransform.anchoredPosition = this.GetElementPosition(positionConfig);
		}

		// Token: 0x0600B50A RID: 46346 RVA: 0x00527970 File Offset: 0x00525B70
		private void ApplyFacePartVisibility()
		{
			bool flag = this.FacePartVisibility == EFacePartVisibility.All;
			if (!flag)
			{
				RectTransform rectTransform = this.body;
				if (rectTransform != null)
				{
					rectTransform.gameObject.SetActive(false);
				}
				RectTransform rectTransform2 = this.bodyCover;
				if (rectTransform2 != null)
				{
					rectTransform2.gameObject.SetActive(false);
				}
				CImage cimage = this.backHair;
				if (cimage != null)
				{
					cimage.gameObject.SetActive(false);
				}
				CImage cimage2 = this.frontHair;
				if (cimage2 != null)
				{
					cimage2.gameObject.SetActive(false);
				}
				CImage cimage3 = this.upperBeard;
				if (cimage3 != null)
				{
					cimage3.gameObject.SetActive(false);
				}
				CImage cimage4 = this.lowerBeard;
				if (cimage4 != null)
				{
					cimage4.gameObject.SetActive(false);
				}
				CImage cimage5 = this.feature1CenterOrLeft;
				if (cimage5 != null)
				{
					cimage5.gameObject.SetActive(false);
				}
				CImage cimage6 = this.feature1Right;
				if (cimage6 != null)
				{
					cimage6.gameObject.SetActive(false);
				}
				CImage cimage7 = this.feature1LeftEye;
				if (cimage7 != null)
				{
					cimage7.gameObject.SetActive(false);
				}
				CImage cimage8 = this.feature1RightEye;
				if (cimage8 != null)
				{
					cimage8.gameObject.SetActive(false);
				}
				CImage cimage9 = this.feature2CenterOrLeft;
				if (cimage9 != null)
				{
					cimage9.gameObject.SetActive(false);
				}
				CImage cimage10 = this.feature2Right;
				if (cimage10 != null)
				{
					cimage10.gameObject.SetActive(false);
				}
				CImage cimage11 = this.feature2LeftEye;
				if (cimage11 != null)
				{
					cimage11.gameObject.SetActive(false);
				}
				CImage cimage12 = this.feature2RightEye;
				if (cimage12 != null)
				{
					cimage12.gameObject.SetActive(false);
				}
				CImage cimage13 = this.wrinkle1;
				if (cimage13 != null)
				{
					cimage13.gameObject.SetActive(false);
				}
				CImage cimage14 = this.wrinkle2;
				if (cimage14 != null)
				{
					cimage14.gameObject.SetActive(false);
				}
				bool showEyebrow = this.FacePartVisibility == EFacePartVisibility.Eyebrow;
				bool showEyes = this.FacePartVisibility == EFacePartVisibility.Eyes;
				bool showNose = this.FacePartVisibility == EFacePartVisibility.Nose;
				bool showMouth = this.FacePartVisibility == EFacePartVisibility.Mouth;
				CImage cimage15 = this.leftEyebrow;
				if (cimage15 != null)
				{
					cimage15.gameObject.SetActive(showEyebrow);
				}
				CImage cimage16 = this.rightEyebrow;
				if (cimage16 != null)
				{
					cimage16.gameObject.SetActive(showEyebrow);
				}
				CImage cimage17 = this.leftEye;
				if (cimage17 != null)
				{
					cimage17.gameObject.SetActive(showEyes);
				}
				CImage cimage18 = this.rightEye;
				if (cimage18 != null)
				{
					cimage18.gameObject.SetActive(showEyes);
				}
				CImage cimage19 = this.nose;
				if (cimage19 != null)
				{
					cimage19.gameObject.SetActive(showNose);
				}
				CImage cimage20 = this.mouth;
				if (cimage20 != null)
				{
					cimage20.gameObject.SetActive(showMouth);
				}
				CImage cimage21 = this.headImage;
				if (cimage21 != null)
				{
					cimage21.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x04008CC6 RID: 36038
		[SerializeField]
		private AvatarSkeleton avatarSkeleton;

		// Token: 0x04008CC7 RID: 36039
		[Header("Containers")]
		[SerializeField]
		private GameObject avatarContainer;

		// Token: 0x04008CC8 RID: 36040
		[SerializeField]
		private GameObject gravestoneContainer;

		// Token: 0x04008CC9 RID: 36041
		[SerializeField]
		private CImage gravestoneImage;

		// Token: 0x04008CCA RID: 36042
		[Header("PositionOrderObjects")]
		[SerializeField]
		private RectTransform body;

		// Token: 0x04008CCB RID: 36043
		[SerializeField]
		private RectTransform bodyCover;

		// Token: 0x04008CCC RID: 36044
		[SerializeField]
		private RectTransform head;

		// Token: 0x04008CCD RID: 36045
		[Header("Body Parts")]
		[SerializeField]
		private CImage cloth;

		// Token: 0x04008CCE RID: 36046
		[SerializeField]
		private CImage clothColor;

		// Token: 0x04008CCF RID: 36047
		[SerializeField]
		private CImage clothSkin;

		// Token: 0x04008CD0 RID: 36048
		[Header("Body Cover Parts")]
		[SerializeField]
		private CImage clothCover;

		// Token: 0x04008CD1 RID: 36049
		[SerializeField]
		private CImage clothColorCover;

		// Token: 0x04008CD2 RID: 36050
		[SerializeField]
		private CImage clothSkinCover;

		// Token: 0x04008CD3 RID: 36051
		[Header("Head Parts")]
		[SerializeField]
		private CImage headImage;

		// Token: 0x04008CD4 RID: 36052
		[SerializeField]
		private CImage backHair;

		// Token: 0x04008CD5 RID: 36053
		[SerializeField]
		private CImage backHairPart;

		// Token: 0x04008CD6 RID: 36054
		[SerializeField]
		private CImage frontHair;

		// Token: 0x04008CD7 RID: 36055
		[SerializeField]
		private RectTransform backHairPosition;

		// Token: 0x04008CD8 RID: 36056
		[SerializeField]
		private CImage frontHairPart;

		// Token: 0x04008CD9 RID: 36057
		[SerializeField]
		private CImage mouth;

		// Token: 0x04008CDA RID: 36058
		[SerializeField]
		private CImage mouthPart;

		// Token: 0x04008CDB RID: 36059
		[SerializeField]
		private CImage upperBeard;

		// Token: 0x04008CDC RID: 36060
		[SerializeField]
		private CImage lowerBeard;

		// Token: 0x04008CDD RID: 36061
		[SerializeField]
		private CImage nose;

		// Token: 0x04008CDE RID: 36062
		[SerializeField]
		private CImage leftEyebrow;

		// Token: 0x04008CDF RID: 36063
		[SerializeField]
		private CImage rightEyebrow;

		// Token: 0x04008CE0 RID: 36064
		[SerializeField]
		private CImage leftEye;

		// Token: 0x04008CE1 RID: 36065
		[SerializeField]
		private CImage rightEye;

		// Token: 0x04008CE2 RID: 36066
		[SerializeField]
		private CImage leftEyeball;

		// Token: 0x04008CE3 RID: 36067
		[SerializeField]
		private CImage rightEyeball;

		// Token: 0x04008CE4 RID: 36068
		[SerializeField]
		private CImage feature1CenterOrLeft;

		// Token: 0x04008CE5 RID: 36069
		[SerializeField]
		private CImage feature1Right;

		// Token: 0x04008CE6 RID: 36070
		[SerializeField]
		private CImage feature1LeftEye;

		// Token: 0x04008CE7 RID: 36071
		[SerializeField]
		private CImage feature1RightEye;

		// Token: 0x04008CE8 RID: 36072
		[SerializeField]
		private CImage feature2CenterOrLeft;

		// Token: 0x04008CE9 RID: 36073
		[SerializeField]
		private CImage feature2Right;

		// Token: 0x04008CEA RID: 36074
		[SerializeField]
		private CImage feature2LeftEye;

		// Token: 0x04008CEB RID: 36075
		[SerializeField]
		private CImage feature2RightEye;

		// Token: 0x04008CEC RID: 36076
		[SerializeField]
		private CImage wrinkle1;

		// Token: 0x04008CED RID: 36077
		[SerializeField]
		private CImage wrinkle2;

		// Token: 0x04008CEE RID: 36078
		[SerializeField]
		private CImage wrinkle3Left;

		// Token: 0x04008CEF RID: 36079
		[SerializeField]
		private CImage wrinkle3Right;

		// Token: 0x04008CF0 RID: 36080
		[SerializeField]
		private CImage frontHairHighLight;

		// Token: 0x04008CF1 RID: 36081
		[SerializeField]
		private CImage backHairHighLight;

		// Token: 0x04008CF2 RID: 36082
		[Header("Follow Targets")]
		[SerializeField]
		private RectTransform eyesCenterPosition;

		// Token: 0x04008CF3 RID: 36083
		[Header("Other")]
		[SerializeField]
		private RectTransform extraPartTemplate;

		// Token: 0x04008CF4 RID: 36084
		[SerializeField]
		private SkeletonGraphic npcSkeleton;

		// Token: 0x04008CF5 RID: 36085
		[Header("XiangshuInfection")]
		[SerializeField]
		private CImage xiangshuInfectionImage;

		// Token: 0x04008CF6 RID: 36086
		[Header("HuanxinFace")]
		[SerializeField]
		private CImage huanxinFaceImage;

		// Token: 0x04008CF7 RID: 36087
		[SerializeField]
		private bool disableXinNian;

		// Token: 0x04008CF8 RID: 36088
		[Header("Options")]
		[SerializeField]
		public AvatarSize size = AvatarSize.Normal;

		// Token: 0x04008CF9 RID: 36089
		[SerializeField]
		private bool preferDynamicAvatar;

		// Token: 0x04008CFA RID: 36090
		[Header("Offset Settings")]
		[SerializeField]
		public int FixedAvatarOffsetSetIndex = 0;

		// Token: 0x04008CFB RID: 36091
		[SerializeField]
		public int NormalAvatarOffsetSetIndex = 0;

		// Token: 0x04008CFC RID: 36092
		[Header("Outer Reference")]
		[SerializeField]
		public RectTransform FaceRectIfNpc;

		// Token: 0x04008D04 RID: 36100
		private AvatarManager _manager;

		// Token: 0x04008D05 RID: 36101
		private static Sprite _graveSprite;

		// Token: 0x04008D06 RID: 36102
		private Dictionary<EAvatarElementsType, PositionFollower> _extraPartMap;

		// Token: 0x04008D07 RID: 36103
		private Dictionary<EAvatarExtraPartsType, PositionFollower> _outComeExtraPartMap;

		// Token: 0x04008D08 RID: 36104
		private string _currentSpineName;

		// Token: 0x04008D09 RID: 36105
		private string _currentSpineSkin;

		// Token: 0x04008D0A RID: 36106
		private uint _spineRequestVersion;

		// Token: 0x04008D0B RID: 36107
		private const int ChildAvatarIdMin = 251;

		// Token: 0x04008D0C RID: 36108
		private const int ChildAvatarIdMax = 254;

		// Token: 0x04008D0D RID: 36109
		private bool _usingDynamicAvatar;

		// Token: 0x04008D0E RID: 36110
		private static readonly string[] HuanxinFaceStaticSpriteNames = new string[]
		{
			"NpcFace_lanfahuanxinxinnianzhengyan",
			"NpcFace_lanfahuanxinxinnian",
			"NpcFace_baifahuanxinxinnianzhengyan",
			"NpcFace_baifahuanxinxinnian",
			"NpcFace_hongfahuanxinxinnianzhengyan",
			"NpcFace_hongfahuanxinxinnian"
		};

		// Token: 0x04008D0F RID: 36111
		private static readonly float[,] Wrinkle3YOffsets = new float[,]
		{
			{
				8.5f,
				4f,
				1.7f
			},
			{
				3.5f,
				1.5f,
				0.5f
			},
			{
				4f,
				1.5f,
				0.5f
			},
			{
				4f,
				2f,
				1f
			},
			{
				1.5f,
				0.5f,
				0f
			},
			{
				3.5f,
				1.5f,
				0.5f
			},
			{
				0.5f,
				0.5f,
				-0.5f
			},
			{
				-1f,
				-1f,
				-1f
			},
			{
				-1.5f,
				-0.5f,
				-0.5f
			},
			{
				2.5f,
				1.5f,
				0f
			}
		};
	}
}
