using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Adventure;
using Game.Views.Adventure;
using Game.Views.World;
using GameData.Adventure;
using GameData.Common;
using GameData.Domains.Adventure;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item.Display;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Google.Protobuf.Collections;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000165 RID: 357
public class UI_AdventureMajorEvent : UIBase
{
	// Token: 0x17000230 RID: 560
	// (get) Token: 0x0600139F RID: 5023 RVA: 0x00079E6B File Offset: 0x0007806B
	private static AdventureRemakeModel AdventureRemakeModel
	{
		get
		{
			return SingletonObject.getInstance<AdventureRemakeModel>();
		}
	}

	// Token: 0x17000231 RID: 561
	// (get) Token: 0x060013A0 RID: 5024 RVA: 0x00079E72 File Offset: 0x00078072
	private AdventureMajorEvent MajorEvent
	{
		get
		{
			return UI_AdventureMajorEvent.AdventureRemakeModel.AdventureMajorEventDict[this._majorEventId];
		}
	}

	// Token: 0x17000232 RID: 562
	// (get) Token: 0x060013A1 RID: 5025 RVA: 0x00079E89 File Offset: 0x00078089
	private AdventureMajorEventData MajorEventData
	{
		get
		{
			return this.MajorEvent.Core;
		}
	}

	// Token: 0x17000233 RID: 563
	// (get) Token: 0x060013A2 RID: 5026 RVA: 0x00079E96 File Offset: 0x00078096
	private AdventureMajorEventTaiwu MajorEventRuntime
	{
		get
		{
			return UI_AdventureMajorEvent.AdventureRemakeModel.AdventureMajorEventTaiwu;
		}
	}

	// Token: 0x060013A3 RID: 5027 RVA: 0x00079EA4 File Offset: 0x000780A4
	public override void OnInit(ArgumentBox argsBox)
	{
		bool flag = argsBox == null;
		if (flag)
		{
			this.NeedDataListenerId = true;
		}
		else
		{
			argsBox.Get("MajorEventId", out this._majorEventId);
			this.Init();
			this.ResetData();
			this.ResetDisplay();
			this.DrawNodeAndLine();
			this.DrawLine();
			this.SetTaiwuIcon();
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
		}
	}

	// Token: 0x060013A4 RID: 5028 RVA: 0x00079F30 File Offset: 0x00078130
	private void ResetDisplay()
	{
		this.adventureRemakeFinish.Finish = false;
		UIRectDragMove mapDragRoot = base.CGet<UIRectDragMove>("MapDragRoot");
		RectTransform mapDragRootRect = mapDragRoot.GetComponent<RectTransform>();
		mapDragRootRect.localScale = Vector3.one;
		CanvasGroup animBg = base.CGet<CanvasGroup>("AnimBg");
		CanvasGroup animUI = base.CGet<CanvasGroup>("AnimUI");
		CanvasGroup animCarrier = base.CGet<CanvasGroup>("AnimCarrier");
		CanvasGroup nodeRootCanvas = base.CGet<RectTransform>("NodeRoot").GetComponent<CanvasGroup>();
		CanvasGroup startNodeRootCanvas = base.CGet<RectTransform>("StartNodeRoot").GetComponent<CanvasGroup>();
		CanvasGroup taiwuIconCanvas = base.CGet<RectTransform>("TaiwuIcon").GetComponent<CanvasGroup>();
		CanvasGroup lineRoot = base.CGet<RectTransform>("LineRoot").GetComponent<CanvasGroup>();
		animBg.alpha = 0f;
		animUI.alpha = 0f;
		animCarrier.alpha = 0f;
		nodeRootCanvas.alpha = 0f;
		startNodeRootCanvas.alpha = 0f;
		taiwuIconCanvas.alpha = 0f;
		lineRoot.alpha = 0f;
	}

	// Token: 0x060013A5 RID: 5029 RVA: 0x0007A030 File Offset: 0x00078230
	private IEnumerator Show()
	{
		CanvasGroup animBg = base.CGet<CanvasGroup>("AnimBg");
		CanvasGroup animCarrier = base.CGet<CanvasGroup>("AnimCarrier");
		CanvasGroup animUI = base.CGet<CanvasGroup>("AnimUI");
		CanvasGroup startNodeRootCanvas = base.CGet<RectTransform>("StartNodeRoot").GetComponent<CanvasGroup>();
		CanvasGroup nodeRootCanvas = base.CGet<RectTransform>("NodeRoot").GetComponent<CanvasGroup>();
		CanvasGroup taiwuIconCanvas = base.CGet<RectTransform>("TaiwuIcon").GetComponent<CanvasGroup>();
		CanvasGroup lineRoot = base.CGet<RectTransform>("LineRoot").GetComponent<CanvasGroup>();
		this._animSequence = DOTween.Sequence();
		TweenerCore<float, float, FloatOptions> bg = animBg.DOFade(1f, this.bgFadeTime).SetEase(Ease.OutQuart);
		TweenerCore<float, float, FloatOptions> carrier = animCarrier.DOFade(1f, this.carrierFadeTime).SetEase(Ease.OutQuart);
		TweenerCore<float, float, FloatOptions> ui = animUI.DOFade(1f, this.uiFadeTime).SetEase(Ease.OutQuart);
		TweenerCore<float, float, FloatOptions> startNode = startNodeRootCanvas.DOFade(1f, this.startNodeFadeTime).SetEase(Ease.OutQuart);
		TweenerCore<float, float, FloatOptions> taiwuIcon = taiwuIconCanvas.DOFade(1f, this.startNodeFadeTime).SetEase(Ease.OutQuart);
		TweenerCore<float, float, FloatOptions> otherNode = nodeRootCanvas.DOFade(1f, this.otherNodeFadeTime).SetEase(Ease.OutQuart);
		TweenerCore<float, float, FloatOptions> line = lineRoot.DOFade(1f, this.otherNodeFadeTime).SetEase(Ease.OutQuart);
		this._animSequence.Append(bg);
		this._animSequence.Append(carrier);
		this._animSequence.Append(ui);
		this._animSequence.Append(startNode);
		this._animSequence.Join(taiwuIcon);
		this._animSequence.Append(otherNode);
		this._animSequence.Join(line);
		this._animSequence.Play<DG.Tweening.Sequence>().OnComplete(delegate
		{
			AdventureDomainMethod.Call.PostEnterMajorEvent(this.Element.GameDataListenerId);
			base.DelayCall(delegate
			{
				this.SetMovingState(false);
			}, 0.5f);
		});
		GEvent.OnEvent(UiEvents.AdventureRemakeOpenPartTwo, null);
		yield return null;
		yield break;
	}

	// Token: 0x17000234 RID: 564
	// (get) Token: 0x060013A6 RID: 5030 RVA: 0x0007A03F File Offset: 0x0007823F
	private int TaiwuCharId
	{
		get
		{
			return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		}
	}

	// Token: 0x060013A7 RID: 5031 RVA: 0x0007A04B File Offset: 0x0007824B
	private void OnListenerIdReady()
	{
		this.SetTaiwuLocation();
		this.InitAtmosphere();
		base.StartCoroutine(this.Show());
	}

	// Token: 0x060013A8 RID: 5032 RVA: 0x0007A06C File Offset: 0x0007826C
	private void RequestAllData()
	{
		CharacterDomainMethod.Call.GetPersonalities(this.Element.GameDataListenerId, this.TaiwuCharId);
		CharacterDomainMethod.Call.GetAllLifeSkillAttainment(this.Element.GameDataListenerId, this.TaiwuCharId);
		CharacterDomainMethod.Call.GetAllCombatSkillAttainment(this.Element.GameDataListenerId, this.TaiwuCharId);
	}

