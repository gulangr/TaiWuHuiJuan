using System;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Story.SectMainStory;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.SectInteract.Emei
{
	// Token: 0x020009ED RID: 2541
	public class EmeiAddBonusItem : MonoBehaviour
	{
		// Token: 0x17000DAE RID: 3502
		// (get) Token: 0x06007CE9 RID: 31977 RVA: 0x003A0DB1 File Offset: 0x0039EFB1
		// (set) Token: 0x06007CEA RID: 31978 RVA: 0x003A0DB9 File Offset: 0x0039EFB9
		public bool Locked
		{
			get
			{
				return this._locked;
			}
			set
			{
				this._locked = value;
				this.Refresh();
			}
		}

		// Token: 0x17000DAF RID: 3503
		// (get) Token: 0x06007CEB RID: 31979 RVA: 0x003A0DCA File Offset: 0x0039EFCA
		// (set) Token: 0x06007CEC RID: 31980 RVA: 0x003A0DD2 File Offset: 0x0039EFD2
		public SectEmeiBreakBonusData Origin
		{
			get
			{
				return this._origin;
			}
			set
			{
				this._origin = value;
				this.Refresh();
			}
		}

		// Token: 0x17000DB0 RID: 3504
		// (get) Token: 0x06007CED RID: 31981 RVA: 0x003A0DE3 File Offset: 0x0039EFE3
		// (set) Token: 0x06007CEE RID: 31982 RVA: 0x003A0DEB File Offset: 0x0039EFEB
		public SectEmeiBreakBonusData Selected
		{
			get
			{
				return this._selected;
			}
			set
			{
				this._selected = value;
				this.Refresh();
			}
		}

		// Token: 0x06007CEF RID: 31983 RVA: 0x003A0DFC File Offset: 0x0039EFFC
		public void Init(int index, Action<int> onSelect, Action<int> onCancel, Action<int> onRemove)
		{
			this._index = index;
			this._onSelect = onSelect;
			this._onCancel = onCancel;
			this._onRemove = onRemove;
			PointerTrigger pointerTrigger = this.noContent.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(new UnityAction(this.TurnOnHover));
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.AddListener(new UnityAction(this.TurnOffHover));
			this.btnSelect.ClearAndAddListener(new Action(this.OnClickSelect));
			this.btnCancel.ClearAndAddListener(new Action(this.OnClickCancel));
			this.btnRemove.ClearAndAddListener(new Action(this.OnClickRemove));
			this.btnSelectWhenFilled.ClearAndAddListener(new Action(this.OnClickSelect));
			this.btnCancel.GetComponent<PointerTrigger>().EnterEvent.ResetListener(delegate()
			{
				this.bonus.SetHover(true);
			});
			this.btnCancel.GetComponent<PointerTrigger>().ExitEvent.ResetListener(delegate()
			{
				this.bonus.SetHover(false);
			});
		}

		// Token: 0x06007CF0 RID: 31984 RVA: 0x003A0F20 File Offset: 0x0039F120
		public void SetSelectedStatus(bool value)
		{
			this.selected.SetActive(value && !this.Locked);
			this.bonus.SetSelected(value && !this.Locked);
			this.btnSelectWhenFilled.gameObject.SetActive(!value);
		}

		// Token: 0x06007CF1 RID: 31985 RVA: 0x003A0F7C File Offset: 0x0039F17C
		private void Refresh()
		{
			this.SetLock(this._locked);
			bool flag = this._locked;
			if (!flag)
			{
				bool flag2 = this.Selected.TemplateId >= 0;
				if (flag2)
				{
					this.Set(this._selected);
				}
				else
				{
					bool flag3 = this.Origin.TemplateId >= 0;
					if (flag3)
					{
						this.Set(this._origin);
					}
					else
					{
						this.SetEmpty();
					}
				}
				this.btnRemove.gameObject.SetActive(this.Selected.TemplateId < 0 && this.Origin.TemplateId >= 0);
			}
		}

		// Token: 0x06007CF2 RID: 31986 RVA: 0x003A1028 File Offset: 0x0039F228
		private void Set(SectEmeiBreakBonusData data)
		{
			this.bonus.Set(data.TemplateId, data.BonusProgress, data.BonusCount, false);
			this.bonusImg.SetSprite(string.Format("{0}_{1}", "ui9_back_special_break_title", SkillBreakPlateGridBonusType.Instance[data.TemplateId].FilterGroup), false, null);
			this.bonus.gameObject.SetActive(true);
			this.noContent.SetActive(false);
			this.btnRemove.gameObject.SetActive(true);
		}

		// Token: 0x06007CF3 RID: 31987 RVA: 0x003A10C0 File Offset: 0x0039F2C0
		private void SetEmpty()
		{
			this.noContent.SetActive(true);
			this.icon.SetActive(!this._locked);
			this.bonus.gameObject.SetActive(false);
			this.btnRemove.gameObject.SetActive(false);
		}

		// Token: 0x06007CF4 RID: 31988 RVA: 0x003A1114 File Offset: 0x0039F314
		private void SetLock(bool value)
		{
			if (value)
			{
				this.locked.SetActive(true);
				this.icon.SetActive(false);
				this.btnSelect.interactable = false;
				this.btnCancel.gameObject.SetActive(false);
				this.noContent.SetActive(true);
				this.bonus.gameObject.SetActive(false);
				this.btnRemove.gameObject.SetActive(false);
				this.noContent.GetComponent<PointerTrigger>().enabled = false;
				this.noContent.GetComponent<TooltipInvoker>().PresetParam[0] = LanguageKey.LK_CombatSkill_SpecialBreak_CantNotAddReason_Locked.Tr();
			}
			else
			{
				this.locked.SetActive(false);
				this.btnCancel.gameObject.SetActive(true);
				this.noContent.GetComponent<PointerTrigger>().enabled = true;
				this.noContent.GetComponent<TooltipInvoker>().PresetParam[0] = LanguageKey.LK_CombatSkill_SpecialBreak_CantNotAddReason_Entry.Tr();
			}
		}

		// Token: 0x06007CF5 RID: 31989 RVA: 0x003A1215 File Offset: 0x0039F415
		private void OnClickSelect()
		{
			this._onSelect(this._index);
		}

		// Token: 0x06007CF6 RID: 31990 RVA: 0x003A122A File Offset: 0x0039F42A
		private void OnClickCancel()
		{
			this._onCancel(this._index);
		}

		// Token: 0x06007CF7 RID: 31991 RVA: 0x003A1240 File Offset: 0x0039F440
		private void OnClickRemove()
		{
			DialogCmd dialogCmd = new DialogCmd
			{
				Type = 1,
				Title = LocalStringManager.Get(LanguageKey.LK_CombatSkill_SpecialBreak_RemoveBreakBonus_Dialog_Title),
				Content = LocalStringManager.Get(LanguageKey.LK_CombatSkill_SpecialBreak_RemoveBreakBonus_Dialog_Content).ColorReplace(),
				Yes = new Action(this.OnConfirmRemove)
			};
			UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", dialogCmd));
			UIManager.Instance.MaskUI(UIElement.Dialog);
		}

		// Token: 0x06007CF8 RID: 31992 RVA: 0x003A12BD File Offset: 0x0039F4BD
		private void OnConfirmRemove()
		{
			this._onRemove(this._index);
		}

		// Token: 0x06007CF9 RID: 31993 RVA: 0x003A12D2 File Offset: 0x0039F4D2
		private void TurnOnHover()
		{
			this.hover.SetActive(true);
		}

		// Token: 0x06007CFA RID: 31994 RVA: 0x003A12E2 File Offset: 0x0039F4E2
		private void TurnOffHover()
		{
			this.hover.SetActive(false);
		}

		// Token: 0x04005EFA RID: 24314
		public GameObject noContent;

		// Token: 0x04005EFB RID: 24315
		public GameObject selected;

		// Token: 0x04005EFC RID: 24316
		public GameObject hover;

		// Token: 0x04005EFD RID: 24317
		public GameObject icon;

		// Token: 0x04005EFE RID: 24318
		public GameObject locked;

		// Token: 0x04005EFF RID: 24319
		public CImage bonusImg;

		// Token: 0x04005F00 RID: 24320
		public EmeiSpecialBreakBonus bonus;

		// Token: 0x04005F01 RID: 24321
		public CButton btnSelect;

		// Token: 0x04005F02 RID: 24322
		public CButton btnRemove;

		// Token: 0x04005F03 RID: 24323
		public CButton btnCancel;

		// Token: 0x04005F04 RID: 24324
		public CButton btnSelectWhenFilled;

		// Token: 0x04005F05 RID: 24325
		private int _index;

		// Token: 0x04005F06 RID: 24326
		private Action<int> _onSelect;

		// Token: 0x04005F07 RID: 24327
		private Action<int> _onCancel;

		// Token: 0x04005F08 RID: 24328
		private Action<int> _onRemove;

		// Token: 0x04005F09 RID: 24329
		private bool _locked;

		// Token: 0x04005F0A RID: 24330
		private SectEmeiBreakBonusData _origin = ViewEmeiCombatSkillSpecialBreak.InvalidBonus;

		// Token: 0x04005F0B RID: 24331
		private SectEmeiBreakBonusData _selected = ViewEmeiCombatSkillSpecialBreak.InvalidBonus;
	}
}
