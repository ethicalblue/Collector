using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace CollectorApp.Data
{
    public class LinkService
    {
        private ApplicationDbContext db;

        public LinkService(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        public async Task<Link?> GetLinkAsync(int id)
        {
            if (db.Link == null)
                return null;

            if (db.Link.Any() == false)
                return null;

            return await db.Link.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Link>?> GetLinksAsync()
        {
            if (db.Link == null)
                return null;

            if (db.Link.Any() == false)
                return null;

            return await db.Link.OrderByDescending(x => x.ClickedDateTime).Where(y => y.IsArchived == false).ToListAsync();
        }

        public async Task<long?> GetLinkCountAsync()
        {
            if (db.Link == null)
                return null;

            if (db.Link.Any() == false)
                return null;

            return await db.Link.Where(y => y.IsArchived == false).LongCountAsync();
        }

        public async Task ZeroClickCounterAsync(Link link)
        {
            if (db.Link == null)
                throw new NullReferenceException();

            var exists = db.Link.Single(p => p.Id == link.Id);
            if (exists != null)
            {
                exists.LinkWasClicked = false;
                exists.ClickedDateTime = DateTime.MinValue;
                exists.ClickCounter = 0;
                exists.IsArchived = false;
                db.Update(exists);
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<string?>?> GetCampaignsAsync()
        {
            if (db.Link == null)
                return null;

            if (db.Link.Any() == false)
                return null;

            return await db.Link.Where(y => y.IsArchived == false).Select(a => a.Campaign).Distinct().ToListAsync();
        }

        public async Task<List<string?>?> GetArchivedCampaignsAsync()
        {
            if (db.Link == null)
                return null;

            if (db.Link.Any() == false)
                return null;

            return await db.Link.Where(y => y.IsArchived == true).Select(a => a.Campaign).Distinct().ToListAsync();
        }

        public async Task<Link> AddLinkAsync(Link link)
        {
            if (db.Link != null)
            {
                db.Link.Add(link);
                await db.SaveChangesAsync();
            }
            return link;
        }

        public async Task<Link> UpdateLinkAsync(Link link)
        {
            if (db.Link == null)
                throw new NullReferenceException();

            var exists = db.Link.Single(p => p.Id == link.Id);
            if (exists != null)
            {
                db.Update(link);
                await db.SaveChangesAsync();
            }

            return link;
        }

        public async Task ArchiveOrRestoreCampaignAsync(string campaign, bool archived)
        {
            if (db.Link == null)
                throw new NullReferenceException();

            foreach (var item in db.Link.Where(p => p.Campaign == campaign))
            {
                item.IsArchived = archived;
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteLinkAsync(Link link)
        {
            if (db.Link == null)
                throw new NullReferenceException();

            db.Link.Remove(link);
            await db.SaveChangesAsync();
        }

        public async Task DeleteCampaignAsync(string campaign)
        {
            if (db.Link == null)
                throw new NullReferenceException();

            var list = db.Link.Where(p => p.Campaign == campaign).ToList();

            db.Link.RemoveRange(list);
            await db.SaveChangesAsync();
        }
    }

    internal static class LinkHelpers
    {
        internal static string MakeStringURLFriendly(string text)
        {
            text = text.ToLowerInvariant().Normalize(NormalizationForm.FormD);

            var stringBuilder = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                var info = CharUnicodeInfo.GetUnicodeCategory(text[i]);

                if (info == UnicodeCategory.LowercaseLetter ||
                    info == UnicodeCategory.UppercaseLetter ||
                    info == UnicodeCategory.DecimalDigitNumber)
                {
                    stringBuilder.Append(text[i]);
                }

                if (info == UnicodeCategory.SpaceSeparator)
                {
                    stringBuilder.Append('_');
                }

                if(text[i] == '_' || text[i] == '-')
                {
                    stringBuilder.Append(text[i]);
                }
            }

            return stringBuilder.ToString();
        }
    }
}