using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Config;
using GameData.Utilities;

namespace AiEditor
{
	// Token: 0x02000664 RID: 1636
	public class AiBlueprintSnapshot
	{
		// Token: 0x06004DB9 RID: 19897 RVA: 0x0024A0CE File Offset: 0x002482CE
		public AiBlueprintSnapshot()
		{
			this.Data = new List<AiNodeDataSnapshot>();
		}

		// Token: 0x06004DBA RID: 19898 RVA: 0x0024A0ED File Offset: 0x002482ED
		public AiBlueprintSnapshot(IEnumerable<AiNodeDataSnapshot> data)
		{
			this.Data = new List<AiNodeDataSnapshot>(data);
		}

		// Token: 0x06004DBB RID: 19899 RVA: 0x0024A110 File Offset: 0x00248310
		public void Save(string path)
		{
			using (FileStream stream = new FileStream(path, FileMode.Create))
			{
				using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
				{
					writer.NewLine = "\n";
					writer.WriteLine("Ai Blueprint file v" + this.Version);
					for (int i = 0; i < this.Data.Count; i++)
					{
						AiNodeDataSnapshot node = this.Data[i];
						Tester.Assert(i == node.RuntimeId, string.Format("{0} is not equal {1}", i, node.RuntimeId));
						writer.WriteLine(string.Format("#NODEBEGIN - {0}", i));
						writer.WriteLine(string.Format("  - runtimeId: {0}", node.RuntimeId));
						writer.WriteLine(string.Format("  - posX: {0}", node.Position.x));
						writer.WriteLine(string.Format("  - posY: {0}", node.Position.y));
						writer.WriteLine(string.Format("  - type: {0}", node.Type));
						writer.WriteLine("  - ids: [" + string.Join<int>(',', node.Ids) + "]");
						writer.WriteLine("  - subTypes: [" + string.Join<int>(',', node.SubTypes) + "]");
						writer.WriteLine(string.Format("  - params: {0}", node.Params.Count));
						for (int pi = 0; pi < node.Params.Count; pi++)
						{
							List<string> param = node.Params[pi];
							writer.WriteLine(string.Format("#PARAMBEGIN - {0}", pi));
							foreach (string str in param)
							{
								writer.WriteLine(str.Replace("\n", "<NL>"));
							}
							writer.WriteLine(string.Format("#PARAMEND - {0}", pi));
						}
						writer.WriteLine(string.Format("#NODEEND - {0}", i));
					}
				}
			}
		}

