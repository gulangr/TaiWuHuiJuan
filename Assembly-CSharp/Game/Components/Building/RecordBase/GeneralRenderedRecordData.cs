using System;
using Config;
using Game.Components.Character.LifeRecord;
using GameData.Domains.Character.Display;
using GameData.Domains.LifeRecord;
using GameData.Domains.World;

namespace Game.Components.Building.RecordBase
{
	// Token: 0x02000F68 RID: 3944
	public class GeneralRenderedRecordData : RenderedRecordDataBase
	{
		// Token: 0x0600B49E RID: 46238 RVA: 0x005230B4 File Offset: 0x005212B4
		public override void SetData(TransferableRecord record, TransferableRecordDataBase data)
		{
			this.TemplateId = record.RecordType;
			this.SetDataImpl(record, data);
		}

		// Token: 0x0600B49F RID: 46239 RVA: 0x005230CC File Offset: 0x005212CC
		private void SetDataImpl(TransferableRecord record, TransferableRecordDataBase data)
		{
			switch (this.TemplateId)
			{
			case -3:
				this.Main = (this.Sub = "");
				break;
			case -2:
			{
				int date = record.Arguments[0].Item2;
				int year = date / 12;
				int month = date % 12;
				bool flag = month < 0;
				if (flag)
				{
					month += 12;
				}
				this.Main = LanguageKey.LK_Game_Time.TrFormat(new object[]
				{
					year + 1,
					month + 1,
					LocalStringManager.Get(string.Format("LK_Season_{0}", TimeKit.GetSeason(date))),
					Month.Instance[month].Name
				}).ColorReplace();
				this.Sub = "";
				break;
			}
			case -1:
			{
				NameRelatedData nameRelatedData = data.CharName.NameRelatedData;
				string realName = NameCenter.GetRealName(ref nameRelatedData);
				int bornDate = record.Arguments[0].Item2;
				int month2 = bornDate % 12;
				bool flag2 = month2 < 0;
				if (flag2)
				{
					month2 += 12;
				}
				MonthItem monthConfig = Month.Instance[month2];
				bool flag3 = bornDate < 0;
				if (flag3)
				{
					int yearDiffer = SingletonObject.getInstance<TimeManager>().GetYearBetweenDate(bornDate, 0);
					this.Main = LanguageKey.LK_Birth_Record_Before.TrFormat(yearDiffer, monthConfig.Name, realName).ColorReplace();
				}
				else
				{
					this.Main = LanguageKey.LK_Birth_Record.TrFormat(monthConfig.Name, realName).ColorReplace();
				}
				this.Sub = "";
				break;
			}
			default:
			{
				ValueTuple<string, string> valueTuple = this.Parent.DataRender(record, data);
				this.Main = valueTuple.Item1;
				this.Sub = valueTuple.Item2;
				break;
			}
			}
		}

		// Token: 0x04008CB8 RID: 36024
		[NonSerialized]
		public GeneralRecord Parent;
	}
}
