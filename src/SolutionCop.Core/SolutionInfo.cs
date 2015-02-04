using System.Collections.Generic;
using System.Linq;

namespace SolutionCop.Core
{
    public class SolutionInfo
    {
        private readonly string[] _projectFilePaths;
        private readonly bool _isParsed;

        public SolutionInfo()
        {
            _projectFilePaths = new string[0];
        }

        public SolutionInfo(string[] projectFilePaths)
        {
            _isParsed = true;
            _projectFilePaths = projectFilePaths;
        }

        public IEnumerable<string> ProjectFilePaths
        {
            get
            {
                return _projectFilePaths.AsEnumerable();
            }
        }

        public bool IsParsed
        {
            get { return _isParsed; }
        }
    }
}