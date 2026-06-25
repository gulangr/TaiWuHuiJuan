using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using Game.Components.Common;
using Game.Views.MouseTips.Item.Common;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Item.Display;
using GameData.Domains.Taiwu;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.MouseTips
{
	// Token: 0x02000856 RID: 2134
	public class MouseTipFuyu : MouseTipItem
	{
		// Token: 0x17000C6D RID: 3181
		// (get) Token: 0x06006793 RID: 26515 RVA: 0x002F521D File Offset: 0x002F341D
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06006794 RID: 26516 RVA: 0x002F5220 File Offset: 0x002F3420
		protected override void Init(ArgumentBox argsBox)
		{
			base.Init(argsBox);
			ItemDisplayData itemData;
			argsBox.Get<ItemDisplayData>("ItemData", out itemData);
			argsBox.Get("TemplateDataOnly", out this._templateDataOnly);
			this._itemData = itemData;
			MiscItem configData = Misc.Instance[itemData.Key.TemplateId];
			bool flag = this.basicArea != null;
			if (flag)
			{
				this.basicArea.Set(itemData, this._templateDataOnly);
			}
			this.InitItemTypeArea(itemData, configData);
			this.InitCombatUseArea(itemData, configData);
			this.RefreshFuyuFaith(true);
			TaiwuDomainMethod.AsyncCall.GetAllCharacterPropertyBonusData(this, new AsyncMethodCallbackDelegate(this.HandleCharacterPropertyBonusData));
		}

		// Token: 0x06006795 RID: 26517 RVA: 0x002F52C3 File Offset: 0x002F34C3
		private void Update()
		{
			this.UpdateDetail();
			this.CheckHotkeyDisplayViewEncyclopedia();
		}

		// Token: 0x06006796 RID: 26518 RVA: 0x002F52D4 File Offset: 0x002F34D4
		private void InitItemTypeArea(ItemDisplayData itemData, MiscItem configData)
		{
			bool flag = this.itemType == null;
			if (!flag)
			{
				List<Game.Views.MouseTips.Item.Common.ItemFunction> disableFunctions = new List<Game.Views.MouseTips.Item.Common.ItemFunction>();
				bool flag2 = !configData.Repairable;
				if (flag2)
				{
					disableFunctions.Add(Game.Views.MouseTips.Item.Common.ItemFunction.Repairable);
				}
				bool flag3 = !configData.Transferable;
				if (flag3)
				{
					disableFunctions.Add(Game.Views.MouseTips.Item.Common.ItemFunction.Transferable);
				}
				bool flag4 = !configData.Poisonable;
				if (flag4)
				{
					disableFunctions.Add(Game.Views.MouseTips.Item.Common.ItemFunction.Poisonable);
				}
				bool flag5 = !configData.Refinable;
				if (flag5)
				{
					disableFunctions.Add(Game.Views.MouseTips.Item.Common.ItemFunction.Refinable);
				}
				this.itemType.Set(disableFunctions);
			}
		}

		// Token: 0x06006797 RID: 26519 RVA: 0x002F5360 File Offset: 0x002F3560
		private void InitCombatUseArea(ItemDisplayData itemData, MiscItem configData)
		{
			bool flag = this.combatUse == null;
			if (!flag)
			{
				this.combatUse.Set((int)configData.ConsumedFeatureMedals, true);
			}
		}

		// Token: 0x06006798 RID: 26520 RVA: 0x002F5394 File Offset: 0x002F3594
		private void RefreshFuyuFaith(bool addListener = false)
		{
			bool showFaith = SingletonObject.getInstance<TaiwuCharacterModel>().HasInheritedTaiwu;
			bool showStoneHouse = SingletonObject.getInstance<BasicGameData>().XiangshuProgress >= 6;
			bool flag = this.swordGrip != null;
			if (flag)
			{
				this.swordGrip.gameObject.SetActive(showFaith);
				this.swordGrip.Set(showStoneHouse);
			}
			bool flag2 = this.faith != null;
			if (flag2)
			{
				this.faith.Set(this._fuyuFaith, showFaith);
			}
			bool flag3 = addListener && !this.MonitorFields.Contains(new UIBase.MonitorDataField(4, 39, ulong.MaxValue, null));
			if (flag3)
			{
				this.MonitorFields.Add(new UIBase.MonitorDataField(4, 39, ulong.MaxValue, null));
			}
		}

		// Token: 0x06006799 RID: 26521 RVA: 0x002F5450 File Offset: 0x002F3650
		private void HandleCharacterPropertyBonusData(int offset, RawDataPool dataPool)
		{
			this._taiwuPropertyPermanentBonuses = new List<CharacterPropertyBonus>();
			Serializer.Deserialize(dataPool, offset, ref this._taiwuPropertyPermanentBonuses);
			int[] mainAttributeTypes = new int[]
			{
				0,
				1,
				2,
				3,
				4,
				5,
				112,
				113,
				114,
				115,
				116,
				117
			};
			bool flag = this.mainAttribute != null;
			if (flag)
			{
				this.mainAttribute.Set(this.GetFormattedBonuses(mainAttributeTypes, true));
			}
			int[] basicBonusTypes = new int[]
			{
				105,
				101,
				104,
				94,
				95,
				96,
				97,
				98,
				99,
				100
			};
			bool flag2 = this.basicBonus != null;
			if (flag2)
			{
				this.basicBonus.Set(this.GetFormattedBonuses(basicBonusTypes, true));
			}
			int[] specialBonusTypes = new int[]
			{
				133,
				134,
				108,
				109,
				119,
				120,
				121,
				122,
				123,
				124,
				125,
				126,
				127,
				128,
				129,
				130,
				131,
				132
			};
			bool flag3 = this.specialBonus != null;
			if (flag3)
			{
				this.specialBonus.Set(this.GetFormattedBonuses(specialBonusTypes, true));
			}
			int[] attributeBonusTypes = new int[]
			{
				10,
				11,
				16,
				17,
				6,
				7,
				8,
				9,
				12,
				13,
				14,
				15
			};
			bool flag4 = this.attributeBonus != null;
			if (flag4)
			{
				this.attributeBonus.Set(this.GetFormattedBonuses(attributeBonusTypes, true));
			}
			bool flag5 = this.bodyDamage != null;
			if (flag5)
			{
				bool flag6 = this._taiwuPropertyPermanentBonuses.Count > 118;
				if (flag6)
				{
					CharacterPropertyBonus damageBonus = this._taiwuPropertyPermanentBonuses[118];
					this.bodyDamage.Set(damageBonus.AddValue, damageBonus.AddPercent);
				}
				else
				{
					this.bodyDamage.Set(0, 0);
				}
			}
			int[] combatSkillTypes = new int[]
			{
				66,
				67,
				68,
				69,
				70,
				71,
				72,
				73,
				74,
				75,
				76,
				77,
				78,
				79
			};
			bool flag7 = this.combatSkill != null;
			if (flag7)
			{
				this.combatSkill.Set(this.GetFormattedBonuses(combatSkillTypes, true));
			}
			int[] lifeSkillTypes = new int[]
			{
				34,
				35,
				36,
				37,
				38,
				39,
				40,
				41,
				42,
				43,
				44,
				45,
				46,
				47,
				48,
				49
			};
			bool flag8 = this.lifeSkill != null;
			if (flag8)
			{
				this.lifeSkill.Set(this.GetFormattedBonuses(lifeSkillTypes, true));
			}
			int taiwuId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			ExtraDomainMethod.AsyncCall.GetPoisonImmunities(this, taiwuId, delegate(int poisonOffset, RawDataPool pool)
			{
				byte poisonImmunities = 0;
				Serializer.Deserialize(pool, poisonOffset, ref poisonImmunities);
				bool flag9 = this.poisonResist != null;
				if (flag9)
				{
					int[] poisonTypes = new int[]
					{
						28,
						29,
						30,
						31,
						32,
						33
					};
					this.poisonResist.Set(this.GetPoisonBonuses(poisonTypes, poisonImmunities));
				}
				RectTransform partOne = this.detailPartOne.GetComponent<RectTransform>();
				bool showPartOne = false;
				for (int j = 0; j < partOne.childCount; j++)
				{
					Transform child2 = partOne.GetChild(j);
					bool activeSelf2 = child2.gameObject.activeSelf;
					if (activeSelf2)
					{
						showPartOne = true;
						break;
					}
				}
				partOne.gameObject.SetActive(showPartOne);
			});
			RectTransform partTwo = this.detailPartTwo.GetComponent<RectTransform>();
			bool showPartTwo = false;
			for (int i = 0; i < partTwo.childCount; i++)
			{
				Transform child = partTwo.GetChild(i);
				bool activeSelf = child.gameObject.activeSelf;
				if (activeSelf)
				{
					showPartTwo = true;
					break;
				}
			}
			partTwo.gameObject.SetActive(showPartTwo);
		}

		// Token: 0x0600679A RID: 26522 RVA: 0x002F56B8 File Offset: 0x002F38B8
		public override void OnNotifyGameData(List<NotificationWrapper> notifications)
		{
			foreach (NotificationWrapper notification in notifications)
			{
				bool flag = notification.Notification.Type == 0;
				if (flag)
				{
					this.OnDataReceived(notification.Notification, notification.DataPool);
				}
			}
		}

		// Token: 0x0600679B RID: 26523 RVA: 0x002F5728 File Offset: 0x002F3928
		private void OnDataReceived(Notification notification, RawDataPool pool)
		{
			DataUid uid = notification.Uid;
			bool flag = uid.DomainId == 4 && uid.DataId == 39;
			if (flag)
			{
				Serializer.Deserialize(pool, notification.ValueOffset, ref this._fuyuFaith);
				this.RefreshFuyuFaith(false);
			}
		}

		// Token: 0x0600679C RID: 26524 RVA: 0x002F5774 File Offset: 0x002F3974
		[return: TupleElementNames(new string[]
		{
			"icon",
			"title",
			"value",
			"percent"
		})]
		private List<ValueTuple<string, string, int, int>> GetFormattedBonuses(int[] types, bool checkProfessionSkill = true)
		{
			List<ValueTuple<string, string, int, int>> result = new List<ValueTuple<string, string, int, int>>();
			bool flag = this._taiwuPropertyPermanentBonuses == null;
			List<ValueTuple<string, string, int, int>> result2;
			if (flag)
			{
				result2 = result;
			}
			else
			{
				bool hasCivilianSkill3 = SingletonObject.getInstance<ProfessionModel>().IsSkillEquipped(43);
				foreach (int typeId in types)
				{
					bool flag2 = typeId < 0 || typeId >= this._taiwuPropertyPermanentBonuses.Count;
					if (!flag2)
					{
						CharacterPropertyBonus property = this._taiwuPropertyPermanentBonuses[typeId];
						int value = property.AddValue;
						int percent = property.AddPercent;
						bool flag3 = checkProfessionSkill && hasCivilianSkill3 && value >= 0 && percent >= 0;
						if (flag3)
						{
							value = value * 150 / 100;
							percent = percent * 150 / 100;
						}
						bool flag4 = value == 0 && percent == 0;
						if (!flag4)
						{
							CharacterPropertyReferencedItem refItem = CharacterPropertyReferenced.Instance.GetItem((short)typeId);
							bool flag5 = refItem == null || refItem.DisplayType < 0;
							if (!flag5)
							{
								CharacterPropertyDisplayItem displayItem = CharacterPropertyDisplay.Instance.GetItem(refItem.DisplayType);
								bool flag6 = displayItem == null;
								if (!flag6)
								{
									result.Add(new ValueTuple<string, string, int, int>(displayItem.TipsIcon, displayItem.Name, value, percent));
								}
							}
						}
					}
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x0600679D RID: 26525 RVA: 0x002F58CC File Offset: 0x002F3ACC
		[return: TupleElementNames(new string[]
		{
			"icon",
			"title",
			"value",
			"percent",
			"isImmune"
		})]
		private List<ValueTuple<string, string, int, int, bool>> GetPoisonBonuses(int[] types, byte poisonImmunities)
		{
			List<ValueTuple<string, string, int, int, bool>> result = new List<ValueTuple<string, string, int, int, bool>>();
			bool flag = this._taiwuPropertyPermanentBonuses == null;
			List<ValueTuple<string, string, int, int, bool>> result2;
			if (flag)
			{
				result2 = result;
			}
			else
			{
				foreach (int typeId in types)
				{
					bool flag2 = typeId < 0 || typeId >= this._taiwuPropertyPermanentBonuses.Count;
					if (!flag2)
					{
						int poisonIndex = typeId - 28;
						bool flag3 = poisonIndex < 0 || poisonIndex > 5;
						if (flag3)
						{
							poisonIndex = 0;
						}
						bool isImmune = BitOperation.GetBit(poisonImmunities, poisonIndex);
						CharacterPropertyBonus property = this._taiwuPropertyPermanentBonuses[typeId];
						int value = property.AddValue;
						int percent = property.AddPercent;
						bool flag4 = !isImmune && value == 0 && percent == 0;
						if (!flag4)
						{
							CharacterPropertyReferencedItem refItem = CharacterPropertyReferenced.Instance.GetItem((short)typeId);
							bool flag5 = refItem == null || refItem.DisplayType < 0;
							if (!flag5)
							{
								CharacterPropertyDisplayItem displayItem = CharacterPropertyDisplay.Instance.GetItem(refItem.DisplayType);
								bool flag6 = displayItem == null;
								if (!flag6)
								{
									result.Add(new ValueTuple<string, string, int, int, bool>(displayItem.TipsIcon, displayItem.Name, value, percent, isImmune));
								}
							}
						}
					}
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x0600679E RID: 26526 RVA: 0x002F5A08 File Offset: 0x002F3C08
		protected void UpdateDetail()
		{
			bool altDown = this.Element != null && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));
			this.detailHotKey.Refresh(altDown ? EHotKeyDisplayType.CancelDetail : EHotKeyDisplayType.Detail);
			this.detailPart.gameObject.SetActive(altDown);
			LayoutRebuilder.ForceRebuildLayoutImmediate(base.transform as RectTransform);
			base.SetAllowOverlapLayout(altDown);
		}

		// Token: 0x0600679F RID: 26527 RVA: 0x002F5A7C File Offset: 0x002F3C7C
		private void CheckHotkeyDisplayViewEncyclopedia()
		{
			bool flag = this.Element != null && CommonCommandKit.PrimaryInteraction.Check(this.Element, true, true, false, true, false);
			if (flag)
			{
				ArgumentBox args = EasyPool.Get<ArgumentBox>().Set("key", EncyclopediaTipLink.DefValue.FuyuSwordGrip.RefName);
				UIElement.Encyclopedia.SetOnInitArgs(args);
				UIManager.Instance.ShowUI(UIElement.Encyclopedia, true);
			}
		}

		// Token: 0x0400492D RID: 18733
		[Header("分区")]
		[SerializeField]
		private GameObject detailPart;

		// Token: 0x0400492E RID: 18734
		[SerializeField]
		private GameObject detailPartOne;

		// Token: 0x0400492F RID: 18735
		[SerializeField]
		private GameObject detailPartTwo;

		// Token: 0x04004930 RID: 18736
		[Header("快捷键")]
		[SerializeField]
		protected Game.Components.Common.HotkeyDisplay detailHotKey;

		// Token: 0x04004931 RID: 18737
		[SerializeField]
		protected Game.Components.Common.HotkeyDisplay encyclopediaHotKey;

		// Token: 0x04004932 RID: 18738
		[Header("物品组")]
		[SerializeField]
		private MouseTipFuyuBasicArea basicArea;

		// Token: 0x04004933 RID: 18739
		[SerializeField]
		private MouseTipFuyuItemType itemType;

		// Token: 0x04004934 RID: 18740
		[SerializeField]
		private MouseTipFuyuCombatUse combatUse;

		// Token: 0x04004935 RID: 18741
		[SerializeField]
		private MouseTipFuyuSwordGrip swordGrip;

		// Token: 0x04004936 RID: 18742
		[SerializeField]
		private MouseTipFuyuFaith faith;

		// Token: 0x04004937 RID: 18743
		[SerializeField]
		private MouseTipFuyuMainAttribute mainAttribute;

		// Token: 0x04004938 RID: 18744
		[SerializeField]
		private MouseTipFuyuBasicBonus basicBonus;

		// Token: 0x04004939 RID: 18745
		[SerializeField]
		private MouseTipFuyuSpecialBonus specialBonus;

		// Token: 0x0400493A RID: 18746
		[SerializeField]
		private MouseTipFuyuAttributeBonus attributeBonus;

		// Token: 0x0400493B RID: 18747
		[SerializeField]
		private MouseTipFuyuBodyDamage bodyDamage;

		// Token: 0x0400493C RID: 18748
		[SerializeField]
		private MouseTipFuyuPoisonResist poisonResist;

		// Token: 0x0400493D RID: 18749
		[SerializeField]
		private MouseTipFuyuCombatSkill combatSkill;

		// Token: 0x0400493E RID: 18750
		[SerializeField]
		private MouseTipFuyuLifeSkill lifeSkill;

		// Token: 0x0400493F RID: 18751
		private int _fuyuFaith;

		// Token: 0x04004940 RID: 18752
		private List<CharacterPropertyBonus> _taiwuPropertyPermanentBonuses;

		// Token: 0x04004941 RID: 18753
		private bool _templateDataOnly;
	}
}
