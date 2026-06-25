using System;
using Config;
using TMPro;
using UnityEngine;

// Token: 0x020002FC RID: 764
[RequireComponent(typeof(DisableStyleRoot))]
public class SectMainStoryToggleGroup : MonoBehaviour
{
	// Token: 0x06002CC8 RID: 11464 RVA: 0x00160D04 File Offset: 0x0015EF04
	public void SwitchOff(bool isOn)
	{
		bool flag = GameApp.Instance.GetCurrentGameStateName() != EGameState.InGame;
		if (flag)
		{
			this.RefreshToggle();
		}
		else
		{
			bool flag2 = this.on.isOn == isOn;
			if (flag2)
			{
				this.on.isOn = !isOn;
			}
			SectMainSettings.SetSectMainStoryIsActive(this.orgTemplateId, isOn);
			this.RefreshToggle();
		}
	}

	// Token: 0x06002CC9 RID: 11465 RVA: 0x00160D68 File Offset: 0x0015EF68
	public void SwitchOn(bool isOn)
	{
		bool flag = GameApp.Instance.GetCurrentGameStateName() != EGameState.InGame;
		if (flag)
		{
			this.RefreshToggle();
		}
		else
		{
			bool flag2 = this.off.isOn == isOn;
			if (flag2)
			{
				this.off.isOn = !isOn;
			}
		}
	}

	// Token: 0x06002CCA RID: 11466 RVA: 0x00160DB6 File Offset: 0x0015EFB6
	public void OnEnable()
	{
		this.RefreshName();
		this.RefreshToggle();
	}

	// Token: 0x06002CCB RID: 11467 RVA: 0x00160DC8 File Offset: 0x0015EFC8
	public void RefreshName()
	{
		this.text.text = LanguageKey.LK_SystemSetting_BaseSettings_SectMain_Name_And_Story.TrFormat(Organization.Instance[this.orgTemplateId].Name, Organization.Instance[this.orgTemplateId].SectMainStory.Name);
	}

	// Token: 0x06002CCC RID: 11468 RVA: 0x00160E1C File Offset: 0x0015F01C
	private void SetBtnInteractable(bool interactable)
	{
		if (interactable)
		{
			this.off.interactable = this.on.isOn;
			this.on.interactable = this.off.isOn;
			this.onTipDisplayer.Type = (this.offTipDisplayer.Type = TipType.Simple);
			this.onTipDisplayer.PresetParam = SectMainStoryToggleGroup.OnTipDisplayerPresetParam;
			this.offTipDisplayer.PresetParam = SectMainStoryToggleGroup.OffTipDisplayerPresetParam;
		}
		else
		{
			this.off.interactable = (this.on.interactable = false);
			this.onTipDisplayer.Type = (this.offTipDisplayer.Type = TipType.SingleDesc);
			this.onTipDisplayer.PresetParam = (this.offTipDisplayer.PresetParam = this.tipDisplayer.PresetParam);
		}
		this.root.SetStyleEffect(!interactable, false);
	}

	// Token: 0x06002CCD RID: 11469 RVA: 0x00160F08 File Offset: 0x0015F108
	public void RefreshToggle()
	{
		SectMainSettings.GetSectMainStoryIsActive(this.orgTemplateId, delegate(int status)
		{
			bool flag = (status == -1 || status == 0) && GameApp.Instance.GetCurrentGameStateName() == EGameState.InGame && (UIElement.EventWindow.Exist || UIElement.Combat.Exist);
			if (flag)
			{
				this.tipDisplayer.PresetParam = SectMainStoryToggleGroup.TipDisplayerProcessing;
				this.SetBtnInteractable(false);
			}
			else if (status != -2147483648)
			{
				switch (status)
				{
				case -1:
				{
					this.tipDisplayer.PresetParam = Array.Empty<string>();
					bool isOn = this.on.isOn;
					if (isOn)
					{
						this.on.isOn = false;
					}
					bool flag2 = !this.off.isOn;
					if (flag2)
					{
						this.off.isOn = true;
					}
					this.SetBtnInteractable(true);
					break;
				}
				case 0:
				{
					this.tipDisplayer.PresetParam = Array.Empty<string>();
					bool flag3 = !this.on.isOn;
					if (flag3)
					{
						this.on.isOn = true;
					}
					bool isOn2 = this.off.isOn;
					if (isOn2)
					{
						this.off.isOn = false;
					}
					this.SetBtnInteractable(true);
					break;
				}
				case 1:
					this.tipDisplayer.PresetParam = SectMainStoryToggleGroup.TipDisplayerPresetParam;
					this.SetBtnInteractable(false);
					break;
				case 2:
					this.tipDisplayer.PresetParam = SectMainStoryToggleGroup.TipDisplayerWaitingParam;
					this.SetBtnInteractable(false);
					break;
				default:
					Debug.LogError(string.Format("Not supported status: {0}", status));
					this.SetBtnInteractable(false);
					break;
				}
			}
			else
			{
				this.tipDisplayer.PresetParam = SectMainStoryToggleGroup.TipDisplayerNeedGameStartParam;
				this.SetBtnInteractable(false);
			}
		}, null);
	}

	// Token: 0x04002071 RID: 8305
	public sbyte orgTemplateId;

	// Token: 0x04002072 RID: 8306
	[SerializeField]
	private DisableStyleRoot root;

	// Token: 0x04002073 RID: 8307
	public TMP_Text text;

	// Token: 0x04002074 RID: 8308
	public TooltipInvoker tipDisplayer;

	// Token: 0x04002075 RID: 8309
	public CToggleObsolete on;

	// Token: 0x04002076 RID: 8310
	public CToggleObsolete off;

	// Token: 0x04002077 RID: 8311
	[SerializeField]
	private TooltipInvoker onTipDisplayer;

	// Token: 0x04002078 RID: 8312
	[SerializeField]
	private TooltipInvoker offTipDisplayer;

	// Token: 0x04002079 RID: 8313
	public static readonly string[] TipDisplayerPresetParam = new string[]
	{
		"LK_SystemSetting_BaseSettings_Started"
	};

	// Token: 0x0400207A RID: 8314
	public static readonly string[] TipDisplayerNeedGameStartParam = new string[]
	{
		"LK_SystemSetting_BaseSettings_NeedGameStart"
	};

	// Token: 0x0400207B RID: 8315
	public static readonly string[] TipDisplayerWaitingParam = new string[]
	{
		"LK_SystemSetting_BaseSettings_Waiting"
	};

	// Token: 0x0400207C RID: 8316
	public static readonly string[] TipDisplayerProcessing = new string[]
	{
		"LK_SystemSetting_BaseSettings_Processing"
	};

	// Token: 0x0400207D RID: 8317
	public static readonly string[] OnTipDisplayerPresetParam = new string[]
	{
		"LK_SystemSetting_BaseSettings_BtnTips_Start_Title",
		"LK_SystemSetting_BaseSettings_BtnTips_Start_Desc"
	};

	// Token: 0x0400207E RID: 8318
	public static readonly string[] OffTipDisplayerPresetParam = new string[]
	{
		"LK_SystemSetting_BaseSettings_BtnTips_Pause_Title",
		"LK_SystemSetting_BaseSettings_BtnTips_Pause_Desc"
	};
}
