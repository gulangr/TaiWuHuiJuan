using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B85 RID: 2949
	public abstract class SkillBreakCellBase : MonoBehaviour
	{
		// Token: 0x17000FC2 RID: 4034
		// (get) Token: 0x060091A0 RID: 37280 RVA: 0x0043D944 File Offset: 0x0043BB44
		protected SkillBreakPlateBonus Bonus
		{
			get
			{
				return this._plate.GetBonus(this._coordinate);
			}
		}

		// Token: 0x17000FC3 RID: 4035
		// (get) Token: 0x060091A1 RID: 37281 RVA: 0x0043D957 File Offset: 0x0043BB57
		protected int Power
		{
			get
			{
				return this._plate.CalcAddMaxPower(this._coordinate);
			}
		}

		// Token: 0x17000FC4 RID: 4036
		// (get) Token: 0x060091A2 RID: 37282 RVA: 0x0043D96A File Offset: 0x0043BB6A
		protected sbyte TemplateId
		{
			get
			{
				return this._cellData.TemplateId;
			}
		}

		// Token: 0x17000FC5 RID: 4037
		// (get) Token: 0x060091A3 RID: 37283 RVA: 0x0043D977 File Offset: 0x0043BB77
		protected SkillBreakGridTypeItem Template
		{
			get
			{
				return this._cellData.Template;
			}
		}

		// Token: 0x17000FC6 RID: 4038
		// (get) Token: 0x060091A4 RID: 37284 RVA: 0x0043D984 File Offset: 0x0043BB84
		protected bool IsDimOrDisableApplied
		{
			get
			{
				return this._isDimOrDisableApplied;
			}
		}

		// Token: 0x060091A5 RID: 37285 RVA: 0x0043D98C File Offset: 0x0043BB8C
		protected virtual void OnAwake()
		{
		}

		// Token: 0x060091A6 RID: 37286 RVA: 0x0043D990 File Offset: 0x0043BB90
		protected void Awake()
		{
			if (SkillBreakCellBase._powerColors == null)
			{
				SkillBreakCellBase._powerColors = new Color[]
				{
					Colors.Instance["lightgrey"],
					Colors.Instance["pinkyellow"],
					Colors.Instance["GradeColor_6"]
				};
			}
			SkillBreakCellBase._power3Color = Colors.Instance["skillbreakyellow"];
			this.OnAwake();
			this._colorMultiplyStyleRoot = base.GetComponent<ColorMultiplyStyleRoot>();
			this.CacheTmpTexts();
			this.BackupAllTmpTextColors();
		}

		// Token: 0x060091A7 RID: 37287 RVA: 0x0043DA28 File Offset: 0x0043BC28
		private void CacheTmpTexts()
		{
			this._cachedTmpTexts.Clear();
			base.GetComponentsInChildren<TextMeshProUGUI>(true, this._cachedTmpTexts);
			TextMeshProUGUI selfText = base.GetComponent<TextMeshProUGUI>();
			bool flag = selfText != null && !this._cachedTmpTexts.Contains(selfText);
			if (flag)
			{
				this._cachedTmpTexts.Add(selfText);
			}
		}

		// Token: 0x060091A8 RID: 37288 RVA: 0x0043DA84 File Offset: 0x0043BC84
		private void BackupAllTmpTextColors()
		{
			this._cachedTmpTextBaseColors.Clear();
			foreach (TextMeshProUGUI tmp in this._cachedTmpTexts)
			{
				bool flag = tmp == null;
				if (!flag)
				{
					this._cachedTmpTextBaseColors[tmp] = tmp.color;
				}
			}
		}

		// Token: 0x060091A9 RID: 37289 RVA: 0x0043DB04 File Offset: 0x0043BD04
		protected void BackupTmpTextColor(TextMeshProUGUI tmp)
		{
			bool flag = tmp == null;
			if (!flag)
			{
				this._cachedTmpTextBaseColors[tmp] = tmp.color;
			}
		}

		// Token: 0x060091AA RID: 37290 RVA: 0x0043DB32 File Offset: 0x0043BD32
		public void OnPointerEnter()
		{
			Action<SkillBreakPlateIndex> onPointerEnter = this._onPointerEnter;
			if (onPointerEnter != null)
			{
				onPointerEnter(this._coordinate);
			}
		}

		// Token: 0x060091AB RID: 37291 RVA: 0x0043DB50 File Offset: 0x0043BD50
		public void OnPointerExit()
		{
			bool activeInHierarchy = base.gameObject.activeInHierarchy;
			if (activeInHierarchy)
			{
				Action<SkillBreakPlateIndex> onPointerExit = this._onPointerExit;
				if (onPointerExit != null)
				{
					onPointerExit(this._coordinate);
				}
			}
		}

		// Token: 0x060091AC RID: 37292 RVA: 0x0043DB88 File Offset: 0x0043BD88
		public virtual void Refresh(GameData.Domains.Taiwu.SkillBreakPlate plate, SkillBreakPlateGrid cellData, SkillBreakPlateIndex coordinate, Action<SkillBreakPlateIndex> onCellClicked, short skillId, LifeSkillShorts lifeSkillAttainments, int currentExp, int baseNeedExp, IAsyncMethodRequestHandler requestHandler = null, GameData.Domains.Taiwu.SkillBreakPlate oldPlate = null, Action<SkillBreakPlateIndex> onPointerEnter = null, Action<SkillBreakPlateIndex> onPointerExit = null)
		{
			this._plate = plate;
			this._oldPlate = oldPlate;
			this._cellData = cellData;
			this._coordinate = coordinate;
			this._onCellClicked = onCellClicked;
			this._onPointerEnter = onPointerEnter;
			this._onPointerExit = onPointerExit;
			this._requestHandler = requestHandler;
			this._currentExp = currentExp;
			this._baseNeedExp = baseNeedExp;
			this._skillId = skillId;
			this._lifeSkillAttainments = lifeSkillAttainments;
			this._valueFactor = 1f;
			this._disableStyle = false;
			this.RefreshInternal();
			this.RefreshCommon(plate, coordinate);
			this.SetSwapSelectable(false);
			this.SetSwapSelected(false);
			this.ApplyStyleNow();
		}

		// Token: 0x060091AD RID: 37293 RVA: 0x0043DC2C File Offset: 0x0043BE2C
		protected void RefreshCommon(GameData.Domains.Taiwu.SkillBreakPlate plate, SkillBreakPlateIndex coordinate)
		{
			bool flag = this.current != null;
			if (flag)
			{
				CommonUtils.SetActiveByMove(this.current, plate.Current == coordinate);
			}
			ESkillBreakGridTypeType type = this._cellData.Template.Type;
			bool isDisableStyle = type != ESkillBreakGridTypeType.Bonus && type != ESkillBreakGridTypeType.EndPoint && this._cellData.State == ESkillBreakGridState.CanSelect && (this.GetNeedExp() > this._currentExp || (this._plate.StepExhausted && this._plate.CalcCostStep(coordinate) > 0));
			this._disableStyle = isDisableStyle;
		}

		// Token: 0x060091AE RID: 37294
		protected abstract void RefreshInternal();

		// Token: 0x060091AF RID: 37295 RVA: 0x0043DCC6 File Offset: 0x0043BEC6
		public virtual void SetStartPointGray(bool setGray)
		{
		}

		// Token: 0x060091B0 RID: 37296 RVA: 0x0043DCC9 File Offset: 0x0043BEC9
		public virtual void SetAllImageDoubleDim()
		{
		}

		// Token: 0x060091B1 RID: 37297 RVA: 0x0043DCCC File Offset: 0x0043BECC
		protected void RefreshPowerLabel(TextMeshProUGUI label)
		{
			bool flag = label == null;
			if (!flag)
			{
				label.text = this.Power.ToString();
				label.color = ((this.Power >= 3) ? SkillBreakCellBase._power3Color : SkillBreakCellBase._powerColors[Mathf.Clamp(this.Power, 0, 2)]);
				this.BackupTmpTextColor(label);
			}
		}

		// Token: 0x060091B2 RID: 37298 RVA: 0x0043DD34 File Offset: 0x0043BF34
		protected void BindButtonEvent(CButton button)
		{
			bool flag = button == null;
			if (!flag)
			{
				button.ClearAndAddListener(delegate
				{
					Action<SkillBreakPlateIndex> onCellClicked = this._onCellClicked;
					if (onCellClicked != null)
					{
						onCellClicked(this._coordinate);
					}
				});
			}
		}

		// Token: 0x060091B3 RID: 37299 RVA: 0x0043DD64 File Offset: 0x0043BF64
		private void RefreshDisableAndDim()
		{
			if (this._colorMultiplyStyleRoot == null)
			{
				this._colorMultiplyStyleRoot = base.GetComponent<ColorMultiplyStyleRoot>();
			}
			bool flag = this._colorMultiplyStyleRoot == null;
			if (!flag)
			{
				bool shouldDimOrDisable = this._disableStyle || Mathf.Abs(this._valueFactor - 1f) > 0.0001f;
				bool flag2 = !shouldDimOrDisable && !this._isDimOrDisableApplied;
				if (flag2)
				{
					this._isDimOrDisableApplied = false;
				}
				else
				{
					bool flag3 = this._cachedTmpTexts.Count == 0;
					if (flag3)
					{
						this.CacheTmpTexts();
						this.BackupAllTmpTextColors();
					}
					this._colorMultiplyStyleRoot.RestoreAllToWhite();
					foreach (KeyValuePair<TextMeshProUGUI, Color> keyValuePair in this._cachedTmpTextBaseColors)
					{
						TextMeshProUGUI textMeshProUGUI;
						Color color2;
						keyValuePair.Deconstruct(out textMeshProUGUI, out color2);
						TextMeshProUGUI tmp = textMeshProUGUI;
						Color color = color2;
						bool flag4 = tmp == null;
						if (!flag4)
						{
							tmp.color = color;
						}
					}
					bool flag5 = !shouldDimOrDisable;
					if (flag5)
					{
						this._isDimOrDisableApplied = false;
					}
					else
					{
						this._colorMultiplyStyleRoot.MultiplyColor(new Vector4(0.5f, 0.5f, 0.5f, 1f));
						this._isDimOrDisableApplied = true;
					}
				}
			}
		}

		// Token: 0x060091B4 RID: 37300 RVA: 0x0043DEC0 File Offset: 0x0043C0C0
		protected void SetAllImageDim(bool dim)
		{
			this._valueFactor = (dim ? 0.4f : 1f);
		}

		// Token: 0x060091B5 RID: 37301 RVA: 0x0043DED8 File Offset: 0x0043C0D8
		protected void ApplyStyleNow()
		{
			this.RefreshDisableAndDim();
		}

		// Token: 0x060091B6 RID: 37302 RVA: 0x0043DEE4 File Offset: 0x0043C0E4
		protected int GetNeedExp()
		{
			return SkillBreakPlateUtils.GetNeedExp(this._skillId, this._plate, this._coordinate, this._baseNeedExp);
		}

		// Token: 0x060091B7 RID: 37303 RVA: 0x0043DF14 File Offset: 0x0043C114
		protected bool CanSelect()
		{
			return this._cellData.State == ESkillBreakGridState.CanSelect && this.GetNeedExp() <= this._currentExp && this._plate.CanSelectBreak(this._coordinate);
		}

		// Token: 0x060091B8 RID: 37304 RVA: 0x0043DF58 File Offset: 0x0043C158
		public virtual void SetHighlight(bool highlight)
		{
			bool flag = this.hoverObject == null;
			if (!flag)
			{
				this.hoverObject.SetActive(highlight);
			}
		}

		// Token: 0x060091B9 RID: 37305 RVA: 0x0043DF85 File Offset: 0x0043C185
		public virtual void SetSwapSelectable(bool selectable)
		{
		}

		// Token: 0x060091BA RID: 37306 RVA: 0x0043DF88 File Offset: 0x0043C188
		public void SetSwapSelected(bool selected)
		{
			bool flag = this.swapSelectedIndicator == null;
			if (!flag)
			{
				this.swapSelectedIndicator.SetActive(selected);
			}
		}

		// Token: 0x17000FC7 RID: 4039
		// (get) Token: 0x060091BB RID: 37307 RVA: 0x0043DFB5 File Offset: 0x0043C1B5
		internal SkillBreakPlateIndex Coordinate
		{
			get
			{
				return this._coordinate;
			}
		}

		// Token: 0x04007029 RID: 28713
		protected GameData.Domains.Taiwu.SkillBreakPlate _plate;

		// Token: 0x0400702A RID: 28714
		protected GameData.Domains.Taiwu.SkillBreakPlate _oldPlate;

		// Token: 0x0400702B RID: 28715
		protected SkillBreakPlateGrid _cellData;

		// Token: 0x0400702C RID: 28716
		protected SkillBreakPlateIndex _coordinate;

		// Token: 0x0400702D RID: 28717
		protected Action<SkillBreakPlateIndex> _onCellClicked;

		// Token: 0x0400702E RID: 28718
		protected Action<SkillBreakPlateIndex> _onPointerEnter;

		// Token: 0x0400702F RID: 28719
		protected Action<SkillBreakPlateIndex> _onPointerExit;

		// Token: 0x04007030 RID: 28720
		protected IAsyncMethodRequestHandler _requestHandler;

		// Token: 0x04007031 RID: 28721
		protected short _skillId;

		// Token: 0x04007032 RID: 28722
		protected int _currentExp;

		// Token: 0x04007033 RID: 28723
		protected int _baseNeedExp;

		// Token: 0x04007034 RID: 28724
		protected LifeSkillShorts _lifeSkillAttainments;

		// Token: 0x04007035 RID: 28725
		[Header("通用组件")]
		[SerializeField]
		protected RectTransform current;

		// Token: 0x04007036 RID: 28726
		[SerializeField]
		protected GameObject hoverObject;

		// Token: 0x04007037 RID: 28727
		[SerializeField]
		private GameObject swapSelectedIndicator;

		// Token: 0x04007038 RID: 28728
		protected bool _disableStyle;

		// Token: 0x04007039 RID: 28729
		protected float _valueFactor = 1f;

		// Token: 0x0400703A RID: 28730
		protected static Color[] _powerColors;

		// Token: 0x0400703B RID: 28731
		protected static Color _power3Color;

		// Token: 0x0400703C RID: 28732
		private readonly List<TextMeshProUGUI> _cachedTmpTexts = new List<TextMeshProUGUI>();

		// Token: 0x0400703D RID: 28733
		private readonly Dictionary<TextMeshProUGUI, Color> _cachedTmpTextBaseColors = new Dictionary<TextMeshProUGUI, Color>();

		// Token: 0x0400703E RID: 28734
		private ColorMultiplyStyleRoot _colorMultiplyStyleRoot;

		// Token: 0x0400703F RID: 28735
		private bool _isDimOrDisableApplied;
	}
}
