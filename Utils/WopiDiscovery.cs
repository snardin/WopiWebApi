using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using WopiWebApi.Models;
using WopiWebApi.Utils;

namespace WopiWebApi.Utils
{
    public class WopiDiscovery
    {
        private readonly IOptions<WopiConfig> _appSettings;

        private DateTime? _dateLoad;

        private XDocument _itemXml;

        public WopiDiscovery(IOptions<WopiConfig> app)
        {
            _appSettings = app;
            _dateLoad = null;
        }

        //Method to read the WopiDiscovery xml
        public void ReadXmlFromUrl()
        {
            DateTime today = DateTime.Now;

            //Check if the xml has been read in the last 24h
            if (_dateLoad == null || (today - _dateLoad.Value).TotalHours > 24)
            {
                var httpClient = new HttpClient();
                var result = httpClient.GetAsync(_appSettings.Value.DiscoveryURL).Result;
                var xmlString = result.Content.ReadAsStringAsync().Result;

                _itemXml = XDocument.Parse(xmlString);
                _dateLoad = today;
            }
        }

        //Method to retrieve the defined actions
        public List<WopiAppModel> GetXmlActions(string hostPath)
        {
            List<WopiAppModel> output = new List<WopiAppModel>();

            ReadXmlFromUrl();

            if (_itemXml != null)
            {
                var xapps = _itemXml.Descendants("app");
                foreach (var xapp in xapps)
                {
                    // Parse the actions for the app
                    output.Add(new WopiAppModel()
                    {
                        app = xapp.Attribute("name").Value,
                        favIconUrl = xapp.Attribute("favIconUrl").Value,
                        checkLicense = Convert.ToBoolean(xapp.Attribute("checkLicense").Value),
                        actions = xapp.Elements("action").Select(x => new WopiActionModel()
                        {
                            name = x.Attribute("name").Value,
                            ext = (x.Attribute("ext") != null) ? x.Attribute("ext").Value : String.Empty,
                            progid = (x.Attribute("progid") != null) ? x.Attribute("progid").Value : String.Empty,
                            isDefault = (x.Attribute("default") != null) ? true : false,
                            urlsrc = WopiUtils.GetActionUrl(hostPath, x.Attribute("urlsrc").Value),
                            requires = (x.Attribute("requires") != null) ? x.Attribute("requires").Value : String.Empty
                        }).ToList()
                    });
                }
            }

            return output;
        }

        //Method to retrieve the defined actions for a specific extension
        public WopiAppModel GetXmlActions(string hostPath, string actionExt)
        {
            WopiAppModel output = new WopiAppModel();

            ReadXmlFromUrl();

            if (_itemXml != null)
            {                
                IEnumerable<XElement> actions =
                    (from el in _itemXml.Descendants("action")
                    where (string)el.Attribute("ext") == actionExt
                    select el);

                XElement xapp = actions?.FirstOrDefault()?.Parent;
                if (xapp != null)
                {
                    // Parse the actions for the app
                    output = new WopiAppModel()
                    {
                        app = xapp.Attribute("name").Value,
                        favIconUrl = xapp.Attribute("favIconUrl").Value,
                        checkLicense = Convert.ToBoolean(xapp.Attribute("checkLicense").Value),
                        actions = actions.Select(x => new WopiActionModel()
                        {
                            name = x.Attribute("name").Value,
                            ext = (x.Attribute("ext") != null) ? x.Attribute("ext").Value : String.Empty,
                            progid = (x.Attribute("progid") != null) ? x.Attribute("progid").Value : String.Empty,
                            isDefault = (x.Attribute("default") != null) ? true : false,
                            urlsrc = WopiUtils.GetActionUrl(hostPath, x.Attribute("urlsrc").Value),
                            requires = (x.Attribute("requires") != null) ? x.Attribute("requires").Value : String.Empty
                        }).ToList()
                    };
                }
            }

            return output;
        }

        //Method to retrieve a specific action for a specific extension
        public WopiAppModel GetXmlAction(string hostPath, string actionExt, string actionName)
        {
            WopiAppModel output = new WopiAppModel();

            ReadXmlFromUrl();

            if (_itemXml != null)
            {
                IEnumerable<XElement> actions =
                    (from el in _itemXml.Descendants("action")
                     where (string)el.Attribute("ext") == actionExt
                     && (string)el.Attribute("name") == actionName
                     select el);

                XElement xapp = actions?.FirstOrDefault()?.Parent;
                if (xapp != null)
                {
                    // Parse the actions for the app
                    output = new WopiAppModel()
                    {
                        app = xapp.Attribute("name").Value,
                        favIconUrl = xapp.Attribute("favIconUrl").Value,
                        checkLicense = Convert.ToBoolean(xapp.Attribute("checkLicense").Value),
                        actions = actions.Select(x => new WopiActionModel()
                        {
                            name = x.Attribute("name").Value,
                            ext = (x.Attribute("ext") != null) ? x.Attribute("ext").Value : String.Empty,
                            progid = (x.Attribute("progid") != null) ? x.Attribute("progid").Value : String.Empty,
                            isDefault = (x.Attribute("default") != null) ? true : false,
                            urlsrc = WopiUtils.GetActionUrl(hostPath, x.Attribute("urlsrc").Value),
                            requires = (x.Attribute("requires") != null) ? x.Attribute("requires").Value : String.Empty
                        }).ToList()
                    };
                }
            }

            return output;
        }
    }
}
