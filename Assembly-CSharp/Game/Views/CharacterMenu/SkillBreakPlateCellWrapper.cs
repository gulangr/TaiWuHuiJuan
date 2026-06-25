using System;
using System.Collections.Generic;
using GameData.Domains.Character;
using GameData.Domains.Taiwu;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B91 RID: 2961
	public class SkillBreakPlateCellWrapper : MonoBehaviour
	{
		// Token: 0x17000FC9 RID: 4041
		// (get) Token: 0x060091FD RID: 37373 RVA: 0x0043F91D File Offset: 0x0043DB1D
		public SkillBreakCellBase ActiveCell
		{
			get
			{
				return this._activeCell;
			}
		}

		// Token: 0x17000FCA RID: 4042
		// (get) Token: 0x060091FE RID: 37374 RVA: 0x0043F925 File Offset: 0x0043DB25
		public SkillBreakPlateIndex CurrentCoordinate
		{
			get
			{
				SkillBreakCellBase activeCell = this._activeCell;
				return (activeCell != null) ? activeCell.Coordinate : SkillBreakPlateIndex.Invalid;
			}
		}

		// Token: 0x17000FCB RID: 4043
		// (get) Token: 0x060091FF RID: 37375 RVA: 0x0043F93D File Offset: 0x0043DB3D
		public RectTransform RectTransform
		{
			get
			{
				return base.GetComponent<RectTransform>();
			}
		}

		// Token: 0x06009200 RID: 37376 RVA: 0x0043F948 File Offset: 0x0043DB48
		public void Refresh(SkillBreakPlate plate, SkillBreakPlateGrid cellData, SkillBreakPlateIndex coordinate, Action<SkillBreakPlateIndex> onCellClicked, short skillId, LifeSkillShorts lifeSkillAttainments, int currentExp, int baseNeedExp, IAsyncMethodRequestHandler requestHandler = null, SkillBreakPlate oldPlate = null, Action<SkillBreakPlateIndex> onPointerEnter = null, Action<SkillBreakPlateIndex> onPointerExit = null)
		{
			SkillBreakCellBase templateToUse = this.GetTemplateForCellType(cellData);
			this.HideAllCells();
			SkillBreakCellBase cellToActivate = this.EnsureAndGetCell(templateToUse);
			bool flag = cellToActivate;
			if (flag)
			{
				this._activeCell = cellToActivate;
				CommonUtils.SetActiveByMove(cellToActivate.GetComponent<RectTransform>(), true);
				cellToActivate.Refresh(plate, cellData, coordinate, onCellClicked, skillId, lifeSkillAttainments, currentExp, baseNeedExp, requestHandler, oldPlate, onPointerEnter, onPointerExit);
				return;
			}
			throw new InvalidOperationException(string.Format("无法为格子类型 {0} 创建或获取对应的单元格预制体。请检查预制体设置和类型匹配。", cellData.Template.Type));
		}

		// Token: 0x06009201 RID: 37377 RVA: 0x0043F9CC File Offset: 0x0043DBCC
		private SkillBreakCellBase GetTemplateForCellType(SkillBreakPlateGrid cellData)
		{
			bool flag = cellData.State == ESkillBreakGridState.Invisible;
			SkillBreakCellBase result;
			if (flag)
			{
				result = this.emptyCellTemplate;
			}
			else
			{
				switch (cellData.Template.Type)
				{
				case ESkillBreakGridTypeType.StartPoint:
					return this.startPointCellTemplate;
				case ESkillBreakGridTypeType.EndPoint:
					return this.endPointCellTemplate;
				case ESkillBreakGridTypeType.Bonus:
					return this.bonusCellTemplate;
				}
				bool flag2 = cellData.State == ESkillBreakGridState.Selected || cellData.State == ESkillBreakGridState.Failed;
				if (flag2)
				{
					result = this.selectedCellTemplate;
				}
				else
				{
					bool flag3 = cellData.Template.Type == ESkillBreakGridTypeType.Normal;
					if (flag3)
					{
						result = this.normalCellTemplate;
					}
					else
					{
						result = this.specialCellTemplate;
					}
				}
			}
			return result;
		}

		// Token: 0x06009202 RID: 37378 RVA: 0x0043FA84 File Offset: 0x0043DC84
		private SkillBreakCellBase EnsureAndGetCell(SkillBreakCellBase template)
		{
			bool flag = !template;
			SkillBreakCellBase result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = template == this.emptyCellTemplate;
				if (flag2)
				{
					bool flag3 = this._emptyCellInstance;
					if (flag3)
					{
						result = this._emptyCellInstance;
					}
					else
					{
						this._emptyCellInstance = Object.Instantiate<SkillBreakCellEmpty>(this.emptyCellTemplate, base.transform);
						CommonUtils.SetActiveByMove(this._emptyCellInstance.GetComponent<RectTransform>(), false);
						this._allCellInstances.Add(this._emptyCellInstance);
						result = this._emptyCellInstance;
					}
				}
				else
				{
					bool flag4 = template == this.startPointCellTemplate;
					if (flag4)
					{
						bool flag5 = this._startPointCellInstance;
						if (flag5)
						{
							result = this._startPointCellInstance;
						}
						else
						{
							this._startPointCellInstance = Object.Instantiate<SkillBreakCellStartPoint>(this.startPointCellTemplate, base.transform);
							CommonUtils.SetActiveByMove(this._startPointCellInstance.GetComponent<RectTransform>(), false);
							this._allCellInstances.Add(this._startPointCellInstance);
							result = this._startPointCellInstance;
						}
					}
					else
					{
						bool flag6 = template == this.endPointCellTemplate;
						if (flag6)
						{
							bool flag7 = this._endPointCellInstance;
							if (flag7)
							{
								result = this._endPointCellInstance;
							}
							else
							{
								this._endPointCellInstance = Object.Instantiate<SkillBreakCellEndPoint>(this.endPointCellTemplate, base.transform);
								CommonUtils.SetActiveByMove(this._endPointCellInstance.GetComponent<RectTransform>(), false);
								this._allCellInstances.Add(this._endPointCellInstance);
								result = this._endPointCellInstance;
							}
						}
						else
						{
							bool flag8 = template == this.normalCellTemplate;
							if (flag8)
							{
								bool flag9 = this._normalCellInstance;
								if (flag9)
								{
									result = this._normalCellInstance;
								}
								else
								{
									this._normalCellInstance = Object.Instantiate<SkillBreakCellNormal>(this.normalCellTemplate, base.transform);
									CommonUtils.SetActiveByMove(this._normalCellInstance.GetComponent<RectTransform>(), false);
									this._allCellInstances.Add(this._normalCellInstance);
									result = this._normalCellInstance;
								}
							}
							else
							{
								bool flag10 = template == this.specialCellTemplate;
								if (flag10)
								{
									bool flag11 = this._specialCellInstance;
									if (flag11)
									{
										result = this._specialCellInstance;
									}
									else
									{
										this._specialCellInstance = Object.Instantiate<SkillBreakCellSpecial>(this.specialCellTemplate, base.transform);
										CommonUtils.SetActiveByMove(this._specialCellInstance.GetComponent<RectTransform>(), false);
										this._allCellInstances.Add(this._specialCellInstance);
										result = this._specialCellInstance;
									}
								}
								else
								{
									bool flag12 = template == this.selectedCellTemplate;
									if (flag12)
									{
										bool flag13 = this._selectedCellInstance;
										if (flag13)
										{
											result = this._selectedCellInstance;
										}
										else
										{
											this._selectedCellInstance = Object.Instantiate<SkillBreakCellSelected>(this.selectedCellTemplate, base.transform);
											CommonUtils.SetActiveByMove(this._selectedCellInstance.GetComponent<RectTransform>(), false);
											this._allCellInstances.Add(this._selectedCellInstance);
											result = this._selectedCellInstance;
										}
									}
									else
									{
										bool flag14 = template == this.bonusCellTemplate;
										if (flag14)
										{
											bool flag15 = this._bonusCellInstance;
											if (flag15)
											{
												result = this._bonusCellInstance;
											}
											else
											{
												this._bonusCellInstance = Object.Instantiate<SkillBreakCellBonus>(this.bonusCellTemplate, base.transform);
												CommonUtils.SetActiveByMove(this._bonusCellInstance.GetComponent<RectTransform>(), false);
												this._allCellInstances.Add(this._bonusCellInstance);
												result = this._bonusCellInstance;
											}
										}
										else
										{
											result = null;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06009203 RID: 37379 RVA: 0x0043FDE0 File Offset: 0x0043DFE0
		private void HideAllCells()
		{
			foreach (SkillBreakCellBase cell in this._allCellInstances)
			{
				bool flag = !cell;
				if (!flag)
				{
					CommonUtils.SetActiveByMove(cell.GetComponent<RectTransform>(), false);
					cell.SetSwapSelectable(false);
					cell.SetSwapSelected(false);
				}
			}
			this._activeCell = null;
		}

		// Token: 0x06009204 RID: 37380 RVA: 0x0043FE64 File Offset: 0x0043E064
		public void SetStartPointGray(bool setGray)
		{
			this._activeCell.SetStartPointGray(setGray);
		}

		// Token: 0x06009205 RID: 37381 RVA: 0x0043FE74 File Offset: 0x0043E074
		public void SetNormalAndSpecialContentAllImageDoubleDim()
		{
			this._activeCell.SetAllImageDoubleDim();
		}

		// Token: 0x06009206 RID: 37382 RVA: 0x0043FE83 File Offset: 0x0043E083
		public void SetHighlight(bool hovering)
		{
			this._activeCell.SetHighlight(hovering);
		}

		// Token: 0x06009207 RID: 37383 RVA: 0x0043FE93 File Offset: 0x0043E093
		public void SetSwapPresentation(bool selectable, bool selected)
		{
			SkillBreakCellBase activeCell = this._activeCell;
			if (activeCell != null)
			{
				activeCell.SetSwapSelectable(selectable);
			}
			SkillBreakCellBase activeCell2 = this._activeCell;
			if (activeCell2 != null)
			{
				activeCell2.SetSwapSelected(selected);
			}
		}

		// Token: 0x0400707E RID: 28798
		[SerializeField]
		private SkillBreakCellEmpty emptyCellTemplate;

		// Token: 0x0400707F RID: 28799
		[SerializeField]
		private SkillBreakCellStartPoint startPointCellTemplate;

		// Token: 0x04007080 RID: 28800
		[SerializeField]
		private SkillBreakCellEndPoint endPointCellTemplate;

		// Token: 0x04007081 RID: 28801
		[SerializeField]
		private SkillBreakCellNormal normalCellTemplate;

		// Token: 0x04007082 RID: 28802
		[SerializeField]
		private SkillBreakCellSpecial specialCellTemplate;

		// Token: 0x04007083 RID: 28803
		[SerializeField]
		private SkillBreakCellSelected selectedCellTemplate;

		// Token: 0x04007084 RID: 28804
		[SerializeField]
		private SkillBreakCellBonus bonusCellTemplate;

		// Token: 0x04007085 RID: 28805
		private SkillBreakCellEmpty _emptyCellInstance;

		// Token: 0x04007086 RID: 28806
		private SkillBreakCellStartPoint _startPointCellInstance;

		// Token: 0x04007087 RID: 28807
		private SkillBreakCellEndPoint _endPointCellInstance;

		// Token: 0x04007088 RID: 28808
		private SkillBreakCellNormal _normalCellInstance;

		// Token: 0x04007089 RID: 28809
		private SkillBreakCellSpecial _specialCellInstance;

		// Token: 0x0400708A RID: 28810
		private SkillBreakCellSelected _selectedCellInstance;

		// Token: 0x0400708B RID: 28811
		private SkillBreakCellBonus _bonusCellInstance;

		// Token: 0x0400708C RID: 28812
		private readonly List<SkillBreakCellBase> _allCellInstances = new List<SkillBreakCellBase>();

		// Token: 0x0400708D RID: 28813
		private SkillBreakCellBase _activeCell;
	}
}
