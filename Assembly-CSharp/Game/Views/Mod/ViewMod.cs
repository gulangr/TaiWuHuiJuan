using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameData.Domains.Mod;
using Steamworks;
using UnityEngine;

namespace Game.Views.Mod
{
	// Token: 0x020008D1 RID: 2257
	public class ViewMod : UIBase
	{
		// Token: 0x06006BEB RID: 27627 RVA: 0x0031C49C File Offset: 0x0031A69C
		public override void OnInit(ArgumentBox argsBox)
		{
			SteamManager.InitString();
			foreach (ModSubPage subPage in this.subPages)
			{
				subPage.Init(this);
			}
			this.subPageToggleGroup.Init(-1);
			ToggleGroupHotkeyController.Set(this.Element, this.subPageToggleGroup, 0, null);
			this.subPageToggleGroup.OnActiveIndexChange -= this.SubPageToggleGroupOnActiveIndexChange;
			this.subPageToggleGroup.OnActiveIndexChange += this.SubPageToggleGroupOnActiveIndexChange;
			this.subPageToggleGroup.Get(ViewMod.ToggleKey.WorkshopMod.ToInt()).gameObject.SetActive(ModManager.IsPlatformAvailable);
			this.subPageToggleGroup.Get(ViewMod.ToggleKey.UploadMod.ToInt()).gameObject.SetActive(ModManager.IsPlatformAvailable);
			this.buttonClose.ClearAndAddListener(new Action(this.QuickHide));
			this.Refresh();
		}

		// Token: 0x06006BEC RID: 27628 RVA: 0x0031C594 File Offset: 0x0031A794
		private void OnEnable()
		{
			GEvent.Add(UiEvents.OnModViewShowMask, new GEvent.Callback(this.OnModViewShowMask));
			GEvent.Add(UiEvents.OnModViewHideMask, new GEvent.Callback(this.OnModViewHideMask));
			GEvent.Add(UiEvents.OnModViewRefresh, new GEvent.Callback(this.OnModViewRefresh));
			GEvent.Add(UiEvents.OnModViewDownload, new GEvent.Callback(this.OnModViewDownload));
			GEvent.Add(UiEvents.OnModViewUnSubscribe, new GEvent.Callback(this.OnModViewUnSubscribe));
		}

		// Token: 0x06006BED RID: 27629 RVA: 0x0031C630 File Offset: 0x0031A830
		private void OnDisable()
		{
			GEvent.Remove(UiEvents.OnModViewShowMask, new GEvent.Callback(this.OnModViewShowMask));
			GEvent.Remove(UiEvents.OnModViewHideMask, new GEvent.Callback(this.OnModViewHideMask));
			GEvent.Remove(UiEvents.OnModViewRefresh, new GEvent.Callback(this.OnModViewRefresh));
			GEvent.Remove(UiEvents.OnModViewDownload, new GEvent.Callback(this.OnModViewDownload));
			GEvent.Remove(UiEvents.OnModViewUnSubscribe, new GEvent.Callback(this.OnModViewUnSubscribe));
		}

		// Token: 0x06006BEE RID: 27630 RVA: 0x0031C6CA File Offset: 0x0031A8CA
		private void OnModViewShowMask(ArgumentBox _)
		{
			this.ShowMask();
		}

		// Token: 0x06006BEF RID: 27631 RVA: 0x0031C6D3 File Offset: 0x0031A8D3
		private void OnModViewHideMask(ArgumentBox _)
		{
			this.HideMask();
		}

		// Token: 0x06006BF0 RID: 27632 RVA: 0x0031C6DC File Offset: 0x0031A8DC
		private void OnModViewRefresh(ArgumentBox _)
		{
			this.Refresh();
		}

		// Token: 0x06006BF1 RID: 27633 RVA: 0x0031C6E8 File Offset: 0x0031A8E8
		private void OnModViewDownload(ArgumentBox args)
		{
			ModId modId;
			args.Get<ModId>("ModId", out modId);
			this.DownloadMod(modId);
		}

