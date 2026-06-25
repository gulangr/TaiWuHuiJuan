using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Adventure;
using Property;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200018E RID: 398
public class MajorEventCharacterTemplate : MajorEventTemplate<AdventureCharacterGroup>
{
	// Token: 0x1700027A RID: 634
	// (get) Token: 0x06001669 RID: 5737 RVA: 0x0008A735 File Offset: 0x00088935
	public override IList<AdventureCharacterGroup> DataList
	{
		get
		{
			return this.parent.Snapshot.Characters;
		}
	}

	// Token: 0x0600166A RID: 5738 RVA: 0x0008A747 File Offset: 0x00088947
	public override void RefreshAll()
	{
		this.parent.RefreshCharacter();
	}

	// Token: 0x0600166B RID: 5739 RVA: 0x0008A758 File Offset: 0x00088958
	public override void RefreshData()
	{
		this.dropdownType.SetupEditor(Enum.GetValues(typeof(EAdventureCharacterType)).Cast<EAdventureCharacterType>(), (EAdventureCharacterType type) => (type == EAdventureCharacterType.Invalid) ? LanguageKey.LK_None.Tr() : LocalStringManager.Get("LK_AdventureEditor_AdventureCharacterType_" + type.ToString()), delegate(EAdventureCharacterType type)
		{
			base.Data.Data.Type = type;
		}, (EAdventureCharacterType type) => type == base.Data.Data.Type, true);
		this.dropdownSearchRange.SetupEditor(new sbyte[]
		{
			0,
			1,
			2
		}, (sbyte r) => LocalStringManager.Get(string.Format("LK_AdventureEditor_CharacterSearchRange_{0}", r)), delegate(sbyte r)
		{
			base.Data.Data.SearchRangeType = (int)r;
		}, (sbyte r) => (int)r == base.Data.Data.SearchRangeType, true);
		this.dropdownFilterRule.SetupEditor(from pair in CharacterFilterRules.Instance.RefNameMap
		orderby pair.Value
		select pair, (KeyValuePair<string, int> pair) => pair.Key, delegate(KeyValuePair<string, int> pair)
		{
			base.Data.Data.FilterRuleTemplateId = (int)((short)pair.Value);
		}, (KeyValuePair<string, int> pair) => pair.Value == base.Data.Data.FilterRuleTemplateId, true);
		this.RefreshCount();
		this.RefreshMajor();
		this.RefreshStayIndex();
	}

	// Token: 0x0600166C RID: 5740 RVA: 0x0008A897 File Offset: 0x00088A97
	private void Awake()
	{
		this.stayIndexToggle.onValueChanged.RemoveAllListeners();
		this.stayIndexToggle.onValueChanged.AddListener(new UnityAction<bool>(this.SetStayIndex));
	}

	// Token: 0x0600166D RID: 5741 RVA: 0x0008A8C8 File Offset: 0x00088AC8
	public void SetMajor(bool major)
	{
		base.Data.Major = major;
	}

	// Token: 0x0600166E RID: 5742 RVA: 0x0008A8D7 File Offset: 0x00088AD7
	public void RefreshMajor()
	{
		this.isMajor.isOn = base.Data.Major;
	}

	// Token: 0x0600166F RID: 5743 RVA: 0x0008A8F0 File Offset: 0x00088AF0
	public void SetStayIndex(bool stayIndex)
	{
		base.Data.StayIndex = stayIndex;
	}

	// Token: 0x06001670 RID: 5744 RVA: 0x0008A8FF File Offset: 0x00088AFF
	public void RefreshStayIndex()
	{
		this.stayIndexToggle.isOn = base.Data.StayIndex;
	}

	// Token: 0x06001671 RID: 5745 RVA: 0x0008A918 File Offset: 0x00088B18
	public void EditCount(string str)
	{
		int value = base.Data.Count;
		AdventureMajorEventTool.EditInt(ref value, str, "EditCount");
		base.Data.Count = value;
	}

	// Token: 0x06001672 RID: 5746 RVA: 0x0008A950 File Offset: 0x00088B50
	public void RefreshCount()
	{
		this.count.SetTextWithoutNotify(base.Data.Count.ToString());
	}

	// Token: 0x06001674 RID: 5748 RVA: 0x0008A988 File Offset: 0x00088B88
	// Note: this type is marked as 'beforefieldinit'.
	static MajorEventCharacterTemplate()
	{
		EAdventureCharacterType[] array = new EAdventureCharacterType[4];
		RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.BAED642339816AFFB3FE8719792D0E4CE82F12DB72B7373D244EAA65445800FE).FieldHandle);
		MajorEventCharacterTemplate.Types = array;
		MajorEventCharacterTemplate.Type2Int = MajorEventCharacterTemplate.Types.Select((EAdventureCharacterType key, int value) => new ValueTuple<EAdventureCharacterType, int>(key, value)).ToDictionary(([TupleElementNames(new string[]
		{
			"key",
			"value"
		})] ValueTuple<EAdventureCharacterType, int> item) => item.Item1, ([TupleElementNames(new string[]
		{
			"key",
			"value"
		})] ValueTuple<EAdventureCharacterType, int> item) => item.Item2);
	}

	// Token: 0x04001247 RID: 4679
	[SerializeField]
	private CDropdown dropdownType;

	// Token: 0x04001248 RID: 4680
	[SerializeField]
	private CDropdown dropdownSearchRange;

	// Token: 0x04001249 RID: 4681
	[SerializeField]
	private CDropdown dropdownFilterRule;

	// Token: 0x0400124A RID: 4682
	[SerializeField]
	private TMP_InputField count;

	// Token: 0x0400124B RID: 4683
	[SerializeField]
	private CToggle isMajor;

	// Token: 0x0400124C RID: 4684
	[SerializeField]
	private CToggle stayIndexToggle;

	// Token: 0x0400124D RID: 4685
	public static readonly IReadOnlyList<EAdventureCharacterType> Types;

	// Token: 0x0400124E RID: 4686
	public static readonly IReadOnlyDictionary<EAdventureCharacterType, int> Type2Int;
}
