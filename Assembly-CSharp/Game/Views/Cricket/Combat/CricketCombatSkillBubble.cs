using System;
using System.Collections.Generic;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Combat.Cricket;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AD2 RID: 2770
	public class CricketCombatSkillBubble : MonoBehaviour, ICricketCombatComponent
	{
		// Token: 0x17000F03 RID: 3843
		// (get) Token: 0x06008863 RID: 34915 RVA: 0x003F434C File Offset: 0x003F254C
		// (set) Token: 0x06008864 RID: 34916 RVA: 0x003F4354 File Offset: 0x003F2554
		public ICricketCombatHandler Handler { get; set; }

		// Token: 0x06008865 RID: 34917 RVA: 0x003F4360 File Offset: 0x003F2560
		public void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox)
		{
			bool flag = type == ECricketCombatGlobalEventType.Initialize;
			if (flag)
			{
				this.ClearAllBubbles();
				this.bubbleContainer.gameObject.SetActive(CricketPolymorphHelper.IsCricketPolymorphEnabled);
			}
		}

		// Token: 0x06008866 RID: 34918 RVA: 0x003F4398 File Offset: 0x003F2598
		public Sequence HandleLog(CricketCombatLog log, Sequence sequence)
		{
			CricketCombatLogSkill skillLog = log as CricketCombatLogSkill;
			bool flag = skillLog == null;
			Sequence result;
			if (flag)
			{
				result = sequence;
			}
			else
			{
				CricketCombatSkillBase skill = skillLog.Skill;
				bool isAlly = CricketCombatKit.Board.IsAlly(skill.Owner.RuntimeId);
				bool flag2 = !isAlly && skill.Type == ECricketCombatSkillType.ExtraWager;
				if (flag2)
				{
					result = sequence;
				}
				else
				{
					bool flag3 = !isAlly || !CricketPolymorphHelper.IsCricketPolymorphEnabled;
					if (flag3)
					{
						result = sequence;
					}
					else
					{
						bool flag4 = CricketCombatKit.Board.CurrentMatch != this.jarIndex;
						if (flag4)
						{
							result = sequence;
						}
						else
						{
							bool flag5 = CricketCombatKit.Board.GetSelectedCricketPolymorphCharacter(this.jarIndex) == null;
							if (flag5)
							{
								result = sequence;
							}
							else
							{
								int skillTemplateId = (int)skill.Type;
								bool flag6 = skillTemplateId < 0;
								if (flag6)
								{
									result = sequence;
								}
								else
								{
									CricketSkillItem skillConfig = CricketSkill.Instance[skillTemplateId];
									bool flag7 = skillConfig == null;
									if (flag7)
									{
										result = sequence;
									}
									else
									{
										bool flag8 = !string.IsNullOrEmpty(skillConfig.EffectTips);
										if (flag8)
										{
											this.SpawnBubble(skillConfig.EffectTips);
										}
										result = sequence;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06008867 RID: 34919 RVA: 0x003F44B4 File Offset: 0x003F26B4
		private void SpawnBubble(string text)
		{
			this.ShiftExistingBubbles();
			CricketCombatSkillBubbleItem instance = Object.Instantiate<CricketCombatSkillBubbleItem>(this.bubblePrefab, this.bubbleContainer, false);
			instance.gameObject.SetActive(true);
			instance.Setup(text);
			instance.transform.SetAsLastSibling();
			instance.CanvasGroup.alpha = 0f;
			instance.RectTransform.anchoredPosition = this.GetPositionForIndex(0);
			instance.CanvasGroup.DOFade(CricketCombatSkillBubble.AlphaByPosition[0], 0.3f);
			this._bubbles.Insert(0, instance);
			CricketCombatKit.DelayCallRealTime(this, delegate
			{
				this.FadeOutAndRecycle(instance);
			}, 4f);
			this.TrimBubbles();
		}

		// Token: 0x06008868 RID: 34920 RVA: 0x003F459C File Offset: 0x003F279C
		private void ShiftExistingBubbles()
		{
			for (int i = 0; i < this._bubbles.Count; i++)
			{
				CricketCombatSkillBubbleItem bubble = this._bubbles[i];
				bool flag = bubble == null || bubble.gameObject == null;
				if (!flag)
				{
					int targetIndex = i + 1;
					float targetAlpha = (targetIndex < 4) ? CricketCombatSkillBubble.AlphaByPosition[targetIndex] : 0f;
					Vector2 targetPos = this.GetPositionForIndex(targetIndex);
					bubble.RectTransform.DOAnchorPos(targetPos, 0.3f, false).SetEase(Ease.OutCubic);
					bubble.CanvasGroup.DOFade(targetAlpha, 0.3f);
				}
			}
		}

		// Token: 0x06008869 RID: 34921 RVA: 0x003F4648 File Offset: 0x003F2848
		private void TrimBubbles()
		{
			while (this._bubbles.Count > 4)
			{
				CricketCombatSkillBubbleItem last = this._bubbles[this._bubbles.Count - 1];
				this._bubbles.RemoveAt(this._bubbles.Count - 1);
				bool flag = last != null && last.gameObject != null;
				if (flag)
				{
					Object.Destroy(last.gameObject);
				}
			}
		}

		// Token: 0x0600886A RID: 34922 RVA: 0x003F46C8 File Offset: 0x003F28C8
		private void FadeOutAndRecycle(CricketCombatSkillBubbleItem instance)
		{
			bool flag = instance == null || instance.gameObject == null;
			if (!flag)
			{
				instance.CanvasGroup.DOFade(0f, 0.5f).OnComplete(delegate
				{
					bool flag2 = instance != null && instance.gameObject != null;
					if (flag2)
					{
						this._bubbles.Remove(instance);
						Object.Destroy(instance.gameObject);
					}
				});
			}
		}

		// Token: 0x0600886B RID: 34923 RVA: 0x003F4740 File Offset: 0x003F2940
		private void ClearAllBubbles()
		{
			foreach (CricketCombatSkillBubbleItem bubble in this._bubbles)
			{
				bool flag = bubble != null && bubble.gameObject != null;
				if (flag)
				{
					Object.Destroy(bubble.gameObject);
				}
			}
			this._bubbles.Clear();
		}

		// Token: 0x0600886C RID: 34924 RVA: 0x003F47C4 File Offset: 0x003F29C4
		private Vector2 GetPositionForIndex(int index)
		{
			return new Vector2(0f, (float)index * this.bubbleSpacingY);
		}

		// Token: 0x0400687B RID: 26747
		private const float MoveDuration = 0.3f;

		// Token: 0x0400687C RID: 26748
		private const float BubbleLifetime = 4f;

		// Token: 0x0400687D RID: 26749
		private const float FadeOutDuration = 0.5f;

		// Token: 0x0400687E RID: 26750
		private const int MaxBubbles = 4;

		// Token: 0x0400687F RID: 26751
		private static readonly float[] AlphaByPosition = new float[]
		{
			1f,
			0.75f,
			0.5f,
			0.25f
		};

		// Token: 0x04006880 RID: 26752
		[SerializeField]
		private int jarIndex;

		// Token: 0x04006881 RID: 26753
		[SerializeField]
		private float bubbleSpacingY = 60f;

		// Token: 0x04006882 RID: 26754
		[SerializeField]
		private RectTransform bubbleContainer;

		// Token: 0x04006883 RID: 26755
		[SerializeField]
		private CricketCombatSkillBubbleItem bubblePrefab;

		// Token: 0x04006884 RID: 26756
		private readonly List<CricketCombatSkillBubbleItem> _bubbles = new List<CricketCombatSkillBubbleItem>();
	}
}
