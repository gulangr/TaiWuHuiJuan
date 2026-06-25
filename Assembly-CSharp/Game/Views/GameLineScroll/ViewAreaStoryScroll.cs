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

namespace Game.Views.GameLineScroll
{
	// Token: 0x02000A1E RID: 2590
	public class ViewAreaStoryScroll : UIBase
	{
		// Token: 0x06007F15 RID: 32533 RVA: 0x003B38A8 File Offset: 0x003B1AA8
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
			this.leftMouseSkipAnimNotice.gameObject.SetActive(true);
			this._isExpand = false;
			this.SetScrollMask(true);
		}

		// Token: 0x06007F16 RID: 32534 RVA: 0x003B3914 File Offset: 0x003B1B14
		private void Update()
		{
			bool flag = CommonCommandKit.LeftMouse.Check(this.Element, false, false, false, true, false) && this.leftMouseSkipAnimNotice.IsSkipEnabled && this.leftMouseSkipAnimNotice.gameObject.activeSelf;
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

		// Token: 0x06007F17 RID: 32535 RVA: 0x003B3980 File Offset: 0x003B1B80
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "Close";
			if (flag)
			{
				this.OnClickClose();
			}
		}

		// Token: 0x17000DC2 RID: 3522
		// (get) Token: 0x06007F18 RID: 32536 RVA: 0x003B39AC File Offset: 0x003B1BAC
		private Sequence NewExpandSequence
		{
			get
			{
				Sequence expandSequence = DOTween.Sequence();
				expandSequence.Append(this.scrollRight.DOLocalMoveX(this.scrollReadyPos.localPosition.x, 1f, false));
				expandSequence.Join(this.canvas.DOFade(1f, 0.5f).OnStart(delegate
				{
					this.scrollLeft.gameObject.SetActive(false);
				}).OnComplete(delegate
				{
					this.scrollLeft.gameObject.SetActive(true);
				}));
				expandSequence.Join(this.scrollLeft.DOLocalMoveX(this.scrollReadyPos.localPosition.x, 1f, false));
				expandSequence.Append(this.scrollLeft.DOLocalMoveX(this.scrollTergetPos.localPosition.x, 0.8f, false).OnStart(delegate
				{
					this.page.enabled = true;
					this.noteArea.gameObject.SetActive(true);
				}));
				expandSequence.AppendCallback(new TweenCallback(this.ShowClose));
				return expandSequence;
			}
		}

		// Token: 0x17000DC3 RID: 3523
		// (get) Token: 0x06007F19 RID: 32537 RVA: 0x003B3AA0 File Offset: 0x003B1CA0
		private Sequence NewCloseSequence
		{
			get
			{
				Sequence closeSequence = DOTween.Sequence();
				closeSequence.Append(this.scrollLeft.DOLocalMoveX(this.scrollReadyPos.localPosition.x, 0.8f, false).OnComplete(delegate
				{
					this.page.enabled = false;
					this.noteArea.gameObject.SetActive(false);
				}));
				closeSequence.Append(this.scrollRight.DOLocalMoveX(this.scrollInitPos.localPosition.x, 1f, false));
				closeSequence.Join(this.scrollLeft.DOLocalMoveX(this.scrollInitPos.localPosition.x, 1f, false));
				closeSequence.Append(this.canvas.DOFade(0f, 0.5f).OnStart(delegate
				{
					this.scrollLeft.gameObject.SetActive(false);
				}).OnComplete(delegate
				{
					this.scrollLeft.gameObject.SetActive(true);
				}));
				closeSequence.AppendCallback(new TweenCallback(this.HideSelf));
				return closeSequence;
			}
		}

		// Token: 0x06007F1A RID: 32538 RVA: 0x003B3B93 File Offset: 0x003B1D93
		private void ChangeCloseActive(bool inActive)
		{
			this.close.SetActive(inActive);
		}

		// Token: 0x06007F1B RID: 32539 RVA: 0x003B3BA4 File Offset: 0x003B1DA4
		public override void QuickHide()
		{
			bool flag = !this.close.activeSelf;
			if (!flag)
			{
				this.OnClickClose();
			}
		}

