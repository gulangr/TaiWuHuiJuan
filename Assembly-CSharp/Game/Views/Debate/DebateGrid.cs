using System;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using GameData.Domains.Taiwu.Debate;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Debate
{
	// Token: 0x02000A9E RID: 2718
	public class DebateGrid : MonoBehaviour
	{
		// Token: 0x17000EA2 RID: 3746
		// (get) Token: 0x0600850E RID: 34062 RVA: 0x003DCD32 File Offset: 0x003DAF32
		private LifeSkillCombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<LifeSkillCombatModel>();
			}
		}

		// Token: 0x0600850F RID: 34063 RVA: 0x003DCD3C File Offset: 0x003DAF3C
		public void Init()
		{
			for (int i = 0; i < this.blockLayout.transform.childCount; i++)
			{
				int x = i % 6;
				int y = i / 6;
				Vector2Int pos = new Vector2Int(x, y);
				bool isSelf = this._selfBlockPosSet.Contains(pos);
				DebateBlock block = this.blockLayout.transform.GetChild(i).GetComponent<DebateBlock>();
				block.Init(pos, isSelf, this.blockOperationLayout);
				this._blockList.Add(block);
			}
			PoolManager.SetSrcObject("LifeSkillCombatUnitSelf", this.unitSelfPrefab);
			PoolManager.SetSrcObject("LifeSkillCombatUnitEnemy", this.unitEnemyPrefab);
			this.unitSelfPrefab.SetActive(false);
			this.unitEnemyPrefab.SetActive(false);
		}

		// Token: 0x06008510 RID: 34064 RVA: 0x003DCE00 File Offset: 0x003DB000
		public void Clear()
		{
			foreach (DebateBlock block in this._blockList)
			{
				block.RemoveEffect();
			}
			this._blockList.Clear();
			foreach (DebateNodeEffectItem nodeEffectConfig in ((IEnumerable<DebateNodeEffectItem>)DebateNodeEffect.Instance))
			{
				AudioManager.Instance.StopAllSound(nodeEffectConfig.TriggerSound);
				AudioManager.Instance.StopAllSound(nodeEffectConfig.ExtraTriggerSound);
				AudioManager.Instance.StopAllSound(nodeEffectConfig.LoopSound);
			}
			foreach (DebateUnit unit in this._unitList)
			{
				unit.RectTrans.SetParent(null);
				string key = unit.Pawn.IsOwnedByTaiwu ? "LifeSkillCombatUnitSelf" : "LifeSkillCombatUnitEnemy";
				PoolManager.Destroy(key, unit.RectTrans.gameObject);
			}
			this._unitList.Clear();
			PoolManager.RemoveData("LifeSkillCombatUnitSelf");
			PoolManager.RemoveData("LifeSkillCombatUnitEnemy");
		}

		// Token: 0x06008511 RID: 34065 RVA: 0x003DCF70 File Offset: 0x003DB170
		public DebateBlock FindBlock(Vector2Int position)
		{
			return this._blockList.Find((DebateBlock b) => b.Position == position);
		}

		// Token: 0x06008512 RID: 34066 RVA: 0x003DCFA8 File Offset: 0x003DB1A8
		public bool HasOnlyOneBlockEffect(short effectTemplateId)
		{
			int count = this._blockList.Count((DebateBlock b) => b.DebateNodeEffectState != null && b.DebateNodeEffectState.TemplateId == (int)effectTemplateId);
			return count == 1;
		}

		// Token: 0x06008513 RID: 34067 RVA: 0x003DCFE4 File Offset: 0x003DB1E4
		public DebateUnit FindUnit(int pawnId)
		{
			return this._unitList.Find((DebateUnit u) => u.Pawn.Id == pawnId);
		}

		// Token: 0x06008514 RID: 34068 RVA: 0x003DD01C File Offset: 0x003DB21C
		public void PlayCreateUnitAnim(bool play, Vector3 position, bool isSelf)
		{
			GameObject go = isSelf ? this.createUnitSelfEffect : this.createUnitEnemyEffect;
			go.transform.position = position;
			go.SetActive(play);
			if (play)
			{
				go.GetComponentInChildren<UIParticle>().Play();
			}
		}

		// Token: 0x06008515 RID: 34069 RVA: 0x003DD064 File Offset: 0x003DB264
		public void CreateUnit(Vector2Int position, Pawn pawn, int bases)
		{
			bool findUnit = this._unitList.Exists((DebateUnit u) => u.Pawn.Id == pawn.Id);
			bool flag = findUnit;
			if (!flag)
			{
				AudioManager.Instance.PlaySound(ViewDebate.SoundCreateUnit, false, true);
				DebateBlock block = this.FindBlock(position);
				this.PlayCreateUnitAnim(true, block.RectTrans.position, pawn.IsOwnedByTaiwu);
				DOVirtual.DelayedCall(0.2f, delegate
				{
					string key = pawn.IsOwnedByTaiwu ? "LifeSkillCombatUnitSelf" : "LifeSkillCombatUnitEnemy";
					DebateUnit refers = PoolManager.GetObject<DebateUnit>(key);
					Tester.Assert(refers != null, "");
					refers.transform.SetParent(this.unitLayout);
					refers.transform.localScale = Vector3.one;
					refers.transform.position = block.RectTrans.position;
					refers.Init(pawn, bases);
					this._unitList.Add(refers);
				}, false);
			}
		}

		// Token: 0x06008516 RID: 34070 RVA: 0x003DD108 File Offset: 0x003DB308
		public void DeleteUnit(Pawn pawn)
		{
			DebateUnit findUnit = this.FindUnit(pawn.Id);
			this._unitList.Remove(findUnit);
			findUnit.RectTrans.SetParent(null);
			string key = pawn.IsOwnedByTaiwu ? "LifeSkillCombatUnitSelf" : "LifeSkillCombatUnitEnemy";
			PoolManager.Destroy(key, findUnit.RectTrans.gameObject);
		}

		// Token: 0x06008517 RID: 34071 RVA: 0x003DD164 File Offset: 0x003DB364
		private void RefreshUnit(Pawn pawn, bool refreshStrategy = true)
		{
			DebateUnit findUnit = this.FindUnit(pawn.Id);
			if (findUnit != null)
			{
				findUnit.SetData(pawn, false, refreshStrategy);
			}
		}

		// Token: 0x06008518 RID: 34072 RVA: 0x003DD190 File Offset: 0x003DB390
		public void RefreshAllUnit(bool useNewData = true, bool refreshStrategy = true)
		{
			if (useNewData)
			{
				DebateGame debateGame = this.Model.DebateGame;
				List<Pawn> list;
				if (debateGame == null)
				{
					list = null;
				}
				else
				{
					Dictionary<int, Pawn> pawns2 = debateGame.Pawns;
					list = ((pawns2 != null) ? pawns2.Values.ToList<Pawn>() : null);
				}
				List<Pawn> pawns = list;
				bool flag = pawns == null || pawns.Count <= 0;
				if (!flag)
				{
					foreach (Pawn pawn in pawns)
					{
						this.RefreshUnit(pawn, true);
					}
				}
			}
			else
			{
				foreach (DebateUnit unit in this._unitList)
				{
					this.RefreshUnit(unit.Pawn, refreshStrategy);
				}
			}
		}

		// Token: 0x06008519 RID: 34073 RVA: 0x003DD284 File Offset: 0x003DB484
		public void ConflictUnit(Pawn selfPawn, Pawn enemyPawn, int result, IntPair powers)
		{
			DebateUnit selfUnit = this.FindUnit(selfPawn.Id);
			DebateUnit enemyUnit = this.FindUnit(enemyPawn.Id);
			selfUnit.RectTrans.SetParent(this.conflictAnimRoot);
			enemyUnit.RectTrans.SetParent(this.conflictAnimRoot);
			float duration = (enemyUnit.HasUnrevealedObject() || selfUnit.HasUnrevealedObject()) ? 0.5f : 0f;
			enemyUnit.SetPowerRevealed();
			enemyUnit.RefreshPower(powers.Second, false);
			enemyUnit.SetStrategyRevealed();
			enemyUnit.PlayRevealSound();
			selfUnit.SetPowerRevealed();
			selfUnit.RefreshPower(powers.First, false);
			selfUnit.SetStrategyRevealed();
			selfUnit.PlayRevealSound();
			TweenCallback <>9__1;
			DOVirtual.DelayedCall(duration, delegate
			{
				AudioManager.Instance.PlaySound(ViewDebate.SoundConflict, false, true);
				switch (result)
				{
				case 0:
					selfUnit.PlayConflictAnim(true);
					enemyUnit.PlayConflictAnim(false);
					break;
				case 1:
					selfUnit.PlayConflictAnim(false);
					enemyUnit.PlayConflictAnim(true);
					break;
				case 2:
					selfUnit.PlayConflictAnim(false);
					enemyUnit.PlayConflictAnim(false);
					break;
				default:
					throw new Exception("DebateConflictResult Not Found");
				}
				float delay = 1f;
				TweenCallback callback;
				if ((callback = <>9__1) == null)
				{
					callback = (<>9__1 = delegate()
					{
						selfUnit.PlayIdleAnim();
						enemyUnit.PlayIdleAnim();
						selfUnit.RectTrans.SetParent(this.unitLayout);
						enemyUnit.RectTrans.SetParent(this.unitLayout);
					});
				}
				DOVirtual.DelayedCall(delay, callback, false);
			}, false);
		}

		// Token: 0x0600851A RID: 34074 RVA: 0x003DD3A8 File Offset: 0x003DB5A8
		public void MoveUnit(Pawn pawn, Vector2Int targetBlockPos)
		{
			AudioManager.Instance.PlaySound(ViewDebate.SoundMoveUnit, false, true);
			DebateUnit findUnit = this.FindUnit(pawn.Id);
			DebateBlock findBlock = this.FindBlock(targetBlockPos);
			findUnit.RectTrans.DOMove(findBlock.RectTrans.position, 0.2f, false).SetEase(Ease.InOutExpo).OnComplete(delegate
			{
				findUnit.RectTrans.localScale = Vector3.one;
			});
		}

		// Token: 0x0600851B RID: 34075 RVA: 0x003DD424 File Offset: 0x003DB624
		public void RefreshAllBlock()
		{
			foreach (DebateBlock block in this._blockList)
			{
				IntPair coordinate = new IntPair(block.Position.x, block.Position.y);
				DebateNode debateNode = this.Model.DebateGame.DebateGrid[coordinate];
				block.Refresh(debateNode);
			}
		}

		// Token: 0x04006604 RID: 26116
		[SerializeField]
		private RectTransform blockLayout;

		// Token: 0x04006605 RID: 26117
		[SerializeField]
		private RectTransform unitLayout;

		// Token: 0x04006606 RID: 26118
		[SerializeField]
		private RectTransform blockOperationLayout;

		// Token: 0x04006607 RID: 26119
		[SerializeField]
		private GameObject unitSelfPrefab;

		// Token: 0x04006608 RID: 26120
		[SerializeField]
		private GameObject unitEnemyPrefab;

		// Token: 0x04006609 RID: 26121
		[SerializeField]
		private GameObject createUnitSelfEffect;

		// Token: 0x0400660A RID: 26122
		[SerializeField]
		private GameObject createUnitEnemyEffect;

		// Token: 0x0400660B RID: 26123
		[SerializeField]
		private RectTransform conflictAnimRoot;

		// Token: 0x0400660C RID: 26124
		public const int Column = 6;

		// Token: 0x0400660D RID: 26125
		public const int Row = 3;

		// Token: 0x0400660E RID: 26126
		private readonly HashSet<Vector2Int> _selfBlockPosSet = new HashSet<Vector2Int>
		{
			new Vector2Int(0, 0),
			new Vector2Int(1, 0),
			new Vector2Int(2, 0),
			new Vector2Int(3, 0),
			new Vector2Int(0, 1),
			new Vector2Int(1, 1),
			new Vector2Int(2, 1),
			new Vector2Int(0, 2),
			new Vector2Int(1, 2)
		};

		// Token: 0x0400660F RID: 26127
		private readonly HashSet<Vector2Int> _enemyBlockPosSet = new HashSet<Vector2Int>
		{
			new Vector2Int(4, 0),
			new Vector2Int(5, 0),
			new Vector2Int(3, 1),
			new Vector2Int(4, 1),
			new Vector2Int(5, 1),
			new Vector2Int(2, 2),
			new Vector2Int(3, 2),
			new Vector2Int(4, 2),
			new Vector2Int(5, 2)
		};

		// Token: 0x04006610 RID: 26128
		private const string LifeSkillCombatUnitSelfKey = "LifeSkillCombatUnitSelf";

		// Token: 0x04006611 RID: 26129
		private const string LifeSkillCombatUnitEnemyKey = "LifeSkillCombatUnitEnemy";

		// Token: 0x04006612 RID: 26130
		private readonly List<DebateBlock> _blockList = new List<DebateBlock>();

		// Token: 0x04006613 RID: 26131
		private readonly List<DebateUnit> _unitList = new List<DebateUnit>();
	}
}
