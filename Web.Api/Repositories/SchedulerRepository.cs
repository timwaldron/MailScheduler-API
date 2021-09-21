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

        public async Task<UserScheduleDto> GetScheduleByToken(string surveyId, string token)
        {
            var surveyFilter = Builders<UserSchedule>.Filter.Eq(d => d.SurveyId, surveyId);
            var tokenFilter = Builders<UserSchedule>.Filter.Eq(d => d.Token, token);

            var filter = Builders<UserSchedule>.Filter.And(surveyFilter, tokenFilter);
            
            var response = (await FindByQuery(filter))?.FirstOrDefault();

            return response?.ToDto();
        }

        public async Task<string> SaveUserSchedule(UserScheduleDto dto)
        {
            var entity = dto.ToEntity();
            var response = await Upsert(entity);

            // TODO: ASSESS CODE
            return response?.Id ?? entity.Id;
        }

        public async Task<List<UserScheduleDto>> GetAllSchedules()
        {
            var entites = await GetAll();

            return entites.Select(e => e.ToDto()).ToList();
        }
    }
}
