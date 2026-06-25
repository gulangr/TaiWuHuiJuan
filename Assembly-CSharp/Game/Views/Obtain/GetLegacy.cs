using System;
using Config;
using TMPro;
using UnityEngine;

namespace Game.Views.Obtain
{
	// Token: 0x020007D7 RID: 2007
	public class GetLegacy : MonoBehaviour
	{
		// Token: 0x060061ED RID: 25069 RVA: 0x002CEAB0 File Offset: 0x002CCCB0
		public void Set(short templateId, bool isFree)
		{
			LegacyItem config = Legacy.Instance[templateId];
			this.legacyName.text = config.Name;
			this.icon.SetSprite(config.Icon, false, null);
			this.grade.SetColor(Colors.Instance.GradeColors[(int)config.Grade]);
			this.cost.text = (isFree ? "0" : config.Cost.ToString());
			this.tips.PresetParam = new string[2];
			this.tips.PresetParam[0] = config.Name;
			this.tips.PresetParam[1] = config.Desc;
		}

		// Token: 0x04004402 RID: 17410
		[SerializeField]
		private TextMeshProUGUI legacyName;

		// Token: 0x04004403 RID: 17411
		[SerializeField]
		private CImage grade;

		// Token: 0x04004404 RID: 17412
		[SerializeField]
		private CImage icon;

		// Token: 0x04004405 RID: 17413
		[SerializeField]
		private TextMeshProUGUI cost;

		// Token: 0x04004406 RID: 17414
		[SerializeField]
		private TooltipInvoker tips;
	}
}
