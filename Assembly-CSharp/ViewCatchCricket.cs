using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Common;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000163 RID: 355
public class ViewCatchCricket : UIBase
{
	// Token: 0x1700022D RID: 557
	// (get) Token: 0x06001376 RID: 4982 RVA: 0x0007752E File Offset: 0x0007572E
	private static string CatchCursor
	{
		get
		{
			return "ui9_icon_catchcricket_capturenet_0";
		}
	}

	// Token: 0x1700022E RID: 558
	// (get) Token: 0x06001377 RID: 4983 RVA: 0x00077535 File Offset: 0x00075735
	private static string CatchCursorDown
	{
		get
		{
			return "ui9_icon_catchcricket_capturenet_1";
		}
	}

	// Token: 0x1700022F RID: 559
	// (get) Token: 0x06001378 RID: 4984 RVA: 0x0007753C File Offset: 0x0007573C
	// (set) Token: 0x06001379 RID: 4985 RVA: 0x00077544 File Offset: 0x00075744
	public EMapAreaAreaDirection AreaType
	{
		get
		{
			return this._areaType;
		}
		set
		{
			this._areaType = value;
		}
	}

	// Token: 0x0600137A RID: 4986 RVA: 0x00077550 File Offset: 0x00075750
	public override void OnInit(ArgumentBox argsBox)
	{
		this._catching = false;
		this._timer = 0f;
		this.finalCatchResult = false;
		this._finalSceneShow = false;
		UIElement element = this.Element;
		element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(this.StartCatch));
		base.GetComponent<CanvasGroup>().alpha = 1f;
		this.clickMask.SetActive(true);
		this.catchPanel.SetActive(false);
		this.catchResult.SetActive(false);
		this.encyclopediaAreaRect.gameObject.SetActive(true);
		this.catchJarLight.gameObject.SetActive(false);
		this.findCricketTips.gameObject.SetActive(true);
		this.findCricketTips.overrideSorting = true;
		sbyte areaType;
		bool flag = argsBox != null && argsBox.Get("AreaType", out areaType);
		if (flag)
		{
			this._areaType = (EMapAreaAreaDirection)areaType;
		}
		short wishingCricketId;
		bool flag2 = argsBox != null && argsBox.Get("WishingCricketId", out wishingCricketId);
		if (flag2)
		{
			this._wishingCricketId = wishingCricketId;
		}
		else
		{
			this._wishingCricketId = -1;
		}
		ConchShipCursor.Instance.SetCursorImageWithKey("catch_cricket", ViewCatchCricket.CatchCursor, 0.1f, 0.1f);
		for (int i = 0; i < this.effHolder.childCount; i++)
		{
			this.effHolder.GetChild(i).gameObject.SetActive(i == (int)this._areaType);
		}
		Color color = this.blackMask.color;
		color.a = 1f;
		this.blackMask.color = color;
		UIElement element2 = this.Element;
		element2.OnShowed = (Action)Delegate.Combine(element2.OnShowed, new Action(delegate()
		{
			this.blackMask.DOFade(0f, 0.3f);
		}));
	}

	// Token: 0x0600137B RID: 4987 RVA: 0x00077716 File Offset: 0x00075916
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(5, 2, ulong.MaxValue, null));
	}

	// Token: 0x0600137C RID: 4988 RVA: 0x00077730 File Offset: 0x00075930
	private void Awake()
	{
		this.back.sortingOrder = 500;
		Vector2 placeInverval = new Vector2(14f, 7f);
		float halfSize = this.catchPlacePrefab.GetComponent<RectTransform>().sizeDelta.x / 2f;
		this._cellHeight = placeInverval.y + this.catchPlacePrefab.GetComponent<RectTransform>().sizeDelta.y / 2f;
		for (int row = 0; row < 3; row++)
		{
			for (int col = 0; col < 12; col++)
			{
				this._catchPlacePosRandomPool[row * 12 + col] = new Vector2((halfSize + placeInverval.x) * (float)col, -(halfSize + placeInverval.y) * 2f * (float)row - ((col % 2 == 0) ? 0f : (halfSize + placeInverval.y)));
			}
		}
		while (this.catchPlaceRoot.childCount < 21)
		{
			Object.Instantiate<GameObject>(this.catchPlacePrefab, this.catchPlaceRoot);
		}
		for (int j = 0; j < 21; j++)
		{
			int index = j;
			this._catchPlaceList[j] = new ViewCatchCricket.CricketPlaceInfo();
			CatchCricketPlace placeView = this.catchPlaceRoot.GetChild(j).GetComponent<CatchCricketPlace>();
			placeView.Init(index, new Action<int>(this.OnClickCatchPlace), new Action<CatchCricketPlace>(this.OnHoverEnter), new Action<CatchCricketPlace>(this.OnHoverExit));
			this._catchPlaceList[j].PlaceView = placeView;
			AudioManager.Instance.AddAudioSource(placeView.PlaceAudioSource, 0f);
		}
		ResLoader.LoadModOrGameResource<Texture2D>("CatchCricket_2/" + this.GetAreaTypeName() + "_cricket_interface", delegate(Texture2D texture)
		{
			this.interfaceImg.texture = texture;
		}, null);
		this.timeoutConfirmBtn.onClick.AddListener(new UnityAction(this.OnTimeoutConfirmClick));
		this.wuqiHolder.gameObject.SetActive(true);
		int i;
		int i2;
		for (i = 0; i < 6; i = i2 + 1)
		{
			ResLoader.LoadModOrGameResource<Texture2D>(string.Format("CatchCricket_2/{0}_wuqi_{1}", this.GetAreaTypeName(), i + 1), delegate(Texture2D texture)
			{
				this.wuqiHolder.GetChild(this.wuqiHolder.childCount - i - 1).GetComponent<RawImage>().texture = texture;
			}, null);
			i2 = i;
		}
	}

	// Token: 0x0600137D RID: 4989 RVA: 0x0007799C File Offset: 0x00075B9C
	private void UpdateCatchPlaceLocation()
	{
		for (int i = 0; i < this._catchPlaceList.Length; i++)
		{
			CatchCricketPlace placeView = this._catchPlaceList[i].PlaceView;
			int index = (int)Math.Abs(placeView.RectTransform.anchoredPosition.y / this._cellHeight);
			placeView.RectTransform.SetParent(this.wuqiHolder.GetChild(index), true);
		}
	}

	// Token: 0x0600137E RID: 4990 RVA: 0x00077A08 File Offset: 0x00075C08
	private string GetAreaTypeName()
	{
		EMapAreaAreaDirection areaType = this._areaType;
		if (!true)
		{
		}
		string result;
		switch (areaType)
		{
		case EMapAreaAreaDirection.South:
			result = "South";
			break;
		case EMapAreaAreaDirection.North:
			result = "North";
			break;
		case EMapAreaAreaDirection.West:
			result = "West";
			break;
		default:
			result = "North";
			break;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x0600137F RID: 4991 RVA: 0x00077A5E File Offset: 0x00075C5E
	private void OnEnable()
	{
		GEvent.Add(EEvents.OnConfirmQuitGameState, new GEvent.Callback(this.OnConfirmQuitGameState));
	}

	// Token: 0x06001380 RID: 4992 RVA: 0x00077A7C File Offset: 0x00075C7C
	private void OnDisable()
	{
		GEvent.Remove(EEvents.OnConfirmQuitGameState, new GEvent.Callback(this.OnConfirmQuitGameState));
		SingletonObject.getInstance<WorldMapModel>().UpdateBgm();
		for (int i = 0; i < this.jar.childCount; i++)
		{
			this.jar.GetChild(i).GetComponent<CricketView>().StopLoopSing();
		}
		TaiwuEventDomainMethod.Call.MainStoryFinishCatchCricket(this.finalCatchResult);
		TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg("CricketCatchOver", EventTriggerParameter.DefValue.CricketCatchSuccess.ArgBoxKey, this.finalCatchResult);
		TaiwuEventDomainMethod.Call.TriggerListener("CricketCatchOver", true);
	}

	// Token: 0x06001381 RID: 4993 RVA: 0x00077B14 File Offset: 0x00075D14
	private void Update()
	{
		bool catching = this._catching;
		if (catching)
		{
			bool flag = UIManager.Instance.IsFocusElement(this.Element);
			if (flag)
			{
				foreach (ViewCatchCricket.CricketPlaceInfo place in this._catchPlaceList)
				{
					this.UpdateCricketSing(place);
				}
				this.UpdateTime();
				bool isInEncyclopediaArea = RectTransformUtility.RectangleContainsScreenPoint(this.encyclopediaAreaRect, Input.mousePosition, UIManager.Instance.UiCamera);
				bool flag2 = this._inEncyclopediaArea != isInEncyclopediaArea;
				if (flag2)
				{
					bool flag3 = isInEncyclopediaArea;
					if (flag3)
					{
						ConchShipCursor.Instance.SetDefaultCursorAndReleaseKey("catch_cricket");
					}
					else
					{
						ConchShipCursor.Instance.SetCursorImageWithKey("catch_cricket", ViewCatchCricket.CatchCursor, 0.1f, 0.1f);
					}
					this._inEncyclopediaArea = isInEncyclopediaArea;
				}
				bool flag4 = !this._inEncyclopediaArea;
				if (flag4)
				{
					bool keyDown = Input.GetKeyDown(KeyCode.Mouse0);
					if (keyDown)
					{
						ConchShipCursor.Instance.SetCursorImageWithKey("catch_cricket", ViewCatchCricket.CatchCursorDown, 0.5f, 0f);
					}
					else
					{
						bool keyUp = Input.GetKeyUp(KeyCode.Mouse0);
						if (keyUp)
						{
							ConchShipCursor.Instance.SetCursorImageWithKey("catch_cricket", ViewCatchCricket.CatchCursor, 0.1f, 0.1f);
						}
					}
				}
			}
		}
		bool finalSceneShow = this._finalSceneShow;
		if (finalSceneShow)
		{
			float delay = 3f;
			bool flag5 = this._timeToActivate == 0f;
			if (flag5)
			{
				this._timeToActivate = Time.time + delay;
			}
			bool flag6 = Time.time >= this._timeToActivate;
			if (flag6)
			{
				bool flag7 = !this.hotkeyDisplay.activeSelf;
				if (flag7)
				{
					this.hotkeyDisplay.SetActive(true);
				}
				bool flag8 = CommonCommandKit.LeftMouse.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false) || CommonCommandKit.Space.Check(this.Element, false, false, false, true, false);
				if (flag8)
				{
					this._finalSceneShow = false;
					this._timeToActivate = 0f;
					UIManager.Instance.HideUI(this.Element);
				}
			}
		}
	}

	// Token: 0x06001382 RID: 4994 RVA: 0x00077D48 File Offset: 0x00075F48
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b == 1)
				{
					bool flag = notification.DomainId == 6 && notification.MethodId == 1;
					if (flag)
					{
						Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._catchResult);
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag2 = uid.DomainId == 5 && uid.DataId == 2;
				if (flag2)
				{
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._luckPoint);
					this.InitCatchPlace();
					base.RemoveMonitorFieldId(5, 2);
				}
			}
		}
	}

	// Token: 0x06001383 RID: 4995 RVA: 0x00077E44 File Offset: 0x00076044
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
	}

	// Token: 0x06001384 RID: 4996 RVA: 0x00077E5C File Offset: 0x0007605C
	private void InitCatchPlace()
	{
		int kingPlaceIndex = Random.Range(0, 21);
		List<Vector2> posRandomPool = new List<Vector2>(this._catchPlacePosRandomPool);
		Dictionary<short, int> partRandomPool = new Dictionary<short, int>();
		Dictionary<short, int> kingRandomPool = new Dictionary<short, int>();
		Dictionary<sbyte, int> placeRandomPool = new Dictionary<sbyte, int>();
		KeyValuePair<ECricketPartsType, int>[] colorWeights = new KeyValuePair<ECricketPartsType, int>[7];
		foreach (CricketPartsItem cricketConfig in ((IEnumerable<CricketPartsItem>)CricketParts.Instance))
		{
			bool flag = cricketConfig.Type == ECricketPartsType.Parts;
			if (flag)
			{
				partRandomPool.Add(cricketConfig.TemplateId, (int)cricketConfig.Rate);
			}
			else
			{
				bool flag2 = cricketConfig.Type == ECricketPartsType.King;
				if (flag2)
				{
					kingRandomPool.Add(cricketConfig.TemplateId, (int)cricketConfig.Rate);
				}
			}
		}
		foreach (CricketPlaceItem placeConfig in ((IEnumerable<CricketPlaceItem>)CricketPlace.Instance))
		{
			placeRandomPool.Add(placeConfig.TemplateId, (int)placeConfig.PlaceRate);
		}
		int[] placeIconOptions = (from _ in new int[]
		{
			0,
			1,
			2
		}
		orderby Random.value
		select _).Take(2).ToArray<int>();
		for (int i = 0; i < 21; i++)
		{
			ViewCatchCricket.CricketPlaceInfo placeInfo = this._catchPlaceList[i];
			CatchCricketPlace placeView = placeInfo.PlaceView;
			CImage placeImg = placeView.PlaceImg;
			CImage blockImg = placeView.BlockImg;
			RectTransform singImg = placeView.CricketSingImage;
			sbyte placeId = Utils_Random.GetRandomResult<sbyte>(placeRandomPool);
			CricketPlaceItem placeConfig2 = CricketPlace.Instance[placeId];
			int posIndex = Random.Range(0, posRandomPool.Count);
			placeView.RectTransform.SetParent(this.catchPlaceRoot);
			placeView.RectTransform.anchoredPosition = posRandomPool[posIndex];
			posRandomPool.RemoveAt(posIndex);
			placeImg.SetSprite(placeConfig2.Icon[(int)this._areaType][placeIconOptions[Random.Range(0, placeIconOptions.Length)]], false, null);
			placeView.ResetVisual();
			blockImg.SetSprite(this.GetAreaTypeName() + "_cricket_plot_" + Random.Range(1, 3).ToString(), false, null);
			placeInfo.PlaceId = placeId;
			placeInfo.DelayTime = 0f;
			placeInfo.SingCount = 0;
			placeInfo.SingLevel = 0;
			placeView.CricketSingImage.localScale = Vector3.forward;
			colorWeights[0] = new KeyValuePair<ECricketPartsType, int>(ECricketPartsType.Cyan, (int)placeConfig2.Cyan);
			colorWeights[1] = new KeyValuePair<ECricketPartsType, int>(ECricketPartsType.Yellow, (int)placeConfig2.Yellow);
			colorWeights[2] = new KeyValuePair<ECricketPartsType, int>(ECricketPartsType.Purple, (int)placeConfig2.Purple);
			colorWeights[3] = new KeyValuePair<ECricketPartsType, int>(ECricketPartsType.Red, (int)placeConfig2.Red);
			colorWeights[4] = new KeyValuePair<ECricketPartsType, int>(ECricketPartsType.Black, (int)placeConfig2.Black);
			colorWeights[5] = new KeyValuePair<ECricketPartsType, int>(ECricketPartsType.White, (int)placeConfig2.White);
			colorWeights[6] = new KeyValuePair<ECricketPartsType, int>(ECricketPartsType.Trash, (int)placeConfig2.Trash);
			ECricketPartsType baseColorType = Utils_Random.GetRandomResult<ECricketPartsType>(colorWeights);
			bool flag3 = baseColorType == ECricketPartsType.Trash;
			if (flag3)
			{
				placeInfo.CricketColorId = 0;
				placeInfo.CricketPartsId = 0;
				placeInfo.CricketLevel = CricketParts.Instance[0].Level;
			}
			else
			{
				IEnumerable<KeyValuePair<short, int>> colorsInfo = from configData in CricketParts.Instance
				where configData.Type == baseColorType
				select new KeyValuePair<short, int>(configData.TemplateId, (int)configData.Rate);
				short colorId = Utils_Random.GetRandomResult<short>(colorsInfo);
				bool flag4 = i == kingPlaceIndex && this._wishingCricketId >= 0;
				if (flag4)
				{
					placeInfo.CricketColorId = this._wishingCricketId;
					placeInfo.CricketPartsId = 0;
					placeInfo.CricketLevel = CricketParts.Instance[this._wishingCricketId].Level;
				}
				else
				{
					bool flag5 = i == kingPlaceIndex && Utils_Random.RandomCheck((int)CricketParts.Instance[colorId].AdvanceRate * this._luckPoint / 100, 100);
					if (flag5)
					{
						short cricketKingId = Utils_Random.RandomCheck(this._luckPoint / 10, 100) ? Utils_Random.GetRandomResult<short>(kingRandomPool) : ViewCatchCricket.Color2RealColor[baseColorType];
						placeInfo.CricketColorId = cricketKingId;
						placeInfo.CricketPartsId = 0;
						placeInfo.CricketLevel = CricketParts.Instance[cricketKingId].Level;
					}
					else
					{
						short partId = Utils_Random.GetRandomResult<short>(partRandomPool);
						placeInfo.CricketColorId = colorId;
						placeInfo.CricketPartsId = partId;
						placeInfo.CricketLevel = (sbyte)Mathf.Max((int)CricketParts.Instance[colorId].Level, (int)CricketParts.Instance[partId].Level);
					}
				}
			}
			placeInfo.SingPitch = CricketParts.Instance[placeInfo.CricketColorId].SingPitch + ((placeInfo.CricketPartsId > 0) ? CricketParts.Instance[placeInfo.CricketPartsId].SingPitch : 0);
			placeInfo.SingSize = CricketParts.Instance[placeInfo.CricketColorId].SingSize + ((placeInfo.CricketPartsId > 0) ? CricketParts.Instance[placeInfo.CricketPartsId].SingSize : 0);
		}
		foreach (ValueTuple<int, float, short> valueTuple in CatchUtils.RandomGroups(kingPlaceIndex))
		{
			int index = valueTuple.Item1;
			float singTime = valueTuple.Item2;
			short singCount = valueTuple.Item3;
			ViewCatchCricket.CricketPlaceInfo placeInfo2 = this._catchPlaceList[index];
			placeInfo2.SingStartTime = singTime;
			placeInfo2.SingCount = singCount;
			bool flag6 = index == kingPlaceIndex && this._wishingCricketId >= 0 && placeInfo2.SingCount == 0;
			if (flag6)
			{
				placeInfo2.SingCount = 1;
			}
		}
		this.UpdateTime();
		this.UpdateCatchPlaceLocation();
		this.Element.ShowAfterRefresh();
	}

	// Token: 0x06001385 RID: 4997 RVA: 0x0007847C File Offset: 0x0007667C
	private void OnHoverEnter(CatchCricketPlace placeView)
	{
		placeView.HoverImg.gameObject.SetActive(true);
	}

	// Token: 0x06001386 RID: 4998 RVA: 0x00078491 File Offset: 0x00076691
	private void OnHoverExit(CatchCricketPlace placeView)
	{
		placeView.HoverImg.gameObject.SetActive(false);
	}

	// Token: 0x06001387 RID: 4999 RVA: 0x000784A8 File Offset: 0x000766A8
	private void StartCatch()
	{
		this._catching = true;
		this.catchResult.SetActive(false);
		this.timeoutResultPage.gameObject.SetActive(false);
		this.catchSuccess.SetActive(false);
		this.catchFail.SetActive(false);
		this.findCricketTips.overrideSorting = true;
		this.clickMask.SetActive(false);
		AudioManager.Instance.PlayMusic("catch_cricket", 1f, 100, null);
		AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 1f, 100);
		AudioManager.Instance.PlaySound("CCricket_BG", false, false);
		bool flag = UIManager.Instance.IsFocusElement(this.Element);
		if (flag)
		{
			bool key = Input.GetKey(KeyCode.Mouse0);
			if (key)
			{
				ConchShipCursor.Instance.SetCursorImageWithKey("catch_cricket", ViewCatchCricket.CatchCursorDown, 0.5f, 0f);
			}
			else
			{
				ConchShipCursor.Instance.SetCursorImageWithKey("catch_cricket", ViewCatchCricket.CatchCursor, 0.1f, 0.1f);
			}
		}
	}

	// Token: 0x06001388 RID: 5000 RVA: 0x000785B8 File Offset: 0x000767B8
	private void UpdateTime()
	{
		bool firstCountdown = this._timer < 20f;
		this._timer += Time.deltaTime;
		int countdown = Mathf.Max(0, 30 - (int)this._timer);
		this.UpdateCountdown(countdown);
		bool flag = !firstCountdown && this._timer >= 30f;
		if (flag)
		{
			this.FinishCatch();
			DG.Tweening.Sequence seq = DOTween.Sequence();
			seq.AppendInterval(0.6f);
			seq.AppendCallback(delegate
			{
				this.catchResult.SetActive(true);
				this.hotkeyDisplay.SetActive(false);
				this.findCricketTips.gameObject.SetActive(false);
				this.timeoutResultPage.gameObject.SetActive(true);
			});
			seq.Play<DG.Tweening.Sequence>();
		}
	}

	// Token: 0x06001389 RID: 5001 RVA: 0x00078650 File Offset: 0x00076850
	private void UpdateCountdown(int countdown)
	{
		ViewCatchCricket.<>c__DisplayClass96_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		bool flag = countdown == this._countdown;
		if (!flag)
		{
			this._countdown = countdown;
			CS$<>8__locals1.doubleDigits = countdown / 10;
			CS$<>8__locals1.singeDigits = countdown % 10;
			CS$<>8__locals1.isEnd = (this._countdown <= 15);
			this.<UpdateCountdown>g__SetDigits|96_0(this.countdownLayout, ref CS$<>8__locals1);
			this.<UpdateCountdown>g__SetDigits|96_0(this.countdownEffectLayout, ref CS$<>8__locals1);
			CanvasGroup canvasGroup = this.countdownEffectLayout.GetComponent<CanvasGroup>();
			canvasGroup.alpha = 1f;
			canvasGroup.DOKill(false);
			this.countdownLayout.localScale = Vector3.one;
			this.countdownEffectLayout.localScale = Vector3.one;
			this.countdownEffectLayout.DOKill(false);
			bool isEnd = CS$<>8__locals1.isEnd;
			if (isEnd)
			{
				canvasGroup.DOFade(0f, 0.4f);
				this.countdownEffectLayout.DOScale(3f, 0.4f);
			}
		}
	}

	// Token: 0x0600138A RID: 5002 RVA: 0x0007874C File Offset: 0x0007694C
	private void UpdateCricketSing(ViewCatchCricket.CricketPlaceInfo place)
	{
		bool flag = place.SingLevel > 0;
		if (flag)
		{
			float singTotalTime = place.SingTotalTime + ((place.SingLevel >= 80) ? CatchUtils.BaseSingTimeMin : 0f);
			place.SingTimer += Time.deltaTime;
			bool flag2 = this._timer / 30f > 0.8f && place.LoudSingTimer <= 0f;
			if (flag2)
			{
				place.SingTimer = 0f;
				this.UpdateCricketSingVolume(place);
				place.SingLevel = 80;
				this.ShowCricketSingImage(place);
				place.LoudSingTimer = 0.1f;
			}
			else
			{
				bool flag3 = place.SingTimer >= singTotalTime;
				if (flag3)
				{
					bool flag4 = place.SingLevel >= 80;
					if (flag4)
					{
						bool flag5 = Utils_Random.RandomCheck((int)place.CricketLevel, 100);
						if (flag5)
						{
							place.SingLevel = 0;
						}
						else
						{
							bool flag6 = Utils_Random.RandomCheck((place.SingCount <= 0) ? 40 : 30, 100);
							if (flag6)
							{
								place.SingLevel = (short)Mathf.Clamp((int)(place.SingLevel + (short)(10 + place.CricketLevel * 2)), 0, 100);
							}
							else
							{
								place.SingLevel = (short)Mathf.Clamp((int)(place.SingLevel - (short)(10 + place.CricketLevel * 2)), 0, 100);
							}
						}
					}
					else
					{
						bool flag7 = Utils_Random.RandomCheck((int)place.CricketLevel, 100);
						if (flag7)
						{
							place.SingLevel = 80;
						}
						else
						{
							bool flag8 = Utils_Random.RandomCheck((place.SingCount <= 0) ? 25 : 35, 100);
							if (flag8)
							{
								place.SingLevel = (short)Mathf.Clamp((int)(place.SingLevel - (short)(10 + place.CricketLevel * 2)), 0, 100);
							}
							else
							{
								place.SingLevel = (short)Mathf.Clamp((int)(place.SingLevel + (short)(10 + place.CricketLevel * 2)), 0, 100);
							}
						}
					}
					place.SingTimer = 0f;
					this.UpdateCricketSingVolume(place);
					bool flag9 = place.SingLevel > 0;
					if (flag9)
					{
						this.ShowCricketSingImage(place);
					}
				}
				else
				{
					bool flag10 = place.SingLevel >= 80;
					if (flag10)
					{
						bool flag11 = place.LoudSingTimer >= place.SingTotalTime / 2f;
						if (flag11)
						{
							this.ShowCricketSingImage(place);
							place.LoudSingTimer = 0f;
						}
						place.LoudSingTimer += Time.deltaTime;
					}
				}
			}
		}
		else
		{
			bool flag12 = place.DelayTime > 0f;
			if (flag12)
			{
				place.DelayTime -= Time.deltaTime;
			}
			else
			{
				bool flag13 = this._timer >= place.SingStartTime && place.SingCount > 0;
				if (flag13)
				{
					place.SingCount -= 1;
					place.SingLevel = (short)Random.Range((int)((place.CricketLevel + 1) * 2), (int)((place.CricketLevel + 1) * 4));
					place.SingTotalTime = CatchUtils.RandomSingTime + (float)place.CricketLevel * GlobalConfig.Instance.CricketSingGradeTime;
					place.DelayTime = CatchUtils.RandomDelayTime;
					place.SingTimer = 0f;
					place.LoudSingTimer = 0f;
					this.UpdateCricketSingVolume(place);
					this.ShowCricketSingImage(place);
				}
			}
		}
	}

	// Token: 0x0600138B RID: 5003 RVA: 0x00078A7C File Offset: 0x00076C7C
	private void UpdateCricketSingVolume(ViewCatchCricket.CricketPlaceInfo place)
	{
		float maxVolume = (float)SingletonObject.getInstance<GlobalSettings>().SeVolume / 100f;
		float volume = (place.SingLevel > 0) ? (maxVolume * 0.4f + maxVolume * 0.6f * (float)place.SingLevel / 100f - (float)(9 - place.CricketLevel) * 0.025f) : 0f;
		AudioSource audioSource = place.PlaceView.PlaceAudioSource;
		volume = ((SingletonObject.getInstance<GlobalSettings>().SeOn && !AudioManager.Instance.GetShouldMuteIfFocus()) ? Mathf.Clamp(volume, 0f, 1f) : 0f);
		audioSource.DOKill(false);
		audioSource.DOFade(volume, 0.1f);
		audioSource.pitch = 1.25f - 0.65f * (float)place.SingPitch / 100f;
		AudioManager.Instance.AddAudioSource(audioSource, volume);
	}

	// Token: 0x0600138C RID: 5004 RVA: 0x00078B5C File Offset: 0x00076D5C
	private void ShowCricketSingImage(ViewCatchCricket.CricketPlaceInfo place)
	{
		float t = (float)place.SingLevel / 100f;
		float loadFactor = (place.SingLevel >= 80) ? 1f : 0.5f;
		float singLevelFactor = (0.2f + 0.8f * t) * loadFactor;
		float singSizeTerm = 0.2f + 0.8f * (float)place.SingSize / 100f;
		float aniTime = (place.SingLevel >= 80) ? (place.SingTotalTime / 2f) : place.SingTotalTime;
		int shakePower = (place.SingLevel >= 80) ? 20 : 10;
		CImage placeImg = place.PlaceView.PlaceImg;
		RectTransform singImg = place.PlaceView.CricketSingImage;
		AudioSource sudioSource = place.PlaceView.PlaceAudioSource;
		place.PlaceView.IsShaking = true;
		singImg.DOKill(false);
		singImg.localScale = Vector3.forward;
		float finalScale = Mathf.Max(0.15f, singLevelFactor * singSizeTerm);
		singImg.DOScale(finalScale, aniTime);
		singImg.GetComponent<CImage>().color = Color.white;
		singImg.GetComponent<CImage>().DOFade(0f, aniTime / 2f).SetDelay(aniTime / 2f);
		placeImg.rectTransform.DOShakeRotation((place.SingLevel >= 80) ? 0.1f : 0.2f, new Vector3(0f, 0f, (float)(shakePower + shakePower * (int)place.CricketLevel * 10 / 100)), 10, 90f, true, ShakeRandomnessMode.Full).OnComplete(delegate
		{
			placeImg.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
			place.PlaceView.IsShaking = false;
		});
		bool flag = !sudioSource.isPlaying;
		if (flag)
		{
			sudioSource.Play();
		}
	}

	// Token: 0x0600138D RID: 5005 RVA: 0x00078D58 File Offset: 0x00076F58
	private void StopSing(ViewCatchCricket.CricketPlaceInfo place)
	{
		AudioSource audioSource = place.PlaceView.PlaceAudioSource;
		audioSource.DOKill(false);
		bool isPlaying = audioSource.isPlaying;
		if (isPlaying)
		{
			audioSource.DOFade(0f, 0.1f).SetDelay(0.2f).OnComplete(new TweenCallback(audioSource.Stop));
		}
	}

	// Token: 0x0600138E RID: 5006 RVA: 0x00078DB0 File Offset: 0x00076FB0
	private void OnClickCatchPlace(int index)
	{
		ViewCatchCricket.CricketPlaceInfo place = this._catchPlaceList[index];
		CricketPlaceItem placeConfig = CricketPlace.Instance[place.PlaceId];
		this._clickedPlaceIndex = index;
		this.FinishCatch();
		int poetryIndex = Random.Range(0, 5);
		SkeletonGraphic catchAniGraphic = this.catchAniGraphic;
		SkeletonAnimation catchAniBone = this.catchAniForBoneFollower;
		this.catchBack.SetTexture(placeConfig.CatchAniBack);
		catchAniBone.skeletonDataAsset = (catchAniGraphic.skeletonDataAsset = this.catchAniRefers.CGet<SkeletonDataAsset>(placeConfig.CatchAni));
		catchAniGraphic.Initialize(true);
		catchAniGraphic.AnimationState.SetAnimation(0, "idle", true);
		catchAniGraphic.AnimationState.SetAnimation(1, "catch", false);
		catchAniGraphic.AnimationState.Event -= this.OnCatchJarDrop;
		catchAniGraphic.AnimationState.Event += this.OnCatchJarDrop;
		catchAniBone.Initialize(true, false);
		catchAniBone.AnimationState.SetAnimation(0, "idle", true);
		catchAniBone.AnimationState.SetAnimation(1, "catch", false);
		this.catchPoetryImage.SetTexture(string.Format("{0}{1}", "ui9_tex_catchcricket_catch_poetry_", poetryIndex));
		this.catchPoemEnText.text = LocalStringManager.Get(string.Format("LK_Catch_Cricket_Poem_{0}", poetryIndex));
		AudioManager.Instance.PlaySound("ui_cacthcricket_pre", false, false);
		this.catchJar.SetAlpha(0f);
		this.catchJarEffectEarth.gameObject.SetActive(false);
		this.catchJarEffectWater.gameObject.SetActive(false);
		this.catchJarLight.gameObject.SetActive(false);
		this.catchJarLightSpread.gameObject.SetActive(false);
		this.catchPanel.SetActive(true);
		this.findCricketTips.overrideSorting = false;
		CricketPartsItem cricketPartsItem = CricketParts.Instance[place.CricketColorId];
		int a = (int)((cricketPartsItem != null) ? cricketPartsItem.Level : 0);
		int b;
		if (place.CricketPartsId <= 0)
		{
			b = 0;
		}
		else
		{
			CricketPartsItem cricketPartsItem2 = CricketParts.Instance[place.CricketPartsId];
			b = (int)((cricketPartsItem2 != null) ? cricketPartsItem2.Level : 0);
		}
		int cricketLevel = Mathf.Max(a, b);
		bool isLoudVisual = place.SingLevel >= 80;
		int successOdds = (int)(10 + place.SingLevel) - Mathf.Min(cricketLevel * 5, 40);
		int num = isLoudVisual ? successOdds : (successOdds / 2);
		ItemDomainMethod.Call.CatchCricket(this.Element.GameDataListenerId, place.CricketColorId, place.CricketPartsId, place.SingLevel, place.PlaceId);
	}

	// Token: 0x0600138F RID: 5007 RVA: 0x00079034 File Offset: 0x00077234
	private void FinishCatch()
	{
		this._catching = false;
		this.clickMask.SetActive(true);
		this.encyclopediaAreaRect.gameObject.SetActive(false);
		AudioManager.Instance.StopSound("CCricket_BG");
		foreach (ViewCatchCricket.CricketPlaceInfo catchPlace in this._catchPlaceList)
		{
			this.StopSing(catchPlace);
		}
		ConchShipCursor.Instance.SetDefaultCursorAndReleaseKey("catch_cricket");
	}

	// Token: 0x06001390 RID: 5008 RVA: 0x000790AC File Offset: 0x000772AC
	private void OnCatchJarDrop(TrackEntry trackEntry, Spine.Event aniEvent)
	{
		ViewCatchCricket.CricketPlaceInfo place = this._catchPlaceList[this._clickedPlaceIndex];
		ParticleSystem dropEffect = (place.PlaceId == 3) ? this.catchJarEffectWater : this.catchJarEffectEarth;
		ParticleSystem lightEffect = this.catchJarLight;
		ParticleSystem lightSpreadEffect = this.catchJarLightSpread;
		DG.Tweening.Sequence seq = DOTween.Sequence();
		this.catchJar.SetAlpha(1f);
		dropEffect.gameObject.SetActive(true);
		dropEffect.Play(true);
		this.whiteLight.SetAlpha(0f);
		lightEffect.gameObject.SetActive(false);
		seq.AppendInterval(1f);
		seq.AppendCallback(delegate
		{
			dropEffect.gameObject.SetActive(false);
			lightSpreadEffect.gameObject.SetActive(true);
			lightSpreadEffect.Play();
		});
		seq.AppendCallback(delegate
		{
			lightEffect.gameObject.SetActive(true);
			lightEffect.Play(true);
		});
		seq.AppendInterval(1.7f);
		seq.AppendCallback(delegate
		{
			this.catchPanel.SetActive(false);
			bool flag = this._catchResult.Count == 0 || this._catchResult[0].Key.ItemType == 11;
			if (flag)
			{
				this._supriseShowed = false;
				this.ShowCatchResult();
			}
			else
			{
				this.GetComponent<CanvasGroup>().alpha = 0f;
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("ItemList", this._catchResult);
				argBox.Set("IntoWarehouse", false);
				UIElement.GetItem.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.GetItem);
			}
		});
		seq.AppendCallback(delegate
		{
			bool flag = this._catchResult.Count > 0 && this._catchResult[0].Key.ItemType != 11;
			if (flag)
			{
				UIManager.Instance.HideUI(this.Element);
			}
		});
		seq.PlayForward();
	}

	// Token: 0x06001391 RID: 5009 RVA: 0x000791D0 File Offset: 0x000773D0
	private void ShowCatchResult()
	{
		this._timeToActivate = 0f;
		bool success = this._catchResult.Count > 0 && this._catchResult[0].Key.ItemType == 11;
		bool showSuccess = success && (this._supriseShowed || Utils_Random.RandomCheck(99, 100));
		ViewCatchCricket.CricketPlaceInfo place = this._catchPlaceList[this._clickedPlaceIndex];
		List<ItemDisplayData> list = this._catchResult;
		bool wishingFailed = list == null || list.Count <= 0 || this._catchResult.All((ItemDisplayData x) => x.CricketColorId != this._wishingCricketId);
		bool flag = wishingFailed && this._wishingCricketId >= 0;
		if (flag)
		{
			TaiwuDomainMethod.Call.CricketWishingCricketReturnLuckPoint();
		}
		this._wishingCricketId = -1;
		this.catchSuccess.SetActive(showSuccess);
		this.catchFail.SetActive(!showSuccess);
		this.catchResult.SetActive(true);
		this.hotkeyDisplay.SetActive(false);
		bool flag2 = showSuccess;
		if (flag2)
		{
			string[] poem = this.SplitPoem(CricketParts.Instance[(place.CricketLevel < 7) ? place.CricketPartsId : place.CricketColorId].Desc);
			int effectIndex = (place.CricketLevel < 3) ? 0 : ((place.CricketLevel < 6) ? 1 : ((place.CricketLevel < 8) ? 2 : 3));
			RectTransform cricketJar = this.jar;
			RectTransform effectHolder = this.effectHolder;
			this.UpdateCricketInJar(cricketJar.GetChild(0).GetComponent<CricketView>(), (this._catchResult.Count == 1) ? 20f : -140f, this._catchResult[0]);
			cricketJar.GetChild(1).gameObject.SetActive(this._catchResult.Count > 1);
			bool flag3 = this._catchResult.Count > 1;
			if (flag3)
			{
				CricketView secondCricket = cricketJar.GetChild(1).GetComponent<CricketView>();
				float scale = (this._catchResult[1].Key.ItemType == 11) ? 0.7f : 0.56f;
				secondCricket.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1f);
				bool flag4 = this._catchResult[1].Key.ItemType == 11;
				if (flag4)
				{
					this.UpdateCricketInJar(secondCricket, 180f, this._catchResult[1]);
				}
				else
				{
					bool flag5 = this._catchResult[1].Key.ItemType == 12;
					if (flag5)
					{
						RectTransform cricketTransform = secondCricket.GetComponent<RectTransform>();
						TooltipInvoker tipDisplayer = secondCricket.GetComponent<TooltipInvoker>();
						secondCricket.SetCricketData(0, 0, true, null, true);
						secondCricket.StopLoopSing();
						secondCricket.PlayAnimation(ECricketAnim.Idle, true, false);
						cricketTransform.anchoredPosition = new Vector2(180f, 0f);
						cricketTransform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
						secondCricket.GetComponent<CEmptyGraphic>().enabled = true;
						tipDisplayer.RuntimeParam = null;
						tipDisplayer.PresetParam = new string[]
						{
							Misc.Instance[this._catchResult[1].Key.TemplateId].Name
						};
						tipDisplayer.enabled = true;
						tipDisplayer.Refresh(false, -1);
					}
				}
			}
			for (int i = 0; i < effectHolder.childCount; i++)
			{
				effectHolder.GetChild(i).gameObject.SetActive(i == effectIndex);
			}
			bool flag6 = effectIndex >= 2;
			if (flag6)
			{
				AudioManager.Instance.PlaySound("ui_cacthcricket_rare", false, false);
			}
			CricketPartsItem cricketPart = CricketParts.Instance[(place.CricketLevel < 7) ? place.CricketPartsId : place.CricketColorId];
			this.cricketPoemImage.gameObject.SetActive(true);
			this.cricketPoemBackImage.gameObject.SetActive(true);
			this.cricketPoemImage.SetTexture(cricketPart.PoetryTexture);
			bool isPoem2Line = cricketPart.PoetryTextureLength == 1;
			Texture2D backTexture = isPoem2Line ? this.cricketPoemBack2 : this.cricketPoemBack4;
			this.cricketPoemBackImage.texture = backTexture;
			Vector2 backSize = isPoem2Line ? ViewCatchCricket.PoemBackSize2 : ViewCatchCricket.PoemBackSize4;
			this.cricketPoemBackImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backSize.x);
			this.cricketPoemBackImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, backSize.y);
			this.cricketNameImage.SetTexture(cricketPart.NameTexture);
			this.cricketPoemEnText.text = cricketPart.Desc;
			this.cricketNameText.text = new ValueTuple<short, short>(place.CricketColorId, place.CricketPartsId).CalcCricketName();
		}
		else
		{
			CricketView cricket = this.escapeCricketView;
			RectTransform cloudHolder = this.cloudHolder;
			CanvasGroup failTips = this.failTipsBack;
			cricket.SetCricketData(place.CricketColorId, place.CricketPartsId, false, null, false);
			cricket.PlayAnimation(success ? ECricketAnim.cricket_nocatch_catch : ECricketAnim.cricket_nocatch, false, true);
			AudioManager.Instance.PlaySound(success ? "ui_cacthcricket_get" : "ui_cacthcricket_run", false, false);
			failTips.alpha = 0f;
			bool flag7 = success;
			if (flag7)
			{
				this._supriseShowed = true;
				for (int j = 0; j < cloudHolder.childCount; j++)
				{
					RectTransform cloud = cloudHolder.GetChild(j).GetComponent<RectTransform>();
					Vector2 endPos = cloud.anchoredPosition;
					Vector3 scale2 = cloud.localScale;
					cloud.anchoredPosition = new Vector2(endPos.x + 1100f * ((cloud.pivot.x == 0f) ? (-scale2.x) : scale2.x), endPos.y + 1100f * scale2.y);
				}
				DG.Tweening.Sequence seq = DOTween.Sequence();
				seq.AppendInterval(4.5f);
				seq.AppendCallback(delegate
				{
					this.surpriseLight.gameObject.SetActive(true);
					this.surpriseLight.Play();
				});
				seq.Append(this.whiteLight.DOFade(1f, 0f).SetEase(Ease.InQuad));
				seq.AppendCallback(new TweenCallback(this.ShowCatchResult));
				seq.AppendInterval(0.3f);
				seq.Append(this.whiteLight.DOFade(0f, 0f).SetEase(Ease.Linear));
				seq.PlayForward();
			}
			else
			{
				for (int k = 0; k < cloudHolder.childCount; k++)
				{
					RectTransform cloud2 = cloudHolder.GetChild(k).GetComponent<RectTransform>();
					Vector2 endPos2 = cloud2.anchoredPosition;
					Vector3 scale3 = cloud2.localScale;
					cloud2.anchoredPosition = new Vector2(endPos2.x + 1100f * ((cloud2.pivot.x == 0f) ? (-scale3.x) : scale3.x), endPos2.y + 1100f * scale3.y);
					cloud2.DOAnchorPos(endPos2, 1f, false).SetDelay(2.83f);
				}
				failTips.DOFade(1f, 0.5f).SetDelay(3.83f).OnStart(delegate
				{
					AudioManager.Instance.PlaySound("ui_cacthcricket_word", false, false);
				});
			}
		}
		this.finalCatchResult = success;
		this._finalSceneShow = true;
	}

	// Token: 0x06001392 RID: 5010 RVA: 0x00079968 File Offset: 0x00077B68
	private void UpdateCricketInJar(CricketView cricket, float posX, ItemDisplayData itemData)
	{
		short colorId = itemData.CricketColorId;
		short partId = itemData.CricketPartId;
		RectTransform cricketTransform = cricket.GetComponent<RectTransform>();
		cricket.SetCricketData(colorId, partId, true, itemData, false);
		cricket.PlayAnimation(ECricketAnim.Idle, true, false);
		SingletonObject.getInstance<YieldHelper>().DelaySecondsDo((float)Random.Range(1, cricket.Level + 1), delegate
		{
			cricket.Sing(true, true, true, 1f, null, 0f);
		});
		cricketTransform.anchoredPosition = new Vector2(posX, 0f);
		cricketTransform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
	}

	// Token: 0x06001393 RID: 5011 RVA: 0x00079A20 File Offset: 0x00077C20
	private string[] SplitPoem(string originPoem)
	{
		char[] spliters = new char[]
		{
			LocalStringManager.Get(LanguageKey.LK_Comma_Symbol)[0],
			LocalStringManager.Get(LanguageKey.LK_period_Symbol)[0],
			'\n'
		};
		return originPoem.Split(spliters, StringSplitOptions.RemoveEmptyEntries);
	}

	// Token: 0x06001394 RID: 5012 RVA: 0x00079A6C File Offset: 0x00077C6C
	private void OnTimeoutConfirmClick()
	{
		this.timeoutResultPage.gameObject.SetActive(false);
		this.catchResult.SetActive(false);
		UIManager.Instance.HideUI(this.Element);
	}

	// Token: 0x06001395 RID: 5013 RVA: 0x00079AA0 File Offset: 0x00077CA0
	private void OnConfirmQuitGameState(ArgumentBox argBox)
	{
		bool show;
		argBox.Get("ShowState", out show);
		Time.timeScale = (float)(show ? 0 : 1);
		bool flag = !show;
		if (flag)
		{
			AudioManager.Instance.Resume();
		}
		else
		{
			AudioManager.Instance.Pause();
		}
		bool catching = this._catching;
		if (catching)
		{
			foreach (ViewCatchCricket.CricketPlaceInfo place in this._catchPlaceList)
			{
				AudioSource placeAudio = place.PlaceView.PlaceAudioSource;
				bool flag2 = show;
				if (flag2)
				{
					placeAudio.Pause();
				}
				else
				{
					placeAudio.Play();
				}
			}
			bool flag3 = !show;
			if (flag3)
			{
				bool key = Input.GetKey(KeyCode.Mouse0);
				if (key)
				{
					ConchShipCursor.Instance.SetCursorImage(ViewCatchCricket.CatchCursorDown, 0.5f, 0f);
				}
				else
				{
					ConchShipCursor.Instance.SetCursorImage(ViewCatchCricket.CatchCursor, 0.1f, 0.1f);
				}
			}
			else
			{
				ConchShipCursor.Instance.SetDefaultCursor();
			}
		}
		else
		{
			bool activeSelf = this.catchSuccess.activeSelf;
			if (activeSelf)
			{
				RectTransform cricketJar = this.jar;
				for (int i = 0; i < 2; i++)
				{
					CricketView cricket = cricketJar.GetChild(i).GetComponent<CricketView>();
					bool activeSelf2 = cricket.gameObject.activeSelf;
					if (activeSelf2)
					{
						bool flag4 = show;
						if (flag4)
						{
							cricket.PauseSing();
						}
						else
						{
							cricket.ResumeSing();
						}
					}
				}
			}
		}
	}

	// Token: 0x06001397 RID: 5015 RVA: 0x00079C54 File Offset: 0x00077E54
	// Note: this type is marked as 'beforefieldinit'.
	static ViewCatchCricket()
	{
		Dictionary<ECricketPartsType, short> dictionary = new Dictionary<ECricketPartsType, short>();
		dictionary[ECricketPartsType.Cyan] = 22;
		dictionary[ECricketPartsType.Yellow] = 23;
		dictionary[ECricketPartsType.Purple] = 24;
		dictionary[ECricketPartsType.Red] = 25;
		dictionary[ECricketPartsType.Black] = 26;
		dictionary[ECricketPartsType.White] = 27;
		ViewCatchCricket.Color2RealColor = dictionary;
		ViewCatchCricket.PoemBackSize2 = new Vector2(183f, 469f);
		ViewCatchCricket.PoemBackSize4 = new Vector2(363f, 470f);
	}

	// Token: 0x0600139B RID: 5019 RVA: 0x00079D48 File Offset: 0x00077F48
	[CompilerGenerated]
	private void <UpdateCountdown>g__SetDigits|96_0(Transform layout, ref ViewCatchCricket.<>c__DisplayClass96_0 A_2)
	{
		layout.GetChild(0).gameObject.SetActive(A_2.doubleDigits > 0);
		CImage doubleDigitsImage = layout.GetChild(0).GetComponent<CImage>();
		CImage singeDigitsImage = layout.GetChild(1).GetComponent<CImage>();
		string imageSet = A_2.isEnd ? "1_" : "0_";
		doubleDigitsImage.SetSprite(string.Format("{0}{1}{2}", "ui9_back_catchcricket_number_", imageSet, A_2.doubleDigits), false, null);
		singeDigitsImage.SetSprite(string.Format("{0}{1}{2}", "ui9_back_catchcricket_number_", imageSet, A_2.singeDigits), false, null);
		this.countdownBackImage.sprite = (A_2.isEnd ? this.countdownUrgentSprite : this.countdownNormalSprite);
	}

	// Token: 0x0400104C RID: 4172
	private const float FirstCountdownAniTime = 20f;

	// Token: 0x0400104D RID: 4173
	private bool finalCatchResult;

	// Token: 0x0400104E RID: 4174
	private const float LightSpreadTime = 0f;

	// Token: 0x0400104F RID: 4175
	private const float LightStayTime = 0.3f;

	// Token: 0x04001050 RID: 4176
	private const float LightHideTime = 0f;

	// Token: 0x04001051 RID: 4177
	private static readonly Dictionary<ECricketPartsType, short> Color2RealColor;

	// Token: 0x04001052 RID: 4178
	private const float PosInJarSingle = 20f;

	// Token: 0x04001053 RID: 4179
	private const float PosInJarDouble0 = -140f;

	// Token: 0x04001054 RID: 4180
	private const float PosInJarDouble1 = 180f;

	// Token: 0x04001055 RID: 4181
	private readonly Vector2[] _catchPlacePosRandomPool = new Vector2[36];

	// Token: 0x04001056 RID: 4182
	private bool _finalSceneShow;

	// Token: 0x04001057 RID: 4183
	private float _timeToActivate;

	// Token: 0x04001058 RID: 4184
	private int _luckPoint;

	// Token: 0x04001059 RID: 4185
	private readonly ViewCatchCricket.CricketPlaceInfo[] _catchPlaceList = new ViewCatchCricket.CricketPlaceInfo[21];

	// Token: 0x0400105A RID: 4186
	private bool _catching;

	// Token: 0x0400105B RID: 4187
	private float _timer;

	// Token: 0x0400105C RID: 4188
	private int _countdown;

	// Token: 0x0400105D RID: 4189
	private int _clickedPlaceIndex;

	// Token: 0x0400105E RID: 4190
	private List<ItemDisplayData> _catchResult = new List<ItemDisplayData>();

	// Token: 0x0400105F RID: 4191
	private bool _supriseShowed;

	// Token: 0x04001060 RID: 4192
	private short _wishingCricketId = -1;

	// Token: 0x04001061 RID: 4193
	[SerializeField]
	private Canvas back;

	// Token: 0x04001062 RID: 4194
	[SerializeField]
	private GameObject clickMask;

	// Token: 0x04001063 RID: 4195
	[SerializeField]
	private GameObject catchPanel;

	// Token: 0x04001064 RID: 4196
	[SerializeField]
	private GameObject catchResult;

	// Token: 0x04001065 RID: 4197
	[SerializeField]
	private ParticleSystem catchJarLight;

	// Token: 0x04001066 RID: 4198
	[SerializeField]
	private Canvas findCricketTips;

	// Token: 0x04001067 RID: 4199
	[SerializeField]
	private RectTransform effHolder;

	// Token: 0x04001068 RID: 4200
	[SerializeField]
	private CImage blackMask;

	// Token: 0x04001069 RID: 4201
	[SerializeField]
	private RectTransform catchPlaceRoot;

	// Token: 0x0400106A RID: 4202
	[SerializeField]
	private GameObject catchPlacePrefab;

	// Token: 0x0400106B RID: 4203
	[SerializeField]
	private CRawImage interfaceImg;

	// Token: 0x0400106C RID: 4204
	[SerializeField]
	private RectTransform wuqiHolder;

	// Token: 0x0400106D RID: 4205
	[SerializeField]
	private RectTransform jar;

	// Token: 0x0400106E RID: 4206
	[SerializeField]
	private RectTransform countdownLayout;

	// Token: 0x0400106F RID: 4207
	[SerializeField]
	private RectTransform countdownEffectLayout;

	// Token: 0x04001070 RID: 4208
	[SerializeField]
	private CImage countdownBackImage;

	// Token: 0x04001071 RID: 4209
	[SerializeField]
	private Sprite countdownNormalSprite;

	// Token: 0x04001072 RID: 4210
	[SerializeField]
	private Sprite countdownUrgentSprite;

	// Token: 0x04001073 RID: 4211
	[SerializeField]
	private SkeletonGraphic catchAniGraphic;

	// Token: 0x04001074 RID: 4212
	[SerializeField]
	private SkeletonAnimation catchAniForBoneFollower;

	// Token: 0x04001075 RID: 4213
	[SerializeField]
	private Refers catchAniRefers;

	// Token: 0x04001076 RID: 4214
	[SerializeField]
	private CRawImage catchPoetryImage;

	// Token: 0x04001077 RID: 4215
	[SerializeField]
	private TextMeshProUGUI catchPoemEnText;

	// Token: 0x04001078 RID: 4216
	[SerializeField]
	private CRawImage catchBack;

	// Token: 0x04001079 RID: 4217
	[SerializeField]
	private CImage catchJar;

	// Token: 0x0400107A RID: 4218
	[SerializeField]
	private ParticleSystem catchJarEffectEarth;

	// Token: 0x0400107B RID: 4219
	[SerializeField]
	private ParticleSystem catchJarEffectWater;

	// Token: 0x0400107C RID: 4220
	[SerializeField]
	private ParticleSystem catchJarLightSpread;

	// Token: 0x0400107D RID: 4221
	[SerializeField]
	private CImage whiteLight;

	// Token: 0x0400107E RID: 4222
	[SerializeField]
	private GameObject catchSuccess;

	// Token: 0x0400107F RID: 4223
	[SerializeField]
	private GameObject catchFail;

	// Token: 0x04001080 RID: 4224
	[SerializeField]
	private RectTransform effectHolder;

	// Token: 0x04001081 RID: 4225
	[SerializeField]
	private CRawImage cricketPoemImage;

	// Token: 0x04001082 RID: 4226
	[SerializeField]
	private CRawImage cricketPoemBackImage;

	// Token: 0x04001083 RID: 4227
	[SerializeField]
	private TextMeshProUGUI cricketPoemEnText;

	// Token: 0x04001084 RID: 4228
	[SerializeField]
	private TextMeshProUGUI cricketNameText;

	// Token: 0x04001085 RID: 4229
	[SerializeField]
	private CRawImage cricketNameImage;

	// Token: 0x04001086 RID: 4230
	[SerializeField]
	private Texture2D cricketPoemBack2;

	// Token: 0x04001087 RID: 4231
	[SerializeField]
	private Texture2D cricketPoemBack4;

	// Token: 0x04001088 RID: 4232
	private static readonly Vector2 PoemBackSize2;

	// Token: 0x04001089 RID: 4233
	private static readonly Vector2 PoemBackSize4;

	// Token: 0x0400108A RID: 4234
	[SerializeField]
	private CricketView escapeCricketView;

	// Token: 0x0400108B RID: 4235
	[SerializeField]
	private RectTransform cloudHolder;

	// Token: 0x0400108C RID: 4236
	[SerializeField]
	private CanvasGroup failTipsBack;

	// Token: 0x0400108D RID: 4237
	[SerializeField]
	private ParticleSystem surpriseLight;

	// Token: 0x0400108E RID: 4238
	[SerializeField]
	private RectTransform timeoutResultPage;

	// Token: 0x0400108F RID: 4239
	[SerializeField]
	private CButton timeoutConfirmBtn;

	// Token: 0x04001090 RID: 4240
	[SerializeField]
	private RectTransform encyclopediaAreaRect;

	// Token: 0x04001091 RID: 4241
	[SerializeField]
	private GameObject hotkeyDisplay;

	// Token: 0x04001092 RID: 4242
	private float _cellHeight;

	// Token: 0x04001093 RID: 4243
	private EMapAreaAreaDirection _areaType = EMapAreaAreaDirection.North;

	// Token: 0x04001094 RID: 4244
	private bool _inEncyclopediaArea;

	// Token: 0x0200123F RID: 4671
	public class CricketPlaceInfo
	{
		// Token: 0x040099F4 RID: 39412
		public sbyte PlaceId;

		// Token: 0x040099F5 RID: 39413
		public short CricketColorId;

		// Token: 0x040099F6 RID: 39414
		public short CricketPartsId;

		// Token: 0x040099F7 RID: 39415
		public sbyte CricketLevel;

		// Token: 0x040099F8 RID: 39416
		public float SingStartTime;

		// Token: 0x040099F9 RID: 39417
		public short SingCount;

		// Token: 0x040099FA RID: 39418
		public sbyte SingPitch;

		// Token: 0x040099FB RID: 39419
		public short SingSize;

		// Token: 0x040099FC RID: 39420
		public short SingLevel;

		// Token: 0x040099FD RID: 39421
		public float SingTotalTime;

		// Token: 0x040099FE RID: 39422
		public float SingTimer;

		// Token: 0x040099FF RID: 39423
		public float DelayTime;

		// Token: 0x04009A00 RID: 39424
		public float LoudSingTimer;

		// Token: 0x04009A01 RID: 39425
		public CatchCricketPlace PlaceView;
	}
}
