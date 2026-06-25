using System;
using CharacterDataMonitor;
using FrameWork;
using TMPro;

namespace UICommon.Character
{
	// Token: 0x020005CC RID: 1484
	public class CharacterConsummateLevel : CharacterUIElement
	{
		// Token: 0x170008DE RID: 2270
		// (get) Token: 0x06004658 RID: 18008 RVA: 0x0020F9A0 File Offset: 0x0020DBA0
		private ConsummateLevelMonitor Item
		{
			get
			{
				return this.MonitorDataItem as ConsummateLevelMonitor;
			}
		}

		// Token: 0x06004659 RID: 18009 RVA: 0x0020F9AD File Offset: 0x0020DBAD
		public CharacterConsummateLevel(CImage icon, TextMeshProUGUI labelLevel, TextMeshProUGUI labelName)
		{
			this._iconImg = icon;
			this._levelLabel = labelLevel;
			this._levelNameLabel = labelName;
		}

		// Token: 0x0600465A RID: 18010 RVA: 0x0020F9CC File Offset: 0x0020DBCC
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<ConsummateLevelMonitor>(charId, this.IsDead);
		}

		// Token: 0x0600465B RID: 18011 RVA: 0x0020F9EF File Offset: 0x0020DBEF
		internal override void BindEvent()
		{
			this.Item.AddConsummateLevelListener(new Action(this.FillElement));
		}

		// Token: 0x0600465C RID: 18012 RVA: 0x0020FA0B File Offset: 0x0020DC0B
		public override void UnbindEvent()
		{
			this.Item.RemoveConsummateLevelListener(new Action(this.FillElement));
		}

		// Token: 0x0600465D RID: 18013 RVA: 0x0020FA28 File Offset: 0x0020DC28
		public override void FillElement()
		{
			ValueTuple<string, string> consummateLevelShowData = CommonUtils.GetConsummateLevelShowData(this.Item.Level);
			string iconName = consummateLevelShowData.Item1;
			string levelName = consummateLevelShowData.Item2;
			bool isIntelligentChar = this.Item.CreatingType == 1;
			bool flag = null != this._iconImg;
			if (flag)
			{
				this._iconImg.SetSprite(isIntelligentChar ? iconName : string.Format("ui9_icon_consummate_level_big_{0}", 0), false, null);
				TooltipInvoker mouseTip = this._iconImg.gameObject.GetOrAddComponent<TooltipInvoker>();
				mouseTip.triggerByChildRaycast = true;
				mouseTip.Type = TipType.SingleDesc;
				mouseTip.RuntimeParam = new ArgumentBox();
				mouseTip.RuntimeParam.Set("arg0", LocalStringManager.GetFormat(LanguageKey.UI_EventWindow_Consummate_Level_Tips, isIntelligentChar ? levelName : LocalStringManager.Get(LanguageKey.LK_Unknow)));
			}
			bool flag2 = null != this._levelLabel;
			if (flag2)
			{
				this._levelLabel.text = (isIntelligentChar ? this.Item.Level.ToString() : "?".SetColor("grey"));
			}
			bool flag3 = null != this._levelNameLabel;
			if (flag3)
			{
				this._levelNameLabel.text = (isIntelligentChar ? levelName : LocalStringManager.Get(LanguageKey.LK_Unknow));
			}
		}

		// Token: 0x0600465E RID: 18014 RVA: 0x0020FB6C File Offset: 0x0020DD6C
		public override void ResetToEmpty()
		{
			bool flag = null != this._iconImg;
			if (flag)
			{
				this._iconImg.SetSprite(string.Format("ui9_icon_consummate_level_big_{0}", 0), false, null);
			}
			bool flag2 = null != this._levelLabel;
			if (flag2)
			{
				this._levelLabel.text = "?".SetColor("grey");
			}
			bool flag3 = null != this._levelNameLabel;
			if (flag3)
			{
				this._levelNameLabel.text = LocalStringManager.Get(LanguageKey.LK_Unknow);
			}
		}

		// Token: 0x040030BD RID: 12477
		private CImage _iconImg;

		// Token: 0x040030BE RID: 12478
		private TextMeshProUGUI _levelLabel;

		// Token: 0x040030BF RID: 12479
		private TextMeshProUGUI _levelNameLabel;
	}
}
