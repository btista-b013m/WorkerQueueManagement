using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkerQueueManagement.Models
{
    public class CandidateRequest
    {
        public string Apellido {get;set;}
        public string Nombre {get;set;}
        public string Direccion {get;set;}
        public string Telefono {get;set;}
        public string Email {get;set;}
        public string CarreraId {get;set;}
        public string JornadaId {get;set;}
        public string ExamenId {get;set;}
    }
}