using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using GameData.Domains.Building;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

// Token: 0x020001AA RID: 426
[RequireComponent(typeof(CDropdownLegacy))]
public class BuildingStoreToDropdown : MonoBehaviour
{
	// Token: 0x1700029D RID: 669
	// (set) Token: 0x06001824 RID: 6180 RVA: 0x000943AC File Offset: 0x000925AC
	public int MakeItemMethod
	{
		set
		{
			this._makeItemMethod = value;
			bool flag = this._forcingDisplayingType != -1;
			if (!flag)
			{
				BuildingDomainMethod.AsyncCall.GetStoreLocation(this._parent, this._makeItemMethod, delegate(int offset, RawDataPool pool)
				{
					bool flag2 = this._forcingDisplayingType != -1;
					if (!flag2)
					{
						int index = 0;
						Serializer.Deserialize(pool, offset, ref index);
						this.ApplyDefaultValue(index);
						int makeItemMethod = this._makeItemMethod;
						bool flag3 = makeItemMethod < 0 && makeItemMethod >= -16;
						if (flag3)
						{
							BuildingDomainMethod.Call.SetStoreLocation(-1, index);
						}
					}
				});
			}
		}
	}

	// Token: 0x1700029E RID: 670
	// (get) Token: 0x06001825 RID: 6181 RVA: 0x000943F1 File Offset: 0x000925F1
	// (set) Token: 0x06001826 RID: 6182 RVA: 0x000943FC File Offset: 0x000925FC
	public bool InSettlement
	{
		get
		{
			return this._inSettlement;
		}
		set
		{
			this._inSettlement = value;
			this.RefreshLang(value);
		}
	}

	// Token: 0x1700029F RID: 671
	// (get) Token: 0x06001827 RID: 6183 RVA: 0x0009441A File Offset: 0x0009261A
	// (set) Token: 0x06001828 RID: 6184 RVA: 0x00094428 File Offset: 0x00092628
	public bool Active
	{
		get
		{
			return base.gameObject.activeSelf;
		}
		set
		{
			bool flag = value == this.Active;
			if (!flag)
			{
				base.gameObject.SetActive(value);
				bool flag2 = this.mask && !value;
				if (flag2)
				{
					this.mask.SetActive(false);
				}
			}
		}
	}

	// Token: 0x170002A0 RID: 672
	// (get) Token: 0x06001829 RID: 6185 RVA: 0x00094477 File Offset: 0x00092677
	public int Value
	{
		get
		{
			return this.dropdown.value;
		}
	}

	// Token: 0x0600182A RID: 6186 RVA: 0x00094484 File Offset: 0x00092684
	public void Init(IAsyncMethodRequestHandler asyncMethodRequestHandler, int makeItemMethod, Action<int> onChangeStock)
	{
		this._forcingDisplayingType = -1;
		this._parent = asyncMethodRequestHandler;
		this._onChangeStock = onChangeStock;
		this.MakeItemMethod = makeItemMethod;
	}

	// Token: 0x0600182B RID: 6187 RVA: 0x000944A4 File Offset: 0x000926A4
	public void OnEnable()
	{
		this.MakeItemMethod = this._makeItemMethod;
		this.InSettlement = SingletonObject.getInstance<WorldMapModel>().IsTaiwuOnSettlement;
	}

	// Token: 0x0600182C RID: 6188 RVA: 0x000944C8 File Offset: 0x000926C8
	public void OnChangeStock(int stock)
	{
		UIManager.Instance.SetEscHandler(null);
		Action<int> onChangeStock = this._onChangeStock;
		if (onChangeStock != null)
		{
			onChangeStock(stock);
		}
		BuildingDomainMethod.Call.SetStoreLocation(this._makeItemMethod, stock);
		GameObject gameObject = this.mask;
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
	}

	// Token: 0x0600182D RID: 6189 RVA: 0x00094515 File Offset: 0x00092715
	public void ApplyDefaultValue(int index)
	{
		this.dropdown.SetValueWithoutNotify(index);
	}

	// Token: 0x0600182E RID: 6190 RVA: 0x00094524 File Offset: 0x00092724
	public void ForceRefreshStorageType(int type)
	{
		CDropdownLegacy cdropdownLegacy = this.dropdown;
		if (!true)
		{
		}
		int forcingDisplayingType;
		switch (type)
		{
		case 1:
			forcingDisplayingType = 0;
			goto IL_4F;
		case 2:
			forcingDisplayingType = 1;
			goto IL_4F;
		case 3:
			forcingDisplayingType = 2;
			goto IL_4F;
		case 5:
			forcingDisplayingType = 3;
			goto IL_4F;
		}
		throw new NotImplementedException(string.Format("type {0} is not implemented.", type));
		IL_4F:
		if (!true)
		{
		}
		cdropdownLegacy.SetValueWithoutNotify(this._forcingDisplayingType = forcingDisplayingType);
	}

	// Token: 0x0600182F RID: 6191 RVA: 0x00094598 File Offset: 0x00092798
	public void RefreshLang(bool nearSettlement = false)
	{
		this.dropdown.options.Clear();
		this.dropdown.AddOptions(this.nameKeys.Zip(this.icons, (string k, Sprite sp) => new ValueTuple<string, Sprite>(k, sp)).Select(([TupleElementNames(new string[]
		{
			"k",
			"sp"
		})] ValueTuple<string, Sprite> tuple, int i) => new TMP_Dropdown.OptionData(LocalStringManager.Get(tuple.Item1), (nearSettlement || i != 0) ? tuple.Item2 : this.invInactiveIcon)).ToList<TMP_Dropdown.OptionData>());
	}

