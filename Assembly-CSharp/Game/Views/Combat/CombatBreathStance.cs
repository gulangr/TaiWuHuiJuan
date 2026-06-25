using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AE7 RID: 2791
	public class CombatBreathStance : MonoBehaviour, ICombatComponent
	{
		// Token: 0x06008918 RID: 35096 RVA: 0x003F74A0 File Offset: 0x003F56A0
		public void Setup()
		{
			CombatModel model = this.Model;
			model.OnStanceValueChanged = (OnDataChangedEvent)Delegate.Combine(model.OnStanceValueChanged, new OnDataChangedEvent(this.OnStanceValueChanged));
			CombatModel model2 = this.Model;
			model2.OnBreathValueChanged = (OnDataChangedEvent)Delegate.Combine(model2.OnBreathValueChanged, new OnDataChangedEvent(this.OnBreathValueChanged));
			this.Model.AddEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
			this.Model.AddEvent(ECombatEvents.UpdateSkillCostPreview, new OnCombatEvent(this.OnUpdateSkillCostPreview));
			this._blockPercentage = 1f / (float)this.normalList.Count;
			this._normalProgressData = new CombatBreathStance.ProgressDisplayData(this.normalList)
			{
				speedPositive = 2f,
				speedNegative = 3f
			};
			this._previewProgressData = new CombatBreathStance.ProgressDisplayData(this.previewList)
			{
				speedPositive = 2f,
				speedNegative = 0.5f,
				displayOffset = -0.005f
			};
			this.UpdateProgress(this._normalProgressData, 0f, (float)this.MaxValue, true);
		}

		// Token: 0x06008919 RID: 35097 RVA: 0x003F75BC File Offset: 0x003F57BC
		public void Close()
		{
			CombatModel model = this.Model;
			model.OnStanceValueChanged = (OnDataChangedEvent)Delegate.Remove(model.OnStanceValueChanged, new OnDataChangedEvent(this.OnStanceValueChanged));
			CombatModel model2 = this.Model;
			model2.OnBreathValueChanged = (OnDataChangedEvent)Delegate.Remove(model2.OnBreathValueChanged, new OnDataChangedEvent(this.OnBreathValueChanged));
			this.Model.RemoveEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
			this.Model.RemoveEvent(ECombatEvents.UpdateSkillCostPreview, new OnCombatEvent(this.OnUpdateSkillCostPreview));
		}

		// Token: 0x17000F22 RID: 3874
		// (get) Token: 0x0600891A RID: 35098 RVA: 0x003F764B File Offset: 0x003F584B
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x17000F23 RID: 3875
		// (get) Token: 0x0600891B RID: 35099 RVA: 0x003F7654 File Offset: 0x003F5854
		private short MaxValue
		{
			get
			{
				CombatBreathStance.EBreathStanceType ebreathStanceType = this.type;
				if (!true)
				{
				}
				short result;
				if (ebreathStanceType != CombatBreathStance.EBreathStanceType.Breath)
				{
					if (ebreathStanceType != CombatBreathStance.EBreathStanceType.Stance)
					{
						throw new ArgumentOutOfRangeException();
					}
					result = 4000;
				}
				else
				{
					result = 30000;
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x17000F24 RID: 3876
		// (get) Token: 0x0600891C RID: 35100 RVA: 0x003F7694 File Offset: 0x003F5894
		private sbyte CostValue
		{
			get
			{
				CombatBreathStance.EBreathStanceType ebreathStanceType = this.type;
				if (!true)
				{
				}
				sbyte result;
				if (ebreathStanceType != CombatBreathStance.EBreathStanceType.Breath)
				{
					if (ebreathStanceType != CombatBreathStance.EBreathStanceType.Stance)
					{
						throw new ArgumentOutOfRangeException();
					}
					result = this.Model.PreviewCostSkillData.CostStance;
				}
				else
				{
					result = this.Model.PreviewCostSkillData.CostBreath;
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x17000F25 RID: 3877
		// (get) Token: 0x0600891D RID: 35101 RVA: 0x003F76E8 File Offset: 0x003F58E8
		private LanguageKey TitleKey
		{
			get
			{
				CombatBreathStance.EBreathStanceType ebreathStanceType = this.type;
				if (!true)
				{
				}
				LanguageKey result;
				if (ebreathStanceType != CombatBreathStance.EBreathStanceType.Breath)
				{
					if (ebreathStanceType != CombatBreathStance.EBreathStanceType.Stance)
					{
						throw new ArgumentOutOfRangeException();
					}
					result = LanguageKey.LK_CombatSkill_Stance;
				}
				else
				{
					result = LanguageKey.LK_CombatSkill_Breath;
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x17000F26 RID: 3878
		// (get) Token: 0x0600891E RID: 35102 RVA: 0x003F7728 File Offset: 0x003F5928
		private LanguageKey ContentKey1
		{
			get
			{
				CombatBreathStance.EBreathStanceType ebreathStanceType = this.type;
				if (!true)
				{
				}
				LanguageKey result;
				if (ebreathStanceType != CombatBreathStance.EBreathStanceType.Breath)
				{
					if (ebreathStanceType != CombatBreathStance.EBreathStanceType.Stance)
					{
						throw new ArgumentOutOfRangeException();
					}
					result = LanguageKey.LK_CombatSkill_Stance;
				}
				else
				{
					result = LanguageKey.LK_CombatSkill_Breath;
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x17000F27 RID: 3879
		// (get) Token: 0x0600891F RID: 35103 RVA: 0x003F7768 File Offset: 0x003F5968
		private LanguageKey ContentKey2
		{
			get
			{
				CombatBreathStance.EBreathStanceType ebreathStanceType = this.type;
				if (!true)
				{
				}
				LanguageKey result;
				if (ebreathStanceType != CombatBreathStance.EBreathStanceType.Breath)
				{
					if (ebreathStanceType != CombatBreathStance.EBreathStanceType.Stance)
					{
						throw new ArgumentOutOfRangeException();
					}
					result = LanguageKey.LK_Combat_Stance_Tips;
				}
				else
				{
					result = LanguageKey.LK_Combat_Breath_Tips;
				}
				if (!true)
				{
				}
				return result;
			}
		}

		// Token: 0x06008920 RID: 35104 RVA: 0x003F77A8 File Offset: 0x003F59A8
		private int GetCurrentValue(CombatSubProcessorCharacter processor)
		{
			CombatBreathStance.EBreathStanceType ebreathStanceType = this.type;
			if (!true)
			{
			}
			int result;
			if (ebreathStanceType != CombatBreathStance.EBreathStanceType.Breath)
			{
				if (ebreathStanceType != CombatBreathStance.EBreathStanceType.Stance)
				{
					throw new ArgumentOutOfRangeException();
				}
				result = processor.StanceValue;
			}
			else
			{
				result = processor.BreathValue;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008921 RID: 35105 RVA: 0x003F77F0 File Offset: 0x003F59F0
		private void OnChangeChar()
		{
			int currCharId = this.Model.ChangingToCharId;
			bool flag = this.Model.CharIsAlly(currCharId) != this.ally;
			if (!flag)
			{
				this.Refresh(currCharId);
			}
		}

		// Token: 0x06008922 RID: 35106 RVA: 0x003F7830 File Offset: 0x003F5A30
		private void OnUpdateSkillCostPreview()
		{
			bool flag = !this.ally;
			if (!flag)
			{
				CombatSubProcessorCharacter processor;
				bool flag2 = !this.Model.ProcessorCharacters.TryGetValue(this.Model.SelfCharId, out processor);
				if (!flag2)
				{
					int currValue = this.GetCurrentValue(processor);
					short maxValue = this.MaxValue;
					this.UpdateSkillCostPreview(this.Model.ShowSkillCostPreview, currValue, maxValue);
				}
			}
		}

		// Token: 0x06008923 RID: 35107 RVA: 0x003F7898 File Offset: 0x003F5A98
		private void OnStanceValueChanged(bool isAlly)
		{
			bool flag = this.ally != isAlly;
			if (!flag)
			{
				bool flag2 = this.type != CombatBreathStance.EBreathStanceType.Stance;
				if (!flag2)
				{
					int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
					this.Refresh(charId);
				}
			}
		}

		// Token: 0x06008924 RID: 35108 RVA: 0x003F78F0 File Offset: 0x003F5AF0
		private void OnBreathValueChanged(bool isAlly)
		{
			bool flag = this.ally != isAlly;
			if (!flag)
			{
				bool flag2 = this.type > CombatBreathStance.EBreathStanceType.Breath;
				if (!flag2)
				{
					int charId = isAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
					this.Refresh(charId);
				}
			}
		}

		// Token: 0x06008925 RID: 35109 RVA: 0x003F7944 File Offset: 0x003F5B44
		private void UpdateSkillCostPreview(bool showSkillCostPreview, int currValue, short maxValue)
		{
			bool flag = !showSkillCostPreview;
			if (flag)
			{
				this.UpdateAllProgress((float)currValue, (float)maxValue, false);
			}
			else
			{
				sbyte cost = this.CostValue;
				bool flag2 = cost <= 0;
				if (flag2)
				{
					this.UpdateAllProgress((float)currValue, (float)maxValue, false);
				}
				else
				{
					int costValue = (int)(maxValue * (short)cost / 100);
					int previewValue = currValue - costValue;
					this.UpdateProgress(this._normalProgressData, (float)previewValue, (float)maxValue, false);
					this.UpdateProgress(this._previewProgressData, (float)currValue, (float)maxValue, false);
					this.UpdateProgressNum();
				}
			}
		}

		// Token: 0x06008926 RID: 35110 RVA: 0x003F79C4 File Offset: 0x003F5BC4
		private void Refresh(int charId)
		{
			CombatSubProcessorCharacter processor;
			bool flag = !this.Model.ProcessorCharacters.TryGetValue(charId, out processor);
			if (!flag)
			{
				int currValue = this.GetCurrentValue(processor);
				short maxValue = this.MaxValue;
				bool showSkillCostPreview = this.Model.ShowSkillCostPreview;
				this.UpdateTips(currValue, (int)maxValue);
				bool flag2 = !this.ally || !showSkillCostPreview;
				if (flag2)
				{
					this.UpdateAllProgress((float)currValue, (float)maxValue, false);
				}
				else
				{
					this.UpdateSkillCostPreview(true, currValue, maxValue);
				}
			}
		}

		// Token: 0x06008927 RID: 35111 RVA: 0x003F7A44 File Offset: 0x003F5C44
		private void UpdateTips(int currValue, int maxValue)
		{
			string str = string.Format("{0}%", currValue * 100 / maxValue).SetColor("pinkyellow");
			this.tip.PresetParam[0] = LocalStringManager.Get(this.TitleKey);
			this.tip.PresetParam[1] = string.Concat(new string[]
			{
				LocalStringManager.Get(this.ContentKey1),
				LocalStringManager.Get(LanguageKey.LK_Colon_Symbol),
				str,
				"\n",
				LocalStringManager.Get(this.ContentKey2)
			});
			this.tip.Refresh(false, (this.type == CombatBreathStance.EBreathStanceType.Stance) ? 107 : 108);
		}

		// Token: 0x06008928 RID: 35112 RVA: 0x003F7AF3 File Offset: 0x003F5CF3
		private void UpdateAllProgress(float currValue, float maxValue, bool isInstant = false)
		{
			this.UpdateProgress(this._normalProgressData, currValue, maxValue, isInstant);
			this.UpdateProgress(this._previewProgressData, currValue, maxValue, isInstant);
			this.UpdateProgressNum();
		}

		// Token: 0x06008929 RID: 35113 RVA: 0x003F7B20 File Offset: 0x003F5D20
		private void UpdateProgress(CombatBreathStance.ProgressDisplayData progress, float currValue, float maxValue, bool isInstant = false)
		{
			float targetProgress = currValue / maxValue;
			progress.targetProgress = targetProgress;
			if (isInstant)
			{
				progress.curProgress = progress.targetProgress;
			}
			this.ApplyProgressDisplay(progress);
		}

		// Token: 0x0600892A RID: 35114 RVA: 0x003F7B58 File Offset: 0x003F5D58
		private void UpdateProgressNum()
		{
			float progress = Mathf.Max(this._normalProgressData.curProgress, this._previewProgressData.curProgress);
			this.progressValue.text = progress.ToString("P");
		}

		// Token: 0x0600892B RID: 35115 RVA: 0x003F7B9C File Offset: 0x003F5D9C
		private void ApplyProgressDisplay(CombatBreathStance.ProgressDisplayData displayer)
		{
			float progress = displayer.curProgress;
			List<ValueTuple<ParticleSystem, Material>> list = displayer.displayObjects;
			for (int i = 0; i < list.Count; i++)
			{
				float blockStart = (float)i * this._blockPercentage;
				float blockEnd = (float)(i + 1) * this._blockPercentage;
				bool flag = progress >= blockEnd;
				float amount;
				if (flag)
				{
					amount = 1f;
				}
				else
				{
					bool flag2 = progress <= blockStart;
					if (flag2)
					{
						amount = 0f;
					}
					else
					{
						amount = (progress - blockStart) / this._blockPercentage;
					}
				}
				float dissolveAmount = Mathf.Lerp(1f, -0.1f, amount + displayer.displayOffset);
				this.SetPSDissolveAmount(list[i].Item2, dissolveAmount);
			}
		}

		// Token: 0x0600892C RID: 35116 RVA: 0x003F7C56 File Offset: 0x003F5E56
		private void SetPSDissolveAmount(Material mat, float dissolveAmount)
		{
			mat.SetFloat(CombatBreathStance.DissolveProperty, dissolveAmount);
		}

		// Token: 0x0600892D RID: 35117 RVA: 0x003F7C68 File Offset: 0x003F5E68
		private void Update()
		{
			float deltaTime = Time.unscaledDeltaTime;
			bool needUpdate = false;
			needUpdate |= this._normalProgressData.UpdateProgress(deltaTime);
			needUpdate |= this._previewProgressData.UpdateProgress(deltaTime);
			bool flag = needUpdate;
			if (flag)
			{
				this.ApplyProgressDisplay(this._normalProgressData);
				this.ApplyProgressDisplay(this._previewProgressData);
				this.UpdateProgressNum();
			}
		}

		// Token: 0x04006911 RID: 26897
		[SerializeField]
		private List<ParticleSystem> normalList;

		// Token: 0x04006912 RID: 26898
		[SerializeField]
		private List<ParticleSystem> previewList;

		// Token: 0x04006913 RID: 26899
		[SerializeField]
		private TextMeshProUGUI progressValue;

		// Token: 0x04006914 RID: 26900
		[SerializeField]
		private TooltipInvoker tip;

		// Token: 0x04006915 RID: 26901
		[SerializeField]
		private bool ally;

		// Token: 0x04006916 RID: 26902
		[SerializeField]
		private CombatBreathStance.EBreathStanceType type;

		// Token: 0x04006917 RID: 26903
		private static readonly int DissolveProperty = Shader.PropertyToID("_rongjie");

		// Token: 0x04006918 RID: 26904
		private float _blockPercentage;

		// Token: 0x04006919 RID: 26905
		private CombatBreathStance.ProgressDisplayData _normalProgressData;

		// Token: 0x0400691A RID: 26906
		private CombatBreathStance.ProgressDisplayData _previewProgressData;

		// Token: 0x0400691B RID: 26907
		private const float ProgressAnimSpeed = 2f;

		// Token: 0x0400691C RID: 26908
		private const float ProgressAnimSpeedPreviewNagative = 0.5f;

		// Token: 0x0400691D RID: 26909
		private const float progressAnimSpeedNormalNagative = 3f;

		// Token: 0x0400691E RID: 26910
		private const float previewDissolveOffset = -0.005f;

		// Token: 0x020020AD RID: 8365
		private enum EBreathStanceType
		{
			// Token: 0x0400D1DF RID: 53727
			Breath,
			// Token: 0x0400D1E0 RID: 53728
			Stance
		}

		// Token: 0x020020AE RID: 8366
		private class ProgressDisplayData
		{
			// Token: 0x0600F7F2 RID: 63474 RVA: 0x0062AA58 File Offset: 0x00628C58
			public ProgressDisplayData(List<ParticleSystem> psList)
			{
				this.displayObjects = new List<ValueTuple<ParticleSystem, Material>>();
				foreach (ParticleSystem ps in psList)
				{
					Material mat = ps.GetComponent<ParticleSystemRenderer>().material;
					this.displayObjects.Add(new ValueTuple<ParticleSystem, Material>(ps, mat));
				}
			}

			// Token: 0x0600F7F3 RID: 63475 RVA: 0x0062AAF8 File Offset: 0x00628CF8
			public bool UpdateProgress(float deltaTime)
			{
				bool flag = this.curProgress < this.targetProgress;
				bool result;
				if (flag)
				{
					this.curProgress += this.speedPositive * deltaTime;
					bool flag2 = this.curProgress >= this.targetProgress;
					if (flag2)
					{
						this.curProgress = this.targetProgress;
					}
					result = true;
				}
				else
				{
					bool flag3 = this.curProgress > this.targetProgress;
					if (flag3)
					{
						this.curProgress -= this.speedNegative * deltaTime;
						bool flag4 = this.curProgress <= this.targetProgress;
						if (flag4)
						{
							this.curProgress = this.targetProgress;
						}
						result = true;
					}
					else
					{
						result = false;
					}
				}
				return result;
			}

			// Token: 0x0400D1E1 RID: 53729
			[TupleElementNames(new string[]
			{
				"ps",
				"mat"
			})]
			public readonly List<ValueTuple<ParticleSystem, Material>> displayObjects;

			// Token: 0x0400D1E2 RID: 53730
			public float curProgress;

			// Token: 0x0400D1E3 RID: 53731
			public float targetProgress;

			// Token: 0x0400D1E4 RID: 53732
			public float speedPositive = 0.5f;

			// Token: 0x0400D1E5 RID: 53733
			public float speedNegative = 0.5f;

			// Token: 0x0400D1E6 RID: 53734
			public float displayOffset = 0f;
		}
	}
}
