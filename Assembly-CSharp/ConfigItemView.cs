using System;
using FrameWork;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using TMPro;
using UnityEngine;

// Token: 0x02000228 RID: 552
public class ConfigItemView : Refers
{
	// Token: 0x0600231A RID: 8986 RVA: 0x00102E90 File Offset: 0x00101090
	public void Refresh(TemplateKey itemKey, Action<TemplateKey> onClick = null, bool isSelect = false, bool showMouseTip = false)
	{
		this._onClick = onClick;
		this.InitRefers();
		this._itemKey = itemKey;
		this.SetIcon();
		this.SetName();
		this.SetGrade();
		this.SetValue();
		this.SetWeight();
		this.SetType();
		this.RefreshButton();
		this.RefreshPointerTrigger();
		this.RefreshSelected(isSelect);
		this.SetMouseTipDisplayer(itemKey, showMouseTip);
	}

	// Token: 0x0600231B RID: 8987 RVA: 0x00102F00 File Offset: 0x00101100
	public void RefreshArrangementIcon(bool isShow, string spriteName = null)
	{
		this._arrangementIcon.gameObject.SetActive(isShow);
		if (isShow)
		{
			this._arrangementIcon.SetSprite(spriteName, false, null);
		}
	}

	// Token: 0x0600231C RID: 8988 RVA: 0x00102F38 File Offset: 0x00101138
	public void SetButtonInteractable(bool interactable)
	{
		CButtonObsolete button = base.GetComponent<CButtonObsolete>();
		PointerTrigger pointerTrigger = base.GetComponent<PointerTrigger>();
		button.interactable = interactable;
		pointerTrigger.enabled = interactable;
	}

	// Token: 0x0600231D RID: 8989 RVA: 0x00102F64 File Offset: 0x00101164
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

	// Token: 0x0600231E RID: 8990 RVA: 0x00102FA9 File Offset: 0x001011A9
	private void RefreshSelected(bool isSelected)
	{
		this._checkMark.SetActive(isSelected);
	}

	// Token: 0x0600231F RID: 8991 RVA: 0x00102FBC File Offset: 0x001011BC
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

	// Token: 0x06002320 RID: 8992 RVA: 0x00102FE4 File Offset: 0x001011E4
	private void SetIcon()
	{
		bool flag = this._icon == null;
		if (!flag)
		{
			string iconSprite = ItemTemplateHelper.GetIcon(this._itemKey.ItemType, this._itemKey.TemplateId);
			this._icon.SetSprite(iconSprite, false, null);
		}
	}

	// Token: 0x06002321 RID: 8993 RVA: 0x00103030 File Offset: 0x00101230
	private void SetName()
	{
		bool flag = !this.CTryGet<TextMeshProUGUI>("Name", out this._name);
		if (!flag)
		{
			sbyte grade = ItemTemplateHelper.GetGrade(this._itemKey.ItemType, this._itemKey.TemplateId);
			this._name.text = ItemTemplateHelper.GetName(this._itemKey.ItemType, this._itemKey.TemplateId).SetColor(Colors.Instance.GradeColors[(int)grade]);
		}
	}

	// Token: 0x06002322 RID: 8994 RVA: 0x001030B0 File Offset: 0x001012B0
	private void SetGrade()
	{
		sbyte grade = ItemTemplateHelper.GetGrade(this._itemKey.ItemType, this._itemKey.TemplateId);
		this._iconBack.SetSprite(ItemView.GetGradeBack(grade), false, null);
		this.ApplyGradeBack(grade);
		this.ApplyGradeLabel(grade);
	}

	// Token: 0x06002323 RID: 8995 RVA: 0x00103100 File Offset: 0x00101300
	protected virtual void ApplyGradeLabel(sbyte grade)
	{
		bool flag = this._grade != null;
		if (flag)
		{
			this._grade.gameObject.SetActive(true);
			this._grade.text = LocalStringManager.Get(string.Format("LK_ShortGrade_{0}", grade));
		}
	}

	// Token: 0x06002324 RID: 8996 RVA: 0x00103154 File Offset: 0x00101354
	protected virtual void ApplyGradeBack(sbyte grade)
	{
		bool flag = this._gradeBack != null;
		if (flag)
		{
			this._gradeBack.SetSprite(ItemView.GetGradeIcon(grade), false, null);
		}
	}

