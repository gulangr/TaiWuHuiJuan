using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.Components.EffectPlayer;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.Common;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.CombatSkill;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.SkillBreak
{
	// Token: 0x02000B5D RID: 2909
	public class ViewCharacterMenuSkillBreakPlate : UIBase
	{
		// Token: 0x17000FA9 RID: 4009
		// (get) Token: 0x06008F5A RID: 36698 RVA: 0x0042C33F File Offset: 0x0042A53F
		private UnityEngine.Material matPurpleBg
		{
			get
			{
				return this.purpleBgImage.material;
			}
		}

		// Token: 0x17000FAA RID: 4010
		// (get) Token: 0x06008F5B RID: 36699 RVA: 0x0042C34C File Offset: 0x0042A54C
		private UnityEngine.Material matYellowBg
		{
			get
			{
				return this.yellowBgImage.material;
			}
		}

		// Token: 0x17000FAB RID: 4011
		// (get) Token: 0x06008F5C RID: 36700 RVA: 0x0042C359 File Offset: 0x0042A559
		public AttributeAndInjuryDynamic CharacterAttributeDataView
		{
			get
			{
				return this.attributeView;
			}
		}

		// Token: 0x17000FAC RID: 4012
		// (get) Token: 0x06008F5D RID: 36701 RVA: 0x0042C361 File Offset: 0x0042A561
		private bool SwapModeActive
		{
			get
			{
				return this.swapSkillArea.gameObject.activeSelf;
			}
		}

		// Token: 0x17000FAD RID: 4013
		// (get) Token: 0x06008F5E RID: 36702 RVA: 0x0042C373 File Offset: 0x0042A573
		private GameData.Domains.Taiwu.SkillBreakPlate DisplayPlate
		{
			get
			{
				SkillBreakPlateRenderer skillBreakPlateRenderer = this.gridArea;
				return (skillBreakPlateRenderer != null) ? skillBreakPlateRenderer.DisplayPlate : null;
			}
		}

		// Token: 0x17000FAE RID: 4014
		// (get) Token: 0x06008F5F RID: 36703 RVA: 0x0042C387 File Offset: 0x0042A587
		private bool CanQuit
		{
			get
			{
				return !this._waitingAnimationInFinish && !this.cover.activeSelf;
			}
		}

		// Token: 0x06008F60 RID: 36704 RVA: 0x0042C3A4 File Offset: 0x0042A5A4
		private void Awake()
		{
			this.mouseWheelScaleCustom.listeningUI = this.Element;
			this._infomationAreaLeftCanvasGroup = this.infomationAreaLeft.GetComponent<CanvasGroup>();
			this._infomationAreaRightCanvasGroup = this.infomationAreaRight.GetComponent<CanvasGroup>();
			this._infomationAreaLeftCanvasGroup.alpha = 0f;
			this._infomationAreaRightCanvasGroup.alpha = 0f;
			this.ResetAreaMasksToNormal();
		}

		// Token: 0x06008F61 RID: 36705 RVA: 0x0042C40E File Offset: 0x0042A60E
		private void SetSnakeAnimation(string animationName, bool loop = false)
		{
			this.blackSnakeSkeleton.AnimationState.SetAnimation(0, animationName, loop);
		}

		// Token: 0x06008F62 RID: 36706 RVA: 0x0042C428 File Offset: 0x0042A628
		public unsafe override void OnInit(ArgumentBox argsBox)
		{
			this.InitRefers();
			this.gridArea.Clear();
			this._health = -1;
			this._injuries.Initialize();
			this._currentExp = -1;
			this._lifeSkillAttainments.Initialize();
			*this._lifeSkillAttainments[0] = -1;
			this._baseCostExp = -1;
			this._usingEnterPlate = true;
			this._isLeftPanelOpen = this.infomationAreaLeft.activeSelf;
			this._isRightPanelOpen = this.infomationAreaRight.activeSelf;
			for (sbyte p = 0; p < 7; p += 1)
			{
				this._injuries.Set(p, true, sbyte.MaxValue);
				this._injuries.Set(p, false, sbyte.MaxValue);
			}
			this._waitingAnimationInFinish = true;
			this._lastPlate = null;
			this.ReadArgs(argsBox);
			this.PrepareAnimation();
			bool flag = this.AnimSequenceIn == null || this.AnimSequenceOut == null;
			if (flag)
			{
				this.GenerateAnimationSequence();
			}
			this._taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this.gridArea.OnCellClicked = new Action<SkillBreakPlateIndex>(this.OnClickCell);
			this.gridArea.OnPointerEnter = new Action<SkillBreakPlateIndex>(this.OnPointerEnterCell);
			this.gridArea.OnPointerExit = new Action<SkillBreakPlateIndex>(this.OnPointerExitCell);
			this.NeedDataListenerId = true;
			this.RefreshCombatSkillInfo();
			this.RefreshOutlineTips();
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
			UIElement element2 = this.Element;
			element2.OnShowed = (Action)Delegate.Combine(element2.OnShowed, new Action(delegate()
			{
				bool flag2 = !this._isReview && SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
				if (flag2)
				{
					TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg(EventActionKey.DefValue.TutorialEnterSkillBreak, EventTriggerParameter.DefValue.CombatSkillTemplateId.ArgBoxKey, (int)this._skillId);
					TaiwuEventDomainMethod.Call.TriggerListener(EventActionKey.DefValue.TutorialEnterSkillBreak, false);
				}
			}));
			this.hintTextSprite.Parse();
		}

		// Token: 0x06008F63 RID: 36707 RVA: 0x0042C5E0 File Offset: 0x0042A7E0
		private void PrepareAnimation()
		{
			this.SetInformationVisible(false);
			this.ResetAndFadeInStatusAvatar();
			this.levelEffectRoot.gameObject.SetActive(false);
			this._stateMarks.gameObject.SetActive(false);
			this._statusAvatarImage.gameObject.SetActive(false);
			AudioManager.Instance.PlaySound("ui_breach_open", false, false);
			this.startSwapButton.gameObject.SetActive(false);
		}

		// Token: 0x06008F64 RID: 36708 RVA: 0x0042C658 File Offset: 0x0042A858
		private void ReadArgs(ArgumentBox argsBox)
		{
			argsBox.Get("SkillId", out this._skillId);
			argsBox.Get("SelectedPage", out this._selectedPage);
			argsBox.Get("IsReview", out this._isReview);
			argsBox.Get<GameData.Domains.Taiwu.SkillBreakPlate>("SpecificDisplayPlate", out this._specificDisplayPlate);
			bool flag = !argsBox.Get<SkillBreakPlateIndex>("AutoOpenBonusCoordinate", out this._targetAutoOpenBonusIndex);
			if (flag)
			{
				this._targetAutoOpenBonusIndex = SkillBreakPlateIndex.Invalid;
			}
		}

		// Token: 0x06008F65 RID: 36709 RVA: 0x0042C6D4 File Offset: 0x0042A8D4
		private void OnListenerIdReady()
		{
			TaiwuDomainMethod.AsyncCall.GetEnterSkillBreakPlateInfo(null, this._skillId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._skillBreakExtraData);
				this._baseCostExp = this._skillBreakExtraData.BaseCostExp;
				this._swapFunctionUnlocked = this._skillBreakExtraData.WudangUpgradeInteractionUnlocked;
				bool flag2 = !this._waitingAnimationInFinish;
				if (flag2)
				{
					this.RefreshStartSwapButtonVisible(false, null);
				}
				this.RefreshCachedData();
			});
			bool flag = this._specificDisplayPlate != null;
			if (flag)
			{
				this.RefreshWithPlate(this._specificDisplayPlate, null);
			}
			else
			{
				TaiwuDomainMethod.Call.EnterSkillBreakPlate(this.Element.GameDataListenerId, this._skillId, this._selectedPage);
			}
			this.attributeView.CharacterId = this._taiwuCharId;
		}

		// Token: 0x06008F66 RID: 36710 RVA: 0x0042C74C File Offset: 0x0042A94C
		public override void InitMonitorFieldIds()
		{
			this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)((long)this._taiwuCharId), new uint[]
			{
				66U,
				97U,
				19U,
				26U
			}));
			this.MonitorFields.Add(new UIBase.MonitorDataField(5, 68, ulong.MaxValue, null));
		}

		// Token: 0x06008F67 RID: 36711 RVA: 0x0042C79C File Offset: 0x0042A99C
		private void GenerateAnimationSequence()
		{
			CanvasGroup enterAnimationGroup = this._statusAvatarAnimationRoot.GetComponent<CanvasGroup>();
			this.AnimSequenceIn = DOTween.Sequence();
			this.AnimSequenceIn.AppendInterval(1f);
			this.AnimSequenceIn.AppendCallback(delegate
			{
				base.<GenerateAnimationSequence>g__MoveAvatarAnimationToTopLeft|6();
				base.<GenerateAnimationSequence>g__FadeInInformation|4();
			});
			this.AnimSequenceIn.AppendInterval(0.5f);
			this.AnimSequenceIn.AppendCallback(delegate
			{
				this._waitingAnimationInFinish = false;
				GEvent.OnEvent(UiEvents.HideCharacterMenuAndSubPages, null);
				this.startSwapButton.gameObject.SetActive(this._swapFunctionUnlocked);
				this.RefreshUpAreaMove();
			});
			this.AnimSequenceIn.Pause<Sequence>();
			this.AnimSequenceOut = DOTween.Sequence();
			this.AnimSequenceOut.AppendCallback(delegate
			{
				GEvent.OnEvent(UiEvents.RestoreCharacterMenuAndSubPages, null);
				AudioManager.Instance.PlaySound("ui_breach_close", false, false);
				this._waitingAnimationOutFinish = true;
				base.<GenerateAnimationSequence>g__FadeOutInformation|5();
				base.<GenerateAnimationSequence>g__MoveAvatarAnimationToCenter|7();
				this.levelEffectRoot.gameObject.SetActive(false);
				this.startSwapButton.gameObject.SetActive(false);
			});
			this.AnimSequenceOut.AppendInterval(1f);
			this.AnimSequenceOut.AppendCallback(delegate
			{
				this._waitingAnimationOutFinish = false;
			});
			this.AnimSequenceOut.Pause<Sequence>();
		}

		// Token: 0x06008F68 RID: 36712 RVA: 0x0042C891 File Offset: 0x0042AA91
		private void SetInformationVisible(bool visible)
		{
			this.informationArea.alpha = (visible ? 1f : 0f);
			this._upArea.alpha = (visible ? 1f : 0f);
		}

		// Token: 0x06008F69 RID: 36713 RVA: 0x0042C8CC File Offset: 0x0042AACC
		private void ResetAndFadeInStatusAvatar()
		{
			CanvasGroup enterAnimationGroup = this._statusAvatarAnimationRoot.GetComponent<CanvasGroup>();
			enterAnimationGroup.alpha = 0f;
			this._statusAvatarAnimationRoot.gameObject.SetActive(true);
			this._statusAvatarAnimationRoot.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			this._statusAvatarAnimationRoot.transform.localScale = Vector3.one;
			enterAnimationGroup.DOFade(1f, 0.5f).SetDelay(0.6f).OnStart(delegate
			{
				bool flag = this.DisplayPlate == null;
				if (!flag)
				{
					this.PlayEffectLegacy(this.breakLevelEffectArray[(int)ViewCharacterMenuSkillBreakPlate.CalcBreakLevel(this.DisplayPlate)], this._statusAvatarAnimationRoot, true, 1.3f, "");
				}
			});
		}

		// Token: 0x06008F6A RID: 36714 RVA: 0x0042C95C File Offset: 0x0042AB5C
		private void OnEnable()
		{
			this.mainBg.SetActive(true);
			this.cover.SetActive(false);
			this.effSwitchToSkill.gameObject.SetActive(false);
			this.effSwitchToBreak.gameObject.SetActive(false);
			this.ExitSwapMode();
			GEvent.Add(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUIChanged));
			AudioManager.Instance.PlayAmbience("study_ambience", 1f, 100);
			GlobalDomainMethod.Call.InvokeGuidingTrigger(79);
		}

		// Token: 0x06008F6B RID: 36715 RVA: 0x0042C9E8 File Offset: 0x0042ABE8
		private void RefreshCachedData()
		{
			Debug.Log(string.Format("Test RefreshCachedData _skillBreakExtraData not null :{0}", this._skillBreakExtraData != null));
			bool flag = this._skillBreakExtraData == null;
			if (!flag)
			{
				this.skillDisplayItem.SetByCharacterMenuList(this._skillBreakExtraData.SkillDisplayDataSimple);
				this.RefreshStepTips();
			}
		}

		// Token: 0x06008F6C RID: 36716 RVA: 0x0042CA41 File Offset: 0x0042AC41
		private void RefreshSkillData()
		{
			Debug.Log("Test RefreshSkillData");
			TaiwuDomainMethod.AsyncCall.GetSkillBreakPlateSkillInfo(null, this._skillId, delegate(int offset, RawDataPool pool)
			{
				CombatSkillDisplayDataCharacterMenuListItem skillItem = null;
				Serializer.Deserialize(pool, offset, ref skillItem);
				this.skillDisplayItem.SetByCharacterMenuList(skillItem);
			});
		}

		// Token: 0x06008F6D RID: 36717 RVA: 0x0042CA68 File Offset: 0x0042AC68
		private void OnDisable()
		{
			this.ExitSwapMode();
			GEvent.Remove(UiEvents.TopUiChanged, new GEvent.Callback(this.OnTopUIChanged));
			this.CleanBonusLayout();
			this.CleanLevelEffectRoot();
			AudioManager.Instance.PlayAmbience(AudioManager.DummyAudioName, 1f, 100);
			GlobalDomainMethod.Call.InvokeGuidingTrigger(285);
		}

		// Token: 0x06008F6E RID: 36718 RVA: 0x0042CAC5 File Offset: 0x0042ACC5
		private void CleanBonusLayout()
		{
			this.bonusLayout.Clean();
		}

		// Token: 0x06008F6F RID: 36719 RVA: 0x0042CAD4 File Offset: 0x0042ACD4
		private void CleanLevelEffectRoot()
		{
			for (int i = this.levelEffectRoot.childCount - 1; i >= 0; i--)
			{
				Object.Destroy(this.levelEffectRoot.GetChild(i).gameObject);
			}
		}

		// Token: 0x06008F70 RID: 36720 RVA: 0x0042CB1C File Offset: 0x0042AD1C
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
						this.HandleMethodReturn(notification, wrapper);
					}
				}
				else
				{
					this.HandleDataModification(notification, wrapper);
				}
			}
		}

		// Token: 0x06008F71 RID: 36721 RVA: 0x0042CB9C File Offset: 0x0042AD9C
		private void HandleMethodReturn(Notification notification, NotificationWrapper wrapper)
		{
			ushort domainId = notification.DomainId;
			ushort methodId = notification.MethodId;
			RawDataPool pool = wrapper.DataPool;
			int offset = notification.ValueOffset;
			bool flag = domainId == 5 && methodId == 51;
			if (flag)
			{
				Serializer.Deserialize(pool, offset, ref this._enteredPlate);
				base.StartCoroutine(this.WaitOtherDataToRefreshByEnter());
			}
			else
			{
				bool flag2 = domainId == 5 && (methodId == 53 || methodId == 144 || methodId == 143 || methodId == 142 || methodId == 154 || methodId == 149 || methodId == 192);
				if (flag2)
				{
					bool flag3 = methodId == 144 || methodId == 143 || methodId == 142 || methodId == 154;
					if (flag3)
					{
						AudioManager.Instance.PlaySound("study_xuanji", false, false);
						this.attributeView.CharacterId = this._taiwuCharId;
					}
					else
					{
						bool flag4 = methodId == 149;
						if (flag4)
						{
							this.attributeView.CharacterId = this._taiwuCharId;
						}
					}
					GameData.Domains.Taiwu.SkillBreakPlate plate = this._usingPlate1 ? this._reusePlate1 : this._reusePlate2;
					Serializer.Deserialize(pool, offset, ref plate);
					bool flag5 = plate == null;
					if (!flag5)
					{
						this._usingPlate1 = !this._usingPlate1;
						this._usingEnterPlate = false;
						bool flag6 = methodId == 53;
						if (flag6)
						{
							bool flag7 = ViewCharacterMenuSkillBreakPlate.IsGridBreakFail(plate, this._lastPlate) || ViewCharacterMenuSkillBreakPlate.IsHardStudyGrid(plate, this._lastPlate);
							if (flag7)
							{
								AudioManager.Instance.PlaySound("study_change_fail", false, false);
							}
							bool flag8 = plate.Current != SkillBreakPlateIndex.Invalid && plate.GetGridAt(plate.Current).TemplateId == 21;
							if (flag8)
							{
								AudioManager.Instance.PlaySound("study_gate", false, false);
							}
							this.CheckFinish(plate);
						}
						bool flag9 = methodId == 192;
						if (flag9)
						{
							bool flag10 = this._predictExp >= this.GetSwapCostExp(plate);
							if (flag10)
							{
								this.SwapSuccess(plate);
							}
							else
							{
								this.ExitSwapModeByAnimation(true, plate);
							}
						}
						else
						{
							this.RefreshPlateByCo(plate, null);
						}
					}
				}
			}
		}

		// Token: 0x06008F72 RID: 36722 RVA: 0x0042CDE8 File Offset: 0x0042AFE8
		private void RefreshPlateByCo(GameData.Domains.Taiwu.SkillBreakPlate plate, Action onRefreshFinish = null)
		{
			bool flag = this._oneShortParticlesCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._oneShortParticlesCoroutine);
			}
			this._oneShortParticlesCoroutine = base.StartCoroutine(this.OneShortParticlesCo(plate, onRefreshFinish));
		}

		// Token: 0x06008F73 RID: 36723 RVA: 0x0042CE26 File Offset: 0x0042B026
		private IEnumerator OneShortParticlesCo(GameData.Domains.Taiwu.SkillBreakPlate plate, Action onRefreshFinish = null)
		{
			yield return this.gridArea.RefreshOneShotParticlesCo(plate, this._lastPlate);
			this.RefreshWithPlate(plate, onRefreshFinish);
			yield break;
		}

		// Token: 0x06008F74 RID: 36724 RVA: 0x0042CE44 File Offset: 0x0042B044
		public static bool IsGridBreakFail(GameData.Domains.Taiwu.SkillBreakPlate plate, GameData.Domains.Taiwu.SkillBreakPlate lastPlate)
		{
			IReadOnlyList<SkillBreakPlateIndex> selectPath = plate.SelectPath;
			bool flag;
			if (selectPath != null && selectPath.Count > 0 && lastPlate != null)
			{
				IReadOnlyList<SkillBreakPlateIndex> selectPath2 = plate.SelectPath;
				flag = (plate.GetGridAt(selectPath2[selectPath2.Count - 1]).State == ESkillBreakGridState.Failed);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				IReadOnlyList<SkillBreakPlateIndex> selectPath3 = lastPlate.SelectPath;
				int lastLength = (selectPath3 != null) ? selectPath3.Count : 0;
				int currentLength = plate.SelectPath.Count;
				bool flag3 = currentLength > lastLength;
				if (flag3)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06008F75 RID: 36725 RVA: 0x0042CEC8 File Offset: 0x0042B0C8
		public static bool IsHardStudyGrid(GameData.Domains.Taiwu.SkillBreakPlate plate, GameData.Domains.Taiwu.SkillBreakPlate lastPlate)
		{
			IReadOnlyList<SkillBreakPlateIndex> selectPath = plate.SelectPath;
			bool result;
			if (selectPath != null && selectPath.Count > 0 && lastPlate != null && plate.Current == lastPlate.Current)
			{
				IReadOnlyList<SkillBreakPlateIndex> selectPath2 = plate.SelectPath;
				result = (plate.GetGridAt(selectPath2[selectPath2.Count - 1]).State != ESkillBreakGridState.Failed);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06008F76 RID: 36726 RVA: 0x0042CF2A File Offset: 0x0042B12A
		private unsafe IEnumerator WaitOtherDataToRefreshByEnter()
		{
			int num;
			for (int i = 0; i < 5; i = num + 1)
			{
				bool flag = this._currentExp >= 0;
				if (flag)
				{
					break;
				}
				yield return null;
				num = i;
			}
			for (int j = 0; j < 5; j = num + 1)
			{
				bool flag2 = *this._lifeSkillAttainments[0] >= 0;
				if (flag2)
				{
					break;
				}
				yield return null;
				num = j;
			}
			for (int k = 0; k < 5; k = num + 1)
			{
				bool flag3 = this._baseCostExp >= 0;
				if (flag3)
				{
					break;
				}
				yield return null;
				num = k;
			}
			this.RefreshWithPlate(this._enteredPlate, null);
			bool flag4 = this._targetAutoOpenBonusIndex != SkillBreakPlateIndex.Invalid;
			if (flag4)
			{
				base.StartCoroutine(this.AutoOpenBonusCo());
			}
			yield break;
		}

		// Token: 0x06008F77 RID: 36727 RVA: 0x0042CF3C File Offset: 0x0042B13C
		private void HandleDataModification(Notification notification, NotificationWrapper wrapper)
		{
			ushort domainId = notification.Uid.DomainId;
			uint subId = notification.Uid.SubId1;
			RawDataPool pool = wrapper.DataPool;
			int offset = notification.ValueOffset;
			ushort dataId = notification.Uid.DataId;
			bool flag = domainId == 4 && subId == 66U;
			if (flag)
			{
				Serializer.Deserialize(pool, offset, ref this._currentExp);
				this.RefreshExpLabel();
				this.RefreshStatusMarks();
			}
			else
			{
				bool flag2 = domainId == 4 && subId == 97U;
				if (flag2)
				{
					Serializer.Deserialize(pool, offset, ref this._lifeSkillAttainments);
				}
				else
				{
					bool flag3 = domainId == 4 && subId == 19U;
					if (flag3)
					{
						short health = 0;
						Serializer.Deserialize(pool, offset, ref health);
						this.CheckHealthParticles(health);
						this._health = health;
					}
					else
					{
						bool flag4 = domainId == 4 && subId == 26U;
						if (flag4)
						{
							Injuries injuries = default(Injuries);
							Serializer.Deserialize(pool, offset, ref injuries);
							this.CheckInjuryParticles(injuries);
							this._injuries = injuries;
						}
						else
						{
							bool flag5 = domainId == 5 && dataId == 68;
							if (flag5)
							{
								Serializer.Deserialize(pool, offset, ref this._canBreakOut);
								this.RefreshByCanBreakOut();
							}
						}
					}
				}
			}
		}

		// Token: 0x06008F78 RID: 36728 RVA: 0x0042D070 File Offset: 0x0042B270
		private void RefreshWithPlate(GameData.Domains.Taiwu.SkillBreakPlate plate, Action onRefreshNodeFinish = null)
		{
			this.gridArea.Refresh(plate, this._skillId, this._lifeSkillAttainments, this._currentExp, this._baseCostExp, this, this._lastPlate, onRefreshNodeFinish);
			RectTransform parent = this.gridArea.transform.parent.GetComponent<RectTransform>();
			parent.SetSize(this.gridArea.GetComponent<RectTransform>().rect.size);
			string statusAvatarSprite = string.Format("ui_charactermenu_20_tupozhuangtai_{0}", ViewCharacterMenuSkillBreakPlate.CalcBreakLevel(plate));
			this._statusAvatarImage.SetSprite(statusAvatarSprite, false, null);
			this._statusAvatarAnimationRoot.GetComponent<CImage>().SetSprite(statusAvatarSprite, false, null);
			this.maxPowerLabel.text = string.Format("{0}", plate.AddMaxPower).SetGradeColor(ViewCharacterMenuSkillBreakPlate.GetPowerGrade(this._skillId, plate.AddMaxPower));
			this.RefreshStepLabel(plate);
			this.RefreshExpLabel();
			this.RefreshStatusLabel(plate);
			this.RefreshStatusMarks();
			this.RefreshBonusLayout(plate);
			this.TryPlayStepEffect(plate);
			this.TryPlayTopPowerDiffEffect(plate);
			this.TryPlayStepNormalDiffSound(plate);
			this.RefreshStatusEffect(plate);
			this.Element.ShowAfterRefresh();
			this.RefreshPowerTips(plate, this._skillId);
			this._lastPlate = plate;
			this.RefreshHealthTips(plate);
			this.powerLevelName.text = ViewCharacterMenuSkillBreakPlate.GetBreakoutMaxPowerName(this._skillId, plate.AddMaxPower, false);
			this.RefreshStartSwapButtonVisible(plate.State == ESkillBreakPlateState.NotFinished, null);
		}

		// Token: 0x06008F79 RID: 36729 RVA: 0x0042D1F4 File Offset: 0x0042B3F4
		private void TryPlayTopPowerDiffEffect(GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			bool flag = this._lastPlate == null || this._lastPlate.AddMaxPower == plate.AddMaxPower;
			if (!flag)
			{
				this._uiParticlePlayHelper.PlayOnceParticle(this.maxPowerDiffEffect, 0.5f, null);
				AudioManager.Instance.PlaySound("study_score", false, false);
			}
		}

		// Token: 0x06008F7A RID: 36730 RVA: 0x0042D250 File Offset: 0x0042B450
		private void TryPlayStepEffect(GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			byte breakLevel = ViewCharacterMenuSkillBreakPlate.CalcBreakLevel(plate);
			bool flag = this._lastPlate == null || this._lastPlate.Current != plate.Current;
			if (flag)
			{
				GameObject effectObject = this.breakStepEffectArray[(int)breakLevel];
				bool finished = plate.Finished;
				if (finished)
				{
					this.PlayEffectLegacy(this.finishEffectArray[(int)breakLevel], this.levelEffectRoot, false, 0f, "");
				}
				else
				{
					bool flag2 = null != effectObject;
					if (flag2)
					{
						this.PlayEffectLegacy(effectObject, this.levelEffectRoot, true, 0f, "");
					}
				}
			}
		}

		// Token: 0x06008F7B RID: 36731 RVA: 0x0042D2EC File Offset: 0x0042B4EC
		private void TryPlayStepNormalDiffSound(GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			bool flag = this._lastPlate != null && this._lastPlate.StepNormal < plate.StepNormal;
			if (flag)
			{
				AudioManager.Instance.PlaySound("study_increase", false, false);
			}
		}

		// Token: 0x06008F7C RID: 36732 RVA: 0x0042D330 File Offset: 0x0042B530
		private void StartSwapSkill()
		{
			bool flag = !this._canBreakOut;
			if (!flag)
			{
				bool flag2 = !this.EnterSwapMode();
				if (flag2)
				{
					AudioManager.Instance.PlaySound("ui_fail", false, false);
				}
			}
		}

		// Token: 0x06008F7D RID: 36733 RVA: 0x0042D370 File Offset: 0x0042B570
		private bool EnterSwapMode()
		{
			bool flag = this.DisplayPlate == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.ExitSwapMode();
				this.avatarArea.SetActive(true);
				this._swapFirstSelected = SkillBreakPlateIndex.Invalid;
				this._swapCandidateWrappers.Clear();
				List<ValueTuple<SkillBreakPlateIndex, SkillBreakPlateCellWrapper>> candidates = new List<ValueTuple<SkillBreakPlateIndex, SkillBreakPlateCellWrapper>>();
				foreach (SkillBreakPlateIndex coord in this.DisplayPlate.GetIndexes())
				{
					SkillBreakPlateGrid cellData = this.DisplayPlate.GetGridAt(coord);
					bool flag2 = !ViewCharacterMenuSkillBreakPlate.CanSwapCell(cellData);
					if (!flag2)
					{
						SkillBreakPlateCellWrapper wrapper;
						bool flag3 = !this.gridArea.TryGetCellWrapper(coord, out wrapper);
						if (!flag3)
						{
							candidates.Add(new ValueTuple<SkillBreakPlateIndex, SkillBreakPlateCellWrapper>(coord, wrapper));
						}
					}
				}
				bool flag4 = candidates.Count == 0;
				if (flag4)
				{
					result = false;
				}
				else
				{
					StringBuilder orderBuilder = new StringBuilder();
					for (int i = 0; i < candidates.Count; i++)
					{
						SkillBreakPlateIndex coord2 = candidates[i].Item1;
						bool flag5 = i > 0;
						if (flag5)
						{
							orderBuilder.Append(",");
						}
						orderBuilder.Append('(').Append(coord2.X).Append(',').Append(coord2.Y).Append(')');
					}
					this.swapSkillArea.gameObject.SetActive(true);
					this.swapBlock.SetActive(true);
					this._scaleAnimationRoot.GetComponent<RectMask2D>().padding = new Vector4(0f, 0f, 0f, 0f);
					this.gridArea.UseLineFlowEffect = true;
					this.effSwitchToSkill.transform.parent = this.fadeInTrans;
					this.effSwitchToBreak.transform.parent = this.fadeBackGroudTrans;
					this.matPurpleBg.SetFloat("_DissolveAmount", 1f);
					Sequence seqBackGround = DOTween.Sequence();
					seqBackGround.AppendCallback(delegate
					{
						this.cover.gameObject.SetActive(true);
					});
					seqBackGround.AppendCallback(delegate
					{
						this.effSwitchToSkill.gameObject.SetActive(true);
					});
					seqBackGround.AppendCallback(delegate
					{
						this.effSwitchToSkill.Play();
					});
					seqBackGround.AppendCallback(new TweenCallback(this.PlayAreaMaskTransitionToSwap));
					seqBackGround.AppendInterval(this.areaMaskFadeDurationPhase1);
					seqBackGround.AppendCallback(delegate
					{
						this.matPurpleBg.DOFloat(-0.1f, "_DissolveAmount", this.areaMaskFadeDurationPhase2);
					});
					seqBackGround.AppendInterval(this.areaMaskFadeDurationPhase2);
					seqBackGround.AppendCallback(delegate
					{
						this.cover.gameObject.SetActive(false);
					});
					seqBackGround.AppendCallback(delegate
					{
						this.RefreshStartSwapButtonVisible(true, null);
					});
					seqBackGround.Play<Sequence>();
					CanvasGroup leftGroup = this.leftPanel.GetComponent<CanvasGroup>();
					CanvasGroup rightGroup = this.rightPanel.GetComponent<CanvasGroup>();
					CanvasGroup avatarGroup = this.avatarArea.GetComponent<CanvasGroup>();
					Sequence seqPanelMove = DOTween.Sequence();
					seqPanelMove.AppendCallback(delegate
					{
						leftGroup.DOFade(0f, this.swapModeFadeDuration);
					});
					seqPanelMove.AppendCallback(delegate
					{
						rightGroup.DOFade(0f, this.swapModeFadeDuration);
					});
					seqPanelMove.AppendCallback(delegate
					{
						avatarGroup.DOFade(1f, this.swapModeFadeDuration);
					});
					seqPanelMove.AppendCallback(delegate
					{
						this.leftPanel.DOMove(this.leftElementFadeOutPoint.position, this.swapModeFadeDuration, false).SetEase(Ease.OutExpo);
					});
					seqPanelMove.AppendCallback(delegate
					{
						this.RefreshUpAreaMoveToSwapSkillPosition();
					});
					seqPanelMove.AppendCallback(delegate
					{
						this.rightPanel.DOMove(this.rightElementFadeOutPoint.position, this.swapModeFadeDuration, false).SetEase(Ease.OutExpo);
					});
					seqPanelMove.AppendCallback(delegate
					{
						this.avatarArea.GetComponent<RectTransform>().DOMove(this.avatarElementFadeInPoint.position, this.swapModeFadeDuration, false).SetEase(Ease.OutExpo);
					});
					seqPanelMove.AppendInterval(this.swapModeFadeDuration);
					seqPanelMove.AppendCallback(new TweenCallback(this.PlayCharacterBgEffect));
					seqPanelMove.AppendCallback(delegate
					{
						this.uiRectDragMove.AdjustPadding.left = this.movePaddingDefault;
						this.uiRectDragMove.AdjustPadding.right = this.movePaddingDefault;
					});
					seqPanelMove.Play<Sequence>();
					foreach (ValueTuple<SkillBreakPlateIndex, SkillBreakPlateCellWrapper> entry in candidates)
					{
						int key = ViewCharacterMenuSkillBreakPlate.EncodeSwapKey(entry.Item1);
						SkillBreakPlateCellWrapper wrapper2 = entry.Item2;
						this._swapCandidateWrappers[key] = wrapper2;
						wrapper2.SetSwapPresentation(true, false);
					}
					GameData.Domains.Taiwu.SkillBreakPlate plate = this._usingEnterPlate ? this._enteredPlate : (this._usingPlate1 ? this._reusePlate2 : this._reusePlate1);
					int costExp = this.GetSwapCostExp(plate);
					this._predictExp = this._currentExp - costExp;
					bool isEnough = this._currentExp >= costExp;
					this.swapExpLabel.text = string.Format("{0}/{1}", this._currentExp.ToString().SetColor(isEnough ? "brightblue" : "brightred"), costExp);
					this.RefreshStartSwapButtonVisible(plate.State == ESkillBreakPlateState.NotFinished, null);
					this.openInjuryPanel.gameObject.SetActive(false);
					this.openLeftPanel.gameObject.SetActive(false);
					this.effSwapStarBg.SetActive(true);
					this.attributeCanvasGroup.blocksRaycasts = false;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06008F7E RID: 36734 RVA: 0x0042D894 File Offset: 0x0042BA94
		private void RefreshUpAreaMove()
		{
			this._upArea.transform.DOKill(false);
			bool flag = this._isLeftPanelOpen && this._isRightPanelOpen;
			RectTransform targetPosition;
			if (flag)
			{
				targetPosition = this.upAreaPositionTargetBothOpen;
			}
			else
			{
				bool flag2 = this._isLeftPanelOpen && !this._isRightPanelOpen;
				if (flag2)
				{
					targetPosition = this.upAreaPositionTargetRightClose;
				}
				else
				{
					bool flag3 = !this._isLeftPanelOpen && this._isRightPanelOpen;
					if (flag3)
					{
						targetPosition = this.upAreaPositionTargetLeftClose;
					}
					else
					{
						targetPosition = this.upAreaPositionTargetBothClose;
					}
				}
			}
			this._upArea.transform.DOMove(targetPosition.position, 0.5f, false).SetEase(Ease.OutExpo);
		}

		// Token: 0x06008F7F RID: 36735 RVA: 0x0042D944 File Offset: 0x0042BB44
		private void RefreshUpAreaMoveToSwapSkillPosition()
		{
			this._upArea.transform.DOKill(false);
			this._upArea.transform.DOMove(this.upAreaPositionTargetBothClose.position, 0.5f, false).SetEase(Ease.OutExpo);
		}

		// Token: 0x06008F80 RID: 36736 RVA: 0x0042D984 File Offset: 0x0042BB84
		private int GetSwapCostExp(GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			bool flag = this._skillBreakExtraData == null || plate == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				result = ((int)GlobalConfig.Instance.SwapSkillBreakCostExp + plate.SwapCount) * this._skillBreakExtraData.BaseCostExp;
			}
			return result;
		}

		// Token: 0x06008F81 RID: 36737 RVA: 0x0042D9CC File Offset: 0x0042BBCC
		private void ResetAreaMasksToNormal()
		{
			bool flag = this.normalAreaMask == null || this.swapAreaMask == null;
			if (!flag)
			{
				this.normalAreaMask.DOKill(false);
				this.swapAreaMask.DOKill(false);
				ViewCharacterMenuSkillBreakPlate.SetAreaMaskAlpha(this.normalAreaMask, 1f);
				this.normalAreaMask.gameObject.SetActive(true);
				ViewCharacterMenuSkillBreakPlate.SetAreaMaskAlpha(this.swapAreaMask, 0f);
				this.swapAreaMask.gameObject.SetActive(false);
			}
		}

		// Token: 0x06008F82 RID: 36738 RVA: 0x0042DA60 File Offset: 0x0042BC60
		private void PlayAreaMaskTransitionToSwap()
		{
			bool flag = this.normalAreaMask == null || this.swapAreaMask == null;
			if (!flag)
			{
				this.normalAreaMask.DOKill(false);
				this.swapAreaMask.DOKill(false);
				this.swapAreaMask.gameObject.SetActive(true);
				ViewCharacterMenuSkillBreakPlate.SetAreaMaskAlpha(this.swapAreaMask, 0f);
				this.normalAreaMask.DOFade(0f, this.areaMaskFadeDuration).SetEase(Ease.OutQuad).OnComplete(delegate
				{
					this.normalAreaMask.gameObject.SetActive(false);
				});
				this.swapAreaMask.DOFade(1f, this.areaMaskFadeDuration).SetEase(Ease.InQuad);
			}
		}

		// Token: 0x06008F83 RID: 36739 RVA: 0x0042DB1C File Offset: 0x0042BD1C
		private void PlayAreaMaskTransitionToNormal()
		{
			bool flag = this.normalAreaMask == null || this.swapAreaMask == null;
			if (!flag)
			{
				this.normalAreaMask.DOKill(false);
				this.swapAreaMask.DOKill(false);
				this.normalAreaMask.gameObject.SetActive(true);
				ViewCharacterMenuSkillBreakPlate.SetAreaMaskAlpha(this.normalAreaMask, 0f);
				this.swapAreaMask.DOFade(0f, this.areaMaskFadeDuration).SetEase(Ease.OutQuad).OnComplete(delegate
				{
					this.swapAreaMask.gameObject.SetActive(false);
				});
				this.normalAreaMask.DOFade(1f, this.areaMaskFadeDuration).SetEase(Ease.InQuad);
			}
		}

		// Token: 0x06008F84 RID: 36740 RVA: 0x0042DBD8 File Offset: 0x0042BDD8
		private static void SetAreaMaskAlpha(CRawImage image, float alpha)
		{
			Color color = image.color;
			color.a = alpha;
			image.color = color;
		}

		// Token: 0x06008F85 RID: 36741 RVA: 0x0042DC00 File Offset: 0x0042BE00
		private void PlayCharacterBgEffect()
		{
			this.imgEffCharacterBg.material.SetFloat("_rongjie", 1f);
			Sequence seq2 = DOTween.Sequence();
			float duration1 = this.snakeBgEffDuration1 / this.snakeBgEffFrameRate;
			float duration2 = this.snakeBgEffDuration2 / this.snakeBgEffFrameRate;
			float duration3 = this.snakeBgEffDuration3 / this.snakeBgEffFrameRate;
			float duration4 = this.snakeBgEffDuration4 / this.snakeBgEffFrameRate;
			seq2.AppendCallback(delegate
			{
				this.imgEffCharacterBg.material.DOFloat(0.2f, "_rongjie", duration1);
			});
			seq2.AppendInterval(duration1);
			seq2.AppendCallback(delegate
			{
				this.imgEffCharacterBg.material.DOFloat(-0.02f, "_rongjie", duration2);
			});
			seq2.AppendInterval(duration2);
			seq2.AppendCallback(delegate
			{
				this.imgEffCharacterBg.material.DOFloat(-0.05f, "_rongjie", duration3);
			});
			seq2.AppendInterval(duration3);
			seq2.AppendCallback(delegate
			{
				this.imgEffCharacterBg.material.DOFloat(-0.1f, "_rongjie", duration4);
			});
			seq2.Play<Sequence>();
		}

		// Token: 0x06008F86 RID: 36742 RVA: 0x0042DD04 File Offset: 0x0042BF04
		private void PlayCharacterBgEffectHide()
		{
			this.imgEffCharacterBg.material.SetFloat("_rongjie", -1f);
			Sequence seq2 = DOTween.Sequence();
			float duration1 = this.snakeBgEffDuration1 / this.snakeBgEffFrameRate;
			float duration2 = this.snakeBgEffDuration2 / this.snakeBgEffFrameRate;
			float duration3 = this.snakeBgEffDuration3 / this.snakeBgEffFrameRate;
			float duration4 = this.snakeBgEffDuration4 / this.snakeBgEffFrameRate;
			seq2.AppendCallback(delegate
			{
				this.imgEffCharacterBg.material.DOFloat(-0.1f, "_rongjie", duration4);
			});
			seq2.AppendInterval(duration4);
			seq2.AppendCallback(delegate
			{
				this.imgEffCharacterBg.material.DOFloat(-0.05f, "_rongjie", duration3);
			});
			seq2.AppendInterval(duration3);
			seq2.AppendCallback(delegate
			{
				this.imgEffCharacterBg.material.DOFloat(-0.02f, "_rongjie", duration2);
			});
			seq2.AppendInterval(duration2);
			seq2.AppendCallback(delegate
			{
				this.imgEffCharacterBg.material.DOFloat(0.2f, "_rongjie", duration1);
			});
			seq2.Play<Sequence>();
		}

		// Token: 0x06008F87 RID: 36743 RVA: 0x0042DE08 File Offset: 0x0042C008
		private void HandleSwapCellClick(SkillBreakPlateIndex coordinate)
		{
			int key = ViewCharacterMenuSkillBreakPlate.EncodeSwapKey(coordinate);
			Debug.Log(string.Format("test HandleSwapCellClick {0},{1}", coordinate.X, coordinate.Y));
			SkillBreakPlateCellWrapper wrapper;
			bool flag = !this._swapCandidateWrappers.TryGetValue(key, out wrapper);
			if (!flag)
			{
				bool flag2 = this._swapFirstSelected == SkillBreakPlateIndex.Invalid;
				if (flag2)
				{
					this._swapFirstSelected = coordinate;
					wrapper.SetSwapPresentation(true, true);
					this.SetSnakeAnimation("prepare", true);
				}
				else
				{
					bool flag3 = coordinate.Equals(this._swapFirstSelected);
					if (flag3)
					{
						wrapper.SetSwapPresentation(true, false);
						this._swapFirstSelected = SkillBreakPlateIndex.Invalid;
						this.SetSnakeAnimation("idle", true);
					}
					else
					{
						this.RequestSkillSwap(this._swapFirstSelected, coordinate);
					}
				}
			}
		}

		// Token: 0x06008F88 RID: 36744 RVA: 0x0042DED8 File Offset: 0x0042C0D8
		private void RequestSkillSwap(SkillBreakPlateIndex first, SkillBreakPlateIndex second)
		{
			int key = ViewCharacterMenuSkillBreakPlate.EncodeSwapKey(first);
			int key2 = ViewCharacterMenuSkillBreakPlate.EncodeSwapKey(second);
			Debug.Log(string.Format("test RequestSkillSwap {0},{1};; {2},{3}", new object[]
			{
				first.X,
				first.Y,
				second.X,
				second.Y
			}));
			TaiwuDomainMethod.Call.SwapSkillBreakGrid(this.Element.GameDataListenerId, this._skillId, first, second);
		}

		// Token: 0x06008F89 RID: 36745 RVA: 0x0042DF64 File Offset: 0x0042C164
		private void ExitSwapMode()
		{
			this.ExitSwapModeLogic();
			this.ResetAreaMasksToNormal();
			this.swapSkillArea.gameObject.SetActive(false);
			this.swapBlock.SetActive(false);
			this._scaleAnimationRoot.GetComponent<RectMask2D>().padding.Set(0f, 0f, 0f, 0f);
			this.SetLeftPanelOpen(this._leftElementOpen, false);
			this.SetInjuryPanelOpen(this._rightElementOpen, false);
			this.gridArea.UseLineFlowEffect = false;
			this._scaleAnimationRoot.GetComponent<RectMask2D>().padding = new Vector4(0f, 0f, 0f, 0f);
			this.effSwapStarBg.SetActive(false);
			this.attributeCanvasGroup.blocksRaycasts = true;
		}

		// Token: 0x06008F8A RID: 36746 RVA: 0x0042E03C File Offset: 0x0042C23C
		private void ExitSwapModeLogic()
		{
			bool flag = this._swapCandidateWrappers.Count > 0;
			if (flag)
			{
				foreach (SkillBreakPlateCellWrapper wrapper in this._swapCandidateWrappers.Values)
				{
					if (wrapper != null)
					{
						wrapper.SetSwapPresentation(false, false);
					}
				}
			}
			this._swapCandidateWrappers.Clear();
			this._swapFirstSelected = SkillBreakPlateIndex.Invalid;
		}

		// Token: 0x06008F8B RID: 36747 RVA: 0x0042E0CC File Offset: 0x0042C2CC
		private void ExitSwapModeByAnimation(bool playSnakeAttackAnimation, GameData.Domains.Taiwu.SkillBreakPlate plate = null)
		{
			this.swapSkillArea.ReturnCellsBack();
			bool flag = plate != null;
			if (flag)
			{
				this.RefreshPlateByCo(plate, null);
			}
			this.SetLeftPanelOpen(this._leftElementOpen, false);
			this.SetInjuryPanelOpen(this._rightElementOpen, false);
			Sequence seq = DOTween.Sequence();
			seq.AppendCallback(delegate
			{
				this.cover.gameObject.SetActive(true);
			});
			CanvasGroup leftGroup = this.leftPanel.GetComponent<CanvasGroup>();
			CanvasGroup rightGroup = this.rightPanel.GetComponent<CanvasGroup>();
			CanvasGroup avatarGroup = this.avatarArea.GetComponent<CanvasGroup>();
			Sequence seqPanelMove = DOTween.Sequence();
			seqPanelMove.Pause<Sequence>();
			seqPanelMove.AppendCallback(delegate
			{
				leftGroup.DOFade(1f, this.swapModeFadeDuration);
			});
			seqPanelMove.AppendCallback(delegate
			{
				rightGroup.DOFade(1f, this.swapModeFadeDuration);
			});
			seqPanelMove.AppendCallback(delegate
			{
				avatarGroup.DOFade(0f, this.swapModeFadeDuration);
			});
			seqPanelMove.AppendCallback(new TweenCallback(this.PlayCharacterBgEffectHide));
			seqPanelMove.AppendCallback(delegate
			{
				this.leftPanel.DOMove(this.leftElementFadeInPoint.position, this.swapModeFadeDuration, false).SetEase(Ease.OutExpo);
			});
			seqPanelMove.AppendCallback(delegate
			{
				this.rightPanel.DOMove(this.rightElementFadeInPoint.position, this.swapModeFadeDuration, false).SetEase(Ease.OutExpo);
			});
			seqPanelMove.AppendCallback(delegate
			{
				this.avatarArea.GetComponent<RectTransform>().DOMove(this.avatarElementFadeOutPoint.position, this.swapModeFadeDuration, false).SetEase(Ease.OutExpo);
			});
			seqPanelMove.AppendInterval(this.swapModeFadeDuration);
			if (playSnakeAttackAnimation)
			{
				seq.AppendCallback(delegate
				{
					this.SetSnakeAnimation("attack", false);
				});
				seq.AppendInterval(1.33f);
				seq.AppendCallback(delegate
				{
					this.SetSnakeAnimation("idle", true);
				});
				seq.AppendCallback(delegate
				{
					seqPanelMove.Play<Sequence>();
				});
			}
			else
			{
				seqPanelMove.Play<Sequence>();
			}
			this.effSwitchToBreak.transform.parent = this.fadeInTrans;
			this.effSwitchToSkill.transform.parent = this.fadeBackGroudTrans;
			this.matYellowBg.SetFloat("_DissolveAmount", 1f);
			seq.AppendCallback(delegate
			{
				this.effSwitchToBreak.gameObject.SetActive(true);
			});
			seq.AppendCallback(delegate
			{
				this.effSwitchToBreak.Play();
			});
			seq.AppendCallback(new TweenCallback(this.PlayAreaMaskTransitionToNormal));
			seq.AppendInterval(this.areaMaskFadeDurationPhase1);
			seq.AppendCallback(delegate
			{
				this.matYellowBg.DOFloat(-0.1f, "_DissolveAmount", this.areaMaskFadeDurationPhase2);
			});
			seq.AppendInterval(this.areaMaskFadeDurationPhase2);
			seq.AppendCallback(new TweenCallback(this.ExitSwapMode));
			seq.AppendCallback(delegate
			{
				this.cover.gameObject.SetActive(false);
			});
			seq.AppendCallback(delegate
			{
				this.effSwitchToSkill.gameObject.SetActive(false);
			});
			seq.AppendCallback(delegate
			{
				this.mainBg.gameObject.SetActive(false);
			});
			seq.AppendCallback(delegate
			{
				this.RefreshStartSwapButtonVisible(true, null);
			});
			bool flag2 = plate != null;
			if (flag2)
			{
				seq.AppendCallback(delegate
				{
					this.startSwapButton.interactable = (this.startSwapButton.interactable && this._predictExp >= this.GetSwapCostExp(plate));
				});
			}
			seq.Play<Sequence>();
		}

		// Token: 0x06008F8C RID: 36748 RVA: 0x0042E3E0 File Offset: 0x0042C5E0
		private void SwapSuccess(GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			bool flag = plate == null;
			if (!flag)
			{
				this.swapSkillArea.ReturnCellsBack();
				this.cover.gameObject.SetActive(true);
				this.ExitSwapModeLogic();
				this.RefreshPlateByCo(plate, delegate
				{
					this.cover.gameObject.SetActive(false);
					base.StartCoroutine(this.EnterSwapModeLogic());
				});
				Sequence seq = DOTween.Sequence();
				seq.AppendCallback(delegate
				{
					this.SetSnakeAnimation("attack", false);
				});
				seq.AppendInterval(1.33f);
				seq.AppendCallback(delegate
				{
					this.SetSnakeAnimation("idle", true);
				});
				seq.Play<Sequence>();
			}
		}

		// Token: 0x06008F8D RID: 36749 RVA: 0x0042E470 File Offset: 0x0042C670
		private IEnumerator EnterSwapModeLogic()
		{
			bool flag = this.DisplayPlate == null;
			if (flag)
			{
				yield break;
			}
			yield return new WaitForEndOfFrame();
			this._swapFirstSelected = SkillBreakPlateIndex.Invalid;
			this._swapCandidateWrappers.Clear();
			List<ValueTuple<SkillBreakPlateIndex, SkillBreakPlateCellWrapper>> candidates = new List<ValueTuple<SkillBreakPlateIndex, SkillBreakPlateCellWrapper>>();
			foreach (SkillBreakPlateIndex coord in this.DisplayPlate.GetIndexes())
			{
				SkillBreakPlateGrid cellData = this.DisplayPlate.GetGridAt(coord);
				bool flag2 = !ViewCharacterMenuSkillBreakPlate.CanSwapCell(cellData);
				if (!flag2)
				{
					SkillBreakPlateCellWrapper wrapper;
					bool flag3 = !this.gridArea.TryGetCellWrapper(coord, out wrapper);
					if (!flag3)
					{
						candidates.Add(new ValueTuple<SkillBreakPlateIndex, SkillBreakPlateCellWrapper>(coord, wrapper));
						cellData = null;
						wrapper = null;
						coord = default(SkillBreakPlateIndex);
					}
				}
			}
			IEnumerator<SkillBreakPlateIndex> enumerator = null;
			bool flag4 = candidates.Count == 0;
			if (flag4)
			{
				yield break;
			}
			StringBuilder orderBuilder = new StringBuilder();
			int num;
			for (int i = 0; i < candidates.Count; i = num + 1)
			{
				SkillBreakPlateIndex coord2 = candidates[i].Item1;
				bool flag5 = i > 0;
				if (flag5)
				{
					orderBuilder.Append(",");
				}
				orderBuilder.Append('(').Append(coord2.X).Append(',').Append(coord2.Y).Append(')');
				coord2 = default(SkillBreakPlateIndex);
				num = i;
			}
			this.swapBlock.SetActive(true);
			foreach (ValueTuple<SkillBreakPlateIndex, SkillBreakPlateCellWrapper> entry in candidates)
			{
				int key = ViewCharacterMenuSkillBreakPlate.EncodeSwapKey(entry.Item1);
				SkillBreakPlateCellWrapper wrapper2 = entry.Item2;
				this._swapCandidateWrappers[key] = wrapper2;
				wrapper2.SetSwapPresentation(true, false);
				wrapper2 = null;
				entry = default(ValueTuple<SkillBreakPlateIndex, SkillBreakPlateCellWrapper>);
			}
			List<ValueTuple<SkillBreakPlateIndex, SkillBreakPlateCellWrapper>>.Enumerator enumerator2 = default(List<ValueTuple<SkillBreakPlateIndex, SkillBreakPlateCellWrapper>>.Enumerator);
			GameData.Domains.Taiwu.SkillBreakPlate plate = this._usingEnterPlate ? this._enteredPlate : (this._usingPlate1 ? this._reusePlate2 : this._reusePlate1);
			int costExp = this.GetSwapCostExp(plate);
			this._predictExp = this._currentExp - costExp;
			bool isEnough = this._currentExp >= costExp;
			this.swapExpLabel.text = string.Format("{0}/{1}", this._currentExp.ToString().SetColor(isEnough ? "brightblue" : "brightred"), costExp);
			yield break;
		}

		// Token: 0x06008F8E RID: 36750 RVA: 0x0042E480 File Offset: 0x0042C680
		private static bool CanSwapCell(SkillBreakPlateGrid cellData)
		{
			bool flag = cellData == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				ESkillBreakGridState state = cellData.State;
				bool flag2 = state != ESkillBreakGridState.CanSelect && state != ESkillBreakGridState.Showed;
				if (flag2)
				{
					result = false;
				}
				else
				{
					ESkillBreakGridTypeType type = cellData.Template.Type;
					result = (type != ESkillBreakGridTypeType.StartPoint && type != ESkillBreakGridTypeType.EndPoint);
				}
			}
			return result;
		}

		// Token: 0x06008F8F RID: 36751 RVA: 0x0042E4D8 File Offset: 0x0042C6D8
		private static int EncodeSwapKey(SkillBreakPlateIndex coordinate)
		{
			int x = coordinate.X & 65535;
			int y = coordinate.Y & 65535;
			return y << 16 | x;
		}

		// Token: 0x06008F90 RID: 36752 RVA: 0x0042E50C File Offset: 0x0042C70C
		private void RefreshStartSwapButtonTips()
		{
			TooltipInvoker tip = this.startSwapButton.GetComponent<TooltipInvoker>();
			SkillBreakStartSwapButtonTipsHelper.RefreshStartSwapButtonTips(tip, this.DisplayPlate, this._currentExp, this._skillId);
		}

		// Token: 0x06008F91 RID: 36753 RVA: 0x0042E540 File Offset: 0x0042C740
		private void RefreshStartSwapButtonVisible(bool notFinished = false, GameData.Domains.Taiwu.SkillBreakPlate plate = null)
		{
			bool expEnough = plate == null || this._currentExp >= this.GetSwapCostExp(plate);
			this.startSwapButton.gameObject.SetActive(this._swapFunctionUnlocked && !this.SwapModeActive);
			this.startSwapButton.interactable = (!this._isReview && notFinished && expEnough);
		}

		// Token: 0x06008F92 RID: 36754 RVA: 0x0042E5A8 File Offset: 0x0042C7A8
		private void RefreshStatusEffect(GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			byte breakLevel = ViewCharacterMenuSkillBreakPlate.CalcBreakLevel(plate);
			this.PlayEffectLegacy(this.statusEffectArray[(int)breakLevel], this.levelEffectRoot, false, 0f, "LevelEffect");
		}

		// Token: 0x06008F93 RID: 36755 RVA: 0x0042E5E0 File Offset: 0x0042C7E0
		private void CheckHealthParticles(short health)
		{
			bool flag = health < this._health && this.attributeView.isActiveAndEnabled;
			if (flag)
			{
				base.DelayCall(delegate
				{
					this._uiParticlePlayHelper.PlayOnceParticle(this.healthParticle, 1f, null);
					AudioManager.Instance.PlaySound("study_hurt", false, false);
				}, 0.3f);
				base.DelayFrameCall(delegate
				{
					GameData.Domains.Taiwu.SkillBreakPlate displayPlate = this.DisplayPlate;
					int num;
					if (displayPlate != null)
					{
						IReadOnlyList<SkillBreakPlateIndex> selectPath = displayPlate.SelectPath;
						if (selectPath != null)
						{
							num = ((selectPath.Count > 0) ? 1 : 0);
							goto IL_2D;
						}
					}
					num = 0;
					IL_2D:
					bool flag2 = num == 0;
					if (!flag2)
					{
						IReadOnlyList<SkillBreakPlateIndex> selectPath2 = this.DisplayPlate.SelectPath;
						SkillBreakPlateIndex coord = selectPath2[selectPath2.Count - 1];
						Vector2 startPosition = this.gridArea.GetChildWorldPosition(coord.X, coord.Y);
						Vector3 targetPosition = this.healthFlyTargetPosition.transform.position;
						GameObject obj = Object.Instantiate<GameObject>(this.healthFlyEffectTemplate, this.flyEffectRoot);
						obj.gameObject.SetActive(true);
						obj.GetComponent<ParticleSystem>().Play();
						obj.transform.position = new Vector3(startPosition.x, startPosition.y, 0f);
						obj.transform.DOMove(targetPosition, 0.3f, false).OnComplete(delegate
						{
							Object.Destroy(obj.gameObject);
						});
					}
				}, 1U);
			}
		}

		// Token: 0x06008F94 RID: 36756 RVA: 0x0042E638 File Offset: 0x0042C838
		private void CheckInjuryParticles(Injuries injuries)
		{
			ViewCharacterMenuSkillBreakPlate.ENewInjuriesType newInjuriesType = this.HasNewInjuries(this._injuries, injuries);
			bool flag = newInjuriesType == ViewCharacterMenuSkillBreakPlate.ENewInjuriesType.None;
			if (!flag)
			{
				base.DelayFrameCall(delegate
				{
					GameData.Domains.Taiwu.SkillBreakPlate displayPlate = this.DisplayPlate;
					int num;
					if (displayPlate != null)
					{
						IReadOnlyList<SkillBreakPlateIndex> selectPath = displayPlate.SelectPath;
						if (selectPath != null)
						{
							num = ((selectPath.Count > 0) ? 1 : 0);
							goto IL_32;
						}
					}
					num = 0;
					IL_32:
					bool flag2 = num == 0;
					if (!flag2)
					{
						IReadOnlyList<SkillBreakPlateIndex> selectPath2 = this.DisplayPlate.SelectPath;
						SkillBreakPlateIndex coord = selectPath2[selectPath2.Count - 1];
						Vector2 startPosition = this.gridArea.GetChildWorldPosition(coord.X, coord.Y);
						Vector3 targetPosition = this.injuryFlyTargetPosition.transform.position;
						GameObject template = (newInjuriesType == ViewCharacterMenuSkillBreakPlate.ENewInjuriesType.NewInner) ? this.innerInjuryFlyEffectTemplate : this.outerInjuryFlyEffectTemplate;
						GameObject obj = Object.Instantiate<GameObject>(template, this.flyEffectRoot);
						obj.gameObject.SetActive(true);
						obj.GetComponent<ParticleSystem>().Play();
						obj.transform.position = new Vector3(startPosition.x, startPosition.y, 0f);
						obj.transform.DOMove(targetPosition, 0.3f, false).OnComplete(delegate
						{
							Object.Destroy(obj.gameObject);
						});
					}
				}, 1U);
			}
		}

		// Token: 0x06008F95 RID: 36757 RVA: 0x0042E68C File Offset: 0x0042C88C
		private ViewCharacterMenuSkillBreakPlate.ENewInjuriesType HasNewInjuries(Injuries oldInjuries, Injuries newInjuries)
		{
			sbyte part = 0;
			while (part < 7)
			{
				ValueTuple<sbyte, sbyte> valueTuple = oldInjuries.Get(part);
				sbyte oldOuter = valueTuple.Item1;
				sbyte oldInner = valueTuple.Item2;
				ValueTuple<sbyte, sbyte> valueTuple2 = newInjuries.Get(part);
				sbyte newOuter = valueTuple2.Item1;
				sbyte newInner = valueTuple2.Item2;
				bool flag = newOuter > oldOuter;
				ViewCharacterMenuSkillBreakPlate.ENewInjuriesType result;
				if (flag)
				{
					result = ViewCharacterMenuSkillBreakPlate.ENewInjuriesType.NewOuter;
				}
				else
				{
					bool flag2 = newInner > oldInner;
					if (!flag2)
					{
						part += 1;
						continue;
					}
					result = ViewCharacterMenuSkillBreakPlate.ENewInjuriesType.NewInner;
				}
				return result;
			}
			return ViewCharacterMenuSkillBreakPlate.ENewInjuriesType.None;
		}

		// Token: 0x06008F96 RID: 36758 RVA: 0x0042E700 File Offset: 0x0042C900
		private void CheckFinish(GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			bool finished = plate.Finished;
			if (finished)
			{
				this.RefreshSkillData();
			}
			bool flag = (!plate.Failed && !plate.Finished) || this._isReview;
			if (!flag)
			{
				int maxPower = plate.AddMaxPower;
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("Title", LocalStringManager.Get(LanguageKey.LK_GetItem_CombatSkillBreak));
				argBox.Set("CombatSkillId", this._skillId);
				argBox.Set("BreakResult", plate.Success);
				argBox.Set("CombatSkillMaxPower", maxPower);
				bool failed = plate.Failed;
				if (failed)
				{
					argBox.SetObject("CloseAction", new Action(this.QuickHide));
				}
				UIElement.GetItem.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.GetItem);
			}
		}

		// Token: 0x06008F97 RID: 36759 RVA: 0x0042E7D8 File Offset: 0x0042C9D8
		private void RefreshStatusLabel(GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			this._statusLabel.text = LocalStringManager.Get(string.Format("LK_Skill_Break_Status_{0}", ViewCharacterMenuSkillBreakPlate.CalcBreakLevel(plate))).SetColor(plate.GetIsInGoneMad() ? "brightred" : "pinkyellow");
		}

		// Token: 0x06008F98 RID: 36760 RVA: 0x0042E825 File Offset: 0x0042CA25
		private void RefreshByCanBreakOut()
		{
			this.RefreshStatusMarks();
		}

		// Token: 0x06008F99 RID: 36761 RVA: 0x0042E830 File Offset: 0x0042CA30
		private void RefreshStatusMarks()
		{
			this.cannotBreakOutMark.SetActive(false);
			this.expNotEnoughMark.SetActive(false);
			this.dangerMark.SetActive(false);
			bool flag = this.DisplayPlate == null;
			if (!flag)
			{
				bool flag2 = ViewCharacterMenuSkillBreakPlate.ShowCannotBreakOutMark(this._canBreakOut, this.DisplayPlate);
				if (flag2)
				{
					this.cannotBreakOutMark.SetActive(true);
				}
				else
				{
					bool flag3 = ViewCharacterMenuSkillBreakPlate.ShowDangerMark(this.DisplayPlate);
					if (flag3)
					{
						this.dangerMark.SetActive(true);
					}
				}
			}
		}

		// Token: 0x06008F9A RID: 36762 RVA: 0x0042E8B8 File Offset: 0x0042CAB8
		private void RefreshStepLabel(GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			bool isNormalStepExhausted = SkillBreakPlateUtils.IsNormalStepExhausted(plate);
			bool lastIsNormalStepExhausted = this._lastPlate != null && SkillBreakPlateUtils.IsNormalStepExhausted(this._lastPlate);
			this._smallStepTitle.text = LocalStringManager.Get(isNormalStepExhausted ? LanguageKey.LK_Skill_Break_Talent_Limit : LanguageKey.LK_Skill_Break_Max_Limit);
			string largeTitle = LocalStringManager.Get(isNormalStepExhausted ? LanguageKey.LK_Skill_Break_Max_Limit : LanguageKey.LK_Skill_Break_Talent_Limit);
			string smallTitle = LocalStringManager.Get(isNormalStepExhausted ? LanguageKey.LK_Skill_Break_Talent_Limit : LanguageKey.LK_Skill_Break_Max_Limit);
			int largeStep = isNormalStepExhausted ? plate.StepCostedGoneMad : plate.StepCostedNormal;
			int smallStep = isNormalStepExhausted ? plate.StepCostedNormal : plate.StepCostedGoneMad;
			int largeTotalStep = isNormalStepExhausted ? plate.StepGoneMad : plate.StepNormal;
			int smallTotalStep = isNormalStepExhausted ? plate.StepNormal : plate.StepGoneMad;
			this.RefreshLargeStep(largeTitle, largeStep, largeTotalStep, isNormalStepExhausted);
			this.RefreshSmallStep(smallTitle, smallStep, smallTotalStep);
			bool flag = isNormalStepExhausted != lastIsNormalStepExhausted;
			if (flag)
			{
				this._uiParticlePlayHelper.PlayOnceParticle(this._stepSwitchParticle, 0.5f, null);
			}
		}

		// Token: 0x06008F9B RID: 36763 RVA: 0x0042E9B8 File Offset: 0x0042CBB8
		private void RefreshSmallStep(string title, int nowStep, int totalStep)
		{
			this._smallStepTitle.text = title;
			this._smallStepLabel.text = CommonUtils.GetColoredStringByCompare(nowStep, totalStep, nowStep.CompareTo(totalStep), false);
		}

		// Token: 0x06008F9C RID: 36764 RVA: 0x0042E9E4 File Offset: 0x0042CBE4
		private void RefreshLargeStep(string title, int nowStep, int totalStep, bool isStepOver)
		{
			bool flag = !this._triggerHalfStep;
			if (flag)
			{
				int half = (totalStep + 1) / 2;
				bool flag2 = nowStep >= half;
				if (flag2)
				{
					GlobalDomainMethod.Call.InvokeGuidingTrigger(316);
				}
			}
			this.largeStepTitle.text = title;
			int startChar = isStepOver ? 97 : ((nowStep == totalStep) ? 48 : ((nowStep < totalStep) ? 107 : 97));
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			sb.Clear();
			string nowStepStr = nowStep.ToString();
			foreach (char t in nowStepStr)
			{
				sb.Append((char)(startChar + (int)(t - '0')));
			}
			sb.Append('/');
			string totalStepStr = totalStep.ToString();
			foreach (char t2 in totalStepStr)
			{
				sb.Append('0' + (t2 - '0'));
			}
			this.largeStepLabel.Refresh(sb.ToString());
			EasyPool.Free<StringBuilder>(sb);
		}

		// Token: 0x06008F9D RID: 36765 RVA: 0x0042EAFC File Offset: 0x0042CCFC
		private void RefreshExpLabel()
		{
			bool flag = this.DisplayPlate == null;
			if (!flag)
			{
				this.expLabel.text = this._currentExp.ToString();
			}
		}

		// Token: 0x06008F9E RID: 36766 RVA: 0x0042EB30 File Offset: 0x0042CD30
		private void RefreshCombatSkillInfo()
		{
			CombatSkillItem skillConfig = CombatSkill.Instance.GetItem(this._skillId);
			this._practiceTypeLabel.text = LocalStringManager.Get(string.Format("LK_CombatSkill_Practice_Type_{0}", skillConfig.PracticeType));
			this.RefreshWeakness(skillConfig);
			bool isInner = skillConfig.GoneMadInnerInjury;
			LanguageKey inOutKey = isInner ? LanguageKey.LK_Inner_Injury : LanguageKey.LK_Out_Injury;
			this.injuryInOutLabel.text = ViewCharacterMenuSkillBreakPlate.SetInjuryColor(LocalStringManager.Get(inOutKey), isInner);
			this.injuryValueLabel.text = ViewCharacterMenuSkillBreakPlate.SetInjuryColor(string.Format("+{0}", skillConfig.GoneMadInjuryValue), isInner);
		}

		// Token: 0x06008F9F RID: 36767 RVA: 0x0042EBD4 File Offset: 0x0042CDD4
		private void RefreshWeakness(CombatSkillItem skillConfig)
		{
			int count = skillConfig.GoneMadInjuredPart.Count;
			CommonUtils.PrepareEnoughChildren(this.injuryPartLayout.transform, this.injuryIconTemplate.gameObject, count, null);
			for (int i = 0; i < count; i++)
			{
				CImage iconImage = this.injuryPartLayout.GetChild(i).GetComponent<CImage>();
				sbyte bodyPart = skillConfig.GoneMadInjuredPart[i];
				BodyPartItem config = BodyPart.Instance[bodyPart];
				iconImage.SetSprite(skillConfig.GoneMadInnerInjury ? config.InnerInjuryIcon : config.OuterInjuryIcon, false, null);
			}
		}

		// Token: 0x06008FA0 RID: 36768 RVA: 0x0042EC78 File Offset: 0x0042CE78
		private static string SetInjuryColor(string text, bool isInner)
		{
			return text.SetColor(isInner ? "innerinjury" : "outterinjury");
		}

		// Token: 0x06008FA1 RID: 36769 RVA: 0x0042ECA0 File Offset: 0x0042CEA0
		private void PlayEffectLegacy(GameObject effectObj, Transform parent, bool autoDestroy, float fixTime = 0f, string fixName = "")
		{
			bool flag = null == parent || null == effectObj;
			if (!flag)
			{
				bool flag2 = !string.IsNullOrEmpty(fixName);
				Transform instanceTransform;
				if (flag2)
				{
					instanceTransform = parent.Find(fixName);
					bool flag3 = null != instanceTransform;
					if (flag3)
					{
						Object.Destroy(instanceTransform.gameObject);
					}
				}
				instanceTransform = Object.Instantiate<GameObject>(effectObj, parent, false).GetComponent<Transform>();
				this.DisableRaycastTarget(instanceTransform);
				instanceTransform.localPosition = Vector3.zero;
				bool flag4 = !string.IsNullOrEmpty(fixName);
				if (flag4)
				{
					instanceTransform.name = fixName;
				}
				instanceTransform.gameObject.SetActive(true);
				if (autoDestroy)
				{
					float time = (fixTime > 0f) ? fixTime : effectObj.GetComponent<ParticleSystem>().main.duration;
					SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(time, delegate
					{
						bool flag5 = null != instanceTransform;
						if (flag5)
						{
							Object.Destroy(instanceTransform.gameObject);
						}
					});
				}
			}
		}

		// Token: 0x06008FA2 RID: 36770 RVA: 0x0042EDB8 File Offset: 0x0042CFB8
		private void DisableRaycastTarget(Transform trans)
		{
			Image[] images = trans.GetComponentsInChildren<Image>();
			foreach (Image image in images)
			{
				image.raycastTarget = false;
			}
		}

		// Token: 0x06008FA3 RID: 36771 RVA: 0x0042EDEC File Offset: 0x0042CFEC
		private void RefreshBonusLayout(GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			List<SkillBreakPlateBonus> bonusList = EasyPool.Get<List<SkillBreakPlateBonus>>();
			foreach (SkillBreakPlateIndex coordinate in plate.GetIndexes())
			{
				SkillBreakPlateBonus bonus = plate.GetBonus(coordinate);
				bool flag = bonus.Type == ESkillBreakPlateBonusType.None;
				if (!flag)
				{
					bonusList.Add(bonus);
				}
			}
			this.bonusLayout.Refresh(bonusList, this._skillId, this._lifeSkillAttainments, this);
			EasyPool.Free<List<SkillBreakPlateBonus>>(bonusList);
		}

		// Token: 0x06008FA4 RID: 36772 RVA: 0x0042EE80 File Offset: 0x0042D080
		private void RefreshPowerTips(GameData.Domains.Taiwu.SkillBreakPlate plate, short skillId)
		{
			if (this._maxPowerReuseTipDataDesc == null)
			{
				this._maxPowerReuseTipDataDesc = new GeneralLineData
				{
					Type = 11,
					Args = new List<string>
					{
						LocalStringManager.Get(LanguageKey.LK_Skill_Break_Power_Tip_Desc)
					}
				};
			}
			if (this._maxPowerReuseTipDataPowerLine == null)
			{
				this._maxPowerReuseTipDataPowerLine = new GeneralLineData
				{
					Type = 5,
					Args = new List<string>
					{
						LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_Power_Tip_Power_Line, 0)
					}
				};
			}
			this._maxPowerReuseTipDataPowerLine.Args[0] = LocalStringManager.GetFormat(LanguageKey.LK_Skill_Break_Power_Tip_Power_Line, plate.AddMaxPower) + ViewCharacterMenuSkillBreakPlate.GetBreakoutMaxPowerName(skillId, plate.AddMaxPower, true);
			TooltipInvoker tooltipInvoker = this.powerTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.powerTip.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Skill_Break_Power_Tip_Title)).SetObject("LineData1", this._maxPowerReuseTipDataDesc).SetObject("LineData2", this._maxPowerReuseTipDataPowerLine).Set("LineCount", 2);
		}

		// Token: 0x06008FA5 RID: 36773 RVA: 0x0042EFA4 File Offset: 0x0042D1A4
		private void RefreshStepTips()
		{
			CombatSkillItem configData = CombatSkill.Instance[this._skillId];
			string qualificationTypeName = this._skillBreakExtraData.IsCombatSkillQualification ? Config.CombatSkillType.Instance[configData.Type].Name : Config.LifeSkillType.Instance[configData.Type].Name;
			string qualificationTypeIcon = this._skillBreakExtraData.IsCombatSkillQualification ? ("sp_18_iconwuxuezhanshi_" + configData.Type.ToString()) : ("mousetip_jiyi_" + Config.LifeSkillType.Instance[configData.Type].TemplateId.ToString());
			string currQualificationText = this._skillBreakExtraData.CurrQualification.ToString().SetColor((this._skillBreakExtraData.CurrQualification > this._skillBreakExtraData.RequireQualification) ? "brightblue" : "brightred");
			string qualificationRequireValue = string.Format("{0}/{1}", currQualificationText, this._skillBreakExtraData.RequireQualification).ColorReplace();
			ViewCharacterMenuSkillBreakPlate.CommonRefreshStepTips(this._stepTip, qualificationTypeIcon, qualificationTypeName, qualificationRequireValue);
		}

		// Token: 0x06008FA6 RID: 36774 RVA: 0x0042F0B4 File Offset: 0x0042D2B4
		public static void CommonRefreshStepTips(TooltipInvoker tip, string qualificationTypeIcon, string qualificationTypeName, string qualificationRequireValue)
		{
			List<object> extraArgs = new List<object>
			{
				20
			};
			GeneralLineData desc1Line = new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Skill_Break_Step_Tip_Desc_1)
				}
			};
			GeneralLineData subTitleQualificationLine = new GeneralLineData
			{
				Type = 1,
				Args = new List<string>
				{
					LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Symbol, LocalStringManager.Get(LanguageKey.LK_Skill_Break_Step_Tip_Title))
				}
			};
			GeneralLineData desc2Line = new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					string.Concat(new string[]
					{
						LocalStringManager.Get(LanguageKey.LK_Break_Require_Qualification),
						LocalStringManager.Get(LanguageKey.LK_Colon_Symbol),
						"<SpName=",
						qualificationTypeIcon,
						">",
						qualificationTypeName,
						"  ",
						qualificationRequireValue
					}),
					"true"
				},
				ExtraArgs = extraArgs
			};
			GeneralLineData desc3Line = new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Skill_Break_Step_Tip_Desc_5)
				},
				ExtraArgs = extraArgs
			};
			GeneralLineData subTitleMadLine = new GeneralLineData
			{
				Type = 1,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Skill_Break_Step_Tip_SubTitle_1)
				}
			};
			GeneralLineData desc4Line = new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Skill_Break_Step_Tip_Desc_2)
				},
				ExtraArgs = extraArgs
			};
			GeneralLineData desc5Line = new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Skill_Break_Step_Tip_Desc_3)
				},
				ExtraArgs = extraArgs
			};
			GeneralLineData desc6Line = new GeneralLineData
			{
				Type = 11,
				Args = new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Skill_Break_Step_Tip_Desc_4)
				},
				ExtraArgs = extraArgs
			};
			GeneralLineData space = new GeneralLineData
			{
				Type = 4,
				PreferredHeight = 15f
			};
			if (tip.RuntimeParam == null)
			{
				tip.RuntimeParam = new ArgumentBox();
			}
			tip.Type = TipType.GeneralLines;
			tip.RuntimeParam.Set("Title", LocalStringManager.Get(LanguageKey.LK_Skill_Break_Step_Tip_Title)).SetObject("LineData1", desc1Line).SetObject("LineData2", space).SetObject("LineData3", subTitleQualificationLine).SetObject("LineData4", desc2Line).SetObject("LineData5", desc3Line).SetObject("LineData6", subTitleMadLine).SetObject("LineData7", desc4Line).SetObject("LineData8", desc5Line).SetObject("LineData9", desc6Line).Set("LineCount", 9);
		}

		// Token: 0x06008FA7 RID: 36775 RVA: 0x0042F37C File Offset: 0x0042D57C
		private void RefreshHealthTips(GameData.Domains.Taiwu.SkillBreakPlate skillBreakPlate)
		{
			bool isNormalStepExhausted = SkillBreakPlateUtils.IsNormalStepExhausted(skillBreakPlate);
			CombatSkillItem skillConfig = CombatSkill.Instance.GetItem(this._skillId);
			bool isInner = skillConfig.GoneMadInnerInjury;
			bool overflow = false;
			for (int i = 0; i < skillConfig.GoneMadInjuredPart.Count; i++)
			{
				sbyte bodyPart = skillConfig.GoneMadInjuredPart[i];
				ValueTuple<sbyte, sbyte> injury = this._injuries.Get(bodyPart);
				bool flag = (isInner ? injury.Item2 : injury.Item1) >= 6;
				if (flag)
				{
					overflow = true;
				}
			}
			this.healthDamageTip.gameObject.SetActive(isNormalStepExhausted && overflow);
		}

		// Token: 0x17000FAF RID: 4015
		// (get) Token: 0x06008FA8 RID: 36776 RVA: 0x0042F422 File Offset: 0x0042D622
		public sbyte OutlineType
		{
			get
			{
				return CombatSkillStateHelper.GetActiveOutlinePageType(this._selectedPage);
			}
		}

		// Token: 0x17000FB0 RID: 4016
		// (get) Token: 0x06008FA9 RID: 36777 RVA: 0x0042F42F File Offset: 0x0042D62F
		public SkillBreakOutlineEffectItem OutlineConfig
		{
			get
			{
				return SkillBreakOutlineEffect.Instance[this.OutlineType];
			}
		}

		// Token: 0x06008FAA RID: 36778 RVA: 0x0042F444 File Offset: 0x0042D644
		private void RefreshOutlineTips()
		{
			string dot = LocalStringManager.Get(LanguageKey.LK_Dot_Symbol);
			string outlineName = LocalStringManager.Get(string.Format("LK_CombatSkill_First_Page_Type_{0}", this.OutlineType));
			string outlineColor = PracticeSkillPlatePageUtils.OutlinePageColorMap[(int)this.OutlineType];
			this.outlineLabel.text = (outlineName + dot).SetColor(outlineColor) + this.OutlineConfig.DescShort.SetColor("pinkyellow");
			string tipsTitle = (LocalStringManager.Get(LanguageKey.LK_CombatSkill_Book_First_Page) + dot).SetColor("pinkyellow") + outlineName.SetColor(outlineColor);
			TooltipInvoker tooltipInvoker = this.outlineTip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			this.outlineTip.RuntimeParam.Set("arg0", tipsTitle);
			this.outlineTip.RuntimeParam.Set("arg1", this.OutlineConfig.Desc);
		}

		// Token: 0x06008FAB RID: 36779 RVA: 0x0042F53C File Offset: 0x0042D73C
		private void Update()
		{
			this.gridBgMask.SetWidth(this._scaleAnimationRoot.rect.width);
			bool flag = CommonCommandKit.Space.Check(this.Element, false, true, false, true, false) && this.startSwapButton.isActiveAndEnabled && this.startSwapButton.interactable;
			if (flag)
			{
				this.StartSwapSkill();
			}
		}

		// Token: 0x06008FAC RID: 36780 RVA: 0x0042F5A8 File Offset: 0x0042D7A8
		private void OnClickCell(SkillBreakPlateIndex coordinate)
		{
			bool waitingAnimationOutFinish = this._waitingAnimationOutFinish;
			if (!waitingAnimationOutFinish)
			{
				bool waitingAnimationInFinish = this._waitingAnimationInFinish;
				if (!waitingAnimationInFinish)
				{
					bool swapModeActive = this.SwapModeActive;
					if (swapModeActive)
					{
						this.HandleSwapCellClick(coordinate);
					}
					else
					{
						this._selectedCoordinate = coordinate;
						SkillBreakPlateBonus bonus = this.DisplayPlate.GetBonus(coordinate);
						SkillBreakPlateGrid cell = this.DisplayPlate.GetGridAt(coordinate);
						bool flag = bonus.Type == ESkillBreakPlateBonusType.None && cell.State == ESkillBreakGridState.CanSelect;
						if (flag)
						{
							TaiwuDomainMethod.Call.SelectSkillBreakGrid(this.Element.GameDataListenerId, this._skillId, coordinate);
						}
						else
						{
							this.OpenBonusSelect(coordinate);
						}
					}
				}
			}
		}

		// Token: 0x06008FAD RID: 36781 RVA: 0x0042F64C File Offset: 0x0042D84C
		private void OnPointerEnterCell(SkillBreakPlateIndex coordinate)
		{
			bool waitingAnimationOutFinish = this._waitingAnimationOutFinish;
			if (!waitingAnimationOutFinish)
			{
				bool waitingAnimationInFinish = this._waitingAnimationInFinish;
				if (!waitingAnimationInFinish)
				{
					bool flag = !base.gameObject.activeInHierarchy;
					if (!flag)
					{
						bool swapModeActive = this.SwapModeActive;
						if (!swapModeActive)
						{
							SkillBreakPlateBonus bonus = this.DisplayPlate.GetBonus(coordinate);
							SkillBreakPlateGrid cell = this.DisplayPlate.GetGridAt(coordinate);
							bool flag2 = cell.State != ESkillBreakGridState.CanSelect;
							if (!flag2)
							{
								this.gridArea.RefreshHighlightSameType(coordinate, true);
							}
						}
					}
				}
			}
		}

		// Token: 0x06008FAE RID: 36782 RVA: 0x0042F6D4 File Offset: 0x0042D8D4
		private void OnPointerExitCell(SkillBreakPlateIndex coordinate)
		{
			bool waitingAnimationOutFinish = this._waitingAnimationOutFinish;
			if (!waitingAnimationOutFinish)
			{
				bool waitingAnimationInFinish = this._waitingAnimationInFinish;
				if (!waitingAnimationInFinish)
				{
					bool flag = !base.gameObject.activeInHierarchy;
					if (!flag)
					{
						bool swapModeActive = this.SwapModeActive;
						if (!swapModeActive)
						{
							this.gridArea.RefreshHighlightSameType(coordinate, false);
						}
					}
				}
			}
		}

		// Token: 0x06008FAF RID: 36783 RVA: 0x0042F728 File Offset: 0x0042D928
		private void OpenBonusSelect(SkillBreakPlateIndex coordinate)
		{
			SkillBreakPlateBonus bonus = this.DisplayPlate.GetBonus(coordinate);
			SkillBreakPlateGrid cell = this.DisplayPlate.GetGridAt(coordinate);
			sbyte templateId = cell.TemplateId;
			bool flag = templateId != 2;
			if (!flag)
			{
				List<int> powerList = EasyPool.Get<List<int>>();
				powerList.Clear();
				for (int range = 1; range < 4; range++)
				{
					powerList.Add(this.DisplayPlate.CalcAddMaxPowerAsBonus(coordinate, range));
				}
				ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("CurrentExp", this._currentExp).Set("SkillId", this._skillId).SetObject("LifeSkillAttainments", this._lifeSkillAttainments).SetObject("CurrentBonus", bonus).SetObject("PossiblePowerList", powerList).SetObject("OnConfirmItem", new Action<ItemKey, ItemSourceType>(this.OnConfirmItem)).SetObject("OnConfirmRelation", new Action<int, ushort>(this.OnConfirmRelation)).SetObject("OnConfirmExp", new Action<int>(this.OnConfirmExp)).SetObject("OnConfirmClear", new Action(this.OnConfirmClear));
				UIElement.SkillBreakBonusSelect.SetOnInitArgs(argumentBox);
				UIManager.Instance.ShowUI(UIElement.SkillBreakBonusSelect, true);
				EasyPool.Free<List<int>>(powerList);
			}
		}

		// Token: 0x06008FB0 RID: 36784 RVA: 0x0042F877 File Offset: 0x0042DA77
		private IEnumerator AutoOpenBonusCo()
		{
			while (this._waitingAnimationInFinish)
			{
				yield return null;
			}
			bool isFocusMe = UIManager.Instance.IsFocusElement(this.Element);
			bool flag = this.Element == null || !isFocusMe;
			if (flag)
			{
				this._targetAutoOpenBonusIndex = SkillBreakPlateIndex.Invalid;
				yield break;
			}
			this._selectedCoordinate = this._targetAutoOpenBonusIndex;
			this.OpenBonusSelect(this._targetAutoOpenBonusIndex);
			this._targetAutoOpenBonusIndex = SkillBreakPlateIndex.Invalid;
			yield break;
		}

		// Token: 0x06008FB1 RID: 36785 RVA: 0x0042F886 File Offset: 0x0042DA86
		private void OnConfirmItem(ItemKey itemKey, ItemSourceType itemSourceType)
		{
			TaiwuDomainMethod.Call.SetBonusItem(this.Element.GameDataListenerId, this._skillId, this._selectedCoordinate, itemKey, (sbyte)itemSourceType);
		}

		// Token: 0x06008FB2 RID: 36786 RVA: 0x0042F8A9 File Offset: 0x0042DAA9
		private void OnConfirmExp(int expLevel)
		{
			TaiwuDomainMethod.Call.SetBonusExp(this.Element.GameDataListenerId, this._skillId, this._selectedCoordinate, expLevel);
		}

		// Token: 0x06008FB3 RID: 36787 RVA: 0x0042F8CC File Offset: 0x0042DACC
		private void OnConfirmRelation(int charId, ushort relationType)
		{
			bool flag = relationType == ushort.MaxValue;
			if (flag)
			{
				TaiwuDomainMethod.Call.SetBonusFriend(this.Element.GameDataListenerId, this._skillId, this._selectedCoordinate, charId);
			}
			else
			{
				TaiwuDomainMethod.Call.SetBonusRelation(this.Element.GameDataListenerId, this._skillId, this._selectedCoordinate, charId, relationType);
			}
		}

		// Token: 0x06008FB4 RID: 36788 RVA: 0x0042F925 File Offset: 0x0042DB25
		private void OnConfirmClear()
		{
			TaiwuDomainMethod.Call.ClearBonus(this.Element.GameDataListenerId, this._skillId, this._selectedCoordinate);
		}

		// Token: 0x06008FB5 RID: 36789 RVA: 0x0042F948 File Offset: 0x0042DB48
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (a == "StartSwapButton")
			{
				this.StartSwapSkill();
			}
		}

		// Token: 0x06008FB6 RID: 36790 RVA: 0x0042F978 File Offset: 0x0042DB78
		public override void QuickHide()
		{
			bool flag = !this.Element.Ready || !this.CanQuit;
			if (!flag)
			{
				bool swapModeActive = this.SwapModeActive;
				if (swapModeActive)
				{
					this.ExitSwapModeByAnimation(false, null);
				}
				else
				{
					bool flag2 = !this._isReview && SingletonObject.getInstance<TutorialChapterModel>().InGuiding;
					if (flag2)
					{
						TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg(EventActionKey.DefValue.TutorialExitSkillBreak, EventTriggerParameter.DefValue.CombatSkillTemplateId.ArgBoxKey, (int)this._skillId);
						TaiwuEventDomainMethod.Call.SetListenerEventActionBoolArg(EventActionKey.DefValue.TutorialExitSkillBreak, EventTriggerParameter.DefValue.BreakSuccess.ArgBoxKey, this.DisplayPlate.Success);
						TaiwuEventDomainMethod.Call.TriggerListener(EventActionKey.DefValue.TutorialExitSkillBreak, false);
					}
					CombatSkillItem config = CombatSkill.Instance.GetItem(this._skillId);
					bool isReverse = CombatSkillStateHelper.GetCombatSkillDirection(this._selectedPage) == 1;
					bool flag3 = isReverse && config.SectId == 4 && this.DisplayPlate.Success && !this._isReview && !UIElement.CombatBegin.Exist;
					if (flag3)
					{
						TaiwuEventDomainMethod.Call.CloseUI("UI_SkillBreakPlate", false, (int)this._skillId);
					}
					this.gridArea.QuickHide();
					base.QuickHide();
				}
			}
		}

		// Token: 0x06008FB7 RID: 36791 RVA: 0x0042FAB2 File Offset: 0x0042DCB2
		public void OnPointerEnterStartSwapButton()
		{
			this.RefreshStartSwapButtonTips();
		}

		// Token: 0x06008FB8 RID: 36792 RVA: 0x0042FABC File Offset: 0x0042DCBC
		private void OnTopUIChanged(ArgumentBox box)
		{
			bool isFocusMe = UIManager.Instance.IsFocusElement(this.Element);
			this.levelEffectRoot.gameObject.SetActive(isFocusMe && !this._waitingAnimationInFinish);
		}

		// Token: 0x06008FB9 RID: 36793 RVA: 0x0042FAFC File Offset: 0x0042DCFC
		private static byte CalcBreakLevel(GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			return (byte)plate.GetStepState();
		}

		// Token: 0x06008FBA RID: 36794 RVA: 0x0042FB18 File Offset: 0x0042DD18
		public static bool ShowNoStepMark(GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			return plate.StepExhausted && !plate.Finished;
		}

		// Token: 0x06008FBB RID: 36795 RVA: 0x0042FB40 File Offset: 0x0042DD40
		private static bool ShowCannotBreakOutMark(bool canBreakOut, GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			return !canBreakOut && !plate.Finished;
		}

		// Token: 0x06008FBC RID: 36796 RVA: 0x0042FB64 File Offset: 0x0042DD64
		private static bool ShowDangerMark(GameData.Domains.Taiwu.SkillBreakPlate plate)
		{
			return plate.GetIsInGoneMad() && !plate.Finished;
		}

		// Token: 0x06008FBD RID: 36797 RVA: 0x0042FB8C File Offset: 0x0042DD8C
		public static string GetBreakoutMaxPowerName(short skillId, int addPowerValue, bool withBracket = true)
		{
			CombatSkillItem config = CombatSkill.Instance[skillId];
			sbyte gradeGroup = Grade.GetGroup(config.Grade);
			int[] valueArray = GlobalConfig.BreakoutMaxPowerNameValueArray[(int)gradeGroup];
			int powerGrade = 0;
			for (int i = 0; i < valueArray.Length; i++)
			{
				bool flag = addPowerValue > valueArray[i];
				if (!flag)
				{
					break;
				}
				powerGrade = i;
			}
			string powerName = LocalStringManager.Get(string.Format("LK_Skill_Break_MaxPower_Grade{0}", powerGrade)).SetGradeColor(powerGrade).ColorReplace();
			return withBracket ? LocalStringManager.GetFormat(LanguageKey.LK_Brackets_Fix, powerName) : powerName;
		}

		// Token: 0x06008FBE RID: 36798 RVA: 0x0042FC2C File Offset: 0x0042DE2C
		public static int GetPowerGrade(short skillId, int addPowerValue)
		{
			CombatSkillItem config = CombatSkill.Instance[skillId];
			sbyte gradeGroup = Grade.GetGroup(config.Grade);
			int[] valueArray = GlobalConfig.BreakoutMaxPowerNameValueArray[(int)gradeGroup];
			int powerGrade = 0;
			for (int i = 0; i < valueArray.Length; i++)
			{
				bool flag = addPowerValue > valueArray[i];
				if (!flag)
				{
					break;
				}
				powerGrade = i;
			}
			string powerName = LocalStringManager.Get(string.Format("LK_Skill_Break_MaxPower_Grade{0}", powerGrade)).SetGradeColor(powerGrade).ColorReplace();
			return powerGrade;
		}

		// Token: 0x06008FBF RID: 36799 RVA: 0x0042FCB8 File Offset: 0x0042DEB8
		private void InitRefers()
		{
			bool refersInitialized = this._refersInitialized;
			if (!refersInitialized)
			{
				this.openLeftPanel.ClearAndAddListener(delegate
				{
					this.SetLeftPanelOpen(true, true);
				});
				this.closeLeftPanel.ClearAndAddListener(delegate
				{
					this.SetLeftPanelOpen(false, true);
				});
				this.openInjuryPanel.ClearAndAddListener(delegate
				{
					this.SetInjuryPanelOpen(true, true);
				});
				this.closeInjuryPanel.ClearAndAddListener(delegate
				{
					this.SetInjuryPanelOpen(false, true);
				});
				this.buttonClose.ClearAndAddListener(delegate
				{
					bool swapModeActive = this.SwapModeActive;
					if (swapModeActive)
					{
						this.ExitSwapModeByAnimation(false, null);
					}
					this.QuickHide();
				});
				this.startSwapButton.ClearAndAddListener(delegate
				{
					this.StartSwapSkill();
				});
				this._refersInitialized = true;
			}
		}

		// Token: 0x06008FC0 RID: 36800 RVA: 0x0042FD6C File Offset: 0x0042DF6C
		private void SetInjuryPanelOpen(bool active, bool isAnim = true)
		{
			this._rightElementOpen = active;
			this.closeInjuryPanel.gameObject.SetActive(active);
			this.openInjuryPanel.gameObject.SetActive(!active);
			this._isRightPanelOpen = active;
			this.infomationAreaRightAnim.DOKill(false);
			if (isAnim)
			{
				float startPosX = active ? this.infomationAreaRightAnim.rect.width : 0f;
				float endPosX = active ? 0f : this.infomationAreaRightAnim.rect.width;
				this.infomationAreaRightAnim.anchoredPosition = new Vector2(startPosX, 0f);
				bool active2 = active;
				if (active2)
				{
					this.infomationAreaRight.gameObject.SetActive(true);
				}
				this.infomationAreaRightAnim.DOAnchorPosX(endPosX, 0.3f, false).OnComplete(delegate
				{
					this.infomationAreaRight.gameObject.SetActive(active);
				}).SetEase(Ease.OutCubic);
			}
			else
			{
				this.infomationAreaRight.gameObject.SetActive(active);
			}
			this.uiRectDragMove.AdjustPadding.right = (active ? this.movePaddingRightOpen : this.movePaddingDefault);
			this.RefreshUpAreaMove();
		}

		// Token: 0x06008FC1 RID: 36801 RVA: 0x0042FEE0 File Offset: 0x0042E0E0
		private void SetLeftPanelOpen(bool active, bool isAnim = true)
		{
			this._leftElementOpen = active;
			this.closeLeftPanel.gameObject.SetActive(active);
			this.openLeftPanel.gameObject.SetActive(!active);
			this._isLeftPanelOpen = active;
			this.infomationAreaLeftAnim.DOKill(false);
			if (isAnim)
			{
				float startPosX = active ? (-this.infomationAreaLeftAnim.rect.width) : 0f;
				float endPosX = active ? 0f : (-this.infomationAreaLeftAnim.rect.width);
				this.infomationAreaLeftAnim.anchoredPosition = new Vector2(startPosX, 0f);
				bool active2 = active;
				if (active2)
				{
					this.infomationAreaLeft.gameObject.SetActive(true);
				}
				this.infomationAreaLeftAnim.DOAnchorPosX(endPosX, 0.3f, false).OnComplete(delegate
				{
					this.infomationAreaLeft.gameObject.SetActive(active);
				}).SetEase(Ease.OutCubic);
			}
			else
			{
				this.infomationAreaLeft.gameObject.SetActive(active);
			}
			this.uiRectDragMove.AdjustPadding.left = (active ? this.movePaddingLeftOpen : this.movePaddingDefault);
			this.RefreshUpAreaMove();
		}

		// Token: 0x04006E19 RID: 28185
		[SerializeField]
		private GameObject mainBg;

		// Token: 0x04006E1A RID: 28186
		[SerializeField]
		private GameObject infomationAreaLeft;

		// Token: 0x04006E1B RID: 28187
		[SerializeField]
		private RectTransform infomationAreaLeftAnim;

		// Token: 0x04006E1C RID: 28188
		[SerializeField]
		private GameObject infomationAreaRight;

		// Token: 0x04006E1D RID: 28189
		[SerializeField]
		private RectTransform infomationAreaRightAnim;

		// Token: 0x04006E1E RID: 28190
		[SerializeField]
		private CharacterMenuCombatSkillItem skillDisplayItem;

		// Token: 0x04006E1F RID: 28191
		[SerializeField]
		private AttributeAndInjuryDynamic attributeView;

		// Token: 0x04006E20 RID: 28192
		[SerializeField]
		private GameObject avatarArea;

		// Token: 0x04006E21 RID: 28193
		[SerializeField]
		private SkillBreakPlateBonusGridLayout bonusLayout;

		// Token: 0x04006E22 RID: 28194
		[SerializeField]
		private CButton buttonClose;

		// Token: 0x04006E23 RID: 28195
		[SerializeField]
		private GameObject cannotBreakOutMark;

		// Token: 0x04006E24 RID: 28196
		[SerializeField]
		private CButton closeInjuryPanel;

		// Token: 0x04006E25 RID: 28197
		[SerializeField]
		private CButton closeLeftPanel;

		// Token: 0x04006E26 RID: 28198
		[SerializeField]
		private GameObject dangerMark;

		// Token: 0x04006E27 RID: 28199
		[SerializeField]
		private GameObject decoration;

		// Token: 0x04006E28 RID: 28200
		[SerializeField]
		private TextMeshProUGUI expLabel;

		// Token: 0x04006E29 RID: 28201
		[SerializeField]
		private GameObject expNotEnoughMark;

		// Token: 0x04006E2A RID: 28202
		[SerializeField]
		private RectTransform flyEffectRoot;

		// Token: 0x04006E2B RID: 28203
		[SerializeField]
		private SkillBreakPlateRenderer gridArea;

		// Token: 0x04006E2C RID: 28204
		[SerializeField]
		private RectTransform gridBgMask;

		// Token: 0x04006E2D RID: 28205
		[SerializeField]
		private GameObject healthDamageTip;

		// Token: 0x04006E2E RID: 28206
		[SerializeField]
		private GameObject healthFlyEffectTemplate;

		// Token: 0x04006E2F RID: 28207
		[SerializeField]
		private RectTransform healthFlyTargetPosition;

		// Token: 0x04006E30 RID: 28208
		[SerializeField]
		private UIParticle healthParticle;

		// Token: 0x04006E31 RID: 28209
		[SerializeField]
		private CanvasGroup informationArea;

		// Token: 0x04006E32 RID: 28210
		[SerializeField]
		private RectTransform injuryFlyTargetPosition;

		// Token: 0x04006E33 RID: 28211
		[SerializeField]
		private CImage injuryIconTemplate;

		// Token: 0x04006E34 RID: 28212
		[SerializeField]
		private TextMeshProUGUI injuryInOutLabel;

		// Token: 0x04006E35 RID: 28213
		[SerializeField]
		private RectTransform injuryPartLayout;

		// Token: 0x04006E36 RID: 28214
		[SerializeField]
		private TextMeshProUGUI injuryValueLabel;

		// Token: 0x04006E37 RID: 28215
		[SerializeField]
		private GameObject innerInjuryFlyEffectTemplate;

		// Token: 0x04006E38 RID: 28216
		[SerializeField]
		private CImage largeStepBg;

		// Token: 0x04006E39 RID: 28217
		[SerializeField]
		private SpriteLabel largeStepLabel;

		// Token: 0x04006E3A RID: 28218
		[SerializeField]
		private TextMeshProUGUI largeStepTitle;

		// Token: 0x04006E3B RID: 28219
		[SerializeField]
		private RectTransform leftPanel;

		// Token: 0x04006E3C RID: 28220
		[SerializeField]
		private RectTransform rightPanel;

		// Token: 0x04006E3D RID: 28221
		[SerializeField]
		private RectTransform levelEffectRoot;

		// Token: 0x04006E3E RID: 28222
		[SerializeField]
		private RectTransform lockRoot;

		// Token: 0x04006E3F RID: 28223
		[SerializeField]
		private UIParticle maxPowerDiffEffect;

		// Token: 0x04006E40 RID: 28224
		[SerializeField]
		private TextMeshProUGUI maxPowerLabel;

		// Token: 0x04006E41 RID: 28225
		[SerializeField]
		private GameObject noStepMark;

		// Token: 0x04006E42 RID: 28226
		[SerializeField]
		private CButton openInjuryPanel;

		// Token: 0x04006E43 RID: 28227
		[SerializeField]
		private CButton openLeftPanel;

		// Token: 0x04006E44 RID: 28228
		[SerializeField]
		private GameObject outerInjuryFlyEffectTemplate;

		// Token: 0x04006E45 RID: 28229
		[SerializeField]
		private TextMeshProUGUI outlineLabel;

		// Token: 0x04006E46 RID: 28230
		[SerializeField]
		private TooltipInvoker outlineTip;

		// Token: 0x04006E47 RID: 28231
		[SerializeField]
		private TooltipInvoker powerTip;

		// Token: 0x04006E48 RID: 28232
		[SerializeField]
		private TextMeshProUGUI _practiceTypeLabel;

		// Token: 0x04006E49 RID: 28233
		[SerializeField]
		private RectTransform _scaleAnimationRoot;

		// Token: 0x04006E4A RID: 28234
		[SerializeField]
		private TextMeshProUGUI _smallStepLabel;

		// Token: 0x04006E4B RID: 28235
		[SerializeField]
		private TextMeshProUGUI _smallStepTitle;

		// Token: 0x04006E4C RID: 28236
		[SerializeField]
		private CButton startSwapButton;

		// Token: 0x04006E4D RID: 28237
		[SerializeField]
		private GameObject _stateMarks;

		// Token: 0x04006E4E RID: 28238
		[SerializeField]
		private RectTransform _statusAvatarAnimationRoot;

		// Token: 0x04006E4F RID: 28239
		[SerializeField]
		private CImage _statusAvatarImage;

		// Token: 0x04006E50 RID: 28240
		[SerializeField]
		private TextMeshProUGUI _statusLabel;

		// Token: 0x04006E51 RID: 28241
		[SerializeField]
		private UIParticle _stepSwitchParticle;

		// Token: 0x04006E52 RID: 28242
		[SerializeField]
		private TooltipInvoker _stepTip;

		// Token: 0x04006E53 RID: 28243
		[SerializeField]
		private SkillBreakSwapSkillManager swapSkillArea;

		// Token: 0x04006E54 RID: 28244
		[SerializeField]
		private CanvasGroup _upArea;

		// Token: 0x04006E55 RID: 28245
		[SerializeField]
		private SkeletonGraphic blackSnakeSkeleton;

		// Token: 0x04006E56 RID: 28246
		[SerializeField]
		private UIParticle effSwitchToSkill;

		// Token: 0x04006E57 RID: 28247
		[SerializeField]
		private UIParticle effSwitchToBreak;

		// Token: 0x04006E58 RID: 28248
		[SerializeField]
		private UIParticle effCharacterBg;

		// Token: 0x04006E59 RID: 28249
		[SerializeField]
		private Image imgEffCharacterBg;

		// Token: 0x04006E5A RID: 28250
		[SerializeField]
		private TextMeshProUGUI swapExpLabel;

		// Token: 0x04006E5B RID: 28251
		[SerializeField]
		private GameObject cover;

		// Token: 0x04006E5C RID: 28252
		[SerializeField]
		private Transform fadeInTrans;

		// Token: 0x04006E5D RID: 28253
		[SerializeField]
		private Transform fadeBackGroudTrans;

		// Token: 0x04006E5E RID: 28254
		[SerializeField]
		private Image purpleBgImage;

		// Token: 0x04006E5F RID: 28255
		[SerializeField]
		private Image yellowBgImage;

		// Token: 0x04006E60 RID: 28256
		[SerializeField]
		private TextMeshProUGUI powerLevelName;

		// Token: 0x04006E61 RID: 28257
		[SerializeField]
		private Transform ScaleRootReference;

		// Token: 0x04006E62 RID: 28258
		[SerializeField]
		private TMPTextSpriteHelper hintTextSprite;

		// Token: 0x04006E63 RID: 28259
		[SerializeField]
		private GameObject swapBlock;

		// Token: 0x04006E64 RID: 28260
		[SerializeField]
		private GameObject effSwapStarBg;

		// Token: 0x04006E65 RID: 28261
		[SerializeField]
		private MouseWheelScaleCustom mouseWheelScaleCustom;

		// Token: 0x04006E66 RID: 28262
		[SerializeField]
		private UIRectDragMove uiRectDragMove;

		// Token: 0x04006E67 RID: 28263
		[Header("元素移动持续时间")]
		[SerializeField]
		private float swapModeFadeDuration = 0.5f;

		// Token: 0x04006E68 RID: 28264
		[Header("黑蛇背景花纹特效帧数时间")]
		[SerializeField]
		private float snakeBgEffDuration1 = 3f;

		// Token: 0x04006E69 RID: 28265
		[SerializeField]
		private float snakeBgEffDuration2 = 29f;

		// Token: 0x04006E6A RID: 28266
		[SerializeField]
		private float snakeBgEffDuration3 = 6f;

		// Token: 0x04006E6B RID: 28267
		[SerializeField]
		private float snakeBgEffDuration4 = 4f;

		// Token: 0x04006E6C RID: 28268
		[SerializeField]
		private float snakeBgEffFrameRate = 30f;

		// Token: 0x04006E6D RID: 28269
		[Header("元素移动位置标记")]
		[SerializeField]
		private RectTransform leftElementFadeInPoint;

		// Token: 0x04006E6E RID: 28270
		[SerializeField]
		private RectTransform leftElementFadeOutPoint;

		// Token: 0x04006E6F RID: 28271
		[SerializeField]
		private RectTransform rightElementFadeInPoint;

		// Token: 0x04006E70 RID: 28272
		[SerializeField]
		private RectTransform rightElementFadeOutPoint;

		// Token: 0x04006E71 RID: 28273
		[SerializeField]
		private RectTransform avatarElementFadeInPoint;

		// Token: 0x04006E72 RID: 28274
		[SerializeField]
		private RectTransform avatarElementFadeOutPoint;

		// Token: 0x04006E73 RID: 28275
		[Header("面板打开/关闭状态下的 突破板移动范围")]
		[SerializeField]
		private int movePaddingDefault = -1100;

		// Token: 0x04006E74 RID: 28276
		[SerializeField]
		private int movePaddingLeftOpen = -1100;

		// Token: 0x04006E75 RID: 28277
		[SerializeField]
		private int movePaddingRightOpen = -1100;

		// Token: 0x04006E76 RID: 28278
		[Header("伤病属性Group")]
		[SerializeField]
		private CanvasGroup attributeCanvasGroup;

		// Token: 0x04006E77 RID: 28279
		[Header("顶部区域目标位置")]
		[SerializeField]
		private RectTransform upAreaPositionTargetBothOpen;

		// Token: 0x04006E78 RID: 28280
		[SerializeField]
		private RectTransform upAreaPositionTargetLeftClose;

		// Token: 0x04006E79 RID: 28281
		[SerializeField]
		private RectTransform upAreaPositionTargetRightClose;

		// Token: 0x04006E7A RID: 28282
		[SerializeField]
		private RectTransform upAreaPositionTargetBothClose;

		// Token: 0x04006E7B RID: 28283
		[Header("遮罩")]
		[SerializeField]
		private CRawImage normalAreaMask;

		// Token: 0x04006E7C RID: 28284
		[SerializeField]
		private CRawImage swapAreaMask;

		// Token: 0x04006E7D RID: 28285
		[SerializeField]
		private float areaMaskFadeDuration = 1f;

		// Token: 0x04006E7E RID: 28286
		[SerializeField]
		private float areaMaskFadeDurationPhase1 = 0.2f;

		// Token: 0x04006E7F RID: 28287
		[SerializeField]
		private float areaMaskFadeDurationPhase2 = 0.8f;

		// Token: 0x04006E80 RID: 28288
		private CanvasGroup _infomationAreaLeftCanvasGroup;

		// Token: 0x04006E81 RID: 28289
		private CanvasGroup _infomationAreaRightCanvasGroup;

		// Token: 0x04006E82 RID: 28290
		private const string SPINE_ANIM_NAME_IDLE = "idle";

		// Token: 0x04006E83 RID: 28291
		private const string SPINE_ANIM_NAME_PREPARE = "prepare";

		// Token: 0x04006E84 RID: 28292
		private const string SPINE_ANIM_NAME_ATTACK = "attack";

		// Token: 0x04006E85 RID: 28293
		public GameObject[] breakLevelEffectArray;

		// Token: 0x04006E86 RID: 28294
		public GameObject[] breakStepEffectArray;

		// Token: 0x04006E87 RID: 28295
		public GameObject[] finishEffectArray;

		// Token: 0x04006E88 RID: 28296
		public GameObject[] statusEffectArray;

		// Token: 0x04006E89 RID: 28297
		private const float DoorClosedWidth = 0f;

		// Token: 0x04006E8A RID: 28298
		private const float DoorOpenedWidth = 2522.15f;

		// Token: 0x04006E8B RID: 28299
		private const float MaskHeight = 1334f;

		// Token: 0x04006E8C RID: 28300
		private const float FlyDelay = 0.3f;

		// Token: 0x04006E8D RID: 28301
		private short _skillId;

		// Token: 0x04006E8E RID: 28302
		private ushort _selectedPage;

		// Token: 0x04006E8F RID: 28303
		private bool _isReview;

		// Token: 0x04006E90 RID: 28304
		private GameData.Domains.Taiwu.SkillBreakPlate _specificDisplayPlate;

		// Token: 0x04006E91 RID: 28305
		private GameData.Domains.Taiwu.SkillBreakPlate _enteredPlate;

		// Token: 0x04006E92 RID: 28306
		private int _baseCostExp;

		// Token: 0x04006E93 RID: 28307
		private int _taiwuCharId;

		// Token: 0x04006E94 RID: 28308
		private int _currentExp;

		// Token: 0x04006E95 RID: 28309
		private LifeSkillShorts _lifeSkillAttainments;

		// Token: 0x04006E96 RID: 28310
		private short _health = -1;

		// Token: 0x04006E97 RID: 28311
		private Injuries _injuries;

		// Token: 0x04006E98 RID: 28312
		private int _predictExp;

		// Token: 0x04006E99 RID: 28313
		private bool _canBreakOut;

		// Token: 0x04006E9A RID: 28314
		private readonly GameData.Domains.Taiwu.SkillBreakPlate _reusePlate1 = new GameData.Domains.Taiwu.SkillBreakPlate();

		// Token: 0x04006E9B RID: 28315
		private readonly GameData.Domains.Taiwu.SkillBreakPlate _reusePlate2 = new GameData.Domains.Taiwu.SkillBreakPlate();

		// Token: 0x04006E9C RID: 28316
		private bool _usingPlate1 = true;

		// Token: 0x04006E9D RID: 28317
		private bool _usingEnterPlate = true;

		// Token: 0x04006E9E RID: 28318
		private bool _swapFunctionUnlocked;

		// Token: 0x04006E9F RID: 28319
		private CharacterMenuSkillBreakData _skillBreakExtraData = new CharacterMenuSkillBreakData();

		// Token: 0x04006EA0 RID: 28320
		private bool _refersInitialized;

		// Token: 0x04006EA1 RID: 28321
		private SkillBreakPlateIndex _selectedCoordinate;

		// Token: 0x04006EA2 RID: 28322
		private bool _waitingAnimationInFinish;

		// Token: 0x04006EA3 RID: 28323
		private bool _waitingAnimationOutFinish;

		// Token: 0x04006EA4 RID: 28324
		private GameData.Domains.Taiwu.SkillBreakPlate _lastPlate;

		// Token: 0x04006EA5 RID: 28325
		private readonly UIParticlePlayHelper _uiParticlePlayHelper = new UIParticlePlayHelper();

		// Token: 0x04006EA6 RID: 28326
		private SkillBreakPlateIndex _swapFirstSelected = SkillBreakPlateIndex.Invalid;

		// Token: 0x04006EA7 RID: 28327
		private readonly Dictionary<int, SkillBreakPlateCellWrapper> _swapCandidateWrappers = new Dictionary<int, SkillBreakPlateCellWrapper>();

		// Token: 0x04006EA8 RID: 28328
		private bool _triggerHalfStep = false;

		// Token: 0x04006EA9 RID: 28329
		private SkillBreakPlateIndex _targetAutoOpenBonusIndex = SkillBreakPlateIndex.Invalid;

		// Token: 0x04006EAA RID: 28330
		private bool _isLeftPanelOpen;

		// Token: 0x04006EAB RID: 28331
		private bool _isRightPanelOpen;

		// Token: 0x04006EAC RID: 28332
		private Coroutine _oneShortParticlesCoroutine;

		// Token: 0x04006EAD RID: 28333
		private GeneralLineData _maxPowerReuseTipDataDesc;

		// Token: 0x04006EAE RID: 28334
		private GeneralLineData _maxPowerReuseTipDataPowerLine;

		// Token: 0x04006EAF RID: 28335
		private bool _leftElementOpen = true;

		// Token: 0x04006EB0 RID: 28336
		private bool _rightElementOpen = true;

		// Token: 0x02002145 RID: 8517
		private enum ENewInjuriesType
		{
			// Token: 0x0400D44F RID: 54351
			None,
			// Token: 0x0400D450 RID: 54352
			NewOuter,
			// Token: 0x0400D451 RID: 54353
			NewInner
		}
	}
}
