using System;
using System.Text;
using EasyButtons;
using FrameWork;
using GameData.Domains.Combat;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B2C RID: 2860
	public class CombatDefeatMarkParticle : Refers
	{
		// Token: 0x06008C28 RID: 35880 RVA: 0x0040C3A8 File Offset: 0x0040A5A8
		private static CombatDefeatMarkParticle.EShowType ParseShowType(int markCount, byte requireCount)
		{
			bool flag = markCount >= (int)requireCount;
			CombatDefeatMarkParticle.EShowType result;
			if (flag)
			{
				result = CombatDefeatMarkParticle.EShowType.Full;
			}
			else
			{
				bool flag2 = markCount >= (int)(requireCount / 2);
				if (flag2)
				{
					result = CombatDefeatMarkParticle.EShowType.Half;
				}
				else
				{
					result = CombatDefeatMarkParticle.EShowType.None;
				}
			}
			return result;
		}

		// Token: 0x06008C29 RID: 35881 RVA: 0x0040C3DB File Offset: 0x0040A5DB
		public void Clear()
		{
			this._showType = CombatDefeatMarkParticle.EShowType.None;
		}

		// Token: 0x06008C2A RID: 35882 RVA: 0x0040C3E8 File Offset: 0x0040A5E8
		public void Set(CombatType combatType, DefeatMarkCollection markCollection)
		{
			byte requireCount = GlobalConfig.NeedDefeatMarkCount[(int)combatType];
			int markCount = markCollection.GetTotalCount();
			CombatDefeatMarkParticle.EShowType oldType = this._showType;
			CombatDefeatMarkParticle.EShowType newType = CombatDefeatMarkParticle.ParseShowType(markCount, requireCount);
			bool flag = newType == oldType;
			if (!flag)
			{
				this._showType = newType;
				bool flag2 = oldType == CombatDefeatMarkParticle.EShowType.None || newType == CombatDefeatMarkParticle.EShowType.Full;
				if (flag2)
				{
					this.PlayEffect(combatType, newType);
				}
			}
		}

		// Token: 0x06008C2B RID: 35883 RVA: 0x0040C440 File Offset: 0x0040A640
		[Button("播放特效")]
		private void PlayEffect(CombatType combatType, CombatDefeatMarkParticle.EShowType newType)
		{
			EffectPlayer effectPlayer = base.CGet<EffectPlayer>("Player");
			string effectName = this.GetParticleName(combatType, newType);
			float duration = effectPlayer.GetSrcEffect(effectName).GetComponent<ParticleSystem>().main.duration;
			effectPlayer.PlayEffectAt(base.CGet<RectTransform>("Root"), effectName, duration, false);
		}

		// Token: 0x06008C2C RID: 35884 RVA: 0x0040C494 File Offset: 0x0040A694
		private string GetParticleName(CombatType combatType, CombatDefeatMarkParticle.EShowType showType)
		{
			StringBuilder builder = EasyPool.Get<StringBuilder>();
			builder.Clear();
			builder.Append("eff_combat_ui_mark_");
			StringBuilder stringBuilder = builder;
			if (!true)
			{
			}
			string value;
			if (showType != CombatDefeatMarkParticle.EShowType.Half)
			{
				if (showType != CombatDefeatMarkParticle.EShowType.Full)
				{
					throw new ArgumentOutOfRangeException("showType", showType, null);
				}
				value = "full";
			}
			else
			{
				value = "half";
			}
			if (!true)
			{
			}
			stringBuilder.Append(value);
			builder.Append("_");
			StringBuilder stringBuilder2 = builder;
			if (!true)
			{
			}
			switch (combatType)
			{
			case CombatType.Play:
				value = "0";
				break;
			case CombatType.Beat:
			case CombatType.Test:
				value = "1";
				break;
			case CombatType.Die:
				value = "2";
				break;
			default:
				throw new ArgumentOutOfRangeException("combatType", combatType, null);
			}
			if (!true)
			{
			}
			stringBuilder2.Append(value);
			string ret = builder.ToString();
			EasyPool.Free<StringBuilder>(builder);
			return ret;
		}

		// Token: 0x04006B4A RID: 27466
		private CombatDefeatMarkParticle.EShowType _showType;

		// Token: 0x020020EE RID: 8430
		public enum EShowType
		{
			// Token: 0x0400D2F5 RID: 54005
			None,
			// Token: 0x0400D2F6 RID: 54006
			Half,
			// Token: 0x0400D2F7 RID: 54007
			Full
		}
	}
}
