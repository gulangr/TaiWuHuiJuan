using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C12 RID: 3090
	public class ViewFeastMenu : UIBase
	{
		// Token: 0x06009D30 RID: 40240 RVA: 0x0049A7DE File Offset: 0x004989DE
		public override void OnInit(ArgumentBox argsBox)
		{
			argsBox.Get<BuildingManageSubPageEntertain>("Parent", out this._parent);
		}

		// Token: 0x06009D31 RID: 40241 RVA: 0x0049A7F4 File Offset: 0x004989F4
		private void Awake()
		{
			this.searchField.onValueChanged.RemoveAllListeners();
			this.searchField.onValueChanged.AddListener(new UnityAction<string>(this.OnSearch));
			this.toggleGroup.Init(-1);
			this.toggleGroup.OnActiveIndexChange += this.OnToggleChange;
		}

		// Token: 0x06009D32 RID: 40242 RVA: 0x0049A855 File Offset: 0x00498A55
		private void OnEnable()
		{
			this.searchField.SetTextWithoutNotify(string.Empty);
			this.OnSearch(string.Empty);
		}

		// Token: 0x06009D33 RID: 40243 RVA: 0x0049A878 File Offset: 0x00498A78
		protected override void OnClick(Transform btn)
		{
			string name = btn.name;
			string a = name;
			if (a == "Close")
			{
				this.QuickHide();
			}
		}

		// Token: 0x06009D34 RID: 40244 RVA: 0x0049A8A8 File Offset: 0x00498AA8
		private void OnSearch(string value)
		{
			bool flag = CommonUtils.FixToShowAbleString(ref value, this.searchField.textComponent.font);
			if (flag)
			{
				this.searchField.SetTextWithoutNotify(value);
			}
			this._filteredList.Clear();
			foreach (FeastItem feastItem in ((IEnumerable<FeastItem>)Feast.Instance))
			{
				bool flag2 = feastItem.TemplateId == 0;
				if (!flag2)
				{
					bool isUnlocked = this._parent.UnlockedFeastTypes.Contains(feastItem.TemplateId);
					bool searched = isUnlocked && (feastItem.Name.Contains(value) || feastItem.Desc.Contains(value) || feastItem.ConditionDesc.Contains(value) || feastItem.EffectDesc.Contains(value));
					bool flag3 = value.IsNullOrEmpty() || searched;
					if (flag3)
					{
						this._filteredList.Add(feastItem.TemplateId);
					}
				}
			}
			this._filteredList.Sort(new Comparison<short>(this.CompareFeast));
			this.Refresh();
		}

		// Token: 0x06009D35 RID: 40245 RVA: 0x0049A9D8 File Offset: 0x00498BD8
		private void OnToggleChange(int _, int __)
		{
			int index = this.toggleGroup.GetActiveIndex();
			bool flag = this._filteredList.CheckIndex(index);
			if (flag)
			{
				this._parent.DishDropdown.value = this._parent.DishTypeToDropdownValue[this._filteredList[index]];
				this.QuickHide();
			}
		}

		// Token: 0x06009D36 RID: 40246 RVA: 0x0049AA38 File Offset: 0x00498C38
		private void Refresh()
		{
			Transform content = this.toggleGroup.transform;
			for (int i = 0; i < this._filteredList.Count; i++)
			{
				bool flag = i >= content.childCount;
				if (flag)
				{
					this.toggleGroup.Add(Object.Instantiate<FeastMenuItem>(this.template, content).GetComponent<CToggle>());
				}
				Transform obj = content.GetChild(i);
				obj.GetComponent<FeastMenuItem>().Set(this._filteredList[i], this._parent.UnlockedFeastTypes.Contains(this._filteredList[i]));
				obj.gameObject.SetActive(true);
			}
			for (int j = this._filteredList.Count; j < content.childCount; j++)
			{
				content.GetChild(j).gameObject.SetActive(false);
			}
			this.toggleGroup.DeSelectWithoutNotify();
			short val = this._parent.DropdownValueToDishType.GetValueOrDefault(this._parent.DishDropdown.value, -1);
			for (int k = 0; k < this._filteredList.Count; k++)
			{
				bool flag2 = this._filteredList[k] == val;
				if (flag2)
				{
					this.toggleGroup.SetWithoutNotify(k);
					break;
				}
			}
		}

		// Token: 0x06009D37 RID: 40247 RVA: 0x0049AB9C File Offset: 0x00498D9C
		private int CompareFeast(short a, short b)
		{
			bool flag = this._parent.UnlockedFeastTypes.Contains(a);
			int result;
			if (flag)
			{
				result = (int)a;
			}
			else
			{
				bool flag2 = this._parent.UnlockedFeastTypes.Contains(b);
				if (flag2)
				{
					result = (int)b;
				}
				else
				{
					result = a.CompareTo(b);
				}
			}
			return result;
		}

		// Token: 0x040079DD RID: 31197
		[SerializeField]
		private TMP_InputField searchField;

		// Token: 0x040079DE RID: 31198
		[SerializeField]
		private CToggleGroup toggleGroup;

		// Token: 0x040079DF RID: 31199
		[SerializeField]
		private FeastMenuItem template;

		// Token: 0x040079E0 RID: 31200
		private readonly List<short> _filteredList = new List<short>();

		// Token: 0x040079E1 RID: 31201
		private BuildingManageSubPageEntertain _parent;
	}
}
