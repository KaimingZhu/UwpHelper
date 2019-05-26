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
    public class IndexModel : PageModel
    {
        private readonly IAuthorizationService _authorizationService;
        private IdentityContext _identityContext { get; }
        private UserManager<UWPHelperUser> _userManager { get; }

        public IndexModel(IdentityContext identityContext, UserManager<UWPHelperUser> userManager, IAuthorizationService authorizationService)
        {
            _identityContext = identityContext;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        [BindProperty(SupportsGet = true)]
        public IList<SourceCodeForDisPlay> sourceCodes_ForDisPlay { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            //判断是否已授权
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);
            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            //错误信息返回

            //读取数据
            await GetSourceCodeForDisPlayAsync();

            return Page();
        }

        public async Task GetSourceCodeForDisPlayAsync()
        {
            var SourceCodes = from s in _identityContext.SourceCodes select s;

            foreach (var item in SourceCodes){
                var temp = new SourceCodeForDisPlay(item);
                if (temp.ifExist)
                {
                    sourceCodes_ForDisPlay.Add(temp);
                }
                else
                {
                    var delete = await _identityContext.SourceCodes.FindAsync(item.ID);
                    if(delete != null)
                    {
                        _identityContext.Remove(delete);
                    }
                }
            }

            await _identityContext.SaveChangesAsync();
        }
    }
}