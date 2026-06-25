using System;
using FrameWork;
using GameData.Adventure;
using GameData.Domains.Adventure;
using TMPro;
using UnityEngine;

namespace Game.Views.Adventure
{
	// Token: 0x02000C6F RID: 3183
	public class AdventureRemakeParameterItem : MonoBehaviour
	{
		// Token: 0x0600A1F2 RID: 41458 RVA: 0x004BB448 File Offset: 0x004B9648
		public void SetValue(AdventureParameterData parameterData, AdventureParameterValue parameterValue, bool isGlobal, bool isLast = false)
		{
			bool isState = parameterData.Type == EAdventureParameterType.State;
			bool notForeverState = parameterValue.Max >= 0;
			this.paramName.SetText(parameterData.Name.ColorReplace(), true);
			this.foreverProgress.gameObject.SetActive(!notForeverState);
			this.progress.enabled = isState;
			this.progressBg.enabled = isState;
			CImage cimage = this.verticalLine;
			if (cimage != null)
			{
				cimage.gameObject.SetActive(isState);
			}
			bool flag = notForeverState;
			if (flag)
			{
				string text;
				if (!isState)
				{
					int num = parameterValue.Current;
					text = num.ToString();
				}
				else
				{
					text = ((parameterData.Style == 2) ? (parameterValue.Max - parameterValue.Current) : parameterValue.Current).ToString() + "/" + parameterValue.Max.ToString();
				}
				string valueStr = text;
				this.paramValue.SetText(valueStr, true);
				this.progress.fillAmount = ((parameterData.Style == 2) ? (1f - parameterValue.AsProgress) : parameterValue.AsProgress);
			}
			else
			{
				this.progress.fillAmount = 1f;
				this.paramValue.SetText("∞", true);
			}
			TooltipInvoker tooltipInvoker = this.mouseTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.mouseTip.Type = TipType.Simple;
			this.mouseTip.RuntimeParam.Set("arg0", parameterData.Name.RemoveColor());
			this.mouseTip.RuntimeParam.Set("arg1", parameterData.Desc);
			bool flag2 = this.line != null;
			if (flag2)
			{
				this.line.SetActive(isGlobal && !isLast);
			}
			if (isGlobal)
			{
				bool flag3 = !string.IsNullOrEmpty(parameterData.Icon);
				if (flag3)
				{
					this.icon.SetSprite(ViewAdventureRemake.GetElementParamStateIconName(parameterData.Icon, false), false, null);
					this.icon.gameObject.SetActive(this.icon.enabled);
				}
				else
				{
					this.icon.gameObject.SetActive(false);
				}
			}
			else
			{
				this.icon.SetSprite(ViewAdventureRemake.GetElementParamStateIconName(parameterData.Icon, false), false, null);
			}
		}

		// Token: 0x04007DE5 RID: 32229
		[SerializeField]
		private TextMeshProUGUI paramName;

		// Token: 0x04007DE6 RID: 32230
		[SerializeField]
		private TextMeshProUGUI paramValue;

		// Token: 0x04007DE7 RID: 32231
		[SerializeField]
		private CImage icon;

		// Token: 0x04007DE8 RID: 32232
		[SerializeField]
		private CImage progress;

		// Token: 0x04007DE9 RID: 32233
		[SerializeField]
		private CImage progressBg;

		// Token: 0x04007DEA RID: 32234
		[SerializeField]
		private CImage foreverProgress;

		// Token: 0x04007DEB RID: 32235
		[SerializeField]
		private CImage verticalLine;

		// Token: 0x04007DEC RID: 32236
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04007DED RID: 32237
		[SerializeField]
		private GameObject line;
	}
}
