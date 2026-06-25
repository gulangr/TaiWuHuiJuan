using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using GameData.DLC.FiveLoong;
using GameData.Domains.Character.Display;
using Spine.Unity;
using UnityEngine;

namespace Game.Views.MapBlockCharList
{
	// Token: 0x02000937 RID: 2359
	public class MapBlockCharBg : MonoBehaviour
	{
		// Token: 0x06006E0B RID: 28171 RVA: 0x0032D460 File Offset: 0x0032B660
		public void SetBg(CharacterDisplayData display = null, List<LoongInfo> loongInfos = null)
		{
			bool flag = display == null;
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				base.gameObject.SetActive(true);
				this.darkAshBg.SetActive((display.DarkAshProtector & 512U) > 0U);
				bool flag2 = display.DarkAshProtector == 512U;
				if (flag2)
				{
					this.darkAshEffect.Play();
				}
				else
				{
					this.darkAshEffect.Play();
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(1U, delegate
					{
						UIParticle uiparticle = this.darkAshEffect;
						if (uiparticle != null)
						{
							uiparticle.Pause();
						}
					});
				}
				bool hasLoongDebuff = false;
				bool flag3 = (display.DarkAshProtector & 512U) == 0U && loongInfos != null;
				if (flag3)
				{
					Action<SkeletonDataAsset> <>9__1;
					foreach (LoongInfo loongInfo in loongInfos)
					{
						bool flag4 = !loongInfo.IsDisappear && loongInfo.GetCharacterDebuffCount(display.CharacterId) > 0;
						if (flag4)
						{
							string assetPath = "Dlc/FiveLoong/RemakeResources/Particle/UIEffectPrefabs/MapBlockCharacter/Fiveloong_SkeletonData";
							Action<SkeletonDataAsset> onLoad;
							if ((onLoad = <>9__1) == null)
							{
								onLoad = (<>9__1 = delegate(SkeletonDataAsset dataAsset)
								{
									this.skeletonGraphic.skeletonDataAsset = dataAsset;
									this.skeletonGraphic.Initialize(true);
									this.skeletonGraphic.initialSkinName = "basic";
									this.skeletonGraphic.AnimationState.SetAnimation(0, "animation", true);
									this.skeletonGraphic.gameObject.SetActive(true);
									CommonUtils.SetLoongDebuff(loongInfos, display, this.mouseTipDisplay, this.circleGenerator);
								});
							}
							ResLoader.Load<SkeletonDataAsset>(assetPath, onLoad, null, false);
							hasLoongDebuff = true;
							break;
						}
					}
				}
				bool flag5 = hasLoongDebuff;
				if (!flag5)
				{
					this.skeletonGraphic.gameObject.SetActive(false);
					this.circleGenerator.ClearExistGO();
				}
			}
		}

		// Token: 0x040051A2 RID: 20898
		[SerializeField]
		private GameObject darkAshBg;

		// Token: 0x040051A3 RID: 20899
		[SerializeField]
		private UIParticle darkAshEffect;

		// Token: 0x040051A4 RID: 20900
		[SerializeField]
		protected TooltipInvoker mouseTipDisplay;

		// Token: 0x040051A5 RID: 20901
		[SerializeField]
		protected CircleGenerator circleGenerator;

		// Token: 0x040051A6 RID: 20902
		[SerializeField]
		protected SkeletonGraphic skeletonGraphic;
	}
}
