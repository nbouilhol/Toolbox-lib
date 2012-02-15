using System.Collections.Generic;

namespace Utilities.DynamicXml
{
    public class HypotheseDTO
    {
        public IEnumerable<string> Gratuits { get; set; }
        public bool AchatOpportunite { get; set; }
        public bool AchatCL2 { get; set; }
        public string CibleCodeFunc { get; set; }
        public int NoGaiAnnonceur { get; set; }
        public string Name { get; set; }
        public IEnumerable<short> CodesEcransExclus { get; set; }
        public bool AchatOpportuniteHorsAO15 { get; set; }
        public double TauxPassage { get; set; }
        public double RentaPreviousYear { get; set; }
        public IList<int> Produits { get; set; }
        public int Year { get; set; }
    }
}
