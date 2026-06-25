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
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000247 RID: 583
public class LifeSkillCombatGrid
{
	// Token: 0x170003FC RID: 1020
	// (get) Token: 0x060025F4 RID: 9716 RVA: 0x00116402 File Offset: 0x00114602
	private LifeSkillCombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<LifeSkillCombatModel>();
		}
	}

	// Token: 0x170003FD RID: 1021
	// (get) Token: 0x060025F5 RID: 9717 RVA: 0x00116409 File Offset: 0x00114609
	public Refers Refers
	{
		get
		{
			return this._refers;
		}
	}

	// Token: 0x170003FE RID: 1022
	// (get) Token: 0x060025F6 RID: 9718 RVA: 0x00116411 File Offset: 0x00114611
	public RectTransform ConflictAnimRoot
	{
		get
		{
			return this._refers.CGet<RectTransform>("ConflictAnimRoot");
		}
	}

	// Token: 0x060025F7 RID: 9719 RVA: 0x00116424 File Offset: 0x00114624
	public void Init(Refers refers, Action<Vector2Int, sbyte> actionCreateUnit)
	{
		this._refers = refers;
		this._actionCreateUnit = actionCreateUnit;
		this._blockLayout = refers.CGet<RectTransform>("BlockLayout");
		this._unitLayout = refers.CGet<RectTransform>("UnitLayout");
		this._blockOperationLayout = refers.CGet<RectTransform>("BlockOperationLayout");
		this._blockBorderLayout = refers.CGet<RectTransform>("BlockBorderLayout");
		this._borderLayoutOriginSiblingIndex = this._blockBorderLayout.transform.GetSiblingIndex();
		this._unitGradeArea = refers.CGet<Refers>("UnitGradeArea");
		this.InitUnitGradeArea();
		for (int i = 0; i < this._blockLayout.transform.childCount; i++)
		{
			Refers blockRefers = this._blockLayout.transform.GetChild(i).GetComponent<Refers>();
			int x = i % 6;
			int y = i / 6;
			Vector2Int pos = new Vector2Int(x, y);
			bool isSelf = this._selfBlockPosSet.Contains(pos);
			LifeSkillCombatBlock block = new LifeSkillCombatBlock(blockRefers, pos, isSelf, this._blockOperationLayout);
			this._blockList.Add(block);
		}
		GameObject unitSelfPrefab = refers.CGet<GameObject>("LifeSkillCombatUnitSelf");
		PoolManager.SetSrcObject("LifeSkillCombatUnitSelf", unitSelfPrefab);
		GameObject unitEnemyPrefab = refers.CGet<GameObject>("LifeSkillCombatUnitEnemy");
		PoolManager.SetSrcObject("LifeSkillCombatUnitEnemy", unitEnemyPrefab);
		unitSelfPrefab.SetActive(false);
		unitEnemyPrefab.SetActive(false);
	}

	// Token: 0x060025F8 RID: 9720 RVA: 0x00116570 File Offset: 0x00114770
	public void Clear()
	{
		foreach (LifeSkillCombatBlock block in this._blockList)
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
		foreach (LifeSkillCombatUnit unit in this._unitList)
		{
			unit.RectTrans.SetParent(null);
			string key = unit.Pawn.IsOwnedByTaiwu ? "LifeSkillCombatUnitSelf" : "LifeSkillCombatUnitEnemy";
			PoolManager.Destroy(key, unit.RectTrans.gameObject);
		}
		this._unitList.Clear();
		PoolManager.RemoveData("LifeSkillCombatUnitSelf");
		PoolManager.RemoveData("LifeSkillCombatUnitEnemy");
	}

	// Token: 0x060025F9 RID: 9721 RVA: 0x001166E0 File Offset: 0x001148E0
	public void OnHighlightBlock()
	{
		bool flag = this._blockBorderLayout.parent == this._refers.transform.parent;
		if (!flag)
		{
			this._blockBorderLayout.SetParent(this._refers.transform.parent);
			this._blockBorderLayout.transform.SetSiblingIndex(this._refers.transform.GetSiblingIndex() + 1);
		}
	}

	// Token: 0x060025FA RID: 9722 RVA: 0x00116754 File Offset: 0x00114954
	public void OnCancelHighlightBlock()
	{
		bool flag = this._blockBorderLayout.parent == this._refers.transform;
		if (!flag)
		{
			this._blockBorderLayout.SetParent(this._refers.transform);
			this._blockBorderLayout.transform.SetSiblingIndex(this._borderLayoutOriginSiblingIndex);
		}
	}

	// Token: 0x060025FB RID: 9723 RVA: 0x001167B4 File Offset: 0x001149B4
	public LifeSkillCombatBlock FindBlock(Vector2Int position)
	{
		return this._blockList.Find((LifeSkillCombatBlock b) => b.Position == position);
	}

	// Token: 0x060025FC RID: 9724 RVA: 0x001167EC File Offset: 0x001149EC
	public bool HasOnlyOneBlockEffect(short effectTemplateId)
	{
		int count = this._blockList.Count((LifeSkillCombatBlock b) => b.DebateNodeEffectState != null && b.DebateNodeEffectState.TemplateId == (int)effectTemplateId);
		return count == 1;
	}

	// Token: 0x060025FD RID: 9725 RVA: 0x00116828 File Offset: 0x00114A28
	public LifeSkillCombatUnit FindUnit(int pawnId)
	{
		return this._unitList.Find((LifeSkillCombatUnit u) => u.Pawn.Id == pawnId);
	}

	// Token: 0x060025FE RID: 9726 RVA: 0x00116860 File Offset: 0x00114A60
	public void PlayCreateUnitAnim(bool play, Vector3 position, bool isSelf)
	{
		GameObject go = this._refers.CGet<GameObject>(isSelf ? "CreateUnitSelfEffect" : "CreateUnitEnemyEffect");
		go.transform.position = position;
		go.SetActive(play);
		if (play)
		{
			go.GetComponentInChildren<UIParticle>().Play();
		}
	}

	// Token: 0x060025FF RID: 9727 RVA: 0x001168AF File Offset: 0x00114AAF
	private void CreateUnit(Vector2Int position, sbyte grade)
	{
		this._actionCreateUnit(position, grade);
	}

	// Token: 0x06002600 RID: 9728 RVA: 0x001168C0 File Offset: 0x00114AC0
	public void CreateUnit(Vector2Int position, Pawn pawn, int bases)
	{
		bool findUnit = this._unitList.Exists((LifeSkillCombatUnit u) => u.Pawn.Id == pawn.Id);
		bool flag = findUnit;
		if (!flag)
		{
			AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundCreateUnit, false, true);
			LifeSkillCombatBlock block = this.FindBlock(position);
			this.PlayCreateUnitAnim(true, block.RectTrans.position, pawn.IsOwnedByTaiwu);
			DOVirtual.DelayedCall(0.2f, delegate
			{
				string key = pawn.IsOwnedByTaiwu ? "LifeSkillCombatUnitSelf" : "LifeSkillCombatUnitEnemy";
				Refers refers = PoolManager.GetObject<Refers>(key);
				Tester.Assert(refers != null, "");
				refers.transform.SetParent(this._unitLayout);
				refers.transform.localScale = Vector3.one;
				refers.transform.position = block.RectTrans.position;
				LifeSkillCombatUnit unit = new LifeSkillCombatUnit(refers, pawn, bases);
				this._unitList.Add(unit);
			}, false);
		}
	}

	// Token: 0x06002601 RID: 9729 RVA: 0x00116964 File Offset: 0x00114B64
	public void DeleteUnit(Pawn pawn)
	{
		LifeSkillCombatUnit findUnit = this.FindUnit(pawn.Id);
		this._unitList.Remove(findUnit);
		findUnit.RectTrans.SetParent(null);
		string key = pawn.IsOwnedByTaiwu ? "LifeSkillCombatUnitSelf" : "LifeSkillCombatUnitEnemy";
		PoolManager.Destroy(key, findUnit.RectTrans.gameObject);
	}

	// Token: 0x06002602 RID: 9730 RVA: 0x001169C0 File Offset: 0x00114BC0
	private void RefreshUnit(Pawn pawn, bool refreshStrategy = true)
	{
		LifeSkillCombatUnit findUnit = this.FindUnit(pawn.Id);
		if (findUnit != null)
		{
			findUnit.SetData(pawn, false, refreshStrategy);
		}
	}

	// Token: 0x06002603 RID: 9731 RVA: 0x001169EC File Offset: 0x00114BEC
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
			foreach (LifeSkillCombatUnit unit in this._unitList)
			{
				this.RefreshUnit(unit.Pawn, refreshStrategy);
			}
		}
	}

	// Token: 0x06002604 RID: 9732 RVA: 0x00116AE0 File Offset: 0x00114CE0
	public void ConflictUnit(Pawn selfPawn, Pawn enemyPawn, int result, IntPair powers)
	{
		LifeSkillCombatUnit selfUnit = this.FindUnit(selfPawn.Id);
		LifeSkillCombatUnit enemyUnit = this.FindUnit(enemyPawn.Id);
		selfUnit.RectTrans.SetParent(this.ConflictAnimRoot);
		enemyUnit.RectTrans.SetParent(this.ConflictAnimRoot);
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
			AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundConflict, false, true);
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
					selfUnit.RectTrans.SetParent(this._unitLayout);
					enemyUnit.RectTrans.SetParent(this._unitLayout);
				});
			}
			DOVirtual.DelayedCall(delay, callback, false);
		}, false);
	}

	// Token: 0x06002605 RID: 9733 RVA: 0x00116C04 File Offset: 0x00114E04
	public void MoveUnit(Pawn pawn, Vector2Int targetBlockPos)
	{
		AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundMoveUnit, false, true);
		LifeSkillCombatUnit findUnit = this.FindUnit(pawn.Id);
		LifeSkillCombatBlock findBlock = this.FindBlock(targetBlockPos);
		findUnit.RectTrans.DOMove(findBlock.RectTrans.position, 0.2f, false).SetEase(Ease.InOutExpo).OnComplete(delegate
		{
			findUnit.RectTrans.localScale = Vector3.one;
		});
	}

	// Token: 0x06002606 RID: 9734 RVA: 0x00116C80 File Offset: 0x00114E80
	public void RefreshAllBlock()
	{
		foreach (LifeSkillCombatBlock block in this._blockList)
		{
			IntPair coordinate = new IntPair(block.Position.x, block.Position.y);
			DebateNode debateNode = this.Model.DebateGame.DebateGrid[coordinate];
			block.Refresh(debateNode);
		}
	}

	// Token: 0x06002607 RID: 9735 RVA: 0x00116D18 File Offset: 0x00114F18
	private void InitUnitGradeArea()
	{
		RectTransform layout = this._unitGradeArea.CGet<RectTransform>("Layout");
		float rotationUnitAngel = -360f / (float)layout.childCount;
		for (int i = 0; i < layout.childCount; i++)
		{
			Refers refers = layout.GetChild(i).GetComponent<Refers>();
			Vector3 angel = new Vector3(0f, 0f, (float)i * rotationUnitAngel);
			refers.RectTransform.localEulerAngles = angel;
			refers.CGet<CImage>("Hover").SetSprite(this.GetGradeUnitHoverImage(i), false, null);
			TextMeshProUGUI gradeText = refers.CGet<TextMeshProUGUI>("Grade");
			gradeText.text = i.ToString();
			gradeText.rectTransform.localEulerAngles = -angel;
		}
	}

	// Token: 0x06002608 RID: 9736 RVA: 0x00116DE0 File Offset: 0x00114FE0
	public void RefreshUnitGradeArea(LifeSkillCombatBlock block, Action<int> onSelectGradeForStrategy = null, Action<int> onPreviewCost = null)
	{
		bool flag = !this._unitGradeArea.gameObject.activeSelf;
		if (flag)
		{
			this._unitGradeArea.gameObject.SetActive(true);
		}
		this._unitGradeArea.RectTransform.position = block.RectTrans.position;
		RectTransform layout = this._unitGradeArea.CGet<RectTransform>("Layout");
		if (this._unitGradeAreaEnterEvent == null)
		{
			this._unitGradeAreaEnterEvent = new UnityAction[layout.childCount];
		}
		if (this._unitGradeAreaExitEvent == null)
		{
			this._unitGradeAreaExitEvent = new UnityAction[layout.childCount];
		}
		this.SetUnitGradeAreaCost(0, 0);
		sbyte i = 0;
		UnityAction <>9__1;
		while ((int)i < layout.childCount)
		{
			sbyte itemGrade = i;
			Refers refers = layout.GetChild((int)i).GetComponent<Refers>();
			CButtonObsolete button = refers.CGet<CButtonObsolete>("Button");
			int cost = this.Model.DebateGame.GetPawnGradeToBase(this.Model.DebateGame.PlayerLeft.MaxBases, (int)itemGrade);
			cost = Mathf.Max(0, cost);
			bool costIsMeet = this.Model.DebateGame.PlayerLeft.Bases >= cost;
			button.interactable = costIsMeet;
			string image = this.GetGradeUnitImage((int)i, button.interactable);
			refers.CGet<CImage>("Image").SetSprite(image, false, null);
			TooltipInvoker tip = refers.CGet<TooltipInvoker>("Tip");
			tip.Type = TipType.SingleDesc;
			string[] presetParam = tip.PresetParam;
			bool flag2 = presetParam == null || presetParam.Length != 1;
			if (flag2)
			{
				tip.PresetParam = new string[1];
			}
			string color = costIsMeet ? "brightblue" : "brightred";
			string curValue = this.Model.DebateGame.PlayerLeft.Bases.ToString().SetColor(color);
			tip.PresetParam[0] = LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_CostEnergy_Tip, curValue, cost);
			PointerTrigger pointerTrigger = refers.CGet<PointerTrigger>("PointerTrigger");
			ref UnityAction enterEvent = ref this._unitGradeAreaEnterEvent[(int)i];
			bool flag3 = enterEvent != null;
			if (flag3)
			{
				pointerTrigger.EnterEvent.RemoveListener(enterEvent);
			}
			enterEvent = delegate()
			{
				AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundHoverCreateUnitPanel, false, true);
				this.SetUnitGradeAreaCost(cost, itemGrade);
				Action<int> onPreviewCost2 = onPreviewCost;
				if (onPreviewCost2 != null)
				{
					onPreviewCost2(cost);
				}
			};
			pointerTrigger.EnterEvent.AddListener(enterEvent);
			ref UnityAction exitEvent = ref this._unitGradeAreaExitEvent[(int)i];
			bool flag4 = exitEvent != null;
			if (flag4)
			{
				pointerTrigger.ExitEvent.RemoveListener(exitEvent);
			}
			ref UnityAction ptr = ref exitEvent;
			UnityAction unityAction;
			if ((unityAction = <>9__1) == null)
			{
				unityAction = (<>9__1 = delegate()
				{
					this.SetUnitGradeAreaCost(0, 0);
					Action<int> onPreviewCost2 = onPreviewCost;
					if (onPreviewCost2 != null)
					{
						onPreviewCost2(0);
					}
				});
			}
			ptr = unityAction;
			pointerTrigger.ExitEvent.AddListener(exitEvent);
			bool interactable = button.interactable;
			if (interactable)
			{
				button.ClearAndAddListener(delegate
				{
					sbyte j = 0;
					while ((int)j < layout.childCount)
					{
						Refers refers2 = layout.GetChild((int)j).GetComponent<Refers>();
						PointerTrigger pointerTrigger2 = refers2.CGet<PointerTrigger>("PointerTrigger");
						pointerTrigger2.EnterEvent.RemoveListener(this._unitGradeAreaEnterEvent[(int)j]);
						pointerTrigger2.ExitEvent.RemoveListener(this._unitGradeAreaExitEvent[(int)j]);
						j += 1;
					}
					bool flag5 = onSelectGradeForStrategy != null;
					if (flag5)
					{
						onSelectGradeForStrategy((int)itemGrade);
					}
					else
					{
						this.CreateUnit(block.Position, itemGrade);
					}
				});
			}
			i += 1;
		}
	}

	// Token: 0x06002609 RID: 9737 RVA: 0x00117130 File Offset: 0x00115330
	private string GetGradeUnitImage(int childIndex, bool interactable)
	{
		int imageIndex = 8 - childIndex;
		return interactable ? string.Format("lifeskillcombat_placebutton_{0}_0", imageIndex) : "lifeskillcombat_placebutton_gray";
	}

	// Token: 0x0600260A RID: 9738 RVA: 0x00117164 File Offset: 0x00115364
	private string GetGradeUnitHoverImage(int childIndex)
	{
		int imageIndex = 8 - childIndex;
		return string.Format("lifeskillcombat_placebutton_{0}_1", imageIndex);
	}

	// Token: 0x0600260B RID: 9739 RVA: 0x0011718C File Offset: 0x0011538C
	private void SetUnitGradeAreaCost(int cost, sbyte grade)
	{
		int[] costNumbers = new int[4];
		string numberStr = cost.ToString();
		int offset = costNumbers.Length - numberStr.Length;
		for (int i = 0; i < costNumbers.Length; i++)
		{
			bool flag = i < offset;
			if (flag)
			{
				costNumbers[i] = 0;
			}
			else
			{
				int startIndex = i - offset;
				string str = numberStr.Substring(startIndex, 1);
				costNumbers[i] = int.Parse(str);
			}
		}
		int length = numberStr.Length;
		if (!true)
		{
		}
		float num;
		switch (length)
		{
		case 1:
			num = 1f;
			break;
		case 2:
			num = 1f;
			break;
		case 3:
			num = 0.75f;
			break;
		case 4:
			num = 0.5f;
			break;
		default:
			num = 0.5f;
			break;
		}
		if (!true)
		{
		}
		float imageScale = num;
		RectTransform costNumberLayout = this._unitGradeArea.CGet<RectTransform>("CostNumberLayout");
		costNumberLayout.localScale = Vector3.one * imageScale;
		int startShowIndex = -1;
		for (int j = 0; j < costNumbers.Length; j++)
		{
			int number = costNumbers[j];
			CImage image = costNumberLayout.GetChild(j).GetComponent<CImage>();
			bool flag2 = number > 0 && startShowIndex < 0;
			if (flag2)
			{
				startShowIndex = j;
			}
			bool show = (startShowIndex >= 0 && j >= startShowIndex) || (cost == 0 && j == costNumbers.Length - 1);
			image.gameObject.SetActive(show);
			bool flag3 = show;
			if (flag3)
			{
				image.SetSprite(string.Format("lifeskillcombat_number_4_{0}", number), false, null);
			}
		}
	}

	// Token: 0x0600260C RID: 9740 RVA: 0x00117320 File Offset: 0x00115520
	public void ShowUnitGradeArea()
	{
		AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundShowCreateUnitPanel, false, true);
		bool flag = !this._unitGradeArea.gameObject.activeSelf;
		if (flag)
		{
			this._unitGradeArea.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600260D RID: 9741 RVA: 0x0011736C File Offset: 0x0011556C
	public void HideUnitGradeArea()
	{
		bool activeSelf = this._unitGradeArea.gameObject.activeSelf;
		if (activeSelf)
		{
			this._unitGradeArea.gameObject.SetActive(false);
		}
	}

	// Token: 0x04001C0F RID: 7183
	public const int Column = 6;

	// Token: 0x04001C10 RID: 7184
	public const int Row = 3;

	// Token: 0x04001C11 RID: 7185
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

	// Token: 0x04001C12 RID: 7186
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

	// Token: 0x04001C13 RID: 7187
	private const string LifeSkillCombatUnitSelfKey = "LifeSkillCombatUnitSelf";

	// Token: 0x04001C14 RID: 7188
	private const string LifeSkillCombatUnitEnemyKey = "LifeSkillCombatUnitEnemy";

	// Token: 0x04001C15 RID: 7189
	private Refers _refers;

	// Token: 0x04001C16 RID: 7190
	private RectTransform _blockLayout;

	// Token: 0x04001C17 RID: 7191
	private Refers _unitGradeArea;

	// Token: 0x04001C18 RID: 7192
	private RectTransform _unitLayout;

	// Token: 0x04001C19 RID: 7193
	private RectTransform _blockOperationLayout;

	// Token: 0x04001C1A RID: 7194
	private RectTransform _blockBorderLayout;

	// Token: 0x04001C1B RID: 7195
	private readonly List<LifeSkillCombatBlock> _blockList = new List<LifeSkillCombatBlock>();

	// Token: 0x04001C1C RID: 7196
	private readonly List<LifeSkillCombatUnit> _unitList = new List<LifeSkillCombatUnit>();

	// Token: 0x04001C1D RID: 7197
	private Action<Vector2Int, sbyte> _actionCreateUnit;

	// Token: 0x04001C1E RID: 7198
	private int _borderLayoutOriginSiblingIndex;

	// Token: 0x04001C1F RID: 7199
	private UnityAction[] _unitGradeAreaEnterEvent;

	// Token: 0x04001C20 RID: 7200
	private UnityAction[] _unitGradeAreaExitEvent;
}