		// Token: 0x06006BF2 RID: 27634 RVA: 0x0031C70C File Offset: 0x0031A90C
		private void OnModViewUnSubscribe(ArgumentBox args)
		{
			ModId modId;
			args.Get<ModId>("ModId", out modId);
			this.UnSubscribeItem(modId);
		}

		// Token: 0x06006BF3 RID: 27635 RVA: 0x0031C730 File Offset: 0x0031A930
		public override void QuickHide()
		{
			foreach (ModSubPage subPage in this.subPages)
			{
				bool flag = subPage.QuickHide();
				if (flag)
				{
					return;
				}
			}
			base.QuickHide();
		}

		// Token: 0x06006BF4 RID: 27636 RVA: 0x0031C770 File Offset: 0x0031A970
		private void SubPageToggleGroupOnActiveIndexChange(int newIndex, int oldIndex)
		{
			ModSubPage subPage = this.subPages[oldIndex];
			subPage.TryChangeTab(delegate
			{
				this._curToggleKey = (ViewMod.ToggleKey)newIndex;
				this.Refresh();
			}, delegate
			{
				this.subPageToggleGroup.SetWithoutNotify(oldIndex);
			});
		}

		// Token: 0x06006BF5 RID: 27637 RVA: 0x0031C7C8 File Offset: 0x0031A9C8
		public void Refresh()
		{
			int index = this.subPageToggleGroup.GetActiveIndex();
			for (int i = 0; i < this.subPages.Length; i++)
			{
				ModSubPage subPage = this.subPages[i];
				bool isShow = index == i;
				subPage.gameObject.SetActive(isShow);
				bool flag = isShow;
				if (flag)
				{
					subPage.Refresh();
				}
			}
		}

		// Token: 0x06006BF6 RID: 27638 RVA: 0x0031C828 File Offset: 0x0031AA28
		public void ShowMask()
		{
			ArgumentBox box = EasyPool.Get<ArgumentBox>();
			box.Set("ShowBlackMask", true);
			box.Set("ShowWaitAnimation", true);
			box.Set("Message", LocalStringManager.Get(LanguageKey.LK_Waiting));
			UIElement.FullScreenMask.SetOnInitArgs(box);
			UIElement.FullScreenMask.Show();
			this.StopMaskTimer();
			this.StartMaskTimer();
		}

		// Token: 0x06006BF7 RID: 27639 RVA: 0x0031C891 File Offset: 0x0031AA91
		public void HideMask()
		{
			UIElement.FullScreenMask.Hide(false);
			this.StopMaskTimer();
		}

		// Token: 0x06006BF8 RID: 27640 RVA: 0x0031C8A7 File Offset: 0x0031AAA7
		private IEnumerator CorMaskTimer()
		{
			yield return new WaitForSeconds(15f);
			bool exist = UIElement.FullScreenMask.Exist;
			if (exist)
			{
				this.HideMask();
				string title = LocalStringManager.Get(LanguageKey.LK_Common_Attention);
				string content = LocalStringManager.Get(LanguageKey.LK_Mod_Loading_Failed);
				CommonUtils.ShowDialog(title, content, null, EDialogType.None);
				title = null;
				content = null;
			}
			yield break;
		}

		// Token: 0x06006BF9 RID: 27641 RVA: 0x0031C8B8 File Offset: 0x0031AAB8
		private void StopMaskTimer()
		{
			bool flag = this._corMaskTimer == null;
			if (!flag)
			{
				base.StopCoroutine(this._corMaskTimer);
				this._corMaskTimer = null;
			}
		}

		// Token: 0x06006BFA RID: 27642 RVA: 0x0031C8EC File Offset: 0x0031AAEC
		private void StartMaskTimer()
		{
			bool flag = !base.gameObject.activeSelf;
			if (!flag)
			{
				this._corMaskTimer = base.StartCoroutine(this.CorMaskTimer());
			}
		}

