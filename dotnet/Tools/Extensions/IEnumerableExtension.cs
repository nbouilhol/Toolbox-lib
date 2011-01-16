using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Mvc.Utilitaires.Extensions
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> itemsToMap, Func<T, string> textProperty, Func<T, string> valueProperty)
        {
            return itemsToMap.Select(itemToMap => new SelectListItem
            {
                Value = valueProperty(itemToMap),
                Text = textProperty(itemToMap)
            });
        }

        public static IEnumerable<SelectListItem> Add(this IEnumerable<SelectListItem> selectList, string text, string value)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text=text, Value=value });
            list.AddRange(selectList);
            return list;
        }

        public static IEnumerable<SelectListItem> AddLast(this IEnumerable<SelectListItem> selectList, string text, string value)
        {
            var list = new List<SelectListItem>();
            list.AddRange(selectList);
            list.Add(new SelectListItem { Text = text, Value = value });
            return list;
        }

        public static IEnumerable<SelectListItem> SelectItem(this IEnumerable<SelectListItem> selectList, string value)
        {
            foreach (var item in  selectList)
            {
                item.Selected = item.Value == value;
                yield return item;
            }
        }

        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> itemsToMap, Func<T, string> textProperty, Func<T, string> valueProperty, Predicate<T> isSelected)
        {
            return itemsToMap.Select(itemToMap => new SelectListItem
            {
                Value = valueProperty(itemToMap),
                Text = textProperty(itemToMap),
                Selected = isSelected(itemToMap)
            });
        }
    }
}
