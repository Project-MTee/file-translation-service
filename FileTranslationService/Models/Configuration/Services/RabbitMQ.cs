﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tilde.MT.FileTranslationService.Models.Configuration.Services
{
    public class RabbitMQ
    {
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; } = 5672;
    }
}