using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Config;
using FrameWork;
using Game.Components.Combat;
using Game.Views.SystemSetting;
using GameData.Domains.Global;
using GameData.Domains.TaiwuEvent;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x0200012F RID: 303
public class GlobalSettings : GameData.Serializer.ICommonObjectSerializationAware
{
	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06000D68 RID: 3432 RVA: 0x00058257 File Offset: 0x00056457
	// (set) Token: 0x06000D69 RID: 3433 RVA: 0x0005825F File Offset: 0x0005645F
	[GlobalSettingString(ESettingSubCategory.Other, "")]
	public string FavoritEncyclopediaData { get; set; }

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000D6A RID: 3434 RVA: 0x00058268 File Offset: 0x00056468
	// (set) Token: 0x06000D6B RID: 3435 RVA: 0x00058270 File Offset: 0x00056470
	[GlobalSettingBool(ESettingSubCategory.Other, true)]
	public bool UsingCarrierFirst { get; set; }

	// Token: 0x17000136 RID: 310
	// (get) Token: 0x06000D6C RID: 3436 RVA: 0x00058279 File Offset: 0x00056479
	// (set) Token: 0x06000D6D RID: 3437 RVA: 0x00058281 File Offset: 0x00056481
	[GlobalSettingBool(ESettingSubCategory.Other, false)]
	public bool AreaStateHideAll { get; set; }

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x06000D6E RID: 3438 RVA: 0x0005828A File Offset: 0x0005648A
	// (set) Token: 0x06000D6F RID: 3439 RVA: 0x00058292 File Offset: 0x00056492
	[GlobalSettingString(ESettingSubCategory.Other, "")]
	public string AreaStateDisabledData { get; set; } = "";

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06000D70 RID: 3440 RVA: 0x0005829B File Offset: 0x0005649B
	// (set) Token: 0x06000D71 RID: 3441 RVA: 0x000582A3 File Offset: 0x000564A3
	[GlobalSettingBool(ESettingSubCategory.Tips, false)]
	public bool TipsAutoFixed { get; set; }

	// Token: 0x17000139 RID: 313
	// (get) Token: 0x06000D72 RID: 3442 RVA: 0x000582AC File Offset: 0x000564AC
	// (set) Token: 0x06000D73 RID: 3443 RVA: 0x000582B4 File Offset: 0x000564B4
	[GlobalSettingFloat(ESettingSubCategory.Tips, 0.35f)]
	public float TipsAutoFixedTime { get; set; }

	// Token: 0x1700013A RID: 314
	// (get) Token: 0x06000D74 RID: 3444 RVA: 0x000582BD File Offset: 0x000564BD
	// (set) Token: 0x06000D75 RID: 3445 RVA: 0x000582C5 File Offset: 0x000564C5
	[GlobalSettingFloat(ESettingSubCategory.Tips, 0.25f)]
	public float TipsTriggerTime { get; set; }

	// Token: 0x1700013B RID: 315
	// (get) Token: 0x06000D76 RID: 3446 RVA: 0x000582CE File Offset: 0x000564CE
	// (set) Token: 0x06000D77 RID: 3447 RVA: 0x000582D6 File Offset: 0x000564D6
	[GlobalSettingSbyte(ESettingSubCategory.Tips, 50)]
	public sbyte TipsTriggerSpeed { get; set; }

	// Token: 0x1700013C RID: 316
	// (get) Token: 0x06000D78 RID: 3448 RVA: 0x000582DF File Offset: 0x000564DF
	// (set) Token: 0x06000D79 RID: 3449 RVA: 0x000582E7 File Offset: 0x000564E7
	[GlobalSettingBool(ESettingSubCategory.Tips, false)]
	public bool TipsContinuousTrigger { get; set; }

