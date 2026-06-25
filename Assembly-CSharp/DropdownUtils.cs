using System;
using TMPro;
using UnityEngine;

// Token: 0x02000347 RID: 839
public static class DropdownUtils
{
	// Token: 0x0600310D RID: 12557 RVA: 0x00180760 File Offset: 0x0017E960
	public static void HandleDropdown(TMP_Dropdown dropdown, Action<bool> showDropdownMask)
	{
		bool flag = !dropdown || !dropdown.IsExpanded;
		if (flag)
		{
			if (showDropdownMask != null)
			{
				showDropdownMask(false);
			}
		}
		else
		{
			Transform trans = dropdown.transform.Find("Dropdown List");
			bool flag2 = !trans;
			if (flag2)
			{
				if (showDropdownMask != null)
				{
					showDropdownMask(false);
				}
			}
			else
			{
				if (showDropdownMask != null)
				{
					showDropdownMask(true);
				}
				CToggleObsolete[] toggles = dropdown.GetComponentsInChildren<CToggleObsolete>();
				PositionFollower positionFollower = dropdown.GetComponentInChildren<PositionFollower>();
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
			}
		}
	}
}
