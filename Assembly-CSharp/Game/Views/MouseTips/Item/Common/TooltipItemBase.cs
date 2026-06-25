using System;
using System.Collections.Generic;
using System.Text;
using CharacterDataMonitor;
using Config;
using FrameWork;
using Game.Views.MouseTips.Common;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MouseTips.Item.Common
{
	// Token: 0x020008AC RID: 2220
	public class TooltipItemBase : MouseTipBase
	{
		// Token: 0x06006A41 RID: 27201 RVA: 0x0031085C File Offset: 0x0030EA5C
		protected override void Init(ArgumentBox argsBox)
		{
			bool isNew;
			this._isNew = (argsBox.Get("IsNew", out isNew) && isNew);
			this.DisableCompare = (argsBox.Get("DisableCompare", out this.DisableCompare) && this.DisableCompare);
			this.IsShowEatingTime = argsBox.Get("EatingTime", out this.EatingTime);
			argsBox.Get("ShowProfession", out this.ForceShowProfession);
			this._avatarInfoMonitor = ((this._charId >= 0) ? SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AvatarInfoMonitor>(this._charId, false) : null);
			bool fixedPower;
			if (this._avatarInfoMonitor != null)
			{
				CharacterItem config = Character.Instance[this._avatarInfoMonitor.TemplateId];
				if (config != null)
				{
					fixedPower = (((this._itemKey.ItemType == 0) ? config.FixWeaponPower : config.FixArmorPower) != -1);
					goto IL_CA;
				}
			}
			fixedPower = false;
			IL_CA:
			this._fixedPower = fixedPower;
			bool flag = this.rootDetail;
			if (flag)
			{
				this.rootDetail.SetActive(false);
			}
			this.RefreshHotkeyDisplayLockItem();
		}

		// Token: 0x06006A42 RID: 27202 RVA: 0x0031095C File Offset: 0x0030EB5C
		protected void PostInit()
		{
			bool flag = !this.DisableCompare;
			if (flag)
			{
				CharacterDomainMethod.AsyncCall.GetEquipmentKeys(null, this._charId, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref this._currentEquipments);
				});
			}
		}

		// Token: 0x06006A43 RID: 27203 RVA: 0x00310994 File Offset: 0x0030EB94
		public override void Refresh()
		{
			base.Refresh();
			this.HasSelfPoison = this.InnatePoisons.IsNonZero();
			this.HasAttachedPoison = (this._itemData.HasAnyPoison && this._itemData.PoisonIsIdentified);
			this.RefreshCommonArea();
			bool flag = this.poisonArea;
			if (flag)
			{
				this.poisonArea.Refresh(this.InnatePoisons, this._itemData);
			}
			this.RefreshSpecialArea();
			this.InitItemDisableFunctionList();
			this.RefreshOtherArea();
			this.RefreshHoldCount();
			this.RefreshHotkeyDisplayLockItem();
			this.RefreshPoisonDetail();
			this.RefreshMixedPoisonDetail();
			this.RefreshSpecialEffectDetail();
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
			base.SetAllowOverlapLayout(this.IsDetail);
		}

		// Token: 0x06006A44 RID: 27204 RVA: 0x00310A5F File Offset: 0x0030EC5F
		private void Update()
		{
			this.UpdateDetail();
			this.UpdateHotKeyDetail();
			this.CheckHotkeyDisplayViewEncyclopedia();
		}

		// Token: 0x06006A45 RID: 27205 RVA: 0x00310A78 File Offset: 0x0030EC78
		protected virtual void RefreshCommonArea()
		{
			bool templateDataOnly = this._templateDataOnly;
			if (templateDataOnly)
			{
				this.commonArea.Refresh(this._itemKey, true);
			}
			else
			{
				this.commonArea.Refresh(this._itemData, this.IsDetail);
			}
		}

		// Token: 0x06006A46 RID: 27206 RVA: 0x00310AC0 File Offset: 0x0030ECC0
		protected virtual void RefreshSpecialArea()
		{
			bool flag = this.specialArea;
			if (flag)
			{
				this.specialArea.Refresh(this._itemData, (int)this.EatingTime);
			}
		}

		// Token: 0x06006A47 RID: 27207 RVA: 0x00310AF8 File Offset: 0x0030ECF8
		protected void RefreshHoldCount()
		{
			EGameState gameState = GameApp.Instance.GetCurrentGameStateName();
			bool flag = gameState == EGameState.Adventure || gameState == EGameState.InGame;
			if (flag)
			{
				TaiwuDomainMethod.AsyncCall.GetItemCount(this, this._itemData.Key.ItemType, this._itemData.Key.TemplateId, delegate(int offset, RawDataPool dataPool)
				{
					bool flag2 = !base.gameObject;
					if (!flag2)
					{
						int count = 0;
						Serializer.Deserialize(dataPool, offset, ref count);
						bool isNew = this._isNew;
						if (isNew)
						{
							count -= this._itemData.Amount;
						}
						count = Mathf.Max(0, count);
						this.commonArea.RefreshAmount(count);
					}
				});
			}
			else
			{
				this.commonArea.RefreshAmount(0);
			}
		}

		// Token: 0x06006A48 RID: 27208 RVA: 0x00310B64 File Offset: 0x0030ED64
		protected void OnDestroy()
		{
			this._destroyed = true;
		}

		// Token: 0x06006A49 RID: 27209 RVA: 0x00310B70 File Offset: 0x0030ED70
		protected virtual void InitItemDisableFunctionList()
		{
			this._disableFunctionList.Clear();
			ItemKey key = this._itemData.Key;
			bool dissemblable = ItemTemplateHelper.GetCanDisassemble(key.ItemType, key.TemplateId);
			bool flag = !dissemblable;
			if (flag)
			{
				this._disableFunctionList.Add(ItemFunction.Disassemble);
			}
		}

		// Token: 0x06006A4A RID: 27210 RVA: 0x00310BBD File Offset: 0x0030EDBD
		private void RefreshOtherArea()
		{
			this.otherArea.Refresh(this._disableFunctionList);
		}

		// Token: 0x06006A4B RID: 27211 RVA: 0x00310BD4 File Offset: 0x0030EDD4
		protected void UpdateDetail()
		{
			bool flag = this.DisableDetail || (this.HasStick && !this._isInCompareUI);
			if (!flag)
			{
				bool altDown = TipsCommandKit.ViewDetailInfo.Check(this.Element, true, false, false, true, false);
				this.IsDetail = altDown;
				bool flag2 = this.rootDetail && this.rootDetail.activeSelf != this.IsDetail;
				if (flag2)
				{
					this.rootDetail.SetActive(this.IsDetail);
					this.Refresh();
				}
			}
		}

		// Token: 0x06006A4C RID: 27212 RVA: 0x00310C6C File Offset: 0x0030EE6C
		protected void UpdateHotKeyDetail()
		{
			bool disableDetail = this.DisableDetail;
			if (disableDetail)
			{
				this.operationArea.ShowHotkeyDisplayDetail(false);
			}
			else
			{
				this.operationArea.ShowHotkeyDisplayDetail(true);
				bool isDetail = this.IsDetail;
				if (isDetail)
				{
					this.operationArea.RefreshCancelDetail();
				}
				else
				{
					this.operationArea.RefreshPressToDetail();
				}
			}
		}

		// Token: 0x06006A4D RID: 27213 RVA: 0x00310CC5 File Offset: 0x0030EEC5
		protected void RefreshHotkeyDisplayLockItem()
		{
			this.operationArea.RefreshHotkeyDisplayLockItem(this._itemData);
			this.operationArea.RefreshHotkeyDisplayViewEncyclopedia(false);
		}

		// Token: 0x06006A4E RID: 27214 RVA: 0x00310CE8 File Offset: 0x0030EEE8
		private void CheckHotkeyDisplayViewEncyclopedia()
		{
		}

		// Token: 0x06006A4F RID: 27215 RVA: 0x00310CF8 File Offset: 0x0030EEF8
		protected void ForceRebuildLayout(uint delayFrame = 2U, RectTransform rectTransform = null)
		{
			bool destroyed = this._destroyed;
			if (!destroyed)
			{
				bool flag = rectTransform == null;
				if (flag)
				{
					rectTransform = base.GetComponent<RectTransform>();
				}
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(delayFrame, delegate
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
				});
			}
		}

		// Token: 0x06006A50 RID: 27216 RVA: 0x00310D50 File Offset: 0x0030EF50
		public static string GetBonusValue(int baseValue, int finalValue, bool isDetail, Func<int, string> handler = null, string baseColor = "", bool isPercent = false)
		{
			string percentChar = isPercent ? "%" : string.Empty;
			if (isDetail)
			{
				int offset = finalValue - baseValue;
				bool flag = offset != 0;
				if (flag)
				{
					string baseValueStr = (handler == null) ? baseValue.ToString() : handler(baseValue);
					baseValueStr = (baseValueStr + percentChar).SetColor(baseColor);
					string offsetStr = TooltipItemBase.GetOffsetValue(baseValue, finalValue, handler, isPercent);
					return baseValueStr + offsetStr;
				}
			}
			string finalValueStr = (handler == null) ? finalValue.ToString() : handler(finalValue);
			return (finalValueStr + percentChar).SetColor(baseColor);
		}

		// Token: 0x06006A51 RID: 27217 RVA: 0x00310DEC File Offset: 0x0030EFEC
		public static string GetOffsetValue(int baseValue, int finalValue, Func<int, string> handler = null, bool isPercent = false)
		{
			string percentChar = isPercent ? "%" : string.Empty;
			int offset = finalValue - baseValue;
			bool flag = offset != 0;
			string result;
			if (flag)
			{
				string offsetStr = (handler == null) ? string.Format("{0:+0;-0}{1}", offset, percentChar).SetColor("refined") : ((offset > 0) ? ("+" + handler(offset) + percentChar).SetColor("refined") : (handler(offset) + percentChar).SetColor("refined"));
				result = offsetStr;
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		// Token: 0x06006A52 RID: 27218 RVA: 0x00310E81 File Offset: 0x0030F081
		protected string GetBonusValue(int baseValue, int finalValue, Func<int, string> handler = null, string baseColor = "", bool isPercent = false)
		{
			return TooltipItemBase.GetBonusValue(baseValue, finalValue, this.IsDetail, handler, baseColor, isPercent);
		}

		// Token: 0x06006A53 RID: 27219 RVA: 0x00310E98 File Offset: 0x0030F098
		private void RefreshPoisonDetail()
		{
			bool hasPoison = this.HasSelfPoison || this.HasAttachedPoison;
			TooltipItemProperty tooltipItemProperty = this.propertyPoisonDetail;
			if (tooltipItemProperty != null)
			{
				tooltipItemProperty.gameObject.SetActive(hasPoison);
			}
			bool flag = !hasPoison || !this.propertyPoisonDetail;
			if (!flag)
			{
				sbyte itemType = this._itemData.Key.ItemType;
				if (!true)
				{
				}
				LanguageKey languageKey;
				switch (itemType)
				{
				case 0:
					languageKey = LanguageKey.LK_Weapon_Poison_Tips;
					break;
				case 1:
					languageKey = LanguageKey.LK_Armor_Poison_Tips;
					break;
				case 2:
					languageKey = LanguageKey.LK_Accessory_Poison_Tips;
					break;
				default:
					if (itemType != 10)
					{
						languageKey = LanguageKey.LK_ItemTips_Eating_PoisonTime;
					}
					else
					{
						languageKey = LanguageKey.LK_Book_Poison_Tips;
					}
					break;
				}
				if (!true)
				{
				}
				LanguageKey tipKey = languageKey;
				this.propertyPoisonDetail.SetValue(tipKey.Tr());
			}
		}

		// Token: 0x06006A54 RID: 27220 RVA: 0x00310F60 File Offset: 0x0030F160
		private void RefreshMixedPoisonDetail()
		{
			bool isMixed = this.HasAttachedPoison && this._itemData.PoisonEffects.IsThreePoisonsMix();
			TooltipItemProperty tooltipItemProperty = this.propertyMixedPoisonDetail;
			if (tooltipItemProperty != null)
			{
				tooltipItemProperty.gameObject.SetActive(isMixed);
			}
			bool flag = !isMixed || !this.propertyMixedPoisonDetail;
			if (!flag)
			{
				MedicineItem medicineConfig = Medicine.Instance[this._itemData.PoisonEffects.GetMedicineTemplateId()];
				bool showOutCombat = 10 == this._itemData.Key.ItemType || ItemType.IsEatable(this._itemData.Key.ItemType);
				bool showInCombat = ItemType.IsEquipmentItemType(this._itemData.Key.ItemType) || ItemType.IsEatable(this._itemData.Key.ItemType);
				StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
				bool flag2 = showOutCombat;
				if (flag2)
				{
					stringBuilder.AppendLine(medicineConfig.Desc);
				}
				bool flag3 = showInCombat;
				if (flag3)
				{
					stringBuilder.AppendLine(medicineConfig.SpecialEffectDesc);
				}
				this.propertyMixedPoisonDetail.Set("", medicineConfig.Name, stringBuilder.ToString(), true);
				EasyPool.Free<StringBuilder>(stringBuilder);
			}
		}

		// Token: 0x06006A55 RID: 27221 RVA: 0x00311094 File Offset: 0x0030F294
		private void RefreshSpecialEffectDetail()
		{
			bool flag = !this.layoutSpecialEffectDetail;
			if (!flag)
			{
				List<ValueTuple<string, string>> effects = TooltipItemSpecialArea.GetEquipmentEffectInfoList(this._itemData);
				Transform template = this.layoutSpecialEffectDetail.GetChild(0);
				for (int i = 0; i < effects.Count; i++)
				{
					Transform child = (i < this.layoutSpecialEffectDetail.childCount) ? this.layoutSpecialEffectDetail.GetChild(i) : Object.Instantiate<Transform>(template, this.layoutSpecialEffectDetail);
					child.gameObject.SetActive(true);
					TooltipItemProperty property = child.GetComponent<TooltipItemProperty>();
					ValueTuple<string, string> info = effects[i];
					property.Set("", info.Item1, info.Item2, true);
				}
				for (int j = effects.Count; j < this.layoutSpecialEffectDetail.childCount; j++)
				{
					this.layoutSpecialEffectDetail.GetChild(j).gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x04004CA6 RID: 19622
		[Header("顶部内容")]
		[SerializeField]
		protected TooltipItemCommonArea commonArea;

		// Token: 0x04004CA7 RID: 19623
		[Header("含有毒素")]
		[SerializeField]
		protected TooltipItemPoisonArea poisonArea;

		// Token: 0x04004CA8 RID: 19624
		[Header("特殊效果")]
		[SerializeField]
		protected TooltipItemSpecialArea specialArea;

		// Token: 0x04004CA9 RID: 19625
		[Header("底部其他内容")]
		[SerializeField]
		private TooltipItemOtherArea otherArea;

		// Token: 0x04004CAA RID: 19626
		[SerializeField]
		protected TooltipOperationArea operationArea;

		// Token: 0x04004CAB RID: 19627
		[Header("详细模式")]
		[SerializeField]
		private GameObject rootDetail;

		// Token: 0x04004CAC RID: 19628
		[Header("详细模式 含有毒素")]
		[SerializeField]
		private TooltipItemProperty propertyPoisonDetail;

		// Token: 0x04004CAD RID: 19629
		[Header("详细模式 混合毒素")]
		[SerializeField]
		private TooltipItemProperty propertyMixedPoisonDetail;

		// Token: 0x04004CAE RID: 19630
		[Header("详细模式 特殊效果")]
		[SerializeField]
		protected Transform layoutSpecialEffectDetail;

		// Token: 0x04004CAF RID: 19631
		protected ItemKey _itemKey;

		// Token: 0x04004CB0 RID: 19632
		protected ItemDisplayData _itemData;

		// Token: 0x04004CB1 RID: 19633
		protected int _charId;

		// Token: 0x04004CB2 RID: 19634
		private bool _destroyed;

		// Token: 0x04004CB3 RID: 19635
		protected bool IsDetail;

		// Token: 0x04004CB4 RID: 19636
		protected bool _templateDataOnly;

		// Token: 0x04004CB5 RID: 19637
		protected bool HasSelfPoison;

		// Token: 0x04004CB6 RID: 19638
		protected bool HasAttachedPoison;

		// Token: 0x04004CB7 RID: 19639
		protected short EatingTime;

		// Token: 0x04004CB8 RID: 19640
		protected bool IsShowEatingTime;

		// Token: 0x04004CB9 RID: 19641
		protected PoisonsAndLevels InnatePoisons;

		// Token: 0x04004CBA RID: 19642
		protected bool _isInCompareUI = false;

		// Token: 0x04004CBB RID: 19643
		private List<ItemKey> _currentEquipments = null;

		// Token: 0x04004CBC RID: 19644
		protected bool DisableCompare = false;

		// Token: 0x04004CBD RID: 19645
		protected bool DisableDetail = false;

		// Token: 0x04004CBE RID: 19646
		private bool _isNew;

		// Token: 0x04004CBF RID: 19647
		protected readonly List<ItemFunction> _disableFunctionList = new List<ItemFunction>();

		// Token: 0x04004CC0 RID: 19648
		private AvatarInfoMonitor _avatarInfoMonitor;

		// Token: 0x04004CC1 RID: 19649
		protected bool _fixedPower;

		// Token: 0x04004CC2 RID: 19650
		protected bool ForceShowProfession = false;

		// Token: 0x04004CC3 RID: 19651
		private bool _lastCtrlDown = false;

		// Token: 0x04004CC4 RID: 19652
		private float _ctrlDownTime = 0f;
	}
}
