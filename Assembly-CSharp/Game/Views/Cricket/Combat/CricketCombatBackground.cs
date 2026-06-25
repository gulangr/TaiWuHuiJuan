using System;
using FrameWork;
using GameData.Domains.Map;
using Spine.Unity;
using UnityEngine;

namespace Game.Views.Cricket.Combat
{
	// Token: 0x02000AC3 RID: 2755
	public class CricketCombatBackground : MonoBehaviour, ICricketCombatComponent
	{
		// Token: 0x17000EE6 RID: 3814
		// (get) Token: 0x060087C2 RID: 34754 RVA: 0x003F1C15 File Offset: 0x003EFE15
		// (set) Token: 0x060087C3 RID: 34755 RVA: 0x003F1C1D File Offset: 0x003EFE1D
		public ICricketCombatHandler Handler { get; set; }

		// Token: 0x060087C4 RID: 34756 RVA: 0x003F1C28 File Offset: 0x003EFE28
		public void OnEvent(ECricketCombatGlobalEventType type, ArgumentBox argBox)
		{
			bool flag = type > ECricketCombatGlobalEventType.Initialize;
			if (!flag)
			{
				for (int i = 0; i < this.effectRoot.childCount; i++)
				{
					Transform obj = this.effectRoot.GetChild(i);
					obj.gameObject.SetActive(false);
				}
				int backgroundType = (int)this.AnalysisType(argBox);
				this.background.SetTexture("tex_cricket_combat_bg_" + backgroundType.ToString());
				Transform backgroundEffect = this.effectRoot.GetChild(backgroundType);
				backgroundEffect.gameObject.SetActive(true);
				for (int j = 0; j < backgroundEffect.childCount; j++)
				{
					SkeletonGraphic skeleton;
					bool flag2 = backgroundEffect.GetChild(j).TryGetComponent<SkeletonGraphic>(out skeleton);
					if (flag2)
					{
						skeleton.AnimationState.SetAnimation(0, skeleton.startingAnimation, skeleton.startingLoop);
					}
				}
			}
		}

		// Token: 0x060087C5 RID: 34757 RVA: 0x003F1D08 File Offset: 0x003EFF08
		private CricketCombatBackground.ECricketCombatBackgroundType AnalysisType(ArgumentBox argBox)
		{
			bool isAutumnCricketMatch;
			bool flag = argBox.Get("IsAutumnCricketMatch", out isAutumnCricketMatch) && isAutumnCricketMatch;
			CricketCombatBackground.ECricketCombatBackgroundType result;
			if (flag)
			{
				result = CricketCombatBackground.ECricketCombatBackgroundType.Village;
			}
			else
			{
				WorldMapModel mapModel = SingletonObject.getInstance<WorldMapModel>();
				MapBlockData blockData = mapModel.PlayerAtBlock;
				EMapBlockType blockType = blockData.BlockType;
				if (!true)
				{
				}
				CricketCombatBackground.ECricketCombatBackgroundType ecricketCombatBackgroundType2;
				if (blockType != EMapBlockType.City)
				{
					if (blockType != EMapBlockType.Sect)
					{
						EMapBlockSubType blockSubType = blockData.BlockSubType;
						if (!true)
						{
						}
						CricketCombatBackground.ECricketCombatBackgroundType ecricketCombatBackgroundType;
						if (blockSubType != EMapBlockSubType.TaiwuCun)
						{
							switch (blockSubType)
							{
							case EMapBlockSubType.Village:
								break;
							case EMapBlockSubType.Town:
								ecricketCombatBackgroundType = CricketCombatBackground.ECricketCombatBackgroundType.Town;
								goto IL_8A;
							case EMapBlockSubType.WalledTown:
								ecricketCombatBackgroundType = CricketCombatBackground.ECricketCombatBackgroundType.WalledTown;
								goto IL_8A;
							default:
								ecricketCombatBackgroundType = CricketCombatBackground.ECricketCombatBackgroundType.Wild;
								goto IL_8A;
							}
						}
						ecricketCombatBackgroundType = CricketCombatBackground.ECricketCombatBackgroundType.Village;
						IL_8A:
						if (!true)
						{
						}
						ecricketCombatBackgroundType2 = ecricketCombatBackgroundType;
					}
					else
					{
						ecricketCombatBackgroundType2 = CricketCombatBackground.ECricketCombatBackgroundType.Sect;
					}
				}
				else
				{
					ecricketCombatBackgroundType2 = CricketCombatBackground.ECricketCombatBackgroundType.City;
				}
				if (!true)
				{
				}
				result = ecricketCombatBackgroundType2;
			}
			return result;
		}

		// Token: 0x0400682E RID: 26670
		[SerializeField]
		private CRawImage background;

		// Token: 0x0400682F RID: 26671
		[SerializeField]
		private RectTransform effectRoot;

		// Token: 0x0200209C RID: 8348
		private enum ECricketCombatBackgroundType
		{
			// Token: 0x0400D1A3 RID: 53667
			Wild,
			// Token: 0x0400D1A4 RID: 53668
			City,
			// Token: 0x0400D1A5 RID: 53669
			Sect,
			// Token: 0x0400D1A6 RID: 53670
			WalledTown,
			// Token: 0x0400D1A7 RID: 53671
			Town,
			// Token: 0x0400D1A8 RID: 53672
			Village,
			// Token: 0x0400D1A9 RID: 53673
			CricketConference = 5
		}
	}
}