	// Token: 0x06001830 RID: 6192 RVA: 0x00094620 File Offset: 0x00092820
	public void OnGUI()
	{
		Transform trans = base.transform.Find("Dropdown List");
		RectTransform content = (trans != null) ? trans.GetComponentInChildren<CScrollRectLegacy>().Content : null;
		bool flag = content == null;
		if (!flag)
		{
			int childCount = content.childCount;
			bool active = SingletonObject.getInstance<BasicGameData>().CanShowMoreTogOnWarehouse();
			for (int i = 1; i < childCount; i++)
			{
				Transform item = content.GetChild(i);
				TooltipInvoker mouseTipDisplayer = item.GetComponent<TooltipInvoker>();
				mouseTipDisplayer.PresetParam[0] = LocalStringManager.Get((i != 1 || this._inSettlement) ? this.descKeys[i - 1] : "LK_MouseTip_BuildingStoreTo_Desc0_Far");
				bool flag2 = i < 3;
				if (!flag2)
				{
					DisableStyleRoot component = item.GetComponent<DisableStyleRoot>();
					if (component != null)
					{
						component.SetStyleEffect(!active, false);
					}
					CToggleObsolete cToggle = item.GetComponent<CToggleObsolete>();
					bool flag3 = cToggle != null;
					if (flag3)
					{
						cToggle.interactable = active;
					}
				}
			}
			RectTransform rectTransform = trans as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.SetHeight(this.contentHeight);
			}
			PositionFollower positionFollower = base.GetComponentInChildren<PositionFollower>();
			bool flag4 = positionFollower == null;
			if (!flag4)
			{
				CToggleObsolete[] toggles = base.GetComponentsInChildren<CToggleObsolete>();
				foreach (CToggleObsolete togCell in toggles)
				{
					bool flag5 = !togCell.gameObject.activeSelf;
					if (!flag5)
					{
						Transform transform = togCell.transform.Find("Disable");
						if (transform != null)
						{
							transform.gameObject.SetActive(togCell.isOn);
						}
						bool isOn = togCell.isOn;
						if (isOn)
						{
							positionFollower.Target = togCell.transform;
						}
					}
				}
				base.transform.Find("Dropdown List").GetComponent<Canvas>().sortingOrder = this.dropdownListSortingOrder;
			}
		}
	}

	// Token: 0x06001831 RID: 6193 RVA: 0x000947F0 File Offset: 0x000929F0
	public void OnShow(GameObject blocker)
	{
		Canvas blockerCanvas = (blocker != null) ? blocker.GetComponent<Canvas>() : null;
		bool flag = blockerCanvas != null;
		if (flag)
		{
			blockerCanvas.sortingOrder = this.dropdownBlockerSortingOrder;
		}
		UIManager.Instance.SetEscHandler(new Action(this.OnHide));
		GameObject gameObject = this.mask;
		if (gameObject != null)
		{
			gameObject.SetActive(true);
		}
		bool flag2 = this.canvas;
		if (flag2)
		{
			this.canvas.overrideSorting = true;
		}
	}

	// Token: 0x06001832 RID: 6194 RVA: 0x00094868 File Offset: 0x00092A68
	public void OnHide()
	{
		this.dropdown.Hide();
		bool flag = this.canvas;
		if (flag)
		{
			this.canvas.overrideSorting = false;
		}
		GameObject gameObject = this.mask;
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
	}

	// Token: 0x04001369 RID: 4969
	[SerializeField]
	private GameObject mask;

	// Token: 0x0400136A RID: 4970
	[ReadOnly]
	[SerializeField]
	private CDropdownLegacy dropdown;

	// Token: 0x0400136B RID: 4971
	[MaybeNull]
	[SerializeField]
	private Canvas canvas;

	// Token: 0x0400136C RID: 4972
	[SerializeField]
	private string[] nameKeys = new string[]
	{
		"LK_Building_StoreTo_0",
		"LK_Building_StoreTo_1",
		"LK_Building_StoreTo_2",
		"LK_Building_StoreTo_3"
	};

	// Token: 0x0400136D RID: 4973
	[SerializeField]
	private string[] descKeys = new string[]
	{
		"LK_MouseTip_BuildingStoreTo_Desc0",
		"LK_MouseTip_BuildingStoreTo_Desc1",
		"LK_MouseTip_BuildingStoreTo_Desc2",
		"LK_MouseTip_BuildingStoreTo_Desc3"
	};

	// Token: 0x0400136E RID: 4974
	[SerializeField]
	private float contentHeight = 160f;

	// Token: 0x0400136F RID: 4975
	[SerializeField]
	private Sprite[] icons = new Sprite[0];

	// Token: 0x04001370 RID: 4976
	[SerializeField]
	private Sprite invInactiveIcon;

	// Token: 0x04001371 RID: 4977
	[SerializeField]
	private int dropdownListSortingOrder = 640;

	// Token: 0x04001372 RID: 4978
	[SerializeField]
	private int dropdownBlockerSortingOrder = 639;

	// Token: 0x04001373 RID: 4979
	private int _makeItemMethod = -1;

	// Token: 0x04001374 RID: 4980
	private IAsyncMethodRequestHandler _parent;

	// Token: 0x04001375 RID: 4981
	private int _forcingDisplayingType = -1;

	// Token: 0x04001376 RID: 4982
	private bool _inSettlement;

	// Token: 0x04001377 RID: 4983
	private Action<int> _onChangeStock;
}
