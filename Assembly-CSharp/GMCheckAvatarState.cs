using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.AvatarSystem.AvatarRes;
using GameData.Domains.Character.Display;
using GameData.Serializer;

// Token: 0x0200020D RID: 525
public static class GMCheckAvatarState
{
	// Token: 0x14000015 RID: 21
	// (add) Token: 0x0600214C RID: 8524 RVA: 0x000F2430 File Offset: 0x000F0630
	// (remove) Token: 0x0600214D RID: 8525 RVA: 0x000F2464 File Offset: 0x000F0664
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action StateChanged;

	// Token: 0x17000359 RID: 857
	// (get) Token: 0x0600214E RID: 8526 RVA: 0x000F2497 File Offset: 0x000F0697
	// (set) Token: 0x0600214F RID: 8527 RVA: 0x000F249E File Offset: 0x000F069E
	public static bool Enabled { get; private set; }

	// Token: 0x1700035A RID: 858
	// (get) Token: 0x06002150 RID: 8528 RVA: 0x000F24A6 File Offset: 0x000F06A6
	// (set) Token: 0x06002151 RID: 8529 RVA: 0x000F24AD File Offset: 0x000F06AD
	public static int CurrentCharacterId { get; private set; } = -1;

	// Token: 0x1700035B RID: 859
	// (get) Token: 0x06002152 RID: 8530 RVA: 0x000F24B5 File Offset: 0x000F06B5
	public static bool HasCurrentCharacter
	{
		get
		{
			return GMCheckAvatarState.CurrentCharacterId >= 0 && GMCheckAvatarState._currentAvatarRelatedData != null;
		}
	}

	// Token: 0x06002153 RID: 8531 RVA: 0x000F24CA File Offset: 0x000F06CA
	public static IReadOnlyList<GMCheckAvatarLineDefinition> GetLineDefinitions()
	{
		return GMCheckAvatarState.LineDefinitions;
	}

	// Token: 0x06002154 RID: 8532 RVA: 0x000F24D4 File Offset: 0x000F06D4
	public static GMCheckAvatarLineDefinition GetLineDefinition(GMCheckAvatarComponentType componentType)
	{
		return GMCheckAvatarState.DefinitionMap[componentType];
	}

	// Token: 0x06002155 RID: 8533 RVA: 0x000F24F1 File Offset: 0x000F06F1
	public static void Reset()
	{
		GMCheckAvatarState.Enabled = false;
		GMCheckAvatarState.CurrentCharacterId = -1;
		GMCheckAvatarState._currentAvatarRelatedData = null;
		GMCheckAvatarState.NotifyStateChanged();
	}

	// Token: 0x06002156 RID: 8534 RVA: 0x000F2510 File Offset: 0x000F0710
	public static void SetEnabled(bool enabled)
	{
		bool flag = GMCheckAvatarState.Enabled == enabled;
		if (!flag)
		{
			GMCheckAvatarState.Enabled = enabled;
			GMCheckAvatarState.NotifyStateChanged();
		}
	}

	// Token: 0x06002157 RID: 8535 RVA: 0x000F253C File Offset: 0x000F073C
	public static void ClearCurrentCharacter()
	{
		bool flag = !GMCheckAvatarState.HasCurrentCharacter;
		if (!flag)
		{
			GMCheckAvatarState.CurrentCharacterId = -1;
			GMCheckAvatarState._currentAvatarRelatedData = null;
			GMCheckAvatarState.NotifyStateChanged();
		}
	}

	// Token: 0x06002158 RID: 8536 RVA: 0x000F256C File Offset: 0x000F076C
	public static void UpdateCurrentCharacter(int charId, AvatarRelatedData avatarRelatedData)
	{
		bool flag = !GMCheckAvatarState.Enabled || charId < 0 || avatarRelatedData == null || avatarRelatedData.AvatarData == null;
		if (!flag)
		{
			bool shouldReset = GMCheckAvatarState.CurrentCharacterId != charId || GMCheckAvatarState._currentAvatarRelatedData == null || GMCheckAvatarState._currentAvatarRelatedData.AvatarData == null || GMCheckAvatarState._currentAvatarRelatedData.AvatarData.AvatarId != avatarRelatedData.AvatarData.AvatarId;
			bool flag2 = shouldReset;
			if (flag2)
			{
				GMCheckAvatarState.CurrentCharacterId = charId;
				GMCheckAvatarState._currentAvatarRelatedData = new AvatarRelatedData(avatarRelatedData);
				GMCheckAvatarState.NotifyStateChanged();
			}
			else
			{
				GMCheckAvatarState._currentAvatarRelatedData.DisplayAge = avatarRelatedData.DisplayAge;
				GMCheckAvatarState._currentAvatarRelatedData.HasNewGoods = avatarRelatedData.HasNewGoods;
			}
		}
	}

