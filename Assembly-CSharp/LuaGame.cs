using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Config;
using GameData.Utilities;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using MoonSharp.Interpreter.Loaders;
using UnityEngine;

// Token: 0x020000FC RID: 252
public class LuaGame : IScriptLoader
{
	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x06000861 RID: 2145 RVA: 0x00039C28 File Offset: 0x00037E28
	// (set) Token: 0x06000862 RID: 2146 RVA: 0x00039C2F File Offset: 0x00037E2F
	public static bool LuaReady { get; private set; }

	// Token: 0x06000863 RID: 2147 RVA: 0x00039C38 File Offset: 0x00037E38
	private static Func<ScriptExecutionContext, CallbackArguments, DynValue> HookGlobalFunc(Script env, string func, Func<ScriptExecutionContext, CallbackArguments, Func<ScriptExecutionContext, CallbackArguments, DynValue>, DynValue> e)
	{
		DynValue origin = env.Globals.Get(func);
		CallbackFunction function = (origin.Type == DataType.ClrFunction) ? origin.Callback : CallbackFunction.FromDelegate(env, origin.Function.GetDelegate(), InteropAccessMode.Default);
		Func<ScriptExecutionContext, CallbackArguments, DynValue> <>9__1;
		Func<ScriptExecutionContext, CallbackArguments, DynValue> hooked = delegate(ScriptExecutionContext ctx, CallbackArguments args)
		{
			Func<ScriptExecutionContext, CallbackArguments, Func<ScriptExecutionContext, CallbackArguments, DynValue>, DynValue> e2 = e;
			Func<ScriptExecutionContext, CallbackArguments, DynValue> arg;
			if ((arg = <>9__1) == null)
			{
				arg = (<>9__1 = ((ScriptExecutionContext c, CallbackArguments a) => function.Invoke(c, a.GetArray(0), false)));
			}
			return e2(ctx, args, arg);
		};
		env.Globals[func] = DynValue.NewCallback(hooked, function.Name);
		return hooked;
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x00039CBC File Offset: 0x00037EBC
	public void LuaStart()
	{
		this._mainLuaEnv = new Script
		{
			Options = 
			{
				ScriptLoader = this
			}
		};
		this._mainLuaEnv.Globals.Set("ConchShipRequire", DynValue.NewCallback(delegate(ScriptExecutionContext _, CallbackArguments args)
		{
			this.ConchShipRequire(args[0].String);
			return DynValue.Nil;
		}, null));
		this._mainLuaEnv.Globals.Set("print", DynValue.NewCallback(delegate(ScriptExecutionContext _, CallbackArguments args)
		{
			AdaptableLog.TagInfo("LuaGame", string.Join(string.Empty, from v in args.GetArray(0)
			select v.CastToString()));
			return DynValue.Nil;
		}, null));
		StandardGenericsUserDataDescriptor sharedDescriptor = new StandardGenericsUserDataDescriptor(typeof(List<DynValue>), InteropAccessMode.Default);
		Func<ScriptExecutionContext, CallbackArguments, DynValue> hookedNext = LuaGame.HookGlobalFunc(this._mainLuaEnv, "next", delegate(ScriptExecutionContext _, CallbackArguments args, Func<ScriptExecutionContext, CallbackArguments, DynValue> origin)
		{
			Table table = args[0].Table;
			DynValue key = args[1];
			UserData userData = table.Get("__cached_sorted_keys").UserData;
			List<DynValue> cachedKeys = (List<DynValue>)((userData != null) ? userData.Object : null);
			bool flag = cachedKeys == null || cachedKeys.Count != table.Length;
			if (flag)
			{
				cachedKeys = table.Keys.ToList<DynValue>();
				table.Set("__cached_sorted_keys", UserData.Create(cachedKeys, sharedDescriptor));
			}
			int index = key.IsNil() ? 0 : (cachedKeys.FindIndex((DynValue d) => d.Equals(key)) + 1);
			bool flag2 = cachedKeys.CheckIndex(index) && cachedKeys[index].String == "__cached_sorted_keys";
			if (flag2)
			{
				index++;
			}
			return cachedKeys.CheckIndex(index) ? DynValue.NewTuple(new DynValue[]
			{
				cachedKeys[index],
				table.Get(cachedKeys[index])
			}) : DynValue.Nil;
		});
		LuaGame.HookGlobalFunc(this._mainLuaEnv, "pairs", delegate(ScriptExecutionContext ctx, CallbackArguments args, Func<ScriptExecutionContext, CallbackArguments, DynValue> _)
		{
			DynValue dynValue = args[0];
			Table table = dynValue.Table;
			if (table != null)
			{
				table.Remove("__cached_sorted_keys");
			}
			DynValue result;
			if ((result = ctx.GetMetamethodTailCall(dynValue, "__pairs", args.GetArray(0))) == null)
			{
				result = DynValue.NewTuple(new DynValue[]
				{
					DynValue.NewCallback(hookedNext, null),
					dynValue
				});
			}
			return result;
		});
		ResLoader.Load<AssetBundle>("lua" + FrameCommon.AbSuffix, delegate(AssetBundle bundle)
		{
			this._luaFilesMap = new Dictionary<string, byte[]>();
			TextAsset[] textAssets = bundle.LoadAllAssets<TextAsset>();
			foreach (TextAsset ta in textAssets)
			{
				this._luaFilesMap.Add(ta.name, ta.bytes);
			}
			this.ConchShipRequire("Lua/1_LuaEntry");
			LuaGame.LuaReady = true;
		}, null, false);
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x00039DC8 File Offset: 0x00037FC8
	public Table GetGlobal()
	{
		Script mainLuaEnv = this._mainLuaEnv;
		return (mainLuaEnv != null) ? mainLuaEnv.Globals : null;
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x00039DEC File Offset: 0x00037FEC
	public Script GetMainEnv()
	{
		return this._mainLuaEnv;
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x00039E04 File Offset: 0x00038004
	public Table ReadMoonSharpTable(string chunkContent)
	{
		DynValue content = global::LuaParser.Parse(chunkContent, null);
		bool flag = content.Type == DataType.Table;
		Table result;
		if (flag)
		{
			result = content.Table;
		}
		else
		{
			result = null;
		}
		return result;
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x00039E38 File Offset: 0x00038038
	public object DoString(string chunkContent)
	{
		try
		{
			Script mainLuaEnv = this._mainLuaEnv;
			return (mainLuaEnv != null) ? mainLuaEnv.DoString(chunkContent, null, null).ToObject() : null;
		}
		catch (Exception e)
		{
			PredefinedLog.Show(29, e.Message);
		}
		return null;
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x00039E8C File Offset: 0x0003808C
	public void ConchShipRequire(string path)
	{
		string fileName = path.Replace("Lua/", string.Empty);
		byte[] buffer;
		bool flag = this._luaFilesMap.TryGetValue(fileName, out buffer);
		if (flag)
		{
			string code = Encoding.UTF8.GetString(buffer);
			Script mainLuaEnv = this._mainLuaEnv;
			if (mainLuaEnv != null)
			{
				mainLuaEnv.DoString(code, null, null);
			}
		}
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x00039EE0 File Offset: 0x000380E0
	private byte[] CustomLoader(ref string path)
	{
		path = path.Replace(".lua", "").Replace(".", "/");
		path += ".lua";
		return File.ReadAllBytes(path);
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x00039F2C File Offset: 0x0003812C
	public object LoadFile(string file, Table globalContext)
	{
		return Encoding.UTF8.GetString(this.CustomLoader(ref file));
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x00039F50 File Offset: 0x00038150
	public string ResolveFileName(string filename, Table globalContext)
	{
		return filename;
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x00039F53 File Offset: 0x00038153
	public string ResolveModuleName(string modname, Table globalContext)
	{
		return modname;
	}

	// Token: 0x04000B9B RID: 2971
	private Script _mainLuaEnv;

	// Token: 0x04000B9C RID: 2972
	public static LuaGame Instance;

	// Token: 0x04000B9E RID: 2974
	private Dictionary<string, byte[]> _luaFilesMap;

	// Token: 0x02001147 RID: 4423
	public readonly struct LuaFunctionProxy
	{
		// Token: 0x0600C1F4 RID: 49652 RVA: 0x00571588 File Offset: 0x0056F788
		public LuaFunctionProxy(Closure func)
		{
			this._luaFunction = func;
		}

		// Token: 0x170015E2 RID: 5602
		// (get) Token: 0x0600C1F5 RID: 49653 RVA: 0x00571592 File Offset: 0x0056F792
		public bool IsNull
		{
			get
			{
				return this._luaFunction == null;
			}
		}

		// Token: 0x0600C1F6 RID: 49654 RVA: 0x005715A0 File Offset: 0x0056F7A0
		public DynValue Call(params object[] args)
		{
			object[] converted = new object[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				object arg = args[i];
				Table argTable = arg as Table;
				bool flag = argTable != null;
				if (flag)
				{
					Table newTable = new Table(argTable.OwnerScript);
					foreach (DynValue key in argTable.Keys)
					{
						newTable[key] = argTable[key];
					}
					arg = newTable;
				}
				converted[i] = arg;
			}
			return this._luaFunction.Call(converted);
		}

		// Token: 0x04009674 RID: 38516
		private readonly Closure _luaFunction;
	}
}
