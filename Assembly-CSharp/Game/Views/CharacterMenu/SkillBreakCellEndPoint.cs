using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B88 RID: 2952
	public class SkillBreakCellEndPoint : SkillBreakCellBase
	{
		// Token: 0x060091CE RID: 37326 RVA: 0x0043E610 File Offset: 0x0043C810
		protected override void RefreshInternal()
		{
			this.RefreshButton(base.CanSelect());
			this.RefreshNormalCellTip(this.endButton.GetComponent<TooltipInvoker>());
			this.RefreshEndPointCellName();
		}

		// Token: 0x060091CF RID: 37327 RVA: 0x0043E639 File Offset: 0x0043C839
		private void RefreshEndPointCellName()
		{
			this.txtName.text = CombatSkill.Instance[this._skillId].BreakEnd;
		}

		// Token: 0x060091D0 RID: 37328 RVA: 0x0043E660 File Offset: 0x0043C860
		private void RefreshNormalCellTip(TooltipInvoker tip)
		{
			bool flag = tip == null;
			if (!flag)
			{
				if (tip.RuntimeParam == null)
				{
					tip.RuntimeParam = new ArgumentBox();
				}
				tip.Type = TipType.SkillBreakNormalCell;
				tip.RuntimeParam.SetObject("Plate", this._plate).Set("SkillId", this._skillId).Set("EndPoint", true).Set<SkillBreakPlateIndex>("Coord", this._coordinate).Set("NeedExp", base.GetNeedExp()).Set("CurrentExp", this._currentExp);
				tip.Refresh(false, -1);
			}
		}

		// Token: 0x060091D1 RID: 37329 RVA: 0x0043E70C File Offset: 0x0043C90C
		private void RefreshButton(bool interactable)
		{
			this.endButton.interactable = interactable;
			base.BindButtonEvent(this.endButton);
			this.endButton.GetComponent<PointerTrigger>().enabled = interactable;
		}

		// Token: 0x04007050 RID: 28752
		[Header("终点格子组件")]
		[SerializeField]
		private CButton endButton;

		// Token: 0x04007051 RID: 28753
		[SerializeField]
		private TextMeshProUGUI txtName;
	}
}
