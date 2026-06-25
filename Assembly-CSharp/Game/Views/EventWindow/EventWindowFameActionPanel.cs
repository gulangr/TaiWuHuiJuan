using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character;
using GameData.Domains.Taiwu;
using TMPro;
using UnityEngine;

namespace Game.Views.EventWindow
{
	// Token: 0x02000A3B RID: 2619
	public class EventWindowFameActionPanel : MonoBehaviour
	{
		// Token: 0x0600815F RID: 33119 RVA: 0x003C324E File Offset: 0x003C144E
		private void Awake()
		{
			this.fameScroll.OnItemRender += this.OnFameActionItemRender;
			this.btnSelectAll.ClearAndAddListener(delegate
			{
				bool flag = this.selectedFameActionIdList.Count != this._canSelectedFameActionIdList.Count;
				if (flag)
				{
					this.selectedFameActionIdList.Clear();
					this.selectedFameActionIdList.AddRange(this._canSelectedFameActionIdList);
				}
				else
				{
					this.selectedFameActionIdList.Clear();
				}
				this.fameScroll.ReRender();
				this.SetAuthorityCostAndButtons();
			});
		}

		// Token: 0x06008160 RID: 33120 RVA: 0x003C3284 File Offset: 0x003C1484
		public void Refresh(List<FameActionRecord> fameActionRecords, Action refreshConfirmButtonTips, CButton confitmButton)
		{
			this._fameActionRecords = fameActionRecords;
			this._refreshConfirmButtonTips = refreshConfirmButtonTips;
			this._confirmButton = confitmButton;
			this._fameActionSortIndex = 0;
			this._fameExistTimeSortIndex = 0;
			this.selectedFameActionIdList.Clear();
			this._canSelectedFameActionIdList.Clear();
			this._fameActionIdToIndex.Clear();
			bool flag = this._fameActionRecords != null;
			if (flag)
			{
				foreach (FameActionRecord item in this._fameActionRecords)
				{
					this._canSelectedFameActionIdList.Add(item.Id);
					this._fameActionIdToIndex.Add(item.Id, this._canSelectedFameActionIdList.Count - 1);
				}
			}
			this.fameScroll.SetDataCount(this._canSelectedFameActionIdList.Count);
			this.SetAuthorityCostAndButtons();
			this.UpdateFameActionSort(false);
		}

		// Token: 0x06008161 RID: 33121 RVA: 0x003C3380 File Offset: 0x003C1580
		private void OnFameActionItemRender(int index, GameObject go)
		{
			EventWindowFameRoleTemplate fame = go.GetComponent<EventWindowFameRoleTemplate>();
			FameActionRecord fameAction = this._fameActionRecords[index];
			FameActionItem config = FameAction.Instance[fameAction.Id];
			string color = (config.Fame > 0) ? "brightblue" : "brightred";
			int currDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
			fame.txtName.text = config.Name.SetColor(color);
			string fameActionText = (fameAction.Value > 0) ? string.Format("+{0}", fameAction.Value) : fameAction.Value.ToString();
			fame.txtFameInfluence.text = fameActionText.SetColor(color);
			fame.txtDurationFrom.text = (fameAction.EndDate - currDate).ToString().SetColor("pinkyellow");
			fame.txtDurationTom.text = Math.Max(fameAction.EndDate - currDate - (int)config.ReductionTime, 0).ToString().SetColor("orange");
			short fameActionId = fameAction.Id;
			fame.SetSelected(this.selectedFameActionIdList.Contains(fameActionId));
			fame.ActionOnClick = delegate()
			{
				bool flag = this.selectedFameActionIdList.Contains(fameActionId);
				if (flag)
				{
					this.selectedFameActionIdList.Remove(fameActionId);
					fame.SetSelected(false);
				}
				else
				{
					this.selectedFameActionIdList.Add(fameActionId);
					fame.SetSelected(true);
				}
				Action refreshConfirmButtonTips = this._refreshConfirmButtonTips;
				if (refreshConfirmButtonTips != null)
				{
					refreshConfirmButtonTips();
				}
				this.SetAuthorityCostAndButtons();
			};
		}

		// Token: 0x06008162 RID: 33122 RVA: 0x003C34F4 File Offset: 0x003C16F4
		private void UpdateFameActionSort(bool isFameAction = false)
		{
			EventWindowFameActionPanel.<>c__DisplayClass24_0 CS$<>8__locals1 = new EventWindowFameActionPanel.<>c__DisplayClass24_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.isFameAction = isFameAction;
			EventWindowFameActionPanel.<UpdateFameActionSort>g__SetCheckMark|24_2(this.fameActionSortImage, this.fameActionArrow, this._fameActionSortIndex);
			EventWindowFameActionPanel.<UpdateFameActionSort>g__SetCheckMark|24_2(this.existTimeBSortImage, this.existTimeArrow, this._fameExistTimeSortIndex);
			CS$<>8__locals1.currDate = SingletonObject.getInstance<BasicGameData>().CurrDate;
			List<FameActionRecord> fameActionRecords = this._fameActionRecords;
			bool flag = fameActionRecords == null || fameActionRecords.Count == 0;
			if (!flag)
			{
				bool flag2 = this._fameActionSortIndex != 0 || this._fameExistTimeSortIndex != 0;
				if (flag2)
				{
					bool flag3 = this._fameActionSortIndex > 1 || this._fameExistTimeSortIndex > 1;
					if (flag3)
					{
						fameActionRecords.Sort(new Comparison<FameActionRecord>(CS$<>8__locals1.<UpdateFameActionSort>g__comparison|3));
					}
					else
					{
						fameActionRecords.Sort((FameActionRecord a, FameActionRecord b) => -base.<UpdateFameActionSort>g__comparison|3(a, b));
					}
				}
				else
				{
					fameActionRecords.Sort((FameActionRecord a, FameActionRecord b) => CS$<>8__locals1.<>4__this._fameActionIdToIndex[a.Id].CompareTo(CS$<>8__locals1.<>4__this._fameActionIdToIndex[b.Id]));
				}
				this.fameScroll.ReRender();
			}
		}

