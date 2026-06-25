using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using DG.Tweening;
using FrameWork;
using GameData.Domains.CombatSkill;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Combat
{
	// Token: 0x02000AFD RID: 2813
	public class CombatGangqi : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F42 RID: 3906
		// (get) Token: 0x06008A60 RID: 35424 RVA: 0x004011CC File Offset: 0x003FF3CC
		private static Color DirectProgressColor
		{
			get
			{
				return Colors.Instance.GradeColors[3];
			}
		}

		// Token: 0x17000F43 RID: 3907
		// (get) Token: 0x06008A61 RID: 35425 RVA: 0x004011DE File Offset: 0x003FF3DE
		private static Color ReverseProgressColor
		{
			get
			{
				return Colors.Instance.GradeColors[5];
			}
		}

		// Token: 0x17000F44 RID: 3908
		// (get) Token: 0x06008A62 RID: 35426 RVA: 0x004011F0 File Offset: 0x003FF3F0
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008A63 RID: 35427 RVA: 0x004011F8 File Offset: 0x003FF3F8
		private float GetProgressMargin()
		{
			float totalWidth = this.progress.rectTransform.rect.width;
			float visibleWidth = this.visualProgressRange.rect.width;
			return (totalWidth - visibleWidth) / 2f;
		}

		// Token: 0x06008A64 RID: 35428 RVA: 0x00401240 File Offset: 0x003FF440
		private float CalculateAdjustedFillAmount(float baseFillAmount)
		{
			bool flag = this.progress == null || this.visualProgressRange == null;
			float result;
			if (flag)
			{
				result = baseFillAmount;
			}
			else
			{
				float totalWidth = this.progress.rectTransform.rect.width;
				float visibleWidth = this.visualProgressRange.rect.width;
				float margin = this.GetProgressMargin();
				result = baseFillAmount * (visibleWidth / totalWidth) + margin / totalWidth;
			}
			return result;
		}

		// Token: 0x06008A65 RID: 35429 RVA: 0x004012BC File Offset: 0x003FF4BC
		private void UpdateUiAppearanceBySkillDirection(CombatSkillKey skillKey)
		{
			CombatSubProcessorSkill skillProcessor;
			bool flag = !this.Model.ProcessorSkills.TryGetValue(skillKey, out skillProcessor);
			if (!flag)
			{
				string imageName = (skillProcessor.Direction == 1) ? "combat_progressbar_1" : "combat_progressbar_0";
				this.progress.SetSprite(imageName, false, null);
			}
		}

		// Token: 0x06008A66 RID: 35430 RVA: 0x00401310 File Offset: 0x003FF510
		public void Setup()
		{
			this._currentGangqi = -1;
			this._currentGangqiMax = -1;
			this._skipNextEffect = true;
			this.increaseParticle.gameObject.SetActive(false);
			this.decreaseParticle.gameObject.SetActive(false);
			this.SetupFillOrigin();
			CombatModel model = this.Model;
			model.OnGangqiChanged = (OnDataChangedEvent)Delegate.Combine(model.OnGangqiChanged, new OnDataChangedEvent(this.OnGangqiChanged));
			CombatModel model2 = this.Model;
			model2.OnGangqiMaxChanged = (OnDataChangedEvent)Delegate.Combine(model2.OnGangqiMaxChanged, new OnDataChangedEvent(this.OnGangqiMaxChanged));
			CombatModel model3 = this.Model;
			model3.OnCombatSkillDirectionChanged = (OnCombatSkillDataChangedEvent)Delegate.Combine(model3.OnCombatSkillDirectionChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillDirectionChanged));
			this.Model.AddEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
		}

		// Token: 0x06008A67 RID: 35431 RVA: 0x004013EC File Offset: 0x003FF5EC
		private void SetupFillOrigin()
		{
			this.progress.fillOrigin = (this.ally ? Image.OriginHorizontal.Left.ToInt() : Image.OriginHorizontal.Right.ToInt());
			this.progressReduceDelay.fillOrigin = (this.ally ? Image.OriginHorizontal.Left.ToInt() : Image.OriginHorizontal.Right.ToInt());
		}

		// Token: 0x06008A68 RID: 35432 RVA: 0x00401454 File Offset: 0x003FF654
		public void Close()
		{
			CombatModel model = this.Model;
			model.OnGangqiChanged = (OnDataChangedEvent)Delegate.Remove(model.OnGangqiChanged, new OnDataChangedEvent(this.OnGangqiChanged));
			CombatModel model2 = this.Model;
			model2.OnGangqiMaxChanged = (OnDataChangedEvent)Delegate.Remove(model2.OnGangqiMaxChanged, new OnDataChangedEvent(this.OnGangqiMaxChanged));
			CombatModel model3 = this.Model;
			model3.OnCombatSkillDirectionChanged = (OnCombatSkillDataChangedEvent)Delegate.Remove(model3.OnCombatSkillDirectionChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillDirectionChanged));
			this.Model.RemoveEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
			this.StopAllCustomUpdates();
		}

		// Token: 0x06008A69 RID: 35433 RVA: 0x004014F7 File Offset: 0x003FF6F7
		private void Awake()
		{
			this._rectTransform = base.GetComponent<RectTransform>();
			this._hidePos = this._rectTransform.anchoredPosition;
			this._canvasGroup = base.GetComponent<CanvasGroup>();
		}

		// Token: 0x06008A6A RID: 35434 RVA: 0x00401523 File Offset: 0x003FF723
		private void OnDisable()
		{
			this.StopAllCustomUpdates();
			this.ResetShow();
		}

		// Token: 0x06008A6B RID: 35435 RVA: 0x00401534 File Offset: 0x003FF734
		private void StopAllCustomUpdates()
		{
			this._reduceDelayStartTime = -1f;
			this._particleStartTime = -1f;
		}

		// Token: 0x06008A6C RID: 35436 RVA: 0x0040154D File Offset: 0x003FF74D
		private void Update()
		{
			this.UpdateDelayedReduce();
			this.UpdateParticleEffect();
		}

		// Token: 0x06008A6D RID: 35437 RVA: 0x00401560 File Offset: 0x003FF760
		private void UpdateDelayedReduce()
		{
			bool flag = this._reduceDelayStartTime < 0f;
			if (!flag)
			{
				float elapsedTime = Time.time - this._reduceDelayStartTime;
				bool flag2 = elapsedTime >= 0.5f;
				if (flag2)
				{
					this.progressReduceDelay.fillAmount = this._reduceDelayTargetFillAmount;
					this.UpdateProgressEndParticlePosition();
					this._reduceDelayStartTime = -1f;
				}
				else
				{
					float t = elapsedTime / 0.5f;
					float currentFillAmount = Mathf.Lerp(this._reduceDelayStartFillAmount, this._reduceDelayTargetFillAmount, t);
					this.progressReduceDelay.fillAmount = currentFillAmount;
					this.UpdateProgressEndParticlePosition();
				}
			}
		}

		// Token: 0x06008A6E RID: 35438 RVA: 0x004015F8 File Offset: 0x003FF7F8
		private void UpdateParticleEffect()
		{
			bool flag = this._particleStartTime < 0f;
			if (!flag)
			{
				float elapsedTime = Time.time - this._particleStartTime;
				bool flag2 = elapsedTime >= 0.5f;
				if (flag2)
				{
					int currentGangqi = this._currentGangqi;
					int startGangqi = this._particleStartGangqi;
					bool flag3 = currentGangqi != startGangqi;
					if (flag3)
					{
						this.PlayValueChangeEffect(currentGangqi > startGangqi);
					}
					else
					{
						this._particleStartTime = -1f;
					}
				}
			}
		}

		// Token: 0x06008A6F RID: 35439 RVA: 0x00401674 File Offset: 0x003FF874
		private void OnGangqiChanged(bool isAlly)
		{
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				this.UpdateGangqiByCharacter(charId);
			}
		}

		// Token: 0x06008A70 RID: 35440 RVA: 0x004016B8 File Offset: 0x003FF8B8
		private void OnGangqiMaxChanged(bool isAlly)
		{
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				this.UpdateGangqiMaxByCharacter(charId);
			}
		}

		// Token: 0x06008A71 RID: 35441 RVA: 0x004016FC File Offset: 0x003FF8FC
		private void OnChangeChar()
		{
			int currCharId = this.Model.ChangingToCharId;
			bool flag = this.ally != this.Model.CharIsAlly(currCharId);
			if (!flag)
			{
				this.RefreshByCharacter(currCharId);
			}
		}

		// Token: 0x06008A72 RID: 35442 RVA: 0x0040173C File Offset: 0x003FF93C
		private void OnCombatSkillDirectionChanged(CombatSkillKey combatSkillKey)
		{
			int currCharId = this.ally ? this.Model.SelfCharId : this.Model.EnemyCharId;
			bool flag = combatSkillKey.CharId != currCharId || combatSkillKey.SkillTemplateId != 34;
			if (!flag)
			{
				this.UpdateUiAppearanceBySkillDirection(combatSkillKey);
			}
		}

		// Token: 0x06008A73 RID: 35443 RVA: 0x00401794 File Offset: 0x003FF994
		private void RefreshByCharacter(int charId)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				int gangqi = processor.Gangqi;
				int gangqiMax = processor.GangqiMax;
				bool flag2 = gangqiMax <= 0;
				if (flag2)
				{
					base.gameObject.SetActive(false);
				}
				else
				{
					base.gameObject.SetActive(true);
					this.StartShow();
					this._currentGangqi = -1;
					this._currentGangqiMax = -1;
					CombatSkillKey skillKey = new CombatSkillKey(charId, 34);
					this.UpdateUiAppearanceBySkillDirection(skillKey);
					this.UpdateGangqiMax(gangqiMax);
					this.UpdateGangqi(gangqi, false);
				}
			}
		}

		// Token: 0x06008A74 RID: 35444 RVA: 0x00401830 File Offset: 0x003FFA30
		private void UpdateGangqiByCharacter(int charId)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				this.UpdateGangqi(processor.Gangqi, true);
			}
		}

		// Token: 0x06008A75 RID: 35445 RVA: 0x00401868 File Offset: 0x003FFA68
		private void UpdateGangqiMaxByCharacter(int charId)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				this.UpdateGangqiMax(processor.GangqiMax);
			}
		}

		// Token: 0x06008A76 RID: 35446 RVA: 0x004018A0 File Offset: 0x003FFAA0
		private void UpdateGangqi(int newGangqi, bool playEffect)
		{
			bool flag = this._currentGangqi == newGangqi;
			if (!flag)
			{
				int oldGangqi = this._currentGangqi;
				this._currentGangqi = newGangqi;
				float baseFillAmount = (this._currentGangqiMax > 0) ? ((float)newGangqi / (float)this._currentGangqiMax) : 0f;
				float fillAmount = this.CalculateAdjustedFillAmount(baseFillAmount);
				this.progress.fillAmount = fillAmount;
				this.valueLabel.text = string.Format("{0}%", Mathf.RoundToInt(baseFillAmount * 100f));
				bool flag2 = oldGangqi > newGangqi && oldGangqi >= 0;
				if (flag2)
				{
					this._reduceDelayStartTime = Time.time;
					this._reduceDelayStartFillAmount = this.progressReduceDelay.fillAmount;
					this._reduceDelayTargetFillAmount = fillAmount;
				}
				else
				{
					this.progressReduceDelay.fillAmount = fillAmount;
					this._reduceDelayStartTime = -1f;
				}
				this.UpdateProgressEndParticlePosition();
				bool flag3 = playEffect && oldGangqi >= 0;
				if (flag3)
				{
					bool skipNextEffect = this._skipNextEffect;
					if (skipNextEffect)
					{
						this._skipNextEffect = false;
					}
					else
					{
						bool isIncrease = newGangqi > oldGangqi;
						bool flag4 = this._particleStartTime < 0f || this._currentEffectIsIncrease != isIncrease;
						if (flag4)
						{
							this.PlayValueChangeEffect(isIncrease);
						}
					}
				}
				this.UpdateMouseTip();
			}
		}

		// Token: 0x06008A77 RID: 35447 RVA: 0x004019EC File Offset: 0x003FFBEC
		private void UpdateGangqiMax(int newGangqiMax)
		{
			bool flag = this._currentGangqiMax == newGangqiMax;
			if (!flag)
			{
				this._currentGangqiMax = newGangqiMax;
				bool flag2 = newGangqiMax <= 0;
				if (flag2)
				{
					base.gameObject.SetActive(false);
				}
				else
				{
					base.gameObject.SetActive(true);
					this.StartShow();
					float baseFillAmount = (float)this._currentGangqi / (float)newGangqiMax;
					float fillAmount = this.CalculateAdjustedFillAmount(baseFillAmount);
					this.progress.fillAmount = fillAmount;
					this.progressReduceDelay.fillAmount = fillAmount;
					this.valueLabel.text = string.Format("{0}%", Mathf.RoundToInt(baseFillAmount * 100f));
					this.UpdateProgressEndParticlePosition();
					this.UpdateMouseTip();
				}
			}
		}

		// Token: 0x06008A78 RID: 35448 RVA: 0x00401AA4 File Offset: 0x003FFCA4
		private void StartShow()
		{
			this._rectTransform.DOLocalMoveY(this._hidePos.y, this._showDuration, false);
			this._canvasGroup.DOFade(1f, this._showDuration);
		}

		// Token: 0x06008A79 RID: 35449 RVA: 0x00401ADC File Offset: 0x003FFCDC
		private void ResetShow()
		{
			this._rectTransform.anchoredPosition = this._hidePos;
			this._canvasGroup.alpha = 0f;
		}

		// Token: 0x06008A7A RID: 35450 RVA: 0x00401B04 File Offset: 0x003FFD04
		private void UpdateProgressEndParticlePosition()
		{
			bool flag = this.progressEndParticle == null || this.visualProgressRange == null;
			if (!flag)
			{
				float fillAmount = this.progressReduceDelay.fillAmount;
				float realFillAmountRate = this.CalculateAdjustedFillAmount(0.98f);
				bool flag2 = fillAmount >= realFillAmountRate;
				if (flag2)
				{
					this.progressEndParticle.gameObject.SetActive(false);
				}
				else
				{
					this.progressEndParticle.gameObject.SetActive(true);
					float width = this.progress.GetComponent<RectTransform>().rect.width;
					float xPos = this.ally ? ((1f - fillAmount) * width) : (fillAmount * width);
					RectTransform rectTransform = this.progressEndParticle;
					bool flag3 = rectTransform == null;
					if (!flag3)
					{
						Vector2 anchoredPosition = rectTransform.anchoredPosition;
						anchoredPosition.x = xPos;
						rectTransform.anchoredPosition = anchoredPosition;
					}
				}
			}
		}

		// Token: 0x06008A7B RID: 35451 RVA: 0x00401BEC File Offset: 0x003FFDEC
		private void PlayValueChangeEffect(bool isIncrease)
		{
			bool flag = this._particleStartTime >= 0f && this._currentEffectIsIncrease != isIncrease;
			if (flag)
			{
				UIParticle currentParticle = this._currentEffectIsIncrease ? this.increaseParticle : this.decreaseParticle;
				bool flag2 = currentParticle != null;
				if (flag2)
				{
					currentParticle.Stop();
					currentParticle.gameObject.SetActive(false);
				}
			}
			this._particleStartGangqi = this._currentGangqi;
			this._currentEffectIsIncrease = isIncrease;
			UIParticle particle = isIncrease ? this.increaseParticle : this.decreaseParticle;
			bool flag3 = particle != null;
			if (flag3)
			{
				particle.gameObject.SetActive(true);
				particle.Play();
			}
			this._particleStartTime = Time.time;
		}

		// Token: 0x06008A7C RID: 35452 RVA: 0x00401CA8 File Offset: 0x003FFEA8
		private void UpdateMouseTip()
		{
			bool flag = this.mouseTipDisplayer == null;
			if (!flag)
			{
				int charId = this.ally ? this.Model.SelfCharId : this.Model.EnemyCharId;
				CombatSkillKey skillKey = new CombatSkillKey(charId, 34);
				this.GetCombatDisplayDataAndUpdateMouseTip(skillKey);
			}
		}

		// Token: 0x06008A7D RID: 35453 RVA: 0x00401CFB File Offset: 0x003FFEFB
		private void GetCombatDisplayDataAndUpdateMouseTip(CombatSkillKey skillKey)
		{
			CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayDataOnce(null, skillKey.CharId, skillKey.SkillTemplateId, delegate(int offset, RawDataPool skillDataPool)
			{
				bool flag = !base.gameObject;
				if (!flag)
				{
					CombatSkillDisplayData combatSkillDisplayData = null;
					Serializer.Deserialize(skillDataPool, offset, ref combatSkillDisplayData);
					this.UpdateMouseTipAfter(combatSkillDisplayData);
				}
			});
		}

		// Token: 0x06008A7E RID: 35454 RVA: 0x00401D20 File Offset: 0x003FFF20
		private void UpdateMouseTipAfter(CombatSkillDisplayData combatSkillDisplayData)
		{
			bool flag = this.mouseTipDisplayer == null;
			if (!flag)
			{
				int charId = this.ally ? this.Model.SelfCharId : this.Model.EnemyCharId;
				CombatSkillKey skillKey = new CombatSkillKey(charId, 34);
				bool isReverse = false;
				CombatSubProcessorSkill skillProcessor;
				bool flag2 = this.Model.ProcessorSkills.TryGetValue(skillKey, out skillProcessor);
				if (flag2)
				{
					isReverse = (skillProcessor.Direction == 1);
				}
				this.mouseTipDisplayer.Type = TipType.GeneralLines;
				TooltipInvoker tooltipInvoker = this.mouseTipDisplayer;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				LanguageKey titleKey = isReverse ? LanguageKey.LK_Combat_GangqiTips_Title_Reverse : LanguageKey.LK_Combat_GangqiTips_Title_Direct;
				this.mouseTipDisplayer.RuntimeParam.Set("Title", LocalStringManager.Get(titleKey));
				int lineCount = 0;
				this.mouseTipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(11, new List<string>
				{
					CommonUtils.GetSpecialEffectDesc(combatSkillDisplayData)
				}, null));
				this.mouseTipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData
				{
					Type = 4,
					PreferredHeight = 10f
				});
				this.mouseTipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(1, new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_Combat_GangqiTips_SubTitle_1)
				}, null));
				int percentage = (this._currentGangqiMax > 0) ? Mathf.RoundToInt((float)this._currentGangqi / (float)this._currentGangqiMax * 100f) : 0;
				this.mouseTipDisplayer.RuntimeParam.SetObject(string.Format("LineData{0}", ++lineCount), new GeneralLineData(5, new List<string>
				{
					LocalStringManager.GetFormat(LanguageKey.LK_Combat_GangqiTips_Content_1_Value, percentage)
				}, new List<object>
				{
					20f
				}));
				this.mouseTipDisplayer.RuntimeParam.Set("LineCount", lineCount);
				this.mouseTipDisplayer.Refresh(false, -1);
			}
		}

		// Token: 0x04006A25 RID: 27173
		[SerializeField]
		private bool ally;

		// Token: 0x04006A26 RID: 27174
		[SerializeField]
		private CImage progressReduceDelay;

		// Token: 0x04006A27 RID: 27175
		[SerializeField]
		private CImage progress;

		// Token: 0x04006A28 RID: 27176
		[SerializeField]
		private TextMeshProUGUI valueLabel;

		// Token: 0x04006A29 RID: 27177
		[SerializeField]
		private RectTransform visualProgressRange;

		// Token: 0x04006A2A RID: 27178
		[SerializeField]
		private RectTransform progressEndParticle;

		// Token: 0x04006A2B RID: 27179
		[SerializeField]
		private UIParticle increaseParticle;

		// Token: 0x04006A2C RID: 27180
		[SerializeField]
		private UIParticle decreaseParticle;

		// Token: 0x04006A2D RID: 27181
		[SerializeField]
		private TooltipInvoker mouseTipDisplayer;

		// Token: 0x04006A2E RID: 27182
		private const string DirectProgressImage = "combat_progressbar_0";

		// Token: 0x04006A2F RID: 27183
		private const string ReverseProgressImage = "combat_progressbar_1";

		// Token: 0x04006A30 RID: 27184
		private const short RelatedSkillId = 34;

		// Token: 0x04006A31 RID: 27185
		private readonly float _showDuration = 0.3f;

		// Token: 0x04006A32 RID: 27186
		private const float ReduceDelayTime = 0.5f;

		// Token: 0x04006A33 RID: 27187
		private const float ValueChangeParticleLifetime = 0.5f;

		// Token: 0x04006A34 RID: 27188
		private int _currentGangqi = -1;

		// Token: 0x04006A35 RID: 27189
		private int _currentGangqiMax = -1;

		// Token: 0x04006A36 RID: 27190
		private float _reduceDelayStartTime = -1f;

		// Token: 0x04006A37 RID: 27191
		private float _reduceDelayStartFillAmount;

		// Token: 0x04006A38 RID: 27192
		private float _reduceDelayTargetFillAmount;

		// Token: 0x04006A39 RID: 27193
		private float _particleStartTime = -1f;

		// Token: 0x04006A3A RID: 27194
		private int _particleStartGangqi;

		// Token: 0x04006A3B RID: 27195
		private bool _currentEffectIsIncrease;

		// Token: 0x04006A3C RID: 27196
		private bool _skipNextEffect;

		// Token: 0x04006A3D RID: 27197
		private Vector2 _hidePos;

		// Token: 0x04006A3E RID: 27198
		private RectTransform _rectTransform;

		// Token: 0x04006A3F RID: 27199
		private CanvasGroup _canvasGroup;
	}
}
