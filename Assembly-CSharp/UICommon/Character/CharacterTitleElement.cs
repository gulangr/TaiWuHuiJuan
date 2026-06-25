using System;
using CharacterDataMonitor;
using Config;
using TMPro;
using UICommon.Character.Elements;

namespace UICommon.Character
{
	// Token: 0x020005EB RID: 1515
	public class CharacterTitleElement : CharacterUIElement
	{
		// Token: 0x17000903 RID: 2307
		// (get) Token: 0x06004781 RID: 18305 RVA: 0x0021809D File Offset: 0x0021629D
		private BasicInfoMonitor Item
		{
			get
			{
				return this.MonitorDataItem as BasicInfoMonitor;
			}
		}

		// Token: 0x06004782 RID: 18306 RVA: 0x002180AC File Offset: 0x002162AC
		public CharacterTitleElement(TextMeshProUGUI label, TooltipInvoker mouseTip = null)
		{
			bool flag = null == label;
			if (flag)
			{
				throw new Exception("can not handle CharacterTitleElement for null elements");
			}
			this._infoItem = new InfoItem(null, null, label, null, mouseTip);
			this._infoItem.SetInfoValue(string.Empty);
			this._mouseTip = mouseTip;
			bool flag2 = null != mouseTip;
			if (flag2)
			{
				this._mouseTip.enabled = true;
				this._mouseTip.IsLanguageKey = false;
				this._mouseTip.Type = TipType.Simple;
			}
			CharacterTitleElement._titleNoneString = LocalStringManager.Get(LanguageKey.LK_None);
		}

		// Token: 0x06004783 RID: 18307 RVA: 0x00218140 File Offset: 0x00216340
		public CharacterTitleElement(Refers refers)
		{
			bool flag = null == refers;
			if (flag)
			{
				throw new Exception("can not handle CharacterTitleElement for null refers");
			}
			this._infoItem = new InfoItem(refers);
			this._infoItem.SetIcon("sp_icon_title");
			this._infoItem.SetInfoName(LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Title));
			this._infoItem.SetInfoValue(string.Empty);
			this._mouseTip = this._infoItem.GetMouseTip();
			bool flag2 = null != this._mouseTip;
			if (flag2)
			{
				this._mouseTip.enabled = true;
				this._mouseTip.IsLanguageKey = false;
				this._mouseTip.Type = TipType.Simple;
			}
			CharacterTitleElement._titleNoneString = LocalStringManager.Get(LanguageKey.LK_None);
		}

		// Token: 0x06004784 RID: 18308 RVA: 0x00218206 File Offset: 0x00216406
		internal override void BindEvent()
		{
			this.Item.AddTitleListener(new Action(this.FillElement));
		}

		// Token: 0x06004785 RID: 18309 RVA: 0x00218222 File Offset: 0x00216422
		public override void UnbindEvent()
		{
			this.Item.RemoveTitleListener(new Action(this.FillElement));
		}

		// Token: 0x06004786 RID: 18310 RVA: 0x00218240 File Offset: 0x00216440
		public override void FillElement()
		{
			bool flag = this.Item == null || !this.Item.Init;
			if (!flag)
			{
				bool flag2 = this.Item.TitleIdList.Count <= 0;
				if (flag2)
				{
					this._infoItem.SetInfoValue(CharacterTitleElement._titleNoneString);
				}
				else
				{
					CharacterTitleItem config = CharacterTitle.Instance[this.Item.TitleIdList[0]];
					this._infoItem.SetInfoValue(config.Name);
				}
				bool flag3 = null != this._mouseTip;
				if (flag3)
				{
					string[] tipArgs = new string[]
					{
						LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Title),
						LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Title_TipContent)
					};
					bool flag4 = this.Item.TitleIdList.Count > 0;
					if (flag4)
					{
						for (int i = 0; i < this.Item.TitleIdList.Count; i++)
						{
							string[] array = tipArgs;
							int num = 1;
							array[num] = array[num] + "·" + CharacterTitle.Instance[this.Item.TitleIdList[i]].Name;
						}
					}
					this._mouseTip.PresetParam = tipArgs;
				}
			}
		}

		// Token: 0x06004787 RID: 18311 RVA: 0x00218390 File Offset: 0x00216590
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
				bool flag3 = null != this._mouseTip;
				if (flag3)
				{
					string[] tipArgs = new string[]
					{
						LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Title),
						LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Title_TipContent)
					};
					this._mouseTip.PresetParam = tipArgs;
				}
			}
		}

		// Token: 0x06004788 RID: 18312 RVA: 0x00218420 File Offset: 0x00216620
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<BasicInfoMonitor>(charId, this.IsDead);
		}

		// Token: 0x0400314E RID: 12622
		private readonly TooltipInvoker _mouseTip;

		// Token: 0x0400314F RID: 12623
		private static string _titleNoneString;

		// Token: 0x04003150 RID: 12624
		private readonly InfoItem _infoItem;
	}
}
