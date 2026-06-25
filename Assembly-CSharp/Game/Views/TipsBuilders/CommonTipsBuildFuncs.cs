using System;
using Config;
using FrameWork;
using Game.Views.MouseTips;

namespace Game.Views.TipsBuilders
{
	// Token: 0x02000750 RID: 1872
	public static class CommonTipsBuildFuncs
	{
		// Token: 0x06005AD8 RID: 23256 RVA: 0x002A2590 File Offset: 0x002A0790
		[CommonTipBuilder(41, TipType.Flaw)]
		public static ArgumentBox BuildCommonTipArg_Flaw(CommonTipItem def, ArgumentBox arg)
		{
			CommonTipSimpleRuntime runtime = CommonTipsHelper.GetOrCreateSimpleRuntimeTipForBuild(def, arg);
			byte bodyPart;
			arg.Get("BodyPart", out bodyPart);
			byte count;
			arg.Get("Count", out count);
			string partIconName = MouseTipConstant.HitPartNamesByConfig[(int)bodyPart, 2];
			string partName = LocalStringManager.Get(MouseTipConstant.HitPartNamesByConfig[(int)bodyPart, 1]);
			runtime.Set("BodyPart.Name", partName);
			runtime.Set("2", partIconName.FormatToSpriteNameRichText());
			runtime.Set("1", GlobalConfig.Instance.FlawAddDamagePercent.ToString());
			runtime.Set("0", count.ToString());
			return arg;
		}
	}
}
