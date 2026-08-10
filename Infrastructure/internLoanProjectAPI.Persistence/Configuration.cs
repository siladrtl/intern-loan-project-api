using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Persistence
{
    public class Configuration
    {
        public static string ConnectionString
        {
            get
            {

                ConfigurationManager configurationManager = new();
                configurationManager.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../..internLoanProjectAPI-API"));
                configurationManager.AddJsonFile("appsettings.json");
                return configurationManager.GetConnectionString("SqlConnection");
            }
        }
    }
}
