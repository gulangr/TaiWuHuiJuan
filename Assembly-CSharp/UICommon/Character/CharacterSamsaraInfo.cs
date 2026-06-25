using System;
using CharacterDataMonitor;
using UICommon.Character.Elements;

namespace UICommon.Character
{
	// Token: 0x020005E7 RID: 1511
	public class CharacterSamsaraInfo : CharacterUIElement
	{
		// Token: 0x170008FE RID: 2302
		// (get) Token: 0x06004755 RID: 18261 RVA: 0x002168F8 File Offset: 0x00214AF8
		private DetailInfoMonitor Item
		{
			get
			{
				return this.MonitorDataItem as DetailInfoMonitor;
			}
		}

		// Token: 0x06004756 RID: 18262 RVA: 0x00216908 File Offset: 0x00214B08
		public CharacterSamsaraInfo(Refers refers)
		{
			bool flag = refers == null;
			if (flag)
			{
				throw new Exception("refers can not be null to create CharacterSamsaraInfo element!");
			}
			this._infoItem = new InfoItem(refers);
			string infoName = LocalStringManager.Get(LanguageKey.LK_Samsara);
			string infoDesc = LocalStringManager.Get(LanguageKey.LK_Samsara_TipContent);
			this._infoItem.SetInfoName(infoName);
			TooltipInvoker mouseTip = this._infoItem.GetMouseTip();
			bool flag2 = null != mouseTip;
			if (flag2)
			{
				mouseTip.Type = TipType.Simple;
				mouseTip.enabled = true;
				mouseTip.IsLanguageKey = false;
				mouseTip.PresetParam = new string[]
				{
					infoName,
					infoDesc
				};
			}
		}

		// Token: 0x06004757 RID: 18263 RVA: 0x002169A7 File Offset: 0x00214BA7
		internal override void BindEvent()
		{
			this.Item.AddOnSamsaraListener(new Action(this.FillElement));
		}

		// Token: 0x06004758 RID: 18264 RVA: 0x002169C3 File Offset: 0x00214BC3
		public override void UnbindEvent()
		{
			this.Item.RemoveOnSamsaraListener(new Action(this.FillElement));
		}

		// Token: 0x06004759 RID: 18265 RVA: 0x002169E0 File Offset: 0x00214BE0
		public override void FillElement()
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
			else
			{
				bool flag3 = this.Item == null || !this.Item.Init;
				if (!flag3)
				{
					this._infoItem.SetInfoValue(this.Item.PreexistenceCharIds.Count.ToString());
				}
			}
		}

		// Token: 0x0600475A RID: 18266 RVA: 0x00216A63 File Offset: 0x00214C63
		public override void ResetToEmpty()
		{
			this._infoItem.SetInfoValue(string.Empty);
		}

		// Token: 0x0600475B RID: 18267 RVA: 0x00216A78 File Offset: 0x00214C78
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DetailInfoMonitor>(charId, this.IsDead);
		}

		// Token: 0x04003143 RID: 12611
		private readonly InfoItem _infoItem;
	}
}
