using System;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Views.Building
{
	// Token: 0x02000BD2 RID: 3026
	public class BuildingUpgradeEffectComponent : MonoBehaviour
	{
		// Token: 0x0600984E RID: 38990 RVA: 0x0046F768 File Offset: 0x0046D968
		public void Set(BuildingScaleItem config, int currLevel, int maxLevel, bool buildingAreaLimit)
		{
			int levelIndex = currLevel - 1;
			int fromValue = config.LevelEffect[levelIndex];
			if (buildingAreaLimit)
			{
				fromValue -= GlobalConfig.Instance.ChallengeTaiwuVillageBaseSpaceReduce;
				fromValue -= levelIndex;
			}
			this.nameText.text = config.Name;
			int toValue = config.LevelEffect.CheckIndex(levelIndex + 1) ? config.LevelEffect[levelIndex + 1] : 0;
			bool flag = toValue > 0 && buildingAreaLimit;
			if (flag)
			{
				toValue -= GlobalConfig.Instance.ChallengeTaiwuVillageBaseSpaceReduce;
				toValue -= levelIndex + 1;
			}
			this.Set(fromValue, toValue, currLevel < maxLevel);
		}

		// Token: 0x0600984F RID: 38991 RVA: 0x0046F804 File Offset: 0x0046DA04
		public void Set(BuildingScaleItem config, int currLevel, bool buildingAreaLimit)
		{
			int levelIndex = currLevel - 1;
			int fromValue = config.LevelEffect[levelIndex];
			if (buildingAreaLimit)
			{
				fromValue -= GlobalConfig.Instance.ChallengeTaiwuVillageBaseSpaceReduce;
				fromValue -= levelIndex;
			}
			this.nameText.text = config.Name;
			this.Set(fromValue);
		}

		// Token: 0x06009850 RID: 38992 RVA: 0x0046F858 File Offset: 0x0046DA58
		public void Set(int fromValue, int toValue, bool canUpgrade)
		{
			this.prevText.gameObject.SetActive(canUpgrade);
			this.arrowImage.gameObject.SetActive(canUpgrade);
			this.currText.text = toValue.ToString().SetColor(canUpgrade ? "brightblue" : "pinkyellow");
			if (canUpgrade)
			{
				this.prevText.text = fromValue.ToString();
			}
		}

		// Token: 0x06009851 RID: 38993 RVA: 0x0046F8CC File Offset: 0x0046DACC
		public void Set(int fromValue)
		{
			this.prevText.gameObject.SetActive(true);
			this.arrowImage.gameObject.SetActive(true);
			this.currText.text = "/".SetColor("pinkyellow");
			this.prevText.text = fromValue.ToString();
		}

		// Token: 0x04007524 RID: 29988
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x04007525 RID: 29989
		[SerializeField]
		private TextMeshProUGUI prevText;

		// Token: 0x04007526 RID: 29990
		[SerializeField]
		private CImage arrowImage;

		// Token: 0x04007527 RID: 29991
		[SerializeField]
		private TextMeshProUGUI currText;

		// Token: 0x04007528 RID: 29992
		private const string EMPTY_PLACEHOLDER = "/";
	}
}
