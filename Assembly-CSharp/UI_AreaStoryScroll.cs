using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;

// Token: 0x0200036B RID: 875
public class UI_AreaStoryScroll : UIBase
{
	// Token: 0x1700058B RID: 1419
	// (get) Token: 0x060032BD RID: 12989 RVA: 0x001904FD File Offset: 0x0018E6FD
	private RectTransform ScrollLeft
	{
		get
		{
			return base.CGet<RectTransform>("ScrollLeft");
		}
	}

	// Token: 0x1700058C RID: 1420
	// (get) Token: 0x060032BE RID: 12990 RVA: 0x0019050A File Offset: 0x0018E70A
	private RectTransform ScrollRight
	{
		get
		{
			return base.CGet<CImage>("ScrollRight").rectTransform;
		}
	}

	// Token: 0x1700058D RID: 1421
	// (get) Token: 0x060032BF RID: 12991 RVA: 0x0019051C File Offset: 0x0018E71C
	private Vector3 ScrollInitPos
	{
		get
		{
			return base.CGet<RectTransform>("ScrollInitPos").localPosition;
		}
	}

	// Token: 0x1700058E RID: 1422
	// (get) Token: 0x060032C0 RID: 12992 RVA: 0x0019052E File Offset: 0x0018E72E
	private Vector3 ScrollReadyPos
	{
		get
		{
			return base.CGet<RectTransform>("ScrollReadyPos").localPosition;
		}
	}

	// Token: 0x1700058F RID: 1423
	// (get) Token: 0x060032C1 RID: 12993 RVA: 0x00190540 File Offset: 0x0018E740
	private Vector3 ScrollTergetPos
	{
		get
		{
			return base.CGet<RectTransform>("ScrollTergetPos").localPosition;
		}
	}

	// Token: 0x17000590 RID: 1424
	// (get) Token: 0x060032C2 RID: 12994 RVA: 0x00190552 File Offset: 0x0018E752
	private CRawImage Page
	{
		get
		{
			return base.CGet<CRawImage>("Page");
		}
	}

	// Token: 0x060032C3 RID: 12995 RVA: 0x00190560 File Offset: 0x0018E760
	public override void OnInit(ArgumentBox argsBox)
	{
		sbyte orgTemplateId;
		argsBox.Get("orgTemplateId", out orgTemplateId);
		bool prosper;
		argsBox.Get("prosper", out prosper);
		bool showTip;
		argsBox.Get("showTip", out showTip);
		this.RefreshTexture((int)orgTemplateId, prosper);
		this.PlayExpand();
		base.CGet<LeftMouseSkipAnimNotice>("LeftMouseSkipAnimNotice").gameObject.SetActive(true);
		this._isExpand = false;
	}

	// Token: 0x060032C4 RID: 12996 RVA: 0x001905C8 File Offset: 0x0018E7C8
	private void Update()
	{
		bool flag = CommonCommandKit.LeftMouse.Check(this.Element, false, false, false, true, false) && base.CGet<LeftMouseSkipAnimNotice>("LeftMouseSkipAnimNotice").IsSkipEnabled && base.CGet<LeftMouseSkipAnimNotice>("LeftMouseSkipAnimNotice").gameObject.activeSelf;
		if (flag)
		{
			bool isExpand = this._isExpand;
			if (isExpand)
			{
				this.SkipCloseAnimation();
			}
			else
			{
				this.SkipExpandAnimation();
			}
		}
	}

	// Token: 0x060032C5 RID: 12997 RVA: 0x0019063C File Offset: 0x0018E83C
	protected override void OnClick(Transform btn)
	{
		bool flag = btn.name == "Close";
		if (flag)
		{
			this.OnClickClose();
		}
	}

	// Token: 0x17000591 RID: 1425
	// (get) Token: 0x060032C6 RID: 12998 RVA: 0x00190668 File Offset: 0x0018E868
	private Sequence NewExpandSequence
	{
		get
		{
			Sequence expandSequence = DOTween.Sequence();
			expandSequence.Append(this.ScrollRight.DOLocalMoveX(this.ScrollReadyPos.x, 1f, false));
			expandSequence.Join(base.CGet<CanvasGroup>("Canvas").DOFade(1f, 0.5f).OnStart(delegate
			{
				this.ScrollLeft.gameObject.SetActive(false);
			}).OnComplete(delegate
			{
				this.ScrollLeft.gameObject.SetActive(true);
			}));
			expandSequence.Join(this.ScrollLeft.DOLocalMoveX(this.ScrollReadyPos.x, 1f, false));
			expandSequence.Append(this.ScrollLeft.DOLocalMoveX(this.ScrollTergetPos.x, 0.8f, false).OnStart(delegate
			{
				this.Page.enabled = true;
				base.CGet<Refers>("NoteArea").gameObject.SetActive(true);
			}));
			expandSequence.AppendCallback(new TweenCallback(this.ShowClose));
			return expandSequence;
		}
	}

