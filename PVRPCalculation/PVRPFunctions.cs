using BlobUtils;
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
            _resultFileName = $"{_requestID}_result.dat";
            _stdOutFileName = $"{_requestID}_stdout.dat";
            _stdErrFileName = $"{_requestID}_stderr.dat";


        }
        public bool Optimize()
        {
            bool resultWasOk = false;
            try
            {
                Console.WriteLine($"--{_requestID} STARTED");
                Console.WriteLine($"-- Parameters RequsestID:{_requestID} maxCompTime:{_maxCompTime}");
                Console.WriteLine($"--Work dir:{_workDir}");



                // NOTODO: download the optimize.dat file from blob store
                Console.WriteLine($"--{_requestID} start download from blobstore");

                downloadFromBlob(System.IO.Path.Combine(_workDir, _optimizefileName), _blobOptimizeFileName);

                Console.WriteLine($"--{_requestID} end download from blobstore");

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
                var resultFileWithPath = System.IO.Path.Combine(_workDir, _resultFileName);
                var stdoutFileWithPath = System.IO.Path.Combine(_workDir, _stdOutFileName);
                var stderrFileWithPath = System.IO.Path.Combine(_workDir, _stdErrFileName);

                resultWasOk = CheckResultFiles(resultFileWithPath, okFileWithPath, errorFileWithPath);

                // NOTODO: upload the result files
                Console.WriteLine($"--{_requestID} start upload to blobstore");
                string blobOkFileName = $"REQ_{_requestID}/{_requestID}_ok.dat";
                string blobErrorFileName = $"REQ_{_requestID}/{_requestID}_error.dat";
                string blobResultFileName = $"REQ_{_requestID}/{_requestID}_result.dat";
                string blobStdOutFileName = $"REQ_{_requestID}/{_requestID}_stdout.dat";
                string blobStdErrFileName = $"REQ_{_requestID}/{_requestID}_stderr.dat";


                uploadToBlob(resultFileWithPath, blobResultFileName);
                uploadToBlob(okFileWithPath, blobOkFileName);
                uploadToBlob(errorFileWithPath, blobErrorFileName);
                uploadToBlob(stdoutFileWithPath, blobStdOutFileName);
                uploadToBlob(stderrFileWithPath, blobStdErrFileName);



                Console.WriteLine($"--{_requestID} end upload to blobstore");
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("--ERROR: {0}", ex.Message));
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

            Console.WriteLine(String.Format("--download file with path: {0}", fileWithPath));
            using (var fileStream = System.IO.File.OpenWrite(fileWithPath))
            {
                var blobStream = _blobHandler.DownloadFromStreamAsync(CalcContainerName, blobFileName).GetAwaiter().GetResult();
                blobStream.CopyTo(fileStream);
            }

        }

        private void uploadToBlob(string fileWithPath, string blobFileName)
        {
            if (File.Exists(fileWithPath))
            {
                Console.WriteLine(String.Format("--upload file with path: {0}", fileWithPath));
                using (var fileStream = System.IO.File.OpenRead(fileWithPath))
                {
                    _blobHandler.UploadAsync(CalcContainerName, blobFileName, fileStream);
                }
            }
        }

        private void CreateInifile(string iniFileName)
        {
            var iniFileWithPath = System.IO.Path.Combine(_workDir, iniFileName);
            Console.WriteLine(String.Format("--ini file with path: {0}", iniFileWithPath));

            var optimizeFileWithPath = System.IO.Path.Combine(_workDir, $"{_requestID}_optimize");
            optimizeFileWithPath = optimizeFileWithPath.Replace("\\", "\\\\");//.Remove(optimizeFileWithPath.LastIndexOf("."));
            Console.WriteLine(String.Format("--optimize file with path: {0}", optimizeFileWithPath));

            var outfileFileWithPath = System.IO.Path.Combine(_workDir, $"{_requestID}_result");
            outfileFileWithPath = outfileFileWithPath.Replace("\\", "\\\\");
            Console.WriteLine(String.Format("--outfile file with path: {0}", outfileFileWithPath));

            var okfileFileWithPath = System.IO.Path.Combine(_workDir, $"{_requestID}_ok");
            okfileFileWithPath = okfileFileWithPath.Replace("\\", "\\\\");
            Console.WriteLine(String.Format("--okfile file with path: {0}", okfileFileWithPath));

            var errorfileFileWithPath = System.IO.Path.Combine(_workDir, $"{_requestID}_error");
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
            string readText = System.IO.File.ReadAllText(iniFileWithPath);
            Console.WriteLine(readText);
        }

        private string CreateBatFile(string batFileName, string iniFileName)
        {
            var batFileWithPath = System.IO.Path.Combine(_workDir, batFileName);
            Console.WriteLine(String.Format("--bat file with path: {0}", batFileWithPath));

            var iniFileWithPath = System.IO.Path.Combine(_workDir, iniFileName);

            //iniFileWithPath = iniFileWithPath.Replace("\\", "\\\\");

            Console.WriteLine(String.Format("--ini file with path: {0}", iniFileWithPath));

            if (!System.IO.File.Exists(batFileWithPath))
            {
                using (System.IO.StreamWriter sw = System.IO.File.CreateText(batFileWithPath))
                {
                    String s = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), PVRP_exe) + " " + _config[PVRPParsParName] + " -f " + iniFileWithPath + " > PVRP.log";
                    sw.WriteLine(s);
                }
            }

            string readText = System.IO.File.ReadAllText(batFileWithPath);
            Console.WriteLine(readText);

            return batFileWithPath;
        }


        private void ExecPVRP(int timeoutMS)
        {
            var iniFileWithPath = System.IO.Path.Combine(_workDir, _iniFileName);

            //iniFileWithPath = iniFileWithPath.Replace("\\", "\\\\");
            Console.WriteLine(String.Format("--ini file with path: {0}", iniFileWithPath));


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
                Console.WriteLine($"--{PVRP_exe} started :{startInfo.FileName} {startInfo.Arguments}");
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

                    Console.WriteLine($"--ProcessMemory setting:{maxWorkingSet}, MaxWorkingSet:{exeProcess.MaxWorkingSet}, WorkingSet64: {exeProcess.WorkingSet64}. MinWorkingSet:{exeProcess.MinWorkingSet} byte");

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

                Console.WriteLine($"--{PVRP_exe} finished! exit code:{exitCode}, requestID:{_requestID}");
                if (timeoutHappened)
                {
                    Console.WriteLine($"--{PVRP_exe} timeout happened! Timeout in ms:{timeoutMS}, requestID:{_requestID}");
                }

                saveStdOut(stdout.ToString(), stderr.ToString());
            }
            catch (Exception ex)
            {
                // Log error.
                Console.WriteLine("--ERROR: {0}: ", ex.Message);
            }
        }

        private void saveStdOut(string stdout, string stderr)
        {

            Console.WriteLine($"--{PVRP_exe} output:{stdout}");
            Console.WriteLine($"--{PVRP_exe} error:{stderr}");

            var stdoutFileWithPath = System.IO.Path.Combine(_workDir, _stdOutFileName);
            var stderrFileWithPath = System.IO.Path.Combine(_workDir, _stdErrFileName);

            File.WriteAllText(stdoutFileWithPath, stdout);
            File.WriteAllText(stderrFileWithPath, stderr);

            Console.WriteLine($"--{PVRP_exe} standard outpup/error saved");

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
