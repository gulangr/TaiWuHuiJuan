using System;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Common;
using GameData.Domains.Extra;
using GameData.GameDataBridge;
using GameData.Serializer;
using TMPro;
using UnityEngine;

namespace Game.Views.SectInteract
{
	// Token: 0x020009A8 RID: 2472
	public class ViewJieQingTangram : UIBase
	{
		// Token: 0x17000D5A RID: 3418
		// (get) Token: 0x0600771F RID: 30495 RVA: 0x00376E86 File Offset: 0x00375086
		private string _boardPieceIconPrefix
		{
			get
			{
				return "ui9_back_sectpopup_12_constellation_0_";
			}
		}

		// Token: 0x17000D5B RID: 3419
		// (get) Token: 0x06007720 RID: 30496 RVA: 0x00376E8D File Offset: 0x0037508D
		private string _previewPieceIconPrefix
		{
			get
			{
				return "ui9_back_sectpopup_12_constellation_0_";
			}
		}

		// Token: 0x17000D5C RID: 3420
		// (get) Token: 0x06007721 RID: 30497 RVA: 0x00376E94 File Offset: 0x00375094
		private string _previewCheckPieceIconPrefix
		{
			get
			{
				return "ui9_back_sectpopup_12_constellation_1_";
			}
		}

		// Token: 0x17000D5D RID: 3421
		// (get) Token: 0x06007722 RID: 30498 RVA: 0x00376E9B File Offset: 0x0037509B
		private short _lastPieceTemplateId
		{
			get
			{
				return this._storyData.LastPeaceTemplateId;
			}
		}

		// Token: 0x17000D5E RID: 3422
		// (get) Token: 0x06007723 RID: 30499 RVA: 0x00376EA8 File Offset: 0x003750A8
		// (set) Token: 0x06007724 RID: 30500 RVA: 0x00376EC0 File Offset: 0x003750C0
		public short CurrentTotalChapterIndex
		{
			get
			{
				return this._currentTotalChapterIndex;
			}
			set
			{
				this._currentTotalChapterIndex = value;
				this.SetPreviewChapterBar(this._currentTotalChapterIndex);
			}
		}

		// Token: 0x06007725 RID: 30501 RVA: 0x00376ED7 File Offset: 0x003750D7
		private void SetPreviewChapterBar(short currentTotalChapterIndex)
		{
			this.progressPreview.fillAmount = Mathf.Lerp(0.02f, 1f, (float)currentTotalChapterIndex / (float)(JieqingGameLevel.Instance.Count - 1));
		}

		// Token: 0x06007726 RID: 30502 RVA: 0x00376F05 File Offset: 0x00375105
		private void SetCurrentChapterBar(int chapterIndex)
		{
			this.progressCurrent.fillAmount = Mathf.Lerp(0.02f, 1f, (float)chapterIndex / (float)(JieqingGameLevel.Instance.Count - 1));
		}

		// Token: 0x06007727 RID: 30503 RVA: 0x00376F33 File Offset: 0x00375133
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x06007728 RID: 30504 RVA: 0x00376F40 File Offset: 0x00375140
		public override void QuickHide()
		{
			bool flag = this._currentState == ViewJieQingTangram.EPageState.Normal;
			if (flag)
			{
				this._quiting = true;
				base.QuickHide();
				ExtraDomainMethod.Call.SetJieqingGameData(this._storyData, this._chapterStoryData);
			}
		}

		// Token: 0x06007729 RID: 30505 RVA: 0x00376F84 File Offset: 0x00375184
		public override void OnInit(ArgumentBox argsBox)
		{
			this.Init();
			this._canCheckLeftMouseUp = true;
			this._quiting = false;
			this.guideSingleFirst.gameObject.SetActive(false);
			this.guideSingleSecond.gameObject.SetActive(false);
			this.guideSingleThird.gameObject.SetActive(false);
			this.GuideLoopDisplay(false, true);
			this.effGameBoard.Play();
			this.covers.gameObject.SetActive(false);
			this._currentState = ViewJieQingTangram.EPageState.Normal;
			this.zoneMask.gameObject.SetActive(false);
			this.ClearBoardGridData();
			this.RefreshScore(0);
			this.RefreshRound();
		}

