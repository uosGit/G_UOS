using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace UOS.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/DataTables/jquery.dataTables.js",
                "~/Scripts/DataTables/dataTables.bootstrap4.js",
                "~/Scripts/jquery.datetimepicker.js",
                "~/Scripts/toastr.js"
                ));

            bundles.Add(new ScriptBundle("~/Scripts/jqueryval").Include(
                 "~/Scripts/jquery.validate.js"
                )
            );

            bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/bootstrap.css",
                    "~/Content/CustomeStyle.css",
                    "~/Content/DataTables/css/dataTables.bootstrap.css",
                    "~/Content/jquery.datetimepicker.css",
                    "~/Content/toastr.css",
                    "~/Content/fontawesome.min.css",
                   "~/Content/font-awesome.min.css"
                )
             );

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
          "~/Scripts/jquery-ui-{version}.js"));


            //css  
            bundles.Add(new StyleBundle("~/Content/cssjqryUi").Include(
                   "~/Content/jquery-ui.css"
                   ));

        }
    }
}


  