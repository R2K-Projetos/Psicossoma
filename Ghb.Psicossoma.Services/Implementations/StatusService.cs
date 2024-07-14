﻿using AutoMapper;
using System.Data;
using System.Diagnostics;
using Ghb.Psicossoma.Services.Dtos;
using Ghb.Psicossoma.Domains.Entities;
using Ghb.Psicossoma.Library.Extensions;
using Microsoft.Extensions.Configuration;
using Ghb.Psicossoma.Services.Abstractions;
using Ghb.Psicossoma.Repositories.Abstractions;
using Ghb.Psicossoma.SharedAbstractions.Services.Implementations;

namespace Ghb.Psicossoma.Services.Implementations
{
    public class StatusService : BaseService<StatusDto, Status>, IStatusService
    {
        private readonly IStatusRepository _statusRepository;
        private readonly IConfiguration _configuration;

        public StatusService(IStatusRepository statusRepository, IMapper mapper, IConfiguration configuration) : base(statusRepository, mapper)
        {
            _statusRepository = statusRepository;
            _configuration = configuration;
        }

        public override ResultDto<StatusDto> GetAll()
        {
            Stopwatch elapsedTime = new();
            elapsedTime.Start();

            ResultDto<StatusDto> returnValue = new();

            try
            {
                string selectQuery = $@"SELECT Id, Descricao FROM status;";

                DataTable result = _statusRepository.GetAll(selectQuery);
                List<Status> status = result.CreateListFromTable<Status>();

                if (status?.Count > 0)
                {
                    returnValue.CurrentPage = 1;
                    returnValue.PageSize = -1;
                    returnValue.TotalItems = status.Count;
                    returnValue.Items = _mapper.Map<IEnumerable<Status>, IEnumerable<StatusDto>>(status ?? Enumerable.Empty<Status>());
                    returnValue.WasExecuted = true;
                    returnValue.ResponseCode = 200;
                }
                else
                {
                    returnValue.BindError(404, "Não foram encontrados dados para exibição");
                }
            }
            catch (Exception ex)
            {
                returnValue.BindError(500, ex.GetErrorMessage());
            }

            elapsedTime.Stop();
            returnValue.ElapsedTime = elapsedTime.Elapsed;

            return returnValue;
        }

    }
}
