using System;
using System.Collections.Generic;
using System.Diagnostics;
using Config;
using FrameWork;
using Game.Views.MouseTips;
using Game.Views.TipsBuilders;
using UnityEngine;

// Token: 0x0200040D RID: 1037
public static class CommonTipsHelper
{
	// Token: 0x17000652 RID: 1618
	// (get) Token: 0x06003DE3 RID: 15843 RVA: 0x001F12E4 File Offset: 0x001EF4E4
	public static Dictionary<int, CommonTipsBuilderInfo> BuilderCollection
	{
		get
		{
			bool flag = CommonTipsHelper._builderCollection == null;
			if (flag)
			{
				CommonTipsHelper._builderCollection = CommonTipsBuilderCollection.CreateBuilderCollection();
			}
			return CommonTipsHelper._builderCollection;
		}
	}

	// Token: 0x17000653 RID: 1619
	// (get) Token: 0x06003DE4 RID: 15844 RVA: 0x001F1314 File Offset: 0x001EF514
	public static Dictionary<TipType, int> ReplaceTipTypeToTemplateIdMap
	{
		get
		{
			bool flag = CommonTipsHelper._replaceTipTypeToTemplateIdMap == null;
			if (flag)
			{
				CommonTipsHelper._replaceTipTypeToTemplateIdMap = CommonTipsBuilderCollection.CreateReplaceTipTypeToTemplateIdMap();
			}
			return CommonTipsHelper._replaceTipTypeToTemplateIdMap;
		}
	}

	// Token: 0x06003DE5 RID: 15845 RVA: 0x001F1344 File Offset: 0x001EF544
	public static ArgumentBox BuildCommonTipArg(CommonTipItem tipDef, ArgumentBox arg, TooltipInvoker source = null)
	{
		CommonTipsBuilderInfo builderInfo;
		bool flag = !CommonTipsHelper.BuilderCollection.TryGetValue(tipDef.TemplateId, out builderInfo);
		ArgumentBox result;
		if (flag)
		{
			Debug.LogError("[CommonTip] No CommonTipBuilder for tipItem " + tipDef.Path, source);
			result = arg;
		}
		else
		{
			bool flag2 = builderInfo.Builder != null;
			if (flag2)
			{
				arg = builderInfo.Builder.BuildTip(tipDef, arg);
			}
			else
			{
				bool flag3 = builderInfo.BuildFunc != null;
				if (!flag3)
				{
					Debug.LogError("[CommonTip] BuilderInfo for tipItem " + tipDef.Path + " has no valid builder", source);
					return arg;
				}
				arg = builderInfo.BuildFunc(tipDef, arg);
			}
			result = arg;
		}
		return result;
	}

	// Token: 0x06003DE6 RID: 15846 RVA: 0x001F13F0 File Offset: 0x001EF5F0
	public static ArgumentBox BuildCommonTipArg(int templateId, ArgumentBox arg, TooltipInvoker source = null)
	{
		CommonTipItem tipItem = CommonTip.Instance.GetItem(templateId);
		bool flag = tipItem == null;
		if (flag)
		{
			throw new ArgumentException(string.Format("[CommonTip] No CommonTipItem found for templateId {0}", templateId));
		}
		return CommonTipsHelper.BuildCommonTipArg(tipItem, arg, source);
	}

	// Token: 0x06003DE7 RID: 15847 RVA: 0x001F1438 File Offset: 0x001EF638
	public static ArgumentBox TryParseArgToCommonTipsArg(ArgumentBox arg, TipType sourceTipType, TooltipInvoker source = null)
	{
		bool isParseDisable = CommonTipsHelper.IsParseDisable;
		ArgumentBox result;
		if (isParseDisable)
		{
			result = arg;
		}
		else
		{
			int templateId;
			bool flag = !CommonTipsHelper.ReplaceTipTypeToTemplateIdMap.TryGetValue(sourceTipType, out templateId);
			if (flag)
			{
				result = arg;
			}
			else
			{
				result = CommonTipsHelper.BuildCommonTipArg(templateId, arg, source);
			}
		}
		return result;
	}

