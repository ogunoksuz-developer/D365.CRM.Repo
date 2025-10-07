using System.Threading.Tasks;
using Pars.Core.IInitializer;

namespace Pars.CS.DynamicsToKafka.Producer.API.Data.EF;

public class TemplateContextGenerateDatabaseForSample : IInitializer
{
    private readonly TemplateContext _context;
    public TemplateContextGenerateDatabaseForSample(TemplateContext context)
    {
        _context = context;
    }
    public Task InitializeAsync()
    {
        return _context.Database.EnsureCreatedAsync();
    }
}
