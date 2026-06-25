using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DG.Tweening;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Bottom;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Global;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views
{
	// Token: 0x020006F6 RID: 1782
	public class ViewChoosyResource : UIBase
	{
		// Token: 0x17000A63 RID: 2659
		// (get) Token: 0x06005492 RID: 21650 RVA: 0x00273312 File Offset: 0x00271512
		private bool DisableOperation
		{
			get
			{
				return this.animMask.activeSelf || !base.gameObject.activeSelf;
			}
		}

		// Token: 0x06005493 RID: 21651 RVA: 0x00273334 File Offset: 0x00271534
		public override void OnInit(ArgumentBox argsBox)
		{
			this.buttonChoosy.interactable = false;
			this.buttonChoosy.ClearAndAddListener(new Action(this.OnClickButtonChoosy));
			this.buttonBack.ClearAndAddListener(new Action(this.QuickHide));
			this.buttonCancel.ClearAndAddListener(new Action(this.QuickHide));
			bool flag = argsBox == null || !argsBox.Get<ResourceInts>("targetResources", out this._targetResources);
			if (flag)
			{
				this._targetResources.Initialize();
			}
			this.NeedDataListenerId = true;
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.CallRefresh));
		}

		// Token: 0x06005494 RID: 21652 RVA: 0x002733F1 File Offset: 0x002715F1
		private void Awake()
		{
			this.viewCollectResource.InitChoosy();
			this.InitChoosyResourceItem();
		}

		// Token: 0x06005495 RID: 21653 RVA: 0x00273408 File Offset: 0x00271608
		private unsafe void InitChoosyResourceItem()
		{
			for (sbyte i = 0; i < 6; i += 1)
			{
				sbyte resourceType = i;
				ChoosyResourceItem item = this.choosyResourceItemArray[(int)i];
				item.Init(resourceType, delegate
				{
					this.ChangeMakeResourceCount(resourceType, true, false, false, -1);
					this.RefreshChoosyPage();
				}, delegate
				{
					this.ChangeMakeResourceCount(resourceType, false, false, false, -1);
					this.RefreshChoosyPage();
				}, delegate(string value)
				{
					long tempNumber;
					long.TryParse(value, out tempNumber);
					int amount = *this._resources[(int)resourceType];
					int number = (int)Math.Clamp(tempNumber, 0L, (long)amount);
					int times = number / this._choosyCostUnit;
					int target = times * this._choosyCostUnit;
					this.ChangeMakeResourceCount(resourceType, false, false, false, target);
					this.RefreshChoosyInteractable();
					item.Set(amount, target);
				}, delegate(float value)
				{
					int target = Convert.ToInt32((int)(value * (float)this._choosyCostUnit));
					this.ChangeMakeResourceCount(resourceType, false, false, false, target);
					this.RefreshChoosyInteractable();
					int amount = *this._resources[(int)resourceType];
					item.Set(amount, target);
				});
			}
		}

		// Token: 0x06005496 RID: 21654 RVA: 0x0027348D File Offset: 0x0027168D
		private unsafe void CallRefresh()
		{
			TaiwuDomainMethod.AsyncCall.GetAllResources(this, ItemSourceType.Resources, delegate(int offset, RawDataPool pool)
			{
				ValueTuple<ItemSourceType, ResourceInts> tuple = default(ValueTuple<ItemSourceType, ResourceInts>);
				Serializer.Deserialize(pool, offset, ref tuple);
				this._resources = tuple.Item2;
				this.ShowChoosyPage();
				for (sbyte i = 0; i < 6; i += 1)
				{
					this.ChangeMakeResourceCount(i, true, false, false, *this._targetResources[(int)i]);
				}
				this.Element.ShowAfterRefresh();
				GlobalDomainMethod.Call.InvokeGuidingTrigger(117);
			});
		}

		// Token: 0x06005497 RID: 21655 RVA: 0x002734A8 File Offset: 0x002716A8
		private unsafe void RefreshChoosyInteractable()
		{
			bool isMeet = this._curChoosyResourceValues.Any((int v) => v >= this._choosyCostUnit);
			bool isZero = this._curChoosyResourceValues.All((int v) => v == 0);
			string tipText = LocalStringManager.Get(isMeet ? LanguageKey.LK_Resource_Choosy_Tip : (isZero ? LanguageKey.LK_Resource_Choosy_Zero_Tip : LanguageKey.LK_Resource_Choosy_NotMeet_Tip));
			this.tipButtonChoosy.Type = TipType.SingleDesc;
			this.tipButtonChoosy.PresetParam = new string[]
			{
				tipText
			};
			this.buttonChoosy.interactable = isMeet;
			for (sbyte resourceType = 0; resourceType < 6; resourceType += 1)
			{
				ChoosyResourceItem item = this.choosyResourceItemArray[(int)resourceType];
				int minCount = 0;
				int curCount = this._curChoosyResourceValues[(int)resourceType];
				int maxCount = *this._resources[(int)resourceType] / this._choosyCostUnit * this._choosyCostUnit;
				item.SetButtonMoreInteractable(curCount < maxCount);
				item.SetButtonLessInteractable(curCount > minCount);
			}
		}

		// Token: 0x06005498 RID: 21656 RVA: 0x002735AC File Offset: 0x002717AC
		private unsafe void RefreshChoosyPage()
		{
			for (sbyte resourceType = 0; resourceType < 6; resourceType += 1)
			{
				ChoosyResourceItem item = this.choosyResourceItemArray[(int)resourceType];
				int amount = *this._resources[(int)resourceType];
				int target = this._curChoosyResourceValues[(int)resourceType];
				item.Set(amount, target);
			}
			this.RefreshChoosyInteractable();
		}

		// Token: 0x06005499 RID: 21657 RVA: 0x002735FF File Offset: 0x002717FF
		private void ShowChoosyPage()
		{
			this.viewCollectResource.InitChoosy();
			this.RefreshChoosyPage();
			this.animMask.SetActive(false);
		}

		// Token: 0x0600549A RID: 21658 RVA: 0x00273624 File Offset: 0x00271824
		private void HideChoosyPage()
		{
			for (int index = 0; index < this._curChoosyResourceValues.Length; index++)
			{
				this._curChoosyResourceValues[index] = 0;
			}
			this.RefreshChoosyPage();
			this.viewCollectResource.ClearChoosy();
			EventSystem.current.SetSelectedGameObject(null);
		}

		// Token: 0x0600549B RID: 21659 RVA: 0x00273674 File Offset: 0x00271874
		public override void QuickHide()
		{
			bool activeSelf = this.animMask.gameObject.activeSelf;
			if (!activeSelf)
			{
				base.QuickHide();
				this.HideChoosyPage();
			}
		}

		// Token: 0x0600549C RID: 21660 RVA: 0x002736A8 File Offset: 0x002718A8
		private unsafe void ChangeMakeResourceCount(sbyte resourceType, bool isMore, bool isToEdge = false, bool isToLast = false, int targetResource = -1)
		{
			ViewChoosyResource.<>c__DisplayClass25_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.isMore = isMore;
			CS$<>8__locals1.resourceType = resourceType;
			bool disableOperation = this.DisableOperation;
			if (!disableOperation)
			{
				CS$<>8__locals1.minCount = 0;
				CS$<>8__locals1.curCount = this._curChoosyResourceValues[(int)CS$<>8__locals1.resourceType];
				CS$<>8__locals1.maxCount = *this._resources[(int)CS$<>8__locals1.resourceType] / this._choosyCostUnit * this._choosyCostUnit;
				bool flag = targetResource >= 0;
				if (flag)
				{
					targetResource = Mathf.Clamp(targetResource, CS$<>8__locals1.minCount, CS$<>8__locals1.maxCount);
					bool flag2 = CS$<>8__locals1.curCount == targetResource;
					if (flag2)
					{
						return;
					}
					CS$<>8__locals1.isMore = (CS$<>8__locals1.curCount < targetResource);
					CS$<>8__locals1.minCount = (CS$<>8__locals1.maxCount = targetResource);
					isToEdge = true;
				}
				int moreDiff = Mathf.Max(CS$<>8__locals1.maxCount - CS$<>8__locals1.curCount, 0) / this._choosyCostUnit;
				int lessDiff = Mathf.Max(CS$<>8__locals1.curCount - CS$<>8__locals1.minCount, 0) / this._choosyCostUnit;
				int offset = isToEdge ? (CS$<>8__locals1.isMore ? moreDiff : lessDiff) : 1;
				bool needFast = offset > 100;
				bool flag3 = targetResource >= 0;
				if (flag3)
				{
					bool flag4 = targetResource != this._curChoosyResourceValues[(int)CS$<>8__locals1.resourceType];
					if (flag4)
					{
						this._curChoosyResourceValues[(int)CS$<>8__locals1.resourceType] = targetResource;
						this.viewCollectResource.SetChoosy(CS$<>8__locals1.resourceType, this._curChoosyResourceValues[(int)CS$<>8__locals1.resourceType]);
					}
				}
				else
				{
					bool flag5 = !isToLast;
					if (flag5)
					{
						int change = offset * this._choosyCostUnit;
						this._curChoosyResourceValues[(int)CS$<>8__locals1.resourceType] = (CS$<>8__locals1.isMore ? (CS$<>8__locals1.curCount + change) : (CS$<>8__locals1.curCount - change));
					}
					bool flag6 = needFast || isToLast || offset > 1;
					if (flag6)
					{
						int spriteCount = CS$<>8__locals1.curCount / this._choosyCostUnit;
						int from = isToLast ? 1 : 0;
						int to = isToLast ? (spriteCount + 1) : offset;
						for (int i = from; i < to; i++)
						{
							this.<ChangeMakeResourceCount>g__Tick|25_0(ref CS$<>8__locals1);
						}
					}
					else
					{
						this.<ChangeMakeResourceCount>g__Tick|25_0(ref CS$<>8__locals1);
					}
					this.viewCollectResource.SetChoosy(CS$<>8__locals1.resourceType, this._curChoosyResourceValues[(int)CS$<>8__locals1.resourceType]);
				}
			}
		}

		// Token: 0x0600549D RID: 21661 RVA: 0x00273900 File Offset: 0x00271B00
		private void OnClickButtonChoosy()
		{
			ArgumentBox args = EasyPool.Get<ArgumentBox>();
			args.SetObject("needResources", this._curChoosyResourceValues);
			args.SetObject("onConfirm", new Action(this.ConfirmChoosy));
			UIElement.ChoosyResourceConfirm.SetOnInitArgs(args);
			UIManager.Instance.MaskUI(UIElement.ChoosyResourceConfirm);
		}

		// Token: 0x0600549E RID: 21662 RVA: 0x0027395C File Offset: 0x00271B5C
		private void ConfirmChoosy()
		{
			this.animMask.SetActive(true);
			List<ItemDisplayData> allResults = new List<ItemDisplayData>();
			Action <>9__2;
			TweenCallback <>9__1;
			for (sbyte i = 0; i < 6; i += 1)
			{
				sbyte resourceType = i;
				int amount = this._curChoosyResourceValues[(int)resourceType];
				int count = amount / this._choosyCostUnit;
				this._curChoosyResourceValues[(int)resourceType] = 0;
				float duration;
				this.viewCollectResource.PlayChoosy(resourceType, out duration);
				TaiwuDomainMethod.AsyncCall.ChoosyGetMaterial(this, resourceType, count, delegate(int offset, RawDataPool dataPool)
				{
					List<ItemDisplayData> results = null;
					Serializer.Deserialize(dataPool, offset, ref results);
					bool flag = results != null;
					if (flag)
					{
						allResults.AddRange(results);
					}
					bool flag2 = resourceType == 5;
					if (flag2)
					{
						float duration = duration;
						TweenCallback callback;
						if ((callback = <>9__1) == null)
						{
							callback = (<>9__1 = delegate()
							{
								ArgumentBox args = EasyPool.Get<ArgumentBox>().SetObject("ItemList", allResults);
								UIElement.GetItem.SetOnInitArgs(args);
								UIElement getItem = UIElement.GetItem;
								Delegate onShowed = getItem.OnShowed;
								Action b;
								if ((b = <>9__2) == null)
								{
									b = (<>9__2 = delegate()
									{
										this.animMask.SetActive(false);
										this.viewCollectResource.ClearChoosy();
										this.RefreshChoosyPage();
									});
								}
								getItem.OnShowed = (Action)Delegate.Combine(onShowed, b);
								UIManager.Instance.MaskUI(UIElement.GetItem);
								this.CallRefresh();
							});
						}
						DOVirtual.DelayedCall(duration, callback, true);
					}
				});
			}
		}

		// Token: 0x060054A2 RID: 21666 RVA: 0x00273AB8 File Offset: 0x00271CB8
		[CompilerGenerated]
		private void <ChangeMakeResourceCount>g__Tick|25_0(ref ViewChoosyResource.<>c__DisplayClass25_0 A_1)
		{
			bool disableOperation = this.DisableOperation;
			if (!disableOperation)
			{
				A_1.curCount = (A_1.isMore ? (A_1.curCount + this._choosyCostUnit) : (A_1.curCount - this._choosyCostUnit));
				A_1.curCount = Mathf.Clamp(A_1.curCount, A_1.minCount, A_1.maxCount);
				this._curChoosyResourceValues[(int)A_1.resourceType] = A_1.curCount;
			}
		}

		// Token: 0x04003943 RID: 14659
		[SerializeField]
		private ViewCollectResource viewCollectResource;

		// Token: 0x04003944 RID: 14660
		[SerializeField]
		private GameObject animMask;

		// Token: 0x04003945 RID: 14661
		[SerializeField]
		private CButton buttonChoosy;

		// Token: 0x04003946 RID: 14662
		[SerializeField]
		private TooltipInvoker tipButtonChoosy;

		// Token: 0x04003947 RID: 14663
		[SerializeField]
		private CButton buttonBack;

		// Token: 0x04003948 RID: 14664
		[SerializeField]
		private CButton buttonCancel;

		// Token: 0x04003949 RID: 14665
		[SerializeField]
		private ChoosyResourceItem[] choosyResourceItemArray;

		// Token: 0x0400394A RID: 14666
		private ResourceInts _resources;

		// Token: 0x0400394B RID: 14667
		private ResourceInts _targetResources;

		// Token: 0x0400394C RID: 14668
		private string _curEffectName;

		// Token: 0x0400394D RID: 14669
		private readonly int _choosyCostUnit = GlobalConfig.Instance.ChoosyResourceBaseCost;

		// Token: 0x0400394E RID: 14670
		private readonly int[] _curChoosyResourceValues = new int[6];

		// Token: 0x0400394F RID: 14671
		private const int MinResourceValue = 0;

		// Token: 0x04003950 RID: 14672
		private const int MaxResourceSpriteCount = 100;
	}
}
