using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200034C RID: 844
public class CharacterSort : Refers
{
	// Token: 0x06003154 RID: 12628 RVA: 0x00184774 File Offset: 0x00182974
	public void Init(CharacterSortFilterSetting sortFilterSetting, Action<CharacterSortFilterSetting> onSortOrderChange)
	{
		this._sortFilterSetting = sortFilterSetting;
		this._onSortOrderChange = onSortOrderChange;
		this._sortBtnHolder = base.CGet<Refers>("SortTypeHolder");
		sbyte i = 0;
		while ((int)i < this._sortBtnHolder.Names.Count)
		{
			string typeName = this._sortBtnHolder.Names[(int)i];
			sbyte type = this._nameToTypes[typeName];
			CButtonObsolete sortBtn = this._sortBtnHolder.CGet<CButtonObsolete>(typeName);
			sortBtn.GetComponent<TooltipInvoker>().PresetParam[0] = this._tipDesc[type];
			bool flag = !sortBtn.gameObject.activeSelf;
			if (!flag)
			{
				Refers sortBtnRefers = sortBtn.GetComponent<Refers>();
				sortBtn.ClearAndAddListener(delegate
				{
					this.OnClickSortType(sortBtn, type);
				});
				int index;
				for (index = 0; index < this._sortFilterSetting.SortOrders.Count; index++)
				{
					bool flag2 = this._sortFilterSetting.SortOrders[index].Item1 == (int)type;
					if (flag2)
					{
						break;
					}
				}
				bool flag3 = index >= this._sortFilterSetting.SortOrders.Count;
				if (flag3)
				{
					index = -1;
				}
				this.UpdateSorter(sortBtnRefers, index);
			}
			i += 1;
		}
	}

	// Token: 0x06003155 RID: 12629 RVA: 0x001848EC File Offset: 0x00182AEC
	public List<ValueTuple<int, bool>> GetCurrentSortConfig()
	{
		return this._sortFilterSetting.SortOrders;
	}

	// Token: 0x06003156 RID: 12630 RVA: 0x0018490C File Offset: 0x00182B0C
	private void OnClickSortType(CButtonObsolete btn, sbyte type)
	{
		Refers sorterRefers = btn.GetComponent<Refers>();
		int index;
		for (index = 0; index < this._sortFilterSetting.SortOrders.Count; index++)
		{
			bool flag = this._sortFilterSetting.SortOrders[index].Item1 == (int)type;
			if (flag)
			{
				break;
			}
		}
		bool flag2 = index < this._sortFilterSetting.SortOrders.Count;
		if (flag2)
		{
			bool item = this._sortFilterSetting.SortOrders[index].Item2;
			if (item)
			{
				ValueTuple<int, bool> tuple = this._sortFilterSetting.SortOrders[index];
				tuple.Item2 = false;
				this._sortFilterSetting.SortOrders[index] = tuple;
			}
			else
			{
				this._sortFilterSetting.SortOrders.RemoveAt(index);
				index = -1;
				for (int i = 0; i < this._sortFilterSetting.SortOrders.Count; i++)
				{
					ValueTuple<int, bool> sortOrder = this._sortFilterSetting.SortOrders[i];
					sbyte sortType = (sbyte)sortOrder.Item1;
					bool flag3 = !this._typeNames.ContainsKey(sortType);
					if (flag3)
					{
						Debug.LogWarning(string.Format("Unrecognized sort type {0}.", sortType));
					}
					else
					{
						string typeName = this._typeNames[sortType];
						CButtonObsolete sortBtn = this._sortBtnHolder.CGet<CButtonObsolete>(typeName);
						sortBtn.GetComponent<Refers>().CGet<TextMeshProUGUI>("Index").text = (i + 1).ToString();
					}
				}
			}
		}
		else
		{
			this._sortFilterSetting.SortOrders.Add(new ValueTuple<int, bool>((int)type, true));
		}
		this.UpdateSorter(sorterRefers, index);
		this._onSortOrderChange(this._sortFilterSetting);
	}

	// Token: 0x06003157 RID: 12631 RVA: 0x00184ADC File Offset: 0x00182CDC
	private void UpdateSorter(Refers sorterRefers, int index)
	{
		RectTransform arrow = sorterRefers.CGet<RectTransform>("Arrow");
		bool isDesc = index >= 0 && this._sortFilterSetting.SortOrders[index].Item2;
		arrow.gameObject.SetActive(index >= 0);
		arrow.localRotation = SortFilter.GetArrowRotation(isDesc);
		arrow.anchoredPosition = SortFilter.GetArrowAnchoredPos(isDesc);
		sorterRefers.CGet<CImage>("CheckMark").SetAlpha((float)((index >= 0) ? 1 : 0));
		sorterRefers.CGet<TextMeshProUGUI>("Index").text = ((index < 0) ? "" : string.Format("{0}", index + 1));
		sorterRefers.CGet<GameObject>("IndexBg").SetActive(index >= 0);
	}

	// Token: 0x06003158 RID: 12632 RVA: 0x00184BA4 File Offset: 0x00182DA4
	public void SetSortButtonActive(sbyte type, bool show)
	{
		string typeName;
		bool flag = this._typeNames.TryGetValue(type, out typeName);
		if (flag)
		{
			CButtonObsolete sortBtn = this._sortBtnHolder.CGet<CButtonObsolete>(typeName);
			sortBtn.gameObject.SetActive(show);
		}
	}

