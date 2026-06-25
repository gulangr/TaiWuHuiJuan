using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

// Token: 0x020001A1 RID: 417
public class Bubble : MonoBehaviour
{
	// Token: 0x1700028D RID: 653
	// (get) Token: 0x060017B2 RID: 6066 RVA: 0x00092002 File Offset: 0x00090202
	// (set) Token: 0x060017B3 RID: 6067 RVA: 0x0009200A File Offset: 0x0009020A
	public bool BubbleHide { get; private set; }

	// Token: 0x060017B4 RID: 6068 RVA: 0x00092014 File Offset: 0x00090214
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

	// Token: 0x060017B5 RID: 6069 RVA: 0x000920A4 File Offset: 0x000902A4
	public void Hide()
	{
		this.Hide(null);
	}

	// Token: 0x060017B6 RID: 6070 RVA: 0x000920AE File Offset: 0x000902AE
	public void Hide(Action action)
	{
		base.transform.localScale = new Vector3(0f, 0f, 1f);
		this.BubbleHide = true;
		if (action != null)
		{
			action();
		}
	}

	// Token: 0x060017B7 RID: 6071 RVA: 0x000920E8 File Offset: 0x000902E8
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

	// Token: 0x060017B8 RID: 6072 RVA: 0x0009219A File Offset: 0x0009039A
	public void Clear()
	{
		base.transform.localScale = new Vector3(0f, 0f, 1f);
		this.BubbleHide = true;
	}

	// Token: 0x04001312 RID: 4882
	[SerializeField]
	private TextMeshProUGUI textMeshProUGUI;

	// Token: 0x04001313 RID: 4883
	[SerializeField]
	private bool isAutomaticHeight = true;

	// Token: 0x04001314 RID: 4884
	[SerializeField]
	private float bubbleDuration = 0.5f;

	// Token: 0x04001315 RID: 4885
	[SerializeField]
	private float textDuration = 0.25f;

	// Token: 0x04001316 RID: 4886
	[SerializeField]
	private bool isReverse = false;

	// Token: 0x04001317 RID: 4887
	[SerializeField]
	private bool ignoreTimeScale = false;

	// Token: 0x04001318 RID: 4888
	private BubbleHeightCalc heightCalc;

	// Token: 0x04001319 RID: 4889
	private RectTransform rectTransform;
}
