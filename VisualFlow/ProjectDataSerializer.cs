using System.IO;
using System.Xml.Serialization;

namespace VisualFlow
{
    public class ProjectDataSerializer
    {
        public void SaveProject(string filePath, object data)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(data.GetType());
                serializer.Serialize(writer, data);
            }
        }

        public object LoadProject(string filePath, System.Type dataType)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(dataType);
                return serializer.Deserialize(reader);
            }
        }
    }
}
