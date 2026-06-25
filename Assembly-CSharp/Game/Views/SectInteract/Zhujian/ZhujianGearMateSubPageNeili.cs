using System;
using System.Collections.Generic;
using System.Text;
using Coffee.UIExtensions;
using DG.Tweening;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using UnityEngine;

namespace Game.Views.SectInteract.Zhujian
{
	// Token: 0x020009D4 RID: 2516
	public class ZhujianGearMateSubPageNeili : ZhujianGearMateSubPage
	{
		// Token: 0x17000D84 RID: 3460
		// (get) Token: 0x06007A80 RID: 31360 RVA: 0x0038E25A File Offset: 0x0038C45A
		private static int BloodPlaneFactor
		{
			get
			{
				return ItemTemplateHelper.GetGearMateUpgradeProgress(12, 17);
			}
		}

		// Token: 0x17000D85 RID: 3461
		// (get) Token: 0x06007A81 RID: 31361 RVA: 0x0038E265 File Offset: 0x0038C465
		private static int GearMateMaxTotalNeiliAllocation
		{
			get
			{
				return (int)(GlobalConfig.Instance.MaxExtraNeiliAllocation * 4);
			}
		}

		// Token: 0x06007A82 RID: 31362 RVA: 0x0038E273 File Offset: 0x0038C473
		public override void Init(ViewZhujianGearMate parent)
		{
			base.Init(parent);
			this.InitButton();
			this.InitNeiliAllocationTypes();
			this.InitItemSelector();
		}

		// Token: 0x06007A83 RID: 31363 RVA: 0x0038E293 File Offset: 0x0038C493
		private void OnDisable()
		{
			this.bloodPlane.DOKill(false);
			this.bloodPlane.anchoredPosition = this.bloodPlane.anchoredPosition.SetY(217f);
			this.triangleImage.SetActive(false);
		}

		// Token: 0x06007A84 RID: 31364 RVA: 0x0038E2D1 File Offset: 0x0038C4D1
		protected override void OnShowDataRequest()
		{
			this.itemSelector.ClearAllSelections(true);
			this.RequestData();
			this.RefreshButton();
			base.SetContentReady();
		}

		// Token: 0x06007A85 RID: 31365 RVA: 0x0038E2F8 File Offset: 0x0038C4F8
		public override void SetGearMateId(int gearMateId)
		{
			bool flag = this.GearMateId == gearMateId;
			if (!flag)
			{
				base.SetGearMateId(gearMateId);
				bool isVisible = this.IsVisible;
				if (isVisible)
				{
					this.RequestGearMateData();
					this.itemSelector.ClearAllSelections(false);
				}
			}
		}

		// Token: 0x06007A86 RID: 31366 RVA: 0x0038E33D File Offset: 0x0038C53D
		private void RequestData()
		{
			this.RequestItemData();
			this.RequestGearMateData();
		}

		// Token: 0x06007A87 RID: 31367 RVA: 0x0038E34E File Offset: 0x0038C54E
		private void RequestItemData()
		{
			TaiwuDomainMethod.AsyncCall.CanTransferItemToWarehouse(null, delegate(int offset, RawDataPool dataPool)
			{
				bool res = false;
				Serializer.Deserialize(dataPool, offset, ref res);
				ZhujianGearMateSubPageNeili._rule.OnlyFromInventory = !res;
				this.itemSelector.SetSourceToggleInteractable(1, res);
				this.itemSelector.SetSourceToggleInteractable(2, res);
				bool flag = !res && this._currentSourceIndex > 0;
				if (flag)
				{
					this.itemSelector.SetSourceToggle(0);
				}
				TaiwuDomainMethod.AsyncCall.GetAllItemsForSelect(null, ZhujianGearMateSubPageNeili._rule, delegate(int offset2, RawDataPool dataPool2)
				{
					SelectItemDisplayData data = new SelectItemDisplayData();
					Serializer.Deserialize(dataPool2, offset2, ref data);
					this.RefreshItemList(data);
				});
			});
		}

