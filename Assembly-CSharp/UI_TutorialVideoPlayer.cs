using System;
using System.Collections.Generic;
using System.IO;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.CommandSystem;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x020003B5 RID: 949
public class UI_TutorialVideoPlayer : UIBase
{
	// Token: 0x060038FB RID: 14587 RVA: 0x001CC9F0 File Offset: 0x001CABF0
	public override void OnInit(ArgumentBox argsBox)
	{
		this._videoPlayer = base.CGet<VideoPlayer>("VideoPlayer");
		bool flag = argsBox != null && argsBox.Get("VideoPathName", out this._videoPathName) && !string.IsNullOrEmpty(this._videoPathName);
		if (flag)
		{
			this.FindTutorialVideoConfig(this._videoPathName);
			this.GetShowConfigList();
			this.SetRefers();
			this.SetTitleText();
			this.SetToggleGroup();
		}
		UIElement element = this.Element;
		element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(this.StartPlay));
	}

	// Token: 0x060038FC RID: 14588 RVA: 0x001CCA8C File Offset: 0x001CAC8C
	private void SetToggleGroup()
	{
		this._leftInfinityScroll.UpdateData(this._showConfigList.Count);
		this._leftInfinityScroll.SelectedTogKey = this._showConfigList.Count - 1;
		this._leftToggleGroup.Set(this._showConfigList.Count - 1, true, true);
		this._partDesc.SetText(this._tutorialVideoConfig.PartsDesc[0].ColorReplace(), true);
	}

	// Token: 0x060038FD RID: 14589 RVA: 0x001CCB04 File Offset: 0x001CAD04
	private void SetTitleText()
	{
		this._chapterTitle.SetText(this._showConfigList[0].ChapterName, true);
	}

	// Token: 0x060038FE RID: 14590 RVA: 0x001CCB28 File Offset: 0x001CAD28
	private void FindTutorialVideoConfig(string videoPathName)
	{
		TutorialVideo.Instance.Iterate(delegate(TutorialVideoItem e)
		{
			bool flag = e.VideoPath == videoPathName;
			bool result;
			if (flag)
			{
				this._tutorialVideoConfig = e;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		});
	}

	// Token: 0x060038FF RID: 14591 RVA: 0x001CCB64 File Offset: 0x001CAD64
	private void GetShowConfigList()
	{
		this._showConfigList.Clear();
		for (int i = 0; i < TutorialVideo.Instance.Count; i++)
		{
			TutorialVideoItem tempConfig = TutorialVideo.Instance[i];
			bool flag = tempConfig.Chapter == this._tutorialVideoConfig.Chapter;
			if (flag)
			{
				this._showConfigList.Add(tempConfig);
			}
		}
		this._showConfigList.Sort((TutorialVideoItem l, TutorialVideoItem r) => l.SectionIndex.CompareTo(r.SectionIndex));
		for (int j = this._showConfigList.Count - 1; j >= 0; j--)
		{
			bool flag2 = this._showConfigList[j].VideoPath == this._videoPathName;
			if (flag2)
			{
				break;
			}
			this._showConfigList.RemoveAt(j);
		}
	}

	// Token: 0x06003900 RID: 14592 RVA: 0x001CCC50 File Offset: 0x001CAE50
	private void SetRefers()
	{
		bool isToggleGroupInit = this._isToggleGroupInit;
		if (!isToggleGroupInit)
		{
			this._leftInfinityScroll = base.CGet<InfinityScrollLegacy>("LeftInfinityScroll");
			this._leftToggleGroup = base.CGet<CToggleGroupObsolete>("LeftVerticalScrollView");
			this._leftInfinityScroll.OnItemRender = new Action<int, Refers>(this.OnItemRender);
			this._leftInfinityScroll.SetTogGroup(this._leftToggleGroup, false, false);
			this._leftToggleGroup.InitPreOnToggle(-1);
			this._leftToggleGroup.OnActiveToggleChange = new Action<CToggleObsolete, CToggleObsolete>(this.OnToggleChange);
			this._chapterTitle = base.CGet<TextMeshProUGUI>("ChapterTitle");
			this._partTitle = base.CGet<TextMeshProUGUI>("SectionTitle");
			this._partDesc = base.CGet<TextMeshProUGUI>("PartDesc");
			this._pointGroupTrans = base.CGet<RectTransform>("PointGroup").transform;
			this._pointBtnTemplate = base.CGet<CButtonObsolete>("PointTemplate");
			this._lastBtn = base.CGet<CButtonObsolete>("PartLastStep");
			this._nextBtn = base.CGet<CButtonObsolete>("PartNextStep");
			this._pointParent = base.CGet<GameObject>("PointParent");
			this._playBtn = base.CGet<CButtonObsolete>("PlayBtn");
			this._pauseBtn = base.CGet<CButtonObsolete>("PauseBtn");
			this._videoMask = base.CGet<GameObject>("Mask");
			this._effNextBtn = base.CGet<GameObject>("EffNextBtn");
			this._isToggleGroupInit = true;
		}
	}

	// Token: 0x06003901 RID: 14593 RVA: 0x001CCDB8 File Offset: 0x001CAFB8
	private void OnItemRender(int index, Refers refers)
	{
		TextMeshProUGUI offText = refers.CGet<TextMeshProUGUI>("OffText");
		TextMeshProUGUI onText = refers.CGet<TextMeshProUGUI>("OnText");
		TutorialVideoItem config = this._showConfigList[index];
		offText.SetText(config.Name, true);
		onText.SetText(config.Name, true);
	}

	// Token: 0x06003902 RID: 14594 RVA: 0x001CCE08 File Offset: 0x001CB008
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = "ButtonCancel" == btnName;
		if (flag)
		{
			this.QuickHide();
		}
		else
		{
			bool flag2 = "PauseBtn" == btnName;
			if (flag2)
			{
				this._videoPlayer.Pause();
				this._pauseBtn.gameObject.SetActive(false);
				this._playBtn.gameObject.SetActive(true);
				this._videoMask.SetActive(true);
			}
			else
			{
				bool flag3 = "PlayBtn" == btnName;
				if (flag3)
				{
					this._videoPlayer.Play();
					this._playBtn.gameObject.SetActive(false);
					this._pauseBtn.gameObject.SetActive(true);
					this._videoMask.SetActive(false);
				}
				else
				{
					bool flag4 = "SwitchToNormalAndPlay" == btnName;
					if (flag4)
					{
						bool flag5 = UIManager.Instance.IsFocusElement(UIElement.SystemOption);
						if (!flag5)
						{
							bool flag6 = Time.timeScale <= 0f;
							if (flag6)
							{
								this.SwitchToNormalAndPlay();
							}
							else
							{
								CommandManager.AddCommandAuto(EPriority.ShowUINormal, delegate
								{
									this.SwitchToNormalAndPlay();
									return true;
								}, () => UIManager.Instance.IsFocusElement(this.Element));
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06003903 RID: 14595 RVA: 0x001CCF47 File Offset: 0x001CB147
	public void HideSelf()
	{
	}

	// Token: 0x06003904 RID: 14596 RVA: 0x001CCF4A File Offset: 0x001CB14A
	public override void QuickHide()
	{
		this.SwitchToMiniSizeAndPause();
	}

	// Token: 0x06003905 RID: 14597 RVA: 0x001CCF54 File Offset: 0x001CB154
	private void Awake()
	{
		Debug.Log("prepare " + base.GetType().Name + " _renderTexture");
		this._renderTexture = new RenderTexture(Screen.width / 2, Screen.height / 2, 0);
		Debug.Log("completed " + base.GetType().Name + " _renderTexture");
		this._videoPlayer.renderMode = VideoRenderMode.RenderTexture;
		this._videoPlayer.targetTexture = this._renderTexture;
		this._videoPlayer.loopPointReached += this.PauseAtLoopPoint;
		base.CGet<CRawImage>("VideoRawImage").texture = this._renderTexture;
		this._miniSizeView = base.CGet<RectTransform>("MiniSize");
		this._uiMask = base.CGet<GameObject>("UIMask");
		this._bgRectTrans = base.CGet<RectTransform>("Bg");
		this._miniSizeView.gameObject.SetActive(false);
		this._bgRectTrans.gameObject.SetActive(true);
		this._raycastBlocker = base.CGet<GameObject>("RaycastBlocker");
		this._raycastBlocker.SetActive(false);
		GEvent.Add(UiEvents.RefreshVideo, new GEvent.Callback(this.EventRefreshVideo));
	}

	// Token: 0x06003906 RID: 14598 RVA: 0x001CD098 File Offset: 0x001CB298
	private void EventRefreshVideo(ArgumentBox argBox)
	{
		argBox.Get("TutorialVideoPathName", out this._videoPathName);
		this._needShowAdvanceMonth = false;
		this.FindTutorialVideoConfig(this._videoPathName);
		this.GetShowConfigList();
		this.SetToggleGroup();
		this.SwitchToNormalAndPlay();
	}

	// Token: 0x06003907 RID: 14599 RVA: 0x001CD0D6 File Offset: 0x001CB2D6
	private void EventShowedAndFurlVideo(ArgumentBox argBox)
	{
		this.SwitchToMiniSizeAndPause();
	}

	// Token: 0x06003908 RID: 14600 RVA: 0x001CD0E0 File Offset: 0x001CB2E0
	private void OnDestroy()
	{
		this._needShowAdvanceMonth = false;
		this._showConfigList.Clear();
		GEvent.Remove(UiEvents.RefreshVideo, new GEvent.Callback(this.EventRefreshVideo));
	}

	// Token: 0x06003909 RID: 14601 RVA: 0x001CD110 File Offset: 0x001CB310
	private void Update()
	{
		bool flag = this._playBtn.gameObject.activeSelf && this._pauseBtn.gameObject.activeSelf;
		if (flag)
		{
			this._pauseBtn.gameObject.SetActive(false);
		}
		bool flag2 = this._bgRectTrans.gameObject.activeSelf && CommandKitBase.GetDisable();
		if (flag2)
		{
			bool flag3 = CommonCommandKit.Esc.Check(this.Element, false, false, true, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, true, true, false);
			if (flag3)
			{
				this.QuickHide();
			}
		}
	}

	// Token: 0x0600390A RID: 14602 RVA: 0x001CD1B8 File Offset: 0x001CB3B8
	private void OnToggleChange(CToggleObsolete newTog, CToggleObsolete oldTog)
	{
		bool flag = newTog == null;
		if (!flag)
		{
			this._tutorialVideoConfig = this._showConfigList[newTog.Key];
			this._videoPathName = this._tutorialVideoConfig.VideoPath;
			this.CreatePartsBtn();
			this.UpdatePartsBtn();
			this.UpdatePartsTitle();
			this.RefreshVideo();
			this._playBtn.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600390B RID: 14603 RVA: 0x001CD22C File Offset: 0x001CB42C
	private void SwitchToNormalAndPlay()
	{
		bool flag = DOTween.IsTweening(this._bgRectTrans, false);
		if (!flag)
		{
			bool exist = UIElement.CombatBegin.Exist;
			if (exist)
			{
				UIElement combatBegin = UIElement.CombatBegin;
				combatBegin.OnHide = (Action)Delegate.Combine(combatBegin.OnHide, new Action(this.SwitchToMiniSizeAndPause));
			}
			this._bgRectTrans.localScale = Vector3.zero;
			this._bgRectTrans.position = this._miniSizeView.position;
			this._bgRectTrans.gameObject.SetActive(true);
			this._miniSizeView.gameObject.SetActive(false);
			this._raycastBlocker.SetActive(true);
			this._videoPlayer.targetTexture = this._renderTexture;
			GEvent.OnEvent(UiEvents.OnNeedCombatPause, null);
			this._bgRectTrans.DOKill(false);
			this._bgRectTrans.DOScale(Vector3.one, 0.3f).SetUpdate(true).SetAutoKill(true);
			CommandKitBase.SetDisable(true);
			this._bgRectTrans.DOLocalMove(Vector3.zero, 0.3f, false).SetUpdate(true).SetAutoKill(true).OnComplete(delegate
			{
				CommandKitBase.SetDisable(false);
				this._uiMask.gameObject.SetActive(true);
				this._videoPlayer.Play();
				this._videoMask.SetActive(false);
				this._playBtn.gameObject.SetActive(false);
				this._pauseBtn.gameObject.SetActive(false);
				UIManager.Instance.InsertElementToTop(this.Element);
			});
		}
	}

	// Token: 0x0600390C RID: 14604 RVA: 0x001CD36C File Offset: 0x001CB56C
	private void SwitchToMiniSizeAndPause()
	{
		bool flag = DOTween.IsTweening(this._bgRectTrans, false);
		if (!flag)
		{
			UIManager.Instance.RemoveElement(this.Element);
			GEvent.OnEvent(UiEvents.OnNeedCombatResume, null);
			this._videoPlayer.Pause();
			this._bgRectTrans.localPosition = Vector3.zero;
			base.CGet<TextMeshProUGUI>("VideoSummary").text = this._showConfigList[this._showConfigList.Count - 1].VideoSummary.ColorReplace();
			this._bgRectTrans.DOKill(false);
			this._bgRectTrans.DOMove(this._miniSizeView.position, 0.3f, false).SetUpdate(true).SetAutoKill(true);
			this._bgRectTrans.DOScale(Vector3.zero, 0.3f).SetUpdate(true).SetAutoKill(true).OnComplete(delegate
			{
				this._bgRectTrans.gameObject.SetActive(false);
				this._uiMask.gameObject.SetActive(false);
				this._miniSizeView.gameObject.SetActive(true);
				this._raycastBlocker.SetActive(false);
				bool exist = UIElement.CombatBegin.Exist;
				if (exist)
				{
					UIElement combatBegin = UIElement.CombatBegin;
					combatBegin.OnHide = (Action)Delegate.Combine(combatBegin.OnHide, new Action(delegate()
					{
						this._miniSizeView.gameObject.SetActive(false);
					}));
				}
				bool exist2 = UIElement.Combat.Exist;
				if (exist2)
				{
					UIElement combat = UIElement.Combat;
					combat.OnHide = (Action)Delegate.Combine(combat.OnHide, new Action(delegate()
					{
						this._miniSizeView.gameObject.SetActive(false);
					}));
				}
				TaiwuEventDomainMethod.Call.TriggerListener("TutorialVedioLostFocus", true);
			});
		}
	}

	// Token: 0x0600390D RID: 14605 RVA: 0x001CD46C File Offset: 0x001CB66C
	private void UpdatePartsTitle()
	{
		this._partTitle.SetText(this._tutorialVideoConfig.PartsTitle[this._curPartIndex].ColorReplace(), true);
		this._partDesc.SetText(this._tutorialVideoConfig.PartsDesc[this._curPartIndex].ColorReplace(), true);
	}

	// Token: 0x0600390E RID: 14606 RVA: 0x001CD4C4 File Offset: 0x001CB6C4
	private void UpdatePartsBtn()
	{
		bool flag = this._curPartIndex == 0;
		if (flag)
		{
			this._lastBtn.interactable = false;
			this._lastBtn.GetComponent<DisableStyleRoot>().SetStyleEffect(true, false);
		}
		else
		{
			this._lastBtn.interactable = true;
			this._lastBtn.GetComponent<DisableStyleRoot>().SetStyleEffect(false, false);
			this._lastBtn.ClearAndAddListener(delegate
			{
				this._curPartIndex--;
				this.BtnClick();
			});
		}
		bool flag2 = this._curPartIndex == this._tutorialVideoConfig.PartVideos.Length - 1;
		if (flag2)
		{
			this._nextBtn.interactable = false;
			this._nextBtn.GetComponent<DisableStyleRoot>().SetStyleEffect(true, false);
		}
		else
		{
			this._nextBtn.interactable = true;
			this._nextBtn.GetComponent<DisableStyleRoot>().SetStyleEffect(false, false);
			this._nextBtn.ClearAndAddListener(delegate
			{
				this._curPartIndex++;
				this.BtnClick();
			});
		}
	}

	// Token: 0x0600390F RID: 14607 RVA: 0x001CD5B8 File Offset: 0x001CB7B8
	private void BtnClick()
	{
		for (int i = 0; i < this._curPointBtnList.Count; i++)
		{
			this._pointBtnList[i].GetComponent<CImage>().SetSprite("videoplayer_dian_0", false, null);
		}
		this._curPointBtnList[this._curPartIndex].GetComponent<CImage>().SetSprite("videoplayer_dian_1", false, null);
		this.UpdatePartsTitle();
		this.UpdatePartsBtn();
		this.RefreshVideo();
		this._playBtn.gameObject.SetActive(false);
	}

	// Token: 0x06003910 RID: 14608 RVA: 0x001CD64C File Offset: 0x001CB84C
	private void CreatePartsBtn()
	{
		this._pointParent.gameObject.SetActive(this._tutorialVideoConfig.PartVideos.Length > 1);
		for (int i = 0; i < this._pointBtnList.Count; i++)
		{
			this._pointBtnList[i].gameObject.SetActive(false);
		}
		for (int j = this._pointBtnList.Count; j < this._tutorialVideoConfig.PartVideos.Length; j++)
		{
			CButtonObsolete pointBtn = Object.Instantiate<CButtonObsolete>(this._pointBtnTemplate, this._pointGroupTrans);
			pointBtn.gameObject.SetActive(false);
			this._pointBtnList.Add(pointBtn);
		}
		this._curPointBtnList.Clear();
		for (int k = 0; k < this._tutorialVideoConfig.PartVideos.Length; k++)
		{
			this._pointBtnList[k].gameObject.SetActive(true);
			this._curPointBtnList.Add(this._pointBtnList[k]);
			int curIndex = k;
			this._pointBtnList[k].ClearAndAddListener(delegate
			{
				bool flag = this._curPartIndex == curIndex;
				if (!flag)
				{
					this._curPartIndex = curIndex;
					this.BtnClick();
				}
			});
		}
		this._curPartIndex = 0;
		for (int l = 0; l < this._curPointBtnList.Count; l++)
		{
			this._pointBtnList[l].GetComponent<CImage>().SetSprite("videoplayer_dian_0", false, null);
		}
		this._curPointBtnList[this._curPartIndex].GetComponent<CImage>().SetSprite("videoplayer_dian_1", false, null);
	}

	// Token: 0x06003911 RID: 14609 RVA: 0x001CD810 File Offset: 0x001CBA10
	private void OnEnable()
	{
		this._bgRectTrans.gameObject.SetActive(true);
		this._miniSizeView.gameObject.SetActive(false);
		this._raycastBlocker.SetActive(true);
		this._playBtn.gameObject.SetActive(false);
		this._pauseBtn.gameObject.SetActive(false);
		this._bgRectTrans.localScale = Vector3.one;
		this._bgRectTrans.localPosition = Vector3.zero;
		this._uiMask.gameObject.SetActive(true);
		GEvent.Add(UiEvents.OnNewEventComingToShow, new GEvent.Callback(this.EventShowedAndFurlVideo));
	}

	// Token: 0x06003912 RID: 14610 RVA: 0x001CD8C0 File Offset: 0x001CBAC0
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.OnNewEventComingToShow, new GEvent.Callback(this.EventShowedAndFurlVideo));
	}

	// Token: 0x06003913 RID: 14611 RVA: 0x001CD8DC File Offset: 0x001CBADC
	private void StartPlay()
	{
		bool flag = null == this._videoPlayer.clip;
		if (!flag)
		{
			this._videoPlayer.SetDirectAudioVolume(0, (float)SingletonObject.getInstance<GlobalSettings>().VideoVolume / 100f);
			this._videoPlayer.Play();
			this._playBtn.gameObject.SetActive(false);
			this._pauseBtn.gameObject.SetActive(false);
			this._videoMask.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003914 RID: 14612 RVA: 0x001CD961 File Offset: 0x001CBB61
	private void PauseAtLoopPoint(VideoPlayer source)
	{
		this._videoPlayer.Pause();
		this._pauseBtn.gameObject.SetActive(false);
		this._playBtn.gameObject.SetActive(true);
		this._videoMask.SetActive(true);
	}

	// Token: 0x06003915 RID: 14613 RVA: 0x001CD9A4 File Offset: 0x001CBBA4
	private void ResourceLoadVideoClip(string videoPathName)
	{
		this._videoPlayer.clip = Resources.Load<VideoClip>(videoPathName);
		bool flag = !this._videoClipCash.ContainsKey(videoPathName);
		if (flag)
		{
			this._videoClipCash.Add(videoPathName, this._videoPlayer.clip);
		}
	}

	// Token: 0x06003916 RID: 14614 RVA: 0x001CD9F0 File Offset: 0x001CBBF0
	public void RefreshVideo()
	{
		string videoName = Path.Combine("TutorialVideos/", this._tutorialVideoConfig.VideoPath, this._tutorialVideoConfig.PartVideos[this._curPartIndex]).PathFix();
		bool hasNextVideo = this._tutorialVideoConfig.PartVideos.CheckIndex(this._curPartIndex + 1);
		bool flag = hasNextVideo;
		if (flag)
		{
			string nextVideoName = Path.Combine("TutorialVideos/", this._tutorialVideoConfig.VideoPath, this._tutorialVideoConfig.PartVideos[this._curPartIndex + 1]).PathFix();
			this._effNextBtn.gameObject.SetActive(!this._videoClipCash.ContainsKey(nextVideoName) && this._nextBtn.interactable);
		}
		else
		{
			this._effNextBtn.gameObject.SetActive(false);
		}
		bool flag2 = this._videoClipCash.ContainsKey(videoName);
		if (flag2)
		{
			this._videoPlayer.clip = this._videoClipCash[videoName];
		}
		else
		{
			this.ResourceLoadVideoClip(videoName);
		}
		this.StartPlay();
	}

	// Token: 0x06003917 RID: 14615 RVA: 0x001CDAFA File Offset: 0x001CBCFA
	public void OnPointerEnterVideoArea()
	{
		this._pauseBtn.gameObject.SetActive(this._videoPlayer.isPlaying);
		this._playBtn.gameObject.SetActive(!this._videoPlayer.isPlaying);
	}

	// Token: 0x06003918 RID: 14616 RVA: 0x001CDB38 File Offset: 0x001CBD38
	public void OnPointerExitVideoArea()
	{
		bool isPlaying = this._videoPlayer.isPlaying;
		if (isPlaying)
		{
			this._pauseBtn.gameObject.SetActive(false);
		}
		this._playBtn.gameObject.SetActive(!this._videoPlayer.isPlaying);
	}

	// Token: 0x04002939 RID: 10553
	private VideoPlayer _videoPlayer;

	// Token: 0x0400293A RID: 10554
	private RenderTexture _renderTexture;

	// Token: 0x0400293B RID: 10555
	private const string TutorialVideoFolder = "TutorialVideos/";

	// Token: 0x0400293C RID: 10556
	private CToggleGroupObsolete _leftToggleGroup;

	// Token: 0x0400293D RID: 10557
	private InfinityScrollLegacy _leftInfinityScroll;

	// Token: 0x0400293E RID: 10558
	private bool _isToggleGroupInit;

	// Token: 0x0400293F RID: 10559
	private TutorialVideoItem _tutorialVideoConfig;

	// Token: 0x04002940 RID: 10560
	private List<TutorialVideoItem> _showConfigList = new List<TutorialVideoItem>();

	// Token: 0x04002941 RID: 10561
	private TextMeshProUGUI _chapterTitle;

	// Token: 0x04002942 RID: 10562
	private TextMeshProUGUI _partTitle;

	// Token: 0x04002943 RID: 10563
	private TextMeshProUGUI _partDesc;

	// Token: 0x04002944 RID: 10564
	private CButtonObsolete _pointBtnTemplate;

	// Token: 0x04002945 RID: 10565
	private List<CButtonObsolete> _pointBtnList = new List<CButtonObsolete>();

	// Token: 0x04002946 RID: 10566
	private List<CButtonObsolete> _curPointBtnList = new List<CButtonObsolete>();

	// Token: 0x04002947 RID: 10567
	private Transform _pointGroupTrans;

	// Token: 0x04002948 RID: 10568
	private int _curPartIndex;

	// Token: 0x04002949 RID: 10569
	private Dictionary<string, VideoClip> _videoClipCash = new Dictionary<string, VideoClip>();

	// Token: 0x0400294A RID: 10570
	private string _videoPathName = string.Empty;

	// Token: 0x0400294B RID: 10571
	private CButtonObsolete _lastBtn;

	// Token: 0x0400294C RID: 10572
	private CButtonObsolete _nextBtn;

	// Token: 0x0400294D RID: 10573
	private CButtonObsolete _playBtn;

	// Token: 0x0400294E RID: 10574
	private CButtonObsolete _pauseBtn;

	// Token: 0x0400294F RID: 10575
	private GameObject _pointParent;

	// Token: 0x04002950 RID: 10576
	private GameObject _effNextBtn;

	// Token: 0x04002951 RID: 10577
	private GameObject _videoMask;

	// Token: 0x04002952 RID: 10578
	private RectTransform _miniSizeView;

	// Token: 0x04002953 RID: 10579
	private GameObject _uiMask;

	// Token: 0x04002954 RID: 10580
	private GameObject _raycastBlocker;

	// Token: 0x04002955 RID: 10581
	private RectTransform _bgRectTrans;

	// Token: 0x04002956 RID: 10582
	private bool _needShowAdvanceMonth;
}
