using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _scopeFactory;

    public EventProcessor(IMapper mapper, IServiceScopeFactory scopeFactory)
    {
        _mapper = mapper;
        _scopeFactory = scopeFactory;
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                addPlatform(message);
                break;
            default:
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine($"--> Determining Event");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        switch (eventType?.Event)
        {
            case "Platform_Published":
                Console.WriteLine("--> Platform Published Event Detected");
                return EventType.PlatformPublished;
            default:
                Console.WriteLine("--> Could not determine the event type");
                return EventType.Undetermined;
        }
    }

    private void addPlatform(string platform)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

            var platformPublisherDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platform);

            try
            {
                var plat = _mapper.Map<Platform>(platformPublisherDto);
                if (!repo.ExternalPlatformExists(plat.ExternalID))
                {
                    repo.CreatePlatform(plat);
                    repo.SaveChanges();
                    Console.WriteLine("--> Platform added!");
                }
                else
                {
                    Console.WriteLine("--> Platform already exists...");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not add Platform to DB {ex.Message}");
            }
        }
    }
}

enum EventType
{
    PlatformPublished,
    Undetermined
}
