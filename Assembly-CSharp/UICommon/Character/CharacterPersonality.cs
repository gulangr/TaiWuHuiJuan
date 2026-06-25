using System;
using CharacterDataMonitor;
using DisplayConfig;
using UICommon.Character.Elements;

namespace UICommon.Character
{
	// Token: 0x020005E2 RID: 1506
	public class CharacterPersonality : CharacterUIElement
	{
		// Token: 0x170008F7 RID: 2295
		// (get) Token: 0x06004720 RID: 18208 RVA: 0x0021519D File Offset: 0x0021339D
		private PersonalityMonitor Item
		{
			get
			{
				return this.MonitorDataItem as PersonalityMonitor;
			}
		}

		// Token: 0x06004721 RID: 18209 RVA: 0x002151AC File Offset: 0x002133AC
		public CharacterPersonality(ushort templateId, Refers refers)
		{
			bool flag = refers == null;
			if (flag)
			{
				throw new Exception("refers can not be null to create CharacterPersonality element!");
			}
			this._config = Personality.Instance[(int)templateId];
			this._infoItem = new InfoItem(refers);
			this._infoItem.SetIcon(this._config.Icon);
			string infoName = this._config.Name;
			string infoDesc = this._config.Desc;
			TooltipInvoker mouseTip = this._infoItem.GetMouseTip();
			bool flag2 = null != mouseTip;
			if (flag2)
			{
				mouseTip.enabled = true;
				mouseTip.IsLanguageKey = false;
				mouseTip.Type = TipType.Simple;
				mouseTip.PresetParam = new string[]
				{
					infoName,
					infoDesc
				};
			}
		}

		// Token: 0x06004722 RID: 18210 RVA: 0x00215268 File Offset: 0x00213468
		internal override void BindEvent()
		{
			this.Item.AddPersonalityListener(new Action<sbyte>(this.FillValue));
		}

		// Token: 0x06004723 RID: 18211 RVA: 0x00215283 File Offset: 0x00213483
		public override void UnbindEvent()
		{
			this.Item.RemovePersonalityListener(new Action<sbyte>(this.FillValue));
		}

		// Token: 0x06004724 RID: 18212 RVA: 0x002152A0 File Offset: 0x002134A0
		public override void FillElement()
		{
			bool flag = !this._infoItem.HasValidElement();
			if (flag)
			{
				base.CharacterId = -1;
			}
			else
			{
				this.FillValue(this._config.TemplateId);
			}
		}

		// Token: 0x06004725 RID: 18213 RVA: 0x002152E0 File Offset: 0x002134E0
		public override void ResetToEmpty()
		{
			bool flag = !this._infoItem.HasValidElement();
			if (flag)
			{
				bool flag2 = this.MonitorDataItem != null;
				if (flag2)
				{
					this.UnbindEvent();
					this.MonitorDataItem = null;
				}
			}
		}

		// Token: 0x06004726 RID: 18214 RVA: 0x00215320 File Offset: 0x00213520
		private void FillValue(sbyte type)
		{
			bool flag = type != this._config.TemplateId && type != 7;
			if (!flag)
			{
				bool flag2 = type == 7;
				if (flag2)
				{
					type = this._config.TemplateId;
				}
				this._infoItem.SetInfoValue(this.Item.Personalities[(int)type].ToString());
			}
		}

		// Token: 0x06004727 RID: 18215 RVA: 0x00215384 File Offset: 0x00213584
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<PersonalityMonitor>(charId, this.IsDead);
		}

		// Token: 0x04003126 RID: 12582
		private readonly InfoItem _infoItem;

		// Token: 0x04003127 RID: 12583
		private readonly PersonalityItem _config;
	}
}
