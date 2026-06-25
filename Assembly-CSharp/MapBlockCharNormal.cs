using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Config;
using FrameWork;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character.Display;
using GameData.Domains.Map;
using GameData.Domains.Merchant;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.Taiwu.Profession.SkillsData;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UICommon.Character;
using UnityEngine;

// Token: 0x020003D0 RID: 976
public class MapBlockCharNormal : MapBlockCharAlive
{
	// Token: 0x170005F7 RID: 1527
	// (get) Token: 0x06003AD0 RID: 15056 RVA: 0x001DD0BB File Offset: 0x001DB2BB
	private CharacterDisplayData _characterDisplayData
	{
		get
		{
			return this.DisplayData;
		}
	}

	// Token: 0x06003AD1 RID: 15057 RVA: 0x001DD0C4 File Offset: 0x001DB2C4
	public void Init(bool canInteract, MapBlockData mapBlock, CharacterDisplayData characterDisplayData, List<LoongInfo> loongInfos)
	{
		base.Init(canInteract, mapBlock, characterDisplayData);
		this._murderMapMode = false;
		this._charConfig = Character.Instance[this._characterDisplayData.TemplateId];
		this.CharId = this._characterDisplayData.CharacterId;
		this._loongInfos = loongInfos;
		this.tipDisplayer.enabled = false;
		this.settlementInfo.ShowJieqingSignFixed = false;
		this.Refresh();
	}

	// Token: 0x06003AD2 RID: 15058 RVA: 0x001DD138 File Offset: 0x001DB338
	public void InitJieqingMurdermap(bool canInteract, MapBlockData mapBlock, CharacterDisplayData characterDisplayData, List<LoongInfo> loongInfos, Action onClick)
	{
		this.settlementInfo.ShowJieqingSignFixed = true;
		this._onClickByMurderMap = onClick;
		this.Init(canInteract, mapBlock, characterDisplayData, loongInfos);
		this.button.interactable = true;
		this.button.ClearAndAddListener(new Action(this.OnClickButton));
		this._murderMapMode = true;
	}

	// Token: 0x06003AD3 RID: 15059 RVA: 0x001DD193 File Offset: 0x001DB393
	public void Init(MapBlockData mapBlock, CharacterDisplayData characterDisplayData, Action onClick)
	{
		this._onClickUsedByBeggarSkill3 = onClick;
		this.Init(true, mapBlock, characterDisplayData, null);
	}

	// Token: 0x06003AD4 RID: 15060 RVA: 0x001DD1A8 File Offset: 0x001DB3A8
	protected override void Refresh()
	{
		base.Refresh();
		this.RefreshHappinessAndFavorability();
		this.RefreshExperience();
		this.RefreshApprove();
		this.RefreshWork();
		this.SetBG();
		this.settlementInfo.Refresh(this._characterDisplayData);
		this.RefreshTipDisplayer();
	}