	// Token: 0x06002159 RID: 8537 RVA: 0x000F2618 File Offset: 0x000F0818
	public static AvatarRelatedData GetCurrentAvatarRelatedDataCopy()
	{
		return GMCheckAvatarState.HasCurrentCharacter ? new AvatarRelatedData(GMCheckAvatarState._currentAvatarRelatedData) : null;
	}

	// Token: 0x0600215A RID: 8538 RVA: 0x000F2640 File Offset: 0x000F0840
	public static bool TryStepComponent(GMCheckAvatarComponentType componentType, int direction)
	{
		bool flag = !GMCheckAvatarState.HasCurrentCharacter;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			AvatarData avatarData = GMCheckAvatarState._currentAvatarRelatedData.AvatarData;
			if (!true)
			{
			}
			bool flag2;
			switch (componentType)
			{
			case GMCheckAvatarComponentType.Clothing:
				flag2 = GMCheckAvatarState.TryStepClothing(direction);
				break;
			case GMCheckAvatarComponentType.FrontHair:
				flag2 = GMCheckAvatarState.TryStepId((from item in GMCheckAvatarState.GetCurrentAvatarGroup().Hair1Res
				select item.Id).ToList<short>(), ref avatarData.FrontHairId, direction, new Action(GMCheckAvatarState.ShowHair));
				break;
			case GMCheckAvatarComponentType.BackHair:
				flag2 = GMCheckAvatarState.TryStepId((from item in GMCheckAvatarState.GetCurrentAvatarGroup().Hair2Res
				select item.Id).ToList<short>(), ref avatarData.BackHairId, direction, new Action(GMCheckAvatarState.ShowHair));
				break;
			case GMCheckAvatarComponentType.Eyebrow:
				flag2 = GMCheckAvatarState.TryStepId((from item in GMCheckAvatarState.GetCurrentAvatarGroup().EyeBrowRes
				select item.Id).ToList<short>(), ref avatarData.EyebrowId, direction, new Action(GMCheckAvatarState.ShowEyebrow));
				break;
			case GMCheckAvatarComponentType.Eyes:
				flag2 = GMCheckAvatarState.TryStepEyes(direction);
				break;
			case GMCheckAvatarComponentType.Nose:
				flag2 = GMCheckAvatarState.TryStepId((from item in GMCheckAvatarState.GetCurrentAvatarGroup().NoseRes
				select item.Id).ToList<short>(), ref avatarData.NoseId, direction, null);
				break;
			case GMCheckAvatarComponentType.Mouth:
				flag2 = GMCheckAvatarState.TryStepId((from item in GMCheckAvatarState.GetCurrentAvatarGroup().MouthRes
				select item.Id).ToList<short>(), ref avatarData.MouthId, direction, null);
				break;
			case GMCheckAvatarComponentType.Beard1:
				flag2 = GMCheckAvatarState.TryStepId((from item in GMCheckAvatarState.GetBeardResList(true)
				select item.Id).ToList<short>(), ref avatarData.Beard1Id, direction, new Action(GMCheckAvatarState.ShowBeard1));
				break;
			case GMCheckAvatarComponentType.Beard2:
				flag2 = GMCheckAvatarState.TryStepId((from item in GMCheckAvatarState.GetBeardResList(false)
				select item.Id).ToList<short>(), ref avatarData.Beard2Id, direction, new Action(GMCheckAvatarState.ShowBeard2));
				break;
			case GMCheckAvatarComponentType.Feature1:
				flag2 = GMCheckAvatarState.TryStepId((from item in AvatarGroup.GetFeatureResExcludeDelete(GMCheckAvatarState.GetCurrentAvatarGroup().Feature1Res)
				where item.Id != 1
				select item.Id).ToList<short>(), ref avatarData.Feature1Id, direction, null);
				break;
			case GMCheckAvatarComponentType.Feature2:
				flag2 = GMCheckAvatarState.TryStepId((from item in AvatarGroup.GetFeatureResExcludeDelete(GMCheckAvatarState.GetCurrentAvatarGroup().Feature2Res)
				where item.Id != 1
				select item.Id).ToList<short>(), ref avatarData.Feature2Id, direction, null);
				break;
			case GMCheckAvatarComponentType.Wrinkle1:
				flag2 = GMCheckAvatarState.TryStepId((from item in GMCheckAvatarState.GetCurrentAvatarGroup().Wrinkle1Res
				select item.Id).ToList<short>(), ref avatarData.Wrinkle1Id, direction, new Action(GMCheckAvatarState.ShowWrinkle1));
				break;
			case GMCheckAvatarComponentType.Wrinkle2:
				flag2 = GMCheckAvatarState.TryStepId((from item in GMCheckAvatarState.GetCurrentAvatarGroup().Wrinkle2Res
				select item.Id).ToList<short>(), ref avatarData.Wrinkle2Id, direction, new Action(GMCheckAvatarState.ShowWrinkle2));
				break;
			case GMCheckAvatarComponentType.Wrinkle3:
				flag2 = GMCheckAvatarState.TryStepId((from item in GMCheckAvatarState.GetCurrentAvatarGroup().Wrinkle3Res
				select item.Id).ToList<short>(), ref avatarData.Wrinkle3Id, direction, new Action(GMCheckAvatarState.ShowWrinkle3));
				break;
			default:
				flag2 = false;
				break;
			}
			if (!true)
			{
			}
			bool changed = flag2;
			bool flag3 = !changed;
			if (flag3)
			{
				result = false;
			}
			else
			{
				avatarData.FormatDisabledElements();
				GMCheckAvatarState.NotifyStateChanged();
				result = true;
			}
		}
		return result;
	}

	// Token: 0x0600215B RID: 8539 RVA: 0x000F2AEC File Offset: 0x000F0CEC
	public static bool TryClearComponent(GMCheckAvatarComponentType componentType)
	{
		bool flag = !GMCheckAvatarState.HasCurrentCharacter;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			AvatarData avatarData = GMCheckAvatarState._currentAvatarRelatedData.AvatarData;
			if (!true)
			{
			}
			bool flag2;
			switch (componentType)
			{
			case GMCheckAvatarComponentType.Beard1:
				flag2 = GMCheckAvatarState.HideGrowableElement(1);
				break;
			case GMCheckAvatarComponentType.Beard2:
				flag2 = GMCheckAvatarState.HideGrowableElement(2);
				break;
			case GMCheckAvatarComponentType.Feature1:
				flag2 = GMCheckAvatarState.TrySetDirectValue(ref avatarData.Feature1Id, 1);
				break;
			case GMCheckAvatarComponentType.Feature2:
				flag2 = GMCheckAvatarState.TrySetDirectValue(ref avatarData.Feature2Id, 1);
				break;
			case GMCheckAvatarComponentType.Wrinkle1:
				flag2 = GMCheckAvatarState.HideGrowableElement(3);
				break;
			case GMCheckAvatarComponentType.Wrinkle2:
				flag2 = GMCheckAvatarState.HideGrowableElement(4);
				break;
			case GMCheckAvatarComponentType.Wrinkle3:
				flag2 = GMCheckAvatarState.HideGrowableElement(5);
				break;
			default:
				flag2 = false;
				break;
			}
			if (!true)
			{
			}
			bool changed = flag2;
			bool flag3 = !changed;
			if (flag3)
			{
				result = false;
			}
			else
			{
				GMCheckAvatarState.NotifyStateChanged();
				result = true;
			}
		}
		return result;
	}

	// Token: 0x0600215C RID: 8540 RVA: 0x000F2BBC File Offset: 0x000F0DBC
	public static string BuildCurrentAvatarLog()
	{
		bool flag = !GMCheckAvatarState.HasCurrentCharacter;
		string result;
		if (flag)
		{
			result = LocalStringManager.Get("GM_CheckAvatar_Target_None");
		}
		else
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("charId: ").Append(GMCheckAvatarState.CurrentCharacterId).Append('\n');
			sb.Append("displayAge: ").Append(GMCheckAvatarState._currentAvatarRelatedData.DisplayAge).Append('\n');
			sb.Append("clothingDisplayId: ").Append(GMCheckAvatarState._currentAvatarRelatedData.ClothingDisplayId).Append('\n');
			FieldInfo[] fields = typeof(AvatarData).GetFields();
			foreach (FieldInfo field in fields)
			{
				SerializableGameDataFieldAttribute attr = field.GetCustomAttribute<SerializableGameDataFieldAttribute>();
				bool flag2 = attr == null;
				if (!flag2)
				{
					sb.Append(field.Name).Append(": ").Append(field.GetValue(GMCheckAvatarState._currentAvatarRelatedData.AvatarData)).Append('\n');
				}
			}
			result = sb.ToString();
		}
		return result;
	}

	// Token: 0x0600215D RID: 8541 RVA: 0x000F2CD4 File Offset: 0x000F0ED4
	private static AvatarGroup GetCurrentAvatarGroup()
	{
		return SingletonObject.getInstance<AvatarManager>().GetAvatarGroup((int)GMCheckAvatarState._currentAvatarRelatedData.AvatarData.AvatarId);
	}

	// Token: 0x0600215E RID: 8542 RVA: 0x000F2D00 File Offset: 0x000F0F00
	private static List<AvatarAsset> GetBeardResList(bool isUpperBeard)
	{
		AvatarData avatarData = GMCheckAvatarState._currentAvatarRelatedData.AvatarData;
		AvatarGroup group = GMCheckAvatarState.GetCurrentAvatarGroup();
		List<AvatarAsset> resList = isUpperBeard ? group.Beard1Res : group.Beard2Res;
		bool isFemale = avatarData.AvatarId % 2 == 0;
		bool flag = (resList == null || resList.Count == 0) && !isFemale;
		if (flag)
		{
			AvatarGroup backupGroup = SingletonObject.getInstance<AvatarManager>().GetAvatarGroup(1);
			resList = (isUpperBeard ? backupGroup.Beard1Res : backupGroup.Beard2Res);
		}
		return resList ?? new List<AvatarAsset>();
	}

	// Token: 0x0600215F RID: 8543 RVA: 0x000F2D88 File Offset: 0x000F0F88
	private static bool TryStepEyes(int direction)
	{
		List<EyeRes> eyesGroup = GMCheckAvatarState.GetCurrentAvatarGroup().EyesGroup;
		bool flag = eyesGroup == null || eyesGroup.Count == 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			AvatarData avatarData = GMCheckAvatarState._currentAvatarRelatedData.AvatarData;
			int index = eyesGroup.FindIndex((EyeRes item) => item.Id == avatarData.EyesMainId && item.LeftEye.SubId == avatarData.EyesLeftId && item.RightEye.SubId == avatarData.EyesRightId);
			int targetIndex = GMCheckAvatarState.GetNextIndex(index, eyesGroup.Count, direction);
			EyeRes target = eyesGroup[targetIndex];
			bool changed = avatarData.EyesMainId != target.Id || avatarData.EyesLeftId != target.LeftEye.SubId || avatarData.EyesRightId != target.RightEye.SubId;
			bool flag2 = !changed;
			if (flag2)
			{
				result = false;
			}
			else
			{
				avatarData.EyesMainId = target.Id;
				avatarData.EyesLeftId = (short)((byte)target.LeftEye.SubId);
				avatarData.EyesRightId = (short)((byte)target.RightEye.SubId);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06002160 RID: 8544 RVA: 0x000F2EA8 File Offset: 0x000F10A8
	private static bool TryStepClothing(int direction)
	{
		AvatarManager avatarManager = SingletonObject.getInstance<AvatarManager>();
		AvatarGroup avatarGroup = GMCheckAvatarState.GetCurrentAvatarGroup();
		bool flag = GMCheckAvatarState._currentAvatarRelatedData.DisplayAge < 16;
		if (flag)
		{
			byte childAvatarId = avatarManager.GetChildAvatarIdByAvatarId(GMCheckAvatarState._currentAvatarRelatedData.AvatarData.AvatarId);
			avatarGroup = avatarManager.GetAvatarGroup((int)childAvatarId);
		}
		List<BodyRes> bodyRes = avatarGroup.BodyRes;
		List<short> list;
		if (bodyRes == null)
		{
			list = null;
		}
		else
		{
			list = (from item in bodyRes
			select item.Id).Distinct<short>().ToList<short>();
		}
		List<short> idList = list;
		bool flag2 = idList == null || idList.Count == 0;
		bool result;
		if (flag2)
		{
			result = false;
		}
		else
		{
			short currentId = GMCheckAvatarState._currentAvatarRelatedData.ClothingDisplayId;
			int index = idList.FindIndex((short item) => item == currentId);
			int targetIndex = GMCheckAvatarState.GetNextIndex(index, idList.Count, direction);
			short nextId = idList[targetIndex];
			bool flag3 = currentId == nextId;
			if (flag3)
			{
				result = false;
			}
			else
			{
				GMCheckAvatarState._currentAvatarRelatedData.ClothingDisplayId = nextId;
				GMCheckAvatarState._currentAvatarRelatedData.AvatarData.ClothDisplayId = nextId;
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06002161 RID: 8545 RVA: 0x000F2FCC File Offset: 0x000F11CC
	private static bool TryStepId(List<short> idList, ref short currentId, int direction, Action onChanged)
	{
		bool flag = idList == null || idList.Count == 0;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			short currentValue = currentId;
			int index = idList.FindIndex((short item) => item == currentValue);
			int targetIndex = GMCheckAvatarState.GetNextIndex(index, idList.Count, direction);
			short nextId = idList[targetIndex];
			bool flag2 = currentId == nextId;
			if (flag2)
			{
				if (onChanged != null)
				{
					onChanged();
				}
				result = (onChanged != null);
			}
			else
			{
				currentId = nextId;
				if (onChanged != null)
				{
					onChanged();
				}
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06002162 RID: 8546 RVA: 0x000F3064 File Offset: 0x000F1264
	private static int GetNextIndex(int currentIndex, int count, int direction)
	{
		bool flag = count <= 0;
		int result;
		if (flag)
		{
			result = -1;
		}
		else
		{
			bool flag2 = currentIndex < 0;
			if (flag2)
			{
				result = ((direction >= 0) ? 0 : (count - 1));
			}
			else
			{
				result = (currentIndex + direction + count) % count;
			}
		}
		return result;
	}

	// Token: 0x06002163 RID: 8547 RVA: 0x000F30A4 File Offset: 0x000F12A4
	private static bool TrySetDirectValue(ref short currentValue, short nextValue)
	{
		bool flag = currentValue == nextValue;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			currentValue = nextValue;
			result = true;
		}
		return result;
	}

	// Token: 0x06002164 RID: 8548 RVA: 0x000F30C8 File Offset: 0x000F12C8
	private static bool HideGrowableElement(sbyte growableElementType)
	{
		AvatarData avatarData = GMCheckAvatarState._currentAvatarRelatedData.AvatarData;
		bool flag = !avatarData.GetGrowableElementShowingState(growableElementType);
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			avatarData.SetGrowableElementShowingState(growableElementType, false);
			result = true;
		}
		return result;
	}

	// Token: 0x06002165 RID: 8549 RVA: 0x000F3104 File Offset: 0x000F1304
	private static void ShowHair()
	{
		AvatarData avatarData = GMCheckAvatarState._currentAvatarRelatedData.AvatarData;
		avatarData.SetGrowableElementShowingAbility(0, true);
		avatarData.SetGrowableElementShowingState(0, true);
	}

	// Token: 0x06002166 RID: 8550 RVA: 0x000F3130 File Offset: 0x000F1330
	private static void ShowEyebrow()
	{
		AvatarData avatarData = GMCheckAvatarState._currentAvatarRelatedData.AvatarData;
		avatarData.SetGrowableElementShowingAbility(6, true);
		avatarData.SetGrowableElementShowingState(6, true);
	}

	// Token: 0x06002167 RID: 8551 RVA: 0x000F315C File Offset: 0x000F135C
	private static void ShowBeard1()
	{
		AvatarData avatarData = GMCheckAvatarState._currentAvatarRelatedData.AvatarData;
		avatarData.SetGrowableElementShowingAbility(1, true);
		avatarData.SetGrowableElementShowingState(1, true);
	}

	// Token: 0x06002168 RID: 8552 RVA: 0x000F3188 File Offset: 0x000F1388
	private static void ShowBeard2()
	{
		AvatarData avatarData = GMCheckAvatarState._currentAvatarRelatedData.AvatarData;
		avatarData.SetGrowableElementShowingAbility(2, true);
		avatarData.SetGrowableElementShowingState(2, true);
	}

	// Token: 0x06002169 RID: 8553 RVA: 0x000F31B4 File Offset: 0x000F13B4
	private static void ShowWrinkle1()
	{
		AvatarData avatarData = GMCheckAvatarState._currentAvatarRelatedData.AvatarData;
		avatarData.SetGrowableElementShowingAbility(3, true);
		avatarData.SetGrowableElementShowingState(3, true);
	}

	// Token: 0x0600216A RID: 8554 RVA: 0x000F31E0 File Offset: 0x000F13E0
	private static void ShowWrinkle2()
	{
		AvatarData avatarData = GMCheckAvatarState._currentAvatarRelatedData.AvatarData;
		avatarData.SetGrowableElementShowingAbility(4, true);
		avatarData.SetGrowableElementShowingState(4, true);
	}

	// Token: 0x0600216B RID: 8555 RVA: 0x000F320C File Offset: 0x000F140C
	private static void ShowWrinkle3()
	{
		AvatarData avatarData = GMCheckAvatarState._currentAvatarRelatedData.AvatarData;
		avatarData.SetGrowableElementShowingAbility(5, true);
		avatarData.SetGrowableElementShowingState(5, true);
	}

	// Token: 0x0600216C RID: 8556 RVA: 0x000F3237 File Offset: 0x000F1437
	private static void NotifyStateChanged()
	{
		Action stateChanged = GMCheckAvatarState.StateChanged;
		if (stateChanged != null)
		{
			stateChanged();
		}
	}

	// Token: 0x040019C7 RID: 6599
	private static readonly GMCheckAvatarLineDefinition[] LineDefinitions = new GMCheckAvatarLineDefinition[]
	{
		new GMCheckAvatarLineDefinition(GMCheckAvatarComponentType.Clothing, "GM_CheckAvatar_Component_Clothing", false),
		new GMCheckAvatarLineDefinition(GMCheckAvatarComponentType.FrontHair, "GM_CheckAvatar_Component_FrontHair", false),
		new GMCheckAvatarLineDefinition(GMCheckAvatarComponentType.BackHair, "GM_CheckAvatar_Component_BackHair", false),
		new GMCheckAvatarLineDefinition(GMCheckAvatarComponentType.Eyebrow, "GM_CheckAvatar_Component_Eyebrow", false),
		new GMCheckAvatarLineDefinition(GMCheckAvatarComponentType.Eyes, "GM_CheckAvatar_Component_Eyes", false),
		new GMCheckAvatarLineDefinition(GMCheckAvatarComponentType.Nose, "GM_CheckAvatar_Component_Nose", false),
		new GMCheckAvatarLineDefinition(GMCheckAvatarComponentType.Mouth, "GM_CheckAvatar_Component_Mouth", false),
		new GMCheckAvatarLineDefinition(GMCheckAvatarComponentType.Beard1, "GM_CheckAvatar_Component_Beard1", true),
		new GMCheckAvatarLineDefinition(GMCheckAvatarComponentType.Beard2, "GM_CheckAvatar_Component_Beard2", true),
		new GMCheckAvatarLineDefinition(GMCheckAvatarComponentType.Feature1, "GM_CheckAvatar_Component_Feature1", true),
		new GMCheckAvatarLineDefinition(GMCheckAvatarComponentType.Feature2, "GM_CheckAvatar_Component_Feature2", true),
		new GMCheckAvatarLineDefinition(GMCheckAvatarComponentType.Wrinkle1, "GM_CheckAvatar_Component_Wrinkle1", true),
		new GMCheckAvatarLineDefinition(GMCheckAvatarComponentType.Wrinkle2, "GM_CheckAvatar_Component_Wrinkle2", true),
		new GMCheckAvatarLineDefinition(GMCheckAvatarComponentType.Wrinkle3, "GM_CheckAvatar_Component_Wrinkle3", true)
	};

	// Token: 0x040019C8 RID: 6600
	private static readonly Dictionary<GMCheckAvatarComponentType, GMCheckAvatarLineDefinition> DefinitionMap = GMCheckAvatarState.LineDefinitions.ToDictionary((GMCheckAvatarLineDefinition item) => item.ComponentType, (GMCheckAvatarLineDefinition item) => item);

	// Token: 0x040019C9 RID: 6601
	private static AvatarRelatedData _currentAvatarRelatedData;
}
