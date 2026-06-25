using System;
using System.Collections.Generic;
using System.Text;
using FrameWork;
using GameData.Domains.Building;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage.Production
{
	// Token: 0x02000C1E RID: 3102
	public class ProductionRewardsTitle : MonoBehaviour, IProductionComponent
	{
		// Token: 0x06009D97 RID: 40343 RVA: 0x0049D3FD File Offset: 0x0049B5FD
		public void Setup(IProductionHandler handler)
		{
			this._handler = handler;
		}

		// Token: 0x06009D98 RID: 40344 RVA: 0x0049D408 File Offset: 0x0049B608
		public void Refresh()
		{
			StringBuilder builder = EasyPool.Get<StringBuilder>();
			builder.Clear();
			bool isEntertain = this._handler.IsEntertain;
			if (isEntertain)
			{
				int current = this._handler.Data.Feast.GetInUseGiftSlotCount();
				int max = GlobalConfig.Instance.FeastGiftCount;
				builder.Append(LanguageKey.LK_Building_Reward_Content.TrFormat(current.ToString(), max.ToString()));
			}
			else
			{
				bool isSold = this._handler.IsSold;
				if (isSold)
				{
					int current2 = this.CalcSoldItemCount();
					this.GenerateLegacyText(builder, LanguageKey.LK_Building_ShopSoldItem, current2);
				}
				else
				{
					bool isRecruit = this._handler.IsRecruit;
					if (isRecruit)
					{
						BuildingEarningsData earningsData = this._handler.Data.EarningsData;
						int? num;
						if (earningsData == null)
						{
							num = null;
						}
						else
						{
							List<IntPair> recruitLevelList = earningsData.RecruitLevelList;
							num = ((recruitLevelList != null) ? new int?(recruitLevelList.Count) : null);
						}
						int? num2 = num;
						int current3 = num2.GetValueOrDefault();
						this.GenerateLegacyText(builder, LanguageKey.LK_Building_ShopRecruitPeople, current3);
					}
					else
					{
						builder.Append(LanguageKey.LK_Building_ShopItemOutput.Tr());
					}
				}
			}
			this.titleText.text = builder.ToString();
			EasyPool.Free<StringBuilder>(builder);
		}

		// Token: 0x06009D99 RID: 40345 RVA: 0x0049D540 File Offset: 0x0049B740
		private void GenerateLegacyText(StringBuilder builder, LanguageKey title, int current)
		{
			builder.Append(title.Tr());
			builder.Append("  (");
			builder.Append(current.ToString());
			builder.Append("/");
			builder.Append(SharedMethods.GetBuildingSlotCount(this._handler.TemplateId));
			builder.Append(")");
		}

		// Token: 0x06009D9A RID: 40346 RVA: 0x0049D5A4 File Offset: 0x0049B7A4
		private int CalcSoldItemCount()
		{
			BuildingEarningsData earningsData = this._handler.Data.EarningsData;
			bool flag = earningsData == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int count = 0;
				for (int i = 0; i < earningsData.ShopSoldItemList.Count; i++)
				{
					bool flag2 = earningsData.ShopSoldItemList[i].TemplateId >= 0;
					if (flag2)
					{
						count++;
					}
					bool flag3 = earningsData.ShopSoldItemEarnList[i].First >= 0;
					if (flag3)
					{
						count++;
					}
				}
				result = count;
			}
			return result;
		}

		// Token: 0x04007A00 RID: 31232
		[SerializeField]
		private TextMeshProUGUI titleText;

		// Token: 0x04007A01 RID: 31233
		private IProductionHandler _handler;
	}
}
