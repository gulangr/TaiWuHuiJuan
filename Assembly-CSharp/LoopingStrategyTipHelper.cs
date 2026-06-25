using System;
using Config;
using FrameWork;
using UnityEngine;

// Token: 0x02000252 RID: 594
public class LoopingStrategyTipHelper : MonoBehaviour
{
	// Token: 0x06002757 RID: 10071 RVA: 0x00122180 File Offset: 0x00120380
	public void Refresh(sbyte strategyId)
	{
		bool flag = strategyId == -1;
		if (!flag)
		{
			TooltipInvoker tipDisplayer = base.GetComponent<TooltipInvoker>();
			tipDisplayer.Type = TipType.Simple;
			QiArtStrategyItem config = QiArtStrategy.Instance[strategyId];
			TooltipInvoker tooltipInvoker = tipDisplayer;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tipDisplayer.RuntimeParam.Set("arg0", config.Name);
			tipDisplayer.RuntimeParam.Set("arg1", config.Desc);
			tipDisplayer.Refresh(false, -1);
		}
	}
}
