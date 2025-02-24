using Newtonsoft.Json;
using System.IO;
using System.Xml;

namespace VisualFlow.Models
{
    public class ProjectDataManager
    {
        public List<UIElementModel> UIElements { get; set; } = new List<UIElementModel>();

        public void SaveProject(string filePath)
        {
            string json = JsonConvert.SerializeObject(UIElements, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public void LoadProject(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                UIElements = JsonConvert.DeserializeObject<List<UIElementModel>>(json);
            }
        }
    }
}
