using System;
using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000335 RID: 821
public class CommonTableRowForCharacter : Refers, ILanguage
{
	// Token: 0x1700053A RID: 1338
	// (get) Token: 0x06002F91 RID: 12177 RVA: 0x00174C40 File Offset: 0x00172E40
	private CommonCharacterNameFrame characterName
	{
		get
		{
			return base.CGet<CommonCharacterNameFrame>("NameLabel");
		}
	}

	// Token: 0x06002F92 RID: 12178 RVA: 0x00174C50 File Offset: 0x00172E50
	[Button("修改语言", LabelColor = "#0080FF")]
	public void OnLanguageChange(LocalStringManager.LanguageType lang)
	{
		this.language = lang;
		foreach (TMP_Text text in this.fixedTexts)
		{
			text.overflowMode = ((lang == LocalStringManager.LanguageType.CN) ? TextOverflowModes.Ellipsis : TextOverflowModes.Overflow);
			text.fontSize = (float)((lang == LocalStringManager.LanguageType.CN) ? 24 : 22);
		}
	}

	// Token: 0x1700053B RID: 1339
	// (get) Token: 0x06002F93 RID: 12179 RVA: 0x00174CC8 File Offset: 0x00172EC8
	// (set) Token: 0x06002F94 RID: 12180 RVA: 0x00174CD0 File Offset: 0x00172ED0
	public bool Locked
	{
		get
		{
			return this.locked;
		}
		set
		{
			this.locked = value;
			this.button.interactable = !this.locked;
			this.SetHover(false);
			foreach (TMP_Text item in this.fixedTexts)
			{
				item.color = (this.locked ? this.grey : this.pinkYellow);
			}
			this.ReCalculateWidth();
		}
	}

	// Token: 0x06002F95 RID: 12181 RVA: 0x00174D68 File Offset: 0x00172F68
	public void SetTextWithRefreshHeight(TMP_Text tmpText, string text)
	{
		int oldCount = tmpText.textInfo.lineCount;
		tmpText.text = text;
		int newCount = tmpText.textInfo.lineCount;
	}

	// Token: 0x06002F96 RID: 12182 RVA: 0x00174D98 File Offset: 0x00172F98
	public void ReCalculateWidth()
	{
		float width = this.descField.rect.width;
		bool flag = this.characterName == null;
		if (!flag)
		{
			this.characterName.GetComponent<RectTransform>().SetWidth(width);
		}
	}

	// Token: 0x06002F97 RID: 12183 RVA: 0x00174DE0 File Offset: 0x00172FE0
	public void SetWidth(IEnumerable<float> width, int status = -1)
	{
		float totalWidth = 0f;
		Func<float, int, CommonTableCellForCharacter> <>9__0;
		Func<float, int, CommonTableCellForCharacter> selector;
		if ((selector = <>9__0) == null)
		{
			selector = (<>9__0 = delegate(float x, int i)
			{
				Transform child = this.GetTableRowChild(i);
				bool flag2 = x != 0f;
				if (flag2)
				{
					if (child != null)
					{
						RectTransform component = child.GetComponent<RectTransform>();
						if (component != null)
						{
							component.SetWidth(x + this.padding);
						}
					}
				}
				if (child != null)
				{
					child.gameObject.SetActive(x > 0f);
				}
				bool flag3 = x > 0f && child != null;
				if (flag3)
				{
					totalWidth += x;
				}
				return (child != null) ? child.GetComponent<CommonTableCellForCharacter>() : null;
			});
		}
		foreach (CommonTableCellForCharacter item in width.Select(selector))
		{
			bool flag = status != -1;
			if (flag)
			{
				if (item != null)
				{
					item.SetCurrentStatus(status);
				}
			}
		}
		base.RectTransform.SetWidth(totalWidth);
	}

	// Token: 0x06002F98 RID: 12184 RVA: 0x00174E90 File Offset: 0x00173090
	public void SetEnterEvent(UnityAction onEnter)
	{
		this._onMouseEnterCallback = onEnter;
	}

	// Token: 0x06002F99 RID: 12185 RVA: 0x00174E9A File Offset: 0x0017309A
	public void SetExitEvent(UnityAction onExit)
	{
		this._onMouseExitCallback = onExit;
	}

	// Token: 0x06002F9A RID: 12186 RVA: 0x00174EA4 File Offset: 0x001730A4
	public void OnMouseEnter()
	{
		this.OnMouseHover();
	}

	// Token: 0x06002F9B RID: 12187 RVA: 0x00174EAD File Offset: 0x001730AD
	public void OnMouseHover()
	{
		this.SetHover(true);
	}

	// Token: 0x06002F9C RID: 12188 RVA: 0x00174EB7 File Offset: 0x001730B7
	public void OnMouseExit()
	{
		this.SetHover(false);
	}

