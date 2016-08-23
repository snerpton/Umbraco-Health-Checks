using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;

namespace Umbraco.Web.HealthCheck.Checks.SEO
{
    [HealthCheck("3A482719-3D90-4BC1-B9F8-910CD9CF5B32", "Robots.txt",
    Description = "Create a robots.txt file to block access to system folders.",
    Group = "SEO")]
    public class RobotsTxt : HealthCheck
    {
        private readonly ILocalizedTextService _textService;

        public RobotsTxt(HealthCheckContext healthCheckContext) : base(healthCheckContext)
        {
            _textService = healthCheckContext.ApplicationContext.Services.TextService;
        }

        public override IEnumerable<HealthCheckStatus> GetStatus()
        {
            return new[] { CheckForRobotsTxtFile() };
        }

        public override HealthCheckStatus ExecuteAction(HealthCheckAction action)
        {
            switch (action.Alias)
            {
                case "addDefaultRobotsTxtFile":
                    return AddDefaultRobotsTxtFile();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private HealthCheckStatus CheckForRobotsTxtFile()
        {
            var success = File.Exists(HttpContext.Current.Server.MapPath("~/robots.txt"));
            var message = success
                ? _textService.Localize("healthcheck/seoRobotsCheckSuccess")
                : _textService.Localize("healthcheck/seoRobotsCheckFailed");

            var actions = new List<HealthCheckAction>();

            if (success == false)
                actions.Add(new HealthCheckAction("addDefaultRobotsTxtFile", Id)
                // Override the "Rectify" button name and describe what this action will do
                {
                    Name = _textService.Localize("healthcheck/seoRobotsRectifyButtonName"),
                    Description = _textService.Localize("healthcheck/seoRobotsRectifyDescription")
                });

            return
                new HealthCheckStatus(message)
                {
                    ResultType = success ? StatusResultType.Success : StatusResultType.Error,
                    Actions = actions
                };
        }

        private HealthCheckStatus AddDefaultRobotsTxtFile()
        {
            var success = false;
            var message = string.Empty;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine(@"User-agent: *   # match all bots");
            sb.AppendLine(@"");
            sb.AppendLine(@"# Disallow: /   # keep them out. Remove on go live!");
            sb.AppendLine(@"");
            sb.AppendLine(@"");
            sb.AppendLine(@"# The following directories are all linked to in the frontend, but we probably don't want them indexed.");
            sb.AppendLine(@"Disallow: / css");
            sb.AppendLine(@"Disallow: / Css");
            sb.AppendLine(@"Disallow: / less");
            sb.AppendLine(@"Disallow: / Less");
            sb.AppendLine(@"Disallow: / media");
            sb.AppendLine(@"Disallow: / Media");
            sb.AppendLine(@"Disallow: / scripts");
            sb.AppendLine(@"Disallow: / Scripts");
            sb.AppendLine(@"Disallow: / umbraco");
            sb.AppendLine(@"Disallow: / Umbraco");
            
            string content = sb.ToString();

            try
            {
                File.WriteAllText(HostingEnvironment.MapPath("~/robots.txt"), content);
                success = true;
            }
            catch (Exception exception)
            {
                LogHelper.Error<RobotsTxt>("Could not write robots.txt to the root of the site", exception);
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