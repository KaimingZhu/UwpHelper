using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UWPHelper.Areas.Identity.Data;
using UWPHelper.Models;

namespace UWPHelper.Pages.SourceCodePages
{
    public class DetailModel : PageModel
    {
        private readonly IdentityContext _identitycontext;
        private readonly UserManager<UWPHelperUser> _userManager;

        public DetailModel(IdentityContext context, UserManager<UWPHelperUser> userManager)
        {
            _identitycontext = context;
            _userManager = userManager;
        }

        public SourceCodeForDisPlay sourceCodeForDisPlay { get; set; }

        public IActionResult OnGet(int? id)
        {

            //权限检查

            if(id == null)
            {
                return NotFound();
            }

            var sourceCode = _identitycontext.SourceCodes.FirstOrDefault(m => m.ID == id);
            if (sourceCode == null)
            {
                return NotFound();
            }

            sourceCodeForDisPlay = new SourceCodeForDisPlay(sourceCode.ID, sourceCode.name, sourceCode.FileUrl);

            return Page();
        }
    }
}