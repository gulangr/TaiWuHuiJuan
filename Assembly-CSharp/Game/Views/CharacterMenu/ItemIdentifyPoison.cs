using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Sprites;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B7B RID: 2939
	public class ItemIdentifyPoison : MonoBehaviour
	{
		// Token: 0x17000FC0 RID: 4032
		// (get) Token: 0x06009141 RID: 37185 RVA: 0x0043B02C File Offset: 0x0043922C
		private Material BackgroundMaterial
		{
			get
			{
				return this.background.material;
			}
		}

		// Token: 0x06009142 RID: 37186 RVA: 0x0043B03C File Offset: 0x0043923C
		public void Play(bool identifySuccess, bool hasPoison, Action onCompleted)
		{
			this._identifySuccess = identifySuccess;
			this._hasPoison = hasPoison;
			this._onCompleted = onCompleted;
			bool flag = this.label != null;
			if (flag)
			{
				this.label.SetText(LocalStringManager.Get(LanguageKey.LK_Poison_Identifying), true);
			}
			base.gameObject.SetActive(true);
			base.StopAllCoroutines();
			Tween magnifierTween = this._magnifierTween;
			if (magnifierTween != null)
			{
				magnifierTween.Kill(false);
			}
			this._magnifierTween = null;
			this.SetupBackgroundMaterial();
			this._uvOffset = 0f;
			this.ApplyBackgroundOffset();
			this.PlayMagnifierTween();
		}

		// Token: 0x06009143 RID: 37187 RVA: 0x0043B0D4 File Offset: 0x004392D4
		public void Hide()
		{
			base.StopAllCoroutines();
			Tween magnifierTween = this._magnifierTween;
			if (magnifierTween != null)
			{
				magnifierTween.Kill(false);
			}
			this._magnifierTween = null;
			base.gameObject.SetActive(false);
		}

		// Token: 0x06009144 RID: 37188 RVA: 0x0043B108 File Offset: 0x00439308
		public void AdjustPosition(RectTransform rangeRectTransform, RectTransform rowRectTransform)
		{
			bool flag = !rangeRectTransform || !rowRectTransform;
			if (!flag)
			{
				RectTransform tipRect;
				bool flag2 = !base.TryGetComponent<RectTransform>(out tipRect);
				if (!flag2)
				{
					Vector3[] rowCorners = new Vector3[4];
					rowRectTransform.GetWorldCorners(rowCorners);
					float rowBottom = Mathf.Min(new float[]
					{
						rowCorners[0].y,
						rowCorners[1].y,
						rowCorners[2].y,
						rowCorners[3].y
					});
					float rowTop = Mathf.Max(new float[]
					{
						rowCorners[0].y,
						rowCorners[1].y,
						rowCorners[2].y,
						rowCorners[3].y
					});
					Vector3[] tipCorners = new Vector3[4];
					tipRect.GetWorldCorners(tipCorners);
					float tipTop = Mathf.Max(new float[]
					{
						tipCorners[0].y,
						tipCorners[1].y,
						tipCorners[2].y,
						tipCorners[3].y
					});
					tipRect.position += new Vector3(0f, rowBottom - tipTop, 0f);
					tipRect.GetWorldCorners(tipCorners);
					float tipBottom = Mathf.Min(new float[]
					{
						tipCorners[0].y,
						tipCorners[1].y,
						tipCorners[2].y,
						tipCorners[3].y
					});
					Vector3[] rangeCorners = new Vector3[4];
					rangeRectTransform.GetWorldCorners(rangeCorners);
					float rangeBottom = Mathf.Min(new float[]
					{
						rangeCorners[0].y,
						rangeCorners[1].y,
						rangeCorners[2].y,
						rangeCorners[3].y
					});
					float rangeTop = Mathf.Max(new float[]
					{
						rangeCorners[0].y,
						rangeCorners[1].y,
						rangeCorners[2].y,
						rangeCorners[3].y
					});
					bool flag3 = tipBottom < rangeBottom;
					if (flag3)
					{
						tipRect.GetWorldCorners(tipCorners);
						tipBottom = Mathf.Min(new float[]
						{
							tipCorners[0].y,
							tipCorners[1].y,
							tipCorners[2].y,
							tipCorners[3].y
						});
						tipRect.position += new Vector3(0f, rowTop - tipBottom, 0f);
					}
					tipRect.GetWorldCorners(tipCorners);
					tipBottom = Mathf.Min(new float[]
					{
						tipCorners[0].y,
						tipCorners[1].y,
						tipCorners[2].y,
						tipCorners[3].y
					});
					tipTop = Mathf.Max(new float[]
					{
						tipCorners[0].y,
						tipCorners[1].y,
						tipCorners[2].y,
						tipCorners[3].y
					});
					bool flag4 = tipTop > rangeTop;
					if (flag4)
					{
						tipRect.position += new Vector3(0f, rangeTop - tipTop, 0f);
					}
					else
					{
						bool flag5 = tipBottom < rangeBottom;
						if (flag5)
						{
							tipRect.position += new Vector3(0f, rangeBottom - tipBottom, 0f);
						}
					}
				}
			}
		}

		// Token: 0x06009145 RID: 37189 RVA: 0x0043B500 File Offset: 0x00439700
		private void PlayMagnifierTween()
		{
			ItemIdentifyPoison.<>c__DisplayClass20_0 CS$<>8__locals1 = new ItemIdentifyPoison.<>c__DisplayClass20_0();
			CS$<>8__locals1.<>4__this = this;
			bool flag = !this.icon || !this.magnifier;
			if (flag)
			{
				this.ShowResultText();
				Action onCompleted = this._onCompleted;
				if (onCompleted != null)
				{
					onCompleted();
				}
			}
			else
			{
				float speed = Mathf.Max(1f, this.magnifierDegreesPerSecond);
				CS$<>8__locals1.totalDegrees = 720f;
				float accelDecelDegrees = Mathf.Clamp(this.accelDecelCircleFraction, 0f, 1f) * 360f;
				accelDecelDegrees = Mathf.Min(accelDecelDegrees, CS$<>8__locals1.totalDegrees * 0.5f);
				float cruiseDegrees = Mathf.Max(0f, CS$<>8__locals1.totalDegrees - accelDecelDegrees * 2f);
				float accelTime = (accelDecelDegrees <= 0f) ? 0f : (2f * accelDecelDegrees / speed);
				float cruiseTime = (cruiseDegrees <= 0f) ? 0f : (cruiseDegrees / speed);
				float decelTime = accelTime;
				float totalDuration = accelTime + cruiseTime + decelTime;
				CS$<>8__locals1.orbitOrigin = this.magnifier.anchoredPosition;
				CS$<>8__locals1.startAngleDeg = 45;
				CS$<>8__locals1.totalBackgroundOffset = this.backgroundUvScrollSpeed * totalDuration;
				Ease inEase = Ease.InExpo;
				Ease outEase = Ease.OutExpo;
				CS$<>8__locals1.<PlayMagnifierTween>g__UpdateAngle|0(0f);
				Sequence seq = DOTween.Sequence().SetUpdate(true);
				float angleStart = 0f;
				float angleA = accelDecelDegrees;
				float angleB = accelDecelDegrees + cruiseDegrees;
				float angleEnd = CS$<>8__locals1.totalDegrees;
				bool flag2 = accelTime > 0f;
				if (flag2)
				{
					seq.Append(DOVirtual.Float(angleStart, angleA, accelTime, new TweenCallback<float>(CS$<>8__locals1.<PlayMagnifierTween>g__UpdateAngle|0)).SetEase(inEase));
				}
				bool flag3 = cruiseTime > 0f;
				if (flag3)
				{
					seq.Append(DOVirtual.Float(angleA, angleB, cruiseTime, new TweenCallback<float>(CS$<>8__locals1.<PlayMagnifierTween>g__UpdateAngle|0)).SetEase(Ease.Linear));
				}
				bool flag4 = decelTime > 0f;
				if (flag4)
				{
					seq.Append(DOVirtual.Float(angleB, angleEnd, decelTime, new TweenCallback<float>(CS$<>8__locals1.<PlayMagnifierTween>g__UpdateAngle|0)).SetEase(outEase));
				}
				bool flag5 = seq.Duration(true) <= 0f;
				if (flag5)
				{
					CS$<>8__locals1.<PlayMagnifierTween>g__UpdateAngle|0(angleEnd);
				}
				this._magnifierTween = seq.OnComplete(delegate
				{
					CS$<>8__locals1.<>4__this.ShowResultText();
					base.<PlayMagnifierTween>g__UpdateAngle|0(CS$<>8__locals1.totalDegrees);
					Action onCompleted2 = CS$<>8__locals1.<>4__this._onCompleted;
					if (onCompleted2 != null)
					{
						onCompleted2();
					}
				});
			}
		}

		// Token: 0x06009146 RID: 37190 RVA: 0x0043B738 File Offset: 0x00439938
		private void SetupBackgroundMaterial()
		{
			bool flag = this.BackgroundMaterial == null;
			if (!flag)
			{
				bool flag2 = this.background != null && this.background.sprite != null && !string.IsNullOrEmpty(this.spriteUvRectProperty) && this.BackgroundMaterial.HasProperty(this.spriteUvRectProperty);
				if (flag2)
				{
					Vector4 uv = DataUtility.GetOuterUV(this.background.sprite);
					this.BackgroundMaterial.SetVector(this.spriteUvRectProperty, uv);
				}
				this.ApplyBackgroundOffset();
			}
		}

		// Token: 0x06009147 RID: 37191 RVA: 0x0043B7CC File Offset: 0x004399CC
		private void ApplyBackgroundOffset()
		{
			bool flag = this.BackgroundMaterial == null;
			if (!flag)
			{
				bool flag2 = !string.IsNullOrEmpty(this.uvOffsetXProperty) && this.BackgroundMaterial.HasProperty(this.uvOffsetXProperty);
				if (flag2)
				{
					this.BackgroundMaterial.SetFloat(this.uvOffsetXProperty, this._uvOffset);
				}
			}
		}

		// Token: 0x06009148 RID: 37192 RVA: 0x0043B82C File Offset: 0x00439A2C
		private void ShowResultText()
		{
			bool flag = this.label == null;
			if (!flag)
			{
				LanguageKey key = (!this._identifySuccess) ? LanguageKey.LK_Poison_Identify_LifeSkill_NotMeet : (this._hasPoison ? LanguageKey.LK_Poison_Identify_HasPoison : LanguageKey.LK_Poison_Identify_NoPoison);
				this.label.SetText(LocalStringManager.Get(key).ColorReplace(), true);
			}
		}

		// Token: 0x04006FE9 RID: 28649
		[Header("节点引用")]
		[SerializeField]
		private TextMeshProUGUI label;

		// Token: 0x04006FEA RID: 28650
		[SerializeField]
		private RectTransform icon;

		// Token: 0x04006FEB RID: 28651
		[SerializeField]
		private RectTransform magnifier;

		// Token: 0x04006FEC RID: 28652
		[SerializeField]
		private CImage background;

		// Token: 0x04006FED RID: 28653
		[Header("动画参数")]
		[SerializeField]
		private float magnifierDegreesPerSecond = 720f;

		// Token: 0x04006FEE RID: 28654
		[SerializeField]
		private float magnifierRadius = 60f;

		// Token: 0x04006FEF RID: 28655
		[SerializeField]
		private float backgroundUvScrollSpeed = 0.3f;

		// Token: 0x04006FF0 RID: 28656
		[SerializeField]
		private float accelDecelCircleFraction = 0.25f;

		// Token: 0x04006FF1 RID: 28657
		[Header("Shader属性名")]
		[SerializeField]
		private string uvOffsetXProperty = "_UvOffsetX";

		// Token: 0x04006FF2 RID: 28658
		[SerializeField]
		private string spriteUvRectProperty = "_SpriteUvRect";

		// Token: 0x04006FF3 RID: 28659
		private Action _onCompleted;

		// Token: 0x04006FF4 RID: 28660
		private bool _identifySuccess;

		// Token: 0x04006FF5 RID: 28661
		private bool _hasPoison;

		// Token: 0x04006FF6 RID: 28662
		private float _uvOffset;

		// Token: 0x04006FF7 RID: 28663
		private Tween _magnifierTween;
	}
}
