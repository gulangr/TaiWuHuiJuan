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
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;

// Token: 0x020003D1 RID: 977
public class MapBlockCharNormal2 : MapBlockCharAlive2
{
	// Token: 0x170005F8 RID: 1528
	// (get) Token: 0x06003AE5 RID: 15077 RVA: 0x001DDBA6 File Offset: 0x001DBDA6
	private CharacterDisplayData CharacterDisplayData
	{
		get
		{
			return this.DisplayData;
		}
	}

	// Token: 0x06003AE6 RID: 15078 RVA: 0x001DDBB0 File Offset: 0x001DBDB0
	public void Init(bool canInteract, MapBlockData mapBlock, CharacterDisplayData characterDisplayData, List<LoongInfo> loongInfos)
	{
		base.Init(canInteract, mapBlock, characterDisplayData);
		this._charConfig = Character.Instance[this.CharacterDisplayData.TemplateId];
		this.CharId = this.CharacterDisplayData.CharacterId;
		this._loongInfos = loongInfos;
		this.tipDisplayer.enabled = false;
		this.settlementInfo.ShowJieqingSignFixed = false;
		this.Refresh();
	}

	// Token: 0x06003AE7 RID: 15079 RVA: 0x001DDC1C File Offset: 0x001DBE1C
	public void InitJieqingMurdermap(bool canInteract, MapBlockData mapBlock, CharacterDisplayData characterDisplayData, List<LoongInfo> loongInfos)
	{
		this.settlementInfo.ShowJieqingSignFixed = true;
		this.Init(canInteract, mapBlock, characterDisplayData, loongInfos);
	}

	// Token: 0x06003AE8 RID: 15080 RVA: 0x001DDC37 File Offset: 0x001DBE37
	public void Init(MapBlockData mapBlock, CharacterDisplayData characterDisplayData, Action onClick)
	{
		this._onClickUsedByBeggarSkill3 = onClick;
		this.Init(true, mapBlock, characterDisplayData, null);
	}

	// Token: 0x06003AE9 RID: 15081 RVA: 0x001DDC4C File Offset: 0x001DBE4C
	protected override void Refresh()
	{
		base.Refresh();
		this.RefreshIcon();
		this.RefreshWork();
		this.SetBG();
		this.settlementInfo.Refresh(this.CharacterDisplayData);
		this.RefreshTipDisplayer();
	}

