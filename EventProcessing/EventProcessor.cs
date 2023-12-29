using AutoMapper;
using CommandService.Data;
using CommandService.Dto;
using CommandService.Models;
using System.Text.Json;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _mapper=mapper;
            _scopeFactory = scopeFactory;
   
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    //TO DO
                    addPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private void addPlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

                try 
                {
                    var plat = _mapper.Map<Platform>(platformPublishedDto);
                    if (!repo.ExternalPlatformExists(plat.ExternalID))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                        Console.WriteLine("--> PLatform Added Successfully!");
                    }
                    else 
                    {
                        Console.WriteLine("--> PLatform Already Exists");
                    }
                } catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not add Platform to DB {ex.Message}");
                }
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        { 
            Console.WriteLine("--> Determing Event");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform Published Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not determine event");
                    return EventType.Undetermined;
            }
        }
    }

    enum EventType
    { 
        PlatformPublished,
        Undetermined
    }
}
