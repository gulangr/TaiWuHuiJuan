using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;

// Token: 0x020002A8 RID: 680
public class MouseTipLifeCombatSkillValue : MouseTipBase
{
	// Token: 0x1700049F RID: 1183
	// (get) Token: 0x06002A64 RID: 10852 RVA: 0x0014465E File Offset: 0x0014285E
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002A65 RID: 10853 RVA: 0x00144664 File Offset: 0x00142864
	protected override void Init(ArgumentBox argsBox)
	{
		this.Init();
		argsBox.Get("charId", out this._charId);
		bool customizeData;
		bool flag = argsBox.Get("customizeData", out customizeData);
		if (flag)
		{
			LifeSkillShorts lifeSkillData;
			argsBox.Get<LifeSkillShorts>("lifeSkillQualifications", out lifeSkillData);
			CombatSkillShorts combatSkillData;
			argsBox.Get<CombatSkillShorts>("combatSkillQualifications", out combatSkillData);
			string characterName;
			argsBox.Get("characterName", out characterName);
			this.UpdateSkillInfo(lifeSkillData, combatSkillData, characterName);
		}
		else
		{
			this.UpdateSkillInfo();
		}
	}

	// Token: 0x06002A66 RID: 10854 RVA: 0x001446DE File Offset: 0x001428DE
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x06002A67 RID: 10855 RVA: 0x001446E8 File Offset: 0x001428E8
	private void Init()
	{
		bool inited = this._inited;
		if (!inited)
		{
			this._inited = true;
			this._lifeSkillRefers = base.CGetList<Refers>("LifeSkill_");
			this._combatSkillRefers = base.CGetList<Refers>("CombatSkill_");
		}
	}

	// Token: 0x06002A68 RID: 10856 RVA: 0x0014472C File Offset: 0x0014292C
	private unsafe void UpdateSkillInfo(LifeSkillShorts lifeSkillData, CombatSkillShorts combatSkillData, string characterName)
	{
		base.CGet<TextMeshProUGUI>("TitleName").SetText(LocalStringManager.GetFormat(LanguageKey.LK_Char_LifeCombatSkill_Qualification, characterName), true);
		for (int i = 0; i < 16; i++)
		{
			LifeSkillTypeItem config = Config.LifeSkillType.Instance[i];
			this._lifeSkillRefers[i].CGet<CImage>("SkillIcon").SetSprite(string.Format("sp_14_iconjiyizhanshi_{0}", i), false, null);
			this._lifeSkillRefers[i].CGet<TextMeshProUGUI>("SkillName").SetText(config.Name, true);
			this._lifeSkillRefers[i].CGet<TextMeshProUGUI>("SkillValue").SetText((*(ref lifeSkillData.Items.FixedElementField + (IntPtr)i * 2)).SetValueColor(), true);
		}
		for (int j = 0; j < 14; j++)
		{
			CombatSkillTypeItem config2 = CombatSkillType.Instance[j];
			this._combatSkillRefers[j].CGet<CImage>("SkillIcon").SetSprite(string.Format("sp_18_iconwuxuezhanshi_{0}", j), false, null);
			this._combatSkillRefers[j].CGet<TextMeshProUGUI>("SkillName").SetText(config2.Name, true);
			this._combatSkillRefers[j].CGet<TextMeshProUGUI>("SkillValue").SetText((*(ref combatSkillData.Items.FixedElementField + (IntPtr)j * 2)).SetValueColor(), true);
		}
	}

	// Token: 0x06002A69 RID: 10857 RVA: 0x001448AF File Offset: 0x00142AAF
	private unsafe void UpdateSkillInfo()
	{
		CharacterDomainMethod.AsyncCall.GetGroupCharDisplayDataList(null, new List<int>
		{
			this._charId
		}, delegate(int offset, RawDataPool dataPool)
		{
			List<GroupCharDisplayData> displayDataList = new List<GroupCharDisplayData>();
			Serializer.Deserialize(dataPool, offset, ref displayDataList);
			string charName = NameCenter.GetDisplayName(ref displayDataList[0].NameData, false);
			base.CGet<TextMeshProUGUI>("TitleName").SetText(LocalStringManager.GetFormat(LanguageKey.LK_Char_LifeCombatSkill_Qualification, charName), true);
			for (int i = 0; i < 16; i++)
			{
				LifeSkillTypeItem config = Config.LifeSkillType.Instance[i];
				this._lifeSkillRefers[i].CGet<CImage>("SkillIcon").SetSprite(string.Format("sp_14_iconjiyizhanshi_{0}", i), false, null);
				this._lifeSkillRefers[i].CGet<TextMeshProUGUI>("SkillName").SetText(config.Name, true);
				this._lifeSkillRefers[i].CGet<TextMeshProUGUI>("SkillValue").SetText((*(ref displayDataList[0].LifeSkillQualifications.Items.FixedElementField + (IntPtr)i * 2)).SetValueColor(), true);
			}
			for (int j = 0; j < 14; j++)
			{
				CombatSkillTypeItem config2 = CombatSkillType.Instance[j];
				this._combatSkillRefers[j].CGet<CImage>("SkillIcon").SetSprite(string.Format("sp_18_iconwuxuezhanshi_{0}", j), false, null);
				this._combatSkillRefers[j].CGet<TextMeshProUGUI>("SkillName").SetText(config2.Name, true);
				this._combatSkillRefers[j].CGet<TextMeshProUGUI>("SkillValue").SetText((*(ref displayDataList[0].CombatSkillQualifications.Items.FixedElementField + (IntPtr)j * 2)).SetValueColor(), true);
			}
		});
	}

	// Token: 0x04001EB0 RID: 7856
	private int _charId;

	// Token: 0x04001EB1 RID: 7857
	private List<Refers> _lifeSkillRefers;

	// Token: 0x04001EB2 RID: 7858
	private List<Refers> _combatSkillRefers;

	// Token: 0x04001EB3 RID: 7859
	private bool _inited = false;
}
