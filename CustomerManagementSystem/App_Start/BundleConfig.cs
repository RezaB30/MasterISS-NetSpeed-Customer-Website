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
                        "~/assets/plugins/custom/fullcalendar/*.js"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));            

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
