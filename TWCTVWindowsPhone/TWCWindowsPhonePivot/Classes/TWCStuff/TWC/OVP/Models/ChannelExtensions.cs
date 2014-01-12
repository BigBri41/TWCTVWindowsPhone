namespace TWC.OVP.Models
{
    using Caliburn.Micro;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using TWC.OVP.ViewModels;

    public static class ChannelExtensions
    {
        private static IEnumerable<Channel> Deduplication(ChannelList channelList)
        {
            return (from g in (from c in channelList.channels
                group c by c.tmsId into g
                select g).ToArray<IGrouping<string, Channel>>() select g.First<Channel>());
        }

        public static IList<ChannelViewModel> ToObservable(this ChannelList channelList)
        {
            BindableCollection<ChannelViewModel> bindables = new BindableCollection<ChannelViewModel>();
            int num = 0;
            IEnumerable<Channel> enumerable = Deduplication(channelList);
            IOrderedEnumerable<Channel> first = from c in enumerable
                where !c.networkName.StartsWith("~")
                orderby c.networkName
                select c;
            IOrderedEnumerable<Channel> second = from c in enumerable
                where c.networkName.StartsWith("~")
                orderby c.networkName
                select c;
            foreach (Channel channel in first.Union<Channel>(second))
            {
                bindables.Add(new ChannelViewModel(channel, num++));
            }
            return bindables;
        }
    }
}

