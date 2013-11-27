using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;


namespace Plugin_Note
{
    class Notes
    {
        public static string DataPath = "Plugins/Note/";
        public static string DataFileName = "Notes.json";
        public static string FullPath
        {
            get { return DataPath + DataFileName; }

        }

        public Dictionary<string, List<NoteEntry>> NotesByServer;

        public Notes ()
        {
            NotesByServer = new Dictionary<string, List<NoteEntry>>();
        }

        public void Save()
        {
            var data = JsonConvert.SerializeObject(this);

            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }

            File.WriteAllText(FullPath, data);
        }

        public static Notes Load()
        {
            var data = File.ReadAllText(FullPath);
            return JsonConvert.DeserializeObject<Notes>(data);
        }
    }
}
