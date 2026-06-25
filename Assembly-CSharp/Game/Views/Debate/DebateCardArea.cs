using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Taiwu.Debate;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.Debate
{
	// Token: 0x02000A9A RID: 2714
	public class DebateCardArea : MonoBehaviour
	{
		// Token: 0x17000E90 RID: 3728
		// (get) Token: 0x060084C6 RID: 33990 RVA: 0x003DBA3E File Offset: 0x003D9C3E
		private LifeSkillCombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<LifeSkillCombatModel>();
			}
		}

		// Token: 0x17000E91 RID: 3729
		// (get) Token: 0x060084C7 RID: 33991 RVA: 0x003DBA45 File Offset: 0x003D9C45
		public List<DebateCardItem> CardList
		{
			get
			{
				return this._cardList;
			}
		}

		// Token: 0x17000E92 RID: 3730
		// (get) Token: 0x060084C8 RID: 33992 RVA: 0x003DBA4D File Offset: 0x003D9C4D
		public List<DebateCardItem> EnemyCardList
		{
			get
			{
				return this._enemyCardList;
			}
		}

		// Token: 0x060084C9 RID: 33993 RVA: 0x003DBA55 File Offset: 0x003D9C55
		private void Awake()
		{
			this.InitCardList();
			this.InitCardOwnedCardGroupTip(this.selfCardGroupLayout);
			this.InitCardOwnedCardGroupTip(this.enemyCardGroupLayout);
		}

		// Token: 0x060084CA RID: 33994 RVA: 0x003DBA7C File Offset: 0x003D9C7C
		private void OnEnable()
		{
			this.selfCardCanvasGroup.transform.localScale = Vector3.one;
			this.selfCardCanvasGroup.alpha = 1f;
			this.enemyCardCanvasGroup.transform.localScale = Vector3.zero;
			this.enemyCardCanvasGroup.alpha = 0f;
			this.selfCardCanvasGroup.gameObject.SetActive(true);
			this.enemyCardCanvasGroup.gameObject.SetActive(true);
		}

		// Token: 0x060084CB RID: 33995 RVA: 0x003DBAFC File Offset: 0x003D9CFC
		private void OnDisable()
		{
			base.StopAllCoroutines();
			this.ClearAllCard();
		}

		// Token: 0x060084CC RID: 33996 RVA: 0x003DBB0D File Offset: 0x003D9D0D
		private void SetCardHide(DebateCardItem cardItem)
		{
			cardItem.SetVisible(false, true);
		}

		// Token: 0x060084CD RID: 33997 RVA: 0x003DBB1C File Offset: 0x003D9D1C
		public void UnselectCard(bool refreshStrategy = true)
		{
			bool flag = null == ViewDebate.FocusingCardItem;
			if (!flag)
			{
				ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("refreshStrategy", refreshStrategy);
				GEvent.OnEvent(UiEvents.OnLifeSkillCombatExitCardFocusMode, args);
				ViewDebate.FocusingCardItem.SetSelect(false, true);
				ViewDebate.FocusingCardItem = null;
			}
		}

		// Token: 0x060084CE RID: 33998 RVA: 0x003DBB74 File Offset: 0x003D9D74
		public void SelectCard(DebateCardItem cardItem)
		{
			bool flag = this.Model.Pause && !this.Model.IsAuto && this.Model.IsTaiwuRound;
			if (!flag)
			{
				bool flag2 = ViewDebate.FocusingCardItem;
				if (flag2)
				{
					bool flag3 = ViewDebate.FocusingCardItem == cardItem;
					if (flag3)
					{
						this.UnselectCard(true);
						return;
					}
					this.UnselectCard(true);
				}
				bool flag4 = !cardItem.Visible || (!cardItem.CanSelect && !this.Model.IsAuto);
				if (!flag4)
				{
					ViewDebate.FocusingCardItem = cardItem;
					cardItem.SetSelect(true, this.selectedCardHasScale);
					GEvent.OnEvent(UiEvents.OnLifeSkillCombatEnterCardFocusMode, null);
				}
			}
		}

		// Token: 0x060084CF RID: 33999 RVA: 0x003DBC38 File Offset: 0x003D9E38
		private void SortCardList(bool isSelf)
		{
			List<DebateCardItem> cardList = isSelf ? this._cardList : this._enemyCardList;
			cardList.Sort(delegate(DebateCardItem left, DebateCardItem right)
			{
				bool flag = left.Visible != right.Visible;
				int result;
				if (flag)
				{
					result = (left.Visible ? 1 : -1);
				}
				else
				{
					result = (int)(left.CardConfig.TemplateId - right.CardConfig.TemplateId);
				}
				return result;
			});
		}

		// Token: 0x060084D0 RID: 34000 RVA: 0x003DBC80 File Offset: 0x003D9E80
		private void LayoutCard(GridLayoutGroup gridLayoutGroup, List<DebateCardItem> cardList)
		{
			float totalWidth = gridLayoutGroup.GetComponent<RectTransform>().rect.width;
			int visibleCount = cardList.Count((DebateCardItem c) => c.Visible);
			float spacingX = (totalWidth - gridLayoutGroup.cellSize.x * (float)visibleCount) / (float)(visibleCount - 1);
			gridLayoutGroup.spacing = Vector2.zero.SetX(Mathf.Min(spacingX, 0f));
		}

		// Token: 0x060084D1 RID: 34001 RVA: 0x003DBCFC File Offset: 0x003D9EFC
		private void ClearAllCard()
		{
			this._cardList.ForEach(delegate(DebateCardItem e)
			{
				e.SetVisible(false, true);
			});
			this._enemyCardList.ForEach(delegate(DebateCardItem e)
			{
				e.SetVisible(false, true);
			});
			ViewDebate.FocusingCardItem = null;
		}

		// Token: 0x060084D2 RID: 34002 RVA: 0x003DBD68 File Offset: 0x003D9F68
		public List<short> GetSelectedCards()
		{
			List<short> result = new List<short>();
			foreach (DebateCardItem cardItem in this._cardList)
			{
				bool selected = cardItem.Selected;
				if (selected)
				{
					result.Add(cardItem.CardConfig.TemplateId);
				}
			}
			return result;
		}

		// Token: 0x060084D3 RID: 34003 RVA: 0x003DBDE4 File Offset: 0x003D9FE4
		private void InitCardList()
		{
			this._cardList.Clear();
			this._cardList.AddRange(this.selfCardCanvasGroup.GetComponentsInChildren<DebateCardItem>(true));
			this._enemyCardList.Clear();
			this._enemyCardList.AddRange(this.enemyCardCanvasGroup.GetComponentsInChildren<DebateCardItem>(true));
			int i = 0;
			int max = this._cardList.Count;
			while (i < max)
			{
				DebateCardItem cardItem = this._cardList[i];
				cardItem.CardView.SetOnClick(delegate
				{
					this.SelectCard(cardItem);
				});
				this.SetCardHide(cardItem);
				i++;
			}
			ViewDebate.FocusingCardItem = null;
		}

		// Token: 0x060084D4 RID: 34004 RVA: 0x003DBEA8 File Offset: 0x003DA0A8
		public void RefreshSelfCards(List<short> cardDataList, bool canUseCard)
		{
			for (int i = 0; i < this._cardList.Count; i++)
			{
				DebateCardItem cardItem = this._cardList[i];
				cardItem.CardView.DOKill(true);
				bool flag = !cardDataList.CheckIndex(i);
				if (flag)
				{
					cardItem.SetVisible(false, true);
				}
				else
				{
					cardItem.SetVisible(true, true);
					cardItem.CardView.SetData(cardDataList[i], i);
					cardItem.CardView.SetEnabled(canUseCard, false);
				}
			}
			this.LayoutSelfCards();
		}

		// Token: 0x060084D5 RID: 34005 RVA: 0x003DBF38 File Offset: 0x003DA138
		public void LayoutSelfCards()
		{
			GridLayoutGroup gridLayoutGroup = this.selfCardCanvasGroup.GetComponentInChildren<GridLayoutGroup>(true);
			this.LayoutCard(gridLayoutGroup, this._cardList);
		}

		// Token: 0x060084D6 RID: 34006 RVA: 0x003DBF64 File Offset: 0x003DA164
		public void UnSelectAllCard()
		{
			int i = 0;
			int max = this._cardList.Count;
			while (i < max)
			{
				bool visible = this._cardList[i].Visible;
				if (visible)
				{
					this._cardList[i].SetSelect(false, true);
				}
				i++;
			}
		}

		// Token: 0x060084D7 RID: 34007 RVA: 0x003DBFBC File Offset: 0x003DA1BC
		public void SetAllCardSelectable(bool canUseCard)
		{
			foreach (DebateCardItem t in this._cardList)
			{
				t.CardView.SetEnabled(canUseCard, false);
			}
		}

		// Token: 0x060084D8 RID: 34008 RVA: 0x003DC01C File Offset: 0x003DA21C
		public void RefreshEnemyCards(List<short> cardDataList)
		{
			for (int i = 0; i < this._enemyCardList.Count; i++)
			{
				DebateCardItem cardItem = this._enemyCardList[i];
				bool flag = !cardDataList.CheckIndex(i);
				if (flag)
				{
					cardItem.SetVisible(false, true);
				}
				else
				{
					cardItem.SetVisible(true, true);
					cardItem.CardView.SetData(cardDataList[i], i);
					cardItem.CardView.SetEnabled(true, false);
					cardItem.CardView.SetInteractable(false);
				}
			}
		}

		// Token: 0x060084D9 RID: 34009 RVA: 0x003DC0A8 File Offset: 0x003DA2A8
		public void ShowEnemyCard(bool show)
		{
			float duration = 0.2f;
			int selfValue = show ? 0 : 1;
			this.selfCardCanvasGroup.DOKill(false);
			this.selfCardCanvasGroup.DOFade((float)selfValue, duration);
			this.selfCardCanvasGroup.transform.DOScale((float)selfValue, duration);
			int enemyValue = show ? 1 : 0;
			this.enemyCardCanvasGroup.DOKill(false);
			this.enemyCardCanvasGroup.DOFade((float)enemyValue, duration);
			this.enemyCardCanvasGroup.transform.DOScale((float)enemyValue, duration);
		}

		// Token: 0x060084DA RID: 34010 RVA: 0x003DC12C File Offset: 0x003DA32C
		public bool IsShowingEnemyCard()
		{
			return this.enemyCardCanvasGroup.transform.localScale == Vector3.one;
		}

		// Token: 0x060084DB RID: 34011 RVA: 0x003DC148 File Offset: 0x003DA348
		public void RefreshCardGroup(DebatePlayer debatePlayer, bool isSelf)
		{
			RectTransform layout = isSelf ? this.selfCardGroupLayout : this.enemyCardGroupLayout;
			for (int i = 0; i < layout.childCount; i++)
			{
				int type = i;
				Transform child = layout.GetChild(i);
				CButton button = child.GetComponent<CButton>();
				if (!true)
				{
				}
				int type2 = type;
				List<short> cardList3;
				if (type2 != 0)
				{
					if (type2 != 1)
					{
						throw new ArgumentOutOfRangeException();
					}
					cardList3 = debatePlayer.UsedCards;
				}
				else
				{
					cardList3 = debatePlayer.OwnedCards;
				}
				if (!true)
				{
				}
				List<short> cardList = cardList3;
				List<short> cardList2 = cardList;
				int cardCount = (cardList2 != null) ? cardList2.Count : 0;
				button.interactable = (cardCount > 0 && isSelf);
				HSVStyleRoot component = button.GetComponent<HSVStyleRoot>();
				if (component != null)
				{
					component.SetInteractable(button.interactable);
				}
				bool interactable = button.interactable;
				if (interactable)
				{
					button.ClearAndAddListener(delegate
					{
						AudioManager.Instance.PlaySound(ViewDebate.SoundOpenCardGroup, false, true);
						ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("CardList", cardList).Set("Type", type);
						UIElement.DebateCardGroup.SetOnInitArgs(args);
						UIManager.Instance.ShowUI(UIElement.DebateCardGroup, true);
					});
				}
				Canvas canvas = child.GetComponentInChildren<Canvas>(true);
				this._countBgCanvasSet.Add(canvas);
				TextMeshProUGUI countText = canvas.GetComponentInChildren<TextMeshProUGUI>(true);
				countText.text = cardCount.ToString();
			}
		}

		// Token: 0x060084DC RID: 34012 RVA: 0x003DC274 File Offset: 0x003DA474
		public void OnTopUiChange(bool selfIsTop)
		{
			foreach (Canvas canvas in this._countBgCanvasSet)
			{
				canvas.overrideSorting = selfIsTop;
			}
		}

		// Token: 0x060084DD RID: 34013 RVA: 0x003DC2CC File Offset: 0x003DA4CC
		public Transform GetCardGroupTransform(int location)
		{
			if (!true)
			{
			}
			Transform child;
			switch (location)
			{
			case 0:
				child = this.selfCardGroupLayout.GetChild(0);
				goto IL_6C;
			case 1:
				child = this.selfCardGroupLayout.GetChild(1);
				goto IL_6C;
			case 3:
				child = this.enemyCardGroupLayout.GetChild(0);
				goto IL_6C;
			case 4:
				child = this.enemyCardGroupLayout.GetChild(1);
				goto IL_6C;
			}
			child = this.selfCardGroupLayout.GetChild(0);
			IL_6C:
			if (!true)
			{
			}
			return child;
		}

		// Token: 0x060084DE RID: 34014 RVA: 0x003DC350 File Offset: 0x003DA550
		private void InitCardOwnedCardGroupTip(RectTransform groupLayout)
		{
			TooltipInvoker tip = groupLayout.GetComponentInChildren<TooltipInvoker>();
			string[] presetParam = tip.PresetParam;
			bool flag = presetParam == null || presetParam.Length != 2;
			if (flag)
			{
				tip.PresetParam = new string[2];
			}
			tip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_LifeSkillCombat_CardGroup_Owned);
			tip.PresetParam[1] = LocalStringManager.GetFormat(LanguageKey.LK_LifeSkillCombat_CardGroup_Owned_Tip, DebateConstants.GetStrategyLimit);
		}

		// Token: 0x040065D3 RID: 26067
		[SerializeField]
		private CanvasGroup selfCardCanvasGroup;

		// Token: 0x040065D4 RID: 26068
		[SerializeField]
		private CanvasGroup enemyCardCanvasGroup;

		// Token: 0x040065D5 RID: 26069
		[SerializeField]
		private RectTransform selfCardGroupLayout;

		// Token: 0x040065D6 RID: 26070
		[SerializeField]
		private RectTransform enemyCardGroupLayout;

		// Token: 0x040065D7 RID: 26071
		private readonly HashSet<Canvas> _countBgCanvasSet = new HashSet<Canvas>();

		// Token: 0x040065D8 RID: 26072
		private readonly List<DebateCardItem> _cardList = new List<DebateCardItem>();

		// Token: 0x040065D9 RID: 26073
		private readonly List<DebateCardItem> _enemyCardList = new List<DebateCardItem>();

		// Token: 0x040065DA RID: 26074
		[HideInInspector]
		public bool selectedCardHasScale = true;

		// Token: 0x040065DB RID: 26075
		public const float CardAnimDuration = 0.5f;
	}
}
