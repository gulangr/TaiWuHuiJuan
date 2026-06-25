using System;
using System.Collections.Generic;
using FrameWork.ModSystem;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate.Mod;
using GameData.Domains.Mod;
using Steamworks;
using TMPro;
using UnityEngine;

// Token: 0x0200025E RID: 606
public class ModEnableDependenceDialog : MonoBehaviour
{
	// Token: 0x060027D3 RID: 10195 RVA: 0x00125798 File Offset: 0x00123998
	public void Show(List<ModId> dependenceList, Action onConfirmSubscribe, Action onConfirmEnable)
	{
		UIManager.Instance.MaskComponent(base.transform as RectTransform);
		bool needSubscribe = false;
		bool isDownloading = false;
		this._dependenceList.Clear();
		foreach (ModId modId in dependenceList)
		{
			ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
			bool flag = modInfo.ModId.Source == 1 && !modInfo.IsSubscribed;
			if (flag)
			{
				this._dependenceList.Add(modId);
				needSubscribe = true;
			}
		}
		bool flag2 = !needSubscribe;
		if (flag2)
		{
			foreach (ModId modId2 in dependenceList)
			{
				bool flag3 = modId2.Source == 0;
				if (!flag3)
				{
					bool flag4 = !SteamManager.IsItemStateActive(modId2.FileId, EItemState.k_EItemStateInstalled) || SteamManager.IsItemStateActive(modId2.FileId, EItemState.k_EItemStateDownloading);
					if (flag4)
					{
						this._dependenceList.Add(modId2);
						isDownloading = true;
					}
				}
			}
			bool flag5 = !isDownloading;
			if (flag5)
			{
				this._dependenceList.AddRange(dependenceList);
			}
		}
		this.scrollView.OnItemRender += delegate(int index, GameObject go)
		{
			ModEnableDependenceDialogTemplate refers = go.GetComponent<ModEnableDependenceDialogTemplate>();
			ModInfoWithDisplayData modInfo2 = ModManager.GetModInfo(this._dependenceList[index]);
			bool flag6 = modInfo2 == null || refers == null || refers.title == null;
			if (flag6)
			{
				Debug.LogError(string.Format("ModEnableDependenceDialog OnItemRender error, modInfo or refers is null, index: {0}", index));
			}
			refers.title.text = modInfo2.Title;
			bool isRemote = modInfo2.ModId.Source == 1;
			refers.steam.SetActive(isRemote);
			refers.external.SetActive(!isRemote);
		};
		this.scrollView.SetDataCount(this._dependenceList.Count);
		this.btnYes.ClearAndAddListener(delegate
		{
			bool needSubscribe = needSubscribe;
			if (needSubscribe)
			{
				onConfirmSubscribe();
			}
			else
			{
				bool flag6 = !isDownloading;
				if (flag6)
				{
					onConfirmEnable();
				}
			}
			this.Hide();
		});
		this.btnNo.ClearAndAddListener(new Action(this.Hide));
		LanguageKey titleKey = needSubscribe ? LanguageKey.LK_Mod_Subscribe_Dependency_Title : LanguageKey.LK_Mod_Enable_Dependency_Title;
		this.title.text = LocalStringManager.Get(titleKey);
		LanguageKey contentKey = needSubscribe ? LanguageKey.LK_Mod_Subscribe_Dependency_Content : (isDownloading ? LanguageKey.LK_Mod_Enable_Dependency_DownLoading : LanguageKey.LK_Mod_Enable_Dependency_Content);
		this.content.text = LocalStringManager.Get(contentKey);
		this.confirm.text = (isDownloading ? string.Empty : (needSubscribe ? LocalStringManager.Get(LanguageKey.LK_Mod_Subscribe_Dependency_Confirm) : LocalStringManager.Get(LanguageKey.LK_Mod_Enable_Dependency_Confirm)));
		this.btnNo.gameObject.SetActive(!isDownloading);
		base.gameObject.SetActive(true);
	}

	// Token: 0x060027D4 RID: 10196 RVA: 0x00125A4C File Offset: 0x00123C4C
	public void Hide()
	{
		UIManager.Instance.UnMaskComponent(base.transform as RectTransform);
		base.gameObject.SetActive(false);
	}

	// Token: 0x04001D15 RID: 7445
	[SerializeField]
	private TextMeshProUGUI title;

	// Token: 0x04001D16 RID: 7446
	[SerializeField]
	private TextMeshProUGUI content;

	// Token: 0x04001D17 RID: 7447
	[SerializeField]
	private TextMeshProUGUI confirm;

	// Token: 0x04001D18 RID: 7448
	[SerializeField]
	private CButton btnYes;

	// Token: 0x04001D19 RID: 7449
	[SerializeField]
	private CButton btnNo;

	// Token: 0x04001D1A RID: 7450
	[SerializeField]
	private InfinityScroll scrollView;

	// Token: 0x04001D1B RID: 7451
	private readonly List<ModId> _dependenceList = new List<ModId>();
}
