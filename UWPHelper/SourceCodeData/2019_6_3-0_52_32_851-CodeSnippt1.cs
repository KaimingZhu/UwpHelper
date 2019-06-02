private async Task RefreshTheCollection()
{
   viewProgressCollection.Clear();

   //读取本地
   foreach (var temp in (await _localBlackListProgressService.ListBlackListProgressAsync()))
   {
        viewProgressCollection.Add(temp);
    }

        //判断服务器或数据库

        AppUser userNow = _identityService.GetCurrentUserAsync();

        if (userNow.ID != 0)
        {
            var weblist = await _webBlackListProgressService.ListWebBlackListProgressesAsync();

            //读取服务器
            if (weblist != null)
            {
                foreach (var temp in weblist)
                {
                    viewProgressCollection.Add(_localBlackListProgressService.WebProcessToLocal(temp));
                }
            }
        }
    }