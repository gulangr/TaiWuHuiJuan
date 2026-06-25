using System;
using System.Collections.Generic;
using CharacterDataMonitor;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using TMPro;
using UICommon.Character.Elements;
using UnityEngine;

namespace UICommon.Character
{
	// Token: 0x020005EA RID: 1514
	public class CharacterTabInjury : Refers
	{
		// Token: 0x06004771 RID: 18289 RVA: 0x00217358 File Offset: 0x00215558
		public void Refresh(CombatResultDisplayData displayData, bool isAfter)
		{
			this._beforeSnapshot = displayData.SnapshotBeforeCombat;
			this._afterSnapshot = displayData.SnapshotAfterCombat;
			this._snapshot = (isAfter ? this._afterSnapshot : this._beforeSnapshot);
			this._isAfter = isAfter;
			this.RefreshInjury();
			this.RefreshPoison();
			this.RefreshDisorderOfQi();
			this.RefreshHealth();
			this.RefreshEatingItem();
		}

		// Token: 0x06004772 RID: 18290 RVA: 0x002173C0 File Offset: 0x002155C0
		private void RefreshEatingItem()
		{
			List<ValueTuple<ItemKey, short>> eatingItemList = new List<ValueTuple<ItemKey, short>>();
			EatingItemMonitor.DecodeEatingItems(this._snapshot.EatingItemList, ref eatingItemList);
			CharacterTabInjury.FillEatingItem(this, (int)this._snapshot.CanEatingMaxCount, eatingItemList, SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
		}

		// Token: 0x17000901 RID: 2305
		// (get) Token: 0x06004773 RID: 18291 RVA: 0x00217404 File Offset: 0x00215604
		private static string[] WugSkeletonNames
		{
			get
			{
				return CharacterAttributeDataView.WugSkeletonNames;
			}
		}

		// Token: 0x17000902 RID: 2306
		// (get) Token: 0x06004774 RID: 18292 RVA: 0x0021740B File Offset: 0x0021560B
		private static string[] WugSkeletonSlotOrAttachmentNames
		{
			get
			{
				return CharacterAttributeDataView.WugSkeletonSlotOrAttachmentNames;
			}
		}

		// Token: 0x06004775 RID: 18293 RVA: 0x00217414 File Offset: 0x00215614
		public static void FillEatingItem(Refers tabRefer, int canEatingMaxCount, List<ValueTuple<ItemKey, short>> eatingItemList, int characterId)
		{
			List<Refers> eatingItemRefers = tabRefer.CGet<Refers>("AreaEatingItems").CGetList<Refers>("Item_");
			Refers wugAnimations = tabRefer.CGet<Refers>("WugAnimationReferences");
			for (int i = 0; i < eatingItemRefers.Count; i++)
			{
				Refers refers = eatingItemRefers[i];
				CImage backImg = refers.GetComponent<CImage>();
				CImage icon = refers.CGet<CImage>("Icon");
				TooltipInvoker mouseTip = refers.CGet<TooltipInvoker>("Mousetip");
				mouseTip.IsLanguageKey = false;
				string iconName = string.Empty;
				ValueTuple<ItemKey, short> tuple = new ValueTuple<ItemKey, short>(ItemKey.Invalid, -1);
				bool flag = eatingItemList.CheckIndex(i);
				if (flag)
				{
					tuple = eatingItemList[i];
				}
				bool flag2 = EatingItems.IsValid(tuple.Item1);
				if (flag2)
				{
					backImg.SetSprite("sp_02_gn_fushidizi_5", false, null);
					bool flag3 = EatingItems.IsWug(tuple.Item1) && !EatingItems.IsWugKing(tuple.Item1);
					if (flag3)
					{
						mouseTip.Type = TipType.EatingWug;
						mouseTip.RuntimeParam = EasyPool.Get<ArgumentBox>().Set("TemplateId", tuple.Item1.TemplateId).Set("CharId", characterId).Set("EatingTime", tuple.Item2);
					}
					else
					{
						iconName = ItemTemplateHelper.GetIcon(tuple.Item1.ItemType, tuple.Item1.TemplateId);
						ItemDomainMethod.AsyncCall.GetItemDisplayData(null, tuple.Item1, delegate(int offset, RawDataPool dataPool)
						{
							ItemDisplayData itemData = null;
							Serializer.Deserialize(dataPool, offset, ref itemData);
							mouseTip.Type = TooltipManager.ItemTypeToTipType[tuple.Item1.ItemType];
							bool flag5 = mouseTip.RuntimeParam == null;
							if (flag5)
							{
								mouseTip.RuntimeParam = new ArgumentBox();
							}
							mouseTip.RuntimeParam.SetObject("ItemData", itemData);
							mouseTip.RuntimeParam.Set("EatingTime", tuple.Item2);
							mouseTip.RuntimeParam.Set("CharId", characterId);
						});
					}
				}
				else
				{
					backImg.SetSprite((i < canEatingMaxCount) ? "sp_02_gn_fushidizi_1" : "sp_02_gn_fushidizi_4", false, null);
					string tipsContent = LocalStringManager.Get((i < canEatingMaxCount) ? LanguageKey.LK_CharacterMenu_Injury_EatingNull : LanguageKey.LK_CharacterMenu_Injury_EatingLimit);
					mouseTip.Type = TipType.SingleDesc;
					mouseTip.RuntimeParam = null;
					mouseTip.PresetParam = new string[]
					{
						tipsContent
					};
				}
				icon.SetSprite(iconName, false, null);
				bool isWug = EatingItems.IsWug(tuple.Item1);
				SkeletonGraphic wugSkeleton = refers.CGet<SkeletonGraphic>("Wug");
				wugSkeleton.gameObject.SetActive(isWug);
				bool flag4 = isWug;
				if (flag4)
				{
					MedicineItem wugConfig = Medicine.Instance[tuple.Item1.TemplateId];
					string wugName = CharacterTabInjury.WugSkeletonNames[(int)wugConfig.WugGrowthType];
					SkeletonDataAsset dataAsset = wugAnimations.CGet<SkeletonDataAsset>(wugName);
					CommonUtils.SetSkeletonDataAsset(wugSkeleton, dataAsset, "default", "animation", true);
					string slotOrAttachmentName = CharacterTabInjury.WugSkeletonSlotOrAttachmentNames[(int)wugConfig.WugGrowthType];
					string slotName = string.Format("images/{0}", slotOrAttachmentName);
					string attachmentName = string.Format("images/{0}_{1}", slotOrAttachmentName, (int)(wugConfig.WugType + 1));
					wugSkeleton.Skeleton.FindSlot(slotName).Attachment = wugSkeleton.Skeleton.GetAttachment(slotName, attachmentName);
				}
				bool showing = mouseTip.Showing;
				if (showing)
				{
					mouseTip.Refresh(false, -1);
				}
			}
		}

		// Token: 0x06004776 RID: 18294 RVA: 0x00217780 File Offset: 0x00215980
		private void RefreshHealth()
		{
			CharacterHealthBar characterHealthInfo = base.CGet<CharacterHealthBar>("CharacterHealthInfo");
			bool isAfter = this._isAfter;
			if (isAfter)
			{
				bool isAddHealth = this._afterSnapshot.Health > this._beforeSnapshot.Health;
				Refers healthRefers = base.CGet<CharacterHealthBar>("CharacterHealthInfo").GetComponent<Refers>();
				CImage progressChange = healthRefers.CGet<CImage>("ProgressChange");
				progressChange.gameObject.SetActive(this._afterSnapshot.Health != this._beforeSnapshot.Health);
				progressChange.SetSprite(isAddHealth ? "sp_02_gn_healthprogress_0_4" : "sp_02_gn_healthprogress_0_3", false, null);
				bool flag = isAddHealth;
				if (flag)
				{
					characterHealthInfo.Refresh(this._afterSnapshot.Health, this._afterSnapshot.LeftMaxHealth, this._afterSnapshot.HealthRecovery, -1);
					characterHealthInfo.RefreshProgress(this._beforeSnapshot.Health, this._beforeSnapshot.LeftMaxHealth);
					progressChange.fillAmount = (float)this._afterSnapshot.Health / (float)this._afterSnapshot.LeftMaxHealth;
				}
				else
				{
					characterHealthInfo.Refresh(this._afterSnapshot.Health, this._afterSnapshot.LeftMaxHealth, this._afterSnapshot.HealthRecovery, -1);
					progressChange.fillAmount = (float)this._beforeSnapshot.Health / (float)this._beforeSnapshot.LeftMaxHealth;
				}
			}
			else
			{
				characterHealthInfo.Refresh(this._snapshot.Health, this._snapshot.LeftMaxHealth, this._snapshot.HealthRecovery, -1);
			}
		}

		// Token: 0x06004777 RID: 18295 RVA: 0x0021790C File Offset: 0x00215B0C
		private void RefreshDisorderOfQi()
		{
			Refers disorderOfQiRefers = base.CGet<Refers>("AreaPeriodEffect");
			CSliderLegacy slider = disorderOfQiRefers.CGet<CSliderLegacy>("QiSlider");
			TextMeshProUGUI stateLabel = disorderOfQiRefers.CGet<TextMeshProUGUI>("QiStateText");
			CImage stateIcon = disorderOfQiRefers.CGet<CImage>("QiStateIcon");
			CharacterDisorderOfQi.InitDisorderOfQi(slider, stateLabel);
			bool flag = null != slider;
			if (flag)
			{
				slider.value = (float)this._snapshot.DisorderOfQi;
				float value = slider.value / slider.maxValue;
				base.CGet<Refers>("AreaPeriodEffect").CGet<CImage>("Image_Real_Progress").fillAmount = value;
				CharacterAttributeDataView.UpdateDisorderOfQiSliderHandle(base.CGet<Refers>("AreaPeriodEffect"), value);
			}
			sbyte level = DisorderLevelOfQi.GetDisorderLevelOfQi(this._snapshot.DisorderOfQi);
			bool flag2 = null != stateLabel;
			if (flag2)
			{
				stateLabel.text = LocalStringManager.Get(CharacterDisorderOfQi.DisorderOfQiLevelLangKeys[(int)level]);
				stateLabel.color = CharacterDisorderOfQi.DisorderOfQiLevelColors[(int)level];
			}
			bool flag3 = null != stateIcon;
			if (flag3)
			{
				stateIcon.SetSprite(CharacterDisorderOfQi.GetDisorderOfQiLevelIcon(level), false, null);
			}
			RectTransform rectTransform = disorderOfQiRefers.CGet<RectTransform>("QiChangeMask");
			if (rectTransform != null)
			{
				rectTransform.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004778 RID: 18296 RVA: 0x00217A38 File Offset: 0x00215C38
		public static Refers[] SetPoisonRefers(Refers poisonRefers)
		{
			return new Refers[]
			{
				poisonRefers.CGet<Refers>("Resist_Lie"),
				poisonRefers.CGet<Refers>("Resist_Yu"),
				poisonRefers.CGet<Refers>("Resist_Han"),
				poisonRefers.CGet<Refers>("Resist_Chi"),
				poisonRefers.CGet<Refers>("Resist_Fu"),
				poisonRefers.CGet<Refers>("Resist_Huan")
			};
		}

		// Token: 0x06004779 RID: 18297 RVA: 0x00217AA8 File Offset: 0x00215CA8
		public static GameObject[] SetPoisonMarkRefers(Refers poisonMarkRefers)
		{
			return new GameObject[]
			{
				poisonMarkRefers.CGet<GameObject>("Lie"),
				poisonMarkRefers.CGet<GameObject>("Yu"),
				poisonMarkRefers.CGet<GameObject>("Han"),
				poisonMarkRefers.CGet<GameObject>("Chi"),
				poisonMarkRefers.CGet<GameObject>("Fu"),
				poisonMarkRefers.CGet<GameObject>("Huan")
			};
		}

		// Token: 0x0600477A RID: 18298 RVA: 0x00217B18 File Offset: 0x00215D18
		private void RefreshPoison()
		{
			Refers poisonRefers = base.CGet<Refers>("AreaInjuryPoison").CGet<Refers>("PoisonRoot");
			Refers[] allPoisonRefersArray = CharacterTabInjury.SetPoisonRefers(poisonRefers);
			Refers poisonMarkRefers = poisonRefers.CGet<Refers>("Poisoned");
			GameObject[] allPoisonMarksArray = CharacterTabInjury.SetPoisonMarkRefers(poisonMarkRefers);
			int sum = 0;
			for (int i = 0; i < 6; i++)
			{
				PoisonItem config = Poison.Instance[i];
				Refers refer = allPoisonRefersArray[i];
				int poisonValue = this._snapshot.Poisons.Get(i);
				sbyte poisonLevel = PoisonsAndLevels.CalcPoisonedLevel(poisonValue);
				sum += (int)poisonLevel;
				refer.CGet<TextMeshProUGUI>("Name").text = config.Name;
				refer.CGet<CImage>("Icon").SetSprite(config.Icon, false, null);
				refer.CGet<TextMeshProUGUI>("PoisonValue").text = poisonValue.ToString().SetColor("poisoned");
				int poisonChange = this._afterSnapshot.Poisons.Get(i) - this._beforeSnapshot.Poisons.Get(i);
				bool showPoisonChange = poisonChange != 0 && this._isAfter;
				refer.CGet<GameObject>("NewAddPoison").SetActive(showPoisonChange);
				bool flag = showPoisonChange;
				if (flag)
				{
					string poisonChangeText = (poisonChange > 0) ? string.Format("+{0}", poisonChange) : poisonChange.ToString();
					refer.CGet<TextMeshProUGUI>("NewAddPoisonValue").text = poisonChangeText.SetColor((poisonChange > 0) ? "brightred" : "brightblue");
				}
				Refers progressBarRefer = refer.CGet<Refers>("ProgressBar");
				this.ProgressBarResetToEmpty(progressBarRefer);
				CImage progressBarImage = progressBarRefer.CGet<RectTransform>("Progress").GetComponent<CImage>();
				progressBarImage.fillAmount = this.SetProgressBarValue(poisonValue);
				CImage progressChange = refer.CGet<CImage>("ProgressChange");
				progressChange.gameObject.SetActive(showPoisonChange);
				bool flag2 = showPoisonChange;
				if (flag2)
				{
					progressChange.SetSprite((poisonChange > 0) ? "sp_02_gn_tiaotiaojia" : "sp_02_gn_tiaotiaojian", false, null);
					bool flag3 = poisonChange > 0;
					if (flag3)
					{
						progressBarImage.fillAmount = this.SetProgressBarValue(this._beforeSnapshot.Poisons.Get(i));
						progressChange.fillAmount = this.SetProgressBarValue(this._afterSnapshot.Poisons.Get(i));
					}
					else
					{
						progressBarImage.fillAmount = this.SetProgressBarValue(this._afterSnapshot.Poisons.Get(i));
						progressChange.fillAmount = this.SetProgressBarValue(this._beforeSnapshot.Poisons.Get(i));
					}
				}
				this.SetDot(refer, poisonLevel);
				allPoisonMarksArray[i].gameObject.SetActive(poisonValue > 0);
			}
			TextMeshProUGUI label = base.CGet<Refers>("AreaInjuryPoison").CGet<Refers>("Total").CGet<TextMeshProUGUI>("PoisonValue");
			label.text = sum.ToString().SetColor("poisoned");
		}

		// Token: 0x0600477B RID: 18299 RVA: 0x00217E10 File Offset: 0x00216010
		public void SetDot(Refers refer, sbyte poisonLevel)
		{
			List<CImage> dotList = refer.CGetList<CImage>("Dot_");
			foreach (CImage dot in dotList)
			{
				dot.gameObject.SetActive(false);
			}
			for (int i = 0; i < (int)poisonLevel; i++)
			{
				dotList[i].gameObject.SetActive(true);
			}
		}

		// Token: 0x0600477C RID: 18300 RVA: 0x00217E98 File Offset: 0x00216098
		public float SetProgressBarValue(int poisonValue)
		{
			sbyte poisonLevel = PoisonsAndLevels.CalcPoisonedLevel(poisonValue);
			short maxValue = 25000;
			bool flag = GlobalConfig.Instance.PoisonLevelThresholds.CheckIndex((int)poisonLevel);
			if (flag)
			{
				maxValue = GlobalConfig.Instance.PoisonLevelThresholds[(int)poisonLevel];
			}
			bool flag2 = (float)maxValue < 1E-05f;
			float result;
			if (flag2)
			{
				result = 0f;
			}
			else
			{
				float progressValue = (float)poisonValue / (float)maxValue;
				result = progressValue;
			}
			return result;
		}

		// Token: 0x0600477D RID: 18301 RVA: 0x00217EFC File Offset: 0x002160FC
		public void ProgressBarResetToEmpty(Refers refer)
		{
			RectTransform barNewlyProgress = refer.CGet<RectTransform>("NewlyProgress");
			CImage progressNewlyBar = barNewlyProgress.GetComponent<CImage>();
			progressNewlyBar.fillAmount = 0f;
		}

		// Token: 0x0600477E RID: 18302 RVA: 0x00217F2C File Offset: 0x0021612C
		public static Refers[] InitInjuryGroupRefers(Refers injuryRefers, Refers tabInjury)
		{
			return new Refers[]
			{
				injuryRefers.CGet<Refers>("Chest"),
				injuryRefers.CGet<Refers>("Belly"),
				injuryRefers.CGet<Refers>("Head"),
				injuryRefers.CGet<Refers>("LeftHand"),
				injuryRefers.CGet<Refers>("RightHand"),
				injuryRefers.CGet<Refers>("LeftLeg"),
				injuryRefers.CGet<Refers>("RightLeg"),
				tabInjury.CGet<Refers>("AreaInjuryPoison").CGet<Refers>("Total")
			};
		}

		// Token: 0x0600477F RID: 18303 RVA: 0x00217FC0 File Offset: 0x002161C0
		private void RefreshInjury()
		{
			Refers injuryRefers = base.CGet<Refers>("AreaInjuryPoison").CGet<Refers>("InjuryRoot");
			Refers[] injuryGroupRefers = CharacterTabInjury.InitInjuryGroupRefers(injuryRefers, this);
			CharacterInjuryGroup.InitNameAndValue(injuryGroupRefers);
			CharacterInjuryGroup.ResetToEmpty(injuryGroupRefers);
			sbyte sumOuter = 0;
			sbyte sumInner = 0;
			for (sbyte i = 0; i < 7; i += 1)
			{
				ValueTuple<sbyte, sbyte> valueTuple = this._snapshot.Injuries.Get(i);
				sbyte partOuter = valueTuple.Item1;
				sbyte partInner = valueTuple.Item2;
				CharacterInjuryGroup.FillInjuryByType(i, new ValueTuple<sbyte, sbyte>(partOuter, partInner), injuryGroupRefers, null);
				sumOuter += partOuter;
				sumInner += partInner;
			}
			CharacterInjuryGroup.RefreshSumInjury(this._snapshot.TemplateId, injuryGroupRefers, sumOuter, sumInner);
			bool isAfter = this._isAfter;
			if (isAfter)
			{
				CharacterInjuryGroup.SetInjuryChange(injuryGroupRefers, this._beforeSnapshot.Injuries, this._afterSnapshot.Injuries);
			}
		}

		// Token: 0x0400314A RID: 12618
		private CombatResultSnapshot _beforeSnapshot;

		// Token: 0x0400314B RID: 12619
		private CombatResultSnapshot _afterSnapshot;

		// Token: 0x0400314C RID: 12620
		private CombatResultSnapshot _snapshot;

		// Token: 0x0400314D RID: 12621
		private bool _isAfter;
	}
}
