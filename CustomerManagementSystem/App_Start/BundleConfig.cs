using System.Web;
using System.Web.Optimization;

namespace CustomerManagementSystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jscripts").Include(
                        "~/Content/assets/js/pages/*.js",
                        "~/Content/assets/js/scripts.bundle.js",
                        "~/Content/assets/plugins/global/plugins.bundle.js",
                        "~/Content/assets/plugins/custom/prismjs/prismjs.bundle.js",
                        "~/Content/assets/plugins/custom/fullcalendar/fullcalendar.bundle.js",
                        "~/Scripts/Custom/current-subscription.js",
                        "~/Scripts/Custom/resource-displayText.js",
                        "~/Scripts/Custom/display-message-box.js"));

            bundles.Add(new ScriptBundle("~/bundles/loginjs").Include(
                "~/Content/assets/plugins/global/plugins.bundle.js",
                "~/Content/assets/plugins/custom/prismjs/prismjs.bundle.js",
                "~/Content/assets/js/scripts.bundle.js",
                "~/Scripts/Custom/resource-displayText.js",
                "~/Scripts/Custom/login-screen-loading.js"
                //"~/assets/js/pages/custom/login/login-general.js"
                ));
            bundles.Add(new StyleBundle("~/bundles/logincss").Include(
                "~/Content/assets/css/pages/login/classic/login-6.css",
                "~/Content/assets/plugins/global/plugins.bundle.css",
                "~/Content/assets/plugins/custom/prismjs/prismjs.bundle.css",
                "~/Content/assets/css/style.bundle.css"
                ));
            bundles.Add(new StyleBundle("~/bundles/css")
                .Include("~/Content/assets/css/style.bundle.css")
                .Include("~/Content/assets/plugins/custom/fullcalendar/fullcalendar.bundle.css")
                .Include("~/Content/assets/plugins/custom/prismjs/prismjs.bundle.css")
                .Include("~/Content/assets/plugins/global/plugins.bundle.css"));
            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          //"~/assets/css/pages/error/*.css",
            //          //"~/assets/css/pages/invoice/*.css",
            //          //"~/assets/css/pages/login/*.css",
            //          //"~/assets/css/pages/login/classic/*.css",
            //          //"~/assets/css/pages/users/*.css",
            //          //"~/assets/css/pages/wizard/*.css",
            //          "~/Content/assets/css/style.bundle.css",
            //          //"~/assets/plugins/custom/cropper/*.css",
            //          //"~/assets/plugins/custom/datatables/*.css",
            //          "~/Content/assets/plugins/custom/fullcalendar/*.css",
            //          //"~/assets/plugins/custom/jqvmap/*.css",
            //          //"~/assets/plugins/custom/jstree/*.css",
            //          //"~/assets/plugins/custom/kanban/*.css",
            //          //"~/assets/plugins/custom/leaflet/*.css",
            //          "~/Content/assets/plugins/custom/prismjs/*.css",
            //          //"~/assets/plugins/custom/uppy/*.css",
            //          "~/Content/assets/plugins/global/*.css"
            //          ));
        }
    }
}
