﻿using MassTransit;
using System;
using System.Threading.Tasks;

namespace Tilde.MT.FileTranslationService.Services
{
    public class TaskTranslationService
    {
        private readonly IBus _bus;

        public TaskTranslationService(
            IBus bus
        )
        {
            _bus = bus;
        }

        public async Task Send(Guid task)
        {
            var endpoint = await _bus.GetSendEndpoint(new Uri("queue:file-translation?durable=true"));
            await endpoint.Send(new Models.RabbitMQ.FileTranslationRequest()
            {
                Task = task.ToString()
            });
        }
    }
}
