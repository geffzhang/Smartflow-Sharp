﻿/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

namespace Smartflow
{
    public sealed class XmlConfiguration
    {
        public static T ParseXml<T>(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                T o = (T)serializer.Deserialize(sr);
                return o;
            }
        }

        public static T ParseflowXml<T>(string flowXml)
        {
            byte[] buffer = System.Text.ASCIIEncoding.UTF8.GetBytes(flowXml);
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;
                //IgnoreComments
                T o = (T)serializer.Deserialize(ms);
                return o;
            }
        }
    }
}
