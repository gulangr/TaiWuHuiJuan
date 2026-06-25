using System;
using FrameWork;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B8B RID: 2955
	public class SkillBreakCellSelected : SkillBreakCellBase
	{
		// Token: 0x060091DE RID: 37342 RVA: 0x0043EC44 File Offset: 0x0043CE44
		protected override void RefreshInternal()
		{
			bool isNormal = base.Template.Type == ESkillBreakGridTypeType.Normal;
			bool isActive = !isNormal;
			this.normalCurrentLayout.SetActive(isNormal);
			this.specialCurrentLayout.SetActive(!isNormal);
			CommonUtils.SetActiveByMove(this.specialNameArea, isActive);
			bool flag = isActive;
			if (flag)
			{
				this.specialName.text = this._cellData.Template.Name;
			}
			bool isFailed = this._cellData.State == ESkillBreakGridState.Failed;
			bool flag2 = isFailed;
			if (flag2)
			{
				this.bg.SetSprite(isNormal ? "ui9_back_charactermenu_skillbreak_ruchang_6" : "ui9_back_charactermenu_skillbreak_special_5", true, null);
			}
			else
			{
				this.bg.SetSprite(isNormal ? "ui9_back_charactermenu_skillbreak_ruchang_5" : "ui9_back_charactermenu_skillbreak_special_4", true, null);
				this.hover.SetSprite("ui9_back_charactermenu_skillbreak_ruchang_5", true, null);
			}
			base.RefreshPowerLabel(this.powerLabel);
			this.RefreshNormalCellTip(this.bg.GetComponent<TooltipInvoker>());
		}

		// Token: 0x060091DF RID: 37343 RVA: 0x0043ED3C File Offset: 0x0043CF3C
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
				tip.RuntimeParam.SetObject("Plate", this._plate).Set("SkillId", this._skillId).SetObject("LifeSkillAttainments", this._lifeSkillAttainments).Set<SkillBreakPlateIndex>("Coord", this._coordinate).Set("NeedExp", base.GetNeedExp()).Set("CurrentExp", this._currentExp);
				tip.Refresh(false, -1);
			}
		}

		// Token: 0x0400705F RID: 28767
		[Header("已选择格子组件")]
		[SerializeField]
		private CImage bg;

		// Token: 0x04007060 RID: 28768
		[SerializeField]
		private CImage hover;

		// Token: 0x04007061 RID: 28769
		[SerializeField]
		private TextMeshProUGUI powerLabel;

		// Token: 0x04007062 RID: 28770
		[SerializeField]
		private RectTransform specialNameArea;

		// Token: 0x04007063 RID: 28771
		[SerializeField]
		private TextMeshProUGUI specialName;

		// Token: 0x04007064 RID: 28772
		[SerializeField]
		private GameObject normalCurrentLayout;

		// Token: 0x04007065 RID: 28773
		[SerializeField]
		private GameObject specialCurrentLayout;

		// Token: 0x04007066 RID: 28774
		private RectTransform _current;
	}
}
