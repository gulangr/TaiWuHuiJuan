using System;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.World;
using TMPro;
using UnityEngine;

namespace Game.Views.Bottom
{
	// Token: 0x02000C5B RID: 3163
	public class WorldStateDisplayItem : MonoBehaviour
	{
		// Token: 0x0600A147 RID: 41287 RVA: 0x004B5F4A File Offset: 0x004B414A
		private void Awake()
		{
			this.worldStateButton.onClick.ResetListener(new Action(this.OnClick));
			this.RefreshTemplateId();
		}

		// Token: 0x0600A148 RID: 41288 RVA: 0x004B5F74 File Offset: 0x004B4174
		private void RefreshTemplateId()
		{
			WorldStateItem cfg = WorldState.Instance[this.worldStateTemplateId];
			this.shortDesc.text = cfg.Name;
			this.worldState.SetSprite(cfg.Icon, false, null);
			this.hover.gameObject.SetActive(false);
		}

		// Token: 0x0600A149 RID: 41289 RVA: 0x004B5FCB File Offset: 0x004B41CB
		public void SetIndex(int templateId)
		{
			this.worldStateTemplateId = templateId;
			this.RefreshTemplateId();
		}

		// Token: 0x0600A14A RID: 41290 RVA: 0x004B5FDC File Offset: 0x004B41DC
		public void Set(WorldStateData worldStateData)
		{
			bool flag = worldStateData.GetWorldState((short)this.worldStateTemplateId);
			if (flag)
			{
				this.worldStateButton.interactable = string.IsNullOrEmpty(ViewWorldState.GetWorldStateJumpNotice(WorldState.Instance[this.worldStateTemplateId]));
				base.gameObject.SetActive(true);
				this.RefreshTemplateId();
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600A14B RID: 41291 RVA: 0x004B6047 File Offset: 0x004B4247
		public void OnClick()
		{
			ViewWorldState.TriggerWorldStateJumpInteract(WorldState.Instance[this.worldStateTemplateId], UIManager.Instance.UiCamera.WorldToScreenPoint(base.transform.position));
		}

		// Token: 0x04007D14 RID: 32020
		[SerializeField]
		private int worldStateTemplateId;

		// Token: 0x04007D15 RID: 32021
		[SerializeField]
		private CImage worldState;

		// Token: 0x04007D16 RID: 32022
		[SerializeField]
		private CImage hover;

		// Token: 0x04007D17 RID: 32023
		[SerializeField]
		private CButton worldStateButton;

		// Token: 0x04007D18 RID: 32024
		[SerializeField]
		private TMP_Text shortDesc;
	}
}
