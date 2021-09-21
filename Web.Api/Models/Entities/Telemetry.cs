using MailScheduler.Models.Dtos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Models.Entities
{
    public class Telemetry
    {
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public Severity Severity { get; set; }
        public string Message { get; set; }
        public bool Successful { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
