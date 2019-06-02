using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UWPHelper.Areas.Identity.Data;
using UWPHelper.Services.Interface;

namespace UWPHelper.Pages
{
    [AllowAnonymous]
    public class PrivacyModel : PageModel
    {
        private readonly IDetectManager _detectManager;
        public PrivacyModel(IDetectManager detectManager)
        {
            _detectManager = _detectManager;
        }


        public void OnGet()
        {
        }
    }
}