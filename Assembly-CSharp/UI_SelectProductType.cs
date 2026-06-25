using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using GameData.Domains.Building;
using GameData.Domains.Extra;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020001BE RID: 446
public class UI_SelectProductType : UIBase
{
	// Token: 0x170002D4 RID: 724
	// (get) Token: 0x06001BB1 RID: 7089 RVA: 0x000BEFA4 File Offset: 0x000BD1A4
	private InfinityScrollLegacy ScrollView
	{
		get
		{
			return base.CGet<InfinityScrollLegacy>("ScrollView");
		}
	}

	// Token: 0x170002D5 RID: 725
	// (get) Token: 0x06001BB2 RID: 7090 RVA: 0x000BEFB1 File Offset: 0x000BD1B1
	private CButtonObsolete ButtonConfirm
	{
		get
		{
			return base.CGet<CButtonObsolete>("Confirm");
		}
	}

	// Token: 0x170002D6 RID: 726
	// (get) Token: 0x06001BB3 RID: 7091 RVA: 0x000BEFBE File Offset: 0x000BD1BE
	private CButtonObsolete ButtonCancel
	{
		get
		{
			return base.CGet<CButtonObsolete>("Cancel");
		}
	}

	// Token: 0x06001BB4 RID: 7092 RVA: 0x000BEFCC File Offset: 0x000BD1CC
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get<ArtisanOrder>("ArtisanOrder", out this._artisanOrder);
		this._selectedType = this._artisanOrder.ItemSubType;
		argsBox.Get<Action>("OnConfirm", out this._onConfirm);
		this.ButtonConfirm.ClearAndAddListener(new Action(this.OnClickButtonConfirm));
		this.ButtonCancel.ClearAndAddListener(new Action(this.OnClickButtonCancel));
		this.ScrollView.OnItemRender = new Action<int, Refers>(this.OnItemRender);
		this.NeedWaitData = true;
		ExtraDomainMethod.AsyncCall.GetArtisanOrderCanProduceItemSubType(this, this._artisanOrder, delegate(int offset, RawDataPool pool)
		{
			Serializer.Deserialize(pool, offset, ref this._itemSubTypeList);
			this._itemSubTypeList.RemoveAll(delegate(short t)
			{
				bool flag = t < 0;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					MakeItemTypeItem config = (from m in MakeItemType.Instance
					where !m.TypeBigIcon.IsNullOrEmpty()
					select m).FirstOrDefault((MakeItemTypeItem m) => m.ItemSubType == t);
					result = (config == null);
				}
				return result;
			});
			this.ScrollView.UpdateData(this._itemSubTypeList.Count);
			this.Element.ShowAfterRefresh();
		});
	}

	// Token: 0x06001BB5 RID: 7093 RVA: 0x000BF078 File Offset: 0x000BD278
	private void OnItemRender(int index, Refers refers)
	{
		short itemSubType = this._itemSubTypeList[index];
		string icon;
		string name;
		UI_SelectProductType.GetMakeItemTypeInfo(itemSubType, out icon, out name);
		refers.CGet<TextMeshProUGUI>("Name").text = name;
		refers.CGet<CImage>("Image").SetSprite(icon, false, null);
		bool isSelected = this._selectedType == itemSubType;
		refers.CGet<GameObject>("Selected").SetActive(isSelected);
		refers.CGet<CButtonObsolete>("Button").ClearAndAddListener(delegate
		{
			this._selectedType = itemSubType;
			this.ScrollView.UpdateData(this._itemSubTypeList.Count);
		});
	}

	// Token: 0x06001BB6 RID: 7094 RVA: 0x000BF11C File Offset: 0x000BD31C
	public static void GetMakeItemTypeInfo(short itemSubType, out string icon, out string name)
	{
		MakeItemTypeItem config = (from m in MakeItemType.Instance
		where !m.TypeBigIcon.IsNullOrEmpty()
		select m).FirstOrDefault((MakeItemTypeItem m) => m.ItemSubType == itemSubType);
		name = (((config != null) ? config.TypeName : null) ?? LocalStringManager.Get(LanguageKey.LK_Item_Filter_Type_All));
		icon = (((config != null) ? config.TypeBigIcon : null) ?? "building_icon_wu_0");
	}

	// Token: 0x06001BB7 RID: 7095 RVA: 0x000BF1A5 File Offset: 0x000BD3A5
	private void OnClickButtonConfirm()
	{
		this._artisanOrder.ItemSubType = this._selectedType;
		this._onConfirm();
		this.QuickHide();
	}

	// Token: 0x06001BB8 RID: 7096 RVA: 0x000BF1CC File Offset: 0x000BD3CC
	private void OnClickButtonCancel()
	{
		this.QuickHide();
	}

	// Token: 0x0400159F RID: 5535
	private List<short> _itemSubTypeList = new List<short>();

	// Token: 0x040015A0 RID: 5536
	private ArtisanOrder _artisanOrder;

	// Token: 0x040015A1 RID: 5537
	private short _selectedType;

	// Token: 0x040015A2 RID: 5538
	private Action _onConfirm;
}
