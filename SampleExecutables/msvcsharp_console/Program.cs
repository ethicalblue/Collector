// See https://aka.ms/new-console-template for more information

using System.Management;

#region Sample settings
string collectorURL = "https://localhost:7096/collect";
string campaignName = "campaign001";
#endregion

#pragma warning disable CA1416 // Walidacja zgodności z platformą
ManagementObjectSearcher searcher = new("SELECT * FROM Win32_BIOS");
#pragma warning restore CA1416 // Walidacja zgodności z platformą
ManagementObject? obj = searcher.Get().Cast<ManagementObject>().FirstOrDefault();

if (obj == null) return;

string? BIOSSerialNumber = obj["SerialNumber"].ToString();

HttpClient httpClient = new();
var content = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("CampaignField", campaignName),
    new KeyValuePair<string, string>("UserNameField", Environment.UserName),
    new KeyValuePair<string, string>("MachineNameField", Environment.MachineName),
    new KeyValuePair<string, string>("BIOSSerialNumberField", BIOSSerialNumber ?? "empty")
});

await httpClient.PostAsync(collectorURL, content);