	// Token: 0x17000592 RID: 1426
	// (get) Token: 0x060032C7 RID: 12999 RVA: 0x00190754 File Offset: 0x0018E954
	private Sequence NewCloseSequence
	{
		get
		{
			Sequence closeSequence = DOTween.Sequence();
			closeSequence.Append(this.ScrollLeft.DOLocalMoveX(this.ScrollReadyPos.x, 0.8f, false).OnComplete(delegate
			{
				this.Page.enabled = false;
				base.CGet<Refers>("NoteArea").gameObject.SetActive(false);
			}));
			closeSequence.Append(this.ScrollRight.DOLocalMoveX(this.ScrollInitPos.x, 1f, false));
			closeSequence.Join(this.ScrollLeft.DOLocalMoveX(this.ScrollInitPos.x, 1f, false));
			closeSequence.Append(base.CGet<CanvasGroup>("Canvas").DOFade(0f, 0.5f).OnStart(delegate
			{
				this.ScrollLeft.gameObject.SetActive(false);
			}).OnComplete(delegate
			{
				this.ScrollLeft.gameObject.SetActive(true);
			}));
			closeSequence.AppendCallback(new TweenCallback(this.HideSelf));
			return closeSequence;
		}
	}

	// Token: 0x060032C8 RID: 13000 RVA: 0x0019083D File Offset: 0x0018EA3D
	private void ChangeCloseActive(bool inActive)
	{
		base.CGet<GameObject>("Close").SetActive(inActive);
	}

	// Token: 0x060032C9 RID: 13001 RVA: 0x00190854 File Offset: 0x0018EA54
	public override void QuickHide()
	{
		bool flag = !base.CGet<GameObject>("Close").activeSelf;
		if (!flag)
		{
			this.OnClickClose();
		}
	}

	// Token: 0x060032CA RID: 13002 RVA: 0x00190884 File Offset: 0x0018EA84
	private void OnClickClose()
	{
		this.ChangeCloseActive(false);
		base.CGet<LeftMouseSkipAnimNotice>("LeftMouseSkipAnimNotice").gameObject.SetActive(true);
		Sequence playingSequence = this._playingSequence;
		if (playingSequence != null)
		{
			playingSequence.Kill(false);
		}
		this._playingSequence = this.NewCloseSequence;
		this._playingSequence.PlayForward();
		AudioManager.Instance.PlaySound("UI_GameLineScroll_CloseScroll", false, false);
	}

	// Token: 0x060032CB RID: 13003 RVA: 0x001908EE File Offset: 0x0018EAEE
	private void OnDisable()
	{
		TaiwuEventDomainMethod.Call.TriggerListener("SectMainStoryScrollShowed", true);
	}

	// Token: 0x060032CC RID: 13004 RVA: 0x00190900 File Offset: 0x0018EB00
	private StoryScrollItem GetScrollConfig(int orgTemplateId, bool prosper)
	{
		Dictionary<int, short[]> map = new Dictionary<int, short[]>
		{
			{
				1,
				new short[]
				{
					54,
					55
				}
			},
			{
				2,
				new short[]
				{
					56,
					57
				}
			},
			{
				3,
				new short[]
				{
					58,
					59
				}
			},
			{
				4,
				new short[]
				{
					60,
					61
				}
			},
			{
				5,
				new short[]
				{
					62,
					63
				}
			},
			{
				6,
				new short[]
				{
					64,
					65
				}
			},
			{
				7,
				new short[]
				{
					66,
					67
				}
			},
			{
				8,
				new short[]
				{
					68,
					69
				}
			},
			{
				9,
				new short[]
				{
					70,
					71
				}
			},
			{
				10,
				new short[]
				{
					72,
					73
				}
			},
			{
				11,
				new short[]
				{
					74,
					75
				}
			},
			{
				12,
				new short[]
				{
					76,
					77
				}
			},
			{
				13,
				new short[]
				{
					78,
					79
				}
			},
			{
				14,
				new short[]
				{
					80,
					81
				}
			},
			{
				15,
				new short[]
				{
					82,
					83
				}
			}
		};
		short[] idArray;
		bool flag = map.TryGetValue(orgTemplateId, out idArray);
		StoryScrollItem result;
		if (flag)
		{
			result = StoryScroll.Instance.GetItem(idArray[prosper ? 0 : 1]);
		}
		else
		{
			result = null;
		}
		return result;
	}

