using System;
using System.Linq;
using Config;
using GameData.Domains.Character.Display;
using GameData.Domains.LifeRecord;
using GameData.Domains.World;
using UnityEngine;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F49 RID: 3913
	public class RenderedRecordData : RenderedRecordDataBase
	{
		// Token: 0x0600B395 RID: 45973 RVA: 0x0051BBD1 File Offset: 0x00519DD1
		public override void SetData(TransferableRecord record, TransferableRecordDataBase data)
		{
			this.TemplateId = record.RecordType;
			this.SetDataImpl(record, data);
		}

		// Token: 0x0600B396 RID: 45974 RVA: 0x0051BBEC File Offset: 0x00519DEC
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
				bool flag2 = data.CharId != SingletonObject.getInstance<BasicGameData>().TaiwuCharId && data.FavorToTaiwu < 10000;
				if (flag2)
				{
					this.Main = LifeRecord.InvisibleContent;
					this.Sub = LifeRecord.InvisibleContentDesc;
				}
				else
				{
					NameRelatedData nameRelatedData = data.CharName.NameRelatedData;
					string realName = NameCenter.GetRealName(ref nameRelatedData);
					int bornDate = record.Arguments[0].Item2;
					int month2 = bornDate % 12;
					bool flag3 = month2 < 0;
					if (flag3)
					{
						month2 += 12;
					}
					MonthItem monthConfig = Month.Instance[month2];
					bool flag4 = bornDate < 0;
					if (flag4)
					{
						int yearDiffer = SingletonObject.getInstance<TimeManager>().GetYearBetweenDate(bornDate, 0);
						this.Main = LanguageKey.LK_Birth_Record_Before.TrFormat(yearDiffer, monthConfig.Name, realName).ColorReplace();
					}
					else
					{
						this.Main = LanguageKey.LK_Birth_Record.TrFormat(monthConfig.Name, realName).ColorReplace();
					}
					this.Sub = "";
				}
				break;
			}
			default:
			{
				LifeRecordItem config = LifeRecord.Instance[record.RecordType];
				bool flag5 = config == null;
				if (flag5)
				{
					Debug.LogWarning(string.Format("Invalid record type: {0}", record.RecordType));
					this.TemplateId = -4;
					this.Main = (this.Sub = "");
				}
				else
				{
					bool flag6 = data.CharId != SingletonObject.getInstance<BasicGameData>().TaiwuCharId && data.FavorToTaiwu < config.RequiredFavorability;
					if (flag6)
					{
						this.Score = 50;
						this.Main = LifeRecord.InvisibleContent;
						this.Sub = LifeRecord.InvisibleContentDesc;
					}
					else
					{
						string text = config.Desc;
						this.Score = record.GetCalculatedLifeRecordScore(data.ArgumentCollection);
						object[] formatted = (from x in record.Arguments
						select GameMessageUtils.ReadArguments(x.Item1, x.Item2, data)).ToArray<object>();
						try
						{
							this.Main = string.Format(text, formatted).ColorReplace();
						}
						catch
						{
							Debug.LogError(string.Format("render error: {0}/[{1}] with id {2}", text, string.Join(",", formatted), data.CharId));
							this.Main = text;
						}
						this.Sub = "";
					}
				}
				break;
			}
			}
		}

		// Token: 0x04008B95 RID: 35733
		public int Score = 50;
	}
}
