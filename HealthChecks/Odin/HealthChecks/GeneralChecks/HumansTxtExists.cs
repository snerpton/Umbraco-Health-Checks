using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;

namespace Umbraco.Web.HealthCheck.Checks.SEO
{
    [HealthCheck("99195BDD-2A52-412A-8C20-39B35A359A92", "Humans.txt",
    Description = "Create a humans.txt file to indicate the authors of the site in a non-obtrusive manner. For more information see: http://humanstxt.org",
    Group = "Odin")]
    public class HumansTxt : HealthCheck
    {
        private readonly ILocalizedTextService _textService;

        public HumansTxt(HealthCheckContext healthCheckContext) : base(healthCheckContext)
        {
            _textService = healthCheckContext.ApplicationContext.Services.TextService;
        }

        public override IEnumerable<HealthCheckStatus> GetStatus()
        {
            return new[] { CheckForHumansTxtFile() };
        }

        public override HealthCheckStatus ExecuteAction(HealthCheckAction action)
        {
            switch (action.Alias)
            {
                case "addDefaultHumansTxtFile":
                    return AddDefaultHumansTxtFile();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private HealthCheckStatus CheckForHumansTxtFile()
        {
            //ToDo: should also test if humans.txt has been completed. Maybe 
            //check if file contains the word 'Umbraco' as this should be in 
            //the technology colophon.
            var success = File.Exists(HttpContext.Current.Server.MapPath("~/humans.txt"));
            var message = success
                ? _textService.Localize("healthcheck/odinHumansCheckSuccess")
                : _textService.Localize("healthcheck/odinHumansCheckFailed");

            var actions = new List<HealthCheckAction>();

            if (success == false)
                actions.Add(new HealthCheckAction("addDefaultHumansTxtFile", Id)
                // Override the "Rectify" button name and describe what this action will do
                {
                    Name = _textService.Localize("healthcheck/odinHumansRectifyButtonName"),
                    Description = _textService.Localize("healthcheck/odinHumansRectifyDescription")
                });

            return
                new HealthCheckStatus(message)
                {
                    ResultType = success ? StatusResultType.Success : StatusResultType.Info,
                    Actions = actions
                };
        }

        private HealthCheckStatus AddDefaultHumansTxtFile()
        {
            var success = false;
            var message = string.Empty;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine(@"# humanstxt.org/");
            sb.AppendLine(@"# The humans responsible & technology colophon");
            sb.AppendLine(@"");
            sb.AppendLine(@"# TEAM");
            sb.AppendLine(@"");
            sb.AppendLine(@"    < name > -- < role > -- < twitter >");
            sb.AppendLine(@"");
            sb.AppendLine(@"# THANKS");
            sb.AppendLine(@"");
            sb.AppendLine(@"    < name >");
            sb.AppendLine(@"");
            sb.AppendLine(@"# TECHNOLOGY COLOPHON");
            sb.AppendLine(@"");
            sb.AppendLine(@"    CSS3, HTML5");
            sb.AppendLine(@"    Apache Server Configs, jQuery, Modernizr, Normalize.css");
            
            string content = sb.ToString();

            try
            {
                File.WriteAllText(HostingEnvironment.MapPath("~/humans.txt"), content);
                success = true;
            }
            catch (Exception exception)
            {
                LogHelper.Error<HumansTxt>("Could not write humans.txt to the root of the site", exception);
            }

            return
                new HealthCheckStatus(message)
                {
                    ResultType = success ? StatusResultType.Success : StatusResultType.Error,
                    Actions = new List<HealthCheckAction>()
                };
        }
    }
}