	// Token: 0x060032CD RID: 13005 RVA: 0x00190AB0 File Offset: 0x0018ECB0
	private void RefreshTexture(int orgTemplateId, bool prosper)
	{
		StoryScrollItem scrollConfig = this.GetScrollConfig(orgTemplateId, prosper);
		bool flag = scrollConfig != null;
		if (flag)
		{
			string texturePath = "RemakeResources/Textures/SectStory/" + scrollConfig.StoryImage;
			ResLoader.Load<Texture2D>(texturePath, delegate(Texture2D texture)
			{
				this.Page.texture = texture;
			}, null, false);
			Refers noteRefers = base.CGet<Refers>("NoteArea");
			noteRefers.CGet<TextMeshProUGUI>("Note").text = scrollConfig.StoryNote;
			noteRefers.CGet<TextMeshProUGUI>("ResultMark").color = Colors.Instance["scrollchapter"];
			noteRefers.CGet<CImage>("TypeIcon").SetSprite(scrollConfig.StoryTypeIcon, false, null);
			noteRefers.CGet<CImage>("ChapterBaseImg").SetSprite(prosper ? "gamelinescroll_iconbase_2" : "gamelinescroll_iconbase_1", false, null);
		}
		base.CGet<TextMeshProUGUI>("AreaName").text = Organization.Instance[orgTemplateId].SectMainStory.Name;
		base.CGet<CImage>("NameImage").SetSprite(string.Format("ui_scroll_sect_name_{0}", orgTemplateId - 1), false, null);
		base.CGet<CImage>("StatusImage").SetSprite(prosper ? "ui_scroll_sect_img_feature_0" : "ui_scroll_sect_img_feature_1", false, null);
	}

	// Token: 0x060032CE RID: 13006 RVA: 0x00190BE8 File Offset: 0x0018EDE8
	private void ResetView()
	{
		base.CGet<CanvasGroup>("Canvas").alpha = 0f;
		base.CGet<Refers>("NoteArea").gameObject.SetActive(false);
		this.Page.enabled = false;
		this.ChangeCloseActive(false);
	}

	// Token: 0x060032CF RID: 13007 RVA: 0x00190C38 File Offset: 0x0018EE38
	private void PlayExpand()
	{
		this.ResetView();
		this._playingSequence = this.NewExpandSequence;
		this._playingSequence.PlayForward();
		AudioManager.Instance.PlaySound("UI_GameLineScroll_OpenScroll", false, false);
	}

	// Token: 0x060032D0 RID: 13008 RVA: 0x00190C6C File Offset: 0x0018EE6C
	private void SkipExpandAnimation()
	{
		Sequence playingSequence = this._playingSequence;
		if (playingSequence != null)
		{
			playingSequence.Complete();
		}
		this.ScrollLeft.localPosition = new Vector3(this.ScrollTergetPos.x, this.ScrollLeft.localPosition.y, this.ScrollLeft.localPosition.z);
		this.ScrollRight.localPosition = new Vector3(this.ScrollReadyPos.x, this.ScrollRight.localPosition.y, this.ScrollRight.localPosition.z);
		base.CGet<CanvasGroup>("Canvas").alpha = 1f;
		this.Page.enabled = true;
		base.CGet<Refers>("NoteArea").gameObject.SetActive(true);
		this.ScrollLeft.gameObject.SetActive(true);
		this.ShowClose();
	}

	// Token: 0x060032D1 RID: 13009 RVA: 0x00190D58 File Offset: 0x0018EF58
	private void SkipCloseAnimation()
	{
		Sequence playingSequence = this._playingSequence;
		if (playingSequence != null)
		{
			playingSequence.Complete();
		}
		this.ScrollLeft.localPosition = new Vector3(this.ScrollInitPos.x, this.ScrollLeft.localPosition.y, this.ScrollLeft.localPosition.z);
		this.ScrollRight.localPosition = new Vector3(this.ScrollInitPos.x, this.ScrollRight.localPosition.y, this.ScrollRight.localPosition.z);
		base.CGet<CanvasGroup>("Canvas").alpha = 0f;
		this.Page.enabled = false;
		base.CGet<Refers>("NoteArea").gameObject.SetActive(false);
		this.ScrollLeft.gameObject.SetActive(true);
		this.HideSelf();
	}

	// Token: 0x060032D2 RID: 13010 RVA: 0x00190E43 File Offset: 0x0018F043
	private void ShowClose()
	{
		this.ChangeCloseActive(true);
		this._isExpand = true;
		base.CGet<LeftMouseSkipAnimNotice>("LeftMouseSkipAnimNotice").gameObject.SetActive(false);
	}

	// Token: 0x060032D3 RID: 13011 RVA: 0x00190E6C File Offset: 0x0018F06C
	private void HideSelf()
	{
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x04002528 RID: 9512
	private const string TexturePrefix = "RemakeResources/Textures/SectStory/";

	// Token: 0x04002529 RID: 9513
	private const float FadeDuration = 0.5f;

	// Token: 0x0400252A RID: 9514
	private const float ReadyDuration = 1f;

	// Token: 0x0400252B RID: 9515
	private const float ExpandDuration = 0.8f;

	// Token: 0x0400252C RID: 9516
	private Sequence _playingSequence;

	// Token: 0x0400252D RID: 9517
	private bool _isExpand;
}
