using System;
using System.IO;
using Config;
using FrameWork.UISystem.UIElements;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Game.Views.Building.BuildingManage
{
	// Token: 0x02000C0A RID: 3082
	public class ChickenItem : MonoBehaviour
	{
		// Token: 0x1700108E RID: 4238
		// (get) Token: 0x06009C88 RID: 40072 RVA: 0x00494D54 File Offset: 0x00492F54
		public CButton ChickenButton
		{
			get
			{
				return this.chickenButton;
			}
		}

		// Token: 0x06009C89 RID: 40073 RVA: 0x00494D5C File Offset: 0x00492F5C
		public void Set(ChickenData data, bool isSelected)
		{
			this.selected.gameObject.SetActive(isSelected);
			ChickenItem chickenItem = Chicken.Instance.GetItem(data.TemplateId);
			ResLoader.Load<Sprite>(Path.Combine("RemakeResources/Textures/Chicken", chickenItem.Display), delegate(Sprite sprite)
			{
				this.chickenIcon.sprite = sprite;
				this.chickenIcon.enabled = true;
			}, null, false);
			this.chickenName.text = data.Name;
			this.gradeFrame.color = Colors.Instance.GradeColors[(int)chickenItem.Grade];
			this.favor.text = data.Happiness.ToString();
			this.personalityIcon.SetSprite("ui9_icon_building_personality_big_" + chickenItem.PersonalityType.ToString(), false, null);
			TextMeshProUGUI textMeshProUGUI = this.personalityTitle;
			sbyte personalityType = chickenItem.PersonalityType;
			if (!true)
			{
			}
			string text;
			switch (personalityType)
			{
			case 0:
				text = LanguageKey.LK_Personality_Calm_Name.Tr();
				break;
			case 1:
				text = LanguageKey.LK_Personality_Clever_Name.Tr();
				break;
			case 2:
				text = LanguageKey.LK_Personality_Enthusiastic_Name.Tr();
				break;
			case 3:
				text = LanguageKey.LK_Personality_Brave_Name.Tr();
				break;
			case 4:
				text = LanguageKey.LK_Personality_Firm_Name.Tr();
				break;
			case 5:
				text = LanguageKey.LK_Personality_Lucky_Name.Tr();
				break;
			case 6:
				text = LanguageKey.LK_Personality_Perceptive_Name.Tr();
				break;
			default:
				if (!true)
				{
				}
				<PrivateImplementationDetails>.ThrowSwitchExpressionException(personalityType);
				break;
			}
			if (!true)
			{
			}
			textMeshProUGUI.text = text;
			this.personalityValue.text = chickenItem.PersonalityValue.ToString();
			this.favorIcon.SetSprite("ui9_icon_favor_" + ChickenItem.GetFavorIconIndex((int)data.Happiness).ToString(), false, null);
			bool flag = this.skeleton;
			if (flag)
			{
				this.skeleton.Skeleton.SetAttachment("side_body", string.Format("chicken_{0}_side", chickenItem.ChickenColor.ToInt()));
			}
		}

		// Token: 0x06009C8A RID: 40074 RVA: 0x00494F58 File Offset: 0x00493158
		public static int GetFavorIconIndex(int happiness)
		{
			if (!true)
			{
			}
			int result;
			if (happiness >= 36)
			{
				if (happiness >= 96)
				{
					if (happiness <= 100)
					{
						result = 8;
						goto IL_5E;
					}
				}
				else if (happiness >= 66)
				{
					if (happiness < 81)
					{
						result = 6;
						goto IL_5E;
					}
					result = 7;
					goto IL_5E;
				}
				else
				{
					if (happiness < 51)
					{
						result = 4;
						goto IL_5E;
					}
					result = 5;
					goto IL_5E;
				}
			}
			else if (happiness >= 6)
			{
				if (happiness < 21)
				{
					result = 2;
					goto IL_5E;
				}
				result = 3;
				goto IL_5E;
			}
			else if (happiness >= 0)
			{
				result = 0;
				goto IL_5E;
			}
			result = 0;
			IL_5E:
			if (!true)
			{
			}
			return result;
		}

		// Token: 0x04007951 RID: 31057
		[SerializeField]
		private CImage chickenIcon;

		// Token: 0x04007952 RID: 31058
		[SerializeField]
		private CImage gradeFrame;

		// Token: 0x04007953 RID: 31059
		[SerializeField]
		private CImage favorIcon;

		// Token: 0x04007954 RID: 31060
		[SerializeField]
		private TextMeshProUGUI favor;

		// Token: 0x04007955 RID: 31061
		[SerializeField]
		private TextMeshProUGUI chickenName;

		// Token: 0x04007956 RID: 31062
		[SerializeField]
		private CImage personalityIcon;

		// Token: 0x04007957 RID: 31063
		[SerializeField]
		private TextMeshProUGUI personalityTitle;

		// Token: 0x04007958 RID: 31064
		[SerializeField]
		private TextMeshProUGUI personalityValue;

		// Token: 0x04007959 RID: 31065
		[SerializeField]
		private CImage selected;

		// Token: 0x0400795A RID: 31066
		[SerializeField]
		private CButton chickenButton;

		// Token: 0x0400795B RID: 31067
		[SerializeField]
		private SkeletonGraphic skeleton;
	}
}
