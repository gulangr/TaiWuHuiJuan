using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CharacterDataMonitor;
using Config;
using FrameWork;
using Game.Components.Avatar;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UICommon.Character.Elements;
using UnityEngine;

// Token: 0x020002F7 RID: 759
public class UI_TravelingTaoistMonkSkill2 : UIBase
{
	// Token: 0x06002C86 RID: 11398 RVA: 0x0015D064 File Offset: 0x0015B264
	public override void OnInit(ArgumentBox argsBox)
	{
		UI_TravelingTaoistMonkSkill2.<>c__DisplayClass26_0 CS$<>8__locals1 = new UI_TravelingTaoistMonkSkill2.<>c__DisplayClass26_0();
		CS$<>8__locals1.<>4__this = this;
		this._bubble = base.CGet<Bubble>("RightCloudBubble");
		this._bubble.Hide();
		this._featureItem = base.CGet<Refers>("FeatureItem");
		this._featureGroup.Clear();
		this._mutexFeatureDic.Clear();
		argsBox.Get("characterId", out this._targetCharId);
		this._taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
		this._onConfirm = null;
		this._afterHealth = base.CGet<CharacterHealthBar>("AfterHealth");
		this._afterHealth.GetHealthString = null;
		this._curHealth = base.CGet<CharacterHealthBar>("CurHealth");
		CS$<>8__locals1.curHealth = new CharacterHealth(this._curHealth);
		CS$<>8__locals1.curHealth.CharacterId = this._taiwuId;
		this._hpMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AgeHealthMonitor>(this._taiwuId, false);
		bool init = this._hpMonitor.Init;
		if (init)
		{
			CS$<>8__locals1.<OnInit>g__OnHealthChange|2();
			this._hpMonitor.AddOnHealthChangeEventListener(new Action(CS$<>8__locals1.<OnInit>g__OnHealthChange|2));
		}
		else
		{
			this._hpMonitor.AddOnHealthChangeEventListener(new Action(CS$<>8__locals1.<OnInit>g__OnHealthChange|2));
		}
		base.CGet<CButtonObsolete>("ButtonClosePopup").ClearAndAddListener(delegate
		{
			UIManager.Instance.HideUI(UIElement.TravelingTaoistMonkSkill2);
		});
		CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
		base.CGet<CButtonObsolete>("BtnExchange").interactable = (monitor.GetTaiwuTeamCharIds().Count > 1);
		base.CGet<CButtonObsolete>("BtnExchange").ClearAndAddListener(delegate
		{
			CharacterMonitorModel monitor2 = SingletonObject.getInstance<CharacterMonitorModel>();
			List<int> charIdList = monitor2.GetTaiwuTeamCharIds();
			List<int> res = charIdList.Union(monitor2.GetTaiwuSpecialGroup()).ToList<int>();
			res.Remove(CS$<>8__locals1.<>4__this._targetCharId);
			ArgumentBox args = EasyPool.Get<ArgumentBox>();
			args.SetObject("charIdList", res);
			args.Set("canSelectSpecialChar", false);
			ArgumentBox argumentBox = args;
			string key = "callback";
			Action<int> arg;
			if ((arg = CS$<>8__locals1.<>9__3) == null)
			{
				arg = (CS$<>8__locals1.<>9__3 = delegate(int charId)
				{
					CS$<>8__locals1.<>4__this._taiwuId = charId;
					CS$<>8__locals1.<>4__this._featureGroup.Clear();
					CS$<>8__locals1.<>4__this.InitRefers(CS$<>8__locals1.<>4__this._leftArea, CS$<>8__locals1.<>4__this._taiwuId, false);
					CS$<>8__locals1.<>4__this.InitRefers(CS$<>8__locals1.<>4__this._rightArea, CS$<>8__locals1.<>4__this._targetCharId, true);
					CS$<>8__locals1.<>4__this.UpdateExchangeBtnState(false);
				});
			}
			argumentBox.SetObject(key, arg);
			UIElement.SelectCharLegacy.SetOnInitArgs(args);
			UIManager.Instance.ShowUI(UIElement.SelectCharLegacy, true);
		});
		this._confirmBtn = base.CGet<CButtonObsolete>("ConfirmBtn");
		this._confirmBtn.ClearAndAddListener(new Action(this.ConfirmExchange));
		this.UpdateExchangeBtnState(false);
		this._leftArea = base.CGet<Refers>("LeftArea");
		this._rightArea = base.CGet<Refers>("RightArea");
		this.InitRefers(this._leftArea, this._taiwuId, false);
		this.InitRefers(this._rightArea, this._targetCharId, true);
		this._helathChangeValue = base.CGet<TextMeshProUGUI>("HealthChangeValue");
		this._helathChangeValue.text = "0.00%";
		UIElement element = this.Element;
		element.OnHide = (Action)Delegate.Combine(element.OnHide, new Action(delegate()
		{
			CS$<>8__locals1.curHealth.CharacterId = -1;
			TaiwuEventDomainMethod.Call.TriggerListener("TravelingTaoistMonkSkill2Executed", true);
		}));
	}

