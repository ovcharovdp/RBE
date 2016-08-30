using CoreWeb.Models.Catalog;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CoreWeb.Components.kendo
{
    /// <summary>
    /// Пространство имен для реализации расширений компонентов Kendo UI в системе.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    class NamespaceDoc { }

    /// <summary>
    /// Класс для реализации дополнительных функций формирования разметки HTML библиотеки Kendo
    /// </summary>
    public static class KendoHelpers
    {
        /// <summary>
        /// Формирует строку для описания полей компонента Grid
        /// </summary>
        /// <param name="helper">Контекст вызова</param>
        /// <param name="columns">Список полей</param>
        /// <returns>HTML-строка</returns>
        public static MvcHtmlString KendoGridColumns(this HtmlHelper helper, List<ColumnModel> columns)
        {
            if (columns == null) return MvcHtmlString.Empty;
            var r = columns.Select(p => "{" + string.Format("field:\"{0}\",title:\"{1}\"{2}{3}{4}{5}{6}{7}", p.Name, p.Title,
                p.Type.Equals("date") ? ",type:\"date\",format:\"{0:" + (string.IsNullOrEmpty(p.Format) ? "dd.MM.yyyy" : p.Format) + "}\"" : "",
                p.Values == null ? string.Empty : ",values:[" + string.Join(",", p.Values.Select(v => "{value:" + v.ID + ",text:\"" + v.Name + "\"}")) + "]",
                !p.Filterable ? ",filterable:false" : p.Type.Equals("date") ? ",filterable:{extra:true}" : string.IsNullOrEmpty(p.UI) ? "" : ",filterable:{ui:\"" + p.UI + "\"}",

                !string.IsNullOrEmpty(p.Template) ? string.Format(",template:\"{0}\"", p.Template) :
                (p.Type.Equals("boolean") ? ",template:\"# if(" + p.Name + "){# ✓ #} else { # &nbsp; # } #\"" : string.Empty),

                !p.Sortable ? ",sortable:false" : "",
                p.Width > 0 ? ",width:" + p.Width.ToString() : (p.Type.Equals("date") ? ",width:150" : "")
                ) + "}");
            return MvcHtmlString.Create(string.Join(",", r));
        }
        /// <summary>
        /// Формирует строку для описания полей модели компонента Grid
        /// </summary>
        /// <param name="helper">Контекст вызова</param>
        /// <param name="columns">Список полей</param>
        /// <returns>Строка</returns>
        public static MvcHtmlString KendoModelFields(this HtmlHelper helper, List<ColumnModel> columns)
        {
            if (columns == null) return MvcHtmlString.Empty;
            var r = columns.Where(p => !p.Type.Equals("string")).Select(p => "\"" + p.Name + "\":{type:\"" + p.Type + "\"}");
            return MvcHtmlString.Create(string.Join(",", r));
        }
    }
}
