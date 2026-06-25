using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.UISystem.Components
{
	// Token: 0x02001016 RID: 4118
	[RequireComponent(typeof(ParticleSystem))]
	public class EffectSwitcher : MonoBehaviour
	{
		// Token: 0x0600BC59 RID: 48217 RVA: 0x0055A538 File Offset: 0x00558738
		private void OnEnable()
		{
			foreach (EffectSwitcher.SetupData setupData in this.setups)
			{
				this.DoSetup(setupData);
			}
			foreach (EffectSwitcher.SwitchData switchData in this.switches)
			{
				base.StartCoroutine(this.CoSwitch(switchData));
			}
		}

		// Token: 0x0600BC5A RID: 48218 RVA: 0x0055A5DC File Offset: 0x005587DC
		private void DoSetup(EffectSwitcher.SetupData setupData)
		{
			EffectSwitcher.ESwitchType type = setupData.type;
			EffectSwitcher.ESwitchType eswitchType = type;
			if (eswitchType != EffectSwitcher.ESwitchType.Active)
			{
				if (eswitchType != EffectSwitcher.ESwitchType.DeActive)
				{
					throw new ArgumentOutOfRangeException();
				}
				setupData.target.SetActive(false);
			}
			else
			{
				setupData.target.SetActive(true);
				base.GetComponent<ParticleSystem>().Play(true);
			}
		}

		// Token: 0x0600BC5B RID: 48219 RVA: 0x0055A62F File Offset: 0x0055882F
		private IEnumerator CoSwitch(EffectSwitcher.SwitchData switchData)
		{
			yield return new WaitForSeconds(switchData.delay);
			this.DoSetup(switchData.setup);
			yield break;
		}

		// Token: 0x0400910C RID: 37132
		[Tooltip("此处设置特效显示时的子物体状态")]
		public List<EffectSwitcher.SetupData> setups;

		// Token: 0x0400910D RID: 37133
		[Tooltip("此处设置特效播放时的动态变化")]
		public List<EffectSwitcher.SwitchData> switches;

		// Token: 0x0200265B RID: 9819
		public enum ESwitchType
		{
			// Token: 0x0400EA5A RID: 59994
			Active,
			// Token: 0x0400EA5B RID: 59995
			DeActive
		}

		// Token: 0x0200265C RID: 9820
		[Serializable]
		public class SetupData
		{
			// Token: 0x0400EA5C RID: 59996
			public EffectSwitcher.ESwitchType type;

			// Token: 0x0400EA5D RID: 59997
			public GameObject target;
		}

		// Token: 0x0200265D RID: 9821
		[Serializable]
		public class SwitchData
		{
			// Token: 0x0400EA5E RID: 59998
			public EffectSwitcher.SetupData setup;

			// Token: 0x0400EA5F RID: 59999
			public float delay;
		}
	}
}
