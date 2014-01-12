using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using TWCWindowsPhonePivot.Helpers;
using TWCWindowsPhonePivot.Classes;
using TWCWindowsPhonePivot.Models;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.IO;


namespace TWCWindowsPhonePivot
{
    public partial class MainPage : PhoneApplicationPage
    {
        TWCStaticCalls myCalls = new TWCStaticCalls();
        StreamingHelper streamingHelper = new StreamingHelper();
        TWCModel myModel = new TWCModel();
        string userName = "";
        string password = "";
        bool rememberPassword = false;
        bool displayOnlyHDChannels = false;
        List<ItemViewModel> channelListBoxComplete = new List<ItemViewModel>();
        bool isLoggedIn = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            /*
                //you can add your own username and password here to allow quicker debugging (just don't check it into source control) 
             
            IsolatedStorageSettings.ApplicationSettings["userName"] = "";
            IsolatedStorageSettings.ApplicationSettings["password"] = "";
            IsolatedStorageSettings.ApplicationSettings["rememberPassword"] = true;
            IsolatedStorageSettings.ApplicationSettings.Save();
            */
            string deviceId = "";


            try
            {
                displayOnlyHDChannels = Convert.ToBoolean(IsolatedStorageSettings.ApplicationSettings["displayOnlyHDChannels"].ToString());
                displayOnlyHDChannelsCheckBox.IsChecked = displayOnlyHDChannels;
            }
            catch (Exception) {
                displayOnlyHDChannels = false;
                IsolatedStorageSettings.ApplicationSettings["displayOnlyHDChannels"] = false;
                IsolatedStorageSettings.ApplicationSettings.Save();
                displayOnlyHDChannelsCheckBox.IsChecked = false;
            }
          
            try
            {
                deviceId = IsolatedStorageSettings.ApplicationSettings["deviceId"].ToString();
            }
            catch (Exception) { }


            try
            {
                userName = IsolatedStorageSettings.ApplicationSettings["userName"].ToString();
            }
            catch (Exception) { }

            try
            {
                password = IsolatedStorageSettings.ApplicationSettings["password"].ToString();
            }
            catch (Exception) { }

            try
            {
                rememberPassword = Convert.ToBoolean(IsolatedStorageSettings.ApplicationSettings["rememberPassword"].ToString());
            }
            catch (Exception) { }


            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = TWCModel.GetGuidDeviceId();
                IsolatedStorageSettings.ApplicationSettings.Add("deviceId", deviceId);
                IsolatedStorageSettings.ApplicationSettings.Save();
            }


            string sessionId = TWCModel.GetGuidSessionId();

            myModel.DeviceId = deviceId;
            myModel.SessionId = sessionId;

            HeaderOptions myOptions = new HeaderOptions();


            myOptions.appStartup = TWCDateHelper.ConvertToTWCDate(DateTime.Now.ToUniversalTime());
            myOptions.previousSessionID = "";
            myOptions.UTCOffset = 300;
            myOptions.targetDataConsumers = "PRODUCTION";
            myOptions.logLevel = 0;
            myOptions.sessionID = sessionId;

            myModel.HeaderOptions = myOptions;

