using System;
using CharacterDataMonitor;

namespace UICommon.Character
{
	// Token: 0x020005DC RID: 1500
	public class CharacterLifeSkillScroll : CharacterUIElement
	{
		// Token: 0x170008F1 RID: 2289
		// (get) Token: 0x060046E4 RID: 18148 RVA: 0x002133B3 File Offset: 0x002115B3
		private LifeSkillMonitor Item
		{
			get
			{
				return this.MonitorDataItem as LifeSkillMonitor;
			}
		}

		// Token: 0x060046E5 RID: 18149 RVA: 0x002133C0 File Offset: 0x002115C0
		public CharacterLifeSkillScroll(InfinityScrollLegacy scroll, LifeOrCombatSkillScrollItemType fillType, ESkillIconType iconType)
		{
			bool flag = null == scroll;
			if (flag)
			{
				throw new Exception("can not create CharacterLifeSkillScroll with null InfinityScroll");
			}
			this._fillType = fillType;
			this._iconType = iconType;
			this._scroll = scroll;
			this._scroll.OnItemRender = new Action<int, Refers>(this.OnLifeSkillItemRender);
			this._scroll.SetDataCount(0);
		}

		// Token: 0x060046E6 RID: 18150 RVA: 0x00213424 File Offset: 0x00211624
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<LifeSkillMonitor>(charId, this.IsDead);
		}

		// Token: 0x060046E7 RID: 18151 RVA: 0x00213447 File Offset: 0x00211647
		internal override void BindEvent()
		{
			this.Item.AddQualificationsListener(new Action(this.FillElement));
		}

		// Token: 0x060046E8 RID: 18152 RVA: 0x00213463 File Offset: 0x00211663
		public override void UnbindEvent()
		{
			this.Item.RemoveQualificationsListener(new Action(this.FillElement));
		}

		// Token: 0x060046E9 RID: 18153 RVA: 0x00213480 File Offset: 0x00211680
		public override void FillElement()
		{
			bool flag = null == this._scroll;
			if (flag)
			{
				base.CharacterId = -1;
			}
			else
			{
				this._scroll.OnItemRender = new Action<int, Refers>(this.OnLifeSkillItemRender);
				this._scroll.UpdateData(16);
			}
		}

		// Token: 0x060046EA RID: 18154 RVA: 0x002134D0 File Offset: 0x002116D0
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

		// Token: 0x060046EB RID: 18155 RVA: 0x0021351C File Offset: 0x0021171C
		private void OnLifeSkillItemRender(int index, Refers refers)
		{
			UIItemLifeSkill skillItem = refers as UIItemLifeSkill;
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

		// Token: 0x0400310B RID: 12555
		private readonly InfinityScrollLegacy _scroll;

		// Token: 0x0400310C RID: 12556
		private readonly LifeOrCombatSkillScrollItemType _fillType;

		// Token: 0x0400310D RID: 12557
		private readonly ESkillIconType _iconType;
	}
}