		// Token: 0x06007A88 RID: 31368 RVA: 0x0038E364 File Offset: 0x0038C564
		private void RequestGearMateData()
		{
			ExtraDomainMethod.AsyncCall.GetGearMateById(null, this.GearMateId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._data);
				this.RefreshProgress();
				this.RefreshBtnTips();
				this.RefreshNeiliAllocationTypes();
			});
		}

		// Token: 0x06007A89 RID: 31369 RVA: 0x0038E380 File Offset: 0x0038C580
		private void InitButton()
		{
			this.btnConfirm.ClearAndAddListener(new Action(this.OnConfirm));
		}

		// Token: 0x06007A8A RID: 31370 RVA: 0x0038E39B File Offset: 0x0038C59B
		private void InitNeiliAllocationTypes()
		{
			this.neiliAllocationTypes.Init(new Action<byte>(this.OnClickAdd), new Action<byte>(this.OnClickMinus), null, null, null);
		}

		// Token: 0x06007A8B RID: 31371 RVA: 0x0038E3C8 File Offset: 0x0038C5C8
		private void InitItemSelector()
		{
			this.itemSelector.Init();
			this.itemSelector.SetTitle("LK_GearMateNeili_Neili_Select_Item");
			this.itemSelector.OnSelectionChanged += this.OnChangeSelection;
			this.itemSelector.OnSourceTypeChanged += this.OnChangeSourceType;
			this.itemSelector.OnItemSelected += this.OnSelectItem;
			this.itemSelector.GetMaxSelectableCount = new Func<ItemDisplayData, int>(this.GetItemMaxSelectableCount);
			this.itemSelector.SetSortAndFilterVisible(false);
		}

		// Token: 0x06007A8C RID: 31372 RVA: 0x0038E460 File Offset: 0x0038C660
		private void RefreshItemList(SelectItemDisplayData data)
		{
			this._items.Clear();
			bool flag = data != null;
			if (flag)
			{
				bool flag2 = data.InventoryItems != null;
				if (flag2)
				{
					foreach (ItemDisplayData displayData in data.InventoryItems)
					{
						bool flag3 = ZhujianGearMateSubPageNeili._canUseItems.Contains(displayData.RealKey.TemplateId);
						if (flag3)
						{
							this._items.Add(displayData);
						}
					}
				}
				bool flag4 = data.WarehouseItems != null;
				if (flag4)
				{
					foreach (ItemDisplayData displayData2 in data.WarehouseItems)
					{
						bool flag5 = ZhujianGearMateSubPageNeili._canUseItems.Contains(displayData2.RealKey.TemplateId);
						if (flag5)
						{
							this._items.Add(displayData2);
						}
					}
				}
				bool flag6 = data.TreasuryItems != null;
				if (flag6)
				{
					foreach (ItemDisplayData displayData3 in data.TreasuryItems)
					{
						bool flag7 = ZhujianGearMateSubPageNeili._canUseItems.Contains(displayData3.RealKey.TemplateId);
						if (flag7)
						{
							this._items.Add(displayData3);
						}
					}
				}
			}
			this.itemSelector.SetItems(this._items);
		}

		// Token: 0x06007A8D RID: 31373 RVA: 0x0038E600 File Offset: 0x0038C800
		private void RefreshProgress()
		{
			int preview = this.GetPreviewValue();
			float currPercent = this.GetNeiliPercent((float)this._data.Neili);
			float previewPercent = this.GetNeiliPercent((float)preview);
			int currValue = Math.Min(this._data.Neili, 33572);
			float currAllocationPercent;
			int currCount = this.GetNeiliAllocationData(this._data.Neili, out currAllocationPercent);
			float previewAllocationPercent;
			int previewCount = this.GetNeiliAllocationData(this._data.Neili + preview, out previewAllocationPercent) - currCount;
			bool flag = previewAllocationPercent < currAllocationPercent;
			if (flag)
			{
				currAllocationPercent = 0f;
			}
			else
			{
				previewAllocationPercent -= currAllocationPercent;
			}
			this.neiliProgress.Set(currPercent, previewPercent, currValue, preview, 33572);
			this.neiliAllocationProgress.Set(currAllocationPercent, previewAllocationPercent, currCount, previewCount, ZhujianGearMateSubPageNeili.GearMateMaxTotalNeiliAllocation);
		}

		// Token: 0x06007A8E RID: 31374 RVA: 0x0038E6C0 File Offset: 0x0038C8C0
		private unsafe void RefreshNeiliAllocationTypes()
		{
			short usedCount = this._data.NeiliAllocation.GetTotal();
			float num;
			int canUseCount = this.GetNeiliAllocationData(this._data.Neili, out num);
			for (byte i = 0; i < 4; i += 1)
			{
				short value = *this._data.NeiliAllocation[(int)i];
				bool canAdd = value < GlobalConfig.Instance.MaxExtraNeiliAllocation && (int)usedCount < canUseCount;
				bool canMinus = value > 0;
				this.neiliAllocationTypes.Set(i, (int)value, (int)GlobalConfig.Instance.MaxExtraNeiliAllocation, false);
				this.neiliAllocationTypes.SetCanInteract(i, canAdd, canMinus);
				this.neiliAllocationTypes.transform.GetChild((int)i).GetComponent<ComponentNeiliAllocationType>().btnAdd.GetComponent<TooltipInvoker>().enabled = ((int)usedCount >= canUseCount);
			}
		}

		// Token: 0x06007A8F RID: 31375 RVA: 0x0038E798 File Offset: 0x0038C998
		private void RefreshButton()
		{
			this.btnConfirm.interactable = (this.itemSelector.SelectedItems.Count > 0 && !this._isAnimating);
		}

		// Token: 0x06007A90 RID: 31376 RVA: 0x0038E7C8 File Offset: 0x0038C9C8
		private void RefreshBtnTips()
		{
			string name = NameCenter.GetMonasticTitleOrDisplayName(this.ParentView.DisplayData, false);
			float num;
			int canUseCount = this.GetNeiliAllocationData(this._data.Neili, out num);
			TooltipInvoker tips = this.btnConfirm.GetComponent<TooltipInvoker>();
			StringBuilder sb = new StringBuilder();
			short usedCount = this._data.NeiliAllocation.GetTotal();
			sb.AppendLine(LanguageKey.UI_MouseTipGearMateUpgradeNeili_Desc0.TrFormat(name));
			sb.AppendLine(LanguageKey.UI_MouseTipGearMateUpgradeNeili_Desc1.TrFormat(name));
			sb.AppendLine(LanguageKey.UI_MouseTipGearMateUpgradeNeili_Desc2.TrFormat(name));
			sb.AppendLine(LanguageKey.UI_MouseTipGearMateUpgradeNeili_Desc3.TrFormat(canUseCount));
			sb.AppendLine(LanguageKey.UI_MouseTipGearMateUpgradeNeili_Desc4.TrFormat(canUseCount - (int)usedCount));
			tips.PresetParam = new string[]
			{
				LanguageKey.UI_MouseTipGearMateUpgradeNeili_Title.Tr(),
				sb.ToString()
			};
		}

		// Token: 0x06007A91 RID: 31377 RVA: 0x0038E8A9 File Offset: 0x0038CAA9
		private void OnClickAdd(byte i)
		{
			ExtraDomainMethod.Call.UpgradeGearMate(this._data.Id, (sbyte)(13 + i), ItemKey.Invalid, 1);
			this.RequestGearMateData();
		}

		// Token: 0x06007A92 RID: 31378 RVA: 0x0038E8CF File Offset: 0x0038CACF
		private void OnClickMinus(byte i)
		{
			ExtraDomainMethod.Call.UpgradeGearMate(this._data.Id, (sbyte)(13 + i), ItemKey.Invalid, -1);
			this.RequestGearMateData();
		}

		// Token: 0x06007A93 RID: 31379 RVA: 0x0038E8F8 File Offset: 0x0038CAF8
		private void OnSelectItem(ItemKey key)
		{
			bool isDropping = this._isDropping;
			if (!isDropping)
			{
				this._isDropping = true;
				GameObject itemPrefab = Object.Instantiate<GameObject>(this.dropPrefab, this.dropPrefab.transform.parent);
				RectTransform rectTransform = itemPrefab.GetComponent<RectTransform>();
				rectTransform.anchoredPosition = rectTransform.anchoredPosition.SetX(Random.Range(-75f, 75f));
				itemPrefab.GetComponent<ZhujianGearMateDropItem>().OnTrigger += delegate()
				{
					this._isDropping = false;
					this.burstParticle.gameObject.SetActive(true);
					this.burstParticle.Play();
					this.OnChangeSelection();
				};
				itemPrefab.GetComponent<CImage>().SetSprite(ItemTemplateHelper.GetIcon(key.ItemType, key.TemplateId), false, null);
				itemPrefab.SetActive(true);
			}
		}

		// Token: 0x06007A94 RID: 31380 RVA: 0x0038E9A0 File Offset: 0x0038CBA0
		private void OnChangeSelection()
		{
			bool isDropping = this._isDropping;
			if (!isDropping)
			{
				int previewValue = this.GetPreviewValue();
				float height = Math.Min((float)previewValue / (float)(previewValue + ZhujianGearMateSubPageNeili.BloodPlaneFactor), 1f) * 71f;
				this.burstParticle.gameObject.SetActive(previewValue > 0);
				this.bloodPlane.DOKill(false);
				this.bloodPlane.DOAnchorPosY(height + 217f, 1.5f, false);
				this.triangleImage.SetActive(previewValue > 0);
				this.RefreshProgress();
				this.RefreshButton();
			}
		}

		// Token: 0x06007A95 RID: 31381 RVA: 0x0038EA36 File Offset: 0x0038CC36
		private void OnChangeSourceType(int sourceIndex)
		{
			this._currentSourceIndex = sourceIndex;
			this.RefreshButton();
		}

		// Token: 0x06007A96 RID: 31382 RVA: 0x0038EA48 File Offset: 0x0038CC48
		private static ItemSourceType ResolveItemSourceType(int sourceIndex)
		{
			if (!true)
			{
			}
			ItemSourceType result;
			switch (sourceIndex)
			{
			case 0:
				result = ItemSourceType.Inventory;
				break;
			case 1:
				result = ItemSourceType.Warehouse;
				break;
			case 2:
				result = ItemSourceType.Treasury;
				break;
			default:
				result = ItemSourceType.Inventory;
				break;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06007A97 RID: 31383 RVA: 0x0038EA88 File Offset: 0x0038CC88
		private void OnConfirm()
		{
			bool flag = this.itemSelector.SelectedItems.Count == 0 || this.GearMateId < 0;
			if (!flag)
			{
				this.itemSelector.SetBlocked(true);
				this.PlayUpgradeAnim(delegate
				{
					foreach (ValueTuple<ItemKey, int, ItemSourceType> valueTuple in this.itemSelector.GetAllSelectedItemsWithSource())
					{
						ItemKey key = valueTuple.Item1;
						int amount = valueTuple.Item2;
						ItemSourceType sourceType = valueTuple.Item3;
						ExtraDomainMethod.Call.UpgradeGearMate(this.GearMateId, 11, key, amount, sourceType);
					}
					this.triangleImage.SetActive(false);
					this.roundImages.SetActive(false);
					this.itemSelector.ClearAllSelections(false);
					this.itemSelector.SetBlocked(false);
					this._isAnimating = false;
					this.ParentView.SetChangeButtonInteractable(true);
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, new Action(this.RequestData));
				});
			}
		}

		// Token: 0x06007A98 RID: 31384 RVA: 0x0038EADC File Offset: 0x0038CCDC
		private void PlayUpgradeAnim(Action onComplete)
		{
			this._isAnimating = true;
			this.ParentView.SetChangeButtonInteractable(false);
			this.RefreshButton();
			this.machine.AnimationState.SetAnimation(0, "move", false);
			AudioManager.Instance.PlaySound("SFX_GearMate_machine_loop", false, false);
			this.bloodPlane.DOKill(false);
			this.bloodPlane.DOAnchorPosY(217f, (this.bloodPlane.anchoredPosition.y - 217f) / 71f * 2f, false);
			int preview = this.GetPreviewValue();
			int currValue = Math.Min(this._data.Neili, 33572);
			int maxValue = Math.Min(this._data.Neili + preview, 33572);
			DOVirtual.Float(0f, (float)(maxValue - currValue), 2f, delegate(float stepValue)
			{
				int greenLeft = (int)((float)currValue + stepValue);
				int blueLeft = (int)((float)maxValue - stepValue);
				float currAllocationPercent;
				int greenRight = this.GetNeiliAllocationData(greenLeft, out currAllocationPercent);
				float previewAllocationPercent;
				int blueRight = this.GetNeiliAllocationData(blueLeft, out previewAllocationPercent) - greenRight;
				bool flag = previewAllocationPercent < currAllocationPercent;
				if (flag)
				{
					currAllocationPercent = 0f;
				}
				else
				{
					previewAllocationPercent -= currAllocationPercent;
				}
				this.neiliProgress.Set(this.GetNeiliPercent((float)greenLeft), this.GetNeiliPercent((float)blueLeft), greenLeft, blueLeft, 33572);
				this.neiliAllocationProgress.Set(currAllocationPercent, previewAllocationPercent, greenRight, blueRight, ZhujianGearMateSubPageNeili.GearMateMaxTotalNeiliAllocation);
			}).SetAutoKill(true);
			int randomValue = Random.Range(0, 3);
			this.ParentView.ShowBubble(LocalStringManager.Get(string.Format("LK_GearMateNeili_SpeakWord{0}", randomValue)), 2f);
			this.ParentView.DoGearMateAnimation("break_1");
			this.roundImages.SetActive(true);
			SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(2f, onComplete);
		}

		// Token: 0x06007A99 RID: 31385 RVA: 0x0038EC49 File Offset: 0x0038CE49
		private float GetNeiliPercent(float value)
		{
			return value / 33572f;
		}

		// Token: 0x06007A9A RID: 31386 RVA: 0x0038EC54 File Offset: 0x0038CE54
		private int GetPreviewValue()
		{
			int res = 0;
			foreach (KeyValuePair<ItemKey, int> keyValuePair in this.itemSelector.SelectedItems)
			{
				ItemKey itemKey;
				int num;
				keyValuePair.Deconstruct(out itemKey, out num);
				ItemKey key = itemKey;
				int amount = num;
				res += ItemTemplateHelper.GetGearMateUpgradeProgress(key.ItemType, key.TemplateId) * amount;
			}
			return Math.Min(99999, this._data.Neili + res) - this._data.Neili;
		}

		// Token: 0x06007A9B RID: 31387 RVA: 0x0038ECFC File Offset: 0x0038CEFC
		private int GetNeiliAllocationData(int neili, out float percent)
		{
			short cost = GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatio;
			int canUseCount = 0;
			while (neili > (int)cost)
			{
				neili -= (int)cost;
				canUseCount++;
				bool flag = canUseCount % 4 == 0;
				if (flag)
				{
					cost += GlobalConfig.Instance.ExtraNeiliAllocationFromProgressRatioGrowth;
				}
			}
			percent = ((canUseCount == ZhujianGearMateSubPageNeili.GearMateMaxTotalNeiliAllocation) ? 1f : ((float)neili / (float)cost));
			return canUseCount;
		}

		// Token: 0x06007A9C RID: 31388 RVA: 0x0038ED64 File Offset: 0x0038CF64
		private int GetItemMaxSelectableCount(ItemDisplayData item)
		{
			return (this._data == null) ? 0 : Math.Max(0, Math.Min(item.Amount, (99999 - this._data.Neili) / ItemTemplateHelper.GetGearMateUpgradeProgress(item.RealKey.ItemType, item.RealKey.TemplateId)));
		}

		// Token: 0x04005CD4 RID: 23764
		[SerializeField]
		protected GameObject dropPrefab;

		// Token: 0x04005CD5 RID: 23765
		[SerializeField]
		protected RectTransform bloodPlane;

		// Token: 0x04005CD6 RID: 23766
		[SerializeField]
		protected GameObject triangleImage;

		// Token: 0x04005CD7 RID: 23767
		[SerializeField]
		protected GameObject roundImages;

		// Token: 0x04005CD8 RID: 23768
		[SerializeField]
		protected UIParticle burstParticle;

		// Token: 0x04005CD9 RID: 23769
		[SerializeField]
		protected SkeletonGraphic machine;

		// Token: 0x04005CDA RID: 23770
		[SerializeField]
		protected ZhujianGearMateNeili neiliProgress;

		// Token: 0x04005CDB RID: 23771
		[SerializeField]
		protected ZhujianGearMateNeili neiliAllocationProgress;

		// Token: 0x04005CDC RID: 23772
		[SerializeField]
		protected NeiliAllocationTypes neiliAllocationTypes;

		// Token: 0x04005CDD RID: 23773
		[SerializeField]
		protected CButton btnConfirm;

		// Token: 0x04005CDE RID: 23774
		[SerializeField]
		protected ZhujianGearMateItemSelector itemSelector;

		// Token: 0x04005CDF RID: 23775
		private const float AnimDuration = 2f;

		// Token: 0x04005CE0 RID: 23776
		private const float BloodPlaneLowPos = 217f;

		// Token: 0x04005CE1 RID: 23777
		private const float BloodPlaneHighPos = 288f;

		// Token: 0x04005CE2 RID: 23778
		private const float DropPosLeft = -75f;

		// Token: 0x04005CE3 RID: 23779
		private const float DropPosRight = 75f;

		// Token: 0x04005CE4 RID: 23780
		private const float BloodPlaneDuration = 1.5f;

		// Token: 0x04005CE5 RID: 23781
		private const int GearMateMaxDisplayNeili = 33572;

		// Token: 0x04005CE6 RID: 23782
		private const int GearMateMaxNeili = 99999;

		// Token: 0x04005CE7 RID: 23783
		private static List<short> _canUseItems = new List<short>
		{
			9,
			10,
			11,
			12,
			13,
			14,
			15,
			16,
			17
		};

		// Token: 0x04005CE8 RID: 23784
		private static SelectItemRules _rule = new SelectItemRules
		{
			ItemSubType = 1200
		};

		// Token: 0x04005CE9 RID: 23785
		private List<ItemDisplayData> _items = new List<ItemDisplayData>();

		// Token: 0x04005CEA RID: 23786
		private GearMate _data;

		// Token: 0x04005CEB RID: 23787
		private int _currentSourceIndex = 0;

		// Token: 0x04005CEC RID: 23788
		private bool _isDropping;

		// Token: 0x04005CED RID: 23789
		private bool _isAnimating;
	}
}
