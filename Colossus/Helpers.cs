using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Colossus.RandomVariables;

namespace Colossus
{
    public static class Helpers
    {        
        public static ExperienceOverride.Builder When(this VisitGroup group, ExperienceFactor factor, int level)
        {
            return new ExperienceOverride.Builder(group).And(factor, level);
        }

        public static ExperienceOverride.Builder When(this VisitGroup group, string factorName, int level)
        {
            return new ExperienceOverride.Builder(group).And(factorName, level);
        }

        public static ExperienceOverride.Builder When(this VisitGroup group, string factorName, string levelName)
        {
            return new ExperienceOverride.Builder(group).And(factorName, levelName);
        }

        
        public static VisitGroup Override(this VisitGroup group, VisitGroup baseGroup)
        {
            group.BaseGroup = baseGroup;
            return group;
        }

        

        public static VisitGroup AddVariables(this VisitGroup group, params IRandomVariable[] vars)
        {
            group.Variables.AddRange(vars);
            return group;
        }


        public static HashSet<T> ToSet<T>(this IEnumerable<T> values)
        {
            return new HashSet<T>(values);
        }


        public static void ForEach<TValue>(this IEnumerable<TValue> values, Action<TValue> action)
        {
            foreach (var v in values) action(v);
        }

        public static void ForEach<TValue>(this IEnumerable<TValue> values, Action<TValue, int> action)
        {
            var i = 0;
            foreach (var v in values) action(v, i++);
        }


        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
            TValue def = default(TValue))
        {
            TValue v;
            return dict.TryGetValue(key, out v) ? v : def;
        }

        public static TValue TryGetValue<TObject, TValue>(this TObject o, Func<TObject, TValue> getter, TValue defaultValue = default(TValue))
            where TObject : class
        {
            return o != null ? getter(o) : defaultValue;
        }

        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return dict.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public static bool Contains<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> other, bool equals = false)
        {
            TValue val;
            return other.All(kv => dict.TryGetValue(kv.Key, out val) && val.Equals(kv.Value)) && (!equals || dict.Count == other.Count());
        }

        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> target,
            IDictionary<TKey, TValue> newValues, bool overwrite = false)
        {
            foreach (var val in newValues)
            {
                if (overwrite || !target.ContainsKey(val.Key))
                {
                    target[val.Key] = val.Value;
                }
            }
        }

        public static Dictionary<TKey, TValue> ToDictionary<TItem, TKey, TValue>(
            this IEnumerable<TItem> items, Func<TItem, int, TKey> key, Func<TItem, int, TValue> val)
        {
            var dict = new Dictionary<TKey, TValue>();
            items.ForEach((item, i)=>dict.Add(key(item, i), val(item, i)));
            return dict;
        }

        public static string Format(this KeyValuePair<ExperienceFactor, int> factorLevel, bool includeName = true)
        {
            var sb = new StringBuilder();
            if (includeName) sb.Append(factorLevel.Key.Name).Append(":");
            sb.Append(factorLevel.Key.Levels[factorLevel.Value]);
            return sb.ToString();
        }



        /// <summary>
        /// Sorts a list of values by dependencies
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="values"></param>
        /// <param name="dependencyCheck">Function of item and set of non sorted items. If the item dependes on any other item in the working set this function must return false</param>
        /// <returns></returns>
        public static List<TValue> TopologicalSort<TValue>(this IEnumerable<TValue> values, Func<TValue, ISet<TValue>, bool> dependencyCheck)
        {
            var workingSet = new HashSet<TValue>(values);
            var sorted = new List<TValue>(workingSet.Count);
            while (workingSet.Count > 0)
            {
                var free = workingSet.FirstOrDefault(x => dependencyCheck(x, workingSet));
                if (free == null)
                {
                    throw new InvalidOperationException("Cyclic dependency detected");
                }
                sorted.Add(free);
                workingSet.Remove(free);
            }

            return sorted;
        }


        public static void WriteTsvLine(this StreamWriter writer, params object[] values)
        {
            writer.WriteLine(string.Join("\t", values.Select(v=>v.ToString())));
        }
    }    
}
