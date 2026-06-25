using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001017 RID: 4119
	[RequireComponent(typeof(CDropdownLegacy))]
	public class GradeDropdown : MonoBehaviour
	{
		// Token: 0x1700153C RID: 5436
		// (get) Token: 0x0600BC5D RID: 48221 RVA: 0x0055A650 File Offset: 0x00558850
		private static List<string> GradeNameList
		{
			get
			{
				bool flag = GradeDropdown.InternalGradeNameList.Count != 0;
				List<string> internalGradeNameList;
				if (flag)
				{
					internalGradeNameList = GradeDropdown.InternalGradeNameList;
				}
				else
				{
					for (int i = 9; i > 0; i--)
					{
						string gradeName = CommonUtils.GetItemGradeShortNameWithMoreThan(i);
						GradeDropdown.InternalGradeNameList.Add(gradeName);
					}
					internalGradeNameList = GradeDropdown.InternalGradeNameList;
				}
				return internalGradeNameList;
			}
		}

		// Token: 0x1700153D RID: 5437
		// (get) Token: 0x0600BC5E RID: 48222 RVA: 0x0055A6A8 File Offset: 0x005588A8
		private CDropdownLegacy Dropdown
		{
			get
			{
				return base.GetComponent<CDropdownLegacy>();
			}
		}

		// Token: 0x0600BC5F RID: 48223 RVA: 0x0055A6B0 File Offset: 0x005588B0
		public void Setup(sbyte grade = -1, UnityAction<sbyte> onGradeChanged = null)
		{
			this.Dropdown.onValueChanged.RemoveAllListeners();
			this.Dropdown.ClearOptions();
			this.Dropdown.AddOptions(GradeDropdown.GradeNameList);
			this.SetByGrade(grade);
			this.Dropdown.onValueChanged.AddListener(delegate(int index)
			{
				UnityAction<sbyte> onGradeChanged2 = onGradeChanged;
				if (onGradeChanged2 != null)
				{
					onGradeChanged2(GradeDropdown.GetGradeByIndex(index));
				}
			});
		}

		// Token: 0x0600BC60 RID: 48224 RVA: 0x0055A71E File Offset: 0x0055891E
		public void SetByGrade(sbyte grade)
		{
			this.Dropdown.value = GradeDropdown.GetIndexByGrade(grade);
		}

		// Token: 0x0600BC61 RID: 48225 RVA: 0x0055A734 File Offset: 0x00558934
		public sbyte GetSelectedGrade()
		{
			return GradeDropdown.GetGradeByIndex(this.Dropdown.value);
		}

		// Token: 0x0600BC62 RID: 48226 RVA: 0x0055A758 File Offset: 0x00558958
		private void OnGUI()
		{
			bool flag = this.Dropdown && this.Dropdown.IsExpanded;
			if (flag)
			{
				Transform trans = this.Dropdown.transform.Find("Dropdown List");
				bool flag2 = !trans;
				if (!flag2)
				{
					CToggleObsolete[] toggles = this.Dropdown.GetComponentsInChildren<CToggleObsolete>();
					PositionFollower positionFollower = this.Dropdown.GetComponentInChildren<PositionFollower>();
					foreach (CToggleObsolete togCell in toggles)
					{
						bool flag3 = !togCell.gameObject.activeSelf;
						if (!flag3)
						{
							togCell.transform.Find("Disable").gameObject.SetActive(togCell.isOn);
							bool flag4 = togCell.isOn && positionFollower;
							if (flag4)
							{
								positionFollower.Target = togCell.transform;
							}
						}
					}
					RectTransform content = trans.GetComponentInChildren<CScrollRectLegacy>().Content;
					int childCount = content.childCount;
					for (int i = 1; i < childCount; i++)
					{
						Transform item = content.GetChild(i);
						Transform gradeBack = item.Find("Layout/GradeBack");
						bool flag5 = i == 1;
						if (flag5)
						{
							bool activeSelf = gradeBack.gameObject.activeSelf;
							if (activeSelf)
							{
								gradeBack.gameObject.SetActive(false);
							}
						}
						else
						{
							bool flag6 = !gradeBack.gameObject.activeSelf;
							if (flag6)
							{
								gradeBack.gameObject.SetActive(true);
							}
							sbyte grade = GradeDropdown.GetGradeByIndex(i - 1);
							CImage component = gradeBack.GetComponent<CImage>();
							if (component != null)
							{
								component.SetSprite(ItemView.GetGradeIcon(grade), false, null);
							}
							TextMeshProUGUI componentInChildren = gradeBack.GetComponentInChildren<TextMeshProUGUI>();
							if (componentInChildren != null)
							{
								componentInChildren.SetText(ItemView.GetGradeText(grade), true);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600BC63 RID: 48227 RVA: 0x0055A930 File Offset: 0x00558B30
		private static sbyte GetGradeByIndex(int index)
		{
			bool flag = index == 0;
			sbyte result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				int garde = 9 - index;
				result = (sbyte)garde;
			}
			return result;
		}

		// Token: 0x0600BC64 RID: 48228 RVA: 0x0055A958 File Offset: 0x00558B58
		private static int GetIndexByGrade(sbyte grade)
		{
			bool flag = grade == -1;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int index = (int)(9 - grade);
				result = index;
			}
			return result;
		}

		// Token: 0x0400910E RID: 37134
		private static readonly List<string> InternalGradeNameList = new List<string>(9);
	}
}
