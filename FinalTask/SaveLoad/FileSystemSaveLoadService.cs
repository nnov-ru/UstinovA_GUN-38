namespace FinalTask.SaveLoad
{
    public class FileSystemSaveLoadService : ISaveLoadService<string>
    {
        private readonly string _savePath;
        public FileSystemSaveLoadService(string savePath)
        {
            _savePath = savePath;
            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }
        }
        public void SaveData(string data, string id)
        {
            string filePath = Path.Combine(_savePath, $"{id}.txt");
            File.WriteAllText(filePath, data);
        }
        public string LoadData(string id)
        {
            string filePath = Path.Combine(_savePath, $"{id}.txt");
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            else return null;
        }
        //public void DeleteProfile(string id)
        //{
            
        //}
    }
}
