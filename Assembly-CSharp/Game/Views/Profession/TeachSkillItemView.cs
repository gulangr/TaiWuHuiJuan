using System;
using System.Collections.Generic;
using System.Text;
using Config;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using GameData.Domains.CombatSkill;
using TMPro;
using UnityEngine;

namespace Game.Views.Profession
{
	// Token: 0x020007C3 RID: 1987
	public class TeachSkillItemView : MonoBehaviour
	{
		// Token: 0x060060F8 RID: 24824 RVA: 0x002C7240 File Offset: 0x002C5440
		public void Refresh(CharacterDisplayData characterDisplayData, Dictionary<short, byte> readBookPageData, Dictionary<CharacterDisplayData, bool> favorabilityChangeData, Dictionary<CharacterDisplayData, ushort> relationChangeData, List<ValueTuple<CharacterDisplayData, int>> relationData)
		{
			this.SetName(characterDisplayData);
			this.SetAvatar(characterDisplayData);
			this.SetIcon(characterDisplayData);
			this.SetInfoValue(characterDisplayData);
			this.SetReadBookPageData(readBookPageData);
			this.SetFavorabilityChangeData(favorabilityChangeData);
			this.SetRelationChangeData(relationChangeData, relationData);
		}

		// Token: 0x060060F9 RID: 24825 RVA: 0x002C727E File Offset: 0x002C547E
		private void SetAvatar(CharacterDisplayData characterDisplayData)
		{
			this.avatar.Refresh(characterDisplayData, true);
		}

		// Token: 0x060060FA RID: 24826 RVA: 0x002C728F File Offset: 0x002C548F
		private void SetName(CharacterDisplayData characterDisplayData)
		{
			this.charName.text = NameCenter.GetMonasticTitleOrDisplayName(characterDisplayData, characterDisplayData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
		}

		// Token: 0x060060FB RID: 24827 RVA: 0x002C72B8 File Offset: 0x002C54B8
		private void SetIcon(CharacterDisplayData characterDisplayData)
		{
			this.icon.gameObject.SetActive(true);
			this.icon.SetSprite(string.Format("{0}{1}", "ui9_back_bounty_identity_big_", characterDisplayData.OrgInfo.Grade), false, null);
		}

		// Token: 0x060060FC RID: 24828 RVA: 0x002C7305 File Offset: 0x002C5505
		private void SetInfoValue(CharacterDisplayData characterDisplayData)
		{
			this.infoValue.text = CommonUtils.GetCharacterGradeString(characterDisplayData.OrgInfo, characterDisplayData.Gender, characterDisplayData.PhysiologicalAge);
		}

		// Token: 0x060060FD RID: 24829 RVA: 0x002C732C File Offset: 0x002C552C
		private void SetReadBookPageData(Dictionary<short, byte> readBookPageData)
		{
			StringBuilder content = new StringBuilder();
			StringBuilder pages = new StringBuilder();
			foreach (KeyValuePair<short, byte> item in readBookPageData)
			{
				bool flag = item.Value == 0;
				if (!flag)
				{
					short bookTemplateId = item.Key;
					SkillBookItem skillBook = SkillBook.Instance[bookTemplateId];
					int pageCount = (skillBook.LifeSkillType == -1) ? 15 : 5;
					byte i = 0;
					while ((int)i < pageCount)
					{
						bool flag2 = CombatSkillStateHelper.IsPageRead((ushort)item.Value, i);
						if (flag2)
						{
							pages.Append(i.ToString() + LanguageKey.LK_Separator.Tr());
						}
						i += 1;
					}
					bool flag3 = pages.Length > 0;
					if (flag3)
					{
						StringBuilder stringBuilder = pages;
						int length = stringBuilder.Length - 1;
						stringBuilder.Length = length;
					}
					string icon = (skillBook.LifeSkillType == -1) ? Config.CombatSkillType.Instance[skillBook.CombatSkillType].TipsIcon : LifeSkillType.Instance[skillBook.LifeSkillType].Icon;
					SkillBookItem bookItem = SkillBook.Instance[bookTemplateId];
					content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_07, TMPTextSpriteHelper.GetStringWithTextSpriteTag(icon) + bookItem.Name.SetGradeColor((int)bookItem.Grade), pages.ToString()));
					pages.Clear();
					content.Append("\n");
				}
			}
			this.readBookContentLine.text = ((content.Length != 0) ? content.ToString() : "");
			TMPTextSpriteHelper spriteHelper = this.readBookContentLine.GetComponent<TMPTextSpriteHelper>();
			spriteHelper.Parse();
		}

