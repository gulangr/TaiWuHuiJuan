using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Views.LegacyPassing
{
	// Token: 0x0200099B RID: 2459
	public class ViewSelectRandomSuccessor : UIBase
	{
		// Token: 0x0600766A RID: 30314 RVA: 0x00373414 File Offset: 0x00371614
		private void Awake()
		{
			this.toggleGroup.Init(-1);
			this.toggleGroup.OnActiveIndexChange += delegate(int i, int _)
			{
				this.confirm.interactable = !(this.confirmDisplayer.enabled = (i == -1));
			};
			this.confirm.onClick.ResetListener(delegate()
			{
				this.QuickHide();
				DisplayTriggerModel.ShowInheritUILegacy(this._charList[this.toggleGroup.GetActiveIndex()].TeammateData.CharacterId);
			});
		}

		// Token: 0x0600766B RID: 30315 RVA: 0x00373464 File Offset: 0x00371664
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<List<SuccessorCandidates>>("Candidates", out this._charList);
			this.toggleGroup.DeSelect(false);
			this.confirm.interactable = false;
			this.confirmDisplayer.enabled = true;
			List<SuccessorCandidates> charList = this._charList;
			bool flag = charList == null || charList.Count <= 1;
			if (flag)
			{
				string format = "invalid character count: expect 2, got {0}";
				List<SuccessorCandidates> charList2 = this._charList;
				Debug.LogError(string.Format(format, (charList2 != null) ? charList2.Count : -1));
			}
			else
			{
				this.left.Set(this._charList[0].DisplayData, this._charList[0].CombatSkillQualifications, this._charList[0].CombatSkillAttainments, this._charList[0].LifeSkillQualifications, this._charList[0].LifeSkillAttainments);
				this.right.Set(this._charList[1].DisplayData, this._charList[1].CombatSkillQualifications, this._charList[1].CombatSkillAttainments, this._charList[1].LifeSkillQualifications, this._charList[1].LifeSkillAttainments);
			}
		}

		// Token: 0x04005950 RID: 22864
		[SerializeField]
		private CharacterMenuInfo left;

		// Token: 0x04005951 RID: 22865
		[SerializeField]
		private CharacterMenuInfo right;

		// Token: 0x04005952 RID: 22866
		[SerializeField]
		private CToggleGroup toggleGroup;

		// Token: 0x04005953 RID: 22867
		[SerializeField]
		private CButton confirm;

		// Token: 0x04005954 RID: 22868
		[SerializeField]
		private TooltipInvoker confirmDisplayer;

		// Token: 0x04005955 RID: 22869
		private List<SuccessorCandidates> _charList;
	}
}
