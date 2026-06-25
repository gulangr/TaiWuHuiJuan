using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FrameWork;
using UnityEngine;

// Token: 0x0200001A RID: 26
public static class HotKeyCommandExtensions
{
	// Token: 0x060000C2 RID: 194 RVA: 0x000055C3 File Offset: 0x000037C3
	public static IEnumerable<T> CheckSeries<T>(this IReadOnlyDictionary<HotKeyCommand, T> hotKey2Result, UIElement element)
	{
		Dictionary<KeyCode, Dictionary<KeyCode, HotKeyCommand>> key2FnKeys = EasyPool.Get<Dictionary<KeyCode, Dictionary<KeyCode, HotKeyCommand>>>();
		key2FnKeys.Clear();
		foreach (HotKeyCommand hotKey in hotKey2Result.Keys)
		{
			KeyCode key = hotKey.KeyGroup.Key;
			KeyCode fnKey = hotKey.KeyGroup.FunctionKey;
			Dictionary<KeyCode, HotKeyCommand> fnKeys;
			bool flag = !key2FnKeys.TryGetValue(key, out fnKeys);
			if (flag)
			{
				fnKeys = EasyPool.Get<Dictionary<KeyCode, HotKeyCommand>>();
				fnKeys.Clear();
				key2FnKeys.Add(key, fnKeys);
			}
			bool flag2 = !fnKeys.ContainsKey(fnKey);
			if (flag2)
			{
				fnKeys.Add(fnKey, hotKey);
			}
			fnKeys = null;
			hotKey = null;
		}
		IEnumerator<HotKeyCommand> enumerator = null;
		foreach (Dictionary<KeyCode, HotKeyCommand> fnKeys2 in key2FnKeys.Values)
		{
			foreach (KeyCode fnKey2 in HotKeyCommandExtensions.FnKeyCheckOrder)
			{
				HotKeyCommand hotKey2;
				bool flag3 = !fnKeys2.TryGetValue(fnKey2, out hotKey2) || !hotKey2.Check(element, false, false, false, false, false);
				if (!flag3)
				{
					yield return hotKey2Result[hotKey2];
					break;
				}
			}
			IEnumerator<KeyCode> enumerator3 = null;
			fnKeys2 = null;
		}
		Dictionary<KeyCode, Dictionary<KeyCode, HotKeyCommand>>.ValueCollection.Enumerator enumerator2 = default(Dictionary<KeyCode, Dictionary<KeyCode, HotKeyCommand>>.ValueCollection.Enumerator);
		foreach (Dictionary<KeyCode, HotKeyCommand> fnKeys3 in key2FnKeys.Values)
		{
			EasyPool.Free<Dictionary<KeyCode, HotKeyCommand>>(fnKeys3);
			fnKeys3 = null;
		}
		Dictionary<KeyCode, Dictionary<KeyCode, HotKeyCommand>>.ValueCollection.Enumerator enumerator4 = default(Dictionary<KeyCode, Dictionary<KeyCode, HotKeyCommand>>.ValueCollection.Enumerator);
		EasyPool.Free<Dictionary<KeyCode, Dictionary<KeyCode, HotKeyCommand>>>(key2FnKeys);
		yield break;
		yield break;
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x000055DA File Offset: 0x000037DA
	// Note: this type is marked as 'beforefieldinit'.
	static HotKeyCommandExtensions()
	{
		KeyCode[] array = new KeyCode[4];
		RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.70FF4A32C77CA3FD60F3B5F25BA1E0C94F4DAC2DB9DD6722E2BAAEE1C2D0F32B).FieldHandle);
		HotKeyCommandExtensions.FnKeyCheckOrder = array;
	}

	// Token: 0x04000073 RID: 115
	private static readonly IReadOnlyList<KeyCode> FnKeyCheckOrder;
}
