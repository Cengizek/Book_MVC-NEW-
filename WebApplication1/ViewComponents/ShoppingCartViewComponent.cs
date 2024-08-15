﻿using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.DataAccess.Repository.IRepository;
using WebApplication1.Utility;

namespace WebApplication1.ViewComponents
{
    public class ShoppingCartViewComponent :ViewComponent
    {
        private readonly IUnitofWork _unitofWork;
        public ShoppingCartViewComponent(IUnitofWork unitofWork)
        {
            _unitofWork = unitofWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity =(ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if(HttpContext.Session.GetInt32(SD.SessionCart)== null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                   _unitofWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                }
                return View(HttpContext.Session.GetInt32(SD.SessionCart));

            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        
        
        }


    }
}
