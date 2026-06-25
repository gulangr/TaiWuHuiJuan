using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Views.Main.Reading
{
	// Token: 0x0200096C RID: 2412
	public class ReadingPageProgressBar : MonoBehaviour
	{
		// Token: 0x06007381 RID: 29569 RVA: 0x0035A5EC File Offset: 0x003587EC
		public void ResetDisplay()
		{
			this.KillTween();
			this.currentProgressBar.fillAmount = 0f;
			this.currentProgressPreviewBar.fillAmount = 0f;
			this.rereadPreviewBar.fillAmount = 0f;
			this.skipProgressBar.fillAmount = 0f;
			this.skipProgressBar.gameObject.SetActive(false);
		}

		// Token: 0x06007382 RID: 29570 RVA: 0x0035A658 File Offset: 0x00358858
		public void SetDisplay(float currentProgress, float currentProgressPreview, float rereadPreview, bool isSkipped, bool animateCurrent)
		{
			this.KillTween();
			currentProgress = Mathf.Clamp01(currentProgress);
			currentProgressPreview = Mathf.Clamp01(currentProgressPreview);
			rereadPreview = Mathf.Clamp01(rereadPreview);
			this.currentProgressPreviewBar.fillAmount = (isSkipped ? 0f : currentProgressPreview);
			this.rereadPreviewBar.fillAmount = (isSkipped ? 0f : rereadPreview);
			this.skipProgressBar.gameObject.SetActive(isSkipped);
			if (isSkipped)
			{
				this.currentProgressBar.fillAmount = 0f;
				this.skipProgressBar.fillAmount = 1f;
			}
			else
			{
				this.skipProgressBar.fillAmount = 0f;
				if (animateCurrent)
				{
					this._currentProgressTweener = this.currentProgressBar.DOFillAmount(currentProgress, 0.3f).SetAutoKill(true);
				}
				else
				{
					this.currentProgressBar.fillAmount = currentProgress;
				}
			}
		}

		// Token: 0x06007383 RID: 29571 RVA: 0x0035A73A File Offset: 0x0035893A
		private void OnDisable()
		{
			this.KillTween();
		}

		// Token: 0x06007384 RID: 29572 RVA: 0x0035A744 File Offset: 0x00358944
		private void KillTween()
		{
			bool flag = this._currentProgressTweener == null || !this._currentProgressTweener.IsActive();
			if (!flag)
			{
				this._currentProgressTweener.Kill(false);
				this._currentProgressTweener = null;
			}
		}

		// Token: 0x040055E4 RID: 21988
		private const float AnimationDuration = 0.3f;

		// Token: 0x040055E5 RID: 21989
		[SerializeField]
		[FormerlySerializedAs("progressImage")]
		private CImage currentProgressBar;

		// Token: 0x040055E6 RID: 21990
		[SerializeField]
		[FormerlySerializedAs("progressExtraUnderImage")]
		private CImage currentProgressPreviewBar;

		// Token: 0x040055E7 RID: 21991
		[SerializeField]
		[FormerlySerializedAs("progressExtraAboveImage")]
		private CImage rereadPreviewBar;

		// Token: 0x040055E8 RID: 21992
		[SerializeField]
		private CImage skipProgressBar;

		// Token: 0x040055E9 RID: 21993
		private Tweener _currentProgressTweener;
	}
}
