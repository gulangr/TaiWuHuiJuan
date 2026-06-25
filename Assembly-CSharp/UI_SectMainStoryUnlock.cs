using System;
using System.Collections.Generic;
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

// Token: 0x0200039D RID: 925
public class UI_SectMainStoryUnlock : UIBase
{
	// Token: 0x060037AC RID: 14252 RVA: 0x001C07AC File Offset: 0x001BE9AC
	public override void OnInit(ArgumentBox argsBox)
	{
		this.Reset();
		AudioManager.Instance.PlaySound("SFX_sectstory_title", false, false);
		sbyte orgTemplateId;
		argsBox.Get("OrgTemplateId", out orgTemplateId);
		StoryDomainMethod.Call.NotifySectStoryActivated(orgTemplateId);
		OrganizationItem config = Organization.Instance[orgTemplateId];
		string logoPath = "RemakeResources/UIGraphics5.0/StoryUnlock/" + config.SectMainStory.UnlockStoryLogo;
		string bgPath = "RemakeResources/UIGraphics5.0/StoryUnlock/" + config.SectMainStory.UnlockStoryBg;
		ResLoader.Load<Texture2D>(logoPath, delegate(Texture2D texture2D)
		{
			this.logo.texture = texture2D;
			this.logo.enabled = true;
		}, null, false);
		ResLoader.Load<Texture2D>(bgPath, delegate(Texture2D texture2D)
		{
			this.bg.texture = texture2D;
			this.bg.enabled = true;
		}, null, false);
		this.mask.SetSprite(UI_SectMainStoryUnlock._uiMask.GetValueOrDefault(orgTemplateId, "ui9_btn_story_mask_0"), false, null);
		this.desc.text = config.SectMainStory.UnlockStoryDesc;
		this.openText.SetText(LocalStringManager.GetFormat(LanguageKey.LK_SectMainStoryUnlock_Tip, config.Name), true);
		this.mainArea.DOFade(1f, 0.7f).SetDelay(0.3f).OnComplete(delegate
		{
			this.closeBtn.interactable = true;
			this.hotkeyDisplay.SetActive(true);
			this._canHide = true;
		});
		this.closeBtn.ClearAndAddListener(new Action(this.QuickHide));
	}

	// Token: 0x060037AD RID: 14253 RVA: 0x001C08E6 File Offset: 0x001BEAE6
	private void Reset()
	{
		this.mainArea.alpha = 0f;
		this.closeBtn.interactable = false;
		this.hotkeyDisplay.SetActive(false);
		this._canHide = false;
	}

	// Token: 0x060037AE RID: 14254 RVA: 0x001C091B File Offset: 0x001BEB1B
	private void OnDisable()
	{
		this.Reset();
		TaiwuEventDomainMethod.Call.TriggerListener("ShowSectMainStoryUnlock", true);
	}

	// Token: 0x060037AF RID: 14255 RVA: 0x001C0934 File Offset: 0x001BEB34
	private void Update()
	{
		bool flag = this._canHide && (CommonCommandKit.Space.Check(this.Element, false, false, false, true, false) || CommonCommandKit.Esc.Check(this.Element, false, false, false, true, false) || CommonCommandKit.RightMouse.Check(this.Element, false, false, false, true, false));
		if (flag)
		{
			base.QuickHide();
		}
	}

	// Token: 0x0400283F RID: 10303
	[SerializeField]
	private CRawImage logo;

	// Token: 0x04002840 RID: 10304
	[SerializeField]
	private CRawImage bg;

	// Token: 0x04002841 RID: 10305
	[SerializeField]
	private CImage mask;

	// Token: 0x04002842 RID: 10306
	[SerializeField]
	private TextMeshProUGUI desc;

	// Token: 0x04002843 RID: 10307
	[SerializeField]
	private TextMeshProUGUI openText;

	// Token: 0x04002844 RID: 10308
	[SerializeField]
	private CButton closeBtn;

	// Token: 0x04002845 RID: 10309
	[SerializeField]
	private CanvasGroup mainArea;

	// Token: 0x04002846 RID: 10310
	[SerializeField]
	private GameObject hotkeyDisplay;

	// Token: 0x04002847 RID: 10311
	private bool _canHide = false;

	// Token: 0x04002848 RID: 10312
	private static Dictionary<sbyte, string> _uiMask = new Dictionary<sbyte, string>
	{
		{
			2,
			"ui9_btn_story_mask_emei"
		},
		{
			10,
			"ui9_btn_story_mask_kongsang"
		},
		{
			1,
			"ui9_btn_story_mask_shaolin"
		}
	};
}