	// Token: 0x1700013D RID: 317
	// (get) Token: 0x06000D7A RID: 3450 RVA: 0x000582F0 File Offset: 0x000564F0
	// (set) Token: 0x06000D7B RID: 3451 RVA: 0x000582F8 File Offset: 0x000564F8
	[GlobalSettingBool(ESettingSubCategory.Tips, false)]
	public bool GlobalTipsHide { get; set; }

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x06000D7C RID: 3452 RVA: 0x00058301 File Offset: 0x00056501
	// (set) Token: 0x06000D7D RID: 3453 RVA: 0x00058309 File Offset: 0x00056509
	[GlobalSettingSbyte(ESettingSubCategory.Tips, 0)]
	public sbyte SpecialEffectDescDetailLevel { get; set; }

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000D7E RID: 3454 RVA: 0x00058312 File Offset: 0x00056512
	// (set) Token: 0x06000D7F RID: 3455 RVA: 0x0005831C File Offset: 0x0005651C
	public Vector2Int Resolution
	{
		get
		{
			return this._resolution;
		}
		set
		{
			this._resolution = value;
			int x = this._resolution.x;
			int y = this._resolution.y;
			bool flag = x == 1360 && y == 768;
			if (flag)
			{
				y = 765;
			}
			Screen.SetResolution(x, y, this._fullScreen);
			Debug.Log(string.Format("System.ScreenResolution = {0}", Screen.currentResolution));
		}
	}

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06000D80 RID: 3456 RVA: 0x0005838E File Offset: 0x0005658E
	// (set) Token: 0x06000D81 RID: 3457 RVA: 0x00058398 File Offset: 0x00056598
	[GlobalSettingBool(ESettingSubCategory.Video, true)]
	public bool FullScreen
	{
		get
		{
			return this._fullScreen;
		}
		set
		{
			this._fullScreen = value;
			bool fullScreen = this._fullScreen;
			if (fullScreen)
			{
				Vector2Int maxResolution = GlobalSettings.GetMaxResolution();
				this.Resolution = maxResolution;
			}
			else
			{
				this.Resolution = this._resolution;
			}
			GEvent.OnEvent(EEvents.OnFullScreenChange, EasyPool.Get<ArgumentBox>().Set("FullScreen", this._fullScreen));
		}
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x000583F8 File Offset: 0x000565F8
	public static Vector2Int GetMaxResolution()
	{
		Resolution[] resolutions = Screen.resolutions;
		StringBuilder logSb = EasyPool.Get<StringBuilder>();
		logSb.Clear();
		logSb.AppendLine("Screen.Resolutions:[");
		foreach (Resolution resolution in resolutions)
		{
			logSb.Append("  ").Append(resolution.width).Append("x").Append(resolution.height).AppendLine();
		}
		logSb.Append("]");
		Debug.Log(logSb.ToString());
		int aspectRatioWidth = AspectRatioController.Instance.AspectRatioWidth;
		int aspectRatioHeight = AspectRatioController.Instance.AspectRatioHeight;
		Resolution maxWidthResolution = CommonUtils.GetBestMaxResolution(resolutions);
		Debug.Log(string.Format("MaxWidthResolution: {0}x{1}", maxWidthResolution.width, maxWidthResolution.height));
		int width = maxWidthResolution.width / aspectRatioWidth * aspectRatioWidth;
		int height = aspectRatioHeight * width / aspectRatioWidth;
		bool flag = height > maxWidthResolution.height;
		if (flag)
		{
			height = maxWidthResolution.height / aspectRatioHeight * aspectRatioHeight;
			width = aspectRatioWidth * height / aspectRatioHeight;
		}
		Vector2Int maxResolution = new Vector2Int(width, height);
		return maxResolution;
	}

	// Token: 0x17000141 RID: 321
	// (get) Token: 0x06000D83 RID: 3459 RVA: 0x0005852A File Offset: 0x0005672A
	// (set) Token: 0x06000D84 RID: 3460 RVA: 0x00058532 File Offset: 0x00056732
	[GlobalSettingBool(ESettingSubCategory.Video, true)]
	public bool VSync
	{
		get
		{
			return this._vSync;
		}
		set
		{
			this._vSync = value;
			QualitySettings.vSyncCount = (this._vSync ? 1 : 0);
		}
	}

	// Token: 0x17000142 RID: 322
	// (get) Token: 0x06000D85 RID: 3461 RVA: 0x0005854E File Offset: 0x0005674E
	// (set) Token: 0x06000D86 RID: 3462 RVA: 0x00058556 File Offset: 0x00056756
	[GlobalSettingBool(ESettingSubCategory.Audio, true)]
	public bool BgmOn
	{
		get
		{
			return this._bgmOn;
		}
		set
		{
			this._bgmOn = value;
			AudioManager.Instance.SetMusicMute(!this._bgmOn);
		}
	}

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x06000D87 RID: 3463 RVA: 0x00058574 File Offset: 0x00056774
	// (set) Token: 0x06000D88 RID: 3464 RVA: 0x0005857C File Offset: 0x0005677C
	[GlobalSettingSbyte(ESettingSubCategory.Audio, 60)]
	public sbyte BgmVolume
	{
		get
		{
			return this._bgmVolume;
		}
		set
		{
			this._bgmVolume = value;
			AudioManager.Instance.SetMusicVolume(this._bgmOn ? ((float)this._bgmVolume / 100f) : 0f, true);
		}
	}

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x06000D89 RID: 3465 RVA: 0x000585AE File Offset: 0x000567AE
	// (set) Token: 0x06000D8A RID: 3466 RVA: 0x000585B6 File Offset: 0x000567B6
	[GlobalSettingBool(ESettingSubCategory.Audio, true)]
	public bool SeOn
	{
		get
		{
			return this._seOn;
		}
		set
		{
			this._seOn = value;
			AudioManager.Instance.SetSoundMute(!this._seOn);
		}
	}

	// Token: 0x17000145 RID: 325
	// (get) Token: 0x06000D8B RID: 3467 RVA: 0x000585D4 File Offset: 0x000567D4
	// (set) Token: 0x06000D8C RID: 3468 RVA: 0x000585DC File Offset: 0x000567DC
	[GlobalSettingSbyte(ESettingSubCategory.Audio, 80)]
	public sbyte SeVolume
	{
		get
		{
			return this._seVolume;
		}
		set
		{
			this._seVolume = value;
			AudioManager.Instance.SetSoundVolume((float)this._seVolume / 100f);
		}
	}

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x06000D8D RID: 3469 RVA: 0x000585FE File Offset: 0x000567FE
	// (set) Token: 0x06000D8E RID: 3470 RVA: 0x00058606 File Offset: 0x00056806
	[GlobalSettingSbyte(ESettingSubCategory.Audio, 50)]
	public sbyte VideoVolume { get; set; }

	// Token: 0x17000147 RID: 327
	// (get) Token: 0x06000D8F RID: 3471 RVA: 0x0005860F File Offset: 0x0005680F
	// (set) Token: 0x06000D90 RID: 3472 RVA: 0x00058617 File Offset: 0x00056817
	[GlobalSettingBool(ESettingSubCategory.Audio, true)]
	public bool MuteIfNotFocus { get; set; }

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x06000D91 RID: 3473 RVA: 0x00058620 File Offset: 0x00056820
	// (set) Token: 0x06000D92 RID: 3474 RVA: 0x00058628 File Offset: 0x00056828
	[GlobalSettingBool(ESettingSubCategory.Other, false)]
	public bool SkipTutorialChapters
	{
		get
		{
			return this._skipTutorialChapters;
		}
		set
		{
			this._skipTutorialChapters = value;
		}
	}

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06000D93 RID: 3475 RVA: 0x00058631 File Offset: 0x00056831
	// (set) Token: 0x06000D94 RID: 3476 RVA: 0x00058639 File Offset: 0x00056839
	[GlobalSettingInt(ESettingSubCategory.Other, 0)]
	public int CompletedChapters
	{
		get
		{
			return this._completedChapters;
		}
		set
		{
			this._completedChapters = value;
		}
	}

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x06000D95 RID: 3477 RVA: 0x00058642 File Offset: 0x00056842
	// (set) Token: 0x06000D96 RID: 3478 RVA: 0x0005864A File Offset: 0x0005684A
	[GlobalSettingInt(ESettingSubCategory.Other, -1)]
	public int Monitor
	{
		get
		{
			return this._monitor;
		}
		set
		{
			this._monitor = value;
		}
	}

	// Token: 0x1700014B RID: 331
	// (get) Token: 0x06000D97 RID: 3479 RVA: 0x00058653 File Offset: 0x00056853
	// (set) Token: 0x06000D98 RID: 3480 RVA: 0x0005865B File Offset: 0x0005685B
	[GlobalSettingSbyte(ESettingSubCategory.Other, 0)]
	public sbyte LastEnterWorldIndex
	{
		get
		{
			return this._lastEnterWorldIndex;
		}
		set
		{
			this._lastEnterWorldIndex = value;
		}
	}

	// Token: 0x1700014C RID: 332
	// (get) Token: 0x06000D99 RID: 3481 RVA: 0x00058664 File Offset: 0x00056864
	// (set) Token: 0x06000D9A RID: 3482 RVA: 0x0005866C File Offset: 0x0005686C
	[GlobalSettingBool(ESettingSubCategory.Other, false)]
	public bool QuickStartGame { get; private set; }

	// Token: 0x1700014D RID: 333
	// (get) Token: 0x06000D9B RID: 3483 RVA: 0x00058675 File Offset: 0x00056875
	// (set) Token: 0x06000D9C RID: 3484 RVA: 0x0005867D File Offset: 0x0005687D
	[GlobalSettingBool(ESettingSubCategory.Other, false)]
	public bool HasActivatedQuickBeginning { get; private set; }

	// Token: 0x1700014E RID: 334
	// (get) Token: 0x06000D9D RID: 3485 RVA: 0x00058686 File Offset: 0x00056886
	// (set) Token: 0x06000D9E RID: 3486 RVA: 0x0005868E File Offset: 0x0005688E
	[GlobalSettingBool(ESettingSubCategory.Other, false)]
	public bool HasCompletedNewGameGuide { get; set; }

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x06000D9F RID: 3487 RVA: 0x00058697 File Offset: 0x00056897
	// (set) Token: 0x06000DA0 RID: 3488 RVA: 0x000586A0 File Offset: 0x000568A0
	[GlobalSettingBool(ESettingSubCategory.Saving, false)]
	public bool ImproveSaveSpeed
	{
		get
		{
			return this._improveSaveSpeed;
		}
		set
		{
			this._improveSaveSpeed = value;
			byte compressionType = this._improveSaveSpeed ? 1 : 2;
			bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
			if (flag)
			{
				GlobalDomainMethod.Call.SetCompressionType(compressionType);
			}
		}
	}

	// Token: 0x17000150 RID: 336
	// (get) Token: 0x06000DA1 RID: 3489 RVA: 0x000586DC File Offset: 0x000568DC
	// (set) Token: 0x06000DA2 RID: 3490 RVA: 0x00058731 File Offset: 0x00056931
	[GlobalSettingBool(ESettingSubCategory.Other, false)]
	public bool HaveDoneSave
	{
		get
		{
			bool flag = !File.Exists(GlobalSettings.SettingFilePath);
			if (flag)
			{
				PlayerPrefs.DeleteKey("HaveDoneSave");
			}
			bool flag2 = !this._haveDoneSave;
			if (flag2)
			{
				this._haveDoneSave = (PlayerPrefs.GetInt("HaveDoneSave", 0) == 1);
			}
			return this._haveDoneSave;
		}
		set
		{
			this._haveDoneSave = value;
			PlayerPrefs.SetInt("HaveDoneSave", 1);
		}
	}

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x06000DA3 RID: 3491 RVA: 0x00058748 File Offset: 0x00056948
	// (set) Token: 0x06000DA4 RID: 3492 RVA: 0x0005879D File Offset: 0x0005699D
	[GlobalSettingBool(ESettingSubCategory.Other, false)]
	public bool HaveShowFirstTime
	{
		get
		{
			bool flag = !File.Exists(GlobalSettings.SettingFilePath);
			if (flag)
			{
				PlayerPrefs.DeleteKey("HaveShowFirstTime");
			}
			bool flag2 = !this._haveShowFirstTime;
			if (flag2)
			{
				this._haveShowFirstTime = (PlayerPrefs.GetInt("HaveShowFirstTime", 0) == 1);
			}
			return this._haveShowFirstTime;
		}
		set
		{
			this._haveShowFirstTime = value;
			PlayerPrefs.SetInt("HaveShowFirstTime", 1);
		}
	}

	// Token: 0x17000152 RID: 338
	// (get) Token: 0x06000DA5 RID: 3493 RVA: 0x000587B4 File Offset: 0x000569B4
	// (set) Token: 0x06000DA6 RID: 3494 RVA: 0x00058810 File Offset: 0x00056A10
	public string Language
	{
		get
		{
			bool flag = string.IsNullOrEmpty(this._language);
			if (flag)
			{
				this._language = PlayerPrefs.GetString("Language", SteamManager.Language);
				bool flag2 = string.IsNullOrEmpty(this._language);
				if (flag2)
				{
					this._language = "CN";
				}
			}
			return this._language;
		}
		set
		{
			this._language = value;
			bool flag = !string.IsNullOrEmpty(this._language);
			if (flag)
			{
				PlayerPrefs.SetString("Language", this._language);
			}
		}
	}

	// Token: 0x17000153 RID: 339
	// (get) Token: 0x06000DA7 RID: 3495 RVA: 0x00058848 File Offset: 0x00056A48
	// (set) Token: 0x06000DA8 RID: 3496 RVA: 0x00058850 File Offset: 0x00056A50
	[GlobalSettingBool(ESettingSubCategory.Game, true)]
	public bool HideTaiwuOriginalSurname
	{
		get
		{
			return this._hideTaiwuOriginalSurname;
		}
		set
		{
			this._hideTaiwuOriginalSurname = value;
			bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
			if (flag)
			{
				GameDataBridge.AddDataModification<bool>(1, 22, ulong.MaxValue, uint.MaxValue, value);
				GEvent.OnEvent(UiEvents.OnTaiwuOriginalSurnameSettingChanged, null);
			}
		}
	}

	// Token: 0x17000154 RID: 340
	// (get) Token: 0x06000DA9 RID: 3497 RVA: 0x00058896 File Offset: 0x00056A96
	// (set) Token: 0x06000DAA RID: 3498 RVA: 0x0005889E File Offset: 0x00056A9E
	[GlobalSettingInt(ESettingSubCategory.Other, -1)]
	public int JieQingMurderSignDisplay
	{
		get
		{
			return this._jieQingMurderSignDisplay;
		}
		set
		{
			this._jieQingMurderSignDisplay = value;
		}
	}

	// Token: 0x17000155 RID: 341
	// (get) Token: 0x06000DAB RID: 3499 RVA: 0x000588A7 File Offset: 0x00056AA7
	// (set) Token: 0x06000DAC RID: 3500 RVA: 0x000588AF File Offset: 0x00056AAF
	[GlobalSettingBool(ESettingSubCategory.Function, true)]
	public bool CombatPrepare { get; set; }

	// Token: 0x17000156 RID: 342
	// (get) Token: 0x06000DAD RID: 3501 RVA: 0x000588B8 File Offset: 0x00056AB8
	// (set) Token: 0x06000DAE RID: 3502 RVA: 0x000588C0 File Offset: 0x00056AC0
	[GlobalSettingBool(ESettingSubCategory.Function, false)]
	public bool AutoPauseInCastSkill { get; set; }

	// Token: 0x17000157 RID: 343
	// (get) Token: 0x06000DAF RID: 3503 RVA: 0x000588C9 File Offset: 0x00056AC9
	// (set) Token: 0x06000DB0 RID: 3504 RVA: 0x000588D1 File Offset: 0x00056AD1
	[GlobalSettingBool(ESettingSubCategory.Function, false)]
	public bool AutoPauseInAllyCastSkill { get; set; }

	// Token: 0x17000158 RID: 344
	// (get) Token: 0x06000DB1 RID: 3505 RVA: 0x000588DA File Offset: 0x00056ADA
	// (set) Token: 0x06000DB2 RID: 3506 RVA: 0x000588E2 File Offset: 0x00056AE2
	[GlobalSettingBool(ESettingSubCategory.Function, true)]
	public bool NormalAttackReserve { get; set; }

	// Token: 0x17000159 RID: 345
	// (get) Token: 0x06000DB3 RID: 3507 RVA: 0x000588EB File Offset: 0x00056AEB
	// (set) Token: 0x06000DB4 RID: 3508 RVA: 0x000588F3 File Offset: 0x00056AF3
	[GlobalSettingBool(ESettingSubCategory.Function, false)]
	public bool AutoClearDefendInBlockAttackSkill { get; set; }

	// Token: 0x1700015A RID: 346
	// (get) Token: 0x06000DB5 RID: 3509 RVA: 0x000588FC File Offset: 0x00056AFC
	// (set) Token: 0x06000DB6 RID: 3510 RVA: 0x00058904 File Offset: 0x00056B04
	[GlobalSettingBool(ESettingSubCategory.Function, true)]
	public bool AutoAllocateNeiliToMax { get; set; }

	// Token: 0x1700015B RID: 347
	// (get) Token: 0x06000DB7 RID: 3511 RVA: 0x0005890D File Offset: 0x00056B0D
	// (set) Token: 0x06000DB8 RID: 3512 RVA: 0x00058924 File Offset: 0x00056B24
	[GlobalSettingBool(ESettingSubCategory.Function, false)]
	public bool AutoCombat
	{
		get
		{
			return !SingletonObject.getInstance<TutorialChapterModel>().InGuiding && this._autoCombat;
		}
		set
		{
			this._autoCombat = value;
		}
	}

	// Token: 0x1700015C RID: 348
	// (get) Token: 0x06000DB9 RID: 3513 RVA: 0x0005892D File Offset: 0x00056B2D
	// (set) Token: 0x06000DBA RID: 3514 RVA: 0x00058935 File Offset: 0x00056B35
	[GlobalSettingBool(ESettingSubCategory.Function, true)]
	public bool AutoSaveAutoCombat { get; set; }

	// Token: 0x1700015D RID: 349
	// (get) Token: 0x06000DBB RID: 3515 RVA: 0x0005893E File Offset: 0x00056B3E
	// (set) Token: 0x06000DBC RID: 3516 RVA: 0x00058946 File Offset: 0x00056B46
	[GlobalSettingFloat(ESettingSubCategory.Game, 1f)]
	public float CombatSpeed { get; set; }

	// Token: 0x1700015E RID: 350
	// (get) Token: 0x06000DBD RID: 3517 RVA: 0x0005894F File Offset: 0x00056B4F
	// (set) Token: 0x06000DBE RID: 3518 RVA: 0x00058957 File Offset: 0x00056B57
	[GlobalSettingBool(ESettingSubCategory.Game, true)]
	public bool AutoSaveCombatSpeed { get; set; }

	// Token: 0x1700015F RID: 351
	// (get) Token: 0x06000DBF RID: 3519 RVA: 0x00058960 File Offset: 0x00056B60
	// (set) Token: 0x06000DC0 RID: 3520 RVA: 0x00058968 File Offset: 0x00056B68
	[GlobalSettingFloat(ESettingSubCategory.Game, 1f)]
	public float DebateSpeed { get; set; }

	// Token: 0x17000160 RID: 352
	// (get) Token: 0x06000DC1 RID: 3521 RVA: 0x00058971 File Offset: 0x00056B71
	// (set) Token: 0x06000DC2 RID: 3522 RVA: 0x00058979 File Offset: 0x00056B79
	[GlobalSettingBool(ESettingSubCategory.Game, true)]
	public bool AutoSaveDebateSpeed { get; set; }

	// Token: 0x17000161 RID: 353
	// (get) Token: 0x06000DC3 RID: 3523 RVA: 0x00058982 File Offset: 0x00056B82
	// (set) Token: 0x06000DC4 RID: 3524 RVA: 0x0005898A File Offset: 0x00056B8A
	[GlobalSettingFloat(ESettingSubCategory.Game, 1f)]
	public float CricketCombatSpeed { get; set; }

	// Token: 0x17000162 RID: 354
	// (get) Token: 0x06000DC5 RID: 3525 RVA: 0x00058993 File Offset: 0x00056B93
	// (set) Token: 0x06000DC6 RID: 3526 RVA: 0x0005899B File Offset: 0x00056B9B
	[GlobalSettingBool(ESettingSubCategory.Game, true)]
	public bool AutoSaveCricketCombatSpeed { get; set; }

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06000DC7 RID: 3527 RVA: 0x000589A4 File Offset: 0x00056BA4
	// (set) Token: 0x06000DC8 RID: 3528 RVA: 0x000589AC File Offset: 0x00056BAC
	[GlobalSettingBool(ESettingSubCategory.Display, false)]
	public bool CombatPure
	{
		get
		{
			return this._combatPure;
		}
		set
		{
			this._combatPure = value;
			GEvent.OnEvent(UiEvents.CombatPureMode, null);
		}
	}

	// Token: 0x17000164 RID: 356
	// (get) Token: 0x06000DC9 RID: 3529 RVA: 0x000589C7 File Offset: 0x00056BC7
	// (set) Token: 0x06000DCA RID: 3530 RVA: 0x000589CF File Offset: 0x00056BCF
	[GlobalSettingBool(ESettingSubCategory.Display, true)]
	public bool AbbreviatedInformation { get; set; }

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x06000DCB RID: 3531 RVA: 0x000589D8 File Offset: 0x00056BD8
	// (set) Token: 0x06000DCC RID: 3532 RVA: 0x000589E0 File Offset: 0x00056BE0
	[GlobalSettingBool(ESettingSubCategory.Display, true)]
	public bool DistanceAttackRange { get; set; }

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06000DCD RID: 3533 RVA: 0x000589E9 File Offset: 0x00056BE9
	// (set) Token: 0x06000DCE RID: 3534 RVA: 0x000589F4 File Offset: 0x00056BF4
	[GlobalSettingBool(ESettingSubCategory.Display, false)]
	public bool AllowExecute
	{
		get
		{
			return this._allowExecute;
		}
		set
		{
			this._allowExecute = value;
			bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
			if (flag)
			{
				GameDataBridge.AddDataModification<bool>(1, 23, ulong.MaxValue, uint.MaxValue, value);
			}
		}
	}

	// Token: 0x17000167 RID: 359
	// (get) Token: 0x06000DCF RID: 3535 RVA: 0x00058A27 File Offset: 0x00056C27
	// (set) Token: 0x06000DD0 RID: 3536 RVA: 0x00058A2F File Offset: 0x00056C2F
	[GlobalSettingBool(ESettingSubCategory.Display, true)]
	public bool CombatShake { get; set; }

	// Token: 0x17000168 RID: 360
	// (get) Token: 0x06000DD1 RID: 3537 RVA: 0x00058A38 File Offset: 0x00056C38
	// (set) Token: 0x06000DD2 RID: 3538 RVA: 0x00058A40 File Offset: 0x00056C40
	[GlobalSettingBool(ESettingSubCategory.Display, true)]
	public bool ShowDamageNumber { get; set; }

	// Token: 0x17000169 RID: 361
	// (get) Token: 0x06000DD3 RID: 3539 RVA: 0x00058A49 File Offset: 0x00056C49
	// (set) Token: 0x06000DD4 RID: 3540 RVA: 0x00058A51 File Offset: 0x00056C51
	[GlobalSettingBool(ESettingSubCategory.Display, true)]
	public bool ShowCombatTutorial
	{
		get
		{
			return this._showCombatTutorial;
		}
		set
		{
			this._showCombatTutorial = value;
			GEvent.OnEvent(UiEvents.CombatTutorialSettingChanged, null);
		}
	}

	// Token: 0x1700016A RID: 362
	// (get) Token: 0x06000DD5 RID: 3541 RVA: 0x00058A6C File Offset: 0x00056C6C
	// (set) Token: 0x06000DD6 RID: 3542 RVA: 0x00058A74 File Offset: 0x00056C74
	[Obsolete]
	[GlobalSettingBool(ESettingSubCategory.Game, true)]
	public bool ShowMapBlockEnemyCount { get; set; }

	// Token: 0x1700016B RID: 363
	// (get) Token: 0x06000DD7 RID: 3543 RVA: 0x00058A7D File Offset: 0x00056C7D
	// (set) Token: 0x06000DD8 RID: 3544 RVA: 0x00058A85 File Offset: 0x00056C85
	[Obsolete]
	[GlobalSettingBool(ESettingSubCategory.Game, true)]
	public bool ShowMapBlockFriendCount { get; set; }

	// Token: 0x1700016C RID: 364
	// (get) Token: 0x06000DD9 RID: 3545 RVA: 0x00058A8E File Offset: 0x00056C8E
	// (set) Token: 0x06000DDA RID: 3546 RVA: 0x00058A96 File Offset: 0x00056C96
	[Obsolete]
	[GlobalSettingBool(ESettingSubCategory.Game, true)]
	public bool ShowMapBlockMerchantIcon { get; set; }

	// Token: 0x1700016D RID: 365
	// (get) Token: 0x06000DDB RID: 3547 RVA: 0x00058A9F File Offset: 0x00056C9F
	// (set) Token: 0x06000DDC RID: 3548 RVA: 0x00058AA7 File Offset: 0x00056CA7
	[Obsolete]
	[GlobalSettingBool(ESettingSubCategory.Game, true)]
	public bool ShowMapBlockSettlementEdge { get; set; }

	// Token: 0x1700016E RID: 366
	// (get) Token: 0x06000DDD RID: 3549 RVA: 0x00058AB0 File Offset: 0x00056CB0
	// (set) Token: 0x06000DDE RID: 3550 RVA: 0x00058AB8 File Offset: 0x00056CB8
	[Obsolete]
	[GlobalSettingBool(ESettingSubCategory.Game, true)]
	public bool ShowMapBlockPastLifeRelationIcon { get; set; }

	// Token: 0x1700016F RID: 367
	// (get) Token: 0x06000DDF RID: 3551 RVA: 0x00058AC1 File Offset: 0x00056CC1
	// (set) Token: 0x06000DE0 RID: 3552 RVA: 0x00058AC9 File Offset: 0x00056CC9
	[GlobalSettingBool(ESettingSubCategory.Game, true)]
	public bool AdventureCarrier { get; set; }

	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06000DE1 RID: 3553 RVA: 0x00058AD2 File Offset: 0x00056CD2
	// (set) Token: 0x06000DE2 RID: 3554 RVA: 0x00058ADA File Offset: 0x00056CDA
	[GlobalSettingBool(ESettingSubCategory.Display, true)]
	public bool MapWeather
	{
		get
		{
			return this._mapWeather;
		}
		set
		{
			this._mapWeather = value;
			GEvent.OnEvent(UiEvents.WeatherChanged, null);
		}
	}

	// Token: 0x17000171 RID: 369
	// (get) Token: 0x06000DE3 RID: 3555 RVA: 0x00058AF2 File Offset: 0x00056CF2
	// (set) Token: 0x06000DE4 RID: 3556 RVA: 0x00058AFA File Offset: 0x00056CFA
	[GlobalSettingBool(ESettingSubCategory.Display, true)]
	public bool CombatWeather { get; set; }

	// Token: 0x17000172 RID: 370
	// (get) Token: 0x06000DE5 RID: 3557 RVA: 0x00058B03 File Offset: 0x00056D03
	// (set) Token: 0x06000DE6 RID: 3558 RVA: 0x00058B0C File Offset: 0x00056D0C
	[GlobalSettingBool(ESettingSubCategory.Game, true)]
	public bool AdventureLighting
	{
		get
		{
			return this._adventureLighting;
		}
		set
		{
			bool flag = this._adventureLighting == value;
			if (!flag)
			{
				this._adventureLighting = value;
				GEvent.OnEvent(UiEvents.AdventureLightingSettingChanged, null);
			}
		}
	}

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x06000DE7 RID: 3559 RVA: 0x00058B41 File Offset: 0x00056D41
	// (set) Token: 0x06000DE8 RID: 3560 RVA: 0x00058B49 File Offset: 0x00056D49
	[GlobalSettingBool(ESettingSubCategory.Game, true)]
	public bool MiniScene { get; set; }

	// Token: 0x17000174 RID: 372
	// (get) Token: 0x06000DE9 RID: 3561 RVA: 0x00058B52 File Offset: 0x00056D52
	// (set) Token: 0x06000DEA RID: 3562 RVA: 0x00058B5A File Offset: 0x00056D5A
	[GlobalSettingBool(ESettingSubCategory.Game, true)]
	public bool Guiding { get; set; }

	// Token: 0x17000175 RID: 373
	// (get) Token: 0x06000DEB RID: 3563 RVA: 0x00058B63 File Offset: 0x00056D63
	// (set) Token: 0x06000DEC RID: 3564 RVA: 0x00058B6B File Offset: 0x00056D6B
	[GlobalSettingBool(ESettingSubCategory.Game, true)]
	public bool VillagerAnimation { get; set; }

	// Token: 0x17000176 RID: 374
	// (get) Token: 0x06000DED RID: 3565 RVA: 0x00058B74 File Offset: 0x00056D74
	// (set) Token: 0x06000DEE RID: 3566 RVA: 0x00058B7C File Offset: 0x00056D7C
	[GlobalSettingBool(ESettingSubCategory.Game, true)]
	public bool ShowDynamicAvatarIfPossible
	{
		get
		{
			return this._showDynamicAvatarIfPossible;
		}
		set
		{
			bool flag = this._showDynamicAvatarIfPossible == value;
			if (!flag)
			{
				this._showDynamicAvatarIfPossible = value;
				GEvent.OnEvent(UiEvents.RefreshAllAvatar, null);
			}
		}
	}

	// Token: 0x17000177 RID: 375
	// (get) Token: 0x06000DEF RID: 3567 RVA: 0x00058BB1 File Offset: 0x00056DB1
	// (set) Token: 0x06000DF0 RID: 3568 RVA: 0x00058BB9 File Offset: 0x00056DB9
	[GlobalSettingBool(ESettingSubCategory.Game, true)]
	public bool EnableInteractCheckResultAnimation { get; set; }

	// Token: 0x17000178 RID: 376
	// (get) Token: 0x06000DF1 RID: 3569 RVA: 0x00058BC2 File Offset: 0x00056DC2
	// (set) Token: 0x06000DF2 RID: 3570 RVA: 0x00058BCA File Offset: 0x00056DCA
	[GlobalSettingBool(ESettingSubCategory.MapExplore, false)]
	public bool EnableAutoTriggerNormalMapPickup
	{
		get
		{
			return this._enableAutoTriggerNormalMapPickup;
		}
		set
		{
			this._enableAutoTriggerNormalMapPickup = value;
			GEvent.OnEvent(UiEvents.OnEnableAutoTriggerNormalMapPickupChanged, null);
		}
	}

	// Token: 0x17000179 RID: 377
	// (get) Token: 0x06000DF3 RID: 3571 RVA: 0x00058BE5 File Offset: 0x00056DE5
	// (set) Token: 0x06000DF4 RID: 3572 RVA: 0x00058BED File Offset: 0x00056DED
	[GlobalSettingBool(ESettingSubCategory.MapExplore, true)]
	public bool AutoTriggerNormalPickupIncludeXiangshuMinion { get; set; }

	// Token: 0x1700017A RID: 378
	// (get) Token: 0x06000DF5 RID: 3573 RVA: 0x00058BF6 File Offset: 0x00056DF6
	// (set) Token: 0x06000DF6 RID: 3574 RVA: 0x00058BFE File Offset: 0x00056DFE
	[GlobalSettingBool(ESettingSubCategory.Game, false)]
	public bool AutoSelectToolOnMake { get; set; }

	// Token: 0x1700017B RID: 379
	// (get) Token: 0x06000DF7 RID: 3575 RVA: 0x00058C07 File Offset: 0x00056E07
	private static string SettingFilePath
	{
		get
		{
			return Path.Combine(GameApp.GetArchiveDirPath(), "SystemSetting.lua");
		}
	}

	// Token: 0x06000DF8 RID: 3576 RVA: 0x00058C18 File Offset: 0x00056E18
	public void SetImproveSaveSpeed(bool value)
	{
		this._improveSaveSpeed = value;
		byte compressionType = this._improveSaveSpeed ? 1 : 2;
		bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
		if (flag)
		{
			GlobalDomainMethod.Call.SetCompressionType(compressionType);
		}
	}

	// Token: 0x06000DF9 RID: 3577 RVA: 0x00058C54 File Offset: 0x00056E54
	public void SetHideTaiwuOriginalSurname(bool value)
	{
		this._hideTaiwuOriginalSurname = value;
		bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
		if (flag)
		{
			GameDataBridge.AddDataModification<bool>(1, 22, ulong.MaxValue, uint.MaxValue, value);
			GEvent.OnEvent(UiEvents.OnTaiwuOriginalSurnameSettingChanged, null);
		}
	}

	// Token: 0x06000DFA RID: 3578 RVA: 0x00058C9A File Offset: 0x00056E9A
	public void SetQuickStartGame(bool value)
	{
		this.QuickStartGame = value;
		TaiwuEventDomainMethod.Call.SetIsQuickStartGame(this.QuickStartGame);
	}

	// Token: 0x06000DFB RID: 3579 RVA: 0x00058CB1 File Offset: 0x00056EB1
	public void SetHasSelectedQuickStartGame()
	{
		this.HasActivatedQuickBeginning = true;
	}

	// Token: 0x1700017C RID: 380
	// (get) Token: 0x06000DFC RID: 3580 RVA: 0x00058CBC File Offset: 0x00056EBC
	// (set) Token: 0x06000DFD RID: 3581 RVA: 0x00058CC4 File Offset: 0x00056EC4
	[GlobalSettingBool(ESettingSubCategory.Saving, true)]
	public bool ArchiveFilesBackupOn
	{
		get
		{
			return this._archiveFilesBackupOn;
		}
		set
		{
			this._archiveFilesBackupOn = value;
		}
	}

	// Token: 0x1700017D RID: 381
	// (get) Token: 0x06000DFE RID: 3582 RVA: 0x00058CCD File Offset: 0x00056ECD
	// (set) Token: 0x06000DFF RID: 3583 RVA: 0x00058CD8 File Offset: 0x00056ED8
	[GlobalSettingSbyte(ESettingSubCategory.Saving, 1)]
	public sbyte ArchiveFilesBackupInterval
	{
		get
		{
			return this._archiveFilesBackupInterval;
		}
		set
		{
			this._archiveFilesBackupInterval = value;
			bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
			if (flag)
			{
				GameDataBridge.AddDataModification<sbyte>(1, 24, ulong.MaxValue, uint.MaxValue, this._archiveFilesBackupOn ? value : -1);
			}
		}
	}

	// Token: 0x1700017E RID: 382
	// (get) Token: 0x06000E00 RID: 3584 RVA: 0x00058D16 File Offset: 0x00056F16
	// (set) Token: 0x06000E01 RID: 3585 RVA: 0x00058D20 File Offset: 0x00056F20
	[GlobalSettingSbyte(ESettingSubCategory.Saving, 5)]
	public sbyte ArchiveFilesBackupCount
	{
		get
		{
			return this._archiveFilesBackupCount;
		}
		set
		{
			this._archiveFilesBackupCount = value;
			bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
			if (flag)
			{
				GameDataBridge.AddDataModification<sbyte>(1, 32, ulong.MaxValue, uint.MaxValue, this._archiveFilesBackupOn ? value : -1);
			}
		}
	}

	// Token: 0x1700017F RID: 383
	// (get) Token: 0x06000E02 RID: 3586 RVA: 0x00058D5E File Offset: 0x00056F5E
	// (set) Token: 0x06000E03 RID: 3587 RVA: 0x00058D66 File Offset: 0x00056F66
	[GlobalSettingBool(ESettingSubCategory.Game, false)]
	public bool ForceUnlockWorldDetailDifficultyLevel4 { get; set; }

	// Token: 0x17000180 RID: 384
	// (get) Token: 0x06000E04 RID: 3588 RVA: 0x00058D6F File Offset: 0x00056F6F
	// (set) Token: 0x06000E05 RID: 3589 RVA: 0x00058D7F File Offset: 0x00056F7F
	public bool SkipSaving
	{
		get
		{
			return PlayerPrefs.GetInt("SkipSaving", 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt("SkipSaving", value ? 1 : 0);
		}
	}

	// Token: 0x06000E06 RID: 3590 RVA: 0x00058D93 File Offset: 0x00056F93
	public void SetArchiveFilesBackupOn(bool on)
	{
		this._archiveFilesBackupOn = on;
		this.SetArchiveFilesBackupInterval(this._archiveFilesBackupInterval);
		this.SetArchiveFilesBackupCount(this._archiveFilesBackupCount);
	}

	// Token: 0x06000E07 RID: 3591 RVA: 0x00058DB8 File Offset: 0x00056FB8
	public void SetArchiveFilesBackupInterval(sbyte value)
	{
		this._archiveFilesBackupInterval = value;
		bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
		if (flag)
		{
			GameDataBridge.AddDataModification<sbyte>(1, 24, ulong.MaxValue, uint.MaxValue, this._archiveFilesBackupOn ? value : -1);
		}
	}

	// Token: 0x06000E08 RID: 3592 RVA: 0x00058DF8 File Offset: 0x00056FF8
	public void SetArchiveFilesBackupCount(sbyte value)
	{
		this._archiveFilesBackupCount = value;
		bool flag = GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame;
		if (flag)
		{
			GameDataBridge.AddDataModification<sbyte>(1, 32, ulong.MaxValue, uint.MaxValue, this._archiveFilesBackupOn ? value : -1);
		}
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x00058E38 File Offset: 0x00057038
	public bool SkipMember(MemberInfo member, bool deserializing)
	{
		return member.GetCustomAttribute<GlobalSettingAttribute>() == null;
	}

	// Token: 0x06000E0A RID: 3594 RVA: 0x00058E53 File Offset: 0x00057053
	public IEnumerable<GameData.Serializer.CommonObjectSerializationMember> ExtraMembers(bool deserializing)
	{
		yield return GameData.Serializer.CommonObjectSerializationMember.Make<int>("ScreenWidth", () => this._resolution.x, delegate(int v)
		{
			this._resolution.x = v;
		});
		yield return GameData.Serializer.CommonObjectSerializationMember.Make<int>("ScreenHeight", () => this._resolution.y, delegate(int v)
		{
			this._resolution.y = v;
		});
		yield return GameData.Serializer.CommonObjectSerializationMember.Make<Dictionary<int, bool>>("DialogDonotShow", () => this._dialogDonotShow, delegate(Dictionary<int, bool> v)
		{
			this._dialogDonotShow = v;
		});
		yield return GameData.Serializer.CommonObjectSerializationMember.Make<Dictionary<short, bool>>("MapElementDisplayRuleGroupDict", () => this._mapElementDisplayRuleGroupDict, delegate(Dictionary<short, bool> v)
		{
			this._mapElementDisplayRuleGroupDict = v;
		});
		yield return GameData.Serializer.CommonObjectSerializationMember.Make<Dictionary<short, bool>>("MapElementDisplayRuleItemDict", () => this._mapElementDisplayRuleItemDict, delegate(Dictionary<short, bool> v)
		{
			this._mapElementDisplayRuleItemDict = v;
		});
		yield break;
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x00058E6C File Offset: 0x0005706C
	public bool DeserializingUnknownField(string name, out GameData.Serializer.CommonObjectSerializationMember proc)
	{
		bool flag = name == "HasActivatedQuickStart" && this.QuickStartGame;
		if (flag)
		{
			this.SetHasSelectedQuickStartGame();
		}
		proc = default(GameData.Serializer.CommonObjectSerializationMember);
		return false;
	}

	// Token: 0x06000E0C RID: 3596 RVA: 0x00058EAC File Offset: 0x000570AC
	public void DeserializingMissingField(GameData.Serializer.CommonObjectSerializationMember member)
	{
		MemberInfo memberInfo = member.MemberInfo;
		GlobalSettingAttribute attribute = (memberInfo != null) ? memberInfo.GetCustomAttribute<GlobalSettingAttribute>() : null;
		bool flag = attribute != null;
		if (flag)
		{
			GlobalSettingAttribute globalSettingAttribute = attribute;
			GlobalSettingAttribute globalSettingAttribute2 = globalSettingAttribute;
			GlobalSettingBool attributeBool = globalSettingAttribute2 as GlobalSettingBool;
			if (attributeBool == null)
			{
				GlobalSettingInt attributeInt = globalSettingAttribute2 as GlobalSettingInt;
				if (attributeInt == null)
				{
					GlobalSettingFloat attributeFloat = globalSettingAttribute2 as GlobalSettingFloat;
					if (attributeFloat == null)
					{
						GlobalSettingSbyte attributeSbyte = globalSettingAttribute2 as GlobalSettingSbyte;
						if (attributeSbyte != null)
						{
							member.Setter(attributeSbyte.ResetValue);
						}
					}
					else
					{
						member.Setter(attributeFloat.ResetValue);
					}
				}
				else
				{
					member.Setter(attributeInt.ResetValue);
				}
			}
			else
			{
				member.Setter(attributeBool.ResetValue);
			}
		}
		else
		{
			string name = member.Name;
			string a = name;
			if (a == "MapElementDisplayRuleGroupDict" || a == "MapElementDisplayRuleItemDict")
			{
				member.Setter(new Dictionary<short, bool>());
			}
		}
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x00058FC4 File Offset: 0x000571C4
	public void SaveSettings()
	{
		bool flag = !this._initialized;
		if (!flag)
		{
			string archiveDir = GameApp.GetArchiveDirPath();
			bool flag2 = !Directory.Exists(archiveDir);
			if (flag2)
			{
				Directory.CreateDirectory(archiveDir);
			}
			string data;
			GameData.Serializer.CommonObjectSerializer.Serialize<GlobalSettings>(this, out data, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
			File.WriteAllText(Path.Combine(archiveDir, "SystemSetting.lua"), data, Encoding.UTF8);
			CommandKitBase.SaveHotKeyConfig();
			this.SyncToGameDataModule();
		}
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x0005902C File Offset: 0x0005722C
	public void LoadSettings()
	{
		this._initialized = true;
		bool flag = File.Exists(GlobalSettings.SettingFilePath);
		if (flag)
		{
			try
			{
				this.Reset();
				GameData.Serializer.CommonObjectSerializer.RestoreObject<GlobalSettings>(File.ReadAllText(GlobalSettings.SettingFilePath), this, GameData.Serializer.CommonObjectSerializer.MarshalFormat.LuaWithReturnPrefix);
				this.FixResolution();
				this.FixSpeed();
				return;
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				File.Delete(GlobalSettings.SettingFilePath);
				this.Reset();
				return;
			}
		}
		this.Reset();
	}

	// Token: 0x06000E0F RID: 3599 RVA: 0x000590B0 File Offset: 0x000572B0
	public void EnsureLoaded()
	{
		bool flag = !this._initialized;
		if (flag)
		{
			this.LoadSettings();
		}
	}

	// Token: 0x06000E10 RID: 3600 RVA: 0x000590D2 File Offset: 0x000572D2
	internal void SyncToGameDataModule()
	{
		GlobalDomainMethod.Call.UpdateSharedGlobalSettings(-1, this.DumpSharedGlobalSettings());
	}

	// Token: 0x06000E11 RID: 3601 RVA: 0x000590E4 File Offset: 0x000572E4
	internal SharedGlobalSettings DumpSharedGlobalSettings()
	{
		return new SharedGlobalSettings
		{
			Language = this.Language,
			AutoTriggerMapNormalPickup = this.EnableAutoTriggerNormalMapPickup,
			NormalMapPickupAutoTriggerSetting = new MapPickupAutoTriggerSetting(this.AutoTriggerNormalPickupIncludeXiangshuMinion, this.AutoTriggerNormalPickupMinGrade, this.AutoTriggerNormalPickupTypes),
			AutoWipeOut = this.AutoWipeOut,
			AvoidHereticAttackBlocks = this.AvoidHereticAttackBlocks,
			AvoidInfectedCharacterBlocks = this.AvoidInfectedCharacterBlocks,
			PreferUnlockedTravelRoute = this.PreferUnlockedTravelRoute
		};
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x0005915C File Offset: 0x0005735C
	public void Reset()
	{
		for (ESettingSubCategory i = ESettingSubCategory.Localization; i <= ESettingSubCategory.Other; i++)
		{
			this.ResetSettingByType(i);
		}
		this.ReSetResolution();
		CommandKitBase.Reset();
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x00059198 File Offset: 0x00057398
	public bool CanResetCategory(ESettingSubCategory category)
	{
		bool flag = category == ESettingSubCategory.Invalid;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			foreach (PropertyInfo property in base.GetType().GetProperties())
			{
				GlobalSettingAttribute attribute = property.GetCustomAttribute<GlobalSettingAttribute>();
				bool flag2 = attribute != null && attribute.GlobalSettingType == category;
				if (flag2)
				{
					return true;
				}
			}
			foreach (FieldInfo field in base.GetType().GetFields())
			{
				GlobalSettingAttribute attribute2 = field.GetCustomAttribute<GlobalSettingAttribute>();
				bool flag3 = attribute2 != null && attribute2.GlobalSettingType == category;
				if (flag3)
				{
					return true;
				}
			}
			result = false;
		}
		return result;
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x00059250 File Offset: 0x00057450
	public void ResetSettingByType(ESettingSubCategory globalSettingType)
	{
		foreach (PropertyInfo property in base.GetType().GetProperties())
		{
			GlobalSettingAttribute attribute = property.GetCustomAttribute<GlobalSettingAttribute>();
			bool flag = attribute != null && (globalSettingType == ESettingSubCategory.Invalid || attribute.GlobalSettingType == globalSettingType);
			if (flag)
			{
				GlobalSettingAttribute globalSettingAttribute = attribute;
				GlobalSettingAttribute globalSettingAttribute2 = globalSettingAttribute;
				GlobalSettingBool globalSettingBool = globalSettingAttribute2 as GlobalSettingBool;
				if (globalSettingBool == null)
				{
					GlobalSettingSbyte globalSettingSbyte = globalSettingAttribute2 as GlobalSettingSbyte;
					if (globalSettingSbyte == null)
					{
						GlobalSettingInt globalSettingInt = globalSettingAttribute2 as GlobalSettingInt;
						if (globalSettingInt == null)
						{
							GlobalSettingFloat globalSettingFloat = globalSettingAttribute2 as GlobalSettingFloat;
							if (globalSettingFloat == null)
							{
								GlobalSettingString globalSettingString = globalSettingAttribute2 as GlobalSettingString;
								if (globalSettingString != null)
								{
									property.SetValue(this, globalSettingString.ResetValue);
								}
							}
							else
							{
								property.SetValue(this, globalSettingFloat.ResetValue);
							}
						}
						else
						{
							property.SetValue(this, globalSettingInt.ResetValue);
						}
					}
					else
					{
						property.SetValue(this, globalSettingSbyte.ResetValue);
					}
				}
				else
				{
					property.SetValue(this, globalSettingBool.ResetValue);
				}
			}
		}
		foreach (FieldInfo field in base.GetType().GetFields())
		{
			GlobalSettingAttribute attribute2 = field.GetCustomAttribute<GlobalSettingAttribute>();
			bool flag2 = attribute2 != null && (globalSettingType == ESettingSubCategory.Invalid || attribute2.GlobalSettingType == globalSettingType);
			if (flag2)
			{
				GlobalSettingAttribute globalSettingAttribute3 = attribute2;
				GlobalSettingAttribute globalSettingAttribute4 = globalSettingAttribute3;
				GlobalSettingBool globalSettingBool2 = globalSettingAttribute4 as GlobalSettingBool;
				if (globalSettingBool2 == null)
				{
					GlobalSettingSbyte globalSettingSbyte2 = globalSettingAttribute4 as GlobalSettingSbyte;
					if (globalSettingSbyte2 == null)
					{
						GlobalSettingInt globalSettingInt2 = globalSettingAttribute4 as GlobalSettingInt;
						if (globalSettingInt2 == null)
						{
							GlobalSettingFloat globalSettingFloat2 = globalSettingAttribute4 as GlobalSettingFloat;
							if (globalSettingFloat2 != null)
							{
								field.SetValue(this, globalSettingFloat2.ResetValue);
							}
						}
						else
						{
							field.SetValue(this, globalSettingInt2.ResetValue);
						}
					}
					else
					{
						field.SetValue(this, globalSettingSbyte2.ResetValue);
					}
				}
				else
				{
					field.SetValue(this, globalSettingBool2.ResetValue);
				}
			}
		}
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x00059464 File Offset: 0x00057664
	private void ReSetResolution()
	{
		Resolution[] systemResolutions = Screen.resolutions;
		Resolution resolution2;
		if (systemResolutions == null || systemResolutions.Length == 0)
		{
			resolution2 = new Resolution
			{
				width = 2560,
				height = 1440
			};
		}
		else
		{
			Resolution[] array = systemResolutions;
			resolution2 = array[array.Length - 1];
		}
		Resolution resolution = resolution2;
		this.Resolution = AspectRatioController.Instance.FormatResolution(resolution.width, resolution.height);
	}

	// Token: 0x06000E16 RID: 3606 RVA: 0x000594D0 File Offset: 0x000576D0
	private void FixResolution()
	{
		int width = this._resolution.x;
		int height = this._resolution.y;
		bool hasZero = false;
		bool flag = height == 0 || width == 0;
		if (flag)
		{
			hasZero = true;
			Debug.Log(string.Format("[FixWindowSize] screen width({0}) or height({1}) is 0, reset to default value.", width, height));
			Resolution[] resolutions = Screen.resolutions;
			bool flag2 = resolutions.Length != 0;
			if (flag2)
			{
				height = resolutions[0].height;
				width = resolutions[0].width;
			}
			else
			{
				height = 720;
				width = 1280;
			}
			Debug.Log(string.Format("[FixWindowSize] set to {0}x{1}", width, height));
		}
		bool flag3 = hasZero;
		if (flag3)
		{
			this.Resolution = AspectRatioController.Instance.FormatResolutionWitoutScreen(width, height);
		}
		else
		{
			this.Resolution = AspectRatioController.Instance.FormatResolution(width, height);
		}
	}

	// Token: 0x06000E17 RID: 3607 RVA: 0x000595B2 File Offset: 0x000577B2
	private void FixSpeed()
	{
		this.CombatSpeed = CombatTimeScaleToggle.ClampTimeScale(this.CombatSpeed);
		this.CricketCombatSpeed = CombatTimeScaleToggle.ClampTimeScale(this.CricketCombatSpeed);
		this.DebateSpeed = CombatTimeScaleToggle.ClampTimeScale(this.DebateSpeed);
	}

	// Token: 0x06000E18 RID: 3608 RVA: 0x000595EC File Offset: 0x000577EC
	public void AddDialogDoNotShow(EDialogType type)
	{
		this._dialogDonotShow[(int)type] = true;
	}

	// Token: 0x06000E19 RID: 3609 RVA: 0x0005960C File Offset: 0x0005780C
	public void RemoveDialogDoNotShow(EDialogType type)
	{
		bool flag = this._dialogDonotShow.ContainsKey((int)type);
		if (flag)
		{
			this._dialogDonotShow[(int)type] = false;
		}
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x0005963C File Offset: 0x0005783C
	public bool CheckDialogDoNotShow(EDialogType type)
	{
		bool flag = this._dialogDonotShow.ContainsKey((int)type);
		return flag && this._dialogDonotShow[(int)type];
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x00059671 File Offset: 0x00057871
	public bool GetMapElementDisplayRuleGroupState(short groupId)
	{
		return this._mapElementDisplayRuleGroupDict.GetOrDefault(groupId, true);
	}

	// Token: 0x06000E1C RID: 3612 RVA: 0x00059680 File Offset: 0x00057880
	public void SetMapElementDisplayRuleGroupState(short groupId, bool state)
	{
		this._mapElementDisplayRuleGroupDict[groupId] = state;
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x00059690 File Offset: 0x00057890
	public bool GetMapElementDisplayRuleItemState(short itemId, bool considerGroup)
	{
		bool itemState = this._mapElementDisplayRuleItemDict.GetOrDefault(itemId, true);
		bool flag = !considerGroup;
		bool result;
		if (flag)
		{
			result = itemState;
		}
		else
		{
			MapElementDisplayRuleItemItem config = MapElementDisplayRuleItem.Instance[itemId];
			bool groupState = this.GetMapElementDisplayRuleGroupState(config.Group);
			result = (itemState && groupState);
		}
		return result;
	}

	// Token: 0x14000010 RID: 16
	// (add) Token: 0x06000E1E RID: 3614 RVA: 0x000596DC File Offset: 0x000578DC
	// (remove) Token: 0x06000E1F RID: 3615 RVA: 0x00059710 File Offset: 0x00057910
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static event Action<short, bool> OnMapElementDisplayRuleItemStateChanged;

	// Token: 0x06000E20 RID: 3616 RVA: 0x00059743 File Offset: 0x00057943
	public void SetMapElementDisplayRuleItemState(short itemId, bool state)
	{
		this._mapElementDisplayRuleItemDict[itemId] = state;
		Action<short, bool> onMapElementDisplayRuleItemStateChanged = GlobalSettings.OnMapElementDisplayRuleItemStateChanged;
		if (onMapElementDisplayRuleItemStateChanged != null)
		{
			onMapElementDisplayRuleItemStateChanged(itemId, state);
		}
	}

	// Token: 0x04000DF8 RID: 3576
	private bool _initialized = false;

	// Token: 0x04000DF9 RID: 3577
	private const string SaveFileName = "SystemSetting.lua";

	// Token: 0x04000DFA RID: 3578
	private int _monitor;

	// Token: 0x04000DFB RID: 3579
	private Vector2Int _resolution;

	// Token: 0x04000DFC RID: 3580
	private Vector2Int _maxResolution;

	// Token: 0x04000DFD RID: 3581
	private bool _fullScreen;

	// Token: 0x04000DFE RID: 3582
	private bool _vSync;

	// Token: 0x04000DFF RID: 3583
	private bool _bgmOn;

	// Token: 0x04000E00 RID: 3584
	private sbyte _bgmVolume;

	// Token: 0x04000E01 RID: 3585
	private bool _seOn;

	// Token: 0x04000E02 RID: 3586
	private sbyte _seVolume;

	// Token: 0x04000E03 RID: 3587
	private bool _hideTaiwuOriginalSurname;

	// Token: 0x04000E04 RID: 3588
	private bool _autoCombat;

	// Token: 0x04000E05 RID: 3589
	private bool _allowExecute;

	// Token: 0x04000E06 RID: 3590
	private bool _showCombatTutorial;

	// Token: 0x04000E07 RID: 3591
	private bool _combatPure;

	// Token: 0x04000E08 RID: 3592
	private bool _skipTutorialChapters;

	// Token: 0x04000E09 RID: 3593
	private bool _archiveFilesBackupOn;

	// Token: 0x04000E0A RID: 3594
	private sbyte _archiveFilesBackupInterval;

	// Token: 0x04000E0B RID: 3595
	private sbyte _archiveFilesBackupCount;

	// Token: 0x04000E0C RID: 3596
	private sbyte _lastEnterWorldIndex;

	// Token: 0x04000E0D RID: 3597
	private int _completedChapters;

	// Token: 0x04000E0E RID: 3598
	private bool _improveSaveSpeed;

	// Token: 0x04000E0F RID: 3599
	private bool _haveDoneSave;

	// Token: 0x04000E10 RID: 3600
	private bool _haveShowFirstTime;

	// Token: 0x04000E11 RID: 3601
	private const sbyte DefaultSoundVolume = 80;

	// Token: 0x04000E12 RID: 3602
	private const sbyte DefaultMusicVolume = 60;

	// Token: 0x04000E13 RID: 3603
	private const sbyte DefaultVideoVolume = 50;

	// Token: 0x04000E14 RID: 3604
	private int _jieQingMurderSignDisplay;

	// Token: 0x04000E15 RID: 3605
	private Dictionary<int, bool> _dialogDonotShow = new Dictionary<int, bool>();

	// Token: 0x04000E26 RID: 3622
	private const string HaveDoneSaveKey = "HaveDoneSave";

	// Token: 0x04000E27 RID: 3623
	private const string HaveShowFirstTimeKey = "HaveShowFirstTime";

	// Token: 0x04000E28 RID: 3624
	private string _language;

	// Token: 0x04000E29 RID: 3625
	private const string LanguageSettingKey = "Language";

	// Token: 0x04000E2A RID: 3626
	public bool HaveShowLogo = false;

	// Token: 0x04000E42 RID: 3650
	private bool _mapWeather = true;

	// Token: 0x04000E47 RID: 3655
	private bool _showDynamicAvatarIfPossible;

	// Token: 0x04000E48 RID: 3656
	private bool _adventureLighting = true;

	// Token: 0x04000E4A RID: 3658
	private bool _enableAutoTriggerNormalMapPickup;

	// Token: 0x04000E4C RID: 3660
	[GlobalSettingSbyte(ESettingSubCategory.MapExplore, -1)]
	public sbyte AutoTriggerNormalPickupMinGrade;

	// Token: 0x04000E4D RID: 3661
	[GlobalSettingInt(ESettingSubCategory.MapExplore, -1)]
	public int AutoTriggerNormalPickupTypes;

	// Token: 0x04000E4E RID: 3662
	[GlobalSettingInt(ESettingSubCategory.MapExplore, 0)]
	public int AutoWipeOut;

	// Token: 0x04000E4F RID: 3663
	[GlobalSettingBool(ESettingSubCategory.Other, true)]
	public bool ShowTreasure;

	// Token: 0x04000E50 RID: 3664
	[GlobalSettingBool(ESettingSubCategory.Other, true)]
	public bool ShowSavageSkillDestroyedBlock;

	// Token: 0x04000E51 RID: 3665
	[GlobalSettingBool(ESettingSubCategory.Other, true)]
	public bool ShowHunterSkillAnimal;

	// Token: 0x04000E52 RID: 3666
	[GlobalSettingBool(ESettingSubCategory.Other, true)]
	public bool ShowTaoistMonkSkillPartlyInfected;

	// Token: 0x04000E53 RID: 3667
	[GlobalSettingBool(ESettingSubCategory.Other, true)]
	public bool ShowTaoistMonkSkillCompletelyInfected;

	// Token: 0x04000E54 RID: 3668
	[GlobalSettingBool(ESettingSubCategory.Other, true)]
	public bool ShowCivilianSkillPersonalEnemy;

	// Token: 0x04000E55 RID: 3669
	[GlobalSettingBool(ESettingSubCategory.Other, true)]
	public bool ShowTravelingBuddhistMonkSkillRebelAndEgoisticPeople;

	// Token: 0x04000E56 RID: 3670
	[GlobalSettingBool(ESettingSubCategory.Other, true)]
	public bool ShowDoctorSkillHurtPeople;

	// Token: 0x04000E57 RID: 3671
	[GlobalSettingBool(ESettingSubCategory.Other, true)]
	public bool ShowDoctorSkillPoisonedPeople;

	// Token: 0x04000E58 RID: 3672
	[GlobalSettingBool(ESettingSubCategory.Other, true)]
	public bool ShowDoctorSkillDisorderPeople;

	// Token: 0x04000E59 RID: 3673
	[GlobalSettingBool(ESettingSubCategory.Other, true)]
	public bool ShowDukeSkillGiveTitle;

	// Token: 0x04000E5A RID: 3674
	private Dictionary<short, bool> _mapElementDisplayRuleGroupDict = new Dictionary<short, bool>();

	// Token: 0x04000E5B RID: 3675
	private Dictionary<short, bool> _mapElementDisplayRuleItemDict = new Dictionary<short, bool>();

	// Token: 0x04000E5E RID: 3678
	[GlobalSettingBool(ESettingSubCategory.Game, false)]
	public bool AvoidHereticAttackBlocks;

	// Token: 0x04000E5F RID: 3679
	[GlobalSettingBool(ESettingSubCategory.Game, false)]
	public bool AvoidInfectedCharacterBlocks;

	// Token: 0x04000E60 RID: 3680
	[GlobalSettingBool(ESettingSubCategory.Game, false)]
	public bool PreferUnlockedTravelRoute;
}
