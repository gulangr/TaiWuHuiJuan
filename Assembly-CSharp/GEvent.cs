using System;
using System.Collections.Generic;
using FrameWork;

// Token: 0x02000029 RID: 41
public class GEvent
{
	// Token: 0x0600016D RID: 365 RVA: 0x00009C94 File Offset: 0x00007E94
	public static void Add(Enum em, GEvent.Callback cb)
	{
		GEvent.ZEventData zed = new GEvent.ZEventData();
		GEvent.ZEventData zeventData = zed;
		zeventData.Callback = (GEvent.Callback)Delegate.Combine(zeventData.Callback, cb);
		zed.BOnce = false;
		GEvent.AddEvent(em, zed);
	}

	// Token: 0x0600016E RID: 366 RVA: 0x00009CD0 File Offset: 0x00007ED0
	public static void AddOneShot(Enum em, GEvent.Callback cb)
	{
		GEvent.ZEventData zed = new GEvent.ZEventData();
		GEvent.ZEventData zeventData = zed;
		zeventData.Callback = (GEvent.Callback)Delegate.Combine(zeventData.Callback, cb);
		zed.BOnce = true;
		GEvent.AddEvent(em, zed);
	}

	// Token: 0x0600016F RID: 367 RVA: 0x00009D0C File Offset: 0x00007F0C
	public static void ClearEvent(Enum _em)
	{
		bool flag = GEvent._dicGlobalEvent.ContainsKey(_em);
		if (flag)
		{
			GEvent._dicGlobalEvent[_em].Clear();
			GEvent._dicGlobalEvent.Remove(_em);
		}
	}

	// Token: 0x06000170 RID: 368 RVA: 0x00009D48 File Offset: 0x00007F48
	public static void Remove(Enum em, GEvent.Callback cb)
	{
		bool isProcessing = GEvent._isProcessing;
		if (isProcessing)
		{
			GEvent.EventsToBeRemoved.Add(new GEvent.EventToBeRemoved
			{
				Type = em,
				Callback = cb
			});
		}
		else
		{
			bool flag = GEvent._dicGlobalEvent.ContainsKey(em);
			if (flag)
			{
				foreach (GEvent.ZEventData zed in GEvent._dicGlobalEvent[em])
				{
					bool flag2 = zed.Callback.Equals(cb);
					if (flag2)
					{
						GEvent._dicGlobalEvent[em].Remove(zed);
						break;
					}
				}
				bool flag3 = GEvent._dicGlobalEvent[em].Count <= 0;
				if (flag3)
				{
					GEvent._dicGlobalEvent.Remove(em);
				}
			}
		}
	}

	// Token: 0x06000171 RID: 369 RVA: 0x00009E30 File Offset: 0x00008030
	public static void OnEvent(Enum em, ArgumentBox argsBox = null)
	{
		bool isProcessing = GEvent._isProcessing;
		if (isProcessing)
		{
			GEvent.MessagesToBeProcessed.Enqueue(new GEvent.Message
			{
				Type = em,
				ArgsBox = (((argsBox != null) ? argsBox.Clone() : null) as ArgumentBox)
			});
		}
		else
		{
			GEvent.ProcessEvent(em, argsBox);
			int nProcessedSubMessages = 0;
			while (GEvent.MessagesToBeProcessed.Count > 0)
			{
				GEvent.Message message = GEvent.MessagesToBeProcessed.Dequeue();
				GEvent.ProcessEvent(message.Type, message.ArgsBox);
				nProcessedSubMessages++;
			}
			bool flag = nProcessedSubMessages > 10;
			if (flag)
			{
				GLog.TagWarn("GEvent", "Too many sub-messages ({0}) in event {1}", new object[]
				{
					nProcessedSubMessages,
					em
				});
			}
			bool flag2 = GEvent.EventsToBeRemoved.Count > 0;
			if (flag2)
			{
				foreach (GEvent.EventToBeRemoved evt in GEvent.EventsToBeRemoved)
				{
					GEvent.Remove(evt.Type, evt.Callback);
				}
				GEvent.EventsToBeRemoved.Clear();
			}
		}
	}

	// Token: 0x06000172 RID: 370 RVA: 0x00009F64 File Offset: 0x00008164
	private static void ProcessEvent(Enum em, ArgumentBox argsBox)
	{
		bool flag = !GEvent._dicGlobalEvent.ContainsKey(em);
		if (flag)
		{
			EasyPool.Free<ArgumentBox>(argsBox);
		}
		else
		{
			GEvent._isProcessing = true;
			List<GEvent.ZEventData> invokeList = GEvent._dicGlobalEvent[em];
			List<GEvent.ZEventData> removeList = new List<GEvent.ZEventData>();
			object obj = new object();
			object obj2 = obj;
			lock (obj2)
			{
				foreach (GEvent.ZEventData zed in invokeList)
				{
					bool bonce = zed.BOnce;
					if (bonce)
					{
						removeList.Add(zed);
					}
					if (argsBox != null)
					{
						argsBox.Set("EventType", em);
					}
					zed.Callback(argsBox);
				}
				EasyPool.Free<ArgumentBox>(argsBox);
				foreach (GEvent.ZEventData zed2 in removeList)
				{
					invokeList.Remove(zed2);
				}
				bool flag3 = invokeList.Count <= 0;
				if (flag3)
				{
					GEvent._dicGlobalEvent.Remove(em);
				}
			}
			GEvent._isProcessing = false;
		}
	}

	// Token: 0x06000173 RID: 371 RVA: 0x0000A0C0 File Offset: 0x000082C0
	private static void AddEvent(Enum em, GEvent.ZEventData zed)
	{
		bool flag = GEvent._dicGlobalEvent.ContainsKey(em);
		if (flag)
		{
			GEvent._dicGlobalEvent[em].Add(zed);
		}
		else
		{
			List<GEvent.ZEventData> list = new List<GEvent.ZEventData>();
			list.Add(zed);
			GEvent._dicGlobalEvent.Add(em, list);
		}
	}

	// Token: 0x040000BC RID: 188
	private static Dictionary<Enum, List<GEvent.ZEventData>> _dicGlobalEvent = new Dictionary<Enum, List<GEvent.ZEventData>>();

	// Token: 0x040000BD RID: 189
	private static bool _isProcessing = false;

	// Token: 0x040000BE RID: 190
	private static readonly Queue<GEvent.Message> MessagesToBeProcessed = new Queue<GEvent.Message>();

	// Token: 0x040000BF RID: 191
	private static readonly List<GEvent.EventToBeRemoved> EventsToBeRemoved = new List<GEvent.EventToBeRemoved>();

	// Token: 0x0200109F RID: 4255
	// (Invoke) Token: 0x0600BFFA RID: 49146
	public delegate void Callback(ArgumentBox argBox);

	// Token: 0x020010A0 RID: 4256
	private class ZEventData
	{
		// Token: 0x040093D8 RID: 37848
		public GEvent.Callback Callback;

		// Token: 0x040093D9 RID: 37849
		public bool BOnce = false;
	}

	// Token: 0x020010A1 RID: 4257
	private class Message
	{
		// Token: 0x040093DA RID: 37850
		public Enum Type;

		// Token: 0x040093DB RID: 37851
		public ArgumentBox ArgsBox;
	}

	// Token: 0x020010A2 RID: 4258
	private class EventToBeRemoved
	{
		// Token: 0x040093DC RID: 37852
		public Enum Type;

		// Token: 0x040093DD RID: 37853
		public GEvent.Callback Callback;
	}
}
