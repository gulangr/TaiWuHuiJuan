using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020002ED RID: 749
[Obsolete]
public class UI_MouseTipVillagerRoleAvailableCount : MouseTipBase
{
	// Token: 0x06002BFD RID: 11261 RVA: 0x0015883C File Offset: 0x00156A3C
	protected override void Init(ArgumentBox argsBox)
	{
		bool flag = !this._isInit;
		if (flag)
		{
			this.InitRefers();
		}
		this.Refresh(argsBox);
	}

	// Token: 0x06002BFE RID: 11262 RVA: 0x00158868 File Offset: 0x00156A68
	public override void Refresh(ArgumentBox argsBox)
	{
		short roleId;
		argsBox.Get("RoleId", out roleId);
		TaiwuDomainMethod.AsyncCall.GetVillagerRoleTipsDisplayData(this, roleId, delegate(int offset, RawDataPool pool)
		{
			VillagerRoleTipsDisplayData displayData = new VillagerRoleTipsDisplayData();
			Serializer.Deserialize(pool, offset, ref displayData);
			this.RefreshAll(displayData);
		});
	}

	// Token: 0x06002BFF RID: 11263 RVA: 0x00158898 File Offset: 0x00156A98
	private void RefreshAll(VillagerRoleTipsDisplayData displayData)
	{
		this.RefreshNeedBuildingClass(displayData);
	}

	// Token: 0x06002C00 RID: 11264 RVA: 0x001588A4 File Offset: 0x00156AA4
	private void RefreshNeedBuildingClass(VillagerRoleTipsDisplayData displayData)
	{
		List<int> relatedBuildingClassList = displayData.RelatedBuildingClassList;
		bool flag = relatedBuildingClassList == null;
		if (flag)
		{
			CommonUtils.PrepareEnoughChildren(this._needBuildingItemLayout.transform, this._needBuildingItemTemplate.gameObject, 0, null);
		}
		else
		{
			relatedBuildingClassList.Sort();
			CommonUtils.PrepareEnoughChildren(this._needBuildingItemLayout.transform, this._needBuildingItemTemplate.gameObject, relatedBuildingClassList.Count, null);
			for (int i = 0; i < relatedBuildingClassList.Count; i++)
			{
				EBuildingBlockClass buildingClass = (EBuildingBlockClass)relatedBuildingClassList[i];
				Refers item = this._needBuildingItemLayout.GetChild(i).GetComponent<Refers>();
				CImage icon = item.CGet<CImage>("Icon");
				TextMeshProUGUI buildName = item.CGet<TextMeshProUGUI>("BuildName");
				icon.SetSprite(CommonUtils.GetBuildingClassTipIcon(buildingClass), false, null);
				buildName.text = LocalStringManager.Get(string.Format("LK_BuildingClassName_{0}", (int)buildingClass));
			}
		}
	}

	// Token: 0x06002C01 RID: 11265 RVA: 0x0015899F File Offset: 0x00156B9F
	private void InitRefers()
	{
		this._needBuildingItemLayout = base.CGet<RectTransform>("NeedBuildingItemLayout");
		this._needBuildingItemTemplate = base.CGet<Refers>("NeedBuildingItemTemplate");
	}

	// Token: 0x04001FEC RID: 8172
	private bool _isInit = false;

	// Token: 0x04001FED RID: 8173
	private RectTransform _needBuildingItemLayout;

	// Token: 0x04001FEE RID: 8174
	private Refers _needBuildingItemTemplate;
}
