using System;
using System.Collections.Generic;
using System.Linq;
using XLog.Formatters;

namespace XLog.Categories
{
    public class DefaultCategoryFormatter : ICategoryFormatter
    {
        private readonly LogCategoryRegistrar _categoryRegistry;
        private const string Separator = "-";

        private readonly Dictionary<long, string> _cachedOutputCategories = new Dictionary<long, string>();

        public DefaultCategoryFormatter(LogCategoryRegistrar categoryRegistry)
        {
            _categoryRegistry = categoryRegistry;
        }

        public string[] GetAsStringArray(long categories)
        {
            //TODO: Optimize. Introduce caching
            if (categories == 0)
            {
                return new string[0];
            }

            return _categoryRegistry.GetAll()
                .Where(value => (value & categories) == value)
                .Select(value => _categoryRegistry.Get(value))
                .ToArray();
        }

        public string GetString(long categories)
        {
            if (categories == 0)
            {
                return Separator;
            }

            string orderedCategories;
            lock (_cachedOutputCategories)
            {
                if (!_cachedOutputCategories.TryGetValue(categories, out orderedCategories))
                {
                    orderedCategories = CombineCategories(categories);
                    _cachedOutputCategories.Add(categories, orderedCategories);
                }
            }
            return orderedCategories;
        }

        private string CombineCategories(long categories)
        {
            int num;
            var sb = FixedStringBuilderPool.Get(out num);
            try
            {
                sb.Append(Separator);
                foreach (var value in _categoryRegistry.GetAll())
                {
                    if ((value & categories) == value)
                    {
                        sb.Append(_categoryRegistry.Get(value));
                        sb.Append(Separator);
                    }
                }

                return sb.ToString();
            }
            finally
            {
                FixedStringBuilderPool.Return(num, sb);
            }
        }
    }
}