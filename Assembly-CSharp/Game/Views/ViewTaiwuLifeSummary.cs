using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Common;
using Game.Components.Switch;
using Game.Views.GameLineScroll;
using Game.Views.TaiwuLifeSummary;
using GameData.Domains.Character;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views
{
	// Token: 0x02000713 RID: 1811
	public class ViewTaiwuLifeSummary : UIBase
	{
		// Token: 0x17000A70 RID: 2672
		// (get) Token: 0x060055E2 RID: 21986 RVA: 0x0027C98A File Offset: 0x0027AB8A
		// (set) Token: 0x060055E3 RID: 21987 RVA: 0x0027C992 File Offset: 0x0027AB92
		public int CurrentTaiwuToggleIndex
		{
			get
			{
				return this._currTaiwuToggleIndex;
			}
			set
			{
				this._currTaiwuToggleIndex = value;
				this._currentTaiwuIndex = value - 1;
				this.OnCurrentTaiwuToggleIndexChange();
			}
		}

		// Token: 0x060055E4 RID: 21988 RVA: 0x0027C9AC File Offset: 0x0027ABAC
		private void OnCurrentTaiwuToggleIndexChange()
		{
			this.avatarMask.gameObject.SetActive(this._currentTaiwuIndex == -1);
			this.avatar.gameObject.SetActive(this._currentTaiwuIndex != -1);
			bool flag = !this.taiwuToggleScroll.GetActiveCell(this.CurrentTaiwuToggleIndex);
			if (flag)
			{
				this.taiwuToggleScroll.ScrollTo(this.CurrentTaiwuToggleIndex, 0.3f);
			}
			List<int> achievements = (this._currentTaiwuIndex != -1) ? this._displayData.TotalTaiwuLifeSummaries[this._currentTaiwuIndex].GetAchievements() : this.GetTotalAchievements();
			this._curAchievements.ClearAndAddRange(achievements);
			this.achievementScroll.SetDataCount(this._curAchievements.Count);
			this.achievementHolderTitle.text = this._curAchievements.Count.ToString();
			this.taiwuToggleGroupLeftBtn.interactable = (this.CurrentTaiwuToggleIndex > 0);
			this.taiwuToggleGroupRightBtn.interactable = (this._currentTaiwuIndex < this._displayData.TotalTaiwuLifeSummaries.Count - 1);
			this.taiwuToggleScroll.ReRender();
			this.summaryTypeToggleGroup.Set(this.summaryTypeToggleGroup.GetActiveIndex(), true);
			this.achievementHolderTitle.text = LanguageKey.LK_TaiwuSummary_Format.TrFormat(LanguageKey.LK_TaiwuSummary_Achievement.Tr(), string.Format("{0}/{1}", this._curAchievements.Count, AchievementInfo.Instance.Count));
			bool flag2 = this._currentTaiwuIndex >= 0;
			if (flag2)
			{
				bool flag3 = this._currentTaiwuIndex == this._displayData.TotalTaiwuDisplayDatas.Count - 1;
				if (flag3)
				{
					this.avatar.Refresh(this._displayData.TotalTaiwuDisplayDatas[this._currentTaiwuIndex], false);
				}
				else
				{
					Game.Components.Avatar.Avatar avatar = this.avatar;
					AbridgedCharacter abridgedCharacter = this._displayData.TotalTaiwuLifeSummaries[this._currentTaiwuIndex].AbridgedCharacter;
					avatar.Refresh(((abridgedCharacter != null) ? abridgedCharacter.GenerateAvatarRelatedData() : null) ?? this._displayData.TotalTaiwuDisplayDatas[this._currentTaiwuIndex].AvatarRelatedData);
				}
			}
		}

		// Token: 0x060055E5 RID: 21989 RVA: 0x0027CBEC File Offset: 0x0027ADEC
		private void Awake()
		{
			this.summaryTypeLeftBtn.ClearAndAddListener(delegate
			{
				int index = this.summaryTypeToggleGroup.GetActiveIndex() - 1;
				this.summaryTypeToggleGroup.Set(index, false);
			});
			this.summaryTypeRightBtn.ClearAndAddListener(delegate
			{
				int index = this.summaryTypeToggleGroup.GetActiveIndex() + 1;
				this.summaryTypeToggleGroup.Set(index, false);
			});
			this.summaryTypeToggleGroup.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.summaryTypeToggleGroup, 1, null);
			this.summaryTypeToggleGroup.OnActiveIndexChange += this.OnSummaryTypeToggleGroupActiveIndexChange;
			this.achievementScroll.OnItemRender += this.OnAchievementScrollItemRender;
			this.scrollBtn.ClearAndAddListener(delegate
			{
				UIElement.GameLineScroll.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("targetScrollIndex", 2));
				UIManager.Instance.MaskUI(UIElement.GameLineScroll);
			});
			this.taiwuToggleGroupLeftBtn.ClearAndAddListener(delegate
			{
				this.CurrentTaiwuToggleIndex--;
			});
			this.taiwuToggleGroupRightBtn.ClearAndAddListener(delegate
			{
				this.CurrentTaiwuToggleIndex++;
				bool flag = !this.taiwuToggleScroll.GetActiveCell(this.CurrentTaiwuToggleIndex);
				if (flag)
				{
					this.taiwuToggleScroll.ScrollTo(this.CurrentTaiwuToggleIndex, 0.3f);
				}
			});
			this.taiwuToggleScroll.OnItemRender += this.OnTaiwuToggleScrollItemRender;
			PoolManager.SetSrcObject("SmallItemHolder", this.smallItemHolderTemplate.gameObject);
			PoolManager.SetSrcObject("NormalItemHolder", this.normalItemHolderTemplate.gameObject);
			PoolManager.SetSrcObject("BigItemHolder", this.bigItemHolderTemplate.gameObject);
		}

		// Token: 0x060055E6 RID: 21990 RVA: 0x0027CD30 File Offset: 0x0027AF30
		private void OnAchievementScrollItemRender(int index, GameObject obj)
		{
			CImage image = obj.GetComponentInChildren<CImage>();
			TextMeshProUGUI[] labels = obj.GetComponentsInChildren<TextMeshProUGUI>();
			labels[0].text = AchievementInfo.Instance[this._curAchievements[index]].Name;
			labels[1].text = AchievementInfo.Instance[this._curAchievements[index]].Desc;
			image.SetSprite(AchievementInfo.Instance[this._curAchievements[index]].IconSmall, false, null);
		}

		// Token: 0x060055E7 RID: 21991 RVA: 0x0027CDB8 File Offset: 0x0027AFB8
		private void OnDestroy()
		{
			PoolManager.RemoveData("SmallItemHolder");
			PoolManager.RemoveData("NormalItemHolder");
			PoolManager.RemoveData("BigItemHolder");
		}

		// Token: 0x060055E8 RID: 21992 RVA: 0x0027CDDC File Offset: 0x0027AFDC
		private void OnTaiwuToggleScrollItemRender(int index, GameObject obj)
		{
			SwitchToggleSmall toggle = obj.GetComponent<SwitchToggleSmall>();
			toggle.SetIsOnWithoutNotify(this.CurrentTaiwuToggleIndex == index);
			toggle.onValueChanged.ResetListener(delegate(bool isOn)
			{
				if (isOn)
				{
					this.CurrentTaiwuToggleIndex = index;
				}
			});
			toggle.GetComponent<ToggleStyle>().SetLabelText((index == 0) ? LanguageKey.LK_TaiwuSummary_Index_Total.Tr() : LanguageKey.LK_TaiwuSummary_Index.TrFormat(TaiwuScrollItem.GetTaiwuOrdinalDisplayText(index), NameCenter.GetNameByDisplayData(this._displayData.TotalTaiwuDisplayDatas[index - 1], true, false)));
		}

		// Token: 0x060055E9 RID: 21993 RVA: 0x0027CE88 File Offset: 0x0027B088
		private void OnSummaryTypeToggleGroupActiveIndexChange(int newIndex, int preIndex)
		{
			RectTransform rect = (RectTransform)this.summaryTypeToggleGroup.Get(newIndex).transform;
			this.summaryTypeScrollRect.ScrollTo(rect, 0.3f);
			this.summaryTypeLeftBtn.interactable = (newIndex > 0);
			this.summaryTypeRightBtn.interactable = (newIndex < this.summaryTypeToggleGroup.GetAll().Count - 1);
			this.SetSummaryHolderItemRoot(newIndex);
		}

		// Token: 0x060055EA RID: 21994 RVA: 0x0027CEF8 File Offset: 0x0027B0F8
		public override void OnInit(ArgumentBox argsBox)
		{
			this.NeedWaitData = true;
			this.NeedDataListenerId = true;
			this.RequestData();
		}

		// Token: 0x060055EB RID: 21995 RVA: 0x0027CF10 File Offset: 0x0027B110
		private void RequestData()
		{
			TaiwuDomainMethod.AsyncCall.GetTaiwuLifeSummaryDisplayData(this, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._displayData);
				this.Element.ShowAfterRefresh();
				this.UpdateData();
				this.Refresh();
			});
		}

		// Token: 0x060055EC RID: 21996 RVA: 0x0027CF26 File Offset: 0x0027B126
		private void UpdateData()
		{
			this.GetTotalSummaryValues();
		}

		// Token: 0x060055ED RID: 21997 RVA: 0x0027CF30 File Offset: 0x0027B130
		private void Refresh()
		{
			this.taiwuToggleScroll.SetDataCount(this._displayData.TotalTaiwuLifeSummaries.Count + 1);
			this.CurrentTaiwuToggleIndex = 0;
			this.summaryTypeToggleGroup.Set(0, true);
		}

		// Token: 0x060055EE RID: 21998 RVA: 0x0027CF68 File Offset: 0x0027B168
		protected override void OnClick(Transform btn)
		{
			string btnName = btn.name;
			string text = btnName;
			string a = text;
			if (a == "CloseBtn")
			{
				this.QuickHide();
			}
		}

		// Token: 0x060055EF RID: 21999 RVA: 0x0027CF9C File Offset: 0x0027B19C
		private void Update()
		{
			bool flag = TabSwitchCommandKit.PrevTabLevel1.Check(this.Element, false, false, false, true, false) && this.CurrentTaiwuToggleIndex > 0;
			if (flag)
			{
				int currentTaiwuToggleIndex = this.CurrentTaiwuToggleIndex;
				this.CurrentTaiwuToggleIndex = currentTaiwuToggleIndex - 1;
			}
			else
			{
				bool flag2 = TabSwitchCommandKit.NextTabLevel1.Check(this.Element, false, false, false, true, false) && this._currentTaiwuIndex < this._displayData.TotalTaiwuLifeSummaries.Count - 1;
				if (flag2)
				{
					int currentTaiwuToggleIndex = this.CurrentTaiwuToggleIndex;
					this.CurrentTaiwuToggleIndex = currentTaiwuToggleIndex + 1;
				}
			}
		}

		// Token: 0x060055F0 RID: 22000 RVA: 0x0027D034 File Offset: 0x0027B234
		private void GetTotalSummaryValues()
		{
			this._totalSummaryValues.Clear();
			using (IEnumerator<TaiwuLifeSummaryTypeItem> enumerator = ((IEnumerable<TaiwuLifeSummaryTypeItem>)TaiwuLifeSummaryType.Instance).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TaiwuLifeSummaryTypeItem item = enumerator.Current;
					Dictionary<int, int> totalSummaryValues = this._totalSummaryValues;
					int templateId = item.TemplateId;
					List<TaiwuLifeSummary> totalTaiwuLifeSummaries = this._displayData.TotalTaiwuLifeSummaries;
					totalSummaryValues[templateId] = ((totalTaiwuLifeSummaries != null) ? totalTaiwuLifeSummaries.Sum((TaiwuLifeSummary x) => x.Get(item.TemplateId)) : 0);
				}
			}
		}

		// Token: 0x060055F1 RID: 22001 RVA: 0x0027D0D0 File Offset: 0x0027B2D0
		private List<int> GetTotalAchievements()
		{
			List<TaiwuLifeSummary> totalTaiwuLifeSummaries = this._displayData.TotalTaiwuLifeSummaries;
			List<int> result;
			if (totalTaiwuLifeSummaries == null)
			{
				result = null;
			}
			else
			{
				result = totalTaiwuLifeSummaries.SelectMany((TaiwuLifeSummary x) => x.GetAchievements()).Distinct<int>().ToList<int>();
			}
			return result;
		}

		// Token: 0x060055F2 RID: 22002 RVA: 0x0027D124 File Offset: 0x0027B324
		private void SetSummaryHolderItemRoot(int summaryType)
		{
			float cellWidth = ((RectTransform)this.smallItemHolderTemplate.transform).rect.width;
			float cellHeight = ((RectTransform)this.smallItemHolderTemplate.transform).rect.height;
			for (int i = this.summaryHolderItemRoot.childCount - 1; i >= 0; i--)
			{
				TaiwuSummaryHolderItem child = this.summaryHolderItemRoot.GetChild(i).GetComponent<TaiwuSummaryHolderItem>();
				bool flag = child != null;
				if (flag)
				{
					this.ReturnHolderItem(child);
				}
			}
			this._occupied.Clear();
			int maxRow = 0;
			List<TaiwuLifeSummaryGroupItem> summaryGroups = (from x in TaiwuLifeSummaryGroup.Instance
			where (int)x.Type == summaryType
			select x).ToPoolList<TaiwuLifeSummaryGroupItem>();
			foreach (TaiwuLifeSummaryGroupItem configItem in summaryGroups)
			{
				ETaiwuSummaryHolderItemType itemType = (ETaiwuSummaryHolderItemType)configItem.Size;
				ValueTuple<int, int> itemSize = ViewTaiwuLifeSummary.GetItemSize(itemType);
				int itemCols = itemSize.Item1;
				int itemRows = itemSize.Item2;
				bool placed = false;
				int row = 0;
				while (!placed)
				{
					for (int col = 0; col <= 4 - itemCols; col++)
					{
						bool flag2 = !ViewTaiwuLifeSummary.CanPlace(col, row, itemCols, itemRows, this._occupied);
						if (!flag2)
						{
							TaiwuSummaryHolderItem holderItem = this.GetHolderItem(itemType);
							holderItem.transform.SetParent(this.summaryHolderItemRoot, false);
							bool flag3 = this._currentTaiwuIndex >= 0;
							if (flag3)
							{
								holderItem.Set((from x in configItem.Items
								select TaiwuLifeSummaryType.Instance[x]).ToList<TaiwuLifeSummaryTypeItem>(), this._displayData.TotalTaiwuLifeSummaries[this._currentTaiwuIndex], "");
							}
							else
							{
								holderItem.Set((from x in configItem.Items
								select TaiwuLifeSummaryType.Instance[x]).ToList<TaiwuLifeSummaryTypeItem>(), this._totalSummaryValues, "");
							}
							RectTransform rt = (RectTransform)holderItem.transform;
							rt.anchoredPosition = new Vector2((float)col * (cellWidth + 10f), (float)(-(float)row) * (cellHeight + 10f));
							for (int c = col; c < col + itemCols; c++)
							{
								for (int r = row; r < row + itemRows; r++)
								{
									this._occupied.Add(new ValueTuple<int, int>(c, r));
								}
							}
							maxRow = Math.Max(maxRow, row + itemRows);
							placed = true;
							break;
						}
					}
					row++;
				}
			}
			float totalHeight = (float)maxRow * cellHeight + (float)((maxRow - 1) * 10);
			this.summaryHolderItemRoot.sizeDelta = new Vector2(this.summaryHolderItemRoot.sizeDelta.x, totalHeight);
			this.summaryHolderItemRoot.anchoredPosition = new Vector2(this.summaryHolderItemRoot.anchoredPosition.x, 0f);
			EasyPool.Free<List<TaiwuLifeSummaryGroupItem>>(summaryGroups);
		}

		// Token: 0x060055F3 RID: 22003 RVA: 0x0027D48C File Offset: 0x0027B68C
		[return: TupleElementNames(new string[]
		{
			"cols",
			"rows"
		})]
		private static ValueTuple<int, int> GetItemSize(ETaiwuSummaryHolderItemType itemType)
		{
			if (!true)
			{
			}
			ValueTuple<int, int> result;
			switch (itemType)
			{
			case ETaiwuSummaryHolderItemType.Small:
				result = new ValueTuple<int, int>(1, 1);
				break;
			case ETaiwuSummaryHolderItemType.Normal:
				result = new ValueTuple<int, int>(2, 1);
				break;
			case ETaiwuSummaryHolderItemType.Big:
				result = new ValueTuple<int, int>(2, 2);
				break;
			default:
				result = new ValueTuple<int, int>(1, 1);
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x060055F4 RID: 22004 RVA: 0x0027D4E0 File Offset: 0x0027B6E0
		private static bool CanPlace(int col, int row, int itemCols, int itemRows, [TupleElementNames(new string[]
		{
			"col",
			"row"
		})] HashSet<ValueTuple<int, int>> occupied)
		{
			for (int c = col; c < col + itemCols; c++)
			{
				for (int r = row; r < row + itemRows; r++)
				{
					bool flag = occupied.Contains(new ValueTuple<int, int>(c, r));
					if (flag)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060055F5 RID: 22005 RVA: 0x0027D534 File Offset: 0x0027B734
		private TaiwuSummaryHolderItem GetHolderItem(ETaiwuSummaryHolderItemType itemType)
		{
			if (!true)
			{
			}
			TaiwuSummaryHolderItem @object;
			switch (itemType)
			{
			case ETaiwuSummaryHolderItemType.Small:
				@object = PoolManager.GetObject<TaiwuSummaryHolderItem>("SmallItemHolder");
				break;
			case ETaiwuSummaryHolderItemType.Normal:
				@object = PoolManager.GetObject<TaiwuSummaryHolderItem>("NormalItemHolder");
				break;
			case ETaiwuSummaryHolderItemType.Big:
				@object = PoolManager.GetObject<TaiwuSummaryHolderItem>("BigItemHolder");
				break;
			default:
				if (!true)
				{
				}
				<PrivateImplementationDetails>.ThrowSwitchExpressionException(itemType);
				break;
			}
			if (!true)
			{
			}
			return @object;
		}

		// Token: 0x060055F6 RID: 22006 RVA: 0x0027D59C File Offset: 0x0027B79C
		private void ReturnHolderItem(TaiwuSummaryHolderItem item)
		{
			ETaiwuSummaryHolderItemType itemType = item.ItemType;
			if (!true)
			{
			}
			string text;
			switch (itemType)
			{
			case ETaiwuSummaryHolderItemType.Small:
				text = "SmallItemHolder";
				break;
			case ETaiwuSummaryHolderItemType.Normal:
				text = "NormalItemHolder";
				break;
			case ETaiwuSummaryHolderItemType.Big:
				text = "BigItemHolder";
				break;
			default:
				if (!true)
				{
				}
				<PrivateImplementationDetails>.ThrowSwitchExpressionException(itemType);
				break;
			}
			if (!true)
			{
			}
			string key = text;
			PoolManager.Destroy(key, item.gameObject);
		}

		// Token: 0x04003ACD RID: 15053
		[SerializeField]
		private CImage avatarMask;

		// Token: 0x04003ACE RID: 15054
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04003ACF RID: 15055
		[SerializeField]
		private TextMeshProUGUI achievementHolderTitle;

		// Token: 0x04003AD0 RID: 15056
		[SerializeField]
		private InfinityScroll achievementScroll;

		// Token: 0x04003AD1 RID: 15057
		[SerializeField]
		private CButton summaryTypeLeftBtn;

		// Token: 0x04003AD2 RID: 15058
		[SerializeField]
		private CButton summaryTypeRightBtn;

		// Token: 0x04003AD3 RID: 15059
		[SerializeField]
		private CToggleGroup summaryTypeToggleGroup;

		// Token: 0x04003AD4 RID: 15060
		[SerializeField]
		private CScrollRect summaryTypeScrollRect;

		// Token: 0x04003AD5 RID: 15061
		[SerializeField]
		private RectTransform summaryHolderItemRoot;

		// Token: 0x04003AD6 RID: 15062
		[SerializeField]
		private CButton scrollBtn;

		// Token: 0x04003AD7 RID: 15063
		[SerializeField]
		private CButton taiwuToggleGroupLeftBtn;

		// Token: 0x04003AD8 RID: 15064
		[SerializeField]
		private CButton taiwuToggleGroupRightBtn;

		// Token: 0x04003AD9 RID: 15065
		[SerializeField]
		private InfinityScroll taiwuToggleScroll;

		// Token: 0x04003ADA RID: 15066
		[SerializeField]
		private TaiwuSummaryHolderItem smallItemHolderTemplate;

		// Token: 0x04003ADB RID: 15067
		[SerializeField]
		private TaiwuSummaryHolderItem normalItemHolderTemplate;

		// Token: 0x04003ADC RID: 15068
		[SerializeField]
		private TaiwuSummaryHolderItem bigItemHolderTemplate;

		// Token: 0x04003ADD RID: 15069
		private const string SmallItemHolderTemplateKey = "SmallItemHolder";

		// Token: 0x04003ADE RID: 15070
		private const string NormalItemHolderTemplateKey = "NormalItemHolder";

		// Token: 0x04003ADF RID: 15071
		private const string BigItemHolderTemplateKey = "BigItemHolder";

		// Token: 0x04003AE0 RID: 15072
		private TaiwuLifeSummaryDisplayData _displayData = new TaiwuLifeSummaryDisplayData();

		// Token: 0x04003AE1 RID: 15073
		private readonly List<int> _curAchievements = new List<int>();

		// Token: 0x04003AE2 RID: 15074
		private readonly Dictionary<int, int> _totalSummaryValues = new Dictionary<int, int>();

		// Token: 0x04003AE3 RID: 15075
		[TupleElementNames(new string[]
		{
			"col",
			"row"
		})]
		private readonly HashSet<ValueTuple<int, int>> _occupied = new HashSet<ValueTuple<int, int>>();

		// Token: 0x04003AE4 RID: 15076
		private int _currentTaiwuIndex;

		// Token: 0x04003AE5 RID: 15077
		private int _currTaiwuToggleIndex;
	}
}
