using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactManager.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UWPHelper.Areas.Identity.Data;
using UWPHelper.Models;

namespace UWPHelper.Pages.SourceCodePages
{
    public class DetailModel : PageModel
    {
        private IAuthorizationService _authorizationService;
        private readonly IdentityContext _identitycontext;
        private readonly UserManager<UWPHelperUser> _userManager;

        public DetailModel(IdentityContext context, UserManager<UWPHelperUser> userManager, IAuthorizationService authorizationService)
        {
            _identitycontext = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        public SourceCodeForDisPlay sourceCodeForDisPlay { get; set; }

        public IActionResult OnGet(int? id)
        {

            //判断是否已授权
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);
            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            if (id == null)
            {
                return NotFound();
            }

            var sourceCode = _identitycontext.SourceCodes.FirstOrDefault(m => m.ID == id);
            if (sourceCode == null)
            {
                return NotFound();
            }

            sourceCodeForDisPlay = new SourceCodeForDisPlay(sourceCode);

            return Page();
        }
    }
}