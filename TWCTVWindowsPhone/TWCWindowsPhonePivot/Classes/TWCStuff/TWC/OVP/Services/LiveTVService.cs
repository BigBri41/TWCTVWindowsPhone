namespace TWC.OVP.Services
{
    using Caliburn.Micro;
    using System;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    public static class LiveTVService
    {
        public static readonly string AegisRefresh = "https://services.timewarnercable.com/api/smarttv/aegis/v1/refresh";
        public static readonly string BehindOwnModemUri = "https://services.timewarnercable.com/api/smarttv/behindOwnModem/v1";
        public static readonly string CDNStreamInfoUriPattern = "https://services.timewarnercable.com/api/live/v1/channels/{0}/streams/smooth";
        public static readonly string ChannelsUri = "https://services.timewarnercable.com/api/smarttv/live/v1/channels";
        public static readonly string GenreDataUri = "http://genres.timewarnercable.com/api/genres?format=json";
        public static readonly string GenreServicesUri = "http://genres.timewarnercable.com/api/services?format=json";
        public static readonly string LocationUri = "https://services.timewarnercable.com/api/smarttv/location/v1";
        public static readonly string OVPDomain = "https://services.timewarnercable.com";
        public static readonly string WhatsOnUri = "https://video-services.timewarnercable.com/api/v3/secured/guide/streaming";

        public static IResult CheckBehindOwnModem(Action<BehindOwnModem> callback, Action<Exception> exceptionHandler = null)
        {
            return JsonService.CreateJsonRequest<BehindOwnModem>(BehindOwnModemUri, callback, null, exceptionHandler, 0x2710, null);
        }

        public static IResult CheckLocation(Action<Location> callback, Action<Exception> exceptionHandler = null)
        {
            return JsonService.CreateJsonRequest<Location>(LocationUri, callback, null, exceptionHandler, 0x2710, null);
        }

        private static string HackUglyJsonWhatsOn(string json)
        {
            return (Regex.Replace(json, "(\"[0-9]+\"):\\[", "\"item\":[", RegexOptions.None).Replace("{\"item\"", "{\"items\":[{\"item\"").Replace("],\"item\":", "]},{\"item\":") + "]}");
        }

        public static IResult LoadChannels(Action<ChannelList> callback, Action<Exception> exceptionHandler = null)
        {
            return JsonService.CreateJsonRequest<ChannelList>(ChannelsUri, callback, null, exceptionHandler, 0x2710, null);
        }

        public static JsonRequestResult<ChannelList> LoadChannels1()
        {
            return OVPClientHttpRequest.JsonRequest<ChannelList>(ChannelsUri, 0x1388, false, null);
        }

        public static IResult LoadEpisodeDetails(string airDate, string episodeID, int serviceID, Action<EpisodeData> callback, Action<Exception> exceptionHandler = null)
        {
            return JsonService.CreateJsonRequest<EpisodeData>(string.Format("https://dvr.timewarnercable.com/muse/data/detailedEpisodeInfo/?apicall=detail-episode&headEnd=STREAM&airDateTime={0}&episodeId={1}&serviceId={2}", airDate, episodeID, serviceID), callback, null, exceptionHandler, 0x2710, null);
        }

        public static IResult LoadEpisodeDetailsNMD(string episoldeId, Action<Program> callback, Action<Exception> exceptionHandler = null)
        {
            return JsonService.CreateJsonRequest<Program>(string.Format("https://services.timewarnercable.com/nmd/program/{0}?_type=json", episoldeId), callback, null, exceptionHandler, 0x2710, null);
        }

        public static IResult LoadGenreData(Action<GenreData> callback, Action<Exception> exceptionHandler = null, System.Func<string, string> processJson = null)
        {
            return JsonService.CreateJsonRequest<GenreData>(GenreDataUri, callback, null, exceptionHandler, 0x2710, null);
        }

        public static IResult LoadGenreServices(Action<GenreServiceData> callback, Action<Exception> exceptionHandler = null, System.Func<string, string> processJson = null)
        {
            return JsonService.CreateJsonRequest<GenreServiceData>(GenreServicesUri, callback, null, exceptionHandler, 0x2710, processJson);
        }

        public static IResult LoadWhatsOn(Action<WhatsOnList> callback, Action<Exception> exceptionHandler = null)
        {
            return JsonService.CreateJsonRequest<WhatsOnList>(WhatsOnUri, callback, null, exceptionHandler, 0x2710, new System.Func<string, string>(LiveTVService.HackUglyJsonWhatsOn));
        }
    }
}

