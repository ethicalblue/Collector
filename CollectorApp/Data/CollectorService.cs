using Microsoft.EntityFrameworkCore;

namespace CollectorApp.Data
{
    public class CollectorService
    {
        private ApplicationDbContext db;

        public CollectorService(ApplicationDbContext dbContext)
        {
            db = dbContext;
        }

        public async Task<List<Collected>?> GetCollectedDataAsync()
        {
            if (db.Collected == null)
                return null;

            if (db.Collected.Any() == false)
                return null;

            return await db.Collected.Where(y => y.IsArchived == false && y.UserName != "placeholder").OrderByDescending(x => x.Id).ToListAsync();
        }

        public async Task<Collected?> GetSingleCollectedDataAsync(int id)
        {
            if (db.Collected == null)
                return null;

            if (db.Collected.Any() == false)
                return null;

            return await db.Collected.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<long?> GetCollectedDataCountAsync()
        {
            if (db.Collected == null)
                return null;

            if (db.Collected.Any() == false)
                return null;

            return await db.Collected.Where(y => y.IsArchived == false && y.UserName != "placeholder").LongCountAsync();
        }

        public async Task<List<string?>?> GetCampaignsAsync()
        {
            if (db.Collected == null)
                return null;

            if (db.Collected.Any() == false)
                return null;

            return await db.Collected.Where(y => y.IsArchived == false).Select(a => a.Campaign).Distinct().ToListAsync();
        }

        public async Task<List<string?>?> GetArchivedCampaignsAsync()
        {
            if (db.Collected == null)
                return null;

            if (db.Collected.Any() == false)
                return null;

            return await db.Collected.Where(y => y.IsArchived == true).Select(a => a.Campaign).Distinct().ToListAsync();
        }

        public async Task<Collected> AddCollectedDataAsync(Collected collected)
        {
            if (db.Collected != null)
            {
                db.Collected.Add(collected);
                await db.SaveChangesAsync();
            }
            return collected;
        }

        public async Task<Collected> UpdateCollectedDataAsync(Collected collected)
        {
            if (db.Collected == null)
                throw new NullReferenceException();

            var exists = db.Collected.Single(p => p.Id == collected.Id);
            if (exists != null)
            {
                db.Update(collected);
                await db.SaveChangesAsync();
            }

            return collected;
        }

        public async Task ArchiveOrRestoreCampaignAsync(string campaign, bool archived)
        {
            if (db.Collected == null)
                throw new NullReferenceException();

            foreach(var item in db.Collected.Where(p => p.Campaign == campaign))
            {
                item.IsArchived = archived;
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteCollectedDataAsync(Collected collected)
        {
            if (db.Collected == null)
                throw new NullReferenceException();

            db.Collected.Remove(collected);
            await db.SaveChangesAsync();
        }

        public async Task DeleteCampaignAsync(string campaign)
        {
            if (db.Collected == null)
                throw new NullReferenceException();

            var list = db.Collected.Where(p => p.Campaign == campaign).ToList();

            db.Collected.RemoveRange(list);
            await db.SaveChangesAsync();
        }
    }
}
