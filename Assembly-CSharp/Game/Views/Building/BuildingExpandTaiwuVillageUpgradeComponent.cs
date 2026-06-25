using System;
using CharacterDataMonitor;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Building;
using TMPro;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BD8 RID: 3032
	public class BuildingExpandTaiwuVillageUpgradeComponent : MonoBehaviour
	{
		// Token: 0x1700105A RID: 4186
		// (get) Token: 0x060098A4 RID: 39076 RVA: 0x0047152D File Offset: 0x0046F72D
		private BuildingModel Model
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x060098A5 RID: 39077 RVA: 0x00471534 File Offset: 0x0046F734
		public void Set(BuildingBlockData blockData, BuildingBlockItem config)
		{
			this.disk.Set(blockData, config);
			this.RefreshUpgradeInfo(blockData, config, false);
			this.UpdateConfirmButton();
		}

		// Token: 0x060098A6 RID: 39078 RVA: 0x00471558 File Offset: 0x0046F758
		private void RefreshUpgradeInfo(BuildingBlockData blockData, BuildingBlockItem config, bool upgraded = false)
		{
			sbyte level = blockData.CalcUnlockedLevelCount();
			if (upgraded)
			{
				this.levelInfo.Set((int)level);
			}
			else
			{
				this.levelInfo.Set((int)level, (int)(level + 1), level < config.MaxLevel);
			}
			bool buildingAreaLimit = SingletonObject.getInstance<BasicGameData>().ChallengeModeData.IsEnabled(EChallengeModeImplement.BuildingAreaLimit);
			for (int i = 0; i < config.ExpandInfos.Count; i++)
			{
				short scaleTemplateId = config.ExpandInfos[i];
				BuildingScaleItem scaleConfig = BuildingScale.Instance[scaleTemplateId];
				if (!true)
				{
				}
				BuildingUpgradeEffectComponent buildingUpgradeEffectComponent;
				switch (i)
				{
				case 0:
					buildingUpgradeEffectComponent = this.buildingCapacity;
					break;
				case 1:
					buildingUpgradeEffectComponent = this.stoneRoomCapacity;
					break;
				case 2:
					buildingUpgradeEffectComponent = this.jiaoPoolCapacity;
					break;
				default:
					buildingUpgradeEffectComponent = null;
					break;
				}
				if (!true)
				{
				}
				BuildingUpgradeEffectComponent effect = buildingUpgradeEffectComponent;
				bool flag = effect == null;
				if (flag)
				{
					throw new NotSupportedException(string.Format("building upgrade effect {0} is not supported", i));
				}
				if (upgraded)
				{
					effect.Set(scaleConfig, (int)level, buildingAreaLimit);
				}
				else
				{
					effect.Set(scaleConfig, (int)level, (int)config.MaxLevel, buildingAreaLimit);
				}
			}
			this._baseCost = ((level < config.MaxLevel) ? GlobalConfig.Instance.TaiwuVillageUpgradeAuthorityCosts[(int)level] : 0);
		}

		// Token: 0x060098A7 RID: 39079 RVA: 0x004716A0 File Offset: 0x0046F8A0
		public void Set(sbyte orgTemplateId, bool unlocked, BuildingBlockData blockData, BuildingBlockItem config)
		{
			this._unlocked = unlocked;
			OrganizationItem orgConfig = Organization.Instance[orgTemplateId];
			this.hintText.text = LocalStringManager.GetFormat(LanguageKey.LK_Building_ExpandTaiwuVillage_Desc, orgConfig.Name);
			this._reduceCost = !this.Model.OrgIsTaskStatus(orgTemplateId, 0);
			this.UpdateConfirmButton();
			this.RefreshUpgradeInfo(blockData, config, unlocked);
			this.imgUpgradeSectIcon.SetSprite(string.Format("{0}{1}", "ui9_back_building_vow_sect_", (int)(orgTemplateId - 1)), false, null);
		}

		// Token: 0x060098A8 RID: 39080 RVA: 0x0047172A File Offset: 0x0046F92A
		public void Set(bool jiaoPoolIsOpen)
		{
			this.jiaoPoolCapacity.gameObject.SetActive(jiaoPoolIsOpen);
		}

		// Token: 0x060098A9 RID: 39081 RVA: 0x00471740 File Offset: 0x0046F940
		private void UpdateConfirmButton()
		{
			bool unlocked = this._unlocked;
			if (unlocked)
			{
				this.authorityCost.SetEmpty();
				this.confirm.interactable = false;
			}
			else
			{
				int charId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				ResourceMonitor monitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ResourceMonitor>(charId, false);
				int current = monitor.Resources[7];
				int cost = this._baseCost;
				bool reduceCost = this._reduceCost;
				if (reduceCost)
				{
					cost *= GlobalConfig.Instance.VowFinishedSectStoryAuthorityPercent;
				}
				this.authorityCost.Set(current, cost);
				this.confirm.interactable = (current >= cost);
				this.confirmMousetips.enabled = this._reachAmountLimition;
				bool reachAmountLimition = this._reachAmountLimition;
				if (reachAmountLimition)
				{
					this.confirm.interactable = false;
				}
			}
		}

		// Token: 0x060098AA RID: 39082 RVA: 0x00471811 File Offset: 0x0046FA11
		public void SetLimition(bool reachAmountLimition)
		{
			this._reachAmountLimition = reachAmountLimition;
			this.UpdateConfirmButton();
		}

		// Token: 0x0400756C RID: 30060
		[SerializeField]
		private BuildingExpandTaiwuVillageDiskComponent disk;

		// Token: 0x0400756D RID: 30061
		[SerializeField]
		private BuildingUpgradeEffectComponent levelInfo;

		// Token: 0x0400756E RID: 30062
		[SerializeField]
		private BuildingUpgradeCostComponent authorityCost;

		// Token: 0x0400756F RID: 30063
		[SerializeField]
		private BuildingUpgradeEffectComponent buildingCapacity;

		// Token: 0x04007570 RID: 30064
		[SerializeField]
		private BuildingUpgradeEffectComponent stoneRoomCapacity;

		// Token: 0x04007571 RID: 30065
		[SerializeField]
		private BuildingUpgradeEffectComponent jiaoPoolCapacity;

		// Token: 0x04007572 RID: 30066
		[SerializeField]
		private CButton confirm;

		// Token: 0x04007573 RID: 30067
		[SerializeField]
		private TooltipInvoker confirmMousetips;

		// Token: 0x04007574 RID: 30068
		[SerializeField]
		private CImage imgUpgradeSectIcon;

		// Token: 0x04007575 RID: 30069
		[SerializeField]
		private TextMeshProUGUI hintText;

		// Token: 0x04007576 RID: 30070
		private int _baseCost;

		// Token: 0x04007577 RID: 30071
		private bool _reduceCost;

		// Token: 0x04007578 RID: 30072
		private bool _reachAmountLimition;

		// Token: 0x04007579 RID: 30073
		private bool _unlocked;
	}
}
