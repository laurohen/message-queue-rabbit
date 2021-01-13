using AutoMapper;
using messageqServer.Helpers;
using messageqServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace messageqServer.Services
{
    public interface IEventosService
    {
        IEnumerable<EventResponse> GetAll();
        IEnumerable<EventResponse> GetBySensor(string sensor);
        IEnumerable<EventResponse> GetByRegiao(string regiao);
    }
    
    public class EventosService : IEventosService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public EventosService(
            DataContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<EventResponse> GetAll()
        {
            var accounts = _context.Events;
            return _mapper.Map<IList<EventResponse>>(accounts);
        }

        public IEnumerable<EventResponse> GetBySensor(string Sensor)
        {
            var process = _context.Events;
            var aux = process.Where(c => c.Sensor == Sensor);
            return _mapper.Map<IList<EventResponse>>(aux);
        }
        public IEnumerable<EventResponse> GetByRegiao(string Regiao)
        {
            var process = _context.Events;
            var aux = process.Where(c => c.Regiao == Regiao);
            return _mapper.Map<IList<EventResponse>>(aux);
        }
    }
}
