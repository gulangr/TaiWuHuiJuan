using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FrameWork;
using GameData.Domains.Taiwu.Debate;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000244 RID: 580
public class LifeSkillCombatCardArea2 : MonoBehaviour
{
	// Token: 0x170003F2 RID: 1010
	// (get) Token: 0x060025C8 RID: 9672 RVA: 0x00115856 File Offset: 0x00113A56
	private LifeSkillCombatModel Model
	{
		get
		{
			return SingletonObject.getInstance<LifeSkillCombatModel>();
		}
	}

	// Token: 0x170003F3 RID: 1011
	// (get) Token: 0x060025C9 RID: 9673 RVA: 0x0011585D File Offset: 0x00113A5D
	// (set) Token: 0x060025CA RID: 9674 RVA: 0x0011586A File Offset: 0x00113A6A
	public LifeSkillCombatCardItem FocusingCardItem
	{
		get
		{
			return this.Model.FocusingCardItem;
		}
		set
		{
			this.Model.FocusingCardItem = value;
		}
	}

	// Token: 0x170003F4 RID: 1012
	// (get) Token: 0x060025CB RID: 9675 RVA: 0x00115879 File Offset: 0x00113A79
	public List<LifeSkillCombatCardItem> CardList
	{
		get
		{
			return this._cardList;
		}
	}

	// Token: 0x170003F5 RID: 1013
	// (get) Token: 0x060025CC RID: 9676 RVA: 0x00115881 File Offset: 0x00113A81
	public List<LifeSkillCombatCardItem> EnemyCardList
	{
		get
		{
			return this._enemyCardList;
		}
	}

	// Token: 0x060025CD RID: 9677 RVA: 0x00115889 File Offset: 0x00113A89
	private void Awake()
	{
		this.LifeSkillCombatTipCardView.gameObject.SetActive(false);
		this.InitCardList();
		this.InitCardOwnedCardGroupTip(this._selfCardGroupLayout);
		this.InitCardOwnedCardGroupTip(this._enemyCardGroupLayout);
	}

	// Token: 0x060025CE RID: 9678 RVA: 0x001158C0 File Offset: 0x00113AC0
	private void OnEnable()
	{
		this._selfCardCanvasGroup.transform.localScale = Vector3.one;
		this._selfCardCanvasGroup.alpha = 1f;
		this._enemyCardCanvasGroup.transform.localScale = Vector3.zero;
		this._enemyCardCanvasGroup.alpha = 0f;
		this._selfCardCanvasGroup.gameObject.SetActive(true);
		this._enemyCardCanvasGroup.gameObject.SetActive(true);
	}

