using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Views.Select;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Information;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Merchant;
using GameData.Domains.Taiwu;
using GameData.Domains.Taiwu.Display;
using GameData.Domains.Taiwu.ExchangeSystem;
using GameData.Domains.TaiwuEvent;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Exchange
{
	// Token: 0x02000A32 RID: 2610
	public class ViewExchangeBook : ViewExchangeBase
	{
		// Token: 0x17000DF9 RID: 3577
		// (get) Token: 0x06008058 RID: 32856 RVA: 0x003BBBB9 File Offset: 0x003B9DB9
		protected override string SortSaveKeyPrefix
		{
			get
			{
				return "ViewExchangeBook";
			}
		}

		// Token: 0x17000DFA RID: 3578
		// (get) Token: 0x06008059 RID: 32857 RVA: 0x003BBBC0 File Offset: 0x003B9DC0
		public override Exchange Exchange
		{
			get
			{
				return this._displayData.Exchange;
			}
		}

		// Token: 0x17000DFB RID: 3579
		// (get) Token: 0x0600805A RID: 32858 RVA: 0x003BBBCD File Offset: 0x003B9DCD
		public override SecretInformationDisplayPackage SecretInformationDisplayPackage
		{
			get
			{
				return this._displayData.SecretInformationDisplayPackage;
			}
		}

		// Token: 0x17000DFC RID: 3580
		// (get) Token: 0x0600805B RID: 32859 RVA: 0x003BBBDA File Offset: 0x003B9DDA
		public override CharacterDisplayData TargetDisplayData
		{
			get
			{
				return this._displayData.TargetCharacterDisplayData;
			}
		}

		// Token: 0x17000DFD RID: 3581
		// (get) Token: 0x0600805C RID: 32860 RVA: 0x003BBBE7 File Offset: 0x003B9DE7
		public override CharacterDisplayData TaiwuDisplayData
		{
			get
			{
				return this._displayData.TaiwuDisplayData;
			}
		}

		// Token: 0x17000DFE RID: 3582
		// (get) Token: 0x0600805D RID: 32861 RVA: 0x003BBBF4 File Offset: 0x003B9DF4
		protected override ItemListScroll.EColumnType ColumnType
		{
			get
			{
				return ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability | ItemListScroll.EColumnType.Book;
			}
		}

		// Token: 0x0600805E RID: 32862 RVA: 0x003BBBFC File Offset: 0x003B9DFC
		public override IReadOnlyList<ITradeableContent> GetSelfTradeableList(int index)
		{
			if (!true)
			{
			}
			IReadOnlyList<ITradeableContent> result;
			switch (index)
			{
			case 0:
				result = this._displayData.TaiwuInventoryItemDisplayDataList;
				break;
			case 1:
				result = this._displayData.TaiwuWarehouseItemDisplayDataList;
				break;
			case 2:
				result = this._displayData.TaiwuTreasuryItemDisplayDataList;
				break;
			default:
				result = this._displayData.TaiwuKidnapMenuDisplayData.KidnapCharDisplayDataList;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600805F RID: 32863 RVA: 0x003BBC63 File Offset: 0x003B9E63
		public override IReadOnlyList<ITradeableContent> GetTargetTradeableList(int index)
		{
			return (index == -1) ? this._displayData.TargetItemDisplayDataList : this._displayData.TargetItemDisplayDataListBuyBack;
		}

		// Token: 0x17000DFF RID: 3583
		// (get) Token: 0x06008060 RID: 32864 RVA: 0x003BBC81 File Offset: 0x003B9E81
		protected override LanguageKey ExchangeNotFair
		{
			get
			{
				return LanguageKey.LK_Exchange_Not_Fair_Book;
			}
		}

		// Token: 0x17000E00 RID: 3584
		// (get) Token: 0x06008061 RID: 32865 RVA: 0x003BBC88 File Offset: 0x003B9E88
		protected override LanguageKey ExchangeEmpty
		{
			get
			{
				return LanguageKey.LK_Exchange_Plan_Is_Empty_Book;
			}
		}

		// Token: 0x06008062 RID: 32866 RVA: 0x003BBC8F File Offset: 0x003B9E8F
		protected override void Awake()
		{
			this.ConfirmOnHideKey = LanguageKey.LK_Exchange_Book_Title;
			base.Awake();
			this.exchangeContainer.exchangeBack.SetMerchant(1);
			this.exchangeContainer.exchangeFront.SetMerchant(1);
		}

		// Token: 0x06008063 RID: 32867 RVA: 0x003BBCC8 File Offset: 0x003B9EC8
		protected override void AwakeAvatarBtn()
		{
			this.exchangeContainer.avatarBtn.interactable = true;
			this.exchangeContainer.avatarBtn.onClick.ResetListener(delegate()
			{
				base.ShowCharacterMenu(this._displayData.TargetCharacterDisplayData);
			});
			this.exchangeContainer.btnOpenCharMenu.onClick.ResetListener(delegate()
			{
				base.ShowCharacterMenu(this._displayData.TargetCharacterDisplayData);
			});
		}

		// Token: 0x06008064 RID: 32868 RVA: 0x003BBD2C File Offset: 0x003B9F2C
		public override void Submit()
		{
			bool flag = this.Exchange.TargetValueBase + this.Exchange.TotalValueWithAdvantage < 0L;
			if (flag)
			{
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
				{
					Title = LanguageKey.LK_ExchangeBook_Dialog_Title.Tr(),
					Content = LanguageKey.LK_ExchangeBook_Dialog_Content.Tr(),
					Yes = new Action(this.SubmitImpl)
				}));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
			else
			{
				this.SubmitImpl();
			}
		}

		// Token: 0x06008065 RID: 32869 RVA: 0x003BBDC5 File Offset: 0x003B9FC5
		private void SubmitImpl()
		{
			TaiwuDomainMethod.Call.ConfirmExchange(this.Exchange);
			this.Reset();
		}

		// Token: 0x06008066 RID: 32870 RVA: 0x003BBDDC File Offset: 0x003B9FDC
		public override void OnInit(ArgumentBox argsBox)
		{
			this.ExchangeValueType = 7;
			this.ShowNonInteractableGoods = true;
			base.OnInit(argsBox);
			argsBox.Get("IsFavorExchange", out this._isFavorExchange);
			this.exchangeContainer.secretText.text = LanguageKey.LK_Exchange_Use_Secret.Tr();
			this.exchangeContainer.confirmButtonText.text = LanguageKey.LK_Exchange_Confirm_Book.Tr();
			this.exchangeContainer.AddSwitchToggleListener();
			this.exchangeContainer.currPage.Get(3).gameObject.SetActive(false);
			bool flag = !this._firstEnter;
			if (flag)
			{
				this._firstEnter = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(81);
			}
		}

		// Token: 0x06008067 RID: 32871 RVA: 0x003BBE8F File Offset: 0x003BA08F
		protected override void RequestData()
		{
			TaiwuDomainMethod.AsyncCall.GetExchangeDisplayData(this, this.TargetId, this._isFavorExchange ? EExchangeType.BookPriv : EExchangeType.BookSect, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._displayData);
				List<ItemDisplayData> targetItemDisplayDataListBuyBack = this._displayData.TargetItemDisplayDataListBuyBack;
				bool canBuyBack = ((targetItemDisplayDataListBuyBack != null) ? targetItemDisplayDataListBuyBack.Count : 0) > 0;
				this.exchangeContainer.targetPage.SetInteractable(canBuyBack);
				bool flag = !canBuyBack;
				if (flag)
				{
					this.exchangeContainer.targetPage.DeSelect(false);
				}
				this.SetTargetCharacterDisplayData(this._displayData.TargetCharacterDisplayData, LanguageKey.LK_Exchange_SubTitle_Book);
				bool flag2 = !this._isFavorExchange;
				if (flag2)
				{
					this.approvingRate.text = LanguageKey.LK_Exchange_CurrentApproving.TrFormat((float)this._displayData.ApproveRate / 10f, this._displayData.ApproveRateMax / 10);
					GradeComponent grade = this.exchangeContainer.grade;
					if (grade != null)
					{
						grade.gameObject.SetActive(false);
					}
					this.approvingRate.gameObject.SetActive(true);
				}
				else
				{
					GradeComponent grade2 = this.exchangeContainer.grade;
					if (grade2 != null)
					{
						grade2.gameObject.SetActive(true);
					}
					this.approvingRate.gameObject.SetActive(false);
				}
				this.exchangeContainer.titleTaiwu.text = LanguageKey.LK_Exchange_SubTitle_Book.TrFormat(SingletonObject.getInstance<BasicGameData>().TaiwuMonasticTitleOrDisplayName);
				this.SetWarehouseCanInteract(this._displayData.CanTransferItemToWarehouse);
				this.SetExchangeData();
				this.exchangeContainer.selfItemList.SortAndFilterController.SetToggleVisible(0, 3);
				this.exchangeContainer.selfItemList.SortAndFilterController.SetToggleIsOn(0, 3);
				this.exchangeContainer.targetItemList.SortAndFilterController.SetToggleVisible(0, 3);
				this.exchangeContainer.targetItemList.SortAndFilterController.SetToggleIsOn(0, 3);
				this.Refresh();
				this.Element.ShowAfterRefresh();
			});
		}

		// Token: 0x06008068 RID: 32872 RVA: 0x003BBEB8 File Offset: 0x003BA0B8
		protected override void RefreshValues()
		{
			this.exchangeContainer.targetValue1.text = ViewExchangeBase.GetAuthorityString((int)this.Exchange.TargetValueBase, (int)(this.Exchange.TargetValueBase + this.Exchange.TotalValueWithAdvantage));
			TMP_Text selfValue = this.exchangeContainer.selfValue3;
			int activeIndex = this.exchangeContainer.currPage.GetActiveIndex();
			selfValue.text = ((activeIndex == 0 || activeIndex == 5) ? ViewExchangeBase.GetWeightString(this.Exchange.TaiwuInventoryCurLoad, this.Exchange.TaiwuInventoryMaxLoad, this.Exchange.TaiwuInventoryCurLoadPreview, this.Exchange.TaiwuInventoryMaxLoadPreview, LanguageKey.LK_Exchange_Weight_Value) : ViewExchangeBase.GetWeightString(this.Exchange.TaiwuWarehouseCurLoad, this.Exchange.TaiwuWarehouseMaxLoad, this.Exchange.TaiwuWarehouseCurLoadPreview, this.Exchange.TaiwuWarehouseMaxLoadPreview, LanguageKey.LK_Exchange_Weight_Value));
			this.exchangeContainer.selfValue1.text = ViewExchangeBase.GetAuthorityString((int)this.Exchange.TaiwuValueBase, (int)(this.Exchange.TaiwuValueBase - this.Exchange.TotalValueWithAdvantage));
			this.exchangeContainer.secretText.text = LanguageKey.LK_Exchange_Use_Secret.Tr();
		}

		// Token: 0x06008069 RID: 32873 RVA: 0x003BBFEC File Offset: 0x003BA1EC
		protected override bool CheckItemIsLocked(ITradeableContent itemData, CharacterDisplayData cd, out string reason)
		{
			bool flag = base.CheckItemIsLocked(itemData, cd, out reason);
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = cd == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = cd.CharacterId == this.TaiwuDisplayData.CharacterId;
					if (flag3)
					{
						result = false;
					}
					else
					{
						bool flag4 = this.exchangeContainer.targetPage.GetActiveIndex() != -1;
						if (flag4)
						{
							result = false;
						}
						else
						{
							bool isFavorExchange = this._isFavorExchange;
							if (isFavorExchange)
							{
								bool flag5 = !this.CanReadBookByConsummateLevel(itemData) && this.TargetDisplayData.OrgInfo.OrgTemplateId != 16;
								if (flag5)
								{
									reason = LanguageKey.LK_ExchangeBook_Tag_0.Tr();
									return true;
								}
							}
							else
							{
								bool flag6 = this.IsLockByApproveEnough(itemData);
								if (flag6)
								{
									reason = LanguageKey.LK_ExchangeBook_Tag_1.Tr();
									return true;
								}
								bool flag7 = !this.IsTaiwuLearned(itemData);
								if (flag7)
								{
									reason = LanguageKey.LK_ExchangeBook_Tag_2.Tr();
									return true;
								}
							}
							result = false;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600806A RID: 32874 RVA: 0x003BC0F0 File Offset: 0x003BA2F0
		private bool CanReadBookByConsummateLevel(ITradeableContent item)
		{
			sbyte grade = ItemTemplateHelper.GetGrade(item.Key.ItemType, item.Key.TemplateId);
			return SingletonObject.getInstance<BasicGameData>().XiangshuProgress >= (grade - 1) * 2;
		}

		// Token: 0x0600806B RID: 32875 RVA: 0x003BC134 File Offset: 0x003BA334
		private bool IsTaiwuLearned(ITradeableContent itemDisplayData)
		{
			SkillBookItem config = SkillBook.Instance[itemDisplayData.Key.TemplateId];
			return this._displayData.TaiwuLearnedCombatSkills.Contains(config.CombatSkillTemplateId);
		}

		// Token: 0x0600806C RID: 32876 RVA: 0x003BC174 File Offset: 0x003BA374
		private bool IsLockByApproveEnough(ITradeableContent item)
		{
			sbyte grade = ItemTemplateHelper.GetGrade(item.Key.ItemType, item.Key.TemplateId);
			return this._displayData.ApproveHighestGrade < grade;
		}

		// Token: 0x0600806D RID: 32877 RVA: 0x003BC1B0 File Offset: 0x003BA3B0
		public override void PrepareCustomRowTemplateContainersForCharacter(RowItem rowTemplate)
		{
			this.PrepareCustomRowTemplateContainers(rowTemplate, false);
		}

		// Token: 0x0600806E RID: 32878 RVA: 0x003BC1BC File Offset: 0x003BA3BC
		public override void PrepareCustomRowTemplateContainers(RowItem rowTemplate, bool isItemMode)
		{
			Transform containerRoot = rowTemplate.ContainerRoot;
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.itemIconAndNameCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.exchangeContainer.singleTextCellContainer, containerRoot).gameObject.SetActive(true);
			Object.Instantiate<RowCellContainer>(this.bookContainer, containerRoot).gameObject.SetActive(true);
		}

		// Token: 0x0600806F RID: 32879 RVA: 0x003BC240 File Offset: 0x003BA440
		public override IEnumerable<ColumnDefinition> GenerateColumnDefinitions(ItemListScroll scroll, bool isItemMode)
		{
			yield return scroll.ColumnIconAndName(default(LayoutOption), false);
			LayoutOption numOption = new LayoutOption(40f, 1f, 122f, 1);
			yield return scroll.ColumnValue(numOption, true, LanguageKey.LK_Resource_Name_Authority);
			yield return scroll.ColumnDurability(numOption, true);
			ColumnDefinition progress = SelectItemColumnHelper.CreateBookReadingInfoColumn();
			progress.LayoutOption.MinWidth = 400f;
			yield return progress;
			yield break;
		}

		// Token: 0x17000E01 RID: 3585
		// (get) Token: 0x06008070 RID: 32880 RVA: 0x003BC25E File Offset: 0x003BA45E
		protected override string ConfirmOnHideDesc
		{
			get
			{
				return LanguageKey.LK_Exchange_Confirm_Book_Cancel.Tr();
			}
		}

		// Token: 0x06008071 RID: 32881 RVA: 0x003BC26A File Offset: 0x003BA46A
		public override void QuickHideImpl()
		{
			MerchantDomainMethod.Call.FinishBookTrade(this.TargetId, this._isFavorExchange);
			TaiwuEventDomainMethod.Call.TriggerListener("ExchangeBookComplete", true);
			base.QuickHideImpl();
		}

		// Token: 0x0400622B RID: 25131
		[SerializeField]
		private RowCellContainer bookContainer;

		// Token: 0x0400622C RID: 25132
		[SerializeField]
		private TMP_Text approvingRate;

		// Token: 0x0400622D RID: 25133
		private ExchangeDisplayData _displayData;

		// Token: 0x0400622E RID: 25134
		private bool _isFavorExchange;

		// Token: 0x0400622F RID: 25135
		private bool _firstEnter;
	}
}
