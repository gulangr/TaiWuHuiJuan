using System;
using System.Linq;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter;
using GameData.Domains.Item.Display;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Game.Views.Exchange
{
	// Token: 0x02000A27 RID: 2599
	public class ExchangeContainer : MonoBehaviour
	{
		// Token: 0x06007F6D RID: 32621 RVA: 0x003B58AC File Offset: 0x003B3AAC
		public void SetCurrPageInteractable(bool interactable, int index)
		{
			CToggle toggle = this.currPage.Get(index);
			bool flag = toggle == null;
			if (!flag)
			{
				toggle.interactable = interactable;
				TooltipInvoker tips = toggle.GetComponent<TooltipInvoker>();
				bool flag2 = tips != null;
				if (flag2)
				{
					tips.enabled = !interactable;
				}
			}
		}

		// Token: 0x06007F6E RID: 32622 RVA: 0x003B58F8 File Offset: 0x003B3AF8
		public void AddSwitchToggleListener()
		{
			this.targetToggleGroup.Init(0);
			this.targetToggleGroup.OnActiveIndexChange += this.OnTargetCardModeChange;
			this.selfToggleGroup.Init(0);
			this.selfToggleGroup.OnActiveIndexChange += this.OnSelfCardModeChange;
		}

		// Token: 0x06007F6F RID: 32623 RVA: 0x003B5950 File Offset: 0x003B3B50
		private void OnTargetCardModeChange(int previousIndex, int currentIndex)
		{
			this.targetItemList.SwitchCardModeToggle(previousIndex, currentIndex);
			this.targetExchangeList.SwitchCardModeToggle(previousIndex, currentIndex);
		}

		// Token: 0x06007F70 RID: 32624 RVA: 0x003B596F File Offset: 0x003B3B6F
		private void OnSelfCardModeChange(int previousIndex, int currentIndex)
		{
			this.selfItemList.SwitchCardModeToggle(previousIndex, currentIndex);
			this.selfExchangeList.SwitchCardModeToggle(previousIndex, currentIndex);
		}

		// Token: 0x06007F71 RID: 32625 RVA: 0x003B5990 File Offset: 0x003B3B90
		public void SetSelfFilterEnabled(bool enable)
		{
			bool flag = !enable;
			if (flag)
			{
				this.selfItemList.SortAndFilterController.SetToggleIsOn(this.selfItemList.SortAndFilterController.FilterLines.First<FilterLineBase<ITradeableContent>>().Id, -1);
			}
			foreach (GameObject go in this.selfSortAndFilter)
			{
				bool flag2 = go;
				if (flag2)
				{
					go.SetActive(enable);
				}
			}
		}

		// Token: 0x04006189 RID: 24969
		public CToggleGroup currPage;

		// Token: 0x0400618A RID: 24970
		[CanBeNull]
		public CToggleGroup targetPage;

		// Token: 0x0400618B RID: 24971
		public ExchangeBack exchangeBack;

		// Token: 0x0400618C RID: 24972
		public ExchangeBack exchangeFront;

		// Token: 0x0400618D RID: 24973
		public Game.Components.Avatar.Avatar avatar;

		// Token: 0x0400618E RID: 24974
		public TMP_Text nameTarget;

		// Token: 0x0400618F RID: 24975
		public TMP_Text titleTarget;

		// Token: 0x04006190 RID: 24976
		public TMP_Text titleTaiwu;

		// Token: 0x04006191 RID: 24977
		public TMP_Text targetValue1;

		// Token: 0x04006192 RID: 24978
		public TMP_Text targetValue2;

		// Token: 0x04006193 RID: 24979
		public TMP_Text targetValue3;

		// Token: 0x04006194 RID: 24980
		public TMP_Text selfValue1;

		// Token: 0x04006195 RID: 24981
		public TMP_Text selfValue2;

		// Token: 0x04006196 RID: 24982
		public TMP_Text selfValue3;

		// Token: 0x04006197 RID: 24983
		public TMP_Text secretText;

		// Token: 0x04006198 RID: 24984
		public TMP_Text debtText;

		// Token: 0x04006199 RID: 24985
		public TMP_Text bubbleText;

		// Token: 0x0400619A RID: 24986
		public TMP_Text confirmButtonText;

		// Token: 0x0400619B RID: 24987
		public ItemListScroll targetItemList;

		// Token: 0x0400619C RID: 24988
		public ItemListScroll selfItemList;

		// Token: 0x0400619D RID: 24989
		public ItemListScroll targetExchangeList;

		// Token: 0x0400619E RID: 24990
		public ItemListScroll selfExchangeList;

		// Token: 0x0400619F RID: 24991
		public CToggleGroup targetToggleGroup;

		// Token: 0x040061A0 RID: 24992
		public CToggleGroup selfToggleGroup;

		// Token: 0x040061A1 RID: 24993
		public CButton confirm;

		// Token: 0x040061A2 RID: 24994
		public CButton balance;

		// Token: 0x040061A3 RID: 24995
		public CButton hide;

		// Token: 0x040061A4 RID: 24996
		public CButton avatarBtn;

		// Token: 0x040061A5 RID: 24997
		public CButton btnOpenCharMenu;

		// Token: 0x040061A6 RID: 24998
		[CanBeNull]
		public CButton reset;

		// Token: 0x040061A7 RID: 24999
		[CanBeNull]
		public CButton resetSelf;

		// Token: 0x040061A8 RID: 25000
		[CanBeNull]
		public CButton resetTarget;

		// Token: 0x040061A9 RID: 25001
		[CanBeNull]
		public CButton secret;

		// Token: 0x040061AA RID: 25002
		[CanBeNull]
		public CButton debt;

		// Token: 0x040061AB RID: 25003
		[CanBeNull]
		public CButton putSelfAll;

		// Token: 0x040061AC RID: 25004
		[CanBeNull]
		public CButton putTargetAll;

		// Token: 0x040061AD RID: 25005
		[CanBeNull]
		public CButton selfMultiplyOperate;

		// Token: 0x040061AE RID: 25006
		[CanBeNull]
		public CButton targetMultiplyOperate;

		// Token: 0x040061AF RID: 25007
		public CImage secretImg;

		// Token: 0x040061B0 RID: 25008
		public CImage debtImg;

		// Token: 0x040061B1 RID: 25009
		public Sprite secretIsNone;

		// Token: 0x040061B2 RID: 25010
		public Sprite secretIsSome;

		// Token: 0x040061B3 RID: 25011
		[CanBeNull]
		[Header("地区恩义不可交互")]
		public DisableStyleRoot debtIsNegative;

		// Token: 0x040061B4 RID: 25012
		[CanBeNull]
		[Header("地区恩义不可交互-tips")]
		public TooltipInvoker debtIsNegativeTips;

		// Token: 0x040061B5 RID: 25013
		public RowCellContainer itemIconAndNameCellContainer;

		// Token: 0x040061B6 RID: 25014
		public RowCellContainer singleTextCellContainer;

		// Token: 0x040061B7 RID: 25015
		public CanvasGroup bubble;

		// Token: 0x040061B8 RID: 25016
		public TooltipInvoker confirmDisplayer;

		// Token: 0x040061B9 RID: 25017
		[CanBeNull]
		[Header("身份显示")]
		public GradeComponent grade;

		// Token: 0x040061BA RID: 25018
		[CanBeNull]
		[Header("立场显示")]
		public BehaviorType behavior;

		// Token: 0x040061BB RID: 25019
		[CanBeNull]
		[Header("好感显示")]
		public FavorWithProgressBar favor;

		// Token: 0x040061BC RID: 25020
		[CanBeNull]
		[Header("戒心显示")]
		public AlertnessWithProgressBar alertness;

		// Token: 0x040061BD RID: 25021
		[Header("交换任务，非空，但可能不显示")]
		public ExchangeTaskPanel exchangeTaskPanel;

		// Token: 0x040061BE RID: 25022
		[Header("交换优势进度条，非空，但可能不显示")]
		public ExchangeAdvantageBar exchangeAdvantageBar;

		// Token: 0x040061BF RID: 25023
		[Header("恩义弹窗偏移量，默认(168,0)")]
		public Vector2 debtFollowOffset = new Vector2(168f, 0f);

		// Token: 0x040061C0 RID: 25024
		private bool _cardMode = false;

		// Token: 0x040061C1 RID: 25025
		[SerializeField]
		private GameObject[] selfSortAndFilter;
	}
}
