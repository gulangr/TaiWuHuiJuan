using System;
using Config;
using FrameWork.UISystem.UIElements;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C0D RID: 3085
	public class FeastMenuItem : MonoBehaviour
	{
		// Token: 0x06009C92 RID: 40082 RVA: 0x0049519C File Offset: 0x0049339C
		public void Set(short templateId, bool isUnlocked)
		{
			FeastItem config = Feast.Instance[templateId];
			string lockedText = LocalStringManager.Get(LanguageKey.LK_ThreeQuestionMark);
			base.GetComponent<CToggle>().interactable = isUnlocked;
			this.disabled.SetActive(!isUnlocked);
			this.icon.sprite = (isUnlocked ? this.unlocked : this.locked);
			this.titleLabel.text = (isUnlocked ? config.Name.ColorReplace() : lockedText);
			this.conditionDesc.text = (isUnlocked ? config.ConditionDesc.ColorReplace() : lockedText);
			this.effectDesc.text = (isUnlocked ? config.EffectDesc.ColorReplace() : lockedText);
		}

		// Token: 0x0400796A RID: 31082
		public GameObject disabled;

		// Token: 0x0400796B RID: 31083
		public CImage icon;

		// Token: 0x0400796C RID: 31084
		public TextMeshProUGUI titleLabel;

		// Token: 0x0400796D RID: 31085
		public TextMeshProUGUI conditionDesc;

		// Token: 0x0400796E RID: 31086
		public TextMeshProUGUI effectDesc;

		// Token: 0x0400796F RID: 31087
		[Header("Sprite")]
		[SerializeField]
		private Sprite locked;

		// Token: 0x04007970 RID: 31088
		[SerializeField]
		private Sprite unlocked;
	}
}
