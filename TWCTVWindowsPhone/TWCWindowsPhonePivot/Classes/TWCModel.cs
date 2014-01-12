using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using TWCWindowsPhonePivot.Helpers;

namespace TWCWindowsPhonePivot.Classes
{
    public class TWCModel
    {
        public string SessionId { get; set; }
        public string DeviceId { get; set; }
        public string Wayfarer { get; set; }
        public string Vs_Guid { get; set; }
        public StbInfo Boxes { get; set; }
        public List<Channel> Channels { get; set; }
        public List<Channel> StreamingChannels { get; set; }
        public List<Show> Shows { get; set; }
        public List<Recording> Recordings { get; set; }
        public List<ShowImage> ShowImages { get; set; }

        public HeaderOptions HeaderOptions { get; set; }
        public ClientDetails ClientDetails { get; set; }

        public string HeaderOptionsJson
        {
            get
            {
                JsonSerializerSettings mySettings = new JsonSerializerSettings();
                mySettings.NullValueHandling = NullValueHandling.Ignore;
                return JsonConvert.SerializeObject(HeaderOptions, mySettings);
            }
        }

        public string ClientDetailsJson
        {
            get
            {
                JsonSerializerSettings mySettings = new JsonSerializerSettings();
                mySettings.NullValueHandling = NullValueHandling.Ignore;
                return JsonConvert.SerializeObject(ClientDetails, mySettings);
            }
        }


        public static string GetGuidSessionId()
        {
            string returnString = "";
            //6ab724f4-0274-44b5-bef2-8c0777b82dc9

            returnString = Guid.NewGuid().ToString().Replace("-", "");

            returnString = returnString.Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-");

            return returnString;
        }

        public static string GetGuidDeviceId()
        {
            string returnString = "";
            //5572-239ffcbd-e8b6d19d-7bb56ee1-b631

            returnString = Guid.NewGuid().ToString().Replace("-", "");

            returnString = returnString.Insert(4, "-").Insert(13, "-").Insert(22, "-").Insert(31, "-");

            return returnString;
        }
    }


    public class ShowImage
    {
        public string EpisodeId { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public byte[] EpisodeImageBytes { get; set; }
        public BitmapImage EpisodeBitmapImage { get; set; }
    }

    
public class Recording
{
    public string description { get; set; }
    public string episodeId { get; set; }
    public string airDate { get; set; }
    public int startTime { get; set; }
    public string serviceId { get; set; }
    public string airTime { get; set; }
    public string title { get; set; }
    public int stopAdjust { get; set; }
    public string saveOptions { get; set; }
    public string stationNumber { get; set; }
    public string liveTapeDelay { get; set; }
    public int startAdjust { get; set; }
    public string repeatOptions { get; set; }
    public string eventId { get; set; }
    public string channelId { get; set; }
    public string eventType { get; set; }
    public int gmTime { get; set; }
    public int duration { get; set; }
    public string numberOfEpisodes { get; set; }
}

public class RecordingObject
{
    public List<Recording> recordings { get; set; }
}

    public class Show
    {
        public string title { get; set; }
        public string episodeTitle { get; set; }
        public string episodeId { get; set; }
        public string duration { get; set; }
        public string airDateTime { get; set; }
        public string gmTime { get; set; }
        public string channelId { get; set; }
        public string callSign { get; set; }
        public string shortDesc { get; set; }
        public string rating { get; set; }
        public string starRating { get; set; }
        public string serviceId { get; set; }
        public List<string> icons { get; set; }

        public DateTime gmDateTime { get { return TWCDateHelper.FromUnixTime((long)Convert.ToDouble(gmTime)); } }
        public int channelIdInt
        {
            get
            {
                int returnChannel = -1;
                try
                {
                    returnChannel = Convert.ToInt32(channelId);
                }
                catch (Exception) { }

                return returnChannel;
            }
        }
    }

    public class ShowData
    {
        public List<Show> shows { get; set; }
    }

    public class ShowDataObject
    {
        public string error { get; set; }
        public ShowData data { get; set; }
    }


    public class Channel
    {
        public bool availableOutOfHome { get; set; }
        public List<Stream> streams { get; set; }
        public string tmsId { get; set; }
        public bool hd { get; set; }
        public string callSign { get; set; }
        public List<object> logos { get; set; }
        public string streamTmsId { get; set; }
        public string networkName { get; set; }
        public int channelId { get; set; }
        public string logoUrl { get; set; }
        public bool authorizedForStream { get; set; }

        public int channelIdInt
        {
            get
            {
                int returnChannelId = -1;
                try
                {
                    returnChannelId = Convert.ToInt32(channelId);
                }
                catch (Exception e)
                { }

                return returnChannelId;
            }
        }
    }

    public class ChanelObject
    {
        public List<Channel> channels { get; set; }
    }

    public class SetTopBox
    {
        public string clientType { get; set; }
        public string clientVersion { get; set; }
        public string macAddress { get; set; }
        public string name { get; set; }
        public string lineupId { get; set; }
        public string headend { get; set; }
        public bool isDvr { get; set; }
        public bool dvr { get; set; }
    }

    public class StbInfo
    {
        public string mas { get; set; }
        public List<SetTopBox> setTopBoxes { get; set; }
    }

    public class StbInfoObject
    {
        public StbInfo stbInfo { get; set; }
    }

    public class HeaderOptions
    {
        public string appStartup { get; set; }
        public string sessionID { get; set; }
        public string previousSessionID { get; set; }
        public int UTCOffset { get; set; }
        public string targetDataConsumers { get; set; }
        public int logLevel { get; set; }
        public string headendID { get; set; }
        public string MAS { get; set; }
        public string date { get; set; }

    }

    public class ClientDetails
    {
        public string applicationName { get; set; }
        public string applicationVersion { get; set; }
        public string apiVersion { get; set; }
        public string formFactor { get; set; }
        public TriggeredBy triggeredBy { get; set; }
        public string deviceModel { get; set; }
        public string deviceOS { get; set; }
        public string deviceID { get; set; }

    }

    public class networkConnection
    {

    }

    public class TriggeredBy
    {
        public string initiator { get; set; }
        public string link { get; set; }
    }

    public class messages
    {
        public string timestamp { get; set; }
        public eventMetadata eventMetaData { get; set; }
        public string type { get; set; }
        public string triggeredBy { get; set; }
        public string username { get; set; }
        public string previousEventSentAt { get; set; }
    }

    public class eventMetadata
    {
        public string functionName { get; set; }
        public string uri { get; set; }
        public string parentType { get; set; }
        public string[] path { get; set; }
        public int sequenceNumber { get; set; }
    }

    public class ChannelStreamInfo
    {
        public string iv { get; set; }
        public string key_url { get; set; }
        public string stream_url { get; set; }

    }

    public class Stream
    {
        public string type { get; set; }
        public bool drm { get; set; }
        public string uri { get; set; }
    }

    public class StreamChannels
    {
        public List<Channel> channels { get; set; }
    }

}
