using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameData.Utilities;

namespace AiEditor
{
	// Token: 0x02000666 RID: 1638
	public static class AiBlueprintUpdaterHelper
	{
		// Token: 0x06004DC4 RID: 19908 RVA: 0x0024AB68 File Offset: 0x00248D68
		static AiBlueprintUpdaterHelper()
		{
			Type updaterInterface = typeof(IAiBlueprintUpdater);
			foreach (Type type in typeof(AiBlueprintUpdaterHelper).Assembly.GetTypes())
			{
				bool flag = !type.GetInterfaces().Contains(updaterInterface) || !type.GetConstructors().Any(new Func<ConstructorInfo, bool>(AiBlueprintUpdaterHelper.NoParameters));
				if (!flag)
				{
					IAiBlueprintUpdater updater = (IAiBlueprintUpdater)Activator.CreateInstance(type);
					AiBlueprintUpdaterHelper.Updaters.GetOrNew(updater.FromVersion).Add(updater);
				}
			}
		}

		// Token: 0x06004DC5 RID: 19909 RVA: 0x0024AC24 File Offset: 0x00248E24
		private static bool BfsFindUpdatePath(string fromVersion)
		{
			AiBlueprintUpdaterHelper.UpdatePath.Clear();
			Queue<string> queue = new Queue<string>();
			HashSet<string> visit = new HashSet<string>();
			Dictionary<string, IAiBlueprintUpdater> parent = new Dictionary<string, IAiBlueprintUpdater>();
			queue.Enqueue(fromVersion);
			visit.Add(fromVersion);
			Func<IAiBlueprintUpdater, bool> <>9__0;
			while (queue.Count > 0)
			{
				string version = queue.Dequeue();
				bool flag = version == AiBlueprintUpdaterHelper.CurrentVersion;
				if (flag)
				{
					while (version != fromVersion)
					{
						IAiBlueprintUpdater updater = parent[version];
						version = updater.FromVersion;
						AiBlueprintUpdaterHelper.UpdatePath.Add(updater);
					}
					AiBlueprintUpdaterHelper.UpdatePath.Reverse();
					return true;
				}
				List<IAiBlueprintUpdater> updaters;
				bool flag2 = AiBlueprintUpdaterHelper.Updaters.TryGetValue(version, out updaters);
				if (flag2)
				{
					IEnumerable<IAiBlueprintUpdater> source = updaters;
					Func<IAiBlueprintUpdater, bool> predicate;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = ((IAiBlueprintUpdater x) => !visit.Contains(x.ToVersion)));
					}
					foreach (IAiBlueprintUpdater updater2 in source.Where(predicate))
					{
						queue.Enqueue(updater2.ToVersion);
						visit.Add(updater2.ToVersion);
						parent[updater2.ToVersion] = updater2;
					}
				}
			}
			return false;
		}

		// Token: 0x06004DC6 RID: 19910 RVA: 0x0024AD98 File Offset: 0x00248F98
		public static bool CanUpdate(string version)
		{
			return AiBlueprintUpdaterHelper.BfsFindUpdatePath(version);
		}

		// Token: 0x06004DC7 RID: 19911 RVA: 0x0024ADB0 File Offset: 0x00248FB0
		public static bool Update(string version, AiBlueprintSnapshot blueprint)
		{
			bool flag = !AiBlueprintUpdaterHelper.BfsFindUpdatePath(version);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (IAiBlueprintUpdater updater in AiBlueprintUpdaterHelper.UpdatePath)
				{
					Tester.Assert(updater.FromVersion == blueprint.Version, "");
					updater.Update(blueprint);
					blueprint.Version = updater.ToVersion;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06004DC8 RID: 19912 RVA: 0x0024AE48 File Offset: 0x00249048
		private static bool NoParameters(ConstructorInfo arg)
		{
			ParameterInfo[] parameters = arg.GetParameters();
			return parameters == null || parameters.Length <= 0;
		}

		// Token: 0x040035EE RID: 13806
		public static readonly string CurrentVersion = "1.0.6";

		// Token: 0x040035EF RID: 13807
		private static readonly Dictionary<string, List<IAiBlueprintUpdater>> Updaters = new Dictionary<string, List<IAiBlueprintUpdater>>();

		// Token: 0x040035F0 RID: 13808
		private static readonly List<IAiBlueprintUpdater> UpdatePath = new List<IAiBlueprintUpdater>();
	}
}
