using System;
using Coffee.UIExtensions;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Game.Views.SectInteract.Wudang
{
	// Token: 0x020009D9 RID: 2521
	public class DefendHeavenlyTreeFeedBookItem : MonoBehaviour
	{
		// Token: 0x06007B58 RID: 31576 RVA: 0x00394A08 File Offset: 0x00392C08
		public void Show()
		{
			bool flag = this._originPos == Vector3.zero;
			if (flag)
			{
				this._originPos = base.transform.position;
			}
			base.transform.position = this._originPos;
			bool activeSelf = base.gameObject.activeSelf;
			if (!activeSelf)
			{
				base.gameObject.SetActive(true);
				this.canvasGroup.DOKill(false);
				this.canvasGroup.alpha = 0f;
				this.canvasGroup.DOFade(1f, 0.7f).From(0f, true, false);
				this.effectIdle.Play();
				this.effectDisappear.Clear();
			}
		}

		// Token: 0x06007B59 RID: 31577 RVA: 0x00394AC4 File Offset: 0x00392CC4
		public void Hide()
		{
			base.transform.DOKill(false);
			bool flag = this.canvasGroup != null;
			if (flag)
			{
				this.canvasGroup.DOKill(false);
			}
			this.effectIdle.Clear();
			this.effectDisappear.Clear();
			bool flag2 = this._originPos != Vector3.zero;
			if (flag2)
			{
				base.transform.position = this._originPos;
			}
			bool flag3 = this.canvasGroup != null;
			if (flag3)
			{
				this.canvasGroup.alpha = 0f;
			}
			base.gameObject.SetActive(false);
		}

		// Token: 0x06007B5A RID: 31578 RVA: 0x00394B68 File Offset: 0x00392D68
		public float PlayEffectDisappear(Vector3 targetPos)
		{
			bool flag = !base.gameObject.activeSelf;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				base.transform.DOKill(false);
				Sequence sequence = DOTween.Sequence().AppendCallback(delegate
				{
					this.effectIdle.Clear();
					this.effectDisappear.Play();
				}).AppendInterval(0.4f).AppendCallback(delegate
				{
					this.transform.DOMove(targetPos, Random.Range(0.5f, 1f), false);
				}).AppendInterval(0.6f).AppendCallback(delegate
				{
					this.Hide();
				}).SetTarget(base.transform);
				result = sequence.Duration(true);
			}
			return result;
		}

		// Token: 0x04005DA3 RID: 23971
		[SerializeField]
		private CanvasGroup canvasGroup;

		// Token: 0x04005DA4 RID: 23972
		[SerializeField]
		private UIParticle effectIdle;

		// Token: 0x04005DA5 RID: 23973
		[SerializeField]
		private UIParticle effectDisappear;

		// Token: 0x04005DA6 RID: 23974
		private Vector3 _originPos;
	}
}
