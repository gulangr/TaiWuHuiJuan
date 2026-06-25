using System;
using Config;
using FrameWork;
using Game.Components.Common;
using Game.Views.MouseTips;
using GameData.Domains.Character.AvatarSystem;
using GameData.Domains.Character.Creation;
using GameData.Domains.Character.Display;
using UnityEngine;

namespace Game.Components.Character
{
	// Token: 0x02000F1D RID: 3869
	public class Charm : MonoBehaviour
	{
		// Token: 0x0600B243 RID: 45635 RVA: 0x005125B8 File Offset: 0x005107B8
		public void Set(CharacterDisplayData data, bool isShowBack = true)
		{
			bool flag = data == null;
			if (flag)
			{
				this.SetEmpty();
			}
			else
			{
				AvatarRelatedData avatarData = data.AvatarRelatedData;
				bool isFixedPresetType = CreatingType.IsFixedPresetType(data.CreatingType);
				bool? flag2;
				if (avatarData == null)
				{
					flag2 = null;
				}
				else
				{
					AvatarData avatarData2 = avatarData.AvatarData;
					flag2 = ((avatarData2 != null) ? new bool?(avatarData2.FaceVisible) : null);
				}
				bool faceVisible = flag2 ?? true;
				short clothDisplayId = (avatarData != null) ? avatarData.ClothingDisplayId : 0;
				this.Set(data.Charm, data.Gender, data.PhysiologicalAge, clothDisplayId, isFixedPresetType, faceVisible);
				this.propertyItem.SetShowBack(isShowBack);
			}
		}

		// Token: 0x0600B244 RID: 45636 RVA: 0x0051266C File Offset: 0x0051086C
		public void Set(short charm, sbyte gender, short age, short clothDisplayId, bool isFixedPresetType, bool faceVisible)
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				string charmText = CommonUtils.GetCharmLevelText(charm, gender, age, clothDisplayId, isFixedPresetType, faceVisible);
				int iconIndex = CommonUtils.GetCharmIconIndex(charm, age, clothDisplayId, faceVisible, isFixedPresetType);
				this.propertyItem.Set(this.sprites[iconIndex], LanguageKey.LK_Main_SummaryInfo_Charm.Tr(), charmText, null, false);
			}
			this.RefreshTip();
		}

		// Token: 0x0600B245 RID: 45637 RVA: 0x005126DC File Offset: 0x005108DC
		private void RefreshTip()
		{
			bool flag = this.mouseTip == null;
			if (!flag)
			{
				this.mouseTip.enabled = true;
				this.mouseTip.IsLanguageKey = false;
				this.mouseTip.Type = TipType.CommonTip;
				TooltipInvoker tooltipInvoker = this.mouseTip;
				if (tooltipInvoker.RuntimeParam == null)
				{
					tooltipInvoker.RuntimeParam = new ArgumentBox();
				}
				this.mouseTip.RuntimeParam.Clear();
				CommonTipSimpleRuntime runtime = CommonTip.DefValue.Charm.BuildSimple();
				this.mouseTip.RuntimeParam.SetObject("Runtime", runtime);
			}
		}

		// Token: 0x0600B246 RID: 45638 RVA: 0x00512778 File Offset: 0x00510978
		public void SetEmpty()
		{
			bool flag = this.propertyItem != null;
			if (flag)
			{
				this.propertyItem.Set(string.Empty, string.Empty, string.Empty, null, false);
			}
		}

		// Token: 0x04008A45 RID: 35397
		[Header("魅力组件")]
		[SerializeField]
		private PropertyItem propertyItem;

		// Token: 0x04008A46 RID: 35398
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04008A47 RID: 35399
		[Header("魅力大图标")]
		[SerializeField]
		private Sprite[] sprites;
	}
}
