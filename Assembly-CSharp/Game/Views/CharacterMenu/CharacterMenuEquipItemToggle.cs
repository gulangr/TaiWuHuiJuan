using System;
using System.Runtime.CompilerServices;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Character.Display;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameDataExtensions;
using TMPro;
using UnityEngine;

namespace Game.Views.CharacterMenu
{
	// Token: 0x02000B64 RID: 2916
	public class CharacterMenuEquipItemToggle : MonoBehaviour
	{
		// Token: 0x17000FB6 RID: 4022
		// (get) Token: 0x0600906E RID: 36974 RVA: 0x00435198 File Offset: 0x00433398
		// (set) Token: 0x0600906F RID: 36975 RVA: 0x004351A0 File Offset: 0x004333A0
		public bool HasContent
		{
			get
			{
				return this.hasContent;
			}
			set
			{
				bool flag = this.hasContent == value;
				if (!flag)
				{
					GameObject gameObject = this.notEquipBase.gameObject;
					this.hasContent = value;
					gameObject.SetActive(!value);
					this.equipNameBg.gameObject.SetActive(value);
					this.gradeBack.gameObject.SetActive(value);
					foreach (CharacterMenuEquipItemProperty label in this.subLabels)
					{
						label.gameObject.SetActive(value);
					}
					bool flag2 = !value;
					if (flag2)
					{
						this.SetMouseTip(null);
						bool flag3 = this.defaultSprite != null;
						if (flag3)
						{
							this.mainIcon.sprite = this.defaultSprite;
						}
						else
						{
							this.mainIcon.enabled = false;
						}
					}
				}
			}
		}

		// Token: 0x06009070 RID: 36976 RVA: 0x00435272 File Offset: 0x00433472
		private void Awake()
		{
			GEvent.Add(UiEvents.OnLanguageChange, new GEvent.Callback(this.RefreshLang));
			this.RefreshLang(null);
		}

		// Token: 0x06009071 RID: 36977 RVA: 0x00435299 File Offset: 0x00433499
		private void OnDestroy()
		{
			GEvent.Remove(UiEvents.OnLanguageChange, new GEvent.Callback(this.RefreshLang));
		}

		// Token: 0x06009072 RID: 36978 RVA: 0x004352B8 File Offset: 0x004334B8
		private void RefreshLang(ArgumentBox _)
		{
			bool flag = this.hasContent;
			if (!flag)
			{
				this.hasContent = true;
				this.HasContent = false;
			}
		}

		// Token: 0x06009073 RID: 36979 RVA: 0x004352E4 File Offset: 0x004334E4
		public void Set(ItemDisplayData itemDisplayData, byte avatarId)
		{
			this._avatarId = avatarId;
			bool flag = !(this.HasContent = (itemDisplayData != null && itemDisplayData.Key.ItemType != -1));
			if (!flag)
			{
				this.equipName.text = itemDisplayData.GetName(false);
				this.SetSprite(itemDisplayData);
				this.SetProperties(itemDisplayData);
				this.SetMouseTip(itemDisplayData);
			}
		}

