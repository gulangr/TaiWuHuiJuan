using System;
using System.Collections.Generic;
using System.Text;
using CharacterDataMonitor;
using Config;
using FrameWork;
using GameData.Domains.Character;
using UICommon.Character.Elements;

namespace UICommon.Character
{
	// Token: 0x020005CF RID: 1487
	public class CharacterFame : CharacterUIElement
	{
		// Token: 0x170008E0 RID: 2272
		// (get) Token: 0x06004670 RID: 18032 RVA: 0x00210722 File Offset: 0x0020E922
		private DetailInfoMonitor Item
		{
			get
			{
				return this.MonitorDataItem as DetailInfoMonitor;
			}
		}

		// Token: 0x06004671 RID: 18033 RVA: 0x00210730 File Offset: 0x0020E930
		public CharacterFame(Refers refers, bool tipType = false)
		{
			bool flag = refers == null;
			if (flag)
			{
				throw new Exception("refers can not be null to create CharacterFame element!");
			}
			this._tipType = tipType;
			this._infoItem = new InfoItem(refers);
			string infoName = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Fame);
			string infoDesc = this.GetDesc();
			this._infoItem.SetInfoName(infoName);
			this._mouseTip = this._infoItem.GetMouseTip();
			bool flag2 = null != this._mouseTip;
			if (flag2)
			{
				this._mouseTip.enabled = true;
				this._mouseTip.IsLanguageKey = false;
				this._mouseTip.Type = TipType.Simple;
				this._mouseTip.PresetParam = new string[]
				{
					infoName,
					infoDesc
				};
			}
		}

		// Token: 0x06004672 RID: 18034 RVA: 0x002107F5 File Offset: 0x0020E9F5
		internal override void BindEvent()
		{
			this.Item.AddOnFameTypeListener(new Action(this.FillElement));
			FeatureMonitor featureMonitor = this._featureMonitor;
			if (featureMonitor != null)
			{
				featureMonitor.AddFeatureListener(new Action(this.FillElement));
			}
		}

		// Token: 0x06004673 RID: 18035 RVA: 0x00210830 File Offset: 0x0020EA30
		public override void UnbindEvent()
		{
			this.Item.RemoveFameTypeListener(new Action(this.FillElement));
			FeatureMonitor featureMonitor = this._featureMonitor;
			if (featureMonitor != null)
			{
				featureMonitor.RemoveFeatureListener(new Action(this.FillElement));
			}
		}

		// Token: 0x06004674 RID: 18036 RVA: 0x0021086C File Offset: 0x0020EA6C
		public override void FillElement()
		{
			bool flag = !this._infoItem.HasValidElement();
			if (flag)
			{
				base.CharacterId = -1;
			}
			else
			{
				DetailInfoMonitor item = this.Item;
				bool flag2;
				if (item != null && item.Init)
				{
					FeatureMonitor featureMonitor = this._featureMonitor;
					flag2 = (featureMonitor == null || !featureMonitor.Init);
				}
				else
				{
					flag2 = true;
				}
				bool flag3 = flag2;
				if (!flag3)
				{
					sbyte fameType = this.Item.FameType;
					this._infoItem.SetIcon(CommonUtils.GetFameIconLegacy(fameType));
					this._infoItem.SetInfoValue(CommonUtils.GetFameString(fameType));
					bool flag4 = !this._mouseTip;
					if (!flag4)
					{
						bool tipType = this._tipType;
						if (tipType)
						{
							this._mouseTip.PresetParam[1] = CommonUtils.GetFameString(fameType);
						}
						else
						{
							string infoName = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Fame);
							string infoDesc = this.GetDesc();
							this._mouseTip.PresetParam = new string[]
							{
								infoName,
								infoDesc
							};
						}
					}
				}
			}
		}

		// Token: 0x06004675 RID: 18037 RVA: 0x0021096C File Offset: 0x0020EB6C
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

		// Token: 0x06004676 RID: 18038 RVA: 0x002109C0 File Offset: 0x0020EBC0
		public override MonitorDataItemBase GetMonitorItem(int charId)
		{
			this._featureMonitor = SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<FeatureMonitor>(charId, this.IsDead);
			return SingletonObject.getInstance<CharacterMonitorModel>().GetMonitorItem<DetailInfoMonitor>(charId, this.IsDead);
		}

		// Token: 0x06004677 RID: 18039 RVA: 0x002109FC File Offset: 0x0020EBFC
		private string GetDesc()
		{
			StringBuilder strBuilder = EasyPool.Get<StringBuilder>();
			strBuilder.Clear();
			string infoDesc = LocalStringManager.Get(LanguageKey.LK_Main_SummaryInfo_Fame_TipContent).ColorReplace();
			strBuilder.Append(infoDesc);
			FeatureMonitor featureMonitor = this._featureMonitor;
			List<short> list = (featureMonitor != null) ? featureMonitor.FeatureIds : null;
			bool flag = list != null && list.Count > 0;
			if (flag)
			{
				bool newLine2 = false;
				foreach (short featureId in this._featureMonitor.FeatureIds)
				{
					int fameChange = SharedMethods.GetSectFeatureFameBonus(featureId, base.IsTaiwu, this.Item.OrganizationInfo);
					bool flag2 = fameChange != 0;
					if (flag2)
					{
						bool flag3 = !newLine2;
						if (flag3)
						{
							strBuilder.AppendLine();
							newLine2 = true;
						}
						CharacterFeatureItem config = CharacterFeature.Instance[featureId];
						string color = (fameChange > 0) ? "brightblue" : "brightred";
						string changeText = (fameChange > 0) ? string.Format("+{0}", fameChange) : string.Format("{0}", fameChange);
						strBuilder.AppendLine();
						strBuilder.Append(LanguageKey.LK_Dot_Symbol.Tr() + config.Name.SetColor(color) + changeText.SetColor(color));
					}
				}
			}
			DetailInfoMonitor item = this.Item;
			List<FameActionRecord> list2 = (item != null) ? item.FameActionRecords : null;
			bool flag4 = list2 != null && list2.Count > 0;
			if (flag4)
			{
				bool newLine = false;
				this.Item.FameActionRecords.ForEach(delegate(FameActionRecord record)
				{
					int time = record.EndDate - SingletonObject.getInstance<BasicGameData>().CurrDate;
					bool flag5 = record.Id >= 0 && record.Value != 0 && time > 0;
					if (flag5)
					{
						FameActionItem config2 = FameAction.Instance[record.Id];
						bool flag6 = config2 == null;
						if (!flag6)
						{
							bool flag7 = !newLine;
							if (flag7)
							{
								strBuilder.AppendLine();
								newLine = true;
							}
							string valueStr = (record.Value > 0) ? string.Format("{0}+{1}", config2.Name, record.Value).SetColor("brightblue") : string.Format("{0}{1}", config2.Name, record.Value).SetColor("brightred");
							string timeStr = time.ToString().SetColor("pinkyellow");
							string result = LocalStringManager.GetFormat(LanguageKey.LK_Fame_Tip_Record, valueStr, timeStr);
							strBuilder.AppendLine();
							strBuilder.Append(result);
						}
					}
				});
			}
			string desc = strBuilder.ToString();
			EasyPool.Free<StringBuilder>(strBuilder);
			return desc;
		}

		// Token: 0x040030D2 RID: 12498
		private FeatureMonitor _featureMonitor;

		// Token: 0x040030D3 RID: 12499
		private readonly InfoItem _infoItem;

		// Token: 0x040030D4 RID: 12500
		private TooltipInvoker _mouseTip;

		// Token: 0x040030D5 RID: 12501
		private bool _tipType = false;
	}
}