	// Token: 0x06002C87 RID: 11399 RVA: 0x0015D2DC File Offset: 0x0015B4DC
	public override void InitMonitorFieldIds()
	{
		UIBase.MonitorDataField baseMaxHealthUid = new UIBase.MonitorDataField(4, 0, (ulong)((long)this._taiwuId), new uint[]
		{
			20U
		});
		this.MonitorFields.Add(baseMaxHealthUid);
	}

	// Token: 0x06002C88 RID: 11400 RVA: 0x0015D314 File Offset: 0x0015B514
	private void InitRefers(Refers refers, int charId, bool shouldRefreshMutexFeatureDic = false)
	{
		UI_TravelingTaoistMonkSkill2.<>c__DisplayClass28_0 CS$<>8__locals1 = new UI_TravelingTaoistMonkSkill2.<>c__DisplayClass28_0();
		CS$<>8__locals1.charId = charId;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.shouldRefreshMutexFeatureDic = shouldRefreshMutexFeatureDic;
		bool flag = CS$<>8__locals1.charId == -1;
		if (!flag)
		{
			CS$<>8__locals1.avatar = refers.CGet<Game.Components.Avatar.Avatar>("Avatar");
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this, CS$<>8__locals1.charId, delegate(int offset, RawDataPool pool)
			{
				CharacterDisplayData displayData = null;
				Serializer.Deserialize(pool, offset, ref displayData);
				CS$<>8__locals1.avatar.Refresh(displayData.AvatarRelatedData);
			});
			refers.CGet<CButtonObsolete>("ClickCharBtn").ClearAndAddListener(delegate
			{
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("CharacterId", CS$<>8__locals1.charId);
				argBox.Set("CanOperate", false);
				UIElement.CharacterMenu.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			});
			CS$<>8__locals1.featureScroll = refers.CGet<InfinityScrollLegacy>("FeatureScroll");
			CS$<>8__locals1.featureScrollToBeExchange = refers.CGet<InfinityScrollLegacy>("FeatureScrollTobeExchange");
			CS$<>8__locals1.featureScroll.UpdateStyle(this._featureItem, 0);
			CS$<>8__locals1.featureScrollToBeExchange.UpdateStyle(this._featureItem, 0);
			CS$<>8__locals1.featureScrollToBeExchange.OnItemRender = null;
			CS$<>8__locals1.featureScroll.OnItemRender = null;
			CS$<>8__locals1.monitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<FeatureMonitor>(CS$<>8__locals1.charId, false);
			bool flag2 = CS$<>8__locals1.charId == this._taiwuId;
			if (flag2)
			{
				this._taiwuFeaturesLeft.Clear();
				this._taiwuFeaturesTobeExchange.Clear();
				CS$<>8__locals1.featuresLeft = this._taiwuFeaturesLeft;
				CS$<>8__locals1.featuresToBeExchange = this._taiwuFeaturesTobeExchange;
				this._taiwuLeftScroll = CS$<>8__locals1.featureScroll;
				this._taiwuToBeExchangedScroll = CS$<>8__locals1.featureScrollToBeExchange;
			}
			else
			{
				this._targetChrFeaturesLeft.Clear();
				this._targetChrFeaturesTobeExchange.Clear();
				CS$<>8__locals1.featuresLeft = this._targetChrFeaturesLeft;
				CS$<>8__locals1.featuresToBeExchange = this._targetChrFeaturesTobeExchange;
				this._targetLeftScroll = CS$<>8__locals1.featureScroll;
				this._targetToBeExchangedScroll = CS$<>8__locals1.featureScrollToBeExchange;
			}
			InfinityScrollLegacy featureScroll = CS$<>8__locals1.featureScroll;
			featureScroll.OnItemRender = (Action<int, Refers>)Delegate.Combine(featureScroll.OnItemRender, new Action<int, Refers>(delegate(int i, Refers targetRefers)
			{
				short featureId = (short)CS$<>8__locals1.featuresLeft[i];
				FeatureItem item = targetRefers.UserObject as FeatureItem;
				bool flag3 = item == null;
				if (flag3)
				{
					item = new FeatureItem(targetRefers, featureId, CS$<>8__locals1.charId);
					targetRefers.UserObject = item;
				}
				else
				{
					item.Refresh(featureId, CS$<>8__locals1.charId);
				}
				CButtonObsolete button;
				targetRefers.CTryGet<CButtonObsolete>("Btn", out button);
				bool flag4 = button != null;
				if (flag4)
				{
					button.ClearAndAddListener(delegate
					{
						CS$<>8__locals1.<>4__this.OnClickFeature((int)featureId, (CS$<>8__locals1.charId == CS$<>8__locals1.<>4__this._taiwuId) ? UI_TravelingTaoistMonkSkill2.FeatureLocation.TaiwuLeft : UI_TravelingTaoistMonkSkill2.FeatureLocation.TargetLeft);
					});
				}
			}));
			CS$<>8__locals1.featureScroll.SetDataCount(0);
			InfinityScrollLegacy featureScrollToBeExchange = CS$<>8__locals1.featureScrollToBeExchange;
			featureScrollToBeExchange.OnItemRender = (Action<int, Refers>)Delegate.Combine(featureScrollToBeExchange.OnItemRender, new Action<int, Refers>(delegate(int i, Refers targetRefers)
			{
				short featureId = (short)CS$<>8__locals1.featuresToBeExchange[i];
				FeatureItem item = targetRefers.UserObject as FeatureItem;
				bool flag3 = item == null;
				if (flag3)
				{
					item = new FeatureItem(targetRefers, featureId, CS$<>8__locals1.charId);
					targetRefers.UserObject = item;
				}
				else
				{
					item.Refresh(featureId, CS$<>8__locals1.charId);
				}
				CButtonObsolete button;
				bool flag4 = targetRefers.CTryGet<CButtonObsolete>("Btn", out button);
				if (flag4)
				{
					button.ClearAndAddListener(delegate
					{
						CS$<>8__locals1.<>4__this.OnClickFeature((int)featureId, (CS$<>8__locals1.charId == CS$<>8__locals1.<>4__this._taiwuId) ? UI_TravelingTaoistMonkSkill2.FeatureLocation.TaiwuTobeExchanged : UI_TravelingTaoistMonkSkill2.FeatureLocation.TargetTobeExchanged);
					});
				}
			}));
			CS$<>8__locals1.featureScrollToBeExchange.SetDataCount(0);
			bool init = CS$<>8__locals1.monitor.Init;
			if (init)
			{
				CS$<>8__locals1.<InitRefers>g__RefreshFeature|2();
				CS$<>8__locals1.monitor.AddFeatureListener(new Action(CS$<>8__locals1.<InitRefers>g__RefreshFeature|2));
			}
			else
			{
				CS$<>8__locals1.monitor.AddFeatureListener(new Action(CS$<>8__locals1.<InitRefers>g__RefreshFeature|2));
			}
			CS$<>8__locals1.characterName = new CharacterName(refers.CGet<TextMeshProUGUI>("Name"), null, null);
			CS$<>8__locals1.characterName.CharacterId = CS$<>8__locals1.charId;
			CS$<>8__locals1.mouseTipDisplayer = refers.CGet<TooltipInvoker>("MouseTipDisplayer");
			CS$<>8__locals1.mouseTipDisplayer.RuntimeParam = EasyPool.Get<ArgumentBox>();
			CS$<>8__locals1.mouseTipDisplayer.RuntimeParam.Set("CharId", CS$<>8__locals1.charId);
			CS$<>8__locals1.mouseTipDisplayer.enabled = true;
			UIElement element = this.Element;
			element.OnHide = (Action)Delegate.Combine(element.OnHide, new Action(delegate()
			{
				CS$<>8__locals1.mouseTipDisplayer.RuntimeParam = null;
				CS$<>8__locals1.characterName.CharacterId = -1;
				CS$<>8__locals1.monitor.RemoveFeatureListener(new Action(base.<InitRefers>g__RefreshFeature|2));
			}));
			this._onConfirm = (Action)Delegate.Combine(this._onConfirm, new Action(delegate()
			{
				CS$<>8__locals1.featureScroll.UpdateStyle(CS$<>8__locals1.<>4__this.CGet<Refers>("FeatureItemCannotInteractable"), 0);
				CS$<>8__locals1.featureScrollToBeExchange.UpdateData(0);
			}));
		}
	}

	// Token: 0x06002C89 RID: 11401 RVA: 0x0015D628 File Offset: 0x0015B828
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b == 0)
			{
				DataUid baseMaxHealthUid = new DataUid(4, 0, (ulong)((long)this._taiwuId), 20U);
				bool flag = notification.Uid.Equals(baseMaxHealthUid);
				if (flag)
				{
					short health = -1;
					Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref health);
					this._maxHealth = (int)health;
				}
			}
		}
	}

	// Token: 0x06002C8A RID: 11402 RVA: 0x0015D6D8 File Offset: 0x0015B8D8
	private void UpdateExchangeBtnState(bool confirmed = false)
	{
		TooltipInvoker disPlayer = this._confirmBtn.GetComponent<TooltipInvoker>();
		disPlayer.NeedRefresh = true;
		TooltipInvoker tooltipInvoker = disPlayer;
		if (tooltipInvoker.RuntimeParam == null)
		{
			tooltipInvoker.RuntimeParam = new ArgumentBox();
		}
		disPlayer.enabled = true;
		if (confirmed)
		{
			this._confirmBtn.interactable = false;
			disPlayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_ProfessionTravelingTaoistMonkSkill2Text7));
		}
		else
		{
			bool flag = this._taiwuFeaturesTobeExchange.Count != this._targetChrFeaturesTobeExchange.Count;
			if (flag)
			{
				this._confirmBtn.interactable = false;
				disPlayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_ProfessionTravelingTaoistMonkSkill2Text3));
			}
			else
			{
				bool flag2 = this._taiwuFeaturesTobeExchange.Count == 0 || this._targetChrFeaturesTobeExchange.Count == 0;
				if (flag2)
				{
					this._confirmBtn.interactable = false;
					disPlayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_ProfessionTravelingTaoistMonkSkill2Text11));
				}
				else
				{
					bool flag3 = this.GetMaxHpCost() >= this._maxHealth && this._maxHealth != -1;
					if (flag3)
					{
						this._confirmBtn.interactable = false;
						disPlayer.RuntimeParam.Set("arg0", LocalStringManager.Get(LanguageKey.LK_ProfessionTravelingTaoistMonkSkill2Text10));
					}
					else
					{
						this._confirmBtn.interactable = true;
						disPlayer.enabled = false;
					}
				}
			}
		}
	}

	// Token: 0x06002C8B RID: 11403 RVA: 0x0015D854 File Offset: 0x0015BA54
	private void OnClickFeature(int featureId, UI_TravelingTaoistMonkSkill2.FeatureLocation featureLocation)
	{
		UI_TravelingTaoistMonkSkill2.<>c__DisplayClass31_0 CS$<>8__locals1;
		CS$<>8__locals1.featureLocation = featureLocation;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.featureId = featureId;
		switch (CS$<>8__locals1.featureLocation)
		{
		case UI_TravelingTaoistMonkSkill2.FeatureLocation.TaiwuLeft:
		{
			bool flag = this._mutexFeatureDic.ContainsKey(CS$<>8__locals1.featureId);
			if (flag)
			{
				this.<OnClickFeature>g__SwapDoubleLeftToExchange|31_0(ref CS$<>8__locals1);
			}
			else
			{
				this.<OnClickFeature>g__SwapSingle|31_2(this._taiwuFeaturesLeft, this._taiwuFeaturesTobeExchange, this._taiwuLeftScroll, this._taiwuToBeExchangedScroll, CS$<>8__locals1.featureId, ref CS$<>8__locals1);
			}
			break;
		}
		case UI_TravelingTaoistMonkSkill2.FeatureLocation.TaiwuTobeExchanged:
		{
			int targetFeatureId;
			bool flag2 = this._mutexFeatureDic.TryGetValue(CS$<>8__locals1.featureId, out targetFeatureId);
			if (flag2)
			{
				this.<OnClickFeature>g__SwapDouble|31_1(false, CS$<>8__locals1.featureId, targetFeatureId, ref CS$<>8__locals1);
			}
			else
			{
				this.<OnClickFeature>g__SwapSingle|31_2(this._taiwuFeaturesTobeExchange, this._taiwuFeaturesLeft, this._taiwuToBeExchangedScroll, this._taiwuLeftScroll, CS$<>8__locals1.featureId, ref CS$<>8__locals1);
			}
			break;
		}
		case UI_TravelingTaoistMonkSkill2.FeatureLocation.TargetLeft:
		{
			bool flag3 = this._mutexFeatureDic.ContainsKey(CS$<>8__locals1.featureId);
			if (flag3)
			{
				this.<OnClickFeature>g__SwapDoubleLeftToExchange|31_0(ref CS$<>8__locals1);
			}
			else
			{
				this.<OnClickFeature>g__SwapSingle|31_2(this._targetChrFeaturesLeft, this._targetChrFeaturesTobeExchange, this._targetLeftScroll, this._targetToBeExchangedScroll, CS$<>8__locals1.featureId, ref CS$<>8__locals1);
			}
			break;
		}
		case UI_TravelingTaoistMonkSkill2.FeatureLocation.TargetTobeExchanged:
		{
			int taiwuFeatureId;
			bool flag4 = this._mutexFeatureDic.TryGetValue(CS$<>8__locals1.featureId, out taiwuFeatureId);
			if (flag4)
			{
				this.<OnClickFeature>g__SwapDouble|31_1(false, taiwuFeatureId, CS$<>8__locals1.featureId, ref CS$<>8__locals1);
			}
			else
			{
				this.<OnClickFeature>g__SwapSingle|31_2(this._targetChrFeaturesTobeExchange, this._targetChrFeaturesLeft, this._targetToBeExchangedScroll, this._targetLeftScroll, CS$<>8__locals1.featureId, ref CS$<>8__locals1);
			}
			break;
		}
		}
	}

	// Token: 0x06002C8C RID: 11404 RVA: 0x0015D9FC File Offset: 0x0015BBFC
	private int GetMaxHpCost()
	{
		int levelSum = 0;
		foreach (int id in this._targetChrFeaturesTobeExchange)
		{
			levelSum += Mathf.Abs((int)CharacterFeature.Instance[id].Level);
		}
		int curSeniority = SingletonObject.getInstance<ProfessionModel>().GetProfessionData(14).Seniority;
		int maxSeniority = 3000000;
		return levelSum * (9 - 6 * curSeniority / maxSeniority);
	}

	// Token: 0x06002C8D RID: 11405 RVA: 0x0015DA94 File Offset: 0x0015BC94
	private void ConfirmExchange()
	{
		Debug.Log("ConfirmExchange");
		ProfessionSkillArg professionSkillArg2 = new ProfessionSkillArg();
		professionSkillArg2.ProfessionId = 14;
		professionSkillArg2.SkillId = 58;
		professionSkillArg2.CharId = this._taiwuId;
		professionSkillArg2.CharIds = new List<int>
		{
			this._targetCharId
		};
		professionSkillArg2.BookIds = new List<int>(this._taiwuFeaturesTobeExchange);
		professionSkillArg2.EffectBlocks = new List<short>(from x in this._targetChrFeaturesTobeExchange
		select (short)x);
		professionSkillArg2.IsSuccess = true;
		ProfessionSkillArg professionSkillArg = professionSkillArg2;
		ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
		argsBox.SetObject("ProfessionSkillArg", professionSkillArg);
		argsBox.SetObject("OnConfirm", new Action(this.<ConfirmExchange>g__OnProfessionSkillConfirm|33_0));
		UIElement.ProfessionSkillConfirm.SetOnInitArgs(argsBox);
		UIManager.Instance.MaskUI(UIElement.ProfessionSkillConfirm);
	}

	// Token: 0x06002C8E RID: 11406 RVA: 0x0015DB7C File Offset: 0x0015BD7C
	private void BubbleStartCoroutine(string content)
	{
		this._bubble.SetText(content, true);
		bool flag = this._coroutine != null;
		if (flag)
		{
			base.StopCoroutine(this._coroutine);
		}
		this._coroutine = base.StartCoroutine(this.Wait(this._bubble));
	}

	// Token: 0x06002C8F RID: 11407 RVA: 0x0015DBCC File Offset: 0x0015BDCC
	private IEnumerator Wait(Bubble bubble)
	{
		bubble.Show();
		yield return new WaitForSeconds(10f);
		bubble.Hide();
		yield break;
	}

	// Token: 0x06002C91 RID: 11409 RVA: 0x0015DC50 File Offset: 0x0015BE50
	[CompilerGenerated]
	private void <OnClickFeature>g__SwapDoubleLeftToExchange|31_0(ref UI_TravelingTaoistMonkSkill2.<>c__DisplayClass31_0 A_1)
	{
		bool flag = A_1.featureLocation == UI_TravelingTaoistMonkSkill2.FeatureLocation.TaiwuLeft;
		if (flag)
		{
			int targetCharId = this._mutexFeatureDic[A_1.featureId];
			this.<OnClickFeature>g__SwapDouble|31_1(true, A_1.featureId, targetCharId, ref A_1);
		}
		else
		{
			bool flag2 = A_1.featureLocation == UI_TravelingTaoistMonkSkill2.FeatureLocation.TargetLeft;
			if (flag2)
			{
				int taiwuCharId = this._mutexFeatureDic[A_1.featureId];
				this.<OnClickFeature>g__SwapDouble|31_1(true, taiwuCharId, A_1.featureId, ref A_1);
			}
		}
	}

	// Token: 0x06002C92 RID: 11410 RVA: 0x0015DCC4 File Offset: 0x0015BEC4
	[CompilerGenerated]
	private void <OnClickFeature>g__SwapDouble|31_1(bool leftToExchange, int taiwuFeatureId, int targetCharFeatureId, ref UI_TravelingTaoistMonkSkill2.<>c__DisplayClass31_0 A_4)
	{
		if (leftToExchange)
		{
			this.<OnClickFeature>g__SwapSingle|31_2(this._taiwuFeaturesLeft, this._taiwuFeaturesTobeExchange, this._taiwuLeftScroll, this._taiwuToBeExchangedScroll, taiwuFeatureId, ref A_4);
			this.<OnClickFeature>g__SwapSingle|31_2(this._targetChrFeaturesLeft, this._targetChrFeaturesTobeExchange, this._targetLeftScroll, this._targetToBeExchangedScroll, targetCharFeatureId, ref A_4);
		}
		else
		{
			this.<OnClickFeature>g__SwapSingle|31_2(this._taiwuFeaturesTobeExchange, this._taiwuFeaturesLeft, this._taiwuToBeExchangedScroll, this._taiwuLeftScroll, taiwuFeatureId, ref A_4);
			this.<OnClickFeature>g__SwapSingle|31_2(this._targetChrFeaturesTobeExchange, this._targetChrFeaturesLeft, this._targetToBeExchangedScroll, this._targetLeftScroll, targetCharFeatureId, ref A_4);
		}
	}

	// Token: 0x06002C93 RID: 11411 RVA: 0x0015DD68 File Offset: 0x0015BF68
	[CompilerGenerated]
	private void <OnClickFeature>g__SwapSingle|31_2(List<int> from, List<int> to, InfinityScrollLegacy fromScroll, InfinityScrollLegacy toScroll, int id, ref UI_TravelingTaoistMonkSkill2.<>c__DisplayClass31_0 A_6)
	{
		from.Remove(id);
		to.Add(id);
		fromScroll.UpdateData(from.Count);
		toScroll.UpdateData(to.Count);
		this.UpdateExchangeBtnState(false);
		int cost = this.GetMaxHpCost();
		this._helathChangeValue.text = string.Format("{0:f2}%", (float)cost / (float)this._maxHealth * 100f);
		short leftHp = (short)((int)this._hpMonitor.LeftMaxHealth - cost);
		short hp = this._hpMonitor.Health;
		bool flag = hp > leftHp;
		if (flag)
		{
			hp = leftHp;
		}
		this._afterHealth.Refresh(hp, leftHp, this._hpMonitor.HealthRecovery, this._taiwuId);
	}

	// Token: 0x06002C94 RID: 11412 RVA: 0x0015DE24 File Offset: 0x0015C024
	[CompilerGenerated]
	private void <ConfirmExchange>g__OnProfessionSkillConfirm|33_0()
	{
		int taiwuBadFeature = 0;
		foreach (int featureId in this._taiwuFeaturesTobeExchange)
		{
			CharacterFeatureItem config = CharacterFeature.Instance[featureId];
			bool flag = config.Type == ECharacterFeatureType.Bad;
			if (flag)
			{
				taiwuBadFeature++;
			}
		}
		int targetBadFeature = 0;
		foreach (int featureId2 in this._targetChrFeaturesTobeExchange)
		{
			CharacterFeatureItem config2 = CharacterFeature.Instance[featureId2];
			bool flag2 = config2.Type == ECharacterFeatureType.Bad;
			if (flag2)
			{
				targetBadFeature++;
			}
		}
		this._maxHealth -= this.GetMaxHpCost();
		this._taiwuFeaturesLeft.Clear();
		this._targetChrFeaturesLeft.Clear();
		this._taiwuFeaturesTobeExchange.Clear();
		this._targetChrFeaturesTobeExchange.Clear();
		this.UpdateExchangeBtnState(true);
		this._helathChangeValue.text = "0.00%";
		Action onConfirm = this._onConfirm;
		if (onConfirm != null)
		{
			onConfirm();
		}
		UIManager.Instance.HideUI(UIElement.TravelingTaoistMonkSkill2);
	}

	// Token: 0x0400202C RID: 8236
	private int _targetCharId = -1;

	// Token: 0x0400202D RID: 8237
	private int _taiwuId = -1;

	// Token: 0x0400202E RID: 8238
	private CButtonObsolete _buttonClosePopup;

	// Token: 0x0400202F RID: 8239
	private Refers _leftArea;

	// Token: 0x04002030 RID: 8240
	private CButtonObsolete _confirmBtn;

	// Token: 0x04002031 RID: 8241
	private CharacterHealthBar _curHealth;

	// Token: 0x04002032 RID: 8242
	private CharacterHealthBar _afterHealth;

	// Token: 0x04002033 RID: 8243
	private TextMeshProUGUI _helathChangeValue;

	// Token: 0x04002034 RID: 8244
	private Refers _rightArea;

	// Token: 0x04002035 RID: 8245
	private int _maxHealth = -1;

	// Token: 0x04002036 RID: 8246
	private AgeHealthMonitor _hpMonitor;

	// Token: 0x04002037 RID: 8247
	private readonly List<int> _taiwuFeaturesLeft = new List<int>();

	// Token: 0x04002038 RID: 8248
	private readonly List<int> _taiwuFeaturesTobeExchange = new List<int>();

	// Token: 0x04002039 RID: 8249
	private readonly List<int> _targetChrFeaturesLeft = new List<int>();

	// Token: 0x0400203A RID: 8250
	private readonly List<int> _targetChrFeaturesTobeExchange = new List<int>();

	// Token: 0x0400203B RID: 8251
	private InfinityScrollLegacy _taiwuLeftScroll;

	// Token: 0x0400203C RID: 8252
	private InfinityScrollLegacy _taiwuToBeExchangedScroll;

	// Token: 0x0400203D RID: 8253
	private InfinityScrollLegacy _targetLeftScroll;

	// Token: 0x0400203E RID: 8254
	private InfinityScrollLegacy _targetToBeExchangedScroll;

	// Token: 0x0400203F RID: 8255
	private readonly Dictionary<int, List<int>> _featureGroup = new Dictionary<int, List<int>>();

	// Token: 0x04002040 RID: 8256
	private readonly Dictionary<int, int> _mutexFeatureDic = new Dictionary<int, int>();

	// Token: 0x04002041 RID: 8257
	private Refers _featureItem;

	// Token: 0x04002042 RID: 8258
	private Action _onConfirm;

	// Token: 0x04002043 RID: 8259
	private Bubble _bubble;

	// Token: 0x04002044 RID: 8260
	private Coroutine _coroutine;

	// Token: 0x0200165A RID: 5722
	private enum FeatureLocation
	{
		// Token: 0x0400A7A9 RID: 42921
		TaiwuLeft,
		// Token: 0x0400A7AA RID: 42922
		TaiwuTobeExchanged,
		// Token: 0x0400A7AB RID: 42923
		TargetLeft,
		// Token: 0x0400A7AC RID: 42924
		TargetTobeExchanged
	}
}
