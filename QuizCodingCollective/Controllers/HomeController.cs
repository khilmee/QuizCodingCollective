using PetaPoco;
using QuizCodingCollective.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizCodingCollective.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["currentuser"] != null)
            {
                ViewBag.PageTitle = "Employee Data";
                ViewBag.logininfo = Session["currentuser"] as LoginInfoSession;
                string[] DisplayColumns = new string[] { "Name", "JobTitle", "Salary" };
                long draw = Convert.ToInt64(Request.QueryString["draw"]);
                long start = Convert.ToInt64(Request.QueryString["iDisplayStart"]);
                long length = Convert.ToInt64(Request.QueryString["iDisplayLength"]);
                string search = Request.QueryString["sSearch"];
                long sortIndex = Convert.ToInt64(Request.QueryString["iSortCol_0"]);
                string sEcho = Request.QueryString["sEcho"];
                string sortDir = Request.QueryString["sSortDir_0"] == null ? "asc" : Request.QueryString["sSortDir_0"];
                long pagestart = start == 0 || length == 0 ? 0 : start / length;

                if (Request.QueryString["format"] != null && Request.QueryString["format"].ToString().ToUpper() == "JSON")
                {
                    string filter = " WHERE 1=1 ";
                    if (search != null && search != "")
                    {
                        filter += string.Format(" AND Name like '%{0}%' OR JobTitle like '%{0}%' OR Salary like '%{0}%'", search);
                    }

                    string sort = "";
                    if (sEcho == "1")
                    {
                        //pertama kali load sEcho=1
                        sort = " order by name desc";
                    }
                    else
                    {
                        //ditrigger user
                        sort = " order by " + DisplayColumns[sortIndex] + " " + sortDir;
                    }

                    int totalrecords = 0;
                    int totaldisplayrecords = 0;

                    Database DB = new Database("Quiz");
                    var records = new List<EmployeeViewModel>();

                    totalrecords = DB.ExecuteScalar<int>("SELECT COUNT(*) FROM Employee " + filter);
                    totaldisplayrecords = DB.ExecuteScalar<int>("SELECT COUNT(*) FROM Employee" + filter);

                    records = DB.Fetch<EmployeeViewModel>(pagestart + 1, length, "SELECT * FROM Employee " + filter + sort);

                    List<string[]> listResult = new List<string[]>();

                    foreach (var data in records)
                    {
                        listResult.Add(new string[] { data.Name.ToString(), data.JobTitle.ToString(), data.Salary.ToString() });
                    }

                    return Json(new
                    {
                        sEcho = draw,
                        iTotalRecords = totalrecords,
                        iTotalDisplayRecords = totaldisplayrecords,
                        aaData = listResult
                    },
                    JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ViewBag.logininfo = Session["currentuser"] as LoginInfoSession;
                    return View();
                }
            }
            else {
                return RedirectToAction("Login", "Account");
            }
                
        }

        public ActionResult About()
        {
            if (Session["currentuser"] != null)
            {
                ViewBag.Message = "Your application description page.";

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            
        }

        public ActionResult Contact()
        {
            if (Session["currentuser"] != null)
            {
                ViewBag.Message = "Your contact page.";

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }
    }
}