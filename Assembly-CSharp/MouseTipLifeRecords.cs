using System;
using System.Collections.Generic;
using System.Text;
using Config;
using FrameWork;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.LifeRecord;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.Domains.Map;
using GameData.Domains.Organization;
using GameData.Domains.Organization.Display;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;

// Token: 0x020002AA RID: 682
public class MouseTipLifeRecords : MouseTipBase
{
	// Token: 0x06002A73 RID: 10867 RVA: 0x00144F30 File Offset: 0x00143130
	protected override void Init(ArgumentBox argsBox)
	{
		bool flag = this._timeManager == null;
		if (flag)
		{
			this._timeManager = SingletonObject.getInstance<TimeManager>();
			this._argumentCollection = new ArgumentCollection();
			this._renderedArgCollection = new RenderedArgumentCollection();
			this._renderInfos = new List<LifeRecordRenderInfo>();
		}
		this._title = base.CGet<TextMeshProUGUI>("Title");
		this._desc = base.CGet<TextMeshProUGUI>("Desc");
		this._desc.text = string.Empty;
		bool flag2 = argsBox.Get<CharacterDisplayData>("CharacterDisplayData", out this._charDisplayData);
		if (flag2)
		{
			this._title.text = NameCenter.GetCharMonasticTitleOrNameByDisplayData(this._charDisplayData, false, false) + LocalStringManager.Get(LanguageKey.LK_Tips_Recent_LifeRecords);
			this._charId = this._charDisplayData.CharacterId;
		}
		else
		{
			bool flag3 = argsBox.Get<GraveDisplayData>("GraveDisplayData", out this._graveDisplayData);
			if (flag3)
			{
				this._title.text = NameCenter.GetCharMonasticTitleOrNameByGraveData(this._graveDisplayData, false, false) + LocalStringManager.Get(LanguageKey.LK_Tips_Recent_LifeRecords);
				this._charId = this._graveDisplayData.Id;
			}
		}
		this.NeedDataListenerId = true;
		this.Element.OnListenerIdReady = delegate()
		{
			LifeRecordDomainMethod.Call.GetLast(this.Element.GameDataListenerId, this._charId, 5);
			this._nameRenderProgress = 0;
		};
	}

