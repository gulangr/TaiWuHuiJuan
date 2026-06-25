using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Config;
using FrameWork;
using FrameWork.UISystem.UIElements;
using Game.Components.Common;
using GameDataExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Game.Views.UpdateLog
{
	// Token: 0x02000747 RID: 1863
	public class ViewUpdateLog : UIBase
	{
		// Token: 0x06005A39 RID: 23097 RVA: 0x0029DF08 File Offset: 0x0029C108
		private void Awake()
		{
			base.GetComponent<CanvasGroup>().alpha = 0f;
		}

		// Token: 0x06005A3A RID: 23098 RVA: 0x0029DF1C File Offset: 0x0029C11C
		private void Update()
		{
			bool needCheckPackers = this._needCheckPackers;
			if (needCheckPackers)
			{
				bool loadFinish = true;
				foreach (SpriteAtlas item in this.RelativeAtlases)
				{
					bool flag = AtlasInfo.Instance.GetLoadedPacker(item.name) == null;
					if (flag)
					{
						loadFinish = false;
					}
				}
				bool flag2 = loadFinish;
				if (flag2)
				{
					this._needCheckPackers = false;
					base.GetComponent<CanvasGroup>().alpha = 1f;
				}
			}
		}

		// Token: 0x17000AB8 RID: 2744
		// (get) Token: 0x06005A3B RID: 23099 RVA: 0x0029DF97 File Offset: 0x0029C197
		private string _versionTagIconPath
		{
			get
			{
				return "RemakeResources/Textures/RemakeTextures/UpdateLog";
			}
		}

		// Token: 0x17000AB9 RID: 2745
		// (get) Token: 0x06005A3C RID: 23100 RVA: 0x0029DF9E File Offset: 0x0029C19E
		private string _subentryIconPath
		{
			get
			{
				return "RemakeResources/Textures/RemakeTextures/UpdateLog";
			}
		}

		// Token: 0x06005A3D RID: 23101 RVA: 0x0029DFA8 File Offset: 0x0029C1A8
		public override void OnInit(ArgumentBox argsBox)
		{
			this.LoadVersionTagIdListCache();
			this._stringBuilder = new StringBuilder();
			this._instantiatedSubentry = new List<Refers>();
			this._activedSubentryAmount = 0;
			this.pageSwitcher.PageItemRefreshHandler = new Action<int, Refers>(this.RefreshVersionTags);
			this.pageSwitcher.SetItemSelectStateHandler = new Action<Refers, bool>(this.SetVersionTagsSelect);
			this.pageSwitcher.RegisterOnSelectIndexChangeHandler(new Action<int>(this.OnSelectIndexChange));
			this.pageSwitcher.InitPageCount(UpdateLog.Instance.Count, UpdateLog.Instance.Count - 1, false);
			this._currentLanguageType = LocalStringManager.CurLanguageType;
		}

		// Token: 0x06005A3E RID: 23102 RVA: 0x0029E04E File Offset: 0x0029C24E
		public override void OnLanguageChange(LocalStringManager.LanguageType languageType)
		{
			base.OnLanguageChange(languageType);
			this._currentLanguageType = languageType;
			this.RefreshVersionTitle();
		}

		// Token: 0x06005A3F RID: 23103 RVA: 0x0029E068 File Offset: 0x0029C268
		private void OnDisable()
		{
			for (int i = 0; i < this._instantiatedSubentry.Count; i++)
			{
				Object.Destroy(this._instantiatedSubentry[i].gameObject);
			}
			this._instantiatedSubentry = null;
			this.pageSwitcher.InitPageCount(0, 0, false);
		}

		// Token: 0x06005A40 RID: 23104 RVA: 0x0029E0C0 File Offset: 0x0029C2C0
		private void LoadVersionTagIdListCache()
		{
			this._versionLogIdList = (from e in UpdateLog.Instance
			orderby e.IncrementSortOrder
			select e.TemplateId).ToList<byte>();
		}

		// Token: 0x06005A41 RID: 23105 RVA: 0x0029E128 File Offset: 0x0029C328
		protected override void OnClick(Transform btn)
		{
			base.OnClick(btn);
			string btnName = btn.name;
			string text = btnName;
			string a = text;
			if (!(a == "ViewDetailLog"))
			{
				if (a == "ButtonClosePopup")
				{
					UIManager.Instance.HideUI(UIElement.UpdateLog);
				}
			}
			else
			{
				Application.OpenURL(UpdateLog.Instance.GetItem(this._currentLogId).OfficialLink);
			}
		}

		// Token: 0x06005A42 RID: 23106 RVA: 0x0029E194 File Offset: 0x0029C394
		private void OnSelectIndexChange(int index)
		{
			foreach (Refers item in this._instantiatedSubentry)
			{
				item.gameObject.SetActive(false);
			}
			this._activedSubentryAmount = 0;
			this._currentLogId = this._versionLogIdList[index];
			UpdateLogItem updateLogConfig = UpdateLog.Instance.GetItem(this._currentLogId);
			this.RefreshVersionTitle();
			this.subPageTitleIcon.SetTexture(updateLogConfig.SubentryTitleIcon);
			for (int i = 0; i < updateLogConfig.SubentryTitles.Length; i++)
			{
				string subentryIcon = (updateLogConfig.SubentryIcons == null || updateLogConfig.SubentryIcons.Length <= i) ? "" : updateLogConfig.SubentryIcons[i];
				this.AddSubentry(updateLogConfig.SubentryTitles[i], subentryIcon, updateLogConfig.SubentryDescriptions[i]);
			}
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(this.subentryScrollRect.Content);
				this.subentryScrollRect.ScrollBar.value = 0f;
			});
		}

		// Token: 0x06005A43 RID: 23107 RVA: 0x0029E2A8 File Offset: 0x0029C4A8
		private void RefreshVersionTitle()
		{
			UpdateLogItem updateLogConfig = UpdateLog.Instance.GetItem(this._currentLogId);
			this._stringBuilder.Clear();
			this._stringBuilder.Append(updateLogConfig.VersionPublishDate);
			bool flag = this._currentLanguageType > LocalStringManager.LanguageType.CN;
			if (flag)
			{
				this._stringBuilder.Append(" ");
				this._stringBuilder.Append(LocalStringManager.Get(updateLogConfig.VersionTitle));
			}
			this.versionTitle.text = this._stringBuilder.ToString();
		}

		// Token: 0x06005A44 RID: 23108 RVA: 0x0029E334 File Offset: 0x0029C534
		private void AddSubentry(string subentryTitle, string subentryIcon, string subentryDescription)
		{
			bool flag = this._activedSubentryAmount < this._instantiatedSubentry.Count;
			Refers subentryInstance;
			if (flag)
			{
				subentryInstance = this._instantiatedSubentry[this._activedSubentryAmount];
			}
			else
			{
				subentryInstance = Object.Instantiate<Refers>(this.subentryPrefab, this.subentryScrollRect.Content);
				this._instantiatedSubentry.Add(subentryInstance);
			}
			CRawImage versionTagRawImage = subentryInstance.CGet<CRawImage>("VersionTagIcon");
			versionTagRawImage.SetTexture(subentryIcon);
			string titleLocal = LocalStringManager.Get(subentryTitle);
			TextMeshProUGUI txtComp = subentryInstance.CGet<TextMeshProUGUI>("subentryTitle");
			txtComp.fontSize = (float)(titleLocal.Contains('\n') ? 28 : 30);
			txtComp.text = titleLocal;
			subentryInstance.CGet<TextMeshProUGUI>("SubentryDescribe").text = LocalStringManager.Get(subentryDescription);
			this._activedSubentryAmount++;
			subentryInstance.gameObject.SetActive(true);
		}

		// Token: 0x06005A45 RID: 23109 RVA: 0x0029E412 File Offset: 0x0029C612
		private void SetVersionTagsSelect(Refers refers, bool selectState)
		{
			refers.CGet<GameObject>("selectedFrame").SetActive(selectState);
		}

		// Token: 0x06005A46 RID: 23110 RVA: 0x0029E428 File Offset: 0x0029C628
		private void RefreshVersionTags(int index, Refers referItem)
		{
			UpdateLogItem updateLogConfig = UpdateLog.Instance.GetItem(this._versionLogIdList[index]);
			referItem.CGet<CButton>("versionTagButton").onClick.RemoveAllListeners();
			referItem.CGet<CButton>("versionTagButton").onClick.AddListener(delegate()
			{
				this.pageSwitcher.SetSelect(index, true);
			});
			CRawImage versionTagRawImage = referItem.CGet<CRawImage>("versionTagIcon");
			versionTagRawImage.SetTexture(updateLogConfig.VersionTagIcon);
			TextMeshProUGUI textComp = referItem.CGet<TextMeshProUGUI>("Label_Normal");
			textComp.text = LocalStringManager.Get(updateLogConfig.VersionTitle);
			textComp.fontSize = (float)((LocalStringManagerHelper.CurLanguageType == LocalStringManager.LanguageType.CN) ? 36 : 20);
		}

		// Token: 0x04003E26 RID: 15910
		[SerializeField]
		private CRawImage subPageTitleIcon;

		// Token: 0x04003E27 RID: 15911
		[SerializeField]
		private PageSwitchHorizontalNormalHeadController pageSwitcher;

		// Token: 0x04003E28 RID: 15912
		[SerializeField]
		private CScrollRect subentryScrollRect;

		// Token: 0x04003E29 RID: 15913
		[SerializeField]
		private Refers subentryPrefab;

		// Token: 0x04003E2A RID: 15914
		[SerializeField]
		private CButton buttonClosePopup;

		// Token: 0x04003E2B RID: 15915
		[SerializeField]
		private CButton viewDetailLog;

		// Token: 0x04003E2C RID: 15916
		[SerializeField]
		private TextMeshProUGUI versionTitle;

		// Token: 0x04003E2D RID: 15917
		private bool _needCheckPackers = true;

		// Token: 0x04003E2E RID: 15918
		private List<byte> _versionLogIdList;

		// Token: 0x04003E2F RID: 15919
		private byte _currentLogId;

		// Token: 0x04003E30 RID: 15920
		private List<Refers> _instantiatedSubentry;

		// Token: 0x04003E31 RID: 15921
		private int _activedSubentryAmount;

		// Token: 0x04003E32 RID: 15922
		private StringBuilder _stringBuilder;

		// Token: 0x04003E33 RID: 15923
		private LocalStringManager.LanguageType _currentLanguageType;
	}
}
