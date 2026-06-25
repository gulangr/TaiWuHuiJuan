using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork.UI.LanguageRule;
using GameData.Domains.Global;
using GameData.Domains.World;
using GameData.GameDataBridge;
using Spine.Unity;
using TMPro;
using UnityEngine;

// Token: 0x020002FA RID: 762
public static class ScrollHelper
{
	// Token: 0x06002C9B RID: 11419 RVA: 0x0015E0E8 File Offset: 0x0015C2E8
	public static void OnInit(Refers root, int unlockIndex, bool isInGameWorld, int currSaveSlot)
	{
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
		ScrollHelper._root.CGet<GameObject>("ShowIllustration").SetActive(false);
		bool flag3 = !ScrollHelper._initedRoots.Contains(ScrollHelper._root);
		if (flag3)
		{
			ScrollHelper.Init();
		}
		for (int i = 0; i < ScrollHelper._scrollHolder.childCount; i++)
		{
			Refers scrollRefers = ScrollHelper._scrollHolder.GetChild(i).GetComponent<Refers>();
			bool unlocked = i < ScrollHelper._xiangshuAvatarTaskStatuses.Length && ScrollHelper._xiangshuAvatarTaskStatuses[i].JuniorXiangshuTaskStatus != 0;
			scrollRefers.GetComponent<CImage>().SetSprite((!unlocked) ? "ui_scroll_bamboo_roller_2" : ((i < 9) ? "ui_scroll_bamboo_roller_0" : "ui_scroll_bamboo_roller_1"), false, null);
			scrollRefers.GetComponent<CButtonObsolete>().interactable = unlocked;
			scrollRefers.GetComponent<PointerTrigger>().enabled = unlocked;
			scrollRefers.CGet<TextMeshProUGUI>("Name").gameObject.SetActive(unlocked);
			scrollRefers.CGet<GameObject>("NameUnknown").SetActive(!unlocked);
			scrollRefers.CGet<GameObject>("NameImg").SetActive(unlocked);
			scrollRefers.CGet<GameObject>("NameImgUnknown").SetActive(!unlocked);
			scrollRefers.CGet<GameObject>("Logo").SetActive(unlocked);
			scrollRefers.CGet<GameObject>("Decorate").SetActive(unlocked);
		}
		ScrollHelper._showingUnlock = (ScrollHelper._unlockIndex >= 0);
		ScrollHelper._scrollHolderBack.SetAlpha((float)(ScrollHelper._showingUnlock ? 0 : 1));
		ScrollHelper._scrollHolder.gameObject.SetActive(!ScrollHelper._showingUnlock);
		ScrollHelper._illustrationRefers.gameObject.SetActive(ScrollHelper._showingUnlock);
		bool showingUnlock = ScrollHelper._showingUnlock;
		if (showingUnlock)
		{
			ScrollHelper.ShowUnlockIllustration();
		}
	}

	// Token: 0x06002C9C RID: 11420 RVA: 0x0015E34C File Offset: 0x0015C54C
	public static void Init()
	{
		ScrollHelper._scrollHolder = ScrollHelper._root.CGet<RectTransform>("ScrollHolder");
		ScrollHelper._scrollHolderBack = ScrollHelper._root.CGet<CImage>("ScrollBack");
		ScrollHelper._storyTransform = ScrollHelper._root.CGet<RectTransform>("Story");
		ScrollHelper._illustrationRefers = ScrollHelper._root.CGet<Refers>("Illustration");
		for (int i = 0; i < ScrollHelper._scrollHolder.childCount; i++)
		{
			int index = i;
			Refers scrollRefers = ScrollHelper._scrollHolder.GetChild(i).GetComponent<Refers>();
			PointerTrigger scrollPointerTrigger = scrollRefers.GetComponent<PointerTrigger>();
			ParticleSystem highLight = scrollRefers.CGet<ParticleSystem>("HighLight");
			GameObject highLightHolder;
			scrollRefers.CTryGet<GameObject>("HighLightHolder", out highLightHolder);
			CharacterItem charConfig = Character.Instance[Boss.Instance[(int)scrollRefers.UserFloat].CharacterIdList[0]];
			ScrollHelper._scrollOriginPos[i] = scrollRefers.GetComponent<RectTransform>().anchoredPosition;
			scrollRefers.CGet<TextMeshProUGUI>("Name").text = (charConfig.Surname ?? string.Empty) + charConfig.GivenName;
			scrollRefers.GetComponent<CButtonObsolete>().ClearAndAddListener(delegate
			{
				ScrollHelper.ExpandScroll(index, true);
			});
			scrollPointerTrigger.EnterEvent.AddListener(delegate()
			{
				bool flag = highLightHolder != null;
				if (flag)
				{
					highLightHolder.SetActive(true);
				}
				highLight.gameObject.SetActive(true);
				highLight.Simulate(3f);
				highLight.Play();
			});
			scrollPointerTrigger.ExitEvent.AddListener(delegate()
			{
				bool flag = highLightHolder != null;
				if (flag)
				{
					highLightHolder.SetActive(false);
				}
				highLight.gameObject.SetActive(false);
			});
		}
		PointerTrigger btnPointerTrigger = ScrollHelper._isInGameWorld ? ScrollHelper._illustrationRefers.CGet<CButtonObsolete>("UnlockIllustration").GetComponent<PointerTrigger>() : ScrollHelper._illustrationRefers.CGet<GameObject>("UnlockIllustration").GetComponent<PointerTrigger>();
		ParticleSystem btnLight = ScrollHelper._illustrationRefers.CGet<ParticleSystem>("UnlockBtnLight");
		btnPointerTrigger.EnterEvent.AddListener(delegate()
		{
			btnLight.gameObject.SetActive(true);
		});
		btnPointerTrigger.ExitEvent.AddListener(delegate()
		{
			btnLight.gameObject.SetActive(false);
		});
		ScrollHelper._initedRoots.Add(ScrollHelper._root);
	}

