using System;
using System.Collections.Generic;
using System.IO;
using Config;
using MoonSharp.Interpreter;

// Token: 0x02000129 RID: 297
public class EncyclopediaModel : ISingletonInit, IDisposable
{
	// Token: 0x06000C6F RID: 3183 RVA: 0x000517E4 File Offset: 0x0004F9E4
	public void Init()
	{
		this._curPath = Path.Combine("RemakeResources/Encyclopedia", SingletonObject.getInstance<GlobalSettings>().Language ?? "CN");
		this._luaEnv = LuaGame.Instance.GetMainEnv();
		ResLoader.Load<EncyclopediaStructure>(Path.Combine(this._curPath, "Structure"), delegate(EncyclopediaStructure asset)
		{
			this.Structure = asset;
		}, null, false);
		this._superLinkMap = new Dictionary<string, EncyclopediaSuperLinkItem>();
		EncyclopediaSuperLink.Instance.Iterate(delegate(EncyclopediaSuperLinkItem item)
		{
			bool flag = !string.IsNullOrEmpty(item.LinkId) && !this._superLinkMap.ContainsKey(item.LinkId);
			if (flag)
			{
				this._superLinkMap.Add(item.LinkId, item);
			}
			return true;
		});
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x0005186C File Offset: 0x0004FA6C
	public EncyclopediaSuperLinkItem GetSuperLinkItem(string linkId)
	{
		EncyclopediaSuperLinkItem item;
		this._superLinkMap.TryGetValue(linkId, out item);
		return item;
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x0005188E File Offset: 0x0004FA8E
	public void Dispose()
	{
		this._superLinkMap.Clear();
		this._superLinkMap = null;
	}

	// Token: 0x04000DB1 RID: 3505
	private const string EncyclopediaPath = "RemakeResources/Encyclopedia";

	// Token: 0x04000DB2 RID: 3506
	private string _curPath;

	// Token: 0x04000DB3 RID: 3507
	public EncyclopediaStructure Structure;

	// Token: 0x04000DB4 RID: 3508
	private Script _luaEnv;

	// Token: 0x04000DB5 RID: 3509
	private Dictionary<string, EncyclopediaSuperLinkItem> _superLinkMap;
}
