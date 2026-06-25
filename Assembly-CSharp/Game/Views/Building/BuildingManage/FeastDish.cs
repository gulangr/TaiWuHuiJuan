using System;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Item;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C0C RID: 3084
	public class FeastDish : MonoBehaviour
	{
		// Token: 0x06009C8D RID: 40077 RVA: 0x00494FF4 File Offset: 0x004931F4
		public void Init(Action onClick)
		{
			PointerTrigger pointerTrigger = base.GetComponent<PointerTrigger>();
			pointerTrigger.EnterEvent.RemoveAllListeners();
			pointerTrigger.ExitEvent.RemoveAllListeners();
			pointerTrigger.EnterEvent.AddListener(delegate()
			{
				this.hover.SetActive(true);
			});
			pointerTrigger.ExitEvent.AddListener(delegate()
			{
				this.hover.SetActive(false);
			});
			base.GetComponent<CButton>().ClearAndAddListener(onClick);
		}

		// Token: 0x06009C8E RID: 40078 RVA: 0x00495060 File Offset: 0x00493260
		public void Set(ItemKey itemKey, int durability)
		{
			bool isValid = itemKey.IsValid();
			bool flag = isValid;
			if (flag)
			{
				this.itemIcon.SetSprite((itemKey.ItemType == 7) ? Food.Instance[itemKey.TemplateId].BigIcon : TeaWine.Instance[itemKey.TemplateId].BigIcon, false, null);
				this.nameLabel.text = ItemTemplateHelper.GetName(itemKey.ItemType, itemKey.TemplateId).SetGradeColor((int)ItemTemplateHelper.GetGrade(itemKey.ItemType, itemKey.TemplateId));
				this.countLabel.text = LanguageKey.LK_Building_Entertain_RemainCount.TrFormat(durability, GlobalConfig.Instance.FeastDurability);
				this.hoverIcon.sprite = this.changeSprite;
			}
			else
			{
				this.itemIcon.sprite = this.emptySprite;
				this.hoverIcon.sprite = this.addSprite;
			}
			this.textBack.SetActive(isValid);
			this.particle.SetActive(isValid);
		}

		// Token: 0x04007960 RID: 31072
		public GameObject particle;

		// Token: 0x04007961 RID: 31073
		public CImage itemIcon;

		// Token: 0x04007962 RID: 31074
		public GameObject hover;

		// Token: 0x04007963 RID: 31075
		public CImage hoverIcon;

		// Token: 0x04007964 RID: 31076
		public GameObject textBack;

		// Token: 0x04007965 RID: 31077
		public TextMeshProUGUI nameLabel;

		// Token: 0x04007966 RID: 31078
		public TextMeshProUGUI countLabel;

		// Token: 0x04007967 RID: 31079
		[Header("Sprite")]
		[SerializeField]
		private Sprite emptySprite;

		// Token: 0x04007968 RID: 31080
		[SerializeField]
		private Sprite addSprite;

		// Token: 0x04007969 RID: 31081
		[SerializeField]
		private Sprite changeSprite;
	}
}
