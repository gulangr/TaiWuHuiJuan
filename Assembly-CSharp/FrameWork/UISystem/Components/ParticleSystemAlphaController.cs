using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UISystem.Components
{
	// Token: 0x0200101E RID: 4126
	public class ParticleSystemAlphaController : MonoBehaviour
	{
		// Token: 0x0600BCFA RID: 48378 RVA: 0x0055E5EB File Offset: 0x0055C7EB
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x0600BCFB RID: 48379 RVA: 0x0055E5F8 File Offset: 0x0055C7F8
		public void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				Transform root = base.transform;
				this._particleRoot = root;
				this._controlTargets = new List<AlphaControlTarget>();
				this._isActive = false;
				this._fadePercent = 0f;
				this.InitTargets();
				this._inited = true;
			}
		}

		// Token: 0x0600BCFC RID: 48380 RVA: 0x0055E64C File Offset: 0x0055C84C
		public void SetActiveWithTween(bool isActive, float duration, Ease ease = Ease.Linear)
		{
			bool flag = !this._isActive && isActive;
			if (flag)
			{
				this.TweenAlphaPercentTo(1f, duration, ease);
			}
			bool flag2 = this._isActive && !isActive;
			if (flag2)
			{
				this.TweenAlphaPercentTo(0f, duration, ease);
			}
			this._isActive = isActive;
		}

		// Token: 0x0600BCFD RID: 48381 RVA: 0x0055E6A3 File Offset: 0x0055C8A3
		public void SetActiveNoTween(bool isActive)
		{
			this._isActive = isActive;
			this._fadePercent = (isActive ? 1f : 0f);
			this.SetAlphaPercent(this._fadePercent);
		}

		// Token: 0x0600BCFE RID: 48382 RVA: 0x0055E6D0 File Offset: 0x0055C8D0
		public void TweenAlphaPercentTo(float percent, float duration, Ease ease = Ease.Linear)
		{
			this._particleRoot.gameObject.SetActive(true);
			TweenerCore<float, float, FloatOptions> tween = DOTween.To(() => this._fadePercent, new DOSetter<float>(this.SetAlphaPercent), percent, duration);
			tween.SetEase(ease);
		}

		// Token: 0x0600BCFF RID: 48383 RVA: 0x0055E718 File Offset: 0x0055C918
		public void SetAlphaPercent(float percent)
		{
			this._fadePercent = percent;
			foreach (AlphaControlTarget target in this._controlTargets)
			{
				target.SetAlphaPercent(percent);
			}
		}

		// Token: 0x0600BD00 RID: 48384 RVA: 0x0055E778 File Offset: 0x0055C978
		private void InitTargets()
		{
			for (int i = 0; i < this._particleRoot.childCount; i++)
			{
				this.InitTargetRecursively(this._particleRoot.GetChild(i));
			}
		}

		// Token: 0x0600BD01 RID: 48385 RVA: 0x0055E7B8 File Offset: 0x0055C9B8
		private void InitTargetRecursively(Transform child)
		{
			RawImage rawImage = child.GetComponent<RawImage>();
			bool flag = rawImage != null;
			if (flag)
			{
				rawImage.material = Object.Instantiate<Material>(rawImage.material);
				AlphaControlTarget alphaControlTarget = AlphaControlTarget.CreateByMaterial(rawImage.material);
				bool flag2 = alphaControlTarget != null;
				if (flag2)
				{
					this._controlTargets.Add(alphaControlTarget);
				}
			}
			ParticleSystem particle = child.GetComponent<ParticleSystem>();
			bool flag3 = particle != null;
			if (flag3)
			{
				Renderer particleRenderer = particle.GetComponent<Renderer>();
				bool flag4 = particleRenderer && particleRenderer.material;
				if (flag4)
				{
					particleRenderer.material = Object.Instantiate<Material>(particleRenderer.material);
					AlphaControlTarget alphaControlTarget2 = AlphaControlTarget.CreateByMaterial(particleRenderer.material);
					bool flag5 = alphaControlTarget2 != null;
					if (flag5)
					{
						this._controlTargets.Add(alphaControlTarget2);
					}
				}
			}
			for (int i = 0; i < child.childCount; i++)
			{
				this.InitTargetRecursively(child.GetChild(i));
			}
		}

		// Token: 0x04009161 RID: 37217
		private Transform _particleRoot;

		// Token: 0x04009162 RID: 37218
		private List<AlphaControlTarget> _controlTargets;

		// Token: 0x04009163 RID: 37219
		private bool _isActive;

		// Token: 0x04009164 RID: 37220
		private float _fadePercent;

		// Token: 0x04009165 RID: 37221
		private bool _inited;
	}
}
