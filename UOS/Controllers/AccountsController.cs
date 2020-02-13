using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Data.Entity;
using System.Security.Claims;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using UOS.Models;
using UOS.App_Start;
using UOS.Models.TwoFactorAuthentication;
using System.Data.Entity.Core.Objects;

namespace UOS.Controllers
{
    public class AccountsController : Controller
    {
        
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //Constructors
        public AccountsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public AccountsController()
        {
           
        }

        
        [AllowAnonymous]
        // Get: /Accounts/Login
        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("_Dashboard", "AffiliationAdminSide");
                if (User.IsInRole("Member"))
                    return RedirectToAction("Index", "AffiliationMembers");
                if (User.IsInRole("User"))
                {
                    return RedirectToAction("Dashboard", "UserDashboard");
                    
                }
                if (User.IsInRole("RegistrationAdmin"))
                {
                    return RedirectToAction("Index", "RegistrationAdminSide");

                }
                if (User.IsInRole("AcadBranch"))
                {
                    return RedirectToAction("EligibilityChecks", "CommonInitials");
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        // POST:   /Accounts/Login
        [HttpPost]
        public async Task<ActionResult> Login(Login login)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(login);
            try
            {

                var result = await SignInManager.PasswordSignInAsync(login.Email, login.Password, false, shouldLockout: true);
                switch (result)
                {
                    case SignInStatus.Success:
                        {
                            var dbUser = db.AspNetUsers.SingleOrDefault(u => u.UserName == login.Email);
                            var isDisable = dbUser.isDisable;
                            if (isDisable == false)
                            {
                                var claims = UserManager.GetClaims(dbUser.Id);
                                if (claims != null)
                                {
                                    foreach (var claim in claims)
                                    {
                                        UserManager.RemoveClaim(dbUser.Id, claim);
                                    }
                                }
                                var dbyear = db.BI_Affi_pop_year_for_the_affilation();
                                SessionObject obj = new SessionObject();
                                foreach (var year in dbyear)
                                {
                                    obj.Affiliation_Year = Convert.ToInt32(year.Year);
                                    obj.Id_year_for_affilation = Convert.ToInt32(year.ID);
                                    obj.Affiliation_Year = Convert.ToInt32(year.Year);
                                    obj.Id_year_for_affilation = Convert.ToInt32(year.ID);
                                }

                                obj.Institute_Id = dbUser.Institute_ID.ToString();
                                UserManager.AddClaim(dbUser.Id, new Claim("InstituteId", obj.Institute_Id.ToString()));
                                UserManager.AddClaim(dbUser.Id, new Claim("AffiliationYear", obj.Affiliation_Year.ToString()));
                                UserManager.AddClaim(dbUser.Id, new Claim("YearId", obj.Id_year_for_affilation.ToString()));
                                return RedirectToAction("Login", "Accounts");
                            }
                            return View(login);
                        }
                    case SignInStatus.LockedOut:
                        {
                            ViewBag.Account = "Account Is Locked";
                            return View(login);
                        }
                    case SignInStatus.RequiresVerification:
                        {
                            return RedirectToAction("SendCode", "Accounts", new {login=login });
                        }
                    case SignInStatus.Failure:
                        {
                            ViewBag.UserError = "UserName And Password Does Not Match";
                            return View(login);
                        }
                    default:
                        return View(login);
                }
            }
            catch (Exception e)
            {
                ViewBag.RegisterError = "Error While Logging Your Account Contact Admin Person"+ e.Message;
                return View(login);
            }
            }
        }

        ////Confirm that it's authenticated User\
        //public async Task<ActionResult> SendCode(Login login)
        //{
        //    var userId = await SignInManager.GetVerifiedUserIdAsync();
        //    var userfactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
        //    LoginVerification model = new LoginVerification();

        //    List<Providers> providerlist = new List<Providers>();
        //    foreach (var userfactor in userfactors)
        //    {
        //        Providers provider = new Providers();
        //        provider.Name = userfactor;
        //        providerlist.Add(provider);
        //    }
        //    model.ProviderDD = providerlist;
        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<ActionResult> SendCode(LoginVerification model)
        //{
        //    await SignInManager.SendTwoFactorCodeAsync(model.Provider);
        //    return RedirectToAction("VerifyCode", "Accounts", new  {Provider=model.Provider });
        //}

        //public ActionResult VerifyCode(string Provider)
        //{
        //    var model = new LoginVerification {Provider=Provider };
        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<ActionResult> VerifyCode(LoginVerification model)
        //{
        //    var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code,false, false);
        //    var userid = await SignInManager.GetVerifiedUserIdAsync();
        //    var claims = UserManager.GetClaims(userid);
        //    if (claims != null)
        //    {
        //        foreach (var claim in claims)
        //        {
        //            UserManager.RemoveClaim(userid, claim);
        //        }
        //    }
        //    var dbyear = db.BI_Affi_pop_year_for_the_affilation();
        //    SessionObject obj = new SessionObject();
        //    foreach (var year in dbyear)
        //    {
        //        obj.Affiliation_Year = Convert.ToInt32(year.Year);
        //        obj.Id_year_for_affilation = Convert.ToInt32(year.ID);
        //        obj.Affiliation_Year = Convert.ToInt32(year.Year);
        //        obj.Id_year_for_affilation = Convert.ToInt32(year.ID);
        //    }
        //    var dbUser = db.AspNetUsers.SingleOrDefault(u => u.Id == userid);
        //    obj.Institute_Id = dbUser.Institute_ID.ToString();
        //    UserManager.AddClaim(userid, new Claim("InstituteId", obj.Institute_Id.ToString()));
        //    UserManager.AddClaim(userid, new Claim("AffiliationYear", obj.Affiliation_Year.ToString()));
        //    UserManager.AddClaim(userid, new Claim("YearId", obj.Id_year_for_affilation.ToString()));

