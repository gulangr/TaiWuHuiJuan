using System;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Combat;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AFC RID: 2812
	public class CombatAttackRecovery : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F41 RID: 3905
		// (get) Token: 0x06008A59 RID: 35417 RVA: 0x0040109E File Offset: 0x003FF29E
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008A5A RID: 35418 RVA: 0x004010A5 File Offset: 0x003FF2A5
		public void Setup()
		{
			CombatModel model = this.Model;
			model.OnNormalAttackRecoveryChanged = (OnDataChangedEvent)Delegate.Combine(model.OnNormalAttackRecoveryChanged, new OnDataChangedEvent(this.OnNormalAttackRecoveryChanged));
		}

		// Token: 0x06008A5B RID: 35419 RVA: 0x004010CF File Offset: 0x003FF2CF
		public void Close()
		{
			CombatModel model = this.Model;
			model.OnNormalAttackRecoveryChanged = (OnDataChangedEvent)Delegate.Remove(model.OnNormalAttackRecoveryChanged, new OnDataChangedEvent(this.OnNormalAttackRecoveryChanged));
		}

		// Token: 0x06008A5C RID: 35420 RVA: 0x004010FC File Offset: 0x003FF2FC
		private void Awake()
		{
			CButton button = base.GetComponent<CButton>();
			bool flag = button != null && this.ally;
			if (flag)
			{
				button.ClearAndAddListener(new Action(CombatDomainMethod.Call.ClearReserveNormalAttack));
			}
		}

		// Token: 0x06008A5D RID: 35421 RVA: 0x0040113C File Offset: 0x003FF33C
		private void OnNormalAttackRecoveryChanged(bool isAlly)
		{
			bool flag = isAlly != this.ally;
			if (!flag)
			{
				this.UpdateStatus();
			}
		}

		// Token: 0x06008A5E RID: 35422 RVA: 0x00401164 File Offset: 0x003FF364
		private void UpdateStatus()
		{
			CombatSubProcessorCharacter processor = this.ally ? this.Model.SelfCharacter : this.Model.EnemyCharacter;
			bool flag = processor == null;
			if (!flag)
			{
				float amount = 1f - processor.NormalAttackRecovery.Progress;
				amount = Mathf.Clamp01(amount);
				this.fill.fillAmount = amount;
			}
		}

		// Token: 0x04006A23 RID: 27171
		[SerializeField]
		private bool ally;

		// Token: 0x04006A24 RID: 27172
		[SerializeField]
		private CImage fill;
	}
}
