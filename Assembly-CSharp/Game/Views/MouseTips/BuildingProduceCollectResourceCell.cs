using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Building;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000821 RID: 2081
	public class BuildingProduceCollectResourceCell : MonoBehaviour
	{
		// Token: 0x06006641 RID: 26177 RVA: 0x002EB360 File Offset: 0x002E9560
		public void Set(KeyValuePair<BuildingBlockKey, BuildingProduceDependencyData> pair)
		{
			BuildingBlockItem dependConfig = BuildingBlock.Instance[pair.Value.TemplateId];
			this.title.text = dependConfig.Name;
			this.level.text = LanguageKey.LK_Mousetip_Building_PredictProduct_Tips_Line_1.TrFormat(pair.Value.Level.ToString()).ColorReplace();
			this.produce.text = LanguageKey.LK_Mousetip_Building_PredictProduct_Tips_Line_2.TrFormat(pair.Value.ResourceSingleOutputValuation).ColorReplace();
			this.baseProd.text = LanguageKey.LK_Mousetip_Building_PredictProduct_Tips_Line_4.TrFormat(pair.Value.ResourceYieldLevelFactor).ColorReplace();
			this.resourceModifier.SetPercentageText(LanguageKey.LK_Mousetip_Building_PredictProduct_Tips_Line_5, pair.Value.BlockBaseYieldFactor);
			this.buildingModifier.SetPercentageText(LanguageKey.LK_Mousetip_Building_PredictProduct_Tips_Line_6, pair.Value.ProductivityFactor);
			this.efficient.SetPercentageText(LanguageKey.LK_Mousetip_Building_PredictProduct_Tips_Line_7, pair.Value.TotalAttainmentFactor);
			this.difficultyModifier.SetPercentageText(LanguageKey.LK_Mousetip_Building_PredictProduct_Tips_Line_8, pair.Value.GainResourcePercentFactor);
		}

		// Token: 0x04004784 RID: 18308
		[SerializeField]
		private TMP_Text title;

		// Token: 0x04004785 RID: 18309
		[SerializeField]
		private TMP_Text level;

		// Token: 0x04004786 RID: 18310
		[SerializeField]
		private TMP_Text produce;

		// Token: 0x04004787 RID: 18311
		[SerializeField]
		private TMP_Text baseProd;

		// Token: 0x04004788 RID: 18312
		[SerializeField]
		private TMP_Text resourceModifier;

		// Token: 0x04004789 RID: 18313
		[SerializeField]
		private TMP_Text buildingModifier;

		// Token: 0x0400478A RID: 18314
		[SerializeField]
		private TMP_Text efficient;

		// Token: 0x0400478B RID: 18315
		[SerializeField]
		private TMP_Text difficultyModifier;
	}
}
