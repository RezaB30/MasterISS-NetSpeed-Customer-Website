using CMS.ViewModels.Supports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomerManagementSystem.Helper
{
    public static class MyExtensions
    {
        //public static MvcHtmlString RequestDropDownList(string requestLabel, IEnumerable<SelectListItem> datas, RequestTypes types)
        //{
        //    //var attr = htmlAttributes != null ? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) : null;
        //    /*
        //    <div class="form-group row fv-plugins-icon-container">
        //                            <label class="col-xl-3 col-lg-3 col-form-label">
        //                                Talebiniz
        //                                Nedir? (ARIZA)
        //                            </label>
        //                            <div class="col-lg-8 col-xl-9">
        //                                <select name="timezone" class="form-control form-control-lg form-control-solid">
        //                                    <option data-offset="10800" value="1">
        //                                        ADSL Işığı
        //                                        Yanmıyor
        //                                    </option>
        //                                    <option data-offset="10800" value="3">
        //                                        Sık Sık Kopma
        //                                        Problemi
        //                                    </option>
        //                                    <option data-offset="10800" value="6">
        //                                        İnternet Işığı
        //                                        Yanmıyor
        //                                    </option>
        //                                    <option data-offset="10800" value="24">
        //                                        Bazı
        //                                        Sayfalara Erişemiyorum
        //                                    </option>
        //                                    <option data-offset="10800" value="48">
        //                                        Düşük Hız
        //                                        Alıyorum
        //                                    </option>
        //                                    <option data-offset="10800" value="48">
        //                                        Farklı Bir
        //                                        Sorunum Var
        //                                    </option>
        //                                </select>
        //                                <div class="fv-plugins-message-container"></div>
        //                            </div>
        //                        </div>
        //     */
        //    var divBuilder = new TagBuilder("div");
        //    var labelBuilder = new TagBuilder("label");
        //    var insideDiv = new TagBuilder("div");
        //    var emptyDiv = new TagBuilder("div");
        //    var selectBuilder = new TagBuilder("select");
        //    foreach (var item in datas)
        //    {
        //        var optionBuilder = new TagBuilder("option");
        //        optionBuilder.MergeAttribute("data-offset", "1080");
        //        optionBuilder.MergeAttribute("value", item.Value);
        //        optionBuilder.SetInnerText(item.Text);
        //        selectBuilder.InnerHtml += optionBuilder.ToString();
        //    }
        //    selectBuilder.MergeAttribute("")
        //}
    }
}