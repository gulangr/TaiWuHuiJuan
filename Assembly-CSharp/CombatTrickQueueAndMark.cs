using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Combat;
using Game.Views.Combat.Migrate;
using Game.Views.MouseTips;
using UnityEngine;

// Token: 0x02000152 RID: 338
public class CombatTrickQueueAndMark : MonoBehaviour, ICombatComponent
{
	// Token: 0x060012A3 RID: 4771 RVA: 0x000716B8 File Offset: 0x0006F8B8
	public void Setup()
	{
		CombatModel model = this.Model;
		model.OnTricksChanged = (OnDataChangedEvent)Delegate.Combine(model.OnTricksChanged, new OnDataChangedEvent(this.RefreshTrickCollection));
		CombatModel model2 = this.Model;
		model2.OnMaxTrickCountChanged = (OnDataChangedEvent)Delegate.Combine(model2.OnMaxTrickCountChanged, new OnDataChangedEvent(this.UpdateMaxTrickCount));
		this.Model.AddEvent(ECombatEvents.OnDataReady, new OnCombatEvent(this.OnDataReady));
		this.Model.AddEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
		this.Model.AddEvent(ECombatEvents.UpdateSkillCostPreview, new OnCombatEvent(this.UpdateSkillCostTrickPreview));
		this.Model.AddEvent(ECombatEvents.OnGetCanCostTrickDuringPreparingSkill, new OnCombatEvent(this.OnGetCanCostTrickDuringPreparingSkill));
	}

	// Token: 0x060012A4 RID: 4772 RVA: 0x0007177C File Offset: 0x0006F97C
	public void Close()
	{
		CombatModel model = this.Model;
		model.OnTricksChanged = (OnDataChangedEvent)Delegate.Remove(model.OnTricksChanged, new OnDataChangedEvent(this.RefreshTrickCollection));
		CombatModel model2 = this.Model;
		model2.OnMaxTrickCountChanged = (OnDataChangedEvent)Delegate.Remove(model2.OnMaxTrickCountChanged, new OnDataChangedEvent(this.UpdateMaxTrickCount));
		this.Model.RemoveEvent(ECombatEvents.OnDataReady, new OnCombatEvent(this.OnDataReady));
		this.Model.RemoveEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
		this.Model.RemoveEvent(ECombatEvents.UpdateSkillCostPreview, new OnCombatEvent(this.UpdateSkillCostTrickPreview));
		this.Model.RemoveEvent(ECombatEvents.OnGetCanCostTrickDuringPreparingSkill, new OnCombatEvent(this.OnGetCanCostTrickDuringPreparingSkill));
	}

	// Token: 0x17000216 RID: 534
	// (get) Token: 0x060012A5 RID: 4773 RVA: 0x0007183E File Offset: 0x0006FA3E
	private bool Flip
	{
		get
		{
			return !this.ally;
		}
	}

