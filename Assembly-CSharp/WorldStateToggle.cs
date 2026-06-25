using System;
using FrameWork;
using Game.Views.Bottom;
using UnityEngine;

// Token: 0x020003DD RID: 989
[RequireComponent(typeof(TooltipInvoker))]
[RequireComponent(typeof(DisableStyleRoot))]
public class WorldStateToggle : MonoBehaviour
{
	// Token: 0x17000606 RID: 1542
	// (get) Token: 0x06003B95 RID: 15253 RVA: 0x001E2FCF File Offset: 0x001E11CF
	// (set) Token: 0x06003B96 RID: 15254 RVA: 0x001E2FD7 File Offset: 0x001E11D7
	public sbyte SectId
	{
		get
		{
			return this._id;
		}
		set
		{
			this._id = value;
			this.OnEnable();
		}
	}

	// Token: 0x06003B97 RID: 15255 RVA: 0x001E2FE8 File Offset: 0x001E11E8
	public void OnEnable()
	{
		sbyte id = this._id;
		bool flag = id >= 1 && id <= 15;
		if (flag)
		{
			SectMainSettings.GetSectMainStoryIsActive(this._id, new Action<int>(this.RefreshActiveStatus), UIElement.WorldState.UiBaseAs<ViewWorldState>());
		}
	}

	// Token: 0x06003B98 RID: 15256 RVA: 0x001E3034 File Offset: 0x001E1234
	private void RefreshActiveStatus(int status)
	{
		if (status != -2147483648)
		{
			if (status != -1)
			{
				if (status > 1)
				{
					Debug.LogError(string.Format("Should not display sectMainStory worldStatus with status {0}", status));
				}
				else
				{
					this.disableStyleRoot.SetStyleEffect(false, false);
					ArgumentBox runtimeParam = this.mouseTipDisplayer.RuntimeParam;
					if (runtimeParam != null)
					{
						runtimeParam.Set("Paused", false);
					}
				}
			}
			else
			{
				this.disableStyleRoot.SetStyleEffect(true, false);
				ArgumentBox runtimeParam2 = this.mouseTipDisplayer.RuntimeParam;
				if (runtimeParam2 != null)
				{
					runtimeParam2.Set("Paused", true);
				}
			}
		}
		else
		{
			SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, new Action(this.OnEnable));
		}
	}

	// Token: 0x04002AED RID: 10989
	[SerializeField]
	private TooltipInvoker mouseTipDisplayer;

	// Token: 0x04002AEE RID: 10990
	[SerializeField]
	private DisableStyleRoot disableStyleRoot;

	// Token: 0x04002AEF RID: 10991
	private sbyte _id = -1;
}
