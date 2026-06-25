using System;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using TMPro;
using UnityEngine;

namespace Game.Components.ListStyleGeneralScroll.CellContent
{
	// Token: 0x02000EC3 RID: 3779
	public class LegendaryBookFeatureCell : MonoBehaviour, ICellContent<LegendaryBookFeatureCellData>, ICellContent
	{
		// Token: 0x0600AEFA RID: 44794 RVA: 0x004FB860 File Offset: 0x004F9A60
		public void SetData(LegendaryBookFeatureCellData data)
		{
			this.mouseTip.Type = TipType.Feature;
			TooltipInvoker tooltipInvoker = this.mouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.mouseTip.RuntimeParam.Set("FeatureId", data.FeatureId);
			this.mouseTip.RuntimeParam.Set("CharacterId", data.CharacterId);
			CharacterFeatureItem config = CharacterFeature.Instance[data.FeatureId];
			this.featureName.text = config.Name;
			bool flag = this.levelImages == null || config == null;
			if (!flag)
			{
				int indexMedal = 0;
				for (int i = 0; i < 3; i++)
				{
					FeatureMedals medals = config.FeatureMedals[i];
					foreach (sbyte medalType in medals.Values)
					{
						bool flag2 = indexMedal >= this.levelImages.Length;
						if (flag2)
						{
							break;
						}
						CImage medalImage = this.levelImages[indexMedal++];
						bool flag3 = medalImage == null;
						if (flag3)
						{
							break;
						}
						medalImage.gameObject.SetActive(true);
						medalImage.SetSprite("ui9_icon_strategy_big_" + LegendaryBookFeatureCell.FeatureIconConfig[(int)medalType][i], false, null);
					}
				}
				while (indexMedal < this.levelImages.Length)
				{
					bool flag4 = this.levelImages[indexMedal] != null;
					if (flag4)
					{
						this.levelImages[indexMedal].gameObject.SetActive(false);
					}
					indexMedal++;
				}
			}
		}

		// Token: 0x04008763 RID: 34659
		[SerializeField]
		private CImage[] levelImages;

		// Token: 0x04008764 RID: 34660
		[SerializeField]
		private TextMeshProUGUI featureName;

		// Token: 0x04008765 RID: 34661
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04008766 RID: 34662
		private static readonly string[][] FeatureIconConfig = new string[][]
		{
			new string[]
			{
				"0_2",
				"1_2",
				"3_2"
			},
			new string[]
			{
				"0_1",
				"1_1",
				"3_1"
			},
			new string[]
			{
				"0_0",
				"1_0",
				"3_0"
			},
			new string[]
			{
				"0_3",
				"1_3",
				"3_3"
			}
		};
	}
}