	// Token: 0x060013A9 RID: 5033 RVA: 0x0007A0C0 File Offset: 0x000782C0
	private void Init()
	{
		bool isInit = this._isInit;
		if (!isInit)
		{
			this._isInit = true;
			for (int i = 0; i < this.nodePrefab.Length; i++)
			{
				Refers refer = this.nodePrefab[i];
				PoolManager.SetSrcObject(this._nodePrefabKeys[i], refer.gameObject);
			}
			Refers linePrefab = base.CGet<Refers>("LinePrefab");
			PoolManager.SetSrcObject("UI_AdventureMajorEventLinePrefab", linePrefab.gameObject);
			PoolManager.SetSrcObject("UI_AdventureMajorEventDecorationPrefab", base.CGet<GameObject>("DecorationPrefab"));
		}
	}

	// Token: 0x060013AA RID: 5034 RVA: 0x0007A14C File Offset: 0x0007834C
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
					bool flag = notification.DomainId == 4;
					if (flag)
					{
						bool flag2 = notification.MethodId == 204;
						if (flag2)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._personalities);
						}
						bool flag3 = notification.MethodId == 90;
						if (flag3)
						{
							Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._lifeSkillAttainments);
						}
						else
						{
							bool flag4 = notification.MethodId == 88;
							if (flag4)
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._combatSkillAttainments);
								this.Element.ShowAfterRefresh();
								this.RefreshBtnInteract();
							}
						}
					}
				}
			}
			else
			{
				DataUid uid = notification.Uid;
			}
		}
	}

	// Token: 0x060013AB RID: 5035 RVA: 0x0007A27C File Offset: 0x0007847C
	private void OnDestroy()
	{
		for (int i = 0; i < this.nodePrefab.Length; i++)
		{
			PoolManager.RemoveData(this._nodePrefabKeys[i]);
		}
		PoolManager.RemoveData("UI_AdventureMajorEventLinePrefab");
		PoolManager.RemoveData("UI_AdventureMajorEventDecorationPrefab");
	}

	// Token: 0x060013AC RID: 5036 RVA: 0x0007A2C8 File Offset: 0x000784C8
	private void OnEnable()
	{
		GEvent.OnEvent(UiEvents.RequestBottomTimeDisk, null);
		GEvent.Add(UiEvents.OnAdventureMajorEventTaiwuChanged, new GEvent.Callback(this.OnAdventureMajorEventTaiwuChanged));
		GEvent.Add(UiEvents.OnEventWindowEnded, new GEvent.Callback(this.OnEventWindowEnded));
		GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
		GEvent.Add(UiEvents.AdventureResetCamera, new GEvent.Callback(this.AdventureResetCamera));
		GEvent.Add(UiEvents.OnCharacterTaiwuCarrierChanged, new GEvent.Callback(this.OnCharacterTaiwuCarrierChanged));
		GEvent.Add(UiEvents.MajorEventSkipCompleteAnim, new GEvent.Callback(this.MajorEventSkipCompleteAnim));
	}

	// Token: 0x060013AD RID: 5037 RVA: 0x0007A38C File Offset: 0x0007858C
	private void OnDisable()
	{
		GEvent.OnEvent(UiEvents.ReturnBottomTimeDisk, null);
		GEvent.Remove(UiEvents.OnAdventureMajorEventTaiwuChanged, new GEvent.Callback(this.OnAdventureMajorEventTaiwuChanged));
		GEvent.Remove(UiEvents.OnEventWindowEnded, new GEvent.Callback(this.OnEventWindowEnded));
		GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.TopUiChanged));
		GEvent.Remove(UiEvents.AdventureResetCamera, new GEvent.Callback(this.AdventureResetCamera));
		GEvent.Remove(UiEvents.OnCharacterTaiwuCarrierChanged, new GEvent.Callback(this.OnCharacterTaiwuCarrierChanged));
		GEvent.Remove(UiEvents.MajorEventSkipCompleteAnim, new GEvent.Callback(this.MajorEventSkipCompleteAnim));
		this.StopEventEndedAtmosphereSwitch();
	}

	// Token: 0x060013AE RID: 5038 RVA: 0x0007A458 File Offset: 0x00078658
	private void AdventureResetCamera(ArgumentBox argBox)
	{
		bool moving = this._moving;
		if (!moving)
		{
			this.SetCurrentNodeCenter(true);
		}
	}

	// Token: 0x060013AF RID: 5039 RVA: 0x0007A47C File Offset: 0x0007867C
	private void TopUiChanged(ArgumentBox argBox)
	{
		bool flag = UIManager.Instance.IsFocusElement(UIElement.SystemOption);
		if (flag)
		{
			this.SetTimeScale(0f);
			this.SetMovingState(false);
		}
		else
		{
			this.SetTimeScale(1f);
		}
	}

	// Token: 0x060013B0 RID: 5040 RVA: 0x0007A4C3 File Offset: 0x000786C3
	private void SetTimeScale(float scale)
	{
		Time.timeScale = scale;
	}

	// Token: 0x060013B1 RID: 5041 RVA: 0x0007A4D0 File Offset: 0x000786D0
	private void SetTaiwuIcon()
	{
		WorldMapModel worldMapModel = SingletonObject.getInstance<WorldMapModel>();
		string spriteName = string.Format("adventure_main_taiwu_{0}", worldMapModel.TaiwuGender);
		base.CGet<RectTransform>("TaiwuIcon").GetComponent<CImage>().SetSprite(spriteName, false, null);
	}

	// Token: 0x060013B2 RID: 5042 RVA: 0x0007A514 File Offset: 0x00078714
	private void RefreshBtnInteract()
	{
		bool notInAdventure = this.MajorEventRuntime.NotInAdventure;
		if (!notInAdventure)
		{
			foreach (Refers refer in this._nodeRefersDict.Values)
			{
				GameObject arrow;
				bool flag = refer.CTryGet<GameObject>("Arrow", out arrow);
				if (flag)
				{
					arrow.gameObject.SetActive(false);
				}
				this.SetNodeBtnInteract(refer, false);
			}
			Refers refer2 = this._nodeRefersDict[this.MajorEventRuntime.CurrentMain];
			this.SetNodeBtnInteract(refer2, false);
			CImage icon = refer2.CGet<CImage>("Icon");
			icon.SetSprite(this.GetNodeImage(this.MajorEventRuntime.CurrentMain, 4), false, null);
			AdventureMajorEventNodeData currentNode = this.MajorEventData.Nodes[this.MajorEventRuntime.CurrentMain];
			for (int i = 0; i < currentNode.NextNodes.Count; i++)
			{
				int nextNodeIndex = currentNode.NextNodes[i];
				bool flag2 = this.MajorEventRuntime.UnlockedNodes.Contains(nextNodeIndex);
				if (flag2)
				{
					AdventureMajorEventNodeData nextNode = this.MajorEventData.Nodes[nextNodeIndex];
					Refers refer3 = this._nodeRefersDict[nextNodeIndex];
					bool meet = this.RefreshNodeRequirements(nextNode, nextNodeIndex, refer3);
					this.SetNodeBtnInteract(refer3, meet);
					refer3.CGet<GameObject>("Arrow").gameObject.SetActive(meet);
					CImage icon2 = refer3.CGet<CImage>("Icon");
					icon2.SetSprite(this.GetNodeImage(nextNodeIndex, meet ? 1 : 2), false, null);
				}
				else
				{
					bool flag3 = this.MajorEventRuntime.VisitedNodes.Contains(nextNodeIndex);
					if (flag3)
					{
						Refers refer4 = this._nodeRefersDict[nextNodeIndex];
						this.SetNodeBtnInteract(refer4, false);
						CImage icon3 = refer4.CGet<CImage>("Icon");
						icon3.SetSprite(this.GetNodeImage(nextNodeIndex, 3), false, null);
					}
				}
			}
			this.RefreshAllNodeRequirements();
			this.RefreshAllLineState();
		}
	}

	// Token: 0x060013B3 RID: 5043 RVA: 0x0007A74C File Offset: 0x0007894C
	private void SetNodeBtnInteract(Refers node, bool enable)
	{
		CButton nodeBtn = node.CGet<CButton>("NodeBtn");
		nodeBtn.interactable = enable;
		CImage icon = node.CGet<CImage>("Icon");
		PointerTrigger pointerTrigger = icon.GetComponent<PointerTrigger>();
		bool flag = pointerTrigger != null;
		if (flag)
		{
			pointerTrigger.enabled = enable;
		}
	}

	// Token: 0x060013B4 RID: 5044 RVA: 0x0007A794 File Offset: 0x00078994
	private string GetNodeImage(int nodeIndex, sbyte operateType)
	{
		AdventureMajorEventNodeData nodeData = this.MajorEventData.Nodes[nodeIndex];
		return AdventureMajorEventTool.GetAdventureMajorEventNodeIcon(nodeData.Type, nodeData.Style, operateType);
	}

	// Token: 0x060013B5 RID: 5045 RVA: 0x0007A7CC File Offset: 0x000789CC
	private void OnAdventureMajorEventTaiwuChanged(ArgumentBox argBox)
	{
		bool notInAdventure = this.MajorEventRuntime.NotInAdventure;
		if (notInAdventure)
		{
			this.DestroyLinePrefab();
			this.DestroyNodePrefab();
			this.DestroyDecorationPrefab();
			this.AdventureRemakeFinish();
		}
		else
		{
			this.RequestAllData();
			this.CheckAtmosphereOnEnterNode();
		}
	}

	// Token: 0x060013B6 RID: 5046 RVA: 0x0007A818 File Offset: 0x00078A18
	private void OnEventWindowEnded(ArgumentBox argBox)
	{
		bool notInAdventure = this.MajorEventRuntime.NotInAdventure;
		if (!notInAdventure)
		{
			this.ScheduleAtmosphereSwitchOnEventEnded();
			bool flag = UIManager.Instance.IsFocusElement(this.Element);
			if (flag)
			{
				this.RefreshEventImgState();
			}
			this.RequestAllData();
		}
	}

	// Token: 0x060013B7 RID: 5047 RVA: 0x0007A860 File Offset: 0x00078A60
	private void ScheduleAtmosphereSwitchOnEventEnded()
	{
		this.StopEventEndedAtmosphereSwitch();
		this._eventEndedAtmosphereSwitchCoroutine = base.StartCoroutine(this.DelaySwitchAtmosphereOnEventEnded());
	}

	// Token: 0x060013B8 RID: 5048 RVA: 0x0007A87C File Offset: 0x00078A7C
	private void StopEventEndedAtmosphereSwitch()
	{
		bool flag = this._eventEndedAtmosphereSwitchCoroutine == null;
		if (!flag)
		{
			base.StopCoroutine(this._eventEndedAtmosphereSwitchCoroutine);
			this._eventEndedAtmosphereSwitchCoroutine = null;
		}
	}

	// Token: 0x060013B9 RID: 5049 RVA: 0x0007A8AD File Offset: 0x00078AAD
	private IEnumerator DelaySwitchAtmosphereOnEventEnded()
	{
		yield return new WaitForSeconds(0.5f);
		this._eventEndedAtmosphereSwitchCoroutine = null;
		this.CheckAtmosphereOnEventEnded();
		yield break;
	}

	// Token: 0x060013BA RID: 5050 RVA: 0x0007A8BC File Offset: 0x00078ABC
	private int GetPendingAtmosphereType()
	{
		bool notInAdventure = this.MajorEventRuntime.NotInAdventure;
		int result;
		if (notInAdventure)
		{
			result = -1;
		}
		else
		{
			AdventureMajorEvent majorEvent = this.MajorEventRuntime.MajorEvent;
			bool flag = majorEvent == null;
			if (flag)
			{
				result = -1;
			}
			else
			{
				AdventureParameterValue value;
				result = (majorEvent.TryGetParameter("ConchShipPresetKey_MajorEventAtmosphereType", out value) ? value.Current : -1);
			}
		}
		return result;
	}

	// Token: 0x060013BB RID: 5051 RVA: 0x0007A914 File Offset: 0x00078B14
	private void InitAtmosphere()
	{
		bool flag = this.atmosphereSwitcher == null;
		if (!flag)
		{
			bool notInAdventure = this.MajorEventRuntime.NotInAdventure;
			if (!notInAdventure)
			{
				int pending = this.GetPendingAtmosphereType();
				int targetAtmosphereType = (pending >= 0) ? pending : this.MajorEventData.Nodes[this.MajorEventRuntime.Current].AtmosphereType;
				bool flag2 = targetAtmosphereType >= 0;
				if (flag2)
				{
					this.atmosphereSwitcher.SetAtmosphereImmediate(targetAtmosphereType);
				}
			}
		}
	}

	// Token: 0x060013BC RID: 5052 RVA: 0x0007A990 File Offset: 0x00078B90
	private void CheckAtmosphereOnEnterNode()
	{
		bool flag = this.atmosphereSwitcher == null;
		if (!flag)
		{
			bool notInAdventure = this.MajorEventRuntime.NotInAdventure;
			if (!notInAdventure)
			{
				int pending = this.GetPendingAtmosphereType();
				int targetAtmosphereType = (pending >= 0) ? pending : this.MajorEventData.Nodes[this.MajorEventRuntime.Current].AtmosphereType;
				bool flag2 = this.atmosphereSwitcher.CurrentAtmosphereType != -1;
				if (flag2)
				{
					this.atmosphereSwitcher.SwitchAtmosphere(targetAtmosphereType);
				}
				else
				{
					this.atmosphereSwitcher.SetAtmosphereImmediate(targetAtmosphereType);
				}
			}
		}
	}

	// Token: 0x060013BD RID: 5053 RVA: 0x0007AA28 File Offset: 0x00078C28
	private void CheckAtmosphereOnEventEnded()
	{
		bool flag = this.atmosphereSwitcher == null;
		if (!flag)
		{
			bool notInAdventure = this.MajorEventRuntime.NotInAdventure;
			if (!notInAdventure)
			{
				int pending = this.GetPendingAtmosphereType();
				bool flag2 = pending >= 0;
				if (flag2)
				{
					bool flag3 = this.atmosphereSwitcher.CurrentAtmosphereType != -1;
					if (flag3)
					{
						this.atmosphereSwitcher.SwitchAtmosphere(pending);
					}
					else
					{
						this.atmosphereSwitcher.SetAtmosphereImmediate(pending);
					}
				}
			}
		}
	}

	// Token: 0x060013BE RID: 5054 RVA: 0x0007AAA4 File Offset: 0x00078CA4
	private void SetTaiwuLocation()
	{
		Refers nodeRefer = this._nodeRefersDict[this.MajorEventRuntime.Current];
		RectTransform taiwuIcon = base.CGet<RectTransform>("TaiwuIcon");
		taiwuIcon.position = nodeRefer.GetComponent<RectTransform>().position;
		taiwuIcon.ChangeLocalPositionY(40f);
	}

	// Token: 0x060013BF RID: 5055 RVA: 0x0007AAF4 File Offset: 0x00078CF4
	private float SetCurrentNodeCenter(bool tween)
	{
		bool flag = this.MajorEventRuntime.Current < 0;
		float result;
		if (flag)
		{
			result = 0f;
		}
		else
		{
			RectTransform nodeReferRect = this._nodeRefersDict[this.MajorEventRuntime.Current].GetComponent<RectTransform>();
			UIRectDragMove mapDragRoot = base.CGet<UIRectDragMove>("MapDragRoot");
			RectTransform mapDragRootRect = mapDragRoot.GetComponent<RectTransform>();
			if (tween)
			{
				Vector2 startPos = mapDragRootRect.anchoredPosition;
				Vector2 endPos = new Vector2(-nodeReferRect.anchoredPosition.x, -nodeReferRect.anchoredPosition.y + 200f);
				float distance = Vector2.Distance(startPos, endPos);
				float duration = Math.Min(1.5f, distance * 0.0005f);
				DOVirtual.Float(0f, 1f, duration, delegate(float stepVal)
				{
					mapDragRootRect.anchoredPosition = Vector2.Lerp(startPos, mapDragRootRect.localScale.x * endPos, stepVal);
				});
				result = duration;
			}
			else
			{
				mapDragRootRect.anchoredPosition = new Vector3(-nodeReferRect.anchoredPosition.x * mapDragRootRect.localScale.x, (-nodeReferRect.anchoredPosition.y + 200f) * mapDragRootRect.localScale.x, 0f);
				result = 0f;
			}
		}
		return result;
	}

	// Token: 0x060013C0 RID: 5056 RVA: 0x0007AC6C File Offset: 0x00078E6C
	private void RefreshEventImgState()
	{
		bool notInAdventure = this.MajorEventRuntime.NotInAdventure;
		if (!notInAdventure)
		{
			for (int nodeIndex = 0; nodeIndex < this.MajorEventData.Nodes.Count; nodeIndex++)
			{
				AdventureMajorEventNodeData node = this.MajorEventData.Nodes[nodeIndex];
				Refers refers = this._nodeRefersDict[nodeIndex];
				bool flag = node.Type == EAdventureMajorEventNodeType.Check;
				if (!flag)
				{
					CRawImage eventImg = refers.CGet<CRawImage>("EventImg");
					bool flag2 = this.MajorEventRuntime.VisitedNodes.Contains(nodeIndex) && !eventImg.gameObject.activeSelf;
					if (flag2)
					{
						CanvasGroup canvasGroup = eventImg.GetComponent<CanvasGroup>();
						canvasGroup.alpha = 0f;
						eventImg.gameObject.SetActive(true);
						canvasGroup.DOFade(1f, 1f).SetEase(Ease.OutCubic);
					}
				}
			}
		}
	}

	// Token: 0x060013C1 RID: 5057 RVA: 0x0007AD60 File Offset: 0x00078F60
	private void SetEventImg()
	{
		for (int nodeIndex = 0; nodeIndex < this.MajorEventData.Nodes.Count; nodeIndex++)
		{
			AdventureMajorEventNodeData node = this.MajorEventData.Nodes[nodeIndex];
			this.SetEventImg(node, nodeIndex);
		}
	}

	// Token: 0x060013C2 RID: 5058 RVA: 0x0007ADAC File Offset: 0x00078FAC
	private void SetEventImg(AdventureMajorEventNodeData node, int index)
	{
		Refers refers = this._nodeRefersDict[index];
		CRawImage tex;
		bool flag = !refers.CTryGet<CRawImage>("EventImg", out tex);
		if (!flag)
		{
			bool flag2 = node.Type != EAdventureMajorEventNodeType.Check;
			if (flag2)
			{
				string texName = node.EventTexture;
				bool flag3 = string.IsNullOrEmpty(texName);
				if (flag3)
				{
					texName = SingletonObject.getInstance<WorldMapModel>().PlayerAtBlock.GetConfig().EventBack;
				}
				UI_AdventureMajorEvent.LoadEventTexture(tex, texName, false);
			}
			else
			{
				bool flag4 = tex && tex.gameObject.activeSelf;
				if (flag4)
				{
					tex.gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x060013C3 RID: 5059 RVA: 0x0007AE50 File Offset: 0x00079050
	public static void LoadEventTexture(CRawImage eventRawImage, string eventTexture, bool activeSelf)
	{
		bool flag = string.IsNullOrEmpty(eventTexture);
		if (flag)
		{
			eventTexture = "tex_otherevent_temp_0";
		}
		bool activeSelf2 = eventRawImage.gameObject.activeSelf;
		if (activeSelf2)
		{
			eventRawImage.gameObject.SetActive(false);
		}
		EventTextureManager eventTextureManager = SingletonObject.getInstance<EventTextureManager>();
		string texturePath;
		bool eventBackPath = eventTextureManager.GetEventBackPath(eventTexture, out texturePath);
		if (eventBackPath)
		{
			ResLoader.LoadModOrGameResource<Texture2D>(texturePath, delegate(Texture2D texture)
			{
				eventRawImage.texture = texture;
				eventRawImage.enabled = true;
				bool activeSelf3 = activeSelf;
				if (activeSelf3)
				{
					eventRawImage.gameObject.SetActive(true);
				}
			}, null);
		}
	}

	// Token: 0x060013C4 RID: 5060 RVA: 0x0007AED8 File Offset: 0x000790D8
	private void DrawNodeAndLine()
	{
		for (int nodeIndex = 0; nodeIndex < this.MajorEventData.Nodes.Count; nodeIndex++)
		{
			AdventureMajorEventNodeData node = this.MajorEventData.Nodes[nodeIndex];
			Refers refer = this.CreateNodeRefers(node, nodeIndex);
			this.SetNodeInfo(refer, node, nodeIndex);
			this.SetLineData(node, nodeIndex);
		}
		this.SetEventImg();
	}

	// Token: 0x060013C5 RID: 5061 RVA: 0x0007AF3C File Offset: 0x0007913C
	private void SetNodeInfo(Refers refer, AdventureMajorEventNodeData node, int nodeIndex)
	{
		CImage icon = refer.CGet<CImage>("Icon");
		icon.SetSprite(this.GetNodeImage(nodeIndex, 0), false, null);
		refer.CGet<CButton>("NodeBtn").interactable = false;
		refer.RectTransform.anchoredPosition = new Vector3(node.X, node.Y, 0f);
		TextMeshProUGUI nodeName = refer.CGet<TextMeshProUGUI>("NodeName");
		bool flag = string.IsNullOrEmpty(node.Name);
		if (flag)
		{
			nodeName.rectTransform.parent.gameObject.SetActive(false);
		}
		else
		{
			nodeName.rectTransform.parent.gameObject.SetActive(true);
			nodeName.SetText(node.Name, true);
		}
		TemplatedContainerAssembly container;
		bool flag2 = refer.CTryGet<TemplatedContainerAssembly>("RequirementsHolder", out container);
		if (flag2)
		{
			container.Rebuild(0, null);
		}
	}

	// Token: 0x060013C6 RID: 5062 RVA: 0x0007B01C File Offset: 0x0007921C
	private void DrawLine()
	{
		for (int i = 0; i < this._lineDataList.Count; i++)
		{
			ValueTuple<int, int> lineData = this._lineDataList[i];
			Refers startRefer = this._nodeRefersDict[lineData.Item1];
			Refers endRefer = this._nodeRefersDict[lineData.Item2];
			Refers lineRefers = this.GetLineRefers();
			this._lineRefersList.Add(new ValueTuple<int, int, Refers>(lineData.Item1, lineData.Item2, lineRefers));
			CImage linSprite = lineRefers.CGet<CImage>("Icon");
			linSprite.SetSprite(AdventureMajorEventTool.GetLineSpriteName(0, 0), false, null);
			linSprite.type = Image.Type.Sliced;
			RectTransform lineRect = linSprite.GetComponent<RectTransform>();
			float lineWidth = AdventureMajorEventTool.GetLineWidth(startRefer.GetComponent<RectTransform>(), endRefer.GetComponent<RectTransform>());
			lineRect.SetWidth(lineWidth);
			lineRect.SetHeight(6f);
			lineRefers.RectTransform.localPosition = AdventureMajorEventTool.GetCenterPos(startRefer.RectTransform, endRefer.RectTransform);
			lineRefers.RectTransform.rotation = AdventureMajorEventTool.GetQuaternion(startRefer.RectTransform, endRefer.RectTransform);
			lineRefers.RectTransform.Rotate(0f, 0f, 90f);
		}
		this.SetCurrentNodeCenter(false);
	}

	// Token: 0x060013C7 RID: 5063 RVA: 0x0007B164 File Offset: 0x00079364
	private void ResetData()
	{
		this.SetMovingState(true);
		this.PlayCarrierAnim(false);
		this.DestroyLinePrefab();
		this.DestroyNodePrefab();
		this.DestroyDecorationPrefab();
		this._nodeRefersDict.Clear();
		this._lineDataList.Clear();
		this._lineRefersList.Clear();
		this._skipCompleteAnim = false;
	}

	// Token: 0x060013C8 RID: 5064 RVA: 0x0007B1C4 File Offset: 0x000793C4
	private void SetLineData(AdventureMajorEventNodeData node, int nodeIndex)
	{
		foreach (int nextNodeIndex in node.NextNodes)
		{
			this._lineDataList.Add(new ValueTuple<int, int>(nodeIndex, nextNodeIndex));
		}
	}

	// Token: 0x060013C9 RID: 5065 RVA: 0x0007B224 File Offset: 0x00079424
	private Refers CreateNodeRefers(AdventureMajorEventNodeData node, int nodeIndex)
	{
		string prefabKey = this._nodePrefabKeys[node.Type.ToInt()];
		Refers nodeRefers = PoolManager.GetObject<Refers>(prefabKey);
		nodeRefers.UserInt = node.Type.ToInt();
		nodeRefers.gameObject.SetActive(true);
		CImage icon = nodeRefers.CGet<CImage>("Icon");
		PointerTrigger pointerTrigger = icon.GetComponent<PointerTrigger>();
		bool flag = pointerTrigger != null;
		if (flag)
		{
			pointerTrigger.enabled = false;
		}
		nodeRefers.transform.SetParent(base.CGet<RectTransform>((node.Type == EAdventureMajorEventNodeType.Start) ? "StartNodeRoot" : "NodeRoot"), false);
		CButton nodeBtn = nodeRefers.CGet<CButton>("NodeBtn");
		nodeBtn.onClick.RemoveAllListeners();
		nodeBtn.onClick.AddListener(delegate()
		{
			bool moving = this._moving;
			if (!moving)
			{
				this.StartCoroutine(this.PlayMoveAnim(nodeIndex));
			}
		});
		this._nodeRefersDict.Add(nodeIndex, nodeRefers);
		return nodeRefers;
	}

	// Token: 0x060013CA RID: 5066 RVA: 0x0007B328 File Offset: 0x00079528
	private Refers GetLineRefers()
	{
		Refers lineRefers = PoolManager.GetObject<Refers>("UI_AdventureMajorEventLinePrefab");
		lineRefers.gameObject.SetActive(true);
		lineRefers.transform.SetParent(base.CGet<RectTransform>("LineRoot"), false);
		return lineRefers;
	}

	// Token: 0x060013CB RID: 5067 RVA: 0x0007B36B File Offset: 0x0007956B
	private void DestroyLinePrefab()
	{
		this._lineRefersList.ForEach(delegate([TupleElementNames(new string[]
		{
			"startIndex",
			"endIndex",
			"lineRefer"
		})] ValueTuple<int, int, Refers> e)
		{
			bool flag = e.Item3 == null;
			if (!flag)
			{
				PoolManager.Destroy("UI_AdventureMajorEventLinePrefab", e.Item3.gameObject);
			}
		});
		this._lineDataList.Clear();
	}

	// Token: 0x060013CC RID: 5068 RVA: 0x0007B3A5 File Offset: 0x000795A5
	private void DestroyNodePrefab()
	{
		this._nodeRefersDict.Values.ToList<Refers>().ForEach(delegate(Refers e)
		{
			bool flag = e == null;
			if (!flag)
			{
				string prefabKey = this._nodePrefabKeys[e.UserInt];
				PoolManager.Destroy(prefabKey, e.gameObject);
			}
		});
	}

	// Token: 0x060013CD RID: 5069 RVA: 0x0007B3CC File Offset: 0x000795CC
	private void DestroyDecorationPrefab()
	{
		RectTransform decorationRoot = base.CGet<RectTransform>("DecorationRoot");
		for (int i = 0; i < decorationRoot.childCount; i++)
		{
			Transform child = decorationRoot.GetChild(i);
			PoolManager.Destroy("UI_AdventureMajorEventDecorationPrefab", child.gameObject);
		}
	}

	// Token: 0x060013CE RID: 5070 RVA: 0x0007B416 File Offset: 0x00079616
	private IEnumerator PlayMoveAnim(int endNodeIndex)
	{
		UI_AdventureMajorEvent.<>c__DisplayClass75_0 CS$<>8__locals1 = new UI_AdventureMajorEvent.<>c__DisplayClass75_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.endNodeIndex = endNodeIndex;
		this.SetMovingState(true);
		float time = this.SetCurrentNodeCenter(true);
		yield return new WaitForSeconds(time);
		this.PlayCarrierAnim(true);
		this.PlayCarrierSound();
		List<int> pathNodeIndexList = new List<int>
		{
			this.MajorEventRuntime.Current,
			CS$<>8__locals1.endNodeIndex
		};
		bool flag = this.MajorEventRuntime.CurrentMain != this.MajorEventRuntime.Current;
		if (flag)
		{
			pathNodeIndexList.Insert(1, this.MajorEventRuntime.CurrentMain);
		}
		CS$<>8__locals1.taiwuIcon = base.CGet<RectTransform>("TaiwuIcon");
		UIRectDragMove mapDragRoot = base.CGet<UIRectDragMove>("MapDragRoot");
		CS$<>8__locals1.mapDragRootRect = mapDragRoot.GetComponent<RectTransform>();
		DG.Tweening.Sequence sequence = DOTween.Sequence();
		int num;
		for (int i = 0; i < pathNodeIndexList.Count - 1; i = num + 1)
		{
			UI_AdventureMajorEvent.<>c__DisplayClass75_1 CS$<>8__locals2 = new UI_AdventureMajorEvent.<>c__DisplayClass75_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			Refers startNode = this._nodeRefersDict[pathNodeIndexList[i]];
			Refers endNode = this._nodeRefersDict[pathNodeIndexList[i + 1]];
			Vector2 startNodePos = startNode.GetComponent<RectTransform>().anchoredPosition;
			CS$<>8__locals2.startNodeTaiwuPos = new Vector2(startNodePos.x, startNodePos.y + 40f);
			CS$<>8__locals2.startNodeMapPos = new Vector2(startNodePos.x, startNodePos.y - 200f);
			Vector2 endNodePos = endNode.GetComponent<RectTransform>().anchoredPosition;
			CS$<>8__locals2.endNodeTaiwuPos = new Vector2(endNodePos.x, endNodePos.y + 40f);
			CS$<>8__locals2.endNodeMapPos = new Vector2(endNodePos.x, endNodePos.y - 200f);
			CS$<>8__locals2.CS$<>8__locals1.mapDragRootRect.pivot = new Vector2(0.5f, 0.5f);
			Tweener tween = DOVirtual.Float(0f, 1f, UI_AdventureMajorEvent.MoveStepTime, delegate(float stepVal)
			{
				CS$<>8__locals2.CS$<>8__locals1.mapDragRootRect.anchoredPosition = Vector2.Lerp(-1f * CS$<>8__locals2.CS$<>8__locals1.mapDragRootRect.localScale.x * CS$<>8__locals2.startNodeMapPos, -1f * CS$<>8__locals2.CS$<>8__locals1.mapDragRootRect.localScale.x * CS$<>8__locals2.endNodeMapPos, stepVal);
				CS$<>8__locals2.CS$<>8__locals1.taiwuIcon.anchoredPosition = Vector2.Lerp(CS$<>8__locals2.startNodeTaiwuPos, CS$<>8__locals2.endNodeTaiwuPos, stepVal);
			}).SetEase(Ease.InOutCubic);
			sequence.Append(tween);
			CS$<>8__locals2 = null;
			startNode = null;
			endNode = null;
			startNodePos = default(Vector2);
			endNodePos = default(Vector2);
			tween = null;
			num = i;
		}
		float delayTime = (float)(pathNodeIndexList.Count - 2) * UI_AdventureMajorEvent.MoveStepTime + UI_AdventureMajorEvent.MoveStepTime / 4f;
		base.StartCoroutine(this.RefreshPassedLine(CS$<>8__locals1.endNodeIndex, delayTime));
		sequence.Play<DG.Tweening.Sequence>().OnComplete(delegate
		{
			AdventureDomainMethod.Call.SelectMajorEvent(CS$<>8__locals1.<>4__this.Element.GameDataListenerId, CS$<>8__locals1.endNodeIndex);
			RefersBase <>4__this = CS$<>8__locals1.<>4__this;
			Action action;
			if ((action = CS$<>8__locals1.<>9__2) == null)
			{
				action = (CS$<>8__locals1.<>9__2 = delegate()
				{
					CS$<>8__locals1.<>4__this.SetMovingState(false);
				});
			}
			<>4__this.DelayCall(action, 0.5f);
			CS$<>8__locals1.<>4__this.PlayCarrierAnim(false);
			AudioManager.Instance.PlaySound("ui_adventure_appraisal_1", false, false);
		});
		yield break;
	}

	// Token: 0x060013CF RID: 5071 RVA: 0x0007B42C File Offset: 0x0007962C
	private void SetMovingState(bool state)
	{
		this._moving = state;
		base.CGet<GameObject>("BlockMask").gameObject.SetActive(state);
	}

	// Token: 0x060013D0 RID: 5072 RVA: 0x0007B450 File Offset: 0x00079650
	private void RefreshAllNodeRequirements()
	{
		for (int nodeIndex = 0; nodeIndex < this.MajorEventData.Nodes.Count; nodeIndex++)
		{
			AdventureMajorEventNodeData node = this.MajorEventData.Nodes[nodeIndex];
			Refers refer = this._nodeRefersDict[nodeIndex];
			this.RefreshNodeRequirements(node, nodeIndex, refer);
		}
	}

	// Token: 0x060013D1 RID: 5073 RVA: 0x0007B4A8 File Offset: 0x000796A8
	private bool RefreshNodeRequirements(AdventureMajorEventNodeData node, int nodeIndex, Refers refer)
	{
		bool flag = node.Type != EAdventureMajorEventNodeType.Check;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			bool meetRequirements = true;
			int totalCount = 0;
			int lifeSkillAttainmentsStartIndex = 0;
			int combatSkillAttainmentsStartIndex = 0;
			RepeatedField<AdventureCostResource> repeatedField = node.Requirements.Personalities;
			bool flag2 = repeatedField != null && repeatedField.Count > 0;
			if (flag2)
			{
				totalCount += node.Requirements.Personalities.Count;
				lifeSkillAttainmentsStartIndex = totalCount;
			}
			repeatedField = node.Requirements.LifeSkillAttainments;
			bool flag3 = repeatedField != null && repeatedField.Count > 0;
			if (flag3)
			{
				totalCount += node.Requirements.LifeSkillAttainments.Count;
				combatSkillAttainmentsStartIndex = totalCount;
			}
			repeatedField = node.Requirements.CombatSkillAttainments;
			bool flag4 = repeatedField != null && repeatedField.Count > 0;
			if (flag4)
			{
				totalCount += node.Requirements.CombatSkillAttainments.Count;
			}
			refer.CGet<TemplatedContainerAssembly>("RequirementsHolder").Rebuild(totalCount, delegate(Refers requireRefer, int index)
			{
				CImage icon = requireRefer.CGet<CImage>("Icon");
				TextMeshProUGUI requirementName = requireRefer.CGet<TextMeshProUGUI>("Name");
				TextMeshProUGUI count = requireRefer.CGet<TextMeshProUGUI>("Count");
				bool flag5 = index < lifeSkillAttainmentsStartIndex;
				if (flag5)
				{
					AdventureCostResource personality = node.Requirements.Personalities[index];
					icon.SetSprite("ui9_icon_personality_big_" + personality.Type.ToString(), false, null);
					requirementName.SetText(LocalStringManager.Get(UI_CharacterMenuTeam.PersonalityNameKeys[(int)((sbyte)personality.Type)]), true);
					bool flag6 = this._personalities[personality.Type] < personality.Value;
					if (flag6)
					{
						meetRequirements = false;
					}
					count.SetText(personality.Value.ToString().SetColor(meetRequirements ? "brightblue" : "brightred").ColorReplace(), true);
				}
				else
				{
					bool flag7 = index < combatSkillAttainmentsStartIndex;
					if (flag7)
					{
						AdventureCostResource lifeSkillAttainments = node.Requirements.LifeSkillAttainments[index - lifeSkillAttainmentsStartIndex];
						icon.SetSprite("ui9_back_attainments_life_0_" + lifeSkillAttainments.Type.ToString(), false, null);
						requirementName.SetText(LocalStringManager.Get(UI_CharacterMenuTeam.LifeSkillNameKeys[lifeSkillAttainments.Type]), true);
						bool flag8 = this._lifeSkillAttainments[lifeSkillAttainments.Type] < lifeSkillAttainments.Value;
						if (flag8)
						{
							meetRequirements = false;
						}
						count.SetText(lifeSkillAttainments.Value.ToString().SetColor(meetRequirements ? "brightblue" : "brightred").ColorReplace(), true);
					}
					else
					{
						bool flag9 = index >= combatSkillAttainmentsStartIndex;
						if (flag9)
						{
							AdventureCostResource combatSkillAttainments = node.Requirements.CombatSkillAttainments[index - combatSkillAttainmentsStartIndex];
							icon.SetSprite("ui9_back_attainments_life_0_" + combatSkillAttainments.Type.ToString(), false, null);
							requirementName.SetText(LocalStringManager.Get(UI_CharacterMenuTeam.CombatSkillNameKeys[combatSkillAttainments.Type]), true);
							bool flag10 = this._combatSkillAttainments[combatSkillAttainments.Type] < combatSkillAttainments.Value;
							if (flag10)
							{
								meetRequirements = false;
							}
							count.SetText(combatSkillAttainments.Value.ToString().SetColor(meetRequirements ? "brightblue" : "brightred").ColorReplace(), true);
						}
					}
				}
			});
			result = meetRequirements;
		}
		return result;
	}

	// Token: 0x060013D2 RID: 5074 RVA: 0x0007B5FC File Offset: 0x000797FC
	private void RefreshAllLineState()
	{
		bool flag = this.MajorEventRuntime.CurrentMain != this.MajorEventRuntime.Current;
		if (!flag)
		{
			int currentMain = this.MajorEventRuntime.CurrentMain;
			AdventureMajorEventNodeData node = this.MajorEventData.Nodes[currentMain];
			for (int i = 0; i < node.NextNodes.Count; i++)
			{
				int nextNodeIndex = node.NextNodes[i];
				AdventureMajorEventNodeData nextNode = this.MajorEventData.Nodes[nextNodeIndex];
				Refers lineRefers = this.GetLineRefer(currentMain, nextNodeIndex);
				CImage lineSprite = lineRefers.CGet<CImage>("Icon");
				bool flag2 = lineSprite.type == Image.Type.Simple;
				if (!flag2)
				{
					Refers nextNodeRefer = this._nodeRefersDict[nextNodeIndex];
					bool flag3 = this.MajorEventRuntime.UnlockedNodes.Contains(nextNodeIndex) && this.RefreshNodeRequirements(nextNode, nextNodeIndex, nextNodeRefer);
					sbyte passType;
					if (flag3)
					{
						passType = 1;
					}
					else
					{
						passType = 2;
					}
					lineSprite.DOFade(0f, this._lineFadeDuration).OnComplete(delegate
					{
						lineSprite.SetSprite(AdventureMajorEventTool.GetLineSpriteName(passType, 0), false, null);
						lineSprite.DOFade(1f, this._lineFadeDuration);
					});
				}
			}
			for (int j = 0; j < this.MajorEventRuntime.VisitedNodes.Count; j++)
			{
				int nodeIndex = this.MajorEventRuntime.VisitedNodes[j];
				bool flag4 = nodeIndex == this.MajorEventRuntime.CurrentMain;
				if (!flag4)
				{
					AdventureMajorEventNodeData visitedNode = this.MajorEventData.Nodes[nodeIndex];
					for (int k = 0; k < visitedNode.NextNodes.Count; k++)
					{
						int nextNodeIndex2 = visitedNode.NextNodes[k];
						Refers lineRefers2 = this.GetLineRefer(nodeIndex, nextNodeIndex2);
						CImage lineSprite = lineRefers2.CGet<CImage>("Icon");
						bool flag5 = lineSprite.sprite.name.Contains(AdventureMajorEventTool.GetLineSpriteName(1, 0));
						if (flag5)
						{
							lineSprite.DOFade(0f, this._lineFadeDuration).OnComplete(delegate
							{
								lineSprite.SetSprite(AdventureMajorEventTool.GetLineSpriteName(0, 0), false, null);
								lineSprite.DOFade(1f, this._lineFadeDuration);
							});
						}
						Refers refer = this._nodeRefersDict[nextNodeIndex2];
						CImage nodeIcon = refer.CGet<CImage>("Icon");
						bool flag6 = nodeIcon.sprite.name.Contains(this.GetNodeImage(nextNodeIndex2, 1));
						if (flag6)
						{
							nodeIcon.SetSprite(this.GetNodeImage(nextNodeIndex2, 0), false, null);
						}
					}
				}
			}
		}
	}

	// Token: 0x060013D3 RID: 5075 RVA: 0x0007B8C5 File Offset: 0x00079AC5
	private IEnumerator RefreshPassedLine(int endNodeIndex, float delayTime)
	{
		UI_AdventureMajorEvent.<>c__DisplayClass84_0 CS$<>8__locals1 = new UI_AdventureMajorEvent.<>c__DisplayClass84_0();
		CS$<>8__locals1.<>4__this = this;
		yield return new WaitForSeconds(delayTime);
		int nodeIndex = this.MajorEventRuntime.CurrentMain;
		CS$<>8__locals1.node = this.MajorEventData.Nodes[nodeIndex];
		CS$<>8__locals1.nextNode = this.MajorEventData.Nodes[endNodeIndex];
		Refers lineRefers = this.GetLineRefer(nodeIndex, endNodeIndex);
		bool flag = lineRefers != null;
		if (flag)
		{
			UI_AdventureMajorEvent.<>c__DisplayClass84_1 CS$<>8__locals2 = new UI_AdventureMajorEvent.<>c__DisplayClass84_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			CS$<>8__locals2.lineSprite = lineRefers.CGet<CImage>("Icon");
			CS$<>8__locals2.lineSprite.DOFade(0f, this._lineFadeDuration).OnComplete(delegate
			{
				CS$<>8__locals2.lineSprite.SetSprite(AdventureMajorEventTool.GetLineSpriteName(3, AdventureMajorEventTool.GetPassedLineStyle(CS$<>8__locals2.CS$<>8__locals1.node.Type, CS$<>8__locals2.CS$<>8__locals1.nextNode.Type)), false, null);
				CS$<>8__locals2.lineSprite.type = Image.Type.Simple;
				CS$<>8__locals2.lineSprite.SetNativeSize();
				CS$<>8__locals2.lineSprite.DOFade(1f, CS$<>8__locals2.CS$<>8__locals1.<>4__this._lineFadeDuration);
			});
			CS$<>8__locals2 = null;
		}
		yield break;
	}

	// Token: 0x060013D4 RID: 5076 RVA: 0x0007B8E4 File Offset: 0x00079AE4
	private Refers GetLineRefer(int startIndex, int endIndex)
	{
		for (int i = 0; i < this._lineRefersList.Count; i++)
		{
			ValueTuple<int, int, Refers> lineData = this._lineRefersList[i];
			bool flag = lineData.Item1 == startIndex && lineData.Item2 == endIndex;
			if (flag)
			{
				return lineData.Item3;
			}
		}
		return null;
	}

	// Token: 0x060013D5 RID: 5077 RVA: 0x0007B944 File Offset: 0x00079B44
	private void AdventureRemakeFinish()
	{
		CommandKitBase.SetDisable(true);
		bool skipCompleteAnim = this._skipCompleteAnim;
		if (skipCompleteAnim)
		{
			this.ExitAdventure();
		}
		else
		{
			this.adventureRemakeFinish.Show(new Action(this.ExitAdventure), true);
		}
	}

	// Token: 0x060013D6 RID: 5078 RVA: 0x0007B985 File Offset: 0x00079B85
	private void ExitAdventure()
	{
		CommandKitBase.SetDisable(false);
		UIManager.Instance.StackBack(null);
		TaiwuEventDomainMethod.Call.TriggerListener("ExitMajorEvent", true);
		GEvent.OnEvent(UiEvents.AdventureRemakeExit, null);
		this.SetTimeScale(1f);
	}

	// Token: 0x060013D7 RID: 5079 RVA: 0x0007B9C4 File Offset: 0x00079BC4
	private void MajorEventSkipCompleteAnim(ArgumentBox argumentBox)
	{
		this._skipCompleteAnim = true;
	}

	// Token: 0x17000235 RID: 565
	// (get) Token: 0x060013D8 RID: 5080 RVA: 0x0007B9D0 File Offset: 0x00079BD0
	private bool TaiwuIsKid
	{
		get
		{
			CharacterDisplayData taiwuDisplayData = this._taiwuDisplayData;
			short? num = (taiwuDisplayData != null) ? new short?(taiwuDisplayData.AvatarRelatedData.DisplayAge) : null;
			int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
			int num3 = 16;
			return num2.GetValueOrDefault() < num3 & num2 != null;
		}
	}

	// Token: 0x17000236 RID: 566
	// (get) Token: 0x060013D9 RID: 5081 RVA: 0x0007BA39 File Offset: 0x00079C39
	private WorldMapModel MapModel
	{
		get
		{
			return SingletonObject.getInstance<WorldMapModel>();
		}
	}

	// Token: 0x17000237 RID: 567
	// (get) Token: 0x060013DA RID: 5082 RVA: 0x0007BA40 File Offset: 0x00079C40
	private TravelSkeletonItem TravelSkeleton
	{
		get
		{
			return ViewPartWorldMap.GetSkeleton(this.MapModel.TaiwuCarrier);
		}
	}

	// Token: 0x060013DB RID: 5083 RVA: 0x0007BA52 File Offset: 0x00079C52
	private void PlayCarrierAnim(bool isMove)
	{
		this.PlayCarrierAnimCharacter(isMove);
		this.PlayCarrierAnimCarrier(isMove);
		base.CGet<RotateHelper>("BottomCircle").enabled = isMove;
	}

	// Token: 0x060013DC RID: 5084 RVA: 0x0007BA78 File Offset: 0x00079C78
	private void PlayCarrierAnimCharacter(bool isMove)
	{
		SkeletonGraphic charSkeleton = base.CGet<SkeletonGraphic>("CharacterSkeleton");
		charSkeleton.transform.parent.localScale = Vector3.one * this.TravelSkeleton.Size;
		this.PlayCarrierAnimCharacterEquipments(charSkeleton, SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
		string animName = isMove ? this.TravelSkeleton.Animation : this.TravelSkeleton.AnimationIdle;
		this.PlayCarrierAnimCharacterAnimation(charSkeleton, animName);
	}

	// Token: 0x060013DD RID: 5085 RVA: 0x0007BAF0 File Offset: 0x00079CF0
	private void PlayCarrierAnimCharacterAnimation(SkeletonGraphic charSkeleton, string animName)
	{
		string animPath = "RemakeResources/SpineAnimations/Character/TravelAnimations/" + animName;
		ResLoader.Load<RawAnimationAsset>(animPath, delegate(RawAnimationAsset rawAnimation)
		{
			Spine.Animation anim = rawAnimation.GetAnimation(charSkeleton.skeletonDataAsset);
			charSkeleton.AnimationState.SetAnimation(0, anim, true);
			charSkeleton.gameObject.SetActive(true);
		}, null, false);
	}

	// Token: 0x060013DE RID: 5086 RVA: 0x0007BB2C File Offset: 0x00079D2C
	private void PlayCarrierAnimCharacterEquipments(SkeletonGraphic skeleton, int charId)
	{
		bool flag = this._taiwuDisplayData == null;
		if (flag)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this, charId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._taiwuDisplayData);
			});
		}
		CharacterDomainMethod.AsyncCall.GetAllEquipmentItems(this, charId, delegate(int offset, RawDataPool pool)
		{
			List<ItemDisplayData> equipments = EasyPool.Get<List<ItemDisplayData>>();
			Serializer.Deserialize(pool, offset, ref equipments);
			CombatAnimationUtils.UpdateSkeleton(skeleton, this._taiwuDisplayData, equipments);
			EasyPool.Free<List<ItemDisplayData>>(equipments);
		});
	}

	// Token: 0x060013DF RID: 5087 RVA: 0x0007BB84 File Offset: 0x00079D84
	private void PlayCarrierAnimCarrier(bool isMove)
	{
		SkeletonGraphic carrierSkeleton = base.CGet<SkeletonGraphic>("CarrierSkeleton");
		carrierSkeleton.gameObject.SetActive(this.TravelSkeleton.AnyCarrier);
		bool flag = !this.TravelSkeleton.AnyCarrier;
		if (!flag)
		{
			string carrierAnim = isMove ? this.TravelSkeleton.CarrierAnimation : this.TravelSkeleton.CarrierAnimationIdle;
			string carrierAnimPath = "RemakeResources/SpineAnimations/Carrier/" + this.TravelSkeleton.CarrierAnimationPath;
			ResLoader.Load<SkeletonDataAsset>(carrierAnimPath, delegate(SkeletonDataAsset animData)
			{
				carrierSkeleton.skeletonDataAsset = animData;
				carrierSkeleton.initialSkinName = this.TravelSkeleton.CarrierAnimationSkin;
				carrierSkeleton.Initialize(true);
				carrierSkeleton.transform.localScale = Vector3.one * (this.TaiwuIsKid ? 0.85f : 1f);
				carrierSkeleton.AnimationState.SetAnimation(0, carrierAnim, true);
				carrierSkeleton.gameObject.SetActive(true);
			}, null, false);
		}
	}

	// Token: 0x060013E0 RID: 5088 RVA: 0x0007BC2C File Offset: 0x00079E2C
	private void PlayCarrierSound()
	{
		string travelSound = this.TravelSkeleton.Sound;
		bool flag = !string.IsNullOrEmpty(travelSound);
		if (flag)
		{
			AudioManager.Instance.PlaySound(travelSound, false, false);
		}
	}

	// Token: 0x060013E1 RID: 5089 RVA: 0x0007BC61 File Offset: 0x00079E61
	private void Update()
	{
	}

	// Token: 0x060013E2 RID: 5090 RVA: 0x0007BC64 File Offset: 0x00079E64
	public bool IsCurrentNodeEnd()
	{
		bool notInAdventure = this.MajorEventRuntime.NotInAdventure;
		return !notInAdventure && this.IsEndNode(this.MajorEventRuntime.Current);
	}

	// Token: 0x060013E3 RID: 5091 RVA: 0x0007BC9C File Offset: 0x00079E9C
	private bool IsEndNode(int nodeIndex)
	{
		bool flag = nodeIndex < 0 || nodeIndex >= this.MajorEventData.Nodes.Count;
		return !flag && this.MajorEventData.Nodes[nodeIndex].Type == EAdventureMajorEventNodeType.End;
	}

	// Token: 0x060013E4 RID: 5092 RVA: 0x0007BCEC File Offset: 0x00079EEC
	private void OnCharacterTaiwuCarrierChanged(ArgumentBox argBox)
	{
		this.PlayCarrierAnim(false);
	}

	// Token: 0x04001098 RID: 4248
	[SerializeField]
	public Refers[] nodePrefab;

	// Token: 0x04001099 RID: 4249
	[SerializeField]
	private AdventureRemakeFinish adventureRemakeFinish;

	// Token: 0x0400109A RID: 4250
	private const string LinePrefabKey = "UI_AdventureMajorEventLinePrefab";

	// Token: 0x0400109B RID: 4251
	private const string DecorationPrefabKey = "UI_AdventureMajorEventDecorationPrefab";

	// Token: 0x0400109C RID: 4252
	private readonly string[] _nodePrefabKeys = new string[]
	{
		"UI_AdventureMajorEventStartNodePrefab",
		"UI_AdventureMajorEventTureNodePrefab",
		"UI_AdventureMajorEventEndNodePrefab",
		"UI_AdventureMajorEventCheckNodePrefab",
		"UI_AdventureMajorEventExtraNodePrefab"
	};

	// Token: 0x0400109D RID: 4253
	private bool _isInit;

	// Token: 0x0400109E RID: 4254
	private int _majorEventId;

	// Token: 0x0400109F RID: 4255
	private readonly Dictionary<int, Refers> _nodeRefersDict = new Dictionary<int, Refers>();

	// Token: 0x040010A0 RID: 4256
	[TupleElementNames(new string[]
	{
		"startIndex",
		"endIndex"
	})]
	private readonly List<ValueTuple<int, int>> _lineDataList = new List<ValueTuple<int, int>>();

	// Token: 0x040010A1 RID: 4257
	[TupleElementNames(new string[]
	{
		"startIndex",
		"endIndex",
		"lineRefer"
	})]
	private readonly List<ValueTuple<int, int, Refers>> _lineRefersList = new List<ValueTuple<int, int, Refers>>();

	// Token: 0x040010A2 RID: 4258
	[SerializeField]
	private AdventureMajorEventAtmosphereSwitcher atmosphereSwitcher;

	// Token: 0x040010A3 RID: 4259
	private const float EventEndedAtmosphereSwitchDelay = 0.5f;

	// Token: 0x040010A4 RID: 4260
	private Coroutine _eventEndedAtmosphereSwitchCoroutine;

	// Token: 0x040010A5 RID: 4261
	private DG.Tweening.Sequence _animSequence;

	// Token: 0x040010A6 RID: 4262
	[SerializeField]
	private float bgFadeTime;

	// Token: 0x040010A7 RID: 4263
	[SerializeField]
	private float carrierFadeTime;

	// Token: 0x040010A8 RID: 4264
	[SerializeField]
	private float uiFadeTime;

	// Token: 0x040010A9 RID: 4265
	[SerializeField]
	private float startNodeFadeTime;

	// Token: 0x040010AA RID: 4266
	[SerializeField]
	private float otherNodeFadeTime;

	// Token: 0x040010AB RID: 4267
	private const float TotalPosOffsetY = 200f;

	// Token: 0x040010AC RID: 4268
	private const float TaiwuPosOffsetY = 40f;

	// Token: 0x040010AD RID: 4269
	public static readonly float MoveStepTime = 1f;

	// Token: 0x040010AE RID: 4270
	private bool _moving;

	// Token: 0x040010AF RID: 4271
	private int[] _personalities;

	// Token: 0x040010B0 RID: 4272
	private int[] _lifeSkillAttainments;

	// Token: 0x040010B1 RID: 4273
	private int[] _combatSkillAttainments;

	// Token: 0x040010B2 RID: 4274
	private float _lineFadeDuration = 0.4f;

	// Token: 0x040010B3 RID: 4275
	private bool _skipCompleteAnim;

	// Token: 0x040010B4 RID: 4276
	private CharacterDisplayData _taiwuDisplayData;
}
