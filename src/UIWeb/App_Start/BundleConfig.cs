using System.Web.Optimization;

namespace Wiz.Gringotts.UIWeb
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.globalize/globalize.js",
                        "~/Scripts/jquery.globalize/cultures/globalize.culture.en-US.js",
                        "~/Scripts/jquery.serializeToJSON.js",
                        "~/Scripts/mustache.js",
                        "~/Scripts/lodash.min.js",
                        "~/Scripts/jquery.hotkeys*",
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/Modules/global.navigation.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.globalize.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/mvcvalidationextensions.unobtrusive.js",
                        "~/Scripts/jquery.validate.unobtrusive.bootstrap.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));


            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/bootstrap-accessibility*",
                      "~/Scripts/bootstrap-switch*",
                      "~/Scripts/bootstrap-dialog*",
                      "~/Scripts/dataTables.bootstrap.js",
                      "~/Scripts/typeahead.bundle.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-theme.css",
                      "~/Content/PagedList.css",
                      "~/Content/typeaheadjs.css",
                      "~/Content/bootstrap-accessibility.css",
                      "~/Content/dataTables.bootstrap.css",
                      "~/Content/bootstrap-switch/bootstrap3/bootstrap-switch.css",
                      "~/Content/bootstrap-dialog.css",
                      "~/Content/font-awesome.css",
                      "~/Content/dataTables.fontAwesome.css",
                      "~/Content/cropper.css",
                      "~/Content/site.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }
    }
}