using MailScheduler.Config;
using MailScheduler.Mappers;
using MailScheduler.Models.Dtos;
using MailScheduler.Models.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailScheduler.Repositories
{
    public class SchedulerRepository : RepositoryBase<UserSchedule>, ISchedulerRepository
    {
        public override string CollectionName { get => "schedule"; }

        public SchedulerRepository(IAppSettings settings) : base(settings)
        {
        }

        public UserScheduleDto GetScheduleByToken(string surveyId, string token)
        {
            var surveyFilter = Builders<UserSchedule>.Filter.Eq(d => d.SurveyId, surveyId);
            var tokenFilter = Builders<UserSchedule>.Filter.Eq(d => d.Token, token);

            var filter = Builders<UserSchedule>.Filter.And(surveyFilter, tokenFilter);
            
            var response = base.FindByQuery(filter)?.FirstOrDefault();

            return response?.ToDto();
        }

        public string SaveUserSchedule(UserScheduleDto dto)
        {
            var entity = dto.ToEntity();
            
            if (string.IsNullOrEmpty(entity.Id))
            {
                var existingUser = GetScheduleByToken(dto.SurveyId, dto.Token);

                entity.Id = existingUser?.Id ?? ObjectId.GenerateNewId().ToString();
            }

            var filter = Builders<UserSchedule>.Filter.Where(d => d.Id == entity.Id);

            return base.UpsertDocument(filter, entity) ?? entity.Id;
        }
    }
}
