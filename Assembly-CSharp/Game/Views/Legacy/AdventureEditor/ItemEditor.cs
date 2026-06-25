using System;
using System.Collections.Generic;
using System.Linq;
using AdventureEditor.Beta;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Adventure;
using GameData.Utilities;
using Google.Protobuf.Collections;
using TMPro;
using UnityEngine;

namespace Game.Views.Legacy.AdventureEditor
{
	// Token: 0x020009FF RID: 2559
	public class ItemEditor : MonoBehaviour
	{
		// Token: 0x06007DC2 RID: 32194 RVA: 0x003A65ED File Offset: 0x003A47ED
		private void Awake()
		{
			this.removeBtn.onClick.ResetListener(delegate()
			{
				this._remove(this.index);
				this._refresh();
			});
			this.editBtn.onClick.ResetListener(new Action(this.EditItem));
		}

		// Token: 0x06007DC3 RID: 32195 RVA: 0x003A662A File Offset: 0x003A482A
		public void Set(int idx, Action<int, List<EditingAdventureData.ItemCostItem>> setData, Func<int, AdventureCostItem> getData, Action<int> remove, Action refresh)
		{
			this.index = idx;
			this._setData = setData;
			this._getData = getData;
			this._remove = remove;
			this._refresh = refresh;
			this.RefreshTemplateIds();
		}

		// Token: 0x06007DC4 RID: 32196 RVA: 0x003A665C File Offset: 0x003A485C
		public void EditItem()
		{
			ArgumentBox argumentBox = EasyPool.Get<ArgumentBox>().Set("MultipleChoice", true).Set("SelectType", UI_ItemTemplateSelector.ESelectType.ItemTemplate);
			string key = "InitialSelection";
			AdventureCostItem adventureCostItem = this._getData(this.index);
			object obj;
			if (adventureCostItem == null)
			{
				obj = null;
			}
			else
			{
				RepeatedField<AdventureItemReference> availableItems = adventureCostItem.AvailableItems;
				if (availableItems == null)
				{
					obj = null;
				}
				else
				{
					obj = (from data in availableItems
					select new EditingAdventureData.ItemCostItem
					{
						Item1 = (sbyte)data.Type,
						Item2 = (short)data.TemplateId
					}).ToArray<EditingAdventureData.ItemCostItem>();
				}
			}
			ArgumentBox argBox = argumentBox.SetObject(key, obj ?? Array.Empty<EditingAdventureData.ItemCostItem>()).SetObject("OnConfirm", new UI_ItemTemplateSelector.OnConfirmHandler(delegate(List<EditingAdventureData.ItemCostItem> list)
			{
				this._setData(this.index, list);
				this._refresh();
				this.RefreshTemplateIds();
			}));
			UIElement.ItemTemplateSelector.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.ItemTemplateSelector, true);
		}

		// Token: 0x06007DC5 RID: 32197 RVA: 0x003A6720 File Offset: 0x003A4920
		public void RefreshTemplateIds()
		{
			TMP_Text tmp_Text = this.items;
			string separator = "|";
			AdventureCostItem adventureCostItem = this._getData(this.index);
			IEnumerable<string> enumerable;
			if (adventureCostItem == null)
			{
				enumerable = null;
			}
			else
			{
				enumerable = adventureCostItem.AvailableItems.Select(delegate(AdventureItemReference x)
				{
					IItemConfig item = ItemConfigHelper.GetConfig((sbyte)x.Type, (short)x.TemplateId);
					return string.Format("<color=#GradeColor_{0}>{1}</color>", item.Grade, item.Name);
				});
			}
			tmp_Text.text = string.Join(separator, enumerable ?? Array.Empty<string>()).ColorReplace();
		}

		// Token: 0x04005FCD RID: 24525
		private Action<int, List<EditingAdventureData.ItemCostItem>> _setData;

		// Token: 0x04005FCE RID: 24526
		private Func<int, AdventureCostItem> _getData;

		// Token: 0x04005FCF RID: 24527
		private Action<int> _remove;

		// Token: 0x04005FD0 RID: 24528
		private Action _refresh;

		// Token: 0x04005FD1 RID: 24529
		[SerializeField]
		private int index;

		// Token: 0x04005FD2 RID: 24530
		[SerializeField]
		private TMP_Text items;

		// Token: 0x04005FD3 RID: 24531
		[SerializeField]
		private CButton removeBtn;

		// Token: 0x04005FD4 RID: 24532
		[SerializeField]
		private CButton editBtn;
	}
}