	// Token: 0x1700053C RID: 1340
	// (get) Token: 0x06002F9D RID: 12189 RVA: 0x00174EC1 File Offset: 0x001730C1
	private bool _selected
	{
		get
		{
			return base.GetComponent<CToggleObsolete>().isOn;
		}
	}

	// Token: 0x06002F9E RID: 12190 RVA: 0x00174ECE File Offset: 0x001730CE
	public void SetHover(bool hover)
	{
		this._hovering = hover;
		this.RefreshStatus();
	}

	// Token: 0x06002F9F RID: 12191 RVA: 0x00174EE0 File Offset: 0x001730E0
	public void RefreshStatus()
	{
		int status = (this._hovering || this._selected) ? 1 : 0;
		bool flag = this.locked;
		if (flag)
		{
			status += 2;
		}
		bool flag2 = this._cells == null;
		if (!flag2)
		{
			foreach (CommonTableCellForCharacter item in this._cells)
			{
				item.SetCurrentStatus(status);
			}
		}
	}

	// Token: 0x06002FA0 RID: 12192 RVA: 0x00174F4C File Offset: 0x0017314C
	public Transform GetTableRowChild(int contentIndex)
	{
		bool flag = contentIndex < 0 || contentIndex >= this.container.childCount;
		Transform result;
		if (flag)
		{
			result = null;
		}
		else
		{
			result = this.container.GetChild(contentIndex);
		}
		return result;
	}

	// Token: 0x06002FA1 RID: 12193 RVA: 0x00174F8C File Offset: 0x0017318C
	public void OnHoverObject(bool isHover)
	{
		this.SetHover(isHover);
		if (isHover)
		{
			UnityAction onMouseEnterCallback = this._onMouseEnterCallback;
			if (onMouseEnterCallback != null)
			{
				onMouseEnterCallback();
			}
		}
		else
		{
			UnityAction onMouseExitCallback = this._onMouseExitCallback;
			if (onMouseExitCallback != null)
			{
				onMouseExitCallback();
			}
		}
	}

	// Token: 0x06002FA2 RID: 12194 RVA: 0x00174FCD File Offset: 0x001731CD
	public void SetName(string text)
	{
		this.characterName.SetName(text);
	}

	// Token: 0x06002FA3 RID: 12195 RVA: 0x00174FDC File Offset: 0x001731DC
	public void SetSpecialBg(int index, bool active)
	{
		Transform child = this.container.GetChild(index);
		CommonTableCellForCharacter comp = child.GetComponent<CommonTableCellForCharacter>();
		comp.SetSpecial(active);
	}

	// Token: 0x0400228B RID: 8843
	[SerializeField]
	private List<TMP_Text> fixedTexts = new List<TMP_Text>();

	// Token: 0x0400228C RID: 8844
	[SerializeField]
	private List<Refers> generateRefers = new List<Refers>();

	// Token: 0x0400228D RID: 8845
	[SerializeField]
	private CommonTableCellForCharacter[] _cells;

	// Token: 0x0400228E RID: 8846
	[Header("排版元素间隔，需与Horizontal Layout同步")]
	[SerializeField]
	private float padding = 2f;

	// Token: 0x0400228F RID: 8847
	[Header("0行文本对应的表格高")]
	[SerializeField]
	private float zeroLineHeight = 48f;

	// Token: 0x04002290 RID: 8848
	[Header("每行文本对应的表格高")]
	[SerializeField]
	private float textLineHeight = 22f;

	// Token: 0x04002291 RID: 8849
	[Header("中文文本对应的表格高")]
	[SerializeField]
	private float cnRowHeight = 70f;

	// Token: 0x04002292 RID: 8850
	[Header("表头：CommonTableHeadForItem (Prefab)")]
	[SerializeField]
	private CommonTableHead tableHead;

	// Token: 0x04002293 RID: 8851
	[Header("普通格子模板")]
	[SerializeField]
	private CommonTableCellForCharacter cellTemplateNormal;

	// Token: 0x04002294 RID: 8852
	[SerializeField]
	private bool locked;

	// Token: 0x04002295 RID: 8853
	[SerializeField]
	private CToggleObsolete button;

	// Token: 0x04002296 RID: 8854
	[SerializeField]
	[ReadOnly]
	private LocalStringManager.LanguageType language;

	// Token: 0x04002297 RID: 8855
	[SerializeField]
	private RectTransform container;

	// Token: 0x04002298 RID: 8856
	[SerializeField]
	private RectTransform descField;

	// Token: 0x04002299 RID: 8857
	[SerializeField]
	private Color pinkYellow;

	// Token: 0x0400229A RID: 8858
	[SerializeField]
	private Color grey;

	// Token: 0x0400229B RID: 8859
	private List<TMP_Text> _texts = new List<TMP_Text>();

	// Token: 0x0400229C RID: 8860
	private UnityAction _onMouseEnterCallback;

	// Token: 0x0400229D RID: 8861
	private UnityAction _onMouseExitCallback;

	// Token: 0x0400229E RID: 8862
	private bool _hovering;
}
