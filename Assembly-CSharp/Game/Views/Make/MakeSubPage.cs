using System;
using GameData.Domains.Building;
using GameData.Domains.Global;
using UnityEngine;

namespace Game.Views.Make
{
	// Token: 0x02000952 RID: 2386
	public class MakeSubPage : MonoBehaviour
	{
		// Token: 0x060070B4 RID: 28852 RVA: 0x00342A14 File Offset: 0x00340C14
		public virtual void Init(ViewMake parentView)
		{
			this.ParentView = parentView;
		}

		// Token: 0x060070B5 RID: 28853 RVA: 0x00342A20 File Offset: 0x00340C20
		public virtual void Refresh(BuildingMakeDisplayData displayData)
		{
			this.DisplayData = displayData;
			if (!true)
			{
			}
			short num;
			if (!(this is MakeSubPageAddPoison))
			{
				if (!(this is MakeSubPageMake))
				{
					if (!(this is MakeSubPageRefine))
					{
						if (!(this is MakeSubPageRemovePoison))
						{
							if (!(this is MakeSubPageRepair))
							{
								if (!(this is MakeSubPageWeave))
								{
									throw new ArgumentOutOfRangeException();
								}
								num = 136;
							}
							else
							{
								num = 103;
							}
						}
						else
						{
							num = 135;
						}
					}
					else
					{
						num = 104;
					}
				}
				else
				{
					num = 102;
				}
			}
			else
			{
				num = 137;
			}
			if (!true)
			{
			}
			short triggerKey = num;
			GlobalDomainMethod.Call.InvokeGuidingTrigger(triggerKey);
		}

		// Token: 0x060070B6 RID: 28854 RVA: 0x00342AA7 File Offset: 0x00340CA7
		public virtual void RequestData()
		{
		}

		// Token: 0x060070B7 RID: 28855 RVA: 0x00342AAC File Offset: 0x00340CAC
		public virtual bool QuickHide()
		{
			return false;
		}

		// Token: 0x060070B8 RID: 28856 RVA: 0x00342AC0 File Offset: 0x00340CC0
		public void OnAutoSelectToolToggleValueChanged(bool isOn)
		{
			SingletonObject.getInstance<GlobalSettings>().AutoSelectToolOnMake = isOn;
			SingletonObject.getInstance<GlobalSettings>().SaveSettings();
			if (isOn)
			{
				this.ParentView.AutoSelectTool();
			}
		}

		// Token: 0x040053B7 RID: 21431
		protected BuildingMakeDisplayData DisplayData;

		// Token: 0x040053B8 RID: 21432
		protected ViewMake ParentView;
	}
}
