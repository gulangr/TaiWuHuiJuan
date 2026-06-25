using System;
using Config;
using DG.Tweening;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B02 RID: 2818
	[RequireComponent(typeof(CImage))]
	public class CombatBackgroundMask : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F4C RID: 3916
		// (get) Token: 0x06008AB2 RID: 35506 RVA: 0x00403096 File Offset: 0x00401296
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008AB3 RID: 35507 RVA: 0x004030A0 File Offset: 0x004012A0
		public void Setup()
		{
			CombatModel model = this.Model;
			model.OnPreparingSkillIdChanged = (OnDataChangedEvent)Delegate.Combine(model.OnPreparingSkillIdChanged, new OnDataChangedEvent(this.UpdateBackgroundMask));
			CombatModel model2 = this.Model;
			model2.OnPerformingSkillIdChanged = (OnDataChangedEvent)Delegate.Combine(model2.OnPerformingSkillIdChanged, new OnDataChangedEvent(this.UpdateBackgroundMask));
		}

		// Token: 0x06008AB4 RID: 35508 RVA: 0x004030FC File Offset: 0x004012FC
		public void Close()
		{
			CombatModel model = this.Model;
			model.OnPreparingSkillIdChanged = (OnDataChangedEvent)Delegate.Remove(model.OnPreparingSkillIdChanged, new OnDataChangedEvent(this.UpdateBackgroundMask));
			CombatModel model2 = this.Model;
			model2.OnPerformingSkillIdChanged = (OnDataChangedEvent)Delegate.Remove(model2.OnPerformingSkillIdChanged, new OnDataChangedEvent(this.UpdateBackgroundMask));
		}

		// Token: 0x06008AB5 RID: 35509 RVA: 0x00403158 File Offset: 0x00401358
		private void UpdateBackgroundMask(bool isAlly)
		{
			short selfPerformingSkillId = this.Model.SelfCharacter.PerformingSkillId;
			short enemyPerformingSkillId = this.Model.EnemyCharacter.PerformingSkillId;
			bool flag;
			if (selfPerformingSkillId >= 0)
			{
				CombatSkillItem combatSkillItem = CombatSkill.Instance[selfPerformingSkillId];
				if (combatSkillItem != null && combatSkillItem.EquipType == 1)
				{
					flag = true;
					goto IL_6A;
				}
			}
			if (enemyPerformingSkillId >= 0)
			{
				CombatSkillItem combatSkillItem = CombatSkill.Instance[enemyPerformingSkillId];
				flag = (combatSkillItem != null && combatSkillItem.EquipType == 1);
			}
			else
			{
				flag = false;
			}
			IL_6A:
			bool isAnyPreparingAttack = flag;
			CImage image = base.GetComponent<CImage>();
			bool flag2 = isAnyPreparingAttack != base.gameObject.activeSelf;
			if (flag2)
			{
				base.gameObject.SetActive(isAnyPreparingAttack);
				image.color = image.color.SetAlpha(0f);
			}
			bool activeSelf = base.gameObject.activeSelf;
			if (activeSelf)
			{
				image.DOKill(false);
				image.DOFade(0.25f, 0.3f);
			}
		}
	}
}
