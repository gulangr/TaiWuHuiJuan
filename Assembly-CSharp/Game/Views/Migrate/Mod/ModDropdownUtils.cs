using System;
using FrameWork.UISystem.UIElements;
using UnityEngine;

namespace Game.Views.Migrate.Mod
{
	// Token: 0x02000912 RID: 2322
	public static class ModDropdownUtils
	{
		// Token: 0x06006D99 RID: 28057 RVA: 0x0032B1E4 File Offset: 0x003293E4
		public static void HandleDropdown(CDropdown dropdown, Action<bool> showDropdownMask)
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
					CToggle[] toggles = dropdown.GetComponentsInChildren<CToggle>();
					PositionFollower positionFollower = dropdown.GetComponentInChildren<PositionFollower>();
					foreach (CToggle togCell in toggles)
					{
						bool flag3 = !togCell.gameObject.activeSelf;
						if (!flag3)
						{
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
}
