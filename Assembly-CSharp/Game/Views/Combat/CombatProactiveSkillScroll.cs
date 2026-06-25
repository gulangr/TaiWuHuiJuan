using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.UISystem.UIElements;
using GameData.Domains.CombatSkill;
using GameData.Domains.Global;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AE9 RID: 2793
	public class CombatProactiveSkillScroll : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F2B RID: 3883
		// (get) Token: 0x0600893F RID: 35135 RVA: 0x003F7FBD File Offset: 0x003F61BD
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008940 RID: 35136 RVA: 0x003F7FC4 File Offset: 0x003F61C4
		public void Setup()
		{
			this._skillScroll = base.GetComponent<CScrollRect>();
			this.Model.OnGetProactiveSkillList += this.OnGetProactiveSkillList;
			CombatModel model = this.Model;
			model.OnCombatSkillCanUseChanged = (OnCombatSkillDataChangedEvent)Delegate.Combine(model.OnCombatSkillCanUseChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillCanUseChanged));
			CombatModel model2 = this.Model;
			model2.OnCombatSkillLeftCdFrameChanged = (OnCombatSkillDataChangedEvent)Delegate.Combine(model2.OnCombatSkillLeftCdFrameChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillCdFrameChanged));
			CombatModel model3 = this.Model;
			model3.OnCombatSkillTotalCdFrameChanged = (OnCombatSkillDataChangedEvent)Delegate.Combine(model3.OnCombatSkillTotalCdFrameChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillCdFrameChanged));
			CombatModel model4 = this.Model;
			model4.OnCombatSkillEffectDataChanged = (OnCombatSkillDataChangedEvent)Delegate.Combine(model4.OnCombatSkillEffectDataChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillEffectDataChanged));
			CombatModel model5 = this.Model;
			model5.OnCombatSkillBanReasonChanged = (OnCombatSkillDataChangedEvent)Delegate.Combine(model5.OnCombatSkillBanReasonChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillBanReasonChanged));
			this.Model.AddEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
		}

		// Token: 0x06008941 RID: 35137 RVA: 0x003F80D4 File Offset: 0x003F62D4
		public void Close()
		{
			this.Model.OnGetProactiveSkillList -= this.OnGetProactiveSkillList;
			CombatModel model = this.Model;
			model.OnCombatSkillCanUseChanged = (OnCombatSkillDataChangedEvent)Delegate.Remove(model.OnCombatSkillCanUseChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillCanUseChanged));
			CombatModel model2 = this.Model;
			model2.OnCombatSkillLeftCdFrameChanged = (OnCombatSkillDataChangedEvent)Delegate.Remove(model2.OnCombatSkillLeftCdFrameChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillCdFrameChanged));
			CombatModel model3 = this.Model;
			model3.OnCombatSkillTotalCdFrameChanged = (OnCombatSkillDataChangedEvent)Delegate.Remove(model3.OnCombatSkillTotalCdFrameChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillCdFrameChanged));
			CombatModel model4 = this.Model;
			model4.OnCombatSkillEffectDataChanged = (OnCombatSkillDataChangedEvent)Delegate.Remove(model4.OnCombatSkillEffectDataChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillEffectDataChanged));
			CombatModel model5 = this.Model;
			model5.OnCombatSkillBanReasonChanged = (OnCombatSkillDataChangedEvent)Delegate.Remove(model5.OnCombatSkillBanReasonChanged, new OnCombatSkillDataChangedEvent(this.OnCombatSkillBanReasonChanged));
			this.Model.RemoveEvent(ECombatEvents.ChangeChar, new OnCombatEvent(this.OnChangeChar));
		}

		// Token: 0x06008942 RID: 35138 RVA: 0x003F81D8 File Offset: 0x003F63D8
		private void OnChangeChar()
		{
			int currCharId = this.Model.ChangingToCharId;
			bool flag = !this.Model.CharIsAlly(currCharId);
			if (!flag)
			{
				for (int i = 0; i < this._skillScroll.Content.childCount; i++)
				{
					CombatProactiveSkillView skillView = this.GetProactiveSkillViewByIndex(i);
					bool activeSelf = skillView.gameObject.activeSelf;
					if (activeSelf)
					{
						skillView.SetInteractable(false);
					}
				}
			}
		}

		// Token: 0x06008943 RID: 35139 RVA: 0x003F824B File Offset: 0x003F644B
		public void InitCallbacks(CombatProactiveSkillScroll.ProactiveCombatSkillViewEvent onClickSkill, CombatProactiveSkillScroll.ProactiveCombatSkillViewEvent onClickJumpSetting, CombatProactiveSkillScroll.ProactiveCombatSkillViewEvent onMouseEnterSkill, CombatProactiveSkillScroll.ProactiveCombatSkillViewEvent onMouseExitSkill)
		{
			this._onClickSkill = onClickSkill;
			this._onMouseEnterSkill = onMouseEnterSkill;
			this._onMouseExitSkill = onMouseExitSkill;
			this._onClickJumpSetting = onClickJumpSetting;
		}

		// Token: 0x06008944 RID: 35140 RVA: 0x003F826C File Offset: 0x003F646C
		public CombatProactiveSkillView GetProactiveSkillView(short skillId)
		{
			List<short> proactiveSkillList = this.Model.OrderedProactiveSkillList[this.Model.SelfCharId];
			bool flag = !proactiveSkillList.Contains(skillId);
			CombatProactiveSkillView result;
			if (flag)
			{
				result = null;
			}
			else
			{
				int index = proactiveSkillList.IndexOf(skillId);
				result = this.GetProactiveSkillViewByIndex(index);
			}
			return result;
		}

		// Token: 0x06008945 RID: 35141 RVA: 0x003F82BC File Offset: 0x003F64BC
		public void SetSkillViewByIndex(int index, CombatSkillDisplayData skillData)
		{
			CombatProactiveSkillView skillView = this.GetProactiveSkillViewByIndex(index);
			this.SetData(skillView, skillData);
			skillView.gameObject.SetActive(skillData != null);
		}

		// Token: 0x06008946 RID: 35142 RVA: 0x003F82EC File Offset: 0x003F64EC
		public void OnRequestCastSkill(int index)
		{
			CombatProactiveSkillView skillView = this.GetProactiveSkillViewByIndex(index);
			CombatUtils.PlayAndHideParticle(skillView.particle, 2f);
		}

		// Token: 0x06008947 RID: 35143 RVA: 0x003F8314 File Offset: 0x003F6514
		private void OnGetProactiveSkillList(int charId)
		{
			bool flag = charId != this.Model.SelfCharId;
			if (!flag)
			{
				IReadOnlyList<CombatSkillDisplayData> displayDataList = this.Model.ProactiveSkillData[charId];
				int skillCount = displayDataList.Count;
				List<short> orderedProactiveSkill = this.Model.OrderedProactiveSkillList[charId];
				bool isMainChar = charId == this.Model.SelfTeam[0];
				ShortList orderPlan;
				List<short> skillOrderPlan = this.Model.CombatSkillOrderPlans.TryGetValue(this.Model.CurrCombatSkillPlanId, out orderPlan) ? orderPlan.Items : null;
				GameObject skillPrefab = this._skillScroll.Content.GetChild(0).gameObject;
				CommonUtils.PrepareEnoughChildren(this._skillScroll.Content, skillPrefab, 54, null);
				bool flag2 = isMainChar && skillOrderPlan == null;
				if (flag2)
				{
					skillOrderPlan = new List<short>();
					for (int i = 0; i < 54; i++)
					{
						skillOrderPlan.Add((i < skillCount) ? displayDataList[i].TemplateId : -1);
					}
				}
				for (int j = 0; j < 54; j++)
				{
					short skillId = orderedProactiveSkill[j];
					Transform slotTransform = this._skillScroll.Content.GetChild(j);
					CombatProactiveSkillView skillView = slotTransform.GetChild(0).GetComponent<CombatProactiveSkillView>();
					slotTransform.gameObject.SetActive(true);
					CombatSkillDisplayData skillData2 = displayDataList.FirstOrDefault((CombatSkillDisplayData skillData) => skillData.TemplateId == skillId);
					skillView.gameObject.SetActive(skillData2 != null);
					bool flag3 = skillData2 != null;
					if (flag3)
					{
						this.SetData(skillView, skillData2);
					}
				}
				for (int k = 54; k < this._skillScroll.Content.childCount; k++)
				{
					this._skillScroll.Content.GetChild(k).gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06008948 RID: 35144 RVA: 0x003F8518 File Offset: 0x003F6718
		private void SetData(CombatProactiveSkillView skillView, CombatSkillDisplayData skillData)
		{
			skillView.SetData(skillData, delegate
			{
				CombatProactiveSkillScroll.ProactiveCombatSkillViewEvent onClickSkill = this._onClickSkill;
				if (onClickSkill != null)
				{
					onClickSkill(skillView, skillData);
				}
			}, delegate
			{
				CombatProactiveSkillScroll.ProactiveCombatSkillViewEvent onClickJumpSetting = this._onClickJumpSetting;
				if (onClickJumpSetting != null)
				{
					onClickJumpSetting(skillView, skillData);
				}
			}, delegate
			{
				CombatProactiveSkillScroll.ProactiveCombatSkillViewEvent onMouseEnterSkill = this._onMouseEnterSkill;
				if (onMouseEnterSkill != null)
				{
					onMouseEnterSkill(skillView, skillData);
				}
			}, delegate
			{
				CombatProactiveSkillScroll.ProactiveCombatSkillViewEvent onMouseExitSkill = this._onMouseExitSkill;
				if (onMouseExitSkill != null)
				{
					onMouseExitSkill(skillView, skillData);
				}
			});
			CombatSkillKey combatSkillKey = new CombatSkillKey(this.Model.SelfCharId, skillData.TemplateId);
			CombatSubProcessorSkill processorSkill;
			bool flag = !this.Model.ProcessorSkills.TryGetValue(combatSkillKey, out processorSkill);
			if (!flag)
			{
				bool canUse = processorSkill.CanUse;
				skillView.SetInteractable(canUse && this.Model.CanOperateSelfCharacter);
			}
		}

		// Token: 0x06008949 RID: 35145 RVA: 0x003F85E4 File Offset: 0x003F67E4
		private void OnCombatSkillCanUseChanged(CombatSkillKey combatSkillKey)
		{
			bool flag = CombatSkillEquipType.IsAssist(combatSkillKey.SkillTemplateId);
			if (!flag)
			{
				CombatSubProcessorSkill processorSkill;
				bool flag2 = !this.Model.ProcessorSkills.TryGetValue(combatSkillKey, out processorSkill);
				if (!flag2)
				{
					int charId = combatSkillKey.CharId;
					bool flag3 = charId != this.Model.SelfCharId;
					if (!flag3)
					{
						short skillId = combatSkillKey.SkillTemplateId;
						CombatProactiveSkillView skillRefers = this.GetProactiveSkillView(skillId);
						bool flag4 = skillRefers == null;
						if (!flag4)
						{
							bool canUse = processorSkill.CanUse;
							bool interactable = canUse && this.Model.CanOperateSelfCharacter;
							skillRefers.GetComponent<CombatProactiveSkillView>().SetInteractable(interactable);
							bool flag5 = !interactable;
							if (!flag5)
							{
								bool flag6 = CombatSkillEquipType.IsAttack(combatSkillKey.SkillTemplateId);
								if (flag6)
								{
									GlobalDomainMethod.Call.InvokeGuidingTrigger(146);
								}
								bool flag7 = CombatSkillEquipType.IsAgile(combatSkillKey.SkillTemplateId) || CombatSkillType.IsLeg(combatSkillKey.SkillTemplateId);
								if (flag7)
								{
									GlobalDomainMethod.Call.InvokeGuidingTrigger(155);
								}
								else
								{
									GlobalDomainMethod.Call.InvokeGuidingTrigger(273);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600894A RID: 35146 RVA: 0x003F86FC File Offset: 0x003F68FC
		private void OnCombatSkillCdFrameChanged(CombatSkillKey combatSkillKey)
		{
			bool flag = CombatSkillEquipType.IsAssist(combatSkillKey.SkillTemplateId);
			if (!flag)
			{
				CombatProactiveSkillView proactiveSkillView = this.GetProactiveSkillView(combatSkillKey.SkillTemplateId);
				bool flag2 = proactiveSkillView == null;
				if (!flag2)
				{
					proactiveSkillView.UpdateSkillCdAndLock(combatSkillKey.SkillTemplateId);
				}
			}
		}

		// Token: 0x0600894B RID: 35147 RVA: 0x003F8744 File Offset: 0x003F6944
		private void OnCombatSkillEffectDataChanged(CombatSkillKey combatSkillKey)
		{
			bool flag = CombatSkillEquipType.IsAssist(combatSkillKey.SkillTemplateId);
			if (!flag)
			{
				CombatProactiveSkillView skillRefers = this.GetProactiveSkillView(combatSkillKey.SkillTemplateId);
				bool flag2 = skillRefers == null;
				if (!flag2)
				{
					skillRefers.UpdateMouseTip();
				}
			}
		}

		// Token: 0x0600894C RID: 35148 RVA: 0x003F8784 File Offset: 0x003F6984
		private void OnCombatSkillBanReasonChanged(CombatSkillKey combatSkillKey)
		{
			bool flag = CombatSkillEquipType.IsAssist(combatSkillKey.SkillTemplateId);
			if (!flag)
			{
				CombatProactiveSkillView skillView = this.GetProactiveSkillView(combatSkillKey.SkillTemplateId);
				bool flag2 = skillView == null;
				if (!flag2)
				{
					skillView.UpdateMouseTip();
					skillView.SetBodyPartBroken();
				}
			}
		}

		// Token: 0x0600894D RID: 35149 RVA: 0x003F87CC File Offset: 0x003F69CC
		private CombatProactiveSkillView GetProactiveSkillViewByIndex(int index)
		{
			return this._skillScroll.Content.GetChild(index).GetChild(0).GetComponent<CombatProactiveSkillView>();
		}

		// Token: 0x0400692D RID: 26925
		private CScrollRect _skillScroll;

		// Token: 0x0400692E RID: 26926
		private CombatProactiveSkillScroll.ProactiveCombatSkillViewEvent _onClickSkill;

		// Token: 0x0400692F RID: 26927
		private CombatProactiveSkillScroll.ProactiveCombatSkillViewEvent _onMouseEnterSkill;

		// Token: 0x04006930 RID: 26928
		private CombatProactiveSkillScroll.ProactiveCombatSkillViewEvent _onMouseExitSkill;

		// Token: 0x04006931 RID: 26929
		private CombatProactiveSkillScroll.ProactiveCombatSkillViewEvent _onClickJumpSetting;

		// Token: 0x020020AF RID: 8367
		// (Invoke) Token: 0x0600F7F5 RID: 63477
		public delegate void ProactiveCombatSkillViewEvent(CombatProactiveSkillView view, CombatSkillDisplayData skillData);
	}
}
