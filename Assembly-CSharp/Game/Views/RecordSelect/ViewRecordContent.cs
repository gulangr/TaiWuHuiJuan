using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameWork;
using FrameWork.ModSystem;
using FrameWork.UISystem.Components;
using Game.Views.DLCIntroduce;
using GameData.DLC;
using GameData.Domains.Mod;
using TMPro;
using UnityEngine;

namespace Game.Views.RecordSelect
{
	// Token: 0x020007BB RID: 1979
	public class ViewRecordContent : UIBase
	{
		// Token: 0x06006091 RID: 24721 RVA: 0x002C3FB4 File Offset: 0x002C21B4
		public override void OnInit(ArgumentBox argsBox)
		{
			this._addedModList.Clear();
			this._removedModList.Clear();
			this._recordModList.Clear();
			this._showModList.Clear();
			int localModCount = 0;
			List<ModId> recordModList;
			bool flag = argsBox.Get<List<ModId>>("ModList", out recordModList) && recordModList != null;
			if (flag)
			{
				foreach (ModId modId in recordModList)
				{
					bool flag2 = ModManager.PlatformMods.Contains(modId);
					if (flag2)
					{
						this._recordModList.Add(modId);
					}
					else
					{
						localModCount++;
					}
				}
			}
			base.CGet<GameObject>("UnknownModCountBG").SetActive(localModCount > 0);
			string str = LocalStringManager.GetFormat(LanguageKey.LK_RecordContent_Unknown_Tip, localModCount);
			base.CGet<TextMeshProUGUI>("UnknownModCount").SetText(str, true);
			List<ModId> curModList = (from id in ModManager.EnabledMods
			where id.Source == 1
			select id).ToList<ModId>();
			this._addedModList.AddRange(curModList.Except(this._recordModList));
			this._removedModList.AddRange(this._recordModList.Except(curModList));
			this._showModList.AddRange(this._removedModList);
			this._showModList.AddRange(this._addedModList);
			InfinityScroll modScroll = base.CGet<InfinityScroll>("ModScrollView");
			modScroll.OnItemRender -= this.OnModItemRender;
			modScroll.OnItemRender += this.OnModItemRender;
			modScroll.SetDataCount(this._showModList.Count);
			modScroll.gameObject.SetActive(this._showModList.Count > 0);
			this._addedDLCList.Clear();
			this._removedDLCList.Clear();
			this._recordDLCList.Clear();
			this._showDLCList.Clear();
			List<DlcId> recordDLcList;
			bool flag3 = argsBox.Get<List<DlcId>>("DLCList", out recordDLcList) && recordDLcList != null;
			if (flag3)
			{
				this._recordDLCList.AddRange(recordDLcList);
			}
			List<DlcId> curDlcIdList = SingletonObject.getInstance<DlcManager>().GetDlcIdList();
			this._addedDLCList.AddRange(from gameDlc in curDlcIdList
			where this._recordDLCList.All((DlcId worldDlc) => worldDlc.AppId != gameDlc.AppId)
			select gameDlc);
			this._removedDLCList.AddRange(from worldDlc in this._recordDLCList
			where curDlcIdList.All((DlcId gameDlc) => gameDlc.AppId != worldDlc.AppId)
			select worldDlc);
			this._showDLCList.AddRange(this._removedDLCList);
			this._showDLCList.AddRange(this._addedDLCList);
			InfinityScroll dlcScroll = base.CGet<InfinityScroll>("DLCScrollView");
			dlcScroll.OnItemRender -= this.OnDlcItemRender;
			dlcScroll.OnItemRender += this.OnDlcItemRender;
			dlcScroll.SetDataCount(this._showDLCList.Count);
			RectTransform rect = dlcScroll.GetComponent<RectTransform>();
			Vector2 sizeDelta = rect.sizeDelta;
			dlcScroll.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeDelta.x, (float)((this._showModList.Count > 0) ? 426 : 824));
		}

