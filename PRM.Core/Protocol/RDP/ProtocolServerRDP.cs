﻿using System;
using Newtonsoft.Json;
using PRM.Core.Model;
using RdpHelper;

namespace PRM.Core.Protocol.RDP
{
    public enum ERdpWindowResizeMode
    {
        AutoResize = 0,
        Stretch = 1,
        Fixed = 2,
        StretchFullScreen = 3,
        FixedFullScreen = 4,
    }
    public enum ERdpFullScreenFlag
    {
        Disable = 0,
        EnableFullScreen = 1,
        EnableFullAllScreens = 2,
    }

    public enum EDisplayPerformance
    {
        /// <summary>
        /// Auto judge(by connection speed)
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Low(8bit color with no feature support)
        /// </summary>
        Low = 1,
        /// <summary>
        /// Mdiddle(16bit color with only font smoothing and desktop composition)
        /// </summary>
        Middle = 2,
        /// <summary>
        /// High(32bit color with full features support)
        /// </summary>
        High = 3,
    }
    
    public enum EGatewayMode
    {
        AutomaticallyDetectGatewayServerSettings = 0,
        UseTheseGatewayServerSettings = 1,
        DoNotUseGateway = 2,
    }
    public enum EGatewayLogonMethod
    {
        Password = 0,
        SmartCard = 1,
    }

    public sealed class ProtocolServerRDP : ProtocolServerWithAddrPortUserPwdBase
    {
        public class LocalSetting : NotifyPropertyChangedBase
        {
            private bool _fullScreenLastSessionIsFullScreen = false;
            public bool FullScreenLastSessionIsFullScreen
            {
                get => _fullScreenLastSessionIsFullScreen;
                set => SetAndNotifyIfChanged(nameof(FullScreenLastSessionIsFullScreen), ref _fullScreenLastSessionIsFullScreen, value);
            }

            private int _fullScreenLastSessionScreenIndex = -1;
            public int FullScreenLastSessionScreenIndex
            {
                get => _fullScreenLastSessionScreenIndex;
                set => SetAndNotifyIfChanged(nameof(FullScreenLastSessionScreenIndex), ref _fullScreenLastSessionScreenIndex, value);
            }
        }

        public ProtocolServerRDP() : base("RDP", "RDP.V1", "RDP")
        {
            base.Port = "3389";
            base.UserName = "Administrator";
        }


        #region Display

