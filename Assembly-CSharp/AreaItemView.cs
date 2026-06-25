using System;
using FrameWork;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

// Token: 0x02000223 RID: 547
public class AreaItemView : Refers
{
	// Token: 0x060022E3 RID: 8931 RVA: 0x001020C8 File Offset: 0x001002C8
	public void Refresh(TemplateKey itemKey, int amount, Action<TemplateKey> onClick = null, bool isSelect = false)
	{
		this._onClick = onClick;
		this.InitRefers();
		this._itemKey = itemKey;
		this._amount = amount;
		this.SetIcon();
		this.SetName();
		this.SetGrade();
		this.SetValue();
		this.SetWeight();
		this.SetCount();
		this.SetType();
		this.RefreshButton();
		this.RefreshPointerTrigger();
		this.RefreshSelected(isSelect);
		this.SetMouseTipDisplayer(base.GetComponent<TooltipInvoker>(), itemKey.ItemType, itemKey.TemplateId);
	}

	// Token: 0x060022E4 RID: 8932 RVA: 0x00102154 File Offset: 0x00100354
	public void RefreshArrangementIcon(bool isShow, string spriteName = null)
	{
		this._arrangementIcon.gameObject.SetActive(isShow);
		if (isShow)
		{
			this._arrangementIcon.SetSprite(spriteName, false, null);
		}
	}

	// Token: 0x060022E5 RID: 8933 RVA: 0x0010218C File Offset: 0x0010038C
	public void SetButtonInteractable(bool interactable)
	{
		CButtonObsolete button = base.GetComponent<CButtonObsolete>();
		PointerTrigger pointerTrigger = base.GetComponent<PointerTrigger>();
		button.interactable = interactable;
		pointerTrigger.enabled = interactable;
	}

	// Token: 0x060022E6 RID: 8934 RVA: 0x001021B8 File Offset: 0x001003B8
	private void RefreshPointerTrigger()
	{
		PointerTrigger pointerTrigger = base.GetComponent<PointerTrigger>();
		pointerTrigger.EnterEvent.AddListener(delegate()
		{
			this._enterMark.SetActive(true);
		});
		pointerTrigger.ExitEvent.AddListener(delegate()
		{
			this._enterMark.SetActive(false);
		});
	}

	// Token: 0x060022E7 RID: 8935 RVA: 0x001021FD File Offset: 0x001003FD
	private void RefreshSelected(bool isSelected)
	{
		this._checkMark.SetActive(isSelected);
	}

	// Token: 0x060022E8 RID: 8936 RVA: 0x00102210 File Offset: 0x00100410
	private void RefreshButton()
	{
		CButtonObsolete button = base.GetComponent<CButtonObsolete>();
		button.ClearAndAddListener(delegate
		{
			Action<TemplateKey> onClick = this._onClick;
			if (onClick != null)
			{
				onClick(this._itemKey);
			}
		});
	}

	// Token: 0x060022E9 RID: 8937 RVA: 0x00102238 File Offset: 0x00100438
	private void SetIcon()
	{
		bool flag = this._icon == null;
		if (!flag)
		{
			string iconSprite = ItemTemplateHelper.GetIcon(this._itemKey.ItemType, this._itemKey.TemplateId);
			this._icon.SetSprite(iconSprite, false, null);
		}
	}

	// Token: 0x060022EA RID: 8938 RVA: 0x00102284 File Offset: 0x00100484
	private void SetName()
	{
		bool flag = !this.CTryGet<TextMeshProUGUI>("Name", out this._name);
		if (!flag)
		{
			this._name.text = ItemTemplateHelper.GetName(this._itemKey.ItemType, this._itemKey.TemplateId);
		}
	}

	// Token: 0x060022EB RID: 8939 RVA: 0x001022D4 File Offset: 0x001004D4
	private void SetGrade()
	{
		sbyte grade = ItemTemplateHelper.GetGrade(this._itemKey.ItemType, this._itemKey.TemplateId);
		this._iconBack.SetSprite(ItemView.GetGradeBack(grade), false, null);
		bool flag = this._gradeBack != null;
		if (flag)
		{
			this._gradeBack.SetSprite(ItemView.GetGradeIcon(grade), false, null);
		}
		bool flag2 = this._grade != null;
		if (flag2)
		{
			this._grade.text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", grade));
		}
	}

	// Token: 0x060022EC RID: 8940 RVA: 0x0010236C File Offset: 0x0010056C
	private void SetValue()
	{
		bool flag = !this.CTryGet<TextMeshProUGUI>("Value", out this._value);
		if (!flag)
		{
			this._value.text = ItemTemplateHelper.GetBaseValue(this._itemKey.ItemType, this._itemKey.TemplateId).ToString();
		}
	}

