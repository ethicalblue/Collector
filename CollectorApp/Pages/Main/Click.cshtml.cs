using CollectorApp.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CollectorApp.Pages
{
    public class ClickModel : PageModel
    {
        ApplicationDbContext? context;
        IHttpContextAccessor? accessor;

        public ClickModel(ApplicationDbContext db, IHttpContextAccessor acc)
        {
            context = db;
            accessor = acc;
        }

        public async Task<IActionResult> OnGetAsync(string campaign, int linkId)
        {
            if (context == null)
                return Redirect("//");

            var linkService = new LinkService(context);

            var items = await linkService.GetLinksAsync();

            if (items == null)
                return Redirect("//");

            var item = items.FirstOrDefault(x => x.Campaign == campaign && x.Id == linkId);

            if (item == null)
                return Redirect("//");

            item.LinkWasClicked = true;
            item.ClickedDateTime = DateTime.Now;
            item.ClickCounter++;

            var remoteIPAddress = accessor?.HttpContext?.Request.Headers["X-Forwarded-For"];

            if (string.IsNullOrEmpty(remoteIPAddress))
                remoteIPAddress = accessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            item.HostnameIP = remoteIPAddress;

            await linkService.UpdateLinkAsync(item);

            return Redirect(item.RedirectToURL ?? "//");
        }
    }
}
