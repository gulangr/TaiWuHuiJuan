using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Extra;
using GameData.Domains.Taiwu.Profession;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;

// Token: 0x0200013E RID: 318
public class ProfessionModel : ISingletonInit, IDisposable
{
	// Token: 0x170001DA RID: 474
	// (get) Token: 0x060010D2 RID: 4306 RVA: 0x0006444F File Offset: 0x0006264F
	// (set) Token: 0x060010D3 RID: 4307 RVA: 0x00064457 File Offset: 0x00062657
	[Obsolete]
	public int TaiwuCurrProfessionId { get; private set; }

	// Token: 0x170001DB RID: 475
	// (get) Token: 0x060010D4 RID: 4308 RVA: 0x00064460 File Offset: 0x00062660
	public Dictionary<int, ProfessionData> TaiwuProfessions
	{
		get
		{
			return this._taiwuProfessions;
		}
	}

	// Token: 0x060010D5 RID: 4309 RVA: 0x00064468 File Offset: 0x00062668
	public void Init()
	{
		this._listenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		GameDataBridge.AddDataMonitor(this._listenerId, 19, 146, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 19, 147, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataMonitor(this._listenerId, 19, 162, ulong.MaxValue, uint.MaxValue);
	}

	// Token: 0x060010D6 RID: 4310 RVA: 0x000644D0 File Offset: 0x000626D0
	public void Dispose()
	{
		GameDataBridge.AddDataUnMonitor(this._listenerId, 19, 146, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 19, 147, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.AddDataUnMonitor(this._listenerId, 19, 162, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.UnregisterListener(this._listenerId);
		this._listenerId = -1;
	}

	// Token: 0x060010D7 RID: 4311 RVA: 0x00064534 File Offset: 0x00062734
	private void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			byte type = notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b != 1)
				{
				}
			}
			else
			{
				DataUid uid = notification.Uid;
				bool flag = uid.DomainId == 19;
				if (flag)
				{
					this.UpdateGameData(uid, notification.ValueOffset, wrapper.DataPool);
				}
			}
		}
	}

	// Token: 0x060010D8 RID: 4312 RVA: 0x000645D4 File Offset: 0x000627D4
	private void UpdateGameData(DataUid uid, int valueOffset, RawDataPool dataPool)
	{
		ushort dataId = uid.DataId;
		ushort num = dataId;
		if (num != 146)
		{
			if (num != 147)
			{
				if (num == 162)
				{
					Serializer.Deserialize(dataPool, valueOffset, ref this.IsExtraProfessionSkillUnlocked);
				}
			}
			else
			{
				Serializer.Deserialize(dataPool, valueOffset, ref this._taiwuProfessionSkillSlots);
				GEvent.OnEvent(UiEvents.OnProfessionSlotsChange, null);
			}
		}
		else
		{
			Serializer.DeserializeModifications<int>(dataPool, valueOffset, this._taiwuProfessions);
			HashSet<int> changeKetSet = new HashSet<int>();
			CommonUtils.GetModifiedSingleValueCollectionKeyOfClass<int, ProfessionData>(dataPool, valueOffset, changeKetSet);
			foreach (int key in changeKetSet)
			{
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				box.Set("ProfessionId", key);
				GEvent.OnEvent(UiEvents.OnProfessionDataChange, box);
			}
		}
	}

	// Token: 0x060010D9 RID: 4313 RVA: 0x000646CC File Offset: 0x000628CC
	public ProfessionData GetProfessionData(int professionId)
	{
		ProfessionData data;
		this._taiwuProfessions.TryGetValue(professionId, out data);
		return data;
	}

	// Token: 0x060010DA RID: 4314 RVA: 0x000646F0 File Offset: 0x000628F0
	public bool TryGetProfessionData(int professionId, out ProfessionData data)
	{
		return this._taiwuProfessions.TryGetValue(professionId, out data);
	}

	// Token: 0x060010DB RID: 4315 RVA: 0x00064710 File Offset: 0x00062910
	public bool IsProfessionalSkillUnlockedAndEquipped(int skillTemplateId)
	{
		ProfessionSkillItem skillCfg = ProfessionSkill.Instance[skillTemplateId];
		ProfessionData professionData = this._taiwuProfessions[skillCfg.Profession];
		int skillIndex = (int)(skillCfg.Level - 1);
		bool flag = !professionData.IsSkillUnlocked(skillIndex);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = SingletonObject.getInstance<BasicGameData>().ChallengeModeData.IsEnabled(EChallengeModeImplement.ProfessionSkillRequiresExp);
			if (flag2)
			{
				bool flag3 = !professionData.IsSkillLearned(skillIndex);
				if (flag3)
				{
					return false;
				}
			}
			result = this._taiwuProfessionSkillSlots.IsEquipped(skillTemplateId);
		}
		return result;
	}

	// Token: 0x060010DC RID: 4316 RVA: 0x00064798 File Offset: 0x00062998
	public bool IsProfessionalSkillUnlocked(int skillTemplateId)
	{
		ProfessionSkillItem skillCfg = ProfessionSkill.Instance[skillTemplateId];
		ProfessionData professionData = this._taiwuProfessions[skillCfg.Profession];
		int skillIndex = (int)(skillCfg.Level - 1);
		bool flag = !professionData.IsSkillUnlocked(skillIndex);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			bool flag2 = SingletonObject.getInstance<BasicGameData>().ChallengeModeData.IsEnabled(EChallengeModeImplement.ProfessionSkillRequiresExp);
			if (flag2)
			{
				bool flag3 = !professionData.IsSkillLearned(skillIndex);
				if (flag3)
				{
					return false;
				}
			}
			result = true;
		}
		return result;
	}

	// Token: 0x060010DD RID: 4317 RVA: 0x00064818 File Offset: 0x00062A18
	public bool IsProfessionalSkillPendingLearn(int professionId, int skillIndex)
	{
		bool flag = !SingletonObject.getInstance<BasicGameData>().ChallengeModeData.IsEnabled(EChallengeModeImplement.ProfessionSkillRequiresExp);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			ProfessionData professionData = this.GetProfessionData(professionId);
			bool flag2 = professionData == null;
			result = (!flag2 && professionData.IsSkillUnlocked(skillIndex) && !professionData.IsSkillLearned(skillIndex));
		}
		return result;
	}

	// Token: 0x060010DE RID: 4318 RVA: 0x00064870 File Offset: 0x00062A70
	public int GetProfessionSkillLearnExpCost()
	{
		bool flag = !SingletonObject.getInstance<BasicGameData>().ChallengeModeData.IsEnabled(EChallengeModeImplement.ProfessionSkillRequiresExp);
		int result;
		if (flag)
		{
			result = 0;
		}
		else
		{
			int[] scores = GlobalConfig.Instance.ChallengeProfessionSkillLearnScores;
			int totalScore = 0;
			foreach (KeyValuePair<int, ProfessionData> kvp in this._taiwuProfessions)
			{
				ProfessionData professionData = kvp.Value;
				int skillCount = professionData.GetSkillCount();
				int i = 0;
				while (i < skillCount && i < scores.Length)
				{
					bool flag2 = professionData.IsSkillLearned(i);
					if (flag2)
					{
						totalScore += scores[i];
					}
					i++;
				}
			}
			int multiplier = GlobalConfig.Instance.ChallengeProfessionSkillLearnExpMultiplier;
			result = totalScore * multiplier;
		}
		return result;
	}

	// Token: 0x060010DF RID: 4319 RVA: 0x00064950 File Offset: 0x00062B50
	public void UpdateSpecialProfessionSkill()
	{
		ProfessionModel.<>c__DisplayClass23_0 CS$<>8__locals1 = new ProfessionModel.<>c__DisplayClass23_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.index = 0;
		CS$<>8__locals1.<UpdateSpecialProfessionSkill>g__UpdateTargetSpecialProfessionSkill|0();
	}

	// Token: 0x060010E0 RID: 4320 RVA: 0x0006497C File Offset: 0x00062B7C
	public bool IsSpecialProfessionSkillActive(int skillId)
	{
		bool flag;
		this._specialProfessionSkillConditionCache.TryGetValue(skillId, out flag);
		return flag;
	}

	// Token: 0x060010E1 RID: 4321 RVA: 0x000649A0 File Offset: 0x00062BA0
	public void HandleSetGiveNameResult(string givenName, FullName fullName)
	{
		string surName = string.Empty;
		int surNameId = fullName.GetCustomSurnameId();
		bool flag = surNameId >= 0;
		if (flag)
		{
			SingletonObject.getInstance<BasicGameData>().CustomTexts.TryGetValue(surNameId, out surName);
		}
		bool flag2 = string.IsNullOrEmpty(surName);
		if (flag2)
		{
			surNameId = (int)fullName.GetSurnameId();
			bool flag3 = surNameId > 0 && LocalSurnames.Instance.SurnameCore.CheckIndex(surNameId);
			if (flag3)
			{
				surName = LocalSurnames.Instance.SurnameCore[surNameId].Surname;
			}
		}
		string checkString = surName + givenName;
		bool flag4 = string.IsNullOrEmpty(checkString);
		if (!flag4)
		{
			List<SensitiveWordsMatchResult> resultList = SensitiveWordsSystem.Instance.TryMatch(checkString, 10);
			int sensitiveWordsType = -1;
			foreach (SensitiveWordsMatchResult result in resultList)
			{
				bool flag5 = result.Type == ESensitiveWordsType.Politics;
				if (flag5)
				{
					sensitiveWordsType = 12;
					break;
				}
				bool flag6 = result.Type == ESensitiveWordsType.Abuse;
				if (flag6)
				{
					sensitiveWordsType = 6;
					break;
				}
				sensitiveWordsType = (int)result.Type;
			}
			TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("InputActionComplete", "ConchShip_PresetKey_SetGivenNameMatchSensitive", sensitiveWordsType);
			sbyte matchGender = -1;
			bool flag7 = givenName.Length == 2;
			if (flag7)
			{
				string midChar = givenName.Substring(0, 1);
				HanNameItem item = Array.Find<HanNameItem>(LocalNames.Instance.AllNamesCore, (HanNameItem e) => e != null && e.MiddleChar == midChar);
				bool flag8 = item != null;
				if (flag8)
				{
					string tailChar = givenName.Substring(1, 1);
					bool flag9 = item.SerialNeutral != null && Array.IndexOf<string>(item.SerialNeutral, tailChar) >= 0;
					if (flag9)
					{
						matchGender = 2;
					}
					bool flag10 = item.SerialMan != null && Array.IndexOf<string>(item.SerialMan, tailChar) >= 0;
					if (flag10)
					{
						matchGender = 1;
					}
					else
					{
						bool flag11 = item.SerialWoman != null && Array.IndexOf<string>(item.SerialWoman, tailChar) >= 0;
						if (flag11)
						{
							matchGender = 0;
						}
					}
				}
			}
			else
			{
				bool flag12 = givenName.Length == 1;
				if (flag12)
				{
					int i = 0;
					int max = LocalNames.Instance.AllNamesCore.Length;
					while (i < max)
					{
						HanNameItem item2 = LocalNames.Instance.AllNamesCore[i];
						bool flag13 = item2 == null;
						if (!flag13)
						{
							bool flag14 = item2.ApartNeutral != null && Array.IndexOf<string>(item2.ApartNeutral, givenName) >= 0;
							if (flag14)
							{
								matchGender = 2;
							}
							bool flag15 = item2.ApartMan != null && Array.IndexOf<string>(item2.ApartMan, givenName) >= 0;
							if (flag15)
							{
								matchGender = 1;
							}
							else
							{
								bool flag16 = item2.ApartWoman != null && Array.IndexOf<string>(item2.ApartWoman, givenName) >= 0;
								if (flag16)
								{
									matchGender = 0;
								}
							}
						}
						i++;
					}
				}
			}
			TaiwuEventDomainMethod.Call.SetListenerEventActionIntArg("InputActionComplete", "ConchShip_PresetKey_SetGivenNameMatchSystemRuleType", (int)matchGender);
		}
	}

	// Token: 0x060010E2 RID: 4322 RVA: 0x00064CA8 File Offset: 0x00062EA8
	public bool IsExtraProfessionSkillReady()
	{
		return this._lastTimeCastExtraSkill != SingletonObject.getInstance<BasicGameData>().CurrDate;
	}

	// Token: 0x060010E3 RID: 4323 RVA: 0x00064CCF File Offset: 0x00062ECF
	public void SetLastTimeCastExtraSkill()
	{
		this._lastTimeCastExtraSkill = SingletonObject.getInstance<BasicGameData>().CurrDate;
	}

	// Token: 0x060010E4 RID: 4324 RVA: 0x00064CE4 File Offset: 0x00062EE4
	public void TryOpenProfessionSkillUnlockedInProfessionView()
	{
		bool ignoreProfessionSkillUnlockAnimation = GMFunc.IgnoreProfessionSkillUnlockAnimation;
		if (!ignoreProfessionSkillUnlockAnimation)
		{
			bool flag = !UIManager.Instance.IsFocusElement(UIElement.Profession);
			if (!flag)
			{
				bool flag2 = !UI_ProfessionSkillUnlocked.HasProfessionSkillUnlockedToShow();
				if (!flag2)
				{
					ExtraDomainMethod.AsyncCall.CanShowProfessionSkillUnlocked(null, delegate(int offset, RawDataPool dataPool)
					{
						bool canShow = false;
						Serializer.Deserialize(dataPool, offset, ref canShow);
						bool flag3 = !canShow;
						if (!flag3)
						{
							bool flag4 = !UIManager.Instance.IsFocusElement(UIElement.Profession);
							if (!flag4)
							{
								bool exist = UIElement.ProfessionSkillUnlocked.Exist;
								if (!exist)
								{
									UIManager.Instance.MaskUI(UIElement.ProfessionSkillUnlocked);
								}
							}
						}
					});
				}
			}
		}
	}

	// Token: 0x060010E5 RID: 4325 RVA: 0x00064D4C File Offset: 0x00062F4C
	public void ModifyOneEquipSlot(int level, int index, int skillId)
	{
		IntList slots = this._taiwuProfessionSkillSlots.Slots[level];
		slots.Items[index] = skillId;
		this.SaveEquipSkillSlots();
	}

	// Token: 0x060010E6 RID: 4326 RVA: 0x00064D84 File Offset: 0x00062F84
	public void RemoveSkillIdFromSlot(int skillId)
	{
		for (int i = 0; i < this._taiwuProfessionSkillSlots.Slots.Length; i++)
		{
			IntList slot = this._taiwuProfessionSkillSlots.Slots[i];
			for (int j = 0; j < slot.Items.Count; j++)
			{
				bool flag = slot.Items[j] == skillId;
				if (flag)
				{
					slot.Items[j] = -1;
					this.SaveEquipSkillSlots();
					return;
				}
			}
		}
	}

	// Token: 0x060010E7 RID: 4327 RVA: 0x00064E10 File Offset: 0x00063010
	public int GetProfessionSkillFromSlot(int level, int index)
	{
		bool flag = this._taiwuProfessionSkillSlots == null;
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			result = this._taiwuProfessionSkillSlots.Slots[level].Items[index];
		}
		return result;
	}

	// Token: 0x060010E8 RID: 4328 RVA: 0x00064E50 File Offset: 0x00063050
	public bool HasEmptyEquipSlotInSameLevel(int skillId)
	{
		ProfessionSkillItem skillConfig = ProfessionSkill.Instance[skillId];
		int level = (int)(skillConfig.Level - 1);
		for (int i = 0; i < this._taiwuProfessionSkillSlots.Slots[level].Items.Count; i++)
		{
			bool flag = this._taiwuProfessionSkillSlots.Slots[level].Items[i] == -1;
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060010E9 RID: 4329 RVA: 0x00064ED4 File Offset: 0x000630D4
	public ProfessionModel.SlotIndex FindEmptyEquipSlotInSameLevelForEquip(int skillId)
	{
		bool flag = this.IsSkillEquipped(skillId);
		ProfessionModel.SlotIndex invalid;
		if (flag)
		{
			invalid = ProfessionModel.SlotIndex.Invalid;
		}
		else
		{
			ProfessionSkillItem skillConfig = ProfessionSkill.Instance[skillId];
			int level = (int)(skillConfig.Level - 1);
			for (int i = 0; i < this._taiwuProfessionSkillSlots.Slots[level].Items.Count; i++)
			{
				bool flag2 = this._taiwuProfessionSkillSlots.Slots[level].Items[i] == -1;
				if (flag2)
				{
					return new ProfessionModel.SlotIndex(level, i);
				}
			}
			invalid = ProfessionModel.SlotIndex.Invalid;
		}
		return invalid;
	}

	// Token: 0x060010EA RID: 4330 RVA: 0x00064F7C File Offset: 0x0006317C
	public bool IsSkillEquipped(int skillId)
	{
		bool flag = this._taiwuProfessionSkillSlots == null;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			for (int i = 0; i < this._taiwuProfessionSkillSlots.Slots.Length; i++)
			{
				IntList slot = this._taiwuProfessionSkillSlots.Slots[i];
				bool flag2 = slot.Items.Contains(skillId);
				if (flag2)
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x060010EB RID: 4331 RVA: 0x00064FE8 File Offset: 0x000631E8
	public int CountFullSlotLevels()
	{
		int count = 0;
		for (int i = 0; i < this._taiwuProfessionSkillSlots.Slots.Length; i++)
		{
			IntList slot = this._taiwuProfessionSkillSlots.Slots[i];
			bool flag = slot.Items.Contains(-1);
			if (!flag)
			{
				count++;
			}
		}
		return count;
	}

	// Token: 0x060010EC RID: 4332 RVA: 0x00065048 File Offset: 0x00063248
	public List<int> GetProfessionIdsFromSlots()
	{
		List<int> professionIds = EasyPool.Get<List<int>>();
		professionIds.Clear();
		for (int i = 0; i < this._taiwuProfessionSkillSlots.Slots.Length; i++)
		{
			IntList slot = this._taiwuProfessionSkillSlots.Slots[i];
			for (int j = 0; j < slot.Items.Count; j++)
			{
				int skillId = slot.Items[j];
				bool flag = skillId != -1;
				if (flag)
				{
					int professionId = ProfessionSkill.Instance[skillId].Profession;
					bool flag2 = !professionIds.Contains(professionId);
					if (flag2)
					{
						professionIds.Add(professionId);
					}
				}
			}
		}
		return professionIds;
	}

	// Token: 0x060010ED RID: 4333 RVA: 0x0006510C File Offset: 0x0006330C
	[return: TupleElementNames(new string[]
	{
		"professionData",
		"skillIndex"
	})]
	public ValueTuple<ProfessionData, int> FindProfessionDataBySkillId(int skillId)
	{
		bool flag = skillId == -1;
		ValueTuple<ProfessionData, int> result;
		if (flag)
		{
			result = new ValueTuple<ProfessionData, int>(null, -1);
		}
		else
		{
			ProfessionSkillItem skillConfig = ProfessionSkill.Instance[skillId];
			int professionId = skillConfig.Profession;
			ProfessionItem professionConfig = Profession.Instance[professionId];
			int skillIndex = (professionConfig.ExtraProfessionSkill == skillId) ? 3 : professionConfig.ProfessionSkills.IndexOf(skillId);
			ProfessionData professionData;
			bool flag2 = this.TryGetProfessionData(professionId, out professionData);
			if (flag2)
			{
				result = new ValueTuple<ProfessionData, int>(professionData, skillIndex);
			}
			else
			{
				result = new ValueTuple<ProfessionData, int>(null, skillIndex);
			}
		}
		return result;
	}

	// Token: 0x060010EE RID: 4334 RVA: 0x0006518F File Offset: 0x0006338F
	private void SaveEquipSkillSlots()
	{
		ExtraDomainMethod.Call.ConfirmProfessionSkillsEquipment(this._taiwuProfessionSkillSlots);
	}

	// Token: 0x04000EEC RID: 3820
	private int _listenerId = -1;

	// Token: 0x04000EEE RID: 3822
	public bool IsExtraProfessionSkillUnlocked;

	// Token: 0x04000EEF RID: 3823
	private int _lastTimeCastExtraSkill = 0;

	// Token: 0x04000EF0 RID: 3824
	private readonly Dictionary<int, ProfessionData> _taiwuProfessions = new Dictionary<int, ProfessionData>();

	// Token: 0x04000EF1 RID: 3825
	private TaiwuProfessionSkillSlots _taiwuProfessionSkillSlots;

	// Token: 0x04000EF2 RID: 3826
	private Dictionary<int, bool> _specialProfessionSkillConditionCache = new Dictionary<int, bool>();

	// Token: 0x04000EF3 RID: 3827
	private readonly List<ValueTuple<int, int>> _specialProfessionSkillIdList = new List<ValueTuple<int, int>>
	{
		new ValueTuple<int, int>(6, 27),
		new ValueTuple<int, int>(12, 51),
		new ValueTuple<int, int>(5, 23),
		new ValueTuple<int, int>(14, 59)
	};

	// Token: 0x020011F3 RID: 4595
	public struct SlotIndex
	{
		// Token: 0x0600C45A RID: 50266 RVA: 0x0057B514 File Offset: 0x00579714
		public SlotIndex(int level, int index)
		{
			this.Level = level;
			this.Index = index;
		}

		// Token: 0x17001609 RID: 5641
		// (get) Token: 0x0600C45B RID: 50267 RVA: 0x0057B525 File Offset: 0x00579725
		public bool IsValid
		{
			get
			{
				return this.Level >= 0 && this.Index >= 0;
			}
		}

		// Token: 0x1700160A RID: 5642
		// (get) Token: 0x0600C45C RID: 50268 RVA: 0x0057B53F File Offset: 0x0057973F
		public static ProfessionModel.SlotIndex Invalid
		{
			get
			{
				return new ProfessionModel.SlotIndex(-1, -1);
			}
		}

		// Token: 0x040098DE RID: 39134
		public int Level;

		// Token: 0x040098DF RID: 39135
		public int Index;
	}
}
