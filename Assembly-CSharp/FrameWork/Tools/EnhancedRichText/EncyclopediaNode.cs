using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;

namespace FrameWork.Tools.EnhancedRichText
{
	// Token: 0x0200103A RID: 4154
	public class EncyclopediaNode
	{
		// Token: 0x0600BDC2 RID: 48578 RVA: 0x005626F0 File Offset: 0x005608F0
		public TipType OnMouseOverLink(string id, ArgumentBox argBox)
		{
			int separatorIndex = id.IndexOf('_');
			string typeStr = id.Substring(0, separatorIndex);
			string val = id.Substring(separatorIndex + 1);
			bool flag = typeStr == "Simple";
			TipType result;
			if (flag)
			{
				separatorIndex = val.IndexOf('&');
				bool flag2 = separatorIndex >= 0;
				string title;
				string content;
				if (flag2)
				{
					title = val.Substring(0, separatorIndex);
					content = val.Substring(separatorIndex + 1);
					bool flag3 = title.Length > 0 && title[0] == '#';
					if (flag3)
					{
						title = LocalStringManager.Get(title.Substring(1));
					}
					bool flag4 = content.Length > 0 && content[0] == '#';
					if (flag4)
					{
						content = LocalStringManager.Get(content.Substring(1));
					}
				}
				else
				{
					ValueTuple<string, string> valueTuple = this.SimpleTipsList[int.Parse(val)];
					title = valueTuple.Item1;
					content = valueTuple.Item2;
				}
				argBox.Set("arg0", title);
				argBox.Set("arg1", content);
				result = TipType.Simple;
			}
			else
			{
				bool flag5 = typeStr == "SingleDesc";
				if (flag5)
				{
					string desc = val;
					bool flag6 = desc.Length > 0;
					if (flag6)
					{
						bool flag7 = desc[0] == '#';
						if (flag7)
						{
							desc = LocalStringManager.Get(desc.Substring(1));
						}
						else
						{
							bool flag8 = desc[1] == '$';
							if (flag8)
							{
								string[] configInfo = desc.Substring(1).Split('_', StringSplitOptions.None);
							}
						}
					}
					else
					{
						int index;
						bool flag9 = int.TryParse(desc, out index);
						if (flag9)
						{
							desc = this.SingleDescTipsList[index];
						}
					}
					argBox.Set("arg0", desc);
					result = TipType.SingleDesc;
				}
				else
				{
					ItemDisplayData displayData = new ItemDisplayData
					{
						Key = new ItemKey(-1, 0, short.Parse(val), -1),
						Amount = 1,
						Durability = 1,
						MaxDurability = 1,
						Weight = 1,
						Value = 1L,
						SpecialArg = 0,
						Requirements = new List<ValueTuple<int, int, int>>()
					};
					string text = typeStr;
					string text2 = text;
					uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
					if (num <= 2369785178U)
					{
						if (num <= 448849815U)
						{
							if (num != 377761645U)
							{
								if (num != 394544176U)
								{
									if (num == 448849815U)
									{
										if (text2 == "Carrier")
										{
											argBox.SetObject("ItemData", displayData);
											return TipType.Carrier;
										}
									}
								}
								else if (text2 == "Cricket")
								{
									return TipType.Simple;
								}
							}
							else if (text2 == "Clothing")
							{
								argBox.SetObject("ItemData", displayData);
								return TipType.Clothing;
							}
						}
						else if (num <= 2100812256U)
						{
							if (num != 1347445242U)
							{
								if (num == 2100812256U)
								{
									if (text2 == "TeaWine")
									{
										argBox.SetObject("ItemData", displayData);
										return TipType.TeaWine;
									}
								}
							}
							else if (text2 == "CombatSkill")
							{
								return TipType.Simple;
							}
						}
						else if (num != 2226667892U)
						{
							if (num == 2369785178U)
							{
								if (text2 == "Sundries")
								{
									argBox.SetObject("ItemData", displayData);
									return TipType.Misc;
								}
							}
						}
						else if (text2 == "Armor")
						{
							return TipType.Simple;
						}
					}
					else if (num <= 2575767490U)
					{
						if (num != 2556266301U)
						{
							if (num != 2565036635U)
							{
								if (num == 2575767490U)
								{
									if (text2 == "MakingTool")
									{
										argBox.SetObject("ItemData", displayData);
										return TipType.CraftTool;
									}
								}
							}
							else if (text2 == "SkillBook")
							{
								return TipType.Simple;
							}
						}
						else if (text2 == "Medicine")
						{
							argBox.SetObject("ItemData", displayData);
							return TipType.Medicine;
						}
					}
					else if (num <= 3179069417U)
					{
						if (num != 3082879841U)
						{
							if (num == 3179069417U)
							{
								if (text2 == "Food")
								{
									argBox.SetObject("ItemData", displayData);
									return TipType.Food;
								}
							}
						}
						else if (text2 == "Weapon")
						{
							return TipType.Simple;
						}
					}
					else if (num != 3419754368U)
					{
						if (num == 4160079225U)
						{
							if (text2 == "Accessory")
							{
								argBox.SetObject("ItemData", displayData);
								return TipType.Accessory;
							}
						}
					}
					else if (text2 == "Material")
					{
						argBox.SetObject("ItemData", displayData);
						return TipType.Material;
					}
					argBox.Set("arg0", "无效的LinkId");
					argBox.Set("arg1", id);
					result = TipType.Simple;
				}
			}
			return result;
		}

		// Token: 0x040091FB RID: 37371
		public string Name;

		// Token: 0x040091FC RID: 37372
		public string TextContent;

		// Token: 0x040091FD RID: 37373
		public List<string> SingleDescTipsList;

		// Token: 0x040091FE RID: 37374
		[TupleElementNames(new string[]
		{
			"title",
			"content"
		})]
		public List<ValueTuple<string, string>> SimpleTipsList;

		// Token: 0x040091FF RID: 37375
		public List<string> LearningAssistantTextList;

		// Token: 0x04009200 RID: 37376
		public List<EncyclopediaNode> Children = new List<EncyclopediaNode>();

		// Token: 0x04009201 RID: 37377
		public int Level;
	}
}
