using System;
using CharacterDataMonitor;
using Config;
using UICommon.Character.Elements;

namespace UICommon.Character
{
	// Token: 0x020005CA RID: 1482
	public class CharacterCombatSkillAttainmentGroup : CharacterUIElement
	{
		// Token: 0x170008DC RID: 2268
		// (get) Token: 0x06004649 RID: 17993 RVA: 0x0020F5B1 File Offset: 0x0020D7B1
		private CombatSkillMonitor Item
		{
			get
			{
				return this.MonitorDataItem as CombatSkillMonitor;
			}
		}

		// Token: 0x0600464A RID: 17994 RVA: 0x0020F5C0 File Offset: 0x0020D7C0
		public CharacterCombatSkillAttainmentGroup(Refers refers)
		{
			bool flag = refers == null;
			if (flag)
			{
				throw new Exception("refers can not be null to create CharacterPersonality element!");
			}
			this._combatSkills = new InfoItem[CombatSkillType.Instance.Count];
			sbyte i = 0;
			while ((int)i < CombatSkillType.Instance.Count)
			{
				bool flag2 = refers.Names.Contains(string.Format("CombatSkill_{0}", i));
				if (flag2)
				{
					CombatSkillTypeItem config = CombatSkillType.Instance.GetItem(i);
					this._combatSkills[(int)i] = new InfoItem(refers.CGet<Refers>(string.Format("CombatSkill_{0}", i)));
					this._combatSkills[(int)i].SetInfoName(config.Name);
					this._combatSkills[(int)i].SetIcon(config.Icon);
				}
				i += 1;
			}
		}

		// Token: 0x0600464B RID: 17995 RVA: 0x0020F699 File Offset: 0x0020D899
		internal override void BindEvent()
		{
			this.Item.AddAttainmentListener(new Action(this.FillElement));
		}

		// Token: 0x0600464C RID: 17996 RVA: 0x0020F6B5 File Offset: 0x0020D8B5
		public override void UnbindEvent()
		{
			this.Item.RemoveAttainmentListener(new Action(this.FillElement));
		}

		// Token: 0x0600464D RID: 17997 RVA: 0x0020F6D4 File Offset: 0x0020D8D4
		public override void FillElement()
		{
			for (int i = 0; i < this._combatSkills.Length; i++)
			{
				InfoItem infoItem = this._combatSkills[i];
				if (infoItem != null)
				{
					infoItem.SetInfoValue(this.Item.Attainments[i].ToString());
				}
			}
		}

		// Token: 0x0600464E RID: 17998 RVA: 0x0020F728 File Offset: 0x0020D928
		public override void ResetToEmpty()
		{
			foreach (InfoItem item in this._combatSkills)
			{
				if (item != null)
				{
					item.SetInfoValue("");
				}
			}
		}

		// Token: 0x0600464F RID: 17999 RVA: 0x0020F764 File Offset: 0x0020D964
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<CombatSkillMonitor>(charId, this.IsDead);
		}

		// Token: 0x040030B9 RID: 12473
		private readonly InfoItem[] _combatSkills;
	}
}
