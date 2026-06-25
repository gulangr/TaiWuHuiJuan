using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using FrameWork.CommandSystem;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Common;
using Game.Views.Combat.Migrate;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.Combat
{
	// Token: 0x02000B15 RID: 2837
	public class CombatRawCreatePage : MonoBehaviour
	{
		// Token: 0x06008B39 RID: 35641 RVA: 0x00405E1C File Offset: 0x0040401C
		private void Awake()
		{
			this.tabGroup.OnActiveIndexChange += this.OnSelectPageIndexChange;
			this.tabGroup.Init(0);
			ToggleGroupHotkeyController.Set(UIElement.Combat, this.tabGroup, 0, null);
			this.tabGroup.Set(0, false);
		}

		// Token: 0x06008B3A RID: 35642 RVA: 0x00405E70 File Offset: 0x00404070
		public void InitSet()
		{
			this._characterAvatar = new CharacterAvatar(this.avatar, true);
			this._equipmentSlotList = this.equipmentHolder.equipmentSlotList;
			this.rawCreateItemSelect.Init("CombatRawCreateDestinationItems");
			this.closeButton.onClick.AddListener(new UnityAction(this.OnIgnoreButtonClick));
			this.ignoreButton.onClick.AddListener(new UnityAction(this.OnIgnoreButtonClick));
			this.confirmButton.onClick.AddListener(new UnityAction(this.OnConfirmButtonClick));
			this._confirmButtonTip = this.confirmButton.GetComponent<TooltipInvoker>();
			bool flag = this._confirmButtonTip == null;
			if (flag)
			{
				this._confirmButtonTip = this.confirmButton.gameObject.AddComponent<TooltipInvoker>();
			}
			this._confirmButtonTip.Type = TipType.SingleDesc;
			this._selectEquipmentItemView = this.equipmentSelectHolder.equipmentSelect;
		}

		// Token: 0x06008B3B RID: 35643 RVA: 0x00405F60 File Offset: 0x00404160
		private void OnDisable()
		{
			bool flag = this._selectEquipmentItemView != null && this._selectEquipmentItemView.gameObject != null;
			if (flag)
			{
				this._selectEquipmentItemView.gameObject.SetActive(false);
			}
			bool flag2 = this.rawCreateItemSelect != null && this.rawCreateItemSelect.ItemListScroll != null && this.rawCreateItemSelect.ItemListScroll.gameObject != null;
			if (flag2)
			{
				this.rawCreateItemSelect.ItemListScroll.gameObject.SetActive(false);
			}
			bool flag3 = this.itemSelBarAnimator != null;
			if (flag3)
			{
				this.itemSelBarAnimator.SetFloat(CombatRawCreatePage.AnimParam_FillProgress, 0f);
			}
		}

		// Token: 0x06008B3C RID: 35644 RVA: 0x00406028 File Offset: 0x00404228
		public void Refresh(List<int> specialEffectList, int charId, string characterName, RawCreateCollection rawCreateCollection)
		{
			this._rawCreateCollection = rawCreateCollection;
			this.UpdateAvatar(charId, characterName);
			CharacterDomainMethod.AsyncCall.GetAllEquipmentItems(null, charId, delegate(int offset, RawDataPool dataPool)
			{
				Serializer.Deserialize(dataPool, offset, ref this._equipments);
				this._specialEffectTemplateIdList = specialEffectList;
				this.RefreshRawCreateTabStrip();
				this.UpdateConfirmButton(false, null);
				this.SetNeedItemShow(false);
				this.UpdateEquipments();
			});
		}

		// Token: 0x06008B3D RID: 35645 RVA: 0x0040606F File Offset: 0x0040426F
		private void UpdateAvatar(int charId, string characterName)
		{
			this._characterAvatar.CharacterId = -1;
			this._characterAvatar.CharacterId = charId;
			this.charName.text = characterName;
		}

		// Token: 0x06008B3E RID: 35646 RVA: 0x0040609C File Offset: 0x0040429C
		private bool CanCreateRaw(sbyte slotIndex)
		{
			bool flag = slotIndex == 4 || slotIndex == 11;
			return !flag;
		}

		// Token: 0x06008B3F RID: 35647 RVA: 0x004060C4 File Offset: 0x004042C4
		private void UpdateEquipments()
		{
			int count = Mathf.Min(this._equipmentSlotList.Count, this._equipments.Count);
			sbyte i = 0;
			while ((int)i < count)
			{
				bool flag = !this.CanCreateRaw(i);
				if (!flag)
				{
					bool flag2 = this._equipments[(int)i].Key.IsValid();
					if (flag2)
					{
						this._equipmentSlotList[(int)i].SetData(this._equipments[(int)i], false, -1, false, true, null, false, false);
					}
					else
					{
						this._equipmentSlotList[(int)i].gameObject.SetActive(false);
					}
				}
				i += 1;
			}
		}

		// Token: 0x06008B40 RID: 35648 RVA: 0x00406178 File Offset: 0x00404378
		private void UpdateEquipmentsRawCreateState(List<sbyte> availableEquipmentSlots)
		{
			availableEquipmentSlots.RemoveAll(new Predicate<sbyte>(this.CheckLocalDisable));
			int count = Mathf.Min(this._equipmentSlotList.Count, this._equipments.Count);
			sbyte i = 0;
			while ((int)i < count)
			{
				bool flag = !this.CanCreateRaw(i);
				if (!flag)
				{
					bool disable = !availableEquipmentSlots.Contains(i);
					this._equipmentSlotList[(int)i].SetMask(disable);
					bool flag2 = disable;
					if (flag2)
					{
						this._equipmentSlotList[(int)i].SetItemNotCanSelectReason(LocalStringManager.Get(this.GetDisableLanguageKey(i)).SetColor("brightred").ColorReplace());
					}
					else
					{
						this._equipmentSlotList[(int)i].HideInteractionState();
					}
					this._equipmentSlotList[(int)i].GetComponent<PointerTrigger>().enabled = !disable;
					this._equipmentSlotList[(int)i].SetInteractable(availableEquipmentSlots.Contains(i));
				}
				i += 1;
			}
			bool flag3 = availableEquipmentSlots.Count > 0;
			if (flag3)
			{
				this.ClearSelectState();
				this._curSelectEquipmentSlotIndex = availableEquipmentSlots[0];
				this._equipmentSlotList[(int)this._curSelectEquipmentSlotIndex].SetHighLight(true);
				this._curSelectEquipmentItemData = this._equipments[(int)this._curSelectEquipmentSlotIndex];
				this.UpdateCanRawCreateItemsFromConfig(this._equipments[(int)this._curSelectEquipmentSlotIndex]);
				this.UpdateSelectEquipment(this._curSelectEquipmentItemData);
				this._selectEquipmentItemView.gameObject.SetActive(true);
				this.rawCreateSelect.gameObject.SetActive(true);
				foreach (sbyte slotIndex in availableEquipmentSlots)
				{
					sbyte index = slotIndex;
					this._equipmentSlotList[(int)slotIndex].SetClickEvent(delegate
					{
						this.OnClickEquipment(index);
					});
				}
			}
			else
			{
				this.rawCreateItemSelect.Clear();
				this._curSelectEquipmentItemData = null;
				this._selectEquipmentItemView.gameObject.SetActive(false);
				int templateId = this._specialEffectTemplateIdList[this._curSelectPageIndex];
				SpecialEffectItem specialEffectItem = SpecialEffect.Instance[templateId];
				this.equipmentSelectHolder.icon.SetSprite(this._rawCreateTypeToIcon[(int)specialEffectItem.RawCreateType], false, null);
				this.rawCreateSelect.gameObject.SetActive(false);
				this.ClearSelectState();
				this.UpdateConfirmButton(false, LocalStringManager.Get(LanguageKey.LK_Combat_RawCreate_MouseTips_10).SetColor("brightred").ColorReplace());
			}
			this.UpdateItemSelBarFillAmount();
		}

		// Token: 0x06008B41 RID: 35649 RVA: 0x00406448 File Offset: 0x00404648
		private LanguageKey GetDisableLanguageKey(sbyte slot)
		{
			bool flag = this.CheckLocalDisable(slot);
			LanguageKey result;
			if (flag)
			{
				result = LanguageKey.LK_Combat_RawCreate_MouseTips_14;
			}
			else
			{
				result = (this._rawCreateCollection.Effects.Keys.Contains(this._equipments[(int)slot].Key) ? LanguageKey.LK_Combat_RawCreate_MouseTips_11 : LanguageKey.LK_Combat_RawCreate_MouseTips_12);
			}
			return result;
		}

		// Token: 0x06008B42 RID: 35650 RVA: 0x004064A4 File Offset: 0x004046A4
		private bool CheckLocalDisable(sbyte slot)
		{
			bool flag = !this._equipments.CheckIndex((int)slot);
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				ItemDisplayData data = this._equipments[(int)slot];
				result = !ItemTemplateHelper.GetAllowRawCreate(data.Key.ItemType, data.Key.TemplateId);
			}
			return result;
		}

		// Token: 0x06008B43 RID: 35651 RVA: 0x004064F8 File Offset: 0x004046F8
		private void OnClickEquipment(sbyte slotIndex)
		{
			this.UpdateSelectState((int)slotIndex);
			this._curSelectEquipmentSlotIndex = slotIndex;
			this.UpdateCanRawCreateItemsFromConfig(this._equipments[(int)slotIndex]);
			this.UpdateSelectEquipment(this._curSelectEquipmentItemData);
		}

		// Token: 0x06008B44 RID: 35652 RVA: 0x0040652A File Offset: 0x0040472A
		private void UpdateSelectState(int slotIndex)
		{
			this.ClearSelectState();
			this._equipmentSlotList[slotIndex].SetHighLight(true);
			this._curSelectEquipmentItemData = this._equipments[slotIndex];
		}

		// Token: 0x06008B45 RID: 35653 RVA: 0x0040655C File Offset: 0x0040475C
		private void ClearSelectState()
		{
			sbyte i = 0;
			while ((int)i < this._equipmentSlotList.Count)
			{
				bool flag = !this.CanCreateRaw(i);
				if (!flag)
				{
					this._equipmentSlotList[(int)i].SetHighLight(false);
				}
				i += 1;
			}
		}

		// Token: 0x06008B46 RID: 35654 RVA: 0x004065AC File Offset: 0x004047AC
		private void UpdateCanRawCreateItemsFromConfig(ItemDisplayData itemDisplayData)
		{
			ItemKey itemKey = itemDisplayData.Key;
			List<TemplateKey> list = new List<TemplateKey>();
			foreach (short destination in ItemTemplateHelper.GetRawCreateDestinations(itemKey.ItemType, itemKey.TemplateId))
			{
				list.Add(new TemplateKey(itemKey.ItemType, destination));
			}
			this.rawCreateItemSelect.Set(list, new Action<TemplateKey>(this.OnRawCreateItemClick), this._curSelectEquipmentSlotIndex);
			this.rawCreateItemSelect.TrySelectFirstMatchingGrade(itemDisplayData);
			bool flag = this.rawCreateItemSelect.SelectedRawCreateItem.TemplateId < 0;
			if (flag)
			{
				this.UpdateConfirmButton(false, CombatRawCreatePage.FormatRequiredNotEnoughTip(LanguageKey.LK_Combat_RawCreate_03.Tr()));
			}
			this.UpdateItemSelBarFillAmount();
		}

		// Token: 0x06008B47 RID: 35655 RVA: 0x00406684 File Offset: 0x00404884
		private void OnRawCreateItemClick(TemplateKey key)
		{
			this.rawCreateSelect.Refresh(key, null, false, true);
			this.UpdateNeedItem();
			this.UpdateItemSelBarFillAmount();
		}

		// Token: 0x06008B48 RID: 35656 RVA: 0x004066A8 File Offset: 0x004048A8
		private void RefreshPageItem(int index, GameObject refers)
		{
			bool flag = index < 0 || index >= this._specialEffectTemplateIdList.Count;
			if (!flag)
			{
				int templateId = this._specialEffectTemplateIdList[index];
				SpecialEffectItem specialEffectItem = SpecialEffect.Instance[templateId];
				EquipmentEffectItem equipmentEffectItem = EquipmentEffect.Instance[specialEffectItem.RawCreateEffect];
				TextMeshProUGUI textComponent = refers.GetComponentInChildren<TextMeshProUGUI>();
				textComponent.SetText(equipmentEffectItem.Name, true);
			}
		}

		// Token: 0x06008B49 RID: 35657 RVA: 0x00406718 File Offset: 0x00404918
		private void OnSelectPageIndexChange(int newIndex, int oldIndex)
		{
			bool flag = this._specialEffectTemplateIdList == null || newIndex < 0 || newIndex >= this._specialEffectTemplateIdList.Count;
			if (!flag)
			{
				this.ApplyRawCreateTabSelection(newIndex);
			}
		}

		// Token: 0x06008B4A RID: 35658 RVA: 0x00406754 File Offset: 0x00404954
		private void RefreshRawCreateTabStrip()
		{
			List<int> list = this._specialEffectTemplateIdList;
			List<CToggle> toggles = this.tabGroup.GetAll();
			int dataCount = (list != null) ? list.Count : 0;
			for (int i = 0; i < toggles.Count; i++)
			{
				GameObject go = toggles[i].gameObject;
				bool flag = i < dataCount;
				if (flag)
				{
					go.SetActive(true);
					this.RefreshPageItem(i, go);
				}
				else
				{
					go.SetActive(false);
				}
			}
			bool flag2 = dataCount == 0;
			if (flag2)
			{
				this._curSelectPageIndex = 0;
				this.description.SetText(string.Empty, true);
				this.rawCreateItemSelect.Clear();
				bool flag3 = this._selectEquipmentItemView != null;
				if (flag3)
				{
					this._selectEquipmentItemView.gameObject.SetActive(false);
				}
				this.rawCreateSelect.gameObject.SetActive(false);
				this.ClearSelectState();
				this.UpdateItemSelBarFillAmount();
			}
			else
			{
				int active = this.tabGroup.GetActiveIndex();
				int targetIndex = Mathf.Clamp((active < 0) ? 0 : active, 0, dataCount - 1);
				bool flag4 = active != targetIndex;
				if (flag4)
				{
					this.tabGroup.SetWithoutNotify(targetIndex);
				}
				this.ApplyRawCreateTabSelection(targetIndex);
			}
		}

		// Token: 0x06008B4B RID: 35659 RVA: 0x00406898 File Offset: 0x00404A98
		private void ApplyRawCreateTabSelection(int tabIndex)
		{
			this._curSelectPageIndex = tabIndex;
			int effectId = this._specialEffectTemplateIdList[tabIndex];
			CombatDomainMethod.AsyncCall.GetAllCanRawCreateEquipmentSlots(null, effectId, delegate(int offset, RawDataPool dataPool)
			{
				List<sbyte> availableEquipmentSlots = new List<sbyte>();
				Serializer.Deserialize(dataPool, offset, ref availableEquipmentSlots);
				this.UpdateEquipmentsRawCreateState(availableEquipmentSlots);
			});
			SpecialEffectItem config = SpecialEffect.Instance[effectId];
			this.description.SetText(EquipmentEffect.Instance[config.RawCreateEffect].Desc, true);
		}

		// Token: 0x06008B4C RID: 35660 RVA: 0x004068FC File Offset: 0x00404AFC
		private void UpdateNeedItem()
		{
			short materialTemplateId = ItemTemplateHelper.GetRawCreateMaterial(this._curSelectEquipmentItemData.Key.ItemType, this._curSelectEquipmentItemData.Key.TemplateId, this.rawCreateItemSelect.SelectedRawCreateItem.TemplateId);
			TaiwuDomainMethod.AsyncCall.GetAllItems(null, ItemSourceType.Inventory, false, delegate(int offset, RawDataPool dataPool)
			{
				ValueTuple<ItemSourceType, List<ItemDisplayData>> tuple = default(ValueTuple<ItemSourceType, List<ItemDisplayData>>);
				Serializer.Deserialize(dataPool, offset, ref tuple);
				bool canConfirm = true;
				bool needMaterial = materialTemplateId != -1;
				bool flag = needMaterial;
				if (flag)
				{
					int count = 0;
					foreach (ItemDisplayData inventoryItem in tuple.Item2)
					{
						bool flag2 = inventoryItem.Key.ItemType == 5 && inventoryItem.Key.TemplateId == materialTemplateId;
						if (flag2)
						{
							count = inventoryItem.Amount;
						}
					}
					bool flag3 = this._curSelectPageIndex < 0 || this._curSelectPageIndex >= this._specialEffectTemplateIdList.Count;
					if (flag3)
					{
						Debug.Log(string.Format("_curSelectPageIndex wrong,_curSelectPageIndex:{0}", this._curSelectPageIndex));
						return;
					}
					int templateId = this._specialEffectTemplateIdList[this._curSelectPageIndex];
					SpecialEffectItem specialEffectItem = SpecialEffect.Instance[templateId];
					int needCount = specialEffectItem.RawCreateRequireMaterialCount;
					canConfirm = (needCount <= count);
					StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
					stringBuilder.Clear();
					stringBuilder.Append(count.ToString().SetColor(canConfirm ? "lightblue" : "brightred")).Append("/").Append(needCount.ToString());
					string countText = stringBuilder.ToString();
					EasyPool.Free<StringBuilder>(stringBuilder);
					this.needItem.Refresh(new TemplateKey(5, materialTemplateId), null, false, true);
					TextMeshProUGUI countTmp;
					bool flag4 = this.needItem.CTryGet<TextMeshProUGUI>("Count", out countTmp);
					if (flag4)
					{
						countTmp.SetText(countText, true);
					}
				}
				this.SetNeedItemShow(needMaterial);
				bool hasDestination = this.rawCreateItemSelect.SelectedRawCreateItem.TemplateId >= 0;
				bool canConfirmFinal = hasDestination && canConfirm;
				string disableTip = null;
				bool flag5 = !canConfirmFinal;
				if (flag5)
				{
					disableTip = ((!hasDestination) ? CombatRawCreatePage.FormatRequiredNotEnoughTip(LanguageKey.LK_Combat_RawCreate_03.Tr()) : CombatRawCreatePage.FormatRequiredNotEnoughTip(ItemTemplateHelper.GetName(5, materialTemplateId)));
				}
				this.UpdateConfirmButton(canConfirmFinal, disableTip);
			});
		}

		// Token: 0x06008B4D RID: 35661 RVA: 0x00406967 File Offset: 0x00404B67
		private void SetNeedItemShow(bool show)
		{
			this.needItem.gameObject.SetActive(show);
			this.noNeedItem.gameObject.SetActive(!show);
		}

		// Token: 0x06008B4E RID: 35662 RVA: 0x00406991 File Offset: 0x00404B91
		private void UpdateConfirmButton(bool interactable, string disableTip = null)
		{
			this.confirmButton.interactable = interactable;
			this.RefreshConfirmButtonTips(interactable, disableTip);
		}

		// Token: 0x06008B4F RID: 35663 RVA: 0x004069AA File Offset: 0x00404BAA
		private static string FormatRequiredNotEnoughTip(string targetName)
		{
			return LocalStringManager.GetFormat(LanguageKey.LK_ItemMenu_NotEnough, targetName ?? "").ColorReplace();
		}

		// Token: 0x06008B50 RID: 35664 RVA: 0x004069C8 File Offset: 0x00404BC8
		private void RefreshConfirmButtonTips(bool interactable, string disableTip)
		{
			bool flag = this._confirmButtonTip == null;
			if (!flag)
			{
				bool flag2 = interactable || string.IsNullOrEmpty(disableTip);
				if (flag2)
				{
					this._confirmButtonTip.enabled = false;
					this._confirmButtonTip.HideTips();
				}
				else
				{
					this._confirmButtonTip.enabled = true;
					this._confirmButtonTip.Type = TipType.SingleDesc;
					TooltipInvoker confirmButtonTip = this._confirmButtonTip;
					if (confirmButtonTip.RuntimeParam == null)
					{
						confirmButtonTip.RuntimeParam = new ArgumentBox();
					}
					this._confirmButtonTip.RuntimeParam.Set("arg0", disableTip);
					bool showing = this._confirmButtonTip.Showing;
					if (showing)
					{
						this._confirmButtonTip.Refresh(true, -1);
					}
				}
			}
		}

		// Token: 0x06008B51 RID: 35665 RVA: 0x00406A84 File Offset: 0x00404C84
		private void OnConfirmButtonClick()
		{
			AudioManager.Instance.PlaySound("ui_combat_rawcreate_confirm", false, false);
			CommandManager.AddCommandMethodCall<int, sbyte, short>(EPriority.DoRawCreate, 8, 89, this._specialEffectTemplateIdList[this._curSelectPageIndex], this._curSelectEquipmentSlotIndex, this.rawCreateItemSelect.SelectedRawCreateItem.TemplateId, delegate(int offset, RawDataPool dataPool)
			{
				bool result = false;
				Serializer.Deserialize(dataPool, offset, ref result);
				bool flag = !result;
				if (flag)
				{
					Debug.LogWarning("DoRawCreate失败");
				}
			}, null);
		}

		// Token: 0x06008B52 RID: 35666 RVA: 0x00406AF5 File Offset: 0x00404CF5
		private void OnIgnoreButtonClick()
		{
			CommandManager.AddCommandMethodCall<int>(EPriority.IgnoreRawCreate, 8, 88, this._specialEffectTemplateIdList[this._curSelectPageIndex], delegate(int offset, RawDataPool dataPool)
			{
				bool result = false;
				Serializer.Deserialize(dataPool, offset, ref result);
				bool flag = !result;
				if (flag)
				{
					Debug.LogWarning("IgnoreRawCreate失败");
				}
			}, null);
		}

		// Token: 0x06008B53 RID: 35667 RVA: 0x00406B33 File Offset: 0x00404D33
		private void OnEnable()
		{
			this._lastClickRightMouseTime = Time.realtimeSinceStartup;
		}

		// Token: 0x06008B54 RID: 35668 RVA: 0x00406B44 File Offset: 0x00404D44
		private void Update()
		{
			bool flag = CommonCommandKit.RightMouse.Check(UIElement.Combat, false, false, false, true, false) && base.gameObject.activeSelf;
			if (flag)
			{
				bool flag2 = this.CheckTimeAndIgnore();
				if (flag2)
				{
					return;
				}
			}
			bool flag3 = CommonCommandKit.Esc.Check(UIElement.Combat, false, false, false, true, false) && base.gameObject.activeSelf;
			if (flag3)
			{
				bool flag4 = this.CheckTimeAndIgnore();
				if (flag4)
				{
					return;
				}
			}
			bool flag5 = CommonCommandKit.Space.Check(UIElement.Combat, false, false, false, true, false) && base.gameObject.activeSelf;
			if (flag5)
			{
				bool interactable = this.confirmButton.interactable;
				if (interactable)
				{
					this.OnConfirmButtonClick();
				}
			}
		}

		// Token: 0x06008B55 RID: 35669 RVA: 0x00406C04 File Offset: 0x00404E04
		private bool CheckTimeAndIgnore()
		{
			bool flag = Time.realtimeSinceStartup - this._lastClickRightMouseTime > 0.3f;
			bool result;
			if (flag)
			{
				bool flag2 = CommonCommandKit.Esc.Check(UIElement.Combat, false, false, false, true, false);
				if (flag2)
				{
					UIManager.Instance.SetEscHandler(delegate
					{
					});
				}
				this.OnIgnoreButtonClick();
				this._lastClickRightMouseTime = Time.realtimeSinceStartup;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06008B56 RID: 35670 RVA: 0x00406C8C File Offset: 0x00404E8C
		private void UpdateSelectEquipment(ItemDisplayData itemDisplayData)
		{
			this._selectEquipmentItemView.SetData(itemDisplayData, false, -1, false, true, null, false, true);
		}

		// Token: 0x06008B57 RID: 35671 RVA: 0x00406CB0 File Offset: 0x00404EB0
		private void UpdateItemSelBarFillAmount()
		{
			bool flag = this.itemSelBar == null;
			if (!flag)
			{
				bool flag2 = this.itemSelBarAnimator == null;
				if (!flag2)
				{
					ItemViewWithColor equipView = (this.equipmentSelectHolder != null) ? this.equipmentSelectHolder.equipmentSelect : null;
					bool flag3 = equipView == null || !equipView.gameObject.activeSelf;
					if (flag3)
					{
						this.itemSelBar.fillAmount = 0f;
						this.itemSelBarAnimator.SetFloat(CombatRawCreatePage.AnimParam_FillProgress, 0f);
					}
					else
					{
						TemplateKey selected = (this.rawCreateItemSelect != null) ? this.rawCreateItemSelect.SelectedRawCreateItem : default(TemplateKey);
						this.itemSelBar.fillAmount = ((selected.TemplateId >= 0) ? 1f : 0.6f);
						this.itemSelBarAnimator.SetFloat(CombatRawCreatePage.AnimParam_FillProgress, (selected.TemplateId >= 0) ? 1f : 0.6f);
					}
				}
			}
		}

		// Token: 0x04006ABA RID: 27322
		[SerializeField]
		private CButton closeButton;

		// Token: 0x04006ABB RID: 27323
		[SerializeField]
		private CToggleGroup tabGroup;

		// Token: 0x04006ABC RID: 27324
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x04006ABD RID: 27325
		[SerializeField]
		private TextMeshProUGUI charName;

		// Token: 0x04006ABE RID: 27326
		[SerializeField]
		private CombatEquipmentHolder equipmentHolder;

		// Token: 0x04006ABF RID: 27327
		[SerializeField]
		private CombatRawCreateItemSelect rawCreateItemSelect;

		// Token: 0x04006AC0 RID: 27328
		[SerializeField]
		private CButton ignoreButton;

		// Token: 0x04006AC1 RID: 27329
		[SerializeField]
		private CButton confirmButton;

		// Token: 0x04006AC2 RID: 27330
		[SerializeField]
		private ConfigItemViewWithColor needItem;

		// Token: 0x04006AC3 RID: 27331
		[SerializeField]
		private GameObject noNeedItem;

		// Token: 0x04006AC4 RID: 27332
		[SerializeField]
		private TextMeshProUGUI description;

		// Token: 0x04006AC5 RID: 27333
		[SerializeField]
		private CombatEquipmentSelectHolder equipmentSelectHolder;

		// Token: 0x04006AC6 RID: 27334
		[SerializeField]
		private ConfigItemViewWithColor rawCreateSelect;

		// Token: 0x04006AC7 RID: 27335
		[SerializeField]
		private CImage itemSelBar;

		// Token: 0x04006AC8 RID: 27336
		[SerializeField]
		private Animator itemSelBarAnimator;

		// Token: 0x04006AC9 RID: 27337
		private static readonly int AnimParam_FillProgress = Animator.StringToHash("Progress");

		// Token: 0x04006ACA RID: 27338
		private List<int> _specialEffectTemplateIdList;

		// Token: 0x04006ACB RID: 27339
		private int _curSelectPageIndex;

		// Token: 0x04006ACC RID: 27340
		private CharacterAvatar _characterAvatar;

		// Token: 0x04006ACD RID: 27341
		private List<ItemDisplayData> _equipments;

		// Token: 0x04006ACE RID: 27342
		private List<ItemViewWithColor> _equipmentSlotList;

		// Token: 0x04006ACF RID: 27343
		private ItemDisplayData _curSelectEquipmentItemData;

		// Token: 0x04006AD0 RID: 27344
		private sbyte _curSelectEquipmentSlotIndex;

		// Token: 0x04006AD1 RID: 27345
		private RawCreateCollection _rawCreateCollection;

		// Token: 0x04006AD2 RID: 27346
		private ItemView _selectEquipmentItemView;

		// Token: 0x04006AD3 RID: 27347
		private readonly string[] _rawCreateTypeToIcon = new string[]
		{
			"sp_icon_equip_6",
			"sp_icon_equip_6",
			"sp_icon_equip_6",
			"sp_icon_equip_3",
			"sp_icon_equip_0"
		};

		// Token: 0x04006AD4 RID: 27348
		private readonly string[] _rawCreateTypeToNameLString = new string[]
		{
			"LK_ItemType_0",
			"LK_ItemType_0",
			"LK_ItemType_0",
			"LK_ItemType_1",
			"LK_ItemType_2"
		};

		// Token: 0x04006AD5 RID: 27349
		private float _lastClickRightMouseTime = -1f;

		// Token: 0x04006AD6 RID: 27350
		private TooltipInvoker _confirmButtonTip;
	}
}
