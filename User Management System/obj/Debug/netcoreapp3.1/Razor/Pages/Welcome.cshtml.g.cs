#pragma checksum "C:\Users\luke\Dropbox\School Stuff\CS 3750\Assignment 1 (ASP)\User Management System\Pages\Welcome.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0d5afeb4759af469587c50af5c927b2a5727ae17"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(User_Management_System.Pages.Pages_Welcome), @"mvc.1.0.razor-page", @"/Pages/Welcome.cshtml")]
namespace User_Management_System.Pages
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
#line 1 "C:\Users\luke\Dropbox\School Stuff\CS 3750\Assignment 1 (ASP)\User Management System\Pages\_ViewImports.cshtml"
using User_Management_System;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0d5afeb4759af469587c50af5c927b2a5727ae17", @"/Pages/Welcome.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"31f655af59d4899434e7e42b8f2ef15f40a02800", @"/Pages/_ViewImports.cshtml")]
    public class Pages_Welcome : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 3 "C:\Users\luke\Dropbox\School Stuff\CS 3750\Assignment 1 (ASP)\User Management System\Pages\Welcome.cshtml"
  
    ViewData["Title"] = "Welcome";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h1>Welcome</h1>\r\n<h4>");
#nullable restore
#line 8 "C:\Users\luke\Dropbox\School Stuff\CS 3750\Assignment 1 (ASP)\User Management System\Pages\Welcome.cshtml"
Write(Html.DisplayFor(model => model.Users.firstname));

#line default
#line hidden
#nullable disable
            WriteLiteral(" ");
#nullable restore
#line 8 "C:\Users\luke\Dropbox\School Stuff\CS 3750\Assignment 1 (ASP)\User Management System\Pages\Welcome.cshtml"
                                                Write(Html.DisplayFor(model => model.Users.lastname));

#line default
#line hidden
#nullable disable
            WriteLiteral("</h4>\r\n");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<User_Management_System.Pages.WelcomeModel> Html { get; private set; }
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<User_Management_System.Pages.WelcomeModel> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<User_Management_System.Pages.WelcomeModel>)PageContext?.ViewData;
        public User_Management_System.Pages.WelcomeModel Model => ViewData.Model;
    }
}
#pragma warning restore 1591
