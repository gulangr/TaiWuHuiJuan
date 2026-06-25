using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.NewGame;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using GameData.Domains.Global.Inscription;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x0200040E RID: 1038
public class ViewCharacterShave : UIBase, IAvatarSubPageParent
{
	// Token: 0x06003DF0 RID: 15856 RVA: 0x001F16E0 File Offset: 0x001EF8E0
	public override void OnInit(ArgumentBox argsBox)
	{
		AvatarAdjustController.InitGroupColors();
		this.NeedDataListenerId = true;
		argsBox.Get("CharId", out this._characterId);
		argsBox.Get("NpcId", out this._cutterCharId);
		this._onShaveComplete = this.Confirm();
		this.avatarDisplay.ResetToBlank(false);
		this.Element.OnListenerIdReady = delegate()
		{
			this._monitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AvatarInfoMonitor>(this._characterId, false);
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this, this._characterId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._displayData);
				this.OnGetCharacterDisplayData(this._displayData);
				bool init = this._monitor.Init;
				if (init)
				{
					this.OnGetCharacterAvatarData();
				}
				else
				{
					this._monitor.AddOnAvatarDataChangeEventListener(new Action(this.OnGetCharacterAvatarData));
				}
			});
		};
		this.openCharMenuButton.ClearAndAddListener(new Action(this.OnClickOpenCharMenuButton));
	}

	// Token: 0x06003DF1 RID: 15857 RVA: 0x001F1767 File Offset: 0x001EF967
	private void OnGetCharacterDisplayData(CharacterDisplayData data)
	{
		this._name = NameCenter.GetNameByDisplayData(data, data.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId, false);
		this.nameLabel.text = this._name;
	}

	// Token: 0x06003DF2 RID: 15858 RVA: 0x001F179C File Offset: 0x001EF99C
	private void OnGetCharacterAvatarData()
	{
		this._avatarData = new AvatarData(this._monitor.AvatarData);
		this._avatarData.Copy(this._monitor.AvatarData);
		bool flag = this._avatarDataOld == null;
		if (flag)
		{
			this._avatarDataOld = new AvatarData(this._monitor.AvatarData);
			this._avatarDataOld.Copy(this._monitor.AvatarData);
		}
		List<AvatarSecondaryToggleConfig> configs = this.GetSecondaryToggleConfigs();
		this.secondaryToggleGroupHelper.Refresh(configs, false);
		this.avatarDisplay.Refresh(this._avatarData, this._displayData.PhysiologicalAge);
		this.UpdateSubPageVisibility();
		this.hairPage.UpdateUI();
		this.beardPage.UpdateUI();
		this.facePartPage.UpdateUI();
		this.featurePage.UpdateUI();
		this.clothPage.UpdateUI();
		this._monitor.RemoveOnAvatarDataChangeEventListener(new Action(this.OnGetCharacterAvatarData));
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x06003DF3 RID: 15859 RVA: 0x001F18AC File Offset: 0x001EFAAC
	private void Awake()
	{
		this.InitializeUI();
	}

	// Token: 0x06003DF4 RID: 15860 RVA: 0x001F18B6 File Offset: 0x001EFAB6
	private void Update()
	{
		this.root.SetActive(!UIElement.CharacterMenu.Exist);
	}

	// Token: 0x06003DF5 RID: 15861 RVA: 0x001F18D4 File Offset: 0x001EFAD4
	private List<AvatarSecondaryToggleConfig> GetSecondaryToggleConfigs()
	{
		List<AvatarSecondaryToggleConfig> configs = new List<AvatarSecondaryToggleConfig>();
		configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_hair_front", true));
		bool backHairEnabled = this.IsBackHairEnabled();
		configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_hair_back", backHairEnabled));
		bool beard1Enabled = this.IsBeardAvailable(1);
		configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_beard_upper", beard1Enabled));
		bool beard2Enabled = this.IsBeardAvailable(2);
		configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_beard_lower", beard2Enabled));
		configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_eyebrow", true));
		bool mouthColorAvailable = this.IsMouthColorAvailable();
		configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_mouth", mouthColorAvailable));
		bool feature1ColorAvailable = this.IsFeatureColorAvailable(EAvatarElementsType.Feature1);
		configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_feature", feature1ColorAvailable));
		bool feature2ColorAvailable = this.IsFeatureColorAvailable(EAvatarElementsType.Feature2);
		configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_feature", feature2ColorAvailable));
		configs.Add(new AvatarSecondaryToggleConfig("ui9_btn_avatar_create_sub_toggle_cloth", true));
		return configs;
	}

	// Token: 0x06003DF6 RID: 15862 RVA: 0x001F19C7 File Offset: 0x001EFBC7
	private void InitializeUI()
	{
		this.secondaryToggleGroupHelper.Init();
		this.secondaryToggleGroupHelper.AddOnActiveIndexChangeListener(new Action<int, int>(this.OnSecondaryPageChanged));
		this.InitializeSubPages();
	}

	// Token: 0x06003DF7 RID: 15863 RVA: 0x001F19F8 File Offset: 0x001EFBF8
	private void InitializeSubPages()
	{
		this.hairPage.Init(this);
		this.beardPage.Init(this);
		this.facePartPage.Init(this);
		this.featurePage.Init(this);
		this.clothPage.Init(this);
		this.clothPage.OnlyCreateRes = true;
	}

	// Token: 0x06003DF8 RID: 15864 RVA: 0x001F1A54 File Offset: 0x001EFC54
	private void OnSecondaryPageChanged(int newIndex, int oldIndex)
	{
		this.UpdateSubPageVisibility();
	}

	// Token: 0x06003DF9 RID: 15865 RVA: 0x001F1A60 File Offset: 0x001EFC60
	private void UpdateSubPageVisibility()
	{
		int secondaryIndex = this.secondaryToggleGroupHelper.GetActiveIndex();
		bool flag = secondaryIndex < 0;
		if (!flag)
		{
			NewGameSubPageAvatarPageBase targetPage = null;
			switch (secondaryIndex)
			{
			case 0:
			case 1:
				targetPage = this.hairPage;
				break;
			case 2:
			case 3:
				targetPage = this.beardPage;
				break;
			case 4:
			case 5:
				targetPage = this.facePartPage;
				break;
			case 6:
			case 7:
				targetPage = this.featurePage;
				break;
			case 8:
				targetPage = this.clothPage;
				break;
			}
			bool needSwitchPage = targetPage != this._currentActiveSubPage;
			bool flag2 = needSwitchPage;
			if (flag2)
			{
				bool flag3 = this._currentActiveSubPage != null;
				if (flag3)
				{
					this._currentActiveSubPage.gameObject.SetActive(false);
				}
				bool flag4 = targetPage != null;
				if (flag4)
				{
					targetPage.gameObject.SetActive(true);
				}
				this._currentActiveSubPage = targetPage;
			}
			switch (secondaryIndex)
			{
			case 0:
				this.hairPage.SetHairType(false);
				break;
			case 1:
				this.hairPage.SetHairType(true);
				break;
			case 2:
				this.beardPage.SetBeardType(false);
				break;
			case 3:
				this.beardPage.SetBeardType(true);
				break;
			case 4:
				this.facePartPage.SetPartType(EAvatarElementsType.EyeBrow);
				break;
			case 5:
				this.facePartPage.SetPartType(EAvatarElementsType.Mouth);
				break;
			case 6:
				this.featurePage.SetFeatureType(EAvatarElementsType.Feature1);
				break;
			case 7:
				this.featurePage.SetFeatureType(EAvatarElementsType.Feature2);
				break;
			}
		}
	}

	// Token: 0x06003DFA RID: 15866 RVA: 0x001F1BFC File Offset: 0x001EFDFC
	private Action<AvatarData> Confirm()
	{
		return delegate(AvatarData avatarData)
		{
			bool flag = avatarData == null;
			if (flag)
			{
				TaiwuEventDomainMethod.Call.TriggerListener("CharacterShaveComplete", true);
			}
			else
			{
				CharacterDomainMethod.Call.CharacterShaveAvatar(this.Element.GameDataListenerId, this._cutterCharId, this._characterId, avatarData);
			}
		};
	}

	// Token: 0x06003DFB RID: 15867 RVA: 0x001F1C1C File Offset: 0x001EFE1C
	public override void QuickHide()
	{
		bool flag = this.Element.IsInState(EUiElementState.Ready) || this.Element.IsInState(EUiElementState.AnimateIn);
		if (flag)
		{
			UIManager.Instance.HideUI(this.Element);
		}
		Action<AvatarData> onShaveComplete = this._onShaveComplete;
		if (onShaveComplete != null)
		{
			onShaveComplete(null);
		}
		AudioManager.Instance.PlaySound("ui_default_cancel", false, false);
	}

	// Token: 0x06003DFC RID: 15868 RVA: 0x001F1C84 File Offset: 0x001EFE84
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = "ConfirmBtn" == btnName;
		if (flag)
		{
			bool hasChanged = this.HasAvatarDataChanged(this._avatarData, this._avatarDataOld);
			bool flag2 = hasChanged;
			if (flag2)
			{
				Action<AvatarData> onShaveComplete = this._onShaveComplete;
				if (onShaveComplete != null)
				{
					onShaveComplete(this._avatarData.FormatDisabledElements());
				}
			}
			else
			{
				this.QuickHide();
			}
		}
		else
		{
			bool flag3 = "ButtonCloseView" == btnName;
			if (flag3)
			{
				this.QuickHide();
			}
		}
	}

	// Token: 0x06003DFD RID: 15869 RVA: 0x001F1D06 File Offset: 0x001EFF06
	private void OnDisable()
	{
		this._monitor.RemoveOnAvatarDataChangeEventListener(new Action(this.OnGetCharacterAvatarData));
		this._monitor = null;
		this._avatarDataOld = null;
	}

	// Token: 0x06003DFE RID: 15870 RVA: 0x001F1D30 File Offset: 0x001EFF30
	private bool HasAvatarDataChanged(AvatarData data1, AvatarData data2)
	{
		bool flag = data1 == null || data2 == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool hasChanged = data1.FrontHairId != data2.FrontHairId || data1.ColorFrontHairId != data2.ColorFrontHairId || data1.BackHairId != data2.BackHairId || data1.ColorBackHairId != data2.ColorBackHairId || data1.Beard1Id != data2.Beard1Id || data1.ColorBeard1Id != data2.ColorBeard1Id || data1.Beard2Id != data2.Beard2Id || data1.ColorBeard2Id != data2.ColorBeard2Id || data1.EyebrowId != data2.EyebrowId || data1.EyebrowAngle != data2.EyebrowAngle || data1.EyebrowHeight != data2.EyebrowHeight || data1.EyebrowScale != data2.EyebrowScale || data1.EyebrowDistance != data2.EyebrowDistance || data1.ColorEyebrowId != data2.ColorEyebrowId || data1.ColorMouthId != data2.ColorMouthId || data1.Feature1Id != data2.Feature1Id || data1.Feature2Id != data2.Feature2Id || data1.ColorClothId != data2.ColorClothId || data1.GetGrowableElementShowingState(0) != data2.GetGrowableElementShowingState(0) || data1.GetGrowableElementShowingState(1) != data2.GetGrowableElementShowingState(1) || data1.GetGrowableElementShowingState(2) != data2.GetGrowableElementShowingState(2) || data1.GetGrowableElementShowingState(6) != data2.GetGrowableElementShowingState(6);
			result = hasChanged;
		}
		return result;
	}

	// Token: 0x06003DFF RID: 15871 RVA: 0x001F1F6C File Offset: 0x001F016C
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			bool flag = notification.Type == 1;
			if (flag)
			{
				bool flag2 = notification.MethodId == 80;
				if (flag2)
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._shaveResult);
					ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
					argBox.Set("ShaveResult", this._shaveResult);
					bool flag3 = !this._shaveResult;
					if (flag3)
					{
						CharacterDomainMethod.Call.GetAvatarData(this.Element.GameDataListenerId, this._characterId);
					}
					else
					{
						argBox.SetObject("ShaveAvatar", new List<AvatarData>
						{
							this._avatarDataOld,
							this._avatarData
						});
						argBox.Set("ShaveCharName", this._name);
						argBox.Set("ShaveCharAge", this._displayData.PhysiologicalAge);
						UIElement.GetItem.SetOnInitArgs(argBox);
						UIManager.Instance.MaskUI(UIElement.GetItem);
						UIManager.Instance.HideUI(this.Element);
						UIElement getItem = UIElement.GetItem;
						getItem.OnHide = (Action)Delegate.Combine(getItem.OnHide, new Action(delegate()
						{
							TaiwuEventDomainMethod.Call.TriggerListener("CharacterShaveComplete", true);
						}));
					}
				}
				else
				{
					bool flag4 = notification.MethodId == 99;
					if (flag4)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._avatarData);
						ArgumentBox argBox2 = EasyPool.Get<ArgumentBox>();
						argBox2.Set("ShaveResult", this._shaveResult);
						argBox2.SetObject("ShaveAvatar", new List<AvatarData>
						{
							this._avatarDataOld,
							this._avatarData
						});
						argBox2.Set("ShaveCharName", this._name);
						argBox2.Set("ShaveCharAge", this._displayData.PhysiologicalAge);
						UIElement.GetItem.SetOnInitArgs(argBox2);
						UIManager.Instance.MaskUI(UIElement.GetItem);
						UIManager.Instance.HideUI(this.Element);
						UIElement getItem2 = UIElement.GetItem;
						getItem2.OnHide = (Action)Delegate.Combine(getItem2.OnHide, new Action(delegate()
						{
							TaiwuEventDomainMethod.Call.TriggerListener("CharacterShaveComplete", true);
						}));
					}
				}
			}
		}
	}

	// Token: 0x06003E00 RID: 15872 RVA: 0x001F2230 File Offset: 0x001F0430
	public short GetAge()
	{
		return this._displayData.PhysiologicalAge;
	}

	// Token: 0x06003E01 RID: 15873 RVA: 0x001F2240 File Offset: 0x001F0440
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

	// Token: 0x06003E02 RID: 15874 RVA: 0x001F22C0 File Offset: 0x001F04C0
	public bool IsBeardAvailable(sbyte beardType)
	{
		bool flag = this.GetGender() != 1 || this.GetIsTransGender();
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
				result = ((int)this.GetAge() >= GlobalConfig.Instance.AgeShowBeard1);
			}
			else
			{
				bool flag3 = beardType == 2;
				result = (flag3 && (int)this.GetAge() >= GlobalConfig.Instance.AgeShowBeard2);
			}
		}
		return result;
	}

	// Token: 0x06003E03 RID: 15875 RVA: 0x001F2330 File Offset: 0x001F0530
	public bool IsMouthColorAvailable()
	{
		bool flag = this._avatarData == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			AvatarGroup group = SingletonObject.getInstance<AvatarManager>().GetAvatarGroup((int)this._avatarData.AvatarId);
			bool flag2 = ((group != null) ? group.MouthRes : null) == null;
			if (flag2)
			{
				result = false;
			}
			else
			{
				MouthRes mouth = group.MouthRes.Find((MouthRes m) => m.Id == this._avatarData.MouthId);
				byte? b;
				if (mouth == null)
				{
					b = null;
				}
				else
				{
					AvatarAsset mouth2 = mouth.Mouth;
					if (mouth2 == null)
					{
						b = null;
					}
					else
					{
						AvatarElementsItem config = mouth2.Config;
						b = ((config != null) ? new byte?(config.ColorGroup) : null);
					}
				}
				byte? b2 = b;
				int? num = (b2 != null) ? new int?((int)b2.GetValueOrDefault()) : null;
				int num2 = 0;
				result = !(num.GetValueOrDefault() == num2 & num != null);
			}
		}
		return result;
	}

	// Token: 0x06003E04 RID: 15876 RVA: 0x001F2424 File Offset: 0x001F0624
	public bool IsFeatureColorAvailable(EAvatarElementsType featureType)
	{
		bool flag = this._avatarData == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			AvatarGroup group = SingletonObject.getInstance<AvatarManager>().GetAvatarGroup((int)this._avatarData.AvatarId);
			bool flag2 = group == null;
			if (flag2)
			{
				result = false;
			}
			else
			{
				short featureId = (featureType == EAvatarElementsType.Feature1) ? this._avatarData.Feature1Id : this._avatarData.Feature2Id;
				bool flag3 = featureId == 1;
				if (flag3)
				{
					result = false;
				}
				else
				{
					List<AvatarAsset> sourceList = (featureType == EAvatarElementsType.Feature1) ? group.Feature1Res : group.Feature2Res;
					bool flag4 = sourceList == null;
					if (flag4)
					{
						result = false;
					}
					else
					{
						AvatarAsset feature = sourceList.Find((AvatarAsset f) => f.Id == featureId);
						byte? b;
						if (feature == null)
						{
							b = null;
						}
						else
						{
							AvatarElementsItem config = feature.Config;
							b = ((config != null) ? new byte?(config.ColorGroup) : null);
						}
						byte? b2 = b;
						int? num = (b2 != null) ? new int?((int)b2.GetValueOrDefault()) : null;
						int num2 = 0;
						result = !(num.GetValueOrDefault() == num2 & num != null);
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06003E05 RID: 15877 RVA: 0x001F2560 File Offset: 0x001F0760
	public void RefreshHairTabsIfNeeded()
	{
		bool beardStateChanged = this.UpdateBeardShowingAbility();
		bool flag = beardStateChanged;
		if (flag)
		{
			this.RefreshAvatar();
		}
		int secondaryIndex = this.secondaryToggleGroupHelper.GetActiveIndex();
		bool isOnBeardTab = secondaryIndex >= 2 && secondaryIndex <= 3;
		bool flag2 = isOnBeardTab;
		if (flag2)
		{
			sbyte beardType = (secondaryIndex == 2) ? 1 : 2;
			bool flag3 = !this.IsBeardAvailable(beardType);
			if (flag3)
			{
				this.secondaryToggleGroupHelper.SetActiveIndex(0);
			}
		}
		List<AvatarSecondaryToggleConfig> configs = this.GetSecondaryToggleConfigs();
		this.secondaryToggleGroupHelper.Refresh(configs, false);
	}

	// Token: 0x06003E06 RID: 15878 RVA: 0x001F25E8 File Offset: 0x001F07E8
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

	// Token: 0x06003E07 RID: 15879 RVA: 0x001F2694 File Offset: 0x001F0894
	private void OnClickOpenCharMenuButton()
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("CharacterId", this._characterId);
		argBox.Set("CanOperate", false);
		UIElement.CharacterMenu.SetOnInitArgs(argBox);
		UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		this.root.SetActive(false);
	}

	// Token: 0x06003E08 RID: 15880 RVA: 0x001F26F1 File Offset: 0x001F08F1
	public AvatarData GetAvatarData()
	{
		return this._avatarData;
	}

	// Token: 0x06003E09 RID: 15881 RVA: 0x001F26FC File Offset: 0x001F08FC
	public void RefreshAvatar()
	{
		bool flag = this._avatarData == null || this._displayData == null;
		if (!flag)
		{
			this.avatarDisplay.Refresh(this._avatarData, this._displayData.PhysiologicalAge);
			NewGameSubPageAvatarPageBase currentActiveSubPage = this._currentActiveSubPage;
			if (currentActiveSubPage != null)
			{
				currentActiveSubPage.UpdateUI();
			}
			AvatarRelatedData avatarRelatedData = this._displayData.AvatarRelatedData;
			bool isFixedPresetType = CreatingType.IsFixedPresetType(this._displayData.CreatingType);
			bool? flag2;
			if (avatarRelatedData == null)
			{
				flag2 = null;
			}
			else
			{
				AvatarData avatarData = avatarRelatedData.AvatarData;
				flag2 = ((avatarData != null) ? new bool?(avatarData.FaceVisible) : null);
			}
			bool faceVisible = flag2 ?? true;
			short clothDisplayId = (avatarRelatedData != null) ? avatarRelatedData.ClothingDisplayId : 0;
			short charm = this._avatarData.GetCharm(this._displayData.PhysiologicalAge, clothDisplayId);
			string charmText = CommonUtils.GetCharmLevelText(charm, this._avatarData.Gender, this._displayData.PhysiologicalAge, clothDisplayId, isFixedPresetType, faceVisible);
			this.charmLabel.text = charmText;
		}
	}

	// Token: 0x06003E0A RID: 15882 RVA: 0x001F2810 File Offset: 0x001F0A10
	public void MarkAvatarDirty()
	{
	}

	// Token: 0x06003E0B RID: 15883 RVA: 0x001F2813 File Offset: 0x001F0A13
	public void MarkDirtyWithoutInscriptionClear()
	{
	}

	// Token: 0x06003E0C RID: 15884 RVA: 0x001F2818 File Offset: 0x001F0A18
	public bool IsAvatarDirty()
	{
		return false;
	}

	// Token: 0x06003E0D RID: 15885 RVA: 0x001F282B File Offset: 0x001F0A2B
	public void ApplyPreset(AvatarPreset preset)
	{
		throw new NotSupportedException("ViewCharacterShave does not support preset functionality.");
	}

	// Token: 0x06003E0E RID: 15886 RVA: 0x001F2838 File Offset: 0x001F0A38
	public void ApplyInscribedCharacter(InscribedCharacterKey key, InscribedCharacter character)
	{
		throw new NotSupportedException("ViewCharacterShave does not support inscribed character functionality.");
	}

	// Token: 0x06003E0F RID: 15887 RVA: 0x001F2845 File Offset: 0x001F0A45
	public void TrySavePendingPreset()
	{
		throw new NotSupportedException("ViewCharacterShave does not support preset functionality.");
	}

	// Token: 0x06003E10 RID: 15888 RVA: 0x001F2852 File Offset: 0x001F0A52
	public AvatarData GetCurrentAvatarData()
	{
		return this._avatarData;
	}

	// Token: 0x06003E11 RID: 15889 RVA: 0x001F285C File Offset: 0x001F0A5C
	public bool GetIsTransGender()
	{
		return this._avatarData.Gender != this._displayData.Gender;
	}

	// Token: 0x06003E12 RID: 15890 RVA: 0x001F288C File Offset: 0x001F0A8C
	public sbyte GetGender()
	{
		return this._avatarData.Gender;
	}

	// Token: 0x06003E13 RID: 15891 RVA: 0x001F28AC File Offset: 0x001F0AAC
	public bool OnlyShowShaveItems()
	{
		return true;
	}

	// Token: 0x06003E14 RID: 15892 RVA: 0x001F28C0 File Offset: 0x001F0AC0
	public bool IsCopyCloth()
	{
		return true;
	}

	// Token: 0x04002C9A RID: 11418
	[SerializeField]
	private Game.Components.Avatar.Avatar avatarDisplay;

	// Token: 0x04002C9B RID: 11419
	[SerializeField]
	private TextMeshProUGUI nameLabel;

	// Token: 0x04002C9C RID: 11420
	[SerializeField]
	private CButton openCharMenuButton;

	// Token: 0x04002C9D RID: 11421
	[SerializeField]
	private TextMeshProUGUI charmLabel;

	// Token: 0x04002C9E RID: 11422
	[SerializeField]
	private GameObject root;

	// Token: 0x04002C9F RID: 11423
	[Header("右侧页签")]
	[SerializeField]
	private AvatarSecondaryToggleGroupHelper secondaryToggleGroupHelper;

	// Token: 0x04002CA0 RID: 11424
	[Header("子页面组件")]
	[SerializeField]
	private NewGameSubPageAvatarHairPage hairPage;

	// Token: 0x04002CA1 RID: 11425
	[SerializeField]
	private NewGameSubPageAvatarBeardPage beardPage;

	// Token: 0x04002CA2 RID: 11426
	[SerializeField]
	private NewGameSubPageAvatarFacePartPage facePartPage;

	// Token: 0x04002CA3 RID: 11427
	[SerializeField]
	private NewGameSubPageAvatarFeaturePage featurePage;

	// Token: 0x04002CA4 RID: 11428
	[SerializeField]
	private NewGameSubPageAvatarClothPage clothPage;

	// Token: 0x04002CA5 RID: 11429
	private Action<AvatarData> _onShaveComplete;

	// Token: 0x04002CA6 RID: 11430
	private int _characterId;

	// Token: 0x04002CA7 RID: 11431
	private int _cutterCharId;

	// Token: 0x04002CA8 RID: 11432
	private AvatarData _avatarData;

	// Token: 0x04002CA9 RID: 11433
	private AvatarData _avatarDataOld;

	// Token: 0x04002CAA RID: 11434
	private AvatarInfoMonitor _monitor;

	// Token: 0x04002CAB RID: 11435
	private string _name;

	// Token: 0x04002CAC RID: 11436
	private bool _shaveResult;

	// Token: 0x04002CAD RID: 11437
	private CharacterDisplayData _displayData;

	// Token: 0x04002CAE RID: 11438
	private NewGameSubPageAvatarPageBase _currentActiveSubPage;
}
