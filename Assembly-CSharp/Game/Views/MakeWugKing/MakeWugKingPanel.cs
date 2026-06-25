using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Coffee.UIExtensions;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SortAndFilter.Item.Apply;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Story;
using GameData.Domains.Story.SectMainStory;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.MakeWugKing
{
	// Token: 0x02000948 RID: 2376
	public class MakeWugKingPanel : MonoBehaviour
	{
		// Token: 0x06007020 RID: 28704 RVA: 0x0033E6A4 File Offset: 0x0033C8A4
		private void Update()
		{
			for (int i = 0; i < this.treeAnimation.transform.childCount; i++)
			{
				Transform flowerLayout = this.treeAnimation.transform.GetChild(i);
				for (int j = 0; j < flowerLayout.childCount; j++)
				{
					GameObject flower = flowerLayout.GetChild(j).gameObject;
					bool flag = !flower.activeSelf;
					if (!flag)
					{
						RectTransform targetRectTrans = flower.transform as RectTransform;
						bool contains = RectTransformUtility.RectangleContainsScreenPoint(targetRectTrans, Input.mousePosition, UIManager.Instance.UiCamera);
						bool flag2 = contains;
						if (flag2)
						{
							this.flowerHover.SetActive(true);
							this.flowerHover.transform.position = targetRectTrans.position;
							RectTransform flowerHoverRectTrans = this.flowerHover.transform as RectTransform;
							flowerHoverRectTrans.sizeDelta = Vector2.Max(new Vector2(66f, 66f), targetRectTrans.sizeDelta);
							return;
						}
					}
				}
			}
			bool activeSelf = this.flowerHover.activeSelf;
			if (activeSelf)
			{
				this.flowerHover.SetActive(false);
				return;
			}
		}

		// Token: 0x06007021 RID: 28705 RVA: 0x0033E7DC File Offset: 0x0033C9DC
		private void OnEnable()
		{
			this.itemPrefab.gameObject.SetActive(false);
			PoolManager.SetSrcObject("ItemPrefab", this.itemPrefab);
			PoolManager.SetSrcObject("eff_makewugking_ui_leidu", this.hotPoisonFlyPrefab);
			PoolManager.SetSrcObject("eff_makewugking_ui_yudu", this.gloomyPoisonFlyPrefab);
			PoolManager.SetSrcObject("eff_makewugking_ui_handu", this.coldPoisonFlyPrefab);
			PoolManager.SetSrcObject("eff_makewugking_ui_chidu", this.redPoisonFlyPrefab);
			PoolManager.SetSrcObject("eff_makewugking_ui_fudu", this.rottenPoisonFlyPrefab);
			PoolManager.SetSrcObject("eff_makewugking_ui_huandu", this.illusoryPoisonFlyPrefab);
			this.RefreshPoisonLayoutAfterShow();
		}

		// Token: 0x06007022 RID: 28706 RVA: 0x0033E87C File Offset: 0x0033CA7C
		private void OnDisable()
		{
			this._lastPoison.Initialize();
			this._curPoison.Initialize();
			this._addPoison.Initialize();
			this._dropItemDict.Clear();
			PoolManager.RemoveData("ItemPrefab");
			PoolManager.RemoveData("eff_makewugking_ui_leidu");
			PoolManager.RemoveData("eff_makewugking_ui_yudu");
			PoolManager.RemoveData("eff_makewugking_ui_handu");
			PoolManager.RemoveData("eff_makewugking_ui_chidu");
			PoolManager.RemoveData("eff_makewugking_ui_fudu");
			PoolManager.RemoveData("eff_makewugking_ui_huandu");
		}

		// Token: 0x06007023 RID: 28707 RVA: 0x0033E907 File Offset: 0x0033CB07
		public void Setup(ViewMakeWugKing parent)
		{
			this.Init();
			this._parent = parent;
		}

		// Token: 0x06007024 RID: 28708 RVA: 0x0033E918 File Offset: 0x0033CB18
		private void Init()
		{
			bool inited = this._inited;
			if (!inited)
			{
				this._inited = true;
				this.itemToggleGroup.Init(-1);
				this.itemToggleGroup.OnActiveIndexChange += this.ItemToggleGroup_OnActiveIndexChange;
				this.btnPut.ClearAndAddListener(new Action(this.OnClickPut));
				this.btnMake.ClearAndAddListener(new Action(this.OnClickMake));
			}
		}

		// Token: 0x06007025 RID: 28709 RVA: 0x0033E990 File Offset: 0x0033CB90
		private void ItemToggleGroup_OnActiveIndexChange(int newIndex, int oldIndex)
		{
			List<ItemDisplayData> inventoryItems;
			this._itemDisplayDataFilteredDic.TryGetValue(newIndex, out inventoryItems);
			this.itemScrollView.SetItemList((inventoryItems == null) ? new List<ItemDisplayData>() : inventoryItems);
			this.itemScrollView.ReRender();
			bool flag = this._selectedMultiplyItemDict.Count > 0;
			if (flag)
			{
				this.RefreshPreviewPoison();
			}
		}

		// Token: 0x06007026 RID: 28710 RVA: 0x0033E9EC File Offset: 0x0033CBEC
		public void OnInit(ArgumentBox _)
		{
			this._lastFlowerStageDict.Clear();
			this._curFlowerStageDict.Clear();
			this.btnPut.interactable = false;
			this._isPut = false;
			this._isInit = false;
			new Dictionary<ItemSourceType, List<ItemDisplayData>>().Add(ItemSourceType.Inventory, this._parent.InventoryItems);
			ItemListScroll.EColumnType columnType = ItemListScroll.EColumnType.IconAndName | ItemListScroll.EColumnType.Amount;
			this.itemScrollView.Init("make_wug_king_item_scroll_view", ESortAndFilterControllerType.Item, true, new Action<ITradeableContent, RowItemLine>(this.OnRenderItem), new Action<ITradeableContent, RowItemLine>(this.OnClickItem), columnType, null, null, null);
			List<ItemSortAndFilter.ItemFilterType> list = new List<ItemSortAndFilter.ItemFilterType>();
			list.Add(ItemSortAndFilter.ItemFilterType.Invalid);
			list.Add(ItemSortAndFilter.ItemFilterType.Material);
			list.Add(ItemSortAndFilter.ItemFilterType.Equip);
			this.PlayRoleAnim(ViewMakeWugKing.RoleAnimName.IdleLoop);
			this.InitFlowerTip();
		}

		// Token: 0x06007027 RID: 28711 RVA: 0x0033EAA5 File Offset: 0x0033CCA5
		public void OnQuickHide()
		{
			this.ClearItemEffect();
			this._selectedMultiplyItemDict.Clear();
		}

		// Token: 0x06007028 RID: 28712 RVA: 0x0033EABC File Offset: 0x0033CCBC
		private void InitFlowerTip()
		{
			for (int i = 0; i < this.treeAnimation.transform.childCount; i++)
			{
				WugKingItem wugKingConfig = WugKing.Instance[i];
				Transform flower = this.treeAnimation.transform.GetChild(i);
				for (int j = 0; j < flower.childCount; j++)
				{
					RectTransform targetRectTrans = flower.GetChild(j) as RectTransform;
					TooltipInvoker tip = targetRectTrans.GetComponent<TooltipInvoker>();
					ItemKey itemKey = new ItemKey(8, 0, wugKingConfig.WugMedicine, -1);
					ItemDisplayData itemData = new ItemDisplayData
					{
						Key = itemKey
					};
					tip.Type = TipType.Medicine;
					tip.RuntimeParam = new ArgumentBox().SetObject("ItemData", itemData);
				}
			}
		}

		// Token: 0x06007029 RID: 28713 RVA: 0x0033EB84 File Offset: 0x0033CD84
		public void OnRenderItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, false);
			bool interactable = !itemData.IsLocked;
			rowItemLine.SetInteractable(interactable, true);
			rowItemLine.SetDisabled(!interactable);
			bool isSelected = this._selectedMultiplyItemDict.Any((KeyValuePair<ItemKey, int> t) => t.Key.Equals(itemData.RealKey));
			rowItemLine.SetSelected(isSelected);
			bool flag = isSelected;
			if (flag)
			{
				rowItemLine.SetDataForContainerDirect(1, string.Format("{0}/{1}", this._selectedMultiplyItemDict[itemData.RealKey], itemData.Amount));
			}
			else
			{
				rowItemLine.SetDataForContainerDirect(1, itemData.Amount.ToString());
			}
		}

		// Token: 0x0600702A RID: 28714 RVA: 0x0033EC68 File Offset: 0x0033CE68
		public void OnClickItem(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			MakeWugKingPanel.<>c__DisplayClass14_0 CS$<>8__locals1 = new MakeWugKingPanel.<>c__DisplayClass14_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.itemData = itemData;
			bool flag = this._selectedMultiplyItemDict.ContainsKey(CS$<>8__locals1.itemData.Key);
			if (flag)
			{
				ValueTuple<ItemKey, int> pair = new ValueTuple<ItemKey, int>(CS$<>8__locals1.itemData.Key, -this._selectedMultiplyItemDict[CS$<>8__locals1.itemData.Key]);
				this._selectedMultiplyItemDict.Remove(CS$<>8__locals1.itemData.Key);
				CS$<>8__locals1.<OnClickItem>g__OnRefresh|0(pair);
			}
			else
			{
				sbyte itemType = CS$<>8__locals1.itemData.RealKey.ItemType;
				sbyte b = itemType;
				if (b != 0)
				{
					if (b != 5)
					{
						throw new Exception(string.Format("UnHandled ItemType: {0}", CS$<>8__locals1.itemData.RealKey.ItemType));
					}
					bool flag2 = CS$<>8__locals1.itemData.Amount == 1;
					if (flag2)
					{
						CS$<>8__locals1.<OnClickItem>g__OnRefresh|0(this.SetSelectedItem(CS$<>8__locals1.itemData.Clone(1)));
					}
					else
					{
						this.itemScrollView.SetItemToSelectCountMode(rowItemLine, delegate(int count)
						{
							ValueTuple<ItemKey, int> pair2 = CS$<>8__locals1.<>4__this.SetSelectedItem(CS$<>8__locals1.itemData.Clone(count));
							base.<OnClickItem>g__OnRefresh|0(pair2);
						}, delegate
						{
							ValueTuple<ItemKey, int> pair2 = CS$<>8__locals1.<>4__this.SetSelectedItem(null);
							base.<OnClickItem>g__OnRefresh|0(pair2);
						}, 0, 0, 1, null, false, null, false);
					}
				}
				else
				{
					CS$<>8__locals1.<OnClickItem>g__OnRefresh|0(this.SetSelectedItem(CS$<>8__locals1.itemData.Clone(-1)));
				}
			}
		}

		// Token: 0x0600702B RID: 28715 RVA: 0x0033EDB8 File Offset: 0x0033CFB8
		[return: TupleElementNames(new string[]
		{
			"data",
			"count"
		})]
		private ValueTuple<ItemKey, int> SetSelectedItem(ITradeableContent itemData)
		{
			this.itemScrollView.SelectedItem = itemData;
			bool flag = itemData == null;
			ValueTuple<ItemKey, int> result;
			if (flag)
			{
				result = new ValueTuple<ItemKey, int>(ItemKey.Invalid, -1);
			}
			else
			{
				int originalAmount = 0;
				bool flag2 = this._selectedMultiplyItemDict.ContainsKey(itemData.Key);
				if (flag2)
				{
					originalAmount = this._selectedMultiplyItemDict[itemData.Key];
				}
				this._selectedMultiplyItemDict[itemData.Key] = itemData.Amount;
				result = new ValueTuple<ItemKey, int>(itemData.Key, itemData.Amount - originalAmount);
			}
			return result;
		}

		// Token: 0x0600702C RID: 28716 RVA: 0x0033EE44 File Offset: 0x0033D044
		public unsafe void OnGotWugJugData()
		{
			this._addPoison.Initialize();
			this._lastPoison.Initialize();
			bool isInit = this._isInit;
			if (isInit)
			{
				for (int i = 0; i < this._parent.WugJugData.Poisons.Count; i++)
				{
					*this._lastPoison[i] += *this._curPoison[i];
				}
			}
			else
			{
				for (int j = 0; j < this._parent.WugJugData.Poisons.Count; j++)
				{
					*this._lastPoison[j] += this._parent.WugJugData.Poisons[j];
				}
			}
			this._curPoison.Initialize();
			for (int k = 0; k < this._parent.WugJugData.Poisons.Count; k++)
			{
				*this._curPoison[k] += this._parent.WugJugData.Poisons[k];
			}
			bool flag = !UIElement.FullScreenMask.IsShowing;
			if (flag)
			{
				this.RefreshPoisonLayoutDisplay();
				this.RefreshTreeAnim();
				this.RefreshFlowerStage(!this._isInit);
			}
			this._isInit = true;
		}

		// Token: 0x0600702D RID: 28717 RVA: 0x0033EF9C File Offset: 0x0033D19C
		private void RefreshTreeAnim()
		{
			int lastTreeStage = MakeWugKingPanel.GetTreeAnimStage(this._lastPoison.Sum());
			int curTreeStage = MakeWugKingPanel.GetTreeAnimStage(this._curPoison.Sum());
			bool flag = curTreeStage > lastTreeStage;
			if (flag)
			{
				this.treeAnimation.AnimationState.Complete -= this.OnTreeAnimCompleted;
				this.treeAnimation.AnimationState.ClearTracks();
				this.treeAnimation.AnimationState.SetAnimation(0, string.Format("tree_{0}_{1}_grow", lastTreeStage, curTreeStage), false);
				this.treeAnimation.AnimationState.Complete += this.OnTreeAnimCompleted;
				AudioManager.Instance.PlaySound("SFX_Wugjug_upgrade", false, false);
			}
			else
			{
				this.OnTreeAnimCompleted(null);
			}
		}

		// Token: 0x0600702E RID: 28718 RVA: 0x0033F06C File Offset: 0x0033D26C
		private static int GetTreeAnimStage(int allPoisonValue)
		{
			int costPoison = GlobalConfig.Instance.WugJugRefiningCostPoison;
			int poisonPercent = allPoisonValue * 100 / costPoison;
			int result = Mathf.FloorToInt((float)poisonPercent / 25f) + 1;
			return Mathf.Clamp(result, 1, 5);
		}

		// Token: 0x0600702F RID: 28719 RVA: 0x0033F0AC File Offset: 0x0033D2AC
		private void OnTreeAnimCompleted(TrackEntry trackEntry)
		{
			this.treeAnimation.AnimationState.Complete -= this.OnTreeAnimCompleted;
			int curTreeStage = MakeWugKingPanel.GetTreeAnimStage(this._curPoison.Sum());
			this.treeAnimation.AnimationState.SetAnimation(0, string.Format("tree_{0}_idle", curTreeStage), true);
		}

		// Token: 0x06007030 RID: 28720 RVA: 0x0033F10C File Offset: 0x0033D30C
		private unsafe void SyncCurPoisonFromWugJugData()
		{
			this._curPoison.Initialize();
			ViewMakeWugKing parent = this._parent;
			IReadOnlyList<int> readOnlyList;
			if (parent == null)
			{
				readOnlyList = null;
			}
			else
			{
				SectWuxianWugJugData wugJugData = parent.WugJugData;
				readOnlyList = ((wugJugData != null) ? wugJugData.Poisons : null);
			}
			IReadOnlyList<int> poisons = readOnlyList;
			bool flag = poisons == null;
			if (!flag)
			{
				for (int i = 0; i < poisons.Count; i++)
				{
					*this._curPoison[i] += poisons[i];
				}
			}
		}

		// Token: 0x06007031 RID: 28721 RVA: 0x0033F17C File Offset: 0x0033D37C
		private void RefreshPoisonLayoutAfterShow()
		{
			bool flag = !this._isInit || this._parent == null;
			if (!flag)
			{
				this.SyncCurPoisonFromWugJugData();
				this._addPoison.Initialize();
				this.RefreshPoisonLayoutDisplay();
				this.btnPut.interactable = (this._selectedMultiplyItemDict.Count > 0);
			}
		}

		// Token: 0x06007032 RID: 28722 RVA: 0x0033F1DC File Offset: 0x0033D3DC
		private void RefreshPoisonLayoutDisplay()
		{
			bool hasSelection = this._selectedMultiplyItemDict.Count > 0;
			this.RefreshMakeResult(hasSelection);
			bool flag = hasSelection;
			if (flag)
			{
				this.RefreshPreviewPoison();
			}
		}

		// Token: 0x06007033 RID: 28723 RVA: 0x0033F210 File Offset: 0x0033D410
		private unsafe void RefreshMakeResult(bool skipPoisonValue = false)
		{
			for (int i = 0; i < 6; i++)
			{
				PoisonItem config = Poison.Instance[i];
				Refers refers = this.poisonLayout.GetChild(i).GetComponent<Refers>();
				bool flag = !skipPoisonValue;
				if (flag)
				{
					refers.CGet<TextMeshProUGUI>("Value").text = CommonUtils.GetDisplayStringForNum(*this._curPoison[i], 100000);
					refers.CGet<GameObject>("HighLight").SetActive(false);
				}
			}
			int allPoisonValue = this._curPoison.Sum();
			int costPoison;
			bool meet = this.RefreshButtonMake(out costPoison);
			string color = meet ? "brightblue" : "brightred";
			this.costLabel.text = allPoisonValue.ToString().SetColor(color) + "/" + costPoison.ToString();
			this.RefreshButtonMakeTip();
			bool isPut = this._isPut;
			if (isPut)
			{
				this.PlayRoleAnim(meet ? ViewMakeWugKing.RoleAnimName.MaterialsMeet : ViewMakeWugKing.RoleAnimName.PutMaterials);
				this._isPut = false;
			}
			else
			{
				bool flag2 = !UIElement.FullScreenMask.IsShowing;
				if (flag2)
				{
					this.PlayRoleAnim(meet ? ViewMakeWugKing.RoleAnimName.IdleAfterMeetLoop : ViewMakeWugKing.RoleAnimName.IdleLoop);
				}
			}
		}

		// Token: 0x06007034 RID: 28724 RVA: 0x0033F340 File Offset: 0x0033D540
		private void RefreshButtonMakeTip()
		{
			int costPoison;
			bool meet = this.RefreshButtonMake(out costPoison);
			bool hasSelection = this._selectedMultiplyItemDict.Count > 0;
			TooltipInvoker tip = this.btnMake.GetComponent<TooltipInvoker>();
			bool flag = hasSelection;
			if (flag)
			{
				tip.enabled = true;
				tip.Type = TipType.SingleDesc;
				bool flag2 = tip.PresetParam == null || tip.PresetParam.Length != 1;
				if (flag2)
				{
					tip.PresetParam = new string[1];
				}
				tip.PresetParam[0] = LocalStringManager.Get(LanguageKey.LK_MakeWugKing_Make_NotPut);
				tip.RuntimeParam = null;
			}
			else
			{
				bool flag3 = meet;
				if (flag3)
				{
					tip.enabled = true;
					for (int type = 0; type < 6; type++)
					{
						this._poisonCostList[type] = 0;
					}
					this._wugKingTemplateId = SectMainStorySharedMethods.CalcWugKingType(this._poisonCostList, this._parent.WugJugData);
					tip.Type = TipType.MakeWugKing;
					tip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("TemplateId", this._wugKingTemplateId).Set("CostPoisonValue", costPoison).Set("AllPoisonValue", this._curPoison.Sum()).SetObject("CostPoison", this._poisonCostList);
				}
				else
				{
					tip.enabled = false;
				}
			}
		}

		// Token: 0x06007035 RID: 28725 RVA: 0x0033F48C File Offset: 0x0033D68C
		private bool RefreshButtonMake(out int costPoison)
		{
			int allPoisonValue = this._curPoison.Sum();
			costPoison = SectMainStorySharedMethods.CalcWugJugRefiningCostPoisonValue(this._parent.WugJugData);
			bool meet = allPoisonValue >= costPoison;
			bool hasEdit = this._addPoison.Sum() > 0;
			this.btnMake.interactable = (meet && !hasEdit);
			this.btnMakeEffect.SetActive(this.btnMake.interactable);
			return meet;
		}

		// Token: 0x06007036 RID: 28726 RVA: 0x0033F504 File Offset: 0x0033D704
		private void RefreshFlowerStage(bool clear)
		{
			bool play = false;
			sbyte i = 0;
			while ((int)i < WugKing.Instance.Count)
			{
				int lastFlowerStage;
				bool flag = this._curFlowerStageDict.TryGetValue(i, out lastFlowerStage);
				if (flag)
				{
					this._lastFlowerStageDict[i] = lastFlowerStage;
				}
				int curFlowerStage = this.GetFlowerStage(i, this._curPoison);
				if (clear)
				{
					this.SetFlowerStage(i, curFlowerStage);
				}
				else
				{
					bool flag2 = curFlowerStage > lastFlowerStage;
					if (flag2)
					{
						UIParticle treeGrowEffect = this.flowerEffectLayout.GetChild((int)i).GetComponent<UIParticle>();
						treeGrowEffect.Play();
						sbyte index = i;
						DOVirtual.DelayedCall(0.2f, delegate
						{
							this.SetFlowerStage(index, curFlowerStage);
						}, true);
						play = true;
					}
				}
				i += 1;
			}
			bool flag3 = play;
			if (flag3)
			{
				AudioManager.Instance.PlaySound("SFX_Wugjug_upgrade", false, false);
			}
		}

		// Token: 0x06007037 RID: 28727 RVA: 0x0033F60C File Offset: 0x0033D80C
		private int GetFlowerStage(sbyte flowerIndex, PoisonInts poison)
		{
			int stage = -1;
			bool flag = flowerIndex < 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool sumIsMeetStage4 = this._wugKingTemplateId == flowerIndex && this.btnMake.interactable;
				bool flag2 = sumIsMeetStage4;
				if (flag2)
				{
					result = 3;
				}
				else
				{
					int sum = poison.Sum();
					bool sumIsMeetStage5 = (float)sum >= (float)GlobalConfig.Instance.WugJugRefiningCostPoison * 0.5f;
					bool sumIsMeetStage6 = sum >= 0;
					WugKingItem config = WugKing.Instance.GetItem(flowerIndex);
					bool flag3 = config.RefiningPoisons.Count == 1;
					if (flag3)
					{
						sbyte type = config.RefiningPoisons.First<sbyte>();
						int value = poison.Get((int)type);
						bool flag4 = value >= 2500 && sumIsMeetStage5;
						if (flag4)
						{
							result = 2;
						}
						else
						{
							bool flag5 = value >= 1500 && sumIsMeetStage6;
							if (flag5)
							{
								result = 1;
							}
							else
							{
								result = 0;
							}
						}
					}
					else
					{
						bool flag6 = config.RefiningPoisons.Count == 3;
						if (flag6)
						{
							foreach (sbyte type2 in config.RefiningPoisons)
							{
								int value2 = poison.Get((int)type2);
								bool flag7 = value2 >= 2000 && sumIsMeetStage5;
								if (flag7)
								{
									stage = ((stage == -1) ? 2 : Mathf.Min(stage, 2));
								}
								else
								{
									bool flag8 = value2 >= 1000 && sumIsMeetStage6;
									if (flag8)
									{
										stage = ((stage == -1) ? 1 : Mathf.Min(stage, 1));
									}
									else
									{
										stage = ((stage == -1) ? 0 : Mathf.Min(stage, 0));
									}
								}
							}
						}
						else
						{
							foreach (sbyte type3 in config.RefiningPoisons)
							{
								int value3 = poison.Get((int)type3);
								bool flag9 = value3 >= 1200 && sumIsMeetStage5;
								if (flag9)
								{
									stage = ((stage == -1) ? 2 : Mathf.Min(stage, 2));
								}
								else
								{
									bool flag10 = value3 >= 800 && sumIsMeetStage6;
									if (flag10)
									{
										stage = ((stage == -1) ? 1 : Mathf.Min(stage, 1));
									}
									else
									{
										stage = Mathf.Min(stage, 0);
									}
								}
							}
						}
						result = Mathf.Max(0, stage);
					}
				}
			}
			return result;
		}

		// Token: 0x06007038 RID: 28728 RVA: 0x0033F880 File Offset: 0x0033DA80
		private void SetFlowerStage(sbyte flowerIndex, int curFlowerStage)
		{
			this._curFlowerStageDict[flowerIndex] = curFlowerStage;
			for (int i = 0; i < this.treeAnimation.transform.childCount; i++)
			{
				bool flag = i == (int)flowerIndex;
				if (flag)
				{
					Transform flower = this.treeAnimation.transform.GetChild(i);
					for (int j = 0; j < flower.transform.childCount; j++)
					{
						flower.GetChild(j).gameObject.SetActive(curFlowerStage == j);
					}
					this.SetFlowerArrow(flowerIndex, curFlowerStage, flower);
				}
			}
		}

		// Token: 0x06007039 RID: 28729 RVA: 0x0033F918 File Offset: 0x0033DB18
		private void SetFlowerArrow(sbyte flowerIndex, int curFlowerStage, Transform flower)
		{
			bool showArrow = curFlowerStage == 3;
			RectTransform trans = this.arrowRoot.transform.GetChild((int)flowerIndex) as RectTransform;
			bool flag = trans == null;
			if (!flag)
			{
				trans.gameObject.SetActive(showArrow);
				bool flag2 = !showArrow;
				if (!flag2)
				{
					int stageIndex = Mathf.Clamp(curFlowerStage, 0, flower.childCount - 1);
					RectTransform targetFlower = flower.GetChild(stageIndex) as RectTransform;
					bool flag3 = targetFlower == null;
					if (!flag3)
					{
						Vector3 localTip = new Vector3(targetFlower.rect.center.x, targetFlower.rect.center.y + 80f, 0f);
						trans.position = targetFlower.TransformPoint(localTip);
					}
				}
			}
		}

		// Token: 0x0600703A RID: 28730 RVA: 0x0033F9E4 File Offset: 0x0033DBE4
		private unsafe void RefreshPreviewPoison()
		{
			bool skipRefreshAddPoison = this._skipRefreshAddPoison;
			if (skipRefreshAddPoison)
			{
				this._skipRefreshAddPoison = false;
			}
			else
			{
				this._addPoison.Initialize();
				List<ItemDisplayData> itemList = EasyPool.Get<List<ItemDisplayData>>();
				foreach (KeyValuePair<ItemKey, int> pair in this._selectedMultiplyItemDict)
				{
					ItemKey data = pair.Key;
					int count = pair.Value;
					for (int i = 0; i < count; i++)
					{
						itemList.Add(this._itemDisplayDataDic[data]);
					}
				}
				PoisonInts poison = SectMainStorySharedMethods.CalcDropPoisonValue(this._parent.WugJugData, itemList);
				this._addPoison.Add(ref poison);
				EasyPool.Free<List<ItemDisplayData>>(itemList);
			}
			for (int j = 0; j < 6; j++)
			{
				int curValue = *this._curPoison[j];
				int addValue = *this._addPoison[j];
				Refers refers = this.poisonLayout.GetChild(j).GetComponent<Refers>();
				TextMeshProUGUI text = refers.CGet<TextMeshProUGUI>("Value");
				GameObject highLight = refers.CGet<GameObject>("HighLight");
				bool flag = addValue > 0;
				if (flag)
				{
					int finalValue = curValue + addValue;
					text.text = CommonUtils.GetDisplayStringForNum(finalValue, 100000).SetColor("brightblue");
					highLight.SetActive(true);
				}
				else
				{
					text.text = CommonUtils.GetDisplayStringForNum(curValue, 100000);
					highLight.SetActive(false);
				}
			}
			int num;
			this.RefreshButtonMake(out num);
		}

		// Token: 0x0600703B RID: 28731 RVA: 0x0033FB9C File Offset: 0x0033DD9C
		private void OnContentChange([TupleElementNames(new string[]
		{
			"data",
			"count"
		})] List<ValueTuple<ItemKey, int>> changeList = null)
		{
			this.btnPut.interactable = (this._selectedMultiplyItemDict.Count > 0);
			this.RefreshPoisonLayoutDisplay();
			this.RefreshDropItem(changeList);
			this.RefreshButtonMakeTip();
		}

		// Token: 0x0600703C RID: 28732 RVA: 0x0033FBD0 File Offset: 0x0033DDD0
		private void RefreshDropItem([TupleElementNames(new string[]
		{
			"data",
			"count"
		})] List<ValueTuple<ItemKey, int>> changeList = null)
		{
			bool flag = changeList != null;
			if (flag)
			{
				foreach (ValueTuple<ItemKey, int> valueTuple in changeList)
				{
					ItemKey itemKey = valueTuple.Item1;
					int amount = valueTuple.Item2;
					Queue<GameObject> queue;
					bool flag2 = !this._dropItemDict.TryGetValue(itemKey, out queue);
					if (flag2)
					{
						queue = new Queue<GameObject>();
						this._dropItemDict[itemKey] = queue;
					}
					int count = Mathf.Clamp(amount, 0, 10);
					bool flag3 = count > 0;
					if (flag3)
					{
						int times = count;
						while (times > 0)
						{
							times--;
							GameObject item = PoolManager.GetObject("ItemPrefab");
							queue.Enqueue(item);
							CImage image = item.GetComponent<CImage>();
							bool flag4 = ItemTemplateHelper.IsJiaoEgg(itemKey.ItemType, itemKey.TemplateId);
							if (flag4)
							{
								image.SetSprite("", false, null);
								JiaoEggView jiaoEggView = SingletonObject.getInstance<ItemViewPool>().Get<JiaoEggView>();
								jiaoEggView.Refresh(this._itemDisplayDataDic[itemKey].JiaoLoongDisplayData, true);
								jiaoEggView.transform.SetParent(item.transform);
								jiaoEggView.transform.localPosition = Vector3.zero;
								jiaoEggView.transform.localScale = 2f * Vector3.one;
							}
							else
							{
								string icon = ItemTemplateHelper.GetIcon(itemKey.ItemType, itemKey.TemplateId);
								image.SetSprite(icon, false, null);
							}
							item.transform.SetParent(this.itemRoot);
							item.transform.localScale = this.itemPrefab.transform.localScale;
							item.transform.position = Vector3.Lerp(this.leftBornPoint.position, this.rightBornPoint.position, Random.value);
							int totalCount = this._dropItemDict.Sum((KeyValuePair<ItemKey, Queue<GameObject>> p) => p.Value.Count);
							bool flag5 = totalCount > 60;
							if (flag5)
							{
								int removeCount = totalCount - 60;
								for (int i = 0; i < removeCount; i++)
								{
									foreach (KeyValuePair<ItemKey, Queue<GameObject>> keyValuePair in this._dropItemDict)
									{
										ItemKey itemKey3;
										Queue<GameObject> queue3;
										keyValuePair.Deconstruct(out itemKey3, out queue3);
										Queue<GameObject> dropItemQueue = queue3;
										bool flag6 = dropItemQueue.Count > 1;
										if (flag6)
										{
											GameObject removeItem = dropItemQueue.Dequeue();
											MakeWugKingPanel.<RefreshDropItem>g__ReturnItem|32_0(removeItem);
											break;
										}
									}
								}
							}
						}
					}
					else
					{
						bool flag7 = count < 0;
						if (flag7)
						{
							int times2 = -count;
							while (times2 > 0)
							{
								times2--;
								GameObject item2 = queue.Dequeue();
								MakeWugKingPanel.<RefreshDropItem>g__ReturnItem|32_0(item2);
							}
						}
					}
				}
			}
			foreach (KeyValuePair<ItemKey, Queue<GameObject>> keyValuePair in this._dropItemDict)
			{
				ItemKey itemKey3;
				Queue<GameObject> queue3;
				keyValuePair.Deconstruct(out itemKey3, out queue3);
				ItemKey itemKey2 = itemKey3;
				Queue<GameObject> queue2 = queue3;
				bool needHide = !this._selectedMultiplyItemDict.ContainsKey(itemKey2);
				bool flag8 = needHide;
				if (flag8)
				{
					while (queue2.Count > 0)
					{
						GameObject item3 = queue2.Dequeue();
						MakeWugKingPanel.<RefreshDropItem>g__ReturnItem|32_0(item3);
					}
				}
			}
		}

		// Token: 0x0600703D RID: 28733 RVA: 0x0033FF94 File Offset: 0x0033E194
		private void PlayRoleAnim(ViewMakeWugKing.RoleAnimName animName)
		{
			this._parent.PlayRoleAnim(animName, new Spine.AnimationState.TrackEntryDelegate(this.OnRoleComplete));
		}

		// Token: 0x0600703E RID: 28734 RVA: 0x0033FFB0 File Offset: 0x0033E1B0
		private void OnRoleComplete(TrackEntry trackEntry)
		{
			ViewMakeWugKing.RoleAnimName curRoleAnimName = this._parent.CurRoleAnimName;
			bool flag = curRoleAnimName == ViewMakeWugKing.RoleAnimName.PutMaterials || curRoleAnimName == ViewMakeWugKing.RoleAnimName.MaterialsMeet;
			if (flag)
			{
				ViewMakeWugKing.RoleAnimName animName = this.btnMake.interactable ? ViewMakeWugKing.RoleAnimName.IdleAfterMeetLoop : ViewMakeWugKing.RoleAnimName.IdleLoop;
				this.PlayRoleAnim(animName);
			}
		}

		// Token: 0x0600703F RID: 28735 RVA: 0x0033FFFC File Offset: 0x0033E1FC
		public void OnClickPut()
		{
			string title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
			string content = LocalStringManager.GetFormat(LanguageKey.LK_Item_PutWarning_Multiply, Array.Empty<object>());
			this.<OnClickPut>g__Action|35_0();
		}

		// Token: 0x06007040 RID: 28736 RVA: 0x00340030 File Offset: 0x0033E230
		private void OnClickMultiplyPutPoisonMaterial()
		{
			Inventory inventory = new Inventory();
			foreach (KeyValuePair<ItemKey, int> operation in this._selectedMultiplyItemDict)
			{
				inventory.OfflineAdd(operation.Key, operation.Value);
			}
			StoryDomainMethod.Call.DropPoisonsToWugJug(-1, inventory);
			ItemDisplayData.ReturnInventoryToPool(inventory);
		}

		// Token: 0x06007041 RID: 28737 RVA: 0x003400AC File Offset: 0x0033E2AC
		private void PlayDropItemAnim()
		{
			bool hasPlayEndSound = false;
			string sound = (Random.Range(0, 2) == 0) ? "SFX_Wugjug_fly_01" : "SFX_Wugjug_fly_02";
			AudioManager.Instance.PlaySound(sound, false, false);
			AudioManager.Instance.PlaySound("SFX_Wugjug_disappear", false, false);
			foreach (KeyValuePair<ItemKey, Queue<GameObject>> keyValuePair in this._dropItemDict)
			{
				ItemKey itemKey2;
				Queue<GameObject> queue2;
				keyValuePair.Deconstruct(out itemKey2, out queue2);
				ItemKey itemKey = itemKey2;
				Queue<GameObject> queue = queue2;
				List<GameObject> list = EasyPool.Get<List<GameObject>>();
				list.AddRange(queue);
				foreach (GameObject go in list)
				{
					go.gameObject.SetActive(false);
					List<sbyte> poisonList = MakeWugKingPanel.GetPoisonTypePoolList(itemKey);
					for (int index = 0; index < poisonList.Count; index++)
					{
						sbyte poisonType = poisonList[index];
						string effectKey = this.GetPoisonFlyEffectKey(poisonType);
						UIParticle uiParticle = PoolManager.GetObject<UIParticle>(effectKey);
						uiParticle.name = effectKey;
						uiParticle.transform.SetParent(this.itemEffectRoot);
						uiParticle.transform.position = go.transform.position;
						uiParticle.Play();
						Refers refers = this.poisonLayout.GetChild((int)poisonType).GetComponent<Refers>();
						RectTransform targetTrans = refers.CGet<CImage>("Icon").transform as RectTransform;
						refers.CGet<GameObject>("HighLight").SetActive(false);
						float delay = (float)(index % 6) * 0.01f;
						uiParticle.transform.DOKill(false);
						uiParticle.transform.DOMove(targetTrans.position, 1f, false).SetDelay(delay).OnComplete(delegate
						{
							AudioManager.Instance.StopSound("SFX_Wugjug_fly_01");
							AudioManager.Instance.StopSound("SFX_Wugjug_fly_02");
							uiParticle.Stop();
							uiParticle.transform.SetParent(null);
							PoolManager.Destroy(effectKey, uiParticle.gameObject);
							bool flag = !hasPlayEndSound;
							if (flag)
							{
								hasPlayEndSound = true;
								AudioManager.Instance.PlaySound("SFX_Wugjug_fly_in", false, false);
							}
						});
					}
					EasyPool.Free<List<sbyte>>(poisonList);
				}
				EasyPool.Free<List<GameObject>>(list);
			}
		}

		// Token: 0x06007042 RID: 28738 RVA: 0x0034033C File Offset: 0x0033E53C
		private string GetPoisonFlyEffectKey(sbyte poisonType)
		{
			if (!true)
			{
			}
			string result;
			switch (poisonType)
			{
			case 0:
				result = "eff_makewugking_ui_leidu";
				break;
			case 1:
				result = "eff_makewugking_ui_yudu";
				break;
			case 2:
				result = "eff_makewugking_ui_handu";
				break;
			case 3:
				result = "eff_makewugking_ui_chidu";
				break;
			case 4:
				result = "eff_makewugking_ui_fudu";
				break;
			case 5:
				result = "eff_makewugking_ui_huandu";
				break;
			default:
				result = "eff_makewugking_ui_leidu";
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007043 RID: 28739 RVA: 0x003403B0 File Offset: 0x0033E5B0
		public static List<sbyte> GetPoisonTypePoolList(ItemKey itemKey)
		{
			MakeWugKingPanel.<>c__DisplayClass39_0 CS$<>8__locals1;
			CS$<>8__locals1.list = EasyPool.Get<List<sbyte>>();
			bool flag = itemKey.ItemType == 5;
			if (flag)
			{
				MaterialItem config = Config.Material.Instance[itemKey.TemplateId];
				MakeWugKingPanel.<GetPoisonTypePoolList>g__AddToList|39_0(config.InnatePoisons, ref CS$<>8__locals1);
			}
			else
			{
				bool flag2 = itemKey.ItemType == 4;
				if (flag2)
				{
					CarrierItem config2 = Carrier.Instance[itemKey.TemplateId];
					MakeWugKingPanel.<GetPoisonTypePoolList>g__AddToList|39_0(config2.InnatePoisons, ref CS$<>8__locals1);
				}
			}
			return CS$<>8__locals1.list;
		}

		// Token: 0x06007044 RID: 28740 RVA: 0x00340436 File Offset: 0x0033E636
		public void OnClickMake()
		{
			this.PlayRoleAnim(ViewMakeWugKing.RoleAnimName.MakeWugLoop);
			StoryDomainMethod.AsyncCall.RefiningWugKing(this._parent, delegate(int offset, RawDataPool dataPool)
			{
				ItemKey itemKey = ItemKey.Invalid;
				Serializer.Deserialize(dataPool, offset, ref itemKey);
				bool flag = !itemKey.IsValid();
				if (!flag)
				{
					MedicineItem medicineConfig = Medicine.Instance[itemKey.TemplateId];
					sbyte wugKingId = 0;
					WugKing.Instance.Iterate(delegate(WugKingItem w)
					{
						bool flag2 = w.WugMedicine == medicineConfig.TemplateId;
						bool result;
						if (flag2)
						{
							wugKingId = w.TemplateId;
							result = false;
						}
						else
						{
							result = true;
						}
						return result;
					});
					UIElement.FullScreenMask.Show();
					DG.Tweening.Sequence sequence = DOTween.Sequence();
					sequence.AppendCallback(delegate
					{
						this.treeStartEffect.gameObject.SetActive(true);
						this.treeStartEffect.Stop();
						this.treeStartEffect.Play();
						AudioManager.Instance.PlaySound("SFX_Wugjug_tree", false, false);
					});
					sequence.AppendInterval(1f);
					sequence.AppendCallback(delegate
					{
						Transform flowerLayout = this.treeAnimation.transform.GetChild((int)wugKingId);
						this.SetFlowerStage(wugKingId, 3);
						Transform targetFlower = flowerLayout.GetChild(3);
						Vector3 targetPos = targetFlower.position;
						this.treeEndEffect.transform.position = targetPos;
						this.treeEndEffect.gameObject.SetActive(true);
						this.treeEndEffect.Stop();
						this.treeEndEffect.Play();
						AudioManager.Instance.PlaySound("SFX_Wugjug_lighting", false, false);
					});
					sequence.AppendInterval(0.3f);
					sequence.AppendCallback(delegate
					{
						this.treeStartEffect.Stop();
						this.treeStartEffect.gameObject.SetActive(false);
					});
					sequence.AppendInterval(1f);
					sequence.AppendCallback(delegate
					{
						this.treeEndEffect.Stop();
						this.treeEndEffect.gameObject.SetActive(false);
						UIElement.FullScreenMask.Hide(false);
						ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
						ItemDisplayData itemData = new ItemDisplayData
						{
							Key = itemKey,
							Amount = 1
						};
						argBox.SetObject("ItemList", new List<ItemDisplayData>
						{
							itemData
						});
						UIElement.GetItem.SetOnInitArgs(argBox);
						UIManager.Instance.MaskUI(UIElement.GetItem);
						DOVirtual.DelayedCall(0.2f, delegate
						{
							this.RefreshMakeResult(false);
							this.RefreshTreeAnim();
							this.RefreshFlowerStage(true);
						}, true);
					});
					sequence.Play<DG.Tweening.Sequence>();
				}
			});
		}

		// Token: 0x06007045 RID: 28741 RVA: 0x0034045C File Offset: 0x0033E65C
		internal void OnGotInventoryItems()
		{
			this._itemDisplayDataDic.Clear();
			this._itemDisplayDataFilteredDic.Clear();
			foreach (ItemDisplayData item in this._parent.InventoryItems)
			{
				List<sbyte> posionTypes = MakeWugKingPanel.GetPoisonTypePoolList(item.Key);
				this._itemDisplayDataDic[item.Key] = item;
				bool flag = posionTypes != null && posionTypes.Count > 0;
				if (flag)
				{
					this.<OnGotInventoryItems>g__AddToDic|41_0(0, item);
				}
				foreach (sbyte poisonType in posionTypes)
				{
					this.<OnGotInventoryItems>g__AddToDic|41_0((int)(poisonType + 1), item);
				}
				EasyPool.Free<List<sbyte>>(posionTypes);
			}
			this.ItemToggleGroup_OnActiveIndexChange(this.itemToggleGroup.GetActiveIndex(), -1);
		}

		// Token: 0x06007046 RID: 28742 RVA: 0x00340570 File Offset: 0x0033E770
		private void ClearItemEffect()
		{
			List<GameObject> itemEffectList = new List<GameObject>();
			for (int i = 0; i < this.itemEffectRoot.childCount; i++)
			{
				Transform child = this.itemEffectRoot.GetChild(i);
				itemEffectList.Add(child.gameObject);
				PoolManager.Destroy(child.name, child.gameObject);
			}
			foreach (GameObject go in itemEffectList)
			{
				go.transform.SetParent(null);
			}
		}

		// Token: 0x06007048 RID: 28744 RVA: 0x003406B0 File Offset: 0x0033E8B0
		[CompilerGenerated]
		internal static void <RefreshDropItem>g__ReturnItem|32_0(GameObject item)
		{
			JiaoEggView jiaoEggView = item.GetComponentInChildren<JiaoEggView>(true);
			bool flag = jiaoEggView;
			if (flag)
			{
				jiaoEggView.transform.SetParent(null);
				SingletonObject.getInstance<ItemViewPool>().Return<JiaoEggView>(jiaoEggView);
			}
			item.transform.SetParent(null);
			PoolManager.Destroy("ItemPrefab", item);
		}

		// Token: 0x06007049 RID: 28745 RVA: 0x00340704 File Offset: 0x0033E904
		[CompilerGenerated]
		private void <OnClickPut>g__Action|35_0()
		{
			this._isPut = true;
			this.PlayDropItemAnim();
			this._skipRefreshAddPoison = true;
			this.OnClickMultiplyPutPoisonMaterial();
			this._selectedMultiplyItemDict.Clear();
			this._parent.RefreshMakePageInfo();
			this.btnPut.interactable = false;
		}

		// Token: 0x0600704A RID: 28746 RVA: 0x00340754 File Offset: 0x0033E954
		[CompilerGenerated]
		internal unsafe static void <GetPoisonTypePoolList>g__AddToList|39_0(PoisonsAndLevels poisonsAndLevels, ref MakeWugKingPanel.<>c__DisplayClass39_0 A_1)
		{
			for (sbyte type = 0; type < 6; type += 1)
			{
				short value = *(ref poisonsAndLevels.Values.FixedElementField + (IntPtr)type * 2);
				bool flag = value > 0;
				if (flag)
				{
					A_1.list.Add(type);
				}
			}
		}

		// Token: 0x0600704F RID: 28751 RVA: 0x00340928 File Offset: 0x0033EB28
		[CompilerGenerated]
		private void <OnGotInventoryItems>g__AddToDic|41_0(int key, ItemDisplayData data)
		{
			bool flag = !this._itemDisplayDataFilteredDic.ContainsKey(key);
			if (flag)
			{
				this._itemDisplayDataFilteredDic[key] = new List<ItemDisplayData>();
			}
			this._itemDisplayDataFilteredDic[key].Add(data);
		}

		// Token: 0x0400531C RID: 21276
		public Dictionary<ItemKey, int> _selectedMultiplyItemDict = new Dictionary<ItemKey, int>();

		// Token: 0x0400531D RID: 21277
		public Dictionary<ItemKey, ItemDisplayData> _itemDisplayDataDic = new Dictionary<ItemKey, ItemDisplayData>();

		// Token: 0x0400531E RID: 21278
		public Dictionary<int, List<ItemDisplayData>> _itemDisplayDataFilteredDic = new Dictionary<int, List<ItemDisplayData>>();

		// Token: 0x0400531F RID: 21279
		private bool _inited = false;

		// Token: 0x04005320 RID: 21280
		private const string ItemPrefabKey = "ItemPrefab";

		// Token: 0x04005321 RID: 21281
		private const string HotPoisonFlyPrefabKey = "eff_makewugking_ui_leidu";

		// Token: 0x04005322 RID: 21282
		private const string GloomyPoisonFlyPrefabKey = "eff_makewugking_ui_yudu";

		// Token: 0x04005323 RID: 21283
		private const string ColdPoisonFlyPrefabKey = "eff_makewugking_ui_handu";

		// Token: 0x04005324 RID: 21284
		private const string RedPoisonFlyPrefabKey = "eff_makewugking_ui_chidu";

		// Token: 0x04005325 RID: 21285
		private const string RottenPoisonFlyPrefabKey = "eff_makewugking_ui_fudu";

		// Token: 0x04005326 RID: 21286
		private const string IllusoryPoisonFlyPrefabKey = "eff_makewugking_ui_huandu";

		// Token: 0x04005327 RID: 21287
		private const int DropItemSingleMaxCount = 10;

		// Token: 0x04005328 RID: 21288
		private const int DropItemTotalMaxCount = 60;

		// Token: 0x04005329 RID: 21289
		[SerializeField]
		private RectTransform flowerEffectLayout;

		// Token: 0x0400532A RID: 21290
		[SerializeField]
		private CButton btnPut;

		// Token: 0x0400532B RID: 21291
		[SerializeField]
		private CButton btnMake;

		// Token: 0x0400532C RID: 21292
		[SerializeField]
		private RectTransform poisonLayout;

		// Token: 0x0400532D RID: 21293
		[SerializeField]
		private TextMeshProUGUI costLabel;

		// Token: 0x0400532E RID: 21294
		[SerializeField]
		private CToggleGroup itemToggleGroup;

		// Token: 0x0400532F RID: 21295
		[SerializeField]
		private ItemListScroll itemScrollView;

		// Token: 0x04005330 RID: 21296
		[SerializeField]
		private SkeletonGraphic treeAnimation;

		// Token: 0x04005331 RID: 21297
		[SerializeField]
		private GameObject btnMakeEffect;

		// Token: 0x04005332 RID: 21298
		[SerializeField]
		private GameObject itemPrefab;

		// Token: 0x04005333 RID: 21299
		[SerializeField]
		private GameObject hotPoisonFlyPrefab;

		// Token: 0x04005334 RID: 21300
		[SerializeField]
		private GameObject gloomyPoisonFlyPrefab;

		// Token: 0x04005335 RID: 21301
		[SerializeField]
		private GameObject coldPoisonFlyPrefab;

		// Token: 0x04005336 RID: 21302
		[SerializeField]
		private GameObject redPoisonFlyPrefab;

		// Token: 0x04005337 RID: 21303
		[SerializeField]
		private GameObject rottenPoisonFlyPrefab;

		// Token: 0x04005338 RID: 21304
		[SerializeField]
		private GameObject illusoryPoisonFlyPrefab;

		// Token: 0x04005339 RID: 21305
		[SerializeField]
		private RectTransform itemRoot;

		// Token: 0x0400533A RID: 21306
		[SerializeField]
		private RectTransform leftBornPoint;

		// Token: 0x0400533B RID: 21307
		[SerializeField]
		private RectTransform rightBornPoint;

		// Token: 0x0400533C RID: 21308
		[SerializeField]
		private ParticleSystem treeStartEffect;

		// Token: 0x0400533D RID: 21309
		[SerializeField]
		private ParticleSystem treeEndEffect;

		// Token: 0x0400533E RID: 21310
		[SerializeField]
		private GameObject flowerHover;

		// Token: 0x0400533F RID: 21311
		[SerializeField]
		private RectTransform itemEffectRoot;

		// Token: 0x04005340 RID: 21312
		[SerializeField]
		private RectTransform arrowRoot;

		// Token: 0x04005341 RID: 21313
		private ViewMakeWugKing _parent;

		// Token: 0x04005342 RID: 21314
		private sbyte _wugKingTemplateId;

		// Token: 0x04005343 RID: 21315
		private readonly Dictionary<sbyte, int> _lastFlowerStageDict = new Dictionary<sbyte, int>();

		// Token: 0x04005344 RID: 21316
		private readonly Dictionary<sbyte, int> _curFlowerStageDict = new Dictionary<sbyte, int>();

		// Token: 0x04005345 RID: 21317
		private PoisonInts _lastPoison;

		// Token: 0x04005346 RID: 21318
		private PoisonInts _curPoison;

		// Token: 0x04005347 RID: 21319
		private PoisonInts _addPoison;

		// Token: 0x04005348 RID: 21320
		private bool _isPut;

		// Token: 0x04005349 RID: 21321
		private bool _isInit;

		// Token: 0x0400534A RID: 21322
		private readonly List<int> _poisonCostList = new List<int>
		{
			1,
			2,
			3,
			4,
			5,
			6
		};

		// Token: 0x0400534B RID: 21323
		private bool _skipRefreshAddPoison;

		// Token: 0x0400534C RID: 21324
		private readonly Dictionary<ItemKey, Queue<GameObject>> _dropItemDict = new Dictionary<ItemKey, Queue<GameObject>>();
	}
}
