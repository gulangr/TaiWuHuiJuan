using System;
using Game.Components.Avatar;
using GameData.Domains.Character.Relation;
using GameData.Serializer;
using GameData.Utilities;
using Spine.Unity;
using UnityEngine;

// Token: 0x020003C4 RID: 964
public abstract class MapBlockCharAlive : MapBlockCharBase
{
	// Token: 0x06003A41 RID: 14913 RVA: 0x001DA504 File Offset: 0x001D8704
	protected virtual void RefreshAvatar()
	{
		bool flag = this.DisplayData == null;
		if (flag)
		{
			this.avatar.ResetToBlank(false);
		}
		else
		{
			this.avatar.Refresh(this.DisplayData, true);
		}
		this.avatar.gameObject.SetActive(true);
		this.avatarImage.gameObject.SetActive(false);
	}

	// Token: 0x06003A42 RID: 14914
	protected abstract void RefreshIcon();

	// Token: 0x06003A43 RID: 14915 RVA: 0x001DA565 File Offset: 0x001D8765
	protected override void Refresh()
	{
		base.Refresh();
		this.RefreshAvatar();
		this.RefreshIcon();
		this.RefreshRelation();
	}

	// Token: 0x06003A44 RID: 14916 RVA: 0x001DA584 File Offset: 0x001D8784
	private void RefreshRelation()
	{
		bool flag = this.CharId <= 0;
		if (flag)
		{
			bool flag2 = this.loveObj;
			if (flag2)
			{
				this.loveObj.SetActive(false);
			}
			bool flag3 = this.hateObj;
			if (flag3)
			{
				this.hateObj.SetActive(false);
			}
		}
		else
		{
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			SingletonObject.getInstance<AsyncMethodDispatcher>().AsyncMethodCall<int, int>(4, 129, taiwuCharId, this.CharId, delegate(int offset, RawDataPool pool)
			{
				bool flag4 = this.CharId <= 0;
				if (!flag4)
				{
					ValueTuple<ushort, ushort> result = default(ValueTuple<ushort, ushort>);
					Serializer.Deserialize(pool, offset, ref result);
					bool hasAdoredRelationship = result.Item1 != ushort.MaxValue && RelationType.HasRelation(result.Item1, 16384);
					bool hasEnemyRelationship = result.Item1 != ushort.MaxValue && RelationType.HasRelation(result.Item1, 32768);
					bool flag5 = this.loveObj;
					if (flag5)
					{
						this.loveObj.SetActive(hasAdoredRelationship);
					}
					bool flag6 = this.hateObj;
					if (flag6)
					{
						this.hateObj.SetActive(hasEnemyRelationship);
					}
				}
			});
		}
	}

	// Token: 0x06003A45 RID: 14917 RVA: 0x001DA60C File Offset: 0x001D880C
	protected string GetMerchantLevelImage(sbyte level)
	{
		return string.Format("blockchar_shoplevel_{0}", level);
	}

	// Token: 0x040029FF RID: 10751
	[SerializeField]
	protected Game.Components.Avatar.Avatar avatar;

	// Token: 0x04002A00 RID: 10752
	[SerializeField]
	protected CImage avatarImage;

	// Token: 0x04002A01 RID: 10753
	[SerializeField]
	protected CImage iconImage;

	// Token: 0x04002A02 RID: 10754
	[SerializeField]
	protected GameObject loveObj;

	// Token: 0x04002A03 RID: 10755
	[SerializeField]
	protected GameObject hateObj;

	// Token: 0x04002A04 RID: 10756
	[SerializeField]
	protected TooltipInvoker mouseTipDisplay;

	// Token: 0x04002A05 RID: 10757
	[SerializeField]
	protected CircleGenerator circleGenerator;

	// Token: 0x04002A06 RID: 10758
	[SerializeField]
	protected SkeletonGraphic skeletonGraphic;
}
