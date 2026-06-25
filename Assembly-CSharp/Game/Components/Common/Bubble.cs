using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace Game.Components.Common
{
	// Token: 0x02000F86 RID: 3974
	public class Bubble : MonoBehaviour
	{
		// Token: 0x1700149B RID: 5275
		// (get) Token: 0x0600B6AB RID: 46763 RVA: 0x0053469E File Offset: 0x0053289E
		// (set) Token: 0x0600B6AC RID: 46764 RVA: 0x005346A6 File Offset: 0x005328A6
		public bool BubbleHide { get; private set; }

		// Token: 0x0600B6AD RID: 46765 RVA: 0x005346B0 File Offset: 0x005328B0
		public void Show()
		{
			this.BubbleHide = false;
			Vector3 scale = this.isReverse ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
			TweenerCore<Vector3, Vector3, VectorOptions> tweenCallback = base.transform.DOScale(scale, this.bubbleDuration);
			TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = tweenCallback;
			tweenerCore.onComplete = (TweenCallback)Delegate.Combine(tweenerCore.onComplete, new TweenCallback(delegate()
			{
				TweenerCore<Color, Color, ColorOptions> fade = this.textMeshProUGUI.DOFade(1f, this.textDuration);
				bool flag2 = this.ignoreTimeScale;
				if (flag2)
				{
					fade.SetUpdate(true);
				}
			}));
			bool flag = this.ignoreTimeScale;
			if (flag)
			{
				tweenCallback.SetUpdate(true);
			}
		}

		// Token: 0x0600B6AE RID: 46766 RVA: 0x00534740 File Offset: 0x00532940
		public void Hide()
		{
			this.Hide(null);
		}

		// Token: 0x0600B6AF RID: 46767 RVA: 0x0053474A File Offset: 0x0053294A
		public void Hide(Action action)
		{
			base.transform.localScale = new Vector3(0f, 0f, 1f);
			this.BubbleHide = true;
			if (action != null)
			{
				action();
			}
		}

		// Token: 0x0600B6B0 RID: 46768 RVA: 0x00534784 File Offset: 0x00532984
		public void SetText(string str, bool isShow = true)
		{
			bool flag = this.isAutomaticHeight;
			if (flag)
			{
				bool flag2 = this.rectTransform == null;
				if (flag2)
				{
					this.rectTransform = base.GetComponent<RectTransform>();
					base.transform.localScale = new Vector3(0f, 0f, 1f);
				}
				if (this.heightCalc == null)
				{
					this.heightCalc = new BubbleHeightCalc(this.textMeshProUGUI, this.rectTransform);
				}
				this.heightCalc.SetText(str);
			}
			else
			{
				this.textMeshProUGUI.text = str;
			}
			bool flag3 = !isShow;
			if (!flag3)
			{
				this.Hide(new Action(this.Show));
			}
		}

		// Token: 0x0600B6B1 RID: 46769 RVA: 0x00534836 File Offset: 0x00532A36
		public void Clear()
		{
			base.transform.localScale = new Vector3(0f, 0f, 1f);
			this.BubbleHide = true;
		}

		// Token: 0x04008DE8 RID: 36328
		[SerializeField]
		private TextMeshProUGUI textMeshProUGUI;

		// Token: 0x04008DE9 RID: 36329
		[SerializeField]
		private bool isAutomaticHeight = true;

		// Token: 0x04008DEA RID: 36330
		[SerializeField]
		private float bubbleDuration = 0.5f;

		// Token: 0x04008DEB RID: 36331
		[SerializeField]
		private float textDuration = 0.25f;

		// Token: 0x04008DEC RID: 36332
		[SerializeField]
		private bool isReverse = false;

		// Token: 0x04008DED RID: 36333
		[SerializeField]
		private bool ignoreTimeScale = false;

		// Token: 0x04008DEE RID: 36334
		private BubbleHeightCalc heightCalc;

		// Token: 0x04008DEF RID: 36335
		private RectTransform rectTransform;
	}
}
