using CMS.ViewModels.Supports;
using RezaB.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomerManagementSystem.Helper
{
    public static class PagedListHelper
    {
        /// <summary>
        /// Makes a paging row for data tables
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="pageCount">Total number of pages in current table</param>
        /// <param name="pageNumber">Current data page</param>
        /// <returns></returns>
        public static MvcHtmlString PagedList(this HtmlHelper helper, int pageCount, int pagesLinkCount, int? pageNumber = null)
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            //retrieve page number from query string
            int routePage;
            if (!pageNumber.HasValue)
            {
                try
                {
                    routePage = Convert.ToInt32(HttpContext.Current.Request.QueryString.Get("page"));
                }
                catch (Exception)
                {
                    routePage = 0;
                }
                pageNumber = routePage;
            }
            //Makes a div wrapper around the entire element
            var wrapper = new TagBuilder("div");
            wrapper.AddCssClass("table-pages");
            //Make first, previous and ... elements
            var firstLink = new TagBuilder("a");
            firstLink.AddCssClass("btn btn-icon btn-sm btn-light-primary mr-2 my-1");
            firstLink.MergeAttribute("title", CMS.Localization.Common.First);
            firstLink.MergeAttribute("href", urlHelper.Current(new { page = 0 }, new string[] { "errorMessage" }).ToString());
            if (pageNumber == 0)
            {
                firstLink.AddCssClass("disabled");
                firstLink.MergeAttribute("href", "javascript:void(0);", true);
            }
            firstLink.InnerHtml = "<<";
            var prevLink = new TagBuilder("a");
            prevLink.AddCssClass("btn btn-icon btn-sm btn-light-primary mr-2 my-1");
            prevLink.MergeAttribute("title", CMS.Localization.Common.Previous);
            prevLink.MergeAttribute("href", urlHelper.Current(new { page = pageNumber - 1 }, new string[] { "errorMessage" }).ToString());
            if (pageNumber - 1 < 0)
            {
                prevLink.AddCssClass("disabled");
                prevLink.MergeAttribute("href", "javascript:void(0);", true);
            }
            prevLink.InnerHtml = "<";
            if (pageNumber - pagesLinkCount > 0)
            {
                wrapper.InnerHtml += "...";
            }
            //Making page number links

            var pageLinks = string.Empty;
            for (int i = pageNumber.Value - pagesLinkCount; i <= pageNumber.Value + pagesLinkCount; i++)
            {
                var pageLink = new TagBuilder("a");
                if (i >= 0 && i < pageCount)
                {
                    pageLink.MergeAttribute("href", urlHelper.Current(new { page = i }, new string[] { "errorMessage" }).ToString());
                    pageLink.InnerHtml = i + 1 + "";

                    if (i == pageNumber.Value)
                    {
                        pageLink.AddCssClass("btn btn-icon btn-sm border-0 btn-hover-primary active mr-2 my-1");
                        pageLink.MergeAttribute("href", "javascript:void(0);", true);
                    }
                    else
                    {
                        pageLink.AddCssClass("btn btn-icon btn-sm border-0 btn-hover-primary mr-2 my-1");
                    }
                    pageLinks += pageLink.ToString(TagRenderMode.Normal);
                }
            }
            //Make next, last and ... elements
            if (pageNumber + pagesLinkCount < pageCount - 1)
            {
                wrapper.InnerHtml += "...";
            }
            var nextLink = new TagBuilder("a");
            nextLink.AddCssClass("btn btn-icon btn-sm btn-light-primary mr-2 my-1");
            nextLink.MergeAttribute("title", CMS.Localization.Common.Next);
            nextLink.MergeAttribute("href", urlHelper.Current(new { page = pageNumber + 1 }, new string[] { "errorMessage" }).ToString());
            if (pageNumber + 1 >= pageCount)
            {
                nextLink.AddCssClass("disabled");
                nextLink.MergeAttribute("href", "javascript:void(0);", true);
            }
            nextLink.InnerHtml = ">";
            var lastLink = new TagBuilder("a");
            lastLink.AddCssClass("btn btn-icon btn-sm btn-light-primary mr-2 my-1");
            lastLink.MergeAttribute("title", CMS.Localization.Common.Last);
            lastLink.MergeAttribute("href", urlHelper.Current(new { page = pageCount - 1 }, new string[] { "errorMessage" }).ToString());
            if (pageNumber + 1 >= pageCount)
            {
                lastLink.AddCssClass("disabled");
                lastLink.MergeAttribute("href", "javascript:void(0);", true);
            }
            lastLink.InnerHtml = ">>";

            wrapper.InnerHtml += nextLink.ToString(TagRenderMode.Normal)
                + lastLink.ToString(TagRenderMode.Normal);
            //Make summary of pages

            var pagesSummary = new TagBuilder("span");
            pagesSummary.AddCssClass("text-muted");
            var pageSummaryParent = new TagBuilder("div");
            pageSummaryParent.AddCssClass("d-flex align-items-center");
            pagesSummary.InnerHtml = string.Format(CMS.Localization.Common.PageSummary, pageNumber + 1, pageCount);
            pageSummaryParent.InnerHtml = pagesSummary.ToString(TagRenderMode.Normal);
            wrapper.InnerHtml = firstLink.ToString(TagRenderMode.Normal) +
                prevLink.ToString(TagRenderMode.Normal) +
                pageLinks +
                nextLink.ToString(TagRenderMode.Normal) +
                lastLink.ToString(TagRenderMode.Normal) +
                pageSummaryParent.ToString(TagRenderMode.Normal);
            //wrapper.InnerHtml += pagesSummary.ToString(TagRenderMode.Normal)
            //    + firstLink.ToString(TagRenderMode.Normal)
            //    + prevLink.ToString(TagRenderMode.Normal);
            //return the result
            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}