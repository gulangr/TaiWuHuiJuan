using System;
using CharacterDataMonitor;
using Config;
using GameData.Domains.Building;
using TMPro;
using UnityEngine;

namespace Building
{
	// Token: 0x02000661 RID: 1633
	public class BuildingExpandTaiwuVillageUpgrade : MonoBehaviour
	{
		// Token: 0x17000974 RID: 2420
		// (get) Token: 0x06004DAE RID: 19886 RVA: 0x00249D70 File Offset: 0x00247F70
		private BuildingModel Model
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x06004DAF RID: 19887 RVA: 0x00249D78 File Offset: 0x00247F78
		public void Set(BuildingBlockData blockData, BuildingBlockItem config)
		{
			this.disk.Set(blockData, config);
			sbyte level = blockData.CalcUnlockedLevelCount();
			this.levelInfo.Set((int)level, (int)(level + 1), level < config.MaxLevel);
			for (int i = 0; i < config.ExpandInfos.Count; i++)
			{
				short scaleTemplateId = config.ExpandInfos[i];
				BuildingScaleItem scaleConfig = BuildingScale.Instance[scaleTemplateId];
				if (!true)
				{
				}
				BuildingUpgradeEffect buildingUpgradeEffect;
				switch (i)
				{
				case 0:
					buildingUpgradeEffect = this.buildingCapacity;
					break;
				case 1:
					buildingUpgradeEffect = this.stoneRoomCapacity;
					break;
				case 2:
					buildingUpgradeEffect = this.jiaoPoolCapacity;
					break;
				default:
					buildingUpgradeEffect = null;
					break;
				}
				if (!true)
				{
				}
				BuildingUpgradeEffect effect = buildingUpgradeEffect;
				bool flag = effect == null;
				if (flag)
				{
					throw new NotSupportedException(string.Format("building upgrade effect {0} is not supported", i));
				}
				effect.Set(scaleConfig, (int)level, (int)config.MaxLevel);
			}
			this._baseCost = ((level < config.MaxLevel) ? GlobalConfig.Instance.TaiwuVillageUpgradeAuthorityCosts[(int)level] : 0);
			this.UpdateCost();
		}

		// Token: 0x06004DB0 RID: 19888 RVA: 0x00249E90 File Offset: 0x00248090
		public void Set(sbyte orgTemplateId)
		{
			OrganizationItem orgConfig = Organization.Instance[orgTemplateId];
			this.hintText.text = LocalStringManager.GetFormat(LanguageKey.LK_Building_ExpandTaiwuVillage_Desc, orgConfig.Name);
			this.iconImage.SetSprite(string.Format("building_vow_sect_{0}", (int)(orgTemplateId - 1)), false, null);
			this._reduceCost = !this.Model.OrgIsTaskStatus(orgTemplateId, 0);
			this.UpdateCost();
		}

		// Token: 0x06004DB1 RID: 19889 RVA: 0x00249F03 File Offset: 0x00248103
		public void Set(bool jiaoPoolIsOpen)
		{
			this.jiaoPoolCapacity.gameObject.SetActive(jiaoPoolIsOpen);
		}

		// Token: 0x06004DB2 RID: 19890 RVA: 0x00249F18 File Offset: 0x00248118
		private void UpdateCost()
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
		}

		// Token: 0x040035DB RID: 13787
		[SerializeField]
		private BuildingExpandTaiwuVillageDisk disk;

		// Token: 0x040035DC RID: 13788
		[SerializeField]
		private BuildingUpgradeEffect levelInfo;

		// Token: 0x040035DD RID: 13789
		[SerializeField]
		private BuildingUpgradeCost authorityCost;

		// Token: 0x040035DE RID: 13790
		[SerializeField]
		private BuildingUpgradeEffect buildingCapacity;

		// Token: 0x040035DF RID: 13791
		[SerializeField]
		private BuildingUpgradeEffect stoneRoomCapacity;

		// Token: 0x040035E0 RID: 13792
		[SerializeField]
		private BuildingUpgradeEffect jiaoPoolCapacity;

		// Token: 0x040035E1 RID: 13793
		[SerializeField]
		private CButtonObsolete confirm;

		// Token: 0x040035E2 RID: 13794
		[SerializeField]
		private TextMeshProUGUI hintText;

		// Token: 0x040035E3 RID: 13795
		[SerializeField]
		private CImage iconImage;

		// Token: 0x040035E4 RID: 13796
		private int _baseCost;

		// Token: 0x040035E5 RID: 13797
		private bool _reduceCost;
	}
}
