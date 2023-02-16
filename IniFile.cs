using System.Runtime.InteropServices;
using System.Text;

namespace ScannerAPI
{
    class IniFile
    {
        string Path; //Имя файла.

        [DllImport("kernel32")] // Подключаем kernel32.dll и описываем его функцию WritePrivateProfilesString
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32")] // Еще раз подключаем kernel32.dll, а теперь описываем функцию GetPrivateProfileString
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);


        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpszReturnBuffer, int nSize, string lpFileName);
        

        // С помощью конструктора записываем путь до файла и его имя.
        public IniFile(string IniPath)
        {
            Path = new FileInfo(IniPath).FullName.ToString();
        }

        //Читаем ini-файл и возвращаем значение указного ключа из заданной секции.
        public string ReadINI(string Section, string Key)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }
        //Записываем в ini-файл. Запись происходит в выбранную секцию в выбранный ключ.
        public void Write(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, Path);
        }

        //Удаляем ключ из выбранной секции.
        public void DeleteKey(string Key, string Section = null)
        {
            Write(Section, Key, null);
        }
        //Удаляем выбранную секцию
        public void DeleteSection(string Section = null)
        {
            Write(Section, null, null);
        }
        //Проверяем, есть ли такой ключ, в этой секции
        public bool KeyExists(string Key, string Section = null)
        {
            return ReadINI(Section, Key).Length > 0;
        }

        public bool ReadConfigSection(string sectionName,out List<string> Keys)
        {
            List<string> result = new List<string>();
            byte[] buffer = new byte[4096];
            GetPrivateProfileSection(sectionName, buffer, 4096, Path);
            String[] tmp = Encoding.ASCII.GetString(buffer).Trim('\0').Split('\0');
            foreach (String entry in tmp)
            {
                string[] parts = entry.Split('=');
                if (parts.Length == 2)
                {
                    result.Add(parts[0]);
                }
            }
            if (result.Count > 0)
            {
                Keys = result;
                return true;
            }
            else {
                Keys = result;
                return false;
            }

        }

    }
}