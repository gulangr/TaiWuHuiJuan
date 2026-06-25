using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Config;
using FrameWork;
using Game.Views.Encyclopedia;
using Game.Views.MouseTips.Common;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using GameData.Domains.Item;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000852 RID: 2130
	public class MouseTipDefeatMark : MouseTipBase
	{
		// Token: 0x0600676C RID: 26476 RVA: 0x002F2860 File Offset: 0x002F0A60
		private static bool NeedsBackendData(EMarkType markType)
		{
			if (!true)
			{
			}
			bool result = markType == EMarkType.Poison || markType == EMarkType.Wug || markType == EMarkType.State;
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600676D RID: 26477 RVA: 0x002F28A0 File Offset: 0x002F0AA0
		public override bool CanShowWithArgumentBox(ArgumentBox argumentBox)
		{
			int markKeyInt;
			List<DefeatMarkKey> markKeyList;
			bool flag = argumentBox.Get("MarkKey", out markKeyInt) && argumentBox.Get<List<DefeatMarkKey>>("MarkKeyList", out markKeyList);
			bool result;
			if (flag)
			{
				bool flag2 = !((DefeatMarkKey)markKeyInt).Valid || (markKeyList == null || markKeyList.Count <= 0);
				if (flag2)
				{
					result = false;
				}
				else
				{
					int charId;
					bool flag3 = argumentBox.Get("CharId", out charId);
					if (flag3)
					{
						CombatModel combatModel = SingletonObject.getInstance<CombatModel>();
						bool flag4 = ((combatModel != null) ? combatModel.ProcessorCharacters : null) == null || !combatModel.ProcessorCharacters.ContainsKey(charId);
						if (flag4)
						{
							return false;
						}
					}
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600676E RID: 26478 RVA: 0x002F295C File Offset: 0x002F0B5C
		protected override void Init(ArgumentBox argsBox)
		{
			this.Element.ForceListenCommand = true;
			int markKeyInt;
			argsBox.Get("MarkKey", out markKeyInt);
			this._markKey = (DefeatMarkKey)markKeyInt;
			argsBox.Get<List<DefeatMarkKey>>("MarkKeyList", out this._markKeyList);
			argsBox.Get("CharId", out this._charId);
			this._tipData.Reset();
			this._tipData.MarkType = this._markKey.Type;
			bool flag = MouseTipDefeatMark.NeedsBackendData(this._markKey.Type);
			if (flag)
			{
				this.NeedWaitData = true;
				CombatDomainMethod.AsyncCall.GetMarkDisplayData(this, this._charId, this._markKey, delegate(int offset, RawDataPool pool)
				{
					DefeatMarkDetailInfoDisplayData defeatMarkData = new DefeatMarkDetailInfoDisplayData();
					Serializer.Deserialize(pool, offset, ref defeatMarkData);
					this.CopyToTipData(defeatMarkData, ref this._tipData);
					this.FillDefeatMarkDataFromFrontend(ref this._tipData);
					this.UpdateMarks();
					this.Element.ShowAfterRefresh();
				});
			}
			else
			{
				this.FillDefeatMarkDataFromFrontend(ref this._tipData);
				this.UpdateMarks();
			}
		}

		// Token: 0x0600676F RID: 26479 RVA: 0x002F2A2C File Offset: 0x002F0C2C
		private void Update()
		{
			this.UpdateDetail();
			this.UpdateHotKeyDetail();
			bool flag = CommonCommandKit.PrimaryInteraction.Check(this.Element, false, false, false, true, false);
			if (flag)
			{
				ViewEncyclopediaPanel.OpenLink(EncyclopediaTipLink.DefValue.DefeatMarkers);
			}
		}

		// Token: 0x06006770 RID: 26480 RVA: 0x002F2A6C File Offset: 0x002F0C6C
		protected void UpdateDetail()
		{
			bool hasStick = this.HasStick;
			if (!hasStick)
			{
				bool altDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
				this.IsDetail = altDown;
				bool flag = this.detailInfoPanel && this.detailInfoPanel.gameObject.activeSelf != this.IsDetail;
				if (flag)
				{
					this.detailInfoPanel.gameObject.SetActive(this.IsDetail);
					this.Refresh();
				}
			}
		}

		// Token: 0x06006771 RID: 26481 RVA: 0x002F2AF8 File Offset: 0x002F0CF8
		private void UpdateHotKeyDetail()
		{
			this.tooltipOperationArea.ShowHotkeyDisplayDetail(true);
			bool isDetail = this.IsDetail;
			if (isDetail)
			{
				this.tooltipOperationArea.RefreshCancelDetail();
			}
			else
			{
				this.tooltipOperationArea.RefreshPressToDetail();
			}
		}

		// Token: 0x06006772 RID: 26482 RVA: 0x002F2B38 File Offset: 0x002F0D38
		private void CopyToTipData(DefeatMarkDetailInfoDisplayData source, ref MouseTipDefeatMark.DefeatMarkTipData target)
		{
			target.PoisonTriggerProgress = source.PoisonTriggerProgress;
			target.StateBuffPower = source.StateBuffPower;
			bool flag = source.WugTemplateIds != null;
			if (flag)
			{
				target.WugTemplateIds = (short[])source.WugTemplateIds.Clone();
			}
		}

		// Token: 0x06006773 RID: 26483 RVA: 0x002F2B81 File Offset: 0x002F0D81
		private bool GroupEquals(DefeatMarkKey markKey)
		{
			return this._markKey.GroupEquals(markKey);
		}

		// Token: 0x06006774 RID: 26484 RVA: 0x002F2B8F File Offset: 0x002F0D8F
		private bool GroupEqualsAndOld(DefeatMarkKey markKey)
		{
			return markKey.Old && this.GroupEquals(markKey);
		}

		// Token: 0x06006775 RID: 26485 RVA: 0x002F2BA4 File Offset: 0x002F0DA4
		private void FillDefeatMarkDataFromFrontend(ref MouseTipDefeatMark.DefeatMarkTipData data)
		{
			data.MarkType = this._markKey.Type;
			CombatModel combatModel = SingletonObject.getInstance<CombatModel>();
			CombatSubProcessorCharacter processor;
			bool flag = ((combatModel != null) ? combatModel.ProcessorCharacters : null) == null || !combatModel.ProcessorCharacters.TryGetValue(this._charId, out processor) || processor.DefeatMarkCollection == null;
			if (!flag)
			{
				int count = this._markKeyList.Count(new Func<DefeatMarkKey, bool>(this.GroupEquals));
				bool flag2 = !this._markKey.HasOld;
				if (flag2)
				{
					data.NewMarkCount = count;
				}
				else
				{
					data.OldMarkCount = this._markKeyList.Count(new Func<DefeatMarkKey, bool>(this.GroupEqualsAndOld));
					data.NewMarkCount = count - data.OldMarkCount;
				}
				switch (this._markKey.Type)
				{
				case EMarkType.Outer:
					this.FillOuterInjuryData(ref data, processor);
					break;
				case EMarkType.Inner:
					this.FillInnerInjuryData(ref data, processor);
					break;
				case EMarkType.Flaw:
					this.FillFlawData(ref data, processor);
					break;
				case EMarkType.Acupoint:
					this.FillAcupointData(ref data, processor);
					break;
				case EMarkType.Poison:
					this.FillPoisonData(ref data, processor);
					break;
				case EMarkType.Fatal:
					this.FillFatalData(ref data, processor, combatModel);
					break;
				case EMarkType.Wug:
					this.FillWugData(ref data, processor, combatModel);
					break;
				case EMarkType.QiDisorder:
					this.FillQiDisorderData(ref data, processor);
					break;
				case EMarkType.NeiliAllocation:
					this.FillNeiliAllocationData(ref data, processor);
					break;
				case EMarkType.Health:
					this.FillHealthData(ref data, processor);
					break;
				}
			}
		}

		// Token: 0x06006776 RID: 26486 RVA: 0x002F2D24 File Offset: 0x002F0F24
		private void FillOuterInjuryData(ref MouseTipDefeatMark.DefeatMarkTipData data, CombatSubProcessorCharacter processor)
		{
			sbyte bodyPart = this._markKey.BodyPart;
			data.SubTypeId = bodyPart;
		}

		// Token: 0x06006777 RID: 26487 RVA: 0x002F2D48 File Offset: 0x002F0F48
		private void FillInnerInjuryData(ref MouseTipDefeatMark.DefeatMarkTipData data, CombatSubProcessorCharacter processor)
		{
			sbyte bodyPart = this._markKey.BodyPart;
			data.SubTypeId = bodyPart;
		}

		// Token: 0x06006778 RID: 26488 RVA: 0x002F2D6C File Offset: 0x002F0F6C
		private void FillHealthData(ref MouseTipDefeatMark.DefeatMarkTipData data, CombatSubProcessorCharacter processor)
		{
			sbyte healthMarkCount = processor.DefeatMarkCollection.HealthMarkCount;
			data.HealthType = MouseTipDefeatMark.GetHealthTypeFromMarkCount(healthMarkCount);
		}

		// Token: 0x06006779 RID: 26489 RVA: 0x002F2D94 File Offset: 0x002F0F94
		private static EHealthType GetHealthTypeFromMarkCount(sbyte markCount)
		{
			if (!true)
			{
			}
			EHealthType result;
			if (markCount >= 4)
			{
				if (markCount < 8)
				{
					if (markCount < 6)
					{
						result = EHealthType.Weak;
					}
					else
					{
						result = EHealthType.CriticallyIll;
					}
				}
				else
				{
					result = EHealthType.Dying;
				}
			}
			else if (markCount < 2)
			{
				result = EHealthType.Healthy;
			}
			else
			{
				result = EHealthType.Sick;
			}
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x0600677A RID: 26490 RVA: 0x002F2DD8 File Offset: 0x002F0FD8
		private void FillPoisonData(ref MouseTipDefeatMark.DefeatMarkTipData data, CombatSubProcessorCharacter processor)
		{
			sbyte poisonType = this._markKey.PoisonType;
			data.SubTypeId = poisonType;
			byte markCount = processor.DefeatMarkCollection.PoisonMarkList[(int)poisonType];
			data.PoisonLevel = (sbyte)markCount;
		}

		// Token: 0x0600677B RID: 26491 RVA: 0x002F2E10 File Offset: 0x002F1010
		private void FillFlawData(ref MouseTipDefeatMark.DefeatMarkTipData data, CombatSubProcessorCharacter processor)
		{
			sbyte bodyPart = this._markKey.BodyPart;
			ByteList flawList = processor.DefeatMarkCollection.FlawMarkList[(int)bodyPart];
			data.SubTypeId = bodyPart;
			data.LevelList = new List<sbyte>();
			sbyte level;
			sbyte level2;
			for (level = 1; level <= 4; level = level2 + 1)
			{
				int count = (flawList != null) ? flawList.Count((byte l) => l == (byte)level) : 0;
				data.LevelList.Add((sbyte)count);
				level2 = level;
			}
		}

		// Token: 0x0600677C RID: 26492 RVA: 0x002F2EA8 File Offset: 0x002F10A8
		private void FillAcupointData(ref MouseTipDefeatMark.DefeatMarkTipData data, CombatSubProcessorCharacter processor)
		{
			sbyte bodyPart = this._markKey.BodyPart;
			ByteList acupointList = processor.DefeatMarkCollection.AcupointMarkList[(int)bodyPart];
			data.SubTypeId = bodyPart;
			data.LevelList = new List<sbyte>();
			sbyte level;
			sbyte level2;
			for (level = 1; level <= 4; level = level2 + 1)
			{
				int count = (acupointList != null) ? acupointList.Count((byte l) => l == (byte)level) : 0;
				data.LevelList.Add((sbyte)count);
				level2 = level;
			}
		}

		// Token: 0x0600677D RID: 26493 RVA: 0x002F2F40 File Offset: 0x002F1140
		private void FillFatalData(ref MouseTipDefeatMark.DefeatMarkTipData data, CombatSubProcessorCharacter processor, CombatModel combatModel)
		{
			bool flag = ((combatModel != null) ? combatModel.Config : null) != null;
			if (flag)
			{
				data.CombatType = (CombatType)combatModel.Config.CombatType;
			}
		}

		// Token: 0x0600677E RID: 26494 RVA: 0x002F2F74 File Offset: 0x002F1174
		private void FillQiDisorderData(ref MouseTipDefeatMark.DefeatMarkTipData data, CombatSubProcessorCharacter processor)
		{
			short disorderOfQi = processor.DisorderOfQi;
			sbyte level = DisorderLevelOfQi.GetDisorderLevelOfQi(disorderOfQi);
			data.QiDisorderLevel = level;
			QiDisorderEffectItem config = QiDisorderEffect.Instance[level];
			bool flag = config != null;
			if (flag)
			{
				data.InjuredRate = (int)config.InjuredRate;
				data.NeiliCostInCombat = (int)config.NeiliCostInCombat;
				data.PoisonResistChange = config.PoisonResistChange;
				data.HealthRecovery = (int)config.HealthRecovery;
			}
		}

		// Token: 0x0600677F RID: 26495 RVA: 0x002F2FE0 File Offset: 0x002F11E0
		private unsafe void FillNeiliAllocationData(ref MouseTipDefeatMark.DefeatMarkTipData data, CombatSubProcessorCharacter processor)
		{
			data.NeiliStatusList = new List<ENeiliAllocationStatusType>(4);
			for (byte i = 0; i < 4; i += 1)
			{
				ENeiliAllocationStatusType status = NeiliAllocationStatusHelper.GetStatus(*processor.NeiliAllocation[(int)i], *processor.OriginNeiliAllocation[(int)i]);
				data.NeiliStatusList.Add(status);
			}
		}

		// Token: 0x06006780 RID: 26496 RVA: 0x002F303C File Offset: 0x002F123C
		private unsafe void FillWugData(ref MouseTipDefeatMark.DefeatMarkTipData data, CombatSubProcessorCharacter processor, CombatModel combatModel)
		{
			CombatSubProcessorTaiwu processorTaiwu = (combatModel != null) ? combatModel.ProcessorTaiwu : null;
			bool flag = processorTaiwu == null;
			if (!flag)
			{
				bool flag2 = processor.CharacterId != SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
				if (!flag2)
				{
					data.WugTemplateIds = new short[9];
					for (int i = 0; i < 9; i++)
					{
						data.WugTemplateIds[i] = -1;
					}
					EatingItems eatingItems = processorTaiwu.EatingItems;
					for (int j = 0; j < 9; j++)
					{
						ItemKey itemKey = (ItemKey)(*(ref eatingItems.ItemKeys.FixedElementField + (IntPtr)j * 8));
						bool flag3 = EatingItems.IsValid(itemKey) && EatingItems.IsWug(itemKey);
						if (flag3)
						{
							data.WugTemplateIds[j] = itemKey.TemplateId;
						}
					}
				}
			}
		}

		// Token: 0x06006781 RID: 26497 RVA: 0x002F3114 File Offset: 0x002F1314
		private void UpdateMarks()
		{
			this.titleLabel.text = CombatConstants.ParseMarkType(this._markKey);
			this.typeLabel.text = TMPTextSpriteHelper.GetStringWithTextSpriteTag(CombatConstants.ParseMarkIcon(this._markKey)) + CombatConstants.ParseMarkName(this._markKey);
			TMPTextSpriteHelper typeLabelSpriteHelper;
			bool flag = this.typeLabel.TryGetComponent<TMPTextSpriteHelper>(out typeLabelSpriteHelper);
			if (flag)
			{
				typeLabelSpriteHelper.autoClear = false;
				typeLabelSpriteHelper.Parse();
			}
			this.countLabel.text = (this._tipData.NewMarkCount + this._tipData.OldMarkCount).ToString();
			this.extraHolder.gameObject.SetActive(false);
			List<ValueTuple<string, string>> tempMarkData = new List<ValueTuple<string, string>>();
			switch (this._markKey.Type)
			{
			case EMarkType.Outer:
				tempMarkData.Add(new ValueTuple<string, string>(LanguageKey.LK_Combat_MarkName_Outer_Temp.Tr(), this._tipData.NewMarkCount.ToString()));
				tempMarkData.Add(new ValueTuple<string, string>(LanguageKey.LK_Combat_MarkName_Outer_Old.Tr(), this._tipData.OldMarkCount.ToString()));
				break;
			case EMarkType.Inner:
				tempMarkData.Add(new ValueTuple<string, string>(LanguageKey.LK_Combat_MarkName_Inner_Temp.Tr(), this._tipData.NewMarkCount.ToString()));
				tempMarkData.Add(new ValueTuple<string, string>(LanguageKey.LK_Combat_MarkName_Inner_Old.Tr(), this._tipData.OldMarkCount.ToString()));
				break;
			case EMarkType.Flaw:
			{
				List<sbyte> levelList = this._tipData.LevelList;
				bool flag2 = levelList == null || levelList.Count <= 0;
				if (!flag2)
				{
					for (int i = 0; i < this._tipData.LevelList.Count; i++)
					{
						bool flag3 = this._tipData.LevelList[i] > 0;
						if (flag3)
						{
							tempMarkData.Add(new ValueTuple<string, string>(LocalStringManager.Get("LK_Combat_MarkName_Flaw_" + i.ToString()), this._tipData.LevelList[i].ToString()));
						}
					}
				}
				break;
			}
			case EMarkType.Acupoint:
			{
				List<sbyte> levelList = this._tipData.LevelList;
				bool flag4 = levelList == null || levelList.Count <= 0;
				if (!flag4)
				{
					for (int j = 0; j < this._tipData.LevelList.Count; j++)
					{
						bool flag5 = this._tipData.LevelList[j] > 0;
						if (flag5)
						{
							tempMarkData.Add(new ValueTuple<string, string>(LocalStringManager.Get("LK_Combat_MarkName_Acupoint_" + j.ToString()), this._tipData.LevelList[j].ToString()));
						}
					}
				}
				break;
			}
			case EMarkType.Poison:
				tempMarkData.Add(new ValueTuple<string, string>(LocalStringManager.Get(string.Format("LK_Combat_MarkName_Poison{0}_Temp", this._markKey.PoisonType)), this._tipData.NewMarkCount.ToString()));
				tempMarkData.Add(new ValueTuple<string, string>(LocalStringManager.Get(string.Format("LK_Combat_MarkName_Poison{0}_Old", this._markKey.PoisonType)), this._tipData.OldMarkCount.ToString()));
				this.extraHolder.gameObject.SetActive(true);
				CommonUtils.PrepareEnoughChildren(this.extraHolder, this.extraHolder.GetChild(1).gameObject, 0, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
				{
					ExtraItemCount = 1,
					TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems
				}));
				this.SetMarkProperty(this.extraHolder.GetChild(0), LanguageKey.LK_Combat_DefeatMarkTip_Poison_Level_Title.Tr(), LocalStringManager.Get("LK_Combat_DefeatMarkTip_Toxicity_Poison" + ((int)(this._tipData.PoisonLevel - 1)).ToString()));
				break;
			case EMarkType.Mind:
				tempMarkData.Add(new ValueTuple<string, string>(LanguageKey.LK_Combat_MarkName_Mind.Tr(), this._tipData.NewMarkCount.ToString()));
				tempMarkData.Add(new ValueTuple<string, string>(LanguageKey.LK_Combat_MarkName_Mind_Permanent.Tr(), this._tipData.OldMarkCount.ToString()));
				break;
			case EMarkType.Fatal:
				tempMarkData.Add(new ValueTuple<string, string>(LanguageKey.LK_Combat_MarkName_Fatal.Tr(), this._tipData.NewMarkCount.ToString()));
				break;
			case EMarkType.Die:
				tempMarkData.Add(new ValueTuple<string, string>(LanguageKey.LK_Combat_MarkName_Die.Tr(), this._tipData.NewMarkCount.ToString()));
				break;
			case EMarkType.Wug:
			{
				tempMarkData.Add(new ValueTuple<string, string>(LanguageKey.LK_Combat_MarkName_Wug.Tr(), this._tipData.NewMarkCount.ToString()));
				this.extraHolder.gameObject.SetActive(true);
				bool flag6 = this._tipData.WugTemplateIds == null;
				if (flag6)
				{
					this._tipData.WugTemplateIds = new short[0];
				}
				int validWugCount = this._tipData.WugTemplateIds.Count((short id) => id >= 0);
				CommonUtils.PrepareEnoughChildren(this.extraHolder, this.extraHolder.GetChild(1).gameObject, validWugCount, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
				{
					ExtraItemCount = 1,
					TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems
				}));
				this.SetMarkProperty(this.extraHolder.GetChild(0), LanguageKey.LK_ItemSubType_802.Tr() + LanguageKey.LK_Colon_Symbol.Tr(), "");
				int displayIdx = 0;
				for (int k = 0; k < this._tipData.WugTemplateIds.Length; k++)
				{
					short id2 = this._tipData.WugTemplateIds[k];
					bool flag7 = id2 < 0;
					if (!flag7)
					{
						MedicineItem medicineConfig = Medicine.Instance[id2];
						bool flag8 = medicineConfig == null;
						if (!flag8)
						{
							this.SetMarkProperty(this.extraHolder.GetChild(displayIdx + 1), TMPTextSpriteHelper.GetStringWithTextSpriteTag("ui9_icon_mousetip_point_0") + medicineConfig.Name, "");
							displayIdx++;
						}
					}
				}
				break;
			}
			case EMarkType.QiDisorder:
				tempMarkData.Add(new ValueTuple<string, string>(LanguageKey.LK_Combat_MarkName_QiDisorder_Temp.Tr(), this._tipData.NewMarkCount.ToString()));
				tempMarkData.Add(new ValueTuple<string, string>(LanguageKey.LK_Combat_MarkName_QiDisorder_Old.Tr(), this._tipData.OldMarkCount.ToString()));
				this.extraHolder.gameObject.SetActive(true);
				CommonUtils.PrepareEnoughChildren(this.extraHolder, this.extraHolder.GetChild(1).gameObject, 0, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
				{
					ExtraItemCount = 1,
					TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems
				}));
				this.SetMarkProperty(this.extraHolder.GetChild(0), LanguageKey.LK_Combat_DefeatMarkTip_DisorderOfQi_Level_Title.Tr(), LocalStringManager.Get("LK_DisorderOfQi_Level_" + this._tipData.QiDisorderLevel.ToString()));
				break;
			case EMarkType.State:
				tempMarkData.Add(new ValueTuple<string, string>(LanguageKey.LK_Combat_MarkName_State.Tr(), this._tipData.NewMarkCount.ToString()));
				this.extraHolder.gameObject.SetActive(true);
				CommonUtils.PrepareEnoughChildren(this.extraHolder, this.extraHolder.GetChild(1).gameObject, 0, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
				{
					ExtraItemCount = 1,
					TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems
				}));
				this.SetMarkProperty(this.extraHolder.GetChild(0), LanguageKey.LK_Combat_DefeatMarkTip_DisorderOfQi_State_Title.Tr(), string.Format("{0}%", this._tipData.StateBuffPower));
				break;
			case EMarkType.NeiliAllocation:
			{
				tempMarkData.Add(new ValueTuple<string, string>(LanguageKey.LK_Combat_MarkName_NeiliAllocation.Tr(), this._tipData.NewMarkCount.ToString()));
				this.extraHolder.gameObject.SetActive(true);
				List<ENeiliAllocationStatusType> neiliStatusList = this._tipData.NeiliStatusList;
				int num;
				if (neiliStatusList == null)
				{
					num = 0;
				}
				else
				{
					num = neiliStatusList.Count((ENeiliAllocationStatusType x) => x != ENeiliAllocationStatusType.None && x != ENeiliAllocationStatusType.Full);
				}
				int validNeiliCount = num;
				CommonUtils.PrepareEnoughChildren(this.extraHolder, this.extraHolder.GetChild(1).gameObject, validNeiliCount, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
				{
					ExtraItemCount = 1,
					TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems
				}));
				this.SetMarkProperty(this.extraHolder.GetChild(0), LanguageKey.LK_Combat_DefeatMarkTip_DisorderOfQi_State_Title.Tr(), "");
				int count = 0;
				bool flag9 = this._tipData.NeiliStatusList != null;
				if (flag9)
				{
					for (int l = 0; l < 4; l++)
					{
						ENeiliAllocationStatusType type = this._tipData.NeiliStatusList[l];
						bool flag10 = type != ENeiliAllocationStatusType.None && type != ENeiliAllocationStatusType.Full;
						if (flag10)
						{
							Transform child = this.extraHolder.GetChild(count + 1);
							string title = TMPTextSpriteHelper.GetStringWithTextSpriteTag("ui9_icon_mousetip_zhenqi_" + l.ToString()) + LocalStringManager.Get("LK_Combat_MarkName_NeiliAllocation_" + l.ToString());
							string str = "LK_Combat_DefeatMarkTip_Degree_NeiliAllocation_Level_";
							int num2 = (int)type;
							this.SetMarkProperty(child, title, LocalStringManager.Get(str + num2.ToString()));
							count++;
						}
					}
				}
				break;
			}
			case EMarkType.Health:
				tempMarkData.Add(new ValueTuple<string, string>(LanguageKey.LK_Combat_MarkName_Health.Tr(), this._tipData.NewMarkCount.ToString()));
				this.extraHolder.gameObject.SetActive(true);
				CommonUtils.PrepareEnoughChildren(this.extraHolder, this.extraHolder.GetChild(1).gameObject, 0, new CommonUtils.PrepareExtraItemInfo?(new CommonUtils.PrepareExtraItemInfo
				{
					ExtraItemCount = 1,
					TemplateOrder = CommonUtils.EPrepareTemplateOrder.AfterExtraItems
				}));
				this.SetMarkProperty(this.extraHolder.GetChild(0), LanguageKey.LK_Combat_DefeatMarkTip_DisorderOfQi_Level_Title.Tr(), LocalStringManager.Get(CommonUtils.GetHealthString(this._tipData.HealthType)));
				break;
			case EMarkType.Scar:
				tempMarkData.Add(new ValueTuple<string, string>(LanguageKey.LK_Combat_MarkType_Scar.Tr(), this._tipData.NewMarkCount.ToString()));
				break;
			case EMarkType.Tired:
				tempMarkData.Add(new ValueTuple<string, string>(LanguageKey.LK_Combat_MarkType_Tired.Tr(), this._tipData.NewMarkCount.ToString()));
				break;
			}
			this.SetMarkDetail(tempMarkData);
			this.UpdateDetials();
		}

		// Token: 0x06006782 RID: 26498 RVA: 0x002F3BB4 File Offset: 0x002F1DB4
		private void SetMarkDetail([TupleElementNames(new string[]
		{
			"title",
			"content"
		})] List<ValueTuple<string, string>> markDetailList)
		{
			CommonUtils.PrepareEnoughChildren(this.detailHolder, this.detailHolder.GetChild(0).gameObject, markDetailList.Count, null);
			for (int i = 0; i < markDetailList.Count; i++)
			{
				this.SetMarkProperty(this.detailHolder.GetChild(i), markDetailList[i].Item1, markDetailList[i].Item2);
			}
		}

		// Token: 0x06006783 RID: 26499 RVA: 0x002F3C30 File Offset: 0x002F1E30
		private void SetMarkProperty(Transform propertyHolder, string title, string content)
		{
			TextMeshProUGUI titleLabel = propertyHolder.GetChild(0).GetComponent<TextMeshProUGUI>();
			titleLabel.text = title;
			TMPTextSpriteHelper titleLabelSpriteHelper;
			bool flag = titleLabel.TryGetComponent<TMPTextSpriteHelper>(out titleLabelSpriteHelper);
			if (flag)
			{
				titleLabelSpriteHelper.autoClear = false;
				titleLabelSpriteHelper.Parse();
			}
			TextMeshProUGUI contentLabel = propertyHolder.GetChild(1).GetComponent<TextMeshProUGUI>();
			contentLabel.text = content;
		}

		// Token: 0x06006784 RID: 26500 RVA: 0x002F3C84 File Offset: 0x002F1E84
		private void UpdateDetials()
		{
			List<string> tempConditionDetails = new List<string>();
			List<string> tempEffectDetails = new List<string>();
			BodyPartItem bodyPartItem = BodyPart.Instance[this._tipData.SubTypeId];
			string bodyPartName = (bodyPartItem != null) ? bodyPartItem.Name : null;
			switch (this._tipData.MarkType)
			{
			case EMarkType.Outer:
			{
				bool flag = bodyPartName.IsNullOrEmpty();
				if (!flag)
				{
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Outer_0.TrFormat(bodyPartName, bodyPartName)));
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Outer_1.Tr()));
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Outer_2.Tr()));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Outer_0.TrFormat(bodyPartName, bodyPartName, bodyPartName)));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Outer_0.TrFormat(bodyPartName, bodyPartName, bodyPartName)));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Outer_0.TrFormat(bodyPartName, bodyPartName)));
				}
				break;
			}
			case EMarkType.Inner:
			{
				bool flag2 = bodyPartName.IsNullOrEmpty();
				if (!flag2)
				{
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Inner_0.TrFormat(bodyPartName, bodyPartName)));
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Inner_1.Tr()));
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Inner_2.Tr()));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Inner_0.TrFormat(bodyPartName, bodyPartName, bodyPartName)));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Inner_1.TrFormat(bodyPartName, bodyPartName, bodyPartName)));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Inner_2.TrFormat(bodyPartName, bodyPartName)));
				}
				break;
			}
			case EMarkType.Flaw:
			{
				bool flag3 = bodyPartName.IsNullOrEmpty();
				if (!flag3)
				{
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Flaw_0.Tr()));
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Flaw_1.Tr()));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Flaw_0.TrFormat(bodyPartName, bodyPartName)));
				}
				break;
			}
			case EMarkType.Acupoint:
			{
				bool flag4 = bodyPartName.IsNullOrEmpty();
				if (!flag4)
				{
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Acupoint_0.Tr()));
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Acupoint_1.Tr()));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", string.Format(LocalStringManager.Get("LK_Combat_DefeatMarkTip_Effect_Flaw_" + this._tipData.SubTypeId.ToString()), bodyPartName, bodyPartName, bodyPartName)));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Acupoint_7.TrFormat(bodyPartName)));
				}
				break;
			}
			case EMarkType.Poison:
				switch (this._tipData.SubTypeId)
				{
				case 0:
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Poison0_0.Tr()));
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Poison0_1.Tr()));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Poison0_0.TrFormat((int)Poison.Instance[this._tipData.SubTypeId].AffectNeedValue - this._tipData.PoisonTriggerProgress)));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Poison0_1.Tr()));
					break;
				case 1:
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Poison1_0.Tr()));
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Poison1_1.Tr()));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Poison1_0.TrFormat(((int)Poison.Instance[this._tipData.SubTypeId].AffectNeedValue - this._tipData.PoisonTriggerProgress) / 10)));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Poison1_1.Tr()));
					break;
				case 2:
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Poison2_0.Tr()));
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Poison2_1.Tr()));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Poison2_0.TrFormat(((int)Poison.Instance[this._tipData.SubTypeId].AffectNeedValue - this._tipData.PoisonTriggerProgress) * 100 / (int)Poison.Instance[this._tipData.SubTypeId].AffectNeedValue).ToString()));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Poison2_1.Tr()));
					break;
				case 3:
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Poison3_0.Tr()));
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Poison3_1.Tr()));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Poison3_0.TrFormat(((int)Poison.Instance[this._tipData.SubTypeId].AffectNeedValue - this._tipData.PoisonTriggerProgress) * 100 / (int)Poison.Instance[this._tipData.SubTypeId].AffectNeedValue)));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Poison3_1.Tr()));
					break;
				case 4:
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Poison4_0.Tr()));
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Poison4_1.Tr()));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Poison4_0.Tr()));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Poison4_1.Tr()));
					break;
				case 5:
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Poison5_0.Tr()));
					tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Poison5_1.Tr()));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Poison5_0.Tr()));
					tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Poison5_1.Tr()));
					break;
				}
				break;
			case EMarkType.Mind:
				tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Mind_0.Tr()));
				tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Mind_1.Tr()));
				tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Mind_0.Tr()));
				tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Mind_1.Tr()));
				break;
			case EMarkType.Fatal:
				tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Fatal_0.Tr()));
				tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Fatal_1.Tr()));
				tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Fatal_2.Tr()));
				tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Fatal_0.Tr()));
				tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LocalStringManager.Get("LK_Combat_DefeatMarkTip_Effect_Fatal_" + ((int)(this._tipData.CombatType + 1)).ToString())));
				break;
			case EMarkType.Die:
				tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Die_0.Tr()));
				tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Die_0.Tr()));
				tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Die_1.Tr()));
				break;
			case EMarkType.Wug:
				tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Wug_0.Tr()));
				tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Wug_0.Tr()));
				tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Wug_1.Tr()));
				break;
			case EMarkType.QiDisorder:
				tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_QiDisorder_0.Tr()));
				tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_QiDisorder_1.Tr()));
				tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_QiDisorder_0.TrFormat(this._tipData.InjuredRate)));
				tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_QiDisorder_1.TrFormat(this._tipData.NeiliCostInCombat)));
				tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_QiDisorder_2.TrFormat(this._tipData.PoisonResistChange)));
				tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_QiDisorder_3.TrFormat(this._tipData.HealthRecovery)));
				break;
			case EMarkType.State:
				tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_State_0.Tr()));
				tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_State_0.Tr()));
				break;
			case EMarkType.NeiliAllocation:
			{
				tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_NeiliAllocation_0.Tr()));
				StringBuilder sbNeiliAllocation = EasyPool.Get<StringBuilder>();
				StringBuilder sbAllocationType = EasyPool.Get<StringBuilder>();
				for (byte i = 0; i < 5; i += 1)
				{
					ENeiliAllocationStatusType eneiliAllocationStatusType = (ENeiliAllocationStatusType)i;
					bool flag5 = eneiliAllocationStatusType == ENeiliAllocationStatusType.None || eneiliAllocationStatusType == ENeiliAllocationStatusType.Full;
					if (!flag5)
					{
						int count = 0;
						for (int j = 0; j < 4; j++)
						{
							ENeiliAllocationStatusType status = this._tipData.NeiliStatusList[j];
							bool flag6 = status == (ENeiliAllocationStatusType)i;
							if (flag6)
							{
								string neiliAllocation = LocalStringManager.Get(string.Format("LK_Combat_MarkName_NeiliAllocation_{0}", j));
								bool flag7 = count != 0;
								if (flag7)
								{
									sbNeiliAllocation.Append(LanguageKey.LK_Separator.Tr());
								}
								sbNeiliAllocation.Append(neiliAllocation);
								string allocationType = LocalStringManager.Get(string.Format("LK_Neili_Allocation_Type_{0}", j));
								bool flag8 = count != 0;
								if (flag8)
								{
									sbAllocationType.Append(LanguageKey.LK_Separator.Tr());
								}
								sbAllocationType.Append(allocationType);
								count++;
							}
						}
						bool flag9 = count > 0;
						if (flag9)
						{
							tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", string.Format(LocalStringManager.Get(string.Format("LK_Combat_DefeatMarkTip_Effect_NeiliAllocation_{0}", i)), sbNeiliAllocation.ToString(), sbAllocationType.ToString())));
						}
						sbNeiliAllocation.Clear();
						sbAllocationType.Clear();
					}
				}
				EasyPool.Free<StringBuilder>(sbNeiliAllocation);
				EasyPool.Free<StringBuilder>(sbAllocationType);
				break;
			}
			case EMarkType.Health:
				tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Health_0.Tr()));
				break;
			case EMarkType.Scar:
				tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Scar_0.Tr()));
				tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Scar_0.Tr()));
				break;
			case EMarkType.Tired:
				tempConditionDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Condition_Tired_0.Tr()));
				tempEffectDetails.Add(this.GetIconString("ui9_icon_mousetip_point_0", LanguageKey.LK_Combat_DefeatMarkTip_Effect_Tired_0.Tr()));
				break;
			}
			CommonUtils.PrepareEnoughChildren(this.effectHolder, this.effectHolder.GetChild(0).gameObject, tempEffectDetails.Count, null);
			for (int k = 0; k < tempEffectDetails.Count; k++)
			{
				TextMeshProUGUI obj = this.effectHolder.GetChild(k).GetComponent<TextMeshProUGUI>();
				obj.text = tempEffectDetails[k];
				TMPTextSpriteHelper helper;
				bool flag10 = obj.TryGetComponent<TMPTextSpriteHelper>(out helper);
				if (flag10)
				{
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
					{
						helper.autoClear = false;
						helper.Parse();
					});
				}
			}
			CommonUtils.PrepareEnoughChildren(this.conditionHolder, this.conditionHolder.GetChild(0).gameObject, tempConditionDetails.Count, null);
			for (int l = 0; l < tempConditionDetails.Count; l++)
			{
				TextMeshProUGUI obj2 = this.conditionHolder.GetChild(l).GetComponent<TextMeshProUGUI>();
				obj2.text = tempConditionDetails[l];
				TMPTextSpriteHelper helper;
				bool flag11 = obj2.TryGetComponent<TMPTextSpriteHelper>(out helper);
				if (flag11)
				{
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
					{
						helper.autoClear = false;
						helper.Parse();
						this.Refresh();
					});
				}
			}
		}

		// Token: 0x06006785 RID: 26501 RVA: 0x002F4A18 File Offset: 0x002F2C18
		private string GetIconString(string iconName, string content)
		{
			return TMPTextSpriteHelper.GetStringWithTextSpriteTag(iconName) + content;
		}

		// Token: 0x0400490A RID: 18698
		[Header("通用")]
		[SerializeField]
		private TextMeshProUGUI titleLabel;

		// Token: 0x0400490B RID: 18699
		[SerializeField]
		private TextMeshProUGUI typeLabel;

		// Token: 0x0400490C RID: 18700
		[SerializeField]
		private TextMeshProUGUI countLabel;

		// Token: 0x0400490D RID: 18701
		[SerializeField]
		private RectTransform detailHolder;

		// Token: 0x0400490E RID: 18702
		[Header("自定义")]
		[SerializeField]
		private RectTransform extraHolder;

		// Token: 0x0400490F RID: 18703
		[Header("详细信息")]
		[SerializeField]
		private TooltipOperationArea tooltipOperationArea;

		// Token: 0x04004910 RID: 18704
		[SerializeField]
		private RectTransform detailInfoPanel;

		// Token: 0x04004911 RID: 18705
		[SerializeField]
		private RectTransform conditionHolder;

		// Token: 0x04004912 RID: 18706
		[SerializeField]
		private RectTransform effectHolder;

		// Token: 0x04004913 RID: 18707
		private DefeatMarkKey _markKey;

		// Token: 0x04004914 RID: 18708
		private List<DefeatMarkKey> _markKeyList;

		// Token: 0x04004915 RID: 18709
		private int _charId;

		// Token: 0x04004916 RID: 18710
		private MouseTipDefeatMark.DefeatMarkTipData _tipData;

		// Token: 0x04004917 RID: 18711
		public bool IsDetail;

		// Token: 0x02001D74 RID: 7540
		private struct DefeatMarkTipData
		{
			// Token: 0x0600ED50 RID: 60752 RVA: 0x006085B0 File Offset: 0x006067B0
			public void Reset()
			{
				this.MarkType = EMarkType.Invalid;
				this.NewMarkCount = 0;
				this.OldMarkCount = 0;
				this.SubTypeId = -1;
				this.LevelList = null;
				this.PoisonLevel = 0;
				this.PoisonTriggerProgress = 0;
				this.CombatType = CombatType.Play;
				this.NeiliStatusList = null;
				this.QiDisorderLevel = 0;
				this.InjuredRate = 0;
				this.NeiliCostInCombat = 0;
				this.PoisonResistChange = 0;
				this.HealthRecovery = 0;
				this.HealthType = EHealthType.Healthy;
				this.StateBuffPower = 0;
				this.WugTemplateIds = null;
			}

			// Token: 0x0400C630 RID: 50736
			public EMarkType MarkType;

			// Token: 0x0400C631 RID: 50737
			public int NewMarkCount;

			// Token: 0x0400C632 RID: 50738
			public int OldMarkCount;

			// Token: 0x0400C633 RID: 50739
			public sbyte SubTypeId;

			// Token: 0x0400C634 RID: 50740
			public List<sbyte> LevelList;

			// Token: 0x0400C635 RID: 50741
			public sbyte PoisonLevel;

			// Token: 0x0400C636 RID: 50742
			public int PoisonTriggerProgress;

			// Token: 0x0400C637 RID: 50743
			public CombatType CombatType;

			// Token: 0x0400C638 RID: 50744
			public List<ENeiliAllocationStatusType> NeiliStatusList;

			// Token: 0x0400C639 RID: 50745
			public sbyte QiDisorderLevel;

			// Token: 0x0400C63A RID: 50746
			public int InjuredRate;

			// Token: 0x0400C63B RID: 50747
			public int NeiliCostInCombat;

			// Token: 0x0400C63C RID: 50748
			public int PoisonResistChange;

			// Token: 0x0400C63D RID: 50749
			public int HealthRecovery;

			// Token: 0x0400C63E RID: 50750
			public EHealthType HealthType;

			// Token: 0x0400C63F RID: 50751
			public int StateBuffPower;

			// Token: 0x0400C640 RID: 50752
			public short[] WugTemplateIds;
		}
	}
}
