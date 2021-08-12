using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_indexer.Models
{
    public class SettingsField
    {
        public string Enabled { get; set; }
        public string Name { get; set; }
        public string Sticky { get; set; }
        public string Required { get; set; }
        public string Filename { get; set; }

        public void TempAssign(string enabled, string name, string sticky, string required, string filename)
        {
            this.Enabled = enabled;
            if (name.Length == 0)
                this.Name = "null";
            else
                this.Name = name;
            this.Sticky = sticky;
            this.Required = required;
            this.Filename = filename;
        }
    }

    public class Settings
    {
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("Project id")]
        public string ProjectID { get; set; }
        [JsonProperty("File renaming")]
        public string FileRenaming { get; set; }
        [JsonProperty("Fields")]
        public Dictionary<string, SettingsField> Fields { get; set; }
    }
}