	// Token: 0x06003DE8 RID: 15848 RVA: 0x001F147C File Offset: 0x001EF67C
	public static bool IsCommonTipArg(ArgumentBox arg, int? tipDefKey = null)
	{
		bool isParseDisable = CommonTipsHelper.IsParseDisable;
		bool result;
		if (isParseDisable)
		{
			result = false;
		}
		else
		{
			CommonTipItem item = CommonTipsHelper.GetCommonTipItem(arg);
			bool flag;
			if (item != null)
			{
				if (tipDefKey != null)
				{
					int templateId = item.TemplateId;
					int? num = tipDefKey;
					flag = (templateId == num.GetValueOrDefault() & num != null);
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = false;
			}
			result = flag;
		}
		return result;
	}

	// Token: 0x06003DE9 RID: 15849 RVA: 0x001F14D0 File Offset: 0x001EF6D0
	public static CommonTipItem GetCommonTipItem(ArgumentBox arg)
	{
		CommonTipBaseRuntime runtime;
		bool flag = arg != null && arg.Get<CommonTipBaseRuntime>("Runtime", out runtime);
		CommonTipItem result;
		if (flag)
		{
			result = runtime.ConfigLine;
		}
		else
		{
			result = null;
		}
		return result;
	}

	// Token: 0x06003DEA RID: 15850 RVA: 0x001F1504 File Offset: 0x001EF704
	public static CommonTipSimpleRuntime GetOrCreateSimpleRuntimeTipForBuild(CommonTipItem tipDef, ArgumentBox arg)
	{
		CommonTipSimpleRuntime runtime;
		bool flag = !arg.Get<CommonTipSimpleRuntime>("Runtime", out runtime);
		if (flag)
		{
			runtime = tipDef.BuildSimple();
			arg.Set("Runtime", runtime);
		}
		else
		{
			bool flag2 = runtime.ConfigLine.TemplateId != tipDef.TemplateId;
			if (flag2)
			{
				Debug.LogWarning(string.Format("[CommonTip] Runtime's tipDef {0} does not match commonTipItem's tipDef {1}, will create new runtime", runtime.ConfigLine, tipDef.Path));
				runtime = tipDef.BuildSimple();
				arg.Set("Runtime", runtime);
			}
		}
		return runtime;
	}

	// Token: 0x06003DEB RID: 15851 RVA: 0x001F1590 File Offset: 0x001EF790
	[Conditional("UNITY_EDITOR")]
	public static void OnShowTipsDebug(TooltipInvoker mouseTipDisplayer)
	{
		CommonTipItem tipItem = CommonTipsHelper.GetCommonTipItem(mouseTipDisplayer.RuntimeParam);
		bool flag = tipItem != null;
		if (flag)
		{
			Debug.Log(string.Format("[PBTest][CommonTip][ShowDebug] <color=blue>Show CommonTip for TipType {0} with RuntimeParam {1}</color>", mouseTipDisplayer.Type, tipItem.Path), mouseTipDisplayer);
		}
		else
		{
			TipType displayerType = mouseTipDisplayer.Type;
			int templateId;
			bool flag2 = !CommonTipsHelper.ReplaceTipTypeToTemplateIdMap.TryGetValue(displayerType, out templateId);
			if (flag2)
			{
				Debug.Log(string.Format("[PBTest][CommonTip][ShowDebug] No replace template for TipType {0}", displayerType), mouseTipDisplayer);
			}
			else
			{
				CommonTipsBuilderInfo builderInfo;
				bool flag3 = !CommonTipsHelper.BuilderCollection.TryGetValue(templateId, out builderInfo);
				if (flag3)
				{
					Debug.LogWarning(string.Format("[PBTest][CommonTip][ShowDebug] No CommonTipBuilder for templateId {0}", templateId), mouseTipDisplayer);
				}
			}
		}
	}

	// Token: 0x06003DEC RID: 15852 RVA: 0x001F1640 File Offset: 0x001EF840
	public static string FormatToSpriteNameRichText(this string spName)
	{
		return "<SpName=" + spName + ">";
	}

	// Token: 0x06003DED RID: 15853 RVA: 0x001F1664 File Offset: 0x001EF864
	public static CommonTipSimpleRuntime SetAtomVisible(this CommonTipSimpleRuntime runtime, string paragraphName, string atomName, bool show)
	{
		if (show)
		{
			runtime.ShowAtom(paragraphName, atomName);
		}
		else
		{
			runtime.HideAtom(paragraphName, atomName);
		}
		return runtime;
	}

	// Token: 0x06003DEE RID: 15854 RVA: 0x001F1690 File Offset: 0x001EF890
	public static CommonTipSimpleRuntime SetParagraphVisible(this CommonTipSimpleRuntime runtime, string paragraphName, bool show)
	{
		if (show)
		{
			runtime.ShowParagraph(paragraphName);
		}
		else
		{
			runtime.HideParagraph(paragraphName);
		}
		return runtime;
	}

	// Token: 0x06003DEF RID: 15855 RVA: 0x001F16BC File Offset: 0x001EF8BC
	public static string SetColorIfNotEmpty(this string content, string color)
	{
		return string.IsNullOrEmpty(content) ? content : content.SetColor(color);
	}

	// Token: 0x04002C95 RID: 11413
	public static bool IsParseDisable;

	// Token: 0x04002C96 RID: 11414
	private static Dictionary<int, CommonTipsBuilderInfo> _builderCollection;

	// Token: 0x04002C97 RID: 11415
	private static Dictionary<TipType, int> _replaceTipTypeToTemplateIdMap;

	// Token: 0x04002C98 RID: 11416
	public const string KEY_ARG_RUNTIME = "Runtime";

	// Token: 0x04002C99 RID: 11417
	public const string KEY_ARG_RUNTIME_NEED_REFRESH = "RuntimeNeedRefresh";
}
