using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Avatar;
using Game.Components.Combat;
using Game.Components.Item;
using Game.Views.CharacterMenu;
using GameData.Domains.Character.Display;
using GameData.Domains.Combat;
using GameData.Domains.CombatSkill;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UICommon.Character;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B37 RID: 2871
	public class ViewCombatDamageDetail : UIBase
	{
		// Token: 0x17000F98 RID: 3992
		// (get) Token: 0x06008E9F RID: 36511 RVA: 0x00425834 File Offset: 0x00423A34
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x17000F99 RID: 3993
		// (get) Token: 0x06008EA0 RID: 36512 RVA: 0x0042583B File Offset: 0x00423A3B
		private DamageCompareData Data
		{
			get
			{
				return this.Model.DamageCompareData;
			}
		}

		// Token: 0x06008EA1 RID: 36513 RVA: 0x00425848 File Offset: 0x00423A48
		public override void OnInit(ArgumentBox argsBox)
		{
			this.closeButton.ClearAndAddListener(new Action(this.OnClickClose));
		}

		// Token: 0x06008EA2 RID: 36514 RVA: 0x00425864 File Offset: 0x00423A64
		private void OnEnable()
		{
			this.Model.RaiseEvent(ECombatEvents.OnDamageDetailShow);
			this.RefreshAvatars();
			this.RefreshWeaponDurability();
			this.RefreshAttackDefense();
			this.RefreshConsummateLevels();
			this.RefreshWeaponIcons();
			this.RefreshCenterSkill();
			this.SetupDiscs();
			this.RefreshLines();
			this.RefreshFinalDisc();
			this.RefreshChengShu();
			this.RefreshTotalDamage();
		}

		// Token: 0x06008EA3 RID: 36515 RVA: 0x004258CD File Offset: 0x00423ACD
		private void OnDisable()
		{
			this.Model.RaiseEvent(ECombatEvents.OnDamageDetailHide);
			this.ClearAvatars();
		}

		// Token: 0x06008EA4 RID: 36516 RVA: 0x004258E5 File Offset: 0x00423AE5
		private void OnClickClose()
		{
			this.QuickHide();
		}

		// Token: 0x06008EA5 RID: 36517 RVA: 0x004258F0 File Offset: 0x00423AF0
		private void RefreshAvatars()
		{
			CombatSubProcessorCharacter selfProcessor = this.Model.SelfCharacter;
			CombatSubProcessorCharacter enemyProcessor = this.Model.EnemyCharacter;
			bool flag = selfProcessor == null || enemyProcessor == null;
			if (!flag)
			{
				int selfId = selfProcessor.CharacterId;
				int enemyId = enemyProcessor.CharacterId;
				if (this._leftCharacterAvatar == null)
				{
					this._leftCharacterAvatar = new CharacterAvatar(this.leftAvatar, true);
				}
				if (this._rightCharacterAvatar == null)
				{
					this._rightCharacterAvatar = new CharacterAvatar(this.rightAvatar, true);
				}
				this._leftCharacterAvatar.CharacterId = selfId;
				this._rightCharacterAvatar.CharacterId = enemyId;
				CharacterDisplayData selfDisplay;
				bool flag2 = this.Model.DisplayDataCache.TryGetValue(selfId, out selfDisplay);
				if (flag2)
				{
					this.leftCharName.text = CombatUtils.GetNameString(selfDisplay, true);
				}
				CharacterDisplayData enemyDisplay;
				bool flag3 = this.Model.DisplayDataCache.TryGetValue(enemyId, out enemyDisplay);
				if (flag3)
				{
					this.rightCharName.text = CombatUtils.GetNameString(enemyDisplay, false);
				}
				this.leftCharMenuButton.ClearAndAddListener(delegate
				{
					CombatUtils.ShowCharMenu(selfId);
				});
				this.rightCharMenuButton.ClearAndAddListener(delegate
				{
					CombatUtils.ShowCharMenu(enemyId);
				});
			}
		}

		// Token: 0x06008EA6 RID: 36518 RVA: 0x00425A34 File Offset: 0x00423C34
		private void ClearAvatars()
		{
			bool flag = this._leftCharacterAvatar != null;
			if (flag)
			{
				this._leftCharacterAvatar.CharacterId = -1;
			}
			bool flag2 = this._rightCharacterAvatar != null;
			if (flag2)
			{
				this._rightCharacterAvatar.CharacterId = -1;
			}
		}

		// Token: 0x06008EA7 RID: 36519 RVA: 0x00425A78 File Offset: 0x00423C78
		private void RefreshWeaponDurability()
		{
			SkillEquipmentSnapshot data = this.Model.SkillDamageData.EquipmentSnapshot;
			ItemKey weaponOrShoesKey = data.WeaponOrShoesKey;
			short weaponOrShoesStartDurability = data.WeaponOrShoesStartDurability;
			ItemKey weaponKey = weaponOrShoesKey;
			short snapshotDurability = weaponOrShoesStartDurability;
			bool flag = !weaponKey.IsValid();
			if (flag)
			{
				this.weaponDurabilityText.text = string.Empty;
			}
			else
			{
				CombatSubProcessorCharacter processor = (this.Data != null) ? (this.Data.IsAlly ? this.Model.SelfCharacter : this.Model.EnemyCharacter) : null;
				bool flag2 = processor == null || (processor != null && processor.UsingWeaponIndex >= 3 && weaponKey.ItemType == 0);
				if (flag2)
				{
					this.weaponDurabilityText.text = string.Empty;
				}
				else
				{
					List<ItemDisplayData> orDefault = this.Model.EquipmentDataCache.GetOrDefault(processor.CharacterId);
					short? num;
					if (orDefault == null)
					{
						num = null;
					}
					else
					{
						ItemDisplayData itemDisplayData = orDefault.FirstOrDefault((ItemDisplayData x) => x.Key == weaponKey);
						num = ((itemDisplayData != null) ? new short?(itemDisplayData.MaxDurability) : null);
					}
					short? num2 = num;
					short max = num2.GetValueOrDefault();
					short current = data.WeaponOrShoesEndDurability;
					int reduction = (int)(snapshotDurability - current);
					bool flag3 = reduction > 0;
					if (flag3)
					{
						this.weaponDurabilityText.text = string.Format("{0}{1}/{2}", snapshotDurability, string.Format("-{0}", reduction).SetColor("brightred"), max);
					}
					else
					{
						this.weaponDurabilityText.text = string.Format("{0}/{1}", snapshotDurability, max);
					}
				}
			}
		}

		// Token: 0x06008EA8 RID: 36520 RVA: 0x00425C28 File Offset: 0x00423E28
		private void RefreshAttackDefense()
		{
			bool flag = this.Data == null || this.Data.SkillId < 0;
			if (!flag)
			{
				bool isAlly = this.Data.IsAlly;
				this.leftSideTitle.text = (isAlly ? LanguageKey.LK_CombatDamageDetail_Attack_Side.Tr() : LanguageKey.LK_CombatDamageDetail_Defence_Side.Tr());
				this.rightSideTitle.text = (isAlly ? LanguageKey.LK_CombatDamageDetail_Defence_Side.Tr() : LanguageKey.LK_CombatDamageDetail_Attack_Side.Tr());
				long outerAttack = (long)Mathf.Max(this.Data.OuterAttackValue, 1);
				long outerDefend = (long)Mathf.Max(this.Data.OuterDefendValue, 1);
				long innerAttack = (long)Mathf.Max(this.Data.InnerAttackValue, 1);
				long innerDefend = (long)Mathf.Max(this.Data.InnerDefendValue, 1);
				bool flag2 = this.Data.OuterAttackValue >= 0;
				if (flag2)
				{
					bool selfHigher = outerAttack > outerDefend;
					bool enemyHigher = outerDefend > outerAttack;
					bool flag3 = isAlly;
					if (flag3)
					{
						this.selfOuterValue.Set("ui9_icon_attribute_attack_big_0", LanguageKey.LK_Penetrate_Outer.Tr(), CommonUtils.GetDisplayStringForNum(outerAttack), selfHigher);
						this.enemyOuterValue.Set("ui9_icon_attribute_defence_big_0", LanguageKey.LK_Penetrate_Resist_Outer.Tr(), CommonUtils.GetDisplayStringForNum(outerDefend), enemyHigher);
					}
					else
					{
						this.selfOuterValue.Set("ui9_icon_attribute_defence_big_0", LanguageKey.LK_Penetrate_Resist_Outer.Tr(), CommonUtils.GetDisplayStringForNum(outerDefend), enemyHigher);
						this.enemyOuterValue.Set("ui9_icon_attribute_attack_big_0", LanguageKey.LK_Penetrate_Outer.Tr(), CommonUtils.GetDisplayStringForNum(outerAttack), selfHigher);
					}
					int outerPercent = Mathf.Min((int)(outerAttack * 100L / outerDefend), 9999);
					this.outerDamageResult.Set(string.Empty, LanguageKey.LK_Combat_OuterDamage_Bonus.Tr(), string.Format("{0}%", outerPercent));
					this.outerDamageResult.SetValueColor(Colors.Instance["outterinjury"]);
				}
				bool flag4 = this.Data.InnerAttackValue >= 0;
				if (flag4)
				{
					bool selfHigher2 = innerAttack > innerDefend;
					bool enemyHigher2 = innerDefend > innerAttack;
					bool flag5 = isAlly;
					if (flag5)
					{
						this.selfInnerValue.Set("ui9_icon_attribute_attack_big_1", LanguageKey.LK_Penetrate_Inner.Tr(), CommonUtils.GetDisplayStringForNum(innerAttack), selfHigher2);
						this.enemyInnerValue.Set("ui9_icon_attribute_defence_big_1", LanguageKey.LK_Penetrate_Resist_Inner.Tr(), CommonUtils.GetDisplayStringForNum(innerDefend), enemyHigher2);
					}
					else
					{
						this.selfInnerValue.Set("ui9_icon_attribute_defence_big_1", LanguageKey.LK_Penetrate_Resist_Inner.Tr(), CommonUtils.GetDisplayStringForNum(innerDefend), enemyHigher2);
						this.enemyInnerValue.Set("ui9_icon_attribute_attack_big_1", LanguageKey.LK_Penetrate_Inner.Tr(), CommonUtils.GetDisplayStringForNum(innerAttack), selfHigher2);
					}
					int innerPercent = Mathf.Min((int)(innerAttack * 100L / innerDefend), 9999);
					this.innerDamageResult.Set(string.Empty, LanguageKey.LK_Combat_InnerDamage_Bonus.Tr(), string.Format("{0}%", innerPercent));
					this.innerDamageResult.SetValueColor(Colors.Instance["innerinjury"]);
				}
			}
		}

		// Token: 0x06008EA9 RID: 36521 RVA: 0x00425F3C File Offset: 0x0042413C
		private void RefreshCenterSkill()
		{
			bool flag = this.Data == null || this.Data.SkillId < 0;
			if (flag)
			{
				this.centerSkill.gameObject.SetActive(false);
			}
			else
			{
				int attackerCharId = this.Data.IsAlly ? this.Model.SelfCharId : this.Model.EnemyCharId;
				CombatSkillDomainMethod.AsyncCall.GetCombatSkillDisplayDataOnce(this, attackerCharId, this.Data.SkillId, delegate(int offset, RawDataPool pool)
				{
					bool flag2 = this == null;
					if (!flag2)
					{
						CombatSkillDisplayData skillData = null;
						Serializer.Deserialize(pool, offset, ref skillData);
						bool flag3 = skillData != null;
						if (flag3)
						{
							this.centerSkill.Set(skillData);
						}
						else
						{
							this.centerSkill.Set(this.Data.SkillId, 0, false, 0, null, attackerCharId, -1, false);
						}
					}
				});
			}
		}

		// Token: 0x06008EAA RID: 36522 RVA: 0x00425FD8 File Offset: 0x004241D8
		private void RefreshConsummateLevels()
		{
			bool flag = this.Model.SelfCharacter != null;
			if (flag)
			{
				this.leftConsummateLevel.Set(this.Model.SelfCharacter.ConsummateLevel);
			}
			bool flag2 = this.Model.EnemyCharacter != null;
			if (flag2)
			{
				this.rightConsummateLevel.Set(this.Model.EnemyCharacter.ConsummateLevel);
			}
		}

		// Token: 0x06008EAB RID: 36523 RVA: 0x00426044 File Offset: 0x00424244
		private void RefreshWeaponIcons()
		{
			bool flag = this.Data != null && !this.Data.IsAlly;
			if (flag)
			{
				this.LoadAndSetArmorIcon(this.leftWeaponIcon, this.Model.SelfCharacter);
				this.SetWeaponIcon(this.rightArmorIcon, this.Model.EnemyCharacter);
				this.enemyNoArmorNode.SetActive(false);
			}
			else
			{
				this.SetWeaponIcon(this.leftWeaponIcon, this.Model.SelfCharacter);
				this.LoadAndSetArmorIcon(this.rightArmorIcon, this.Model.EnemyCharacter);
			}
		}

		// Token: 0x06008EAC RID: 36524 RVA: 0x004260E4 File Offset: 0x004242E4
		private void SetWeaponIcon(ItemBack iconHolder, CombatSubProcessorCharacter processor)
		{
			SkillEquipmentSnapshot data = this.Model.SkillDamageData.EquipmentSnapshot;
			ItemKey weaponKey = data.WeaponOrShoesKey;
			bool flag = !weaponKey.IsValid();
			if (flag)
			{
				iconHolder.SetIcon(string.Empty);
				iconHolder.SetBack(-1);
			}
			else
			{
				string icon = ItemTemplateHelper.GetIcon(weaponKey.ItemType, weaponKey.TemplateId);
				sbyte grade = ItemTemplateHelper.GetGrade(weaponKey.ItemType, weaponKey.TemplateId);
				iconHolder.SetIcon(icon);
				iconHolder.SetBack(grade);
			}
		}

		// Token: 0x06008EAD RID: 36525 RVA: 0x00426168 File Offset: 0x00424368
		private void RefreshTotalDamage()
		{
			SkillDamageData data = this.Model.SkillDamageData;
			Dictionary<DefeatMarkKey, int> dictionary;
			if (data == null)
			{
				dictionary = null;
			}
			else
			{
				SkillDamageSectionData total = data.Total;
				dictionary = ((total != null) ? total.Values : null);
			}
			Dictionary<DefeatMarkKey, int> values = dictionary;
			bool flag = values == null || values.Count == 0;
			if (flag)
			{
				this.totalDamageContainer.gameObject.SetActive(false);
			}
			else
			{
				this.totalDamageContainer.gameObject.SetActive(true);
				DefeatMarkKey[] sortedKeys = values.Keys.OrderBy(new Func<DefeatMarkKey, int>(ViewCombatDamageDetail.GetTotalItemOrder)).ToArray<DefeatMarkKey>();
				CommonUtils.PrepareEnoughChildren(this.totalDamageContainer, this.totalDamageItemPrefab.gameObject, sortedKeys.Length, null);
				for (int i = 0; i < sortedKeys.Length; i++)
				{
					CombatDamageTotalItem item = this.totalDamageContainer.GetChild(i).GetComponent<CombatDamageTotalItem>();
					item.Set(sortedKeys[i], values[sortedKeys[i]]);
				}
			}
		}

		// Token: 0x06008EAE RID: 36526 RVA: 0x00426268 File Offset: 0x00424468
		private static int GetTotalItemOrder(DefeatMarkKey key)
		{
			EMarkType type = key.Type;
			if (!true)
			{
			}
			int result;
			switch (type)
			{
			case EMarkType.Outer:
				result = key.SubType * 2;
				goto IL_67;
			case EMarkType.Inner:
				result = key.SubType * 2 + 1;
				goto IL_67;
			case EMarkType.Poison:
				result = 20 + key.SubType;
				goto IL_67;
			case EMarkType.Mind:
				result = 15;
				goto IL_67;
			case EMarkType.Fatal:
				result = 14;
				goto IL_67;
			}
			result = 99;
			IL_67:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x06008EAF RID: 36527 RVA: 0x004262E8 File Offset: 0x004244E8
		private void LoadAndSetArmorIcon(ItemBack iconHolder, CombatSubProcessorCharacter processor)
		{
			bool flag = processor == null;
			if (flag)
			{
				iconHolder.SetIcon(string.Empty);
				iconHolder.SetBack(-1);
				this.enemyNoArmorNode.SetActive(true);
			}
			else
			{
				List<ItemDisplayData> equipments;
				bool flag2 = this.Model.EquipmentDataCache.TryGetValue(processor.CharacterId, out equipments) && equipments != null;
				if (flag2)
				{
					ItemDisplayData armor = equipments.Find((ItemDisplayData e) => e.EquipmentSlot == 5);
					bool flag3 = armor != null;
					if (flag3)
					{
						string icon = ItemTemplateHelper.GetIcon(armor.RealKey.ItemType, armor.RealKey.TemplateId);
						sbyte grade = ItemTemplateHelper.GetGrade(armor.RealKey.ItemType, armor.RealKey.TemplateId);
						iconHolder.SetIcon(icon);
						iconHolder.SetBack(grade);
						this.enemyNoArmorNode.SetActive(false);
						return;
					}
				}
				iconHolder.SetIcon(string.Empty);
				iconHolder.SetBack(-1);
				this.enemyNoArmorNode.SetActive(true);
			}
		}

		// Token: 0x06008EB0 RID: 36528 RVA: 0x004263FC File Offset: 0x004245FC
		private void SetupDiscs()
		{
			bool flag = this.Data == null;
			if (!flag)
			{
				int[] validSectionIndices = new int[3];
				int count = 0;
				for (int i = 0; i < 3; i++)
				{
					bool flag2 = this.Data.HitValue[i] >= 0 && this.Data.HitType[i] >= 0;
					if (flag2)
					{
						validSectionIndices[count++] = i;
					}
				}
				bool flag3 = count <= 0;
				if (!flag3)
				{
					this.SwitchHitRateGroup(count);
					GameObject group = this.GetActiveGroup(count);
					bool flag4 = group == null;
					if (!flag4)
					{
						CombatDamageDetailDisc[] discs = group.GetComponentsInChildren<CombatDamageDetailDisc>(true);
						int d = 0;
						while (d < discs.Length && d < count)
						{
							int si = validSectionIndices[d];
							discs[d].SetData(si);
							d++;
						}
					}
				}
			}
		}

		// Token: 0x06008EB1 RID: 36529 RVA: 0x004264E0 File Offset: 0x004246E0
		private void RefreshLines()
		{
			bool flag = this.Data == null;
			if (!flag)
			{
				int[] validSectionIndices = new int[3];
				int count = 0;
				for (int i = 0; i < 3; i++)
				{
					bool flag2 = this.Data.HitValue[i] >= 0 && this.Data.HitType[i] >= 0;
					if (flag2)
					{
						validSectionIndices[count++] = i;
					}
				}
				bool flag3 = count <= 0;
				if (!flag3)
				{
					GameObject group = this.GetActiveGroup(count);
					bool flag4 = group == null;
					if (!flag4)
					{
						CombatDamageDetailDisc[] discs = group.GetComponentsInChildren<CombatDamageDetailDisc>(true);
						CombatDamageDetailLine[] lines = group.GetComponentsInChildren<CombatDamageDetailLine>(true);
						bool flag5 = lines == null || lines.Length == 0;
						if (!flag5)
						{
							SkillDamageData skillDamageData = this.Model.SkillDamageData;
							Dictionary<int, SkillDamageSectionData> sections = (skillDamageData != null) ? skillDamageData.Sections : null;
							for (int li = 0; li < lines.Length; li++)
							{
								int targetDiscIndex = li + 1;
								bool flag6 = targetDiscIndex < discs.Length;
								if (flag6)
								{
									lines[li].SetState(discs[targetDiscIndex].GetCurrentState());
								}
								else
								{
									SkillDamageSectionData lineFinalSec = null;
									bool flag7 = sections != null;
									if (flag7)
									{
										sections.TryGetValue(3, out lineFinalSec);
									}
									bool hasFinalSection = lineFinalSec != null && lineFinalSec.Values != null && lineFinalSec.Values.Count > 0;
									lines[li].SetState(hasFinalSection ? CombatDamageDetailDisc.EState.Hit : CombatDamageDetailDisc.EState.NotConducted);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06008EB2 RID: 36530 RVA: 0x00426663 File Offset: 0x00424863
		private void SwitchHitRateGroup(int count)
		{
			this.damageHitRateOne.SetActive(count == 1);
			this.damageHitRateTwo.SetActive(count == 2);
			this.damageHitRateThree.SetActive(count == 3);
		}

		// Token: 0x06008EB3 RID: 36531 RVA: 0x00426698 File Offset: 0x00424898
		private GameObject GetActiveGroup(int count)
		{
			bool flag = count <= 1;
			GameObject result;
			if (flag)
			{
				result = this.damageHitRateOne;
			}
			else
			{
				bool flag2 = count == 2;
				if (flag2)
				{
					result = this.damageHitRateTwo;
				}
				else
				{
					result = this.damageHitRateThree;
				}
			}
			return result;
		}

		// Token: 0x06008EB4 RID: 36532 RVA: 0x004266D8 File Offset: 0x004248D8
		private void RefreshFinalDisc()
		{
			SkillDamageData skillDamageData = this.Model.SkillDamageData;
			SkillDamageSectionData finalSec = null;
			bool flag = ((skillDamageData != null) ? skillDamageData.Sections : null) != null;
			if (flag)
			{
				skillDamageData.Sections.TryGetValue(3, out finalSec);
			}
			bool hasFinalSection = finalSec != null && finalSec.Values != null && finalSec.Values.Count > 0;
			bool flag2 = !hasFinalSection;
			if (flag2)
			{
				this.finalSuccessNode.SetActive(false);
				this.finalFailNode.SetActive(true);
				this.finalDamageContainer.gameObject.SetActive(false);
				this.damagePartImage.gameObject.SetActive(false);
				bool hasFinalSectionResult = false;
				SkillDamageSectionData sec3;
				bool flag3 = ((skillDamageData != null) ? skillDamageData.Sections : null) != null && skillDamageData.Sections.TryGetValue(3, out sec3);
				if (flag3)
				{
					hasFinalSectionResult = (sec3.Result >= ESkillDamageSectionResult.Checked);
				}
				this.finalFailLabel.text = (hasFinalSectionResult ? LanguageKey.LK_CombatDamageDetail_FinalFail.Tr() : LanguageKey.LK_CombatDamageDetail_FinalFail_OutOfRange_Or_NotTriggered.Tr());
			}
			else
			{
				this.finalSuccessNode.SetActive(true);
				this.finalFailNode.SetActive(false);
				this.RefreshFinalDamageItems(finalSec.Values);
				this.RefreshFinalBodyPart(skillDamageData.TargetBodyPart);
				this.finalNormalNode.SetActive(!finalSec.Critical);
				this.finalCriticalNode.SetActive(finalSec.Critical);
			}
		}

		// Token: 0x06008EB5 RID: 36533 RVA: 0x0042683C File Offset: 0x00424A3C
		private void RefreshChengShu()
		{
			bool flag = this.Data == null || this.Data.SkillId < 0;
			if (flag)
			{
				this.damageNumImage.gameObject.SetActive(false);
			}
			else
			{
				CombatSkillItem skillConfig = CombatSkill.Instance[this.Data.SkillId];
				bool flag2 = skillConfig == null;
				if (flag2)
				{
					this.damageNumImage.gameObject.SetActive(false);
				}
				else
				{
					int totalDistribution = 0;
					SkillDamageData skillDamageData = this.Model.SkillDamageData;
					Dictionary<int, SkillDamageSectionData> sections = (skillDamageData != null) ? skillDamageData.Sections : null;
					for (int i = 0; i < 3; i++)
					{
						bool flag3 = this.Data.HitValue[i] < 0 || this.Data.HitType[i] < 0;
						if (!flag3)
						{
							ESkillDamageSectionResult sectionResult = ESkillDamageSectionResult.Uncheck;
							SkillDamageSectionData sectionData;
							bool flag4 = sections != null && sections.TryGetValue(i, out sectionData);
							if (flag4)
							{
								sectionResult = sectionData.Result;
							}
							bool flag5 = sectionResult < ESkillDamageSectionResult.Hit;
							if (!flag5)
							{
								sbyte t = this.Data.HitType[i];
								bool flag6 = t >= 0 && (int)t < skillConfig.PerHitDamageRateDistribution.Length;
								if (flag6)
								{
									totalDistribution += (int)skillConfig.PerHitDamageRateDistribution[(int)t];
								}
							}
						}
					}
					int digit = Mathf.Clamp(totalDistribution / 10, 0, 10);
					this.damageNumImage.SetSprite(string.Format("combat_bifen_chengshu_0_{0}", digit), false, null);
					this.damageNumImage.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x06008EB6 RID: 36534 RVA: 0x004269C0 File Offset: 0x00424BC0
		private void RefreshFinalDamageItems(Dictionary<DefeatMarkKey, int> values)
		{
			bool flag = values == null || values.Count == 0;
			if (flag)
			{
				this.finalDamageContainer.gameObject.SetActive(false);
			}
			else
			{
				this.finalDamageContainer.gameObject.SetActive(true);
				DefeatMarkKey[] sortedKeys = values.Keys.OrderBy(new Func<DefeatMarkKey, int>(ViewCombatDamageDetail.GetTotalItemOrder)).ToArray<DefeatMarkKey>();
				CommonUtils.PrepareEnoughChildren(this.finalDamageContainer, this.finalDamageItemPrefab.gameObject, sortedKeys.Length, null);
				for (int i = 0; i < sortedKeys.Length; i++)
				{
					CombatDamageDetailSectionItem item = this.finalDamageContainer.GetChild(i).GetComponent<CombatDamageDetailSectionItem>();
					item.Set(sortedKeys[i], values[sortedKeys[i]]);
				}
			}
		}

		// Token: 0x06008EB7 RID: 36535 RVA: 0x00426A90 File Offset: 0x00424C90
		private void RefreshFinalBodyPart(sbyte targetBodyPart)
		{
			bool flag = targetBodyPart < 0;
			if (flag)
			{
				this.damagePartImage.gameObject.SetActive(false);
			}
			else
			{
				this.damagePartImage.gameObject.SetActive(true);
				this.damagePartImage.SetSprite(string.Format("ui9_back_damage_detail_final_body_{0}", targetBodyPart), false, null);
				bool flag2 = this.Data.SkillId < 0;
				if (!flag2)
				{
					CombatSkillItem skillConfig = CombatSkill.Instance[this.Data.SkillId];
					bool flag3 = skillConfig == null;
					if (!flag3)
					{
						sbyte[] weights = skillConfig.InjuryPartAtkRateDistribution;
						bool flag4 = weights == null || weights.Length <= (int)targetBodyPart;
						if (flag4)
						{
							this.damagePartPercentText.text = "0%";
						}
						else
						{
							int totalWeight = 0;
							for (int i = 0; i < weights.Length; i++)
							{
								bool flag5 = weights[i] > 0;
								if (flag5)
								{
									totalWeight += (int)weights[i];
								}
							}
							bool flag6 = totalWeight <= 0;
							if (flag6)
							{
								this.damagePartPercentText.text = "0%";
							}
							else
							{
								sbyte weight = weights[(int)targetBodyPart];
								int showInt = (weight > 0) ? Mathf.Max(1, (int)((double)weight * 1000.0 / (double)totalWeight + 0.5)) : 0;
								this.damagePartPercentText.text = ((showInt > 0) ? string.Format("{0}.{1}%", showInt / 10, showInt % 10) : "0%");
							}
						}
					}
				}
			}
		}

		// Token: 0x04006CA2 RID: 27810
		[SerializeField]
		private CButton closeButton;

		// Token: 0x04006CA3 RID: 27811
		[Header("角色头像")]
		[SerializeField]
		private Game.Components.Avatar.Avatar leftAvatar;

		// Token: 0x04006CA4 RID: 27812
		[SerializeField]
		private Game.Components.Avatar.Avatar rightAvatar;

		// Token: 0x04006CA5 RID: 27813
		[SerializeField]
		private TextMeshProUGUI leftCharName;

		// Token: 0x04006CA6 RID: 27814
		[SerializeField]
		private TextMeshProUGUI rightCharName;

		// Token: 0x04006CA7 RID: 27815
		[SerializeField]
		private CButton leftCharMenuButton;

		// Token: 0x04006CA8 RID: 27816
		[SerializeField]
		private CButton rightCharMenuButton;

		// Token: 0x04006CA9 RID: 27817
		[Header("精纯等级")]
		[SerializeField]
		private CombatConsummateLevel leftConsummateLevel;

		// Token: 0x04006CAA RID: 27818
		[SerializeField]
		private CombatConsummateLevel rightConsummateLevel;

		// Token: 0x04006CAB RID: 27819
		[Header("装备图标")]
		[SerializeField]
		private ItemBack leftWeaponIcon;

		// Token: 0x04006CAC RID: 27820
		[SerializeField]
		private ItemBack rightArmorIcon;

		// Token: 0x04006CAD RID: 27821
		[SerializeField]
		private GameObject enemyNoArmorNode;

		// Token: 0x04006CAE RID: 27822
		[Header("当前功法")]
		[SerializeField]
		private CharacterMenuCombatSkillItem centerSkill;

		// Token: 0x04006CAF RID: 27823
		[Header("总伤害/效果")]
		[SerializeField]
		private RectTransform totalDamageContainer;

		// Token: 0x04006CB0 RID: 27824
		[SerializeField]
		private CombatDamageTotalItem totalDamageItemPrefab;

		// Token: 0x04006CB1 RID: 27825
		[Header("分段命中")]
		[SerializeField]
		private GameObject damageHitRateOne;

		// Token: 0x04006CB2 RID: 27826
		[SerializeField]
		private GameObject damageHitRateTwo;

		// Token: 0x04006CB3 RID: 27827
		[SerializeField]
		private GameObject damageHitRateThree;

		// Token: 0x04006CB4 RID: 27828
		[Header("中央汇总")]
		[SerializeField]
		private GameObject finalSuccessNode;

		// Token: 0x04006CB5 RID: 27829
		[SerializeField]
		private GameObject finalFailNode;

		// Token: 0x04006CB6 RID: 27830
		[SerializeField]
		private TextMeshProUGUI finalFailLabel;

		// Token: 0x04006CB7 RID: 27831
		[SerializeField]
		private CImage damageNumImage;

		// Token: 0x04006CB8 RID: 27832
		[SerializeField]
		private CImage damagePartImage;

		// Token: 0x04006CB9 RID: 27833
		[SerializeField]
		private TextMeshProUGUI damagePartPercentText;

		// Token: 0x04006CBA RID: 27834
		[SerializeField]
		private RectTransform finalDamageContainer;

		// Token: 0x04006CBB RID: 27835
		[SerializeField]
		private CombatDamageDetailSectionItem finalDamageItemPrefab;

		// Token: 0x04006CBC RID: 27836
		[SerializeField]
		private GameObject finalNormalNode;

		// Token: 0x04006CBD RID: 27837
		[SerializeField]
		private GameObject finalCriticalNode;

		// Token: 0x04006CBE RID: 27838
		[Header("武器耐久")]
		[SerializeField]
		private TextMeshProUGUI weaponDurabilityText;

		// Token: 0x04006CBF RID: 27839
		[Header("攻击和防御区域")]
		[SerializeField]
		private TextMeshProUGUI leftSideTitle;

		// Token: 0x04006CC0 RID: 27840
		[SerializeField]
		private TextMeshProUGUI rightSideTitle;

		// Token: 0x04006CC1 RID: 27841
		[SerializeField]
		private CombatDamageDetailPropertyItem selfOuterValue;

		// Token: 0x04006CC2 RID: 27842
		[SerializeField]
		private CombatDamageDetailPropertyItem enemyOuterValue;

		// Token: 0x04006CC3 RID: 27843
		[SerializeField]
		private CombatDamageDetailResultItem outerDamageResult;

		// Token: 0x04006CC4 RID: 27844
		[SerializeField]
		private CombatDamageDetailPropertyItem selfInnerValue;

		// Token: 0x04006CC5 RID: 27845
		[SerializeField]
		private CombatDamageDetailPropertyItem enemyInnerValue;

		// Token: 0x04006CC6 RID: 27846
		[SerializeField]
		private CombatDamageDetailResultItem innerDamageResult;

		// Token: 0x04006CC7 RID: 27847
		private CharacterAvatar _leftCharacterAvatar;

		// Token: 0x04006CC8 RID: 27848
		private CharacterAvatar _rightCharacterAvatar;
	}
}
