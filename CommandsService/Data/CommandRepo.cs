using CommandsService.Models;

namespace CommandsService.Data;

public class CommandRepo : ICommandRepo
{
    private readonly AppDbContext _context;

    public CommandRepo(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Command GetCommand(int platformId, int commandId)
    {
        return _context.Commands
                ?.Where(c => c.PlatformId == platformId && c.Id == commandId)
                .FirstOrDefault() ?? new Command();
    }

    public void CreateCommand(int platformId, Command command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        command.PlatformId = platformId;
        _context.Commands?.Add(command);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms?.ToList() ?? Enumerable.Empty<Platform>();
    }

    public void CreatePlatform(Platform plat)
    {
        if (plat == null)
        {
            throw new ArgumentNullException(nameof(plat));
        }

        _context.Platforms?.Add(plat);
    }

    public bool PlatformExits(int platformId)
    {
        return _context.Platforms?.Any(p => p.Id == platformId) ?? false;
    }

    public bool ExternalPlatformExists(int externalPlatformId)
    {
        return _context.Platforms?.Any(p => p.ExternalID == externalPlatformId) ?? false;
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return _context.Commands
                ?.Where(c => c.PlatformId == platformId)
                .OrderBy(c => c.Platform.Name) ?? Enumerable.Empty<Command>();
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}
