using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace HandyControl.Tools;

public static class XmlHelper
{
    /// <summary>
    /// Serializes the data in the object to the designated file path
    /// </summary>
    /// <typeparam name="T">Type of Object to serialize</typeparam>
    /// <param name="dataToSerialize">Object to serialize</param>
    /// <param name="filePath">FilePath for the XML file</param>
    public static void Serialize<T>(T dataToSerialize, string filePath)
    {
        try
        {
            using (Stream stream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                XmlTextWriter writer = new XmlTextWriter(stream, Encoding.Default);
                writer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, dataToSerialize);
                writer.Close();
            }
        }
        catch
        {
            throw;
        }
    }

    /// <summary>
    /// Deserializes the data in the XML file into an object
    /// </summary>
    /// <typeparam name="T">Type of object to deserialize</typeparam>
    /// <param name="filePath">FilePath to XML file</param>
    /// <returns>Object containing deserialized data</returns>
    public static T Deserialize<T>(string filePath)
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T serializedData;

            using (Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                serializedData = (T) serializer.Deserialize(stream);
            }

            return serializedData;
        }
        catch
        {
            throw;
        }
    }
}
