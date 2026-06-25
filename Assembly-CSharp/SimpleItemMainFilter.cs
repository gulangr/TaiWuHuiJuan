using System;
using UnityEngine;

// Token: 0x02000359 RID: 857
public class SimpleItemMainFilter : Refers
{
	// Token: 0x1700056D RID: 1389
	// (get) Token: 0x060031E6 RID: 12774 RVA: 0x0018A35E File Offset: 0x0018855E
	// (set) Token: 0x060031E7 RID: 12775 RVA: 0x0018A368 File Offset: 0x00188568
	public SimpleItemMainFilter.ItemFilterType SelectedType
	{
		get
		{
			return this._selectedType;
		}
		set
		{
			this._selectedType = value;
			for (int i = 0; i < SimpleItemMainFilter.FilterTypes.Length; i++)
			{
				bool flag = SimpleItemMainFilter.FilterTypes[i] == this._selectedType;
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

	// Token: 0x060031E8 RID: 12776 RVA: 0x0018A3DF File Offset: 0x001885DF
	public void SetCallback(Action callback)
	{
		this._onSelectedTypeChanged = callback;
	}

	// Token: 0x060031E9 RID: 12777 RVA: 0x0018A3EC File Offset: 0x001885EC
	public bool IsItemTypeMatch(sbyte itemType)
	{
		bool flag = this._selectedType == SimpleItemMainFilter.ItemFilterType.All;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			SimpleItemMainFilter.ItemFilterType filterType = SimpleItemMainFilter.GetFilterType(itemType);
			result = (filterType == this._selectedType);
		}
		return result;
	}

	// Token: 0x060031EA RID: 12778 RVA: 0x0018A420 File Offset: 0x00188620
	private static SimpleItemMainFilter.ItemFilterType GetFilterType(sbyte itemType)
	{
		SimpleItemMainFilter.ItemFilterType result;
		switch (itemType)
		{
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
			result = SimpleItemMainFilter.ItemFilterType.Equip;
			break;
		case 5:
			result = SimpleItemMainFilter.ItemFilterType.Material;
			break;
		case 6:
			result = SimpleItemMainFilter.ItemFilterType.Make;
			break;
		case 7:
		case 9:
			result = SimpleItemMainFilter.ItemFilterType.Food;
			break;
		case 8:
			result = SimpleItemMainFilter.ItemFilterType.Medicine;
			break;
		case 10:
			result = SimpleItemMainFilter.ItemFilterType.Book;
			break;
		default:
			result = SimpleItemMainFilter.ItemFilterType.Other;
			break;
		}
		return result;
	}

	// Token: 0x060031EB RID: 12779 RVA: 0x0018A483 File Offset: 0x00188683
	public void Init()
	{
		this.InitRefers();
		this.InitToggles();
	}

	// Token: 0x060031EC RID: 12780 RVA: 0x0018A494 File Offset: 0x00188694
	private void InitToggles()
	{
		this._toggles = new CToggleObsolete[]
		{
			this._all,
			this._food,
			this._medicine,
			this._equip,
			this._book,
			this._manufacture,
			this._materail,
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
					this._selectedType = SimpleItemMainFilter.FilterTypes[ii];
					Action onSelectedTypeChanged = this._onSelectedTypeChanged;
					if (onSelectedTypeChanged != null)
					{
						onSelectedTypeChanged();
					}
				}
			});
		}
	}

	// Token: 0x060031ED RID: 12781 RVA: 0x0018A558 File Offset: 0x00188758
	public RectTransform GetSelectedToggleTransform()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			bool flag = i == SimpleItemMainFilter.FilterTypes.IndexOf(this._selectedType);
			if (flag)
			{
				return base.transform.GetChild(i).GetComponent<RectTransform>();
			}
		}
		return null;
	}

	// Token: 0x060031EE RID: 12782 RVA: 0x0018A5B4 File Offset: 0x001887B4
	private void InitRefers()
	{
		this._all = base.CGet<CToggleObsolete>("All");
		this._food = base.CGet<CToggleObsolete>("Food");
		this._medicine = base.CGet<CToggleObsolete>("Medicine");
		this._equip = base.CGet<CToggleObsolete>("Equip");
		this._book = base.CGet<CToggleObsolete>("Book");
		this._manufacture = base.CGet<CToggleObsolete>("Manufacture");
		this._materail = base.CGet<CToggleObsolete>("Materail");
		this._other = base.CGet<CToggleObsolete>("Other");
	}

	// Token: 0x04002494 RID: 9364
	private SimpleItemMainFilter.ItemFilterType _selectedType;

	// Token: 0x04002495 RID: 9365
	private Action _onSelectedTypeChanged;

	// Token: 0x04002496 RID: 9366
	private CToggleObsolete[] _toggles;

	// Token: 0x04002497 RID: 9367
	private static readonly SimpleItemMainFilter.ItemFilterType[] FilterTypes = new SimpleItemMainFilter.ItemFilterType[]
	{
		SimpleItemMainFilter.ItemFilterType.All,
		SimpleItemMainFilter.ItemFilterType.Food,
		SimpleItemMainFilter.ItemFilterType.Medicine,
		SimpleItemMainFilter.ItemFilterType.Equip,
		SimpleItemMainFilter.ItemFilterType.Book,
		SimpleItemMainFilter.ItemFilterType.Make,
		SimpleItemMainFilter.ItemFilterType.Material,
		SimpleItemMainFilter.ItemFilterType.Other
	};

	// Token: 0x04002498 RID: 9368
	private CToggleObsolete _all;

	// Token: 0x04002499 RID: 9369
	private CToggleObsolete _food;

	// Token: 0x0400249A RID: 9370
	private CToggleObsolete _medicine;

	// Token: 0x0400249B RID: 9371
	private CToggleObsolete _equip;

	// Token: 0x0400249C RID: 9372
	private CToggleObsolete _book;

	// Token: 0x0400249D RID: 9373
	private CToggleObsolete _manufacture;

	// Token: 0x0400249E RID: 9374
	private CToggleObsolete _materail;

	// Token: 0x0400249F RID: 9375
	private CToggleObsolete _other;

	// Token: 0x02001706 RID: 5894
	public enum ItemFilterType
	{
		// Token: 0x0400AA13 RID: 43539
		All,
		// Token: 0x0400AA14 RID: 43540
		Food,
		// Token: 0x0400AA15 RID: 43541
		Medicine,
		// Token: 0x0400AA16 RID: 43542
		Equip,
		// Token: 0x0400AA17 RID: 43543
		Book,
		// Token: 0x0400AA18 RID: 43544
		Make,
		// Token: 0x0400AA19 RID: 43545
		Material,
		// Token: 0x0400AA1A RID: 43546
		Other,
		// Token: 0x0400AA1B RID: 43547
		Count
	}
}