	// Token: 0x060022ED RID: 8941 RVA: 0x001023C4 File Offset: 0x001005C4
	private void SetWeight()
	{
		bool flag = !this.CTryGet<TextMeshProUGUI>("Weight", out this._weight);
		if (!flag)
		{
			this._weight.text = ItemTemplateHelper.GetBaseWeight(this._itemKey.ItemType, this._itemKey.TemplateId).ToString();
		}
	}

	// Token: 0x060022EE RID: 8942 RVA: 0x0010241C File Offset: 0x0010061C
	private void SetType()
	{
		bool flag = !this.CTryGet<TextMeshProUGUI>("Type", out this._type);
		if (!flag)
		{
			this._type.text = LocalStringManager.Get(string.Format("LK_ItemType_{0}", this._itemKey.ItemType));
		}
	}

	// Token: 0x060022EF RID: 8943 RVA: 0x00102470 File Offset: 0x00100670
	private void SetCount()
	{
		bool flag = !this.CTryGet<TextMeshProUGUI>("Count", out this._count);
		if (!flag)
		{
			this._count.text = string.Format("x{0}", this._amount).SetColor("lightwhite");
		}
	}

	// Token: 0x060022F0 RID: 8944 RVA: 0x001024C4 File Offset: 0x001006C4
	private void SetMouseTipDisplayer(TooltipInvoker tipDisplayer, sbyte itemType, short templateId)
	{
		tipDisplayer.RuntimeParam = null;
		TemplateKey itemKey = new TemplateKey(itemType, templateId);
		ItemDisplayData Data = new ItemDisplayData(itemKey.ItemType, itemKey.TemplateId);
		tipDisplayer.Type = TooltipManager.ItemTypeToTipType[Data.Key.ItemType];
		tipDisplayer.NeedRefresh = (UIElement.Combat.Exist && Data.Key.ItemType == 0 && Data.UsingType == ItemDisplayData.ItemUsingType.Equiped);
		tipDisplayer.RuntimeParam = new ArgumentBox().SetObject("ItemData", Data.Clone(-1));
		tipDisplayer.RuntimeParam.Set("ShowPageInfo", Data.Key.ItemType == 10);
		tipDisplayer.RuntimeParam.Set("TemplateDataOnly", true);
		tipDisplayer.RuntimeParam.Set("CharId", Data.OwnerCharId);
	}

	// Token: 0x060022F1 RID: 8945 RVA: 0x001025A4 File Offset: 0x001007A4
	private void InitRefers()
	{
		this._icon = base.CGet<CImage>("Icon");
		this._enterMark = base.CGet<GameObject>("EnterMark");
		this._checkMark = base.CGet<GameObject>("CheckMark");
		this._gradeBack = base.CGet<CImage>("GradeBack");
		this._grade = base.CGet<TextMeshProUGUI>("Grade");
		this._selectStatus = base.CGet<GameObject>("SelectStatus");
		this._iconBack = base.CGet<CImage>("IconBack");
		this._arrangementIcon = base.CGet<CImage>("ArrangementIcon");
		this._count = base.CGet<TextMeshProUGUI>("Count");
	}

	// Token: 0x04001AC6 RID: 6854
	private TemplateKey _itemKey;

	// Token: 0x04001AC7 RID: 6855
	private Action<TemplateKey> _onClick;

	// Token: 0x04001AC8 RID: 6856
	private int _amount;

	// Token: 0x04001AC9 RID: 6857
	private CImage _icon;

	// Token: 0x04001ACA RID: 6858
	private GameObject _enterMark;

	// Token: 0x04001ACB RID: 6859
	private GameObject _checkMark;

	// Token: 0x04001ACC RID: 6860
	private TextMeshProUGUI _name;

	// Token: 0x04001ACD RID: 6861
	private TextMeshProUGUI _value;

	// Token: 0x04001ACE RID: 6862
	private TextMeshProUGUI _weight;

	// Token: 0x04001ACF RID: 6863
	private CImage _gradeBack;

	// Token: 0x04001AD0 RID: 6864
	private TextMeshProUGUI _grade;

	// Token: 0x04001AD1 RID: 6865
	private CImage _typeBack;

	// Token: 0x04001AD2 RID: 6866
	private TextMeshProUGUI _type;

	// Token: 0x04001AD3 RID: 6867
	private TextMeshProUGUI _count;

	// Token: 0x04001AD4 RID: 6868
	private GameObject _selectStatus;

	// Token: 0x04001AD5 RID: 6869
	private CImage _iconBack;

	// Token: 0x04001AD6 RID: 6870
	private CImage _arrangementIcon;
}
