using System;

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