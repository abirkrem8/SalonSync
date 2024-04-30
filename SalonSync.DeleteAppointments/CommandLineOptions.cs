using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SalonSync.DeleteAppointments
{
    public class CommandLineOptions
    {

        [Option(shortName: 'a', longName: "deleteAllAppointments", Required = false, HelpText = "Flag to delete all appointments, true or false", Default = false)]
        public bool DeleteAllAppointments { get; set; }

        [Option(shortName: 'd', longName: "numberOfDaysToDelete", Required = false, HelpText = "Number of days to delete appointments for, number value", Default = 0)]
        public int NumberOfDaysToDelete { get; set; }

        [Option(shortName: 'f', longName: "deleteFutureAppointments", Required = false, HelpText = "Flag to delete appointments in the future, true or false", Default = false)]
        public bool DeleteFutureAppointments { get; set; }

    }
}
