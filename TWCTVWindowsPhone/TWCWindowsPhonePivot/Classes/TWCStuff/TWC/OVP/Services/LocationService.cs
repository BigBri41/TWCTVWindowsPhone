namespace TWC.OVP.Services
{
    using Caliburn.Micro;
    using System;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Browser;
    using TWC.OVP.Framework.Extensions;
    using TWC.OVP.Messages;
    using TWC.OVP.Models;

    public class LocationService : PropertyChangedBase
    {
        private IEventAggregator _eventAggregator;
        private bool? _IsOutOfCountry = null;
        private bool? _isOutOfHome = null;

        public LocationService(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
        }

        public IResult CheckLocation(System.Action<bool, bool, bool> callback = null)
        {
            return LiveTVService.CheckLocation(delegate (Location result) {
                bool flag = false;
                if (!this.IsOutOfHome.HasValue || (result.behindOwnModem != this.IsOutOfHome))
                {
                    if (!this.IsOutOfCountry.HasValue)
                    {
                        goto Label_0084;
                    }
                    bool? isOutOfCountry = this.IsOutOfCountry;
                    bool inUS = result.inUS;
                    if (!((isOutOfCountry.GetValueOrDefault() == inUS) && isOutOfCountry.HasValue))
                    {
                        goto Label_0084;
                    }
                }
                flag = true;
            Label_0084:
                this.IsOutOfHome = new bool?(!result.behindOwnModem);
                this.IsOutOfCountry = new bool?(!result.inUS);
                if (flag)
                {
                    this._eventAggregator.Publish(new OutOfHomeStatusChangedMessage(this.IsOutOfHome.Value, this.IsOutOfCountry.Value));
                }
                if (callback != null)
                {
                    callback(this.IsOutOfHome.Value, this.IsOutOfCountry.Value, flag);
                }
            }, delegate (Exception ex) {
                this._eventAggregator.Publish(new ErrorMessage(MessageText.TitleDefault, MessageText.VideoTemporarilyUnavailable, ex.ToString(), ErrorMessageType.PlayerbackError, 0x3e8, null, true));
            });
        }

        public void UpdateInitialLocationFromCookie()
        {
            Cookie cookie = CookieHelper.GetCookies()["loc"];
            if (cookie != null)
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(HttpUtility.UrlDecode(cookie.Value));
                    Location location = Encoding.UTF8.GetString(bytes, 0, bytes.Length).DeserializeFromJson<Location>();
                    this.IsOutOfHome = new bool?(!location.behindOwnModem);
                    this.IsOutOfCountry = new bool?(!location.inUS);
                }
                catch
                {
                }
            }
        }

        public bool? IsOutOfCountry
        {
            get
            {
                return this._IsOutOfCountry;
            }
            set
            {
                this._IsOutOfCountry = value;
            }
        }

        public bool? IsOutOfHome
        {
            get
            {
                return this._isOutOfHome;
            }
            set
            {
                this._isOutOfHome = value;
                this.NotifyOfPropertyChange<bool?>(Expression.Lambda<System.Func<bool?>>(Expression.Property(Expression.Constant(this, typeof(LocationService)), (MethodInfo) methodof(LocationService.get_IsOutOfHome)), new ParameterExpression[0]));
            }
        }
    }
}

