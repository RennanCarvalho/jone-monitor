using System.IO;
using Newtonsoft.Json;

namespace jone_fora
{
    public static class Get<T>
    {
        private static string _caminho = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", $"{typeof(T).Name}.json");

        public static List<T> Lista()
        {
            if (!File.Exists(_caminho))
                return new List<T>();

            string json = File.ReadAllText(_caminho);
            return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        }

        public static void Salvar(List<T> dados)
        {
            string json = JsonConvert.SerializeObject(dados, Formatting.Indented);
            File.WriteAllText(_caminho, json);
        }

    }
}
