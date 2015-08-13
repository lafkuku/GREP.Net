using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Grep.Net.Entities;


namespace Grep.Net.Data
{
    public static class SerializationHelper
    {
        private static Type[] knownTypes =
        {
            typeof(Pattern),
            typeof(FileExtension),
            typeof(Template),
            typeof(GrepResult),
            typeof(FileTypeDefinition),
            typeof(PatternPackage)
        };

        private static Type[] xmlTypes =
        {
            typeof(Pattern),
            typeof(PatternPackage),
            typeof(FileTypeDefinition),
            typeof(Template),
            typeof(EntityContainer),
            typeof(GrepResult),
            typeof(Entities.FileInfo),
            typeof(GrepContext)
        };

        public static String GetObjectAsXml<T>(T entity)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                StreamWriter sw = new StreamWriter(ms);
                
                SerializeXml(sw, entity);
                return sw.ToString();
            }
        }

        public static void SerializeIntoXmlFile<T>(string fileName, T o)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(fileName);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
            using (StreamWriter sw = new StreamWriter(File.Open(file.FullName, FileMode.Create)))
            {
                SerializeXml(sw, o);
            }
        }

        public static T DeserializeXmlFromFile<T>(string fileName)
        {
            return (T)DeserializeXmlFromFile(typeof(T), fileName);
        }

        public static object DeserializeXmlFromFile(Type t, string filePath)
        {
            using (FileStream fs = File.Open(filePath, FileMode.Open))
            {
               
                using (var sr = new StreamReader(fs))
                {
                    XmlReaderSettings settings = new XmlReaderSettings();

                    settings.IgnoreWhitespace = true;
                    using (var xtr = new XmlTextReader(sr))
                    {
                        //xtr.Normalization = false;
                        xtr.WhitespaceHandling = WhitespaceHandling.Significant;

                        //Deserialize the collection type. 
                        return DeserializeXml(t, xtr);
                    }
                }
            }
        }

        public static T DeserializeXml<T>(XmlTextReader reader)
        {
            return (T)DeserializeXml(typeof(T), reader);
        }

        public static object DeserializeXml(Type t, XmlTextReader reader)
        {
            try
            {
                var serializer = new XmlSerializer(t, xmlTypes);
                // This will also lazy load all details
                return serializer.Deserialize(reader);
            }
            catch
            {
                //TODO: Throw exception. 
                throw;
            }
        }

        public static void SerializeXml<T>(StreamWriter sw, T entity)
        {
            try
            {

                var serializer = new XmlSerializer(typeof(T), xmlTypes);
                // This will also lazy load all details
                serializer.Serialize(sw, entity);
            }
            catch
            {
                //TODO: Throw exception. 
                throw;
            }
        }

        public static string SerializeAsXml(Object o)
        {
            string json = SerializeAsJSON(o);
            XmlDocument doc = JsonConvert.DeserializeXmlNode(json);
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                doc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }
        }
        public static T DeserializeAsXml<T>(String rawXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(rawXml);

            String json = JsonConvert.SerializeXmlNode(doc);


            return DeserializeObjectFromJSON<T>(json);
        }

        public static string SerializeAsJSON(object o)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();

            return JsonConvert.SerializeObject(o, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            });
        }

        public static T DeserializeObjectFromJSON<T>(string jsonObjString)
        {
            return JsonConvert.DeserializeObject<T>(jsonObjString);
        }
        public static object DeserializeObjectFromJSON(string jsonObjString)
        {
            return JsonConvert.DeserializeObject(jsonObjString);
        }
    }
}