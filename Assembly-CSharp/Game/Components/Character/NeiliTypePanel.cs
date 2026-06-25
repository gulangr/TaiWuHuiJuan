using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using GameData.Domains.Character;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F3C RID: 3900
	public class NeiliTypePanel : MonoBehaviour
	{
		// Token: 0x0600B346 RID: 45894 RVA: 0x00519BDC File Offset: 0x00517DDC
		public void Init()
		{
			for (sbyte type = 0; type < 6; type += 1)
			{
				NeiliTypeItem config = NeiliType.Instance[type];
				Transform obj = this.highlightNode.GetChild((int)type);
				bool isBuff = config.ColorType == 1;
				obj.GetComponent<TooltipInvoker>().RuntimeParam = new ArgumentBox().Set("neiliType", (int)type);
				obj.GetComponent<TooltipInvoker>().Refresh(false, -1);
				TMP_Text component = obj.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
				string name = config.Name;
				component.text = name.Substring(3, name.Length - 3);
				obj.GetComponent<CImage>().SetSprite(isBuff ? "ui9_icon_five_elements_highlight_big_1" : "ui9_icon_five_elements_highlight_big_0", false, null);
				bool flag = type != 5;
				if (flag)
				{
					obj.GetChild(0).GetComponent<CImage>().SetSprite(isBuff ? "ui9_back_five_elements_frame_1" : "ui9_back_five_elements_frame_0", false, null);
				}
			}
		}

		// Token: 0x0600B347 RID: 45895 RVA: 0x00519CD0 File Offset: 0x00517ED0
		public void Set(sbyte type)
		{
			NeiliTypeItem config = NeiliType.Instance[type];
			for (sbyte nodeType = 0; nodeType < 6; nodeType += 1)
			{
				this.highlightNode.GetChild((int)nodeType).gameObject.SetActive(nodeType == config.TemplateId);
			}
			bool flag = type < 6;
			if (flag)
			{
				this.blueLine.gameObject.SetActive(false);
				this.redLine.gameObject.SetActive(false);
				this.highlightLine.gameObject.SetActive(false);
			}
			else
			{
				bool showLine = config.LinePos != null;
				bool isBuff = config.ColorType == 1;
				this.blueLine.gameObject.SetActive(showLine && isBuff);
				this.redLine.gameObject.SetActive(showLine && !isBuff);
				bool flag2 = showLine;
				if (flag2)
				{
					Transform neiliLine = isBuff ? this.blueLine : this.redLine;
					neiliLine.GetComponent<RectTransform>().anchoredPosition = new Vector2((float)config.LinePos[0], (float)config.LinePos[1]);
					neiliLine.localRotation = Quaternion.Euler(0f, 0f, (float)config.LineAngle);
				}
				RectTransform iconRectTransform = this.highlightLine.GetComponent<RectTransform>();
				iconRectTransform.anchoredPosition = new Vector2((float)config.TypeIconPos[0], (float)config.TypeIconPos[1]);
				bool isRight = iconRectTransform.anchoredPosition.x >= 0f;
				Transform use = isRight ? this.highlightLine.GetChild(0) : this.highlightLine.GetChild(1);
				Transform unUse = isRight ? this.highlightLine.GetChild(1) : this.highlightLine.GetChild(0);
				this.highlightLine.GetComponent<CImage>().SetSprite(isBuff ? "ui9_icon_five_elements_highlight_small_1" : "ui9_icon_five_elements_highlight_small_0", false, null);
				this.highlightLine.GetComponent<TooltipInvoker>().RuntimeParam = new ArgumentBox().Set("neiliType", (int)type);
				this.highlightLine.GetComponent<TooltipInvoker>().Refresh(false, -1);
				use.GetComponent<CImage>().SetSprite(isBuff ? "ui9_back_five_elements_frame_1" : "ui9_back_five_elements_frame_0", false, null);
				int length;
				bool hasPrefix = this._languageNeiliTypeNamePrefixLength.TryGetValue(LocalStringManager.CurLanguageType, out length);
				TMP_Text component = use.GetChild(0).GetComponent<TextMeshProUGUI>();
				string text;
				if (!hasPrefix)
				{
					text = config.Name;
				}
				else
				{
					string name = config.Name;
					int num = length;
					text = name.Substring(num, name.Length - num);
				}
				component.text = text;
				unUse.gameObject.SetActive(false);
				use.gameObject.SetActive(true);
				this.highlightLine.gameObject.SetActive(true);
			}
		}

		// Token: 0x0600B348 RID: 45896 RVA: 0x00519F80 File Offset: 0x00518180
		public unsafe void Set(NeiliProportionOfFiveElements currValue, sbyte transferType, sbyte dstType, int amount)
		{
			StringBuilder sb = new StringBuilder();
			for (sbyte i = 0; i < 5; i += 1)
			{
				Transform obj = this.fiveElementsValue.GetChild((int)i);
				sb.Clear();
				sb.Append(*currValue[(int)i]);
				sb.Append('%');
				bool flag = amount != 0;
				if (flag)
				{
					bool flag2 = transferType == i;
					if (flag2)
					{
						sb.Append(string.Format("-{0}%", amount).SetColor("darkred"));
					}
					else
					{
						bool flag3 = dstType == i;
						if (flag3)
						{
							sb.Append(string.Format("+{0}%", amount).SetColor("lightblue"));
						}
					}
				}
				obj.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(sb.ToString(), true);
				obj.GetChild(1).gameObject.SetActive(amount != 0 && (transferType == i || dstType == i));
			}
		}

		// Token: 0x04008B39 RID: 35641
		public Transform redLine;

		// Token: 0x04008B3A RID: 35642
		public Transform blueLine;

		// Token: 0x04008B3B RID: 35643
		public Transform highlightNode;

		// Token: 0x04008B3C RID: 35644
		public Transform highlightLine;

		// Token: 0x04008B3D RID: 35645
		public Transform fiveElementsValue;

		// Token: 0x04008B3E RID: 35646
		private readonly Dictionary<LocalStringManager.LanguageType, int> _languageNeiliTypeNamePrefixLength = new Dictionary<LocalStringManager.LanguageType, int>
		{
			{
				LocalStringManager.LanguageType.CN,
				3
			},
			{
				LocalStringManager.LanguageType.CNH,
				3
			},
			{
				LocalStringManager.LanguageType.KO,
				3
			}
		};
	}
}
