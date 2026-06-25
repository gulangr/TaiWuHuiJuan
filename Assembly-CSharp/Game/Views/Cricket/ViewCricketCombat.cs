using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using Game.Views.Cricket.Combat;
using GameData.Combat.Cricket;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.Cricket
{
	// Token: 0x02000ABC RID: 2748
	public class ViewCricketCombat : UIBase, ICricketCombatHandler
	{
		// Token: 0x17000ED8 RID: 3800
		// (get) Token: 0x0600871A RID: 34586 RVA: 0x003ED823 File Offset: 0x003EBA23
		private GlobalSettings SettingData
		{
			get
			{
				return SingletonObject.getInstance<GlobalSettings>();
			}
		}

		// Token: 0x17000ED9 RID: 3801
		// (get) Token: 0x0600871B RID: 34587 RVA: 0x003ED82A File Offset: 0x003EBA2A
		private bool FairCombatEnabled
		{
			get
			{
				return CricketFairCombatHelper.IsEnabled;
			}
		}

		// Token: 0x17000EDA RID: 3802
		// (get) Token: 0x0600871C RID: 34588 RVA: 0x003ED831 File Offset: 0x003EBA31
		private int SelectedFairCombatPoint
		{
			get
			{
				return CricketFairCombatHelper.GetSelectedCost(CricketCombatKit.Board.SelfCrickets);
			}
		}

		// Token: 0x17000EDB RID: 3803
		// (get) Token: 0x0600871D RID: 34589 RVA: 0x003ED842 File Offset: 0x003EBA42
		private int MaxFairCombatPoint
		{
			get
			{
				return CricketFairCombatHelper.GetMaxPointByWager(CricketCombatKit.Board.EnemyWager);
			}
		}

		// Token: 0x0600871E RID: 34590 RVA: 0x003ED854 File Offset: 0x003EBA54
		public override void OnInit(ArgumentBox argsBox)
		{
			this.GenerateComponentCache();
			ViewCricketCombat.AnalysisArgs(argsBox);
			bool fb;
			this._fromBetting = (argsBox.Get("FromBetting", out fb) && fb);
			bool fromBetting = this._fromBetting;
			if (fromBetting)
			{
				argsBox.Get("BettingSelectedReward", out this._bettingSelectedReward);
			}
			this.OnEvent(ECricketCombatGlobalEventType.Initialize, argsBox);
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
			long selfWagerValue;
			argsBox.Get("SelfWagerValue", out selfWagerValue);
			long enemyWagerValue;
			argsBox.Get("EnemyWagerValue", out enemyWagerValue);
			this.selfWagerView.SetData(CricketCombatKit.Board.SelfWager, selfWagerValue);
			this.enemyWagerView.SetData(CricketCombatKit.Board.EnemyWager, enemyWagerValue);
			this.RefreshFairCombatPointUI();
			this.combatProperties.alpha = 0f;
			AudioManager.Instance.PlayMusic("cricket_battle_loop", 1f, 100, null);
			AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 1f, 100);
		}

		// Token: 0x0600871F RID: 34591 RVA: 0x003ED96C File Offset: 0x003EBB6C
		private static void AnalysisArgs(ArgumentBox argsBox)
		{
			int enemyId;
			argsBox.Get("EnemyId", out enemyId);
			Wager selfWager;
			argsBox.Get<Wager>("SelfWager", out selfWager);
			CricketWagerData enemyWager;
			argsBox.Get<CricketWagerData>("EnemyWagerData", out enemyWager);
			bool doubleDamage;
			argsBox.Get("DoubleDamage", out doubleDamage);
			bool onlyNoInjuryCricket;
			argsBox.Get("OnlyNoInjuryCricket", out onlyNoInjuryCricket);
			sbyte minGrade;
			bool flag = !argsBox.Get("MinGrade", out minGrade);
			if (flag)
			{
				minGrade = 0;
			}
			sbyte maxGrade;
			bool flag2 = !argsBox.Get("MaxGrade", out maxGrade);
			if (flag2)
			{
				maxGrade = 8;
			}
			CricketCombatKit.Board.Initialize(new ValueTuple<bool, sbyte, sbyte>(onlyNoInjuryCricket, minGrade, maxGrade), enemyId, doubleDamage, selfWager, enemyWager);
		}

		// Token: 0x06008720 RID: 34592 RVA: 0x003EDA11 File Offset: 0x003EBC11
		private void Awake()
		{
			PoolManager.SetSrcObject("UI_CricketCombat_DamagePrefabKey", this.damagePrefab);
		}

		// Token: 0x06008721 RID: 34593 RVA: 0x003EDA25 File Offset: 0x003EBC25
		private void OnDestroy()
		{
			PoolManager.RemoveData("UI_CricketCombat_DamagePrefabKey");
		}

		// Token: 0x06008722 RID: 34594 RVA: 0x003EDA33 File Offset: 0x003EBC33
		private void OnListenerIdReady()
		{
			CricketCombatKit.Board.RequestData(this);
		}

		// Token: 0x06008723 RID: 34595 RVA: 0x003EDA44 File Offset: 0x003EBC44
		private void OnEnable()
		{
			this.Element.ShowAfterRefresh();
			GEvent.Add(EEvents.OnConfirmQuitGameState, new GEvent.Callback(this.OnConfirmQuitGameState));
			CommandKitBase.SetDisable(true);
			this.startAnim.DOFade(1f, 0.2f);
			this.startAnim.DOFade(0f, 0.2f).SetDelay(2f);
			this.startBg.DOFade(1f, 0f).OnStart(delegate
			{
				this.startBg.raycastTarget = true;
			});
			this.startBg.DOFade(0f, 0.2f).SetDelay(2f).OnComplete(new TweenCallback(this.OnOpeningAnimationComplete));
			this.startAnim.GetComponentsInChildren<SkeletonGraphic>().ForEach(delegate(int _, SkeletonGraphic graphic)
			{
				graphic.AnimationState.SetAnimation(0, "animation", false);
				return false;
			});
		}

		// Token: 0x06008724 RID: 34596 RVA: 0x003EDB3C File Offset: 0x003EBD3C
		private void Update()
		{
			bool flag = !this.startBg.raycastTarget;
			if (flag)
			{
				this.MainLoop();
			}
			bool pendingPanelRefresh = this._pendingPanelRefresh;
			if (pendingPanelRefresh)
			{
				this._pendingPanelRefresh = false;
				bool showingSelectItemPanel = this._showingSelectItemPanel;
				if (showingSelectItemPanel)
				{
					this.RefreshSelectItemPanel();
				}
			}
		}

		// Token: 0x06008725 RID: 34597 RVA: 0x003EDB88 File Offset: 0x003EBD88
		private void OnDisable()
		{
			CommandKitBase.SetDisable(false);
			this.HideSelectItemPanel();
			this.ClearRunningSequences();
			GEvent.Remove(EEvents.OnConfirmQuitGameState, new GEvent.Callback(this.OnConfirmQuitGameState));
			SingletonObject.getInstance<WorldMapModel>().UpdateBgm();
			Time.timeScale = 1f;
		}

		// Token: 0x06008726 RID: 34598 RVA: 0x003EDBDC File Offset: 0x003EBDDC
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				byte type = notification.Type;
				byte b = type;
				if (b == 1)
				{
					ushort domainId = notification.DomainId;
					ushort num = domainId;
					if (num == 6)
					{
						this.HandlerMethodItemDomain(wrapper, notification);
					}
				}
			}
		}

		// Token: 0x06008727 RID: 34599 RVA: 0x003EDC60 File Offset: 0x003EBE60
		public override void QuickHide()
		{
			bool showingSelectItemPanel = this._showingSelectItemPanel;
			if (showingSelectItemPanel)
			{
				this.HideSelectItemPanel();
			}
			else
			{
				base.QuickHide();
			}
		}

		// Token: 0x06008728 RID: 34600 RVA: 0x003EDC8C File Offset: 0x003EBE8C
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			bool flag = btnName.StartsWith("Jar");
			if (flag)
			{
				this.OnClickJar(btn.transform.GetSiblingIndex(), btn.GetComponent<CricketJar>());
			}
			else
			{
				bool flag2 = btnName == "StartCombat";
				if (flag2)
				{
					this.OnClickStartCombat();
				}
				else
				{
					bool flag3 = btnName == "SwitchPause";
					if (flag3)
					{
						this.OnClickSwitchPause();
					}
					else
					{
						bool flag4 = btnName == "SpeedDown";
						if (flag4)
						{
							this.OnClickSpeedDown();
						}
						else
						{
							bool flag5 = btnName == "SpeedUp";
							if (flag5)
							{
								this.OnClickSpeedUp();
							}
							else
							{
								bool flag6 = btnName == "TaiwuGiveUp";
								if (flag6)
								{
									this.OnClickTaiwuGiveUp();
								}
								else
								{
									bool flag7 = btnName == "ForceGiveUp";
									if (flag7)
									{
										this.OnClickForceGiveUp();
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06008729 RID: 34601 RVA: 0x003EDD64 File Offset: 0x003EBF64
		private void OnConfirmQuitGameState(ArgumentBox argBox)
		{
			bool show;
			argBox.Get("ShowState", out show);
			Time.timeScale = (show ? 0f : this.combatActions.CurrentTimeScale);
			bool flag = show;
			if (flag)
			{
				AudioManager.Instance.Pause();
			}
			else
			{
				AudioManager.Instance.Resume();
			}
		}

		// Token: 0x0600872A RID: 34602 RVA: 0x003EDDB8 File Offset: 0x003EBFB8
		private void MainLoop()
		{
			bool viewingCharacterMenu = this._viewingCharacterMenu;
			if (!viewingCharacterMenu)
			{
				for (;;)
				{
					if (!this._runningSequences.All((Sequence sequence) => !sequence.IsPlaying()))
					{
						break;
					}
					this._runningSequences.Clear();
					CricketCombatLog nextLog = CricketCombatKit.Board.NextLog();
					bool flag = nextLog == null;
					if (flag)
					{
						break;
					}
					this.HandleLog(nextLog);
				}
				bool flag2 = this._runningSequences.Count == 0 && CricketCombatKit.Board.Status == ECricketCombatStatus.Combating;
				if (flag2)
				{
					CricketCombatKit.Board.EndCombat();
				}
			}
		}

		// Token: 0x0600872B RID: 34603 RVA: 0x003EDE60 File Offset: 0x003EC060
		private void GenerateComponentCache()
		{
			List<ICricketCombatComponent> components = this._components;
			bool flag = components != null && components.Count > 0;
			if (!flag)
			{
				base.GetComponentsInChildren<ICricketCombatComponent>(true, this._components);
				foreach (ICricketCombatComponent component in this._components)
				{
					component.Handler = this;
				}
				this.combatActions.OnCancelRequested = new Action(this.OnClickCloseSelectItemPanelOrTaiwuGiveUp);
				this.combatActions.OnStartCombat = new Action(this.OnClickStartCombat);
				this.combatActions.OnForceGiveUp = new Action(this.OnClickForceGiveUp);
			}
		}

		// Token: 0x0600872C RID: 34604 RVA: 0x003EDF2C File Offset: 0x003EC12C
		private void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox = null)
		{
			foreach (ICricketCombatComponent component in this._components)
			{
				component.OnEvent(type, argBox);
			}
			bool flag = type == ECricketCombatGlobalEventType.Initialize || type == ECricketCombatGlobalEventType.SelfCricketChanged || type == ECricketCombatGlobalEventType.CombatStatusChanged;
			if (flag)
			{
				this.RefreshFairCombatPointUI();
			}
			bool flag2 = type == ECricketCombatGlobalEventType.SelfCricketChanged && this._showingSelectItemPanel && !this._previewing;
			if (flag2)
			{
				this.RefreshSelectItemPanel();
			}
			bool flag3 = type == ECricketCombatGlobalEventType.CharacterMenuShowed;
			if (flag3)
			{
				this._viewingCharacterMenu = true;
				foreach (Sequence seq in this._runningSequences)
				{
					bool flag4 = seq.IsActive() && seq.IsPlaying();
					if (flag4)
					{
						seq.Pause<Sequence>();
					}
				}
			}
			else
			{
				bool flag5 = type == ECricketCombatGlobalEventType.CharacterMenuHide;
				if (flag5)
				{
					foreach (Sequence seq2 in this._runningSequences)
					{
						bool flag6 = seq2.IsActive() && !seq2.IsPlaying();
						if (flag6)
						{
							seq2.Play<Sequence>();
						}
					}
					this._viewingCharacterMenu = false;
				}
			}
		}

		// Token: 0x0600872D RID: 34605 RVA: 0x003EE0B4 File Offset: 0x003EC2B4
		private void HandleLog(CricketCombatLog log)
		{
			this._components.Sort((ICricketCombatComponent a, ICricketCombatComponent b) => a.GetPriority(log).CompareTo(b.GetPriority(log)));
			Sequence sequence = null;
			foreach (ICricketCombatComponent component in this._components)
			{
				sequence = component.HandleLog(log, sequence);
			}
			bool flag = sequence == null;
			if (!flag)
			{
				sequence.Play<Sequence>();
				this._runningSequences.Add(sequence);
			}
		}

		// Token: 0x17000EDC RID: 3804
		// (get) Token: 0x0600872E RID: 34606 RVA: 0x003EE158 File Offset: 0x003EC358
		IAsyncMethodRequestHandler ICricketCombatHandler.Async
		{
			get
			{
				return this;
			}
		}

		// Token: 0x0600872F RID: 34607 RVA: 0x003EE15B File Offset: 0x003EC35B
		void ICricketCombatHandler.OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox)
		{
			this.OnEvent(type, argBox);
		}

		// Token: 0x06008730 RID: 34608 RVA: 0x003EE168 File Offset: 0x003EC368
		bool ICricketCombatHandler.CanReorderSelfCricket(int fromJarIndex, int toJarIndex)
		{
			return this.CanReorderSelfCricket(fromJarIndex, toJarIndex);
		}

		// Token: 0x06008731 RID: 34609 RVA: 0x003EE182 File Offset: 0x003EC382
		void ICricketCombatHandler.ReorderSelfCricket(int fromJarIndex, int toJarIndex)
		{
			this.ReorderSelfCricket(fromJarIndex, toJarIndex, true);
		}

		// Token: 0x06008732 RID: 34610 RVA: 0x003EE190 File Offset: 0x003EC390
		private void HandlerMethodItemDomain(NotificationWrapper wrapper, Notification notification)
		{
			ushort methodId = notification.MethodId;
			ushort num = methodId;
			if (num == 31 || num == 33)
			{
				Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._settlementResult);
				this.HandlerMethodItemSettlementCricketWager(this._settlementResult);
			}
		}

		// Token: 0x06008733 RID: 34611 RVA: 0x003EE1E0 File Offset: 0x003EC3E0
		private void HandlerMethodItemSettlementCricketWager(CricketSettlementResult result)
		{
			this.ClearRunningSequences();
			bool win = result.TaiwuWin;
			ArgumentBox argumentBox = new ArgumentBox();
			argumentBox.Set("IsWin", win);
			Wager wager = win ? CricketCombatKit.Board.EnemyWager : CricketCombatKit.Board.SelfWager;
			argumentBox.SetObject("Wager", wager);
			argumentBox.SetObject("WagerChar", CricketCombatKit.Board.GetWagerChar(wager));
			argumentBox.SetObject("ExtraWager", result.ExtraWager);
			argumentBox.SetObject("TaiwuChar", CricketCombatKit.Board.SelfChar);
			argumentBox.SetObject("EnemyChar", CricketCombatKit.Board.EnemyChar);
			UIElement.CricketCombatResult.SetOnInitArgs(argumentBox);
			UIManager.Instance.MaskUI(UIElement.CricketCombatResult);
		}

		// Token: 0x06008734 RID: 34612 RVA: 0x003EE2B4 File Offset: 0x003EC4B4
		private void OnClickJar(int jarIndex, CricketJar cricketJar)
		{
			bool requesting = this._requesting;
			if (!requesting)
			{
				bool flag = CricketCombatKit.Board.Status != ECricketCombatStatus.Preparing || !CricketCombatKit.Board.AllowSelectCricket;
				if (!flag)
				{
					bool flag2 = !this._showingSelectItemPanel;
					if (flag2)
					{
						this._originalCricketsSnapshot.Clear();
						for (int i = 0; i < 3; i++)
						{
							ItemDisplayData cricket = CricketCombatKit.Board.SelfCrickets.GetOrDefault(i);
							this._originalCricketsSnapshot.Add((cricket != null) ? cricket.Key : ItemKey.Invalid);
						}
					}
					this._selectingJarIndex = jarIndex;
					this.SetSelectedJar(cricketJar);
					ItemDisplayData orDefault = CricketCombatKit.Board.SelfCrickets.GetOrDefault(jarIndex);
					ItemKey initItemKey = (orDefault != null) ? orDefault.Key : ItemKey.Invalid;
					List<ItemDisplayData> cricketList = this.BuildSelectableCrickets(jarIndex);
					bool showFairCombatPoint = this.FairCombatEnabled && CricketCombatKit.Board.Status == ECricketCombatStatus.Preparing;
					bool showingSelectItemPanel = this._showingSelectItemPanel;
					if (showingSelectItemPanel)
					{
						this.selectItemPanel.RefreshCandidates(cricketList, initItemKey, new Func<ItemDisplayData, bool>(this.CanSelectCricketItemForCurrentJar), showFairCombatPoint, this.SelectedFairCombatPoint, this.MaxFairCombatPoint, new Func<ItemKey, int>(this.GetSelectedFairCombatPointForCurrentJar));
					}
					else
					{
						this._showingSelectItemPanel = true;
						this.selectItemPanel.Show(cricketList, initItemKey, new Func<ItemDisplayData, bool>(this.CanSelectCricketItemForCurrentJar), showFairCombatPoint, this.SelectedFairCombatPoint, this.MaxFairCombatPoint, new Func<ItemKey, int>(this.GetSelectedFairCombatPointForCurrentJar), new Action<ItemKey>(this.OnPreviewSelectCricket), new Action<ItemKey>(this.OnClickSelectCricket), new Action(this.OnSelectItemPanelClose));
						UIManager.Instance.MaskComponent(this.selectItemLayer);
						this.presetTitle.SetActive(false);
					}
				}
			}
		}

		// Token: 0x06008735 RID: 34613 RVA: 0x003EE470 File Offset: 0x003EC670
		private void OnPreviewSelectCricket(ItemKey key)
		{
			bool flag = !key.IsValid();
			if (!flag)
			{
				this._previewing = true;
				int existingIndex = CricketCombatKit.Board.SelfCrickets.FindIndex((ItemDisplayData x) => x != null && x.Key == key);
				bool flag2 = existingIndex >= 0;
				if (flag2)
				{
					this.SetCricket(existingIndex, ItemKey.Invalid, true);
				}
				else
				{
					bool flag3 = this.CanSelectCricketForJar(this._selectingJarIndex, key);
					if (flag3)
					{
						this.SetCricket(this._selectingJarIndex, key, true);
					}
				}
				this._previewing = false;
				this._pendingPanelRefresh = true;
			}
		}

		// Token: 0x06008736 RID: 34614 RVA: 0x003EE517 File Offset: 0x003EC717
		private void OnClickSelectCricket(ItemKey key)
		{
			this.ConfirmSelection();
		}

		// Token: 0x06008737 RID: 34615 RVA: 0x003EE521 File Offset: 0x003EC721
		private void OnClickSpeedUp()
		{
			this.combatActions.IncreaseSpeed();
		}

		// Token: 0x06008738 RID: 34616 RVA: 0x003EE530 File Offset: 0x003EC730
		private void OnClickSpeedDown()
		{
			this.combatActions.DecreaseSpeed();
		}

		// Token: 0x06008739 RID: 34617 RVA: 0x003EE53F File Offset: 0x003EC73F
		private void OnClickSwitchPause()
		{
			this.combatActions.TogglePause();
		}

		// Token: 0x0600873A RID: 34618 RVA: 0x003EE550 File Offset: 0x003EC750
		private void OnClickStartCombat()
		{
			bool flag = !CricketFairCombatHelper.CanStartCombat(CricketCombatKit.Board.SelfCrickets, CricketCombatKit.Board.EnemyWager);
			if (!flag)
			{
				GlobalDomainMethod.Call.InvokeGuidingTrigger(275);
				CricketCombatKit.Board.ChangeAllowSelectCricket(false);
				this.ConfirmSelection();
				bool fromBetting = this._fromBetting;
				if (fromBetting)
				{
					TaiwuEventDomainMethod.Call.SetCricketBettingResult(true, CricketCombatKit.Board.SelfWager, this._bettingSelectedReward);
					UIManager.Instance.HideUI(UIElement.CricketBetting);
					this._fromBetting = false;
				}
				Tester.Assert(CricketCombatKit.Board.CurrentMatch == 0, "");
				this.combatActions.SetDisplayTimeScale(this.SettingData.CricketCombatSpeed);
				this.combatActions.SetInCombat(true);
				this.combatProperties.DOFade(1f, 0.3f);
				CricketCombatKit.Board.ChangeState(ECricketCombatStatus.Combating);
				this.RefreshFairCombatPointUI();
				CricketCombatKit.Board.StartCombat();
			}
		}

		// Token: 0x0600873B RID: 34619 RVA: 0x003EE64C File Offset: 0x003EC84C
		private void OnClickCloseSelectItemPanelOrTaiwuGiveUp()
		{
			bool showingSelectItemPanel = this._showingSelectItemPanel;
			if (showingSelectItemPanel)
			{
				this.HideSelectItemPanel();
			}
			else
			{
				this.OnClickTaiwuGiveUp();
			}
		}

		// Token: 0x0600873C RID: 34620 RVA: 0x003EE678 File Offset: 0x003EC878
		private void OnClickTaiwuGiveUp()
		{
			bool flag = this._fromBetting && CricketCombatKit.Board.Status == ECricketCombatStatus.Preparing;
			if (flag)
			{
				this.HideSelectItemPanel();
				UIManager.Instance.HideUI(this.Element);
			}
			else
			{
				bool tempPause = this.combatActions.IsPaused;
				bool flag2 = !tempPause;
				if (flag2)
				{
					this.OnClickSwitchPause();
				}
				DialogCmd dialogCmd = EasyPool.Get<DialogCmd>();
				dialogCmd.Type = 1;
				dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_CricketBattle_Exit);
				dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_CricketBattle_ExitWarning);
				dialogCmd.Yes = delegate()
				{
					this.DoSettlement(false, true);
				};
				dialogCmd.No = delegate()
				{
					bool flag3 = !tempPause;
					if (flag3)
					{
						this.OnClickSwitchPause();
					}
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}

		// Token: 0x0600873D RID: 34621 RVA: 0x003EE770 File Offset: 0x003EC970
		private void OnClickForceGiveUp()
		{
			bool flag = CricketCombatKit.Board.Status != ECricketCombatStatus.Combating;
			if (!flag)
			{
				this.OnEvent(ECricketCombatGlobalEventType.ForceGiveUpCheck, null);
				base.DelayCall(new Action(this.CheckEnemyGiveUp), 1.5f);
			}
		}

		// Token: 0x0600873E RID: 34622 RVA: 0x003EE7B8 File Offset: 0x003EC9B8
		private void CheckEnemyGiveUp()
		{
			bool flag = CricketCombatKit.Board.Status != ECricketCombatStatus.Combating;
			if (!flag)
			{
				bool flag2 = !UIManager.Instance.IsFocusElement(this.Element);
				if (flag2)
				{
					this.combatActions.SetForceGiveUpInteractable(true);
				}
				else
				{
					int selfCricketsGrade = CricketCombatKit.SumCricketGrades(CricketCombatKit.Board.SelfCrickets);
					int enemyCricketsGrade = CricketCombatKit.SumCricketGrades(CricketCombatKit.Board.EnemyCrickets);
					bool flag3 = enemyCricketsGrade <= selfCricketsGrade - 12;
					if (flag3)
					{
						this.DoSettlement(true, true);
					}
					else
					{
						bool flag4 = CricketCombatKit.Board.MatchWinCount - CricketCombatKit.Board.MatchLoseCount < 1;
						if (flag4)
						{
							this.ForceGiveUpFail();
						}
						else
						{
							int selfCricketsGradeRemain = CricketCombatKit.SumCricketGrades(CricketCombatKit.Board.SelfCrickets.Skip(CricketCombatKit.Board.CurrentMatch));
							int enemyCricketsGradeRemain = CricketCombatKit.SumCricketGrades(CricketCombatKit.Board.EnemyCrickets.Skip(CricketCombatKit.Board.CurrentMatch));
							bool flag5 = enemyCricketsGradeRemain <= selfCricketsGradeRemain - 6;
							if (flag5)
							{
								this.DoSettlement(true, true);
							}
							this.ForceGiveUpFail();
						}
					}
				}
			}
		}

		// Token: 0x0600873F RID: 34623 RVA: 0x003EE8D1 File Offset: 0x003ECAD1
		private void ForceGiveUpFail()
		{
			this.OnEvent(ECricketCombatGlobalEventType.ForceGiveUpRefuse, null);
		}

		// Token: 0x06008740 RID: 34624 RVA: 0x003EE8E0 File Offset: 0x003ECAE0
		private void SetCricket(int cricketIndex, ItemKey key, bool syncToPlan = false)
		{
			bool flag = !key.IsValid();
			if (flag)
			{
				CricketCombatKit.Board.SelfCrickets.SetOrAdd(cricketIndex, null, null);
				this.SetCricketPlan(cricketIndex, ItemKey.Invalid, syncToPlan);
			}
			else
			{
				int index = CricketCombatKit.Board.SelfCrickets.FindIndex((ItemDisplayData x) => x != null && x.Key == key);
				bool flag2 = index >= 0;
				if (flag2)
				{
					CricketCombatKit.Board.SelfCrickets.SetOrAdd(index, null, null);
					this.SetCricketPlan(index, ItemKey.Invalid, syncToPlan);
				}
				ItemDisplayData data = CricketCombatKit.Board.TaiwuAllowCrickets[key.Id];
				CricketCombatKit.Board.SelfCrickets.SetOrAdd(cricketIndex, data, null);
				this.SetCricketPlan(cricketIndex, key, syncToPlan);
			}
			this.RefreshFairCombatPointUI();
			this.OnEvent(ECricketCombatGlobalEventType.SelfCricketChanged, null);
		}

		// Token: 0x06008741 RID: 34625 RVA: 0x003EE9D0 File Offset: 0x003ECBD0
		private List<ItemDisplayData> BuildSelectableCrickets(int jarIndex)
		{
			List<ItemDisplayData> cricketList = new List<ItemDisplayData>();
			foreach (ItemDisplayData cricket in CricketCombatKit.Board.TaiwuAllowCrickets.Values)
			{
				bool flag = CricketCombatKit.Board.SelfWager.Type == 1 && cricket.Key.Equals(CricketCombatKit.Board.SelfWager.ItemKey);
				if (!flag)
				{
					cricketList.Add(cricket);
				}
			}
			return cricketList;
		}

		// Token: 0x06008742 RID: 34626 RVA: 0x003EEA74 File Offset: 0x003ECC74
		private void HideSelectItemPanel()
		{
			bool flag = !this._showingSelectItemPanel;
			if (!flag)
			{
				this.RevertAllJars();
				this.ClearSelectItemPanel();
			}
		}

		// Token: 0x06008743 RID: 34627 RVA: 0x003EEAA0 File Offset: 0x003ECCA0
		private void ConfirmSelection()
		{
			bool flag = !this._showingSelectItemPanel;
			if (!flag)
			{
				this.ClearSelectItemPanel();
			}
		}

		// Token: 0x06008744 RID: 34628 RVA: 0x003EEAC4 File Offset: 0x003ECCC4
		private void ClearSelectItemPanel()
		{
			this._showingSelectItemPanel = false;
			CricketCombatSelectItemPanel cricketCombatSelectItemPanel = this.selectItemPanel;
			if (cricketCombatSelectItemPanel != null)
			{
				cricketCombatSelectItemPanel.Hide(false);
			}
			UIManager.Instance.UnMaskComponent(this.selectItemLayer);
			this.presetTitle.SetActive(true);
			this.ClearSelectedJar();
		}

		// Token: 0x06008745 RID: 34629 RVA: 0x003EEB14 File Offset: 0x003ECD14
		private void RevertAllJars()
		{
			for (int i = 0; i < this._originalCricketsSnapshot.Count; i++)
			{
				ItemKey originalKey = this._originalCricketsSnapshot[i];
				this.SetCricket(i, originalKey, true);
			}
		}

		// Token: 0x06008746 RID: 34630 RVA: 0x003EEB58 File Offset: 0x003ECD58
		private void RefreshSelectItemPanel()
		{
			ItemDisplayData orDefault = CricketCombatKit.Board.SelfCrickets.GetOrDefault(this._selectingJarIndex);
			ItemKey initItemKey = (orDefault != null) ? orDefault.Key : ItemKey.Invalid;
			List<ItemDisplayData> cricketList = this.BuildSelectableCrickets(this._selectingJarIndex);
			bool showFairCombatPoint = this.FairCombatEnabled && CricketCombatKit.Board.Status == ECricketCombatStatus.Preparing;
			this.selectItemPanel.RefreshCandidates(cricketList, initItemKey, new Func<ItemDisplayData, bool>(this.CanSelectCricketItemForCurrentJar), showFairCombatPoint, this.SelectedFairCombatPoint, this.MaxFairCombatPoint, new Func<ItemKey, int>(this.GetSelectedFairCombatPointForCurrentJar));
		}

		// Token: 0x06008747 RID: 34631 RVA: 0x003EEBE5 File Offset: 0x003ECDE5
		private void SetSelectedJar(CricketJar jar)
		{
			this.ClearSelectedJar();
			this._selectedJar = jar;
			CricketJar selectedJar = this._selectedJar;
			if (selectedJar != null)
			{
				selectedJar.SetSelected(true);
			}
		}

		// Token: 0x06008748 RID: 34632 RVA: 0x003EEC0C File Offset: 0x003ECE0C
		private void ClearSelectedJar()
		{
			bool flag = this._selectedJar == null;
			if (!flag)
			{
				this._selectedJar.SetSelected(false);
				this._selectedJar = null;
			}
		}

		// Token: 0x06008749 RID: 34633 RVA: 0x003EEC40 File Offset: 0x003ECE40
		private bool CanReorderSelfCricket(int fromJarIndex, int toJarIndex)
		{
			bool flag = CricketCombatKit.Board.Status != ECricketCombatStatus.Preparing || !CricketCombatKit.Board.AllowSelectCricket;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = fromJarIndex == toJarIndex;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = fromJarIndex < 0 || fromJarIndex >= 3;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = toJarIndex < 0 || toJarIndex >= 3;
						result = (!flag4 && CricketCombatKit.Board.SelfCrickets.GetOrDefault(fromJarIndex) != null);
					}
				}
			}
			return result;
		}

		// Token: 0x0600874A RID: 34634 RVA: 0x003EECC4 File Offset: 0x003ECEC4
		private void ReorderSelfCricket(int fromJarIndex, int toJarIndex, bool syncToPlan)
		{
			bool flag = !this.CanReorderSelfCricket(fromJarIndex, toJarIndex);
			if (!flag)
			{
				this.HideSelectItemPanel();
				ItemDisplayData fromCricket = CricketCombatKit.Board.SelfCrickets[fromJarIndex];
				ItemDisplayData toCricket = CricketCombatKit.Board.SelfCrickets.GetOrDefault(toJarIndex);
				CricketCombatKit.Board.SelfCrickets.SetOrAdd(fromJarIndex, toCricket, null);
				CricketCombatKit.Board.SelfCrickets.SetOrAdd(toJarIndex, fromCricket, null);
				this.SetCricketPlan(fromJarIndex, (toCricket != null) ? toCricket.Key : ItemKey.Invalid, syncToPlan);
				this.SetCricketPlan(toJarIndex, fromCricket.Key, syncToPlan);
				this.RefreshFairCombatPointUI();
				this.OnEvent(ECricketCombatGlobalEventType.SelfCricketChanged, null);
			}
		}

		// Token: 0x0600874B RID: 34635 RVA: 0x003EED6D File Offset: 0x003ECF6D
		private void OnSelectItemPanelClose()
		{
			this.HideSelectItemPanel();
		}

		// Token: 0x0600874C RID: 34636 RVA: 0x003EED78 File Offset: 0x003ECF78
		private bool CanSelectCricketForJar(int cricketIndex, ItemKey key)
		{
			bool flag = !this.FairCombatEnabled;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				int point = this.SelectedFairCombatPoint - CricketFairCombatHelper.GetCricketCost(CricketCombatKit.Board.SelfCrickets.GetOrDefault(cricketIndex));
				bool flag2 = key.IsValid();
				if (flag2)
				{
					point += CricketFairCombatHelper.GetCricketCost(CricketCombatKit.Board.TaiwuAllowCrickets[key.Id]);
				}
				result = (point <= this.MaxFairCombatPoint);
			}
			return result;
		}

		// Token: 0x0600874D RID: 34637 RVA: 0x003EEDEC File Offset: 0x003ECFEC
		private bool CanSelectCricketItemForCurrentJar(ItemDisplayData itemData)
		{
			return itemData != null;
		}

		// Token: 0x0600874E RID: 34638 RVA: 0x003EEE04 File Offset: 0x003ED004
		private int GetSelectedFairCombatPointForCurrentJar(ItemKey key)
		{
			int point = this.SelectedFairCombatPoint - CricketFairCombatHelper.GetCricketCost(CricketCombatKit.Board.SelfCrickets.GetOrDefault(this._selectingJarIndex));
			bool flag = key.IsValid();
			if (flag)
			{
				point += CricketFairCombatHelper.GetCricketCost(CricketCombatKit.Board.TaiwuAllowCrickets[key.Id]);
			}
			return point;
		}

		// Token: 0x0600874F RID: 34639 RVA: 0x003EEE64 File Offset: 0x003ED064
		private void RefreshFairCombatPointUI()
		{
			bool showFairCombatPoint = this.FairCombatEnabled && CricketCombatKit.Board.Status == ECricketCombatStatus.Preparing;
			this.fairCombatPointRoot.SetActive(showFairCombatPoint);
			bool flag = !showFairCombatPoint;
			if (flag)
			{
				for (int i = 0; i < this.fairCombatJarPointRoots.Length; i++)
				{
					this.fairCombatJarPointRoots[i].SetActive(false);
				}
			}
			else
			{
				this.fairCombatPointText.text = LanguageKey.LK_CricketCombat_FairCombat_Point.TrFormat(this.SelectedFairCombatPoint, this.MaxFairCombatPoint);
				for (int j = 0; j < this.fairCombatJarPointRoots.Length; j++)
				{
					ItemDisplayData cricket = CricketCombatKit.Board.SelfCrickets.GetOrDefault(j);
					bool showJarPoint = cricket != null;
					this.fairCombatJarPointRoots[j].SetActive(showJarPoint);
					bool flag2 = showJarPoint;
					if (flag2)
					{
						this.fairCombatJarPointTexts[j].text = CricketFairCombatHelper.GetCricketCost(cricket).ToString();
					}
				}
			}
		}

		// Token: 0x06008750 RID: 34640 RVA: 0x003EEF6C File Offset: 0x003ED16C
		private void SetCricketPlan(int cricketIndex, ItemKey cricket, bool syncToPlan)
		{
			bool flag = syncToPlan && CricketCombatKit.Board.CurrentCricketPlanIndex >= 0;
			if (flag)
			{
				TaiwuDomainMethod.Call.SetCricketPlan(CricketCombatKit.Board.CurrentCricketPlanIndex, cricket, cricketIndex);
				CricketCombatKit.Board.UpdateCurrentCricketPlanCricket(cricketIndex, cricket);
			}
		}

		// Token: 0x06008751 RID: 34641 RVA: 0x003EEFB8 File Offset: 0x003ED1B8
		private void ClearRunningSequences()
		{
			foreach (Sequence seq in this._runningSequences)
			{
				bool flag = seq != null && (seq.IsActive() || !seq.IsComplete());
				if (flag)
				{
					seq.Kill(false);
				}
			}
			this._runningSequences.Clear();
		}

		// Token: 0x06008752 RID: 34642 RVA: 0x003EF03C File Offset: 0x003ED23C
		private void OnOpeningAnimationComplete()
		{
			this.startBg.raycastTarget = false;
			CommandKitBase.SetDisable(false);
		}

		// Token: 0x06008753 RID: 34643 RVA: 0x003EF054 File Offset: 0x003ED254
		public void DoSettlement(bool win, bool giveUp = false)
		{
			CricketCombatKit.Board.ChangeState(ECricketCombatStatus.Finishing);
			this.RefreshFairCombatPointUI();
			Time.timeScale = 1f;
			if (giveUp)
			{
				CricketJar selfCricketJar = CricketCombatKit.Board.SelfCricketJar;
				if (selfCricketJar != null)
				{
					selfCricketJar.Settlement(win);
				}
				CricketJar enemyCricketJar = CricketCombatKit.Board.EnemyCricketJar;
				if (enemyCricketJar != null)
				{
					enemyCricketJar.Settlement(!win);
				}
				ItemDomainMethod.Call.SettlementCricketWagerByGiveUp(this.Element.GameDataListenerId, win, CricketCombatKit.Board.InvokeExtraWager);
			}
			else
			{
				ItemKey[] cricketKeys = (from x in CricketCombatKit.Board.SelfCrickets
				select x.Key).ToArray<ItemKey>();
				short[] durabilityList = (from x in CricketCombatKit.Board.SelfCrickets
				select x.Durability).ToArray<short>();
				ItemDomainMethod.Call.SettlementCricketWager(this.Element.GameDataListenerId, win, cricketKeys, durabilityList, CricketCombatKit.Board.InvokeExtraWager);
			}
		}

		// Token: 0x040067C0 RID: 26560
		public const string DamagePrefabKey = "UI_CricketCombat_DamagePrefabKey";

		// Token: 0x040067C1 RID: 26561
		[SerializeField]
		private CImage startBg;

		// Token: 0x040067C2 RID: 26562
		[SerializeField]
		private CricketBettingWagerView selfWagerView;

		// Token: 0x040067C3 RID: 26563
		[SerializeField]
		private CricketBettingWagerView enemyWagerView;

		// Token: 0x040067C4 RID: 26564
		[SerializeField]
		private CanvasGroup combatProperties;

		// Token: 0x040067C5 RID: 26565
		[SerializeField]
		private GameObject damagePrefab;

		// Token: 0x040067C6 RID: 26566
		[SerializeField]
		private CanvasGroup startAnim;

		// Token: 0x040067C7 RID: 26567
		[SerializeField]
		private CricketCombatActions combatActions;

		// Token: 0x040067C8 RID: 26568
		[SerializeField]
		private RectTransform selectItemLayer;

		// Token: 0x040067C9 RID: 26569
		[SerializeField]
		private CricketCombatSelectItemPanel selectItemPanel;

		// Token: 0x040067CA RID: 26570
		[SerializeField]
		private GameObject fairCombatPointRoot;

		// Token: 0x040067CB RID: 26571
		[SerializeField]
		private TextMeshProUGUI fairCombatPointText;

		// Token: 0x040067CC RID: 26572
		[SerializeField]
		private GameObject[] fairCombatJarPointRoots = Array.Empty<GameObject>();

		// Token: 0x040067CD RID: 26573
		[SerializeField]
		private TextMeshProUGUI[] fairCombatJarPointTexts = Array.Empty<TextMeshProUGUI>();

		// Token: 0x040067CE RID: 26574
		[SerializeField]
		private GameObject presetTitle;

		// Token: 0x040067CF RID: 26575
		private readonly List<ICricketCombatComponent> _components = new List<ICricketCombatComponent>();

		// Token: 0x040067D0 RID: 26576
		private readonly HashSet<Sequence> _runningSequences = new HashSet<Sequence>();

		// Token: 0x040067D1 RID: 26577
		private bool _requesting;

		// Token: 0x040067D2 RID: 26578
		private bool _viewingCharacterMenu;

		// Token: 0x040067D3 RID: 26579
		private int _selectingJarIndex;

		// Token: 0x040067D4 RID: 26580
		private bool _fromBetting;

		// Token: 0x040067D5 RID: 26581
		private int _bettingSelectedReward;

		// Token: 0x040067D6 RID: 26582
		private bool _showingSelectItemPanel;

		// Token: 0x040067D7 RID: 26583
		private CricketJar _selectedJar;

		// Token: 0x040067D8 RID: 26584
		private readonly List<ItemKey> _originalCricketsSnapshot = new List<ItemKey>();

		// Token: 0x040067D9 RID: 26585
		private bool _pendingPanelRefresh;

		// Token: 0x040067DA RID: 26586
		private bool _previewing;

		// Token: 0x040067DB RID: 26587
		private CricketSettlementResult _settlementResult = new CricketSettlementResult();
	}
}
