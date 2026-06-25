using System;
using System.Collections.Generic;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Character;
using Game.Views.Select;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Taiwu;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.CharacterMenu.Kidnap
{
	// Token: 0x02000BB7 RID: 2999
	public class CharacterMenuKidnapGridItem : MonoBehaviour
	{
		// Token: 0x060096E8 RID: 38632 RVA: 0x00465F58 File Offset: 0x00464158
		private void Awake()
		{
			this.InitButtons();
			bool flag = this.changeRopeButtonHoverItem != null;
			if (flag)
			{
				this.changeRopeButtonHoverItem.SetActive(false);
			}
		}

		// Token: 0x060096E9 RID: 38633 RVA: 0x00465F8C File Offset: 0x0046418C
		public void Set(int kidnapperId, int taiwuId, KidnapCharDisplayData displayData, bool canOperateNow, bool canSelectRope, Action onRopeChanged, Action<int> onViewPrisoner, Action onTalkAndHide)
		{
			this._displayData = displayData;
			this._kidnapperId = kidnapperId;
			this._onRopeChanged = onRopeChanged;
			this._onViewPrisoner = onViewPrisoner;
			this._onTalkAndHide = onTalkAndHide;
			bool isTaiwu = displayData.CharacterId == taiwuId;
			this.nameLabel.text = NameCenter.GetMonasticTitleOrDisplayName(ref displayData.NameData, isTaiwu, false);
			this.ageLabel.text = LocalStringManager.GetFormat(LanguageKey.LK_Age, displayData.PhysiologicalAge);
			this.kidnapDurationLabel.text = displayData.KidnapDuration.ToString();
			this.totalResistanceLabel.text = displayData.TotalResistance.ToString().SetColor((displayData.TotalResistance > 100) ? "brightred" : "brightblue");
			bool flag = this.resistanceTip != null;
			if (flag)
			{
				this.resistanceTip.Type = TipType.PrisonerResistance;
				TooltipInvoker tooltipInvoker = this.resistanceTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>();
				}
				this.resistanceTip.RuntimeParam.Set("IsPrivate", true).Set("Resistance", displayData.TotalResistance).Set("EscapeRate", displayData.EscapeRate).Set("RopeEffect", displayData.RopeEffect).Set("CompletelyInfected", displayData.CompletelyInfected).Set("OwningBook", displayData.OwningBook);
			}
			this.healthItem.Set(displayData.Health, displayData.MaxLeftHealth, displayData.CharacterId);
			this.happinessItem.Set(displayData.Happiness);
			this.favorabilityItem.Set(displayData.FavorabilityToTaiwu, isTaiwu, displayData.IsInteractedWithTaiwu);
			this.RefreshRopeDisplay(displayData.RopeItemKey);
			this.RefreshButtonStates(canOperateNow, canSelectRope);
			this.avatar.Refresh(displayData.AvatarRelatedData, displayData.CharacterTemplateId);
		}

		// Token: 0x060096EA RID: 38634 RVA: 0x00466170 File Offset: 0x00464370
		private void InitButtons()
		{
			this.inspectButton.ClearAndAddListener(new Action(this.OnTalkClicked));
			this.talkButton.ClearAndAddListener(new Action(this.OnAvatarClicked));
			this.changeRopeButton.ClearAndAddListener(new Action(this.OnChangeRopeClicked));
			this.changeRopeButtonPointerTrigger.EnterEvent.AddListener(new UnityAction(this.OnRopeButtonPointerEnter));
			this.changeRopeButtonPointerTrigger.ExitEvent.AddListener(new UnityAction(this.OnRopeButtonPointerExit));
		}

		// Token: 0x060096EB RID: 38635 RVA: 0x00466200 File Offset: 0x00464400
		private void RefreshRopeDisplay(ItemKey ropeItemKey)
		{
			bool flag = this.ropeItemIcon == null;
			if (!flag)
			{
				bool flag2 = ropeItemKey.IsValid();
				if (flag2)
				{
					this.ropeItemIcon.gameObject.SetActive(true);
					string iconName = ItemTemplateHelper.GetIcon(ropeItemKey.ItemType, ropeItemKey.TemplateId);
					this.ropeItemIcon.SetSprite(iconName, false, null);
					sbyte grade = ItemTemplateHelper.GetGrade(ropeItemKey.ItemType, ropeItemKey.TemplateId);
					this.ropeItemGradeIcon.gameObject.SetActive(true);
					this.ropeItemGradeIcon.SetSprite("ui9_icon_item_grade_" + grade.ToString(), false, null);
				}
				else
				{
					this.ropeItemIcon.gameObject.SetActive(false);
					this.ropeItemGradeIcon.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x060096EC RID: 38636 RVA: 0x004662D0 File Offset: 0x004644D0
		private void RefreshButtonStates(bool canOperateNow, bool canSelectRope)
		{
			bool isOnNormalInteractEvent = SingletonObject.getInstance<EventModel>().IsOnNormalInteractEvent;
			bool canInteract = canOperateNow && !isOnNormalInteractEvent;
			bool flag = this.talkButton != null;
			if (flag)
			{
				this.talkButton.gameObject.SetActive(canOperateNow);
				this.talkButton.interactable = canInteract;
			}
			bool flag2 = this.inspectButton != null;
			if (flag2)
			{
				this.inspectButton.gameObject.SetActive(true);
			}
			bool flag3 = this.changeRopeButton != null;
			if (flag3)
			{
				this.changeRopeButton.gameObject.SetActive(true);
				this.changeRopeButton.interactable = canSelectRope;
			}
		}

		// Token: 0x060096ED RID: 38637 RVA: 0x0046637C File Offset: 0x0046457C
		private void OnAvatarClicked()
		{
			bool flag = this._displayData == null;
			if (!flag)
			{
				UIElement eventWindow = UIElement.EventWindow;
				eventWindow.OnHide = (Action)Delegate.Combine(eventWindow.OnHide, new Action(this.OnEventWindowHide));
				TaiwuEventDomainMethod.Call.OnInteractKidnappedCharacter(this._displayData.CharacterId);
			}
		}

		// Token: 0x060096EE RID: 38638 RVA: 0x004663D0 File Offset: 0x004645D0
		private void OnEventWindowHide()
		{
			UIElement eventWindow = UIElement.EventWindow;
			eventWindow.OnHide = (Action)Delegate.Remove(eventWindow.OnHide, new Action(this.OnEventWindowHide));
			Action onTalkAndHide = this._onTalkAndHide;
			if (onTalkAndHide != null)
			{
				onTalkAndHide();
			}
			this._onTalkAndHide = null;
		}

		// Token: 0x060096EF RID: 38639 RVA: 0x0046641D File Offset: 0x0046461D
		private void OnTalkClicked()
		{
			Action<int> onViewPrisoner = this._onViewPrisoner;
			if (onViewPrisoner != null)
			{
				onViewPrisoner(this._displayData.CharacterId);
			}
		}

		// Token: 0x060096F0 RID: 38640 RVA: 0x00466440 File Offset: 0x00464640
		private void OnChangeRopeClicked()
		{
			bool flag = this._displayData == null;
			if (!flag)
			{
				SelectItemConfig config = SelectItemConfig.CreateSingleSelectConfig(new SelectItemRules
				{
					ItemSubType = 1206,
					OnlyFromInventory = true
				}, delegate(List<SelectedItemData> selectedItems)
				{
					bool flag2 = selectedItems.Count > 0;
					if (flag2)
					{
						ItemKey ropeItemKey = selectedItems[0].ItemData.Key;
						CharacterDomainMethod.Call.ChangeKidnappedCharacterRope(this._kidnapperId, this._displayData.CharacterId, ropeItemKey);
						Action onRopeChanged = this._onRopeChanged;
						if (onRopeChanged != null)
						{
							onRopeChanged();
						}
					}
				}, "", new ESelectItemColumnType?(ESelectItemColumnType.IconAndName | ESelectItemColumnType.Amount | ESelectItemColumnType.Type | ESelectItemColumnType.Value | ESelectItemColumnType.Weight | ESelectItemColumnType.EscapeRate));
				config.VisibleMainFilterToggles = new List<int>
				{
					6
				};
				config.HideSourceToggles = true;
				config.HideSortAndFilter = true;
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				argBox.SetObject("SelectItemConfig", config);
				UIElement.SelectItem.SetOnInitArgs(argBox);
				UIManager.Instance.MaskUI(UIElement.SelectItem);
			}
		}

		// Token: 0x060096F1 RID: 38641 RVA: 0x004664EC File Offset: 0x004646EC
		private void OnRopeButtonPointerEnter()
		{
			bool flag = this.changeRopeButtonHoverItem != null && this.changeRopeButton != null && this.changeRopeButton.interactable;
			if (flag)
			{
				this.changeRopeButtonHoverItem.SetActive(true);
			}
		}

		// Token: 0x060096F2 RID: 38642 RVA: 0x00466538 File Offset: 0x00464738
		private void OnRopeButtonPointerExit()
		{
			bool flag = this.changeRopeButtonHoverItem != null;
			if (flag)
			{
				this.changeRopeButtonHoverItem.SetActive(false);
			}
		}

		// Token: 0x040073C8 RID: 29640
		[SerializeField]
		private CButton inspectButton;

		// Token: 0x040073C9 RID: 29641
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x040073CA RID: 29642
		[SerializeField]
		private CButton talkButton;

		// Token: 0x040073CB RID: 29643
		[SerializeField]
		private CButton changeRopeButton;

		// Token: 0x040073CC RID: 29644
		[SerializeField]
		private PointerTrigger changeRopeButtonPointerTrigger;

		// Token: 0x040073CD RID: 29645
		[SerializeField]
		private GameObject changeRopeButtonHoverItem;

		// Token: 0x040073CE RID: 29646
		[SerializeField]
		private TextMeshProUGUI nameLabel;

		// Token: 0x040073CF RID: 29647
		[SerializeField]
		private TextMeshProUGUI ageLabel;

		// Token: 0x040073D0 RID: 29648
		[SerializeField]
		private TextMeshProUGUI kidnapDurationLabel;

		// Token: 0x040073D1 RID: 29649
		[SerializeField]
		private TextMeshProUGUI totalResistanceLabel;

		// Token: 0x040073D2 RID: 29650
		[SerializeField]
		private Health healthItem;

		// Token: 0x040073D3 RID: 29651
		[SerializeField]
		private Happiness happinessItem;

		// Token: 0x040073D4 RID: 29652
		[SerializeField]
		private Favorability favorabilityItem;

		// Token: 0x040073D5 RID: 29653
		[SerializeField]
		private CImage ropeItemIcon;

		// Token: 0x040073D6 RID: 29654
		[SerializeField]
		private CImage ropeItemGradeIcon;

		// Token: 0x040073D7 RID: 29655
		[SerializeField]
		private TooltipInvoker resistanceTip;

		// Token: 0x040073D8 RID: 29656
		private KidnapCharDisplayData _displayData;

		// Token: 0x040073D9 RID: 29657
		private int _kidnapperId;

		// Token: 0x040073DA RID: 29658
		private Action _onRopeChanged;

		// Token: 0x040073DB RID: 29659
		private Action<int> _onViewPrisoner;

		// Token: 0x040073DC RID: 29660
		private Action _onTalkAndHide;
	}
}