	// Token: 0x06002C9D RID: 11421 RVA: 0x0015E55C File Offset: 0x0015C75C
	public static void Update()
	{
		bool flag = !ScrollHelper._storyTransform.gameObject.activeSelf || ScrollHelper._scrollAniSeq != null;
		if (!flag)
		{
			RectTransform storyParent = ScrollHelper._storyTransform.parent as RectTransform;
			float maskHalfWidth = ScrollHelper._root.CGet<RectTransform>("Mask").sizeDelta.x / 2f;
			float halfWidth = ScrollHelper._storyTransform.sizeDelta.x / 2f;
			ScrollHelper._root.CGet<CImage>("BorderLeft").SetAlpha((storyParent.anchoredPosition.x + ScrollHelper._storyTransform.anchoredPosition.x - halfWidth - 32f < -maskHalfWidth) ? 1f : 0.3f);
			ScrollHelper._root.CGet<CImage>("BorderRight").SetAlpha((storyParent.anchoredPosition.x + ScrollHelper._storyTransform.anchoredPosition.x + halfWidth + 68f > maskHalfWidth) ? 1f : 0.3f);
		}
	}

	// Token: 0x06002C9E RID: 11422 RVA: 0x0015E664 File Offset: 0x0015C864
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

	// Token: 0x06002C9F RID: 11423 RVA: 0x0015E758 File Offset: 0x0015C958
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
				ScrollHelper.HideIllustration(false);
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

