using System;
using CharacterDataMonitor;

namespace UICommon.Character
{
	// Token: 0x020005CD RID: 1485
	public class CharacterDetailInfo : CharacterUIElement
	{
		// Token: 0x0600465F RID: 18015 RVA: 0x0020FBFC File Offset: 0x0020DDFC
		public CharacterDetailInfo(Refers rootRefers)
		{
			bool flag = rootRefers == null;
			if (flag)
			{
				throw new Exception("rootRefers can not be null to create CharacterDetailInfo element!");
			}
			bool flag2 = rootRefers.Names.Contains("CharacterTitle");
			if (flag2)
			{
				this._characterTitle = new CharacterTitleElement(rootRefers.CGet<Refers>("CharacterTitle"));
			}
			bool flag3 = rootRefers.Names.Contains("CharacterGender");
			if (flag3)
			{
				this._characterGender = new CharacterGender(rootRefers.CGet<Refers>("CharacterGender"));
			}
			bool flag4 = rootRefers.Names.Contains("CharacterCharm");
			if (flag4)
			{
				this._characterCharm = new CharacterCharm(rootRefers.CGet<Refers>("CharacterCharm"));
			}
			bool flag5 = rootRefers.Names.Contains("CharacterHappiness");
			if (flag5)
			{
				this._characterHappiness = new CharacterHappiness(rootRefers.CGet<Refers>("CharacterHappiness"), false);
			}
			bool flag6 = rootRefers.Names.Contains("CharacterBehavior");
			if (flag6)
			{
				this._characterBehavior = new CharacterBehavior(rootRefers.CGet<Refers>("CharacterBehavior"), false);
			}
			bool flag7 = rootRefers.Names.Contains("CharacterSamsara");
			if (flag7)
			{
				this._characterSamsaraInfo = new CharacterSamsaraInfo(rootRefers.CGet<Refers>("CharacterSamsara"));
			}
			bool flag8 = rootRefers.Names.Contains("CharacterFame");
			if (flag8)
			{
				this._characterFame = new CharacterFame(rootRefers.CGet<Refers>("CharacterFame"), false);
			}
			Refers organizationRefers = null;
			Refers gradeRefers = null;
			bool flag9 = rootRefers.Names.Contains("CharacterOrganization");
			if (flag9)
			{
				organizationRefers = rootRefers.CGet<Refers>("CharacterOrganization");
			}
			bool flag10 = rootRefers.Names.Contains("CharacterIdentity");
			if (flag10)
			{
				gradeRefers = rootRefers.CGet<Refers>("CharacterIdentity");
			}
			bool flag11 = null != organizationRefers || null != gradeRefers;
			if (flag11)
			{
				this._characterOrganization = new CharacterOrganization(organizationRefers, gradeRefers);
			}
		}

		// Token: 0x06004660 RID: 18016 RVA: 0x0020FDCC File Offset: 0x0020DFCC
		internal override void BindEvent()
		{
			bool flag = this._characterTitle != null;
			if (flag)
			{
				this._characterTitle.BindEvent();
			}
			bool flag2 = this._characterGender != null;
			if (flag2)
			{
				this._characterGender.BindEvent();
			}
			bool flag3 = this._characterCharm != null;
			if (flag3)
			{
				this._characterCharm.BindEvent();
			}
			bool flag4 = this._characterHappiness != null;
			if (flag4)
			{
				this._characterHappiness.BindEvent();
			}
			bool flag5 = this._characterBehavior != null;
			if (flag5)
			{
				this._characterBehavior.BindEvent();
			}
			bool flag6 = this._characterSamsaraInfo != null;
			if (flag6)
			{
				this._characterSamsaraInfo.BindEvent();
			}
			bool flag7 = this._characterOrganization != null;
			if (flag7)
			{
				this._characterOrganization.BindEvent();
			}
			bool flag8 = this._characterFame != null;
			if (flag8)
			{
				this._characterFame.BindEvent();
			}
		}

		// Token: 0x06004661 RID: 18017 RVA: 0x0020FEAC File Offset: 0x0020E0AC
		public override void UnbindEvent()
		{
			bool flag = this._characterTitle != null;
			if (flag)
			{
				this._characterTitle.UnbindEvent();
			}
			bool flag2 = this._characterGender != null;
			if (flag2)
			{
				this._characterGender.UnbindEvent();
			}
			bool flag3 = this._characterCharm != null;
			if (flag3)
			{
				this._characterCharm.UnbindEvent();
			}
			bool flag4 = this._characterHappiness != null;
			if (flag4)
			{
				this._characterHappiness.UnbindEvent();
			}
			bool flag5 = this._characterBehavior != null;
			if (flag5)
			{
				this._characterBehavior.UnbindEvent();
			}
			bool flag6 = this._characterSamsaraInfo != null;
			if (flag6)
			{
				this._characterSamsaraInfo.UnbindEvent();
			}
			bool flag7 = this._characterOrganization != null;
			if (flag7)
			{
				this._characterOrganization.UnbindEvent();
			}
			bool flag8 = this._characterFame != null;
			if (flag8)
			{
				this._characterFame.UnbindEvent();
			}
		}

