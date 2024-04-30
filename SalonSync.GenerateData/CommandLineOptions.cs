using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SalonSync.GenerateData
{
    public class CommandLineOptions
    {

        [Option(shortName: 'h', longName: "scheduleHistorically", Required = false, HelpText = "Flag to schedule appointments in the past, true or false", Default = false)]
        public bool ScheduleHistorically { get; set; }

        [Option(shortName: 'd', longName: "numberOfDaysToSchedule", Required = false, HelpText = "Number of days to schedule, number value", Default = 0)]
        public int NumberOfDaysToSchedule { get; set; }

        [Option(shortName: 'c', longName: "createNewClients", Required = false, HelpText = "Flag to Create new clients, true or false", Default = false)]
        public bool CreateNewClients { get; set; }

        [Option(shortName: 'n', longName: "NumberOfNewClientsToCreate", Required = false, HelpText = "Number Of New Clients To Create, number value", Default = 0)]
        public int NumberOfNewClientsToCreate { get; set; }

    }
}
