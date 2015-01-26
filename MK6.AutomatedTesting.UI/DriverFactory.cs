﻿using MK6.AutomatedTesting.UI.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;

namespace MK6.AutomatedTesting.UI
{
    public static class DriverFactory
    {
        public static IWebDriver Build(DriverConfigurationSection configSection)
        {
            switch (configSection.Browser)
            {
                case "Firefox":
                    return new FirefoxDriver();
                case "Chrome":
                    CopyChromeDriverServerToLocalDirectory(
                        configSection.ServerSource);
                    return new ChromeDriver();
                case "InternetExplorer":
                    CopyIEDriverServerToLocalDirectory(
                        configSection.ServerSource);
                    return new InternetExplorerDriver(
                        Directory.GetCurrentDirectory(),
                        new InternetExplorerOptions
                        {
                            IgnoreZoomLevel = true
                        });
                case "PhantomJS":
                    CopyPhantomJSServerToLocalDirectory(configSection.ServerSource);
                    var browser = new PhantomJSDriver();
                    ApplyPhantomJSHack(browser);
                    return browser;
                case "Remote":
                    return new RemoteWebDriver(
                        new Uri(configSection.RemoteUrl),
                        new DesiredCapabilities(new Dictionary<string, object> 
                        { 
                            { "browser", configSection.RemoteBrowser }, 
                            { "version", configSection.RemoteBrowserVersion } 
                        }));
                default:
                    throw new ApplicationException(
                        string.Format(
                            "Unable to create a driver of type {0}",
                            configSection.Browser));
            }
        }

        private static void ApplyPhantomJSHack(IWebDriver browser)
        {
            browser.Manage().Window.Maximize();
        }

        private static void CopyChromeDriverServerToLocalDirectory(string serverSource)
        {
            CopyFileLocally(
                serverSource,
                "chromedriver.exe");
        }

        private static void CopyIEDriverServerToLocalDirectory(string serverSource)
        {
            CopyFileLocally(
                serverSource,
                "IEDriverServer.exe");
        }

        private static void CopyPhantomJSServerToLocalDirectory(string serverSource)
        {
            CopyFileLocally(
                serverSource,
                "phantomjs.exe");
        }

        private static void CopyFileLocally(
            string sourceDirectory,
            string filename)
        {
            MoveFileLocallyIfNeeded(
                Path.Combine(sourceDirectory, filename),
                Path.Combine(Directory.GetCurrentDirectory(), filename));
        }

        private static void MoveFileLocallyIfNeeded(
            string sourceFile,
            string destinationFile)
        {
            if (!File.Exists(destinationFile))
            {
                File.Copy(sourceFile, destinationFile);
            }
        }
    }
}
