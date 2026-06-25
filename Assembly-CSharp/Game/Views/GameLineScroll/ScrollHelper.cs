using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Global;
using GameData.Domains.Taiwu;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.GameLineScroll
{
	// Token: 0x02000A1A RID: 2586
	public static class ScrollHelper
	{
		// Token: 0x06007EB4 RID: 32436 RVA: 0x003AF2C0 File Offset: 0x003AD4C0
		private static void InitAreaStoryScroll(Refers curPage, bool isInGameWorld, int currSaveSlot)
		{
			GameObject areaStoryScrollHolder = curPage.CGet<GameObject>("AreaStoryScrollHolder");
			sbyte i = 0;
			while ((int)i < areaStoryScrollHolder.transform.childCount)
			{
				sbyte orgTemplateId = i + 1;
				sbyte b;
				if (!isInGameWorld)
				{
					if (currSaveSlot < 0)
					{
						b = 0;
					}
					else
					{
						WorldInfo worldInfo = GlobalOperations.ArchivesInfo[currSaveSlot].WorldInfo;
						b = ((worldInfo != null) ? worldInfo.StateTaskStatuses[(int)i] : 0);
					}
				}
				else
				{
					b = SingletonObject.getInstance<BuildingModel>().GetStateTaskStatus((int)orgTemplateId);
				}
				sbyte status = b;
				bool isShow = status != 0;
				bool prosper = status == 1;
				AreaStoryScrollItem scrollItem = areaStoryScrollHolder.transform.GetChild((int)i).GetComponent<AreaStoryScrollItem>();
				scrollItem.Set((int)i, isShow ? Organization.Instance[orgTemplateId].SectMainStory.Name : string.Empty, isShow, delegate
				{
					ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("orgTemplateId", orgTemplateId).Set("prosper", prosper);
					UIElement.AreaStoryScroll.SetOnInitArgs(args);
					UIManager.Instance.ShowUI(UIElement.AreaStoryScroll, true);
				});
				i += 1;
			}
		}

		// Token: 0x06007EB5 RID: 32437 RVA: 0x003AF3A8 File Offset: 0x003AD5A8
		public static void OnInit(Refers root, int unlockIndex, bool isInGameWorld, int currSaveSlot, bool isUnlockScroll = false, int targetScrollIndex = 2)
		{
			ScrollHelper.InitAreaStoryScroll(root, isInGameWorld, currSaveSlot);
			GameObject gameObject = root.CGet<GameObject>("ScrollRoot2");
			if (gameObject != null)
			{
				gameObject.GetComponent<TaiwuScroll>().Set(isInGameWorld, currSaveSlot, ScrollHelper._uiElement);
			}
			bool flag = !isInGameWorld;
			if (flag)
			{
				bool flag2 = currSaveSlot >= 0 && GlobalOperations.ArchivesInfo[currSaveSlot].Status == 1;
				if (flag2)
				{
					WorldInfo gameData = GlobalOperations.ArchivesInfo[currSaveSlot].WorldInfo;
					ScrollHelper._xiangshuAvatarTaskStatuses = gameData.XiangshuAvatarTaskStatuses;
					ScrollHelper._beatRanChenZi = gameData.BeatRanChenZi;
				}
				else
				{
					ScrollHelper._xiangshuAvatarTaskStatuses = Array.Empty<XiangshuAvatarTaskStatus>();
					ScrollHelper._beatRanChenZi = false;
				}
			}
			else
			{
				BasicGameData gameData2 = SingletonObject.getInstance<BasicGameData>();
				ScrollHelper._xiangshuAvatarTaskStatuses = gameData2.XiangshuAvatarTaskStatusArray;
				ScrollHelper._beatRanChenZi = gameData2.BeatRanChenZi;
			}
			ScrollHelper._unlockIndex = unlockIndex;
			ScrollHelper._root = root;
			ScrollHelper._isInGameWorld = isInGameWorld;
			ScrollHelper.SetIllustrationBtnVisible(false);
			bool flag3 = !ScrollHelper._initedRoots.Contains(ScrollHelper._root);
			if (flag3)
			{
				ScrollHelper.Init();
			}
			else
			{
				ScrollHelper.RebindScrollRootRefs();
			}
			CToggleGroup scrollTypeToggleGroup = ScrollHelper._root.CGet<CToggleGroup>("ScrollTypeToggleGroup");
			scrollTypeToggleGroup.Init(-1);
			scrollTypeToggleGroup.OnActiveIndexChange -= ScrollHelper.OnScrollTypeToggleGroupChanged;
			scrollTypeToggleGroup.OnActiveIndexChange += ScrollHelper.OnScrollTypeToggleGroupChanged;
			scrollTypeToggleGroup.Get(0).interactable = ScrollHelper._xiangshuAvatarTaskStatuses.Any((XiangshuAvatarTaskStatus x) => x.JuniorXiangshuTaskStatus != 0);
			bool hasCompletedAreaStory = false;
			if (isInGameWorld)
			{
				BuildingModel buildingModel = SingletonObject.getInstance<BuildingModel>();
				for (sbyte i = 1; i < 15; i += 1)
				{
					bool flag4 = buildingModel.GetStateTaskStatus((int)i) != 0;
					if (flag4)
					{
						hasCompletedAreaStory = true;
						break;
					}
				}
			}
			else
			{
				bool flag5 = currSaveSlot >= 0 && GlobalOperations.ArchivesInfo.CheckIndex(currSaveSlot) && GlobalOperations.ArchivesInfo[currSaveSlot].Status == 1;
				if (flag5)
				{
					WorldInfo worldInfo = GlobalOperations.ArchivesInfo[currSaveSlot].WorldInfo;
					sbyte[] stateTaskStatuses = (worldInfo != null) ? worldInfo.StateTaskStatuses : null;
					bool flag6 = stateTaskStatuses != null;
					if (flag6)
					{
						foreach (sbyte status in stateTaskStatuses)
						{
							bool flag7 = status != 0;
							if (flag7)
							{
								hasCompletedAreaStory = true;
								break;
							}
						}
					}
				}
			}
			scrollTypeToggleGroup.Get(1).interactable = hasCompletedAreaStory;
			Selectable selectable = scrollTypeToggleGroup.Get(2);
			bool interactable;
			if (!isInGameWorld)
			{
				if (GlobalOperations.ArchivesInfo.CheckIndex(currSaveSlot))
				{
					WorldInfo worldInfo2 = GlobalOperations.ArchivesInfo[currSaveSlot].WorldInfo;
					if (worldInfo2 == null)
					{
						interactable = false;
					}
					else
					{
						TotalTaiwuLifeSummaryInfo totalTaiwuLifeSummaryInfo = worldInfo2.TotalTaiwuLifeSummaryInfo;
						int? num;
						if (totalTaiwuLifeSummaryInfo == null)
						{
							num = null;
						}
						else
						{
							List<TaiwuLifeSummary> totalTaiwuLifeSummaries = totalTaiwuLifeSummaryInfo.TotalTaiwuLifeSummaries;
							num = ((totalTaiwuLifeSummaries != null) ? new int?(totalTaiwuLifeSummaries.Count) : null);
						}
						int? num2 = num;
						int num3 = 0;
						interactable = (num2.GetValueOrDefault() >= num3 & num2 != null);
					}
				}
				else
				{
					interactable = false;
				}
			}
			else
			{
				interactable = true;
			}
			selectable.interactable = interactable;
			int selectedIndex = targetScrollIndex;
			bool flag8 = !scrollTypeToggleGroup.Get(targetScrollIndex).interactable;
			if (flag8)
			{
				selectedIndex = 0;
				for (int j = 0; j < 3; j++)
				{
					bool interactable2 = scrollTypeToggleGroup.Get(j).interactable;
					if (interactable2)
					{
						selectedIndex = j;
						break;
					}
				}
			}
			scrollTypeToggleGroup.Set(selectedIndex, true);
			short k = 0;
			while ((int)k < ScrollHelper._scrollHolder.childCount)
			{
				GameLineScrollItem gameLineScrollItem = ScrollHelper._scrollHolder.GetChild((int)k).GetComponent<GameLineScrollItem>();
				bool unlocked = (int)k < ScrollHelper._xiangshuAvatarTaskStatuses.Length && ScrollHelper._xiangshuAvatarTaskStatuses[(int)k].JuniorXiangshuTaskStatus != 0;
				bool isAllUnlock = (int)k < ScrollHelper._xiangshuAvatarTaskStatuses.Length && ScrollHelper._xiangshuAvatarTaskStatuses[(int)k].JuniorXiangshuTaskStatus >= 5;
				gameLineScrollItem.Set(k, unlocked, isAllUnlock);
				short currentIndex = k;
				gameLineScrollItem.ScrollBtn.ClearAndAddListener(delegate
				{
					ScrollHelper.ExpandScroll((int)currentIndex, true);
				});
				k += 1;
			}
			ScrollHelper._showingUnlock = (ScrollHelper._unlockIndex >= 0 && !isUnlockScroll && targetScrollIndex == 0);
			ScrollHelper._isUnlockScroll = isUnlockScroll;
			ScrollHelper._scrollHolderBack.color = ScrollHelper._scrollHolderBack.color.SetAlpha((float)(ScrollHelper._showingUnlock ? 0 : 1));
			ScrollHelper._scrollHolder.gameObject.SetActive(!ScrollHelper._showingUnlock);
			ScrollHelper._illustrationRefers.gameObject.SetActive(ScrollHelper._showingUnlock);
			CButton cbutton = ScrollHelper._root.CGet<CButton>("ViewClose");
			if (cbutton != null)
			{
				cbutton.gameObject.SetActive(!ScrollHelper._showingUnlock);
			}
			CButton cbutton2 = ScrollHelper._root.CGet<CButton>("UnlockBtn");
			if (cbutton2 != null)
			{
				cbutton2.gameObject.SetActive(ScrollHelper._showingUnlock);
			}
			CButton cbutton3 = ScrollHelper._root.CGet<CButton>("UnlockBtn");
			if (cbutton3 != null)
			{
				cbutton3.ClearAndAddListener(delegate
				{
					ScrollHelper.UnlockStory();
					CButton cbutton4 = ScrollHelper._root.CGet<CButton>("UnlockBtn");
					if (cbutton4 != null)
					{
						cbutton4.gameObject.SetActive(false);
					}
				});
			}
			CToggleGroup ctoggleGroup = ScrollHelper._root.CGet<CToggleGroup>("ScrollTypeToggleGroup");
			if (ctoggleGroup != null)
			{
				ctoggleGroup.gameObject.SetActive(!ScrollHelper._showingUnlock);
			}
			RectTransform rectTransform = ScrollHelper._root.CGet<RectTransform>("ViewTitle");
			if (rectTransform != null)
			{
				rectTransform.gameObject.SetActive(!ScrollHelper._showingUnlock);
			}
			RectTransform rectTransform2 = ScrollHelper._root.CGet<RectTransform>("Notice");
			if (rectTransform2 != null)
			{
				rectTransform2.gameObject.SetActive(!ScrollHelper._showingUnlock);
			}
			ScrollHelper._root.CGet<GameObject>("Close").SetActive(false);
			ScrollHelper._root.CGet<GameObject>("ClickMask").SetActive(false);
			bool showingUnlock = ScrollHelper._showingUnlock;
			if (showingUnlock)
			{
				ScrollHelper.ShowUnlockIllustration();
			}
			bool isUnlockScroll2 = ScrollHelper._isUnlockScroll;
			if (isUnlockScroll2)
			{
				ScrollHelper.ShowUnlockScroll();
			}
			ScrollHelper.RefreshStoryNavButtonsState();
		}

		// Token: 0x06007EB6 RID: 32438 RVA: 0x003AF958 File Offset: 0x003ADB58
		private static void OnScrollTypeToggleGroupChanged(int newIndex, int oldIndex)
		{
			List<GameObject> scrollList = ScrollHelper._root.CGetList<GameObject>("ScrollRoot");
			for (int i = 0; i < scrollList.Count; i++)
			{
				scrollList[i].gameObject.SetActive(i == newIndex);
			}
		}

		// Token: 0x06007EB7 RID: 32439 RVA: 0x003AF9A4 File Offset: 0x003ADBA4
		public static void ProcessUnlockScrollList(int unlockIndex, bool moveToFirst)
		{
			bool flag = GameApp.Instance.GetCurrentGameStateName() != EGameState.InGame;
			if (!flag)
			{
				TaiwuDomainMethod.AsyncCall.GetUnlockScrollListForDisplay(null, delegate(int offset, RawDataPool dataPool)
				{
					List<IntPair> unlockScrollList = new List<IntPair>();
					Serializer.Deserialize(dataPool, offset, ref unlockScrollList);
					IntPair data = new IntPair(0, unlockIndex);
					unlockScrollList.Remove(data);
					bool moveToFirst2 = moveToFirst;
					if (moveToFirst2)
					{
						unlockScrollList.Insert(0, data);
					}
					GEvent.OnEvent(UiEvents.NotifyBottomUnlockScrollListChange, EasyPool.Get<ArgumentBox>().SetObject("UnlockScrollList", unlockScrollList));
					TaiwuDomainMethod.Call.UpdateUnlockScrollList(unlockScrollList);
				});
			}
		}

		// Token: 0x06007EB8 RID: 32440 RVA: 0x003AF9F0 File Offset: 0x003ADBF0
		public static void Init()
		{
			ScrollHelper._scrollHolder = ScrollHelper._root.CGet<RectTransform>("ScrollHolder");
			ScrollHelper._scrollHolderBack = ScrollHelper._root.CGet<CRawImage>("ScrollBack");
			ScrollHelper._storyTransform = ScrollHelper._root.CGet<RectTransform>("Story");
			ScrollHelper._storyRoot = ScrollHelper._root.CGet<GameObject>("StoryRoot");
			ScrollHelper._uiMaskNode = ScrollHelper._root.CGet<GameObject>("UIMaskNode");
			ScrollHelper._storyDragMove = ScrollHelper._storyTransform.GetComponent<UIRectDragMove>();
			ScrollHelper._illustrationRefers = ScrollHelper._root.CGet<Refers>("Illustration");
			ScrollHelper._illustrationBtn = ScrollHelper._root.CGet<CButton>("ShowIllustration");
			ScrollHelper._storyScrollSmall = ScrollHelper._root.CGet<RectTransform>("ScrollSmall");
			ScrollHelper.InitStoryNavButtons();
			ScrollHelper.SetScrollUIMask(false);
			short i = 0;
			while ((int)i < ScrollHelper._scrollHolder.childCount)
			{
				GameLineScrollItem gameLineScrollItem = ScrollHelper._scrollHolder.GetChild((int)i).GetComponent<GameLineScrollItem>();
				ScrollHelper._scrollOriginPos[(int)i] = gameLineScrollItem.GetComponent<RectTransform>().anchoredPosition;
				gameLineScrollItem.Set(i, true, true);
				i += 1;
			}
			ScrollHelper._initedRoots.Add(ScrollHelper._root);
		}

		// Token: 0x06007EB9 RID: 32441 RVA: 0x003AFB18 File Offset: 0x003ADD18
		private static void RebindScrollRootRefs()
		{
			ScrollHelper._scrollHolder = ScrollHelper._root.CGet<RectTransform>("ScrollHolder");
			ScrollHelper._scrollHolderBack = ScrollHelper._root.CGet<CRawImage>("ScrollBack");
			ScrollHelper._storyTransform = ScrollHelper._root.CGet<RectTransform>("Story");
			ScrollHelper._storyRoot = ScrollHelper._root.CGet<GameObject>("StoryRoot");
			ScrollHelper._uiMaskNode = ScrollHelper._root.CGet<GameObject>("UIMaskNode");
			ScrollHelper._storyDragMove = ScrollHelper._storyTransform.GetComponent<UIRectDragMove>();
			ScrollHelper._illustrationRefers = ScrollHelper._root.CGet<Refers>("Illustration");
			ScrollHelper._illustrationBtn = ScrollHelper._root.CGet<CButton>("ShowIllustration");
			ScrollHelper._storyScrollSmall = ScrollHelper._root.CGet<RectTransform>("ScrollSmall");
			ScrollHelper.InitStoryNavButtons();
		}

		// Token: 0x06007EBA RID: 32442 RVA: 0x003AFBDC File Offset: 0x003ADDDC
		private static void SetScrollUIMask(bool enable)
		{
			bool flag = ScrollHelper._uiMaskNode == null;
			if (!flag)
			{
				ScrollHelper._uiMaskNode.SetActive(enable);
			}
		}

		// Token: 0x06007EBB RID: 32443 RVA: 0x003AFC08 File Offset: 0x003ADE08
		private static void AttachExpandedScrollToStoryRoot()
		{
			ScrollHelper._expandedScroll.SetParent(ScrollHelper._storyRoot.transform, false);
			ScrollHelper._expandedScroll.SetAsLastSibling();
			ScrollHelper._expandedScroll.SetAnchor(ScrollHelper._anchorRight, ScrollHelper._anchorRight);
			ScrollHelper._expandedScroll.anchoredPosition = Vector2.zero;
		}

		// Token: 0x06007EBC RID: 32444 RVA: 0x003AFC5C File Offset: 0x003ADE5C
		private static void PrepareExpandedScrollFlyInOnStoryRoot()
		{
			Vector3 worldPos = ScrollHelper._expandedScroll.position;
			ScrollHelper._expandedScroll.SetParent(ScrollHelper._storyRoot.transform, false);
			ScrollHelper._expandedScroll.SetAsLastSibling();
			ScrollHelper._expandedScroll.SetAnchor(ScrollHelper._anchorRight, ScrollHelper._anchorRight);
			ScrollHelper._expandedScroll.position = worldPos;
		}

		// Token: 0x06007EBD RID: 32445 RVA: 0x003AFCB8 File Offset: 0x003ADEB8
		private static void SyncExpandedScrollWorldPosToStory()
		{
			bool flag = ScrollHelper._expandedScroll != null && ScrollHelper._storyTransform != null;
			if (flag)
			{
				ScrollHelper._expandedScroll.position = ScrollHelper._storyTransform.position;
			}
		}

		// Token: 0x06007EBE RID: 32446 RVA: 0x003AFCFC File Offset: 0x003ADEFC
		public static void HideScroll(bool isHideAll)
		{
			if (isHideAll)
			{
				bool flag = ScrollHelper._illustrationRefers != null && ScrollHelper._illustrationRefers.gameObject.activeSelf;
				if (flag)
				{
					ScrollHelper.HideIllustration(false);
				}
				bool flag2 = ScrollHelper._expandedScroll != null;
				if (flag2)
				{
					ScrollHelper.CollapseScroll(true);
					ScrollHelper._root.CGet<GameObject>("Close").SetActive(false);
				}
				CommandKitBase.SetDisable(false);
				bool flag3 = !ScrollHelper._isInGameWorld;
				if (flag3)
				{
					ScrollHelper._quickHideBanned = false;
				}
			}
			else
			{
				bool flag4 = ScrollHelper._showingUnlock || ScrollHelper._scrollAniSeq != null;
				if (!flag4)
				{
					bool activeSelf = ScrollHelper._illustrationRefers.gameObject.activeSelf;
					if (activeSelf)
					{
						ScrollHelper.HideIllustration(true);
					}
					else
					{
						bool flag5 = ScrollHelper._expandedScroll != null;
						if (flag5)
						{
							ScrollHelper.CollapseScroll(true);
							ScrollHelper._root.CGet<GameObject>("Close").SetActive(false);
						}
					}
				}
			}
		}

		// Token: 0x06007EBF RID: 32447 RVA: 0x003AFDF0 File Offset: 0x003ADFF0
		public static bool QuickHide()
		{
			bool flag = ScrollHelper._showingUnlock || ScrollHelper._scrollAniSeq != null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool activeSelf = ScrollHelper._illustrationRefers.gameObject.activeSelf;
				if (activeSelf)
				{
					ScrollHelper.HideIllustration(true);
					result = false;
				}
				else
				{
					bool flag2 = ScrollHelper._expandedScroll != null;
					if (flag2)
					{
						ScrollHelper.CollapseScroll(true);
						ScrollHelper._root.CGet<GameObject>("Close").SetActive(false);
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06007EC0 RID: 32448 RVA: 0x003AFE70 File Offset: 0x003AE070
		public static void ExpandScroll(int index, bool doAni)
		{
			bool flag = !ScrollHelper._isInGameWorld;
			if (flag)
			{
				ScrollHelper._quickHideBanned = true;
			}
			ScrollHelper.ProcessUnlockScrollList(index, false);
			bool flag2 = !ScrollHelper._showingUnlock && !ScrollHelper._isUnlockScroll;
			if (flag2)
			{
				GEvent.OnEvent(UiEvents.ShowUnlockScrollBtnAnim, EasyPool.Get<ArgumentBox>().Set("NeedShowAnim", false));
			}
			bool flag3 = !ScrollHelper._showingUnlock && !ScrollHelper._isUnlockScroll;
			if (flag3)
			{
				ScrollHelper._root.CGet<GameObject>("Close").SetActive(true);
			}
			ScrollHelper._expandedScroll = ScrollHelper._scrollHolder.GetChild(index).GetComponent<RectTransform>();
			bool flag4 = ScrollHelper._isInGameWorld || true;
			if (flag4)
			{
				ScrollHelper.ChangeScrollScale(ScrollHelper._expandedScroll.GetComponent<GameLineScrollItem>(), true);
			}
			bool isRanChenZi = index >= ScrollHelper._xiangshuAvatarTaskStatuses.Length;
			int pageCount = ScrollHelper.GetPageCount(index);
			RectTransform pageHolder = ScrollHelper._root.CGet<RectTransform>("PageHolder");
			ScrollHelper.SetIllustrationBtnVisible(ScrollHelper.CanShowIllustrationBtn(index));
			ScrollHelper._expanding = true;
			int i = 0;
			while (i < pageHolder.childCount)
			{
				CRawImage rawImg = pageHolder.GetChild(i).GetComponent<CRawImage>();
				bool showPage = i < pageCount;
				rawImg.gameObject.SetActive(showPage);
				bool flag5 = showPage;
				if (flag5)
				{
					bool flag6 = !isRanChenZi;
					int indexOffset;
					if (flag6)
					{
						indexOffset = ((i < 4) ? i : (ScrollHelper.IsGoodEnding(index) ? 4 : 5));
					}
					else
					{
						indexOffset = (ScrollHelper._beatRanChenZi ? 0 : 1);
					}
					short scrollTemplateId = (short)(index * 6 + indexOffset);
					StoryScrollItem scrollConfig = StoryScroll.Instance.GetItem(scrollTemplateId);
					bool flag7 = scrollConfig == null;
					if (!flag7)
					{
						string texturePath = "RemakeResources/Textures/GameLineScroll/" + scrollConfig.StoryImage;
						ResLoader.Load<Texture2D>(texturePath, delegate(Texture2D texture)
						{
							rawImg.texture = texture;
						}, null, false);
						Refers noteRefers = rawImg.GetComponentInChildren<Refers>();
						bool flag8 = null != noteRefers;
						if (flag8)
						{
							TextMeshProUGUI note = noteRefers.CGet<TextMeshProUGUI>("Note");
							note.text = scrollConfig.StoryNote;
							note.enableWordWrapping = (LocalStringManager.CurLanguageType > LocalStringManager.LanguageType.CN);
							note.lineSpacing = ((LocalStringManager.CurLanguageType == LocalStringManager.LanguageType.CN) ? 0f : -30f);
							noteRefers.CGet<CImage>("ResultMark").SetSprite(string.Format("{0}{1}", "ui9_icon_scroll_sect_seal_", (int)scrollConfig.StoryResultMark), false, null);
							noteRefers.CGet<CImage>("TypeIcon").SetSprite(scrollConfig.StoryTypeIcon, false, null);
							if (!true)
							{
							}
							string text;
							switch (indexOffset)
							{
							case 0:
								text = "gamelinescroll_iconbase_0";
								break;
							case 1:
								text = "gamelinescroll_iconbase_0";
								break;
							case 2:
								text = "gamelinescroll_iconbase_0";
								break;
							case 3:
								text = "gamelinescroll_iconbase_0";
								break;
							case 4:
								text = "gamelinescroll_iconbase_2";
								break;
							case 5:
								text = "gamelinescroll_iconbase_1";
								break;
							default:
								if (!true)
								{
								}
								<PrivateImplementationDetails>.ThrowSwitchExpressionException(indexOffset);
								break;
							}
							if (!true)
							{
							}
							string chapterBaseSpriteName = text;
							noteRefers.CGet<CImage>("ChapterBaseImg").SetSprite(chapterBaseSpriteName, false, null);
						}
					}
				}
				IL_306:
				i++;
				continue;
				goto IL_306;
			}
			bool flag9 = !doAni;
			if (flag9)
			{
				ScrollHelper.SetToAniEndState(true);
			}
			else
			{
				bool flag10 = pageCount > 4 || isRanChenZi;
				if (flag10)
				{
					ScrollHelper.UpdateIllustration(ScrollHelper._expandedScroll, false, true);
				}
				GameObject skipUiAni = ScrollHelper._root.CGet<GameObject>("SkipUiAni");
				Sequence scrollAniSeq = ScrollHelper._scrollAniSeq;
				if (scrollAniSeq != null)
				{
					scrollAniSeq.Pause<Sequence>();
				}
				ScrollHelper._scrollAniSeq = DOTween.Sequence();
				ScrollHelper._storyTransform.sizeDelta = ScrollHelper._storyTransform.sizeDelta.SetX(0f);
				ScrollHelper._storyTransform.SetPivot(ScrollHelper._anchorCenter);
				ScrollHelper._storyTransform.position = ScrollHelper._expandedScroll.position;
				ScrollHelper._storyTransform.SetPivot(ScrollHelper._anchorRight);
				ScrollHelper._storyTransform.SetAnchor(ScrollHelper._anchorRight, ScrollHelper._anchorRight);
				ScrollHelper.SetStoryActive(true);
				ScrollHelper._storyTransform.anchoredPosition = new Vector2(ScrollHelper._storyTransform.anchoredPosition.x, 0f);
				ScrollHelper._storyScrollSmall.gameObject.SetActive(false);
				ScrollHelper.SetStoryRootCanvasAlpha(1f);
				ScrollHelper._expandedScroll.GetComponent<GameLineScrollItem>().SetScrollBtn(false);
				ScrollHelper.PrepareExpandedScrollFlyInOnStoryRoot();
				ScrollHelper._scrollAniSeq.Append(ScrollHelper._storyTransform.DOAnchorPosX(0f, 1f, false).OnUpdate(new TweenCallback(ScrollHelper.SyncExpandedScrollWorldPosToStory)));
				ScrollHelper._scrollAniSeq.Join(ScrollHelper._scrollHolderBack.DOFade(0f, 1f));
				for (int j = 0; j < ScrollHelper._scrollHolder.childCount; j++)
				{
					ScrollHelper._scrollAniSeq.Join(ScrollHelper._scrollHolder.GetChild(j).GetComponent<CanvasGroup>().DOFade(0f, 1f));
				}
				ScrollHelper._scrollAniSeq.AppendCallback(delegate
				{
					ScrollHelper._scrollHolder.gameObject.SetActive(false);
					ScrollHelper.AttachExpandedScrollToStoryRoot();
				});
				float storyWidth = (float)(1795 * pageCount + 45);
				float expandDuration = ScrollHelper.GetScrollExpandDuration(ScrollHelper.MaxExpandWidth);
				ScrollHelper._scrollAniSeq.Append(ScrollHelper._storyTransform.DOSizeDelta(ScrollHelper._storyTransform.sizeDelta.SetX(ScrollHelper.MaxExpandWidth), expandDuration, false).SetEase(Ease.Linear));
				ScrollHelper._scrollAniSeq.InsertCallback(1.75f, delegate
				{
					ScrollHelper._storyScrollSmall.gameObject.SetActive(true);
				});
				ScrollHelper._scrollAniSeq.AppendCallback(delegate
				{
					ScrollHelper._storyTransform.sizeDelta = ScrollHelper._storyTransform.sizeDelta.SetX(storyWidth);
					ScrollHelper._storyTransform.SetPivot(ScrollHelper._anchorCenter);
					ScrollHelper._storyTransform.SetAnchor(ScrollHelper._anchorCenter, ScrollHelper._anchorCenter);
					ScrollHelper._scrollAniSeq = null;
					skipUiAni.SetActive(false);
					ScrollHelper.RefreshStoryNavButtonsState();
				});
				ScrollHelper._scrollAniSeq.Play<Sequence>();
				skipUiAni.SetActive(true);
				AudioManager.Instance.PlaySound("UI_GameLineScroll_OpenScroll", false, false);
				ScrollHelper._scrollAniSeq.OnComplete(delegate
				{
					ScrollHelper.ExpandState = true;
					ScrollHelper.RefreshStoryNavButtonsState();
				});
			}
		}

		// Token: 0x06007EC1 RID: 32449 RVA: 0x003B0470 File Offset: 0x003AE670
		public static void CollapseScroll(bool doAni)
		{
			ScrollHelper.ExpandState = false;
			ScrollHelper._expanding = false;
			ScrollHelper.SetIllustrationBtnVisible(false);
			bool flag = !doAni;
			if (flag)
			{
				ScrollHelper.SetToAniEndState(false);
			}
			else
			{
				int index = ScrollHelper._expandedScroll.GetComponent<GameLineScrollItem>().Index;
				int pageCount = ScrollHelper.GetPageCount(index);
				GameObject skipUiAni = ScrollHelper._root.CGet<GameObject>("SkipUiAni");
				Sequence scrollAniSeq = ScrollHelper._scrollAniSeq;
				if (scrollAniSeq != null)
				{
					scrollAniSeq.Pause<Sequence>();
				}
				ScrollHelper._scrollAniSeq = DOTween.Sequence();
				ScrollHelper._storyTransform.SetPivot(ScrollHelper._anchorRight);
				ScrollHelper._storyTransform.SetAnchor(ScrollHelper._anchorRight, ScrollHelper._anchorRight);
				float moveBackDuration = 0.1f * ScrollHelper._storyTransform.anchoredPosition.x / 1000f;
				ScrollHelper._scrollAniSeq.Append(ScrollHelper._storyTransform.DOAnchorPosX(0f, moveBackDuration, false));
				float collapseDuration = ScrollHelper.GetScrollExpandDuration(ScrollHelper.MaxExpandWidth);
				ScrollHelper._scrollAniSeq.AppendCallback(delegate
				{
					ScrollHelper._storyTransform.sizeDelta = ScrollHelper._storyTransform.sizeDelta.SetX(ScrollHelper.MaxExpandWidth);
				});
				ScrollHelper._scrollAniSeq.Append(ScrollHelper._storyTransform.DOSizeDelta(ScrollHelper._storyTransform.sizeDelta.SetX(0f), collapseDuration, false).SetEase(Ease.Linear));
				ScrollHelper._scrollAniSeq.InsertCallback(moveBackDuration + ScrollHelper.GetScrollSmallHideDelayInCollapse(), delegate
				{
					ScrollHelper._storyScrollSmall.gameObject.SetActive(false);
				});
				ScrollHelper._scrollAniSeq.AppendCallback(delegate
				{
					ScrollHelper.SetStoryActive(false);
					ScrollHelper._scrollHolder.gameObject.SetActive(true);
					ScrollHelper._expandedScroll.SetParent(ScrollHelper._scrollHolder, true);
					ScrollHelper._expandedScroll.SetAnchor(ScrollHelper._anchorCenter, ScrollHelper._anchorCenter);
				});
				ScrollHelper._scrollAniSeq.AppendInterval(0.1f);
				ScrollHelper._scrollAniSeq.Append(ScrollHelper._expandedScroll.DOAnchorPos(ScrollHelper._scrollOriginPos[index], 1f, false));
				ScrollHelper._scrollAniSeq.Join(ScrollHelper._scrollHolderBack.DOFade(1f, 1f));
				for (int i = 0; i < ScrollHelper._scrollHolder.childCount; i++)
				{
					ScrollHelper._scrollAniSeq.Join(ScrollHelper._scrollHolder.GetChild(i).GetComponent<CanvasGroup>().DOFade(1f, 1f));
				}
				ScrollHelper._scrollAniSeq.AppendCallback(delegate
				{
					ScrollHelper._expandedScroll.SetSiblingIndex(index);
					ScrollHelper._expandedScroll.GetComponent<GameLineScrollItem>().SetScrollBtn(true);
					ScrollHelper.ChangeScrollScale(ScrollHelper._expandedScroll.GetComponent<GameLineScrollItem>(), false);
					ScrollHelper._expandedScroll = null;
					ScrollHelper._scrollAniSeq = null;
					skipUiAni.SetActive(false);
					CToggleGroup ctoggleGroup = ScrollHelper._root.CGet<CToggleGroup>("ScrollTypeToggleGroup");
					if (ctoggleGroup != null)
					{
						ctoggleGroup.gameObject.SetActive(true);
					}
					CButton cbutton = ScrollHelper._root.CGet<CButton>("ViewClose");
					if (cbutton != null)
					{
						cbutton.gameObject.SetActive(true);
					}
					bool flag2 = !ScrollHelper._isInGameWorld;
					if (flag2)
					{
						ScrollHelper._quickHideBanned = false;
					}
				});
				skipUiAni.SetActive(true);
				AudioManager.Instance.PlaySound("UI_GameLineScroll_CloseScroll", false, false);
			}
		}

		// Token: 0x06007EC2 RID: 32450 RVA: 0x003B06F8 File Offset: 0x003AE8F8
		public static void SetToAniEndState(bool expand)
		{
			int index = ScrollHelper._expandedScroll.GetComponent<GameLineScrollItem>().Index;
			int pageCount = ScrollHelper.GetPageCount(index);
			Sequence scrollAniSeq = ScrollHelper._scrollAniSeq;
			if (scrollAniSeq != null)
			{
				scrollAniSeq.Kill(false);
			}
			ScrollHelper._scrollAniSeq = null;
			CommandKitBase.SetDisable(false);
			ScrollHelper.SetStoryActive(expand);
			ScrollHelper._scrollHolder.gameObject.SetActive(!expand);
			if (expand)
			{
				ScrollHelper.ExpandState = true;
				ScrollHelper.SetStoryRootCanvasAlpha(1f);
				float storyWidth = (float)(1795 * pageCount + 45);
				ScrollHelper._storyTransform.SetPivot(ScrollHelper._anchorCenter);
				ScrollHelper._storyTransform.SetAnchor(ScrollHelper._anchorCenter, ScrollHelper._anchorCenter);
				ScrollHelper._storyTransform.sizeDelta = ScrollHelper._storyTransform.sizeDelta.SetX(storyWidth);
				ScrollHelper._storyTransform.anchoredPosition = ScrollHelper._storyTransform.anchoredPosition.SetX((ScrollHelper._storyTransform.parent.GetComponent<RectTransform>().sizeDelta.x - ScrollHelper._storyTransform.sizeDelta.x) / 2f);
				ScrollHelper._storyScrollSmall.gameObject.SetActive(true);
				ScrollHelper._expandedScroll.GetComponent<GameLineScrollItem>().SetScrollBtn(false);
				ScrollHelper.AttachExpandedScrollToStoryRoot();
				ScrollHelper._expandedScroll.localScale = new Vector3(1.27f, 1.27f, 1.27f);
				ScrollHelper.SetIllustrationBtnVisible(ScrollHelper.CanShowIllustrationBtn(index));
				ScrollHelper.RefreshStoryNavButtonsState();
			}
			else
			{
				ScrollHelper.ExpandState = false;
				ScrollHelper.SetIllustrationBtnVisible(false);
				ScrollHelper._expandedScroll.SetParent(ScrollHelper._scrollHolder);
				ScrollHelper._expandedScroll.SetSiblingIndex(index);
				ScrollHelper._expandedScroll.SetAnchor(ScrollHelper._anchorCenter, ScrollHelper._anchorCenter);
				ScrollHelper._expandedScroll.localScale = new Vector3(0.844f, 0.844f, 0.844f);
				ScrollHelper._expandedScroll.anchoredPosition = ScrollHelper._scrollOriginPos[index];
				ScrollHelper._expandedScroll.GetComponent<GameLineScrollItem>().SetScrollBtn(true);
				ScrollHelper._storyScrollSmall.gameObject.SetActive(false);
				ScrollHelper._expandedScroll = null;
				bool flag = !ScrollHelper._isInGameWorld;
				if (flag)
				{
					ScrollHelper._quickHideBanned = false;
				}
				CToggleGroup ctoggleGroup = ScrollHelper._root.CGet<CToggleGroup>("ScrollTypeToggleGroup");
				if (ctoggleGroup != null)
				{
					ctoggleGroup.gameObject.SetActive(true);
				}
				CButton cbutton = ScrollHelper._root.CGet<CButton>("ViewClose");
				if (cbutton != null)
				{
					cbutton.gameObject.SetActive(true);
				}
			}
			ScrollHelper._scrollHolderBack.color = ScrollHelper._scrollHolderBack.color.SetAlpha((float)(expand ? 0 : 1));
			for (int i = 0; i < ScrollHelper._scrollHolder.childCount; i++)
			{
				ScrollHelper._scrollHolder.GetChild(i).GetComponent<CanvasGroup>().alpha = (float)(expand ? 0 : 1);
			}
			ScrollHelper._root.CGet<GameObject>("SkipUiAni").SetActive(false);
		}

		// Token: 0x06007EC3 RID: 32451 RVA: 0x003B09D0 File Offset: 0x003AEBD0
		private static void InitStoryNavButtons()
		{
			ScrollHelper.ResolveStoryNavButtons();
			bool flag = ScrollHelper._storyLeftBtn;
			if (flag)
			{
				ScrollHelper._storyLeftBtn.ClearAndAddListener(delegate
				{
					ScrollHelper.ScrollStoryByPage(1);
				});
			}
			bool flag2 = ScrollHelper._storyRightBtn;
			if (flag2)
			{
				ScrollHelper._storyRightBtn.ClearAndAddListener(delegate
				{
					ScrollHelper.ScrollStoryByPage(-1);
				});
			}
			ScrollHelper.SetStoryNavButtonsVisible(false);
			bool flag3 = ScrollHelper._storyDragMove != null;
			if (flag3)
			{
				UIRectDragMove storyDragMove = ScrollHelper._storyDragMove;
				storyDragMove.EndDragCallback = (Action)Delegate.Remove(storyDragMove.EndDragCallback, new Action(ScrollHelper.UpdateStoryNavButtonVisibility));
				UIRectDragMove storyDragMove2 = ScrollHelper._storyDragMove;
				storyDragMove2.EndDragCallback = (Action)Delegate.Combine(storyDragMove2.EndDragCallback, new Action(ScrollHelper.UpdateStoryNavButtonVisibility));
				UIRectDragMove storyDragMove3 = ScrollHelper._storyDragMove;
				storyDragMove3.AfterClampCallback = (Action)Delegate.Remove(storyDragMove3.AfterClampCallback, new Action(ScrollHelper.UpdateStoryNavButtonVisibility));
				UIRectDragMove storyDragMove4 = ScrollHelper._storyDragMove;
				storyDragMove4.AfterClampCallback = (Action)Delegate.Combine(storyDragMove4.AfterClampCallback, new Action(ScrollHelper.UpdateStoryNavButtonVisibility));
			}
		}

		// Token: 0x06007EC4 RID: 32452 RVA: 0x003B0B08 File Offset: 0x003AED08
		private static void ResolveStoryNavButtons()
		{
			ScrollHelper._storyLeftBtn = ScrollHelper._root.CGet<CButton>("LeftBtn");
			ScrollHelper._storyRightBtn = ScrollHelper._root.CGet<CButton>("RightBtn");
			bool flag = ScrollHelper._storyLeftBtn != null && ScrollHelper._storyRightBtn != null;
			if (!flag)
			{
				RectTransform mask = ScrollHelper._root.CGet<RectTransform>("Mask");
				bool flag2 = mask == null;
				if (!flag2)
				{
					bool flag3 = ScrollHelper._storyLeftBtn == null;
					if (flag3)
					{
						Transform left = mask.Find("LeftBtn");
						bool flag4 = left != null;
						if (flag4)
						{
							ScrollHelper._storyLeftBtn = left.GetComponent<CButton>();
						}
					}
					bool flag5 = ScrollHelper._storyRightBtn == null;
					if (flag5)
					{
						Transform right = mask.Find("RightBtn");
						bool flag6 = right != null;
						if (flag6)
						{
							ScrollHelper._storyRightBtn = right.GetComponent<CButton>();
						}
					}
				}
			}
		}

		// Token: 0x06007EC5 RID: 32453 RVA: 0x003B0BF4 File Offset: 0x003AEDF4
		private static bool TryGetStoryHorizontalScrollBounds(out float minX, out float maxX)
		{
			minX = (maxX = 0f);
			bool flag = ScrollHelper._storyTransform == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				RectTransform parent = ScrollHelper._storyTransform.parent as RectTransform;
				bool flag2 = parent == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					float storyWidth = ScrollHelper._storyTransform.rect.width * ScrollHelper._storyTransform.localScale.x;
					float parentWidth = parent.rect.width;
					bool flag3 = storyWidth <= parentWidth + 1f;
					if (flag3)
					{
						result = false;
					}
					else
					{
						float edgeX = parentWidth * 0.5f;
						minX = edgeX - storyWidth * 0.5f;
						maxX = -edgeX + storyWidth * 0.5f;
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06007EC6 RID: 32454 RVA: 0x003B0CC0 File Offset: 0x003AEEC0
		private static void ScrollStoryByPage(int direction)
		{
			bool flag = ScrollHelper._storyDragMove == null || !ScrollHelper._storyRoot.activeInHierarchy;
			if (!flag)
			{
				bool flag2 = ScrollHelper._scrollAniSeq != null || ScrollHelper._storyDragMove.Dragging;
				if (!flag2)
				{
					float minX;
					float maxX;
					bool flag3 = !ScrollHelper.TryGetStoryHorizontalScrollBounds(out minX, out maxX);
					if (!flag3)
					{
						ScrollHelper._storyTransform.DOKill(false);
						Vector2 pos = ScrollHelper._storyTransform.anchoredPosition;
						float targetX = Mathf.Clamp(pos.x + (float)(direction * 1795), minX, maxX);
						bool flag4 = Mathf.Approximately(pos.x, targetX);
						if (flag4)
						{
							ScrollHelper.RefreshStoryNavButtonsState();
						}
						else
						{
							ScrollHelper._storyTransform.DOAnchorPos(pos.SetX(targetX), 0.3f, false).SetUpdate(true).OnComplete(delegate
							{
								ScrollHelper._storyDragMove.SetDirty();
								ScrollHelper.RefreshStoryNavButtonsState();
							});
						}
					}
				}
			}
		}

		// Token: 0x06007EC7 RID: 32455 RVA: 0x003B0DB8 File Offset: 0x003AEFB8
		private static void SetStoryActive(bool active)
		{
			bool flag = ScrollHelper._storyTransform == null;
			if (!flag)
			{
				ScrollHelper._storyRoot.SetActive(active);
				ScrollHelper.SetScrollUIMask(active);
				ScrollHelper.RefreshStoryNavButtonsState();
			}
		}

		// Token: 0x06007EC8 RID: 32456 RVA: 0x003B0DF0 File Offset: 0x003AEFF0
		private static void SetStoryRootCanvasAlpha(float alpha)
		{
			bool flag = ScrollHelper._storyRoot == null;
			if (!flag)
			{
				CanvasGroup canvasGroup = ScrollHelper._storyRoot.GetComponent<CanvasGroup>();
				bool flag2 = canvasGroup != null;
				if (flag2)
				{
					canvasGroup.alpha = alpha;
				}
			}
		}

		// Token: 0x06007EC9 RID: 32457 RVA: 0x003B0E30 File Offset: 0x003AF030
		private static void SetStoryNavButtonsVisible(bool visible)
		{
			bool flag = ScrollHelper._storyLeftBtn;
			if (flag)
			{
				ScrollHelper._storyLeftBtn.gameObject.SetActive(visible);
			}
			bool flag2 = ScrollHelper._storyRightBtn;
			if (flag2)
			{
				ScrollHelper._storyRightBtn.gameObject.SetActive(visible);
			}
		}

		// Token: 0x06007ECA RID: 32458 RVA: 0x003B0E7C File Offset: 0x003AF07C
		private static void RefreshStoryNavButtonsState()
		{
			ScrollHelper.ResolveStoryNavButtons();
			bool storyVisible = ScrollHelper._storyTransform != null && ScrollHelper._storyRoot.activeSelf;
			bool flag = !storyVisible || ScrollHelper._expandedScroll == null || ScrollHelper._storyLeftBtn == null || ScrollHelper._storyRightBtn == null;
			if (flag)
			{
				ScrollHelper.SetStoryNavButtonsVisible(false);
			}
			else
			{
				int pageCount = ScrollHelper.GetPageCount(ScrollHelper._expandedScroll.GetComponent<GameLineScrollItem>().Index);
				bool flag2 = pageCount <= 1;
				if (flag2)
				{
					ScrollHelper.SetStoryNavButtonsVisible(false);
				}
				else
				{
					ScrollHelper.UpdateStoryNavButtonVisibility();
				}
			}
		}

		// Token: 0x06007ECB RID: 32459 RVA: 0x003B0F14 File Offset: 0x003AF114
		private static void UpdateStoryNavButtonVisibility()
		{
			bool flag = ScrollHelper._storyLeftBtn == null || ScrollHelper._storyRightBtn == null || ScrollHelper._storyTransform == null || !ScrollHelper._storyRoot.activeSelf;
			if (!flag)
			{
				float minX;
				float maxX;
				bool flag2 = !ScrollHelper.TryGetStoryHorizontalScrollBounds(out minX, out maxX);
				if (flag2)
				{
					ScrollHelper._storyLeftBtn.gameObject.SetActive(false);
					ScrollHelper._storyRightBtn.gameObject.SetActive(false);
				}
				else
				{
					float x = ScrollHelper._storyTransform.anchoredPosition.x;
					bool showLeft = x < maxX - 1f;
					bool showRight = x > minX + 1f;
					ScrollHelper._storyLeftBtn.gameObject.SetActive(showLeft);
					ScrollHelper._storyRightBtn.gameObject.SetActive(showRight);
					bool flag3 = showLeft;
					if (flag3)
					{
						ScrollHelper._storyLeftBtn.interactable = (x < maxX - 0.5f);
					}
					bool flag4 = showRight;
					if (flag4)
					{
						ScrollHelper._storyRightBtn.interactable = (x > minX + 0.5f);
					}
				}
			}
		}

		// Token: 0x06007ECC RID: 32460 RVA: 0x003B1024 File Offset: 0x003AF224
		public static void UpdateIllustration(RectTransform scrolTransform, bool showUnlock = false, bool updateShadowTexture = true)
		{
			int index = scrolTransform.GetComponent<GameLineScrollItem>().Index;
			BossItem bossConfig = Boss.Instance[index];
			SkeletonGraphic skeletonGraphic = ScrollHelper._illustrationRefers.CGet<SkeletonGraphic>("CharSkeletonGraphic");
			int illustrationIndex = ScrollHelper.IsGoodEnding(index) ? 1 : 2;
			CRawImage shadowImg = ScrollHelper._illustrationRefers.CGet<CRawImage>("Shadow");
			if (updateShadowTexture)
			{
				string texturePath = "RemakeResources/Textures/GameLineScroll/" + bossConfig.ShadowTexture[0];
				ResLoader.Load<Texture2D>(texturePath, delegate(Texture2D texture)
				{
					shadowImg.texture = texture;
				}, null, false);
				shadowImg.rectTransform.anchoredPosition = ScrollHelper._normalShadowPos;
			}
			skeletonGraphic.gameObject.SetActive(!showUnlock);
			ScrollHelper._illustrationRefers.CGet<RectTransform>("Seal").gameObject.SetActive(showUnlock);
			shadowImg.gameObject.SetActive(showUnlock);
			bool flag = !showUnlock;
			if (flag)
			{
				string assetPath = "RemakeResources/SpineAnimations/DynamicIllustration/" + bossConfig.DynamicIllustration[illustrationIndex];
				ResLoader.Load<SkeletonDataAsset>(assetPath, delegate(SkeletonDataAsset aniData)
				{
					skeletonGraphic.skeletonDataAsset = aniData;
					skeletonGraphic.Initialize(true);
					skeletonGraphic.AnimationState.SetAnimation(0, skeletonGraphic.Skeleton.Data.Animations.Items[0], true);
				}, null, false);
			}
		}

		// Token: 0x06007ECD RID: 32461 RVA: 0x003B1154 File Offset: 0x003AF354
		public static void ShowIllustration(bool playSound = false)
		{
			int index = ScrollHelper._expandedScroll.GetComponent<GameLineScrollItem>().Index;
			int pageCount = ScrollHelper.GetPageCount(index);
			bool isRanChenZi = index >= ScrollHelper._xiangshuAvatarTaskStatuses.Length;
			bool flag = pageCount <= 4 && !isRanChenZi;
			if (!flag)
			{
				ScrollHelper.SetIllustrationBtnVisible(false);
				SkeletonGraphic[] scrollGraphics = ScrollHelper._illustrationRefers.CGet<RectTransform>("Scroll").GetComponentsInChildren<SkeletonGraphic>();
				Sequence illustrationSequence = ScrollHelper._illustrationSequence;
				if (illustrationSequence != null)
				{
					illustrationSequence.Kill(false);
				}
				ScrollHelper._illustrationSequence = DOTween.Sequence();
				ScrollHelper._illustrationRefers.gameObject.SetActive(true);
				for (int i = 0; i < scrollGraphics.Length; i++)
				{
					SkeletonGraphic graphic = scrollGraphics[i];
					graphic.color = Color.white.SetAlpha(0f);
					ScrollHelper._illustrationSequence.Join(graphic.DOFade(1f, 0.3f).SetEase(Ease.Linear)).OnComplete(delegate
					{
						Color color = graphic.color;
						color.a = 1f;
						graphic.color = color;
					});
				}
				CommandKitBase.SetDisable(true);
				ScrollHelper._storyRoot.transform.DOKill(true);
				ScrollHelper._illustrationSequence.Join(ScrollHelper._storyRoot.GetComponent<CanvasGroup>().DOFade(0f, 0.3f).SetEase(Ease.Linear)).OnComplete(delegate
				{
					CommandKitBase.SetDisable(false);
					ScrollHelper._storyRoot.GetComponent<CanvasGroup>().alpha = 0f;
					ScrollHelper._storyLeftBtn.gameObject.SetActive(false);
					ScrollHelper._storyRightBtn.gameObject.SetActive(false);
					ScrollHelper.SetQuickHideBanned(false);
				});
				ScrollHelper._illustrationSequence.Play<Sequence>();
				if (playSound)
				{
					AudioManager.Instance.PlaySound("UI_GameLineScroll_ShowScroll_Loop", true, false);
				}
			}
		}

		// Token: 0x06007ECE RID: 32462 RVA: 0x003B12F4 File Offset: 0x003AF4F4
		public static void HideIllustration(bool showButtonOnReturn = true)
		{
			bool flag = ScrollHelper._expandedScroll == null;
			if (!flag)
			{
				int index = ScrollHelper._expandedScroll.GetComponent<GameLineScrollItem>().Index;
				int pageCount = ScrollHelper.GetPageCount(index);
				SkeletonGraphic[] scrollGraphics = ScrollHelper._illustrationRefers.CGet<RectTransform>("Scroll").GetComponentsInChildren<SkeletonGraphic>();
				Sequence illustrationSequence = ScrollHelper._illustrationSequence;
				if (illustrationSequence != null)
				{
					illustrationSequence.Kill(true);
				}
				ScrollHelper._illustrationSequence = DOTween.Sequence();
				int j;
				int i;
				for (i = 0; i < scrollGraphics.Length; i = j + 1)
				{
					ScrollHelper._illustrationSequence.Join(scrollGraphics[i].DOFade(0f, 0.3f).SetEase(Ease.Linear)).OnComplete(delegate
					{
						Color color = scrollGraphics[i].color;
						color.a = 0f;
						scrollGraphics[i].color = color;
					});
					j = i;
				}
				CommandKitBase.SetDisable(true);
				ScrollHelper._storyRoot.transform.DOKill(false);
				ScrollHelper._illustrationSequence.Join(ScrollHelper._storyRoot.GetComponent<CanvasGroup>().DOFade(1f, 0.3f).SetEase(Ease.Linear)).OnComplete(delegate
				{
					ScrollHelper._illustrationRefers.gameObject.SetActive(false);
					CommandKitBase.SetDisable(false);
					ScrollHelper._storyRoot.GetComponent<CanvasGroup>().alpha = 1f;
					ScrollHelper.RefreshStoryNavButtonsState();
					bool showButtonOnReturn2 = showButtonOnReturn;
					if (showButtonOnReturn2)
					{
						ScrollHelper.SetIllustrationBtnVisible(ScrollHelper.CanShowIllustrationBtn(index));
					}
				});
				ScrollHelper._illustrationSequence.Play<Sequence>();
				AudioManager.Instance.StopSound("UI_GameLineScroll_ShowScroll_Loop");
			}
		}

		// Token: 0x06007ECF RID: 32463 RVA: 0x003B1470 File Offset: 0x003AF670
		private static bool CanShowIllustrationBtn(int index)
		{
			bool showingUnlock = ScrollHelper._showingUnlock;
			bool result;
			if (showingUnlock)
			{
				result = false;
			}
			else
			{
				bool isRanChenZi = index >= ScrollHelper._xiangshuAvatarTaskStatuses.Length;
				int pageCount = ScrollHelper.GetPageCount(index);
				result = (isRanChenZi || pageCount > 4);
			}
			return result;
		}

		// Token: 0x06007ED0 RID: 32464 RVA: 0x003B14B0 File Offset: 0x003AF6B0
		private static void SetIllustrationBtnVisible(bool visible)
		{
			bool flag = ScrollHelper._illustrationBtn != null;
			if (flag)
			{
				ScrollHelper._illustrationBtn.gameObject.SetActive(visible);
			}
		}

		// Token: 0x06007ED1 RID: 32465 RVA: 0x003B14DE File Offset: 0x003AF6DE
		private static float GetScrollExpandDuration(float width)
		{
			return width / ScrollHelper.ScrollExpandSpeed;
		}

		// Token: 0x06007ED2 RID: 32466 RVA: 0x003B14E8 File Offset: 0x003AF6E8
		private static float GetScrollSmallHideDelayInCollapse()
		{
			float expandProgressAtShow = Mathf.Clamp01(0.75f);
			return 1f * (1f - expandProgressAtShow);
		}

		// Token: 0x06007ED3 RID: 32467 RVA: 0x003B1514 File Offset: 0x003AF714
		private static void ShowUnlockIllustration()
		{
			bool flag = ScrollHelper._unlockIndex < 0 || ScrollHelper._unlockIndex >= ScrollHelper._scrollHolder.childCount;
			if (flag)
			{
				Debug.LogWarning(string.Format("[ScrollHelper] ShowUnlockIllustration: _unlockIndex={0} out of range (_scrollHolder.childCount={1})", ScrollHelper._unlockIndex, ScrollHelper._scrollHolder.childCount));
				ScrollHelper._showingUnlock = false;
				ScrollHelper._scrollHolderBack.color = ScrollHelper._scrollHolderBack.color.SetAlpha(1f);
				ScrollHelper._scrollHolder.gameObject.SetActive(true);
				ScrollHelper._illustrationRefers.gameObject.SetActive(false);
				CButton cbutton = ScrollHelper._root.CGet<CButton>("ViewClose");
				if (cbutton != null)
				{
					cbutton.gameObject.SetActive(true);
				}
				CButton cbutton2 = ScrollHelper._root.CGet<CButton>("UnlockBtn");
				if (cbutton2 != null)
				{
					cbutton2.gameObject.SetActive(false);
				}
				CToggleGroup ctoggleGroup = ScrollHelper._root.CGet<CToggleGroup>("ScrollTypeToggleGroup");
				if (ctoggleGroup != null)
				{
					ctoggleGroup.gameObject.SetActive(true);
				}
				RectTransform rectTransform = ScrollHelper._root.CGet<RectTransform>("ViewTitle");
				if (rectTransform != null)
				{
					rectTransform.gameObject.SetActive(true);
				}
				RectTransform rectTransform2 = ScrollHelper._root.CGet<RectTransform>("Notice");
				if (rectTransform2 != null)
				{
					rectTransform2.gameObject.SetActive(true);
				}
				ScrollHelper.ExpandScroll(0, true);
			}
			else
			{
				AudioManager.Instance.PlaySound("UI_GameLineScroll_Illustration_Loop", true, false);
				ScrollHelper._expandedScroll = ScrollHelper._scrollHolder.GetChild(ScrollHelper._unlockIndex).GetComponent<RectTransform>();
				ScrollHelper.UpdateIllustration(ScrollHelper._expandedScroll, true, true);
				ScrollHelper.ShowIllustration(false);
				ScrollHelper.SetStoryRootCanvasAlpha(0f);
				SkeletonGraphic[] scrollGraphics = ScrollHelper._illustrationRefers.CGet<RectTransform>("Scroll").GetComponentsInChildren<SkeletonGraphic>();
				SkeletonGraphic[] sealGraphics = ScrollHelper._illustrationRefers.CGet<RectTransform>("Seal").GetComponentsInChildren<SkeletonGraphic>();
				CRawImage shadowImg = ScrollHelper._illustrationRefers.CGet<CRawImage>("Shadow");
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(0.3f, delegate
				{
					ScrollHelper._root.CGet<GameObject>("ClickMask").SetActive(true);
				});
				Sequence seq = DOTween.Sequence();
				shadowImg.color = Color.white.SetAlpha(0f);
				for (int i = 0; i < scrollGraphics.Length; i++)
				{
					scrollGraphics[i].color = Color.white.SetAlpha(0f);
				}
				for (int j = 0; j < sealGraphics.Length; j++)
				{
					sealGraphics[j].color = Color.white.SetAlpha(0f);
				}
				seq.Append(scrollGraphics[0].DOFade(1f, 0.3f).SetEase(Ease.Linear));
				for (int k = 1; k < scrollGraphics.Length; k++)
				{
					seq.Join(scrollGraphics[k].DOFade(1f, 0.3f).SetEase(Ease.Linear));
				}
				seq.AppendInterval(0.1f);
				seq.Append(shadowImg.DOFade(1f, 0.3f).SetEase(Ease.Linear));
				for (int l = 0; l < sealGraphics.Length; l++)
				{
					seq.Join(sealGraphics[l].DOFade(1f, 0.3f).SetEase(Ease.Linear));
				}
				seq.AppendInterval(0.1f);
				seq.Play<Sequence>();
			}
		}

		// Token: 0x06007ED4 RID: 32468 RVA: 0x003B186C File Offset: 0x003AFA6C
		public static Sequence HideUnlockIllustration(Action onComplete = null)
		{
			AudioManager.Instance.StopSound("UI_GameLineScroll_Illustration_Loop");
			SkeletonGraphic[] scrollGraphics = ScrollHelper._illustrationRefers.CGet<RectTransform>("Scroll").GetComponentsInChildren<SkeletonGraphic>();
			SkeletonGraphic[] sealGraphics = ScrollHelper._illustrationRefers.CGet<RectTransform>("Seal").GetComponentsInChildren<SkeletonGraphic>();
			CRawImage shadowImg = ScrollHelper._illustrationRefers.CGet<CRawImage>("Shadow");
			Sequence seq = DOTween.Sequence();
			shadowImg.gameObject.SetActive(true);
			shadowImg.color = Color.white.SetAlpha(1f);
			for (int i = 0; i < scrollGraphics.Length; i++)
			{
				seq.Join(scrollGraphics[i].DOFade(0f, 0.3f).SetEase(Ease.Linear));
			}
			for (int j = 0; j < sealGraphics.Length; j++)
			{
				seq.Join(sealGraphics[j].DOFade(0f, 0.3f).SetEase(Ease.Linear));
			}
			seq.Join(shadowImg.DOFade(0f, 0.3f).SetEase(Ease.Linear));
			bool flag = onComplete != null;
			if (flag)
			{
				seq.AppendCallback(delegate
				{
					onComplete();
				});
			}
			return seq;
		}

		// Token: 0x06007ED5 RID: 32469 RVA: 0x003B19B8 File Offset: 0x003AFBB8
		public static void ShowUnlockScroll()
		{
			BasicGameData gameData = SingletonObject.getInstance<BasicGameData>();
			bool isRanChenZi = ScrollHelper._unlockIndex >= gameData.XiangshuAvatarTaskStatusArray.Length;
			XiangshuAvatarTaskStatus status = (!isRanChenZi) ? gameData.XiangshuAvatarTaskStatusArray[ScrollHelper._unlockIndex] : default(XiangshuAvatarTaskStatus);
			int pageCount = ScrollHelper.GetPageCount(ScrollHelper._unlockIndex);
			float moveToEndTime = 1f * (float)(pageCount - 1);
			ScrollHelper._root.CGet<GameObject>("ClickMask").SetActive(false);
			Sequence seq = DOTween.Sequence();
			for (int i = 0; i < ScrollHelper._scrollHolder.childCount; i++)
			{
				CanvasGroup scrollCanvas = ScrollHelper._scrollHolder.GetChild(i).GetComponent<CanvasGroup>();
				scrollCanvas.alpha = 0f;
				seq.Join(scrollCanvas.DOFade(1f, 0.3f).SetEase(Ease.Linear));
			}
			ScrollHelper._scrollHolder.gameObject.SetActive(true);
			seq.AppendCallback(delegate
			{
				ScrollHelper.ExpandScroll(ScrollHelper._unlockIndex, true);
				ScrollHelper.SetStoryRootCanvasAlpha(1f);
				ScrollHelper._illustrationRefers.gameObject.SetActive(false);
				ScrollHelper.SetIllustrationBtnVisible(false);
			});
			seq.AppendInterval(1f + ScrollHelper.GetScrollExpandDuration(ScrollHelper.MaxExpandWidth) + 0.2f);
			seq.AppendCallback(delegate
			{
				bool flag2 = moveToEndTime <= 0f;
				if (!flag2)
				{
					float endPosX = (ScrollHelper._storyTransform.sizeDelta.x - ScrollHelper._storyTransform.parent.GetComponent<RectTransform>().sizeDelta.x) / 2f;
					ScrollHelper._storyTransform.DOAnchorPosX(endPosX, moveToEndTime, false).SetEase(Ease.OutCubic);
				}
			});
			seq.AppendInterval(moveToEndTime + (float)(isRanChenZi ? 1 : 0));
			seq.AppendInterval(moveToEndTime + (float)(isRanChenZi ? 1 : 0));
			bool flag = status.JuniorXiangshuTaskStatus > 4 || isRanChenZi;
			if (flag)
			{
				ScrollHelper.SetQuickHideBanned(true);
				seq.AppendCallback(delegate
				{
					BossItem bossConfig = Boss.Instance[ScrollHelper._expandedScroll.GetComponent<GameLineScrollItem>().Index];
					SkeletonGraphic skeletonGraphic = ScrollHelper._illustrationRefers.CGet<SkeletonGraphic>("CharSkeletonGraphic");
					bool goodEnding = ScrollHelper.IsGoodEnding(ScrollHelper._unlockIndex);
					string particlePath = "RemakeResources/Particle/UIEffectPrefabs/Gamelinescroll/" + bossConfig.IllustrationUnlockParticle[goodEnding ? 0 : 1];
					string texturePath = "RemakeResources/Textures/GameLineScroll/" + bossConfig.ShadowTexture[goodEnding ? 1 : 2];
					CRawImage shadowImg = ScrollHelper._illustrationRefers.CGet<CRawImage>("Shadow");
					SkeletonGraphic[] scrollGraphics = ScrollHelper._illustrationRefers.CGet<RectTransform>("Scroll").GetComponentsInChildren<SkeletonGraphic>();
					ScrollHelper.UpdateIllustration(ScrollHelper._expandedScroll, false, false);
					ScrollHelper.ShowIllustration(false);
					ResLoader.Load<Texture2D>(texturePath, delegate(Texture2D texture)
					{
						shadowImg.texture = texture;
					}, null, false);
					short[] shadowPos = bossConfig.ShadowPos[goodEnding ? 0 : 1];
					shadowImg.rectTransform.anchoredPosition = new Vector2((float)shadowPos[0], (float)shadowPos[1]);
					skeletonGraphic.Skeleton.SetToSetupPose();
					skeletonGraphic.timeScale = 0f;
					skeletonGraphic.color = Color.white.SetAlpha(0f);
					for (int j = 0; j < scrollGraphics.Length; j++)
					{
						scrollGraphics[j].DOFade(1f, 0.3f).SetEase(Ease.Linear);
					}
					shadowImg.gameObject.SetActive(true);
					shadowImg.DOFade(1f, 0.3f).SetEase(Ease.Linear);
					ScrollHelper._storyRoot.transform.DOKill(false);
					ScrollHelper._storyRoot.GetComponent<CanvasGroup>().DOFade(0f, 0.3f).SetEase(Ease.Linear).OnComplete(delegate
					{
						skeletonGraphic.color = Color.white;
					});
					TweenCallback <>9__8;
					ResLoader.Load<GameObject>(particlePath, delegate(GameObject particlePrefab)
					{
						GameObject particleObj = Object.Instantiate<GameObject>(particlePrefab, ScrollHelper._illustrationRefers.transform);
						ParticleSystem particle = particleObj.GetComponent<ParticleSystem>();
						float delay = 0.94f;
						TweenCallback callback;
						if ((callback = <>9__8) == null)
						{
							callback = (<>9__8 = delegate()
							{
								shadowImg.gameObject.SetActive(false);
								skeletonGraphic.timeScale = 1f;
							});
						}
						DOVirtual.DelayedCall(delay, callback, true);
						DOVirtual.DelayedCall(2.94f, delegate
						{
							Object.Destroy(particleObj);
							ScrollHelper._showingUnlock = false;
							ScrollHelper._root.CGet<GameObject>("Close").SetActive(true);
						}, true);
						particleObj.transform.localPosition = Vector3.zero;
						particle.gameObject.SetActive(true);
						particle.Play(true);
					}, null, false);
				});
			}
			else
			{
				seq.AppendCallback(delegate
				{
					ScrollHelper._showingUnlock = false;
					ScrollHelper._root.CGet<GameObject>("Close").SetActive(true);
					ScrollHelper._root.CGet<GameObject>("ClickMask").SetActive(false);
				});
			}
			seq.AppendCallback(delegate
			{
				bool flag2 = !ScrollHelper._illustrationRefers.gameObject.activeSelf;
				if (flag2)
				{
					ScrollHelper.SetIllustrationBtnVisible(ScrollHelper.CanShowIllustrationBtn(ScrollHelper._unlockIndex));
				}
			});
			seq.Play<Sequence>();
		}

		// Token: 0x06007ED6 RID: 32470 RVA: 0x003B1BD4 File Offset: 0x003AFDD4
		private static void UnlockStory()
		{
			Sequence seq = ScrollHelper.HideUnlockIllustration(new Action(ScrollHelper.ShowUnlockScroll));
			seq.Play<Sequence>();
		}

		// Token: 0x06007ED7 RID: 32471 RVA: 0x003B1BFB File Offset: 0x003AFDFB
		public static void OnSaveFileDeleted()
		{
			ScrollHelper.OnOnRecordRootToggleChanged(ScrollHelper._root, -1);
		}

		// Token: 0x06007ED8 RID: 32472 RVA: 0x003B1C0A File Offset: 0x003AFE0A
		public static void SetUIElement(UIElement uiElement)
		{
			ScrollHelper._uiElement = uiElement;
		}

		// Token: 0x06007ED9 RID: 32473 RVA: 0x003B1C14 File Offset: 0x003AFE14
		public static void OnOnRecordRootToggleChanged(Refers root, int key)
		{
			bool flag = ScrollHelper._illustrationRefers != null;
			if (flag)
			{
				bool flag2 = ScrollHelper._scrollAniSeq != null && ScrollHelper._scrollAniSeq.IsActive();
				if (flag2)
				{
					ScrollHelper._scrollAniSeq.Complete(true);
				}
				ScrollHelper.HideScroll(true);
				bool flag3 = ScrollHelper._scrollAniSeq != null && ScrollHelper._scrollAniSeq.IsActive();
				if (flag3)
				{
					ScrollHelper._scrollAniSeq.Complete(true);
				}
				CommandKitBase.SetDisable(false);
				ScrollHelper._quickHideBanned = false;
			}
			bool flag4 = GlobalOperations.ArchivesInfo != null;
			if (flag4)
			{
				ScrollHelper.OnInit(root, -1, false, key, false, 2);
			}
		}

		// Token: 0x06007EDA RID: 32474 RVA: 0x003B1CA8 File Offset: 0x003AFEA8
		public static void OnClick(Transform btn)
		{
			string text = (btn != null) ? btn.name : null;
			string text2 = text;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
			if (num <= 1494262648U)
			{
				if (num != 815638986U)
				{
					if (num != 1334377395U)
					{
						if (num == 1494262648U)
						{
							if (text2 == "LeftBtn")
							{
								ScrollHelper.ScrollStoryByPage(1);
							}
						}
					}
					else if (!(text2 == "UnlockIllustration"))
					{
					}
				}
				else if (text2 == "ShowIllustration")
				{
					ScrollHelper.ShowIllustration(false);
				}
			}
			else if (num <= 3133146507U)
			{
				if (num != 2934913870U)
				{
					if (num == 3133146507U)
					{
						if (text2 == "CloseButton")
						{
							ScrollHelper.HideScroll(true);
							ScrollHelper._scrollAniSeq.Complete(true);
						}
					}
				}
				else if (text2 == "SkipUiAni")
				{
					bool isSkipEnabled = ScrollHelper._root.CGet<LeftMouseSkipAnimNotice>("LeftMouseSkipAnimNotice").IsSkipEnabled;
					if (isSkipEnabled)
					{
						ScrollHelper.SetToAniEndState(ScrollHelper._expanding);
					}
				}
			}
			else if (num != 3448155331U)
			{
				if (num == 3536430647U)
				{
					if (text2 == "RightBtn")
					{
						ScrollHelper.ScrollStoryByPage(-1);
					}
				}
			}
			else if (text2 == "Close")
			{
				ScrollHelper.HideScroll(false);
			}
		}

		// Token: 0x06007EDB RID: 32475 RVA: 0x003B1E04 File Offset: 0x003B0004
		public static bool GetQuickHideBanned()
		{
			return ScrollHelper._quickHideBanned;
		}

		// Token: 0x06007EDC RID: 32476 RVA: 0x003B1E1B File Offset: 0x003B001B
		public static void SetQuickHideBanned(bool val)
		{
			ScrollHelper._quickHideBanned = val;
		}

		// Token: 0x06007EDD RID: 32477 RVA: 0x003B1E24 File Offset: 0x003B0024
		private static int GetPageCount(int index)
		{
			bool isRanChenZi = index >= ScrollHelper._xiangshuAvatarTaskStatuses.Length;
			return (!isRanChenZi) ? Mathf.Min((int)ScrollHelper._xiangshuAvatarTaskStatuses[index].JuniorXiangshuTaskStatus, 5) : 1;
		}

		// Token: 0x06007EDE RID: 32478 RVA: 0x003B1E60 File Offset: 0x003B0060
		private static bool IsGoodEnding(int index)
		{
			bool isRanChenZi = index >= ScrollHelper._xiangshuAvatarTaskStatuses.Length;
			return (!isRanChenZi) ? (ScrollHelper._xiangshuAvatarTaskStatuses[index].JuniorXiangshuTaskStatus == 6) : ScrollHelper._beatRanChenZi;
		}

		// Token: 0x06007EDF RID: 32479 RVA: 0x003B1E9D File Offset: 0x003B009D
		private static void ChangeScrollScale(GameLineScrollItem scrollItem, bool isExpand = true)
		{
			scrollItem.transform.localScale = (isExpand ? new Vector3(1.27f, 1.27f, 1.27f) : new Vector3(0.844f, 0.844f, 0.844f));
		}

		// Token: 0x040060BE RID: 24766
		private const string TextureDir = "RemakeResources/Textures/GameLineScroll/";

		// Token: 0x040060BF RID: 24767
		private const string IllustrationDir = "RemakeResources/SpineAnimations/DynamicIllustration/";

		// Token: 0x040060C0 RID: 24768
		private const string ParticleDir = "RemakeResources/Particle/UIEffectPrefabs/Gamelinescroll/";

		// Token: 0x040060C1 RID: 24769
		private const float ScrollExpandMoveTime = 1f;

		// Token: 0x040060C2 RID: 24770
		private const float ScrollSmallExpandMoveTime = 0.8f;

		// Token: 0x040060C3 RID: 24771
		private const float ScrollExpandTime = 1f;

		// Token: 0x040060C4 RID: 24772
		private const float ScrollSmallCollapseMoveTime = 1.3f;

		// Token: 0x040060C5 RID: 24773
		private const float ScrollSmallShowTime = 1.75f;

		// Token: 0x040060C6 RID: 24774
		private const float StoryPageScrollDuration = 0.3f;

		// Token: 0x040060C7 RID: 24775
		private const short PageWidth = 1795;

		// Token: 0x040060C8 RID: 24776
		private const short StoryWidthAdd = 45;

		// Token: 0x040060C9 RID: 24777
		private const short MultiplePageExpandAdd = 500;

		// Token: 0x040060CA RID: 24778
		private static readonly float MaxExpandWidth = 2340f;

		// Token: 0x040060CB RID: 24779
		private static readonly float ScrollExpandSpeed = ScrollHelper.MaxExpandWidth / 1f;

		// Token: 0x040060CC RID: 24780
		private const short ScrollSmallOriginPosX = 1530;

		// Token: 0x040060CD RID: 24781
		private static readonly Vector2[] _scrollOriginPos = new Vector2[10];

		// Token: 0x040060CE RID: 24782
		private static readonly Vector2 _normalShadowPos = new Vector2(20f, 190f);

		// Token: 0x040060CF RID: 24783
		private static readonly Vector2 _anchorCenter = new Vector2(0.5f, 0.5f);

		// Token: 0x040060D0 RID: 24784
		private static readonly Vector2 _anchorRight = new Vector2(1f, 0.5f);

		// Token: 0x040060D1 RID: 24785
		private static int _unlockIndex;

		// Token: 0x040060D2 RID: 24786
		private static bool _showingUnlock;

		// Token: 0x040060D3 RID: 24787
		private static bool _isUnlockScroll;

		// Token: 0x040060D4 RID: 24788
		private static RectTransform _scrollHolder;

		// Token: 0x040060D5 RID: 24789
		private static CRawImage _scrollHolderBack;

		// Token: 0x040060D6 RID: 24790
		private static RectTransform _expandedScroll;

		// Token: 0x040060D7 RID: 24791
		private static RectTransform _storyTransform;

		// Token: 0x040060D8 RID: 24792
		private static GameObject _storyRoot;

		// Token: 0x040060D9 RID: 24793
		private static RectTransform _storyScrollSmall;

		// Token: 0x040060DA RID: 24794
		private static GameObject _uiMaskNode;

		// Token: 0x040060DB RID: 24795
		private static UIRectDragMove _storyDragMove;

		// Token: 0x040060DC RID: 24796
		private static CButton _storyLeftBtn;

		// Token: 0x040060DD RID: 24797
		private static CButton _storyRightBtn;

		// Token: 0x040060DE RID: 24798
		private static Refers _illustrationRefers;

		// Token: 0x040060DF RID: 24799
		private static CButton _illustrationBtn;

		// Token: 0x040060E0 RID: 24800
		private static Sequence _scrollAniSeq;

		// Token: 0x040060E1 RID: 24801
		private static bool _expanding;

		// Token: 0x040060E2 RID: 24802
		private static Refers _root;

		// Token: 0x040060E3 RID: 24803
		private static bool _isInGameWorld;

		// Token: 0x040060E4 RID: 24804
		private static XiangshuAvatarTaskStatus[] _xiangshuAvatarTaskStatuses;

		// Token: 0x040060E5 RID: 24805
		private static bool _beatRanChenZi;

		// Token: 0x040060E6 RID: 24806
		private static bool _mainStoryScroll;

		// Token: 0x040060E7 RID: 24807
		public static bool _quickHideBanned = false;

		// Token: 0x040060E8 RID: 24808
		private static HashSet<Refers> _initedRoots = new HashSet<Refers>();

		// Token: 0x040060E9 RID: 24809
		public static bool ExpandState;

		// Token: 0x040060EA RID: 24810
		private static Sequence _illustrationSequence;

		// Token: 0x040060EB RID: 24811
		private static UIElement _uiElement;

		// Token: 0x02001FA0 RID: 8096
		private static class BossIllustrationIndex
		{
			// Token: 0x0400CE18 RID: 52760
			public const sbyte Normal = 0;

			// Token: 0x0400CE19 RID: 52761
			public const sbyte Joy = 1;

			// Token: 0x0400CE1A RID: 52762
			public const sbyte Sad = 2;

			// Token: 0x0400CE1B RID: 52763
			public const sbyte KidNormal = 3;

			// Token: 0x0400CE1C RID: 52764
			public const sbyte KidJoy = 4;

			// Token: 0x0400CE1D RID: 52765
			public const sbyte KidSad = 5;
		}

		// Token: 0x02001FA1 RID: 8097
		private static class BossUnlockEffectIndex
		{
			// Token: 0x0400CE1E RID: 52766
			public const sbyte Joy = 0;

			// Token: 0x0400CE1F RID: 52767
			public const sbyte Sad = 1;

			// Token: 0x0400CE20 RID: 52768
			public const sbyte KidJoy = 2;

			// Token: 0x0400CE21 RID: 52769
			public const sbyte KidSad = 3;
		}
	}
}
