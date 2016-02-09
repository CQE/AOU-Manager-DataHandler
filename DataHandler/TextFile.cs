using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace DataHandler
{
    public static class FileLog
    {
        public static void AddLog(string logText)
        {
            // Just for having a breakpoint here at the moment
            string logToDo = logText;
        }
    }

    public class FileResult
    {
        public bool Succeded { get; set; }
        public string ErrorMessage { get; set; }

        public FileResult()
        {
            Succeded = false;
            ErrorMessage = "Unknown Error";
        }
    }

    // Store data from one sensor in one file, named with date, change file every day
    public class TextFile
    {

        StorageFolder dataFolder;

        public string StrData { get; private set; }

        public bool NewTextLoaded { get; private set; }

        public TextFile()
        {
            /*  Most safe to use this. Belongs to User and not App.
                Capabilities for Pictures Library must be enabled in project properties
            */
            dataFolder = KnownFolders.PicturesLibrary;
            StrData = "";
            NewTextLoaded = false;
        }

        private async void SaveToFileAsync(StorageFile f, string content)
        {
            try
            {
                using (StorageStreamTransaction transaction = await f.OpenTransactedWriteAsync())
                {

                    using (DataWriter dataWriter = new DataWriter(transaction.Stream))
                    {
                        dataWriter.WriteString(content);
                        transaction.Stream.Size = await dataWriter.StoreAsync(); // reset stream size to override the file
                        await transaction.CommitAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                FileLog.AddLog("AddToFileAsync Error: " + ex.Message);
            }
        }

        private async void AddToFileAsync(StorageFile f, string lineOfText)
        {
            try
            {
                await FileIO.AppendTextAsync(f, lineOfText + Environment.NewLine);
            }
            catch (Exception ex)
            {
                FileLog.AddLog("AddToFileAsync Error: " + ex.Message);
            }
        }


        private async void CreateFileIfNotExistAndAddTextLine(string subPath, string fileName, string lineOfText)
        {
            try
            {
                var folder = await dataFolder.CreateFolderAsync(subPath, CreationCollisionOption.OpenIfExists);

                StorageFile f = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                AddToFileAsync(f, lineOfText);
            }
            catch (Exception ex)
            {
                FileLog.AddLog("CreateFileIfNotExistAndAddTextLine Error: " + ex.Message);
            }
        }


        private async void CreateFileIfNotExistAndReplaceText(string subPath, string fileName, string text)
        {
            try
            {
                var folder = await dataFolder.CreateFolderAsync(subPath, CreationCollisionOption.OpenIfExists);

                StorageFile f = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                SaveToFileAsync(f, text);
            }
            catch (Exception ex)
            {
                FileLog.AddLog("CreateFileIfNotExistAndReplaceText Error: " + ex.Message);
            }
        }



        /***********************/
        /* public from here    */
        /***********************/
        public async void OpenFileIfExistAndGetText(string fileName)
        {
            try
            {
                StorageFile f = await dataFolder.GetFileAsync(fileName);
                StrData = await FileIO.ReadTextAsync(f);
                NewTextLoaded = true;
            }
            catch (Exception ex)
            {
                FileLog.AddLog("CreateFileIfNotExistAndReplaceText Error: " + ex.Message);
            }
        }

        public FileResult SaveToFile(string subPath, string fileName, string textContent)
        {
            FileResult fhRes = new FileResult();
            CreateFileIfNotExistAndReplaceText(subPath, fileName, textContent);

            return fhRes;
        }

        public FileResult AddToFile(string subPath, string fileName, string textLine)
        {
            FileResult fhRes = new FileResult();
            CreateFileIfNotExistAndAddTextLine(subPath, fileName, textLine);

            return fhRes;
        }

    }
}
