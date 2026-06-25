using System;
using FrameWork.UISystem.UIElements;
using Game.Views.CharacterMenu;
using GameData.Domains.CombatSkill;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Components.Common
{
	// Token: 0x02000F8A RID: 3978
	public abstract class CombatSkillSelectable : MonoBehaviour
	{
		// Token: 0x0600B713 RID: 46867 RVA: 0x005370B8 File Offset: 0x005352B8
		public virtual void Set(CombatSkillDisplayDataForList data, Action<short> onSelect, bool init)
		{
			if (init)
			{
				this.pointerTrigger.EnterEvent.RemoveAllListeners();
				this.pointerTrigger.EnterEvent.AddListener(new UnityAction(this.TurnOnHover));
				this.pointerTrigger.ExitEvent.RemoveAllListeners();
				this.pointerTrigger.ExitEvent.AddListener(new UnityAction(this.TurnOffHover));
				this.btn.ClearAndAddListener(delegate
				{
					onSelect(data.TemplateId);
				});
			}
			this.item.Set(data.TemplateId, data.Power, data.BreakSuccess, data.ActivationState, data.BreakBonusGrades, data.CharId, data.LuohanId, data.Revoked);
		}

		// Token: 0x0600B714 RID: 46868 RVA: 0x005371B9 File Offset: 0x005353B9
		public void SetSelected(bool value)
		{
			this.selected.SetActive(value);
		}

		// Token: 0x0600B715 RID: 46869 RVA: 0x005371C9 File Offset: 0x005353C9
		private void TurnOnHover()
		{
			this.hover.SetActive(true);
		}

		// Token: 0x0600B716 RID: 46870 RVA: 0x005371D9 File Offset: 0x005353D9
		private void TurnOffHover()
		{
			this.hover.SetActive(false);
		}

		// Token: 0x04008E2B RID: 36395
		public GameObject selected;

		// Token: 0x04008E2C RID: 36396
		public GameObject hover;

		// Token: 0x04008E2D RID: 36397
		public CharacterMenuCombatSkillItem item;

		// Token: 0x04008E2E RID: 36398
		public CButton btn;

		// Token: 0x04008E2F RID: 36399
		public PointerTrigger pointerTrigger;
	}
}
