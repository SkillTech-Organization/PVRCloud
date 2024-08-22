using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WebJobPOC
{
    public class PVRPFunctions
    {
        public static string AzureWebJobsStorageParName = "AzureWebJobsStorage";
        public static string CalcContainerName = "calculations";


        private int _requestID;
        private int _maxCompTime;
        private string _fileName;
        private string _blobFileName;
        private string _iniFileName;
        private string _localPath;
        private IConfiguration _config;
        private ILogger _logger;

        public PVRPFunctions(int requestID, int maxCompTime, IConfiguration config, ILogger logger)
        {
            _requestID = requestID;
            _maxCompTime = maxCompTime;
            _config = config;
            _logger = logger;

            _fileName = $"{_requestID}_optimize.dat";
            _blobFileName = $"REQ_{_requestID}/{_requestID}_optimize.dat";
            _iniFileName = $"{_requestID}_init.cl";
            _localPath = Directory.CreateTempSubdirectory("PVRPTemp").FullName;
        }
        public bool Optimize()
        {

            Console.WriteLine($"--{_requestID} STARTED");
            Console.WriteLine($"-- Parameters RequsestID:{_requestID} maxCompTime:{_maxCompTime}");


            // NOTODO: download the optimize.dat file from blob store
            Console.WriteLine($"--{_requestID} start download from blobstore");

            DownloadFromBlobStore(_blobFileName, _fileName);

            Console.WriteLine($"--{_requestID} end download from blobstore");

            // NOTODO: create the .ini file
            CreateInifile(_iniFileName);

            int ExecTimeOutMS = _maxCompTime + 2000;

            // NOTODO: create the .bat file
            //CreateBatFile(_localPath, batFileName, iniFileName);

            // NOTODO: start the .bat file
            ExecPVRP(ExecTimeOutMS);

            // NOTODO: check  the result file
            string okFileName = $"{_requestID}_ok.dat";
            string errorFileName = $"{_requestID}_error.dat";
            string resultFileName = $"{_requestID}_result.dat";
            var okFileWithPath = System.IO.Path.Combine(_localPath, okFileName);
            var errorFileWithPath = System.IO.Path.Combine(_localPath, errorFileName);
            var resultFileWithPath = System.IO.Path.Combine(_localPath, resultFileName);

            bool resultWasOk = CheckResultFiles(resultFileWithPath, okFileWithPath, errorFileWithPath);

            // NOTODO: upload the result files
            Console.WriteLine($"--{_requestID} start upload to blobstore");
            string blobOkFileName = $"REQ_{_requestID}/{_requestID}_ok.dat";
            string blobErrorFileName = $"REQ_{_requestID}/{_requestID}_error.dat";
            string blobResultFileName = $"REQ_{_requestID}/{_requestID}_result.dat";
            UploadToBlobStore(resultFileWithPath, okFileWithPath, errorFileWithPath, blobResultFileName, blobOkFileName, blobErrorFileName);
            Console.WriteLine($"--{_requestID} end upload to blobstore");

            return resultWasOk;
        }


        private void UploadToBlobStore(string resultFileWithPath, string okFileWithPath, string errorFileWithPath, string resultFileName, string okFileName, string errorFileName)
        {
            try
            {
                // Retrieve storage account from connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_config[AzureWebJobsStorageParName]);

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobClient.GetContainerReference(CalcContainerName);

                if (File.Exists(resultFileWithPath))
                {
                    CloudBlockBlob blockBlob_result = container.GetBlockBlobReference(resultFileName);
                    Console.WriteLine(String.Format("--upload file with path: {0}", resultFileWithPath));
                    // Save file to blob content.
                    using (var fileStream = System.IO.File.OpenRead(resultFileWithPath))
                    {
                        blockBlob_result.UploadFromStreamAsync(fileStream).GetAwaiter().GetResult();
                    }
                }

                if (File.Exists(okFileWithPath))
                {
                    CloudBlockBlob blockBlob_ok = container.GetBlockBlobReference(okFileName);
                    Console.WriteLine(String.Format("--upload file with path: {0}", okFileWithPath));
                    // Save file to blob content.
                    using (var fileStream = System.IO.File.OpenRead(okFileWithPath))
                    {
                        blockBlob_ok.UploadFromStreamAsync(fileStream).GetAwaiter().GetResult();
                    }
                }

                if (File.Exists(errorFileWithPath))
                {
                    CloudBlockBlob blockBlob_error = container.GetBlockBlobReference(errorFileName);
                    Console.WriteLine(String.Format("--upload file with path: {0}", errorFileWithPath));
                    // Save file to blob content.
                    using (var fileStream = System.IO.File.OpenRead(errorFileWithPath))
                    {
                        blockBlob_error.UploadFromStreamAsync(fileStream).GetAwaiter().GetResult();
                    }
                }

                DeleteFilesAndFoldersRecursively(_localPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("--ERROR: {0}", ex.Message));
                throw;
            }

        }


        private void DownloadFromBlobStore(string blobFileName, string fileName)
        {
            try
            {
                // Retrieve storage account from connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_config[AzureWebJobsStorageParName]);

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobClient.GetContainerReference(CalcContainerName);

                // Retrieve reference to a blob named "photo1.jpg".
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobFileName);

                var dwnlFileWithPath = System.IO.Path.Combine(_localPath, fileName);

                Console.WriteLine(String.Format("--downloaded file with path: {0}", dwnlFileWithPath));

                // Save blob contents to a file.
                using (var fileStream = System.IO.File.OpenWrite(dwnlFileWithPath))
                {
                    blockBlob.DownloadToStreamAsync(fileStream).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("--ERROR: {0}", ex.Message));
                throw;
            }

        }

        private void CreateInifile(string iniFileName)
        {
            var iniFileWithPath = System.IO.Path.Combine(_localPath, iniFileName);
            Console.WriteLine(String.Format("--ini file with path: {0}", iniFileWithPath));

            var optimizeFileWithPath = System.IO.Path.Combine(_localPath, $"{_requestID}_optimize");
            optimizeFileWithPath = optimizeFileWithPath.Replace("\\", "\\\\");//.Remove(optimizeFileWithPath.LastIndexOf("."));
            Console.WriteLine(String.Format("--optimize file with path: {0}", optimizeFileWithPath));

            var outfileFileWithPath = System.IO.Path.Combine(_localPath, $"{_requestID}_result");
            outfileFileWithPath = outfileFileWithPath.Replace("\\", "\\\\");
            Console.WriteLine(String.Format("--outfile file with path: {0}", outfileFileWithPath));

            var okfileFileWithPath = System.IO.Path.Combine(_localPath, $"{_requestID}_ok");
            okfileFileWithPath = okfileFileWithPath.Replace("\\", "\\\\");
            Console.WriteLine(String.Format("--okfile file with path: {0}", okfileFileWithPath));

            var errorfileFileWithPath = System.IO.Path.Combine(_localPath, $"{_requestID}_error");
            errorfileFileWithPath = errorfileFileWithPath.Replace("\\", "\\\\");
            Console.WriteLine(String.Format("--errorfile file with path: {0}", errorfileFileWithPath));

            if (!System.IO.File.Exists(iniFileWithPath))
            {
                // Create a file to write to.
                //D:\local\Temp\jobs\triggered\CalcEngineCalculatorWebJob\lbs3gzgd.jev\636416315670407114_optimize.dat 
                using (System.IO.StreamWriter sw = System.IO.File.CreateText(iniFileWithPath))
                {
                    String s_ini = "(INFILE := \"" + optimizeFileWithPath + "\",";
                    sw.WriteLine(s_ini);
                    String s_out = "OUTFILE := \"" + outfileFileWithPath + "\",";
                    sw.WriteLine(s_out);
                    String s_ok = "OKFILE := \"" + okfileFileWithPath + "\",";
                    sw.WriteLine(s_ok);
                    String s_error = "ERRORFILE := \"" + errorfileFileWithPath + "\",";
                    sw.WriteLine(s_error);
                    String s1 = "MAXCOMPTIME := " + _maxCompTime + ",";
                    //            String s2 = "NOWAIT? := false,";
                    String s2 = "NOWAIT? := true,";
                    String s3 = "MULTITOURS? := false,";
                    String s4 = "go(),";
                    String s5 = "exit(1)";
                    String s6 = ")";
                    sw.WriteLine(s1);
                    sw.WriteLine(s2);
                    sw.WriteLine(s3);
                    sw.WriteLine(s4);
                    sw.WriteLine(s5);
                    sw.WriteLine(s6);
                }
            }
            //            (INFILE := ".\\optimize",
            //  OUTFILE:= ".\\result", 
            //  OKFILE:= ".\\ok",
            //  ERRORFILE:= ".\\error",
            //  MAXCOMPTIME:= 120000000,
            //  NOWAIT ? := false,
            //  MULTITOURS ? := false,
            //  go(),
            //  exit(1)
            //)
            string readText = System.IO.File.ReadAllText(iniFileWithPath);
            Console.WriteLine(readText);
        }

        private void ExecPVRP(int timeoutMS)
        {
            var iniFileWithPath = System.IO.Path.Combine(_localPath, _iniFileName);
            iniFileWithPath = iniFileWithPath.Replace("\\", "\\\\");
            Console.WriteLine(String.Format("--ini file with path: {0}", iniFileWithPath));

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;

            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = true;

            //string p = @"%WEBROOT_PATH%\App_Data\jobs\%WEBJOBS_TYPE%\%WEBJOBS_NAME%";
            ///var batFileWithPath = System.IO.Path.Combine(_localPath, batFileName);
            //var batFileWithPath = System.IO.Path.Combine(p, batFileName);
            ///Console.WriteLine(String.Format("--bat file with path: {0}", batFileWithPath));
            //startInfo.FileName = "P-VRP.bat";// batFileWithPath;
            startInfo.FileName = "PVRP.exe";// batFileWithPath;
            startInfo.Arguments = "-s 9 9  -f  " + iniFileWithPath;

            startInfo.WindowStyle = ProcessWindowStyle.Normal;

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using-statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    Task.Factory.StartNew(() =>
                        {
                            if (exeProcess != null)
                            {
                                Thread.Sleep(timeoutMS - 1000);
                                if (!exeProcess.HasExited)
                                {
                                    exeProcess.Kill();
                                }
                            }
                        });

                    exeProcess.WaitForExit(timeoutMS);
                }
                Console.WriteLine("PRVP.EXE finished!");
            }
            catch (Exception ex)
            {
                // Log error.
                Console.WriteLine("--ERROR: {0}: ", ex.Message);
            }
        }

        private bool CheckResultFiles(string resultFileWithPath, string okFileWithPath, string errorFileWithPath)
        {
            bool res = false;

            if ((System.IO.File.Exists(resultFileWithPath)) && (System.IO.File.Exists(okFileWithPath))
                && ((System.IO.File.Exists(errorFileWithPath))))
            {
                res = true;
            }

            return res;
        }

        private void DeleteFilesAndFoldersRecursively(string target_dir)
        {
            foreach (string file in Directory.GetFiles(target_dir))
            {
                File.Delete(file);
            }

            foreach (string subDir in Directory.GetDirectories(target_dir))
            {
                DeleteFilesAndFoldersRecursively(subDir);
            }

            Thread.Sleep(1); // This makes the difference between whether it works or not. Sleep(0) is not enough.
            Directory.Delete(target_dir);
        }

    }
}
