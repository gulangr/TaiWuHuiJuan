using System;

// Token: 0x0200035D RID: 861
public class SimpleVillagerFarmerFilter : Refers
{
	// Token: 0x17000572 RID: 1394
	// (get) Token: 0x06003226 RID: 12838 RVA: 0x0018BF34 File Offset: 0x0018A134
	// (set) Token: 0x06003227 RID: 12839 RVA: 0x0018BF3C File Offset: 0x0018A13C
	public SimpleVillagerFarmerFilter.VillagerRoleFilterType SelectedType
	{
		get
		{
			return this._selectedType;
		}
		set
		{
			this._selectedType = value;
			for (int i = 0; i < SimpleVillagerFarmerFilter.FilterTypes.Length; i++)
			{
				bool flag = SimpleVillagerFarmerFilter.FilterTypes[i] == this._selectedType;
				if (flag)
				{
					bool isOn = this._toggles[i].isOn;
					if (isOn)
					{
						Action onSelectedTypeChanged = this._onSelectedTypeChanged;
						if (onSelectedTypeChanged != null)
						{
							onSelectedTypeChanged();
						}
					}
					else
					{
						this._toggles[i].isOn = true;
					}
					break;
				}
			}
		}
	}

	// Token: 0x06003228 RID: 12840 RVA: 0x0018BFB3 File Offset: 0x0018A1B3
	public void SetCallback(Action callback)
	{
		this._onSelectedTypeChanged = callback;
	}

	// Token: 0x06003229 RID: 12841 RVA: 0x0018BFC0 File Offset: 0x0018A1C0
	public bool IsVillagerRoleMatch(short roleTemplateId)
	{
		bool flag = this._selectedType == SimpleVillagerFarmerFilter.VillagerRoleFilterType.All;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			SimpleVillagerFarmerFilter.VillagerRoleFilterType filterType = SimpleVillagerFarmerFilter.GetFilterType(roleTemplateId);
			result = (filterType == this._selectedType);
		}
		return result;
	}

	// Token: 0x0600322A RID: 12842 RVA: 0x0018BFF4 File Offset: 0x0018A1F4
	private static SimpleVillagerFarmerFilter.VillagerRoleFilterType GetFilterType(short roleTemplateId)
	{
		SimpleVillagerFarmerFilter.VillagerRoleFilterType result;
		if (roleTemplateId != 0)
		{
			result = SimpleVillagerFarmerFilter.VillagerRoleFilterType.Other;
		}
		else
		{
			result = SimpleVillagerFarmerFilter.VillagerRoleFilterType.Farmer;
		}
		return result;
	}

	// Token: 0x0600322B RID: 12843 RVA: 0x0018C015 File Offset: 0x0018A215
	public void Init()
	{
		this.InitRefers();
		this.InitToggles();
	}

	// Token: 0x0600322C RID: 12844 RVA: 0x0018C028 File Offset: 0x0018A228
	private void InitToggles()
	{
		this._toggles = new CToggleObsolete[]
		{
			this._all,
			this._farmer,
			this._other
		};
		for (int i = 0; i < this._toggles.Length; i++)
		{
			this._toggles[i].onValueChanged.RemoveAllListeners();
			int ii = i;
			this._toggles[i].onValueChanged.AddListener(delegate(bool isOn)
			{
				bool flag = !isOn;
				if (!flag)
				{
					this._selectedType = SimpleVillagerFarmerFilter.FilterTypes[ii];
					Action onSelectedTypeChanged = this._onSelectedTypeChanged;
					if (onSelectedTypeChanged != null)
					{
						onSelectedTypeChanged();
					}
				}
			});
		}
	}

	// Token: 0x0600322D RID: 12845 RVA: 0x0018C0BC File Offset: 0x0018A2BC
	private void InitRefers()
	{
		this._all = base.CGet<CToggleObsolete>("All");
		this._farmer = base.CGet<CToggleObsolete>("Farmer");
		this._other = base.CGet<CToggleObsolete>("Other");
	}

	// Token: 0x040024B2 RID: 9394
	private SimpleVillagerFarmerFilter.VillagerRoleFilterType _selectedType;

	// Token: 0x040024B3 RID: 9395
	private Action _onSelectedTypeChanged;

	// Token: 0x040024B4 RID: 9396
	private CToggleObsolete[] _toggles;

	// Token: 0x040024B5 RID: 9397
	private static readonly SimpleVillagerFarmerFilter.VillagerRoleFilterType[] FilterTypes = new SimpleVillagerFarmerFilter.VillagerRoleFilterType[]
	{
		SimpleVillagerFarmerFilter.VillagerRoleFilterType.All,
		SimpleVillagerFarmerFilter.VillagerRoleFilterType.Farmer,
		SimpleVillagerFarmerFilter.VillagerRoleFilterType.Other
	};

	// Token: 0x040024B6 RID: 9398
	private CToggleObsolete _all;

	// Token: 0x040024B7 RID: 9399
	private CToggleObsolete _farmer;

	// Token: 0x040024B8 RID: 9400
	private CToggleObsolete _other;

	// Token: 0x0200171D RID: 5917
	public enum VillagerRoleFilterType
	{
		// Token: 0x0400AA7A RID: 43642
		All,
		// Token: 0x0400AA7B RID: 43643
		Farmer,
		// Token: 0x0400AA7C RID: 43644
		Other,
		// Token: 0x0400AA7D RID: 43645
		Count
	}
}
