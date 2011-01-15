using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Helper.Pagination;
using System.Web.Mvc;
using System.Text;

namespace Mvc.Helper.UI
{
    public class Footer : IHtmlString 
    {
        private const int pageCount = 5;
        private string paginationInfoFormat = @"Affichage de l'élement {0} à {1} sur {2} éléments";
        private string paginationFirst = @"Premier";
        private string paginationPrevious = @"Précédent";
        private string paginationNext = @"Suivant";
        private string paginationLast = @"Dernier";
        private readonly IPagination pagination;
        private readonly Func<int, string> urlBuilder;

        public Footer(IPagination pagination, Func<int, string> urlBuilder)
        {
            this.pagination = pagination;
            this.urlBuilder = urlBuilder;
        }

        public override string ToString()
        {
            return Render();
        }

        private string Render()
        {
            return string.Format(@"<div class='grid-footer fg-toolbar ui-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix'>{0}</div>", RenderBody());
        }

        private string RenderBody()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(@"<div class='dataTables_info'>{0}</div>", RenderInfo());
            builder.AppendFormat(@"<div class='dataTables_paginate fg-buttonset ui-buttonset fg-buttonset-multi ui-buttonset-multi paging_full_numbers'>{0}</div>", RenderPager());
            return builder.ToString();
        }

        private string RenderPager()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(@"<span class='first ui-corner-tl ui-corner-bl fg-button ui-button ui-state-default {0}'>{1}</span>", pagination.PageNumber == 1 ? "ui-state-disabled" : "", CreatePageLink(1, paginationFirst));
            builder.AppendFormat(@"<span class='previous fg-button ui-button ui-state-default {0}'>{1}</span>", pagination.HasPreviousPage ? "" : "ui-state-disabled", CreatePageLink(pagination.PageNumber - 1, paginationPrevious));
            builder.AppendFormat(@"<span>{0}</span>", RenderNumericPager());
            builder.AppendFormat(@"<span class='next fg-button ui-button ui-state-default {0}'>{1}</span>", pagination.HasNextPage ? "" : "ui-state-disabled", CreatePageLink(pagination.PageNumber + 1, paginationNext));
            builder.AppendFormat(@"<span class='last ui-corner-tr ui-corner-br fg-button ui-button ui-state-default {0}'>{1}</span>", pagination.PageNumber == pagination.TotalPages ? "ui-state-disabled" : "", CreatePageLink(pagination.TotalPages, paginationLast));
            return builder.ToString();
        }

        private string RenderNumericPager()
        {
            var currentLinks = GetCurrentLinks();
            var builder = new StringBuilder();

            for (int i = currentLinks.First; i <= currentLinks.Last; i++)
                RenderNumericPageLink(builder, i);
            return builder.ToString();
        }

        private CurrentPageLinks GetCurrentLinks()
        {
            var links = new CurrentPageLinks();
            double pageCountHalf = Math.Floor((double)(pageCount / 2));

            if (pagination.TotalPages < pageCount)
            {
                links.First = 1;
                links.Last = pagination.TotalPages;
            }
            else
            {
                if (pagination.PageNumber <= pageCountHalf)
                {
                    links.First = 1;
                    links.Last = pageCount;
                }
                else
                {
                    if (pagination.PageNumber >= (pagination.TotalPages - pageCountHalf))
                    {
                        links.First = pagination.TotalPages - pageCount + 1;
                        links.Last = pagination.TotalPages;
                    }
                    else
                    {
                        links.First = pagination.PageNumber - ((int)Math.Ceiling((double)(pageCount / 2))) + 1;
                        links.Last = links.First + pageCount - 1;
                    }
                }
            }

            return links;
        }

        private void RenderNumericPageLink(StringBuilder builder, int page) 
        { 
            builder.AppendFormat(@"<span class='fg-button ui-button ui-state-default {0}'>{1}</span>", pagination.PageNumber == page ? "ui-state-disabled" : "", CreatePageLink(page, page.ToString()));
        }

        private string RenderInfo()
        {
            if (pagination.TotalItems == 0)
                return string.Empty;
            return string.Format(paginationInfoFormat, pagination.FirstItem, pagination.LastItem, pagination.TotalItems); 
        }

        private string CreatePageLink(int pageNumber, string text) 
        { 
            var builder = new TagBuilder("a");
            builder.SetInnerText(text); 
            builder.MergeAttribute("href", urlBuilder(pageNumber)); 
            return builder.ToString(TagRenderMode.Normal); 
        }

        private class CurrentPageLinks
        {
            public int First { get; set; }
            public int Last { get; set; }
        }

        public string ToHtmlString()
        {
            return Render();
        }
    }
}