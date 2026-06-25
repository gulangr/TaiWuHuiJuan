using System;
using System.Collections.Generic;
using System.Text;
using Config;
using Game.Components.Avatar;
using GameData.Domains.Character.Display;
using GameData.Domains.CombatSkill;
using TMPro;

// Token: 0x02000235 RID: 565
public class TeachSkillItemView : Refers
{
	// Token: 0x060024C8 RID: 9416 RVA: 0x0010EC0C File Offset: 0x0010CE0C
	public void Refresh(CharacterDisplayData characterDisplayData, Dictionary<short, byte> readBookPageData, Dictionary<CharacterDisplayData, bool> favorabilityChangeData, Dictionary<CharacterDisplayData, ushort> relationChangeData, List<ValueTuple<CharacterDisplayData, int>> relationData)
	{
		this.InitRefers();
		this.SetName(characterDisplayData);
		this.SetAvatar(characterDisplayData);
		this.SetIcon(characterDisplayData);
		this.SetInfoValue(characterDisplayData);
		this.SetReadBookPageData(readBookPageData);
		this.SetFavorabilityChangeData(favorabilityChangeData);
		this.SetRelationChangeData(relationChangeData, relationData);
	}

	// Token: 0x060024C9 RID: 9417 RVA: 0x0010EC5C File Offset: 0x0010CE5C
	private void SetAvatar(CharacterDisplayData characterDisplayData)
	{
		this._avatar_SmallSize.Refresh(characterDisplayData.AvatarRelatedData);
	}

	// Token: 0x060024CA RID: 9418 RVA: 0x0010EC71 File Offset: 0x0010CE71
	private void SetName(CharacterDisplayData characterDisplayData)
	{
		this._name.text = NameCenter.GetMonasticTitleOrDisplayName(characterDisplayData, characterDisplayData.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId);
	}

	// Token: 0x060024CB RID: 9419 RVA: 0x0010EC98 File Offset: 0x0010CE98
	private void SetIcon(CharacterDisplayData characterDisplayData)
	{
		this._icon.gameObject.SetActive(true);
		this._icon.SetSprite(CommonUtils.GetIdentityIcon(characterDisplayData.OrgInfo.Grade), false, null);
	}

	// Token: 0x060024CC RID: 9420 RVA: 0x0010ECCB File Offset: 0x0010CECB
	private void SetInfoValue(CharacterDisplayData characterDisplayData)
	{
		this._infoValue.text = CommonUtils.GetCharacterGradeString(characterDisplayData.OrgInfo, characterDisplayData.Gender, characterDisplayData.PhysiologicalAge);
	}

