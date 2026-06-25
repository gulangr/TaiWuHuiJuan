using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B68 RID: 2920
	public class CharacterMenuLocalLoadingAnim : MonoBehaviour
	{
		// Token: 0x06009082 RID: 36994 RVA: 0x00435CE8 File Offset: 0x00433EE8
		private void OnDisable()
		{
			bool flag = this._delayHideLoadingCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._delayHideLoadingCoroutine);
			}
			this._delayHideLoadingCoroutine = null;
			this._isLoading = false;
		}

		// Token: 0x06009083 RID: 36995 RVA: 0x00435D1E File Offset: 0x00433F1E
		public void SetLoadingContentList(List<GameObject> list)
		{
			this.localLoadingContent = list;
		}

		// Token: 0x06009084 RID: 36996 RVA: 0x00435D28 File Offset: 0x00433F28
		public void SetLoadingEvent(Action loadingStartCb = null, Action loadingCompleteCb = null)
		{
			this._loadingStartCb = loadingStartCb;
			this._loadingCompleteCb = loadingCompleteCb;
		}

		// Token: 0x06009085 RID: 36997 RVA: 0x00435D39 File Offset: 0x00433F39
		public void EnsureContentActiveForLayout()
		{
			this._keepContentActiveForLayout = true;
			this.RefreshLoadingInfo();
		}

		// Token: 0x06009086 RID: 36998 RVA: 0x00435D4C File Offset: 0x00433F4C
		public void SetLoadingState(bool isLoading)
		{
			bool flag = !base.gameObject.activeInHierarchy;
			if (!flag)
			{
				if (isLoading)
				{
					this._keepContentActiveForLayout = false;
					bool flag2 = this._delayHideLoadingCoroutine != null;
					if (flag2)
					{
						base.StopCoroutine(this._delayHideLoadingCoroutine);
						this._delayHideLoadingCoroutine = null;
					}
					this._isLoading = true;
					this.RefreshLoadingInfo();
					this._loadingShowStartTime = Time.unscaledTime;
					base.StartCoroutine(this.DelayHideLoading(this.loadingMinVisibleDuration));
				}
				else
				{
					this._isLoading = false;
					this._keepContentActiveForLayout = false;
					float minDuration = Mathf.Max(0f, this.loadingMinVisibleDuration);
					float elapsed = Time.unscaledTime - this._loadingShowStartTime;
					float remain = minDuration - elapsed;
					bool flag3 = remain <= 0f;
					if (flag3)
					{
						this.RefreshLoadingInfo();
					}
				}
			}
		}

		// Token: 0x06009087 RID: 36999 RVA: 0x00435E20 File Offset: 0x00434020
		private IEnumerator DelayHideLoading(float delay)
		{
			yield return new WaitForSeconds(delay);
			bool flag = !this._isLoading;
			if (flag)
			{
				this.RefreshLoadingInfo();
			}
			this._delayHideLoadingCoroutine = null;
			yield break;
		}

		// Token: 0x06009088 RID: 37000 RVA: 0x00435E38 File Offset: 0x00434038
		private void RefreshLoadingInfo()
		{
			bool flag = this._isLoading && this._loadingStartCb != null;
			if (flag)
			{
				this._loadingStartCb();
			}
			bool flag2 = this.localLoadingContent != null;
			if (flag2)
			{
				for (int i = 0; i < this.localLoadingContent.Count; i++)
				{
					bool flag3 = this._isLoading && this._keepContentActiveForLayout;
					if (flag3)
					{
						CharacterMenuLocalLoadingAnim.SetCanvasGroupVisible(this.localLoadingContent[i], false);
					}
					else
					{
						bool visible = !this._isLoading;
						this.localLoadingContent[i].SetActive(visible);
						bool flag4 = visible;
						if (flag4)
						{
							CharacterMenuLocalLoadingAnim.SetCanvasGroupVisible(this.localLoadingContent[i], true);
						}
					}
				}
			}
			bool flag5 = this.localLoadingAnim != null;
			if (flag5)
			{
				for (int j = 0; j < this.localLoadingAnim.Count; j++)
				{
					this.localLoadingAnim[j].SetActive(this._isLoading);
				}
			}
			bool flag6 = !this._isLoading && this._loadingCompleteCb != null;
			if (flag6)
			{
				this._loadingCompleteCb();
			}
		}

		// Token: 0x06009089 RID: 37001 RVA: 0x00435F74 File Offset: 0x00434174
		private static void SetCanvasGroupVisible(GameObject go, bool visible)
		{
			go.SetActive(true);
			CanvasGroup canvasGroup = go.GetComponent<CanvasGroup>();
			bool flag = canvasGroup == null;
			if (flag)
			{
				canvasGroup = go.AddComponent<CanvasGroup>();
			}
			canvasGroup.alpha = (visible ? 1f : 0f);
			canvasGroup.interactable = visible;
			canvasGroup.blocksRaycasts = visible;
		}

		// Token: 0x04006F3C RID: 28476
		[SerializeField]
		private List<GameObject> localLoadingContent;

		// Token: 0x04006F3D RID: 28477
		[SerializeField]
		private List<GameObject> localLoadingAnim;

		// Token: 0x04006F3E RID: 28478
		[SerializeField]
		protected float loadingMinVisibleDuration = 0.2f;

		// Token: 0x04006F3F RID: 28479
		private bool _isLoading;

		// Token: 0x04006F40 RID: 28480
		private bool _keepContentActiveForLayout;

		// Token: 0x04006F41 RID: 28481
		private float _loadingShowStartTime;

		// Token: 0x04006F42 RID: 28482
		private Coroutine _delayHideLoadingCoroutine;

		// Token: 0x04006F43 RID: 28483
		private Action _loadingStartCb;

		// Token: 0x04006F44 RID: 28484
		private Action _loadingCompleteCb;
	}
}
