using System.Text.Encodings.Web;
using Abp.Collections.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;

namespace MyCompanyName.AbpZeroTemplate.Web.TagHelpers
{
    [HtmlTargetElement("script")]
    public class AbpZeroTemplateScriptSrcTagHelper : ScriptTagHelper
    {
        [HtmlAttributeName("minify-in-dev")]
        public bool MinifyInDevelopment { get; set; } = true;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (output.Attributes["abp-ignore-src-modification"] != null && output.Attributes["abp-ignore-src-modification"].Value.ToString() == "true")
            {
                base.Process(context, output);
                return;
            }

            string srcKey;
            if (output.Attributes["abp-src"] != null)
            {
                srcKey = "abp-src";
            }
            else if (output.Attributes["src"] != null)
            {
                srcKey = "src";
            }
            else
            {
                base.Process(context, output);
                return;
            }

            if (output.Attributes[srcKey].Value.ToString().StartsWith("~"))
            {
                base.Process(context, output);
                return;
            }

            if (output.Attributes[srcKey].Value is HtmlString ||
                output.Attributes[srcKey].Value is string)
            {
                var href = output.Attributes[srcKey].Value.ToString();
                if (href.StartsWith("~"))
                {
                    return;
                }

                var basePath = ViewContext.HttpContext.Request.PathBase.HasValue
                    ? ViewContext.HttpContext.Request.PathBase.Value
                    : string.Empty;

                if (!href.Contains(".min.js"))
                {
                    if (MinifyInDevelopment)
                    {
                        href = href.Replace(".js", ".min.js");
                    }
                    else if (!HostingEnvironment.IsDevelopment())
                    {
                        href = href.Replace(".js", ".min.js");
                    }
                }

                Src = basePath + href;
                context.AllAttributes.AddIfNotContains(new TagHelperAttribute("src", Src));
            }

            base.Process(context, output);
        }

        public AbpZeroTemplateScriptSrcTagHelper(
            IHostingEnvironment hostingEnvironment, 
            TagHelperMemoryCacheProvider cacheProvider, 
            IFileVersionProvider fileVersionProvider, 
            HtmlEncoder htmlEncoder, 
            JavaScriptEncoder javaScriptEncoder, 
            IUrlHelperFactory urlHelperFactory
        ) : base(hostingEnvironment, cacheProvider, fileVersionProvider, htmlEncoder, javaScriptEncoder, urlHelperFactory)
        {
        }
    }
}