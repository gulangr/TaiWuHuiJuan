using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using Spine.Unity;
using UnityEngine;

// Token: 0x02000375 RID: 885
public class UI_ChickenMap : UIBase
{
	// Token: 0x060033D1 RID: 13265 RVA: 0x00199F20 File Offset: 0x00198120
	public static void OpenChickenMap()
	{
	}

	// Token: 0x060033D2 RID: 13266 RVA: 0x00199F23 File Offset: 0x00198123
	public override void OnInit(ArgumentBox argsBox)
	{
	}

	// Token: 0x060033D3 RID: 13267 RVA: 0x00199F28 File Offset: 0x00198128
	private void OnEnable()
	{
		SkeletonGraphic chickenFeatherup = base.CGet<SkeletonGraphic>("ChickenFeatherUp");
		chickenFeatherup.AnimationState.SetAnimation(0, "animation", false);
		SkeletonGraphic chickenFeatherDown = base.CGet<SkeletonGraphic>("ChickenFeatherDown");
		chickenFeatherDown.AnimationState.SetAnimation(0, "animation", false);
		AudioManager.Instance.PlaySound("SFX_ChickenFeather_seek", false, false);
		Refers refers = base.CGet<Refers>("RootRefers");
		List<CImage> rotateImages = refers.CGetList<CImage>("Rotate_");
		bool flag = this._progressBarSubDefaultAngle == null;
		if (flag)
		{
			this._progressBarSubDefaultAngle = new Vector3[]
			{
				rotateImages[0].rectTransform.parent.eulerAngles,
				rotateImages[1].rectTransform.parent.eulerAngles,
				rotateImages[2].rectTransform.parent.eulerAngles
			};
		}
		List<CImage> progressBarMain = refers.CGetList<CImage>("Progress1_");
		progressBarMain.ForEach(delegate(CImage bar)
		{
			bar.rectTransform.eulerAngles = Vector3.zero;
		});
		for (int i = 0; i < rotateImages.Count; i++)
		{
			rotateImages[i].rectTransform.parent.eulerAngles = this._progressBarSubDefaultAngle[i];
		}
		this.FadeIn();
		base.StartCoroutine(this.DelayAction(refers, rotateImages, progressBarMain));
	}

	// Token: 0x060033D4 RID: 13268 RVA: 0x0019A09E File Offset: 0x0019829E
	private IEnumerator DelayAction(Refers refers, List<CImage> rotateImages, List<CImage> progressBarMain)
	{
		float generateGap = 0.4f;
		int num;
		for (int i = 0; i < 6; i = num + 1)
		{
			WaitForSeconds wait = new WaitForSeconds(generateGap);
			yield return wait;
			for (int j = 0; j < progressBarMain.Count; j = num + 1)
			{
				CImage bar = progressBarMain[j];
				bar.rectTransform.DORotate(new Vector3(0f, 0f, -12f), 2.4f + this.TextChangeDelayTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
				bar = null;
				num = j;
			}
			for (int k = 0; k < rotateImages.Count; k = num + 1)
			{
				Transform rectTrans = rotateImages[k].rectTransform.parent;
				bool flag = !DOTween.IsTweening(rectTrans, true);
				if (flag)
				{
					rectTrans.DORotate(rectTrans.eulerAngles - new Vector3(0f, 0f, Random.value * 5f + 5f), Random.value * 0.2f + 0.3f, RotateMode.Fast).SetEase(Ease.OutBounce).SetDelay(Random.value * 0.3f + 0.2f);
				}
				rectTrans = null;
				num = k;
			}
			wait = null;
			num = i;
		}
		this.FiadeOut();
		yield return new WaitForSeconds(this.FadeTime);
		this.QuickHide();
		yield break;
	}

	// Token: 0x060033D5 RID: 13269 RVA: 0x0019A0C4 File Offset: 0x001982C4
	private void FadeIn()
	{
		SkeletonGraphic[] graphics = base.GetComponentsInChildren<SkeletonGraphic>();
		foreach (SkeletonGraphic graphic in graphics)
		{
			graphic.color = graphic.color.SetAlpha(0f);
			graphic.DOKill(false);
			graphic.DOFade(1f, this.FadeTime);
		}
		CImage[] iamges = base.GetComponentsInChildren<CImage>();
		foreach (CImage iamge in iamges)
		{
			iamge.color = iamge.color.SetAlpha(0f);
			iamge.DOKill(false);
			iamge.DOFade(1f, this.FadeTime);
		}
	}

	// Token: 0x060033D6 RID: 13270 RVA: 0x0019A184 File Offset: 0x00198384
	private void FiadeOut()
	{
		SkeletonGraphic[] graphics = base.GetComponentsInChildren<SkeletonGraphic>();
		foreach (SkeletonGraphic graphic in graphics)
		{
			graphic.DOKill(false);
			graphic.DOFade(0f, this.FadeTime);
		}
		CImage[] iamges = base.GetComponentsInChildren<CImage>();
		foreach (CImage iamge in iamges)
		{
			iamge.DOKill(false);
			iamge.DOFade(0f, this.FadeTime);
		}
	}

	// Token: 0x040025B8 RID: 9656
	private const float AnimationTime = 2.4f;

	// Token: 0x040025B9 RID: 9657
	private readonly float FadeTime = 0.3f;

	// Token: 0x040025BA RID: 9658
	private readonly float TextChangeDelayTime = 0.5f;

	// Token: 0x040025BB RID: 9659
	private Vector3[] _progressBarSubDefaultAngle;
}
