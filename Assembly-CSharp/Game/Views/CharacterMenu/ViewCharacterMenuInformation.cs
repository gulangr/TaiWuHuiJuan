using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using Game.Components.Information;
using Game.Components.SortAndFilter;
using Game.Components.SortAndFilter.Information;
using GameData.Domains.Information;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BA6 RID: 2982
	public class ViewCharacterMenuInformation : UI_CharacterMenuSubPageBase
	{
		// Token: 0x1700100A RID: 4106
		// (get) Token: 0x060094D6 RID: 38102 RVA: 0x004555E2 File Offset: 0x004537E2
		public override LanguageKey TitleKey
		{
			get
			{
				return LanguageKey.LK_CharacterMenu_Title_Information;
			}
		}

		// Token: 0x060094D7 RID: 38103 RVA: 0x004555EC File Offset: 0x004537EC
		public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
		{
			return curSubTogglePage == ECharacterSubToggleBase.InformationBase && curSubPage == ECharacterSubPage.Information;
		}

		// Token: 0x060094D8 RID: 38104 RVA: 0x0045560C File Offset: 0x0045380C
		public override void OnInit(ArgumentBox argsBox)
		{
			this.switchToggle.Init(1);
			ToggleGroupHotkeyController.Set(this.Element, this.switchToggle, 1, null);
			this.switchToggle.OnActiveIndexChange += this.OnSwitchInfo;
			this.switchToggle.Set(1, false);
		}

		// Token: 0x060094D9 RID: 38105 RVA: 0x00455661 File Offset: 0x00453861
		private void OnSwitchInfo(int newIndex, int oldIndex)
		{
			this.scroll.gameObject.SetActive(newIndex == 0);
			this.briefScroll.gameObject.SetActive(newIndex == 1);
		}

		// Token: 0x060094DA RID: 38106 RVA: 0x00455690 File Offset: 0x00453890
		private void Awake()
		{
			this.toggleGroup.Init(new Action(this.OnSkillListChanged));
			this._sortAndFilterController = new InformationSortAndFilterController(this.sortAndFilter, false);
			this._sortAndFilterController.Init(new Action(this.OnSkillListChanged), "InformationSortAndFilter");
			this.scroll.InitPageCount();
			this.scroll.OnItemRender += this.OnItemRender;
			this.briefScroll.InitPageCount();
			this.briefScroll.OnItemRender += this.OnItemRender;
		}

		// Token: 0x060094DB RID: 38107 RVA: 0x0045572D File Offset: 0x0045392D
		private void OnEnable()
		{
			this.localLoadingAnim.SetLoadingState(true);
			this.RequestData();
		}

		// Token: 0x060094DC RID: 38108 RVA: 0x00455744 File Offset: 0x00453944
		public override void OnCurrentCharacterChange(int prevCharacterId)
		{
			bool flag = base.CharacterMenu.CurCharacterId < 0;
			if (!flag)
			{
				this.localLoadingAnim.SetLoadingState(true);
				this.RequestData();
			}
		}

		// Token: 0x060094DD RID: 38109 RVA: 0x0045577A File Offset: 0x0045397A
		private void RequestData()
		{
			InformationDomainMethod.AsyncCall.GetCharacterNormalInformation(this, base.CharacterMenu.CurCharacterId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._normalInformationCollection);
				this.RefreshToggleGroupLabel();
				this.OnSkillListChanged();
			});
		}

		// Token: 0x060094DE RID: 38110 RVA: 0x0045579C File Offset: 0x0045399C
		private void RefreshToggleGroupLabel()
		{
			foreach (InformationTypeItem item in ((IEnumerable<InformationTypeItem>)InformationType.Instance))
			{
				this._dataCount[item.TemplateId] = 0;
			}
			foreach (NormalInformation info in this._normalInformationCollection.GetList())
			{
				Dictionary<sbyte, int> dataCount = this._dataCount;
				sbyte type = Information.Instance[info.TemplateId].Type;
				int num = dataCount[type];
				dataCount[type] = num + 1;
			}
			foreach (InformationTypeItem item2 in ((IEnumerable<InformationTypeItem>)InformationType.Instance))
			{
				this.toggleGroup.Set(item2.TemplateId, this._dataCount[item2.TemplateId]);
			}
		}

		// Token: 0x060094DF RID: 38111 RVA: 0x004558C8 File Offset: 0x00453AC8
		private void OnSkillListChanged()
		{
			this._filteredData.Clear();
			int type = this.toggleGroup.Get();
			this._sortAndFilterController.SetVisibleDropdownMenus(0, new int[]
			{
				type
			});
			Func<InformationSortAndFilterData, bool> filter = this._sortAndFilterController.GenerateFilter();
			Comparison<InformationSortAndFilterData> comparer = this._sortAndFilterController.GenerateComparer(this._filteredData);
			List<InformationSortAndFilterData> allData = new List<InformationSortAndFilterData>();
			foreach (NormalInformation info in this._normalInformationCollection.GetList())
			{
				InformationItem config = Information.Instance[info.TemplateId];
				bool flag = (int)config.Type == type;
				if (flag)
				{
					InformationSortAndFilterData item = new InformationSortAndFilterData
					{
						TemplateId = info.TemplateId,
						Level = info.Level,
						UsedCount = this._normalInformationCollection.GetUsedCount(info),
						UsedCountMax = this._normalInformationCollection.GetUsedCountMax(info)
					};
					allData.Add(item);
					bool flag2 = filter(item);
					if (flag2)
					{
						this._filteredData.Add(item);
					}
				}
			}
			bool flag3 = comparer != null;
			if (flag3)
			{
				this._filteredData.Sort(comparer);
			}
			this._sortAndFilterController.AfterFilter(allData);
			this.noContent.SetActive(this._filteredData.Count == 0);
			this.scroll.SetDataCount(this._filteredData.Count);
			this.briefScroll.SetDataCount(this._filteredData.Count);
			this.Element.ShowAfterRefresh();
			this.localLoadingAnim.SetLoadingState(false);
		}

		// Token: 0x060094E0 RID: 38112 RVA: 0x00455A90 File Offset: 0x00453C90
		private void OnItemRender(int index, GameObject obj)
		{
			obj.GetComponent<InformationCardItem>().Set(this._filteredData[index], true);
		}

		// Token: 0x0400727B RID: 29307
		[SerializeField]
		private InformationTypeToggleGroup toggleGroup;

		// Token: 0x0400727C RID: 29308
		[SerializeField]
		private SortAndFilter sortAndFilter;

		// Token: 0x0400727D RID: 29309
		[SerializeField]
		private InfinityScroll scroll;

		// Token: 0x0400727E RID: 29310
		[SerializeField]
		private InfinityScroll briefScroll;

		// Token: 0x0400727F RID: 29311
		[SerializeField]
		private GameObject noContent;

		// Token: 0x04007280 RID: 29312
		[SerializeField]
		private CToggleGroup switchToggle;

		// Token: 0x04007281 RID: 29313
		[SerializeField]
		private CharacterMenuLocalLoadingAnim localLoadingAnim;

		// Token: 0x04007282 RID: 29314
		private NormalInformationCollection _normalInformationCollection;

		// Token: 0x04007283 RID: 29315
		private InformationSortAndFilterController _sortAndFilterController;

		// Token: 0x04007284 RID: 29316
		private List<InformationSortAndFilterData> _filteredData = new List<InformationSortAndFilterData>();

		// Token: 0x04007285 RID: 29317
		private Dictionary<sbyte, int> _dataCount = new Dictionary<sbyte, int>();
	}
}
