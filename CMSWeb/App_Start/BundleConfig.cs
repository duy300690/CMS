using System.Web;
using System.Web.Optimization;

namespace CMSWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/chart").Include(
                     "~/Scripts/chart/Chart.min.js",
                     "~/Scripts/chart-area-demo.js",
                     "~/Scripts/chart-pie-demo.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap-bundle").Include(
                      "~/Scripts/bootstrap.bundle.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery.easing").Include(
                      "~/Scripts/jquery.easing.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/sb-admin").Include(
                      "~/Scripts/sb-admin-2.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/fontawesome").Include(
                      "~/fontawesome-free/css/all.min.css"));

            bundles.Add(new StyleBundle("~/Content/sb-admin").Include(
                      "~/Content/sb-admin-2.css"));

            // Xicooc Library
            bundles.Add(new ScriptBundle("~/js/custom").Include("~/Scripts/custom.js",
                                                                "~/Scripts/custom_Admin.js"));
        }
    }
}
