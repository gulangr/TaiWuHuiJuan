using System;
using Coffee.UIExtensions;
using Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.SectInteract.Shixiang
{
	// Token: 0x020009E3 RID: 2531
	public class ShixiangUpgradeTeammateCommandProperty : MonoBehaviour
	{
		// Token: 0x06007C29 RID: 31785 RVA: 0x0039B884 File Offset: 0x00399A84
		public void Set(TeammateCommandItem afterConfig, TeammateCommandItem originConfig, int index, bool isUpgrade)
		{
			this._type = afterConfig.EffectDisplayPositiveList[index];
			this.propertyName.text = afterConfig.EffectDisplayTextList[index];
			TextMeshProUGUI textMeshProUGUI = this.propertyValue;
			string str = afterConfig.ShixiangEffectDisplayValueList[index];
			string color;
			if (isUpgrade)
			{
				color = "brightyellow";
			}
			else
			{
				sbyte type = this._type;
				if (!true)
				{
				}
				string text2;
				if (type != 1)
				{
					if (type != 2)
					{
						text2 = "brightyellow";
					}
					else
					{
						text2 = "brightred";
					}
				}
				else
				{
					text2 = "brightblue";
				}
				if (!true)
				{
				}
				color = text2;
			}
			textMeshProUGUI.text = str.SetColor(color);
			int originConfigIndex = -1;
			bool isUseCount = false;
			string text = LanguageKey.LK_Making_Weave_WeavedClothing_Count.Tr();
			bool flag = string.Equals(afterConfig.EffectDisplayTextList[index], text);
			if (flag)
			{
				isUseCount = true;
			}
			else
			{
				for (int i = 0; i < originConfig.EffectDisplayTextList.Length; i++)
				{
					string effect = originConfig.EffectDisplayTextList[i];
					bool flag2 = string.Equals(afterConfig.EffectDisplayTextList[index], effect);
					if (flag2)
					{
						originConfigIndex = i;
						break;
					}
				}
			}
			bool flag3 = originConfigIndex >= 0 || isUseCount;
			if (flag3)
			{
				this.propertyOriginValue.text = (isUseCount ? "1" : originConfig.ShixiangEffectDisplayValueList[originConfigIndex]);
				this.propertyOriginValue.gameObject.SetActive(true);
				this.changeMark.gameObject.SetActive(true);
			}
			else
			{
				this.propertyOriginValue.gameObject.SetActive(false);
				this.changeMark.gameObject.SetActive(false);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(this.changeRoot);
			this.changeMark.sprite = ((this._type == 1) ? this.spriteUp : this.spriteDown);
			this.positiveParticle.gameObject.SetActive(false);
			this.negativeParticle.gameObject.SetActive(false);
			this.positiveParticles.gameObject.SetActive(false);
			this.negativeParticles.gameObject.SetActive(false);
			bool flag4 = !isUpgrade && this._type != 0;
			if (flag4)
			{
				Transform particles = (this._type == 1) ? this.positiveParticles : this.negativeParticles;
				particles.gameObject.SetActive(true);
				UIParticle uiParticle = particles.GetComponent<UIParticle>();
				bool flag5 = uiParticle != null;
				if (flag5)
				{
					SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
					{
						uiParticle.Play();
					});
				}
				else
				{
					for (int j = 0; j < particles.childCount; j++)
					{
						ParticleSystem particle = particles.GetChild(j).GetComponent<ParticleSystem>();
						particle.Stop();
						particle.gameObject.SetActive(false);
						particle.gameObject.SetActive(true);
						SingletonObject.getInstance<YieldHelper>().DelayFrameDo(2U, delegate
						{
							particle.Play();
						});
					}
				}
			}
			if (isUpgrade)
			{
				this.propertyOriginValue.gameObject.SetActive(false);
				this.changeMark.gameObject.SetActive(false);
			}
		}

		// Token: 0x06007C2A RID: 31786 RVA: 0x0039BBC4 File Offset: 0x00399DC4
		public void PlayParticle()
		{
			bool flag = this._type == 0;
			if (!flag)
			{
				ParticleSystem particle = (this._type == 1) ? this.positiveParticle : this.negativeParticle;
				particle.gameObject.SetActive(true);
				particle.Play();
				SingletonObject.getInstance<YieldHelper>().DelaySecondsDo(1.5f, delegate
				{
					particle.Stop();
					particle.gameObject.SetActive(false);
				});
			}
		}

		// Token: 0x04005E5C RID: 24156
		public TextMeshProUGUI propertyName;

		// Token: 0x04005E5D RID: 24157
		public TextMeshProUGUI propertyValue;

		// Token: 0x04005E5E RID: 24158
		public TextMeshProUGUI propertyOriginValue;

		// Token: 0x04005E5F RID: 24159
		public CImage changeMark;

		// Token: 0x04005E60 RID: 24160
		public Sprite spriteUp;

		// Token: 0x04005E61 RID: 24161
		public Sprite spriteDown;

		// Token: 0x04005E62 RID: 24162
		public RectTransform changeRoot;

		// Token: 0x04005E63 RID: 24163
		public Transform positiveParticles;

		// Token: 0x04005E64 RID: 24164
		public Transform negativeParticles;

		// Token: 0x04005E65 RID: 24165
		public ParticleSystem positiveParticle;

		// Token: 0x04005E66 RID: 24166
		public ParticleSystem negativeParticle;

		// Token: 0x04005E67 RID: 24167
		private sbyte _type;

		// Token: 0x04005E68 RID: 24168
		private const float Duration = 1.5f;
	}
}
