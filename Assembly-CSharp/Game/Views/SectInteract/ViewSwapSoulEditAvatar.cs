using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.NewGame;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using GameData.Domains.Global.Inscription;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009AD RID: 2477
	public class ViewSwapSoulEditAvatar : UIBase, IAvatarSubPageParent
	{
		// Token: 0x060077E2 RID: 30690 RVA: 0x0037D1BD File Offset: 0x0037B3BD
		private void Awake()
		{
			this.InitializeUI();
			this.primaryToggleGroup.Set(0, true);
		}

		// Token: 0x060077E3 RID: 30691 RVA: 0x0037D1D8 File Offset: 0x0037B3D8
		private void InitializeUI()
		{
			this.primaryToggleGroup.Init(-1);
			this.primaryToggleGroup.OnActiveIndexChange += this.OnPrimaryPageChanged;
			this.secondaryToggleGroupHelper.Init();
			this.secondaryToggleGroupHelper.AddOnActiveIndexChangeListener(new Action<int, int>(this.OnSecondaryPageChanged));
			this.InitializeSubPages();
		}

		// Token: 0x060077E4 RID: 30692 RVA: 0x0037D238 File Offset: 0x0037B438
		private void InitializeSubPages()
		{
			this.bodySkinPage.Init(this);
			this.hairPage.Init(this);
			this.beardPage.Init(this);
			this.facePartPage.Init(this);
			this.featurePage.Init(this);
		}

		// Token: 0x060077E5 RID: 30693 RVA: 0x0037D287 File Offset: 0x0037B487
		private void OnSecondaryPageChanged(int newIndex, int oldIndex)
		{
			this.UpdateSubPageVisibility();
		}

		// Token: 0x060077E6 RID: 30694 RVA: 0x0037D294 File Offset: 0x0037B494
		private void OnPrimaryPageChanged(int newIndex, int oldIndex)
		{
			bool flag = this.secondaryToggleGroupHelper != null;
			if (flag)
			{
				List<AvatarSecondaryToggleConfig> configs = this.GetSecondaryToggleConfigs(newIndex);
				this.secondaryToggleGroupHelper.Refresh(configs, true);
			}
			this.UpdateSubPageVisibility();
		}

		// Token: 0x060077E7 RID: 30695 RVA: 0x0037D2D4 File Offset: 0x0037B4D4
		private List<AvatarSecondaryToggleConfig> GetSecondaryToggleConfigs(int primaryIndex)
		{
			List<AvatarSecondaryToggleConfig> configs = new List<AvatarSecondaryToggleConfig>();
			switch (primaryIndex)
			{
			case 0:
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_body_skin", true));
				break;
			case 1:
			{
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_hair_front", true));
				bool backHairEnabled = this.IsBackHairEnabled();
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_hair_back", backHairEnabled));
				bool beard1Enabled = this.IsBeardAvailable(1);
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_beard_upper", beard1Enabled));
				bool beard2Enabled = this.IsBeardAvailable(2);
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_beard_lower", beard2Enabled));
				break;
			}
			case 2:
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_eyebrow", true));
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_eye", true));
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_nose", true));
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_mouth", true));
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_feature", true));
				configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_feature", true));
				break;
			}
			return configs;
		}

		// Token: 0x060077E8 RID: 30696 RVA: 0x0037D3F4 File Offset: 0x0037B5F4
		public bool IsBackHairEnabled()
		{
			bool flag = this._avatarData == null;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				AvatarGroup group = SingletonObject.getInstance<AvatarManager>().GetAvatarGroup((int)this._avatarData.AvatarId);
				bool flag2 = group == null;
				if (flag2)
				{
					result = true;
				}
				else
				{
					AvatarAsset frontHairAsset = group.Get(EAvatarElementsType.Hair1, new short[]
					{
						this._avatarData.FrontHairId
					});
					bool flag3 = frontHairAsset == null;
					result = (flag3 || !frontHairAsset.Config.DisableRelativeType);
				}
			}
			return result;
		}

		// Token: 0x060077E9 RID: 30697 RVA: 0x0037D474 File Offset: 0x0037B674
		public bool IsBeardAvailable(sbyte beardType)
		{
			bool flag = this._gender != 1 || this._isTransGender;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = beardType == 1;
				if (flag2)
				{
					result = ((int)this._age >= GlobalConfig.Instance.AgeShowBeard1);
				}
				else
				{
					bool flag3 = beardType == 2;
					result = (flag3 && (int)this._age >= GlobalConfig.Instance.AgeShowBeard2);
				}
			}
			return result;
		}

		// Token: 0x060077EA RID: 30698 RVA: 0x0037D4E4 File Offset: 0x0037B6E4
		private void UpdateSubPageVisibility()
		{
			int primaryIndex = this.primaryToggleGroup.GetActiveIndex();
			bool flag = primaryIndex < 0;
			if (!flag)
			{
				int secondaryIndex = this.secondaryToggleGroupHelper.GetActiveIndex();
				bool flag2 = secondaryIndex < 0;
				if (!flag2)
				{
					NewGameSubPageAvatarPageBase targetPage = null;
					switch (primaryIndex)
					{
					case 0:
						targetPage = this.bodySkinPage;
						break;
					case 1:
						targetPage = ((secondaryIndex < 2) ? this.hairPage : this.beardPage);
						break;
					case 2:
						targetPage = ((secondaryIndex < 4) ? this.facePartPage : this.featurePage);
						break;
					}
					bool needSwitchPage = targetPage != this._currentActiveSubPage;
					bool flag3 = needSwitchPage;
					if (flag3)
					{
						bool flag4 = this._currentActiveSubPage != null;
						if (flag4)
						{
							this._currentActiveSubPage.gameObject.SetActive(false);
						}
						bool flag5 = targetPage != null;
						if (flag5)
						{
							targetPage.gameObject.SetActive(true);
						}
						this._currentActiveSubPage = targetPage;
					}
					int num = primaryIndex;
					int num2 = num;
					if (num2 != 1)
					{
						if (num2 == 2)
						{
							bool flag6 = secondaryIndex < 4;
							if (flag6)
							{
								if (!true)
								{
								}
								EAvatarElementsType eavatarElementsType;
								switch (secondaryIndex)
								{
								case 0:
									eavatarElementsType = EAvatarElementsType.EyeBrow;
									break;
								case 1:
									eavatarElementsType = EAvatarElementsType.Eye;
									break;
								case 2:
									eavatarElementsType = EAvatarElementsType.Nose;
									break;
								case 3:
									eavatarElementsType = EAvatarElementsType.Mouth;
									break;
								default:
									eavatarElementsType = EAvatarElementsType.EyeBrow;
									break;
								}
								if (!true)
								{
								}
								EAvatarElementsType partType = eavatarElementsType;
								this.facePartPage.SetPartType(partType);
							}
							else
							{
								EAvatarElementsType featureType = (secondaryIndex == 4) ? EAvatarElementsType.Feature1 : EAvatarElementsType.Feature2;
								this.featurePage.SetFeatureType(featureType);
							}
						}
					}
					else
					{
						bool flag7 = secondaryIndex < 2;
						if (flag7)
						{
							this.hairPage.SetHairType(secondaryIndex == 1);
						}
						else
						{
							this.beardPage.SetBeardType(secondaryIndex == 3);
						}
					}
				}
			}
		}

		// Token: 0x060077EB RID: 30699 RVA: 0x0037D69C File Offset: 0x0037B89C
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<AvatarData>("AvatarData", out this._srcAvatarData);
			this._avatarData = new AvatarData();
			this._avatarData.Copy(this._srcAvatarData);
			this._isAvatarDirty = false;
			argsBox.Get<Func<AvatarData>>("RandomAvatarHandler", out this._onRandomAvatar);
			argsBox.Get<Action<AvatarData>>("OnEditComplete", out this._onEditComplete);
			argsBox.Get("Age", out this._age);
			argsBox.Get("Gender", out this._gender);
			argsBox.Get("TransGender", out this._isTransGender);
			argsBox.Get<Func<sbyte, bool>>("BodyTypeFilter", out this._bodyTypeFilter);
			argsBox.Get<Func<List<ValueTuple<byte, Color>>, List<ValueTuple<byte, Color>>>>("SkinColorFilter", out this._skinColorFilter);
			argsBox.Get<Func<List<HairRes>, List<HairRes>>>("FrontHairFilter", out this._frontHairFilter);
			argsBox.Get<Func<List<HairRes>, List<HairRes>>>("BackHairFilter", out this._backHairFilter);
			argsBox.Get<Func<List<AvatarAsset>, List<AvatarAsset>>>("EyeBrowFilter", out this._eyebrowFilter);
			argsBox.Get<Func<List<EyeRes>, List<EyeRes>>>("EyesFilter", out this._eyesFilter);
			argsBox.Get<Func<List<AvatarAsset>, List<AvatarAsset>>>("NoseFilter", out this._noseFilter);
			argsBox.Get<Func<List<MouthRes>, List<MouthRes>>>("MouthFilter", out this._mouthFilter);
			argsBox.Get<Func<List<AvatarAsset>, List<AvatarAsset>>>("Beard1Filter", out this._beard1Filter);
			argsBox.Get<Func<List<AvatarAsset>, List<AvatarAsset>>>("Beard2Filter", out this._beard2Filter);
			argsBox.Get<Func<List<AvatarAsset>, List<AvatarAsset>>>("Feature1Filter", out this._feature1Filter);
			argsBox.Get<Func<List<AvatarAsset>, List<AvatarAsset>>>("Feature2Filter", out this._feature2Filter);
			argsBox.Get<Func<bool>>("CanShaveHairBald", out this._canShaveHairBald);
			argsBox.Get<Func<bool>>("CanShaveBeard1Bald", out this._canShaveBeard1Bald);
			argsBox.Get<Func<bool>>("CanShaveBeard2Bald", out this._canShaveBeard2Bald);
			argsBox.Get<Func<bool>>("CanShaveEyebrowBald", out this._canShaveEyebrowBald);
			this.avatarDisplay.Refresh(this._avatarData, this._age);
		}

		// Token: 0x060077EC RID: 30700 RVA: 0x0037D874 File Offset: 0x0037BA74
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			string text = btnName;
			string a = text;
			if (!(a == "ButtonCloseView"))
			{
				if (!(a == "ConfirmBtn"))
				{
					if (a == "RandomButton")
					{
						this.OnRandomAvatar();
					}
				}
				else
				{
					Action<AvatarData> onEditComplete = this._onEditComplete;
					if (onEditComplete != null)
					{
						onEditComplete(this._avatarData);
					}
					this.QuickHide();
				}
			}
			else
			{
				this.TryCancelAndHide();
			}
		}

		// Token: 0x060077ED RID: 30701 RVA: 0x0037D8EC File Offset: 0x0037BAEC
		private void TryCancelAndHide()
		{
			bool isAvatarDirty = this._isAvatarDirty;
			if (isAvatarDirty)
			{
				DialogCmd cmd = new DialogCmd();
				cmd.Type = 1;
				cmd.Title = LanguageKey.UI_EditAvatar_Cancel.Tr();
				cmd.Content = LanguageKey.UI_EditAvatar_Cancel_Content.Tr();
				cmd.Yes = delegate()
				{
					this._avatarData.Copy(this._srcAvatarData);
					this.QuickHide();
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.QuickHide();
			}
		}

		// Token: 0x060077EE RID: 30702 RVA: 0x0037D97C File Offset: 0x0037BB7C
		private void OnRandomAvatar()
		{
			Func<AvatarData> onRandomAvatar = this._onRandomAvatar;
			AvatarData avatarData = (onRandomAvatar != null) ? onRandomAvatar() : null;
			bool flag = avatarData != null;
			if (flag)
			{
				this._avatarData.Copy(avatarData);
				this.RefreshAvatar();
				int primaryIndex = this.primaryToggleGroup.GetActiveIndex();
				bool flag2 = primaryIndex >= 0;
				if (flag2)
				{
					List<AvatarSecondaryToggleConfig> configs = this.GetSecondaryToggleConfigs(primaryIndex);
					this.secondaryToggleGroupHelper.Refresh(configs, true);
				}
				NewGameSubPageAvatarPageBase currentActiveSubPage = this._currentActiveSubPage;
				if (currentActiveSubPage != null)
				{
					currentActiveSubPage.UpdateUI();
				}
			}
		}

		// Token: 0x060077EF RID: 30703 RVA: 0x0037DA00 File Offset: 0x0037BC00
		public AvatarData GetAvatarData()
		{
			return this._avatarData;
		}

		// Token: 0x060077F0 RID: 30704 RVA: 0x0037DA18 File Offset: 0x0037BC18
		public void RefreshAvatar()
		{
			this.avatarDisplay.Refresh(this._avatarData, this._age);
		}

		// Token: 0x060077F1 RID: 30705 RVA: 0x0037DA33 File Offset: 0x0037BC33
		public void MarkAvatarDirty()
		{
			this._isAvatarDirty = true;
		}

		// Token: 0x060077F2 RID: 30706 RVA: 0x0037DA3D File Offset: 0x0037BC3D
		public void MarkDirtyWithoutInscriptionClear()
		{
			this._isAvatarDirty = true;
		}

		// Token: 0x060077F3 RID: 30707 RVA: 0x0037DA48 File Offset: 0x0037BC48
		public bool IsAvatarDirty()
		{
			return this._isAvatarDirty;
		}

		// Token: 0x060077F4 RID: 30708 RVA: 0x0037DA60 File Offset: 0x0037BC60
		public AvatarData GetCurrentAvatarData()
		{
			return this._avatarData;
		}

		// Token: 0x060077F5 RID: 30709 RVA: 0x0037DA78 File Offset: 0x0037BC78
		public void ApplyPreset(AvatarPreset preset)
		{
		}

		// Token: 0x060077F6 RID: 30710 RVA: 0x0037DA7D File Offset: 0x0037BC7D
		public void ApplyInscribedCharacter(InscribedCharacterKey key, InscribedCharacter character)
		{
		}

		// Token: 0x060077F7 RID: 30711 RVA: 0x0037DA82 File Offset: 0x0037BC82
		public void TrySavePendingPreset()
		{
		}

		// Token: 0x060077F8 RID: 30712 RVA: 0x0037DA87 File Offset: 0x0037BC87
		public Func<sbyte, bool> GetBodyTypeFilter()
		{
			return this._bodyTypeFilter;
		}

		// Token: 0x060077F9 RID: 30713 RVA: 0x0037DA8F File Offset: 0x0037BC8F
		public Func<List<ValueTuple<byte, Color>>, List<ValueTuple<byte, Color>>> GetSkinColorFilter()
		{
			return this._skinColorFilter;
		}

		// Token: 0x060077FA RID: 30714 RVA: 0x0037DA97 File Offset: 0x0037BC97
		public Func<List<HairRes>, List<HairRes>> GetFrontHairFilter()
		{
			return this._frontHairFilter;
		}

		// Token: 0x060077FB RID: 30715 RVA: 0x0037DA9F File Offset: 0x0037BC9F
		public Func<List<HairRes>, List<HairRes>> GetBackHairFilter()
		{
			return this._backHairFilter;
		}

		// Token: 0x060077FC RID: 30716 RVA: 0x0037DAA7 File Offset: 0x0037BCA7
		public Func<List<AvatarAsset>, List<AvatarAsset>> GetEyebrowFilter()
		{
			return this._eyebrowFilter;
		}

		// Token: 0x060077FD RID: 30717 RVA: 0x0037DAAF File Offset: 0x0037BCAF
		public Func<List<EyeRes>, List<EyeRes>> GetEyesFilter()
		{
			return this._eyesFilter;
		}

		// Token: 0x060077FE RID: 30718 RVA: 0x0037DAB7 File Offset: 0x0037BCB7
		public Func<List<AvatarAsset>, List<AvatarAsset>> GetNoseFilter()
		{
			return this._noseFilter;
		}

		// Token: 0x060077FF RID: 30719 RVA: 0x0037DABF File Offset: 0x0037BCBF
		public Func<List<MouthRes>, List<MouthRes>> GetMouthFilter()
		{
			return this._mouthFilter;
		}

		// Token: 0x06007800 RID: 30720 RVA: 0x0037DAC7 File Offset: 0x0037BCC7
		public Func<List<AvatarAsset>, List<AvatarAsset>> GetBeard1Filter()
		{
			return this._beard1Filter;
		}

		// Token: 0x06007801 RID: 30721 RVA: 0x0037DACF File Offset: 0x0037BCCF
		public Func<List<AvatarAsset>, List<AvatarAsset>> GetBeard2Filter()
		{
			return this._beard2Filter;
		}

		// Token: 0x06007802 RID: 30722 RVA: 0x0037DAD7 File Offset: 0x0037BCD7
		public Func<List<AvatarAsset>, List<AvatarAsset>> GetFeature1Filter()
		{
			return this._feature1Filter;
		}

		// Token: 0x06007803 RID: 30723 RVA: 0x0037DADF File Offset: 0x0037BCDF
		public Func<List<AvatarAsset>, List<AvatarAsset>> GetFeature2Filter()
		{
			return this._feature2Filter;
		}

		// Token: 0x06007804 RID: 30724 RVA: 0x0037DAE7 File Offset: 0x0037BCE7
		public bool CanShaveHairBald()
		{
			Func<bool> canShaveHairBald = this._canShaveHairBald;
			return canShaveHairBald == null || canShaveHairBald();
		}

		// Token: 0x06007805 RID: 30725 RVA: 0x0037DAFB File Offset: 0x0037BCFB
		public bool CanShaveBeard1Bald()
		{
			Func<bool> canShaveBeard1Bald = this._canShaveBeard1Bald;
			return canShaveBeard1Bald == null || canShaveBeard1Bald();
		}

		// Token: 0x06007806 RID: 30726 RVA: 0x0037DB0F File Offset: 0x0037BD0F
		public bool CanShaveBeard2Bald()
		{
			Func<bool> canShaveBeard2Bald = this._canShaveBeard2Bald;
			return canShaveBeard2Bald == null || canShaveBeard2Bald();
		}

		// Token: 0x06007807 RID: 30727 RVA: 0x0037DB23 File Offset: 0x0037BD23
		public bool CanShaveEyebrowBald()
		{
			Func<bool> canShaveEyebrowBald = this._canShaveEyebrowBald;
			return canShaveEyebrowBald == null || canShaveEyebrowBald();
		}

		// Token: 0x06007808 RID: 30728 RVA: 0x0037DB38 File Offset: 0x0037BD38
		public bool GetIsTransGender()
		{
			return this._isTransGender;
		}

		// Token: 0x06007809 RID: 30729 RVA: 0x0037DB50 File Offset: 0x0037BD50
		public sbyte GetGender()
		{
			return this._gender;
		}

		// Token: 0x0600780A RID: 30730 RVA: 0x0037DB68 File Offset: 0x0037BD68
		public short GetAge()
		{
			return this._age;
		}

		// Token: 0x0600780B RID: 30731 RVA: 0x0037DB80 File Offset: 0x0037BD80
		public void RefreshHairTabsIfNeeded()
		{
			bool beardStateChanged = this.UpdateBeardShowingAbility();
			bool flag = beardStateChanged;
			if (flag)
			{
				this.RefreshAvatar();
			}
			int primaryIndex = this.primaryToggleGroup.GetActiveIndex();
			bool flag2 = primaryIndex != 1;
			if (!flag2)
			{
				int secondaryIndex = this.secondaryToggleGroupHelper.GetActiveIndex();
				bool isOnBeardTab = secondaryIndex >= 2;
				bool flag3 = isOnBeardTab;
				if (flag3)
				{
					sbyte beardType = (secondaryIndex == 2) ? 1 : 2;
					bool flag4 = !this.IsBeardAvailable(beardType);
					if (flag4)
					{
						this.secondaryToggleGroupHelper.SetActiveIndex(0);
					}
				}
				List<AvatarSecondaryToggleConfig> configs = this.GetSecondaryToggleConfigs(primaryIndex);
				this.secondaryToggleGroupHelper.Refresh(configs, true);
			}
		}

		// Token: 0x0600780C RID: 30732 RVA: 0x0037DC20 File Offset: 0x0037BE20
		private bool UpdateBeardShowingAbility()
		{
			bool flag = this._avatarData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool beard1Available = this.IsBeardAvailable(1);
				bool beard2Available = this.IsBeardAvailable(2);
				bool hasChange = false;
				this._avatarData.SetGrowableElementShowingAbility(1, beard1Available);
				this._avatarData.SetGrowableElementShowingAbility(2, beard2Available);
				bool flag2 = !beard1Available && this._avatarData.GetGrowableElementShowingState(1);
				if (flag2)
				{
					this._avatarData.SetGrowableElementShowingState(1, false);
					hasChange = true;
				}
				bool flag3 = !beard2Available && this._avatarData.GetGrowableElementShowingState(2);
				if (flag3)
				{
					this._avatarData.SetGrowableElementShowingState(2, false);
					hasChange = true;
				}
				result = hasChange;
			}
			return result;
		}

		// Token: 0x04005A93 RID: 23187
		[SerializeField]
		private Game.Components.Avatar.Avatar avatarDisplay;

		// Token: 0x04005A94 RID: 23188
		[Header("右侧页签")]
		[SerializeField]
		private CToggleGroup primaryToggleGroup;

		// Token: 0x04005A95 RID: 23189
		[SerializeField]
		private AvatarSecondaryToggleGroupHelper secondaryToggleGroupHelper;

		// Token: 0x04005A96 RID: 23190
		[Header("子页面组件")]
		[SerializeField]
		private NewGameSubPageAvatarBodySkinPage bodySkinPage;

		// Token: 0x04005A97 RID: 23191
		[SerializeField]
		private NewGameSubPageAvatarHairPage hairPage;

		// Token: 0x04005A98 RID: 23192
		[SerializeField]
		private NewGameSubPageAvatarBeardPage beardPage;

		// Token: 0x04005A99 RID: 23193
		[SerializeField]
		private NewGameSubPageAvatarFacePartPage facePartPage;

		// Token: 0x04005A9A RID: 23194
		[SerializeField]
		private NewGameSubPageAvatarFeaturePage featurePage;

		// Token: 0x04005A9B RID: 23195
		private AvatarData _srcAvatarData;

		// Token: 0x04005A9C RID: 23196
		private AvatarData _avatarData;

		// Token: 0x04005A9D RID: 23197
		private Func<AvatarData> _onRandomAvatar;

		// Token: 0x04005A9E RID: 23198
		private Action<AvatarData> _onEditComplete;

		// Token: 0x04005A9F RID: 23199
		private NewGameSubPageAvatarPageBase _currentActiveSubPage;

		// Token: 0x04005AA0 RID: 23200
		private short _age;

		// Token: 0x04005AA1 RID: 23201
		private sbyte _gender;

		// Token: 0x04005AA2 RID: 23202
		private bool _isTransGender;

		// Token: 0x04005AA3 RID: 23203
		private bool _isAvatarDirty;

		// Token: 0x04005AA4 RID: 23204
		private Func<sbyte, bool> _bodyTypeFilter;

		// Token: 0x04005AA5 RID: 23205
		private Func<List<ValueTuple<byte, Color>>, List<ValueTuple<byte, Color>>> _skinColorFilter;

		// Token: 0x04005AA6 RID: 23206
		private Func<List<HairRes>, List<HairRes>> _frontHairFilter;

		// Token: 0x04005AA7 RID: 23207
		private Func<List<HairRes>, List<HairRes>> _backHairFilter;

		// Token: 0x04005AA8 RID: 23208
		private Func<List<AvatarAsset>, List<AvatarAsset>> _eyebrowFilter;

		// Token: 0x04005AA9 RID: 23209
		private Func<List<EyeRes>, List<EyeRes>> _eyesFilter;

		// Token: 0x04005AAA RID: 23210
		private Func<List<AvatarAsset>, List<AvatarAsset>> _noseFilter;

		// Token: 0x04005AAB RID: 23211
		private Func<List<MouthRes>, List<MouthRes>> _mouthFilter;

		// Token: 0x04005AAC RID: 23212
		private Func<List<AvatarAsset>, List<AvatarAsset>> _beard1Filter;

		// Token: 0x04005AAD RID: 23213
		private Func<List<AvatarAsset>, List<AvatarAsset>> _beard2Filter;

		// Token: 0x04005AAE RID: 23214
		private Func<List<AvatarAsset>, List<AvatarAsset>> _feature1Filter;

		// Token: 0x04005AAF RID: 23215
		private Func<List<AvatarAsset>, List<AvatarAsset>> _feature2Filter;

		// Token: 0x04005AB0 RID: 23216
		private Func<bool> _canShaveHairBald;

		// Token: 0x04005AB1 RID: 23217
		private Func<bool> _canShaveBeard1Bald;

		// Token: 0x04005AB2 RID: 23218
		private Func<bool> _canShaveBeard2Bald;

		// Token: 0x04005AB3 RID: 23219
		private Func<bool> _canShaveEyebrowBald;
	}
}
