using System;
using TMPro;
using UnityEngine;

// Token: 0x020003C0 RID: 960
public class BlockPromptCount : MonoBehaviour
{
	// Token: 0x06003A1C RID: 14876 RVA: 0x001D975C File Offset: 0x001D795C
	public bool RefreshWithActive(int professionId, int index, int count)
	{
		bool hasCount = count > 0 && BlockPromptCount.ParseEnabled(professionId, index);
		base.gameObject.SetActive(hasCount);
		bool flag = !hasCount;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			string sprite = BlockPromptCount.ParseIconSprite(professionId, index);
			this.Refresh(sprite, count);
			result = true;
		}
		return result;
	}

	// Token: 0x06003A1D RID: 14877 RVA: 0x001D97A8 File Offset: 0x001D79A8
	public void Refresh(string sprite, int count)
	{
		this.imgIcon.SetSprite(sprite, false, null);
		this.txtMeshCount.text = count.ToString();
	}

	// Token: 0x06003A1E RID: 14878 RVA: 0x001D97D0 File Offset: 0x001D79D0
	public static bool ParseEnabled(int professionId, int index)
	{
		GlobalSettings globalSettings = SingletonObject.getInstance<GlobalSettings>();
		if (!true)
		{
		}
		bool result;
		if (professionId <= 1)
		{
			if (professionId == 0)
			{
				if (!true)
				{
				}
				bool flag = index == 0 && globalSettings.ShowSavageSkillDestroyedBlock;
				if (!true)
				{
				}
				result = flag;
				goto IL_16C;
			}
			if (professionId == 1)
			{
				if (!true)
				{
				}
				bool flag = index == 0 && globalSettings.ShowHunterSkillAnimal;
				if (!true)
				{
				}
				result = flag;
				goto IL_16C;
			}
		}
		else
		{
			if (professionId == 5)
			{
				if (!true)
				{
				}
				bool flag;
				if (index != 0)
				{
					flag = (index == 1 && globalSettings.ShowTaoistMonkSkillCompletelyInfected);
				}
				else
				{
					flag = globalSettings.ShowTaoistMonkSkillPartlyInfected;
				}
				if (!true)
				{
				}
				result = flag;
				goto IL_16C;
			}
			switch (professionId)
			{
			case 10:
			{
				if (!true)
				{
				}
				bool flag = index == 0 && globalSettings.ShowCivilianSkillPersonalEnemy;
				if (!true)
				{
				}
				result = flag;
				goto IL_16C;
			}
			case 11:
				break;
			case 12:
			{
				if (!true)
				{
				}
				bool flag;
				if (index != 0)
				{
					flag = (index == 1 && globalSettings.ShowTravelingBuddhistMonkSkillRebelAndEgoisticPeople);
				}
				else
				{
					flag = globalSettings.ShowTravelingBuddhistMonkSkillRebelAndEgoisticPeople;
				}
				if (!true)
				{
				}
				result = flag;
				goto IL_16C;
			}
			case 13:
			{
				if (!true)
				{
				}
				bool flag;
				switch (index)
				{
				case 0:
					flag = globalSettings.ShowDoctorSkillHurtPeople;
					break;
				case 1:
					flag = globalSettings.ShowDoctorSkillPoisonedPeople;
					break;
				case 2:
					flag = globalSettings.ShowDoctorSkillDisorderPeople;
					break;
				default:
					flag = false;
					break;
				}
				if (!true)
				{
				}
				result = flag;
				goto IL_16C;
			}
			default:
				if (professionId == 17)
				{
					if (!true)
					{
					}
					bool flag = index == 0 && globalSettings.ShowDukeSkillGiveTitle;
					if (!true)
					{
					}
					result = flag;
					goto IL_16C;
				}
				break;
			}
		}
		result = false;
		IL_16C:
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x06003A1F RID: 14879 RVA: 0x001D9954 File Offset: 0x001D7B54
	public static string ParseIconSprite(int professionId, int index)
	{
		if (!true)
		{
		}
		string result;
		if (professionId <= 1)
		{
			if (professionId == 0)
			{
				if (!true)
				{
				}
				string text;
				if (index != 0)
				{
					text = string.Empty;
				}
				else
				{
					text = "map_profession_icon_0";
				}
				if (!true)
				{
				}
				result = text;
				goto IL_17B;
			}
			if (professionId == 1)
			{
				if (!true)
				{
				}
				string text;
				if (index != 0)
				{
					text = string.Empty;
				}
				else
				{
					text = "map_profession_icon_1";
				}
				if (!true)
				{
				}
				result = text;
				goto IL_17B;
			}
		}
		else
		{
			if (professionId == 5)
			{
				if (!true)
				{
				}
				string text;
				if (index != 0)
				{
					if (index != 1)
					{
						text = string.Empty;
					}
					else
					{
						text = "map_profession_icon_6";
					}
				}
				else
				{
					text = "map_profession_icon_5";
				}
				if (!true)
				{
				}
				result = text;
				goto IL_17B;
			}
			switch (professionId)
			{
			case 10:
			{
				if (!true)
				{
				}
				string text;
				if (index != 0)
				{
					text = string.Empty;
				}
				else
				{
					text = "map_profession_icon_2";
				}
				if (!true)
				{
				}
				result = text;
				goto IL_17B;
			}
			case 11:
				break;
			case 12:
			{
				if (!true)
				{
				}
				string text;
				if (index != 0)
				{
					if (index != 1)
					{
						text = string.Empty;
					}
					else
					{
						text = "map_profession_icon_4";
					}
				}
				else
				{
					text = "map_profession_icon_3";
				}
				if (!true)
				{
				}
				result = text;
				goto IL_17B;
			}
			case 13:
			{
				if (!true)
				{
				}
				string text;
				switch (index)
				{
				case 0:
					text = "map_profession_icon_7";
					break;
				case 1:
					text = "map_profession_icon_8";
					break;
				case 2:
					text = "map_profession_icon_9";
					break;
				default:
					text = string.Empty;
					break;
				}
				if (!true)
				{
				}
				result = text;
				goto IL_17B;
			}
			default:
				if (professionId == 17)
				{
					if (!true)
					{
					}
					string text;
					if (index != 0)
					{
						text = string.Empty;
					}
					else
					{
						text = "map_profession_icon_10";
					}
					if (!true)
					{
					}
					result = text;
					goto IL_17B;
				}
				break;
			}
		}
		result = string.Empty;
		IL_17B:
		if (!true)
		{
		}
		return result;
	}

	// Token: 0x040029EA RID: 10730
	[SerializeField]
	private TextMeshProUGUI txtMeshCount;

	// Token: 0x040029EB RID: 10731
	[SerializeField]
	private CImage imgIcon;
}
