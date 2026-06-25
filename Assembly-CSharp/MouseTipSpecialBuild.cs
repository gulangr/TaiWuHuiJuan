using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x020002D9 RID: 729
public class MouseTipSpecialBuild : MouseTipBase
{
	// Token: 0x170004BD RID: 1213
	// (get) Token: 0x06002B63 RID: 11107 RVA: 0x0015274B File Offset: 0x0015094B
	private TextMeshProUGUI _title
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("Title");
		}
	}

	// Token: 0x170004BE RID: 1214
	// (get) Token: 0x06002B64 RID: 11108 RVA: 0x00152758 File Offset: 0x00150958
	private TextMeshProUGUI _desc
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("Desc");
		}
	}

	// Token: 0x170004BF RID: 1215
	// (get) Token: 0x06002B65 RID: 11109 RVA: 0x00152765 File Offset: 0x00150965
	private GameObject _resourceHolder
	{
		get
		{
			return base.CGet<GameObject>("NeedResourceHolder");
		}
	}

	// Token: 0x170004C0 RID: 1216
	// (get) Token: 0x06002B66 RID: 11110 RVA: 0x00152772 File Offset: 0x00150972
	private Refers _foodResourceInfo
	{
		get
		{
			return base.CGet<Refers>("FoodResourceInfo");
		}
	}

	// Token: 0x170004C1 RID: 1217
	// (get) Token: 0x06002B67 RID: 11111 RVA: 0x0015277F File Offset: 0x0015097F
	private Refers _woodResourceInfo
	{
		get
		{
			return base.CGet<Refers>("WoodResourceInfo");
		}
	}

	// Token: 0x170004C2 RID: 1218
	// (get) Token: 0x06002B68 RID: 11112 RVA: 0x0015278C File Offset: 0x0015098C
	private Refers _metalResourceInfo
	{
		get
		{
			return base.CGet<Refers>("MetalResourceInfo");
		}
	}

	// Token: 0x170004C3 RID: 1219
	// (get) Token: 0x06002B69 RID: 11113 RVA: 0x00152799 File Offset: 0x00150999
	private Refers _jadeResourceInfo
	{
		get
		{
			return base.CGet<Refers>("JadeResourceInfo");
		}
	}

	// Token: 0x170004C4 RID: 1220
	// (get) Token: 0x06002B6A RID: 11114 RVA: 0x001527A6 File Offset: 0x001509A6
	private Refers _fabricResourceInfo
	{
		get
		{
			return base.CGet<Refers>("FabricResourceInfo");
		}
	}

	// Token: 0x170004C5 RID: 1221
	// (get) Token: 0x06002B6B RID: 11115 RVA: 0x001527B3 File Offset: 0x001509B3
	private Refers _herbResourceInfo
	{
		get
		{
			return base.CGet<Refers>("HerbResourceInfo");
		}
	}

	// Token: 0x170004C6 RID: 1222
	// (get) Token: 0x06002B6C RID: 11116 RVA: 0x001527C0 File Offset: 0x001509C0
	private Refers _moneyResourceInfo
	{
		get
		{
			return base.CGet<Refers>("MoneyResourceInfo");
		}
	}

	// Token: 0x170004C7 RID: 1223
	// (get) Token: 0x06002B6D RID: 11117 RVA: 0x001527CD File Offset: 0x001509CD
	private Refers _authorityResourceInfo
	{
		get
		{
			return base.CGet<Refers>("AuthorityResourceInfo");
		}
	}

	// Token: 0x170004C8 RID: 1224
	// (get) Token: 0x06002B6E RID: 11118 RVA: 0x001527DA File Offset: 0x001509DA
	private TextMeshProUGUI _approveContent
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("ApproveContent");
		}
	}

	// Token: 0x170004C9 RID: 1225
	// (get) Token: 0x06002B6F RID: 11119 RVA: 0x001527E7 File Offset: 0x001509E7
	private RectTransform _errorLayout
	{
		get
		{
			return base.CGet<RectTransform>("ErrorLayout");
		}
	}

	// Token: 0x170004CA RID: 1226
	// (get) Token: 0x06002B70 RID: 11120 RVA: 0x001527F4 File Offset: 0x001509F4
	private TextMeshProUGUI _errorContentPrefab
	{
		get
		{
			return base.CGet<TextMeshProUGUI>("ErrorContent");
		}
	}

	// Token: 0x170004CB RID: 1227
	// (get) Token: 0x06002B71 RID: 11121 RVA: 0x00152801 File Offset: 0x00150A01
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170004CC RID: 1228
	// (get) Token: 0x06002B72 RID: 11122 RVA: 0x00152804 File Offset: 0x00150A04
	private BuildingModel _buildingModel
	{
		get
		{
			return SingletonObject.getInstance<BuildingModel>();
		}
	}

	// Token: 0x06002B73 RID: 11123 RVA: 0x0015280C File Offset: 0x00150A0C
	protected override void Init(ArgumentBox argsBox)
	{
		this.InitUIBind();
		TextMeshProUGUI textTitle = base.CGet<TextMeshProUGUI>("Title");
		argsBox.Get<BuildingBlockItem>("BuildingBlockItem", out this._config);
		argsBox.Get<List<ESpecialBuildErrorType>>("ErrorTypes", out this._errorType);
		argsBox.Get("ApproveRate", out this._approveRate);
		argsBox.Get("ApproveNeeded", out this._approveNeeded);
		argsBox.Get("CurCount", out this._curCount);
		argsBox.Get("MaxCount", out this._maxCount);
		this.SetupContent();
	}

	// Token: 0x06002B74 RID: 11124 RVA: 0x001528A0 File Offset: 0x00150AA0
	private void InitUIBind()
	{
		bool uiInited = this._uiInited;
		if (!uiInited)
		{
			this._resourceObjList = new List<Refers>();
			this._resourceObjList.Add(this._foodResourceInfo);
			this._resourceObjList.Add(this._woodResourceInfo);
			this._resourceObjList.Add(this._metalResourceInfo);
			this._resourceObjList.Add(this._jadeResourceInfo);
			this._resourceObjList.Add(this._fabricResourceInfo);
			this._resourceObjList.Add(this._herbResourceInfo);
			this._resourceObjList.Add(this._moneyResourceInfo);
			this._resourceObjList.Add(this._authorityResourceInfo);
			this._resourceTextList = (from t in this._resourceObjList
			select t.CGet<TextMeshProUGUI>("ResourceCount")).ToList<TextMeshProUGUI>();
			this._uiInited = true;
		}
	}

	// Token: 0x06002B75 RID: 11125 RVA: 0x00152994 File Offset: 0x00150B94
	private void SetupContent()
	{
		this._title.text = LocalStringManager.Get(LanguageKey.LK_MouseTip_SpecialBuild_BuildQiwenxingtai);
		this._desc.text = this._config.Desc;
		this.SetResourceInfo((from x in this._config.BaseBuildCost
		select (int)x).ToList<int>());
		this._approveContent.text = LocalStringManager.GetFormat(LanguageKey.LK_MouseTip_SpecialBuild_CurrentApproving, (float)this._approveNeeded / 10f);
		this._curAndMax.text = LanguageKey.LK_MouseTip_SpecialBuild_CurrentCount.TrFormat(this._curCount, this._maxCount);
		this.SetupErrorType();
	}

	// Token: 0x06002B76 RID: 11126 RVA: 0x00152A68 File Offset: 0x00150C68
	private void SetupErrorType()
	{
		bool flag = this._errorType == null;
		if (flag)
		{
			this._errorType = new List<ESpecialBuildErrorType>();
		}
		CommonUtils.PrepareEnoughChildren(this._errorLayout.transform, this._errorContentPrefab.gameObject, this._errorType.Count, null);
		for (int i = 0; i < this._errorType.Count; i++)
		{
			this._errorLayout.GetChild(i).GetComponent<TextMeshProUGUI>().text = this.GetErrorMessage(this._errorType[i]);
		}
	}

	// Token: 0x06002B77 RID: 11127 RVA: 0x00152B08 File Offset: 0x00150D08
	private string GetErrorMessage(ESpecialBuildErrorType eSpecialBuildErrorType)
	{
		LanguageKey key;
		switch (eSpecialBuildErrorType)
		{
		case ESpecialBuildErrorType.Resrouce:
			key = LanguageKey.LK_MouseTip_SpecialBuild_Error_Resource;
			break;
		case ESpecialBuildErrorType.Approve:
			key = LanguageKey.LK_MouseTip_SpecialBuild_Error_Approving;
			break;
		case ESpecialBuildErrorType.AlreadyBuilt:
			key = LanguageKey.LK_MouseTip_SpecialBuild_Error_Built;
			break;
		case ESpecialBuildErrorType.NotAvailable:
			key = LanguageKey.LK_MouseTip_SpecialBuild_Error_NotAvailable;
			break;
		case ESpecialBuildErrorType.ReachMaxCount:
			key = LanguageKey.LK_MouseTip_SpecialBuild_Error_ReachMaxCount;
			break;
		default:
			return "";
		}
		return LocalStringManager.Get(key).ColorReplace();
	}

	// Token: 0x06002B78 RID: 11128 RVA: 0x00152B78 File Offset: 0x00150D78
	private void SetResourceInfo(List<int> resourceCountList)
	{
		bool costResource = true;
		for (sbyte i = 0; i < 8; i += 1)
		{
			bool valid = resourceCountList.CheckIndex((int)i);
			this._resourceObjList[(int)i].gameObject.SetActive(valid && resourceCountList[(int)i] > 0);
			bool flag = valid && resourceCountList[(int)i] > 0;
			if (flag)
			{
				bool flag2 = costResource;
				string resourceStr;
				if (flag2)
				{
					string color = (this._buildingModel.GetResourceCount(i) >= resourceCountList[(int)i]) ? "brightblue" : "brightred";
					string ownStr = CommonUtils.GetDisplayStringForNum(this._buildingModel.GetResourceCount(i), 100000).SetColor(color);
					resourceStr = string.Format("{0}/{1}", ownStr, resourceCountList[(int)i]);
				}
				else
				{
					resourceStr = resourceCountList[(int)i].ToString();
				}
				this._resourceTextList[(int)i].text = resourceStr;
			}
		}
	}

	// Token: 0x06002B79 RID: 11129 RVA: 0x00152C7A File Offset: 0x00150E7A
	public override void Refresh(ArgumentBox argBox)
	{
		this.Init(argBox);
	}

	// Token: 0x04001F8F RID: 8079
	private BuildingBlockItem _config;

	// Token: 0x04001F90 RID: 8080
	private List<ESpecialBuildErrorType> _errorType;

	// Token: 0x04001F91 RID: 8081
	private short _approveRate;

	// Token: 0x04001F92 RID: 8082
	private short _approveNeeded;

	// Token: 0x04001F93 RID: 8083
	[SerializeField]
	private TMP_Text _curAndMax;

	// Token: 0x04001F94 RID: 8084
	private int _curCount;

	// Token: 0x04001F95 RID: 8085
	private int _maxCount;

	// Token: 0x04001F96 RID: 8086
	private List<Refers> _resourceObjList;

	// Token: 0x04001F97 RID: 8087
	private List<TextMeshProUGUI> _resourceTextList;

	// Token: 0x04001F98 RID: 8088
	private bool _uiInited = false;
}