		// Token: 0x0600772A RID: 30506 RVA: 0x00377034 File Offset: 0x00375234
		private void ClearBoardGridData()
		{
			int height = this._boardGridData.GetLength(0);
			int width = this._boardGridData.GetLength(1);
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					this._boardGridData[y, x] = EBoardGridState.None;
				}
			}
		}

		// Token: 0x0600772B RID: 30507 RVA: 0x00377092 File Offset: 0x00375292
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 197, ulong.MaxValue, null));
			this.MonitorFields.Add(new UIBase.MonitorDataField(19, 198, ulong.MaxValue, null));
		}

		// Token: 0x0600772C RID: 30508 RVA: 0x003770CC File Offset: 0x003752CC
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 0)
				{
					DataUid uid = notification.Uid;
					bool flag = uid.DomainId == 19 && uid.DataId == 197;
					if (flag)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._chapterStoryData);
					}
					else
					{
						bool flag2 = uid.DomainId == 19 && uid.DataId == 198;
						if (flag2)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._storyData);
							this.SetupPanelContent();
							this.Element.ShowAfterRefresh();
						}
					}
				}
			}
		}

		// Token: 0x0600772D RID: 30509 RVA: 0x003771D0 File Offset: 0x003753D0
		private void SetupPanelContent()
		{
			this.SetupBoard(this._storyData.BroadChessData);
			this.SetupChapterBasicInfo(this._storyData);
			this.SetupPreviewBoard(this._storyData);
			int score = this.RefreshZone();
			this.CheckSuccessState(score);
			this.SetPreviewChapterBar(this.CurrentTotalChapterIndex);
			this.chapterTitle.text = LanguageKey.UI_MiniGame_JieQing_ChapterTitle.Tr() + LocalStringManager.Get(string.Format("LK_Num_{0}", (int)(this.CurrentTotalChapterIndex + 1)));
		}

		// Token: 0x0600772E RID: 30510 RVA: 0x00377260 File Offset: 0x00375460
		private void CheckSuccessState(int score)
		{
			bool flag = this._currentBoardPieceData.Count == 0;
			if (!flag)
			{
				int currentChapterIndex;
				bool flag2 = this.CheckChapterEndOnApplyPiece(out currentChapterIndex);
				if (flag2)
				{
					sbyte scoreNeed = JieqingGameLevel.Instance[currentChapterIndex].SingPitch;
					bool scoreEnough = score >= (int)scoreNeed;
					bool flag3 = !scoreEnough;
					if (flag3)
					{
						this._storyData.GameResult = 2;
						this.TryForceRetry();
					}
					else
					{
						bool flag4 = currentChapterIndex == (int)this.CurrentTotalChapterIndex;
						if (flag4)
						{
							this._storyData.GameResult = 1;
							SingletonObject.getInstance<YieldHelper>().DelayFrameDo(10U, delegate
							{
								this.QuitWithResult(true, currentChapterIndex);
							});
						}
						else
						{
							this._storyData.GameResult = 3;
							this._chapterStoryData = new SectStoryJieqingGame(this._storyData);
							this._chapterStoryData.GameResult = 1;
						}
					}
				}
			}
		}

		// Token: 0x0600772F RID: 30511 RVA: 0x00377358 File Offset: 0x00375558
		private void OnEnable()
		{
			this.ShowGuildEffSingle();
			for (int i = 0; i < this._resultSuccessEffs.Length; i++)
			{
				this._resultSuccessEffs[i].Stop();
			}
			for (int j = 0; j < this._resultFailEffs.Length; j++)
			{
				this._resultFailEffs[j].Stop();
			}
			for (int k = 0; k < this._pieceShowEffs.Length; k++)
			{
				this._pieceShowEffs[k].Stop();
			}
			for (int l = 0; l < this._pieceActiveEffs.Length; l++)
			{
				this._pieceActiveEffs[l].Stop();
			}
		}

		// Token: 0x06007730 RID: 30512 RVA: 0x00377418 File Offset: 0x00375618
		private void ShowGuildEffSingle()
		{
			bool guildDisplayed = this._guildDisplayed;
			if (!guildDisplayed)
			{
				this._guildDisplayed = true;
				this.guideSingleFirst.gameObject.SetActive(true);
				this.guideSingleSecond.gameObject.SetActive(true);
				this.guideSingleThird.gameObject.SetActive(true);
				this.guideSingleFirst.Stop();
				this.guideSingleSecond.Stop();
				this.guideSingleThird.Stop();
				Sequence seq = DOTween.Sequence();
				seq.AppendInterval(1f);
				seq.AppendCallback(delegate
				{
					this.guideSingleFirst.Play();
				});
				seq.AppendInterval(0.5f);
				seq.AppendCallback(delegate
				{
					this.guideSingleSecond.Play();
				});
				seq.AppendInterval(0.5f);
				seq.AppendCallback(delegate
				{
					this.guideSingleThird.Play();
				});
				seq.AppendInterval(1f);
				seq.AppendCallback(delegate
				{
					this.guideSingleSecond.gameObject.SetActive(false);
				});
				seq.Play<Sequence>();
			}
		}

		// Token: 0x06007731 RID: 30513 RVA: 0x00377520 File Offset: 0x00375720
		private void GuideLoopDisplay(bool isPlay, bool forceChange = false)
		{
			bool flag = !forceChange && this._loopGuideActive == isPlay;
			if (!flag)
			{
				this.guideLoop.gameObject.SetActive(this._loopGuideActive);
				this._loopGuideActive = isPlay;
				if (isPlay)
				{
					this.guideLoop.Play();
				}
				else
				{
					this.guideLoop.Stop();
				}
			}
		}

		// Token: 0x06007732 RID: 30514 RVA: 0x00377580 File Offset: 0x00375780
		private void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._hintPositionOffset = new Dictionary<short, Vector2>();
				this._commonOffset = new Vector2(5f, -4f);
				this._tempSingleBlockOffset = new Vector2(this._blockWidthOffset, this._blockHeightOffset) + this._commonOffset;
				this._tempOneAndHalfBlockOffset = new Vector2(this._blockWidthOffset * 1.5f, this._blockHeightOffset * 1.5f) + this._commonOffset;
				this._tempDoubleBlockOffset = new Vector2(this._blockWidthOffset * 2f, this._blockHeightOffset * 2f) + this._commonOffset;
				this._tempTrippleBlockOffset = new Vector2(this._blockWidthOffset * 3f, this._blockHeightOffset * 3f) + this._commonOffset;
				this._hintPositionOffset[0] = this._tempSingleBlockOffset;
				this._hintPositionOffset[1] = this._tempOneAndHalfBlockOffset;
				this._hintPositionOffset[2] = this._tempOneAndHalfBlockOffset;
				this._hintPositionOffset[3] = this._tempDoubleBlockOffset;
				this._hintPositionOffset[4] = this._tempDoubleBlockOffset;
				this._hintPositionOffset[5] = this._tempSingleBlockOffset;
				this._hintPositionOffset[6] = this._tempSingleBlockOffset;
				this._hintPositionOffset[7] = this._tempSingleBlockOffset;
				this._hotkeyHintParent = this.hotkeyHints.parent.GetComponent<RectTransform>();
				this._resultSuccessEffs = new UIParticle[this.successEffsRect.childCount];
				for (int i = 0; i < this.successEffsRect.childCount; i++)
				{
					this._resultSuccessEffs[i] = this.successEffsRect.GetChild(i).gameObject.GetComponent<UIParticle>();
				}
				this._resultFailEffs = new UIParticle[this.failEffsRect.childCount];
				for (int j = 0; j < this.failEffsRect.childCount; j++)
				{
					this._resultFailEffs[j] = this.failEffsRect.GetChild(j).gameObject.GetComponent<UIParticle>();
				}
				this._pieceShowEffs = new UIParticle[this.effShowsRect.childCount];
				for (int k = 0; k < this.effShowsRect.childCount; k++)
				{
					this._pieceShowEffs[k] = this.effShowsRect.GetChild(k).gameObject.GetComponent<UIParticle>();
				}
				this._pieceActiveEffs = new UIParticle[this.effActiveRect.childCount];
				for (int l = 0; l < this.effActiveRect.childCount; l++)
				{
					this._pieceActiveEffs[l] = this.effActiveRect.GetChild(l).gameObject.GetComponent<UIParticle>();
				}
				this._currentBoardPieceData = new List<ViewJieQingTangram.PieceData>();
				this._boardGridData = new EBoardGridState[7, 7];
				this._candidatesNormal = (from t in JieqingGamePeace.Instance
				where t.TemplateId != 7
				select t).ToList<JieqingGamePeaceItem>();
				this.buttonChange.ClearAndAddListener(new Action(this.OnClickChange));
				this.buttonRetry.ClearAndAddListener(new Action(this.OnClickRetry));
				this.buttonClose.ClearAndAddListener(delegate
				{
					bool flag = this._currentState == ViewJieQingTangram.EPageState.DragPiece;
					if (flag)
					{
						this.QuitPreviewDrag();
					}
					this.QuickHide();
				});
				TangramDraggable tangramDraggable = this.previewPiece;
				tangramDraggable.OnClickgAction = (Action)Delegate.Combine(tangramDraggable.OnClickgAction, new Action(this.OnPreviewClick));
				this._inited = true;
			}
		}

		// Token: 0x06007733 RID: 30515 RVA: 0x0037792C File Offset: 0x00375B2C
		private void SetupPreviewBoard(SectStoryJieqingGame storyData)
		{
			bool flag = storyData.CurrPeaceTemplateId >= 0;
			if (flag)
			{
				this.SetupPreviewPiece(storyData.CurrPeaceTemplateId, (int)storyData.CurrPeaceRotationState, storyData.CurrPeaceIsFlipped);
			}
			else
			{
				this.SetRandomPiecePreview(true);
			}
			bool flag2 = storyData.NextPieceTemplateId >= 0;
			if (flag2)
			{
				this.SetupNextPreviewPiece(storyData.NextPieceTemplateId, (int)storyData.CurrPeaceRotationState, storyData.CurrPeaceIsFlipped);
			}
			else
			{
				this.SetRandomPieceNextPreview();
			}
		}

		// Token: 0x06007734 RID: 30516 RVA: 0x003779A8 File Offset: 0x00375BA8
		private void SetupBoard(List<JieqingGameChessData> broadChessData)
		{
			this.ClearBoardPieceData();
			bool flag = broadChessData == null;
			if (!flag)
			{
				foreach (JieqingGameChessData item in broadChessData)
				{
					ViewJieQingTangram.PieceData boardPieceData = new ViewJieQingTangram.PieceData(JieqingGamePeace.Instance[item.ChessTemplateId], (short)item.ChessRotationState, item.ChessIsFlipped);
					this.ApplyPiece(boardPieceData, (int)item.CoordinateX, (int)item.CoordinateY, false);
				}
			}
		}

		// Token: 0x06007735 RID: 30517 RVA: 0x00377A40 File Offset: 0x00375C40
		private void ClearBoardPieceData()
		{
			foreach (ViewJieQingTangram.PieceData item in this._currentBoardPieceData)
			{
				Object.Destroy(item.BoardPiece.gameObject);
				Object.Destroy(item.SelectedStar.gameObject);
			}
			this._currentBoardPieceData.Clear();
			this.ClearBoardGridData();
		}

		// Token: 0x06007736 RID: 30518 RVA: 0x00377AC8 File Offset: 0x00375CC8
		private void SetupChapterBasicInfo(SectStoryJieqingGame storyData)
		{
			this.RefreshScore(storyData.CurrentScore);
			this.RefreshRound();
			this.RefreshButtonChange(storyData);
			this.RefreshButtonRetry(storyData);
		}

		// Token: 0x06007737 RID: 30519 RVA: 0x00377AEF File Offset: 0x00375CEF
		private void RefreshButtonRetry(SectStoryJieqingGame storyData)
		{
			this.buttonRetry.interactable = (storyData.ReopenLeftCount > 0);
		}

		// Token: 0x06007738 RID: 30520 RVA: 0x00377B08 File Offset: 0x00375D08
		private void RefreshButtonChange(SectStoryJieqingGame storyData)
		{
			this.changeAmountTxt.SetText(string.Format("{0}/{1}", storyData.RerollLeftCount, storyData.RerollMaxCount), true);
			bool isFixedPiece = this._currentBoardPieceData.Count > 0 && this._currentBoardPieceData.Count % 4 == 3;
			this.buttonChange.interactable = (storyData.RerollLeftCount > 0 && !isFixedPiece);
			this.mouseTipButtonChange.Type = TipType.Simple;
			this.mouseTipButtonChange.IsLanguageKey = false;
			this.mouseTipButtonChange.PresetParam = new string[]
			{
				LanguageKey.UI_MiniGame_JieQing_Change.Tr(),
				LocalStringManager.GetFormat(LanguageKey.UI_MiniGame_JieQing_RandomPiece_TipsContent, storyData.RerollLeftCount, storyData.RerollMaxCount)
			};
		}

		// Token: 0x06007739 RID: 30521 RVA: 0x00377BDC File Offset: 0x00375DDC
		private void OnPreviewPieceDrag()
		{
			int x;
			int y;
			bool flag = this.CheckMouseGridPosition(out x, out y);
			if (flag)
			{
				Vector3 cellCorner = this.GetCellPivot(x, y);
				this.ShowCheckPreview(cellCorner);
				bool valid = this.CheckCellValid(x, y, this._currentPreviewData);
				this.SetCheckPreviewState(valid);
			}
			else
			{
				this.HideCheckPreview();
			}
		}

		// Token: 0x0600773A RID: 30522 RVA: 0x00377C2F File Offset: 0x00375E2F
		private void OnPreviewPieceBeginDrag()
		{
			this.SetCheckPreviewIconState(this._currentPreviewData);
			this.SetHotkeyHintsState(true);
			this.SetPreviewPieceBlockRaycast(false);
		}

		// Token: 0x0600773B RID: 30523 RVA: 0x00377C4F File Offset: 0x00375E4F
		private void SetPreviewPieceBlockRaycast(bool blockRaycast)
		{
			this.previewPieceMainIcon.raycastTarget = blockRaycast;
			this.previewPieceEmptyGraphic.enabled = blockRaycast;
		}

		// Token: 0x0600773C RID: 30524 RVA: 0x00377C6C File Offset: 0x00375E6C
		private void SetHotkeyHintsState(bool v)
		{
			this.hotkeyHints.gameObject.SetActive(v);
		}

		// Token: 0x0600773D RID: 30525 RVA: 0x00377C84 File Offset: 0x00375E84
		private void OnPreviewEndDrag()
		{
			this.SetHotkeyHintsState(false);
			this.SetPreviewPieceBlockRaycast(true);
			int x;
			int y;
			bool flag = this.CheckMouseGridPosition(out x, out y);
			if (flag)
			{
				Vector3 cellCorner = this.GetCellPivot(x, y);
				bool valid = this.CheckCellValid(x, y, this._currentPreviewData);
				bool flag2 = !valid;
				if (flag2)
				{
					this.HideCheckPreview();
					return;
				}
				this.ApplyPiece(this._currentPreviewData, x, y, true);
				int score = this.RefreshZone();
				this.RefreshRound();
				this.CheckSuccessState(score);
				this.SetRandomPiecePreview(true);
				this.SetRandomPieceNextPreview();
				this.RefreshButtonChange(this._storyData);
				this.QuitPreviewDrag();
			}
			this.HideCheckPreview();
		}

		// Token: 0x0600773E RID: 30526 RVA: 0x00377D34 File Offset: 0x00375F34
		private bool CheckMouseInPreviewRect()
		{
			return RectTransformUtility.RectangleContainsScreenPoint(this.previewBoard, Input.mousePosition, UIManager.Instance.UiCamera);
		}

		// Token: 0x0600773F RID: 30527 RVA: 0x00377D68 File Offset: 0x00375F68
		private void OnPreviewClick()
		{
			bool flag = this._currentState == ViewJieQingTangram.EPageState.Normal;
			if (flag)
			{
				this.OnPreviewPieceBeginDrag();
				this._currentState = ViewJieQingTangram.EPageState.DragPiece;
				this._canCheckLeftMouseUp = false;
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
				{
					this._canCheckLeftMouseUp = true;
				});
			}
		}

		// Token: 0x06007740 RID: 30528 RVA: 0x00377DB2 File Offset: 0x00375FB2
		private void QuitPreviewDrag()
		{
			this.SetHotkeyHintsState(false);
			this.SetPreviewPieceBlockRaycast(true);
			this.previewPiece.ResetPosition();
			this.HideCheckPreview();
			this._currentState = ViewJieQingTangram.EPageState.Normal;
		}

		// Token: 0x06007741 RID: 30529 RVA: 0x00377DE0 File Offset: 0x00375FE0
		private void Update()
		{
			bool flag = this._currentState == ViewJieQingTangram.EPageState.DragPiece;
			if (flag)
			{
				this.previewPiece.SnapTo(Input.mousePosition);
				this.OnPreviewPieceDrag();
				Vector2 localPos;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this._hotkeyHintParent, Input.mousePosition, UIManager.Instance.UiCamera, out localPos);
				Vector2 hintOffet = this.GetHintOffset(this._currentPreviewData.TemplateId, this._currentPreviewData.IsFliped, this._currentPreviewData.RotationType);
				localPos += hintOffet;
				this.hotkeyHints.anchoredPosition = localPos;
				bool flag2 = CommonCommandKit.LeftMouse.Check(this.Element, false, false, false, true, false) && this._canCheckLeftMouseUp;
				if (flag2)
				{
					bool flag3 = this.CheckMouseInPreviewRect();
					if (flag3)
					{
						this.QuitPreviewDrag();
					}
					else
					{
						this.OnPreviewEndDrag();
					}
				}
				bool flag4 = CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false) && this._canCheckLeftMouseUp;
				if (flag4)
				{
					this.OnClickFlip();
				}
				bool flag5 = Input.mouseScrollDelta.sqrMagnitude > 0.01f;
				if (flag5)
				{
					this.OnClickRotate();
				}
				bool flag6 = CommonCommandKit.Esc.Check(this.Element, false, true, false, true, false);
				if (flag6)
				{
					this.QuitPreviewDrag();
				}
			}
			bool flag7 = this.CheckInput();
			if (flag7)
			{
				this.GuideLoopDisplay(false, false);
				this._loopGuideCounter = 0f;
			}
			else
			{
				this._loopGuideCounter += Time.deltaTime;
			}
			bool flag8 = this._loopGuideCounter >= 3f;
			if (flag8)
			{
				this.GuideLoopDisplay(true, false);
			}
		}

		// Token: 0x06007742 RID: 30530 RVA: 0x00377F90 File Offset: 0x00376190
		private Vector2 GetHintOffset(short templateId, bool isFliped, int rotationType)
		{
			sbyte artResourceIndex = JieqingGamePeace.Instance[templateId].ArtResourceIndex;
			sbyte b = artResourceIndex;
			sbyte b2 = b;
			Vector2 result;
			if (b2 != 0)
			{
				if (b2 != 1)
				{
					result = this._hintPositionOffset[templateId];
				}
				else
				{
					bool flag = !isFliped;
					if (flag)
					{
						bool flag2 = rotationType == 0;
						if (flag2)
						{
							return this._tempOneAndHalfBlockOffset;
						}
					}
					result = this._tempTrippleBlockOffset;
				}
			}
			else
			{
				result = this._tempSingleBlockOffset;
			}
			return result;
		}

		// Token: 0x06007743 RID: 30531 RVA: 0x00378000 File Offset: 0x00376200
		private bool CheckInput()
		{
			bool flag = HotKeyCommand.CheckAnyKeyDown();
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = Mathf.Abs(Input.GetAxis("Mouse X")) > 0.01f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.01f;
				result = flag2;
			}
			return result;
		}

		// Token: 0x06007744 RID: 30532 RVA: 0x00378058 File Offset: 0x00376258
		private bool CheckCellValid(int oriX, int oriY, ViewJieQingTangram.PieceData currentPreviewData)
		{
			EBoardGridState[,] targetShape = currentPreviewData.CurrentShape;
			int height = targetShape.GetLength(0);
			int width = targetShape.GetLength(1);
			for (int y = 0; y < height; y++)
			{
				int checkY = oriY + y;
				for (int x = 0; x < width; x++)
				{
					int checkX = oriX + x;
					bool validPosition = this.ValidBoardPosition(checkX, checkY);
					bool flag = !validPosition && targetShape[y, x] > EBoardGridState.None;
					if (flag)
					{
						return false;
					}
					bool flag2 = !validPosition;
					if (!flag2)
					{
						bool flag3 = (targetShape[y, x] & this._boardGridData[checkY, checkX]) > EBoardGridState.None;
						if (flag3)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x06007745 RID: 30533 RVA: 0x00378120 File Offset: 0x00376320
		private bool ValidBoardPosition(int checkX, int checkY)
		{
			return checkX >= 0 && checkX < 7 && checkY >= 0 && checkY < 7;
		}

		// Token: 0x06007746 RID: 30534 RVA: 0x00378148 File Offset: 0x00376348
		private Vector3 GetCellPivot(int gridX, int gridY)
		{
			float cellWidth = this.boardRect.rect.width / 7f;
			float cellHeight = this.boardRect.rect.height / 7f;
			Vector3 boardLocalTopLeft = new Vector3(-this.boardRect.rect.width / 2f, this.boardRect.rect.height / 2f, 0f);
			Vector3 cellLocalTopLeft = boardLocalTopLeft + new Vector3((float)gridX * cellWidth, (float)(-(float)gridY) * cellHeight, 0f);
			return this.boardRect.TransformPoint(cellLocalTopLeft);
		}

		// Token: 0x06007747 RID: 30535 RVA: 0x003781F9 File Offset: 0x003763F9
		private void HideCheckPreview()
		{
			this.checkPreview.gameObject.SetActive(false);
		}

		// Token: 0x06007748 RID: 30536 RVA: 0x00378210 File Offset: 0x00376410
		private void ShowCheckPreview(Vector3 cellCorner)
		{
			this.checkPreview.gameObject.SetActive(true);
			this.checkPreview.transform.position = new Vector3(cellCorner.x, cellCorner.y, this.checkPreview.transform.position.z);
		}

		// Token: 0x06007749 RID: 30537 RVA: 0x00378267 File Offset: 0x00376467
		private void SetCheckPreviewState(bool valid)
		{
			this.checkPreview.CGet<CImage>("Valid").gameObject.SetActive(valid);
			this.checkPreview.CGet<CImage>("Invalid").gameObject.SetActive(!valid);
		}

		// Token: 0x0600774A RID: 30538 RVA: 0x003782A8 File Offset: 0x003764A8
		private void SetCheckPreviewIconState(ViewJieQingTangram.PieceData currentPreviewData)
		{
			CImage validIcon = this.checkPreview.CGet<CImage>("Valid");
			CImage invalidIcon = this.checkPreview.CGet<CImage>("Invalid");
			JieqingGamePeaceItem config = JieqingGamePeace.Instance[currentPreviewData.TemplateId];
			string spriteName = string.Format("{0}{1}", this._previewCheckPieceIconPrefix, config.ArtResourceIndex);
			validIcon.SetSprite(spriteName, false, null);
			invalidIcon.SetSprite(spriteName, false, null);
			validIcon.rectTransform.sizeDelta = new Vector2(validIcon.sprite.rect.width, validIcon.sprite.rect.height);
			invalidIcon.rectTransform.sizeDelta = validIcon.rectTransform.sizeDelta;
			this.SetIconState(validIcon.rectTransform, this.previewPieceMainIcon);
			this.SetIconState(invalidIcon.rectTransform, this.previewPieceMainIcon);
		}

		// Token: 0x0600774B RID: 30539 RVA: 0x00378390 File Offset: 0x00376590
		private void SetIconState(RectTransform icon, CImage targetIcon)
		{
			icon.localPosition = targetIcon.transform.localPosition;
			icon.sizeDelta = targetIcon.rectTransform.sizeDelta;
			icon.rotation = targetIcon.rectTransform.rotation;
			icon.localScale = targetIcon.rectTransform.localScale;
		}

		// Token: 0x0600774C RID: 30540 RVA: 0x003783E8 File Offset: 0x003765E8
		private bool TryForceRetry()
		{
			bool quiting = this._quiting;
			bool result;
			if (quiting)
			{
				result = false;
			}
			else
			{
				bool flag = this._storyData.ReopenLeftCount > 0;
				if (flag)
				{
					this.RetryDialog(true);
					result = true;
				}
				else
				{
					int currentChapterIndex;
					this.CheckChapterEndOnApplyPiece(out currentChapterIndex);
					this.QuitWithResult(false, currentChapterIndex);
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600774D RID: 30541 RVA: 0x0037843C File Offset: 0x0037663C
		private int RefreshZone()
		{
			MaxRectangleFinder.RectangleInfo rectInfo = MaxRectangleFinder.CalculateMaxRectangleArea(this._boardGridData);
			this.ApplyZoneRect(rectInfo);
			int score = rectInfo.Width * rectInfo.Height;
			this._storyData.CurrentScore = score;
			this.RefreshScore(score);
			return score;
		}

		// Token: 0x0600774E RID: 30542 RVA: 0x00378488 File Offset: 0x00376688
		private bool CheckChapterEndOnApplyPiece(out int currentChapterIndex)
		{
			currentChapterIndex = (this._currentBoardPieceData.Count - 1) / 4;
			return this._currentBoardPieceData.Count % 4 == 0;
		}

		// Token: 0x0600774F RID: 30543 RVA: 0x003784BC File Offset: 0x003766BC
		private void RefreshScore(int score)
		{
			int currentChapter = this._currentBoardPieceData.Count / 4;
			sbyte right = JieqingGameLevel.Instance[currentChapter].SingPitch;
			bool scoreEffFlag = this._preScroe != score;
			this.scoreNumberComp.Refresh(string.Format("{0}/{1}", score.ToString(), right));
			bool flag = scoreEffFlag;
			if (flag)
			{
				this.effGainScore.Play();
			}
			this._preScroe = score;
		}

		// Token: 0x06007750 RID: 30544 RVA: 0x00378534 File Offset: 0x00376734
		private void RefreshRound()
		{
			int currentRound = this._currentBoardPieceData.Count + 1;
			int currentChapter = this._currentBoardPieceData.Count / 4;
			this.roundNumberComp.Refresh(string.Format("{0}/{1}", currentRound.ToString(), (int)((this.CurrentTotalChapterIndex + 1) * 4)));
			this.SetCurrentChapterBar(currentChapter);
		}

		// Token: 0x06007751 RID: 30545 RVA: 0x00378594 File Offset: 0x00376794
		private void ApplyZoneRect(MaxRectangleFinder.RectangleInfo rectInfo)
		{
			this.zoneMask.gameObject.SetActive(true);
			List<Transform> tempChildList = new List<Transform>();
			for (int i = 1; i < this.zoneMask.childCount; i++)
			{
				tempChildList.Add(this.zoneMask.GetChild(i));
			}
			foreach (Transform item in tempChildList)
			{
				item.parent = this.zoneRect;
			}
			float cellWidth = this.boardRect.rect.width / 7f;
			float cellHeight = this.boardRect.rect.height / 7f;
			float width = cellWidth * (float)rectInfo.Width;
			float height = cellHeight * (float)rectInfo.Height;
			bool flag = width == 0f || height == 0f;
			if (flag)
			{
				this.zoneMask.gameObject.SetActive(false);
			}
			else
			{
				Vector3 pivotPosition = this.GetCellPivot(rectInfo.Left, rectInfo.Top);
				this.zoneMask.SetSize(new Vector2(width, height));
				this.zoneMask.position = pivotPosition;
				tempChildList.Clear();
				for (int j = 0; j < this.zoneRect.childCount; j++)
				{
					bool flag2 = this.zoneRect.GetChild(j) == this.zoneMask;
					if (!flag2)
					{
						tempChildList.Add(this.zoneRect.GetChild(j));
					}
				}
				foreach (Transform item2 in tempChildList)
				{
					item2.parent = this.zoneMask;
				}
			}
		}

		// Token: 0x06007752 RID: 30546 RVA: 0x00378798 File Offset: 0x00376998
		private void ApplyPiece(ViewJieQingTangram.PieceData currentPreviewData, int oriX, int oriY, bool pushIntoStoryData = true)
		{
			this.ApplyPieceData(currentPreviewData, oriX, oriY, pushIntoStoryData);
			this.GeneratePieceOnBoard(oriX, oriY, currentPreviewData);
		}

		// Token: 0x06007753 RID: 30547 RVA: 0x003787B4 File Offset: 0x003769B4
		private void ApplyPieceData(ViewJieQingTangram.PieceData currentPreviewData, int oriX, int oriY, bool pushIntoStoryData)
		{
			this._currentBoardPieceData.Add(currentPreviewData);
			if (pushIntoStoryData)
			{
				bool flag = this._storyData.BroadChessData == null;
				if (flag)
				{
					this._storyData.BroadChessData = new List<JieqingGameChessData>();
				}
				this._storyData.BroadChessData.Add(this.PieceData2GameChessData(currentPreviewData, oriX, oriY));
			}
			EBoardGridState[,] targetShape = currentPreviewData.CurrentShape;
			int height = targetShape.GetLength(0);
			int width = targetShape.GetLength(1);
			for (int y = 0; y < height; y++)
			{
				int boardY = oriY + y;
				for (int x = 0; x < width; x++)
				{
					int boardX = oriX + x;
					bool flag2 = this.ValidBoardPosition(boardX, boardY);
					if (flag2)
					{
						this._boardGridData[boardY, boardX] |= targetShape[y, x];
					}
				}
			}
		}

		// Token: 0x06007754 RID: 30548 RVA: 0x0037889C File Offset: 0x00376A9C
		private JieqingGameChessData PieceData2GameChessData(ViewJieQingTangram.PieceData currentPreviewData, int oriX, int oriY)
		{
			return new JieqingGameChessData
			{
				ChessRotationState = (sbyte)currentPreviewData.RotationType,
				ChessIsFlipped = currentPreviewData.IsFliped,
				ChessTemplateId = currentPreviewData.TemplateId,
				CoordinateX = (byte)oriX,
				CoordinateY = (byte)oriY
			};
		}

		// Token: 0x06007755 RID: 30549 RVA: 0x003788F0 File Offset: 0x00376AF0
		private void GeneratePieceOnBoard(int oriX, int oriY, ViewJieQingTangram.PieceData currentPreviewData)
		{
			Refers piece = Object.Instantiate<Refers>(this.boardPiecePrefab, this.boardRect);
			Vector3 cellCorner = this.GetCellPivot(oriX, oriY);
			piece.transform.position = cellCorner;
			piece.gameObject.SetActive(true);
			JieqingGamePeaceItem config = JieqingGamePeace.Instance[currentPreviewData.TemplateId];
			CIrregularImage mainIcon = piece.CGet<CIrregularImage>("MainIcon");
			mainIcon.threshold = 0.1f;
			piece.CGet<CImage>("Highlight").SetSprite(string.Format("popup_tangram_constellation_2_{0}", config.ArtResourceIndex), false, null);
			string spriteName = string.Format("{0}{1}", this._boardPieceIconPrefix, config.ArtResourceIndex);
			UIParticle showEff = this.AddPieceShowEff(piece, config.ArtResourceIndex);
			showEff.Play();
			this.SetConstellation(mainIcon, spriteName, currentPreviewData.RotationType, currentPreviewData.IsFliped);
			RectTransform activeEffHolder = this.AddPieceActiveEff(piece, config.ArtResourceIndex);
			activeEffHolder.parent = this.zoneRect;
			currentPreviewData.BoardPiece = piece;
			currentPreviewData.SelectedStar = activeEffHolder;
		}

		// Token: 0x06007756 RID: 30550 RVA: 0x003789FC File Offset: 0x00376BFC
		private UIParticle AddPieceShowEff(Refers piece, sbyte artResourceIndex)
		{
			RectTransform showEffHolder = piece.CGet<RectTransform>("PieceShowHolder");
			UIParticle showEff = Object.Instantiate<UIParticle>(this._pieceShowEffs[(int)artResourceIndex]);
			showEff.transform.SetParent(showEffHolder, false);
			showEff.transform.localPosition = Vector3.zero;
			return showEff;
		}

		// Token: 0x06007757 RID: 30551 RVA: 0x00378A48 File Offset: 0x00376C48
		private RectTransform AddPieceActiveEff(Refers piece, sbyte artResourceIndex)
		{
			RectTransform selectedStarHolder = piece.CGet<RectTransform>("ActiveStarHolder");
			UIParticle activeEff = Object.Instantiate<UIParticle>(this._pieceActiveEffs[(int)artResourceIndex]);
			activeEff.transform.SetParent(selectedStarHolder, false);
			activeEff.transform.localPosition = Vector3.zero;
			activeEff.Play();
			return selectedStarHolder;
		}

		// Token: 0x06007758 RID: 30552 RVA: 0x00378A9C File Offset: 0x00376C9C
		private bool CheckMouseGridPosition(out int gridX, out int gridY)
		{
			gridX = -1;
			gridY = -1;
			Vector2 localPoint;
			bool flag = RectTransformUtility.ScreenPointToLocalPointInRectangle(this.boardRect, Input.mousePosition, UIManager.Instance.UiCamera, out localPoint);
			if (flag)
			{
				float cellWidth = this.boardRect.rect.width / 7f;
				float cellHeight = this.boardRect.rect.height / 7f;
				float originX = -this.boardRect.rect.width / 2f;
				float originY = this.boardRect.rect.height / 2f;
				float relativeX = localPoint.x - originX;
				float relativeY = originY - localPoint.y;
				gridX = Mathf.FloorToInt(relativeX / cellWidth);
				gridY = Mathf.FloorToInt(relativeY / cellHeight);
				bool flag2 = gridX >= 0 && gridX < 7 && gridY >= 0 && gridY < 7;
				if (flag2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007759 RID: 30553 RVA: 0x00378B9F File Offset: 0x00376D9F
		private void OnClickRetry()
		{
			this.RetryDialog(false);
		}

		// Token: 0x0600775A RID: 30554 RVA: 0x00378BAC File Offset: 0x00376DAC
		private void RetryDialog(bool quitIfNotRetry)
		{
			int currentChapterIndex;
			this.CheckChapterEndOnApplyPiece(out currentChapterIndex);
			TangramDialogCmd cmd = new TangramDialogCmd
			{
				Yes = delegate(int selection)
				{
					bool flag = selection == 0;
					if (flag)
					{
						this.RetryChapter();
					}
					else
					{
						this.RetryGame();
					}
				},
				No = delegate()
				{
					bool quitIfNotRetry2 = quitIfNotRetry;
					if (quitIfNotRetry2)
					{
						this.QuitWithResult(false, currentChapterIndex);
					}
				}
			};
			UIElement.TangramDialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.TangramDialog);
		}

		// Token: 0x0600775B RID: 30555 RVA: 0x00378C30 File Offset: 0x00376E30
		private void QuitWithResult(bool isSuccess, int levelIndex)
		{
			Sequence seq = DOTween.Sequence();
			this.covers.gameObject.SetActive(true);
			this.resultIcon.SetSprite(isSuccess ? "ui9_back_tangram_success_0" : "ui9_back_tangram_fail_0", false, null);
			UIParticle uiParticle = isSuccess ? this._resultSuccessEffs[levelIndex] : this._resultFailEffs[levelIndex];
			seq.AppendInterval(0.3f);
			seq.AppendCallback(delegate
			{
				uiParticle.Play();
			});
			seq.AppendInterval(2f);
			seq.AppendCallback(delegate
			{
				uiParticle.Stop();
			});
			seq.AppendCallback(delegate
			{
				this.QuickHide();
			});
			seq.Play<Sequence>();
		}

		// Token: 0x0600775C RID: 30556 RVA: 0x00378CF4 File Offset: 0x00376EF4
		private void RetryGame()
		{
			SectStoryJieqingGame storyData = this._storyData;
			int reopenLeftCount = storyData.ReopenLeftCount;
			storyData.ReopenLeftCount = reopenLeftCount - 1;
			this._storyData.BroadChessData = new List<JieqingGameChessData>();
			this.SetupPanelContent();
			this.SetRandomPiecePreview(true);
			this.SetRandomPieceNextPreview();
		}

		// Token: 0x0600775D RID: 30557 RVA: 0x00378D40 File Offset: 0x00376F40
		private void RetryChapter()
		{
			SectStoryJieqingGame storyData = this._storyData;
			int reopenLeftCount = storyData.ReopenLeftCount;
			storyData.ReopenLeftCount = reopenLeftCount - 1;
			bool flag = this._chapterStoryData.BroadChessData != null;
			if (flag)
			{
				this._storyData.BroadChessData = new List<JieqingGameChessData>(this._chapterStoryData.BroadChessData);
			}
			else
			{
				this._storyData.BroadChessData = new List<JieqingGameChessData>();
			}
			this.SetupPanelContent();
			this.SetRandomPiecePreview(true);
			this.SetRandomPieceNextPreview();
		}

		// Token: 0x0600775E RID: 30558 RVA: 0x00378DBC File Offset: 0x00376FBC
		private void OnClickChange()
		{
			bool flag = this._storyData.RerollLeftCount > 0;
			if (flag)
			{
				SectStoryJieqingGame storyData = this._storyData;
				int rerollLeftCount = storyData.RerollLeftCount;
				storyData.RerollLeftCount = rerollLeftCount - 1;
			}
			this.RefreshButtonChange(this._storyData);
			this.SetRandomPiecePreview(false);
			bool flag2 = this._currentState == ViewJieQingTangram.EPageState.DragPiece;
			if (flag2)
			{
				this.SetCheckPreviewIconState(this._currentPreviewData);
			}
		}

		// Token: 0x0600775F RID: 30559 RVA: 0x00378E28 File Offset: 0x00377028
		private void SetRandomPiecePreview(bool canUseNextPreview = true)
		{
			Debug.Log(string.Format("test SetRandomPiecePreview 000 :{0}", this._storyData.NextPieceTemplateId));
			bool flag = this._currentBoardPieceData.Count > 0 && this._currentBoardPieceData.Count % 4 == 3;
			short pieceId;
			if (flag)
			{
				pieceId = 7;
			}
			else
			{
				bool flag2 = canUseNextPreview && this._storyData.NextPieceTemplateId >= 0;
				if (flag2)
				{
					pieceId = this._storyData.NextPieceTemplateId;
					Debug.Log(string.Format("test SetRandomPiecePreview 111 :{0}", this._storyData.NextPieceTemplateId));
				}
				else
				{
					pieceId = this.GetRandomPieceIndex(this._lastPieceTemplateId);
				}
			}
			this.SetPiecePreview(pieceId);
		}

		// Token: 0x06007760 RID: 30560 RVA: 0x00378EE4 File Offset: 0x003770E4
		private int SetRandomPieceNextPreview()
		{
			bool flag = this._currentBoardPieceData.Count > 0 && this._currentBoardPieceData.Count % 4 == 2;
			short pieceId;
			if (flag)
			{
				pieceId = 7;
			}
			else
			{
				pieceId = this.GetRandomPieceIndex(this._lastPieceTemplateId);
			}
			this.SetNextPiecePreview(pieceId);
			return (int)pieceId;
		}

		// Token: 0x06007761 RID: 30561 RVA: 0x00378F3C File Offset: 0x0037713C
		private void SetPiecePreview(short pieceId)
		{
			this._storyData.CurrPeaceTemplateId = pieceId;
			this._storyData.CurrPeaceRotationState = 0;
			this._storyData.CurrPeaceIsFlipped = false;
			this.SetupPreviewPiece(pieceId, 0, false);
			this._storyData.LastPeaceTemplateId = pieceId;
		}

		// Token: 0x06007762 RID: 30562 RVA: 0x00378F88 File Offset: 0x00377188
		private void SetNextPiecePreview(short pieceId)
		{
			this.SetupNextPreviewPiece(pieceId, 0, false);
		}

		// Token: 0x06007763 RID: 30563 RVA: 0x00378F98 File Offset: 0x00377198
		private void SetupPreviewPiece(short pieceId, int rotateState, bool fliped)
		{
			JieqingGamePeaceItem pieceConfig = JieqingGamePeace.Instance[pieceId];
			CImage mainIcon = this.previewPieceMainIcon;
			this.previewTxt.text = pieceConfig.Name;
			this.SetConstellation(mainIcon, string.Format("{0}{1}", this._previewPieceIconPrefix, pieceConfig.ArtResourceIndex), rotateState, fliped);
			this._currentPreviewData = new ViewJieQingTangram.PieceData(pieceConfig);
		}

		// Token: 0x06007764 RID: 30564 RVA: 0x00378FFC File Offset: 0x003771FC
		private void SetupNextPreviewPiece(short pieceId, int rotateState, bool fliped)
		{
			JieqingGamePeaceItem pieceConfig = JieqingGamePeace.Instance[pieceId];
			CImage mainIcon = this.previewPiece2MainIcon;
			this._storyData.NextPieceTemplateId = pieceId;
			Debug.Log(string.Format("test SetupNextPreviewPiece:{0}", this._storyData.NextPieceTemplateId));
			this.SetConstellation(mainIcon, string.Format("{0}{1}", this._previewPieceIconPrefix, pieceConfig.ArtResourceIndex), 0, false);
		}

		// Token: 0x06007765 RID: 30565 RVA: 0x0037906F File Offset: 0x0037726F
		private void SetConstellation(CImage mainIcon, string spriteName, int rotateState, bool fliped)
		{
			mainIcon.SetSprite(spriteName, false, null);
			mainIcon.SetNativeSize();
			this.SetRotationAndFlipVisual(mainIcon, rotateState, fliped);
		}

		// Token: 0x06007766 RID: 30566 RVA: 0x00379090 File Offset: 0x00377290
		private void CloseInParentCorner(CImage mainIcon, int rotationState, bool fliped)
		{
			RectTransform parentRect = mainIcon.rectTransform.parent as RectTransform;
			Vector3 childTopLeft = this.GetVisualTopLeftCorner(mainIcon.rectTransform);
			Vector3 parentTopLeft = this.GetVisualTopLeftCorner(parentRect);
			Vector3 offset = parentTopLeft - childTopLeft;
			mainIcon.rectTransform.position += offset;
		}

		// Token: 0x06007767 RID: 30567 RVA: 0x003790E8 File Offset: 0x003772E8
		private Vector3 GetVisualTopLeftCorner(RectTransform rt)
		{
			rt.GetWorldCorners(this._rectCorners);
			float maxY = this._rectCorners[0].y;
			for (int i = 1; i < 4; i++)
			{
				bool flag = this._rectCorners[i].y > maxY;
				if (flag)
				{
					maxY = this._rectCorners[i].y;
				}
			}
			float tolerance = 0.0001f;
			Vector3 topLeft = Vector3.zero;
			bool firstSet = false;
			for (int j = 0; j < 4; j++)
			{
				bool flag2 = Mathf.Abs(this._rectCorners[j].y - maxY) <= tolerance;
				if (flag2)
				{
					bool flag3 = !firstSet || this._rectCorners[j].x < topLeft.x;
					if (flag3)
					{
						topLeft = this._rectCorners[j];
						firstSet = true;
					}
				}
			}
			return topLeft;
		}

		// Token: 0x06007768 RID: 30568 RVA: 0x003791E8 File Offset: 0x003773E8
		private Vector3 GetVisualBottomRightCorner(RectTransform rt)
		{
			rt.GetLocalCorners(this._rectCorners);
			float maxY = this._rectCorners[0].y;
			for (int i = 1; i < 4; i++)
			{
				bool flag = this._rectCorners[i].y < maxY;
				if (flag)
				{
					maxY = this._rectCorners[i].y;
				}
			}
			float tolerance = 0.0001f;
			Vector3 bottomRight = Vector3.zero;
			bool firstSet = false;
			for (int j = 0; j < 4; j++)
			{
				bool flag2 = Mathf.Abs(this._rectCorners[j].y - maxY) <= tolerance;
				if (flag2)
				{
					bool flag3 = !firstSet || this._rectCorners[j].x > bottomRight.x;
					if (flag3)
					{
						bottomRight = this._rectCorners[j];
						firstSet = true;
					}
				}
			}
			return bottomRight;
		}

		// Token: 0x06007769 RID: 30569 RVA: 0x003792E8 File Offset: 0x003774E8
		private void SetRotationAndFlipVisual(CImage mainIcon, int rotationState, bool fliped)
		{
			Vector3 targetFlipScale = (rotationState == 0 || rotationState == 2) ? this._flipedScaleHor : this._flipedScaleVer;
			mainIcon.rectTransform.localScale = (fliped ? targetFlipScale : Vector3.one);
			switch (rotationState)
			{
			case 0:
				mainIcon.rectTransform.rotation = this.rotation0;
				break;
			case 1:
				mainIcon.rectTransform.rotation = this.rotation90;
				break;
			case 2:
				mainIcon.rectTransform.rotation = this.rotation180;
				break;
			case 3:
				mainIcon.rectTransform.rotation = this.rotation270;
				break;
			}
			this.CloseInParentCorner(mainIcon, rotationState, fliped);
		}

		// Token: 0x0600776A RID: 30570 RVA: 0x0037939C File Offset: 0x0037759C
		private short GetRandomPieceIndex(short except)
		{
			short result = this._candidatesNormal[Random.Range(0, this._candidatesNormal.Count)].TemplateId;
			bool flag = result == except;
			if (flag)
			{
				result += 1;
				bool flag2 = (int)result >= this._candidatesNormal.Count;
				if (flag2)
				{
					return 0;
				}
			}
			return result;
		}

		// Token: 0x0600776B RID: 30571 RVA: 0x003793F9 File Offset: 0x003775F9
		private void OnClickFlip()
		{
			this._currentPreviewData.Flip();
			this.RefreshPreviewVisual();
		}

		// Token: 0x0600776C RID: 30572 RVA: 0x0037940F File Offset: 0x0037760F
		private void OnClickRotate()
		{
			this._currentPreviewData.RotateCW90();
			this.RefreshPreviewVisual();
		}

		// Token: 0x0600776D RID: 30573 RVA: 0x00379428 File Offset: 0x00377628
		private void RefreshPreviewVisual()
		{
			this.SetRotationAndFlipVisual(this.previewPieceMainIcon, this._currentPreviewData.RotationType, this._currentPreviewData.IsFliped);
			CImage previewValidImg = this.checkPreview.CGet<CImage>("Valid");
			CImage previewInvalidImg = this.checkPreview.CGet<CImage>("Invalid");
			this.SetRotationAndFlipVisual(previewValidImg, this._currentPreviewData.RotationType, this._currentPreviewData.IsFliped);
			this.SetRotationAndFlipVisual(previewInvalidImg, this._currentPreviewData.RotationType, this._currentPreviewData.IsFliped);
		}

		// Token: 0x0600776E RID: 30574 RVA: 0x003794B8 File Offset: 0x003776B8
		private static EBoardGridState[,] ConfigShape2GridShape3x3(JieqingGamePeaceItem pieceConfig)
		{
			EBoardGridState[,] gridShape = new EBoardGridState[3, 3];
			for (int i = 0; i < pieceConfig.Shape.Length; i++)
			{
				int grid = i / 4;
				int edge = i % 4;
				bool flag = pieceConfig.Shape[i];
				if (flag)
				{
					gridShape[grid / (int)pieceConfig.Width, grid % (int)pieceConfig.Width] |= ViewJieQingTangram.GetGridStateByEdgeInde(edge);
				}
			}
			return gridShape;
		}

		// Token: 0x0600776F RID: 30575 RVA: 0x00379528 File Offset: 0x00377728
		private static void RotateGridShape(EBoardGridState[,] target, EBoardGridState[,] initShape, int rotationState, bool fliped)
		{
			int R = initShape.GetLength(0);
			int C = initShape.GetLength(1);
			EBoardGridState[,] temp = new EBoardGridState[R, C];
			Array.Copy(initShape, temp, initShape.Length);
			if (fliped)
			{
				bool flag = rotationState == 0 || rotationState == 2;
				if (flag)
				{
					temp = ViewJieQingTangram.FlipHorizontal(temp);
				}
				else
				{
					temp = ViewJieQingTangram.FlipVertical(temp);
				}
			}
			int times = rotationState % 4;
			for (int i = 0; i < times; i++)
			{
				temp = ViewJieQingTangram.Rotate90(temp);
			}
			int minR;
			int maxR;
			int minC;
			int maxC;
			ViewJieQingTangram.GetBounds(temp, out minR, out maxR, out minC, out maxC);
			Array.Clear(target, 0, target.Length);
			for (int r = 0; r <= maxR - minR; r++)
			{
				for (int c = 0; c <= maxC - minC; c++)
				{
					target[r, c] = temp[minR + r, minC + c];
				}
			}
		}

		// Token: 0x06007770 RID: 30576 RVA: 0x00379624 File Offset: 0x00377824
		private static EBoardGridState[,] FlipHorizontal(EBoardGridState[,] src)
		{
			int rows = src.GetLength(0);
			int cols = src.GetLength(1);
			EBoardGridState[,] dst = new EBoardGridState[rows, cols];
			for (int r = 0; r < rows; r++)
			{
				for (int c = 0; c < cols; c++)
				{
					dst[r, cols - 1 - c] = ViewJieQingTangram.FlipEdgesHorizontal(src[r, c]);
				}
			}
			return dst;
		}

		// Token: 0x06007771 RID: 30577 RVA: 0x00379694 File Offset: 0x00377894
		private static EBoardGridState[,] FlipVertical(EBoardGridState[,] src)
		{
			int rows = src.GetLength(0);
			int cols = src.GetLength(1);
			EBoardGridState[,] dst = new EBoardGridState[rows, cols];
			for (int r = 0; r < rows; r++)
			{
				for (int c = 0; c < cols; c++)
				{
					dst[rows - 1 - r, c] = ViewJieQingTangram.FlipEdgesVertical(src[r, c]);
				}
			}
			return dst;
		}

		// Token: 0x06007772 RID: 30578 RVA: 0x00379704 File Offset: 0x00377904
		private static EBoardGridState[,] Rotate90(EBoardGridState[,] src)
		{
			int rows = src.GetLength(0);
			int cols = src.GetLength(1);
			EBoardGridState[,] dst = new EBoardGridState[cols, rows];
			for (int r = 0; r < rows; r++)
			{
				for (int c = 0; c < cols; c++)
				{
					dst[c, rows - 1 - r] = ViewJieQingTangram.RotateEdges90CW(src[r, c]);
				}
			}
			return dst;
		}

		// Token: 0x06007773 RID: 30579 RVA: 0x00379774 File Offset: 0x00377974
		private static void GetBounds(EBoardGridState[,] grid, out int minR, out int maxR, out int minC, out int maxC)
		{
			int rows = grid.GetLength(0);
			int cols = grid.GetLength(1);
			minR = rows;
			maxR = -1;
			minC = cols;
			maxC = -1;
			for (int r = 0; r < rows; r++)
			{
				for (int c = 0; c < cols; c++)
				{
					bool flag = grid[r, c] > EBoardGridState.None;
					if (flag)
					{
						bool flag2 = r < minR;
						if (flag2)
						{
							minR = r;
						}
						bool flag3 = r > maxR;
						if (flag3)
						{
							maxR = r;
						}
						bool flag4 = c < minC;
						if (flag4)
						{
							minC = c;
						}
						bool flag5 = c > maxC;
						if (flag5)
						{
							maxC = c;
						}
					}
				}
			}
			bool flag6 = maxR == -1;
			if (flag6)
			{
				minR = (minC = 0);
				maxR = (maxC = 0);
			}
		}

		// Token: 0x06007774 RID: 30580 RVA: 0x00379838 File Offset: 0x00377A38
		private static EBoardGridState RotateEdges90CW(EBoardGridState edges)
		{
			EBoardGridState result = EBoardGridState.None;
			bool flag = edges.HasFlag(EBoardGridState.Top);
			if (flag)
			{
				result |= EBoardGridState.Right;
			}
			bool flag2 = edges.HasFlag(EBoardGridState.Right);
			if (flag2)
			{
				result |= EBoardGridState.Bottom;
			}
			bool flag3 = edges.HasFlag(EBoardGridState.Bottom);
			if (flag3)
			{
				result |= EBoardGridState.Left;
			}
			bool flag4 = edges.HasFlag(EBoardGridState.Left);
			if (flag4)
			{
				result |= EBoardGridState.Top;
			}
			return result;
		}

		// Token: 0x06007775 RID: 30581 RVA: 0x003798B8 File Offset: 0x00377AB8
		private static EBoardGridState FlipEdgesHorizontal(EBoardGridState edges)
		{
			EBoardGridState result = EBoardGridState.None;
			bool flag = edges.HasFlag(EBoardGridState.Left);
			if (flag)
			{
				result |= EBoardGridState.Right;
			}
			bool flag2 = edges.HasFlag(EBoardGridState.Right);
			if (flag2)
			{
				result |= EBoardGridState.Left;
			}
			bool flag3 = edges.HasFlag(EBoardGridState.Top);
			if (flag3)
			{
				result |= EBoardGridState.Top;
			}
			bool flag4 = edges.HasFlag(EBoardGridState.Bottom);
			if (flag4)
			{
				result |= EBoardGridState.Bottom;
			}
			return result;
		}

		// Token: 0x06007776 RID: 30582 RVA: 0x00379938 File Offset: 0x00377B38
		private static EBoardGridState FlipEdgesVertical(EBoardGridState state)
		{
			EBoardGridState newState = EBoardGridState.None;
			bool flag = (state & EBoardGridState.Top) > EBoardGridState.None;
			if (flag)
			{
				newState |= EBoardGridState.Bottom;
			}
			bool flag2 = (state & EBoardGridState.Bottom) > EBoardGridState.None;
			if (flag2)
			{
				newState |= EBoardGridState.Top;
			}
			bool flag3 = (state & EBoardGridState.Left) > EBoardGridState.None;
			if (flag3)
			{
				newState |= EBoardGridState.Left;
			}
			bool flag4 = (state & EBoardGridState.Right) > EBoardGridState.None;
			if (flag4)
			{
				newState |= EBoardGridState.Right;
			}
			return newState;
		}

		// Token: 0x06007777 RID: 30583 RVA: 0x0037998C File Offset: 0x00377B8C
		private static EBoardGridState GetGridStateByEdgeInde(int edge)
		{
			EBoardGridState result;
			switch (edge)
			{
			case 0:
				result = EBoardGridState.Top;
				break;
			case 1:
				result = EBoardGridState.Right;
				break;
			case 2:
				result = EBoardGridState.Bottom;
				break;
			case 3:
				result = EBoardGridState.Left;
				break;
			default:
				result = EBoardGridState.None;
				break;
			}
			return result;
		}

		// Token: 0x040059FE RID: 23038
		[SerializeField]
		private float _blockWidthOffset = 116f;

		// Token: 0x040059FF RID: 23039
		[SerializeField]
		private float _blockHeightOffset = -116f;

		// Token: 0x04005A00 RID: 23040
		[Header("特效")]
		[SerializeField]
		private RectTransform successEffsRect;

		// Token: 0x04005A01 RID: 23041
		[SerializeField]
		private RectTransform failEffsRect;

		// Token: 0x04005A02 RID: 23042
		[SerializeField]
		private RectTransform effShowsRect;

		// Token: 0x04005A03 RID: 23043
		[SerializeField]
		private RectTransform effActiveRect;

		// Token: 0x04005A04 RID: 23044
		[SerializeField]
		private UIParticle guideSingleFirst;

		// Token: 0x04005A05 RID: 23045
		[SerializeField]
		private UIParticle guideSingleSecond;

		// Token: 0x04005A06 RID: 23046
		[SerializeField]
		private UIParticle guideSingleThird;

		// Token: 0x04005A07 RID: 23047
		[SerializeField]
		private UIParticle guideLoop;

		// Token: 0x04005A08 RID: 23048
		[SerializeField]
		private UIParticle effGameBoard;

		// Token: 0x04005A09 RID: 23049
		[SerializeField]
		private UIParticle effGainScore;

		// Token: 0x04005A0A RID: 23050
		private UIParticle[] _pieceShowEffs;

		// Token: 0x04005A0B RID: 23051
		private UIParticle[] _pieceActiveEffs;

		// Token: 0x04005A0C RID: 23052
		private UIParticle[] _resultSuccessEffs;

		// Token: 0x04005A0D RID: 23053
		private UIParticle[] _resultFailEffs;

		// Token: 0x04005A0E RID: 23054
		[SerializeField]
		private CButton buttonChange;

		// Token: 0x04005A0F RID: 23055
		[SerializeField]
		private CButton buttonRetry;

		// Token: 0x04005A10 RID: 23056
		[SerializeField]
		private CButton buttonClose;

		// Token: 0x04005A11 RID: 23057
		[SerializeField]
		private TooltipInvoker mouseTipButtonChange;

		// Token: 0x04005A12 RID: 23058
		[SerializeField]
		private TangramDraggable previewPiece;

		// Token: 0x04005A13 RID: 23059
		[SerializeField]
		private CImage previewPieceMainIcon;

		// Token: 0x04005A14 RID: 23060
		[SerializeField]
		private CEmptyGraphic previewPieceEmptyGraphic;

		// Token: 0x04005A15 RID: 23061
		[SerializeField]
		private TangramDraggable previewPiece2;

		// Token: 0x04005A16 RID: 23062
		[SerializeField]
		private CImage previewPiece2MainIcon;

		// Token: 0x04005A17 RID: 23063
		[SerializeField]
		private RectTransform boardRect;

		// Token: 0x04005A18 RID: 23064
		[SerializeField]
		private RectTransform zoneRect;

		// Token: 0x04005A19 RID: 23065
		[SerializeField]
		private RectTransform zoneMask;

		// Token: 0x04005A1A RID: 23066
		[SerializeField]
		private TextMeshProUGUI previewTxt;

		// Token: 0x04005A1B RID: 23067
		[SerializeField]
		private TextMeshProUGUI changeAmountTxt;

		// Token: 0x04005A1C RID: 23068
		[SerializeField]
		private Refers checkPreview;

		// Token: 0x04005A1D RID: 23069
		[SerializeField]
		private Refers boardPiecePrefab;

		// Token: 0x04005A1E RID: 23070
		[SerializeField]
		private SpriteLabel roundNumberComp;

		// Token: 0x04005A1F RID: 23071
		[SerializeField]
		private SpriteLabel scoreNumberComp;

		// Token: 0x04005A20 RID: 23072
		[SerializeField]
		private RectTransform previewBoard;

		// Token: 0x04005A21 RID: 23073
		[SerializeField]
		private GameObject covers;

		// Token: 0x04005A22 RID: 23074
		[SerializeField]
		private CImage resultIcon;

		// Token: 0x04005A23 RID: 23075
		[Header("快捷键提示Rect")]
		[SerializeField]
		private RectTransform hotkeyHints;

		// Token: 0x04005A24 RID: 23076
		[Header("顶部标题")]
		[SerializeField]
		private TextMeshProUGUI chapterTitle;

		// Token: 0x04005A25 RID: 23077
		[SerializeField]
		private CImage progressPreview;

		// Token: 0x04005A26 RID: 23078
		[SerializeField]
		private CImage progressCurrent;

		// Token: 0x04005A27 RID: 23079
		private RectTransform _hotkeyHintParent;

		// Token: 0x04005A28 RID: 23080
		private const int _fixedPieceIndex = 7;

		// Token: 0x04005A29 RID: 23081
		private const int _boardSize = 7;

		// Token: 0x04005A2A RID: 23082
		private Vector3 _flipedScaleHor = new Vector3(-1f, 1f, 1f);

		// Token: 0x04005A2B RID: 23083
		private Vector3 _flipedScaleVer = new Vector3(1f, -1f, 1f);

		// Token: 0x04005A2C RID: 23084
		private Quaternion rotation0 = Quaternion.Euler(0f, 0f, 0f);

		// Token: 0x04005A2D RID: 23085
		private Quaternion rotation90 = Quaternion.Euler(0f, 0f, -90f);

		// Token: 0x04005A2E RID: 23086
		private Quaternion rotation180 = Quaternion.Euler(0f, 0f, -180f);

		// Token: 0x04005A2F RID: 23087
		private Quaternion rotation270 = Quaternion.Euler(0f, 0f, -270f);

		// Token: 0x04005A30 RID: 23088
		private Vector2 _commonOffset;

		// Token: 0x04005A31 RID: 23089
		private Vector2 _tempSingleBlockOffset;

		// Token: 0x04005A32 RID: 23090
		private Vector2 _tempOneAndHalfBlockOffset;

		// Token: 0x04005A33 RID: 23091
		private Vector2 _tempDoubleBlockOffset;

		// Token: 0x04005A34 RID: 23092
		private Vector2 _tempTrippleBlockOffset;

		// Token: 0x04005A35 RID: 23093
		private Dictionary<short, Vector2> _hintPositionOffset;

		// Token: 0x04005A36 RID: 23094
		private short _currentTotalChapterIndex = 3;

		// Token: 0x04005A37 RID: 23095
		private SectStoryJieqingGame _storyData;

		// Token: 0x04005A38 RID: 23096
		private SectStoryJieqingGame _chapterStoryData;

		// Token: 0x04005A39 RID: 23097
		private bool _inited;

		// Token: 0x04005A3A RID: 23098
		private List<JieqingGamePeaceItem> _candidatesNormal;

		// Token: 0x04005A3B RID: 23099
		private ViewJieQingTangram.PieceData _currentPreviewData;

		// Token: 0x04005A3C RID: 23100
		private Vector3[] _rectCorners = new Vector3[4];

		// Token: 0x04005A3D RID: 23101
		private EBoardGridState[,] _boardGridData;

		// Token: 0x04005A3E RID: 23102
		private List<ViewJieQingTangram.PieceData> _currentBoardPieceData;

		// Token: 0x04005A3F RID: 23103
		private ViewJieQingTangram.EPageState _currentState = ViewJieQingTangram.EPageState.Normal;

		// Token: 0x04005A40 RID: 23104
		private bool _guildDisplayed = false;

		// Token: 0x04005A41 RID: 23105
		private bool _quiting = false;

		// Token: 0x04005A42 RID: 23106
		private float _loopGuideCounter = 0f;

		// Token: 0x04005A43 RID: 23107
		private bool _loopGuideActive;

		// Token: 0x04005A44 RID: 23108
		private bool _canCheckLeftMouseUp = true;

		// Token: 0x04005A45 RID: 23109
		private int _preScroe = 0;

		// Token: 0x02001ED4 RID: 7892
		private class PieceData
		{
			// Token: 0x17001904 RID: 6404
			// (get) Token: 0x0600F1DC RID: 61916 RVA: 0x00617A0B File Offset: 0x00615C0B
			public short TemplateId { get; }

			// Token: 0x17001905 RID: 6405
			// (get) Token: 0x0600F1DD RID: 61917 RVA: 0x00617A13 File Offset: 0x00615C13
			public sbyte ResourceIndex
			{
				get
				{
					return JieqingGamePeace.Instance[this.TemplateId].ArtResourceIndex;
				}
			}

			// Token: 0x17001906 RID: 6406
			// (get) Token: 0x0600F1DE RID: 61918 RVA: 0x00617A2A File Offset: 0x00615C2A
			// (set) Token: 0x0600F1DF RID: 61919 RVA: 0x00617A32 File Offset: 0x00615C32
			public bool IsFliped { get; private set; }

			// Token: 0x17001907 RID: 6407
			// (get) Token: 0x0600F1E0 RID: 61920 RVA: 0x00617A3B File Offset: 0x00615C3B
			// (set) Token: 0x0600F1E1 RID: 61921 RVA: 0x00617A43 File Offset: 0x00615C43
			public int RotationType { get; private set; }

			// Token: 0x0600F1E2 RID: 61922 RVA: 0x00617A4C File Offset: 0x00615C4C
			public PieceData(JieqingGamePeaceItem pieceConfig)
			{
				this.TemplateId = pieceConfig.TemplateId;
				this.CurrentShape = ViewJieQingTangram.ConfigShape2GridShape3x3(pieceConfig);
				this.InitShape = (this.CurrentShape.Clone() as EBoardGridState[,]);
				this.IsFliped = false;
				this.RotationType = 0;
			}

			// Token: 0x0600F1E3 RID: 61923 RVA: 0x00617AA0 File Offset: 0x00615CA0
			public PieceData(JieqingGamePeaceItem pieceConfig, short rotationType, bool fliped)
			{
				this.TemplateId = pieceConfig.TemplateId;
				this.InitShape = ViewJieQingTangram.ConfigShape2GridShape3x3(pieceConfig);
				this.IsFliped = fliped;
				this.RotationType = (int)rotationType;
				this.CurrentShape = new EBoardGridState[3, 3];
				ViewJieQingTangram.RotateGridShape(this.CurrentShape, this.InitShape, this.RotationType, this.IsFliped);
			}

			// Token: 0x0600F1E4 RID: 61924 RVA: 0x00617B08 File Offset: 0x00615D08
			public void RotateCW90()
			{
				this.PrintGridState(this.CurrentShape);
				int previousRotationType = this.RotationType;
				this.RotationType += (this.IsFliped ? -1 : 1);
				this.RotationType = ((this.RotationType < 0) ? 3 : this.RotationType);
				this.RotationType %= 4;
				ViewJieQingTangram.RotateGridShape(this.CurrentShape, this.InitShape, this.RotationType, this.IsFliped);
				this.PrintGridState(this.CurrentShape);
			}

			// Token: 0x0600F1E5 RID: 61925 RVA: 0x00617B98 File Offset: 0x00615D98
			public void Flip()
			{
				this.PrintGridState(this.CurrentShape);
				this.IsFliped = !this.IsFliped;
				ViewJieQingTangram.RotateGridShape(this.CurrentShape, this.InitShape, this.RotationType, this.IsFliped);
				this.PrintGridState(this.CurrentShape);
			}

			// Token: 0x0600F1E6 RID: 61926 RVA: 0x00617BF0 File Offset: 0x00615DF0
			private void PrintGridState(EBoardGridState[,] targetShape)
			{
				int height = targetShape.GetLength(0);
				int width = targetShape.GetLength(1);
				string result = "";
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						int val = (int)targetShape[y, x];
						string binary = Convert.ToString(val, 2).PadLeft(4, '0');
						result = result + binary + " ";
					}
					result += "\n";
				}
			}

			// Token: 0x0400CB36 RID: 52022
			public EBoardGridState[,] CurrentShape;

			// Token: 0x0400CB37 RID: 52023
			public EBoardGridState[,] InitShape;

			// Token: 0x0400CB38 RID: 52024
			public RectTransform SelectedStar;

			// Token: 0x0400CB39 RID: 52025
			public Refers BoardPiece;
		}

		// Token: 0x02001ED5 RID: 7893
		private enum EPageState
		{
			// Token: 0x0400CB3B RID: 52027
			Normal,
			// Token: 0x0400CB3C RID: 52028
			DragPiece
		}
	}
}
