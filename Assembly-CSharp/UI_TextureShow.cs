using System;
using System.IO;
using FrameWork;
using GameData.Domains.TaiwuEvent;
using GameData.Utilities;
using UnityEngine;

// Token: 0x020003B1 RID: 945
public class UI_TextureShow : UIBase
{
	// Token: 0x060038B3 RID: 14515 RVA: 0x001C9D55 File Offset: 0x001C7F55
	public override void OnInit(ArgumentBox argsBox)
	{
		this.LoadTexture(argsBox);
	}

	// Token: 0x060038B4 RID: 14516 RVA: 0x001C9D60 File Offset: 0x001C7F60
	private void LoadTexture(ArgumentBox argsBox)
	{
		argsBox.Get("CountDownTime", out this._countDownTime);
		argsBox.Get("TexturePath", out this._texturePath);
		argsBox.Get("TextureType", out this._textureType);
		argsBox.Get("CountDownActionName", out this._countDownActionName);
		argsBox.Get("IntParam", out this._intParam);
		argsBox.Get("ModName", out this._modName);
		AdaptableLog.TagInfo("UI_TextureShow", "Showing texture: " + this._texturePath);
		bool flag = string.IsNullOrEmpty(this._texturePath);
		if (flag)
		{
			this.QuickHide();
		}
		else
		{
			ResLoader.Load<Texture2D>(this.GetTextureLoadPath(), delegate(Texture2D tex)
			{
				base.CGet<CRawImage>("MainTexture").texture = tex;
				base.CGet<CRawImage>("MainTexture").SetNativeSize();
			}, null, false);
			bool flag2 = this._countDownTime >= 0f;
			if (flag2)
			{
				base.Invoke("CountDownAction", this._countDownTime);
			}
		}
	}

	// Token: 0x060038B5 RID: 14517 RVA: 0x001C9E4E File Offset: 0x001C804E
	private void OnEnable()
	{
		GEvent.Add(UiEvents.OnTextureShowChanged, new GEvent.Callback(this.LoadTexture));
	}

	// Token: 0x060038B6 RID: 14518 RVA: 0x001C9E6D File Offset: 0x001C806D
	private void OnDisable()
	{
		GEvent.Remove(UiEvents.OnTextureShowChanged, new GEvent.Callback(this.LoadTexture));
	}

	// Token: 0x060038B7 RID: 14519 RVA: 0x001C9E8C File Offset: 0x001C808C
	private void CountDownAction()
	{
		bool flag = string.IsNullOrEmpty(this._countDownActionName);
		if (!flag)
		{
			bool flag2 = this._countDownActionName.Equals("OpenLegacy");
			if (flag2)
			{
				UIElement.Legacy.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("Inherit", true).Set("CrossArchive", true));
				UIManager.Instance.MaskUI(UIElement.Legacy);
			}
			else
			{
				bool flag3 = this._countDownActionName.Equals("TextureShowCountDown");
				if (flag3)
				{
					TaiwuEventDomainMethod.Call.TriggerListener("TextureShowCountDown", true);
				}
			}
		}
	}

	// Token: 0x060038B8 RID: 14520 RVA: 0x001C9F1C File Offset: 0x001C811C
	private string GetTextureLoadPath()
	{
		string path = string.Empty;
		bool flag = this._textureType.Equals("CG");
		if (flag)
		{
			path = "RemakeResources/Textures/CGTextures";
		}
		return Path.Combine(path, this._texturePath);
	}

	// Token: 0x04002910 RID: 10512
	public const string CountDownTime = "CountDownTime";

	// Token: 0x04002911 RID: 10513
	public const string TexturePath = "TexturePath";

	// Token: 0x04002912 RID: 10514
	public const string TextureType = "TextureType";

	// Token: 0x04002913 RID: 10515
	public const string CountDownActionName = "CountDownActionName";

	// Token: 0x04002914 RID: 10516
	public const string ModName = "ModName";

	// Token: 0x04002915 RID: 10517
	public const string IntParam = "IntParam";

	// Token: 0x04002916 RID: 10518
	private const string EventCgTextureDirectory = "RemakeResources/Textures/CGTextures";

	// Token: 0x04002917 RID: 10519
	private float _countDownTime;

	// Token: 0x04002918 RID: 10520
	private string _texturePath;

	// Token: 0x04002919 RID: 10521
	private string _textureType;

	// Token: 0x0400291A RID: 10522
	private string _countDownActionName;

	// Token: 0x0400291B RID: 10523
	private int _intParam;

	// Token: 0x0400291C RID: 10524
	private string _modName;
}