	// Token: 0x17000217 RID: 535
	// (get) Token: 0x060012A6 RID: 4774 RVA: 0x00071849 File Offset: 0x0006FA49
	private CombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<CombatModel>();
		}
	}

	// Token: 0x060012A7 RID: 4775 RVA: 0x00071850 File Offset: 0x0006FA50
	private void Awake()
	{
		this._trickMaskInitPos = this.trickMask.anchoredPosition;
		this._trickPointList.Clear();
		for (int i = 0; i < this.trickPointParent.childCount; i++)
		{
			this._trickPointList.Add(this.trickPointParent.GetChild(i).GetComponent<RectTransform>().anchoredPosition);
		}
	}

	// Token: 0x060012A8 RID: 4776 RVA: 0x000718B9 File Offset: 0x0006FAB9
	private void OnDataReady()
	{
		this.Init();
	}

	// Token: 0x060012A9 RID: 4777 RVA: 0x000718C4 File Offset: 0x0006FAC4
	private void Init()
	{
		foreach (KeyValuePair<int, RectTransform> trickObj in this._trickObjs)
		{
			CombatPoolAdaptor.Destroy("ui_Combat_TrickPrefab", trickObj.Value.gameObject, true);
		}
		this._trickObjs.Clear();
		this._needRemoveTricks.Clear();
		this._lastTricks.Clear();
		this._lastMaxTrick = 9;
		this.trickMask.anchoredPosition = this._trickMaskInitPos;
	}

	// Token: 0x060012AA RID: 4778 RVA: 0x0007196C File Offset: 0x0006FB6C
	public void SetSpawnPointFollowTarget(Transform target)
	{
		this.spawnPoint.GetComponent<PositionFollower>().Target = target;
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x00071980 File Offset: 0x0006FB80
	private void OnChangeChar()
	{
		int charId = this.Model.ChangingToCharId;
		bool flag = this.ally != this.Model.CharIsAlly(charId);
		if (!flag)
		{
			this.RefreshTrickCollectionByCharacter(charId);
			this.UpdateMaxTrickCountByCharacter(charId);
		}
	}

	// Token: 0x060012AC RID: 4780 RVA: 0x000719C8 File Offset: 0x0006FBC8
	private void RefreshTrickCollection(bool isAlly)
	{
		bool flag = isAlly != this.ally;
		if (!flag)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			this.RefreshTrickCollectionByCharacter(charId);
		}
	}

	// Token: 0x060012AD RID: 4781 RVA: 0x00071A0C File Offset: 0x0006FC0C
	private void RefreshTrickCollectionByCharacter(int charId)
	{
		CombatSubProcessorCharacter processor;
		bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
		if (!flag)
		{
			IReadOnlyDictionary<int, sbyte> tricks = processor.Tricks.Tricks;
			CombatSubProcessorCharacter enemyProcessor = this.ally ? this.Model.ProcessorCharacters.GetValueOrDefault(this.Model.EnemyCharId) : this.Model.ProcessorCharacters.GetValueOrDefault(this.Model.SelfCharId);
			bool flag2 = enemyProcessor == null;
			if (!flag2)
			{
				MouseTipTrickType.AvoidTrickTipHitOddsInfo avoidTrickTipHitOddsInfo = new MouseTipTrickType.AvoidTrickTipHitOddsInfo
				{
					SelfHit = processor.HitValues,
					EnemyAvoid = enemyProcessor.AvoidValues
				};
				bool isAddNewTrick = false;
				foreach (KeyValuePair<int, sbyte> trickInfo in tricks)
				{
					bool isAvoidTrick = processor.Tricks.IsAvoidTrick(trickInfo.Key);
					bool flag3 = !this._lastTricks.ContainsKey(trickInfo.Key);
					RectTransform trickTransform;
					if (flag3)
					{
						trickTransform = CombatPoolAdaptor.GetObject<RectTransform>("ui_Combat_TrickPrefab", true);
						trickTransform.position = this.spawnPoint.position;
						trickTransform.SetParent(this.trickQueue, true);
						trickTransform.localScale = Vector3.one;
						this._trickObjs.Add(trickInfo.Key, trickTransform);
						isAddNewTrick = true;
					}
					else
					{
						trickTransform = this._trickObjs[trickInfo.Key];
					}
					CombatWeaponTrick.UpdateTrickIcon(trickTransform.GetComponent<CombatTrickPrefab>(), trickInfo.Value, true, false, isAvoidTrick, isAvoidTrick ? new MouseTipTrickType.AvoidTrickTipHitOddsInfo?(avoidTrickTipHitOddsInfo) : null);
					this.UpdateQueuedTrickHover(trickTransform.GetComponent<CombatTrickPrefab>(), this.Model.CanCostTrickDuringPreparingSkill, trickInfo.Key);
				}
				bool flag4 = isAddNewTrick;
				if (flag4)
				{
					this.effTrickSpawnHint.Play(true);
				}
				bool flag5 = !this._lastCanCostTrickDuringPreparingSkill && this.Model.CanCostTrickDuringPreparingSkill;
				if (flag5)
				{
					ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("soundName", "se_effect_40209_select");
					GEvent.OnEvent(UiEvents.PlayCombatSoundOnce, argumentBox);
				}
				this._lastCanCostTrickDuringPreparingSkill = this.Model.CanCostTrickDuringPreparingSkill;
				foreach (int trickKey in this._lastTricks.Keys)
				{
					bool flag6 = !tricks.ContainsKey(trickKey);
					if (flag6)
					{
						this._needRemoveTricks.Add(trickKey);
					}
				}
				this._lastTricks.Clear();
				foreach (KeyValuePair<int, sbyte> trick in tricks)
				{
					this._lastTricks.Add(trick.Key, trick.Value);
				}
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
				{
					bool doingChangeMaxTrickAni = this._doingChangeMaxTrickAni;
					if (!doingChangeMaxTrickAni)
					{
						bool flag7 = this._needRemoveTricks.Count > 0;
						if (flag7)
						{
							this.RemoveTrickObjs();
						}
						bool flag8 = this.ally && this.Model.ShowSkillCostPreview;
						if (flag8)
						{
							this.UpdateSkillCostTrickPreview();
						}
						this.UpdateTrickPos(true);
					}
				});
			}
		}
	}

	// Token: 0x060012AE RID: 4782 RVA: 0x00071D60 File Offset: 0x0006FF60
	private void UpdateQueuedTrickHover(CombatTrickPrefab trickRefers, bool canCostTrickDuringPreparingSkill, int trickIndexInCollection)
	{
		PointerTrigger pointerTrigger = trickRefers.GetComponent<PointerTrigger>();
		CButton button = trickRefers.GetComponent<CButton>();
		UIParticle clickableParticle = trickRefers.clickableParticle;
		List<GameObject> clickableHoverList = trickRefers.clickableHoverList;
		bool flag = !this.ally;
		if (flag)
		{
			this.ClearTrickObjInteractAndEffect(pointerTrigger, button, clickableParticle, clickableHoverList);
		}
		else
		{
			bool interactable = canCostTrickDuringPreparingSkill && trickIndexInCollection >= 0;
			bool flag2 = pointerTrigger;
			if (flag2)
			{
				pointerTrigger.enabled = interactable;
				pointerTrigger.EnterEvent.RemoveAllListeners();
				pointerTrigger.ExitEvent.RemoveAllListeners();
			}
			bool flag3 = button;
			if (flag3)
			{
				button.interactable = interactable;
			}
			bool flag4 = clickableParticle;
			if (flag4)
			{
				clickableParticle.gameObject.SetActive(interactable);
			}
			button.EnableClickAudio = !interactable;
			bool flag5 = clickableHoverList == null;
			if (!flag5)
			{
				if (canCostTrickDuringPreparingSkill)
				{
					pointerTrigger.EnterEvent.AddListener(delegate()
					{
						foreach (GameObject hover2 in clickableHoverList)
						{
							bool flag7 = hover2;
							if (flag7)
							{
								hover2.SetActive(true);
							}
						}
						trickRefers.transform.SetAsLastSibling();
					});
					pointerTrigger.ExitEvent.AddListener(delegate()
					{
						foreach (GameObject hover2 in clickableHoverList)
						{
							bool flag7 = hover2;
							if (flag7)
							{
								hover2.SetActive(false);
							}
						}
					});
					button.ClearAndAddListener(delegate
					{
						SingletonObject.getInstance<CombatModel>().DoRequestCostTrickDuringPreparingSkill(trickIndexInCollection);
						ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("soundName", "se_effect_40209_click");
						GEvent.OnEvent(UiEvents.PlayCombatSoundOnce, argumentBox);
						ArgumentBox argumentBox2 = EasyPool.Get<ArgumentBox>().Set("soundName", "se_effect_40209_buff");
						GEvent.OnEvent(UiEvents.PlayCombatSoundOnce, argumentBox2);
						GEvent.OnEvent(UiEvents.PlaySkeletonParticle, new ArgumentBox().Set("IsAlly", this.ally).Set("VfxName", "Particle_Effect_CostTrick"));
					});
				}
				else
				{
					foreach (GameObject hover in clickableHoverList)
					{
						bool flag6 = hover;
						if (flag6)
						{
							hover.SetActive(false);
						}
					}
				}
			}
		}
	}

	// Token: 0x060012AF RID: 4783 RVA: 0x00071F20 File Offset: 0x00070120
	private void UpdateMaxTrickCount(bool isAlly)
	{
		bool flag = this.ally != isAlly;
		if (!flag)
		{
			int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
			this.UpdateMaxTrickCountByCharacter(charId);
		}
	}

	// Token: 0x060012B0 RID: 4784 RVA: 0x00071F64 File Offset: 0x00070164
	private void UpdateMaxTrickCountByCharacter(int charId)
	{
		CombatSubProcessorCharacter processor;
		bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
		if (!flag)
		{
			int maxTrickCount = processor.MaxTrickCount;
			bool flag2 = this._lastMaxTrick == maxTrickCount;
			if (!flag2)
			{
				float targetPos = this.CalcTrickMaskPositionX(maxTrickCount);
				float aniTime = 0.1f * (float)Mathf.Abs(this._lastMaxTrick - maxTrickCount);
				bool flag3 = maxTrickCount < this._lastMaxTrick;
				if (flag3)
				{
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
					{
						this.trickMask.DOAnchorPosX(targetPos, aniTime, false).OnComplete(new TweenCallback(this.OnTrickMaskMoveComplete));
					});
				}
				else
				{
					this.trickMask.DOAnchorPosX(targetPos, aniTime, false).OnComplete(new TweenCallback(this.OnTrickMaskMoveComplete));
				}
				this._lastMaxTrick = maxTrickCount;
				this._doingChangeMaxTrickAni = true;
			}
		}
	}

	// Token: 0x060012B1 RID: 4785 RVA: 0x00072048 File Offset: 0x00070248
	private void OnTrickMaskMoveComplete()
	{
		this._doingChangeMaxTrickAni = false;
		bool flag = this._needRemoveTricks.Count > 0;
		if (flag)
		{
			this.RemoveTrickObjs();
		}
		this.UpdateTrickPos(false);
	}

	// Token: 0x060012B2 RID: 4786 RVA: 0x00072080 File Offset: 0x00070280
	private void RemoveTrickObjs()
	{
		foreach (int trickKey in this._needRemoveTricks)
		{
			this.ReturnTrickObj(trickKey);
		}
		this._needRemoveTricks.Clear();
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x000720E4 File Offset: 0x000702E4
	private void ReturnTrickObj(int trickKey)
	{
		RectTransform trickTransform = this._trickObjs[trickKey];
		this._trickObjs.Remove(trickKey);
		bool flag = this.ally;
		if (flag)
		{
			trickTransform.DOKill(false);
		}
		this._trickRect2ScaleTween.Remove(trickTransform);
		CombatTrickPrefab trickRefers;
		bool flag2 = trickTransform.TryGetComponent<CombatTrickPrefab>(out trickRefers);
		if (flag2)
		{
			this.ResetTrickObjState(trickRefers);
		}
		CombatPoolAdaptor.Destroy("ui_Combat_TrickPrefab", trickTransform.gameObject, true);
	}

	// Token: 0x060012B4 RID: 4788 RVA: 0x00072154 File Offset: 0x00070354
	private void ResetTrickObjState(CombatTrickPrefab trickRefers)
	{
		PointerTrigger pointerTrigger = trickRefers.GetComponent<PointerTrigger>();
		CButton button = trickRefers.GetComponent<CButton>();
		UIParticle clickableParticle = trickRefers.clickableParticle;
		List<GameObject> clickableHoverList = trickRefers.clickableHoverList;
		this.ClearTrickObjInteractAndEffect(pointerTrigger, button, clickableParticle, clickableHoverList);
		GameObject costPreview = trickRefers.costPreview;
		bool flag = costPreview;
		if (flag)
		{
			costPreview.SetActive(false);
		}
	}

	// Token: 0x060012B5 RID: 4789 RVA: 0x000721A8 File Offset: 0x000703A8
	private void ClearTrickObjInteractAndEffect(PointerTrigger pointerTrigger, CButton button, UIParticle clickableParticle, List<GameObject> clickableHoverList)
	{
		bool flag = pointerTrigger != null;
		if (flag)
		{
			pointerTrigger.enabled = false;
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.RemoveAllListeners();
		}
		bool flag2 = button != null;
		if (flag2)
		{
			button.interactable = false;
		}
		bool flag3 = clickableParticle != null;
		if (flag3)
		{
			clickableParticle.gameObject.SetActive(false);
		}
		bool flag4 = clickableHoverList != null;
		if (flag4)
		{
			foreach (GameObject hover in clickableHoverList)
			{
				bool flag5 = hover != null;
				if (flag5)
				{
					hover.SetActive(false);
				}
			}
		}
	}

	// Token: 0x060012B6 RID: 4790 RVA: 0x00072274 File Offset: 0x00070474
	private void UpdateSkillCostTrickPreview()
	{
		bool flag = !this.ally;
		if (!flag)
		{
			bool flag2;
			if (this.Model.ShowSkillCostPreview)
			{
				List<NeedTrick> costTricks = this.Model.PreviewCostSkillData.CostTricks;
				flag2 = (costTricks != null && costTricks.Count > 0);
			}
			else
			{
				flag2 = false;
			}
			bool flag3 = flag2;
			if (flag3)
			{
				foreach (NeedTrick needTrick in this.Model.PreviewCostSkillData.CostTricks)
				{
					int foundTrickCount = 0;
					foreach (KeyValuePair<int, sbyte> trickInfo in this._lastTricks)
					{
						bool flag4 = trickInfo.Value == needTrick.TrickType && !this._needRemoveTricks.Contains(trickInfo.Key);
						if (flag4)
						{
							RectTransform trickObj;
							bool flag5 = this._trickObjs.TryGetValue(trickInfo.Key, out trickObj);
							if (flag5)
							{
								Tweener tween;
								bool flag6 = this._trickRect2ScaleTween.TryGetValue(trickObj, out tween);
								if (flag6)
								{
									tween.Kill(false);
								}
								trickObj.localScale = Vector3.one;
								this._trickRect2ScaleTween[trickObj] = trickObj.DOScale(1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
								trickObj.GetComponent<CombatTrickPrefab>().costPreview.SetActive(true);
							}
							foundTrickCount++;
							bool flag7 = foundTrickCount == (int)needTrick.NeedCount;
							if (flag7)
							{
								break;
							}
						}
					}
				}
			}
			else
			{
				foreach (RectTransform trickTransform in this._trickObjs.Values)
				{
					Tweener tween2;
					bool flag8 = this._trickRect2ScaleTween.TryGetValue(trickTransform, out tween2);
					if (flag8)
					{
						tween2.Kill(false);
					}
					this._trickRect2ScaleTween.Remove(trickTransform);
					trickTransform.localScale = Vector3.one;
					trickTransform.GetComponent<CombatTrickPrefab>().costPreview.SetActive(false);
				}
			}
		}
	}

	// Token: 0x060012B7 RID: 4791 RVA: 0x000724F4 File Offset: 0x000706F4
	private void OnGetCanCostTrickDuringPreparingSkill()
	{
		bool flag = this.ally;
		if (flag)
		{
			this.RefreshTrickCollection(this.ally);
		}
	}

	// Token: 0x060012B8 RID: 4792 RVA: 0x0007251C File Offset: 0x0007071C
	private void UpdateTrickPos(bool doAni = true)
	{
		int index = this._trickObjs.Count - 1;
		foreach (KeyValuePair<int, RectTransform> trickObj in this._trickObjs)
		{
			RectTransform trickTransform = trickObj.Value;
			bool removedTrick = this._needRemoveTricks.Contains(trickObj.Key);
			Vector2 pos = removedTrick ? Vector2.zero : this._trickPointList[index];
			Vector2 trickPos = pos;
			if (doAni)
			{
				trickTransform.DOAnchorPos(trickPos, 0.2f, false);
			}
			else
			{
				trickTransform.anchoredPosition = trickPos;
			}
			bool flag = !removedTrick;
			if (flag)
			{
				index--;
			}
		}
		if (doAni)
		{
			DOVirtual.DelayedCall(0.2f, delegate
			{
				bool flag2 = this._needRemoveTricks.Count > 0;
				if (flag2)
				{
					this.RemoveTrickObjs();
				}
			}, true);
		}
	}

	// Token: 0x060012B9 RID: 4793 RVA: 0x00072604 File Offset: 0x00070804
	private void RemoveTrickObj(int trickKey)
	{
		this.ReturnTrickObj(trickKey);
		this._needRemoveTricks.Remove(trickKey);
	}

	// Token: 0x060012BA RID: 4794 RVA: 0x0007261C File Offset: 0x0007081C
	private float CalcTrickMaskPositionX(int maxTrickCount)
	{
		return this._trickMaskInitPos.x + (float)((maxTrickCount - 9) * -10 * (this.Flip ? -1 : 1));
	}

	// Token: 0x04000FD8 RID: 4056
	private const int UiDefaultMaxTrickCount = 9;

	// Token: 0x04000FD9 RID: 4057
	private const int TrickSpacing = -10;

	// Token: 0x04000FDA RID: 4058
	[SerializeField]
	private bool ally;

	// Token: 0x04000FDB RID: 4059
	[SerializeField]
	private RectTransform trickQueue;

	// Token: 0x04000FDC RID: 4060
	[SerializeField]
	private RectTransform trickMask;

	// Token: 0x04000FDD RID: 4061
	[SerializeField]
	private RectTransform spawnPoint;

	// Token: 0x04000FDE RID: 4062
	[SerializeField]
	private RectTransform trickPointParent;

	// Token: 0x04000FDF RID: 4063
	[SerializeField]
	private ParticleSystem effTrickSpawnHint;

	// Token: 0x04000FE0 RID: 4064
	private Vector2 _trickMaskInitPos;

	// Token: 0x04000FE1 RID: 4065
	private bool _lastCanCostTrickDuringPreparingSkill;

	// Token: 0x04000FE2 RID: 4066
	private int _lastMaxTrick;

	// Token: 0x04000FE3 RID: 4067
	private readonly SortedDictionary<int, sbyte> _lastTricks = new SortedDictionary<int, sbyte>();

	// Token: 0x04000FE4 RID: 4068
	private readonly SortedDictionary<int, RectTransform> _trickObjs = new SortedDictionary<int, RectTransform>();

	// Token: 0x04000FE5 RID: 4069
	private readonly List<int> _needRemoveTricks = new List<int>();

	// Token: 0x04000FE6 RID: 4070
	private bool _doingChangeMaxTrickAni;

	// Token: 0x04000FE7 RID: 4071
	private readonly Dictionary<RectTransform, Tweener> _trickRect2ScaleTween = new Dictionary<RectTransform, Tweener>();

	// Token: 0x04000FE8 RID: 4072
	private readonly List<Vector2> _trickPointList = new List<Vector2>();
}
