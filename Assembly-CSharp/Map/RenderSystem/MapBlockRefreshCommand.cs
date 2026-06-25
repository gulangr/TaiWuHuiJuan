using System;
using System.Collections.Generic;
using UnityEngine;

namespace Map.RenderSystem
{
	// Token: 0x020006CD RID: 1741
	public class MapBlockRefreshCommand
	{
		// Token: 0x060052DC RID: 21212 RVA: 0x0026386C File Offset: 0x00261A6C
		public void ApplyStateChangeProgress(float progress, int controlCode)
		{
			bool flag = this.Type != MapBlockRefreshCommand.EMapBlockRefreshType.ChangeBlockState;
			if (!flag)
			{
				bool flag2 = this.RelatedRenderInfoList[0].ControlCode != controlCode;
				if (!flag2)
				{
					bool flag3 = this.ToState == MapBlockRenderInfo.EMapBlockState.Invisible;
					if (flag3)
					{
						this.RelatedRenderInfoList[0].ApplyAnimBlockHide(progress, this.FromColor);
					}
					else
					{
						bool flag4 = this.FromState == MapBlockRenderInfo.EMapBlockState.NegativeFilm || this.ToState == MapBlockRenderInfo.EMapBlockState.NegativeFilm;
						if (flag4)
						{
							this.RelatedRenderInfoList[0].ApplyAnimBlockNegativeFilm(progress);
						}
						else
						{
							bool flag5 = this.FromState == MapBlockRenderInfo.EMapBlockState.DarkVisible || this.ToState == MapBlockRenderInfo.EMapBlockState.DarkVisible;
							if (flag5)
							{
								this.RelatedRenderInfoList[0].ApplyAnimBlockDarkVisible(progress);
							}
							else
							{
								bool flag6 = this.FromState == MapBlockRenderInfo.EMapBlockState.Invisible && MapBlockRenderInfo.IsColoredState(this.ToState);
								if (flag6)
								{
									this.RelatedRenderInfoList[0].ApplyAnimBlockAppear(progress, this.FromVertexPositions);
								}
								else
								{
									bool flag7 = MapBlockRenderInfo.IsColoredState(this.FromState) && MapBlockRenderInfo.IsColoredState(this.ToState);
									if (flag7)
									{
										this.RelatedRenderInfoList[0].ApplyAnimBlockColorChanged(progress, this.FromColor, MapBlockRenderInfo.GetStateColor(this.ToState));
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060052DD RID: 21213 RVA: 0x002639B8 File Offset: 0x00261BB8
		public static MapBlockRefreshCommand CreateSetUpSpriteRefreshCommand()
		{
			return new MapBlockRefreshCommand
			{
				Type = MapBlockRefreshCommand.EMapBlockRefreshType.SetUpSprite,
				RelatedRenderInfoList = new List<MapBlockRenderInfo>(),
				Complete = false
			};
		}

		// Token: 0x060052DE RID: 21214 RVA: 0x002639EC File Offset: 0x00261BEC
		public static MapBlockRefreshCommand CreateSetUpEffectRefreshCommand()
		{
			return new MapBlockRefreshCommand
			{
				Type = MapBlockRefreshCommand.EMapBlockRefreshType.SetUpEffect,
				RelatedRenderInfoList = new List<MapBlockRenderInfo>(),
				Complete = false
			};
		}

		// Token: 0x060052DF RID: 21215 RVA: 0x00263A20 File Offset: 0x00261C20
		public static MapBlockRefreshCommand CreateChangeBlockStateRefreshCommand()
		{
			return new MapBlockRefreshCommand
			{
				Type = MapBlockRefreshCommand.EMapBlockRefreshType.ChangeBlockState,
				RelatedRenderInfoList = new List<MapBlockRenderInfo>(),
				Complete = false
			};
		}

		// Token: 0x04003819 RID: 14361
		public MapBlockRefreshCommand.EMapBlockRefreshType Type;

		// Token: 0x0400381A RID: 14362
		public List<MapBlockRenderInfo> RelatedRenderInfoList;

		// Token: 0x0400381B RID: 14363
		public bool Complete;

		// Token: 0x0400381C RID: 14364
		public string SpriteName;

		// Token: 0x0400381D RID: 14365
		public string EffectName;

		// Token: 0x0400381E RID: 14366
		public MapBlockRenderInfo.EMapBlockState FromState;

		// Token: 0x0400381F RID: 14367
		public MapBlockRenderInfo.EMapBlockState ToState;

		// Token: 0x04003820 RID: 14368
		public Color32 FromColor;

		// Token: 0x04003821 RID: 14369
		public Vector3[] FromVertexPositions;

		// Token: 0x02001B08 RID: 6920
		public enum EMapBlockRefreshType
		{
			// Token: 0x0400B7CD RID: 47053
			SetUpSprite,
			// Token: 0x0400B7CE RID: 47054
			SetUpEffect,
			// Token: 0x0400B7CF RID: 47055
			ChangeBlockState
		}
	}
}
