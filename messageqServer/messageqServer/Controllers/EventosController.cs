using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using messageqServer.Models;
using messageqServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace messageqServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventosController : Controller
    {
        private readonly IEventosService _eventosService;
        private readonly IMapper _mapper;
        public EventosController(
            IEventosService eventosService,
            IMapper mapper)
        {
            _eventosService = eventosService;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<EventResponse>> GetAll()
        {
            var eventos = _eventosService.GetAll();
            return Ok(eventos);
        }

        [HttpGet("GetSensor/{sensor}")]
        public ActionResult<IEnumerable<EventResponse>> GetEventoBySensor(string sensor)
        {
            var process = _eventosService.GetBySensor(sensor);
            return Ok(process);
        }

        [HttpGet("GetRegiao/{regiao}")]
        public ActionResult<IEnumerable<EventResponse>> GetEventoByRegiao(string regiao)
        {
            var process = _eventosService.GetByRegiao(regiao);
            return Ok(process);
        }
    }
}
