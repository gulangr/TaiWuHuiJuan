using System;
using DG.Tweening;
using FrameWork;
using GameData.Combat.Cricket;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AC6 RID: 2758
	public class CricketCombatBubble : MonoBehaviour, ICricketCombatComponent
	{
		// Token: 0x17000EFC RID: 3836
		// (get) Token: 0x06008806 RID: 34822 RVA: 0x003F29DE File Offset: 0x003F0BDE
		// (set) Token: 0x06008807 RID: 34823 RVA: 0x003F29E6 File Offset: 0x003F0BE6
		public ICricketCombatHandler Handler { get; set; }

		// Token: 0x06008808 RID: 34824 RVA: 0x003F29F0 File Offset: 0x003F0BF0
		public void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox)
		{
			bool flag = type == ECricketCombatGlobalEventType.Initialize;
			if (flag)
			{
				this.HideAllBubbles();
			}
			else
			{
				bool flag2 = type == ECricketCombatGlobalEventType.ForceGiveUpCheck;
				if (flag2)
				{
					this.ShowBubbleText(true, LocalStringManager.Get(LanguageKey.LK_CricketCombat_Bubble_ForceGiveUp));
				}
				else
				{
					bool flag3 = type == ECricketCombatGlobalEventType.ForceGiveUpRefuse;
					if (flag3)
					{
						this.ShowBubbleText(false, LocalStringManager.Get(LanguageKey.LK_CricketCombat_Bubble_ForceGiveUp_Refuse));
					}
				}
			}
		}

		// Token: 0x06008809 RID: 34825 RVA: 0x003F2A48 File Offset: 0x003F0C48
		public Sequence HandleLog(CricketCombatLog log, Sequence sequence)
		{
			bool flag = log.Type == ECricketCombatLogEventType.CombatStart;
			if (flag)
			{
				this.ShowBubbleText(new Action<bool>(this.CombatStart));
			}
			bool flag2 = log.Type == ECricketCombatLogEventType.Damage;
			if (flag2)
			{
				this.ShowBubbleText(new Action<bool>(this.Damage));
			}
			bool flag3 = log.Type == ECricketCombatLogEventType.CombatEnd;
			if (flag3)
			{
				this.ShowBubbleText(new Action<bool>(this.CombatEnd));
			}
			return sequence;
		}

		// Token: 0x0600880A RID: 34826 RVA: 0x003F2ABB File Offset: 0x003F0CBB
		private void HideAllBubbles()
		{
			this.selfBubble.Hide();
			this.enemyBubble.Hide();
		}

		// Token: 0x0600880B RID: 34827 RVA: 0x003F2AD8 File Offset: 0x003F0CD8
		private void CombatStart(bool ally)
		{
			ref bool isHalfFallen = ref ally ? ref this._selfIsHalfFallen : ref this._enemyIsHalfFallen;
			isHalfFallen = CricketCombatKit.Board.GetCricket(ally).Data.IsHalfFail;
			this.ShowBubbleText(ally, ECricketWordType.BeforeCombat);
		}

		// Token: 0x0600880C RID: 34828 RVA: 0x003F2B18 File Offset: 0x003F0D18
		private void Damage(bool ally)
		{
			ref bool isHalfFallen = ref ally ? ref this._selfIsHalfFallen : ref this._enemyIsHalfFallen;
			bool newIsHalfFallen = CricketCombatKit.Board.GetCricket(ally).Data.IsHalfFail;
			bool flag = newIsHalfFallen == isHalfFallen;
			if (!flag)
			{
				isHalfFallen = newIsHalfFallen;
				this.ShowBubbleText(ally, ECricketWordType.HalfFallen);
			}
		}

		// Token: 0x0600880D RID: 34829 RVA: 0x003F2B68 File Offset: 0x003F0D68
		private void CombatEnd(bool ally)
		{
			bool anyFail = CricketCombatKit.Board.SelfCricket.Data.IsFail || CricketCombatKit.Board.EnemyCricket.Data.IsFail;
			bool flag = !anyFail;
			if (!flag)
			{
				CricketCombatDisplayData cricket = CricketCombatKit.Board.GetCricket(ally);
				bool win = CricketCombatKit.Board.IsWinner(cricket.Data.RuntimeId);
				bool die = CricketCombatKit.Board.SelfCricket.Data.IsDie || CricketCombatKit.Board.EnemyCricket.Data.IsDie;
				bool flag2 = win;
				if (!true)
				{
				}
				ECricketWordType ecricketWordType;
				if (!flag2)
				{
					if (!die)
					{
						ecricketWordType = ECricketWordType.AfterLose;
					}
					else
					{
						ecricketWordType = ECricketWordType.AfterDieLose;
					}
				}
				else if (!die)
				{
					ecricketWordType = ECricketWordType.AfterWin;
				}
				else
				{
					ecricketWordType = ECricketWordType.AfterDieWin;
				}
				if (!true)
				{
				}
				ECricketWordType type = ecricketWordType;
				this.ShowBubbleText(ally, type);
			}
		}

		// Token: 0x0600880E RID: 34830 RVA: 0x003F2C40 File Offset: 0x003F0E40
		private void ShowBubbleText(bool ally, ECricketWordType type)
		{
			bool flag = !Utils_Random.RandomCheck(50, 100);
			if (!flag)
			{
				CharacterDisplayData charData = ally ? CricketCombatKit.Board.SelfChar : CricketCombatKit.Board.EnemyChar;
				string key = string.Format("LK_CricketCombat_Bubble_{0}_{1}", type, charData.BehaviorType.ToString());
				this.ShowBubbleText(ally, LocalStringManager.Get(key));
			}
		}

		// Token: 0x0600880F RID: 34831 RVA: 0x003F2CA4 File Offset: 0x003F0EA4
		private void ShowBubbleText(Action<bool> callback)
		{
			callback(true);
			callback(false);
		}

		// Token: 0x06008810 RID: 34832 RVA: 0x003F2CB8 File Offset: 0x003F0EB8
		private void ShowBubbleText(bool ally, string text)
		{
			Bubble bubble = ally ? this.selfBubble : this.enemyBubble;
			bubble.SetText(text, true);
			CricketCombatKit.DelayCallRealTime(this, new Action(bubble.Hide), 6f);
		}

		// Token: 0x0400684D RID: 26701
		[SerializeField]
		private Bubble selfBubble;

		// Token: 0x0400684E RID: 26702
		[SerializeField]
		private Bubble enemyBubble;

		// Token: 0x0400684F RID: 26703
		private bool _selfIsHalfFallen;

		// Token: 0x04006850 RID: 26704
		private bool _enemyIsHalfFallen;
	}
}
