using System;
using System.IO;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using GameData.Utilities;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x02000373 RID: 883
public class UI_CgPlayer : UIBase
{
	// Token: 0x060033B8 RID: 13240 RVA: 0x00199228 File Offset: 0x00197428
	public override void OnInit(ArgumentBox argsBox)
	{
		argsBox.Get("CGName", out this._curCgName);
		byte mode;
		bool flag = argsBox.Get("RenderMode", out mode);
		if (flag)
		{
			base.CGet<VideoPlayer>("Player").renderMode = (VideoRenderMode)mode;
		}
		else
		{
			base.CGet<VideoPlayer>("Player").renderMode = VideoRenderMode.CameraNearPlane;
		}
		bool flag2 = !argsBox.Get("JumpGap", out this._jumpGapTime);
		if (flag2)
		{
			this._jumpGapTime = 3f;
		}
		argsBox.Get("Localized", out this._localized);
		argsBox.Get("JumpOpen", out this._jumpOpen);
		argsBox.Get<Action>("OnVideoPlayStart", out this._onVideoPlayStarted);
		argsBox.Get<Action>("OnVideoPlayError", out this._onVideoPlayError);
		Vector2 commandPanelOffset;
		argsBox.Get<Vector2>("CommandPanelOffset", out commandPanelOffset);
		base.CGet<HotkeyDisplay>("HotkeyDisplay").GetComponent<RectTransform>().anchoredPosition = commandPanelOffset;
		this._canJump = false;
		this._hideJumpTime = 0f;
		bool flag3 = this._hotkeyDisplayObject && this._hotkeyDisplayObject.gameObject.activeSelf;
		if (flag3)
		{
			this._hotkeyDisplayObject.gameObject.SetActive(false);
		}
	}

	// Token: 0x060033B9 RID: 13241 RVA: 0x0019935C File Offset: 0x0019755C
	private void Awake()
	{
		this._maskImage = base.CGet<CImage>("MaskImage");
		this._maskImage.gameObject.SetActive(false);
		this._videoTextureHolder = base.CGet<CRawImage>("VideoTextureHolder");
		this._videoPlayer = base.CGet<VideoPlayer>("Player");
		this._videoPlayer.targetCamera = UIManager.Instance.UiCamera;
		this._hotkeyDisplayObject = base.CGet<GameObject>("Info");
		this._hotkeyDisplayObject.gameObject.SetActive(false);
		this._videoPlayer.errorReceived += this.OnVideoPlayError;
		this._videoPlayer.started += this.OnVideoPlayStart;
	}

	// Token: 0x060033BA RID: 13242 RVA: 0x00199418 File Offset: 0x00197618
	private void OnEnable()
	{
		try
		{
			GLog.Log("UI_CgPlayer OnEnable");
			this._videoPlayer.url = (this._localized ? this.GetLocalizedCgFileUrl() : this.GetCgFileUrl());
			this._videoPlayer.prepareCompleted += this.OnVideoPrepared;
			this._videoTextureHolder.enabled = false;
			this._videoPlayer.Prepare();
		}
		catch (Exception e)
		{
			GLog.Log("UI_CgPlayer OnEnable video play failed");
			Debug.LogWarning(e);
		}
		finally
		{
			bool flag = this._videoPlayer.clip == null && this._videoPlayer.url == null;
			if (flag)
			{
				GLog.Log("UI_CgPlayer OnEnable video play failed and hide self");
				this.Element.Hide(false);
			}
		}
	}

