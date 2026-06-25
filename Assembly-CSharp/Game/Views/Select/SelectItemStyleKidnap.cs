using System;
using System.Collections.Generic;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Item.Display;
using UnityEngine;

namespace Game.Views.Select
{
	// Token: 0x020007A9 RID: 1961
	[RequireComponent(typeof(CScrollRect))]
	public class SelectItemStyleKidnap : MonoBehaviour
	{
		// Token: 0x06005EDB RID: 24283 RVA: 0x002B8058 File Offset: 0x002B6258
		private void Awake()
		{
			bool flag = !this.PreCheck();
			if (!flag)
			{
				this._templates.Add(this.template);
				this._lines.Add(this.line);
			}
		}

		// Token: 0x06005EDC RID: 24284 RVA: 0x002B8099 File Offset: 0x002B6299
		public void Bind(Action<int> onClickItem)
		{
			this._onClickItem = onClickItem;
		}

		// Token: 0x06005EDD RID: 24285 RVA: 0x002B80A4 File Offset: 0x002B62A4
		public void Set(IEnumerable<ItemDisplayData> dataList, int selectedIndex)
		{
			bool flag = this.content == null || this.template == null || this.line == null;
			if (!flag)
			{
				bool flag2 = dataList == null;
				if (flag2)
				{
					this.SetToEmpty();
				}
				else
				{
					this.SetByCheckedData(dataList, selectedIndex);
				}
			}
		}

		// Token: 0x06005EDE RID: 24286 RVA: 0x002B8100 File Offset: 0x002B6300
		public void SetToEmpty()
		{
			foreach (SelectItemTemplateList t in this._templates)
			{
				bool activeSelf = t.gameObject.activeSelf;
				if (activeSelf)
				{
					t.gameObject.SetActive(false);
				}
			}
			foreach (GameObject i in this._lines)
			{
				bool activeSelf2 = i.gameObject.activeSelf;
				if (activeSelf2)
				{
					i.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06005EDF RID: 24287 RVA: 0x002B81CC File Offset: 0x002B63CC
		private bool PreCheck()
		{
			bool flag = this.content == null;
			if (flag)
			{
				this.content = base.GetComponent<CScrollRect>().Content;
				this.template = this.content.GetChild(0).GetComponent<SelectItemTemplateList>();
				this.line = this.content.GetChild(1).gameObject;
			}
			bool flag2 = this.content == null || this.template == null || this.line == null;
			bool result;
			if (flag2)
			{
				Debug.LogError(string.Format("SelectItemStyle precheck failed by {0} {1} {2}", this.content, this.template, this.line));
				result = false;
			}
			else
			{
				bool flag3 = this.line.transform.parent != this.content;
				if (flag3)
				{
					this.line.transform.SetParent(this.content);
				}
				bool activeSelf = this.line.gameObject.activeSelf;
				if (activeSelf)
				{
					this.line.gameObject.SetActive(false);
				}
				this.line.transform.SetAsFirstSibling();
				bool flag4 = this.template.transform.parent != this.content;
				if (flag4)
				{
					this.template.transform.SetParent(this.content);
				}
				bool activeSelf2 = this.template.gameObject.activeSelf;
				if (activeSelf2)
				{
					this.template.gameObject.SetActive(false);
				}
				this.template.transform.SetAsFirstSibling();
				this.AutoBind(this.template, 0);
				for (int i = this.content.childCount - 1; i >= 2; i--)
				{
					Object.Destroy(this.content.GetChild(i).gameObject);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06005EE0 RID: 24288 RVA: 0x002B83B4 File Offset: 0x002B65B4
		private void SetByCheckedData(IEnumerable<ItemDisplayData> dataList, int selectedIndex)
		{
			int templateIndex = 0;
			int lineIndex = 0;
			foreach (ItemDisplayData data in dataList)
			{
				this.RenderTemplate(ref templateIndex, data, templateIndex == selectedIndex);
				this.RenderLine(ref lineIndex);
			}
			lineIndex--;
			for (int i = 0; i < this._templates.Count; i++)
			{
				bool show = i < templateIndex;
				bool flag = show != this._templates[i].gameObject.activeSelf;
				if (flag)
				{
					this._templates[i].gameObject.SetActive(show);
				}
			}
			for (int j = 0; j < this._lines.Count; j++)
			{
				bool show2 = j < lineIndex;
				bool flag2 = show2 != this._lines[j].gameObject.activeSelf;
				if (flag2)
				{
					this._lines[j].gameObject.SetActive(show2);
				}
			}
		}

		// Token: 0x06005EE1 RID: 24289 RVA: 0x002B84E8 File Offset: 0x002B66E8
		private SelectItemTemplateList AutoBind(SelectItemTemplateList item, int index)
		{
			CButton button = item.GetComponent<CButton>();
			button.ClearAndAddListener(delegate
			{
				this.OnClick(index);
			});
			return item;
		}

		// Token: 0x06005EE2 RID: 24290 RVA: 0x002B8529 File Offset: 0x002B6729
		private void OnClick(int index)
		{
			Action<int> onClickItem = this._onClickItem;
			if (onClickItem != null)
			{
				onClickItem(index);
			}
		}

		// Token: 0x06005EE3 RID: 24291 RVA: 0x002B8540 File Offset: 0x002B6740
		private void RenderTemplate(ref int templateIndex, ItemDisplayData data, bool isSelected)
		{
			bool flag = templateIndex == this._templates.Count;
			if (flag)
			{
				this._templates.Add(this.AutoBind(Object.Instantiate<SelectItemTemplateList>(this.template, this.content), templateIndex));
			}
			SelectItemTemplateList item = this._templates[templateIndex];
			item.Set(data, isSelected);
			templateIndex++;
		}

		// Token: 0x06005EE4 RID: 24292 RVA: 0x002B85A4 File Offset: 0x002B67A4
		private void RenderLine(ref int lineIndex)
		{
			bool flag = lineIndex == this._lines.Count;
			if (flag)
			{
				this._lines.Add(Object.Instantiate<GameObject>(this.line, this.content));
			}
			lineIndex++;
		}

		// Token: 0x040041A5 RID: 16805
		[SerializeField]
		private RectTransform content;

		// Token: 0x040041A6 RID: 16806
		[SerializeField]
		private SelectItemTemplateList template;

		// Token: 0x040041A7 RID: 16807
		[SerializeField]
		private GameObject line;

		// Token: 0x040041A8 RID: 16808
		private readonly List<SelectItemTemplateList> _templates = new List<SelectItemTemplateList>();

		// Token: 0x040041A9 RID: 16809
		private readonly List<GameObject> _lines = new List<GameObject>();

		// Token: 0x040041AA RID: 16810
		private Action<int> _onClickItem;
	}
}
