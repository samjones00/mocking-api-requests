using Api.Interfaces;
using Api.Services;
using App.Models;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class WeatherServiceTests
    {
        private IWeatherService weatherService;

        [Fact]
        public async Task GetWeather_GivenValidLocation_ShouldReturnSuccess()
        {
            var handlerMock = CreateMockHttpMessageHandler(HttpStatusCode.OK, new StringContent("{'coord':{'lon':-0.13,'lat':51.51},'weather':[{'id':721,'main':'Haze','description':'haze','icon':'50n'}],'base':'stations','main':{'temp':1.87,'feels_like':-1.29,'temp_min':1.67,'temp_max':2.78,'pressure':1002,'humidity':100},'visibility':1700,'wind':{'speed':2.1,'deg':330},'clouds':{'all':90},'dt':1607282907,'sys':{'type':1,'id':1414,'country':'GB','sunrise':1607241062,'sunset':1607269962},'timezone':0,'id':2643743,'name':'London','cod':200}"));

            var request = new WeatherServiceRequest
            {
                Location = "London",
                BaseUrl = "http://test.com/",
                AppId = "123"
            };

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(request.BaseUrl),
            };

            weatherService = new WeatherService(httpClient);

            var result = await weatherService.GetWeatherAsync(request);

            handlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri($"{httpClient.BaseAddress}{request.ResolveUrl}")), ItExpr.IsAny<CancellationToken>());

            Assert.Equal("London", result.name);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetWeather_GivenInvalidUrl_ShouldReturnNotFound()
        {
            var handlerMock = CreateMockHttpMessageHandler(HttpStatusCode.NotFound);

            var request = new WeatherServiceRequest
            {
                Location = "london",
                BaseUrl = "http://test.com/",
                AppId = "123"
            };

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(request.BaseUrl),
            };

            weatherService = new WeatherService(httpClient);

            _ = Assert.ThrowsAsync<Exception>(() => weatherService.GetWeatherAsync(request));

            handlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri(httpClient.BaseAddress, request.ResolveUrl)), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public void GetWeather_GivenNoAppId_ShouldReturnForbidden()
        {
            var handlerMock = CreateMockHttpMessageHandler(HttpStatusCode.Unauthorized);

            var request = new WeatherServiceRequest
            {
                Location = "London",
                BaseUrl = "http://test.com/",
                AppId = string.Empty
            };

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(request.BaseUrl),
            };

            weatherService = new WeatherService(httpClient);

            _ = Assert.ThrowsAsync<Exception>(() => weatherService.GetWeatherAsync(request));

            handlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri == new Uri($"{httpClient.BaseAddress}{request.ResolveUrl}")), ItExpr.IsAny<CancellationToken>());
        }

        private Mock<HttpMessageHandler> CreateMockHttpMessageHandler(HttpStatusCode statusCode, HttpContent content = null)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = statusCode,
                   Content = content
               })
               .Verifiable();

            return handlerMock;
        }
    }
}
