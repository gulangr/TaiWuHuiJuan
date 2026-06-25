using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Combat
{
	// Token: 0x02000B0C RID: 2828
	public class CombatPlayground : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F56 RID: 3926
		// (get) Token: 0x06008B10 RID: 35600 RVA: 0x0040591D File Offset: 0x00403B1D
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008B11 RID: 35601 RVA: 0x00405924 File Offset: 0x00403B24
		public void Setup()
		{
			this.Model.AddEvent(ECombatEvents.OnIsPlaygroundCombatChanged, new OnCombatEvent(this.OnIsPlaygroundCombatChanged));
			this.Model.AddEvent(ECombatEvents.OnDisableEnemyAiChanged, new OnCombatEvent(this.OnDisableEnemyAiChanged));
			this.Model.AddEvent(ECombatEvents.OnEnemyUnyieldingFallenChanged, new OnCombatEvent(this.OnEnemyUnyieldingFallenChanged));
		}

		// Token: 0x06008B12 RID: 35602 RVA: 0x00405980 File Offset: 0x00403B80
		public void Close()
		{
			this.Model.RemoveEvent(ECombatEvents.OnIsPlaygroundCombatChanged, new OnCombatEvent(this.OnIsPlaygroundCombatChanged));
			this.Model.RemoveEvent(ECombatEvents.OnDisableEnemyAiChanged, new OnCombatEvent(this.OnDisableEnemyAiChanged));
			this.Model.RemoveEvent(ECombatEvents.OnEnemyUnyieldingFallenChanged, new OnCombatEvent(this.OnEnemyUnyieldingFallenChanged));
		}

		// Token: 0x06008B13 RID: 35603 RVA: 0x004059DC File Offset: 0x00403BDC
		private void OnIsPlaygroundCombatChanged()
		{
			base.gameObject.SetActive(this.Model.IsPlaygroundCombat);
			bool isPlaygroundCombat = this.Model.IsPlaygroundCombat;
			if (isPlaygroundCombat)
			{
				this._processorPlayground.Setup();
			}
			else
			{
				this._processorPlayground.Close();
			}
		}

		// Token: 0x06008B14 RID: 35604 RVA: 0x00405A2C File Offset: 0x00403C2C
		private void OnDisableEnemyAiChanged()
		{
			CToggle toggle = this.disableEnemyAiToggle;
			toggle.onValueChanged.RemoveAllListeners();
			toggle.isOn = this._processorPlayground.DisableEnemyAi;
			toggle.onValueChanged.AddListener(new UnityAction<bool>(this.Model.RequestSetPuppetDisableAi));
		}

		// Token: 0x06008B15 RID: 35605 RVA: 0x00405A7C File Offset: 0x00403C7C
		private void OnEnemyUnyieldingFallenChanged()
		{
			CToggle toggle = this.enemyUnyieldingFallenToggle;
			toggle.onValueChanged.RemoveAllListeners();
			toggle.isOn = this._processorPlayground.EnemyUnyieldingFallen;
			toggle.onValueChanged.AddListener(new UnityAction<bool>(this.Model.RequestSetPuppetUnyieldingFallen));
		}

		// Token: 0x04006AB1 RID: 27313
		[SerializeField]
		private CToggle disableEnemyAiToggle;

		// Token: 0x04006AB2 RID: 27314
		[SerializeField]
		private CToggle enemyUnyieldingFallenToggle;

		// Token: 0x04006AB3 RID: 27315
		private readonly CombatSubProcessorPlayground _processorPlayground = new CombatSubProcessorPlayground();
	}
}
