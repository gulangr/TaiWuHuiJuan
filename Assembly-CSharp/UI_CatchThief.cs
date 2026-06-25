using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Story;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

// Token: 0x020001C3 RID: 451
public class UI_CatchThief : UIBase
{
	// Token: 0x06001BF1 RID: 7153 RVA: 0x000C12E0 File Offset: 0x000BF4E0
	public override void OnInit(ArgumentBox argBox)
	{
		this.Awake();
		this._catching = false;
		this._timer = 0f;
		this._clickedPlaceIndex = -1;
		RectTransform fireHolder = base.CGet<RectTransform>("FireHolder");
		base.GetComponent<CanvasGroup>().alpha = 1f;
		base.CGet<GameObject>("ClickMask").SetActive(true);
		base.CGet<GameObject>("CatchPanel").SetActive(false);
		for (int i = 0; i < fireHolder.childCount; i++)
		{
			fireHolder.GetChild(i).gameObject.SetActive(false);
		}
		fireHolder.gameObject.SetActive(true);
		AudioManager.Instance.PlaySound("ui_cacthcricket_begin_whoosh", false, false);
		int catchThiefTimes;
		argBox.Get("CatchThiefTimes", out catchThiefTimes);
		this.InitCatchPlace(catchThiefTimes);
		this.UpdateCursor(true);
		UIElement element = this.Element;
		element.OnShowed = (Action)Delegate.Combine(element.OnShowed, new Action(this.OnShowed));
	}

	// Token: 0x06001BF2 RID: 7154 RVA: 0x000C13DC File Offset: 0x000BF5DC
	private void Awake()
	{
		bool awakeInvoked = this._awakeInvoked;
		if (!awakeInvoked)
		{
			this._awakeInvoked = true;
			this._bambooAndRope = base.CGet<SkeletonAnimation>("BambooAndRope");
			RectTransform catchPlaceRoot = base.CGet<RectTransform>("CatchPlaceRoot");
			GameObject catchPlacePrefab = base.CGet<GameObject>("CatchPlacePrefab");
			Vector2 placeInterval = new Vector2(14f, 7f);
			float halfSize = catchPlacePrefab.GetComponent<RectTransform>().sizeDelta.x / 2f;
			for (int row = 0; row < 3; row++)
			{
				for (int col = 0; col < 12; col++)
				{
					this._catchPlacePosRandomPool[row * 12 + col] = new Vector2((halfSize + placeInterval.x) * (float)col, -(halfSize + placeInterval.y) * 2f * (float)row - ((col % 2 == 0) ? 0f : (halfSize + placeInterval.y)));
				}
			}
			while (catchPlaceRoot.childCount < 21)
			{
				Object.Instantiate<GameObject>(catchPlacePrefab, catchPlaceRoot);
			}
			for (int i = 0; i < 21; i++)
			{
				this._catchPlaceList[i] = catchPlaceRoot.GetChild(i).GetComponent<global::CatchThiefPlace>();
				this._catchPlaceList[i].Init(i, new Action<int>(this.OnClickCatchPlace));
			}
		}
	}

