using System;
using System.Collections.Generic;
using GameData.Domains.Building;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

namespace Game.Views.Building.BuildingAreaQuickActionMenu.LeftButton
{
	// Token: 0x02000C29 RID: 3113
	public class QuickRemoveProcessor : LeftButtonProcessor
	{
		// Token: 0x06009E47 RID: 40519 RVA: 0x004A0C46 File Offset: 0x0049EE46
		public QuickRemoveProcessor(ViewBuildingQuickActionMenu menu, GameObject buttonObject, LeftButtonType type) : base(menu, buttonObject, type)
		{
		}

		// Token: 0x06009E48 RID: 40520 RVA: 0x004A0C53 File Offset: 0x0049EE53
		public override void PrepareData()
		{
			this._menu.FetchAvailableWorkers(delegate(List<int> availableWorkers)
			{
				this._menu.PrepareWorkerRelatedDataAsync(delegate
				{
					this.OnAvailableWorkersReady();
				});
			});
		}

		// Token: 0x06009E49 RID: 40521 RVA: 0x004A0C6E File Offset: 0x0049EE6E
		private void OnAvailableWorkersReady()
		{
			base.UpdateVisibility();
			base.UpdateInteractivity();
		}

		// Token: 0x06009E4A RID: 40522 RVA: 0x004A0C80 File Offset: 0x0049EE80
		public override bool IsVisible()
		{
			return false;
		}

		// Token: 0x06009E4B RID: 40523 RVA: 0x004A0C94 File Offset: 0x0049EE94
		public override bool CanInteract()
		{
			return this.IsVisible() && this._menu.HasAvailableWorkerForExpandRemove();
		}

		// Token: 0x06009E4C RID: 40524 RVA: 0x004A0CBC File Offset: 0x0049EEBC
		public override void OnClick()
		{
			bool flag = !this.CanInteract();
			if (!flag)
			{
				BuildingBlockData blockData = this._menu.BlockData;
				bool flag2 = blockData.OperationType == -1;
				if (flag2)
				{
					AsyncMethodCallbackDelegate <>9__3;
					BuildingDomainMethod.AsyncCall.QuickArrangeBuildOperator(this._menu, this._menu.ConfigData.TemplateId, this._menu.BlockKey, 1, delegate(int offset, RawDataPool dataPool)
					{
						List<int> charIdList = new List<int>();
						Serializer.Deserialize(dataPool, offset, ref charIdList);
						blockData.OperationType = 1;
						IAsyncMethodRequestHandler menu = this._menu;
						BuildingBlockKey blockKey = this._menu.BlockKey;
						int[] workers = charIdList.ToArray();
						AsyncMethodCallbackDelegate callback;
						if ((callback = <>9__3) == null)
						{
							callback = (<>9__3 = delegate(int offset, RawDataPool pool)
							{
								ValueTuple<short, BuildingBlockData> retValue = new ValueTuple<short, BuildingBlockData>(0, new BuildingBlockData());
								Serializer.Deserialize(pool, offset, ref retValue);
								bool flag4 = retValue.Item1 == this._menu.BlockKey.BuildingBlockIndex;
								if (flag4)
								{
									this._menu.BlockData = retValue.Item2;
								}
								base.<OnClick>g__OnComplete|2();
							});
						}
						BuildingDomainMethod.AsyncCall.Remove(menu, blockKey, workers, callback);
					});
				}
				else
				{
					bool flag3 = blockData.OperationType == 1;
					if (flag3)
					{
						BuildingDomainMethod.AsyncCall.SetStopOperation(this._menu, this._menu.BlockKey, true, delegate(int offset, RawDataPool pool)
						{
							ValueTuple<short, BuildingBlockData> retValue = new ValueTuple<short, BuildingBlockData>(0, new BuildingBlockData());
							Serializer.Deserialize(pool, offset, ref retValue);
							bool flag4 = retValue.Item1 == this._menu.BlockKey.BuildingBlockIndex;
							if (flag4)
							{
								this._menu.BlockData = retValue.Item2;
							}
							base.<OnClick>g__OnComplete|2();
						});
					}
				}
			}
		}

		// Token: 0x06009E4D RID: 40525 RVA: 0x004A0D80 File Offset: 0x0049EF80
		public override void OnHoverEnter()
		{
			string content = this.CanInteract() ? LanguageKey.LK_Building_QuickAction_Remove_Content_Normal.Tr() : LanguageKey.LK_Building_QuickAction_Remove_Content_Disable.Tr();
			string title = LanguageKey.LK_Building_QuickAction_Remove_Title.Tr();
			this._tip.PresetParam = new string[]
			{
				title,
				content
			};
		}
	}
}