	// Token: 0x06002325 RID: 8997 RVA: 0x00103188 File Offset: 0x00101388
	private void SetValue()
	{
		bool flag = !this.CTryGet<TextMeshProUGUI>("Value", out this._value);
		if (!flag)
		{
			this._value.text = ItemTemplateHelper.GetBaseValue(this._itemKey.ItemType, this._itemKey.TemplateId).ToString();
		}
	}

	// Token: 0x06002326 RID: 8998 RVA: 0x001031E0 File Offset: 0x001013E0
	private void SetWeight()
	{
		bool flag = !this.CTryGet<TextMeshProUGUI>("Weight", out this._weight);
		if (!flag)
		{
			this._weight.text = ItemTemplateHelper.GetBaseWeight(this._itemKey.ItemType, this._itemKey.TemplateId).ToString();
		}
	}

	// Token: 0x06002327 RID: 8999 RVA: 0x00103238 File Offset: 0x00101438
	private void SetType()
	{
		bool flag = !this.CTryGet<TextMeshProUGUI>("Type", out this._type);
		if (!flag)
		{
			this._type.text = LocalStringManager.Get(string.Format("LK_ItemType_{0}", this._itemKey.ItemType));
		}
	}

	// Token: 0x06002328 RID: 9000 RVA: 0x0010328C File Offset: 0x0010148C
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
	}

	// Token: 0x06002329 RID: 9001 RVA: 0x00103324 File Offset: 0x00101524
	private void SetMouseTipDisplayer(TemplateKey itemKey, bool showMouseTip)
	{
		TooltipInvoker tipDisplayer = base.GetComponent<TooltipInvoker>();
		bool flag = tipDisplayer != null;
		if (flag)
		{
			tipDisplayer.enabled = showMouseTip;
			if (showMouseTip)
			{
				this.SetMouseTipDisplayer(new ItemDisplayData(itemKey.ItemType, itemKey.TemplateId));
			}
		}
	}

	// Token: 0x0600232A RID: 9002 RVA: 0x00103370 File Offset: 0x00101570
	private void SetMouseTipDisplayer(ItemDisplayData itemDisplayData)
	{
		TooltipInvoker tipDisplayer = base.GetComponent<TooltipInvoker>();
		tipDisplayer.RuntimeParam = null;
		tipDisplayer.Type = TooltipManager.ItemTypeToTipType[itemDisplayData.Key.ItemType];
		tipDisplayer.NeedRefresh = (UIElement.Combat.Exist && itemDisplayData.Key.ItemType == 0 && itemDisplayData.UsingType == ItemDisplayData.ItemUsingType.Equiped);
		tipDisplayer.RuntimeParam = new ArgumentBox().SetObject("ItemData", itemDisplayData.Clone(-1));
		tipDisplayer.RuntimeParam.Set("ShowPageInfo", itemDisplayData.Key.ItemType == 10);
		tipDisplayer.RuntimeParam.Set("TemplateDataOnly", true);
		tipDisplayer.RuntimeParam.Set("CharId", itemDisplayData.OwnerCharId);
	}

	// Token: 0x04001AED RID: 6893
	private TemplateKey _itemKey;

	// Token: 0x04001AEE RID: 6894
	private Action<TemplateKey> _onClick;

	// Token: 0x04001AEF RID: 6895
	private CImage _icon;

	// Token: 0x04001AF0 RID: 6896
	private GameObject _enterMark;

	// Token: 0x04001AF1 RID: 6897
	private GameObject _checkMark;

	// Token: 0x04001AF2 RID: 6898
	private TextMeshProUGUI _name;

	// Token: 0x04001AF3 RID: 6899
	private TextMeshProUGUI _value;

	// Token: 0x04001AF4 RID: 6900
	private TextMeshProUGUI _weight;

	// Token: 0x04001AF5 RID: 6901
	protected CImage _gradeBack;

	// Token: 0x04001AF6 RID: 6902
	protected TextMeshProUGUI _grade;

	// Token: 0x04001AF7 RID: 6903
	private CImage _typeBack;

	// Token: 0x04001AF8 RID: 6904
	private TextMeshProUGUI _type;

	// Token: 0x04001AF9 RID: 6905
	private GameObject _selectStatus;

	// Token: 0x04001AFA RID: 6906
	private CImage _iconBack;

	// Token: 0x04001AFB RID: 6907
	private CImage _arrangementIcon;
}
