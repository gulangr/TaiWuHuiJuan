using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.CharacterMenu;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F2C RID: 3884
	public class Injury : MonoBehaviour
	{
		// Token: 0x17001430 RID: 5168
		// (get) Token: 0x0600B29C RID: 45724 RVA: 0x005147F3 File Offset: 0x005129F3
		public CharacterInjuryDisplayData Data
		{
			get
			{
				return this._characterInjuryDisplayData;
			}
		}

		// Token: 0x17001431 RID: 5169
		// (get) Token: 0x0600B29D RID: 45725 RVA: 0x005147FB File Offset: 0x005129FB
		private bool IsTaiwu
		{
			get
			{
				return this._characterInjuryDisplayData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			}
		}

		// Token: 0x17001432 RID: 5170
		// (get) Token: 0x0600B29E RID: 45726 RVA: 0x00514814 File Offset: 0x00512A14
		private bool IsTaiwuGearMate
		{
			get
			{
				return SingletonObject.getInstance<CharacterMonitorModel>().IsTaiwuGearMate(this._characterInjuryDisplayData.CharacterId);
			}
		}

		// Token: 0x17001433 RID: 5171
		// (get) Token: 0x0600B29F RID: 45727 RVA: 0x0051482B File Offset: 0x00512A2B
		private ViewCharacterMenu CharacterMenu
		{
			get
			{
				return UIElement.CharacterMenu.UiBaseAs<ViewCharacterMenu>();
			}
		}

		// Token: 0x17001434 RID: 5172
		// (get) Token: 0x0600B2A0 RID: 45728 RVA: 0x00514837 File Offset: 0x00512A37
		private bool CharacterMenuCanNotOperate
		{
			get
			{
				return this.CharacterMenu.Element.IsShowing && !this.CharacterMenu.CanOperate;
			}
		}

		// Token: 0x0600B2A1 RID: 45729 RVA: 0x0051485C File Offset: 0x00512A5C
		private void Awake()
		{
			this._attributeAndInjuryDynamic = base.GetComponentInParent<AttributeAndInjuryDynamic>(true);
			this.buttonUseMedicine.ClearAndAddListener(delegate
			{
				this.OnClickShowMedicineItem(UsingMedicineItemType.Invalid);
			});
			this.buttonHeal.ClearAndAddListener(new Action(this.OnClickShowHealUI));
			bool flag = this.buttonEatArea != null;
			if (flag)
			{
				this.buttonEatArea.ClearAndAddListener(new Action(this.OnClickEatArea));
			}
			this.InitButtonTips();
		}

		// Token: 0x0600B2A2 RID: 45730 RVA: 0x005148D8 File Offset: 0x00512AD8
		private void OnEnable()
		{
			GlobalDomainMethod.Call.InvokeGuidingTrigger(101);
		}

		// Token: 0x0600B2A3 RID: 45731 RVA: 0x005148E3 File Offset: 0x00512AE3
		private void OnClickEatArea()
		{
			Action onClickEatArea = this._onClickEatArea;
			if (onClickEatArea != null)
			{
				onClickEatArea();
			}
		}

		// Token: 0x0600B2A4 RID: 45732 RVA: 0x005148F8 File Offset: 0x00512AF8
		private void InitButtonTips()
		{
			bool flag = this.buttonUseMedicineTip != null;
			if (flag)
			{
				this.buttonUseMedicineTip.Type = TipType.Simple;
				this.buttonUseMedicineTip.IsLanguageKey = true;
				this.buttonUseMedicineTip.PresetParam = new string[]
				{
					"LK_UsingMedicine_Title",
					"LK_UsingMedicine_Tip"
				};
			}
			bool flag2 = this.buttonHealTip != null;
			if (flag2)
			{
				this.buttonHealTip.Type = TipType.Simple;
				this.buttonHealTip.IsLanguageKey = true;
				this.buttonHealTip.PresetParam = new string[]
				{
					"LK_Ui_Heal_Title",
					"LK_Heal_Tip"
				};
			}
		}

		// Token: 0x0600B2A5 RID: 45733 RVA: 0x0051499C File Offset: 0x00512B9C
		public void SetAreaInteract(Action onClickEatArea)
		{
			this._onClickEatArea = onClickEatArea;
			this.injuryEatArea.SetAreaInteract(onClickEatArea);
		}

		// Token: 0x0600B2A6 RID: 45734 RVA: 0x005149B3 File Offset: 0x00512BB3
		public void SetEatAreaInteractable(bool canClickEatArea)
		{
			this.buttonEatArea.interactable = canClickEatArea;
		}

		// Token: 0x0600B2A7 RID: 45735 RVA: 0x005149C4 File Offset: 0x00512BC4
		public void Set(CharacterInjuryDisplayData displayData, bool interactive = true)
		{
			bool flag = displayData == null;
			if (!flag)
			{
				this._characterInjuryDisplayData = displayData;
				this.injuryPartArea.Set(displayData);
				this.injuryQiArea.Set(displayData);
				this.injuryHealthArea.Set(displayData);
				this.injuryPoisonArea.Set(displayData);
				this.injuryEatArea.Set(displayData);
				this.attribute.RefreshMainAttributes(displayData.CurMainAttributes, displayData.MaxMainAttributes);
				this.attribute.RefreshMainAttributeRecoveries(displayData.MainAttributeRecoveries);
				this.attribute.RefreshMainAttributeTips(displayData.MainAttributeRecoveries);
				this.RefreshAllHealBtn(displayData.CharacterId, !interactive);
			}
		}

		// Token: 0x0600B2A8 RID: 45736 RVA: 0x00514A74 File Offset: 0x00512C74
		public void Set(int charId)
		{
			CharacterDomainMethod.AsyncCall.GetCharacterInjuryDisplayData(null, charId, delegate(int offset, RawDataPool pool)
			{
				Serializer.Deserialize(pool, offset, ref this._characterInjuryDisplayData);
				this.Set(this._characterInjuryDisplayData, true);
			});
		}

		// Token: 0x0600B2A9 RID: 45737 RVA: 0x00514A8C File Offset: 0x00512C8C
		public void PlayTakeEffect(IEnumerable<sbyte> targetBodyParts)
		{
			this.injuryPartArea.PlayTakeEffect();
			bool flag = targetBodyParts == null;
			if (!flag)
			{
				foreach (sbyte bodyPartType in targetBodyParts)
				{
					InjuryPartItem injuryPartItem = this.injuryPartArea.GetInjuryPartItem(bodyPartType);
					if (injuryPartItem != null)
					{
						injuryPartItem.PlayTakeEffect();
					}
				}
			}
		}

		// Token: 0x0600B2AA RID: 45738 RVA: 0x00514B04 File Offset: 0x00512D04
		public void DelayRefreshOnEatItemSend()
		{
			InjuryDynamic injuryDynamic = base.GetComponentInParent<InjuryDynamic>();
			bool flag = injuryDynamic;
			if (!flag)
			{
				SingletonObject.getInstance<YieldHelper>().DelayFrameDo(5U, delegate
				{
					this.Set(this._characterInjuryDisplayData.CharacterId);
				});
			}
		}

		// Token: 0x0600B2AB RID: 45739 RVA: 0x00514B40 File Offset: 0x00512D40
		public void RefreshAllHealBtn(int charId, bool disableAll = false)
		{
			bool flag = this._characterInjuryDisplayData == null;
			if (!flag)
			{
				CharacterMonitorModel characterMonitorModel = SingletonObject.getInstance<CharacterMonitorModel>();
				bool isCanHealTeammate = characterMonitorModel.IsTaiwuTeamCharacter(charId) || characterMonitorModel.IsTaiwuSpecialTeammate(charId);
				bool isOnBeggarUltimate = UIManager.Instance.IsElementActive(UIElement.BeggarSkill3);
				CharacterMenuFunctionControlItem config;
				bool disable = !isCanHealTeammate || this.CharacterMenuCanNotOperate || this._characterInjuryDisplayData.TemplateId == 880 || (this.CharacterMenu.TryGetFunctionControlConfig(out config) && this.CharacterMenu.IsFunctionBanned(config.Medicine)) || isOnBeggarUltimate;
				if (disableAll)
				{
					this.buttonHeal.interactable = false;
					bool flag2 = this.textHeal != null;
					if (flag2)
					{
						this.textHeal.color = Colors.Instance["grey"];
					}
				}
				else
				{
					this.buttonHeal.interactable = !disable;
					bool flag3 = this.textHeal != null;
					if (flag3)
					{
						this.textHeal.color = Colors.Instance[(!disable) ? "pinkyellow" : "grey"];
					}
				}
				bool isOnUsingMedicineItem = UIManager.Instance.IsElementActive(UIElement.UsingMedicineItem);
				bool flag4 = disable || isOnUsingMedicineItem || characterMonitorModel.IsTaiwuGearMate(charId);
				if (flag4)
				{
					this._canUseMedicineItem = false;
				}
				else
				{
					this._canUseMedicineItem = (!disableAll && this._characterInjuryDisplayData.InventoryMedicineItemCount > 0 && (!this.CharacterMenu.TryGetFunctionControlConfig(out config) || !this.CharacterMenu.IsFunctionBanned(config.Medicine)));
				}
				this.buttonUseMedicine.interactable = this._canUseMedicineItem;
				bool flag5 = this.textUseMedicine != null;
				if (flag5)
				{
					this.textUseMedicine.color = Colors.Instance[this._canUseMedicineItem ? "pinkyellow" : "grey"];
				}
				this.RefreshInjuryAndPoison(this._canUseMedicineItem);
			}
		}

		// Token: 0x0600B2AC RID: 45740 RVA: 0x00514D34 File Offset: 0x00512F34
		private void RefreshInjuryAndPoison(bool isEnabled)
		{
			Injury.<>c__DisplayClass45_0 CS$<>8__locals1 = new Injury.<>c__DisplayClass45_0();
			CS$<>8__locals1.<>4__this = this;
			bool isSelectInjuryPart = this.IsSelectInjuryPart;
			if (!isSelectInjuryPart)
			{
				CS$<>8__locals1.isUsingMedicineItemPage = (UIElement.UsingMedicineItem.IsShowing || UIElement.Combat.IsShowing);
				CS$<>8__locals1.interactable = (CS$<>8__locals1.isUsingMedicineItemPage || isEnabled);
				for (sbyte i = 0; i < 7; i += 1)
				{
					InjuryPartItem item = this.injuryPartArea.GetInjuryPartItem(i);
					CS$<>8__locals1.<RefreshInjuryAndPoison>g__SetPointerTriggerAndButton|0(item.Button, (short)i + UsingMedicineItemType.BodyPartTypeChest, item.HSVStyleRoot);
				}
				for (sbyte j = 0; j < 6; j += 1)
				{
					InjuryPoisonItem item2 = this.injuryPoisonArea.GetInjuryPoisonItem(j);
					CS$<>8__locals1.<RefreshInjuryAndPoison>g__SetPointerTriggerAndButton|0(item2.Button, (short)j + UsingMedicineItemType.PoisonTypeHot, null);
				}
				CS$<>8__locals1.<RefreshInjuryAndPoison>g__SetPointerTriggerAndButton|0(this.injuryHealthArea.Button, UsingMedicineItemType.Health, null);
				CS$<>8__locals1.<RefreshInjuryAndPoison>g__SetPointerTriggerAndButton|0(this.injuryHealthArea.SliderButton, UsingMedicineItemType.Health, null);
				CS$<>8__locals1.<RefreshInjuryAndPoison>g__SetPointerTriggerAndButton|0(this.injuryQiArea.Button, UsingMedicineItemType.DisorderOfQi, null);
				CS$<>8__locals1.<RefreshInjuryAndPoison>g__SetPointerTriggerAndButton|0(this.injuryQiArea.SliderButton, UsingMedicineItemType.DisorderOfQi, null);
			}
		}

		// Token: 0x0600B2AD RID: 45741 RVA: 0x00514E6A File Offset: 0x0051306A
		public void UsingMedicineItemSwitch(short type)
		{
			GEvent.OnEvent(UiEvents.UsingMedicineItemSwitch, EasyPool.Get<ArgumentBox>().Set("UsingMedicineItemType", type));
		}

		// Token: 0x0600B2AE RID: 45742 RVA: 0x00514E8C File Offset: 0x0051308C
		public void OnClickShowHealUI()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			CharacterMonitorModel monitor = SingletonObject.getInstance<CharacterMonitorModel>();
			List<int> teamCharList = monitor.GetTaiwuTeamCharIds();
			teamCharList.AddRange(monitor.GetTaiwuSpecialGroup());
			argBox.SetObject("DoctorList", teamCharList);
			List<int> patientList = new List<int>();
			patientList.AddRange(teamCharList);
			argBox.SetObject("PatientList", patientList);
			argBox.Set("NeedPay", false);
			argBox.Set("CurrentCharacterId", this._characterInjuryDisplayData.CharacterId);
			CharacterDomainMethod.AsyncCall.GetSomeoneKidnapCharacters(null, SingletonObject.getInstance<BasicGameData>().TaiwuCharId, delegate(int offset, RawDataPool dataPool)
			{
				KidnappedCharacterList kidnappedCharacterList = null;
				Serializer.Deserialize(dataPool, offset, ref kidnappedCharacterList);
				bool flag = kidnappedCharacterList != null;
				if (flag)
				{
					for (int i = 0; i < kidnappedCharacterList.GetCount(); i++)
					{
						patientList.Add(kidnappedCharacterList.Get(i).CharId);
					}
				}
				UIElement.Heal.SetOnInitArgs(argBox);
				UIManager.Instance.ShowUI(UIElement.Heal, true);
			});
		}

		// Token: 0x0600B2AF RID: 45743 RVA: 0x00514F54 File Offset: 0x00513154
		public void OnClickShowMedicineItem(short type)
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CurrentCharacterId", this._characterInjuryDisplayData.CharacterId).Set("UsingMedicineItemType", type);
			UIElement.UsingMedicineItem.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.UsingMedicineItem, true);
		}

		// Token: 0x0600B2B0 RID: 45744 RVA: 0x00514FA8 File Offset: 0x005131A8
		public void OnClickShowMedicineItem(short type, int charId)
		{
			((this._attributeAndInjuryDynamic != null) ? this._attributeAndInjuryDynamic : base.GetComponentInParent<AttributeAndInjuryDynamic>(true)).SwitchToInjury();
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("CurrentCharacterId", charId).Set("UsingMedicineItemType", type);
			UIElement.UsingMedicineItem.SetOnInitArgs(argBox);
			UIManager.Instance.ShowUI(UIElement.UsingMedicineItem, true);
		}

		// Token: 0x0600B2B1 RID: 45745 RVA: 0x00515014 File Offset: 0x00513214
		public void ShowInfectNotice(ITradeableContent itemDisplayData, int amount = 1)
		{
			this._showingInfectNotice = true;
			SimulateEatingEffectResult simulateEatingEffectResult = new SimulateEatingEffectResult();
			CharacterDomainMethod.AsyncCall.SimulateEatingEffect(null, this._characterInjuryDisplayData.CharacterId, itemDisplayData.Key, amount, delegate(int offset, RawDataPool dataPool)
			{
				bool flag = !this._showingInfectNotice;
				if (!flag)
				{
					Serializer.Deserialize(dataPool, offset, ref simulateEatingEffectResult);
					this.HandleShowInfectNotice(itemDisplayData, simulateEatingEffectResult);
				}
			});
		}

		// Token: 0x0600B2B2 RID: 45746 RVA: 0x00515074 File Offset: 0x00513274
		public void ShowInfectNoticeWithDoctor(ITradeableContent itemDisplayData, int amount, int doctorId)
		{
			this._showingInfectNotice = true;
			SimulateEatingEffectResult simulateEatingEffectResult = new SimulateEatingEffectResult();
			CharacterDomainMethod.AsyncCall.SimulateProfessionDoctorSkill0(null, this._characterInjuryDisplayData.CharacterId, itemDisplayData.Key, amount, doctorId, delegate(int offset, RawDataPool dataPool)
			{
				bool flag = !this._showingInfectNotice;
				if (!flag)
				{
					Serializer.Deserialize(dataPool, offset, ref simulateEatingEffectResult);
					this.HandleShowInfectNotice(itemDisplayData, simulateEatingEffectResult);
				}
			});
		}

		// Token: 0x0600B2B3 RID: 45747 RVA: 0x005150D4 File Offset: 0x005132D4
		private void HandleShowInfectNotice(ITradeableContent itemDisplayData, SimulateEatingEffectResult simulateEatingEffectResult)
		{
			AttributeAndInjuryDynamic attributeAndInjuryDynamic = this._attributeAndInjuryDynamic;
			if (attributeAndInjuryDynamic != null)
			{
				attributeAndInjuryDynamic.TempSetTableState(1);
			}
			switch (itemDisplayData.Key.ItemType)
			{
			case 7:
			{
				FoodItem foodItem = Food.Instance.GetItem(itemDisplayData.Key.TemplateId);
				break;
			}
			case 8:
			{
				MedicineItem medicineItem = Medicine.Instance.GetItem(itemDisplayData.Key.TemplateId);
				bool flag = medicineItem != null;
				if (flag)
				{
					switch (medicineItem.EffectType)
					{
					case EMedicineEffectType.RecoverOuterInjury:
					case EMedicineEffectType.RecoverInnerInjury:
						this.injuryPartArea.ShowInfectNotice(simulateEatingEffectResult.Injuries);
						break;
					case EMedicineEffectType.RecoverHealth:
						this.injuryHealthArea.ShowInfectNotice((short)simulateEatingEffectResult.Health);
						break;
					case EMedicineEffectType.ChangeDisorderOfQi:
						this.injuryQiArea.ShowInfectNotice(simulateEatingEffectResult);
						break;
					case EMedicineEffectType.DetoxPoison:
					case EMedicineEffectType.ApplyPoison:
						this.injuryPoisonArea.ShowInfectNotice(medicineItem, simulateEatingEffectResult.Poisons);
						break;
					}
				}
				break;
			}
			case 9:
			{
				TeaWineItem teaWineItem = TeaWine.Instance.GetItem(itemDisplayData.Key.TemplateId);
				this.injuryQiArea.ShowInfectNotice(simulateEatingEffectResult);
				break;
			}
			}
		}

		// Token: 0x0600B2B4 RID: 45748 RVA: 0x0051520C File Offset: 0x0051340C
		public void HideNotice(bool backToPrevState = true, bool refreshData = true)
		{
			this._showingInfectNotice = false;
			if (backToPrevState)
			{
				AttributeAndInjuryDynamic attributeAndInjuryDynamic = this._attributeAndInjuryDynamic;
				if (attributeAndInjuryDynamic != null)
				{
					attributeAndInjuryDynamic.BackToPrevState();
				}
			}
			if (refreshData)
			{
				this.Set(this._characterInjuryDisplayData, true);
			}
			bool flag = this._usingMedicineItemType != UsingMedicineItemType.Invalid;
			if (flag)
			{
				this.SelectPartByUsingMedicineItemType(this._usingMedicineItemType);
			}
		}

		// Token: 0x0600B2B5 RID: 45749 RVA: 0x0051526C File Offset: 0x0051346C
		public void ShowEatNotice(ITradeableContent itemDisplayData, int amount = 1)
		{
			bool flag = itemDisplayData.Key.ItemType == 12 && itemDisplayData.Key.TemplateId != 265;
			if (!flag)
			{
				bool flag2;
				if (itemDisplayData.Key.ItemType == 8)
				{
					MedicineItem medicineItem = Medicine.Instance[itemDisplayData.Key.TemplateId];
					short? num = (medicineItem != null) ? new short?(medicineItem.Duration) : null;
					int? num2 = (num != null) ? new int?((int)num.GetValueOrDefault()) : null;
					int num3 = 0;
					flag2 = (num2.GetValueOrDefault() == num3 & num2 != null);
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (!flag3)
				{
					this.injuryEatArea.ShowEatNotice(itemDisplayData, amount);
					AttributeAndInjuryDynamic attributeAndInjuryDynamic = this._attributeAndInjuryDynamic;
					if (attributeAndInjuryDynamic != null)
					{
						attributeAndInjuryDynamic.TempSetTableState(1);
					}
				}
			}
		}

		// Token: 0x0600B2B6 RID: 45750 RVA: 0x0051534C File Offset: 0x0051354C
		public void UnselectPartByUsingMedicineItemType()
		{
			GameObject refers = this.GetPartRefersByUsingMedicineItemType(this._usingMedicineItemType);
			if (refers != null)
			{
				refers.SetActive(false);
			}
			this._usingMedicineItemType = UsingMedicineItemType.Invalid;
		}

		// Token: 0x0600B2B7 RID: 45751 RVA: 0x00515380 File Offset: 0x00513580
		public void SelectPartByUsingMedicineItemType(short type)
		{
			bool flag = this._usingMedicineItemType > UsingMedicineItemType.Invalid;
			if (flag)
			{
				this.UnselectPartByUsingMedicineItemType();
			}
			GameObject refers = this.GetPartRefersByUsingMedicineItemType(type);
			if (refers != null)
			{
				refers.SetActive(true);
			}
			this._usingMedicineItemType = type;
		}

		// Token: 0x0600B2B8 RID: 45752 RVA: 0x005153C4 File Offset: 0x005135C4
		private GameObject GetPartRefersByUsingMedicineItemType(short type)
		{
			bool flag = type == UsingMedicineItemType.Invalid;
			GameObject result;
			if (flag)
			{
				result = null;
			}
			else
			{
				for (sbyte i = 0; i < 7; i += 1)
				{
					bool flag2 = type - UsingMedicineItemType.BodyPartTypeChest == (short)i;
					if (flag2)
					{
						return this.injuryPartArea.GetInjuryPartItem(i).Selected;
					}
				}
				for (sbyte j = 0; j < 6; j += 1)
				{
					bool flag3 = type - UsingMedicineItemType.PoisonTypeHot == (short)j;
					if (flag3)
					{
						return this.injuryPoisonArea.GetInjuryPoisonItem(j).Selected;
					}
				}
				bool flag4 = type == UsingMedicineItemType.Health;
				if (flag4)
				{
					result = this.injuryHealthArea.Selected;
				}
				else
				{
					bool flag5 = type == UsingMedicineItemType.DisorderOfQi;
					if (flag5)
					{
						result = this.injuryQiArea.Selected;
					}
					else
					{
						result = null;
					}
				}
			}
			return result;
		}

		// Token: 0x0600B2B9 RID: 45753 RVA: 0x005154A0 File Offset: 0x005136A0
		public unsafe bool HasAttributeToTopical(ItemKey itemKey)
		{
			bool flag = itemKey.ItemType != 8;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				MedicineItem medicineItem = Medicine.Instance.GetItem(itemKey.TemplateId);
				result = (medicineItem.RequiredMainAttributeType < 0 || *this._characterInjuryDisplayData.CurMainAttributes[(int)medicineItem.RequiredMainAttributeType] >= (short)medicineItem.RequiredMainAttributeValue);
			}
			return result;
		}

		// Token: 0x0600B2BA RID: 45754 RVA: 0x00515508 File Offset: 0x00513708
		public unsafe static bool IsRecoverInnerOuterMedicineCanUse(CharacterInjuryDisplayData data, short templateId, out string tipContent)
		{
			tipContent = string.Empty;
			MedicineItem medicineItem = Medicine.Instance.GetItem(templateId);
			bool flag = medicineItem == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = medicineItem.EffectType != EMedicineEffectType.RecoverInnerInjury && medicineItem.EffectType > EMedicineEffectType.RecoverOuterInjury;
				if (flag2)
				{
					tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
					result = false;
				}
				else
				{
					bool flag3 = medicineItem.Duration == 0;
					if (flag3)
					{
						bool isCombat = UIManager.Instance.IsElementActive(UIElement.Combat);
						Injuries injuries = data.Injuries;
						ValueTuple<sbyte, sbyte> sum = injuries.GetBothSum();
						sbyte value = (medicineItem.EffectType == EMedicineEffectType.RecoverOuterInjury) ? sum.Item1 : sum.Item2;
						bool hasInjury = value > 0;
						bool flag4 = !isCombat && !hasInjury;
						if (flag4)
						{
							tipContent = LanguageKey.LK_UsingMedicine_Tip_NoNeed.Tr();
							return false;
						}
						bool flag5 = hasInjury;
						if (flag5)
						{
							bool isInner = medicineItem.EffectType == EMedicineEffectType.RecoverInnerInjury;
							bool isMeetEffectThreshold = false;
							for (sbyte i = 0; i < 7; i += 1)
							{
								sbyte injuryValue = injuries.Get(i, isInner);
								bool flag6 = injuryValue > 0 && (short)injuryValue <= medicineItem.EffectThresholdValue;
								if (flag6)
								{
									isMeetEffectThreshold = true;
									break;
								}
							}
							bool flag7 = !isMeetEffectThreshold;
							if (flag7)
							{
								tipContent = LanguageKey.LK_UsingMedicine_Tip_Value_Not_Enough.Tr();
								return false;
							}
						}
					}
					bool flag8 = medicineItem.RequiredMainAttributeType >= 0 && *data.CurMainAttributes[(int)medicineItem.RequiredMainAttributeType] < (short)medicineItem.RequiredMainAttributeValue;
					if (flag8)
					{
						ECharacterPropertyDisplayType type = ECharacterPropertyDisplayType.Strength + (int)medicineItem.RequiredMainAttributeType;
						CharacterPropertyDisplayItem characterPropertyDisplayItem = CharacterPropertyDisplay.Instance[type.ToInt()];
						tipContent = LocalStringManager.GetFormat(LanguageKey.LK_UsingMedicine_Tip_Attribute_Not_Enough, characterPropertyDisplayItem.Name);
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x0600B2BB RID: 45755 RVA: 0x005156C8 File Offset: 0x005138C8
		public static bool CheckMedicineItemIsLocked(CharacterInjuryDisplayData data, ITradeableContent itemData, int usingMedicineItemType, out string tipContent)
		{
			Injury.<>c__DisplayClass60_0 CS$<>8__locals1;
			CS$<>8__locals1.data = data;
			tipContent = string.Empty;
			bool flag = itemData.Key.ItemType != 8;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				CS$<>8__locals1.medicineItem = Medicine.Instance[itemData.Key.TemplateId];
				CS$<>8__locals1.isCombat = UIManager.Instance.IsElementActive(UIElement.Combat);
				bool isOuterMedicine = CS$<>8__locals1.medicineItem.EffectType == EMedicineEffectType.RecoverOuterInjury;
				bool isInnerMedicine = CS$<>8__locals1.medicineItem.EffectType == EMedicineEffectType.RecoverInnerInjury;
				bool healOuterLocked = SingletonObject.getInstance<DisplayTriggerModel>().HealingOuterRestriction;
				bool healInnerLocked = SingletonObject.getInstance<DisplayTriggerModel>().HealingInnerRestriction;
				bool medicineLocked = (isOuterMedicine && healOuterLocked) || (isInnerMedicine && healInnerLocked);
				bool flag2 = medicineLocked;
				if (flag2)
				{
					tipContent = LocalStringManager.Get(LanguageKey.LK_Use_Medicine_Tip_NotAllow);
					result = true;
				}
				else
				{
					EMedicineEffectType effectType = CS$<>8__locals1.medicineItem.EffectType;
					bool flag3 = effectType == EMedicineEffectType.RecoverInnerInjury || effectType <= EMedicineEffectType.RecoverOuterInjury;
					if (flag3)
					{
						bool flag4 = usingMedicineItemType > (int)UsingMedicineItemType.Invalid && (usingMedicineItemType < (int)UsingMedicineItemType.BodyPartTypeChest || usingMedicineItemType > (int)UsingMedicineItemType.BodyPartTypeRightLeg);
						if (flag4)
						{
							tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
							result = true;
						}
						else
						{
							result = !Injury.IsRecoverInnerOuterMedicineCanUse(CS$<>8__locals1.data, itemData.Key.TemplateId, out tipContent);
						}
					}
					else
					{
						bool flag5 = CS$<>8__locals1.medicineItem.EffectType == EMedicineEffectType.DetoxPoison;
						if (flag5)
						{
							bool flag6 = usingMedicineItemType > (int)UsingMedicineItemType.Invalid && (usingMedicineItemType < (int)UsingMedicineItemType.PoisonTypeHot || usingMedicineItemType > (int)UsingMedicineItemType.PoisonTypeIllusory);
							if (flag6)
							{
								tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
								result = true;
							}
							else
							{
								bool flag7 = usingMedicineItemType >= (int)UsingMedicineItemType.PoisonTypeHot && usingMedicineItemType <= (int)UsingMedicineItemType.PoisonTypeIllusory;
								if (flag7)
								{
									int poisonType = usingMedicineItemType - (int)UsingMedicineItemType.PoisonTypeHot;
									result = Injury.<CheckMedicineItemIsLocked>g__CheckIsLockedForDetoxPoison|60_0((sbyte)poisonType, out tipContent, ref CS$<>8__locals1);
								}
								else
								{
									result = Injury.<CheckMedicineItemIsLocked>g__CheckIsLockedForDetoxPoison|60_0(CS$<>8__locals1.medicineItem.PoisonType, out tipContent, ref CS$<>8__locals1);
								}
							}
						}
						else
						{
							bool flag8 = CS$<>8__locals1.medicineItem.EffectType == EMedicineEffectType.ApplyPoison;
							if (flag8)
							{
								bool flag9 = usingMedicineItemType <= (int)UsingMedicineItemType.Invalid;
								if (flag9)
								{
									result = false;
								}
								else
								{
									bool flag10 = usingMedicineItemType < (int)UsingMedicineItemType.PoisonTypeHot || usingMedicineItemType > (int)UsingMedicineItemType.PoisonTypeIllusory;
									if (flag10)
									{
										tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
										result = true;
									}
									else
									{
										int poisonType2 = usingMedicineItemType - (int)UsingMedicineItemType.PoisonTypeHot;
										bool flag11 = (int)CS$<>8__locals1.medicineItem.PoisonType != poisonType2;
										if (flag11)
										{
											tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
											result = true;
										}
										else
										{
											result = false;
										}
									}
								}
							}
							else
							{
								bool flag12 = CS$<>8__locals1.medicineItem.EffectType == EMedicineEffectType.RecoverHealth;
								if (flag12)
								{
									bool flag13 = usingMedicineItemType > (int)UsingMedicineItemType.Invalid && usingMedicineItemType != (int)UsingMedicineItemType.Health;
									if (flag13)
									{
										tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
										result = true;
									}
									else
									{
										bool flag14 = CS$<>8__locals1.medicineItem.Duration == 0;
										if (flag14)
										{
											bool flag15 = CS$<>8__locals1.data.LeftMaxHealth <= CS$<>8__locals1.data.Health && !CS$<>8__locals1.isCombat;
											if (flag15)
											{
												tipContent = LanguageKey.LK_UsingMedicine_Tip_NoNeed.Tr();
												return true;
											}
										}
										result = false;
									}
								}
								else
								{
									bool flag16 = CS$<>8__locals1.medicineItem.EffectType == EMedicineEffectType.ChangeDisorderOfQi;
									if (flag16)
									{
										bool flag17 = usingMedicineItemType > (int)UsingMedicineItemType.Invalid && usingMedicineItemType != (int)UsingMedicineItemType.DisorderOfQi;
										if (flag17)
										{
											tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
											result = true;
										}
										else
										{
											bool flag18 = CS$<>8__locals1.medicineItem.Duration == 0;
											if (flag18)
											{
												bool flag19 = CS$<>8__locals1.data.DisorderOfQi <= 0 && !CS$<>8__locals1.isCombat;
												if (flag19)
												{
													tipContent = LanguageKey.LK_UsingMedicine_Tip_NoNeed.Tr();
													return true;
												}
											}
											result = false;
										}
									}
									else
									{
										bool flag20 = usingMedicineItemType > (int)UsingMedicineItemType.Invalid;
										if (flag20)
										{
											tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
											result = true;
										}
										else
										{
											result = false;
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600B2BC RID: 45756 RVA: 0x00515AC0 File Offset: 0x00513CC0
		public static bool CheckEatItemIsLocked(CharacterInjuryDisplayData data, ITradeableContent itemData, int usingMedicineItemType, out string tipContent)
		{
			tipContent = string.Empty;
			bool isLocked = Injury.CheckMedicineItemIsLocked(data, itemData, usingMedicineItemType, out tipContent);
			bool flag = isLocked;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				ValueTuple<int, string> valueTuple = Injury.CheckEatSlot(data, itemData.Key, itemData.Amount);
				int count = valueTuple.Item1;
				string reason = valueTuple.Item2;
				bool canEatMore = count > 0;
				bool flag2 = !canEatMore;
				if (flag2)
				{
					tipContent = reason;
					result = true;
				}
				else
				{
					bool flag3 = itemData.RealKey.ItemType != 8 && usingMedicineItemType > (int)UsingMedicineItemType.Invalid;
					if (flag3)
					{
						tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Type_Different);
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x0600B2BD RID: 45757 RVA: 0x00515B5C File Offset: 0x00513D5C
		[return: TupleElementNames(new string[]
		{
			"Count",
			"Tip"
		})]
		public unsafe static ValueTuple<int, string> CheckEatSlot(CharacterInjuryDisplayData data, ItemKey key, int amount = 2147483647)
		{
			int limitCount = amount;
			string limitTip = string.Empty;
			bool flag = key.ItemType == 12 && ItemTemplateHelper.GetGroupId(key.ItemType, key.TemplateId) == 375;
			ValueTuple<int, string> result;
			if (flag)
			{
				result = new ValueTuple<int, string>(int.MaxValue, string.Empty);
			}
			else
			{
				bool flag2 = key.ItemType == 12 && key.TemplateId != 265;
				if (flag2)
				{
					MiscItem miscItem = Misc.Instance[key.TemplateId];
					limitCount = Mathf.CeilToInt((float)(data.MaxNeili - data.CurrNeili) / (float)miscItem.Neili);
					limitTip = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_NeiliMax);
					result = new ValueTuple<int, string>(limitCount, limitTip);
				}
				else
				{
					bool flag3 = key.ItemType == 12 && key.TemplateId == 265;
					if (flag3)
					{
						limitCount = ((amount >= ItemTemplateHelper.GetTianJieFuLuCountUnit()) ? ItemTemplateHelper.GetTianJieFuLuCountUnit() : 0);
						limitTip = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_CountLess);
					}
					bool flag4 = key.ItemType == 8;
					if (flag4)
					{
						MedicineItem medicineItem = Medicine.Instance.GetItem(key.TemplateId);
						bool flag5 = medicineItem.RequiredMainAttributeType >= 0;
						if (flag5)
						{
							short cur = *data.CurMainAttributes[(int)medicineItem.RequiredMainAttributeType];
							sbyte need = Math.Max(medicineItem.RequiredMainAttributeValue, 1);
							int attrLimit = Math.Max(0, (int)(cur / (short)need));
							bool flag6 = attrLimit < limitCount;
							if (flag6)
							{
								ECharacterPropertyDisplayType type = ECharacterPropertyDisplayType.Strength + (int)medicineItem.RequiredMainAttributeType;
								CharacterPropertyDisplayItem characterPropertyDisplayItem = CharacterPropertyDisplay.Instance[type.ToInt()];
								limitCount = attrLimit;
								limitTip = LanguageKey.LK_UsingMedicine_Tip_Attribute_Not_Enough.TrFormat(((characterPropertyDisplayItem != null) ? characterPropertyDisplayItem.Name : null) ?? "{unknown}");
								bool flag7 = characterPropertyDisplayItem == null;
								if (flag7)
								{
									Debug.LogError(string.Format("medicineItem ({0}, {1}) requires property {2}, trying to get type {3}'s name, but failed.", new object[]
									{
										medicineItem.Name,
										key.TemplateId,
										medicineItem.RequiredMainAttributeType,
										type
									}));
								}
								return new ValueTuple<int, string>(limitCount, limitTip);
							}
						}
						bool flag8 = EatingItems.IsWugKing(key);
						if (flag8)
						{
							int wugKingCount = data.EatingItems.GetWugKingCount();
							int slotCount = data.AvailableEatingSlotsCount * ItemTemplateHelper.GetItemCountUnit(key.ItemType, key.TemplateId);
							limitCount = Math.Max(slotCount, wugKingCount);
							bool flag9 = limitCount == 0;
							if (flag9)
							{
								limitTip = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_SlotMax);
							}
							return new ValueTuple<int, string>(limitCount, limitTip);
						}
						bool flag10 = medicineItem.Duration == 0;
						if (flag10)
						{
							return new ValueTuple<int, string>(amount, limitTip);
						}
					}
					bool flag11 = limitCount > 0;
					if (flag11)
					{
						int slotCount2 = data.AvailableEatingSlotsCount * ItemTemplateHelper.GetItemCountUnit(key.ItemType, key.TemplateId);
						limitCount = Math.Min(slotCount2, limitCount);
						bool flag12 = limitCount == 0;
						if (flag12)
						{
							limitTip = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_SlotMax);
						}
					}
					result = new ValueTuple<int, string>(limitCount, limitTip);
				}
			}
			return result;
		}

		// Token: 0x17001435 RID: 5173
		// (get) Token: 0x0600B2BE RID: 45758 RVA: 0x00515E51 File Offset: 0x00514051
		private MedicineItem SelectedInjuryPartUsingMedicineConfig
		{
			get
			{
				return (this._selectedInjuryPartUsingMedicine == null) ? null : Medicine.Instance[this._selectedInjuryPartUsingMedicine.RealKey.TemplateId];
			}
		}

		// Token: 0x17001436 RID: 5174
		// (get) Token: 0x0600B2BF RID: 45759 RVA: 0x00515E78 File Offset: 0x00514078
		public bool IsSelectInjuryPart
		{
			get
			{
				return this._selectedInjuryPartUsingMedicine != null;
			}
		}

		// Token: 0x0600B2C0 RID: 45760 RVA: 0x00515E88 File Offset: 0x00514088
		public void EnterSelectInjuryPart(ITradeableContent itemData, Action<List<sbyte>> onConfirm, Action onCancel)
		{
			Injury.<>c__DisplayClass72_0 CS$<>8__locals1 = new Injury.<>c__DisplayClass72_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.onConfirm = onConfirm;
			bool isSelectInjuryPart = this.IsSelectInjuryPart;
			if (!isSelectInjuryPart)
			{
				this._onCancelSelectInjuryPart = onCancel;
				this._selectedInjuryPartUsingMedicine = itemData;
				this._selectedInjuryPartList.Clear();
				this.HighLight(this.injuryPartArea.HighlightRoot);
				this.RefreshInjuryPartForSelect(true, false);
				this.buttonConfirm.ClearAndAddListener(delegate
				{
					bool flag = CS$<>8__locals1.<>4__this._curCanSelectInjuryPartCount > 0 && CS$<>8__locals1.<>4__this._selectedInjuryPartList.Count < (int)CS$<>8__locals1.<>4__this.SelectedInjuryPartUsingMedicineConfig.InjuryRecoveryTimes;
					if (flag)
					{
						string title = LanguageKey.LK_Use_Medicine_InjuryRecoveryTimes_Dialog_Title.Tr();
						string content = LanguageKey.LK_Use_Medicine_InjuryRecoveryTimes_Dialog_Content.Tr();
						CommonUtils.ShowConfirmDialog(title, content, new Action(base.<EnterSelectInjuryPart>g__Confirm|1), null, EDialogType.None);
					}
					else
					{
						base.<EnterSelectInjuryPart>g__Confirm|1();
					}
				});
				this.buttonCancel.ClearAndAddListener(new Action(CS$<>8__locals1.<EnterSelectInjuryPart>g__Cancel|2));
			}
		}

		// Token: 0x0600B2C1 RID: 45761 RVA: 0x00515F20 File Offset: 0x00514120
		private void RefreshInjuryPartForSelect(bool isEnter, bool isClick)
		{
			this.buttonConfirm.interactable = (this._selectedInjuryPartList.Count > 0);
			MedicineItem config = Medicine.Instance[this._selectedInjuryPartUsingMedicine.RealKey.TemplateId];
			bool isInner = config.EffectType == EMedicineEffectType.RecoverInnerInjury;
			this._curCanSelectInjuryPartCount = 0;
			if (isEnter)
			{
				this._totalCanSelectInjuryPartCount = 0;
			}
			for (sbyte index = 0; index < 7; index += 1)
			{
				sbyte partType = index;
				InjuryPartItem injuryPart = this.injuryPartArea.GetInjuryPartItem(index);
				sbyte injuryValue = this._characterInjuryDisplayData.Injuries.Get(index, isInner);
				bool isSelectedUsingMedicineItemType = !isEnter && !isClick && (short)partType == this._usingMedicineItemType;
				bool isSelected = this._selectedInjuryPartList.Contains(partType);
				bool hasInjury = injuryValue > 0 || UIManager.Instance.IsElementActive(UIElement.Combat);
				bool isMeetThreshold = (short)injuryValue <= config.EffectThresholdValue;
				bool canSelect = hasInjury && isMeetThreshold && this._selectedInjuryPartList.Count < (int)config.InjuryRecoveryTimes;
				bool flag = isEnter && canSelect;
				if (flag)
				{
					this._totalCanSelectInjuryPartCount++;
				}
				bool flag2 = canSelect && !isSelected;
				if (flag2)
				{
					this._curCanSelectInjuryPartCount++;
				}
				injuryPart.Selected.SetActive(isSelected || isSelectedUsingMedicineItemType);
				TextMeshProUGUI valueLabel = isInner ? injuryPart.TextInnerValue : injuryPart.TextOuterValue;
				string originValueColor = "pinkyellow";
				bool flag3 = !isSelected;
				if (flag3)
				{
					valueLabel.text = injuryValue.ToString().SetColor(originValueColor);
				}
				PointerTrigger pointerTrigger = injuryPart.PointerTrigger;
				pointerTrigger.IgnoreOnDisableTrigger = isSelected;
				pointerTrigger.enabled = (canSelect && !isSelected);
				pointerTrigger.EnterEvent.RemoveAllListeners();
				pointerTrigger.ExitEvent.RemoveAllListeners();
				bool enabled = pointerTrigger.enabled;
				if (enabled)
				{
					injuryPart.RefreshHover(config.EffectValue > 0);
					pointerTrigger.EnterEvent.AddListener(delegate()
					{
						int curValue = Mathf.Max(0, (int)((short)injuryValue - config.EffectValue));
						valueLabel.text = curValue.ToString().SetColor("brightblue");
						injuryPart.Hover.SetActive(curValue != (int)injuryValue);
					});
					pointerTrigger.ExitEvent.AddListener(delegate()
					{
						valueLabel.text = injuryValue.ToString().SetColor(originValueColor);
						injuryPart.Hover.SetActive(false);
					});
				}
				CButton button = injuryPart.Button;
				button.interactable = (canSelect | isSelected);
				injuryPart.HSVStyleRoot.SetInteractable(button.interactable);
				button.onClick.RemoveAllListeners();
				bool interactable = button.interactable;
				if (interactable)
				{
					button.onClick.AddListener(delegate()
					{
						bool isSelected = isSelected;
						if (isSelected)
						{
							this._selectedInjuryPartList.Remove(partType);
						}
						else
						{
							this._selectedInjuryPartList.Add(partType);
						}
						this.RefreshInjuryPartForSelect(false, true);
					});
				}
				TooltipInvoker partTip = injuryPart.BackTip;
				bool flag4 = partTip;
				if (flag4)
				{
					partTip.enabled = false;
				}
				TooltipInvoker selectTip = injuryPart.ButtonTip;
				selectTip.enabled = (!hasInjury || !isMeetThreshold);
				string[] presetParam = selectTip.PresetParam;
				bool flag5 = presetParam == null || presetParam.Length != 1;
				if (flag5)
				{
					selectTip.PresetParam = new string[1];
				}
				bool flag6 = !hasInjury;
				if (flag6)
				{
					selectTip.PresetParam[0] = LanguageKey.LK_UsingMedicine_Tip_NoNeed.Tr().ColorReplace() + LanguageKey.LK_Ignore.Tr().SetColor("brightred");
				}
				else
				{
					bool flag7 = !isMeetThreshold;
					if (flag7)
					{
						selectTip.PresetParam[0] = LanguageKey.LK_UsingMedicine_Tip_Value_Not_Enough.Tr().ColorReplace() + LanguageKey.LK_Ignore.Tr().SetColor("brightred");
					}
				}
			}
			this.ShowSelectInjuryPartCursorTip();
		}

		// Token: 0x0600B2C2 RID: 45762 RVA: 0x00516364 File Offset: 0x00514564
		public void ExitSelectInjuryPart()
		{
			bool flag = !this.IsSelectInjuryPart;
			if (!flag)
			{
				Action onCancelSelectInjuryPart = this._onCancelSelectInjuryPart;
				if (onCancelSelectInjuryPart != null)
				{
					onCancelSelectInjuryPart();
				}
				this._onCancelSelectInjuryPart = null;
				this.RefreshInjuryPartForSelect(false, false);
				this._selectedInjuryPartUsingMedicine = null;
				this.CancelHighLight();
				this.injuryPartArea.Set(this._characterInjuryDisplayData);
				this.RefreshInjuryAndPoison(this._canUseMedicineItem);
			}
		}

		// Token: 0x0600B2C3 RID: 45763 RVA: 0x005163D0 File Offset: 0x005145D0
		private void ShowSelectInjuryPartCursorTip()
		{
			int cur = this._selectedInjuryPartList.Count;
			int max = Math.Min(this._totalCanSelectInjuryPartCount, (int)this.SelectedInjuryPartUsingMedicineConfig.InjuryRecoveryTimes);
			string curText = cur.ToString().SetColor((cur < max) ? "brightred" : "brightblue");
			string maxText = max.ToString();
			this.textSelectPart.text = LanguageKey.LK_Use_Medicine_SelectInjuryPartTip.TrFormat(curText, maxText);
		}

		// Token: 0x0600B2C4 RID: 45764 RVA: 0x00516440 File Offset: 0x00514640
		private void HighLight(GameObject target)
		{
			bool flag = null == target;
			if (!flag)
			{
				this._focusingTuple.Item1 = target;
				this._focusingTuple.Item2 = target.transform.parent;
				this._focusingTuple.Item3 = target.transform.GetSiblingIndex();
				this.focusMask.SetActive(true);
				target.transform.SetParent(this.focusMask.transform, true);
				target.transform.localScale = Vector3.one;
			}
		}

		// Token: 0x0600B2C5 RID: 45765 RVA: 0x005164CC File Offset: 0x005146CC
		private void CancelHighLight()
		{
			bool flag = null == this._focusingTuple.Item1;
			if (!flag)
			{
				this._focusingTuple.Item1.transform.SetParent(this._focusingTuple.Item2, true);
				this._focusingTuple.Item1.transform.SetSiblingIndex(this._focusingTuple.Item3);
				this._focusingTuple.Item1 = null;
				this.focusMask.SetActive(false);
			}
		}

		// Token: 0x0600B2CA RID: 45770 RVA: 0x005165B0 File Offset: 0x005147B0
		[CompilerGenerated]
		internal unsafe static bool <CheckMedicineItemIsLocked>g__CheckIsLockedForDetoxPoison|60_0(sbyte poisonType, out string tipContent, ref Injury.<>c__DisplayClass60_0 A_2)
		{
			tipContent = string.Empty;
			int poisonNow = *A_2.data.Poisons[(int)poisonType];
			bool flag = A_2.medicineItem.Duration == 0;
			if (flag)
			{
				bool immuneFlag = A_2.data.IsImmune[(int)poisonType];
				bool need = poisonNow > 0 | A_2.isCombat;
				bool flag2 = !need || immuneFlag;
				if (flag2)
				{
					tipContent = LanguageKey.LK_UsingMedicine_Tip_NoNeed.Tr();
					return true;
				}
			}
			sbyte poisonLevel = PoisonsAndLevels.CalcPoisonedLevel(poisonNow);
			bool flag3 = A_2.medicineItem.EffectThresholdValue < (short)poisonLevel && A_2.medicineItem.Duration == 0;
			bool result;
			if (flag3)
			{
				tipContent = LocalStringManager.Get(LanguageKey.LK_UsingMedicine_Tip_Value_Not_Enough);
				result = true;
			}
			else
			{
				bool flag4 = A_2.medicineItem.PoisonType != poisonType;
				if (flag4)
				{
					tipContent = LanguageKey.LK_UsingMedicine_Tip_Type_Different.Tr();
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x04008A9A RID: 35482
		[SerializeField]
		private CButton buttonUseMedicine;

		// Token: 0x04008A9B RID: 35483
		[SerializeField]
		private TextMeshProUGUI textUseMedicine;

		// Token: 0x04008A9C RID: 35484
		[SerializeField]
		private CButton buttonHeal;

		// Token: 0x04008A9D RID: 35485
		[SerializeField]
		private TextMeshProUGUI textHeal;

		// Token: 0x04008A9E RID: 35486
		[SerializeField]
		private CButton buttonConfirm;

		// Token: 0x04008A9F RID: 35487
		[SerializeField]
		private CButton buttonCancel;

		// Token: 0x04008AA0 RID: 35488
		[SerializeField]
		private CButton buttonEatArea;

		// Token: 0x04008AA1 RID: 35489
		[SerializeField]
		private TooltipInvoker buttonUseMedicineTip;

		// Token: 0x04008AA2 RID: 35490
		[SerializeField]
		private TooltipInvoker buttonHealTip;

		// Token: 0x04008AA3 RID: 35491
		[SerializeField]
		private InjuryPartArea injuryPartArea;

		// Token: 0x04008AA4 RID: 35492
		[SerializeField]
		private InjuryQiArea injuryQiArea;

		// Token: 0x04008AA5 RID: 35493
		[SerializeField]
		private InjuryHealthArea injuryHealthArea;

		// Token: 0x04008AA6 RID: 35494
		[SerializeField]
		private InjuryPoisonArea injuryPoisonArea;

		// Token: 0x04008AA7 RID: 35495
		[SerializeField]
		private InjuryEatArea injuryEatArea;

		// Token: 0x04008AA8 RID: 35496
		[SerializeField]
		private Attribute attribute;

		// Token: 0x04008AA9 RID: 35497
		[SerializeField]
		private GameObject focusMask;

		// Token: 0x04008AAA RID: 35498
		[SerializeField]
		private TextMeshProUGUI textSelectPart;

		// Token: 0x04008AAB RID: 35499
		private CharacterInjuryDisplayData _characterInjuryDisplayData;

		// Token: 0x04008AAC RID: 35500
		private bool _canUseMedicineItem;

		// Token: 0x04008AAD RID: 35501
		private bool _showingInfectNotice;

		// Token: 0x04008AAE RID: 35502
		private short _usingMedicineItemType = UsingMedicineItemType.Invalid;

		// Token: 0x04008AAF RID: 35503
		[TupleElementNames(new string[]
		{
			"target",
			"parent",
			"sibling"
		})]
		private ValueTuple<GameObject, Transform, int> _focusingTuple;

		// Token: 0x04008AB0 RID: 35504
		private AttributeAndInjuryDynamic _attributeAndInjuryDynamic;

		// Token: 0x04008AB1 RID: 35505
		private Action _onClickEatArea;

		// Token: 0x04008AB2 RID: 35506
		private int _curCanSelectInjuryPartCount;

		// Token: 0x04008AB3 RID: 35507
		private int _totalCanSelectInjuryPartCount;

		// Token: 0x04008AB4 RID: 35508
		private readonly List<sbyte> _selectedInjuryPartList = new List<sbyte>();

		// Token: 0x04008AB5 RID: 35509
		private ITradeableContent _selectedInjuryPartUsingMedicine;

		// Token: 0x04008AB6 RID: 35510
		private Action _onCancelSelectInjuryPart;
	}
}
