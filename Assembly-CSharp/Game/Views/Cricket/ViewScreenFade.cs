using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using Game.Views.Map;
using UnityEngine;

namespace Game.Views.Cricket
{
	// Token: 0x02000AC0 RID: 2752
	public class ViewScreenFade : UIBase
	{
		// Token: 0x06008789 RID: 34697 RVA: 0x003F093F File Offset: 0x003EEB3F
		public override void OnInit(ArgumentBox argsBox)
		{
		}

		// Token: 0x0600878A RID: 34698 RVA: 0x003F0942 File Offset: 0x003EEB42
		private void Awake()
		{
			this.fadeImage.SetAlpha(0f);
			this.fadeImage.raycastTarget = false;
		}

		// Token: 0x0600878B RID: 34699 RVA: 0x003F0963 File Offset: 0x003EEB63
		private void OnEnable()
		{
			CommandKitBase.SetDisable(true);
			ViewWorldMap.SetDisableMoving(true);
		}

		// Token: 0x0600878C RID: 34700 RVA: 0x003F0974 File Offset: 0x003EEB74
		private void OnDisable()
		{
			this.KillTween();
			CommandKitBase.SetDisable(false);
			ViewWorldMap.SetDisableMoving(false);
		}

		// Token: 0x0600878D RID: 34701 RVA: 0x003F098C File Offset: 0x003EEB8C
		public void FadeToDark(Action onComplete = null)
		{
			this.FadeToDark(this.defaultDuration, onComplete);
		}

		// Token: 0x0600878E RID: 34702 RVA: 0x003F09A0 File Offset: 0x003EEBA0
		public void FadeToDark(float duration, Action onComplete = null)
		{
			this.KillTween();
			this.fadeImage.raycastTarget = true;
			this._fadeTween = this.fadeImage.DOFade(1f, duration).SetUpdate(true).OnComplete(delegate
			{
				this._fadeTween = null;
				Action onComplete2 = onComplete;
				if (onComplete2 != null)
				{
					onComplete2();
				}
			}).SetAutoKill(true);
		}

		// Token: 0x0600878F RID: 34703 RVA: 0x003F0A0A File Offset: 0x003EEC0A
		public void FadeToBright(Action onComplete = null)
		{
			this.FadeToBright(this.defaultDuration, onComplete);
		}

		// Token: 0x06008790 RID: 34704 RVA: 0x003F0A1C File Offset: 0x003EEC1C
		public void FadeToBright(float duration, Action onComplete = null)
		{
			this.KillTween();
			this._fadeTween = this.fadeImage.DOFade(0f, duration).SetUpdate(true).OnComplete(delegate
			{
				this._fadeTween = null;
				this.fadeImage.raycastTarget = false;
				Action onComplete2 = onComplete;
				if (onComplete2 != null)
				{
					onComplete2();
				}
			}).SetAutoKill(true);
		}

		// Token: 0x06008791 RID: 34705 RVA: 0x003F0A7C File Offset: 0x003EEC7C
		private void KillTween()
		{
			bool flag = this._fadeTween != null;
			if (flag)
			{
				this._fadeTween.Kill(false);
				this._fadeTween = null;
			}
		}

		// Token: 0x0400681B RID: 26651
		[SerializeField]
		private CImage fadeImage;

		// Token: 0x0400681C RID: 26652
		[SerializeField]
		private float defaultDuration = 0.5f;

		// Token: 0x0400681D RID: 26653
		private Tween _fadeTween;
	}
}
