using System;
using FrameWork;
using GameData.Domains.Global;
using UnityEngine;

// Token: 0x02000305 RID: 773
public class AutoWipeOutPanel : Refers
{
	// Token: 0x06002DB1 RID: 11697 RVA: 0x0016A17C File Offset: 0x0016837C
	public void Init()
	{
		this.Heretic.ClearAndAddListener(delegate
		{
			this.SetAutoWipeOutSetting(WipeOutType.Heretic);
		});
		this.Righteous.ClearAndAddListener(delegate
		{
			bool autoWipeOutSetting = this.GetAutoWipeOutSetting(WipeOutType.Righteous);
			if (autoWipeOutSetting)
			{
				this.SetAutoWipeOutSetting(WipeOutType.Righteous);
			}
			else
			{
				DialogCmd cmd = new DialogCmd
				{
					Title = LocalStringManager.Get(LanguageKey.LK_SystemSetting_AutoWipeOut_Warning_Title),
					Content = LocalStringManager.Get(LanguageKey.LK_SystemSetting_AutoWipeOut_Warning_Desc),
					Yes = delegate()
					{
						this.SetAutoWipeOutSetting(WipeOutType.Righteous);
					}
				};
				UIElement.Dialog.SetOnInitArgs(EasyPool.Get<ArgumentBox>().SetObject("Cmd", cmd));
				UIManager.Instance.MaskUI(UIElement.Dialog);
			}
		});
		this.Xiangshu.ClearAndAddListener(delegate
		{
			this.SetAutoWipeOutSetting(WipeOutType.Xiangshu);
		});
		this.Beast.ClearAndAddListener(delegate
		{
			this.SetAutoWipeOutSetting(WipeOutType.Beast);
		});
		this.Refresh();
	}

	// Token: 0x06002DB2 RID: 11698 RVA: 0x0016A1F4 File Offset: 0x001683F4
	public void Refresh()
	{
		this.HereticOn.SetActive(this.GetAutoWipeOutSetting(WipeOutType.Heretic));
		this.RighteousOn.SetActive(this.GetAutoWipeOutSetting(WipeOutType.Righteous));
		this.XiangshuOn.SetActive(this.GetAutoWipeOutSetting(WipeOutType.Xiangshu));
		this.BeastOn.SetActive(this.GetAutoWipeOutSetting(WipeOutType.Beast));
	}

	// Token: 0x06002DB3 RID: 11699 RVA: 0x0016A250 File Offset: 0x00168450
	private bool GetAutoWipeOutSetting(WipeOutType type)
	{
		return (SingletonObject.getInstance<GlobalSettings>().AutoWipeOut & 1 << (int)type) != 0;
	}

	// Token: 0x06002DB4 RID: 11700 RVA: 0x0016A278 File Offset: 0x00168478
	private void SetAutoWipeOutSetting(WipeOutType type)
	{
		GlobalSettings settings = SingletonObject.getInstance<GlobalSettings>();
		bool autoWipeOutSetting = this.GetAutoWipeOutSetting(type);
		if (autoWipeOutSetting)
		{
			settings.AutoWipeOut &= ~(1 << (int)type);
		}
		else
		{
			settings.AutoWipeOut |= 1 << (int)type;
		}
		this.Refresh();
		settings.SaveSettings();
	}

	// Token: 0x04002107 RID: 8455
	public CButtonObsolete Heretic;

	// Token: 0x04002108 RID: 8456
	public CButtonObsolete Righteous;

	// Token: 0x04002109 RID: 8457
	public CButtonObsolete Xiangshu;

	// Token: 0x0400210A RID: 8458
	public CButtonObsolete Beast;

	// Token: 0x0400210B RID: 8459
	public GameObject HereticOn;

	// Token: 0x0400210C RID: 8460
	public GameObject RighteousOn;

	// Token: 0x0400210D RID: 8461
	public GameObject XiangshuOn;

	// Token: 0x0400210E RID: 8462
	public GameObject BeastOn;
}
