using System;
using System.IO;
using System.Text;
using UnityEngine;

public class FileDataHandler
{
    private string _directoryPath = "";
    private string _fileName = "";

    //암호화해서 저장할꺼냐.
    private bool _encrypt = false;
    private string _codeWord = "ggm_high"; //테스트용 암호화코드
    
    public FileDataHandler(string directoryPath, string fileName, bool encrypt)
    {
        _directoryPath = directoryPath;
        _fileName = fileName;
        _encrypt = encrypt;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(_directoryPath, _fileName);

        try
        {
            Directory.CreateDirectory(_directoryPath);
            string dataToStore = JsonUtility.ToJson(data, true);

            if (_encrypt)
            {
                dataToStore = EncryptAndDeCryptData(dataToStore);
            }
            
            //유징을 쓰면 클로즈를 자동으로 해줘.
            using (FileStream writeStream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(writeStream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error on trying to save data to file {fullPath} \n {ex.Message}");
        }
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(_directoryPath, _fileName);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";   
                
                //유징을 쓰면 클로즈를 자동으로 해줘.
                using (FileStream readStream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(readStream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                if (_encrypt)
                {
                    dataToLoad = EncryptAndDeCryptData(dataToLoad);
                }
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad); //불러온다.
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error on trying to load data to file {fullPath} \n {ex.Message}");
            }
        }
        
        return loadedData;
    }

    public void DeleteSaveData()
    {
        string fullPath = Path.Combine(_directoryPath, _fileName);

        if (File.Exists(fullPath))
        {
            try
            {
                File.Delete(fullPath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error on trying to delete data file : {fullPath} \n {ex.Message}");
            }
        }
    }

    //초간단 XOR암호화
    private string EncryptAndDeCryptData(string data)
    {
        StringBuilder sBuilder = new StringBuilder();

        for (int i = 0; i < data.Length; ++i)
        {
            sBuilder.Append((char)(data[i] ^ _codeWord[i % _codeWord.Length]));
        }

        return sBuilder.ToString();
    }
}
