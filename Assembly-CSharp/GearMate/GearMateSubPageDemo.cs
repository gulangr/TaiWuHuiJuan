using System;
using System.Collections.Generic;
using GameData.Domains.Building;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace GearMate
{
	// Token: 0x0200061B RID: 1563
	public class GearMateSubPageDemo : GearMateSubPageBase
	{
		// Token: 0x060049AD RID: 18861 RVA: 0x0022665C File Offset: 0x0022485C
		protected override void InitInternal()
		{
			this.InitRefers();
			this._demoButtonList[0].ClearAndAddListener(delegate
			{
				base.Parent.Avatar.ShowBubble("测试气泡", 2f);
			});
		}

		// Token: 0x060049AE RID: 18862 RVA: 0x00226684 File Offset: 0x00224884
		protected override IList<UIBase.MonitorDataField> GetMonitorFields()
		{
			return new List<UIBase.MonitorDataField>
			{
				new UIBase.MonitorDataField(5, 58, ulong.MaxValue, null)
			};
		}

		// Token: 0x060049AF RID: 18863 RVA: 0x002266AD File Offset: 0x002248AD
		public override void OnListenerIdReady()
		{
			BuildingDomainMethod.Call.IsHaveChickenKing(base.ListenerId);
		}

		// Token: 0x060049B0 RID: 18864 RVA: 0x002266BC File Offset: 0x002248BC
		protected override void HandleDataModification(Notification notification, NotificationWrapper wrapper)
		{
			ushort domianId = notification.Uid.DomainId;
			ushort dataId = notification.Uid.DataId;
			RawDataPool pool = wrapper.DataPool;
			int offset = notification.ValueOffset;
			bool flag = domianId == 5;
			if (flag)
			{
				bool flag2 = dataId == 58;
				if (flag2)
				{
					short count = 0;
					Serializer.Deserialize(pool, offset, ref count);
					Debug.Log(string.Format("count = {0}", count));
					TextMeshProUGUI demoText = this._demoText;
					demoText.text += string.Format("count = {0}", count);
				}
			}
		}

		// Token: 0x060049B1 RID: 18865 RVA: 0x00226758 File Offset: 0x00224958
		public override void HandleMethodReturn(Notification notification, NotificationWrapper wrapper)
		{
			ushort domianId = notification.DomainId;
			ushort methodId = notification.MethodId;
			RawDataPool pool = wrapper.DataPool;
			int offset = notification.ValueOffset;
			bool flag = domianId == 9;
			if (flag)
			{
				bool flag2 = methodId == 80;
				if (flag2)
				{
					bool isHave = false;
					Serializer.Deserialize(pool, offset, ref isHave);
					Debug.Log(string.Format("isHave = {0}", isHave));
					TextMeshProUGUI demoText = this._demoText;
					demoText.text += string.Format("have = {0}", isHave);
				}
			}
		}

		// Token: 0x060049B2 RID: 18866 RVA: 0x002267E8 File Offset: 0x002249E8
		public override void OnGearMateCharacterIdChanged(int lastId)
		{
			Debug.Log(string.Format("{0} -> {1}", lastId, base.GearMateDisplayData.CharacterId));
		}

		// Token: 0x060049B3 RID: 18867 RVA: 0x00226811 File Offset: 0x00224A11
		private void InitRefers()
		{
			this._demoButtonList = base.CGetList<CButtonObsolete>("DemoButton");
			this._demoText = base.CGet<TextMeshProUGUI>("DemoText");
		}

		// Token: 0x04003321 RID: 13089
		private List<CButtonObsolete> _demoButtonList;

		// Token: 0x04003322 RID: 13090
		private TextMeshProUGUI _demoText;
	}
}