		// Token: 0x06004662 RID: 18018 RVA: 0x0020FF8C File Offset: 0x0020E18C
		public override void FillElement()
		{
			CharacterTitleElement characterTitle = this._characterTitle;
			if (characterTitle != null)
			{
				characterTitle.FillElement();
			}
			CharacterGender characterGender = this._characterGender;
			if (characterGender != null)
			{
				characterGender.FillElement();
			}
			CharacterCharm characterCharm = this._characterCharm;
			if (characterCharm != null)
			{
				characterCharm.FillElement();
			}
			CharacterHappiness characterHappiness = this._characterHappiness;
			if (characterHappiness != null)
			{
				characterHappiness.FillElement();
			}
			CharacterBehavior characterBehavior = this._characterBehavior;
			if (characterBehavior != null)
			{
				characterBehavior.FillElement();
			}
			CharacterSamsaraInfo characterSamsaraInfo = this._characterSamsaraInfo;
			if (characterSamsaraInfo != null)
			{
				characterSamsaraInfo.FillElement();
			}
			CharacterOrganization characterOrganization = this._characterOrganization;
			if (characterOrganization != null)
			{
				characterOrganization.FillElement();
			}
			CharacterFame characterFame = this._characterFame;
			if (characterFame != null)
			{
				characterFame.FillElement();
			}
		}

		// Token: 0x06004663 RID: 18019 RVA: 0x0021002C File Offset: 0x0020E22C
		public override void ResetToEmpty()
		{
			bool flag = this._characterTitle != null;
			if (flag)
			{
				this._characterTitle.CharacterId = -1;
			}
			bool flag2 = this._characterGender != null;
			if (flag2)
			{
				this._characterGender.CharacterId = -1;
			}
			bool flag3 = this._characterCharm != null;
			if (flag3)
			{
				this._characterCharm.CharacterId = -1;
			}
			bool flag4 = this._characterHappiness != null;
			if (flag4)
			{
				this._characterHappiness.CharacterId = -1;
			}
			bool flag5 = this._characterBehavior != null;
			if (flag5)
			{
				this._characterBehavior.CharacterId = -1;
			}
			bool flag6 = this._characterSamsaraInfo != null;
			if (flag6)
			{
				this._characterSamsaraInfo.CharacterId = -1;
			}
			bool flag7 = this._characterOrganization != null;
			if (flag7)
			{
				this._characterOrganization.CharacterId = -1;
			}
			bool flag8 = this._characterFame != null;
			if (flag8)
			{
				this._characterFame.CharacterId = -1;
			}
		}

		// Token: 0x06004664 RID: 18020 RVA: 0x00210114 File Offset: 0x0020E314
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			bool flag = this._characterTitle != null;
			if (flag)
			{
				this._characterTitle.CharacterId = charId;
			}
			bool flag2 = this._characterGender != null;
			if (flag2)
			{
				this._characterGender.CharacterId = charId;
			}
			bool flag3 = this._characterCharm != null;
			if (flag3)
			{
				this._characterCharm.CharacterId = charId;
			}
			bool flag4 = this._characterHappiness != null;
			if (flag4)
			{
				this._characterHappiness.CharacterId = charId;
			}
			bool flag5 = this._characterBehavior != null;
			if (flag5)
			{
				this._characterBehavior.CharacterId = charId;
			}
			bool flag6 = this._characterSamsaraInfo != null;
			if (flag6)
			{
				this._characterSamsaraInfo.CharacterId = charId;
			}
			bool flag7 = this._characterOrganization != null;
			if (flag7)
			{
				this._characterOrganization.CharacterId = charId;
			}
			bool flag8 = this._characterFame != null;
			if (flag8)
			{
				this._characterFame.CharacterId = charId;
			}
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DetailInfoMonitor>(charId, this.IsDead);
		}

		// Token: 0x040030C0 RID: 12480
		private readonly CharacterTitleElement _characterTitle;

		// Token: 0x040030C1 RID: 12481
		private readonly CharacterGender _characterGender;

		// Token: 0x040030C2 RID: 12482
		private readonly CharacterCharm _characterCharm;

		// Token: 0x040030C3 RID: 12483
		private readonly CharacterHappiness _characterHappiness;

		// Token: 0x040030C4 RID: 12484
		private readonly CharacterBehavior _characterBehavior;

		// Token: 0x040030C5 RID: 12485
		private readonly CharacterOrganization _characterOrganization;

		// Token: 0x040030C6 RID: 12486
		private readonly CharacterSamsaraInfo _characterSamsaraInfo;

		// Token: 0x040030C7 RID: 12487
		private readonly CharacterFame _characterFame;
	}
}
