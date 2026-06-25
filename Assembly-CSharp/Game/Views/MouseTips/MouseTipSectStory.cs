using System;
using Config;
using FrameWork;
using GameData.Domains.Story;
using GameData.Serializer;
using GameData.Utilities;
using TMPro;
using UnityEngine;

namespace Game.Views.MouseTips
{
	// Token: 0x02000877 RID: 2167
	public class MouseTipSectStory : MouseTipBase
	{
		// Token: 0x17000C7E RID: 3198
		// (get) Token: 0x06006853 RID: 26707 RVA: 0x002FB2CB File Offset: 0x002F94CB
		protected override bool CanStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06006854 RID: 26708 RVA: 0x002FB2CE File Offset: 0x002F94CE
		protected override void Init(ArgumentBox argsBox)
		{
			this.Refresh(argsBox);
		}

		// Token: 0x06006855 RID: 26709 RVA: 0x002FB2DC File Offset: 0x002F94DC
		public override void Refresh(ArgumentBox argBox)
		{
			base.Refresh();
			base.gameObject.GetComponent<CImage>().raycastTarget = false;
			sbyte templateId;
			argBox.Get("TemplateId", out templateId);
			int status;
			argBox.Get("ConditionStatus", out status);
			bool paused;
			argBox.Get("Paused", out paused);
			WorldStateItem config = WorldState.Instance[templateId];
			sbyte sectTemplateId = config.Sect;
			this.back.SetSprite(string.Format("{0}{1}", "ui9_back_mousetip_sectstory_", (int)(sectTemplateId - 1)), false, null);
			this.title.text = config.Name;
			this.desc.text = config.Desc;
			bool flag = config.SectStoryCondition == null;
			if (!flag)
			{
				this.UpdateStatus(config, status, paused);
				StoryDomainMethod.AsyncCall.GetSectMainStoryTriggerConditions(null, (short)config.Sect, delegate(int offset, RawDataPool dataPool)
				{
					Serializer.Deserialize(dataPool, offset, ref status);
					this.UpdateStatus(config, status, paused);
				});
			}
		}

		// Token: 0x06006856 RID: 26710 RVA: 0x002FB400 File Offset: 0x002F9600
		private void UpdateStatus(WorldStateItem config, int status, bool paused)
		{
			for (int i = 0; i < config.SectStoryCondition.Length; i++)
			{
				bool flag = i >= this.holder.childCount;
				if (flag)
				{
					Object.Instantiate<Transform>(this.holder.GetChild(0), this.holder);
				}
				Transform obj = this.holder.GetChild(i);
				bool isEnable = (status & 1 << i) != 0;
				obj.GetComponent<TextMeshProUGUI>().text = config.SectStoryCondition[i].SetColor(isEnable ? "brightblue" : "grey");
				obj.gameObject.SetActive(true);
			}
			for (int j = config.SectStoryCondition.Length; j < this.holder.childCount; j++)
			{
				this.holder.GetChild(j).gameObject.SetActive(false);
			}
			this.starting.gameObject.SetActive(!paused);
			this.pause.gameObject.SetActive(paused);
			this.close.SetActive(!paused);
			this.open.SetActive(paused);
		}

		// Token: 0x040049FC RID: 18940
		[SerializeField]
		private CImage back;

		// Token: 0x040049FD RID: 18941
		[SerializeField]
		private TextMeshProUGUI title;

		// Token: 0x040049FE RID: 18942
		[SerializeField]
		private TextMeshProUGUI desc;

		// Token: 0x040049FF RID: 18943
		[SerializeField]
		private Transform holder;

		// Token: 0x04004A00 RID: 18944
		[SerializeField]
		private GameObject starting;

		// Token: 0x04004A01 RID: 18945
		[SerializeField]
		private GameObject pause;

		// Token: 0x04004A02 RID: 18946
		[SerializeField]
		private GameObject close;

		// Token: 0x04004A03 RID: 18947
		[SerializeField]
		private GameObject open;
	}
}
