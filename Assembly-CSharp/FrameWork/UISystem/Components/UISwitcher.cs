using System;
using EasyButtons;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UISystem.Components
{
	// Token: 0x0200102B RID: 4139
	public class UISwitcher : MonoBehaviour
	{
		// Token: 0x0600BD42 RID: 48450 RVA: 0x0055FC60 File Offset: 0x0055DE60
		[Button]
		public void Switch(bool on)
		{
			foreach (UISwitcher subSwitcher in this.subSwitchers)
			{
				subSwitcher.Switch(on);
			}
			foreach (GameObject go in this.onObjects)
			{
				go.SetActive(on);
			}
			foreach (GameObject go2 in this.offObjects)
			{
				go2.SetActive(!on);
			}
			foreach (Behaviour behaviour in this.onBehaviours)
			{
				behaviour.enabled = on;
			}
			foreach (Behaviour behaviour2 in this.offBehaviours)
			{
				behaviour2.enabled = !on;
			}
			foreach (Button button in this.buttons)
			{
				button.interactable = on;
			}
			foreach (UISwitcher.ImageData data in this.images)
			{
				data.target.sprite = (on ? data.on : data.off);
			}
			foreach (UISwitcher.GraphicData data2 in this.graphics)
			{
				data2.target.color = Colors.Instance[on ? data2.onColor : data2.offColor];
			}
			foreach (UISwitcher.MouseTipData data3 in this.mouseTips)
			{
				data3.target.PresetParam[data3.presetIndex] = (on ? data3.onLanguageKey : data3.offLanguageKey);
			}
			foreach (UISwitcher.PaddingData data4 in this.paddings)
			{
				bool leftToRight = data4.leftToRight;
				if (leftToRight)
				{
					data4.target.padding.left = (on ? data4.offset.left : data4.offset.right);
					data4.target.padding.right = (on ? data4.offset.right : data4.offset.left);
				}
				bool topToBottom = data4.topToBottom;
				if (topToBottom)
				{
					data4.target.padding.top = (on ? data4.offset.top : data4.offset.bottom);
					data4.target.padding.bottom = (on ? data4.offset.bottom : data4.offset.top);
				}
			}
			foreach (UISwitcher.ScaleData data5 in this.scales)
			{
				data5.target.localScale = data5.target.localScale.SetX((float)((on == data5.onIsReverse) ? -1 : 1));
			}
			foreach (UISwitcher.AnchorAndPivotData data6 in this.anchorAndPivotsX)
			{
				int x = (on == data6.onIsLeftOrTop) ? 0 : 1;
				data6.target.anchorMin = data6.target.anchorMin.SetX((float)x);
				data6.target.anchorMax = data6.target.anchorMax.SetX((float)x);
				data6.target.pivot = data6.target.pivot.SetX((float)x);
				data6.target.anchoredPosition = data6.position.SetX(data6.position.x * (float)((on == data6.onIsLeftOrTop) ? 1 : -1));
			}
		}

		// Token: 0x040091AA RID: 37290
		public UISwitcher[] subSwitchers;

		// Token: 0x040091AB RID: 37291
		public GameObject[] onObjects;

		// Token: 0x040091AC RID: 37292
		public GameObject[] offObjects;

		// Token: 0x040091AD RID: 37293
		public Behaviour[] onBehaviours;

		// Token: 0x040091AE RID: 37294
		public Behaviour[] offBehaviours;

		// Token: 0x040091AF RID: 37295
		public Button[] buttons;

		// Token: 0x040091B0 RID: 37296
		public UISwitcher.ImageData[] images;

		// Token: 0x040091B1 RID: 37297
		public UISwitcher.GraphicData[] graphics;

		// Token: 0x040091B2 RID: 37298
		public UISwitcher.MouseTipData[] mouseTips;

		// Token: 0x040091B3 RID: 37299
		public UISwitcher.PaddingData[] paddings;

		// Token: 0x040091B4 RID: 37300
		public UISwitcher.ScaleData[] scales;

		// Token: 0x040091B5 RID: 37301
		public UISwitcher.AnchorAndPivotData[] anchorAndPivotsX;

		// Token: 0x02002674 RID: 9844
		[Serializable]
		public class ImageData
		{
			// Token: 0x0400EABF RID: 60095
			public Image target;

			// Token: 0x0400EAC0 RID: 60096
			public Sprite on;

			// Token: 0x0400EAC1 RID: 60097
			public Sprite off;
		}

		// Token: 0x02002675 RID: 9845
		[Serializable]
		public class GraphicData
		{
			// Token: 0x0400EAC2 RID: 60098
			public Graphic target;

			// Token: 0x0400EAC3 RID: 60099
			public string onColor;

			// Token: 0x0400EAC4 RID: 60100
			public string offColor;
		}

		// Token: 0x02002676 RID: 9846
		[Serializable]
		public class MouseTipData
		{
			// Token: 0x0400EAC5 RID: 60101
			public TooltipInvoker target;

			// Token: 0x0400EAC6 RID: 60102
			public int presetIndex;

			// Token: 0x0400EAC7 RID: 60103
			public string onLanguageKey;

			// Token: 0x0400EAC8 RID: 60104
			public string offLanguageKey;
		}

		// Token: 0x02002677 RID: 9847
		[Serializable]
		public class PaddingData
		{
			// Token: 0x0400EAC9 RID: 60105
			public LayoutGroup target;

			// Token: 0x0400EACA RID: 60106
			public bool leftToRight;

			// Token: 0x0400EACB RID: 60107
			public bool topToBottom;

			// Token: 0x0400EACC RID: 60108
			public RectOffset offset;
		}

		// Token: 0x02002678 RID: 9848
		[Serializable]
		public class ScaleData
		{
			// Token: 0x0400EACD RID: 60109
			public RectTransform target;

			// Token: 0x0400EACE RID: 60110
			public bool onIsReverse;
		}

		// Token: 0x02002679 RID: 9849
		[Serializable]
		public class AnchorAndPivotData
		{
			// Token: 0x0400EACF RID: 60111
			public RectTransform target;

			// Token: 0x0400EAD0 RID: 60112
			public bool onIsLeftOrTop;

			// Token: 0x0400EAD1 RID: 60113
			public Vector2 position;
		}
	}
}
