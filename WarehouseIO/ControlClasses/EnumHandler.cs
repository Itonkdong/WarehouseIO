using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WarehouseIO.Models;

namespace WarehouseIO.ControlClasses
{
    public class EnumHandler
    {
        public static List<T> GetAllEnumValues<T>()
        {
            return new List<T>((T[])Enum.GetValues(typeof(T)));
        }

        public static T GetValue<T>(string enumString)
        {
            return (T)Enum.Parse(typeof(T), enumString);
        }

        public static string GetDisplayName(Enum enumValue)
        {
            DisplayAttribute displayAttribute = enumValue.GetType()
                .GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;

            return displayAttribute?.Name ?? enumValue.ToString();
        }

        public static SelectList GetSelectList<T>(List<T> enumValues) where T : Enum
        {
            List<SelectListItem> items = enumValues
                .Select(enumValue => new SelectListItem()
                {
                    Value = enumValue.ToString(),
                    Text = EnumHandler.GetDisplayName(enumValue)
                }).ToList();

            return new SelectList(items, "Value", "Text");
        }
    }
}