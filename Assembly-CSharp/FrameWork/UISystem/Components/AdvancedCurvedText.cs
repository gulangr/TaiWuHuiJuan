using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001010 RID: 4112
	[ExecuteInEditMode]
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class AdvancedCurvedText : MonoBehaviour
	{
		// Token: 0x0600BC05 RID: 48133 RVA: 0x0055813E File Offset: 0x0055633E
		private void OnEnable()
		{
			this._textComponent = base.GetComponent<TextMeshProUGUI>();
			this._textComponent.RegisterDirtyVerticesCallback(new UnityAction(this.MarkDirty));
			this.MarkDirty();
		}

		// Token: 0x0600BC06 RID: 48134 RVA: 0x0055816C File Offset: 0x0055636C
		private void OnDisable()
		{
			bool flag = this._textComponent != null;
			if (flag)
			{
				this._textComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.MarkDirty));
			}
		}

		// Token: 0x0600BC07 RID: 48135 RVA: 0x005581A4 File Offset: 0x005563A4
		private void LateUpdate()
		{
			bool isDirty = this._isDirty;
			if (isDirty)
			{
				this.UpdateDisplay();
				this._isDirty = false;
			}
		}

		// Token: 0x0600BC08 RID: 48136 RVA: 0x005581CC File Offset: 0x005563CC
		private void MarkDirty()
		{
			this._isDirty = true;
		}

		// Token: 0x0600BC09 RID: 48137 RVA: 0x005581D8 File Offset: 0x005563D8
		public void UpdateDisplay()
		{
			bool flag = this._textComponent == null;
			if (!flag)
			{
				this._textComponent.ForceMeshUpdate(false, false);
				TMP_TextInfo textInfo = this._textComponent.textInfo;
				bool flag2 = textInfo.characterCount == 0;
				if (!flag2)
				{
					List<int> visibleCharacterIndices = new List<int>();
					for (int i = 0; i < textInfo.characterCount; i++)
					{
						bool isVisible = textInfo.characterInfo[i].isVisible;
						if (isVisible)
						{
							visibleCharacterIndices.Add(i);
						}
					}
					int visibleCharCount = visibleCharacterIndices.Count;
					bool flag3 = visibleCharCount == 0;
					if (!flag3)
					{
						bool flag4 = this.mode == AdvancedCurvedText.CurveMode.AnglePerCharacter;
						float angleStep;
						float totalArcAngle;
						if (flag4)
						{
							angleStep = this.anglePerCharacter;
							totalArcAngle = angleStep * (float)((visibleCharCount > 1) ? (visibleCharCount - 1) : 0);
						}
						else
						{
							totalArcAngle = this.totalAngle;
							angleStep = ((visibleCharCount > 1) ? (totalArcAngle / (float)(visibleCharCount - 1)) : 0f);
						}
						float startAngle = -totalArcAngle / 2f;
						float basePositionAngle = (this.placement == AdvancedCurvedText.ArcPlacement.Bottom) ? -90f : 90f;
						float effectiveRadius = this.radius + ((this.placement == AdvancedCurvedText.ArcPlacement.Bottom) ? this.bottomExtraRadius : 0f);
						for (int j = 0; j < visibleCharCount; j++)
						{
							int charIndex = visibleCharacterIndices[j];
							TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
							int vertexIndex = charInfo.vertexIndex;
							int materialIndex = charInfo.materialReferenceIndex;
							Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
							float centerX = (vertices[vertexIndex].x + vertices[vertexIndex + 2].x) / 2f;
							Vector3 charMidBaselinePos = new Vector3(centerX, charInfo.baseLine, 0f);
							vertices[vertexIndex] -= charMidBaselinePos;
							vertices[vertexIndex + 1] -= charMidBaselinePos;
							vertices[vertexIndex + 2] -= charMidBaselinePos;
							vertices[vertexIndex + 3] -= charMidBaselinePos;
							float abstractCharAngle = startAngle + (float)j * angleStep;
							bool flag5 = this.placement == AdvancedCurvedText.ArcPlacement.Bottom;
							float positionAngle;
							float rotationAngle;
							if (flag5)
							{
								positionAngle = abstractCharAngle;
								rotationAngle = abstractCharAngle;
							}
							else
							{
								positionAngle = -abstractCharAngle;
								rotationAngle = -abstractCharAngle;
							}
							float finalPositionAngleRad = (positionAngle + basePositionAngle + this.angularOffset) * 0.017453292f;
							float cos = Mathf.Cos(finalPositionAngleRad);
							float sin = Mathf.Sin(finalPositionAngleRad);
							Vector3 position = new Vector3(cos * effectiveRadius, sin * effectiveRadius, 0f) + this.positionOffset;
							float finalRotation = rotationAngle + this.angularOffset;
							Quaternion rotation = Quaternion.AngleAxis(finalRotation, Vector3.forward);
							Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
							vertices[vertexIndex] = matrix.MultiplyPoint3x4(vertices[vertexIndex]);
							vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
							vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
							vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);
						}
						this._textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
					}
				}
			}
		}

		// Token: 0x040090D5 RID: 37077
		[Header("模式选择")]
		[SerializeField]
		[Tooltip("TotalAngle: 指定总弧度, AnglePerCharacter: 指定每字符角度")]
		private AdvancedCurvedText.CurveMode mode = AdvancedCurvedText.CurveMode.TotalAngle;

		// Token: 0x040090D6 RID: 37078
		[Header("位置与方向")]
		[SerializeField]
		[Tooltip("选择文本是在圆弧上方还是下方")]
		private AdvancedCurvedText.ArcPlacement placement = AdvancedCurvedText.ArcPlacement.Bottom;

		// Token: 0x040090D7 RID: 37079
		[Header("曲线参数")]
		[SerializeField]
		[Tooltip("文本环绕的圆的基础半径")]
		private float radius = 100f;

		// Token: 0x040090D8 RID: 37080
		[SerializeField]
		[Tooltip("仅在Bottom模式生效，用于增加额外半径，避免与Top模式文本重叠")]
		private float bottomExtraRadius;

		// Token: 0x040090D9 RID: 37081
		[SerializeField]
		[Tooltip("当模式为 TotalAngle 时，整个文本块的总弧度")]
		private float totalAngle = 90f;

		// Token: 0x040090DA RID: 37082
		[SerializeField]
		[Tooltip("当模式为 AnglePerCharacter 时，每个字符之间的角度")]
		private float anglePerCharacter = 5f;

		// Token: 0x040090DB RID: 37083
		[Header("高级设置")]
		[SerializeField]
		[Tooltip("整体旋转角度偏移")]
		private float angularOffset;

		// Token: 0x040090DC RID: 37084
		[SerializeField]
		[Tooltip("整体位置偏移")]
		private Vector2 positionOffset = Vector2.zero;

		// Token: 0x040090DD RID: 37085
		private TextMeshProUGUI _textComponent;

		// Token: 0x040090DE RID: 37086
		private bool _isDirty;

		// Token: 0x0200264C RID: 9804
		private enum CurveMode
		{
			// Token: 0x0400EA31 RID: 59953
			[Tooltip("指定总弧长角度")]
			TotalAngle,
			// Token: 0x0400EA32 RID: 59954
			[Tooltip("指定每个字符之间的夹角")]
			AnglePerCharacter
		}

		// Token: 0x0200264D RID: 9805
		private enum ArcPlacement
		{
			// Token: 0x0400EA34 RID: 59956
			[Tooltip("文本在圆弧下方, 文字正上方朝向圆心，就像人在南极倒立")]
			Bottom,
			// Token: 0x0400EA35 RID: 59957
			[Tooltip("文本在圆弧上方, 文字正下方朝向圆心，就像人在北极站立")]
			Top
		}
	}
}
