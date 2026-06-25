using System;

namespace FrameWork.UISystem.UIElements
{
	// Token: 0x0200100D RID: 4109
	public interface ICToggleGroup
	{
		// Token: 0x0600BBDB RID: 48091
		bool ValidateStateChange(CToggle toggle, bool isOn);

		// Token: 0x0600BBDC RID: 48092
		void NotifyToggle(CToggle toggle, bool value, bool raiseEvent = true);
	}
}
