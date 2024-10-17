﻿using Ghb.Psicossoma.Cache;
using Ghb.Psicossoma.Webapp.Models;
using Ghb.Psicossoma.Webapp.Models.ResultModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Ghb.Psicossoma.Webapp.Controllers
{
    public class PacienteController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly CacheService _cacheService;
        private readonly IConfiguration _configuration;

        public PacienteController(CacheService cacheService, IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
            _cacheService = cacheService;

            _httpClient.BaseAddress = new Uri(_configuration["baseApiAddress"]!);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _cacheService.GetCacheEntry("token", ""));
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<PacienteViewModel>? pacientes = new();
            HttpResponseMessage message = _httpClient.GetAsync($"paciente/getall").Result;

            if (message.IsSuccessStatusCode)
            {
                string? data = message.Content.ReadAsStringAsync().Result;
                ResultModel<PacienteViewModel>? model = JsonConvert.DeserializeObject<ResultModel<PacienteViewModel>>(data);
                pacientes = model?.Items.ToList();

                ViewBag.TotalEncontrado = pacientes.Count;
            }

            return View(pacientes);
        }

        public IActionResult Create()
        {
            PacienteViewModel model = new();

            ViewBag.OpcoesSexo = FillSexoDropDown();

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(PacienteViewModel obj)
        {
            HttpResponseMessage message = _httpClient.PostAsJsonAsync($"paciente/create", obj).Result;

            if (message.IsSuccessStatusCode)
            {
                string content = message.Content.ReadAsStringAsync().Result;
                ResultModel<PacienteViewModel>? model = JsonConvert.DeserializeObject<ResultModel<PacienteViewModel>>(content);
            }

            return View(obj);
        }

        public ActionResult Edit(int id)
        {
            PacienteViewModel? itemFound = null;
            HttpResponseMessage message = _httpClient.GetAsync($"paciente/get/{id}").Result;

            if (message.IsSuccessStatusCode)
            {
                string content = message.Content.ReadAsStringAsync().Result;
                ResultModel<PacienteViewModel>? model = JsonConvert.DeserializeObject<ResultModel<PacienteViewModel>>(content);
                itemFound = model!.Items.FirstOrDefault()!;
            }

            return View(itemFound);
        }

        [HttpPost]
        public ActionResult Edit(PacienteViewModel obj)
        {
            HttpResponseMessage message = _httpClient.PostAsJsonAsync($"paciente/update", obj).Result;
            if (message.IsSuccessStatusCode)
            {
                string content = message.Content.ReadAsStringAsync().Result;
                ResultModel<PacienteViewModel>? model = JsonConvert.DeserializeObject<ResultModel<PacienteViewModel>>(content);
            }

            return View(obj);
        }

        private List<SelectListItem> FillSexoDropDown()
        {
            List<SelectListItem> sexo = new()
            {
                new() { Text = "[Selecione]", Value = ""},
                new() { Text = "Masculino", Value = "M"},
                new() { Text = "Feminino", Value = "F"}
            };
            return sexo;
        }

        private List<SelectListItem> FillUf()
        {
            List<SelectListItem> ufs = new();
            HttpResponseMessage message = _httpClient.GetAsync($"uf/getall").Result;
            string content = message.Content.ReadAsStringAsync().Result;
            ResultModel<UfViewModel>? response = JsonConvert.DeserializeObject<ResultModel<UfViewModel>>(content);

            if (message.IsSuccessStatusCode)
            {
                if (!response?.HasError ?? false)
                {
                    foreach (UfViewModel item in response?.Items!)
                    {
                        SelectListItem select = new() { Text = item.Sigla, Value = item.Id.ToString() };
                        ufs.Add(select);
                    }

                    ufs.Insert(0, new SelectListItem() { Text = "[Selecione]", Value = "-1" });
                }
            }

            return ufs;
        }

        private List<SelectListItem> FillTipoCelular()
        {
            List<SelectListItem> tipos = new();
            HttpResponseMessage message = _httpClient.GetAsync($"telefone/gettypes").Result;
            string content = message.Content.ReadAsStringAsync().Result;
            ResultModel<TipoTelefoneViewModel>? response = JsonConvert.DeserializeObject<ResultModel<TipoTelefoneViewModel>>(content);

            if (message.IsSuccessStatusCode)
            {
                if (!response?.HasError ?? false)
                {
                    foreach (TipoTelefoneViewModel item in response?.Items!)
                    {
                        SelectListItem select = new() { Text = item.Nome, Value = item.Id.ToString() };
                        tipos.Add(select);
                    }

                    tipos.Insert(0, new SelectListItem() { Text = "[Selecione]", Value = "-1" });
                }
            }

            return tipos;
        }

        public IActionResult FillCidadesUF(int ufId)
        {
            HttpResponseMessage message = _httpClient.GetAsync($"cidade/GetAllByUf?ufId={ufId}").Result;
            string content = message.Content.ReadAsStringAsync().Result;
            ResultModel<CidadeViewModel>? response = JsonConvert.DeserializeObject<ResultModel<CidadeViewModel>>(content);

            var listaCidades = new List<CidadeViewModel>();
            if (message.IsSuccessStatusCode)
            {
                if (!response?.HasError ?? false)
                {
                    foreach (CidadeViewModel item in response?.Items!)
                    {
                        var cidade = new CidadeViewModel
                        {
                            Id = item.Id,
                            Nome = item.Nome,
                            UFId = ufId
                        };
                        listaCidades.Add(cidade);
                    }
                }
            }

            return Json(listaCidades);
        }

        private List<SelectListItem> FillGrauParentesco()
        {
            List<SelectListItem> ufs = new();
            HttpResponseMessage message = _httpClient.GetAsync($"uf/getall").Result;
            string content = message.Content.ReadAsStringAsync().Result;
            ResultModel<UfViewModel>? response = JsonConvert.DeserializeObject<ResultModel<UfViewModel>>(content);

            if (message.IsSuccessStatusCode)
            {
                if (!response?.HasError ?? false)
                {
                    foreach (UfViewModel item in response?.Items!)
                    {
                        SelectListItem select = new() { Text = item.Sigla, Value = item.Id.ToString() };
                        ufs.Add(select);
                    }

                    ufs.Insert(0, new SelectListItem() { Text = "[Selecione]", Value = "-1" });
                }
            }

            return ufs;
        }
    }
}