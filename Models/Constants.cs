//#define Home
#if !Home
#define Work
#endif
namespace Plazma.Controllers
{
    class Constants
    {
#if Home                                            
        public const string bdconnectionstring = @"Data Source=R2D2;Initial Catalog=PLASMA;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;MultipleActiveResultSets=True";
        public const string configurationFile= "Home-settings.json";
        public const string _plasmaPath = @"D:\PlazmaProgs";
#elif Work                                              
        public const string bdconnectionstring = @"Data Source=NOVIKOV\PLAZMASERVER;Initial Catalog=PLASMA;User ID='sa';Password='J3qq4h7h2v';Integrated Security=false;Connect Timeout=30;MultipleActiveResultSets=True";
        //public const string bdconnectionstring = @"Data Source=NOVIKOV\MS_SQL_SERVER;Initial Catalog=PLASMA;Integrated Security=True;MultipleActiveResultSets=True";
        public const string configurationFile = "Work-settings.json";
        public const string _plasmaPath = @"\\korolev\_Плазменная резка";
        public const string CNCPath = @"D:\PlazmaProgs";
#endif
        Constants() { }
    }
}

