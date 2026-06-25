using System;
using CharacterDataMonitor;

namespace UICommon.Character
{
	// Token: 0x020005CB RID: 1483
	public class CharacterCombatSKillScroll : CharacterUIElement
	{
		// Token: 0x170008DD RID: 2269
		// (get) Token: 0x06004650 RID: 18000 RVA: 0x0020F787 File Offset: 0x0020D987
		private CombatSkillMonitor Item
		{
			get
			{
				return this.MonitorDataItem as CombatSkillMonitor;
			}
		}

		// Token: 0x06004651 RID: 18001 RVA: 0x0020F794 File Offset: 0x0020D994
		public CharacterCombatSKillScroll(InfinityScrollLegacy scroll, LifeOrCombatSkillScrollItemType fillType, ESkillIconType iconType)
		{
			bool flag = null == scroll;
			if (flag)
			{
				throw new Exception("can not create CharacterCombatSKillScroll with null InfinityScroll");
			}
			this._fillType = fillType;
			this._iconType = iconType;
			this._scroll = scroll;
			this._scroll.OnItemRender = new Action<int, Refers>(this.OnCombatSkillItemRender);
			this._scroll.SetDataCount(0);
		}

		// Token: 0x06004652 RID: 18002 RVA: 0x0020F7F8 File Offset: 0x0020D9F8
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<CombatSkillMonitor>(charId, this.IsDead);
		}

		// Token: 0x06004653 RID: 18003 RVA: 0x0020F81B File Offset: 0x0020DA1B
		internal override void BindEvent()
		{
			this.Item.AddQualificationsListener(new Action(this.FillElement));
		}

		// Token: 0x06004654 RID: 18004 RVA: 0x0020F837 File Offset: 0x0020DA37
		public override void UnbindEvent()
		{
			this.Item.RemoveQualificationsListener(new Action(this.FillElement));
		}

		// Token: 0x06004655 RID: 18005 RVA: 0x0020F854 File Offset: 0x0020DA54
		public override void FillElement()
		{
			bool flag = null == this._scroll;
			if (flag)
			{
				base.CharacterId = -1;
			}
			else
			{
				this._scroll.OnItemRender = new Action<int, Refers>(this.OnCombatSkillItemRender);
				this._scroll.UpdateData(14);
			}
		}

		// Token: 0x06004656 RID: 18006 RVA: 0x0020F8A4 File Offset: 0x0020DAA4
		public override void ResetToEmpty()
		{
			bool flag = null == this._scroll;
			if (flag)
			{
				bool flag2 = this.MonitorDataItem != null;
				if (flag2)
				{
					this.UnbindEvent();
					this.MonitorDataItem = null;
				}
			}
			else
			{
				this._scroll.UpdateData(0);
			}
		}

		// Token: 0x06004657 RID: 18007 RVA: 0x0020F8F0 File Offset: 0x0020DAF0
		private void OnCombatSkillItemRender(int index, Refers refers)
		{
			UIItemCombatSkill skillItem = refers as UIItemCombatSkill;
			bool flag = null == skillItem;
			if (!flag)
			{
				sbyte templateId = (sbyte)index;
				bool flag2 = this._fillType == LifeOrCombatSkillScrollItemType.AttainmentOnly;
				if (flag2)
				{
					skillItem.Refresh(templateId, short.MinValue, this.Item.Attainments[index], this._iconType);
				}
				else
				{
					bool flag3 = this._fillType == LifeOrCombatSkillScrollItemType.QualificationOnly;
					if (flag3)
					{
						skillItem.Refresh(templateId, this.Item.Qualifications[index], short.MinValue, this._iconType);
					}
					else
					{
						skillItem.Refresh(templateId, this.Item.Qualifications[index], this.Item.Attainments[index], this._iconType);
					}
				}
			}
		}

		// Token: 0x040030BA RID: 12474
		private readonly InfinityScrollLegacy _scroll;

		// Token: 0x040030BB RID: 12475
		private readonly LifeOrCombatSkillScrollItemType _fillType;

		// Token: 0x040030BC RID: 12476
		private readonly ESkillIconType _iconType;
	}
}