	// Token: 0x060025CF RID: 9679 RVA: 0x00115940 File Offset: 0x00113B40
	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.ClearAllCard();
	}

	// Token: 0x060025D0 RID: 9680 RVA: 0x00115951 File Offset: 0x00113B51
	private void SetCardHide(LifeSkillCombatCardItem cardItem)
	{
		cardItem.SetVisible(false, true);
	}

	// Token: 0x060025D1 RID: 9681 RVA: 0x00115960 File Offset: 0x00113B60
	public void UnselectCard(bool refreshStrategy = true)
	{
		bool flag = null == this.FocusingCardItem;
		if (!flag)
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("refreshStrategy", refreshStrategy);
			GEvent.OnEvent(UiEvents.OnLifeSkillCombatExitCardFocusMode, args);
			this.FocusingCardItem.SetSelect(false, true);
			this.FocusingCardItem = null;
		}
	}

	// Token: 0x060025D2 RID: 9682 RVA: 0x001159B8 File Offset: 0x00113BB8
	public void SelectCard(LifeSkillCombatCardItem cardItem)
	{
		bool flag = this.Model.Pause && !this.Model.IsAuto && this.Model.IsTaiwuRound;
		if (!flag)
		{
			bool flag2 = this.FocusingCardItem;
			if (flag2)
			{
				bool flag3 = this.FocusingCardItem == cardItem;
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
				this.FocusingCardItem = cardItem;
				cardItem.SetSelect(true, this.SelectedCardHasScale);
				GEvent.OnEvent(UiEvents.OnLifeSkillCombatEnterCardFocusMode, null);
			}
		}
	}

	// Token: 0x060025D3 RID: 9683 RVA: 0x00115A7C File Offset: 0x00113C7C
	private void SortCardList(bool isSelf)
	{
		List<LifeSkillCombatCardItem> cardList = isSelf ? this._cardList : this._enemyCardList;
		cardList.Sort(delegate(LifeSkillCombatCardItem left, LifeSkillCombatCardItem right)
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

	// Token: 0x060025D4 RID: 9684 RVA: 0x00115AC4 File Offset: 0x00113CC4
	private void LayoutCard(GridLayoutGroup gridLayoutGroup, List<LifeSkillCombatCardItem> cardList)
	{
		float totalWidth = gridLayoutGroup.GetComponent<RectTransform>().rect.width;
		int visibleCount = cardList.Count((LifeSkillCombatCardItem c) => c.Visible);
		float spacingX = (totalWidth - gridLayoutGroup.cellSize.x * (float)visibleCount) / (float)(visibleCount - 1);
		gridLayoutGroup.spacing = Vector2.zero.SetX(Mathf.Min(spacingX, 0f));
	}

	// Token: 0x060025D5 RID: 9685 RVA: 0x00115B40 File Offset: 0x00113D40
	private void ClearAllCard()
	{
		this._cardList.ForEach(delegate(LifeSkillCombatCardItem e)
		{
			e.SetVisible(false, true);
		});
		this._enemyCardList.ForEach(delegate(LifeSkillCombatCardItem e)
		{
			e.SetVisible(false, true);
		});
		this.FocusingCardItem = null;
	}

	// Token: 0x060025D6 RID: 9686 RVA: 0x00115BAC File Offset: 0x00113DAC
	public List<short> GetSelectedCards()
	{
		List<short> result = new List<short>();
		foreach (LifeSkillCombatCardItem cardItem in this._cardList)
		{
			bool selected = cardItem.Selected;
			if (selected)
			{
				result.Add(cardItem.CardConfig.TemplateId);
			}
		}
		return result;
	}

	// Token: 0x060025D7 RID: 9687 RVA: 0x00115C28 File Offset: 0x00113E28
	private void InitCardList()
	{
		this._cardList.Clear();
		this._cardList.AddRange(this._selfCardCanvasGroup.GetComponentsInChildren<LifeSkillCombatCardItem>(true));
		this._enemyCardList.Clear();
		this._enemyCardList.AddRange(this._enemyCardCanvasGroup.GetComponentsInChildren<LifeSkillCombatCardItem>(true));
		int i = 0;
		int max = this._cardList.Count;
		while (i < max)
		{
			LifeSkillCombatCardItem cardItem = this._cardList[i];
			cardItem.CardView.SetOnClick(delegate
			{
				this.SelectCard(cardItem);
			});
			this.SetCardHide(cardItem);
			i++;
		}
		this.FocusingCardItem = null;
	}

	// Token: 0x060025D8 RID: 9688 RVA: 0x00115CF0 File Offset: 0x00113EF0
	public void RefreshSelfCards(List<short> cardDataList, bool canUseCard)
	{
		for (int i = 0; i < this._cardList.Count; i++)
		{
			LifeSkillCombatCardItem cardItem = this._cardList[i];
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

	// Token: 0x060025D9 RID: 9689 RVA: 0x00115D80 File Offset: 0x00113F80
	public void LayoutSelfCards()
	{
		GridLayoutGroup gridLayoutGroup = this._selfCardCanvasGroup.GetComponentInChildren<GridLayoutGroup>(true);
		this.LayoutCard(gridLayoutGroup, this._cardList);
	}

	// Token: 0x060025DA RID: 9690 RVA: 0x00115DAC File Offset: 0x00113FAC
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

	// Token: 0x060025DB RID: 9691 RVA: 0x00115E04 File Offset: 0x00114004
	public void SetAllCardSelectable(bool canUseCard)
	{
		foreach (LifeSkillCombatCardItem t in this._cardList)
		{
			t.CardView.SetEnabled(canUseCard, false);
		}
	}

	// Token: 0x060025DC RID: 9692 RVA: 0x00115E64 File Offset: 0x00114064
	public void RefreshEnemyCards(List<short> cardDataList)
	{
		for (int i = 0; i < this._enemyCardList.Count; i++)
		{
			LifeSkillCombatCardItem cardItem = this._enemyCardList[i];
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

	// Token: 0x060025DD RID: 9693 RVA: 0x00115EF0 File Offset: 0x001140F0
	public void ShowEnemyCard(bool show)
	{
		float duration = 0.2f;
		int selfValue = show ? 0 : 1;
		this._selfCardCanvasGroup.DOKill(false);
		this._selfCardCanvasGroup.DOFade((float)selfValue, duration);
		this._selfCardCanvasGroup.transform.DOScale((float)selfValue, duration);
		int enemyValue = show ? 1 : 0;
		this._enemyCardCanvasGroup.DOKill(false);
		this._enemyCardCanvasGroup.DOFade((float)enemyValue, duration);
		this._enemyCardCanvasGroup.transform.DOScale((float)enemyValue, duration);
	}

	// Token: 0x060025DE RID: 9694 RVA: 0x00115F74 File Offset: 0x00114174
	public bool IsShowingEnemyCard()
	{
		return this._enemyCardCanvasGroup.transform.localScale == Vector3.one;
	}

	// Token: 0x060025DF RID: 9695 RVA: 0x00115F90 File Offset: 0x00114190
	public void RefreshCardGroup(DebatePlayer debatePlayer, bool isSelf)
	{
		RectTransform layout = isSelf ? this._selfCardGroupLayout : this._enemyCardGroupLayout;
		for (int i = 0; i < layout.childCount; i++)
		{
			int type = i;
			Transform child = layout.GetChild(i);
			CButtonObsolete button = child.GetComponent<CButtonObsolete>();
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
			bool interactable = button.interactable;
			if (interactable)
			{
				button.ClearAndAddListener(delegate
				{
					AudioManager.Instance.PlaySound(UI_LifeSkillCombat2.SoundOpenCardGroup, false, true);
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

	// Token: 0x060025E0 RID: 9696 RVA: 0x001160A0 File Offset: 0x001142A0
	public void OnTopUiChange(bool selfIsTop)
	{
		foreach (Canvas canvas in this._countBgCanvasSet)
		{
			canvas.overrideSorting = selfIsTop;
		}
	}

	// Token: 0x060025E1 RID: 9697 RVA: 0x001160F8 File Offset: 0x001142F8
	public Transform GetCardGroupTransform(int location)
	{
		if (!true)
		{
		}
		Transform child;
		switch (location)
		{
		case 0:
			child = this._selfCardGroupLayout.GetChild(0);
			goto IL_6C;
		case 1:
			child = this._selfCardGroupLayout.GetChild(1);
			goto IL_6C;
		case 3:
			child = this._enemyCardGroupLayout.GetChild(0);
			goto IL_6C;
		case 4:
			child = this._enemyCardGroupLayout.GetChild(1);
			goto IL_6C;
		}
		child = this._selfCardGroupLayout.GetChild(0);
		IL_6C:
		if (!true)
		{
		}
		return child;
	}

	// Token: 0x060025E2 RID: 9698 RVA: 0x0011617C File Offset: 0x0011437C
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

	// Token: 0x04001BFE RID: 7166
	public LifeSkillCombatCardView LifeSkillCombatTipCardView;

	// Token: 0x04001BFF RID: 7167
	[SerializeField]
	private CanvasGroup _selfCardCanvasGroup;

	// Token: 0x04001C00 RID: 7168
	[SerializeField]
	private CanvasGroup _enemyCardCanvasGroup;

	// Token: 0x04001C01 RID: 7169
	[SerializeField]
	private RectTransform _selfCardGroupLayout;

	// Token: 0x04001C02 RID: 7170
	[SerializeField]
	private RectTransform _enemyCardGroupLayout;

	// Token: 0x04001C03 RID: 7171
	private readonly HashSet<Canvas> _countBgCanvasSet = new HashSet<Canvas>();

	// Token: 0x04001C04 RID: 7172
	private readonly List<LifeSkillCombatCardItem> _cardList = new List<LifeSkillCombatCardItem>();

	// Token: 0x04001C05 RID: 7173
	private readonly List<LifeSkillCombatCardItem> _enemyCardList = new List<LifeSkillCombatCardItem>();

	// Token: 0x04001C06 RID: 7174
	[HideInInspector]
	public bool SelectedCardHasScale = true;

	// Token: 0x04001C07 RID: 7175
	public const float CardAnimDuration = 0.5f;
}