	// Token: 0x06003AEA RID: 15082 RVA: 0x001DDC84 File Offset: 0x001DBE84
	private void SetBG()
	{
		this.darkAshBg.SetActive((this.CharacterDisplayData.DarkAshProtector & 512U) > 0U);
		bool flag = this.CharacterDisplayData.DarkAshProtector == 512U;
		if (flag)
		{
			this.darkAshEffect.Play();
		}
		else
		{
			this.darkAshEffect.Play();
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				bool flag4 = this.darkAshEffect;
				if (flag4)
				{
					this.darkAshEffect.Pause();
				}
			});
		}
		bool flag2 = (this.CharacterDisplayData.DarkAshProtector & 512U) == 0U && this._loongInfos != null;
		if (flag2)
		{
			for (int i = 0; i < this._loongInfos.Count; i++)
			{
				bool flag3 = !this._loongInfos[i].IsDisappear && this._loongInfos[i].GetCharacterDebuffCount(this.CharacterDisplayData.CharacterId) > 0;
				if (flag3)
				{
					ResLoader.Load<SkeletonDataAsset>("Dlc/FiveLoong/RemakeResources/Particle/UIEffectPrefabs/MapBlockCharacter/Fiveloong_SkeletonData", delegate(SkeletonDataAsset dataAsset)
					{
						this.skeletonGraphic.skeletonDataAsset = dataAsset;
						this.skeletonGraphic.Initialize(true);
						this.skeletonGraphic.initialSkinName = "basic";
						this.skeletonGraphic.AnimationState.SetAnimation(0, "animation", true);
						this.skeletonGraphic.gameObject.SetActive(true);
						CommonUtils.SetLoongDebuff(this._loongInfos, this.CharacterDisplayData, this.extraMouseTipDisplay, this.circleGenerator);
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

	// Token: 0x06003AEB RID: 15083 RVA: 0x001DDDDC File Offset: 0x001DBFDC
	protected override void RefreshName()
	{
		string nameContent = NameCenter.GetMonasticTitleOrDisplayName(this.CharacterDisplayData, false);
		this.nameLabel.text = nameContent;
	}

	// Token: 0x06003AEC RID: 15084 RVA: 0x001DDE04 File Offset: 0x001DC004
	protected override void RefreshOrganization()
	{
		this.organizationLabel.text = CommonUtils.GetOrganizationGradeString(this.CharacterDisplayData.OrgInfo, this.CharacterDisplayData.Gender, this.CharacterDisplayData.PhysiologicalAge, (int)this.CharacterDisplayData.TemplateId);
		bool isChild = CommonUtils.CheckCharIsChild(this.CharacterDisplayData.OrgInfo, this.CharacterDisplayData.PhysiologicalAge);
		bool isMerchant = CommonUtils.CheckCharIsMerchant(this.CharacterDisplayData.OrgInfo);
		bool flag = !isChild && isMerchant;
		if (flag)
		{
			MerchantDomainMethod.AsyncCall.GetMerchantTemplateId(null, this.CharacterDisplayData.CharacterId, delegate(int offset, RawDataPool dataPool)
			{
				sbyte merchantTemplateId = -1;
				Serializer.Deserialize(dataPool, offset, ref merchantTemplateId);
				bool flag2 = merchantTemplateId > -1;
				if (flag2)
				{
					this.merchantIcon.gameObject.SetActive(true);
					MerchantItem merchantConfig = Merchant.Instance[merchantTemplateId];
					this.merchantLevelLabel.text = merchantConfig.Level.ToString();
				}
			});
		}
		else
		{
			this.merchantIcon.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003AED RID: 15085 RVA: 0x001DDEBC File Offset: 0x001DC0BC
	protected virtual void RefreshIcon()
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
			flag = ((infectedCharacterSet != null) ? new bool?(infectedCharacterSet.Contains(this.CharacterDisplayData.CharacterId)) : null);
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
			flag2 = ((enemyCharacterSet != null) ? new bool?(enemyCharacterSet.Contains(this.CharacterDisplayData.CharacterId)) : null);
		}
		bool? isEnemy = flag2;
		bool flag3 = this.CharacterDisplayData.CompletelyInfected || isInfected.GetValueOrDefault() || isEnemy.GetValueOrDefault();
		if (flag3)
		{
			this.specialIcon.gameObject.SetActive(true);
			this.specialIcon.SetSprite("blockchar_icon_diren", false, null);
		}
		else
		{
			bool flag4 = this._charConfig.XiangshuType == 1;
			if (flag4)
			{
				this.specialIcon.gameObject.SetActive(true);
				this.specialIcon.SetSprite("ui_sp_icon_specialcharacter_2", false, null);
			}
			else
			{
				bool flag5 = this._charConfig.XiangshuType == 3;
				if (flag5)
				{
					this.specialIcon.gameObject.SetActive(true);
					this.specialIcon.SetSprite("map_icon_zizhu", false, null);
				}
				else
				{
					bool flag6;
					if (this._charConfig.CreatingType != 1)
					{
						MapBlockData mapBlock3 = this.MapBlock;
						flag6 = (((mapBlock3 != null) ? mapBlock3.CharacterSet : null) == null || !this.MapBlock.CharacterSet.Contains(this.CharacterDisplayData.CharacterId));
					}
					else
					{
						flag6 = false;
					}
					bool flag7 = flag6;
					if (flag7)
					{
						this.specialIcon.gameObject.SetActive(true);
						this.specialIcon.SetSprite("ui_sp_icon_specialcharacter_1", false, null);
					}
					else
					{
						this.specialIcon.gameObject.SetActive(false);
					}
				}
			}
		}
	}

	// Token: 0x06003AEE RID: 15086 RVA: 0x001DE0A4 File Offset: 0x001DC2A4
	protected override void OnClickButton()
	{
		bool isMoving = base.IsMoving;
		if (!isMoving)
		{
			MapBlockData mapBlock = this.MapBlock;
			short? num = (mapBlock != null) ? new short?(mapBlock.BlockId) : null;
			int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
			int currentBlockId = base.CurrentBlockId;
			bool flag = !(num2.GetValueOrDefault() == currentBlockId & num2 != null) || (int)this.CharacterDisplayData.Location.BlockId != base.CurrentBlockId;
			if (!flag)
			{
				bool flag2 = this._onClickUsedByBeggarSkill3 != null;
				if (flag2)
				{
					this._onClickUsedByBeggarSkill3();
				}
				else
				{
					GameDataBridge.AddMethodCall<int>(-1, 12, 13, this.CharacterDisplayData.CharacterId);
					base.OnClickButton();
				}
			}
		}
	}

	// Token: 0x06003AEF RID: 15087 RVA: 0x001DE17F File Offset: 0x001DC37F
	private void RefreshWork()
	{
		this.workingObj.SetActive(false);
		TaiwuDomainMethod.AsyncCall.GetVillagerRoleCharacterDisplayData(null, this.CharacterDisplayData.CharacterId, delegate(int offset, RawDataPool dataPool)
		{
			VillagerRoleCharacterDisplayData displayData = new VillagerRoleCharacterDisplayData();
			Serializer.Deserialize(dataPool, offset, ref displayData);
			bool flag = displayData == null || !this.workingObj;
			if (!flag)
			{
				VillagerRoleArrangementDisplayDataWrapper arrangementDisplayData = displayData.ArrangementDisplayData;
				int arrangementId = (arrangementDisplayData != null) ? arrangementDisplayData.ArrangementTemplateId : -1;
				VillagerRoleUtils.RefreshWorkingIcon(this.workingObj.GetComponent<CImage>(), this.CharacterDisplayData.CharacterId, (short)arrangementId, displayData);
			}
		});
	}

	// Token: 0x06003AF0 RID: 15088 RVA: 0x001DE1B0 File Offset: 0x001DC3B0
	private void RefreshTipDisplayer()
	{
		bool flag = !this.tipDisplayer;
		if (!flag)
		{
			this.tipDisplayer.enabled = (this.DisplayData.CreatingType == 1 && this._charConfig.CanOpenCharacterMenu);
			this.tipDisplayer.Type = TipType.CharacterOnMapBlock;
			this.tipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("CharId", this.CharId).Set("IsMapBlockCharList", true);
		}
	}

	// Token: 0x04002A62 RID: 10850
	[SerializeField]
	private GameObject workingObj;

	// Token: 0x04002A63 RID: 10851
	[SerializeField]
	private GameObject darkAshBg;

	// Token: 0x04002A64 RID: 10852
	[SerializeField]
	private UIParticle darkAshEffect;

	// Token: 0x04002A65 RID: 10853
	[SerializeField]
	private TooltipInvoker tipDisplayer;

	// Token: 0x04002A66 RID: 10854
	[SerializeField]
	private CharacterSettlementInfo settlementInfo;

	// Token: 0x04002A67 RID: 10855
	[SerializeField]
	private CImage merchantIcon;

	// Token: 0x04002A68 RID: 10856
	[SerializeField]
	private TextMeshProUGUI merchantLevelLabel;

	// Token: 0x04002A69 RID: 10857
	[SerializeField]
	private CImage specialIcon;

	// Token: 0x04002A6A RID: 10858
	protected const string IconXiangshu = "ui_sp_icon_specialcharacter_2";

	// Token: 0x04002A6B RID: 10859
	protected const string IconSpecial = "ui_sp_icon_specialcharacter_1";

	// Token: 0x04002A6C RID: 10860
	protected const string IconCaravan = "ui_sp_icon_specialcharacter_0";

	// Token: 0x04002A6D RID: 10861
	protected const string IconEnemy = "blockchar_icon_diren";

	// Token: 0x04002A6E RID: 10862
	protected const string IconZizhu = "map_icon_zizhu";

	// Token: 0x04002A6F RID: 10863
	private CharacterItem _charConfig;

	// Token: 0x04002A70 RID: 10864
	private List<LoongInfo> _loongInfos;

	// Token: 0x04002A71 RID: 10865
	private Action _onClickUsedByBeggarSkill3;
}