	// Token: 0x06002CA0 RID: 11424 RVA: 0x0015E7D8 File Offset: 0x0015C9D8
	public static void ExpandScroll(int index, bool doAni)
	{
		bool flag = !ScrollHelper._isInGameWorld;
		if (flag)
		{
			ScrollHelper._quickHideBanned = true;
		}
		ScrollHelper._root.CGet<GameObject>("Close").SetActive(true);
		ScrollHelper._expandedScroll = ScrollHelper._scrollHolder.GetChild(index).GetComponent<RectTransform>();
		bool isInGameWorld = ScrollHelper._isInGameWorld;
		if (isInGameWorld)
		{
			ScrollHelper.ChangeScrollScale(ScrollHelper._expandedScroll.GetComponent<Refers>(), true);
		}
		bool isRanChenZi = index >= ScrollHelper._xiangshuAvatarTaskStatuses.Length;
		int pageCount = ScrollHelper.GetPageCount(index);
		RectTransform pageHolder = ScrollHelper._root.CGet<RectTransform>("PageHolder");
		ScrollHelper._expanding = true;
		bool isInGameWorld2 = ScrollHelper._isInGameWorld;
		if (isInGameWorld2)
		{
			CImage boderLeft = ScrollHelper._root.CGet<CImage>("BorderLeft");
			CImage boderRight = ScrollHelper._root.CGet<CImage>("BorderRight");
			boderLeft.SetAlpha(0.3f);
			boderLeft.gameObject.SetActive(pageCount > 1);
			boderRight.SetAlpha(0.3f);
			boderRight.gameObject.SetActive(pageCount > 1);
		}
		ScrollHelper._root.CGet<GameObject>("ShowIllustration").SetActive(!ScrollHelper._showingUnlock && (isRanChenZi || pageCount > 4));
		int i = 0;
		while (i < pageHolder.childCount)
		{
			CRawImage rawImg = pageHolder.GetChild(i).GetComponent<CRawImage>();
			bool showPage = i < pageCount;
			rawImg.gameObject.SetActive(showPage);
			bool flag2 = showPage;
			if (flag2)
			{
				bool flag3 = !isRanChenZi;
				int indexOffset;
				if (flag3)
				{
					indexOffset = ((i < 4) ? i : (ScrollHelper.IsGoodEnding(index) ? 4 : 5));
				}
				else
				{
					indexOffset = (ScrollHelper._beatRanChenZi ? 0 : 1);
				}
				short scrollTemplateId = (short)(index * 6 + indexOffset);
				StoryScrollItem scrollConfig = StoryScroll.Instance.GetItem(scrollTemplateId);
				bool flag4 = scrollConfig == null;
				if (!flag4)
				{
					string texturePath = "RemakeResources/Textures/GameLineScroll/" + scrollConfig.StoryImage;
					ResLoader.Load<Texture2D>(texturePath, delegate(Texture2D texture)
					{
						rawImg.texture = texture;
					}, null, false);
					Refers noteRefers = rawImg.GetComponentInChildren<Refers>();
					bool flag5 = null != noteRefers;
					if (flag5)
					{
						noteRefers.CGet<TextMeshProUGUI>("Note").text = scrollConfig.StoryNote;
						noteRefers.CGet<TextMeshProUGUI>("ResultMark").color = Colors.Instance["scrollchapter"];
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
			IL_2F1:
			i++;
			continue;
			goto IL_2F1;
		}
		bool flag6 = !doAni;
		if (flag6)
		{
			ScrollHelper.SetToAniEndState(true);
		}
		else
		{
			bool flag7 = pageCount > 4 || isRanChenZi;
			if (flag7)
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
			ScrollHelper._storyTransform.gameObject.SetActive(true);
			ScrollHelper._storyTransform.anchoredPosition = new Vector2(ScrollHelper._storyTransform.anchoredPosition.x, 0f);
			ScrollHelper._storyTransform.GetComponent<CanvasGroup>().alpha = 1f;
			ScrollHelper._expandedScroll.GetComponent<CButtonObsolete>().interactable = false;
			ScrollHelper._expandedScroll.GetComponent<PointerTrigger>().enabled = false;
			ScrollHelper._expandedScroll.SetParent(ScrollHelper._storyTransform);
			ScrollHelper._expandedScroll.SetAnchor(ScrollHelper._anchorRight, ScrollHelper._anchorRight);
			ScrollHelper._expandedScroll.anchoredPosition = Vector2.zero;
			ScrollHelper._scrollAniSeq.Append(ScrollHelper._storyTransform.DOAnchorPosX(0f, 1f, false));
			ScrollHelper._scrollAniSeq.Join(ScrollHelper._scrollHolderBack.DOFade(0f, 1f));
			for (int j = 0; j < ScrollHelper._scrollHolder.childCount; j++)
			{
				ScrollHelper._scrollAniSeq.Join(ScrollHelper._scrollHolder.GetChild(j).GetComponent<CanvasGroup>().DOFade(0f, 1f));
			}
			ScrollHelper._scrollAniSeq.AppendCallback(delegate
			{
				ScrollHelper._scrollHolder.gameObject.SetActive(false);
			});
			float storyWidth = (float)(1795 * pageCount + 45);
			float expandWidth = (float)(1840 + ((pageCount > 1) ? 500 : 0));
			ScrollHelper._scrollAniSeq.Append(ScrollHelper._storyTransform.DOSizeDelta(ScrollHelper._storyTransform.sizeDelta.SetX(expandWidth), 1f, false));
			ScrollHelper._scrollAniSeq.AppendCallback(delegate
			{
				ScrollHelper._storyTransform.sizeDelta = ScrollHelper._storyTransform.sizeDelta.SetX(storyWidth);
				ScrollHelper._storyTransform.SetPivot(ScrollHelper._anchorCenter);
				ScrollHelper._storyTransform.SetAnchor(ScrollHelper._anchorCenter, ScrollHelper._anchorCenter);
				ScrollHelper._scrollAniSeq = null;
				skipUiAni.SetActive(false);
			});
			ScrollHelper._scrollAniSeq.Play<Sequence>();
			skipUiAni.SetActive(true);
			AudioManager.Instance.PlaySound("UI_GameLineScroll_OpenScroll", false, false);
			ScrollHelper._scrollAniSeq.OnComplete(delegate
			{
				ScrollHelper.ExpandState = true;
			});
		}
	}

	// Token: 0x06002CA1 RID: 11425 RVA: 0x0015EDCC File Offset: 0x0015CFCC
	public static void CollapseScroll(bool doAni)
	{
		ScrollHelper.ExpandState = false;
		ScrollHelper._expanding = false;
		ScrollHelper._root.CGet<GameObject>("ShowIllustration").SetActive(false);
		bool flag = !doAni;
		if (flag)
		{
			ScrollHelper.SetToAniEndState(false);
		}
		else
		{
			int index = ScrollHelper._expandedScroll.GetComponent<Refers>().UserInt;
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
			ScrollHelper._scrollAniSeq.Append(ScrollHelper._storyTransform.DOAnchorPosX(0f, 0.1f * ScrollHelper._storyTransform.anchoredPosition.x / 1000f, false));
			float expandWidth = (float)(1840 + ((pageCount > 1) ? 500 : 0));
			ScrollHelper._scrollAniSeq.AppendCallback(delegate
			{
				ScrollHelper._storyTransform.sizeDelta = ScrollHelper._storyTransform.sizeDelta.SetX(expandWidth);
			});
			ScrollHelper._scrollAniSeq.Append(ScrollHelper._storyTransform.DOSizeDelta(ScrollHelper._storyTransform.sizeDelta.SetX(0f), 1f, false));
			UI_GameLineScroll gameLineScroll = UIElement.GameLineScroll.UiBaseAs<UI_GameLineScroll>();
			bool flag2 = gameLineScroll != null;
			if (flag2)
			{
				GameObject UIMask = gameLineScroll.CGet<GameObject>("UIMask");
				UIMask.transform.SetParent(ScrollHelper._root.transform.parent);
				UIMask.transform.SetSiblingIndex(0);
			}
			ScrollHelper._scrollAniSeq.AppendCallback(delegate
			{
				bool isInGameWorld = ScrollHelper._isInGameWorld;
				if (isInGameWorld)
				{
					ScrollHelper._root.CGet<CImage>("BorderLeft").gameObject.SetActive(false);
					ScrollHelper._root.CGet<CImage>("BorderRight").gameObject.SetActive(false);
				}
				ScrollHelper._storyTransform.gameObject.SetActive(false);
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
				ScrollHelper._expandedScroll.GetComponent<CButtonObsolete>().interactable = true;
				ScrollHelper._expandedScroll.GetComponent<PointerTrigger>().enabled = true;
				bool isInGameWorld = ScrollHelper._isInGameWorld;
				if (isInGameWorld)
				{
					ScrollHelper.ChangeScrollScale(ScrollHelper._expandedScroll.GetComponent<Refers>(), false);
				}
				ScrollHelper._expandedScroll = null;
				ScrollHelper._scrollAniSeq = null;
				skipUiAni.SetActive(false);
				bool flag3 = !ScrollHelper._isInGameWorld;
				if (flag3)
				{
					ScrollHelper._quickHideBanned = false;
				}
			});
			skipUiAni.SetActive(true);
			AudioManager.Instance.PlaySound("UI_GameLineScroll_CloseScroll", false, false);
		}
	}

	// Token: 0x06002CA2 RID: 11426 RVA: 0x0015F078 File Offset: 0x0015D278
	public static void SetToAniEndState(bool expand)
	{
		int index = ScrollHelper._expandedScroll.GetComponent<Refers>().UserInt;
		int pageCount = ScrollHelper.GetPageCount(index);
		Sequence scrollAniSeq = ScrollHelper._scrollAniSeq;
		if (scrollAniSeq != null)
		{
			scrollAniSeq.Kill(false);
		}
		ScrollHelper._scrollAniSeq = null;
		CommandKitBase.SetDisable(false);
		ScrollHelper._storyTransform.gameObject.SetActive(expand);
		ScrollHelper._scrollHolder.gameObject.SetActive(!expand);
		bool isInGameWorld = ScrollHelper._isInGameWorld;
		if (isInGameWorld)
		{
			ScrollHelper._root.CGet<CImage>("BorderLeft").gameObject.SetActive(expand && pageCount > 1);
			ScrollHelper._root.CGet<CImage>("BorderRight").gameObject.SetActive(expand && pageCount > 1);
		}
		if (expand)
		{
			ScrollHelper.ExpandState = true;
			float storyWidth = (float)(1795 * pageCount + 45);
			ScrollHelper._storyTransform.SetPivot(ScrollHelper._anchorCenter);
			ScrollHelper._storyTransform.SetAnchor(ScrollHelper._anchorCenter, ScrollHelper._anchorCenter);
			ScrollHelper._storyTransform.sizeDelta = ScrollHelper._storyTransform.sizeDelta.SetX(storyWidth);
			ScrollHelper._storyTransform.anchoredPosition = ScrollHelper._storyTransform.anchoredPosition.SetX((ScrollHelper._storyTransform.parent.GetComponent<RectTransform>().sizeDelta.x - ScrollHelper._storyTransform.sizeDelta.x) / 2f);
			ScrollHelper._expandedScroll.GetComponent<CButtonObsolete>().interactable = false;
			ScrollHelper._expandedScroll.GetComponent<PointerTrigger>().enabled = false;
			ScrollHelper._expandedScroll.SetParent(ScrollHelper._storyTransform);
			ScrollHelper._expandedScroll.SetAnchor(ScrollHelper._anchorRight, ScrollHelper._anchorRight);
			ScrollHelper._expandedScroll.localScale = Vector3.one;
			ScrollHelper._expandedScroll.anchoredPosition = Vector2.zero;
		}
		else
		{
			ScrollHelper.ExpandState = false;
			ScrollHelper._expandedScroll.SetParent(ScrollHelper._scrollHolder);
			ScrollHelper._expandedScroll.SetSiblingIndex(index);
			ScrollHelper._expandedScroll.SetAnchor(ScrollHelper._anchorCenter, ScrollHelper._anchorCenter);
			bool isInGameWorld2 = ScrollHelper._isInGameWorld;
			if (isInGameWorld2)
			{
				ScrollHelper._expandedScroll.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			}
			ScrollHelper._expandedScroll.anchoredPosition = ScrollHelper._scrollOriginPos[index];
			ScrollHelper._expandedScroll.GetComponent<CButtonObsolete>().interactable = true;
			ScrollHelper._expandedScroll.GetComponent<PointerTrigger>().enabled = true;
			ScrollHelper._expandedScroll = null;
			bool flag = !ScrollHelper._isInGameWorld;
			if (flag)
			{
				ScrollHelper._quickHideBanned = false;
			}
		}
		ScrollHelper._scrollHolderBack.SetAlpha((float)(expand ? 0 : 1));
		for (int i = 0; i < ScrollHelper._scrollHolder.childCount; i++)
		{
			ScrollHelper._scrollHolder.GetChild(i).GetComponent<CanvasGroup>().alpha = (float)(expand ? 0 : 1);
		}
		ScrollHelper._root.CGet<GameObject>("SkipUiAni").SetActive(false);
	}

	// Token: 0x06002CA3 RID: 11427 RVA: 0x0015F368 File Offset: 0x0015D568
	public static void UpdateIllustration(RectTransform scrolTransform, bool showUnlock = false, bool updateShadowTexture = true)
	{
		int index = scrolTransform.GetComponent<Refers>().UserInt;
		BossItem bossConfig = Boss.Instance[(int)scrolTransform.GetComponent<Refers>().UserFloat];
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
		bool isInGameWorld = ScrollHelper._isInGameWorld;
		if (isInGameWorld)
		{
			CButtonObsolete unlockBtn = ScrollHelper._illustrationRefers.CGet<CButtonObsolete>("UnlockIllustration");
			unlockBtn.interactable = true;
			unlockBtn.gameObject.SetActive(showUnlock);
		}
		else
		{
			ScrollHelper._illustrationRefers.CGet<GameObject>("UnlockIllustration").SetActive(showUnlock);
		}
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

	// Token: 0x06002CA4 RID: 11428 RVA: 0x0015F4F4 File Offset: 0x0015D6F4
	public static void ShowIllustration(bool playSound = false)
	{
		int index = ScrollHelper._expandedScroll.GetComponent<Refers>().UserInt;
		int pageCount = ScrollHelper.GetPageCount(index);
		bool isRanChenZi = index >= ScrollHelper._xiangshuAvatarTaskStatuses.Length;
		bool flag = pageCount <= 4 && !isRanChenZi;
		if (!flag)
		{
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
			ScrollHelper._storyTransform.DOKill(true);
			ScrollHelper._illustrationSequence.Join(ScrollHelper._storyTransform.GetComponent<CanvasGroup>().DOFade(0f, 0.3f).SetEase(Ease.Linear)).OnComplete(delegate
			{
				CommandKitBase.SetDisable(false);
				ScrollHelper._storyTransform.GetComponent<CanvasGroup>().alpha = 0f;
			});
			bool isInGameWorld = ScrollHelper._isInGameWorld;
			if (isInGameWorld)
			{
				ScrollHelper._root.CGet<CImage>("BorderLeft").gameObject.SetActive(false);
				ScrollHelper._root.CGet<CImage>("BorderRight").gameObject.SetActive(false);
			}
			ScrollHelper._root.CGet<GameObject>("ShowIllustration").SetActive(false);
			ScrollHelper._illustrationSequence.Play<Sequence>();
			if (playSound)
			{
				AudioManager.Instance.PlaySound("UI_GameLineScroll_ShowScroll_Loop", true, false);
			}
		}
	}

	// Token: 0x06002CA5 RID: 11429 RVA: 0x0015F6E0 File Offset: 0x0015D8E0
	public static void HideIllustration(bool showButtonOnReturn = true)
	{
		bool flag = ScrollHelper._expandedScroll == null;
		if (!flag)
		{
			int index = ScrollHelper._expandedScroll.GetComponent<Refers>().UserInt;
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
			ScrollHelper._storyTransform.DOKill(false);
			ScrollHelper._illustrationSequence.Join(ScrollHelper._storyTransform.GetComponent<CanvasGroup>().DOFade(1f, 0.3f).SetEase(Ease.Linear)).OnComplete(delegate
			{
				ScrollHelper._illustrationRefers.gameObject.SetActive(false);
				bool showButtonOnReturn2 = showButtonOnReturn;
				if (showButtonOnReturn2)
				{
					ScrollHelper._root.CGet<GameObject>("ShowIllustration").SetActive(true);
				}
				CommandKitBase.SetDisable(false);
				ScrollHelper._storyTransform.GetComponent<CanvasGroup>().alpha = 1f;
			});
			bool isInGameWorld = ScrollHelper._isInGameWorld;
			if (isInGameWorld)
			{
				ScrollHelper._root.CGet<CImage>("BorderLeft").gameObject.SetActive(pageCount > 1);
				ScrollHelper._root.CGet<CImage>("BorderRight").gameObject.SetActive(pageCount > 1);
			}
			ScrollHelper._illustrationSequence.Play<Sequence>();
			AudioManager.Instance.StopSound("UI_GameLineScroll_ShowScroll_Loop");
		}
	}

	// Token: 0x06002CA6 RID: 11430 RVA: 0x0015F8A0 File Offset: 0x0015DAA0
	private static void ShowUnlockIllustration()
	{
		AudioManager.Instance.PlaySound("UI_GameLineScroll_Illustration_Loop", true, false);
		ScrollHelper._expandedScroll = ScrollHelper._scrollHolder.GetChild(ScrollHelper._unlockIndex).GetComponent<RectTransform>();
		ScrollHelper.UpdateIllustration(ScrollHelper._expandedScroll, true, true);
		ScrollHelper.ShowIllustration(false);
		bool isInGameWorld = ScrollHelper._isInGameWorld;
		if (isInGameWorld)
		{
			ScrollHelper._root.CGet<CImage>("BorderLeft").gameObject.SetActive(false);
			ScrollHelper._root.CGet<CImage>("BorderRight").gameObject.SetActive(false);
		}
		ScrollHelper._root.CGet<GameObject>("Close").SetActive(false);
		ScrollHelper._root.CGet<GameObject>("ClickMask").SetActive(true);
		ScrollHelper._storyTransform.GetComponent<CanvasGroup>().alpha = 0f;
		SkeletonGraphic[] scrollGraphics = ScrollHelper._illustrationRefers.CGet<RectTransform>("Scroll").GetComponentsInChildren<SkeletonGraphic>();
		SkeletonGraphic[] sealGraphics = ScrollHelper._illustrationRefers.CGet<RectTransform>("Seal").GetComponentsInChildren<SkeletonGraphic>();
		CRawImage shadowImg = ScrollHelper._illustrationRefers.CGet<CRawImage>("Shadow");
		CanvasGroup unlockBtnCanvas = ScrollHelper._isInGameWorld ? ScrollHelper._illustrationRefers.CGet<CButtonObsolete>("UnlockIllustration").GetComponent<CanvasGroup>() : ScrollHelper._illustrationRefers.CGet<GameObject>("UnlockIllustration").GetComponent<CanvasGroup>();
		GameObject btnBack = ScrollHelper._illustrationRefers.CGet<GameObject>("UnlockBtnBack");
		Sequence seq = DOTween.Sequence();
		unlockBtnCanvas.GetComponent<CButtonObsolete>().interactable = false;
		unlockBtnCanvas.alpha = 0f;
		btnBack.gameObject.SetActive(false);
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
		seq.Append(unlockBtnCanvas.DOFade(1f, 0.3f).SetEase(Ease.Linear).OnComplete(delegate
		{
			unlockBtnCanvas.GetComponent<CButtonObsolete>().interactable = true;
			btnBack.gameObject.SetActive(true);
		}));
		seq.Play<Sequence>();
	}

	// Token: 0x06002CA7 RID: 11431 RVA: 0x0015FBB8 File Offset: 0x0015DDB8
	private static void UnlockStory()
	{
		BasicGameData gameData = SingletonObject.getInstance<BasicGameData>();
		bool isRanChenZi = ScrollHelper._unlockIndex >= gameData.XiangshuAvatarTaskStatusArray.Length;
		XiangshuAvatarTaskStatus status = (!isRanChenZi) ? gameData.XiangshuAvatarTaskStatusArray[ScrollHelper._unlockIndex] : default(XiangshuAvatarTaskStatus);
		int pageCount = ScrollHelper.GetPageCount(ScrollHelper._unlockIndex);
		float moveToEndTime = 1f * (float)(pageCount - 1);
		SkeletonGraphic[] scrollGraphics = ScrollHelper._illustrationRefers.CGet<RectTransform>("Scroll").GetComponentsInChildren<SkeletonGraphic>();
		SkeletonGraphic[] sealGraphics = ScrollHelper._illustrationRefers.CGet<RectTransform>("Seal").GetComponentsInChildren<SkeletonGraphic>();
		CRawImage shadowImg = ScrollHelper._illustrationRefers.CGet<CRawImage>("Shadow");
		CanvasGroup unlockBtnCanvas = ScrollHelper._illustrationRefers.CGet<CButtonObsolete>("UnlockIllustration").GetComponent<CanvasGroup>();
		Sequence seq = DOTween.Sequence();
		ScrollHelper._illustrationRefers.CGet<GameObject>("UnlockBtnBack").gameObject.SetActive(false);
		shadowImg.gameObject.SetActive(true);
		shadowImg.color = Color.white.SetAlpha(1f);
		for (int i = 0; i < ScrollHelper._scrollHolder.childCount; i++)
		{
			CanvasGroup scrollCanvas = ScrollHelper._scrollHolder.GetChild(i).GetComponent<CanvasGroup>();
			scrollCanvas.alpha = 0f;
			seq.Join(scrollCanvas.DOFade(1f, 0.3f).SetEase(Ease.Linear));
		}
		ScrollHelper._scrollHolder.gameObject.SetActive(true);
		for (int j = 0; j < scrollGraphics.Length; j++)
		{
			seq.Join(scrollGraphics[j].DOFade(0f, 0.3f).SetEase(Ease.Linear));
		}
		for (int k = 0; k < sealGraphics.Length; k++)
		{
			seq.Join(sealGraphics[k].DOFade(0f, 0.3f).SetEase(Ease.Linear));
		}
		seq.Join(unlockBtnCanvas.DOFade(0f, 0.3f).SetEase(Ease.Linear));
		seq.Join(shadowImg.DOFade(0f, 0.3f).SetEase(Ease.Linear));
		seq.AppendCallback(delegate
		{
			ScrollHelper.ExpandScroll(ScrollHelper._unlockIndex, true);
			ScrollHelper._storyTransform.GetComponent<CanvasGroup>().alpha = 1f;
			ScrollHelper._illustrationRefers.gameObject.SetActive(false);
			bool isInGameWorld = ScrollHelper._isInGameWorld;
			if (isInGameWorld)
			{
				ScrollHelper._root.CGet<CImage>("BorderLeft").gameObject.SetActive(pageCount > 1);
				ScrollHelper._root.CGet<CImage>("BorderRight").gameObject.SetActive(pageCount > 1);
			}
		});
		seq.AppendInterval(2.2f);
		seq.AppendCallback(delegate
		{
			float endPosX = (ScrollHelper._storyTransform.sizeDelta.x - ScrollHelper._storyTransform.parent.GetComponent<RectTransform>().sizeDelta.x) / 2f;
			ScrollHelper._storyTransform.DOAnchorPosX(endPosX, moveToEndTime, false).SetEase(Ease.OutCubic);
		});
		seq.AppendInterval(moveToEndTime + (float)(isRanChenZi ? 1 : 0));
		bool flag = status.JuniorXiangshuTaskStatus > 4 || isRanChenZi;
		if (flag)
		{
			Action<Texture2D> <>9__4;
			seq.AppendCallback(delegate
			{
				BossItem bossConfig = Boss.Instance[(int)ScrollHelper._expandedScroll.GetComponent<Refers>().UserFloat];
				SkeletonGraphic skeletonGraphic = ScrollHelper._illustrationRefers.CGet<SkeletonGraphic>("CharSkeletonGraphic");
				bool goodEnding = ScrollHelper.IsGoodEnding(ScrollHelper._unlockIndex);
				string particlePath = "RemakeResources/Particle/UIEffectPrefabs/Gamelinescroll/" + bossConfig.IllustrationUnlockParticle[goodEnding ? 0 : 1];
				string texturePath = "RemakeResources/Textures/GameLineScroll/" + bossConfig.ShadowTexture[goodEnding ? 1 : 2];
				ScrollHelper.UpdateIllustration(ScrollHelper._expandedScroll, false, false);
				ScrollHelper.ShowIllustration(false);
				string assetPath = texturePath;
				Action<Texture2D> onLoad;
				if ((onLoad = <>9__4) == null)
				{
					onLoad = (<>9__4 = delegate(Texture2D texture)
					{
						shadowImg.texture = texture;
					});
				}
				ResLoader.Load<Texture2D>(assetPath, onLoad, null, false);
				short[] shadowPos = bossConfig.ShadowPos[goodEnding ? 0 : 1];
				shadowImg.rectTransform.anchoredPosition = new Vector2((float)shadowPos[0], (float)shadowPos[1]);
				skeletonGraphic.Skeleton.SetToSetupPose();
				skeletonGraphic.timeScale = 0f;
				skeletonGraphic.color = Color.white.SetAlpha(0f);
				for (int l = 0; l < scrollGraphics.Length; l++)
				{
					scrollGraphics[l].DOFade(1f, 0.3f).SetEase(Ease.Linear);
				}
				shadowImg.gameObject.SetActive(true);
				shadowImg.DOFade(1f, 0.3f).SetEase(Ease.Linear);
				ScrollHelper._storyTransform.DOKill(false);
				ScrollHelper._storyTransform.GetComponent<CanvasGroup>().DOFade(0f, 0.3f).SetEase(Ease.Linear).OnComplete(delegate
				{
					skeletonGraphic.color = Color.white;
				});
				TweenCallback <>9__7;
				ResLoader.Load<GameObject>(particlePath, delegate(GameObject particlePrefab)
				{
					GameObject particleObj = Object.Instantiate<GameObject>(particlePrefab, ScrollHelper._illustrationRefers.transform);
					ParticleSystem particle = particleObj.GetComponent<ParticleSystem>();
					float delay = 0.94f;
					TweenCallback callback;
					if ((callback = <>9__7) == null)
					{
						callback = (<>9__7 = delegate()
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
						ScrollHelper._root.CGet<GameObject>("ClickMask").SetActive(false);
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
		seq.Play<Sequence>();
	}

	// Token: 0x06002CA8 RID: 11432 RVA: 0x0015FEAB File Offset: 0x0015E0AB
	public static void OnSaveFileDeleted()
	{
		ScrollHelper.OnOnRecordRootToggleChanged(ScrollHelper._root, -1);
	}

	// Token: 0x06002CA9 RID: 11433 RVA: 0x0015FEBC File Offset: 0x0015E0BC
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
			ScrollHelper.OnInit(root, -1, false, key);
		}
	}

	// Token: 0x06002CAA RID: 11434 RVA: 0x0015FF50 File Offset: 0x0015E150
	public static void OnClick(CButtonObsolete btn)
	{
		string name = btn.name;
		string a = name;
		if (!(a == "CloseButton"))
		{
			if (!(a == "ShowIllustration"))
			{
				if (!(a == "UnlockIllustration"))
				{
					if (!(a == "SkipUiAni"))
					{
						if (a == "Close")
						{
							ScrollHelper.HideScroll(false);
						}
					}
					else
					{
						bool isSkipEnabled = ScrollHelper._root.CGet<LeftMouseSkipAnimNotice>("LeftMouseSkipAnimNotice").IsSkipEnabled;
						if (isSkipEnabled)
						{
							ScrollHelper.SetToAniEndState(ScrollHelper._expanding);
						}
					}
				}
				else
				{
					btn.interactable = false;
					AudioManager.Instance.StopSound("UI_GameLineScroll_Illustration_Loop");
					ScrollHelper.UnlockStory();
				}
			}
			else
			{
				ScrollHelper.ShowIllustration(false);
			}
		}
		else
		{
			ScrollHelper.HideScroll(true);
			ScrollHelper._scrollAniSeq.Complete(true);
		}
	}

	// Token: 0x06002CAB RID: 11435 RVA: 0x00160018 File Offset: 0x0015E218
	public static bool GetQuickHideBanned()
	{
		return ScrollHelper._quickHideBanned;
	}

	// Token: 0x06002CAC RID: 11436 RVA: 0x0016002F File Offset: 0x0015E22F
	public static void SetQuickHideBanned(bool val)
	{
		ScrollHelper._quickHideBanned = val;
	}

	// Token: 0x06002CAD RID: 11437 RVA: 0x00160038 File Offset: 0x0015E238
	private static int GetPageCount(int index)
	{
		bool isRanChenZi = index >= ScrollHelper._xiangshuAvatarTaskStatuses.Length;
		return (!isRanChenZi) ? Mathf.Min((int)ScrollHelper._xiangshuAvatarTaskStatuses[index].JuniorXiangshuTaskStatus, 5) : 1;
	}

	// Token: 0x06002CAE RID: 11438 RVA: 0x00160074 File Offset: 0x0015E274
	private static bool IsGoodEnding(int index)
	{
		bool isRanChenZi = index >= ScrollHelper._xiangshuAvatarTaskStatuses.Length;
		return (!isRanChenZi) ? (ScrollHelper._xiangshuAvatarTaskStatuses[index].JuniorXiangshuTaskStatus == 6) : ScrollHelper._beatRanChenZi;
	}

	// Token: 0x06002CAF RID: 11439 RVA: 0x001600B4 File Offset: 0x0015E2B4
	private static void ChangeScrollScale(Refers scroll, bool isExpand = true)
	{
		scroll.transform.localScale = (isExpand ? new Vector3(1f, 1f, 1f) : new Vector3(0.8f, 0.8f, 0.8f));
		scroll.CGet<TextMeshProUGUI>("Name").transform.localScale = (isExpand ? new Vector3(1f, 1f, 1f) : new Vector3(1.25f, 1.25f, 1.25f));
		GameObject nameEnHolder;
		bool flag = scroll.CTryGet<GameObject>("NameEnHolder", out nameEnHolder);
		if (flag)
		{
			nameEnHolder.transform.localScale = (isExpand ? new Vector3(1f, 1f, 1f) : new Vector3(1.25f, 1.25f, 1.25f));
		}
		UI_GameLineScroll gameLineScroll = UIElement.GameLineScroll.UiBaseAs<UI_GameLineScroll>();
		GameObject UIMask = gameLineScroll.CGet<GameObject>("UIMask");
		if (isExpand)
		{
			UIMask.transform.SetParent(ScrollHelper._root.CGet<RectTransform>("Mask"));
			UIMask.transform.SetSiblingIndex(1);
		}
	}

	// Token: 0x06002CB0 RID: 11440 RVA: 0x001601D4 File Offset: 0x0015E3D4
	public static void OnLanguageChange(LocalStringManager.LanguageType languageType)
	{
		for (int i = 0; i < ScrollHelper._scrollHolder.childCount; i++)
		{
			Refers scrollRefers = ScrollHelper._scrollHolder.GetChild(i).GetComponent<Refers>();
			GameObject nameEnHolder;
			bool flag = scrollRefers.CTryGet<GameObject>("NameEnHolder", out nameEnHolder);
			if (flag)
			{
				nameEnHolder.SetActive(languageType > LocalStringManager.LanguageType.CN);
				nameEnHolder.transform.GetChild(0).GetComponent<LanguageRuleExpandOnHover>().OnLanguageChange(languageType);
			}
		}
	}

	// Token: 0x04002048 RID: 8264
	private const string TextureDir = "RemakeResources/Textures/GameLineScroll/";

	// Token: 0x04002049 RID: 8265
	private const string IllustrationDir = "RemakeResources/SpineAnimations/DynamicIllustration/";

	// Token: 0x0400204A RID: 8266
	private const string ParticleDir = "RemakeResources/Particle/UIEffectPrefabs/Gamelinescroll/";

	// Token: 0x0400204B RID: 8267
	private const float ScrollExpandMoveTime = 1f;

	// Token: 0x0400204C RID: 8268
	private const float ScrollExpandTime = 1f;

	// Token: 0x0400204D RID: 8269
	private const short PageWidth = 1795;

	// Token: 0x0400204E RID: 8270
	private const short StoryWidthAdd = 45;

	// Token: 0x0400204F RID: 8271
	private const short MultiplePageExpandAdd = 500;

	// Token: 0x04002050 RID: 8272
	private static readonly Vector2[] _scrollOriginPos = new Vector2[10];

	// Token: 0x04002051 RID: 8273
	private static readonly Vector2 _normalShadowPos = new Vector2(20f, 190f);

	// Token: 0x04002052 RID: 8274
	private static readonly Vector2 _anchorCenter = new Vector2(0.5f, 0.5f);

	// Token: 0x04002053 RID: 8275
	private static readonly Vector2 _anchorRight = new Vector2(1f, 0.5f);

	// Token: 0x04002054 RID: 8276
	private static int _unlockIndex;

	// Token: 0x04002055 RID: 8277
	private static bool _showingUnlock;

	// Token: 0x04002056 RID: 8278
	private static RectTransform _scrollHolder;

	// Token: 0x04002057 RID: 8279
	private static CImage _scrollHolderBack;

	// Token: 0x04002058 RID: 8280
	private static RectTransform _expandedScroll;

	// Token: 0x04002059 RID: 8281
	private static RectTransform _storyTransform;

	// Token: 0x0400205A RID: 8282
	private static Refers _illustrationRefers;

	// Token: 0x0400205B RID: 8283
	private static Sequence _scrollAniSeq;

	// Token: 0x0400205C RID: 8284
	private static bool _expanding;

	// Token: 0x0400205D RID: 8285
	private static Refers _root;

	// Token: 0x0400205E RID: 8286
	private static bool _isInGameWorld;

	// Token: 0x0400205F RID: 8287
	private static XiangshuAvatarTaskStatus[] _xiangshuAvatarTaskStatuses;

	// Token: 0x04002060 RID: 8288
	private static bool _beatRanChenZi;

	// Token: 0x04002061 RID: 8289
	public static bool _quickHideBanned = false;

	// Token: 0x04002062 RID: 8290
	private static HashSet<Refers> _initedRoots = new HashSet<Refers>();

	// Token: 0x04002063 RID: 8291
	public static bool ExpandState;

	// Token: 0x04002064 RID: 8292
	private static Sequence _illustrationSequence;

	// Token: 0x02001662 RID: 5730
	private static class BossIllustrationIndex
	{
		// Token: 0x0400A7C9 RID: 42953
		public const sbyte Normal = 0;

		// Token: 0x0400A7CA RID: 42954
		public const sbyte Joy = 1;

		// Token: 0x0400A7CB RID: 42955
		public const sbyte Sad = 2;

		// Token: 0x0400A7CC RID: 42956
		public const sbyte KidNormal = 3;

		// Token: 0x0400A7CD RID: 42957
		public const sbyte KidJoy = 4;

		// Token: 0x0400A7CE RID: 42958
		public const sbyte KidSad = 5;
	}

	// Token: 0x02001663 RID: 5731
	private static class BossUnlockEffectIndex
	{
		// Token: 0x0400A7CF RID: 42959
		public const sbyte Joy = 0;

		// Token: 0x0400A7D0 RID: 42960
		public const sbyte Sad = 1;

		// Token: 0x0400A7D1 RID: 42961
		public const sbyte KidJoy = 2;

		// Token: 0x0400A7D2 RID: 42962
		public const sbyte KidSad = 3;
	}
}
