using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Config;
using Config.Common;
using FrameWork;
using FrameWork.ExternalTexture;
using FrameWork.Tools.Random;
using Game.Components.Avatar;
using Game.Views.Adventure;
using Game.Views.CharacterMenu;
using Game.Views.Combat;
using Game.Views.SettlementInformation;
using GameData;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Global;
using GameData.Domains.World;
using GameData.GameDataBridge;
using GameData.GameDataBridge.UnityAdapter;
using GameData.Utilities;
using Redzen.Random;
using UnityEngine;

// Token: 0x020000E5 RID: 229
public class GameApp : MonoBehaviour
{
	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x060007E6 RID: 2022 RVA: 0x00036D43 File Offset: 0x00034F43
	// (set) Token: 0x060007E7 RID: 2023 RVA: 0x00036D4A File Offset: 0x00034F4A
	public static bool GameResReady { get; private set; }

	// Token: 0x170000C3 RID: 195
	// (get) Token: 0x060007E8 RID: 2024 RVA: 0x00036D54 File Offset: 0x00034F54
	public static bool IsWindows7
	{
		get
		{
			return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1;
		}
	}

	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x060007E9 RID: 2025 RVA: 0x00036D9C File Offset: 0x00034F9C
	public Version ParsedGameVersion
	{
		get
		{
			bool flag = this._parsedGameVersion == null;
			if (flag)
			{
				this._parsedGameVersion = GameVersionInfo.ParseGameVersion(this.GameVersion);
			}
			return this._parsedGameVersion;
		}
	}

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x060007EA RID: 2026 RVA: 0x00036DD5 File Offset: 0x00034FD5
	public static IRandomSource Random
	{
		get
		{
			return GameApp.Instance._random;
		}
	}

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x060007EB RID: 2027 RVA: 0x00036DE1 File Offset: 0x00034FE1
	public bool IsTestBranch
	{
		get
		{
			return this._commandLineArgs.IsTestBranch;
		}
	}

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x060007EC RID: 2028 RVA: 0x00036DEE File Offset: 0x00034FEE
	public bool EnableGMPanel
	{
		get
		{
			return this._commandLineArgs.EnableGMPanel;
		}
	}

	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x060007ED RID: 2029 RVA: 0x00036DFB File Offset: 0x00034FFB
	public bool EnableEventEditor
	{
		get
		{
			return this._commandLineArgs.EnableEventEditor;
		}
	}

	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x060007EE RID: 2030 RVA: 0x00036E08 File Offset: 0x00035008
	public bool ShowBackendWindow
	{
		get
		{
			return this._commandLineArgs.ShowBackendWindow;
		}
	}

	// Token: 0x170000CA RID: 202
	// (get) Token: 0x060007EF RID: 2031 RVA: 0x00036E15 File Offset: 0x00035015
	public bool SkipBackend
	{
		get
		{
			return this._commandLineArgs.SkipBackend;
		}
	}

