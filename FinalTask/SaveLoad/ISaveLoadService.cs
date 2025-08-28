namespace FinalTask.SaveLoad
{
    public interface ISaveLoadService<T>
    {
        void SaveData(T data, string id);
        T LoadData(string id);
    }
}
