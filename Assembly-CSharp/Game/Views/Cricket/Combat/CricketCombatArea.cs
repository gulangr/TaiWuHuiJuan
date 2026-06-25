using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Combat.Cricket;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AC2 RID: 2754
	public class CricketCombatArea : MonoBehaviour, ICricketCombatComponent
	{
		// Token: 0x17000EE5 RID: 3813
		// (get) Token: 0x060087A8 RID: 34728 RVA: 0x003F0E97 File Offset: 0x003EF097
		// (set) Token: 0x060087A9 RID: 34729 RVA: 0x003F0E9F File Offset: 0x003EF09F
		public ICricketCombatHandler Handler { get; set; }

		// Token: 0x060087AA RID: 34730 RVA: 0x003F0EA8 File Offset: 0x003EF0A8
		public void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox)
		{
			bool flag = type == ECricketCombatGlobalEventType.Initialize;
			if (flag)
			{
				this.DoReset();
			}
			bool flag2 = type == ECricketCombatGlobalEventType.PauseStateChanged;
			if (flag2)
			{
				this.OnPauseStateChanged(argBox);
			}
			bool flag3 = type == ECricketCombatGlobalEventType.SkillResolved;
			if (flag3)
			{
				this.OnSkillResolved();
			}
		}

		// Token: 0x060087AB RID: 34731 RVA: 0x003F0EE8 File Offset: 0x003EF0E8
		public Sequence HandleLog(CricketCombatLog log, Sequence sequence)
		{
			ECricketCombatLogEventType type = log.Type;
			if (!true)
			{
			}
			Sequence result;
			switch (type)
			{
			case ECricketCombatLogEventType.CombatStart:
				result = this.CombatStart(sequence);
				goto IL_7B;
			case ECricketCombatLogEventType.CheckWithoutFight:
				result = this.CheckWithoutFight(log, sequence);
				goto IL_7B;
			case ECricketCombatLogEventType.RoundStart:
				result = this.RoundStart(sequence);
				goto IL_7B;
			case ECricketCombatLogEventType.Interlude:
			case ECricketCombatLogEventType.RoundEnd:
				result = this.CheckedFirst(log, sequence);
				goto IL_7B;
			case ECricketCombatLogEventType.Damage:
				result = this.Damage(log, sequence);
				goto IL_7B;
			case ECricketCombatLogEventType.CombatEnd:
				result = this.CombatEnd(log, sequence);
				goto IL_7B;
			}
			result = sequence;
			IL_7B:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060087AC RID: 34732 RVA: 0x003F0F75 File Offset: 0x003EF175
		private void DoReset()
		{
			this.HideCombatStateInfo();
			this.combatHolder.rotation = Quaternion.identity;
		}

		// Token: 0x060087AD RID: 34733 RVA: 0x003F0F90 File Offset: 0x003EF190
		private void ShowCombatStateInfo(string info, float duration = -1f, float yAdjust = 5f)
		{
			this.stateText.text = info;
			this.stateBack.transform.localPosition = Vector3.zero + Vector3.up * yAdjust;
			this.stateBack.SetActive(true);
			bool flag = duration > 0f;
			if (flag)
			{
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(duration, new Action(this.HideCombatStateInfo));
			}
		}

		// Token: 0x060087AE RID: 34734 RVA: 0x003F1002 File Offset: 0x003EF202
		private void HideCombatStateInfo()
		{
			this.stateBack.SetActive(false);
			this.stateText.text = "";
		}

		// Token: 0x060087AF RID: 34735 RVA: 0x003F1023 File Offset: 0x003EF223
		private void PlayRandomCricketHitSound()
		{
			AudioManager.Instance.PlaySound(string.Format("CCricket_punch0{0}", Random.Range(1, 5)), false, false);
		}

		// Token: 0x060087B0 RID: 34736 RVA: 0x003F104C File Offset: 0x003EF24C
		private bool ShowDamageInfo(CricketCombatLogDamage damageLog)
		{
			bool ally = CricketCombatKit.Board.IsAlly(damageLog.Defender.RuntimeId);
			bool critical = damageLog.AttackStatus.Contains(ECricketCombatAttackStatus.Critical);
			CricketCombatDamage damage = damageLog.Damage;
			bool flag = damage.Durability > 0;
			if (flag)
			{
				this.ShowDamageInfo(ally, ECricketDamageShowType.Durability, damage.Durability, false, critical);
				CricketViewNew attacker = CricketCombatKit.Board.GetCricketView(damageLog.Attacker.RuntimeId);
				attacker.Sing(false, true, false, -1f, null, 0f);
			}
			bool needDelay = damage.Durability > 0;
			bool flag2 = damage.Hp > 0;
			if (flag2)
			{
				this.ShowDamageInfo(ally, ECricketDamageShowType.Hp, damage.Hp, needDelay, critical);
			}
			needDelay = (needDelay || damage.Hp > 0);
			bool flag3 = damage.Sp > 0;
			if (flag3)
			{
				this.ShowDamageInfo(ally, ECricketDamageShowType.Sp, damage.Sp, needDelay, critical);
			}
			CricketCombatDisplayData defenderData = CricketCombatKit.Board.GetCricket(damageLog.Defender.RuntimeId);
			defenderData.Data.ApplyDamage(damage);
			GEvent.OnEvent(UiEvents.CricketCombatUpdateProperty, null);
			CricketCombatArea.ApplyItemDamage(ally, damage);
			return defenderData.Data.IsFail;
		}

		// Token: 0x060087B1 RID: 34737 RVA: 0x003F1180 File Offset: 0x003EF380
		private static void ApplyItemDamage(bool ally, CricketCombatDamage damage)
		{
			ItemDisplayData item = CricketCombatKit.Board.GetCricketItem(ally);
			item.Durability = (short)Math.Max((int)item.Durability - damage.Durability, 0);
			int itemId = item.Key.Id;
			CricketInjury injury = damage.Injury;
			bool flag = injury.Hp > 0;
			if (flag)
			{
				ItemDomainMethod.Call.AddCricketInjury(itemId, 0, (short)injury.Hp);
			}
			bool flag2 = injury.Sp > 0;
			if (flag2)
			{
				ItemDomainMethod.Call.AddCricketInjury(itemId, 1, (short)injury.Sp);
			}
			bool flag3 = injury.Vigor > 0;
			if (flag3)
			{
				ItemDomainMethod.Call.AddCricketInjury(itemId, 2, (short)injury.Vigor);
			}
			bool flag4 = injury.Strength > 0;
			if (flag4)
			{
				ItemDomainMethod.Call.AddCricketInjury(itemId, 3, (short)injury.Strength);
			}
			bool flag5 = injury.Bite > 0;
			if (flag5)
			{
				ItemDomainMethod.Call.AddCricketInjury(itemId, 4, (short)injury.Bite);
			}
		}

		// Token: 0x060087B2 RID: 34738 RVA: 0x003F125C File Offset: 0x003EF45C
		private void ShowDamageInfo(bool ally, ECricketDamageShowType damageType, int damageValue, bool needDelay = false, bool isCritical = false)
		{
			CricketDamage damage = PoolManager.GetObject<CricketDamage>("UI_CricketCombat_DamagePrefabKey");
			Transform damageTransform = damage.transform;
			damageTransform.SetParent(this.combatArea, false);
			damageTransform.SetAsLastSibling();
			damageTransform.position = CricketCombatKit.Board.GetCricketView(ally).transform.position;
			Vector3 damageLocalPosition = damageTransform.localPosition;
			damageTransform.localPosition = damageLocalPosition.SetX(damageLocalPosition.x + (float)Random.Range(-100, 100));
			damage.Set(damageType, damageValue, isCritical, ally);
			damage.DoAnimation(needDelay, isCritical);
		}

		// Token: 0x060087B3 RID: 34739 RVA: 0x003F12EC File Offset: 0x003EF4EC
		private void OnPauseStateChanged(ArgumentBox argBox)
		{
			bool nowPauseState;
			bool flag = !argBox.Get("NowPauseState", out nowPauseState);
			if (!flag)
			{
				bool flag2 = nowPauseState;
				if (flag2)
				{
					this.ShowCombatStateInfo(LocalStringManager.Get(LanguageKey.LK_CricketBattle_FightPause), -1f, 180f);
				}
				else
				{
					this.HideCombatStateInfo();
				}
			}
		}

		// Token: 0x060087B4 RID: 34740 RVA: 0x003F133C File Offset: 0x003EF53C
		private Sequence CombatStart(Sequence sequence)
		{
			this.CombatStartEnter(true);
			this.CombatStartEnter(false);
			return sequence;
		}

		// Token: 0x060087B5 RID: 34741 RVA: 0x003F1360 File Offset: 0x003EF560
		private void CombatStartEnter(bool ally)
		{
			CricketViewNew cricket = ally ? CricketCombatKit.Board.SelfCricketView : CricketCombatKit.Board.EnemyCricketView;
			RectTransform cricketTransform = cricket.GetComponent<RectTransform>();
			float aniTime = 0.2f;
			float delayTime = Random.Range(0f, 0.4f);
			cricket.PlayAnimation(ECricketAnim.Jump, false, false);
			cricketTransform.SetParent(this.combatHolder);
			DOTweenPath tweenPath = cricket.GetComponent<DOTweenPath>();
			tweenPath.GetTween().OnComplete(delegate
			{
				cricket.PlayAnimation(ECricketAnim.Idle, true, false);
				cricketTransform.localEulerAngles = Vector3.zero.SetZ((float)(90 * (ally ? -1 : 1)));
			});
			tweenPath.delay = delayTime;
			tweenPath.DORestart(true);
			cricketTransform.DOScale(0.4f, aniTime);
			this.ShowCombatStateInfo(LocalStringManager.Get(LanguageKey.LK_CricketBattle_Ready), 2f, 180f);
		}

		// Token: 0x060087B6 RID: 34742 RVA: 0x003F1447 File Offset: 0x003EF647
		private void OnSkillResolved()
		{
			CricketCombatKit.Board.SelfCricketView.ShowSkillReplaceTip(CricketCombatKit.Board.SelfCricket, true);
		}

		// Token: 0x060087B7 RID: 34743 RVA: 0x003F1468 File Offset: 0x003EF668
		private Sequence CheckWithoutFight(CricketCombatLog log, Sequence sequence)
		{
			CricketCombatArea.<>c__DisplayClass22_0 CS$<>8__locals1 = new CricketCombatArea.<>c__DisplayClass22_0();
			CS$<>8__locals1.<>4__this = this;
			CricketCombatArea.<>c__DisplayClass22_0 CS$<>8__locals2 = CS$<>8__locals1;
			CricketCombatLogCheckWithoutFight check = log as CricketCombatLogCheckWithoutFight;
			CS$<>8__locals2.compareResult = ((check != null) ? check.WithoutFightType : ECricketWithoutFightType.None);
			if (sequence == null)
			{
				sequence = DOTween.Sequence();
			}
			sequence.AppendInterval(1f);
			sequence.AppendCallback(delegate
			{
				if (!true)
				{
				}
				ECricketWithoutFightType compareResult = CS$<>8__locals1.compareResult;
				LanguageKey id;
				if (compareResult != ECricketWithoutFightType.None)
				{
					if (compareResult != ECricketWithoutFightType.Win)
					{
						id = LanguageKey.LK_CricketBattle_Disadvantage;
					}
					else
					{
						id = LanguageKey.LK_CricketBattle_Advantage;
					}
				}
				else
				{
					id = LanguageKey.LK_CricketBattle_Tie;
				}
				if (!true)
				{
				}
				string info = LocalStringManager.Get(id);
				CS$<>8__locals1.<>4__this.ShowCombatStateInfo(info, -1f, 180f);
				bool flag = CS$<>8__locals1.compareResult == ECricketWithoutFightType.Win;
				if (flag)
				{
					CricketCombatKit.Board.EnemyCricketView.PlayAnimation(ECricketAnim.Die, false, false);
				}
				else
				{
					bool flag2 = CS$<>8__locals1.compareResult == ECricketWithoutFightType.Lose;
					if (flag2)
					{
						CricketCombatKit.Board.SelfCricketView.PlayAnimation(ECricketAnim.Die, false, false);
					}
				}
			});
			sequence.AppendInterval(1f);
			sequence.AppendCallback(delegate
			{
				if (!true)
				{
				}
				ECricketWithoutFightType compareResult = CS$<>8__locals1.compareResult;
				LanguageKey id;
				if (compareResult != ECricketWithoutFightType.None)
				{
					if (compareResult != ECricketWithoutFightType.Win)
					{
						id = LanguageKey.LK_Lose;
					}
					else
					{
						id = LanguageKey.LK_Win;
					}
				}
				else
				{
					id = LanguageKey.LK_CricketBattle_Fight;
				}
				if (!true)
				{
				}
				string info = LocalStringManager.Get(id);
				CS$<>8__locals1.<>4__this.ShowCombatStateInfo(info, 2f, 180f);
			});
			sequence.AppendInterval(1f);
			return sequence;
		}

		// Token: 0x060087B8 RID: 34744 RVA: 0x003F14F8 File Offset: 0x003EF6F8
		private Sequence RoundStart(Sequence sequence)
		{
			if (sequence == null)
			{
				sequence = DOTween.Sequence();
			}
			CricketViewNew selfFightingCricket = CricketCombatKit.Board.SelfCricketView;
			RectTransform selfTransform = (RectTransform)selfFightingCricket.transform;
			CricketViewNew enemyFightingCricket = CricketCombatKit.Board.EnemyCricketView;
			RectTransform enemyTransform = (RectTransform)enemyFightingCricket.transform;
			int i = 0;
			int count = Random.Range(1, 3);
			while (i < count)
			{
				float rotateAngle = (float)(Random.Range(30, 50) * (Utils_Random.RandomCheck(50, 100) ? 1 : -1));
				bool selfMoveFirst = Utils_Random.RandomCheck(50, 100);
				CricketViewNew cricket0 = selfMoveFirst ? selfFightingCricket : enemyFightingCricket;
				RectTransform transform0 = selfMoveFirst ? selfTransform : enemyTransform;
				CricketViewNew cricket1 = selfMoveFirst ? enemyFightingCricket : selfFightingCricket;
				RectTransform transform1 = selfMoveFirst ? enemyTransform : selfTransform;
				sequence.AppendInterval(Random.Range(0.5f, 1f));
				TweenCallback <>9__5;
				sequence.AppendCallback(delegate
				{
					Vector3 worldPos0 = transform0.position;
					Quaternion worldRot0 = transform0.rotation;
					Vector3 worldPos = transform1.position;
					Quaternion worldRot = transform1.rotation;
					this.combatHolder.Rotate(Vector3.forward, rotateAngle);
					transform0.position = worldPos0;
					transform0.rotation = worldRot0;
					transform1.position = worldPos;
					transform1.rotation = worldRot;
					cricket0.PlayAnimation(ECricketAnim.Jump, false, false);
					transform0.DOJumpAnchorPos(new Vector2(200f * (float)(selfMoveFirst ? -1 : 1), 0f), 1f, 1, 0.1f, false);
					TweenerCore<Quaternion, Vector3, QuaternionOptions> t = transform0.DOLocalRotate(Vector3.zero.SetZ((float)(90 * (selfMoveFirst ? -1 : 1))), 0.1f, RotateMode.Fast);
					TweenCallback action;
					if ((action = <>9__5) == null)
					{
						action = (<>9__5 = delegate()
						{
							cricket0.PlayAnimation(ECricketAnim.Idle, true, false);
						});
					}
					t.OnComplete(action);
				});
				sequence.AppendInterval(0.25f);
				TweenCallback <>9__6;
				sequence.AppendCallback(delegate
				{
					cricket1.PlayAnimation(ECricketAnim.Jump, false, false);
					transform1.DOJumpAnchorPos(new Vector2(200f * (float)(selfMoveFirst ? 1 : -1), 0f), 1f, 1, 0.1f, false);
					TweenerCore<Quaternion, Vector3, QuaternionOptions> t = transform1.DOLocalRotate(Vector3.zero.SetZ((float)(90 * (selfMoveFirst ? 1 : -1))), 0.1f, RotateMode.Fast);
					TweenCallback action;
					if ((action = <>9__6) == null)
					{
						action = (<>9__6 = delegate()
						{
							cricket1.PlayAnimation(ECricketAnim.Idle, true, false);
						});
					}
					t.OnComplete(action);
				});
				sequence.AppendInterval(0.5f);
				i++;
			}
			sequence.AppendCallback(delegate
			{
				selfFightingCricket.PlayAnimation(ECricketAnim.Attack, false, false);
				enemyFightingCricket.PlayAnimation(ECricketAnim.Attack, false, false);
				this.PlayRandomCricketHitSound();
			});
			sequence.Append(selfTransform.DOLocalMoveX(-100f, 0.2f, false).SetEase(Ease.InBack));
			sequence.Join(enemyTransform.DOLocalMoveX(100f, 0.2f, false).SetEase(Ease.InBack));
			sequence.AppendCallback(delegate
			{
				selfFightingCricket.PlayAnimation(ECricketAnim.Jump, false, false);
				enemyFightingCricket.PlayAnimation(ECricketAnim.Jump, false, false);
			});
			sequence.Append(selfTransform.DOJumpAnchorPos(new Vector2(-200f, 0f), 1f, 1, 0.2f, false));
			sequence.Join(enemyTransform.DOJumpAnchorPos(new Vector2(200f, 0f), 1f, 1, 0.2f, false));
			sequence.AppendCallback(delegate
			{
				selfFightingCricket.PlayAnimation(ECricketAnim.Idle, false, false);
				enemyFightingCricket.PlayAnimation(ECricketAnim.Idle, false, false);
			});
			return sequence;
		}

		// Token: 0x060087B9 RID: 34745 RVA: 0x003F1764 File Offset: 0x003EF964
		private Sequence Damage(CricketCombatLog log, Sequence sequence)
		{
			CricketCombatLogDamage damage = log as CricketCombatLogDamage;
			bool flag = damage == null;
			Sequence result;
			if (flag)
			{
				result = sequence;
			}
			else
			{
				ECricketCombatDamageType damageType = damage.DamageType;
				if (!true)
				{
				}
				Sequence sequence2;
				switch (damageType)
				{
				case ECricketCombatDamageType.Vigor:
					sequence2 = this.DamageVigor(damage, sequence);
					break;
				case ECricketCombatDamageType.Strength:
					sequence2 = this.DamageStrength(damage, sequence, true);
					break;
				case ECricketCombatDamageType.Bite:
					sequence2 = this.DamageBite(damage, sequence);
					break;
				default:
					sequence2 = this.DamageNoAnim(damage, sequence);
					break;
				}
				if (!true)
				{
				}
				result = sequence2;
			}
			return result;
		}

		// Token: 0x060087BA RID: 34746 RVA: 0x003F17E0 File Offset: 0x003EF9E0
		private Sequence DamageVigor(CricketCombatLogDamage damage, Sequence sequence)
		{
			if (sequence == null)
			{
				sequence = DOTween.Sequence();
			}
			sequence.AppendCallback(delegate
			{
				CricketViewNew attacker = CricketCombatKit.Board.GetCricketView(damage.Attacker.RuntimeId);
				attacker.PlayAnimation(ECricketAnim.Attack2, false, false);
				attacker.Sing(false, true, false, -1f, null, 0f);
			});
			sequence.AppendInterval(0.7f);
			sequence.AppendCallback(delegate
			{
				bool isFail = this.ShowDamageInfo(damage);
				CricketViewNew defender = CricketCombatKit.Board.GetCricketView(damage.Defender.RuntimeId);
				defender.PlayAnimation(isFail ? ECricketAnim.Die : ECricketAnim.Hit, false, false);
				defender.Twinkle();
			});
			sequence.AppendInterval(0.6f);
			return sequence;
		}

		// Token: 0x060087BB RID: 34747 RVA: 0x003F1850 File Offset: 0x003EFA50
		private Sequence DamageBite(CricketCombatLogDamage damage, Sequence sequence)
		{
			bool flag = damage.AttackStatus.Contains(ECricketCombatAttackStatus.Counter);
			Sequence result;
			if (flag)
			{
				result = this.DamageStrength(damage, sequence, false);
			}
			else
			{
				if (sequence == null)
				{
					sequence = DOTween.Sequence();
				}
				CricketViewNew attacker = CricketCombatKit.Board.GetCricketView(damage.Attacker.RuntimeId);
				TweenCallback <>9__2;
				sequence.AppendCallback(delegate
				{
					bool ally = CricketCombatKit.Board.IsAlly(damage.Attacker.RuntimeId);
					attacker.transform.SetAsLastSibling();
					attacker.PlayAnimation(ECricketAnim.Jump, false, false);
					Sequence t = attacker.GetComponent<RectTransform>().DOJumpAnchorPos(new Vector2((float)(8 * (ally ? -1 : 1)), 0f), 1f, 1, 0.1f, false);
					TweenCallback action;
					if ((action = <>9__2) == null)
					{
						action = (<>9__2 = delegate()
						{
							attacker.PlayAnimation(ECricketAnim.Idle, false, false);
						});
					}
					t.OnComplete(action);
				});
				sequence.AppendInterval(0.3f);
				sequence.AppendCallback(delegate
				{
					attacker.PlayAnimation(ECricketAnim.Attack, false, false);
					CricketViewNew defender = CricketCombatKit.Board.GetCricketView(damage.Defender.RuntimeId);
					bool flag2 = damage.AttackStatus.Contains(ECricketCombatAttackStatus.Parry);
					if (flag2)
					{
						defender.PlayAnimation(ECricketAnim.Def, false, false);
					}
					this.PlayRandomCricketHitSound();
				});
				sequence.AppendInterval(attacker.GetEventTime(ECricketAnim.Attack, "act0"));
				result = this.DamageAttack(damage, sequence);
			}
			return result;
		}

		// Token: 0x060087BC RID: 34748 RVA: 0x003F1920 File Offset: 0x003EFB20
		private Sequence DamageStrength(CricketCombatLogDamage damage, Sequence sequence, bool isAnti = true)
		{
			if (sequence == null)
			{
				sequence = DOTween.Sequence();
			}
			CricketViewNew attacker = CricketCombatKit.Board.GetCricketView(damage.Attacker.RuntimeId);
			sequence.AppendCallback(delegate
			{
				attacker.PlayAnimation(isAnti ? ECricketAnim.Anti : ECricketAnim.Attack, false, false);
				CricketViewNew defender = CricketCombatKit.Board.GetCricketView(damage.Defender.RuntimeId);
				bool flag = damage.AttackStatus.Contains(ECricketCombatAttackStatus.Parry);
				if (flag)
				{
					defender.PlayAnimation(ECricketAnim.Def, false, false);
				}
				this.PlayRandomCricketHitSound();
			});
			sequence.AppendInterval(attacker.GetEventTime(ECricketAnim.Anti, "act0"));
			return this.DamageAttack(damage, sequence);
		}

		// Token: 0x060087BD RID: 34749 RVA: 0x003F19B0 File Offset: 0x003EFBB0
		private Sequence DamageAttack(CricketCombatLogDamage damage, Sequence sequence)
		{
			CricketViewNew defender = CricketCombatKit.Board.GetCricketView(damage.Defender.RuntimeId);
			sequence.AppendCallback(delegate
			{
				bool isFail = this.ShowDamageInfo(damage);
				defender.PlayAnimation(isFail ? ECricketAnim.Die : ECricketAnim.Hit, false, false);
				defender.Twinkle();
			});
			CricketCombatDisplayData defenderData = CricketCombatKit.Board.GetCricket(damage.Defender.RuntimeId);
			bool flag = defenderData.Data.SimulateDamage(damage.Damage);
			if (flag)
			{
				sequence.AppendInterval(0.8f);
			}
			return sequence;
		}

		// Token: 0x060087BE RID: 34750 RVA: 0x003F1A4C File Offset: 0x003EFC4C
		private Sequence DamageNoAnim(CricketCombatLogDamage damage, Sequence sequence)
		{
			this.ShowDamageInfo(damage);
			return sequence;
		}

		// Token: 0x060087BF RID: 34751 RVA: 0x003F1A68 File Offset: 0x003EFC68
		private Sequence CheckedFirst(CricketCombatLog log, Sequence sequence)
		{
			ICricketCombatLogCheckedFirst cricketCombatLogCheckedFirst = log as ICricketCombatLogCheckedFirst;
			bool selfFirst = cricketCombatLogCheckedFirst != null && cricketCombatLogCheckedFirst.LeftFirst;
			if (sequence == null)
			{
				sequence = DOTween.Sequence();
			}
			sequence.AppendInterval(0.3f);
			CricketViewNew first = CricketCombatKit.Board.GetCricketView(selfFirst);
			sequence.AppendCallback(delegate
			{
				first.PlayAnimation(ECricketAnim.Jump, false, false);
			});
			sequence.Append(first.GetComponent<RectTransform>().DOJumpAnchorPos(new Vector2(200f * (float)(selfFirst ? -1 : 1), 0f), 1f, 1, 0.2f, false));
			sequence.AppendCallback(delegate
			{
				CricketCombatKit.Board.SelfCricketView.PlayAnimation(ECricketAnim.Idle, true, false);
				CricketCombatKit.Board.EnemyCricketView.PlayAnimation(ECricketAnim.Idle, true, false);
			});
			return sequence;
		}

		// Token: 0x060087C0 RID: 34752 RVA: 0x003F1B30 File Offset: 0x003EFD30
		private Sequence CombatEnd(CricketCombatLog log, Sequence sequence)
		{
			CricketCombatArea.<>c__DisplayClass31_0 CS$<>8__locals1 = new CricketCombatArea.<>c__DisplayClass31_0();
			CS$<>8__locals1.<>4__this = this;
			if (sequence == null)
			{
				sequence = DOTween.Sequence();
			}
			AudioManager.Instance.PlaySound("ui_cricket_word", false, false);
			CricketCombatArea.<>c__DisplayClass31_0 CS$<>8__locals2 = CS$<>8__locals1;
			CricketCombatLogCombatEnd cricketCombatLogCombatEnd = log as CricketCombatLogCombatEnd;
			CS$<>8__locals2.win = (cricketCombatLogCombatEnd != null && cricketCombatLogCombatEnd.Win);
			this.ShowCombatStateInfo(LocalStringManager.Get(CS$<>8__locals1.win ? LanguageKey.LK_Win : LanguageKey.LK_Lose), 2f, 5f);
			sequence.AppendInterval(1f);
			sequence.AppendCallback(delegate
			{
				CricketCombatKit.Board.SelfCricketView.Fade(false);
				CricketCombatKit.Board.EnemyCricketView.Fade(false);
			});
			sequence.AppendInterval(1f);
			sequence.AppendCallback(delegate
			{
				CS$<>8__locals1.<>4__this.DoReset();
				CricketCombatKit.Board.SelfCricketJar.Settlement(CS$<>8__locals1.win);
				CricketCombatKit.Board.EnemyCricketJar.Settlement(!CS$<>8__locals1.win);
				CricketCombatKit.Board.SelfCricketView.RestoreDefaultMouseTip(true);
			});
			sequence.AppendInterval(2f);
			return sequence;
		}

		// Token: 0x04006828 RID: 26664
		private const float BattleStartPos = 200f;

		// Token: 0x04006829 RID: 26665
		[SerializeField]
		private RectTransform combatArea;

		// Token: 0x0400682A RID: 26666
		[SerializeField]
		private RectTransform combatHolder;

		// Token: 0x0400682B RID: 26667
		[SerializeField]
		private GameObject stateBack;

		// Token: 0x0400682C RID: 26668
		[SerializeField]
		private TextMeshProUGUI stateText;
	}
}
