using System;
using Config;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Views.NewGame
{
	// Token: 0x020007FF RID: 2047
	public class NewGameSubPageBornAreaToggleHelper : MonoBehaviour
	{
		// Token: 0x060063EB RID: 25579 RVA: 0x002DCE92 File Offset: 0x002DB092
		private void Awake()
		{
			UnityEvent unityEvent = this.onSetup;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
		}

		// Token: 0x060063EC RID: 25580 RVA: 0x002DCEA7 File Offset: 0x002DB0A7
		public void RefreshMapState(MapStateItem state)
		{
			this.areaIcon.SetSprite(MapArea.Instance[(short)state.MainAreaID].BigMapIcon, false, null);
			this.labelStateName.text = state.Name;
		}

		// Token: 0x060063ED RID: 25581 RVA: 0x002DCEDF File Offset: 0x002DB0DF
		public void RefreshLandFormType(LandFormTypeItem landFormType)
		{
			this.labelLandFormType.text = landFormType.Name;
		}

		// Token: 0x040045CE RID: 17870
		[SerializeField]
		private CImage areaIcon;

		// Token: 0x040045CF RID: 17871
		[SerializeField]
		private TextMeshProUGUI labelStateName;

		// Token: 0x040045D0 RID: 17872
		[SerializeField]
		private TextMeshProUGUI labelLandFormType;

		// Token: 0x040045D1 RID: 17873
		[SerializeField]
		private UnityEvent onSetup;
	}
}
