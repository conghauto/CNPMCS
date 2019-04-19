using System;
using MyCompanyName.AbpZeroTemplate.Core;
using MyCompanyName.AbpZeroTemplate.Localization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyCompanyName.AbpZeroTemplate.Extensions.MarkupExtensions
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (ApplicationBootstrapper.AbpBootstrapper == null || Text == null)
            {
                return Text;
            }

            return L.Localize(Text);
        }
    }
}