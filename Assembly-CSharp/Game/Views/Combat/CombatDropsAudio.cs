using System;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B0A RID: 2826
	public class CombatDropsAudio : MonoBehaviour
	{
		// Token: 0x06008AF4 RID: 35572 RVA: 0x00404BBF File Offset: 0x00402DBF
		public void SetAudioName(string audioName)
		{
			this._audioName = audioName;
		}

		// Token: 0x06008AF5 RID: 35573 RVA: 0x00404BCC File Offset: 0x00402DCC
		private void OnCollisionEnter2D(Collision2D collision)
		{
			bool flag = collision.collider.CompareTag("Untagged");
			if (flag)
			{
				AudioManager.Instance.PlaySound(this._audioName, false, false);
			}
		}

		// Token: 0x04006AA4 RID: 27300
		private string _audioName;
	}
}