		// Token: 0x06004DBC RID: 19900 RVA: 0x0024A3D4 File Offset: 0x002485D4
		public bool Load(string path)
		{
			bool result;
			using (FileStream stream = new FileStream(path, FileMode.Open))
			{
				using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
				{
					string versionStr = reader.ReadLine() ?? string.Empty;
					bool flag = !versionStr.StartsWith("Ai Blueprint file v");
					if (flag)
					{
						result = false;
					}
					else
					{
						Match versionMatch = AiBlueprintSnapshot.VersionRegex.Match(versionStr.Replace("Ai Blueprint file v", ""));
						bool flag2 = !versionMatch.Success;
						if (flag2)
						{
							result = false;
						}
						else
						{
							this.Version = versionMatch.Groups[0].Value;
							bool flag3 = !AiBlueprintUpdaterHelper.CanUpdate(this.Version);
							if (flag3)
							{
								result = false;
							}
							else
							{
								AiNodeDataSnapshot node = null;
								List<string> param = null;
								while (!reader.EndOfStream)
								{
									string line = reader.ReadLine() ?? string.Empty;
									bool flag4 = line.StartsWith("//");
									if (!flag4)
									{
										bool flag5 = line.StartsWith("#NODEBEGIN");
										if (flag5)
										{
											node = new AiNodeDataSnapshot();
										}
										bool flag6 = line.StartsWith("#NODEEND");
										if (flag6)
										{
											Tester.Assert(node != null, "");
											this.Data.Add(node);
											node = null;
										}
										bool flag7 = line.StartsWith("#PARAMBEGIN");
										if (flag7)
										{
											param = new List<string>();
										}
										else
										{
											bool flag8 = line.StartsWith("#PARAMEND");
											if (flag8)
											{
												Tester.Assert(param != null, "");
												node.Params.Add(param);
												param = null;
											}
											bool flag9 = param != null;
											if (flag9)
											{
												param.Add(line.Replace("<NL>", "\n"));
											}
											else
											{
												bool flag10 = line.StartsWith("  - runtimeId: ");
												if (flag10)
												{
													node.RuntimeId = int.Parse(line.Replace("  - runtimeId: ", ""));
												}
												bool flag11 = line.StartsWith("  - posX: ");
												if (flag11)
												{
													node.Position = node.Position.SetX(float.Parse(line.Replace("  - posX: ", "")));
												}
												bool flag12 = line.StartsWith("  - posY: ");
												if (flag12)
												{
													node.Position = node.Position.SetY(float.Parse(line.Replace("  - posY: ", "")));
												}
												bool flag13 = line.StartsWith("  - type: ");
												if (flag13)
												{
													node.Type = int.Parse(line.Replace("  - type: ", ""));
												}
												bool flag14 = line.StartsWith("  - ids: [") && line.EndsWith("]");
												if (flag14)
												{
													node.Ids.AddRange((from x in line.Replace("  - ids: [", "").Replace("]", "").Split(',', StringSplitOptions.None)
													where !x.IsNullOrEmpty()
													select x).Select(new Func<string, int>(int.Parse)));
												}
												bool flag15 = line.StartsWith("  - subTypes: [") && line.EndsWith("]");
												if (flag15)
												{
													node.SubTypes.AddRange((from x in line.Replace("  - subTypes: [", "").Replace("]", "").Split(',', StringSplitOptions.None)
													where !x.IsNullOrEmpty()
													select x).Select(new Func<string, int>(int.Parse)));
												}
											}
										}
									}
								}
								result = AiBlueprintUpdaterHelper.Update(this.Version, this);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06004DBD RID: 19901 RVA: 0x0024A7E8 File Offset: 0x002489E8
		public void Export(string path)
		{
			using (FileStream stream = new FileStream(path, FileMode.Create))
			{
				using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8))
				{
					List<AiNodeDataSnapshot> conditions = new List<AiNodeDataSnapshot>();
					List<AiNodeDataSnapshot> actions = new List<AiNodeDataSnapshot>();
					int conditionCount = 0;
					int actionCount = 0;
					writer.Write(this.Data.Count);
					for (int i = 0; i < this.Data.Count; i++)
					{
						AiNodeDataSnapshot node = this.Data[i];
						writer.Write(node.Type);
						node.ParseMapping(ref conditionCount, ref actionCount);
						bool flag = node.Type == 1;
						if (flag)
						{
							conditions.Add(node);
						}
						bool flag2 = node.Type == 2;
						if (flag2)
						{
							actions.Add(node);
						}
						writer.Write(node.Ids.Count);
						foreach (int id in node.Ids)
						{
							writer.Write(id);
						}
					}
					writer.Write(conditionCount);
					AiBlueprintSnapshot.ExportParam(conditions, writer);
					writer.Write(actionCount);
					AiBlueprintSnapshot.ExportParam(actions, writer);
				}
			}
		}

		// Token: 0x06004DBE RID: 19902 RVA: 0x0024A988 File Offset: 0x00248B88
		private static void ExportParam(IEnumerable<AiNodeDataSnapshot> dataList, BinaryWriter writer)
		{
			foreach (AiNodeDataSnapshot data in dataList)
			{
				for (int i = 0; i < data.SubTypes.Count; i++)
				{
					int type = data.SubTypes[i];
					List<string> param = data.Params[i];
					writer.Write(type);
					int paramIndex = 0;
					IReadOnlyList<int> strings;
					IReadOnlyList<int> ints;
					AiBlueprintSnapshot.AnalysisParam(data.Type, type, out strings, out ints);
					bool flag = strings != null;
					if (flag)
					{
						for (int pi = 0; pi < strings.Count; pi++)
						{
							writer.Write(AiParam.Instance[strings[pi]].ConvertString(param[paramIndex++]));
						}
					}
					bool flag2 = ints != null;
					if (flag2)
					{
						for (int pi2 = 0; pi2 < ints.Count; pi2++)
						{
							writer.Write(AiParam.Instance[ints[pi2]].ConvertInt(param[paramIndex++]));
						}
					}
				}
			}
		}

		// Token: 0x06004DBF RID: 19903 RVA: 0x0024AAE4 File Offset: 0x00248CE4
		private static void AnalysisParam(int type, int subType, out IReadOnlyList<int> paramStrings, out IReadOnlyList<int> paramInts)
		{
			bool flag = type == 1;
			if (flag)
			{
				AiConditionItem config = AiCondition.Instance[subType];
				IReadOnlyList<int> readOnlyList = config.ParamStrings;
				IReadOnlyList<int> readOnlyList2 = config.ParamInts;
				paramStrings = readOnlyList;
				paramInts = readOnlyList2;
			}
			else
			{
				bool flag2 = type == 2;
				if (flag2)
				{
					AiActionItem config2 = AiAction.Instance[subType];
					IReadOnlyList<int> readOnlyList2 = config2.ParamStrings;
					IReadOnlyList<int> readOnlyList = config2.ParamInts;
					paramStrings = readOnlyList2;
					paramInts = readOnlyList;
				}
				else
				{
					paramStrings = null;
					paramInts = null;
				}
			}
		}

		// Token: 0x040035EB RID: 13803
		public string Version = AiBlueprintUpdaterHelper.CurrentVersion;

		// Token: 0x040035EC RID: 13804
		public static readonly Regex VersionRegex = new Regex("^(\\d+)\\.(\\d+)\\.(\\d+)$", RegexOptions.Compiled);

		// Token: 0x040035ED RID: 13805
		public readonly List<AiNodeDataSnapshot> Data;
	}
}
