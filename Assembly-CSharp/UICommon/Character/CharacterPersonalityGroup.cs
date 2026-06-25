using System;
using CharacterDataMonitor;

namespace UICommon.Character
{
	// Token: 0x020005E3 RID: 1507
	public class CharacterPersonalityGroup : CharacterUIElement
	{
		// Token: 0x06004728 RID: 18216 RVA: 0x002153A8 File Offset: 0x002135A8
		public CharacterPersonalityGroup(Refers refers)
		{
			bool flag = refers == null;
			if (flag)
			{
				throw new Exception("refers can not be null to create CharacterPersonality element!");
			}
			this._personalities = new CharacterPersonality[7];
			for (sbyte i = 0; i < 7; i += 1)
			{
				bool flag2 = refers.Names.Contains(string.Format("PersonalityItem{0}", i));
				if (flag2)
				{
					this._personalities[(int)i] = new CharacterPersonality((ushort)i, refers.CGet<Refers>(string.Format("PersonalityItem{0}", i)));
				}
			}
		}

		// Token: 0x06004729 RID: 18217 RVA: 0x00215438 File Offset: 0x00213638
		internal override void BindEvent()
		{
			foreach (CharacterPersonality item in this._personalities)
			{
				bool flag = item != null;
				if (flag)
				{
					item.CharacterId = base.CharacterId;
				}
			}
		}

		// Token: 0x0600472A RID: 18218 RVA: 0x00215478 File Offset: 0x00213678
		public override void UnbindEvent()
		{
			foreach (CharacterPersonality item in this._personalities)
			{
				bool flag = item != null;
				if (flag)
				{
					item.CharacterId = -1;
				}
			}
		}

		// Token: 0x0600472B RID: 18219 RVA: 0x002154B4 File Offset: 0x002136B4
		public override void FillElement()
		{
			foreach (CharacterPersonality item in this._personalities)
			{
				if (item != null)
				{
					item.FillElement();
				}
			}
		}

		// Token: 0x0600472C RID: 18220 RVA: 0x002154EC File Offset: 0x002136EC
		public override void ResetToEmpty()
		{
			foreach (CharacterPersonality item in this._personalities)
			{
				bool flag = item != null;
				if (flag)
				{
					item.CharacterId = -1;
				}
			}
		}

		// Token: 0x0600472D RID: 18221 RVA: 0x00215528 File Offset: 0x00213728
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<PersonalityMonitor>(charId, this.IsDead);
		}

		// Token: 0x04003128 RID: 12584
		private readonly CharacterPersonality[] _personalities;
	}
}