	// Token: 0x06002A74 RID: 10868 RVA: 0x0014506C File Offset: 0x0014326C
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			Notification notification = wrapper.Notification;
			this.FetchMethodReturnValue(notification.DomainId, notification.MethodId, notification.ValueOffset, wrapper.DataPool);
		}
	}

	// Token: 0x06002A75 RID: 10869 RVA: 0x001450E0 File Offset: 0x001432E0
	private void FetchMethodReturnValue(ushort domainId, ushort methodId, int offset, RawDataPool dataPool)
	{
		bool flag = this.Element == null;
		if (!flag)
		{
			switch (domainId)
			{
			case 2:
				if (methodId == 24)
				{
					Serializer.Deserialize(dataPool, offset, ref this._locationNames);
					GameMessageUtils.RenderLocationNames(this._locationNames, this._renderedArgCollection);
					this._nameRenderProgress += 1;
				}
				break;
			case 3:
				if (methodId == 1)
				{
					Serializer.Deserialize(dataPool, offset, ref this._settlementNames);
					GameMessageUtils.RenderSettlementNames(this._settlementNames, this._renderedArgCollection);
					this._nameRenderProgress += 1;
				}
				break;
			case 4:
				if (methodId == 9)
				{
					Serializer.Deserialize(dataPool, offset, ref this._charNameAndLifeDataList);
					int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
					GameMessageUtils.RenderCharacterNames(taiwuCharId, this._charNameAndLifeDataList, this._argumentCollection, this._renderedArgCollection, false);
					this._nameRenderProgress += 1;
				}
				break;
			default:
				if (domainId != 13)
				{
					if (domainId == 19)
					{
						if (methodId == 83)
						{
							Serializer.Deserialize(dataPool, offset, ref this._jiaoLoongNames);
							GameMessageUtils.RenderJiaoLoongNames(this._jiaoLoongNames, this._renderedArgCollection);
							this._nameRenderProgress += 1;
						}
					}
				}
				else if (methodId != 2)
				{
					if (methodId != 3)
					{
					}
				}
				else
				{
					ReadonlyLifeRecords lifeRecords = new ReadonlyLifeRecords();
					Serializer.Deserialize(dataPool, offset, ref lifeRecords);
					this._argumentCollection.Clear();
					this._renderedArgCollection.Clear();
					this._renderInfos.Clear();
					this._nameRenderProgress = 0;
					bool flag2 = lifeRecords.Count == 0;
					if (flag2)
					{
						this._desc.SetText(LocalStringManager.Get(LanguageKey.LK_NewGame_Empty), true);
						UIElement element = this.Element;
						if (element != null)
						{
							element.ShowAfterRefresh();
						}
						return;
					}
					CharacterDisplayData charDisplayData = this._charDisplayData;
					sbyte b = (charDisplayData != null) ? charDisplayData.Gender : this._graveDisplayData.NameData.Gender;
					lifeRecords.GetRenderInfos(this._renderInfos, this._argumentCollection);
					GameMessageUtils.RenderFixedArguments(this._argumentCollection, this._renderedArgCollection, false);
					this._charIdList.Clear();
					this._charIdList.AddRange(this._argumentCollection.Characters);
					this._charIdList.AddRange(this._argumentCollection.CharacterRealNames);
					bool flag3 = this._charIdList.Count > 0;
					if (flag3)
					{
						CharacterDomainMethod.Call.GetNameAndLifeRelatedDataList(this.Element.GameDataListenerId, this._charIdList);
					}
					else
					{
						this._nameRenderProgress += 1;
					}
					bool flag4 = this._argumentCollection.Locations.Count > 0;
					if (flag4)
					{
						MapDomainMethod.Call.GetLocationNameRelatedDataList(this.Element.GameDataListenerId, this._argumentCollection.Locations);
					}
					else
					{
						this._nameRenderProgress += 1;
					}
					bool flag5 = this._argumentCollection.Settlements.Count > 0;
					if (flag5)
					{
						OrganizationDomainMethod.Call.GetSettlementNameRelatedData(this.Element.GameDataListenerId, this._argumentCollection.Settlements);
					}
					else
					{
						this._nameRenderProgress += 1;
					}
					bool flag6 = this._argumentCollection.JiaoLoongs.Count > 0;
					if (flag6)
					{
						ExtraDomainMethod.Call.GetJiaoLoongNameRelatedDataList(this.Element.GameDataListenerId, this._argumentCollection.JiaoLoongs);
					}
					else
					{
						this._nameRenderProgress += 1;
					}
				}
				break;
			}
			bool flag7 = this._nameRenderProgress >= 4;
			if (flag7)
			{
				this.RenderAllRecords();
			}
		}
	}

	// Token: 0x06002A76 RID: 10870 RVA: 0x00145474 File Offset: 0x00143674
	private void RenderAllRecords()
	{
		StringBuilder stringBuilder = EasyPool.Get<StringBuilder>();
		stringBuilder.Clear();
		foreach (LifeRecordRenderInfo renderInfo in this._renderInfos)
		{
			int year = renderInfo.Date / 12;
			int month = renderInfo.Date % 12;
			string date = LocalStringManager.GetFormat(LanguageKey.LK_Game_Time, new object[]
			{
				year + 1,
				month + 1,
				LocalStringManager.Get(string.Format("LK_Season_{0}", TimeKit.GetSeason(renderInfo.Date))),
				Month.Instance[month].Name
			});
			string message = GameMessageUtils.ParseRenderInfoToText(LifeRecord.Instance[renderInfo.RecordType].Desc, renderInfo, this._renderedArgCollection);
			stringBuilder.AppendFormat("· {0}: {1}\n", date, message);
		}
		this._desc.SetText(stringBuilder.ToString(), true);
		base.CGet<TMPTextSpriteHelper>("SpriteHelper").Parse();
		EasyPool.Free<StringBuilder>(stringBuilder);
		UIElement element = this.Element;
		if (element != null)
		{
			element.ShowAfterRefresh();
		}
	}

	// Token: 0x04001EB5 RID: 7861
	private sbyte _nameRenderProgress;

	// Token: 0x04001EB6 RID: 7862
	private TimeManager _timeManager;

	// Token: 0x04001EB7 RID: 7863
	private ArgumentCollection _argumentCollection;

	// Token: 0x04001EB8 RID: 7864
	private RenderedArgumentCollection _renderedArgCollection;

	// Token: 0x04001EB9 RID: 7865
	private List<LifeRecordRenderInfo> _renderInfos;

	// Token: 0x04001EBA RID: 7866
	private List<NameAndLifeRelatedData> _charNameAndLifeDataList;

	// Token: 0x04001EBB RID: 7867
	private List<SettlementNameRelatedData> _settlementNames;

	// Token: 0x04001EBC RID: 7868
	private List<LocationNameRelatedData> _locationNames;

	// Token: 0x04001EBD RID: 7869
	private List<JiaoLoongNameRelatedData> _jiaoLoongNames;

	// Token: 0x04001EBE RID: 7870
	private CharacterDisplayData _charDisplayData;

	// Token: 0x04001EBF RID: 7871
	private GraveDisplayData _graveDisplayData;

	// Token: 0x04001EC0 RID: 7872
	private TextMeshProUGUI _title;

	// Token: 0x04001EC1 RID: 7873
	private TextMeshProUGUI _desc;

	// Token: 0x04001EC2 RID: 7874
	private int _charId;

	// Token: 0x04001EC3 RID: 7875
	private readonly List<int> _charIdList = new List<int>();
}
