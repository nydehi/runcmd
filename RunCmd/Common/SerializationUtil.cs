﻿using RunCmd.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RunCmd.Common
{
    /// <summary>
    /// Taken from http://stackoverflow.com/questions/6115721/how-to-save-restore-serializable-object-to-from-file
    /// </summary>
   public static class SerializationUtil
    {
        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="fileName"></param>
        public static void SerializeObject<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                //XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                XmlSerializer serializer = XmlSerializer.FromTypes(new[] { serializableObject.GetType() })[0];
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(fileName);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                //Log exception here
                Utility.WriteToFile(ex.Message + ex.StackTrace, Utility.getAsolutePathForRelativeFileName("log", "Error" + RunCmdConstants.DateTimeStampAsString + ".log"));
            }
        }


        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T DeSerializeObject<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default(T); }

            T objectOut = default(T);

            try
            {
                string attributeXml = string.Empty;

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = XmlSerializer.FromTypes(new[]{outType})[0];
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }
            }
            catch (Exception ex)
            {
                //Log exception here
                Utility.WriteToFile(ex.Message + ex.StackTrace, Utility.getAsolutePathForRelativeFileName("log", "Error" + RunCmdConstants.DateTimeStampAsString + ".log"));
            }

            return objectOut;
        }
    }
}
