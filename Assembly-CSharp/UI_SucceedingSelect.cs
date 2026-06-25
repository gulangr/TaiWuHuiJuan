using System;
using System.Collections.Generic;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x020001DA RID: 474
public class UI_SucceedingSelect : UIBase
{
	// Token: 0x06001F07 RID: 7943 RVA: 0x000E17CC File Offset: 0x000DF9CC
	public override void OnInit(ArgumentBox argsBox)
	{
		List<int> characterIdList;
		argsBox.Get<List<int>>("CharacterIdList", out characterIdList);
		List<int> bannedIds;
		argsBox.Get<List<int>>("CharacterIdBanned", out bannedIds);
		CharacterDomainMethod.AsyncCall.GetGroupCharDisplayDataList(null, characterIdList, delegate(int offset, RawDataPool dataPool)
		{
			List<GroupCharDisplayData> dataList = new List<GroupCharDisplayData>();
			Serializer.Deserialize(dataPool, offset, ref dataList);
			List<int> charList = new List<int>();
			HashSet<int> banned = (bannedIds == null) ? new HashSet<int>() : new HashSet<int>(bannedIds);
			foreach (GroupCharDisplayData data in dataList)
			{
				charList.Add(data.CharacterId);
				bool flag = AgeGroup.GetAgeGroup(data.PhysiologicalAge) != 2 || !SharedMethods.CanBeTaiwu(data.CharacterTemplateId);
				if (flag)
				{
					banned.Add(data.CharacterId);
				}
			}
			CharacterTable table = this.CGet<CharacterTable>("CharacterTable");
			table.Init(charList, null, null, banned, null, null, null, null, -1, null, true, null);
		});
	}

	// Token: 0x06001F08 RID: 7944 RVA: 0x000E181B File Offset: 0x000DFA1B
	public override void QuickHide()
	{
	}

	// Token: 0x06001F09 RID: 7945 RVA: 0x000E1820 File Offset: 0x000DFA20
	protected override void OnClick(Transform btn)
	{
		string btnName = btn.name;
		bool flag = btnName == "ButtonConfirm";
		if (flag)
		{
			this.OnClickConfirm();
		}
	}

	// Token: 0x06001F0A RID: 7946 RVA: 0x000E184C File Offset: 0x000DFA4C
	private void OnClickConfirm()
	{
		List<int> chars = base.CGet<CharacterTable>("CharacterTable").GetSelectedCharIdList();
		int selectedCharId = chars[0];
		DisplayTriggerModel.ShowInheritUILegacy(selectedCharId);
	}
}
