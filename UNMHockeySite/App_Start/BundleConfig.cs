using System.Web;
using System.Web.Optimization;

namespace UNMHockeySite
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Calendar css file
            bundles.Add(new StyleBundle("~/Content/fullcalendarcss").Include(
                     "~/Content/fullcalendar.css"));

            //Calendar Script file

            bundles.Add(new ScriptBundle("~/bundles/fullcalendarjs").Include(
                      "~/Scripts/fullcalendar.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/countdown").Include(
                "~/Scripts/jquery.countdown.js"));

            bundles.Add(new ScriptBundle("~/bundles/crop").Include(
                "~/Scripts/jquery.Jcrop.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
          "~/Scripts/bootstrap-datepicker.js"
          ));

            bundles.Add(new ScriptBundle("~/bundles/timepicker").Include(
            "~/Scripts/jquery.timepicker.js"
            ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/jquery.ui.all.css",
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/jquery.Jcrop.css"));

            bundles.Add(new StyleBundle("~/Content/datepickercss").Include(
                    "~/Content/bootstrap-datepicker.css"
                    ));
            bundles.Add(new StyleBundle("~/Content/timepickercss").Include(
                "~/Content/jquery.timepicker.css"
                ));

        }
    }
}
