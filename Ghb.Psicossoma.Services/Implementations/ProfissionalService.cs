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
    public class ProfissionalService : BaseService<ProfissionalDto, Profissional>, IProfissionalService
    {
        private readonly IProfissionalRepository _profissionalRepository;
        private readonly IConfiguration _configuration;

        public ProfissionalService(IProfissionalRepository profissionalRepository, IMapper mapper, IConfiguration configuration) : base(profissionalRepository, mapper)
        {
            _profissionalRepository = profissionalRepository;
            _configuration = configuration;
        }

        ResultDto<ProfissionalResponseDto> IProfissionalService.Get(string id)
        {
            Stopwatch elapsedTime = new();
            elapsedTime.Start();

            ResultDto<ProfissionalResponseDto> returnValue = new();

            try
            {
                string selectQuery = $@"SELECT pf.Id, ps.Nome, rp.Descricao AS RegistroProfissional, pf.Numero, pf.Ativo
                                        FROM profissional pf
                                        INNER JOIN pessoa ps ON pf.pessoaId = ps.Id
                                        INNER JOIN registroProfissional rp ON pf.registroProfissionalId = rp.id
                                        WHERE pf.Id = {id};";

                DataTable result = _profissionalRepository.Get(selectQuery);
                List<ProfissionalResponse> profissionais = result.CreateListFromTable<ProfissionalResponse>();

                if (profissionais?.Count > 0)
                {
                    returnValue.CurrentPage = 1;
                    returnValue.PageSize = -1;
                    returnValue.TotalItems = profissionais.Count;
                    returnValue.Items = _mapper.Map<IEnumerable<ProfissionalResponse>, IEnumerable<ProfissionalResponseDto>>(profissionais ?? Enumerable.Empty<ProfissionalResponse>());
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

        ResultDto<ProfissionalResponseDto> IProfissionalService.GetAll()
        {
            Stopwatch elapsedTime = new();
            elapsedTime.Start();

            ResultDto<ProfissionalResponseDto> returnValue = new();

            try
            {
                string selectQuery = $@"SELECT pf.Id, ps.Nome, rp.Descricao AS RegistroProfissional, pf.Numero, pf.Ativo
                                        FROM profissional pf
                                        INNER JOIN pessoa ps ON pf.pessoaId = ps.Id
                                        INNER JOIN registroProfissional rp ON pf.registroProfissionalId = rp.id;";

                DataTable result = _profissionalRepository.GetAll(selectQuery);
                List<ProfissionalResponse> profissionais = result.CreateListFromTable<ProfissionalResponse>();

                if (profissionais?.Count > 0)
                {
                    returnValue.CurrentPage = 1;
                    returnValue.PageSize = -1;
                    returnValue.TotalItems = profissionais.Count;
                    returnValue.Items = _mapper.Map<IEnumerable<ProfissionalResponse>, IEnumerable<ProfissionalResponseDto>>(profissionais ?? Enumerable.Empty<ProfissionalResponse>());
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

        public override ResultDto<ProfissionalDto> Insert(ProfissionalDto dto)
        {
            Stopwatch elapsedTime = new();
            elapsedTime.Start();

            ResultDto<ProfissionalDto> returnValue = new();

            try
            {
                Profissional profissional = _mapper.Map<ProfissionalDto, Profissional>(dto);
                string insertQuery = $@"INSERT INTO profissional(Id, PessoaId, RegistroProfissionalId, Numero, Ativo)
                                        VALUES(null, {profissional.PessoaId}, {profissional.RegistroProfissionalId}, '{profissional.Numero}', {profissional.Ativo});";

                long newId = _profissionalRepository.Insert(insertQuery);
                if (newId > 0)
                    profissional.Id = (int)newId;

                var item = _mapper.Map<Profissional, ProfissionalDto>(profissional);

                returnValue.Items = returnValue.Items.Concat(new[] { item });
                returnValue.WasExecuted = true;
                returnValue.ResponseCode = 200;
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
