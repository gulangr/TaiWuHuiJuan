using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using Game.Components.Common;
using GameData.Domains.Character.Display;
using GameData.Domains.Taiwu.Debate;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Debate.DebateResult
{
	// Token: 0x02000AAA RID: 2730
	public class DebateResultCharacterArea : MonoBehaviour
	{
		// Token: 0x17000EBF RID: 3775
		// (get) Token: 0x06008626 RID: 34342 RVA: 0x003E6F6F File Offset: 0x003E516F
		private LifeSkillCombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<LifeSkillCombatModel>();
			}
		}

		// Token: 0x06008627 RID: 34343 RVA: 0x003E6F78 File Offset: 0x003E5178
		public void Refresh(DebateResult debateResult, bool isSelf)
		{
			this.rootSelfGain.gameObject.SetActive(isSelf);
			if (isSelf)
			{
				ResourceTypeItem authorityConfig = ResourceType.Instance[7];
				string authorityStr = (debateResult.Exp.Second != 0) ? (CommonUtils.GetDisplayStringForNum(debateResult.Authority.First, 100000) + ViewDebateResult.GetValueString(debateResult.Authority.Second, false)) : "-".SetColor("grey");
				this.propertyAuthority.Set(authorityConfig.Icon, authorityConfig.Name, authorityStr, null, false);
				string expIcon = "ui9_icon_resource_big_8";
				string expName = LanguageKey.LK_Exp.Tr();
				string expStr = (debateResult.Exp.Second != 0) ? (CommonUtils.GetDisplayStringForNum(debateResult.Exp.First, 100000) + ViewDebateResult.GetValueString(debateResult.Exp.Second, false)) : "-".SetColor("grey");
				this.propertyExp.Set(expIcon, expName, expStr, null, false);
			}
			int mainCharId = isSelf ? this.Model.TaiwuCharId : this.Model.EnemyCharId;
			CharacterDisplayData charData = debateResult.CharacterDisplayDataMap[mainCharId];
			int delta = debateResult.Happiness[mainCharId].Second - debateResult.Happiness[mainCharId].First;
			this.mainCharacter.Refresh(charData, false, delta, isSelf);
			List<int> audienceList = (from d in this.Model.GetAudienceList(isSelf)
			where d != null
			select d.CharacterId).ToList<int>();
			this.objNoneSpectator.gameObject.SetActive(audienceList.Count == 0);
			for (int i = 0; i < this.spectatorCharacterArray.Length; i++)
			{
				DebateResultCharacter character = this.spectatorCharacterArray[i];
				int charId = audienceList.GetOrDefault(i, -1);
				bool isValid = charId >= 0;
				character.gameObject.SetActive(isValid);
				bool flag = !isValid;
				if (!flag)
				{
					CharacterDisplayData charData2 = debateResult.CharacterDisplayDataMap[charId];
					int delta2 = debateResult.Favorability[charId].Second - debateResult.Favorability[charId].First;
					character.Refresh(charData2, true, delta2, false);
				}
			}
		}

		// Token: 0x040066F7 RID: 26359
		[SerializeField]
		private GameObject rootSelfGain;

		// Token: 0x040066F8 RID: 26360
		[SerializeField]
		private PropertyItem propertyAuthority;

		// Token: 0x040066F9 RID: 26361
		[SerializeField]
		private PropertyItem propertyExp;

		// Token: 0x040066FA RID: 26362
		[SerializeField]
		private DebateResultCharacter mainCharacter;

		// Token: 0x040066FB RID: 26363
		[SerializeField]
		private DebateResultCharacter[] spectatorCharacterArray;

		// Token: 0x040066FC RID: 26364
		[SerializeField]
		private GameObject objNoneSpectator;
	}
}
