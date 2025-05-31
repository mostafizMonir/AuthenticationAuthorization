using System;
using MilanAuth.Abstractions;

namespace MilanAuth.Data;

    public class OutboxMessage : IEntity
    {
        public int Id { get; set; }
        public DateTime OccurredOn { get; set; }
        public string Data { get; set; }
    }

