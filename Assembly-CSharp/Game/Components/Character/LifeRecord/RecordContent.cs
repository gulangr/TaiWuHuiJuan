using System;
using GameData.Domains.LifeRecord;
using TMPro;
using UnityEngine;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F5C RID: 3932
	public class RecordContent : MonoBehaviour
	{
		// Token: 0x0600B406 RID: 46086 RVA: 0x0051E784 File Offset: 0x0051C984
		public virtual void Set(TransferableRecordDataBase data, string contentText, string effectText)
		{
			this.content.text = contentText;
			this.contentHelper.Parse();
			this.effect.gameObject.SetActive(!string.IsNullOrEmpty(effectText));
			bool flag = string.IsNullOrEmpty(effectText);
			if (flag)
			{
				this.effect.gameObject.SetActive(false);
				this.nameButtonAnalyzer.Set(data, new TMP_Text[]
				{
					this.content
				});
			}
			else
			{
				this.effect.gameObject.SetActive(true);
				this.effect.text = effectText;
				this.effectHelper.Parse();
				this.nameButtonAnalyzer.Set(data, new TMP_Text[]
				{
					this.content,
					this.effect
				});
			}
			base.gameObject.SetActive(true);
		}

		// Token: 0x04008C04 RID: 35844
		[SerializeField]
		private TMP_Text content;

		// Token: 0x04008C05 RID: 35845
		[SerializeField]
		private TMP_Text effect;

		// Token: 0x04008C06 RID: 35846
		[SerializeField]
		private TMPTextSpriteHelper contentHelper;

		// Token: 0x04008C07 RID: 35847
		[SerializeField]
		private TMPTextSpriteHelper effectHelper;

		// Token: 0x04008C08 RID: 35848
		[SerializeField]
		private NameButtonAnalyzerBase nameButtonAnalyzer;
	}
}
