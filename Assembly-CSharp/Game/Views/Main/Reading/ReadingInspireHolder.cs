using System;
using System.Collections.Generic;
using GameData.Domains.Item;
using GameData.Domains.Taiwu;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Main.Reading
{
	// Token: 0x0200096A RID: 2410
	public class ReadingInspireHolder : MonoBehaviour, IAsyncMethodRequestHandler
	{
		// Token: 0x0600737B RID: 29563 RVA: 0x0035A4F8 File Offset: 0x003586F8
		public void RefreshAllBookReadingData(ItemKey currReadingBook)
		{
			TaiwuDomainMethod.AsyncCall.GetCurrReadingEventBonusRate(this, delegate(int offset, RawDataPool dataPool)
			{
				short inspireRetio = 0;
				Serializer.Deserialize(dataPool, offset, ref inspireRetio);
				this.inspireRatio.text = string.Format("{0} %", inspireRetio).SetColor("lightblue");
			});
		}

		// Token: 0x0600737C RID: 29564 RVA: 0x0035A50E File Offset: 0x0035870E
		public void RegisterAsyncMethodCall(int requestId)
		{
			this._requestedAsyncMethods.Add(requestId);
		}

		// Token: 0x0600737D RID: 29565 RVA: 0x0035A520 File Offset: 0x00358720
		public void ClearAsyncMethodCalls()
		{
			AsyncMethodDispatcher dispatcher = SingletonObject.getInstance<AsyncMethodDispatcher>();
			foreach (int one in this._requestedAsyncMethods)
			{
				dispatcher.UnregisterAsyncMethodCall(one);
			}
			this._requestedAsyncMethods.Clear();
		}

		// Token: 0x040055D2 RID: 21970
		public TextMeshProUGUI inspireRatio;

		// Token: 0x040055D3 RID: 21971
		private readonly List<int> _requestedAsyncMethods = new List<int>();
	}
}