	// Token: 0x06003AD5 RID: 15061 RVA: 0x001DD1FC File Offset: 0x001DB3FC
	private void SetBG()
	{
		this.darkAshBg.SetActive((this._characterDisplayData.DarkAshProtector & 512U) > 0U);
		bool flag = this._characterDisplayData.DarkAshProtector == 512U;
		if (flag)
		{
			this.darkAshEffect.Play();
		}
		else
		{
			this.darkAshEffect.Play();
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				UIParticle uiparticle = this.darkAshEffect;
				if (uiparticle != null)
				{
					uiparticle.Pause();
				}
			});
		}
		bool flag2 = (this._characterDisplayData.DarkAshProtector & 512U) == 0U && this._loongInfos != null;
		if (flag2)
		{
			for (int i = 0; i < this._loongInfos.Count; i++)
			{
				bool flag3 = !this._loongInfos[i].IsDisappear && this._loongInfos[i].GetCharacterDebuffCount(this._characterDisplayData.CharacterId) > 0;
				if (flag3)
				{
					ResLoader.Load<SkeletonDataAsset>("Dlc/FiveLoong/RemakeResources/Particle/UIEffectPrefabs/MapBlockCharacter/Fiveloong_SkeletonData", delegate(SkeletonDataAsset dataAsset)
					{
						this.skeletonGraphic.skeletonDataAsset = dataAsset;
						this.skeletonGraphic.Initialize(true);
						this.skeletonGraphic.initialSkinName = "basic";
						this.skeletonGraphic.AnimationState.SetAnimation(0, "animation", true);
						this.skeletonGraphic.gameObject.SetActive(true);
						CommonUtils.SetLoongDebuff(this._loongInfos, this._characterDisplayData, this.mouseTipDisplay, this.circleGenerator);
					}, null, false);
					break;
				}
				this.skeletonGraphic.gameObject.SetActive(false);
				this.circleGenerator.ClearExistGO();
			}
		}
		else
		{
			this.skeletonGraphic.gameObject.SetActive(false);
			this.circleGenerator.ClearExistGO();
		}
	}

	// Token: 0x06003AD6 RID: 15062 RVA: 0x001DD354 File Offset: 0x001DB554
	protected override void RefreshName()
	{
		string nameContent = NameCenter.GetCharMonasticTitleOrNameByDisplayData(this._characterDisplayData, false, false);
		this.nameText.text = nameContent;
	}

	// Token: 0x06003AD7 RID: 15063 RVA: 0x001DD380 File Offset: 0x001DB580
	protected override void RefreshOrganization()
	{
		this.organizationText.text = CommonUtils.GetOrganizationGradeString(this._characterDisplayData.OrgInfo, this._characterDisplayData.Gender, this._characterDisplayData.PhysiologicalAge, (int)this._characterDisplayData.TemplateId);
		bool isChild = CommonUtils.CheckCharIsChild(this._characterDisplayData.OrgInfo, this._characterDisplayData.PhysiologicalAge);
		bool isMerchant = CommonUtils.CheckCharIsMerchant(this._characterDisplayData.OrgInfo);
		bool flag = !isChild && isMerchant;
		if (flag)
		{
			MerchantDomainMethod.AsyncCall.GetMerchantTemplateId(null, this._characterDisplayData.CharacterId, delegate(int offset, RawDataPool dataPool)
			{
				sbyte merchantTemplateId = -1;
				Serializer.Deserialize(dataPool, offset, ref merchantTemplateId);
				bool flag2 = merchantTemplateId > -1;
				if (flag2)
				{
					this.merchantNameBg.gameObject.SetActive(true);
					MerchantItem merchantConfig = Merchant.Instance[merchantTemplateId];
					MerchantTypeItem merchantTypeConfig = Config.MerchantType.Instance[merchantConfig.MerchantType];
					this.merchantNameText.text = merchantTypeConfig.Name;
					this.merchantLevelImage.SetSprite(base.GetMerchantLevelImage(merchantConfig.Level), false, null);
				}
			});
		}
		else
		{
			this.merchantNameBg.gameObject.SetActive(false);
		}
		this.organizationIcon.gameObject.SetActive(true);
		this.organizationIcon.SetSprite(CommonUtils.GetIdentityIcon(this._characterDisplayData.OrgInfo.Grade), false, null);
	}

	// Token: 0x06003AD8 RID: 15064 RVA: 0x001DD470 File Offset: 0x001DB670
	protected override void RefreshIcon()
	{
		MapBlockData mapBlock = this.MapBlock;
		bool? flag;
		if (mapBlock == null)
		{
			flag = null;
		}
		else
		{
			HashSet<int> infectedCharacterSet = mapBlock.InfectedCharacterSet;
			flag = ((infectedCharacterSet != null) ? new bool?(infectedCharacterSet.Contains(this._characterDisplayData.CharacterId)) : null);
		}
		bool? isInfected = flag;
		MapBlockData mapBlock2 = this.MapBlock;
		bool? flag2;
		if (mapBlock2 == null)
		{
			flag2 = null;
		}
		else
		{
			HashSet<int> enemyCharacterSet = mapBlock2.EnemyCharacterSet;
			flag2 = ((enemyCharacterSet != null) ? new bool?(enemyCharacterSet.Contains(this._characterDisplayData.CharacterId)) : null);
		}
		bool? isEnemy = flag2;
		bool flag3 = this._characterDisplayData.CompletelyInfected || isInfected.GetValueOrDefault() || isEnemy.GetValueOrDefault();
		if (flag3)
		{
			this.iconImage.SetSprite("blockchar_icon_diren", false, null);
		}
		else
		{
			bool flag4 = this._charConfig.XiangshuType == 1;
			if (flag4)
			{
				this.iconImage.SetSprite("map_icon_xiangshu", false, null);
			}
			else
			{
				bool flag5 = this._charConfig.XiangshuType == 3;
				if (flag5)
				{
					this.iconImage.SetSprite("map_icon_zizhu", false, null);
				}
				else
				{
					bool flag6;
					if (this._charConfig.CreatingType != 1)
					{
						MapBlockData mapBlock3 = this.MapBlock;
						flag6 = (((mapBlock3 != null) ? mapBlock3.CharacterSet : null) == null || !this.MapBlock.CharacterSet.Contains(this._characterDisplayData.CharacterId));
					}
					else
					{
						flag6 = false;
					}
					bool flag7 = flag6;
					if (flag7)
					{
						this.iconImage.SetSprite("map_icon_teshu", false, null);
					}
					else
					{
						this.iconImage.enabled = false;
					}
				}
			}
		}
	}

	// Token: 0x06003AD9 RID: 15065 RVA: 0x001DD5FC File Offset: 0x001DB7FC
	protected override void OnClickButton()
	{
		bool murderMapMode = this._murderMapMode;
		if (murderMapMode)
		{
			Action onClickByMurderMap = this._onClickByMurderMap;
			if (onClickByMurderMap != null)
			{
				onClickByMurderMap();
			}
		}
		else
		{
			bool isMoving = base.IsMoving;
			if (!isMoving)
			{
				MapBlockData mapBlock = this.MapBlock;
				short? num = (mapBlock != null) ? new short?(mapBlock.BlockId) : null;
				int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
				int currentBlockId = base.CurrentBlockId;
				bool flag = !(num2.GetValueOrDefault() == currentBlockId & num2 != null) || (int)this._characterDisplayData.Location.BlockId != base.CurrentBlockId;
				if (!flag)
				{
					bool flag2 = this._onClickUsedByBeggarSkill3 != null;
					if (flag2)
					{
						this._onClickUsedByBeggarSkill3();
					}
					else
					{
						GameDataBridge.AddMethodCall<int>(-1, 12, 13, this._characterDisplayData.CharacterId);
						base.OnClickButton();
					}
				}
			}
		}
	}

	// Token: 0x06003ADA RID: 15066 RVA: 0x001DD6FC File Offset: 0x001DB8FC
	private void RefreshHappinessAndFavorability()
	{
		Refers happinessRefers = this.iconHolder.CGet<Refers>("CharacterHappiness");
		CharacterHappiness happinessController = happinessRefers.UserObject as CharacterHappiness;
		bool flag = happinessController == null;
		if (flag)
		{
			happinessController = new CharacterHappiness(happinessRefers, false);
			happinessRefers.UserObject = happinessController;
		}
		happinessController.CharacterId = this._characterDisplayData.CharacterId;
		Refers favorRefers = this.iconHolder.CGet<Refers>("CharacterFavorability");
		CharacterFavorability characterFavorability = favorRefers.UserObject as CharacterFavorability;
		bool flag2 = characterFavorability == null;
		if (flag2)
		{
			characterFavorability = new CharacterFavorability(favorRefers, false);
			favorRefers.UserObject = characterFavorability;
		}
		characterFavorability.CharacterId = this._characterDisplayData.CharacterId;
	}

	// Token: 0x06003ADB RID: 15067 RVA: 0x001DD7A4 File Offset: 0x001DB9A4
	public override void OnHide()
	{
		base.OnHide();
		Refers happinessRefers = this.iconHolder.CGet<Refers>("CharacterHappiness");
		CharacterHappiness happinessController = happinessRefers.UserObject as CharacterHappiness;
		bool flag = happinessController != null;
		if (flag)
		{
			happinessController.CharacterId = -1;
		}
		Refers favorRefers = this.iconHolder.CGet<Refers>("CharacterFavorability");
		CharacterHappiness characterFavorability = favorRefers.UserObject as CharacterHappiness;
		bool flag2 = characterFavorability != null;
		if (flag2)
		{
			characterFavorability.CharacterId = -1;
		}
	}

	// Token: 0x06003ADC RID: 15068 RVA: 0x001DD818 File Offset: 0x001DBA18
	private void RefreshExperience()
	{
		TooltipInvoker tooltipInvoker = this.experienceTip;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		this.experienceTip.RuntimeParam.Clear();
		this.experienceTip.RuntimeParam.SetObject("CharacterDisplayData", this._characterDisplayData);
		this.experienceTip.Refresh(true, -1);
	}

	// Token: 0x06003ADD RID: 15069 RVA: 0x001DD880 File Offset: 0x001DBA80
	private void RefreshApprove()
	{
		bool showApprove = this._characterDisplayData != null && this._characterDisplayData.IsApproveTaiwu;
		this.approveObj.SetActive(showApprove);
		bool flag = showApprove;
		if (flag)
		{
			double value = GameData.Domains.World.SharedMethods.GetApproveTaiwuDisplayData(this._characterDisplayData.ApproveTaiwu);
			this.approvingRateText.text = LocalStringManager.GetFormat(LanguageKey.LK_MapBlockCharList_Approve, value);
			ProfessionModel professionModel = SingletonObject.getInstance<ProfessionModel>();
			ProfessionData profession = professionModel.GetProfessionData(17);
			DukeSkillsData dukeSkillsData = (profession != null) ? profession.GetSkillsData<DukeSkillsData>() : null;
			bool isApproveDuck = dukeSkillsData != null && dukeSkillsData.CharacterHasTitle(this._characterDisplayData.CharacterId);
			this.approveObj.transform.Find("DukeIcon").gameObject.SetActive(isApproveDuck);
			this.approveObj.transform.Find("Icon").gameObject.SetActive(!isApproveDuck);
		}
	}

	// Token: 0x06003ADE RID: 15070 RVA: 0x001DD96B File Offset: 0x001DBB6B
	private void RefreshWork()
	{
		this.workingObj.gameObject.SetActive(false);
		TaiwuDomainMethod.AsyncCall.GetVillagerRoleCharacterDisplayData(null, this._characterDisplayData.CharacterId, delegate(int offset, RawDataPool dataPool)
		{
			VillagerRoleCharacterDisplayData displayData = new VillagerRoleCharacterDisplayData();
			Serializer.Deserialize(dataPool, offset, ref displayData);
			bool flag = displayData == null || !this.workingObj;
			if (!flag)
			{
				VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = displayData.ArrangementDisplayData;
				int arrangementId = (arrangementDisplayData != null) ? arrangementDisplayData.ArrangementTemplateId : -1;
				VillagerRoleUtils.RefreshWorkingIcon(this.workingObj.GetComponent<CImage>(), this._characterDisplayData.CharacterId, (short)arrangementId, displayData);
			}
		});
	}

	// Token: 0x06003ADF RID: 15071 RVA: 0x001DD9A0 File Offset: 0x001DBBA0
	private void RefreshTipDisplayer()
	{
		bool flag = !this.tipDisplayer;
		if (!flag)
		{
			this.tipDisplayer.enabled = (this.DisplayData.CreatingType == 1 && this._charConfig.CanOpenCharacterMenu);
			this.tipDisplayer.Type = TipType.CharacterOnMapBlock;
			this.tipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CharId", this.CharId);
		}
	}

	// Token: 0x04002A51 RID: 10833
	[SerializeField]
	private Refers iconHolder;

	// Token: 0x04002A52 RID: 10834
	[SerializeField]
	private GameObject merchantNameBg;

	// Token: 0x04002A53 RID: 10835
	[SerializeField]
	private TextMeshProUGUI merchantNameText;

	// Token: 0x04002A54 RID: 10836
	[SerializeField]
	private TooltipInvoker experienceTip;

	// Token: 0x04002A55 RID: 10837
	[SerializeField]
	private GameObject approveObj;

	// Token: 0x04002A56 RID: 10838
	[SerializeField]
	private TextMeshProUGUI approvingRateText;

	// Token: 0x04002A57 RID: 10839
	[SerializeField]
	private GameObject workingObj;

	// Token: 0x04002A58 RID: 10840
	[SerializeField]
	private GameObject darkAshBg;

	// Token: 0x04002A59 RID: 10841
	[SerializeField]
	private UIParticle darkAshEffect;

	// Token: 0x04002A5A RID: 10842
	[SerializeField]
	private TooltipInvoker tipDisplayer;

	// Token: 0x04002A5B RID: 10843
	[SerializeField]
	private CImage merchantLevelImage;

	// Token: 0x04002A5C RID: 10844
	[SerializeField]
	private CharacterSettlementInfo settlementInfo;

	// Token: 0x04002A5D RID: 10845
	private CharacterItem _charConfig;

	// Token: 0x04002A5E RID: 10846
	private List<LoongInfo> _loongInfos;

	// Token: 0x04002A5F RID: 10847
	private Action _onClickUsedByBeggarSkill3;

	// Token: 0x04002A60 RID: 10848
	private Action _onClickByMurderMap;

	// Token: 0x04002A61 RID: 10849
	private bool _murderMapMode = false;
}
