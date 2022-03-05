using CollectorApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CollectorApp.Pages
{
    [IgnoreAntiforgeryToken]
    public class CollectModel : PageModel
    {
        ApplicationDbContext? context;
        IHttpContextAccessor? accessor;

        public CollectModel(ApplicationDbContext db, IHttpContextAccessor acc)
        {
            context = db;
            accessor = acc;
        }

        public async void OnPostAsync()
        {
            var campaignField = Request.Form["CampaignField"];
            var userNameField = Request.Form["UserNameField"];
            var machineNameField = Request.Form["MachineNameField"];
            var BIOSSerialNumberField = Request.Form["BIOSSerialNumberField"];
            
            if (context == null)
                return;

            var collectorService = new CollectorService(context);

            var remoteIPAddress = accessor?.HttpContext?.Request.Headers["X-Forwarded-For"];

            if (string.IsNullOrEmpty(remoteIPAddress))
                remoteIPAddress = accessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            await collectorService.AddCollectedDataAsync(new Collected()
            {
                Campaign = campaignField,
                UserName = userNameField,
                MachineName = machineNameField,
                BIOSSerialNumber = BIOSSerialNumberField,
                CollectedDateTime = DateTime.Now,
                HostnameIP = remoteIPAddress,
            });

            await Task.CompletedTask;
        }
    }
}
