using QuizCodingCollective.ViewModels;
using QuizCodingCollective.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetaPoco;

namespace QuizCodingCollective.Controllers
{
    public class AccountController : Controller
    {
        // Login
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["currentuser"] == null)
            {
                return View("login");
            }
            else
            {
                ViewBag.logininfo = Session["currentuser"] as LoginInfoSession;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            List<string> errorMsg = new List<string>();
            var resultAuth = UserFacade.Auth(model, out errorMsg);
            foreach (var error in errorMsg)
            {
                ModelState.AddModelError(error, error);
            }

            if (resultAuth != null)
            {
                Session["currentuser"] = resultAuth;
                ViewBag.logininfo = Session["currentuser"] as LoginInfoSession;
                return Redirect("/home/index");
            }
            return View("login");
        }

        // Logout
        public ActionResult Logout()
        {
            Session["currentuser"] = null;
            return View("login");
        }

        // Register
        [HttpGet]
        public ActionResult Register()
        {
            if (Session["currentuser"] == null)
            {
                return View("register");
            }
            return Redirect("/home");

        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            UserFacade userFacade = new UserFacade();
            List<string> errorMsg = new List<string>();
            var loginInfoSession = userFacade.RegisterVisitor(model, out errorMsg);

            foreach (var error in errorMsg)
            {
                ModelState.AddModelError(error, error);
            }
            if (errorMsg.Count > 0)
            {
                return View("register");
            }
            return Redirect("/account/login");
        }
    }
}