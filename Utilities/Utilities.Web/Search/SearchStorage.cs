using System;

namespace Mvc.Helper.Search
{
    [Serializable]
    public class SearchStorage : ISearch
    {
        public SearchStorage(string input)
        {
            Input = input;
        }

        public string Input { get; private set; }
    }
}