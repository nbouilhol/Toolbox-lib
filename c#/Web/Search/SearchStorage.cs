using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc.Helper.Search
{
    [Serializable]
    public class SearchStorage : ISearch
    {
        public string Input { get; private set; }

        public SearchStorage(string input)
        {
            Input = input;
        }
    }
}