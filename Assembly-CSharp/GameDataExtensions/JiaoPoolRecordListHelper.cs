using System;
using System.Runtime.CompilerServices;
using Config;
using GameData.DLC.FiveLoong;
using GameData.Utilities;

namespace GameDataExtensions
{
	// Token: 0x020006D7 RID: 1751
	public static class JiaoPoolRecordListHelper
	{
		// Token: 0x0600536F RID: 21359 RVA: 0x0026B554 File Offset: 0x00269754
		[return: TupleElementNames(new string[]
		{
			"name",
			"content"
		})]
		public static ValueTuple<string, string> DecodeRecord(this JiaoPoolRecordList list, int index, JiaoPoolRecordArgumentCollection argumentCollection)
		{
			bool flag = !list.Collection.CheckIndex(index);
			ValueTuple<string, string> result;
			if (flag)
			{
				result = new ValueTuple<string, string>(string.Empty, string.Empty);
			}
			else
			{
				JiaoPoolRecord record = list.Collection[index];
				JiaoRecordItem config = JiaoRecord.Instance.GetItem(record.RecordTemplateId);
				string name = config.Name;
				string content = config.Desc;
				bool flag2 = config.Parameters != null && config.Parameters.Length != 0;
				if (flag2)
				{
					object[] fillParams = new object[config.Parameters.Length];
					int i = 0;
					int max = config.Parameters.Length;
					while (i < max)
					{
						string text = config.Parameters[i];
						string text2 = text;
						uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
						if (num <= 2312977542U)
						{
							if (num != 1101079613U)
							{
								if (num != 1283547685U)
								{
									if (num == 2312977542U)
									{
										if (text2 == "Nurturance")
										{
											JiaoNurturanceItem nurturanceItem = JiaoNurturance.Instance[record.NurturanceTemplateId];
											bool flag3 = nurturanceItem != null;
											if (flag3)
											{
												fillParams[i] = nurturanceItem.Name.SetColor("pinkyellow");
											}
										}
									}
								}
								else if (text2 == "Float")
								{
									fillParams[i] = ((float)record.PropertyChangeVolume / 10000f).ToString("F1").SetColor("pinkyellow");
								}
							}
							else if (text2 == "Jiao2Name")
							{
								fillParams[i] = argumentCollection.JiaoNameMap.GetOrDefault(record.Jiao2Id).SetColor("pinkyellow");
							}
						}
						else if (num <= 3651752933U)
						{
							if (num != 2426825094U)
							{
								if (num == 3651752933U)
								{
									if (text2 == "Integer")
									{
										fillParams[i] = string.Format("{0}", (float)record.PropertyChangeVolume / 100f).SetColor("pinkyellow");
									}
								}
							}
							else if (text2 == "JiaoEggName")
							{
								MaterialItem itemConfig = Material.Instance[record.TemplateId];
								bool flag4 = itemConfig != null;
								if (flag4)
								{
									fillParams[i] = itemConfig.Name.SetColor("pinkyellow");
								}
							}
						}
						else if (num != 3871241584U)
						{
							if (num == 3894392736U)
							{
								if (text2 == "PropertyType")
								{
									JiaoPropertyItem jiaoPropertyItem = Config.JiaoProperty.Instance[record.TemplateId];
									bool flag5 = jiaoPropertyItem != null;
									if (flag5)
									{
										fillParams[i] = jiaoPropertyItem.Name.SetColor("pinkyellow");
									}
								}
							}
						}
						else if (text2 == "Jiao1Name")
						{
							fillParams[i] = argumentCollection.JiaoNameMap.GetOrDefault(record.Jiao1Id).SetColor("pinkyellow");
						}
						i++;
					}
					content = content.GetFormat(fillParams);
				}
				result = new ValueTuple<string, string>(name, content);
			}
			return result;
		}
	}
}
