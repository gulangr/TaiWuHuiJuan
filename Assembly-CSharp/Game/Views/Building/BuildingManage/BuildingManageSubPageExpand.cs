using System;
using Config;
using GameData.Domains.Building;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BFA RID: 3066
	public class BuildingManageSubPageExpand : BuildingManageSubPage
	{
		// Token: 0x17001076 RID: 4214
		// (get) Token: 0x06009BED RID: 39917 RVA: 0x00490FEC File Offset: 0x0048F1EC
		private BuildingBlockKey _key
		{
			get
			{
				return this.ParentView.BlockKey;
			}
		}

		// Token: 0x06009BEE RID: 39918 RVA: 0x00490FFC File Offset: 0x0048F1FC
		public static bool CheckCanOpenExpandPage(short blockTemplateId)
		{
			bool flag = blockTemplateId == 44;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				BuildingBlockItem blockConfig = BuildingBlock.Instance[blockTemplateId];
				bool isResource = blockConfig.Class == EBuildingBlockClass.BornResource;
				bool flag2 = isResource;
				result = flag2;
			}
			return result;
		}

		// Token: 0x06009BEF RID: 39919 RVA: 0x0049103D File Offset: 0x0048F23D
		private void Awake()
		{
		}

		// Token: 0x06009BF0 RID: 39920 RVA: 0x00491040 File Offset: 0x0048F240
		public override void Init(ViewBuildingManage parentView)
		{
			base.Init(parentView);
			bool inited = this._inited;
			if (!inited)
			{
				this.expandResourcePage.Init(new Action(this.OnRefresh));
				this.taiwuVowPage.Init(new Action(this.OnRefresh));
				this._inited = true;
			}
		}

		// Token: 0x06009BF1 RID: 39921 RVA: 0x00491099 File Offset: 0x0048F299
		private void OnRefresh()
		{
			this.ParentView.RequestData();
		}

		// Token: 0x06009BF2 RID: 39922 RVA: 0x004910A8 File Offset: 0x0048F2A8
		public override void Refresh(BuildingManageDisplayData displayData)
		{
			base.Refresh(displayData);
			bool isTaiwuVow = displayData.BlockData.TemplateId == 44;
			BuildingBlockItem blockConfig = BuildingBlock.Instance[displayData.BlockData.TemplateId];
			bool isResource = blockConfig.Class == EBuildingBlockClass.BornResource;
			this.expandResourcePage.gameObject.SetActive(isResource);
			bool flag = isResource;
			if (flag)
			{
				this.expandResourcePage.Refresh(this._key, displayData.BlockData);
			}
			this.taiwuVowPage.gameObject.SetActive(isTaiwuVow);
			bool flag2 = isTaiwuVow;
			if (flag2)
			{
				this.taiwuVowPage.Refresh(displayData.BlockData, this._key);
			}
		}

		// Token: 0x040078C9 RID: 30921
		[SerializeField]
		private SubPageExpandTaiwuVow taiwuVowPage;

		// Token: 0x040078CA RID: 30922
		[SerializeField]
		private BuildingExpandResourceComponent expandResourcePage;

		// Token: 0x040078CB RID: 30923
		private bool _inited = false;
	}
}