		// Token: 0x06006092 RID: 24722 RVA: 0x002C430C File Offset: 0x002C250C
		private void OnModItemRender(int index, GameObject obj)
		{
			ModId modId = this._showModList[index];
			ModInfoWithDisplayData modInfo = ModManager.GetModInfo(modId);
			string title = modInfo.Title.SetColor("pinkyellow");
			string nameTitle = LocalStringManager.Get(LanguageKey.LK_Mod_Name).SetColor("lightgrey");
			string colonSymbol = LocalStringManager.Get(LanguageKey.LK_Colon_Symbol).SetColor("lightgrey");
			string authorTitle = LocalStringManager.Get(LanguageKey.LK_Author).SetColor("lightgrey");
			string author = modInfo.Author.SetColor("pinkyellow");
			StringBuilder sb = EasyPool.Get<StringBuilder>();
			sb.Append(nameTitle);
			sb.Append(colonSymbol);
			sb.Append(title);
			sb.AppendLine();
			sb.Append(authorTitle);
			sb.Append(colonSymbol);
			sb.Append(author);
			string tipContent = sb.ToString();
			EasyPool.Free<StringBuilder>(sb);
			bool isAdded = this._addedModList.Contains(modId);
			bool flag = this._removedModList.Contains(modId) && ModManager.PlatformMods.Contains(modId);
			if (flag)
			{
				bool flag2 = SteamManager.CheckModHasChangeGameConfig(modId) || modInfo.ChangeConfig || modInfo.HasArchive;
				if (flag2)
				{
				}
			}
			ContentItem contentItem = obj.GetComponent<ContentItem>();
			contentItem.Set(isAdded, title, authorTitle + colonSymbol + author, null, tipContent);
		}

		// Token: 0x06006093 RID: 24723 RVA: 0x002C4468 File Offset: 0x002C2668
		private void OnDlcItemRender(int index, GameObject obj)
		{
			DlcId dlcId = this._showDLCList[index];
			string dlcName = SingletonObject.getInstance<DlcManager>().GetDlcName(dlcId);
			string title = dlcName.SetColor("pinkyellow");
			bool isAdded = this._addedDLCList.Contains(dlcId);
			ContentItem contentItem = obj.GetComponent<ContentItem>();
			contentItem.Set(isAdded, title, string.Empty, delegate
			{
				int dlcIndex = DLCIntroduceHelper.GetDlcIndex(dlcId.AppId);
				UIElement.DLCIntroduce.SetOnInitArgs(EasyPool.Get<ArgumentBox>().Set("SelectedIndex", dlcIndex));
				UIManager.Instance.MaskUI(UIElement.DLCIntroduce);
			}, string.Empty);
		}

		// Token: 0x06006094 RID: 24724 RVA: 0x002C44E8 File Offset: 0x002C26E8
		private void RefreshTip(TooltipInvoker bgTip, bool isAdded)
		{
			bgTip.Type = TipType.Simple;
			bool flag = bgTip.PresetParam == null || bgTip.PresetParam.Length < 2;
			if (flag)
			{
				bgTip.PresetParam = new string[2];
			}
			LanguageKey titleKey = isAdded ? LanguageKey.LK_RecordContent_Increased_Tip_Title : LanguageKey.LK_RecordContent_Reduced_Tip_Title;
			LanguageKey contentKey = isAdded ? LanguageKey.LK_RecordContent_Increased_Tip_Content : LanguageKey.LK_RecordContent_Reduced_Tip_Content;
			bgTip.PresetParam[0] = LocalStringManager.Get(titleKey);
			bgTip.PresetParam[1] = LocalStringManager.Get(contentKey);
		}

		// Token: 0x06006095 RID: 24725 RVA: 0x002C4560 File Offset: 0x002C2760
		protected override void OnClick(Transform btn)
		{
			bool flag = btn.name == "BtnClose";
			if (flag)
			{
				this.QuickHide();
			}
		}

		// Token: 0x040042F2 RID: 17138
		private const string AddedSpriteName = "popup_modpanel_compare_0";

		// Token: 0x040042F3 RID: 17139
		private const string RemovedSpriteName = "popup_modpanel_compare_1";

		// Token: 0x040042F4 RID: 17140
		private const string RemovedConfigSpriteName = "popup_modpanel_compare_2";

		// Token: 0x040042F5 RID: 17141
		private readonly List<ModId> _addedModList = new List<ModId>();

		// Token: 0x040042F6 RID: 17142
		private readonly List<ModId> _removedModList = new List<ModId>();

		// Token: 0x040042F7 RID: 17143
		private readonly List<ModId> _recordModList = new List<ModId>();

		// Token: 0x040042F8 RID: 17144
		private readonly List<ModId> _showModList = new List<ModId>();

		// Token: 0x040042F9 RID: 17145
		private readonly List<DlcId> _addedDLCList = new List<DlcId>();

		// Token: 0x040042FA RID: 17146
		private readonly List<DlcId> _removedDLCList = new List<DlcId>();

		// Token: 0x040042FB RID: 17147
		private readonly List<DlcId> _recordDLCList = new List<DlcId>();

		// Token: 0x040042FC RID: 17148
		private readonly List<DlcId> _showDLCList = new List<DlcId>();
	}
}
