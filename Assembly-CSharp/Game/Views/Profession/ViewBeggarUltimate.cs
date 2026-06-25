using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item.Apply;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Map;
using GameData.Domains.Taiwu.Profession;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Profession
{
	// Token: 0x020007C7 RID: 1991
	public class ViewBeggarUltimate : UIBase
	{
		// Token: 0x17000BD7 RID: 3031
		// (get) Token: 0x06006126 RID: 24870 RVA: 0x002C8274 File Offset: 0x002C6474
		private Injury Injury
		{
			get
			{
				return this.attributeAndInjury.Injury;
			}
		}

		// Token: 0x17000BD8 RID: 3032
		// (get) Token: 0x06006127 RID: 24871 RVA: 0x002C8281 File Offset: 0x002C6481
		private WorldMapModel MapModel
		{
			get
			{
				return SingletonObject.getInstance<WorldMapModel>();
			}
		}

		// Token: 0x17000BD9 RID: 3033
		// (get) Token: 0x06006128 RID: 24872 RVA: 0x002C8288 File Offset: 0x002C6488
		private int TaiwuId
		{
			get
			{
				return SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17000BDA RID: 3034
		// (get) Token: 0x06006129 RID: 24873 RVA: 0x002C8294 File Offset: 0x002C6494
		private int CurrCharId
		{
			get
			{
				return this._data[this._currCharIndex].CharacterData.CharacterId;
			}
		}

		// Token: 0x0600612A RID: 24874 RVA: 0x002C82B1 File Offset: 0x002C64B1
		public override void OnInit(ArgumentBox argsBox)
		{
		}

		// Token: 0x0600612B RID: 24875 RVA: 0x002C82B4 File Offset: 0x002C64B4
		private void Awake()
		{
			this.characterScroll.InitPageCount();
			this.characterScroll.OnItemRender += this.OnRenderCharacter;
			this.itemScroll.SetCustomBuildGroup(new Action(this.CustomBuildGroup), false);
			this.itemScroll.Init("BeggarUltimate", ESortAndFilterControllerType.Food, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderItem), new Action<ITradeableContent, RowItemLine>(this.OnClickItem), ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Durability, null, null, null);
			this.closeButton.ClearAndAddListener(new Action(this.QuickHide));
			this._btnList = new List<ViewPopupMenu.BtnData>
			{
				new ViewPopupMenu.BtnData(LanguageKey.LK_Eat_Item.Tr(), true, EItemMenuDisplayOrder.Eat, new Action(this.OnClickPopupBtnEat), new UnityAction(this.ShowEatItemInfectNotice), new UnityAction(this.HideEatItemInfectNotice), false)
			};
		}

		// Token: 0x0600612C RID: 24876 RVA: 0x002C8392 File Offset: 0x002C6592
		private void OnEnable()
		{
			this._currCharIndex = 0;
			this.attributeAndInjury.CharacterId = this.TaiwuId;
			this.RequestData();
		}

		// Token: 0x0600612D RID: 24877 RVA: 0x002C83B8 File Offset: 0x002C65B8
		private void RequestData()
		{
			MapBlockData block = this.MapModel.GetBlockData(this.MapModel.CurrentLocation);
			List<int> list = new List<int>();
			bool flag = ((block != null) ? block.CharacterSet : null) != null;
			if (flag)
			{
				list.AddRange(block.CharacterSet);
			}
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayDataForBeggarUltimate(this, list, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._data);
				this.OnRequestData();
			});
		}

		// Token: 0x0600612E RID: 24878 RVA: 0x002C8418 File Offset: 0x002C6618
		private void OnRequestData()
		{
			foreach (CharacterDisplayDataForBeggarUltimate data in this._data)
			{
				CharacterDisplayDataForBeggarUltimate characterDisplayDataForBeggarUltimate = data;
				if (characterDisplayDataForBeggarUltimate.ItemDataList == null)
				{
					characterDisplayDataForBeggarUltimate.ItemDataList = new List<ItemDisplayData>();
				}
			}
			this._data.Sort(new Comparison<CharacterDisplayDataForBeggarUltimate>(this.CompareCharacter));
			this.charCountLabel.text = this._data.Count.ToString();
			this.characterScroll.SetDataCount(this._data.Count);
			this.OnClickCharacter(0);
		}

		// Token: 0x0600612F RID: 24879 RVA: 0x002C84D4 File Offset: 0x002C66D4
		private void OnRenderCharacter(int index, GameObject obj)
		{
			CharacterDisplayDataForBeggarUltimate data = this._data[index];
			CharacterMenuSelectableCharacterNormal item = obj.GetComponent<CharacterMenuSelectableCharacterNormal>();
			bool canInteract = data.ItemDataList.Count > 0;
			item.interactable = canInteract;
			item.GetComponent<DisableStyleRoot>().SetStyleEffect(!canInteract, false);
			item.ClearAndAddListener(delegate
			{
				this.OnClickCharacter(index);
			});
			item.Set(data.CharacterData, index == this._currCharIndex);
			bool flag = !canInteract;
			if (flag)
			{
				item.GetComponent<TooltipInvoker>().PresetParam = new string[]
				{
					LanguageKey.LK_ProfessionBeggarSkill3Tip.Tr().SetColor("brightred")
				};
				item.GetComponent<TooltipInvoker>().enabled = true;
			}
			else
			{
				item.GetComponent<TooltipInvoker>().enabled = false;
			}
		}

		// Token: 0x06006130 RID: 24880 RVA: 0x002C85B8 File Offset: 0x002C67B8
		private void OnClickCharacter(int index)
		{
			GameObject prevObj = this.characterScroll.GetActiveCell(this._currCharIndex);
			bool flag = prevObj != null;
			if (flag)
			{
				prevObj.GetComponent<CharacterMenuSelectableCharacterNormal>().SetSelected(false);
			}
			this._currCharIndex = index;
			GameObject obj = this.characterScroll.GetActiveCell(this._currCharIndex);
			bool flag2 = obj != null;
			if (flag2)
			{
				obj.GetComponent<CharacterMenuSelectableCharacterNormal>().SetSelected(true);
			}
			this.itemScroll.SetItemList(this._data[index].ItemDataList);
		}

		// Token: 0x06006131 RID: 24881 RVA: 0x002C8640 File Offset: 0x002C6840
		private void OnRenderItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			rowItemLine.SetSelected(false);
			rowItemLine.RowItemMain.HideInteractionState();
			rowItemLine.RowItemMain.SetFavoriteStatus(RowItemMain.FavoriteStatus.None);
		}

		// Token: 0x06006132 RID: 24882 RVA: 0x002C8687 File Offset: 0x002C6887
		private void OnClickItem(ITradeableContent content, RowItemLine rowItemLine)
		{
			this._selectedItem = content;
			this.itemScroll.SetItemToPopupMenuMode(rowItemLine, this._btnList, new Action(this.HideEatItemInfectNotice), false);
		}

		// Token: 0x06006133 RID: 24883 RVA: 0x002C86B4 File Offset: 0x002C68B4
		private void OnClickPopupBtnEat()
		{
			this._skillArg.CharId = this.CurrCharId;
			this._skillArg.ItemKey = this._selectedItem.RealKey;
			ArgumentBox argsBox = EasyPool.Get<ArgumentBox>();
			argsBox.SetObject("ProfessionSkillArg", this._skillArg);
			argsBox.SetObject("OnConfirm", new Action(this.OnProfessionSkillConfirm));
			UIElement.ProfessionSkillConfirm.SetOnInitArgs(argsBox);
			UIManager.Instance.MaskUI(UIElement.ProfessionSkillConfirm);
		}

		// Token: 0x06006134 RID: 24884 RVA: 0x002C8735 File Offset: 0x002C6935
		private void OnProfessionSkillConfirm()
		{
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(5U, delegate
			{
				this.CallRefreshItems(this._currCharIndex);
			});
		}

		// Token: 0x06006135 RID: 24885 RVA: 0x002C8750 File Offset: 0x002C6950
		private void ShowEatItemInfectNotice()
		{
			bool isActiveAndEnabled = this.Injury.isActiveAndEnabled;
			if (isActiveAndEnabled)
			{
				this.Injury.ShowInfectNotice(this._selectedItem, 1);
				this.Injury.ShowEatNotice(this._selectedItem, 1);
			}
		}

		// Token: 0x06006136 RID: 24886 RVA: 0x002C8795 File Offset: 0x002C6995
		private void HideEatItemInfectNotice()
		{
			this.Injury.HideNotice(true, true);
		}

		// Token: 0x06006137 RID: 24887 RVA: 0x002C87A8 File Offset: 0x002C69A8
		private void CallRefreshItems(int index)
		{
			CharacterDisplayDataForBeggarUltimate data = this._data[index];
			data.ItemDataList.Clear();
			AsyncMethodCallbackDelegate <>9__3;
			ExtraDomainMethod.AsyncCall.CanExecuteProfessionSkill(this, 9, 3, delegate(int offset, RawDataPool dataPool)
			{
				bool canExecute = false;
				Serializer.Deserialize(dataPool, offset, ref canExecute);
				this._btnList[0] = new ViewPopupMenu.BtnData(LanguageKey.LK_Eat_Item.Tr(), canExecute, EItemMenuDisplayOrder.Eat, new Action(this.OnClickPopupBtnEat), new UnityAction(this.ShowEatItemInfectNotice), new UnityAction(this.HideEatItemInfectNotice), false);
				bool flag = !canExecute;
				if (flag)
				{
					IAsyncMethodRequestHandler <>4__this = this;
					AsyncMethodCallbackDelegate callback;
					if ((callback = <>9__3) == null)
					{
						callback = (<>9__3 = delegate(int offset1, RawDataPool dataPool1)
						{
							int res = 0;
							Serializer.Deserialize(dataPool1, offset1, ref res);
							bool flag2 = ProfessionSkillInvalidType.Contains(res, 16);
							if (flag2)
							{
								string tips = (LanguageKey.LK_UsingMedicine_Tip_SlotMax.Tr() + LanguageKey.LK_Ignore.Tr()).SetColor("brightred");
								this._btnList[0].SetTip(string.Empty, tips);
							}
						});
					}
					ExtraDomainMethod.AsyncCall.CheckBeggarUltimateSpecialCondition(<>4__this, callback);
				}
			});
			CharacterDomainMethod.AsyncCall.GetInventoryItemsByItemType(this, data.CharacterData.CharacterId, 7, delegate(int offset, RawDataPool pool)
			{
				List<ItemDisplayData> itemList = new List<ItemDisplayData>();
				Serializer.Deserialize(pool, offset, ref itemList);
				data.ItemDataList.AddRange(itemList);
			});
			CharacterDomainMethod.AsyncCall.GetInventoryItemsByItemType(this, data.CharacterData.CharacterId, 9, delegate(int offset, RawDataPool pool)
			{
				List<ItemDisplayData> itemList = new List<ItemDisplayData>();
				Serializer.Deserialize(pool, offset, ref itemList);
				data.ItemDataList.AddRange(itemList);
				this.itemScroll.SetItemList(data.ItemDataList);
			});
		}

		// Token: 0x06006138 RID: 24888 RVA: 0x002C8848 File Offset: 0x002C6A48
		private void CustomBuildGroup()
		{
			List<ITradeableContent> sourceSet = new List<ITradeableContent>(this.itemScroll.FilteredData);
			this.itemScroll.AddGroup(0, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Food_0.Tr(), (from d in sourceSet
			where ItemTemplateHelper.GetItemSubType(d.RealKey.ItemType, d.RealKey.TemplateId) == 700
			select d).ToList<ITradeableContent>(), sourceSet, true);
			this.itemScroll.AddGroup(1, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Food_1.Tr(), (from d in sourceSet
			where ItemTemplateHelper.GetItemSubType(d.RealKey.ItemType, d.RealKey.TemplateId) == 701
			select d).ToList<ITradeableContent>(), sourceSet, true);
			this.itemScroll.AddGroup(2, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Food_2.Tr(), (from d in sourceSet
			where ItemTemplateHelper.GetItemSubType(d.RealKey.ItemType, d.RealKey.TemplateId) == 900
			select d).ToList<ITradeableContent>(), sourceSet, true);
			this.itemScroll.AddGroup(3, LanguageKey.LK_CommonSortAndFilter_Filter_Item_Sub_Food_3.Tr(), (from d in sourceSet
			where ItemTemplateHelper.GetItemSubType(d.RealKey.ItemType, d.RealKey.TemplateId) == 901
			select d).ToList<ITradeableContent>(), sourceSet, true);
		}

		// Token: 0x06006139 RID: 24889 RVA: 0x002C8974 File Offset: 0x002C6B74
		private int CompareCharacter(CharacterDisplayDataForBeggarUltimate a, CharacterDisplayDataForBeggarUltimate b)
		{
			return -a.ItemDataList.Count.CompareTo(b.ItemDataList.Count);
		}

		// Token: 0x04004369 RID: 17257
		[SerializeField]
		private InfinityScroll characterScroll;

		// Token: 0x0400436A RID: 17258
		[SerializeField]
		private CButton closeButton;

		// Token: 0x0400436B RID: 17259
		[SerializeField]
		private TextMeshProUGUI charCountLabel;

		// Token: 0x0400436C RID: 17260
		[SerializeField]
		private AttributeAndInjuryDynamic attributeAndInjury;

		// Token: 0x0400436D RID: 17261
		[SerializeField]
		private ItemListScroll itemScroll;

		// Token: 0x0400436E RID: 17262
		private readonly ProfessionSkillArg _skillArg = new ProfessionSkillArg
		{
			ProfessionId = 9,
			SkillId = 39,
			CharId = -1,
			ItemKey = ItemKey.Invalid,
			IsSuccess = true
		};

		// Token: 0x0400436F RID: 17263
		private List<ViewPopupMenu.BtnData> _btnList;

		// Token: 0x04004370 RID: 17264
		private List<CharacterDisplayDataForBeggarUltimate> _data;

		// Token: 0x04004371 RID: 17265
		private int _currCharIndex;

		// Token: 0x04004372 RID: 17266
		private ITradeableContent _selectedItem;
	}
}
