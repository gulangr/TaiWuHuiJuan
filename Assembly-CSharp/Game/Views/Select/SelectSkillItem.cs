using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.CharacterMenu;
using GameData.Domains.CombatSkill;
using UnityEngine;

namespace Game.Views.Select
{
	// Token: 0x020007B3 RID: 1971
	public class SelectSkillItem : MonoBehaviour
	{
		// Token: 0x17000BB3 RID: 2995
		// (get) Token: 0x06006005 RID: 24581 RVA: 0x002C1303 File Offset: 0x002BF503
		private ViewSelectSkill Parent
		{
			get
			{
				return UIElement.SelectSkill.UiBaseAs<ViewSelectSkill>();
			}
		}

		// Token: 0x17000BB4 RID: 2996
		// (get) Token: 0x06006006 RID: 24582 RVA: 0x002C130F File Offset: 0x002BF50F
		// (set) Token: 0x06006007 RID: 24583 RVA: 0x002C1317 File Offset: 0x002BF517
		public bool IsHover
		{
			get
			{
				return this._isHover;
			}
			set
			{
				this._isHover = value;
				this.hover.gameObject.SetActive(this._isHover);
			}
		}

		// Token: 0x06006008 RID: 24584 RVA: 0x002C1338 File Offset: 0x002BF538
		private void Awake()
		{
			this.skillBtn.ClearAndAddListener(delegate
			{
				this.Parent.OnSkillItemClicked(this._index, this._combatSkillDisplayData);
			});
			this.pointerTrigger.EnterEvent.RemoveAllListeners();
			this.pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.IsHover = true;
				bool isTaiwu = this.Parent.IsTaiwu;
				if (isTaiwu)
				{
					this.btnSetting.gameObject.SetActive(false);
				}
			});
			this.pointerTrigger.ExitEvent.RemoveAllListeners();
			this.pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.IsHover = false;
				this.btnSetting.gameObject.SetActive(false);
			});
			this.btnSetting.ClearAndAddListener(new Action(this.OpenCombatSkillPanel));
		}

		// Token: 0x06006009 RID: 24585 RVA: 0x002C13D4 File Offset: 0x002BF5D4
		public void Set(int index, CombatSkillDisplayData combatSkillDisplayData, bool isSelected)
		{
			this._index = index;
			this._combatSkillDisplayData = combatSkillDisplayData;
			this.combatSkillItem.Set(this._combatSkillDisplayData);
			this.selected.gameObject.SetActive(isSelected);
			bool flag = this._combatSkillDisplayData == null;
			if (flag)
			{
			}
		}

		// Token: 0x0600600A RID: 24586 RVA: 0x002C1428 File Offset: 0x002BF628
		private void OpenCombatSkillPanel()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CombatSkillId", this._combatSkillDisplayData.TemplateId);
			UIElement.CombatSkillPanel.OnHide = new Action(this.Parent.RequestData);
			UIElement.CombatSkillPanel.SetOnInitArgs(argBox);
			UIManager.Instance.MaskUI(UIElement.CombatSkillPanel);
		}

		// Token: 0x0400428E RID: 17038
		[SerializeField]
		private CButton skillBtn;

		// Token: 0x0400428F RID: 17039
		[SerializeField]
		private PointerTrigger pointerTrigger;

		// Token: 0x04004290 RID: 17040
		[SerializeField]
		private CImage hover;

		// Token: 0x04004291 RID: 17041
		[SerializeField]
		private CImage selected;

		// Token: 0x04004292 RID: 17042
		[SerializeField]
		private CharacterMenuCombatSkillItem combatSkillItem;

		// Token: 0x04004293 RID: 17043
		[SerializeField]
		private CButton btnSetting;

		// Token: 0x04004294 RID: 17044
		private int _index;

		// Token: 0x04004295 RID: 17045
		private CombatSkillDisplayData _combatSkillDisplayData;

		// Token: 0x04004296 RID: 17046
		private bool _isHover;
	}
}
