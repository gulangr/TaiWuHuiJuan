using System;
using DisplayConfig;
using FrameWork;
using GameData.Domains.Adventure;
using GameData.Domains.Character;
using TMPro;
using UnityEngine;

// Token: 0x02000270 RID: 624
public class MouseTipAdventureNode : MouseTipBase
{
	// Token: 0x06002904 RID: 10500 RVA: 0x001308C4 File Offset: 0x0012EAC4
	protected override void Init(ArgumentBox argsBox)
	{
		AdventureMapPoint node;
		argsBox.Get<AdventureMapPoint>("node", out node);
		bool isArrange;
		argsBox.Get("isArrange", out isArrange);
		bool isCancel;
		argsBox.Get("isCancel", out isCancel);
		bool interactable;
		argsBox.Get("interactable", out interactable);
		bool costIsMeet;
		argsBox.Get("costIsMeet", out costIsMeet);
		bool isOnBranch;
		argsBox.Get("isOnBranch", out isOnBranch);
		bool isOnPath;
		argsBox.Get("isOnPath", out isOnPath);
		string title;
		argsBox.Get("title", out title);
		base.CGet<TextMeshProUGUI>("Name").text = title;
		sbyte elementType = isArrange ? node.SevenElementType : 6;
		Refers add = base.CGet<Refers>("Add");
		Refers reduce = base.CGet<Refers>("Reduce");
		reduce.gameObject.SetActive(true);
		PersonalityItem personalityCfg = Personality.Instance[(int)elementType];
		reduce.CGet<CImage>("Icon").SetSprite(personalityCfg.Icon, false, null);
		string reduceText = isCancel ? string.Format("+{0}", node.SevenElementCost).SetColor("brightblue") : string.Format("-{0}", node.SevenElementCost).SetColor("brightred");
		reduceText = personalityCfg.Name + reduceText;
		reduce.CGet<TextMeshProUGUI>("Text").SetText(reduceText, true);
		TextMeshProUGUI moreDesc = base.CGet<TextMeshProUGUI>("MoreDesc");
		GameObject descriptionHolder = base.CGet<GameObject>("DescriptionHolder");
		bool flag = isArrange;
		if (flag)
		{
			descriptionHolder.SetActive(true);
			PersonalityItem addPersonalityCfg = Personality.Instance[(int)PersonalityType.Producing[(int)node.SevenElementType]];
			add.CGet<CImage>("Icon").SetSprite(addPersonalityCfg.Icon, false, null);
			string addText = isCancel ? string.Format("-{0}", node.SevenElementCost).SetColor("brightred") : string.Format("+{0}", node.SevenElementCost).SetColor("brightblue");
			addText = addPersonalityCfg.Name + addText;
			add.CGet<TextMeshProUGUI>("Text").SetText(addText, true);
			add.gameObject.SetActive(true);
			base.CGet<TextMeshProUGUI>("Desc").text = LocalStringManager.Get(LanguageKey.LK_Adventure_ArrangeNode_Tip);
			moreDesc.gameObject.SetActive(false);
		}
		else
		{
			add.gameObject.SetActive(false);
			bool showState = interactable || !costIsMeet;
			descriptionHolder.SetActive(showState);
			base.CGet<TextMeshProUGUI>("Desc").text = LocalStringManager.Get(LanguageKey.LK_Adventure_PerceiveNode_Tip2);
			bool flag2 = !showState;
			if (flag2)
			{
				bool flag3 = isOnBranch;
				if (flag3)
				{
					moreDesc.text = LocalStringManager.Get(LanguageKey.LK_Adventure_PerceiveNode_Tip3).ColorReplace();
				}
				else
				{
					bool flag4 = !isOnPath;
					if (flag4)
					{
						moreDesc.text = LocalStringManager.Get(LanguageKey.LK_Adventure_PerceiveNode_NotOnPath).ColorReplace();
					}
					else
					{
						moreDesc.text = LocalStringManager.Get(LanguageKey.LK_Adventure_PerceiveNode_NoNeed).ColorReplace();
					}
				}
			}
			moreDesc.gameObject.SetActive(!showState);
		}
	}
}
