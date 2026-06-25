using System;
using FrameWork.UISystem.Components;
using UnityEngine;

namespace Game.Views.Combat
{
	// Token: 0x02000B34 RID: 2868
	public class CombatTime : MonoBehaviour, ICombatComponent
	{
		// Token: 0x17000F78 RID: 3960
		// (get) Token: 0x06008C87 RID: 35975 RVA: 0x0040F150 File Offset: 0x0040D350
		private CombatModel Model
		{
			get
			{
				return SingletonObject.getInstance<CombatModel>();
			}
		}

		// Token: 0x06008C88 RID: 35976 RVA: 0x0040F157 File Offset: 0x0040D357
		public void Setup()
		{
			this.Model.AddEvent(ECombatEvents.OnCombatFrameChanged, new OnCombatEvent(this.Set));
		}

		// Token: 0x06008C89 RID: 35977 RVA: 0x0040F172 File Offset: 0x0040D372
		public void Close()
		{
			this.Model.RemoveEvent(ECombatEvents.OnCombatFrameChanged, new OnCombatEvent(this.Set));
		}

		// Token: 0x06008C8A RID: 35978 RVA: 0x0040F190 File Offset: 0x0040D390
		private void Set()
		{
			uint defeatFrame = this.Model.Config.ForceDefeatFrame;
			bool useSpecialColor = false;
			bool infinity = defeatFrame == 0U || this.Model.Config.ForceDefeatType == ECombatConfigForceDefeatType.Invalid;
			bool flag = infinity;
			if (flag)
			{
				this.time.SetAsInfinity();
			}
			else
			{
				ulong leftFrame = this.CalcLeftFrame();
				ulong leftSeconds = leftFrame / 60UL + (ulong)((leftFrame % 60UL > 0UL) ? 1L : 0L);
				this.time.Set((int)leftSeconds);
				useSpecialColor = (leftSeconds < 10UL);
			}
			this.time.SetColor(useSpecialColor ? "#f66751".HexStringToColor() : Color.white);
			LanguageKey languageKey = infinity ? LanguageKey.LK_Combat_Time_Tips_Content_Infinity : ((this.Model.Config.ForceDefeatType == ECombatConfigForceDefeatType.TiredMark) ? LanguageKey.LK_Combat_Time_Tips_Content_Tired_Mark : LanguageKey.LK_Combat_Time_Tips_Content_Immediate_Win);
			this.mouseTip.PresetParam[1] = languageKey.Tr();
		}

		// Token: 0x06008C8B RID: 35979 RVA: 0x0040F278 File Offset: 0x0040D478
		private ulong CalcLeftFrame()
		{
			ulong frame = this.Model.CombatFrame;
			uint defeatFrame = this.Model.Config.ForceDefeatFrame;
			bool flag = (ulong)defeatFrame >= frame;
			ulong result;
			if (flag)
			{
				result = (ulong)defeatFrame - frame;
			}
			else
			{
				bool flag2 = this.Model.Config.ForceDefeatType != ECombatConfigForceDefeatType.TiredMark;
				if (flag2)
				{
					result = 0UL;
				}
				else
				{
					uint appearFrame = GlobalConfig.Instance.TiredMarkAppearFrame;
					ulong duration = frame - (ulong)defeatFrame;
					bool flag3 = duration % (ulong)appearFrame == 0UL;
					if (flag3)
					{
						result = 0UL;
					}
					else
					{
						result = (ulong)appearFrame - duration % (ulong)appearFrame;
					}
				}
			}
			return result;
		}

		// Token: 0x04006B70 RID: 27504
		[SerializeField]
		private TooltipInvoker mouseTip;

		// Token: 0x04006B71 RID: 27505
		[SerializeField]
		private ImageNumber time;
	}
}