		// Token: 0x06008163 RID: 33123 RVA: 0x003C35FC File Offset: 0x003C17FC
		private int GetAuthorityCost(int index)
		{
			FameActionRecord fameAction = this._fameActionRecords[index];
			FameActionItem config = FameAction.Instance[fameAction.Id];
			return (int)(Math.Abs(fameAction.Value) * config.ReductionTime * 20);
		}

		// Token: 0x06008164 RID: 33124 RVA: 0x003C3644 File Offset: 0x003C1844
		private void SetAuthorityCostAndButtons()
		{
			int authorityCost = 0;
			for (int i = 0; i < this._canSelectedFameActionIdList.Count; i++)
			{
				bool flag = this.selectedFameActionIdList.Contains(this._canSelectedFameActionIdList[i]);
				if (flag)
				{
					authorityCost += this.GetAuthorityCost(i);
				}
			}
			this.txtAuthorityCost.text = authorityCost.ToString().SetColor("pinkyellow");
			int haveValue = SingletonObject.getInstance<BuildingModel>().GetResourceCount(ItemSourceType.Resources, 7);
			this.txtAuthorityOwn.text = CommonUtils.GetDisplayStringForNum(haveValue, 100000).SetColor((haveValue < authorityCost) ? "brightred" : "brightblue");
			this._confirmButton.interactable = (this.selectedFameActionIdList.Count > 0 && authorityCost < haveValue);
			this.checkMark.gameObject.SetActive(this.selectedFameActionIdList.Count == this._canSelectedFameActionIdList.Count);
		}

		// Token: 0x06008167 RID: 33127 RVA: 0x003C37E0 File Offset: 0x003C19E0
		[CompilerGenerated]
		internal static void <UpdateFameActionSort>g__SetCheckMark|24_2(CImage checkMark, RectTransform arrow, sbyte index)
		{
			checkMark.SetAlpha((float)((index > 0) ? 1 : 0));
			arrow.gameObject.SetActive(index > 0);
			arrow.localRotation = SortFilter.GetArrowRotation(index < 2);
			arrow.anchoredPosition = SortFilter.GetArrowAnchoredPos(index < 2);
		}

		// Token: 0x040062BC RID: 25276
		[SerializeField]
		private TextMeshProUGUI txtTargetFame;

		// Token: 0x040062BD RID: 25277
		[SerializeField]
		private TextMeshProUGUI txtOwnFame;

		// Token: 0x040062BE RID: 25278
		[SerializeField]
		private CImage checkMark;

		// Token: 0x040062BF RID: 25279
		[SerializeField]
		private CButton btnSelectAll;

		// Token: 0x040062C0 RID: 25280
		[SerializeField]
		private InfinityScroll fameScroll;

		// Token: 0x040062C1 RID: 25281
		[SerializeField]
		private TextMeshProUGUI txtAuthorityCost;

		// Token: 0x040062C2 RID: 25282
		[SerializeField]
		private TextMeshProUGUI txtAuthorityOwn;

		// Token: 0x040062C3 RID: 25283
		[Header("排序")]
		[SerializeField]
		private CButton fameActionButton;

		// Token: 0x040062C4 RID: 25284
		[SerializeField]
		private CImage fameActionSortImage;

		// Token: 0x040062C5 RID: 25285
		[SerializeField]
		private RectTransform fameActionArrow;

		// Token: 0x040062C6 RID: 25286
		[SerializeField]
		private CButton existTimeButton;

		// Token: 0x040062C7 RID: 25287
		[SerializeField]
		private CImage existTimeBSortImage;

		// Token: 0x040062C8 RID: 25288
		[SerializeField]
		private RectTransform existTimeArrow;

		// Token: 0x040062C9 RID: 25289
		private sbyte _fameActionSortIndex = 0;

		// Token: 0x040062CA RID: 25290
		private sbyte _fameExistTimeSortIndex = 0;

		// Token: 0x040062CB RID: 25291
		public readonly List<short> selectedFameActionIdList = new List<short>();

		// Token: 0x040062CC RID: 25292
		private readonly List<short> _canSelectedFameActionIdList = new List<short>();

		// Token: 0x040062CD RID: 25293
		private readonly Dictionary<short, int> _fameActionIdToIndex = new Dictionary<short, int>();

		// Token: 0x040062CE RID: 25294
		private List<FameActionRecord> _fameActionRecords;

		// Token: 0x040062CF RID: 25295
		private Action _refreshConfirmButtonTips;

		// Token: 0x040062D0 RID: 25296
		private CButton _confirmButton;
	}
}
