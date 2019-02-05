using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using TAT001.Common;

namespace TAT001.Services
{
    public class Files
    {

        public string createDir(string path, string documento, string ejercicio)
        {

            string ex = "";

            // Specify the path to save the uploaded file to.
            string savePath = path + ejercicio + "\\" + documento + "\\";//RSG 01.08.2018

            // Create the path and file name to check for duplicates.
            string pathToCheck = savePath;
            Log.Info(pathToCheck);
            try
            {
                string saveFileDev = ConfigurationManager.AppSettings["saveFileDev"];
                if (!System.IO.File.Exists(pathToCheck))
                {
                    //No existe, se necesita crear
                    if (saveFileDev=="1") {
                        Directory.CreateDirectory(pathToCheck);
                    }
                    else
                    {
                        string serverDocs = ConfigurationManager.AppSettings["serverDocs"],
                        serverDocsUser = ConfigurationManager.AppSettings["serverDocsUser"],
                        serverDocsPass = ConfigurationManager.AppSettings["serverDocsPass"];
                        using (Impersonation.LogonUser(serverDocs, serverDocsUser, serverDocsPass, LogonType.NewCredentials))
                        {
                            Directory.CreateDirectory(pathToCheck);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorLogApp(e,"Files", "createDir");
                ex = "No se puede crear el directorio para guardar los archivos";
            }

            return ex;
        }

        public string SaveFile(HttpPostedFileBase file, string path, string documento, out string exception, out string pathsaved, string ejercicio)
        {
            string ex = "";
            //string exdir = "";
            // Get the name of the file to upload.
            string fileName = Path.GetFileName(file.FileName); // must be declared in the class above

            // Specify the path to save the uploaded file to.
            //string savePath = path + documento + "\\";//RSG 01.08.2018
            string savePath = path + ejercicio + "\\" + documento + "\\";//RSG 01.08.2018

            // Create the path and file name to check for duplicates.
            string pathToCheck = savePath;

            // Append the name of the file to upload to the path.
            savePath += fileName;

            string saveFileDev = ConfigurationManager.AppSettings["saveFileDev"];
            try
            {
                if (saveFileDev == "1")
                {
                    //Guardar el archivo
                    file.SaveAs(savePath);
                }
                else
                {
                    //file to domain
                    //Parte para guardar archivo en el servidor
                    string serverDocs = ConfigurationManager.AppSettings["serverDocs"],
                          serverDocsUser = ConfigurationManager.AppSettings["serverDocsUser"],
                          serverDocsPass = ConfigurationManager.AppSettings["serverDocsPass"];
                    using (Impersonation.LogonUser(serverDocs, serverDocsUser, serverDocsPass, LogonType.NewCredentials))
                    {  
                        //Guardar el archivo
                        file.SaveAs(savePath);
                    }
                }
            }
            catch (Exception e)
            {
                ex = "";
                ex = fileName;
                Log.ErrorLogApp(e, "Files", "SaveFile");
            }
            
            pathsaved = savePath;
            exception = ex;
            return fileName;
        }

    }
}