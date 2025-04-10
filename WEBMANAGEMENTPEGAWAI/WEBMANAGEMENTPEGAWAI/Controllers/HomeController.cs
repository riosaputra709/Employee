using System;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WEBMANAGEMENTPEGAWAI.Models;

namespace WEBMANAGEMENTPEGAWAI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiUrl = "https://localhost:7266/api/Employee/";

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            await doSearch(null, null, null);
            return View();
        }

        public async Task doSearch(string? name, DateTime? tglAwal, DateTime? tglAkhir)
        {
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response;
            string url = _apiUrl + "GetPegawaiDetailsSP?name=";

            if (tglAwal != null)
                url += "&tanggalAwal=" + tglAwal.Value.ToString("yyyy-MM-dd");

            if (tglAkhir != null)
                url += "&tanggalAkhir=" + tglAkhir.Value.ToString("yyyy-MM-dd");


            response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<PegawaiModel> data = JsonConvert.DeserializeObject<List<PegawaiModel>>(content);

                ViewData["pegawais"] = data;
            }
        }

        public async Task<ActionResult> Search(string? name, DateTime? tglAwal, DateTime? tglAkhir)
        {
            try
            {
                await doSearch(name, tglAwal, tglAkhir);
            }
            catch (Exception ex)
            {
                return Json("Error : " + ex.Message);
            }

            return PartialView("_GridView");
        }

        public async Task<ActionResult> Delete(string kdPegawai)
        {
            AjaxResult ajaxResult = new AjaxResult();
            ajaxResult.ErrMesgs = new string[1];
            ajaxResult.SuccMesgs = new string[1];
            ajaxResult.Result = AjaxResult.VALUE_ERROR;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Ganti URL di bawah ini sesuai dengan endpoint API kamu
                    string apiUrl = _apiUrl + $"DeletePegawai/{kdPegawai}";

                    var response = await httpClient.DeleteAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        ajaxResult.SuccMesgs[0] = "sukses delete data";
                    }
                    else
                    {
                        // Menangani jika terjadi kegagalan
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        throw new Exception(errorMessage);
                    }
                }
                ajaxResult.Result = AjaxResult.VALUE_SUCCESS;
            }
            catch (Exception ex)
            {
                ajaxResult.ErrMesgs[0] = ex.Message;
                ajaxResult.Result = AjaxResult.VALUE_ERROR;
            }
            return Json(ajaxResult);
        }

        public async Task<ActionResult> Edit(PegawaiModel pegawai)
        {
            AjaxResult ajaxResult = new AjaxResult();
            ajaxResult.ErrMesgs = new string[1];
            ajaxResult.SuccMesgs = new string[1];
            ajaxResult.Result = AjaxResult.VALUE_ERROR;

            try
            {
                var data = new
                {
                    kodePegawai = pegawai.KodePegawai ,
                    namaPegawai = pegawai.NamaPegawai,
                    tanggalMulaiKontrak = pegawai.TanggalMulaiKontrak,
                    tanggalHabisKontrak = pegawai.TanggalHabisKontrak,
                    kodeCabang = pegawai.KodeCabang,
                    kodeJabatan = pegawai.KodeJabatan,
                };

                // Serialisasi objek menjadi string JSON
                var json = JsonConvert.SerializeObject(data);

                using (HttpClient client = new HttpClient())
                {
                    string url = _apiUrl + $"UpdatePegawai/{pegawai.KodePegawai}";
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Mengirim PUT request
                    var response = await client.PutAsync(url, content);


                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        ajaxResult.SuccMesgs[0] = "sukses update data";
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        throw new Exception(errorMessage);
                    }
                }
                ajaxResult.Result = AjaxResult.VALUE_SUCCESS;
            }
            catch (Exception ex)
            {
                ajaxResult.ErrMesgs[0] = ex.Message;
                ajaxResult.Result = AjaxResult.VALUE_ERROR;
            }
            return Json(ajaxResult);
        }


        [HttpPost]
        public async Task<IActionResult> UploadXls(IFormFile fileUpload)
        {
            if (fileUpload == null || fileUpload.Length == 0)
            {
                TempData["Error"] = "File kosong atau belum dipilih.";
                return RedirectToAction("Index");
            }

            using (var httpClient = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new StreamContent(fileUpload.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.ms-excel");
                    content.Add(fileContent, "file", fileUpload.FileName);

                    var apiUrl = _apiUrl + "UploadDataPegawai";

                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Success"] = "File berhasil dikirim ke API.";
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        TempData["Error"] = errorMessage;
                    }
                }
            }

            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
