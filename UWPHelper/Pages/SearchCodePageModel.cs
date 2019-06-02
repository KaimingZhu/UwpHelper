using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWPHelper.Areas.Identity.Data;
using UWPHelper.Models;
using UWPHelper.Services.Interface;

namespace UWPHelper.Pages
{
    public class SearchCodePageModel : PageModel
    {
        public UserManager<UWPHelperUser> _userManager;
        protected readonly SignInManager<UWPHelperUser> _signInManager;
        protected readonly IdentityContext _identityContext;
        protected readonly IAuthorizationService _authorizationService;
        protected readonly ISourceCodeManager _sourceCodeManager;
        protected readonly IDetectManager _detectManager;

        public SearchCodePageModel(UserManager<UWPHelperUser> userManager, SignInManager<UWPHelperUser> signInManager, IDetectManager detectManager,
            IdentityContext identityContext, IAuthorizationService authorizationService, ISourceCodeManager sourceCodeManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _identityContext = identityContext;
            _authorizationService = authorizationService;
            _sourceCodeManager = sourceCodeManager;
            _detectManager = detectManager;
        }
    }
}
