using System;
using System.Collections.Generic;
using GameData.Domains.Character;
using GameData.Domains.SpecialEffect;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B25 RID: 2853
	public class CombatCostNeiliAllocation : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F71 RID: 3953
		// (get) Token: 0x06008BFD RID: 35837 RVA: 0x0040B17A File Offset: 0x0040937A
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008BFE RID: 35838 RVA: 0x0040B184 File Offset: 0x00409384
		public void Setup()
		{
			CombatModel model = this.Model;
			model.OnNeiliAllocationChanged = (OnDataChangedEvent)Delegate.Combine(model.OnNeiliAllocationChanged, new OnDataChangedEvent(this.OnNeiliAllocationChanged));
			this.Model.AddEvent(ECombatEvents.OnGetAllCostNeiliEffectData, new OnCombatEvent(this.UpdateCostNeiliEffects));
			this.Model.AddEvent(ECombatEvents.OnAffectingDefendSkillIdChanged, new OnCombatEvent(this.UpdateCostNeiliEffects));
			CombatModel model2 = this.Model;
			model2.OnPreparingSkillIdChanged = (OnDataChangedEvent)Delegate.Combine(model2.OnPreparingSkillIdChanged, new OnDataChangedEvent(this.HideCostNeiliAllocation));
		}

		// Token: 0x06008BFF RID: 35839 RVA: 0x0040B214 File Offset: 0x00409414
		public void Close()
		{
			CombatModel model = this.Model;
			model.OnNeiliAllocationChanged = (OnDataChangedEvent)Delegate.Remove(model.OnNeiliAllocationChanged, new OnDataChangedEvent(this.OnNeiliAllocationChanged));
			this.Model.RemoveEvent(ECombatEvents.OnGetAllCostNeiliEffectData, new OnCombatEvent(this.UpdateCostNeiliEffects));
			this.Model.RemoveEvent(ECombatEvents.OnAffectingDefendSkillIdChanged, new OnCombatEvent(this.UpdateCostNeiliEffects));
			CombatModel model2 = this.Model;
			model2.OnPreparingSkillIdChanged = (OnDataChangedEvent)Delegate.Remove(model2.OnPreparingSkillIdChanged, new OnDataChangedEvent(this.HideCostNeiliAllocation));
		}

		// Token: 0x06008C00 RID: 35840 RVA: 0x0040B2A4 File Offset: 0x004094A4
		private void OnNeiliAllocationChanged(bool isAlly)
		{
			bool isCombatOver = this.Model.IsCombatOver;
			if (!isCombatOver)
			{
				this.UpdateCostNeiliEffects();
			}
		}

		// Token: 0x06008C01 RID: 35841 RVA: 0x0040B2CC File Offset: 0x004094CC
		private unsafe void UpdateCostNeiliEffects()
		{
			IReadOnlyList<CastBoostEffectDisplayData> costNeiliEffects = this.Model.CostNeiliEffects;
			base.gameObject.SetActive(this.Model.CanOperateSelfCharacter && costNeiliEffects.Count > 0 && this.Model.SelfCharacter.PreparingSkillId >= 0);
			bool flag = !base.gameObject.activeSelf;
			if (!flag)
			{
				for (int i = 0; i < base.transform.childCount; i++)
				{
					Transform costNeiliChild = base.transform.GetChild(i);
					costNeiliChild.gameObject.SetActive(i < costNeiliEffects.Count);
					bool flag2 = !costNeiliChild.gameObject.activeSelf;
					if (!flag2)
					{
						CastBoostEffectDisplayData effectData = costNeiliEffects[i];
						ECastBoostType type = effectData.Type;
						if (!true)
						{
						}
						int num;
						if (type != ECastBoostType.CostNeiliAllocation)
						{
							if (type != ECastBoostType.CostClearDefend)
							{
								num = 0;
							}
							else
							{
								num = (int)(DisorderLevelOfQi.MaxValue - this.Model.SelfCharacter.DisorderOfQi);
							}
						}
						else
						{
							num = (int)(*this.Model.SelfCharacter.NeiliAllocation[(int)effectData.NeiliAllocationType]);
						}
						if (!true)
						{
						}
						int current = num;
						ECastBoostType type2 = effectData.Type;
						if (!true)
						{
						}
						if (type2 != ECastBoostType.CostNeiliAllocation)
						{
							if (type2 != ECastBoostType.CostClearDefend)
							{
								num = 0;
							}
							else
							{
								num = effectData.AddQiDisorder;
							}
						}
						else
						{
							num = -effectData.NeiliAllocationValue;
						}
						if (!true)
						{
						}
						int cost = num;
						ECastBoostType type3 = effectData.Type;
						if (!true)
						{
						}
						bool flag3 = type3 != ECastBoostType.CostClearDefend || this.Model.EnemyCharacter.AffectingDefendSkillId >= 0;
						if (!true)
						{
						}
						bool extraCheck = flag3;
						CombatSpecialEffectCostNeiliAllocation costNeiliEffect = costNeiliChild.GetComponent<CombatSpecialEffectCostNeiliAllocation>();
						costNeiliEffect.Refresh(effectData, current, extraCheck, new CombatSpecialEffectCostNeiliAllocation.RequestCostNeiliEffectDelegate(this.DoRequestCostNeiliEffect));
						costNeiliEffect.ChangeInteractable(current >= cost && extraCheck);
					}
				}
			}
		}

		// Token: 0x06008C02 RID: 35842 RVA: 0x0040B4B4 File Offset: 0x004096B4
		private void HideCostNeiliAllocation(bool isAlly)
		{
			if (isAlly)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x06008C03 RID: 35843 RVA: 0x0040B4D4 File Offset: 0x004096D4
		private void DoRequestCostNeiliEffect(CastBoostEffectDisplayData effectData)
		{
			this.Model.DoRequestCostNeiliEffect((short)effectData.EffectId);
			this.Model.DoRequestGetAllCostNeiliEffectData();
		}
	}
}
