using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Views.Combat.Migrate;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000AF6 RID: 2806
	public class CombatWheel : MonoBehaviour
	{
		// Token: 0x17000F3C RID: 3900
		// (get) Token: 0x06008A08 RID: 35336 RVA: 0x003FE702 File Offset: 0x003FC902
		private RectTransform _maskTarget
		{
			get
			{
				return (this.externalMaskTarget != null) ? this.externalMaskTarget : this.wheelRoot;
			}
		}

		// Token: 0x17000F3D RID: 3901
		// (get) Token: 0x06008A09 RID: 35337 RVA: 0x003FE720 File Offset: 0x003FC920
		public bool IsOpened
		{
			get
			{
				return this._openFrameCount > 0;
			}
		}

		// Token: 0x17000F3E RID: 3902
		// (get) Token: 0x06008A0A RID: 35338 RVA: 0x003FE72B File Offset: 0x003FC92B
		// (set) Token: 0x06008A0B RID: 35339 RVA: 0x003FE733 File Offset: 0x003FC933
		public float RightClickHoldTime { get; set; } = 0f;

		// Token: 0x17000F3F RID: 3903
		// (get) Token: 0x06008A0C RID: 35340 RVA: 0x003FE73C File Offset: 0x003FC93C
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008A0D RID: 35341 RVA: 0x003FE743 File Offset: 0x003FC943
		public void Init()
		{
			this._cachedEscHandler = new Action(this.CloseWheel);
			this.BindButtons();
			this.InitFreeWeapons();
			this.ForceClose();
			this.SetupModelListeners();
			this.RefreshAllData();
		}

		// Token: 0x06008A0E RID: 35342 RVA: 0x003FE77C File Offset: 0x003FC97C
		private void SetupModelListeners()
		{
			CombatModel model = this.Model;
			bool flag = model == null;
			if (!flag)
			{
				CombatModel combatModel = model;
				combatModel.OnPreparingSkillIdChanged = (OnDataChangedEvent)Delegate.Combine(combatModel.OnPreparingSkillIdChanged, new OnDataChangedEvent(this.OnSkillStateChanged));
				CombatModel combatModel2 = model;
				combatModel2.OnPerformingSkillIdChanged = (OnDataChangedEvent)Delegate.Combine(combatModel2.OnPerformingSkillIdChanged, new OnDataChangedEvent(this.OnSkillStateChanged));
				CombatModel combatModel3 = model;
				combatModel3.OnSkillPreparePercentChanged = (OnDataChangedEvent)Delegate.Combine(combatModel3.OnSkillPreparePercentChanged, new OnDataChangedEvent(this.OnSkillStateChanged));
				CombatModel combatModel4 = model;
				combatModel4.OnWeaponsChanged = (OnDataChangedEvent)Delegate.Combine(combatModel4.OnWeaponsChanged, new OnDataChangedEvent(this.OnWeaponsChanged));
				CombatModel combatModel5 = model;
				combatModel5.OnUsingWeaponIndexChanged = (OnDataChangedEvent)Delegate.Combine(combatModel5.OnUsingWeaponIndexChanged, new OnDataChangedEvent(this.OnUsingWeaponIndexChanged));
				CombatModel combatModel6 = model;
				combatModel6.OnWeaponDurabilityChanged = (OnWeaponDataChangedEvent)Delegate.Combine(combatModel6.OnWeaponDurabilityChanged, new OnWeaponDataChangedEvent(this.OnWeaponDurabilityChanged));
				CombatModel combatModel7 = model;
				combatModel7.OnWeaponCdFrameChanged = (OnWeaponDataChangedEvent)Delegate.Combine(combatModel7.OnWeaponCdFrameChanged, new OnWeaponDataChangedEvent(this.OnWeaponCdFrameChanged));
				CombatModel combatModel8 = model;
				combatModel8.OnWeaponFixedCdLeftFrameChanged = (OnWeaponDataChangedEvent)Delegate.Combine(combatModel8.OnWeaponFixedCdLeftFrameChanged, new OnWeaponDataChangedEvent(this.OnWeaponCdFrameChanged));
				CombatModel combatModel9 = model;
				combatModel9.OnWeaponFixedCdTotalFrameChanged = (OnWeaponDataChangedEvent)Delegate.Combine(combatModel9.OnWeaponFixedCdTotalFrameChanged, new OnWeaponDataChangedEvent(this.OnWeaponCdFrameChanged));
				CombatModel combatModel10 = model;
				combatModel10.OnWeaponCanChangeToChanged = (OnWeaponDataChangedEvent)Delegate.Combine(combatModel10.OnWeaponCanChangeToChanged, new OnWeaponDataChangedEvent(this.OnWeaponCanChangeToChanged));
				CombatModel combatModel11 = model;
				combatModel11.OnCanChangeTrickChanged = (OnDataChangedEvent)Delegate.Combine(combatModel11.OnCanChangeTrickChanged, new OnDataChangedEvent(delegate(bool isAlly)
				{
					if (isAlly)
					{
						this.UpdateChangeTrickButton();
					}
				}));
				CombatModel combatModel12 = model;
				combatModel12.OnChangeTrickCountChanged = (OnDataChangedEvent)Delegate.Combine(combatModel12.OnChangeTrickCountChanged, new OnDataChangedEvent(delegate(bool isAlly)
				{
					if (isAlly)
					{
						this.UpdateChangeTrickButton();
					}
				}));
				CombatModel combatModel13 = model;
				combatModel13.OnChangingTrickChanged = (OnDataChangedEvent)Delegate.Combine(combatModel13.OnChangingTrickChanged, new OnDataChangedEvent(delegate(bool isAlly)
				{
					if (isAlly)
					{
						this.UpdateChangeTrickButton();
					}
				}));
				CombatModel combatModel14 = model;
				combatModel14.OnOtherActionCanUseChanged = (OnDataChangedEvent)Delegate.Combine(combatModel14.OnOtherActionCanUseChanged, new OnDataChangedEvent(delegate(bool isAlly)
				{
					if (isAlly)
					{
						this.SyncOtherActionButtons();
					}
				}));
			}
		}

		// Token: 0x06008A0F RID: 35343 RVA: 0x003FE97C File Offset: 0x003FCB7C
		private void OnDestroy()
		{
			CombatModel model = this.Model;
			bool flag = model != null;
			if (flag)
			{
				CombatModel combatModel = model;
				combatModel.OnPreparingSkillIdChanged = (OnDataChangedEvent)Delegate.Remove(combatModel.OnPreparingSkillIdChanged, new OnDataChangedEvent(this.OnSkillStateChanged));
				CombatModel combatModel2 = model;
				combatModel2.OnPerformingSkillIdChanged = (OnDataChangedEvent)Delegate.Remove(combatModel2.OnPerformingSkillIdChanged, new OnDataChangedEvent(this.OnSkillStateChanged));
				CombatModel combatModel3 = model;
				combatModel3.OnSkillPreparePercentChanged = (OnDataChangedEvent)Delegate.Remove(combatModel3.OnSkillPreparePercentChanged, new OnDataChangedEvent(this.OnSkillStateChanged));
				CombatModel combatModel4 = model;
				combatModel4.OnWeaponsChanged = (OnDataChangedEvent)Delegate.Remove(combatModel4.OnWeaponsChanged, new OnDataChangedEvent(this.OnWeaponsChanged));
				CombatModel combatModel5 = model;
				combatModel5.OnUsingWeaponIndexChanged = (OnDataChangedEvent)Delegate.Remove(combatModel5.OnUsingWeaponIndexChanged, new OnDataChangedEvent(this.OnUsingWeaponIndexChanged));
				CombatModel combatModel6 = model;
				combatModel6.OnWeaponDurabilityChanged = (OnWeaponDataChangedEvent)Delegate.Remove(combatModel6.OnWeaponDurabilityChanged, new OnWeaponDataChangedEvent(this.OnWeaponDurabilityChanged));
				CombatModel combatModel7 = model;
				combatModel7.OnWeaponCdFrameChanged = (OnWeaponDataChangedEvent)Delegate.Remove(combatModel7.OnWeaponCdFrameChanged, new OnWeaponDataChangedEvent(this.OnWeaponCdFrameChanged));
				CombatModel combatModel8 = model;
				combatModel8.OnWeaponFixedCdLeftFrameChanged = (OnWeaponDataChangedEvent)Delegate.Remove(combatModel8.OnWeaponFixedCdLeftFrameChanged, new OnWeaponDataChangedEvent(this.OnWeaponCdFrameChanged));
				CombatModel combatModel9 = model;
				combatModel9.OnWeaponFixedCdTotalFrameChanged = (OnWeaponDataChangedEvent)Delegate.Remove(combatModel9.OnWeaponFixedCdTotalFrameChanged, new OnWeaponDataChangedEvent(this.OnWeaponCdFrameChanged));
				CombatModel combatModel10 = model;
				combatModel10.OnWeaponCanChangeToChanged = (OnWeaponDataChangedEvent)Delegate.Remove(combatModel10.OnWeaponCanChangeToChanged, new OnWeaponDataChangedEvent(this.OnWeaponCanChangeToChanged));
			}
		}

		// Token: 0x06008A10 RID: 35344 RVA: 0x003FEAF4 File Offset: 0x003FCCF4
		public void RefreshAllData()
		{
			CombatModel model = this.Model;
			CombatSubProcessorCharacter processor;
			bool flag = model != null && model.ProcessorCharacters.TryGetValue(model.SelfCharId, out processor);
			if (flag)
			{
				ItemKey[] weaponList = processor.Weapons;
				for (int i = 0; i < weaponList.Length; i++)
				{
					this.SyncWeaponBasic(i, weaponList[i], model.SelfCharId);
					this.UpdateWheelWeaponDurability(i, weaponList[i]);
					this.UpdateWheelWeaponCdImage(i, weaponList[i]);
				}
				this.SyncUsingWeaponIndex(processor.UsingWeaponIndex);
				this.UpdateChangeTrickButton();
				this.SyncOtherActionButtons();
			}
		}

		// Token: 0x06008A11 RID: 35345 RVA: 0x003FEBA0 File Offset: 0x003FCDA0
		private void InitFreeWeapons()
		{
			bool flag = this.weaponHolder == null;
			if (!flag)
			{
				short[] freeWeapons = new short[]
				{
					0,
					1,
					2,
					884
				};
				sbyte i = 3;
				while ((int)i < this.weaponHolder.childCount)
				{
					CombatWeaponPrefab wheelWeapon = this.weaponHolder.GetChild((int)i).GetComponent<CombatWeaponPrefab>();
					bool flag2 = wheelWeapon != null;
					if (flag2)
					{
						WeaponItem weaponConfig = Weapon.Instance[freeWeapons[(int)(i - 3)]];
						wheelWeapon.icon.SetSprite(weaponConfig.Icon, false, null);
					}
					i += 1;
				}
			}
		}

		// Token: 0x06008A12 RID: 35346 RVA: 0x003FEC38 File Offset: 0x003FCE38
		public void SyncWeaponBasic(int index, ItemKey weaponKey, int charId)
		{
			bool flag = this.weaponHolder == null || index >= this.weaponHolder.childCount;
			if (!flag)
			{
				Transform child = this.weaponHolder.GetChild(index);
				child.gameObject.SetActive(weaponKey.IsValid());
				bool flag2 = weaponKey.IsValid();
				if (flag2)
				{
					bool flag3 = index < 3;
					if (flag3)
					{
						CombatWeaponPrefab weaponRefers = child.GetComponent<CombatWeaponPrefab>();
						bool flag4 = weaponRefers != null && weaponRefers.icon != null;
						if (flag4)
						{
							sbyte grade = ItemTemplateHelper.GetGrade(weaponKey.ItemType, weaponKey.TemplateId);
							weaponRefers.icon.SetSprite(ItemTemplateHelper.GetIcon(weaponKey.ItemType, weaponKey.TemplateId), false, null);
							bool flag5 = weaponRefers.iconBack != null;
							if (flag5)
							{
								weaponRefers.iconBack.color = Colors.Instance.GradeColors[(int)grade];
							}
						}
					}
					ArgumentBox wheelArgBox = EasyPool.Get<ArgumentBox>();
					wheelArgBox.Set<ItemKey>("ItemKey", weaponKey);
					wheelArgBox.Set("CharId", charId);
					wheelArgBox.Set("GetNewItemDisplayData", true);
					child.GetComponent<TooltipInvoker>().RuntimeParam = wheelArgBox;
				}
			}
		}

		// Token: 0x06008A13 RID: 35347 RVA: 0x003FED78 File Offset: 0x003FCF78
		public void SyncUsingWeaponIndex(int usingWeaponIndex)
		{
			bool flag = this.weaponHolder == null;
			if (!flag)
			{
				for (int i = 0; i < this.weaponHolder.childCount; i++)
				{
					CombatWeaponPrefab weaponRefers = this.weaponHolder.GetChild(i).GetComponent<CombatWeaponPrefab>();
					bool flag2 = weaponRefers != null;
					if (flag2)
					{
						weaponRefers.usingGo.SetActive(usingWeaponIndex == i);
						weaponRefers.outAttackRange.SetActive(false);
					}
				}
			}
		}

		// Token: 0x06008A14 RID: 35348 RVA: 0x003FEDF4 File Offset: 0x003FCFF4
		private void OnWeaponsChanged(bool isAlly)
		{
			bool flag = !isAlly;
			if (!flag)
			{
				CombatModel model = this.Model;
				CombatSubProcessorCharacter processor;
				bool flag2 = model.ProcessorCharacters.TryGetValue(model.SelfCharId, out processor);
				if (flag2)
				{
					ItemKey[] weaponList = processor.Weapons;
					for (int i = 0; i < weaponList.Length; i++)
					{
						this.SyncWeaponBasic(i, weaponList[i], model.SelfCharId);
					}
				}
			}
		}

		// Token: 0x06008A15 RID: 35349 RVA: 0x003FEE68 File Offset: 0x003FD068
		private void OnUsingWeaponIndexChanged(bool isAlly)
		{
			bool flag = !isAlly;
			if (!flag)
			{
				CombatModel model = this.Model;
				CombatSubProcessorCharacter processor;
				bool flag2 = model.ProcessorCharacters.TryGetValue(model.SelfCharId, out processor);
				if (flag2)
				{
					this.SyncUsingWeaponIndex(processor.UsingWeaponIndex);
				}
			}
		}

		// Token: 0x06008A16 RID: 35350 RVA: 0x003FEEB0 File Offset: 0x003FD0B0
		private void OnWeaponDurabilityChanged(ItemKey itemKey)
		{
			CombatModel model = this.Model;
			CombatSubProcessorCharacter processor;
			bool flag = model.ProcessorCharacters.TryGetValue(model.SelfCharId, out processor);
			if (flag)
			{
				for (int i = 0; i < processor.Weapons.Length; i++)
				{
					bool flag2 = processor.Weapons[i].Equals(itemKey);
					if (flag2)
					{
						this.UpdateWheelWeaponDurability(i, itemKey);
						break;
					}
				}
			}
		}

		// Token: 0x06008A17 RID: 35351 RVA: 0x003FEF20 File Offset: 0x003FD120
		private void OnWeaponCdFrameChanged(ItemKey itemKey)
		{
			CombatModel model = this.Model;
			CombatSubProcessorCharacter processor;
			bool flag = model.ProcessorCharacters.TryGetValue(model.SelfCharId, out processor);
			if (flag)
			{
				for (int i = 0; i < processor.Weapons.Length; i++)
				{
					bool flag2 = processor.Weapons[i].Equals(itemKey);
					if (flag2)
					{
						this.UpdateWheelWeaponCdImage(i, itemKey);
						break;
					}
				}
			}
		}

		// Token: 0x06008A18 RID: 35352 RVA: 0x003FEF90 File Offset: 0x003FD190
		private void OnWeaponCanChangeToChanged(ItemKey itemKey)
		{
			CombatModel model = this.Model;
			CombatSubProcessorCharacter processor;
			bool flag = model.ProcessorCharacters.TryGetValue(model.SelfCharId, out processor);
			if (flag)
			{
				for (int i = 0; i < processor.Weapons.Length; i++)
				{
					bool flag2 = processor.Weapons[i].Equals(itemKey);
					if (flag2)
					{
						CombatSubProcessorWeapon weaponProcessor;
						bool flag3 = model.ProcessorWeapons.TryGetValue(itemKey, out weaponProcessor);
						if (flag3)
						{
							this.SyncWeaponCanChangeTo(i, weaponProcessor.CanChangeTo);
						}
						break;
					}
				}
			}
		}

		// Token: 0x06008A19 RID: 35353 RVA: 0x003FF01C File Offset: 0x003FD21C
		public void SyncWeaponCanChangeTo(int index, bool canChangeTo)
		{
			bool flag = this.weaponHolder == null || index < 0 || index >= this.weaponHolder.childCount;
			if (!flag)
			{
				CombatWeaponPrefab wheelWeaponRefers = this.weaponHolder.GetChild(index).GetComponent<CombatWeaponPrefab>();
				bool flag2 = wheelWeaponRefers != null;
				if (flag2)
				{
					CButton btn = wheelWeaponRefers.GetComponent<CButton>();
					bool flag3 = btn != null;
					if (flag3)
					{
						btn.interactable = canChangeTo;
					}
				}
			}
		}

		// Token: 0x06008A1A RID: 35354 RVA: 0x003FF091 File Offset: 0x003FD291
		public int GetWeaponSlotCount()
		{
			return (this.weaponHolder != null) ? this.weaponHolder.childCount : 0;
		}

		// Token: 0x06008A1B RID: 35355 RVA: 0x003FF0B0 File Offset: 0x003FD2B0
		public CombatWeaponPrefab GetWeaponPrefab(int index)
		{
			bool flag = this.weaponHolder == null || index < 0 || index >= this.weaponHolder.childCount;
			CombatWeaponPrefab result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = this.weaponHolder.GetChild(index).GetComponent<CombatWeaponPrefab>();
			}
			return result;
		}

		// Token: 0x06008A1C RID: 35356 RVA: 0x003FF104 File Offset: 0x003FD304
		public void UpdateWheelWeaponAttackRange(int weaponIndex, bool outRange)
		{
			bool flag = weaponIndex < 0 || weaponIndex >= this.GetWeaponSlotCount();
			if (!flag)
			{
				CombatWeaponPrefab wheelWeapon = this.GetWeaponPrefab(weaponIndex);
				bool flag2 = wheelWeapon != null && wheelWeapon.outAttackRange.activeSelf != outRange;
				if (flag2)
				{
					wheelWeapon.outAttackRange.SetActive(outRange);
				}
			}
		}

		// Token: 0x06008A1D RID: 35357 RVA: 0x003FF161 File Offset: 0x003FD361
		public void UpdateWheelWeaponSlot(int index, ItemKey itemKey, int charId)
		{
			this.SyncWeaponBasic(index, itemKey, charId);
		}

		// Token: 0x06008A1E RID: 35358 RVA: 0x003FF170 File Offset: 0x003FD370
		public void UpdateWheelWeaponDurability(int index, ItemKey itemKey)
		{
			bool flag = index < 0 || index >= this.GetWeaponSlotCount();
			if (!flag)
			{
				CombatWeaponPrefab weaponRefers = this.GetWeaponPrefab(index);
				bool flag2 = weaponRefers == null;
				if (!flag2)
				{
					CombatSubProcessorWeapon weaponProcessor = this.Model.ProcessorWeapons.GetValueOrDefault(itemKey);
					bool flag3 = weaponProcessor == null;
					if (!flag3)
					{
						short durability = weaponProcessor.Durability;
						short maxDurability = weaponProcessor.MaxDurability;
						weaponRefers.UserInt = (int)durability;
						bool flag4 = durability <= 0;
						if (flag4)
						{
							weaponRefers.highLight.gameObject.SetActive(false);
						}
						bool flag5 = weaponRefers.currDurability != null && weaponRefers.maxDurability != null;
						if (flag5)
						{
							weaponRefers.currDurability.text = ((durability > maxDurability / 2) ? durability.ToString() : durability.ToString().SetColor("brightred"));
							weaponRefers.maxDurability.text = string.Format("/{0}", maxDurability);
						}
					}
				}
			}
		}

		// Token: 0x06008A1F RID: 35359 RVA: 0x003FF278 File Offset: 0x003FD478
		public void UpdateWheelWeaponCdImage(int index, ItemKey itemKey)
		{
			bool flag = index < 0 || index >= this.GetWeaponSlotCount();
			if (!flag)
			{
				CombatWeaponPrefab weaponRefers = this.GetWeaponPrefab(index);
				bool flag2 = weaponRefers == null;
				if (!flag2)
				{
					CombatSubProcessorWeapon weaponProcessor = this.Model.ProcessorWeapons.GetValueOrDefault(itemKey);
					bool flag3 = weaponProcessor == null;
					if (!flag3)
					{
						bool noDurability = index < 3 && weaponProcessor.Durability <= 0;
						short cdFrame = weaponProcessor.CdFrame;
						int fixedLeftCdFrame = (int)((weaponProcessor.FixedCdLeftFrame > 0) ? weaponProcessor.FixedCdLeftFrame : 0);
						int fixedTotalCdFrame = (int)((weaponProcessor.FixedCdTotalFrame > 0) ? weaponProcessor.FixedCdTotalFrame : 0);
						bool isLock = fixedLeftCdFrame > 0 || noDurability;
						bool isCd = cdFrame > 0 && (index >= 3 || !isLock);
						CImage cdProgress = weaponRefers.cdProgress;
						CImage lockProgress = weaponRefers.lockProgress;
						bool flag4 = isLock;
						if (flag4)
						{
							float fillAmount = (float)fixedLeftCdFrame / (float)fixedTotalCdFrame;
							bool flag5 = noDurability;
							if (flag5)
							{
								fillAmount = 1f;
							}
							bool flag6 = !lockProgress.gameObject.activeSelf;
							if (flag6)
							{
								lockProgress.gameObject.SetActive(true);
							}
							bool activeSelf = cdProgress.gameObject.activeSelf;
							if (activeSelf)
							{
								cdProgress.gameObject.SetActive(false);
							}
							lockProgress.fillAmount = fillAmount;
							bool flag7 = weaponRefers.countDownText != null;
							if (flag7)
							{
								string countDownText = CombatUtils.StyleCountDownText((float)fixedLeftCdFrame / 60f, noDurability);
								weaponRefers.countDownText.SetText(countDownText.SetColor("goldyellow"), true);
							}
						}
						else
						{
							bool flag8 = isCd;
							if (flag8)
							{
								bool flag9 = !cdProgress.gameObject.activeSelf;
								if (flag9)
								{
									cdProgress.gameObject.SetActive(true);
								}
								bool activeSelf2 = lockProgress.gameObject.activeSelf;
								if (activeSelf2)
								{
									lockProgress.gameObject.SetActive(false);
								}
								int cdFrameSpeed = CFormula.CalcWeaponCdFrameSpeed((int)this.Model.SelfCharacter.WeaponSwitchSpeed, weaponProcessor.Weight);
								string countDownText2 = CombatUtils.StyleCountDownText((float)cdFrame / (float)cdFrameSpeed / 60f, false);
								float fillAmount2 = Mathf.Min((float)cdFrame / 30000f, 1f);
								cdProgress.fillAmount = fillAmount2;
								bool flag10 = weaponRefers.countDownText != null;
								if (flag10)
								{
									weaponRefers.countDownText.SetText(countDownText2.SetColor("goldyellow"), true);
								}
							}
							else
							{
								cdProgress.fillAmount = 0f;
								lockProgress.fillAmount = 0f;
								bool flag11 = weaponRefers.countDownText != null;
								if (flag11)
								{
									weaponRefers.countDownText.gameObject.SetActive(false);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06008A20 RID: 35360 RVA: 0x003FF514 File Offset: 0x003FD714
		public void UpdateWheelWeaponUnlockState(int index, int unlockPrepareValue, bool canUnlock)
		{
			bool flag = index < 0 || index >= this.GetWeaponSlotCount();
			if (!flag)
			{
				CombatWeaponPrefab weaponRefers = this.GetWeaponPrefab(index);
				bool flag2 = weaponRefers == null;
				if (!flag2)
				{
					CombatWeaponUnlockHolderPrefab unlockHolder = weaponRefers.unlockHolder;
					bool flag3 = unlockHolder == null;
					if (!flag3)
					{
						unlockHolder.gameObject.SetActive(unlockPrepareValue > 0);
						bool flag4 = unlockPrepareValue > 0;
						if (flag4)
						{
							CImage progressHolder = unlockHolder.unlockProgressHolder;
							CButton unlockBtn = unlockHolder.unlockBtn;
							bool unlock = unlockPrepareValue >= GlobalConfig.Instance.UnlockAttackUnit;
							progressHolder.gameObject.SetActive(!unlock);
							unlockBtn.gameObject.SetActive(unlock);
							bool flag5 = !unlock;
							if (flag5)
							{
								CImage progress = unlockHolder.unlockProgress;
								progress.fillAmount = (float)unlockPrepareValue / (float)GlobalConfig.Instance.UnlockAttackUnit;
							}
						}
					}
				}
			}
		}

		// Token: 0x06008A21 RID: 35361 RVA: 0x003FF5FC File Offset: 0x003FD7FC
		public void UpdateAllWheelWeaponUnlockState(List<int> unlockPrepareValues, List<bool> canUnlockList)
		{
			int i = 0;
			while (i < 3 && i < unlockPrepareValues.Count && i < canUnlockList.Count)
			{
				this.UpdateWheelWeaponUnlockState(i, unlockPrepareValues[i], canUnlockList[i]);
				i++;
			}
		}

		// Token: 0x06008A22 RID: 35362 RVA: 0x003FF648 File Offset: 0x003FD848
		public void UpdateChangeTrickButton()
		{
			bool flag;
			if (!(this.centerChangeTrick == null))
			{
				CombatModel model = this.Model;
				flag = (((model != null) ? model.SelfCharacter : null) == null);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (!flag2)
			{
				CombatSubProcessorCharacter processor = this.Model.SelfCharacter;
				bool canChangeTrick = processor.CanChangeTrick;
				short changeTrickCount = processor.ChangeTrickCount;
				this.centerChangeTrick.interactable = canChangeTrick;
				this.centerChangeTrickCount.text = changeTrickCount.ToString();
				SkillEffectCollection effectCollection = processor.SkillEffectCollection;
				int num;
				CombatSkillItem combatSkillItem;
				SpecialEffectItem specialEffectItem;
				bool existTransferEffect = effectCollection != null && CombatUtils.FindTransferProportionEffectFromCollection(effectCollection, out num, out combatSkillItem, out specialEffectItem);
				CImage buttonImage = this.centerChangeTrick.GetComponent<CImage>();
				bool flag3 = buttonImage != null;
				if (flag3)
				{
					bool flag4 = existTransferEffect;
					if (flag4)
					{
						buttonImage.SetSprite("combat_bottom_yuan_6", false, null);
					}
					else
					{
						buttonImage.SetSprite(canChangeTrick ? "combat_bottom_yuan_1" : "combat_bottom_yuan_2", false, null);
					}
				}
			}
		}

		// Token: 0x06008A23 RID: 35363 RVA: 0x003FF730 File Offset: 0x003FD930
		public void OpenWheelAtPosition(Vector2 screenPosition, short affectingAgileSkillId, short affectingDefenseSkillId)
		{
			bool flag = this._openFrameCount > 0;
			if (!flag)
			{
				this._affectingAgileSkillId = affectingAgileSkillId;
				this._affectingDefenseSkillId = affectingDefenseSkillId;
				Vector2 localPoint;
				bool flag2 = !RectTransformUtility.ScreenPointToLocalPointInRectangle(this.wheelRoot.parent as RectTransform, screenPosition, UIManager.Instance.UiCamera, out localPoint);
				if (!flag2)
				{
					this.wheelRoot.anchoredPosition = localPoint;
					this.ClampWheelInsideScreen();
					this.wheelRoot.gameObject.SetActive(true);
					UIManager.Instance.MaskComponent(this._maskTarget);
					this._pendingSetEscHandler = true;
					this._openFrameCount = 1;
					this.RefreshTopAttackArea();
					this.UpdateChangeTrickButton();
					this.SyncOtherActionButtons();
					Action onOpen = this.OnOpen;
					if (onOpen != null)
					{
						onOpen();
					}
				}
			}
		}

		// Token: 0x06008A24 RID: 35364 RVA: 0x003FF7F8 File Offset: 0x003FD9F8
		private void Update()
		{
			bool flag = this._pendingSetEscHandler && this._openFrameCount > 1;
			if (flag)
			{
				this._pendingSetEscHandler = false;
				UIManager.Instance.SetEscHandler(this._cachedEscHandler);
			}
			bool flag2 = this._openFrameCount > 0;
			if (flag2)
			{
				bool flag3 = this._openFrameCount > 1 && Input.GetMouseButtonDown(1);
				if (flag3)
				{
					this.CloseWheel();
				}
				else
				{
					this._openFrameCount++;
				}
			}
		}

		// Token: 0x06008A25 RID: 35365 RVA: 0x003FF878 File Offset: 0x003FDA78
		public void ForceClose()
		{
			this._openFrameCount = 0;
			this._pendingSetEscHandler = false;
			this.wheelRoot.gameObject.SetActive(false);
			UIManager.Instance.UnMaskComponent(this._maskTarget);
			this.RefreshTopAttackArea();
			this.HideBreaker();
		}

		// Token: 0x06008A26 RID: 35366 RVA: 0x003FF8C8 File Offset: 0x003FDAC8
		private void OnDisable()
		{
			bool flag = this._openFrameCount > 0;
			if (flag)
			{
				this.CloseWheel();
			}
			else
			{
				this.ForceClose();
			}
		}

		// Token: 0x06008A27 RID: 35367 RVA: 0x003FF8F4 File Offset: 0x003FDAF4
		private void OnSkillStateChanged(bool isAlly)
		{
			bool flag = !isAlly;
			if (!flag)
			{
				CombatModel model = this.Model;
				bool flag2 = ((model != null) ? model.SelfCharacter : null) != null;
				if (flag2)
				{
					short preparingSkillId = model.SelfCharacter.PreparingSkillId;
					bool flag3 = preparingSkillId >= 0;
					if (flag3)
					{
						this.RecordRecentUsedSkill(preparingSkillId);
					}
				}
				bool flag4 = this._openFrameCount > 0;
				if (flag4)
				{
					this.RefreshTopAttackArea();
					this.SyncOtherActionButtons();
				}
			}
		}

		// Token: 0x06008A28 RID: 35368 RVA: 0x003FF96C File Offset: 0x003FDB6C
		private void BindButtons()
		{
			this.centerChangeTrick.ClearAndAddListener(delegate
			{
				Action onOpenChangeTrick = this.OnOpenChangeTrick;
				if (onOpenChangeTrick != null)
				{
					onOpenChangeTrick();
				}
				this.HideWheelForMaskTransition();
			});
			this.useItem.ClearAndAddListener(delegate
			{
				Action onToggleUseItem = this.OnToggleUseItem;
				if (onToggleUseItem != null)
				{
					onToggleUseItem();
				}
				this.CloseWheel();
			});
			this.SetupButtonHoverEffect(this.useItem);
			CombatCastSkill combatCastSkill = this.castingAttack;
			combatCastSkill.OnMainButtonClick = (Action)Delegate.Combine(combatCastSkill.OnMainButtonClick, new Action(this.CloseWheel));
			this.closeButton.ClearAndAddListener(new Action(this.CloseWheel));
			for (int i = 0; i < this.weaponHolder.childCount; i++)
			{
				int index = i;
				CombatWeaponPrefab weaponPrefab = this.weaponHolder.GetChild(i).GetComponent<CombatWeaponPrefab>();
				bool flag = weaponPrefab == null;
				if (!flag)
				{
					PointerTrigger weaponPointerTrigger = weaponPrefab.GetComponent<PointerTrigger>();
					CImage highLightImg = weaponPrefab.highLight;
					bool flag2 = weaponPointerTrigger != null && highLightImg != null;
					if (flag2)
					{
						weaponPointerTrigger.EnterEvent.AddListener(delegate()
						{
							bool flag6 = index >= 3 || weaponPrefab.UserInt > 0;
							if (flag6)
							{
								highLightImg.gameObject.SetActive(true);
							}
							bool flag7 = this.Model.SelfCharacter == null;
							if (!flag7)
							{
								bool flag8 = index != this.Model.SelfCharacter.UsingWeaponIndex;
								if (flag8)
								{
									Action<int> onUpdatePreviewRangeWeapon = this.OnUpdatePreviewRangeWeapon;
									if (onUpdatePreviewRangeWeapon != null)
									{
										onUpdatePreviewRangeWeapon(index);
									}
								}
							}
						});
						weaponPointerTrigger.ExitEvent.AddListener(delegate()
						{
							highLightImg.gameObject.SetActive(false);
							bool flag6 = this.Model.SelfCharacter == null;
							if (!flag6)
							{
								bool flag7 = index != this.Model.SelfCharacter.UsingWeaponIndex;
								if (flag7)
								{
									Action onClearPreviewRangeWeapon = this.OnClearPreviewRangeWeapon;
									if (onClearPreviewRangeWeapon != null)
									{
										onClearPreviewRangeWeapon();
									}
								}
							}
						});
					}
					CButton btn = weaponPrefab.GetComponent<CButton>();
					bool flag3 = btn != null;
					if (flag3)
					{
						btn.ClearAndAddListener(delegate
						{
							Action onClearPreviewRangeWeapon = this.OnClearPreviewRangeWeapon;
							if (onClearPreviewRangeWeapon != null)
							{
								onClearPreviewRangeWeapon();
							}
							Action<int> onChangeWeapon = this.OnChangeWeapon;
							if (onChangeWeapon != null)
							{
								onChangeWeapon(index);
							}
							this.CloseWheel();
						});
					}
					CombatWeaponUnlockHolderPrefab unlockHolder = weaponPrefab.unlockHolder;
					bool flag4 = unlockHolder != null && unlockHolder.unlockBtn != null;
					if (flag4)
					{
						unlockHolder.unlockBtn.ClearAndAddListener(delegate
						{
							Action<int> onUnlockWeapon = this.OnUnlockWeapon;
							if (onUnlockWeapon != null)
							{
								onUnlockWeapon(index);
							}
							this.CloseWheel();
						});
					}
				}
			}
			foreach (CombatWheel.OtherActionSlot slot in this.otherActionSlots)
			{
				bool flag5 = slot.button == null;
				if (!flag5)
				{
					CombatWheel.OtherActionSlot localSlot = slot;
					slot.button.ClearAndAddListener(delegate
					{
						Action<sbyte> onRequestOtherAction = this.OnRequestOtherAction;
						if (onRequestOtherAction != null)
						{
							onRequestOtherAction(localSlot.actionType);
						}
						this.CloseWheel();
					});
					this.SetupButtonHoverEffect(slot.button);
				}
			}
		}

		// Token: 0x06008A29 RID: 35369 RVA: 0x003FFBDC File Offset: 0x003FDDDC
		private void SetupButtonHoverEffect(CButton button)
		{
			bool flag = button == null;
			if (!flag)
			{
				PointerTrigger pointerTrigger = button.GetComponent<PointerTrigger>();
				bool flag2 = pointerTrigger != null;
				if (flag2)
				{
					pointerTrigger.EnterEvent.AddListener(delegate()
					{
						Transform selected = button.transform.Find("Selected");
						bool flag3 = selected != null;
						if (flag3)
						{
							selected.gameObject.SetActive(true);
						}
					});
					pointerTrigger.ExitEvent.AddListener(delegate()
					{
						Transform selected = button.transform.Find("Selected");
						bool flag3 = selected != null;
						if (flag3)
						{
							selected.gameObject.SetActive(false);
						}
					});
				}
			}
		}

		// Token: 0x06008A2A RID: 35370 RVA: 0x003FFC54 File Offset: 0x003FDE54
		public void CloseWheel()
		{
			bool flag = this._openFrameCount == 0;
			if (!flag)
			{
				this._openFrameCount = 0;
				this._pendingSetEscHandler = false;
				bool flag2 = UIManager.Instance.CheckEscHandler(this._cachedEscHandler);
				if (flag2)
				{
					UIManager.Instance.SetEscHandler(null);
				}
				this.wheelRoot.gameObject.SetActive(false);
				UIManager.Instance.UnMaskComponent(this._maskTarget);
				this.RefreshTopAttackArea();
				this.HideBreaker();
				Action onClose = this.OnClose;
				if (onClose != null)
				{
					onClose();
				}
			}
		}

		// Token: 0x06008A2B RID: 35371 RVA: 0x003FFCE4 File Offset: 0x003FDEE4
		public void HideWheelForMaskTransition()
		{
			bool flag = this._openFrameCount == 0;
			if (!flag)
			{
				bool flag2 = this.externalMaskTarget == null;
				if (flag2)
				{
					this.CloseWheel();
				}
				else
				{
					this._openFrameCount = 0;
					this._pendingSetEscHandler = false;
					bool flag3 = UIManager.Instance.CheckEscHandler(this._cachedEscHandler);
					if (flag3)
					{
						UIManager.Instance.SetEscHandler(null);
					}
					this.wheelRoot.gameObject.SetActive(false);
					this.RefreshTopAttackArea();
					this.HideBreaker();
				}
			}
		}

		// Token: 0x06008A2C RID: 35372 RVA: 0x003FFD6C File Offset: 0x003FDF6C
		private void RefreshTopAttackArea()
		{
			bool showCastingAttack = this._openFrameCount > 0 && CombatWheel.IsCastingAttackSkill();
			this.topAttackSlotsRoot.SetActive(!showCastingAttack);
			this.castingAttackRoot.SetActive(showCastingAttack);
			bool flag = showCastingAttack && this.castingAttack != null;
			if (flag)
			{
				CombatCastSkill combatCastSkill = this.castingAttack.GetComponent<CombatCastSkill>();
				bool flag2 = combatCastSkill != null;
				if (flag2)
				{
					CombatModel model = this.Model;
					CombatSubProcessorCharacter selfCharacter = (model != null) ? model.SelfCharacter : null;
					bool isAttackSkill = false;
					bool flag3 = selfCharacter != null;
					if (flag3)
					{
						short preparingSkill = selfCharacter.PreparingSkillId;
						short performingSkill = selfCharacter.PerformingSkillId;
						bool flag4 = preparingSkill >= 0;
						if (flag4)
						{
							CombatSkillItem skillConfig = CombatSkill.Instance[preparingSkill];
							isAttackSkill = (skillConfig.EquipType == 1);
						}
						else
						{
							bool flag5 = performingSkill >= 0;
							if (flag5)
							{
								CombatSkillItem skillConfig2 = CombatSkill.Instance[performingSkill];
								isAttackSkill = (skillConfig2.EquipType == 1);
							}
						}
					}
					combatCastSkill.gameObject.SetActive(isAttackSkill);
				}
			}
			bool flag6 = this._openFrameCount > 0;
			if (flag6)
			{
				this.RefreshRecentSkills();
			}
		}

		// Token: 0x06008A2D RID: 35373 RVA: 0x003FFE98 File Offset: 0x003FE098
		private void RefreshRecentSkills()
		{
			CombatWheel.<>c__DisplayClass80_0 CS$<>8__locals1 = new CombatWheel.<>c__DisplayClass80_0();
			CS$<>8__locals1.<>4__this = this;
			CombatModel model = this.Model;
			int selfCharId = model.SelfCharId;
			bool flag = !model.ProactiveSkillData.TryGetValue(selfCharId, out CS$<>8__locals1.displayDataList);
			if (!flag)
			{
				short agileToShow = (this._affectingAgileSkillId >= 0) ? this._affectingAgileSkillId : this._recentAgileSkill;
				CS$<>8__locals1.<RefreshRecentSkills>g__SetSkillSlot|0(this.recentAgileSlot, agileToShow);
				short defendToShow = (this._affectingDefenseSkillId >= 0) ? this._affectingDefenseSkillId : this._recentDefenseSkill;
				CS$<>8__locals1.<RefreshRecentSkills>g__SetSkillSlot|0(this.recentDefenseSlot, defendToShow);
				List<short> recentAttacks = this._recentAttackSkills;
				short attack = (recentAttacks.Count > 0) ? recentAttacks[0] : -1;
				short attack2 = (recentAttacks.Count > 1) ? recentAttacks[1] : -1;
				CS$<>8__locals1.<RefreshRecentSkills>g__SetSkillSlot|0(this.recentAttackSlot1, attack);
				CS$<>8__locals1.<RefreshRecentSkills>g__SetSkillSlot|0(this.recentAttackSlot2, attack2);
				this.UpdateBreaker();
			}
		}

		// Token: 0x06008A2E RID: 35374 RVA: 0x003FFF8B File Offset: 0x003FE18B
		private void UpdateBreaker()
		{
			this.UpdateBreakerForSkill(this._affectingAgileSkillId, this.breakerAgile, this.recentAgileSlot);
			this.UpdateBreakerForSkill(this._affectingDefenseSkillId, this.breakerDefense, this.recentDefenseSlot);
		}

		// Token: 0x06008A2F RID: 35375 RVA: 0x003FFFC0 File Offset: 0x003FE1C0
		private void UpdateBreakerForSkill(short affectingSkillId, GameObject breaker, CombatProactiveSkillView targetSlot)
		{
			bool flag = breaker == null;
			if (!flag)
			{
				bool flag2 = affectingSkillId < 0 || targetSlot == null || !targetSlot.gameObject.activeSelf;
				if (flag2)
				{
					breaker.SetActive(false);
				}
				else
				{
					RectTransform breakerRect = breaker.GetComponent<RectTransform>();
					breakerRect.SetParent(targetSlot.transform, false);
					breakerRect.anchoredPosition = Vector2.zero;
					breaker.SetActive(true);
					CButton btn = breaker.GetComponent<CButton>();
					bool flag3 = btn != null;
					if (flag3)
					{
						btn.ClearAndAddListener(delegate
						{
							Action<short> onBreakAffectingSkill = this.OnBreakAffectingSkill;
							if (onBreakAffectingSkill != null)
							{
								onBreakAffectingSkill(affectingSkillId);
							}
						});
					}
					PointerTrigger pointerTrigger = breaker.GetComponent<PointerTrigger>();
					bool flag4 = pointerTrigger != null;
					if (flag4)
					{
						pointerTrigger.EnterEvent.RemoveAllListeners();
						pointerTrigger.ExitEvent.RemoveAllListeners();
						pointerTrigger.EnterEvent.AddListener(delegate()
						{
							Transform transform = breaker.transform.Find("OnNormal");
							GameObject onNormal = (transform != null) ? transform.gameObject : null;
							Transform transform2 = breaker.transform.Find("OnHover");
							GameObject onHover = (transform2 != null) ? transform2.gameObject : null;
							bool flag5 = onNormal != null;
							if (flag5)
							{
								onNormal.SetActive(false);
							}
							bool flag6 = onHover != null;
							if (flag6)
							{
								onHover.SetActive(true);
							}
						});
						pointerTrigger.ExitEvent.AddListener(delegate()
						{
							Transform transform = breaker.transform.Find("OnNormal");
							GameObject onNormal = (transform != null) ? transform.gameObject : null;
							Transform transform2 = breaker.transform.Find("OnHover");
							GameObject onHover = (transform2 != null) ? transform2.gameObject : null;
							bool flag5 = onNormal != null;
							if (flag5)
							{
								onNormal.SetActive(true);
							}
							bool flag6 = onHover != null;
							if (flag6)
							{
								onHover.SetActive(false);
							}
						});
					}
				}
			}
		}

		// Token: 0x06008A30 RID: 35376 RVA: 0x00400100 File Offset: 0x003FE300
		private void HideBreaker()
		{
			bool flag = this.breakerAgile != null;
			if (flag)
			{
				this.breakerAgile.SetActive(false);
			}
			bool flag2 = this.breakerDefense != null;
			if (flag2)
			{
				this.breakerDefense.SetActive(false);
			}
		}

		// Token: 0x06008A31 RID: 35377 RVA: 0x00400148 File Offset: 0x003FE348
		private static bool IsCastingAttackSkill()
		{
			CombatModel model = SingletonObject.getInstance<CombatModel>();
			CombatSubProcessorCharacter selfCharacter = (model != null) ? model.SelfCharacter : null;
			bool flag = selfCharacter == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				short preparingSkill = selfCharacter.PreparingSkillId;
				bool flag2 = preparingSkill >= 0 && CombatSkill.Instance[preparingSkill].EquipType == 1;
				if (flag2)
				{
					result = true;
				}
				else
				{
					short performingSkill = selfCharacter.PerformingSkillId;
					result = (performingSkill >= 0 && CombatSkill.Instance[performingSkill].EquipType == 1);
				}
			}
			return result;
		}

		// Token: 0x06008A32 RID: 35378 RVA: 0x004001CC File Offset: 0x003FE3CC
		private void ClampWheelInsideScreen()
		{
			Canvas canvas = this.wheelRoot.GetComponentInParent<Canvas>();
			RectTransform canvasRect = canvas.GetComponent<RectTransform>();
			Vector3[] screenCorners = new Vector3[4];
			canvasRect.GetWorldCorners(screenCorners);
			RectTransform parentRect = this.wheelRoot.parent as RectTransform;
			Vector3 min = parentRect.InverseTransformPoint(screenCorners[0]);
			Vector3 max = parentRect.InverseTransformPoint(screenCorners[2]);
			Rect rect = this.wheelRoot.rect;
			Vector2 pivot = this.wheelRoot.pivot;
			Vector2 minOffset = new Vector2(rect.width * pivot.x, rect.height * pivot.y);
			Vector2 maxOffset = new Vector2(rect.width * (1f - pivot.x), rect.height * (1f - pivot.y));
			Vector2 anchoredPos = this.wheelRoot.anchoredPosition;
			anchoredPos.x = Mathf.Clamp(anchoredPos.x, min.x + minOffset.x, max.x - maxOffset.x);
			anchoredPos.y = Mathf.Clamp(anchoredPos.y, min.y + minOffset.y, max.y - maxOffset.y);
			this.wheelRoot.anchoredPosition = anchoredPos;
		}

		// Token: 0x06008A33 RID: 35379 RVA: 0x00400318 File Offset: 0x003FE518
		public void SyncOtherActionButtons()
		{
		}

		// Token: 0x06008A34 RID: 35380 RVA: 0x0040031C File Offset: 0x003FE51C
		public void SetOtherActionButtonVisible(sbyte actionType, bool visible)
		{
			foreach (CombatWheel.OtherActionSlot slot in this.otherActionSlots)
			{
				bool flag = slot.actionType == actionType && slot.button != null;
				if (flag)
				{
					slot.button.gameObject.SetActive(visible);
					break;
				}
			}
		}

		// Token: 0x06008A35 RID: 35381 RVA: 0x004003A0 File Offset: 0x003FE5A0
		public void SetOtherActionCount(sbyte actionType, byte countValue)
		{
			foreach (CombatWheel.OtherActionSlot slot in this.otherActionSlots)
			{
				bool flag = slot.actionType == actionType && slot.count != null;
				if (flag)
				{
					slot.count.text = countValue.ToString();
					break;
				}
			}
		}

		// Token: 0x06008A36 RID: 35382 RVA: 0x00400424 File Offset: 0x003FE624
		public void SetOtherActionTip(sbyte actionType, string tipText)
		{
			foreach (CombatWheel.OtherActionSlot slot in this.otherActionSlots)
			{
				bool flag = slot.actionType == actionType && slot.button != null;
				if (flag)
				{
					TooltipInvoker mouseTip = slot.button.GetComponent<TooltipInvoker>();
					bool flag2 = mouseTip != null;
					if (flag2)
					{
						mouseTip.PresetParam[1] = tipText;
						bool showing = mouseTip.Showing;
						if (showing)
						{
							mouseTip.Refresh(false, -1);
						}
					}
					break;
				}
			}
		}

		// Token: 0x06008A37 RID: 35383 RVA: 0x004004D0 File Offset: 0x003FE6D0
		public void SetUseItemWisdomCount(short wisdomCount, short specialWisdomCount)
		{
			bool flag = this.useItemWisdomCount == null;
			if (!flag)
			{
				string textColor = (specialWisdomCount > 0) ? "brightblue" : "pinkyellow";
				this.useItemWisdomCount.text = wisdomCount.ToString().SetColor(textColor);
			}
		}

		// Token: 0x06008A38 RID: 35384 RVA: 0x0040051C File Offset: 0x003FE71C
		public void UpdateAffectingSkills(short affectingAgileSkillId, short affectingDefenseSkillId)
		{
			this._affectingAgileSkillId = affectingAgileSkillId;
			this._affectingDefenseSkillId = affectingDefenseSkillId;
			bool flag = this._openFrameCount > 0;
			if (flag)
			{
				this.RefreshRecentSkills();
			}
		}

		// Token: 0x06008A39 RID: 35385 RVA: 0x00400550 File Offset: 0x003FE750
		public void RecordRecentUsedSkill(short skillId)
		{
			bool flag = skillId < 0;
			if (!flag)
			{
				CombatSkillItem config = CombatSkill.Instance[skillId];
				bool flag2 = config == null;
				if (!flag2)
				{
					switch (config.EquipType)
					{
					case 1:
					{
						this._recentAttackSkills.Remove(skillId);
						this._recentAttackSkills.Insert(0, skillId);
						bool flag3 = this._recentAttackSkills.Count > 2;
						if (flag3)
						{
							this._recentAttackSkills.RemoveRange(2, this._recentAttackSkills.Count - 2);
						}
						break;
					}
					case 2:
						this._recentAgileSkill = skillId;
						break;
					case 3:
						this._recentDefenseSkill = skillId;
						break;
					}
				}
			}
		}

		// Token: 0x040069DC RID: 27100
		[SerializeField]
		private RectTransform wheelRoot;

		// Token: 0x040069DD RID: 27101
		[SerializeField]
		private CButton centerChangeTrick;

		// Token: 0x040069DE RID: 27102
		[SerializeField]
		private TextMeshProUGUI centerChangeTrickCount;

		// Token: 0x040069DF RID: 27103
		[SerializeField]
		private CButton useItem;

		// Token: 0x040069E0 RID: 27104
		[SerializeField]
		private TextMeshProUGUI useItemWisdomCount;

		// Token: 0x040069E1 RID: 27105
		[SerializeField]
		private GameObject topAttackSlotsRoot;

		// Token: 0x040069E2 RID: 27106
		[SerializeField]
		private GameObject castingAttackRoot;

		// Token: 0x040069E3 RID: 27107
		[SerializeField]
		private CombatCastSkill castingAttack;

		// Token: 0x040069E4 RID: 27108
		[SerializeField]
		private RectTransform weaponHolder;

		// Token: 0x040069E5 RID: 27109
		[SerializeField]
		private List<CombatWheel.OtherActionSlot> otherActionSlots = new List<CombatWheel.OtherActionSlot>();

		// Token: 0x040069E6 RID: 27110
		[SerializeField]
		private CButton closeButton;

		// Token: 0x040069E7 RID: 27111
		[Header("Recent Skills")]
		[SerializeField]
		private CombatProactiveSkillView recentAgileSlot;

		// Token: 0x040069E8 RID: 27112
		[SerializeField]
		private CombatProactiveSkillView recentDefenseSlot;

		// Token: 0x040069E9 RID: 27113
		[SerializeField]
		private CombatProactiveSkillView recentAttackSlot1;

		// Token: 0x040069EA RID: 27114
		[SerializeField]
		private CombatProactiveSkillView recentAttackSlot2;

		// Token: 0x040069EB RID: 27115
		[Header("运转中功法中断器")]
		[SerializeField]
		private GameObject breakerAgile;

		// Token: 0x040069EC RID: 27116
		[SerializeField]
		private GameObject breakerDefense;

		// Token: 0x040069ED RID: 27117
		[NonSerialized]
		public RectTransform externalMaskTarget;

		// Token: 0x040069EE RID: 27118
		private int _openFrameCount;

		// Token: 0x040069EF RID: 27119
		private Action _cachedEscHandler;

		// Token: 0x040069F0 RID: 27120
		public Action OnOpen;

		// Token: 0x040069F1 RID: 27121
		public Action OnClose;

		// Token: 0x040069F2 RID: 27122
		public Action OnOpenChangeTrick;

		// Token: 0x040069F3 RID: 27123
		public Action OnToggleUseItem;

		// Token: 0x040069F4 RID: 27124
		public Action<int> OnChangeWeapon;

		// Token: 0x040069F5 RID: 27125
		public Action<int> OnUnlockWeapon;

		// Token: 0x040069F6 RID: 27126
		public Action<sbyte> OnRequestOtherAction;

		// Token: 0x040069F7 RID: 27127
		public Action<short> OnCastSkill;

		// Token: 0x040069F8 RID: 27128
		public Action<short> OnBreakAffectingSkill;

		// Token: 0x040069F9 RID: 27129
		public Action<int> OnUpdatePreviewRangeWeapon;

		// Token: 0x040069FA RID: 27130
		public Action OnClearPreviewRangeWeapon;

		// Token: 0x040069FC RID: 27132
		private readonly List<short> _recentAttackSkills = new List<short>();

		// Token: 0x040069FD RID: 27133
		private short _recentAgileSkill = -1;

		// Token: 0x040069FE RID: 27134
		private short _recentDefenseSkill = -1;

		// Token: 0x040069FF RID: 27135
		private short _affectingAgileSkillId = -1;

		// Token: 0x04006A00 RID: 27136
		private short _affectingDefenseSkillId = -1;

		// Token: 0x04006A01 RID: 27137
		private bool _pendingSetEscHandler;

		// Token: 0x020020BE RID: 8382
		[Serializable]
		private struct OtherActionSlot
		{
			// Token: 0x0400D208 RID: 53768
			public CButton button;

			// Token: 0x0400D209 RID: 53769
			public TextMeshProUGUI count;

			// Token: 0x0400D20A RID: 53770
			public sbyte actionType;
		}
	}
}
