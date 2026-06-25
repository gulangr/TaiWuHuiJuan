using System;
using Config;
using GameData.Domains.LifeRecord;
using UnityEngine;

namespace Game.Components.Character.LifeRecord
{
	// Token: 0x02000F4B RID: 3915
	public class LifeRecordContent : RecordContent
	{
		// Token: 0x0600B39F RID: 45983 RVA: 0x0051C300 File Offset: 0x0051A500
		public void Set(RenderedRecordData renderedData, TransferableRecordDataBase data, string contentText, string effectText)
		{
			LifeRecordItem item = LifeRecord.Instance[renderedData.TemplateId];
			int num;
			if (item == null)
			{
				num = -1;
			}
			else
			{
				int score = renderedData.Score;
				if (!true)
				{
				}
				int num2;
				if (item.DisplayType != ELifeRecordDisplayType.Great)
				{
					if (score <= 60)
					{
						if (score < 40)
						{
							if (score >= 20)
							{
								num2 = 2;
							}
							else
							{
								num2 = 1;
							}
						}
						else if (score >= 50)
						{
							if (score != 50)
							{
								num2 = 4;
							}
							else
							{
								num2 = -1;
							}
						}
						else
						{
							num2 = 3;
						}
					}
					else if (score > 80)
					{
						num2 = 6;
					}
					else
					{
						num2 = 5;
					}
				}
				else
				{
					num2 = 0;
				}
				if (!true)
				{
				}
				num = num2;
			}
			int scoreIndex = num;
			bool flag = this.bg.enabled = this.sprites.CheckIndex(scoreIndex);
			if (flag)
			{
				this.bg.sprite = this.sprites[scoreIndex];
			}
			base.Set(data, contentText, effectText);
		}

		// Token: 0x04008B96 RID: 35734
		[SerializeField]
		private Sprite[] sprites;

		// Token: 0x04008B97 RID: 35735
		[SerializeField]
		private CImage bg;
	}
}
