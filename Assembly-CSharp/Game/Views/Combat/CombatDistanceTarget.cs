using System;
using FrameWork;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B08 RID: 2824
	public class CombatDistanceTarget : MonoBehaviour
	{
		// Token: 0x06008AE7 RID: 35559 RVA: 0x004045F4 File Offset: 0x004027F4
		public void Refresh(float scale, short distance)
		{
			bool invalidDistance = distance < 0;
			base.gameObject.SetActive(!invalidDistance);
			bool flag = invalidDistance;
			if (!flag)
			{
				RectTransform rectTransform = (RectTransform)base.transform;
				float pos = (float)(distance * 5) * scale;
				pos = (this.reserve ? (-pos) : pos);
				rectTransform.anchoredPosition = rectTransform.anchoredPosition.SetX(pos);
				string distanceStr = ((float)distance / 10f).ToString("F1");
				this.distanceText.text = distanceStr;
				bool flag2 = !this.updateTips;
				if (!flag2)
				{
					TooltipInvoker mouseTip = base.GetComponent<TooltipInvoker>();
					mouseTip.Type = TipType.SimpleWithHotkeyDisplay;
					TooltipInvoker tooltipInvoker = mouseTip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = new ArgumentBox();
					}
					mouseTip.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_Combat_TargetDistance));
					mouseTip.RuntimeParam.Set("arg1", LocalStringManager.GetFormat(LanguageKey.LK_Combat_TargetDistance_Tips, distanceStr));
					mouseTip.RuntimeParam.Set("HotkeyDisplayId", 4);
					mouseTip.Refresh(false, -1);
				}
			}
		}

		// Token: 0x04006A86 RID: 27270
		public bool reserve;

		// Token: 0x04006A87 RID: 27271
		public bool updateTips;

		// Token: 0x04006A88 RID: 27272
		public TextMeshProUGUI distanceText;
	}
}
