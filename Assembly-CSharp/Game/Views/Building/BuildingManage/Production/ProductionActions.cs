using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using Config.ConfigCells.Character;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Building;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.Building.BuildingManage.Production
{
	// Token: 0x02000C17 RID: 3095
	public class ProductionActions : MonoBehaviour, IProductionComponent
	{
		// Token: 0x170010A8 RID: 4264
		// (get) Token: 0x06009D55 RID: 40277 RVA: 0x0049AEAB File Offset: 0x004990AB
		private BuildingModel Model
		{
			get
			{
				return SingletonObject.getInstance<BuildingModel>();
			}
		}

		// Token: 0x06009D56 RID: 40278 RVA: 0x0049AEB4 File Offset: 0x004990B4
		private void Awake()
		{
			this.quickCollect.onClick.AddListener(new UnityAction(this.DoQuickCollect));
			this.quickAddSold.onClick.AddListener(new UnityAction(this.DoQuickAddSold));
			this.quickRemoveSold.onClick.AddListener(new UnityAction(this.DoQuickRemoveSold));
			this.autoSold.onValueChanged.AddListener(new UnityAction<bool>(this.OnAutoSoldChanged));
			this.quickRecruit.onClick.AddListener(new UnityAction(this.DoQuickRecruit));
			this.quickReject.onClick.AddListener(new UnityAction(this.DoQuickReject));
			this.quickSoldSettings.onClick.ResetListener(new Action(this.panel.Show));
		}

		// Token: 0x06009D57 RID: 40279 RVA: 0x0049AF92 File Offset: 0x00499192
		public void Setup(IProductionHandler handler)
		{
			this._handler = handler;
		}

		// Token: 0x06009D58 RID: 40280 RVA: 0x0049AF9C File Offset: 0x0049919C
		public void Refresh()
		{
			ShopEventItem shopEvent = this._handler.ShopEvent;
			this.quickAddSold.gameObject.SetActive(this._handler.IsSold);
			this.quickRemoveSold.gameObject.SetActive(this._handler.IsSold);
			this.autoSold.gameObject.SetActive(this._handler.IsSold);
			bool isSold = this._handler.IsSold;
			if (isSold)
			{
				this.autoSold.SetIsOnWithoutNotify(this._handler.Data.AutoSoldItem);
			}
			this.quickRecruit.gameObject.SetActive(this._handler.IsRecruit);
			this.quickReject.gameObject.SetActive(this._handler.IsRecruit);
			bool flag;
			if (this._handler.Template.FuncType != EBuildingBlockFuncType.People && this._handler.Template.TemplateId != 47)
			{
				short num;
				List<sbyte> list = GameData.Domains.Building.SharedMethods.GetBuildingCanSoldItemTypeList(this._handler.Template, out num);
				flag = (list != null && list.Count > 0);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.quickSoldSettings.gameObject.SetActive(true);
			}
			else
			{
				this.quickSoldSettings.gameObject.SetActive(false);
				this.panel.gameObject.SetActive(false);
			}
			bool isRecruit = this._handler.IsRecruit;
			if (isRecruit)
			{
				IProductionHandler handler = this._handler;
				BuildingEarningsData earningsData = this._handler.Data.EarningsData;
				int? num2;
				if (earningsData == null)
				{
					num2 = null;
				}
				else
				{
					List<IntPair> recruitLevelList = earningsData.RecruitLevelList;
					num2 = ((recruitLevelList != null) ? new int?(recruitLevelList.Count) : null);
				}
				int? num3 = num2;
				bool flag3 = handler.IsAuthorityEnoughToRecruit(num3.GetValueOrDefault());
				if (flag3)
				{
					Selectable selectable = this.quickReject;
					Selectable selectable2 = this.quickRecruit;
					BuildingEarningsData earningsData2 = this._handler.Data.EarningsData;
					List<IntPair> list2 = (earningsData2 != null) ? earningsData2.RecruitLevelList : null;
					selectable.interactable = (selectable2.interactable = (list2 != null && list2.Count > 0));
					this.quickRecruitTip.PresetParam[0] = LanguageKey.LK_Building_AutoRecruit.Tr();
				}
				else
				{
					this.quickReject.interactable = true;
					this.quickRecruit.interactable = false;
					this.quickRecruitTip.PresetParam[0] = LanguageKey.LK_Building_LockOfResource.Tr();
				}
			}
			bool flag4;
			if (!this._handler.IsSold && (shopEvent == null || shopEvent.ResourceGoods < 0))
			{
				List<PresetInventoryItem> list3 = (shopEvent != null) ? shopEvent.ItemList : null;
				if (list3 == null || list3.Count <= 0)
				{
					List<sbyte> list = (shopEvent != null) ? shopEvent.ItemGradeProbList : null;
					if (list == null || list.Count <= 0)
					{
						flag4 = this._handler.IsEntertain;
						goto IL_2B9;
					}
				}
			}
			flag4 = true;
			IL_2B9:
			bool anyItem = flag4;
			this.quickCollect.gameObject.SetActive(anyItem);
			Selectable selectable3 = this.quickCollect;
			bool interactable;
			if (this._handler.IsEntertain)
			{
				if (this._handler.Data.Feast.Gift.Values.Any((ItemKey x) => x.HasTemplate))
				{
					interactable = true;
					goto IL_47D;
				}
			}
			BuildingEarningsData data = this._handler.Data.EarningsData;
			if (data != null)
			{
				List<ItemKey> collectionItemList = data.CollectionItemList;
				int num4;
				if (collectionItemList == null)
				{
					num4 = 0;
				}
				else
				{
					num4 = collectionItemList.Count((ItemKey x) => x.HasTemplate);
				}
				List<IntPair> collectionResourceList = data.CollectionResourceList;
				int num5;
				if (collectionResourceList == null)
				{
					num5 = 0;
				}
				else
				{
					num5 = collectionResourceList.Count((IntPair x) => x.Second >= 0);
				}
				int num6 = num4 + num5;
				List<IntPair> recruitLevelList2 = data.RecruitLevelList;
				int num7;
				if (recruitLevelList2 == null)
				{
					num7 = 0;
				}
				else
				{
					num7 = recruitLevelList2.Count((IntPair x) => x.Second >= 0);
				}
				int num8 = num6 + num7;
				List<ItemKey> fixBookInfoList = data.FixBookInfoList;
				int num9;
				if (fixBookInfoList == null)
				{
					num9 = 0;
				}
				else
				{
					num9 = fixBookInfoList.Count((ItemKey x) => x.HasTemplate);
				}
				int num10 = num8 + num9;
				List<IntPair> shopSoldItemEarnList = data.ShopSoldItemEarnList;
				int num11;
				if (shopSoldItemEarnList == null)
				{
					num11 = 0;
				}
				else
				{
					num11 = shopSoldItemEarnList.Count((IntPair x) => x.Second >= 0);
				}
				int num12 = num10 + num11;
				int num13;
				if (!this._handler.IsSold)
				{
					List<ItemKey> shopSoldItemList = data.ShopSoldItemList;
					if (shopSoldItemList == null)
					{
						num13 = 0;
					}
					else
					{
						num13 = shopSoldItemList.Count((ItemKey x) => x.HasTemplate);
					}
				}
				else
				{
					num13 = 0;
				}
				interactable = (num12 + num13 > 0);
			}
			else
			{
				interactable = false;
			}
			IL_47D:
			selectable3.interactable = interactable;
			bool flag5 = this._handler.Data.EarningsData != null;
			if (flag5)
			{
				data = this._handler.Data.EarningsData;
			}
		}

		// Token: 0x06009D59 RID: 40281 RVA: 0x0049B459 File Offset: 0x00499659
		private static int SelectValue(ItemKey key)
		{
			return ItemTemplateHelper.GetBaseValue(key.ItemType, key.TemplateId);
		}

		// Token: 0x06009D5A RID: 40282 RVA: 0x0049B46C File Offset: 0x0049966C
		private static string SelectName(ItemKey key)
		{
			string itemName = ItemTemplateHelper.GetName(key.ItemType, key.TemplateId);
			sbyte grade = ItemTemplateHelper.GetGrade(key.ItemType, key.TemplateId);
			return itemName.SetGradeColor((int)grade);
		}

		// Token: 0x06009D5B RID: 40283 RVA: 0x0049B4AC File Offset: 0x004996AC
		private void DoQuickCollect()
		{
			bool isEntertain = this._handler.IsEntertain;
			if (isEntertain)
			{
				this.DoQuickCollectEntertain();
			}
			else
			{
				BuildingEarningsData earningsData = this._handler.Data.EarningsData;
				List<ItemKey> list = (earningsData != null) ? earningsData.CollectionItemList : null;
				bool flag = list == null || list.Count <= 0;
				if (flag)
				{
					BuildingEarningsData earningsData2 = this._handler.Data.EarningsData;
					List<IntPair> list2 = (earningsData2 != null) ? earningsData2.ShopSoldItemEarnList : null;
					bool flag2 = list2 != null && list2.Count > 0;
					if (flag2)
					{
						this.DoQuickCollectShop();
					}
					else
					{
						BuildingEarningsData earningsData3 = this._handler.Data.EarningsData;
						list2 = ((earningsData3 != null) ? earningsData3.CollectionResourceList : null);
						bool flag3 = list2 != null && list2.Count > 0;
						if (flag3)
						{
							this.DoQuickCollectCollectionResourceList();
						}
					}
				}
				else
				{
					bool flag4 = this._handler.TemplateId == 222;
					if (flag4)
					{
						this.DoQuickCollectPawnshop();
					}
					else
					{
						this.DoQuickCollectNormal();
					}
				}
			}
		}

		// Token: 0x06009D5C RID: 40284 RVA: 0x0049B5B0 File Offset: 0x004997B0
		private void DoQuickCollectEntertain()
		{
			foreach (KeyValuePair<int, ItemKey> keyValuePair in this._handler.Data.Feast.Gift)
			{
				int num;
				ItemKey itemKey2;
				keyValuePair.Deconstruct(out num, out itemKey2);
				ItemKey itemKey = itemKey2;
				bool hasTemplate = itemKey.HasTemplate;
				if (hasTemplate)
				{
					int[] index = (from x in this._handler.Data.Feast.Gift
					where x.Value.HasTemplate
					select x.Key).ToArray<int>();
					ItemDomainMethod.AsyncCall.GetItemDisplayDataListOptional(this._handler.Async, (from x in index
					select this._handler.Data.Feast.Gift[x]).ToList<ItemKey>(), SingletonObject.getInstance<BasicGameData>().TaiwuCharId, 1, false, delegate(int offset, RawDataPool pool)
					{
						List<ItemDisplayData> itemList = new List<ItemDisplayData>();
						Serializer.Deserialize(pool, offset, ref itemList);
						int idx = index.Length;
						while (idx-- > 0)
						{
							itemList[idx].Amount = this._handler.Data.Feast.GiftCount.GetValueOrDefault(idx);
						}
						this._handler.ShowGetItemView(itemList);
					});
					ExtraDomainMethod.Call.FeastReceiveGift(this._handler.Key, -1);
					this._handler.Reload();
					this._handler.DataChanged = true;
					break;
				}
			}
		}

		// Token: 0x06009D5D RID: 40285 RVA: 0x0049B730 File Offset: 0x00499930
		private void DoQuickCollectPawnshop()
		{
			List<ItemKey> items = this._handler.Data.EarningsData.CollectionItemList;
			int totalCost = items.Sum(new Func<ItemKey, int>(ProductionActions.SelectValue));
			string itemNameStr = string.Join(LanguageKey.LK_Separator.Tr(), items.Select(new Func<ItemKey, string>(ProductionActions.SelectName)));
			ConfirmDialogCmd cmd = new ConfirmDialogCmd
			{
				Title = LanguageKey.Lk_Building_PawnShop_GetItem_Title.Tr(),
				ContentUpper = LanguageKey.Lk_Building_PawnShop_GetItem_Content1.TrFormat(itemNameStr),
				ContentLower = LanguageKey.Lk_Building_PawnShop_GetItem_Content2.Tr(),
				ConfirmDialogCost = new List<ConfirmDialogCost>
				{
					new ConfirmDialogCost
					{
						Type = EConfirmDialogCostType.Money,
						ValueCost = totalCost,
						ValueHave = this.Model.GetResourceCount(6)
					}
				},
				Yes = new Action(this.DoQuickCollectNormal)
			};
			UIElement.ConfirmDialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
			UIManager.Instance.MaskUI(UIElement.ConfirmDialog);
		}

		// Token: 0x06009D5E RID: 40286 RVA: 0x0049B83C File Offset: 0x00499A3C
		private void DoQuickCollectShop()
		{
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this._handler.ShowGetItemView((from x in this._handler.Data.EarningsData.ShopSoldItemEarnList
			where x.First != -1
			select ItemDisplayData.CreateResource((sbyte)x.First, x.Second, taiwuCharId)).ToList<ItemDisplayData>());
			BuildingDomainMethod.Call.ShopBuildingSoldItemReceiveQuick(this._handler.Key);
			this._handler.Reload();
			this._handler.DataChanged = true;
		}

		// Token: 0x06009D5F RID: 40287 RVA: 0x0049B8E8 File Offset: 0x00499AE8
		private void DoQuickCollectCollectionResourceList()
		{
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			this._handler.ShowGetItemView((from x in this._handler.Data.EarningsData.CollectionResourceList
			where x.First != -1
			select ItemDisplayData.CreateResource((sbyte)x.First, x.Second, taiwuCharId)).ToList<ItemDisplayData>());
			BuildingDomainMethod.Call.AcceptBuildingBlockCollectEarningQuick(this._handler.Key, false);
			this._handler.Reload();
			this._handler.DataChanged = true;
		}

		// Token: 0x06009D60 RID: 40288 RVA: 0x0049B994 File Offset: 0x00499B94
		private void DoQuickCollectNormal()
		{
			ItemDomainMethod.AsyncCall.GetItemDisplayDataListOptional(this._handler.Async, this._handler.Data.EarningsData.CollectionItemList, delegate(int offset, RawDataPool pool)
			{
				List<ItemDisplayData> itemList = new List<ItemDisplayData>();
				Serializer.Deserialize(pool, offset, ref itemList);
				this._handler.ShowGetItemView(itemList);
			});
			bool flag = this._handler.TemplateId == 222;
			if (flag)
			{
				int index = this._handler.Data.EarningsData.CollectionItemList.Count;
				while (index-- > 0)
				{
					BuildingDomainMethod.Call.AcceptBuildingBlockCollectEarning(-1, this._handler.Key, index, false, index == 0, true);
				}
			}
			else
			{
				BuildingDomainMethod.Call.AcceptBuildingBlockCollectEarningQuick(this._handler.Key, false);
			}
			this._handler.Reload();
			this._handler.DataChanged = true;
		}

		// Token: 0x06009D61 RID: 40289 RVA: 0x0049BA59 File Offset: 0x00499C59
		private void DoQuickAddSold()
		{
			BuildingDomainMethod.Call.QuickAddShopSoldItem(this._handler.Key);
			this._handler.Reload();
		}

		// Token: 0x06009D62 RID: 40290 RVA: 0x0049BA79 File Offset: 0x00499C79
		private void DoQuickRemoveSold()
		{
			BuildingDomainMethod.Call.QuickRemoveShopSoldItem(this._handler.Key);
			this._handler.Reload();
		}

		// Token: 0x06009D63 RID: 40291 RVA: 0x0049BA99 File Offset: 0x00499C99
		private void OnAutoSoldChanged(bool newValue)
		{
			BuildingDomainMethod.Call.SetBuildingAutoSold(this._handler.BlockIndex, newValue);
			this._handler.Reload();
		}

		// Token: 0x06009D64 RID: 40292 RVA: 0x0049BABC File Offset: 0x00499CBC
		private void DoQuickRecruit()
		{
			BuildingEarningsData earningsData = this._handler.Data.EarningsData;
			List<IntPair> list = (earningsData != null) ? earningsData.RecruitLevelList : null;
			bool flag = list == null || list.Count <= 0;
			if (!flag)
			{
				string content = LanguageKey.LK_Building_RecruitPeople_Dialog_Text.Tr().ColorReplace();
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
				{
					Title = LanguageKey.LK_Building_RecruitPeople_Dialog_Title.Tr().ColorReplace(),
					Content = ((this._handler.TemplateId == 223) ? (content + "\n" + LanguageKey.LK_Building_RecruitPeople_Dialog_NeedAuthority.TrFormat(this._handler.AuthorityToRecruit(this._handler.Data.EarningsData.RecruitLevelList.Count))) : content),
					Type = 1,
					Yes = new Action(this.DoQuickRecruitConfirm)
				}));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}

		// Token: 0x06009D65 RID: 40293 RVA: 0x0049BBCC File Offset: 0x00499DCC
		private void DoQuickRecruitConfirm()
		{
			BuildingDomainMethod.AsyncCall.AcceptBuildingBlockRecruitPeopleQuick(this._handler.Async, this._handler.Key, new AsyncMethodCallbackDelegate(this.DoQuickRecruitHandler));
			this._handler.Reload();
			this._handler.DataChanged = true;
		}

		// Token: 0x06009D66 RID: 40294 RVA: 0x0049BC1C File Offset: 0x00499E1C
		private void DoQuickRecruitHandler(int offset, RawDataPool pool)
		{
			List<int> charIdList = null;
			Serializer.Deserialize(pool, offset, ref charIdList);
			this._handler.ShowGetPeopleView(charIdList);
		}

		// Token: 0x06009D67 RID: 40295 RVA: 0x0049BC44 File Offset: 0x00499E44
		private void DoQuickReject()
		{
			BuildingEarningsData earningsData = this._handler.Data.EarningsData;
			List<IntPair> list = (earningsData != null) ? earningsData.RecruitLevelList : null;
			bool flag = list == null || list.Count <= 0;
			if (!flag)
			{
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.LK_Building_QuickRejectRecruitPeople_Cmd_Title).ColorReplace(),
					Content = LocalStringManager.Get(LanguageKey.LK_Building_QuickRejectRecruitPeople_Cmd_Text).ColorReplace(),
					Type = 1,
					Yes = new Action(this.DoQuickRejectConfirm)
				}));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		}

		// Token: 0x06009D68 RID: 40296 RVA: 0x0049BCF9 File Offset: 0x00499EF9
		private void DoQuickRejectConfirm()
		{
			BuildingDomainMethod.Call.RejectBuildingBlockRecruitPeopleQuick(this._handler.Key);
			this._handler.Reload();
			this._handler.DataChanged = true;
		}

		// Token: 0x040079E3 RID: 31203
		[SerializeField]
		private CButton quickCollect;

		// Token: 0x040079E4 RID: 31204
		[SerializeField]
		private CButton quickAddSold;

		// Token: 0x040079E5 RID: 31205
		[SerializeField]
		private CButton quickRemoveSold;

		// Token: 0x040079E6 RID: 31206
		[SerializeField]
		private CButton quickRecruit;

		// Token: 0x040079E7 RID: 31207
		[SerializeField]
		private CButton quickReject;

		// Token: 0x040079E8 RID: 31208
		[SerializeField]
		private CButton quickSoldSettings;

		// Token: 0x040079E9 RID: 31209
		[SerializeField]
		private AutoSoldSettingPanel panel;

		// Token: 0x040079EA RID: 31210
		[SerializeField]
		private TooltipInvoker quickRecruitTip;

		// Token: 0x040079EB RID: 31211
		[SerializeField]
		private CToggle autoSold;

		// Token: 0x040079EC RID: 31212
		private IProductionHandler _handler;
	}
}
