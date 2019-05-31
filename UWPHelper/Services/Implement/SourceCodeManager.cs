using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWPHelper.Models;
using UWPHelper.Services.Interface;

namespace UWPHelper.Services.Implement
{
    public class SourceCodeManager : ISourceCodeManager
    {
        private readonly IdentityContext _identityContext;

        public SourceCodeManager(IdentityContext identityContext)
        {
            _identityContext = identityContext;
        }

        public void AddSourceCode(SourceCodeForDisPlay temp)
        {
            throw new NotImplementedException();
        }

        public SourceCodeForDisPlay FindSourceCode(int id)
        {
            throw new NotImplementedException();
        }

        public SourceCodeForDisPlay FindSourceCode(string FileUrl)
        {
            throw new NotImplementedException();
        }

        public List<SourceCodeForDisPlay> GetSourceCodeForDisPlays()
        {
            throw new NotImplementedException();
        }

        public void RemoveSourceCode(SourceCodeForDisPlay temp)
        {
            throw new NotImplementedException();
        }

        public void RemoveSourceCode(int id)
        {
            throw new NotImplementedException();
        }

        public void SortSourceCode()
        {
            throw new NotImplementedException();
        }

        public void UpdateSourceCode(SourceCodeForDisPlay temp)
        {
            throw new NotImplementedException();
        }

        public void UpdateSourceCode(int id)
        {
            throw new NotImplementedException();
        }
    }
}
