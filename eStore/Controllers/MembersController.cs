using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObject;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace eStore.Controllers
{
    public class MembersController : Controller
    {
        IMemberRepository memberRepository = null;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;


        public MembersController(IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            memberRepository = new MemberRepository();
            _webHostEnvironment = webHostEnvironment;
            _config = config;
        }

        [Route("/members", Name = "members")]

        // GET: MembersController
        public ActionResult Index()
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }
            
            var memberList = memberRepository.GetMembers();

            return View(memberList);    
        }

        [HttpGet]
        public ActionResult Login()
        {
            ISession session = HttpContext.Session;
            if (session.GetString("Role") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (email == null || password == null)
            {
                ViewBag.Message = "email or password cannot be empty!!!";
                return View();
            }

            var session = HttpContext.Session;

            if (email.Equals(_config.GetSection("AdminAccount").GetSection("Email").Value) && password.Equals(_config.GetSection("AdminAccount").GetSection("Password").Value))
            {
                session.SetString("Email", email);

                session.SetString("Role", "Admin");

                return Redirect("../Home/Index");
            }
            else if (memberRepository.GetMemberByEmailAndPassword(email, password) != null)
            {
                session.SetString("Email", email);

                session.SetString("Role", "Member");

                session.SetInt32("Id", memberRepository.GetMemberByEmailAndPassword(email, password).MemberId);

                return Redirect("../Home/Index");
            }
            else
            {
                ViewBag.Message = "Incorrect!";
                return View();
            }

        }
        public ActionResult Logout()
        {
            var session = HttpContext.Session;
            if (session.GetString("Email") == null)
            {
                return StatusCode(403);
            }
            session.Clear();
            return RedirectToAction("Index", "Home");
        }


        // GET: MembersController/Details/5
        public ActionResult Details(int id)
        {
            var session = HttpContext.Session;

            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            Member member = null;
            
            if (id > 0) {
                member = memberRepository.GetMemberByID(id);
            }

            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }


        // GET: MembersController/Create
        public ActionResult Create()
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            return View();
        }

        // POST: MembersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Member member)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            if (member.Email == null || member.Password == null || member.CompanyName == null || member.City == null || member.Country == null)
            {
                ViewBag.Message = "All field must be filled!";
                return View();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    memberRepository.InsertMember(member);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // GET: MembersController/Edit/5
        public ActionResult Edit(int id)
        {
            var member = memberRepository.GetMemberByID(id);
            var session = HttpContext.Session;
            if (session.GetString("Role") == null)
            {
                return StatusCode(403);
            }

          
            if (session.GetString("Role") != "Admin" && session.GetString("Email") != member.Email)
            {
                return StatusCode(403);
            }

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: MembersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Member member)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            if (session.GetString("Role") != "Admin" && session.GetString("Email") != member.Email)
            {
                return StatusCode(403);
            }

            if (member.Email == null || member.CompanyName == null || member.City == null || member.Country == null || member.Password == null)
            {
                ViewBag.Message = "All field must be filled!";
                return View();
            }

            try
            {
                if (member.MemberId <= 0) 
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    memberRepository.UpdateMember(member);
                    ViewBag.Message = "Update success!";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        // GET: MembersController/Delete/5
        public ActionResult Delete(int id)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                return StatusCode(403);
            }

            var member = memberRepository.GetMemberByID(id);
            if (member == null)
            {
                return NotFound();
            }
           
            return View(member);
        }

        // POST: MembersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            var session = HttpContext.Session;
            if (session.GetString("Role") != "Admin")
            {
                 return StatusCode(403);
            }

            try
            {
                memberRepository.DeleteMember(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }





    }
}
