using System;
using System.Collections.Generic;
using Config;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace EventEditor
{
	// Token: 0x02000637 RID: 1591
	public class ItemSimulate : MonoBehaviour
	{
		// Token: 0x06004B35 RID: 19253 RVA: 0x002356C0 File Offset: 0x002338C0
		public void Init()
		{
			this._allItems = new List<EventEditorItem>();
			this.itemScroll.OnItemRender += this.OnScrollItemRender;
			this.btnAddNewItem.ClearAndAddListener(new Action(this.OnAddNewItem));
			this.btnCopyInsert.ClearAndAddListener(new Action(this.OnCopyInsertString));
			this.itemIdInput.onEndEdit.AddListener(new UnityAction<string>(this.OnItemIdInputEndEdit));
			this.itemNameInput.onEndEdit.AddListener(new UnityAction<string>(this.OnItemNameInputEndEdit));
			this.itemKeyInput.onEndEdit.AddListener(new UnityAction<string>(this.OnItemKeyInputEndEdit));
			this.toggleUseIcon.onValueChanged.AddListener(new UnityAction<bool>(this.OnUseIconToggleValueChanged));
			this.itemStringDropdown.ClearOptions();
			this.itemStringDropdown.AddOptions(new List<string>
			{
				LocalStringManager.Get(LanguageKey.UI_EventEditor_ItemInsertString_0),
				LocalStringManager.Get(LanguageKey.UI_EventEditor_ItemInsertString_1),
				LocalStringManager.Get(LanguageKey.UI_EventEditor_ItemInsertString_2)
			});
			this.itemStringDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnItemStringDropdownValueChanged));
			this.itemTypeDropdown.ClearOptions();
			bool isDev = UI_EventEditor.IsDev;
			if (isDev)
			{
				this.itemTypeDropdown.AddOptions(new List<string>(this._typeIdToString));
			}
			else
			{
				this.itemTypeDropdown.AddOptions(new List<string>
				{
					LocalStringManager.Get(LanguageKey.LK_ItemType_0),
					LocalStringManager.Get(LanguageKey.LK_ItemType_1),
					LocalStringManager.Get(LanguageKey.LK_ItemType_2),
					LocalStringManager.Get(LanguageKey.LK_ItemType_3),
					LocalStringManager.Get(LanguageKey.LK_ItemType_4),
					LocalStringManager.Get(LanguageKey.LK_ItemType_5),
					LocalStringManager.Get(LanguageKey.LK_ItemType_6),
					LocalStringManager.Get(LanguageKey.LK_ItemType_7),
					LocalStringManager.Get(LanguageKey.LK_ItemType_8),
					LocalStringManager.Get(LanguageKey.LK_ItemType_9),
					LocalStringManager.Get(LanguageKey.LK_ItemType_10),
					LocalStringManager.Get(LanguageKey.LK_ItemType_11),
					LocalStringManager.Get(LanguageKey.LK_ItemType_12)
				});
			}
			this.itemTypeDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnItemTypeDropdownValueChanged));
			this._curSelectIndex = -1;
		}

		// Token: 0x06004B36 RID: 19254 RVA: 0x00235949 File Offset: 0x00233B49
		public void Refresh()
		{
			this.itemScroll.UpdateData(this._allItems.Count);
			this.UpdateInfoArea();
		}

		// Token: 0x06004B37 RID: 19255 RVA: 0x0023596C File Offset: 0x00233B6C
		public void SendPageDirty()
		{
			bool pageDirty = this._pageDirty;
			if (pageDirty)
			{
				EventEditorEventPreview.Instance.Refresh(null);
			}
			else
			{
				this._allItems.ForEach(delegate(EventEditorItem e)
				{
					e.SendDirty();
				});
			}
			this._pageDirty = false;
		}

		// Token: 0x06004B38 RID: 19256 RVA: 0x002359C4 File Offset: 0x00233BC4
		public EventEditorItem GetItem(string key)
		{
			return this._allItems.Find((EventEditorItem e) => e.Key == key);
		}

		// Token: 0x06004B39 RID: 19257 RVA: 0x002359FC File Offset: 0x00233BFC
		public EventEditorItem GetItem(string typeString, short id)
		{
			sbyte type;
			bool flag = !ItemType.TypeName2TypeId.TryGetValue(typeString, out type);
			EventEditorItem result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = this._allItems.Find((EventEditorItem e) => e.Type == type && e.TemplateId == id);
			}
			return result;
		}

		// Token: 0x06004B3A RID: 19258 RVA: 0x00235A50 File Offset: 0x00233C50
		private void UpdateInfoArea()
		{
			TextMeshProUGUI itemIdRange = this.txtMeshItemIdRange;
			TMP_InputField idInput = this.itemIdInput;
			TMP_InputField nameInput = this.itemNameInput;
			TMP_InputField keyInput = this.itemKeyInput;
			CImage itemBack = this.imgItemBack;
			CImage itemIcon = this.imgItemIcon;
			TextMeshProUGUI itemDesc = this.txtMeshItemDesc;
			bool flag = -1 == this._curSelectIndex || this._allItems.Count <= 0;
			if (flag)
			{
				this.itemTypeDropdown.value = 0;
				idInput.text = string.Empty;
				itemIdRange.text = string.Empty;
				nameInput.text = string.Empty;
				keyInput.text = string.Empty;
				itemBack.SetSprite(string.Empty, false, null);
				itemBack.color = Color.white;
				itemIcon.SetSprite(string.Empty, false, null);
				itemDesc.text = string.Empty;
				this.txtMeshLabelInsert.text = string.Empty;
			}
			else
			{
				EventEditorItem item = this._allItems[(int)this._curSelectIndex];
				for (int i = 0; i < this._dropdownValueToItemType.Length; i++)
				{
					bool flag2 = this._dropdownValueToItemType[i] == item.Type;
					if (flag2)
					{
						this.itemTypeDropdown.value = i;
						break;
					}
				}
				short[] idRange = this.GetTypeIdRange(item.Type);
				itemIdRange.text = string.Format("{0}~{1}", idRange[0], idRange[1]);
				idInput.text = item.TemplateId.ToString();
				nameInput.text = item.Name;
				keyInput.text = item.Key;
				itemBack.SetSprite(item.IconBack, false, null);
				itemBack.color = Colors.Instance.GradeColors[(int)(item.Grade - 1)];
				itemIcon.SetSprite(item.Icon, false, null);
				itemDesc.text = item.Desc;
				this.UpdateItemInsertString();
			}
		}

		// Token: 0x06004B3B RID: 19259 RVA: 0x00235C58 File Offset: 0x00233E58
		private short[] GetTypeIdRange(sbyte type)
		{
			List<short> keyList = null;
			switch (type)
			{
			case 0:
				keyList = Weapon.Instance.GetAllKeys();
				break;
			case 1:
				keyList = Armor.Instance.GetAllKeys();
				break;
			case 2:
				keyList = Accessory.Instance.GetAllKeys();
				break;
			case 3:
				keyList = Clothing.Instance.GetAllKeys();
				break;
			case 4:
				keyList = Carrier.Instance.GetAllKeys();
				break;
			case 5:
				keyList = Config.Material.Instance.GetAllKeys();
				break;
			case 6:
				keyList = CraftTool.Instance.GetAllKeys();
				break;
			case 7:
				keyList = Food.Instance.GetAllKeys();
				break;
			case 8:
				keyList = Medicine.Instance.GetAllKeys();
				break;
			case 9:
				keyList = TeaWine.Instance.GetAllKeys();
				break;
			case 10:
				keyList = SkillBook.Instance.GetAllKeys();
				break;
			case 11:
				keyList = Cricket.Instance.GetAllKeys();
				break;
			case 12:
				keyList = Misc.Instance.GetAllKeys();
				break;
			}
			short min = short.MaxValue;
			short max = short.MinValue;
			bool flag = keyList != null;
			if (flag)
			{
				for (int i = 0; i < keyList.Count; i++)
				{
					min = (short)Mathf.Min((int)min, (int)keyList[i]);
					max = (short)Mathf.Max((int)max, (int)keyList[i]);
				}
			}
			return new short[]
			{
				min,
				max
			};
		}

		// Token: 0x06004B3C RID: 19260 RVA: 0x00235DC8 File Offset: 0x00233FC8
		private void OnItemTypeDropdownValueChanged(int newValue)
		{
			bool flag = -1 == this._curSelectIndex || this._allItems.Count <= 0;
			if (!flag)
			{
				EventEditorItem item = this._allItems[(int)this._curSelectIndex];
				item.Type = this._dropdownValueToItemType[newValue];
				short[] idRange = this.GetTypeIdRange(item.Type);
				item.TemplateId = (short)Mathf.Clamp((int)item.TemplateId, (int)idRange[0], (int)idRange[1]);
				item.Dirty = true;
				this.UpdateInfoArea();
				this.itemScroll.RefreshCell((int)this._curSelectIndex);
			}
		}

		// Token: 0x06004B3D RID: 19261 RVA: 0x00235E60 File Offset: 0x00234060
		private void OnItemIdInputEndEdit(string newIdString)
		{
			bool flag = -1 == this._curSelectIndex || this._allItems.Count <= 0;
			if (!flag)
			{
				EventEditorItem item = this._allItems[(int)this._curSelectIndex];
				short newId;
				bool flag2 = !short.TryParse(newIdString, out newId);
				if (!flag2)
				{
					short[] idRange = this.GetTypeIdRange(item.Type);
					newId = (short)Mathf.Clamp((int)newId, (int)idRange[0], (int)idRange[1]);
					TMP_InputField idInput = this.itemIdInput;
					bool flag3 = newId.ToString() != idInput.text;
					if (flag3)
					{
						idInput.text = newId.ToString();
					}
					item.TemplateId = newId;
					item.Dirty = true;
					this.UpdateInfoArea();
					this.itemScroll.RefreshCell((int)this._curSelectIndex);
				}
			}
		}

		// Token: 0x06004B3E RID: 19262 RVA: 0x00235F2A File Offset: 0x0023412A
		private void OnItemNameInputEndEdit(string searchName)
		{
		}

		// Token: 0x06004B3F RID: 19263 RVA: 0x00235F30 File Offset: 0x00234130
		private void OnItemKeyInputEndEdit(string newKey)
		{
			bool flag = -1 == this._curSelectIndex || this._allItems.Count <= 0;
			if (!flag)
			{
				EventEditorItem item = this._allItems[(int)this._curSelectIndex];
				item.Key = newKey;
				for (int i = 0; i < this._allItems.Count; i++)
				{
					bool flag2 = this._allItems[i].Key == newKey && i != (int)this._curSelectIndex;
					if (flag2)
					{
						this._allItems[i].Key = string.Empty;
					}
				}
				this._pageDirty = true;
				this.itemScroll.RefreshCell((int)this._curSelectIndex);
			}
		}

		// Token: 0x06004B40 RID: 19264 RVA: 0x00235FF8 File Offset: 0x002341F8
		private void UpdateItemInsertString()
		{
			TextMeshProUGUI labelInsert = this.txtMeshLabelInsert;
			CToggle useIconToggle = this.toggleUseIcon;
			bool flag = -1 == this._curSelectIndex || this._allItems.Count <= 0 || (!useIconToggle.isOn && this.itemStringDropdown.value == 0);
			if (flag)
			{
				labelInsert.text = string.Empty;
			}
			else
			{
				string itemInsertName = "Item";
				string itemKeyInsert = string.Empty;
				string itemTypeInsert = string.Empty;
				string itemIdInsert = string.Empty;
				string itemStrInsert = string.Empty;
				string itemSpInsert = string.Empty;
				EventEditorItem item = this._allItems[(int)this._curSelectIndex];
				bool flag2 = !string.IsNullOrEmpty(item.Key);
				if (flag2)
				{
					itemKeyInsert = " key=" + item.Key;
				}
				else
				{
					itemTypeInsert = " type=" + this._typeIdToString[(int)item.Type];
					itemIdInsert = string.Format(" id={0}", item.TemplateId);
				}
				bool flag3 = this.itemStringDropdown.value == 1;
				if (flag3)
				{
					itemStrInsert = " str=Name";
				}
				else
				{
					bool flag4 = this.itemStringDropdown.value == 2;
					if (flag4)
					{
						itemStrInsert = " str=ColorName";
					}
				}
				bool isOn = useIconToggle.isOn;
				if (isOn)
				{
					itemSpInsert = " sp=Icon";
				}
				labelInsert.text = string.Concat(new string[]
				{
					"<",
					itemInsertName,
					itemKeyInsert,
					itemTypeInsert,
					itemIdInsert,
					itemStrInsert,
					itemSpInsert,
					"/>"
				});
			}
		}

		// Token: 0x06004B41 RID: 19265 RVA: 0x00236182 File Offset: 0x00234382
		private void OnItemStringDropdownValueChanged(int newIndex)
		{
			this.UpdateItemInsertString();
		}

		// Token: 0x06004B42 RID: 19266 RVA: 0x0023618C File Offset: 0x0023438C
		private void OnUseIconToggleValueChanged(bool useIcon)
		{
			this.UpdateItemInsertString();
		}

		// Token: 0x06004B43 RID: 19267 RVA: 0x00236198 File Offset: 0x00234398
		private void OnScrollItemRender(int dataIndex, GameObject itemGo)
		{
			EventEditorItem item = this._allItems[dataIndex];
			ItemSimulateItemPrefabInfo itemInfo = itemGo.GetComponent<ItemSimulateItemPrefabInfo>();
			itemInfo.image.enabled = (dataIndex % 2 == 0);
			itemInfo.txtMeshItemName.text = item.Name;
			itemInfo.txtMeshItemDesc.text = item.Desc;
			itemInfo.txtMeshItemKey.text = item.Key;
			itemInfo.imgItemBack.SetSprite(item.IconBack, false, null);
			itemInfo.imgItemBack.color = Colors.Instance.GradeColors[(int)(item.Grade - 1)];
			itemInfo.imgItemIcon.SetSprite(item.Icon, false, null);
			itemInfo.btn.ClearAndAddListener(delegate
			{
				this._curSelectIndex = (sbyte)dataIndex;
				this.UpdateInfoArea();
			});
			itemInfo.btnDeleteItem.ClearAndAddListener(delegate
			{
				this._allItems.RemoveAt(dataIndex);
				bool flag = !this._allItems.CheckIndex((int)this._curSelectIndex);
				if (flag)
				{
					this._curSelectIndex = 0;
				}
				this.Refresh();
			});
		}

		// Token: 0x06004B44 RID: 19268 RVA: 0x0023629C File Offset: 0x0023449C
		private void OnAddNewItem()
		{
			EventEditorItem item = new EventEditorItem();
			int randomTypeKey = GameApp.RandomRange(0, this.itemTypeDropdown.options.Count);
			item.Type = this._dropdownValueToItemType[randomTypeKey];
			short[] idRange = this.GetTypeIdRange(item.Type);
			item.TemplateId = (short)GameApp.RandomRange((int)idRange[0], (int)idRange[1]);
			this._allItems.Add(item);
			this._curSelectIndex = (sbyte)(this._allItems.Count - 1);
			this.itemScroll.initSuccess = false;
			this.Refresh();
		}

		// Token: 0x06004B45 RID: 19269 RVA: 0x00236328 File Offset: 0x00234528
		private void OnCopyInsertString()
		{
			CToggle useIconToggle = this.toggleUseIcon;
			bool flag = -1 == this._curSelectIndex || (!useIconToggle.isOn && this.itemStringDropdown.value == 0);
			if (!flag)
			{
				GUIUtility.systemCopyBuffer = this.txtMeshLabelInsert.text;
				string info = LocalStringManager.Get(LanguageKey.UI_EventEditor_Tip_CopyOK);
				TaskControlPanel.Instance.ShowTips(info, true);
			}
		}

		// Token: 0x0400343A RID: 13370
		[SerializeField]
		private InfinityScroll itemScroll;

		// Token: 0x0400343B RID: 13371
		[SerializeField]
		private CButton btnAddNewItem;

		// Token: 0x0400343C RID: 13372
		[SerializeField]
		private CDropdown itemTypeDropdown;

		// Token: 0x0400343D RID: 13373
		[SerializeField]
		private TMP_InputField itemIdInput;

		// Token: 0x0400343E RID: 13374
		[SerializeField]
		private TextMeshProUGUI txtMeshItemIdRange;

		// Token: 0x0400343F RID: 13375
		[SerializeField]
		private CImage imgItemBack;

		// Token: 0x04003440 RID: 13376
		[SerializeField]
		private CImage imgItemIcon;

		// Token: 0x04003441 RID: 13377
		[SerializeField]
		private TMP_InputField itemNameInput;

		// Token: 0x04003442 RID: 13378
		[SerializeField]
		private TMP_InputField itemKeyInput;

		// Token: 0x04003443 RID: 13379
		[SerializeField]
		private TextMeshProUGUI txtMeshItemDesc;

		// Token: 0x04003444 RID: 13380
		[SerializeField]
		private CDropdown itemStringDropdown;

		// Token: 0x04003445 RID: 13381
		[SerializeField]
		private CToggle toggleUseIcon;

		// Token: 0x04003446 RID: 13382
		[SerializeField]
		private TextMeshProUGUI txtMeshLabelInsert;

		// Token: 0x04003447 RID: 13383
		[SerializeField]
		private CButton btnCopyInsert;

		// Token: 0x04003448 RID: 13384
		private List<EventEditorItem> _allItems;

		// Token: 0x04003449 RID: 13385
		private sbyte _curSelectIndex;

		// Token: 0x0400344A RID: 13386
		private bool _pageDirty = false;

		// Token: 0x0400344B RID: 13387
		private readonly sbyte[] _dropdownValueToItemType = new sbyte[]
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11,
			12
		};

		// Token: 0x0400344C RID: 13388
		private readonly string[] _typeIdToString = new string[]
		{
			"Weapon",
			"Armor",
			"Accessory",
			"Clothing",
			"Carrier",
			"Material",
			"CraftTool",
			"Food",
			"Medicine",
			"TeaWine",
			"SkillBook",
			"Cricket",
			"Misc"
		};
	}
}
