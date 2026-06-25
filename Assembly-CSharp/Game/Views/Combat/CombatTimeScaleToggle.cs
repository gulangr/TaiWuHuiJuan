using System;
using Game.Components.Combat;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AF5 RID: 2805
	[RequireComponent(typeof(CombatTimeScaleToggle))]
	public class CombatTimeScaleToggle : MonoBehaviour
	{
		// Token: 0x17000F35 RID: 3893
		// (get) Token: 0x060089F5 RID: 35317 RVA: 0x003FE480 File Offset: 0x003FC680
		private GlobalSettings SettingData
		{
			get
			{
				return SingletonObject.getInstance<GlobalSettings>();
			}
		}

		// Token: 0x17000F36 RID: 3894
		// (get) Token: 0x060089F6 RID: 35318 RVA: 0x003FE487 File Offset: 0x003FC687
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x17000F37 RID: 3895
		// (get) Token: 0x060089F7 RID: 35319 RVA: 0x003FE490 File Offset: 0x003FC690
		private CombatTimeScaleToggle Base
		{
			get
			{
				bool flag = this.baseToggle == null;
				if (flag)
				{
					this.baseToggle = base.GetComponent<CombatTimeScaleToggle>();
					this.SubscribeEvents();
				}
				return this.baseToggle;
			}
		}

		// Token: 0x17000F38 RID: 3896
		// (get) Token: 0x060089F8 RID: 35320 RVA: 0x003FE4CD File Offset: 0x003FC6CD
		public bool IsPaused
		{
			get
			{
				return this.Base.IsPaused;
			}
		}

		// Token: 0x17000F39 RID: 3897
		// (get) Token: 0x060089F9 RID: 35321 RVA: 0x003FE4DA File Offset: 0x003FC6DA
		// (set) Token: 0x060089FA RID: 35322 RVA: 0x003FE4E7 File Offset: 0x003FC6E7
		public bool PauseInteractable
		{
			get
			{
				return this.Base.PauseInteractable;
			}
			set
			{
				this.Base.PauseInteractable = value;
			}
		}

		// Token: 0x17000F3A RID: 3898
		// (get) Token: 0x060089FB RID: 35323 RVA: 0x003FE4F6 File Offset: 0x003FC6F6
		public float DisplayTimeScale
		{
			get
			{
				return this.Base.DisplayTimeScale;
			}
		}

		// Token: 0x17000F3B RID: 3899
		// (get) Token: 0x060089FC RID: 35324 RVA: 0x003FE503 File Offset: 0x003FC703
		public float CurrentTimeScale
		{
			get
			{
				return this.Base.CurrentTimeScale;
			}
		}

		// Token: 0x060089FD RID: 35325 RVA: 0x003FE510 File Offset: 0x003FC710
		private void SubscribeEvents()
		{
			bool flag = !this.eventsSubscribed && this.baseToggle != null;
			if (flag)
			{
				this.baseToggle.OnPauseChangedEvent += this.OnPauseChanged;
				this.baseToggle.OnDisplayTimeScaleChangedEvent += this.OnDisplayTimeScaleChanged;
				this.eventsSubscribed = true;
			}
		}

		// Token: 0x060089FE RID: 35326 RVA: 0x003FE574 File Offset: 0x003FC774
		private void OnDestroy()
		{
			bool flag = this.baseToggle != null;
			if (flag)
			{
				this.baseToggle.OnPauseChangedEvent -= this.OnPauseChanged;
				this.baseToggle.OnDisplayTimeScaleChangedEvent -= this.OnDisplayTimeScaleChanged;
			}
		}

		// Token: 0x060089FF RID: 35327 RVA: 0x003FE5C4 File Offset: 0x003FC7C4
		public void Setup(CombatConfigTips tips)
		{
			this.combatConfigTipsBack = tips;
		}

		// Token: 0x06008A00 RID: 35328 RVA: 0x003FE5CE File Offset: 0x003FC7CE
		public void SetPause(bool isPaused)
		{
			this.Base.SetPause(isPaused);
		}

		// Token: 0x06008A01 RID: 35329 RVA: 0x003FE5DD File Offset: 0x003FC7DD
		public void TogglePause()
		{
			this.Base.TogglePause();
		}

		// Token: 0x06008A02 RID: 35330 RVA: 0x003FE5EB File Offset: 0x003FC7EB
		public bool IncreaseSpeed()
		{
			return this.Base.IncreaseSpeed();
		}

		// Token: 0x06008A03 RID: 35331 RVA: 0x003FE5F8 File Offset: 0x003FC7F8
		public bool DecreaseSpeed()
		{
			return this.Base.DecreaseSpeed();
		}

		// Token: 0x06008A04 RID: 35332 RVA: 0x003FE605 File Offset: 0x003FC805
		public void SetDisplayTimeScale(float timeScale, bool init = false)
		{
			this.Base.SetDisplayTimeScale(timeScale, init);
		}

		// Token: 0x06008A05 RID: 35333 RVA: 0x003FE618 File Offset: 0x003FC818
		private void OnPauseChanged(bool isPaused)
		{
			CombatDomainMethod.Call.SetTimeScale(isPaused ? 0f : this.baseToggle.DisplayTimeScale);
			this.Model.SetPausing(isPaused);
			CharacterDisplayData enemyCharDisplayData;
			bool flag = isPaused && this.Model.DisplayDataCache.TryGetValue(this.Model.EnemyCharId, out enemyCharDisplayData);
			if (flag)
			{
				this.combatConfigTipsBack.SetTips((int)enemyCharDisplayData.TemplateId);
				this.combatConfigTipsBack.gameObject.SetActive(true);
			}
			else
			{
				this.combatConfigTipsBack.gameObject.SetActive(false);
			}
		}

		// Token: 0x06008A06 RID: 35334 RVA: 0x003FE6B0 File Offset: 0x003FC8B0
		private void OnDisplayTimeScaleChanged(float timeScale, bool init)
		{
			bool autoSaveCombatSpeed = this.SettingData.AutoSaveCombatSpeed;
			if (autoSaveCombatSpeed)
			{
				this.SettingData.CombatSpeed = timeScale;
			}
			bool flag = init || !this.baseToggle.IsPaused;
			if (flag)
			{
				CombatDomainMethod.Call.SetTimeScale(timeScale);
			}
		}

		// Token: 0x040069D9 RID: 27097
		private CombatTimeScaleToggle baseToggle;

		// Token: 0x040069DA RID: 27098
		private bool eventsSubscribed;

		// Token: 0x040069DB RID: 27099
		private CombatConfigTips combatConfigTipsBack;
	}
}
