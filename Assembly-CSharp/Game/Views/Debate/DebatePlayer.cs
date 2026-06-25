using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Views.CharacterMenu;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu.Debate;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Debate
{
	// Token: 0x02000A9F RID: 2719
	public class DebatePlayer : MonoBehaviour
	{
		// Token: 0x17000EA3 RID: 3747
		// (get) Token: 0x0600851D RID: 34077 RVA: 0x003DD5F8 File Offset: 0x003DB7F8
		private LifeSkillCombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<LifeSkillCombatModel>();
			}
		}

		// Token: 0x17000EA4 RID: 3748
		// (get) Token: 0x0600851E RID: 34078 RVA: 0x003DD5FF File Offset: 0x003DB7FF
		// (set) Token: 0x0600851F RID: 34079 RVA: 0x003DD607 File Offset: 0x003DB807
		public int Score { get; private set; }

		// Token: 0x17000EA5 RID: 3749
		// (get) Token: 0x06008520 RID: 34080 RVA: 0x003DD610 File Offset: 0x003DB810
		// (set) Token: 0x06008521 RID: 34081 RVA: 0x003DD618 File Offset: 0x003DB818
		public int Stress { get; private set; }

		// Token: 0x17000EA6 RID: 3750
		// (get) Token: 0x06008522 RID: 34082 RVA: 0x003DD621 File Offset: 0x003DB821
		// (set) Token: 0x06008523 RID: 34083 RVA: 0x003DD629 File Offset: 0x003DB829
		public int Energy { get; private set; }

		// Token: 0x17000EA7 RID: 3751
		// (get) Token: 0x06008524 RID: 34084 RVA: 0x003DD632 File Offset: 0x003DB832
		// (set) Token: 0x06008525 RID: 34085 RVA: 0x003DD63A File Offset: 0x003DB83A
		public int StrategyCount { get; private set; }

		// Token: 0x17000EA8 RID: 3752
		// (get) Token: 0x06008526 RID: 34086 RVA: 0x003DD643 File Offset: 0x003DB843
		public string PlayerName
		{
			get
			{
				return this.textName.text;
			}
		}

		// Token: 0x17000EA9 RID: 3753
		// (get) Token: 0x06008527 RID: 34087 RVA: 0x003DD650 File Offset: 0x003DB850
		public RectTransform RectTrans
		{
			get
			{
				return base.transform as RectTransform;
			}
		}

		// Token: 0x17000EAA RID: 3754
		// (get) Token: 0x06008528 RID: 34088 RVA: 0x003DD65D File Offset: 0x003DB85D
		public Transform EnergySliderTrans
		{
			get
			{
				return this.imageEnergy.transform;
			}
		}

		// Token: 0x06008529 RID: 34089 RVA: 0x003DD66C File Offset: 0x003DB86C
		public void Init(bool isTaiwu, CharacterDisplayData characterDisplayData)
		{
			this._isTaiwu = isTaiwu;
			this._characterDisplayData = characterDisplayData;
			this._charId = characterDisplayData.CharacterId;
			this.bubble.Hide();
			this.avatar.Refresh(characterDisplayData, true);
			this.buttonAvatar.ClearAndAddListener(delegate
			{
				this.ShowCharMenu(this._charId);
			});
			this.textName.text = NameCenter.GetMonasticTitleOrDisplayName(characterDisplayData, this._isTaiwu);
			this.InitAudience();
			this.InitComment();
			this.RefreshStrategyCountTip();
			this.RefreshEnergyTip();
			this.RefreshFirstMove();
		}

		// Token: 0x0600852A RID: 34090 RVA: 0x003DD701 File Offset: 0x003DB901
		public void Refresh(DebatePlayer debatePlayer, bool hasAnim)
		{
			this._debatePlayer = debatePlayer;
			this.SetScore(debatePlayer.GamePoint);
			this.SetEnergy(debatePlayer.Bases, hasAnim);
			this.SetStress(debatePlayer.Pressure, hasAnim);
			this.SetStrategyCount(debatePlayer.StrategyPoint);
		}

		// Token: 0x0600852B RID: 34091 RVA: 0x003DD744 File Offset: 0x003DB944
		public void SetStrategyCount(int strategyCount)
		{
			this.StrategyCount = strategyCount;
			this.textStrategyCount.text = this.StrategyCount.ToString();
		}

		// Token: 0x0600852C RID: 34092 RVA: 0x003DD774 File Offset: 0x003DB974
		public void PreviewStrategyCount(int strategyCount)
		{
			strategyCount = Math.Clamp(strategyCount, 0, DebateConstants.MaxStrategyPoint);
			bool flag = this.StrategyCount == strategyCount;
			if (flag)
			{
				this.textStrategyCount.text = this.StrategyCount.ToString();
			}
			else
			{
				string color = (strategyCount > this.StrategyCount) ? "brightblue" : "brightred";
				this.textStrategyCount.text = strategyCount.ToString().SetColor(color);
			}
		}

		// Token: 0x0600852D RID: 34093 RVA: 0x003DD7EC File Offset: 0x003DB9EC
		private void RefreshStrategyCountTip()
		{
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			stringBuilder.Clear();
			string[] presetParam = this.tipStrategyCount.PresetParam;
			bool flag = presetParam == null || presetParam.Length != 2;
			if (flag)
			{
				this.tipStrategyCount.PresetParam = new string[2];
			}
			this.tipStrategyCount.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_StrategyPoint);
			stringBuilder.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_StrategyPoint_Tip, DebateConstants.StrategyPointRecover));
			bool flag2 = this.Stress >= DebateConstants.MidPressurePercent;
			if (flag2)
			{
				stringBuilder.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_StrategyPoint_Tip_Stress, DebateConstants.MidPressurePercent));
			}
			this.tipStrategyCount.PresetParam[1] = stringBuilder.ToString();
			stringBuilder.Clear();
			EasyPool.Free<StringBuilder>(stringBuilder);
		}

		// Token: 0x0600852E RID: 34094 RVA: 0x003DD8BC File Offset: 0x003DBABC
		public void SetEnergy(int energy, bool hasAnim)
		{
			this.imageEnergyBack.enabled = false;
			bool flag = !this._isTaiwu && !SingletonObject.getInstance<LifeSkillCombatModel>().ShowHiddenInfo;
			if (flag)
			{
				this.textEnergy.text = this._debatePlayer.MaxBases.ToString();
				this.imageEnergy.fillAmount = 1f;
				this.objHideEnergy.gameObject.SetActive(true);
				this.textHideEnergy.text = string.Format("{0}", this._debatePlayer.MaxBases);
				this.textEnergy.gameObject.SetActive(false);
			}
			else
			{
				float value = (float)energy / (float)this._debatePlayer.MaxBases;
				this.imageEnergy.fillAmount = value;
				this.objHideEnergy.gameObject.SetActive(false);
				this.textEnergy.gameObject.SetActive(true);
				this.textEnergy.text = string.Format("{0}/{1}", energy, this._debatePlayer.MaxBases);
				if (hasAnim)
				{
					this.PreviewEnergy(energy, true);
				}
				else
				{
					this.ClearPreviewEnergy();
				}
				this.Energy = energy;
			}
		}

		// Token: 0x0600852F RID: 34095 RVA: 0x003DD9FC File Offset: 0x003DBBFC
		public void PreviewEnergy(int previewEnergy, bool hasAnim = false)
		{
			bool flag = previewEnergy > this.Energy;
			if (flag)
			{
				this.imageEnergyAdd.fillAmount = (float)previewEnergy / (float)this._debatePlayer.MaxBases;
				this.imageEnergyBack.enabled = false;
				this.imageEnergyReduce.fillAmount = 0f;
				this.imageEnergyPreview.fillAmount = (float)this.Energy / (float)this._debatePlayer.MaxBases;
				if (hasAnim)
				{
					this.imageEnergyPreview.DOKill(true);
					this.imageEnergyPreview.DOFillAmount(this.imageEnergyAdd.fillAmount, 1f).OnComplete(new TweenCallback(this.ClearPreviewEnergy));
				}
			}
			else
			{
				bool flag2 = previewEnergy < this.Energy;
				if (flag2)
				{
					this.imageEnergyAdd.fillAmount = 0f;
					this.imageEnergyBack.enabled = true;
					this.imageEnergyReduce.fillAmount = (float)this.Energy / (float)this._debatePlayer.MaxBases;
					this.imageEnergyPreview.fillAmount = (float)previewEnergy / (float)this._debatePlayer.MaxBases;
					if (hasAnim)
					{
						this.imageEnergyReduce.DOKill(true);
						this.imageEnergyReduce.DOFillAmount(this.imageEnergyPreview.fillAmount, 1f).OnComplete(new TweenCallback(this.ClearPreviewEnergy));
					}
				}
				else
				{
					bool flag3 = !hasAnim;
					if (flag3)
					{
						this.ClearPreviewEnergy();
					}
				}
			}
		}

		// Token: 0x06008530 RID: 34096 RVA: 0x003DDB7C File Offset: 0x003DBD7C
		private void ClearPreviewEnergy()
		{
			this.imageEnergyBack.enabled = false;
			this.imageEnergyAdd.fillAmount = 0f;
			this.imageEnergyBack.fillAmount = 0f;
			this.imageEnergyReduce.fillAmount = 0f;
			this.imageEnergyPreview.fillAmount = 0f;
		}

		// Token: 0x06008531 RID: 34097 RVA: 0x003DDBDC File Offset: 0x003DBDDC
		private void RefreshEnergyTip()
		{
			StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
			stringBuilder.Clear();
			this.tipEnergy.Type = TipType.Simple;
			string[] presetParam = this.tipEnergy.PresetParam;
			bool flag = presetParam == null || presetParam.Length != 2;
			if (flag)
			{
				this.tipEnergy.PresetParam = new string[2];
			}
			this.tipEnergy.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Energy);
			stringBuilder.Clear();
			stringBuilder.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_Energy_Tip, DebateConstants.GradeToBasesPercent, DebateConstants.BasesRecoverPercent));
			bool flag2 = this.Stress >= DebateConstants.MidPressurePercent;
			if (flag2)
			{
				stringBuilder.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_Energy_Tip_Stress, DebateConstants.MidPressurePercent));
			}
			this.tipEnergy.PresetParam[1] = stringBuilder.ToString();
			stringBuilder.Clear();
			EasyPool.Free<StringBuilder>(stringBuilder);
		}

		// Token: 0x06008532 RID: 34098 RVA: 0x003DDCC8 File Offset: 0x003DBEC8
		public void SetStress(int stress, bool hasAnim)
		{
			bool flag = stress > this.Stress;
			if (flag)
			{
				AudioManager.Instance.PlaySound(ViewDebate.SoundAddStress, false, true);
			}
			this.Stress = stress;
			this.imageStress.DOKill(false);
			float lastValue = this.imageStress.fillAmount;
			float value = (float)stress / (float)this._debatePlayer.MaxPressure;
			if (hasAnim)
			{
				this.imageStress.DOFillAmount(value, 1f).OnUpdate(delegate
				{
					this.SetStressFillImage(this.imageStress.fillAmount);
				}).OnComplete(delegate
				{
					this.PlayStressStateEffect(lastValue, this.imageStress.fillAmount);
				});
			}
			else
			{
				this.imageStress.fillAmount = value;
				this.SetStressFillImage(this.imageStress.fillAmount);
			}
			this.tipStress.Type = TipType.LifeSkillCombatStress;
			TooltipInvoker tooltipInvoker = this.tipStress;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
			}
			this.tipStress.RuntimeParam.Set("IsTaiwu", this._isTaiwu);
			this.RefreshStrategyCountTip();
			this.RefreshEnergyTip();
			this.RefreshStressEffect();
		}

		// Token: 0x06008533 RID: 34099 RVA: 0x003DDDF8 File Offset: 0x003DBFF8
		private void SetStressFillImage(float curValue)
		{
			int curImageIndex = this.GetStressStateIndex(curValue);
			this.imageStress.sprite = this.spriteStressArray[curImageIndex];
		}

		// Token: 0x06008534 RID: 34100 RVA: 0x003DDE22 File Offset: 0x003DC022
		private void RefreshStressEffect()
		{
			this.objStrategyCountStressEffect.SetActive(this.Stress >= DebateConstants.LowPressurePercent);
			this.objEnergyStressEffect.SetActive(this.Stress >= DebateConstants.MidPressurePercent);
		}

		// Token: 0x06008535 RID: 34101 RVA: 0x003DDE60 File Offset: 0x003DC060
		private void PlayStressStateEffect(float lastValue, float curValue)
		{
			int lastImageIndex = this.GetStressStateIndex(lastValue);
			int curImageIndex = this.GetStressStateIndex(curValue);
			bool flag = curImageIndex > lastImageIndex && curImageIndex >= 1;
			if (flag)
			{
				AudioManager.Instance.PlaySound(ViewDebate.SoundStressEffect, false, true);
				if (!true)
				{
				}
				string text;
				switch (curImageIndex)
				{
				case 1:
					text = "eff_lifeskillcombat_ui_huangluan";
					break;
				case 2:
					text = "eff_lifeskillcombat_ui_yuwulunci";
					break;
				case 3:
					text = "eff_lifeskillcombat_ui_diediebuxiu";
					break;
				default:
					if (!true)
					{
					}
					<PrivateImplementationDetails>.ThrowSwitchExpressionException(curImageIndex);
					break;
				}
				if (!true)
				{
				}
				string avatarEffectName = text;
				this.avatarBackEffectPlayer.PlayEffectAt(this.avatarBackEffectPlayer.transform, avatarEffectName, 2f, false);
			}
			else
			{
				bool flag2 = curImageIndex == lastImageIndex && curValue > lastValue;
				if (flag2)
				{
					this.avatarBackEffectPlayer.PlayEffectAt(this.avatarBackEffectPlayer.transform, "eff_lifeskillcombat_ui_yalishangsheng", 1f, false);
					AudioManager.Instance.PlaySound(ViewDebate.SoundStressUp, false, false);
				}
				else
				{
					bool flag3 = curValue < lastValue;
					if (flag3)
					{
						this.avatarBackEffectPlayer.PlayEffectAt(this.avatarBackEffectPlayer.transform, "eff_lifeskillcombat_ui_yalixiajiang", 1f, false);
						AudioManager.Instance.PlaySound(ViewDebate.SoundStressDown, false, false);
					}
				}
			}
		}

		// Token: 0x06008536 RID: 34102 RVA: 0x003DDFA0 File Offset: 0x003DC1A0
		private int GetStressStateIndex(float value)
		{
			value *= 100f;
			bool flag = value < (float)DebateConstants.LowPressurePercent;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = value < (float)DebateConstants.MidPressurePercent;
				if (flag2)
				{
					result = 1;
				}
				else
				{
					bool flag3 = value < (float)DebateConstants.HighPressurePercent;
					if (flag3)
					{
						result = 2;
					}
					else
					{
						result = 3;
					}
				}
			}
			return result;
		}

		// Token: 0x06008537 RID: 34103 RVA: 0x003DDFEF File Offset: 0x003DC1EF
		public void ShowPreviewPressure(int origin, int max, int delta)
		{
		}

		// Token: 0x06008538 RID: 34104 RVA: 0x003DDFF2 File Offset: 0x003DC1F2
		public void HidePreviewPressure()
		{
		}

		// Token: 0x06008539 RID: 34105 RVA: 0x003DDFF8 File Offset: 0x003DC1F8
		public static string GetScoreSpriteName(bool enabled, bool isTaiwu, sbyte lifeSkillType)
		{
			int index = enabled ? (isTaiwu ? 0 : 1) : 2;
			return string.Format("lifeskillcombat_score_{0}_{1}", index, lifeSkillType);
		}

		// Token: 0x0600853A RID: 34106 RVA: 0x003DE030 File Offset: 0x003DC230
		public void SetScore(int score)
		{
			this.Score = score;
			for (int i = 0; i < this.scoreLayout.childCount; i++)
			{
				bool enabled = score > i;
				string spName = DebatePlayer.GetScoreSpriteName(enabled, this._isTaiwu, this.Model.LifeSkillType);
				this.scoreLayout.GetChild(i).GetComponent<CImage>().SetSprite(spName, false, null);
			}
			Action scoreChanged = this.ScoreChanged;
			if (scoreChanged != null)
			{
				scoreChanged();
			}
		}

		// Token: 0x0600853B RID: 34107 RVA: 0x003DE0AC File Offset: 0x003DC2AC
		public RectTransform GetScoreRectTrans(int score)
		{
			for (int i = 0; i < this.scoreLayout.childCount; i++)
			{
				bool flag = i == score - 1;
				if (flag)
				{
					return this.scoreLayout.GetChild(i) as RectTransform;
				}
			}
			return null;
		}

		// Token: 0x0600853C RID: 34108 RVA: 0x003DE0FC File Offset: 0x003DC2FC
		public void PlayScoreAddedAnim(int curScore, bool isTaiwuCasted)
		{
			AudioManager.Instance.PlaySound(ViewDebate.SoundAddScore, false, true);
			string effectName = isTaiwuCasted ? "EffectTaiwuScoreAdded" : "EffectEnemyScoreAdded";
			int lastScore = this.Score;
			for (int i = lastScore + 1; i <= curScore; i++)
			{
				RectTransform target = this.GetScoreRectTrans(i);
				bool flag = target == null;
				if (!flag)
				{
					this.effectPlayer.PlayEffectAt(target, effectName, 1f, false);
				}
			}
		}

		// Token: 0x0600853D RID: 34109 RVA: 0x003DE174 File Offset: 0x003DC374
		public void PlayScoreReducedAnim(int curScore, bool isTaiwuCasted)
		{
			AudioManager.Instance.PlaySound(ViewDebate.SoundRemoveScore, false, true);
			curScore = Mathf.Max(0, curScore);
			string scoreEffectName = isTaiwuCasted ? "EffectTaiwuScoreHit" : "EffectEnemyScoreHit";
			int lastScore = this.Score;
			for (int i = lastScore; i > curScore; i--)
			{
				RectTransform target = this.GetScoreRectTrans(i);
				bool flag = target == null;
				if (!flag)
				{
					this.effectPlayer.PlayEffectAt(target, scoreEffectName, 1f, false);
				}
			}
			this.SetScore(curScore);
		}

		// Token: 0x0600853E RID: 34110 RVA: 0x003DE1F8 File Offset: 0x003DC3F8
		public void PlayStressReduceScoreAnim(int score, int stress)
		{
			AudioManager.Instance.PlaySound(ViewDebate.SoundStressReduceScore, false, true);
			int stressIndex = this.GetStressStateIndex((float)stress / 100f);
			for (int i = this.Score; i > score; i--)
			{
				RectTransform target = this.GetScoreRectTrans(i);
				int offset = score - i + 1;
				string effectName = string.Format("eff_lifeskillcombat_ui_xue_huo{0}", stressIndex + offset);
				this.effectPlayer.PlayEffectAt(target, effectName, 2f, false);
			}
		}

		// Token: 0x0600853F RID: 34111 RVA: 0x003DE278 File Offset: 0x003DC478
		public void Speak(short key, float delayTime = 4f)
		{
			LifeSkillCombatTalkItem talkConfig = LifeSkillCombatTalk.Instance.GetItem(key);
			bool flag = talkConfig == null;
			if (!flag)
			{
				sbyte behaviorType = this._characterDisplayData.BehaviorType;
				if (!true)
				{
				}
				string text;
				switch (behaviorType)
				{
				case 0:
					text = talkConfig.JustContent;
					break;
				case 1:
					text = talkConfig.KindContent;
					break;
				case 2:
					text = talkConfig.EvenContent;
					break;
				case 3:
					text = talkConfig.RebelContent;
					break;
				case 4:
					text = talkConfig.EgoisticContent;
					break;
				default:
					if (!true)
					{
					}
					<PrivateImplementationDetails>.ThrowSwitchExpressionException(behaviorType);
					break;
				}
				if (!true)
				{
				}
				string content = text;
				bool flag2 = content.IsNullOrEmpty();
				if (flag2)
				{
					content = talkConfig.NormalContent;
				}
				bool needRepalceType = talkConfig.NeedRepalceType;
				if (needRepalceType)
				{
					LifeSkillTypeItem typeConfig = LifeSkillType.Instance.GetItem(SingletonObject.getInstance<LifeSkillCombatModel>().LifeSkillType);
					content = content.GetFormat(typeConfig.DialogInBattle);
				}
				bool flag3 = content.IsNullOrEmpty();
				if (!flag3)
				{
					this.HideAllComment(0f);
					this.bubble.SetText(content, true);
					Tween bubbleTween = this._bubbleTween;
					if (bubbleTween != null)
					{
						bubbleTween.Kill(false);
					}
					this._bubbleTween = DOVirtual.DelayedCall(delayTime, new TweenCallback(this.HideSpeak), false);
				}
			}
		}

		// Token: 0x06008540 RID: 34112 RVA: 0x003DE3AD File Offset: 0x003DC5AD
		public void HideSpeak()
		{
			this.bubble.Clear();
		}

		// Token: 0x06008541 RID: 34113 RVA: 0x003DE3BC File Offset: 0x003DC5BC
		private void InitAudience()
		{
			List<CharacterDisplayData> audienceList = SingletonObject.getInstance<LifeSkillCombatModel>().GetAudienceList(this._isTaiwu);
			for (int i = 0; i < this.audienceLayout.childCount; i++)
			{
				Transform refers = this.audienceLayout.GetChild(i);
				CharacterDisplayData charData = audienceList[i];
				bool hasChar = charData != null;
				refers.gameObject.SetActive(hasChar);
				bool flag = hasChar;
				if (flag)
				{
					refers.GetComponentInChildren<Game.Components.Avatar.Avatar>().Refresh(charData, true);
					CButton button = refers.GetComponentInChildren<CButton>();
					button.ClearAndAddListener(delegate
					{
						this.ShowAllAudienceCharMenu(charData.CharacterId);
					});
					TooltipInvoker tip = refers.GetComponentInChildren<TooltipInvoker>();
					tip.Type = TipType.LifeSkillCombatAudience;
					TooltipInvoker tooltipInvoker = tip;
					if (tooltipInvoker.RuntimeParam == null)
					{
						tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
					}
					tip.RuntimeParam.SetObject("CharData", charData);
				}
			}
		}

		// Token: 0x06008542 RID: 34114 RVA: 0x003DE4C4 File Offset: 0x003DC6C4
		private void ShowCharMenu(int charId)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CharacterId", charId);
			argBox.Set("CanOperate", false);
			argBox.Set("OpenFromCombatPrepare", true);
			argBox.Set("PreviousView", 3);
			argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.AttainmentBase, ECharacterSubPage.AttainmentLifeSkill));
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIElement characterMenu = UIElement.CharacterMenu;
			characterMenu.OnShowed = (Action)Delegate.Combine(characterMenu.OnShowed, new Action(delegate()
			{
				GEvent.OnEvent(UiEvents.CombatLifeSkillClickChar, null);
				Time.timeScale = 1f;
			}));
			UIElement characterMenu2 = UIElement.CharacterMenu;
			characterMenu2.OnHide = (Action)Delegate.Combine(characterMenu2.OnHide, new Action(delegate()
			{
				this.Model.RefreshTimeScale();
			}));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06008543 RID: 34115 RVA: 0x003DE5A0 File Offset: 0x003DC7A0
		private void ShowAllAudienceCharMenu(int charId)
		{
			List<int> selfAudienceList = (from d in this.Model.GetAudienceList(true)
			where d != null
			select d.CharacterId).ToList<int>();
			List<int> enemyAudienceList = (from d in this.Model.GetAudienceList(false)
			where d != null
			select d.CharacterId).ToList<int>();
			List<int> allAudienceList = selfAudienceList.Union(enemyAudienceList).ToList<int>();
			allAudienceList.Remove(charId);
			allAudienceList.Insert(0, charId);
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("CharacterIdList", allAudienceList);
			argBox.Set("CanOperate", false);
			argBox.Set("OpenFromCombatPrepare", true);
			argBox.Set("PreviousView", 3);
			argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.AttainmentBase, ECharacterSubPage.AttainmentLifeSkill));
			UIElement.CharacterMenu.SetOnInitArgs(argBox);
			UIElement characterMenu = UIElement.CharacterMenu;
			characterMenu.OnShowed = (Action)Delegate.Combine(characterMenu.OnShowed, new Action(delegate()
			{
				GEvent.OnEvent(UiEvents.CombatLifeSkillClickChar, null);
				Time.timeScale = 1f;
			}));
			UIElement characterMenu2 = UIElement.CharacterMenu;
			characterMenu2.OnHide = (Action)Delegate.Combine(characterMenu2.OnHide, new Action(delegate()
			{
				this.Model.RefreshTimeScale();
			}));
			UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
		}

		// Token: 0x06008544 RID: 34116 RVA: 0x003DE750 File Offset: 0x003DC950
		private void InitComment()
		{
			for (int i = 0; i < this.commentLayout.childCount; i++)
			{
				Bubble bubble = this.commentLayout.GetChild(i).GetComponent<Bubble>();
				bubble.Hide();
			}
		}

		// Token: 0x06008545 RID: 34117 RVA: 0x003DE794 File Offset: 0x003DC994
		public void AddComment(short commentTemplateId, int commentCharId, bool targetIsTaiwu, string targetCharName)
		{
			List<CharacterDisplayData> audienceList = SingletonObject.getInstance<LifeSkillCombatModel>().GetAudienceList(this._isTaiwu);
			int index = audienceList.FindIndex((CharacterDisplayData d) => d != null && d.CharacterId == commentCharId);
			bool flag = index < 0;
			if (!flag)
			{
				Bubble bubble = this.commentLayout.GetChild(index).GetComponent<Bubble>();
				float delay = this.HideAllComment(1f) ? 1f : 0f;
				TweenCallback <>9__2;
				DOVirtual.DelayedCall(delay, delegate
				{
					DebateCommentItem config = Config.DebateComment.Instance[commentTemplateId];
					string playerName = targetCharName.SetColor("pinkyellow");
					string content = config.BubbleContent.GetFormat(playerName);
					bool flag2 = !content.IsNullOrEmpty();
					if (flag2)
					{
						content = content.SetColor(config.IsPositive ? "brightblue" : "brightred");
					}
					bubble.SetText(content, true);
					float delay2 = 4f;
					TweenCallback callback;
					if ((callback = <>9__2) == null)
					{
						callback = (<>9__2 = delegate()
						{
							bubble.Hide();
						});
					}
					DOVirtual.DelayedCall(delay2, callback, false).SetTarget(bubble);
				}, false).SetTarget(bubble);
			}
		}

		// Token: 0x06008546 RID: 34118 RVA: 0x003DE844 File Offset: 0x003DCA44
		public bool HideAllComment(float delay = 1f)
		{
			bool hasShowing = false;
			for (int i = 0; i < this.commentLayout.childCount; i++)
			{
				Bubble bubble = this.commentLayout.GetChild(i).GetComponent<Bubble>();
				bool flag = bubble.transform.localScale.y > 0f;
				if (flag)
				{
					hasShowing = true;
					bubble.DOKill(false);
					DOVirtual.DelayedCall(delay, delegate
					{
						bubble.Hide();
					}, false).SetTarget(bubble);
				}
			}
			return hasShowing;
		}

		// Token: 0x06008547 RID: 34119 RVA: 0x003DE8EC File Offset: 0x003DCAEC
		public void AudienceSpeak(int audienceId, string content, bool useTaiwuColor)
		{
			List<CharacterDisplayData> audienceList = SingletonObject.getInstance<LifeSkillCombatModel>().GetAudienceList(this._isTaiwu);
			int index = audienceList.FindIndex((CharacterDisplayData d) => d != null && d.CharacterId == audienceId);
			bool flag = index < 0;
			if (!flag)
			{
				Bubble bubble = this.commentLayout.GetChild(index).GetComponent<Bubble>();
				float delay = this.HideAllComment(1f) ? 1f : 0f;
				TweenCallback <>9__2;
				DOVirtual.DelayedCall(delay, delegate
				{
					bubble.SetText(content, true);
					float delay2 = 4f;
					TweenCallback callback;
					if ((callback = <>9__2) == null)
					{
						callback = (<>9__2 = delegate()
						{
							bubble.Hide();
						});
					}
					DOVirtual.DelayedCall(delay2, callback, false).SetTarget(bubble);
				}, false).SetTarget(bubble);
			}
		}

		// Token: 0x06008548 RID: 34120 RVA: 0x003DE994 File Offset: 0x003DCB94
		private void RefreshFirstMove()
		{
			bool isFirst = (this._isTaiwu && this.Model.DebateGame.IsTaiwuFirst) || (!this._isTaiwu && !this.Model.DebateGame.IsTaiwuFirst);
			Sprite sprite = isFirst ? this.spriteFirstMoveArray[0] : this.spriteFirstMoveArray[1];
			this.imageFirstMove.sprite = sprite;
			TooltipInvoker tip = this.imageFirstMove.GetComponent<TooltipInvoker>();
			tip.RuntimeParam = EasyPool.Get<ArgumentBox>();
			tip.Type = (isFirst ? TipType.LifeSkillCombatFirstMove : TipType.LifeSkillCombatLastMove);
		}

		// Token: 0x04006614 RID: 26132
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04006615 RID: 26133
		[SerializeField]
		private CButton buttonAvatar;

		// Token: 0x04006616 RID: 26134
		[SerializeField]
		private TextMeshProUGUI textName;

		// Token: 0x04006617 RID: 26135
		[SerializeField]
		private Bubble bubble;

		// Token: 0x04006618 RID: 26136
		[SerializeField]
		private CImage imageEnergy;

		// Token: 0x04006619 RID: 26137
		[SerializeField]
		private CImage imageEnergyAdd;

		// Token: 0x0400661A RID: 26138
		[SerializeField]
		private CImage imageEnergyReduce;

		// Token: 0x0400661B RID: 26139
		[SerializeField]
		private CImage imageEnergyBack;

		// Token: 0x0400661C RID: 26140
		[SerializeField]
		private CImage imageEnergyPreview;

		// Token: 0x0400661D RID: 26141
		[SerializeField]
		private GameObject objEnergyStressEffect;

		// Token: 0x0400661E RID: 26142
		[SerializeField]
		private GameObject objHideEnergy;

		// Token: 0x0400661F RID: 26143
		[SerializeField]
		private TextMeshProUGUI textEnergy;

		// Token: 0x04006620 RID: 26144
		[SerializeField]
		private TextMeshProUGUI textHideEnergy;

		// Token: 0x04006621 RID: 26145
		[SerializeField]
		private TooltipInvoker tipEnergy;

		// Token: 0x04006622 RID: 26146
		[SerializeField]
		private Transform scoreLayout;

		// Token: 0x04006623 RID: 26147
		[SerializeField]
		private CImage imageStress;

		// Token: 0x04006624 RID: 26148
		[SerializeField]
		private TooltipInvoker tipStress;

		// Token: 0x04006625 RID: 26149
		[SerializeField]
		private TextMeshProUGUI textStrategyCount;

		// Token: 0x04006626 RID: 26150
		[SerializeField]
		private TooltipInvoker tipStrategyCount;

		// Token: 0x04006627 RID: 26151
		[SerializeField]
		private GameObject objStrategyCountStressEffect;

		// Token: 0x04006628 RID: 26152
		[SerializeField]
		private RectTransform audienceLayout;

		// Token: 0x04006629 RID: 26153
		[SerializeField]
		private RectTransform commentLayout;

		// Token: 0x0400662A RID: 26154
		[SerializeField]
		private EffectPlayer effectPlayer;

		// Token: 0x0400662B RID: 26155
		[SerializeField]
		private EffectPlayer avatarBackEffectPlayer;

		// Token: 0x0400662C RID: 26156
		[SerializeField]
		private CImage imageFirstMove;

		// Token: 0x0400662D RID: 26157
		[SerializeField]
		private Sprite[] spriteStressArray;

		// Token: 0x0400662E RID: 26158
		[SerializeField]
		private Sprite[] spriteFirstMoveArray;

		// Token: 0x0400662F RID: 26159
		private Tween _bubbleTween;

		// Token: 0x04006630 RID: 26160
		private DebatePlayer _debatePlayer;

		// Token: 0x04006631 RID: 26161
		public const float AnimDuration = 1f;

		// Token: 0x04006632 RID: 26162
		private bool _isTaiwu;

		// Token: 0x04006633 RID: 26163
		private int _charId;

		// Token: 0x04006638 RID: 26168
		public Action ScoreChanged;

		// Token: 0x04006639 RID: 26169
		private CharacterDisplayData _characterDisplayData;
	}
}