		// Token: 0x06009074 RID: 36980 RVA: 0x00435350 File Offset: 0x00433550
		private void SetMouseTip(ItemDisplayData itemDisplayData)
		{
			bool hasNoContent = this.mousetipEquip.Type == TipType.SingleDesc;
			bool flag = itemDisplayData != null && itemDisplayData.Key.IsValid();
			if (flag)
			{
				bool flag2 = hasNoContent;
				if (flag2)
				{
					this.mousetipEquip.HideTips();
				}
				bool templateDataOnly = !itemDisplayData.RealKey.IsValid();
				TooltipInvoker tooltipInvoker = this.mousetipEquip;
				short itemSubType = ItemTemplateHelper.GetItemSubType(itemDisplayData.Key.ItemType, itemDisplayData.Key.TemplateId);
				if (!true)
				{
				}
				TipType type;
				if (itemSubType - 403 > 1)
				{
					type = TooltipManager.ItemTypeToTipType[itemDisplayData.Key.ItemType];
				}
				else
				{
					type = TipType.Jiao;
				}
				if (!true)
				{
				}
				tooltipInvoker.Type = type;
				this.mousetipEquip.NeedRefresh = false;
				TooltipInvoker tooltipInvoker2 = this.mousetipEquip;
				ArgumentBox argumentBox;
				if ((argumentBox = tooltipInvoker2.RuntimeParam) == null)
				{
					argumentBox = (tooltipInvoker2.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				argumentBox.SetObject("ItemData", itemDisplayData.Clone(-1)).Set("TemplateDataOnly", templateDataOnly).Set("CharId", itemDisplayData.OwnerCharId).Set("EquipSlot", this.slot).Set("IsNew", false);
			}
			else
			{
				bool flag3 = !hasNoContent;
				if (flag3)
				{
					this.mousetipEquip.HideTips();
				}
				this.mousetipEquip.Type = TipType.SingleDesc;
				TooltipInvoker tooltipInvoker2 = this.mousetipEquip;
				ArgumentBox argumentBox2;
				if ((argumentBox2 = tooltipInvoker2.RuntimeParam) == null)
				{
					argumentBox2 = (tooltipInvoker2.RuntimeParam = EasyPool.Get<ArgumentBox>());
				}
				argumentBox2.Set("arg0", CharacterMenuEquipItemToggle.SlotToKey[this.slot].Tr());
			}
		}

		// Token: 0x06009075 RID: 36981 RVA: 0x004354F0 File Offset: 0x004336F0
		public void SetCarrierMaxProperty(CarrierMaxProperty cmp)
		{
			TooltipInvoker tooltipInvoker = this.mousetipEquip;
			ArgumentBox argumentBox;
			if ((argumentBox = tooltipInvoker.RuntimeParam) == null)
			{
				argumentBox = (tooltipInvoker.RuntimeParam = EasyPool.Get<ArgumentBox>());
			}
			argumentBox.SetObject("CarrierMaxProperty", cmp);
		}

		// Token: 0x06009076 RID: 36982 RVA: 0x0043552C File Offset: 0x0043372C
		private void SetSprite(ItemDisplayData data)
		{
			bool flag = data != null && data.Key.IsValid();
			if (flag)
			{
				this.mainIcon.SetSprite(ItemTemplateHelper.GetIcon(data.Key.ItemType, data.Key.TemplateId), false, null);
			}
			else
			{
				this.mainIcon.sprite = this.defaultSprite;
			}
			this.gradeBack.color = Colors.Instance.GradeColors[(int)((data != null) ? data.Grade : 0)];
		}

		// Token: 0x06009077 RID: 36983 RVA: 0x004355B8 File Offset: 0x004337B8
		private void SetProperties(ItemDisplayData data)
		{
			switch (this.slot)
			{
			case 0:
			case 1:
			case 2:
				this.subLabels[2].SetPower(data);
				this.subLabels[1].SetDurability(data);
				this.subLabels[0].SetWeight(data);
				break;
			case 3:
			case 5:
			case 6:
			case 7:
				this.subLabels[2].SetPower(data);
				this.subLabels[1].SetDurability(data);
				this.subLabels[0].SetWeight(data);
				break;
			case 4:
				this.subLabels[0].gameObject.SetActive(false);
				break;
			case 8:
			case 9:
			case 10:
				this.subLabels[1].SetDurability(data);
				this.subLabels[0].SetWeight(data);
				break;
			case 11:
				this.subLabels[0].SetDurability(data);
				break;
			case 12:
				this.subLabels[1].SetTamePoint(data);
				this.subLabels[0].SetDurability(data);
				break;
			case 13:
				this.subLabels[1].SetTamePoint(data);
				this.subLabels[0].SetDurability(data);
				break;
			case 14:
			case 15:
			case 16:
				this.subLabels[1].SetInventoryLoad(data);
				this.subLabels[0].SetDurability(data);
				break;
			default:
				this.HasContent = false;
				break;
			}
		}

		// Token: 0x06009079 RID: 36985 RVA: 0x0043573F File Offset: 0x0043393F
		// Note: this type is marked as 'beforefieldinit'.
		static CharacterMenuEquipItemToggle()
		{
			LanguageKey[] array = new LanguageKey[17];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.BD8F728BD35B38378A49E75A5391E8331F49AFF1B8900485AA72CD31A7C499DF).FieldHandle);
			CharacterMenuEquipItemToggle.EmptyName = array;
			LanguageKey[] array2 = new LanguageKey[17];
			RuntimeHelpers.InitializeArray(array2, fieldof(<PrivateImplementationDetails>.4446DA843DBF4BB8406BDB2AC269170452608E55862BFBB360E41DB40F5C0931).FieldHandle);
			CharacterMenuEquipItemToggle.SlotToKey = array2;
		}

		// Token: 0x04006F15 RID: 28437
		private static readonly LanguageKey[] EmptyName;

		// Token: 0x04006F16 RID: 28438
		private static readonly LanguageKey[] SlotToKey;

		// Token: 0x04006F17 RID: 28439
		[SerializeField]
		internal int slot;

		// Token: 0x04006F18 RID: 28440
		[SerializeField]
		private bool hasContent;

		// Token: 0x04006F19 RID: 28441
		[SerializeField]
		private Sprite defaultSprite;

		// Token: 0x04006F1A RID: 28442
		[SerializeField]
		private CImage mainIcon;

		// Token: 0x04006F1B RID: 28443
		[SerializeField]
		private CImage gradeBack;

		// Token: 0x04006F1C RID: 28444
		[SerializeField]
		private TMP_Text equipName;

		// Token: 0x04006F1D RID: 28445
		[SerializeField]
		private CImage equipNameBg;

		// Token: 0x04006F1E RID: 28446
		[SerializeField]
		private CharacterMenuEquipItemProperty[] subLabels;

		// Token: 0x04006F1F RID: 28447
		[SerializeField]
		internal CToggle toggle;

		// Token: 0x04006F20 RID: 28448
		[SerializeField]
		private TooltipInvoker mousetipEquip;

		// Token: 0x04006F21 RID: 28449
		[SerializeField]
		private TMP_Text notEquip;

		// Token: 0x04006F22 RID: 28450
		[SerializeField]
		private CImage notEquipBase;

		// Token: 0x04006F23 RID: 28451
		private byte _avatarId;
	}
}