	// Token: 0x06001BF3 RID: 7155 RVA: 0x000C1534 File Offset: 0x000BF734
	private void OnShowed()
	{
		this._catching = true;
		base.CGet<GameObject>("ClickMask").SetActive(false);
		AudioManager.Instance.PlayMusic("catch_cricket", 1f, 100, null);
		AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 1f, 100);
		AudioManager.Instance.PlaySound("ui_cacthcricket_bg", false, false);
	}

	// Token: 0x06001BF4 RID: 7156 RVA: 0x000C159C File Offset: 0x000BF79C
	private void OnEnable()
	{
		GEvent.Add(EEvents.OnConfirmQuitGameState, new GEvent.Callback(this.OnConfirmQuitGameState));
	}

	// Token: 0x06001BF5 RID: 7157 RVA: 0x000C15B8 File Offset: 0x000BF7B8
	private void OnDisable()
	{
		GEvent.Remove(EEvents.OnConfirmQuitGameState, new GEvent.Callback(this.OnConfirmQuitGameState));
		SingletonObject.getInstance<WorldMapModel>().UpdateBgm();
	}

	// Token: 0x06001BF6 RID: 7158 RVA: 0x000C15E0 File Offset: 0x000BF7E0
	private void Update()
	{
		bool catching = this._catching;
		if (catching)
		{
			foreach (global::CatchThiefPlace place in this._catchPlaceList)
			{
				place.UpdateSing(this._timer);
			}
			this.UpdateTime();
		}
	}

	// Token: 0x06001BF7 RID: 7159 RVA: 0x000C1628 File Offset: 0x000BF828
	private void InitCatchPlace(int catchThiefTimes)
	{
		List<Vector2> posPool = new List<Vector2>(this._catchPlacePosRandomPool);
		List<int> placePool = new List<int>();
		foreach (CatchThiefPlaceItem placeConfig in ((IEnumerable<CatchThiefPlaceItem>)Config.CatchThiefPlace.Instance))
		{
			placePool.Add((int)placeConfig.Rate);
		}
		for (int i = 0; i < 21; i++)
		{
			global::CatchThiefPlace place = this._catchPlaceList[i];
			int posIndex = Random.Range(0, posPool.Count);
			place.RectTransform.anchoredPosition = posPool[posIndex];
			posPool.RemoveAt(posIndex);
			place.Set((sbyte)placePool.GetRandomIndex(), catchThiefTimes);
		}
		foreach (ValueTuple<int, float, short> valueTuple in CatchUtils.RandomGroups(-1))
		{
			int index = valueTuple.Item1;
			float singTime = valueTuple.Item2;
			short singCount = valueTuple.Item3;
			bool flag = index < 0;
			if (!flag)
			{
				this._catchPlaceList[index].SetSing(singTime, (int)singCount);
			}
		}
		this._bambooAndRope.AnimationState.SetAnimation(0, "idle", true);
		this.UpdateTime();
	}

	// Token: 0x06001BF8 RID: 7160 RVA: 0x000C1784 File Offset: 0x000BF984
	private void UpdateTime()
	{
		PathConstraint[] pathList = this._bambooAndRope.Skeleton.PathConstraints.Items;
		RectTransform fireHolder = base.CGet<RectTransform>("FireHolder");
		bool firstCountdown = this._timer < 20f;
		this._timer += Time.deltaTime;
		int countdown = Mathf.Max(0, 30 - (int)this._timer);
		base.CGet<CatchCountdown>("Countdown").Set(countdown);
		fireHolder.GetChild(0).gameObject.SetActive(firstCountdown);
		fireHolder.GetChild(1).gameObject.SetActive(!firstCountdown);
		fireHolder.GetChild(2).gameObject.SetActive(!firstCountdown);
		bool flag = firstCountdown;
		if (flag)
		{
			pathList[0].Position = this._timer / 20f;
			pathList[1].Position = 0f;
			pathList[2].Position = 0f;
		}
		else
		{
			bool flag2 = this._timer < 30f;
			if (flag2)
			{
				float timePercent = Mathf.Min((this._timer - 20f) / 9f, 1f);
				pathList[0].Position = 1f;
				pathList[1].Position = timePercent;
				pathList[2].Position = timePercent;
			}
			else
			{
				this.FinishCatch();
				this.DoRequestCatchAndHide(true);
			}
		}
	}

	// Token: 0x06001BF9 RID: 7161 RVA: 0x000C18DC File Offset: 0x000BFADC
	private void UpdateCursor(bool catching)
	{
		if (catching)
		{
			ConchShipCursor.Instance.SetCursorImageWithKey("catch_thief", "catchcricket_01_gn_guangbiaosheng", 0f, 1f);
		}
		else
		{
			ConchShipCursor.Instance.SetDefaultCursorAndReleaseKey("catch_thief");
		}
	}

	// Token: 0x06001BFA RID: 7162 RVA: 0x000C1920 File Offset: 0x000BFB20
	private void OnClickCatchPlace(int index)
	{
		this._clickedPlaceIndex = index;
		this.FinishCatch();
		int poetryIndex = Random.Range(0, 3);
		this.cnPoemImage.SetTexture(string.Format("{0}{1}", "ui9_tex_catchthief_catch_poetry_", poetryIndex));
		this.enPoemLabel.text = LocalStringManager.Get(string.Format("LK_Catch_Thief_Poem_{0}", poetryIndex));
		base.CGet<CImage>("CatchBack").SetSprite(this._catchPlaceList[this._clickedPlaceIndex].PlaceConfig.CatchAniBack, false, null);
		SkeletonGraphic catchAniGraphic = base.CGet<SkeletonGraphic>("CatchAni");
		catchAniGraphic.Initialize(true);
		catchAniGraphic.AnimationState.SetAnimation(0, "idle", true);
		catchAniGraphic.AnimationState.SetAnimation(1, "catch", false);
		catchAniGraphic.AnimationState.Complete += this.AnimationStateOnComplete;
		AudioManager.Instance.PlaySound("ui_cacthcricket_pre", false, false);
		base.CGet<GameObject>("CatchPanel").SetActive(true);
	}

	// Token: 0x06001BFB RID: 7163 RVA: 0x000C1A28 File Offset: 0x000BFC28
	private void AnimationStateOnComplete(TrackEntry track)
	{
		bool flag = track.TrackIndex != 1;
		if (!flag)
		{
			this.DoRequestCatchAndHide(false);
		}
	}

	// Token: 0x06001BFC RID: 7164 RVA: 0x000C1A50 File Offset: 0x000BFC50
	private void FinishCatch()
	{
		this._catching = false;
		base.CGet<GameObject>("ClickMask").SetActive(true);
		AudioManager.Instance.StopSound("ui_cacthcricket_bg");
		foreach (global::CatchThiefPlace catchPlace in this._catchPlaceList)
		{
			catchPlace.StopPlaying();
		}
		this.UpdateCursor(this._catching);
	}

	// Token: 0x06001BFD RID: 7165 RVA: 0x000C1AB8 File Offset: 0x000BFCB8
	private void DoRequestCatchAndHide(bool timeOut)
	{
		UIManager.Instance.HideUI(this.Element);
		sbyte thiefLevel = (timeOut || this._clickedPlaceIndex < 0) ? -1 : this._catchPlaceList[this._clickedPlaceIndex].ThiefLevel;
		StoryDomainMethod.Call.CatchThief(thiefLevel, timeOut);
	}

	// Token: 0x06001BFE RID: 7166 RVA: 0x000C1B04 File Offset: 0x000BFD04
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
			this.UpdateCursor(!show);
		}
	}

	// Token: 0x040015C8 RID: 5576
	private const float FirstCountdownAniTime = 20f;

	// Token: 0x040015C9 RID: 5577
	private const string CatchCursor = "catchcricket_01_gn_guangbiaosheng";

	// Token: 0x040015CA RID: 5578
	private const string CatchCursorKey = "catch_thief";

	// Token: 0x040015CB RID: 5579
	private readonly Vector2[] _catchPlacePosRandomPool = new Vector2[36];

	// Token: 0x040015CC RID: 5580
	private readonly global::CatchThiefPlace[] _catchPlaceList = new global::CatchThiefPlace[21];

	// Token: 0x040015CD RID: 5581
	private bool _awakeInvoked;

	// Token: 0x040015CE RID: 5582
	private float _timeToActivate;

	// Token: 0x040015CF RID: 5583
	private bool _catching;

	// Token: 0x040015D0 RID: 5584
	private float _timer;

	// Token: 0x040015D1 RID: 5585
	private int _clickedPlaceIndex;

	// Token: 0x040015D2 RID: 5586
	private SkeletonAnimation _bambooAndRope;

	// Token: 0x040015D3 RID: 5587
	[SerializeField]
	private TextMeshProUGUI enPoemLabel;

	// Token: 0x040015D4 RID: 5588
	[SerializeField]
	private CRawImage cnPoemImage;
}
