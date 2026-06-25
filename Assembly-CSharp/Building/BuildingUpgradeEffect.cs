using System;
using Config;
using TMPro;
using UnityEngine;

namespace Building
{
	// Token: 0x02000663 RID: 1635
	public class BuildingUpgradeEffect : MonoBehaviour
	{
		// Token: 0x06004DB6 RID: 19894 RVA: 0x00249FF4 File Offset: 0x002481F4
		public void Set(BuildingScaleItem config, int currLevel, int maxLevel)
		{
			int levelIndex = currLevel - 1;
			int fromValue = config.LevelEffect[levelIndex];
			this.nameText.text = config.Name;
			int toValue = config.LevelEffect.CheckIndex(levelIndex + 1) ? config.LevelEffect[levelIndex + 1] : 0;
			this.Set(fromValue, toValue, currLevel < maxLevel);
		}

		// Token: 0x06004DB7 RID: 19895 RVA: 0x0024A054 File Offset: 0x00248254
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

		// Token: 0x040035E7 RID: 13799
		[SerializeField]
		private TextMeshProUGUI nameText;

		// Token: 0x040035E8 RID: 13800
		[SerializeField]
		private TextMeshProUGUI prevText;

		// Token: 0x040035E9 RID: 13801
		[SerializeField]
		private CImage arrowImage;

		// Token: 0x040035EA RID: 13802
		[SerializeField]
		private TextMeshProUGUI currText;
	}
}
