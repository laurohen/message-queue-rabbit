using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using messageqServer.Entities;
using messageqServer.Helpers;
using messageqServer.Models;

namespace messageqServer.Services
{

    public interface ICreateService
    {
        CreateEventResponse Create(CreateEventRequest model);
        
    }
    public class CreateService: ICreateService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CreateService(
            DataContext context,
            IMapper mapper
            )
        {
            _context = context;
            _mapper = mapper;
        }
        public CreateEventResponse Create(CreateEventRequest model)
        {
            // validate
            //if (_context.Accounts.Any(x => x.Email == model.Email))
            //    throw new AppException($"Email '{model.Email}' is already registered");

            // map model to new account object
            var eventProcess = _mapper.Map<EventProcess>(model);
            
            eventProcess.Created = DateTime.UtcNow;


            // save account
            _context.Events.Add(eventProcess);
            Console.WriteLine("Enviado pro BD");
            _context.SaveChanges();

            return _mapper.Map<CreateEventResponse>(eventProcess);
        }
    }
}
