using System;
using System.Collections.Generic;
using Config;
using FrameWork;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Combat;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;

// Token: 0x02000291 RID: 657
public class MouseTipDisorderOfQi : MouseTipBase
{
	// Token: 0x17000491 RID: 1169
	// (get) Token: 0x060029E5 RID: 10725 RVA: 0x0013DF7E File Offset: 0x0013C17E
	private bool AllReceived
	{
		get
		{
			return this._respondedDataTypes.Count == 3;
		}
	}

	// Token: 0x060029E6 RID: 10726 RVA: 0x0013DF90 File Offset: 0x0013C190
	protected override void Init(ArgumentBox argsBox)
	{
		argsBox.Get("CharId", out this._charId);
		argsBox.Get("CombatStyle", out this._combatStyle);
		UIElement element = this.Element;
		element.OnListenerIdReady = (Action)Delegate.Combine(element.OnListenerIdReady, new Action(this.OnListenerIdReady));
	}

	// Token: 0x060029E7 RID: 10727 RVA: 0x0013DFE9 File Offset: 0x0013C1E9
	public override void InitMonitorFieldIds()
	{
		this.MonitorFields.Add(new UIBase.MonitorDataField(4, 0, (ulong)this._charId, new uint[]
		{
			21U
		}));
	}

	// Token: 0x060029E8 RID: 10728 RVA: 0x0013E011 File Offset: 0x0013C211
	private void OnListenerIdReady()
	{
		CharacterDomainMethod.Call.GetHealthRecovery(this.Element.GameDataListenerId, this._charId);
		CharacterDomainMethod.Call.GetChangeOfQiDisorder(this.Element.GameDataListenerId, this._charId);
	}

	// Token: 0x060029E9 RID: 10729 RVA: 0x0013E044 File Offset: 0x0013C244
	public override void OnNotifyGameData(List<NotificationWrapper> notifications)
	{
		foreach (NotificationWrapper wrapper in notifications)
		{
			byte type = wrapper.Notification.Type;
			byte b = type;
			if (b != 0)
			{
				if (b == 1)
				{
					this.HandlerMethodReturn(wrapper);
				}
			}
			else
			{
				this.HandlerDataModification(wrapper);
			}
		}
	}

	// Token: 0x060029EA RID: 10730 RVA: 0x0013E0C0 File Offset: 0x0013C2C0
	private void HandlerDataModification(NotificationWrapper wrapper)
	{
		DataUid uid = wrapper.Notification.Uid;
		bool flag = uid.DomainId != 4 || uid.DataId > 0;
		if (!flag)
		{
			bool flag2 = uid.SubId0 != (ulong)this._charId;
			if (!flag2)
			{
				RawDataPool dataPool = wrapper.DataPool;
				int valueOffset = wrapper.Notification.ValueOffset;
				RawDataPool pool = dataPool;
				int offset = valueOffset;
				uint subId = uid.SubId1;
				uint num = subId;
				if (num == 21U)
				{
					Serializer.Deserialize(pool, offset, ref this._disorderOfQi);
					this.OnDataResponded(MouseTipDisorderOfQi.EDataType.DisorderOfQi);
				}
			}
		}
	}

	// Token: 0x060029EB RID: 10731 RVA: 0x0013E154 File Offset: 0x0013C354
	private void HandlerMethodReturn(NotificationWrapper wrapper)
	{
		Notification notification = wrapper.Notification;
		bool flag = notification.DomainId != 4;
		if (!flag)
		{
			RawDataPool dataPool = wrapper.DataPool;
			int valueOffset = wrapper.Notification.ValueOffset;
			RawDataPool pool = dataPool;
			int offset = valueOffset;
			ushort methodId = notification.MethodId;
			ushort num = methodId;
			if (num != 83)
			{
				if (num == 97)
				{
					Serializer.Deserialize(pool, offset, ref this._healthRecovery);
					this.OnDataResponded(MouseTipDisorderOfQi.EDataType.HealthRecovery);
				}
			}
			else
			{
				Serializer.Deserialize(pool, offset, ref this._changeOfQiDisorder);
				this.OnDataResponded(MouseTipDisorderOfQi.EDataType.ChangeOfQiDisorder);
			}
		}
	}

	// Token: 0x060029EC RID: 10732 RVA: 0x0013E1E0 File Offset: 0x0013C3E0
	private void OnDataResponded(MouseTipDisorderOfQi.EDataType dataType)
	{
		this._respondedDataTypes.Add(dataType);
		bool flag = !this.AllReceived;
		if (!flag)
		{
			sbyte level = DisorderLevelOfQi.GetDisorderLevelOfQi(this._disorderOfQi);
			QiDisorderEffectItem config = QiDisorderEffect.Instance[level];
			int changeValue = (int)(this._changeOfQiDisorder / 10);
			string changeString = (this._changeOfQiDisorder > 0) ? string.Format("+{0}", changeValue).SetColor("brightred") : changeValue.ToString().SetColor("brightblue");
			TextMeshProUGUI desc = base.CGet<TextMeshProUGUI>("Desc");
			desc.text = LocalStringManager.GetFormat(LanguageKey.LK_Qi_Disorder_StateContent, new object[]
			{
				config.Name,
				config.Desc,
				changeString,
				this._healthRecovery,
				config.InjuredRate
			}).ColorReplace();
			bool combatStyle = this._combatStyle;
			if (combatStyle)
			{
				short threshold = DefeatMarkCollection.CalcQiDisorderMarkThreshold((int)this._disorderOfQi);
				string thresholdStr = (this._disorderOfQi == DisorderLevelOfQi.MaxValue) ? "-" : ((int)(threshold / 10)).ToString();
				TextMeshProUGUI textMeshProUGUI = desc;
				textMeshProUGUI.text += LocalStringManager.GetFormat(LanguageKey.LK_Qi_Disorder_StateContent_CombatStyle, ((int)(this._disorderOfQi % threshold / 10)).ToString(), thresholdStr).ColorReplace();
			}
			desc.GetComponent<TMPTextSpriteHelper>().Parse();
			this.Element.ShowAfterRefresh();
		}
	}

	// Token: 0x04001E6B RID: 7787
	private readonly HashSet<MouseTipDisorderOfQi.EDataType> _respondedDataTypes = new HashSet<MouseTipDisorderOfQi.EDataType>();

	// Token: 0x04001E6C RID: 7788
	private int _charId;

	// Token: 0x04001E6D RID: 7789
	private bool _combatStyle;

	// Token: 0x04001E6E RID: 7790
	private short _disorderOfQi;

	// Token: 0x04001E6F RID: 7791
	private short _changeOfQiDisorder;

	// Token: 0x04001E70 RID: 7792
	private short _healthRecovery;

	// Token: 0x020015FC RID: 5628
	private enum EDataType
	{
		// Token: 0x0400A699 RID: 42649
		DisorderOfQi,
		// Token: 0x0400A69A RID: 42650
		ChangeOfQiDisorder,
		// Token: 0x0400A69B RID: 42651
		HealthRecovery,
		// Token: 0x0400A69C RID: 42652
		Count
	}
}
