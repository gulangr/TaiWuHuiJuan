using System;
using FrameWork.UISystem.UIElements;
using GameData.Adventure;
using GameData.Adventure.Editor;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000187 RID: 391
public class AdventureElementLightSubEditor : MonoBehaviour
{
	// Token: 0x06001612 RID: 5650 RVA: 0x00088C0B File Offset: 0x00086E0B
	public void Setup(AdventureElementSnapshot elementData)
	{
		this._elementData = elementData;
		this.Refresh();
	}

	// Token: 0x06001613 RID: 5651 RVA: 0x00088C1C File Offset: 0x00086E1C
	private void Awake()
	{
		this.lightToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleChanged));
	}

	// Token: 0x06001614 RID: 5652 RVA: 0x00088C3C File Offset: 0x00086E3C
	private void Refresh()
	{
		AdventureElementSnapshot elementData = this._elementData;
		bool hasLight = ((elementData != null) ? elementData.LightData : null) != null;
		this.lightToggle.SetIsOnWithoutNotify(hasLight);
		this.configRoot.SetActive(hasLight);
		bool flag = hasLight && this.lightConfig != null;
		if (flag)
		{
			this.lightConfig.Setup(this._elementData);
			this.lightConfig.ReloadFromBlackBoard();
		}
	}

	// Token: 0x06001615 RID: 5653 RVA: 0x00088CB0 File Offset: 0x00086EB0
	private void OnToggleChanged(bool enabled)
	{
		bool flag = this._elementData == null;
		if (!flag)
		{
			if (enabled)
			{
				AdventureElementSnapshot elementData = this._elementData;
				if (elementData.LightData == null)
				{
					elementData.LightData = new AdventureLightData
					{
						ColorInHex = "FFFFFF",
						Strength = 1f,
						Height = 1f
					};
				}
			}
			else
			{
				this._elementData.LightData = null;
			}
			this.configRoot.SetActive(enabled);
			bool flag2 = enabled && this.lightConfig != null;
			if (flag2)
			{
				this.lightConfig.Setup(this._elementData);
				this.lightConfig.ReloadFromBlackBoard();
			}
		}
	}

	// Token: 0x0400120B RID: 4619
	[SerializeField]
	private CToggle lightToggle;

	// Token: 0x0400120C RID: 4620
	[SerializeField]
	private AdventureEditorElementPointLightComponent lightConfig;

	// Token: 0x0400120D RID: 4621
	[SerializeField]
	private GameObject configRoot;

	// Token: 0x0400120E RID: 4622
	private AdventureElementSnapshot _elementData;
}
