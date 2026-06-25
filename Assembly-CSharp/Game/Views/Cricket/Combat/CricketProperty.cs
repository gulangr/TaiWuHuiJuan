using System;
using DG.Tweening;
using FrameWork;
using GameData.Combat.Cricket;
using TMPro;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AE0 RID: 2784
	public class CricketProperty : MonoBehaviour, ICricketCombatComponent
	{
		// Token: 0x17000F1E RID: 3870
		// (get) Token: 0x060088F9 RID: 35065 RVA: 0x003F69FF File Offset: 0x003F4BFF
		// (set) Token: 0x060088FA RID: 35066 RVA: 0x003F6A07 File Offset: 0x003F4C07
		public ICricketCombatHandler Handler { get; set; }

		// Token: 0x060088FB RID: 35067 RVA: 0x003F6A10 File Offset: 0x003F4C10
		private void OnEnable()
		{
			GEvent.Add(UiEvents.CricketCombatUpdateProperty, new GEvent.Callback(this.UpdateProperty));
		}

		// Token: 0x060088FC RID: 35068 RVA: 0x003F6A2F File Offset: 0x003F4C2F
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.CricketCombatUpdateProperty, new GEvent.Callback(this.UpdateProperty));
		}

		// Token: 0x060088FD RID: 35069 RVA: 0x003F6A50 File Offset: 0x003F4C50
		public void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox)
		{
			bool flag = type == ECricketCombatGlobalEventType.Initialize;
			if (flag)
			{
				this.DoReset();
			}
		}

		// Token: 0x060088FE RID: 35070 RVA: 0x003F6A70 File Offset: 0x003F4C70
		public Sequence HandleLog(CricketCombatLog log, Sequence sequence)
		{
			ECricketCombatLogEventType type = log.Type;
			bool flag = type == ECricketCombatLogEventType.CombatStart || type == ECricketCombatLogEventType.SkillPropertyModify;
			if (flag)
			{
				this.UpdateProperty(CricketCombatKit.Board.GetCricket(this.ally));
			}
			return sequence;
		}

		// Token: 0x060088FF RID: 35071 RVA: 0x003F6AB8 File Offset: 0x003F4CB8
		private void DoReset()
		{
			this.cricketName.text = string.Empty;
			this.vigor.text = string.Empty;
			this.strength.text = string.Empty;
			this.bite.text = string.Empty;
			this.hp.Clear();
			this.sp.Clear();
			this.durability.Clear();
		}

		// Token: 0x06008900 RID: 35072 RVA: 0x003F6B2E File Offset: 0x003F4D2E
		private void UpdateProperty(ArgumentBox argBox)
		{
			this.UpdateProperty(CricketCombatKit.Board.GetCricket(this.ally));
		}

		// Token: 0x06008901 RID: 35073 RVA: 0x003F6B48 File Offset: 0x003F4D48
		private void UpdateProperty(CricketCombatDisplayData displayData)
		{
			this.cricketName.text = displayData.Name;
			CricketCombatData data = displayData.Data;
			this.vigor.text = CricketCombatKit.WrapProperty(data.Vigor, data.Injury.Vigor);
			this.strength.text = CricketCombatKit.WrapProperty(data.Strength, data.Injury.Strength);
			this.bite.text = CricketCombatKit.WrapProperty(data.Bite, data.Injury.Bite);
			this.hp.Set(data.Hp, data.MaxHp, data.Injury.Hp);
			this.sp.Set(data.Sp, data.MaxSp, data.Injury.Sp);
			this.durability.Set(data.Durability, data.MaxDurability, data.DurabilityInjury);
		}

		// Token: 0x040068DB RID: 26843
		[SerializeField]
		private bool ally;

		// Token: 0x040068DC RID: 26844
		[SerializeField]
		private TextMeshProUGUI vigor;

		// Token: 0x040068DD RID: 26845
		[SerializeField]
		private TextMeshProUGUI strength;

		// Token: 0x040068DE RID: 26846
		[SerializeField]
		private TextMeshProUGUI bite;

		// Token: 0x040068DF RID: 26847
		[SerializeField]
		private CricketHealthBar hp;

		// Token: 0x040068E0 RID: 26848
		[SerializeField]
		private CricketHealthBar sp;

		// Token: 0x040068E1 RID: 26849
		[SerializeField]
		private CricketHealthBar durability;

		// Token: 0x040068E2 RID: 26850
		[SerializeField]
		private TextMeshProUGUI cricketName;
	}
}