		// Token: 0x06006BFB RID: 27643 RVA: 0x0031C920 File Offset: 0x0031AB20
		public static string GetTimeString(uint time)
		{
			return DateTimeOffset.FromUnixTimeSeconds((long)((ulong)time)).DateTime.ToLocalTime().ToString(CultureInfo.CurrentCulture);
		}

		// Token: 0x06006BFC RID: 27644 RVA: 0x0031C958 File Offset: 0x0031AB58
		public void DownloadMod(ModId modId)
		{
			PublishedFileId_t publishedFileId = new PublishedFileId_t(modId.FileId);
			SteamUGC.DownloadItem(publishedFileId, true);
			this._downLoadingItemList.Add(modId);
			base.StopCoroutine(this.CorUpdateDownloadingMod());
			base.StartCoroutine(this.CorUpdateDownloadingMod());
		}

		// Token: 0x06006BFD RID: 27645 RVA: 0x0031C9A2 File Offset: 0x0031ABA2
		private IEnumerator CorUpdateDownloadingMod()
		{
			while (this._downLoadingItemList.Count > 0)
			{
				bool hasChanged = false;
				foreach (ModId modId in this._downLoadingItemList)
				{
					bool flag = SteamManager.IsItemStateActive(modId.FileId, EItemState.k_EItemStateInstalled) && !SteamManager.IsItemStateActive(modId.FileId, EItemState.k_EItemStateNeedsUpdate) && !SteamManager.IsItemStateActive(modId.FileId, EItemState.k_EItemStateDownloading);
					if (flag)
					{
						this._downLoadedItemList.Add(modId);
						hasChanged = true;
					}
					modId = default(ModId);
				}
				List<ModId>.Enumerator enumerator = default(List<ModId>.Enumerator);
				foreach (ModId modId2 in this._downLoadedItemList)
				{
					bool flag2 = this._downLoadingItemList.Contains(modId2);
					if (flag2)
					{
						this._downLoadingItemList.Remove(modId2);
					}
					modId2 = default(ModId);
				}
				List<ModId>.Enumerator enumerator2 = default(List<ModId>.Enumerator);
				bool flag3 = hasChanged;
				if (flag3)
				{
					this._downLoadedItemList.Clear();
					bool flag4 = this._curToggleKey == ViewMod.ToggleKey.CurMod;
					if (flag4)
					{
						this.Refresh();
					}
				}
				yield return null;
			}
			yield break;
		}

		// Token: 0x06006BFE RID: 27646 RVA: 0x0031C9B4 File Offset: 0x0031ABB4
		private void UnSubscribeItem(ModId modId)
		{
			ModManager.UnSubscribeItem(modId);
			bool flag = this._downLoadingItemList.Contains(modId);
			if (flag)
			{
				this._downLoadingItemList.Remove(modId);
			}
			bool flag2 = this._downLoadedItemList.Contains(modId);
			if (flag2)
			{
				this._downLoadedItemList.Remove(modId);
			}
		}

		// Token: 0x04004E3D RID: 20029
		[SerializeField]
		private ModSubPage[] subPages;

		// Token: 0x04004E3E RID: 20030
		[SerializeField]
		private CToggleGroup subPageToggleGroup;

		// Token: 0x04004E3F RID: 20031
		[SerializeField]
		private CButton buttonClose;

		// Token: 0x04004E40 RID: 20032
		private Coroutine _corMaskTimer;

		// Token: 0x04004E41 RID: 20033
		private readonly List<ModId> _downLoadingItemList = new List<ModId>();

		// Token: 0x04004E42 RID: 20034
		private readonly List<ModId> _downLoadedItemList = new List<ModId>();

		// Token: 0x04004E43 RID: 20035
		private ViewMod.ToggleKey _curToggleKey = ViewMod.ToggleKey.Invalid;

		// Token: 0x02001DBF RID: 7615
		private enum ToggleKey
		{
			// Token: 0x0400C73F RID: 51007
			Invalid = -1,
			// Token: 0x0400C740 RID: 51008
			CurMod,
			// Token: 0x0400C741 RID: 51009
			WorkshopMod,
			// Token: 0x0400C742 RID: 51010
			UploadMod
		}
	}
}
