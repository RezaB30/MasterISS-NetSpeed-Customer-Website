using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CustomerManagementSystem.Helper
{
    public partial class PageBuilder
    {
        public static string TariffPagePartial(IEnumerable<TariffPageBuilderModel> tariffPageBuilder)
        {
            if (tariffPageBuilder == null || tariffPageBuilder.Count() == 0)
            {
                return string.Empty;
            }
            TagBuilder row = new TagBuilder("div");
            row.AddCssClass("row");
            foreach (var item in tariffPageBuilder)
            {
                var col = new TagBuilder("div");
                col.AddCssClass("col-xl-12 ml-2");
                var cardInCol = new TagBuilder("div");
                cardInCol.AddCssClass("card card-custom gutter-b");
                cardInCol.MergeAttribute("style", "background:#ffffff; box-shadow: -10px 68px 106px -16px rgba(0,0,0,0.37);-moz-box-shadow: -13px 68px 106px -16px rgba(0,0,0,0.37);box-shadow: 1px 4px 4px 0px rgb(0 0 0 / 14%)");
                var cardBodyInCard = new TagBuilder("div");
                cardBodyInCard.AddCssClass("card-body d-flex align-items-center py-0 mt-6");
                var cardBodyColumnInCardBody = new TagBuilder("div");
                cardBodyColumnInCardBody.AddCssClass("d-flex flex-column flex-grow-1 py-2 py-lg-0");
                var labelInCardBodyColumn = new TagBuilder("label");
                labelInCardBodyColumn.AddCssClass("option option option-plain");
                var optionTariff = new StringBuilder();
                optionTariff.Append($"<span class='option-control'>" +
                    $"<span class='radio radio-info radio-lg mb-8'>" +
                    $"<input type='radio' name='ServiceID' id='ServiceID' value='{item.Value}'>" +
                    $"<span></span>" +
                    $"</span></span>");
                optionTariff.Append($"<span class='option-label'>" +
                    $"<span class='option-head'>" +
                    $"<span class='option-title font-size-h6 font-weight-bolder m-1 text-dark'>{item.Name}</span>" +
                    $"</span>" +
                    $"<span class='text-dark-50 font-weight font-size-h6 mt5 option-body text-left'>{item.TariffType}</span>" +
                    $"</span>");

                labelInCardBodyColumn.InnerHtml = optionTariff.ToString();
                cardBodyColumnInCardBody.InnerHtml = labelInCardBodyColumn.ToString();
                cardBodyInCard.InnerHtml = cardBodyColumnInCardBody.ToString();
                cardInCol.InnerHtml = cardBodyInCard.ToString();
                col.InnerHtml = cardInCol.ToString();
                row.InnerHtml += col.ToString();
            }
            return row.ToString();
        }
    }
    public class TariffPageBuilderModel
    {
        public string TariffType { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}