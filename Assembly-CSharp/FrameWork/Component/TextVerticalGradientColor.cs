using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FrameWork.Component
{
	// Token: 0x02000FEC RID: 4076
	[ExecuteAlways]
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class TextVerticalGradientColor : MonoBehaviour
	{
		// Token: 0x0600BA13 RID: 47635 RVA: 0x0054BE71 File Offset: 0x0054A071
		private void Awake()
		{
			this._textComponent = base.GetComponent<TextMeshProUGUI>();
			this._textComponent.OnPreRenderText += this.OnPreRenderText;
		}

		// Token: 0x0600BA14 RID: 47636 RVA: 0x0054BE98 File Offset: 0x0054A098
		private void OnPreRenderText(TMP_TextInfo info)
		{
			List<float> yPosList = EasyPool.Get<List<float>>();
			foreach (TMP_MeshInfo tmpMeshInfo in info.meshInfo)
			{
				for (int i = 0; i < tmpMeshInfo.vertexCount; i++)
				{
					yPosList.Add(tmpMeshInfo.vertices[i].y);
				}
			}
			float[] yPosArray = yPosList.ToArray();
			float yMax = Mathf.Max(yPosArray);
			float yMin = Mathf.Min(yPosArray);
			float yCenter = yMin + (yMax - yMin) * this.CenterValue;
			sbyte s = 0;
			while ((int)s < info.meshInfo.Length)
			{
				TMP_MeshInfo tmpMeshInfo2 = info.meshInfo[(int)s];
				for (int j = 0; j < tmpMeshInfo2.vertexCount; j++)
				{
					float iYPos = tmpMeshInfo2.vertices[j].y;
					bool flag = iYPos < yCenter;
					Color32 iColor;
					if (flag)
					{
						float stepValue = (yCenter - iYPos) / (yCenter - yMin);
						iColor = Color32.Lerp(this.CenterColor, this.BottomColor, stepValue);
					}
					else
					{
						float stepValue2 = (iYPos - yCenter) / (yMax - yCenter);
						iColor = Color32.Lerp(this.CenterColor, this.TopColor, stepValue2);
					}
					tmpMeshInfo2.colors32[j] = iColor;
				}
				info.meshInfo[(int)s] = tmpMeshInfo2;
				s += 1;
			}
		}

		// Token: 0x04008FDC RID: 36828
		[Range(0f, 1f)]
		public float CenterValue;

		// Token: 0x04008FDD RID: 36829
		public Color32 TopColor = Color.white;

		// Token: 0x04008FDE RID: 36830
		public Color32 CenterColor = Color.green;

		// Token: 0x04008FDF RID: 36831
		public Color32 BottomColor = Color.blue;

		// Token: 0x04008FE0 RID: 36832
		private TextMeshProUGUI _textComponent;
	}
}
