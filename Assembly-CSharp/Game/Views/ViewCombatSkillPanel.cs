using System;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.CombatSkill;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x020006F8 RID: 1784
	public class ViewCombatSkillPanel : UIBase
	{
		// Token: 0x17000A64 RID: 2660
		// (get) Token: 0x060054AC RID: 21676 RVA: 0x00273E21 File Offset: 0x00272021
		private int TaiwuId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x060054AD RID: 21677 RVA: 0x00273E30 File Offset: 0x00272030
		public override void OnInit(ArgumentBox argsBox)
		{
			short id;
			bool flag = argsBox != null && argsBox.Get("CombatSkillId", out id);
			if (flag)
			{
				this._skillTemplateId = id;
			}
			base.GetComponent<CanvasGroup>().alpha = 0f;
		}

		// Token: 0x060054AE RID: 21678 RVA: 0x00273E70 File Offset: 0x00272070
		private void Awake()
		{
			this.combatSkillPanel.Init(new Action(this.RequestData), null, base.GetComponent<RectTransform>(), null, delegate
			{
				base.GetComponent<CanvasGroup>().alpha = 1f;
			});
			this.btnTransfer.ClearAndAddListener(new Action(this.OpenPracticeMenu));
			this.btnClose.ClearAndAddListener(new Action(this.QuickHide));
		}

		// Token: 0x060054AF RID: 21679 RVA: 0x00273EDC File Offset: 0x002720DC
		private void OnEnable()
		{
			bool flag = this._skillTemplateId < 0;
			if (flag)
			{
				this.QuickHide();
			}
			this.RequestData();
		}

		// Token: 0x060054B0 RID: 21680 RVA: 0x00273F05 File Offset: 0x00272105
		private void RequestData()
		{
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForPractice(null, this.TaiwuId, delegate(int offset1, RawDataPool dataPool1)
			{
				CharacterDisplayDataForPractice data = null;
				Serializer.Deserialize(dataPool1, offset1, ref data);
				CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayDataForPractice(null, this.TaiwuId, this._skillTemplateId, delegate(int offset, RawDataPool pool)
				{
					CombatSkillPracticeDisplayData currentSkillData = null;
					Serializer.Deserialize(pool, offset, ref currentSkillData);
					this.combatSkillPanel.Set(currentSkillData, data, false);
					this.Element.ShowAfterRefresh();
				});
			});
		}

		// Token: 0x060054B1 RID: 21681 RVA: 0x00273F24 File Offset: 0x00272124
		private void OpenPracticeMenu()
		{
			this.QuickHide();
			ViewCharacterMenuPractice uiPractice = UIElement.CharacterMenuPractice.UiBaseAs<ViewCharacterMenuPractice>();
			bool flag = UIManager.Instance.IsElementActive(UIElement.CharacterMenu);
			if (flag)
			{
				ViewCharacterMenu ui = UIElement.CharacterMenu.UiBaseAs<ViewCharacterMenu>();
				bool flag2 = uiPractice.gameObject.activeSelf && ui.CurrentCharacterIsTaiwu;
				if (flag2)
				{
					uiPractice.SetSkill(this._skillTemplateId, true);
				}
				else
				{
					uiPractice.PresetSkill(this._skillTemplateId);
					ui.SelectCharacter(this.TaiwuId);
				}
				ui.SwitchToSubToggle(ECharacterSubToggleBase.PracticeBase);
			}
			else
			{
				uiPractice.PresetSkill(this._skillTemplateId);
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.Set("CharacterId", this.TaiwuId);
				argBox.Set("CanOperate", true);
				argBox.SetObject("ViewCharacterMenuTaretPage", new SubPageIndex(ECharacterSubToggleBase.PracticeBase, ECharacterSubPage.None));
				UIElement.CharacterMenu.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.CharacterMenu, true);
			}
		}

		// Token: 0x04003958 RID: 14680
		[SerializeField]
		private CombatSkillPanel combatSkillPanel;

		// Token: 0x04003959 RID: 14681
		[SerializeField]
		private CButton btnTransfer;

		// Token: 0x0400395A RID: 14682
		[SerializeField]
		private CButton btnClose;

		// Token: 0x0400395B RID: 14683
		private short _skillTemplateId;
	}
}
