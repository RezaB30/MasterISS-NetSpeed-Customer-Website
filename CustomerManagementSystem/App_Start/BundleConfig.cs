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
                        "~/assets/js/pages/*.js",
                        "~/assets/js/scripts.bundle.js",
                        "~/assets/plugins/global/*.js",
                        "~/assets/plugins/custom/prismjs/*.js",
                        "~/assets/plugins/custom/fullcalendar/*.js",
                        "~/Scripts/Custom/current-subscription.js",
                        "~/Scripts/Custom/resource-displayText.js",
                        "~/Scripts/Custom/display-message-box.js"));

            bundles.Add(new ScriptBundle("~/bundles/loginjs").Include(
                "~/assets/plugins/global/plugins.bundle.js",
                "~/assets/plugins/custom/prismjs/prismjs.bundle.js",
                "~/assets/js/scripts.bundle.js",
                "~/Scripts/Custom/resource-displayText.js"
                //"~/assets/js/pages/custom/login/login-general.js"
                ));
            bundles.Add(new StyleBundle("~/bundles/logincss").Include(
                "~/assets/css/pages/login/classic/login-6.css",
                "~/assets/plugins/global/plugins.bundle.css",
                "~/assets/plugins/custom/prismjs/prismjs.bundle.css",
                "~/assets/css/style.bundle.css"                
                ));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/assets/css/pages/error/*.css",
                      //"~/assets/css/pages/invoice/*.css",
                      //"~/assets/css/pages/login/*.css",
                      //"~/assets/css/pages/login/classic/*.css",
                      //"~/assets/css/pages/users/*.css",
                      //"~/assets/css/pages/wizard/*.css",
                      "~/assets/css/style.bundle.css",
                      //"~/assets/plugins/custom/cropper/*.css",
                      //"~/assets/plugins/custom/datatables/*.css",
                      "~/assets/plugins/custom/fullcalendar/*.css",
                      //"~/assets/plugins/custom/jqvmap/*.css",
                      //"~/assets/plugins/custom/jstree/*.css",
                      //"~/assets/plugins/custom/kanban/*.css",
                      //"~/assets/plugins/custom/leaflet/*.css",
                      "~/assets/plugins/custom/prismjs/*.css",
                      //"~/assets/plugins/custom/uppy/*.css",
                      "~/assets/plugins/global/*.css"
                      ));
        }
    }
}