		// Token: 0x060060FE RID: 24830 RVA: 0x002C7504 File Offset: 0x002C5704
		private void SetFavorabilityChangeData(Dictionary<CharacterDisplayData, bool> favorabilityChangeData)
		{
			StringBuilder content = new StringBuilder();
			foreach (KeyValuePair<CharacterDisplayData, bool> item in favorabilityChangeData)
			{
				string charName = NameCenter.GetMonasticTitleOrDisplayName(item.Key, item.Key.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
				charName = string.Format("<color=#ce5d20>{0}</color>", charName);
				bool value = item.Value;
				if (value)
				{
					content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_08, charName));
				}
				else
				{
					content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_09, charName));
				}
				content.Append("\n");
			}
			TextMeshProUGUI contentLable = this.favorabilityContentLine;
			contentLable.text = ((content.Length != 0) ? content.ToString() : "");
		}

		// Token: 0x060060FF RID: 24831 RVA: 0x002C75F4 File Offset: 0x002C57F4
		private void SetRelationChangeData(Dictionary<CharacterDisplayData, ushort> relationChangeData, List<ValueTuple<CharacterDisplayData, int>> relationData)
		{
			StringBuilder content = new StringBuilder();
			foreach (KeyValuePair<CharacterDisplayData, ushort> item in relationChangeData)
			{
				string charName = NameCenter.GetMonasticTitleOrDisplayName(item.Key, item.Key.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
				charName = string.Format("<color=#ce5d20>{0}</color>", charName);
				ushort value = item.Value;
				ushort num = value;
				if (num != 512)
				{
					if (num != 8192)
					{
						if (num == 32768)
						{
							content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_16, charName));
						}
					}
					else
					{
						content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_10, charName));
					}
				}
				else
				{
					content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_11, charName));
				}
				content.Append("\n");
			}
			foreach (ValueTuple<CharacterDisplayData, int> relationItem in relationData)
			{
				string charName2 = NameCenter.GetMonasticTitleOrDisplayName(relationItem.Item1, relationItem.Item1.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
				charName2 = string.Format("<color=#ce5d20>{0}</color>", charName2);
				switch (relationItem.Item2)
				{
				case 0:
					content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_10, charName2));
					break;
				case 1:
				{
					bool flag = relationItem.Item1.Gender == 1;
					if (flag)
					{
						content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_Adopted_Son, charName2));
					}
					else
					{
						content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_Adopted_Daughter, charName2));
					}
					break;
				}
				case 2:
				{
					bool flag2 = relationItem.Item1.Gender == 1;
					if (flag2)
					{
						content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_Adopted_Father, charName2));
					}
					else
					{
						content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_Adopted_Mother, charName2));
					}
					break;
				}
				case 3:
					content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_11, charName2));
					break;
				case 4:
					content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_12, charName2));
					break;
				case 5:
					content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_13, charName2));
					break;
				case 6:
					content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_14, charName2));
					break;
				case 7:
					content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_15, charName2));
					break;
				case 9:
					content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_17, charName2));
					break;
				}
				content.Append("\n");
			}
			TextMeshProUGUI contentLable = this.relationContentLine;
			contentLable.text = ((content.Length != 0) ? content.ToString() : "");
		}

		// Token: 0x04004339 RID: 17209
		[SerializeField]
		private TextMeshProUGUI charName;

		// Token: 0x0400433A RID: 17210
		[SerializeField]
		private Game.Components.Avatar.Avatar avatar;

		// Token: 0x0400433B RID: 17211
		[SerializeField]
		private CImage icon;

		// Token: 0x0400433C RID: 17212
		[SerializeField]
		private TextMeshProUGUI infoValue;

		// Token: 0x0400433D RID: 17213
		[SerializeField]
		private TextMeshProUGUI readBookContentLine;

		// Token: 0x0400433E RID: 17214
		[SerializeField]
		private TextMeshProUGUI favorabilityContentLine;

		// Token: 0x0400433F RID: 17215
		[SerializeField]
		private TextMeshProUGUI relationContentLine;

		// Token: 0x04004340 RID: 17216
		private const string CharNameColor = "<color=#ce5d20>{0}</color>";
	}
}