		// Token: 0x06007F1C RID: 32540 RVA: 0x003B3BD0 File Offset: 0x003B1DD0
		private void OnClickClose()
		{
			this.ChangeCloseActive(false);
			this.leftMouseSkipAnimNotice.gameObject.SetActive(true);
			Sequence playingSequence = this._playingSequence;
			if (playingSequence != null)
			{
				playingSequence.Kill(false);
			}
			this._playingSequence = this.NewCloseSequence;
			this._playingSequence.PlayForward();
			AudioManager.Instance.PlaySound("UI_GameLineScroll_CloseScroll", false, false);
		}

		// Token: 0x06007F1D RID: 32541 RVA: 0x003B3C35 File Offset: 0x003B1E35
		private void OnDisable()
		{
			this.SetScrollMask(false);
			TaiwuEventDomainMethod.Call.TriggerListener("SectMainStoryScrollShowed", true);
		}

		// Token: 0x06007F1E RID: 32542 RVA: 0x003B3C4C File Offset: 0x003B1E4C
		private void SetScrollMask(bool enable)
		{
			bool flag = this.maskTrans == null;
			if (!flag)
			{
				if (enable)
				{
					UIManager.Instance.MaskComponent(this.maskTrans);
				}
				else
				{
					UIManager.Instance.UnMaskComponent(this.maskTrans);
				}
			}
		}

		// Token: 0x06007F1F RID: 32543 RVA: 0x003B3C98 File Offset: 0x003B1E98
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

		// Token: 0x06007F20 RID: 32544 RVA: 0x003B3E48 File Offset: 0x003B2048
		private void RefreshTexture(int orgTemplateId, bool prosper)
		{
			StoryScrollItem scrollConfig = this.GetScrollConfig(orgTemplateId, prosper);
			bool flag = scrollConfig != null;
			if (flag)
			{
				string texturePath = "RemakeResources/Textures/SectStory/" + scrollConfig.StoryImage;
				ResLoader.Load<Texture2D>(texturePath, delegate(Texture2D texture)
				{
					this.page.texture = texture;
				}, null, false);
				Refers noteRefers = this.noteArea;
				noteRefers.CGet<TextMeshProUGUI>("Note").text = scrollConfig.StoryNote;
				noteRefers.CGet<CImage>("ResultMark").SetSprite(string.Format("{0}{1}", "ui9_icon_scroll_sect_seal_", (int)scrollConfig.StoryResultMark), false, null);
				noteRefers.CGet<CImage>("TypeIcon").SetSprite(scrollConfig.StoryTypeIcon, false, null);
				noteRefers.CGet<CImage>("ChapterBaseImg").SetSprite(prosper ? "gamelinescroll_iconbase_2" : "gamelinescroll_iconbase_1", false, null);
			}
			this.areaName.text = Organization.Instance[orgTemplateId].SectMainStory.Name;
			this.statusImage.SetSprite(scrollConfig.StoryCharm, false, null);
			this.scrollLeft.GetComponent<CImage>().SetSprite(scrollConfig.StoryEnd, false, null);
			this.scrollRight.GetComponent<CImage>().SetSprite("ui9_back_sect_scroll_0_" + ((orgTemplateId - 1) / 5).ToString(), false, null);
		}

		// Token: 0x06007F21 RID: 32545 RVA: 0x003B3F91 File Offset: 0x003B2191
		private void ResetView()
		{
			this.canvas.alpha = 0f;
			this.noteArea.gameObject.SetActive(false);
			this.page.enabled = false;
			this.ChangeCloseActive(false);
		}

		// Token: 0x06007F22 RID: 32546 RVA: 0x003B3FCC File Offset: 0x003B21CC
		private void PlayExpand()
		{
			this.ResetView();
			this._playingSequence = this.NewExpandSequence;
			this._playingSequence.PlayForward();
			AudioManager.Instance.PlaySound("UI_GameLineScroll_OpenScroll", false, false);
		}

