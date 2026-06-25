using System;
using CharacterDataMonitor;
using Config;
using UICommon.Character.Elements;

namespace UICommon.Character
{
	// Token: 0x020005DA RID: 1498
	public class CharacterLifeSkillAttainmentGroup : CharacterUIElement
	{
		// Token: 0x170008F0 RID: 2288
		// (get) Token: 0x060046DD RID: 18141 RVA: 0x002131DF File Offset: 0x002113DF
		private LifeSkillMonitor Item
		{
			get
			{
				return this.MonitorDataItem as LifeSkillMonitor;
			}
		}

		// Token: 0x060046DE RID: 18142 RVA: 0x002131EC File Offset: 0x002113EC
		public CharacterLifeSkillAttainmentGroup(Refers refers)
		{
			bool flag = refers == null;
			if (flag)
			{
				throw new Exception("refers can not be null to create CharacterPersonality element!");
			}
			this._lifeSkills = new InfoItem[LifeSkillType.Instance.Count];
			sbyte i = 0;
			while ((int)i < LifeSkillType.Instance.Count)
			{
				bool flag2 = refers.Names.Contains(string.Format("LifeSkill_{0}", i));
				if (flag2)
				{
					LifeSkillTypeItem config = LifeSkillType.Instance.GetItem(i);
					this._lifeSkills[(int)i] = new InfoItem(refers.CGet<Refers>(string.Format("LifeSkill_{0}", i)));
					this._lifeSkills[(int)i].SetInfoName(config.Name);
					this._lifeSkills[(int)i].SetIcon(config.Icon);
				}
				i += 1;
			}
		}

		// Token: 0x060046DF RID: 18143 RVA: 0x002132C5 File Offset: 0x002114C5
		internal override void BindEvent()
		{
			this.Item.AddAttainmentListener(new Action(this.FillElement));
		}

		// Token: 0x060046E0 RID: 18144 RVA: 0x002132E1 File Offset: 0x002114E1
		public override void UnbindEvent()
		{
			this.Item.RemoveAttainmentListener(new Action(this.FillElement));
		}

		// Token: 0x060046E1 RID: 18145 RVA: 0x00213300 File Offset: 0x00211500
		public override void FillElement()
		{
			for (int i = 0; i < this._lifeSkills.Length; i++)
			{
				InfoItem infoItem = this._lifeSkills[i];
				if (infoItem != null)
				{
					infoItem.SetInfoValue(this.Item.Attainments[i].ToString());
				}
			}
		}

		// Token: 0x060046E2 RID: 18146 RVA: 0x00213354 File Offset: 0x00211554
		public override void ResetToEmpty()
		{
			foreach (InfoItem item in this._lifeSkills)
			{
				if (item != null)
				{
					item.SetInfoValue("");
				}
			}
		}

		// Token: 0x060046E3 RID: 18147 RVA: 0x00213390 File Offset: 0x00211590
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<LifeSkillMonitor>(charId, this.IsDead);
		}

		// Token: 0x04003106 RID: 12550
		private readonly InfoItem[] _lifeSkills;
	}
}