        private ERdpFullScreenFlag _rdpFullScreenFlag = ERdpFullScreenFlag.EnableFullScreen;
        public ERdpFullScreenFlag RdpFullScreenFlag
        {
            get => _rdpFullScreenFlag;
            set
            {
                SetAndNotifyIfChanged(nameof(RdpFullScreenFlag), ref _rdpFullScreenFlag, value);
                switch (value)
                {
                    case ERdpFullScreenFlag.EnableFullAllScreens:
                        IsConnWithFullScreen = true;
                        break;
                    case ERdpFullScreenFlag.EnableFullScreen:
                        break;
                    case ERdpFullScreenFlag.Disable:
                        IsConnWithFullScreen = false;
                        if(RdpWindowResizeMode == ERdpWindowResizeMode.FixedFullScreen)
                            RdpWindowResizeMode = ERdpWindowResizeMode.Fixed;
                        if(RdpWindowResizeMode == ERdpWindowResizeMode.StretchFullScreen)
                            RdpWindowResizeMode = ERdpWindowResizeMode.Stretch;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }


        private bool _isConnWithFullScreen = false;
        public bool IsConnWithFullScreen
        {
            get => _isConnWithFullScreen;
            set => SetAndNotifyIfChanged(nameof(IsConnWithFullScreen), ref _isConnWithFullScreen, value);
        }

        
        private bool _isFullScreenWithConnectionBar = true;
        public bool IsFullScreenWithConnectionBar
        {
            get => _isFullScreenWithConnectionBar;
            set => SetAndNotifyIfChanged(nameof(IsFullScreenWithConnectionBar), ref _isFullScreenWithConnectionBar, value);
        }

        private ERdpWindowResizeMode _rdpWindowResizeMode = ERdpWindowResizeMode.AutoResize;
        public ERdpWindowResizeMode RdpWindowResizeMode
        {
            get => _rdpWindowResizeMode;
            set
            {
                var tmp = value;
                if (RdpFullScreenFlag == ERdpFullScreenFlag.Disable)
                {
                    if (tmp == ERdpWindowResizeMode.FixedFullScreen)
                        tmp = ERdpWindowResizeMode.Fixed;
                    if (tmp == ERdpWindowResizeMode.StretchFullScreen)
                        tmp = ERdpWindowResizeMode.Stretch;
                }
                _rdpWindowResizeMode = tmp;
                RaisePropertyChanged(nameof(RdpWindowResizeMode));
            }
        }


        private int _rdpWidth = 800;
        public int RdpWidth
        {
            get => _rdpWidth;
            set => SetAndNotifyIfChanged(nameof(RdpWidth), ref _rdpWidth, value);
        }


        private int _rdpHeight = 600;
        public int RdpHeight
        {
            get => _rdpHeight;
            set => SetAndNotifyIfChanged(nameof(RdpHeight), ref _rdpHeight, value);
        }



        private EDisplayPerformance _displayPerformance = EDisplayPerformance.Auto;
        public EDisplayPerformance DisplayPerformance
        {
            get => _displayPerformance;
            set => SetAndNotifyIfChanged(nameof(DisplayPerformance), ref _displayPerformance, value);
        }


        #endregion


        #region resource switch

        private bool _enableClipboard = true;
        public bool EnableClipboard
        {
            get => _enableClipboard;
            set
            {
                if (!value && _enableSounds)
                {
                    SetAndNotifyIfChanged(nameof(EnableSounds), ref _enableSounds, false);
                }
                SetAndNotifyIfChanged(nameof(EnableClipboard), ref _enableClipboard, value);
            }
        }


        private bool _enableDiskDrives = true;
        public bool EnableDiskDrives
        {
            get => _enableDiskDrives;
            set => SetAndNotifyIfChanged(nameof(EnableDiskDrives), ref _enableDiskDrives, value);
        }



        private bool _enableKeyCombinations = true;
        public bool EnableKeyCombinations
        {
            get => _enableKeyCombinations;
            set => SetAndNotifyIfChanged(nameof(EnableKeyCombinations), ref _enableKeyCombinations, value);
        }


        private bool _enableSounds = true;
        public bool EnableSounds
        {
            get => _enableSounds;
            set
            {
                if (value && !_enableClipboard)
                {
                    SetAndNotifyIfChanged(nameof(EnableClipboard), ref _enableClipboard, true);
                }
                SetAndNotifyIfChanged(nameof(EnableSounds), ref _enableSounds, value);
            }
        }


        private bool _enableAudioCapture = false;
        public bool EnableAudioCapture
        {
            get => _enableAudioCapture;
            set => SetAndNotifyIfChanged(nameof(EnableAudioCapture), ref _enableAudioCapture, value);
        }





        private bool _enablePorts = false;
        public bool EnablePorts
        {
            get => _enablePorts;
            set => SetAndNotifyIfChanged(nameof(EnablePorts), ref _enablePorts, value);
        }




        private bool _enablePrinters = false;
        public bool EnablePrinters
        {
            get => _enablePrinters;
            set => SetAndNotifyIfChanged(nameof(EnablePrinters), ref _enablePrinters, value);
        }




        private bool _enableSmartCardsAndWinHello = false;

        public bool EnableSmartCardsAndWinHello
        {
            get => _enableSmartCardsAndWinHello;
            set => SetAndNotifyIfChanged(nameof(EnableSmartCardsAndWinHello), ref _enableSmartCardsAndWinHello, value);
        }

        #endregion



        #region Gateway
        private EGatewayMode _gatewayMode = EGatewayMode.AutomaticallyDetectGatewayServerSettings;
        public EGatewayMode GatewayMode
        {
            get => _gatewayMode;
            set => SetAndNotifyIfChanged(nameof(GatewayMode), ref _gatewayMode, value);
        }

        
        private bool _gatewayBypassForLocalAddress = true;
        public bool GatewayBypassForLocalAddress
        {
            get => _gatewayBypassForLocalAddress;
            set => SetAndNotifyIfChanged(nameof(GatewayBypassForLocalAddress), ref _gatewayBypassForLocalAddress, value);
        }
        
        private string _gatewayHostName = "";
        public string GatewayHostName
        {
            get => _gatewayHostName;
            set => SetAndNotifyIfChanged(nameof(GatewayHostName), ref _gatewayHostName, value);
        }

        
        private EGatewayLogonMethod _gatewayLogonMethod = EGatewayLogonMethod.Password;
        public EGatewayLogonMethod GatewayLogonMethod
        {
            get => _gatewayLogonMethod;
            set => SetAndNotifyIfChanged(nameof(GatewayLogonMethod), ref _gatewayLogonMethod, value);
        }

        
        private string _gatewayUserName = "";
        public string GatewayUserName
        {
            get => _gatewayUserName;
            set => SetAndNotifyIfChanged(nameof(GatewayUserName), ref _gatewayUserName, value);
        }

        private string _gatewayPassword = "";
        public string GatewayPassword
        {
            get => _gatewayPassword;
            set => SetAndNotifyIfChanged(nameof(GatewayPassword), ref _gatewayPassword, value);
        }

        public string GetDecryptedGatewayPassword()
        {
            if (SystemConfig.Instance.DataSecurity.Rsa != null)
            {
                return SystemConfig.Instance.DataSecurity.Rsa.DecodeOrNull(_gatewayPassword) ?? "";
            }
            return _gatewayPassword;
        }
        #endregion

        private LocalSetting _autoSetting = new LocalSetting();
        public LocalSetting AutoSetting
        {
            get => _autoSetting;
            set => SetAndNotifyIfChanged(nameof(AutoSetting), ref _autoSetting, value);
        }


        public override ProtocolServerBase CreateFromJsonString(string jsonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<ProtocolServerRDP>(jsonString);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// To rdp file object
        /// </summary>
        /// <returns></returns>
        public RdpConfig ToRdpConfig()
        {
            var rdpConfig = new RdpConfig($"{this.Address}:{this.Port}", this.UserName, this.GetDecryptedPassWord());
            rdpConfig.AuthenticationLevel = 0;
            rdpConfig.DisplayConnectionBar = this.IsFullScreenWithConnectionBar ? 1 : 0;
            switch (this.RdpFullScreenFlag)
            {
                case ERdpFullScreenFlag.Disable:
                    rdpConfig.ScreenModeId = 1;
                    rdpConfig.DesktopWidth = this.RdpWidth;
                    rdpConfig.DesktopHeight = this.RdpHeight;
                    break;
                case ERdpFullScreenFlag.EnableFullScreen:
                    rdpConfig.ScreenModeId = 2;
                    break;
                case ERdpFullScreenFlag.EnableFullAllScreens:
                    rdpConfig.ScreenModeId = 2;
                    rdpConfig.UseMultimon = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            switch (this.RdpWindowResizeMode)
            {
                case ERdpWindowResizeMode.AutoResize:
                    rdpConfig.SmartSizing = 0;
                    rdpConfig.DynamicResolution = 1;
                    break;
                case ERdpWindowResizeMode.Stretch:
                case ERdpWindowResizeMode.StretchFullScreen:
                    rdpConfig.SmartSizing = 1;
                    rdpConfig.DynamicResolution = 0;
                    break;
                case ERdpWindowResizeMode.Fixed:
                case ERdpWindowResizeMode.FixedFullScreen:
                    rdpConfig.SmartSizing = 0;
                    rdpConfig.DynamicResolution = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            rdpConfig.NetworkAutodetect = 0;
            switch (this.DisplayPerformance)
            {
                case EDisplayPerformance.Auto:
                    rdpConfig.NetworkAutodetect = 1;
                    break;
                case EDisplayPerformance.Low:
                    rdpConfig.ConnectionType = 1;
                    rdpConfig.SessionBpp = 8;
                    rdpConfig.AllowDesktopComposition = 0;
                    rdpConfig.AllowFontSmoothing = 0;
                    rdpConfig.DisableFullWindowDrag = 1;
                    rdpConfig.DisableThemes = 1;
                    rdpConfig.DisableWallpaper = 1;
                    rdpConfig.DisableMenuAnims = 1;
                    rdpConfig.DisableCursorSetting = 1;
                    break;
                case EDisplayPerformance.Middle:
                    rdpConfig.SessionBpp = 16;
                    rdpConfig.ConnectionType = 3;
                    rdpConfig.AllowDesktopComposition = 1;
                    rdpConfig.AllowFontSmoothing = 1;
                    rdpConfig.DisableFullWindowDrag = 1;
                    rdpConfig.DisableThemes = 1;
                    rdpConfig.DisableWallpaper = 1;
                    rdpConfig.DisableMenuAnims = 1;
                    rdpConfig.DisableCursorSetting = 1;
                    break;
                case EDisplayPerformance.High:
                    rdpConfig.SessionBpp = 32;
                    rdpConfig.ConnectionType = 6;
                    rdpConfig.AllowDesktopComposition = 1;
                    rdpConfig.AllowFontSmoothing = 1;
                    rdpConfig.DisableFullWindowDrag = 0;
                    rdpConfig.DisableThemes = 0;
                    rdpConfig.DisableWallpaper = 0;
                    rdpConfig.DisableMenuAnims = 0;
                    rdpConfig.DisableCursorSetting = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            rdpConfig.KeyboardHook = 0;
            rdpConfig.AudioMode = 2;
            rdpConfig.AudioCaptureMode = 0;

            
            if (this.EnableDiskDrives)
                rdpConfig.RedirectDrives = 1;
            if (this.EnableClipboard)
                rdpConfig.RedirectClipboard = 1;
            if (this.EnablePrinters)
                rdpConfig.RedirectPrinters = 1;
            if (this.EnablePorts)
                rdpConfig.RedirectComPorts = 1;
            if (this.EnableSmartCardsAndWinHello)
                rdpConfig.RedirectSmartCards = 1;
            if (this.EnableKeyCombinations)
                rdpConfig.KeyboardHook = 2;
            if (this.EnableSounds)
                rdpConfig.AudioMode = 0;
            if (this.EnableAudioCapture)
                rdpConfig.AudioCaptureMode = 1;

            rdpConfig.AutoReconnectionEnabled = 1;


            return rdpConfig;
        }
    }
}
