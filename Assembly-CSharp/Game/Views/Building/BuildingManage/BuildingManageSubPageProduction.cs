using System;
using System.Collections.Generic;
using Config;
using Game.Views.Building.BuildingManage.Production;
using GameData.Domains.Building;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000BFE RID: 3070
	public class BuildingManageSubPageProduction : BuildingManageSubPage, IProductionHandler
	{
		// Token: 0x17001077 RID: 4215
		// (get) Token: 0x06009C0A RID: 39946 RVA: 0x00491CFE File Offset: 0x0048FEFE
		// (set) Token: 0x06009C0B RID: 39947 RVA: 0x00491D06 File Offset: 0x0048FF06
		public bool DataChanged { get; set; }

		// Token: 0x17001078 RID: 4216
		// (get) Token: 0x06009C0C RID: 39948 RVA: 0x00491D0F File Offset: 0x0048FF0F
		public BuildingModel BuildingModel
		{
			get
			{
				return base.Model;
			}
		}

		// Token: 0x17001079 RID: 4217
		// (get) Token: 0x06009C0D RID: 39949 RVA: 0x00491D17 File Offset: 0x0048FF17
		public BuildingBlockKey BlockKey
		{
			get
			{
				return this.ParentView.BlockKey;
			}
		}

		// Token: 0x1700107A RID: 4218
		// (get) Token: 0x06009C0E RID: 39950 RVA: 0x00491D24 File Offset: 0x0048FF24
		public BuildingBlockData BlockData
		{
			get
			{
				return this.ParentView.BlockData;
			}
		}

		// Token: 0x1700107B RID: 4219
		// (get) Token: 0x06009C0F RID: 39951 RVA: 0x00491D31 File Offset: 0x0048FF31
		private BuildingBlockItem ConfigData
		{
			get
			{
				return this.ParentView.ConfigData;
			}
		}

		// Token: 0x06009C10 RID: 39952 RVA: 0x00491D40 File Offset: 0x0048FF40
		private void Awake()
		{
			this._components = base.GetComponentsInChildren<IProductionComponent>(true);
			foreach (IProductionComponent component in this._components)
			{
				component.Setup(this);
			}
			bool flag = this.DisplayData == null;
			if (!flag)
			{
				this.RefreshComponents();
			}
		}

		// Token: 0x06009C11 RID: 39953 RVA: 0x00491DB4 File Offset: 0x0048FFB4
		private void OnDisable()
		{
			bool dataChanged = this.DataChanged;
			if (dataChanged)
			{
				this.DataChanged = false;
				GEvent.OnEvent(UiEvents.UpdateAllBlockInfo, null);
			}
		}

		// Token: 0x06009C12 RID: 39954 RVA: 0x00491DE4 File Offset: 0x0048FFE4
		public override void Refresh(BuildingManageDisplayData displayData)
		{
			base.Refresh(displayData);
			bool flag = this._components == null;
			if (!flag)
			{
				this.RefreshComponents();
			}
		}

		// Token: 0x06009C13 RID: 39955 RVA: 0x00491E10 File Offset: 0x00490010
		private void RefreshComponents()
		{
			foreach (IProductionComponent component in this._components)
			{
				component.Refresh();
			}
		}

		// Token: 0x1700107C RID: 4220
		// (get) Token: 0x06009C14 RID: 39956 RVA: 0x00491E60 File Offset: 0x00490060
		BuildingBlockKey IProductionHandler.Key
		{
			get
			{
				return this.ParentView.BlockKey;
			}
		}

		// Token: 0x1700107D RID: 4221
		// (get) Token: 0x06009C15 RID: 39957 RVA: 0x00491E6D File Offset: 0x0049006D
		BuildingManageDisplayData IProductionHandler.Data
		{
			get
			{
				return this.DisplayData;
			}
		}

		// Token: 0x1700107E RID: 4222
		// (get) Token: 0x06009C16 RID: 39958 RVA: 0x00491E75 File Offset: 0x00490075
		IAsyncMethodRequestHandler IProductionHandler.Async
		{
			get
			{
				return this.ParentView;
			}
		}

		// Token: 0x06009C17 RID: 39959 RVA: 0x00491E7D File Offset: 0x0049007D
		void IProductionHandler.Reload()
		{
			this.ParentView.RequestData();
		}

		// Token: 0x040078E2 RID: 30946
		private IReadOnlyList<IProductionComponent> _components;
	}
}
