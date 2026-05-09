using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace RunalyzeMcp
{
    public class RunalyzeApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _token;
        public RunalyzeApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = Environment.GetEnvironmentVariable("RUNALYZE_BASE_URL") ?? "https://runalyze.com";
            _token = Environment.GetEnvironmentVariable("RUNALYZE_TOKEN") ?? "";
        }
        private void AddTokenHeader()
        {
            _httpClient.DefaultRequestHeaders.Remove("token");
            _httpClient.DefaultRequestHeaders.Add("token", _token);
        }
        private void AddAcceptHeaders()
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/ld+json"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        private void AddAcceptHeadersForCsv()
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/csv"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/ld+json"));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        #region Activity Endpoints
        public async Task<HttpResponseMessage> UploadActivityAsync(string base64File, string? title = null, string? note = null, string? route = null, int? elevationUp = null, int? elevationDown = null)
        {
            AddTokenHeader();
            AddAcceptHeaders();
            var url = $"{_baseUrl}/api/v1/activities/uploads";
            using var content = new MultipartFormDataContent();
            var fileBytes = Convert.FromBase64String(base64File);
            content.Add(new ByteArrayContent(fileBytes), "file", "activity.fit");
            if (!string.IsNullOrEmpty(title)) content.Add(new StringContent(title), "title");
            if (!string.IsNullOrEmpty(note)) content.Add(new StringContent(note), "note");
            if (!string.IsNullOrEmpty(route)) content.Add(new StringContent(route), "route");
            if (elevationUp.HasValue) content.Add(new StringContent(elevationUp.Value.ToString()), "elevation_up_file");
            if (elevationDown.HasValue) content.Add(new StringContent(elevationDown.Value.ToString()), "elevation_down_file");
            return await _httpClient.PostAsync(url, content);
        }
        public async Task<HttpResponseMessage> GetActivityUploadStatusAsync(string id)
        {
            AddTokenHeader();
            AddAcceptHeaders();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/activities/uploads/{id}");
        }
        public async Task<HttpResponseMessage> GetActivitiesAsync(int? page = null, string? orderById = null)
        {
            AddTokenHeader();
            AddAcceptHeadersForCsv();
            var queryParams = new List<string>();
            if (page.HasValue) queryParams.Add($"page={page.Value}");
            if (!string.IsNullOrEmpty(orderById)) queryParams.Add($"order[id]={orderById}");
            var url = $"{_baseUrl}/api/v1/activity";
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> GetActivityAsync(string id)
        {
            AddTokenHeader();
            AddAcceptHeadersForCsv();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/activity/{id}");
        }
        #endregion
        #region Activity Download Endpoints
        public async Task<HttpResponseMessage> DownloadFitOriginalAsync(string id)
        {
            AddTokenHeader();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/activity/{id}/fit");
        }
        public async Task<HttpResponseMessage> DownloadFitlogAsync(string id)
        {
            AddTokenHeader();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/activity/{id}/fitlog");
        }
        public async Task<HttpResponseMessage> DownloadGpxAsync(string id)
        {
            AddTokenHeader();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/gpx+xml"));
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/activity/{id}/gpx");
        }
        public async Task<HttpResponseMessage> DownloadKmlAsync(string id)
        {
            AddTokenHeader();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.google-earth.kml+xml"));
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/activity/{id}/kml");
        }
        public async Task<HttpResponseMessage> DownloadSocialImageAsync(string id)
        {
            AddTokenHeader();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/png"));
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/activity/{id}/social-image");
        }
        public async Task<HttpResponseMessage> DownloadTcxAsync(string id)
        {
            AddTokenHeader();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.garmin.tcx+xml"));
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/activity/{id}/tcx");
        }
        #endregion
        #region Statistics Endpoints
        public async Task<HttpResponseMessage> GetCurrentStatisticsAsync()
        {
            AddTokenHeader();
            AddAcceptHeaders();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/statistics/current");
        }
        #endregion
        #region Equipment Endpoints
        public async Task<HttpResponseMessage> GetEquipmentAsync(int? page = null, string? orderById = null)
        {
            AddTokenHeader();
            AddAcceptHeaders();
            var queryParams = new List<string>();
            if (page.HasValue) queryParams.Add($"page={page.Value}");
            if (!string.IsNullOrEmpty(orderById)) queryParams.Add($"order[id]={orderById}");
            var url = $"{_baseUrl}/api/v1/equipment";
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> GetEquipmentByIdAsync(string id)
        {
            AddTokenHeader();
            AddAcceptHeaders();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/equipment/{id}");
        }
        public async Task<HttpResponseMessage> GetEquipmentCategoriesAsync(int? page = null, string? orderById = null)
        {
            AddTokenHeader();
            AddAcceptHeaders();
            var queryParams = new List<string>();
            if (page.HasValue) queryParams.Add($"page={page.Value}");
            if (!string.IsNullOrEmpty(orderById)) queryParams.Add($"order[id]={orderById}");
            var url = $"{_baseUrl}/api/v1/equipment/category";
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> GetEquipmentCategoryByIdAsync(string id)
        {
            AddTokenHeader();
            AddAcceptHeaders();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/equipment/category/{id}");
        }
        #endregion
        #region Health Endpoints
        public async Task<HttpResponseMessage> BulkUploadHealthAsync(string base64File)
        {
            AddTokenHeader();
            AddAcceptHeaders();
            var url = $"{_baseUrl}/api/v1/health/bulk-upload";
            using var content = new MultipartFormDataContent();
            var fileBytes = Convert.FromBase64String(base64File);
            content.Add(new ByteArrayContent(fileBytes), "file", "health.csv");
            return await _httpClient.PostAsync(url, content);
        }
        #endregion
        #region Metrics Endpoints
        public async Task<HttpResponseMessage> GetBloodGlucoseMetricsAsync(int? page = null, string? orderById = null)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var queryParams = new List<string>();
            if (page.HasValue) queryParams.Add($"page={page.Value}");
            if (!string.IsNullOrEmpty(orderById)) queryParams.Add($"order[id]={orderById}");
            var url = $"{_baseUrl}/api/v1/metrics/blood-glucose";
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> CreateBloodGlucoseMetricAsync(object metricData)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var json = JsonSerializer.Serialize(metricData);
            return await _httpClient.PostAsync($"{_baseUrl}/api/v1/metrics/blood-glucose", new StringContent(json, Encoding.UTF8, "application/json"));
        }
        public async Task<HttpResponseMessage> GetBloodGlucoseMetricByIdAsync(string id)
        {
            AddTokenHeader(); AddAcceptHeaders();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/metrics/blood-glucose/{id}");
        }
        public async Task<HttpResponseMessage> GetBloodPressureMetricsAsync(int? page = null, string? orderById = null)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var queryParams = new List<string>();
            if (page.HasValue) queryParams.Add($"page={page.Value}");
            if (!string.IsNullOrEmpty(orderById)) queryParams.Add($"order[id]={orderById}");
            var url = $"{_baseUrl}/api/v1/metrics/blood-pressure";
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> CreateBloodPressureMetricAsync(object metricData)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var json = JsonSerializer.Serialize(metricData);
            return await _httpClient.PostAsync($"{_baseUrl}/api/v1/metrics/blood-pressure", new StringContent(json, Encoding.UTF8, "application/json"));
        }
        public async Task<HttpResponseMessage> GetBloodPressureMetricByIdAsync(string id)
        {
            AddTokenHeader(); AddAcceptHeaders();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/metrics/blood-pressure/{id}");
        }
        public async Task<HttpResponseMessage> GetBodyCompositionMetricsAsync(int? page = null, string? orderById = null)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var queryParams = new List<string>();
            if (page.HasValue) queryParams.Add($"page={page.Value}");
            if (!string.IsNullOrEmpty(orderById)) queryParams.Add($"order[id]={orderById}");
            var url = $"{_baseUrl}/api/v1/metrics/body-composition";
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> CreateBodyCompositionMetricAsync(object metricData)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var json = JsonSerializer.Serialize(metricData);
            return await _httpClient.PostAsync($"{_baseUrl}/api/v1/metrics/body-composition", new StringContent(json, Encoding.UTF8, "application/json"));
        }
        public async Task<HttpResponseMessage> GetBodyCompositionMetricByIdAsync(string id)
        {
            AddTokenHeader(); AddAcceptHeaders();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/metrics/body-composition/{id}");
        }
        public async Task<HttpResponseMessage> GetBodyTemperatureMetricsAsync(int? page = null, string? orderById = null)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var queryParams = new List<string>();
            if (page.HasValue) queryParams.Add($"page={page.Value}");
            if (!string.IsNullOrEmpty(orderById)) queryParams.Add($"order[id]={orderById}");
            var url = $"{_baseUrl}/api/v1/metrics/body-temperature";
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> CreateBodyTemperatureMetricAsync(object metricData)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var json = JsonSerializer.Serialize(metricData);
            return await _httpClient.PostAsync($"{_baseUrl}/api/v1/metrics/body-temperature", new StringContent(json, Encoding.UTF8, "application/json"));
        }
        public async Task<HttpResponseMessage> GetBodyTemperatureMetricByIdAsync(string id)
        {
            AddTokenHeader(); AddAcceptHeaders();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/metrics/body-temperature/{id}");
        }
        public async Task<HttpResponseMessage> GetDailyNoteMetricsAsync(int? page = null, string? orderById = null)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var queryParams = new List<string>();
            if (page.HasValue) queryParams.Add($"page={page.Value}");
            if (!string.IsNullOrEmpty(orderById)) queryParams.Add($"order[id]={orderById}");
            var url = $"{_baseUrl}/api/v1/metrics/daily-note";
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> CreateDailyNoteMetricAsync(object metricData)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var json = JsonSerializer.Serialize(metricData);
            return await _httpClient.PostAsync($"{_baseUrl}/api/v1/metrics/daily-note", new StringContent(json, Encoding.UTF8, "application/json"));
        }
        public async Task<HttpResponseMessage> GetDailyNoteByDateAsync(string date)
        {
            AddTokenHeader(); AddAcceptHeaders();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/metrics/daily-note/{date}");
        }
        public async Task<HttpResponseMessage> GetHrvMetricsAsync(int? page = null, string? orderById = null)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var queryParams = new List<string>();
            if (page.HasValue) queryParams.Add($"page={page.Value}");
            if (!string.IsNullOrEmpty(orderById)) queryParams.Add($"order[id]={orderById}");
            var url = $"{_baseUrl}/api/v1/metrics/hrv";
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> CreateHrvMetricAsync(object metricData)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var json = JsonSerializer.Serialize(metricData);
            return await _httpClient.PostAsync($"{_baseUrl}/api/v1/metrics/hrv", new StringContent(json, Encoding.UTF8, "application/json"));
        }
        public async Task<HttpResponseMessage> GetHrvMetricByIdAsync(string id)
        {
            AddTokenHeader(); AddAcceptHeaders();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/metrics/hrv/{id}");
        }
        public async Task<HttpResponseMessage> GetHeartRateMaxMetricsAsync(int? page = null, string? orderById = null)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var queryParams = new List<string>();
            if (page.HasValue) queryParams.Add($"page={page.Value}");
            if (!string.IsNullOrEmpty(orderById)) queryParams.Add($"order[id]={orderById}");
            var url = $"{_baseUrl}/api/v1/metrics/heart-rate-max";
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> CreateHeartRateMaxMetricAsync(object metricData)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var json = JsonSerializer.Serialize(metricData);
            return await _httpClient.PostAsync($"{_baseUrl}/api/v1/metrics/heart-rate-max", new StringContent(json, Encoding.UTF8, "application/json"));
        }
        public async Task<HttpResponseMessage> GetHeartRateMaxMetricByIdAsync(string id)
        {
            AddTokenHeader(); AddAcceptHeaders();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/metrics/heart-rate-max/{id}");
        }
        public async Task<HttpResponseMessage> GetHeartRateRestMetricsAsync(int? page = null, string? orderById = null)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var queryParams = new List<string>();
            if (page.HasValue) queryParams.Add($"page={page.Value}");
            if (!string.IsNullOrEmpty(orderById)) queryParams.Add($"order[id]={orderById}");
            var url = $"{_baseUrl}/api/v1/metrics/heart-rate-rest";
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> CreateHeartRateRestMetricAsync(object metricData)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var json = JsonSerializer.Serialize(metricData);
            return await _httpClient.PostAsync($"{_baseUrl}/api/v1/metrics/heart-rate-rest", new StringContent(json, Encoding.UTF8, "application/json"));
        }
        public async Task<HttpResponseMessage> GetHeartRateRestMetricByIdAsync(string id)
        {
            AddTokenHeader(); AddAcceptHeaders();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/metrics/heart-rate-rest/{id}");
        }
        public async Task<HttpResponseMessage> GetMentalMetricsAsync(int? page = null, string? orderById = null)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var queryParams = new List<string>();
            if (page.HasValue) queryParams.Add($"page={page.Value}");
            if (!string.IsNullOrEmpty(orderById)) queryParams.Add($"order[id]={orderById}");
            var url = $"{_baseUrl}/api/v1/metrics/mental";
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> CreateMentalMetricAsync(object metricData)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var json = JsonSerializer.Serialize(metricData);
            return await _httpClient.PostAsync($"{_baseUrl}/api/v1/metrics/mental", new StringContent(json, Encoding.UTF8, "application/json"));
        }
        public async Task<HttpResponseMessage> GetMentalMetricByDateAsync(string date)
        {
            AddTokenHeader(); AddAcceptHeaders();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/metrics/mental/{date}");
        }
        public async Task<HttpResponseMessage> GetSleepMetricsAsync(int? page = null, string? orderById = null)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var queryParams = new List<string>();
            if (page.HasValue) queryParams.Add($"page={page.Value}");
            if (!string.IsNullOrEmpty(orderById)) queryParams.Add($"order[id]={orderById}");
            var url = $"{_baseUrl}/api/v1/metrics/sleep";
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> CreateSleepMetricAsync(object metricData)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var json = JsonSerializer.Serialize(metricData);
            return await _httpClient.PostAsync($"{_baseUrl}/api/v1/metrics/sleep", new StringContent(json, Encoding.UTF8, "application/json"));
        }
        public async Task<HttpResponseMessage> GetSleepMetricByIdAsync(string id)
        {
            AddTokenHeader(); AddAcceptHeaders();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/metrics/sleep/{id}");
        }
        #endregion
        #region Race Result Endpoints
        public async Task<HttpResponseMessage> GetRaceResultsAsync(int? page = null, string? orderById = null)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var queryParams = new List<string>();
            if (page.HasValue) queryParams.Add($"page={page.Value}");
            if (!string.IsNullOrEmpty(orderById)) queryParams.Add($"order[id]={orderById}");
            var url = $"{_baseUrl}/api/v1/race-result";
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> GetRaceResultByActivityAsync(string activityId)
        {
            AddTokenHeader(); AddAcceptHeaders();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/race-result/activity/{activityId}");
        }
        #endregion
        #region Tag Endpoints
        public async Task<HttpResponseMessage> GetTagsAsync(int? page = null, string? orderById = null)
        {
            AddTokenHeader(); AddAcceptHeaders();
            var queryParams = new List<string>();
            if (page.HasValue) queryParams.Add($"page={page.Value}");
            if (!string.IsNullOrEmpty(orderById)) queryParams.Add($"order[id]={orderById}");
            var url = $"{_baseUrl}/api/v1/tag";
            if (queryParams.Count > 0) url += "?" + string.Join("&", queryParams);
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> GetTagByIdAsync(string id)
        {
            AddTokenHeader(); AddAcceptHeaders();
            return await _httpClient.GetAsync($"{_baseUrl}/api/v1/tag/{id}");
        }
        #endregion
    }
}
