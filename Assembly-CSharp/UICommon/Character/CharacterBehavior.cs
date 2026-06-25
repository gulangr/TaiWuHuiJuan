using System;
using CharacterDataMonitor;
using Config;
using FrameWork;
using GameData.Domains.Character;
using UICommon.Character.Elements;

namespace UICommon.Character
{
	// Token: 0x020005C8 RID: 1480
	public class CharacterBehavior : CharacterUIElement
	{
		// Token: 0x170008DA RID: 2266
		// (get) Token: 0x0600463A RID: 17978 RVA: 0x0020EFB9 File Offset: 0x0020D1B9
		private DetailInfoMonitor Item
		{
			get
			{
				return this.MonitorDataItem as DetailInfoMonitor;
			}
		}

		// Token: 0x0600463B RID: 17979 RVA: 0x0020EFC8 File Offset: 0x0020D1C8
		public CharacterBehavior(Refers refers, bool tipType = false)
		{
			bool flag = refers == null;
			if (flag)
			{
				throw new Exception("refers can not be null to create CharacterBehavior element!");
			}
			this._refers = refers;
			this._tipType = tipType;
			this._infoItem = new InfoItem(refers);
			string infoName = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Behavior);
			string infoDesc = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Behavior_TipContent);
			this._infoItem.SetInfoName(infoName);
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

		// Token: 0x0600463C RID: 17980 RVA: 0x0020F083 File Offset: 0x0020D283
		internal override void BindEvent()
		{
			this.Item.AddOnBehaviorListener(new Action(this.FillElement));
		}

		// Token: 0x0600463D RID: 17981 RVA: 0x0020F09F File Offset: 0x0020D29F
		public override void UnbindEvent()
		{
			this.Item.RemoveOnBehaviorListener(new Action(this.FillElement));
		}

		// Token: 0x0600463E RID: 17982 RVA: 0x0020F0BC File Offset: 0x0020D2BC
		public override void FillElement()
		{
			bool flag = !this._infoItem.HasValidElement();
			if (flag)
			{
				base.CharacterId = -1;
			}
			else
			{
				bool flag2 = this.Item == null || !this.Item.Init;
				if (!flag2)
				{
					sbyte behaviorType = GameData.Domains.Character.BehaviorType.GetBehaviorType(this.Item.Behavior);
					BehaviorTypeItem config = Config.BehaviorType.Instance.GetItem((short)behaviorType);
					string behaviorName = CommonUtils.GetBehaviorString(behaviorType);
					this._infoItem.SetInfoValue(behaviorName);
					this._infoItem.SetIcon(this.UseNewIcon ? ("ui9_icon_behavior_type_" + behaviorType.ToString()) : config.Icon);
					bool useNewIcon = this.UseNewIcon;
					if (useNewIcon)
					{
						TooltipInvoker mouseTip = this._refers.gameObject.GetOrAddComponent<TooltipInvoker>();
						mouseTip.triggerByChildRaycast = true;
						mouseTip.Type = TipType.SingleDesc;
						mouseTip.RuntimeParam = new ArgumentBox();
						mouseTip.RuntimeParam.Set("arg0", LocalStringManager.GetFormat(LanguageKey.UI_EventWindow_Behavior_Type_Tips, behaviorName));
					}
					bool tipType = this._tipType;
					if (tipType)
					{
						this._infoItem.GetMouseTip().PresetParam[1] = CommonUtils.GetBehaviorString(behaviorType);
					}
				}
			}
		}

		// Token: 0x0600463F RID: 17983 RVA: 0x0020F1EC File Offset: 0x0020D3EC
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
			else
			{
				this._infoItem.SetInfoValue(string.Empty);
			}
		}

		// Token: 0x06004640 RID: 17984 RVA: 0x0020F240 File Offset: 0x0020D440
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DetailInfoMonitor>(charId, this.IsDead);
		}

		// Token: 0x040030B2 RID: 12466
		private readonly InfoItem _infoItem;

		// Token: 0x040030B3 RID: 12467
		private bool _tipType = false;

		// Token: 0x040030B4 RID: 12468
		private Refers _refers;

		// Token: 0x040030B5 RID: 12469
		public bool UseNewIcon = false;
	}
}