	// Token: 0x060033BB RID: 13243 RVA: 0x00199504 File Offset: 0x00197704
	private void OnVideoPrepared(VideoPlayer videoPlayer)
	{
		try
		{
			AdaptableLog.TagInfo("UI_CgPlayer", "Video prepared: " + videoPlayer.url + ".");
			this._maskImage.gameObject.SetActive(false);
			this._videoPlayer.frame = 0L;
			this._videoPlayer.SetDirectAudioVolume(0, (float)SingletonObject.getInstance<GlobalSettings>().VideoVolume / 100f);
			bool flag = this._videoPlayer.renderMode == VideoRenderMode.RenderTexture;
			if (flag)
			{
				AdaptableLog.TagInfo("UI_CgPlayer", "Initializing render texture.");
				RenderTexture texture = new RenderTexture(this._videoPlayer.texture.width, this._videoPlayer.texture.height, 24);
				RenderTexture prevTexture = this._videoPlayer.targetTexture;
				this._videoPlayer.targetTexture = texture;
				this._videoTextureHolder.texture = texture;
				bool flag2 = prevTexture != null;
				if (flag2)
				{
					Object.Destroy(prevTexture);
				}
			}
			this._videoPlayer.Play();
		}
		catch (Exception e)
		{
			GLog.Log("UI_CgPlayer OnEnable video play failed");
			Debug.LogWarning(e);
		}
		finally
		{
			bool flag3 = this._videoPlayer.clip == null && this._videoPlayer.url == null;
			if (flag3)
			{
				GLog.Log("UI_CgPlayer OnEnable video play failed and hide self");
				this.Element.Hide(false);
			}
		}
	}

	// Token: 0x060033BC RID: 13244 RVA: 0x0019969C File Offset: 0x0019789C
	private void OnDisable()
	{
		this._maskImage.DOKill(false);
		this._playStarted = false;
		UIManager.Instance.SetMaskBackShow(true);
	}

	// Token: 0x060033BD RID: 13245 RVA: 0x001996C0 File Offset: 0x001978C0
	private void LateUpdate()
	{
		bool flag = !this._videoPlayer.isPlaying;
		if (flag)
		{
			bool playStarted = this._playStarted;
			if (playStarted)
			{
				this.Element.Hide(false);
			}
		}
		else
		{
			this._playStarted = true;
		}
		this.HandleJumpThings();
	}

