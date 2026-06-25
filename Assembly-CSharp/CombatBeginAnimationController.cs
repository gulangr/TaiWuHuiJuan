using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Spine;
using Spine.Unity;
using UnityEngine;

// Token: 0x02000150 RID: 336
public class CombatBeginAnimationController : MonoBehaviour
{
	// Token: 0x17000213 RID: 531
	// (get) Token: 0x06001297 RID: 4759 RVA: 0x00071153 File Offset: 0x0006F353
	private TutorialChapterModel TutorialModel
	{
		get
		{
			return SingletonObject.getInstance<TutorialChapterModel>();
		}
	}

	// Token: 0x17000214 RID: 532
	// (get) Token: 0x06001298 RID: 4760 RVA: 0x0007115A File Offset: 0x0006F35A
	public bool PointerMaskOn
	{
		get
		{
			return this.pointerMask.activeSelf;
		}
	}

	// Token: 0x06001299 RID: 4761 RVA: 0x00071168 File Offset: 0x0006F368
	public void Init()
	{
		this.cloudLeft.anchoredPosition = this.cloudLeft.anchoredPosition.SetX(-210f);
		this.cloudRight.anchoredPosition = this.cloudRight.anchoredPosition.SetX(230f);
		this.ani1.gameObject.SetActive(false);
		this.ani2.gameObject.SetActive(false);
		this.content.alpha = 0f;
		this.pointerMask.SetActive(true);
		this.clickToStartTips.SetActive(false);
	}

	// Token: 0x0600129A RID: 4762 RVA: 0x00071207 File Offset: 0x0006F407
	public void OnChangeToCombat()
	{
		this.pointerMask.SetActive(true);
		this.clickToStartTips.SetActive(false);
	}

	// Token: 0x0600129B RID: 4763 RVA: 0x00071224 File Offset: 0x0006F424
	public void PlayFadeIn(TweenCallback onFadeIn)
	{
		this.content.DOFade(1f, 0.5f).SetEase(Ease.Linear).OnComplete(onFadeIn);
	}

	// Token: 0x0600129C RID: 4764 RVA: 0x0007124C File Offset: 0x0006F44C
	public void PlayBegin(TweenCallback onTrack1Complete)
	{
		TrackEntry track = this.ani1.AnimationState.SetAnimation(0, "in", false);
		TrackEntry track2 = this.ani2.AnimationState.SetAnimation(0, "in", false);
		this.ani1.gameObject.SetActive(true);
		this.ani2.gameObject.SetActive(true);
		track.MixDuration = 0f;
		Action <>9__2;
		track.Complete += delegate(TrackEntry _)
		{
			this.ani1.AnimationState.SetAnimation(0, "idle", true);
			bool tutorialWaitOpenCharacterNeili = this.TutorialModel.WaitOpenCharacterNeili;
			this.pointerMask.SetActive(false);
			this.clickToStartTips.SetActive(!tutorialWaitOpenCharacterNeili);
			bool flag = tutorialWaitOpenCharacterNeili;
			if (flag)
			{
				UIElement characterMenu = UIElement.CharacterMenu;
				Delegate onHide = characterMenu.OnHide;
				Action b;
				if ((b = <>9__2) == null)
				{
					b = (<>9__2 = delegate()
					{
						this.clickToStartTips.SetActive(true);
					});
				}
				characterMenu.OnHide = (Action)Delegate.Combine(onHide, b);
			}
			onTrack1Complete();
		};
		track2.MixDuration = 0f;
		track2.Complete += delegate(TrackEntry _)
		{
			this.ani2.AnimationState.SetAnimation(0, "idle", true);
		};
		this.cloudLeft.DOAnchorPosX(-160f, 0.2f, false);
		this.cloudRight.DOAnchorPosX(180f, 0.2f, false);
	}

	// Token: 0x0600129D RID: 4765 RVA: 0x00071330 File Offset: 0x0006F530
	public void PlayEnd(TweenCallback onFadeComplete)
	{
		TrackEntry track = this.ani1.AnimationState.SetAnimation(0, "out", false);
		TrackEntry track2 = this.ani2.AnimationState.SetAnimation(0, "out", false);
		track.MixDuration = 0.1f;
		track2.MixDuration = 0.1f;
		track.Complete += delegate(TrackEntry _)
		{
			this.content.DOFade(0f, 0.5f).SetEase(Ease.Linear).OnComplete(onFadeComplete);
		};
	}

	// Token: 0x04000FCE RID: 4046
	[SerializeField]
	private RectTransform cloudLeft;

	// Token: 0x04000FCF RID: 4047
	[SerializeField]
	private RectTransform cloudRight;

	// Token: 0x04000FD0 RID: 4048
	[SerializeField]
	private SkeletonGraphic ani1;

	// Token: 0x04000FD1 RID: 4049
	[SerializeField]
	private SkeletonGraphic ani2;

	// Token: 0x04000FD2 RID: 4050
	[SerializeField]
	private CanvasGroup content;

	// Token: 0x04000FD3 RID: 4051
	[SerializeField]
	private GameObject pointerMask;

	// Token: 0x04000FD4 RID: 4052
	[SerializeField]
	private GameObject clickToStartTips;
}