	// Token: 0x170000CB RID: 203
	// (get) Token: 0x060007F0 RID: 2032 RVA: 0x00036E22 File Offset: 0x00035022
	public bool AdvanceMonthSingleThread
	{
		get
		{
			return this._commandLineArgs.AdvanceMonthSingleThread;
		}
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x00036E30 File Offset: 0x00035030
	private void Awake()
	{
		AdaptableLog.Initialize(new Action<string>(Debug.Log), new Action<string>(Debug.LogWarning), new Action<string>(Debug.LogError));
		LogHandle.OnAppendMessage += delegate(string message)
		{
			Debug.LogError(message.SetColor(Color.yellow));
		};
		ExternalDataBridge.Initialize(new GameContext());
		Debug.Log(string.Format("Game start at {0}", DateTime.Now));
		Debug.Log("Game version = " + this.GameVersion);
		Debug.Log("System = " + SystemInfo.operatingSystem);
		Debug.Log(string.Format("System.CPU = {0} System.CPUCoreCount = {1} System.systemMemorySize = {2:F2}GB", SystemInfo.processorType, SystemInfo.processorCount, (float)SystemInfo.systemMemorySize / 1024f));
		Debug.Log(string.Format("System.graphicsDeviceName = {0} System.graphicsMemorySize = {1:F2}GB", SystemInfo.graphicsDeviceName, (float)SystemInfo.graphicsMemorySize / 1024f));
		GameApp.Instance = this;
		GameApp.GameResReady = false;
		GameApp.Quiting = false;
		Object.DontDestroyOnLoad(base.gameObject);
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		Application.targetFrameRate = 60;
		Time.timeScale = 1f;
		this._random = new CustomRandom();
		this._machine = new GameStateMachine();
		GameApp.InitPath();
	}

	// Token: 0x060007F2 RID: 2034 RVA: 0x00036FA1 File Offset: 0x000351A1
	private void Start()
	{
		Debug.Log("Game started.");
		SteamManager.Initialize();
		Application.logMessageReceived += GameApp.HandleUnhandledException;
		this.InitCommandLineArgs();
		base.StartCoroutine(this.<Start>g__Entry|35_0());
	}

	// Token: 0x060007F3 RID: 2035 RVA: 0x00036FE0 File Offset: 0x000351E0
	private void Update()
	{
		SteamManager.Update();
		bool flag = GameApp.IsToggleFullscreenShortcutPressed();
		if (flag)
		{
			GlobalSettings settingData = SingletonObject.getInstance<GlobalSettings>();
			settingData.FullScreen = !settingData.FullScreen;
		}
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x00037015 File Offset: 0x00035215
	private void OnDestroy()
	{
		SteamManager.UnInitialize();
		Application.logMessageReceived -= GameApp.HandleUnhandledException;
		GameApp.Quiting = true;
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x00037036 File Offset: 0x00035236
	public void Reload()
	{
		Debug.Log("Starting reload coroutine...");
		base.StartCoroutine(this.PrewarmGameRes());
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x00037054 File Offset: 0x00035254
	public void ReStart()
	{
		string[] strs = new string[]
		{
			"@echo off",
			"taskkill /f /t /im \"The Scroll of Taiwu.exe\"",
			"start steam://rungameid/838350",
			"exit"
		};
		string batPath = Application.dataPath + "/../restart.bat";
		bool flag = File.Exists(batPath);
		if (flag)
		{
			File.Delete(batPath);
		}
		using (FileStream fileStream = File.OpenWrite(batPath))
		{
			using (StreamWriter writer = new StreamWriter(fileStream, Encoding.ASCII))
			{
				foreach (string s in strs)
				{
					writer.WriteLine(s);
				}
				writer.Close();
			}
		}
		Application.OpenURL(batPath);
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x00037138 File Offset: 0x00035338
	private IEnumerator PrewarmGameRes()
	{
		Debug.Log("Pre-warming...");
		float clock = Time.realtimeSinceStartup;
		GameApp.ClockAndLogInfo("start initialize all game resources....", true);
		AtlasInfo.Init();
		TextureInfo.Init();
		AvatarSetting.Init();
		EventTextureManager.InitData();
		GameApp.ClockAndLogInfo("sprite atlas preload complete ....", false);
		yield return new WaitForEndOfFrame();
		GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
		settings.EnsureLoaded();
		string langKey = settings.Language;
		GameApp.ClockAndLogInfo("load language settings complete ....", false);
		LocalStringManager.Init(langKey);
		yield return new WaitUntil(() => LocalStringManager.ConfigLanguageInitReady);
		GameApp.ClockAndLogInfo("load local strings complete ....", false);
		Task<ParallelLoopResult> initCfgTask = Task.Run<ParallelLoopResult>(() => Parallel.ForEach<IConfigData>(ConfigCollection.Items, delegate(IConfigData item)
		{
			item.Init();
		}));
		while (!initCfgTask.IsCompleted)
		{
			yield return null;
		}
		bool flag = initCfgTask.Exception != null;
		if (flag)
		{
			throw initCfgTask.Exception;
		}
		RefNameMap.DoQueuedLoadRequests();
		SensitiveWordsSystem.Instance.Init();
		AdventureRemakeModel.InitializeAdventureCore();
		GameApp.ClockAndLogInfo("all config data load complete ....", false);
		yield return new WaitUntil(() => null != AtlasInfo.Instance);
		GameApp.ClockAndLogInfo("AtlasInfo initialize complete ....", false);
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		box.SetObject("OnLoadingStart", new Action(delegate
		{
			ConchShipCursor.Instance.SetCursorVisible(true);
			base.StartCoroutine(this.InitGameRes());
		}));
		box.SetObject("OnLoadingFinish", new Action(delegate
		{
			GEvent.OnEvent(EEvents.OnGameResourceReady, null);
			GameApp.ClockAndLogInfo("switch game state to LogoShow complete ....", false);
		}));
		float passTime = Time.realtimeSinceStartup - clock;
		bool flag2 = passTime < 2f;
		if (flag2)
		{
			yield return new WaitForSeconds(2f - passTime);
		}
		this.ChangeGameState(EGameState.Loading, box);
		yield break;
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x00037147 File Offset: 0x00035347
	private IEnumerator InitGameRes()
	{
		LuaGame.Instance = new LuaGame();
		LuaGame.Instance.LuaStart();
		yield return new WaitUntil(() => LuaGame.LuaReady);
		GameApp.ClockAndLogInfo("Lua VM initialized ....", false);
		SingletonObject.getInstance<GlobalSettings>().EnsureLoaded();
		GameApp.ClockAndLogInfo("global settings load complete ....", false);
		GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 10));
		yield return null;
		CommandKitBase.Init();
		GameApp.ClockAndLogInfo("hot key settings initialized ....", false);
		GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 20));
		yield return null;
		AvatarAtlasAssets.Init();
		GameApp.ClockAndLogInfo("avatar atlas load complete ....", false);
		GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 30));
		yield return null;
		SingletonObject.getInstance<DlcManager>();
		GameApp.ClockAndLogInfo("dlc load complete, enter game main menu enabled ....", false);
		GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 40));
		SingletonObject.getInstance<TextureCenter>();
		IEnumerator item = ModManager.Init();
		while (item.MoveNext())
		{
			object obj = item.Current;
			yield return obj;
		}
		item = ModManager.LoadAllEnabledMods();
		while (item.MoveNext())
		{
			object obj2 = item.Current;
			yield return obj2;
		}
		GameApp.ClockAndLogInfo("Mod module load complete ....", false);
		GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 50));
		yield return null;
		while (!GameDataBridgeUnityAdapter.IsConnected)
		{
			bool flag = GameDataBridge.CheckConnection();
			if (flag)
			{
				GameDataBridgeUnityAdapter.IsConnected = true;
				GameDataBridge.AllowSendingInitializationMessage();
				break;
			}
			yield return null;
		}
		GameApp.ClockAndLogInfo("game data connected ....", false);
		GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 60));
		yield return null;
		AvatarManager.Instance = SingletonObject.getInstance<AvatarManager>();
		AvatarManager.Instance.InitAvatarCore(true, new Action(AvatarManagerUtils.InitModAvatar), false);
		GameApp.ClockAndLogInfo("avatar manager init complete ....", false);
		GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 70));
		yield return null;
		yield return base.StartCoroutine(this.PreloadComplexPrefabs());
		while (GameDataBridge.GetGameDataModuleInitializationState() != 3)
		{
			yield return null;
		}
		GameApp.ClockAndLogInfo("game data initialized ....", false);
		GameApp.GameResReady = true;
		this.BindGlobalEvents();
		GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 100));
		GlobalDomainMethod.Call.SetGameBuildInfo(this.GameVersion, this.GameBuildDate);
		bool flag2 = !SingletonObject.getInstance<GlobalSettings>().HaveShowLogo && SingletonObject.getInstance<GlobalSettings>().HaveDoneSave;
		if (flag2)
		{
			this.ChangeGameState(EGameState.Login, null);
		}
		yield break;
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x00037156 File Offset: 0x00035356
	private IEnumerator PreloadComplexPrefabs()
	{
		SingletonObject.getInstance<ItemViewPool>();
		UIElement.CharacterMenu.PrepareRes(false, null, false);
		yield return new WaitUntil(() => null != UIElement.CharacterMenu.UiBase);
		UIElement.CharacterMenu.UiBase.OnReset();
		GameApp.ClockAndLogInfo("ui_charactermenu preload complete ....", false);
		yield return null;
		UIElement.Combat.PrepareRes(false, null, false);
		yield return new WaitUntil(() => null != UIElement.Combat.UiBase);
		UIElement.Combat.UiBaseAs<ViewCombat>().Preload();
		GameApp.ClockAndLogInfo("ui_combat preload complete ....", false);
		yield return null;
		UIElement.AdventureRemake.PrepareRes(false, null, false);
		yield return new WaitUntil(() => null != UIElement.AdventureRemake.UiBase);
		UIElement.AdventureRemake.UiBaseAs<ViewAdventureRemake>().Preload();
		GameApp.ClockAndLogInfo("ui_adventureRemake preload complete ....", false);
		yield return null;
		UIElement.PartWorld.PrepareRes(false, null, false);
		yield return new WaitUntil(() => null != UIElement.PartWorld.UiBase);
		GameApp.ClockAndLogInfo("ui_partworldmap preload complete ....", false);
		yield return null;
		int num;
		for (int i = 0; i < UIElement.PreloadElements.Count; i = num + 1)
		{
			UIElement.PreloadElements[i].PrepareRes(false, null, false);
			num = i;
		}
		GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 90));
		yield break;
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x00037165 File Offset: 0x00035365
	[RuntimeInitializeOnLoadMethod]
	private static void RuntimeOnLoad()
	{
		Application.wantsToQuit += GameApp.GameQuitConfirm;
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0003717C File Offset: 0x0003537C
	public static bool GameQuitConfirm()
	{
		SingletonObject.getInstance<GlobalSettings>().SaveSettings();
		bool flag = GameApp.ReadyToQuit || UIElement.LogoShow.Exist;
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			bool hasUnhandledExceptionOccurred = GameApp._hasUnhandledExceptionOccurred;
			if (hasUnhandledExceptionOccurred)
			{
				result = true;
			}
			else
			{
				DialogCmd dialogCmd = new DialogCmd();
				dialogCmd.Title = LocalStringManager.Get(LanguageKey.LK_Exit_Game);
				dialogCmd.Content = LocalStringManager.Get(LanguageKey.LK_UI_Exit_Confirm);
				dialogCmd.Yes = delegate()
				{
					GameApp.ReadyToQuit = true;
					Application.Quit();
				};
				DialogCmd cmd = dialogCmd;
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				box.SetObject("Cmd", cmd);
				UIElement.Dialog.SetOnInitArgs(box);
				UIManager.Instance.MaskUI(UIElement.Dialog);
				result = false;
			}
		}
		return result;
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x00037242 File Offset: 0x00035442
	private static void InitPath()
	{
		GameApp.DataPath = Application.dataPath;
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x00037250 File Offset: 0x00035450
	private void InitCommandLineArgs()
	{
		Debug.Log("Initializing command line args...");
		this._commandLineArgs = new GameApp.CommandLineArgs();
		bool flag = !File.Exists(Path.Combine(GameApp.DataPath, "disable_cmd_args.txt"));
		if (flag)
		{
			string[] args = Environment.GetCommandLineArgs();
			foreach (string arg in args)
			{
				Debug.Log("Command line arg: " + arg);
				string text = arg;
				string a = text;
				if (!(a == "--enable-gm"))
				{
					if (!(a == "--enable-event-editor"))
					{
						if (!(a == "--test-branch"))
						{
							if (!(a == "--backend-window"))
							{
								if (!(a == "--skip-backend"))
								{
									if (a == "--advance-month-singlethread")
									{
										this._commandLineArgs.AdvanceMonthSingleThread = true;
									}
								}
								else
								{
									this._commandLineArgs.SkipBackend = true;
								}
							}
							else
							{
								this._commandLineArgs.ShowBackendWindow = true;
							}
						}
						else
						{
							this._commandLineArgs.IsTestBranch = true;
						}
					}
					else
					{
						this._commandLineArgs.EnableEventEditor = true;
					}
				}
				else
				{
					this._commandLineArgs.EnableGMPanel = true;
				}
			}
		}
		string branchName = SteamManager.Branch;
		bool flag2 = !string.IsNullOrEmpty(branchName);
		if (flag2)
		{
			this._commandLineArgs.IsTestBranch = !branchName.Contains("pre-release");
			bool flag3 = branchName == "event-test";
			if (flag3)
			{
				this._commandLineArgs.EnableEventEditor = true;
			}
		}
		bool isTestBranch = this._commandLineArgs.IsTestBranch;
		if (isTestBranch)
		{
			this.GameVersion += "-test";
		}
		bool enableGMPanel = this._commandLineArgs.EnableGMPanel;
		if (enableGMPanel)
		{
			this.GameVersion += "-gm";
		}
		bool enableEventEditor = this._commandLineArgs.EnableEventEditor;
		if (enableEventEditor)
		{
			this.GameVersion += "-event-editor";
		}
		bool advanceMonthSingleThread = this._commandLineArgs.AdvanceMonthSingleThread;
		if (advanceMonthSingleThread)
		{
			this.GameVersion += "-singlethread";
		}
		Debug.Log("GameVersion: " + this.GameVersion);
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x00037480 File Offset: 0x00035680
	private void BindGlobalEvents()
	{
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x00037484 File Offset: 0x00035684
	public EGameState GetCurrentGameStateName()
	{
		State gameState = this._machine.GetCurrentState() as State;
		return ((EGameState?)((gameState != null) ? gameState.stateName : null)).GetValueOrDefault();
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x000374C4 File Offset: 0x000356C4
	public void ChangeGameState(EGameState newState, ArgumentBox argsBox = null)
	{
		EGameState preState = this.GetCurrentGameStateName();
		this._machine.TranslateState(newState, argsBox);
		GEvent.OnEvent(EEvents.OnGameStateChange, EasyPool.Get<ArgumentBox>().Set("preState", preState).Set("newState", newState));
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x00037520 File Offset: 0x00035720
	public static void ReturnToMainMenu(ArgumentBox box = null, Action onShowAction = null, Action onHideAction = null)
	{
		bool flag = SingletonObject.IsCreatedInstance<WorldMapModel>();
		if (flag)
		{
			SingletonObject.getInstance<WorldMapModel>().ChangeTaiwuMoveState(WorldMapModel.MoveState.Idle);
		}
		ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
		argBox.SetObject("OnLoadingStart", new Action(delegate
		{
			bool flag4 = box == null;
			if (flag4)
			{
				box = EasyPool.Get<ArgumentBox>();
			}
			GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 100));
			UIManager.Instance.HideAll();
			GameApp.Instance.ChangeGameState(EGameState.Login, box.Set("IsBack", true));
		}));
		argBox.SetObject("OnLoadingFinish", new Action(delegate
		{
			UIManager.Instance.DestroyAll(new List<UIElement>
			{
				UIElement.Loading,
				UIElement.MainMenu,
				UIElement.Combat,
				UIElement.PermanentTips
			}, false);
			SingletonObject.ClearInstances();
		}));
		bool flag2 = onShowAction != null;
		if (flag2)
		{
			UIElement loading = UIElement.Loading;
			loading.OnShowed = (Action)Delegate.Combine(loading.OnShowed, onShowAction);
		}
		bool flag3 = onHideAction != null;
		if (flag3)
		{
			UIElement loading2 = UIElement.Loading;
			loading2.OnHide = (Action)Delegate.Combine(loading2.OnHide, onHideAction);
		}
		GameApp.Instance.ChangeGameState(EGameState.Loading, argBox);
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x000375F0 File Offset: 0x000357F0
	public static void LoadArchive(sbyte index, bool isDreamBack = false)
	{
		GameApp.<>c__DisplayClass51_0 CS$<>8__locals1 = new GameApp.<>c__DisplayClass51_0();
		CS$<>8__locals1.isDreamBack = isDreamBack;
		CS$<>8__locals1.index = index;
		GameApp.Instance.ChangeGameState(EGameState.Loading, EasyPool.Get<ArgumentBox>().SetObject("OnLoadingFinish", new Action(GameApp.<LoadArchive>g__OnEnterWorldLoadFinish|51_1)).SetObject("OnLoadingStart", new Action(CS$<>8__locals1.<LoadArchive>g__OnEnterWorldLoadStart|0)));
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x00037654 File Offset: 0x00035854
	public static void EnterInGameGuideWorld()
	{
		GameApp.Instance.ChangeGameState(EGameState.Loading, EasyPool.Get<ArgumentBox>().SetObject("OnLoadingFinish", new Action(GameApp.<EnterInGameGuideWorld>g__OnEnterWorldLoadFinish|52_1)).SetObject("OnLoadingStart", new Action(GameApp.<EnterInGameGuideWorld>g__OnEnterWorldLoadStart|52_0)));
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x000376A4 File Offset: 0x000358A4
	public static void ExitInGameGuideWorld()
	{
		GameApp.Instance.ChangeGameState(EGameState.Loading, EasyPool.Get<ArgumentBox>().SetObject("OnLoadingFinish", new Action(GameApp.<ExitInGameGuideWorld>g__OnEnterWorldLoadFinish|53_1)).SetObject("OnLoadingStart", new Action(GameApp.<ExitInGameGuideWorld>g__OnEnterWorldLoadStart|53_0)));
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x000376F4 File Offset: 0x000358F4
	public static void PlayCg(string cgName, Action onComplete)
	{
		bool flag = string.IsNullOrEmpty(cgName);
		if (!flag)
		{
			bool flag2 = onComplete != null;
			if (flag2)
			{
				UIElement cgPlayer = UIElement.CgPlayer;
				cgPlayer.OnDeActive = (Action)Delegate.Combine(cgPlayer.OnDeActive, onComplete);
			}
			UIElement.CgPlayer.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("CGName", cgName));
			UIManager.Instance.ShowUI(UIElement.CgPlayer, true);
		}
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x00037760 File Offset: 0x00035960
	public static int RandomRange(int min, int max)
	{
		bool flag = null != GameApp.Instance;
		int result;
		if (flag)
		{
			result = GameApp.Instance._random.Next(min, max);
		}
		else
		{
			result = UnityEngine.Random.Range(min, max);
		}
		return result;
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x0003779C File Offset: 0x0003599C
	public static float RandomRange(float min, float max)
	{
		bool flag = null != GameApp.Instance;
		float result;
		if (flag)
		{
			result = (float)GameApp.RandomRange((int)(min * 100f), (int)(max * 100f)) * 0.01f;
		}
		else
		{
			result = UnityEngine.Random.Range(min, max);
		}
		return result;
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x000377E4 File Offset: 0x000359E4
	public static string GetArchiveDirPath()
	{
		string suffix = (GameApp.Instance != null && GameApp.Instance._commandLineArgs.IsTestBranch) ? "SaveGames_test" : "SaveGames";
		string baseDataDir = Application.dataPath;
		string path = Path.Combine(baseDataDir, "..", suffix);
		return Path.GetFullPath(path);
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x0003783A File Offset: 0x00035A3A
	public static void ResetGameSubPageState()
	{
		ViewCharacterMenu.NeedClear = true;
		CharacterAttributeDataView.CurTabIndex = 0;
		ViewSettlementInformation.LastOpenSettlementId = -1;
		AvatarAdjustController.AvatarGroupInitState = false;
	}

	// Token: 0x0600080A RID: 2058 RVA: 0x00037858 File Offset: 0x00035A58
	public static void ClockAndLogInfo(string info, bool isReset)
	{
		if (isReset)
		{
			GLog.TagLog("Clock", string.Format("[{0:HH:mm:ss.ffff}]{1}: Clock Reset!!!", DateTime.Now, info), Array.Empty<object>());
		}
		else
		{
			GLog.TagLog("Clock", string.Format("[{0:HH:mm:ss.ffff}]{1}: {2:F1} ms", DateTime.Now, info, (Time.realtimeSinceStartup - GameApp.Clock) * 1000f), Array.Empty<object>());
		}
		GameApp.Clock = Time.realtimeSinceStartup;
	}

	// Token: 0x170000CC RID: 204
	// (get) Token: 0x0600080B RID: 2059 RVA: 0x000378DC File Offset: 0x00035ADC
	// (set) Token: 0x0600080C RID: 2060 RVA: 0x000378F3 File Offset: 0x00035AF3
	public static bool HasUnhandledExceptionOccurred
	{
		get
		{
			return GameApp._hasUnhandledExceptionOccurred;
		}
		set
		{
			GameApp._hasUnhandledExceptionOccurred = value;
		}
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x000378FC File Offset: 0x00035AFC
	public static void HandleUnhandledException(string logString, string stackTrace, LogType type)
	{
		bool flag = type == LogType.Exception;
		if (flag)
		{
			GameApp._hasUnhandledExceptionOccurred = true;
		}
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x0003791A File Offset: 0x00035B1A
	public static void QuitGame()
	{
		Application.Quit();
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x00037924 File Offset: 0x00035B24
	public static bool IsConnectedToInternet()
	{
		bool isConnected = false;
		foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
		{
			bool flag = netInterface.OperationalStatus == OperationalStatus.Up && netInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback;
			if (flag)
			{
				isConnected = true;
				break;
			}
		}
		return isConnected;
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x0003797C File Offset: 0x00035B7C
	private static bool IsToggleFullscreenShortcutPressed()
	{
		return Input.GetKeyDown(KeyCode.Return) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x000379BC File Offset: 0x00035BBC
	[CompilerGenerated]
	private IEnumerator <Start>g__Entry|35_0()
	{
		yield return new WaitForEndOfFrame();
		GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
		settings.EnsureLoaded();
		bool flag = !settings.HaveDoneSave;
		if (flag)
		{
			Debug.Log("Show logo...");
			UIElement logoShow = UIElement.LogoShow;
			logoShow.OnShowed = (Action)Delegate.Combine(logoShow.OnShowed, new Action(this.Reload));
			UIElement.LogoShow.Show();
		}
		else
		{
			this.Reload();
		}
		yield break;
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x000379F8 File Offset: 0x00035BF8
	[CompilerGenerated]
	internal static void <LoadArchive>g__OnEnterWorldLoadFinish|51_1()
	{
		PoolManager.CleanPool();
		UIElement worldMap = UIElement.WorldMap;
		worldMap.OnShowed = (Action)Delegate.Combine(worldMap.OnShowed, new Action(delegate()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("AnimToShowMask", false);
			argBox.Set("AnimTime", 4.3f);
			argBox.Set("HideAfterShow", true);
			UIElement.BlackMask.SetOnInitArgs(argBox);
			UIElement.BlackMask.Show();
		}));
		GameApp.Instance.ChangeGameState(EGameState.InGame, null);
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x00037A54 File Offset: 0x00035C54
	[CompilerGenerated]
	internal static void <EnterInGameGuideWorld>g__OnEnterWorldLoadStart|52_0()
	{
		UIManager.Instance.HideAll();
		SingletonObject.RemoveInstance<CharacterMonitorModel>();
		GlobalOperations.OnLeaveWorld();
		UIManager.Instance.DestroyAll(new List<UIElement>
		{
			UIElement.Loading,
			UIElement.MainMenu,
			UIElement.Combat,
			UIElement.PermanentTips
		}, false);
		GameApp.ResetGameSubPageState();
		SingletonObject.ClearInstances();
		GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 50));
		GlobalOperations.EnterInGameGuideWorld();
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x00037AEC File Offset: 0x00035CEC
	[CompilerGenerated]
	internal static void <EnterInGameGuideWorld>g__OnEnterWorldLoadFinish|52_1()
	{
		PoolManager.CleanPool();
		UIElement worldMap = UIElement.WorldMap;
		worldMap.OnShowed = (Action)Delegate.Combine(worldMap.OnShowed, new Action(delegate()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("AnimToShowMask", false);
			argBox.Set("AnimTime", 4.3f);
			argBox.Set("HideAfterShow", true);
			UIElement.BlackMask.SetOnInitArgs(argBox);
			UIElement.BlackMask.Show();
		}));
		GameApp.Instance.ChangeGameState(EGameState.InGame, null);
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x00037B48 File Offset: 0x00035D48
	[CompilerGenerated]
	internal static void <ExitInGameGuideWorld>g__OnEnterWorldLoadStart|53_0()
	{
		UIManager.Instance.HideAll();
		SingletonObject.RemoveInstance<CharacterMonitorModel>();
		GlobalOperations.OnLeaveWorld();
		UIManager.Instance.DestroyAll(new List<UIElement>
		{
			UIElement.Loading,
			UIElement.MainMenu,
			UIElement.Combat,
			UIElement.PermanentTips
		}, false);
		GameApp.ResetGameSubPageState();
		SingletonObject.ClearInstances();
		GEvent.OnEvent(EEvents.LoadingProgress, EasyPool.Get<ArgumentBox>().Set("Progress", 50));
		GlobalOperations.ExitInGameGuideWorld();
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x00037BE0 File Offset: 0x00035DE0
	[CompilerGenerated]
	internal static void <ExitInGameGuideWorld>g__OnEnterWorldLoadFinish|53_1()
	{
		PoolManager.CleanPool();
		UIElement worldMap = UIElement.WorldMap;
		worldMap.OnShowed = (Action)Delegate.Combine(worldMap.OnShowed, new Action(delegate()
		{
			ArgumentBox argBox = EasyPool.Get<ArgumentBox>();
			argBox.Set("AnimToShowMask", false);
			argBox.Set("AnimTime", 4.3f);
			argBox.Set("HideAfterShow", true);
			UIElement.BlackMask.SetOnInitArgs(argBox);
			UIElement.BlackMask.Show();
		}));
		GameApp.Instance.ChangeGameState(EGameState.InGame, null);
	}

	// Token: 0x04000A7A RID: 2682
	public static GameApp Instance;

	// Token: 0x04000A7B RID: 2683
	private GameStateMachine _machine;

	// Token: 0x04000A7D RID: 2685
	public static bool Quiting;

	// Token: 0x04000A7E RID: 2686
	public static string DataPath;

	// Token: 0x04000A7F RID: 2687
	public static bool ReadyToQuit;

	// Token: 0x04000A80 RID: 2688
	public string GameVersion;

	// Token: 0x04000A81 RID: 2689
	public string GameBuildDate;

	// Token: 0x04000A82 RID: 2690
	private Version _parsedGameVersion;

	// Token: 0x04000A83 RID: 2691
	private IRandomSource _random;

	// Token: 0x04000A84 RID: 2692
	public static bool AdvancingMonth;

	// Token: 0x04000A85 RID: 2693
	private GameApp.CommandLineArgs _commandLineArgs;

	// Token: 0x04000A86 RID: 2694
	public static float Clock;

	// Token: 0x04000A87 RID: 2695
	private static bool _hasUnhandledExceptionOccurred;

	// Token: 0x0200113D RID: 4413
	public class CommandLineArgs
	{
		// Token: 0x04009641 RID: 38465
		public bool IsTestBranch;

		// Token: 0x04009642 RID: 38466
		public bool EnableGMPanel;

		// Token: 0x04009643 RID: 38467
		public bool EnableEventEditor;

		// Token: 0x04009644 RID: 38468
		public bool ShowBackendWindow;

		// Token: 0x04009645 RID: 38469
		public bool SkipBackend;

		// Token: 0x04009646 RID: 38470
		public bool AdvanceMonthSingleThread;
	}
}
