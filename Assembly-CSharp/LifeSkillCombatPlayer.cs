using System;
using System.Collections.Generic;
using System.Text;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using Game.Components.Avatar;
using Game.Views.CharacterMenu;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu.Debate;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000248 RID: 584
public class LifeSkillCombatPlayer
{
	// Token: 0x170003FF RID: 1023
	// (get) Token: 0x0600260F RID: 9743 RVA: 0x001174DC File Offset: 0x001156DC
	private LifeSkillCombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<LifeSkillCombatModel>();
		}
	}

	// Token: 0x17000400 RID: 1024
	// (get) Token: 0x06002610 RID: 9744 RVA: 0x001174E3 File Offset: 0x001156E3
	// (set) Token: 0x06002611 RID: 9745 RVA: 0x001174EB File Offset: 0x001156EB
	public int Score { get; private set; }

	// Token: 0x17000401 RID: 1025
	// (get) Token: 0x06002612 RID: 9746 RVA: 0x001174F4 File Offset: 0x001156F4
	// (set) Token: 0x06002613 RID: 9747 RVA: 0x001174FC File Offset: 0x001156FC
	public int Stress { get; private set; }

	// Token: 0x17000402 RID: 1026
	// (get) Token: 0x06002614 RID: 9748 RVA: 0x00117505 File Offset: 0x00115705
	// (set) Token: 0x06002615 RID: 9749 RVA: 0x0011750D File Offset: 0x0011570D
	public int Energy { get; private set; }

	// Token: 0x17000403 RID: 1027
	// (get) Token: 0x06002616 RID: 9750 RVA: 0x00117516 File Offset: 0x00115716
	// (set) Token: 0x06002617 RID: 9751 RVA: 0x0011751E File Offset: 0x0011571E
	public int StrategyCount { get; private set; }

	// Token: 0x17000404 RID: 1028
	// (get) Token: 0x06002618 RID: 9752 RVA: 0x00117527 File Offset: 0x00115727
	public RectTransform RectTrans
	{
		get
		{
			return this._refers.RectTransform;
		}
	}

	// Token: 0x17000405 RID: 1029
	// (get) Token: 0x06002619 RID: 9753 RVA: 0x00117534 File Offset: 0x00115734
	public string PlayerName
	{
		get
		{
			return this._playerName.Name;
		}
	}

	// Token: 0x17000406 RID: 1030
	// (get) Token: 0x0600261A RID: 9754 RVA: 0x00117541 File Offset: 0x00115741
	public Transform EnergySliderTrans
	{
		get
		{
			return this._energySlider.transform;
		}
	}

	// Token: 0x0600261B RID: 9755 RVA: 0x00117550 File Offset: 0x00115750
	public void Init(Refers refers, bool isTaiwu, CharacterDisplayData characterDisplayData)
	{
		this._refers = refers;
		this._isTaiwu = isTaiwu;
		this._characterDisplayData = characterDisplayData;
		this._charId = characterDisplayData.CharacterId;
		if (this._playerAvatar == null)
		{
			this._playerAvatar = new CharacterAvatar(refers.CGet<Game.Components.Avatar.Avatar>("Avatar"), true);
		}
		if (this._playerName == null)
		{
			this._playerName = new CharacterName(refers.CGet<TextMeshProUGUI>("Name"), null, null);
		}
		this._playerAvatar.CharacterId = this._charId;
		this._playerName.CharacterId = this._charId;
		this._bubble = refers.CGet<Bubble>("Bubble");
		this._bubble.Hide();
		this._energySlider = refers.CGet<CSliderLegacy>("EnergySlider");
		this._energyText = refers.CGet<TextMeshProUGUI>("EnergyText");
		this._scoreLayout = refers.CGet<Transform>("ScoreLayout");
		this._stressSlider = refers.CGet<CSliderLegacy>("StressSlider");
		this._strategyCount = refers.CGet<TextMeshProUGUI>("StrategyCount");
		this._audienceLayout = refers.CGet<RectTransform>("AudienceLayout");
		this._commentLayout = refers.CGet<RectTransform>("CommentLayout");
		this._stressTip = refers.CGet<TooltipInvoker>("StressTip");
		this._stressFill = refers.CGet<CImage>("StressFill");
		this._stressEffectLayout = refers.CGet<RectTransform>("StressEffectLayout");
		this._stressMarkLayout = refers.CGet<RectTransform>("StressMarkLayout");
		this._effectPlayer = refers.CGet<EffectPlayer>("EffectPlayer");
		this._avatarBackEffectPlayer = refers.CGet<EffectPlayer>("AvatarBackEffectPlayer");
		this._energyMask = refers.CGet<GameObject>("EnergyMask");
		this._stressEffectMask = refers.CGet<RectTransform>("StressEffectMask");
		this._stressMaskEffectLayout = refers.CGet<RectTransform>("StressMaskEffectLayout");
		refers.CGet<CButtonObsolete>("Button").ClearAndAddListener(delegate
		{
			this.ShowCharMenu(this._charId);
		});
		this.InitAudience();
		this.InitComment();
		this.RefreshStrategyCountTip();
		this.RefreshEnergyTip();
	}

	// Token: 0x0600261C RID: 9756 RVA: 0x00117748 File Offset: 0x00115948
	public void Refresh(DebatePlayer debatePlayer, bool hasAnim)
	{
		this._debatePlayer = debatePlayer;
		this.SetScore(debatePlayer.GamePoint);
		this.SetEnergy(debatePlayer.Bases, hasAnim);
		this.SetStress(debatePlayer.Pressure, hasAnim);
		this.SetStrategyCount(debatePlayer.StrategyPoint);
	}

	// Token: 0x0600261D RID: 9757 RVA: 0x00117788 File Offset: 0x00115988
	public void SetStrategyCount(int strategyCount)
	{
		this.StrategyCount = strategyCount;
		this._strategyCount.text = this.StrategyCount.ToString();
	}

	// Token: 0x0600261E RID: 9758 RVA: 0x001177B8 File Offset: 0x001159B8
	public void PreviewStrategyCount(int strategyCount)
	{
		strategyCount = Math.Clamp(strategyCount, 0, DebateConstants.MaxStrategyPoint);
		bool flag = this.StrategyCount == strategyCount;
		if (flag)
		{
			this._strategyCount.text = this.StrategyCount.ToString();
		}
		else
		{
			string color = (strategyCount > this.StrategyCount) ? "brightblue" : "brightred";
			this._strategyCount.text = strategyCount.ToString().SetColor(color);
		}
	}

	// Token: 0x0600261F RID: 9759 RVA: 0x00117830 File Offset: 0x00115A30
	private void RefreshStrategyCountTip()
	{
		StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
		stringBuilder.Clear();
		TooltipInvoker strategyCountTip = this._refers.CGet<TooltipInvoker>("StrategyCountTip");
		string[] presetParam = strategyCountTip.PresetParam;
		bool flag = presetParam == null || presetParam.Length != 2;
		if (flag)
		{
			strategyCountTip.PresetParam = new string[2];
		}
		strategyCountTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_StrategyPoint);
		stringBuilder.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_StrategyPoint_Tip, DebateConstants.StrategyPointRecover));
		bool flag2 = this.Stress >= DebateConstants.LowPressurePercent;
		if (flag2)
		{
			stringBuilder.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_StrategyPoint_Tip_Stress, DebateConstants.MidPressurePercent));
		}
		strategyCountTip.PresetParam[1] = stringBuilder.ToString();
		stringBuilder.Clear();
		EasyPool.Free<StringBuilder>(stringBuilder);
	}

	// Token: 0x06002620 RID: 9760 RVA: 0x001178FC File Offset: 0x00115AFC
	public void SetEnergy(int energy, bool hasAnim)
	{
		bool flag = !this._isTaiwu && !SingletonObject.getInstance<LifeSkillCombatModel>().ShowHiddenInfo;
		if (flag)
		{
			this._energyText.text = this._debatePlayer.MaxBases.ToString();
			this._energySlider.value = 1f;
			this._energyMask.gameObject.SetActive(true);
		}
		else
		{
			float value = (float)energy / (float)this._debatePlayer.MaxBases;
			this._energySlider.value = value;
			this._energyText.text = string.Format("{0}/{1}", energy, this._debatePlayer.MaxBases);
			this._energyMask.gameObject.SetActive(false);
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

	// Token: 0x06002621 RID: 9761 RVA: 0x001179E4 File Offset: 0x00115BE4
	public void PreviewEnergy(int previewEnergy, bool hasAnim = false)
	{
		CImage addFill = this._refers.CGet<CImage>("AddFill");
		CImage reduceFill = this._refers.CGet<CImage>("ReduceFill");
		CImage bgFill = this._refers.CGet<CImage>("BgFill");
		CImage previewFill = this._refers.CGet<CImage>("PreviewFill");
		bool flag = previewEnergy > this.Energy;
		if (flag)
		{
			addFill.fillAmount = (float)previewEnergy / (float)this._debatePlayer.MaxBases;
			bgFill.fillAmount = 0f;
			reduceFill.fillAmount = 0f;
			previewFill.fillAmount = (float)this.Energy / (float)this._debatePlayer.MaxBases;
			if (hasAnim)
			{
				previewFill.DOKill(true);
				previewFill.DOFillAmount(addFill.fillAmount, 0.2f).OnComplete(new TweenCallback(this.ClearPreviewEnergy));
			}
		}
		else
		{
			bool flag2 = previewEnergy < this.Energy;
			if (flag2)
			{
				addFill.fillAmount = 0f;
				bgFill.fillAmount = 1f;
				reduceFill.fillAmount = (float)this.Energy / (float)this._debatePlayer.MaxBases;
				previewFill.fillAmount = (float)previewEnergy / (float)this._debatePlayer.MaxBases;
				if (hasAnim)
				{
					reduceFill.DOKill(true);
					reduceFill.DOFillAmount(previewFill.fillAmount, 0.2f).OnComplete(new TweenCallback(this.ClearPreviewEnergy));
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

	// Token: 0x06002622 RID: 9762 RVA: 0x00117B74 File Offset: 0x00115D74
	private void ClearPreviewEnergy()
	{
		CImage addFill = this._refers.CGet<CImage>("AddFill");
		CImage reduceFill = this._refers.CGet<CImage>("ReduceFill");
		CImage bgFill = this._refers.CGet<CImage>("BgFill");
		CImage previewFill = this._refers.CGet<CImage>("PreviewFill");
		addFill.fillAmount = 0f;
		bgFill.fillAmount = 0f;
		reduceFill.fillAmount = 0f;
		previewFill.fillAmount = 0f;
	}

	// Token: 0x06002623 RID: 9763 RVA: 0x00117BF8 File Offset: 0x00115DF8
	private void RefreshEnergyTip()
	{
		StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
		stringBuilder.Clear();
		TooltipInvoker energyTip = this._refers.CGet<TooltipInvoker>("EnergyTip");
		string[] presetParam = energyTip.PresetParam;
		bool flag = presetParam == null || presetParam.Length != 2;
		if (flag)
		{
			energyTip.PresetParam = new string[2];
		}
		energyTip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_Energy);
		stringBuilder.Clear();
		stringBuilder.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_Energy_Tip, DebateConstants.GradeToBasesPercent, DebateConstants.BasesRecoverPercent));
		bool flag2 = this.Stress >= DebateConstants.MidPressurePercent;
		if (flag2)
		{
			stringBuilder.AppendLine(LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_Energy_Tip_Stress, DebateConstants.MidPressurePercent));
		}
		energyTip.PresetParam[1] = stringBuilder.ToString();
		stringBuilder.Clear();
		EasyPool.Free<StringBuilder>(stringBuilder);
	}

	// Token: 0x06002624 RID: 9764 RVA: 0x00117CD8 File Offset: 0x00115ED8
	public void SetStress(int stress, bool hasAnim)
	{
		bool flag = stress > this.Stress;
		if (flag)
		{
			AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundAddStress, false, true);
		}
		this.Stress = stress;
		this._stressSlider.DOKill(false);
		float lastValue = this._stressSlider.value;
		float value = (float)stress / (float)this._debatePlayer.MaxPressure;
		if (hasAnim)
		{
			this._stressSlider.DOValue(value, 0.2f, false).OnUpdate(delegate
			{
				this.SetStressFillImage(this._stressSlider.value);
			}).OnComplete(delegate
			{
				this.PlayStressStateEffect(lastValue, this._stressSlider.value);
			});
		}
		else
		{
			this._stressSlider.value = value;
			this.SetStressFillImage(this._stressSlider.value);
		}
		this._stressTip.Type = TipType.LifeSkillCombatStress;
		TooltipInvoker stressTip = this._stressTip;
		if (stressTip.RuntimeParam == null)
		{
			stressTip.RuntimeParam = EasyPool.Get<ArgumentBox>();
		}
		this._stressTip.RuntimeParam.Set("IsTaiwu", this._isTaiwu);
		this.RefreshStrategyCountTip();
		this.RefreshEnergyTip();
		this.RefreshStressEffect();
	}

	// Token: 0x06002625 RID: 9765 RVA: 0x00117E0C File Offset: 0x0011600C
	private void SetStressFillImage(float curValue)
	{
		int curImageIndex = this.GetStressStateIndex(curValue);
		this._stressFill.SetSprite(string.Format("lifeskillcombat_progress_1_{0}", curImageIndex + 2), false, null);
		for (int i = 0; i < this._stressEffectLayout.childCount; i++)
		{
			GameObject go = this._stressEffectLayout.GetChild(i).gameObject;
			bool show = curImageIndex == i && curValue > 0f;
			bool flag = go.activeSelf != show;
			if (flag)
			{
				go.SetActive(show);
			}
		}
		for (int j = 0; j < this._stressMaskEffectLayout.childCount; j++)
		{
			GameObject go2 = this._stressMaskEffectLayout.GetChild(j).gameObject;
			bool show2 = curImageIndex == j && curValue > 0f;
			bool flag2 = go2.activeSelf != show2;
			if (flag2)
			{
				go2.SetActive(show2);
			}
		}
		this._stressEffectMask.sizeDelta = this._stressEffectMask.sizeDelta.SetY(curValue * this._stressSlider.fillRect.rect.height);
		this._stressMaskEffectLayout.anchoredPosition = this._stressEffectMask.anchoredPosition + Vector2.zero.SetY(this._stressEffectMask.sizeDelta.y);
		for (int k = 0; k < this._stressMarkLayout.childCount; k++)
		{
			string spName = this.GetStressStateMarkImage(curValue, k);
			this._stressMarkLayout.GetChild(k).GetComponent<CImage>().SetSprite(spName, false, null);
		}
	}

	// Token: 0x06002626 RID: 9766 RVA: 0x00117FB8 File Offset: 0x001161B8
	private void PlayStressStateEffect(float lastValue, float curValue)
	{
		int lastImageIndex = this.GetStressStateIndex(lastValue);
		int curImageIndex = this.GetStressStateIndex(curValue);
		bool flag = curImageIndex > lastImageIndex && curImageIndex >= 1;
		if (flag)
		{
			int markIndex = curImageIndex - 1;
			Transform target = this._stressMarkLayout.GetChild(markIndex);
			string markEffectName = string.Format("eff_lifeskillcombat_ui_yali_huo{0}", curImageIndex);
			this._effectPlayer.PlayEffectAt(target, markEffectName, 2f, false);
			AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundStressEffect, false, true);
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
			this._avatarBackEffectPlayer.PlayEffectAt(this.RectTrans, avatarEffectName, 2f, false);
		}
		else
		{
			bool flag2 = curImageIndex == lastImageIndex && curValue > lastValue;
			if (flag2)
			{
				this._avatarBackEffectPlayer.PlayEffectAt(this.RectTrans, "eff_lifeskillcombat_ui_yalishangsheng", 1f, false);
				AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundStressUp, false, false);
			}
			else
			{
				bool flag3 = curValue < lastValue;
				if (flag3)
				{
					this._avatarBackEffectPlayer.PlayEffectAt(this.RectTrans, "eff_lifeskillcombat_ui_yalixiajiang", 1f, false);
					AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundStressDown, false, false);
				}
			}
		}
	}

	// Token: 0x06002627 RID: 9767 RVA: 0x00118124 File Offset: 0x00116324
	private int GetStressStateIndex(float value)
	{
		if (!true)
		{
		}
		int result;
		if (value < 0.75f)
		{
			if (value >= 0.5f)
			{
				result = 1;
			}
			else
			{
				result = 0;
			}
		}
		else if (value >= 1f)
		{
			result = 3;
		}
		else
		{
			result = 2;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06002628 RID: 9768 RVA: 0x00118168 File Offset: 0x00116368
	private string GetStressStateMarkImage(float value, int index)
	{
		if (!true)
		{
		}
		string result;
		switch (index)
		{
		case 0:
			result = ((value < 0.5f) ? "lifeskillcombat_node_3" : "lifeskillcombat_node_0");
			break;
		case 1:
			result = ((value < 0.75f) ? "lifeskillcombat_node_3" : "lifeskillcombat_node_1");
			break;
		case 2:
			result = ((value < 1f) ? "lifeskillcombat_node_3" : "lifeskillcombat_node_2");
			break;
		default:
			if (!true)
			{
			}
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(index);
			break;
		}
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06002629 RID: 9769 RVA: 0x001181E8 File Offset: 0x001163E8
	private void RefreshStressEffect()
	{
		bool isLow = this.Stress >= DebateConstants.LowPressurePercent;
		this._refers.CGet<GameObject>("StrategyCountBgStressEffect").SetActive(isLow);
		bool isMiddle = this.Stress >= DebateConstants.MidPressurePercent;
		this._refers.CGet<GameObject>("EnergyFrameStressEffect").SetActive(isMiddle);
		this._refers.CGet<GameObject>("EnergyMaskStressEffect").SetActive(isMiddle);
	}

	// Token: 0x0600262A RID: 9770 RVA: 0x00118260 File Offset: 0x00116460
	public void ShowPreviewPressure(int origin, int max, int delta)
	{
		GameObject obj = this._refers.CGet<GameObject>("Preview");
		Refers refers = obj.GetComponent<Refers>();
		RectMask2D fillMask = refers.CGet<RectMask2D>("FillMask");
		RectTransform fillParticle = refers.CGet<RectTransform>("FillParticle");
		RectTransform topParticle = refers.CGet<RectTransform>("TopParticle");
		float height = this._stressSlider.GetComponent<RectTransform>().rect.height;
		float percent = (float)origin / (float)max;
		float originHeight = height * percent;
		int diff = max - origin - delta;
		float location = (diff > 0) ? ((float)diff * 3.9f * 100f / (float)max) : 0f;
		topParticle.anchoredPosition = new Vector2(0f, -location);
		fillMask.padding = new Vector4(0f, 0f, 0f, location);
		fillParticle.SetHeight(originHeight);
		obj.SetActive(true);
	}

	// Token: 0x0600262B RID: 9771 RVA: 0x0011833E File Offset: 0x0011653E
	public void HidePreviewPressure()
	{
		this._refers.CGet<GameObject>("Preview").SetActive(false);
	}

	// Token: 0x0600262C RID: 9772 RVA: 0x00118358 File Offset: 0x00116558
	public void SetScore(int score)
	{
		this.Score = score;
		for (int i = 0; i < this._scoreLayout.childCount; i++)
		{
			int index = (score > i) ? (this._isTaiwu ? 0 : 1) : 2;
			this._scoreLayout.GetChild(i).GetComponent<CImage>().SetSprite(string.Format("lifeskillcombat_score_{0}_{1}", index, this.Model.LifeSkillType), false, null);
		}
		Action scoreChanged = this.ScoreChanged;
		if (scoreChanged != null)
		{
			scoreChanged();
		}
	}

	// Token: 0x0600262D RID: 9773 RVA: 0x001183F0 File Offset: 0x001165F0
	public RectTransform GetScoreRectTrans(int score)
	{
		for (int i = 0; i < this._scoreLayout.childCount; i++)
		{
			bool flag = i == score - 1;
			if (flag)
			{
				return this._scoreLayout.GetChild(i) as RectTransform;
			}
		}
		return null;
	}

	// Token: 0x0600262E RID: 9774 RVA: 0x00118440 File Offset: 0x00116640
	public void PlayScoreAddedAnim(int curScore, bool isTaiwuCasted)
	{
		AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundAddScore, false, true);
		string effectName = isTaiwuCasted ? "EffectTaiwuScoreAdded" : "EffectEnemyScoreAdded";
		int lastScore = this.Score;
		for (int i = lastScore + 1; i <= curScore; i++)
		{
			RectTransform target = this.GetScoreRectTrans(i);
			bool flag = target == null;
			if (!flag)
			{
				this._effectPlayer.PlayEffectAt(target, effectName, 1f, false);
			}
		}
	}

	// Token: 0x0600262F RID: 9775 RVA: 0x001184B8 File Offset: 0x001166B8
	public void PlayScoreReducedAnim(int curScore, bool isTaiwuCasted)
	{
		AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundRemoveScore, false, true);
		curScore = Mathf.Max(0, curScore);
		string scoreEffectName = isTaiwuCasted ? "EffectTaiwuScoreHit" : "EffectEnemyScoreHit";
		int lastScore = this.Score;
		for (int i = lastScore; i > curScore; i--)
		{
			RectTransform target = this.GetScoreRectTrans(i);
			bool flag = target == null;
			if (!flag)
			{
				this._effectPlayer.PlayEffectAt(target, scoreEffectName, 1f, false);
			}
		}
		this.SetScore(curScore);
	}

	// Token: 0x06002630 RID: 9776 RVA: 0x0011853C File Offset: 0x0011673C
	public void PlayStressReduceScoreAnim(int score, int stress)
	{
		AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundStressReduceScore, false, true);
		int stressIndex = this.GetStressStateIndex((float)stress / 100f);
		for (int i = this.Score; i > score; i--)
		{
			RectTransform target = this.GetScoreRectTrans(i);
			int offset = score - i + 1;
			string effectName = string.Format("eff_lifeskillcombat_ui_xue_huo{0}", stressIndex + offset);
			this._effectPlayer.PlayEffectAt(target, effectName, 2f, false);
		}
	}

	// Token: 0x06002631 RID: 9777 RVA: 0x001185BC File Offset: 0x001167BC
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
				this._bubble.SetText(content, true);
				Tween bubbleTween = this._bubbleTween;
				if (bubbleTween != null)
				{
					bubbleTween.Kill(false);
				}
				this._bubbleTween = DOVirtual.DelayedCall(delayTime, new TweenCallback(this.HideSpeak), false);
			}
		}
	}

	// Token: 0x06002632 RID: 9778 RVA: 0x001186F1 File Offset: 0x001168F1
	public void HideSpeak()
	{
		this._bubble.Clear();
	}

	// Token: 0x06002633 RID: 9779 RVA: 0x00118700 File Offset: 0x00116900
	private void InitAudience()
	{
		List<CharacterDisplayData> audienceList = SingletonObject.getInstance<LifeSkillCombatModel>().GetAudienceList(this._isTaiwu);
		for (int i = 0; i < this._audienceLayout.childCount; i++)
		{
			Refers refers = this._audienceLayout.GetChild(i).GetComponent<Refers>();
			CharacterDisplayData charData = audienceList[i];
			bool hasChar = charData != null;
			refers.gameObject.SetActive(hasChar);
			bool flag = hasChar;
			if (flag)
			{
				refers.CGet<Game.Components.Avatar.Avatar>("Avatar").Refresh(charData, true);
				CButtonObsolete button = refers.CGet<CButtonObsolete>("Button");
				button.ClearAndAddListener(delegate
				{
					this.ShowCharMenu(charData.CharacterId);
				});
				TooltipInvoker tip = refers.CGet<TooltipInvoker>("Tip");
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

	// Token: 0x06002634 RID: 9780 RVA: 0x0011881C File Offset: 0x00116A1C
	private void ShowCharMenu(int charId)
	{
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.Set("CharacterId", charId);
		argBox.Set("CanOperate", false);
		argBox.Set("OpenFromCombatPrepare", true);
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

	// Token: 0x06002635 RID: 9781 RVA: 0x001188EC File Offset: 0x00116AEC
	private void InitComment()
	{
		for (int i = 0; i < this._commentLayout.childCount; i++)
		{
			Bubble bubble = this._commentLayout.GetChild(i).GetComponent<Bubble>();
			bubble.Hide();
		}
	}

	// Token: 0x06002636 RID: 9782 RVA: 0x00118930 File Offset: 0x00116B30
	public void AddComment(short commentTemplateId, int commentCharId, bool targetIsTaiwu, string targetCharName)
	{
		List<CharacterDisplayData> audienceList = SingletonObject.getInstance<LifeSkillCombatModel>().GetAudienceList(this._isTaiwu);
		int index = audienceList.FindIndex((CharacterDisplayData d) => d != null && d.CharacterId == commentCharId);
		bool flag = index < 0;
		if (!flag)
		{
			Bubble bubble = this._commentLayout.GetChild(index).GetComponent<Bubble>();
			float delay = this.HideAllComment(1f) ? 1f : 0f;
			TweenCallback <>9__2;
			DOVirtual.DelayedCall(delay, delegate
			{
				DebateCommentItem config = Config.DebateComment.Instance[commentTemplateId];
				string spName = targetIsTaiwu ? "sp_bubblebase_4" : "sp_bubblebase_5";
				foreach (CImage image in bubble.GetComponentsInChildren<CImage>())
				{
					image.SetSprite(spName, false, null);
				}
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

	// Token: 0x06002637 RID: 9783 RVA: 0x001189E8 File Offset: 0x00116BE8
	public bool HideAllComment(float delay = 1f)
	{
		bool hasShowing = false;
		for (int i = 0; i < this._commentLayout.childCount; i++)
		{
			Bubble bubble = this._commentLayout.GetChild(i).GetComponent<Bubble>();
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

	// Token: 0x06002638 RID: 9784 RVA: 0x00118A90 File Offset: 0x00116C90
	public void AudienceSpeak(int audienceId, string content, bool useTaiwuColor)
	{
		List<CharacterDisplayData> audienceList = SingletonObject.getInstance<LifeSkillCombatModel>().GetAudienceList(this._isTaiwu);
		int index = audienceList.FindIndex((CharacterDisplayData d) => d != null && d.CharacterId == audienceId);
		bool flag = index < 0;
		if (!flag)
		{
			Bubble bubble = this._commentLayout.GetChild(index).GetComponent<Bubble>();
			float delay = this.HideAllComment(1f) ? 1f : 0f;
			TweenCallback <>9__2;
			DOVirtual.DelayedCall(delay, delegate
			{
				string spName = useTaiwuColor ? "sp_bubblebase_4" : "sp_bubblebase_5";
				foreach (CImage image in bubble.GetComponentsInChildren<CImage>())
				{
					image.SetSprite(spName, false, null);
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

	// Token: 0x04001C21 RID: 7201
	private const float FillMaskScale = 3.9f;

	// Token: 0x04001C22 RID: 7202
	public const float AnimDuration = 0.2f;

	// Token: 0x04001C23 RID: 7203
	private bool _isTaiwu;

	// Token: 0x04001C24 RID: 7204
	private int _charId;

	// Token: 0x04001C29 RID: 7209
	private CharacterDisplayData _characterDisplayData;

	// Token: 0x04001C2A RID: 7210
	private CharacterAvatar _playerAvatar;

	// Token: 0x04001C2B RID: 7211
	private CharacterName _playerName;

	// Token: 0x04001C2C RID: 7212
	private Bubble _bubble;

	// Token: 0x04001C2D RID: 7213
	private CSliderLegacy _energySlider;

	// Token: 0x04001C2E RID: 7214
	private TextMeshProUGUI _energyText;

	// Token: 0x04001C2F RID: 7215
	private Transform _scoreLayout;

	// Token: 0x04001C30 RID: 7216
	private CSliderLegacy _stressSlider;

	// Token: 0x04001C31 RID: 7217
	private CImage _stressFill;

	// Token: 0x04001C32 RID: 7218
	private TextMeshProUGUI _strategyCount;

	// Token: 0x04001C33 RID: 7219
	private Tween _bubbleTween;

	// Token: 0x04001C34 RID: 7220
	private DebatePlayer _debatePlayer;

	// Token: 0x04001C35 RID: 7221
	private RectTransform _audienceLayout;

	// Token: 0x04001C36 RID: 7222
	private RectTransform _commentLayout;

	// Token: 0x04001C37 RID: 7223
	private TooltipInvoker _stressTip;

	// Token: 0x04001C38 RID: 7224
	private RectTransform _stressEffectLayout;

	// Token: 0x04001C39 RID: 7225
	private RectTransform _stressMarkLayout;

	// Token: 0x04001C3A RID: 7226
	private RectTransform _stressEffectMask;

	// Token: 0x04001C3B RID: 7227
	private RectTransform _stressMaskEffectLayout;

	// Token: 0x04001C3C RID: 7228
	private EffectPlayer _effectPlayer;

	// Token: 0x04001C3D RID: 7229
	private EffectPlayer _avatarBackEffectPlayer;

	// Token: 0x04001C3E RID: 7230
	private GameObject _energyMask;

	// Token: 0x04001C3F RID: 7231
	private Refers _refers;

	// Token: 0x04001C40 RID: 7232
	public Action ScoreChanged;

	// Token: 0x04001C41 RID: 7233
	private const float StressStateValue1 = 0.5f;

	// Token: 0x04001C42 RID: 7234
	private const float StressStateValue2 = 0.75f;

	// Token: 0x04001C43 RID: 7235
	private const float StressStateValue3 = 1f;
}