        //    return RedirectToAction("Login", "Accounts");
        //}

        [Authorize]
        public ActionResult Logout()
        {
            try
            {
                Session.Contents.RemoveAll();
                var userid=User.Identity.GetUserId();
                var claims = UserManager.GetClaims(userid);
                foreach (var claim in claims)
                {
                    UserManager.RemoveClaim(userid,claim);
                }
                var AuthenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
                AuthenticationManager.SignOut();
                
                return RedirectToAction("Login", "Accounts");
            }
            catch (Exception e)
            {
                ViewBag.LogoutError = "Error in Logging Out";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: /Accounts/Register
        //public async Task<ActionResult> Register()
        //{
        //    ApplicationUser User = new ApplicationUser()
        //    {
        //        UserName="VerifyUser",
        //        UserType=7,
        //        Email="fastfood9699@gmail.com",
        //        isDisable=false,
        //        PhoneNumber="+923070501323"
        //    };
        //    await UserManager.CreateAsync(User, "User12345@");
        //    await UserManager.AddToRoleAsync(User.Id, "User");
        //    return RedirectToAction("VerifyCellNo", "Accounts", new { PhoneNumber = "+923070501323", UserId = User.Id });
        //}


       // Creating New Role
        //public ActionResult NewRole()
        //{
        //    var RoleStore = new RoleStore<IdentityUser>();
        //    var RoleManager = new RoleManager<IdentityUser>(RoleStore);

        //    return View();
        //}

        //public async Task<ActionResult> VerifyCellNo(string PhoneNumber,string UserId)
        //{
        //    await UserManager.SetTwoFactorEnabledAsync(UserId, true);
        //    var code = await UserManager.GenerateChangePhoneNumberTokenAsync(UserId,PhoneNumber );
        //    if (UserManager.SmsService != null)
        //    {
        //        var message = new IdentityMessage
        //        {
        //            Destination = PhoneNumber,
        //            Body = "Your security code is: " + code
        //        };
        //        await UserManager.SmsService.SendAsync(message);
        //    }
        //    LoginVerification model = new LoginVerification() {PhoneNumber=PhoneNumber,UserId=UserId };
        //    return View(model);
        //}

        //[HttpPost]
        //public async Task<ActionResult> VerifyCellNo(LoginVerification model)
        //{
            
        //    var result=await UserManager.ChangePhoneNumberAsync(model.UserId, model.PhoneNumber, model.Code);
        //    if (!result.Succeeded)
        //        return RedirectToAction("VerifyCellNo", "Accounts", new { PhoneNumber = model.PhoneNumber, UserId = model.UserId });
        //    var returnUrl = TempData["ReturnUrl"].ToString();
        //    TempData["RegisteredUserId"] = model.UserId;

        //    return Redirect(returnUrl);
        //}

         //POST: /Accounts/Register
        //It is creating new User login never ever try to give access to it
        //public async Task<ActionResult> RegisterUserMember()
        //{

        //    var RoleStore = new RoleStore<IdentityRole>();
        //    var RoleManager = new RoleManager<IdentityRole>(RoleStore);
        //    var Role = new IdentityRole()
        //    {
        //        Name = "AcadBranch"
        //    };
        //    await RoleManager.CreateAsync(Role);
        //    //await UserManager.AddToRoleAsync(User.Id, Role.Name);
        //    var UserStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
        //    var UserManager = new UserManager<ApplicationUser>(UserStore);
        //    var User = new ApplicationUser()
        //    {
        //        UserName = "AdminAcad",
        //        Email = "AcadBranch@gmail.com",
        //        UserType = 13,
        //        isDisable = false

        //    };
        //    IdentityResult result = UserManager.Create(User, "Admin1122");
        //    await UserManager.AddToRoleAsync(User.Id, "AcadBranch");

        //    return View();
        //}


        // POST: /Accounts/Register
        //It is creating new admin login never ever try to give access to it
        //public ActionResult RegisterAdminMember()
        //{

        //    var UserStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
        //    var UserManager = new UserManager<ApplicationUser>(UserStore);
        //    var User = new ApplicationUser()
        //    {
        //        UserName = "RegAdmin",
        //        Email = "RegistrationAdmin@gmail.com",
        //        UserType = 12,
        //        isDisable = false

        //    };
        //    IdentityResult result = UserManager.Create(User, "Admin123123");
        //    UserManager.AddToRoleAsync(User.Id, "RegistrationAdmin");

        //    return View();
        //}


	}
}