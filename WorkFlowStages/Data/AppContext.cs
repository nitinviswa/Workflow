using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;
using WorkFlowStages.Model;

namespace WorkFlowStages.Data
{
    public class AppContext : DbContext
    {

        public AppContext(DbContextOptions<AppContext> options) : base(options) { }

        public DbSet<WorkFlowStages.Model.WorkFlowStages> workflowstages { get; set; }
        public DbSet<WorkFlowStageJson> workflowstagesjson { get; set; }
        public DbSet<WorkFlowStages.Model.Envelope> envelopes { get; set; }
    }
}
