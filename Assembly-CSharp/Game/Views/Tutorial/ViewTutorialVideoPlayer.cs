using System;
using System.Collections.Generic;
using System.IO;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace Game.Views.Tutorial
{
	// Token: 0x0200074B RID: 1867
	public class ViewTutorialVideoPlayer : UIBase
	{
		// Token: 0x06005A85 RID: 23173 RVA: 0x0029FCA0 File Offset: 0x0029DEA0
		public override void OnInit(ArgumentBox argsBox)
		{
			this._settingData = SingletonObject.getInstance<GlobalSettings>();
			this.InitRefers();
			this.InitChapterToggleGroup();
			this.RefreshLeftToggle(argsBox);
			this.RefreshChapterSwitchButton();
			UIElement element = this.Element;
			element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(this.StartPlay));
		}

		// Token: 0x06005A86 RID: 23174 RVA: 0x0029FCFD File Offset: 0x0029DEFD
		private void EventRefreshVideo(ArgumentBox argsBox)
		{
			this.RefreshLeftToggle(argsBox);
			this.SwitchToNormalAndPlay();
		}

		// Token: 0x06005A87 RID: 23175 RVA: 0x0029FD10 File Offset: 0x0029DF10
		private void TopUiChanged(ArgumentBox argsBox)
		{
			bool flag = UIManager.Instance.IsElementActive(UIElement.Loading);
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x06005A88 RID: 23176 RVA: 0x0029FD40 File Offset: 0x0029DF40
		private void InitFromBox(ArgumentBox argBox)
		{
			bool flag = argBox.Get("TutorialVideoTemplateId", out this._tutorialVideoTemplateId);
			if (flag)
			{
				this._currTutorialVideoConfig = TutorialVideo.Instance[this._tutorialVideoTemplateId];
				this._triggeredTutorialVideoConfig = TutorialVideo.Instance[this._tutorialVideoTemplateId];
				this._curChapterIndex = (int)this._currTutorialVideoConfig.Chapter;
			}
			else
			{
				argBox.Get("TutorialVideoPathName", out this._videoPathName);
				TutorialVideo.Instance.Iterate(delegate(TutorialVideoItem e)
				{
					bool flag2 = e.VideoPath == this._videoPathName;
					bool result;
					if (flag2)
					{
						this._currTutorialVideoConfig = e;
						this._triggeredTutorialVideoConfig = e;
						this._curChapterIndex = (int)this._currTutorialVideoConfig.Chapter;
						result = false;
					}
					else
					{
						result = true;
					}
					return result;
				});
			}
			this.videoSummary.text = this._triggeredTutorialVideoConfig.VideoSummary.ColorReplace();
		}

		// Token: 0x06005A89 RID: 23177 RVA: 0x0029FDEB File Offset: 0x0029DFEB
		private void RebuildToggleGroup()
		{
			this.leftPartToggleHolder.Rebuild<CToggle>(this._showConfigList.Count, delegate(CToggle toggle, int index)
			{
				TutorialVideoItem config = this._showConfigList[index];
				TextMeshProUGUI title = toggle.GetComponentInChildren<TextMeshProUGUI>();
				title.SetText(config.Name, true);
				bool flag = this.leftToggleGroup.CanAddToggle(toggle);
				if (flag)
				{
					this.leftToggleGroup.Add(toggle);
				}
			});
		}

		// Token: 0x06005A8A RID: 23178 RVA: 0x0029FE14 File Offset: 0x0029E014
		private int FindConfigIndex(TutorialVideoItem findConfig)
		{
			for (int i = 0; i < this._showConfigList.Count; i++)
			{
				TutorialVideoItem showConfig = this._showConfigList[i];
				bool flag = showConfig.TemplateId == findConfig.TemplateId;
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06005A8B RID: 23179 RVA: 0x0029FE68 File Offset: 0x0029E068
		private void RefreshShowConfigList(short chapterIndex)
		{
			this._showConfigList.Clear();
			for (int i = 0; i < TutorialVideo.Instance.Count; i++)
			{
				TutorialVideoItem tempConfig = TutorialVideo.Instance[i];
				bool flag = tempConfig.Chapter == chapterIndex;
				if (flag)
				{
					this._showConfigList.Add(tempConfig);
				}
			}
			bool flag2 = (int)SingletonObject.getInstance<TutorialChapterModel>().TutorialChapterIndex == this._settingData.CompletedChapters && this._curChapterIndex == this._settingData.CompletedChapters;
			if (flag2)
			{
				this._showConfigList.Sort((TutorialVideoItem l, TutorialVideoItem r) => l.SectionIndex.CompareTo(r.SectionIndex));
				for (int j = this._showConfigList.Count - 1; j >= 0; j--)
				{
					bool flag3 = this._showConfigList[j].TemplateId == this._tutorialVideoTemplateId;
					if (flag3)
					{
						break;
					}
					this._showConfigList.RemoveAt(j);
				}
			}
		}

		// Token: 0x06005A8C RID: 23180 RVA: 0x0029FF80 File Offset: 0x0029E180
		private void InitRefers()
		{
			bool isInit = this._isInit;
			if (!isInit)
			{
				this._isInit = true;
				this.leftToggleGroup.Init(0);
				this.leftToggleGroup.OnActiveIndexChange += this.OnLeftToggleChange;
				this.chapterToggleGroup.Init(0);
				ToggleGroupHotkeyController.Set(this.Element, this.chapterToggleGroup, 0, null);
				this.chapterToggleGroup.OnActiveIndexChange += this.OnChapterToggleChange;
				this.videoPointerTrigger.EnterEvent.RemoveAllListeners();
				this.videoPointerTrigger.ExitEvent.RemoveAllListeners();
				this.videoPointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnPointerEnterVideoArea));
				this.videoPointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnPointerExitVideoArea));
				this.InitBtn();
			}
		}

		// Token: 0x06005A8D RID: 23181 RVA: 0x002A0068 File Offset: 0x0029E268
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
					this.videoPlayer.Pause();
					this.pauseBtn.gameObject.SetActive(false);
					this.playBtn.gameObject.SetActive(true);
					this.videoMask.SetActive(true);
				}
				else
				{
					bool flag3 = "PlayBtn" == btnName;
					if (flag3)
					{
						this.videoPlayer.Play();
						this.playBtn.gameObject.SetActive(false);
						this.pauseBtn.gameObject.SetActive(true);
						this.videoMask.SetActive(false);
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

		// Token: 0x06005A8E RID: 23182 RVA: 0x002A01A7 File Offset: 0x0029E3A7
		public void HideSelf()
		{
		}

		// Token: 0x06005A8F RID: 23183 RVA: 0x002A01AA File Offset: 0x0029E3AA
		public override void QuickHide()
		{
			this.SwitchToMiniSizeAndPause();
		}

		// Token: 0x06005A90 RID: 23184 RVA: 0x002A01B4 File Offset: 0x0029E3B4
		private void Awake()
		{
			this._renderTexture = new RenderTexture(Screen.width / 2, Screen.height / 2, 0);
			this.videoPlayer.renderMode = VideoRenderMode.RenderTexture;
			this.videoPlayer.targetTexture = this._renderTexture;
			this.videoPlayer.loopPointReached += this.PauseAtLoopPoint;
			this.videoRawImage.texture = this._renderTexture;
			this.miniSizeView.gameObject.SetActive(false);
			this.bgRectTrans.gameObject.SetActive(true);
			GEvent.Add(UiEvents.RefreshVideo, new GEvent.Callback(this.EventRefreshVideo));
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
			this.prevClass.onClick.ResetListener(delegate()
			{
				this.RefreshChapterSwitchButton();
				bool flag = this._curChapterIndex > 0;
				if (flag)
				{
					CToggleGroup ctoggleGroup = this.chapterToggleGroup;
					int num = this._curChapterIndex - 1;
					this._curChapterIndex = num;
					ctoggleGroup.Set(num, true);
				}
			});
			this.nextClass.onClick.ResetListener(delegate()
			{
				this.RefreshChapterSwitchButton();
				bool flag = this._curChapterIndex < this.chapterToggleGroup.Count() - 1;
				if (flag)
				{
					CToggleGroup ctoggleGroup = this.chapterToggleGroup;
					int num = this._curChapterIndex + 1;
					this._curChapterIndex = num;
					ctoggleGroup.Set(num, true);
				}
			});
		}

		// Token: 0x06005A91 RID: 23185 RVA: 0x002A02B4 File Offset: 0x0029E4B4
		private void RefreshChapterSwitchButton()
		{
			this.prevClass.interactable = (this._curChapterIndex > 0);
			this.nextClass.interactable = (this._curChapterIndex < this._settingData.CompletedChapters);
		}

		// Token: 0x06005A92 RID: 23186 RVA: 0x002A02EC File Offset: 0x0029E4EC
		private void RefreshLeftToggle(ArgumentBox argsBox)
		{
			this.InitFromBox(argsBox);
			this.chapterToggleGroup.SetWithoutNotify(this._curChapterIndex);
			this.RefreshShowConfigList(this._currTutorialVideoConfig.Chapter);
			this.RebuildToggleGroup();
			int leftGroupIndex = this.FindConfigIndex(this._currTutorialVideoConfig);
			this.OnLeftToggleChange(leftGroupIndex, -1);
		}

		// Token: 0x06005A93 RID: 23187 RVA: 0x002A0343 File Offset: 0x0029E543
		private void EventShowedAndFurlVideo(ArgumentBox argBox)
		{
			this.SwitchToMiniSizeAndPause();
		}

		// Token: 0x06005A94 RID: 23188 RVA: 0x002A034D File Offset: 0x0029E54D
		private void OnDestroy()
		{
			this._showConfigList.Clear();
			GEvent.Remove(UiEvents.RefreshVideo, new GEvent.Callback(this.EventRefreshVideo));
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
		}

		// Token: 0x06005A95 RID: 23189 RVA: 0x002A0390 File Offset: 0x0029E590
		private void Update()
		{
			bool flag = this.playBtn.gameObject.activeSelf && this.pauseBtn.gameObject.activeSelf;
			if (flag)
			{
				this.pauseBtn.gameObject.SetActive(false);
			}
			bool flag2 = this.bgRectTrans.gameObject.activeSelf && CommandKitBase.GetDisable();
			if (flag2)
			{
				bool flag3 = CommonCommandKit.Esc.Check(this.Element, false, false, true, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, true, true, false);
				if (flag3)
				{
					this.QuickHide();
				}
			}
		}

		// Token: 0x06005A96 RID: 23190 RVA: 0x002A0438 File Offset: 0x0029E638
		private void OnLeftToggleChange(int newIndex, int oldIndex)
		{
			this._currTutorialVideoConfig = this._showConfigList[newIndex];
			this._videoPathName = this._currTutorialVideoConfig.VideoPath;
			this.leftToggleGroup.SetWithoutNotify(newIndex);
			this.SetPartsIndex();
			this.UpdatePartsBtn();
			this.UpdatePartsTitle();
			this.RefreshVideo();
			this.playBtn.gameObject.SetActive(false);
		}

		// Token: 0x06005A97 RID: 23191 RVA: 0x002A04A4 File Offset: 0x0029E6A4
		private void SwitchToNormalAndPlay()
		{
			bool flag = DOTween.IsTweening(this.bgRectTrans, false);
			if (!flag)
			{
				bool exist = UIElement.CombatBegin.Exist;
				if (exist)
				{
					UIElement combatBegin = UIElement.CombatBegin;
					combatBegin.OnHide = (Action)Delegate.Combine(combatBegin.OnHide, new Action(this.SwitchToMiniSizeAndPause));
				}
				this.bgRectTrans.localScale = Vector3.zero;
				this.bgRectTrans.position = this.miniSizeView.position;
				this.bgRectTrans.gameObject.SetActive(true);
				this.miniSizeView.gameObject.SetActive(false);
				this.videoPlayer.targetTexture = this._renderTexture;
				GEvent.OnEvent(UiEvents.TopUiChanged, null);
				GEvent.OnEvent(UiEvents.OnNeedCombatPause, null);
				this.bgRectTrans.DOKill(false);
				this.bgRectTrans.DOScale(Vector3.one, 0.3f).SetUpdate(true).SetAutoKill(true);
				CommandKitBase.SetDisable(true);
				this.bgRectTrans.DOLocalMove(Vector3.zero, 0.3f, false).SetUpdate(true).SetAutoKill(true).OnComplete(delegate
				{
					CommandKitBase.SetDisable(false);
					this.videoPlayer.Play();
					this.videoMask.SetActive(false);
					this.playBtn.gameObject.SetActive(false);
					this.pauseBtn.gameObject.SetActive(false);
					UIManager.Instance.InsertElementToTop(this.Element);
				});
			}
		}

		// Token: 0x06005A98 RID: 23192 RVA: 0x002A05E4 File Offset: 0x0029E7E4
		private void SwitchToMiniSizeAndPause()
		{
			bool flag = DOTween.IsTweening(this.bgRectTrans, false);
			if (!flag)
			{
				UIManager.Instance.RemoveElement(this.Element);
				GEvent.OnEvent(UiEvents.OnNeedCombatResume, null);
				GEvent.OnEvent(UiEvents.TutorialVideoSwitchToMiniSize, null);
				GEvent.OnEvent(UiEvents.TopUiChanged, null);
				int[] customPosition = this._currTutorialVideoConfig.CustomPosition;
				bool flag2 = customPosition != null && customPosition.Length == 2;
				if (flag2)
				{
					this.miniSizeView.localPosition = new Vector3((float)this._currTutorialVideoConfig.CustomPosition[0], (float)this._currTutorialVideoConfig.CustomPosition[1], this.miniSizeView.localPosition.z);
				}
				this.videoPlayer.Pause();
				this.bgRectTrans.localPosition = Vector3.zero;
				this.bgRectTrans.DOKill(false);
				this.bgRectTrans.DOMove(this.miniSizeView.position, 0.3f, false).SetUpdate(true).SetAutoKill(true);
				this.bgRectTrans.DOScale(Vector3.zero, 0.3f).SetUpdate(true).SetAutoKill(true).OnComplete(delegate
				{
					this.bgRectTrans.gameObject.SetActive(false);
					this.miniSizeView.gameObject.SetActive(true);
					bool exist = UIElement.CombatBegin.Exist;
					if (exist)
					{
						UIElement combatBegin = UIElement.CombatBegin;
						combatBegin.OnHide = (Action)Delegate.Combine(combatBegin.OnHide, new Action(delegate()
						{
							this.miniSizeView.gameObject.SetActive(false);
						}));
					}
					bool exist2 = UIElement.Combat.Exist;
					if (exist2)
					{
						UIElement combat = UIElement.Combat;
						combat.OnHide = (Action)Delegate.Combine(combat.OnHide, new Action(delegate()
						{
							this.miniSizeView.gameObject.SetActive(false);
						}));
					}
					TaiwuEventDomainMethod.Call.TriggerListener("TutorialVedioLostFocus", true);
				});
			}
		}

		// Token: 0x06005A99 RID: 23193 RVA: 0x002A0724 File Offset: 0x0029E924
		private void UpdatePartsTitle()
		{
			this.partTitle.SetText(this._currTutorialVideoConfig.PartsTitle[this._curPartIndex].ColorReplace(), true);
			this.partDesc.SetText(this._currTutorialVideoConfig.PartsDesc[this._curPartIndex].ColorReplace(), true);
			this.videoPartText.SetText((this._curPartIndex + 1).ToString() + "/" + this._currTutorialVideoConfig.PartVideos.Length.ToString(), true);
		}

		// Token: 0x06005A9A RID: 23194 RVA: 0x002A07B8 File Offset: 0x0029E9B8
		private void UpdatePartsBtn()
		{
			this.prevBtn.interactable = (this.firstBtn.interactable = (this._curPartIndex > 0));
			this.nextBtn.interactable = (this.lastBtn.interactable = (this._curPartIndex < this._currTutorialVideoConfig.PartVideos.Length - 1));
		}

		// Token: 0x06005A9B RID: 23195 RVA: 0x002A081C File Offset: 0x0029EA1C
		private void InitBtn()
		{
			this.prevBtn.ClearAndAddListener(delegate
			{
				this._curPartIndex--;
				this.BtnClickAction();
			});
			this.firstBtn.ClearAndAddListener(delegate
			{
				this._curPartIndex = 0;
				this.BtnClickAction();
			});
			this.nextBtn.ClearAndAddListener(delegate
			{
				this._curPartIndex++;
				this.BtnClickAction();
			});
			this.lastBtn.ClearAndAddListener(delegate
			{
				this._curPartIndex = this._currTutorialVideoConfig.PartVideos.Length - 1;
				this.BtnClickAction();
			});
			ToggleGroupHotkeyController.Set(this.Element, this.prevBtn, this.nextBtn, 1, null);
		}

		// Token: 0x06005A9C RID: 23196 RVA: 0x002A08A4 File Offset: 0x0029EAA4
		private void BtnClickAction()
		{
			this.UpdatePartsTitle();
			this.UpdatePartsBtn();
			this.RefreshVideo();
			this.playBtn.gameObject.SetActive(false);
		}

		// Token: 0x06005A9D RID: 23197 RVA: 0x002A08CE File Offset: 0x0029EACE
		private void SetPartsIndex()
		{
			this._curPartIndex = 0;
		}

		// Token: 0x06005A9E RID: 23198 RVA: 0x002A08D8 File Offset: 0x0029EAD8
		private void OnEnable()
		{
			this.bgRectTrans.gameObject.SetActive(true);
			this.miniSizeView.gameObject.SetActive(false);
			this.playBtn.gameObject.SetActive(false);
			this.pauseBtn.gameObject.SetActive(false);
			this.bgRectTrans.localScale = Vector3.one;
			this.bgRectTrans.localPosition = Vector3.zero;
			GEvent.Add(UiEvents.OnNewEventComingToShow, new GEvent.Callback(this.EventShowedAndFurlVideo));
		}

		// Token: 0x06005A9F RID: 23199 RVA: 0x002A096C File Offset: 0x0029EB6C
		private void OnDisable()
		{
			this.leftToggleGroup.OnActiveIndexChange -= this.OnLeftToggleChange;
			this.chapterToggleGroup.OnActiveIndexChange -= this.OnChapterToggleChange;
			GEvent.Remove(UiEvents.OnNewEventComingToShow, new GEvent.Callback(this.EventShowedAndFurlVideo));
		}

		// Token: 0x06005AA0 RID: 23200 RVA: 0x002A09C4 File Offset: 0x0029EBC4
		private void StartPlay()
		{
			bool flag = null == this.videoPlayer.clip;
			if (!flag)
			{
				this.videoPlayer.SetDirectAudioVolume(0, (float)SingletonObject.getInstance<GlobalSettings>().VideoVolume / 100f);
				this.videoPlayer.Play();
				this.playBtn.gameObject.SetActive(false);
				this.pauseBtn.gameObject.SetActive(false);
				this.videoMask.gameObject.SetActive(false);
			}
		}

		// Token: 0x06005AA1 RID: 23201 RVA: 0x002A0A49 File Offset: 0x0029EC49
		private void PauseAtLoopPoint(VideoPlayer source)
		{
			this.videoPlayer.Pause();
			this.pauseBtn.gameObject.SetActive(false);
			this.playBtn.gameObject.SetActive(true);
			this.videoMask.SetActive(true);
		}

		// Token: 0x06005AA2 RID: 23202 RVA: 0x002A0A8C File Offset: 0x0029EC8C
		private void ResourceLoadVideoClip(string videoPathName)
		{
			this.videoPlayer.clip = Resources.Load<VideoClip>(videoPathName);
			bool flag = !this._videoClipCache.ContainsKey(videoPathName);
			if (flag)
			{
				this._videoClipCache.Add(videoPathName, this.videoPlayer.clip);
			}
		}

		// Token: 0x06005AA3 RID: 23203 RVA: 0x002A0AD8 File Offset: 0x0029ECD8
		public void RefreshVideo()
		{
			string videoName = Path.Combine("TutorialVideos/", this._currTutorialVideoConfig.VideoPath, this._currTutorialVideoConfig.PartVideos[this._curPartIndex]).PathFix();
			VideoClip cash;
			bool flag = this._videoClipCache.TryGetValue(videoName, out cash);
			if (flag)
			{
				this.videoPlayer.clip = cash;
			}
			else
			{
				this.ResourceLoadVideoClip(videoName);
			}
			this.StartPlay();
		}

		// Token: 0x06005AA4 RID: 23204 RVA: 0x002A0B47 File Offset: 0x0029ED47
		public void OnPointerEnterVideoArea()
		{
			this.pauseBtn.gameObject.SetActive(this.videoPlayer.isPlaying);
			this.playBtn.gameObject.SetActive(!this.videoPlayer.isPlaying);
		}

		// Token: 0x06005AA5 RID: 23205 RVA: 0x002A0B88 File Offset: 0x0029ED88
		public void OnPointerExitVideoArea()
		{
			bool isPlaying = this.videoPlayer.isPlaying;
			if (isPlaying)
			{
				this.pauseBtn.gameObject.SetActive(false);
			}
			this.playBtn.gameObject.SetActive(!this.videoPlayer.isPlaying);
		}

		// Token: 0x06005AA6 RID: 23206 RVA: 0x002A0BD8 File Offset: 0x0029EDD8
		private void InitChapterToggleGroup()
		{
			this.chapterToggleHolder.Rebuild<CToggle>(Mathf.Min(this._settingData.CompletedChapters + 1, TutorialChapters.Instance.Count), delegate(CToggle toggle, int index)
			{
				TutorialChaptersItem config = TutorialChapters.Instance[index];
				TextMeshProUGUI title = toggle.GetComponentInChildren<TextMeshProUGUI>();
				TooltipInvoker mouseTip = toggle.GetComponentInChildren<TooltipInvoker>();
				title.SetText(config.ToggleName, true);
				bool flag = this.chapterToggleGroup.CanAddToggle(toggle);
				if (flag)
				{
					this.chapterToggleGroup.Add(toggle);
				}
				toggle.interactable = (index <= this._settingData.CompletedChapters);
				mouseTip.enabled = !toggle.interactable;
				title.text.SetColor(toggle.interactable ? "lightyellow" : "grey").ColorReplace();
			});
			this.chapterToggleGroup.SetWithoutNotify(this._curChapterIndex);
		}

		// Token: 0x06005AA7 RID: 23207 RVA: 0x002A0C2C File Offset: 0x0029EE2C
		private void OnChapterToggleChange(int newIndex, int oldIndex)
		{
			this._curChapterIndex = newIndex;
			this.RefreshShowConfigList((short)this._curChapterIndex);
			this.RebuildToggleGroup();
			this.leftToggleGroup.Set(0, true);
			this.RefreshChapterSwitchButton();
		}

		// Token: 0x04003E61 RID: 15969
		[SerializeField]
		private VideoPlayer videoPlayer;

		// Token: 0x04003E62 RID: 15970
		[SerializeField]
		private CRawImage videoRawImage;

		// Token: 0x04003E63 RID: 15971
		[SerializeField]
		private TextMeshProUGUI partDesc;

		// Token: 0x04003E64 RID: 15972
		[SerializeField]
		private TextMeshProUGUI partTitle;

		// Token: 0x04003E65 RID: 15973
		[SerializeField]
		private TextMeshProUGUI videoSummary;

		// Token: 0x04003E66 RID: 15974
		[SerializeField]
		private TextMeshProUGUI videoPartText;

		// Token: 0x04003E67 RID: 15975
		[SerializeField]
		private CToggleGroup leftToggleGroup;

		// Token: 0x04003E68 RID: 15976
		[SerializeField]
		private CToggleGroup chapterToggleGroup;

		// Token: 0x04003E69 RID: 15977
		[SerializeField]
		private TemplatedContainerAssemblyNew leftPartToggleHolder;

		// Token: 0x04003E6A RID: 15978
		[SerializeField]
		private TemplatedContainerAssemblyNew chapterToggleHolder;

		// Token: 0x04003E6B RID: 15979
		[SerializeField]
		private RectTransform bgRectTrans;

		// Token: 0x04003E6C RID: 15980
		[SerializeField]
		private RectTransform miniSizeView;

		// Token: 0x04003E6D RID: 15981
		[SerializeField]
		private GameObject videoMask;

		// Token: 0x04003E6E RID: 15982
		[SerializeField]
		private CButton prevBtn;

		// Token: 0x04003E6F RID: 15983
		[SerializeField]
		private CButton nextBtn;

		// Token: 0x04003E70 RID: 15984
		[SerializeField]
		private CButton firstBtn;

		// Token: 0x04003E71 RID: 15985
		[SerializeField]
		private CButton lastBtn;

		// Token: 0x04003E72 RID: 15986
		[SerializeField]
		private CButton playBtn;

		// Token: 0x04003E73 RID: 15987
		[SerializeField]
		private CButton pauseBtn;

		// Token: 0x04003E74 RID: 15988
		[SerializeField]
		private CButton prevClass;

		// Token: 0x04003E75 RID: 15989
		[SerializeField]
		private CButton nextClass;

		// Token: 0x04003E76 RID: 15990
		[SerializeField]
		private PointerTrigger videoPointerTrigger;

		// Token: 0x04003E77 RID: 15991
		private RenderTexture _renderTexture;

		// Token: 0x04003E78 RID: 15992
		private const string TutorialVideoFolder = "TutorialVideos/";

		// Token: 0x04003E79 RID: 15993
		private bool _isInit;

		// Token: 0x04003E7A RID: 15994
		private readonly Dictionary<string, VideoClip> _videoClipCache = new Dictionary<string, VideoClip>();

		// Token: 0x04003E7B RID: 15995
		private string _videoPathName = string.Empty;

		// Token: 0x04003E7C RID: 15996
		private short _tutorialVideoTemplateId;

		// Token: 0x04003E7D RID: 15997
		private TutorialVideoItem _triggeredTutorialVideoConfig;

		// Token: 0x04003E7E RID: 15998
		private TutorialVideoItem _currTutorialVideoConfig;

		// Token: 0x04003E7F RID: 15999
		private readonly List<TutorialVideoItem> _showConfigList = new List<TutorialVideoItem>();

		// Token: 0x04003E80 RID: 16000
		private int _curPartIndex;

		// Token: 0x04003E81 RID: 16001
		private int _curChapterIndex;

		// Token: 0x04003E82 RID: 16002
		private GlobalSettings _settingData;
	}
}
