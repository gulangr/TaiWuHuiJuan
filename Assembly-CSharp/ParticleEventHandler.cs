using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000034 RID: 52
[RequireComponent(typeof(ParticleSystem))]
public class ParticleEventHandler : MonoBehaviour
{
	// Token: 0x060001D5 RID: 469 RVA: 0x0000B920 File Offset: 0x00009B20
	private void Awake()
	{
		base.GetComponent<ParticleSystem>().main.stopAction = ParticleSystemStopAction.Callback;
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x0000B943 File Offset: 0x00009B43
	private void OnParticleSystemStopped()
	{
		UnityEvent particleSystemStoppedEvent = this.ParticleSystemStoppedEvent;
		if (particleSystemStoppedEvent != null)
		{
			particleSystemStoppedEvent.Invoke();
		}
	}

	// Token: 0x040000EF RID: 239
	public UnityEvent ParticleSystemStoppedEvent = new UnityEvent();
}