	// Token: 0x060024CD RID: 9421 RVA: 0x0010ECF4 File Offset: 0x0010CEF4
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
						pages.Append(i.ToString() + "、");
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
				string icon = (skillBook.LifeSkillType == -1) ? Config.CombatSkillType.Instance[skillBook.CombatSkillType].TipsIcon : LifeSkillType.Instance[skillBook.LifeSkillType].DisplayIconBig;
				SkillBookItem bookItem = SkillBook.Instance[bookTemplateId];
				content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_07, TMPTextSpriteHelper.GetStringWithTextSpriteTag(icon) + bookItem.Name.SetGradeColor((int)bookItem.Grade), pages.ToString().SetColor("pinkyellow")).SetColor("grey"));
				pages.Clear();
				content.Append("\n");
			}
		}
		this._readBookContentLine.CGet<TextMeshProUGUI>("Content").text = ((content.Length != 0) ? content.ToString() : "");
		TMPTextSpriteHelper spriteHelper = this._readBookContentLine.CGet<TMPTextSpriteHelper>("SpriteHelper");
		spriteHelper.Parse();
	}

	// Token: 0x060024CE RID: 9422 RVA: 0x0010EEEC File Offset: 0x0010D0EC
	private void SetFavorabilityChangeData(Dictionary<CharacterDisplayData, bool> favorabilityChangeData)
	{
		StringBuilder content = new StringBuilder();
		foreach (KeyValuePair<CharacterDisplayData, bool> item in favorabilityChangeData)
		{
			string charName = NameCenter.GetMonasticTitleOrDisplayName(item.Key, item.Key.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId).SetColor("brightred");
			bool value = item.Value;
			if (value)
			{
				content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_08, charName).SetColor("grey"));
			}
			else
			{
				content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_09, charName).SetColor("grey"));
			}
			content.Append("\n");
		}
		TextMeshProUGUI contentLable = this._favorabilityContentLine.CGet<TextMeshProUGUI>("Content");
		TMPTextSpriteHelper spriteHelper = this._favorabilityContentLine.CGet<TMPTextSpriteHelper>("SpriteHelper");
		contentLable.text = ((content.Length != 0) ? content.ToString() : "");
		spriteHelper.Parse();
	}

	// Token: 0x060024CF RID: 9423 RVA: 0x0010F010 File Offset: 0x0010D210
	private void SetRelationChangeData(Dictionary<CharacterDisplayData, ushort> relationChangeData, List<ValueTuple<CharacterDisplayData, int>> relationData)
	{
		StringBuilder content = new StringBuilder();
		foreach (KeyValuePair<CharacterDisplayData, ushort> item in relationChangeData)
		{
			string charName = NameCenter.GetMonasticTitleOrDisplayName(item.Key, item.Key.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId).SetColor("brightred");
			ushort value = item.Value;
			ushort num = value;
			if (num != 512)
			{
				if (num != 8192)
				{
					if (num == 32768)
					{
						content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_16, charName).SetColor("grey"));
					}
				}
				else
				{
					content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_10, charName).SetColor("grey"));
				}
			}
			else
			{
				content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_11, charName).SetColor("grey"));
			}
			content.Append("\n");
		}
		foreach (ValueTuple<CharacterDisplayData, int> relationItem in relationData)
		{
			string charName2 = NameCenter.GetMonasticTitleOrDisplayName(relationItem.Item1, relationItem.Item1.CharacterId == SingletonObject.getInstance<BasicGameData>().TaiwuCharId).SetColor("brightred");
			switch (relationItem.Item2)
			{
			case 0:
				content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_10, charName2).SetColor("grey"));
				break;
			case 1:
			{
				bool flag = relationItem.Item1.Gender == 1;
				if (flag)
				{
					content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_Adopted_Son, charName2).SetColor("grey"));
				}
				else
				{
					content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_Adopted_Daughter, charName2).SetColor("grey"));
				}
				break;
			}
			case 2:
			{
				bool flag2 = relationItem.Item1.Gender == 1;
				if (flag2)
				{
					content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_Adopted_Father, charName2).SetColor("grey"));
				}
				else
				{
					content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_Adopted_Mother, charName2).SetColor("grey"));
				}
				break;
			}
			case 3:
				content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_11, charName2).SetColor("grey"));
				break;
			case 4:
				content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_12, charName2).SetColor("grey"));
				break;
			case 5:
				content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_13, charName2).SetColor("grey"));
				break;
			case 6:
				content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_14, charName2).SetColor("grey"));
				break;
			case 7:
				content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_15, charName2).SetColor("grey"));
				break;
			case 9:
				content.Append(LocalStringManager.GetFormat(LanguageKey.LK_TeachSkillResult_17, charName2).SetColor("grey"));
				break;
			}
			content.Append("\n");
		}
		TextMeshProUGUI contentLable = this._relationContentLine.CGet<TextMeshProUGUI>("Content");
		TMPTextSpriteHelper spriteHelper = this._relationContentLine.CGet<TMPTextSpriteHelper>("SpriteHelper");
		contentLable.text = ((content.Length != 0) ? content.ToString() : "");
		spriteHelper.Parse();
	}

	// Token: 0x060024D0 RID: 9424 RVA: 0x0010F3E8 File Offset: 0x0010D5E8
	private void InitRefers()
	{
		this._name = base.CGet<TextMeshProUGUI>("Name");
		this._avatar_SmallSize = base.CGet<Avatar>("Avatar_SmallSize");
		this._icon = base.CGet<CImage>("Icon");
		this._infoValue = base.CGet<TextMeshProUGUI>("InfoValue");
		this._readBookContentLine = base.CGet<Refers>("ReadBookContentLine");
		this._favorabilityContentLine = base.CGet<Refers>("FavorabilityContentLine");
		this._relationContentLine = base.CGet<Refers>("RelationContentLine");
	}

	// Token: 0x04001B77 RID: 7031
	private TextMeshProUGUI _name;

	// Token: 0x04001B78 RID: 7032
	private Avatar _avatar_SmallSize;

	// Token: 0x04001B79 RID: 7033
	private CImage _icon;

	// Token: 0x04001B7A RID: 7034
	private TextMeshProUGUI _infoValue;

	// Token: 0x04001B7B RID: 7035
	private Refers _practiceContentLine;

	// Token: 0x04001B7C RID: 7036
	private Refers _readBookContentLine;

	// Token: 0x04001B7D RID: 7037
	private Refers _favorabilityContentLine;

	// Token: 0x04001B7E RID: 7038
	private Refers _relationContentLine;
}
