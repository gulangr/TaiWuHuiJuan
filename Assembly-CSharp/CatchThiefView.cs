using System;
using System.Collections;
using DG.Tweening;
using EasyButtons;
using UnityEngine;

// Token: 0x020001C2 RID: 450
[RequireComponent(typeof(AudioSource))]
public class CatchThiefView : MonoBehaviour
{
	// Token: 0x06001BED RID: 7149 RVA: 0x000C11F4 File Offset: 0x000BF3F4
	private void Awake()
	{
		bool flag = this.audioSource == null;
		if (flag)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
	}

	// Token: 0x06001BEE RID: 7150 RVA: 0x000C1220 File Offset: 0x000BF420
	[Button]
	private void Test()
	{
		bool flag = this.audioSource == null;
		if (!flag)
		{
			base.StopAllCoroutines();
			this.audioSource.DOKill(false);
			this.audioSource.Stop();
			this.audioSource.loop = true;
			this.audioSource.pitch = Random.Range(0.6f, 1.25f);
			this.audioSource.volume = 0f;
			this.randomDelay = Random.Range(0f, 5f);
			base.StartCoroutine(this.DelayPlay());
		}
	}

	// Token: 0x06001BEF RID: 7151 RVA: 0x000C12BB File Offset: 0x000BF4BB
	public IEnumerator DelayPlay()
	{
		yield return new WaitForSeconds(this.randomDelay);
		this.audioSource.DOFade(1f, this.fadeDuration);
		this.audioSource.Play();
		yield break;
	}

	// Token: 0x040015C5 RID: 5573
	public AudioSource audioSource;

	// Token: 0x040015C6 RID: 5574
	public float fadeDuration = 0.6f;

	// Token: 0x040015C7 RID: 5575
	public float randomDelay;
}
