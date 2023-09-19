    using SustiVest.Data.Entities;
    
    namespace SustiVest.Data.Services
    {
    public interface IAnalystsService
    {
        IList<Analysts> GetAnalysts();
        Paged<Analysts> GetAnalysts(int page = 1, int size = 20, string orderBy = "AnalystNo", string direction = "asc");

        Analysts GetAnalyst(int analystNo);
        Analysts GetAnalystByName (string name);
        Analysts GetAnalystByEmail (string email);  

        Analysts AddAnalyst(Analysts analyst);
        Analysts UpdateAnalyst(Analysts updated);
        bool DeleteAnalyst(int analystNo);

    }
}