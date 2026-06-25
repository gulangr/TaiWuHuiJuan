using System;
using System.Collections.Generic;
using Config;
using GameData.Domains.Building;

namespace Game.Views.Building.BuildingManage.Production
{
	// Token: 0x02000C14 RID: 3092
	public interface IProductionHandler
	{
		// Token: 0x1700109B RID: 4251
		// (get) Token: 0x06009D3B RID: 40251
		// (set) Token: 0x06009D3C RID: 40252
		bool DataChanged { get; set; }

		// Token: 0x1700109C RID: 4252
		// (get) Token: 0x06009D3D RID: 40253
		BuildingBlockKey Key { get; }

		// Token: 0x1700109D RID: 4253
		// (get) Token: 0x06009D3E RID: 40254
		BuildingManageDisplayData Data { get; }

		// Token: 0x1700109E RID: 4254
		// (get) Token: 0x06009D3F RID: 40255
		IAsyncMethodRequestHandler Async { get; }

		// Token: 0x1700109F RID: 4255
		// (get) Token: 0x06009D40 RID: 40256 RVA: 0x0049AC4A File Offset: 0x00498E4A
		short BlockIndex
		{
			get
			{
				return this.Data.BlockData.BlockIndex;
			}
		}

		// Token: 0x170010A0 RID: 4256
		// (get) Token: 0x06009D41 RID: 40257 RVA: 0x0049AC5C File Offset: 0x00498E5C
		short TemplateId
		{
			get
			{
				return this.Data.BlockData.TemplateId;
			}
		}

		// Token: 0x170010A1 RID: 4257
		// (get) Token: 0x06009D42 RID: 40258 RVA: 0x0049AC6E File Offset: 0x00498E6E
		BuildingBlockItem Template
		{
			get
			{
				return BuildingBlock.Instance[this.TemplateId];
			}
		}

		// Token: 0x170010A2 RID: 4258
		// (get) Token: 0x06009D43 RID: 40259 RVA: 0x0049AC80 File Offset: 0x00498E80
		ShopEventItem ShopEvent
		{
			get
			{
				BuildingBlockItem template = this.Template;
				List<short> list = (template != null) ? template.SuccesEvent : null;
				return (list != null && list.Count > 0) ? Config.ShopEvent.Instance[this.Template.SuccesEvent[0]] : null;
			}
		}

		// Token: 0x170010A3 RID: 4259
		// (get) Token: 0x06009D44 RID: 40260 RVA: 0x0049ACCC File Offset: 0x00498ECC
		bool IsSold
		{
			get
			{
				ShopEventItem shopEvent = this.ShopEvent;
				return shopEvent != null && shopEvent.ExchangeResourceGoods >= 0;
			}
		}

		// Token: 0x170010A4 RID: 4260
		// (get) Token: 0x06009D45 RID: 40261 RVA: 0x0049ACF4 File Offset: 0x00498EF4
		bool IsRecruit
		{
			get
			{
				ShopEventItem shopEvent = this.ShopEvent;
				List<sbyte> list = (shopEvent != null) ? shopEvent.RecruitPeopleProb : null;
				return list != null && list.Count > 0;
			}
		}

		// Token: 0x170010A5 RID: 4261
		// (get) Token: 0x06009D46 RID: 40262 RVA: 0x0049AD23 File Offset: 0x00498F23
		bool IsEntertain
		{
			get
			{
				return this.TemplateId == 47;
			}
		}

		// Token: 0x170010A6 RID: 4262
		// (get) Token: 0x06009D47 RID: 40263 RVA: 0x0049AD30 File Offset: 0x00498F30
		bool NeedResource
		{
			get
			{
				short templateId = this.TemplateId;
				return templateId == 222 || templateId == 223;
			}
		}

		// Token: 0x06009D48 RID: 40264
		void Reload();
	}
}
