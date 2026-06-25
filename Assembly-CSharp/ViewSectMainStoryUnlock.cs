using System;
using Config;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Story;
using GameData.Domains.TaiwuEvent;
using TMPro;
using UnityEngine;

// Token: 0x02000412 RID: 1042
public class ViewSectMainStoryUnlock : UIBase
{
	// Token: 0x06003E20 RID: 15904 RVA: 0x001F2B18 File Offset: 0x001F0D18
	public override void OnInit(ArgumentBox argsBox)
	{
		this.Reset();
		AudioManager.Instance.PlaySound("SFX_sectstory_title", false, false);
		sbyte orgTemplateId;
		argsBox.Get("OrgTemplateId", out orgTemplateId);
		StoryDomainMethod.Call.NotifySectStoryActivated(orgTemplateId);
		OrganizationItem config = Organization.Instance[orgTemplateId];
		this.logo.SetTexture(config.SectMainStory.UnlockStoryLogo);
		this.bg.SetTexture(config.SectMainStory.UnlockStoryBg);
		this.desc.text = config.SectMainStory.UnlockStoryDesc;
		this.openText.SetText(LocalStringManager.GetFormat(LanguageKey.LK_SectMainStoryUnlock_Tip, config.Name), true);
		this.mainArea.DOFade(1f, 0.7f).SetDelay(0.3f).OnComplete(delegate
		{
			this.closeBtn.interactable = true;
			this.hotkeyDisplay.SetActive(true);
			this._canHide = true;
		});
		this.closeBtn.ClearAndAddListener(new Action(this.QuickHide));
		AudioManager.Instance.PlaySound("ui_SectstoryOpen", false, false);
	}

	// Token: 0x06003E21 RID: 15905 RVA: 0x001F2C1E File Offset: 0x001F0E1E
	private void Reset()
	{
		this.mainArea.alpha = 0f;
		this.closeBtn.interactable = false;
		this.hotkeyDisplay.SetActive(false);
		this._canHide = false;
	}

	// Token: 0x06003E22 RID: 15906 RVA: 0x001F2C53 File Offset: 0x001F0E53
	private void OnDisable()
	{
		this.Reset();
		TaiwuEventDomainMethod.Call.TriggerListener("ShowSectMainStoryUnlock", true);
	}

	// Token: 0x06003E23 RID: 15907 RVA: 0x001F2C6C File Offset: 0x001F0E6C
	private void Update()
	{
		bool flag = this._canHide && (CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) || CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false));
		if (flag)
		{
			base.QuickHide();
		}
	}

	// Token: 0x04002CCF RID: 11471
	[SerializeField]
	private CRawImage logo;

	// Token: 0x04002CD0 RID: 11472
	[SerializeField]
	private CRawImage bg;

	// Token: 0x04002CD1 RID: 11473
	[SerializeField]
	private CImage mask;

	// Token: 0x04002CD2 RID: 11474
	[SerializeField]
	private TextMeshProUGUI desc;

	// Token: 0x04002CD3 RID: 11475
	[SerializeField]
	private TextMeshProUGUI openText;

	// Token: 0x04002CD4 RID: 11476
	[SerializeField]
	private CButton closeBtn;

	// Token: 0x04002CD5 RID: 11477
	[SerializeField]
	private CanvasGroup mainArea;

	// Token: 0x04002CD6 RID: 11478
	[SerializeField]
	private GameObject hotkeyDisplay;

	// Token: 0x04002CD7 RID: 11479
	private bool _canHide = false;
}
