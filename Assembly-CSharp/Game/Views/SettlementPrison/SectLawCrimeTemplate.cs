using System;
using Config;
using FrameWork.UISystem.UIElements;
using GameData.Domains.Organization.Display;
using TMPro;
using UnityEngine;

namespace Game.Views.SettlementPrison
{
	// Token: 0x02000787 RID: 1927
	public class SectLawCrimeTemplate : MonoBehaviour
	{
		// Token: 0x17000B50 RID: 2896
		// (get) Token: 0x06005D4C RID: 23884 RVA: 0x002AE7D8 File Offset: 0x002AC9D8
		public CDropdown Dropdown
		{
			get
			{
				return this.dropdown;
			}
		}

		// Token: 0x06005D4D RID: 23885 RVA: 0x002AE7E0 File Offset: 0x002AC9E0
		public void Init(short punishmentTypeId, short punishmentSeverityId, bool isCustom, PunishmentSeverityCustomizeData customizeData, short originalSeverityId)
		{
			this._punishmentTypeId = punishmentTypeId;
			this._punishmentSeverityId = punishmentSeverityId;
			this._customizeData = customizeData;
			this._originalSeverityId = originalSeverityId;
			this.dropdown.gameObject.SetActive(isCustom);
			PunishmentTypeItem typeConfig = PunishmentType.Instance[this._punishmentTypeId];
			ResLoader.Load<Texture2D>("RemakeResources/Textures/UITexturesRemake/SectLaw/" + typeConfig.Image, delegate(Texture2D texture2D)
			{
				this.crimeImage.texture = texture2D;
			}, null, false);
			this.crimeName.text = typeConfig.ShortName;
			this.crimeDesc.text = typeConfig.PunishmentDesc;
			this.SetPunishmentSeverity(this._punishmentSeverityId);
			this.SetChangeDuration(this._punishmentSeverityId);
		}

		// Token: 0x06005D4E RID: 23886 RVA: 0x002AE894 File Offset: 0x002ACA94
		public void SetPunishmentSeverity(short punishmentSeverityId)
		{
			PunishmentSeverityItem severityConfig = PunishmentSeverity.Instance[(int)punishmentSeverityId];
			this.punishmentName.text = severityConfig.Name.SetColor(severityConfig.NameColor);
			this.punishmentDesc.text = severityConfig.PunishmentDesc;
			this.punishmentDescTip.PresetParam = new string[]
			{
				severityConfig.PunishmentDesc
			};
			this.punishmentMonthIcon.SetActive(severityConfig.PrisonTime != 0);
			this.punishmentDuration.text = ((severityConfig.PrisonTime == 0) ? "/" : severityConfig.PrisonTime.ToString());
		}

		// Token: 0x06005D4F RID: 23887 RVA: 0x002AE934 File Offset: 0x002ACB34
		public void SetChangeDuration(short tempSeverityId)
		{
			bool isChange = this._originalSeverityId != tempSeverityId;
			this.changedDurationRoot.SetActive(isChange);
			int remainDate = (int)(isChange ? GlobalConfig.Instance.TownPunishmentSeverityCustomizeDuration : 0);
			bool flag = this._customizeData != null;
			if (flag)
			{
				bool flag2 = this._originalSeverityId == this._punishmentSeverityId;
				if (flag2)
				{
					remainDate = 0;
				}
				else
				{
					remainDate = (int)GlobalConfig.Instance.TownPunishmentSeverityCustomizeDuration - (SingletonObject.getInstance<BasicGameData>().CurrDate - this._customizeData.ModifyDate);
				}
				this.largeDiffBg.gameObject.SetActive(Math.Abs((int)(tempSeverityId - this._punishmentSeverityId)) > (int)GlobalConfig.Instance.ModifySeverityDefaultRange);
			}
			else
			{
				this.largeDiffBg.gameObject.SetActive(false);
			}
			this.crimeDuration.text = ((remainDate <= 0) ? "/" : remainDate.ToString());
			this.crimeMonthIcon.SetActive(remainDate > 0);
			this.SetPunishmentSeverity(tempSeverityId);
		}

		// Token: 0x0400400F RID: 16399
		[SerializeField]
		private GameObject largeDiffBg;

		// Token: 0x04004010 RID: 16400
		[SerializeField]
		private CRawImage crimeImage;

		// Token: 0x04004011 RID: 16401
		[SerializeField]
		private TextMeshProUGUI crimeName;

		// Token: 0x04004012 RID: 16402
		[SerializeField]
		private TextMeshProUGUI crimeDesc;

		// Token: 0x04004013 RID: 16403
		[SerializeField]
		private TextMeshProUGUI punishmentName;

		// Token: 0x04004014 RID: 16404
		[SerializeField]
		private TextMeshProUGUI punishmentDesc;

		// Token: 0x04004015 RID: 16405
		[SerializeField]
		private TooltipInvoker punishmentDescTip;

		// Token: 0x04004016 RID: 16406
		[SerializeField]
		private GameObject punishmentMonthIcon;

		// Token: 0x04004017 RID: 16407
		[SerializeField]
		private TextMeshProUGUI punishmentDuration;

		// Token: 0x04004018 RID: 16408
		[SerializeField]
		private GameObject changedDurationRoot;

		// Token: 0x04004019 RID: 16409
		[SerializeField]
		private GameObject crimeMonthIcon;

		// Token: 0x0400401A RID: 16410
		[SerializeField]
		private TextMeshProUGUI crimeDuration;

		// Token: 0x0400401B RID: 16411
		[SerializeField]
		private CDropdown dropdown;

		// Token: 0x0400401C RID: 16412
		[SerializeField]
		private Sprite[] punishmentSeverityIconSprites;

		// Token: 0x0400401D RID: 16413
		[SerializeField]
		private Sprite[] punishmentSeverityTextSprites;

		// Token: 0x0400401E RID: 16414
		private short _punishmentTypeId;

		// Token: 0x0400401F RID: 16415
		private short _punishmentSeverityId;

		// Token: 0x04004020 RID: 16416
		private PunishmentSeverityCustomizeData _customizeData;

		// Token: 0x04004021 RID: 16417
		private short _originalSeverityId;

		// Token: 0x04004022 RID: 16418
		private const string SectLawPath = "RemakeResources/Textures/UITexturesRemake/SectLaw/";
	}
}