		// Token: 0x06007F23 RID: 32547 RVA: 0x003B4000 File Offset: 0x003B2200
		private void SkipExpandAnimation()
		{
			Sequence playingSequence = this._playingSequence;
			if (playingSequence != null)
			{
				playingSequence.Complete();
			}
			this.scrollLeft.localPosition = new Vector3(this.scrollTergetPos.localPosition.x, this.scrollLeft.localPosition.y, this.scrollLeft.localPosition.z);
			this.scrollRight.localPosition = new Vector3(this.scrollReadyPos.localPosition.x, this.scrollRight.localPosition.y, this.scrollRight.localPosition.z);
			this.canvas.alpha = 1f;
			this.page.enabled = true;
			this.noteArea.gameObject.SetActive(true);
			this.scrollLeft.gameObject.SetActive(true);
			this.ShowClose();
		}

		// Token: 0x06007F24 RID: 32548 RVA: 0x003B40EC File Offset: 0x003B22EC
		private void SkipCloseAnimation()
		{
			Sequence playingSequence = this._playingSequence;
			if (playingSequence != null)
			{
				playingSequence.Complete();
			}
			this.scrollLeft.localPosition = new Vector3(this.scrollInitPos.localPosition.x, this.scrollLeft.localPosition.y, this.scrollLeft.localPosition.z);
			this.scrollRight.localPosition = new Vector3(this.scrollInitPos.localPosition.x, this.scrollRight.localPosition.y, this.scrollRight.localPosition.z);
			this.canvas.alpha = 0f;
			this.page.enabled = false;
			this.noteArea.gameObject.SetActive(false);
			this.scrollLeft.gameObject.SetActive(true);
			this.HideSelf();
		}

		// Token: 0x06007F25 RID: 32549 RVA: 0x003B41D7 File Offset: 0x003B23D7
		private void ShowClose()
		{
			this.ChangeCloseActive(true);
			this._isExpand = true;
			this.leftMouseSkipAnimNotice.gameObject.SetActive(false);
		}

		// Token: 0x06007F26 RID: 32550 RVA: 0x003B41FB File Offset: 0x003B23FB
		private void HideSelf()
		{
			this.SetScrollMask(false);
			UIManager.Instance.HideUI(this.Element);
		}

		// Token: 0x04006136 RID: 24886
		private const string TexturePrefix = "RemakeResources/Textures/SectStory/";

		// Token: 0x04006137 RID: 24887
		private const float FadeDuration = 0.5f;

		// Token: 0x04006138 RID: 24888
		private const float ReadyDuration = 1f;

		// Token: 0x04006139 RID: 24889
		private const float ExpandDuration = 0.8f;

		// Token: 0x0400613A RID: 24890
		private Sequence _playingSequence;

		// Token: 0x0400613B RID: 24891
		private bool _isExpand;

		// Token: 0x0400613C RID: 24892
		[SerializeField]
		private RectTransform scrollLeft;

		// Token: 0x0400613D RID: 24893
		[SerializeField]
		private RectTransform scrollRight;

		// Token: 0x0400613E RID: 24894
		[SerializeField]
		private RectTransform scrollInitPos;

		// Token: 0x0400613F RID: 24895
		[SerializeField]
		private RectTransform scrollReadyPos;

		// Token: 0x04006140 RID: 24896
		[SerializeField]
		private RectTransform scrollTergetPos;

		// Token: 0x04006141 RID: 24897
		[SerializeField]
		private CRawImage page;

		// Token: 0x04006142 RID: 24898
		[SerializeField]
		private LeftMouseSkipAnimNotice leftMouseSkipAnimNotice;

		// Token: 0x04006143 RID: 24899
		[SerializeField]
		private CanvasGroup canvas;

		// Token: 0x04006144 RID: 24900
		[SerializeField]
		private Refers noteArea;

		// Token: 0x04006145 RID: 24901
		[SerializeField]
		private GameObject close;

		// Token: 0x04006146 RID: 24902
		[SerializeField]
		private TextMeshProUGUI areaName;

		// Token: 0x04006147 RID: 24903
		[SerializeField]
		private CImage statusImage;

		// Token: 0x04006148 RID: 24904
		[SerializeField]
		private RectTransform maskTrans;
	}
}
