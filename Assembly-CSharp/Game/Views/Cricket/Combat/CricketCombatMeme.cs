using System;
using System.Collections.Generic;
using DG.Tweening;
using FrameWork;
using GameData.Combat.Cricket;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000ACB RID: 2763
	public class CricketCombatMeme : MonoBehaviour, ICricketCombatComponent
	{
		// Token: 0x17000EFF RID: 3839
		// (get) Token: 0x0600882A RID: 34858 RVA: 0x003F3296 File Offset: 0x003F1496
		// (set) Token: 0x0600882B RID: 34859 RVA: 0x003F329E File Offset: 0x003F149E
		public ICricketCombatHandler Handler { get; set; }

		// Token: 0x0600882C RID: 34860 RVA: 0x003F32A7 File Offset: 0x003F14A7
		public void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox)
		{
		}

		// Token: 0x0600882D RID: 34861 RVA: 0x003F32AC File Offset: 0x003F14AC
		public ECricketCombatSequencePriority GetPriority(CricketCombatLog log)
		{
			return ECricketCombatSequencePriority.Meme;
		}

		// Token: 0x0600882E RID: 34862 RVA: 0x003F32C0 File Offset: 0x003F14C0
		public Sequence HandleLog(CricketCombatLog log, Sequence sequence)
		{
			bool flag = log.Type == ECricketCombatLogEventType.CombatStart;
			if (flag)
			{
				this._roundMemeCount = 0;
				this._selfMeme.Clear();
				this._enemyMeme.Clear();
			}
			CricketCombatLogDamage damage = log as CricketCombatLogDamage;
			bool flag2 = damage != null;
			if (flag2)
			{
				bool flag3 = damage.AttackStatus.Contains(ECricketCombatAttackStatus.Critical);
				if (flag3)
				{
					this.TryInvokeMeme(CricketCombatKit.Board.IsAlly(damage.Attacker.RuntimeId), ECricketAnim.CriticalHit);
				}
				bool flag4 = damage.AttackStatus.Contains(ECricketCombatAttackStatus.Parry);
				if (flag4)
				{
					this.TryInvokeMeme(CricketCombatKit.Board.IsAlly(damage.Defender.RuntimeId), ECricketAnim.Parry);
				}
				bool flag5 = damage.AttackStatus.Contains(ECricketCombatAttackStatus.Counter);
				if (flag5)
				{
					this.TryInvokeMeme(CricketCombatKit.Board.IsAlly(damage.Attacker.RuntimeId), ECricketAnim.CounterAttack);
				}
			}
			bool flag6 = log.Type == ECricketCombatLogEventType.RoundEnd;
			Sequence result;
			if (flag6)
			{
				result = this.InvokeMeme(sequence);
			}
			else
			{
				result = sequence;
			}
			return result;
		}

		// Token: 0x0600882F RID: 34863 RVA: 0x003F33C0 File Offset: 0x003F15C0
		private void TryInvokeMeme(bool ally, ECricketAnim anim)
		{
			bool flag = this._roundMemeCount >= 2 || !Utils_Random.RandomCheck(50, 100);
			if (!flag)
			{
				List<ECricketAnim> meme = ally ? this._selfMeme : this._enemyMeme;
				bool flag2 = meme.Contains(anim);
				if (!flag2)
				{
					meme.Add(anim);
				}
			}
		}

		// Token: 0x06008830 RID: 34864 RVA: 0x003F3414 File Offset: 0x003F1614
		private bool TryGetMeme(bool ally, out ECricketAnim anim)
		{
			anim = ECricketAnim.Count;
			List<ECricketAnim> meme = ally ? this._selfMeme : this._enemyMeme;
			bool flag = meme.Count == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				meme.Sort();
				anim = meme[0];
				meme.Clear();
				result = true;
			}
			return result;
		}

		// Token: 0x06008831 RID: 34865 RVA: 0x003F3468 File Offset: 0x003F1668
		private Sequence InvokeMeme(Sequence seq)
		{
			ECricketAnim selfMeme;
			bool selfHasMeme = this.TryGetMeme(true, out selfMeme);
			ECricketAnim enemyMeme;
			bool enemyHasMeme = this.TryGetMeme(false, out enemyMeme);
			bool flag = !selfHasMeme && !enemyHasMeme;
			Sequence result;
			if (flag)
			{
				result = seq;
			}
			else
			{
				if (seq == null)
				{
					seq = DOTween.Sequence();
				}
				this._roundMemeCount++;
				bool onlyOneMeme = selfHasMeme != enemyHasMeme;
				float selfMemeDuration = CricketCombatKit.Board.SelfCricketView.GetAnimationDuration(selfMeme);
				float enemyMemeDuration = CricketCombatKit.Board.EnemyCricketView.GetAnimationDuration(enemyMeme);
				float minMemeDuration = Mathf.Min(selfMemeDuration, enemyMemeDuration);
				float maxMemeDuration = Mathf.Max(selfMemeDuration, enemyMemeDuration);
				seq.AppendCallback(delegate
				{
					bool selfHasMeme;
					CricketCombatKit.Board.SelfCricketView.PlayAnimation(selfHasMeme ? selfMeme : ECricketAnim.Idle, false, false);
					selfHasMeme = selfHasMeme;
					if (selfHasMeme)
					{
						CricketCombatKit.Board.SelfCricketView.Sing(false, true, false, -1f, null, 0f);
					}
					bool enemyHasMeme;
					CricketCombatKit.Board.EnemyCricketView.PlayAnimation(enemyHasMeme ? enemyMeme : ECricketAnim.Idle, false, false);
					enemyHasMeme = enemyHasMeme;
					if (enemyHasMeme)
					{
						CricketCombatKit.Board.EnemyCricketView.Sing(false, true, false, -1f, null, 0f);
					}
				});
				bool flag2 = onlyOneMeme || Mathf.Approximately(minMemeDuration, maxMemeDuration);
				if (flag2)
				{
					seq.AppendInterval(maxMemeDuration);
				}
				else
				{
					seq.AppendInterval(minMemeDuration);
					seq.AppendCallback(delegate
					{
						bool flag3 = selfMemeDuration < enemyMemeDuration;
						if (flag3)
						{
							CricketCombatKit.Board.SelfCricketView.PlayAnimation(ECricketAnim.Idle, false, false);
						}
						else
						{
							CricketCombatKit.Board.EnemyCricketView.PlayAnimation(ECricketAnim.Idle, false, false);
						}
					});
					seq.AppendInterval(maxMemeDuration - minMemeDuration);
				}
				result = seq;
			}
			return result;
		}

		// Token: 0x0400685D RID: 26717
		private int _roundMemeCount;

		// Token: 0x0400685E RID: 26718
		private readonly List<ECricketAnim> _selfMeme = new List<ECricketAnim>();

		// Token: 0x0400685F RID: 26719
		private readonly List<ECricketAnim> _enemyMeme = new List<ECricketAnim>();
	}
}
