using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Character;
using Game.Components.ListStyleGeneralScroll.Item;
using Game.Components.SkeletonAnim;
using Game.Components.SortAndFilter.Item.Apply;
using GameData.Domains.Character;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000BA2 RID: 2978
	public class ViewCharacterMenuEquip : UI_CharacterMenuSubPageBase
	{
		// Token: 0x17000FEC RID: 4076
		// (get) Token: 0x060093A6 RID: 37798 RVA: 0x0044C670 File Offset: 0x0044A870
		public override bool ShowBaseAttribute
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060093A7 RID: 37799 RVA: 0x0044C673 File Offset: 0x0044A873
		public override bool CheckState(ECharacterSubToggleBase curSubTogglePage, ECharacterSubPage curSubPage)
		{
			return curSubTogglePage == ECharacterSubToggleBase.EquipmentBase;
		}

		// Token: 0x17000FED RID: 4077
		// (get) Token: 0x060093A8 RID: 37800 RVA: 0x0044C679 File Offset: 0x0044A879
		public override LanguageKey TitleKey
		{
			get
			{
				return LanguageKey.LK_CharacterMenu_Title_Equip;
			}
		}

		// Token: 0x060093A9 RID: 37801 RVA: 0x0044C680 File Offset: 0x0044A880
		private string GetEquipSoundName(sbyte slotIndex, ItemKey itemKey, bool isLoad)
		{
			sbyte itemType = itemKey.ItemType;
			if (!true)
			{
			}
			sbyte b;
			switch (itemType)
			{
			case 0:
				b = Weapon.Instance[itemKey.TemplateId].ResourceType;
				break;
			case 1:
				b = Armor.Instance[itemKey.TemplateId].ResourceType;
				break;
			case 2:
				b = Accessory.Instance[itemKey.TemplateId].ResourceType;
				break;
			default:
				b = -1;
				break;
			}
			if (!true)
			{
			}
			sbyte resourceType = b;
			if (!true)
			{
			}
			string text;
			switch (resourceType)
			{
			case 1:
				text = "wood";
				break;
			case 2:
				text = "iron";
				break;
			case 3:
				text = "jewelry";
				break;
			case 4:
				text = "fabric";
				break;
			default:
				text = null;
				break;
			}
			if (!true)
			{
			}
			string material = text;
			bool flag = material == null;
			string result;
			if (flag)
			{
				result = (isLoad ? this.soundEffectEquipOn : this.soundEffectEquipOff);
			}
			else
			{
				if (!true)
				{
				}
				switch (slotIndex)
				{
				case 0:
				case 1:
				case 2:
					text = "weapon";
					goto IL_13C;
				case 3:
				case 5:
				case 6:
				case 7:
					text = "cloth";
					goto IL_13C;
				case 8:
				case 9:
				case 10:
					text = "treasure";
					goto IL_13C;
				}
				text = null;
				IL_13C:
				if (!true)
				{
				}
				string slotType = text;
				bool flag2 = slotType == null;
				if (flag2)
				{
					result = (isLoad ? this.soundEffectEquipOn : this.soundEffectEquipOff);
				}
				else
				{
					result = string.Concat(new string[]
					{
						"ui_equipment_",
						material,
						"_",
						slotType,
						"_",
						isLoad ? "load" : "unload"
					});
				}
			}
			return result;
		}

		// Token: 0x17000FEE RID: 4078
		// (get) Token: 0x060093AA RID: 37802 RVA: 0x0044C832 File Offset: 0x0044AA32
		private bool CanShowCharacterPreview
		{
			get
			{
				return this.charDisplayTogGroup.GetActiveIndex() == 1;
			}
		}

		// Token: 0x17000FEF RID: 4079
		// (get) Token: 0x060093AB RID: 37803 RVA: 0x0044C842 File Offset: 0x0044AA42
		private bool CanShowCharacterCarrier
		{
			get
			{
				return this.charDisplayTogGroup.GetActiveIndex() == 2;
			}
		}

		// Token: 0x17000FF0 RID: 4080
		// (get) Token: 0x060093AC RID: 37804 RVA: 0x0044C852 File Offset: 0x0044AA52
		private bool CharacterCarrierIsCarrier
		{
			get
			{
				return this._currSlot == 11 || (this.charDisplayTogGroup.GetActiveIndex() == 2 && this.showCarrierGroup.GetActiveIndex() == 0 && this._currSlot != 12);
			}
		}

		// Token: 0x17000FF1 RID: 4081
		// (get) Token: 0x060093AD RID: 37805 RVA: 0x0044C88C File Offset: 0x0044AA8C
		private bool CharacterCarrierIsLivestockCarrier
		{
			get
			{
				return this._currSlot == 12 || (this.charDisplayTogGroup.GetActiveIndex() == 2 && this.showCarrierGroup.GetActiveIndex() != 0 && this._currSlot != 11);
			}
		}

		// Token: 0x17000FF2 RID: 4082
		// (get) Token: 0x060093AE RID: 37806 RVA: 0x0044C8C6 File Offset: 0x0044AAC6
		private bool CanShowCharacterAvatar
		{
			get
			{
				return this.charDisplayTogGroup.GetActiveIndex() == 0;
			}
		}

		// Token: 0x060093AF RID: 37807 RVA: 0x0044C8D6 File Offset: 0x0044AAD6
		private void TriggerChangeCharacterClothingEvent()
		{
			GEvent.OnEvent(UiEvents.OnChangeCharacterClothing, EasyPool.Get<ArgumentBox>().Set("CharacterId", base.CharacterMenu.CurCharacterId));
			this.RequestCharDisplayData();
		}

		// Token: 0x060093B0 RID: 37808 RVA: 0x0044C90C File Offset: 0x0044AB0C
		private void InitToggleGroup()
		{
			this.equipPlanTogGroup.Init(-1);
			this.equipPlanTogGroup.OnActiveIndexChange += delegate(int togNew, int togOld)
			{
				bool flag = togNew < 0;
				if (!flag)
				{
					TaiwuDomainMethod.Call.SwitchEquipmentPlan(togNew);
					this.TriggerChangeCharacterClothingEvent();
				}
			};
			this.showCarrierGroup.Init(-1);
			this.showCarrierGroup.Set(WorldMapModel.UsingCarrierFirst ? 0 : 1, false);
			this.showCarrierGroup.OnActiveIndexChange += delegate(int togNew, int togOld)
			{
				bool flag = togNew < 0;
				if (flag)
				{
					bool flag2 = togOld == 0;
					if (flag2)
					{
						this.showCarrierGroup.Set(1, false);
					}
					else
					{
						bool flag3 = togOld == 1;
						if (flag3)
						{
							this.showCarrierGroup.Set(0, false);
						}
					}
				}
				else
				{
					WorldMapModel.UsingCarrierFirst = (togNew == 0);
					SingletonObject.getInstance<WorldMapModel>().HandlerDataCharacterEquipments((from x in this._equipItems
					select x.Key).ToArray<ItemKey>());
					this.travelAnimation.Set(this._charDisplayData, this._equipItems, this.NormalEquipmentTemplateId, true);
				}
			};
			this.hideGroup.Init();
			this.hideGroup.OnActiveIndexChange += delegate(int togNew, int togOld)
			{
				bool flag = ViewCharacterMenuEquip.HideableEquipmentSlot.CheckIndex(togOld);
				if (flag)
				{
					TaiwuDomainMethod.Call.AddHideSkeletonEquipSlot(ViewCharacterMenuEquip.HideableEquipmentSlot[togOld]);
				}
				bool flag2 = ViewCharacterMenuEquip.HideableEquipmentSlot.CheckIndex(togNew);
				if (flag2)
				{
					TaiwuDomainMethod.Call.RemoveHideSkeletonEquipSlot(ViewCharacterMenuEquip.HideableEquipmentSlot[togNew]);
				}
				this.RefreshHideStatus();
			};
			this.pageSwitchTogGroup.Init(this.subPage.gameObject.activeSelf ? 1 : 0);
			this.SetToggleColor(this.subPage.gameObject.activeSelf ? 1 : 0);
			this.pageSwitchTogGroup.OnActiveIndexChange += delegate(int togNew, int togOld)
			{
				bool flag = togNew < 0;
				if (!flag)
				{
					this.subPage.gameObject.SetActive(togNew == 1);
					this.mainPage.gameObject.SetActive(togNew != 1);
					this.SetToggleColor(togNew);
				}
			};
			this.charDisplayTogGroup.Init(-1);
			this.charDisplayTogGroup.OnActiveIndexChange += delegate(int togNew, int togOld)
			{
				this.RefreshAnim(togNew);
			};
			this.equipSwitchTogGroup.Init(-1);
			this.equipSwitchTogGroup.OnActiveIndexChange += delegate(int togNew, int togOld)
			{
				bool flag = togNew < 0;
				if (flag)
				{
					this.HideDetailPanel();
				}
				else
				{
					bool flag2 = this._currSlot < 0;
					if (flag2)
					{
						ItemDisplayData itemDisplayData = this._equipItems[togNew];
						bool flag3 = itemDisplayData != null && itemDisplayData.Key.IsValid();
						if (flag3)
						{
							this.ShowPopupMenu(togNew);
						}
						else
						{
							this.OpenDetailPanel(togNew);
						}
					}
					else
					{
						this.OpenDetailPanel(togNew);
					}
				}
			};
		}

		// Token: 0x060093B1 RID: 37809 RVA: 0x0044CA44 File Offset: 0x0044AC44
		private void SetToggleColor(int togNew)
		{
			bool flag = togNew != 1;
			if (flag)
			{
				this.mainLayout.preferredWidth = this.activeSize;
				this.subLayout.preferredWidth = this.inactiveSize;
				this.subHover.enabled = (this.mainImg.enabled = false);
				this.mainHover.enabled = (this.subImg.enabled = true);
				this.mainText.text = LanguageKey.LK_CharacterMenuEquip_MainEquip.Tr().SetColor("0d0d0d");
				this.subText.text = LanguageKey.LK_CharacterMenuEquip_SubEquip.Tr();
				this.TrySetcharDisplayTogGroup(1);
			}
			else
			{
				this.mainLayout.preferredWidth = this.inactiveSize;
				this.subLayout.preferredWidth = this.activeSize;
				this.subHover.enabled = (this.mainImg.enabled = true);
				this.mainHover.enabled = (this.subImg.enabled = false);
				this.mainText.text = LanguageKey.LK_CharacterMenuEquip_MainEquip.Tr();
				this.subText.text = LanguageKey.LK_CharacterMenuEquip_SubEquip.Tr().SetColor("0d0d0d");
			}
		}

		// Token: 0x060093B2 RID: 37810 RVA: 0x0044CB98 File Offset: 0x0044AD98
		private void InitToggles()
		{
			this.autoEquip.onClick.AddListener(new UnityAction(this.OnAutoEquipButtonClicked));
			this.lockChangeEquipTog.onValueChanged.ResetListener(delegate(bool isOn)
			{
				bool flag = isOn == this._manualChangeEquipCharIds.Contains(base.CharacterMenu.CurCharacterId);
				if (!flag)
				{
					if (isOn)
					{
						TaiwuDomainMethod.Call.AddManualChangeEquipGroupChar(base.CharacterMenu.CurCharacterId);
						this._manualChangeEquipCharIds.Add(base.CharacterMenu.CurCharacterId);
					}
					else
					{
						TaiwuDomainMethod.Call.RemoveManualChangeEquipGroupChar(base.CharacterMenu.CurCharacterId);
						this._manualChangeEquipCharIds.Remove(base.CharacterMenu.CurCharacterId);
					}
				}
			});
			this.hideScroll.onClick.ResetListener(delegate()
			{
				UIManager.Instance.SetEscHandler(null);
				this.HideDetailPanel();
			});
		}

		// Token: 0x060093B3 RID: 37811 RVA: 0x0044CC00 File Offset: 0x0044AE00
		private void OnAutoEquipButtonClicked()
		{
			bool flag = !base.CharacterMenu.IsTaiwu(base.CharacterMenu.CurCharacterId);
			if (flag)
			{
				this.lockChangeEquipTog.isOn = false;
			}
			CharacterDomainMethod.Call.AutoEquipItems(base.CharacterMenu.CurCharacterId);
			this.TriggerChangeCharacterClothingEvent();
		}

		// Token: 0x060093B4 RID: 37812 RVA: 0x0044CC50 File Offset: 0x0044AE50
		public void Refresh()
		{
			bool flag = base.CharacterMenu.CurCharacterId == -1;
			if (!flag)
			{
				bool flag2 = this._charDisplayData == null || this._charDisplayData.CharacterId != base.CharacterMenu.CurCharacterId;
				if (flag2)
				{
					CharacterDomainMethod.Call.GetCharacterItemsDisplayData(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
					CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this, base.CharacterMenu.CurCharacterId, delegate(int offset, RawDataPool pool)
					{
						Serializer.Deserialize(pool, offset, ref this._charDisplayData);
						this.<Refresh>g__RefreshCall|81_1();
					});
				}
				else
				{
					this.<Refresh>g__RefreshCall|81_1();
				}
			}
		}

		// Token: 0x060093B5 RID: 37813 RVA: 0x0044CCE4 File Offset: 0x0044AEE4
		public void RequestCharDisplayData()
		{
			bool flag = base.CharacterMenu.CurCharacterId == -1;
			if (flag)
			{
				Debug.LogError("CurCharacter is -1, RequestCharDisplayData cannot be done.");
			}
			else
			{
				this.SetMainLoadingState(true);
				CharacterDomainMethod.Call.GetCharacterItemsDisplayData(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
				CharacterDomainMethod.Call.GetCharacterDisplayData(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
				this.RequestManualChangeEquipCharIds();
				this.RequestEquips();
			}
		}

		// Token: 0x060093B6 RID: 37814 RVA: 0x0044CD61 File Offset: 0x0044AF61
		public void SetCharDisplayData(CharacterDisplayData characterDisplayData)
		{
			this._charDisplayData = characterDisplayData;
			this.charCircle.Set(this._charDisplayData, false, false);
		}

		// Token: 0x17000FF3 RID: 4083
		// (get) Token: 0x060093B7 RID: 37815 RVA: 0x0044CD80 File Offset: 0x0044AF80
		private short NormalEquipmentTemplateId
		{
			get
			{
				short carrier = this._equipItems[11].Key.TemplateId;
				short livestock = this._equipItems[12].Key.TemplateId;
				bool flag = (livestock == -1 && this._currSlot == -1) || this._currSlot == 11;
				short result;
				if (flag)
				{
					result = carrier;
				}
				else
				{
					bool flag2 = (carrier == -1 && this._currSlot == -1) || this._currSlot == 12;
					if (flag2)
					{
						result = livestock;
					}
					else
					{
						result = ((this.showCarrierGroup.GetActiveIndex() == 0) ? carrier : livestock);
					}
				}
				return result;
			}
		}

		// Token: 0x060093B8 RID: 37816 RVA: 0x0044CE1C File Offset: 0x0044B01C
		public void SwitchPlayMode()
		{
			bool flag = this._animIsPlay = !this._animIsPlay;
			if (flag)
			{
				this.characterAnimation.ResumeAnimation();
			}
			else
			{
				this.characterAnimation.PauseAnimation();
			}
		}

		// Token: 0x060093B9 RID: 37817 RVA: 0x0044CE5C File Offset: 0x0044B05C
		private void RefreshAnim(int togNew)
		{
			bool flag = togNew < 0;
			if (!flag)
			{
				this.characterAnimation.gameObject.SetActive(togNew == 1);
				this.characterAnimationPauser.gameObject.SetActive(togNew == 1);
				this.travelAnimation.gameObject.SetActive(togNew == 2);
				this.charCircle.gameObject.SetActive(togNew == 0);
				bool flag2 = togNew == 1 && !this._animIsPlay;
				if (flag2)
				{
					this.SwitchPlayMode();
				}
				bool flag3 = togNew == 2;
				if (flag3)
				{
					this.travelAnimation.Set(this._charDisplayData, this._equipItems, this.NormalEquipmentTemplateId, true);
				}
				else
				{
					bool flag4 = togNew == 1;
					if (flag4)
					{
						this.characterAnimation.Set(this._charDisplayData, this._equipItems, true, base.CharacterMenu.IsTaiwu(base.CharacterMenu.CurCharacterId) ? this._hideData.Items : null);
					}
					else
					{
						this.charCircle.Set(this._charDisplayData, false, false);
					}
				}
			}
		}

		// Token: 0x060093BA RID: 37818 RVA: 0x0044CF6C File Offset: 0x0044B16C
		public void RequestManualChangeEquipCharIds()
		{
			this.equipPlanTogGroup.SetInteractable(base.CharacterMenu.CanOperate);
			this.autoEquip.gameObject.SetActive(base.CharacterMenu.IsTaiwuTeam && this._isNormalCharacter && base.CharacterMenu.CanOperate);
			bool flag = this.autoEquip.gameObject.activeSelf && base.CharacterMenu.CurrentCharacterIsTaiwuTeammate;
			if (flag)
			{
				this.lockChangeEquipTog.gameObject.SetActive(true);
				TaiwuDomainMethod.Call.RequestManualChangeEquipGroupCharIds(this.Element.GameDataListenerId);
			}
			else
			{
				this.lockChangeEquipTog.gameObject.SetActive(false);
			}
		}

		// Token: 0x060093BB RID: 37819 RVA: 0x0044D022 File Offset: 0x0044B222
		private void RefreshManualChangeEquipCharIds()
		{
			this.lockChangeEquipTog.isOn = this._manualChangeEquipCharIds.Contains(base.CharacterMenu.CurCharacterId);
		}

		// Token: 0x060093BC RID: 37820 RVA: 0x0044D048 File Offset: 0x0044B248
		public void RequestCurrEquipmentPlanId()
		{
			bool flag = !base.CharacterMenu.IsTaiwu(base.CharacterMenu.CurCharacterId);
			if (!flag)
			{
				TaiwuDomainMethod.Call.RequestCurrEquipmentPlanId(this.Element.GameDataListenerId);
			}
		}

		// Token: 0x060093BD RID: 37821 RVA: 0x0044D086 File Offset: 0x0044B286
		public void RefreshCurrEquipmentPlanId()
		{
			this.equipPlanTogGroup.SetWithoutNotify(this._currEquipPlanId);
		}

		// Token: 0x060093BE RID: 37822 RVA: 0x0044D09C File Offset: 0x0044B29C
		public void RequestEquips()
		{
			bool flag = base.CharacterMenu.CurCharacterId == -1;
			if (flag)
			{
				Debug.LogError("CurCharacter is -1, RequestEquips cannot be done.");
			}
			else
			{
				this.SetMainLoadingState(true);
				this.SetAttributeLoadingState(true);
				CharacterDomainMethod.AsyncCall.GetCarrierMaxProperty(this, base.CharacterMenu.CurCharacterId, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._maxProperty);
					this.equipments[11].SetCarrierMaxProperty(this._maxProperty);
					this.equipments[12].SetCarrierMaxProperty(this._maxProperty);
				});
				CharacterDomainMethod.AsyncCall.GetAttributeWithDelta(this, base.CharacterMenu.CurCharacterId, ItemKey.Invalid, -1, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._displayData);
					this._baseData = new CharacterAttributeDisplayData(this._displayData[0]);
					this._fullData = new CharacterAttributeDisplayData(this._displayData[1]);
					this.RefreshAttributeWithDelta();
				});
				CharacterDomainMethod.Call.GetEquipLoad(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
				CharacterDomainMethod.Call.GetAllInventoryItems(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
				CharacterDomainMethod.Call.GetAllEquipmentItems(this.Element.GameDataListenerId, base.CharacterMenu.CurCharacterId);
			}
		}

		// Token: 0x060093BF RID: 37823 RVA: 0x0044D174 File Offset: 0x0044B374
		private void RefreshAttributeWithDelta()
		{
			this.attribute.Set(this._baseData, this._fullData, null);
			this.SetAttributeLoadingState(false);
		}

		// Token: 0x060093C0 RID: 37824 RVA: 0x0044D198 File Offset: 0x0044B398
		public void OnHoverEquipment(CharacterMenuEquipItemToggle equipment)
		{
			int slot = equipment.slot;
			ItemKey key;
			if (!this._equipItems.CheckIndex(equipment.slot))
			{
				key = ItemKey.Invalid;
			}
			else
			{
				ItemDisplayData itemDisplayData = this._equipItems[equipment.slot];
				key = ((itemDisplayData != null) ? itemDisplayData.Key : ItemKey.Invalid);
			}
			this.OnHoverEquipment(slot, key);
		}

		// Token: 0x060093C1 RID: 37825 RVA: 0x0044D1ED File Offset: 0x0044B3ED
		public void OnHoverEquipment(int slot, ItemKey key)
		{
			this._hoveringOnEquipment = true;
			CharacterDomainMethod.AsyncCall.GetAttributeWithDelta(this, base.CharacterMenu.CurCharacterId, key, slot, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._displayData);
				this.RefreshHoverEquipment(this._displayData, false);
			});
		}

		// Token: 0x060093C2 RID: 37826 RVA: 0x0044D218 File Offset: 0x0044B418
		private void RefreshHoverEquipment(CharacterAttributeDisplayData[] displayData, bool noHover)
		{
			bool hoveringOnEquipment = this._hoveringOnEquipment;
			if (hoveringOnEquipment)
			{
				this.attribute.Set(displayData[0], displayData[1], noHover ? null : displayData[2]);
			}
		}

		// Token: 0x060093C3 RID: 37827 RVA: 0x0044D24B File Offset: 0x0044B44B
		public void OnHoverEquipmentEnd(CharacterMenuEquipItemToggle equipment)
		{
			this.OnHoverEquipmentEnd();
		}

		// Token: 0x060093C4 RID: 37828 RVA: 0x0044D254 File Offset: 0x0044B454
		public void OnHoverEquipmentEnd()
		{
			this._hoveringOnEquipment = false;
			this.attribute.Set(this._baseData, this._fullData, null);
		}

		// Token: 0x060093C5 RID: 37829 RVA: 0x0044D278 File Offset: 0x0044B478
		private void RefreshEquipmentItems()
		{
			sbyte slotIndex = 0;
			while ((int)slotIndex < this.equipments.Length)
			{
				ItemDisplayData itemData = this._equipItems[(int)slotIndex].Clone(-1);
				this._equipItems[(int)slotIndex].UsingType = ItemDisplayData.ItemUsingType.Equiped;
				bool flag = itemData.Key.IsValid();
				if (flag)
				{
					itemData.UsingType = ItemDisplayData.ItemUsingType.Equiped;
				}
				this.SetEquipSlot(slotIndex, this._equipItems[(int)slotIndex], false);
				slotIndex += 1;
			}
			this.RefreshAnim(this.charDisplayTogGroup.GetActiveIndex());
			this.RefreshItemScroll();
			this.SetMainLoadingState(false);
			this._lockChangeEquipment = false;
			ItemDisplayData itemDisplayData = this._equipItems[11];
			bool? flag2 = (itemDisplayData != null) ? new bool?(itemDisplayData.Key.HasTemplate) : null;
			bool flag3;
			if (flag2 != null && flag2.GetValueOrDefault())
			{
				ItemDisplayData itemDisplayData2 = this._equipItems[12];
				flag3 = (((itemDisplayData2 != null) ? new bool?(itemDisplayData2.Key.HasTemplate) : null) ?? false);
			}
			else
			{
				flag3 = false;
			}
			bool showCarrierGroupActive = flag3;
			foreach (CToggle tog in this.showCarrierGroup.GetAll())
			{
				tog.gameObject.SetActive(showCarrierGroupActive);
			}
			this.Element.ShowAfterRefresh();
		}

		// Token: 0x060093C6 RID: 37830 RVA: 0x0044D410 File Offset: 0x0044B610
		private void RefreshItemScroll()
		{
			List<ItemDisplayData> items = this._equipItems.Concat(this._inventoryItems).Where(new Func<ItemDisplayData, bool>(this.IsEquipmentMatch)).ToList<ItemDisplayData>();
			this.itemScroll.SetItemList((from item in items
			select item.Clone(-1)).ToList<ItemDisplayData>());
		}

		// Token: 0x060093C7 RID: 37831 RVA: 0x0044D47C File Offset: 0x0044B67C
		private void RefreshEquipLoad()
		{
			this.equipLoad.text = string.Format("{0}/{1:f1}", string.Format("{0:f1}", (double)this._equipLoad.Item1 / 100.0).SetColor(CommonUtils.GetLoadWeightValueColor(this._equipLoad.Item1, this._equipLoad.Item2)), (double)this._equipLoad.Item2 / 100.0).ColorReplace();
			TooltipInvoker tip = this.equipWeight.gameObject.GetComponent<TooltipInvoker>();
			tip.Type = TipType.EquipLoad;
			TooltipInvoker tooltipInvoker = tip;
			if (tooltipInvoker.RuntimeParam == null)
			{
				tooltipInvoker.RuntimeParam = new ArgumentBox();
			}
			tip.RuntimeParam.Set("currentLoad", this._equipLoad.Item1);
			tip.RuntimeParam.Set("maxLoad", this._equipLoad.Item2);
			bool flag = !this._equipOverload && this._equipLoad.Item1 > this._equipLoad.Item2 && base.CharacterMenu.IsTaiwu(base.CharacterMenu.CurCharacterId);
			if (flag)
			{
				this._equipOverload = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(224);
			}
		}

		// Token: 0x060093C8 RID: 37832 RVA: 0x0044D5BF File Offset: 0x0044B7BF
		private void RefreshInventoryItems()
		{
		}

		// Token: 0x060093C9 RID: 37833 RVA: 0x0044D5C4 File Offset: 0x0044B7C4
		private void RefreshHideStatus()
		{
			bool flag = !base.CharacterMenu.IsTaiwu(base.CharacterMenu.CurCharacterId) || UIElement.Combat.Exist;
			if (flag)
			{
				foreach (CToggle toggle in this.hideGroup.GetAll())
				{
					toggle.gameObject.SetActive(false);
				}
			}
			else
			{
				TaiwuDomainMethod.AsyncCall.RequestHideSkeletonEquipSlots(this, delegate(int offset, RawDataPool pool)
				{
					Serializer.Deserialize(pool, offset, ref this._hideData);
					int i = ViewCharacterMenuEquip.HideableEquipmentSlot.Length;
					while (i-- > 0)
					{
						List<sbyte> items = this._hideData.Items;
						bool flag2 = items != null && items.Contains(ViewCharacterMenuEquip.HideableEquipmentSlot[i]);
						if (flag2)
						{
							this.hideGroup.DeSelectWithoutNotify(i);
						}
						else
						{
							this.hideGroup.SelectWithoutNotify(i);
						}
					}
					foreach (CToggle toggle2 in this.hideGroup.GetAll())
					{
						toggle2.gameObject.SetActive(true);
					}
					this.RefreshAnim(this.charDisplayTogGroup.GetActiveIndex());
				});
			}
		}

		// Token: 0x060093CA RID: 37834 RVA: 0x0044D664 File Offset: 0x0044B864
		public void Awake()
		{
			this.InitToggleGroup();
			this.InitToggles();
			this.itemScroll.Init("EquipSortAndFilterController", ESortAndFilterControllerType.Equip, true, new Action<ITradeableContent, RowItemLine>(this.OnItemRender), delegate(ITradeableContent itemData, RowItemLine rowItemLine)
			{
				bool flag = this._equipItems[(int)this._currSlot].Key == itemData.Key;
				if (flag)
				{
					AudioManager.Instance.PlaySound(this.GetEquipSoundName(this._currSlot, itemData.Key, false), false, false);
					this.ChangeEquipment(this._currSlot, -1, itemData);
				}
				else
				{
					AudioManager.Instance.PlaySound(this.GetEquipSoundName(this._currSlot, itemData.Key, true), false, false);
					this.ChangeEquipment((sbyte)this._equipItems.FindIndex((ItemDisplayData key) => key.Key == itemData.Key), this._currSlot, itemData);
				}
			}, ItemListScroll.EColumnType.IconAndNameWithDurability | ItemListScroll.EColumnType.Type | ItemListScroll.EColumnType.Weight | ItemListScroll.EColumnType.Value | ItemListScroll.EColumnType.Power, null, null, null);
			this.itemScroll.SetTableHeadSortEnabled(false);
			if (this._inventoryItems == null)
			{
				this._inventoryItems = new List<ItemDisplayData>();
			}
			this._inventoryItems.Clear();
			GEvent.Add(UiEvents.OnLanguageChange, new GEvent.Callback(this.ChangeLanguageType));
		}

		// Token: 0x060093CB RID: 37835 RVA: 0x0044D6FC File Offset: 0x0044B8FC
		public void OnDestroy()
		{
			GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.ChangeLanguageType));
			bool flag = this._mainAreaDelayHideCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._mainAreaDelayHideCoroutine);
				this._mainAreaDelayHideCoroutine = null;
			}
			bool flag2 = this._attributeAreaDelayHideCoroutine != null;
			if (flag2)
			{
				base.StopCoroutine(this._attributeAreaDelayHideCoroutine);
				this._attributeAreaDelayHideCoroutine = null;
			}
		}

		// Token: 0x060093CC RID: 37836 RVA: 0x0044D76C File Offset: 0x0044B96C
		private new void OnDisable()
		{
			bool flag = this._mainAreaDelayHideCoroutine != null;
			if (flag)
			{
				base.StopCoroutine(this._mainAreaDelayHideCoroutine);
				this._mainAreaDelayHideCoroutine = null;
			}
			bool flag2 = this._attributeAreaDelayHideCoroutine != null;
			if (flag2)
			{
				base.StopCoroutine(this._attributeAreaDelayHideCoroutine);
				this._attributeAreaDelayHideCoroutine = null;
			}
			this._isMainAreaLoading = false;
			this._isAttributeAreaLoading = false;
			bool flag3 = this.mainAreaContentRoot != null;
			if (flag3)
			{
				((RectTransform)this.mainAreaContentRoot.transform).anchoredPosition = this._mainAreaContentRootOriginalPos;
			}
		}

		// Token: 0x060093CD RID: 37837 RVA: 0x0044D7FC File Offset: 0x0044B9FC
		private bool IsLock(sbyte equipmentSlot, ItemKey itemKey)
		{
			CharacterMenuFunctionControlItem config;
			return !base.CharacterMenu.CanOperate || base.CharacterMenu.IsTaiwuBeastTeammate(base.CharacterMenu.CurCharacterId) || !base.CharacterMenu.IsTaiwuTeam || !GameData.Domains.Character.SharedMethods.CanModifyEquipSlot(this._charDisplayData.TemplateId, equipmentSlot, itemKey) || (base.CharacterMenu.TryGetFunctionControlConfig(out config) && base.CharacterMenu.IsFunctionBanned(config.ItemEquip));
		}

		// Token: 0x060093CE RID: 37838 RVA: 0x0044D878 File Offset: 0x0044BA78
		private void OnItemRender(ITradeableContent itemData, RowItemLine rowItemLine)
		{
			RowItemMain rowItemMain = rowItemLine.GetComponentInChildren<RowItemMain>();
			rowItemMain.SetData(itemData);
			rowItemLine.Set(rowItemMain, true);
			bool isLock = this.IsLock(this._currSlot, itemData.Key) || this.IsLock(this._currSlot, this._equipItems[(int)this._currSlot].Key);
			bool flag = !isLock && itemData.ItemSourceType == 0;
			if (flag)
			{
				sbyte slot = 0;
				while ((int)slot < this._equipItems.Count)
				{
					bool flag2 = this._equipItems[(int)slot].Key == itemData.Key;
					if (flag2)
					{
						isLock = !GameData.Domains.Character.SharedMethods.CanModifyEquipSlot(this._charDisplayData.TemplateId, slot, itemData.Key);
						break;
					}
					slot += 1;
				}
			}
			rowItemLine.SetInteractable(!isLock, true);
			rowItemLine.SetDisabled(isLock);
			rowItemLine.SetSelected(itemData.Key == this._equipItems[(int)this._currSlot].Key);
			rowItemLine.OnPointerEnterEvent = (rowItemLine.enabled ? delegate()
			{
				this.OnHoverEquipment((int)this._currSlot, itemData.Key);
			} : null);
			rowItemLine.OnPointerExitEvent = new Action(this.OnHoverEquipmentEnd);
		}

		// Token: 0x060093CF RID: 37839 RVA: 0x0044D9EC File Offset: 0x0044BBEC
		public override void OnCurrentCharacterChange(int prevCharacterId)
		{
			bool flag = base.CharacterMenu.CurCharacterId < 0 || prevCharacterId == base.CharacterMenu.CurCharacterId;
			if (!flag)
			{
				this.SetLoadingState(true);
				this.Refresh();
				this.RefreshHideStatus();
			}
		}

		// Token: 0x060093D0 RID: 37840 RVA: 0x0044DA38 File Offset: 0x0044BC38
		public override void OnSubpageVisible()
		{
			base.OnSubpageVisible();
			this.SetLoadingState(true);
			this.Refresh();
			bool flag = !this._animIsPlay;
			if (flag)
			{
				this.SwitchPlayMode();
			}
		}

		// Token: 0x060093D1 RID: 37841 RVA: 0x0044DA70 File Offset: 0x0044BC70
		protected override void SetLoadingState(bool loading)
		{
			this.SetMainLoadingState(loading);
			this.SetAttributeLoadingState(loading);
		}

		// Token: 0x060093D2 RID: 37842 RVA: 0x0044DA84 File Offset: 0x0044BC84
		private void SetMainLoadingState(bool loading)
		{
			if (loading)
			{
				bool flag = this._mainAreaDelayHideCoroutine != null;
				if (flag)
				{
					base.StopCoroutine(this._mainAreaDelayHideCoroutine);
					this._mainAreaDelayHideCoroutine = null;
				}
				this._mainAreaLoadingShowStartTime = Time.unscaledTime;
				bool isMainAreaLoading = this._isMainAreaLoading;
				if (!isMainAreaLoading)
				{
					this.SetMainLoadingStateImmediate(true);
				}
			}
			else
			{
				bool flag2 = !this._isMainAreaLoading;
				if (!flag2)
				{
					bool flag3 = this._mainAreaDelayHideCoroutine != null;
					if (!flag3)
					{
						float minDuration = Mathf.Max(0f, this.partitionLoadingMinVisibleDuration);
						float elapsed = Time.unscaledTime - this._mainAreaLoadingShowStartTime;
						float remain = minDuration - elapsed;
						bool flag4 = remain <= 0f;
						if (flag4)
						{
							this.SetMainLoadingStateImmediate(false);
						}
						else
						{
							bool flag5 = !base.isActiveAndEnabled || !base.gameObject.activeInHierarchy;
							if (flag5)
							{
								this.SetMainLoadingStateImmediate(false);
							}
							else
							{
								this._mainAreaDelayHideCoroutine = base.StartCoroutine(this.DelayHideMainAreaLoading(remain));
							}
						}
					}
				}
			}
		}

		// Token: 0x060093D3 RID: 37843 RVA: 0x0044DB8F File Offset: 0x0044BD8F
		private IEnumerator DelayHideMainAreaLoading(float delay)
		{
			yield return new WaitForSecondsRealtime(delay);
			this._mainAreaDelayHideCoroutine = null;
			bool isMainAreaLoading = this._isMainAreaLoading;
			if (isMainAreaLoading)
			{
				this.SetMainLoadingStateImmediate(false);
			}
			yield break;
		}

		// Token: 0x060093D4 RID: 37844 RVA: 0x0044DBA8 File Offset: 0x0044BDA8
		private void SetMainLoadingStateImmediate(bool loading)
		{
			this._isMainAreaLoading = loading;
			bool flag = this.mainAreaLoading != null;
			if (flag)
			{
				this.mainAreaLoading.gameObject.SetActive(loading);
			}
			bool flag2 = this.mainAreaContentRoot != null;
			if (flag2)
			{
				RectTransform rectTransform = (RectTransform)this.mainAreaContentRoot.transform;
				if (loading)
				{
					this._mainAreaContentRootOriginalPos = rectTransform.anchoredPosition;
					rectTransform.anchoredPosition = new Vector2(10000f, 10000f);
				}
				else
				{
					rectTransform.anchoredPosition = this._mainAreaContentRootOriginalPos;
				}
			}
		}

		// Token: 0x060093D5 RID: 37845 RVA: 0x0044DC3C File Offset: 0x0044BE3C
		private void SetAttributeLoadingState(bool loading)
		{
			if (loading)
			{
				bool flag = this._attributeAreaDelayHideCoroutine != null;
				if (flag)
				{
					base.StopCoroutine(this._attributeAreaDelayHideCoroutine);
					this._attributeAreaDelayHideCoroutine = null;
				}
				this._attributeAreaLoadingShowStartTime = Time.unscaledTime;
				bool isAttributeAreaLoading = this._isAttributeAreaLoading;
				if (!isAttributeAreaLoading)
				{
					this.SetAttributeLoadingStateImmediate(true);
				}
			}
			else
			{
				bool flag2 = !this._isAttributeAreaLoading;
				if (!flag2)
				{
					bool flag3 = this._attributeAreaDelayHideCoroutine != null;
					if (!flag3)
					{
						float minDuration = Mathf.Max(0f, this.partitionLoadingMinVisibleDuration);
						float elapsed = Time.unscaledTime - this._attributeAreaLoadingShowStartTime;
						float remain = minDuration - elapsed;
						bool flag4 = remain <= 0f;
						if (flag4)
						{
							this.SetAttributeLoadingStateImmediate(false);
						}
						else
						{
							bool flag5 = !base.isActiveAndEnabled || !base.gameObject.activeInHierarchy;
							if (flag5)
							{
								this.SetAttributeLoadingStateImmediate(false);
							}
							else
							{
								this._attributeAreaDelayHideCoroutine = base.StartCoroutine(this.DelayHideAttributeAreaLoading(remain));
							}
						}
					}
				}
			}
		}

		// Token: 0x060093D6 RID: 37846 RVA: 0x0044DD47 File Offset: 0x0044BF47
		private IEnumerator DelayHideAttributeAreaLoading(float delay)
		{
			yield return new WaitForSecondsRealtime(delay);
			this._attributeAreaDelayHideCoroutine = null;
			bool isAttributeAreaLoading = this._isAttributeAreaLoading;
			if (isAttributeAreaLoading)
			{
				this.SetAttributeLoadingStateImmediate(false);
			}
			yield break;
		}

		// Token: 0x060093D7 RID: 37847 RVA: 0x0044DD60 File Offset: 0x0044BF60
		private void SetAttributeLoadingStateImmediate(bool loading)
		{
			this._isAttributeAreaLoading = loading;
			bool flag = this.attributeAreaLoading != null;
			if (flag)
			{
				this.attributeAreaLoading.gameObject.SetActive(loading);
			}
			bool flag2 = this.attributeAreaContentRoot != null;
			if (flag2)
			{
				this.attributeAreaContentRoot.SetActive(!loading);
			}
		}

		// Token: 0x060093D8 RID: 37848 RVA: 0x0044DDB8 File Offset: 0x0044BFB8
		public override void OnInit(ArgumentBox argumentBox)
		{
			this._lockChangeEquipment = (this.NeedDataListenerId = true);
			foreach (CToggle tog in this.showCarrierGroup.GetAll())
			{
				tog.gameObject.SetActive(false);
			}
			UIElement element = this.Element;
			element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(delegate()
			{
				this.SetLoadingState(true);
				this.RefreshHideStatus();
				this.Refresh();
				this.charDisplayTogGroup.Set(0, false);
			}));
			this.background.enabled = false;
			ResLoader.Load<Texture2D>(Path.Combine("RemakeResources/Textures/UITexturesRemake", SingletonObject.getInstance<WorldMapModel>().CurrentBlockData.GetConfig().BackgroundForMenuEquip), delegate(Texture2D tex)
			{
				this.background.texture = tex;
				this.background.enabled = true;
			}, null, false);
			this.ChangeLanguageType(null);
			bool flag = !this._firstEnter;
			if (flag)
			{
				this._firstEnter = true;
				GlobalDomainMethod.Call.InvokeGuidingTrigger(133);
			}
			this.RefreshHideStatus();
		}

		// Token: 0x060093D9 RID: 37849 RVA: 0x0044DEC0 File Offset: 0x0044C0C0
		private void ChangeLanguageType(ArgumentBox _)
		{
			bool flag = (this.CurLanguageType = LocalStringManager.CurLanguageType) == LocalStringManager.LanguageType.CN;
			if (flag)
			{
				this.mainText.lineSpacing = (this.subText.lineSpacing = 0f);
				this.mainText.fontSize = (this.subText.fontSize = 28f);
				this.equipLoadTitle.fontSize = 24f;
			}
			else
			{
				this.mainText.lineSpacing = (this.subText.lineSpacing = -24f);
				this.mainText.fontSize = (this.subText.fontSize = 26f);
				this.equipLoadTitle.fontSize = 22f;
			}
			this.SetToggleColor(this.subPage.gameObject.activeSelf ? 1 : 0);
		}

		// Token: 0x060093DA RID: 37850 RVA: 0x0044DFA8 File Offset: 0x0044C1A8
		protected override void OnNotifyGameDataFiltered(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper wrapper in notifications)
			{
				Notification notification = wrapper.Notification;
				Notification notification2 = notification;
				Notification notification3 = notification2;
				byte type = notification3.Type;
				if (type == 1)
				{
					ushort domainId = notification3.DomainId;
					if (domainId != 4)
					{
						if (domainId == 5)
						{
							ushort methodId = notification3.MethodId;
							if (methodId != 195)
							{
								if (methodId == 198)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._manualChangeEquipCharIds);
									this.RefreshManualChangeEquipCharIds();
								}
							}
							else
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._currEquipPlanId);
								this.RefreshCurrEquipmentPlanId();
							}
						}
					}
					else
					{
						ushort methodId = notification3.MethodId;
						if (methodId <= 29)
						{
							if (methodId != 27)
							{
								if (methodId == 29)
								{
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._equipItems);
									this.RefreshEquipmentItems();
								}
							}
							else
							{
								Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._inventoryItems);
								this.RefreshInventoryItems();
							}
						}
						else if (methodId != 131)
						{
							if (methodId != 199)
							{
								if (methodId == 207)
								{
									this._characterItemsDisplayData = null;
									Serializer.Deserialize(wrapper.DataPool, notification.ValueOffset, ref this._characterItemsDisplayData);
								}
							}
							else
							{
								Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref this._equipLoad);
								this.RefreshEquipLoad();
							}
						}
						else
						{
							Serializer.Deserialize(wrapper.DataPool, wrapper.Notification.ValueOffset, ref this._charDisplayData);
						}
					}
				}
			}
		}

		// Token: 0x060093DB RID: 37851 RVA: 0x0044E1A0 File Offset: 0x0044C3A0
		public void ButtonSwitchSubpage(int subPageIndex)
		{
			ViewCharacterMenu characterMenu = base.CharacterMenu;
			if (!true)
			{
			}
			int curPageSubpage;
			if (subPageIndex != 0)
			{
				if (subPageIndex != 1)
				{
					curPageSubpage = -1;
				}
				else
				{
					curPageSubpage = 12;
				}
			}
			else
			{
				curPageSubpage = 11;
			}
			if (!true)
			{
			}
			characterMenu.SetCurPageSubpage(curPageSubpage);
			this.OnSwitchToSubpage(subPageIndex);
		}

		// Token: 0x060093DC RID: 37852 RVA: 0x0044E1E8 File Offset: 0x0044C3E8
		public override void OnSwitchToSubpage(int subPageIndex)
		{
			bool flag = subPageIndex < 0;
			if (!flag)
			{
				this.subPage.gameObject.SetActive(subPageIndex == 1);
				this.mainPage.gameObject.SetActive(subPageIndex != 1);
				this.SetToggleColor(subPageIndex);
			}
		}

		// Token: 0x060093DD RID: 37853 RVA: 0x0044E238 File Offset: 0x0044C438
		private void SetEquipSlot(sbyte slotIndex, ItemDisplayData itemData, bool setDisplayTogGroup = true)
		{
			if (slotIndex > 2)
			{
				if (slotIndex != 11)
				{
					if (slotIndex == 12)
					{
						if (setDisplayTogGroup)
						{
							this.TrySetcharDisplayTogGroup(2);
						}
					}
				}
				else if (setDisplayTogGroup)
				{
					this.TrySetcharDisplayTogGroup(2);
				}
			}
			else
			{
				short templateId = (this._equipItems[(int)slotIndex].Key != ItemKey.Invalid) ? this._equipItems[(int)slotIndex].Key.TemplateId : 0;
				if (setDisplayTogGroup)
				{
					this.TrySetcharDisplayTogGroup(1);
				}
				bool canShowCharacterPreview = this.CanShowCharacterPreview;
				if (canShowCharacterPreview)
				{
					this.characterAnimation.CurrentWeaponId = templateId;
				}
			}
			CharacterMenuEquipItemToggle characterMenuEquipItemToggle = this.equipments[(int)slotIndex];
			CharacterDisplayData charDisplayData = this._charDisplayData;
			byte? b;
			if (charDisplayData == null)
			{
				b = null;
			}
			else
			{
				AvatarRelatedData avatarRelatedData = charDisplayData.AvatarRelatedData;
				if (avatarRelatedData == null)
				{
					b = null;
				}
				else
				{
					AvatarData avatarData = avatarRelatedData.AvatarData;
					b = ((avatarData != null) ? new byte?(avatarData.AvatarId) : null);
				}
			}
			characterMenuEquipItemToggle.Set(itemData, b ?? 1);
		}

		// Token: 0x060093DE RID: 37854 RVA: 0x0044E350 File Offset: 0x0044C550
		private void ChangeEquipment(sbyte srcSlot, sbyte destSlot, ITradeableContent itemData)
		{
			bool lockChangeEquipment = this._lockChangeEquipment;
			if (!lockChangeEquipment)
			{
				this._lockChangeEquipment = true;
				ItemDisplayData itemData2 = (this._equipItems.Contains(itemData) ? this._inventoryItems : this._equipItems).Find((ItemDisplayData data) => data.Key.Equals(itemData.Key));
				bool flag;
				if (srcSlot == 13 && destSlot == 12)
				{
					IItemConfig config = this._equipItems[(int)destSlot].Key.GetConfig();
					flag = !(((config != null) ? new short?(config.ItemSubType) : null) == 401);
				}
				else
				{
					flag = true;
				}
				bool canSwap = flag;
				bool flag2 = srcSlot >= 0;
				if (flag2)
				{
					bool flag3 = destSlot >= 0 && this._equipItems[(int)destSlot].Key.IsValid() && canSwap;
					if (flag3)
					{
						this._equipItems[(int)srcSlot] = this._equipItems[(int)destSlot];
						this.SetEquipSlot(srcSlot, this._equipItems[(int)srcSlot], true);
					}
					else
					{
						this._equipItems[(int)srcSlot] = new ItemDisplayData();
						this.SetEquipSlot(srcSlot, this._equipItems[(int)srcSlot], true);
					}
				}
				bool flag4 = destSlot >= 0;
				if (flag4)
				{
					this.equipSwitchTogGroup.Set((int)destSlot, false);
					this._equipItems[(int)destSlot] = (itemData.Clone(-1) as ItemDisplayData);
					this._equipItems[(int)destSlot].UsingType = ItemDisplayData.ItemUsingType.Equiped;
					this.SetEquipSlot(destSlot, this._equipItems[(int)destSlot], true);
					bool flag5 = SingletonObject.getInstance<TutorialChapterModel>().IsInTutorialChapter(0) && destSlot == 11 && itemData.Key.TemplateId == 45;
					if (flag5)
					{
						GEvent.OnEvent(UiEvents.RefreshVideo, EasyPool.Get<ArgumentBox>().Set("TutorialVideoPathName", "Tutorial_Chapter_1_3"));
					}
				}
				bool flag6 = !canSwap;
				if (flag6)
				{
					CharacterDomainMethod.Call.ChangeEquipment(base.CharacterMenu.CurCharacterId, destSlot, -1, itemData.Key);
				}
				CharacterDomainMethod.Call.ChangeEquipment(base.CharacterMenu.CurCharacterId, srcSlot, destSlot, itemData.Key);
				GEvent.OnEvent(UiEvents.OnCharacterTaiwuCarrierChanged, null);
				bool flag7 = srcSlot == 4 || destSlot == 4;
				if (flag7)
				{
					this.TriggerChangeCharacterClothingEvent();
				}
				else
				{
					this.RequestEquips();
				}
			}
		}

		// Token: 0x060093DF RID: 37855 RVA: 0x0044E5D4 File Offset: 0x0044C7D4
		public bool IsEquipmentMatch(ItemDisplayData data)
		{
			return this._currSlot >= 0 && data != null && data.Key.IsValid() && ItemTemplateHelper.IsItemMeetSlot(data.Key.ItemType, data.Key.TemplateId, this._currSlot);
		}

		// Token: 0x060093E0 RID: 37856 RVA: 0x0044E624 File Offset: 0x0044C824
		public void TrySetcharDisplayTogGroup(int index)
		{
			bool isNormalCharacter = this._isNormalCharacter;
			if (isNormalCharacter)
			{
				this.charDisplayTogGroup.Set(index, false);
			}
		}

		// Token: 0x060093E1 RID: 37857 RVA: 0x0044E64C File Offset: 0x0044C84C
		private void OpenDetailPanel(int togNew)
		{
			bool flag = togNew < 0;
			if (!flag)
			{
				switch (this._currSlot = (sbyte)togNew)
				{
				case 0:
				case 1:
				case 2:
					this.itemScroll.SortAndFilterController.SetToggleVisible(3, 0);
					this.itemScroll.SortAndFilterController.SetToggleIsOn(3, 0);
					this.TrySetcharDisplayTogGroup(1);
					break;
				case 3:
					this.itemScroll.SortAndFilterController.SetToggleVisible(3, 1);
					this.itemScroll.SortAndFilterController.SetToggleIsOn(3, 1);
					this.TrySetcharDisplayTogGroup(1);
					break;
				case 4:
					this.itemScroll.SortAndFilterController.SetToggleVisible(3, 6);
					this.itemScroll.SortAndFilterController.SetToggleIsOn(3, 6);
					this.TrySetcharDisplayTogGroup(0);
					break;
				case 5:
					this.itemScroll.SortAndFilterController.SetToggleVisible(3, 2);
					this.itemScroll.SortAndFilterController.SetToggleIsOn(3, 2);
					this.TrySetcharDisplayTogGroup(1);
					break;
				case 6:
					this.itemScroll.SortAndFilterController.SetToggleVisible(3, 3);
					this.itemScroll.SortAndFilterController.SetToggleIsOn(3, 3);
					this.TrySetcharDisplayTogGroup(1);
					break;
				case 7:
					this.itemScroll.SortAndFilterController.SetToggleVisible(3, 4);
					this.itemScroll.SortAndFilterController.SetToggleIsOn(3, 4);
					this.TrySetcharDisplayTogGroup(1);
					break;
				case 8:
				case 9:
				case 10:
					this.itemScroll.SortAndFilterController.SetToggleVisible(3, 5);
					this.itemScroll.SortAndFilterController.SetToggleIsOn(3, 5);
					break;
				case 11:
					this.itemScroll.SortAndFilterController.SetToggleVisible(3, 7);
					this.itemScroll.SortAndFilterController.SetToggleIsOn(3, 7);
					this.TrySetcharDisplayTogGroup(2);
					break;
				case 12:
					this.itemScroll.SortAndFilterController.SetToggleVisible(3, 8);
					this.itemScroll.SortAndFilterController.SetToggleIsOn(3, 8);
					this.TrySetcharDisplayTogGroup(2);
					break;
				case 13:
					this.itemScroll.SortAndFilterController.SetToggleVisible(3, 9);
					this.itemScroll.SortAndFilterController.SetToggleIsOn(3, 9);
					this.TrySetcharDisplayTogGroup(2);
					break;
				case 14:
				case 15:
				case 16:
					this.itemScroll.SortAndFilterController.SetToggleVisible(3, 10);
					this.itemScroll.SortAndFilterController.SetToggleIsOn(3, 10);
					this.TrySetcharDisplayTogGroup(2);
					break;
				}
				this.RefreshItemScroll();
				this.middleContent.DOKill(false);
				this.middleContent.DOSizeDelta(this.middleContent.sizeDelta.SetX((float)this.shrinkage), this.animDuration, false);
				this.middleContent.DOLocalMoveX((float)(-(float)this.shrinkage) / 2f, this.animDuration, false);
				base.CharacterMenu.MoveCharacterScrollLeft(0.5f, Ease.OutExpo);
				this.itemScroll.transform.DOKill(false);
				this.itemScroll.gameObject.SetActive(true);
				this.itemScroll.transform.DOLocalMoveX((float)this.moveTo, this.animDuration, false).SetEase(Ease.OutExpo);
				sbyte currSlot = this._currSlot;
				bool flag2 = currSlot == 11 || currSlot == 12;
				if (flag2)
				{
					this.travelAnimation.Set(this._charDisplayData, this._equipItems, (this._currSlot == 11) ? this._equipItems[11].Key.TemplateId : this._equipItems[12].Key.TemplateId, true);
				}
				UIManager.Instance.SetEscHandler(new Action(this.HideDetailPanel));
			}
		}

		// Token: 0x060093E2 RID: 37858 RVA: 0x0044EA30 File Offset: 0x0044CC30
		private void HideDetailPanel()
		{
			this.middleContent.DOKill(false);
			this.middleContent.DOSizeDelta(this.middleContent.sizeDelta.SetX(0f), this.animDuration, false);
			this.middleContent.DOLocalMoveX(0f, this.animDuration, false);
			this.equipSwitchTogGroup.DeSelectWithoutNotify();
			base.CharacterMenu.MoveCharacterScrollBack(0.5f, Ease.OutExpo);
			this.itemScroll.transform.DOKill(false);
			this.itemScroll.transform.DOLocalMoveX((float)this.moveFrom, this.animDuration, false).SetEase(Ease.OutExpo).OnComplete(delegate
			{
				this.itemScroll.gameObject.SetActive(false);
			});
			this._currSlot = -1;
			this.travelAnimation.Set(this._charDisplayData, this._equipItems, this.NormalEquipmentTemplateId, true);
		}

		// Token: 0x060093E3 RID: 37859 RVA: 0x0044EB1C File Offset: 0x0044CD1C
		public override void OnSubpageInVisible()
		{
			bool flag = UIManager.Instance.CheckEscHandler(new Action(this.HideDetailPanel));
			if (flag)
			{
				UIManager.Instance.SetEscHandler(null);
				this.HideDetailPanel();
			}
		}

		// Token: 0x060093E4 RID: 37860 RVA: 0x0044EB5C File Offset: 0x0044CD5C
		private void ShowPopupMenu(int index)
		{
			List<ViewPopupMenu.BtnData> btnList = new List<ViewPopupMenu.BtnData>();
			ItemDisplayData itemData = this._equipItems[index];
			bool currentCharacterIsTaiwu = base.CharacterMenu.CurrentCharacterIsTaiwu;
			if (currentCharacterIsTaiwu)
			{
				bool flag = itemData.Key.ItemType == 0;
				if (flag)
				{
					string btnName = LocalStringManager.Get(LanguageKey.LK_ItemUsingOperationType_ChangeInnerRatio);
					ViewPopupMenu.BtnData btnData = new ViewPopupMenu.BtnData(btnName, true, EItemMenuDisplayOrder.ChangeInnerRatio, delegate()
					{
						this.OpenSetChangeWeaponInnerRatio(index);
					}, null, null, false);
					btnList.Add(btnData);
				}
				bool flag2 = itemData.Key.ItemType == 4;
				if (flag2)
				{
					bool flag3 = ItemTemplateHelper.CanFeedCarrier(itemData.Key.ItemType, itemData.Key.TemplateId);
					if (flag3)
					{
						CharacterMenuFunctionControlItem config;
						bool banned = base.CharacterMenu.TryGetFunctionControlConfig(out config) && base.CharacterMenu.IsFunctionBanned(config.Feed);
						bool isDurabilityMax = itemData.Durability >= itemData.MaxDurability;
						CarrierItem carrierConfig = Carrier.Instance[itemData.Key.TemplateId];
						bool isTameMax = carrierConfig.CombatState < 0 || itemData.CarrierTamePoint >= GlobalConfig.Instance.MaxCarrierTamePoint;
						bool needFeed = !isDurabilityMax || !isTameMax;
						bool hasFood = this._inventoryItems.Exists((ItemDisplayData d) => ItemTemplateHelper.IsFeedingAble(d.Key.ItemType, d.Key.TemplateId));
						bool interactable = needFeed && hasFood && !banned;
						string btnName2 = LocalStringManager.Get(LanguageKey.LK_Feeding_Item);
						ViewPopupMenu.BtnData btnData2 = new ViewPopupMenu.BtnData(btnName2, interactable, EItemMenuDisplayOrder.Feed, delegate()
						{
							UIElement.CharacterMenuItems.UiBaseAs<ViewCharacterMenuItems>().AddPreOperation(ItemOperationType.EItemOperationType.Feeding, itemData.RealKey);
							UIElement.CharacterMenu.UiBaseAs<ViewCharacterMenu>().SwitchToSubToggle(ECharacterSubToggleBase.ItemBase);
						}, null, null, false);
						btnList.Add(btnData2);
						bool flag4 = !interactable;
						if (flag4)
						{
							LanguageKey key = needFeed ? LanguageKey.LK_Feeding_Item_Tip_MaterialNotMeet : LanguageKey.LK_Feeding_Item_Tip_NoNeed;
							btnData2.SetTip("", LocalStringManager.Get(key));
						}
					}
				}
				sbyte skillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(itemData.Key.ItemType, itemData.Key.TemplateId);
				LifeSkillTypeItem skillConfig = Config.LifeSkillType.Instance[skillType];
				bool isRepairable = ItemTemplateHelper.IsRepairable(itemData.Key.ItemType, itemData.Key.TemplateId);
				bool flag5 = isRepairable;
				if (flag5)
				{
					string btnName3 = LocalStringManager.Get(LanguageKey.LK_Repair_Item);
					bool flag6;
					bool noNeed;
					bool noResource;
					bool hasAnySkillMatchedTool;
					CharacterMenuFunctionControlItem config2;
					bool interactable2 = this.CheckItemRepairCondition(itemData, out flag6, out noNeed, out noResource, out hasAnySkillMatchedTool) && (!base.CharacterMenu.TryGetFunctionControlConfig(out config2) || !base.CharacterMenu.IsFunctionBanned(config2.Repair));
					ViewPopupMenu.BtnData btnData3 = new ViewPopupMenu.BtnData(btnName3, interactable2, EItemMenuDisplayOrder.Tool, delegate()
					{
						UIElement.CharacterMenuItems.UiBaseAs<ViewCharacterMenuItems>().AddPreOperation(ItemOperationType.EItemOperationType.Repair, itemData.RealKey);
						UIElement.CharacterMenu.UiBaseAs<ViewCharacterMenu>().SwitchToSubToggle(ECharacterSubToggleBase.ItemBase);
					}, null, null, false);
					btnList.Add(btnData3);
					bool flag7 = !interactable2;
					if (flag7)
					{
						bool flag8 = noNeed;
						string content;
						if (flag8)
						{
							content = LanguageKey.LK_Repair_Item_Tip_NoNeed.Tr();
						}
						else
						{
							bool isLocked = itemData.IsLocked;
							if (isLocked)
							{
								content = LanguageKey.LK_Item_State_Locked_Tip.Tr();
							}
							else
							{
								bool flag9 = hasAnySkillMatchedTool;
								if (flag9)
								{
									content = LanguageKey.LK_Item_Operation_LifeSkillAttainment_NotMeet.TrFormat(skillConfig.Name).SetColor("brightred");
								}
								else
								{
									bool flag10 = noResource;
									if (flag10)
									{
										content = LanguageKey.LK_Repair_Item_Tip_NoResource.Tr();
									}
									else
									{
										content = LanguageKey.LK_Repair_Item_Tip_NoTool.Tr();
									}
								}
							}
						}
						btnData3.SetTip("", content.ColorReplace());
					}
				}
			}
			bool flag11 = base.CharacterMenu.IsTaiwuTeam && !base.CharacterMenu.IsTaiwuBeastTeammate(base.CharacterMenu.CurCharacterId);
			if (flag11)
			{
				string btnName4 = LocalStringManager.Get(LanguageKey.LK_EquipCombatSkill_Switch);
				ViewPopupMenu.BtnData btnData4 = new ViewPopupMenu.BtnData(btnName4, true, EItemMenuDisplayOrder.Use, delegate()
				{
					this.OpenDetailPanel(index);
				}, null, null, false);
				btnList.Add(btnData4);
				bool interactable3 = !this.IsLock((sbyte)index, this._equipItems[index].Key);
				string btnName5 = LocalStringManager.Get(LanguageKey.LK_EquipCombatSkill_DeleteButton);
				ViewPopupMenu.BtnData btnData5 = new ViewPopupMenu.BtnData(btnName5, interactable3, EItemMenuDisplayOrder.Use, delegate()
				{
					AudioManager.Instance.PlaySound(this.GetEquipSoundName((sbyte)index, this._equipItems[index].Key, false), false, false);
					this.ChangeEquipment((sbyte)index, -1, this._equipItems[index]);
					this.HideDetailPanel();
				}, null, null, false);
				btnList.Add(btnData5);
			}
			bool flag12 = btnList.Count > 0;
			if (flag12)
			{
				CToggle toggle = this.equipSwitchTogGroup.Get(index);
				RectTransform itemRectTrans = toggle.GetComponent<RectTransform>();
				ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
				Vector3 itemScreenPos = UIManager.Instance.UiCamera.WorldToScreenPoint(itemRectTrans.position);
				Vector3 mouseScreenPos = Input.mousePosition;
				itemScreenPos.x = mouseScreenPos.x;
				argBox.SetObject("BtnInfo", btnList);
				argBox.SetObject("ScreenPos", itemScreenPos);
				argBox.SetObject("ItemSize", itemRectTrans.rect.size);
				argBox.SetObject("OnCancel", new Action(this.HideDetailPanel));
				argBox.SetObject("TargetItem", itemData.Clone(-1));
				this.focusItemMask.gameObject.SetActive(true);
				UIElement popupMenu = UIElement.PopupMenu;
				popupMenu.OnHide = (Action)Delegate.Combine(popupMenu.OnHide, new Action(delegate()
				{
					this.focusItemMask.gameObject.SetActive(false);
				}));
				UIElement.PopupMenu.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.PopupMenu, true);
			}
		}

		// Token: 0x060093E5 RID: 37861 RVA: 0x0044F0E8 File Offset: 0x0044D2E8
		private void OpenSetChangeWeaponInnerRatio(int index)
		{
			CToggle toggle = this.equipSwitchTogGroup.Get(index);
			RectTransform itemRectTrans = toggle.GetComponent<RectTransform>();
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.SetObject("ItemRectTrans", itemRectTrans);
			argBox.SetObject("TargetItem", this._equipItems[index].Clone(-1));
			this.focusItemMask.gameObject.SetActive(true);
			UIElement setInnerRatio = UIElement.SetInnerRatio;
			setInnerRatio.OnHide = (Action)Delegate.Combine(setInnerRatio.OnHide, new Action(delegate()
			{
				this.focusItemMask.gameObject.SetActive(false);
				this.HideDetailPanel();
				this.Refresh();
			}));
			UIElement.SetInnerRatio.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.SetInnerRatio, true);
		}

		// Token: 0x060093E6 RID: 37862 RVA: 0x0044F190 File Offset: 0x0044D390
		private bool CheckItemRepairCondition(ItemDisplayData itemData, out bool emptyToolMeet, out bool noNeed, out bool noResource, out bool hasAnySkillMatchedTool)
		{
			emptyToolMeet = false;
			noNeed = false;
			noResource = false;
			hasAnySkillMatchedTool = false;
			bool flag = itemData.Durability >= itemData.MaxDurability;
			bool result;
			if (flag)
			{
				noNeed = true;
				result = false;
			}
			else
			{
				ResourceInts needResource = ItemTemplateHelper.GetRepairNeedResources(itemData.MaterialResources, itemData.Key, itemData.Durability);
				ResourceInts curResource = this._characterItemsDisplayData.Resources;
				bool flag2 = !curResource.CheckIsMeet(ref needResource);
				if (flag2)
				{
					noResource = true;
					result = false;
				}
				else
				{
					bool toolMeet = this.CheckInventoryTool(ItemOperationType.EItemOperationType.Repair, itemData, out emptyToolMeet, out hasAnySkillMatchedTool);
					result = toolMeet;
				}
			}
			return result;
		}

		// Token: 0x060093E7 RID: 37863 RVA: 0x0044F220 File Offset: 0x0044D420
		private unsafe bool CheckInventoryTool(ItemOperationType.EItemOperationType operationType, ItemDisplayData itemData, out bool emptyToolMeet, out bool hasAnySkillMatchedTool)
		{
			sbyte skillType = ItemTemplateHelper.GetCraftRequiredLifeSkillType(itemData.Key.ItemType, itemData.Key.TemplateId);
			short skillAttainment = *this._characterItemsDisplayData.LifeSkillAttainments[(int)skillType];
			short requiredAttainment = ViewCharacterMenuItems.GetOperationRequiredAttainment(operationType, itemData);
			hasAnySkillMatchedTool = false;
			short finalAttainment = UI_Make.GetFinalAttainment(-1, skillAttainment, skillType);
			emptyToolMeet = (finalAttainment >= requiredAttainment);
			bool flag = emptyToolMeet;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				List<ItemDisplayData> skillMatchedToolList = this._inventoryItems.Where(delegate(ItemDisplayData item)
				{
					bool flag2 = item.Key.ItemType != 6 || item.IsLocked;
					bool result2;
					if (flag2)
					{
						result2 = false;
					}
					else
					{
						CraftToolItem toolConfig = CraftTool.Instance[item.Key.TemplateId];
						bool flag3 = !toolConfig.RequiredLifeSkillTypes.Contains(skillType);
						if (flag3)
						{
							result2 = false;
						}
						else
						{
							bool flag4 = item.Durability < 0;
							result2 = !flag4;
						}
					}
					return result2;
				}).ToList<ItemDisplayData>();
				hasAnySkillMatchedTool = (skillMatchedToolList.Count > 0);
				bool toolMeet = skillMatchedToolList.Any(delegate(ItemDisplayData item)
				{
					short finalAttainment2 = UI_Make.GetFinalAttainment(item.Key.TemplateId, skillAttainment, skillType);
					return finalAttainment2 >= requiredAttainment;
				});
				result = toolMeet;
			}
			return result;
		}

		// Token: 0x060093F3 RID: 37875 RVA: 0x0044F674 File Offset: 0x0044D874
		[CompilerGenerated]
		private void <Refresh>g__RefreshCall|81_1()
		{
			this._isNormalCharacter = ((string.IsNullOrWhiteSpace(Character.Instance[this._charDisplayData.TemplateId].FixedAvatarName) && (int)this._charDisplayData.PhysiologicalAge >= GlobalConfig.Instance.AgeBaby) || base.CharacterMenu.IsTaiwuGearMate(base.CharacterMenu.CurCharacterId));
			bool currentCharacterIsTaiwu = base.CharacterMenu.CurrentCharacterIsTaiwu;
			if (currentCharacterIsTaiwu)
			{
				this.RequestCurrEquipmentPlanId();
			}
			this.equipPlanTogGroup.gameObject.SetActive(base.CharacterMenu.CurrentCharacterIsTaiwu && !SingletonObject.getInstance<TutorialChapterModel>().InGuiding);
			this.charDisplayTogGroup.SetInteractable(this._isNormalCharacter);
			CharacterMenuFunctionControlItem config;
			this.equipSwitchTogGroup.SetInteractable(!base.CharacterMenu.TryGetFunctionControlConfig(out config) || !base.CharacterMenu.IsFunctionBanned(config.ItemEquip));
			bool flag = !this._isNormalCharacter;
			if (flag)
			{
				this.charDisplayTogGroup.Set(0, false);
			}
			this.RequestCharDisplayData();
		}

		// Token: 0x040071AE RID: 29102
		[SerializeField]
		private CToggleGroup equipPlanTogGroup;

		// Token: 0x040071AF RID: 29103
		[SerializeField]
		private CToggleGroup charDisplayTogGroup;

		// Token: 0x040071B0 RID: 29104
		[SerializeField]
		private CToggleGroup equipSwitchTogGroup;

		// Token: 0x040071B1 RID: 29105
		[SerializeField]
		private CToggleGroup pageSwitchTogGroup;

		// Token: 0x040071B2 RID: 29106
		[SerializeField]
		private AttributeWithDelta attribute;

		// Token: 0x040071B3 RID: 29107
		[SerializeField]
		private ItemListScroll itemScroll;

		// Token: 0x040071B4 RID: 29108
		[SerializeField]
		private CButton hideScroll;

		// Token: 0x040071B5 RID: 29109
		[SerializeField]
		private TMP_Text equipLoadTitle;

		// Token: 0x040071B6 RID: 29110
		[SerializeField]
		private TMP_Text equipLoad;

		// Token: 0x040071B7 RID: 29111
		[SerializeField]
		private TravelAnimation travelAnimation;

		// Token: 0x040071B8 RID: 29112
		[SerializeField]
		private CharacterAnimation characterAnimation;

		// Token: 0x040071B9 RID: 29113
		[SerializeField]
		private GameObject characterAnimationPauser;

		// Token: 0x040071BA RID: 29114
		[SerializeField]
		private string soundEffectEquipOn;

		// Token: 0x040071BB RID: 29115
		[SerializeField]
		private string soundEffectEquipOff;

		// Token: 0x040071BC RID: 29116
		[SerializeField]
		private RectTransform focusItemMask;

		// Token: 0x040071BD RID: 29117
		private CarrierMaxProperty _maxProperty = new CarrierMaxProperty();

		// Token: 0x040071BE RID: 29118
		[SerializeField]
		private RectTransform page;

		// Token: 0x040071BF RID: 29119
		[SerializeField]
		private RectTransform mainPage;

		// Token: 0x040071C0 RID: 29120
		[SerializeField]
		private RectTransform subPage;

		// Token: 0x040071C1 RID: 29121
		[SerializeField]
		private CharacterCircle charCircle;

		// Token: 0x040071C2 RID: 29122
		[SerializeField]
		private CToggle lockChangeEquipTog;

		// Token: 0x040071C3 RID: 29123
		[SerializeField]
		private CButton autoEquip;

		// Token: 0x040071C4 RID: 29124
		[SerializeField]
		private CharacterMenuEquipItemToggle[] equipments;

		// Token: 0x040071C5 RID: 29125
		[SerializeField]
		private CRawImage background;

		// Token: 0x040071C6 RID: 29126
		[SerializeField]
		private TMP_Text mainText;

		// Token: 0x040071C7 RID: 29127
		[SerializeField]
		private TMP_Text subText;

		// Token: 0x040071C8 RID: 29128
		[SerializeField]
		private CImage mainImg;

		// Token: 0x040071C9 RID: 29129
		[SerializeField]
		private CImage mainHover;

		// Token: 0x040071CA RID: 29130
		[SerializeField]
		private CImage subImg;

		// Token: 0x040071CB RID: 29131
		[SerializeField]
		private CImage subHover;

		// Token: 0x040071CC RID: 29132
		[Header("Loading 分区")]
		[SerializeField]
		private LoadingAnimation mainAreaLoading;

		// Token: 0x040071CD RID: 29133
		[SerializeField]
		private LoadingAnimation attributeAreaLoading;

		// Token: 0x040071CE RID: 29134
		[SerializeField]
		private GameObject mainAreaContentRoot;

		// Token: 0x040071CF RID: 29135
		[SerializeField]
		private GameObject attributeAreaContentRoot;

		// Token: 0x040071D0 RID: 29136
		[SerializeField]
		private float partitionLoadingMinVisibleDuration = 0.1f;

		// Token: 0x040071D1 RID: 29137
		private int _currEquipPlanId;

		// Token: 0x040071D2 RID: 29138
		private CharacterSet _manualChangeEquipCharIds;

		// Token: 0x040071D3 RID: 29139
		private CharacterDisplayData _charDisplayData;

		// Token: 0x040071D4 RID: 29140
		private List<ItemDisplayData> _equipItems = new List<ItemDisplayData>();

		// Token: 0x040071D5 RID: 29141
		private ValueTuple<int, int> _equipLoad;

		// Token: 0x040071D6 RID: 29142
		private List<ItemDisplayData> _inventoryItems = new List<ItemDisplayData>();

		// Token: 0x040071D7 RID: 29143
		private CharacterAttributeDisplayData _baseData = new CharacterAttributeDisplayData();

		// Token: 0x040071D8 RID: 29144
		private CharacterAttributeDisplayData _fullData = new CharacterAttributeDisplayData();

		// Token: 0x040071D9 RID: 29145
		private CharacterAttributeDisplayData _specificData = new CharacterAttributeDisplayData();

		// Token: 0x040071DA RID: 29146
		private sbyte _currSlot = -1;

		// Token: 0x040071DB RID: 29147
		private bool _isMainAreaLoading;

		// Token: 0x040071DC RID: 29148
		private bool _isAttributeAreaLoading;

		// Token: 0x040071DD RID: 29149
		private float _mainAreaLoadingShowStartTime;

		// Token: 0x040071DE RID: 29150
		private float _attributeAreaLoadingShowStartTime;

		// Token: 0x040071DF RID: 29151
		private Coroutine _mainAreaDelayHideCoroutine;

		// Token: 0x040071E0 RID: 29152
		private Coroutine _attributeAreaDelayHideCoroutine;

		// Token: 0x040071E1 RID: 29153
		private Vector2 _mainAreaContentRootOriginalPos;

		// Token: 0x040071E2 RID: 29154
		private CharacterItemsDisplayData _characterItemsDisplayData;

		// Token: 0x040071E3 RID: 29155
		[SerializeField]
		private float activeSize = 208f;

		// Token: 0x040071E4 RID: 29156
		[SerializeField]
		private float inactiveSize = 180f;

		// Token: 0x040071E5 RID: 29157
		[SerializeField]
		private LayoutElement mainLayout;

		// Token: 0x040071E6 RID: 29158
		[SerializeField]
		private LayoutElement subLayout;

		// Token: 0x040071E7 RID: 29159
		[SerializeField]
		private CToggleGroup showCarrierGroup;

		// Token: 0x040071E8 RID: 29160
		private bool _isNormalCharacter;

		// Token: 0x040071E9 RID: 29161
		private bool _animIsPlay = false;

		// Token: 0x040071EA RID: 29162
		private bool _hoveringOnEquipment = false;

		// Token: 0x040071EB RID: 29163
		private CharacterAttributeDisplayData[] _displayData = Array.Empty<CharacterAttributeDisplayData>();

		// Token: 0x040071EC RID: 29164
		private static readonly sbyte[] HideableEquipmentSlot = new sbyte[]
		{
			3,
			5,
			6,
			7
		};

		// Token: 0x040071ED RID: 29165
		[SerializeField]
		private CToggleGroupMultiSelect hideGroup;

		// Token: 0x040071EE RID: 29166
		private bool _firstEnter;

		// Token: 0x040071EF RID: 29167
		private bool _equipOverload;

		// Token: 0x040071F0 RID: 29168
		private SByteList _hideData;

		// Token: 0x040071F1 RID: 29169
		private bool _lockChangeEquipment;

		// Token: 0x040071F2 RID: 29170
		[SerializeField]
		private int moveTo = -860;

		// Token: 0x040071F3 RID: 29171
		[SerializeField]
		private int moveFrom = -2560;

		// Token: 0x040071F4 RID: 29172
		[SerializeField]
		private int shrinkage = -500;

		// Token: 0x040071F5 RID: 29173
		[SerializeField]
		private float animDuration = 0.5f;

		// Token: 0x040071F6 RID: 29174
		[SerializeField]
		private RectTransform equipWeight;

		// Token: 0x040071F7 RID: 29175
		[SerializeField]
		private RectTransform middleContent;

		// Token: 0x020021D4 RID: 8660
		public static class CharacterDisplayToggle
		{
			// Token: 0x0400D714 RID: 55060
			public const int CharacterAvatar = 0;

			// Token: 0x0400D715 RID: 55061
			public const int CharacterPreview = 1;

			// Token: 0x0400D716 RID: 55062
			public const int CharacterCarrier = 2;

			// Token: 0x0400D717 RID: 55063
			public const int CharacterLivestockCarrier = 2;
		}
	}
}
