using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x02000204 RID: 516
public class EventTextureManager : ISingletonInit, IDisposable
{
	// Token: 0x06002102 RID: 8450 RVA: 0x000F06BB File Offset: 0x000EE8BB
	private static void InitEventBackPathMap()
	{
		EventTextureManager.EventBackPathMap.Clear();
		EventTextureManager._eventTextureScriptableObject.eventTexturePathList.ForEach(delegate(string e)
		{
			string fileName = Path.GetFileName(e);
			EventTextureManager.EventBackPathMap.Add(fileName, e);
		});
	}

	// Token: 0x06002103 RID: 8451 RVA: 0x000F06F8 File Offset: 0x000EE8F8
	public bool GetEventBackPath(string textureName, out string path)
	{
		path = string.Empty;
		string texturePath;
		bool flag = EventTextureManager.EventBackPathMap.TryGetValue(textureName, out texturePath);
		bool result;
		if (flag)
		{
			path = "EventBack/" + texturePath;
			result = true;
		}
		else
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06002104 RID: 8452 RVA: 0x000F0735 File Offset: 0x000EE935
	public static void InitData()
	{
		ResLoader.Load<EventTextureScriptableObject>("GameAtlas/EventTextureScriptableObject", delegate(EventTextureScriptableObject a)
		{
			EventTextureManager._eventTextureScriptableObject = a;
			EventTextureManager.InitEventBackPathMap();
		}, null, false);
	}

	// Token: 0x06002105 RID: 8453 RVA: 0x000F0764 File Offset: 0x000EE964
	public void Dispose()
	{
	}

	// Token: 0x06002106 RID: 8454 RVA: 0x000F0767 File Offset: 0x000EE967
	public void Init()
	{
	}

	// Token: 0x0400197E RID: 6526
	public const string TextureDirectory = "RemakeResources/Textures/EventBack";

	// Token: 0x0400197F RID: 6527
	public const string FullAssetPath = "Assets/GameAtlas/EventTextureScriptableObject.asset";

	// Token: 0x04001980 RID: 6528
	private const string AssetPath = "GameAtlas/EventTextureScriptableObject";

	// Token: 0x04001981 RID: 6529
	private static EventTextureScriptableObject _eventTextureScriptableObject;

	// Token: 0x04001982 RID: 6530
	private static readonly Dictionary<string, string> EventBackPathMap = new Dictionary<string, string>();
}
