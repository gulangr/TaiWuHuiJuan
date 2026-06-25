using System;
using System.Collections.Generic;
using FrameWork.ModSystem;
using FrameWork.UISystem.Components;
using FrameWork.UISystem.UIElements;
using Game.Views.Migrate.Mod;
using GameData.Domains.Mod;
using UnityEngine;

// Token: 0x02000261 RID: 609
public class ModSubscribeDependenceDialog : MonoBehaviour
{
	// Token: 0x060027F5 RID: 10229 RVA: 0x00126824 File Offset: 0x00124A24
	public void Init()
	{
		this.scrollView.OnItemRender += delegate(int index, GameObject refers)
		{
			ModInfoWithDisplayData modInfo = ModManager.GetModInfo(this._dependenceList[index]);
			refers.GetComponent<ModSubscribeDependenceDialogTemplate>().title.text = modInfo.Title;
		};
		this.toggleGroup.Init(1);
		this.toggleGroup.OnActiveIndexChange += delegate(int newToggle, int oldToggle)
		{
			this._only = (newToggle == 0);
		};
		this.btnYes.ClearAndAddListener(delegate
		{
			this.Hide();
			this._onConfirm(this._only);
		});
		this.btnNo.ClearAndAddListener(new Action(this.Hide));
	}

	// Token: 0x060027F6 RID: 10230 RVA: 0x001268A0 File Offset: 0x00124AA0
	public void Show(IReadOnlyList<ModId> dependenceList, Action<bool> onConfirm)
	{
		this.toggleGroup.Set(1, true);
		this._dependenceList.Clear();
		this._dependenceList.AddRange(dependenceList);
		this.scrollView.SetDataCount(dependenceList.Count);
		this._onConfirm = onConfirm;
		base.gameObject.SetActive(true);
	}

	// Token: 0x060027F7 RID: 10231 RVA: 0x001268FB File Offset: 0x00124AFB
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04001D36 RID: 7478
	[SerializeField]
	private InfinityScroll scrollView;

	// Token: 0x04001D37 RID: 7479
	[SerializeField]
	private CToggleGroup toggleGroup;

	// Token: 0x04001D38 RID: 7480
	[SerializeField]
	private CButton btnYes;

	// Token: 0x04001D39 RID: 7481
	[SerializeField]
	private CButton btnNo;

	// Token: 0x04001D3A RID: 7482
	private bool _only;

	// Token: 0x04001D3B RID: 7483
	private Action<bool> _onConfirm;

	// Token: 0x04001D3C RID: 7484
	private readonly List<ModId> _dependenceList = new List<ModId>();
}
