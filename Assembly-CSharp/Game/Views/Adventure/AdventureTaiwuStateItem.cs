using System;
using FrameWork;
using GameData.Adventure;
using GameData.Domains.Adventure;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C71 RID: 3185
	public class AdventureTaiwuStateItem : MonoBehaviour
	{
		// Token: 0x0600A1F7 RID: 41463 RVA: 0x004BB71C File Offset: 0x004B991C
		public void SetValue(AdventureParameterData parameterData, AdventureParameterValue parameterValue)
		{
			TooltipInvoker tooltipInvoker = this.mouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.mouseTip.Type = TipType.Simple;
			this.mouseTip.RuntimeParam.Set("arg0", parameterData.Name);
			this.mouseTip.RuntimeParam.Set("arg1", parameterData.Desc);
			this.paramStateBg.SetSprite(ViewAdventureRemake.GetElementParamStateIconName(parameterData.Icon, true), false, null);
			this.paramStateProgress.SetSprite(ViewAdventureRemake.GetElementParamStateIconName(parameterData.Icon, false), false, null);
			this.paramStateProgress.fillAmount = parameterValue.AsProgress;
		}

		// Token: 0x04007DF1 RID: 32241
		[SerializeField]
		private CImage paramStateBg;

		// Token: 0x04007DF2 RID: 32242
		[SerializeField]
		private CImage paramStateProgress;

		// Token: 0x04007DF3 RID: 32243
		[SerializeField]
		private TooltipInvoker mouseTip;
	}
}
