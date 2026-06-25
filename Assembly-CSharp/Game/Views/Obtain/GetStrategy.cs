using System;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;

namespace Game.Views.Obtain
{
	// Token: 0x020007D9 RID: 2009
	public class GetStrategy : MonoBehaviour
	{
		// Token: 0x060061F1 RID: 25073 RVA: 0x002CEBB8 File Offset: 0x002CCDB8
		public void Set(short templateId)
		{
			DebateStrategyItem config = DebateStrategy.Instance[templateId];
			this.strategyName.text = config.Name;
			this.icon.SetSprite(config.Image, false, null);
			this.cost.SetText(config.UsedCost.ToString(), true);
			TooltipInvoker tooltipInvoker = this.tips;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			this.tips.RuntimeParam.Set("TemplateId", templateId);
		}

		// Token: 0x0400440B RID: 17419
		[SerializeField]
		private TextMeshProUGUI strategyName;

		// Token: 0x0400440C RID: 17420
		[SerializeField]
		private CImage icon;

		// Token: 0x0400440D RID: 17421
		[SerializeField]
		private TextMeshProUGUI cost;

		// Token: 0x0400440E RID: 17422
		[SerializeField]
		private TooltipInvoker tips;
	}
}