            ClientDetails details = new ClientDetails();
            details.applicationName = "OVP";
            details.applicationVersion = "2.3.0.1";
            details.apiVersion = "2.3.0.1";
            details.formFactor = "PC";
            details.deviceModel = "Win32";
            details.deviceOS = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; Media Center PC 6.0; MS-RTC LM 8; .NET CLR 3.5.30729; .NET CLR 3.0.30729; .NET4.0E; .NET CLR 1.1.4322; InfoPath.3; .NET4.0C; Zune 4.7)";
            details.deviceID = deviceId;

            TriggeredBy triggered = new TriggeredBy();
            triggered.initiator = "user";
            triggered.link = "undefined";

            details.triggeredBy = triggered;

            myModel.ClientDetails = details;

            myCalls.LoginFinished += new EventHandler(myCalls_LoginFinished);
            myCalls.ChangeChannelFinished += new EventHandler(myCalls_ChangeChannelFinished);
            myCalls.GetDevicesFinished += new EventHandler(myCalls_GetDevicesFinished);
            myCalls.GetGuideFinished += new EventHandler(myCalls_GetGuideFinished);
            myCalls.GetShowsFinished += new EventHandler(myCalls_GetShowsFinished);
            myCalls.GetRecordingsFinished += new EventHandler(myCalls_GetRecordingsFinished);
            myCalls.GetShowImageFinished += new EventHandler(myCalls_GetShowImageFinished);
            streamingHelper.GetStreamingChannelsFinished += new EventHandler(streamingHelper_GetStreamingChannelsFinished);

            if (!string.IsNullOrEmpty(userName))
            {
                userNameTextBlock.Text = userName;
            }

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                alertTextBox.Text = "Logging In...";
                passwordTextBox.Password = password;
                rememberPasswordCheckBox.IsChecked = true;
                loginGridStatic.Visibility = System.Windows.Visibility.Collapsed;
                alertTextBox.Visibility = System.Windows.Visibility.Visible;
                myCalls.LoginGetWayfarer(userName, password, myModel);
            }

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void streamingHelper_GetStreamingChannelsFinished(object sender, EventArgs e)
        {
            List<Channel> streamingChannels = (List<Channel>)sender;

            myModel.StreamingChannels = streamingChannels;
            myCalls.Model.StreamingChannels = streamingChannels;

            myCalls.GetShows(myCalls.Model.Boxes.setTopBoxes[0].headend, "0", "2", "0", "44", myCalls.Model);
        }

        void myCalls_GetShowImageFinished(object sender, EventArgs e)
        {
            
            Dispatcher.BeginInvoke(() =>
            {
                string finishedEpisodeId = sender.ToString();
                ShowImage currentImage = myCalls.Model.ShowImages.Where(x => x.EpisodeId == finishedEpisodeId).FirstOrDefault();
                //BitmapImage finishedBitmapImage = myCalls.Model.ShowImages.Where(x => x.EpisodeId == finishedEpisodeId).Select(x => x.EpisodeBitmapImage).FirstOrDefault();

                var currentItems = dvrListBox.Items.Cast<ItemViewModel>().Where(x => x.LineTwo == finishedEpisodeId);


                MemoryStream readStream = new MemoryStream(currentImage.EpisodeImageBytes);

                BitmapImage myBitmap = new BitmapImage();
                myBitmap.SetSource(readStream);

                Image myImage = new Image();
                myImage.Source = myBitmap;

               
                foreach (ItemViewModel currentEditItem in currentItems)
                {
                    currentEditItem.Type = myBitmap;
                }

                string firstEpisodeId = dvrListBox.Items.Cast<ItemViewModel>().Where(x => x.Type == null).Select(x => x.LineTwo).FirstOrDefault();

                if (!string.IsNullOrEmpty(firstEpisodeId))
                {
                    myCalls.GetShowImage(firstEpisodeId, 120, 160, myCalls.Model);
                }
            });
        }

        void GetAllShowImages()
        {

            string firstEpisodeId = dvrListBox.Items.Cast<ItemViewModel>().Where(x => x.Type == null).Select(x => x.LineTwo).FirstOrDefault();

            if (!string.IsNullOrEmpty(firstEpisodeId))
            {
                myCalls.GetShowImage(firstEpisodeId, 120, 160, myCalls.Model);
            }
        }

        void myCalls_GetRecordingsFinished(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (myCalls.Model.Recordings != null)
                {

                    foreach (Recording currentRecording in myCalls.Model.Recordings.OrderBy(x => x.description))
                    {

                        ItemViewModel myModel = new ItemViewModel();
                        myModel.ChannelString = currentRecording.title;
                        myModel.LineOne = currentRecording.description;
                        myModel.LineTwo = currentRecording.episodeId;
                        // myModel.LineThree = currentChannel.callSign;
                        myModel.Type = null;
                        //Image blah = new Image();
                        //blah.Source = new BitmapImage(new Uri("https://services.timewarnercable.com/imageserver/guide/YNNAT",UriKind.Absolute));
                        //https://services.timewarnercable.com/imageserver/program/EP015054650016?width=120&height=160

                        //myModel.Type = "https://services.timewarnercable.com/imageserver/program/" + currentRecording.episodeId;// +"?width=50&height=60";


                        dvrListBox.Items.Add(myModel);

                    }
                }

                GetAllShowImages();
            });
        }


        int lastChannelMinTry = 0;
        int lastChannelMaxTry = 0;
        int numChannelShowTry = 0;
        void myCalls_GetShowsFinished(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                //TWCStaticCalls currentCalls = (TWCStaticCalls)sender;

                string[] splitString = sender.ToString().Split(',');



                int startChannel = Convert.ToInt32(splitString[0].ToString());
                int endChannel = Convert.ToInt32(splitString[1].ToString());

                List<string> returnedChannels = new List<string>();

                for (int addChannel = startChannel; addChannel <= endChannel; addChannel++)
                {
     
                    returnedChannels.Add(addChannel.ToString());
                }

                //var allItems = channelListBox.Items.Cast<ItemViewModel>().Where(x=>returnedChannels.Contains(x.ChannelString));
                var allItems = channelListBoxComplete.Cast<ItemViewModel>().Where(x => returnedChannels.Contains(x.ChannelString));

                foreach (ItemViewModel currentItem in allItems)
                {
                    List<Show> currentChannelShows = myCalls.Model.Shows.Where(x => x.channelId == currentItem.ChannelString).ToList<Show>();

                    foreach (Show currentChannelShow in currentChannelShows)
                    {
                        DateTime currentShowStartTime = currentChannelShow.gmDateTime.ToLocalTime();
                        DateTime currentShowEndTime = currentChannelShow.gmDateTime.AddMinutes((long)Convert.ToDouble(currentChannelShow.duration)).ToLocalTime();


                        if (DateTime.Now >= currentShowStartTime && DateTime.Now <= currentShowEndTime)
                        {
                            currentItem.LineOne = currentChannelShow.title;
                            currentItem.LineTwo = currentChannelShow.shortDesc;

                            if (channelListBox.Items.Cast<ItemViewModel>().Where(x => returnedChannels.Contains(x.ChannelString) && x.Channel == currentItem.Channel).Count() > 0)
                            {
                                channelListBox.Items.Cast<ItemViewModel>().Where(x => returnedChannels.Contains(x.ChannelString) && x.Channel == currentItem.Channel).FirstOrDefault().LineOne = currentChannelShow.title;
                                channelListBox.Items.Cast<ItemViewModel>().Where(x => returnedChannels.Contains(x.ChannelString) && x.Channel == currentItem.Channel).FirstOrDefault().LineOne = currentChannelShow.shortDesc;
                            }
                        }
                        
                    }
                }

                int maxChannelShow = myCalls.Model.Shows.Select(x => x.channelIdInt).Max();
                int currentMax = myCalls.Model.Channels.IndexOf(myCalls.Model.Channels.Where(x=>x.channelIdInt == maxChannelShow).FirstOrDefault())+1;
                int channelMaxNotToExceed = myCalls.Model.Channels.Count;
                int submitMaxChannel = -1;
                int maxChannelsInShows = myCalls.Model.Shows.Select(x => x.channelIdInt).Distinct().Count();

                if (currentMax + 44 <= channelMaxNotToExceed)
                {
                    submitMaxChannel = currentMax + 44;
                }
                else
                {
                    submitMaxChannel = channelMaxNotToExceed;
                }

                if (currentMax != channelMaxNotToExceed)
                {
                    if (lastChannelMaxTry == submitMaxChannel && lastChannelMinTry == currentMax)
                    {
                        numChannelShowTry++;
                    }
                    else
                    {
                        numChannelShowTry = 0;
                    }

                    if (numChannelShowTry < 3)
                    {
                        myCalls.GetShows(myCalls.Model.Boxes.setTopBoxes[0].headend, "0", "2", currentMax.ToString(), submitMaxChannel.ToString(), myCalls.Model);
                    }

                    lastChannelMinTry = currentMax;
                    lastChannelMaxTry = submitMaxChannel;
                }
                else
                {
                    string hold = "";
                }
            });

        }




        void myCalls_GetGuideFinished(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {


                alertTextBox.Visibility = System.Windows.Visibility.Collapsed;
                applicationPivot.Visibility = System.Windows.Visibility.Visible;

                foreach (Channel currentChannel in myCalls.Model.Channels.OrderBy(x => x.channelId))
                {

                    ItemViewModel myModel = new ItemViewModel();
                    myModel.ChannelString = currentChannel.channelId.ToString();
                    myModel.LineOne = currentChannel.callSign;
                    myModel.LineTwo = currentChannel.networkName;
                    // myModel.LineThree = currentChannel.callSign;
                    myModel.IsHd = currentChannel.hd;

                    //Image blah = new Image();
                    //blah.Source = new BitmapImage(new Uri("https://services.timewarnercable.com/imageserver/guide/YNNAT",UriKind.Absolute));

                    myModel.Type = currentChannel.logoUrl;
                   
                    channelListBoxComplete.Add(myModel);

                }

                if (displayOnlyHDChannels)
                {

                    channelListBox.Items.Clear();

                    foreach (ItemViewModel currentItem in channelListBoxComplete.Cast<ItemViewModel>().Where(x=>x.IsHd == true))
                    {
                        channelListBox.Items.Add(currentItem);
                    }
                }
                else
                {
                    channelListBox.Items.Clear();

                    foreach (ItemViewModel currentItem in channelListBoxComplete.Cast<ItemViewModel>())
                    {
                        channelListBox.Items.Add(currentItem);
                    }
                }

                channelListBox.UpdateLayout();
            });
            //ImageSource blahblah = new ImageSource();

            streamingHelper.SetParameters(myCalls.Parameters);
            streamingHelper.GetStreamingChannels(myCalls.Model);

            string hold = "";
        }

        void myCalls_GetDevicesFinished(object sender, EventArgs e)
        {


            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                string selectedBoxMac = "";

                try
                {
                    selectedBoxMac = IsolatedStorageSettings.ApplicationSettings["selectedBoxMac"].ToString();
                }
                catch (Exception) { }
                
                /*
                TextBlock myBlock = new TextBlock();
                myBlock.Text = "Devices:";
                myBlock.Height = 72;
                myBlock.FontSize = 50;
               
                myBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                myBlock.Margin = new Thickness(12, 0, 0, 0);
                myBlock.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                devicesGrid.Children.Add(myBlock);
                */

                int fromTop = 60;

                foreach (SetTopBox currentBox in myCalls.Model.Boxes.setTopBoxes)
                {
                    RadioButton deviceButton = new RadioButton();

                    string contentString = currentBox.name;

                    if (currentBox.isDvr)
                    {
                        contentString = contentString + " (DVR)";
                    }

                    deviceButton.Content = contentString;
                    deviceButton.Name = currentBox.macAddress;
                    deviceButton.Checked += new RoutedEventHandler(deviceButton_checkChange);
                    //deviceButton.Height = 72;
                    deviceButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    //deviceButton.Margin = new Thickness(12, fromTop, 0, 0);
                    deviceButton.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                    /*
                    if (fromTop == 0)
                    {
                        deviceButton.IsChecked = true;
                    }
                    else
                    {
                        deviceButton.IsChecked = false;
                    }
                    */

                    devicesListBox.Items.Add(deviceButton);

                    //                                <RadioButton Content="Bedroom" Height="72" HorizontalAlignment="Left" Margin="12,70,0,0" Name="bedRoomRadioButton" VerticalAlignment="Top" IsChecked="True" />
                    //          <RadioButton Content="LivingRoom" Height="72" HorizontalAlignment="Left" Margin="12,130,0,0" Name="livingRoomRadioButton" VerticalAlignment="Top" />
                                   
                }


                if (!string.IsNullOrEmpty(selectedBoxMac) && devicesListBox.Items.Cast<RadioButton>().Where(x => x.Name == selectedBoxMac).Count() > 0)
                {
                    RadioButton currentBox = devicesListBox.Items.Cast<RadioButton>().Where(x => x.Name == selectedBoxMac).FirstOrDefault();
                    currentBox.IsChecked = true;
                }
                else
                {
                    string setMacAddress = "";

                    if (myCalls.Model.Boxes.setTopBoxes.Where(x => x.isDvr == true).Count() > 0)
                    {
                        setMacAddress = myCalls.Model.Boxes.setTopBoxes.Where(x => x.isDvr == true).Select(x => x.macAddress).FirstOrDefault();
                    }
                    else
                    {
                        setMacAddress = myCalls.Model.Boxes.setTopBoxes.Select(x => x.macAddress).FirstOrDefault();
                    }

                    if (string.IsNullOrEmpty(selectedBoxMac))
                    {
                        IsolatedStorageSettings.ApplicationSettings.Add("selectedBoxMac", setMacAddress);
                    }
                    else
                    {
                        IsolatedStorageSettings.ApplicationSettings["selectedBoxMac"] = setMacAddress;
                    }

                    IsolatedStorageSettings.ApplicationSettings.Save();

                    RadioButton currentBox = devicesListBox.Items.Cast<RadioButton>().Where(x => x.Name == setMacAddress).FirstOrDefault();
                    currentBox.IsChecked = true;

                }



            });
            string hold = "";
            
            Dispatcher.BeginInvoke(() =>
            {
                alertTextBox.Text = "Getting guide for default device...";
            });

            myCalls.GetGuide(myCalls.Model.Boxes.setTopBoxes[0].headend, myCalls.Model);
        }

        void deviceButton_checkChange(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    RadioButton checkedButton = (RadioButton)sender;
                    string deviceMacAddress = checkedButton.Name;

                    IsolatedStorageSettings.ApplicationSettings["selectedBoxMac"] = deviceMacAddress;
                    
                    IsolatedStorageSettings.ApplicationSettings.Save();

                    bool isDvr = Convert.ToBoolean(myCalls.Model.Boxes.setTopBoxes.Where(x => x.macAddress == deviceMacAddress).Select(x => x.isDvr).FirstOrDefault());

                    if (isDvr)
                    {
                        dvrPivotItem.IsEnabled = true;
                    }
                    else
                    {
                        dvrPivotItem.IsEnabled = false;
                    }
                });
            string hold = "";
        }

        void myCalls_ChangeChannelFinished(object sender, EventArgs e)
        {
            string hold = "";
        }

        void myCalls_LoginFinished(object sender, EventArgs e)
        {
            //myCalls = (TWCStaticCalls)sender;
            bool loggedIn = (bool)sender;

            if (loggedIn)
            {
                Dispatcher.BeginInvoke(() =>
                    {
                        if (loggedIn)
                        {
                            isLoggedIn = true;
                            passwordTextBox.Password = password;
                            userNameTextBlock.Text = userName;
                            rememberPasswordCheckBox.IsChecked = rememberPassword;

                            applicationPivot.SelectedItem = channelPivotItem;
                            alertTextBox.Visibility = System.Windows.Visibility.Visible;
                            loginGridStatic.Visibility = System.Windows.Visibility.Collapsed;
                            alertTextBox.Text = "Getting Devices...";
                            loginPasswordIncorrect.Visibility = System.Windows.Visibility.Collapsed;
                            loginPasswordIncorrectStatic.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            if (loginGridStatic.Visibility == System.Windows.Visibility.Visible)
                            {
                                loginPasswordIncorrectStatic.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                loginPasswordIncorrect.Visibility = System.Windows.Visibility.Visible;
                            }
                        }
                    });

                myCalls.GetDevices(myCalls.Model);
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                    {
                        isLoggedIn = false;
                        applicationPivot.SelectedItem = loginPivotItem;
                        MessageBox.Show("Login incorrect, please try again.");
                    });
            }

        }

        


        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(userNameTextBlock.Text) || string.IsNullOrEmpty(passwordTextBox.Password))
            {
                applicationPivot.SelectedItem = loginPivotItem;
            }
        }

        private void channelListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (channelListBox.SelectedIndex == -1)
            {
                return;
            }
            else
            {
                string macAddress = "";

                macAddress = devicesListBox.Items.Cast<RadioButton>().Where(x => x.IsChecked == true).Select(x => x.Name).FirstOrDefault();
               
                if (!string.IsNullOrEmpty(macAddress))
                {
                    ItemViewModel currentChannel = (ItemViewModel)e.AddedItems[0];

                    if (!string.IsNullOrEmpty(currentChannel.ChannelString))
                    {
                        myCalls.ChangeChannel(currentChannel.ChannelString, macAddress, myCalls.Model);
                        // Navigate to the new page
                        //NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + channelListBox.SelectedIndex, UriKind.Relative));
                    }
                }
            }
            // Reset selected index to -1 (no selection)
            channelListBox.SelectedIndex = -1;
        }


        void video_CurrentStateChanged(object sender, RoutedEventArgs e)
        {


            
            string hold = "";
        }


        void streamingHelper_GetStreamFinished(object sender, EventArgs e)
        {

            Dispatcher.BeginInvoke(() =>
    {
        
        ChannelStreamInfo streamInfo = (ChannelStreamInfo)streamingHelper.parameters["streamChannelInfo"];
        Uri myUri = new Uri(streamInfo.stream_url);

        string deviceIdHeaderString = HttpUtility.UrlEncode(myModel.DeviceId);
        string headerString = HttpUtility.UrlEncode(myModel.HeaderOptionsJson);
        string clientDetailsString = HttpUtility.UrlEncode(myModel.ClientDetailsJson);
        string wayFarer = HttpUtility.UrlEncode(myModel.Wayfarer);
        string vs_guid = HttpUtility.UrlEncode(myModel.Vs_Guid);
        string vs_guid_ns = HttpUtility.UrlEncode(myModel.Vs_Guid);
        string vxtoken = streamingHelper.parameters["vxtoken"].ToString();

        CookieContainer myContainer = new CookieContainer();
        myContainer.Add(myUri, new Cookie("device_id",myCalls.Model.DeviceId));
        myContainer.Add(myUri, new Cookie("header", HttpUtility.UrlEncode(myCalls.Model.HeaderOptionsJson)));
        myContainer.Add(myUri, new Cookie("clientDetails", HttpUtility.UrlEncode(myCalls.Model.ClientDetailsJson)));
        myContainer.Add(myUri, new Cookie("Wayfarer", myCalls.Model.Wayfarer));
        myContainer.Add(myUri, new Cookie("Wayfarer_ns", myCalls.Model.Wayfarer));
        myContainer.Add(myUri, new Cookie("vs_guid", myCalls.Model.Vs_Guid));
        myContainer.Add(myUri, new Cookie("vs_guid_ns", myCalls.Model.Vs_Guid));
        myContainer.Add(myUri, new Cookie("vx_token", vxtoken));
        video.CookieContainer = myContainer;
        //request.Headers["Cookie"] = "device_id=" + deviceIdHeaderString + "; header=" + headerString + "; clientDetails=" + clientDetailsString + "; Wayfarer=" + wayFarer + "; vs_guid=" + vs_guid + "; vs_guid_ns=" + vs_guid_ns + "; vxtoken=" + vxtoken + ";";

        
        video.SmoothStreamingSource = new Uri(streamInfo.stream_url);
        //video.SetSource(streamingHelper.StreamResponse.GetResponseStream());
    });
        }


        void video_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            video.Play();
            string hold = "";
        }

        private void applicationPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot sendingObject = (Pivot)sender;

            if (isLoggedIn)
            {
                if (((PivotItem)sendingObject.SelectedItem).Header.ToString().ToUpper() == "STREAMING")
                {
                    //string videoLink = "http://ss-lin-20.timewarnercable.com/ANE_HD/index.isml/Manifest?sessionid=b5f20d474a2fe5";

                    video.CurrentStateChanged += new RoutedEventHandler(video_CurrentStateChanged);

                    video.AutoPlay = true;

                    video.SmoothStreamingPlaybackMode = Microsoft.Web.Media.SmoothStreaming.PlaybackMode.AudioVideo;
                    //video.SmoothStreamingSource = new Uri(videoLink);
                    video.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(video_MediaFailed);
                    Channel currentStreamChannel = myModel.StreamingChannels.FirstOrDefault();
                    Classes.Stream currentStreamChannelStream = currentStreamChannel.streams.Where(x => x.type.ToUpper() == "SMOOTH").FirstOrDefault();

                    streamingHelper.GetStreamFinished += new EventHandler(streamingHelper_GetStreamFinished);

                    streamingHelper.GetStream(currentStreamChannelStream.uri, myCalls.Model);

                    string hold = "";
                    //streamingHelper.GetStreamFinished += new EventHandler(myHelper_GetStreamFinished);
                    //streamingHelper.GetStream(videoLink, myCalls.Model);
                    //streamingHelper.GetToken(myCalls.Model);
                    //streamingHelper.GetStreamingChannels(myCalls.Model);
                }



                if (((PivotItem)sendingObject.SelectedItem).Header.ToString().ToUpper() == "DVR")
                {
                    dvrListBox.Items.Clear();

                    if (dvrPivotItem.IsEnabled)
                    {
                        myCalls.GetRecordings(myCalls.Model.Boxes.setTopBoxes[0].macAddress, myCalls.Model);
                    }
                    else
                    {
                        TextBlock myBlock = new TextBlock();
                        myBlock.Text = "DVR not supported on selected device.";
                        dvrListBox.Items.Add(myBlock);
                    }
                }
            }
            else
            {

                if (applicationPivot.SelectedItem != loginPivotItem)
                {
                    applicationPivot.SelectedItem = loginPivotItem;
                }

            }
        }



        private void loginButtonStatic_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(userNameTextBlockStatic.Text) && !string.IsNullOrEmpty(passwordTextBoxStatic.Password))
            {

                try
                {
                    IsolatedStorageSettings.ApplicationSettings["userName"] = userNameTextBlockStatic.Text;
                }
                catch (Exception) { }

                try
                {
                    if (rememberPasswordCheckBoxStatic.IsChecked.Value)
                    {
                        IsolatedStorageSettings.ApplicationSettings["password"] = passwordTextBoxStatic.Password;
                    }
                    else
                    {
                        try
                        {
                            IsolatedStorageSettings.ApplicationSettings.Remove("password");
                        }
                        catch (Exception) { }
                    }
                }
                catch (Exception) { }

                try
                {
                    IsolatedStorageSettings.ApplicationSettings["rememberPassword"] = rememberPasswordCheckBoxStatic.IsChecked.Value;
                }
                catch (Exception) { }

                IsolatedStorageSettings.ApplicationSettings.Save();

                myCalls.LoginGetWayfarer(userNameTextBlockStatic.Text, passwordTextBoxStatic.Password, myModel);
            }
        }

        private void rememberPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void rememberPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            rememberPassword = false;
            rememberPasswordCheckBox.IsChecked = false;

            try
            {
                IsolatedStorageSettings.ApplicationSettings["rememberPassword"] = false;
            }
            catch (Exception) { }

            try
            {
                IsolatedStorageSettings.ApplicationSettings.Remove("password");
            }
            catch (Exception) { }

            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    displayOnlyHDChannels = false;
                    IsolatedStorageSettings.ApplicationSettings["displayOnlyHDChannels"] = false;
                    IsolatedStorageSettings.ApplicationSettings.Save();

                    var allItems = channelListBoxComplete.Cast<ItemViewModel>();

                    channelListBox.Items.Clear();

                    foreach (ItemViewModel currentItem in allItems)
                    {
                        channelListBox.Items.Add(currentItem);
                    }
                    
                    channelListBox.UpdateLayout();

                });

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
    {
        displayOnlyHDChannels = true;
        IsolatedStorageSettings.ApplicationSettings["displayOnlyHDChannels"] = true;
        IsolatedStorageSettings.ApplicationSettings.Save();

        var allItems = channelListBoxComplete.Cast<ItemViewModel>().Where(x=>x.IsHd == true);

        foreach (ItemViewModel currentItem in allItems)
        {
            channelListBox.Items.Add(currentItem);
        }

        channelListBox.UpdateLayout();
    });
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            {
                if (!string.IsNullOrEmpty(userNameTextBlock.Text) && !string.IsNullOrEmpty(passwordTextBox.Password))
                {

                    try
                    {
                        IsolatedStorageSettings.ApplicationSettings["userName"] = userNameTextBlock.Text;
                        userName = userNameTextBlock.Text;
                    }
                    catch (Exception) { }

                    try
                    {
                        if (rememberPasswordCheckBox.IsChecked.Value)
                        {
                            IsolatedStorageSettings.ApplicationSettings["password"] = passwordTextBox.Password;
                            password = passwordTextBox.Password;
                        }
                        else
                        {
                            try
                            {
                                IsolatedStorageSettings.ApplicationSettings.Remove("password");
                            }
                            catch (Exception) { }
                        }
                    }
                    catch (Exception) { }

                    try
                    {
                        IsolatedStorageSettings.ApplicationSettings["rememberPassword"] = rememberPasswordCheckBox.IsChecked.Value;
                        rememberPassword = rememberPasswordCheckBox.IsChecked.Value;
                    }
                    catch (Exception) { }

                    IsolatedStorageSettings.ApplicationSettings.Save();
                    
                    myCalls.LoginGetWayfarer(userNameTextBlock.Text, passwordTextBox.Password, myModel);
                }
            }
        }



    }
}