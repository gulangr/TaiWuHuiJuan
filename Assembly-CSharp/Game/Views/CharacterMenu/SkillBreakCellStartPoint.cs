using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B8D RID: 2957
	public class SkillBreakCellStartPoint : SkillBreakCellBase
	{
		// Token: 0x060091E7 RID: 37351 RVA: 0x0043F0CC File Offset: 0x0043D2CC
		protected override void RefreshInternal()
		{
			this.txtNameObj.SetActive(this._plate.SelectPath != null && this._plate.SelectPath.Count > 0 && this._plate.SelectPath[0] == this._coordinate);
			base.BindButtonEvent(this.startButton);
			this.RefreshButton(base.CanSelect());
			this.RefreshNormalCellTip(this.startButton.GetComponent<TooltipInvoker>());
			this.RefreshStartPointName();
		}

		// Token: 0x060091E8 RID: 37352 RVA: 0x0043F158 File Offset: 0x0043D358
		private void RefreshStartPointName()
		{
			this.txtName.text = CombatSkill.Instance[this._skillId].BreakStart;
		}

		// Token: 0x060091E9 RID: 37353 RVA: 0x0043F17C File Offset: 0x0043D37C
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
				tip.RuntimeParam.Set("SkillId", this._skillId).Set("StartPoint", true);
				tip.Refresh(false, -1);
			}
		}

		// Token: 0x060091EA RID: 37354 RVA: 0x0043F1E5 File Offset: 0x0043D3E5
		private void RefreshButton(bool interactable)
		{
			this.startButton.interactable = interactable;
			this.startButton.GetComponent<PointerTrigger>().enabled = interactable;
		}

		// Token: 0x060091EB RID: 37355 RVA: 0x0043F207 File Offset: 0x0043D407
		public override void SetStartPointGray(bool setGray)
		{
			base.SetAllImageDim(setGray);
		}

		// Token: 0x04007070 RID: 28784
		[Header("起点格子组件")]
		[SerializeField]
		private CButton startButton;

		// Token: 0x04007071 RID: 28785
		[SerializeField]
		private GameObject txtNameObj;

		// Token: 0x04007072 RID: 28786
		[SerializeField]
		private TextMeshProUGUI txtName;
	}
}
