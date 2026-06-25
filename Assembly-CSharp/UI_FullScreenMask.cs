using System;
using FrameWork;
using TMPro;
using UnityEngine;

// Token: 0x02000381 RID: 897
public class UI_FullScreenMask : UIBase
{
	// Token: 0x060034DB RID: 13531 RVA: 0x001A62D0 File Offset: 0x001A44D0
	public override void OnInit(ArgumentBox argsBox)
	{
		bool showBlackMash;
		bool flag = argsBox != null && argsBox.Get("ShowBlackMask", out showBlackMash) && showBlackMash;
		if (flag)
		{
			base.CGet<GameObject>("BlackMask").SetActive(true);
		}
		else
		{
			base.CGet<GameObject>("BlackMask").SetActive(false);
		}
		bool showWaitAnimation;
		bool flag2 = argsBox != null && argsBox.Get("ShowWaitAnimation", out showWaitAnimation) && showWaitAnimation;
		if (flag2)
		{
			base.CGet<GameObject>("WaitAnimation").SetActive(true);
		}
		else
		{
			base.CGet<GameObject>("WaitAnimation").SetActive(false);
		}
		string message;
		bool flag3 = argsBox != null && argsBox.Get("Message", out message);
		if (flag3)
		{
			base.CGet<TextMeshProUGUI>("Message").text = message;
		}
		else
		{
			base.CGet<TextMeshProUGUI>("Message").text = string.Empty;
		}
	}

	// Token: 0x060034DC RID: 13532 RVA: 0x001A63A1 File Offset: 0x001A45A1
	private void OnEnable()
	{
		CommandKitBase.SetDisable(true);
	}

	// Token: 0x060034DD RID: 13533 RVA: 0x001A63AC File Offset: 0x001A45AC
	private void OnDisable()
	{
		bool flag = !GameApp.AdvancingMonth;
		if (flag)
		{
			CommandKitBase.SetDisable(false);
		}
	}

	// Token: 0x060034DE RID: 13534 RVA: 0x001A63CD File Offset: 0x001A45CD
	public override void PlayAudioOut()
	{
	}

	// Token: 0x060034DF RID: 13535 RVA: 0x001A63D0 File Offset: 0x001A45D0
	public void UpdateMessage(string message)
	{
		base.CGet<TextMeshProUGUI>("Message").text = message;
	}
}
