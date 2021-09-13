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
                    string filteradmin = " WHERE 1=1 ";
                    if (search != null && search != "")
                    {
                        filter += string.Format(" AND Name like '%{0}%' OR JobTitle like '%{0}%' OR Salary like '%{0}%'", search);
                        filteradmin += string.Format("  WHERE (Name LIKE '%{0}%' OR Agenda like '%{0}%' OR Agenda like '%{0}%') ", search);
                    }

                    string sort = "";
                    if (sEcho == "1")
                    {
                        //pertama kali load sEcho=1
                        sort = " order by createddate desc";
                    }
                    else
                    {
                        //ditrigger user
                        sort = " order by " + DisplayColumns[sortIndex] + " " + sortDir;
                    }

                    int totalrecords = 0;
                    int totaldisplayrecords = 0;

                    Database DB = new Database("VMS");
                    var records = new List<SecurityViewModel>();

                    if (CommonFacade.GetCurrentLoginInfoSession().Role == "SECURITY")
                    {
                        filter += " AND IsCheckIn = 1 AND CONVERT(VARCHAR(10), VisitingDate, 111) = CONVERT(VARCHAR(10), GETDATE(), 111)  ";
                        totalrecords = DB.ExecuteScalar<int>("SELECT COUNT(*) FROM vwSecurity " + filter);
                        totaldisplayrecords = DB.ExecuteScalar<int>("SELECT COUNT(*) FROM vwSecurity" + filter);

                        // Query select untuk menampilkan visitor yang checkin pada hari ini dengan detail asset yang dibawa
                        // Detail Asset

                        records = DB.Fetch<SecurityViewModel>(pagestart + 1, length, "SELECT * FROM vwSecurity " + filter + sort);

                    }
                    else
                    {
                        totalrecords = DB.ExecuteScalar<int>("SELECT COUNT(*) FROM vwSecurity " + filteradmin);
                        totaldisplayrecords = DB.ExecuteScalar<int>("SELECT COUNT(*) FROM vwSecurity" + filteradmin);

                        // Query select untuk menampilkan visitor yang checkin pada hari ini dengan detail asset yang dibawa
                        // Detail Asset

                        records = DB.Fetch<SecurityViewModel>(pagestart + 1, length, "SELECT * FROM vwSecurity " + filteradmin + sort);

                    }
                    //var records = DB.Fetch<SecurityViewModel>(pagestart + 1, length, "SELECT * FROM VisitingRequest " + filter + sort);

                    List<string[]> listResult = new List<string[]>();

                    foreach (var data in records)
                    {
                        string buttonhtml = "";
                        var aset = DB.Fetch<dynamic>("Select * From vwVisitorAsset Where VisitorAccountNumber = '" + data.VisitorAccountNumber + "'");
                        var assetHtml = "<button jsondata='" + JsonConvert.SerializeObject(aset) + "' class='btn btn-info btn-block btn-border btn-detail-user' style='width: 110;' type='submit' data-toggle='modal' data-target='modal-detail'>View Asset</button>";

                        var IsChecked = DB.FirstOrDefault<AssetSecurity>("select * from AssetSecurity where VisitingID =@0 AND VisitorAccountNumber=@1 AND CreatedBy=@2", data.ID, data.VisitorAccountNumber, CommonFacade.GetCurrentLoginInfoSession().Name);

                        if (IsChecked != null)
                        {
                            buttonhtml = "<div class='waiting-approval'>Check Out</div>";
                        }
                        else
                        {
                            buttonhtml = "<button class='btn btn-success btn-block btn-match' jsondata='" + JsonConvert.SerializeObject(data) + "' style='width: 110px;'>Match</button>" +
                                "<button class='btn btn-danger btn-block btn-not-match' jsondata='" + JsonConvert.SerializeObject(data) + "' style='width: 110px;'>Not Match</button>";

                        }

                        listResult.Add(new string[] { data.ID.ToString(), data.Name.ToString(), data.VisitingDate == null ? "" : ((DateTime)data.VisitingDate).ToString("dd-MM-yyyy"), data.Agenda.ToString(), data.MeetingWith.ToString(), assetHtml, buttonhtml, JsonConvert.SerializeObject(data) });
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