﻿using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace WebApplication1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class UserController : Controller
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitofWork _unitofWork;

        public UserController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
            _roleManager = roleManager;
            _userManager = userManager;
           
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagment(string userId)
        {
          
            RoleManagmentVM RoleVM = new RoleManagmentVM()
            {
                ApplicationUser = _unitofWork.ApplicationUser.Get(u=>u.Id == userId , includeProperties:"Company"),
                RoleList =_roleManager.Roles.Select(i=> new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList =_unitofWork.Company.GetAll().Select(i=> new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            RoleVM.ApplicationUser.Role = _userManager.GetRolesAsync(_unitofWork.ApplicationUser.Get(u => u.Id == userId))
                .GetAwaiter().GetResult().FirstOrDefault();



            return View(RoleVM);
        }
        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
        {      
            string oldRole = _userManager.GetRolesAsync(_unitofWork.ApplicationUser.Get(u=>u.Id == roleManagmentVM.ApplicationUser.Id)).GetAwaiter().GetResult().FirstOrDefault();
            ApplicationUser applicationUser = _unitofWork.ApplicationUser.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id);
            if (!(roleManagmentVM.ApplicationUser.Role == oldRole))
            {
                //a role was updated
              
                if (roleManagmentVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }

                _unitofWork.ApplicationUser.Update(applicationUser);
                _unitofWork.Save();


                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if (oldRole == SD.Role_Company && applicationUser.CompanyId != roleManagmentVM.ApplicationUser.CompanyId)
                {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                    _unitofWork.ApplicationUser.Update(applicationUser);
                    _unitofWork.Save();
                }
                
            }
            return RedirectToAction("Index");
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _unitofWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();
            
            foreach (var user in objUserList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault(); 
                if(user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
            
            return Json(new {data = objUserList });
        }

        [HttpPost]
        public IActionResult LockUnLock([FromBody]string id)
        {
           
            var objFromDb = _unitofWork.ApplicationUser.Get(u => u.Id == id);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if(objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _unitofWork.ApplicationUser.Update(objFromDb);
            _unitofWork.Save();
            return Json(new { success = true, message = "Operation Successfully" });

        }
        #endregion


    }
}
