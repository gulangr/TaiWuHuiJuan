using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.TaiwuEvent;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;

// Token: 0x02000376 RID: 886
public class UI_CollectResource : UIBase
{
	// Token: 0x060033D8 RID: 13272 RVA: 0x0019A230 File Offset: 0x00198430
	public override void OnInit(ArgumentBox argsBox)
	{
		this.Init(argsBox);
		CRawImage treeRoot = base.CGet<CRawImage>("SkillCollect");
		treeRoot.gameObject.SetActive(!this._singleMode);
		treeRoot.color = treeRoot.color.SetAlpha(0f);
		this._singleCollectRefers.gameObject.SetActive(this._singleMode);
		bool flag = this._singleMode && !this._isPracticeCombat;
		if (flag)
		{
			this.PrepareForResourceCollection(this._singleCollectRefers, this._collectResults[0]);
		}
		else
		{
			bool isPracticeCombat = this._isPracticeCombat;
			if (isPracticeCombat)
			{
				this.PrepareForPracticeCombatSkill(this._singleCollectRefers);
			}
		}
		int i = 0;
		int max = this._collectPrefabList.Count;
		while (i < max)
		{
			Refers refers = this._collectPrefabList[i];
			refers.gameObject.SetActive(!this._singleMode);
			bool singleMode = this._singleMode;
			if (!singleMode)
			{
				bool hasData = this._collectResults.CheckIndex(i);
				refers.gameObject.SetActive(hasData);
				bool flag2 = hasData;
				if (flag2)
				{
					CollectResourceResult current = this._collectResults[i];
					for (int j = 0; j < this._collectResults.Count; j++)
					{
						bool flag3 = i == j;
						if (!flag3)
						{
							CollectResourceResult merging = this._collectResults[j];
							bool flag4 = merging.ResourceType != current.ResourceType;
							if (!flag4)
							{
								current.ResourceCount += merging.ResourceCount;
							}
						}
					}
					this.PrepareForResourceCollection(refers, current);
				}
			}
			i++;
		}
		bool flag5 = !this._singleMode;
		if (flag5)
		{
			UIElement element = this.Element;
			element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(this.StartTreeTweeningIn));
		}
		this._collecting = true;
	}

	// Token: 0x060033D9 RID: 13273 RVA: 0x0019A42C File Offset: 0x0019862C
	public void Init(ArgumentBox argsBox)
	{
		this._singleCollectRefers = base.CGet<Refers>("SingleCollectPrefab");
		this._collectPrefabList = base.CGetList<Refers>("CollectPrefab_");
		if (this._resourceIconPool == null)
		{
			this._resourceIconPool = new PoolItem("CollectResource_ResourceIcon", base.CGet<GameObject>("ResourceIcon"));
		}
		if (this._itemIconPool == null)
		{
			this._itemIconPool = new PoolItem("CollectResource_ItemIcon", base.CGet<GameObject>("ItemIcon"));
		}
		this._bottomRoot = base.CGet<Transform>("BottomRoot");
		for (int index = 0; index < this._showingResIconCacheList.Length; index++)
		{
			List<GameObject>[] showingResIconCacheList = this._showingResIconCacheList;
			int num = index;
			if (showingResIconCacheList[num] == null)
			{
				showingResIconCacheList[num] = new List<GameObject>();
			}
		}
		if (argsBox == null)
		{
			argsBox = new ArgumentBox();
		}
		argsBox.Get<List<CollectResourceResult>>("CollectInfo", out this._collectResults);
		argsBox.Get("CollectResourceIsMax", out this._collectResourceIsMax);
		argsBox.Get("PracticeCombat", out this._isPracticeCombat);
		argsBox.Get("Proficiency", out this._proficiency);
		argsBox.Get("IsChoosy", out this._isChoosy);
		List<CollectResourceResult> collectResults = this._collectResults;
		this._singleMode = ((collectResults != null && collectResults.Count == 1) || this._isPracticeCombat);
		this._collecting = false;
		bool flag = this.Element != null;
		if (flag)
		{
			GEvent.Add(UiEvents.ResponseBottomTimeDisk, new GEvent.Callback(this.ResponseBottomTimeDisk));
			GEvent.OnEvent(UiEvents.RequestBottomTimeDisk, null);
			UIElement element = this.Element;
			element.OnHide = (Action)Delegate.Combine(element.OnHide, new Action(this.OnHide));
		}
	}

	// Token: 0x060033DA RID: 13274 RVA: 0x0019A5DC File Offset: 0x001987DC
	public void ClearResourceIcon(sbyte resourceType)
	{
		List<GameObject> list = this._showingResIconCacheList[(int)resourceType];
		list.ForEach(new Action<GameObject>(this._resourceIconPool.DestroyObject));
		list.Clear();
	}

	// Token: 0x060033DB RID: 13275 RVA: 0x0019A614 File Offset: 0x00198814
	public void Clear()
	{
		this._resourceType = -1;
		bool flag = this._itemIconPool == null;
		if (!flag)
		{
			this._showingItemIconCacheList.ForEach(new Action<GameObject>(this._itemIconPool.DestroyObject));
			this._showingItemIconCacheList.Clear();
			sbyte index = 0;
			while ((int)index < this._showingResIconCacheList.Length)
			{
				this.ClearResourceIcon(index);
				index += 1;
			}
			for (int index2 = 0; index2 < this._showingResIconCoroutineList.Length; index2++)
			{
				this._showingResIconCoroutineList[index2] = null;
			}
			RectTransform singlePrefabAnimPosition = this._singleCollectRefers.CGet<RectTransform>("AnimationPosition");
			for (int i = 0; i < singlePrefabAnimPosition.childCount; i++)
			{
				Object.Destroy(singlePrefabAnimPosition.GetChild(i).gameObject);
			}
			foreach (Refers refers in this._collectPrefabList)
			{
				RectTransform position = refers.CGet<RectTransform>("AnimationPosition");
				for (int j = 0; j < position.childCount; j++)
				{
					Object.Destroy(position.GetChild(j).gameObject);
				}
				refers.gameObject.SetActive(false);
			}
			this._collectResults = null;
			this._collecting = false;
			this.StopAllResourceDropAnim();
		}
	}

	// Token: 0x060033DC RID: 13276 RVA: 0x0019A790 File Offset: 0x00198990
	private void Update()
	{
		bool isChoosy = this._isChoosy;
		if (!isChoosy)
		{
			bool collecting = this._collecting;
			if (collecting)
			{
				bool singleMode = this._singleMode;
				if (singleMode)
				{
					bool flag = this._singleCollectRefers.UserObject != null && (!CommonCommandKit.LeftMouse.Check(this.Element, false, false, false, true, false) || !base.CGet<LeftMouseSkipAnimNotice>("LeftMouseSkipAnimNotice").IsSkipEnabled);
					if (flag)
					{
						return;
					}
				}
				else
				{
					foreach (Refers refers in this._collectPrefabList)
					{
						bool flag2 = refers.UserObject != null;
						if (flag2)
						{
							return;
						}
					}
				}
				this._collecting = false;
				GameObject skipAnimHotKeyDisplay;
				bool flag3 = this.CTryGet<GameObject>("SkipAnimHotKeyDisplay", out skipAnimHotKeyDisplay);
				if (flag3)
				{
					skipAnimHotKeyDisplay.SetActive(false);
				}
			}
			else
			{
				bool anyKeyDown = Input.anyKeyDown;
				if (anyKeyDown)
				{
					base.StartCoroutine(this.ExitCollectAction());
				}
			}
		}
	}

	// Token: 0x060033DD RID: 13277 RVA: 0x0019A8B0 File Offset: 0x00198AB0
	private void LateUpdate()
	{
		bool flag = this._resourceType < 0;
		if (!flag)
		{
			bool flag2 = this._showingResIconCacheList[this._resourceType].Count > 81;
			if (flag2)
			{
				int count = this._showingResIconCacheList[this._resourceType].Count / 2;
				for (int i = 0; i < count; i++)
				{
					Rigidbody2D b = this._showingResIconCacheList[this._resourceType][i].GetComponent<Rigidbody2D>();
					b.constraints = ((b.attachedColliderCount > 0) ? RigidbodyConstraints2D.FreezePosition : RigidbodyConstraints2D.None);
				}
			}
		}
	}

	// Token: 0x060033DE RID: 13278 RVA: 0x0019A942 File Offset: 0x00198B42
	private void OnDisable()
	{
		this.Clear();
	}

	// Token: 0x060033DF RID: 13279 RVA: 0x0019A94C File Offset: 0x00198B4C
	private void OnHide()
	{
		GEvent.Remove(UiEvents.ResponseBottomTimeDisk, new GEvent.Callback(this.ResponseBottomTimeDisk));
		this.TryReturnTimeDisk();
	}

	// Token: 0x060033E0 RID: 13280 RVA: 0x0019A974 File Offset: 0x00198B74
	private void ShowGetItemInfo()
	{
		bool isPracticeCombat = this._isPracticeCombat;
		if (!isPracticeCombat)
		{
			List<ItemDisplayData> list = new List<ItemDisplayData>();
			foreach (CollectResourceResult result in this._collectResults)
			{
				bool flag = result.ItemDisplayData != null;
				if (flag)
				{
					result.ItemDisplayData.Amount = 1;
					list.Add(result.ItemDisplayData);
				}
			}
			Action closeAction = (SingletonObject.getInstance<TaiwuCharacterModel>().HasInheritedTaiwu && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding) ? this.GetWudangHeavenlyTreeSeedAction() : null;
			bool flag2 = list.Count > 0;
			if (flag2)
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("ItemList", list);
				argBox.SetObject("CloseAction", closeAction);
				UIElement.GetItem.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.GetItem);
			}
			else if (closeAction != null)
			{
				closeAction();
			}
		}
	}

	// Token: 0x060033E1 RID: 13281 RVA: 0x0019AA8C File Offset: 0x00198C8C
	private unsafe Action GetWudangHeavenlyTreeSeedAction()
	{
		Action closeAction = null;
		sbyte resourceType = -1;
		MapBlockData mapBlockData = SingletonObject.getInstance<WorldMapModel>().PlayerAtBlock;
		int maxResourceCount = 0;
		bool singleMode = this._singleMode;
		if (singleMode)
		{
			resourceType = this._collectResults[0].ResourceType;
			maxResourceCount = (int)(this._collectResourceIsMax ? (*(ref mapBlockData.MaxResources.Items.FixedElementField + (IntPtr)resourceType * 2)) : 0);
		}
		else
		{
			for (int i = 0; i < 6; i++)
			{
				bool flag = *(ref mapBlockData.CurrResources.Items.FixedElementField + (IntPtr)i * 2) == *(ref mapBlockData.MaxResources.Items.FixedElementField + (IntPtr)i * 2);
				if (flag)
				{
					maxResourceCount += (int)(*(ref mapBlockData.MaxResources.Items.FixedElementField + (IntPtr)i * 2));
				}
			}
		}
		int prob = maxResourceCount / 10;
		bool flag2 = GameApp.Random.CheckPercentProb(prob);
		if (flag2)
		{
			closeAction = delegate()
			{
				TaiwuEventDomainMethod.Call.TaiwuCollectWudangHeavenlyTreeSeed(resourceType);
			};
		}
		return closeAction;
	}

	// Token: 0x060033E2 RID: 13282 RVA: 0x0019AB9C File Offset: 0x00198D9C
	private void StartTreeTweeningIn()
	{
		CRawImage treeRoot = base.CGet<CRawImage>("SkillCollect");
		treeRoot.rectTransform.anchoredPosition = Vector2.down * 400f;
		treeRoot.rectTransform.DOAnchorPosY(0f, this.TextChangeDelayTime, false).SetAutoKill(true);
		DOVirtual.Float(0f, 1f, this.TextChangeDelayTime, delegate(float stepValue)
		{
			treeRoot.color = treeRoot.color.SetAlpha(stepValue);
		}).SetAutoKill(true);
	}

	// Token: 0x060033E3 RID: 13283 RVA: 0x0019AC2C File Offset: 0x00198E2C
	private void StartTreeTweeningOut()
	{
		CRawImage treeRoot = base.CGet<CRawImage>("SkillCollect");
		treeRoot.rectTransform.DOLocalMoveY(-400f, this.FadeTime, false).SetAutoKill(true);
		DOVirtual.Float(1f, 0f, this.FadeTime, delegate(float stepValue)
		{
			treeRoot.color = treeRoot.color.SetAlpha(stepValue);
		}).SetAutoKill(true).SetUpdate(true);
	}

	// Token: 0x060033E4 RID: 13284 RVA: 0x0019ACA4 File Offset: 0x00198EA4
	private void PrepareForResourceCollection(Refers refers, CollectResourceResult result)
	{
		bool flag = null == refers;
		if (!flag)
		{
			List<CImage> rotateImages;
			List<CRawImage> progressBarMain;
			this.PrepareForResourceCollectionWithoutSpine(refers, result, out rotateImages, out progressBarMain);
			this.PrepareForResourceCollectionSpine(refers, result, true, true);
			int dropIconCount = this.GetDropIconCount(refers, result);
			GameObject skipAnimHotKeyDisplay;
			bool flag2 = this.CTryGet<GameObject>("SkipAnimHotKeyDisplay", out skipAnimHotKeyDisplay);
			if (flag2)
			{
				skipAnimHotKeyDisplay.SetActive(true);
			}
			GameObject successObj;
			bool flag3 = this._singleCollectRefers.CTryGet<GameObject>("Success", out successObj);
			if (flag3)
			{
				successObj.SetActive(false);
			}
			this._resourceType = (int)result.ResourceType;
			UIElement element = this.Element;
			element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(delegate()
			{
				this.StartCoroutine(this.CreateResourceIcon(refers, rotateImages, progressBarMain, result, dropIconCount));
			}));
		}
	}

	// Token: 0x060033E5 RID: 13285 RVA: 0x0019ADA3 File Offset: 0x00198FA3
	private List<CRawImage> GetProgressBarMainList(Refers refers)
	{
		return refers.CGetList<CRawImage>("Progress1_");
	}

	// Token: 0x060033E6 RID: 13286 RVA: 0x0019ADB0 File Offset: 0x00198FB0
	private void PrepareForResourceCollectionWithoutSpine(Refers refers, CollectResourceResult result, out List<CImage> rotateImages, out List<CRawImage> progressBarMain)
	{
		rotateImages = null;
		progressBarMain = null;
		bool flag = null == refers;
		if (!flag)
		{
			CRawImage treeRoot = base.CGet<CRawImage>("SkillCollect");
			ResourceTypeItem config = ResourceType.Instance.GetItem(result.ResourceType);
			refers.UserObject = config;
			rotateImages = refers.CGetList<CImage>("Rotate_");
			bool flag2 = this._progressBarSubDefaultAngle == null;
			if (flag2)
			{
				this._progressBarSubDefaultAngle = new Vector3[]
				{
					rotateImages[0].rectTransform.parent.eulerAngles,
					rotateImages[1].rectTransform.parent.eulerAngles,
					rotateImages[2].rectTransform.parent.eulerAngles
				};
			}
			sbyte resourceType = result.ResourceType;
			bool isChoosy = this._isChoosy;
			if (isChoosy)
			{
				refers.CGet<TextMeshProUGUI>("WorkDesc").text = config.Name;
			}
			else
			{
				Transform target = treeRoot.transform.Find(string.Format("MoveFromDot_{0}", (int)refers.UserFloat));
				bool flag3 = null != target;
				if (flag3)
				{
					refers.transform.position = target.position;
				}
				refers.CGet<TextMeshProUGUI>("WorkDesc").text = LocalStringManager.Get(string.Format("LK_Resource_Collect_Title_{0}", resourceType));
				refers.CGet<TextMeshProUGUI>("ResourceCount").text = "0";
			}
			progressBarMain = this.GetProgressBarMainList(refers);
			progressBarMain.ForEach(delegate(CRawImage bar)
			{
				bar.rectTransform.eulerAngles = Vector3.zero;
			});
			for (int i = 0; i < rotateImages.Count; i++)
			{
				rotateImages[i].rectTransform.parent.eulerAngles = this._progressBarSubDefaultAngle[i];
			}
		}
	}

	// Token: 0x060033E7 RID: 13287 RVA: 0x0019AFA4 File Offset: 0x001991A4
	private void PrepareForResourceCollectionSpine(Refers refers, CollectResourceResult result, bool fadeIn = true, bool startingLoop = true)
	{
		UI_CollectResource.<>c__DisplayClass42_0 CS$<>8__locals1 = new UI_CollectResource.<>c__DisplayClass42_0();
		CS$<>8__locals1.fadeIn = fadeIn;
		CS$<>8__locals1.startingLoop = startingLoop;
		sbyte resourceType = result.ResourceType;
		CS$<>8__locals1.position = refers.CGet<RectTransform>("AnimationPosition");
		CanvasGroup canvasGroup = refers.GetComponent<CanvasGroup>();
		bool fadeIn2 = CS$<>8__locals1.fadeIn;
		if (fadeIn2)
		{
			canvasGroup.DOKill(false);
			canvasGroup.alpha = 0f;
		}
		bool isChoosy = this._isChoosy;
		int index;
		if (isChoosy)
		{
			int choosyCount = result.ResourceCount / GlobalConfig.Instance.ChoosyResourceBaseCost;
			index = Mathf.Clamp(choosyCount / 10, 1, 3);
		}
		else
		{
			index = Mathf.Clamp(result.ResourceCount / 48, 1, 3);
		}
		string typeName = UI_CollectResource.ResourceTypeNames[(int)resourceType];
		string path = string.Format("RemakeResources/Prefab/Legacy/Core/Gather/gather_{0}_{1}", typeName, index);
		bool hasGetSpine = false;
		for (int i = 0; i < CS$<>8__locals1.position.childCount; i++)
		{
			GameObject root = CS$<>8__locals1.position.transform.GetChild(i).gameObject;
			bool flag = root.name.Contains(typeName) && root.name.Contains(index.ToString());
			if (flag)
			{
				root.gameObject.SetActive(true);
				CS$<>8__locals1.<PrepareForResourceCollectionSpine>g__OnGetSpine|1(root);
				hasGetSpine = true;
			}
			else
			{
				root.gameObject.SetActive(false);
			}
		}
		bool flag2 = hasGetSpine;
		if (!flag2)
		{
			ResLoader.Load<GameObject>(path, delegate(GameObject prefab)
			{
				GameObject root2 = Object.Instantiate<GameObject>(prefab, CS$<>8__locals1.position);
				base.<PrepareForResourceCollectionSpine>g__OnGetSpine|1(root2);
			}, null, false);
		}
	}

	// Token: 0x060033E8 RID: 13288 RVA: 0x0019B128 File Offset: 0x00199328
	private float GetCollectTime(Refers refers)
	{
		float resCollectTime = 2f;
		bool flag = refers.UserInt >= 0;
		if (flag)
		{
			resCollectTime *= 2f;
		}
		return resCollectTime;
	}

	// Token: 0x060033E9 RID: 13289 RVA: 0x0019B15C File Offset: 0x0019935C
	private int GetDropIconCount(Refers refers, CollectResourceResult result)
	{
		ResourceTypeItem config = (ResourceTypeItem)refers.UserObject;
		int dropIconCount = result.ResourceCount / (int)config.CollectMultiplier / ((SingletonObject.getInstance<BasicGameData>().WorldResourceAmountType == 0) ? 2 : 1);
		bool flag = dropIconCount > 10;
		if (flag)
		{
			dropIconCount = (int)Mathf.Min(220f, (float)((long)result.ResourceCount * (long)(94 - result.ResourceCount / 10) / 100L));
		}
		return dropIconCount;
	}

	// Token: 0x060033EA RID: 13290 RVA: 0x0019B1C9 File Offset: 0x001993C9
	private IEnumerator CreateResourceIcon(Refers refers, List<CImage> rotateImages, List<CRawImage> progressBarMain, CollectResourceResult result, int dropIconCount)
	{
		refers.UserInt = ((result.ItemDisplayData != null) ? Mathf.Max(0, dropIconCount - 4) : -1);
		float resCollectTime = this.GetCollectTime(refers);
		float generateGap = Mathf.Min(resCollectTime / (float)Math.Max(dropIconCount - 1, 1), 0.3f);
		WaitForSeconds wait = new WaitForSeconds(generateGap);
		bool singleMode = this._singleMode;
		if (singleMode)
		{
			yield return wait;
		}
		bool flag = !this._isChoosy;
		if (flag)
		{
			this.PlayResourceAnim(refers, progressBarMain, resCollectTime, true);
		}
		RectTransform holder = refers.CGet<RectTransform>("IconHolder");
		int num;
		for (int i = 0; i < dropIconCount; i = num)
		{
			bool flag2 = !refers.gameObject.activeInHierarchy;
			if (flag2)
			{
				yield break;
			}
			bool flag3 = i == refers.UserInt;
			GameObject dropIcon;
			if (flag3)
			{
				dropIcon = this._itemIconPool.GetObject();
				this._showingItemIconCacheList.Add(dropIcon);
				GameObject successObj;
				bool flag4 = refers.CTryGet<GameObject>("Success", out successObj);
				if (flag4)
				{
					successObj.SetActive(true);
				}
				successObj = null;
			}
			else
			{
				dropIcon = this._resourceIconPool.GetObject();
				string resourceSprite = CommonUtils.GetResourceSpriteName(result.ResourceType, true);
				dropIcon.GetComponent<CImage>().SetSprite(resourceSprite, false, null);
				this._showingResIconCacheList[(int)result.ResourceType].Add(dropIcon);
				resourceSprite = null;
			}
			dropIcon.transform.SetParent(holder, false);
			dropIcon.transform.localPosition = new Vector3((float)Random.Range(-150, 151), (float)(290 + i), 0f);
			Rigidbody2D rigidBody = dropIcon.GetComponent<Rigidbody2D>();
			rigidBody.constraints = RigidbodyConstraints2D.None;
			bool flag5 = i == refers.UserInt;
			if (flag5)
			{
				rigidBody.AddForce(new Vector2(Random.Range(-50f, 50f), 0f));
			}
			else
			{
				rigidBody.AddForce(new Vector2(Random.Range(-10f, 10f), 0f));
			}
			dropIcon.gameObject.SetActive(true);
			for (int j = 0; j < rotateImages.Count; j = num + 1)
			{
				Transform rectTrans = rotateImages[j].rectTransform.parent;
				bool flag6 = !DOTween.IsTweening(rectTrans, true);
				if (flag6)
				{
					rectTrans.DORotate(rectTrans.eulerAngles - new Vector3(0f, 0f, Random.value * 5f + 5f), Random.value * 0.2f + 0.3f, RotateMode.Fast).SetEase(Ease.OutBounce).SetDelay(Random.value * 0.3f + 0.2f);
				}
				rectTrans = null;
				num = j;
			}
			yield return wait;
			long count = (dropIconCount == 0) ? 0L : ((long)result.ResourceCount * (long)i / (long)dropIconCount);
			refers.CGet<TextMeshProUGUI>("ResourceCount").text = count.ToString();
			dropIcon = null;
			rigidBody = null;
			num = i + 1;
		}
		yield return wait;
		refers.CGet<TextMeshProUGUI>("ResourceCount").text = result.ResourceCount.ToString();
		yield return new WaitForSeconds(generateGap + this.FadeTime);
		bool flag7 = !this._isChoosy;
		if (flag7)
		{
			refers.UserObject = null;
		}
		yield break;
	}

	// Token: 0x060033EB RID: 13291 RVA: 0x0019B200 File Offset: 0x00199400
	private void PlayResourceAnim(Refers refers, List<CRawImage> progressBarMain, float resCollectTime, bool fadeIn = true)
	{
		CRawImage treeRoot = base.CGet<CRawImage>("SkillCollect");
		CanvasGroup canvasGroup = refers.GetComponent<CanvasGroup>();
		RectTransform position = refers.CGet<RectTransform>("AnimationPosition");
		for (int i = 0; i < position.transform.childCount; i++)
		{
			Transform root = position.transform.GetChild(i);
			bool flag = !root.gameObject.activeSelf;
			if (!flag)
			{
				Refers childRefers = root.GetComponent<Refers>();
				bool flag2 = childRefers != null && childRefers != null && refers.Names.Contains("SE");
				if (flag2)
				{
					AudioClip se = refers.CGet<AudioClip>("SE");
					AudioManager.Instance.PlaySound(se.name, false, false);
				}
				else
				{
					ResourceTypeItem config = (ResourceTypeItem)refers.UserObject;
					AudioManager.Instance.PlaySound(UI_CollectResource._resourceSeNames[(int)config.TemplateId], false, false);
				}
				SkeletonGraphic[] skeletonGraphics = position.GetComponentsInChildren<SkeletonGraphic>();
				foreach (SkeletonGraphic graphic in skeletonGraphics)
				{
					if (fadeIn)
					{
						graphic.color = graphic.color.SetAlpha(0f);
						graphic.DOKill(false);
						graphic.DOFade(1f, this.FadeTime);
					}
					graphic.AnimationState.SetAnimation(0, "animation", true);
				}
			}
		}
		if (fadeIn)
		{
			canvasGroup.DOFade(1f, this.FadeTime);
		}
		bool flag3 = !this._isChoosy;
		if (flag3)
		{
			Transform fromPoint = treeRoot.transform.Find(string.Format("MoveFromDot_{0}", (int)refers.UserFloat));
			Transform target = treeRoot.transform.Find(string.Format("MoveToDot_{0}", (int)refers.UserFloat));
			bool flag4 = null != fromPoint && null != target;
			if (flag4)
			{
				DOVirtual.Float(0f, 1f, this.FadeTime, delegate(float stepValue)
				{
					refers.transform.localPosition = Vector3.Lerp(fromPoint.localPosition, target.localPosition, stepValue);
				}).SetAutoKill(true);
			}
		}
		for (int j = 0; j < progressBarMain.Count; j++)
		{
			CRawImage bar = progressBarMain[j];
			bar.rectTransform.DORotate(new Vector3(0f, 0f, -12f), resCollectTime + this.TextChangeDelayTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
		}
	}

	// Token: 0x060033EC RID: 13292 RVA: 0x0019B4E8 File Offset: 0x001996E8
	private void HideCollectCell(Refers refers)
	{
		CanvasGroup canvasGroup = refers.GetComponent<CanvasGroup>();
		RectTransform position = refers.CGet<RectTransform>("AnimationPosition");
		SkeletonGraphic[] skeletonGraphics = position.GetComponentsInChildren<SkeletonGraphic>();
		foreach (SkeletonGraphic graphic in skeletonGraphics)
		{
			graphic.DOKill(false);
			graphic.DOFade(0f, this.FadeTime);
		}
		canvasGroup.DOKill(false);
		canvasGroup.DOFade(0f, this.FadeTime);
	}

	// Token: 0x060033ED RID: 13293 RVA: 0x0019B563 File Offset: 0x00199763
	private IEnumerator ExitCollectAction()
	{
		bool singleMode = this._singleMode;
		if (singleMode)
		{
			this.HideCollectCell(this._singleCollectRefers);
		}
		else
		{
			foreach (Refers refers in this._collectPrefabList)
			{
				this.HideCollectCell(refers);
				refers = null;
			}
			List<Refers>.Enumerator enumerator = default(List<Refers>.Enumerator);
			this.StartTreeTweeningOut();
		}
		yield return new WaitForSeconds(this.FadeTime);
		this.ShowGetItemInfo();
		this.QuickHide();
		yield break;
	}

	// Token: 0x060033EE RID: 13294 RVA: 0x0019B574 File Offset: 0x00199774
	private void ResponseBottomTimeDisk(ArgumentBox argbox)
	{
		Transform backImg;
		argbox.Get<Transform>("backImg", out backImg);
		bool flag = backImg != null;
		if (flag)
		{
			this._backImg = backImg;
			this._backImg.SetParent(this._bottomRoot, true);
		}
		Transform timeDisk;
		argbox.Get<Transform>("timeDisk", out timeDisk);
		bool flag2 = timeDisk != null;
		if (flag2)
		{
			this._timeDisk = timeDisk;
			this._timeDisk.SetParent(this._bottomRoot, true);
			this._timeDisk.GetComponent<Refers>().CGet<CButtonObsolete>("AdvanceSolarTerm").interactable = false;
		}
		Transform readAndLoop;
		argbox.Get<Transform>("readAndLoop", out readAndLoop);
		bool flag3 = readAndLoop != null;
		if (flag3)
		{
			this._readAndLoop = readAndLoop;
			this._readAndLoop.SetParent(this._bottomRoot, true);
		}
		Transform timeText;
		argbox.Get<Transform>("timeText", out timeText);
		bool flag4 = timeText != null;
		if (flag4)
		{
			this._timeText = timeText;
			this._timeText.SetParent(this._bottomRoot, true);
		}
	}

	// Token: 0x060033EF RID: 13295 RVA: 0x0019B67C File Offset: 0x0019987C
	private void TryReturnTimeDisk()
	{
		ArgumentBox args = EasyPool.Get<ArgumentBox>();
		bool flag = this._timeDisk != null || this._backImg != null || this._readAndLoop != null || this._timeText != null;
		if (flag)
		{
			Transform timeDisk = this._timeDisk;
			CButtonObsolete cbuttonObsolete;
			if (timeDisk == null)
			{
				cbuttonObsolete = null;
			}
			else
			{
				Refers component = timeDisk.GetComponent<Refers>();
				cbuttonObsolete = ((component != null) ? component.CGet<CButtonObsolete>("AdvanceSolarTerm") : null);
			}
			CButtonObsolete btn = cbuttonObsolete;
			bool flag2 = btn != null;
			if (flag2)
			{
				btn.interactable = true;
			}
			bool flag3 = this._timeDisk != null;
			if (flag3)
			{
				args.SetObject("timeDisk", this._timeDisk);
			}
			bool flag4 = this._backImg != null;
			if (flag4)
			{
				args.SetObject("backImg", this._backImg);
			}
			bool flag5 = this._readAndLoop != null;
			if (flag5)
			{
				args.SetObject("readAndLoop", this._readAndLoop);
			}
			bool flag6 = this._timeText != null;
			if (flag6)
			{
				args.SetObject("timeText", this._timeText);
			}
			GEvent.OnEvent(UiEvents.ReturnBottomTimeDisk, args);
		}
	}

	// Token: 0x060033F0 RID: 13296 RVA: 0x0019B7A8 File Offset: 0x001999A8
	private void PrepareForPracticeCombatSkill(Refers refers)
	{
		bool flag = null == refers;
		if (!flag)
		{
			refers.UserObject = new object();
			List<CImage> rotateImages = refers.CGetList<CImage>("Rotate_");
			bool flag2 = this._progressBarSubDefaultAngle == null;
			if (flag2)
			{
				this._progressBarSubDefaultAngle = new Vector3[]
				{
					rotateImages[0].rectTransform.parent.eulerAngles,
					rotateImages[1].rectTransform.parent.eulerAngles,
					rotateImages[2].rectTransform.parent.eulerAngles
				};
			}
			RectTransform position = refers.CGet<RectTransform>("AnimationPosition");
			CanvasGroup canvasGroup = refers.GetComponent<CanvasGroup>();
			canvasGroup.DOKill(false);
			canvasGroup.alpha = 0f;
			ResLoader.Load<GameObject>(string.Format("RemakeResources/Prefab/Legacy/Core/PracticeCombatSkill/practiceCombatSkill", Array.Empty<object>()), delegate(GameObject prefab)
			{
				GameObject root = Object.Instantiate<GameObject>(prefab, position);
				foreach (SkeletonGraphic graphic in root.GetComponentsInChildren<SkeletonGraphic>())
				{
					bool flag3 = graphic.skeletonDataAsset == null;
					if (flag3)
					{
						graphic.enabled = false;
					}
					else
					{
						graphic.color = graphic.color.SetAlpha(0f);
					}
				}
				AudioManager.Instance.PlaySound("ui_training", false, false);
			}, null, false);
			refers.CGet<TextMeshProUGUI>("WorkDesc").text = LocalStringManager.Get(LanguageKey.LK_PracticeCombatSkill_AnimationName);
			refers.CGet<TextMeshProUGUI>("ResourceCount").text = "0";
			List<CRawImage> progressBarMain = this.GetProgressBarMainList(refers);
			progressBarMain.ForEach(delegate(CRawImage bar)
			{
				bar.rectTransform.eulerAngles = Vector3.zero;
			});
			for (int i = 0; i < rotateImages.Count; i++)
			{
				rotateImages[i].rectTransform.parent.eulerAngles = this._progressBarSubDefaultAngle[i];
			}
			UIElement element = this.Element;
			element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(delegate()
			{
				this.StartCoroutine(this.CreatePracticeCombatSkillIcon(refers, rotateImages, progressBarMain));
			}));
		}
	}

	// Token: 0x060033F1 RID: 13297 RVA: 0x0019B9C8 File Offset: 0x00199BC8
	private IEnumerator CreatePracticeCombatSkillIcon(Refers refers, List<CImage> rotateImages, List<CRawImage> progressBarMain)
	{
		CRawImage treeRoot = base.CGet<CRawImage>("SkillCollect");
		int dropIconCount = this._proficiency;
		bool flag = dropIconCount > 10;
		if (flag)
		{
			dropIconCount = Mathf.Min(220, dropIconCount);
		}
		float resCollectTime = 2f;
		float generateGap = Mathf.Min(resCollectTime / (float)Math.Max(dropIconCount - 1, 1), 0.3f);
		WaitForSeconds wait = new WaitForSeconds(generateGap);
		bool singleMode = this._singleMode;
		if (singleMode)
		{
			yield return wait;
		}
		CanvasGroup canvasGroup = refers.GetComponent<CanvasGroup>();
		RectTransform position = refers.CGet<RectTransform>("AnimationPosition");
		SkeletonGraphic[] skeletonGraphics = position.GetComponentsInChildren<SkeletonGraphic>();
		foreach (SkeletonGraphic graphic in skeletonGraphics)
		{
			graphic.color = graphic.color.SetAlpha(0f);
			graphic.DOKill(false);
			graphic.DOFade(1f, this.FadeTime);
			graphic = null;
		}
		SkeletonGraphic[] array = null;
		canvasGroup.DOFade(1f, this.FadeTime);
		Transform fromPoint = treeRoot.transform.Find(string.Format("MoveFromDot_{0}", (int)refers.UserFloat));
		Transform target = treeRoot.transform.Find(string.Format("MoveToDot_{0}", (int)refers.UserFloat));
		bool flag2 = null != fromPoint && null != target;
		if (flag2)
		{
			DOVirtual.Float(0f, 1f, this.FadeTime, delegate(float stepValue)
			{
				refers.transform.localPosition = Vector3.Lerp(fromPoint.localPosition, target.localPosition, stepValue);
			}).SetAutoKill(true);
		}
		int num;
		for (int i = 0; i < progressBarMain.Count; i = num + 1)
		{
			CRawImage bar = progressBarMain[i];
			bar.rectTransform.DORotate(new Vector3(0f, 0f, -12f), resCollectTime + this.TextChangeDelayTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
			bar = null;
			num = i;
		}
		RectTransform holder = refers.CGet<RectTransform>("IconHolder");
		for (int j = 0; j < dropIconCount; j = num)
		{
			bool flag3 = !refers.gameObject.activeInHierarchy;
			if (flag3)
			{
				yield break;
			}
			GameObject dropIcon = this._resourceIconPool.GetObject();
			dropIcon.GetComponent<CImage>().SetSprite("mousetip_actualcombat_big", false, null);
			this._showingResIconCacheList[0].Add(dropIcon);
			dropIcon.transform.SetParent(holder, false);
			dropIcon.transform.localPosition = new Vector3((float)Random.Range(-150, 151), 290f, 0f);
			Rigidbody2D rigidBody = dropIcon.GetComponent<Rigidbody2D>();
			rigidBody.AddForce(new Vector2(Random.Range(-10f, 10f), 0f));
			dropIcon.gameObject.SetActive(true);
			for (int k = 0; k < rotateImages.Count; k = num + 1)
			{
				Transform rectTrans = rotateImages[k].rectTransform.parent;
				bool flag4 = !DOTween.IsTweening(rectTrans, true);
				if (flag4)
				{
					rectTrans.DORotate(rectTrans.eulerAngles - new Vector3(0f, 0f, Random.value * 5f + 5f), Random.value * 0.2f + 0.3f, RotateMode.Fast).SetEase(Ease.OutBounce).SetDelay(Random.value * 0.3f + 0.2f);
				}
				rectTrans = null;
				num = k;
			}
			yield return wait;
			refers.CGet<TextMeshProUGUI>("ResourceCount").text = (dropIconCount * j / dropIconCount).ToString();
			dropIcon = null;
			rigidBody = null;
			num = j + 1;
		}
		yield return wait;
		refers.CGet<TextMeshProUGUI>("ResourceCount").text = this._proficiency.ToString();
		yield return new WaitForSeconds(generateGap + this.FadeTime);
		refers.UserObject = null;
		yield break;
	}

	// Token: 0x060033F2 RID: 13298 RVA: 0x0019B9EC File Offset: 0x00199BEC
	public void InitForChoosy()
	{
		this.Init(null);
		this._isChoosy = true;
		CRawImage treeRoot = base.CGet<CRawImage>("SkillCollect");
		treeRoot.gameObject.SetActive(true);
		treeRoot.color = treeRoot.color.SetAlpha(1f);
		this._singleCollectRefers.gameObject.SetActive(false);
		int i = 0;
		int max = this._collectPrefabList.Count;
		while (i < max)
		{
			sbyte resourceType = (sbyte)i;
			Refers refers = this._collectPrefabList[i];
			refers.gameObject.SetActive(false);
			refers.CGet<TextMeshProUGUI>("ResourceCount").text = 0.ToString();
			ResourceTypeItem config = ResourceType.Instance.GetItem(resourceType);
			refers.CGet<TextMeshProUGUI>("WorkDesc").text = config.Name;
			i++;
		}
	}

	// Token: 0x060033F3 RID: 13299 RVA: 0x0019BACC File Offset: 0x00199CCC
	public void PlayResourceDropAnim(sbyte resourceType, int resourceAmount)
	{
		ref Coroutine cor = ref this._showingResIconCoroutineList[(int)resourceType];
		bool flag = cor != null;
		if (flag)
		{
			base.StopCoroutine(cor);
		}
		this.ClearResourceIcon(resourceType);
		Refers refers = this._collectPrefabList[(int)resourceType];
		bool flag2 = resourceAmount == 0;
		if (flag2)
		{
			refers.gameObject.SetActive(false);
		}
		else
		{
			refers.gameObject.SetActive(true);
			CollectResourceResult collectResult = new CollectResourceResult
			{
				ResourceType = resourceType,
				ResourceCount = resourceAmount
			};
			List<CImage> rotateImages;
			List<CRawImage> progressBarMain;
			this.PrepareForResourceCollectionWithoutSpine(refers, collectResult, out rotateImages, out progressBarMain);
			this.PrepareForResourceCollectionSpine(refers, collectResult, false, false);
			int choosyCount = resourceAmount / GlobalConfig.Instance.ChoosyResourceBaseCost;
			int dropCount = Math.Clamp(choosyCount, 0, 220);
			cor = base.StartCoroutine(this.CreateResourceIcon(refers, rotateImages, progressBarMain, collectResult, dropCount));
		}
	}

	// Token: 0x060033F4 RID: 13300 RVA: 0x0019BBA0 File Offset: 0x00199DA0
	private void StopAllResourceDropAnim()
	{
		for (int index = 0; index < this._showingResIconCoroutineList.Length; index++)
		{
			Coroutine coroutine = this._showingResIconCoroutineList[index];
			bool flag = coroutine != null;
			if (flag)
			{
				base.StopCoroutine(coroutine);
				this._showingResIconCoroutineList[index] = null;
			}
		}
	}

	// Token: 0x060033F5 RID: 13301 RVA: 0x0019BBEC File Offset: 0x00199DEC
	public void PlayResourceCollectAnim(sbyte resourceType, out float duration)
	{
		Refers refers = this._collectPrefabList[(int)resourceType];
		List<CRawImage> progressBarMain = this.GetProgressBarMainList(refers);
		float resCollectTime = this.GetCollectTime(refers);
		this.PlayResourceAnim(refers, progressBarMain, resCollectTime, false);
		duration = resCollectTime;
	}

	// Token: 0x040025BC RID: 9660
	private const float CollectAniTimeBase = 2f;

	// Token: 0x040025BD RID: 9661
	private readonly float FadeTime = 0.3f;

	// Token: 0x040025BE RID: 9662
	private readonly float TextChangeDelayTime = 0.5f;

	// Token: 0x040025BF RID: 9663
	public const int MaxDropResourceIconCount = 220;

	// Token: 0x040025C0 RID: 9664
	public const int FreezeCount = 81;

	// Token: 0x040025C1 RID: 9665
	private Vector3[] _progressBarSubDefaultAngle;

	// Token: 0x040025C2 RID: 9666
	public static readonly string[] ResourceTypeNames = new string[]
	{
		"food",
		"wood",
		"ore",
		"jewelry",
		"fabric",
		"herbal",
		"money"
	};

	// Token: 0x040025C3 RID: 9667
	private static readonly string[] _resourceSeNames = new string[]
	{
		"ui_collect_fhs",
		"ui_collect_wood",
		"ui_collect_gemstone",
		"ui_collect_gemstone",
		"ui_collect_fhs",
		"ui_collect_fhs",
		"ui_collect_coins"
	};

	// Token: 0x040025C4 RID: 9668
	private const string CollectAnimPrefabPathBase = "RemakeResources/Prefab/Legacy/Core/Gather/gather_{0}_{1}";

	// Token: 0x040025C5 RID: 9669
	private List<CollectResourceResult> _collectResults;

	// Token: 0x040025C6 RID: 9670
	private List<Refers> _collectPrefabList;

	// Token: 0x040025C7 RID: 9671
	private Refers _singleCollectRefers;

	// Token: 0x040025C8 RID: 9672
	private PoolItem _resourceIconPool;

	// Token: 0x040025C9 RID: 9673
	private PoolItem _itemIconPool;

	// Token: 0x040025CA RID: 9674
	private readonly List<GameObject>[] _showingResIconCacheList = new List<GameObject>[6];

	// Token: 0x040025CB RID: 9675
	private readonly Coroutine[] _showingResIconCoroutineList = new Coroutine[6];

	// Token: 0x040025CC RID: 9676
	private readonly List<GameObject> _showingItemIconCacheList = new List<GameObject>();

	// Token: 0x040025CD RID: 9677
	private bool _collecting;

	// Token: 0x040025CE RID: 9678
	private bool _singleMode;

	// Token: 0x040025CF RID: 9679
	private bool _collectResourceIsMax;

	// Token: 0x040025D0 RID: 9680
	private int _resourceType = -1;

	// Token: 0x040025D1 RID: 9681
	private bool _isChoosy;

	// Token: 0x040025D2 RID: 9682
	private Transform _timeDisk;

	// Token: 0x040025D3 RID: 9683
	private Transform _backImg;

	// Token: 0x040025D4 RID: 9684
	private Transform _readAndLoop;

	// Token: 0x040025D5 RID: 9685
	private Transform _timeText;

	// Token: 0x040025D6 RID: 9686
	private Transform _bottomRoot;

	// Token: 0x040025D7 RID: 9687
	private bool _isPracticeCombat;

	// Token: 0x040025D8 RID: 9688
	private int _proficiency;
}
