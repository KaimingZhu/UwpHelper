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
    public class IndexModel : PageModel
    {

        private IdentityContext _identityContext { get; }
        private UserManager<UWPHelperUser> _userManager { get; }

        public IndexModel(IdentityContext identityContext, UserManager<UWPHelperUser> userManager)
        {
            _identityContext = identityContext;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public IList<SourceCodeForDisPlay> sourceCodes_ForDisPlay { get; set; }

        public async void OnGetAsync()
        {
            //授权处理

            //错误信息返回

            //读取数据
            await GetSourceCodeForDisPlayAsync();

        }

        public async Task GetSourceCodeForDisPlayAsync()
        {
            var SourceCodes = from s in _identityContext.SourceCodes select s;

            foreach (var item in SourceCodes){
                var temp = new SourceCodeForDisPlay(item.ID, item.name, item.FileUrl);
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