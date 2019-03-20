using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Smartflow.Elements
{
    public abstract class ElementAttribute : Element
    {
        [JsonProperty("name")]
        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [JsonProperty("id")]
        [XmlAttribute("id")]
        public string ID
        {
            get;
            set;
        }
    }
}