	// Token: 0x060033BE RID: 13246 RVA: 0x0019970C File Offset: 0x0019790C
	private void HandleJumpThings()
	{
		bool flag = !this._jumpOpen;
		if (!flag)
		{
			bool flag2 = Input.GetMouseButtonDown(0) && this._canJump;
			if (flag2)
			{
				bool flag3 = Time.realtimeSinceStartup < this._hideJumpTime;
				if (flag3)
				{
					this._canJump = false;
					this._maskImage.SetAlpha(0f);
					this._maskImage.gameObject.SetActive(true);
					this._maskImage.DOFade(1f, 0.5f).OnComplete(delegate
					{
						this._videoPlayer.Pause();
						this._videoPlayer.Stop();
					});
				}
				else
				{
					bool flag4 = !this._hotkeyDisplayObject.gameObject.activeSelf;
					if (flag4)
					{
						this._hideJumpTime = Time.realtimeSinceStartup + this._jumpGapTime;
						this._hotkeyDisplayObject.gameObject.SetActive(true);
					}
				}
			}
			bool activeSelf = this._hotkeyDisplayObject.gameObject.activeSelf;
			if (activeSelf)
			{
				bool flag5 = Time.realtimeSinceStartup > this._hideJumpTime;
				if (flag5)
				{
					this._hotkeyDisplayObject.gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x060033BF RID: 13247 RVA: 0x00199828 File Offset: 0x00197A28
	private void OnVideoPlayError(VideoPlayer source, string message)
	{
		GLog.Warn("OnVideoPlayError:" + message);
		Action onVideoPlayError = this._onVideoPlayError;
		if (onVideoPlayError != null)
		{
			onVideoPlayError();
		}
		this._videoTextureHolder.enabled = false;
		this.Element.Hide(false);
	}

	// Token: 0x060033C0 RID: 13248 RVA: 0x00199868 File Offset: 0x00197A68
	private void OnVideoPlayStart(VideoPlayer source)
	{
		bool flag = source != this._videoPlayer;
		if (!flag)
		{
			Action onVideoPlayStarted = this._onVideoPlayStarted;
			if (onVideoPlayStarted != null)
			{
				onVideoPlayStarted();
			}
			bool flag2 = this._videoPlayer.renderMode == VideoRenderMode.RenderTexture;
			if (flag2)
			{
				this._videoTextureHolder.enabled = true;
			}
			UIManager.Instance.SetMaskBackShow(false);
			this._canJump = true;
		}
	}

	// Token: 0x060033C1 RID: 13249 RVA: 0x001998CC File Offset: 0x00197ACC
	private string GetCgFileUrl()
	{
		string path = Path.Combine(Application.streamingAssetsPath, "Movies", this._curCgName + ".mp4").PathFix();
		bool flag = File.Exists(path);
		string result;
		if (flag)
		{
			result = path;
		}
		else
		{
			for (int i = 0; i < 3; i++)
			{
				string localPath = Path.Combine(Application.streamingAssetsPath, "Movies", string.Format("{0}_{1}.mp4", this._curCgName, i)).PathFix();
				bool flag2 = File.Exists(localPath);
				if (flag2)
				{
					return localPath;
				}
			}
			result = path;
		}
		return result;
	}

	// Token: 0x060033C2 RID: 13250 RVA: 0x00199964 File Offset: 0x00197B64
	private string GetLocalizedCgFileUrl()
	{
		string language = SingletonObject.getInstance<GlobalSettings>().Language;
		string path = Path.Combine(Application.streamingAssetsPath, "Movies", this._curCgName + "_" + language + ".mp4").PathFix();
		bool flag = File.Exists(path);
		string result;
		if (flag)
		{
			result = path;
		}
		else
		{
			bool flag2 = language.Equals("CNH");
			if (flag2)
			{
				path = Path.Combine(Application.streamingAssetsPath, "Movies", this._curCgName + "_CN.mp4").PathFix();
				bool flag3 = File.Exists(path);
				if (flag3)
				{
					return path;
				}
			}
			else
			{
				path = Path.Combine(Application.streamingAssetsPath, "Movies", this._curCgName + "_EN.mp4").PathFix();
				bool flag4 = File.Exists(path);
				if (flag4)
				{
					return path;
				}
			}
			result = Path.Combine(Application.streamingAssetsPath, "Movies", this._curCgName + "_CN.mp4").PathFix();
		}
		return result;
	}

	// Token: 0x060033C3 RID: 13251 RVA: 0x00199A64 File Offset: 0x00197C64
	private VideoClip GetVideoClip()
	{
		string filePath = Path.Combine("Movies", this._curCgName ?? "").PathFix();
		return Resources.Load<VideoClip>(filePath);
	}

	// Token: 0x060033C4 RID: 13252 RVA: 0x00199A9C File Offset: 0x00197C9C
	private VideoClip GetVideoClip(int index)
	{
		string filePath = Path.Combine("Movies", string.Format("{0}_{1}", this._curCgName, index)).PathFix();
		return Resources.Load<VideoClip>(filePath);
	}

	// Token: 0x040025A1 RID: 9633
	private string _curCgName;

	// Token: 0x040025A2 RID: 9634
	private CRawImage _videoTextureHolder;

	// Token: 0x040025A3 RID: 9635
	private VideoPlayer _videoPlayer;

	// Token: 0x040025A4 RID: 9636
	private bool _playStarted;

	// Token: 0x040025A5 RID: 9637
	private Action _onVideoPlayStarted;

	// Token: 0x040025A6 RID: 9638
	private Action _onVideoPlayError;

	// Token: 0x040025A7 RID: 9639
	private CImage _maskImage;

	// Token: 0x040025A8 RID: 9640
	private GameObject _hotkeyDisplayObject;

	// Token: 0x040025A9 RID: 9641
	private bool _canJump;

	// Token: 0x040025AA RID: 9642
	private bool _jumpOpen;

	// Token: 0x040025AB RID: 9643
	private float _hideJumpTime;

	// Token: 0x040025AC RID: 9644
	private float _jumpGapTime;

	// Token: 0x040025AD RID: 9645
	private bool _localized;

	// Token: 0x040025AE RID: 9646
	private const float DefaultJumpGapTime = 3f;

	// Token: 0x040025AF RID: 9647
	private const float JumpAnimDuration = 0.5f;

	// Token: 0x040025B0 RID: 9648
	private const string VideoFolder = "Movies";
}
