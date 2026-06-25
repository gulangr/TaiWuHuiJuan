using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000055 RID: 85
[ExecuteInEditMode]
public class CurvedText : MonoBehaviour
{
	// Token: 0x060002D6 RID: 726 RVA: 0x00011247 File Offset: 0x0000F447
	private void Awake()
	{
		this._textComponent = base.gameObject.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x0001125B File Offset: 0x0000F45B
	private void OnEnable()
	{
		this.UpdateDisplay();
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x00011268 File Offset: 0x0000F468
	public void UpdateDisplay()
	{
		this._textComponent.ForceMeshUpdate(false, false);
		TMP_TextInfo textInfo = this._textComponent.textInfo;
		int characterCount = textInfo.characterCount;
		bool flag = characterCount == 0;
		if (!flag)
		{
			float degree;
			bool flag2 = this._charCountSetting && CurvedText._degreeSettingDict.TryGetValue(characterCount, out degree);
			if (flag2)
			{
				this._arcDegrees = degree;
			}
			float boundsMinX = this._textComponent.bounds.min.x;
			float boundsMaxX = this._textComponent.bounds.max.x;
			float arcDegree = Mathf.Min(this._arcDegrees, (float)(textInfo.characterCount / textInfo.lineCount) * this._maxDegreePerLetter);
			for (int i = 0; i < characterCount; i++)
			{
				bool flag3 = !textInfo.characterInfo[i].isVisible;
				if (!flag3)
				{
					int vertexIndex = textInfo.characterInfo[i].vertexIndex;
					int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
					Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
					Vector3 charMidBaselinePos = new Vector2((vertices[vertexIndex].x + vertices[vertexIndex + 2].x) / 2f, textInfo.characterInfo[i].baseLine);
					vertices[vertexIndex] += -charMidBaselinePos;
					vertices[vertexIndex + 1] += -charMidBaselinePos;
					vertices[vertexIndex + 2] += -charMidBaselinePos;
					vertices[vertexIndex + 3] += -charMidBaselinePos;
					float charPosRatio = (charMidBaselinePos.x - boundsMinX) / (boundsMaxX - boundsMinX) - 0.5f;
					float charRadians = (arcDegree * charPosRatio - 90f + this._angularOffset) * 0.017453292f;
					float x = Mathf.Cos(charRadians);
					float y = Mathf.Sin(charRadians);
					float radiusForThisLine = this._radius - textInfo.lineInfo[0].lineExtents.max.y * (float)textInfo.characterInfo[i].lineNumber;
					Vector2 newMidBaselinePos = new Vector2(x * radiusForThisLine, -y * radiusForThisLine);
					Vector3 pos = new Vector3(newMidBaselinePos.x + this._posOffset.x, newMidBaselinePos.y + this._posOffset.y);
					Quaternion rotate = Quaternion.AngleAxis(-Mathf.Atan2(y, x) * 57.29578f - 90f, Vector3.forward);
					Matrix4x4 matrix = Matrix4x4.TRS(pos, rotate, Vector3.one);
					vertices[vertexIndex] = matrix.MultiplyPoint3x4(vertices[vertexIndex]);
					vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
					vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
					vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);
				}
			}
			this._textComponent.UpdateVertexData();
		}
	}

	// Token: 0x0400017F RID: 383
	private TextMeshProUGUI _textComponent;

	// Token: 0x04000180 RID: 384
	[SerializeField]
	[Tooltip("文本环绕的圆的半径")]
	private float _radius = 10f;

	// Token: 0x04000181 RID: 385
	[SerializeField]
	[Tooltip("圆弧角度")]
	private float _arcDegrees = 90f;

	// Token: 0x04000182 RID: 386
	[SerializeField]
	[Tooltip("旋转角度偏移")]
	private float _angularOffset = 0f;

	// Token: 0x04000183 RID: 387
	[SerializeField]
	[Tooltip("每个字符可占用的最大角度")]
	private float _maxDegreePerLetter = 360f;

	// Token: 0x04000184 RID: 388
	[SerializeField]
	[Tooltip("位置偏移")]
	private Vector2 _posOffset;

	// Token: 0x04000185 RID: 389
	[SerializeField]
	[Tooltip("是否根据字数调整设置，目前用于卷轴上的文本")]
	private bool _charCountSetting;

	// Token: 0x04000186 RID: 390
	private static Dictionary<int, float> _degreeSettingDict = new Dictionary<int, float>
	{
		{
			2,
			16f
		},
		{
			3,
			22f
		},
		{
			4,
			29f
		}
	};
}
