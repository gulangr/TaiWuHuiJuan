using System;
using CharacterDataMonitor;
using TMPro;

namespace UICommon.Character
{
	// Token: 0x020005D8 RID: 1496
	public class CharacterIdentity : CharacterUIElement
	{
		// Token: 0x170008EE RID: 2286
		// (get) Token: 0x060046C9 RID: 18121 RVA: 0x00212A42 File Offset: 0x00210C42
		private DetailInfoMonitor Item
		{
			get
			{
				return this.MonitorDataItem as DetailInfoMonitor;
			}
		}

		// Token: 0x060046CA RID: 18122 RVA: 0x00212A4F File Offset: 0x00210C4F
		public CharacterIdentity(TMP_Text identityText, CImage identityIcon = null)
		{
			if (identityText == null)
			{
				throw new ArgumentNullException("identityText");
			}
			this._identityText = identityText;
			this._identityIcon = identityIcon;
		}

		// Token: 0x060046CB RID: 18123 RVA: 0x00212A78 File Offset: 0x00210C78
		internal override void BindEvent()
		{
			this._basicMonitor.AddGenderListener(new Action(this.FillElement));
			this._ageHealthMonitor.AddActualAgeListener(new Action(this.FillElement));
			this.Item.AddOnOrganizationInfoListener(new Action(this.FillElement));
		}

		// Token: 0x060046CC RID: 18124 RVA: 0x00212AD4 File Offset: 0x00210CD4
		public override void UnbindEvent()
		{
			this._basicMonitor.RemoveGenderListener(new Action(this.FillElement));
			this._ageHealthMonitor.RemoveActualAgeListener(new Action(this.FillElement));
			this.Item.RemoveOnOrganizationInfoListener(new Action(this.FillElement));
		}

		// Token: 0x060046CD RID: 18125 RVA: 0x00212B30 File Offset: 0x00210D30
		public override void FillElement()
		{
			bool flag = this._identityText == null;
			if (flag)
			{
				base.CharacterId = -1;
			}
			else
			{
				bool flag2 = this.Item == null || !this.Item.Init || !this._basicMonitor.Init || !this._ageHealthMonitor.Init;
				if (!flag2)
				{
					bool flag3 = this._identityIcon != null;
					if (flag3)
					{
						this._identityIcon.SetSprite(CommonUtils.GetIdentityIcon(this.Item.OrganizationInfo.Grade), false, null);
					}
					string identityString = CommonUtils.GetIdentityStringWithSpecialCharacterConfig((int)this._ageHealthMonitor.TemplateId, this.Item.OrganizationInfo, this._basicMonitor.Gender, this._ageHealthMonitor.PhysiologicalAge, this.Item.IsReclusiveChar);
					this._identityText.text = identityString;
				}
			}
		}

		// Token: 0x060046CE RID: 18126 RVA: 0x00212C14 File Offset: 0x00210E14
		public override void ResetToEmpty()
		{
			bool flag = this._identityText != null;
			if (flag)
			{
				this._identityText.text = string.Empty;
			}
			bool flag2 = this.MonitorDataItem != null;
			if (flag2)
			{
				this.UnbindEvent();
				this.MonitorDataItem = null;
			}
		}

		// Token: 0x060046CF RID: 18127 RVA: 0x00212C64 File Offset: 0x00210E64
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			bool flag = this._basicMonitor == null || this._basicMonitor.CharacterId != charId;
			if (flag)
			{
				this._basicMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<BasicInfoMonitor>(charId, this.IsDead);
			}
			bool flag2 = this._ageHealthMonitor == null || this._ageHealthMonitor.CharacterId != charId;
			if (flag2)
			{
				this._ageHealthMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<AgeHealthMonitor>(charId, this.IsDead);
			}
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DetailInfoMonitor>(charId, this.IsDead);
		}

		// Token: 0x040030FE RID: 12542
		private AgeHealthMonitor _ageHealthMonitor;

		// Token: 0x040030FF RID: 12543
		private BasicInfoMonitor _basicMonitor;

		// Token: 0x04003100 RID: 12544
		private readonly TMP_Text _identityText;

		// Token: 0x04003101 RID: 12545
		private readonly CImage _identityIcon;
	}
}
