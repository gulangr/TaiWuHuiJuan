using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FrameWork.UISystem.UIElements;
using GameData.Domains.World;
using TMPro;
using UnityEngine;

namespace Game.Views.NewGame
{
	// Token: 0x02000811 RID: 2065
	public class NewGameSubPageWorldDetailDifficultyLevelItem2 : MonoBehaviour
	{
		// Token: 0x06006567 RID: 25959 RVA: 0x002E5960 File Offset: 0x002E3B60
		public void Init(int index, bool isLocked, bool isSelection, Action onClickButtonSetting = null, bool isAnim = false)
		{
			this.textTitle.text = NewGameSubPageWorldDetail.DifficultyLevelKeys[index].Tr().ColorReplace();
			LanguageKey descKey = LanguageKey.LK_NewGame_DifficultyLevel_Desc_1 + index;
			this.textDesc.text = descKey.Tr().ColorReplace();
			if (isAnim)
			{
				this.PlayImageAnim(index);
			}
			else
			{
				this.image.SetTexture("ui9_tex_world_detail_difficulty_" + index.ToString());
			}
			bool isCustom = index == WorldCreationInfo.EDifficultyLevel.Custom.ToInt();
			this.toggle.interactable = (!isCustom && isSelection);
			bool showButtonSetting = isCustom && isSelection;
			this.buttonSetting.gameObject.SetActive(showButtonSetting);
			this.buttonSetting.RemoveAllListeners();
			bool flag = showButtonSetting && onClickButtonSetting != null;
			if (flag)
			{
				this.buttonSetting.ClearAndAddListener(onClickButtonSetting);
			}
			this.textDesc.gameObject.SetActive(!isCustom && isSelection);
			this.lockedObj.gameObject.SetActive(isLocked);
			this.lockedTip.enabled = isLocked;
		}

		// Token: 0x06006568 RID: 25960 RVA: 0x002E5A70 File Offset: 0x002E3C70
		private void PlayImageAnim(int index)
		{
			this.image.DOKill(true);
			this.image.color = Color.white;
			this.image.DOFade(0f, 0.3f).OnComplete(delegate
			{
				this.image.SetTexture("ui9_tex_world_detail_difficulty_" + index.ToString());
				this.image.DOFade(1f, 0.3f);
			});
		}

		// Token: 0x040046A6 RID: 18086
		[SerializeField]
		private TextMeshProUGUI textTitle;

		// Token: 0x040046A7 RID: 18087
		[SerializeField]
		private TextMeshProUGUI textDesc;

		// Token: 0x040046A8 RID: 18088
		[SerializeField]
		private CToggle toggle;

		// Token: 0x040046A9 RID: 18089
		[SerializeField]
		private CRawImage image;

		// Token: 0x040046AA RID: 18090
		[SerializeField]
		private CButton buttonSetting;

		// Token: 0x040046AB RID: 18091
		[SerializeField]
		private GameObject lockedObj;

		// Token: 0x040046AC RID: 18092
		[SerializeField]
		private TooltipInvoker lockedTip;
	}
}
