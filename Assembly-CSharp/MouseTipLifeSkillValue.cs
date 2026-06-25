using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Domains.Character;
using TMPro;

// Token: 0x020002B4 RID: 692
public class MouseTipLifeSkillValue : MouseTipBase
{
	// Token: 0x170004A7 RID: 1191
	// (get) Token: 0x06002AA2 RID: 10914 RVA: 0x00146895 File Offset: 0x00144A95
	protected override bool CanStick
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002AA3 RID: 10915 RVA: 0x00146898 File Offset: 0x00144A98
	protected override void Init(ArgumentBox argsBox)
	{
		argsBox.Get("charName", out this._charName);
		argsBox.Get<LifeSkillShorts>("values", out this._lifeSkillShorts);
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.UpdateSkillInfo));
	}

	// Token: 0x06002AA4 RID: 10916 RVA: 0x001468F1 File Offset: 0x00144AF1
	private void Awake()
	{
		this._lifeSkillRefers = base.CGetList<Refers>("LifeSkill");
	}

	// Token: 0x06002AA5 RID: 10917 RVA: 0x00146908 File Offset: 0x00144B08
	private unsafe void UpdateSkillInfo()
	{
		base.CGet<TextMeshProUGUI>("TitleName").SetText(LocalStringManager.GetFormat(LanguageKey.LK_Char_LifeSkill_Qualification, this._charName), true);
		for (int i = 0; i < 16; i++)
		{
			LifeSkillTypeItem config = Config.LifeSkillType.Instance[i];
			this._lifeSkillRefers[i].CGet<CImage>("SkillIcon").SetSprite(string.Format("sp_14_iconjiyizhanshi_{0}", i), false, null);
			this._lifeSkillRefers[i].CGet<TextMeshProUGUI>("SkillName").SetText(config.Name, true);
			this._lifeSkillRefers[i].CGet<TextMeshProUGUI>("SkillValue").SetText((*(ref this._lifeSkillShorts.Items.FixedElementField + (IntPtr)i * 2)).SetValueColor(), true);
		}
	}

	// Token: 0x04001EDB RID: 7899
	private string _charName;

	// Token: 0x04001EDC RID: 7900
	private LifeSkillShorts _lifeSkillShorts;

	// Token: 0x04001EDD RID: 7901
	private List<Refers> _lifeSkillRefers;
}
