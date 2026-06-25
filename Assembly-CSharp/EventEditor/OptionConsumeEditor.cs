using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.TaiwuEvent.EventOption;
using UnityEngine;

namespace EventEditor
{
	// Token: 0x02000654 RID: 1620
	public class OptionConsumeEditor : EventEditorSubPageBase
	{
		// Token: 0x06004D2A RID: 19754 RVA: 0x00246FCC File Offset: 0x002451CC
		public static void Init(OptionConsumeEditor instance)
		{
			OptionConsumeEditor.Instance = instance;
			OptionConsumeEditor.Instance.InternalInit();
			OptionConsumeEditor.Instance.Hide();
		}

		// Token: 0x06004D2B RID: 19755 RVA: 0x00246FEC File Offset: 0x002451EC
		protected override void InternalInit()
		{
			this.OperateStack = new OperateStack(32);
			this._costEntries = new List<EventEditorConsumeInfo>();
			this._confirmBtn = this.btnConfirm;
			this._cancelBtn = this.btnCancel;
			this._costTemplate = this.goConsumptionTemp;
			this._contentRoot = this.contentRoot;
			this._createEntryBtn = this.btnCreateEntry;
			this._confirmBtn.ClearAndAddListener(new Action(this.OnConfirm));
			this._cancelBtn.ClearAndAddListener(new Action(this.OnCancel));
			this._createEntryBtn.ClearAndAddListener(new Action(this.CreateCostEntry));
		}

		// Token: 0x06004D2C RID: 19756 RVA: 0x00247098 File Offset: 0x00245298
		public void Show(List<EventOptionCost> costs, Action<List<EventOptionCost>> onConfirm)
		{
			this._isDirty = false;
			this._onConfirm = onConfirm;
			this._costs = (costs ?? new List<EventOptionCost>());
			this.ClearCostEntries();
			foreach (EventOptionCost cost in this._costs)
			{
				EventEditorConsumeInfo obj = this.LoadCostEntry(cost);
				this._costEntries.Add(obj);
			}
			this.OperateStack.Clear();
			base.gameObject.SetActive(true);
			this._confirmBtn.interactable = false;
		}

		// Token: 0x06004D2D RID: 19757 RVA: 0x0024714C File Offset: 0x0024534C
		public override void Show()
		{
			this.Show(null, null);
		}

		// Token: 0x06004D2E RID: 19758 RVA: 0x00247158 File Offset: 0x00245358
		public override void Hide()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06004D2F RID: 19759 RVA: 0x00247168 File Offset: 0x00245368
		public void SetDirty()
		{
			this._isDirty = true;
			this._confirmBtn.interactable = true;
		}

		// Token: 0x06004D30 RID: 19760 RVA: 0x0024717F File Offset: 0x0024537F
		public void OnConfirm()
		{
			this._isDirty = false;
			this._confirmBtn.interactable = false;
			Action<List<EventOptionCost>> onConfirm = this._onConfirm;
			if (onConfirm != null)
			{
				onConfirm(this._costs);
			}
		}

		// Token: 0x06004D31 RID: 19761 RVA: 0x002471B0 File Offset: 0x002453B0
		public void OnCancel()
		{
			bool isDirty = this._isDirty;
			if (isDirty)
			{
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.UI_InstructionEditor_Confirm_Edit),
					Content = LocalStringManager.Get(LanguageKey.UI_InstructionEditor_Confirm_Edit_Desc),
					Type = 3,
					Yes = new Action(this.OnConfirm),
					No = new Action(this.Hide),
					GroupYesText = LocalStringManager.Get(LanguageKey.LK_Yes),
					GroupNoText = LocalStringManager.Get(LanguageKey.LK_No)
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.Hide();
			}
		}

		// Token: 0x06004D32 RID: 19762 RVA: 0x00247274 File Offset: 0x00245474
		private void ClearCostEntries()
		{
			foreach (EventEditorConsumeInfo entry in this._costEntries)
			{
				Object.Destroy(entry.gameObject);
			}
			this._costEntries.Clear();
		}

		// Token: 0x06004D33 RID: 19763 RVA: 0x002472E0 File Offset: 0x002454E0
		private EventEditorConsumeInfo LoadCostEntry(EventOptionCost cost)
		{
			GameObject gameObj = Object.Instantiate<GameObject>(this._costTemplate, this._contentRoot);
			EventEditorConsumeInfo costEntry = gameObj.GetComponent<EventEditorConsumeInfo>();
			costEntry.LoadData(cost);
			return costEntry;
		}

		// Token: 0x06004D34 RID: 19764 RVA: 0x00247314 File Offset: 0x00245514
		public void CreateCostEntry()
		{
			EventOptionCost costData = new EventOptionCost
			{
				ConsumeType = 0,
				CostAmount = 0,
				AutoConsume = false,
				Expression = null
			};
			EventEditorConsumeInfo entry = this.LoadCostEntry(costData);
			this._costEntries.Add(entry);
			this._costs.Add(costData);
			this.SetDirty();
		}

		// Token: 0x06004D35 RID: 19765 RVA: 0x00247370 File Offset: 0x00245570
		public void RemoveIndex(int index)
		{
			bool flag = !this._costEntries.CheckIndex(index);
			if (!flag)
			{
				EventEditorConsumeInfo entry = this._costEntries[index];
				this._costs.RemoveAt(index);
				this._costEntries.RemoveAt(index);
				Object.Destroy(entry.gameObject);
				this.SetDirty();
			}
		}

		// Token: 0x0400357C RID: 13692
		public static OptionConsumeEditor Instance;

		// Token: 0x0400357D RID: 13693
		private List<EventOptionCost> _costs;

		// Token: 0x0400357E RID: 13694
		private List<EventEditorConsumeInfo> _costEntries;

		// Token: 0x0400357F RID: 13695
		private Action<List<EventOptionCost>> _onConfirm;

		// Token: 0x04003580 RID: 13696
		private bool _isDirty;

		// Token: 0x04003581 RID: 13697
		private RectTransform _contentRoot;

		// Token: 0x04003582 RID: 13698
		private GameObject _costTemplate;

		// Token: 0x04003583 RID: 13699
		private CButton _confirmBtn;

		// Token: 0x04003584 RID: 13700
		private CButton _cancelBtn;

		// Token: 0x04003585 RID: 13701
		private CButton _createEntryBtn;

		// Token: 0x04003586 RID: 13702
		[SerializeField]
		private CButton btnConfirm;

		// Token: 0x04003587 RID: 13703
		[SerializeField]
		private CButton btnCancel;

		// Token: 0x04003588 RID: 13704
		[SerializeField]
		private GameObject goConsumptionTemp;

		// Token: 0x04003589 RID: 13705
		[SerializeField]
		private RectTransform contentRoot;

		// Token: 0x0400358A RID: 13706
		[SerializeField]
		private CButton btnCreateEntry;
	}
}
