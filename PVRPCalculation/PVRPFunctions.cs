using Azure.Storage.Blobs.Models;
using BlobUtils;
using CommonUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace WebJobPOC
{
    public class PVRPTimeOutException : Exception
    {
        public PVRPTimeOutException(string message) : base(message) { }
    }

    public class PVRPFunctions
    {
        public static string AzureWebJobsStorageParName = "ConnectionStrings:AzureWebJobsStorage";
        public static string PVRPParsParName = "PVRPPars";
        public static string ProcessMemoryInMBParName = "ProcessMemoryInMB";
        public static string CalcContainerName = "calculations";
        public static string PVRP_exe = "PVRP.exe";


        private string _requestID;
        private int _maxCompTime;
        private string _optimizefileName;
        private string _blobOptimizeFileName;
        private string _iniFileName;
        private string _workDir;
        private string _okFileName;
        private string _finishFileName;
        private string _errorFileName;
        private string _resultFileName;
        private string _stdOutFileName;
        private string _stdErrFileName;

        private readonly IBlobHandler _blobHandler;


        private IConfiguration _config;
        private ILogger _logger;

        public PVRPFunctions(string requestID, int maxCompTime, IConfiguration config, ILogger logger)
        {
            _requestID = requestID;
            _maxCompTime = maxCompTime;
            _config = config;
            _logger = logger;



            _blobHandler = new BlobHandler(_config[AzureWebJobsStorageParName]);

            _optimizefileName = $"{_requestID}_optimize.dat";
            _blobOptimizeFileName = $"REQ_{_requestID}/{_requestID}_optimize.dat";
            _iniFileName = $"{_requestID}_init.cl";


#if RELEASE                
            _workDir = Directory.CreateTempSubdirectory("PVRPTemp").FullName;
#else
            _workDir = $"C:\\local\\Temp\\xx{DateTime.UtcNow.Ticks}";
            Directory.CreateDirectory(_workDir);
#endif

            _okFileName = $"{_requestID}_ok.dat";
            _errorFileName = $"{_requestID}_error.dat";
            _finishFileName = $"{_requestID}_finish.dat";
            _resultFileName = $"{_requestID}_result.dat";
            _stdOutFileName = $"{_requestID}_stdout.dat";
            _stdErrFileName = $"{_requestID}_stderr.dat";

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("hu-HU");

        }
        public bool Optimize()
        {
            bool resultWasOk = false;
            try
            {
                _logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", _requestID, "INFO", $"workdir:{_workDir}");



                // NOTODO: download the optimize.dat file from blob store
                downloadFromBlob(System.IO.Path.Combine(_workDir, _optimizefileName), _blobOptimizeFileName);

                // NOTODO: create the .ini file
                CreateInifile(_iniFileName);

                int ExecTimeOutMS = _maxCompTime + 2000;

                // NOTODO: create the .bat file
                //CreateBatFile(_localPath, batFileName, iniFileName);

                // NOTODO: start the .bat file
                ExecPVRP(ExecTimeOutMS);

                // NOTODO: check  the result file
                var okFileWithPath = System.IO.Path.Combine(_workDir, _okFileName);
                var errorFileWithPath = System.IO.Path.Combine(_workDir, _errorFileName);
                var finishFileWithPath = System.IO.Path.Combine(_workDir, _finishFileName);
                var resultFileWithPath = System.IO.Path.Combine(_workDir, _resultFileName);
                var stdoutFileWithPath = System.IO.Path.Combine(_workDir, _stdOutFileName);
                var stderrFileWithPath = System.IO.Path.Combine(_workDir, _stdErrFileName);

                resultWasOk = CheckResultFiles(resultFileWithPath, okFileWithPath, errorFileWithPath);

                // NOTODO: upload the result files
                _logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", _requestID, "INFO", $"Start upload to blobstore");
                string blobResultFileName = $"REQ_{_requestID}/{_requestID}_result.dat";
                string blobStdOutFileName = $"REQ_{_requestID}/{_requestID}_stdout.dat";
                string blobStdErrFileName = $"REQ_{_requestID}/{_requestID}_stderr.dat";
                string blobOkFileName = $"REQ_{_requestID}/{_requestID}_ok.dat";
                string blobErrorFileName = $"REQ_{_requestID}/{_requestID}_error.dat";
                string blobFinishFileName = $"REQ_{_requestID}/{_requestID}_finish.dat";


                uploadToBlob(resultFileWithPath, blobResultFileName, AccessTier.Hot);
                uploadToBlob(stdoutFileWithPath, blobStdOutFileName);
                uploadToBlob(stderrFileWithPath, blobStdErrFileName);
                uploadToBlob(okFileWithPath, blobOkFileName, AccessTier.Hot);
                uploadToBlob(errorFileWithPath, blobErrorFileName, AccessTier.Hot);
                uploadToBlob(finishFileWithPath, blobFinishFileName, AccessTier.Hot);
                _logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", _requestID, "INFO", $"end uploads to blobstore");
            }
            catch (Exception)
            {
                throw;
            }


            return resultWasOk;
        }

        private void downloadFromBlob(string fileWithPath, string blobFileName)
        {
            if (File.Exists(fileWithPath))
            {
                File.Delete(fileWithPath);
            }

            using (var fileStream = System.IO.File.OpenWrite(fileWithPath))
            {
                var blobStream = _blobHandler.DownloadFromStreamAsync(CalcContainerName, blobFileName).GetAwaiter().GetResult();
                blobStream.CopyTo(fileStream);
            }
            _logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", _requestID, "INFO", $"file has been downloaded:{blobFileName} -> {fileWithPath}");

        }
        private void uploadToBlob(string fileWithPath, string blobFileName, AccessTier? accessTier = null)
        {
            if (File.Exists(fileWithPath))
            {
                using (var fileStream = System.IO.File.OpenRead(fileWithPath))
                {
                    _blobHandler.UploadAsync(CalcContainerName, blobFileName, fileStream, accessTier);
                }
                _logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", _requestID, "INFO", $"file has been uploaded:{fileWithPath} -> {blobFileName}");
            }
            else
            {
                _logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", _requestID, "INFO", $"file not found:{fileWithPath}");
            }
        }

        private void CreateInifile(string iniFileName)
        {
            var iniFileWithPath = System.IO.Path.Combine(_workDir, iniFileName);

            var optimizeFileWithPath = System.IO.Path.Combine(_workDir, $"{_requestID}_optimize");
            optimizeFileWithPath = optimizeFileWithPath.Replace("\\", "\\\\");//.Remove(optimizeFileWithPath.LastIndexOf("."));

            var outfileFileWithPath = System.IO.Path.Combine(_workDir, $"{_requestID}_result");
            outfileFileWithPath = outfileFileWithPath.Replace("\\", "\\\\");

            var okfileFileWithPath = System.IO.Path.Combine(_workDir, $"{_requestID}_ok");
            okfileFileWithPath = okfileFileWithPath.Replace("\\", "\\\\");

            var errorfileFileWithPath = System.IO.Path.Combine(_workDir, $"{_requestID}_error");
            errorfileFileWithPath = errorfileFileWithPath.Replace("\\", "\\\\");

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
            string readText = System.IO.File.ReadAllText(iniFileWithPath);

            _logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", _requestID, "INFO", $"inifile ({iniFileWithPath}):{readText}");
        }

        private string CreateBatFile(string batFileName, string iniFileName)
        {
            var batFileWithPath = System.IO.Path.Combine(_workDir, batFileName);

            var iniFileWithPath = System.IO.Path.Combine(_workDir, iniFileName);

            //iniFileWithPath = iniFileWithPath.Replace("\\", "\\\\");

            if (!System.IO.File.Exists(batFileWithPath))
            {
                using (System.IO.StreamWriter sw = System.IO.File.CreateText(batFileWithPath))
                {
                    String s = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), PVRP_exe) + " " + _config[PVRPParsParName] + " -f " + iniFileWithPath + " > PVRP.log";
                    sw.WriteLine(s);
                }
            }

            string readText = System.IO.File.ReadAllText(batFileWithPath);
            _logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", _requestID, "INFO", $"batfile ({batFileWithPath}):{readText}");

            return batFileWithPath;
        }


        private void ExecPVRP(int timeoutMS)
        {
            var finishMsg = "???";
            var iniFileWithPath = System.IO.Path.Combine(_workDir, _iniFileName);

            //iniFileWithPath = iniFileWithPath.Replace("\\", "\\\\");


            var fullExeFileName = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), PVRP_exe);
            var arguments = $"{_config[PVRPParsParName]}  -f  " + iniFileWithPath;
            //                            + " > " + System.IO.Path.Combine(_workDir, "stdout.datX");


            //fullExeFileName = "dir *.*";
            //arguments = "";
            //var startInfo = new ProcessStartInfo("cmd.exe", "/c " + fullExeFileName + " " + arguments);

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = fullExeFileName;
            startInfo.Arguments = arguments;
            startInfo.WorkingDirectory = _workDir;


            /* BAT file 
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = true;
            //string p = @"%WEBROOT_PATH%\App_Data\jobs\%WEBJOBS_TYPE%\%WEBJOBS_NAME%";
            ///var batFileWithPath = System.IO.Path.Combine(_localPath, batFileName);
            //var batFileWithPath = System.IO.Path.Combine(p, batFileName);
            ///Console.WriteLine(String.Format("--bat file with path: {0}", batFileWithPath));
            //startInfo.FileName = "P-VRP.bat";// batFileWithPath;
            */


            /*
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = true;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            */

            /*output redirect:
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            */

            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            // startInfo.WindowStyle = ProcessWindowStyle.Normal;

            try
            {
                _logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", _requestID, "INFO", $"--{PVRP_exe} started :{startInfo.FileName} {startInfo.Arguments}");



                // Start the process with the info we specified.
                // Call WaitForExit and then the using-statement will close.


                var stdout = new StringBuilder();
                var stderr = new StringBuilder();
                int exitCode;
                var timeoutHappened = false;
                using (Process exeProcess = Process.Start(startInfo))
                {
                    var maxWorkingSet = Int64.Parse("0" + _config[ProcessMemoryInMBParName].Trim()) * 1024 * 1024;
                    if (maxWorkingSet > 0)
                    {
                        exeProcess.MaxWorkingSet = (nint)Math.Max(maxWorkingSet, exeProcess.MinWorkingSet);
                    }

                    _logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", _requestID, "INFO", $"ProcessMemory setting:{maxWorkingSet}, MaxWorkingSet:{exeProcess.MaxWorkingSet}, WorkingSet64: {exeProcess.WorkingSet64}. MinWorkingSet:{exeProcess.MinWorkingSet} byte");

                    exeProcess.OutputDataReceived += (sender, eventArgs) =>
                    {
                        if (eventArgs.Data != null)
                        {
                            stdout.AppendLine(eventArgs.Data);
                        }
                    };
                    exeProcess.BeginOutputReadLine();

                    exeProcess.ErrorDataReceived += (sender, eventArgs) =>
                    {
                        stderr.AppendLine(eventArgs.Data);
                    };
                    exeProcess.BeginErrorReadLine();

                    if (!exeProcess.WaitForExit(timeoutMS))
                    {
                        timeoutHappened = true;
                        exeProcess.Kill(true);
                    }
                    exitCode = exeProcess.ExitCode;
                }

                //Megjegyzés: A _logger.LogInformation-ba ne a finishMsg -t küldjük, mert a AppInsightsMsgTemplate-ben lévő named placehldereket használjuk!
                finishMsg = $"PVRP {_requestID} INFO {PVRP_exe} finished! exit code:{exitCode}";

                _logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", _requestID, "INFO", $"{PVRP_exe} finished! exit code:{exitCode}");
                if (timeoutHappened)
                {
                    finishMsg = $"PVRP {_requestID} INFO {PVRP_exe} timeout happened! Timeout in ms:{timeoutMS}";
                    _logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", _requestID, "INFO", $"{PVRP_exe} timeout happened! Timeout in ms:{timeoutMS}");
                }
            }
            catch (Exception ex)
            {
                // Log error.
                finishMsg = $"PVRP {_requestID} EXCEPTION {PVRP_exe} exception happened! {ex.Message}";
                _logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", _requestID, "EXCEPTION", $"{PVRP_exe} exception happened! {ex.Message}");
            }

            var finishFileWithPath = System.IO.Path.Combine(_workDir, _finishFileName);
            using (System.IO.StreamWriter sw = System.IO.File.CreateText(finishFileWithPath))
            {
                sw.WriteLine(finishMsg);
            }
        }

        private void saveStdOut(string stdout, string stderr)
        {

            _logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", _requestID, "INFO", $"{PVRP_exe} output:{stdout}");
            _logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", _requestID, "INFO", $"{PVRP_exe} stderr:{stderr}");

            var stdoutFileWithPath = System.IO.Path.Combine(_workDir, _stdOutFileName);
            var stderrFileWithPath = System.IO.Path.Combine(_workDir, _stdErrFileName);

            File.WriteAllText(stdoutFileWithPath, stdout);
            File.WriteAllText(stderrFileWithPath, stderr);

            _logger.LogInformation(Consts.AppInsightsMsgTemplate, "PVRP", _requestID, "INFO", $"{PVRP_exe} standard outpup/error saved");

        }

        private bool CheckResultFiles(string resultFileWithPath, string okFileWithPath, string errorFileWithPath)
        {
            bool res = false;

            if (System.IO.File.Exists(resultFileWithPath) && System.IO.File.Exists(okFileWithPath) && System.IO.File.Exists(errorFileWithPath))
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
