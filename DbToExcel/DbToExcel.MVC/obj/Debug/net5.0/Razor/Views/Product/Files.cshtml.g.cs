#pragma checksum "C:\Users\Blackerback\OneDrive\Masaüstü\NetCoreRabbitMQ\DbToExcel\DbToExcel.MVC\Views\Product\Files.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "b497597ce422177e075f39f85102febc1f369c70"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Product_Files), @"mvc.1.0.view", @"/Views/Product/Files.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\Blackerback\OneDrive\Masaüstü\NetCoreRabbitMQ\DbToExcel\DbToExcel.MVC\Views\_ViewImports.cshtml"
using DbToExcel.MVC;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\Blackerback\OneDrive\Masaüstü\NetCoreRabbitMQ\DbToExcel\DbToExcel.MVC\Views\_ViewImports.cshtml"
using DbToExcel.MVC.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b497597ce422177e075f39f85102febc1f369c70", @"/Views/Product/Files.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"755fd98298d8f6c19016b99751e7460726d2e85b", @"/Views/_ViewImports.cshtml")]
    public class Views_Product_Files : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<List<UserFile>>
    {
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<table class=\"table table-striped\">\r\n    <thead>\r\n        <tr>\r\n            <th>File Name</th>\r\n            <th>Created Date</th>\r\n            <th>File Status</th>\r\n            <th>Download</th>\r\n        </tr>\r\n    </thead>\r\n");
#nullable restore
#line 12 "C:\Users\Blackerback\OneDrive\Masaüstü\NetCoreRabbitMQ\DbToExcel\DbToExcel.MVC\Views\Product\Files.cshtml"
     foreach (var item in Model)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <tr>\r\n            <td>");
#nullable restore
#line 15 "C:\Users\Blackerback\OneDrive\Masaüstü\NetCoreRabbitMQ\DbToExcel\DbToExcel.MVC\Views\Product\Files.cshtml"
           Write(item.FileName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n            <td>");
#nullable restore
#line 16 "C:\Users\Blackerback\OneDrive\Masaüstü\NetCoreRabbitMQ\DbToExcel\DbToExcel.MVC\Views\Product\Files.cshtml"
           Write(item.CreatedDate);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n            <td>");
#nullable restore
#line 17 "C:\Users\Blackerback\OneDrive\Masaüstü\NetCoreRabbitMQ\DbToExcel\DbToExcel.MVC\Views\Product\Files.cshtml"
           Write(item.FileStatus);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n            <td>\r\n                ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "b497597ce422177e075f39f85102febc1f369c704582", async() => {
                WriteLiteral("Download");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
            BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "href", 2, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            AddHtmlAttributeValue("", 464, "~/files/", 464, 8, true);
#nullable restore
#line 19 "C:\Users\Blackerback\OneDrive\Masaüstü\NetCoreRabbitMQ\DbToExcel\DbToExcel.MVC\Views\Product\Files.cshtml"
AddHtmlAttributeValue("", 472, item.FilePath, 472, 14, false);

#line default
#line hidden
#nullable disable
            EndAddHtmlAttributeValues(__tagHelperExecutionContext);
            BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "class", 3, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            AddHtmlAttributeValue("", 495, "btn", 495, 3, true);
            AddHtmlAttributeValue(" ", 498, "btn-primary", 499, 12, true);
#nullable restore
#line 19 "C:\Users\Blackerback\OneDrive\Masaüstü\NetCoreRabbitMQ\DbToExcel\DbToExcel.MVC\Views\Product\Files.cshtml"
AddHtmlAttributeValue(" ", 510, item.FileStatus==FileStatus.Creating? "disabled":"", 511, 54, false);

#line default
#line hidden
#nullable disable
            EndAddHtmlAttributeValues(__tagHelperExecutionContext);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n            </td>\r\n        </tr>\r\n");
#nullable restore
#line 22 "C:\Users\Blackerback\OneDrive\Masaüstü\NetCoreRabbitMQ\DbToExcel\DbToExcel.MVC\Views\Product\Files.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </table>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<List<UserFile>> Html { get; private set; }
    }
}
#pragma warning restore 1591