	// Token: 0x04002420 RID: 9248
	private Dictionary<sbyte, string> _typeNames = new Dictionary<sbyte, string>
	{
		{
			0,
			"Name"
		},
		{
			1,
			"Grade"
		},
		{
			2,
			"Age"
		},
		{
			3,
			"Health"
		},
		{
			4,
			"Gender"
		},
		{
			5,
			"BehaviorType"
		},
		{
			6,
			"Happiness"
		},
		{
			7,
			"FavorabilityToTaiwu"
		},
		{
			8,
			"Fame"
		},
		{
			9,
			"LivingStatus"
		},
		{
			10,
			"WorkStatus"
		},
		{
			34,
			"ConsummateLevel"
		},
		{
			35,
			"PrisonTime"
		},
		{
			36,
			"PunishmentSeverity"
		},
		{
			37,
			"BountyAmount"
		},
		{
			39,
			"LifeSkillAttainment"
		}
	};

	// Token: 0x04002421 RID: 9249
	private Dictionary<string, sbyte> _nameToTypes = new Dictionary<string, sbyte>
	{
		{
			"Name",
			0
		},
		{
			"Grade",
			1
		},
		{
			"Age",
			2
		},
		{
			"Health",
			3
		},
		{
			"Gender",
			4
		},
		{
			"BehaviorType",
			5
		},
		{
			"Happiness",
			6
		},
		{
			"FavorabilityToTaiwu",
			7
		},
		{
			"Fame",
			8
		},
		{
			"LivingStatus",
			9
		},
		{
			"WorkStatus",
			10
		},
		{
			"ConsummateLevel",
			34
		},
		{
			"PrisonTime",
			35
		},
		{
			"PunishmentSeverity",
			36
		},
		{
			"BountyAmount",
			37
		},
		{
			"LifeSkillAttainment",
			39
		}
	};

	// Token: 0x04002422 RID: 9250
	private Dictionary<sbyte, string> _tipDesc = new Dictionary<sbyte, string>
	{
		{
			0,
			LocalStringManager.Get(LanguageKey.LK_Mousetip_Sort_Desc_Name)
		},
		{
			1,
			LocalStringManager.Get(LanguageKey.LK_Mousetip_Sort_Desc_Grade)
		},
		{
			2,
			LocalStringManager.Get(LanguageKey.LK_Char_Age)
		},
		{
			3,
			LocalStringManager.Get(LanguageKey.LK_Health)
		},
		{
			4,
			LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Gender)
		},
		{
			5,
			LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Behavior)
		},
		{
			6,
			LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Happiness)
		},
		{
			7,
			LocalStringManager.Get(LanguageKey.LK_Favorability)
		},
		{
			8,
			LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Fame)
		},
		{
			9,
			LocalStringManager.Get(LanguageKey.LK_Mousetip_Sort_Desc_LivingStatus)
		},
		{
			10,
			LocalStringManager.Get(LanguageKey.LK_Mousetip_Sort_Desc_WorkStatus)
		},
		{
			34,
			LocalStringManager.Get(LanguageKey.LK_Mousetip_Sort_Desc_ConsummateLevel)
		},
		{
			35,
			LocalStringManager.Get(LanguageKey.LK_PrisonTime)
		},
		{
			36,
			LocalStringManager.Get(LanguageKey.LK_PunishmentSeverity)
		},
		{
			37,
			LocalStringManager.Get(LanguageKey.LK_BountyAmount)
		},
		{
			39,
			LocalStringManager.Get(LanguageKey.LK_Attainment)
		}
	};

	// Token: 0x04002423 RID: 9251
	private Refers _sortBtnHolder;

	// Token: 0x04002424 RID: 9252
	private CharacterSortFilterSetting _sortFilterSetting;

	// Token: 0x04002425 RID: 9253
	private Action<CharacterSortFilterSetting> _onSortOrderChange;

	// Token: 0x020016DA RID: 5850
	[Obsolete("Use GameData.Domains.Character.SortFilter.CharacterSortingType instead.")]
	public static class SortType
	{
		// Token: 0x0400A949 RID: 43337
		public const sbyte Name = 0;

		// Token: 0x0400A94A RID: 43338
		public const sbyte Grade = 1;

		// Token: 0x0400A94B RID: 43339
		public const sbyte Age = 2;

		// Token: 0x0400A94C RID: 43340
		public const sbyte Health = 3;

		// Token: 0x0400A94D RID: 43341
		public const sbyte Gender = 4;

		// Token: 0x0400A94E RID: 43342
		public const sbyte BehaviorType = 5;

		// Token: 0x0400A94F RID: 43343
		public const sbyte Happiness = 6;

		// Token: 0x0400A950 RID: 43344
		public const sbyte Favorite = 7;

		// Token: 0x0400A951 RID: 43345
		public const sbyte Fame = 8;

		// Token: 0x0400A952 RID: 43346
		public const sbyte Reside = 9;

		// Token: 0x0400A953 RID: 43347
		public const sbyte Work = 10;

		// Token: 0x0400A954 RID: 43348
		public const sbyte Count = 11;
	}
}
